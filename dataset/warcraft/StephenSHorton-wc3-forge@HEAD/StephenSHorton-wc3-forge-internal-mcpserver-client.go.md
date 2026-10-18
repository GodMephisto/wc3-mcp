---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/mcpserver/client.go"
ref: HEAD
---

package mcpserver

// Bridge client for the in-binary MCP proxy. Port of mcp/src/bridge_client.ts +
// the discovery half of mcp/src/discovery.ts. When wc3-forge runs as `--mcp`,
// it is a *client* of a separately-running editor instance: it discovers live
// instances via their lockfiles (bridge.ListLocks) and forwards JSON-RPC over
// the same NDJSON/TCP wire the Node server uses, injecting params._token.

import (
	"bufio"
	"bytes"
	"encoding/json"
	"fmt"
	"net"
	"strconv"
	"strings"
	"sync"
	"time"

	"github.com/StephenSHorton/wc3-forge/internal/bridge"
)

const (
	requestTimeout = 30 * time.Second
	dialTimeout    = 5 * time.Second
)

// bridgeError carries a JSON-RPC error returned by the bridge (mirrors the TS
// BridgeError). Its message is formatted to match the Node server's output.
type bridgeError struct {
	Code    int
	Message string
}

func (e *bridgeError) Error() string {
	return fmt.Sprintf("Bridge error %d: %s", e.Code, e.Message)
}

// bridgeNotRunning means no live wc3-forge instance was found (or not the one
// requested). Mirrors the TS BridgeNotRunningError.
type bridgeNotRunning struct{ msg string }

func (e *bridgeNotRunning) Error() string { return e.msg }

// bridgeConn is a reused TCP connection to one wc3-forge bridge. Calls are
// serialized by mu, so there is only ever one request in flight per connection
// — the bridge handles requests synchronously per connection, so responses
// come back in order and id-matching is a defensive check, not a necessity.
type bridgeConn struct {
	lock bridge.LockInfo

	mu     sync.Mutex
	conn   net.Conn
	r      *bufio.Reader
	nextID int
}

func (c *bridgeConn) ensureConn() error {
	if c.conn != nil {
		return nil
	}
	conn, err := net.DialTimeout("tcp", fmt.Sprintf("127.0.0.1:%d", c.lock.Port), dialTimeout)
	if err != nil {
		return fmt.Errorf("dial bridge pid=%d port=%d: %w", c.lock.PID, c.lock.Port, err)
	}
	if tc, ok := conn.(*net.TCPConn); ok {
		_ = tc.SetNoDelay(true)
	}
	c.conn = conn
	c.r = bufio.NewReader(conn)
	return nil
}

func (c *bridgeConn) closeLocked() {
	if c.conn != nil {
		_ = c.conn.Close()
		c.conn = nil
		c.r = nil
	}
}

func (c *bridgeConn) call(method string, args json.RawMessage) (json.RawMessage, error) {
	c.mu.Lock()
	defer c.mu.Unlock()
	if err := c.ensureConn(); err != nil {
		return nil, err
	}

	c.nextID++
	id := c.nextID

	params, err := mergeToken(args, c.lock.Token)
	if err != nil {
		return nil, err
	}
	payload, err := json.Marshal(map[string]any{
		"jsonrpc": "2.0",
		"id":      id,
		"method":  method,
		"params":  params,
	})
	if err != nil {
		return nil, err
	}
	payload = append(payload, '\n')

	_ = c.conn.SetDeadline(time.Now().Add(requestTimeout))

	if _, err := c.conn.Write(payload); err != nil {
		c.closeLocked()
		return nil, fmt.Errorf("write to bridge: %w", err)
	}

	for {
		line, err := c.r.ReadBytes('\n')
		if err != nil {
			c.closeLocked()
			return nil, fmt.Errorf("read from bridge: %w", err)
		}
		line = bytes.TrimSpace(line)
		if len(line) == 0 {
			continue
		}
		var resp struct {
			ID     *int            `json:"id"`
			Result json.RawMessage `json:"result"`
			Error  *struct {
				Code    int    `json:"code"`
				Message string `json:"message"`
			} `json:"error"`
		}
		if err := json.Unmarshal(line, &resp); err != nil {
			// Bad frame; skip and keep reading for our id.
			continue
		}
		if resp.ID == nil || *resp.ID != id {
			continue
		}
		if resp.Error != nil {
			return nil, &bridgeError{Code: resp.Error.Code, Message: resp.Error.Message}
		}
		return resp.Result, nil
	}
}

