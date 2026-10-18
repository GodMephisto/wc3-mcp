---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/mcpserver/server.go"
ref: HEAD
---

// Package mcpserver implements wc3-forge's `--mcp` mode: the binary acts as an
// MCP stdio server that proxies tool calls to a separately-running wc3-forge
// editor instance over the existing JSON-RPC bridge. This is the in-binary
// replacement for the standalone Node server in mcp/ — it ships with the
// installer, so end-users register the MCP with just the executable:
//
//	claude mcp add wc3-forge -- "C:\Program Files\wc3-forge\wc3-forge.exe" --mcp
//
// It is deliberately a thin proxy (not a headless in-process editor): Claude
// launches this as its own stdio subprocess, which can't be the user's GUI
// window, so to preserve the "GUI and agent share one session + undo stack"
// model every call is forwarded to the running GUI instance.
//
// The tool catalog (names, descriptions, JSON Schemas, bridge methods) is
// generated from the TypeScript source of truth (mcp/src/tools.ts) into the
// embedded tools.json — see mcp/scripts/gen-tools.mjs.
package mcpserver

import (
	"bytes"
	"context"
	_ "embed"
	"encoding/json"
	"fmt"

	"github.com/StephenSHorton/wc3-forge/internal/bridge"
	"github.com/modelcontextprotocol/go-sdk/mcp"
)

//go:embed tools.json
var catalogJSON []byte

// catalogEntry is one tool from the generated catalog. Method is empty for the
// two "local" tools (sessions_list, session_select) handled in-proxy.
type catalogEntry struct {
	Name        string          `json:"name"`
	Description string          `json:"description"`
	InputSchema json.RawMessage `json:"inputSchema"`
	Method      string          `json:"method"`
}

// Run builds the MCP server from the embedded catalog and serves it over stdio
// until the client disconnects. It blocks. Nothing in this path may write to
// stdout (the MCP channel) — diagnostics go to the error return / the log file.
func Run() error {
	entries, err := loadCatalog()
	if err != nil {
		return err
	}

	pool := newSessionPool()
	server := mcp.NewServer(&mcp.Implementation{
		Name:    "wc3-forge",
		Version: bridge.Version,
	}, nil)

	for _, e := range entries {
		tool := &mcp.Tool{
			Name:        e.Name,
			Description: e.Description,
			InputSchema: json.RawMessage(e.InputSchema),
		}
		switch {
		case e.Method != "":
			server.AddTool(tool, forwardHandler(pool, e.Method))
		case e.Name == "sessions_list":
			server.AddTool(tool, sessionsListHandler(pool))
		case e.Name == "session_select":
			server.AddTool(tool, sessionSelectHandler(pool))
		default:
			return fmt.Errorf("catalog tool %q has no bridge method and no local handler", e.Name)
		}
	}

	return server.Run(context.Background(), &mcp.StdioTransport{})
}

func loadCatalog() ([]catalogEntry, error) {
	var entries []catalogEntry
	if err := json.Unmarshal(catalogJSON, &entries); err != nil {
		return nil, fmt.Errorf("parse embedded tool catalog: %w", err)
	}
	return entries, nil
}

// forwardHandler forwards a tool call to the selected bridge method and wraps
// the result the same way the TS wrap() does (pretty JSON text content; bridge
// errors become an IsError result rather than a protocol error).
func forwardHandler(pool *sessionPool, method string) mcp.ToolHandler {
	return func(_ context.Context, req *mcp.CallToolRequest) (*mcp.CallToolResult, error) {
		result, err := pool.call(method, req.Params.Arguments)
		if err != nil {
			return errorResult(err), nil
		}
		return textResult(prettyJSON(result)), nil
	}
}

