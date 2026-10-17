---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/nvidia-texture-tools/src/nvtt/squish/CMakeLists.txt"
ref: HEAD
---

PROJECT(nvsquish)

INCLUDE_DIRECTORIES(${CMAKE_CURRENT_SOURCE_DIR})

SET(SQUISH_SRCS
	fastclusterfit.cpp
	fastclusterfit.h
	weightedclusterfit.cpp
	weightedclusterfit.h
	colourblock.cpp
	colourblock.h
	colourfit.cpp
	colourfit.h
	colourset.cpp
	colourset.h
	config.h
	maths.cpp
	maths.h
	simd.h
	simd_sse.h
	simd_ve.h)

ADD_LIBRARY(nvsquish STATIC ${SQUISH_SRCS})
