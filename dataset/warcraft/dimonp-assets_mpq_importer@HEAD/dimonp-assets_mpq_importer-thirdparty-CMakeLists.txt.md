---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/CMakeLists.txt"
ref: HEAD
---


# suppress warnings for all thirdparty libraries
if (CMAKE_CXX_COMPILER_ID MATCHES "GNU" OR CMAKE_CXX_COMPILER_ID MATCHES "Clang")
    add_compile_options(-w)
elseif (CMAKE_CXX_COMPILER_ID MATCHES "MSVC" OR CMAKE_CXX_COMPILER_FRONTEND_VARIANT MATCHES "ClangCl")
    add_compile_options(/wd4996 /WX-)
endif()

set_property(DIRECTORY PROPERTY CXX_CLANG_TIDY "")

include(thirdparty_wc3lib.cmake)
include(thirdparty_nvidia-texture-tools.cmake)
include(thirdparty_compressionator.cmake)
