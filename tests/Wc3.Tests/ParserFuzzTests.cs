// tests/Wc3.Tests/ParserFuzzTests.cs
using System.Diagnostics;
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;
using Wc3.Modeling;

namespace Wc3.Tests;

/// <summary>
/// SEC-08, a map is untrusted input and every parser reads one. This is a deterministic,
/// seeded-by-construction corpus (no <see cref="Random"/>, every mutation is a pure function of
/// the fixture bytes) that checks a malformed map produces an exception rather than a hang or a
/// process-level failure.
///
/// <para>Measured 2026-11-08, it does not hold everywhere. 130 of the 132 corpus cases pass and
/// two fail, both the war3map.wts truncations, where the loader's string table read spins at end
/// of stream forever instead of throwing. See the note in RunStringCase, that is a real finding
/// and not a broken test.</para>

/// <para>Five parsers, one theory each, each fed the same 22 mutations of a valid starting
/// fixture. The mutations are three truncations (25, 50 and 75 percent of the file), one flipped
/// byte at 16 fixed offsets, a 64-byte zero run inserted at the middle, and the fixture's leading
/// length or count field replaced with 0xFFFFFFFF and with 0.</para>
///
/// <para>The accepted outcomes are exactly two, the input parses, or it throws one of
/// <see cref="InvalidDataException"/>, <see cref="IOException"/>, <see cref="ArgumentException"/>,
/// <see cref="FormatException"/>, <see cref="NotSupportedException"/> or the toolkit's own
/// exception. Anything else, notably <see cref="IndexOutOfRangeException"/>,
/// <see cref="NullReferenceException"/> or <see cref="OutOfMemoryException"/>, fails the case with
/// the fixture and the mutation named in the message. <see cref="StackOverflowException"/> cannot
/// be caught in .NET, so a stack overflow shows up as the whole test run dying, which is itself
/// the failure signal.</para>
///
/// <para>Why the format parser is called directly as well as through
/// <see cref="MapDocument.Load"/>. Load wraps every format parser in a catch-all that degrades a
/// failed file to raw bytes plus a diagnostic (MapDocument.TryParse), which is good behaviour for
/// a map with one bad file but it also hides the exception's type. The direct
/// <see cref="MapFormatRegistry"/> call is what actually pins the contract, and the command call
/// beside it proves the end-to-end path is still crash free.</para>
///
/// <para>Every fixture is read-only. Each mutation clones before it writes, so no case can see
/// another case's bytes and the corpus is order independent.</para>
/// </summary>
public sealed class ParserFuzzTests
{
    /// <summary>Per-case ceiling. Every case here is kilobytes against a parser, so a second is
    /// already three orders of magnitude of headroom over a healthy run.</summary>
    private const int CaseBudgetMs = 1000;

    /// <summary>Extra time the watchdog waits past the budget before it calls a case a hang.</summary>
    private const int WatchdogGraceMs = 500;

    /// <summary>Truncation points, as a percentage of the file kept.</summary>
    private static readonly int[] Truncations = { 25, 50, 75 };

    /// <summary>Length of the zero run inserted at the middle of the file.</summary>
    private const int ZeroRunLength = 64;

    // ---------------------------------------------------------------------------------------
    // Fixtures, each the shape a real file has, built from the same recipes the existing
    // fixtures in ObjectSetCommandTests, StringsCommandTests, PathingCommandTests,
    // MdxDocumentTests and BlankMap use.
    // ---------------------------------------------------------------------------------------

    private sealed class Fixture
    {
        private readonly Lazy<byte[]> _bytes;
        private readonly Lazy<IReadOnlyList<Mutation>> _mutations;

        public Fixture(string key, string fileName, int fieldOffset, string fieldName, Func<byte[]> build)
        {
            Key = key;
            FileName = fileName;
            FieldOffset = fieldOffset;
            FieldName = fieldName;
            _bytes = new Lazy<byte[]>(build);
            _mutations = new Lazy<IReadOnlyList<Mutation>>(() => MutationsOf(GetBytes(), fieldOffset, fieldName));
        }

