---
type: vendor-doc
source_repo: "https://github.com/sumneko/JassParser"
source_path: "测试.bat"
ref: HEAD
---

@echo off
chcp 65001
cd %~dp0
"%~dp0bin\lua.exe" "%~dp0jass.lua" -ver=24 -gui %1
pause