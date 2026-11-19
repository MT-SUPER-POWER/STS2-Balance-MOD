import { setTimeout as delay } from "node:timers/promises";
import { fileURLToPath } from "node:url";

const steamLibraryPath = fileURLToPath(
  new URL("../tools/WorkshopPublisher/native/steam_api64.dll", import.meta.url),
);
const invalidUpdateHandle = 0xffffffffffffffffn;
const submitItemUpdateCallback = 3404;
const submitTimeoutMs = 10 * 60 * 1000;
const progressLabels = [
  "等待 Steam",
  "准备配置",
  "准备内容",
  "上传内容",
  "上传预览图",
  "提交更新",
];
const resultLabels = {
  8: "参数无效，请检查标题、介绍、标签和预览图",
  9: "找不到创意工坊条目或上传文件",
  15: "访问被拒绝，请确认 Steam 账号有权更新此条目",
  16: "Steam 响应超时，请先在创意工坊页面核对结果",
  20: "Steam 服务暂不可用",
  24: "当前 Steam 账号权限不足",
  25: "达到 Steam 限额",
};

function bindSteamworks(koffi) {
  const library = koffi.load(steamLibraryPath);
  // These types and interface versions match the Windows x64 Steamworks DLL in the repository.
  const tagList = koffi.struct({ strings: "const char **", count: "int32_t" });
  const submitResult = koffi.struct({
    result: "int32_t",
    needsAgreement: "bool",
    itemId: "uint64_t",
  });
  const ugc = (name, result, parameters) =>
    library.func(`SteamAPI_ISteamUGC_${name}`, result, [
      "void *",
      ...parameters,
    ]);
  return {
    init: library.func("int SteamAPI_InitFlat(void *errorMessage)"),
    shutdown: library.func("void SteamAPI_Shutdown()"),
    runCallbacks: library.func("void SteamAPI_RunCallbacks()"),
    getUgc: library.func("void *SteamAPI_SteamUGC_v020()"),
    getUtils: library.func("void *SteamAPI_SteamUtils_v010()"),
    getAppId: library.func(
      "uint32_t SteamAPI_ISteamUtils_GetAppID(void *utils)",
    ),
    startUpdate: ugc("StartItemUpdate", "uint64_t", ["uint32_t", "uint64_t"]),
    setContent: ugc("SetItemContent", "bool", ["uint64_t", "str"]),
    setLanguage: ugc("SetItemUpdateLanguage", "bool", ["uint64_t", "str"]),
    setTitle: ugc("SetItemTitle", "bool", ["uint64_t", "str"]),
    setDescription: ugc("SetItemDescription", "bool", ["uint64_t", "str"]),
    setPreview: ugc("SetItemPreview", "bool", ["uint64_t", "str"]),
    setTags: ugc("SetItemTags", "bool", [
      "uint64_t",
      koffi.pointer(tagList),
      "bool",
    ]),
    setVisibility: ugc("SetItemVisibility", "bool", ["uint64_t", "int32_t"]),
    submitUpdate: ugc("SubmitItemUpdate", "uint64_t", ["uint64_t", "str"]),
    getProgress: ugc("GetItemUpdateProgress", "int32_t", [
      "uint64_t",
      koffi.out("uint64_t *"),
      koffi.out("uint64_t *"),
    ]),
    isCallComplete: library.func(
      "bool SteamAPI_ISteamUtils_IsAPICallCompleted(void *utils, uint64_t call, _Out_ bool *failed)",
    ),
    getCallResult: library.func(
      "bool SteamAPI_ISteamUtils_GetAPICallResult(void *utils, uint64_t call, void *result, int size, int callback, _Out_ bool *failed)",
    ),
    getCallFailure: library.func(
      "int SteamAPI_ISteamUtils_GetAPICallFailureReason(void *utils, uint64_t call)",
    ),
    readSubmitResult(utils, call) {
      const buffer = Buffer.alloc(koffi.sizeof(submitResult));
      const failed = [false];
      if (
        !this.getCallResult(
          utils,
          call,
          buffer,
          buffer.length,
          submitItemUpdateCallback,
          failed,
        ) ||
        failed[0]
      ) {
        throw new Error(
          `无法读取 Steam 提交结果（API 错误 ${this.getCallFailure(utils, call)}）。`,
        );
      }
      return koffi.decode(buffer, submitResult);
    },
  };
}

