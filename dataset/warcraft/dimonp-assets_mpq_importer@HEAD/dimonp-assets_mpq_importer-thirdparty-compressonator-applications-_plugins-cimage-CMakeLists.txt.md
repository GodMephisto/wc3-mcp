---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/compressonator/applications/_plugins/cimage/CMakeLists.txt"
ref: HEAD
---


add_library(Plugin_CImage INTERFACE)

add_subdirectory(dds)
add_subdirectory(exr)
add_subdirectory(tga)
add_subdirectory(binary)

set(CIMAGE_PLUGINS "")

if (OPTION_BUILD_ASTC)
    add_subdirectory(astc)
    list(APPEND CIMAGE_PLUGINS Plugin_CImage_ASTC)
endif()

if (OPTION_BUILD_KTX2)
    add_subdirectory(ktx)
    list(APPEND CIMAGE_PLUGINS Plugin_CImage_KTX)
endif()

list(APPEND CIMAGE_PLUGINS
    Plugin_CImage_DDS
    Plugin_CImage_EXR
    Plugin_CImage_TGA
    Plugin_CIMAGE_BINARY
)

target_link_libraries(Plugin_CImage INTERFACE
    ${CIMAGE_PLUGINS}
)

#set_target_properties(Plugin_CImage PROPERTIES FOLDER ${FOLDER_NAME})