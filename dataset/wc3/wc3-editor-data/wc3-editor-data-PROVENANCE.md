---
acquired: 2026-08-28
method: CascLib read of the retail install, extracted verbatim
target: Warcraft III 2.0.x, D:\Warcraft III
files:
  - war3.w3mod:ui\uniteditordata.txt  -> UnitEditorData.txt
  - war3.w3mod:ui\worldeditdata.txt   -> WorldEditData.txt
note: Blizzard's own data, not documentation. UnitEditorData.txt holds one section per
  enumerated object-data field TYPE, each an ordered list of "stored value, WESTRING display
  key". WorldEditData.txt holds the editor's other catalogs, tilesets, doodad and destructable
  categories, sound environments and so on. Together they are the authority for what an
  enumerated field may legally be set to and what each value is CALLED.
---
