---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "mcp/src/bridge_client.ts"
ref: HEAD
---

import { createConnection, type Socket } from "node:net";
import { findSession, type BridgeLock } from "./discovery.js";

interface JsonRpcRequest {
  jsonrpc: "2.0";
  id: number;
  method: string;
  params: Record<string, unknown>;
}

interface JsonRpcResponse {
  jsonrpc: "2.0";
  id: number;
  result?: unknown;
  error?: { code: number; message: string; data?: unknown };
}

const REQUEST_TIMEOUT_MS = 30_000;

/** A live TCP connection to one wc3-forge bridge. Owned by the SessionPool. */
class BridgeConnection {
  private socket: Socket | null = null;
  private nextId = 1;
  private buffer = "";
  private pending = new Map<number, (response: JsonRpcResponse) => void>();
  private connectPromise: Promise<void> | null = null;

  constructor(public readonly lock: BridgeLock) {}

  async call(method: string, params: Record<string, unknown> = {}): Promise<unknown> {
    await this.ensureConnected();
    const id = this.nextId++;
    const request: JsonRpcRequest = {
      jsonrpc: "2.0",
      id,
      method,
      params: { ...params, _token: this.lock.token },
    };

    return new Promise((resolve, reject) => {
      const timeout = setTimeout(() => {
        this.pending.delete(id);
        reject(new Error(`Bridge call ${method} timed out after ${REQUEST_TIMEOUT_MS}ms`));
      }, REQUEST_TIMEOUT_MS);

      this.pending.set(id, (response) => {
        clearTimeout(timeout);
        if (response.error) {
          reject(new BridgeError(response.error.code, response.error.message, response.error.data));
        } else {
          resolve(response.result);
        }
      });

      this.socket!.write(JSON.stringify(request) + "\n");
    });
  }

  close(): void {
    if (this.socket) {
      this.socket.destroy();
      this.socket = null;
    }
    this.connectPromise = null;
  }

  private async ensureConnected(): Promise<void> {
    if (this.socket && !this.socket.destroyed) return;
    if (this.connectPromise) return this.connectPromise;
    this.connectPromise = this.connect();
    try {
      await this.connectPromise;
    } finally {
      this.connectPromise = null;
    }
  }

  private async connect(): Promise<void> {
    await new Promise<void>((resolve, reject) => {
      const sock = createConnection({ host: "127.0.0.1", port: this.lock.port });
      sock.setNoDelay(true);
      const onError = (err: Error) => {
        sock.removeListener("connect", onConnect);
        reject(err);
      };
      const onConnect = () => {
        sock.removeListener("error", onError);
        this.socket = sock;
        this.attachReader(sock);
        resolve();
      };
      sock.once("error", onError);
      sock.once("connect", onConnect);
    });
  }

  private attachReader(socket: Socket): void {
    socket.setEncoding("utf8");
    socket.on("data", (chunk: string) => {
      this.buffer += chunk;
      let newlineAt: number;
      while ((newlineAt = this.buffer.indexOf("\n")) !== -1) {
        const line = this.buffer.slice(0, newlineAt);
        this.buffer = this.buffer.slice(newlineAt + 1);
        if (line.trim().length === 0) continue;
        try {
          const response = JSON.parse(line) as JsonRpcResponse;
          const handler = this.pending.get(response.id);
          if (handler) {
            this.pending.delete(response.id);
            handler(response);
          }
        } catch {
          process.stderr.write(`wc3-forge-mcp: bad bridge frame: ${line}\n`);
        }
      }
    });
    socket.on("close", () => {
      const stillPending = [...this.pending.values()];
      this.pending.clear();
      this.socket = null;
      this.buffer = "";
      for (const handler of stillPending) {
        handler({
          jsonrpc: "2.0",
          id: -1,
          error: { code: -32000, message: "Bridge connection closed" },
        });
      }
    });
  }
}

/**
 * Pool of bridge connections, one per running wc3-forge. Keeps a "selected"
 * pid that all tool calls route to. When no pid is selected, calls route to
 * the oldest running instance.
 */
export class SessionPool {
  private connections = new Map<number, BridgeConnection>();
  private selectedPid: number | null = null;

  async call(method: string, params: Record<string, unknown> = {}): Promise<unknown> {
    const lock = await findSession(this.selectedPid ?? undefined);
    let conn = this.connections.get(lock.pid);
    if (!conn) {
      conn = new BridgeConnection(lock);
      this.connections.set(lock.pid, conn);
    }
    return conn.call(method, params);
  }

  /** Select a specific wc3-forge instance by pid for all subsequent calls. */
  select(pid: number): void {
    this.selectedPid = pid;
  }

  /** Reset selection — next call routes to the oldest running instance. */
  clearSelection(): void {
    this.selectedPid = null;
  }

  selected(): number | null {
    return this.selectedPid;
  }

  closeAll(): void {
    for (const conn of this.connections.values()) conn.close();
    this.connections.clear();
  }
}

export class BridgeError extends Error {
  constructor(public code: number, message: string, public data?: unknown) {
    super(message);
    this.name = "BridgeError";
  }
}
