'use strict';

// Launcher tests for bin/wc3-mcp.js and lib/ensure-binary.js (spec TST-08).
//
// Every case starts the real launcher in a child process with LOCALAPPDATA, HOME and
// USERPROFILE pointed into a throwaway sandbox, so the per-version cache the tests read
// and write is never the one on the developer's machine.
//
// The launcher's only network entry point is the global `fetch`, so each child is
// started with `-r <stub>` preloading a stub that replaces it. The stub appends every
// requested URL to a log and either fails the way a dead network fails or serves bytes
// handed to it through the environment. An empty log is the proof of "downloaded
// nothing"; if a test ever reached github.com it would either throw or show up there.

const { test, after } = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const crypto = require('node:crypto');
const { spawnSync } = require('node:child_process');

const PACKAGE_DIR = path.join(__dirname, '..');
const LAUNCHER = path.join(PACKAGE_DIR, 'bin', 'wc3-mcp.js');
const pkg = require('../package.json');
const VERSION = pkg.version;

const SANDBOXES = [];

const FETCH_STUB = `
'use strict';
// Preloaded with -r before the launcher. Replaces the global fetch so the download
// path can be driven without a network.

const fs = require('fs');

function record(url) {
  const logPath = process.env.WC3_TEST_FETCH_LOG;
  if (logPath) fs.appendFileSync(logPath, String(url) + '\\n');
}

function respond(text) {
  return {
    ok: true,
    status: 200,
    statusText: 'OK',
    async arrayBuffer() { return Buffer.from(text, 'utf8'); },
  };
}

globalThis.fetch = async (url) => {
  record(url);
  const mode = process.env.WC3_TEST_FETCH_MODE || 'network-error';
  if (mode === 'network-error') {
    const error = new Error('getaddrinfo ENOTFOUND github.com');
    error.code = 'ENOTFOUND';
    throw error;
  }
  if (mode === 'http-503') {
    return {
      ok: false,
      status: 503,
      statusText: 'Service Unavailable',
      async arrayBuffer() { return Buffer.alloc(0); },
    };
  }
  if (String(url).endsWith('.sha256')) return respond(process.env.WC3_TEST_SHA256 || '');
  return respond(process.env.WC3_TEST_ZIP || '');
};
`;

function makeSandbox() {
  const root = fs.mkdtempSync(path.join(os.tmpdir(), 'wc3-mcp-launcher-'));
  const localAppData = path.join(root, 'localappdata');
  const home = path.join(root, 'home');
  fs.mkdirSync(localAppData, { recursive: true });
  fs.mkdirSync(home, { recursive: true });
  const cacheRoot = path.join(localAppData, 'Programs', 'wc3-mcp', 'versions');
  const sandbox = {
    root,
    localAppData,
    home,
    cacheRoot,
    versionDir: path.join(cacheRoot, `v${VERSION}`),
    fetchLog: path.join(root, 'fetch.log'),
  };
  SANDBOXES.push(sandbox);
  return sandbox;
}

function runLauncher(sandbox, args, env = {}) {
  const stubPath = path.join(sandbox.root, 'fetch-stub.cjs');
  fs.writeFileSync(stubPath, FETCH_STUB);
  return spawnSync(process.execPath, ['-r', stubPath, LAUNCHER, ...args], {
    cwd: sandbox.root,
    env: {
      ...process.env,
      LOCALAPPDATA: sandbox.localAppData,
      APPDATA: path.join(sandbox.root, 'appdata'),
      HOME: sandbox.home,
      USERPROFILE: sandbox.home,
      WC3_TEST_FETCH_LOG: sandbox.fetchLog,
      WC3_TEST_FETCH_MODE: 'network-error',
      ...env,
    },
    encoding: 'utf8',
    timeout: 60000,
    windowsHide: true,
  });
}

function fetchedUrls(sandbox) {
  if (!fs.existsSync(sandbox.fetchLog)) return [];
  return fs.readFileSync(sandbox.fetchLog, 'utf8').split('\n').filter(Boolean);
}

function sha256(buffer) {
  return crypto.createHash('sha256').update(buffer).digest('hex');
}

// A cache hit needs a spawnable binary. A hard link (or copy) of the running node
// binary under the cache's exe name is enough: the launcher only spawns that path.
function installCachedBinary(sandbox) {
  fs.mkdirSync(sandbox.versionDir, { recursive: true });
  const exe = path.join(sandbox.versionDir, 'wc3-mcp.exe');
  try {
    fs.linkSync(process.execPath, exe);
  } catch {
    fs.copyFileSync(process.execPath, exe);
  }
  fs.writeFileSync(path.join(sandbox.versionDir, 'CascLib.dll'), 'placeholder, never loaded');
  return exe;
}

after(() => {
  for (const sandbox of SANDBOXES) {
    try { fs.rmSync(sandbox.root, { recursive: true, force: true }); } catch { /* best effort */ }
  }
});

