---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/mesh3d/parse_obj.go"
ref: HEAD
---

package mesh3d

import (
	"fmt"
	"os"
	"path/filepath"
	"strings"

	"github.com/udhos/gwob"
)

// ParseOBJ parses a Wavefront OBJ (with its referenced MTL, if resolvable) into
// the shared mesh IR. baseDir is the directory the .obj came from; it is used to
// locate the sibling .mtl and any diffuse image files (map_Kd) the materials
// reference. baseDir may be empty when the source is in-memory only, in which
// case material/texture resolution is skipped.
//
// gwob produces an interleaved Coord slice (positions, optional UVs, optional
// normals) with stride offsets, plus Groups carrying Usemtl + IndexBegin/Count.
// This de-interleaves Coord into the parallel IR arrays and maps each Group to a
// Submesh. The OBJ V axis (bottom-left origin) is left as-is here; Normalize
// applies the V-flip and the Y-up→Z-up remap.
func ParseOBJ(data []byte, baseDir string) (*Mesh, error) {
	// gwob bounds-checks vertex and texcoord indices but NOT normal indices
	// (obj.go addVertex), so a malformed OBJ — e.g. a face referencing a vn
	// index past the declared normals — panics deep inside the library rather
	// than returning an error. Untrusted user model files must never crash the
	// editor, so recover any parser panic and surface it as a normal error.
	o, err := parseOBJBuf(data)
	if err != nil {
		return nil, fmt.Errorf("parse obj: %w", err)
	}
	if o.StrideSize <= 0 {
		return nil, fmt.Errorf("parse obj: empty or malformed OBJ (no vertex data)")
	}

	mesh := &Mesh{
		NormalsProvided: o.NormCoordFound,
		UVsProvided:     o.TextCoordFound,
		NeedsWeld:       false, // OBJ is already indexed/shared
	}

	// De-interleave Coord into the parallel IR arrays. gwob's stride layout:
	//   [pos.x pos.y pos.z] [uv.u uv.v]? [norm.x norm.y norm.z]?
	// with byte offsets in StrideOffset*; divide by 4 to get float indices.
	floatsPerStride := o.StrideSize / 4
	if floatsPerStride <= 0 || len(o.Coord)%floatsPerStride != 0 {
		return nil, fmt.Errorf("parse obj: inconsistent stride (size=%d, coord=%d)", o.StrideSize, len(o.Coord))
	}
	vertCount := len(o.Coord) / floatsPerStride
	posOff := o.StrideOffsetPosition / 4
	texOff := o.StrideOffsetTexture / 4
	normOff := o.StrideOffsetNormal / 4

	mesh.Positions = make([][3]float32, vertCount)
	if o.NormCoordFound {
		mesh.Normals = make([][3]float32, vertCount)
	}
	if o.TextCoordFound {
		mesh.UVs = make([][2]float32, vertCount)
	}
	for v := 0; v < vertCount; v++ {
		base := v * floatsPerStride
		mesh.Positions[v] = [3]float32{
			o.Coord[base+posOff+0],
			o.Coord[base+posOff+1],
			o.Coord[base+posOff+2],
		}
		if o.TextCoordFound {
			mesh.UVs[v] = [2]float32{
				o.Coord[base+texOff+0],
				o.Coord[base+texOff+1],
			}
		}
		if o.NormCoordFound {
			mesh.Normals[v] = [3]float32{
				o.Coord[base+normOff+0],
				o.Coord[base+normOff+1],
				o.Coord[base+normOff+2],
			}
		}
	}

	// Resolve the material library once so multiple groups sharing a material
	// don't each re-read it. Missing/unreadable MTL is non-fatal: groups simply
	// carry no texture and the integration layer substitutes a placeholder.
	var lib gwob.MaterialLib
	haveLib := false
	if baseDir != "" && o.Mtllib != "" {
		mtlPath := filepath.Join(baseDir, filepath.FromSlash(o.Mtllib))
		if mtlBytes, e := os.ReadFile(mtlPath); e == nil {
			if parsed, e2 := gwob.ReadMaterialLibFromBuf(mtlBytes, &gwob.ObjParserOptions{}); e2 == nil {
				lib = parsed
				haveLib = true
			}
		}
	}

	// Map each gwob Group to a Submesh. gwob's Indices are global; a group's
	// triangles are Indices[IndexBegin : IndexBegin+IndexCount].
	for _, g := range o.Groups {
		if g.IndexCount < 3 {
			continue // skip degenerate/empty groups
		}
		end := g.IndexBegin + g.IndexCount
		if end > len(o.Indices) {
			return nil, fmt.Errorf("parse obj: group %q index range [%d:%d] exceeds %d indices", g.Name, g.IndexBegin, end, len(o.Indices))
		}
		sm := Submesh{
			Indices:      make([]uint32, 0, g.IndexCount),
			MaterialName: g.Usemtl,
		}
		for _, idx := range o.Indices[g.IndexBegin:end] {
			sm.Indices = append(sm.Indices, uint32(idx))
		}

		// Resolve the diffuse texture (map_Kd) for this group's material and
		// load the sibling image file into Images.
		if haveLib && g.Usemtl != "" {
			if mat, ok := lib.Lib[g.Usemtl]; ok && mat.MapKd != "" {
				ref := textureBaseName(mat.MapKd)
				sm.TextureRef = ref
				if baseDir != "" {
					imgPath := filepath.Join(baseDir, filepath.FromSlash(mat.MapKd))
					if imgBytes, e := os.ReadFile(imgPath); e == nil {
						mesh.addImage(ref, imgBytes)
					}
				}
			}
		}
		mesh.Submeshes = append(mesh.Submeshes, sm)
	}

	if len(mesh.Submeshes) == 0 {
		return nil, fmt.Errorf("parse obj: no usable faces found")
	}
	return mesh, nil
}

// parseOBJBuf wraps gwob.NewObjFromBuf with a panic recovery. gwob can panic
// (index out of range) on malformed face data because it skips the bounds check
// on normal indices that it applies to vertex/texcoord indices. Converting the
// panic to an error keeps a bad import from crashing the whole process.
func parseOBJBuf(data []byte) (o *gwob.Obj, err error) {
	defer func() {
		if r := recover(); r != nil {
			o = nil
			err = fmt.Errorf("malformed OBJ: %v", r)
		}
	}()
	return gwob.NewObjFromBuf("model.obj", data, &gwob.ObjParserOptions{})
}

// textureBaseName reduces a texture reference (possibly a relative path with
// either separator) to its bare file name, used as the Images key / TextureRef.
func textureBaseName(p string) string {
	p = strings.ReplaceAll(p, "\\", "/")
	if i := strings.LastIndexByte(p, '/'); i >= 0 {
		p = p[i+1:]
	}
	return p
}
