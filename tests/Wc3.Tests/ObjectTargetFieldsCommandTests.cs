// tests/Wc3.Tests/ObjectTargetFieldsCommandTests.cs
// The object editor's target-build data, over temp schema folders so nothing depends on an install.
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public class ObjectTargetFieldsCommandTests
{
    private static string NewRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "wc3-object-target-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }

    [Fact]
    public void Builds_flags_which_builds_carry_object_data()
    {
        var root = NewRoot();
        try
        {
            GamedataFixtures.Save(root, "TFT-v1.30.0.00001", slk: "units/unitmetadata.slk",
                fields: new[] { GamedataFixtures.Field("ugol", "int", "1", "300", true) });
            GamedataFixtures.Save(root, "TFT-v1.35.0.00001", natives: new[] { "NewNative" });

            var builds = ObjectTargetFieldsCommand.Builds(root);

            Assert.Equal(2, builds.Count);
            Assert.Equal("TFT-v1.30.0.00001", builds[0].Build);
            Assert.True(builds[0].HasObjectData);
            Assert.False(builds[1].HasObjectData);
            // Oldest first, as TargetBuildSettings.List orders them.
            Assert.Contains("TFT", builds[0].Era);
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public void A_build_with_no_object_data_says_so_and_is_not_treated_as_having_one()
    {
        var root = NewRoot();
        try
        {
            GamedataFixtures.Save(root, "TFT-v1.35.0.00001", natives: new[] { "NewNative" });

            Assert.False(ObjectTargetFieldsCommand.HasObjectData("TFT-v1.35.0.00001", root));

            var doc = BlankMap.Create();
            var result = ObjectTargetFieldsCommand.Execute(
                doc, ObjectKind.Unit, "hfoo", "TFT-v1.35.0.00001", root);

            Assert.False(result.HasObjectData);
            Assert.Empty(result.Fields);
            Assert.Contains("no object data", result.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public void Field_reads_type_bounds_and_blank_policy_from_the_target_schema()
    {
        var root = NewRoot();
        try
        {
            GamedataFixtures.Save(root, "TFT-v1.30.0.00001", slk: "units/unitmetadata.slk",
                fields: new[] { GamedataFixtures.Field("ugol", "int", "1", "300", true) });

            var hint = ObjectTargetFieldsCommand.Field("TFT-v1.30.0.00001", ObjectKind.Unit, "ugol", root);

            Assert.NotNull(hint);
            Assert.Equal("int", hint!.Type);
            Assert.Equal("1", hint.MinValue);
            Assert.Equal("300", hint.MaxValue);
            Assert.True(hint.CanBeEmpty);
            Assert.Contains("int", hint.Hint);
            Assert.Contains("1..300", hint.Hint);
            Assert.Contains("blank allowed", hint.Hint);

            // A code the build does not define is null, never a fabricated default.
            Assert.Null(ObjectTargetFieldsCommand.Field(
                "TFT-v1.30.0.00001", ObjectKind.Unit, "zzzz", root));
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public void Fields_lists_the_maps_object_fields_annotated_for_the_build()
    {
        var root = NewRoot();
        try
        {
            GamedataFixtures.Save(root, "TFT-v1.30.0.00001", slk: "units/unitmetadata.slk",
                fields: new[] { GamedataFixtures.Field("ugol", "int", "1", "300", true) });

            var doc = BlankMap.Create();
            var made = ObjectNewCommand.Execute(doc, ObjectKind.Unit, "hfoo");
            Assert.True(made.Ok, made.Message);
            Assert.True(ObjectSetCommand.Execute(doc, ObjectKind.Unit, made.NewRawcode!, "ugol", "123").Ok);

            var result = ObjectTargetFieldsCommand.Execute(
                doc, ObjectKind.Unit, made.NewRawcode!, "TFT-v1.30.0.00001", root);

            Assert.True(result.HasObjectData);
            Assert.Equal("", result.Message);
            var field = result.Fields.Single(f => f.Code == "ugol");
            Assert.True(field.Known);
            Assert.Equal("int", field.Type);
            Assert.Equal("1", field.MinValue);
        }
        finally { Directory.Delete(root, true); }
    }
}
