---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "ProjectOptions.cmake"
ref: HEAD
---

include(cmake/SystemLink.cmake)
include(CMakeDependentOption)
include(CheckCXXCompilerFlag)


include(CheckCXXSourceCompiles)

macro(assets_mpq_importer_supports_sanitizers)
  if((CMAKE_CXX_COMPILER_ID MATCHES ".*Clang.*" OR CMAKE_CXX_COMPILER_ID MATCHES ".*GNU.*") AND NOT WIN32)

    message(STATUS "Sanity checking UndefinedBehaviorSanitizer, it should be supported on this platform")
    set(TEST_PROGRAM "int main() { return 0; }")

    # Check if UndefinedBehaviorSanitizer works at link time
    set(CMAKE_REQUIRED_FLAGS "-fsanitize=undefined")
    set(CMAKE_REQUIRED_LINK_OPTIONS "-fsanitize=undefined")
    check_cxx_source_compiles("${TEST_PROGRAM}" HAS_UBSAN_LINK_SUPPORT)

    if(HAS_UBSAN_LINK_SUPPORT)
      message(STATUS "UndefinedBehaviorSanitizer is supported at both compile and link time.")
      set(SUPPORTS_UBSAN ON)
    else()
      message(WARNING "UndefinedBehaviorSanitizer is NOT supported at link time.")
      set(SUPPORTS_UBSAN OFF)
    endif()
  else()
    set(SUPPORTS_UBSAN OFF)
  endif()

  if((CMAKE_CXX_COMPILER_ID MATCHES ".*Clang.*" OR CMAKE_CXX_COMPILER_ID MATCHES ".*GNU.*") AND WIN32)
    set(SUPPORTS_ASAN OFF)
  else()
    if (NOT WIN32)
      message(STATUS "Sanity checking AddressSanitizer, it should be supported on this platform")
      set(TEST_PROGRAM "int main() { return 0; }")

      # Check if AddressSanitizer works at link time
      set(CMAKE_REQUIRED_FLAGS "-fsanitize=address")
      set(CMAKE_REQUIRED_LINK_OPTIONS "-fsanitize=address")
      check_cxx_source_compiles("${TEST_PROGRAM}" HAS_ASAN_LINK_SUPPORT)

      if(HAS_ASAN_LINK_SUPPORT)
        message(STATUS "AddressSanitizer is supported at both compile and link time.")
        set(SUPPORTS_ASAN ON)
      else()
        message(WARNING "AddressSanitizer is NOT supported at link time.")
        set(SUPPORTS_ASAN OFF)
      endif()
    else()
      set(SUPPORTS_ASAN ON)
    endif()
  endif()
endmacro()

