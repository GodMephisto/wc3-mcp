---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/update/update_test.go"
ref: HEAD
---

package update

import "testing"

func TestParseSemver(t *testing.T) {
	cases := []struct {
		in            string
		maj, min, pat int
		ok            bool
	}{
		{"0.4.1", 0, 4, 1, true},
		{"v0.4.1", 0, 4, 1, true},
		{" v1.20.3 ", 1, 20, 3, true},
		{"1.2.3-rc1", 1, 2, 3, true},
		{"1.2.3+build.5", 1, 2, 3, true},
		{"dev", 0, 0, 0, false},
		{"", 0, 0, 0, false},
		{"1.2", 0, 0, 0, false},
		{"1.2.x", 0, 0, 0, false},
	}
	for _, c := range cases {
		maj, min, pat, ok := parseSemver(c.in)
		if ok != c.ok || (ok && (maj != c.maj || min != c.min || pat != c.pat)) {
			t.Errorf("parseSemver(%q) = (%d,%d,%d,%v), want (%d,%d,%d,%v)",
				c.in, maj, min, pat, ok, c.maj, c.min, c.pat, c.ok)
		}
	}
}

func TestIsNewer(t *testing.T) {
	cases := []struct {
		latest, current string
		want            bool
	}{
		{"0.4.1", "0.4.0", true},
		{"0.5.0", "0.4.9", true},
		{"1.0.0", "0.9.9", true},
		{"0.4.1", "0.4.1", false}, // equal
		{"0.4.0", "0.4.1", false}, // older
		{"0.4.1", "v0.4.0", true}, // leading v tolerated
		{"0.4.1", "dev", false},   // dev build is never "behind"
		{"0.4.1", "", false},
		{"garbage", "0.4.0", false},
	}
	for _, c := range cases {
		if got := isNewer(c.latest, c.current); got != c.want {
			t.Errorf("isNewer(%q, %q) = %v, want %v", c.latest, c.current, got, c.want)
		}
	}
}

func TestResultFromRelease_PicksInstallerAndFlagsNewer(t *testing.T) {
	gh := &ghRelease{
		TagName: "v0.4.1",
		HTMLURL: "https://github.com/StephenSHorton/wc3-forge/releases/tag/v0.4.1",
		Body:    "## notes",
	}
	gh.Assets = []struct {
		Name        string `json:"name"`
		DownloadURL string `json:"browser_download_url"`
		Size        int64  `json:"size"`
	}{
		{Name: "wc3-forge-v0.4.1-windows-amd64.zip", DownloadURL: "https://example/zip", Size: 100},
		{Name: "wc3-forge-amd64-installer.exe", DownloadURL: "https://example/installer", Size: 10362749},
	}

	res := resultFromRelease("0.4.0", gh)
	if res.Latest != "0.4.1" || res.Current != "0.4.0" {
		t.Fatalf("versions: current=%q latest=%q", res.Current, res.Latest)
	}
	if !res.IsNewer {
		t.Errorf("IsNewer = false, want true")
	}
	if res.Release.Installer == nil {
		t.Fatalf("installer not selected")
	}
	if res.Release.Installer.URL != "https://example/installer" || res.Release.Installer.Size != 10362749 {
		t.Errorf("wrong installer asset: %+v", res.Release.Installer)
	}
}

func TestResultFromRelease_DevBuild_FlagsDevAndOffersInstaller(t *testing.T) {
	gh := &ghRelease{TagName: "v0.6.5"}
	gh.Assets = []struct {
		Name        string `json:"name"`
		DownloadURL string `json:"browser_download_url"`
		Size        int64  `json:"size"`
	}{
		{Name: "wc3-forge-amd64-installer.exe", DownloadURL: "https://example/installer", Size: 10},
	}
	// An unstamped local build reports "dev" (and "" / garbage behave the same).
	for _, cur := range []string{"dev", "", "not-a-version"} {
		res := resultFromRelease(cur, gh)
		if !res.IsDev {
			t.Errorf("current %q: IsDev = false, want true (unparseable version)", cur)
		}
		if res.IsNewer {
			t.Errorf("current %q: IsNewer = true, want false (can't compare a dev build)", cur)
		}
		if res.Release.Installer == nil {
			t.Errorf("current %q: installer not offered — dev build should still be able to install the release", cur)
		}
	}
	// A real version that IS current must NOT be flagged as dev.
	if res := resultFromRelease("0.6.5", gh); res.IsDev {
		t.Errorf("current 0.6.5: IsDev = true, want false (parseable version)")
	}
}

func TestResultFromRelease_NoInstaller_NotNewer(t *testing.T) {
	gh := &ghRelease{TagName: "v0.4.1"}
	gh.Assets = []struct {
		Name        string `json:"name"`
		DownloadURL string `json:"browser_download_url"`
		Size        int64  `json:"size"`
	}{
		{Name: "wc3-forge-v0.4.1-windows-amd64.zip", DownloadURL: "https://example/zip", Size: 100},
	}
	res := resultFromRelease("0.4.0", gh)
	// Newer version, but nothing installable → don't prompt.
	if res.IsNewer {
		t.Errorf("IsNewer = true with no installer asset, want false")
	}
	if res.Release.Installer != nil {
		t.Errorf("installer should be nil, got %+v", res.Release.Installer)
	}
}
