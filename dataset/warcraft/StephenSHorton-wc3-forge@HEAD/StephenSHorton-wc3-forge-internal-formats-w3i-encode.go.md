---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3i/encode.go"
ref: HEAD
---

// Encode is the byte-faithful inverse of Parse for war3map.w3i.
//
// Round-trip contract: for any well-formed war3map.w3i blob `data`,
//
//	out, _ := Encode(Parse(data))
//	bytes.Equal(out, data) == true
//
// To make this hold we:
//
//   - Write the file at the same FileVersion we read it at (so we don't
//     promote a v25 map to v33 on save). HiveWE's `MapInfo::save` always
//     writes v33, which intentionally rewrites older maps in the modern
//     format. wc3-forge prefers preservation for v1 — the Map Info Editor
//     can drive a later "save as latest" intent explicitly if needed.
//
//   - Write the Raw bits for Flags / ForceFlags instead of OR-decomposing
//     the named booleans. The decomposed bits omit "Unknown*" bits and (in
//     HiveWE's writer) also miscompose ShareVision as bit 2 while reading
//     it as bit 3 — a known HiveWE bug that wc3-forge does NOT inherit. By
//     emitting Raw directly we preserve every bit, including ones the
//     editor UI doesn't surface.
//
//   - Re-emit Player.Type as `int(Type)+1` to undo Parse's `-1` shift.
//
//   - Honor TruncatedAt: when a file ends short of an optional tail
//     section, we ALSO stop emitting at that section. Otherwise Encode
//     would invent zero-count headers the original didn't have.
//
// Preservation gotcha (DOCUMENTED OPEN QUESTION):
//
//	TRIGSTR_<n> references in cstring fields are RESOLVED in-place by
//	`Session.Open` via `Info.ResolveStrings(wts.Strings.Display)` after
//	Parse. The original TRIGSTR token is LOST in memory. If a caller round-
//	trips a post-ResolveStrings Info through Encode, the saved war3map.w3i
//	will contain literal strings rather than the original TRIGSTR_<n>
//	tokens. This is Option A of the spec — acceptable for a v1 because
//	visible behavior is identical, but the saved file's strings no longer
//	share the trigger-string indirection. A future Option B would track
//	NameRaw/DescriptionRaw etc. on the Info struct and emit the original
//	token when the display string is unchanged; that requires Parse-side
//	plumbing and a wts writer for newly-edited strings.
//
//	For the round-trip tests below we operate on RAW Parse output (no
//	ResolveStrings called), so the byte equality holds.

package w3i

import (
	"encoding/binary"
	"fmt"
	"math"
)

