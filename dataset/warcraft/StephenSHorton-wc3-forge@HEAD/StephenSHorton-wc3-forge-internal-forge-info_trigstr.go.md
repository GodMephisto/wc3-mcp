---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/info_trigstr.go"
ref: HEAD
---

package forge

import (
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3i"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wts"
)

// info_trigstr.go keeps localized maps' Map Info string fields coherent with
// war3map.wts. On a localized map the w3i Description fields are stored as
// "TRIGSTR_<n>" tokens whose display text lives in war3map.wts; Session.Open
// resolves them in place (info.Name becomes the display value), which LOSES the
// token. Editing such a field then wrote the literal display string into w3i,
// orphaning the wts entry — and war3map.wts was never re-written at all.
//
// The fix, kept entirely session-side (no w3i.Info struct change):
//   - At Open, BEFORE ResolveStrings, capture the original token per field
//     (captureInfoTokens → Session.infoTokens).
//   - On edit, update the referenced wts entry instead of orphaning it
//     (SetMapInfo, undo-tracked via setMapInfoCmd's wts deltas).
//   - At Save, re-inject the tokens into a shallow Info copy so w3i.Encode writes
//     the TOKEN (reinjectInfoTokens), and write war3map.wts when dirtyStrings.
//
// Only the four editable Description fields are handled — they're the ones
// map.info_set touches and the common TRIGSTR-backed metadata. Other
// potentially-tokenized fields (loading-screen/prologue text, player/force
// names) aren't edited through this path, so leaving them resolved-to-literal on
// save matches the prior behavior for those.

// infoStringFields are the TRIGSTR-backed Description fields, keyed by the same
// wire key ApplyInfoUpdates uses, with accessors so capture + re-inject share
// one source of truth.
var infoStringFields = []struct {
	key string
	get func(*w3i.Info) string
	set func(*w3i.Info, string)
}{
	{"name", func(i *w3i.Info) string { return i.Name }, func(i *w3i.Info, v string) { i.Name = v }},
	{"author", func(i *w3i.Info) string { return i.Author }, func(i *w3i.Info, v string) { i.Author = v }},
	{"description", func(i *w3i.Info) string { return i.Description }, func(i *w3i.Info, v string) { i.Description = v }},
	{"suggestedPlayers", func(i *w3i.Info) string { return i.SuggestedPlayers }, func(i *w3i.Info, v string) { i.SuggestedPlayers = v }},
}

// captureInfoTokens records the original TRIGSTR token of each Description field
// that carries one, BEFORE the field is resolved to its display value. Returns
// nil for a non-localized map (no tokenized fields). Call before ResolveStrings.
func captureInfoTokens(info *w3i.Info) map[string]string {
	if info == nil {
		return nil
	}
	var m map[string]string
	for _, f := range infoStringFields {
		v := f.get(info)
		if _, ok := wts.RefID(v); ok {
			if m == nil {
				m = make(map[string]string, len(infoStringFields))
			}
			m[f.key] = v
		}
	}
	return m
}

// reinjectInfoTokens returns info unchanged when there are no tokens, otherwise a
// SHALLOW COPY with each tokenized Description field set back to its TRIGSTR
// token so w3i.Encode emits the token (keeping the wts reference) rather than the
// resolved literal. The copy shares Info's slices (Players/Forces/…) — we only
// overwrite the four scalar string fields, so that's safe.
func reinjectInfoTokens(info *w3i.Info, tokens map[string]string) *w3i.Info {
	if info == nil || len(tokens) == 0 {
		return info
	}
	cp := *info
	for _, f := range infoStringFields {
		if tok, ok := tokens[f.key]; ok {
			f.set(&cp, tok)
		}
	}
	return &cp
}

// computeInfoStringDeltaLocked builds the wts-entry before/after maps for the
// tokenized Description fields edited by `updates` (the new value is the display
// string ApplyInfoUpdates just wrote). Empty maps when nothing tokenized
// changed. Caller holds s.mu.
func (s *Session) computeInfoStringDeltaLocked(updates map[string]any) (before, after map[uint32]string) {
	if len(s.infoTokens) == 0 {
		return nil, nil
	}
	for key, tok := range s.infoTokens {
		v, ok := updates[key].(string)
		if !ok {
			continue
		}
		id, ok := wts.RefID(tok)
		if !ok {
			continue
		}
		var cur string
		if s.strings != nil {
			cur = s.strings[id]
		}
		if cur == v {
			continue
		}
		if before == nil {
			before = make(map[uint32]string)
			after = make(map[uint32]string)
		}
		before[id] = cur
		after[id] = v
	}
	return before, after
}
