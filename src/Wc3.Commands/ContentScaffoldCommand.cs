// src/Wc3.Commands/ContentScaffoldCommand.cs
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Wc3.Modeling;

namespace Wc3.Commands;

/// <summary>One generated file and the role it fills, so a package can bind art to gameplay
/// without naming a file inside a trigger. <see cref="Role"/> is the manifest role id for a content
/// file, and the literal <c>manifest</c> for <c>package.json</c> itself.</summary>
public sealed record ContentScaffoldFile(string Role, string Path, long Bytes);

/// <summary>What a scaffold wrote, and the roles a later substitution can rebind.</summary>
public sealed record ContentScaffoldResult(
    string Root, string Name, IReadOnlyList<ContentScaffoldFile> Files, IReadOnlyList<string> Diagnostics)
{
    /// <summary>The manifest this package wrote, the file a rebind edits.</summary>
    public string ManifestPath => System.IO.Path.Combine(Root, ContentScaffoldCommand.ManifestFileName);
}

/// <summary>
/// A publish that did not happen. A package is either wholly there or wholly absent, so this is
/// raised instead of returning a partial result, and it carries the diagnostics gathered before the
/// failure so a caller can report what the generator noted without running it again.
/// </summary>
public sealed class ContentScaffoldException : IOException
{
    public ContentScaffoldException(
        string message, IReadOnlyList<string> diagnostics, Exception? innerException = null)
        : base(message, innerException) => Diagnostics = diagnostics;

    /// <summary>What the generator noted before it failed, empty when it noted nothing.</summary>
    public IReadOnlyList<string> Diagnostics { get; }
}

/// <summary>
/// The manifest a generated package carries, the logical role to file map, so gameplay binds a role
/// and never a filename. A role is a dotted name such as <c>hero.alpha.body</c> or
/// <c>sfx.alpha.impact</c>, and its value is a file name relative to the manifest's own folder, so a
/// package can be moved or imported whole.
/// </summary>
public sealed record ContentPackageManifest(
    int Schema,
    string Name,
    IReadOnlyDictionary<string, string> Roles,
    string Notes);

/// <summary>
/// Generates a playable content package from nothing but a name. It writes a body model, a particle
/// effect, an icon, an impact sound and a role manifest. The body is a real MDX capsule from
/// <see cref="MeshPrimitiveCommand"/>, the effect is a real MDX PRE2 document from
/// <see cref="MdxEmitterWriter"/>, the icon is a 64 by 64 32 bit TGA, and the sound is a 16 bit PCM
/// WAV, so every file is loadable rather than a stub.
///
/// The manifest is the point. Gameplay refers to a role, not a filename, so replacing the
/// placeholder with final art is one binding change through <see cref="RebindRole"/> and never a
/// search through triggers. Both models name the package's own icon as their texture, so the package
/// has no dependency on art outside itself. Pass <c>texturePath</c> to override that with a base
/// game path or a map path of your own.
/// </summary>
public static class ContentScaffoldCommand
{
    /// <summary>The manifest file name, the one fixed name in a package.</summary>
    public const string ManifestFileName = "package.json";

    /// <summary>The infix a staging folder carries, <c>&lt;target&gt;.saving-&lt;8 hex&gt;</c>, so a
    /// package under construction is visible beside its target and never inside it.</summary>
    public const string StagingInfix = ".saving-";

    /// <summary>The infix a replaced package is parked under for the length of one rename.</summary>
    private const string ReplacedInfix = ".replaced-";

    /// <summary>The icon edge length in pixels, square by construction.</summary>
    public const int IconSize = 64;

    /// <summary>The icon bits per pixel, eight red, green, blue and alpha.</summary>
    public const int IconBitsPerPixel = 32;

    /// <summary>The impact sample rate in hertz.</summary>
    public const int ImpactSampleRate = 44100;

    /// <summary>The impact channel count, mono.</summary>
    public const int ImpactChannels = 1;

    /// <summary>The impact bits per sample, sixteen.</summary>
    public const int ImpactBitsPerSample = 16;

    /// <summary>The impact length in seconds, short enough that it does not read as input lag.</summary>
    public const double ImpactSeconds = 0.18;

