---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/mpq/atomic.go"
ref: HEAD
---

package mpq

// WriteFileAtomic writes data to path atomically: bytes go to a sibling temp
// file, fsync, then os.Rename over the destination. On any failure the original
// file is left byte-for-byte untouched and the temp file is removed.
//
// This is the exported entry point to the same temp+fsync+rename idiom WriteFile
// uses for whole-archive writes (see write.go's writeAtomic). It exists so other
// packages — notably internal/forge's folder-backed single-file save path —
// reuse the proven primitive instead of re-implementing a truncating
// os.WriteFile that can leave a half-written, unparseable file after a crash,
// power loss, or disk-full mid-write.
func WriteFileAtomic(path string, data []byte) error {
	return writeAtomic(path, nil, data)
}
