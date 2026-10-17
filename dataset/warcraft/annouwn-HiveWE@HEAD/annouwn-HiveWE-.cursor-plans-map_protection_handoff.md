---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: ".cursor/plans/map_protection_handoff.md"
ref: HEAD
---

# Map Protector — Session Handoff

**Date:** 2026-08-05
**Read this if you're picking up Map Protector work in another session.** Phase 1 and Phase 2 (per `map_protection_plan.md`) were already implemented before this session. This session found and fixed a real bug in the underlying MPQ-open path, then used that fix to solve a real correctness problem in the protector itself. Both are shipped and build-verified; the user manually confirmed both work end-to-end (see "Build status" below).

---

## 1. `.w3x` direct-open bug (prerequisite fix, not Map-Protector-specific)

**File:** `src/file_formats/mpq.ixx`

- **Root cause:** `MPQ::unpack()`'s wildcard enumeration (`SFileFindFirstFile(handle, "*", ...)`) depends on the archive's `(listfile)`. Without one — common on real-world maps, and specifically producible by Map Protector's own "Remove listfile" option — StormLib still enumerates every entry but gives unnamed ones fabricated names like `File00003784.blp`. `unpack()` was silently producing a folder missing `war3map.w3i` etc. under their real names, so "Open Map (MPQ)" failed on any listfile-less archive.
- **Fixes:**
  - `MPQ::unpack()` — guarded `find_handle` before dereferencing (was reading an uninitialized `SFILE_FIND_DATA` when `SFileFindFirstFile` found nothing).
  - Added `MPQ::extract_file(archived_name, disk_path)` — extracts one file by exact name, bypassing enumeration entirely. Works without a listfile since MPQ looks files up by name hash, not directory listing.
  - Fixed `MPQ::file_exists()` / `file_open()` — both ran the archive-internal name through `fs::weakly_canonical`, which resolves relative paths against the process's CWD instead of treating them as archive-relative strings. This was dead code (zero live callers before this session), but is now load-bearing for `extract_file()`.
- **`src/main_window/hivewe.cpp`:** `HiveWE::load_mpq()` now loops over `map->imports.blacklist` (the fixed set of core `war3map.*` filenames, already used for import bookkeeping in `src/base/imports.ixx`) and calls `mpq.extract_file()` for each, after the normal `mpq.unpack()` — so core files are always recovered under their real names regardless of listfile presence. Added `import Imports;` to `hivewe.cpp`.
- **Verified against a real map:** confirmed with a standalone StormLib probe (before touching code) against the user's `release.w3x` (Map Protector output: no listfile/attributes) that wildcard enumeration returns fabricated names but `SFileHasFile`/direct-name lookup resolves core files correctly. Re-verified by building and the user testing "Open Map (MPQ)" in the actual app.
- **Known limitation (expected, not a bug):** custom-imported assets (non-core filenames, e.g. `war3mapImported\Something.mdx`) still can't be recovered under their real relative path without a listfile — there's no fixed name list to fall back on for those. Not fixable in general; this is exactly what "Remove listfile" is supposed to achieve.

---

## 2. Map Protector: protect an external file/folder, not just the live in-memory map

**Files:** `src/map_protector/protection_pipeline.ixx`, `src/map_protector/map_protector.h`, `src/map_protector/map_protector.cpp`

