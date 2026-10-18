---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/bridge/bridge.go"
ref: HEAD
---

// Package bridge implements the wc3-forge MCP JSON-RPC bridge.
//
// JSON-RPC 2.0 over NDJSON on 127.0.0.1, with a per-pid lockfile at
// ~/.wc3-forge/mcp/<pid>.lock containing {pid, port, token, started_at}.
// Every request must carry params._token matching the lockfile token.
//
// The wire contract is preserved verbatim from the HiveWE C++ fork's
// src/mcp_bridge/, so MCP clients written against that bridge work against
// wc3-forge unchanged. See CREDITS.md for the upstream attribution.
package bridge

import (
	"bufio"
	"bytes"
	"crypto/rand"
	"encoding/hex"
	"encoding/json"
	"errors"
	"fmt"
	"io"
	"log"
	"net"
	"sort"
	"sync"
)

const Version = "0.0.1"

type Handler func(params json.RawMessage) (any, error)

type Bridge struct {
	mu       sync.Mutex
	handlers map[string]Handler
	listener net.Listener
	port     int
	token    string
	closed   bool
	wg       sync.WaitGroup
}

func New() *Bridge {
	return &Bridge{handlers: make(map[string]Handler)}
}

func (b *Bridge) Register(method string, h Handler) {
	b.mu.Lock()
	defer b.mu.Unlock()
	b.handlers[method] = h
}

func (b *Bridge) Port() int     { return b.port }
func (b *Bridge) Token() string { return b.token }

// TokenShort returns the first 8 hex chars of the bridge auth token. The
// Agent Console surfaces this in its header so the user can confirm which
// bridge a connected client is talking to without leaking the full secret to
// any UI that ends up screenshotted/streamed.
func (b *Bridge) TokenShort() string {
	if len(b.token) <= 8 {
		return b.token
	}
	return b.token[:8]
}

// ListHandlers returns the sorted set of registered method names. Used by the
// handler/catalog parity test (internal/forge) to verify every registered
// bridge method has a matching tools.json entry and vice versa.
func (b *Bridge) ListHandlers() []string {
	b.mu.Lock()
	defer b.mu.Unlock()
	out := make([]string, 0, len(b.handlers))
	for m := range b.handlers {
		out = append(out, m)
	}
	sort.Strings(out)
	return out
}

func (b *Bridge) Start() error {
	tok, err := makeToken()
	if err != nil {
		return fmt.Errorf("make token: %w", err)
	}
	b.token = tok

	ln, err := net.Listen("tcp", "127.0.0.1:0")
	if err != nil {
		return fmt.Errorf("listen: %w", err)
	}
	b.listener = ln
	b.port = ln.Addr().(*net.TCPAddr).Port

	if err := writeLockfile(b.port, b.token); err != nil {
		_ = ln.Close()
		return fmt.Errorf("write lockfile: %w", err)
	}

	b.wg.Add(1)
	go b.acceptLoop()
	return nil
}

func (b *Bridge) Stop() {
	b.mu.Lock()
	if b.closed {
		b.mu.Unlock()
		return
	}
	b.closed = true
	b.mu.Unlock()

	if b.listener != nil {
		_ = b.listener.Close()
	}
	deleteLockfile()
	b.wg.Wait()
}

func (b *Bridge) acceptLoop() {
	defer b.wg.Done()
	for {
		conn, err := b.listener.Accept()
		if err != nil {
			if errors.Is(err, net.ErrClosed) {
				return
			}
			log.Printf("accept error: %v", err)
			return
		}
		b.wg.Add(1)
		go b.serveClient(conn)
	}
}

func (b *Bridge) serveClient(conn net.Conn) {
	defer b.wg.Done()
	defer conn.Close()
	reader := bufio.NewReader(conn)
	for {
		line, err := reader.ReadBytes('\n')
		if err != nil {
			if !errors.Is(err, io.EOF) && !errors.Is(err, net.ErrClosed) {
				log.Printf("read error: %v", err)
			}
			return
		}
		line = bytes.TrimRight(line, "\r\n")
		if len(line) == 0 {
			continue
		}
		resp := b.handleMessage(line)
		out, err := json.Marshal(resp)
		if err != nil {
			log.Printf("response marshal failed: %v", err)
			return
		}
		out = append(out, '\n')
		if _, err := conn.Write(out); err != nil {
			return
		}
	}
}

type jsonRpcRequest struct {
	JsonRpc string          `json:"jsonrpc"`
	ID      *int64          `json:"id,omitempty"`
	Method  string          `json:"method"`
	Params  json.RawMessage `json:"params,omitempty"`
}

type jsonRpcError struct {
	Code    int    `json:"code"`
	Message string `json:"message"`
	Data    any    `json:"data,omitempty"`
}

type jsonRpcResponse struct {
	JsonRpc string        `json:"jsonrpc"`
	ID      any           `json:"id"`
	Result  any           `json:"result,omitempty"`
	Error   *jsonRpcError `json:"error,omitempty"`
}

func okResponse(id *int64, result any) jsonRpcResponse {
	// Match the C++ fork: missing id in a successful response becomes 0.
	var idVal any = int64(0)
	if id != nil {
		idVal = *id
	}
	return jsonRpcResponse{JsonRpc: "2.0", ID: idVal, Result: result}
}

func errorResponse(id *int64, code int, message string) jsonRpcResponse {
	// Match the C++ fork: missing id in an error response becomes JSON null.
	var idVal any = nil
	if id != nil {
		idVal = *id
	}
	return jsonRpcResponse{JsonRpc: "2.0", ID: idVal, Error: &jsonRpcError{Code: code, Message: message}}
}

func (b *Bridge) handleMessage(line []byte) jsonRpcResponse {
	var req jsonRpcRequest
	if err := json.Unmarshal(line, &req); err != nil {
		return errorResponse(nil, -32700, "Parse error: "+err.Error())
	}
	if req.Method == "" {
		return errorResponse(req.ID, -32600, "Missing or non-string 'method'")
	}

	// Parse params as a map so we can extract + strip _token.
	paramsMap := map[string]json.RawMessage{}
	if len(req.Params) > 0 {
		if err := json.Unmarshal(req.Params, &paramsMap); err != nil {
			return errorResponse(req.ID, -32602, "params must be an object")
		}
	}

	tokRaw, ok := paramsMap["_token"]
	if !ok {
		return errorResponse(req.ID, -32001, "Missing or invalid _token")
	}
	var tok string
	if err := json.Unmarshal(tokRaw, &tok); err != nil || tok != b.token {
		return errorResponse(req.ID, -32001, "Missing or invalid _token")
	}
	delete(paramsMap, "_token")

	b.mu.Lock()
	h, ok := b.handlers[req.Method]
	b.mu.Unlock()
	if !ok {
		return errorResponse(req.ID, -32601, "Method not found: "+req.Method)
	}

	pruned, err := json.Marshal(paramsMap)
	if err != nil {
		return errorResponse(req.ID, -32000, "internal: re-marshal params: "+err.Error())
	}

	result, err := h(pruned)
	if err != nil {
		return errorResponse(req.ID, -32000, "Handler threw: "+err.Error())
	}
	return okResponse(req.ID, result)
}

func makeToken() (string, error) {
	var buf [16]byte
	if _, err := rand.Read(buf[:]); err != nil {
		return "", err
	}
	return hex.EncodeToString(buf[:]), nil
}
