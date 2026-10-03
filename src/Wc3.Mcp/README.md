# Wc3.Mcp

The server project. It speaks MCP (Model Context Protocol) over **stdio** to
any MCP client (Claude Code, Claude Desktop, any MCP-capable agent). Built on
the official [C# MCP SDK](https://github.com/modelcontextprotocol/csharp-sdk)
(`ModelContextProtocol` 1.4.1, stable).

Every tool is a thin wrapper over the shared `Wc3.Commands` layer and returns
that layer's result objects as JSON. No map parsing or logic lives here.

## Build and publish

```
dotnet publish src/Wc3.Mcp/Wc3.Mcp.csproj -c Release \
  --self-contained -r win-x64 -o dist-mcp
```

The published server is `dist-mcp/wc3-mcp.exe`, with `CascLib.dll` beside it.
Setting up an AI app (`wc3-mcp install`) and the full tool list are in the root README.md. The
setup commands live in `Setup/`, and `Program.cs` runs them when the first argument names one.

## Verified how

`tests/Wc3.Tests/McpServerTests.cs` drives the exact server wiring through the
SDK's own `McpClient` over in-memory pipes (hermetic, no processes, no WC3
install). It checks the initialize handshake and the server name `wc3-mcp`,
`tools/list` returning all 49 tools, a real `list_files` call against a
synthetic map, and the clean-error path. The published exe was also checked by
piping raw JSON-RPC (`initialize`, `notifications/initialized`, `tools/list`)
into stdin.
