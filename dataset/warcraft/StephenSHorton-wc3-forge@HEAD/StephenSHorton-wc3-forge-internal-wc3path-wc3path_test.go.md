---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/wc3path/wc3path_test.go"
ref: HEAD
---

package wc3path

import (
	"os"
	"path/filepath"
	"testing"
)

func TestSaveSavedPathRoundTrip(t *testing.T) {
	t.Setenv(ConfigDirEnv, t.TempDir())
	if got := SavedPath(); got != "" {
		t.Fatalf("SavedPath before Save = %q; want empty", got)
	}
	want := filepath.Join("some", "where", "Warcraft III")
	if err := Save(want); err != nil {
		t.Fatalf("Save: %v", err)
	}
	if got := SavedPath(); got != want {
		t.Errorf("SavedPath = %q; want %q", got, want)
	}
}

func TestResolvePrecedence(t *testing.T) {
	cfg := t.TempDir()
	t.Setenv(ConfigDirEnv, cfg)

	// No env, no saved -> default.
	t.Setenv(EnvVar, "")
	if got, want := Resolve(), defaultInstallPath(); got != want {
		t.Errorf("Resolve (none set) = %q; want default %q", got, want)
	}

	// Saved beats default.
	saved := filepath.Join(cfg, "saved-wc3")
	if err := Save(saved); err != nil {
		t.Fatalf("Save: %v", err)
	}
	if got := Resolve(); got != saved {
		t.Errorf("Resolve (saved) = %q; want %q", got, saved)
	}

	// Env beats saved.
	env := filepath.Join(cfg, "env-wc3")
	t.Setenv(EnvVar, env)
	if got := Resolve(); got != env {
		t.Errorf("Resolve (env) = %q; want %q", got, env)
	}
}

func TestIsValidInstall(t *testing.T) {
	root := t.TempDir()
	if IsValidInstall(root) {
		t.Error("empty dir reported valid")
	}
	if IsValidInstall("") {
		t.Error("empty path reported valid")
	}
	if err := os.WriteFile(filepath.Join(root, ".build.info"), []byte("Branch!STRING:0\n"), 0o644); err != nil {
		t.Fatalf("write .build.info: %v", err)
	}
	if !IsValidInstall(root) {
		t.Error("dir with .build.info reported invalid")
	}
	// A .build.info that's a directory must not count.
	dirRoot := t.TempDir()
	if err := os.Mkdir(filepath.Join(dirRoot, ".build.info"), 0o755); err != nil {
		t.Fatalf("mkdir: %v", err)
	}
	if IsValidInstall(dirRoot) {
		t.Error("dir whose .build.info is a directory reported valid")
	}
}
