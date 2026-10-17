---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/wc3lib/lib/pklib/CMakeLists.txt"
ref: HEAD
---

set(wc3lib_LIB_PKLIB_H
	pklib.h
)
set(wc3lib_LIB_PKLIB_SRC
	crc32.c
	explode.c
	implode.c
)



add_library(wc3libpklib ${wc3lib_LIB_PKLIB_SRC})
set_source_files_properties(${wc3lib_LIB_PKLIB_H} ${wc3lib_LIB_PKLIB_SRC} PROPERTIES LANGUAGE CXX)
set_target_properties(wc3libpklib PROPERTIES LINKER_LANGUAGE CXX)

# install

#install(TARGETS wc3libpklib DESTINATION lib)
#install(FILES ${wc3lib_LIB_PKLIB_H} DESTINATION include/wc3lib/lib/pklib)