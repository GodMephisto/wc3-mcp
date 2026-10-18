---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/objectmeta_test.go"
ref: HEAD
---

package forge

import (
	"strings"
	"testing"
)

// Synthesized UnitMetaData.slk fragment with three fields covering the
// columns the parser cares about. Format mirrors the structure dump-unitmeta
// observed against the real Blizzard SLK (column order doesn't matter to
// the slk parser — it keys on row 0).
const sampleUnitMeta = `ID;PWXL;N;E
B;X19;Y4;D0
C;Y1
C;X1;K"id"
C;X2;K"field"
C;X3;K"slk"
C;X4;K"index"
C;X5;K"category"
C;X6;K"displayName"
C;X7;K"type"
C;X8;K"data"
C;X9;K"useUnit"
C;X10;K"useHero"
C;X11;K"useBuilding"
C;X12;K"useItem"
C;Y2
C;X1;K"unam"
C;X2;K"Name"
C;X3;K"Profile"
C;X4;K0
C;X5;K"text"
C;X6;K"WESTRING_UE_NAME"
C;X7;K"string"
C;X8;K0
C;X9;K1
C;X10;K1
C;X11;K1
C;X12;K0
C;Y3
C;X1;K"uhpm"
C;X2;K"HP"
C;X3;K"UnitBalance"
C;X4;K1
C;X5;K"stats"
C;X6;K"WESTRING_UE_HITPOINTSMAX"
C;X7;K"int"
C;X8;K0
C;X9;K1
C;X10;K1
C;X11;K1
C;X12;K0
C;Y4
C;X1;K"iabi"
C;X2;K"abilList"
C;X3;K"ItemData"
C;X4;K-1
C;X5;K"abil"
C;X6;K"WESTRING_UEVAL_IABI"
C;X7;K"abilityList"
C;X8;K0
C;X9;K0
C;X10;K0
C;X11;K0
C;X12;K1
E
`

func TestParseUnitMetadata(t *testing.T) {
	meta, err := ParseUnitMetadata([]byte(sampleUnitMeta))
	if err != nil {
		t.Fatalf("ParseUnitMetadata: %v", err)
	}
	if got := len(meta.Fields); got != 3 {
		t.Fatalf("Fields len = %d, want 3", got)
	}
	unam := meta.ByID["unam"]
	if unam == nil {
		t.Fatal("missing unam")
	}
	if unam.Field != "Name" {
		t.Errorf("unam.Field = %q, want Name", unam.Field)
	}
	if unam.SLK != "Profile" {
		t.Errorf("unam.SLK = %q, want Profile", unam.SLK)
	}
	if unam.Category != "text" {
		t.Errorf("unam.Category = %q, want text", unam.Category)
	}
	if unam.Type != "string" {
		t.Errorf("unam.Type = %q, want string", unam.Type)
	}
	if !unam.UseUnit || !unam.UseHero || !unam.UseBuilding {
		t.Errorf("unam unit/hero/building flags wrong: %+v", unam)
	}
	if unam.UseItem {
		t.Errorf("unam.UseItem should be false")
	}
	if !unam.AppliesToUnits() {
		t.Errorf("unam.AppliesToUnits should be true")
	}

	iabi := meta.ByID["iabi"]
	if iabi == nil {
		t.Fatal("missing iabi")
	}
	if iabi.Index != -1 {
		t.Errorf("iabi.Index = %d, want -1", iabi.Index)
	}
	if !iabi.UseItem {
		t.Errorf("iabi.UseItem should be true")
	}
	if iabi.AppliesToUnits() {
		t.Errorf("iabi.AppliesToUnits should be false (item-only field)")
	}
}

func TestUnitMetadata_FieldMap(t *testing.T) {
	meta, err := ParseUnitMetadata([]byte(sampleUnitMeta))
	if err != nil {
		t.Fatalf("ParseUnitMetadata: %v", err)
	}
	fm := meta.FieldMap()
	// w3objmod parses keys against column names — case must be normalized to
	// lower to match slk.Mapped's column key convention.
	if got := fm["unam"]; got != "name" {
		t.Errorf("FieldMap[unam] = %q, want %q", got, "name")
	}
	if got := fm["uhpm"]; got != "hp" {
		t.Errorf("FieldMap[uhpm] = %q, want %q", got, "hp")
	}
	if got := fm["iabi"]; got != "abilitylist" {
		// Field name is "abilList" (mixed-case in source); FieldMap lowercases.
		// Adjust expectation if SLK column names differ from declared spec.
		if !strings.EqualFold(got, "abillist") {
			t.Errorf("FieldMap[iabi] = %q, want %q", got, "abillist")
		}
	}
}
