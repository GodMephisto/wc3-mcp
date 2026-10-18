---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "blender_manager.py"
ref: HEAD
---

"""Blender dependency manager — auto-download, install, and configure Blender + m3studio.

Eliminates the #1 adoption barrier: manual Blender + addon installation.
"""
from __future__ import annotations
import os, sys, shutil, zipfile, urllib.request, tempfile, subprocess
from typing import Optional, Callable

BLENDER_VERSION = "4.4.3"
BLENDER_DOWNLOAD = f"https://download.blender.org/release/Blender4.4/blender-{BLENDER_VERSION}-windows-x64.zip"
M3STUDIO_URL = "https://github.com/Solstice245/m3studio/archive/refs/heads/main.zip"
APPDATA = os.environ.get("APPDATA", os.path.expanduser("~"))
BLENDER_DIR = os.path.join(APPDATA, "wc3toSC2", "blender")
ADDONS_DIR_TEMPLATE = os.path.join(BLENDER_DIR, f"blender-{BLENDER_VERSION}-windows-x64",
                                    f"{BLENDER_VERSION}", "scripts", "addons")


def get_blender_exe() -> Optional[str]:
    """Return path to managed Blender exe if installed."""
    exe = os.path.join(BLENDER_DIR, f"blender-{BLENDER_VERSION}-windows-x64", "blender.exe")
    return exe if os.path.exists(exe) else None


def download_with_progress(url: str, dest: str, callback: Optional[Callable[[int, int], None]] = None):
    """Download a file with optional progress callback(bytes_downloaded, total_bytes)."""
    os.makedirs(os.path.dirname(dest) or ".", exist_ok=True)
    with urllib.request.urlopen(url) as resp:
        total = int(resp.headers.get("Content-Length", 0))
        downloaded = 0
        with open(dest, "wb") as f:
            while True:
                chunk = resp.read(65536)
                if not chunk:
                    break
                f.write(chunk)
                downloaded += len(chunk)
                if callback and total:
                    callback(downloaded, total)


def install_blender(callback: Optional[Callable[[str, int, int], None]] = None) -> bool:
    """Download and extract Blender Portable. Returns True on success.

    callback(stage, current, total) — stage: 'download' or 'extract'
    """
    if get_blender_exe():
        return True  # Already installed

    zip_path = os.path.join(tempfile.gettempdir(), f"blender-{BLENDER_VERSION}-windows-x64.zip")
    try:
        # Download
        if callback:
            callback("download", 0, 1)
        download_with_progress(BLENDER_DOWNLOAD, zip_path,
                               lambda c, t: callback("download", c, t) if callback else None)

        # Extract
        if callback:
            callback("extract", 0, 1)
        os.makedirs(BLENDER_DIR, exist_ok=True)
        with zipfile.ZipFile(zip_path, "r") as zf:
            zf.extractall(BLENDER_DIR)
        if callback:
            callback("extract", 1, 1)

        os.unlink(zip_path)
        return os.path.exists(get_blender_exe() or "")
    except Exception:
        if os.path.exists(zip_path):
            os.unlink(zip_path)
        return False


def install_m3studio(callback: Optional[Callable[[int, int], None]] = None) -> bool:
    """Download and install the m3studio addon into the managed Blender. Returns True on success."""
    blender_exe = get_blender_exe()
    if not blender_exe:
        return False

    addons_dir = ADDONS_DIR_TEMPLATE
    target = os.path.join(addons_dir, "m3studio-main")
    if os.path.exists(target):
        return True  # Already installed

    zip_path = os.path.join(tempfile.gettempdir(), "m3studio-main.zip")
    try:
        download_with_progress(M3STUDIO_URL, zip_path, callback)
        os.makedirs(addons_dir, exist_ok=True)
        with zipfile.ZipFile(zip_path, "r") as zf:
            # m3studio-main.zip contains m3studio-main/ folder
            for member in zf.namelist():
                if member.startswith("m3studio-main/"):
                    zf.extract(member, addons_dir)
        os.unlink(zip_path)

        # Enable addon in Blender preferences
        _enable_addon(blender_exe)
        return os.path.exists(target)
    except Exception:
        if os.path.exists(zip_path):
            os.unlink(zip_path)
        return False


def _enable_addon(blender_exe: str):
    """Run Blender headless once to enable the m3studio addon."""
    script = (
        "import bpy, addon_utils\n"
        "addon_utils.enable('m3studio-main', default_set=True, persistent=True)\n"
        "bpy.ops.wm.save_userpref()\n"
    )
    try:
        subprocess.run([blender_exe, "--background", "--python-expr", script],
                       capture_output=True, timeout=60)
    except Exception:
        pass  # Non-fatal; user can enable manually


def ensure_blender_ready(callback: Optional[Callable[[str, int, int], None]] = None) -> Optional[str]:
    """Ensure Blender + m3studio are installed and ready. Returns blender.exe path or None.

    This is the one-call entry point for the GUI setup wizard.
    """
    exe = get_blender_exe()
    if exe and os.path.exists(os.path.join(ADDONS_DIR_TEMPLATE, "m3studio-main")):
        return exe

    if not exe:
        ok = install_blender(callback)
        if not ok:
            return None

    ok = install_m3studio(callback)
    if not ok:
        return None

    return get_blender_exe()


def get_managed_blender_path() -> str:
    """Get the managed Blender path for use in conversion config."""
    exe = get_blender_exe()
    if exe:
        return exe
    return ""
