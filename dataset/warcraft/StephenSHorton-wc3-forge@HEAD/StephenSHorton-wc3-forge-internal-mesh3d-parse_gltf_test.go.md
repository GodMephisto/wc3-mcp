---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/mesh3d/parse_gltf_test.go"
ref: HEAD
---

package mesh3d

import (
	"bytes"
	"image"
	"image/color"
	"image/png"
	"testing"

	"github.com/qmuntal/gltf"
	"github.com/qmuntal/gltf/modeler"
)

// buildGLB constructs a tiny in-memory binary GLB: one mesh, one triangle
// primitive with positions, normals, UVs and indices, a material whose
// baseColor texture points at an embedded PNG. Returns the GLB bytes.
func buildGLB(t *testing.T) []byte {
	t.Helper()
	doc := gltf.NewDocument()

	positions := [][3]float32{
		{0, 0, 0}, {1, 0, 0}, {0, 1, 0},
	}
	normals := [][3]float32{
		{0, 0, 1}, {0, 0, 1}, {0, 0, 1},
	}
	uvs := [][2]float32{
		{0, 0}, {1, 0}, {0, 1},
	}
	indices := []uint16{0, 1, 2}

	// Embedded PNG (2x2) written into the GLB buffer as a bufferView image.
	img := image.NewRGBA(image.Rect(0, 0, 2, 2))
	img.Set(0, 0, color.RGBA{255, 0, 0, 255})
	img.Set(1, 1, color.RGBA{0, 255, 0, 255})
	var pngBuf bytes.Buffer
	if err := png.Encode(&pngBuf, img); err != nil {
		t.Fatalf("png encode: %v", err)
	}
	imgIdx, err := modeler.WriteImage(doc, "diffuse.png", "image/png", &pngBuf)
	if err != nil {
		t.Fatalf("WriteImage: %v", err)
	}
	doc.Textures = append(doc.Textures, &gltf.Texture{Source: gltf.Index(imgIdx)})
	texIdx := len(doc.Textures) - 1

	doc.Materials = append(doc.Materials, &gltf.Material{
		Name: "mat0",
		PBRMetallicRoughness: &gltf.PBRMetallicRoughness{
			BaseColorTexture: &gltf.TextureInfo{Index: texIdx},
		},
	})
	matIdx := len(doc.Materials) - 1

	attrs, err := modeler.WritePrimitiveAttributes(doc,
		modeler.PrimitiveAttribute{Name: gltf.POSITION, Data: positions},
		modeler.PrimitiveAttribute{Name: gltf.NORMAL, Data: normals},
		modeler.PrimitiveAttribute{Name: gltf.TEXCOORD_0, Data: uvs},
	)
	if err != nil {
		t.Fatalf("WritePrimitiveAttributes: %v", err)
	}
	idxAcc := modeler.WriteIndices(doc, indices)

	doc.Meshes = append(doc.Meshes, &gltf.Mesh{
		Primitives: []*gltf.Primitive{{
			Attributes: attrs,
			Indices:    gltf.Index(idxAcc),
			Material:   gltf.Index(matIdx),
		}},
	})

	var out bytes.Buffer
	enc := gltf.NewEncoder(&out) // AsBinary defaults true -> GLB
	if err := enc.Encode(doc); err != nil {
		t.Fatalf("encode GLB: %v", err)
	}
	return out.Bytes()
}

func TestParseGLB(t *testing.T) {
	glb := buildGLB(t)
	m, err := ParseGLTF(glb, "")
	if err != nil {
		t.Fatalf("ParseGLTF: %v", err)
	}
	if len(m.Positions) != 3 {
		t.Fatalf("positions = %d, want 3", len(m.Positions))
	}
	if !m.NormalsProvided || !m.UVsProvided {
		t.Fatalf("flags: norm=%v uv=%v, want true/true", m.NormalsProvided, m.UVsProvided)
	}
	if len(m.Submeshes) != 1 || len(m.Submeshes[0].Indices) != 3 {
		t.Fatalf("submesh/index shape wrong: %+v", m.Submeshes)
	}
	ref := m.Submeshes[0].TextureRef
	if ref == "" {
		t.Fatalf("no texture ref resolved")
	}
	if _, ok := m.Images[ref]; !ok {
		t.Fatalf("texture ref %q has no Images entry; keys=%v", ref, keysOf(m.Images))
	}
	// The embedded image bytes should be a valid PNG (round-trips through decode).
	if _, err := png.Decode(bytes.NewReader(m.Images[ref])); err != nil {
		t.Fatalf("embedded image is not a decodable PNG: %v", err)
	}

	if _, err := Normalize(m, Options{UpAxis: "y", FlipV: true, Scale: 2}); err != nil {
		t.Fatalf("Normalize: %v", err)
	}
	if len(m.Normals) != len(m.Positions) || len(m.UVs) != len(m.Positions) {
		t.Fatalf("post-normalize arrays misaligned")
	}
}

func keysOf(m map[string][]byte) []string {
	var ks []string
	for k := range m {
		ks = append(ks, k)
	}
	return ks
}
