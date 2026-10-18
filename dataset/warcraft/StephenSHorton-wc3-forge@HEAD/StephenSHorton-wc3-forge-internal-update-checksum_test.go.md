---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/update/checksum_test.go"
ref: HEAD
---

package update

import "testing"

func TestChecksumsURL(t *testing.T) {
	cases := []struct {
		in, want string
	}{
		{
			"https://github.com/StephenSHorton/wc3-forge/releases/download/v0.9.0/wc3-forge-amd64-installer.exe",
			"https://github.com/StephenSHorton/wc3-forge/releases/download/v0.9.0/SHA256SUMS",
		},
		{
			"https://github.com/StephenSHorton/wc3-forge/releases/download/v0.9.0/wc3-forge-v0.9.0-windows-amd64.zip",
			"https://github.com/StephenSHorton/wc3-forge/releases/download/v0.9.0/SHA256SUMS",
		},
	}
	for _, c := range cases {
		got, err := ChecksumsURL(c.in)
		if err != nil {
			t.Fatalf("ChecksumsURL(%q): %v", c.in, err)
		}
		if got != c.want {
			t.Errorf("ChecksumsURL(%q) = %q, want %q", c.in, got, c.want)
		}
	}
}

func TestParseSHA256SUMS(t *testing.T) {
	const installerHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"
	sums := []byte(
		"# wc3-forge v0.9.0 checksums\n" +
			installerHash + "  wc3-forge-amd64-installer.exe\n" +
			"1111111111111111111111111111111111111111111111111111111111111111  wc3-forge-v0.9.0-windows-amd64.zip\n",
	)

	if got, ok := ParseSHA256SUMS(sums, "wc3-forge-amd64-installer.exe"); !ok || got != installerHash {
		t.Errorf("installer: got (%q, %v), want (%q, true)", got, ok, installerHash)
	}
	// Match on base name even when given a full URL/path.
	if got, ok := ParseSHA256SUMS(sums, "https://github.com/x/y/releases/download/v0.9.0/wc3-forge-amd64-installer.exe"); !ok || got != installerHash {
		t.Errorf("installer via URL: got (%q, %v), want (%q, true)", got, ok, installerHash)
	}
	if got, ok := ParseSHA256SUMS(sums, "wc3-forge-v0.9.0-windows-amd64.zip"); !ok || got != "1111111111111111111111111111111111111111111111111111111111111111" {
		t.Errorf("zip: got (%q, %v)", got, ok)
	}
	if _, ok := ParseSHA256SUMS(sums, "not-present.exe"); ok {
		t.Errorf("missing file should not match")
	}
	// Binary-mode marker (*) + windows separators must be tolerated.
	bin := []byte(installerHash + " *out\\wc3-forge-amd64-installer.exe\n")
	if got, ok := ParseSHA256SUMS(bin, "wc3-forge-amd64-installer.exe"); !ok || got != installerHash {
		t.Errorf("binary-mode line: got (%q, %v)", got, ok)
	}
	// Empty / garbage input.
	if _, ok := ParseSHA256SUMS([]byte("not a checksum line\n"), "x.exe"); ok {
		t.Errorf("garbage should not match")
	}
}
