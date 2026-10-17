---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/nvidia-texture-tools/src/bc6h/CMakeLists.txt"
ref: HEAD
---

PROJECT(bc6h)

INCLUDE_DIRECTORIES(${CMAKE_CURRENT_SOURCE_DIR})

SET(BC6H_SRCS
	bits.h
	shapes_two.h
	tile.h
	zoh_utils.cpp
	zoh_utils.h
	zoh.cpp
	zoh.h
	zohone.cpp
	zohtwo.cpp)

ADD_LIBRARY(bc6h STATIC ${BC6H_SRCS})
TARGET_LINK_LIBRARIES(bc6h nvcore nvmath)