function applyUpdate(api, ugc, handle, update) {
  function set(label, setter, ...args) {
    if (!setter(ugc, handle, ...args)) {
      throw new Error(`Steam 拒绝设置${label}，本次更新未提交。`);
    }
  }

  if (update.contentPath !== undefined)
    set("内容目录", api.setContent, update.contentPath);
  if (update.language !== undefined)
    set("文案语言", api.setLanguage, update.language);
  if (update.title !== undefined) set("标题", api.setTitle, update.title);
  if (update.description !== undefined)
    set("介绍", api.setDescription, update.description);
  if (update.previewPath !== undefined)
    set("预览图", api.setPreview, update.previewPath);
  if (update.visibility !== undefined)
    set("可见性", api.setVisibility, update.visibility);
  if (update.tags !== undefined) {
    set(
      "标签",
      api.setTags,
      { strings: update.tags, count: update.tags.length },
      false,
    );
  }
}

async function waitForResult(api, ugc, utils, updateHandle, call) {
  const deadline = performance.now() + submitTimeoutMs;
  const failed = [false];
  const bytesDone = [0n];
  const bytesTotal = [0n];
  let lastProgress;

  while (true) {
    api.runCallbacks();
    if (api.isCallComplete(utils, call, failed)) {
      if (failed[0]) {
        throw new Error(
          `Steam 未确认提交结果（API 错误 ${api.getCallFailure(utils, call)}），请先在创意工坊页面核对结果。`,
        );
      }
      return api.readSubmitResult(utils, call);
    }
    if (performance.now() >= deadline) {
      throw new Error(
        "等待 Steam 确认超过 10 分钟，请先在创意工坊页面核对结果。",
      );
    }

    const status = api.getProgress(ugc, updateHandle, bytesDone, bytesTotal);
    const total = BigInt(bytesTotal[0]);
    const percent =
      total > 0n ? ` ${(BigInt(bytesDone[0]) * 100n) / total}%` : "";
    const progress = `${progressLabels[status] ?? `状态 ${status}`}${percent}`;
    if (progress !== lastProgress) {
      console.log(`[Steam] ${progress}`);
      lastProgress = progress;
    }
    await delay(500);
  }
}

export async function publishUpdates({ appId, itemId, updates }) {
  if (process.platform !== "win32" || process.arch !== "x64") {
    throw new Error("Steam 上传需要 Windows x64 和 x64 版 Node.js。");
  }
  const { default: koffi } = await import("koffi");
  const previousAppId = process.env.SteamAppId;
  const confirmed = [];
  let shutdown;
  process.env.SteamAppId = String(appId);

  try {
    const api = bindSteamworks(koffi);
    const steamError = Buffer.alloc(1024);
    const initResult = api.init(steamError);
    if (initResult !== 0) {
      const details = steamError.toString("utf8").split("\0")[0];
      throw new Error(
        `无法初始化 Steam（${initResult}）。请先运行并登录 Steam 客户端。${details ? ` ${details}` : ""}`,
      );
    }
    shutdown = api.shutdown;

    const ugc = api.getUgc();
    const utils = api.getUtils();
    if (!ugc || !utils || api.getAppId(utils) !== appId) {
      throw new Error(`无法访问应用 ${appId} 的 Steam Workshop 接口。`);
    }

    for (const update of updates) {
      const label = update.language ?? "模组内容";
      console.log(`[Steam] 提交 ${label}`);
      const handle = BigInt(api.startUpdate(ugc, appId, itemId));
      if (handle === invalidUpdateHandle || handle === 0n) {
        throw new Error(`无法为创意工坊条目 ${itemId} 开始更新。`);
      }
      applyUpdate(api, ugc, handle, update);
      const call = BigInt(
        api.submitUpdate(ugc, handle, update.changeNote ?? null),
      );
      if (call === 0n)
        throw new Error("Steam 未返回提交确认句柄，请先核对创意工坊页面。");

      const result = await waitForResult(api, ugc, utils, handle, call);
      if (result.result !== 1) {
        const reason = resultLabels[result.result];
        throw new Error(
          `Steam 提交失败（EResult ${result.result}）${reason ? `，${reason}` : ""}。`,
        );
      }
      if (BigInt(result.itemId) !== itemId) {
        throw new Error(
          `Steam 返回的条目 ${result.itemId} 与目标 ${itemId} 不一致。`,
        );
      }
      confirmed.push(label);
      console.log(`[Steam] 已确认 ${label}`);
      if (result.needsAgreement) {
        throw new Error(
          "需要先在 Steam 接受创意工坊协议。https://steamcommunity.com/sharedfiles/workshoplegalagreement",
        );
      }
    }
  } catch (error) {
    if (confirmed.length > 0) {
      throw new Error(
        `Steam 已确认 ${confirmed.length}/${updates.length} 批更新（${confirmed.join("、")}）。${error.message}`,
        {
          cause: error,
        },
      );
    }
    throw error;
  } finally {
    try {
      shutdown?.();
    } finally {
      if (previousAppId === undefined) delete process.env.SteamAppId;
      else process.env.SteamAppId = previousAppId;
    }
  }
}
