---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/mesh3d/normalize.go"
ref: HEAD
---

package mesh3d

import (
	"fmt"
	"math"
)

// maxGeosetVerts is the hard cap on distinct vertices a single MDX geoset can
// reference: face indices in a VERS=800 geoset are uint16, so a geoset may index
// at most 65536 vertices (0..65535). Normalize splits any submesh that exceeds
// this into multiple submeshes before emission.
const maxGeosetVerts = 65535

// weldEpsilon is the squared distance below which two positions are considered
// the same vertex during welding. STL stores float32 vertices that are usually
// bit-identical across shared triangle corners, so an exact key works; the small
// epsilon-quantized key below tolerates trivial float noise without merging
// genuinely-distinct vertices.
const weldQuantum = 1.0 / 4096.0 // ~0.00024 game units; well below any real feature size

// Options controls the Normalize pass.
type Options struct {
	// UpAxis selects the source up-axis: "y" (OBJ/glTF default) remaps Y-up to
	// WC3's Z-up via (x,y,z)->(x,-z,y); "z" is already Z-up (identity); "auto"
	// is treated as "y". Unknown values are treated as "y" with a warning.
	UpAxis string
	// Scale is a uniform scale applied to all positions after the axis remap.
	// Zero is treated as 1 (no scale).
	Scale float32
	// FlipV flips texture V coordinates (v -> 1-v) to convert OBJ/glTF's
	// bottom-left UV origin to WC3's top-left expectation.
	FlipV bool
}

// mat3 is a 3x3 matrix stored row-major: m[row][col].
type mat3 [3][3]float32

// det returns the determinant of a 3x3 matrix.
func (m mat3) det() float32 {
	return m[0][0]*(m[1][1]*m[2][2]-m[1][2]*m[2][1]) -
		m[0][1]*(m[1][0]*m[2][2]-m[1][2]*m[2][0]) +
		m[0][2]*(m[1][0]*m[2][1]-m[1][1]*m[2][0])
}

// mulVec applies the matrix to a column vector.
func (m mat3) mulVec(v [3]float32) [3]float32 {
	return [3]float32{
		m[0][0]*v[0] + m[0][1]*v[1] + m[0][2]*v[2],
		m[1][0]*v[0] + m[1][1]*v[1] + m[1][2]*v[2],
		m[2][0]*v[0] + m[2][1]*v[1] + m[2][2]*v[2],
	}
}

// axisRemap returns the explicit 3x3 remap matrix for the given up-axis, plus a
// human-readable description. The matrix maps a source-space column vector into
// WC3 (Z-up, right-handed) space.
//
//   - "z" (or already-Z-up): identity, determinant +1.
//   - "y"/"auto"/anything else: (x,y,z) -> (x,-z,y), a +90° rotation about X.
//     Determinant +1 (a pure rotation), so winding is preserved.
func axisRemap(upAxis string) (mat3, bool) {
	switch upAxis {
	case "z", "Z":
		return mat3{
			{1, 0, 0},
			{0, 1, 0},
			{0, 0, 1},
		}, true
	case "y", "Y", "auto", "":
		// (x, y, z) -> (x, -z, y)
		return mat3{
			{1, 0, 0},
			{0, 0, -1},
			{0, 1, 0},
		}, true
	default:
		// Unknown axis: default to the Y-up remap, signal "unrecognized".
		return mat3{
			{1, 0, 0},
			{0, 0, -1},
			{0, 1, 0},
		}, false
	}
}