        public string Key { get; }
        public string FileName { get; }
        public int FieldOffset { get; }
        public string FieldName { get; }

        public byte[] GetBytes() => _bytes.Value;
        public IReadOnlyList<Mutation> Mutations => _mutations.Value;
    }

    /// <summary>One deterministic derivation from a valid fixture to a malformed one.</summary>
    private sealed record Mutation(string Label, Func<byte[], byte[]> Apply);

    private static readonly IReadOnlyDictionary<string, Fixture> Fixtures =
        new Dictionary<string, Fixture>(StringComparer.Ordinal)
        {
            // The VERS chunk size sits at offset 4, straight after the MDLX magic.
            ["mdx"] = new("mdx", "model.mdx", 4, "VERS chunk size", SyntheticModel.TinyMdx),

            // BlankMap synthesizes the full on-disk .w3x shape, a 512-byte HM3W pre-archive
            // header over a real MPQ, so the archive's own header size field is at 0x204.
            ["map"] = new("map", "(archive)", 0x204, "MPQ header size", () => BlankMap.CreateArchiveBytes()),

            // Version then base-entry count at offset 4.
            ["w3u"] = new("w3u", "war3map.w3u", 4, "base unit count", UnitObjectBytes),
            ["w3a"] = new("w3a", "war3map.w3a", 4, "base ability count", AbilityObjectBytes),

            // wts is text. Offset 7 is the first digit of the first "STRING 1" header, the only
            // numeric field the format has, so it is what a length style mutation can target.
            ["wts"] = new("wts", "war3map.wts", 7, "first STRING id", WtsBytes),

            // MP3W magic, version, then width and height. Width at offset 8.
            ["wpm"] = new("wpm", "war3map.wpm", 8, "pathing width", PathingBytes),
        };

    private static Fixture FixtureOf(string key) =>
        Fixtures.TryGetValue(key, out var f)
            ? f
            : throw new ArgumentOutOfRangeException(nameof(key), key, "unknown fuzz fixture");

