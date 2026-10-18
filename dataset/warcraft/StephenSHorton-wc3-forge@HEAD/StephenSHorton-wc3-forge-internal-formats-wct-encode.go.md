---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wct/encode.go"
ref: HEAD
---

// Encode-side of the war3map.wct binary format. Mirrors Parse byte-for-byte
// so a Parse → Encode round-trip on an untouched file produces exactly the
// original bytes.
//
// Wire format (HiveWE triggers.ixx L779-823 + L924-938):
//
//   1.31+:
//     u32   version = 0x80000004
//     u32   sub_version (0 | 1)
//     if sub_version == 1:
//       cstr global_jass_comment
//       u32  global_jass_size
//       char[size] global_jass    (skipped on save when size==0)
//     for each NON-COMMENT trigger (in category-then-trigger order):
//       u32 size
//       char[size] custom_text + NUL terminator (size includes the NUL)
//
//   Pre-1.31:
//     u32 version (0 | 1)
//     if version == 1:
//       cstr global_jass_comment
//       u32  size
//       char[size] global_jass
//     u32 unknown (zero in HiveWE writes)
//     for ALL triggers (no is_comment skip):
//       u32 size; char[size] custom_text+NUL
//
// CRITICAL ORDERING (HiveWE L924-938): the per-trigger blob order is
// "categories outer, triggers whose ParentID matches inner" — NOT trigger
// insertion order. Encode walks t.Categories and matches each Trigger.ParentID
// to find the per-blob order. Mismatched categories/triggers stay in the
// file but their wct entries become unreachable from the binding (which is
// what HiveWE accepts too).
//
// For round-trip-byte-equal: HiveWE's writer formats each per-trigger blob as
// `size = len(text)+1` (the +1 is the NUL terminator), followed by the text
// bytes + a single 0x00. Empty triggers write size=0 + no body (HiveWE checks
// `if (size == 0) continue` on read; on write it always emits size+1 even for
// empty strings — but real maps that contain empty custom_text often emit
// size=0 only). Parse tolerates both shapes; Encode follows HiveWE's shape
// of always writing size+1+NUL.
package wct

import (
	"bytes"
	"encoding/binary"
	"fmt"

	"github.com/StephenSHorton/wc3-forge/internal/formats/wtg"
)

// Encode serializes a *File + wtg.Triggers pair back to .wct bytes.
//
// The wtg argument is REQUIRED because the per-trigger blob order is derived
// from the wtg's category-then-trigger walk (HiveWE triggers.ixx L924-938).
// Each trigger's CustomText field is read directly off the wtg.Trigger; the
// File argument supplies the header (version, sub_version, global JASS).
//
// Returns an error if the File pointer is nil. A nil wtg is accepted (encodes
// just the header — produces a degenerate but valid .wct).
func Encode(f *File, t *wtg.Triggers) ([]byte, error) {
	if f == nil {
		return nil, fmt.Errorf("wct.Encode: nil File")
	}
	var buf bytes.Buffer
	w := newWriter(&buf)

	if f.IsPre131 {
		w.writeU32(f.Version)
		if f.Version == 1 {
			w.writeCString(f.GlobalJASSComment)
			w.writeU32(uint32(len(f.GlobalJASS)))
			if len(f.GlobalJASS) > 0 {
				w.writeBytes([]byte(f.GlobalJASS))
			}
		}
		// HiveWE write-side: emits a zero u32 here. The Parse path advanced
		// past it; we re-emit the documented zero.
		w.writeU32(0)
		if t != nil {
			ordered := walkOrderedTriggers(t, false)
			for i, tr := range ordered {
				writeBlobWithRawSize(w, tr.CustomText, rawSizeAt(f, i))
			}
		}
		return buf.Bytes(), nil
	}

	// 1.31+ format.
	w.writeU32(f.Version)
	w.writeU32(f.SubVersion)
	if f.SubVersion == 1 {
		w.writeCString(f.GlobalJASSComment)
		w.writeU32(uint32(len(f.GlobalJASS)))
		if len(f.GlobalJASS) > 0 {
			w.writeBytes([]byte(f.GlobalJASS))
		}
	}
	if t != nil {
		ordered := walkOrderedTriggers(t, true)
		for i, tr := range ordered {
			writeBlobWithRawSize(w, tr.CustomText, rawSizeAt(f, i))
		}
	}
	return buf.Bytes(), nil
}

