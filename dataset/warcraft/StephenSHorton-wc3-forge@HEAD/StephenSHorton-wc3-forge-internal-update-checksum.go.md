---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/update/checksum.go"
ref: HEAD
---

package update

import (
	"bufio"
	"bytes"
	"net/url"
	"path"
	"strings"
)

// ChecksumsAssetName is the release asset that lists the SHA-256 of each
// published artifact, one per line in the standard "<hex>␠␠<name>" format.
// release.yml writes it; the updater fetches it to verify the installer before
// running it.
const ChecksumsAssetName = "SHA256SUMS"

// ChecksumsURL derives the URL of the SHA256SUMS asset that sits alongside the
// given release-asset URL. GitHub serves release assets at
// ".../releases/download/<tag>/<filename>", so the checksums file is the
// sibling path with the final segment replaced by "SHA256SUMS".
func ChecksumsURL(assetURL string) (string, error) {
	u, err := url.Parse(assetURL)
	if err != nil {
		return "", err
	}
	u.Path = path.Join(path.Dir(u.Path), ChecksumsAssetName)
	return u.String(), nil
}

// ParseSHA256SUMS returns the lowercase hex SHA-256 recorded for filename in a
// SHA256SUMS file's contents, matching on the base name. Lines are
// "<64 hex>  <name>" (the standard sha256sum format; the name may carry a
// leading "*" for binary mode and may contain spaces). Returns ("", false) when
// no line matches.
func ParseSHA256SUMS(data []byte, filename string) (string, bool) {
	want := path.Base(strings.ReplaceAll(filename, "\\", "/"))
	sc := bufio.NewScanner(bytes.NewReader(data))
	for sc.Scan() {
		line := strings.TrimSpace(sc.Text())
		// Need at least 64 hex chars + a separator + ≥1 name char.
		if len(line) < 66 || strings.HasPrefix(line, "#") {
			continue
		}
		hash := strings.ToLower(line[:64])
		if !isHexSHA256(hash) {
			continue
		}
		name := strings.TrimSpace(line[64:])
		name = strings.TrimPrefix(name, "*")          // binary-mode marker
		name = strings.ReplaceAll(name, "\\", "/")     // tolerate windows separators
		if path.Base(strings.TrimSpace(name)) == want {
			return hash, true
		}
	}
	return "", false
}

func isHexSHA256(s string) bool {
	if len(s) != 64 {
		return false
	}
	for _, c := range s {
		if !((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')) {
			return false
		}
	}
	return true
}
