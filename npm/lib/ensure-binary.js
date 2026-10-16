'use strict';

// Downloads and verifies the matching wc3-mcp.exe and CascLib.dll from a GitHub Release
// into a per-version cache folder. Per-version folders are the reason an upgrade never
// hits a Windows file lock on a running wc3-mcp.exe or CascLib.dll.
//
// Every status message goes to stderr. MCP stdio transport owns stdout, so nothing here
// may write to stdout.

const fs = require('fs');
const os = require('os');
const path = require('path');
const crypto = require('crypto');
const { execFileSync } = require('child_process');

const pkg = require('../package.json');

const REPO = 'GodMephisto/wc3-mcp';
const EXE_NAME = 'wc3-mcp.exe';
const CASC_NAME = 'CascLib.dll';

function log(message) {
  process.stderr.write(`[wc3-mcp] ${message}\n`);
}

function cacheRoot() {
  const local = process.env.LOCALAPPDATA || path.join(os.homedir(), 'AppData', 'Local');
  return path.join(local, 'Programs', 'wc3-mcp', 'versions');
}

function versionDir(version) {
  return path.join(cacheRoot(), `v${version}`);
}

function pathsFor(version) {
  const dir = versionDir(version);
  return { dir, exe: path.join(dir, EXE_NAME), casc: path.join(dir, CASC_NAME) };
}

function isReady(version) {
  const { exe, casc } = pathsFor(version);
  return fs.existsSync(exe) && fs.existsSync(casc);
}

async function download(url) {
  const response = await fetch(url, { redirect: 'follow' });
  if (!response.ok) {
    throw new Error(`download failed with ${response.status} ${response.statusText} for ${url}`);
  }
  return Buffer.from(await response.arrayBuffer());
}

function sha256(buffer) {
  return crypto.createHash('sha256').update(buffer).digest('hex');
}

function extract(zipPath, destDir) {
  fs.mkdirSync(destDir, { recursive: true });
  try {
    // tar.exe ships with Windows 10 and 11 and reads zip archives directly.
    execFileSync('tar.exe', ['-xf', zipPath, '-C', destDir], { stdio: 'ignore' });
    return;
  } catch (error) {
    log(`tar.exe could not unpack the archive (${error.message}), falling back to PowerShell`);
  }
  const quotedZip = zipPath.replace(/'/g, "''");
  const quotedDest = destDir.replace(/'/g, "''");
  execFileSync('powershell.exe', [
    '-NoProfile',
    '-NonInteractive',
    '-Command',
    `Expand-Archive -LiteralPath '${quotedZip}' -DestinationPath '${quotedDest}' -Force`,
  ], { stdio: 'ignore' });
}

function locateDeployDir(stageDir) {
  if (fs.existsSync(path.join(stageDir, EXE_NAME))) return stageDir;
  // Some archives wrap the payload in a single top level folder.
  for (const entry of fs.readdirSync(stageDir, { withFileTypes: true })) {
    if (entry.isDirectory() && fs.existsSync(path.join(stageDir, entry.name, EXE_NAME))) {
      return path.join(stageDir, entry.name);
    }
  }
  return stageDir;
}

function pruneOldVersions(currentVersion) {
  const root = cacheRoot();
  let entries;
  try {
    entries = fs.readdirSync(root, { withFileTypes: true });
  } catch {
    return;
  }
  const dirs = entries
    .filter((entry) => entry.isDirectory() && /^v\d/.test(entry.name))
    .map((entry) => {
      const full = path.join(root, entry.name);
      let mtime = 0;
      try { mtime = fs.statSync(full).mtimeMs; } catch { /* keep 0 */ }
      return { name: entry.name, full, mtime };
    })
    .sort((a, b) => b.mtime - a.mtime);

  const keep = new Set([`v${currentVersion}`]);
  for (const item of dirs) {
    if (keep.size >= 2) break;
    keep.add(item.name);
  }

  for (const item of dirs) {
    if (keep.has(item.name)) continue;
    try {
      fs.rmSync(item.full, { recursive: true, force: true });
      log(`removed old version ${item.name}`);
    } catch (error) {
      // A running client can hold the exe. EBUSY and EPERM are expected and safe to ignore.
      if (error.code !== 'EBUSY' && error.code !== 'EPERM') {
        log(`could not remove old version ${item.name}, ${error.message}`);
      }
    }
  }
}

async function ensureBinary(options = {}) {
  const version = options.version || pkg.version;
  const { exe, casc } = pathsFor(version);

  if (isReady(version)) {
    return exe;
  }

  const root = cacheRoot();
  fs.mkdirSync(root, { recursive: true });

  const base = `https://github.com/${REPO}/releases/download/v${version}/wc3-mcp-v${version}-win-x64.zip`;
  log(`wc3-mcp ${version} is not cached, downloading ${base}`);

  const zipBuffer = await download(base);
  const shaBuffer = await download(`${base}.sha256`);

  const expected = shaBuffer.toString('utf8').trim().split(/\s+/)[0].toLowerCase();
  const actual = sha256(zipBuffer);
  if (expected !== actual) {
    throw new Error(`checksum mismatch for wc3-mcp ${version}, expected ${expected}, got ${actual}`);
  }

  const zipPath = path.join(root, `.wc3-mcp-${version}-${process.pid}-${Date.now()}.zip`);
  fs.writeFileSync(zipPath, zipBuffer);

  const stageDir = path.join(root, `.stage-${process.pid}-${Date.now()}`);
  const targetDir = versionDir(version);
  try {
    extract(zipPath, stageDir);
    const deployDir = locateDeployDir(stageDir);
    if (!fs.existsSync(path.join(deployDir, EXE_NAME)) || !fs.existsSync(path.join(deployDir, CASC_NAME))) {
      throw new Error(`the archive for wc3-mcp ${version} did not contain ${EXE_NAME} and ${CASC_NAME}`);
    }
    try {
      fs.renameSync(deployDir, targetDir);
    } catch (renameError) {
      if (renameError.code !== 'EEXIST' && renameError.code !== 'EPERM' && renameError.code !== 'ENOTEMPTY') {
        throw renameError;
      }
      // Another process finished the same install first, or a locked folder sits there.
      fs.cpSync(deployDir, targetDir, { recursive: true, force: true });
    }
  } finally {
    try { fs.rmSync(stageDir, { recursive: true, force: true }); } catch { /* best effort */ }
    try { fs.rmSync(zipPath, { force: true }); } catch { /* best effort */ }
  }

  if (!isReady(version)) {
    throw new Error(`wc3-mcp ${version} did not install correctly into ${targetDir}`);
  }

  log(`installed wc3-mcp ${version} into ${targetDir}`);
  pruneOldVersions(version);
  return exe;
}

module.exports = { ensureBinary, cacheRoot, versionDir };
