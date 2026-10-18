---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/slk/slk_test.go"
ref: HEAD
---

package slk

import (
	"os"
	"path/filepath"
	"strings"
	"testing"
)

// Minimal SLK with the persisted-Y trick + a quoted string + a numeric and a
// blank cell. Cell at (1,1) intentionally omitted to verify ensure() handles
// sparse rows.
const sampleSLK = `ID;P
B;X3;Y3
C;Y1;X1;K"unitID"
C;X2;K"name"
C;X3;K"file"
C;Y2;X1;K"Hpal"
C;X2;K"Paladin"
C;X3;K"Units\Human\Paladin\Paladin"
C;Y3;X1;K"hfoo"
C;X3;K"Units\Human\Footman\Footman"
E
`

func TestParseSampleSLK(t *testing.T) {
	rows, err := Parse([]byte(sampleSLK))
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if len(rows) != 3 {
		t.Fatalf("want 3 rows, got %d", len(rows))
	}
	// Header
	if got := rows[0][0]; got != "unitID" {
		t.Errorf("rows[0][0] = %q, want unitID", got)
	}
	if got := rows[0][2]; got != "file" {
		t.Errorf("rows[0][2] = %q, want file", got)
	}
	// Paladin row
	if got := rows[1][0]; got != "Hpal" {
		t.Errorf("rows[1][0] = %q, want Hpal", got)
	}
	if got := rows[1][2]; got != `Units\Human\Paladin\Paladin` {
		t.Errorf("rows[1][2] = %q", got)
	}
	// Footman row has X1 and X3 set, X2 blank from sparse
	if rows[2][0] != "hfoo" {
		t.Errorf("rows[2][0] = %q, want hfoo", rows[2][0])
	}
	if rows[2][1] != "" {
		t.Errorf("rows[2][1] = %q, want empty", rows[2][1])
	}
	if rows[2][2] != `Units\Human\Footman\Footman` {
		t.Errorf("rows[2][2] = %q", rows[2][2])
	}
}

func TestMappedLookup(t *testing.T) {
	m := New()
	if err := m.Load([]byte(sampleSLK)); err != nil {
		t.Fatalf("Load: %v", err)
	}
	r := m.Row("Hpal")
	if r == nil {
		t.Fatalf("Row(Hpal) is nil")
	}
	// Case-insensitive column lookup.
	if got := r.String("File"); got != `Units\Human\Paladin\Paladin` {
		t.Errorf("File = %q", got)
	}
	if got := r.String("file"); got != `Units\Human\Paladin\Paladin` {
		t.Errorf("file (lower) = %q", got)
	}
	if r := m.Row("nope"); r != nil {
		t.Errorf("Row(nope) should be nil, got %v", r)
	}
}

func TestMappedMerge(t *testing.T) {
	const extra = `ID;P
B;X2;Y2
C;Y1;X1;K"unitID"
C;X2;K"modelScale"
C;Y2;X1;K"Hpal"
C;X2;K1.25
E
`
	m := New()
	if err := m.Load([]byte(sampleSLK)); err != nil {
		t.Fatalf("Load1: %v", err)
	}
	if err := m.Load([]byte(extra)); err != nil {
		t.Fatalf("Load2: %v", err)
	}
	r := m.Row("Hpal")
	if r == nil {
		t.Fatalf("merged row missing")
	}
	if got := r.String("file"); got != `Units\Human\Paladin\Paladin` {
		t.Errorf("file = %q (lost after merge)", got)
	}
	if got := r.Number("modelScale"); got != 1.25 {
		t.Errorf("modelScale = %v", got)
	}
}

// Skip-if-no-CASC: parse a real SLK pulled from a WC3 install if available.
// This catches WC3-specific quirks we wouldn't see in the synthetic sample.
func TestParseRealCASCAsset(t *testing.T) {
	// Read pre-extracted SLK if the developer dropped one in
	// internal/formats/slk/testdata/. Otherwise skip — keeping this test
	// useful without forcing every contributor to have WC3 installed.
	dir := "testdata"
	entries, err := os.ReadDir(dir)
	if err != nil {
		t.Skip("no testdata/")
	}
	for _, e := range entries {
		if !strings.HasSuffix(strings.ToLower(e.Name()), ".slk") {
			continue
		}
		buf, err := os.ReadFile(filepath.Join(dir, e.Name()))
		if err != nil {
			t.Fatalf("read %s: %v", e.Name(), err)
		}
		m := New()
		if err := m.Load(buf); err != nil {
			t.Errorf("Load %s: %v", e.Name(), err)
		}
		if len(m.Rows) == 0 {
			t.Errorf("Load %s produced no rows", e.Name())
		}
	}
}
