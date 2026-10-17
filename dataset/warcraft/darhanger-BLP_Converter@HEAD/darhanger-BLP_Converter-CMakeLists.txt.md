---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "CMakeLists.txt"
ref: HEAD
---

cmake_minimum_required(VERSION 3.21)

project(DarBLPConverter
    VERSION 0.1.0
    DESCRIPTION "Modern Qt desktop converter for World of Warcraft BLP textures"
    LANGUAGES CXX
)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

find_package(Qt6 REQUIRED COMPONENTS Core Gui Widgets Concurrent)

set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTORCC ON)
set(CMAKE_AUTOUIC ON)

set(PROJECT_SOURCES
    src/main.cpp

    src/core/ImageData.h
    src/core/ImageData.cpp
    src/core/ConversionOptions.h
    src/core/ConversionResult.h
    src/core/Converter.h
    src/core/Converter.cpp

    src/blp/BlpHeader.h
    src/blp/BlpReader.h
    src/blp/BlpReader.cpp
    src/blp/BlpWriter.h
    src/blp/BlpWriter.cpp
    src/blp/BlpDecoder.h
    src/blp/BlpDecoder.cpp
    src/blp/BlpEncoder.h
    src/blp/BlpEncoder.cpp
    src/blp/DxtCodec.h
    src/blp/DxtCodec.cpp
    src/blp/MipmapGenerator.h
    src/blp/MipmapGenerator.cpp

    src/image/PngCodec.h
    src/image/PngCodec.cpp
    src/image/TgaCodec.h
    src/image/TgaCodec.cpp

    src/gui/MainWindow.h
    src/gui/MainWindow.cpp
    src/gui/FileListModel.h
    src/gui/FileListModel.cpp
    src/gui/PreviewWidget.h
    src/gui/PreviewWidget.cpp
    src/gui/LogWidget.h
    src/gui/LogWidget.cpp
    src/gui/Theme.h
    src/gui/Theme.cpp
    src/gui/ConversionWorker.h
    src/gui/ConversionWorker.cpp

    src/cli/CliParser.h
    src/cli/CliParser.cpp
    src/cli/CliCommands.h
    src/cli/CliCommands.cpp

    src/utils/FileUtils.h
    src/utils/FileUtils.cpp
    src/utils/Logger.h
    src/utils/Logger.cpp
)

qt_add_executable(darblp
    WIN32
    MANUAL_FINALIZATION
    ${PROJECT_SOURCES}
    resources/resources.qrc
)

target_include_directories(darblp PRIVATE src)

set_target_properties(darblp PROPERTIES
    OUTPUT_NAME DarBLPConverter
    WIN32_EXECUTABLE TRUE
)

target_link_libraries(darblp PRIVATE
    Qt6::Core
    Qt6::Gui
    Qt6::Widgets
    Qt6::Concurrent
)

qt_finalize_executable(darblp)
