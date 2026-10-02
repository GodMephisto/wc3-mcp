// src/Wc3.MapDocument/Diagnostic.cs
namespace Wc3.Model;

public enum DiagnosticSeverity { Info, Warning, Error }

public sealed record Diagnostic(DiagnosticSeverity Severity, string FileName, string Message);
