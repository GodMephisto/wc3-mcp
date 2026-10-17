---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/compressonator/applications/_plugins/cimage/tga/CMakeLists.txt"
ref: HEAD
---

cmake_minimum_required(VERSION 3.10)

add_library(Image_TGA STATIC "")

target_sources(Image_TGA
    PRIVATE
    ${PROJECT_SOURCE_DIR}/applications/_plugins/common/tc_pluginapi.h
    ${PROJECT_SOURCE_DIR}/applications/_plugins/common/tc_plugininternal.h
    ${PROJECT_SOURCE_DIR}/applications/_plugins/common/tc_plugininternal.cpp
    ${PROJECT_SOURCE_DIR}/applications/_plugins/common/utilfuncs.h
    ${PROJECT_SOURCE_DIR}/applications/_plugins/common/utilfuncs.cpp
    ./tga.cpp
    ./tga.h
    )

target_include_directories(Image_TGA
     PRIVATE
     ${PROJECT_SOURCE_DIR}/cmp_compressonatorlib
     ${PROJECT_SOURCE_DIR}/cmp_framework/common/half
     ${PROJECT_SOURCE_DIR}/applications/_plugins/common
     ${PROJECT_SOURCE_DIR}/external/stb
     )

if (UNIX)
    target_compile_definitions(Image_TGA PRIVATE _LINUX)
endif()

set_target_properties(Image_TGA PROPERTIES FOLDER ${PROJECT_FOLDER_SDK_PLUGIN_STATIC})
