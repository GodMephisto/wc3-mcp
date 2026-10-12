// tests/Wc3.Tests/MapMinBuildCommandTests.cs
// The earliest build a map runs on, computed from the compat union index over temp schemas.
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public class MapMinBuildCommandTests
{
    private static string NewRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "wc3-min-build-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }

    /// <summary>ROC declares OldNative, the newer build adds NewNative on top.</summary>
    private static string TwoBuildRoot()
    {
        var root = NewRoot();
        GamedataFixtures.Save(root, "ROC-v1.00", natives: new[] { "OldNative" });
        GamedataFixtures.Save(root, "TFT-v1.31.0.00001", natives: new[] { "OldNative", "NewNative" });
        return root;
    }

    private static MapDocument MapWith(string script)
    {
        var doc = BlankMap.Create();
        doc.AddOrReplaceRawFile("war3map.j", Encoding.UTF8.GetBytes(script));
        return doc;
    }

    [Fact]
    public void A_map_using_only_an_old_identifier_runs_on_the_oldest_build()
    {
        var root = TwoBuildRoot();
        try
        {
            var doc = MapWith("""
                function main takes nothing returns nothing
                    call OldNative()
                endfunction
                """);

            var result = MapMinBuildCommand.Execute(doc, root);

            Assert.True(result.Known);
            Assert.Equal("ROC-v1.00", result.MinimumBuild);
            Assert.Contains(result.Requirements, r => r.Name == "OldNative" && r.Earliest == "ROC-v1.00");
            Assert.Contains("ROC-v1.00", result.Summary);
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public void A_map_using_a_new_identifier_needs_the_newer_build()
    {
        var root = TwoBuildRoot();
        try
        {
            var doc = MapWith("""
                function main takes nothing returns nothing
                    call OldNative()
                    call NewNative()
                endfunction
                """);

            var result = MapMinBuildCommand.Execute(doc, root);

            Assert.True(result.Known);
            // The newest among the identifiers' earliest declaring builds decides the floor.
            Assert.Equal("TFT-v1.31.0.00001", result.MinimumBuild);
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public void A_map_with_no_script_reports_no_answer_rather_than_a_build()
    {
        var root = TwoBuildRoot();
        try
        {
            var doc = BlankMap.Create();

            var result = MapMinBuildCommand.Execute(doc, root);

            Assert.False(result.Known);
            Assert.Null(result.MinimumBuild);
            Assert.Null(result.Summary);
        }
        finally { Directory.Delete(root, true); }
    }
}
