---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/rsa/辅助-文件签名.bat"
ref: HEAD
---

@echo off
chcp 65001
cd %~dp0
"%~dp0..\w3x2lni\bin\w3x2lni-lua.exe" "%~dp0rsa.lua" %1
pause
