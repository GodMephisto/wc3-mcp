# Requirements

## SDK and runtime

.NET 8 SDK (8.0.x). Windows x64.

## Package references per project

### src/Wc3.Mcp

- ModelContextProtocol 1.4.1
- Microsoft.Extensions.Hosting 8.0.1

### src/Wc3.MapDocument

- War3Net.Build 6.0.3
- War3Net.IO.Mpq 6.0.3

### src/Wc3.GameData

- CascLib.NET 1.50.0.206-alpha.3

### src/Wc3.Modeling

- War3Net.Drawing.Blp 6.0.2

### src/Wc3.Render

- SixLabors.ImageSharp 3.1.12
- War3Net.Drawing.Blp 6.0.2

### src/Wc3.Commands

No direct PackageReference. Depends on Wc3.MapDocument, Wc3.GameData,
Wc3.Render, Wc3.Modeling.

### tests/Wc3.Tests

- Microsoft.NET.Test.Sdk 17.8.0
- xunit 2.5.3
- xunit.runner.visualstudio 2.5.3
- SixLabors.ImageSharp 3.1.12

## Native dependencies

CascLib.dll ships with the `CascLib.NET` NuGet package (version 1.50.0.206-
alpha.3). It is a native x64 library packaged under
`runtimes/win-x64/native/CascLib.dll` inside the package.

## Warcraft III

Warcraft III Reforged is optional. The following tools need a running install:

- uabi_profile (with game_dir)
- object_list (with game_dir)
- object_get (with game_dir)
- render_model (with game_dir)
- port_unit (with game_dir)
- palette_doodad (with game_dir)
- audit_map (with game_dir)
- trigger_catalog_list (with game_dir)
- trigger_catalog_describe (with game_dir)

## An MCP client

Any stdio-based MCP client. Claude Code, Claude Desktop, or a custom client.

## Do not upgrade

- **CascLib.NET 1.50.0.206-alpha.3** (bundles native x64 dll. Newer alpha
  versions may drop the native bundle or change the ABI).
- **SixLabors.ImageSharp 3.1.12** (version 4.x carries a build-time license
  gate that requires a commercial license for some workloads).
- **War3Net.Build / War3Net.IO.Mpq 6.x** (newer major versions target
  newer .NET SDKs or change the public API).
