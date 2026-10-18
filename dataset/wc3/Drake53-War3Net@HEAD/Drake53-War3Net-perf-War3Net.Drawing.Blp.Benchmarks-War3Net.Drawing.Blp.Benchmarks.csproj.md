---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "perf/War3Net.Drawing.Blp.Benchmarks/War3Net.Drawing.Blp.Benchmarks.csproj"
ref: HEAD
---

﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <EnableWindowsTargeting>true</EnableWindowsTargeting>
    <TargetFrameworks>net$(NETCoreAppMaximumVersion);net$(NETCoreAppMaximumVersion)-windows</TargetFrameworks>
    <OutputType>Exe</OutputType>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\War3Net.Drawing.Blp\War3Net.Drawing.Blp.csproj" />
  </ItemGroup>

</Project>
