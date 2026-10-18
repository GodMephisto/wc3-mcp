---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/handlers_objects_import_test.go"
ref: HEAD
---

package forge

import (
	"encoding/json"
	"testing"
)

func TestHandleObjectsImportFromMap_BadParams(t *testing.T) {
	cases := []string{
		`{}`,                                   // missing path + objects
		`{"source_map_path":"x"}`,              // missing objects
		`{"source_map_path":"x","objects":[]}`, // empty objects
		`{"source_map_path":"","objects":[{"kind":"abilities","id":"A100"}]}`, // blank path
		`{"source_map_path":"x","objects":[{"kind":"","id":"A100"}]}`,         // blank kind
		`{"source_map_path":"x","objects":[{"kind":"abilities","id":""}]}`,    // blank id
		`not json`,
	}
	for _, c := range cases {
		if _, err := handleObjectsImportFromMap(json.RawMessage(c)); err == nil {
			t.Errorf("expected error for params %s", c)
		}
	}
}

func TestKindForFieldType(t *testing.T) {
	if kindForFieldType("buffList") != "buffs" {
		t.Error("buffList should map to buffs")
	}
	if kindForFieldType("unitList") != "units" {
		t.Error("unitList should map to units")
	}
	if kindForFieldType("int") != "" {
		t.Error("primitive types should map to empty")
	}
}

func TestSplitFourCCs(t *testing.T) {
	got := splitFourCCs("AHbz,AHwe,_,X1")
	if len(got) != 2 || got[0] != "AHbz" || got[1] != "AHwe" {
		t.Errorf("splitFourCCs gave %v", got)
	}
}
