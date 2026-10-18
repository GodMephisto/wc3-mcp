---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/wc3launch/launch_darwin.go"
ref: HEAD
---

//go:build darwin

package wc3launch

// binaryRelPath is the install-root-relative path to the launchable
// executable. macOS .app bundles are directories; the actual Mach-O
// entry point sits at Contents/MacOS/<bundle-name>. We exec that
// directly so cmd.Start() picks up our argv (open(1)'s --args path
// works too but loses argv0 cleanliness and complicates fire-and-forget
// semantics).
var binaryRelPath = []string{
	"_retail_", "x86_64", "Warcraft III.app", "Contents", "MacOS", "Warcraft III",
}
