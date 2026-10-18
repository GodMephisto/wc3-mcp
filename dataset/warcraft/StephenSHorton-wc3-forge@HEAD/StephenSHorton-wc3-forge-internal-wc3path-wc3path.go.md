---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/wc3path/wc3path.go"
ref: HEAD
---

// Package wc3path resolves the Warcraft III install root that wc3-forge
// reads CASC assets from (asset_handler) and launches for "Test Map"
// (wc3launch), and persists a user-chosen location.
//
// Resolution order (first hit wins):
//
//  1. WC3FORGE_WC3_PATH env var — the power-user / CI override.
//  2. The path the user picked in the GUI, persisted to
//     ~/.wc3-forge/config.json.
//  3. The OS-default install location (see defaultInstallPath, per-platform).
//
// Centralizing this here keeps the asset-serving CASC mount and the Test-Map
// launcher pointed at the same install — they previously each had their own
// copy of the env+default logic and could drift.
package wc3path

import (
	"encoding/json"
	"os"
	"path/filepath"
	"strings"
)

// EnvVar overrides every other source when set. Exported so callers and tests
// don't duplicate the literal.
const EnvVar = "WC3FORGE_WC3_PATH"

// ConfigDirEnv overrides where config.json lives (default ~/.wc3-forge),
// mirroring the bridge's WC3FORGE_MCP_LOCK_DIR. Mainly for tests.
const ConfigDirEnv = "WC3FORGE_CONFIG_DIR"

type config struct {
	// WC3InstallPath is the install ROOT (the directory containing
	// .build.info), not a subfolder like _retail_/x86_64.
	WC3InstallPath string `json:"wc3_install_path"`
}

func configPath() (string, error) {
	dir := os.Getenv(ConfigDirEnv)
	if dir == "" {
		home, err := os.UserHomeDir()
		if err != nil {
			return "", err
		}
		dir = filepath.Join(home, ".wc3-forge")
	}
	return filepath.Join(dir, "config.json"), nil
}

// SavedPath returns the install path the user persisted via Save, or "" if
// none is set or the config is missing/unreadable (all non-fatal — we just
// fall through to the default).
func SavedPath() string {
	p, err := configPath()
	if err != nil {
		return ""
	}
	data, err := os.ReadFile(p)
	if err != nil {
		return ""
	}
	var c config
	if json.Unmarshal(data, &c) != nil {
		return ""
	}
	return c.WC3InstallPath
}

// Save persists the install path so future sessions resolve to it (at lower
// precedence than the env var). Creates the config dir if needed.
func Save(path string) error {
	p, err := configPath()
	if err != nil {
		return err
	}
	if err := os.MkdirAll(filepath.Dir(p), 0o755); err != nil {
		return err
	}
	data, err := json.MarshalIndent(config{WC3InstallPath: path}, "", "  ")
	if err != nil {
		return err
	}
	return os.WriteFile(p, data, 0o644)
}

// Resolve returns the install root to use: env override, else the saved path,
// else the OS default. The result is NOT validated for existence — callers
// that need a real install should gate on IsValidInstall.
func Resolve() string {
	if p := os.Getenv(EnvVar); p != "" {
		return p
	}
	if p := SavedPath(); p != "" {
		return p
	}
	return defaultInstallPath()
}

// Kind classifies a WC3 install root by how its STOCK assets are stored.
type Kind int

const (
	// KindNone — the path is not a recognizable WC3 install root.
	KindNone Kind = iota
	// KindCASC — a Reforged / modern install (.build.info present, assets in
	// CASC storage). Read via internal/casc.
	KindCASC
	// KindClassic — a pre-Reforged, MPQ-based install (1.27b-era): no
	// .build.info, stock assets layered across War3*.mpq archives. Read via
	// internal/mpqchain.
	KindClassic
)

// classicArchiveMarkers are the stock MPQ archives that mark a Classic
// (pre-Reforged) install. Matched case-insensitively against the root's
// directory listing. Mirrors internal/mpqchain's chain; kept here too so the
// install classifier stays free of an mpqchain import (mpqchain imports the
// mpq reader, which we don't want to drag into the path resolver).
var classicArchiveMarkers = []string{"War3.mpq", "War3Patch.mpq"}

// Classify reports how the install at path stores its stock assets:
// KindCASC (.build.info present), KindClassic (no .build.info but a stock
// War3*.mpq present), or KindNone. CASC takes precedence — a modern install
// that happens to retain a stray .mpq is still CASC.
func Classify(path string) Kind {
	if path == "" {
		return KindNone
	}
	if info, err := os.Stat(filepath.Join(path, ".build.info")); err == nil && !info.IsDir() {
		return KindCASC
	}
	for _, marker := range classicArchiveMarkers {
		if hasFileInsensitive(path, marker) {
			return KindClassic
		}
	}
	return KindNone
}

// hasFileInsensitive reports whether dir contains a non-dir entry whose name
// case-insensitively equals name. False on any read failure.
func hasFileInsensitive(dir, name string) bool {
	entries, err := os.ReadDir(dir)
	if err != nil {
		return false
	}
	want := strings.ToLower(name)
	for _, e := range entries {
		if !e.IsDir() && strings.ToLower(e.Name()) == want {
			return true
		}
	}
	return false
}

// IsValidInstall reports whether path looks like a real WC3 install root —
// EITHER a CASC (Reforged/modern) install or a Classic (MPQ-based, 1.27b-era)
// install. Both are accepted so the "locate install" UI takes a Classic
// folder; the install ROOT is what distinguishes it from a subfolder such as
// _retail_/x86_64 (a common mis-pick).
func IsValidInstall(path string) bool {
	return Classify(path) != KindNone
}