    /// <summary>The body placeholder radius in world units.</summary>
    public const float BodyRadius = 24f;

    /// <summary>The body placeholder cylinder length, before the two hemispherical caps.</summary>
    public const float BodyLength = 56f;

    /// <summary>The effect placeholder particle lifetime in seconds.</summary>
    public const float EffectLifetime = 0.85f;

    /// <summary>The effect placeholder emission rate in particles per second.</summary>
    public const float EffectEmissionRate = 24f;

    private static readonly JsonSerializerOptions ManifestJson = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    private const string ManifestNotes =
        "Bind a role to a file. Replacing placeholder art is a binding change, not a filename search.";

    /// <summary>
    /// Writes the package into <paramref name="outDir"/>, creating the folder. The name is reduced to
    /// letters, digits, underscores and hyphens for file and role names, and the result's
    /// <see cref="ContentScaffoldResult.Diagnostics"/> says so when that changed anything.
    /// <paramref name="texturePath"/> is the map path both models name in TEXS, defaulting to the
    /// package's own icon at <c>war3mapImported\alpha_icon.tga</c> for a package named alpha.
    ///
    /// The publish is atomic in the only sense a folder can be. Every file is written into a staging
    /// folder named <c>&lt;target&gt;.saving-&lt;8 hex&gt;</c> beside the target, all five roles are
    /// verified to resolve to a non-empty file there, and only then is the staging folder renamed
    /// onto the target in one move. A failure therefore leaves either the whole package or nothing,
    /// never a partial folder that looks like a package. An existing target is refused unless
    /// <paramref name="overwrite"/> is true, in which case it is moved aside and removed only after
    /// the swap succeeds.
    /// </summary>
    /// <exception cref="IOException">The target already exists and <paramref name="overwrite"/> is
    /// false, or the target is a file rather than a folder. Nothing is written in either case.
    /// </exception>
    /// <exception cref="ContentScaffoldException">Generation failed. The staging folder is removed,
    /// an existing target is left as it was, and <see cref="ContentScaffoldException.Diagnostics"/>
    /// carries what was noted before the failure.</exception>
    public static ContentScaffoldResult Execute(
        string name, string outDir, string? texturePath = null, bool overwrite = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(outDir);

        string safe = Sanitize(name);
        string root = Path.GetFullPath(outDir);
        string folder = Path.GetFileName(root);
        string? parent = Path.GetDirectoryName(root);
        if (string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(parent))
            throw new ArgumentException(
                "outDir must name a package folder, not a drive or directory root", nameof(outDir));

        // Refusals happen before anything is written, so an existing package is never touched on a
        // path that cannot publish. The overwrite check is repeated at the swap, after the staging
        // folder is complete, so a folder that appeared meanwhile is still respected.
        if (File.Exists(root))
            throw new IOException(
                $"'{root}' is a file, not a package folder, and a publish will not replace it");
        if (Directory.Exists(root) && !overwrite)
            throw new IOException(
                $"'{root}' already exists. Pass overwrite to replace it, a package is never merged "
                + "into an existing folder");

        // Beside the target, in the same parent, so the swap is one rename on one volume rather
        // than a copy that can be interrupted halfway.
        string staging = Path.Combine(parent, folder + StagingInfix + Hex8());

        string bodyFile = $"{safe}_body.mdx";
        string effectFile = $"{safe}_effect.mdx";
        string iconFile = $"{safe}_icon.tga";
        string soundFile = $"{safe}_impact.wav";

        // The package names its own icon rather than a base game path, so importing the folder is
        // enough and no model points at art the package does not carry.
        string texture = string.IsNullOrWhiteSpace(texturePath)
            ? "war3mapImported\\" + iconFile
            : texturePath!;

        var diagnostics = new List<string>();
        if (!string.Equals(safe, name, StringComparison.Ordinal))
            diagnostics.Add($"package name '{name}' was reduced to '{safe}' for file and role names");

        long bodyBytes;
        long effectBytesLength;
        long iconBytesLength;
        long soundBytesLength;
        long manifestBytes;

        try
        {
            Directory.CreateDirectory(staging);

            // The body is a capsule, a placeholder silhouette with volume rather than a flat card,
            // and the primitive command writes the v800 MDX through the same builder every importer
            // feeds.
            var body = MeshPrimitiveCommand.Execute(
                MeshPrimitiveKind.Capsule, Path.Combine(staging, bodyFile),
                size: BodyRadius, length: BodyLength, segments: 16, texturePath: texture);
            bodyBytes = body.Bytes;
            foreach (string note in body.Diagnostics)
                diagnostics.Add("body " + note);

            // The effect is one PRE2 emitter, additive, warm at birth and fading out, so it reads as
            // a spark burst. The writer owns the chunk layout, the texture table and the material.
            var effectDocument = MdxEmitterWriter.BuildDocument(new[] { Effect(safe, texture) });
            byte[] effectBytes = effectDocument.Write();
            File.WriteAllBytes(Path.Combine(staging, effectFile), effectBytes);
            effectBytesLength = effectBytes.LongLength;

            byte[] iconBytes = Icon(safe);
            File.WriteAllBytes(Path.Combine(staging, iconFile), iconBytes);
            iconBytesLength = iconBytes.LongLength;

            byte[] soundBytes = Impact(safe);
            File.WriteAllBytes(Path.Combine(staging, soundFile), soundBytes);
            soundBytesLength = soundBytes.LongLength;

            var roles = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [$"hero.{safe}.body"] = bodyFile,
                [$"hero.{safe}.effect"] = effectFile,
                [$"hero.{safe}.icon"] = iconFile,
                [$"sfx.{safe}.impact"] = soundFile,
            };

            var manifest = new ContentPackageManifest(1, safe, roles, ManifestNotes);
            string stagedManifest = Path.Combine(staging, ManifestFileName);
            File.WriteAllText(stagedManifest, JsonSerializer.Serialize(manifest, ManifestJson) + "\n");
            manifestBytes = new FileInfo(stagedManifest).Length;

            // Nothing reaches the target until all five roles are on disk in staging and the
            // manifest's own bindings resolve beside it.
            VerifyPackage(staging, safe);
            Publish(staging, root, overwrite, diagnostics);
        }
        catch (Exception failure)
        {
            // The staging folder is the only place a partial package can live, so removing it is
            // what makes the failure total. An existing target was never opened and stays as it was.
            TryDelete(staging);
            throw new ContentScaffoldException(
                $"the package '{safe}' was not published into '{root}', no partial package was left "
                + $"behind, {failure.Message}",
                diagnostics.ToArray(), failure);
        }

