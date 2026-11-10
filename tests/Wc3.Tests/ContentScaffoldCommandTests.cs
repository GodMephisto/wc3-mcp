// tests/Wc3.Tests/ContentScaffoldCommandTests.cs
// Hermetic tests for ContentScaffoldCommand. A package is generated into a temp folder, then every
// manifest role is resolved and each file is read back by its real parser, the MDX chunk list, the
// TGA header, the WAV header. No game install is needed.
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Wc3.Commands;
using Wc3.Modeling;

namespace Wc3.Tests;

public sealed class ContentScaffoldCommandTests : IDisposable
{
    private const string Name = "alpha";

    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-content-scaffold", Guid.NewGuid().ToString("N"));

    public ContentScaffoldCommandTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    [Fact]
    public void Every_manifest_role_resolves_to_an_existing_file()
    {
        var result = Generate();

        // The manifest is real JSON, and the roles object is what a consumer reads.
        string text = File.ReadAllText(result.ManifestPath);
        using var json = JsonDocument.Parse(text);
        Assert.Equal(1, json.RootElement.GetProperty("schema").GetInt32());
        Assert.Equal(Name, json.RootElement.GetProperty("name").GetString());
        Assert.True(json.RootElement.TryGetProperty("roles", out var rolesElement));
        Assert.Equal(4, rolesElement.EnumerateObject().Count());

        var roles = ReadRoles(result.ManifestPath);
        Assert.Equal(
            new[]
            {
                $"hero.{Name}.body",
                $"hero.{Name}.effect",
                $"hero.{Name}.icon",
                $"sfx.{Name}.impact",
            },
            roles.Keys.OrderBy(k => k, StringComparer.Ordinal));

        foreach (var (role, file) in roles)
        {
            Assert.False(Path.IsPathRooted(file), $"{role} must name a file relative to the package");
            string path = Path.Combine(result.Root, file);
            Assert.True(File.Exists(path), $"{role} names a missing file at {path}");
            Assert.True(new FileInfo(path).Length > 0, $"{role} names an empty file at {path}");
        }

        // Every file the command reported is on disk, including the manifest itself.
        Assert.Equal(5, result.Files.Count);
        foreach (var file in result.Files)
        {
            Assert.True(File.Exists(file.Path), $"{file.Role} was reported but is not on disk");
            Assert.True(file.Bytes > 0, $"{file.Role} carries no bytes");
        }
        Assert.Contains(result.Files, f => f.Role == "manifest" && f.Path == result.ManifestPath);
        Assert.Empty(result.Diagnostics);
    }

    [Fact]
    public void The_body_and_the_effect_parse_as_mdx()
    {
        var result = Generate();

        var body = MdxDocument.Parse(File.ReadAllBytes(Resolve(result, $"hero.{Name}.body")));
        Assert.Contains(body.Chunks, c => c.Tag == "GEOS");

        // A capsule carries more than the four vertices of a card, which pins that the body came
        // from a volumetric primitive rather than the flat fallback.
        int geosetIndex = Assert.Single(body.GeosetIndices());
        var geoset = MdxGeoset.Parse(body.Chunks[geosetIndex].Payload[4..], 800);
        var vrtx = Assert.Single(geoset.Segments.Where(s => s.Tag == "VRTX")).Payload;
        Assert.True(BinaryPrimitives.ReadInt32LittleEndian(vrtx) > 4);

        var effect = MdxDocument.Parse(File.ReadAllBytes(Resolve(result, $"hero.{Name}.effect")));
        Assert.Contains(effect.Chunks, c => c.Tag == "PRE2");
        Assert.Equal(1, effect.CountEffects().ParticleEmitters2);
    }

