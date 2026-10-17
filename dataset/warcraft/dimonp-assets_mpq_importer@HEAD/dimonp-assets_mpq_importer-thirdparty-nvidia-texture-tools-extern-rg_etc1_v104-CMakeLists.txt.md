---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/nvidia-texture-tools/extern/rg_etc1_v104/CMakeLists.txt"
ref: HEAD
---


SET(RGETC_SRCS
	rg_etc1.cpp
	rg_etc1.h)

ADD_LIBRARY(rg_etc1 STATIC ${RGETC_SRCS})

