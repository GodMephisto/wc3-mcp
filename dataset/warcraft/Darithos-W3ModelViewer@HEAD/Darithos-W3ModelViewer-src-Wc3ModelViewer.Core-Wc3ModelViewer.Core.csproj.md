---
type: vendor-doc
source_repo: "https://github.com/Darithos/W3ModelViewer"
source_path: "src/Wc3ModelViewer.Core/Wc3ModelViewer.Core.csproj"
ref: HEAD
---

<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <None Include="Convert\blender_m3studio_export.py" CopyToOutputDirectory="PreserveNewest" />
  </ItemGroup>

</Project>
