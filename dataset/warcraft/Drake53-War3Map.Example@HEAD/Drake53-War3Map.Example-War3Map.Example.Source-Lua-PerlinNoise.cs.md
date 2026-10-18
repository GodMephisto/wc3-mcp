---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Map.Example"
source_path: "War3Map.Example.Source/Lua/PerlinNoise.cs"
ref: HEAD
---

﻿using War3Net.CodeAnalysis.Common;

namespace War3Map.Example.Source.Lua
{
    [NativeLuaMemberContainer]
    internal static class PerlinNoise
    {
        [NativeLuaMember("Noise.perlin1D")]
        public static extern float Noise(float x);

        [NativeLuaMember("Noise.perlin2D")]
        public static extern float Noise(float x, float y);

        [NativeLuaMember("Noise.perlin3D")]
        public static extern float Noise(float x, float y, float z);

        [NativeLuaMember("Noise.permutationInit")]
        public static extern void Init();
    }
}