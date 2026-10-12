// src/Wc3.Commands/CiGate.cs
namespace Wc3.Commands;

/// <summary>
/// One gate's outcome in a CI run. Passed drives both the overall verdict and the exit code,
/// Summary is the single line a pipeline log shows, and Detail names what the gate actually
/// examined, so a green gate is visible rather than silent. Shared by every ci command so the
/// five gates print and exit the same way.
/// </summary>
public sealed record CiGateLine(
    string Name,
    bool Passed,
    string Summary,
    IReadOnlyList<string> Detail);
