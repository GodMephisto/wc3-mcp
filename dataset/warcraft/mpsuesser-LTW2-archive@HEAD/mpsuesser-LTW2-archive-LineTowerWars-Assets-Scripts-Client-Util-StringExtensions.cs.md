---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Util/StringExtensions.cs"
ref: HEAD
---

﻿using UnityEngine;
using System;

public static class StringExtensions {
    public static string AddColor(this string text, Color col) => $"<color={ColorHexFromUnityColor(col)}>{text}</color>";
    public static string ColorHexFromUnityColor(this Color unityColor) => $"#{ColorUtility.ToHtmlStringRGBA(unityColor)}";

    public static bool ContainsCaseInsensitive(this string source, string toCheck)
    {
        return source?.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}