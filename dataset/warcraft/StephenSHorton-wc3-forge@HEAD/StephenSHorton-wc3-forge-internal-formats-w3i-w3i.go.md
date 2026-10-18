---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3i/w3i.go"
ref: HEAD
---

// Package w3i parses Warcraft III map info files (war3map.w3i).
//
// The format is a binary, little-endian, version-prefixed blob written by
// the world editor. Modern Reforged maps use file_version 31, 32, or 33;
// classic TFT maps use 25; Frozen Throne early maps used 18; Reign of Chaos
// originals used 15. This parser handles all of those, falling back leniently
// (returning what it has rather than erroring) when a file ends short of the
// variable-length tail sections.
//
// Strings in the .w3i are null-terminated bytes. Modern Reforged Lua maps
// (file_version >= 28 with the LUA flag) are UTF-8. Older maps are typically
// Windows-1252 but most ASCII content overlaps with UTF-8, and editors have
// mixed conventions over the years. We decode as UTF-8 and substitute the
// Unicode replacement character (U+FFFD) for invalid byte sequences so the
// parser never fails on encoding alone.
//
// Coordinate convention: integer fields (lengths, counts, flags, tile counts)
// stay int. World-space positions (camera bounds, player starting positions)
// are float32 in GAME coordinates (origin at map center, units = world units).
// We do NOT convert these to any other coordinate space.
//
// Reference:
//   - HiveWE: src/base/map_info.ixx (canonical C++ reader/writer)
//   - WC3MapTranslator (TypeScript, MIT): cross-reference for field semantics
//   - Eikonium's Lua guide: flag 0x4000 of the player "all_flags" is unrelated
//     to the Lua flag; the actual Lua marker is a dedicated uint32 added at
//     file_version 28 (1 = Lua, 0 = JASS). Both are surfaced separately below.
package w3i

import (
	"encoding/binary"
	"errors"
	"fmt"
	"io"
	"math"
	"unicode/utf8"
)

// Known file_version values produced by Blizzard editors over the years.
const (
	FileVersionRoC      = 15 // Reign of Chaos
	FileVersionTFTEarly = 18 // The Frozen Throne, early
	FileVersionTFT      = 25 // The Frozen Throne, mature
	FileVersionRefV28   = 28 // Reforged: adds 4-part game_version + lua flag
	FileVersionRefV31   = 31 // Reforged: adds enemy priorities + supported_modes / game_data_version
	FileVersionRefV32   = 32 // Reforged: adds default + max cam distance
	FileVersionRefV33   = 33 // Reforged: adds min cam distance
)

// PlayerType mirrors HiveWE's enum (offset by -1 on the wire — see Parse).
type PlayerType int

const (
	PlayerHuman PlayerType = iota
	PlayerComputer
	PlayerNeutral
	PlayerRescuable
)

// PlayerRace mirrors HiveWE's enum (on-wire value used directly).
type PlayerRace int

const (
	RaceSelectable PlayerRace = iota
	RaceHuman
	RaceOrc
	RaceUndead
	RaceNightElf
)

// Vec2 is a pair of float32 in world coordinates.
type Vec2 struct {
	X, Y float32
}

// IVec4 is four int32 — used for unplayable-border tile counts.
type IVec4 struct {
	A, B, C, D int32
}

// Color is a 4-byte BGRA-ish (HiveWE reads it as u8vec4 of fog/water color;
// we surface it verbatim).
type Color struct {
	R, G, B, A uint8
}

// Flags decodes the 32-bit map flags word into named booleans. Bit names and
// "unknown*" carryovers come from HiveWE's reader. Bits past 0x400000 are
// reserved / unused in observed maps.
type Flags struct {
	Raw                                uint32
	HideMinimapPreview                 bool // 0x000001
	ModifyAllyPriorities               bool // 0x000002
	MeleeMap                           bool // 0x000004
	Unknown1                           bool // 0x000008 (large playable size)
	MaskedAreaPartiallyVisible         bool // 0x000010
	FixedPlayerSettings                bool // 0x000020
	CustomForces                       bool // 0x000040
	CustomTechtree                     bool // 0x000080
	CustomAbilities                    bool // 0x000100
	CustomUpgrades                     bool // 0x000200
	Unknown2                           bool // 0x000400 (properties menu opened)
	CliffShoreWaves                    bool // 0x000800
	RollingShoreWaves                  bool // 0x001000
	Unknown3                           bool // 0x002000 (has terrain fog)
	Unknown4                           bool // 0x004000 (requires expansion)
	ItemClassification                 bool // 0x008000
	WaterTinting                       bool // 0x010000
	AccurateProbabilityForCalculations bool // 0x020000
	CustomAbilitySkins                 bool // 0x040000
	DisableDenyIcon                    bool // 0x080000
	ForceDefaultZoom                   bool // 0x100000
	ForceMaxZoom                       bool // 0x200000
	ForceMinZoom                       bool // 0x400000
}