// mergeToken decodes the MCP tool arguments (a JSON object, or empty) and adds
// the bridge auth token, matching the wire contract params._token.
func mergeToken(args json.RawMessage, token string) (map[string]json.RawMessage, error) {
	m := map[string]json.RawMessage{}
	if len(args) > 0 && string(bytes.TrimSpace(args)) != "null" {
		if err := json.Unmarshal(args, &m); err != nil {
			return nil, fmt.Errorf("arguments must be a JSON object: %w", err)
		}
	}
	tok, err := json.Marshal(token)
	if err != nil {
		return nil, err
	}
	m["_token"] = tok
	return m, nil
}

// sessionPool keeps one bridgeConn per pid and a "selected" pid that all calls
// route to (0 = none selected → oldest running instance). Mirrors the TS
// SessionPool.
type sessionPool struct {
	mu          sync.Mutex
	conns       map[int]*bridgeConn
	selectedPID int
}

func newSessionPool() *sessionPool {
	return &sessionPool{conns: map[int]*bridgeConn{}}
}

// callPID forwards a JSON-RPC call to a specific instance (pid 0 = oldest).
func (p *sessionPool) callPID(pid int, method string, args json.RawMessage) (json.RawMessage, error) {
	lock, err := findSession(pid)
	if err != nil {
		return nil, err
	}
	p.mu.Lock()
	conn := p.conns[lock.PID]
	if conn == nil || conn.lock.Port != lock.Port {
		// New instance, or pid reused on a different port → fresh connection.
		conn = &bridgeConn{lock: lock}
		p.conns[lock.PID] = conn
	}
	p.mu.Unlock()
	return conn.call(method, args)
}

// call routes to the selected instance (or oldest when none selected).
func (p *sessionPool) call(method string, args json.RawMessage) (json.RawMessage, error) {
	return p.callPID(p.selected(), method, args)
}

func (p *sessionPool) selectPID(pid int)   { p.mu.Lock(); p.selectedPID = pid; p.mu.Unlock() }
func (p *sessionPool) clearSelection()     { p.mu.Lock(); p.selectedPID = 0; p.mu.Unlock() }
func (p *sessionPool) selected() (pid int) { p.mu.Lock(); defer p.mu.Unlock(); return p.selectedPID }

// findSession resolves a pid (0 = oldest running) to its lockfile, enumerating
// live instances via bridge.ListLocks. Mirrors discovery.ts findSession.
func findSession(pid int) (bridge.LockInfo, error) {
	locks := bridge.ListLocks()
	if len(locks) == 0 {
		return bridge.LockInfo{}, &bridgeNotRunning{
			msg: fmt.Sprintf("No wc3-forge bridges found in %s. Launch wc3-forge (the MCP bridge is on by default).", bridge.LockDir()),
		}
	}
	if pid == 0 {
		return locks[0], nil // oldest first
	}
	for _, l := range locks {
		if l.PID == pid {
			return l, nil
		}
	}
	return bridge.LockInfo{}, &bridgeNotRunning{
		msg: fmt.Sprintf("No wc3-forge bridge for pid=%d. Active pids: %s.", pid, joinPIDs(locks)),
	}
}

func joinPIDs(locks []bridge.LockInfo) string {
	pids := make([]string, len(locks))
	for i, l := range locks {
		pids[i] = strconv.Itoa(l.PID)
	}
	return strings.Join(pids, ", ")
}
