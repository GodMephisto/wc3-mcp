// src/Wc3.MapDocument/ScriptText.cs
using System.Text;

namespace Wc3.Model;

/// <summary>
/// The one place that decides how a map script's bytes become text.
///
/// A map script has no declared encoding. It is a byte stream, and real maps carry bytes that are
/// not valid UTF-8, from names and messages written in a legacy code page. One map in this library
/// carries about 52,000 of them.
///
/// Decoding those as UTF-8 turns each invalid sequence into U+FFFD, and re-encoding U+FFFD emits
/// the three bytes EF BF BD, so the original byte is gone and every later round trip looks clean.
/// Measured through the Studio's Script panel before this existed, the five bytes E3 29 B5 F1 80
/// came back as EF BF BD 29 EF BF BD EF BF BD, a file five bytes longer with three bytes
/// destroyed, from opening the panel and saving without typing a character.
///
/// Latin-1 maps every byte 0 to 255 to the same code point and back, so it is lossless both ways.
/// The cost is that genuinely multi-byte text renders as mojibake in an editor, which is a display
/// fault in rare string literals rather than permanent damage to somebody's map.
///
/// This exists because the decision had been made correctly three times in private and incorrectly
/// in eight other places. Parsers registered war3map.j as Latin-1, HeroWiringAudit decoded it as
/// Latin-1, and ScriptPorter kept its own private Latin-1 constant, while the Script panel, the
/// linter, the bundler and four script analysers each reached for UTF-8. Agreement by coincidence
/// is not agreement, and the two that mattered were the ones that WRITE.
/// </summary>
public static class ScriptText
{
    /// <summary>The canonical script encoding. Use this rather than naming an encoding.</summary>
    public static readonly Encoding Encoding = Encoding.Latin1;

    /// <summary>Script bytes as text, losslessly.</summary>
    public static string GetString(byte[] bytes) => Encoding.GetString(bytes);

    /// <summary>Script text back to bytes, losslessly, provided the text came from
    /// <see cref="GetString"/> or contains only code points 0 to 255.</summary>
    public static byte[] GetBytes(string text) => Encoding.GetBytes(text);
}
