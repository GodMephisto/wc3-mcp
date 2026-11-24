namespace Wc3.Tests;

using Wc3.GameData;

/// <summary>
/// Whether this machine can open Warcraft III's own data. Two things have to be there, an
/// install holding the CASC archives and a native CascLib beside the binaries, because the
/// managed wrapper asks for "CascLib.dll" by name on every platform.
///
/// The MacBook has the launcher and an empty _retail_ folder, no .build.info and no archives,
/// so nothing that merges base game data can run there. Those tests return instead of failing,
/// which keeps the suite honest about what the machine can actually do. The Windows run covers
/// the same code with the data present.
/// </summary>
internal static class GameDataGate
{
    public static bool Available { get; } = GameInstall.Locate() is not null && NativePresent();

    private static bool NativePresent()
    {
        string beside = Path.GetDirectoryName(typeof(GameDataGate).Assembly.Location) ?? ".";
        return File.Exists(Path.Combine(beside, "CascLib.dll"))
            || File.Exists(Path.Combine(beside, "libCascLib.dylib"));
    }
}
