---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "deploy.bat"
ref: HEAD
---

@echo off
chcp 65001 >nul
cd /d "%~dp0"

set QT_DIR=D:\Qt\6.10.2\mingw_64

echo ^===^= [1/4] qmake ^===^=
if not exist Makefile (
    "%QT_DIR%\bin\qmake6.exe" War3UiBuilder.pro
)

echo ^===^= [2/4] build ^===^=
mingw32-make -f Makefile.Release || exit /b 1

echo ^===^= [3/4] windeployqt (bin) ^===^=
"%QT_DIR%\bin\windeployqt.exe" bin\War3UiBuilder.exe || exit /b 1

echo ^===^= [4/4] copy to deploy ^===^=
if exist deploy rmdir /s /q deploy
mkdir deploy
xcopy bin\* deploy\ /E /I /Y >nul

echo ============ OK ============
echo bin\ — self-contained (Qt DLLs deployed)
echo deploy\ — distribution copy
dir deploy
pause