// DecodeFlags re-derives the named-boolean fields of Flags from its Raw bits.
// Exported so callers that mutate Raw (e.g. ApplyInfoUpdates flipping a single
// option-tab checkbox) can keep the named bools in sync without re-implementing
// the bit→name mapping.
func DecodeFlags(raw uint32) Flags { return decodeFlags(raw) }

func decodeFlags(raw uint32) Flags {
	bit := func(mask uint32) bool { return raw&mask != 0 }
	return Flags{
		Raw:                                raw,
		HideMinimapPreview:                 bit(0x000001),
		ModifyAllyPriorities:               bit(0x000002),
		MeleeMap:                           bit(0x000004),
		Unknown1:                           bit(0x000008),
		MaskedAreaPartiallyVisible:         bit(0x000010),
		FixedPlayerSettings:                bit(0x000020),
		CustomForces:                       bit(0x000040),
		CustomTechtree:                     bit(0x000080),
		CustomAbilities:                    bit(0x000100),
		CustomUpgrades:                     bit(0x000200),
		Unknown2:                           bit(0x000400),
		CliffShoreWaves:                    bit(0x000800),
		RollingShoreWaves:                  bit(0x001000),
		Unknown3:                           bit(0x002000),
		Unknown4:                           bit(0x004000),
		ItemClassification:                 bit(0x008000),
		WaterTinting:                       bit(0x010000),
		AccurateProbabilityForCalculations: bit(0x020000),
		CustomAbilitySkins:                 bit(0x040000),
		DisableDenyIcon:                    bit(0x080000),
		ForceDefaultZoom:                   bit(0x100000),
		ForceMaxZoom:                       bit(0x200000),
		ForceMinZoom:                       bit(0x400000),
	}
}

// LoadingScreen carries the per-version loading screen fields. ModelPath is
// only present in v25+ (TFT); v18 has just the campaign number + strings;
// v15 has only an unknown 1-byte index and strings.
type LoadingScreen struct {
	Number   int32
	Model    string // empty when version < 25
	Text     string
	Title    string
	Subtitle string
}

// Prologue is the campaign prologue cutscreen (TFT+). For v18 ModelPath is
// not present; for v15 the prologue area is just 4 ignored bytes and is left
// empty.
type Prologue struct {
	Model    string
	Text     string
	Title    string
	Subtitle string
}

// Fog parameters for the map. Present from v25 onward; zero-valued otherwise.
type Fog struct {
	Style      int32
	StartZ     float32
	EndZ       float32
	Density    float32
	Color      Color
}

// CamDistance bundles default/max/min camera distance. Only Default+Max are
// present at v32; Min added at v33. Zero on older formats.
type CamDistance struct {
	Default uint32
	Max     uint32
	Min     uint32
}

// Player is one of the slots defined in the editor's Player Properties dialog.
type Player struct {
	InternalNumber       int32
	Type                 PlayerType
	Race                 PlayerRace
	FixedStartPosition   int32
	Name                 string
	StartingPosition     Vec2
	AllyLowPriorities    uint32
	AllyHighPriorities   uint32
	EnemyLowPriorities   uint32 // v31+
	EnemyHighPriorities  uint32 // v31+
}

// ForceFlags decodes the per-force flags word.
type ForceFlags struct {
	Raw                       uint32
	Allied                    bool // 0b0000_0001
	AlliedVictory             bool // 0b0000_0010
	ShareVision               bool // 0b0000_1000 (yes — bit 2 is intentionally skipped per HiveWE)
	ShareUnitControl          bool // 0b0001_0000
	ShareAdvancedUnitControl  bool // 0b0010_0000
}

// Force is a player team / force entry.
type Force struct {
	Flags       ForceFlags
	PlayerMasks uint32 // bitfield of which player slots are in this force
	Name        string
}

