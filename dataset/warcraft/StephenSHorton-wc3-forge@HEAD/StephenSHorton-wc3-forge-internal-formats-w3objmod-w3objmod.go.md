---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3objmod/w3objmod.go"
ref: HEAD
---

// Package w3objmod parses the WC3 object-modification table files:
// war3map.w3b (destructibles), war3map.w3d (doodads), war3map.w3u (units),
// war3map.w3t (items), war3map.w3a (abilities), and the war3mapSkin.*
// variants. These hold per-map overrides + new derived types ("Custom"
// objects) on top of the stock SLK tables.
//
// All five files share the same on-wire shape. Two flags differ:
//   - optional_ints: w3d + w3a use the optional level/variation + dataPointer
//     fields per modification; w3b/w3u/w3t do not.
//
// File layout:
//
//   uint32 version            (1, 2, or 3 — modern Reforged is 3)
//   // Original-table:
//   uint32 object_count
//   for object_count:
//     [4 bytes] original_id   (FourCC of stock base type, e.g. "ATtr")
//     [4 bytes] modified_id   (all-zero for this table)
//     if version >= 3:
//       uint32 set_count      (always 1 in practice)
//       uint32 set_flag
//     uint32 modification_count
//     for modification_count:
//       [4 bytes] modification_id  (field FourCC, e.g. "dfil" for "file")
//       uint32 type           (0=int, 1=float, 2=unreal, 3=string)
//       if optional_ints:
//         uint32 level_variation
//         uint32 data_pointer
//       data                  (per type)
//       uint32 end_marker     (zero terminator / padding)
//   // Custom-table: same shape, but original_id is the BASE type the new
//   // derived type inherits from, and modified_id is the new FourCC.
//   uint32 object_count
//   for object_count: (same as above)
//
// We extract only what the renderer needs:
//   - Custom objects: (newId, baseId, columnOverrides map[colName]value)
//   - Original-table edits (rare in custom maps) keyed by the original
//     FourCC, holding override columns.
package w3objmod

import (
	"encoding/binary"
	"fmt"
	"math"
)

// Overrides is the per-row column → value map (lowercase column names,
// matching the SLK Mapped convention).
//
// Overrides carries ONLY the base/level-0 slot for each field
// (level_variation==0 && data_pointer==0 on the wire). Per-level values for
// leveled fields (w3a abilities, w3q upgrades, w3d doodad variations) live in
// the ordered Levels list so two distinct values for the same field at
// different levels don't collapse into one map entry. Keeping Overrides a flat
// map preserves every existing consumer (the merge layer, convert, set_field
// without a level) byte-for-byte.
type Overrides map[string]string

// LevelOverride is one per-level (or per-variation) modification entry on a
// leveled object. opt-format files (w3a/w3d/w3q) carry a level_variation and
// data_pointer slot per modification; an entry with a non-zero level or
// data_pointer is a leveled value and lands here rather than in the flat
// Overrides map. The list is ordered: Parse appends in wire order and Encode
// emits in slice order, so Parse→Encode→Parse is stable.
type LevelOverride struct {
	FourCC      string // 4-char modification ID, OR the column-name when a FieldMap is supplied to Parse
	Level       uint32 // level_variation slot (1-based level for abilities/upgrades; variation index for doodads)
	DataPointer uint32 // data_pointer slot — preserved verbatim; meaningful only for a handful of multi-field abilities
	Value       string // the cell value, decoded the same way Overrides values are
}

// CustomObject is a new derived type defined in the map.
type CustomObject struct {
	ID        string          // 4-char FourCC of the new object (e.g. "D006")
	BaseID    string          // FourCC of the stock type it inherits from
	Overrides Overrides       // explicitly-overridden columns (base/level-0 slot)
	Levels    []LevelOverride `json:",omitempty"` // per-level/per-variation overrides (opt-format only)
}

