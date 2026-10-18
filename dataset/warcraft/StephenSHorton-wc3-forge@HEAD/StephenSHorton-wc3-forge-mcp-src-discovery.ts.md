---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "mcp/src/discovery.ts"
ref: HEAD
---

import { promises as fs } from "node:fs";
import { homedir } from "node:os";
import { join } from "node:path";

export interface BridgeLock {
  pid: number;
  port: number;
  token: string;
  started_at: string;
}

// Lock directory resolution mirrors the Go side (internal/bridge/lockfile.go).
// Override via WC3FORGE_MCP_LOCK_DIR.
const lockDir = (): string => {
  const override = process.env.WC3FORGE_MCP_LOCK_DIR;
  if (override) return override;
  return join(homedir(), ".wc3-forge", "mcp");
};

export async function listSessions(): Promise<BridgeLock[]> {
  const dir = lockDir();
  let entries: string[];
  try {
    entries = await fs.readdir(dir);
  } catch (err) {
    const e = err as NodeJS.ErrnoException;
    if (e.code === "ENOENT") return [];
    throw err;
  }

  const locks: BridgeLock[] = [];
  for (const name of entries) {
    if (!name.endsWith(".lock")) continue;
    const path = join(dir, name);
    let raw: string;
    try {
      raw = await fs.readFile(path, "utf8");
    } catch {
      continue;
    }
    let lock: BridgeLock;
    try {
      lock = JSON.parse(raw) as BridgeLock;
    } catch {
      continue;
    }
    if (!isAlive(lock.pid)) {
      // Stale — the Go side prunes these on startup too, but be defensive here.
      fs.unlink(path).catch(() => undefined);
      continue;
    }
    locks.push(lock);
  }

  locks.sort((a, b) => a.started_at.localeCompare(b.started_at));
  return locks;
}

export async function findSession(pid: number | undefined): Promise<BridgeLock> {
  const all = await listSessions();
  if (all.length === 0) {
    throw new BridgeNotRunningError(
      `No wc3-forge bridges found in ${lockDir()}. Launch wc3-forge with the MCP bridge enabled (the default).`
    );
  }
  if (pid === undefined) {
    // Default: oldest first. Tools can override with session_select.
    return all[0]!;
  }
  const match = all.find((l) => l.pid === pid);
  if (!match) {
    throw new BridgeNotRunningError(
      `No wc3-forge bridge for pid=${pid}. Active pids: ${all.map((l) => l.pid).join(", ")}.`
    );
  }
  return match;
}

function isAlive(pid: number): boolean {
  try {
    process.kill(pid, 0);
    return true;
  } catch (err) {
    const e = err as NodeJS.ErrnoException;
    return e.code === "EPERM";
  }
}

export class BridgeNotRunningError extends Error {
  constructor(message: string) {
    super(message);
    this.name = "BridgeNotRunningError";
  }
}
