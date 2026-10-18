---
type: vendor-doc
source_repo: "https://github.com/OpenDiablo2/mpq"
source_path: "pkg/file_record.go"
ref: HEAD
---

package pkg

// FileRecord represents a file record in an MPQ
type FileRecord struct {
	MpqFile          string
	IsPatch          bool
	UnpatchedMpqFile string
}
