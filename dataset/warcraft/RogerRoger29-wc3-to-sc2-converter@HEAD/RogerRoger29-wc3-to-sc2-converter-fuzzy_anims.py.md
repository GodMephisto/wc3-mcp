---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "fuzzy_anims.py"
ref: HEAD
---

"""Fuzzy animation name matching for WC3 → SC2 sequence mapping.

Uses Levenshtein distance against known mappings to handle typos, variant spellings,
and non-standard animation names in community models.
"""
from __future__ import annotations
from typing import Dict

# Canonical WC3 sequence names → SC2 animation tokens
CANONICAL_MAP: Dict[str, str] = {
    "Stand": "Stand", "Stand - 1": "Stand", "Stand - 2": "Stand 02",
    "Stand - 3": "Stand 03", "Stand - 4": "Stand 04",
    "Stand Ready": "Stand", "Stand Hit": "Stand", "Stand Channel": "Stand",
    "Walk": "Walk", "Run": "Walk", "Walk Fast": "Walk",
    "Attack": "Attack", "Attack - 1": "Attack", "Attack - 2": "Attack 02",
    "Attack Slam": "Attack", "Attack Alternate": "Attack",
    "Spell": "Spell", "Spell Channel": "Spell Channel", "Spell Slam": "Spell",
    "Spell - 1": "Spell", "Spell - 2": "Spell 02",
    "Death": "Death", "Dissipate": "Death Disintegrate",
    "Decay": "Death Disintegrate", "Decay Flesh": "Death",
    "Decay Bone": "Death Disintegrate",
    "Birth": "Birth", "Portrait": "Portrait", "Portrait Talk": "Portrait",
    "Morph": "Morph", "Morph Alternate": "Morph",
    "Sleep": "Stand", "Eat": "Stand",
}


def _levenshtein(s1: str, s2: str) -> int:
    """Compute Levenshtein edit distance between two strings."""
    if len(s1) < len(s2):
        return _levenshtein(s2, s1)
    if len(s2) == 0:
        return len(s1)
    prev = list(range(len(s2) + 1))
    for i, c1 in enumerate(s1):
        curr = [i + 1]
        for j, c2 in enumerate(s2):
            curr.append(min(
                prev[j + 1] + 1,          # insertion
                curr[j] + 1,              # deletion
                prev[j] + (0 if c1 == c2 else 1)  # substitution
            ))
        prev = curr
    return prev[-1]


def _normalize(name: str) -> str:
    """Normalize an animation name for comparison: lowercase, strip whitespace,
    replace hyphens/underscores with spaces, collapse multiple spaces."""
    n = name.lower().strip()
    n = n.replace("_", " ").replace("-", " ")
    while "  " in n:
        n = n.replace("  ", " ")
    return n


def fuzzy_match(wc3_name: str, min_score: float = 0.6) -> tuple[str, float]:
    """Find the best SC2 animation token for a WC3 sequence name.

    Returns (sc2_token, confidence) where confidence is 0.0–1.0.
    If confidence < min_score, the original name is returned as-is.
    """
    # Exact match (case-insensitive, normalized)
    norm = _normalize(wc3_name)
    for key, val in CANONICAL_MAP.items():
        if _normalize(key) == norm:
            return (val, 1.0)

    # Fuzzy match
    best_key, best_val, best_dist = None, None, 999
    for key, val in CANONICAL_MAP.items():
        d = _levenshtein(norm, _normalize(key))
        if d < best_dist:
            best_dist = d
            best_key = key
            best_val = val

    # Confidence based on edit distance relative to string length
    max_len = max(len(norm), len(_normalize(best_key or "")))
    confidence = 1.0 - (best_dist / max_len) if max_len > 0 else 1.0

    if confidence >= min_score:
        return (best_val or wc3_name, confidence)
    return (wc3_name, confidence)


def build_anim_map(sequence_names: list[str]) -> Dict[str, str]:
    """Given a list of WC3 sequence names, return a complete WC3→SC2 mapping.

    Exact matches take priority, then fuzzy matches fill in the rest.
    Logs low-confidence matches for user review.
    """
    result: Dict[str, str] = {}

    # First pass: exact and high-confidence matches
    low_conf: list[tuple[str, str, float]] = []
    for name in sequence_names:
        token, conf = fuzzy_match(name, min_score=0.6)
        result[name] = token
        if conf < 0.85:
            low_conf.append((name, token, conf))

    # Deduplicate SC2 tokens: if two WC3 names map to the same token, append numbers
    used: Dict[str, int] = {}
    final: Dict[str, str] = {}
    for wc3_name, sc2_token in result.items():
        if sc2_token in used:
            used[sc2_token] += 1
            final[wc3_name] = f"{sc2_token} {used[sc2_token]:02d}"
        else:
            used[sc2_token] = 1
            final[wc3_name] = sc2_token

    return final
