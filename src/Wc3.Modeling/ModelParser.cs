// src/Wc3.Modeling/ModelParser.cs
using System.Text;

namespace Wc3.Modeling;

/// <summary>
/// Entry point for parsing Warcraft III models. Dispatches to the binary MDX
/// reader (by "MDLX" magic) or the text MDL reader (by extension / content).
/// </summary>
public static class ModelParser
{
    public static Model3D Parse(byte[] bytes, string fileName)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        ArgumentNullException.ThrowIfNull(fileName);

        if (HasMdlxMagic(bytes))
            return MdxParser.Parse(bytes);

        var ext = Path.GetExtension(fileName);
        if (ext.Equals(".mdl", StringComparison.OrdinalIgnoreCase))
            return MdlParser.Parse(DecodeText(bytes));

        if (ext.Equals(".mdx", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException($"'{fileName}' has an .mdx extension but no MDLX magic.");

        throw new NotSupportedException($"Unrecognized model format for '{fileName}' (expected .mdx or .mdl).");
    }

    private static bool HasMdlxMagic(byte[] bytes)
        => bytes.Length >= 4 && bytes[0] == 'M' && bytes[1] == 'D' && bytes[2] == 'L' && bytes[3] == 'X';

    private static string DecodeText(byte[] bytes)
    {
        // MDL is ASCII in practice; UTF-8 handles it and any stray high bytes.
        var text = Encoding.UTF8.GetString(bytes);
        return text.Length > 0 && text[0] == '﻿' ? text[1..] : text; // strip BOM
    }
}
