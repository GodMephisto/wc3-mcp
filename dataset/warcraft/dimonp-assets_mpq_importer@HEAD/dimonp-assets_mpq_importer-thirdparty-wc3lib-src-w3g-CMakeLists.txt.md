---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/wc3lib/src/w3g/CMakeLists.txt"
ref: HEAD
---

if (W3G)
	# for proper "config.h" detection we use relative paths of modules as well
	include_directories("${PROJECT_BINARY_DIR}/src/w3g")

	set(wc3lib_W3G_H
		platform.hpp
	)
	set(wc3lib_W3G_SRC
	)

	# install
#	install(FILES ${wc3lib_W3G_H} DESTINATION include/wc3lib/w3g)
endif ()