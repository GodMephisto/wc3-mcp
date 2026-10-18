---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/mdx/encode.go"
ref: HEAD
---

// Package mdx is a pure-Go encoder for minimal, in-game-renderable static
// Warcraft III MDX models (binary, VERS=800).
//
// EncodeStatic emits exactly the irreducible chunk set the WC3 engine needs to
// render a non-animated textured mesh:
//
//	MDLX, VERS(800), MODL, SEQS(1 "Stand"), MTLS, TEXS, GEOS, BONE(1), PIVT(1)
//
// No v>800 fields are emitted (no material shader string, no layer fresnel
// block, no geoset lod/lodName, no TANG/SKIN, no FAFX/BPOS). The byte layout
// follows mdx-m3-viewer's saveMdx chunk order and HiveWE's validator
// invariants; see .import-research/research_WC3_MDX_MDL_format_minimal_Go.md.
//
// This package is deliberately self-contained: its input is the local Model
// struct rather than internal/mesh3d, so it compiles and tests independently.
// The integration layer adapts mesh3d.Mesh -> mdx.Model.
package mdx

import (
	"bytes"
	"encoding/binary"
	"fmt"
	"math"
)

// maxGeosetVerts is the uint16 face-index ceiling for a single geoset. PVTX
// faces are uint16, so a geoset may reference at most 65535 distinct vertices.
const maxGeosetVerts = 65535

// Submesh is one source mesh partition. Each Submesh becomes one GEOS geoset,
// one MTLS material, and (1:1 in this v1) one TEXS entry. Indices are uint32 on
// input but are emitted as uint16 PVTX; EncodeStatic hard-errors if a submesh
// references more than maxGeosetVerts vertices (the upstream Normalizer splits,
// this is defense in depth).
type Submesh struct {
	Indices     []uint32
	TexturePath string
}

// Model is the self-contained input to EncodeStatic. Positions, Normals and UVs
// are parallel global vertex arrays shared by every submesh; each Submesh's
// Indices index into them.
type Model struct {
	Name      string
	Positions [][3]float32
	Normals   [][3]float32
	UVs       [][2]float32
	Submeshes []Submesh
}

// EncodeStatic serializes m into a minimal valid VERS=800 MDX byte stream.
func EncodeStatic(m *Model) ([]byte, error) {
	if m == nil {
		return nil, fmt.Errorf("mdx: nil model")
	}
	if len(m.Submeshes) == 0 {
		return nil, fmt.Errorf("mdx: model has no submeshes")
	}
	if len(m.Positions) == 0 {
		return nil, fmt.Errorf("mdx: model has no vertices")
	}
	if len(m.Normals) != len(m.Positions) {
		return nil, fmt.Errorf("mdx: normal count %d != vertex count %d", len(m.Normals), len(m.Positions))
	}
	// At least one UV set is mandatory; synthesize zeros if absent.
	uvs := m.UVs
	if len(uvs) == 0 {
		uvs = make([][2]float32, len(m.Positions))
	} else if len(uvs) != len(m.Positions) {
		return nil, fmt.Errorf("mdx: UV count %d != vertex count %d", len(uvs), len(m.Positions))
	}

	// Validate each submesh's indices and vertex-reference ceiling up front.
	for si, sm := range m.Submeshes {
		if len(sm.Indices)%3 != 0 {
			return nil, fmt.Errorf("mdx: submesh %d index count %d not a multiple of 3", si, len(sm.Indices))
		}
		maxIdx := -1
		for _, idx := range sm.Indices {
			if int(idx) >= len(m.Positions) {
				return nil, fmt.Errorf("mdx: submesh %d index %d out of range (%d vertices)", si, idx, len(m.Positions))
			}
			if int(idx) > maxIdx {
				maxIdx = int(idx)
			}
		}
		// A geoset references vertices [0, maxIdx]; that span must fit uint16.
		// We emit the full global vertex array per geoset (simple + correct),
		// so the relevant ceiling is the total vertex count.
		if len(m.Positions) > maxGeosetVerts {
			return nil, fmt.Errorf("mdx: submesh %d references %d vertices, exceeds uint16 cap %d (split upstream)", si, len(m.Positions), maxGeosetVerts)
		}
	}

	mExt := computeExtent(m.Positions)

	w := &chunkWriter{}
	w.writeTag("MDLX")

	encodeVERS(w)
	encodeMODL(w, m.Name, mExt)
	encodeSEQS(w, mExt)
	encodeMTLS(w, m.Submeshes)
	encodeTEXS(w, m.Submeshes)
	encodeGEOS(w, m, uvs)
	encodeBONE(w)
	encodePIVT(w)

	return w.bytes(), nil
}

