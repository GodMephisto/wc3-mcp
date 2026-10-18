---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/index.ts"
ref: HEAD
---

#!/usr/bin/env node
/**
 * wc3-mcp — an MCP server for inspecting and editing Warcraft III .w3x/.w3m maps.
 *
 * Transport: stdio. IMPORTANT: stdout is the MCP channel — never console.log to it.
 * All diagnostics go to stderr.
 */
import { dirname, basename, extname, join } from 'node:path';
import { readFile } from 'node:fs/promises';
import { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import { z } from 'zod';
import {
  openMap,
  saveMap,
  listFiles,
  readInfo,
  readPlacements,
  listUnits,
  placeUnit,
  modifyUnits,
  deleteUnits,
  readObjects,
  writeObjects,
  readScript,
  writeScript,
  readRawFile,
  writeRawFile,
} from './map.js';
import { readStrings, setString, addString } from './strings.js';
import { readRegions, addRegion, modifyRegions, deleteRegions } from './regions.js';
import { readCameras, addCamera, deleteCameras } from './cameras.js';
import { readSounds } from './sounds.js';
import { readTerrain } from './terrain.js';
import { readTriggers } from './triggers.js';
import { readImports } from './imports.js';
import { listDoodads, placeDoodad, modifyDoodads, deleteDoodads } from './doodads.js';
import { createObject } from './authoring.js';
import {
  addGuiTrigger,
  modifyGuiTrigger,
  type GuiCallSpec,
  type GuiFunctionSpec,
  type GuiParamSpec,
} from './gui-triggers.js';
import { setDurableScript } from './durable.js';
import { openInEditor, testMap } from './editor.js';
import { buildOverview } from './overview.js';

const OBJECT_KIND = z.enum([
  'units',
  'items',
  'abilities',
  'buffs',
  'destructables',
  'doodads',
  'upgrades',
]);

/** Default edited-map path: "<dir>/<base>.edited.<ext>" beside the source. */
function defaultOutPath(mapPath: string): string {
  const ext = extname(mapPath) || '.w3x';
  return join(dirname(mapPath), `${basename(mapPath, ext)}.edited${ext}`);
}

function ok(text: string) {
  return { content: [{ type: 'text' as const, text }] };
}
function json(obj: unknown) {
  return ok(JSON.stringify(obj, null, 2));
}
function fail(err: unknown) {
  const text = err instanceof Error ? err.message : String(err);
  return { content: [{ type: 'text' as const, text: `Error: ${text}` }], isError: true };
}

const server = new McpServer(
  { name: 'wc3-mcp', version: '1.1.0' },
  {
    instructions: [
      'wc3-mcp edits Warcraft III .w3x/.w3m map files directly on disk (offline file surgery; the',
      'World Editor has no automation API). Orient with wc3_overview first. Most edits (units, doodads,',
      "objects, regions, strings) write the editor's real data files and coexist with the user's own",
      'edits. Two rules matter most:',
      '',
      'DURABLE GAME LOGIC: always add JASS/Lua via wc3_set_durable_script (wc3_add_trigger is a durable',
      "alias for it). It stores the code in war3map.wct (the editor's own trigger storage, which",
      'survives World Editor re-saves and is editable in the Trigger Editor) AND mirrors it into the',
      'compiled war3map.j so it also runs immediately. Never write game logic into war3map.j alone: the',
      'editor regenerates that file on every save and would wipe it.',
      '  - Durable AUTO-RUN needs a custom-text trigger slot. If a',
      '    map has no slot, tell the user to do this ONCE: World Editor -> Trigger Editor (F4) -> New',
      '    Trigger -> Edit menu -> Convert to Custom Text -> Save. Then target it with slot:"trigger" and',
      '    triggerIndex (default 0); name a function InitTrig_<Name> and the editor auto-runs it at init.',
      '  - slot:"header" (the map-header "Custom Script Code") needs no setup and is durable, but in a',
      '    JASS map it does NOT auto-run (it holds definitions only) - use it for libraries, or when',
      '    running immediately via the mirror is enough.',
      '',
      'GUI TRIGGERS: wc3_read_triggers decodes the full tree (events/conditions/actions with their',
      'parameters). wc3_add_gui_trigger and wc3_modify_gui_trigger edit war3map.wtg durably: changes',
      'show up in the Trigger Editor and survive its re-saves. A byte-exact round-trip self-check runs',
      'before every write and refuses maps the parser cannot reproduce, so triggers cannot be corrupted.',
      'The editor compiles GUI triggers into the map script on its next save - after a GUI edit, tell',
      'the user to open the map and Save once before playtesting that change.',
      '',
      'EDIT WITH THE EDITOR CLOSED: the user must close the map in the World Editor before you edit it on',
      'disk, then reopen to see changes - an open editor keeps its own copy and overwrites yours on its',
      'next save. Playtesting via wc3_test_map is always safe. Edits default to a "<map>.edited.<ext>"',
      'copy; pass the source path as outPath to overwrite in place, and keep a backup first.',
    ].join('\n'),
  },
);

server.registerTool(
  'wc3_list_files',
  {
    title: 'List files in a WC3 map',
    description: 'List the internal files inside a Warcraft III .w3x/.w3m map archive.',
    inputSchema: { mapPath: z.string().describe('Absolute path to the .w3x/.w3m map file') },
  },
  async ({ mapPath }) => {
    try {
      const map = await openMap(mapPath, true);
      return ok(listFiles(map).sort().join('\n'));
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_map_info',
  {
    title: 'Read WC3 map info',
    description:
      'Parse war3map.w3i: map name, author, description, players, playable area, script language, flags, etc.',
    inputSchema: { mapPath: z.string().describe('Absolute path to the .w3x/.w3m map file') },
  },
  async ({ mapPath }) => {
    try {
      const map = await openMap(mapPath, true);
      return json(readInfo(map));
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_read_objects',
  {
    title: 'Read WC3 object data',
    description:
      'Read an object-data table as JSON. "kind" is one of units/items/abilities/buffs/destructables/doodads/upgrades. Returns a { original, custom } modification table.',
    inputSchema: {
      mapPath: z.string().describe('Absolute path to the .w3x/.w3m map file'),
      kind: OBJECT_KIND.describe('Which object-data table to read'),
    },
  },
  async ({ mapPath, kind }) => {
    try {
      const map = await openMap(mapPath, true);
      return json(readObjects(map, kind));
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_write_objects',
  {
    title: 'Write WC3 object data',
    description:
      'Write an object-data table back into the map and save. Pass the same { original, custom } shape returned by wc3_read_objects. Saves to outPath, or "<map>.edited.w3x" beside the source if omitted (pass the source path to overwrite in place).',
    inputSchema: {
      mapPath: z.string().describe('Absolute path to the source .w3x/.w3m map file'),
      kind: OBJECT_KIND.describe('Which object-data table to write'),
      data: z
        .object({
          original: z.record(z.string(), z.any()).optional(),
          custom: z.record(z.string(), z.any()).optional(),
        })
        .describe('The object modification table ({ original, custom })'),
      outPath: z.string().optional().describe('Output map path. Defaults to "<map>.edited.w3x".'),
    },
  },
  async ({ mapPath, kind, data, outPath }) => {
    try {
      const map = await openMap(mapPath, false);
      writeObjects(map, kind, data);
      const out = outPath ?? defaultOutPath(mapPath);
      await saveMap(map, out);
      return ok(`Wrote ${kind} table and saved map to:\n${out}`);
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_read_script',
  {
    title: 'Read WC3 map script',
    description: 'Read the map script (war3map.j JASS or war3map.lua).',
    inputSchema: { mapPath: z.string().describe('Absolute path to the .w3x/.w3m map file') },
  },
  async ({ mapPath }) => {
    try {
      const map = await openMap(mapPath, true);
      const { name, content } = readScript(map);
      return ok(`# ${name}\n\n${content}`);
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_write_script',
  {
    title: 'Write WC3 map script',
    description:
      'Replace the map script (war3map.j / war3map.lua) and save. Saves to outPath, or "<map>.edited.w3x" beside the source if omitted.',
    inputSchema: {
      mapPath: z.string().describe('Absolute path to the source .w3x/.w3m map file'),
      content: z.string().describe('New script source text'),
      outPath: z.string().optional().describe('Output map path. Defaults to "<map>.edited.w3x".'),
    },
  },
  async ({ mapPath, content, outPath }) => {
    try {
      const map = await openMap(mapPath, false);
      const name = writeScript(map, content);
      const out = outPath ?? defaultOutPath(mapPath);
      await saveMap(map, out);
      return ok(`Wrote ${name} and saved map to:\n${out}`);
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_read_file',
  {
    title: 'Read a raw file from a WC3 map',
    description: 'Extract one internal file as text (utf8) or base64.',
    inputSchema: {
      mapPath: z.string().describe('Absolute path to the .w3x/.w3m map file'),
      internalName: z.string().describe('Internal archive path, e.g. "war3map.w3i"'),
      encoding: z.enum(['utf8', 'base64']).default('base64').describe('How to return the bytes'),
    },
  },
  async ({ mapPath, internalName, encoding }) => {
    try {
      const map = await openMap(mapPath, true);
      return ok(readRawFile(map, internalName).toString(encoding));
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_replace_file',
  {
    title: 'Replace a file in a WC3 map',
    description:
      'Replace (or add) an internal file from a file on disk, then save. Useful for swapping models/textures/imports.',
    inputSchema: {
      mapPath: z.string().describe('Absolute path to the source .w3x/.w3m map file'),
      internalName: z.string().describe('Internal archive path to write, e.g. "war3map.j"'),
      srcPath: z.string().describe('Path to the file on disk whose bytes will be written'),
      outPath: z.string().optional().describe('Output map path. Defaults to "<map>.edited.w3x".'),
    },
  },
  async ({ mapPath, internalName, srcPath, outPath }) => {
    try {
      const map = await openMap(mapPath, false);
      writeRawFile(map, internalName, await readFile(srcPath));
      const out = outPath ?? defaultOutPath(mapPath);
      await saveMap(map, out);
      return ok(`Wrote ${internalName} and saved map to:\n${out}`);
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_read_placements',
  {
    title: 'Read placed units & doodads',
    description:
      'Read what is placed on the map in the World Editor: preplaced units/items (war3mapUnits.doo) and doodads/destructables (war3map.doo). Returns { units: [...], doodads: [...] } with type code, position, angle, owner, etc.',
    inputSchema: { mapPath: z.string().describe('Absolute path to the .w3x/.w3m map file') },
  },
  async ({ mapPath }) => {
    try {
      const map = await openMap(mapPath, true);
      return json(readPlacements(map));
    } catch (e) {
      return fail(e);
    }
  },
);

/** Reusable target selector for unit-editing tools. */
const UNIT_SELECTOR = z
  .object({
    creationNumber: z
      .number()
      .int()
      .optional()
      .describe('Match the unit with this creation number (the stable id shown by wc3_list_units)'),
    index: z.number().int().optional().describe('Match the unit at this array index'),
    type: z.string().optional().describe('Match every unit of this 4-char type code, e.g. "ugho"'),
  })
  .describe('Which unit(s) to target — provide at least one field; `type` can match many');

server.registerTool(
  'wc3_list_units',
  {
    title: 'List placed units/items',
    description:
      'List the preplaced units and items in war3mapUnits.doo with their index, creationNumber, type code, owner, position and facing. Use the index/creationNumber with wc3_modify_unit or wc3_delete_unit.',
    inputSchema: { mapPath: z.string().describe('Absolute path to the .w3x/.w3m map file') },
  },
  async ({ mapPath }) => {
    try {
      const map = await openMap(mapPath, true);
      return json(listUnits(map));
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_place_unit',
  {
    title: 'Place a unit/item',
    description:
      'Add a preplaced unit or item to the map at world coordinates (x, y). The type is a 4-char raw code, e.g. "ugho" (Ghoul), "hfoo" (Footman), or an item code. Saves to outPath, or "<map>.edited.<ext>" beside the source (pass the source path to overwrite in place).',
    inputSchema: {
      mapPath: z.string().describe('Absolute path to the source .w3x/.w3m map file'),
      type: z.string().describe('4-char unit/item type code, e.g. "ugho"'),
      x: z.number().describe('World X coordinate'),
      y: z.number().describe('World Y coordinate'),
      z: z.number().optional().describe('World Z (height); default 0'),
      facing: z.number().optional().describe('Facing angle in degrees; default 270'),
      player: z
        .number()
        .int()
        .optional()
        .describe('Owner: 0-based player slot (0 = Player 1/Red). Default 0'),
      scale: z.number().optional().describe('Uniform scale; default 1'),
      variation: z.number().int().optional().describe('Model variation; default 0'),
      hitpointsPercent: z.number().int().optional().describe('HP as a percent, or -1 for default'),
      manaPercent: z.number().int().optional().describe('Mana as a percent, or -1 for default'),
      skin: z.string().optional().describe('Reforged skin code; defaults to `type`'),
      outPath: z.string().optional().describe('Output map path. Defaults to "<map>.edited.<ext>".'),
    },
  },
  async ({ mapPath, type, x, y, z, facing, player, scale, variation, hitpointsPercent, manaPercent, skin, outPath }) => {
    try {
      const map = await openMap(mapPath, false);
      const placed = placeUnit(map, {
        type, x, y, z, facing, player, scale, variation, hitpointsPercent, manaPercent, skin,
      });
      const out = outPath ?? defaultOutPath(mapPath);
      await saveMap(map, out);
      return json({ saved: out, placed });
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_modify_unit',
  {
    title: 'Modify placed unit(s)',
    description:
      'Change fields on the unit(s) matched by `select` — move them, re-own them, rescale, re-face, swap type, or set HP/mana. Saves to outPath, or "<map>.edited.<ext>" beside the source (pass the source path to overwrite in place).',
    inputSchema: {
      mapPath: z.string().describe('Absolute path to the source .w3x/.w3m map file'),
      select: UNIT_SELECTOR,
      set: z
        .object({
          x: z.number().optional(),
          y: z.number().optional(),
          z: z.number().optional(),
          facing: z.number().optional().describe('Facing in degrees'),
          player: z.number().int().optional().describe('Owner player slot (0-based)'),
          scale: z.number().optional(),
          type: z.string().optional().describe('New 4-char type code'),
          hitpointsPercent: z.number().int().optional(),
          manaPercent: z.number().int().optional(),
        })
        .describe('Fields to change on every matched unit'),
      outPath: z.string().optional().describe('Output map path. Defaults to "<map>.edited.<ext>".'),
    },
  },
  async ({ mapPath, select, set, outPath }) => {
    try {
      const map = await openMap(mapPath, false);
      const updated = modifyUnits(map, select, set);
      if (updated.length === 0) return ok('No units matched the selector — nothing changed, no file written.');
      const out = outPath ?? defaultOutPath(mapPath);
      await saveMap(map, out);
      return json({ saved: out, updated });
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_delete_unit',
  {
    title: 'Delete placed unit(s)',
    description:
      'Remove the unit(s) matched by `select` from war3mapUnits.doo. Saves to outPath, or "<map>.edited.<ext>" beside the source (pass the source path to overwrite in place).',
    inputSchema: {
      mapPath: z.string().describe('Absolute path to the source .w3x/.w3m map file'),
      select: UNIT_SELECTOR,
      outPath: z.string().optional().describe('Output map path. Defaults to "<map>.edited.<ext>".'),
    },
  },
  async ({ mapPath, select, outPath }) => {
    try {
      const map = await openMap(mapPath, false);
      const removed = deleteUnits(map, select);
      if (removed.length === 0) return ok('No units matched the selector — nothing deleted, no file written.');
      const out = outPath ?? defaultOutPath(mapPath);
      await saveMap(map, out);
      return json({ saved: out, removed });
    } catch (e) {
      return fail(e);
    }
  },
);

// ---------------------------------------------------------------------------
// Convenience wrappers for the open→mutate→save pattern used by the edit tools.
// ---------------------------------------------------------------------------
const MAP_PATH = z.string().describe('Absolute path to the .w3x/.w3m map file');
const OUT_PATH = z
  .string()
  .optional()
  .describe('Output map path. Defaults to "<map>.edited.<ext>" beside the source; pass the source path to overwrite in place.');

/** Open readonly, read, return JSON. */
async function readTool(mapPath: string, fn: (m: Awaited<ReturnType<typeof openMap>>) => unknown) {
  const map = await openMap(mapPath, true);
  return json(fn(map));
}
/** Open writable, mutate the archive, save, return JSON ({ saved, ...result }). When the
 *  mutation reports an empty change set, skip the write and say so. */
async function writeTool(
  mapPath: string,
  outPath: string | undefined,
  fn: (m: Awaited<ReturnType<typeof openMap>>) => unknown,
) {
  const map = await openMap(mapPath, false);
  const result = fn(map);
  if (Array.isArray(result) && result.length === 0) {
    return ok('Nothing matched — no change made, no file written.');
  }
  const out = outPath ?? defaultOutPath(mapPath);
  await saveMap(map, out);
  return json({ saved: out, result });
}

// ===========================================================================
// Whole-map orientation
// ===========================================================================
server.registerTool(
  'wc3_overview',
  {
    title: 'Whole-map overview',
    description:
      'One-call snapshot of the map: info (name/author/players), file count, script language, terrain summary, placed unit/doodad counts, custom-object counts, and the names of regions/cameras/sounds plus a trigger summary. Use this first to orient on an unfamiliar map.',
    inputSchema: { mapPath: MAP_PATH },
  },
  async ({ mapPath }) => {
    try {
      return await readTool(mapPath, buildOverview);
    } catch (e) {
      return fail(e);
    }
  },
);

// ===========================================================================
// Strings (war3map.wts)
// ===========================================================================
server.registerTool(
  'wc3_read_strings',
  {
    title: 'Read the string table',
    description:
      'Read war3map.wts as a { id: text } map. Many map fields are stored as TRIGSTR_### references into this table (map/unit names, quest text, tooltips, …).',
    inputSchema: { mapPath: MAP_PATH },
  },
  async ({ mapPath }) => {
    try {
      return await readTool(mapPath, readStrings);
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_set_string',
  {
    title: 'Set a string-table entry',
    description:
      'Set the text of a TRIGSTR entry by numeric id (the ### in TRIGSTR_###), or omit id to append a new entry and get back its TRIGSTR_### token. Saves the map.',
    inputSchema: {
      mapPath: MAP_PATH,
      id: z.number().int().optional().describe('TRIGSTR numeric id to overwrite; omit to append a new one'),
      value: z.string().describe('The text to store'),
      outPath: OUT_PATH,
    },
  },
  async ({ mapPath, id, value, outPath }) => {
    try {
      return await writeTool(mapPath, outPath, (map) =>
        id === undefined ? addString(map, value) : { id: setString(map, id, value) },
      );
    } catch (e) {
      return fail(e);
    }
  },
);

// ===========================================================================
// Regions (war3map.w3r)
// ===========================================================================
server.registerTool(
  'wc3_read_regions',
  {
    title: 'Read regions',
    description: 'List the rectangular regions (war3map.w3r) — name, id, rect bounds, weather, ambient sound, color.',
    inputSchema: { mapPath: MAP_PATH },
  },
  async ({ mapPath }) => {
    try {
      return await readTool(mapPath, readRegions);
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_add_region',
  {
    title: 'Add a region',
    description:
      'Add a rectangular region defined by world-coordinate bounds (left/right/bottom/top). Returns the region with its assigned id (usable from JASS as gg_rct_<name>). Saves the map.',
    inputSchema: {
      mapPath: MAP_PATH,
      left: z.number(),
      right: z.number(),
      bottom: z.number(),
      top: z.number(),
      name: z.string().optional(),
      weatherEffect: z.string().optional().describe('4-char weather code, e.g. "RAhr"; omit for none'),
      ambientSound: z.string().optional(),
      color: z.array(z.number()).length(3).optional().describe('[r,g,b] 0-255 editor tint'),
      outPath: OUT_PATH,
    },
  },
  async ({ mapPath, outPath, ...r }) => {
    try {
      return await writeTool(mapPath, outPath, (map) =>
        addRegion(map, r as Parameters<typeof addRegion>[1]),
      );
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_modify_region',
  {
    title: 'Modify region(s)',
    description: 'Change bounds/name/weather/sound/color on the region(s) matched by id, name, or index. Saves the map.',
    inputSchema: {
      mapPath: MAP_PATH,
      select: z
        .object({ id: z.number().int().optional(), name: z.string().optional(), index: z.number().int().optional() })
        .describe('Which region(s) to change — provide one of id/name/index'),
      set: z
        .object({
          left: z.number().optional(),
          right: z.number().optional(),
          bottom: z.number().optional(),
          top: z.number().optional(),
          name: z.string().optional(),
          weatherEffect: z.string().optional(),
          ambientSound: z.string().optional(),
          color: z.array(z.number()).length(3).optional(),
        })
        .describe('Fields to change'),
      outPath: OUT_PATH,
    },
  },
  async ({ mapPath, select, set, outPath }) => {
    try {
      return await writeTool(mapPath, outPath, (map) =>
        modifyRegions(map, select, set as Parameters<typeof modifyRegions>[2]),
      );
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_delete_region',
  {
    title: 'Delete region(s)',
    description: 'Remove the region(s) matched by id, name, or index. Saves the map.',
    inputSchema: {
      mapPath: MAP_PATH,
      select: z
        .object({ id: z.number().int().optional(), name: z.string().optional(), index: z.number().int().optional() })
        .describe('Which region(s) to delete — provide one of id/name/index'),
      outPath: OUT_PATH,
    },
  },
  async ({ mapPath, select, outPath }) => {
    try {
      return await writeTool(mapPath, outPath, (map) => deleteRegions(map, select));
    } catch (e) {
      return fail(e);
    }
  },
);

// ===========================================================================
// Cameras (war3map.w3c)
// ===========================================================================
server.registerTool(
  'wc3_read_cameras',
  {
    title: 'Read cameras',
    description: 'List predefined cameras (war3map.w3c) — name, target, rotation, angle of attack, distance, fov, etc.',
    inputSchema: { mapPath: MAP_PATH },
  },
  async ({ mapPath }) => {
    try {
      return await readTool(mapPath, readCameras);
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_add_camera',
  {
    title: 'Add a camera',
    description:
      'Add a predefined camera at target (x,y). Unspecified fields use World Editor defaults (distance 1650, aoa 304, fov 70, rotation 90). Usable from JASS as gg_cam_<name>. Saves the map.',
    inputSchema: {
      mapPath: MAP_PATH,
      x: z.number(),
      y: z.number(),
      name: z.string().optional(),
      rotation: z.number().optional(),
      aoa: z.number().optional().describe('Angle of attack (degrees)'),
      distance: z.number().optional(),
      fov: z.number().optional().describe('Field of view (degrees)'),
      offsetZ: z.number().optional(),
      roll: z.number().optional(),
      outPath: OUT_PATH,
    },
  },
  async ({ mapPath, outPath, ...c }) => {
    try {
      return await writeTool(mapPath, outPath, (map) =>
        addCamera(map, c as Parameters<typeof addCamera>[1]),
      );
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_delete_camera',
  {
    title: 'Delete camera(s)',
    description: 'Remove the camera(s) matched by name or index. Saves the map.',
    inputSchema: {
      mapPath: MAP_PATH,
      select: z
        .object({ name: z.string().optional(), index: z.number().int().optional() })
        .describe('Which camera(s) to delete — provide name or index'),
      outPath: OUT_PATH,
    },
  },
  async ({ mapPath, select, outPath }) => {
    try {
      return await writeTool(mapPath, outPath, (map) => deleteCameras(map, select));
    } catch (e) {
      return fail(e);
    }
  },
);

// ===========================================================================
// Sounds / terrain / triggers / imports (reads)
// ===========================================================================
server.registerTool(
  'wc3_read_sounds',
  {
    title: 'Read sound definitions',
    description: 'List sound definitions (war3map.w3s) — variable name, file path, and playback settings.',
    inputSchema: { mapPath: MAP_PATH },
  },
  async ({ mapPath }) => {
    try {
      return await readTool(mapPath, readSounds);
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_read_terrain',
  {
    title: 'Read terrain summary',
    description:
      'Summarize war3map.w3e: tileset, ground/cliff tileset list, tilepoint dimensions, world size, and height/water ranges.',
    inputSchema: { mapPath: MAP_PATH },
  },
  async ({ mapPath }) => {
    try {
      return await readTool(mapPath, readTerrain);
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_read_triggers',
  {
    title: 'Read the trigger tree',
    description:
      'Read the GUI trigger tree (war3map.wtg) in full: categories, variables, and every trigger\'s ' +
      'events/conditions/actions with decoded parameters, plus custom-text bodies and each ' +
      'trigger\'s war3map.wct slot (wctIndex). The parameter JSON shapes are exactly what ' +
      'wc3_add_gui_trigger / wc3_modify_gui_trigger accept, so read → tweak → write back.',
    inputSchema: { mapPath: MAP_PATH },
  },
  async ({ mapPath }) => {
    try {
      return await readTool(mapPath, readTriggers);
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_read_imports',
  {
    title: 'Read the import table',
    description:
      'List custom imported files declared in war3map.imp (models, textures, sounds, …). Their bytes are reachable via wc3_read_file / wc3_replace_file.',
    inputSchema: { mapPath: MAP_PATH },
  },
  async ({ mapPath }) => {
    try {
      return await readTool(mapPath, readImports);
    } catch (e) {
      return fail(e);
    }
  },
);

// ===========================================================================
// Doodad/destructable placement (war3map.doo)
// ===========================================================================
server.registerTool(
  'wc3_list_doodads',
  {
    title: 'List placed doodads',
    description:
      'List preplaced doodads/destructables in war3map.doo with index, editorId, type code, position, angle, scale, variation, life. Optionally filter by type. Use index/editorId with wc3_modify_doodad / wc3_delete_doodad.',
    inputSchema: { mapPath: MAP_PATH, type: z.string().optional().describe('Only list this 4-char type code') },
  },
  async ({ mapPath, type }) => {
    try {
      return await readTool(mapPath, (map) => listDoodads(map, type));
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_place_doodad',
  {
    title: 'Place a doodad',
    description:
      'Add a doodad/destructable at (x,y). Type is a 4-char code, e.g. "LTlt" (a tree) or "ATtr" (a rock). Saves the map.',
    inputSchema: {
      mapPath: MAP_PATH,
      type: z.string().describe('4-char doodad/destructable code'),
      x: z.number(),
      y: z.number(),
      z: z.number().optional(),
      angle: z.number().optional().describe('Facing in degrees; default 270'),
      scale: z.number().optional(),
      variation: z.number().int().optional(),
      life: z.number().int().optional().describe('Life as a percent; default 100'),
      outPath: OUT_PATH,
    },
  },
  async ({ mapPath, outPath, ...p }) => {
    try {
      return await writeTool(mapPath, outPath, (map) =>
        placeDoodad(map, p as Parameters<typeof placeDoodad>[1]),
      );
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_modify_doodad',
  {
    title: 'Modify doodad(s)',
    description: 'Change fields on the doodad(s) matched by editorId, index, or type. Saves the map.',
    inputSchema: {
      mapPath: MAP_PATH,
      select: z
        .object({ editorId: z.number().int().optional(), index: z.number().int().optional(), type: z.string().optional() })
        .describe('Which doodad(s) — provide one of editorId/index/type (type can match many)'),
      set: z
        .object({
          x: z.number().optional(),
          y: z.number().optional(),
          z: z.number().optional(),
          angle: z.number().optional(),
          scale: z.number().optional(),
          type: z.string().optional(),
          variation: z.number().int().optional(),
          life: z.number().int().optional(),
        })
        .describe('Fields to change on every matched doodad'),
      outPath: OUT_PATH,
    },
  },
  async ({ mapPath, select, set, outPath }) => {
    try {
      return await writeTool(mapPath, outPath, (map) =>
        modifyDoodads(map, select, set as Parameters<typeof modifyDoodads>[2]),
      );
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_delete_doodad',
  {
    title: 'Delete doodad(s)',
    description: 'Remove the doodad(s) matched by editorId, index, or type. Saves the map.',
    inputSchema: {
      mapPath: MAP_PATH,
      select: z
        .object({ editorId: z.number().int().optional(), index: z.number().int().optional(), type: z.string().optional() })
        .describe('Which doodad(s) — provide one of editorId/index/type'),
      outPath: OUT_PATH,
    },
  },
  async ({ mapPath, select, outPath }) => {
    try {
      return await writeTool(mapPath, outPath, (map) => deleteDoodads(map, select));
    } catch (e) {
      return fail(e);
    }
  },
);

// ===========================================================================
// High-level authoring (object data + script triggers)
// ===========================================================================
server.registerTool(
  'wc3_create_object',
  {
    title: 'Create a custom object',
    description:
      'Create a custom object (unit/item/ability/buff/destructable/doodad/upgrade) derived from a base type, setting any fields. newId is a fresh 4-char code (e.g. "x000"); baseId is the 4-char code to copy (e.g. "hfoo"). fields are { id (4-char field code), type, value, level?, column? }. Saves the map.',
    inputSchema: {
      mapPath: MAP_PATH,
      kind: OBJECT_KIND,
      baseId: z.string().length(4).describe('Base type to derive from, e.g. "hfoo"'),
      newId: z.string().length(4).describe('New 4-char id for the custom object, e.g. "x000"'),
      fields: z
        .array(
          z.object({
            id: z.string().length(4).describe('4-char field code, e.g. "unam" (name), "uhpm" (max HP)'),
            type: z.enum(['int', 'real', 'unreal', 'string']),
            value: z.union([z.string(), z.number()]),
            level: z.number().int().optional(),
            column: z.number().int().optional(),
          }),
        )
        .describe('Field overrides for the new object'),
      outPath: OUT_PATH,
    },
  },
  async ({ mapPath, kind, baseId, newId, fields, outPath }) => {
    try {
      return await writeTool(mapPath, outPath, (map) => createObject(map, kind, baseId, newId, fields));
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_add_trigger',
  {
    title: 'Add a script trigger (durable)',
    description:
      'Add new game logic to the map DURABLY. Stores your JASS/Lua as a custom-text trigger body ' +
      'in war3map.wct (survives World Editor re-saves and is editable in the Trigger Editor) and ' +
      'mirrors it into the map script so it runs immediately. This is a thin alias for ' +
      'wc3_set_durable_script with slot:"trigger" (index 0): the map must already have at least one ' +
      'custom-text trigger slot — make one in the editor once (New Trigger → Edit → Convert to ' +
      'Custom Text) and Save. `code` is complete function definition(s); set entryFunction ' +
      '(auto-detected as InitTrig_<name>) to run at init. For library code on a map with no trigger ' +
      'slot, use wc3_set_durable_script with slot:"header". Saves the map.',
    inputSchema: {
      mapPath: MAP_PATH,
      code: z.string().describe('Complete JASS/Lua function definition(s) to add'),
      entryFunction: z
        .string()
        .optional()
        .describe('Name of a zero-arg function in `code` to call at map init; omit to only append'),
      outPath: OUT_PATH,
    },
  },
  async ({ mapPath, code, entryFunction, outPath }) => {
    try {
      return await writeTool(mapPath, outPath, (map) =>
        setDurableScript(map, { code, slot: 'trigger', entryFunction }),
      );
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_set_durable_script',
  {
    title: 'Set durable script (survives editor re-saves)',
    description:
      'Install custom JASS/Lua that BOTH runs now AND survives a re-save in the World Editor — ' +
      'and shows up there, editable by the user. Unlike wc3_add_trigger (which writes only the ' +
      'compiled war3map.j and is wiped when the editor regenerates it), this writes the durable ' +
      'source in war3map.wct and mirrors it into war3map.j, leaving war3map.wtg untouched. ' +
      'slot:"trigger" stores it as a custom-text trigger body (auto-runs at init, durably) — the ' +
      'map must already contain at least one custom-text trigger (make one in the editor once, ' +
      'e.g. named "MCPDurable", and Save); target it with triggerIndex (default 0). slot:"header" ' +
      'stores it in the map-header "Custom Script Code" (no setup needed). For slot:"trigger", make ' +
      '`code` define `function InitTrig_<name>` and it is auto-wired to run. Idempotent: repeat ' +
      'calls replace the prior block rather than duplicating it. Saves the map.',
    inputSchema: {
      mapPath: MAP_PATH,
      code: z.string().describe('Complete JASS/Lua function definition(s) to install durably'),
      slot: z
        .enum(['trigger', 'header'])
        .default('trigger')
        .describe('"trigger" = custom-text trigger body (durable auto-run); "header" = Custom Script Code (zero setup)'),
      triggerIndex: z
        .number()
        .int()
        .optional()
        .describe('Which custom-text trigger slot (0-based) when slot:"trigger". Default 0.'),
      entryFunction: z
        .string()
        .optional()
        .describe('Zero-arg function in `code` to call at init in the war3map.j mirror; auto-detected as InitTrig_<name> for slot:"trigger"'),
      mirror: z
        .boolean()
        .optional()
        .describe('Also mirror into war3map.j for immediate effect (default true)'),
      outPath: OUT_PATH,
    },
  },
  async ({ mapPath, code, slot, triggerIndex, entryFunction, mirror, outPath }) => {
    try {
      return await writeTool(mapPath, outPath, (map) =>
        setDurableScript(map, { code, slot, triggerIndex, entryFunction, mirror }),
      );
    } catch (e) {
      return fail(e);
    }
  },
);

// ===========================================================================
// GUI trigger editing (war3map.wtg — durable, editor-visible)
// ===========================================================================
const GUI_PARAM: z.ZodType<GuiParamSpec> = z.lazy(() =>
  z.object({
    preset: z.string().optional().describe('Editor preset, e.g. "PlayerUnitEventAttacked", "OperatorEqualENE"'),
    variable: z.string().optional().describe('Variable name as shown in the editor, or a generated global like "gg_unit_hfoo_0000"'),
    value: z.string().optional().describe('Literal: text, number, or 4-char rawcode like "hfoo"'),
    call: GUI_CALL.optional().describe('Nested function call, e.g. {"function":"GetTriggerUnit"}'),
    index: GUI_PARAM.optional().describe('Array index (variable parameters only)'),
  }),
);
const GUI_CALL: z.ZodType<GuiCallSpec> = z.lazy(() =>
  z.object({
    function: z.string().describe('TriggerData call name, e.g. "GetUnitTypeId"'),
    params: z.array(GUI_PARAM).optional(),
  }),
);
const GUI_FUNCTION = z.object({
  function: z.string().describe('GUI function name from TriggerData, e.g. "KillUnit"'),
  params: z.array(GUI_PARAM).optional().describe('One entry per parameter; count must match the function'),
}) as z.ZodType<GuiFunctionSpec>;
const GUI_FUNCTIONS = z.array(GUI_FUNCTION);

server.registerTool(
  'wc3_add_gui_trigger',
  {
    title: 'Create a GUI trigger (durable)',
    description:
      'Create a real GUI trigger in war3map.wtg — it appears in the Trigger Editor, is editable ' +
      'there, and survives editor re-saves. Events/conditions/actions are given as TriggerData ' +
      'function names plus parameters ({preset}|{variable}|{value}|{call}); wc3_read_triggers ' +
      'shows existing triggers in this exact format. Example action — kill the attacked unit: ' +
      '{"function":"KillUnit","params":[{"call":{"function":"GetTriggerUnit"}}]}. Names and ' +
      'parameter counts are validated, and a byte-exact round-trip self-check protects the map. ' +
      'The editor compiles the trigger into the map script on its next save (open + Save once, ' +
      'then playtest). Saves the map.',
    inputSchema: {
      mapPath: MAP_PATH,
      name: z.string().describe('Trigger name (must not already exist)'),
      description: z.string().optional().describe('Trigger comment text'),
      category: z.string().optional().describe('Category name to file it under (default: first category)'),
      enabled: z.boolean().optional().describe('Default true'),
      initiallyOff: z.boolean().optional().describe('Start turned off (default false)'),
      runOnInit: z.boolean().optional().describe('Run on map initialization (default false)'),
      events: GUI_FUNCTIONS.optional(),
      conditions: GUI_FUNCTIONS.optional(),
      actions: GUI_FUNCTIONS.optional(),
      outPath: OUT_PATH,
    },
  },
  async ({ mapPath, outPath, ...spec }) => {
    try {
      return await writeTool(mapPath, outPath, (map) => addGuiTrigger(map, spec));
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_modify_gui_trigger',
  {
    title: 'Modify a GUI trigger (durable)',
    description:
      'Edit an existing GUI trigger in war3map.wtg durably (visible in the Trigger Editor, ' +
      'survives its re-saves). Select by name or by index (wc3_read_triggers order). Ops: rename, ' +
      'description, enabled, initiallyOff, runOnInit; replaceParamValue swaps a parameter value ' +
      'everywhere in the trigger (e.g. {"from":"Aakb","to":"AHbh"} to change an ability rawcode); ' +
      'setEvents/setConditions/setActions replace that whole section (same format wc3_read_triggers ' +
      'emits). A byte-exact round-trip self-check protects the map. The editor recompiles the map ' +
      'script on its next save (open + Save once, then playtest). Saves the map.',
    inputSchema: {
      mapPath: MAP_PATH,
      trigger: z.union([z.string(), z.number().int()]).describe('Trigger name, or 0-based index in wc3_read_triggers order'),
      rename: z.string().optional(),
      description: z.string().optional(),
      enabled: z.boolean().optional(),
      initiallyOff: z.boolean().optional(),
      runOnInit: z.boolean().optional(),
      replaceParamValue: z
        .object({
          from: z.string().describe('Exact current parameter value (preset/variable/literal), e.g. "Aakb"'),
          to: z.string().describe('New value, e.g. "AHbh"'),
        })
        .optional()
        .describe('Swap a parameter value everywhere it appears in this trigger'),
      setEvents: GUI_FUNCTIONS.optional().describe('Replace ALL events with this list'),
      setConditions: GUI_FUNCTIONS.optional().describe('Replace ALL conditions with this list'),
      setActions: GUI_FUNCTIONS.optional().describe('Replace ALL actions with this list'),
      outPath: OUT_PATH,
    },
  },
  async ({ mapPath, outPath, ...spec }) => {
    try {
      return await writeTool(mapPath, outPath, (map) => modifyGuiTrigger(map, spec));
    } catch (e) {
      return fail(e);
    }
  },
);

// ===========================================================================
// Live bridge: open the map in the editor / run it in the game
// ===========================================================================
server.registerTool(
  'wc3_open_in_editor',
  {
    title: 'Open the map in the World Editor',
    description:
      'Launch the Warcraft III World Editor on the given map so the user can see/verify the edits. (The World Editor has no automation API; this just opens the edited file.) Pass exePath to override auto-detection.',
    inputSchema: { mapPath: MAP_PATH, exePath: z.string().optional().describe('Path to World Editor.exe') },
  },
  async ({ mapPath, exePath }) => {
    try {
      return json(openInEditor(mapPath, exePath));
    } catch (e) {
      return fail(e);
    }
  },
);

server.registerTool(
  'wc3_test_map',
  {
    title: 'Run the map in Warcraft III',
    description:
      'Launch Warcraft III running the given map (windowed by default) so the change can be played immediately. Pass exePath to override auto-detection, or windowed=false for fullscreen.',
    inputSchema: {
      mapPath: MAP_PATH,
      exePath: z.string().optional().describe('Path to Warcraft III.exe'),
      windowed: z.boolean().optional().describe('Run windowed (default true)'),
    },
  },
  async ({ mapPath, exePath, windowed }) => {
    try {
      return json(testMap(mapPath, exePath, windowed ?? true));
    } catch (e) {
      return fail(e);
    }
  },
);

const transport = new StdioServerTransport();
await server.connect(transport);
console.error('wc3-mcp server running on stdio');
