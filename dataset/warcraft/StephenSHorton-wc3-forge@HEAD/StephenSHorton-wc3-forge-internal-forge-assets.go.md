---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/assets.go"
ref: HEAD
---

package forge

// BaseAssetReader returns a CASC/map asset by path (e.g. "Units/UnitData.slk").
// The function is wired in by package main during startup — before that,
// callers get (nil, false, nil). The folder for empty/missing files is
// deliberately falsy so feature code (objectunits.go) can degrade gracefully
// when wc3-forge runs in a headless test environment without a CASC mount.
type BaseAssetReader func(name string) ([]byte, bool, error)

var baseAssetReader BaseAssetReader

// SetBaseAssetReader registers the asset-lookup function used by feature code
// inside the forge package. Called once from main during startup. The reader
// is expected to do map-first-then-CASC resolution itself — feature code
// here doesn't care which source the bytes come from.
func SetBaseAssetReader(r BaseAssetReader) { baseAssetReader = r }

// readBaseAsset is the package-internal entry point for SLK / INI / icon
// lookups that need to hit either the loaded map or the WC3 CASC mount.
// Returns (nil, false, nil) when no reader is registered (test path with
// no CASC) so callers can treat it like "asset not present".
func readBaseAsset(name string) ([]byte, bool, error) {
	if baseAssetReader == nil {
		return nil, false, nil
	}
	return baseAssetReader(name)
}
