---
type: vendor-doc
source_repo: "https://github.com/Orden4/WCSharp.IO.JassDoc"
source_path: "WCSharp.IO.JassDoc/WCSharp.IO.JassDoc.csproj"
ref: HEAD
---

﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Library</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <DocumentationFile>WCSharp.IO.JassDoc.xml</DocumentationFile>
    <PackageLicenseFile>LICENSE</PackageLicenseFile>
    <PackageProjectUrl>https://github.com/Orden4/WCSharp.IO.JassDoc</PackageProjectUrl>
    <RepositoryUrl>https://github.com/Orden4/WCSharp.IO.JassDoc</RepositoryUrl>
    <PackageReleaseNotes>See the full release notes on the wiki: https://github.com/Orden4/WCSharp/wiki/Release-notes</PackageReleaseNotes>
    <Description>Part of the WCSharp line of packages aimed at providing libraries to simplify the process of mapmaking for WarCraft III in C#.
This tool is used to parse annotated jass specification files, as produced by JassDoc.</Description>
    <Authors>Orden4</Authors>
    <Company />
    <Product>WCSharp</Product>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Pidgin" Version="3.5.1" />
  </ItemGroup>

  <ItemGroup>
    <None Include="..\LICENSE">
      <Pack>True</Pack>
      <PackagePath></PackagePath>
    </None>
  </ItemGroup>

</Project>
