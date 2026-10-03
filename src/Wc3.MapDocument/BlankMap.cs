// src/Wc3.MapDocument/BlankMap.cs
using System.Globalization;
using System.Reflection;
using System.Text;
using War3Net.Build;
using War3Net.Build.Common;
using War3Net.Build.Environment;
using War3Net.Build.Extensions;
using War3Net.Build.Info;
using War3Net.Common.Extensions;
using War3Net.IO.Mpq;

namespace Wc3.Model;

/// <summary>
/// Options controlling blank-map synthesis. Defaults produce a small
/// 32x32-tile Lordaeron Summer map named "Blank Map".
/// </summary>
public sealed record BlankMapOptions
{
    public string MapName { get; init; } = "Blank Map";
    public string MapAuthor { get; init; } = "wc3ctl";
    public string MapDescription { get; init; } = "Created with wc3ctl.";
    public string RecommendedPlayers { get; init; } = "1";

    /// <summary>Playable area is TileEdge x TileEdge tiles (TileEdge+1 vertices per side).</summary>
    public int TileEdge { get; init; } = 32;

    public MapInfoFormatVersion InfoVersion { get; init; } = MapInfoFormatVersion.v28;
    public MapEnvironmentFormatVersion EnvironmentVersion { get; init; } = MapEnvironmentFormatVersion.v11;

    /// <summary>Tileset code; 'L' = Lordaeron Summer.</summary>
    public char TilesetCode { get; init; } = 'L';
}

/// <summary>
/// Synthesizes a minimal, valid Warcraft III map archive from scratch.
///
/// <para>MapDocument.Load/Save operate on real MPQ archive bytes (Save rebuilds via
/// MpqArchiveBuilder over the original archive), so the cleanest way to make a blank
/// map a first-class MapDocument is to synthesize valid archive bytes here and feed
/// them straight through <see cref="MapDocument.Load(byte[])"/>. The resulting document
/// then Saves, parses and renders through every existing code path unchanged.</para>
///
/// <para>The recipe — a war3map.w3i (info) file plus a war3map.w3e (terrain) file packed
/// into an MPQ with a generated listfile — was verified empirically against the pinned
/// War3Net build by tests/Wc3.Tests/War3NetApiProbe.cs. A war3map.j carrying the
/// standard GUI-map skeleton (globals / InitCustomTriggers / main / config) rides along
/// so ScriptPorter can splice + hook ported trigger code into a blank map.</para>
///
/// <para>For external World Editor openability the synthesized bytes carry the full
/// on-disk .w3x shape: a 512-byte HM3W pre-archive header, a war3map.wpm pathing map
/// sized to the terrain (all cells unrestricted) and a small placeholder
/// war3mapMap.tga minimap, alongside the info/terrain/script files.</para>
/// </summary>
public static class BlankMap
{
    /// <summary>Synthesizes a blank map and returns it as a loaded <see cref="MapDocument"/>.</summary>
    public static MapDocument Create(BlankMapOptions? options = null)
        => MapDocument.Load(CreateArchiveBytes(options));

