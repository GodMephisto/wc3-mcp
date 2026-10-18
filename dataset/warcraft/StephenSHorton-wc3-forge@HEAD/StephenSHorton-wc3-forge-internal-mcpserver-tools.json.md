---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/mcpserver/tools.json"
ref: HEAD
---

[
  {
    "name": "bridge_ping",
    "description": "Ping the currently-selected wc3-forge bridge. Returns version + map_loaded flag.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "bridge.ping"
  },
  {
    "name": "camera_set_view",
    "description": "Pan the camera to (x, y, z) game coordinates. Pass distance > 0 to also set zoom; omit (or 0) to leave distance untouched.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "x": {
          "type": "number"
        },
        "y": {
          "type": "number"
        },
        "z": {
          "type": "number"
        },
        "distance": {
          "type": "number"
        }
      },
      "required": [
        "x",
        "y",
        "z"
      ],
      "additionalProperties": false
    },
    "method": "camera.set_view"
  },
  {
    "name": "diagnostics_arm",
    "description": "Arm or disarm the viewport's diagnostics mode remotely (same as pressing F9). When armed, the per-frame GL-error stage attribution and other expensive probes run, and the diagnostics overlay shows. Typical flow: arm → reproduce the issue → diagnostics_get → disarm. Pass { on: true } to arm, { on: false } to disarm.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "on": {
          "type": "boolean",
          "description": "true to arm diagnostics mode, false to disarm"
        }
      },
      "required": [
        "on"
      ],
      "additionalProperties": false
    },
    "method": "diagnostics.arm"
  },
  {
    "name": "diagnostics_get",
    "description": "Read the running viewport's live render diagnostics (pushed ~5Hz from the frontend): frame counter, fps, camera eye/pivot/distance/yaw/pitch, doodad/unit/visible counts, terrain dimensions + center offset, sky/water flags, last GL error + the faulting render stage, GPU renderer string, and document focus/visibility. The response also carries age_ms (snapshot staleness); a large age_ms means the render loop or reporting has stalled. Lets an agent inspect the editor's actual on-screen state without a screenshot. No params.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "diagnostics.get"
  },
  {
    "name": "doodads_create",
    "description": "Create a new placed doodad/destructable in war3map.doo. x/y are WC3 world units (origin = map center). Returns { creation_number }. Undo-aware; emits the standard change events.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "type_id": {
          "type": "string",
          "description": "4-char doodad/destructable rawcode"
        },
        "x": {
          "type": "number",
          "description": "world X (origin = map center)"
        },
        "y": {
          "type": "number",
          "description": "world Y (origin = map center)"
        },
        "z": {
          "type": "number",
          "description": "world Z (default 0 / terrain height)"
        },
        "rotation": {
          "type": "number",
          "description": "facing angle in radians; default 0"
        },
        "scale": {
          "type": "number",
          "description": "uniform scale (default 1.0)"
        },
        "variation": {
          "type": "integer",
          "description": "doodad variation index (default 0)"
        }
      },
      "required": [
        "type_id",
        "x",
        "y"
      ],
      "additionalProperties": false
    },
    "method": "doodads.create"
  },
  {
    "name": "doodads_delete",
    "description": "Delete a placed doodad/destructable by creation_number. Returns { ok: true }. Undo-aware; emits the standard change events.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "creation_number": {
          "type": "integer",
          "description": "creation_number from doodads_list"
        }
      },
      "required": [
        "creation_number"
      ],
      "additionalProperties": false
    },
    "method": "doodads.delete"
  },
  {
    "name": "doodads_get",
    "description": "Read a single placed doodad by creation_number.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "creation_number": {
          "type": "integer"
        }
      },
      "required": [
        "creation_number"
      ],
      "additionalProperties": false
    },
    "method": "doodads.get"
  },
  {
    "name": "doodads_list",
    "description": "List placed doodads + destructibles from war3map.doo.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "doodads.list"
  },
  {
    "name": "doodads_move",
    "description": "Move a placed doodad to (x, y, z) in WC3 game coordinates.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "creation_number": {
          "type": "integer"
        },
        "x": {
          "type": "number"
        },
        "y": {
          "type": "number"
        },
        "z": {
          "type": "number"
        }
      },
      "required": [
        "creation_number",
        "x",
        "y",
        "z"
      ],
      "additionalProperties": false
    },
    "method": "doodads.move"
  },
  {
    "name": "doodads_rotate",
    "description": "Set a doodad's facing angle, in radians around the Z axis.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "creation_number": {
          "type": "integer"
        },
        "rotation": {
          "type": "number"
        }
      },
      "required": [
        "creation_number",
        "rotation"
      ],
      "additionalProperties": false
    },
    "method": "doodads.rotate"
  },
  {
    "name": "doodads_scale",
    "description": "Set per-axis scale on a doodad. Note: doodad scale is stored raw on disk (no /128 divide that units use).",
    "inputSchema": {
      "type": "object",
      "properties": {
        "creation_number": {
          "type": "integer"
        },
        "sx": {
          "type": "number"
        },
        "sy": {
          "type": "number"
        },
        "sz": {
          "type": "number"
        }
      },
      "required": [
        "creation_number",
        "sx",
        "sy",
        "sz"
      ],
      "additionalProperties": false
    },
    "method": "doodads.scale"
  },
  {
    "name": "gameplay_constants_apply",
    "description": "Replace the loaded map's gameplay-constant overrides (war3mapMisc.txt) with the given FULL row set (snapshot replace, not a diff). Rows are written in order; an empty rows array clears all overrides. Returns { ok: true, rows_applied }. Marks the map dirty (Save to persist).",
    "inputSchema": {
      "type": "object",
      "properties": {
        "rows": {
          "type": "array",
          "items": {
            "type": "object",
            "properties": {
              "section": {
                "type": "string",
                "description": "ini section (default 'Misc')"
              },
              "key": {
                "type": "string",
                "description": "constant key, e.g. 'DamageBonusNormal'"
              },
              "value": {
                "type": "string",
                "description": "raw value string (comma-separated for list-valued)"
              }
            },
            "required": [
              "key",
              "value"
            ],
            "additionalProperties": false
          },
          "description": "full post-edit row set"
        }
      },
      "required": [
        "rows"
      ],
      "additionalProperties": false
    },
    "method": "gameplay_constants.apply"
  },
  {
    "name": "gameplay_constants_get",
    "description": "Get the loaded map's per-map gameplay-constant overrides (war3mapMisc.txt). Returns { rows: [{ section, key, value }, ...] } — value is the raw stored string (comma-separated verbatim for list-valued constants like DamageBonusNormal). Empty rows for a map that ships no overrides.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "gameplay_constants.get"
  },
  {
    "name": "history_abort_group",
    "description": "Discard an open undo group without closing it normally: collapses any nesting to depth 0 and re-enables Undo/Redo. Recovery for a dangling group (e.g. an agent began a group then crashed/cancelled before history_end_group, wedging undo/redo). The already-applied mutations stay in place — this drops only the group wrapper, it does not roll back. Errors if no group is open. Detect the wedge via history_list's open_group_depth.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "history.abort_group"
  },
  {
    "name": "history_begin_group",
    "description": "Start an undo transaction. Subsequent mutations until history_end_group land in one undo step. Nested begins increment depth — match each with an end.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "label": {
          "type": "string"
        }
      },
      "additionalProperties": false
    },
    "method": "history.begin_group"
  },
  {
    "name": "history_end_group",
    "description": "Close the outermost undo group (or decrement nesting depth).",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "history.end_group"
  },
  {
    "name": "history_list",
    "description": "Return the current undo/redo stacks (oldest-first) with their labels, plus open_group_depth (the current undo-group nesting depth; >0 means a group is open and Undo/Redo are blocked until it is closed with history_end_group or discarded with history_abort_group).",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "history.list"
  },
  {
    "name": "history_redo",
    "description": "Re-apply the most recently undone mutation.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "history.redo"
  },
  {
    "name": "history_undo",
    "description": "Revert the most recent mutation. Returns { ok, label } where label describes what was undone.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "history.undo"
  },
  {
    "name": "imports_add",
    "description": "Add (or overwrite) an arbitrary file in the loaded map's archive and register it in war3map.imp. Use for custom icons (BLP), loading screens, sounds, replacement textures at stock paths, etc. path is the in-archive destination (forward or back slashes accepted; e.g. 'war3mapImported\\BTNFoo.blp' or a stock path to override). data_base64 is the file's bytes base64-encoded ('' imports a zero-byte placeholder). Returns { path } (the normalized in-archive path). Re-adding the same path overwrites the bytes. Persists on save.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "path": {
          "type": "string",
          "description": "in-archive destination path, e.g. 'war3mapImported\\\\BTNFoo.blp'"
        },
        "data_base64": {
          "type": "string",
          "description": "file bytes, base64-encoded (omit/empty for a zero-byte placeholder)"
        }
      },
      "required": [
        "path"
      ],
      "additionalProperties": false
    },
    "method": "imports.add"
  },
  {
    "name": "imports_list",
    "description": "List every file registered in the loaded map's war3map.imp import table. Returns { imports: [{ path, flag, size, present }, ...] } — path is the in-archive path (e.g. war3mapImported\\foo.blp), flag is the path-kind byte, size is the on-source byte length, present is false for an orphaned table entry whose backing file is missing.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "imports.list"
  },
  {
    "name": "imports_remove",
    "description": "Remove an imported file from the loaded map's archive AND its war3map.imp entry. path is case- and slash-insensitive. Returns { ok: true }. Persists on save.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "path": {
          "type": "string",
          "description": "in-archive path to remove (from imports_list)"
        }
      },
      "required": [
        "path"
      ],
      "additionalProperties": false
    },
    "method": "imports.remove"
  },
  {
    "name": "imports_rename",
    "description": "Rename/move an imported file from old_path to new_path in the archive + war3map.imp (preserving its flag byte). Returns { ok: true }. Errors if old_path isn't present. Persists on save.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "old_path": {
          "type": "string",
          "description": "current in-archive path (from imports_list)"
        },
        "new_path": {
          "type": "string",
          "description": "new in-archive path"
        }
      },
      "required": [
        "old_path",
        "new_path"
      ],
      "additionalProperties": false
    },
    "method": "imports.rename"
  },
  {
    "name": "map_close",
    "description": "Close the currently-loaded map. No-op if nothing is loaded.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "map.close"
  },
  {
    "name": "map_extract_to_folder",
    "description": "Unpack the current MPQ-backed (.w3x) map into a NEW folder at path (must not already exist), writing every listed archive file as loose files. Then map_open(path) to switch to the folder workflow (where map_save writes directly, no MPQ repack). Errors if the map is already folder-backed. Returns { ok, path }.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "path": {
          "type": "string",
          "description": "destination folder (must not already exist)"
        }
      },
      "required": [
        "path"
      ],
      "additionalProperties": false
    },
    "method": "map.extract_to_folder"
  },
  {
    "name": "map_info_get",
    "description": "Read war3map.w3i (name, author, description, suggested_players, dimensions, players, …).",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "map.info_get"
  },
  {
    "name": "map_info_set",
    "description": "Partial-update map_info. Currently supported keys: name, author, description, suggestedPlayers (strings), lua (bool). Wire shape: { updates: { …keys } }; returns { changed_fields: N }. Recorded on the undo history — reversible via history_undo. On a localized map (where these fields are stored as TRIGSTR_<n> tokens referencing war3map.wts), editing one updates the referenced wts entry and keeps the token in w3i — it no longer orphans the entry — and war3map.wts is re-written on save.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "updates": {
          "type": "object",
          "additionalProperties": {},
          "description": "Subset of {name, author, description, suggestedPlayers, lua}"
        }
      },
      "required": [
        "updates"
      ],
      "additionalProperties": false
    },
    "method": "map.info_set"
  },
  {
    "name": "map_new",
    "description": "Create a brand-new map at path (a .w3x) and open it. A flat single-tileset square with no units/doodads (classic-TFT format: w3i v25, w3e v11). width/height are playable tile counts (8–480); tileset is the single-letter code (e.g. 'L' = Lordaeron Summer). Errors if path already exists. Returns { ok, path, name, width, height }.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "name": {
          "type": "string",
          "description": "map display name (also the lobby/HM3W name)"
        },
        "width": {
          "type": "integer",
          "description": "playable width in tiles (8–480)"
        },
        "height": {
          "type": "integer",
          "description": "playable height in tiles (8–480)"
        },
        "tileset": {
          "type": "string",
          "description": "single-letter tileset code, e.g. 'L' (Lordaeron Summer)"
        },
        "path": {
          "type": "string",
          "description": "destination .w3x path (must not already exist)"
        }
      },
      "required": [
        "name",
        "width",
        "height",
        "tileset",
        "path"
      ],
      "additionalProperties": false
    },
    "method": "map.new"
  },
  {
    "name": "map_open",
    "description": "Load a map into wc3-forge. Accepts an extracted folder OR a .w3x / .w3m / .mpq archive path.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "path": {
          "type": "string",
          "description": "Absolute path to a map folder or .w3x/.w3m/.mpq archive"
        }
      },
      "required": [
        "path"
      ],
      "additionalProperties": false
    },
    "method": "map.open"
  },
  {
    "name": "map_save",
    "description": "Save the current map in place. Folder-backed maps commit all-or-nothing (each file staged to a temp, then atomically renamed, with a .bak of the prior bytes); MPQ-backed maps repack the whole .w3x atomically. By default the save is REFUSED with a 'changed on disk' error if any target file was modified since the map was opened (another wc3-forge instance, agent, or human saved underneath you) — your unsaved edits are preserved. Pass force=true to overwrite anyway (prior bytes still backed up to .bak).",
    "inputSchema": {
      "type": "object",
      "properties": {
        "force": {
          "type": "boolean",
          "description": "overwrite even if a target file changed on disk since the map was opened (default false → refuse + report the conflict)"
        }
      },
      "additionalProperties": false
    },
    "method": "map.save"
  },
  {
    "name": "map_save_as",
    "description": "Flush in-memory edits, then export the current map as a standalone .w3x at path and repoint the session there (subsequent saves go to the new path). path must end in .w3x or .w3m. Folder-backed maps are packaged; MPQ-backed maps are copied. Returns { ok, path }.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "path": {
          "type": "string",
          "description": "destination .w3x/.w3m path"
        }
      },
      "required": [
        "path"
      ],
      "additionalProperties": false
    },
    "method": "map.save_as"
  },
  {
    "name": "map_status",
    "description": "Status of the currently-loaded map (loaded flag, path, name, unit count).",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "map.status"
  },
  {
    "name": "minimap_bake",
    "description": "Bake a minimap from the loaded map's terrain (one pixel per cell, colored by ground tile + water). Returns { ok, ext:'png', bytes_base64, width, height, png_path, persisted, blp_path? }. png_path is a temp PNG file the agent can OPEN to self-verify the result visually. Set persist=true to also write the baked image as war3mapMap.blp into the map (marks dirty; Save to persist on disk).",
    "inputSchema": {
      "type": "object",
      "properties": {
        "persist": {
          "type": "boolean",
          "description": "also write war3mapMap.blp into the map (default false)"
        }
      },
      "additionalProperties": false
    },
    "method": "minimap.bake"
  },
  {
    "name": "models_import",
    "description": "Convert a 3D model file (.obj/.gltf/.glb/.stl) to a minimal WC3 MDX with baked BLP textures and import both into the loaded map under war3mapImported\\. Optionally assigns the imported model to an object's 'file' field. Returns { model_path, texture_paths, warnings }.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "source_path": {
          "type": "string",
          "description": "Absolute path to the source .obj/.gltf/.glb/.stl file"
        },
        "scale": {
          "type": "number",
          "description": "Uniform scale applied to the mesh (default 1.0)"
        },
        "up_axis": {
          "type": "string",
          "enum": [
            "y",
            "z",
            "auto"
          ],
          "description": "Source up-axis: 'y' (OBJ/glTF default, remaps to Z-up), 'z' (identity), or 'auto'"
        },
        "flip_v": {
          "type": "boolean",
          "description": "Flip texture V coordinate (v -> 1-v) for sources with bottom-origin UVs"
        },
        "target_object_kind": {
          "type": "string",
          "description": "Optional: object kind to assign the model to (units/items/abilities/buffs/destructables/doodads/upgrades)"
        },
        "target_object_id": {
          "type": "string",
          "description": "Optional: 4-char rawcode of the object to set 'file' on (requires target_object_kind)"
        }
      },
      "required": [
        "source_path"
      ],
      "additionalProperties": false
    },
    "method": "models.import"
  },
  {
    "name": "objects_abilities_create_custom",
    "description": "Create a new custom abilities definition cloned from a base object. Returns { id, detail } where id is the new custom's rawcode. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "base_id": {
          "type": "string",
          "description": "4-char rawcode of the base object to clone from"
        },
        "id": {
          "type": "string",
          "description": "optional explicit 4-char rawcode for the new custom; auto-assigned when omitted"
        }
      },
      "required": [
        "base_id"
      ],
      "additionalProperties": false
    },
    "method": "objects.abilities.create_custom"
  },
  {
    "name": "objects_abilities_delete_custom",
    "description": "Delete a custom abilities definition by id (4-char rawcode). Only custom objects can be deleted; stock rows error. Undo-aware. Returns { ok: true }.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the custom abilities object to delete"
        }
      },
      "required": [
        "id"
      ],
      "additionalProperties": false
    },
    "method": "objects.abilities.delete_custom"
  },
  {
    "name": "objects_abilities_fields_meta",
    "description": "Return the field schema (MetaData) for abilities definitions: id, field (SLK column), display_name, category, type, min_val, max_val. Drives a dynamic editor UI.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "objects.abilities.fields_meta"
  },
  {
    "name": "objects_abilities_get",
    "description": "Return the full field map for one abilities definition by id (4-char rawcode). Includes raw values, display-resolved values (with and without WC3 color codes), per-field category/type metadata, and the model/icon paths.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the abilities object"
        }
      },
      "required": [
        "id"
      ],
      "additionalProperties": false
    },
    "method": "objects.abilities.get"
  },
  {
    "name": "objects_abilities_list",
    "description": "List all abilities definitions (stock SLK rows + per-map object-data overrides). Returns id, name, race, kind, category, is_custom, is_edited, base_id, icon_art — flat rows the UI groups into a tree client-side.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "objects.abilities.list"
  },
  {
    "name": "objects_abilities_set_field",
    "description": "Set one field on a abilities definition (writes the per-map object-data shadow; undo-aware). Re-emits the post-mutation get payload. Pass an optional 'level' for leveled fields (e.g. ability levels); omit for non-leveled fields.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the abilities object to edit"
        },
        "column": {
          "type": "string",
          "description": "SLK column name (lowercased), e.g. 'name', 'hp'"
        },
        "value": {
          "type": "string",
          "description": "raw cell value to write (stringified)"
        },
        "level": {
          "type": "integer",
          "description": "optional 1-based level for leveled fields (ability/upgrade); omit for flat fields"
        }
      },
      "required": [
        "id",
        "column",
        "value"
      ],
      "additionalProperties": false
    },
    "method": "objects.abilities.set_field"
  },
  {
    "name": "objects_buffs_create_custom",
    "description": "Create a new custom buffs definition cloned from a base object. Returns { id, detail } where id is the new custom's rawcode. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "base_id": {
          "type": "string",
          "description": "4-char rawcode of the base object to clone from"
        },
        "id": {
          "type": "string",
          "description": "optional explicit 4-char rawcode for the new custom; auto-assigned when omitted"
        }
      },
      "required": [
        "base_id"
      ],
      "additionalProperties": false
    },
    "method": "objects.buffs.create_custom"
  },
  {
    "name": "objects_buffs_delete_custom",
    "description": "Delete a custom buffs definition by id (4-char rawcode). Only custom objects can be deleted; stock rows error. Undo-aware. Returns { ok: true }.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the custom buffs object to delete"
        }
      },
      "required": [
        "id"
      ],
      "additionalProperties": false
    },
    "method": "objects.buffs.delete_custom"
  },
  {
    "name": "objects_buffs_fields_meta",
    "description": "Return the field schema (MetaData) for buffs definitions: id, field (SLK column), display_name, category, type, min_val, max_val. Drives a dynamic editor UI.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "objects.buffs.fields_meta"
  },
  {
    "name": "objects_buffs_get",
    "description": "Return the full field map for one buffs definition by id (4-char rawcode). Includes raw values, display-resolved values (with and without WC3 color codes), per-field category/type metadata, and the model/icon paths.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the buffs object"
        }
      },
      "required": [
        "id"
      ],
      "additionalProperties": false
    },
    "method": "objects.buffs.get"
  },
  {
    "name": "objects_buffs_list",
    "description": "List all buffs definitions (stock SLK rows + per-map object-data overrides). Returns id, name, race, kind, category, is_custom, is_edited, base_id, icon_art — flat rows the UI groups into a tree client-side.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "objects.buffs.list"
  },
  {
    "name": "objects_buffs_set_field",
    "description": "Set one field on a buffs definition (writes the per-map object-data shadow; undo-aware). Re-emits the post-mutation get payload. Pass an optional 'level' for leveled fields (e.g. ability levels); omit for non-leveled fields.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the buffs object to edit"
        },
        "column": {
          "type": "string",
          "description": "SLK column name (lowercased), e.g. 'name', 'hp'"
        },
        "value": {
          "type": "string",
          "description": "raw cell value to write (stringified)"
        },
        "level": {
          "type": "integer",
          "description": "optional 1-based level for leveled fields (ability/upgrade); omit for flat fields"
        }
      },
      "required": [
        "id",
        "column",
        "value"
      ],
      "additionalProperties": false
    },
    "method": "objects.buffs.set_field"
  },
  {
    "name": "objects_convert",
    "description": "Convert one object definition to another kind, creating a new custom in the destination kind. Today only doodads↔destructables is accepted. Returns { id, detail } for the newly-created custom.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "src_kind": {
          "type": "string",
          "enum": [
            "units",
            "items",
            "abilities",
            "buffs",
            "destructables",
            "doodads",
            "upgrades"
          ],
          "description": "source kind, e.g. 'doodads'"
        },
        "src_id": {
          "type": "string",
          "description": "4-char rawcode of the source object"
        },
        "dst_kind": {
          "type": "string",
          "enum": [
            "units",
            "items",
            "abilities",
            "buffs",
            "destructables",
            "doodads",
            "upgrades"
          ],
          "description": "destination kind, e.g. 'destructables'"
        }
      },
      "required": [
        "src_kind",
        "src_id",
        "dst_kind"
      ],
      "additionalProperties": false
    },
    "method": "objects.convert"
  },
  {
    "name": "objects_destructables_create_custom",
    "description": "Create a new custom destructables definition cloned from a base object. Returns { id, detail } where id is the new custom's rawcode. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "base_id": {
          "type": "string",
          "description": "4-char rawcode of the base object to clone from"
        },
        "id": {
          "type": "string",
          "description": "optional explicit 4-char rawcode for the new custom; auto-assigned when omitted"
        }
      },
      "required": [
        "base_id"
      ],
      "additionalProperties": false
    },
    "method": "objects.destructables.create_custom"
  },
  {
    "name": "objects_destructables_delete_custom",
    "description": "Delete a custom destructables definition by id (4-char rawcode). Only custom objects can be deleted; stock rows error. Undo-aware. Returns { ok: true }.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the custom destructables object to delete"
        }
      },
      "required": [
        "id"
      ],
      "additionalProperties": false
    },
    "method": "objects.destructables.delete_custom"
  },
  {
    "name": "objects_destructables_fields_meta",
    "description": "Return the field schema (MetaData) for destructables definitions: id, field (SLK column), display_name, category, type, min_val, max_val. Drives a dynamic editor UI.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "objects.destructables.fields_meta"
  },
  {
    "name": "objects_destructables_get",
    "description": "Return the full field map for one destructables definition by id (4-char rawcode). Includes raw values, display-resolved values (with and without WC3 color codes), per-field category/type metadata, and the model/icon paths.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the destructables object"
        }
      },
      "required": [
        "id"
      ],
      "additionalProperties": false
    },
    "method": "objects.destructables.get"
  },
  {
    "name": "objects_destructables_list",
    "description": "List all destructables definitions (stock SLK rows + per-map object-data overrides). Returns id, name, race, kind, category, is_custom, is_edited, base_id, icon_art — flat rows the UI groups into a tree client-side.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "objects.destructables.list"
  },
  {
    "name": "objects_destructables_set_field",
    "description": "Set one field on a destructables definition (writes the per-map object-data shadow; undo-aware). Re-emits the post-mutation get payload. Pass an optional 'level' for leveled fields (e.g. ability levels); omit for non-leveled fields.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the destructables object to edit"
        },
        "column": {
          "type": "string",
          "description": "SLK column name (lowercased), e.g. 'name', 'hp'"
        },
        "value": {
          "type": "string",
          "description": "raw cell value to write (stringified)"
        },
        "level": {
          "type": "integer",
          "description": "optional 1-based level for leveled fields (ability/upgrade); omit for flat fields"
        }
      },
      "required": [
        "id",
        "column",
        "value"
      ],
      "additionalProperties": false
    },
    "method": "objects.destructables.set_field"
  },
  {
    "name": "objects_doodads_create_custom",
    "description": "Create a new custom doodads definition cloned from a base object. Returns { id, detail } where id is the new custom's rawcode. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "base_id": {
          "type": "string",
          "description": "4-char rawcode of the base object to clone from"
        },
        "id": {
          "type": "string",
          "description": "optional explicit 4-char rawcode for the new custom; auto-assigned when omitted"
        }
      },
      "required": [
        "base_id"
      ],
      "additionalProperties": false
    },
    "method": "objects.doodads.create_custom"
  },
  {
    "name": "objects_doodads_delete_custom",
    "description": "Delete a custom doodads definition by id (4-char rawcode). Only custom objects can be deleted; stock rows error. Undo-aware. Returns { ok: true }.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the custom doodads object to delete"
        }
      },
      "required": [
        "id"
      ],
      "additionalProperties": false
    },
    "method": "objects.doodads.delete_custom"
  },
  {
    "name": "objects_doodads_fields_meta",
    "description": "Return the field schema (MetaData) for doodads definitions: id, field (SLK column), display_name, category, type, min_val, max_val. Drives a dynamic editor UI.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "objects.doodads.fields_meta"
  },
  {
    "name": "objects_doodads_get",
    "description": "Return the full field map for one doodads definition by id (4-char rawcode). Includes raw values, display-resolved values (with and without WC3 color codes), per-field category/type metadata, and the model/icon paths.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the doodads object"
        }
      },
      "required": [
        "id"
      ],
      "additionalProperties": false
    },
    "method": "objects.doodads.get"
  },
  {
    "name": "objects_doodads_list",
    "description": "List all doodads definitions (stock SLK rows + per-map object-data overrides). Returns id, name, race, kind, category, is_custom, is_edited, base_id, icon_art — flat rows the UI groups into a tree client-side.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "objects.doodads.list"
  },
  {
    "name": "objects_doodads_set_field",
    "description": "Set one field on a doodads definition (writes the per-map object-data shadow; undo-aware). Re-emits the post-mutation get payload. Pass an optional 'level' for leveled fields (e.g. ability levels); omit for non-leveled fields.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the doodads object to edit"
        },
        "column": {
          "type": "string",
          "description": "SLK column name (lowercased), e.g. 'name', 'hp'"
        },
        "value": {
          "type": "string",
          "description": "raw cell value to write (stringified)"
        },
        "level": {
          "type": "integer",
          "description": "optional 1-based level for leveled fields (ability/upgrade); omit for flat fields"
        }
      },
      "required": [
        "id",
        "column",
        "value"
      ],
      "additionalProperties": false
    },
    "method": "objects.doodads.set_field"
  },
  {
    "name": "objects_import_from_map",
    "description": "Faithfully copy objects from another WC3 map into the loaded map, following the dependency graph (an ability's buffs, the units it summons and their abilities, imported icon/model files). Custom objects keep their source FourCC; ids already present in the target are skipped. Undo-aware (one group). Returns { imported, skipped, missing, imported_files, failed } (each a list of 'kind:id').",
    "inputSchema": {
      "type": "object",
      "properties": {
        "source_map_path": {
          "type": "string",
          "description": "absolute path to the source .w3x/.w3m/.mpq archive or extracted map folder"
        },
        "objects": {
          "type": "array",
          "items": {
            "type": "object",
            "properties": {
              "kind": {
                "type": "string",
                "enum": [
                  "units",
                  "items",
                  "abilities",
                  "buffs",
                  "destructables",
                  "doodads",
                  "upgrades"
                ],
                "description": "object kind, e.g. 'abilities'"
              },
              "id": {
                "type": "string",
                "description": "4-char rawcode of the object in the source map"
              }
            },
            "required": [
              "kind",
              "id"
            ],
            "additionalProperties": false
          },
          "description": "the objects to import (their dependencies are pulled in automatically)"
        },
        "on_collision": {
          "type": "string",
          "enum": [
            "skip",
            "remap"
          ],
          "description": "when a source FourCC is already present in the target: 'skip' (default, keep the existing object) or 'remap' (give the import a fresh id and rewrite references)"
        }
      },
      "required": [
        "source_map_path",
        "objects"
      ],
      "additionalProperties": false
    },
    "method": "objects.import_from_map"
  },
  {
    "name": "objects_items_create_custom",
    "description": "Create a new custom items definition cloned from a base object. Returns { id, detail } where id is the new custom's rawcode. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "base_id": {
          "type": "string",
          "description": "4-char rawcode of the base object to clone from"
        },
        "id": {
          "type": "string",
          "description": "optional explicit 4-char rawcode for the new custom; auto-assigned when omitted"
        }
      },
      "required": [
        "base_id"
      ],
      "additionalProperties": false
    },
    "method": "objects.items.create_custom"
  },
  {
    "name": "objects_items_delete_custom",
    "description": "Delete a custom items definition by id (4-char rawcode). Only custom objects can be deleted; stock rows error. Undo-aware. Returns { ok: true }.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the custom items object to delete"
        }
      },
      "required": [
        "id"
      ],
      "additionalProperties": false
    },
    "method": "objects.items.delete_custom"
  },
  {
    "name": "objects_items_fields_meta",
    "description": "Return the field schema (MetaData) for items definitions: id, field (SLK column), display_name, category, type, min_val, max_val. Drives a dynamic editor UI.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "objects.items.fields_meta"
  },
  {
    "name": "objects_items_get",
    "description": "Return the full field map for one items definition by id (4-char rawcode). Includes raw values, display-resolved values (with and without WC3 color codes), per-field category/type metadata, and the model/icon paths.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the items object"
        }
      },
      "required": [
        "id"
      ],
      "additionalProperties": false
    },
    "method": "objects.items.get"
  },
  {
    "name": "objects_items_list",
    "description": "List all items definitions (stock SLK rows + per-map object-data overrides). Returns id, name, race, kind, category, is_custom, is_edited, base_id, icon_art — flat rows the UI groups into a tree client-side.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "objects.items.list"
  },
  {
    "name": "objects_items_set_field",
    "description": "Set one field on a items definition (writes the per-map object-data shadow; undo-aware). Re-emits the post-mutation get payload. Pass an optional 'level' for leveled fields (e.g. ability levels); omit for non-leveled fields.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the items object to edit"
        },
        "column": {
          "type": "string",
          "description": "SLK column name (lowercased), e.g. 'name', 'hp'"
        },
        "value": {
          "type": "string",
          "description": "raw cell value to write (stringified)"
        },
        "level": {
          "type": "integer",
          "description": "optional 1-based level for leveled fields (ability/upgrade); omit for flat fields"
        }
      },
      "required": [
        "id",
        "column",
        "value"
      ],
      "additionalProperties": false
    },
    "method": "objects.items.set_field"
  },
  {
    "name": "objects_units_create_custom",
    "description": "Create a new custom units definition cloned from a base object. Returns { id, detail } where id is the new custom's rawcode. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "base_id": {
          "type": "string",
          "description": "4-char rawcode of the base object to clone from"
        },
        "id": {
          "type": "string",
          "description": "optional explicit 4-char rawcode for the new custom; auto-assigned when omitted"
        }
      },
      "required": [
        "base_id"
      ],
      "additionalProperties": false
    },
    "method": "objects.units.create_custom"
  },
  {
    "name": "objects_units_delete_custom",
    "description": "Delete a custom units definition by id (4-char rawcode). Only custom objects can be deleted; stock rows error. Undo-aware. Returns { ok: true }.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the custom units object to delete"
        }
      },
      "required": [
        "id"
      ],
      "additionalProperties": false
    },
    "method": "objects.units.delete_custom"
  },
  {
    "name": "objects_units_fields_meta",
    "description": "Return the field schema (MetaData) for units definitions: id, field (SLK column), display_name, category, type, min_val, max_val. Drives a dynamic editor UI.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "objects.units.fields_meta"
  },
  {
    "name": "objects_units_get",
    "description": "Return the full field map for one units definition by id (4-char rawcode). Includes raw values, display-resolved values (with and without WC3 color codes), per-field category/type metadata, and the model/icon paths.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the units object"
        }
      },
      "required": [
        "id"
      ],
      "additionalProperties": false
    },
    "method": "objects.units.get"
  },
  {
    "name": "objects_units_list",
    "description": "List all units definitions (stock SLK rows + per-map object-data overrides). Returns id, name, race, kind, category, is_custom, is_edited, base_id, icon_art — flat rows the UI groups into a tree client-side.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "objects.units.list"
  },
  {
    "name": "objects_units_set_field",
    "description": "Set one field on a units definition (writes the per-map object-data shadow; undo-aware). Re-emits the post-mutation get payload. Pass an optional 'level' for leveled fields (e.g. ability levels); omit for non-leveled fields.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the units object to edit"
        },
        "column": {
          "type": "string",
          "description": "SLK column name (lowercased), e.g. 'name', 'hp'"
        },
        "value": {
          "type": "string",
          "description": "raw cell value to write (stringified)"
        },
        "level": {
          "type": "integer",
          "description": "optional 1-based level for leveled fields (ability/upgrade); omit for flat fields"
        }
      },
      "required": [
        "id",
        "column",
        "value"
      ],
      "additionalProperties": false
    },
    "method": "objects.units.set_field"
  },
  {
    "name": "objects_upgrades_create_custom",
    "description": "Create a new custom upgrades definition cloned from a base object. Returns { id, detail } where id is the new custom's rawcode. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "base_id": {
          "type": "string",
          "description": "4-char rawcode of the base object to clone from"
        },
        "id": {
          "type": "string",
          "description": "optional explicit 4-char rawcode for the new custom; auto-assigned when omitted"
        }
      },
      "required": [
        "base_id"
      ],
      "additionalProperties": false
    },
    "method": "objects.upgrades.create_custom"
  },
  {
    "name": "objects_upgrades_delete_custom",
    "description": "Delete a custom upgrades definition by id (4-char rawcode). Only custom objects can be deleted; stock rows error. Undo-aware. Returns { ok: true }.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the custom upgrades object to delete"
        }
      },
      "required": [
        "id"
      ],
      "additionalProperties": false
    },
    "method": "objects.upgrades.delete_custom"
  },
  {
    "name": "objects_upgrades_fields_meta",
    "description": "Return the field schema (MetaData) for upgrades definitions: id, field (SLK column), display_name, category, type, min_val, max_val. Drives a dynamic editor UI.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "objects.upgrades.fields_meta"
  },
  {
    "name": "objects_upgrades_get",
    "description": "Return the full field map for one upgrades definition by id (4-char rawcode). Includes raw values, display-resolved values (with and without WC3 color codes), per-field category/type metadata, and the model/icon paths.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the upgrades object"
        }
      },
      "required": [
        "id"
      ],
      "additionalProperties": false
    },
    "method": "objects.upgrades.get"
  },
  {
    "name": "objects_upgrades_list",
    "description": "List all upgrades definitions (stock SLK rows + per-map object-data overrides). Returns id, name, race, kind, category, is_custom, is_edited, base_id, icon_art — flat rows the UI groups into a tree client-side.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "objects.upgrades.list"
  },
  {
    "name": "objects_upgrades_set_field",
    "description": "Set one field on a upgrades definition (writes the per-map object-data shadow; undo-aware). Re-emits the post-mutation get payload. Pass an optional 'level' for leveled fields (e.g. ability levels); omit for non-leveled fields.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "string",
          "description": "4-char rawcode of the upgrades object to edit"
        },
        "column": {
          "type": "string",
          "description": "SLK column name (lowercased), e.g. 'name', 'hp'"
        },
        "value": {
          "type": "string",
          "description": "raw cell value to write (stringified)"
        },
        "level": {
          "type": "integer",
          "description": "optional 1-based level for leveled fields (ability/upgrade); omit for flat fields"
        }
      },
      "required": [
        "id",
        "column",
        "value"
      ],
      "additionalProperties": false
    },
    "method": "objects.upgrades.set_field"
  },
  {
    "name": "regions_create",
    "description": "Create a new region (rect) in war3map.w3r. min/max are WC3 world units (origin = map center); corners may be given in any order (they're normalized). Allocates a war3map.w3r table if the map shipped none. Returns { creation_number }. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "name": {
          "type": "string",
          "description": "region name (becomes the gg_rct_<name> codegen handle)"
        },
        "min_x": {
          "type": "number",
          "description": "min world X (left)"
        },
        "min_y": {
          "type": "number",
          "description": "min world Y (bottom)"
        },
        "max_x": {
          "type": "number",
          "description": "max world X (right)"
        },
        "max_y": {
          "type": "number",
          "description": "max world Y (top)"
        },
        "weather_id": {
          "type": "string",
          "description": "4-char weather FourCC, e.g. 'RAhr'; omit/empty for none"
        },
        "ambient_id": {
          "type": "string",
          "description": "ambient sound label; omit/empty for none"
        },
        "color": {
          "type": "array",
          "items": {
            "type": "integer"
          },
          "description": "[r,g,b] 0..255 overlay/preview color; default opaque white"
        }
      },
      "required": [
        "name",
        "min_x",
        "min_y",
        "max_x",
        "max_y"
      ],
      "additionalProperties": false
    },
    "method": "regions.create"
  },
  {
    "name": "regions_delete",
    "description": "Delete a region by creation_number. Returns { ok: true }. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "creation_number": {
          "type": "integer",
          "description": "creation_number from regions_list"
        }
      },
      "required": [
        "creation_number"
      ],
      "additionalProperties": false
    },
    "method": "regions.delete"
  },
  {
    "name": "regions_get",
    "description": "Get one region by creation_number. Returns the full RegionInfo (name, bounds, weather_id, ambient_id, color, gg_ref).",
    "inputSchema": {
      "type": "object",
      "properties": {
        "creation_number": {
          "type": "integer",
          "description": "creation_number from regions_list"
        }
      },
      "required": [
        "creation_number"
      ],
      "additionalProperties": false
    },
    "method": "regions.get"
  },
  {
    "name": "regions_list",
    "description": "List the map's regions (war3map.w3r) with full detail: name, creation_number, bounds (min_x/min_y/max_x/max_y in world units), weather_id, ambient_id, color [r,g,b], and gg_rct_* codegen handle. Returns { regions: [...] }.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "regions.list"
  },
  {
    "name": "regions_move",
    "description": "Translate a region by (dx, dy) world units, preserving its size. Returns { ok: true }. Undo-aware. Zero delta is a no-op.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "creation_number": {
          "type": "integer",
          "description": "creation_number from regions_list"
        },
        "dx": {
          "type": "number",
          "description": "world-X delta"
        },
        "dy": {
          "type": "number",
          "description": "world-Y delta"
        }
      },
      "required": [
        "creation_number",
        "dx",
        "dy"
      ],
      "additionalProperties": false
    },
    "method": "regions.move"
  },
  {
    "name": "regions_rename",
    "description": "Rename a region (changes its gg_rct_<name> codegen handle). Returns { ok: true }. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "creation_number": {
          "type": "integer",
          "description": "creation_number from regions_list"
        },
        "name": {
          "type": "string",
          "description": "new region name"
        }
      },
      "required": [
        "creation_number",
        "name"
      ],
      "additionalProperties": false
    },
    "method": "regions.rename"
  },
  {
    "name": "regions_resize",
    "description": "Set a region's absolute bounds (WC3 world units; corners normalized so min<=max). Returns { ok: true }. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "creation_number": {
          "type": "integer",
          "description": "creation_number from regions_list"
        },
        "min_x": {
          "type": "number",
          "description": "min world X (left)"
        },
        "min_y": {
          "type": "number",
          "description": "min world Y (bottom)"
        },
        "max_x": {
          "type": "number",
          "description": "max world X (right)"
        },
        "max_y": {
          "type": "number",
          "description": "max world Y (top)"
        }
      },
      "required": [
        "creation_number",
        "min_x",
        "min_y",
        "max_x",
        "max_y"
      ],
      "additionalProperties": false
    },
    "method": "regions.resize"
  },
  {
    "name": "selection_clear",
    "description": "Clear the current selection.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "selection.clear"
  },
  {
    "name": "selection_get",
    "description": "Read the current editor selection — same selection state shown in the GUI side panels.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "selection.get"
  },
  {
    "name": "selection_set",
    "description": "Replace the current selection. By default the last item in the list becomes the primary selection; pass 'primary' to override — either an index into items, or a 'kind:id' / bare-'id' selector string.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "items": {
          "type": "array",
          "items": {
            "type": "object",
            "properties": {
              "kind": {
                "type": "string",
                "description": "'unit' | 'item' | 'doodad' | 'region' | 'trigger' — entity kind"
              },
              "id": {
                "type": "integer",
                "description": "creation_number for unit/item/doodad; opaque per kind"
              }
            },
            "required": [
              "kind",
              "id"
            ],
            "additionalProperties": false
          }
        },
        "primary": {
          "anyOf": [
            {
              "type": "integer"
            },
            {
              "type": "string"
            }
          ],
          "description": "Optional primary selection: an index into items, or a 'kind:id' (e.g. 'unit:42') / bare 'id' selector. Defaults to the last item."
        }
      },
      "required": [
        "items"
      ],
      "additionalProperties": false
    },
    "method": "selection.set"
  },
  {
    "name": "session_select",
    "description": "Select which wc3-forge instance subsequent tool calls go to, by pid (from sessions_list). Pass pid=null (or omit) to clear and auto-route to the oldest running instance.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "pid": {
          "anyOf": [
            {
              "type": "integer"
            },
            {
              "type": "null"
            }
          ]
        }
      },
      "additionalProperties": false
    },
    "method": null
  },
  {
    "name": "sessions_list",
    "description": "List all running wc3-forge instances detected via their MCP lockfiles. For each one, also reports the bridge_ping result. Useful when multiple wc3-forge windows are open and Claude needs to pick which one to drive.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": null
  },
  {
    "name": "start_locations_create",
    "description": "Create a new start location (sloc) at the given world coords, assigned to start-location index `index` (becomes gg_start_location_<index> in generated script). Rejects a duplicate index. Returns { creation_number }. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "index": {
          "type": "integer",
          "description": "start-location index (0-based; becomes gg_start_location_<index>)"
        },
        "x": {
          "type": "number",
          "description": "world X (origin = map center)"
        },
        "y": {
          "type": "number",
          "description": "world Y (origin = map center)"
        },
        "z": {
          "type": "number",
          "description": "world Z (default 0 / terrain height)"
        },
        "rotation": {
          "type": "number",
          "description": "facing angle in radians; default 3π/2 (the Reforged sloc facing)"
        }
      },
      "required": [
        "index",
        "x",
        "y"
      ],
      "additionalProperties": false
    },
    "method": "start_locations.create"
  },
  {
    "name": "start_locations_delete",
    "description": "Delete the start location with the given index. Returns { ok: true }. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "index": {
          "type": "integer",
          "description": "start-location index from start_locations_list"
        }
      },
      "required": [
        "index"
      ],
      "additionalProperties": false
    },
    "method": "start_locations.delete"
  },
  {
    "name": "start_locations_list",
    "description": "List the map's start locations (sloc entities) sorted by start-location index. Returns { start_locations: [{ index, creation_number, position:[x,y,z], rotation }] }.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "start_locations.list"
  },
  {
    "name": "start_locations_move",
    "description": "Relocate the start location with the given index to the supplied world coords. Looks up by start-location index, not creation_number. Returns { ok: true }. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "index": {
          "type": "integer",
          "description": "start-location index from start_locations_list"
        },
        "x": {
          "type": "number",
          "description": "world X (origin = map center)"
        },
        "y": {
          "type": "number",
          "description": "world Y (origin = map center)"
        },
        "z": {
          "type": "number",
          "description": "world Z (default 0 / terrain height)"
        }
      },
      "required": [
        "index",
        "x",
        "y"
      ],
      "additionalProperties": false
    },
    "method": "start_locations.move"
  },
  {
    "name": "terrain_brush_cliff",
    "description": "Raise/lower/set the integer cliff layer (0..15) over a brush footprint. mode: 'raise' (layer+1) | 'lower' (layer-1) | 'set' (layer=level). cliff_tile_id selects the cliff tileset FourCC (empty → slot 0); the brush sets each corner's cliff texture so a wall actually renders. Cliff walls are derived from layer differences, so painting a higher layer next to lower terrain creates a cliff. Returns { ok: true }. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "col": {
          "type": "integer",
          "description": "footprint center column (0-based corner)"
        },
        "row": {
          "type": "integer",
          "description": "footprint center row (0-based corner)"
        },
        "radius": {
          "type": "number",
          "description": "brush radius in corner units, fractional allowed (0 = single corner)"
        },
        "shape": {
          "type": "string",
          "enum": [
            "circle",
            "square"
          ],
          "description": "brush shape (default circle)"
        },
        "mode": {
          "type": "string",
          "enum": [
            "raise",
            "lower",
            "set"
          ],
          "description": "cliff brush mode"
        },
        "level": {
          "type": "integer",
          "description": "for raise/lower: number of cliff steps to move (default 1); for 'set': absolute target layer (0..15)"
        },
        "cliff_tile_id": {
          "type": "string",
          "description": "4-char cliff-tileset FourCC (empty → slot 0)"
        }
      },
      "required": [
        "col",
        "row",
        "mode"
      ],
      "additionalProperties": false
    },
    "method": "terrain.brush_cliff"
  },
  {
    "name": "terrain_brush_height",
    "description": "Raise/lower/flatten/smooth the terrain heightfield over a brush footprint. mode: 'raise' | 'lower' | 'flatten' | 'smooth'. strength is game-Z units added per dab for raise/lower (default 16), or a 0..1 blend fraction for smooth (default 0.5). target is the height (game-Z) to flatten to. Returns { ok: true }. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "col": {
          "type": "integer",
          "description": "footprint center column (0-based corner)"
        },
        "row": {
          "type": "integer",
          "description": "footprint center row (0-based corner)"
        },
        "radius": {
          "type": "number",
          "description": "brush radius in corner units, fractional allowed (0 = single corner)"
        },
        "shape": {
          "type": "string",
          "enum": [
            "circle",
            "square"
          ],
          "description": "brush shape (default circle)"
        },
        "mode": {
          "type": "string",
          "enum": [
            "raise",
            "lower",
            "flatten",
            "smooth"
          ],
          "description": "height brush mode"
        },
        "strength": {
          "type": "number",
          "description": "game-Z per dab (raise/lower) or 0..1 blend (smooth)"
        },
        "target": {
          "type": "number",
          "description": "flatten target height (game-Z)"
        }
      },
      "required": [
        "col",
        "row",
        "mode"
      ],
      "additionalProperties": false
    },
    "method": "terrain.brush_height"
  },
  {
    "name": "terrain_brush_ramp",
    "description": "Toggle the ramp flag over a brush footprint. on=true makes the cliff step under the footprint render as a walkable slope; on=false reverts it to a sheer wall. Use over a cliff edge after raising a cliff to make it traversable. Returns { ok: true }. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "col": {
          "type": "integer",
          "description": "footprint center column (0-based corner)"
        },
        "row": {
          "type": "integer",
          "description": "footprint center row (0-based corner)"
        },
        "radius": {
          "type": "number",
          "description": "brush radius in corner units, fractional allowed (0 = single corner)"
        },
        "shape": {
          "type": "string",
          "enum": [
            "circle",
            "square"
          ],
          "description": "brush shape (default circle)"
        },
        "on": {
          "type": "boolean",
          "description": "true = mark ramp (slope), false = clear ramp (wall)"
        }
      },
      "required": [
        "col",
        "row",
        "on"
      ],
      "additionalProperties": false
    },
    "method": "terrain.brush_ramp"
  },
  {
    "name": "terrain_brush_water",
    "description": "Add or remove water over a brush footprint. mode: 'add' floods the footprint; 'remove' clears water. For 'add', pass height for an absolute water-surface Z (game-Z studs) applied FLAT across the footprint; omit height to auto-place the surface ~122 studs above the ground under the center. Corners that already have visible water keep their height on 'add', so re-painting a pond won't flatten a tuned surface. A water quad renders for any cell with at least one watered corner. Returns { ok: true }. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "col": {
          "type": "integer",
          "description": "footprint center column (0-based corner)"
        },
        "row": {
          "type": "integer",
          "description": "footprint center row (0-based corner)"
        },
        "radius": {
          "type": "number",
          "description": "brush radius in corner units, fractional allowed (0 = single corner)"
        },
        "shape": {
          "type": "string",
          "enum": [
            "circle",
            "square"
          ],
          "description": "brush shape (default circle)"
        },
        "mode": {
          "type": "string",
          "enum": [
            "add",
            "remove"
          ],
          "description": "water brush mode"
        },
        "height": {
          "type": "number",
          "description": "absolute water-surface height (game-Z studs), applied flat; omit to auto-place just above the ground"
        }
      },
      "required": [
        "col",
        "row",
        "mode"
      ],
      "additionalProperties": false
    },
    "method": "terrain.brush_water"
  },
  {
    "name": "terrain_get_tile",
    "description": "Read back the ground tile + height at a terrain corner (so edits are verifiable). col/row are 0-based corner grid indices. Returns { col, row, ground_tile_id, height }.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "col": {
          "type": "integer",
          "description": "0-based terrain corner column"
        },
        "row": {
          "type": "integer",
          "description": "0-based terrain corner row"
        }
      },
      "required": [
        "col",
        "row"
      ],
      "additionalProperties": false
    },
    "method": "terrain.get_tile"
  },
  {
    "name": "terrain_paint_tile",
    "description": "Paint a ground texture (tile) over a brush footprint of terrain corners. col/row are the footprint center (0-based corner indices). radius is in corner units (0 = single corner). shape is 'circle' or 'square'. ground_tile_id is a 4-char FourCC that must be in the map's ground palette. Returns { ok: true }. Undo-aware (one step per call).",
    "inputSchema": {
      "type": "object",
      "properties": {
        "col": {
          "type": "integer",
          "description": "footprint center column (0-based corner)"
        },
        "row": {
          "type": "integer",
          "description": "footprint center row (0-based corner)"
        },
        "radius": {
          "type": "number",
          "description": "brush radius in corner units, fractional allowed (0 = single corner)"
        },
        "shape": {
          "type": "string",
          "enum": [
            "circle",
            "square"
          ],
          "description": "brush shape (default circle)"
        },
        "ground_tile_id": {
          "type": "string",
          "description": "4-char FourCC in the map ground palette"
        }
      },
      "required": [
        "col",
        "row",
        "ground_tile_id"
      ],
      "additionalProperties": false
    },
    "method": "terrain.paint_tile"
  },
  {
    "name": "terrain_set_height",
    "description": "Set the height at a terrain corner. col/row are 0-based corner grid indices. Returns { ok: true }. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "col": {
          "type": "integer",
          "description": "0-based terrain corner column"
        },
        "row": {
          "type": "integer",
          "description": "0-based terrain corner row"
        },
        "height": {
          "type": "number",
          "description": "terrain height value"
        }
      },
      "required": [
        "col",
        "row",
        "height"
      ],
      "additionalProperties": false
    },
    "method": "terrain.set_height"
  },
  {
    "name": "terrain_set_tile",
    "description": "Set the ground texture (tile) at a terrain corner. col/row are 0-based corner grid indices. ground_tile_id is a 4-char FourCC that must be in the map's ground tile palette. Returns { ok: true }. Undo-aware.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "col": {
          "type": "integer",
          "description": "0-based terrain corner column"
        },
        "row": {
          "type": "integer",
          "description": "0-based terrain corner row"
        },
        "ground_tile_id": {
          "type": "string",
          "description": "4-char FourCC in the map ground palette"
        }
      },
      "required": [
        "col",
        "row",
        "ground_tile_id"
      ],
      "additionalProperties": false
    },
    "method": "terrain.set_tile"
  },
  {
    "name": "terrain_swap_tileset",
    "description": "Re-tile the loaded map: replace the ground/cliff palettes and remap every tilepoint via the from→to tables, updating the tileset letter in both .w3e and .w3i. Caller must pre-resolve the full remap (no auto-pick). *_from_to[oldSlot] = newSlot index into the corresponding new palette. Returns { ok: true }. Undo-aware. See terrain_get_tile to verify.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "new_letter": {
          "type": "string",
          "description": "single-char tileset code, e.g. 'L', 'A'"
        },
        "new_ground_tilesets": {
          "type": "array",
          "items": {
            "type": "string"
          },
          "description": "ordered ground-tileset FourCCs (1..16 for v11, 1..64 for v12+)"
        },
        "new_cliff_tilesets": {
          "type": "array",
          "items": {
            "type": "string"
          },
          "description": "ordered cliff-tileset FourCCs (0..16)"
        },
        "ground_from_to": {
          "type": "array",
          "items": {
            "type": "integer"
          },
          "description": "len == old ground palette; value = index into new_ground_tilesets"
        },
        "cliff_from_to": {
          "type": "array",
          "items": {
            "type": "integer"
          },
          "description": "len == old cliff palette; value = index into new_cliff_tilesets"
        }
      },
      "required": [
        "new_letter",
        "new_ground_tilesets",
        "new_cliff_tilesets",
        "ground_from_to",
        "cliff_from_to"
      ],
      "additionalProperties": false
    },
    "method": "terrain.swap_tileset"
  },
  {
    "name": "triggers_add_category",
    "description": "Add a trigger category (folder). Returns the refreshed tree + the new node's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "name": {
          "type": "string",
          "description": "category display name"
        },
        "parent_id": {
          "type": "integer",
          "description": "parent node id (0 / map-root for top level)"
        }
      },
      "required": [
        "name",
        "parent_id"
      ],
      "additionalProperties": false
    },
    "method": "triggers.add_category"
  },
  {
    "name": "triggers_add_comment",
    "description": "Add a comment trigger node. Returns the refreshed tree + the new node's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "name": {
          "type": "string",
          "description": "comment text / name"
        },
        "parent_id": {
          "type": "integer",
          "description": "parent category id"
        }
      },
      "required": [
        "name",
        "parent_id"
      ],
      "additionalProperties": false
    },
    "method": "triggers.add_comment"
  },
  {
    "name": "triggers_add_eca",
    "description": "Add a top-level ECA (event/condition/action/call) to a trigger. Returns the refreshed tree + the trigger's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "trigger_id": {
          "type": "integer",
          "description": "target trigger id"
        },
        "eca_type": {
          "type": "integer",
          "description": "wtg.ECAType: 0=event, 1=condition, 2=action, 3=call"
        },
        "name": {
          "type": "string",
          "description": "function name from triggers_functions_meta"
        },
        "position": {
          "type": "integer",
          "description": "insert index within the ECA group; omit to append"
        }
      },
      "required": [
        "trigger_id",
        "eca_type",
        "name"
      ],
      "additionalProperties": false
    },
    "method": "triggers.add_eca"
  },
  {
    "name": "triggers_add_gui",
    "description": "Add a normal GUI trigger (holds ECAs). Returns the refreshed tree + the new trigger's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "name": {
          "type": "string",
          "description": "trigger name"
        },
        "parent_id": {
          "type": "integer",
          "description": "parent category id"
        }
      },
      "required": [
        "name",
        "parent_id"
      ],
      "additionalProperties": false
    },
    "method": "triggers.add_gui"
  },
  {
    "name": "triggers_add_nested_eca",
    "description": "Add an ECA nested inside a magic/container ECA (IfThenElse Then-branch, ForLoop body, etc.). Returns the refreshed tree + the trigger's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "trigger_id": {
          "type": "integer",
          "description": "target trigger id"
        },
        "parent_path": {
          "type": "array",
          "items": {
            "type": "integer"
          },
          "description": "ECA path to the container ECA (array of child indices)"
        },
        "eca_type": {
          "type": "integer",
          "description": "wtg.ECAType: 0=event, 1=condition, 2=action, 3=call"
        },
        "name": {
          "type": "string",
          "description": "function name"
        },
        "group_id": {
          "type": "integer",
          "description": "magic-ECA child group id (selects which branch/body)"
        }
      },
      "required": [
        "trigger_id",
        "parent_path",
        "eca_type",
        "name",
        "group_id"
      ],
      "additionalProperties": false
    },
    "method": "triggers.add_nested_eca"
  },
  {
    "name": "triggers_add_script",
    "description": "Add a custom-script (JASS/Lua) trigger. Returns the refreshed tree + the new trigger's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "name": {
          "type": "string",
          "description": "trigger name"
        },
        "parent_id": {
          "type": "integer",
          "description": "parent category id"
        }
      },
      "required": [
        "name",
        "parent_id"
      ],
      "additionalProperties": false
    },
    "method": "triggers.add_script"
  },
  {
    "name": "triggers_add_variable",
    "description": "Add a global trigger variable (udg_*). Returns the refreshed tree + the new variable's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "name": {
          "type": "string",
          "description": "variable name (without udg_ prefix)"
        },
        "type": {
          "type": "string",
          "description": "WC3 variable type, e.g. 'integer', 'unit', 'real'"
        },
        "is_array": {
          "type": "boolean",
          "description": "true for an array variable"
        },
        "array_size": {
          "type": "integer",
          "description": "array size when is_array (else 0)"
        },
        "initial_value": {
          "type": "string",
          "description": "initial value literal ('' for none)"
        }
      },
      "required": [
        "name",
        "type",
        "is_array",
        "array_size",
        "initial_value"
      ],
      "additionalProperties": false
    },
    "method": "triggers.add_variable"
  },
  {
    "name": "triggers_check_convert_to_lua",
    "description": "Read-only blocker scan for the Convert-Map-to-Lua flow. Always returns a result payload (blockers may be empty); never mutates.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "triggers.check_convert_to_lua"
  },
  {
    "name": "triggers_clear_param_sub_function",
    "description": "Remove a sub-function from a parameter slot, reverting it to a plain value. Returns the refreshed tree + the trigger's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "trigger_id": {
          "type": "integer",
          "description": "target trigger id"
        },
        "eca_path": {
          "type": "array",
          "items": {
            "type": "integer"
          },
          "description": "ECA path (array of child indices)"
        },
        "param_path": {
          "type": "array",
          "items": {
            "type": "integer"
          },
          "description": "parameter path (sub-parameter chain)"
        }
      },
      "required": [
        "trigger_id",
        "eca_path",
        "param_path"
      ],
      "additionalProperties": false
    },
    "method": "triggers.clear_param_sub_function"
  },
  {
    "name": "triggers_convert_to_lua",
    "description": "Run the full Convert-Map-to-Lua conversion. When blockers exist, returns a { blockers: [...] } payload (NOT an error) so the UI can render them. Other failures error. Accepts optional { backup } (default true).",
    "inputSchema": {
      "type": "object",
      "properties": {
        "backup": {
          "type": "boolean",
          "description": "write a backup of the original script before converting (default true)"
        }
      },
      "additionalProperties": false
    },
    "method": "triggers.convert_to_lua"
  },
  {
    "name": "triggers_delete",
    "description": "Delete a trigger node (category / trigger / variable) by id. Returns the refreshed tree.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "integer",
          "description": "node id to delete"
        }
      },
      "required": [
        "id"
      ],
      "additionalProperties": false
    },
    "method": "triggers.delete"
  },
  {
    "name": "triggers_delete_eca",
    "description": "Delete an ECA (and its nested children) by path. Returns the refreshed tree + the trigger's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "trigger_id": {
          "type": "integer",
          "description": "target trigger id"
        },
        "eca_path": {
          "type": "array",
          "items": {
            "type": "integer"
          },
          "description": "ECA path (array of child indices)"
        }
      },
      "required": [
        "trigger_id",
        "eca_path"
      ],
      "additionalProperties": false
    },
    "method": "triggers.delete_eca"
  },
  {
    "name": "triggers_functions_meta",
    "description": "Return the TriggerData.txt vocabulary: every event/condition/action/call function, plus category labels, type metadata, and presets. Large, static payload used to render GUI labels client-side.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "triggers.functions_meta"
  },
  {
    "name": "triggers_generate_script",
    "description": "Generate the war3map.lua script text from the current triggers WITHOUT writing it. Returns { text, bytes }.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "triggers.generate_script"
  },
  {
    "name": "triggers_get",
    "description": "Return one trigger node's full detail by id: category, trigger (with nested ECAs + sub-parameters + custom_text), or variable. Exactly one is populated; 'kind' disambiguates.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "integer",
          "description": "node id from triggers_tree"
        }
      },
      "required": [
        "id"
      ],
      "additionalProperties": false
    },
    "method": "triggers.get"
  },
  {
    "name": "triggers_list_cameras",
    "description": "List the map's camera presets (war3map.w3c) with their gg_cam_* codegen names — for camera-typed parameter pickers.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "triggers.list_cameras"
  },
  {
    "name": "triggers_list_destructable_instances",
    "description": "List placed destructable instances with their gg_dest_* codegen names — for destructable-typed parameter pickers.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "triggers.list_destructable_instances"
  },
  {
    "name": "triggers_list_regions",
    "description": "List the map's regions (war3map.w3r) with their gg_rct_* codegen names — for region-typed parameter pickers.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "triggers.list_regions"
  },
  {
    "name": "triggers_list_unit_instances",
    "description": "List placed unit instances with their gg_unit_* codegen names — for unit-typed parameter pickers.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "triggers.list_unit_instances"
  },
  {
    "name": "triggers_move",
    "description": "Re-parent a trigger node under a new parent. Returns the refreshed tree + the node's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "integer",
          "description": "node id to move"
        },
        "new_parent_id": {
          "type": "integer",
          "description": "destination parent node id"
        }
      },
      "required": [
        "id",
        "new_parent_id"
      ],
      "additionalProperties": false
    },
    "method": "triggers.move"
  },
  {
    "name": "triggers_move_eca",
    "description": "Move an ECA within its sibling group to a new position. Returns the refreshed tree + the trigger's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "trigger_id": {
          "type": "integer",
          "description": "target trigger id"
        },
        "eca_path": {
          "type": "array",
          "items": {
            "type": "integer"
          },
          "description": "ECA path (array of child indices) addressing the ECA to move"
        },
        "new_position": {
          "type": "integer",
          "description": "new index within the sibling group"
        }
      },
      "required": [
        "trigger_id",
        "eca_path",
        "new_position"
      ],
      "additionalProperties": false
    },
    "method": "triggers.move_eca"
  },
  {
    "name": "triggers_rename",
    "description": "Rename a trigger node by id. Returns the refreshed tree + the node's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "integer",
          "description": "node id"
        },
        "name": {
          "type": "string",
          "description": "new name"
        }
      },
      "required": [
        "id",
        "name"
      ],
      "additionalProperties": false
    },
    "method": "triggers.rename"
  },
  {
    "name": "triggers_save_script",
    "description": "Generate + write war3map.lua to the map source. Returns { bytes }. Errors for MPQ-backed maps (extract to a folder first).",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "triggers.save_script"
  },
  {
    "name": "triggers_search",
    "description": "Fuzzy substring search across all triggers. Empty query returns the first N triggers by name. Returns { hits, truncated }.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "query": {
          "type": "string",
          "description": "substring to match (empty = first N by name)"
        },
        "limit": {
          "type": "integer",
          "description": "max hits (default 50)"
        }
      },
      "required": [
        "query"
      ],
      "additionalProperties": false
    },
    "method": "triggers.search"
  },
  {
    "name": "triggers_set_custom_text",
    "description": "Set a custom-script trigger's raw script text. Returns the refreshed tree + the node's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "integer",
          "description": "trigger id (a script trigger)"
        },
        "text": {
          "type": "string",
          "description": "JASS/Lua source text"
        }
      },
      "required": [
        "id",
        "text"
      ],
      "additionalProperties": false
    },
    "method": "triggers.set_custom_text"
  },
  {
    "name": "triggers_set_description",
    "description": "Set a trigger's description text. Returns the refreshed tree + the node's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "integer",
          "description": "trigger id"
        },
        "text": {
          "type": "string",
          "description": "description text"
        }
      },
      "required": [
        "id",
        "text"
      ],
      "additionalProperties": false
    },
    "method": "triggers.set_description"
  },
  {
    "name": "triggers_set_eca_enabled",
    "description": "Toggle an ECA's enabled flag. Returns the refreshed tree + the trigger's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "trigger_id": {
          "type": "integer",
          "description": "target trigger id"
        },
        "eca_path": {
          "type": "array",
          "items": {
            "type": "integer"
          },
          "description": "ECA path (array of child indices)"
        },
        "enabled": {
          "type": "boolean"
        }
      },
      "required": [
        "trigger_id",
        "eca_path",
        "enabled"
      ],
      "additionalProperties": false
    },
    "method": "triggers.set_eca_enabled"
  },
  {
    "name": "triggers_set_enabled",
    "description": "Toggle a trigger's enabled flag. Returns the refreshed tree + the node's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "integer",
          "description": "trigger id"
        },
        "value": {
          "type": "boolean",
          "description": "true = enabled"
        }
      },
      "required": [
        "id",
        "value"
      ],
      "additionalProperties": false
    },
    "method": "triggers.set_enabled"
  },
  {
    "name": "triggers_set_initially_on",
    "description": "Toggle a trigger's 'initially on' flag. Returns the refreshed tree + the node's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "integer",
          "description": "trigger id"
        },
        "value": {
          "type": "boolean",
          "description": "true = initially on"
        }
      },
      "required": [
        "id",
        "value"
      ],
      "additionalProperties": false
    },
    "method": "triggers.set_initially_on"
  },
  {
    "name": "triggers_set_map_header_script",
    "description": "Set the map-header custom script (the global JASS/Lua block at the top of the map's script). Returns the refreshed tree.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "content": {
          "type": "string",
          "description": "header script source text"
        }
      },
      "required": [
        "content"
      ],
      "additionalProperties": false
    },
    "method": "triggers.set_map_header_script"
  },
  {
    "name": "triggers_set_param_array",
    "description": "Toggle a parameter slot's array-index flag (so it reads var[index] vs var). Returns the refreshed tree + the trigger's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "trigger_id": {
          "type": "integer",
          "description": "target trigger id"
        },
        "eca_path": {
          "type": "array",
          "items": {
            "type": "integer"
          },
          "description": "ECA path (array of child indices)"
        },
        "param_path": {
          "type": "array",
          "items": {
            "type": "integer"
          },
          "description": "parameter path (sub-parameter chain)"
        },
        "is_array": {
          "type": "boolean"
        }
      },
      "required": [
        "trigger_id",
        "eca_path",
        "param_path",
        "is_array"
      ],
      "additionalProperties": false
    },
    "method": "triggers.set_param_array"
  },
  {
    "name": "triggers_set_param_sub_function",
    "description": "Attach a sub-function (nested call) to a parameter slot. Returns the refreshed tree + the trigger's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "trigger_id": {
          "type": "integer",
          "description": "target trigger id"
        },
        "eca_path": {
          "type": "array",
          "items": {
            "type": "integer"
          },
          "description": "ECA path (array of child indices)"
        },
        "param_path": {
          "type": "array",
          "items": {
            "type": "integer"
          },
          "description": "parameter path (sub-parameter chain)"
        },
        "sub_name": {
          "type": "string",
          "description": "sub-function name from triggers_functions_meta"
        }
      },
      "required": [
        "trigger_id",
        "eca_path",
        "param_path",
        "sub_name"
      ],
      "additionalProperties": false
    },
    "method": "triggers.set_param_sub_function"
  },
  {
    "name": "triggers_set_param_value",
    "description": "Set one parameter slot's value inside an ECA. Use param_path (array, addresses sub-parameter chains) OR the legacy param_index (single int). Returns the refreshed tree + the trigger's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "trigger_id": {
          "type": "integer",
          "description": "target trigger id"
        },
        "eca_path": {
          "type": "array",
          "items": {
            "type": "integer"
          },
          "description": "ECA path (array of child indices)"
        },
        "param_path": {
          "type": "array",
          "items": {
            "type": "integer"
          },
          "description": "2b2 shape: addresses sub-parameter chains; wins over param_index"
        },
        "param_index": {
          "type": "integer",
          "description": "legacy single-index shape; used when param_path omitted"
        },
        "value": {
          "type": "string",
          "description": "the value literal / variable name / preset key / function name"
        },
        "param_type": {
          "type": "integer",
          "description": "wtg.ParamType: 0=preset, 1=variable, 2=function, 3=string"
        }
      },
      "required": [
        "trigger_id",
        "eca_path",
        "value",
        "param_type"
      ],
      "additionalProperties": false
    },
    "method": "triggers.set_param_value"
  },
  {
    "name": "triggers_set_run_on_init",
    "description": "Toggle a trigger's 'run on map initialization' flag. Returns the refreshed tree + the node's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "integer",
          "description": "trigger id"
        },
        "value": {
          "type": "boolean",
          "description": "true = run on initialization"
        }
      },
      "required": [
        "id",
        "value"
      ],
      "additionalProperties": false
    },
    "method": "triggers.set_run_on_init"
  },
  {
    "name": "triggers_set_variable",
    "description": "Edit an existing trigger variable's name/type/array/initial-value. Returns the refreshed tree + the variable's detail.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "id": {
          "type": "integer",
          "description": "variable node id"
        },
        "name": {
          "type": "string",
          "description": "variable name"
        },
        "type": {
          "type": "string",
          "description": "WC3 variable type"
        },
        "is_array": {
          "type": "boolean"
        },
        "array_size": {
          "type": "integer"
        },
        "initial_value": {
          "type": "string"
        }
      },
      "required": [
        "id",
        "name",
        "type",
        "is_array",
        "array_size",
        "initial_value"
      ],
      "additionalProperties": false
    },
    "method": "triggers.set_variable"
  },
  {
    "name": "triggers_test_map",
    "description": "Full Test Map pipeline: save pending edits, regenerate + write war3map.lua, then launch WC3 with the map preloaded. Returns { ok: true }.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "triggers.test_map"
  },
  {
    "name": "triggers_transpile_preview",
    "description": "Return the read-only Convert-to-Lua diff preview (vJASS → Lua) without writing anything.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "triggers.transpile_preview"
  },
  {
    "name": "triggers_tree",
    "description": "Return the full trigger tree (categories + triggers + variables) for the loaded map in one shot — a flat node array the UI stitches into a hierarchy via parent_id. Empty (non-error) when no map / no triggers.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "triggers.tree"
  },
  {
    "name": "units_create",
    "description": "Create a new placed unit in war3mapUnits.doo. x/y are WC3 world units (origin = map center). Returns { creation_number }. Undo-aware; emits the standard change events.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "type_id": {
          "type": "string",
          "description": "4-char unit rawcode, e.g. 'hfoo'"
        },
        "player": {
          "type": "integer",
          "description": "owning player slot (0-based; 27 = neutral hostile)"
        },
        "x": {
          "type": "number",
          "description": "world X (origin = map center)"
        },
        "y": {
          "type": "number",
          "description": "world Y (origin = map center)"
        },
        "z": {
          "type": "number",
          "description": "world Z (default 0 / terrain height)"
        },
        "rotation": {
          "type": "number",
          "description": "facing angle in radians (Z-axis); default 0"
        },
        "scale": {
          "type": "number",
          "description": "uniform scale (default 1.0)"
        }
      },
      "required": [
        "type_id",
        "player",
        "x",
        "y"
      ],
      "additionalProperties": false
    },
    "method": "units.create"
  },
  {
    "name": "units_delete",
    "description": "Delete a placed unit by creation_number. Returns { ok: true }. Undo-aware; emits the standard change events.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "creation_number": {
          "type": "integer",
          "description": "creation_number from units_list"
        }
      },
      "required": [
        "creation_number"
      ],
      "additionalProperties": false
    },
    "method": "units.delete"
  },
  {
    "name": "units_get",
    "description": "Read a single placed unit by creation_number (returned from units_list).",
    "inputSchema": {
      "type": "object",
      "properties": {
        "creation_number": {
          "type": "integer"
        }
      },
      "required": [
        "creation_number"
      ],
      "additionalProperties": false
    },
    "method": "units.get"
  },
  {
    "name": "units_list",
    "description": "List placed units from war3mapUnits.doo. Returns creation_number, type_id, skin_id, player, position [x,y,z], rotation (radians), scale [sx,sy,sz], HP/mana pct, hero level, gold (for gold mines), and inventory slots. Pass `filter.type_id` (4-char rawcode) or `filter.player` (0-based slot; 27 = neutral) to narrow the result — recommended on maps with hundreds of units to keep payloads small.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "filter": {
          "type": "object",
          "properties": {
            "type_id": {
              "type": "string",
              "minLength": 4,
              "maxLength": 4
            },
            "player": {
              "type": "integer",
              "minimum": 0
            }
          },
          "additionalProperties": false
        }
      },
      "additionalProperties": false
    },
    "method": "units.list"
  },
  {
    "name": "units_move",
    "description": "Move a placed unit to (x, y, z) in WC3 game coordinates (origin at map center).",
    "inputSchema": {
      "type": "object",
      "properties": {
        "creation_number": {
          "type": "integer"
        },
        "x": {
          "type": "number"
        },
        "y": {
          "type": "number"
        },
        "z": {
          "type": "number"
        }
      },
      "required": [
        "creation_number",
        "x",
        "y",
        "z"
      ],
      "additionalProperties": false
    },
    "method": "units.move"
  },
  {
    "name": "units_rotate",
    "description": "Set a unit's facing angle, in radians around the Z axis.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "creation_number": {
          "type": "integer"
        },
        "rotation": {
          "type": "number",
          "description": "radians, Z-axis only"
        }
      },
      "required": [
        "creation_number",
        "rotation"
      ],
      "additionalProperties": false
    },
    "method": "units.rotate"
  },
  {
    "name": "units_scale",
    "description": "Set per-axis scale on a unit (1.0 = default).",
    "inputSchema": {
      "type": "object",
      "properties": {
        "creation_number": {
          "type": "integer"
        },
        "sx": {
          "type": "number"
        },
        "sy": {
          "type": "number"
        },
        "sz": {
          "type": "number"
        }
      },
      "required": [
        "creation_number",
        "sx",
        "sy",
        "sz"
      ],
      "additionalProperties": false
    },
    "method": "units.scale"
  },
  {
    "name": "units_set_field",
    "description": "Set a per-instance field on a placed unit (gold mine amount, hp%/mana%, owning player, hero level/stats, custom color, item drops). Undo-aware; persists across save/reopen. For scalar fields pass `value` (a number); for field='item_drops' pass `item_drops` instead.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "creation_number": {
          "type": "integer"
        },
        "field": {
          "type": "string",
          "enum": [
            "gold",
            "hp_pct",
            "mana_pct",
            "player",
            "hero_level",
            "hero_str",
            "hero_agi",
            "hero_int",
            "target_acquisition",
            "custom_color",
            "item_drops"
          ],
          "description": "which per-instance field to set"
        },
        "value": {
          "type": "number",
          "description": "new value for scalar fields; hp_pct/mana_pct/custom_color accept -1 for 'unit default / no override'"
        },
        "item_drops": {
          "type": "array",
          "items": {
            "type": "object",
            "properties": {
              "item_id": {
                "type": "string",
                "minLength": 4,
                "maxLength": 4
              },
              "chance": {
                "type": "integer",
                "minimum": 0
              }
            },
            "required": [
              "item_id",
              "chance"
            ],
            "additionalProperties": false
          },
          "description": "drop list when field='item_drops' (flattened to a single set)"
        }
      },
      "required": [
        "creation_number",
        "field"
      ],
      "additionalProperties": false
    },
    "method": "units.set_field"
  },
  {
    "name": "view_get_mode",
    "description": "Read the session's record of the viewport editing mode ('terrain' | 'doodad'). Tracks the last view_set_mode request (defaults to 'doodad'); the live renderer toggle is owned by the frontend, so this reflects the last request rather than a user's manual toolbar click.",
    "inputSchema": {
      "type": "object",
      "properties": {},
      "additionalProperties": false
    },
    "method": "view.get_mode"
  },
  {
    "name": "view_set_doodad_category_visible",
    "description": "Set the visibility of a doodad category in the viewport to an explicit value (idempotent — the visible flag is honored, not toggled). Category matches the View menu's entries (e.g. 'Trees/Destructibles', 'Structures', or '*' for all). Returns { ok, category, visible } echoing the resulting visibility.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "category": {
          "type": "string"
        },
        "visible": {
          "type": "boolean"
        }
      },
      "required": [
        "category",
        "visible"
      ],
      "additionalProperties": false
    },
    "method": "view.set_doodad_category_visible"
  },
  {
    "name": "view_set_mode",
    "description": "Set the viewport's editing mode to 'terrain' or 'doodad' (idempotent — calling it twice with the same mode lands on that mode, never oscillates). mode is required. Returns { ok, mode } echoing the resulting mode (also readable via view_get_mode).",
    "inputSchema": {
      "type": "object",
      "properties": {
        "mode": {
          "type": "string",
          "enum": [
            "terrain",
            "doodad"
          ]
        }
      },
      "required": [
        "mode"
      ],
      "additionalProperties": false
    },
    "method": "view.set_mode"
  },
  {
    "name": "window_set_title",
    "description": "Set the agent label segment in the OS window title. Lets multiple parallel agents identify their own instance at a glance. Pass an empty string to clear.",
    "inputSchema": {
      "type": "object",
      "properties": {
        "label": {
          "type": "string"
        }
      },
      "required": [
        "label"
      ],
      "additionalProperties": false
    },
    "method": "window.set_title"
  }
]
