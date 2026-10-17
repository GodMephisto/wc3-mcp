---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "CMakeLists.txt"
ref: HEAD
---

cmake_minimum_required(VERSION 3.16)

project(blp_viewer LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# vendored turbojpeg-static.lib 仅有 x64 版本
if (NOT CMAKE_SIZEOF_VOID_P EQUAL 8)
    message(FATAL_ERROR "blp_viewer requires a 64-bit toolchain (turbojpeg-static.lib is x64-only).")
endif()

# --- turbojpeg (vendored static lib, used by built-in BLP codec) ---
set(TURBOJPEG_DIR ${CMAKE_CURRENT_SOURCE_DIR}/third_party/turbojpeg)
set(TURBOJPEG_LIB ${TURBOJPEG_DIR}/lib/turbojpeg-static.lib)

# --- ImGui sources ---
set(IMGUI_DIR ${CMAKE_CURRENT_SOURCE_DIR}/third_party/imgui)
set(IMGUI_SOURCES
    ${IMGUI_DIR}/imgui.cpp
    ${IMGUI_DIR}/imgui_draw.cpp
    ${IMGUI_DIR}/imgui_tables.cpp
    ${IMGUI_DIR}/imgui_widgets.cpp
    ${IMGUI_DIR}/imgui_impl_win32.cpp
    ${IMGUI_DIR}/imgui_impl_dx11.cpp
)

# --- Application sources ---
set(BLP_VIEWER_SOURCES
    src/main.cpp
    src/app.cpp
    src/blp/blp_codec.cpp
    src/ui/style.cpp
    src/ui/image_view.cpp
    src/ui/ui_main.cpp
    src/ui/ui_left_panel.cpp
    src/ui/ui_right_panel.cpp
    src/ui/ui_compose_tab.cpp
    src/ui/batch_convert.cpp
    src/ui/preview_manager.cpp
    src/ui/thumbnail_grid.cpp
    src/core/utils.cpp
    src/core/settings.cpp
    src/core/dds_codec.cpp
    src/core/image_io.cpp
    src/core/image_ops.cpp
    src/core/image_texture.cpp
    src/core/thumbnail_cache.cpp
    src/core/blp_api.cpp
    src/core/file_dialogs.cpp
    src/core/win_integration.cpp
    src/blp_viewer.rc
)

add_executable(blp_viewer WIN32
    ${BLP_VIEWER_SOURCES}
    ${IMGUI_SOURCES}
)

target_include_directories(blp_viewer PRIVATE
    ${CMAKE_CURRENT_SOURCE_DIR}/src
    ${CMAKE_CURRENT_SOURCE_DIR}/src/ui
    ${CMAKE_CURRENT_SOURCE_DIR}/src/core
    ${CMAKE_CURRENT_SOURCE_DIR}/third_party/stb
    ${CMAKE_CURRENT_SOURCE_DIR}/third_party/imgui
    ${TURBOJPEG_DIR}/include
)

target_compile_definitions(blp_viewer PRIVATE
    WIN32_LEAN_AND_MEAN
    NOMINMAX
    UNICODE
    _UNICODE
)

if (MSVC)
    target_compile_options(blp_viewer PRIVATE /utf-8)
endif()

target_link_libraries(blp_viewer PRIVATE
    ${TURBOJPEG_LIB}
    d3d11
    d3dcompiler
    dxgi
    dwmapi
    ole32
    comdlg32
    shell32
    shlwapi
    gdi32
)

# --- blp_thumbnail shell extension ---
add_library(blp_thumbnail SHARED
    shell_ext/blp_thumbnail.cpp
    shell_ext/blp_thumbnail.def
    src/blp/blp_codec.cpp
    src/core/dds_codec.cpp
)

target_include_directories(blp_thumbnail PRIVATE
    ${CMAKE_CURRENT_SOURCE_DIR}/src
    ${CMAKE_CURRENT_SOURCE_DIR}/third_party/stb
    ${TURBOJPEG_DIR}/include
)

if (MSVC)
    target_compile_options(blp_thumbnail PRIVATE /utf-8)
endif()

target_link_libraries(blp_thumbnail PRIVATE
    ${TURBOJPEG_LIB}
    ole32
    shell32
    shlwapi
    gdi32
)

set_target_properties(blp_thumbnail PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/$<CONFIG>
    LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/$<CONFIG>
)

# --- blp_codec self test (console) ---
option(BLP_BUILD_TESTS "Build blp_codec self test" ON)
if (BLP_BUILD_TESTS)
    add_executable(blp_codec_selftest
        tests/blp_codec_selftest.cpp
        src/blp/blp_codec.cpp
    )
    target_include_directories(blp_codec_selftest PRIVATE
        ${CMAKE_CURRENT_SOURCE_DIR}/src
        ${CMAKE_CURRENT_SOURCE_DIR}/third_party/stb
        ${TURBOJPEG_DIR}/include
    )
    if (MSVC)
        target_compile_options(blp_codec_selftest PRIVATE /utf-8)
    endif()
    target_link_libraries(blp_codec_selftest PRIVATE ${TURBOJPEG_LIB})

    add_executable(dds_codec_selftest
        tests/dds_codec_selftest.cpp
        src/core/dds_codec.cpp
    )
    target_include_directories(dds_codec_selftest PRIVATE
        ${CMAKE_CURRENT_SOURCE_DIR}/src
    )
    if (MSVC)
        target_compile_options(dds_codec_selftest PRIVATE /utf-8)
    endif()
endif()
