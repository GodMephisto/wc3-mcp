// tests/Wc3.Tests/AssemblyInfo.cs
// Test classes run in parallel. Anything that touches process-wide state (MapFormatRegistry,
// Console.Out, environment variables) joins the "Serial" collection, which xUnit runs alone.
