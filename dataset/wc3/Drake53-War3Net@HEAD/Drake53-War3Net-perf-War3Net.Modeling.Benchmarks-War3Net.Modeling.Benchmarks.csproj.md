---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "perf/War3Net.Modeling.Benchmarks/War3Net.Modeling.Benchmarks.csproj"
ref: HEAD
---

﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net$(NETCoreAppMaximumVersion)</TargetFramework>
    <OutputType>Exe</OutputType>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\War3Net.Modeling\War3Net.Modeling.csproj" />
    <ProjectReference Include="..\..\submodules\FastMDX\FastMDX\FastMDX.csproj" />
  </ItemGroup>

</Project>