// UpgradeAvailability — an entry in the per-map upgrade availability table.
type UpgradeAvailability struct {
	PlayerFlags  uint32
	ID           string // 4-char rawcode (e.g. "Rhar")
	Level        int32
	Availability int32 // 0=unavailable, 1=available, 2=researched
}

// TechAvailability — an entry in the per-map tech (ability/unit) availability
// table.
type TechAvailability struct {
	PlayerFlags uint32
	ID          string // 4-char rawcode
}

// RandomUnitLine — one row in a random-unit table; chance then one rawcode
// per column ("position"). The column count comes from the parent table's
// Positions slice (each position is a level/tier integer).
type RandomUnitLine struct {
	Chance int32
	IDs    []string // length == len(parent.Positions)
}

// RandomUnitTable — a CreateRandomUnit table (one of the GUI "set custom
// random unit table" entries).
type RandomUnitTable struct {
	CreationNumber int32
	Name           string
	Positions      []int32 // per-column tier values
	Lines          []RandomUnitLine
}

// RandomItemEntry — chance + rawcode pair from a random item set.
type RandomItemEntry struct {
	Chance int32
	ID     string
}

// RandomItemSet — one set inside a RandomItemTable.
type RandomItemSet struct {
	Items []RandomItemEntry
}

// RandomItemTable — a CreateRandomItem table (TFT v25+ only).
type RandomItemTable struct {
	CreationNumber int32
	Name           string
	ItemSets       []RandomItemSet
}

// Info is the fully-decoded contents of a war3map.w3i file. Fields not
// applicable to the file's version are zero-valued; the FileVersion field
// indicates which subset is meaningful.
type Info struct {
	FileVersion      int32
	MapVersion       int32 // v18+
	EditorVersion    int32 // v18+
	GameVersion      [4]int32 // major, minor, patch, build — v28+

	Name             string
	Author           string
	Description      string
	SuggestedPlayers string

	CameraLeftBottom Vec2
	CameraRightTop   Vec2
	CameraLeftTop    Vec2
	CameraRightBottom Vec2
	CameraComplements IVec4 // unplayable-border tile counts (left, right, bottom, top)

	PlayableWidth  int32
	PlayableHeight int32

	Flags Flags

	Tileset byte // 1-byte tileset code; in older versions this is the entire trailing chunk

	LoadingScreen      LoadingScreen
	GameDataSet        int32 // v25+
	Prologue           Prologue
	Fog                Fog
	WeatherID          int32
	CustomSoundEnv     string
	CustomLightTileset byte
	WaterColor         Color

	// Lua is true if the map's primary script is Lua (file_version >= 28
	// with the dedicated marker word set to 1). For RoC/early-TFT maps
	// this is always false.
	Lua bool

	SupportedModes  uint32 // v31+
	GameDataVersion uint32 // v31+
	CamDistance     CamDistance

	Players []Player
	Forces  []Force

	AvailableUpgrades []UpgradeAvailability
	AvailableTech     []TechAvailability

	RandomUnitTables []RandomUnitTable
	RandomItemTables []RandomItemTable // v25+

	// TruncatedAt is non-nil when the parser stopped early at one of the
	// optional tail sections due to insufficient remaining bytes. This is
	// expected for some old/protected maps and is not an error per se.
	TruncatedAt *string `json:",omitempty"`
}

// ResolveStrings substitutes TRIGSTR_<n> references throughout the Info
// in place, using resolve as the lookup. Callers typically pass
// wts.Strings.Resolve; the resolve func itself is expected to be a no-op
// for non-TRIGSTR inputs (so it's safe to apply over fields that already
// contain literal strings).
//
// resolve may be nil, in which case this is a no-op.
func (i *Info) ResolveStrings(resolve func(string) string) {
	if i == nil || resolve == nil {
		return
	}
	i.Name = resolve(i.Name)
	i.Author = resolve(i.Author)
	i.Description = resolve(i.Description)
	i.SuggestedPlayers = resolve(i.SuggestedPlayers)
	i.CustomSoundEnv = resolve(i.CustomSoundEnv)

	i.LoadingScreen.Model = resolve(i.LoadingScreen.Model)
	i.LoadingScreen.Text = resolve(i.LoadingScreen.Text)
	i.LoadingScreen.Title = resolve(i.LoadingScreen.Title)
	i.LoadingScreen.Subtitle = resolve(i.LoadingScreen.Subtitle)

	i.Prologue.Model = resolve(i.Prologue.Model)
	i.Prologue.Text = resolve(i.Prologue.Text)
	i.Prologue.Title = resolve(i.Prologue.Title)
	i.Prologue.Subtitle = resolve(i.Prologue.Subtitle)

	for k := range i.Players {
		i.Players[k].Name = resolve(i.Players[k].Name)
	}
	for k := range i.Forces {
		i.Forces[k].Name = resolve(i.Forces[k].Name)
	}
	for k := range i.RandomUnitTables {
		i.RandomUnitTables[k].Name = resolve(i.RandomUnitTables[k].Name)
	}
	for k := range i.RandomItemTables {
		i.RandomItemTables[k].Name = resolve(i.RandomItemTables[k].Name)
	}
}

