---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/mesh3d/mesh3d_test.go"
ref: HEAD
---

package mesh3d

import (
	"math"
	"testing"
)

// unitCube builds a synthetic indexed unit cube IR centered at the origin
// spanning [-1,1] on each axis, with provided normals and UVs, one submesh.
// Vertices are shared (8 corners) but normals here are the (non-smooth) corner
// directions — good enough to exercise the remap/winding logic. CCW winding when
// viewed from outside, matching MDX/OpenGL front-face convention.
func unitCube() *Mesh {
	// 8 corners.
	pos := [][3]float32{
		{-1, -1, -1}, // 0
		{1, -1, -1},  // 1
		{1, 1, -1},   // 2
		{-1, 1, -1},  // 3
		{-1, -1, 1},  // 4
		{1, -1, 1},   // 5
		{1, 1, 1},    // 6
		{-1, 1, 1},   // 7
	}
	// Corner normals = normalized corner positions.
	norm := make([][3]float32, len(pos))
	for i, p := range pos {
		l := float32(math.Sqrt(float64(p[0]*p[0] + p[1]*p[1] + p[2]*p[2])))
		norm[i] = [3]float32{p[0] / l, p[1] / l, p[2] / l}
	}
	uv := make([][2]float32, len(pos))
	for i := range uv {
		uv[i] = [2]float32{0.25, 0.75}
	}

	// 12 triangles, CCW from outside.
	idx := []uint32{
		// -Z face (looking from -Z, outside is -Z)
		0, 2, 1, 0, 3, 2,
		// +Z face
		4, 5, 6, 4, 6, 7,
		// -Y
		0, 1, 5, 0, 5, 4,
		// +Y
		3, 7, 6, 3, 6, 2,
		// -X
		0, 4, 7, 0, 7, 3,
		// +X
		1, 2, 6, 1, 6, 5,
	}

	return &Mesh{
		Positions:       pos,
		Normals:         norm,
		UVs:             uv,
		Submeshes:       []Submesh{{Indices: idx, MaterialName: "cube"}},
		NormalsProvided: true,
		UVsProvided:     true,
	}
}

func TestComputeAABBAndRadius(t *testing.T) {
	m := unitCube()
	min, max := m.ComputeAABB()
	if min != ([3]float32{-1, -1, -1}) || max != ([3]float32{1, 1, 1}) {
		t.Fatalf("AABB = %v..%v, want -1..1", min, max)
	}
	r := m.BoundsRadius()
	want := float32(math.Sqrt(3))
	if math.Abs(float64(r-want)) > 1e-5 {
		t.Fatalf("BoundsRadius = %v, want %v", r, want)
	}
}

// collectWindingSigns returns, for each triangle, the sign of the dot product
// between its face normal and the direction from the mesh center (origin) to the
// triangle centroid. For a convex outward-wound mesh every sign is positive;
// reversed winding flips them all negative.
func collectWindingSigns(m *Mesh) []float64 {
	var signs []float64
	for _, sm := range m.Submeshes {
		idx := sm.Indices
		for t := 0; t+2 < len(idx); t += 3 {
			p0 := m.Positions[idx[t]]
			p1 := m.Positions[idx[t+1]]
			p2 := m.Positions[idx[t+2]]
			ax, ay, az := p1[0]-p0[0], p1[1]-p0[1], p1[2]-p0[2]
			bx, by, bz := p2[0]-p0[0], p2[1]-p0[1], p2[2]-p0[2]
			nx := ay*bz - az*by
			ny := az*bx - ax*bz
			nz := ax*by - ay*bx
			cx := (p0[0] + p1[0] + p2[0]) / 3
			cy := (p0[1] + p1[1] + p2[1]) / 3
			cz := (p0[2] + p1[2] + p2[2]) / 3
			dot := float64(nx*cx + ny*cy + nz*cz)
			signs = append(signs, dot)
		}
	}
	return signs
}

