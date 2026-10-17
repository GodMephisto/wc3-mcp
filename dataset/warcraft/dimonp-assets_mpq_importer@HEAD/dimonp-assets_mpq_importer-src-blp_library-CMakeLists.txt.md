---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/blp_library/CMakeLists.txt"
ref: HEAD
---

find_package(fmt CONFIG REQUIRED)
find_package(spdlog CONFIG REQUIRED)

include(GenerateExportHeader)

if (WIN32)
#    find_package(dxsdk-d3dx CONFIG REQUIRED)
endif()

add_library(blp_library)
add_library(assets_mpq_importer::blp_library ALIAS blp_library)

target_sources(blp_library
    PRIVATE
      utils_blp.cpp utils_blp.hpp
      converter_png.cpp
      converter_dds_nvtt.cpp
      converter_dds_amdc.cpp
    PUBLIC
      FILE_SET HEADERS
      BASE_DIRS ${CMAKE_SOURCE_DIR}/include
      FILES
        ${CMAKE_SOURCE_DIR}/include/assets_mpq_importer/blp.hpp
)

target_link_libraries(
  blp_library PRIVATE
    assets_mpq_importer_options assets_mpq_importer_warnings
    wc3libcore
    wc3libblp
    nvtt
    CMP_Compressonator
    CMP_Framework)

target_link_system_libraries(
  blp_library
  PRIVATE
          fmt::fmt
          spdlog::spdlog)


target_include_directories(blp_library
  ${WARNING_GUARD} PUBLIC
    $<BUILD_INTERFACE:${PROJECT_SOURCE_DIR}/include>
    $<BUILD_INTERFACE:${PROJECT_BINARY_DIR}/include>)

target_include_directories(blp_library
    SYSTEM PRIVATE
      "\$<BUILD_INTERFACE:${WC3_LIBRARY_INCLUDES}>"
      "\$<BUILD_INTERFACE:${NVTT_LIBRARY_INCLUDES}>"
      "\$<BUILD_INTERFACE:${AMDC_LIBRARY_INCLUDES}>")

if (assets_mpq_importer_BUILD_FUZZ_TESTS)
  target_link_libraries(blp_library PRIVATE -fsanitize=fuzzer-no-link)
  target_compile_options(blp_library PRIVATE -fsanitize=fuzzer-no-link)
endif()

target_compile_features(blp_library PUBLIC cxx_std_${CMAKE_CXX_STANDARD})

set_target_properties(
  blp_library
  PROPERTIES VERSION ${PROJECT_VERSION}
              C_VISIBILITY_PRESET hidden
              CXX_VISIBILITY_PRESET hidden
              VISIBILITY_INLINES_HIDDEN YES)

generate_export_header(blp_library EXPORT_FILE_NAME ${PROJECT_BINARY_DIR}/include/assets_mpq_importer/blp_library_export.hpp)

if(NOT BUILD_SHARED_LIBS)
  target_compile_definitions(blp_library PUBLIC blp_library_STATIC_DEFINE)
endif()
