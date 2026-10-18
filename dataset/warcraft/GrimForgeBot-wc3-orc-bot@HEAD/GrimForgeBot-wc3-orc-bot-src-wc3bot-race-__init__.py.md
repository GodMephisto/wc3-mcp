---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/race/__init__.py"
ref: HEAD
---

"""Race registry — register_race() / get_race()."""
from __future__ import annotations
from typing import TYPE_CHECKING
if TYPE_CHECKING:
    from wc3bot.race.base import RaceConfig

_REGISTRY: dict[str, "RaceConfig"] = {}


def register_race(config: "RaceConfig") -> None:
    _REGISTRY[config.name] = config


def get_race(name: str) -> "RaceConfig":
    if name not in _REGISTRY:
        raise KeyError(f"Race {name!r} not registered. Available: {list(_REGISTRY)}")
    return _REGISTRY[name]