test('version path prints the package version and downloads nothing', () => {
  for (const flag of ['version', '--version']) {
    const sandbox = makeSandbox();
    const result = runLauncher(sandbox, [flag]);

    assert.equal(result.status, 0, `"${flag}" exited ${result.status}: ${result.stderr}`);
    assert.equal(result.stdout, `${VERSION}\n`, `"${flag}" must print the version and nothing else`);
    assert.equal(result.stderr, '', `"${flag}" must not write status to stderr`);
    assert.deepEqual(fetchedUrls(sandbox), [], 'the version path must not touch the network');
    assert.equal(
      fs.existsSync(path.join(sandbox.localAppData, 'Programs')),
      false,
      'the version path must not create the cache',
    );
  }
});

test('a cache hit runs the cached binary and downloads nothing', () => {
  const sandbox = makeSandbox();
  installCachedBinary(sandbox);

  const result = runLauncher(sandbox, [
    '-e',
    'process.stdout.write("child:" + process.env.WC3_MCP_NPX + "\\n"); process.exit(7)',
  ]);

  assert.equal(result.status, 7, `the cached binary's exit code must propagate: ${result.stderr}`);
  assert.equal(result.stdout, 'child:1\n', 'the cached binary must be the process that ran');
  assert.equal(result.stderr, '', 'a cache hit must not warn or report a download');
  assert.deepEqual(fetchedUrls(sandbox), [], 'a cache hit must not touch the network');
  assert.deepEqual(
    fs.readdirSync(sandbox.cacheRoot).filter((name) => name.startsWith('.')),
    [],
    'a cache hit must not stage a download',
  );
});

test('a digest mismatch is refused with a message naming the mismatch', () => {
  const sandbox = makeSandbox();
  const zip = Buffer.from('this is the archive the release would have served');
  const wrongDigest = sha256(Buffer.from('a different archive'));
  const actualDigest = sha256(zip);

  const result = runLauncher(sandbox, [], {
    WC3_TEST_FETCH_MODE: 'ok',
    WC3_TEST_ZIP: zip.toString('utf8'),
    WC3_TEST_SHA256: wrongDigest,
  });

  assert.equal(result.status, 1, `a mismatch must fail the launch: ${result.stderr}`);
  assert.equal(result.stdout, '', 'a mismatch must not write to stdout');
  assert.match(result.stderr, /\[wc3-mcp\] could not prepare the wc3-mcp binary/);
  assert.ok(
    result.stderr.includes(`checksum mismatch for wc3-mcp ${VERSION}, expected ${wrongDigest}, got ${actualDigest}`),
    `the message must name both digests, got: ${JSON.stringify(result.stderr)}`,
  );
  assert.equal(fetchedUrls(sandbox).length, 2, 'the archive and its .sha256 must both have been fetched');
  assert.equal(fs.existsSync(sandbox.versionDir), false, 'nothing may be installed from a mismatched archive');
  assert.deepEqual(fs.readdirSync(sandbox.cacheRoot), [], 'a refused archive must not be left in the cache');
});

test('a network failure during postinstall only warns and does not fail the install', () => {
  assert.match(pkg.scripts.postinstall, /--preload-binary/, 'this must be the path npm install runs');
  const sandbox = makeSandbox();

  const result = runLauncher(sandbox, ['--preload-binary']);

  assert.equal(result.status, 0, `postinstall must survive a dead network: ${result.stderr}`);
  assert.equal(result.stdout, '', 'postinstall must not write to stdout');

  const stderrLines = result.stderr.split('\n').filter(Boolean);
  assert.ok(
    stderrLines.includes('[wc3-mcp] binary preload skipped, getaddrinfo ENOTFOUND github.com'),
    `the warning must reach stderr, got: ${JSON.stringify(result.stderr)}`,
  );
  assert.ok(
    stderrLines.every((line) => line.startsWith('[wc3-mcp] ')),
    `stderr must carry only launcher status, got: ${JSON.stringify(result.stderr)}`,
  );
  assert.equal(fetchedUrls(sandbox).length, 1, 'the preload must have tried exactly one download');
  assert.equal(fs.existsSync(sandbox.versionDir), false, 'a failed preload must install nothing');
});

test('stdout carries the version line and nothing else', async (t) => {
  const mismatchZip = Buffer.from('mismatched archive');

  const cases = [
    {
      name: 'version',
      args: ['version'],
      env: {},
      expected: `${VERSION}\n`,
    },
    {
      name: 'postinstall with no network',
      args: ['--preload-binary'],
      env: { WC3_TEST_FETCH_MODE: 'network-error' },
      expected: '',
    },
    {
      name: 'a refused archive',
      args: [],
      env: {
        WC3_TEST_FETCH_MODE: 'ok',
        WC3_TEST_ZIP: mismatchZip.toString('utf8'),
        WC3_TEST_SHA256: '0'.repeat(64),
      },
      expected: '',
    },
    {
      name: 'a cache hit with a silent binary',
      args: ['-e', 'process.exit(0)'],
      env: {},
      cached: true,
      expected: '',
    },
  ];

  for (const entry of cases) {
    await t.test(entry.name, () => {
      const sandbox = makeSandbox();
      if (entry.cached) installCachedBinary(sandbox);
      const result = runLauncher(sandbox, entry.args, entry.env);
      assert.equal(
        result.stdout,
        entry.expected,
        `${entry.name} wrote to stdout, which is the MCP protocol stream: ${JSON.stringify(result.stdout)}`,
      );
    });
  }
});