    /// <summary>Synthesizes the raw MPQ archive bytes for a blank map.</summary>
    public static byte[] CreateArchiveBytes(BlankMapOptions? options = null)
    {
        var o = options ?? new BlankMapOptions();
        if (o.TileEdge < 1)
            throw new ArgumentOutOfRangeException(nameof(options), "TileEdge must be >= 1.");

        // BOM-less: GetScriptFile writes the encoding preamble through a StreamWriter,
        // and a BOM at the top of war3map.j is not something WC3's JASS parser (or the
        // World Editor) ever sees from real maps. Info/env go through BinaryWriter,
        // which never emits a preamble, so this only affects the script file.
        var enc = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        var info = new MapInfo(o.InfoVersion)
        {
            MapName = o.MapName,
            MapAuthor = o.MapAuthor,
            MapDescription = o.MapDescription,
            RecommendedPlayers = o.RecommendedPlayers,
            // Explicit (it is also the enum default): GetScriptFile names the script
            // file off this — Jass → "war3map.j".
            ScriptLanguage = ScriptLanguage.Jass,
            // The terrain is TileEdge tiles a side with no unplayable border, so all of it is playable.
            // Left at 0, map_info reported a 0 x 0 map for every map this created.
            PlayableMapAreaWidth = o.TileEdge,
            PlayableMapAreaHeight = o.TileEdge,
            // The same one player and one force the script's InitCustomPlayerSlots and
            // InitCustomTeams set up, so the info file and the script agree and the
            // player and force editors have something to edit.
            Players = new List<PlayerData>
            {
                new()
                {
                    Id = 0,
                    Name = "Player 1",
                    Controller = PlayerController.User,
                    Race = PlayerRace.Human,
                    Flags = PlayerFlags.RaceSelectable,
                },
            },
            Forces = new List<ForceData>
            {
                new() { Name = "Force 1", Players = new Bitmask32(1) },
            },
        };
        EnsureSerializable(info);

        var env = BuildEnvironment(o.EnvironmentVersion, o.TileEdge, o.TilesetCode);

        var map = new Map
        {
            Info = info,
            Environment = env,
            PathingMap = BuildPathingMap(o.TileEdge),
            Script = BuildScript(o),
        };
        var files = new List<MpqFile>
        {
            map.GetInfoFile(enc)!,
            map.GetEnvironmentFile(enc)!,
            map.GetPathingMapFile(enc)!,
            map.GetScriptFile(enc)!,
            MpqFile.New(new MemoryStream(BuildMinimapTga()), MinimapFileName),
        };

        // Generate/overwrite the (listfile) so the named entries (war3map.w3i / .w3e)
        // are discoverable when the archive is re-opened by MapDocument.Load.
        var createOpts = new MpqArchiveCreateOptions
        {
            ListFileCreateMode = MpqFileCreateMode.Overwrite,
        };

        using var mpq = new MemoryStream();
        using (MpqArchive.Create(mpq, files, createOpts, leaveOpen: true)) { }

        // A real .w3x is the 512-byte HM3W block followed by the archive. The MPQ
        // format resolves internal offsets relative to wherever its own header is
        // found, so a plain prefix is safe (SyntheticMap and MapDocument.SaveToBytes
        // concatenate the same way). MapDocument.Load captures the block as
        // PreArchiveData and Save re-emits it, so it survives every round-trip.
        using var w3x = new MemoryStream();
        w3x.Write(BuildW3xHeader(info), 0, W3xHeaderSize);
        mpq.Position = 0;
        mpq.CopyTo(w3x);
        return w3x.ToArray();
    }

    private const int W3xHeaderSize = 512;

    /// <summary>
    /// Builds the 512-byte pre-archive block a real .w3x carries: "HM3W" magic, an
    /// unknown dword (zero in editor-saved maps), the null-terminated map name, the
    /// map-flags dword and the max-players dword, zero-padded out to the 512-byte
    /// boundary where the MPQ archive begins (MpqHeader.FindArchiveOffset scans on
    /// exactly that alignment). The pinned War3Net build has no writer for this block,
    /// so the documented layout is written directly. The World Editor rejects .w3x
    /// files without it, which is why a blank map must ship one.
    /// </summary>
    private static byte[] BuildW3xHeader(MapInfo info)
    {
        var block = new byte[W3xHeaderSize];
        using var w = new BinaryWriter(new MemoryStream(block));
        w.Write("HM3W"u8);
        w.Write(0u);

        // Null-terminated display name. The fixed block leaves 495 bytes for it (512
        // minus two leading dwords, the terminator and two trailing dwords). Absurdly
        // long names are truncated here only, war3map.w3i keeps the full text.
        var name = Encoding.UTF8.GetBytes(info.MapName ?? string.Empty);
        int room = W3xHeaderSize - (4 + 4 + 1 + 4 + 4);
        w.Write(name, 0, Math.Min(name.Length, room));
        w.Write((byte)0);

        w.Write((uint)info.MapFlags);
        // Max players, floored at 1: the blank map's script configures one player slot
        // even when the (empty-by-default) w3i player list carries none.
        w.Write((uint)Math.Max(info.Players?.Count ?? 0, 1));
        return block;
    }

    // The pathing grid is 4x4 cells per terrain tile (each cell covers 32x32 world units).
    private const int PathingCellsPerTile = 4;

    /// <summary>
    /// Builds the war3map.wpm pathing grid sized to the terrain: TileEdge*4 cells per
    /// side. A set <see cref="PathingType"/> bit marks that capability as blocked, so
    /// all-clear cells (the <c>default</c>) leave every cell fully walkable, flyable
    /// and buildable, the correct default for freshly synthesized flat ground.
    /// </summary>
    private static MapPathingMap BuildPathingMap(int tileEdge)
    {
        int side = tileEdge * PathingCellsPerTile;
        return new MapPathingMap(MapPathingMapFormatVersion.v0)
        {
            Width = (uint)side,
            Height = (uint)side,
            Cells = Enumerable.Repeat(default(PathingType), side * side).ToList(),
        };
    }