// --- chunk encoders ---------------------------------------------------------

func encodeVERS(w *chunkWriter) {
	body := &chunkWriter{}
	body.writeU32(800)
	w.writeChunk("VERS", body)
}

func encodeMODL(w *chunkWriter, name string, ext extent) {
	body := &chunkWriter{}
	body.writeFixedString(name, 80) // name
	body.writeFixedString("", 260)  // animationFile (empty)
	ext.write(body)                 // extent (radius + min + max)
	body.writeU32(0)                // blendTime
	// MODL payload is fixed at 372 bytes (80 + 260 + 28 + 4).
	w.writeChunk("MODL", body)
}

func encodeSEQS(w *chunkWriter, modelExt extent) {
	body := &chunkWriter{}
	// One "Stand" sequence, 132 bytes.
	body.writeFixedString("Stand", 80) // name
	body.writeU32(0)                   // interval start
	body.writeU32(1)                   // interval end
	body.writeF32(0)                   // moveSpeed
	body.writeU32(0)                   // flags: 0 = looping
	body.writeF32(0)                   // rarity
	body.writeU32(0)                   // syncPoint
	// Extent = model extent. A zero-extent sequence breaks culling.
	modelExt.write(body)
	w.writeChunk("SEQS", body)
}

func encodeMTLS(w *chunkWriter, submeshes []Submesh) {
	body := &chunkWriter{}
	for i := range submeshes {
		mat := &chunkWriter{}
		mat.writeU32(0) // priorityPlane
		mat.writeU32(0) // flags
		// LAYS: layer count + layers.
		mat.writeTag("LAYS")
		mat.writeU32(1) // one layer
		layer := &chunkWriter{}
		layer.writeU32(0)             // filterMode = 0 (None/opaque)
		layer.writeU32(1)             // flags = 1 (Unshaded)
		layer.writeU32(uint32(i))     // textureId (index into TEXS)
		layer.writeI32(-1)            // textureAnimationId
		layer.writeU32(0)             // coordId
		layer.writeF32(1.0)           // alpha
		mat.writeInclusiveSize(layer) // layer inclusiveSize + body
		body.writeInclusiveSize(mat)  // material inclusiveSize + body
	}
	w.writeChunk("MTLS", body)
}

func encodeTEXS(w *chunkWriter, submeshes []Submesh) {
	body := &chunkWriter{}
	for i := range submeshes {
		body.writeU32(0)                                     // replaceableId
		body.writeFixedString(submeshes[i].TexturePath, 260) // path
		body.writeU32(0)                                     // wrapMode
	}
	w.writeChunk("TEXS", body)
}

