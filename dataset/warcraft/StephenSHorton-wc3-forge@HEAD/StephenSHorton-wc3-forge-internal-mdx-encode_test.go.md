---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/mdx/encode_test.go"
ref: HEAD
---

package mdx

import (
	"encoding/binary"
	"math"
	"testing"
)

// cube returns a unit cube Model (8 verts, 12 tris, 1 submesh) with per-vertex
// normals, UVs, and a texture path. CCW winding is not asserted by the encoder;
// the geometry just needs to exercise the byte layout.
func cube() *Model {
	positions := [][3]float32{
		{0, 0, 0}, {1, 0, 0}, {1, 1, 0}, {0, 1, 0}, // bottom (z=0)
		{0, 0, 1}, {1, 0, 1}, {1, 1, 1}, {0, 1, 1}, // top (z=1)
	}
	normals := make([][3]float32, 8)
	uvs := make([][2]float32, 8)
	for i := range normals {
		normals[i] = [3]float32{0, 0, 1}
		uvs[i] = [2]float32{float32(i&1) * 1.0, float32((i>>1)&1) * 1.0}
	}
	indices := []uint32{
		0, 1, 2, 0, 2, 3, // bottom
		4, 6, 5, 4, 7, 6, // top
		0, 4, 5, 0, 5, 1, // sides
		1, 5, 6, 1, 6, 2,
		2, 6, 7, 2, 7, 3,
		3, 7, 4, 3, 4, 0,
	}
	return &Model{
		Name:      "cube",
		Positions: positions,
		Normals:   normals,
		UVs:       uvs,
		Submeshes: []Submesh{{Indices: indices, TexturePath: "Textures\\white.blp"}},
	}
}

// --- chunk-walking helpers --------------------------------------------------

type chunk struct {
	tag  string
	body []byte
}

// walkTopChunks parses the top-level MDX chunk list (after the MDLX magic).
// Every top-level chunk is tag(4) + uint32 size + body.
func walkTopChunks(t *testing.T, data []byte) []chunk {
	t.Helper()
	if len(data) < 4 || string(data[:4]) != "MDLX" {
		t.Fatalf("missing MDLX magic, got %q", safePrefix(data, 4))
	}
	var chunks []chunk
	off := 4
	for off+8 <= len(data) {
		tag := string(data[off : off+4])
		size := binary.LittleEndian.Uint32(data[off+4 : off+8])
		off += 8
		end := off + int(size)
		if end > len(data) {
			t.Fatalf("chunk %q size %d overruns buffer at off %d", tag, size, off)
		}
		chunks = append(chunks, chunk{tag: tag, body: data[off:end]})
		off = end
	}
	if off != len(data) {
		t.Fatalf("trailing %d bytes after last chunk", len(data)-off)
	}
	return chunks
}

func findChunk(chunks []chunk, tag string) (chunk, bool) {
	for _, c := range chunks {
		if c.tag == tag {
			return c, true
		}
	}
	return chunk{}, false
}

// geosetSubChunks walks the fixed-order, count-prefixed leading sub-chunks of a
// single geoset body (after its inclusiveSize has been stripped) and returns a
// map tag -> payload (the bytes after the tag's uint32 count, count included).
//
// We walk structurally rather than scanning for tag bytes, because float
// vertex/normal data can coincidentally contain a 4-byte run equal to a tag.
// The sub-chunks here are all tag + uint32 count + count*elemSize, with known
// element sizes, so the walk is deterministic. We stop after MATS (the bare
// scalar fields that follow have no tag).
func geosetSubChunks(t *testing.T, body []byte) map[string][]byte {
	t.Helper()
	out := map[string][]byte{}
	// elemSize per tag (bytes per element). PTYP/PCNT/MTGC/MATS are uint32.
	elem := map[string]int{
		"VRTX": 12, "NRMS": 12, "PTYP": 4, "PCNT": 4,
		"PVTX": 2, "GNDX": 1, "MTGC": 4, "MATS": 4,
	}
	order := []string{"VRTX", "NRMS", "PTYP", "PCNT", "PVTX", "GNDX", "MTGC", "MATS"}
	off := 0
	for _, tag := range order {
		if off+8 > len(body) {
			t.Fatalf("ran out of bytes before sub-chunk %q", tag)
		}
		got := string(body[off : off+4])
		if got != tag {
			t.Fatalf("expected sub-chunk %q at offset %d, got %q", tag, off, got)
		}
		count := binary.LittleEndian.Uint32(body[off+4 : off+8])
		payloadStart := off + 4 // include the count word in the returned slice
		dataEnd := off + 8 + int(count)*elem[tag]
		if dataEnd > len(body) {
			t.Fatalf("sub-chunk %q overruns: dataEnd %d > len %d", tag, dataEnd, len(body))
		}
		out[tag] = body[payloadStart:dataEnd]
		off = dataEnd
	}
	// After MATS: materialId, selectionGroup, selectionFlags (12 bytes),
	// extent (28), sequenceExtentCount (4) = 44 bytes, then UVAS.
	off += 12 + 28 + 4
	// UVAS: tag + uint32 setCount.
	if off+8 > len(body) || string(body[off:off+4]) != "UVAS" {
		t.Fatalf("expected UVAS at offset %d", off)
	}
	off += 8 // skip UVAS tag + setCount
	if off+8 > len(body) || string(body[off:off+4]) != "UVBS" {
		t.Fatalf("expected UVBS at offset %d", off)
	}
	off += 4 // point at UVBS count word
	out["UVBS"] = body[off:]
	return out
}

