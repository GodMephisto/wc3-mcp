---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/w3m_library/CMakeLists.txt"
ref: HEAD
---

include(GenerateExportHeader)

add_library(w3m_library)
add_library(assets_mpq_importer::w3m_library ALIAS w3m_library)

target_sources(w3m_library
    PRIVATE
      w3m.cpp
    PUBLIC
      FILE_SET HEADERS
      BASE_DIRS ${CMAKE_SOURCE_DIR}/include
      FILES
        ${CMAKE_SOURCE_DIR}/include/assets_mpq_importer/w3m.hpp
)


target_link_libraries(w3m_library
  PRIVATE
    assets_mpq_importer_options assets_mpq_importer_warnings
    wc3libcore
    wc3libmap)

target_include_directories(w3m_library
  ${WARNING_GUARD} PUBLIC
    $<BUILD_INTERFACE:${PROJECT_SOURCE_DIR}/include>
    $<BUILD_INTERFACE:${PROJECT_BINARY_DIR}/include>)

target_include_directories(w3m_library SYSTEM PRIVATE "\$<BUILD_INTERFACE:${WC3_LIBRARY_INCLUDES}>")

if (assets_mpq_importer_BUILD_FUZZ_TESTS)
  target_link_libraries(w3m_library PRIVATE -fsanitize=fuzzer-no-link)
  target_compile_options(w3m_library PRIVATE -fsanitize=fuzzer-no-link)
endif()

target_compile_features(w3m_library PUBLIC cxx_std_${CMAKE_CXX_STANDARD})

set_target_properties(
  w3m_library
  PROPERTIES VERSION ${PROJECT_VERSION}
              C_VISIBILITY_PRESET hidden
              CXX_VISIBILITY_PRESET hidden
              VISIBILITY_INLINES_HIDDEN YES)

generate_export_header(w3m_library EXPORT_FILE_NAME ${PROJECT_BINARY_DIR}/include/assets_mpq_importer/w3m_library_export.hpp)

if(NOT BUILD_SHARED_LIBS)
  target_compile_definitions(w3m_library PUBLIC w3m_library_STATIC_DEFINE)
endif()
