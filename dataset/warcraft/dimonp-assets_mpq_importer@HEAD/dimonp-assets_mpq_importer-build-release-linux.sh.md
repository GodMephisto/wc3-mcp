---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "build-release-linux.sh"
ref: HEAD
---

cmake -B ./out/build/unixlike-gcc -S . \
    -G "Ninja Multi-Config" \
    -DCMAKE_BUILD_TYPE=Release \
    -DENABLE_CLANG_TIDY_DEFAULT=OFF \
    -Dassets_mpq_importer_ENABLE_IPO=ON \
    -Dassets_mpq_importer_PACKAGING_MAINTAINER_MODE=ON \
    -Dassets_mpq_importer_ENABLE_COVERAGE=OFF \
    -DCMAKE_INSTALL_DO_STRIP=ON \
    -DCMAKE_TOOLCHAIN_FILE="${VCPKG_ROOT}/scripts/buildsystems/vcpkg.cmake"

cmake --build ./out/build/unixlike-gcc  --config Release

cd ./out/build/unixlike-gcc
cpack -DCPACK_STRIP_FILES=ON -G TGZ 
#-DCPACK_PACKAGE_FILE_NAME="assets_mpq_importer-linux"
