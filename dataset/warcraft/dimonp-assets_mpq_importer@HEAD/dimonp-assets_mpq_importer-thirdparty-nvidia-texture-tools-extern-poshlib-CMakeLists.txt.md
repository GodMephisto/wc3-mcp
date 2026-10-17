---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/nvidia-texture-tools/extern/poshlib/CMakeLists.txt"
ref: HEAD
---

SET(POSHLIB_SRCS
	posh.c
	posh.h)

ADD_LIBRARY(posh STATIC ${POSHLIB_SRCS})

