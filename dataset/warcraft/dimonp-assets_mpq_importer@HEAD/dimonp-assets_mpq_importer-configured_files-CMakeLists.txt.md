---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "configured_files/CMakeLists.txt"
ref: HEAD
---


# A very simple example of a configured file that might need to be
# converted to one that is publicly installed in the case that
# you are developing a library
configure_file("config.hpp.in" "${CMAKE_BINARY_DIR}/configured_files/include/internal_use_only/config.hpp" ESCAPE_QUOTES)


