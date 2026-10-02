// tests/Wc3.Tests/AssemblyInfo.cs
// MapFormatRegistry is process-global mutable state; tests that swap parsers
// in and out must not run concurrently with tests that parse.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