macro(assets_mpq_importer_setup_options)
  option(assets_mpq_importer_ENABLE_HARDENING "Enable hardening" ON)
  option(assets_mpq_importer_ENABLE_COVERAGE "Enable coverage reporting" OFF)
  cmake_dependent_option(
    assets_mpq_importer_ENABLE_GLOBAL_HARDENING
    "Attempt to push hardening options to built dependencies"
    ON
    assets_mpq_importer_ENABLE_HARDENING
    OFF)

  assets_mpq_importer_supports_sanitizers()

  if(NOT PROJECT_IS_TOP_LEVEL OR assets_mpq_importer_PACKAGING_MAINTAINER_MODE)
    option(assets_mpq_importer_ENABLE_IPO "Enable IPO/LTO" OFF)
    option(assets_mpq_importer_WARNINGS_AS_ERRORS "Treat Warnings As Errors" OFF)
    option(assets_mpq_importer_ENABLE_USER_LINKER "Enable user-selected linker" OFF)
    option(assets_mpq_importer_ENABLE_SANITIZER_ADDRESS "Enable address sanitizer" OFF)
    option(assets_mpq_importer_ENABLE_SANITIZER_LEAK "Enable leak sanitizer" OFF)
    option(assets_mpq_importer_ENABLE_SANITIZER_UNDEFINED "Enable undefined sanitizer" OFF)
    option(assets_mpq_importer_ENABLE_SANITIZER_THREAD "Enable thread sanitizer" OFF)
    option(assets_mpq_importer_ENABLE_SANITIZER_MEMORY "Enable memory sanitizer" OFF)
    option(assets_mpq_importer_ENABLE_UNITY_BUILD "Enable unity builds" OFF)
    option(assets_mpq_importer_ENABLE_CLANG_TIDY "Enable clang-tidy" OFF)
    option(assets_mpq_importer_ENABLE_CPPCHECK "Enable cpp-check analysis" OFF)
    option(assets_mpq_importer_ENABLE_PCH "Enable precompiled headers" OFF)
    option(assets_mpq_importer_ENABLE_CACHE "Enable ccache" OFF)
  else()
    option(assets_mpq_importer_ENABLE_IPO "Enable IPO/LTO" ON)
    option(assets_mpq_importer_WARNINGS_AS_ERRORS "Treat Warnings As Errors" ON)
    option(assets_mpq_importer_ENABLE_USER_LINKER "Enable user-selected linker" OFF)

    if (NOT DISABLE_ALL_SANITIZERS)
      option(assets_mpq_importer_ENABLE_SANITIZER_ADDRESS "Enable address sanitizer" ${SUPPORTS_ASAN})
      option(assets_mpq_importer_ENABLE_SANITIZER_LEAK "Enable leak sanitizer" OFF)
      option(assets_mpq_importer_ENABLE_SANITIZER_UNDEFINED "Enable undefined sanitizer" ${SUPPORTS_UBSAN})
      option(assets_mpq_importer_ENABLE_SANITIZER_THREAD "Enable thread sanitizer" OFF)
      option(assets_mpq_importer_ENABLE_SANITIZER_MEMORY "Enable memory sanitizer" OFF)
    else()
      option(assets_mpq_importer_ENABLE_SANITIZER_ADDRESS "Enable address sanitizer" OFF)
      option(assets_mpq_importer_ENABLE_SANITIZER_LEAK "Enable leak sanitizer" OFF)
      option(assets_mpq_importer_ENABLE_SANITIZER_UNDEFINED "Enable undefined sanitizer" OFF)
      option(assets_mpq_importer_ENABLE_SANITIZER_THREAD "Enable thread sanitizer" OFF)
      option(assets_mpq_importer_ENABLE_SANITIZER_MEMORY "Enable memory sanitizer" OFF)
    endif()

    option(assets_mpq_importer_ENABLE_UNITY_BUILD "Enable unity builds" OFF)
    option(assets_mpq_importer_ENABLE_CLANG_TIDY "Enable clang-tidy" ${ENABLE_CLANG_TIDY_DEFAULT})
    option(assets_mpq_importer_ENABLE_CPPCHECK "Enable cpp-check analysis" ${ENABLE_CPPCHECK_DEFAULT})
    option(assets_mpq_importer_ENABLE_PCH "Enable precompiled headers" OFF)
    option(assets_mpq_importer_ENABLE_CACHE "Enable ccache" ON)
  endif()

  if(NOT PROJECT_IS_TOP_LEVEL)
    mark_as_advanced(
      assets_mpq_importer_ENABLE_IPO
      assets_mpq_importer_WARNINGS_AS_ERRORS
      assets_mpq_importer_ENABLE_USER_LINKER
      assets_mpq_importer_ENABLE_SANITIZER_ADDRESS
      assets_mpq_importer_ENABLE_SANITIZER_LEAK
      assets_mpq_importer_ENABLE_SANITIZER_UNDEFINED
      assets_mpq_importer_ENABLE_SANITIZER_THREAD
      assets_mpq_importer_ENABLE_SANITIZER_MEMORY
      assets_mpq_importer_ENABLE_UNITY_BUILD
      assets_mpq_importer_ENABLE_CLANG_TIDY
      assets_mpq_importer_ENABLE_CPPCHECK
      assets_mpq_importer_ENABLE_COVERAGE
      assets_mpq_importer_ENABLE_PCH
      assets_mpq_importer_ENABLE_CACHE)
  endif()

endmacro()

macro(assets_mpq_importer_global_options)
  if(assets_mpq_importer_ENABLE_IPO)
    include(cmake/InterproceduralOptimization.cmake)
    assets_mpq_importer_enable_ipo()
  endif()

  assets_mpq_importer_supports_sanitizers()

  if(assets_mpq_importer_ENABLE_HARDENING AND assets_mpq_importer_ENABLE_GLOBAL_HARDENING)
    include(cmake/Hardening.cmake)
    if(NOT SUPPORTS_UBSAN
       OR assets_mpq_importer_ENABLE_SANITIZER_UNDEFINED
       OR assets_mpq_importer_ENABLE_SANITIZER_ADDRESS
       OR assets_mpq_importer_ENABLE_SANITIZER_THREAD
       OR assets_mpq_importer_ENABLE_SANITIZER_LEAK)
      set(ENABLE_UBSAN_MINIMAL_RUNTIME FALSE)
    else()
      set(ENABLE_UBSAN_MINIMAL_RUNTIME TRUE)
    endif()
    assets_mpq_importer_enable_hardening(assets_mpq_importer_options ON ${ENABLE_UBSAN_MINIMAL_RUNTIME})
  endif()
