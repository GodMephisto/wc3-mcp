---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "mcp/README.md"
ref: HEAD
---

# wc3-forge-mcp

MCP (Model Context Protocol) server that drives a running wc3-forge editor via its embedded JSON-RPC bridge.

## Install

```powershell
npm install
npm run build
```

## Register with Claude Code

```powershell
claude mcp add wc3-forge --scope user -- node C:\Users\<user>\projects\wc3-forge\mcp\dist\index.js
```

(`--scope` MUST come before `--`, otherwise it gets forwarded to node.)

## Dev mode

```powershell
npm run dev
```

Runs `src/index.ts` directly via `tsx` with hot-reload. The MCP server speaks stdio, so to actually test it you need either Claude Code pointed at the running process or the `npx @modelcontextprotocol/inspector` tool.

## How it talks to wc3-forge

1. Each running wc3-forge writes its own lockfile at `%USERPROFILE%\.wc3-forge\mcp\<pid>.lock` with `{ pid, port, token, started_at }`. Stale lockfiles are pruned on startup (Go side) and on connection (this server's `discovery.ts`).
2. This MCP server reads the directory on each tool invocation, filters by alive pids, and connects via TCP to whichever instance the user has `session_select`ed (default: the oldest one).
3. Every request includes the lockfile token as `params._token` for authentication.

The lockdir can be overridden with `WC3FORGE_MCP_LOCK_DIR`.

Two MCP tools manage the multi-instance case:

- `sessions_list` — returns every running wc3-forge with its pid, port, started_at, and bridge_ping result.
- `session_select` — picks one by pid for all subsequent tool calls. Pass `pid: null` (or omit) to clear and route to the oldest instance.

If no wc3-forge is running, tool calls return an error telling the user to launch one.

## Layout

```
src/
├── index.ts            # MCP server entry (stdio); registers tools
├── discovery.ts        # Finds .wc3-forge/mcp/<pid>.lock files
├── bridge_client.ts    # NDJSON JSON-RPC client over TCP, with token auth
└── tools.ts            # Registers MCP tools on the McpServer
```

## Tool surface

Tools 1:1 with the handlers registered in `internal/forge/handlers.go::RegisterAll`. To add a new tool:

1. Implement the handler in Go under `internal/forge/handlers*.go` and register it in `RegisterAll`.
2. Add the matching `server.tool(...)` in `src/tools.ts`.
3. `npm run build` (or restart `npm run dev`).
