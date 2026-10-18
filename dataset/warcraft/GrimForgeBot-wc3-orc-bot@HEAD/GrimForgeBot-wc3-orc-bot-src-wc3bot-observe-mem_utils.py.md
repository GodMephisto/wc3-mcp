---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/observe/mem_utils.py"
ref: HEAD
---

"""wc3bot.observe.mem_utils — Low-level Mach VM memory read primitives.

Shared by lua_reader and mine_workers to avoid circular imports.
"""
from __future__ import annotations

import ctypes
import ctypes.util
import struct

# ── Mach VM API ───────────────────────────────────────────────────────────────

_libc = ctypes.CDLL(ctypes.util.find_library("c"))

_libc.task_for_pid.restype  = ctypes.c_int
_libc.task_for_pid.argtypes = [ctypes.c_uint, ctypes.c_int,
                                ctypes.POINTER(ctypes.c_uint)]
_libc.mach_task_self.restype  = ctypes.c_uint
_libc.mach_task_self.argtypes = []
_libc.mach_vm_read_overwrite.restype  = ctypes.c_int
_libc.mach_vm_read_overwrite.argtypes = [
    ctypes.c_uint, ctypes.c_uint64, ctypes.c_uint64,
    ctypes.c_uint64, ctypes.POINTER(ctypes.c_uint64),
]


class _VMInfo(ctypes.Structure):
    _fields_ = [
        ("protection",       ctypes.c_int),
        ("max_protection",   ctypes.c_int),
        ("inheritance",      ctypes.c_uint),
        ("shared",           ctypes.c_int),
        ("reserved",         ctypes.c_int),
        ("offset",           ctypes.c_uint64),
        ("behavior",         ctypes.c_int),
        ("user_wired_count", ctypes.c_ushort),
    ]


_libc.mach_vm_region.restype  = ctypes.c_int
_libc.mach_vm_region.argtypes = [
    ctypes.c_uint, ctypes.POINTER(ctypes.c_uint64),
    ctypes.POINTER(ctypes.c_uint64), ctypes.c_int,
    ctypes.POINTER(_VMInfo), ctypes.POINTER(ctypes.c_uint),
    ctypes.POINTER(ctypes.c_uint),
]

_MAX_REGION = 512 * 1024 * 1024  # skip huge anonymous / GPU regions


def _read_mem(task: int, addr: int, size: int) -> bytes | None:
    if size <= 0:
        return b""
    buf = (ctypes.c_char * size)()
    out = ctypes.c_uint64(0)
    kr  = _libc.mach_vm_read_overwrite(
        task, addr, size, ctypes.addressof(buf), ctypes.byref(out)
    )
    return bytes(buf[:out.value]) if kr == 0 else None


def _iter_regions(task: int):
    """Yield (base, size, protection) for each VM region."""
    addr = ctypes.c_uint64(1)
    while True:
        size = ctypes.c_uint64(0)
        info = _VMInfo()
        cnt  = ctypes.c_uint(9)
        obj  = ctypes.c_uint(0)
        kr   = _libc.mach_vm_region(
            task, ctypes.byref(addr), ctypes.byref(size), 9,
            ctypes.byref(info), ctypes.byref(cnt), ctypes.byref(obj),
        )
        if kr != 0:
            break
        yield addr.value, size.value, info.protection
        addr.value += size.value


def _read_region(task: int, base: int, size: int,
                 chunk_size: int = 16 * 1024 * 1024) -> bytes:
    """Read an entire VM region in chunks.  Default 16 MB chunk = ~85 syscalls
    for a 1.5 GB scan instead of ~1350 at 4 MB — ~7× fewer Mach IPC calls."""
    parts, off = [], 0
    while off < size:
        n     = min(chunk_size, size - off)
        chunk = _read_mem(task, base + off, n)
        parts.append(chunk if chunk else b"\x00" * n)
        off += n
    return b"".join(parts)
