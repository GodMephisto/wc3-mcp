---
type: vendor-doc
source_repo: "https://github.com/stijnherfst/HiveWE"
source_path: "overlay-ports/soil2/portfile.cmake"
ref: HEAD
---

vcpkg_check_linkage(ONLY_STATIC_LIBRARY)
vcpkg_minimum_required(VERSION 2022-11-10)

vcpkg_from_github(
    OUT_SOURCE_PATH SOURCE_PATH
    REPO stijnherfst/SOIL2
    REF 630198d69f2ebea860e5a431b28ff836412d9d35
    SHA512 9836edaebef291d6de77b8add52802ad2bf2e5befb2b880321f878cf8bddead2eecd3737b989081ef8b6eeefc5933419a7cc5f4a30d0afbf5549401b190e5d62
    HEAD_REF master
)

vcpkg_cmake_configure(
    SOURCE_PATH "${SOURCE_PATH}"
    OPTIONS_DEBUG
        -DSOIL2_SKIP_HEADERS=ON
)

vcpkg_cmake_install()

vcpkg_copy_pdbs()
vcpkg_cmake_config_fixup()

vcpkg_install_copyright(FILE_LIST "${SOURCE_PATH}/LICENSE")