// rawSizeAt returns the preserved on-disk size for the i-th blob, or 0 if
// out of range (i.e. a newly-added trigger that has no Parse-time size).
func rawSizeAt(f *File, i int) uint32 {
	if i < 0 || i >= len(f.RawSizes) {
		return 0
	}
	return f.RawSizes[i]
}

// walkOrderedTriggers returns the per-trigger blob order HiveWE writes:
// outer loop over Categories (in t.Categories order), inner loop over
// Triggers whose ParentID matches the current category's ID. For 1.31+
// (skipComments=true), is_comment triggers are SKIPPED — HiveWE doesn't
// emit a blob for them in that format. Pre-1.31 keeps every trigger.
//
// Triggers whose ParentID doesn't match any category (orphans from a
// malformed wtg) are silently dropped — same behavior as HiveWE.
func walkOrderedTriggers(t *wtg.Triggers, skipComments bool) []*wtg.Trigger {
	out := make([]*wtg.Trigger, 0, len(t.Triggers))
	for _, cat := range t.Categories {
		for i := range t.Triggers {
			tr := &t.Triggers[i]
			if tr.ParentID != cat.ID {
				continue
			}
			if skipComments && tr.IsComment {
				continue
			}
			out = append(out, tr)
		}
	}
	return out
}

// writeBlobWithRawSize writes one per-trigger blob, preferring the preserved
// rawSize over the canonical "len(s)+1" form. This keeps round-trip-byte-equal
// for real maps that ship odd size conventions:
//
//   - size=0 + no body  → empty string (canonical)
//   - size=1 + one NUL  → empty string (some FFB blobs)
//   - size=N+1 + body + NUL → standard non-empty (most blobs)
//   - size=N+2 + body + 2 NULs → rare double-terminated (also seen in FFB)
//
// When rawSize is 0 (newly-added trigger or unedited empty), we use the
// canonical form. When rawSize > 0 and the text length matches the implied
// content, we echo the original byte pattern.
//
// IMPORTANT: if the user EDITS a trigger's custom_text, rawSize from Parse
// no longer matches len(s). We detect that and fall through to the canonical
// "len(s)+1" emission so edits land correctly. The byte-equality guarantee
// holds for UNTOUCHED triggers; edited triggers re-canonicalize.
func writeBlobWithRawSize(w *writer, s string, rawSize uint32) {
	// New / cleared trigger — canonical empty.
	if rawSize == 0 {
		if s == "" {
			w.writeU32(0)
			return
		}
		// rawSize=0 but text is non-empty → newly-set text (mutator created
		// the trigger or filled the slot). Canonical: len+1 + body + NUL.
		body := []byte(s)
		w.writeU32(uint32(len(body) + 1))
		w.writeBytes(body)
		w.writeBytes([]byte{0})
		return
	}
	// rawSize was captured at Parse time. Did the user edit the text?
	// Parse trimmed trailing NULs so the byte length of s is rawSize minus
	// the count of trailing NULs. If len(s) >= rawSize, the user appended
	// content past the original — re-canonicalize. Otherwise we can
	// recover the on-disk shape by writing s padded with (rawSize - len(s))
	// trailing NULs.
	body := []byte(s)
	if uint32(len(body)) >= rawSize {
		// Edited / grown — canonical emission.
		w.writeU32(uint32(len(body) + 1))
		w.writeBytes(body)
		w.writeBytes([]byte{0})
		return
	}
	w.writeU32(rawSize)
	w.writeBytes(body)
	pad := int(rawSize) - len(body)
	w.writeBytes(make([]byte, pad))
}

// writer mirrors wtg's; intentionally duplicated to keep wct import-free of
// wtg's internal helpers.
type writer struct {
	w *bytes.Buffer
}

func newWriter(b *bytes.Buffer) *writer { return &writer{w: b} }

func (w *writer) writeU32(v uint32) {
	var b [4]byte
	binary.LittleEndian.PutUint32(b[:], v)
	w.w.Write(b[:])
}

func (w *writer) writeBytes(b []byte) { w.w.Write(b) }

func (w *writer) writeCString(s string) {
	w.w.WriteString(s)
	w.w.WriteByte(0)
}
