---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "CMakePresets.json"
ref: HEAD
---

{
    "version": 3,
    "cmakeMinimumRequired": {
        "major": 3,
        "minor": 31,
        "patch": 0
    },
    "configurePresets": [
        {
            "name": "conf-common",
            "description": "General settings that apply to all configurations",
            "hidden": true,
            "generator": "Ninja Multi-Config",
            "binaryDir": "${sourceDir}/out/build/${presetName}",
            "installDir": "${sourceDir}/out/install/${presetName}",
            "cacheVariables": {
                "CMAKE_CONFIGURATION_TYPES": "Debug;Release;RelWithDebInfo;MinSizeRel"
            }
        },
        {
            "name": "conf-windows-common",
            "description": "Windows settings for MSBuild toolchain that apply to msvc and clang",
            "hidden": true,
            "inherits": "conf-common",
            "condition": {
                "type": "equals",
                "lhs": "${hostSystemName}",
                "rhs": "Windows"
            },
            "architecture": {
                "value": "x64",
                "strategy": "external"
            },
            "toolset": {
                "value": "host=x64",
                "strategy": "external"
            },
            "cacheVariables": {
                "ENABLE_CPPCHECK_DEFAULT": "FALSE",
                "ENABLE_CLANG_TIDY_DEFAULT": "FALSE"
            }
        },
        {
            "name": "conf-unixlike-common",
            "description": "Unix-like OS settings for gcc and clang toolchains",
            "hidden": true,
            "inherits": "conf-common",
            "condition": {
                "type": "inList",
                "string": "${hostSystemName}",
                "list": [
                    "Linux",
                    "Darwin"
                ]
            },
            "vendor": {
                "microsoft.com/VisualStudioRemoteSettings/CMake/1.0": {
                    "sourceDir": "$env{HOME}/.vs/$ms{projectDirName}"
                }
            }
        },
        {
            "name": "windows-msvc",
            "displayName": "MSVC configuration",
            "description": "Target Windows with the msvc compiler",
            "inherits": "conf-windows-common",
            "cacheVariables": {
                "CMAKE_C_COMPILER": "cl",
                "CMAKE_CXX_COMPILER": "cl"
            }
        },
        {
            "name": "windows-clang",
            "hidden": true,
            "displayName": "MSVC clang configuration",
            "description": "Target Windows with the clang compiler",
            "inherits": "conf-windows-common",
            "cacheVariables": {
                "CMAKE_C_COMPILER": "clang-cl",
                "CMAKE_CXX_COMPILER": "clang-cl"
            },
            "vendor": {
                "microsoft.com/VisualStudioSettings/CMake/1.0": {
                    "intelliSenseMode": "windows-clang-x64"
                }
            }
        },
        {
            "name": "unixlike-gcc",
            "displayName": "gcc configuration",
            "description": "Target Unix-like OS with the gcc compiler",
            "inherits": "conf-unixlike-common",
            "cacheVariables": {
                "CMAKE_C_COMPILER": "gcc",
                "CMAKE_CXX_COMPILER": "g++"
            }
        },
        {
            "name": "unixlike-clang",
            "displayName": "clang configuration",
            "description": "Target Unix-like OS with the clang compiler",
            "inherits": "conf-unixlike-common",
            "cacheVariables": {
                "CMAKE_C_COMPILER": "clang",
                "CMAKE_CXX_COMPILER": "clang++"
            }
        }
    ],
    "buildPresets": [
        {
            "name": "windows-msvc-debug-build",
            "displayName": "msvc Debug build",
            "configurePreset": "windows-msvc",
            "configuration": "Debug"
        },
        {
            "name": "windows-msvc-release-build",
            "displayName": "msvc Release build",
            "configurePreset": "windows-msvc",
            "configuration": "RelWithDebInfo"
        },
        {
            "name": "unixlike-gcc-debug-build",
            "displayName": "unixlike-gcc Debug build",
            "configurePreset": "unixlike-gcc",
            "configuration": "Debug"
        },
        {
            "name": "unixlike-gcc-release-build",
            "displayName": "unixlike-gcc Release build",
            "configurePreset": "unixlike-gcc",
            "configuration": "RelWithDebInfo"
        },
        {
            "name": "unixlike-clang-debug-build",
            "displayName": "unixlike-clang Debug build",
            "configurePreset": "unixlike-clang",
            "configuration": "Debug"
        },
        {
            "name": "unixlike-clang-release-build",
            "displayName": "unixlike-clang Release build",
            "configurePreset": "unixlike-clang",
            "configuration": "RelWithDebInfo"
        }
    ]
}