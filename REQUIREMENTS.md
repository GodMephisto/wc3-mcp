# Requirements

## SDK and runtime

.NET 8 SDK (8.0.x). Windows x64.

## Package references per project

### src/Wc3.Mcp

- ModelContextProtocol 1.4.1
- Microsoft.Extensions.Hosting 8.0.1

### src/Wc3.MapDocument

- War3Net.Build 6.1.1
- War3Net.IO.Mpq 6.0.3

### src/Wc3.GameData

- CascLib.NET 1.50.0.206-alpha.3

### src/Wc3.Modeling

- War3Net.Drawing.Blp 6.0.2

### src/Wc3.Render

- SixLabors.ImageSharp 3.1.12
- War3Net.Drawing.Blp 6.0.2

### src/Wc3.Commands

- War3Net.CodeAnalysis.Decompilers 6.1.1
- Microsoft.ML.OnnxRuntime 1.31.0 (the local retrieval index)
- Microsoft.ML.Tokenizers 2.0.1 (the BERT tokenizer for the embedding model)

Also depends on Wc3.MapDocument, Wc3.GameData, Wc3.Render, Wc3.Modeling.

### tests/Wc3.Tests

- Microsoft.NET.Test.Sdk 17.8.0
- xunit 2.5.3
- xunit.runner.visualstudio 2.5.3
- SixLabors.ImageSharp 3.1.12
- War3Net.CodeAnalysis.Decompilers 6.1.1

## Native dependencies

CascLib.dll ships with the `CascLib.NET` NuGet package (version 1.50.0.206-
alpha.3). It is a native x64 library packaged under
`runtimes/win-x64/native/CascLib.dll` inside the package.

## Warcraft III

Warcraft III Reforged is optional. Tools that take a `game_dir` argument use the
install to read base game data, and without one they work on the map's own data
and say so. `wc3-mcp doctor` shows which install, if any, was found.

## Context and RAG

The MCP serves the bundled Warcraft III references as resources, and an optional local index and
graph answer `docs_search` and the graph tools. `wc3ctl rag install` fetches two files over the
network, once, and verifies each.

- The embedding model, `BAAI/bge-small-en-v1.5`, about 130 MB, from Hugging Face.
- The corpus, about 61 MB, from the `dataset-v1` release asset, sha256 checked before extract.

No database and no service. The index and the graph are local files, queried in process with ONNX
Runtime. The graph holds nodes for a document and its sections, and edges for a document to its
section, a document to one it links, and a document to one it is close to by embedding. A Neo4j
backend, for Cypher and vault parity, is a possible later addition behind a flag, fed from these
same nodes and edges rather than a second extraction.

## An MCP client

Any stdio-based MCP client. Claude Code, Claude Desktop, or a custom client.

## Do not upgrade

- **CascLib.NET 1.50.0.206-alpha.3** (bundles native x64 dll. Newer alpha
  versions may drop the native bundle or change the ABI).
- **SixLabors.ImageSharp 3.1.12** (version 4.x carries a build-time license
  gate that requires a commercial license for some workloads).
- **War3Net.Build / War3Net.IO.Mpq 6.x** (newer major versions target
  newer .NET SDKs or change the public API).
