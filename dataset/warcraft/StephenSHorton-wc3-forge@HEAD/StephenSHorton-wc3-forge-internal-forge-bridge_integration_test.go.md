---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/bridge_integration_test.go"
ref: HEAD
---

// Package forge — bridge-level integration tests.
//
// Where handlers_test.go calls handler functions directly (so it exercises the
// handler signature but bypasses framing/auth/JSON-RPC shape), this file drives
// the bridge end-to-end as a real TCP client would. Token auth, NDJSON framing,
// per-request connection lifetime, and the standard JSON-RPC error envelope
// all live on the wire side and get coverage here.
//
// Each test spins up its own bridge.Bridge on an OS-chosen ephemeral port
// (127.0.0.1:0), reads the resolved port + token off the Bridge instance, and
// tears down via b.Stop(). The forge.Current singleton is saved + restored
// per-test so a Session.Open in one test doesn't bleed into the next.
//
// The bridge's lockfile lives at $WC3FORGE_MCP_LOCK_DIR/<pid>.lock; we override
// that env var to a per-test temp dir so a parallel real session's lockfile is
// never clobbered.
package forge

import (
	"bufio"
	"encoding/binary"
	"encoding/json"
	"fmt"
	"io"
	"math"
	"net"
	"os"
	"path/filepath"
	"sync/atomic"
	"testing"
	"time"

	"github.com/StephenSHorton/wc3-forge/internal/bridge"
)

// -----------------------------------------------------------------------------
// Helpers
// -----------------------------------------------------------------------------

// startBridge spins up a bridge.Bridge listening on an OS-chosen ephemeral port,
// registers every wc3-forge handler, and returns a teardown closure. It also
// overrides WC3FORGE_MCP_LOCK_DIR to a test-scoped temp dir so the bridge's
// per-pid lockfile lands somewhere isolated (and gets cleaned up) instead of
// clobbering a real interactive session's lockfile under ~/.wc3-forge/mcp/.
//
// The Current session singleton is saved + zeroed; t.Cleanup restores it after
// the test (and after b.Stop). This means each test starts with a "no map
// loaded" session and any Open from a prior test doesn't bleed in.
func startBridge(t *testing.T) (b *bridge.Bridge, port int, token string) {
	t.Helper()

	// Isolate the bridge's lockfile to a temp dir.
	lockDir := t.TempDir()
	prevLockDir, hadLockDir := os.LookupEnv("WC3FORGE_MCP_LOCK_DIR")
	if err := os.Setenv("WC3FORGE_MCP_LOCK_DIR", lockDir); err != nil {
		t.Fatalf("set WC3FORGE_MCP_LOCK_DIR: %v", err)
	}

	// Save + reset the Current session singleton. Tests open maps + mutate
	// selection; without this isolation a test that fails partway through
	// would poison the next one.
	prevCurrent := Current
	Current = &Session{}

	b = bridge.New()
	RegisterAll(b)
	if err := b.Start(); err != nil {
		// Restore on early failure.
		Current = prevCurrent
		if hadLockDir {
			_ = os.Setenv("WC3FORGE_MCP_LOCK_DIR", prevLockDir)
		} else {
			_ = os.Unsetenv("WC3FORGE_MCP_LOCK_DIR")
		}
		t.Fatalf("bridge.Start: %v", err)
	}

	t.Cleanup(func() {
		b.Stop()
		Current = prevCurrent
		if hadLockDir {
			_ = os.Setenv("WC3FORGE_MCP_LOCK_DIR", prevLockDir)
		} else {
			_ = os.Unsetenv("WC3FORGE_MCP_LOCK_DIR")
		}
	})

	return b, b.Port(), b.Token()
}

// rpcResponse mirrors the bridge's JSON-RPC response envelope on the wire.
// Result is left as RawMessage so tests can decode it into a concrete shape.
type rpcResponse struct {
	JsonRpc string          `json:"jsonrpc"`
	ID      any             `json:"id"`
	Result  json.RawMessage `json:"result,omitempty"`
	Error   *rpcError       `json:"error,omitempty"`
}