// reader is an internal helper around a fixed byte slice with a cursor.
type reader struct {
	buf []byte
	pos int
}

func (r *reader) remaining() int { return len(r.buf) - r.pos }

// checkCount validates an untrusted element count read from the byte stream
// before it is used to size an allocation. A count is implausible if the bytes
// it would consume (count * minElemBytes) exceed the bytes remaining in the
// buffer — no legitimate file can describe more elements than it has bytes for.
// Rejecting up front turns a malformed/truncated/protected war3map.w3i (which
// is parsed on EVERY map open) into a fast decode error instead of an attempted
// multi-gigabyte allocation that OOM-crashes the process. Same idiom as
// unitsdoo.reader.checkCount / the doodadsdoo sanity caps.
//
// what is used only to build a descriptive, wrapped error matching the
// package's existing truncation-error style.
func (r *reader) checkCount(count uint32, minElemBytes int, what string) error {
	if minElemBytes < 1 {
		minElemBytes = 1
	}
	// remaining()/minElemBytes is an upper bound on how many elements could
	// possibly follow; anything beyond it is corrupt. Using division (rather
	// than count*minElemBytes) avoids any multiply overflow on a hostile count.
	if uint64(count) > uint64(r.remaining()/minElemBytes) {
		return fmt.Errorf("w3i: %s count %d exceeds %d bytes remaining (min %d bytes/element) at offset %d: %w",
			what, count, r.remaining(), minElemBytes, r.pos, io.ErrUnexpectedEOF)
	}
	return nil
}

func (r *reader) need(n int) error {
	if r.pos+n > len(r.buf) {
		return fmt.Errorf("w3i: short read: need %d bytes at offset %d, have %d", n, r.pos, len(r.buf)-r.pos)
	}
	return nil
}

func (r *reader) skip(n int) error {
	if err := r.need(n); err != nil {
		return err
	}
	r.pos += n
	return nil
}

func (r *reader) u32() (uint32, error) {
	if err := r.need(4); err != nil {
		return 0, err
	}
	v := binary.LittleEndian.Uint32(r.buf[r.pos:])
	r.pos += 4
	return v, nil
}

func (r *reader) i32() (int32, error) {
	v, err := r.u32()
	return int32(v), err
}

func (r *reader) f32() (float32, error) {
	v, err := r.u32()
	return math.Float32frombits(v), err
}

func (r *reader) u8() (byte, error) {
	if err := r.need(1); err != nil {
		return 0, err
	}
	b := r.buf[r.pos]
	r.pos++
	return b, nil
}

func (r *reader) vec2() (Vec2, error) {
	x, err := r.f32()
	if err != nil {
		return Vec2{}, err
	}
	y, err := r.f32()
	if err != nil {
		return Vec2{}, err
	}
	return Vec2{X: x, Y: y}, nil
}

func (r *reader) ivec4() (IVec4, error) {
	a, err := r.i32()
	if err != nil {
		return IVec4{}, err
	}
	b, err := r.i32()
	if err != nil {
		return IVec4{}, err
	}
	c, err := r.i32()
	if err != nil {
		return IVec4{}, err
	}
	d, err := r.i32()
	if err != nil {
		return IVec4{}, err
	}
	return IVec4{A: a, B: b, C: c, D: d}, nil
}

func (r *reader) color() (Color, error) {
	if err := r.need(4); err != nil {
		return Color{}, err
	}
	c := Color{
		R: r.buf[r.pos],
		G: r.buf[r.pos+1],
		B: r.buf[r.pos+2],
		A: r.buf[r.pos+3],
	}
	r.pos += 4
	return c, nil
}

