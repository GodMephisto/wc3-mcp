# wc3-mcp (npm launcher)

An MCP server that lets AI assistants read, audit, and edit Warcraft III (`.w3x` and `.w3m`)
maps byte-faithfully.

This npm package is a tiny zero-dependency launcher. It downloads the matching self-contained
`wc3-mcp.exe` and `CascLib.dll` from the GitHub Release into a per-version cache folder, then
starts the server. Because the package version and the release version move together and the
binary lives in a versioned folder, upgrading never hits a Windows file lock on a running
`wc3-mcp.exe` or `CascLib.dll`.

## Run it

```
npx -y wc3-mcp@latest
```

Point any MCP client at the same command to get one-click setup and an auto-update on every
launch. For example, a client config that takes a command and args looks like this.

```json
{
  "mcpServers": {
    "wc3": {
      "command": "npx",
      "args": ["-y", "wc3-mcp@latest"]
    }
  }
}
```

Then start the setup wizard once, which writes the `npx` entry into every supported client on
this PC.

```
npx -y wc3-mcp@latest install --all
```

## Requirements

Windows x64 with Node 18 or newer. Node 18 is where the built-in `fetch` the downloader uses
arrived, so older versions are not supported.

The launcher uses only Node built-in modules (fetch, crypto, fs, path, child_process). It has
no npm dependencies.

## How the download works

On first launch, or on `npm install`, the launcher fetches
`wc3-mcp-v<version>-win-x64.zip` and its `.sha256` from the GitHub Release, verifies the SHA-256
digest, unpacks it with Windows `tar.exe` (with a PowerShell `Expand-Archive` fallback), and
moves it into `%LOCALAPPDATA%\Programs\wc3-mcp\versions\v<version>\`. Older versions are pruned
on a best-effort basis, and a locked folder is left alone.

If the release asset for this version does not exist yet, the launcher prints a message to
stderr and exits non-zero. It never writes download status to stdout, because the MCP stdio
transport uses stdout for the protocol.

## Commands

- `wc3-mcp` starts the MCP server over stdio.
- `wc3-mcp version` prints the package version from `package.json`, with no download.
- `wc3-mcp install --all` (and the other setup verbs) are handled by the .NET server itself.

## License

Apache-2.0. See `LICENSE`.
