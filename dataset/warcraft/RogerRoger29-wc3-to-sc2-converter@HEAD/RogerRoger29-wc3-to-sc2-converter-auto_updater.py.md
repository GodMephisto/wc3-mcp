---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "auto_updater.py"
ref: HEAD
---

"""Auto-update checker — queries GitHub Releases API for newer versions."""
from __future__ import annotations
import json, urllib.request
from typing import Optional, Tuple


CURRENT_VERSION = "2.0.0"
GITHUB_API = "https://api.github.com/repos/RogerRoger29/wc3-to-sc2-converter/releases/latest"


def _parse_version(v: str) -> Tuple[int, int, int]:
    """Parse '2.1.0' → (2, 1, 0). Strips 'v' prefix."""
    v = v.lstrip("v")
    parts = v.split(".")
    return (int(parts[0]), int(parts[1]), int(parts[2]) if len(parts) > 2 else 0)


def check_for_update() -> Optional[dict]:
    """Check GitHub for a newer release.

    Returns None if no update available or on network error.
    Returns {'version': 'v2.1.0', 'url': '...', 'body': '...'} if update available.
    """
    try:
        req = urllib.request.Request(GITHUB_API, headers={"User-Agent": "wc3toSC2-updater"})
        with urllib.request.urlopen(req, timeout=10) as resp:
            data = json.loads(resp.read().decode())
    except Exception:
        return None

    tag = data.get("tag_name", "")
    if not tag:
        return None

    try:
        current = _parse_version(CURRENT_VERSION)
        latest = _parse_version(tag)
    except (ValueError, IndexError):
        return None

    if latest > current:
        return {
            "version": tag,
            "url": data.get("html_url", ""),
            "body": data.get("body", "")[:500],
            "assets": [
                {"name": a["name"], "url": a["browser_download_url"]}
                for a in data.get("assets", [])
            ],
        }
    return None
