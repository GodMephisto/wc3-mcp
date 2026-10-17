---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/mpq_library/CMakeLists.txt"
ref: HEAD
---

find_package(re2 CONFIG REQUIRED)

include(GenerateExportHeader)

add_library(mpq_library)
add_library(assets_mpq_importer::mpq_library ALIAS mpq_library)

target_sources(mpq_library
    PRIVATE
      mpq.cpp
    PUBLIC
      FILE_SET HEADERS
      BASE_DIRS ${CMAKE_SOURCE_DIR}/include
      FILES
        ${CMAKE_SOURCE_DIR}/include/assets_mpq_importer/mpq.hpp
)


target_link_libraries(mpq_library
  PRIVATE
    assets_mpq_importer_options assets_mpq_importer_warnings
    wc3libcore
    wc3libmpq
    re2::re2)

target_include_directories(mpq_library
  ${WARNING_GUARD} PUBLIC
    $<BUILD_INTERFACE:${PROJECT_SOURCE_DIR}/include>
    $<BUILD_INTERFACE:${PROJECT_BINARY_DIR}/include>)

target_include_directories(mpq_library SYSTEM PRIVATE "\$<BUILD_INTERFACE:${WC3_LIBRARY_INCLUDES}>")

target_compile_features(mpq_library PUBLIC cxx_std_${CMAKE_CXX_STANDARD})

set_target_properties(
  mpq_library
  PROPERTIES VERSION ${PROJECT_VERSION}
              C_VISIBILITY_PRESET hidden
              CXX_VISIBILITY_PRESET hidden
              VISIBILITY_INLINES_HIDDEN YES)

generate_export_header(mpq_library EXPORT_FILE_NAME ${PROJECT_BINARY_DIR}/include/assets_mpq_importer/mpq_library_export.hpp)

if(NOT BUILD_SHARED_LIBS)
  target_compile_definitions(mpq_library PUBLIC mpq_library_STATIC_DEFINE)
endif()
