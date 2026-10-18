---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wtg/wtg.go"
ref: HEAD
---

// Package wtg parses war3map.wtg (the GUI trigger tree). Read-only in Phase
// 1a; the encode path lands in Phase 2a alongside structural editing.
//
// Wire format: see HiveWE's src/base/triggers/triggers.ixx L546-906 — this
// package is a direct port of `load_version_31` (1.31+ format) and
// `load_version_pre31` (legacy). The hardest part is that ECA/Parameter
// argument counts don't live in the .wtg binary — they're computed from
// UI/TriggerData.txt. The caller MUST supply an ArgumentCounts table; see
// data.go for the embedded snapshot we ship.
//
// Hand-rolled-script maps (no .wtg, all in war3map.j/lua) are handled at the
// Session layer, not here: this package only knows about the .wtg binary
// format. The Session synthesizes a "Map Header" script trigger for those
// maps from the script source.
package wtg

import (
	"bytes"
	"encoding/binary"
	"errors"
	"fmt"
	"io"
	"log"
	"math"
)

// Classifier is the 1.31+ element-kind discriminator. Values are the bit-flag
// set HiveWE uses (1=map, 2=library, 4=category, 8=gui, 16=comment, 32=script,
// 64=variable). The pre-1.31 fallback infers these from is_script /
// run_on_initialization / is_comment booleans.
type Classifier uint32

const (
	ClassifierMap      Classifier = 1
	ClassifierLibrary  Classifier = 2
	ClassifierCategory Classifier = 4
	ClassifierGUI      Classifier = 8
	ClassifierComment  Classifier = 16
	ClassifierScript   Classifier = 32
	ClassifierVariable Classifier = 64
)

// ECAType is the event-condition-action-call discriminator for each row in a
// trigger's ECA list. Stored as a u32 on disk.
type ECAType uint32

const (
	ECAEvent     ECAType = 0
	ECACondition ECAType = 1
	ECAAction    ECAType = 2
	ECACall      ECAType = 3
)

// ParamType is the parameter-flavor discriminator. -1 means "invalid /
// unset" which the editor draws as an empty slot the user must fill.
type ParamType int32

const (
	ParamInvalid  ParamType = -1
	ParamPreset   ParamType = 0
	ParamVariable ParamType = 1
	ParamFunction ParamType = 2
	ParamString   ParamType = 3
)

// Parameter is one argument slot of an ECA. The Value field always carries
// the on-disk string (variable name, preset enum tag, literal text), even
// when HasSubParameter is true — HiveWE preserves the bytes verbatim there
// because they "sometimes contain garbage" (triggers.ixx L356) but must be
// round-trip-byte-equal. Phase 2a's encoder will read it back.
//
// SubParameter holds the nested function call when HasSubParameter is true
// (e.g. a CreateUnit's player slot pointing at GetTriggerPlayer()). ArrayIndex
// holds the index expression when IsArray is true (variables only).
type Parameter struct {
	Type            ParamType  `json:"type"`
	Value           string     `json:"value"`
	HasSubParameter bool       `json:"has_sub_parameter,omitempty"`
	SubParameter    *ECA       `json:"sub_parameter,omitempty"`
	Unknown         uint32     `json:"unknown,omitempty"` // sub_version 7 only, when HasSubParameter
	IsArray         bool       `json:"is_array,omitempty"`
	ArrayIndex      *Parameter `json:"array_index,omitempty"`

	// ResolvedDisplay is a UI-only enrichment set by the Session layer's
	// triggers.get path — never read by Parse and never written by Encode.
	// Carries the human-readable display string for codegen-generated
	// references like gg_unit_Hpal_0001 → "Paladin (1)". Empty when Value
	// is not a recognized entity reference or the entity can't be found.
	//
	// MUST stay out of the byte-level round-trip path: Phase 2a's Encode
	// reads Value verbatim. This field is allowed to lag/desync after a
	// session mutates the underlying entity (the next triggers.get re-resolves).
	ResolvedDisplay string `json:"resolved_display,omitempty"`
}

