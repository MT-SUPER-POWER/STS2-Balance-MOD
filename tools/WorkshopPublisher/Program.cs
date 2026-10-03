using System.Text;
using System.Text.Json;
using Steamworks;

namespace WorkshopPublisher;

internal static class Program
{
    private const uint AppId = 2868840; // Slay the Spire 2

    public static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("=================================================");
        Console.WriteLine("    STS2 Steam Workshop Publisher CLI (v1.0.0)   ");
        Console.WriteLine("=================================================");

        // Parse CLI arguments
        var workspaceDir = "workshop";
        var contentOnly = false;
        var metaOnly = false;
        var dryRun = false;
        string? targetLang = null; // null = all, or "schinese", "english"

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i].ToLowerInvariant();
            if (arg is "-w" or "--workspace" && i + 1 < args.Length)
            {
                workspaceDir = args[++i];
            }
            else if (arg is "-c" or "--content-only")
            {
                contentOnly = true;
            }
            else if (arg is "-m" or "--meta-only")
            {
                metaOnly = true;
            }
            else if (arg is "--dry-run")
            {
                dryRun = true;
            }
            else if (arg is "--lang" && i + 1 < args.Length)
            {
                targetLang = args[++i].ToLowerInvariant();
            }
            else if (arg is "-h" or "--help")
            {
                PrintHelp();
                return 0;
            }
        }

        var workspacePath = Path.GetFullPath(workspaceDir);
        if (!Directory.Exists(workspacePath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] Workspace directory not found: {workspacePath}");
            Console.ResetColor();
            return 1;
        }

        var modIdFile = Path.Combine(workspacePath, "mod_id.txt");
        if (!File.Exists(modIdFile))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] mod_id.txt not found in workspace: {modIdFile}");
            Console.ResetColor();
            return 1;
        }

        var modIdStr = (await File.ReadAllTextAsync(modIdFile)).Trim();
        if (!ulong.TryParse(modIdStr, out var publishedFileId) || publishedFileId == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] Invalid publishedFileId in mod_id.txt: '{modIdStr}'");
            Console.ResetColor();
            return 1;
        }

        var workshopJsonFile = Path.Combine(workspacePath, "workshop.json");
        if (!File.Exists(workshopJsonFile))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] workshop.json not found: {workshopJsonFile}");
            Console.ResetColor();
            return 1;
        }

        var jsonText = await File.ReadAllTextAsync(workshopJsonFile);
        using var jsonDoc = JsonDocument.Parse(jsonText);
        var root = jsonDoc.RootElement;

        var titleZh = GetString(root, "title") ?? "STS2 Balance MOD";
        var titleEn = GetString(root, "title_en") ?? titleZh;
        var changeNote = GetString(root, "changeNote") ?? "";
        var visibilityStr = GetString(root, "visibility") ?? "public";

        // Read description: prefer WORKSHOP_DESCRIPTION.md if exists
        var descZhFile = Path.Combine(workspacePath, "WORKSHOP_DESCRIPTION.md");
        string descZh = File.Exists(descZhFile)
            ? await File.ReadAllTextAsync(descZhFile)
            : (GetString(root, "description") ?? "");

        var descEnFile = Path.Combine(workspacePath, "WORKSHOP_DESCRIPTION_EN.md");
        string descEn = File.Exists(descEnFile)
            ? await File.ReadAllTextAsync(descEnFile)
            : (GetString(root, "description_en") ?? descZh);

        // Tags and Dependencies
        var tags = new List<string>();
        if (root.TryGetProperty("tags", out var tagsEl) && tagsEl.ValueKind == JsonValueKind.Array)
        {
            foreach (var t in tagsEl.EnumerateArray())
            {
                if (t.GetString() is { } tagStr && !string.IsNullOrWhiteSpace(tagStr))
                    tags.Add(tagStr);
            }
        }

        var dependencies = new List<ulong>();
        if (root.TryGetProperty("dependencies", out var depsEl) && depsEl.ValueKind == JsonValueKind.Array)
        {
            foreach (var d in depsEl.EnumerateArray())
            {
                if (d.TryGetUInt64(out var depId))
                    dependencies.Add(depId);
            }
        }

        // Validate lengths
        var descZhBytes = Encoding.UTF8.GetByteCount(descZh);
        var descEnBytes = Encoding.UTF8.GetByteCount(descEn);
        Console.WriteLine($"[Config] Item ID: {publishedFileId}");
        Console.WriteLine($"[Config] Mode: {(contentOnly ? "Content Only" : metaOnly ? "Metadata Only" : "Full Update")}");
        Console.WriteLine($"[Config] Target Language: {(targetLang ?? "Bilingual (schinese + english)")}");
        Console.WriteLine($"[Config] Title (ZH): {titleZh}");
        Console.WriteLine($"[Config] Description (ZH): {descZhBytes} bytes (Limit: 8000)");
        Console.WriteLine($"[Config] Description (EN): {descEnBytes} bytes (Limit: 8000)");
        Console.WriteLine($"[Config] ChangeNote: {changeNote}");

        if (descZhBytes > 8000)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] Chinese description exceeds Steam limit of 8000 bytes! (Current: {descZhBytes} bytes)");
            Console.ResetColor();
            return 1;
        }

        if (descEnBytes > 8000)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] English description exceeds Steam limit of 8000 bytes! (Current: {descEnBytes} bytes)");
            Console.ResetColor();
            return 1;
        }

        if (dryRun)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[DRY RUN] Parameter validation passed. No changes were submitted.");
            Console.ResetColor();
            return 0;
        }

        // Initialize Steamworks
        Console.WriteLine("\n[Steam] Initializing Steamworks API...");
        Environment.SetEnvironmentVariable("SteamAppId", AppId.ToString());
        if (!SteamAPI.Init())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] Failed to initialize Steam API! Is Steam running on your computer?");
            Console.ResetColor();
            return 1;
        }

        try
        {
            var personaName = SteamFriends.GetPersonaName();
            Console.WriteLine($"[Steam] Connected as user: '{personaName}'");

            var appIdObj = new AppId_t(AppId);
            var pubFileIdObj = new PublishedFileId_t(publishedFileId);

            var updateHandle = SteamUGC.StartItemUpdate(appIdObj, pubFileIdObj);
            if (updateHandle == UGCUpdateHandle_t.Invalid)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] StartItemUpdate returned invalid handle!");
                Console.ResetColor();
                return 1;
            }

            Console.WriteLine("[Steam] Starting item update handle...");

            // 1. Content upload
            if (!metaOnly)
            {
                var contentDir = Path.Combine(workspacePath, "content");
                if (Directory.Exists(contentDir))
                {
                    Console.WriteLine($"[Steam] Setting content path: {contentDir}");
                    SteamUGC.SetItemContent(updateHandle, contentDir);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARN] Content folder not found at: {contentDir}");
                    Console.ResetColor();
                }
            }

            // 2. Metadata & Preview
            if (!contentOnly)
            {
                var previewImg = Path.Combine(workspacePath, "image.png");
                if (File.Exists(previewImg))
                {
                    Console.WriteLine($"[Steam] Setting preview image: {previewImg}");
                    SteamUGC.SetItemPreview(updateHandle, previewImg);
                }

                var visibility = visibilityStr switch
                {
                    "public" => ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic,
                    "friends_only" => ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityFriendsOnly,
                    _ => ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate
                };
                SteamUGC.SetItemVisibility(updateHandle, visibility);

                if (tags.Count > 0)
                {
                    Console.WriteLine($"[Steam] Setting tags: {string.Join(", ", tags)}");
                    SteamUGC.SetItemTags(updateHandle, tags);
                }

                // Bilingual metadata update
                if (targetLang is null or "schinese")
                {
                    Console.WriteLine("[Steam] Updating Simplified Chinese ('schinese') title & description...");
                    SteamUGC.SetItemUpdateLanguage(updateHandle, "schinese");
                    SteamUGC.SetItemTitle(updateHandle, titleZh);
                    SteamUGC.SetItemDescription(updateHandle, descZh);
                }

                if (targetLang is null or "english")
                {
                    Console.WriteLine("[Steam] Updating English ('english') title & description...");
                    SteamUGC.SetItemUpdateLanguage(updateHandle, "english");
                    SteamUGC.SetItemTitle(updateHandle, titleEn);
                    SteamUGC.SetItemDescription(updateHandle, descEn);
                }
            }

            // 3. Submit
            Console.WriteLine($"\n[Steam] Submitting update (changeNote: '{changeNote}')...");
            var submitCall = SteamUGC.SubmitItemUpdate(updateHandle, changeNote);
            var callResult = CallResult<SubmitItemUpdateResult_t>.Create();

            var tcs = new TaskCompletionSource<SubmitItemUpdateResult_t>();
            callResult.Set(submitCall, (res, failure) =>
            {
                if (failure)
                {
                    tcs.TrySetException(new Exception("Steam IO failure during item submission"));
                }
                else
                {
                    tcs.TrySetResult(res);
                }
            });

            // Monitor progress loop
            EItemUpdateStatus lastStatus = EItemUpdateStatus.k_EItemUpdateStatusInvalid;
            var startTime = DateTime.UtcNow;

            while (!tcs.Task.IsCompleted)
            {
                SteamAPI.RunCallbacks();

                var status = SteamUGC.GetItemUpdateProgress(updateHandle, out var bytesDone, out var bytesTotal);
                if (status != lastStatus || bytesTotal > 0)
                {
                    lastStatus = status;
                    var progressStr = bytesTotal > 0
                        ? $"{bytesDone * 100.0 / bytesTotal:F1}% ({bytesDone / (1024.0 * 1024):F1}MB / {bytesTotal / (1024.0 * 1024):F1}MB)"
                        : "";
                    Console.WriteLine($"  -> Status: {status} {progressStr}");
                }

                await Task.Delay(500);

                if ((DateTime.UtcNow - startTime).TotalSeconds > 180)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[WARN] Timeout threshold (180s) reached while waiting for Steam response.");
                    Console.ResetColor();
                    break;
                }
            }

            if (tcs.Task.IsCompletedSuccessfully)
            {
                var result = tcs.Task.Result;
                if (result.m_eResult == EResult.k_EResultOK)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n=================================================");
                    Console.WriteLine(" [SUCCESS] Workshop item updated successfully!   ");
                    Console.WriteLine($" Link: https://steamcommunity.com/sharedfiles/filedetails/?id={publishedFileId}");
                    Console.WriteLine("=================================================");
                    Console.ResetColor();
                    return 0;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ERROR] SubmitItemUpdate failed with result: {result.m_eResult}");
                Console.ResetColor();

                if (result.m_eResult == EResult.k_EResultInvalidParam)
                {
                    Console.WriteLine("Hint: One or more parameters were invalid (e.g. description > 8000 bytes, invalid tag, or unsupported field).");
                }
                else if (result.m_eResult == EResult.k_EResultTimeout)
                {
                    Console.WriteLine("Hint: Steam server processing timed out. Often the update was still accepted. Check the web page to verify.");
                }
                return 1;
            }
            else if (tcs.Task.IsFaulted)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ERROR] Steam API Call failed: {tcs.Task.Exception?.InnerException?.Message}");
                Console.ResetColor();
                return 1;
            }

            return 0;
        }
        finally
        {
            SteamAPI.Shutdown();
        }
    }

    private static string? GetString(JsonElement el, string propertyName)
    {
        return el.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String
            ? prop.GetString()
            : null;
    }

    private static void PrintHelp()
    {
        Console.WriteLine(@"Usage: WorkshopPublisher [options]

Options:
  -w, --workspace <dir>   Path to the workshop directory (default: 'workshop')
  -c, --content-only      Only upload the mod build content and changeNote (keeps titles and descriptions untouched)
  -m, --meta-only         Only update titles, descriptions, preview, and tags (skips re-uploading large mod files)
  --lang <lang>           Target language to update: 'schinese', 'english', or omit for both
  --dry-run               Validate configuration, paths, and byte limits without submitting
  -h, --help              Show help information
");
    }
}