    private const string MinimapFileName = "war3mapMap.tga";

    /// <summary>
    /// Builds a small valid war3mapMap.tga minimap: an 18-byte uncompressed truecolor
    /// TGA header followed by solid grass-green 32-bit BGRA pixels. The World Editor
    /// regenerates the real minimap from terrain when it saves, so a plain placeholder
    /// is enough to keep the archive complete for map-preview UIs.
    /// </summary>
    private static byte[] BuildMinimapTga()
    {
        const int side = 128;
        // Grass tone matching the Lgrs tile the terrain palette leads with (BGRA order).
        const byte blue = 44, green = 118, red = 68, alpha = 255;

        var tga = new byte[18 + side * side * 4];
        tga[2] = 2;                        // image type: uncompressed truecolor
        tga[12] = (byte)(side & 0xFF);     // width, little-endian
        tga[13] = (byte)(side >> 8);
        tga[14] = (byte)(side & 0xFF);     // height, little-endian
        tga[15] = (byte)(side >> 8);
        tga[16] = 32;                      // bits per pixel
        tga[17] = 8;                       // descriptor: 8 alpha bits, bottom-left origin
        for (int i = 18; i < tga.Length; i += 4)
        {
            tga[i] = blue;
            tga[i + 1] = green;
            tga[i + 2] = red;
            tga[i + 3] = alpha;
        }
        return tga;
    }

    /// <summary>
    /// Emits the minimal standard GUI-map JASS skeleton the World Editor generates for
    /// a fresh one-player map: empty globals, an empty <c>InitCustomTriggers</c> (the
    /// exact function ScriptPorter's init hook targets — keep the name), player/team
    /// setup, <c>main</c> (camera bounds / day-night models / sound environment /
    /// InitBlizzard / InitCustomTriggers) and <c>config</c> (lobby identity, one player,
    /// one team, one start location). Deliberately conservative — just enough structure
    /// for splice + hook and for WC3 to treat the map as a sane starting point.
    /// </summary>
    private static string BuildScript(BlankMapOptions o)
    {
        // Camera bounds: the playable extent inset by the editor's customary 512-unit
        // (4-tile) margin, floored at zero so degenerate tiny maps stay valid.
        string b = Math.Max(o.TileEdge * 64f - 512f, 0f).ToString("0.0", CultureInfo.InvariantCulture);
        string name = JassString(o.MapName);
        string desc = JassString(o.MapDescription);

        return $$"""
            //===========================================================================
            //
            //  {{name}}
            //
            //  Warcraft III map script
            //  Generated by wc3ctl blank-map synthesis
            //
            //===========================================================================

            globals
            endglobals

            //***************************************************************************
            //*
            //*  Triggers
            //*
            //***************************************************************************

            //===========================================================================
            function InitCustomTriggers takes nothing returns nothing
            endfunction

            //***************************************************************************
            //*
            //*  Players
            //*
            //***************************************************************************

            function InitCustomPlayerSlots takes nothing returns nothing
                // Player 0
                call SetPlayerStartLocation( Player(0), 0 )
                call SetPlayerColor( Player(0), ConvertPlayerColor(0) )
                call SetPlayerRacePreference( Player(0), RACE_PREF_HUMAN )
                call SetPlayerRaceSelectable( Player(0), true )
                call SetPlayerController( Player(0), MAP_CONTROL_USER )
            endfunction

            function InitCustomTeams takes nothing returns nothing
                // Force: Force 1
                call SetPlayerTeam( Player(0), 0 )
            endfunction

            //***************************************************************************
            //*
            //*  Main Initialization
            //*
            //***************************************************************************

            //===========================================================================
            function main takes nothing returns nothing
                call SetCameraBounds( -{{b}} + GetCameraMargin(CAMERA_MARGIN_LEFT), -{{b}} + GetCameraMargin(CAMERA_MARGIN_BOTTOM), {{b}} - GetCameraMargin(CAMERA_MARGIN_RIGHT), {{b}} - GetCameraMargin(CAMERA_MARGIN_TOP), -{{b}} + GetCameraMargin(CAMERA_MARGIN_LEFT), {{b}} - GetCameraMargin(CAMERA_MARGIN_TOP), {{b}} - GetCameraMargin(CAMERA_MARGIN_RIGHT), -{{b}} + GetCameraMargin(CAMERA_MARGIN_BOTTOM) )
                call SetDayNightModels( "Environment\\DNC\\DNCLordaeron\\DNCLordaeronTerrain\\DNCLordaeronTerrain.mdl", "Environment\\DNC\\DNCLordaeron\\DNCLordaeronUnit\\DNCLordaeronUnit.mdl" )
                call NewSoundEnvironment( "Default" )
                call InitBlizzard(  )
                call InitCustomTriggers(  )
            endfunction

            //***************************************************************************
            //*
            //*  Map Configuration
            //*
            //***************************************************************************

            function config takes nothing returns nothing
                call SetMapName( "{{name}}" )
                call SetMapDescription( "{{desc}}" )
                call SetPlayers( 1 )
                call SetTeams( 1 )
                call SetGamePlacement( MAP_PLACEMENT_USE_MAP_SETTINGS )

                call DefineStartLocation( 0, 0.0, 0.0 )

                // Player setup
                call InitCustomPlayerSlots(  )
                call InitCustomTeams(  )
            endfunction

            """;
    }