// ECA is a single Event / Condition / Action / Call row inside a trigger.
// Group is only meaningful for nested children (is_child=true in HiveWE).
// Children is the nested rows that hang off magic ECAs like IfThenElseMultiple,
// ForLoopAMultiple, AndMultiple — see triggers.ixx L351-365 for the synthesized
// "If / Then / Else / Loop" UI labels the Trigger Editor pane uses.
//
// HasParameters is meaningful ONLY for sub-parameter ECAs (the inner ECA in
// Parameter.SubParameter). The on-disk format has a separate u32 flag that
// indicates whether parameter slots follow — distinct from the parameter
// count, which lives in argc. Real-world maps (Enfo FFB) sometimes ship
// HasParameters=true with argc[name]=0 (e.g. GetTriggerPlayer with an empty
// parameter list), which the encoder must echo verbatim for byte equality.
// For top-level ECAs (which always carry argc[name] parameter slots),
// HasParameters is unused.
type ECA struct {
	Type          ECAType     `json:"type"`
	Group         uint32      `json:"group,omitempty"` // only when nested
	Name          string      `json:"name"`
	Enabled       bool        `json:"enabled"`
	Parameters    []Parameter `json:"parameters,omitempty"`
	Children      []ECA       `json:"children,omitempty"`
	HasParameters bool        `json:"has_parameters,omitempty"` // SubParameter only — preserved for byte round-trip
}

// Trigger is one GUI / script / comment row in the tree. Custom-script and
// comment kinds use the CustomText field instead of ECAs. ParentID points
// at the owning Category (or at the synthetic Map Header / Variables
// category for the pre-1.31 fallback).
//
// InitiallyOn is stored INVERTED on disk (HiveWE writes !initially_on at
// triggers.ixx L889). The Parse here un-inverts it so consumers see the
// human-meaningful value; the encoder MUST re-invert (Phase 2a concern).
type Trigger struct {
	Classifier          Classifier `json:"classifier"`
	ID                  int32      `json:"id"`
	ParentID            int32      `json:"parent_id"`
	Name                string     `json:"name"`
	Description         string     `json:"description,omitempty"`
	CustomText          string     `json:"custom_text,omitempty"` // populated from .wct after parse
	IsComment           bool       `json:"is_comment,omitempty"`
	IsEnabled           bool       `json:"is_enabled"`
	IsScript            bool       `json:"is_script,omitempty"`
	InitiallyOn         bool       `json:"initially_on"`
	RunOnInitialization bool       `json:"run_on_initialization,omitempty"`
	ECAs                []ECA      `json:"ecas,omitempty"`
}

// Category is a folder in the tree. The Map Header is always id=0 + a magic
// Classifier::map entry. Library is rarely used (Reforged dropped per-library
// authoring in the UI; the parser still accepts it).
type Category struct {
	Classifier Classifier `json:"classifier"`
	ID         int32      `json:"id"`
	ParentID   int32      `json:"parent_id"`
	Name       string     `json:"name"`
	OpenState  bool       `json:"open_state"`
	IsComment  bool       `json:"is_comment,omitempty"`
}

// Variable is a global trigger variable (udg_*). Type is the WC3 type name
// ("integer", "unit", "abilcode", …) — keyed against TriggerTypes in
// TriggerData.txt.
type Variable struct {
	Name          string `json:"name"`
	Type          string `json:"type"`
	Unknown       uint32 `json:"unknown,omitempty"`
	IsArray       bool   `json:"is_array,omitempty"`
	ArraySize     int32  `json:"array_size,omitempty"` // sub_version 7 only
	IsInitialized bool   `json:"is_initialized,omitempty"`
	InitialValue  string `json:"initial_value,omitempty"`
	ID            int32  `json:"id"`
	ParentID      int32  `json:"parent_id"`
}

