---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/mesh3d/mesh3d.go"
ref: HEAD
---

// Package mesh3d is the shared mesh intermediate representation (IR) that every
// supported 3D-model import format (OBJ, glTF/GLB, STL) is parsed into, plus the
// single Normalize step that absorbs all WC3-specific correctness concerns
// before the mesh is handed to the MDX encoder.
//
// The design rationale (see .import-research/design.json) is "one shared IR + one
// MDX emitter": each per-format parser only fills the IR; everything that makes a
// mesh render correctly as a static WC3 MDX prop — the Y-up→Z-up axis remap with a
// determinant-+1 assertion, winding compensation, per-vertex normal recompute via
// inverse-transpose, V-flip, vertex welding, uniform scale, and the >65535-vertex
// geoset split — lives in normalize.go and is unit-tested once.
//
// Coordinate conventions absorbed here:
//   - OBJ / glTF are Y-up, right-handed. WC3 MDX is Z-up, right-handed.
//   - STL has no canonical axis/units and stores fully-duplicated triangle
//     vertices (no sharing), so it is welded and given synthesized UVs.
//
// All vertex arrays are parallel: Positions[i], Normals[i], UVs[i] all describe
// the same vertex i. Submeshes reference those vertices by index.
package mesh3d

import "math"

// Mesh is the shared intermediate representation populated by every parser and
// consumed by the MDX encoder. The three vertex arrays are parallel (one entry
// per vertex); a parser MUST keep them the same length, or leave Normals/UVs
// empty and set the corresponding *Provided flag to false so Normalize knows to
// synthesize them.
type Mesh struct {
	// Positions is the per-vertex position, one [x,y,z] per vertex.
	Positions [][3]float32
	// Normals is the per-vertex normal, parallel to Positions. May be nil/empty
	// when the source provided none (NormalsProvided == false), in which case
	// Normalize recomputes them from the triangulated geometry.
	Normals [][3]float32
	// UVs is the per-vertex texture coordinate, parallel to Positions. May be
	// nil/empty when the source provided none (UVsProvided == false), in which
	// case Normalize synthesizes a zero UV set so the MDX always has a valid
	// UVAS/UVBS chunk.
	UVs [][2]float32

	// Submeshes are the indexed triangle lists, one per source material/group
	// (OBJ usemtl group, glTF primitive, or the single STL triangle soup).
	Submeshes []Submesh

	// Images maps an in-archive texture base name (the value a Submesh.TextureRef
	// points at) to the raw encoded image bytes (PNG/JPG/TGA as found in the
	// source). The integration layer re-encodes these to BLP and writes them
	// under war3mapImported\. A TextureRef with no matching Images entry means
	// the texture could not be located and a placeholder should be substituted.
	Images map[string][]byte

	// NormalsProvided records whether the source supplied per-vertex normals.
	// When false, Normalize recomputes them from triangle geometry. When true
	// but a particular normal is zero-length, Normalize still recomputes that
	// vertex (handles partially-degenerate sources).
	NormalsProvided bool
	// UVsProvided records whether the source supplied texture coordinates. When
	// false, Normalize synthesizes a zero UV set and emits a warning.
	UVsProvided bool
	// NeedsWeld records whether the source stores duplicated, unshared vertices
	// (STL triangle soup) that must be welded before emission. OBJ/glTF are
	// already indexed and set this false.
	NeedsWeld bool
}

// Submesh is one indexed triangle list referencing the parent Mesh's vertex
// arrays. Indices is a flat list of triangle corner indices (len multiple of 3).
type Submesh struct {
	// Indices is the flat triangle index list into the parent Mesh vertex
	// arrays; len(Indices) is always a multiple of 3.
	Indices []uint32
	// MaterialName is the source material/group name (for diagnostics and to
	// distinguish geosets); may be empty.
	MaterialName string
	// TextureRef is the in-archive base name of the diffuse texture this submesh
	// uses, matching a key in Mesh.Images when the texture was resolved. Empty
	// means "no diffuse texture" (placeholder territory).
	TextureRef string
}

// ComputeAABB returns the axis-aligned bounding box (component-wise min and max)
// over every position in the mesh. For an empty mesh it returns two zero vectors.
// The MDX encoder needs this for the MODL/SEQS/GEOS extents — leaving extents
// zero gets the model frustum-culled in-engine.
func (m *Mesh) ComputeAABB() (min, max [3]float32) {
	if len(m.Positions) == 0 {
		return [3]float32{}, [3]float32{}
	}
	min = m.Positions[0]
	max = m.Positions[0]
	for _, p := range m.Positions {
		for i := 0; i < 3; i++ {
			if p[i] < min[i] {
				min[i] = p[i]
			}
			if p[i] > max[i] {
				max[i] = p[i]
			}
		}
	}
	return min, max
}

// BoundsRadius returns the radius of the bounding sphere centered at the AABB
// center: the maximum distance from that center to any vertex. This is the
// boundsRadius field MDX extents require.
func (m *Mesh) BoundsRadius() float32 {
	if len(m.Positions) == 0 {
		return 0
	}
	min, max := m.ComputeAABB()
	center := [3]float32{
		(min[0] + max[0]) * 0.5,
		(min[1] + max[1]) * 0.5,
		(min[2] + max[2]) * 0.5,
	}
	var maxSq float64
	for _, p := range m.Positions {
		dx := float64(p[0] - center[0])
		dy := float64(p[1] - center[1])
		dz := float64(p[2] - center[2])
		sq := dx*dx + dy*dy + dz*dz
		if sq > maxSq {
			maxSq = sq
		}
	}
	return float32(math.Sqrt(maxSq))
}

// VertexCount returns the number of vertices (len of the parallel arrays).
func (m *Mesh) VertexCount() int { return len(m.Positions) }

// TriangleCount returns the total number of triangles across all submeshes.
func (m *Mesh) TriangleCount() int {
	n := 0
	for i := range m.Submeshes {
		n += len(m.Submeshes[i].Indices) / 3
	}
	return n
}

// addImage records raw image bytes under name, lazily allocating the map. It is
// a no-op when name is empty or data is nil. Used by the parsers.
func (m *Mesh) addImage(name string, data []byte) {
	if name == "" || data == nil {
		return
	}
	if m.Images == nil {
		m.Images = make(map[string][]byte)
	}
	m.Images[name] = data
}
