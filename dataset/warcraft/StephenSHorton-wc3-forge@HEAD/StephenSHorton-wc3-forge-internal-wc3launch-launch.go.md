---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/wc3launch/launch.go"
ref: HEAD
---

// Package wc3launch handles spawning Warcraft III with a map preloaded via the
// `-launch -loadfile` CLI flags. Mirrors HiveWE's `play_test` behaviour
// (src/main_window/hivewe.cpp:419-435) so the editor's "Test Map" affordance
// matches what map authors already expect from the de-facto incumbent tool.
//
// Resolution order for the WC3 install root:
//  1. `WC3FORGE_WC3_PATH` env var (an install root path). Matches the existing
//     wc3-forge convention used by asset_handler.go::wc3InstallPath so a user
//     who already points wc3-forge at a custom CASC root automatically gets
//     Test Map too.
//  2. The platform default — see wc3InstallRootDefault and binaryRelPath in
//     the OS-specific file (launch_windows.go / launch_darwin.go).
//
// The launch is fire-and-forget (`cmd.Start`, not `cmd.Run`) — we don't block
// the editor on WC3's lifetime. The OS process becomes a child of wc3-forge,
// which is fine: WC3 detaches its own window and runs independently.
package wc3launch

import (
	"errors"
	"fmt"
	"os"
	"os/exec"
	"path/filepath"

	"github.com/StephenSHorton/wc3-forge/internal/wc3path"
)

// ErrBinaryNotFound is returned when the resolved Warcraft III binary path
// doesn't exist on disk. The UI uses this to surface a friendlier toast
// than the raw exec error.
var ErrBinaryNotFound = errors.New("Warcraft III binary not found")

// ErrMapNotFound is returned when the map path doesn't resolve to a real
// file. Folder-backed sessions don't have a single launchable .w3x, so this
// is also the error path for "session is folder-backed, no map file to pass".
var ErrMapNotFound = errors.New("map file not found")

// wc3InstallRoot returns the WC3 install root directory via the shared
// resolver (env override -> user-saved GUI path -> OS default), so Test Map
// and the CASC asset mount always point at the same install.
func wc3InstallRoot() string {
	return wc3path.Resolve()
}

// BinaryPath returns the full path to the Warcraft III executable that we
// should exec. The exact filename / bundle layout is per-OS — see
// binaryRelPath in the OS-specific file. Does NOT verify the file exists —
// that's the caller's job (the UI can choose to surface ErrBinaryNotFound
// differently than a generic error).
//
// Matches HiveWE's `hierarchy.root_directory / "x86_64" / "Warcraft III.exe"`
// where root_directory is `<install>/_retail_/` (see hierarchy.ixx:48).
func BinaryPath() string {
	return filepath.Join(append([]string{wc3InstallRoot()}, binaryRelPath...)...)
}

// LaunchWithMap spawns Warcraft III with the supplied map preloaded.
// Returns immediately after spawning — the WC3 process is detached from
// wc3-forge's lifetime by the OS itself (its window is its own).
//
// CLI matches HiveWE's play_test exactly:
//
//	Warcraft III -launch -loadfile <absolute map path>
//
// `-launch` skips Battle.net login and goes straight to the local-files
// path; `-loadfile` opens the named .w3x as if the user double-clicked it
// from the Finder / Windows Explorer.
//
// IMPORTANT v1 limitation: this function does NOT save the current session
// before launching. The caller MUST gate the button on session.IsDirty()
// being false; otherwise the user will see their pre-edit state in-game.
// HiveWE saves first (`map->save(map->filesystem_path)` at hivewe.cpp:421),
// but wc3-forge's MPQ writer isn't implemented yet — folder-backed maps
// can save in place, but WC3 can only run from a .w3x file, not a folder.
// So the supported v1 flow is: user opens their original .w3x, doesn't edit
// (or saves + re-packs externally), clicks Test Map → WC3 launches the
// original file.
func LaunchWithMap(mapPath string) error {
	if mapPath == "" {
		return fmt.Errorf("%w: empty path", ErrMapNotFound)
	}

	// WC3 expects an absolute path under -loadfile. Convert and verify
	// the file exists before paying the cost of exec.
	abs, err := filepath.Abs(mapPath)
	if err != nil {
		return fmt.Errorf("%w: %v", ErrMapNotFound, err)
	}
	info, err := os.Stat(abs)
	if err != nil {
		if os.IsNotExist(err) {
			return fmt.Errorf("%w: %s", ErrMapNotFound, abs)
		}
		return fmt.Errorf("stat map: %w", err)
	}
	if info.IsDir() {
		// Folder-backed sessions can't be launched as-is. WC3 needs a
		// packaged .w3x. The UI gates this case by checking
		// session source via the Path heuristic; surfacing the error
		// here too as belt-and-suspenders.
		return fmt.Errorf("%w: path is a folder, not a .w3x file: %s", ErrMapNotFound, abs)
	}

	bin := BinaryPath()
	if _, err := os.Stat(bin); err != nil {
		if os.IsNotExist(err) {
			return fmt.Errorf("%w: %s", ErrBinaryNotFound, bin)
		}
		return fmt.Errorf("stat WC3 binary: %w", err)
	}

	cmd := exec.Command(bin, "-launch", "-loadfile", abs)
	// Don't capture stdout/stderr — WC3 is a GUI process, it has nothing
	// useful to say on those streams and inheriting them would couple our
	// pipes to its lifetime.
	if err := cmd.Start(); err != nil {
		return fmt.Errorf("start Warcraft III: %w", err)
	}
	// Detach: don't Wait on the child. Letting Go's process record leak
	// is fine for the editor's lifetime — when wc3-forge exits, the OS
	// reaps everything. Calling Wait would block this goroutine for WC3's
	// entire session, and not calling Wait at all is the standard
	// "fire-and-forget" idiom for spawning peer apps.
	_ = cmd.Process.Release()
	return nil
}