// Triggers is the parsed war3map.wtg as a whole. Categories + Variables +
// Triggers are kept as flat slices in the order they appeared in the source
// file; the UI builds the tree by walking ParentID.
type Triggers struct {
	// On-disk header fields. Unknown1/Unknown2 are always 0 in practice;
	// TrigDefVer is 2 in HiveWE's writer. Preserved here so the encoder
	// round-trips byte-equal.
	Unknown1   uint32 `json:"unknown1"`
	Unknown2   uint32 `json:"unknown2"`
	TrigDefVer uint32 `json:"trig_def_ver"`
	Version    uint32 `json:"version"`
	SubVersion uint32 `json:"sub_version"`

	// IsPre131 is true when the .wtg used the legacy load_version_pre31
	// format (no Classifier-byte discriminator on elements; categories /
	// variables / triggers are flat sequential blocks). The encoder picks
	// the right writer based on this flag + Version/SubVersion.
	IsPre131 bool `json:"is_pre_131,omitempty"`

	// DeletedIDs preserves the seven (map/library/category/trigger/comment/
	// script/variable) deleted-id arrays that prefix the 1.31+ format.
	// HiveWE skips them at parse and emits empty arrays at encode; we
	// preserve them verbatim so round-trip-byte-equal holds for maps that
	// happen to carry non-empty deletion lists. Always length 7. Each entry
	// is the raw ID slice from disk (length matches the on-disk count2).
	//
	// The first `count` field is preserved separately in DeletedCounts —
	// real-world maps (e.g. Enfo FFB) sometimes ship count != count2 (the
	// stored count is a phantom from World Editor's deletion-tracker state
	// while count2 is the actual ID-array length). Encoder must echo both
	// fields verbatim for byte equality.
	//
	// Convention: nil entries are written as empty (count=0, count2=0); the
	// length-7 outer slice is initialized by Parse on every 1.31+ read.
	DeletedIDs    [7][]int32 `json:"deleted_ids,omitempty"`
	DeletedCounts [7]uint32  `json:"deleted_counts,omitempty"`

	// PreCategoryUnknown preserves the 4-byte "unknown / dunno" field that
	// sits between the categories block and the variables block in the
	// pre-1.31 format (HiveWE: triggers.ixx loadVersionPre31, the
	// `reader.advance(4)` after the category loop). Captured verbatim so
	// the pre-1.31 encoder can round-trip byte-equal.
	PreCategoryUnknown uint32 `json:"pre_category_unknown,omitempty"`

	// PreWctUnknown preserves the same 4-byte "unknown" field that
	// pre-1.31 wct files carry between the global JASS header and the
	// per-trigger blob array. Lives in the *wct.File, not here — but the
	// pre-1.31 wtg has a similar pre-trigger unknown the encoder needs to
	// re-emit. Currently unused; reserved for future fidelity work.
	PreWctUnknown uint32 `json:"-"`

	Categories []Category `json:"categories,omitempty"`
	Variables  []Variable `json:"variables,omitempty"`
	Triggers   []Trigger  `json:"triggers,omitempty"`

	// Elements preserves the on-disk element-block order. The 1.31+ format
	// interleaves categories, triggers, and variable-refs in arbitrary order
	// per the World Editor's authoring history; for round-trip byte equality
	// the encoder must re-emit them in the same order. Each entry is a
	// (kind, index-into-its-slice) pair.
	//
	// For categories/triggers, the index points into Categories/Triggers.
	// For variable-refs (legacy duplicate-variable rows the parser skips
	// per HiveWE L692-697 commentary), the entry carries the raw payload
	// directly since we don't need to model them as first-class entities.
	//
	// Mutators that ADD a category/trigger/variable must also append an
	// entry to Elements so the new element appears in the saved file.
	// DeleteTriggerNode must remove the corresponding Elements entry.
	Elements []ElementRef `json:"-"`
}

// ElementKind discriminates the 1.31+ element-block row kind.
type ElementKind uint8

const (
	ElementKindCategory ElementKind = iota
	ElementKindTrigger
	ElementKindVariableRef // legacy duplicate-variable header; raw bytes preserved
)

// ElementRef points at one entry of the 1.31+ element block. For Category
// and Trigger kinds, Index is the position in Categories/Triggers. For
// VariableRef kind, RawID + RawName + RawParentID hold the verbatim payload
// the parser skipped (a 4-byte id, a cstr name, a 4-byte parent_id).
type ElementRef struct {
	Kind        ElementKind
	Index       int    // into Categories or Triggers
	RawID       uint32 // VariableRef only
	RawName     string // VariableRef only
	RawParentID uint32 // VariableRef only
}

const wtgMagic = "WTG!"

// ErrUnknownECAName is returned when ECA parsing encounters a function name
// whose argc isn't in the supplied TriggerData. The caller should surface this
// (and not retry on the same map) — the wtg is structurally readable but the
// schema disagreement means every subsequent ECA in the file would silently
// misalign. The error wraps a tip pointing at the TriggerData snapshot.
var ErrUnknownECAName = errors.New("wtg: ECA function name not in TriggerData (snapshot may be out of sync)")

