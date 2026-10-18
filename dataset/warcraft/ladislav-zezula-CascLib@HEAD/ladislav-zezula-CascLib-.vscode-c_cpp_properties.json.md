---
type: vendor-doc
source_repo: "https://github.com/ladislav-zezula/CascLib"
source_path: ".vscode/c_cpp_properties.json"
ref: HEAD
---

{
    "configurations": [
        {
            "name": "Linux",
            "compilerPathInCppPropertiesJson": "/usr/bin/clang",
            "includePath": [
                "${workspaceFolder}/**"
            ],
            "defines": [],
            "compileCommands": "${workspaceFolder}/build/compile_commands.json",
            "cStandard": "c17",
            "cppStandard": "c++14",
            "intelliSenseMode": "linux-clang-x64",
            "mergeConfigurations": false,
            "compilerPath": "/usr/bin/clang",
            "browse": {
                "path": [
                    "${workspaceFolder}"
                ],
                "limitSymbolsToIncludedHeaders": true
            },
            "configurationProvider": "ms-vscode.cmake-tools"
        }
    ],
    "version": 4
}