func encodeGEOS(w *chunkWriter, m *Model, uvs [][2]float32) {
	body := &chunkWriter{}
	for matID, sm := range m.Submeshes {
		geo := &chunkWriter{}

		vcount := uint32(len(m.Positions))

		// VRTX
		geo.writeTag("VRTX")
		geo.writeU32(vcount)
		for _, p := range m.Positions {
			geo.writeF32(p[0])
			geo.writeF32(p[1])
			geo.writeF32(p[2])
		}

		// NRMS (count must equal VRTX count)
		geo.writeTag("NRMS")
		geo.writeU32(vcount)
		for _, n := range m.Normals {
			geo.writeF32(n[0])
			geo.writeF32(n[1])
			geo.writeF32(n[2])
		}

		// PTYP = [4] (triangles)
		geo.writeTag("PTYP")
		geo.writeU32(1)
		geo.writeU32(4)

		// PCNT = [totalIndices] (one face group spanning all indices)
		geo.writeTag("PCNT")
		geo.writeU32(1)
		geo.writeU32(uint32(len(sm.Indices)))

		// PVTX = uint16 triangle index list
		geo.writeTag("PVTX")
		geo.writeU32(uint32(len(sm.Indices)))
		for _, idx := range sm.Indices {
			geo.writeU16(uint16(idx))
		}

		// GNDX = per-vertex matrix-group index, all 0
		geo.writeTag("GNDX")
		geo.writeU32(vcount)
		for i := uint32(0); i < vcount; i++ {
			geo.writeU8(0)
		}

		// MTGC = [1] (matrix group 0 spans 1 matrix)
		geo.writeTag("MTGC")
		geo.writeU32(1)
		geo.writeU32(1)

		// MATS = [0] (that matrix = bone 0)
		geo.writeTag("MATS")
		geo.writeU32(1)
		geo.writeU32(0)

		geo.writeU32(uint32(matID)) // materialId
		geo.writeU32(0)             // selectionGroup
		geo.writeU32(0)             // selectionFlags

		// Per-geoset extent. Use the submesh's referenced vertices.
		gExt := computeSubmeshExtent(m.Positions, sm.Indices)
		gExt.write(geo)

		geo.writeU32(0) // sequenceExtentCount

		// UVAS -> UVBS (at least one UV set is mandatory).
		geo.writeTag("UVAS")
		geo.writeU32(1) // one UV set
		geo.writeTag("UVBS")
		geo.writeU32(vcount)
		for _, uv := range uvs {
			geo.writeF32(uv[0])
			geo.writeF32(uv[1])
		}

		body.writeInclusiveSize(geo) // geoset inclusiveSize + body
	}
	w.writeChunk("GEOS", body)
}

func encodeBONE(w *chunkWriter) {
	body := &chunkWriter{}
	gen := &chunkWriter{}
	// GenericObject: name + objectId + parentId + flags (no animation tracks).
	gen.writeFixedString("root", 80)
	gen.writeI32(0)  // objectId
	gen.writeI32(-1) // parentId
	gen.writeU32(0)  // flags
	// The GenericObject inclusiveSize (= 96: the 4-byte size field + 92 bytes of
	// name/ids/flags) covers ONLY the generic object and any animation tracks —
	// NOT the bone-specific geosetId/geosetAnimationId tail. Readers (the game
	// and mdx-m3-viewer) treat (inclusiveSize - 96) trailing bytes as animation
	// tracks; folding the 8-byte tail into inclusiveSize makes them read
	// geosetId (-1) as a bogus animation-track tag and crash. So the tail is
	// written AFTER the inclusive-size block. On-disk bone size is still 104.
	body.writeInclusiveSize(gen) // = 96
	body.writeI32(-1)            // geosetId (outside the generic inclusiveSize)
	body.writeI32(-1)            // geosetAnimationId
	w.writeChunk("BONE", body)
}

func encodePIVT(w *chunkWriter) {
	body := &chunkWriter{}
	// One pivot per node; one bone -> one pivot at origin.
	body.writeF32(0)
	body.writeF32(0)
	body.writeF32(0)
	w.writeChunk("PIVT", body)
}

// --- extent -----------------------------------------------------------------

// extent is the WC3 bounds record: boundsRadius FIRST, then min, then max
// (28 bytes total). Layout per mdx-m3-viewer extent.js.
type extent struct {
	radius   float32
	min, max [3]float32
}

func (e extent) write(w *chunkWriter) {
	w.writeF32(e.radius)
	w.writeF32(e.min[0])
	w.writeF32(e.min[1])
	w.writeF32(e.min[2])
	w.writeF32(e.max[0])
	w.writeF32(e.max[1])
	w.writeF32(e.max[2])
}

// computeExtent returns a non-zero extent over all positions. boundsRadius is
// the distance from the AABB center to the farthest vertex.
func computeExtent(positions [][3]float32) extent {
	return extentOver(positions, nil)
}

// computeSubmeshExtent returns an extent over just the vertices referenced by
// indices. Falls back to the full position set if indices is empty.
func computeSubmeshExtent(positions [][3]float32, indices []uint32) extent {
	if len(indices) == 0 {
		return extentOver(positions, nil)
	}
	return extentOver(positions, indices)
}

