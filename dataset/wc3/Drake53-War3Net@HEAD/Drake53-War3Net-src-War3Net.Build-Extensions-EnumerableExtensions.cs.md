---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build/Extensions/EnumerableExtensions.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<(T Obj, int Id)> IncludeId<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select((obj, id) => (obj, id));
        }
    }
}