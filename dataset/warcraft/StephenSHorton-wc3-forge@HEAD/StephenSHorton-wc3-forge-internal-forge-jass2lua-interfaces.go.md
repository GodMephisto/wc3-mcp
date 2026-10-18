---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/interfaces.go"
ref: HEAD
---

package jass2lua

// interfaces.go — vJASS interface preprocessor (Phase 4).
//
// JassHelper interfaces are duck-typed method-signature contracts. In Lua,
// EVERYTHING is duck-typed at runtime; an interface declaration carries no
// runtime semantics. So this pass simply STRIPS `interface … endinterface`
// blocks and leaves a comment marker so the user can find where they were.
//
// We don't surface any of the interface's method signatures — they're
// documentation-only and the implementing structs (`struct Foo extends Bar`
// — vJASS lets a struct extend an interface) work identically to plain
// inheritance.

import (
	"fmt"
	"regexp"
	"strings"
)

var (
	interfaceOpenerRe = regexp.MustCompile(`^\s*interface\s+([A-Za-z_][A-Za-z0-9_]*)\s*$`)
	interfaceCloserRe = regexp.MustCompile(`^\s*endinterface\s*$`)
)

// PreprocessInterfaces strips `interface … endinterface` blocks from src.
// Returns the stripped source. Idempotent for sources with no interfaces.
//
// We don't return diagnostics (no plausible failure case): an unterminated
// interface block just gets dropped at EOF with a marker comment.
func PreprocessInterfaces(src string) string {
	if !strings.Contains(src, "interface") {
		return src
	}
	var out strings.Builder
	out.Grow(len(src))
	lines := splitLinesKeepEnd(src)
	i := 0
	for i < len(lines) {
		line := lines[i]
		trim := strings.TrimRight(line, "\r\n")
		m := interfaceOpenerRe.FindStringSubmatch(trim)
		if m == nil {
			out.WriteString(line)
			i++
			continue
		}
		name := m[1]
		// Skip body until endinterface (or EOF).
		i++
		for i < len(lines) {
			inner := lines[i]
			innerTrim := strings.TrimRight(inner, "\r\n")
			i++
			if interfaceCloserRe.MatchString(innerTrim) {
				break
			}
		}
		// Emit a marker so the diff stays grep-friendly.
		out.WriteString(fmt.Sprintf("// jass2lua: interface %s stripped (no Lua equivalent; duck-typed)\n", name))
	}
	return out.String()
}