// cstr reads a null-terminated string. Per the format, strings are encoded as
// UTF-8 in modern Reforged maps and Windows-1252 in older ones. We decode as
// UTF-8 and use the Unicode replacement character for invalid sequences so
// the parser never aborts on a single odd byte. (The vast majority of map
// metadata is ASCII; mixed-encoding errors only matter for non-English names.)
func (r *reader) cstr() (string, error) {
	start := r.pos
	for {
		if r.pos >= len(r.buf) {
			return "", io.ErrUnexpectedEOF
		}
		if r.buf[r.pos] == 0 {
			raw := r.buf[start:r.pos]
			r.pos++ // consume the null
			return decodeUTF8Lenient(raw), nil
		}
		r.pos++
	}
}

// fixedStr reads exactly n bytes (no terminator). Used for 4-char rawcodes.
func (r *reader) fixedStr(n int) (string, error) {
	if err := r.need(n); err != nil {
		return "", err
	}
	raw := r.buf[r.pos : r.pos+n]
	r.pos += n
	// Rawcodes (e.g. "hfoo", "Hpal") are pure ASCII; no decoding nuance.
	// Trim trailing nulls just in case.
	end := len(raw)
	for end > 0 && raw[end-1] == 0 {
		end--
	}
	return string(raw[:end]), nil
}

// decodeUTF8Lenient returns a string from bytes, substituting U+FFFD for any
// invalid UTF-8 byte. Equivalent to strings.ToValidUTF8 with the replacement
// character, but operating directly on []byte.
func decodeUTF8Lenient(b []byte) string {
	if utf8.Valid(b) {
		return string(b)
	}
	out := make([]rune, 0, len(b))
	for len(b) > 0 {
		r, size := utf8.DecodeRune(b)
		if r == utf8.RuneError && size == 1 {
			out = append(out, '�')
			b = b[1:]
			continue
		}
		out = append(out, r)
		b = b[size:]
	}
	return string(out)
}

