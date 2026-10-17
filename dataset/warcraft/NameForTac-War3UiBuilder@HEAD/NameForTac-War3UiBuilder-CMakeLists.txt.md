---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "CMakeLists.txt"
ref: HEAD
---

cmake_minimum_required(VERSION 3.16)
project(War3UiBuilder LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTORCC ON)

find_package(Qt6 REQUIRED COMPONENTS Core Gui Widgets)

add_executable(War3UiBuilder
    main.cpp
    mainwindow.cpp
    canvas/uiscene.cpp
    canvas/uiview.cpp
    elements/uielement.cpp
    elements/uielementdata.cpp
    panels/treepanel.cpp
    panels/propertypanel.cpp
    panels/texturepanel.cpp
    panels/searchreplacedialog.cpp
    managers/projectmanager.cpp
    managers/undomanager.cpp
    export/inigenerator.cpp
    export/fdfgenerator.cpp
    3rdparty/stb_image.cpp
    resources/resources.qrc
)

target_include_directories(War3UiBuilder PRIVATE 3rdparty)
target_link_libraries(War3UiBuilder PRIVATE Qt6::Core Qt6::Gui Qt6::Widgets)

# ── Unit tests ──
option(BUILD_TESTS "Build unit tests" ON)
if(BUILD_TESTS)
    enable_testing()

    find_package(Qt6 REQUIRED COMPONENTS Test)
    add_executable(War3UiBuilder_test
        tests/test_coord.cpp
        elements/uielementdata.cpp
        export/inigenerator.cpp
        export/fdfgenerator.cpp
        managers/undomanager.cpp
    )
    target_include_directories(War3UiBuilder_test PRIVATE
        ${CMAKE_SOURCE_DIR}
        ${CMAKE_SOURCE_DIR}/elements
        ${CMAKE_SOURCE_DIR}/export
        ${CMAKE_SOURCE_DIR}/managers
    )
    target_link_libraries(War3UiBuilder_test PRIVATE
        Qt6::Core Qt6::Test
    )
    add_test(NAME unit_test COMMAND War3UiBuilder_test)
endif()