// Normalize applies every WC3-correctness transform to the mesh in place and
// returns any non-fatal warnings. The steps, in order:
//
//  1. Build the explicit axis-remap matrix from opts.UpAxis and assert its
//     determinant is +1 (a reflective remap, det -1, would flip handedness and
//     render the model inside-out). If the matrix is reflective, every
//     triangle's index order is reversed to compensate for the winding flip.
//  2. Apply the remap to positions, and the inverse-transpose of the remap to
//     normals (for a pure rotation that is the rotation itself).
//  3. Weld duplicated vertices when the source requires it (STL), rewriting all
//     submesh indices.
//  4. Recompute per-vertex normals from triangle geometry when the source
//     provided none or a normal is zero-length.
//  5. Synthesize a zero UV set when the source provided none, and flip V when
//     requested.
//  6. Apply the uniform scale (defaulting to 1).
//  7. Split any submesh referencing more than 65535 distinct vertices into
//     multiple submeshes (MDX geosets are uint16-indexed).
//
// The mesh is left with parallel Positions/Normals/UVs arrays (one entry each
// per vertex) and submeshes whose indices fit the geoset cap.
func Normalize(m *Mesh, opts Options) (warnings []string, err error) {
	if len(m.Positions) == 0 {
		return nil, fmt.Errorf("normalize: mesh has no vertices")
	}

	remap, recognized := axisRemap(opts.UpAxis)
	if !recognized {
		warnings = append(warnings, fmt.Sprintf("unrecognized up axis %q; defaulting to Y-up remap", opts.UpAxis))
	}

	// Step 1: determinant assertion + winding compensation.
	d := remap.det()
	reflective := d < 0
	if math.Abs(float64(d)-1) > 1e-4 && math.Abs(float64(d)+1) > 1e-4 {
		// Not ±1: a non-rigid remap we never construct. Defensive guard.
		return warnings, fmt.Errorf("normalize: axis remap has determinant %v (expected +1)", d)
	}
	if reflective {
		// Reverse triangle winding (i0,i1,i2 -> i0,i2,i1) so back-face culling
		// stays correct after the handedness flip.
		for si := range m.Submeshes {
			idx := m.Submeshes[si].Indices
			for t := 0; t+2 < len(idx); t += 3 {
				idx[t+1], idx[t+2] = idx[t+2], idx[t+1]
			}
		}
		warnings = append(warnings, "axis remap is reflective (determinant -1); reversed triangle winding to compensate")
	}

	// Step 2: apply remap to positions and (inverse-transpose) to normals.
	// For a rotation R, the normal transform is (R^-1)^T = R, so the same
	// matrix applies; we still recompute zero/absent normals in step 4.
	for i := range m.Positions {
		m.Positions[i] = remap.mulVec(m.Positions[i])
	}
	for i := range m.Normals {
		m.Normals[i] = remap.mulVec(m.Normals[i])
	}

	// Step 3: weld duplicated vertices (STL triangle soup) before normal
	// recompute, so shared vertices get a single smoothed normal.
	if m.NeedsWeld {
		weldVertices(m)
	}

	// Step 4: recompute per-vertex normals if absent, or fix individual zero
	// normals. Done after welding so the recompute averages adjacent faces.
	needRecompute := !m.NormalsProvided || len(m.Normals) != len(m.Positions)
	if !needRecompute {
		for _, n := range m.Normals {
			if isZeroVec(n) {
				needRecompute = true
				break
			}
		}
	}
	if needRecompute {
		recomputeNormals(m)
		if !m.NormalsProvided {
			warnings = append(warnings, "source provided no normals; recomputed per-vertex normals from geometry")
		}
	}

	// Step 5: UVs — synthesize zeros if absent, then optionally flip V.
	if !m.UVsProvided || len(m.UVs) != len(m.Positions) {
		m.UVs = make([][2]float32, len(m.Positions))
		warnings = append(warnings, "source provided no UVs; synthesized a zero UV set")
	}
	if opts.FlipV {
		for i := range m.UVs {
			m.UVs[i][1] = 1 - m.UVs[i][1]
		}
	}

	// Step 6: uniform scale (default 1).
	scale := opts.Scale
	if scale == 0 {
		scale = 1
	}
	if scale != 1 {
		for i := range m.Positions {
			m.Positions[i][0] *= scale
			m.Positions[i][1] *= scale
			m.Positions[i][2] *= scale
		}
	}

	// Step 7: split oversized submeshes so each geoset's distinct-vertex count
	// fits the uint16 index cap.
	splitOversizedSubmeshes(m)

	return warnings, nil
}

// weldVertices collapses positionally-identical vertices into one, rebuilding
// the parallel arrays and rewriting every submesh's indices. Welding is keyed on
// quantized position (and quantized normal/UV when present) so that genuinely
// distinct attributes at the same position are kept separate — but for STL,
// which has no normals/UVs at weld time, this merges purely by position.
func weldVertices(m *Mesh) {
	type key struct {
		px, py, pz int64
		nx, ny, nz int64
		u, v       int64
	}
	quant := func(f float32) int64 {
		return int64(math.Round(float64(f) / weldQuantum))
	}

	hasNormals := len(m.Normals) == len(m.Positions)
	hasUVs := len(m.UVs) == len(m.Positions)

	remapIdx := make([]uint32, len(m.Positions))
	seen := make(map[key]uint32, len(m.Positions))

	newPos := make([][3]float32, 0, len(m.Positions))
	var newNorm [][3]float32
	var newUV [][2]float32
	if hasNormals {
		newNorm = make([][3]float32, 0, len(m.Positions))
	}
	if hasUVs {
		newUV = make([][2]float32, 0, len(m.Positions))
	}

	for i := range m.Positions {
		p := m.Positions[i]
		var k key
		k.px, k.py, k.pz = quant(p[0]), quant(p[1]), quant(p[2])
		if hasNormals {
			n := m.Normals[i]
			k.nx, k.ny, k.nz = quant(n[0]), quant(n[1]), quant(n[2])
		}
		if hasUVs {
			uv := m.UVs[i]
			k.u, k.v = quant(uv[0]), quant(uv[1])
		}
		if existing, ok := seen[k]; ok {
			remapIdx[i] = existing
			continue
		}
		ni := uint32(len(newPos))
		seen[k] = ni
		remapIdx[i] = ni
		newPos = append(newPos, p)
		if hasNormals {
			newNorm = append(newNorm, m.Normals[i])
		}
		if hasUVs {
			newUV = append(newUV, m.UVs[i])
		}
	}

	m.Positions = newPos
	if hasNormals {
		m.Normals = newNorm
	}
	if hasUVs {
		m.UVs = newUV
	}
	for si := range m.Submeshes {
		idx := m.Submeshes[si].Indices
		for j := range idx {
			idx[j] = remapIdx[idx[j]]
		}
	}
}