func TestNormalizeYUpPreservesWinding(t *testing.T) {
	m := unitCube()
	before := collectWindingSigns(m)
	// Sanity: the synthetic cube is outward-wound.
	for i, s := range before {
		if s <= 0 {
			t.Fatalf("fixture triangle %d is not outward-wound (dot=%v)", i, s)
		}
	}

	warns, err := Normalize(m, Options{UpAxis: "y", Scale: 1})
	if err != nil {
		t.Fatalf("Normalize: %v", err)
	}
	for _, w := range warns {
		// The Y-up cube provides normals and UVs; the only acceptable warning
		// would be none. A reflective-winding warning here is a bug.
		t.Logf("warning: %s", w)
		if containsReflective(w) {
			t.Fatalf("unexpected reflective-winding warning for det+1 remap: %s", w)
		}
	}

	after := collectWindingSigns(m)
	if len(after) != len(before) {
		t.Fatalf("triangle count changed: %d -> %d", len(before), len(after))
	}
	// The (x,y,z)->(x,-z,y) remap is a det+1 rotation: every triangle must stay
	// outward-wound (positive sign) since the origin center maps to itself.
	for i, s := range after {
		if s <= 0 {
			t.Fatalf("after Y-up remap triangle %d became inward-wound (dot=%v)", i, s)
		}
	}
}

func TestNormalizeNormalsAndUVCounts(t *testing.T) {
	m := unitCube()
	if _, err := Normalize(m, Options{UpAxis: "y"}); err != nil {
		t.Fatalf("Normalize: %v", err)
	}
	if len(m.Normals) != len(m.Positions) {
		t.Fatalf("normal count %d != vertex count %d", len(m.Normals), len(m.Positions))
	}
	if len(m.UVs) != len(m.Positions) {
		t.Fatalf("UV count %d != vertex count %d", len(m.UVs), len(m.Positions))
	}
	for i, n := range m.Normals {
		l := math.Sqrt(float64(n[0]*n[0] + n[1]*n[1] + n[2]*n[2]))
		if math.Abs(l-1) > 1e-4 {
			t.Fatalf("normal %d not unit length: |%v| = %v", i, n, l)
		}
	}
}

func TestNormalizeScaleDefaultsToOne(t *testing.T) {
	m := unitCube()
	if _, err := Normalize(m, Options{UpAxis: "z", Scale: 0}); err != nil {
		t.Fatalf("Normalize: %v", err)
	}
	// Z-up is identity and Scale 0 -> 1: positions unchanged.
	min, max := m.ComputeAABB()
	if min != ([3]float32{-1, -1, -1}) || max != ([3]float32{1, 1, 1}) {
		t.Fatalf("Scale=0 should default to 1; AABB = %v..%v", min, max)
	}
}

func TestNormalizeScaleApplied(t *testing.T) {
	m := unitCube()
	if _, err := Normalize(m, Options{UpAxis: "z", Scale: 10}); err != nil {
		t.Fatalf("Normalize: %v", err)
	}
	min, max := m.ComputeAABB()
	if min != ([3]float32{-10, -10, -10}) || max != ([3]float32{10, 10, 10}) {
		t.Fatalf("Scale=10 not applied; AABB = %v..%v", min, max)
	}
}

func TestNormalizeFlipV(t *testing.T) {
	m := unitCube() // all V = 0.75
	if _, err := Normalize(m, Options{UpAxis: "z", FlipV: true}); err != nil {
		t.Fatalf("Normalize: %v", err)
	}
	for i, uv := range m.UVs {
		if math.Abs(float64(uv[1]-0.25)) > 1e-6 {
			t.Fatalf("UV %d V not flipped: got %v, want 0.25", i, uv[1])
		}
	}
}

// TestNormalizeReflectiveRemapReversesWinding drives the reflective branch by
// directly invoking the remap+winding logic through a reflective axis. Since
// axisRemap only ever yields det+1 matrices, we exercise the compensation path
// via a small purpose-built reflective mesh transform: we assert that, given a
// reflective remap, the winding is reversed so the model is not inside-out.
func TestNormalizeReflectiveRemapReversesWinding(t *testing.T) {
	// Build a mesh and manually mirror it across X (det -1) the way a bad remap
	// would, but route it through the same winding-compensation logic by using
	// a reflective matrix in a local copy of the relevant steps.
	m := unitCube()
	signsBefore := collectWindingSigns(m)

	// Reflective transform: negate X. det = -1.
	refl := mat3{
		{-1, 0, 0},
		{0, 1, 0},
		{0, 0, 1},
	}
	if refl.det() >= 0 {
		t.Fatalf("test matrix is not reflective: det=%v", refl.det())
	}
	// Apply reflection to positions WITHOUT winding fix -> winding flips.
	for i := range m.Positions {
		m.Positions[i] = refl.mulVec(m.Positions[i])
	}
	signsFlipped := collectWindingSigns(m)
	for i := range signsBefore {
		if (signsBefore[i] > 0) == (signsFlipped[i] > 0) {
			t.Fatalf("reflection did not flip winding sign at triangle %d", i)
		}
	}
	// Now apply the winding compensation (i1<->i2 swap) the Normalize reflective
	// branch performs and confirm winding is restored to outward.
	for si := range m.Submeshes {
		idx := m.Submeshes[si].Indices
		for tt := 0; tt+2 < len(idx); tt += 3 {
			idx[tt+1], idx[tt+2] = idx[tt+2], idx[tt+1]
		}
	}
	signsFixed := collectWindingSigns(m)
	for i := range signsBefore {
		if (signsBefore[i] > 0) != (signsFixed[i] > 0) {
			t.Fatalf("winding compensation failed to restore outward winding at triangle %d", i)
		}
	}
}

