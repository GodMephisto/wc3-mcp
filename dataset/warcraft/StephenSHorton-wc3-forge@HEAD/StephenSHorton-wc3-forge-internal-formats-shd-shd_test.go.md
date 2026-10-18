---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/shd/shd_test.go"
ref: HEAD
---

package shd

import "testing"

func TestParse(t *testing.T) {
	// Synthetic: 3×3 vertex terrain → (3-1)*4 = 8 wide × 8 tall = 64 cells
	data := make([]byte, 64)
	for i := range data {
		if i%2 == 0 {
			data[i] = 255
		}
	}
	f, err := Parse(data, 3, 3)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if f.Width != 8 || f.Height != 8 {
		t.Fatalf("dims: %dx%d, want 8x8", f.Width, f.Height)
	}
	if got := f.Cells[2]; got != 255 {
		t.Errorf("Cells[2] = %d, want 255", got)
	}
	if got := f.Cells[1]; got != 0 {
		t.Errorf("Cells[1] = %d, want 0", got)
	}
}

func TestParseSizeMismatch(t *testing.T) {
	// Size 10 doesn't match 64; should return zero-fill + error.
	f, err := Parse(make([]byte, 10), 3, 3)
	if err == nil {
		t.Errorf("expected size-mismatch error")
	}
	if f == nil || len(f.Cells) != 64 {
		t.Errorf("recovery cells len = %d, want 64", len(f.Cells))
	}
}
