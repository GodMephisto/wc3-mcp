// tests/Wc3.Tests/ScriptModernizeTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// script modernize, a known rename applied when the target declares the new spelling, and left
/// alone when it does not or when the name sits inside a string.
/// </summary>
public class ScriptModernizeTests
{
    private static MapDocument Doc(string jass) =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes(jass),
        }));

    private static string Read(MapDocument doc) => ScriptCommand.Read(doc).Source;

    [Fact]
    public void A_known_rename_is_applied_when_the_target_declares_the_new_name()
    {
        var temp = Path.Combine(Path.GetTempPath(), "wc3ctl-modernize-" + Guid.NewGuid().ToString("N"));
        try
        {
            ScriptTestSchema.Save(Path.Combine(temp, "new"), "TFT-v1.31.0.10420",
                new[] { "BlzSetUnitBaseDamage" });

            var doc = Doc(string.Join("\n",
                "function main takes nothing returns nothing",
                "    call SetUnitBaseDamage(udg_u, 10, 0)",
                "endfunction"));

            var r = ScriptModernizeCommand.Execute(doc, "TFT-v1.31.0.10420", temp);

            Assert.Equal(1, r.Rewrites);
            var one = Assert.Single(r.Changes);
            Assert.Equal("SetUnitBaseDamage", one.From);
            Assert.Equal("BlzSetUnitBaseDamage", one.To);
            Assert.Equal(2, one.Line);
            Assert.NotNull(r.NewScript);
            Assert.Contains("call BlzSetUnitBaseDamage(udg_u, 10, 0)", r.NewScript);
            // The document was updated in place, ready for the caller to save.
            Assert.Contains("call BlzSetUnitBaseDamage(udg_u, 10, 0)", Read(doc));
        }
        finally
        {
            if (Directory.Exists(temp)) Directory.Delete(temp, recursive: true);
        }
    }

    [Fact]
    public void Nothing_is_rewritten_when_the_target_lacks_the_new_name()
    {
        var temp = Path.Combine(Path.GetTempPath(), "wc3ctl-modernize-nomap-" + Guid.NewGuid().ToString("N"));
        try
        {
            ScriptTestSchema.Save(Path.Combine(temp, "old"), "TFT-v1.30.1.10211",
                new[] { "SetUnitBaseDamage" });

            var doc = Doc("function main takes nothing returns nothing\n"
                + "    call SetUnitBaseDamage(udg_u, 10, 0)\nendfunction\n");

            var r = ScriptModernizeCommand.Execute(doc, "TFT-v1.30.1.10211", temp);

            Assert.Equal(0, r.Rewrites);
            Assert.Empty(r.Changes);
            Assert.Null(r.NewScript);
            Assert.Contains("call SetUnitBaseDamage(udg_u, 10, 0)", Read(doc));
        }
        finally
        {
            if (Directory.Exists(temp)) Directory.Delete(temp, recursive: true);
        }
    }

    [Fact]
    public void A_name_inside_a_string_literal_is_not_rewritten()
    {
        var temp = Path.Combine(Path.GetTempPath(), "wc3ctl-modernize-string-" + Guid.NewGuid().ToString("N"));
        try
        {
            ScriptTestSchema.Save(Path.Combine(temp, "new"), "TFT-v1.31.0.10420",
                new[] { "BlzSetUnitBaseDamage" });

            var doc = Doc("function main takes nothing returns nothing\n"
                + "    call BJDebugMsg(\"SetUnitBaseDamage is gone\")\nendfunction\n");

            var r = ScriptModernizeCommand.Execute(doc, "TFT-v1.31.0.10420", temp);

            Assert.Equal(0, r.Rewrites);
            Assert.Contains("SetUnitBaseDamage is gone", Read(doc));
        }
        finally
        {
            if (Directory.Exists(temp)) Directory.Delete(temp, recursive: true);
        }
    }
}
