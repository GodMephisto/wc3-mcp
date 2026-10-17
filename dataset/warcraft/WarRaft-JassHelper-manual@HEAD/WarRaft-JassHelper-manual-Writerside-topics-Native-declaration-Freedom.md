---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Native-declaration-Freedom.md"
ref: HEAD
---

# Native declaration Freedom

This feature (added in `0.9.I.0`) is similar to global declarations, but it is for more advanced users, so if you do not
understand a single thing of these few paragraphs, feel free to skip to the explanation about the debug keyword.
Warcraft III supports declaration of natives in the map script, but it requires the natives to be declared just after
the script&apos;s globals section. Jasshelper will detect these declarations along the map and move them to the correct
place.

Why would you want to do this? And what native functions exactly? There are some few native functions that were created
for AI scripts that are not declared in common.j, some of them are actually useful for Jass maps. There is also the
possibility you are using a modded/hacked version of warcraft III, and importing a whole new common.j for the native
functions is probably too annoying for you, in this case you can declare the new custom functions in the map as well.

There is a protection in this feature that will make jasshelper delete declarations of duplicate natives. (i.e if the
native was already declared in common.j, it will remove the native from the map script, to ensure your map is actually
playable). This heavily depends on the [common.j](https://github.com/WarRaft/war3mpq/blob/main/extract/Scripts/common.j)
version provided to jasshelper. This is something to consider if for
some reason the common.j version you (or newgen pack) is passing to jasshelper is different to the common.j version you
intend the map to be playable with.

```sql
native GetUnitGoldCost takes integer unitid returns integer

function test takes nothing returns nothing
    call BJDebugMsg("A footman consts : "+I2S( GetUnitGoldCost('hfoo')+" gold coins" ) )
endfunction
```