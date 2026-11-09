"""Read the published server's tool surface once, for the guards that compare it.

Both guards in this folder that compare a written file with the live surface need
the same three things, the tool list, the family of each tool and the parameter
shape of each tool. This reads them from one MCP stdio session so the two guards
cannot drift on how they read the server.

The family comes from the catalog resource `wc3://tools`, which carries it for
every tool. The title comes from the tool list. The parameter shape comes from the
input schema, with every property's type, default and required flag.

    from surface import read_surface, parameter_specs

    surface = read_surface(exe, 180.0)
    surface["tools"]      every tool object from tools/list
    surface["families"]   name to family, from the catalog
    surface["catalog"]    every catalog entry, as the server builds it
"""

from __future__ import annotations

import json
from pathlib import Path

from measure_surface import MeasureError, Session  # noqa: F401

CATALOG_URI = "wc3://tools"


def _pages(session: Session, method: str, key: str) -> list[dict]:
    """Every page of one list method, kept in the order the server sends them."""
    items: list[dict] = []
    cursor = None
    while True:
        page = session.request(method, {"cursor": cursor} if cursor else {})
        items.extend(page.get(key) or [])
        cursor = page.get("nextCursor")
        if not cursor:
            return items


def read_surface(
    exe: Path,
    timeout: float = 180.0,
    client: str = "surface",
    require_families: bool = True,
) -> dict:
    """The tool list, the families and the catalog from one server session.

    An older build answers `wc3://tools` with a bare list rather than an object
    holding a `tools` key. Both shapes are read, because a guard that compares an
    older exe with the export should still get its tool list.
    """
    session = Session(exe, timeout)
    try:
        init = session.request(
            "initialize",
            {
                "protocolVersion": "2025-06-18",
                "capabilities": {},
                "clientInfo": {"name": client, "version": "1"},
            },
        )
        session.notify("notifications/initialized")
        tools = _pages(session, "tools/list", "tools")
        catalog = session.request("resources/read", {"uri": CATALOG_URI})
    finally:
        session.close()

    contents = catalog.get("contents") or []
    if not contents:
        raise MeasureError(f"the catalog resource {CATALOG_URI} came back empty")
    try:
        document = json.loads(contents[0].get("text") or "{}")
    except ValueError as exc:
        raise MeasureError(f"the catalog resource {CATALOG_URI} is not JSON, {exc}") from exc
    if isinstance(document, dict):
        entries = document.get("tools") or []
    elif isinstance(document, list):
        entries = document
    else:
        raise MeasureError(
            f"the catalog resource {CATALOG_URI} is a {type(document).__name__}, "
            "which is neither an object holding tools nor a list"
        )
    if not entries:
        raise MeasureError(f"the catalog resource {CATALOG_URI} names no tools")

    families = {entry.get("name"): entry.get("family") for entry in entries if isinstance(entry, dict)}
    missing = [tool.get("name") for tool in tools if not families.get(tool.get("name"))]
    if missing and require_families:
        raise MeasureError(
            "the catalog names no family for "
            + ", ".join(str(name) for name in missing[:10])
        )
    return {
        "tools": tools,
        "families": families,
        "catalog": entries,
        "catalog_bytes": len(contents[0].get("text") or ""),
        "init": init,
    }


def parameter_specs(schema: dict | None) -> list[dict]:
    """Every parameter, with the type and default the schema declares."""
    if not isinstance(schema, dict):
        return []
    properties = schema.get("properties") or {}
    required = set(schema.get("required") or [])
    specs = []
    for name, spec in properties.items():
        spec = spec if isinstance(spec, dict) else {}
        type_value = spec.get("type")
        if isinstance(type_value, list):
            type_text = "|".join(str(part) for part in type_value)
        else:
            type_text = str(type_value or "any")
        specs.append(
            {
                "name": name,
                "type": type_text,
                "default": spec.get("default", None),
                "has_default": "default" in spec,
                "required": name in required,
            }
        )
    return specs