// sessionsListHandler enumerates running instances and pings each — local to
// the proxy (mirrors tools.ts sessions_list). Pings target each instance
// directly, independent of the current selection.
func sessionsListHandler(pool *sessionPool) mcp.ToolHandler {
	type sessionInfo struct {
		PID        int             `json:"pid"`
		Port       int             `json:"port"`
		StartedAt  string          `json:"started_at"`
		IsSelected bool            `json:"is_selected"`
		Ping       json.RawMessage `json:"ping"`
	}
	return func(_ context.Context, _ *mcp.CallToolRequest) (*mcp.CallToolResult, error) {
		locks := bridge.ListLocks()
		selected := pool.selected()
		sessions := make([]sessionInfo, 0, len(locks))
		for _, l := range locks {
			ping, err := pool.callPID(l.PID, "bridge.ping", nil)
			if err != nil {
				ping, _ = json.Marshal(map[string]string{"error": err.Error()})
			}
			sessions = append(sessions, sessionInfo{
				PID:        l.PID,
				Port:       l.Port,
				StartedAt:  l.StartedAt,
				IsSelected: l.PID == selected,
				Ping:       ping,
			})
		}
		raw, err := json.Marshal(map[string]any{
			"count":        len(sessions),
			"selected_pid": selectedOrNil(selected),
			"sessions":     sessions,
		})
		if err != nil {
			return errorResult(err), nil
		}
		return textResult(prettyJSON(raw)), nil
	}
}

// sessionSelectHandler pins subsequent calls to a pid (or clears selection) —
// local to the proxy (mirrors tools.ts session_select).
func sessionSelectHandler(pool *sessionPool) mcp.ToolHandler {
	return func(_ context.Context, req *mcp.CallToolRequest) (*mcp.CallToolResult, error) {
		var args struct {
			PID *int `json:"pid"`
		}
		if args0 := req.Params.Arguments; len(args0) > 0 {
			if err := json.Unmarshal(args0, &args); err != nil {
				return errorResult(fmt.Errorf("invalid arguments: %w", err)), nil
			}
		}
		if args.PID == nil {
			pool.clearSelection()
			return textResult("Selection cleared; calls route to the oldest running wc3-forge."), nil
		}
		locks := bridge.ListLocks()
		for _, l := range locks {
			if l.PID == *args.PID {
				pool.selectPID(*args.PID)
				raw, _ := json.Marshal(map[string]any{
					"selected_pid": l.PID,
					"port":         l.Port,
					"started_at":   l.StartedAt,
				})
				return textResult(prettyJSON(raw)), nil
			}
		}
		active := joinPIDs(locks)
		if active == "" {
			active = "(none)"
		}
		return errorResult(&bridgeNotRunning{
			msg: fmt.Sprintf("No wc3-forge with pid=%d. Active pids: %s.", *args.PID, active),
		}), nil
	}
}

func textResult(s string) *mcp.CallToolResult {
	return &mcp.CallToolResult{Content: []mcp.Content{&mcp.TextContent{Text: s}}}
}

// errorResult mirrors the TS errorResponse() text formatting so the two servers
// behave identically.
func errorResult(err error) *mcp.CallToolResult {
	var text string
	switch e := err.(type) {
	case *bridgeNotRunning:
		text = "wc3-forge bridge unavailable.\n" + e.Error()
	case *bridgeError:
		text = e.Error()
	default:
		text = "Tool failed: " + err.Error()
	}
	return &mcp.CallToolResult{
		IsError: true,
		Content: []mcp.Content{&mcp.TextContent{Text: text}},
	}
}

// prettyJSON re-indents a raw JSON result to match the Node server's
// JSON.stringify(data, null, 2).
func prettyJSON(raw json.RawMessage) string {
	if len(raw) == 0 {
		return "null"
	}
	var buf bytes.Buffer
	if err := json.Indent(&buf, raw, "", "  "); err != nil {
		return string(raw)
	}
	return buf.String()
}

func selectedOrNil(pid int) any {
	if pid == 0 {
		return nil
	}
	return pid
}