// Parse decodes war3map.wtg bytes. argc supplies the per-function-name
// argument count table (built from UI/TriggerData.txt via ParseTriggerData)
// — the wtg binary doesn't carry argc itself, so this is mandatory.
//
// Unknown 1.31+ sub-versions log a warning and fall through to the standard
// 1.31 loader (mirrors HiveWE L588-591). Pre-1.31 (version 4 or 7 at the
// top) takes the legacy path.
func Parse(data []byte, argc map[string]int) (*Triggers, error) {
	if argc == nil {
		return nil, fmt.Errorf("wtg.Parse: argc map is required")
	}
	r := newReader(data)
	magic := r.readBytes(4)
	if r.err != nil {
		return nil, fmt.Errorf("wtg.Parse: read magic: %w", r.err)
	}
	t := &Triggers{}

	if string(magic) == wtgMagic {
		t.Version = r.readU32()
		if r.err != nil {
			return nil, r.err
		}
		switch t.Version {
		case 0x80000004:
			if err := loadVersion31(r, t, argc); err != nil {
				return nil, err
			}
			return t, nil
		default:
			log.Printf("wtg: unknown version 0x%X — trying 1.31 loader", t.Version)
			if err := loadVersion31(r, t, argc); err != nil {
				return nil, err
			}
			return t, nil
		}
	}

	// No WTG! magic — the bytes we just read were the pre-1.31 version field
	// (a u32 in LE). Re-interpret.
	t.Version = binary.LittleEndian.Uint32(magic)
	if t.Version == 4 || t.Version == 7 {
		t.IsPre131 = true
		if err := loadVersionPre31(r, t, argc); err != nil {
			return nil, err
		}
		return t, nil
	}
	return nil, fmt.Errorf("wtg.Parse: unrecognized header (magic=%q, first u32=0x%X)", magic, t.Version)
}

