---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/slk/slk.go"
ref: HEAD
---

// Package slk parses Warcraft III SLK (Symbolic Link) table files.
//
// SLK is a text-based spreadsheet format (originally Microsoft Multiplan).
// Each line starts with a single-character record type; we only care about:
//
//	ID      header magic; line 1 must start with "ID;"
//	C       cell. Carries X (column), Y (row), K (value) sub-fields.
//	B       bounds (column/row counts) — ignored, untrustworthy in WC3 files.
//	E       end-of-file — ignored.
//
// X/Y sub-fields persist across cells when omitted: a cell that only says
// "C;X3;Kfoo" inherits Y from the previous cell. WC3's SLKs use this for
// compactness — a row typically writes Y once on the first cell and omits
// it thereafter. Values are 1-indexed in the wire format; we store them
// 0-indexed.
//
// String values are double-quoted; unquoted values are numeric or boolean
// literals (we keep them as strings — the consumer parses to number when
// needed).
//
// The format is documented at https://learn.microsoft.com/en-us/openspecs/office_standards/ms-xlsb/
// (yes, really; SLK predates Excel) but the WC3 subset is much smaller than
// the full spec. mdx-m3-viewer's TS parser at parsers/slk/file.ts handles
// the same subset in ~90 LOC; we mirror that.
package slk

import (
	"bufio"
	"bytes"
	"fmt"
	"strconv"
	"strings"
)

// Parse reads an SLK file and returns its rows as [][]string. Empty cells are
// the zero value of string (""). The header row is at index 0.
func Parse(buf []byte) ([][]string, error) {
	if !bytes.HasPrefix(buf, []byte("ID")) {
		return nil, fmt.Errorf("slk: missing ID magic")
	}

	var rows [][]string
	var x, y int

	ensure := func(rows *[][]string, y, x int) {
		for len(*rows) <= y {
			*rows = append(*rows, nil)
		}
		for len((*rows)[y]) <= x {
			(*rows)[y] = append((*rows)[y], "")
		}
	}

	sc := bufio.NewScanner(bytes.NewReader(buf))
	// Some SLKs from CASC can have long lines (tens of KB).
	sc.Buffer(make([]byte, 0, 64*1024), 1024*1024)
	for sc.Scan() {
		line := sc.Text()
		if line == "" {
			continue
		}
		switch line[0] {
		case 'C':
			// Process tokens in order so X/Y are set before K.
			for _, tok := range strings.Split(line, ";") {
				if tok == "" {
					continue
				}
				op := tok[0]
				rest := strings.TrimSpace(tok[1:])
				switch op {
				case 'X':
					if v, err := strconv.Atoi(rest); err == nil {
						x = v - 1
					}
				case 'Y':
					if v, err := strconv.Atoi(rest); err == nil {
						y = v - 1
					}
				case 'K':
					val := rest
					if len(val) >= 2 && val[0] == '"' && val[len(val)-1] == '"' {
						val = val[1 : len(val)-1]
					}
					ensure(&rows, y, x)
					rows[y][x] = val
				}
			}
		case 'B', 'F', 'P', 'O', 'I', 'E':
			// Bounds / format / picture / opts / inherit / end — ignored.
		}
	}
	if err := sc.Err(); err != nil {
		return nil, fmt.Errorf("slk: %w", err)
	}
	return rows, nil
}

// MappedRow is a single SLK row addressable by lowercase column name.
type MappedRow map[string]string

// String returns the cell value at the given column (case-insensitive). Empty
// string if the column doesn't exist on this row.
func (r MappedRow) String(col string) string { return r[strings.ToLower(col)] }

// Number parses the cell as a float64. Returns 0 if the column is empty or not
// numeric. WC3 SLKs occasionally contain numeric cells with a leading '+',
// trailing whitespace, or scientific notation — strconv handles all of these.
func (r MappedRow) Number(col string) float64 {
	s := r.String(col)
	if s == "" {
		return 0
	}
	f, err := strconv.ParseFloat(strings.TrimSpace(s), 64)
	if err != nil {
		return 0
	}
	return f
}

// Mapped is a table indexed by the row-key column (column 0) and lowercased
// column names from the header row (row 0). Mirrors mdx-m3-viewer's
// utils/mappeddata.ts so consumers can use the same column-name vocabulary.
//
// Multiple SLKs (UnitData + unitUI + ItemData, say) can be merged into the
// same Mapped via repeated Load calls; later columns overwrite earlier ones
// when their names collide on the same row key.
type Mapped struct {
	Rows map[string]MappedRow
}

// New returns an empty Mapped table.
func New() *Mapped { return &Mapped{Rows: map[string]MappedRow{}} }

// Load parses an SLK and merges it into m. Row 0 is treated as the header
// (column names). Row 0 col 0 is ignored — that header cell typically holds
// the name of the key column itself ("unitID", "doodID", etc.) and is not
// used for lookup. Each subsequent row's column 0 is the row key.
//
// SLKs in the wild sometimes carry blank rows or a single-underscore key
// ("_") — both are skipped, matching the lib's MappedData behaviour.
func (m *Mapped) Load(buf []byte) error {
	rows, err := Parse(buf)
	if err != nil {
		return err
	}
	if len(rows) == 0 {
		return nil
	}
	header := rows[0]
	for i := 1; i < len(rows); i++ {
		row := rows[i]
		if len(row) == 0 {
			continue
		}
		key := row[0]
		if key == "" || key == "_" {
			continue
		}
		mr, ok := m.Rows[key]
		if !ok {
			mr = MappedRow{}
			m.Rows[key] = mr
		}
		for j := 0; j < len(header); j++ {
			col := header[j]
			if col == "" {
				// UnitBalance.slk has columns with no name; fall back to a
				// positional key so the data isn't lost.
				col = fmt.Sprintf("column%d", j)
			}
			if j < len(row) && row[j] != "" {
				mr[strings.ToLower(col)] = row[j]
			}
		}
	}
	return nil
}

// Row returns the row keyed by k (case-sensitive — FourCC keys are
// case-sensitive in CASC: "Hpal" ≠ "hpal"). Returns nil if absent.
func (m *Mapped) Row(k string) MappedRow { return m.Rows[k] }