type rpcError struct {
	Code    int    `json:"code"`
	Message string `json:"message"`
	Data    any    `json:"data,omitempty"`
}

// nextRequestID hands out monotonically-increasing JSON-RPC ids for tests so
// the (request, response) pairing on the wire is unambiguous. Atomic so
// parallel tests don't collide if t.Parallel ever gets opted into later.
var nextRequestID int64

// rpc opens a fresh TCP connection to (port), writes one JSON-RPC request as
// a single NDJSON line (request + "\n"), reads one response line, closes, and
// returns the parsed envelope. Auto-injects _token=tok into the params object.
//
// Pass token="" to deliberately omit the token (for auth-rejection tests). Pass
// params=nil to send no params at all (the bridge still requires _token; an
// empty params payload becomes `{"_token":"…"}`).
func rpc(t *testing.T, port int, token string, method string, params map[string]any) rpcResponse {
	t.Helper()
	conn := dial(t, port)
	defer conn.Close()
	id := atomic.AddInt64(&nextRequestID, 1)
	resp := rpcSend(t, conn, id, token, method, params)
	return resp
}

// rpcSend writes one request and reads one response on an already-open conn,
// preserving the connection for reuse by the caller. Used by the framing test
// to issue multiple requests back-to-back on a single connection.
func rpcSend(t *testing.T, conn net.Conn, id int64, token string, method string, params map[string]any) rpcResponse {
	t.Helper()

	// Build params. The bridge requires _token to live INSIDE the params
	// object. When token is the empty string, send no _token at all (so the
	// auth-rejection test can prove the bridge rejects it).
	p := map[string]any{}
	for k, v := range params {
		p[k] = v
	}
	if token != "" {
		p["_token"] = token
	}

	req := map[string]any{
		"jsonrpc": "2.0",
		"id":      id,
		"method":  method,
		"params":  p,
	}
	body, err := json.Marshal(req)
	if err != nil {
		t.Fatalf("marshal request: %v", err)
	}
	body = append(body, '\n')

	if err := conn.SetWriteDeadline(time.Now().Add(2 * time.Second)); err != nil {
		t.Fatalf("set write deadline: %v", err)
	}
	if _, err := conn.Write(body); err != nil {
		t.Fatalf("write request: %v", err)
	}

	if err := conn.SetReadDeadline(time.Now().Add(2 * time.Second)); err != nil {
		t.Fatalf("set read deadline: %v", err)
	}
	br := bufio.NewReader(conn)
	line, err := br.ReadBytes('\n')
	if err != nil && err != io.EOF {
		t.Fatalf("read response: %v", err)
	}
	var out rpcResponse
	if err := json.Unmarshal(line, &out); err != nil {
		t.Fatalf("unmarshal response %q: %v", line, err)
	}
	return out
}

// rpcRaw writes an arbitrary byte payload (e.g. malformed JSON) on a fresh
// connection, reads one response line, returns the raw bytes + parse outcome.
// Used for the parse-error branch of the framing test where a structured
// request would defeat the purpose.
func rpcRaw(t *testing.T, port int, payload []byte) ([]byte, error) {
	t.Helper()
	conn := dial(t, port)
	defer conn.Close()
	if err := conn.SetWriteDeadline(time.Now().Add(2 * time.Second)); err != nil {
		t.Fatalf("set write deadline: %v", err)
	}
	if _, err := conn.Write(payload); err != nil {
		t.Fatalf("write payload: %v", err)
	}
	if err := conn.SetReadDeadline(time.Now().Add(2 * time.Second)); err != nil {
		t.Fatalf("set read deadline: %v", err)
	}
	br := bufio.NewReader(conn)
	line, err := br.ReadBytes('\n')
	return line, err
}

func dial(t *testing.T, port int) net.Conn {
	t.Helper()
	conn, err := net.DialTimeout("tcp", fmt.Sprintf("127.0.0.1:%d", port), 2*time.Second)
	if err != nil {
		t.Fatalf("dial 127.0.0.1:%d: %v", port, err)
	}
	return conn
}

