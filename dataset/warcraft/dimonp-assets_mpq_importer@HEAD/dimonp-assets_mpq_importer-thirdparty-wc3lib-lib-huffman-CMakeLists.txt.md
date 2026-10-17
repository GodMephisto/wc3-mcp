---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/wc3lib/lib/huffman/CMakeLists.txt"
ref: HEAD
---

set(wc3lib_LIB_HUFFMAN_H
	huffman.h
)
set(wc3lib_LIB_HUFFMAN_SRC
	huffman.cpp
)

add_library(wc3libhuffman ${wc3lib_LIB_HUFFMAN_SRC})

# install

#install(TARGETS wc3libhuffman DESTINATION lib)
#install(FILES ${wc3lib_LIB_HUFFMAN_H} DESTINATION include/wc3lib/lib/huffman)