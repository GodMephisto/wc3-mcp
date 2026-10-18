---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "mdx_cache.py"
ref: HEAD
---

"""MDX parse cache — avoids double-parsing between convert.py and build_m3.py.

convert.py parses the MDX for diagnostics/texture resolution, then build_m3.py
(re-running inside Blender) parses it again. This module caches the parsed dict
to disk via pickle, saving the second full parse.

Cache invalidation: the cache is keyed by MDX file path + modification time.
"""
from __future__ import annotations
import os, pickle, tempfile
from typing import Dict, Any, Optional

CACHE_DIR = os.path.join(tempfile.gettempdir(), "wc3toSC2_cache")


def _cache_key(mdx_path: str) -> str:
    mtime = os.path.getmtime(mdx_path)
    return f"{os.path.basename(mdx_path)}_{int(mtime)}.pkl"


def load_cached(mdx_path: str) -> Optional[Dict[str, Any]]:
    """Try to load a cached parse result. Returns None if not cached or stale."""
    key = _cache_key(mdx_path)
    cache_path = os.path.join(CACHE_DIR, key)
    if os.path.exists(cache_path):
        try:
            with open(cache_path, "rb") as f:
                return pickle.load(f)
        except Exception:
            pass
    return None


def save_cache(mdx_path: str, data: Dict[str, Any]):
    """Save a parsed MDX dict to the cache."""
    os.makedirs(CACHE_DIR, exist_ok=True)
    key = _cache_key(mdx_path)
    cache_path = os.path.join(CACHE_DIR, key)
    try:
        with open(cache_path, "wb") as f:
            pickle.dump(data, f, protocol=pickle.HIGHEST_PROTOCOL)
    except Exception:
        pass
