---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/slk/ini.go"
ref: HEAD
---

package slk

import (
	"bufio"
	"bytes"
	"strings"
)

// LoadINI merges a WC3-style INI file into m. Section names become row keys
// (case-sensitive — `[Hpal]` ≠ `[hpal]` in WC3 conventions), and each
// `key=value` line becomes a column on that row (case-insensitive column
// names, matching how SLK columns work).
//
// Reforged CASC stripped per-asset paths out of base SLKs (Doodads.slk,
// UnitData.slk, …) and moved them into companion `*Skin.txt` files —
// `doodadSkins.txt`, `unitSkin.txt`, etc. Those text files are this format.
// Without merging them, every doodad/unit lookup returns a row with no
// `file` column, and nothing renders.
//
// Tolerances baked in (all hit during real-world parsing):
//   - UTF-8 BOM prefix on the first line.
//   - CRLF line endings.
//   - Blank lines + lines starting with `//` or `;` (comments).
//   - Keys containing `:` (e.g. `defScale:hd=0.65` for HD-specific overrides);
//     stored verbatim, since some callers want the suffix and others don't.
//   - Trailing whitespace on values.
func (m *Mapped) LoadINI(buf []byte) error {
	// Strip UTF-8 BOM if present so it doesn't end up as part of the first
	// section name.
	buf = bytes.TrimPrefix(buf, []byte{0xEF, 0xBB, 0xBF})

	sc := bufio.NewScanner(bytes.NewReader(buf))
	sc.Buffer(make([]byte, 0, 64*1024), 1024*1024)

	var current MappedRow
	for sc.Scan() {
		line := strings.TrimSpace(sc.Text())
		if line == "" || strings.HasPrefix(line, "//") || strings.HasPrefix(line, ";") {
			continue
		}
		if strings.HasPrefix(line, "[") && strings.HasSuffix(line, "]") {
			key := line[1 : len(line)-1]
			if key == "" {
				current = nil
				continue
			}
			row, ok := m.Rows[key]
			if !ok {
				row = MappedRow{}
				m.Rows[key] = row
			}
			current = row
			continue
		}
		if current == nil {
			continue
		}
		eq := strings.IndexByte(line, '=')
		if eq < 0 {
			continue
		}
		col := strings.TrimSpace(line[:eq])
		val := strings.TrimSpace(line[eq+1:])
		if col == "" {
			continue
		}
		current[strings.ToLower(col)] = val
	}
	return sc.Err()
}