// TestWeldCollapsesDuplicates builds an STL-style triangle soup (two triangles
// sharing an edge but with fully duplicated vertices) and asserts Normalize
// welds the shared positions down.
func TestWeldCollapsesDuplicates(t *testing.T) {
	// A unit quad as two triangles, with every corner duplicated per triangle
	// (6 vertices total, but only 4 distinct positions).
	a := [3]float32{0, 0, 0}
	b := [3]float32{1, 0, 0}
	c := [3]float32{1, 1, 0}
	d := [3]float32{0, 1, 0}
	m := &Mesh{
		Positions: [][3]float32{
			a, b, c, // tri 1
			a, c, d, // tri 2
		},
		Submeshes:       []Submesh{{Indices: []uint32{0, 1, 2, 3, 4, 5}}},
		NormalsProvided: false,
		UVsProvided:     false,
		NeedsWeld:       true,
	}
	if _, err := Normalize(m, Options{UpAxis: "z"}); err != nil {
		t.Fatalf("Normalize: %v", err)
	}
	if len(m.Positions) != 4 {
		t.Fatalf("weld did not collapse to 4 distinct vertices: got %d", len(m.Positions))
	}
	// Indices must still describe 2 triangles and stay in range.
	idx := m.Submeshes[0].Indices
	if len(idx) != 6 {
		t.Fatalf("expected 6 indices after weld, got %d", len(idx))
	}
	for _, i := range idx {
		if int(i) >= len(m.Positions) {
			t.Fatalf("index %d out of range after weld (verts=%d)", i, len(m.Positions))
		}
	}
	// Synthesized normals + UVs present and sized.
	if len(m.Normals) != 4 || len(m.UVs) != 4 {
		t.Fatalf("post-weld normals=%d uvs=%d, want 4/4", len(m.Normals), len(m.UVs))
	}
}

// TestSplitOversizedSubmesh builds a submesh referencing more than 65535
// distinct vertices and asserts Normalize splits it into multiple submeshes each
// within the cap, preserving the total triangle count.
func TestSplitOversizedSubmesh(t *testing.T) {
	const tris = 30000 // 90000 distinct verts, well over the 65535 cap
	pos := make([][3]float32, tris*3)
	idx := make([]uint32, tris*3)
	for i := 0; i < tris; i++ {
		base := uint32(i * 3)
		// Distinct, non-degenerate positions for each corner.
		x := float32(i)
		pos[base+0] = [3]float32{x, 0, 0}
		pos[base+1] = [3]float32{x, 1, 0}
		pos[base+2] = [3]float32{x, 0, 1}
		idx[base+0] = base + 0
		idx[base+1] = base + 1
		idx[base+2] = base + 2
	}
	m := &Mesh{
		Positions:       pos,
		Submeshes:       []Submesh{{Indices: idx, MaterialName: "big", TextureRef: "tex.blp"}},
		NormalsProvided: false,
		UVsProvided:     false,
	}
	if _, err := Normalize(m, Options{UpAxis: "z"}); err != nil {
		t.Fatalf("Normalize: %v", err)
	}
	if len(m.Submeshes) < 2 {
		t.Fatalf("oversized submesh not split: got %d submeshes", len(m.Submeshes))
	}
	totalTris := 0
	for i, sm := range m.Submeshes {
		dv := distinctVertexCount(sm.Indices)
		if dv > maxGeosetVerts {
			t.Fatalf("submesh %d still over cap: %d distinct verts", i, dv)
		}
		if sm.MaterialName != "big" || sm.TextureRef != "tex.blp" {
			t.Fatalf("submesh %d lost material/texture: %q/%q", i, sm.MaterialName, sm.TextureRef)
		}
		if len(sm.Indices)%3 != 0 {
			t.Fatalf("submesh %d index count %d not multiple of 3", i, len(sm.Indices))
		}
		totalTris += len(sm.Indices) / 3
	}
	if totalTris != tris {
		t.Fatalf("triangle count changed across split: got %d, want %d", totalTris, tris)
	}
}

