---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "build-release-windows.bat"
ref: HEAD
---

call "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"
set PATH=%PATH%;%VCToolsInstallDir%\bin\Hostx64\x64\

cmake -S . -B ./out/build/windows-msvc ^
    -G "Visual Studio 17 2022" ^
    -DCMAKE_BUILD_TYPE=Release ^
    -DENABLE_CLANG_TIDY_DEFAULT=OFF ^
    -Dassets_mpq_importer_ENABLE_IPO=ON ^
    -Dassets_mpq_importer_PACKAGING_MAINTAINER_MODE=ON ^
    -Dassets_mpq_importer_ENABLE_COVERAGE=OFF ^
    -DCMAKE_TOOLCHAIN_FILE="%VCPKG_ROOT%/scripts/buildsystems/vcpkg.cmake"

cmake --build ./out/build/windows-msvc --config Release

cd ./out/build/windows-msvc
cpack -G ZIP

