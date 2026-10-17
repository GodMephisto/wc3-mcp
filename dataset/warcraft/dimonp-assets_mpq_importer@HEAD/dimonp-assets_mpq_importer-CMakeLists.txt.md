---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "CMakeLists.txt"
ref: HEAD
---

cmake_minimum_required(VERSION 3.31)

option(BUILD_TESTING "Build the testing suite" ON)

if(BUILD_TESTING)
  list(APPEND VCPKG_MANIFEST_FEATURES "tests")
endif()

# to avoid clang-tidy errors
set(CMAKE_CXX_SCAN_FOR_MODULES OFF)

# Set the project name and language
project(
  assets_mpq_importer
  VERSION 0.1.2
  DESCRIPTION "Assets MPQ importer"
  HOMEPAGE_URL "https://github.com/dimonp/assets_mpq_importer"
  LANGUAGES CXX C)

option(DISABLE_ALL_SANITIZERS "Enable Cppcheck static analyzer" OFF)
option(ENABLE_CPPCHECK_DEFAULT "Enable Cppcheck static analyzer" ON)
option(ENABLE_CLANG_TIDY_DEFAULT "Enable Clang-tidy static analyzer" ON)

# Only set the cxx_standard if it is not set by someone else
if (NOT DEFINED CMAKE_CXX_STANDARD)
  set(CMAKE_CXX_STANDARD 23)
  set(CMAKE_CXX_STANDARD_REQUIRED True)
endif()

# strongly encouraged to enable this globally to avoid conflicts between
# -Wpedantic being enabled and -std=c++20 and -std=gnu++20 for example
# when compiling with PCH enabled
set(CMAKE_CXX_EXTENSIONS OFF)

include(FetchContent)
include(cmake/PreventInSourceBuilds.cmake)
include(ProjectOptions.cmake)

assets_mpq_importer_setup_options()
assets_mpq_importer_global_options()

message(assets_mpq_importer_ENABLE_HARDENING: "${assets_mpq_importer_ENABLE_HARDENING}")
message(ENABLE_UBSAN_MINIMAL_RUNTIME: "${ENABLE_UBSAN_MINIMAL_RUNTIME}")
message(assets_mpq_importer_ENABLE_SANITIZER_UNDEFINED: "${assets_mpq_importer_ENABLE_SANITIZER_UNDEFINED}")
message(assets_mpq_importer_ENABLE_SANITIZER_ADDRESS: "${assets_mpq_importer_ENABLE_SANITIZER_ADDRESS}")
message(assets_mpq_importer_ENABLE_SANITIZER_THREAD: "${assets_mpq_importer_ENABLE_SANITIZER_THREAD}")
message(assets_mpq_importer_ENABLE_SANITIZER_LEAK: "${assets_mpq_importer_ENABLE_SANITIZER_LEAK}")

if(MSYS)
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wa,-mbig-obj")
elseif(MSVC)
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /bigobj")
endif()

# fake call for damn conan provider
find_package(fmt REQUIRED)
enable_testing()

# Third-party dependency libraries
add_subdirectory(thirdparty)

assets_mpq_importer_local_options()

# don't know if this should be set globally from here or not...
set(CMAKE_CXX_VISIBILITY_PRESET hidden)

set(GIT_SHA
    "Unknown"
    CACHE STRING "SHA this build was generated from")
string(
  SUBSTRING "${GIT_SHA}"
            0
            8
            GIT_SHORT_SHA)

target_compile_features(assets_mpq_importer_options INTERFACE cxx_std_${CMAKE_CXX_STANDARD})

add_library(assets_mpq_importer::assets_mpq_importer_options ALIAS assets_mpq_importer_options)
add_library(assets_mpq_importer::assets_mpq_importer_warnings ALIAS assets_mpq_importer_warnings)

# configure files based on CMake configuration options
add_subdirectory(configured_files)

# Adding the src:
add_subdirectory(src)

# Don't even look at tests if we're not top level
if(NOT PROJECT_IS_TOP_LEVEL)
  return()
endif()

# Adding the tests:
if(BUILD_TESTING)
  include(CTest)
  add_subdirectory(test)
endif()

# If MSVC is being used, and ASAN is enabled, we need to set the debugger environment
# so that it behaves well with MSVC's debugger, and we can run the target from visual studio
if(MSVC)
  get_all_installable_targets(all_targets)
  message("all_targets=${all_targets}")
  set_target_properties(${all_targets} PROPERTIES VS_DEBUGGER_ENVIRONMENT "PATH=$(VC_ExecutablePath_x64);%PATH%")
endif()

# set the startup project for the "play" button in MSVC
set_property(DIRECTORY PROPERTY VS_STARTUP_PROJECT importer)

if(CMAKE_SKIP_INSTALL_RULES)
  return()
endif()

include(cmake/PackageProject.cmake)

# Add other targets that you want installed here, by default we just package the one executable
# we know we want to ship
# assets_mpq_importer_package_project(
#   TARGETS
#   importer
#   merger
#   assets_mpq_importer_options
#   assets_mpq_importer_warnings
#   # FIXME: this does not work! CK
#   # PRIVATE_DEPENDENCIES_CONFIGURED project_options project_warnings
# )

include(GNUInstallDirs)

if(CMAKE_SYSTEM_NAME STREQUAL "Windows")
  file(GLOB SCRIPT_FILES "${CMAKE_SOURCE_DIR}/scripts/*.bat")
elseif(CMAKE_SYSTEM_NAME STREQUAL "Linux")
  file(GLOB SCRIPT_FILES "${CMAKE_SOURCE_DIR}/scripts/*.sh")
endif()
install(PROGRAMS ${SCRIPT_FILES} DESTINATION "${CMAKE_INSTALL_BINDIR}")