endmacro()

macro(assets_mpq_importer_local_options)
  if(PROJECT_IS_TOP_LEVEL)
    include(cmake/StandardProjectSettings.cmake)
  endif()

  add_library(assets_mpq_importer_warnings INTERFACE)
  add_library(assets_mpq_importer_options INTERFACE)

  include(cmake/CompilerWarnings.cmake)
  assets_mpq_importer_set_project_warnings(
    assets_mpq_importer_warnings
    ${assets_mpq_importer_WARNINGS_AS_ERRORS}
    ""
    ""
    ""
    "")

  if(assets_mpq_importer_ENABLE_USER_LINKER)
    include(cmake/Linker.cmake)
    assets_mpq_importer_configure_linker(assets_mpq_importer_options)
  endif()

  include(cmake/Sanitizers.cmake)
  assets_mpq_importer_enable_sanitizers(
    assets_mpq_importer_options
    ${assets_mpq_importer_ENABLE_SANITIZER_ADDRESS}
    ${assets_mpq_importer_ENABLE_SANITIZER_LEAK}
    ${assets_mpq_importer_ENABLE_SANITIZER_UNDEFINED}
    ${assets_mpq_importer_ENABLE_SANITIZER_THREAD}
    ${assets_mpq_importer_ENABLE_SANITIZER_MEMORY})

  set_target_properties(assets_mpq_importer_options PROPERTIES UNITY_BUILD ${assets_mpq_importer_ENABLE_UNITY_BUILD})

  if(assets_mpq_importer_ENABLE_PCH)
    target_precompile_headers(
      assets_mpq_importer_options
      INTERFACE
      <vector>
      <string>
      <utility>)
  endif()

  if(assets_mpq_importer_ENABLE_CACHE)
    include(cmake/Cache.cmake)
    assets_mpq_importer_enable_cache()
  endif()

  include(cmake/StaticAnalyzers.cmake)
  if(assets_mpq_importer_ENABLE_CLANG_TIDY)
    assets_mpq_importer_enable_clang_tidy(assets_mpq_importer_options ${assets_mpq_importer_WARNINGS_AS_ERRORS})
  endif()

  if(assets_mpq_importer_ENABLE_CPPCHECK)
    assets_mpq_importer_enable_cppcheck(${assets_mpq_importer_WARNINGS_AS_ERRORS} "" # override cppcheck options
    )
  endif()

  if(assets_mpq_importer_ENABLE_COVERAGE)
    include(cmake/Tests.cmake)
    assets_mpq_importer_enable_coverage(assets_mpq_importer_options)
  endif()

  if(assets_mpq_importer_WARNINGS_AS_ERRORS)
    check_cxx_compiler_flag("-Wl,--fatal-warnings" LINKER_FATAL_WARNINGS)
    if(LINKER_FATAL_WARNINGS)
      # This is not working consistently, so disabling for now
      # target_link_options(assets_mpq_importer_options INTERFACE -Wl,--fatal-warnings)
    endif()

    if(CMAKE_CXX_COMPILER_ID STREQUAL "Clang")
      # Suppress error: argument unused during compilation
      target_compile_options(assets_mpq_importer_warnings INTERFACE -Wno-unused-command-line-argument)
      # Suppress -Wc2y-extensions when using __COUNTER__ to generate unique C++ identifiers.
      target_compile_options(assets_mpq_importer_warnings INTERFACE -Wno-c2y-extensions)
    endif()

  endif()

  if(assets_mpq_importer_ENABLE_HARDENING AND NOT assets_mpq_importer_ENABLE_GLOBAL_HARDENING)
    include(cmake/Hardening.cmake)
    if(NOT SUPPORTS_UBSAN
       OR assets_mpq_importer_ENABLE_SANITIZER_UNDEFINED
       OR assets_mpq_importer_ENABLE_SANITIZER_ADDRESS
       OR assets_mpq_importer_ENABLE_SANITIZER_THREAD
       OR assets_mpq_importer_ENABLE_SANITIZER_LEAK)
      set(ENABLE_UBSAN_MINIMAL_RUNTIME FALSE)
    else()
      set(ENABLE_UBSAN_MINIMAL_RUNTIME TRUE)
    endif()
    assets_mpq_importer_enable_hardening(assets_mpq_importer_options OFF ${ENABLE_UBSAN_MINIMAL_RUNTIME})
  endif()

endmacro()
