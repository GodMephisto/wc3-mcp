---
type: vendor-doc
source_repo: "https://github.com/sidsukana/QBlpPlugin"
source_path: "CMakeLists.txt"
ref: HEAD
---

cmake_minimum_required(VERSION 3.27)

project(qblp LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_DEBUG_POSTFIX d)

set(CMAKE_POSITION_INDEPENDENT_CODE ON)

find_package(Qt6 REQUIRED COMPONENTS Core Gui)

set(QBLP_SOURCE_FILES
    qblpplugin.h
    qblpplugin.cpp
    qblphandler.h
    qblphandler.cpp
)

add_subdirectory(dep EXCLUDE_FROM_ALL)

qt_add_plugin(${PROJECT_NAME} SHARED CLASS_NAME QBlpPlugin)
target_sources(${PROJECT_NAME} PRIVATE ${QBLP_SOURCE_FILES})

target_link_libraries(${PROJECT_NAME} PRIVATE
    Qt${QT_VERSION_MAJOR}::Core
    Qt${QT_VERSION_MAJOR}::Gui
    libsquish::Squish
)

install(TARGETS ${PROJECT_NAME}
    RUNTIME DESTINATION plugins/imageformats COMPONENT Plugins
    LIBRARY DESTINATION plugins/imageformats COMPONENT Plugins
)
