---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/update/update.go"
ref: HEAD
---

// Package update implements the "check for a newer release" half of
// wc3-forge's in-app updater. It is deliberately transport-only: it queries
// the GitHub Releases API for the latest published release, compares it
// against the running binary's version, and reports whether an update is
// available plus where to download the Windows installer. Actually running
// the installer lives in package main (it needs Wails/OS plumbing).
//
// No GitHub auth is used — the unauthenticated API allows 60 requests/hour
// per IP, which is plenty for an occasional update check.
package update

import (
	"context"
	"encoding/json"
	"fmt"
	"net/http"
	"strconv"
	"strings"
	"time"
)

// Repo is the GitHub "owner/name" the updater checks against.
const Repo = "StephenSHorton/wc3-forge"

// installerAssetSuffix identifies the Windows NSIS installer among a
// release's assets (e.g. "wc3-forge-amd64-installer.exe").
const installerAssetSuffix = "-installer.exe"

// Asset is one downloadable file attached to a release.
type Asset struct {
	Name string `json:"name"`
	URL  string `json:"url"`  // browser_download_url
	Size int64  `json:"size"` // bytes
}

// Release is the subset of a GitHub release we care about.
type Release struct {
	Version   string `json:"version"`   // normalized, no leading "v"
	TagName   string `json:"tag"`       // raw tag, e.g. "v0.4.1"
	Notes     string `json:"notes"`     // release body (markdown)
	URL       string `json:"url"`       // html_url of the release page
	Published string `json:"published"` // ISO-8601 publish time
	Installer *Asset `json:"installer"` // the -installer.exe, or nil if absent
}

// CheckResult is what a single update check yields.
type CheckResult struct {
	Current string   `json:"current"` // running binary's version ("dev" if unstamped)
	Latest  string   `json:"latest"`  // latest published version
	IsNewer bool     `json:"isNewer"` // true iff Latest > Current and an installer exists
	// IsDev is true when the running version can't be parsed as semver (an
	// unstamped/local "dev" build). In that case IsNewer is always false — we
	// can't tell if it's behind — so the UI must NOT claim "you're up to date";
	// it should surface the dev build and still offer the latest release.
	IsDev   bool     `json:"isDev"`
	Release *Release `json:"release"` // details of the latest release (nil on error)
}

// ghRelease mirrors the GitHub Releases API response fields we read.
type ghRelease struct {
	TagName     string `json:"tag_name"`
	HTMLURL     string `json:"html_url"`
	Body        string `json:"body"`
	Draft       bool   `json:"draft"`
	Prerelease  bool   `json:"prerelease"`
	PublishedAt string `json:"published_at"`
	Assets      []struct {
		Name        string `json:"name"`
		DownloadURL string `json:"browser_download_url"`
		Size        int64  `json:"size"`
	} `json:"assets"`
}

// Check queries the latest published release for Repo and compares it to
// current (the running binary's version, with or without a leading "v";
// "dev"/"" means an unstamped local build). The http.Client is injected so
// callers can set timeouts and tests can stub the transport; pass nil for a
// sensible default.
func Check(ctx context.Context, current string, client *http.Client) (*CheckResult, error) {
	if client == nil {
		client = &http.Client{Timeout: 10 * time.Second}
	}

	url := fmt.Sprintf("https://api.github.com/repos/%s/releases/latest", Repo)
	req, err := http.NewRequestWithContext(ctx, http.MethodGet, url, nil)
	if err != nil {
		return nil, fmt.Errorf("build request: %w", err)
	}
	req.Header.Set("Accept", "application/vnd.github+json")
	req.Header.Set("User-Agent", "wc3-forge-updater/"+normalize(current))

	resp, err := client.Do(req)
	if err != nil {
		return nil, fmt.Errorf("github releases: %w", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		return nil, fmt.Errorf("github releases: unexpected status %s", resp.Status)
	}

	var gh ghRelease
	if err := json.NewDecoder(resp.Body).Decode(&gh); err != nil {
		return nil, fmt.Errorf("decode release: %w", err)
	}

	return resultFromRelease(current, &gh), nil
}

// resultFromRelease builds a CheckResult from a decoded release. Split out
// from Check so tests can exercise the comparison + asset-selection logic
// without a live HTTP round-trip.
func resultFromRelease(current string, gh *ghRelease) *CheckResult {
	rel := &Release{
		Version:   normalize(gh.TagName),
		TagName:   gh.TagName,
		Notes:     gh.Body,
		URL:       gh.HTMLURL,
		Published: gh.PublishedAt,
		Installer: pickInstaller(gh),
	}

	res := &CheckResult{
		Current: normalize(current),
		Latest:  rel.Version,
		Release: rel,
	}
	// Only flag an update when we can actually act on it: the remote must be
	// strictly newer AND ship a Windows installer to download.
	res.IsNewer = rel.Installer != nil && isNewer(rel.Version, current)
	// A current version we can't parse (an unstamped "dev" build) means the
	// comparison above is meaningless — flag it so the UI offers the release
	// instead of falsely reporting "up to date".
	_, _, _, curOK := parseSemver(current)
	res.IsDev = !curOK
	return res
}

// pickInstaller returns the Windows installer asset, or nil if the release
// has none.
func pickInstaller(gh *ghRelease) *Asset {
	for _, a := range gh.Assets {
		if strings.HasSuffix(strings.ToLower(a.Name), installerAssetSuffix) {
			return &Asset{Name: a.Name, URL: a.DownloadURL, Size: a.Size}
		}
	}
	return nil
}

// normalize strips a leading "v" and surrounding whitespace from a version
// string. "v0.4.1" -> "0.4.1".
func normalize(v string) string {
	v = strings.TrimSpace(v)
	v = strings.TrimPrefix(v, "v")
	if v == "" {
		return "dev"
	}
	return v
}

// isNewer reports whether latest is a strictly greater semantic version than
// current. A current of "dev"/""/unparseable returns false: an unstamped
// local build should never be nagged to "update" to a release it may already
// be ahead of.
func isNewer(latest, current string) bool {
	lm, ln, lp, lok := parseSemver(latest)
	cm, cn, cp, cok := parseSemver(current)
	if !lok || !cok {
		return false
	}
	if lm != cm {
		return lm > cm
	}
	if ln != cn {
		return ln > cn
	}
	return lp > cp
}

// parseSemver parses a "MAJOR.MINOR.PATCH" string (leading "v" tolerated,
// any "-prerelease"/"+build" suffix ignored). ok is false if the three core
// numbers can't be read.
func parseSemver(v string) (major, minor, patch int, ok bool) {
	v = strings.TrimPrefix(strings.TrimSpace(v), "v")
	// Drop any prerelease/build metadata: "1.2.3-rc1+abc" -> "1.2.3".
	if i := strings.IndexAny(v, "-+"); i >= 0 {
		v = v[:i]
	}
	parts := strings.Split(v, ".")
	if len(parts) != 3 {
		return 0, 0, 0, false
	}
	var err error
	if major, err = strconv.Atoi(parts[0]); err != nil {
		return 0, 0, 0, false
	}
	if minor, err = strconv.Atoi(parts[1]); err != nil {
		return 0, 0, 0, false
	}
	if patch, err = strconv.Atoi(parts[2]); err != nil {
		return 0, 0, 0, false
	}
	return major, minor, patch, true
}
