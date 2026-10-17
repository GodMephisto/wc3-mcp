---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/nvidia-texture-tools/src/nvthread/CMakeLists.txt"
ref: HEAD
---

PROJECT(nvthread)

SET(THREAD_SRCS
	nvthread.h nvthread.cpp
	ParallelFor.h ParallelFor.cpp
	progschj/ThreadPool.h)

INCLUDE_DIRECTORIES(${CMAKE_CURRENT_SOURCE_DIR})

# targets
ADD_DEFINITIONS(-DNVTHREAD_EXPORTS)

IF(NVTHREAD_SHARED)
	ADD_LIBRARY(nvthread SHARED ${THREAD_SRCS})
ELSE(NVTHREAD_SHARED)
	ADD_LIBRARY(nvthread ${THREAD_SRCS})
ENDIF(NVTHREAD_SHARED)

TARGET_LINK_LIBRARIES(nvthread ${LIBS} nvcore)

#INSTALL(TARGETS nvthread
#	RUNTIME DESTINATION bin
#	LIBRARY DESTINATION lib
#	ARCHIVE DESTINATION lib/static)
