---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/wc3path/wc3path_windows.go"
ref: HEAD
---

//go:build windows

package wc3path

// defaultInstallPath is Battle.net's default Reforged install location on
// Windows.
func defaultInstallPath() string {
	return `C:\Program Files (x86)\Warcraft III`
}
