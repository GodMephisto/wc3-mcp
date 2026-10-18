---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wtg/encode.go"
ref: HEAD
---

// Encode-side of the war3map.wtg binary format. Mirrors Parse byte-for-byte
// so a Parse → Encode round-trip on an untouched map produces exactly the
// original bytes.
//
// Critical preservation invariants (HiveWE triggers.ixx commentary):
//
//   - Parameter.Unknown — only meaningful in sub_version 7 when
//     HasSubParameter is true. HiveWE calls out that this field "sometimes
//     contains garbage" (L355) but the encoder writes it verbatim (L508).
//     We do too.
//   - Parameter.Value — the on-disk string is preserved verbatim even when
//     HasSubParameter is true (where the bytes are nominally meaningless).
//     The parser caught these bytes; the encoder echoes them.
//   - InitiallyOn — stored INVERTED on disk (HiveWE L889 writes
//     !initially_on). The encoder must re-invert so the parsed `true` ends
//     up as a `0` on disk.
//   - Deleted-id blocks (1.31+ only) — seven (count, count2, ids[]) tuples
//     parsed-and-preserved into Triggers.DeletedIDs. Encoder writes the
//     same `count==count2==len(ids)` shape HiveWE uses (no map carries
//     count != count2 in practice).
//   - Pre-1.31 PreCategoryUnknown — the 4 bytes between the categories
//     section and the variables section.
//
// The encoder needs argc per function name for symmetry with Parse — but
// only at the call-site level: each ECA's parameter slice already has the
// right length (Parse populated it from argc), so Encode walks that slice
// directly. The argc parameter is therefore unused today; it's accepted
// in the signature so future Encode-time validation can light up without
// a breaking change. (Sub-functions inside Parameter.SubParameter likewise
// already carry the right Parameters length.)
package wtg

import (
	"bytes"
	"encoding/binary"
	"fmt"
)

// Encode serializes a *Triggers back to .wtg bytes. argc is reserved for
// future symmetry with Parse — Encode walks the in-memory slice lengths
// today, so a nil argc is accepted. Returns an error if the Triggers
// pointer is nil or the header fields are inconsistent (e.g. pre-1.31
// flag set with a 1.31-shaped Version).
func Encode(t *Triggers, argc map[string]int) ([]byte, error) {
	if t == nil {
		return nil, fmt.Errorf("wtg.Encode: nil Triggers")
	}
	var buf bytes.Buffer
	w := newWriter(&buf)

	if t.IsPre131 {
		// Pre-1.31: no magic, version is the first u32.
		if t.Version != 4 && t.Version != 7 {
			return nil, fmt.Errorf("wtg.Encode(pre-31): version=%d, want 4 or 7", t.Version)
		}
		if err := encodeVersionPre31(w, t); err != nil {
			return nil, err
		}
		return buf.Bytes(), nil
	}

	// 1.31+: "WTG!" magic + version + sub_version.
	w.writeBytes([]byte(wtgMagic))
	w.writeU32(t.Version)
	if err := encodeVersion31(w, t); err != nil {
		return nil, err
	}
	return buf.Bytes(), nil
}