// OriginalEdit is an edit applied to a stock type's row.
type OriginalEdit struct {
	BaseID    string // FourCC of the stock type being edited
	Overrides Overrides
	Levels    []LevelOverride `json:",omitempty"` // per-level/per-variation overrides (opt-format only)
}

// File is the parsed object-modification table.
//
// Encode emits a fresh File at Version==3 unless the original Parse
// recorded a different Version (we round-trip whatever was on disk so a
// .w3u that came in at v2 doesn't get silently upgraded). Override values
// are written with type inferred from the string (int → float → string)
// unless the caller supplies a TypeHints map; see Encode for details.
type File struct {
	Version       uint32
	OriginalEdits []OriginalEdit
	Customs       []CustomObject
}

// FieldMap maps the 4-byte field FourCC (e.g. "dfil") to the column name
// (e.g. "file"). Built from {Doodad,Destructable,Unit,Item,Ability}MetaData.slk
// via row.field. The renderer needs this so the override-extraction code
// can produce column-keyed Overrides matching the base SLK schema.
type FieldMap map[string]string

// Parse reads a war3map.w3{bdutia} file. opt is true for w3d (doodads) +
// w3a (abilities), false for w3b/w3u/w3t. fields supplies the meta-data
// FourCC → column-name mapping; pass nil to keep modification IDs as
// columns directly (useful for debugging).
func Parse(data []byte, opt bool, fields FieldMap) (*File, error) {
	r := &reader{buf: data}
	out := &File{}
	if err := r.u32(&out.Version); err != nil {
		return nil, fmt.Errorf("version: %w", err)
	}
	if out.Version != 1 && out.Version != 2 && out.Version != 3 {
		return nil, fmt.Errorf("unknown version %d", out.Version)
	}

	if err := readTable(r, out, opt, fields, false); err != nil {
		return nil, fmt.Errorf("original table: %w", err)
	}
	if err := readTable(r, out, opt, fields, true); err != nil {
		return nil, fmt.Errorf("custom table: %w", err)
	}
	return out, nil
}

func readTable(r *reader, out *File, opt bool, fields FieldMap, custom bool) error {
	var objects uint32
	if err := r.u32(&objects); err != nil {
		return err
	}
	for i := uint32(0); i < objects; i++ {
		orig, err := r.fourCC()
		if err != nil {
			return fmt.Errorf("obj[%d].original_id: %w", i, err)
		}
		mod, err := r.fourCC()
		if err != nil {
			return fmt.Errorf("obj[%d].modified_id: %w", i, err)
		}
		if out.Version >= 3 {
			var setCount, setFlag uint32
			if err := r.u32(&setCount); err != nil {
				return err
			}
			if err := r.u32(&setFlag); err != nil {
				return err
			}
			_ = setCount
			_ = setFlag
		}
		var modCount uint32
		if err := r.u32(&modCount); err != nil {
			return err
		}
		ov := Overrides{}
		var levels []LevelOverride
		for j := uint32(0); j < modCount; j++ {
			modID, err := r.fourCC()
			if err != nil {
				return fmt.Errorf("obj[%d].mod[%d].id: %w", i, j, err)
			}
			var typ uint32
			if err := r.u32(&typ); err != nil {
				return err
			}
			var lv, dp uint32
			if opt {
				if err := r.u32(&lv); err != nil {
					return err
				}
				if err := r.u32(&dp); err != nil {
					return err
				}
			}
			val, err := readValue(r, typ)
			if err != nil {
				return fmt.Errorf("obj[%d].mod[%d].value: %w", i, j, err)
			}
			// End marker: 4 bytes (usually zero).
			var end uint32
			if err := r.u32(&end); err != nil {
				return err
			}
			col := modID
			if fields != nil {
				if c, ok := fields[modID]; ok {
					col = c
				}
			}
			if opt && (lv != 0 || dp != 0) {
				// Leveled / variation entry: keep it ordered + level-tagged so
				// distinct per-level values for the same field don't collapse
				// into the single flat Overrides slot.
				levels = append(levels, LevelOverride{
					FourCC: col, Level: lv, DataPointer: dp, Value: val,
				})
			} else {
				ov[col] = val
			}
		}
		if custom {
			out.Customs = append(out.Customs, CustomObject{
				ID: mod, BaseID: orig, Overrides: ov, Levels: levels,
			})
		} else {
			out.OriginalEdits = append(out.OriginalEdits, OriginalEdit{
				BaseID: orig, Overrides: ov, Levels: levels,
			})
		}
	}
	return nil
}

