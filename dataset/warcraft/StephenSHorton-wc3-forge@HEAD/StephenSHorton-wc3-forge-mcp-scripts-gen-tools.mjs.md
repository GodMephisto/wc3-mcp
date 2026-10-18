---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "mcp/scripts/gen-tools.mjs"
ref: HEAD
---

// gen-tools.mjs — generate the Go MCP server's tool catalog from the
// TypeScript source of truth (mcp/src/tools.ts).
//
// The Go binary's `--mcp` mode (internal/mcpserver) serves tools/list from a
// committed, go:embed-ed internal/mcpserver/tools.json. Rather than hand-port
// ~179 zod schemas into Go (and keep them in sync forever), we drive the real
// registerTools() with a stub server that records each tool's name, description,
// JSON Schema (via zod-to-json-schema), and the bridge method it forwards to
// (recovered from the __bridgeMethod tag wrap() attaches in tools.ts).
//
// Run via `npm run gen:tools` (which builds dist/ first). Re-run after any edit
// to tools.ts and commit the regenerated tools.json.

import { mkdirSync, writeFileSync } from "node:fs";
import { fileURLToPath } from "node:url";
import { dirname, join } from "node:path";
import { z } from "zod";
import { zodToJsonSchema } from "zod-to-json-schema";
import { registerTools } from "../dist/tools.js";

/** @type {{name:string, description:string, inputSchema:object, method:string|null}[]} */
const tools = [];

// Stub that satisfies the slice of McpServer that registerTools uses: just
// .tool(name, description, zodRawShape, handler). We never invoke handlers —
// the method is read off the __bridgeMethod tag, so no bridge connection opens.
const stubServer = {
  tool(name, description, shape, handler) {
    const schema = zodToJsonSchema(z.object(shape ?? {}), {
      target: "jsonSchema7",
      $refStrategy: "none",
    });
    // MCP inputSchema is a bare object schema; drop the JSON Schema header.
    delete schema.$schema;
    const method =
      handler && typeof handler === "function" && handler.__bridgeMethod
        ? handler.__bridgeMethod
        : null;
    tools.push({ name, description, inputSchema: schema, method });
  },
};

registerTools(stubServer);
tools.sort((a, b) => a.name.localeCompare(b.name));

const here = dirname(fileURLToPath(import.meta.url));
const outPath = join(here, "..", "..", "internal", "mcpserver", "tools.json");
mkdirSync(dirname(outPath), { recursive: true });
writeFileSync(outPath, JSON.stringify(tools, null, 2) + "\n");

const forwarded = tools.filter((t) => t.method).length;
const local = tools.length - forwarded;
// Use stderr so this never pollutes any stdout MCP channel if run in a pipeline.
process.stderr.write(
  `gen-tools: wrote ${tools.length} tools (${forwarded} forwarded, ${local} local) -> ${outPath}\n`,
);