**Problem found:** the existing pipeline (`run_sync_save_and_restore`) always calls `map->save(temp_dir)` — HiveWE's full save, which regenerates `war3map.j`/`war3map.lua` from HiveWE's own trigger/GUI state (`Map::save()` → `triggers.generate_map_script()`, `map.ixx:653`). For maps built externally (the user's case: a VS Code extension compiles the actual script), this silently clobbered the real compiled code. Protecting *any* map always broke its code, even though the map itself loaded and displayed fine.

**Fix:** added a second pipeline path in `protection_pipeline.ixx` that never touches `Map::load()`/`Map::save()`:

- `copy_source_folder(source_directory, temp_dir)` — plain recursive copy for folder-mode sources.
- `unpack_source_archive(source_file, temp_dir)` — unpacks via the fixed `mpq::MPQ::unpack()` plus the same known-filename fallback as `load_mpq()` (constructs a local `Imports{}` for `.blacklist` — doesn't touch the global `map`).
- `sanitize_metadata(temp_dir, options)` — patches `war3map.w3i` metadata (author/description/loading text/name) via a **standalone** `MapInfo` instance, not `map->info`. `MapInfo::save()` needs a tileset byte that `MapInfo::load()` itself discards (`map_info.ixx:234`, `reader.advance(1)`), so it's read directly from `war3map.w3e`'s header (4-byte `"W3E!"` magic, 4-byte version, 1-byte tileset — same layout `Terrain::load()` parses at `terrain.ixx:299-307`) instead of loading all of `Terrain`.
- `prepare_source_path(source, temp_dir, options)` — new **exported** entry point tying the three together. Must be called synchronously on the UI thread before backgrounding the pack step (same constraint as the existing `run_sync_save_and_restore`, since `sanitize_metadata` briefly redirects the global `hierarchy.map_directory`).
- New module imports added to `protection_pipeline.ixx`: `Imports`, `MapInfo`, `BinaryReader`, `MPQ`.
- `run_sync_save_and_restore()` (the full `Map::save()` path) is **unchanged** — still used when the user picks "currently loaded map" as source, which is correct there since that's explicitly protecting the live editor session.

**UI changes** in `map_protector.h`/`.cpp`:

- New "Source" `QGroupBox`, added **first** in `main_layout` (before the output-path row), with two `QRadioButton`s: **Currently loaded map** vs **Map file or folder:** (+ a `QLineEdit` and **Browse File...** / **Browse Folder...** buttons, enabled only when the second radio is checked).
- New members: `source_current_map_radio`, `source_external_radio`, `source_path_edit`, `source_browse_file_button`, `source_browse_folder_button`.
- New handlers: `on_source_browse_file_clicked()`, `on_source_browse_folder_clicked()`, `update_source_controls_enabled()`.
- `on_export_clicked()` now branches on which radio is checked: validates/builds a `source_path` accordingly, then calls `run_sync_save_and_restore()` or `prepare_source_path()`.
- `set_options_enabled()`'s widget list gained `source_current_map_radio`/`source_external_radio`, and it now calls `update_source_controls_enabled()` at the end — so the source path row's enabled state stays tied to the radio selection through the disable/re-enable cycle instead of being blanket-toggled.
- `load_settings()`/`save_settings()` gained two new `QSettings` keys under the `MapProtector` group: `useCurrentMap` (bool, defaults to `map && map->loaded`) and `sourcePath` (string).

**Heads up if you're actively editing this feature:** if you're mid-way modifying `ProtectionOptions`, `on_export_clicked()`, `set_options_enabled()`, `load_settings()`/`save_settings()`, or the constructor layout in `map_protector.cpp`, expect overlap — these are the exact touch points this session's change went through. Diff against `git log` for the commit(s) from this session before merging manually-written changes on top.

---

## 3. Plan doc is stale

`map_protection_plan.md` still says `Status: Planning — not yet implemented` and describes a `protection_pipeline.h`/`.cpp` split. Both are inaccurate: the actual implementation is a single `protection_pipeline.ixx` C++ module (matches this repo's module convention, not the plan's assumed `.h`/`.cpp` split), and Phase 1 + Phase 2 were both already done before this session. This session didn't rewrite the plan doc itself — just flagging so it isn't trusted at face value. Worth a proper pass to bring it in line with reality (current file layout, options actually implemented vs. still-aspirational ones like `obfuscate_object_ids`, `strip_trigger_strings`, `remove_script_source`, `strip_unused_fields` — none of those exist in the current `ProtectionOptions`).

---

## Build / test status

- Both changes build clean via `cmake --build --preset Release --target HiveWE` (only warning is a pre-existing, unrelated `#include "StormLib.h";` semicolon warning in `mpq.ixx`, not introduced this session).
- Not run through `ctest` this session.
- User manually verified in the real app: opened a real listfile-less `.w3x` successfully via "Open Map (MPQ)"; protected it via the new external-source path successfully; confirmed the protected output no longer opens in the stock World Editor and still runs correctly in-game.

---

## Session (2026-08-07/08): loading-screen crash root-caused and fixed; UI green-boxes mostly fixed; spell/hero icons via native functions still unresolved

Picks up directly from `map_protection_plan.md`'s "Status: critical bug found and fixed via real-map testing" section - that doc's blow-by-blow of *this* session's bug hunt is authoritative; this is the condensed handoff version plus what's still open. Commits: `8c23509` (forces/TRIGSTR fix + MDX crash fix), `504e800` (`.fdf`/`war3mapSkin.txt`/extension-normalization/BTN-DISBTN fixes). **Neither commit is pushed** - `git push` fails with `403: Permission to Lufion1420/HiveWE.git denied to Maurice-TPG` (a credentials/collaborator-access issue on the user's end, not something fixable from in-repo). User pushed the first commit manually themselves after being told about it; the second is still local-only as of this handoff.

### Bugs found and fixed, in the order they were hit

1. **Force/player names showing "Team 1"/"Team 2" instead of custom names.** `inline_map_info_trigger_strings()` (`protection_pipeline.ixx`) only ever resolved `war3map.w3i`'s `name`/`author`/`description`/`loading_screen_*` TRIGSTR references, never `MapInfo::players[]`/`forces[]` names - the World Editor writes a `TRIGSTR_XXX` placeholder for a force's custom name exactly like it does for loading-screen text, and `strip_trigger_strings` deletes `war3map.wts` right after this function runs, leaving those dangling. Fixed by extending the same resolve loop to `info.players`/`info.forces`. Confirmed via direct `war3map.w3i` byte inspection (raw `TRIGSTR_1194` etc. before, real names "Akatsuki"/"Evil"/"Otogakure"/"Konoha" after).

2. **The actual crash: `ACCESS_VIOLATION` immediately after the loading screen, 100% deterministic (identical crash address across multiple exports/builds).** Root-caused via user-supplied WinDbg `!analyze -v` output (`mov r12d, dword ptr [rcx+rax+48h]` with both `rcx`/`rax` = 0 - a null table/array dereference, no further resolution possible since retail WC3 ships no symbols) combined with methodical bisection: the user tested `HIVEWE_MP_EXCLUDE_EXTS` against every asset category one at a time and isolated it to **renaming `.blp`/`.tga` textures specifically** (not `.mdx`, not sounds, not flattening directories). That pointed at `rewrite_mdx_references()`, since renaming a texture forces every model referencing it to go through a full `MDX::to_mdx()` reserialize.
   - Added a temporary round-trip diagnostic (read an MDX, reserialize with zero changes, byte-diff against the original) and had the user run it against their real map's ~600 models. **Virtually every model failed to round-trip byte-for-byte even with nothing changed.** Confirmed root causes: `write_GEOS()` only writes the `TANG`/`SKIN` chunk header when the vector is non-empty (`if (geoset.tangents.size())`), silently dropping an originally-present-but-empty chunk's header on any save (the dominant "-8 bytes" pattern in the diagnostic output); `MDX::validate()`'s zero-extent fix-up and unpreserved trailing bytes after a fixed-size string buffer's null terminator cause further, smaller divergence. Separately, a same-named `.MDL` **text**-format file was being matched by the `.mdx` filter (`asset_match_key()`'s `.mdl->.mdx` normalization, meant only for reference-matching, was leaking into "which files count as binary MDX to parse") and got shredded from ~53KB to a 396-byte empty shell.
   - **Fix:** `rewrite_mdx_textures_in_place()` (new, in `asset_obfuscation.ixx`) - `Texture::file_name` is a fixed, statically-known 260-byte null-padded slot inside a fixed 268-byte `TEXS` record, so a texture rename is now applied as a pure byte-level find/replace directly on the raw file bytes, never parsing or reserializing anything else in the file. Also checks the `MDLX` magic before touching anything, which incidentally fixes the `.MDL` corruption too (non-MDX files are now skipped, not corrupted). Attachment/particle-model paths (rarer - needs another `.mdx` to itself be a rename candidate and be referenced as an attachment/particle model; not implicated by the bisection) still go through the old full-reserialize path.
   - Confirmed fixed in-game: crash gone, forces show real names.

3. **UI showing green (missing-texture) boxes almost everywhere after the crash was fixed.** Root-caused in stages, each one found by directly reading the user's real map/source rather than guessing:
   - `.fdf` (frame-definition) files were never scanned by any phase - `rewrite_fdf_references()` (new) fixes this, reusing the existing JASS/Lua-style quoted-literal scanner (`find_string_literals()`) since FDF's texture-path syntax is just an ordinary quoted string with no special escaping rules of its own.
   - `war3mapSkin.txt`'s `[CustomSkin]` section re-skins *stock* frames (gold/lumber icons, hero attribute icons) via `FrameName=path` lines. The file's own name was already correctly protected from renaming, but nothing rewrote its *contents*. `rewrite_war3mapskin_references()` (new) fixes this with a section-agnostic `key=value` scan (safe because a value is only ever rewritten if it matches a real candidate - the `[FrameDef]` TRIGSTR section below it never does).
   - **Extension mismatch, found directly in the user's `war3mapSkin.txt`:** `InfoPanelIconHeroIconAGI=UI/AttNin.tga` when the real file was `AttNin.blp`. Then, far more widespread: the user's UI is built with Reforged UI Designer (RUI), whose generated Lua (`BlzFrameSetTexture(frame, "UI/Something.png", ...)`) references every frame's texture by its **design-time `.png` filename** - 114 call sites in the compiled `war3map.lua` - even though the map only ever ships the compiled `.blp`. `asset_match_key()` now normalizes both `.tga` and `.png` to `.blp` for matching purposes (previously only had `.mdl->.mdx`), which fixes matching everywhere in the pipeline at once since every rewrite/verify phase routes through this one function.
   - **BTN/DISBTN pairing, the biggest single fix:** WC3 never stores a reference to a button's disabled-state icon anywhere in the map's data - it derives the path at runtime purely from the *enabled* icon's own filename (strip a literal `BTN` prefix, prepend `DISBTN`, look specifically in `ReplaceableTextures\CommandButtonsDisabled\`, regardless of what folder the enabled icon itself lives in). The user's map had 422 `BTN*.blp` icons with 418 matching `DISBTN*.blp` counterparts; every one would get an independent random name from the normal flattening logic, permanently breaking that derivation - showing a green box specifically whenever a button is disabled/on cooldown/not-yet-unlocked (which is why "most" icons looked broken while a few actively-usable ones didn't). Fixed in `enumerate_rename_candidates()`: `ReplaceableTextures/CommandButtonsDisabled/` is carved out as an exception to the "never touch anything under `ReplaceableTextures/`" stock-override rule (that rule exists for genuine exact-path overrides like `Cliff0.blp`, which this isn't), then a pairing pass forces both halves to share one generated identifier - enabled icon flattens to `BTN<hex>.ext` at the root like everything else, disabled one becomes `DISBTN<hex>.ext` but **stays** in its fixed folder.
   - All of the above confirmed fixed in-game via screenshots across several iterations - MainUI, char select, and mode select UI all went from "almost entirely green" to "mostly correct."

### RESOLVED (2026-08-07/08, continued): spell/hero icons via native functions - root cause was extensionless icon paths, not a field-type or level-index gap

Picks up directly from the "Still open" section below (kept verbatim afterward for the record). Root-caused by finally reading the real map's own LNI-exported object data (`ObjectEditorData\table\ability.ini`, previously only pointed at, not opened) instead of another round of the type-classification diagnostic.

**Root cause:** the vast majority of this map's custom ability `art` (Icon - Normal) field values are stored **with no file extension at all** - e.g. `art = "Images/Spells/Sasuke/BTNNCOW_Icon_Sasuke_Katon1"` for a real on-disk file `BTNNCOW_Icon_Sasuke_Katon1.blp` (confirmed by finding that exact file in the dev build folder). This is a normal, legal WC3 convention - the client resolves the extension implicitly, the same behavior HiveWE's own `Texture` loader already relies on (`src/resources/texture.ixx`: `new_path.replace_extension(".tga")` etc., which silently *appends* rather than replaces when the input path has no extension to begin with, then falls back through `.blp`/`.dds`).

`rewrite_object_data_file()`'s candidate lookup, `candidates_by_match_key.find(asset_match_key(value))`, never accounted for this: a `RenameCandidate`'s `match_key` is always built from a real file on disk and therefore always carries a real extension, so an extensionless field value could never match anything. The field was correctly classified as `type == "icon"` (meta resolution never failed) and so was silently skipped at the "no candidate found" branch - a branch the `HIVEWE_MP_TYPE_DEBUG` diagnostic never logged, since that diagnostic only instruments meta-resolution/type-whitelist failures (`NO_META_ID`/`UNHANDLED_TYPE`), not "correctly typed field, value just didn't match." That is exactly why two rounds of that diagnostic against the real map came back empty - working theory #4 below (the diagnostic mechanism has a blind spot, not that no gap exists) was the correct one.

A separately-launched investigation this session (into whether `aart`/`ABILITY_SLF_ICON_NORMAL` might be a per-level field with a level-index mismatch - working theory #3) independently confirmed that's *not* the case: the real stock `AbilityMetaData.slk` has `repeat=0` for the `aart` row, so it's a single-value field on disk regardless of ability level count, and both `field_to_meta_id()` and the shadow-map extraction resolve/rewrite it identically to any other single-value icon field. No level-index mismatch exists in this pipeline. Theories #1 (inherited-from-oldid) and #2 (stale skin-file copy) were never confirmed or needed once the real cause was found.

**Fix**, in `src/map_protector/asset_obfuscation.ixx`: new `find_object_data_candidate(value, type, candidates_by_match_key)` helper, used at both existing match sites in `rewrite_object_data_file()` (the plain single-value branch and the `modelList` comma-segment loop). It first tries the existing exact `asset_match_key()` lookup; if that fails *and* the value has no recognized asset extension (`.blp`/`.tga`/`.png`/`.mdx`/`.mdl`), it retries once with a type-appropriate default extension appended (`.mdx` for `model`/`modelList`, `.blp` for `icon`/`pathingTexture` - mirroring `texture.ixx`'s own fallback order/choice for icons). Two new tests in `tests/asset_obfuscation_test.cpp` cover both the icon (`.blp` default) and model (`.mdx` default) cases using a real extensionless value shape. Full suite: 71 tests (69 + 2 new), 66 passed, same 5 pre-existing unrelated failures (composition: 3 `mdl_reader_test.cpp` MDL-round-trip + 2 `object_data_io_test.cpp` text-parser edge cases this run - same known-flaky categories as before, unrelated to Map Protector).

**Not yet re-verified in-game** - this was root-caused and fixed via static analysis of the real map's own exported data plus a unit test reproducing the exact real value shape, not (yet) a full protect-and-launch round-trip against the user's live map. Next session (or the user, if picking this up standalone) should re-run the protector against the real map and confirm char-select ability previews and command-card icons now show correctly, the same way the BTN/DISBTN and `.fdf`/`war3mapSkin.txt` fixes were confirmed earlier this session.

**Scope note:** the fix is deliberately narrow to `rewrite_object_data_file()`, matching where the evidence pointed (the LNI export showed this pattern specifically in object-data icon fields). Extensionless values were not confirmed (or ruled out) in other reference kinds this pipeline rewrites (FDF, `war3mapSkin.txt`, scripts, MDX/TOC) - if a similar green-box gap turns up in one of those after the in-game re-verification above, check there next before assuming a new, unrelated cause.

**Diagnostics still in the codebase, harmless no-ops unless the env var is set:**
- `HIVEWE_MP_EXCLUDE_EXTS` / `HIVEWE_MP_KEEP_DIRS` in `enumerate_rename_candidates()` (`asset_obfuscation.ixx`) - bisection tooling from the MDX crash hunt, safe to remove now.
- `HIVEWE_MP_TYPE_DEBUG` in `rewrite_object_data_file()` (`asset_obfuscation.ixx`) - the gap it was added to find is now understood and fixed (see above), but it has a real blind spot (only logs classification failures, not match failures) worth knowing about if it's reused for a future issue. Safe to remove once the in-game re-verification above passes, or to keep a while longer as a general-purpose classification-gap detector.

---

### Still open (superseded by the section above - kept verbatim for the record)

Two remaining call sites, both explicitly pointed out by the user (not yet independently root-caused):

- **Char select's ability-preview panel:** `loadSkillsTextures()` in `CharacterSelectUI_Extended.lua` reads `BlzGetAbilityStringLevelField(ability, ABILITY_SLF_ICON_NORMAL, 0)` off a dummy unit created to inspect a hero's spells - a **per-level** ability icon field, not the plain `icon` field this pipeline has mostly been exercised against.
- **MainUI's actual command-card buttons:** per the user, these are the *native* WC3 command-card frames just repositioned (`BlzGetFrameByName("CommandButton", ...)`-style), not custom `BlzFrameSetTexture` calls - so their icon comes directly from whatever the engine reads off the unit's current ability data, same underlying source as the char-select case.

**What's been ruled out, with real evidence, not assumption:** a temporary diagnostic (`HIVEWE_MP_TYPE_DEBUG` env var, in `rewrite_object_data_file()`) logs any object-data field whose *value* looks like a path (ends in `.blp`/`.tga`/`.mdx`/`.mdl`) but that this pipeline's type-classification either (a) can't resolve a meta ID for at all, or (b) resolves to a `type` outside the accepted `{model, icon, modelList, pathingTexture}` set - this is exactly the mechanism that found the `modelList`/`pathingTexture` gaps in earlier sessions. **Run twice against the user's real map (once narrow, once widened to also catch case (a) after realizing the first version couldn't) - the report file was empty both times.** That means every path-like object-data field value in the map *is* being classified as an accepted type and *is* going through the rewrite loop - the bug is not a field-type-whitelist gap like the previous ones were, or if it is, this diagnostic isn't catching it for a reason not yet understood.

**Working theories, none confirmed:**
1. The ability's icon-at-level-0 might not be present in the shadow map's own modification data at all if it's inherited unmodified from the `oldid` parent stock ability (custom Object Editor UI sometimes only shows *effective* values, not what's actually stored as an override) - if so, `rewrite_object_data_file()` would never see that field to rewrite, since it only iterates fields explicitly present in the shadow map.
2. `war3mapSkin.w3a` (the skin-file counterpart) might hold a stale/unrewritten copy of the same field that takes precedence at render time over the correctly-rewritten main file - worth checking whether the skin file's version of the same field actually changed.
3. `BlzGetAbilityStringLevelField` with `ABILITY_SLF_ICON_NORMAL` might resolve to a genuinely different underlying storage/field-code than what HiveWE's Object Editor UI exposes as "the icon field" - i.e. a field-code/alias mismatch specific to this native, distinct from the type-classification issue the diagnostic checks for.
4. Baseline sanity check not yet done: verify the diagnostic mechanism itself actually fires under real conditions (e.g. temporarily force it to log *every* icon-type field unconditionally, confirm the report file is non-empty at all) - an empty report is consistent with "no gap exists" but also with "the diagnostic isn't actually running/writing," and that's not yet been distinguished.

### Key files touched this session

| File | What changed |
|---|---|
| `src/map_protector/protection_pipeline.ixx` | Forces/players TRIGSTR fix; wired `rewrite_fdf_references()`/`rewrite_war3mapskin_references()` into `run_asset_obfuscation()`'s pipeline |
| `src/map_protector/asset_obfuscation.ixx` | `rewrite_mdx_textures_in_place()` (surgical TEXS patch), `rewrite_fdf_references()`, `rewrite_war3mapskin_references()`, `asset_match_key()` `.tga`/`.png` normalization, BTN/DISBTN pairing pass in `enumerate_rename_candidates()`, both temporary diagnostics, `find_object_data_candidate()` (extensionless icon/model value fallback, added later in this same session - see "RESOLVED" section above) |
| `tests/asset_obfuscation_test.cpp` | 9 new tests covering all of the above except the MDX crash fix itself (that one was only verified against real production models via the temporary round-trip diagnostic, not a unit test - the existing `rewrite_mdx_references` tests use a small synthetic fixture that already round-tripped fine), plus 2 more added later in this session for the extensionless-value fallback |

Full test suite as of `504e800`: 69 tests, 64 passed, same 5 pre-existing unrelated failures (`object_data_io_test.cpp` x4, `mdl_reader_test.cpp` x1 - predate this entire feature, unrelated to Map Protector). As of the extensionless-icon fix (later in this session, not yet committed as its own hash at time of writing): 71 tests, 66 passed, same 5 pre-existing failures (composition shifted slightly across runs - 3 `mdl_reader_test.cpp` + 2 `object_data_io_test.cpp` this run - but still the same known-flaky categories, unrelated to Map Protector).

### Real-map reference paths used for investigation this session (user's own project, read-only)

These were essential for root-causing the UI issues - static analysis of the exported map alone wasn't enough, several bugs were only found by reading the user's actual UI source and comparing real file names against what's referenced:
- `D:\Stuff\Warcraft_3_All_Assets\Projects\Naruto Clash of Worlds Content\Map\NCOW-Arena\src\Custom-UI\` - RUI-exported Lua UI source (`MainUI.lua`/`MainUIExtended.lua`, `CharSelect\CharacterSelectUI.lua`/`_Extended.lua`, etc.) - "Extended" files add manipulation logic on top of the plain RUI export.
- `D:\Stuff\Warcraft_3_All_Assets\Projects\Naruto Clash of Worlds Content\Map\NCOW-Arena\NCOW_Spell_Dev.w3x\` - an unpacked (folder-form) dev build of the map; useful for cross-checking real file names/extensions against script references, but **not necessarily 1:1 with what actually gets tested** - its compiled `war3map.lua` was found to be stale/missing the Custom-UI code entirely during this session, while the actual tested map (`_warcraft_vscode_test.w3x`, `C:\Users\<user>\Documents\Warcraft III\Maps\Download\NCOW\`) had it. Prefer the actually-tested map's own compiled output over this folder when they might disagree.
- `D:\Stuff\Warcraft_3_All_Assets\Projects\Naruto Clash of Worlds Content\Map\NCOW-Arena\ObjectEditorData\table\` - HiveWE's own LNI-exported object editor data (`ability.ini`, `unit.ini`, etc.). **Now inspected** - this is what actually found the extensionless-icon root cause above; `ability.ini`'s `art =` lines directly showed the bare-path-with-no-extension pattern for the large majority of custom ability icons, which a byte-diff/diagnostic-log approach against the binary `.w3a` alone hadn't surfaced. Worth checking this file directly, early, for any future object-data-adjacent Map Protector bug on this map.
- `C:\Users\<user>\Documents\Warcraft III\Errors\` - crash-dump folders (`Crash.txt`, `War3.dmp`, `War3Log.txt`); `War3Log.txt` in particular sometimes has a "model creation failed - <path>" line pinpointing an exact failing asset load in real time.
