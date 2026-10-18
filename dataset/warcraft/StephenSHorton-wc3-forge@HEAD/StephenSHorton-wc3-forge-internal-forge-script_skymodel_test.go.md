---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/script_skymodel_test.go"
ref: HEAD
---

package forge

import (
	"strings"
	"testing"
)

func TestRewriteSetSkyModel_ReplaceJASS(t *testing.T) {
	src := strings.Join([]string{
		`function main takes nothing returns nothing`,
		`    call SetCameraBounds(...)`,
		`    call SetSkyModel("Environment\\Sky\\OldSky\\OldSky.mdl")`,
		`    call NewSoundEnvironment("Default")`,
		`endfunction`,
	}, "\n")
	out, err := rewriteSetSkyModel([]byte(src), "environment/sky/northrendsky/northrendsky.mdx", false)
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	s := string(out)
	if !strings.Contains(s, `SetSkyModel("Environment\\Sky\\Northrendsky\\Northrendsky.mdl")`) {
		t.Errorf("expected new path in output, got:\n%s", s)
	}
	if strings.Contains(s, "OldSky") {
		t.Errorf("expected old path replaced, got:\n%s", s)
	}
	// Indent preserved.
	if !strings.Contains(s, "    call SetSkyModel(") {
		t.Errorf("expected 4-space indent preserved, got:\n%s", s)
	}
}

func TestRewriteSetSkyModel_ReplaceLua(t *testing.T) {
	src := strings.Join([]string{
		`function main()`,
		`    SetCameraBounds(...)`,
		`    SetSkyModel("Environment\\Sky\\OldSky\\OldSky.mdl")`,
		`    NewSoundEnvironment("Default")`,
		`end`,
	}, "\n")
	out, err := rewriteSetSkyModel([]byte(src), "environment/sky/outland_sky/outland_sky.mdx", true)
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	s := string(out)
	if !strings.Contains(s, `SetSkyModel("Environment\\Sky\\Outland_Sky\\Outland_Sky.mdl")`) {
		t.Errorf("expected new path in output, got:\n%s", s)
	}
	if strings.Contains(s, "OldSky") {
		t.Errorf("expected old path replaced, got:\n%s", s)
	}
	// Lua doesn't get a `call` prefix.
	if strings.Contains(s, "call SetSkyModel") {
		t.Errorf("Lua output should not have `call` prefix, got:\n%s", s)
	}
}

func TestRewriteSetSkyModel_InsertJASS(t *testing.T) {
	src := strings.Join([]string{
		`function main takes nothing returns nothing`,
		`    call SetCameraBounds(...)`,
		`endfunction`,
	}, "\n")
	out, err := rewriteSetSkyModel([]byte(src), "environment/sky/foo/foo.mdx", false)
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	s := string(out)
	if !strings.Contains(s, `call SetSkyModel("Environment\\Sky\\Foo\\Foo.mdl")`) {
		t.Errorf("expected inserted call, got:\n%s", s)
	}
	// Inserted call should be ABOVE the existing SetCameraBounds line.
	insertIdx := strings.Index(s, "call SetSkyModel")
	camIdx := strings.Index(s, "SetCameraBounds")
	if insertIdx < 0 || camIdx < 0 || insertIdx > camIdx {
		t.Errorf("inserted call should precede existing main body, got:\n%s", s)
	}
}

func TestRewriteSetSkyModel_InsertLua(t *testing.T) {
	src := strings.Join([]string{
		`function main()`,
		`    SetCameraBounds(...)`,
		`end`,
	}, "\n")
	out, err := rewriteSetSkyModel([]byte(src), "environment/sky/bar/bar.mdx", true)
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	s := string(out)
	if !strings.Contains(s, `SetSkyModel("Environment\\Sky\\Bar\\Bar.mdl")`) {
		t.Errorf("expected inserted call, got:\n%s", s)
	}
	if strings.Contains(s, "call SetSkyModel") {
		t.Errorf("Lua insertion shouldn't have `call`, got:\n%s", s)
	}
}

func TestRewriteSetSkyModel_NoMainFailsClearly(t *testing.T) {
	src := "// some other content with no main function"
	_, err := rewriteSetSkyModel([]byte(src), "environment/sky/foo/foo.mdx", false)
	if err == nil {
		t.Fatalf("expected error when main can't be found")
	}
}

func TestRewriteSetSkyModel_EmptyPathIsAllowed(t *testing.T) {
	src := strings.Join([]string{
		`function main takes nothing returns nothing`,
		`    call SetSkyModel("OldPath")`,
		`endfunction`,
	}, "\n")
	out, err := rewriteSetSkyModel([]byte(src), "", false)
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	s := string(out)
	if !strings.Contains(s, `call SetSkyModel("")`) {
		t.Errorf("expected SetSkyModel(\"\") for empty intent, got:\n%s", s)
	}
}

func TestRewriteSetSkyModel_ReplacesAllOccurrences(t *testing.T) {
	// Some maps call SetSkyModel multiple times (e.g. cinematic switches). The
	// editor's pick should win across all of them — anything else would leave
	// a later stale call beating our intent.
	src := strings.Join([]string{
		`function main takes nothing returns nothing`,
		`    call SetSkyModel("Environment\\Sky\\First\\First.mdl")`,
		`    // ...`,
		`    call SetSkyModel("Environment\\Sky\\Second\\Second.mdl")`,
		`endfunction`,
	}, "\n")
	out, err := rewriteSetSkyModel([]byte(src), "environment/sky/new/new.mdx", false)
	if err != nil {
		t.Fatalf("unexpected error: %v", err)
	}
	s := string(out)
	if strings.Contains(s, "First") || strings.Contains(s, "Second") {
		t.Errorf("expected all occurrences replaced, got:\n%s", s)
	}
	if strings.Count(s, `SetSkyModel("Environment\\Sky\\New\\New.mdl")`) != 2 {
		t.Errorf("expected 2 occurrences of the new path, got:\n%s", s)
	}
}
