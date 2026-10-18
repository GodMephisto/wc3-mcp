---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToJsMain.UnitTest/JassToJsMain.UnitTest.csproj"
ref: HEAD
---

﻿<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="..\packages\NUnit3TestAdapter.3.16.1\build\net35\NUnit3TestAdapter.props" Condition="Exists('..\packages\NUnit3TestAdapter.3.16.1\build\net35\NUnit3TestAdapter.props')" />
  <Import Project="..\packages\NUnit.3.12.0\build\NUnit.props" Condition="Exists('..\packages\NUnit.3.12.0\build\NUnit.props')" />
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProjectGuid>{4E956620-1566-4CA0-81DD-F7EA70F6102A}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>JassToJsMain.UnitTest</RootNamespace>
    <AssemblyName>JassToJsMain.UnitTest</AssemblyName>
    <TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <ProjectTypeGuids>{3AC096D0-A1C2-E12C-1390-A8335801FDAB};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
    <VisualStudioVersion Condition="'$(VisualStudioVersion)' == ''">15.0</VisualStudioVersion>
    <VSToolsPath Condition="'$(VSToolsPath)' == ''">$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)</VSToolsPath>
    <ReferencePath>$(ProgramFiles)\Common Files\microsoft shared\VSTT\$(VisualStudioVersion)\UITestExtensionPackages</ReferencePath>
    <IsCodedUITest>False</IsCodedUITest>
    <TestProjectType>UnitTest</TestProjectType>
    <NuGetPackageImportStamp>
    </NuGetPackageImportStamp>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="Antlr4.Runtime.Standard, Version=4.8.0.0, Culture=neutral, PublicKeyToken=e78b2c5abd1fcb3f" />
    <Reference Include="Ninject, Version=3.3.4.0, Culture=neutral, PublicKeyToken=c7192dc5380945e7, processorArchitecture=MSIL">
      <HintPath>..\packages\Ninject.3.3.4\lib\net45\Ninject.dll</HintPath>
    </Reference>
    <Reference Include="nunit.framework, Version=3.12.0.0, Culture=neutral, PublicKeyToken=2638cd05610744eb, processorArchitecture=MSIL">
      <HintPath>..\packages\NUnit.3.12.0\lib\net45\nunit.framework.dll</HintPath>
    </Reference>
    <Reference Include="System" />
    <Reference Include="System.Core" />
    <Reference Include="System.Data" />
    <Reference Include="System.Management" />
    <Reference Include="System.Xml" />
  </ItemGroup>
  <ItemGroup>
    <Content Include="Array\Test1\Expected.js" />
    <Content Include="NameReplacement\Test3\Expected.js" />
    <Content Include="NameReplacement\Test2\Expected.js" />
    <Content Include="Operations\IntegerDivision\Test2\Expected.js" />
    <Content Include="Operations\IntegerDivision\Test1\Expected.js" />
    <Content Include="Expressions\Hex\Test2\Expected.js" />
    <Content Include="NameReplacement\Test1\Expected.js" />
    <Content Include="Expressions\Hex\Test1\Expected.js" />
    <Content Include="Expressions\Test2\Expected.js" />
    <Content Include="Expressions\Test1\Expected.js" />
    <Content Include="Globals\Test2\Expected.js" />
    <Content Include="Globals\Test1\Expected.js" />
    <None Include="Array\Test1\Example.j" />
    <None Include="NameReplacement\Test3\Example.j" />
    <None Include="NameReplacement\Test2\Example.j" />
    <None Include="Operations\IntegerDivision\Test2\Example.j" />
    <None Include="Operations\IntegerDivision\Test1\Example.j" />
    <None Include="Expressions\Hex\Test2\Example.j" />
    <None Include="NameReplacement\Test1\Example.j" />
    <None Include="Expressions\Hex\Test1\Example.j" />
    <None Include="Expressions\Test2\Example.j" />
    <None Include="Expressions\Test1\Example.j" />
    <None Include="Globals\Test2\Example.j" />
    <None Include="Globals\Test1\Example.j" />
    <Compile Include="Array\ArrayTest.cs" />
    <Compile Include="Operations\IntegerDivision\IntegerDivision.cs" />
    <Compile Include="NameReplacement\NameReplacementTest.cs" />
    <Compile Include="Expressions\Hex\HexTest.cs" />
    <Compile Include="Expressions\ExpressionsTest.cs" />
    <Compile Include="TestBase.cs" />
    <Compile Include="Globals\GlobalsTest.cs" />
    <Compile Include="Properties\AssemblyInfo.cs" />
  </ItemGroup>
  <ItemGroup>
    <None Include="packages.config" />
  </ItemGroup>
  <ItemGroup />
  <ItemGroup>
    <ProjectReference Include="..\JassToCsMain\JassToJsMain.csproj">
      <Project>{E41DBF6D-590B-45B0-8DB4-8C43C53C69F0}</Project>
      <Name>JassToJsMain</Name>
    </ProjectReference>
  </ItemGroup>
  <Import Project="$(VSToolsPath)\TeamTest\Microsoft.TestTools.targets" Condition="Exists('$(VSToolsPath)\TeamTest\Microsoft.TestTools.targets')" />
  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
  <Target Name="EnsureNuGetPackageBuildImports" BeforeTargets="PrepareForBuild">
    <PropertyGroup>
      <ErrorText>This project references NuGet package(s) that are missing on this computer. Use NuGet Package Restore to download them.  For more information, see http://go.microsoft.com/fwlink/?LinkID=322105. The missing file is {0}.</ErrorText>
    </PropertyGroup>
    <Error Condition="!Exists('..\packages\NUnit.3.12.0\build\NUnit.props')" Text="$([System.String]::Format('$(ErrorText)', '..\packages\NUnit.3.12.0\build\NUnit.props'))" />
    <Error Condition="!Exists('..\packages\NUnit3TestAdapter.3.16.1\build\net35\NUnit3TestAdapter.props')" Text="$([System.String]::Format('$(ErrorText)', '..\packages\NUnit3TestAdapter.3.16.1\build\net35\NUnit3TestAdapter.props'))" />
  </Target>
</Project>