// encodeVersion31 mirrors loadVersion31. SubVersion is whatever the parse
// captured (4 or 7); the encoder echoes the original so a parsed-then-saved
// file stays byte-equal even on unknown sub-versions that HiveWE warns about
// but still accepts.
func encodeVersion31(w *writer, t *Triggers) error {
	w.writeU32(t.SubVersion)

	// Seven deleted-id blocks. Real-world maps may carry count != count2
	// (Enfo FFB ships count=1, count2=0 in slot 0). DeletedCounts preserves
	// the first field; DeletedIDs preserves the rest. nil slot → emit
	// (DeletedCounts[i], 0, nothing) so an untouched map with zero deletions
	// still round-trips.
	for i := 0; i < 7; i++ {
		ids := t.DeletedIDs[i]
		w.writeU32(t.DeletedCounts[i]) // count (may be > count2)
		w.writeU32(uint32(len(ids)))   // count2 (matches len(ids))
		for _, id := range ids {
			w.writeU32(uint32(id))
		}
	}

	w.writeU32(t.Unknown1)
	w.writeU32(t.Unknown2)
	w.writeU32(t.TrigDefVer)

	// Variables.
	w.writeU32(uint32(len(t.Variables)))
	for _, v := range t.Variables {
		w.writeCString(v.Name)
		w.writeCString(v.Type)
		w.writeU32(v.Unknown)
		w.writeBool32(v.IsArray)
		if t.SubVersion == 7 {
			w.writeU32(uint32(v.ArraySize))
		}
		w.writeBool32(v.IsInitialized)
		w.writeCString(v.InitialValue)
		w.writeU32(uint32(v.ID))
		w.writeU32(uint32(v.ParentID))
	}

	// Elements. The on-disk element block interleaves categories, triggers,
	// and variable-refs in arbitrary order (World Editor authoring history).
	// For round-trip byte equality the encoder walks t.Elements which was
	// populated by Parse with the original ordering. Mutators that ADD or
	// REMOVE entities are responsible for keeping Elements in sync.
	//
	// Fallback: if Elements is empty (e.g. session.synthesizeHandRolledScript
	// or future code that builds *Triggers without going through Parse), we
	// emit categories-then-triggers in their slice order. This still produces
	// a valid wtg — just one whose element order is canonicalized rather
	// than round-trip-byte-equal.
	elements := t.Elements
	if len(elements) == 0 {
		elements = make([]ElementRef, 0, len(t.Categories)+len(t.Triggers))
		for i := range t.Categories {
			elements = append(elements, ElementRef{Kind: ElementKindCategory, Index: i})
		}
		for i := range t.Triggers {
			elements = append(elements, ElementRef{Kind: ElementKindTrigger, Index: i})
		}
	}
	w.writeU32(uint32(len(elements)))
	for _, e := range elements {
		switch e.Kind {
		case ElementKindCategory:
			if e.Index < 0 || e.Index >= len(t.Categories) {
				return fmt.Errorf("wtg.Encode: stale element ref category index %d (have %d)", e.Index, len(t.Categories))
			}
			c := t.Categories[e.Index]
			w.writeU32(uint32(c.Classifier))
			w.writeU32(uint32(c.ID))
			w.writeCString(c.Name)
			if t.SubVersion == 7 {
				w.writeBool32(c.IsComment)
			}
			w.writeBool32(c.OpenState)
			w.writeU32(uint32(c.ParentID))
		case ElementKindTrigger:
			if e.Index < 0 || e.Index >= len(t.Triggers) {
				return fmt.Errorf("wtg.Encode: stale element ref trigger index %d (have %d)", e.Index, len(t.Triggers))
			}
			tr := t.Triggers[e.Index]
			w.writeU32(uint32(tr.Classifier))
			w.writeCString(tr.Name)
			w.writeCString(tr.Description)
			if t.SubVersion == 7 {
				w.writeBool32(tr.IsComment)
			}
			w.writeU32(uint32(tr.ID))
			w.writeBool32(tr.IsEnabled)
			w.writeBool32(tr.IsScript)
			// InitiallyOn is INVERTED on disk (HiveWE L889 writes !initially_on).
			w.writeBool32(!tr.InitiallyOn)
			w.writeBool32(tr.RunOnInitialization)
			w.writeU32(uint32(tr.ParentID))
			w.writeU32(uint32(len(tr.ECAs)))
			for i := range tr.ECAs {
				if err := encodeECA(w, &tr.ECAs[i], false, t.SubVersion); err != nil {
					return fmt.Errorf("wtg.Encode trigger %q ECA %d: %w", tr.Name, i, err)
				}
			}
		case ElementKindVariableRef:
			w.writeU32(uint32(ClassifierVariable))
			w.writeU32(e.RawID)
			w.writeCString(e.RawName)
			w.writeU32(e.RawParentID)
		default:
			return fmt.Errorf("wtg.Encode: unknown element kind %d", e.Kind)
		}
	}
	return nil
}