// loadVersion31 parses the 1.31+ format (HiveWE triggers.ixx L594-700).
func loadVersion31(r *reader, t *Triggers, argc map[string]int) error {
	t.SubVersion = r.readU32()
	if t.SubVersion != 7 && t.SubVersion != 4 {
		log.Printf("wtg: unknown 1.31 sub-version %d — trying anyway", t.SubVersion)
	}

	// Seven deleted-id blocks: each is (u32 count, u32 count2, u32 ids[count2]).
	// HiveWE skips them. We preserve them verbatim so the Phase 2a encoder
	// can round-trip byte-equal: maps that ship non-empty deletion lists
	// (rare but legal) would otherwise diff on every save.
	//
	// Real-world maps sometimes have count != count2 (Enfo FFB: count=1,
	// count2=0). Both are preserved; the encoder re-emits both verbatim.
	for i := 0; i < 7; i++ {
		t.DeletedCounts[i] = r.readU32()
		n := r.readU32()
		if r.err != nil {
			return fmt.Errorf("wtg: deleted block %d header: %w", i, r.err)
		}
		if n == 0 {
			t.DeletedIDs[i] = nil
			continue
		}
		// Each id is a 4-byte u32; bound before allocating so a hostile count
		// can't OOM.
		if err := r.checkCount(n, 4, "deleted block id"); err != nil {
			return fmt.Errorf("wtg: deleted block %d body: %w", i, err)
		}
		ids := make([]int32, n)
		for j := uint32(0); j < n; j++ {
			ids[j] = int32(r.readU32())
		}
		if r.err != nil {
			return fmt.Errorf("wtg: deleted block %d body: %w", i, r.err)
		}
		t.DeletedIDs[i] = ids
	}

	t.Unknown1 = r.readU32()
	t.Unknown2 = r.readU32()
	t.TrigDefVer = r.readU32()

	variableCount := r.readU32()
	if r.err != nil {
		return fmt.Errorf("wtg: read variable_count: %w", r.err)
	}
	for i := uint32(0); i < variableCount; i++ {
		v := Variable{}
		v.Name = r.readCString()
		v.Type = r.readCString()
		v.Unknown = r.readU32()
		v.IsArray = r.readU32() != 0
		if t.SubVersion == 7 {
			v.ArraySize = int32(r.readU32())
		}
		v.IsInitialized = r.readU32() != 0
		v.InitialValue = r.readCString()
		v.ID = int32(r.readU32())
		v.ParentID = int32(r.readU32())
		if r.err != nil {
			return fmt.Errorf("wtg: parse variable %d: %w", i, r.err)
		}
		t.Variables = append(t.Variables, v)
	}

	elementCount := r.readU32()
	if r.err != nil {
		return fmt.Errorf("wtg: read element_count: %w", r.err)
	}
	// Each element consumes at minimum a 4-byte classifier u32; bound the
	// freshly-read count before preallocating so a hostile value can't OOM.
	if err := r.checkCount(elementCount, 4, "element"); err != nil {
		return fmt.Errorf("wtg: %w", err)
	}
	t.Elements = make([]ElementRef, 0, elementCount)
	for i := uint32(0); i < elementCount; i++ {
		classifier := Classifier(r.readU32())
		if r.err != nil {
			return fmt.Errorf("wtg: read element %d classifier: %w", i, r.err)
		}
		switch classifier {
		case ClassifierMap, ClassifierLibrary, ClassifierCategory:
			c := Category{Classifier: classifier}
			c.ID = int32(r.readU32())
			c.Name = r.readCString()
			if t.SubVersion == 7 {
				c.IsComment = r.readU32() != 0
			}
			c.OpenState = r.readU32() != 0
			c.ParentID = int32(r.readU32())
			if r.err != nil {
				return fmt.Errorf("wtg: parse category %d: %w", i, r.err)
			}
			t.Elements = append(t.Elements, ElementRef{Kind: ElementKindCategory, Index: len(t.Categories)})
			t.Categories = append(t.Categories, c)
		case ClassifierGUI, ClassifierComment, ClassifierScript:
			tr := Trigger{Classifier: classifier}
			tr.Name = r.readCString()
			tr.Description = r.readCString()
			if t.SubVersion == 7 {
				tr.IsComment = r.readU32() != 0
			}
			tr.ID = int32(r.readU32())
			tr.IsEnabled = r.readU32() != 0
			tr.IsScript = r.readU32() != 0
			// On-disk InitiallyOn is INVERTED; un-flip here so consumers see truth.
			tr.InitiallyOn = r.readU32() == 0
			tr.RunOnInitialization = r.readU32() != 0
			tr.ParentID = int32(r.readU32())
			ecaCount := r.readU32()
			if r.err != nil {
				return fmt.Errorf("wtg: parse trigger %d header: %w", i, r.err)
			}
			// Each ECA consumes at minimum a 4-byte type + (>=1-byte) name +
			// 4-byte enabled = 9 bytes; bound before allocating.
			if err := r.checkCount(ecaCount, 9, "ECA"); err != nil {
				return fmt.Errorf("wtg: parse trigger %d: %w", i, err)
			}
			tr.ECAs = make([]ECA, ecaCount)
			for j := uint32(0); j < ecaCount; j++ {
				if err := parseECA(r, &tr.ECAs[j], false, t.SubVersion, argc); err != nil {
					return fmt.Errorf("wtg: parse trigger %q ECA %d: %w", tr.Name, j, err)
				}
			}
			t.Elements = append(t.Elements, ElementRef{Kind: ElementKindTrigger, Index: len(t.Triggers)})
			t.Triggers = append(t.Triggers, tr)
		case ClassifierVariable:
			// Per HiveWE L692-697: a duplicate variable header — the real
			// variables already came in the dedicated section above. We
			// preserve the raw payload (id + name + parent_id) so the
			// encoder round-trips byte-equal for maps that ship them.
			rawID := r.readU32()
			rawName := r.readCString()
			rawParent := r.readU32()
			if r.err != nil {
				return fmt.Errorf("wtg: read variable-ref %d: %w", i, r.err)
			}
			t.Elements = append(t.Elements, ElementRef{
				Kind: ElementKindVariableRef, RawID: rawID, RawName: rawName, RawParentID: rawParent,
			})
		default:
			return fmt.Errorf("wtg: unknown classifier 0x%X at element %d", classifier, i)
		}
	}
	return nil
}

