---
type: vendor-doc
source_repo: "https://github.com/YElwiddi/warcraft-3-mcp-server"
source_path: "src/editor.ts"
ref: HEAD
---

/**
 * "Live" bridge to the actual game. The Warcraft III World Editor has no automation/plugin
 * API (unlike Unity), so we can't drive its GUI directly — but because every edit this server
 * makes is written into the real .w3x on disk, the loop is closed by *launching* the map:
 *   - openInEditor: open the map in the World Editor so the user sees/edits the change.
 *   - testMap:      run the map in Warcraft III to play the change immediately.
 */
import { existsSync } from 'node:fs';
import { spawn } from 'node:child_process';

/** Common install locations for the Reforged World Editor / game executables. */
const EDITOR_CANDIDATES = [
  'C:/Program Files (x86)/Warcraft III/_retail_/x86_64/World Editor.exe',
  'C:/Program Files/Warcraft III/_retail_/x86_64/World Editor.exe',
  'C:/Program Files (x86)/Warcraft III/World Editor.exe',
];
const GAME_CANDIDATES = [
  'C:/Program Files (x86)/Warcraft III/_retail_/x86_64/Warcraft III.exe',
  'C:/Program Files/Warcraft III/_retail_/x86_64/Warcraft III.exe',
];

function resolveExe(explicit: string | undefined, candidates: string[], label: string): string {
  if (explicit) {
    if (!existsSync(explicit)) throw new Error(`${label} not found at: ${explicit}`);
    return explicit;
  }
  const found = candidates.find((p) => existsSync(p));
  if (!found) {
    throw new Error(
      `Could not find ${label}. Pass exePath explicitly. Looked in:\n${candidates.join('\n')}`,
    );
  }
  return found;
}

/** Launch a detached process that outlives this server (so the editor/game stays open). */
function launchDetached(exe: string, args: string[]): number {
  const child = spawn(exe, args, { detached: true, stdio: 'ignore' });
  child.unref();
  return child.pid ?? -1;
}

/** Open the map in the World Editor. */
export function openInEditor(mapPath: string, exePath?: string): object {
  if (!existsSync(mapPath)) throw new Error(`Map not found: ${mapPath}`);
  const exe = resolveExe(exePath, EDITOR_CANDIDATES, 'World Editor.exe');
  const pid = launchDetached(exe, ['-loadfile', mapPath]);
  return { launched: 'World Editor', exe, mapPath, pid };
}

/** Run the map in Warcraft III. `-launch` skips the Battle.net launcher; `-window` opens
 *  windowed so it doesn't grab the whole screen. */
export function testMap(mapPath: string, exePath?: string, windowed = true): object {
  if (!existsSync(mapPath)) throw new Error(`Map not found: ${mapPath}`);
  const exe = resolveExe(exePath, GAME_CANDIDATES, 'Warcraft III.exe');
  const args = ['-launch', ...(windowed ? ['-window'] : []), '-loadfile', mapPath];
  const pid = launchDetached(exe, args);
  return {
    launched: 'Warcraft III',
    exe,
    mapPath,
    args,
    pid,
    note: 'If the game opens to the menu instead of the map, your build may require the map to live under Documents\\Warcraft III\\Maps — copy it there and retry, or open it from the editor.',
  };
}