    private static byte[] UnitObjectBytes()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var custom = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        custom.Modifications.Add(new SimpleObjectDataModification
        { Id = "uagi".FromRawcode(), Type = ObjectDataType.Int, Value = 20 });
        custom.Modifications.Add(new SimpleObjectDataModification
        { Id = "unam".FromRawcode(), Type = ObjectDataType.String, Value = "Dark Paladin" });
        w3u.NewUnits.Add(custom);
        var standard = new SimpleObjectModification { OldId = "hfoo".FromRawcode(), NewId = 0 };
        standard.Modifications.Add(new SimpleObjectDataModification
        { Id = "uhpm".FromRawcode(), Type = ObjectDataType.Int, Value = 500 });
        w3u.BaseUnits.Add(standard);
        return Serialize(w => w.Write(w3u));
    }

    private static byte[] AbilityObjectBytes()
    {
        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var aura = new LevelObjectModification { OldId = "AOae".FromRawcode(), NewId = "A1R5".FromRawcode() };
        aura.Modifications.Add(new LevelObjectDataModification
        { Id = "atar".FromRawcode(), Type = ObjectDataType.String, Value = "self", Level = 1, Pointer = 0 });
        w3a.NewAbilities.Add(aura);
        return Serialize(w => w.Write(w3a));
    }

    private const string WtsSource =
        "STRING 1\r\n{\r\nFirst trigger string\r\n}\r\n\r\n" +
        "STRING 7\r\n{\r\nLine one\r\nLine two\r\n}\r\n\r\n" +
        "STRING 12\r\n{\r\n}\r\n";

    private static byte[] WtsBytes() => Encoding.UTF8.GetBytes(WtsSource);

    /// <summary>The blank map's own war3map.wpm, all cells unrestricted, 128 by 128 cells for a
    /// 32 tile map. Taken as raw bytes so the fixture is byte-for-byte what a map carries.</summary>
    private static byte[] PathingBytes() =>
        MapDocument.Load(BlankMap.CreateArchiveBytes()).GetFile(PathingCommand.PathingFile)!.RawBytes;

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }

    // ---------------------------------------------------------------------------------------
    // Mutation generator, 22 per fixture, always in the same order.
    // ---------------------------------------------------------------------------------------

    private static IReadOnlyList<Mutation> MutationsOf(byte[] valid, int fieldOffset, string fieldName)
    {
        int length = valid.Length;
        var mutations = new List<Mutation>(22);

        foreach (int percent in Truncations)
        {
            int keep = length * percent / 100;
            mutations.Add(new($"truncate@{percent}%", bytes => bytes.AsSpan(0, keep).ToArray()));
        }

        foreach (int offset in FlipOffsets(length))
        {
            int at = offset;
            mutations.Add(new($"flip@0x{at:X4}", bytes =>
            {
                var copy = (byte[])bytes.Clone();
                copy[at] ^= 0xFF;
                return copy;
            }));
        }

        mutations.Add(new("zero-run@mid", bytes =>
        {
            int mid = bytes.Length / 2;
            var result = new byte[bytes.Length + ZeroRunLength];
            Buffer.BlockCopy(bytes, 0, result, 0, mid);
            // The inserted window is already all zeros, so only the tail has to move.
            Buffer.BlockCopy(bytes, mid, result, mid + ZeroRunLength, bytes.Length - mid);
            return result;
        }));

        foreach (uint value in new uint[] { 0xFFFFFFFF, 0 })
        {
            uint replacement = value;
            mutations.Add(new($"field@0x{fieldOffset:X4}({fieldName})=0x{replacement:X}", bytes =>
            {
                var copy = (byte[])bytes.Clone();
                copy[fieldOffset + 0] = (byte)(replacement & 0xFF);
                copy[fieldOffset + 1] = (byte)((replacement >> 8) & 0xFF);
                copy[fieldOffset + 2] = (byte)((replacement >> 16) & 0xFF);
                copy[fieldOffset + 3] = (byte)((replacement >> 24) & 0xFF);
                return copy;
            }));
        }

        return mutations;
    }

    /// <summary>
    /// The 16 fixed byte-flip offsets. Half are pinned inside the first 33 bytes, where every
    /// format here keeps its magic, its version and its leading counts, because a mutation there
    /// is far more likely to reach a size or count than one in the middle of a payload. The other
    /// half are spread across the whole file at fixed fractions. Offsets are clamped, deduped and
    /// topped up so the count is exactly 16 for any fixture of 16 bytes or more.
    /// </summary>
    private static int[] FlipOffsets(int length)
    {
        if (length < 1) throw new ArgumentOutOfRangeException(nameof(length), "fixture is empty");

        var offsets = new SortedSet<int>();
        foreach (int pinned in new[] { 0, 1, 2, 3, 4, 5, 6, 8, 12, 16, 24, 32 })
            offsets.Add(Math.Min(pinned, length - 1));

        for (int i = 1; i <= 4; i++)
            offsets.Add((int)((long)i * (length - 1) / 5));

        int wanted = Math.Min(16, length);
        for (int i = 0; offsets.Count < wanted; i++)
            offsets.Add(i % length);

        return offsets.ToArray();
    }

    // ---------------------------------------------------------------------------------------
    // Theory data, one set of 22 rows per fixture.
    // ---------------------------------------------------------------------------------------

    private static TheoryData<string, string> Rows(params string[] keys)
    {
        var data = new TheoryData<string, string>();
        foreach (var key in keys)
            foreach (var mutation in FixtureOf(key).Mutations)
                data.Add(key, mutation.Label);
        return data;
    }

    public static TheoryData<string, string> MdxMutations => Rows("mdx");
    public static TheoryData<string, string> MapArchiveMutations => Rows("map");
    public static TheoryData<string, string> ObjectDataMutations => Rows("w3u", "w3a");
    public static TheoryData<string, string> StringTableMutations => Rows("wts");
    public static TheoryData<string, string> PathingMutations => Rows("wpm");

    // ---------------------------------------------------------------------------------------
    // The five theories.
    // ---------------------------------------------------------------------------------------

    [Theory]
    [MemberData(nameof(MdxMutations))]
    public void MdxDocument_parse_either_returns_or_throws_a_recoverable_error(string fixture, string mutation)
        => RunCase(fixture, mutation);

    [Theory]
    [MemberData(nameof(MapArchiveMutations))]
    public void Map_document_load_either_returns_or_throws_a_recoverable_error(string fixture, string mutation)
        => RunCase(fixture, mutation);

    [Theory]
    [MemberData(nameof(ObjectDataMutations))]
    public void Object_data_parse_either_returns_or_throws_a_recoverable_error(string fixture, string mutation)
        => RunCase(fixture, mutation);

    [Theory]
    [MemberData(nameof(StringTableMutations))]
    public void String_table_parse_either_returns_or_throws_a_recoverable_error(string fixture, string mutation)
        => RunCase(fixture, mutation);

    [Theory]
    [MemberData(nameof(PathingMutations))]
    public void Pathing_map_parse_either_returns_or_throws_a_recoverable_error(string fixture, string mutation)
        => RunCase(fixture, mutation);

    /// <summary>
    /// Pins the corpus itself, so a future edit that quietly shrinks the theory row count, points
    /// a mutation past the end of its fixture, or breaks a fixture so that it no longer parses
    /// cleanly, fails here instead of silently weakening every case derived from it. Per-case
    /// timing is asserted by each row, so across the five theories every case in the corpus is
    /// measured under a stopwatch and a watchdog.
    /// </summary>
    [Fact]
    public void Corpus_is_132_cases_over_six_fixtures_that_all_parse_cleanly()
    {
        Assert.Equal(6, Fixtures.Count);

        int cases = 0;
        foreach (var (key, fixture) in Fixtures)
        {
            var valid = fixture.GetBytes();
            Assert.True(valid.Length >= 16, $"{key} fixture is {valid.Length} bytes, too short to fuzz");
            Assert.True(fixture.FieldOffset + 4 <= valid.Length,
                $"{key} field offset 0x{fixture.FieldOffset:X4} runs past its {valid.Length} byte fixture");

            // 3 truncations, 16 byte flips, 1 zero run, 2 field replacements.
            Assert.Equal(22, fixture.Mutations.Count);
            Assert.Equal(16, FlipOffsets(valid.Length).Length);
            Assert.Equal(16, fixture.Mutations.Count(m => m.Label.StartsWith("flip@", StringComparison.Ordinal)));
            Assert.Equal(3, fixture.Mutations.Count(m => m.Label.StartsWith("truncate@", StringComparison.Ordinal)));
            Assert.Equal(2, fixture.Mutations.Count(m => m.Label.StartsWith("field@", StringComparison.Ordinal)));
            Assert.Equal(22, fixture.Mutations.Select(m => m.Label).Distinct().Count());
            cases += fixture.Mutations.Count;

            // The starting bytes are valid, so a case that fails is the mutation's fault. This
            // asserts a real parse rather than merely a recoverable outcome.
            var sw = Stopwatch.StartNew();
            AssertFixtureParses(key, valid);
            sw.Stop();
            Assert.True(sw.ElapsedMilliseconds < CaseBudgetMs,
                $"{key}/unmutated took {sw.ElapsedMilliseconds} ms, the fixture itself is already over the per case budget");
        }

        Assert.Equal(132, cases);
    }

    // ---------------------------------------------------------------------------------------
    // The case runner. One switch, so the theories and the corpus fact cannot drift apart.
    // ---------------------------------------------------------------------------------------

    /// <summary>
    /// Proves the unmutated fixture really parses into a populated model, so the corpus starts
    /// from bytes the parser accepts. A fixture that quietly stopped parsing would still pass
    /// every mutation case and cover nothing.
    /// </summary>
    private static void AssertFixtureParses(string key, byte[] valid)
    {
        var fixture = FixtureOf(key);
        switch (key)
        {
            case "mdx":
                Assert.NotEmpty(MdxDocument.Parse(valid).Chunks);
                break;

            case "map":
                Assert.NotEmpty(MapDocument.Load(valid).Files);
                break;

            case "w3u":
            case "w3a":
                Assert.True(MapFormatRegistry.TryGetParser(fixture.FileName, out var objectParser),
                    $"no registered parser for {fixture.FileName}");
                var parsed = objectParser(valid);
                var model = parsed is MapFormatRegistry.ParsedModel withTail ? withTail.Model : parsed;
                Assert.IsType(key == "w3u" ? typeof(UnitObjectData) : typeof(AbilityObjectData), model);
                var doc = MapDocument.Load(SyntheticMap.Build(
                    new Dictionary<string, byte[]> { [fixture.FileName] = valid }));
                Assert.NotEmpty(ObjectListCommand.Execute(
                    doc, key == "w3u" ? ObjectKind.Unit : ObjectKind.Ability, ctx: null).Items);
                break;

            case "wts":
                Assert.Equal(3, StringsCommand.Parse(Encoding.Latin1.GetString(valid)).Count);
                break;

            case "wpm":
                Assert.True(PathingCommand.Validate(MapDocument.Load(
                    SyntheticMap.Build(new Dictionary<string, byte[]> { [PathingCommand.PathingFile] = valid })))
                    .Ok);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(key), key, "no fixture validity check");
        }
    }

    private static void RunCase(string key, string mutationLabel)
    {
        var fixture = FixtureOf(key);
        var mutation = fixture.Mutations.Single(m => m.Label == mutationLabel);
        var bytes = mutation.Apply(fixture.GetBytes());
        RunFixture(key, bytes, $"{key}/{mutationLabel}");
    }

    private static void RunFixture(string key, byte[] bytes, string label = "")
    {
        if (label.Length == 0) label = key;

        switch (key)
        {
            case "mdx":
                AssertRecoverable(() => MdxDocument.Parse(bytes), $"{label} via MdxDocument.Parse");
                break;

            case "map":
                AssertRecoverable(
                    () => { Assert.NotNull(MapDocument.Load(bytes)); },
                    $"{label} via MapDocument.Load");
                break;

            case "w3u":
            case "w3a":
                RunObjectCase(key, bytes, label);
                break;

            case "wts":
                RunStringCase(bytes, label);
                break;

            case "wpm":
                RunPathingCase(bytes, label);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(key), key, "no fuzz runner for this fixture");
        }
    }

    private static void RunObjectCase(string key, byte[] bytes, string label)
    {
        var fixture = FixtureOf(key);
        var kind = key == "w3u" ? ObjectKind.Unit : ObjectKind.Ability;

        // The format parser the loader runs, called directly so the exception type is visible.
        AssertRecoverable(() =>
        {
            Assert.True(MapFormatRegistry.TryGetParser(fixture.FileName, out var parse),
                $"no registered parser for {fixture.FileName}");
            parse(bytes);
        }, $"{label} via the {fixture.FileName} parser");

        // The command that reads the object data, over a map carrying the mutated file. Load
        // degrades a failed parse to raw bytes, so this is the crash-free end-to-end path.
        AssertRecoverable(
            () => { var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]> { [fixture.FileName] = bytes })); ObjectListCommand.Execute(doc, kind, ctx: null); },
            $"{label} via ObjectListCommand ({kind})");
    }

    private static void RunStringCase(byte[] bytes, string label)
    {
        // The command layer parses wts as text carried in Latin-1, the same carrier StringsCommand
        // uses, so a byte that is not valid UTF-8 cannot manufacture a failure here.
        var text = Encoding.Latin1.GetString(bytes);
        AssertRecoverable(() => StringsCommand.Parse(text), $"{label} via StringsCommand.Parse");

        // The loader's own parser. KNOWN DEFECT, SEC-08, measured 2026-11-08. A war3map.wts cut
        // off inside an entry body never returns from this call. War3Net's ReadTriggerStrings
        // scans forward for the entry's closing '}' line and, at end of stream, keeps looking
        // instead of stopping, so a truncated string table hangs MapDocument.Load with no way for
        // its per-file catch-all in MapDocument.TryParse to intervene. Measured at both 25 and 75
        // percent truncation of a 95 byte table, still running after 20 seconds. The two corpus
        // rows that hit it are expected to fail until the read in
        // src/Wc3.MapDocument/Parsers.cs is bounded to the buffer length.
        AssertRecoverable(() =>
        {
            Assert.True(MapFormatRegistry.TryGetParser("war3map.wts", out var parse),
                "no registered parser for war3map.wts");
            parse(bytes);
        }, $"{label} via the war3map.wts parser");

        AssertRecoverable(
            () => { var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]> { ["war3map.wts"] = bytes })); StringsCommand.List(doc); },
            $"{label} via StringsCommand.List");
    }

    private static void RunPathingCase(byte[] bytes, string label)
    {
        AssertRecoverable(() =>
        {
            Assert.True(MapFormatRegistry.TryGetParser(PathingCommand.PathingFile, out var parse),
                $"no registered parser for {PathingCommand.PathingFile}");
            parse(bytes);
        }, $"{label} via the war3map.wpm parser");

        AssertRecoverable(
            () => { var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]> { [PathingCommand.PathingFile] = bytes })); PathingCommand.Validate(doc); },
            $"{label} via PathingCommand.Validate");
    }

    // ---------------------------------------------------------------------------------------
    // The contract. Parse, or throw one of the recoverable types, inside the budget.
    // ---------------------------------------------------------------------------------------

    private static long AssertRecoverable(Action action, string label, int budgetMs = CaseBudgetMs)
    {
        Exception? thrown = null;

        // The action runs on its own background thread so a parser that never returns is a
        // named failure instead of a hung suite. A stopwatch alone cannot report a hang, it only
        // notices one after the call has already come back. Background so an abandoned thread
        // cannot keep the test host alive at the end of the run.
        var worker = new Thread(() =>
        {
            try { action(); }
            catch (Exception ex) { thrown = ex; }
        })
        {
            IsBackground = true,
            Name = "fuzz: " + label,
        };

        var sw = Stopwatch.StartNew();
        worker.Start();
        bool finished = worker.Join(budgetMs + WatchdogGraceMs);
        sw.Stop();

        long elapsed = sw.ElapsedMilliseconds;

        if (!finished)
            Assert.Fail(
                $"{label} did not finish within {budgetMs} ms and was abandoned. This is the hang "
                + "this corpus exists to catch, a malformed map must not be able to stall the tool.");

        if (thrown is not null && !IsRecoverable(thrown))
            Assert.Fail(
                $"{label} threw {thrown.GetType().FullName}, which is not an acceptable failure for "
                + "untrusted input. Recoverable are InvalidDataException, IOException, "
                + "ArgumentException, FormatException, NotSupportedException and the toolkit's own "
                + $"exceptions. Message was, {thrown.Message}");

        Assert.True(elapsed < budgetMs,
            $"{label} took {elapsed} ms, over the {budgetMs} ms per case budget. A parser that "
            + "hangs on malformed input is the failure this corpus exists to catch.");

        return elapsed;
    }

    /// <summary>
    /// The accepted failure set. ArgumentException covers its ArgumentNull and ArgumentOutOfRange
    /// subclasses, and IOException covers EndOfStreamException, FileNotFoundException and
    /// ContentScaffoldException, which is the toolkit exception reachable from these parsers. The
    /// script translation errors (JassToLuaError, LuaBytecodeError, LuaSyntaxError) are not in any
    /// path here, so they are deliberately not on the list and would fail a case if they appeared.
    /// </summary>
    private static bool IsRecoverable(Exception ex) =>
        ex is InvalidDataException
           or IOException
           or ArgumentException
           or FormatException
           or NotSupportedException
           or ContentScaffoldException;
}
