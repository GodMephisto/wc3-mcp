---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/nvidia-texture-tools/extern/etcpack/CMakeLists.txt"
ref: HEAD
---

SET(ETCPACK_SRCS
    etcdec.cxx
    etcpack.cxx
    image.cxx
    image.h)

add_definitions("-DPGMOUT=0")
ADD_LIBRARY(etcpack STATIC ${ETCPACK_SRCS})