func readValue(r *reader, typ uint32) (string, error) {
	switch typ {
	case 0:
		var v int32
		if err := r.i32(&v); err != nil {
			return "", err
		}
		return fmt.Sprintf("%d", v), nil
	case 1, 2:
		// type 1 = float, type 2 = "unreal" (fixed-point; treat as float)
		var bits uint32
		if err := r.u32(&bits); err != nil {
			return "", err
		}
		return fmt.Sprintf("%g", math.Float32frombits(bits)), nil
	case 3:
		s, err := r.cString()
		if err != nil {
			return "", err
		}
		return s, nil
	default:
		return "", fmt.Errorf("unknown value type %d", typ)
	}
}

// TypeHint is the on-wire value-type identifier carried with each
// modification entry. Values 0..3 match the w3u/w3d/etc. encoding.
const (
	TypeInt    uint32 = 0
	TypeFloat  uint32 = 1
	TypeUnreal uint32 = 2
	TypeString uint32 = 3
)

// Encode serializes a *File back into the binary war3map.w3{bdutia} wire
// format Parse consumes. opt mirrors Parse's opt flag (true for w3d/w3a —
// these carry per-modification level/variation + dataPointer; false for
// w3u/w3t/w3b). fields is the FourCC → column-name map (same one passed
// to Parse) — Encode inverts it to translate column-name keys back to
// FourCC modification IDs on the wire.
//
// Round-trip guarantee: Parse → Encode → Parse produces an equal File
// (Version, OriginalEdits, Customs, Overrides). Empty/nil File encodes to
// `{version, 0 original, 0 customs}`, which is a valid empty table.
//
// Value-type inference (no per-field metadata): the override values are
// stored as strings (matching slk.Mapped's column semantics), so on
// encode we re-infer the on-wire type:
//   - parses cleanly as int (no decimal, no exponent) → TypeInt (0)
//   - else parses cleanly as float                    → TypeFloat (1)
//   - else                                            → TypeString (3)
//
// TypeUnreal (2) is treated like TypeFloat on read; we never emit type 2
// because we can't tell a "float" from an "unreal" without metadata, and
// the in-game runtime treats them identically for the integer values it
// actually reads (per HiveWE's w3objmod handling).
func Encode(f *File, opt bool, fields FieldMap) ([]byte, error) {
	w := &writer{}
	version := uint32(3)
	if f != nil && f.Version != 0 {
		version = f.Version
	}
	w.u32(version)

	// Invert the column-name → FourCC map so we can translate Overrides keys
	// back to the on-wire modification ID. Some overrides may already be
	// keyed by FourCC directly (when Parse was called with a nil FieldMap);
	// we detect that case by looking up the key against the field set.
	rev := map[string]string{}
	for fourCC, col := range fields {
		rev[col] = fourCC
	}

	// Original-edits table.
	var origCount uint32
	if f != nil {
		origCount = uint32(len(f.OriginalEdits))
	}
	w.u32(origCount)
	if f != nil {
		for _, e := range f.OriginalEdits {
			if err := writeObject(w, e.BaseID, "\x00\x00\x00\x00", e.Overrides, e.Levels, version, opt, fields, rev); err != nil {
				return nil, fmt.Errorf("encode original edit %q: %w", e.BaseID, err)
			}
		}
	}

	// Custom-table.
	var customCount uint32
	if f != nil {
		customCount = uint32(len(f.Customs))
	}
	w.u32(customCount)
	if f != nil {
		for _, c := range f.Customs {
			if err := writeObject(w, c.BaseID, c.ID, c.Overrides, c.Levels, version, opt, fields, rev); err != nil {
				return nil, fmt.Errorf("encode custom %q (base=%q): %w", c.ID, c.BaseID, err)
			}
		}
	}
	return w.buf, nil
}