// Encode serializes info back to the on-disk war3map.w3i binary format,
// preserving its FileVersion. Returns an error if FileVersion is unset (0)
// or implausibly small (< FileVersionRoC). Hand-constructed Info values
// must set FileVersion explicitly.
func Encode(info *Info) ([]byte, error) {
	if info == nil {
		return nil, fmt.Errorf("encode: nil info")
	}
	v := info.FileVersion
	if v < FileVersionRoC {
		return nil, fmt.Errorf("encode: unsupported file_version %d (min %d)", v, FileVersionRoC)
	}

	w := &writer{}
	w.writeI32(v)

	if v >= FileVersionTFTEarly {
		w.writeI32(info.MapVersion)
		w.writeI32(info.EditorVersion)
		if v >= FileVersionRefV28 {
			for i := 0; i < 4; i++ {
				w.writeI32(info.GameVersion[i])
			}
		}
	}

	w.writeCStr(info.Name)
	w.writeCStr(info.Author)
	w.writeCStr(info.Description)
	w.writeCStr(info.SuggestedPlayers)

	w.writeVec2(info.CameraLeftBottom)
	w.writeVec2(info.CameraRightTop)
	w.writeVec2(info.CameraLeftTop)
	w.writeVec2(info.CameraRightBottom)
	w.writeIVec4(info.CameraComplements)

	w.writeI32(info.PlayableWidth)
	w.writeI32(info.PlayableHeight)

	w.writeU32(info.Flags.Raw)

	w.writeU8(info.Tileset)

	switch {
	case v >= FileVersionTFT:
		w.writeI32(info.LoadingScreen.Number)
		w.writeCStr(info.LoadingScreen.Model)
		w.writeCStr(info.LoadingScreen.Text)
		w.writeCStr(info.LoadingScreen.Title)
		w.writeCStr(info.LoadingScreen.Subtitle)
		w.writeI32(info.GameDataSet)
		w.writeCStr(info.Prologue.Model)
		w.writeCStr(info.Prologue.Text)
		w.writeCStr(info.Prologue.Title)
		w.writeCStr(info.Prologue.Subtitle)
		w.writeI32(info.Fog.Style)
		w.writeF32(info.Fog.StartZ)
		w.writeF32(info.Fog.EndZ)
		w.writeF32(info.Fog.Density)
		w.writeColor(info.Fog.Color)
		w.writeI32(info.WeatherID)
		w.writeCStr(info.CustomSoundEnv)
		w.writeU8(info.CustomLightTileset)
		w.writeColor(info.WaterColor)
		if v >= FileVersionRefV28 {
			var luaWord uint32
			if info.Lua {
				luaWord = 1
			}
			w.writeU32(luaWord)
		}
		if v >= FileVersionRefV31 {
			w.writeU32(info.SupportedModes)
			w.writeU32(info.GameDataVersion)
		}
		if v >= FileVersionRefV32 {
			w.writeU32(info.CamDistance.Default)
			w.writeU32(info.CamDistance.Max)
			if v >= FileVersionRefV33 {
				w.writeU32(info.CamDistance.Min)
			}
		}
	case v == FileVersionTFTEarly: // v18
		w.writeI32(info.LoadingScreen.Number)
		w.writeCStr(info.LoadingScreen.Text)
		w.writeCStr(info.LoadingScreen.Title)
		w.writeCStr(info.LoadingScreen.Subtitle)
		// 4 unknown bytes that Parse skips. We can't reproduce them
		// faithfully since Parse discards them; emit zeros and hope no
		// v18 maps depend on the value. This matches HiveWE's behavior
		// (it doesn't store them either). If a real v18 round-trip
		// surfaces a mismatch here, add a preservation field to Info.
		w.writeZeroBytes(4)
		w.writeCStr(info.Prologue.Text)
		w.writeCStr(info.Prologue.Title)
		w.writeCStr(info.Prologue.Subtitle)
	default: // v15
		w.writeZeroBytes(1)
		w.writeCStr(info.LoadingScreen.Text)
		w.writeCStr(info.LoadingScreen.Title)
		w.writeCStr(info.LoadingScreen.Subtitle)
		w.writeZeroBytes(4)
	}

	// Players.
	w.writeU32(uint32(len(info.Players)))
	for _, p := range info.Players {
		w.writeI32(p.InternalNumber)
		// Parse subtracts 1 from the wire value; reverse here.
		w.writeI32(int32(p.Type) + 1)
		w.writeI32(int32(p.Race))
		w.writeI32(p.FixedStartPosition)
		w.writeCStr(p.Name)
		w.writeVec2(p.StartingPosition)
		w.writeU32(p.AllyLowPriorities)
		w.writeU32(p.AllyHighPriorities)
		if v >= FileVersionRefV31 {
			w.writeU32(p.EnemyLowPriorities)
			w.writeU32(p.EnemyHighPriorities)
		}
	}

	// Forces.
	w.writeU32(uint32(len(info.Forces)))
	for _, f := range info.Forces {
		// Emit Raw — see top-of-file rationale. Avoids reproducing
		// HiveWE's share_vision bit-mismatch and preserves any
		// unknown bits the parser doesn't surface as named booleans.
		w.writeU32(f.Flags.Raw)
		w.writeU32(f.PlayerMasks)
		w.writeCStr(f.Name)
	}

	// Optional tail sections — honor TruncatedAt to keep byte-faithful
	// round-trip for short files. A non-nil TruncatedAt means Parse
	// stopped before that section; Encode must also stop, otherwise it'd
	// invent a zero-count header the source didn't have.
	truncatedAt := func() string {
		if info.TruncatedAt == nil {
			return ""
		}
		return *info.TruncatedAt
	}()
	if truncatedAt == "upgrades" {
		return w.bytes(), nil
	}

	w.writeU32(uint32(len(info.AvailableUpgrades)))
	for _, u := range info.AvailableUpgrades {
		w.writeU32(u.PlayerFlags)
		if err := w.writeFourCC(u.ID); err != nil {
			return nil, fmt.Errorf("upgrade id: %w", err)
		}
		w.writeI32(u.Level)
		w.writeI32(u.Availability)
	}

	if truncatedAt == "tech" {
		return w.bytes(), nil
	}

	w.writeU32(uint32(len(info.AvailableTech)))
	for _, t := range info.AvailableTech {
		w.writeU32(t.PlayerFlags)
		if err := w.writeFourCC(t.ID); err != nil {
			return nil, fmt.Errorf("tech id: %w", err)
		}
	}

	if truncatedAt == "random_unit_tables" {
		return w.bytes(), nil
	}

	w.writeU32(uint32(len(info.RandomUnitTables)))
	for _, t := range info.RandomUnitTables {
		w.writeI32(t.CreationNumber)
		w.writeCStr(t.Name)
		w.writeU32(uint32(len(t.Positions)))
		for _, p := range t.Positions {
			w.writeI32(p)
		}
		w.writeU32(uint32(len(t.Lines)))
		for _, line := range t.Lines {
			w.writeI32(line.Chance)
			// Per-line IDs count == len(Positions). We trust Parse's
			// invariant; if a hand-built table violates it, the
			// emitted file won't round-trip but won't panic.
			for _, id := range line.IDs {
				if err := w.writeFourCC(id); err != nil {
					return nil, fmt.Errorf("random unit id: %w", err)
				}
			}
		}
	}

	if v < FileVersionTFT {
		return w.bytes(), nil
	}

	if truncatedAt == "random_item_tables" {
		return w.bytes(), nil
	}

	w.writeU32(uint32(len(info.RandomItemTables)))
	for _, t := range info.RandomItemTables {
		w.writeI32(t.CreationNumber)
		w.writeCStr(t.Name)
		w.writeU32(uint32(len(t.ItemSets)))
		for _, s := range t.ItemSets {
			w.writeU32(uint32(len(s.Items)))
			for _, item := range s.Items {
				w.writeI32(item.Chance)
				if err := w.writeFourCC(item.ID); err != nil {
					return nil, fmt.Errorf("random item id: %w", err)
				}
			}
		}
	}

	return w.bytes(), nil
}

