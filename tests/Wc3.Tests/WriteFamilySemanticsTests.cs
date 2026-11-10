// tests/Wc3.Tests/WriteFamilySemanticsTests.cs
// TST-14. Each write family names the test that proves the edit and the test that proves the data
// the edit never named is still there. A map that loads again is not evidence of either, since the
// two defects this suite was written for both produced maps that loaded, one with no UVs and one
// with a wrong trigger subtree.
//
// The table is the claim, and the surface is the measure. A family that gains a writing tool
// without a row here, or a row that names a test no longer in the assembly, fails.
using System.Reflection;
using Wc3.Mcp;

namespace Wc3.Tests;

public class WriteFamilySemanticsTests
{
    /// <summary>
    /// TST-14. One row per family that holds a writing tool. Each entry names the tests that carry
    /// the two clauses, the intended change and the preservation of the data the edit did not
    /// address. Two names are used where the change and the preservation are proved by different
    /// tests, which is how the script family is covered.
    /// </summary>
    private static readonly IReadOnlyDictionary<string, string[]> Semantic =
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["map"] = new[]
            {
                "Set_flag_round_trips_and_leaves_every_other_field_unchanged",
            },
            ["objects"] = new[]
            {
                "Object_set_changes_the_named_field_and_leaves_the_sibling_field_alone",
            },
            ["placed"] = new[]
            {
                "Placed_unit_set_moves_the_target_and_leaves_the_other_instance_in_place",
            },
            ["terrain"] = new[]
            {
                "Terrain_corner_set_moves_the_edited_corner_and_leaves_its_neighbour_alone",
            },
            ["scripts"] = new[]
            {
                "A_known_rename_is_applied_when_the_target_declares_the_new_name",
                "A_name_inside_a_string_literal_is_not_rewritten",
            },
            ["triggers"] = new[]
            {
                "Trigger_rename_changes_the_target_and_leaves_a_sibling_name_and_body_intact",
            },
            ["assets"] = new[]
            {
                "Strings_set_changes_one_entry_and_leaves_the_other_verbatim",
            },
            ["porting"] = new[]
            {
                "Ports_the_script_closure_with_rename_rawcode_rewrite_and_init_hook",
                "Port_preserves_non_utf8_bytes_in_the_target_script",
            },
            ["repair"] = new[]
            {
                "RepairGeneratedHeroes_RewritesOwnersScriptSlotsAndMapInfo",
            },
            ["leaks"] = new[]
            {
                "Leak_harness_jass_names_the_target_and_is_valid_syntax",
                "Leak_harness_for_an_unknown_target_refuses_and_names_the_available_ones",
            },
            ["product"] = new[]
            {
                "A_fresh_name_publishes_all_five_files_and_leaves_no_staging_folder",
                "An_existing_target_is_refused_and_its_bytes_are_unchanged",
            },
        };

    [Fact]
    public void Every_write_family_names_the_semantic_test_that_proves_it()
    {
        string[] writeFamilies = McpToolCatalog.Describe()
            .Where(t => t.ReadOnly != true && t.Family is not null)
            .Select(t => t.Family!)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(f => f, StringComparer.Ordinal)
            .ToArray();

        Assert.NotEmpty(writeFamilies);
        Assert.Equal(writeFamilies, Semantic.Keys.OrderBy(f => f, StringComparer.Ordinal).ToArray());

        var methods = typeof(WriteFamilySemanticsTests).Assembly
            .GetTypes()
            .SelectMany(t => t.GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Instance | BindingFlags.Static))
            .Select(m => m.Name)
            .ToHashSet(StringComparer.Ordinal);

        var problems = new List<string>();
        foreach (var (family, tests) in Semantic.OrderBy(p => p.Key, StringComparer.Ordinal))
        {
            if (tests.Length == 0) problems.Add(family + " names no test");
            foreach (string test in tests)
            {
                if (!methods.Contains(test))
                    problems.Add(family + " names " + test + ", which this assembly does not carry");
            }
        }

        Assert.True(problems.Count == 0, string.Join("\n", problems));
    }
}