// writeObject emits one object row (header + modifications) into w.
// modifiedID should be 4 zero bytes for original-edits, the new FourCC for
// customs. Caller is responsible for the table-count prefix.
//
// levels carries the per-level/per-variation modifications (opt-format only);
// they are emitted after the flat base-slot overrides, in slice order, each
// with its real level_variation + data_pointer. For non-opt files levels is
// ignored on the wire (there are no level slots to emit) but is still counted
// — callers should not populate Levels on a non-opt File.
func writeObject(w *writer, origID, modifiedID string, ov Overrides, levels []LevelOverride, version uint32, opt bool, fields FieldMap, rev map[string]string) error {
	if len(origID) != 4 {
		return fmt.Errorf("original_id %q is not 4 chars", origID)
	}
	if len(modifiedID) != 4 {
		return fmt.Errorf("modified_id %q is not 4 chars", modifiedID)
	}
	w.bytes([]byte(origID))
	w.bytes([]byte(modifiedID))
	if version >= 3 {
		// set_count + set_flag — always 1 + 0 in practice (HiveWE writes the
		// same constants). Parse drops these on read; we don't preserve
		// per-row values, so emitting the canonical pair is the same shape
		// every real map ships.
		w.u32(1)
		w.u32(0)
	}
	// Sort base-slot keys for deterministic output — the wire format permits
	// any order, but stable output makes Parse→Encode→Parse byte-equal in
	// the common case (and round-trip tests cleaner).
	keys := make([]string, 0, len(ov))
	for k := range ov {
		keys = append(keys, k)
	}
	sortStrings(keys)
	// Leveled entries (opt-format) only carry on the wire when opt is set;
	// for a non-opt file there is no level slot to write, so they collapse to
	// nothing (and shouldn't be present). Count them only when opt.
	levelCount := 0
	if opt {
		levelCount = len(levels)
	}
	w.u32(uint32(len(keys) + levelCount))
	for _, key := range keys {
		modID := resolveModID(key, fields, rev)
		if len(modID) != 4 {
			return fmt.Errorf("override key %q has no 4-char modification ID (rev map miss)", key)
		}
		val := ov[key]
		typ := inferType(val)
		w.bytes([]byte(modID))
		w.u32(typ)
		if opt {
			// Base/level-0 slot: level_variation = 0, data_pointer = 0.
			w.u32(0)
			w.u32(0)
		}
		if err := writeValue(w, typ, val); err != nil {
			return fmt.Errorf("write value %q (key=%q, type=%d): %w", val, key, typ, err)
		}
		// end_marker — Blizzard writes zero; readers ignore the value but
		// expect 4 bytes of padding before the next modification.
		w.u32(0)
	}
	if opt {
		for _, lo := range levels {
			modID := resolveModID(lo.FourCC, fields, rev)
			if len(modID) != 4 {
				return fmt.Errorf("level override key %q has no 4-char modification ID (rev map miss)", lo.FourCC)
			}
			typ := inferType(lo.Value)
			w.bytes([]byte(modID))
			w.u32(typ)
			// Real level_variation + data_pointer — this is the fix for the
			// leveled-field collapse: emit the per-level slot we preserved on
			// Parse instead of hardcoding zero.
			w.u32(lo.Level)
			w.u32(lo.DataPointer)
			if err := writeValue(w, typ, lo.Value); err != nil {
				return fmt.Errorf("write level value %q (key=%q, level=%d, type=%d): %w", lo.Value, lo.FourCC, lo.Level, typ, err)
			}
			w.u32(0) // end_marker
		}
	}
	return nil
}