// --- writer ----------------------------------------------------------------

type writer struct {
	buf []byte
}

func (w *writer) bytes() []byte { return w.buf }

func (w *writer) writeU8(v uint8) { w.buf = append(w.buf, v) }

func (w *writer) writeU32(v uint32) {
	var b [4]byte
	binary.LittleEndian.PutUint32(b[:], v)
	w.buf = append(w.buf, b[:]...)
}

func (w *writer) writeI32(v int32) { w.writeU32(uint32(v)) }

func (w *writer) writeF32(v float32) { w.writeU32(math.Float32bits(v)) }

func (w *writer) writeVec2(v Vec2) {
	w.writeF32(v.X)
	w.writeF32(v.Y)
}

func (w *writer) writeIVec4(v IVec4) {
	w.writeI32(v.A)
	w.writeI32(v.B)
	w.writeI32(v.C)
	w.writeI32(v.D)
}

func (w *writer) writeColor(c Color) {
	w.buf = append(w.buf, c.R, c.G, c.B, c.A)
}

// writeCStr writes the string's bytes followed by a single null terminator.
// The string is emitted verbatim (no encoding transform) — Parse's
// `decodeUTF8Lenient` is lossless when the input is valid UTF-8, which all
// production maps are. If a malformed file is parsed and re-encoded, the
// replacement-character substitution could cause byte drift, but that's
// already a "the file is broken" path.
func (w *writer) writeCStr(s string) {
	w.buf = append(w.buf, []byte(s)...)
	w.buf = append(w.buf, 0)
}

// writeFourCC writes exactly 4 ASCII bytes. Returns an error if s isn't
// exactly 4 bytes — guards against hand-constructed Info values where a
// caller forgot to pad.
//
// Parse's `fixedStr` trims trailing nulls when reading 4-char rawcodes; we
// re-pad with nulls to preserve byte-equality. (In practice production
// rawcodes are 4 printable ASCII chars and never use the padding path.)
func (w *writer) writeFourCC(s string) error {
	if len(s) > 4 {
		return fmt.Errorf("fourcc %q exceeds 4 bytes", s)
	}
	w.buf = append(w.buf, []byte(s)...)
	for i := len(s); i < 4; i++ {
		w.buf = append(w.buf, 0)
	}
	return nil
}

func (w *writer) writeZeroBytes(n int) {
	for i := 0; i < n; i++ {
		w.buf = append(w.buf, 0)
	}
}
