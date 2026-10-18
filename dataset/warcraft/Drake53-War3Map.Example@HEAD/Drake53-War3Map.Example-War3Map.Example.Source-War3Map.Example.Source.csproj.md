---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Map.Example"
source_path: "War3Map.Example.Source/War3Map.Example.Source.csproj"
ref: HEAD
---

﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp3.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="War3Api.Common" Version="1.31.1.2" />
    <PackageReference Include="War3Api.Blizzard" Version="1.31.1.1" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\War3Map.Example.Source.Dependency\War3Map.Example.Source.Dependency.csproj" />
  </ItemGroup>

</Project>
