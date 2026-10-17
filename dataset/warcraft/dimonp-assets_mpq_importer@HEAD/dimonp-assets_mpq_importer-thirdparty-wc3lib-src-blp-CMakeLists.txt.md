---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/wc3lib/src/blp/CMakeLists.txt"
ref: HEAD
---

if (BLP)
    find_package(Boost COMPONENTS ptr_container multi_array REQUIRED CONFIG)
    include_directories(${Boost_INCLUDE_DIRS})

	# for proper "config.h" detection we use relative paths of modules as well
	include_directories("${PROJECT_BINARY_DIR}/src/blp")

	# Static linking with JPEG (BLP library)
	find_package(JPEG REQUIRED)
	include_directories(${JPEG_INCLUDE_DIR})

	set(wc3lib_BLP_H
		blp.hpp
		jpeg.hpp
		platform.hpp
	)
	set(wc3lib_BLP_SRC
		blp.cpp
		jpeg.cpp
	)

	add_library(wc3libblp ${wc3lib_BLP_SRC})
	target_link_libraries(wc3libblp
		wc3libcore
		${GETTEXT_LIBRARIES}
		${Boost_LIBRARIES}
		${JPEG_LIBRARIES}
	)

	if(BUILD_TESTING)
		add_subdirectory(test)
	endif ()

	# install
#	install(TARGETS wc3libblp DESTINATION lib)
#	install(FILES ${wc3lib_BLP_H} DESTINATION include/wc3lib/blp)
endif ()
