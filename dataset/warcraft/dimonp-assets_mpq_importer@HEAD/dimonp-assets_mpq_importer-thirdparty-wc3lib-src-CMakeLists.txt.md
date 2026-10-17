---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/wc3lib/src/CMakeLists.txt"
ref: HEAD
---

#find_package(Gettext REQUIRED)

# Boost
if (DEBUG AND NOT WIN32)
	#set(Boost_DEBUG ON)
	add_definitions(-DBOOST_TEST_DYN_LINK=1)
endif ()

if (WIN32)
    #add_definitions(-DBOOST_SYSTEM_NO_DEPRECATED=1)
    # TODO static libs are not found on Windows - finds only .lib files
    set(Boost_USE_STATIC_LIBS ON)

    # http://stackoverflow.com/questions/14181351/i-got-duplicate-section-errors-when-compiling-boost-regex-with-size-optimizati
    #set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -march=x86-64")
    # set(Boost_USE_STATIC_RUNTIME ON)
endif ()

find_package(Boost REQUIRED COMPONENTS serialization iostreams CONFIG)
include_directories(${Boost_INCLUDE_DIRS})

link_directories(${Boost_LIBRARY_DIRS})
include_directories(${GETTEXT_INCLUDE_DIR} ${Boost_INCLUDE_DIRS})

set(wc3lib_CORE_H
	color.hpp
	core.hpp
	exception.hpp
	format.hpp
	i18n.hpp
	platform.hpp
	qi.hpp
	spirit.hpp
	utilities.hpp
	vertex.hpp
	wc3lib.hpp
)
set(wc3lib_CORE_SRC
	color.cpp
	exception.cpp
	format.cpp
	spirit.cpp
	utilities.cpp
	vertex.cpp
)

add_library(wc3libcore ${wc3lib_CORE_SRC})
target_link_libraries(wc3libcore ${Boost_LIBRARIES})

if(BUILD_TESTING)
	add_subdirectory(test)
endif ()

if (BLP)
	add_subdirectory(blp)
endif ()

if (MDLX)
	add_subdirectory(mdlx)
endif ()
if (MPQ)
	add_subdirectory(mpq)
endif ()
if (MAP)
	add_subdirectory(map)
endif ()

# libraries without editor
set(wc3lib_CORE_LIBRARIES wc3libcore)
if (BLP)
	list(APPEND wc3lib_CORE_LIBRARIES wc3libblp)
endif ()
if (MDLX)
	list(APPEND wc3lib_CORE_LIBRARIES wc3libmdlx)
endif ()
if (MPQ)
	list(APPEND wc3lib_CORE_LIBRARIES wc3libmpq)
endif ()
if (MAP)
	list(APPEND wc3lib_CORE_LIBRARIES wc3libmap)
endif ()

if (BLP)
#	install(FILES blp.hpp DESTINATION include/wc3lib)
endif ()
if (MDLX)
#	install(FILES mdlx.hpp DESTINATION include/wc3lib)
endif ()
if (MPQ)
#	install(FILES mpq.hpp DESTINATION include/wc3lib)
endif ()
if (MAP)
#	install(FILES map.hpp DESTINATION include/wc3lib)
endif ()

# install
#install(TARGETS wc3libcore DESTINATION lib)
#install(FILES ${wc3lib_CORE_H} DESTINATION include/wc3lib)