// encodeVersionPre31 mirrors loadVersionPre31. Categories, variables, and
// triggers live in three flat sequential blocks (no classifier discriminator
// on elements). The parser synthesized a "Map Header" + "Variables" pair at
// the front of t.Categories; the encoder must SKIP those when writing the
// real category block, since they didn't exist on disk.
//
// We also have to map the parser's "id == -2" collision-avoidance back to
// id == 0 on the way out (HiveWE: `if (i.id == 0) i.id = -2` at load).
//
// NOTE: pre-1.31 round-trip is best-effort. The parser synthesizes IDs from
// a running counter and the on-disk file doesn't store IDs for variables /
// triggers in this format, so re-encoding produces structurally-equivalent
// bytes (cat/var/trig counts + per-row blobs identical) but the synthetic
// IDs reset on the next parse. Real pre-1.31 maps are rare in 2026; this
// matches HiveWE's tolerance.
func encodeVersionPre31(w *writer, t *Triggers) error {
	w.writeU32(t.Version)

	// Skip the two synthesized prepend-categories the parser added (the
	// Map Header at id=0 + the Variables category at maxID). Count the
	// "real" categories we'll emit.
	realCats := 0
	for _, c := range t.Categories {
		if c.Classifier == ClassifierMap {
			continue
		}
		if c.ParentID == 0 && c.Name == "Variables" {
			continue
		}
		realCats++
	}
	w.writeU32(uint32(realCats))
	for _, c := range t.Categories {
		if c.Classifier == ClassifierMap {
			continue
		}
		if c.ParentID == 0 && c.Name == "Variables" {
			continue
		}
		// Reverse the load-side id=-2 collision-avoidance.
		outID := c.ID
		if outID == -2 {
			outID = 0
		}
		w.writeU32(uint32(outID))
		w.writeCString(c.Name)
		if t.Version == 7 {
			w.writeBool32(c.IsComment)
		}
	}

	// PreCategoryUnknown — preserved verbatim from Parse.
	w.writeU32(t.PreCategoryUnknown)

	// Variables — no IDs on disk in pre-31.
	w.writeU32(uint32(len(t.Variables)))
	for _, v := range t.Variables {
		w.writeCString(v.Name)
		w.writeCString(v.Type)
		w.writeU32(v.Unknown)
		w.writeBool32(v.IsArray)
		if t.Version == 7 {
			w.writeU32(uint32(v.ArraySize))
		}
		w.writeBool32(v.IsInitialized)
		w.writeCString(v.InitialValue)
	}

	// Triggers.
	w.writeU32(uint32(len(t.Triggers)))
	for _, tr := range t.Triggers {
		w.writeCString(tr.Name)
		w.writeCString(tr.Description)
		if t.Version == 7 {
			w.writeBool32(tr.IsComment)
		}
		w.writeBool32(tr.IsEnabled)
		w.writeBool32(tr.IsScript)
		w.writeBool32(!tr.InitiallyOn) // inverted on disk
		w.writeBool32(tr.RunOnInitialization)
		// Reverse the id=-2 collision avoidance for parent_id.
		parent := tr.ParentID
		if parent == -2 {
			parent = 0
		}
		w.writeU32(uint32(parent))
		w.writeU32(uint32(len(tr.ECAs)))
		for i := range tr.ECAs {
			if err := encodeECA(w, &tr.ECAs[i], false, t.Version); err != nil {
				return fmt.Errorf("wtg.Encode(pre-31) trigger %q ECA %d: %w", tr.Name, i, err)
			}
		}
	}
	return nil
}