func safePrefix(b []byte, n int) string {
	if len(b) < n {
		n = len(b)
	}
	return string(b[:n])
}

// --- tests ------------------------------------------------------------------

func TestEncodeStatic_MagicAndChunkOrder(t *testing.T) {
	data, err := EncodeStatic(cube())
	if err != nil {
		t.Fatalf("EncodeStatic: %v\n%s", err, DumpMDL(cube()))
	}
	if got := safePrefix(data, 4); got != "MDLX" {
		t.Fatalf("stream must start with MDLX, got %q", got)
	}

	chunks := walkTopChunks(t, data)

	// The fixed chunk order for a minimal static model.
	wantOrder := []string{"VERS", "MODL", "SEQS", "MTLS", "TEXS", "GEOS", "BONE", "PIVT"}
	if len(chunks) != len(wantOrder) {
		var got []string
		for _, c := range chunks {
			got = append(got, c.tag)
		}
		t.Fatalf("chunk count = %d %v, want %d %v", len(chunks), got, len(wantOrder), wantOrder)
	}
	for i, want := range wantOrder {
		if chunks[i].tag != want {
			t.Errorf("chunk[%d] = %q, want %q", i, chunks[i].tag, want)
		}
	}
}

func TestEncodeStatic_VERS800(t *testing.T) {
	data, err := EncodeStatic(cube())
	if err != nil {
		t.Fatal(err)
	}
	c, ok := findChunk(walkTopChunks(t, data), "VERS")
	if !ok {
		t.Fatal("no VERS chunk")
	}
	if len(c.body) != 4 {
		t.Fatalf("VERS body = %d bytes, want 4", len(c.body))
	}
	if v := binary.LittleEndian.Uint32(c.body); v != 800 {
		t.Errorf("VERS = %d, want 800", v)
	}
}

func TestEncodeStatic_MODLExtentNonZero(t *testing.T) {
	data, err := EncodeStatic(cube())
	if err != nil {
		t.Fatal(err)
	}
	c, ok := findChunk(walkTopChunks(t, data), "MODL")
	if !ok {
		t.Fatal("no MODL chunk")
	}
	if len(c.body) != 372 {
		t.Fatalf("MODL body = %d bytes, want 372", len(c.body))
	}
	// Extent begins after name(80) + animFile(260) = offset 340. radius+min+max.
	ext := readExtent(c.body[340:368])
	if ext.radius == 0 && ext.min == [3]float32{} && ext.max == [3]float32{} {
		t.Errorf("MODL extent is all-zero: %+v", ext)
	}
}

func TestEncodeStatic_SEQSCountAndExtent(t *testing.T) {
	data, err := EncodeStatic(cube())
	if err != nil {
		t.Fatal(err)
	}
	chunks := walkTopChunks(t, data)
	seqs, ok := findChunk(chunks, "SEQS")
	if !ok {
		t.Fatal("no SEQS chunk")
	}
	if len(seqs.body)%132 != 0 {
		t.Fatalf("SEQS body %d not a multiple of 132", len(seqs.body))
	}
	if n := len(seqs.body) / 132; n != 1 {
		t.Fatalf("SEQS count = %d, want 1", n)
	}
	// Sequence extent begins at offset 104 within the 132-byte record:
	// name(80) + interval(8) + moveSpeed(4) + flags(4) + rarity(4) + sync(4) = 104.
	seqExt := readExtent(seqs.body[104:132])

	modl, _ := findChunk(chunks, "MODL")
	modlExt := readExtent(modl.body[340:368])

	if seqExt != modlExt {
		t.Errorf("SEQS extent %+v != MODL extent %+v", seqExt, modlExt)
	}
	if seqExt.radius == 0 && seqExt.min == [3]float32{} && seqExt.max == [3]float32{} {
		t.Errorf("SEQS extent is all-zero")
	}
}