        // The paths are reported from the target, after the rename, because the staging folder no
        // longer exists and a caller must not be handed a path that cannot be opened.
        var files = new List<ContentScaffoldFile>
        {
            new($"hero.{safe}.body", Path.Combine(root, bodyFile), bodyBytes),
            new($"hero.{safe}.effect", Path.Combine(root, effectFile), effectBytesLength),
            new($"hero.{safe}.icon", Path.Combine(root, iconFile), iconBytesLength),
            new($"sfx.{safe}.impact", Path.Combine(root, soundFile), soundBytesLength),
            new("manifest", Path.Combine(root, ManifestFileName), manifestBytes),
        };

        return new(root, safe, files, diagnostics);
    }

    /// <summary>
    /// Refuses to let a staging folder become a package unless every role it claims resolves to a
    /// non-empty file. The manifest is read back from the staging folder itself, so the check is on
    /// the document a consumer will parse and not on the object still held in memory.
    /// </summary>
    private static void VerifyPackage(string staging, string safe)
    {
        var expected = new (string Role, string File)[]
        {
            ($"hero.{safe}.body", $"{safe}_body.mdx"),
            ($"hero.{safe}.effect", $"{safe}_effect.mdx"),
            ($"hero.{safe}.icon", $"{safe}_icon.tga"),
            ($"sfx.{safe}.impact", $"{safe}_impact.wav"),
            ("manifest", ManifestFileName),
        };

        foreach (var (role, file) in expected)
            RequireBytes(staging, role, file);

        var manifest = ReadManifest(Path.Combine(staging, ManifestFileName));
        foreach (var (role, _) in expected)
        {
            if (role == "manifest")
                continue;
            if (!manifest.Roles.TryGetValue(role, out string? file))
                throw new InvalidDataException($"the manifest carries no role '{role}'");
            RequireBytes(staging, role, file);
        }
    }

    private static void RequireBytes(string folder, string role, string file)
    {
        string path = Path.Combine(folder, file);
        long bytes = File.Exists(path) ? new FileInfo(path).Length : -1;
        if (bytes <= 0)
            throw new InvalidDataException(
                $"role '{role}' resolves to no bytes at {path}, the package is incomplete");
    }

    /// <summary>
    /// Renames the finished staging folder onto the target, which is the publish itself. An existing
    /// target is refused unless <paramref name="overwrite"/> is true, and a replaced target is moved
    /// aside first and deleted only once the swap has succeeded, so a failed swap restores it.
    /// </summary>
    private static void Publish(string staging, string root, bool overwrite, List<string> diagnostics)
    {
        if (!Directory.Exists(root))
        {
            Directory.Move(staging, root);
            return;
        }

        // Checked again here, after the write, so a folder that appeared while the package was
        // being staged is refused rather than replaced.
        if (!overwrite)
            throw new IOException(
                $"'{root}' appeared while the package was being written. Pass overwrite to replace it");

        string aside = root + ReplacedInfix + Hex8();
        Directory.Move(root, aside);
        try
        {
            Directory.Move(staging, root);
        }
        catch (Exception swapFailure)
        {
            try
            {
                Directory.Move(aside, root);
            }
            catch (Exception restoreFailure)
            {
                throw new IOException(
                    $"the package could not be moved into '{root}' and the previous package is kept "
                    + $"at '{aside}', {restoreFailure.Message}", swapFailure);
            }
            throw;
        }

        try
        {
            Directory.Delete(aside, recursive: true);
        }
        catch (Exception leftover) when (leftover is IOException or UnauthorizedAccessException)
        {
            // The publish succeeded, so a folder that would not delete is reported and not thrown.
            diagnostics.Add(
                $"the replaced package could not be removed and is kept at {aside}, {leftover.Message}");
        }
    }

    private static void TryDelete(string folder)
    {
        try
        {
            if (Directory.Exists(folder))
                Directory.Delete(folder, recursive: true);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    /// <summary>Eight random hex characters, so two publishes into one parent never pick the same
    /// staging folder even when they are concurrent.</summary>
    private static string Hex8() =>
        Convert.ToHexString(RandomNumberGenerator.GetBytes(4)).ToLowerInvariant();

    /// <summary>The manifest at <paramref name="manifestPath"/>, parsed from its JSON.</summary>
    public static ContentPackageManifest ReadManifest(string manifestPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(manifestPath);

        string text = File.ReadAllText(manifestPath);
        var manifest = JsonSerializer.Deserialize<ContentPackageManifest>(text, ManifestJson)
            ?? throw new InvalidDataException($"the manifest at {manifestPath} carries no data");
        if (manifest.Roles is null)
            throw new InvalidDataException($"the manifest at {manifestPath} carries no roles");
        return manifest;
    }

    /// <summary>
    /// Rebinds one role to another file and rewrites the manifest, leaving every other role and the
    /// rest of the document as it was. <paramref name="fileName"/> is relative to the package folder,
    /// which is how a manifest stays portable. The updated manifest is returned, so a caller does not
    /// have to read the file back.
    /// </summary>
    public static ContentPackageManifest RebindRole(string manifestPath, string role, string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(manifestPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        if (Path.IsPathRooted(fileName))
            throw new ArgumentException(
                "a role binds a file inside the package, not an absolute path", nameof(fileName));

        var manifest = ReadManifest(manifestPath);
        if (!manifest.Roles.ContainsKey(role))
            throw new ArgumentException($"the manifest carries no role '{role}'", nameof(role));

        // Copy first, then change the one key, so the original manifest value object is never
        // mutated and every other binding is carried across untouched.
        var roles = new Dictionary<string, string>(manifest.Roles, StringComparer.Ordinal)
        {
            [role] = fileName,
        };
        var updated = manifest with { Roles = roles };
        File.WriteAllText(manifestPath, JsonSerializer.Serialize(updated, ManifestJson) + "\n");
        return updated;
    }

    private static string Sanitize(string name)
    {
        string safe = new(name.Where(c => char.IsLetterOrDigit(c) || c is '_' or '-').ToArray());
        return safe.Length == 0 ? "content" : safe;
    }

    /// <summary>One PRE2 emitter, a short burst that fades from warm to transparent as it falls.</summary>
    private static MdxEmitterParameters Effect(string name, string texturePath) => new()
    {
        Name = name + "Emitter",
        TexturePath = texturePath,
        Kind = MdxEmitterKind.ParticleEmitter2,
        ObjectId = 0,
        ParentId = -1,
        Lifetime = EffectLifetime,
        EmissionRate = EffectEmissionRate,
        Speed = 48f,
        Variation = 8f,
        Gravity = 60f,
        Latitude = 18f,
        Longitude = 0f,
        StartColour = new MdxEmitterColour(1f, 0.85f, 0.55f, 255),
        EndColour = new MdxEmitterColour(0.9f, 0.25f, 0.1f, 0),
        StartSize = 12f,
        EndSize = 4f,
        BlendMode = MdxEmitterBlend.Additive,
        Width = 8f,
        Length = 8f,
    };

    /// <summary>A 64 by 64 32 bit TGA, a radial rune whose two colours come from the name, so two
    /// packages never look identical and no asset is shared by accident.</summary>
    private static byte[] Icon(string name)
    {
        const int size = IconSize;
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(name));
        (byte r, byte g, byte b) inner = (hash[0], hash[1], hash[2]);
        (byte r, byte g, byte b) outer = ((byte)(hash[3] / 3), (byte)(hash[4] / 3), (byte)(hash[5] / 3));

        var bytes = new byte[18 + size * size * 4];
        bytes[2] = 2;                                   // uncompressed true-colour
        bytes[12] = (byte)(size & 0xFF); bytes[13] = (byte)(size >> 8);
        bytes[14] = (byte)(size & 0xFF); bytes[15] = (byte)(size >> 8);
        bytes[16] = IconBitsPerPixel; bytes[17] = 8;    // BGRA, 8 alpha bits, origin bottom left

        int p = 18;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            double dx = (x - size / 2.0) / (size / 2.0);
            double dy = (y - size / 2.0) / (size / 2.0);
            double d = Math.Sqrt(dx * dx + dy * dy);
            double k = Math.Clamp(1.0 - d, 0, 1);
            k *= k;
            bytes[p++] = (byte)(outer.b + (inner.b - outer.b) * k);
            bytes[p++] = (byte)(outer.g + (inner.g - outer.g) * k);
            bytes[p++] = (byte)(outer.r + (inner.r - outer.r) * k);
            bytes[p++] = (byte)(255 * Math.Clamp(1.4 - d, 0, 1));
        }
        return bytes;
    }

    /// <summary>A 180 ms 16 bit mono PCM WAV, a short noise burst over a low sine with an exponential
    /// decay. It is deliberately short, because an impact that starts with silence reads as lag.</summary>
    private static byte[] Impact(string name)
    {
        int count = (int)(ImpactSampleRate * ImpactSeconds);
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(name));
        double baseHz = 90 + hash[6] % 80;
        var rng = new Random(BitConverter.ToInt32(hash, 8));

        int blockAlign = ImpactChannels * (ImpactBitsPerSample / 8);
        int dataBytes = count * blockAlign;

        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write("RIFF"u8); w.Write(36 + dataBytes); w.Write("WAVE"u8);
        w.Write("fmt "u8); w.Write(16);
        w.Write((short)1); w.Write((short)ImpactChannels);          // PCM, mono
        w.Write(ImpactSampleRate); w.Write(ImpactSampleRate * blockAlign);
        w.Write((short)blockAlign); w.Write((short)ImpactBitsPerSample);
        w.Write("data"u8); w.Write(dataBytes);
        for (int i = 0; i < count; i++)
        {
            double t = (double)i / ImpactSampleRate;
            double decay = Math.Exp(-t * 26);
            double body = Math.Sin(2 * Math.PI * baseHz * t) * 0.5;
            double noise = (rng.NextDouble() * 2 - 1) * 0.5;
            double s = (body + noise) * decay * 0.8;
            w.Write((short)(Math.Clamp(s, -1, 1) * short.MaxValue));
        }
        return ms.ToArray();
    }
}