// loadVersionPre31 parses the legacy format (HiveWE L702-776). Categories,
// variables, and triggers are flat sequential blocks (no classifier byte);
// we synthesize a Map Header + Variables category at the front so the UI
// tree-walk works the same way as the 1.31 path.
func loadVersionPre31(r *reader, t *Triggers, argc map[string]int) error {
	catCount := r.readU32()
	if r.err != nil {
		return fmt.Errorf("wtg(pre-31): read category count: %w", r.err)
	}
	// Each category consumes at minimum a 4-byte id + (>=1-byte) name = 5 bytes;
	// bound the freshly-read count before preallocating so a hostile value
	// can't OOM. (+2 accounts for the synthetic Map Header + Variables rows
	// prepended below.)
	if err := r.checkCount(catCount, 5, "category"); err != nil {
		return fmt.Errorf("wtg(pre-31): %w", err)
	}
	maxID := int32(0)
	t.Categories = make([]Category, 0, int(catCount)+2)
	for i := uint32(0); i < catCount; i++ {
		c := Category{Classifier: ClassifierCategory}
		c.ID = int32(r.readU32())
		c.Name = r.readCString()
		c.ParentID = 0
		if t.Version == 7 {
			c.IsComment = r.readU32() != 0
		}
		if r.err != nil {
			return fmt.Errorf("wtg(pre-31): parse category %d: %w", i, r.err)
		}
		if c.ID+1 > maxID {
			maxID = c.ID + 1
		}
		// HiveWE: if (i.id == 0) i.id = -2  (avoid colliding with synthetic Map Header at 0)
		if c.ID == 0 {
			c.ID = -2
		}
		t.Categories = append(t.Categories, c)
	}

	// Pre-31 has a 4-byte "unknown" field between the categories block and
	// the variables block. HiveWE skips it on read; we capture it so the
	// pre-31 encoder can re-emit verbatim for round-trip equality.
	t.PreCategoryUnknown = r.readU32()
	if r.err != nil {
		return fmt.Errorf("wtg(pre-31): read unknown after categories: %w", r.err)
	}

	variableCategory := maxID
	maxID++

	// Prepend the synthetic Map Header (id=0) and Variables category.
	header := []Category{
		{Classifier: ClassifierMap, ID: 0, ParentID: -1, Name: "Map Header", OpenState: true},
		{Classifier: ClassifierCategory, ID: variableCategory, ParentID: 0, Name: "Variables", OpenState: true},
	}
	t.Categories = append(header, t.Categories...)

	varCount := r.readU32()
	if r.err != nil {
		return fmt.Errorf("wtg(pre-31): read var count: %w", r.err)
	}
	// A Variable consumes at minimum name(>=1) + type(>=1) + unknown(4) +
	// is_array(4) + is_initialized(4) + initial_value(>=1) + parent_id... but
	// the smallest contiguous wire footprint is conservatively >= 4 bytes;
	// bound before allocating.
	if err := r.checkCount(varCount, 4, "variable"); err != nil {
		return fmt.Errorf("wtg(pre-31): %w", err)
	}
	t.Variables = make([]Variable, 0, varCount)
	for i := uint32(0); i < varCount; i++ {
		v := Variable{}
		v.Name = r.readCString()
		v.Type = r.readCString()
		v.Unknown = r.readU32()
		v.ID = maxID
		maxID++
		v.IsArray = r.readU32() != 0
		if t.Version == 7 {
			v.ArraySize = int32(r.readU32())
		}
		v.IsInitialized = r.readU32() != 0
		v.InitialValue = r.readCString()
		v.ParentID = variableCategory
		if r.err != nil {
			return fmt.Errorf("wtg(pre-31): parse variable %d: %w", i, r.err)
		}
		t.Variables = append(t.Variables, v)
	}

	trigCount := r.readU32()
	if r.err != nil {
		return fmt.Errorf("wtg(pre-31): read trigger count: %w", r.err)
	}
	// A Trigger consumes at minimum name(>=1) + description(>=1) + several
	// fixed u32s; conservatively >= 4 bytes. Bound before allocating.
	if err := r.checkCount(trigCount, 4, "trigger"); err != nil {
		return fmt.Errorf("wtg(pre-31): %w", err)
	}
	t.Triggers = make([]Trigger, 0, trigCount)
	for i := uint32(0); i < trigCount; i++ {
		tr := Trigger{}
		tr.Name = r.readCString()
		tr.Description = r.readCString()
		if t.Version == 7 {
			tr.IsComment = r.readU32() != 0
		}
		tr.IsEnabled = r.readU32() != 0
		tr.IsScript = r.readU32() != 0
		tr.InitiallyOn = r.readU32() == 0 // INVERTED on disk
		tr.RunOnInitialization = r.readU32() != 0
		tr.ID = maxID
		maxID++

		// Classifier inference per HiveWE L757-765.
		switch {
		case tr.RunOnInitialization && tr.IsScript:
			tr.Classifier = ClassifierGUI
		case tr.IsComment:
			tr.Classifier = ClassifierComment
		case tr.IsScript:
			tr.Classifier = ClassifierScript
		default:
			tr.Classifier = ClassifierGUI
		}

		tr.ParentID = int32(r.readU32())
		if tr.ParentID == 0 {
			tr.ParentID = -2 // HiveWE: same id=0 collision avoidance as above
		}
		ecaCount := r.readU32()
		if r.err != nil {
			return fmt.Errorf("wtg(pre-31): parse trigger %d header: %w", i, r.err)
		}
		// Each ECA consumes at minimum a 4-byte type + (>=1-byte) name +
		// 4-byte enabled = 9 bytes; bound before allocating.
		if err := r.checkCount(ecaCount, 9, "ECA"); err != nil {
			return fmt.Errorf("wtg(pre-31): parse trigger %d: %w", i, err)
		}
		tr.ECAs = make([]ECA, ecaCount)
		for j := uint32(0); j < ecaCount; j++ {
			if err := parseECA(r, &tr.ECAs[j], false, t.Version, argc); err != nil {
				return fmt.Errorf("wtg(pre-31): parse trigger %q ECA %d: %w", tr.Name, j, err)
			}
		}
		t.Triggers = append(t.Triggers, tr)
	}
	return nil
}