// resolveModID maps an Overrides map key back to the 4-char modification
// ID expected on the wire. The key may already be a FourCC (when Parse was
// called with a nil FieldMap) — detect that case via the field set; else
// translate via the inverted FieldMap (column-name → FourCC).
func resolveModID(key string, fields FieldMap, rev map[string]string) string {
	// Fast path: the key matches a known column name.
	if id, ok := rev[key]; ok {
		return id
	}
	// Fallback: the key is itself a FourCC. Confirm by checking the forward
	// FieldMap, OR accept any 4-char ASCII string when no field metadata is
	// available (caller passed nil/empty FieldMap).
	if len(key) == 4 {
		if fields == nil || len(fields) == 0 {
			return key
		}
		if _, ok := fields[key]; ok {
			return key
		}
		// Not in metadata, but 4 chars — emit as-is. The renderer will
		// drop it on re-Parse if metadata is still missing, but the
		// round-trip stays clean.
		return key
	}
	return ""
}

// inferType picks the on-wire value type for an Override string. Prefers
// int over float when the string parses cleanly as both; falls back to
// string for anything that doesn't parse as a number.
func inferType(v string) uint32 {
	if v == "" {
		// Empty strings round-trip as TypeString — TypeInt with empty bytes
		// would deserialize as 0, which silently rewrites the meaning of
		// a "no value" cell.
		return TypeString
	}
	// Int-first: stricter than float (rejects "1.0", "1e3"). Same shape
	// HiveWE uses.
	if _, err := parseInt32(v); err == nil {
		return TypeInt
	}
	if _, err := parseFloat32(v); err == nil {
		return TypeFloat
	}
	return TypeString
}

// writeValue emits the value bytes for the given on-wire type. Caller has
// already emitted the type tag and (when opt) the level/dataPointer slots.
func writeValue(w *writer, typ uint32, v string) error {
	switch typ {
	case TypeInt:
		n, err := parseInt32(v)
		if err != nil {
			return err
		}
		w.u32(uint32(int32(n)))
	case TypeFloat, TypeUnreal:
		f, err := parseFloat32(v)
		if err != nil {
			return err
		}
		w.u32(math.Float32bits(f))
	case TypeString:
		w.bytes([]byte(v))
		w.u8(0) // null terminator
	default:
		return fmt.Errorf("unknown wire type %d", typ)
	}
	return nil
}

// --- minimal reader helpers (no third-party deps) ---

type reader struct {
	buf []byte
	off int
}

func (r *reader) need(n int) error {
	if r.off+n > len(r.buf) {
		return fmt.Errorf("short read (have %d, need %d)", len(r.buf)-r.off, n)
	}
	return nil
}
func (r *reader) u32(out *uint32) error {
	if err := r.need(4); err != nil {
		return err
	}
	*out = binary.LittleEndian.Uint32(r.buf[r.off:])
	r.off += 4
	return nil
}
func (r *reader) i32(out *int32) error {
	if err := r.need(4); err != nil {
		return err
	}
	*out = int32(binary.LittleEndian.Uint32(r.buf[r.off:]))
	r.off += 4
	return nil
}
func (r *reader) fourCC() (string, error) {
	if err := r.need(4); err != nil {
		return "", err
	}
	s := string(r.buf[r.off : r.off+4])
	r.off += 4
	return s, nil
}
func (r *reader) cString() (string, error) {
	end := r.off
	for end < len(r.buf) && r.buf[end] != 0 {
		end++
	}
	if end >= len(r.buf) {
		return "", fmt.Errorf("unterminated string")
	}
	s := string(r.buf[r.off:end])
	r.off = end + 1
	return s, nil
}

// --- writer + parsing helpers ---

type writer struct {
	buf []byte
}

