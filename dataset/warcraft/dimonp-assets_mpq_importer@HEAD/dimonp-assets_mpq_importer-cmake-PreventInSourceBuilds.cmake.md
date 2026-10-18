---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "cmake/PreventInSourceBuilds.cmake"
ref: HEAD
---

#
# This function will prevent in-source builds
#
function(assets_mpq_importer_assure_out_of_source_builds)
  # make sure the user doesn't play dirty with symlinks
  file(REAL_PATH "${CMAKE_SOURCE_DIR}" srcdir)
  file(REAL_PATH "${CMAKE_BINARY_DIR}" bindir)

  # disallow in-source builds
  if("${srcdir}" STREQUAL "${bindir}")
    message("######################################################")
    message("Warning: in-source builds are disabled")
    message("Please create a separate build directory and run cmake from there")
    message("######################################################")
    message(FATAL_ERROR "Quitting configuration")
  endif()
endfunction()

assets_mpq_importer_assure_out_of_source_builds()