// writeMinimalW3I writes a tiny synthetic war3map.w3i at path/war3map.w3i.
//
// The wc3-forge w3i parser is lenient at the tail (TruncatedAt marks any
// optional section that runs out of bytes), so we only need:
//   - file_version: 25 (FileVersionTFT — avoids the v28+ branch that wants
//     game_version + lua_word and the v31+/v32+/v33+ extras we don't have)
//   - map_version, editor_version: i32
//   - name, author, description, suggested_players: c-strings (just null bytes)
//   - 4 camera vec2 (4*8 = 32 bytes of zeros)
//   - camera_complements ivec4 (16 bytes of zeros)
//   - playable_width/height: i32
//   - flags: u32 (0)
//   - tileset: u8 (0)
//   - v25 branch: loading_screen_number(4) + 4 nulls + game_data_set(4) +
//                 4 nulls + fog struct (4+4+4+4+4) + weather_id(4) +
//                 custom_sound_env null(1) + custom_light_tileset(1) + water_color(4)
//   - player_count: 0
//   - force_count: 0
//
// After the forces the parser flags `mark("upgrades")` and returns gracefully.
//
// Choosing v25 over the Reforged versions is deliberate: it gives the smallest
// header path (no game_version, no lua marker, no cam-distance trailers).
func writeMinimalW3I(t *testing.T, dir string) {
	t.Helper()
	var buf []byte
	u32 := func(v uint32) {
		var b [4]byte
		binary.LittleEndian.PutUint32(b[:], v)
		buf = append(buf, b[:]...)
	}
	i32 := func(v int32) { u32(uint32(v)) }
	f32 := func(v float32) { u32(math.Float32bits(v)) }
	cstr := func(s string) { buf = append(buf, []byte(s)...); buf = append(buf, 0) }
	u8 := func(v uint8) { buf = append(buf, v) }
	color := func() { buf = append(buf, 0, 0, 0, 0) }

	i32(25)            // file_version
	i32(1)             // map_version
	i32(6072)          // editor_version (any plausible value)
	cstr("Test Map")   // name
	cstr("Tester")     // author
	cstr("")           // description
	cstr("1")          // suggested_players
	// camera bounds: 4 vec2 (8 bytes each)
	for i := 0; i < 8; i++ {
		f32(0)
	}
	// camera_complements ivec4
	for i := 0; i < 4; i++ {
		i32(0)
	}
	i32(64) // playable_width
	i32(64) // playable_height
	u32(0)  // flags
	u8(0)   // tileset
	// v25 branch
	i32(0)        // loading_screen.Number
	cstr("")      // loading_screen.Model
	cstr("")      // loading_screen.Text
	cstr("")      // loading_screen.Title
	cstr("")      // loading_screen.Subtitle
	i32(0)        // game_data_set
	cstr("")      // prologue.Model
	cstr("")      // prologue.Text
	cstr("")      // prologue.Title
	cstr("")      // prologue.Subtitle
	i32(0)        // fog.style
	f32(0)        // fog.startZ
	f32(0)        // fog.endZ
	f32(0)        // fog.density
	color()       // fog.color
	i32(0)        // weather_id
	cstr("")      // custom_sound_env
	u8(0)         // custom_light_tileset
	color()       // water_color
	u32(0)        // player_count
	u32(0)        // force_count
	// Parser hits remaining()<4 on the "upgrades" check and returns cleanly.

	path := filepath.Join(dir, "war3map.w3i")
	if err := os.WriteFile(path, buf, 0o644); err != nil {
		t.Fatalf("write %s: %v", path, err)
	}
}