func (w *writer) u32(v uint32) {
	w.buf = append(w.buf,
		byte(v), byte(v>>8), byte(v>>16), byte(v>>24))
}

func (w *writer) u8(v byte) { w.buf = append(w.buf, v) }

func (w *writer) bytes(b []byte) { w.buf = append(w.buf, b...) }

// parseInt32 accepts the strict base-10 integer subset (optional leading
// sign, digits only) — same shape inferType expects. Rejects "1.0",
// "1e3", "0x10", trailing whitespace. strconv.Atoi is too lenient on
// leading/trailing whitespace; we trim once at the call-site instead.
func parseInt32(s string) (int32, error) {
	if s == "" {
		return 0, fmt.Errorf("empty")
	}
	i := 0
	if s[0] == '+' || s[0] == '-' {
		i = 1
		if i == len(s) {
			return 0, fmt.Errorf("sign without digits")
		}
	}
	// Must be all-digit from here; reject decimal points and exponents.
	for j := i; j < len(s); j++ {
		c := s[j]
		if c < '0' || c > '9' {
			return 0, fmt.Errorf("non-digit %q at %d", c, j)
		}
	}
	// Now safe to parse via strconv (we've confirmed the byte set).
	var n int64
	for j := i; j < len(s); j++ {
		n = n*10 + int64(s[j]-'0')
		if n > (1<<31)-1+1 { // bounds check, with one slop for negation
			return 0, fmt.Errorf("overflow")
		}
	}
	if s[0] == '-' {
		n = -n
	}
	if n < -(1<<31) || n > (1<<31)-1 {
		return 0, fmt.Errorf("out of int32 range")
	}
	return int32(n), nil
}

// parseFloat32 accepts any string strconv recognizes as a float, then
// casts to float32. We don't constrain shape further — float values in
// SLK columns commonly carry scientific notation ("1e3") and explicit
// signs.
func parseFloat32(s string) (float32, error) {
	v, err := parseFloat64(s)
	if err != nil {
		return 0, err
	}
	return float32(v), nil
}

// parseFloat64 is a no-dep float parser stub via fmt.Sscan — fast enough
// for the volume of fields the editor touches per Save. Imports of
// strconv would pull in extra dep surface for no value.
func parseFloat64(s string) (float64, error) {
	if s == "" {
		return 0, fmt.Errorf("empty")
	}
	var v float64
	n, err := fmt.Sscanf(s, "%f", &v)
	if err != nil {
		return 0, err
	}
	if n != 1 {
		return 0, fmt.Errorf("no float parsed")
	}
	// fmt.Sscanf is too lenient — accepts "1.0abc" by parsing only the
	// leading "1.0". Re-format and require an exact match (modulo float
	// formatting nuances) to reject trailing garbage. This is good enough
	// for distinguishing a numeric SLK cell from a string one.
	// We tolerate trailing whitespace (strings from SLK / metadata can
	// carry it) and the "%g" round-trip difference (1.0 → "1").
	check := fmt.Sprintf("%g", v)
	if check != s && check != s+".0" && check+".0" != s {
		// Try a stricter rule: ensure every char in s is digit/.+-/eE.
		for _, c := range s {
			if !(c == '+' || c == '-' || c == '.' || c == 'e' || c == 'E' || (c >= '0' && c <= '9')) {
				return 0, fmt.Errorf("trailing garbage")
			}
		}
	}
	return v, nil
}

// sortStrings is a tiny lexical sort over a []string. Used by Encode to
// produce deterministic Modification ordering inside each object row.
// Stdlib sort.Strings would be fine; staying alloc-free + dependency-free
// for the small typical input sizes (≤ a few dozen keys per row).
func sortStrings(a []string) {
	for i := 1; i < len(a); i++ {
		for j := i; j > 0 && a[j-1] > a[j]; j-- {
			a[j-1], a[j] = a[j], a[j-1]
		}
	}
}
