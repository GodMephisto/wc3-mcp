---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "cleanup.bat"
ref: HEAD
---

@echo off
echo Killing stale Python processes...
taskkill /F /IM python.exe >nul 2>&1
taskkill /F /IM blender.exe >nul 2>&1
echo Done.