// makeMapFixture builds a temp directory containing the minimum files needed
// for Session.Open to succeed AND for units.list to return the vendored
// wc3-survival entities. Returns the directory path.
//
// Files produced:
//   - war3map.w3i:        synthesized minimal v25 info blob (see writeMinimalW3I)
//   - war3mapUnits.doo:   copied from internal/formats/unitsdoo/testdata/wc3_survival_v1_6.doo
func makeMapFixture(t *testing.T) string {
	t.Helper()
	tmp := t.TempDir()
	writeMinimalW3I(t, tmp)

	// Copy the vendored doo fixture. The test file lives in internal/forge/
	// and the fixture lives in internal/formats/unitsdoo/testdata/ — that's
	// "../formats/unitsdoo/testdata/wc3_survival_v1_6.doo" from this package's
	// working dir at test time.
	src := filepath.Join("..", "formats", "unitsdoo", "testdata", "wc3_survival_v1_6.doo")
	data, err := os.ReadFile(src)
	if err != nil {
		t.Fatalf("read doo fixture %s: %v", src, err)
	}
	dst := filepath.Join(tmp, "war3mapUnits.doo")
	if err := os.WriteFile(dst, data, 0o644); err != nil {
		t.Fatalf("write %s: %v", dst, err)
	}
	return tmp
}

// expectOK fails the test if the response carries an error envelope. Returns
// the result raw message for downstream decode.
func expectOK(t *testing.T, resp rpcResponse, method string) json.RawMessage {
	t.Helper()
	if resp.Error != nil {
		t.Fatalf("%s: expected ok, got error code=%d message=%q", method, resp.Error.Code, resp.Error.Message)
	}
	return resp.Result
}

// expectErr fails the test if the response does not carry an error with the
// given code.
func expectErr(t *testing.T, resp rpcResponse, method string, wantCode int) *rpcError {
	t.Helper()
	if resp.Error == nil {
		t.Fatalf("%s: expected error code=%d, got ok result=%s", method, wantCode, string(resp.Result))
	}
	if resp.Error.Code != wantCode {
		t.Fatalf("%s: expected error code=%d, got code=%d message=%q", method, wantCode, resp.Error.Code, resp.Error.Message)
	}
	return resp.Error
}

// -----------------------------------------------------------------------------
// Tests
// -----------------------------------------------------------------------------

// TestBridge_Ping exercises the simplest happy path: bridge.ping with the
// correct token, no map loaded. Asserts the envelope shape:
//
//	{ ok: true, version: <bridge.Version>, map_loaded: false }
func TestBridge_Ping(t *testing.T) {
	_, port, tok := startBridge(t)

	resp := rpc(t, port, tok, "bridge.ping", nil)
	raw := expectOK(t, resp, "bridge.ping")

	var got struct {
		OK        bool   `json:"ok"`
		Version   string `json:"version"`
		MapLoaded bool   `json:"map_loaded"`
	}
	if err := json.Unmarshal(raw, &got); err != nil {
		t.Fatalf("decode ping result: %v", err)
	}
	if !got.OK {
		t.Errorf("ok=false, want true")
	}
	if got.Version != bridge.Version {
		t.Errorf("version=%q, want %q", got.Version, bridge.Version)
	}
	if got.MapLoaded {
		t.Errorf("map_loaded=true on fresh bridge, want false")
	}
}

// TestBridge_AuthRejected proves token auth actually works end-to-end. Without
// this, every other "happy path" test could be passing for the wrong reason
// (the bridge happily accepting any token).
//
// Auth code per the bridge: -32001 ("Missing or invalid _token") — the same
// code is returned for both no-token and wrong-token cases (the bridge doesn't
// distinguish, to avoid leaking "user does exist, just wrong creds" signal).
func TestBridge_AuthRejected(t *testing.T) {
	_, port, tok := startBridge(t)

	// Pick any registered method. ping is fine.
	const method = "bridge.ping"

	// 1. No token.
	resp := rpc(t, port, "" /* no token */, method, nil)
	expectErr(t, resp, method+" (no token)", -32001)

	// 2. Wrong token.
	resp = rpc(t, port, "definitely-not-the-token-deadbeef", method, nil)
	expectErr(t, resp, method+" (wrong token)", -32001)

	// 3. Correct token — should succeed.
	resp = rpc(t, port, tok, method, nil)
	expectOK(t, resp, method+" (correct token)")
}

