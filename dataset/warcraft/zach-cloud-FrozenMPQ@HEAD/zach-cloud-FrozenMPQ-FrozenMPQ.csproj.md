---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/FrozenMPQ"
source_path: "FrozenMPQ.csproj"
ref: HEAD
---

<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <Nullable>annotations</Nullable>
  </PropertyGroup>

  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
    <PlatformTarget>x86</PlatformTarget>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="StormLibSharp">
      <HintPath>StormLibSharp.dll</HintPath>
    </Reference>
  </ItemGroup>

</Project>
