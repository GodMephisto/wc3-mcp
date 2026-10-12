// src/Wc3.Commands/LeakListCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>The ranked static leak list plus the periodic drivers, and the hot targets a harness can
/// name. A thin presentation over ScriptLeaksCommand, so the static analysis lives in one place.</summary>
public sealed record LeakListResult(
    bool Ok,
    string Message,
    int Leaks,
    int Hot,
    int Repeat,
    int Once,
    IReadOnlyList<LeakTarget> Targets,
    IReadOnlyList<ScriptLeak> Findings,
    IReadOnlyList<ScriptPeriodic> Periodic,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// The static half of a leak investigation, presented for the runtime half to join against. It wraps
/// ScriptLeaksCommand without adding detection, and exposes the hot targets by function for the
/// harness to drive and the join to attribute a slope to.
/// </summary>
public static class LeakListCommand
{
    public static LeakListResult Execute(MapDocument doc) => From(ScriptLeaksCommand.Execute(doc));

    public static LeakListResult From(ScriptLeaksResult r) => new(
        r.Ok,
        r.Message,
        r.Leaks.Count,
        r.Leaks.Count(l => l.Heat == "hot"),
        r.Leaks.Count(l => l.Heat == "repeat"),
        r.Leaks.Count(l => l.Heat == "once"),
        LeakWorkflow.Targets(r),
        r.Leaks,
        r.Periodic,
        r.Diagnostics);
}