    [Fact]
    public void The_effect_carries_a_pre2_emitter_with_a_lifetime_and_an_emission_rate()
    {
        var result = Generate();
        var document = MdxDocument.Parse(
            File.ReadAllBytes(Resolve(result, $"hero.{Name}.effect")));

        var emitter = Assert.Single(MdxEmitterReader.Read(document));
        Assert.Equal("PRE2", emitter.Tag);
        Assert.Equal(0, emitter.NodeId);
        Assert.Equal(0, emitter.TextureId);

        // The record is a size prefixed node, then the PRE2 fixed fields in their format order,
        // speed, variation, latitude, gravity, lifetime, emission rate, so the two fields under test
        // are read from the record itself and not from what the command reported.
        var payload = Assert.Single(document.Chunks.Where(c => c.Tag == "PRE2")).Payload;
        Assert.Equal(payload.Length, BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(0, 4)));
        int nodeSize = BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(4, 4));
        Assert.True(nodeSize >= 96, $"the emitter node header shrank to {nodeSize} bytes");
        int fields = 4 + nodeSize;

        float lifetime = BinaryPrimitives.ReadSingleLittleEndian(payload.AsSpan(fields + 16, 4));
        float emissionRate = BinaryPrimitives.ReadSingleLittleEndian(payload.AsSpan(fields + 20, 4));
        Assert.Equal(0.85f, lifetime);
        Assert.Equal(24f, emissionRate);
    }

    [Fact]
    public void The_icon_is_a_64_by_64_32_bit_tga()
    {
        var result = Generate();
        byte[] icon = File.ReadAllBytes(Resolve(result, $"hero.{Name}.icon"));

        Assert.Equal(18 + 64 * 64 * 4, icon.Length);
        Assert.Equal(2, icon[2]);                                            // uncompressed true colour
        Assert.Equal(64, BinaryPrimitives.ReadUInt16LittleEndian(icon.AsSpan(12, 2)));
        Assert.Equal(64, BinaryPrimitives.ReadUInt16LittleEndian(icon.AsSpan(14, 2)));
        Assert.Equal(32, icon[16]);                                          // bits per pixel
        Assert.Equal(8, icon[17] & 0x0F);                                    // alpha depth
    }

    [Fact]
    public void The_impact_is_a_16_bit_pcm_wav_with_the_declared_sample_count()
    {
        var result = Generate();
        byte[] wav = File.ReadAllBytes(Resolve(result, $"sfx.{Name}.impact"));

        Assert.Equal("RIFF", Encoding.ASCII.GetString(wav, 0, 4));
        Assert.Equal("WAVE", Encoding.ASCII.GetString(wav, 8, 4));
        Assert.Equal("fmt ", Encoding.ASCII.GetString(wav, 12, 4));
        Assert.Equal(16, BinaryPrimitives.ReadInt32LittleEndian(wav.AsSpan(16, 4)));
        Assert.Equal(1, BinaryPrimitives.ReadInt16LittleEndian(wav.AsSpan(20, 2)));   // PCM
        Assert.Equal(1, BinaryPrimitives.ReadInt16LittleEndian(wav.AsSpan(22, 2)));   // mono
        Assert.Equal(44100, BinaryPrimitives.ReadInt32LittleEndian(wav.AsSpan(24, 4)));
        Assert.Equal(16, BinaryPrimitives.ReadInt16LittleEndian(wav.AsSpan(34, 2)));

        Assert.Equal("data", Encoding.ASCII.GetString(wav, 36, 4));
        int dataBytes = BinaryPrimitives.ReadInt32LittleEndian(wav.AsSpan(40, 4));
        Assert.Equal(36 + dataBytes, BinaryPrimitives.ReadInt32LittleEndian(wav.AsSpan(4, 4)));
        Assert.Equal(44 + dataBytes, wav.Length);

        int blockAlign = BinaryPrimitives.ReadInt16LittleEndian(wav.AsSpan(32, 2));
        Assert.Equal(2, blockAlign);
        Assert.Equal(7938, dataBytes / blockAlign); // 44100 Hz for 0.18 s
    }

    [Fact]
    public void A_role_can_be_rebound_to_a_different_file_without_touching_the_others()
    {
        var result = Generate();
        var before = ReadRoles(result.ManifestPath);
        string originalBody = Path.Combine(result.Root, before[$"hero.{Name}.body"]);
        string alternative = Path.Combine(result.Root, "alt_body.mdx");
        File.Copy(originalBody, alternative);

        var updated = ContentScaffoldCommand.RebindRole(
            result.ManifestPath, $"hero.{Name}.body", "alt_body.mdx");
        Assert.Equal("alt_body.mdx", updated.Roles[$"hero.{Name}.body"]);

        var after = ReadRoles(result.ManifestPath);
        Assert.Equal(before.Count, after.Count);
        foreach (var (role, file) in before)
        {
            if (role == $"hero.{Name}.body") continue;
            Assert.Equal(file, after[role]);
        }
        Assert.Equal("alt_body.mdx", after[$"hero.{Name}.body"]);

        // Both files are still on disk. A rebind changes a binding, it never moves or deletes art.
        Assert.True(File.Exists(originalBody));
        Assert.True(File.Exists(alternative));

        // An unknown role is refused rather than silently adding a key nobody reads.
        Assert.Throws<ArgumentException>(() => ContentScaffoldCommand.RebindRole(
            result.ManifestPath, $"hero.{Name}.missing", "alt_body.mdx"));
        var unchanged = ReadRoles(result.ManifestPath);
        Assert.Equal(after.Count, unchanged.Count);
        foreach (var (role, file) in after)
            Assert.Equal(file, unchanged[role]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void A_package_with_an_empty_name_is_refused(string name)
    {
        string folder = Path.Combine(_dir, "refused");
        Assert.Throws<ArgumentException>(() => ContentScaffoldCommand.Execute(name, folder));
        Assert.False(Directory.Exists(folder), "a refused name must not create a package folder");
    }

    [Fact]
    public void A_fresh_name_publishes_all_five_files_and_leaves_no_staging_folder()
    {
        var result = Generate();

        Assert.Equal(5, result.Files.Count);
        foreach (var file in result.Files)
        {
            Assert.True(File.Exists(file.Path), $"{file.Role} was reported but is not on disk");
            Assert.True(file.Bytes > 0, $"{file.Role} carries no bytes");
        }

        // The staging folder lives beside the target, so the target is the only entry the publish
        // is allowed to leave in the parent.
        Assert.Empty(StagingEntries(_dir));
        AssertParentHoldsOnly(_dir, result.Root);
    }

    [Fact]
    public void An_existing_target_is_refused_and_its_bytes_are_unchanged()
    {
        var first = Generate();
        var before = Snapshot(first.Root);

        var refusal = Assert.Throws<IOException>(
            () => ContentScaffoldCommand.Execute("beta", first.Root));

        Assert.Contains("overwrite", refusal.Message, StringComparison.Ordinal);
        AssertSnapshotUnchanged(first.Root, before);
        Assert.Empty(StagingEntries(_dir));

        // The manifest still binds alpha, so the refusal did not half replace the package either.
        var roles = ReadRoles(first.ManifestPath);
        Assert.All(roles.Keys, role => Assert.Contains(Name, role, StringComparison.Ordinal));
        Assert.Equal(Name, ContentScaffoldCommand.ReadManifest(first.ManifestPath).Name);
    }

    [Fact]
    public void Overwrite_replaces_the_target_and_the_result_lists_the_new_files()
    {
        var first = Generate();
        string stale = Path.Combine(first.Root, "stale.txt");
        File.WriteAllText(stale, "an older package");
        string oldIcon = Path.Combine(first.Root, $"{Name}_icon.tga");
        Assert.True(File.Exists(oldIcon));

        var replaced = ContentScaffoldCommand.Execute("beta", first.Root, overwrite: true);

        Assert.Equal(first.Root, replaced.Root);
        Assert.Equal("beta", replaced.Name);
        Assert.Equal(5, replaced.Files.Count);
        foreach (var file in replaced.Files)
        {
            Assert.StartsWith(
                replaced.Root + Path.DirectorySeparatorChar, file.Path, StringComparison.Ordinal);
            Assert.True(File.Exists(file.Path), $"{file.Role} was reported but is not on disk");
            Assert.True(file.Bytes > 0, $"{file.Role} carries no bytes");
        }
        Assert.Contains(
            replaced.Files, f => f.Role == "manifest" && f.Path == replaced.ManifestPath);

        // A replace is a replace, not a merge. The previous art and a file nobody asked about are
        // both gone, so nothing from the old package can be bound by accident.
        Assert.False(File.Exists(oldIcon), "the replaced package must not survive inside the target");
        Assert.False(File.Exists(stale), "an unrelated file in the old target must not survive");
        Assert.True(File.Exists(Path.Combine(replaced.Root, "beta_icon.tga")));

        var roles = ReadRoles(replaced.ManifestPath);
        Assert.All(roles.Keys, role => Assert.Contains("beta", role, StringComparison.Ordinal));
        Assert.Equal("beta", ContentScaffoldCommand.ReadManifest(replaced.ManifestPath).Name);

        Assert.Empty(StagingEntries(_dir));
        AssertParentHoldsOnly(_dir, replaced.Root);
    }

    [Fact]
    public void The_staging_prefix_appears_nowhere_under_the_parent_after_a_publish()
    {
        var result = Generate();

        // The published contract is this exact infix, so the check below is on the documented name
        // and not on whatever spelling the implementation happens to use.
        Assert.Equal(".saving-", ContentScaffoldCommand.StagingInfix);

        // Recursive, so a staging folder left beside the target or one nested inside it are both
        // caught. The prefix never survives a successful publish.
        Assert.Empty(StagingEntries(_dir));
        Assert.DoesNotContain(
            Directory.EnumerateFileSystemEntries(_dir, "*", SearchOption.AllDirectories),
            entry => entry.Contains(ContentScaffoldCommand.StagingInfix, StringComparison.Ordinal));
        Assert.Equal(5, Directory.EnumerateFiles(result.Root).Count());
    }

    [Fact]
    public void A_failure_while_staging_leaves_no_staging_folder_and_no_target()
    {
        // A file where the target's parent should be makes the staging folder impossible to create,
        // so the publish fails after it has entered the staging path.
        string blocker = Path.Combine(_dir, "blocker");
        File.WriteAllText(blocker, "not a folder");
        string target = Path.Combine(blocker, "pkg");

        var failure = Assert.Throws<ContentScaffoldException>(
            () => ContentScaffoldCommand.Execute("alpha!", target));

        Assert.Contains("not published", failure.Message, StringComparison.Ordinal);
        // The diagnostics gathered before the failure come back with it, so a caller reports them
        // and is never handed a half written package.
        Assert.Contains(
            failure.Diagnostics, d => d.Contains("reduced to 'alpha'", StringComparison.Ordinal));
        Assert.False(Directory.Exists(target), "a failed publish must not leave a target folder");
        Assert.Empty(StagingEntries(_dir));
        Assert.Equal("not a folder", File.ReadAllText(blocker));
        AssertParentHoldsOnly(_dir, blocker);
    }

    [Fact]
    public void A_publish_that_cannot_swap_removes_the_finished_staging_folder_and_keeps_the_target()
    {
        // Renaming a directory is refused by Windows while a file inside it is open without delete
        // sharing, so this fails at the swap, after all five files are staged, which is the state
        // the cleanup must handle.
        if (!OperatingSystem.IsWindows())
            return;

        var first = Generate();
        string held = Path.Combine(first.Root, "held.bin");
        File.WriteAllBytes(held, new byte[16]);
        var before = Snapshot(first.Root);
        using (var handle = new FileStream(held, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            Assert.Throws<ContentScaffoldException>(
                () => ContentScaffoldCommand.Execute("beta", first.Root, overwrite: true));
            Assert.Empty(StagingEntries(_dir));
        }

        // The existing package is exactly as it was and nothing staged survived beside it.
        AssertSnapshotUnchanged(first.Root, before);
        AssertParentHoldsOnly(_dir, first.Root);
    }

    [Fact]
    public void An_output_root_that_is_an_existing_file_is_refused_and_left_alone()
    {
        string target = Path.Combine(_dir, "taken");
        File.WriteAllText(target, "not a package");

        var refusal = Assert.Throws<IOException>(
            () => ContentScaffoldCommand.Execute(Name, target));

        Assert.Contains("is a file", refusal.Message, StringComparison.Ordinal);
        Assert.Equal("not a package", File.ReadAllText(target));
        Assert.Empty(StagingEntries(_dir));
        AssertParentHoldsOnly(_dir, target);
    }

    [Fact]
    public void A_name_that_sanitises_to_nothing_publishes_under_the_content_fallback()
    {
        // Pinned because it explains why that name is not a failure trigger. A name with no usable
        // character falls back to 'content' instead of refusing the package.
        string folder = Path.Combine(_dir, "fallback");
        var result = ContentScaffoldCommand.Execute("###", folder);

        Assert.Equal("content", result.Name);
        Assert.Contains(result.Diagnostics, d => d.Contains("reduced to 'content'", StringComparison.Ordinal));
        Assert.Equal(5, result.Files.Count);
        Assert.Empty(StagingEntries(_dir));
        AssertParentHoldsOnly(_dir, result.Root);
    }

    private ContentScaffoldResult Generate()
    {
        string folder = Path.Combine(_dir, Name);
        var result = ContentScaffoldCommand.Execute(Name, folder);
        Assert.Equal(Path.GetFullPath(folder), result.Root);
        Assert.Equal(Name, result.Name);
        return result;
    }

    /// <summary>The path a manifest role names, read from the manifest the package wrote.</summary>
    private static string Resolve(ContentScaffoldResult result, string role)
    {
        var manifest = ContentScaffoldCommand.ReadManifest(result.ManifestPath);
        Assert.True(manifest.Roles.TryGetValue(role, out string? file), $"role {role} is missing");
        return Path.Combine(result.Root, file!);
    }

    /// <summary>The roles object read straight out of the JSON, so a rebind is checked on disk and
    /// not only through the reader the command itself provides.</summary>
    private static IReadOnlyDictionary<string, string> ReadRoles(string manifestPath)
    {
        using var json = JsonDocument.Parse(File.ReadAllText(manifestPath));
        var roles = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var role in json.RootElement.GetProperty("roles").EnumerateObject())
            roles[role.Name] = role.Value.GetString()
                ?? throw new InvalidDataException($"role {role.Name} names no file");
        return roles;
    }

    /// <summary>Every entry under the parent, recursively, whose path carries the staging infix. A
    /// publish that finished leaves this empty, which is what pins the cleanup.</summary>
    private static IReadOnlyList<string> StagingEntries(string parent) =>
        Directory.Exists(parent)
            ? Directory.EnumerateFileSystemEntries(parent, "*", SearchOption.AllDirectories)
                .Where(p => p.Contains(ContentScaffoldCommand.StagingInfix, StringComparison.Ordinal))
                .ToArray()
            : Array.Empty<string>();

    /// <summary>The parent holds exactly the one entry the publish was asked to produce, so neither a
    /// staging folder nor a parked replacement survives beside it.</summary>
    private static void AssertParentHoldsOnly(string parent, string expected)
    {
        var entries = Directory.EnumerateFileSystemEntries(parent)
            .Select(Path.GetFileName)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();
        Assert.Equal(new[] { Path.GetFileName(expected) }, entries);
    }

    /// <summary>Every file in the package by relative path, hashed, so an untouched package is
    /// compared byte for byte rather than by file count.</summary>
    private static IReadOnlyDictionary<string, string> Snapshot(string folder)
    {
        var snapshot = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (string path in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
            snapshot[Path.GetRelativePath(folder, path)] =
                Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
        return snapshot;
    }

    private static void AssertSnapshotUnchanged(
        string folder, IReadOnlyDictionary<string, string> before)
    {
        var after = Snapshot(folder);
        Assert.Equal(
            before.Keys.OrderBy(k => k, StringComparer.Ordinal),
            after.Keys.OrderBy(k => k, StringComparer.Ordinal));
        foreach (var (path, hash) in before)
            Assert.Equal(hash, after[path]);
    }
}
