---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/casc/casc_unix.go"
ref: HEAD
---

//go:build !windows

package casc

import (
	"fmt"
	"runtime"
	"unsafe"

	"github.com/ebitengine/purego"
)

// libFileName is the CascLib shared-library filename we look for on this OS.
//
// CascLib's CMake produces a "casc.framework" bundle on macOS, but
// scripts/build-casclib-macos.sh extracts the inner Mach-O and re-names
// it to libcasc.dylib (with its install_name rewritten to
// @loader_path/libcasc.dylib so dlopen-by-name works once it sits next
// to the executable).
const libFileName = "libcasc.dylib"

// macOS/Linux call CascLib through purego (no syscall.NewLazyDLL off Windows).
//
// IMPORTANT: every parameter through which CascLib writes back into Go memory
// (handles, sizes, byte counts, the read buffer, the find-data struct) MUST
// be a typed pointer (*T / unsafe.Pointer), NOT uintptr. purego only keeps
// pointer-kind args alive across the foreign call (func.go addValue stores
// v.Pointer() in the live args slice); a `uintptr(unsafe.Pointer(&x))` arg is
// treated as a plain integer, so the GC may reclaim `x` before CascLib's
// write-back — yielding garbage handles/sizes that fail flakily (this was the
// macOS "storage closed" / "CascGetFileSize64 failed" flake). Opaque handles
// we only pass IN (hStorage/hFile/hFind) stay uintptr. The one IN pointer we
// pass as uintptr — szParams in CascOpenStorage — is kept alive explicitly
// with runtime.KeepAlive in rawOpenStorage.
//
// (On Windows the call layer is syscall.LazyProc.Call instead; see
// casc_windows.go for why purego is not used there.)
var (
	cascOpenStorage  func(szParams uintptr, dwLocaleMask uint32, phStorage *uintptr) bool
	cascCloseStorage func(hStorage uintptr) bool
	cascOpenFile     func(hStorage uintptr, pvFileName unsafe.Pointer, dwLocaleFlags uint32, dwOpenFlags uint32, phFile *uintptr) bool
	cascCloseFile    func(hFile uintptr) bool
	cascReadFile     func(hFile uintptr, lpBuffer unsafe.Pointer, dwToRead uint32, pdwRead *uint32) bool
	cascGetFileSize  func(hFile uintptr, pSize *uint64) bool

	cascFindFirstFile func(hStorage uintptr, szMask unsafe.Pointer, pFindData unsafe.Pointer, szListFile unsafe.Pointer) uintptr
	cascFindNextFile  func(hFind uintptr, pFindData unsafe.Pointer) bool
	cascFindClose     func(hFind uintptr) bool
)

// bindCASCLib dlopens the CascLib shared library at path (RTLD_GLOBAL so
// dependent symbols resolve) and binds every entry point via RegisterLibFunc.
// CascGetFileSize64 is the 64-bit size variant (the legacy 32-bit
// CascGetFileSize returns INVALID_FILE_SIZE for valid files).
func bindCASCLib(path string) error {
	handle, err := purego.Dlopen(path, purego.RTLD_NOW|purego.RTLD_GLOBAL)
	if err != nil {
		return fmt.Errorf("dlopen %s: %w", path, err)
	}
	for _, s := range []struct {
		fptr any
		name string
	}{
		{&cascOpenStorage, "CascOpenStorage"},
		{&cascCloseStorage, "CascCloseStorage"},
		{&cascOpenFile, "CascOpenFile"},
		{&cascCloseFile, "CascCloseFile"},
		{&cascReadFile, "CascReadFile"},
		{&cascGetFileSize, "CascGetFileSize64"},
		{&cascFindFirstFile, "CascFindFirstFile"},
		{&cascFindNextFile, "CascFindNextFile"},
		{&cascFindClose, "CascFindClose"},
	} {
		purego.RegisterLibFunc(s.fptr, handle, s.name)
	}
	return nil
}

// rawOpenStorage opens a CASC storage. On macOS/Linux CascLib is compiled
// with TCHAR == char, so szParams is plain UTF-8 — same encoding as a Go
// string in memory. szParams is the one IN pointer we pass as uintptr, so we
// pin the backing buffer with runtime.KeepAlive across the call.
func rawOpenStorage(path string, localeMask uint32) (uintptr, bool) {
	b := append([]byte(path), 0) // NUL-terminated for C
	var handle uintptr
	ok := cascOpenStorage(uintptr(unsafe.Pointer(&b[0])), localeMask, &handle)
	runtime.KeepAlive(b)
	return handle, ok
}

func rawCloseStorage(handle uintptr) bool {
	return cascCloseStorage(handle)
}

// rawOpenFile opens one file by its fully-qualified CASC path. pvFileName is
// a typed unsafe.Pointer so purego keeps nameBytes alive across the call.
func rawOpenFile(storage uintptr, name string) (uintptr, bool) {
	nameBytes := append([]byte(name), 0)
	var fileHandle uintptr
	ok := cascOpenFile(storage, unsafe.Pointer(&nameBytes[0]), 0, 0, &fileHandle)
	runtime.KeepAlive(nameBytes)
	return fileHandle, ok
}

func rawCloseFile(handle uintptr) bool {
	return cascCloseFile(handle)
}

func rawGetFileSize(handle uintptr) (uint64, bool) {
	var size64 uint64
	ok := cascGetFileSize(handle, &size64)
	return size64, ok
}

// rawReadFile reads up to len(buf) bytes into buf. buf must be non-empty
// (the caller guards size==0).
func rawReadFile(handle uintptr, buf []byte) (uint32, bool) {
	var bytesRead uint32
	ok := cascReadFile(handle, unsafe.Pointer(&buf[0]), uint32(len(buf)), &bytesRead)
	runtime.KeepAlive(buf)
	return bytesRead, ok
}

func rawFindFirstFile(storage uintptr, mask string, data *[findDataSize]byte) uintptr {
	maskBytes := append([]byte(mask), 0)
	listfileName := append([]byte(""), 0)
	hFind := cascFindFirstFile(
		storage,
		unsafe.Pointer(&maskBytes[0]),
		unsafe.Pointer(&data[0]),
		unsafe.Pointer(&listfileName[0]),
	)
	runtime.KeepAlive(maskBytes)
	runtime.KeepAlive(listfileName)
	return hFind
}

func rawFindNextFile(hFind uintptr, data *[findDataSize]byte) bool {
	return cascFindNextFile(hFind, unsafe.Pointer(&data[0]))
}

func rawFindClose(hFind uintptr) bool {
	return cascFindClose(hFind)
}

// lastError on non-Windows returns 2 (ENOENT) unconditionally — a deliberate
// degradation. CascLib's POSIX build sets errno on failure, but purego
// dispatches through goroutine machinery that doesn't preserve errno across
// the foreign call, so we can't distinguish "file not in this CASC mount"
// from "real I/O error" the way the Windows GetLastError check does.
//
// The downstream consumer (openOne in casc.go) treats lastError() == 2 as
// "not found — move on to the next mount prefix", which is the right behavior
// for the overwhelmingly common case. Genuine storage errors surface at
// CascOpenStorage time (a separate code path that just reports failure
// without an errno classification anyway), so the lost fidelity is acceptable.
func lastError() uint32 {
	return 2
}
