---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build/Extensions/MapBuilderExtensions.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Extensions
{
    public static class MapBuilderExtensions
    {
        public static void Compile(this MapBuilder mapBuilder, Compiler compiler, IEnumerable<string> luaSystemLibs)
        {
            mapBuilder.Map.CompileScript(compiler, luaSystemLibs);
        }
    }
}