---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/casc/casc_windows.go"
ref: HEAD
---

//go:build windows

package casc

import (
	"fmt"
	"syscall"
	"unsafe"
)

// libFileName is the CascLib shared-library filename we look for on this OS.
const libFileName = "CascLib.dll"

// Windows binds CascLib with syscall.NewLazyDLL and calls each entry point
// through LazyProc.Call — exactly as pre-PR main did. This is deliberately
// NOT purego: routing CascOpenFile's pvFileName / CascReadFile's lpBuffer
// through purego v0.10.0's Windows SyscallN dispatch regressed every
// per-file open to a GetLastError==0 failure (geometry intact, every texture
// missing → pure-white models), while CascOpenStorage kept working. The
// macOS purego path needs typed pointers so the GC can't reclaim a write-back
// arg mid-call (see casc_unix.go); on Windows LazyProc.Call is //go:uintptrescapes,
// so `uintptr(unsafe.Pointer(&x))` args are kept alive across the call by the
// compiler — memory-safe AND the path that demonstrably renders textures.
var (
	dll               *syscall.LazyDLL
	procOpenStorage   *syscall.LazyProc
	procCloseStorage  *syscall.LazyProc
	procOpenFile      *syscall.LazyProc
	procCloseFile     *syscall.LazyProc
	procReadFile      *syscall.LazyProc
	procGetFileSize   *syscall.LazyProc
	procFindFirstFile *syscall.LazyProc
	procFindNextFile  *syscall.LazyProc
	procFindClose     *syscall.LazyProc
)

// bindCASCLib loads CascLib.dll at path and resolves every entry point. The
// symbol names are CascLib's exported (undecorated) names even on the
// _UNICODE build. CascGetFileSize64 is the 64-bit size variant — the legacy
// 32-bit CascGetFileSize returns INVALID_FILE_SIZE for valid files on this
// vcpkg build. We probe dll.Load() up front so a missing/locked DLL surfaces
// as a clear error before the first Open; the LazyProcs resolve on first .Call.
func bindCASCLib(path string) error {
	dll = syscall.NewLazyDLL(path)
	procOpenStorage = dll.NewProc("CascOpenStorage")
	procCloseStorage = dll.NewProc("CascCloseStorage")
	procOpenFile = dll.NewProc("CascOpenFile")
	procCloseFile = dll.NewProc("CascCloseFile")
	procReadFile = dll.NewProc("CascReadFile")
	procGetFileSize = dll.NewProc("CascGetFileSize64")
	procFindFirstFile = dll.NewProc("CascFindFirstFile")
	procFindNextFile = dll.NewProc("CascFindNextFile")
	procFindClose = dll.NewProc("CascFindClose")
	if err := dll.Load(); err != nil {
		return fmt.Errorf("load %s: %w", path, err)
	}
	return nil
}

// rawOpenStorage opens a CASC storage. On Windows CascLib is built with
// _UNICODE, so szParams is LPCTSTR == const wchar_t* (UTF-16LE). The
// conversion-in-the-call-expression keeps pathPtr alive across .Call
// (//go:uintptrescapes).
func rawOpenStorage(path string, localeMask uint32) (uintptr, bool) {
	pathPtr, err := syscall.UTF16PtrFromString(path)
	if err != nil {
		return 0, false
	}
	var handle uintptr
	r1, _, _ := procOpenStorage.Call(
		uintptr(unsafe.Pointer(pathPtr)),
		uintptr(localeMask),
		uintptr(unsafe.Pointer(&handle)),
	)
	return handle, r1 != 0
}

func rawCloseStorage(handle uintptr) bool {
	r1, _, _ := procCloseStorage.Call(handle)
	return r1 != 0
}

// rawOpenFile opens one file by its fully-qualified CASC path. The file name
// is narrow (UTF-8/ASCII) on every platform — CascOpenFile takes const void*.
func rawOpenFile(storage uintptr, name string) (uintptr, bool) {
	nameBytes := append([]byte(name), 0)
	var fileHandle uintptr
	r1, _, _ := procOpenFile.Call(
		storage,
		uintptr(unsafe.Pointer(&nameBytes[0])),
		uintptr(0), // dwLocaleFlags
		uintptr(0), // dwOpenFlags
		uintptr(unsafe.Pointer(&fileHandle)),
	)
	return fileHandle, r1 != 0
}

func rawCloseFile(handle uintptr) bool {
	r1, _, _ := procCloseFile.Call(handle)
	return r1 != 0
}

// rawGetFileSize reads the 64-bit file size via the out-param uint64*.
func rawGetFileSize(handle uintptr) (uint64, bool) {
	var size64 uint64
	r1, _, _ := procGetFileSize.Call(handle, uintptr(unsafe.Pointer(&size64)))
	return size64, r1 != 0
}

// rawReadFile reads up to len(buf) bytes into buf and returns the count
// CascLib reported. buf must be non-empty (the caller guards size==0).
func rawReadFile(handle uintptr, buf []byte) (uint32, bool) {
	var bytesRead uint32
	r1, _, _ := procReadFile.Call(
		handle,
		uintptr(unsafe.Pointer(&buf[0])),
		uintptr(uint32(len(buf))),
		uintptr(unsafe.Pointer(&bytesRead)),
	)
	return bytesRead, r1 != 0
}

// rawFindFirstFile begins an enumeration filling data (a CASC_FIND_DATA) and
// returns the find handle (0 / ^0 means "no entries").
func rawFindFirstFile(storage uintptr, mask string, data *[findDataSize]byte) uintptr {
	maskBytes := append([]byte(mask), 0)
	listfileName := append([]byte(""), 0)
	hFind, _, _ := procFindFirstFile.Call(
		storage,
		uintptr(unsafe.Pointer(&maskBytes[0])),
		uintptr(unsafe.Pointer(&data[0])),
		uintptr(unsafe.Pointer(&listfileName[0])),
	)
	return hFind
}

func rawFindNextFile(hFind uintptr, data *[findDataSize]byte) bool {
	r1, _, _ := procFindNextFile.Call(hFind, uintptr(unsafe.Pointer(&data[0])))
	return r1 != 0
}

func rawFindClose(hFind uintptr) bool {
	r1, _, _ := procFindClose.Call(hFind)
	return r1 != 0
}

// lastError returns CascLib's reported error from the most recent failing
// call. CascLib uses Win32 SetLastError on Windows.
func lastError() uint32 {
	if e := syscall.GetLastError(); e != nil {
		if errno, ok := e.(syscall.Errno); ok {
			return uint32(errno)
		}
	}
	return 0
}
