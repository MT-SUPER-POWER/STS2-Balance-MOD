const fs = require('fs');
const path = require('path');

/**
 * Comments on the PR when code/format check fails.
 * @param {{ github: any, context: any, reportDir?: string }} params
 */
module.exports = async function ({ github, context, reportDir = 'report' }) {
  const dir = path.resolve(process.cwd(), reportDir);
  const prNumberFile = path.join(dir, 'pr_number.txt');
  let outputFile = path.join(dir, 'check-output.txt');

  if (!fs.existsSync(outputFile)) {
    // Fallback to root directory
    outputFile = path.resolve(process.cwd(), 'check-output.txt');
  }

  if (!fs.existsSync(outputFile)) {
    console.log('No check output found, skipping PR comment.');
    return;
  }

  let issueNumber;
  if (fs.existsSync(prNumberFile)) {
    const raw = fs.readFileSync(prNumberFile, 'utf8').trim();
    issueNumber = parseInt(raw, 10);
  } else if (context.issue && context.issue.number) {
    issueNumber = context.issue.number;
  } else if (
    context.payload &&
    context.payload.workflow_run &&
    context.payload.workflow_run.pull_requests &&
    context.payload.workflow_run.pull_requests[0]
  ) {
    issueNumber = context.payload.workflow_run.pull_requests[0].number;
  }

  if (!issueNumber || isNaN(issueNumber)) {
    console.log('No valid PR number found, skipping comment.');
    return;
  }

  const output = fs.readFileSync(outputFile, 'utf8');
  const body = [
    '## 🎨 代码格式与规范检查未通过',
    '',
    '检测到代码格式或 EditorConfig 规范问题，请在本地运行以下命令修复：',
    '```bash',
    'pnpm run format',
    '# 或直接运行全面检查：',
    'pnpm run check',
    '```',
    '',
    '<details><summary>详细输出</summary>',
    '',
    '```',
    output.slice(0, 3000),
    '```',
    '</details>'
  ].join('\n');

  await github.rest.issues.createComment({
    owner: context.repo.owner,
    repo: context.repo.repo,
    issue_number: issueNumber,
    body
  });

  console.log(`Successfully commented on PR #${issueNumber}.`);
};