// Parse decodes a war3map.w3i blob and returns the populated Info.
//
// The parser is lenient at the tail: per HiveWE's behavior, if the file ends
// before an optional table (upgrades, tech, random unit, random item) we
// return what we have with Info.TruncatedAt set to the section name. Truly
// malformed bytes inside a section still produce an error.
func Parse(data []byte) (*Info, error) {
	if len(data) < 4 {
		return nil, errors.New("w3i: data too short to contain file_version")
	}

	r := &reader{buf: data}
	info := &Info{}

	version, err := r.i32()
	if err != nil {
		return nil, err
	}
	info.FileVersion = version

	// Reject only versions outside the spread we know about — HiveWE warns
	// but proceeds for unknown versions. Match that behavior: we don't
	// hard-error; we just attempt to read the v18+ shape.
	if version < FileVersionRoC {
		return nil, fmt.Errorf("w3i: implausibly small file_version %d", version)
	}

	if version >= FileVersionTFTEarly {
		if info.MapVersion, err = r.i32(); err != nil {
			return nil, err
		}
		if info.EditorVersion, err = r.i32(); err != nil {
			return nil, err
		}
		if version >= FileVersionRefV28 {
			for i := 0; i < 4; i++ {
				if info.GameVersion[i], err = r.i32(); err != nil {
					return nil, err
				}
			}
		}
	}

	if info.Name, err = r.cstr(); err != nil {
		return nil, err
	}
	if info.Author, err = r.cstr(); err != nil {
		return nil, err
	}
	if info.Description, err = r.cstr(); err != nil {
		return nil, err
	}
	if info.SuggestedPlayers, err = r.cstr(); err != nil {
		return nil, err
	}

	if info.CameraLeftBottom, err = r.vec2(); err != nil {
		return nil, err
	}
	if info.CameraRightTop, err = r.vec2(); err != nil {
		return nil, err
	}
	if info.CameraLeftTop, err = r.vec2(); err != nil {
		return nil, err
	}
	if info.CameraRightBottom, err = r.vec2(); err != nil {
		return nil, err
	}
	if info.CameraComplements, err = r.ivec4(); err != nil {
		return nil, err
	}

	if info.PlayableWidth, err = r.i32(); err != nil {
		return nil, err
	}
	if info.PlayableHeight, err = r.i32(); err != nil {
		return nil, err
	}

	rawFlags, err := r.u32()
	if err != nil {
		return nil, err
	}
	info.Flags = decodeFlags(rawFlags)

	// One-byte tileset code follows the flags. HiveWE just advances past it
	// (it's also stored in war3map.w3e) but we surface the byte.
	if info.Tileset, err = r.u8(); err != nil {
		return nil, err
	}

	switch {
	case version >= FileVersionTFT:
		if info.LoadingScreen.Number, err = r.i32(); err != nil {
			return nil, err
		}
		if info.LoadingScreen.Model, err = r.cstr(); err != nil {
			return nil, err
		}
		if info.LoadingScreen.Text, err = r.cstr(); err != nil {
			return nil, err
		}
		if info.LoadingScreen.Title, err = r.cstr(); err != nil {
			return nil, err
		}
		if info.LoadingScreen.Subtitle, err = r.cstr(); err != nil {
			return nil, err
		}
		if info.GameDataSet, err = r.i32(); err != nil {
			return nil, err
		}
		if info.Prologue.Model, err = r.cstr(); err != nil {
			return nil, err
		}
		if info.Prologue.Text, err = r.cstr(); err != nil {
			return nil, err
		}
		if info.Prologue.Title, err = r.cstr(); err != nil {
			return nil, err
		}
		if info.Prologue.Subtitle, err = r.cstr(); err != nil {
			return nil, err
		}
		if info.Fog.Style, err = r.i32(); err != nil {
			return nil, err
		}
		if info.Fog.StartZ, err = r.f32(); err != nil {
			return nil, err
		}
		if info.Fog.EndZ, err = r.f32(); err != nil {
			return nil, err
		}
		if info.Fog.Density, err = r.f32(); err != nil {
			return nil, err
		}
		if info.Fog.Color, err = r.color(); err != nil {
			return nil, err
		}
		if info.WeatherID, err = r.i32(); err != nil {
			return nil, err
		}
		if info.CustomSoundEnv, err = r.cstr(); err != nil {
			return nil, err
		}
		if info.CustomLightTileset, err = r.u8(); err != nil {
			return nil, err
		}
		if info.WaterColor, err = r.color(); err != nil {
			return nil, err
		}
		if version >= FileVersionRefV28 {
			luaWord, err := r.u32()
			if err != nil {
				return nil, err
			}
			info.Lua = luaWord == 1
		}
		if version >= FileVersionRefV31 {
			if info.SupportedModes, err = r.u32(); err != nil {
				return nil, err
			}
			if info.GameDataVersion, err = r.u32(); err != nil {
				return nil, err
			}
		}
		if version >= FileVersionRefV32 {
			if info.CamDistance.Default, err = r.u32(); err != nil {
				return nil, err
			}
			if info.CamDistance.Max, err = r.u32(); err != nil {
				return nil, err
			}
			if version >= FileVersionRefV33 {
				if info.CamDistance.Min, err = r.u32(); err != nil {
					return nil, err
				}
			}
		}
	case version == FileVersionTFTEarly: // v18 RoC
		if info.LoadingScreen.Number, err = r.i32(); err != nil {
			return nil, err
		}
		if info.LoadingScreen.Text, err = r.cstr(); err != nil {
			return nil, err
		}
		if info.LoadingScreen.Title, err = r.cstr(); err != nil {
			return nil, err
		}
		if info.LoadingScreen.Subtitle, err = r.cstr(); err != nil {
			return nil, err
		}
		// RoC stores a 4-byte field here that HiveWE treats as unknown
		// (TODO in upstream). Skip per upstream.
		if err = r.skip(4); err != nil {
			return nil, err
		}
		if info.Prologue.Text, err = r.cstr(); err != nil {
			return nil, err
		}
		if info.Prologue.Title, err = r.cstr(); err != nil {
			return nil, err
		}
		if info.Prologue.Subtitle, err = r.cstr(); err != nil {
			return nil, err
		}
	default: // v15 and other early formats
		// Single-byte unknown (probably loading screen number, single
		// digit) followed by three c-strings and a 4-byte unknown.
		if err = r.skip(1); err != nil {
			return nil, err
		}
		if info.LoadingScreen.Text, err = r.cstr(); err != nil {
			return nil, err
		}
		if info.LoadingScreen.Title, err = r.cstr(); err != nil {
			return nil, err
		}
		if info.LoadingScreen.Subtitle, err = r.cstr(); err != nil {
			return nil, err
		}
		if err = r.skip(4); err != nil {
			return nil, err
		}
	}

	// Players.
	playerCount, err := r.u32()
	if err != nil {
		return nil, err
	}
	// A Player consumes at minimum 4 i32s + an (at least 1-byte) name string +
	// a vec2 + 2 u32s = 33 bytes on the wire; bound before allocating so a
	// hostile count can't OOM. (v31+ players are larger; the smaller bound is
	// intentionally conservative — it never rejects a valid file.)
	if err = r.checkCount(playerCount, 33, "player"); err != nil {
		return nil, err
	}
	info.Players = make([]Player, 0, playerCount)
	for i := uint32(0); i < playerCount; i++ {
		var p Player
		if p.InternalNumber, err = r.i32(); err != nil {
			return nil, err
		}
		// HiveWE stores the on-wire value minus one. The on-wire encoding
		// is 1-based (1=human, 2=computer, 3=neutral, 4=rescuable).
		wireType, err := r.i32()
		if err != nil {
			return nil, err
		}
		p.Type = PlayerType(wireType - 1)
		race, err := r.i32()
		if err != nil {
			return nil, err
		}
		p.Race = PlayerRace(race)
		if p.FixedStartPosition, err = r.i32(); err != nil {
			return nil, err
		}
		if p.Name, err = r.cstr(); err != nil {
			return nil, err
		}
		if p.StartingPosition, err = r.vec2(); err != nil {
			return nil, err
		}
		if p.AllyLowPriorities, err = r.u32(); err != nil {
			return nil, err
		}
		if p.AllyHighPriorities, err = r.u32(); err != nil {
			return nil, err
		}
		if version >= FileVersionRefV31 {
			if p.EnemyLowPriorities, err = r.u32(); err != nil {
				return nil, err
			}
			if p.EnemyHighPriorities, err = r.u32(); err != nil {
				return nil, err
			}
		}
		info.Players = append(info.Players, p)
	}

	// Forces.
	forceCount, err := r.u32()
	if err != nil {
		return nil, err
	}
	// A Force consumes at minimum two u32s + an (at least 1-byte) name = 9 bytes.
	if err = r.checkCount(forceCount, 9, "force"); err != nil {
		return nil, err
	}
	info.Forces = make([]Force, 0, forceCount)
	for i := uint32(0); i < forceCount; i++ {
		var f Force
		raw, err := r.u32()
		if err != nil {
			return nil, err
		}
		f.Flags = ForceFlags{
			Raw:                      raw,
			Allied:                   raw&0b0000_0001 != 0,
			AlliedVictory:            raw&0b0000_0010 != 0,
			ShareVision:              raw&0b0000_1000 != 0,
			ShareUnitControl:         raw&0b0001_0000 != 0,
			ShareAdvancedUnitControl: raw&0b0010_0000 != 0,
		}
		if f.PlayerMasks, err = r.u32(); err != nil {
			return nil, err
		}
		if f.Name, err = r.cstr(); err != nil {
			return nil, err
		}
		info.Forces = append(info.Forces, f)
	}

	// Lenient tail: each of the following four sections is optional. If
	// fewer than 4 bytes remain we stop and record where.
	mark := func(name string) {
		if info.TruncatedAt == nil {
			n := name
			info.TruncatedAt = &n
		}
	}

	if r.remaining() < 4 {
		mark("upgrades")
		return info, nil
	}
	upgradeCount, err := r.u32()
	if err != nil {
		return nil, err
	}
	// Each UpgradeAvailability is a fixed 16 bytes (u32 + 4-char id + 2 i32s).
	if err = r.checkCount(upgradeCount, 16, "upgrade"); err != nil {
		return nil, err
	}
	info.AvailableUpgrades = make([]UpgradeAvailability, 0, upgradeCount)
	for i := uint32(0); i < upgradeCount; i++ {
		var u UpgradeAvailability
		if u.PlayerFlags, err = r.u32(); err != nil {
			return nil, err
		}
		if u.ID, err = r.fixedStr(4); err != nil {
			return nil, err
		}
		if u.Level, err = r.i32(); err != nil {
			return nil, err
		}
		if u.Availability, err = r.i32(); err != nil {
			return nil, err
		}
		info.AvailableUpgrades = append(info.AvailableUpgrades, u)
	}

	if r.remaining() < 4 {
		mark("tech")
		return info, nil
	}
	techCount, err := r.u32()
	if err != nil {
		return nil, err
	}
	// Each TechAvailability is a fixed 8 bytes (u32 player flags + 4-char id).
	if err = r.checkCount(techCount, 8, "tech"); err != nil {
		return nil, err
	}
	info.AvailableTech = make([]TechAvailability, 0, techCount)
	for i := uint32(0); i < techCount; i++ {
		var t TechAvailability
		if t.PlayerFlags, err = r.u32(); err != nil {
			return nil, err
		}
		if t.ID, err = r.fixedStr(4); err != nil {
			return nil, err
		}
		info.AvailableTech = append(info.AvailableTech, t)
	}

	if r.remaining() < 4 {
		mark("random_unit_tables")
		return info, nil
	}
	rutCount, err := r.u32()
	if err != nil {
		return nil, err
	}
	// A RandomUnitTable consumes at minimum CreationNumber(4) + name(>=1) +
	// posCount(4) + lineCount(4) = 13 bytes.
	if err = r.checkCount(rutCount, 13, "random_unit_table"); err != nil {
		return nil, err
	}
	info.RandomUnitTables = make([]RandomUnitTable, 0, rutCount)
	for i := uint32(0); i < rutCount; i++ {
		var t RandomUnitTable
		if t.CreationNumber, err = r.i32(); err != nil {
			return nil, err
		}
		if t.Name, err = r.cstr(); err != nil {
			return nil, err
		}
		posCount, err := r.u32()
		if err != nil {
			return nil, err
		}
		// Each position is a 4-byte i32.
		if err = r.checkCount(posCount, 4, "random_unit_table position"); err != nil {
			return nil, err
		}
		t.Positions = make([]int32, posCount)
		for j := uint32(0); j < posCount; j++ {
			if t.Positions[j], err = r.i32(); err != nil {
				return nil, err
			}
		}
		lineCount, err := r.u32()
		if err != nil {
			return nil, err
		}
		// Each line consumes at minimum a 4-byte chance (posCount may be 0).
		if err = r.checkCount(lineCount, 4, "random_unit_table line"); err != nil {
			return nil, err
		}
		t.Lines = make([]RandomUnitLine, lineCount)
		for j := uint32(0); j < lineCount; j++ {
			if t.Lines[j].Chance, err = r.i32(); err != nil {
				return nil, err
			}
			t.Lines[j].IDs = make([]string, 0, posCount)
			for k := uint32(0); k < posCount; k++ {
				id, err := r.fixedStr(4)
				if err != nil {
					return nil, err
				}
				t.Lines[j].IDs = append(t.Lines[j].IDs, id)
			}
		}
		info.RandomUnitTables = append(info.RandomUnitTables, t)
	}

	if version < FileVersionTFT {
		return info, nil
	}

	if r.remaining() < 4 {
		mark("random_item_tables")
		return info, nil
	}
	ritCount, err := r.u32()
	if err != nil {
		return nil, err
	}
	// A RandomItemTable consumes at minimum CreationNumber(4) + name(>=1) +
	// setCount(4) = 9 bytes.
	if err = r.checkCount(ritCount, 9, "random_item_table"); err != nil {
		return nil, err
	}
	info.RandomItemTables = make([]RandomItemTable, 0, ritCount)
	for i := uint32(0); i < ritCount; i++ {
		var t RandomItemTable
		if t.CreationNumber, err = r.i32(); err != nil {
			return nil, err
		}
		if t.Name, err = r.cstr(); err != nil {
			return nil, err
		}
		setCount, err := r.u32()
		if err != nil {
			return nil, err
		}
		// Each set consumes at minimum a 4-byte itemCount prefix.
		if err = r.checkCount(setCount, 4, "random_item_table set"); err != nil {
			return nil, err
		}
		t.ItemSets = make([]RandomItemSet, setCount)
		for j := uint32(0); j < setCount; j++ {
			itemCount, err := r.u32()
			if err != nil {
				return nil, err
			}
			// Each RandomItemEntry is a fixed 8 bytes (i32 chance + 4-char id).
			if err = r.checkCount(itemCount, 8, "random_item_table entry"); err != nil {
				return nil, err
			}
			t.ItemSets[j].Items = make([]RandomItemEntry, itemCount)
			for k := uint32(0); k < itemCount; k++ {
				if t.ItemSets[j].Items[k].Chance, err = r.i32(); err != nil {
					return nil, err
				}
				if t.ItemSets[j].Items[k].ID, err = r.fixedStr(4); err != nil {
					return nil, err
				}
			}
		}
		info.RandomItemTables = append(info.RandomItemTables, t)
	}

	return info, nil
}
