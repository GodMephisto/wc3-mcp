---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/nvidia-texture-tools/src/CMakeLists.txt"
ref: HEAD
---


INCLUDE_DIRECTORIES(${CMAKE_CURRENT_SOURCE_DIR} ${CMAKE_CURRENT_BINARY_DIR})
INCLUDE_DIRECTORIES(${NV_SOURCE_DIR}/extern/poshlib)
INCLUDE_DIRECTORIES(${NV_SOURCE_DIR}/extern/stb)

SUBDIRS(nvcore)
SUBDIRS(nvmath)
SUBDIRS(nvimage)
SUBDIRS(nvthread)
SUBDIRS(nvtt)
SUBDIRS(bc6h)
SUBDIRS(bc7)

# CUDA
FIND_PACKAGE(CUDAToolkit )
IF(CUDAToolkit_FOUND)
    IF(MINGW)
        MESSAGE(STATUS "Looking for CUDA - not supported on MinGW")
        UNSET(CUDAToolkit_FOUND)
    ENDIF()

    IF(CUDAToolkit_FOUND)
        SET(HAVE_CUDA ${CUDAToolkit_FOUND} CACHE BOOL "Set to TRUE if CUDA is found, FALSE otherwise")
        MESSAGE(STATUS "Looking for CUDA - found")
    ENDIF()
ELSE()
    MESSAGE(STATUS "Looking for CUDA - not found")
ENDIF()

# OpenMP
# INCLUDE(FindOpenMP)
# IF(OPENMP_FOUND)
# 	SET(HAVE_OPENMP ${OPENMP_FOUND} CACHE BOOL "Set to TRUE if OpenMP is found, FALSE otherwise")
# 	MESSAGE(STATUS "Looking for OpenMP - found")
# 	SET(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} ${OpenMP_C_FLAGS}")
# 	SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} ${OpenMP_CXX_FLAGS}")
# ELSE(OPENMP_FOUND)
# 	MESSAGE(STATUS "Looking for OpenMP - not found")
# ENDIF(OPENMP_FOUND)

# Threads
FIND_PACKAGE(Threads REQUIRED)
MESSAGE(STATUS "Use thread library: ${CMAKE_THREAD_LIBS_INIT}")
SET(LIBS ${LIBS} ${CMAKE_THREAD_LIBS_INIT})

# configuration file
INCLUDE(CheckIncludeFiles)

CHECK_INCLUDE_FILES("unistd.h" HAVE_UNISTD_H)
CHECK_INCLUDE_FILES("stdarg.h" HAVE_STDARG_H)
CHECK_INCLUDE_FILES("signal.h" HAVE_SIGNAL_H)
CHECK_INCLUDE_FILES("execinfo.h" HAVE_EXECINFO_H)
CHECK_INCLUDE_FILES("malloc.h" HAVE_MALLOC_H)
CHECK_INCLUDE_FILES("dispatch/dispatch.h" HAVE_DISPATCH_H)

CONFIGURE_FILE(${CMAKE_CURRENT_SOURCE_DIR}/nvconfig.h.in ${CMAKE_CURRENT_BINARY_DIR}/nvconfig.h)

