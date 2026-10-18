---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/triggers_resolve_test.go"
ref: HEAD
---

package forge

import (
	"strings"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/doodadsdoo"
	"github.com/StephenSHorton/wc3-forge/internal/formats/unitsdoo"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wtg"
)

// TestResolveEntityName_NonReference verifies the resolver returns "" for
// strings that aren't gg_*_ codegen references. This is the load-bearing
// branch: every Parameter.Value passes through here, and the no-match case
// MUST be cheap + side-effect-free.
func TestResolveEntityName_NonReference(t *testing.T) {
	s := &Session{} // empty session — no map loaded
	cases := []string{
		"",                  // empty
		"udg_PlayerScore",   // user variable
		"GetTriggerUnit()",  // function name
		"5.0",               // literal
		"\"hello world\"",   // string literal
		"Player01_DEFEAT",   // preset enum
		"gg_unitX_Hpal_001", // looks close but no _
	}
	for _, raw := range cases {
		got := resolveEntityName(s, raw)
		if got != "" {
			t.Errorf("resolveEntityName(%q) = %q; want \"\"", raw, got)
		}
	}
}

// TestResolveEntityName_NoMapLoaded confirms the resolver gracefully returns
// "" when no map is loaded (s.Units() is nil) instead of panicking.
func TestResolveEntityName_NoMapLoaded(t *testing.T) {
	s := &Session{}
	if got := resolveEntityName(s, "gg_unit_Hpal_0001"); got != "" {
		t.Errorf("got %q; want \"\" (no map loaded)", got)
	}
	if got := resolveEntityName(s, "gg_dest_LTlt_0042"); got != "" {
		t.Errorf("got %q; want \"\" (no map loaded)", got)
	}
}

// TestResolveEntityName_RegionCamera covers the deferred-parser path: gg_rct_
// + gg_cam_ references log once and return "". The log line gives Phase 1c a
// signal for which maps need region/camera parsers most.
func TestResolveEntityName_RegionCamera(t *testing.T) {
	s := &Session{}
	if got := resolveEntityName(s, "gg_rct_StartZone"); got != "" {
		t.Errorf("gg_rct_ got %q; want \"\" (no w3r parser yet)", got)
	}
	if got := resolveEntityName(s, "gg_cam_OpeningCam"); got != "" {
		t.Errorf("gg_cam_ got %q; want \"\" (no w3c parser yet)", got)
	}
}

// TestResolveEntityName_MissingEntity covers the "reference is well-formed but
// no entity with that creation_number exists" path. We populate a Session
// with a unit at creation_number 7 and ask for #99 — must fall back to "".
func TestResolveEntityName_MissingEntity(t *testing.T) {
	s := &Session{
		loaded: true,
		units: &unitsdoo.File{
			Entities: []unitsdoo.Entity{
				{CreationNumber: 7, TypeID: "Hpal"},
			},
		},
		doodads: &doodadsdoo.File{
			Doodads: []doodadsdoo.Doodad{
				{CreationNumber: 7, TypeID: "LTlt"},
			},
		},
	}
	// CN=99 isn't in the session — empty result.
	if got := resolveEntityName(s, "gg_unit_Hpal_0099"); got != "" {
		t.Errorf("missing unit: got %q; want \"\"", got)
	}
	if got := resolveEntityName(s, "gg_dest_LTlt_0099"); got != "" {
		t.Errorf("missing dest: got %q; want \"\"", got)
	}
}

// TestEnrichTrigger_DeepCopy verifies enrichTriggerWithResolvedNames returns a
// COPY — mutating the result must not mutate the original. This is the
// load-bearing race-safety guarantee: handleTriggersGet shares the cached
// *wtg.Trigger across concurrent callers.
func TestEnrichTrigger_DeepCopy(t *testing.T) {
	original := &wtg.Trigger{
		Name: "T",
		ECAs: []wtg.ECA{
			{
				Name: "DoSomething",
				Parameters: []wtg.Parameter{
					{Value: "gg_unit_Hpal_0001"},
				},
			},
		},
	}
	s := &Session{}
	enriched := enrichTriggerWithResolvedNames(s, original)
	// Mutate the copy's parameter; original must be untouched.
	enriched.ECAs[0].Parameters[0].ResolvedDisplay = "Synthetic Display"
	if original.ECAs[0].Parameters[0].ResolvedDisplay != "" {
		t.Errorf("mutation leaked into original: %q",
			original.ECAs[0].Parameters[0].ResolvedDisplay)
	}
}

// TestEnrichTrigger_NoECAs short-circuits cheap for script/comment triggers
// (no Parameters to walk). Returns the original pointer unchanged.
func TestEnrichTrigger_NoECAs(t *testing.T) {
	tr := &wtg.Trigger{Name: "ScriptOnly", CustomText: "function foo()"}
	s := &Session{}
	got := enrichTriggerWithResolvedNames(s, tr)
	if got != tr {
		t.Errorf("expected same pointer for no-ECA trigger; got copy")
	}
}

// TestEnrichTrigger_NestedSubParameter exercises the recursion into
// sub-parameters (CreateUnit's player slot pointing at GetTriggerPlayer()).
// The sub-call's parameters should also get walked for gg_*_ resolution.
func TestEnrichTrigger_NestedSubParameter(t *testing.T) {
	original := &wtg.Trigger{
		ECAs: []wtg.ECA{
			{
				Name: "DoSomething",
				Parameters: []wtg.Parameter{
					{
						HasSubParameter: true,
						SubParameter: &wtg.ECA{
							Name: "OrderUnitTarget",
							Parameters: []wtg.Parameter{
								{Value: "gg_unit_Hfoo_0042"}, // inner reference
							},
						},
					},
				},
			},
		},
	}
	s := &Session{}
	enriched := enrichTriggerWithResolvedNames(s, original)
	// Verify the sub-parameter array is a copy (different backing array).
	enrichedSubParams := enriched.ECAs[0].Parameters[0].SubParameter.Parameters
	originalSubParams := original.ECAs[0].Parameters[0].SubParameter.Parameters
	if &enrichedSubParams[0] == &originalSubParams[0] {
		t.Errorf("sub-parameter array NOT deep-copied — race-unsafe")
	}
}

// TestGgRefRegex sanity-checks the regex against the four reference shapes +
// a few negatives. Documents the expected pattern shape for future readers.
func TestGgRefRegex(t *testing.T) {
	pos := []string{
		"gg_unit_Hpal_0001",
		"gg_dest_LTlt_0042",
		"gg_rct_StartZone",
		"gg_cam_OpeningCam",
		"gg_rct_With Spaces", // region names can carry spaces — regex must accept
	}
	for _, s := range pos {
		if !strings.HasPrefix(s, "gg_") {
			continue
		}
		if m := ggRefRE.FindStringSubmatch(s); m == nil {
			t.Errorf("regex missed %q", s)
		}
	}
	neg := []string{
		"udg_Player",
		"GetUnitTypeId",
		"gg_unit",      // no tail
		"gg__unit_foo", // double underscore breaks the kind capture
		"",
	}
	for _, s := range neg {
		if m := ggRefRE.FindStringSubmatch(s); m != nil {
			t.Errorf("regex matched %q unexpectedly", s)
		}
	}
}