// TestBridge_Framing covers the NDJSON wire contract:
//   - Two requests back-to-back on the same connection (separated by "\n")
//     produce two responses in order.
//   - A malformed (non-JSON) line produces a parse-error response (code -32700).
//
// The bridge's serveClient is a read-line-loop; both scenarios live or die on
// it framing per "\n" correctly.
func TestBridge_Framing(t *testing.T) {
	_, port, tok := startBridge(t)

	// Two pings on a single connection.
	conn := dial(t, port)
	defer conn.Close()
	id1 := atomic.AddInt64(&nextRequestID, 1)
	id2 := atomic.AddInt64(&nextRequestID, 1)

	// Write both requests before reading either — exercises the bridge's
	// ability to buffer + handle pipelined requests.
	build := func(id int64) []byte {
		req := map[string]any{
			"jsonrpc": "2.0",
			"id":      id,
			"method":  "bridge.ping",
			"params":  map[string]any{"_token": tok},
		}
		b, err := json.Marshal(req)
		if err != nil {
			t.Fatalf("marshal: %v", err)
		}
		return append(b, '\n')
	}

	if err := conn.SetWriteDeadline(time.Now().Add(2 * time.Second)); err != nil {
		t.Fatalf("set write deadline: %v", err)
	}
	payload := append(build(id1), build(id2)...)
	if _, err := conn.Write(payload); err != nil {
		t.Fatalf("write payload: %v", err)
	}

	if err := conn.SetReadDeadline(time.Now().Add(2 * time.Second)); err != nil {
		t.Fatalf("set read deadline: %v", err)
	}
	br := bufio.NewReader(conn)
	for i, wantID := range []int64{id1, id2} {
		line, err := br.ReadBytes('\n')
		if err != nil {
			t.Fatalf("read response %d: %v", i, err)
		}
		var resp rpcResponse
		if err := json.Unmarshal(line, &resp); err != nil {
			t.Fatalf("unmarshal response %d %q: %v", i, line, err)
		}
		// IDs come back as float64 through generic any-decode; compare via the
		// JSON-RPC convention of "id matches what we sent".
		idF, ok := resp.ID.(float64)
		if !ok {
			t.Fatalf("response %d: id is %T, want number", i, resp.ID)
		}
		if int64(idF) != wantID {
			t.Errorf("response %d: id=%d, want %d", i, int64(idF), wantID)
		}
		if resp.Error != nil {
			t.Errorf("response %d: unexpected error code=%d message=%q", i, resp.Error.Code, resp.Error.Message)
		}
	}

	// Parse-error path: malformed JSON on a fresh connection. Bridge should
	// answer with code=-32700 and (per bridge.go's errorResponse) the id
	// becomes JSON null because no parseable id was available.
	line, err := rpcRaw(t, port, []byte("this is not json\n"))
	if err != nil && err != io.EOF {
		t.Fatalf("rpcRaw: %v", err)
	}
	var pErr rpcResponse
	if err := json.Unmarshal(line, &pErr); err != nil {
		t.Fatalf("unmarshal parse-error response %q: %v", line, err)
	}
	if pErr.Error == nil {
		t.Fatalf("expected error envelope for malformed line, got %s", string(line))
	}
	if pErr.Error.Code != -32700 {
		t.Errorf("parse error code=%d, want -32700", pErr.Error.Code)
	}
}

// TestBridge_MapStatus_WhenNoMap asserts map.status returns loaded:false on a
// fresh bridge with no map opened.
func TestBridge_MapStatus_WhenNoMap(t *testing.T) {
	_, port, tok := startBridge(t)

	resp := rpc(t, port, tok, "map.status", nil)
	raw := expectOK(t, resp, "map.status")

	var got struct {
		Loaded    bool   `json:"loaded"`
		Path      string `json:"path,omitempty"`
		UnitCount int    `json:"unit_count,omitempty"`
	}
	if err := json.Unmarshal(raw, &got); err != nil {
		t.Fatalf("decode result: %v", err)
	}
	if got.Loaded {
		t.Errorf("loaded=true on fresh bridge, want false")
	}
}