    /// <summary>Escapes a value for a single-line JASS string literal.</summary>
    private static string JassString(string s) => s
        .Replace("\\", "\\\\")
        .Replace("\"", "\\\"")
        .Replace("\r", string.Empty)
        .Replace("\n", "\\n");

    private static MapEnvironment BuildEnvironment(MapEnvironmentFormatVersion version, int tileEdge, char tilesetCode)
    {
        // Width/Height are the map size in TILES; the terrain grid the serializer
        // writes (and the reader expects) is (Width+1) x (Height+1) corner points.
        // Setting Width=verts here would make the reader expect (verts+1)^2 points and
        // overrun the stream — the count must be tileEdge, with (tileEdge+1)^2 tiles.
        uint verts = (uint)(tileEdge + 1);
        var env = new MapEnvironment(version)
        {
            IsCustomTileset = false,
            Width = (uint)tileEdge,
            Height = (uint)tileEdge,
            Left = -tileEdge * 64f,
            Bottom = -tileEdge * 64f,
            Right = tileEdge * 64f,
            Top = tileEdge * 64f,
        };
        env.Tileset = (Tileset)(byte)tilesetCode;

        // Ground-texture palette so the map renders REAL textured ground (grass/dirt/rock),
        // not a flat gray placeholder: TerrainArtCatalog.BuildLayersForMap reads
        // env.TerrainTypes and each tile's Texture index points into it. Without this the
        // list is empty and every tile falls back to a solid gray fill — the map looks like
        // it "has no ground". The default TerrainTile.Texture is 0 → the first entry (grass).
        // Standard Lordaeron Summer ('L') ground tiles; cliff list stays empty (flat map).
        foreach (var tileId in new[] { "Lgrs", "Lgrd", "Ldrt", "Ldro", "Lrok" })
            env.TerrainTypes.Add((TerrainType)tileId.FromRawcode());

        // War3Net's TerrainTile has no public parameterless ctor surfaced for direct
        // `new`; construct via reflection (approach verified in the API probe). A
        // default-constructed tile has raw height 0, which War3Net normalizes to
        // Height = (0 - 8192) / 512 = -16, i.e. the ground would sit sixteen cliff-steps
        // BELOW the standard datum. That is invisible for a flat map (the camera just
        // frames it), but it breaks anything that reasons about ground level: a water
        // brush computing "sit N steps above the ground" would land below the water
        // format's representable floor and wrap. Seat every corner at the standard flat
        // datum (normalized Height 0, WaterHeight 0) so the blank map behaves like a real one.
        var tileType = typeof(MapEnvironment).Assembly.GetType("War3Net.Build.Environment.TerrainTile")!;
        for (int i = 0; i < verts * verts; i++)
        {
            var tile = (TerrainTile)Activator.CreateInstance(tileType)!;
            tile.Height = 0f;
            tile.WaterHeight = 0f;
            env.TerrainTiles.Add(tile);
        }

        return env;
    }

