---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/JassToJsMain.csproj"
ref: HEAD
---

﻿<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProjectGuid>{E41DBF6D-590B-45B0-8DB4-8C43C53C69F0}</ProjectGuid>
    <OutputType>Exe</OutputType>
    <RootNamespace>JassToCsMain</RootNamespace>
    <AssemblyName>JassToCsMain</AssemblyName>
    <TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
    <Deterministic>true</Deterministic>
    <TargetFrameworkProfile />
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="Antlr4.Runtime.Standard, Version=4.8.0.0, Culture=neutral, PublicKeyToken=e78b2c5abd1fcb3f, processorArchitecture=MSIL">
      <HintPath>..\packages\Antlr4.Runtime.Standard.4.8.0\lib\net35\Antlr4.Runtime.Standard.dll</HintPath>
    </Reference>
    <Reference Include="Ninject, Version=3.3.4.0, Culture=neutral, PublicKeyToken=c7192dc5380945e7, processorArchitecture=MSIL">
      <HintPath>..\packages\Ninject.3.3.4\lib\net45\Ninject.dll</HintPath>
    </Reference>
    <Reference Include="System" />
    <Reference Include="System.Core" />
    <Reference Include="System.Xml.Linq" />
    <Reference Include="System.Data.DataSetExtensions" />
    <Reference Include="Microsoft.CSharp" />
    <Reference Include="System.Data" />
    <Reference Include="System.Net.Http" />
    <Reference Include="System.Xml" />
  </ItemGroup>
  <ItemGroup>
    <Compile Include="Antlr\CS\JassLexer.cs" />
    <Compile Include="Antlr\CS\JassParser.cs" />
    <Compile Include="Antlr\CS\JassParserBaseListener.cs" />
    <Compile Include="Antlr\CS\JassParserBaseVisitor.cs" />
    <Compile Include="Antlr\CS\JassParserListener.cs" />
    <Compile Include="Antlr\CS\JassParserVisitor.cs" />
    <Compile Include="Helpers\Extensions.cs" />
    <Compile Include="Helpers\FuncHelper.cs" />
    <Compile Include="Helpers\Parser.cs" />
    <Compile Include="Helpers\NumberConvertHelper.cs" />
    <Compile Include="Helpers\Helper.cs" />
    <Compile Include="Helpers\NameGenerator.cs" />
    <Compile Include="Main\JassParserVisitor.cs" />
    <Compile Include="Main\JassNativeParserVisitor.cs" />
    <Compile Include="Main\JassVisitor.cs" />
    <Compile Include="Main\Program.cs" />
    <Compile Include="Properties\AssemblyInfo.cs" />
    <Compile Include="Helpers\TimeTracker.cs" />
  </ItemGroup>
  <ItemGroup>
    <None Include="Antlr\Config\Build_CS.cmd" />
    <None Include="Antlr\Config\Build_JS.cmd" />
    <None Include="Antlr\Config\JassLexer.g4" />
    <None Include="Antlr\Config\JassParser.g4" />
    <None Include="Antlr\CS\JassLexer.interp" />
    <None Include="Antlr\CS\JassLexer.tokens" />
    <None Include="Antlr\CS\JassParser.interp" />
    <None Include="Antlr\CS\JassParser.tokens" />
    <None Include="Antlr\JS\Jass.interp" />
    <None Include="Antlr\JS\Jass.tokens" />
    <None Include="Antlr\JS\JassLexer.interp" />
    <None Include="Antlr\JS\JassLexer.tokens" />
    <None Include="App.config" />
    <None Include="packages.config" />
    <None Include="War3Map\blizzard.j" />
    <None Include="War3Map\common.j" />
  </ItemGroup>
  <ItemGroup>
    <Content Include="Antlr\Config\antlr-4.8-complete.jar" />
    <Content Include="Antlr\JS\JassLexer.js" />
    <Content Include="Antlr\JS\JassListener.js" />
    <Content Include="Antlr\JS\JassParser.js" />
    <Content Include="Antlr\JS\JassVisitor.js" />
    <Content Include="War3Map\blizzard.js" />
    <Content Include="War3Map\common-functions.js" />
    <Content Include="War3Map\common.js" />
    <Content Include="War3Map\temp.html" />
    <Content Include="War3Map\war3map.js" />
  </ItemGroup>
  <ItemGroup />
  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
</Project>