---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "CMakePresets.json"
ref: HEAD
---

{
  "version": 6,
  "cmakeMinimumRequired": {
    "major": 3,
    "minor": 29,
    "patch": 0
  },
  "configurePresets": [
    {
      "name": "default",
      "hidden": true,
      "generator": "Visual Studio 17 2022",
      "binaryDir": "${sourceDir}/build/${presetName}",
      "toolchainFile": "$env{VCPKG_ROOT}/scripts/buildsystems/vcpkg.cmake",
      "architecture": {
        "value": "x64",
        "strategy": "external"
      },

      "vendor": {
        "microsoft.com/VisualStudioSettings/CMake/1.0": {
          "hostOS": [ "Windows" ]
        }
      }
    },
    {
      "name": "macos-ninja",
      "hidden": true,
      "generator": "Ninja",
      "binaryDir": "${sourceDir}/build/${presetName}",
      "toolchainFile": "$env{VCPKG_ROOT}/scripts/buildsystems/vcpkg.cmake",
      "cacheVariables": {
        "CMAKE_CXX_COMPILER": "$env{HOMEBREW_PREFIX}/opt/llvm/bin/clang++",
        "CMAKE_CXX_COMPILER_CLANG_SCAN_DEPS": "$env{HOMEBREW_PREFIX}/opt/llvm/bin/clang-scan-deps",
        "CMAKE_CXX_COMPILER_LAUNCHER": "${sourceDir}/tools/ccache-cxx-modules.sh",
        "CMAKE_CXX_STANDARD_LIBRARY": "libc++",
        "CMAKE_CXX_MODULE_STD": "OFF"
      },
      "condition": {
        "type": "equals",
        "lhs": "${hostSystemName}",
        "rhs": "Darwin"
      }
    },
    {
      "name": "macos-ninja-metal",
      "hidden": true,
      "inherits": "macos-ninja",
      "cacheVariables": {
        "CMAKE_OBJCXX_COMPILER": "$env{HOMEBREW_PREFIX}/opt/llvm/bin/clang++",
        "CMAKE_OBJCXX_COMPILER_LAUNCHER": "${sourceDir}/tools/ccache-cxx-modules.sh"
      }
    },
    {
      "name": "Debug",
      "inherits": "default",
      "displayName": "Debug",
      "cacheVariables": {
        "CMAKE_BUILD_TYPE": "Debug"
      }
    },
    {
      "name": "RelWithDebInfo",
      "inherits": "default",
      "displayName": "RelWithDebInfo",
      "cacheVariables": {
        "CMAKE_BUILD_TYPE": "RelWithDebInfo"
      }
    },
    {
      "name": "ReleaseTracing",
      "inherits": "default",
      "displayName": "ReleaseTracing",
      "cacheVariables": {
        "CMAKE_BUILD_TYPE": "RelWithDebInfo",
        "CMAKE_INTERPROCEDURAL_OPTIMIZATION": "ON",
        "CMAKE_VISIBILITY_PRESET": "hidden",
        "CMAKE_VISIBILITY_INLINES_HIDDEN": "ON",
        "TRACY_ENABLE": "ON",
        "TRACY_NO_EXIT": "ON"
      }
    },
    {
      "name": "Release",
      "inherits": "default",
      "displayName": "Release",
      "cacheVariables": {
        "CMAKE_BUILD_TYPE": "Release",
        "CMAKE_INTERPROCEDURAL_OPTIMIZATION": "ON",
        "CMAKE_VISIBILITY_PRESET": "hidden",
        "CMAKE_VISIBILITY_INLINES_HIDDEN": "ON"
      }
    },
    {
      "name": "macOS-Debug",
      "inherits": "macos-ninja-metal",
      "displayName": "macOS Debug (Ninja)",
      "cacheVariables": {
        "CMAKE_BUILD_TYPE": "Debug",
        "HIVEWE_BUILD_TESTS": "OFF"
      }
    },
    {
      "name": "macOS-Debug-WithTests",
      "inherits": "macOS-Debug",
      "displayName": "macOS Debug with Tests (Ninja)",
      "cacheVariables": {
        "HIVEWE_BUILD_TESTS": "ON"
      }
    },
    {
      "name": "macOS-Debug-OpenGL",
      "inherits": "macos-ninja",
      "displayName": "macOS Debug OpenGL fallback (Ninja)",
      "cacheVariables": {
        "CMAKE_BUILD_TYPE": "Debug",
        "HIVEWE_ENABLE_METAL": "OFF",
        "HIVEWE_BUILD_TESTS": "OFF"
      }
    },
    {
      "name": "macOS-Release",
      "inherits": "macos-ninja-metal",
      "displayName": "macOS Release (Ninja)",
      "cacheVariables": {
        "CMAKE_BUILD_TYPE": "Release",
        "CMAKE_INTERPROCEDURAL_OPTIMIZATION": "ON",
        "CMAKE_VISIBILITY_PRESET": "hidden",
        "CMAKE_VISIBILITY_INLINES_HIDDEN": "ON"
      }
    }
  ],
  "buildPresets": [
    {
      "name": "Debug",
      "configurePreset": "Debug",
      "configuration": "Debug"
    },
    {
      "name": "RelWithDebInfoTracing",
      "configurePreset": "ReleaseTracing",
      "configuration": "RelWithDebInfo"
    },
    {
      "name": "Release",
      "configurePreset": "Release",
      "configuration": "Release"
    },
    {
      "name": "macOS-Debug",
      "configurePreset": "macOS-Debug",
      "jobs": 1
    },
    {
      "name": "macOS-Debug-WithTests",
      "configurePreset": "macOS-Debug-WithTests",
      "jobs": 1
    },
    {
      "name": "macOS-Debug-App",
      "displayName": "macOS Debug app only (single tree)",
      "configurePreset": "macOS-Debug-WithTests",
      "targets": [
        "HiveWE"
      ],
      "jobs": 1
    },
    {
      "name": "macOS-Debug-Tests",
      "displayName": "macOS Debug test binary only (single tree)",
      "configurePreset": "macOS-Debug-WithTests",
      "targets": [
        "HiveWE_tests"
      ],
      "jobs": 1
    },
    {
      "name": "macOS-Debug-Verify",
      "displayName": "macOS Debug app and tests (single tree)",
      "configurePreset": "macOS-Debug-WithTests",
      "targets": [
        "HiveWE",
        "HiveWE_tests"
      ],
      "jobs": 1
    },
    {
      "name": "macOS-Debug-OpenGL",
      "displayName": "macOS Debug OpenGL fallback",
      "configurePreset": "macOS-Debug-OpenGL",
      "targets": [
        "HiveWE"
      ],
      "jobs": 1
    },
    {
      "name": "macOS-Release",
      "configurePreset": "macOS-Release",
      "jobs": 1
    },
    {
      "name": "macOS-Release-App",
      "displayName": "macOS Release app only",
      "configurePreset": "macOS-Release",
      "targets": [
        "HiveWE"
      ],
      "jobs": 1
    },
    {
      "name": "macOS-Release-Verify",
      "displayName": "macOS Release app and tests",
      "configurePreset": "macOS-Release",
      "targets": [
        "HiveWE",
        "HiveWE_tests"
      ],
      "jobs": 1
    }
  ],
  "testPresets": [
    {
      "name": "Debug",
      "configurePreset": "Debug",
      "configuration": "Debug",
      "output": {
        "outputOnFailure": true
      }
    },
    {
      "name": "Release",
      "configurePreset": "Release",
      "configuration": "Release",
      "output": {
        "outputOnFailure": true
      }
    },
    {
      "name": "macOS-Debug",
      "configurePreset": "macOS-Debug-WithTests",
      "output": {
        "outputOnFailure": true
      }
    },
    {
      "name": "macOS-Debug-WithTests",
      "configurePreset": "macOS-Debug-WithTests",
      "output": {
        "outputOnFailure": true
      }
    },
    {
      "name": "macOS-Release",
      "configurePreset": "macOS-Release",
      "output": {
        "outputOnFailure": true
      }
    }
  ],
  "packagePresets": [
    {
      "name": "macOS-Debug-DMG",
      "configurePreset": "macOS-Debug-WithTests",
      "generators": [
        "DragNDrop"
      ]
    },
    {
      "name": "macOS-Release-DMG",
      "configurePreset": "macOS-Release",
      "generators": [
        "DragNDrop"
      ]
    }
  ]
}