// parseECA decodes one ECA. version is the SUB_version (4 or 7) for the
// 1.31+ format or the version (4 or 7) for the pre-1.31 format — the disk
// layout is the same either way at the ECA level.
func parseECA(r *reader, eca *ECA, isChild bool, version uint32, argc map[string]int) error {
	eca.Type = ECAType(r.readU32())
	if isChild {
		eca.Group = r.readU32()
	}
	eca.Name = r.readCString()
	eca.Enabled = r.readU32() != 0
	if r.err != nil {
		return r.err
	}
	n, ok := argc[eca.Name]
	if !ok {
		return fmt.Errorf("%w: name=%q", ErrUnknownECAName, eca.Name)
	}
	eca.Parameters = make([]Parameter, n)
	for i := 0; i < n; i++ {
		if err := parseParameter(r, &eca.Parameters[i], version, argc); err != nil {
			return fmt.Errorf("parameter %d of %q: %w", i, eca.Name, err)
		}
	}
	if version == 7 {
		childCount := r.readU32()
		if r.err != nil {
			return r.err
		}
		// Each child ECA consumes at minimum type(4) + group(4) + name(>=1) +
		// enabled(4) = 13 bytes; bound before allocating.
		if err := r.checkCount(childCount, 13, "child ECA"); err != nil {
			return fmt.Errorf("children of %q: %w", eca.Name, err)
		}
		eca.Children = make([]ECA, childCount)
		for j := uint32(0); j < childCount; j++ {
			if err := parseECA(r, &eca.Children[j], true, version, argc); err != nil {
				return fmt.Errorf("child %d of %q: %w", j, eca.Name, err)
			}
		}
	}
	return nil
}

// parseParameter decodes one parameter slot — the gnarly bit. The wire form
// (HiveWE parse_parameter_structure L452-485):
//   u32 type
//   cstr value
//   u32 has_sub_parameter
//   if has_sub_parameter:
//     u32 sub_type ; cstr sub_name ; u32 has_parameters ; (recursive params)
//   if version == 4:
//     if type == function: u32 always_zero
//     else: u32 is_array
//   else (version == 7):
//     if has_sub_parameter: u32 unknown
//     u32 is_array
//   if is_array:
//     one nested Parameter (the index expression)
//
// We preserve Value bytes verbatim — even when HasSubParameter is true the
// Phase 2a encoder needs them back for byte-for-byte round-trip (HiveWE
// comment at triggers.ixx L356).
func parseParameter(r *reader, p *Parameter, version uint32, argc map[string]int) error {
	p.Type = ParamType(int32(r.readU32()))
	p.Value = r.readCString()
	p.HasSubParameter = r.readU32() != 0
	if r.err != nil {
		return r.err
	}
	if p.HasSubParameter {
		sub := &ECA{}
		sub.Type = ECAType(r.readU32())
		sub.Group = 0
		sub.Name = r.readCString()
		sub.Enabled = true
		sub.HasParameters = r.readU32() != 0
		if r.err != nil {
			return r.err
		}
		if sub.HasParameters {
			n, ok := argc[sub.Name]
			if !ok {
				return fmt.Errorf("%w: sub-parameter name=%q", ErrUnknownECAName, sub.Name)
			}
			sub.Parameters = make([]Parameter, n)
			for i := 0; i < n; i++ {
				if err := parseParameter(r, &sub.Parameters[i], version, argc); err != nil {
					return fmt.Errorf("sub-parameter %d of %q: %w", i, sub.Name, err)
				}
			}
		}
		p.SubParameter = sub
	}
	if version == 4 {
		if p.Type == ParamFunction {
			r.advance(4) // unknown, always 0
		} else {
			p.IsArray = r.readU32() != 0
		}
	} else {
		if p.HasSubParameter {
			p.Unknown = r.readU32()
		}
		p.IsArray = r.readU32() != 0
	}
	if r.err != nil {
		return r.err
	}
	if p.IsArray {
		p.ArrayIndex = &Parameter{}
		if err := parseParameter(r, p.ArrayIndex, version, argc); err != nil {
			return fmt.Errorf("array-index parameter: %w", err)
		}
	}
	return nil
}