    /// <summary>
    /// War3Net's MapInfo serialization throws "Cannot serialize MapInfo, because
    /// &lt;Member&gt; is null." for each required-but-null member (camera bounds,
    /// count-prefixed lists such as players/forces/tables, etc.). A pristine MapInfo
    /// with only identity fields set leaves several such members null.
    ///
    /// Rather than hard-code the (version-dependent) member set, we drive it off the
    /// serializer itself: attempt a throwaway serialize, fill the member it names with
    /// a sane default, and retry until it serializes cleanly. Empty/default values are
    /// the correct semantics for a blank map. This reactive recipe was verified in
    /// tests/Wc3.Tests/War3NetApiProbe.cs.
    /// </summary>
    private static void EnsureSerializable(MapInfo info)
    {
        SanitizeVersions(info);

        for (int attempt = 0; attempt < 64; attempt++)
        {
            try
            {
                using var ms = new MemoryStream();
                using (var w = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
                    w.Write(info); // War3Net.Build.Extensions serializer
                return;            // serialized cleanly — all required members present
            }
            catch (InvalidOperationException ex) when (TryExtractNullMember(ex.Message, out var member))
            {
                var prop = typeof(MapInfo).GetProperty(member, BindingFlags.Public | BindingFlags.Instance);
                var value = prop is { CanWrite: true } ? MakeDefault(prop.PropertyType) : null;
                if (value is null)
                    throw new InvalidOperationException(
                        $"Blank-map synthesis could not satisfy required MapInfo member '{member}'.", ex);
                prop!.SetValue(info, value);
            }
        }

        throw new InvalidOperationException(
            "Blank-map synthesis exceeded its fill-attempt budget while building MapInfo.");
    }

    /// <summary>Parses the member name out of "...because &lt;Member&gt; is null.".</summary>
    private static bool TryExtractNullMember(string message, out string member)
    {
        member = string.Empty;
        const string head = "because ";
        const string tail = " is null";
        int i = message.IndexOf(head, StringComparison.Ordinal);
        int j = message.IndexOf(tail, StringComparison.Ordinal);
        if (i < 0 || j < 0 || j <= i) return false;
        i += head.Length;
        member = message[i..j].Trim();
        return member.Length > 0;
    }

    /// <summary>
    /// War3Net reconstructs version members via <c>new Version(major, minor, build,
    /// revision)</c>, which throws when Build/Revision are negative. A pristine MapInfo
    /// (or one whose null Version members we filled) can carry a version with the -1
    /// components System.Version uses for unspecified fields. Rewrite any such member to
    /// a fully specified, non-negative version so the round-trip parses.
    /// </summary>
    private static void SanitizeVersions(MapInfo info)
    {
        foreach (var prop in typeof(MapInfo).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.PropertyType != typeof(Version) || !prop.CanRead || !prop.CanWrite) continue;
            if (prop.GetValue(info) is Version v && (v.Build < 0 || v.Revision < 0))
                prop.SetValue(info, new Version(v.Major, v.Minor, Math.Max(v.Build, 0), Math.Max(v.Revision, 0)));
        }
    }

    /// <summary>
    /// Builds an empty/default value for a member type: "" for strings, empty arrays,
    /// default(T) for value types (unwrapping Nullable&lt;T&gt;), and for reference
    /// types a parameterless instance when available, else the shortest constructor
    /// invoked with default arguments (matches the probe's confirmed MakeDefault).
    /// </summary>
    private static object? MakeDefault(Type t)
    {
        if (t == typeof(string)) return string.Empty;
        // System.Version left with < 4 components stores Build/Revision as -1, which
        // War3Net's reader rejects (`new Version(..., build, ...)` throws). Use a fully
        // specified, non-negative version.
        if (t == typeof(Version)) return new Version(1, 0, 0, 0);

        var underlying = Nullable.GetUnderlyingType(t);
        if (underlying is not null) t = underlying;

        if (t.IsValueType) return Activator.CreateInstance(t);
        if (t.IsArray) return Array.CreateInstance(t.GetElementType()!, 0);

        if (t.GetConstructor(Type.EmptyTypes) is not null)
            return Activator.CreateInstance(t);

        var ctor = t.GetConstructors().OrderBy(c => c.GetParameters().Length).FirstOrDefault();
        if (ctor is null) return null;

        var args = ctor.GetParameters()
            .Select(p => p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType) : null)
            .ToArray();
        try { return ctor.Invoke(args); }
        catch { return null; }
    }
}
