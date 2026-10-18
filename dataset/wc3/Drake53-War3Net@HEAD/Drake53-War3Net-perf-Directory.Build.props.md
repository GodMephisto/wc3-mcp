---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "perf/Directory.Build.props"
ref: HEAD
---

<Project>
  <ImportGroup>
    <Import Project="../Directory.Build.props" />
  </ImportGroup>

  <PropertyGroup>
    <IsPackable>false</IsPackable>
    <WarningLevel>0</WarningLevel>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="BenchmarkDotNet" />
  </ItemGroup>
</Project>