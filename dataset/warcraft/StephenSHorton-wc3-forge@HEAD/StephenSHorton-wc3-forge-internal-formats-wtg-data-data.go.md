---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wtg/data/data.go"
ref: HEAD
---

// Package data embeds a snapshot of the WC3 Trigger Editor vocabulary
// (UI/TriggerData.txt + the en-US TriggerStrings.txt). The wtg parser depends
// on these for argc resolution — see the package-level comment on the wtg
// package for the rationale (argument counts live in the txt file, not the
// .wtg binary itself).
//
// Refresh: rerun cmd/dumptriggerdata after a WC3 patch is suspected to have
// changed the function vocabulary. The Object Editor's own argc table is
// HEAVILY constrained by these files, so a stale snapshot can cause silent
// ECA misalignment when parsing maps authored against a newer World Editor.
//
// The wtg.LoadEmbedded() helper is the convenience wrapper that decodes both
// into a *wtg.TriggerData; tests + production callers (Session.Open) use that.
package data

import _ "embed"

//go:embed triggerdata.txt
var TriggerDataTXT []byte

//go:embed triggerstrings.txt
var TriggerStringsTXT []byte
