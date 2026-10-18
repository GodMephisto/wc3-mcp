---
type: vendor-doc
source_repo: "https://github.com/qq200774491/war3map_w3r"
source_path: "build.bat"
ref: HEAD
---

@echo off
call "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvarsall.bat" x64
cd /d D:\code2\war3map_w3r
cl /std:c++20 /utf-8 /EHsc /O2 /Fe:war3map_w3r.exe src\main.cpp src\binary_reader.cpp src\w3r_parser.cpp src\lua_generator.cpp /Isrc
