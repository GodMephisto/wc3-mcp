#!/usr/bin/env node
'use strict';

// Entry point for the wc3-mcp npm package. It makes sure the matching .NET binary is
// present in the per-version cache and then hands the process over to it.
//
// `wc3-mcp version` answers from package.json alone, with no download, so installs and
// release tooling can read the version offline.
//
// MCP stdio transport owns stdout. All launcher status goes to stderr.

const { spawn } = require('child_process');
const pkg = require('../package.json');
const { ensureBinary } = require('../lib/ensure-binary');

function writeVersion() {
  process.stdout.write(`${pkg.version}\n`);
}

async function main() {
  const args = process.argv.slice(2);
  const first = args[0];

  if (first === 'version' || first === '--version') {
    writeVersion();
    return 0;
  }

  if (first === '--preload-binary') {
    // Called by the npm postinstall step. Never fail the install on a network error,
    // because a restricted build environment has no business blocking the package.
    try {
      await ensureBinary();
    } catch (error) {
      process.stderr.write(`[wc3-mcp] binary preload skipped, ${error.message}\n`);
    }
    return 0;
  }

  let exe;
  try {
    exe = await ensureBinary();
  } catch (error) {
    process.stderr.write(`[wc3-mcp] could not prepare the wc3-mcp binary, ${error.message}\n`);
    process.stderr.write(`[wc3-mcp] install v${pkg.version} from https://github.com/GodMephisto/wc3-mcp/releases\n`);
    return 1;
  }

  // Tells the server it was started through npx, so its own setup can register the
  // auto-updating npx command in each AI client.
  process.env.WC3_MCP_NPX = '1';

  const child = spawn(exe, args, { stdio: 'inherit', windowsHide: true });

  child.on('error', (error) => {
    process.stderr.write(`[wc3-mcp] failed to start ${exe}, ${error.message}\n`);
    process.exitCode = 1;
  });

  child.on('exit', (code, signal) => {
    if (signal) {
      process.kill(process.pid, signal);
      return;
    }
    process.exitCode = code === null ? 1 : code;
  });
}

// process.exitCode rather than process.exit, so pending sockets close cleanly. Calling
// process.exit() right after a failed fetch trips a libuv handle assertion on Windows.
main().then((code) => {
  if (typeof code === 'number') process.exitCode = code;
}).catch((error) => {
  process.stderr.write(`[wc3-mcp] unexpected failure, ${error && error.message ? error.message : error}\n`);
  process.exitCode = 1;
});
