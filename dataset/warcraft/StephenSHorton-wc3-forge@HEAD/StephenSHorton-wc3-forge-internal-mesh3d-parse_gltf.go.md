---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/mesh3d/parse_gltf.go"
ref: HEAD
---

package mesh3d

import (
	"bytes"
	"fmt"
	"os"

	"github.com/qmuntal/gltf"
	"github.com/qmuntal/gltf/modeler"
)

// ParseGLTF parses a glTF 2.0 (.gltf) or binary GLB document into the shared
// mesh IR. The qmuntal/gltf decoder auto-detects ASCII vs GLB from the stream,
// so the same entry point handles both. baseDir is the directory the file came
// from; it is passed to the decoder as an fs.FS so external .bin buffers and
// external image files referenced by relative URI resolve. baseDir may be empty
// (embedded-only GLB), in which case only embedded/dataURI resources resolve.
//
// One Submesh is emitted per primitive. Each primitive's vertices are appended
// to the shared vertex arrays with an index offset, so the IR stays a single
// flat parallel-array mesh. The baseColor texture of each primitive's material
// is resolved (dataURI, GLB-embedded bufferView, or external file) into Images.
//
// Any panic from the decoder/modeler on a malformed file is recovered and
// returned as an error — an untrusted import must never crash the editor.
func ParseGLTF(data []byte, baseDir string) (m *Mesh, err error) {
	defer func() {
		if r := recover(); r != nil {
			m, err = nil, fmt.Errorf("parse gltf: malformed input: %v", r)
		}
	}()
	return parseGLTFImpl(data, baseDir)
}

func parseGLTFImpl(data []byte, baseDir string) (*Mesh, error) {
	var dec *gltf.Decoder
	if baseDir != "" {
		dec = gltf.NewDecoderFS(bytes.NewReader(data), os.DirFS(baseDir))
	} else {
		dec = gltf.NewDecoder(bytes.NewReader(data))
	}
	doc := new(gltf.Document)
	if err := dec.Decode(doc); err != nil {
		return nil, fmt.Errorf("parse gltf: %w", err)
	}

	mesh := &Mesh{
		// glTF requires POSITION; NORMAL and TEXCOORD_0 are optional. Start
		// optimistic and clear the flags if any primitive is missing them.
		NormalsProvided: true,
		UVsProvided:     true,
		NeedsWeld:       false, // glTF is indexed
	}

	// imageCache dedups image decoding across primitives that share a texture.
	imageCache := make(map[int]string) // gltf image index -> Images key

	for mi, gm := range doc.Meshes {
		for pi, prim := range gm.Primitives {
			// Only triangle-mode primitives are supported. PrimitiveTriangles
			// is the default (Mode == 0), so treat the zero value as triangles.
			if prim.Mode != gltf.PrimitiveTriangles && prim.Mode != 0 {
				return nil, fmt.Errorf("parse gltf: mesh %d primitive %d uses unsupported mode %v (only triangles)", mi, pi, prim.Mode)
			}

			posIdx, ok := prim.Attributes[gltf.POSITION]
			if !ok {
				return nil, fmt.Errorf("parse gltf: mesh %d primitive %d has no POSITION attribute", mi, pi)
			}
			positions, err := modeler.ReadPosition(doc, doc.Accessors[posIdx], nil)
			if err != nil {
				return nil, fmt.Errorf("parse gltf: read positions (mesh %d prim %d): %w", mi, pi, err)
			}
			vertN := len(positions)

			var normals [][3]float32
			if nIdx, ok := prim.Attributes[gltf.NORMAL]; ok {
				normals, err = modeler.ReadNormal(doc, doc.Accessors[nIdx], nil)
				if err != nil {
					return nil, fmt.Errorf("parse gltf: read normals (mesh %d prim %d): %w", mi, pi, err)
				}
			} else {
				mesh.NormalsProvided = false
			}

			var uvs [][2]float32
			if tIdx, ok := prim.Attributes[gltf.TEXCOORD_0]; ok {
				uvs, err = modeler.ReadTextureCoord(doc, doc.Accessors[tIdx], nil)
				if err != nil {
					return nil, fmt.Errorf("parse gltf: read texcoords (mesh %d prim %d): %w", mi, pi, err)
				}
			} else {
				mesh.UVsProvided = false
			}

			// Indices. If the primitive is non-indexed, synthesize a sequential
			// index list (0,1,2,...) over its vertices.
			var localIdx []uint32
			if prim.Indices != nil {
				localIdx, err = modeler.ReadIndices(doc, doc.Accessors[*prim.Indices], nil)
				if err != nil {
					return nil, fmt.Errorf("parse gltf: read indices (mesh %d prim %d): %w", mi, pi, err)
				}
			} else {
				localIdx = make([]uint32, vertN)
				for i := range localIdx {
					localIdx[i] = uint32(i)
				}
			}
			if len(localIdx)%3 != 0 {
				return nil, fmt.Errorf("parse gltf: mesh %d primitive %d index count %d not a multiple of 3", mi, pi, len(localIdx))
			}

			// Append this primitive's vertices to the shared arrays, offsetting
			// indices. Pad normals/UVs to keep the parallel arrays aligned even
			// when a later primitive lacks an attribute an earlier one had.
			base := uint32(len(mesh.Positions))
			mesh.Positions = append(mesh.Positions, positions...)
			for i := 0; i < vertN; i++ {
				if i < len(normals) {
					mesh.Normals = append(mesh.Normals, normals[i])
				} else {
					mesh.Normals = append(mesh.Normals, [3]float32{})
				}
				if i < len(uvs) {
					mesh.UVs = append(mesh.UVs, uvs[i])
				} else {
					mesh.UVs = append(mesh.UVs, [2]float32{})
				}
			}

			sm := Submesh{Indices: make([]uint32, len(localIdx))}
			for i, idx := range localIdx {
				sm.Indices[i] = base + idx
			}

			// Resolve the baseColor texture for this primitive's material.
			if prim.Material != nil && *prim.Material < len(doc.Materials) {
				mat := doc.Materials[*prim.Material]
				sm.MaterialName = mat.Name
				if ref := resolveBaseColorTexture(doc, mat, imageCache, mesh); ref != "" {
					sm.TextureRef = ref
				}
			}

			mesh.Submeshes = append(mesh.Submeshes, sm)
		}
	}

	if len(mesh.Positions) == 0 || len(mesh.Submeshes) == 0 {
		return nil, fmt.Errorf("parse gltf: document contains no triangle geometry")
	}
	// If some primitives lacked normals/UVs we padded them with zeros; the
	// flags above already record that they must be synthesized in Normalize.
	if !mesh.NormalsProvided {
		mesh.Normals = nil // force full recompute
	}
	return mesh, nil
}