func TestEncodeStatic_OneBoneOnePivot(t *testing.T) {
	data, err := EncodeStatic(cube())
	if err != nil {
		t.Fatal(err)
	}
	chunks := walkTopChunks(t, data)

	bone, ok := findChunk(chunks, "BONE")
	if !ok {
		t.Fatal("no BONE chunk")
	}
	// One bone = 104 bytes on disk: a 96-byte GenericObject inclusive block
	// (4 inclusiveSize + 80 name + 4 objId + 4 parent + 4 flags) followed by the
	// 8-byte bone tail (4 geosetId + 4 geoAnimId) that sits OUTSIDE the generic
	// inclusiveSize. The inclusiveSize field must read 96, not 104 — folding the
	// tail in makes readers parse it as a bogus animation track and crash.
	if len(bone.body) != 104 {
		t.Fatalf("BONE body = %d bytes, want 104 (exactly one bone)", len(bone.body))
	}
	incl := binary.LittleEndian.Uint32(bone.body[:4])
	if incl != 96 {
		t.Errorf("bone generic inclusiveSize = %d, want 96 (tail excluded)", incl)
	}

	pivt, ok := findChunk(chunks, "PIVT")
	if !ok {
		t.Fatal("no PIVT chunk")
	}
	if len(pivt.body) != 12 {
		t.Fatalf("PIVT body = %d bytes, want 12 (exactly one pivot)", len(pivt.body))
	}
}

func TestEncodeStatic_GeosetInvariants(t *testing.T) {
	m := cube()
	data, err := EncodeStatic(m)
	if err != nil {
		t.Fatal(err)
	}
	geos, ok := findChunk(walkTopChunks(t, data), "GEOS")
	if !ok {
		t.Fatal("no GEOS chunk")
	}
	// The cube has a single geoset; the GEOS body begins with that geoset's
	// uint32 inclusiveSize, then the sub-chunks.
	geoBody := geos.body[4:] // skip geoset inclusiveSize
	sub := geosetSubChunks(t, geoBody)

	vcount := uint32(len(m.Positions))

	vrtxCount := binary.LittleEndian.Uint32(sub["VRTX"][:4])
	if vrtxCount != vcount {
		t.Errorf("VRTX count = %d, want %d", vrtxCount, vcount)
	}

	nrmsCount := binary.LittleEndian.Uint32(sub["NRMS"][:4])
	if nrmsCount != vcount {
		t.Errorf("NRMS count = %d, want VRTX count %d", nrmsCount, vcount)
	}

	uvbsCount := binary.LittleEndian.Uint32(sub["UVBS"][:4])
	if uvbsCount != vcount {
		t.Errorf("UVBS count = %d, want VRTX count %d", uvbsCount, vcount)
	}

	// GNDX: count == vertex count, every entry valid against MTGC length.
	gndx := sub["GNDX"]
	gndxCount := binary.LittleEndian.Uint32(gndx[:4])
	if gndxCount != vcount {
		t.Errorf("GNDX count = %d, want vertex count %d", gndxCount, vcount)
	}
	gndxData := gndx[4 : 4+gndxCount]
	mtgcCount := binary.LittleEndian.Uint32(sub["MTGC"][:4])
	var maxGndx uint8
	for _, g := range gndxData {
		if g > maxGndx {
			maxGndx = g
		}
	}
	if uint32(maxGndx) >= mtgcCount {
		t.Errorf("max(GNDX) = %d not < len(MTGC) = %d", maxGndx, mtgcCount)
	}
}

func TestEncodeStatic_RejectsOversizeSubmesh(t *testing.T) {
	m := &Model{
		Name:      "big",
		Positions: make([][3]float32, maxGeosetVerts+1),
		Normals:   make([][3]float32, maxGeosetVerts+1),
		UVs:       make([][2]float32, maxGeosetVerts+1),
		Submeshes: []Submesh{{Indices: []uint32{0, 1, 2}, TexturePath: "x.blp"}},
	}
	if _, err := EncodeStatic(m); err == nil {
		t.Fatal("expected error for >65535-vertex submesh, got nil")
	}
}

func TestEncodeStatic_SynthesizesUVsWhenAbsent(t *testing.T) {
	m := cube()
	m.UVs = nil // drop UVs
	data, err := EncodeStatic(m)
	if err != nil {
		t.Fatalf("EncodeStatic with no UVs: %v", err)
	}
	geos, _ := findChunk(walkTopChunks(t, data), "GEOS")
	sub := geosetSubChunks(t, geos.body[4:])
	uvbs := sub["UVBS"]
	if got := binary.LittleEndian.Uint32(uvbs[:4]); got != uint32(len(m.Positions)) {
		t.Errorf("synthesized UVBS count = %d, want %d", got, len(m.Positions))
	}
}

func TestEncodeStatic_NilAndEmpty(t *testing.T) {
	if _, err := EncodeStatic(nil); err == nil {
		t.Error("EncodeStatic(nil) should error")
	}
	if _, err := EncodeStatic(&Model{}); err == nil {
		t.Error("EncodeStatic(empty) should error")
	}
}

// --- local readExtent helper ------------------------------------------------

type extentRead struct {
	radius   float32
	min, max [3]float32
}

func readExtent(b []byte) extentRead {
	f := func(off int) float32 {
		return math.Float32frombits(binary.LittleEndian.Uint32(b[off : off+4]))
	}
	return extentRead{
		radius: f(0),
		min:    [3]float32{f(4), f(8), f(12)},
		max:    [3]float32{f(16), f(20), f(24)},
	}
}
