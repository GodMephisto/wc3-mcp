// tests/Wc3.Tests/MapSoundsProbe.cs
// TEMPORARY reflection probe: dump the War3Net MapSounds API shape so we can
// wire war3map.w3s editing (SoundCommand) against confirmed signatures.
// Namespace-agnostic: resolves types by simple name off the War3Net.Build
// assembly (already referenced by Parsers.cs via ReadMapSounds), so it never
// hard-codes a namespace that might not exist.
// Delete after the sound-definition design rests on confirmed ground truth.
using System.Linq;
using System.Reflection;
using System.Text;
using War3Net.Build.Extensions; // BinaryReaderExtensions.ReadMapSounds lives here
using Xunit;

namespace Wc3.Tests;

public class MapSoundsProbe
{
    [Fact]
    public void Dump_MapSounds_api()
    {
        var sb = new StringBuilder();

        // Anchor on the assembly that defines the reader extensions.
        var asm = typeof(BinaryReaderExtensions).Assembly;
        sb.AppendLine($"assembly: {asm.GetName().Name} {asm.GetName().Version}");

        // Confirm ReadMapSounds and grab its real return type (the sounds model).
        var readExt = asm.GetType("War3Net.Build.Extensions.BinaryReaderExtensions");
        var readMapSounds = readExt?.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "ReadMapSounds");
        sb.AppendLine($"ReadMapSounds -> {readMapSounds?.ReturnType.FullName ?? "(not found)"}");

        var soundsType = readMapSounds?.ReturnType;
        if (soundsType is not null)
        {
            sb.AppendLine($"=== {soundsType.FullName} ===");
            DumpType(soundsType, sb);
        }

        // The sound-definition element type: any type in the same namespace whose
        // simple name mentions "Sound" but isn't the container itself.
        if (soundsType is not null)
        {
            foreach (var st in asm.GetTypes()
                         .Where(x => x.Namespace == soundsType.Namespace
                                     && x.Name.Contains("Sound") && x != soundsType))
            {
                sb.AppendLine($"=== {st.FullName} ===");
                DumpType(st, sb);
            }

            sb.AppendLine($"=== all types in {soundsType.Namespace} ===");
            foreach (var at in asm.GetTypes().Where(x => x.Namespace == soundsType.Namespace))
                sb.AppendLine($"  {at.Name} (enum={at.IsEnum})");
        }

        // Writer extension methods that take a MapSounds/Sound parameter. The writers
        // are overloads all named "Write" (e.g. w.Write(mapEnvironment)), so match on
        // the PARAMETER type, not the method name.
        sb.AppendLine("=== BinaryWriterExtensions writers taking a MapSounds/Sound ===");
        var writeExt = asm.GetType("War3Net.Build.Extensions.BinaryWriterExtensions");
        foreach (var m in writeExt?.GetMethods(BindingFlags.Public | BindingFlags.Static) ?? System.Array.Empty<MethodInfo>())
            if (m.GetParameters().Any(p => p.ParameterType.Namespace == soundsType?.Namespace))
                sb.AppendLine($"  {m.Name}({string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name))}) -> {m.ReturnType.Name}");

        // Also check for an instance WriteTo on MapSounds/Sound.
        sb.AppendLine("=== MapSounds/Sound instance methods (WriteTo etc.) ===");
        foreach (var mt in new[] { soundsType, asm.GetType("War3Net.Build.Audio.Sound") })
            foreach (var m in mt?.GetMethods(BindingFlags.Public | BindingFlags.Instance) ?? System.Array.Empty<MethodInfo>())
                if (m.Name.Contains("Write") || m.Name.Contains("Serialize"))
                    sb.AppendLine($"  {mt!.Name}.{m.Name}({string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name))})");

        System.IO.File.WriteAllText(
            System.IO.Path.Combine(System.IO.Path.GetTempPath(), "mapsounds-probe.txt"),
            sb.ToString());
    }

    private static void DumpType(System.Type t, StringBuilder sb)
    {
        foreach (var c in t.GetConstructors())
            sb.AppendLine($"  .ctor({string.Join(", ", c.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})");
        foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            sb.AppendLine($"  prop {p.PropertyType.Name} {p.Name} {(p.CanRead ? "get" : "")}{(p.CanWrite ? "/set" : "")}");
        foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.Instance))
            sb.AppendLine($"  field {f.FieldType.Name} {f.Name}");
    }
}