// resolveBaseColorTexture finds the diffuse (baseColor) image for a material,
// decodes its bytes (dataURI / GLB bufferView / external file already loaded by
// the decoder's fs.FS), records them in mesh.Images, and returns the Images key
// (the TextureRef). Returns "" when there is no resolvable baseColor texture.
func resolveBaseColorTexture(doc *gltf.Document, mat *gltf.Material, cache map[int]string, mesh *Mesh) string {
	if mat.PBRMetallicRoughness == nil || mat.PBRMetallicRoughness.BaseColorTexture == nil {
		return ""
	}
	texIdx := mat.PBRMetallicRoughness.BaseColorTexture.Index
	if texIdx < 0 || texIdx >= len(doc.Textures) {
		return ""
	}
	tex := doc.Textures[texIdx]
	if tex.Source == nil || *tex.Source < 0 || *tex.Source >= len(doc.Images) {
		return ""
	}
	imgIdx := *tex.Source
	if ref, ok := cache[imgIdx]; ok {
		return ref
	}
	img := doc.Images[imgIdx]

	var raw []byte
	switch {
	case img.BufferView != nil:
		// GLB-embedded (or buffer-backed) image: read the bytes out of the
		// referenced buffer view.
		if *img.BufferView >= 0 && *img.BufferView < len(doc.BufferViews) {
			if b, err := modeler.ReadBufferView(doc, doc.BufferViews[*img.BufferView]); err == nil {
				raw = append([]byte(nil), b...)
			}
		}
	case img.IsEmbeddedResource():
		// data:image/...;base64 URI.
		if b, err := img.MarshalData(); err == nil {
			raw = b
		}
	}
	if raw == nil {
		// External file — the decoder's fs.FS does not auto-load images, so the
		// integration layer must resolve by URI relative to baseDir. We still
		// register the TextureRef (derived from URI/name) so a placeholder can
		// be substituted and a warning surfaced.
		ref := gltfImageKey(img, imgIdx)
		cache[imgIdx] = ref
		return ref
	}

	ref := gltfImageKey(img, imgIdx)
	mesh.addImage(ref, raw)
	cache[imgIdx] = ref
	return ref
}

// gltfImageKey derives a stable Images key for a glTF image: prefer its URI base
// name, then its Name, falling back to a synthetic name by index.
func gltfImageKey(img *gltf.Image, idx int) string {
	if img.URI != "" && !img.IsEmbeddedResource() {
		return textureBaseName(img.URI)
	}
	if img.Name != "" {
		return textureBaseName(img.Name)
	}
	return fmt.Sprintf("gltf_image_%d", idx)
}