// TestBridge_MapOpen_Folder drives map.open against the synthesized folder
// fixture and confirms map.status flips to loaded:true with unit_count>0
// (the wc3-survival doo fixture has 10 entities).
func TestBridge_MapOpen_Folder(t *testing.T) {
	_, port, tok := startBridge(t)

	dir := makeMapFixture(t)

	resp := rpc(t, port, tok, "map.open", map[string]any{"path": dir})
	raw := expectOK(t, resp, "map.open")

	var openResult struct {
		Loaded    bool `json:"loaded"`
		UnitCount int  `json:"unit_count,omitempty"`
	}
	if err := json.Unmarshal(raw, &openResult); err != nil {
		t.Fatalf("decode result: %v", err)
	}
	if !openResult.Loaded {
		t.Errorf("loaded=false after map.open, want true")
	}
	if openResult.UnitCount == 0 {
		t.Errorf("unit_count=0 after map.open, want >0")
	}

	// map.status should agree.
	resp = rpc(t, port, tok, "map.status", nil)
	raw = expectOK(t, resp, "map.status")
	var status struct {
		Loaded    bool `json:"loaded"`
		UnitCount int  `json:"unit_count,omitempty"`
	}
	if err := json.Unmarshal(raw, &status); err != nil {
		t.Fatalf("decode status: %v", err)
	}
	if !status.Loaded {
		t.Errorf("status.loaded=false after map.open")
	}
	if status.UnitCount == 0 {
		t.Errorf("status.unit_count=0 after map.open")
	}
}

// TestBridge_UnitsList confirms the Paladin entity from the vendored fixture
// round-trips through the wire correctly. The fixture has 10 entities; we
// look up creation_number=21 (Hpal at position [-400, 300, 0]) and assert the
// expected fields.
func TestBridge_UnitsList(t *testing.T) {
	_, port, tok := startBridge(t)
	dir := makeMapFixture(t)
	expectOK(t, rpc(t, port, tok, "map.open", map[string]any{"path": dir}), "map.open")

	resp := rpc(t, port, tok, "units.list", nil)
	raw := expectOK(t, resp, "units.list")

	var list struct {
		Entities []struct {
			CreationNumber uint32     `json:"creation_number"`
			TypeID         string     `json:"type_id"`
			Position       [3]float32 `json:"position"`
			Player         uint32     `json:"player"`
		} `json:"entities"`
	}
	if err := json.Unmarshal(raw, &list); err != nil {
		t.Fatalf("decode result: %v", err)
	}
	if len(list.Entities) == 0 {
		t.Fatalf("entities empty")
	}

	var pal *struct {
		CreationNumber uint32     `json:"creation_number"`
		TypeID         string     `json:"type_id"`
		Position       [3]float32 `json:"position"`
		Player         uint32     `json:"player"`
	}
	for i := range list.Entities {
		if list.Entities[i].CreationNumber == 21 {
			pal = &list.Entities[i]
			break
		}
	}
	if pal == nil {
		t.Fatalf("Paladin (creation_number=21) not in entities list")
	}
	if pal.TypeID != "Hpal" {
		t.Errorf("Paladin type_id=%q, want %q", pal.TypeID, "Hpal")
	}
	if pal.Position != [3]float32{-400, 300, 0} {
		t.Errorf("Paladin position=%v, want [-400 300 0]", pal.Position)
	}
}