// recomputeNormals computes smoothed per-vertex normals by accumulating each
// triangle's (un-normalized, area-weighted) face normal onto its three vertices,
// then normalizing. Degenerate triangles contribute nothing. A vertex touched by
// no valid triangle ends up with a unit +Z normal so the array stays valid.
func recomputeNormals(m *Mesh) {
	acc := make([][3]float64, len(m.Positions))
	for si := range m.Submeshes {
		idx := m.Submeshes[si].Indices
		for t := 0; t+2 < len(idx); t += 3 {
			i0, i1, i2 := idx[t], idx[t+1], idx[t+2]
			if int(i0) >= len(m.Positions) || int(i1) >= len(m.Positions) || int(i2) >= len(m.Positions) {
				continue
			}
			p0 := m.Positions[i0]
			p1 := m.Positions[i1]
			p2 := m.Positions[i2]
			// Face normal = (p1-p0) x (p2-p0); magnitude ∝ triangle area, so
			// summing area-weights larger faces — the standard smooth-normal
			// approach.
			ax, ay, az := float64(p1[0]-p0[0]), float64(p1[1]-p0[1]), float64(p1[2]-p0[2])
			bx, by, bz := float64(p2[0]-p0[0]), float64(p2[1]-p0[1]), float64(p2[2]-p0[2])
			nx := ay*bz - az*by
			ny := az*bx - ax*bz
			nz := ax*by - ay*bx
			for _, vi := range [3]uint32{i0, i1, i2} {
				acc[vi][0] += nx
				acc[vi][1] += ny
				acc[vi][2] += nz
			}
		}
	}

	m.Normals = make([][3]float32, len(m.Positions))
	for i, a := range acc {
		length := math.Sqrt(a[0]*a[0] + a[1]*a[1] + a[2]*a[2])
		if length < 1e-12 {
			m.Normals[i] = [3]float32{0, 0, 1}
			continue
		}
		m.Normals[i] = [3]float32{
			float32(a[0] / length),
			float32(a[1] / length),
			float32(a[2] / length),
		}
	}
}

// splitOversizedSubmeshes replaces any submesh that references more than
// maxGeosetVerts distinct vertices with several submeshes, each referencing at
// most that many distinct vertices. Triangles are kept whole (assigned as a unit
// to a chunk) so no triangle straddles two geosets. Submeshes that already fit
// are left untouched (and keep their identity/order relative to others).
func splitOversizedSubmeshes(m *Mesh) {
	var out []Submesh
	for _, sm := range m.Submeshes {
		if distinctVertexCount(sm.Indices) <= maxGeosetVerts {
			out = append(out, sm)
			continue
		}
		out = append(out, splitSubmesh(sm)...)
	}
	m.Submeshes = out
}

// distinctVertexCount returns how many unique vertex indices a flat index list
// references.
func distinctVertexCount(indices []uint32) int {
	seen := make(map[uint32]struct{}, len(indices))
	for _, i := range indices {
		seen[i] = struct{}{}
	}
	return len(seen)
}

// splitSubmesh greedily packs whole triangles into chunks, starting a new chunk
// whenever adding a triangle would push the chunk's distinct-vertex count over
// maxGeosetVerts. Each resulting submesh inherits the source material/texture.
func splitSubmesh(sm Submesh) []Submesh {
	var chunks []Submesh
	cur := Submesh{MaterialName: sm.MaterialName, TextureRef: sm.TextureRef}
	curSeen := make(map[uint32]struct{})

	flush := func() {
		if len(cur.Indices) > 0 {
			chunks = append(chunks, cur)
		}
		cur = Submesh{MaterialName: sm.MaterialName, TextureRef: sm.TextureRef}
		curSeen = make(map[uint32]struct{})
	}

	for t := 0; t+2 < len(sm.Indices); t += 3 {
		tri := [3]uint32{sm.Indices[t], sm.Indices[t+1], sm.Indices[t+2]}
		// How many of this triangle's vertices are new to the current chunk?
		added := 0
		for _, v := range tri {
			if _, ok := curSeen[v]; !ok {
				added++
			}
		}
		if len(curSeen)+added > maxGeosetVerts && len(cur.Indices) > 0 {
			flush()
		}
		for _, v := range tri {
			curSeen[v] = struct{}{}
		}
		cur.Indices = append(cur.Indices, tri[0], tri[1], tri[2])
	}
	flush()
	return chunks
}

func isZeroVec(v [3]float32) bool {
	return v[0] == 0 && v[1] == 0 && v[2] == 0
}