// encodeECA mirrors parseECA. version is the SUB_version for 1.31+ or the
// version for pre-1.31 — the disk layout is the same at the ECA level.
func encodeECA(w *writer, eca *ECA, isChild bool, version uint32) error {
	w.writeU32(uint32(eca.Type))
	if isChild {
		w.writeU32(eca.Group)
	}
	w.writeCString(eca.Name)
	w.writeBool32(eca.Enabled)
	for i := range eca.Parameters {
		if err := encodeParameter(w, &eca.Parameters[i], version); err != nil {
			return fmt.Errorf("parameter %d of %q: %w", i, eca.Name, err)
		}
	}
	if version == 7 {
		w.writeU32(uint32(len(eca.Children)))
		for j := range eca.Children {
			if err := encodeECA(w, &eca.Children[j], true, version); err != nil {
				return fmt.Errorf("child %d of %q: %w", j, eca.Name, err)
			}
		}
	}
	return nil
}

// encodeParameter mirrors parseParameter. The gnarly path. Three load-bearing
// preservation rules ride here:
//
//  1. Value is written verbatim even when HasSubParameter (HiveWE L508 — the
//     bytes are nominal-only there but must round-trip).
//  2. Unknown is meaningful only in sub_version 7 when HasSubParameter is
//     true. We write 0 in every other case (Parse leaves it at the zero
//     value there so this is consistent).
//  3. For sub_version 4, the "function" type writes a u32 always_zero
//     instead of is_array. Non-function types write is_array directly. The
//     IsArray field on a function parameter SHOULD be false at parse — we
//     defensively emit 0 to match HiveWE regardless.
func encodeParameter(w *writer, p *Parameter, version uint32) error {
	w.writeU32(uint32(p.Type))
	w.writeCString(p.Value)
	w.writeBool32(p.HasSubParameter)
	if p.HasSubParameter {
		if p.SubParameter == nil {
			return fmt.Errorf("HasSubParameter=true but SubParameter is nil (value=%q)", p.Value)
		}
		sub := p.SubParameter
		w.writeU32(uint32(sub.Type))
		w.writeCString(sub.Name)
		// HasParameters preserved from parse — real maps ship has_params=true
		// with zero-arg functions (FFB carries GetTriggerPlayer this way).
		// Falls back to len-based detection for synthesized SubParameters that
		// don't have HasParameters set (e.g. user-constructed mutations).
		hasParams := sub.HasParameters || len(sub.Parameters) > 0
		w.writeBool32(hasParams)
		for i := range sub.Parameters {
			if err := encodeParameter(w, &sub.Parameters[i], version); err != nil {
				return fmt.Errorf("sub-parameter %d of %q: %w", i, sub.Name, err)
			}
		}
	}
	if version == 4 {
		if p.Type == ParamFunction {
			w.writeU32(0) // always_zero
		} else {
			w.writeBool32(p.IsArray)
		}
	} else {
		if p.HasSubParameter {
			w.writeU32(p.Unknown)
		}
		w.writeBool32(p.IsArray)
	}
	if p.IsArray {
		if p.ArrayIndex == nil {
			return fmt.Errorf("IsArray=true but ArrayIndex is nil (value=%q)", p.Value)
		}
		if err := encodeParameter(w, p.ArrayIndex, version); err != nil {
			return fmt.Errorf("array-index parameter: %w", err)
		}
	}
	return nil
}

// writer is the binary-write counterpart to reader. Same little-endian
// convention; no error accumulation needed since bytes.Buffer.Write* never
// returns an error.
type writer struct {
	w *bytes.Buffer
}

func newWriter(b *bytes.Buffer) *writer { return &writer{w: b} }

func (w *writer) writeU32(v uint32) {
	var b [4]byte
	binary.LittleEndian.PutUint32(b[:], v)
	w.w.Write(b[:])
}

func (w *writer) writeBool32(b bool) {
	if b {
		w.writeU32(1)
	} else {
		w.writeU32(0)
	}
}

func (w *writer) writeBytes(b []byte) { w.w.Write(b) }

// writeCString writes a NUL-terminated string. The string is written as-is
// (no escaping, no BOM); the only post-fix is the trailing zero byte.
func (w *writer) writeCString(s string) {
	w.w.WriteString(s)
	w.w.WriteByte(0)
}
