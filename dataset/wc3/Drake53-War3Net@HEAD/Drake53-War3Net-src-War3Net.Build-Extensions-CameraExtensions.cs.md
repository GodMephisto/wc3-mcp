---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build/Extensions/CameraExtensions.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Extensions
{
    public static class CameraExtensions
    {
        public static string GetVariableName(this Camera camera)
        {
            return $"gg_cam_{camera.Name.Replace(' ', '_')}";
        }
    }
}