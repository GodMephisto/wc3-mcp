// tests/Wc3.Tests/McpToolSmokeTests.Doctor.cs
// The doctor tools run an external program. The hermetic tests point WC3_DOCTOR at a missing exe,
// so each tool must refuse and say how to install it, in milliseconds and on any machine. The
// tests that run the real doctor take about 18 s and are opt-in under Category=Doctor.
using Wc3.Commands;

namespace Wc3.Tests;

/// <summary>Tests that change process-wide state (environment variables, Console.Out, the
/// encoding registry) and so must never run beside another test.</summary>
[CollectionDefinition("Serial", DisableParallelization = true)]
public sealed class SerialCollection { }

[Collection("Serial")]
public sealed class McpSmokeDoctorTests : McpSmokeBase
{
    public McpSmokeDoctorTests(SmokeFixture f) : base(f) { }

    private async Task Refuses(string tool, Dictionary<string, object?> args)
    {
        var saved = Environment.GetEnvironmentVariable("WC3_DOCTOR");
        Environment.SetEnvironmentVariable("WC3_DOCTOR", Path.Combine(_f.Dir, "no-such-doctor.exe"));
        try { Assert.Contains("WC3_DOCTOR", await CallError(tool, args)); }
        finally { Environment.SetEnvironmentVariable("WC3_DOCTOR", saved); }
    }

    [Fact, Covers("doctor_check")]
    public Task Doctor_check_names_the_install_when_missing() => Refuses("doctor_check", Args(("map", Fixture)));

    [Fact, Covers("doctor_inspect")]
    public Task Doctor_inspect_names_the_install_when_missing() =>
        Refuses("doctor_inspect", Args(("map", Fixture), ("report", "files")));

    [Fact, Covers("doctor_compare")]
    public Task Doctor_compare_names_the_install_when_missing() =>
        Refuses("doctor_compare", Args(("map", Fixture), ("other", Fixture)));

    [Fact, Covers("doctor_fix")]
    public async Task Doctor_fix_names_the_install_and_writes_nothing_when_missing()
    {
        var before = Sha(Fixture);
        var outPath = NewOut("doctor_fix");
        await Refuses("doctor_fix", Args(("map", Fixture), ("out_path", outPath)));
        Assert.Equal(before, Sha(Fixture));
        Assert.False(File.Exists(outPath));
    }
}

/// <summary>The same four tools against the real doctor, which must be installed. Run with
/// <c>--filter Category=Doctor</c>.</summary>
[Trait("Category", "Doctor")]
public sealed class McpSmokeDoctorLiveTests : McpSmokeBase
{
    public McpSmokeDoctorLiveTests(SmokeFixture f) : base(f) { }

    private async Task Answers(string tool, Dictionary<string, object?> args)
    {
        MapDoctorCommand.Locate();
        var r = await Call(tool, args);
        Assert.Equal(0, Prop(r, "exitCode").GetInt32());
    }

    [Fact]
    public Task Doctor_check_answers() => Answers("doctor_check", Args(("map", Fixture)));

    [Fact]
    public Task Doctor_inspect_answers() => Answers("doctor_inspect", Args(("map", Fixture), ("report", "files")));

    [Fact]
    public Task Doctor_compare_answers() => Answers("doctor_compare", Args(("map", Fixture), ("other", Fixture)));

    [Fact]
    public async Task Doctor_fix_never_writes_the_input()
    {
        // A blank map has nothing to repair, so the doctor answers "nothing" and no copy is written.
        var before = Sha(Fixture);
        var outPath = NewOut("doctor_fix");
        await Answers("doctor_fix", Args(("map", Fixture), ("out_path", outPath)));
        Assert.Equal(before, Sha(Fixture));
        Assert.False(File.Exists(outPath));
    }
}