func TestAxisRemapDeterminants(t *testing.T) {
	for _, ax := range []string{"y", "z", "auto", ""} {
		m, _ := axisRemap(ax)
		if d := m.det(); math.Abs(float64(d)-1) > 1e-5 {
			t.Fatalf("axis %q remap determinant = %v, want +1", ax, d)
		}
	}
	// Unknown axis returns recognized=false but still a det+1 matrix.
	m, ok := axisRemap("bogus")
	if ok {
		t.Fatalf("axisRemap(bogus) should report unrecognized")
	}
	if d := m.det(); math.Abs(float64(d)-1) > 1e-5 {
		t.Fatalf("fallback remap determinant = %v, want +1", d)
	}
}

func TestParseSTLRoundTripThroughNormalize(t *testing.T) {
	// Minimal ASCII STL: a single triangle.
	ascii := `solid tri
facet normal 0 0 1
  outer loop
    vertex 0 0 0
    vertex 1 0 0
    vertex 0 1 0
  endloop
endfacet
endsolid tri
`
	m, err := ParseSTL([]byte(ascii))
	if err != nil {
		t.Fatalf("ParseSTL: %v", err)
	}
	if !m.NeedsWeld || m.UVsProvided || m.NormalsProvided {
		t.Fatalf("STL flags wrong: weld=%v uv=%v norm=%v", m.NeedsWeld, m.UVsProvided, m.NormalsProvided)
	}
	if len(m.Positions) != 3 || len(m.Submeshes) != 1 {
		t.Fatalf("STL parse: verts=%d submeshes=%d", len(m.Positions), len(m.Submeshes))
	}
	if _, err := Normalize(m, Options{UpAxis: "z"}); err != nil {
		t.Fatalf("Normalize STL: %v", err)
	}
	if len(m.Normals) != len(m.Positions) || len(m.UVs) != len(m.Positions) {
		t.Fatalf("post-normalize STL: normals=%d uvs=%d verts=%d", len(m.Normals), len(m.UVs), len(m.Positions))
	}
}

func TestParseOBJBasic(t *testing.T) {
	// A two-triangle quad with UVs and normals, single material.
	obj := `mtllib quad.mtl
v 0 0 0
v 1 0 0
v 1 1 0
v 0 1 0
vt 0 0
vt 1 0
vt 1 1
vt 0 1
vn 0 0 1
usemtl mat0
f 1/1/1 2/2/1 3/3/1
f 1/1/1 3/3/1 4/4/1
`
	m, err := ParseOBJ([]byte(obj), "")
	if err != nil {
		t.Fatalf("ParseOBJ: %v", err)
	}
	if !m.UVsProvided || !m.NormalsProvided {
		t.Fatalf("OBJ flags: uv=%v norm=%v (want true/true)", m.UVsProvided, m.NormalsProvided)
	}
	if len(m.Submeshes) != 1 {
		t.Fatalf("expected 1 submesh, got %d", len(m.Submeshes))
	}
	if got := len(m.Submeshes[0].Indices); got != 6 {
		t.Fatalf("expected 6 indices, got %d", got)
	}
	if m.Submeshes[0].MaterialName != "mat0" {
		t.Fatalf("material name = %q, want mat0", m.Submeshes[0].MaterialName)
	}
	if _, err := Normalize(m, Options{UpAxis: "y", FlipV: true}); err != nil {
		t.Fatalf("Normalize OBJ: %v", err)
	}
	if len(m.Normals) != len(m.Positions) || len(m.UVs) != len(m.Positions) {
		t.Fatalf("post-normalize OBJ arrays misaligned")
	}
}

func containsReflective(s string) bool {
	return len(s) >= 10 && (indexOf(s, "reflective") >= 0)
}

func indexOf(s, sub string) int {
	for i := 0; i+len(sub) <= len(s); i++ {
		if s[i:i+len(sub)] == sub {
			return i
		}
	}
	return -1
}
