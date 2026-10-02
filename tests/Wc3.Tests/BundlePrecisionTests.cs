// tests/Wc3.Tests/BundlePrecisionTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Dependency-crawl precision: with game metadata available, only fields whose
/// metadata TYPE is an object-reference type (abilList, unitCode, ...) may pull
/// objects into the closure — a plain-text field whose value merely LOOKS like a
/// rawcode must not. Without game data the crawl keeps its historical permissive
/// behavior. Hermetic: the metadata comes from a synthetic in-memory SLK source.
/// </summary>
public class BundlePrecisionTests
{
    /// <summary>In-memory IGameDataSource: serves only the synthetic metadata SLKs
    /// (missing data SLKs are skipped by the store builders by design).</summary>
    private sealed class FakeGameSource : IGameDataSource
    {
        private readonly Dictionary<string, byte[]> _files;
        public FakeGameSource(Dictionary<string, byte[]> files) =>
            _files = new Dictionary<string, byte[]>(files, StringComparer.OrdinalIgnoreCase);
        public byte[]? ReadFile(string name) => _files.TryGetValue(name, out var b) ? b : null;
        public void Dispose() { }
    }

    // Minimal unitmetadata.slk (SYLK cells: row 1 = headers, key column = field code):
    // uhab is an object-reference type (abilList); utub is a plain string.
    private const string UnitMetadataSlk =
        "ID;PWXL;N;E\n" +
        "C;Y1;X1;K\"ID\"\n" +
        "C;X2;K\"slk\"\n" +
        "C;X3;K\"field\"\n" +
        "C;X4;K\"displayName\"\n" +
        "C;X5;K\"type\"\n" +
        "C;Y2;X1;K\"uhab\"\n" +
        "C;X2;K\"UnitAbilities\"\n" +
        "C;X3;K\"heroAbilList\"\n" +
        "C;X4;K\"WESTRING_UHAB\"\n" +
        "C;X5;K\"abilList\"\n" +
        "C;Y3;X1;K\"utub\"\n" +
        "C;X2;K\"Profile\"\n" +
        "C;X3;K\"Ubertip\"\n" +
        "C;X4;K\"WESTRING_UTUB\"\n" +
        "C;X5;K\"string\"\n" +
        "E\n";

    // Header-only abilitymetadata.slk — the ability store just needs to build.
    private const string AbilityMetadataSlk =
        "ID;PWXL;N;E\n" +
        "C;Y1;X1;K\"ID\"\n" +
        "C;X2;K\"slk\"\n" +
        "C;X3;K\"field\"\n" +
        "C;X4;K\"displayName\"\n" +
        "C;X5;K\"type\"\n" +
        "E\n";

    /// <summary>A synthetic GameDataContext carrying just enough unit/ability field
    /// metadata for the reference-type gate; every other store is Empty.</summary>
    private static GameDataContext BuildContext()
    {
        using var src = new FakeGameSource(new Dictionary<string, byte[]>
        {
            [@"war3.w3mod:units\unitmetadata.slk"] = Encoding.UTF8.GetBytes(UnitMetadataSlk),
            [@"war3.w3mod:units\abilitymetadata.slk"] = Encoding.UTF8.GetBytes(AbilityMetadataSlk),
        });
        return new GameDataContext
        {
            Units = BaseUnitStore.Build(src),
            Abilities = BaseAbilityStore.Build(src),
            Items = ObjectDataStore.Empty,
            Destructables = ObjectDataStore.Empty,
            Doodads = ObjectDataStore.Empty,
            Buffs = ObjectDataStore.Empty,
            Upgrades = ObjectDataStore.Empty,
            Strings = WorldEditStrings.Parse(""),
            UnitNames = UnitNameTable.FromSources(src),
        };
    }

    // Custom unit U000 with a REAL ability-list reference to custom A000 (uhab,
    // type abilList) and a plain tooltip string that coincidentally equals custom
    // A001's rawcode (utub, type string). Both abilities exist in the map.
    private static MapDocument TestMap()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var unit = new SimpleObjectModification { OldId = "hfoo".FromRawcode(), NewId = "U000".FromRawcode() };
        unit.Modifications.Add(Str("uhab", "A000"));
        unit.Modifications.Add(Str("utub", "A001"));
        w3u.NewUnits.Add(unit);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        w3a.NewAbilities.Add(new LevelObjectModification { OldId = "ANcl".FromRawcode(), NewId = "A000".FromRawcode() });
        w3a.NewAbilities.Add(new LevelObjectModification { OldId = "ANcl".FromRawcode(), NewId = "A001".FromRawcode() });

        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.w3a"] = Ser(w => w.Write(w3a)),
        }));
    }

    [Fact]
    public void With_metadata_only_reference_typed_fields_pull_objects()
    {
        var doc = TestMap();
        var ctx = BuildContext();

        var bundle = BundleCommand.ResolveObject(doc, ObjectKind.Unit, "U000", ctx, Array.Empty<string>());

        // The real abilList reference resolves, with the field code on the edge (the WHY).
        Assert.Contains(bundle.Objects, o => o.Rawcode == "A000" && o.Kind == ObjectKind.Ability && o.CustomToMap);
        Assert.Contains(bundle.Edges, e => e.From == "U000" && e.To == "A000" && e.Via == "uhab");

        // The tooltip's rawcode-shaped text pulls nothing: A001 stays out entirely.
        Assert.DoesNotContain(bundle.Objects, o => o.Rawcode == "A001");
        Assert.DoesNotContain(bundle.Edges, e => e.To == "A001");
    }

    [Fact]
    public void Without_metadata_the_crawl_stays_permissive()
    {
        var doc = TestMap();

        var bundle = BundleCommand.ResolveObject(doc, ObjectKind.Unit, "U000", ctx: null, Array.Empty<string>());

        // No game data → field types are unknowable → any 4-char token remains a
        // candidate (today's behavior for the no-install path, kept on purpose).
        Assert.Contains(bundle.Objects, o => o.Rawcode == "A000");
        Assert.Contains(bundle.Objects, o => o.Rawcode == "A001");
    }

    private static SimpleObjectDataModification Str(string code, string value) =>
        new() { Id = code.FromRawcode(), Type = ObjectDataType.String, Value = value };

    private static byte[] Ser(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
