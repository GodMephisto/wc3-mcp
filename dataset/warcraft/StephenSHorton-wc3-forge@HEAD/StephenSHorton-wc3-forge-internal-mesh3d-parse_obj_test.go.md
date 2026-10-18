---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/mesh3d/parse_obj_test.go"
ref: HEAD
---

package mesh3d

import "testing"

// TestParseOBJ_MalformedNormalIndexNoPanic guards the regression where gwob
// panics (index out of range) on a face referencing a normal index past the
// declared normals, because gwob bounds-checks vertex/texcoord indices but not
// normal indices. ParseOBJ must recover and return an error, never crash.
func TestParseOBJ_MalformedNormalIndexNoPanic(t *testing.T) {
	// One declared normal (vn), but faces reference normal index 5.
	bad := []byte("v 0 0 0\nv 1 0 0\nv 0 1 0\nvt 0 0\nvn 0 0 1\n" +
		"f 1/1/5 2/1/5 3/1/5\n")
	m, err := ParseOBJ(bad, "")
	if err == nil {
		t.Fatalf("expected an error for malformed normal index, got mesh=%v", m)
	}
	if m != nil {
		t.Fatalf("expected nil mesh on error, got %v", m)
	}
}

// TestParseOBJ_ValidCubeNoNormals parses a cube with UVs but no normals (the
// common exporter case) and confirms Normalize recomputes per-vertex normals.
func TestParseOBJ_ValidCubeNoNormals(t *testing.T) {
	obj := []byte(`v -1 -1 -1
v -1 -1 1
v -1 1 -1
v -1 1 1
v 1 -1 -1
v 1 -1 1
v 1 1 -1
v 1 1 1
vt 0 0
vt 1 0
vt 1 1
vt 0 1
f 1/1 2/2 4/3
f 1/1 4/3 3/4
f 5/1 8/3 6/2
f 5/1 7/4 8/3
f 1/1 6/2 2/3
f 1/1 5/4 6/2
f 3/1 4/2 8/3
f 3/1 8/3 7/4
f 2/1 6/2 8/3
f 2/1 8/3 4/4
f 1/1 3/2 7/3
f 1/1 7/3 5/4
`)
	m, err := ParseOBJ(obj, "")
	if err != nil {
		t.Fatalf("parse: %v", err)
	}
	if _, err := Normalize(m, Options{UpAxis: "y", Scale: 1, FlipV: true}); err != nil {
		t.Fatalf("normalize: %v", err)
	}
	if len(m.Positions) == 0 {
		t.Fatalf("no positions after normalize")
	}
	if len(m.Normals) != len(m.Positions) {
		t.Fatalf("normals not recomputed to match positions: %d vs %d", len(m.Normals), len(m.Positions))
	}
}