// extentOver computes the AABB + bounding radius. If indices is non-nil, only
// the referenced vertices participate; otherwise all positions do.
func extentOver(positions [][3]float32, indices []uint32) extent {
	if len(positions) == 0 {
		return extent{}
	}
	first := func() [3]float32 {
		if len(indices) > 0 {
			return positions[indices[0]]
		}
		return positions[0]
	}()
	min := first
	max := first

	visit := func(p [3]float32) {
		for i := 0; i < 3; i++ {
			if p[i] < min[i] {
				min[i] = p[i]
			}
			if p[i] > max[i] {
				max[i] = p[i]
			}
		}
	}
	if len(indices) > 0 {
		for _, idx := range indices {
			visit(positions[idx])
		}
	} else {
		for _, p := range positions {
			visit(p)
		}
	}

	center := [3]float32{
		(min[0] + max[0]) / 2,
		(min[1] + max[1]) / 2,
		(min[2] + max[2]) / 2,
	}
	var maxDistSq float64
	radiusVisit := func(p [3]float32) {
		dx := float64(p[0] - center[0])
		dy := float64(p[1] - center[1])
		dz := float64(p[2] - center[2])
		d := dx*dx + dy*dy + dz*dz
		if d > maxDistSq {
			maxDistSq = d
		}
	}
	if len(indices) > 0 {
		for _, idx := range indices {
			radiusVisit(positions[idx])
		}
	} else {
		for _, p := range positions {
			radiusVisit(p)
		}
	}

	return extent{
		radius: float32(math.Sqrt(maxDistSq)),
		min:    min,
		max:    max,
	}
}

// --- chunkWriter ------------------------------------------------------------

// chunkWriter is a little-endian byte accumulator with helpers for the MDX
// chunk/inclusive-size idiom. The buffer-then-prepend approach (writeChunk /
// writeInclusiveSize) sidesteps manual offset arithmetic: encode the body to a
// temp writer, then prepend its length.
type chunkWriter struct {
	buf bytes.Buffer
}

func (w *chunkWriter) bytes() []byte { return w.buf.Bytes() }

func (w *chunkWriter) writeBytes(b []byte) { w.buf.Write(b) }

func (w *chunkWriter) writeTag(tag string) {
	if len(tag) != 4 {
		panic(fmt.Sprintf("mdx: chunk tag %q must be 4 bytes", tag))
	}
	w.buf.WriteString(tag)
}

func (w *chunkWriter) writeU8(v uint8) { w.buf.WriteByte(v) }

func (w *chunkWriter) writeU16(v uint16) {
	var b [2]byte
	binary.LittleEndian.PutUint16(b[:], v)
	w.buf.Write(b[:])
}

func (w *chunkWriter) writeU32(v uint32) {
	var b [4]byte
	binary.LittleEndian.PutUint32(b[:], v)
	w.buf.Write(b[:])
}

func (w *chunkWriter) writeI32(v int32) { w.writeU32(uint32(v)) }

func (w *chunkWriter) writeF32(v float32) { w.writeU32(math.Float32bits(v)) }

// writeFixedString writes s into a zeroed buffer of exactly width bytes,
// NUL-padded (or truncated). Always reserves the last byte for a NUL terminator
// when s would fill the field.
func (w *chunkWriter) writeFixedString(s string, width int) {
	b := make([]byte, width)
	n := copy(b, s)
	// Ensure NUL termination if the string exactly fills (or overflows) the
	// field — WC3 strings are NUL-terminated, fixed-width.
	if n >= width {
		b[width-1] = 0
	}
	w.buf.Write(b)
}

// writeChunk writes a named chunk: 4-byte tag + uint32 body-size + body.
func (w *chunkWriter) writeChunk(tag string, body *chunkWriter) {
	w.writeTag(tag)
	w.writeU32(uint32(body.buf.Len()))
	w.buf.Write(body.bytes())
}

// writeInclusiveSize writes a uint32 inclusiveSize field (covering the size
// field itself plus the body) immediately followed by the body. Used for
// materials, layers, geosets and bones.
func (w *chunkWriter) writeInclusiveSize(body *chunkWriter) {
	w.writeU32(uint32(4 + body.buf.Len()))
	w.buf.Write(body.bytes())
}
