---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/mdx/mdl_debug.go"
ref: HEAD
---

package mdx

import (
	"fmt"
	"strings"
)

// DumpMDL renders m as a minimal, human-readable MDL-style text. It is NOT a
// spec-faithful MDL serializer and is never shipped into a map — it exists
// purely for eyeballing structure in test failures.
func DumpMDL(m *Model) string {
	var b strings.Builder

	if m == nil {
		return "<nil model>\n"
	}

	uvCount := len(m.UVs)
	ext := computeExtent(m.Positions)

	fmt.Fprintf(&b, "// MDL debug dump (VERS 800)\n")
	fmt.Fprintf(&b, "Model %q {\n", m.Name)
	fmt.Fprintf(&b, "\tNumVertices %d,\n", len(m.Positions))
	fmt.Fprintf(&b, "\tNumNormals %d,\n", len(m.Normals))
	fmt.Fprintf(&b, "\tNumUVs %d,\n", uvCount)
	fmt.Fprintf(&b, "\tNumGeosets %d,\n", len(m.Submeshes))
	fmt.Fprintf(&b, "\tBoundsRadius %g,\n", ext.radius)
	fmt.Fprintf(&b, "\tMinimumExtent { %g, %g, %g },\n", ext.min[0], ext.min[1], ext.min[2])
	fmt.Fprintf(&b, "\tMaximumExtent { %g, %g, %g },\n", ext.max[0], ext.max[1], ext.max[2])
	fmt.Fprintf(&b, "}\n")

	fmt.Fprintf(&b, "Sequences 1 {\n")
	fmt.Fprintf(&b, "\tAnim %q { Interval { 0, 1 }, },\n", "Stand")
	fmt.Fprintf(&b, "}\n")

	for i, sm := range m.Submeshes {
		gExt := computeSubmeshExtent(m.Positions, sm.Indices)
		fmt.Fprintf(&b, "Geoset %d {\n", i)
		fmt.Fprintf(&b, "\tVertices %d,\n", len(m.Positions))
		fmt.Fprintf(&b, "\tFaces %d (%d tris),\n", len(sm.Indices), len(sm.Indices)/3)
		fmt.Fprintf(&b, "\tMaterialID %d,\n", i)
		fmt.Fprintf(&b, "\tTexture %q,\n", sm.TexturePath)
		fmt.Fprintf(&b, "\tBoundsRadius %g,\n", gExt.radius)
		fmt.Fprintf(&b, "}\n")
	}

	fmt.Fprintf(&b, "Materials %d {\n", len(m.Submeshes))
	for i, sm := range m.Submeshes {
		fmt.Fprintf(&b, "\tMaterial { Layer { FilterMode None, Unshaded, static TextureID %d, static Alpha 1, }, } // %q\n", i, sm.TexturePath)
	}
	fmt.Fprintf(&b, "}\n")

	fmt.Fprintf(&b, "Textures %d {\n", len(m.Submeshes))
	for _, sm := range m.Submeshes {
		fmt.Fprintf(&b, "\tBitmap { Image %q, },\n", sm.TexturePath)
	}
	fmt.Fprintf(&b, "}\n")

	fmt.Fprintf(&b, "Bone \"root\" { ObjectId 0, Parent -1, }\n")
	fmt.Fprintf(&b, "PivotPoints 1 { { 0, 0, 0 }, }\n")

	return b.String()
}
