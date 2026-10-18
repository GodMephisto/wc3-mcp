---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/mesh3d/parse_stl.go"
ref: HEAD
---

package mesh3d

import (
	"bytes"
	"fmt"

	"github.com/hschendel/stl"
)

// ParseSTL parses a binary or ASCII STL solid into the shared mesh IR. The
// hschendel/stl reader auto-detects the format from the stream, so both are
// handled here. STL is geometry-only: a flat triangle soup with a per-face
// normal and three fully-duplicated vertices, no shared/indexed vertices, no
// UVs, and no material/texture references.
//
// The expansion produces one vertex per triangle corner (3 per triangle) and a
// single submesh whose indices are 0,1,2,... in order. NeedsWeld is set so
// Normalize collapses the duplicated vertices, UVsProvided is false so a zero UV
// set is synthesized, and NormalsProvided is false so per-vertex normals are
// recomputed (the STL per-face normal is not propagated, since after welding the
// shared vertices want smoothed per-vertex normals anyway).
// Any panic from the STL reader on a malformed file is recovered and returned
// as an error so an untrusted import cannot crash the editor.
func ParseSTL(data []byte) (m *Mesh, err error) {
	defer func() {
		if r := recover(); r != nil {
			m, err = nil, fmt.Errorf("parse stl: malformed input: %v", r)
		}
	}()
	return parseSTLImpl(data)
}

func parseSTLImpl(data []byte) (*Mesh, error) {
	solid, err := stl.ReadAll(bytes.NewReader(data))
	if err != nil {
		return nil, fmt.Errorf("parse stl: %w", err)
	}
	if len(solid.Triangles) == 0 {
		return nil, fmt.Errorf("parse stl: solid contains no triangles")
	}

	n := len(solid.Triangles)
	mesh := &Mesh{
		Positions:       make([][3]float32, 0, n*3),
		NormalsProvided: false, // recompute post-weld
		UVsProvided:     false, // synthesize zero UVs
		NeedsWeld:       true,  // duplicated triangle-soup vertices
	}
	indices := make([]uint32, 0, n*3)

	for ti := range solid.Triangles {
		t := &solid.Triangles[ti]
		for v := 0; v < 3; v++ {
			vert := t.Vertices[v] // stl.Vec3 == [3]float32
			idx := uint32(len(mesh.Positions))
			mesh.Positions = append(mesh.Positions, [3]float32{vert[0], vert[1], vert[2]})
			indices = append(indices, idx)
		}
	}

	mesh.Submeshes = []Submesh{{Indices: indices}}
	return mesh, nil
}
