---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/bridge/liveness_windows.go"
ref: HEAD
---

//go:build windows

package bridge

import (
	"syscall"
	"unsafe"
)

// Windows pid-liveness check via OpenProcess + GetExitCodeProcess.
// Ports the C++ fork's pid_is_alive (HiveWE/src/mcp_bridge/lockfile.cpp).
//
// PROCESS_QUERY_LIMITED_INFORMATION (0x1000) is the minimum-privilege handle
// that lets us call GetExitCodeProcess; it's available on every Windows
// version we care about and doesn't require admin.

const (
	processQueryLimitedInformation = 0x1000
	stillActive                    = 259
)

var (
	kernel32               = syscall.NewLazyDLL("kernel32.dll")
	procGetExitCodeProcess = kernel32.NewProc("GetExitCodeProcess")
)

func pidAlive(pid int) bool {
	if pid <= 0 {
		return false
	}
	h, err := syscall.OpenProcess(processQueryLimitedInformation, false, uint32(pid))
	if err != nil {
		return false
	}
	defer syscall.CloseHandle(h)
	var code uint32
	r1, _, _ := procGetExitCodeProcess.Call(uintptr(h), uintptr(unsafe.Pointer(&code)))
	if r1 == 0 {
		return false
	}
	return code == stillActive
}
