// tests/Wc3.Tests/ScriptApiUsageTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// script api-usage, checked against a synthetic target schema the same way compat is.
/// </summary>
public class ScriptApiUsageTests
{
    private static readonly string Script = string.Join("\n",
        "function InitTriggers takes nothing returns nothing",  // 1
        "    call KnownNative()",                               // 2
        "    call NewNative()",                                 // 3
        "endfunction");                                         // 4

    [Fact]
    public void A_native_the_target_lacks_is_reported_and_one_it_has_is_not()
    {
        var temp = Path.Combine(Path.GetTempPath(), "wc3ctl-api-usage-" + Guid.NewGuid().ToString("N"));
        try
        {
            ScriptTestSchema.Save(Path.Combine(temp, "old"), "TFT-v1.30.1.10211", new[] { "KnownNative" });
            ScriptTestSchema.Save(Path.Combine(temp, "new"), "TFT-v1.31.0.10420",
                new[] { "KnownNative", "NewNative" });

            var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
            {
                ["war3map.j"] = Encoding.UTF8.GetBytes(Script),
            }));

            var r = ScriptApiUsageCommand.Execute(doc, "TFT-v1.30.1.10211", temp);

            Assert.Equal("TFT-v1.30.1.10211", r.TargetBuild);
            Assert.Equal(2, r.Used);
            var one = Assert.Single(r.Usages);
            Assert.Equal("NewNative", one.Name);
            Assert.Equal("native", one.Kind);
            Assert.Equal(3, one.Line);
        }
        finally
        {
            if (Directory.Exists(temp)) Directory.Delete(temp, recursive: true);
        }
    }

    [Fact]
    public void An_unknown_name_no_schema_declares_is_not_a_finding()
    {
        var temp = Path.Combine(Path.GetTempPath(), "wc3ctl-api-usage-unknown-" + Guid.NewGuid().ToString("N"));
        try
        {
            ScriptTestSchema.Save(Path.Combine(temp, "old"), "TFT-v1.30.1.10211", new[] { "KnownNative" });

            var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
            {
                ["war3map.j"] = Encoding.UTF8.GetBytes(
                    "function InitTriggers takes nothing returns nothing\n"
                    + "    call SomethingNoBuildHas()\n"
                    + "endfunction\n"),
            }));

            var r = ScriptApiUsageCommand.Execute(doc, "TFT-v1.30.1.10211", temp);
            Assert.Empty(r.Usages);
        }
        finally
        {
            if (Directory.Exists(temp)) Directory.Delete(temp, recursive: true);
        }
    }
}