// TestBridge_SelectionRoundTrip exercises selection.set + selection.get +
// selection.clear. Selection doesn't require a map; we can drive it without
// map.open.
func TestBridge_SelectionRoundTrip(t *testing.T) {
	_, port, tok := startBridge(t)

	// Empty to start.
	//
	// NB: a freshly-constructed Session has selection.Primary=0 (struct zero
	// value), NOT -1. Primary only becomes -1 after Open/Close/Clear, which
	// explicitly reset it. The wire contract treats "primary=0 with no items"
	// the same as "no primary", so this isn't a correctness issue — it just
	// surprised me when writing the test. Documented for posterity.
	resp := rpc(t, port, tok, "selection.get", nil)
	raw := expectOK(t, resp, "selection.get")
	var sel struct {
		Items   []SelectionItem `json:"items"`
		Primary int             `json:"primary"`
	}
	if err := json.Unmarshal(raw, &sel); err != nil {
		t.Fatalf("decode initial selection: %v", err)
	}
	if len(sel.Items) != 0 {
		t.Errorf("initial selection items=%v, want empty", sel.Items)
	}

	// Set one item.
	resp = rpc(t, port, tok, "selection.set", map[string]any{
		"items":   []map[string]any{{"kind": "unit", "id": 21}},
		"primary": 0,
	})
	raw = expectOK(t, resp, "selection.set")
	if err := json.Unmarshal(raw, &sel); err != nil {
		t.Fatalf("decode after-set selection: %v", err)
	}
	if len(sel.Items) != 1 || sel.Items[0].Kind != "unit" || sel.Items[0].ID != 21 {
		t.Errorf("after selection.set: items=%+v, want [unit/21]", sel.Items)
	}

	// selection.get should agree.
	resp = rpc(t, port, tok, "selection.get", nil)
	raw = expectOK(t, resp, "selection.get")
	if err := json.Unmarshal(raw, &sel); err != nil {
		t.Fatalf("decode selection.get: %v", err)
	}
	if len(sel.Items) != 1 || sel.Items[0].ID != 21 {
		t.Errorf("selection.get items=%+v, want [unit/21]", sel.Items)
	}

	// Clear.
	resp = rpc(t, port, tok, "selection.clear", nil)
	raw = expectOK(t, resp, "selection.clear")
	if err := json.Unmarshal(raw, &sel); err != nil {
		t.Fatalf("decode selection.clear: %v", err)
	}
	if len(sel.Items) != 0 {
		t.Errorf("after selection.clear: items=%+v, want empty", sel.Items)
	}

	// selection.get again — still empty.
	resp = rpc(t, port, tok, "selection.get", nil)
	raw = expectOK(t, resp, "selection.get")
	if err := json.Unmarshal(raw, &sel); err != nil {
		t.Fatalf("decode selection.get post-clear: %v", err)
	}
	if len(sel.Items) != 0 {
		t.Errorf("selection.get after clear: items=%+v, want empty", sel.Items)
	}
}