// reader is a tiny binary-reader that mirrors HiveWE's BinaryReader API
// (read_u32 / read_c_string / advance) but accumulates the first I/O error
// into r.err so call-sites stay flat. Once r.err is non-nil, every subsequent
// read returns a zero value without advancing — safe to defer error-check to
// the end of a chunk.
type reader struct {
	buf []byte
	off int
	err error
}

func newReader(buf []byte) *reader {
	return &reader{buf: buf}
}

// remaining reports how many unread bytes are left in the buffer.
func (r *reader) remaining() int {
	if r.off >= len(r.buf) {
		return 0
	}
	return len(r.buf) - r.off
}

// checkCount validates an untrusted element count freshly read from the byte
// stream before it is used to size an allocation. A count is implausible if the
// bytes it would consume (count * minElemBytes) exceed the bytes remaining — no
// legitimate file can describe more elements than it has bytes for. Rejecting up
// front turns a malformed/truncated/protected war3map.wtg (parsed on every
// GUI-trigger map open) into a fast decode error instead of an attempted
// multi-gigabyte allocation that OOM-crashes the process. Same idiom as
// unitsdoo.reader.checkCount.
//
// On a sticky prior error this is a no-op (the count is meaningless 0); callers
// already gate on r.err before calling, but this keeps the helper composable.
// On rejection it sets r.err (mirroring this reader's sticky-error convention)
// AND returns the error so the call-site can wrap it with context.
func (r *reader) checkCount(count uint32, minElemBytes int, what string) error {
	if r.err != nil {
		return r.err
	}
	if minElemBytes < 1 {
		minElemBytes = 1
	}
	// remaining()/minElemBytes is an upper bound on how many elements could
	// possibly follow; anything beyond it is corrupt. Division (rather than
	// count*minElemBytes) avoids any multiply overflow on a hostile count.
	if uint64(count) > uint64(r.remaining()/minElemBytes) {
		r.err = io.ErrUnexpectedEOF
		return fmt.Errorf("wtg: %s count %d exceeds %d bytes remaining (min %d bytes/element) at offset %d: %w",
			what, count, r.remaining(), minElemBytes, r.off, io.ErrUnexpectedEOF)
	}
	return nil
}

func (r *reader) advance(n int64) {
	if r.err != nil {
		return
	}
	if int64(r.off)+n > int64(len(r.buf)) {
		r.err = io.ErrUnexpectedEOF
		return
	}
	r.off += int(n)
}

func (r *reader) readU32() uint32 {
	if r.err != nil {
		return 0
	}
	if r.off+4 > len(r.buf) {
		r.err = io.ErrUnexpectedEOF
		return 0
	}
	v := binary.LittleEndian.Uint32(r.buf[r.off:])
	r.off += 4
	return v
}

// readF32 is unused in Phase 1a but kept for the encoder/decoder symmetry
// future variable initializers may need (TriggerData declares real-typed
// defaults that flow through Parameter.Value as text today — no float on
// the wire — but the helper costs nothing).
func (r *reader) readF32() float32 {
	return math.Float32frombits(r.readU32())
}

func (r *reader) readBytes(n int) []byte {
	if r.err != nil {
		return nil
	}
	if r.off+n > len(r.buf) {
		r.err = io.ErrUnexpectedEOF
		return nil
	}
	out := r.buf[r.off : r.off+n]
	r.off += n
	return out
}

// readCString reads a null-terminated string. Returns "" when the read would
// go past the buffer (and sets r.err).
func (r *reader) readCString() string {
	if r.err != nil {
		return ""
	}
	end := bytes.IndexByte(r.buf[r.off:], 0)
	if end < 0 {
		r.err = io.ErrUnexpectedEOF
		return ""
	}
	s := string(r.buf[r.off : r.off+end])
	r.off += end + 1
	return s
}