// TestBridge_UnitsMove drives the units.move handler against the loaded
// Paladin, then verifies units.list reflects the new position. Also confirms
// a no-op (same-position) move returns ok without flipping any wire-level
// error — the no-op short-circuit lives inside Session.MoveUnit and is
// transparent at the handler/wire level.
func TestBridge_UnitsMove(t *testing.T) {
	_, port, tok := startBridge(t)
	dir := makeMapFixture(t)
	expectOK(t, rpc(t, port, tok, "map.open", map[string]any{"path": dir}), "map.open")

	// Move Paladin to (1000, 1500, 0).
	resp := rpc(t, port, tok, "units.move", map[string]any{
		"creation_number": 21,
		"x":               1000,
		"y":               1500,
		"z":               0,
	})
	raw := expectOK(t, resp, "units.move")
	var moveResult struct {
		OK             bool       `json:"ok"`
		CreationNumber uint32     `json:"creation_number"`
		Position       [3]float32 `json:"position"`
	}
	if err := json.Unmarshal(raw, &moveResult); err != nil {
		t.Fatalf("decode units.move: %v", err)
	}
	if !moveResult.OK {
		t.Errorf("units.move ok=false")
	}
	if moveResult.CreationNumber != 21 {
		t.Errorf("units.move creation_number=%d, want 21", moveResult.CreationNumber)
	}
	if moveResult.Position != [3]float32{1000, 1500, 0} {
		t.Errorf("units.move position=%v, want [1000 1500 0]", moveResult.Position)
	}

	// units.list should reflect new position.
	resp = rpc(t, port, tok, "units.list", nil)
	raw = expectOK(t, resp, "units.list")
	var list struct {
		Entities []struct {
			CreationNumber uint32     `json:"creation_number"`
			Position       [3]float32 `json:"position"`
		} `json:"entities"`
	}
	if err := json.Unmarshal(raw, &list); err != nil {
		t.Fatalf("decode units.list: %v", err)
	}
	var palPos [3]float32
	var foundPal bool
	for _, e := range list.Entities {
		if e.CreationNumber == 21 {
			palPos = e.Position
			foundPal = true
			break
		}
	}
	if !foundPal {
		t.Fatalf("Paladin (cn=21) missing from list after move")
	}
	if palPos != [3]float32{1000, 1500, 0} {
		t.Errorf("Paladin position after move=%v, want [1000 1500 0]", palPos)
	}

	// Same-position move should still succeed at the wire level. Session.MoveUnit
	// short-circuits internally (no dirty flip) but returns nil, so the handler
	// returns ok:true here.
	resp = rpc(t, port, tok, "units.move", map[string]any{
		"creation_number": 21,
		"x":               1000,
		"y":               1500,
		"z":               0,
	})
	raw = expectOK(t, resp, "units.move (no-op)")
	if err := json.Unmarshal(raw, &moveResult); err != nil {
		t.Fatalf("decode units.move (no-op): %v", err)
	}
	if !moveResult.OK {
		t.Errorf("no-op units.move ok=false")
	}
}

// TestBridge_MapSave_Folder is the end-to-end save smoke test over the wire:
// open the folder fixture, move the Paladin, save, verify the on-disk
// war3mapUnits.doo bytes changed. Then restore + save and verify byte-equality
// with the original.
func TestBridge_MapSave_Folder(t *testing.T) {
	_, port, tok := startBridge(t)
	dir := makeMapFixture(t)
	dooPath := filepath.Join(dir, "war3mapUnits.doo")

	origBytes, err := os.ReadFile(dooPath)
	if err != nil {
		t.Fatalf("read original doo: %v", err)
	}

	expectOK(t, rpc(t, port, tok, "map.open", map[string]any{"path": dir}), "map.open")

	// Move Paladin then save.
	expectOK(t, rpc(t, port, tok, "units.move", map[string]any{
		"creation_number": 21,
		"x":               1234,
		"y":               5678,
		"z":               0,
	}), "units.move")

	resp := rpc(t, port, tok, "map.save", nil)
	raw := expectOK(t, resp, "map.save")
	var saved map[string]any
	if err := json.Unmarshal(raw, &saved); err != nil {
		t.Fatalf("decode map.save: %v", err)
	}
	if saved["ok"] != true {
		t.Errorf("map.save ok=%v, want true", saved["ok"])
	}

	// File should have changed bytes.
	movedBytes, err := os.ReadFile(dooPath)
	if err != nil {
		t.Fatalf("read moved doo: %v", err)
	}
	if string(movedBytes) == string(origBytes) {
		t.Errorf("war3mapUnits.doo bytes unchanged after move+save")
	}

	// Restore Paladin to original (-400, 300, 0) and save — file should be
	// byte-identical to original (unitsdoo.Encode is byte-faithful per its
	// round-trip test).
	expectOK(t, rpc(t, port, tok, "units.move", map[string]any{
		"creation_number": 21,
		"x":               -400,
		"y":               300,
		"z":               0,
	}), "units.move (restore)")
	expectOK(t, rpc(t, port, tok, "map.save", nil), "map.save (restore)")
	restoredBytes, err := os.ReadFile(dooPath)
	if err != nil {
		t.Fatalf("read restored doo: %v", err)
	}
	if string(restoredBytes) != string(origBytes) {
		t.Errorf("war3mapUnits.doo not byte-identical to original after restore+save (orig=%d bytes, restored=%d bytes)",
			len(origBytes), len(restoredBytes))
	}
}
