---
type: vendor-doc
source_repo: "https://github.com/notmgn/smpq"
source_path: "CMakeLists.txt"
ref: HEAD
---

#
#    CMakeLists.txt - cmake for StormLib MPQ archiving utility
#    Copyright (C) 2010 - 2012  Pali Rohár <pali.rohar@gmail.com>
#
#    This program is free software: you can redistribute it and/or modify
#    it under the terms of the GNU General Public License as published by
#    the Free Software Foundation, either version 3 of the License, or
#    (at your option) any later version.
#
#    This program is distributed in the hope that it will be useful,
#    but WITHOUT ANY WARRANTY; without even the implied warranty of
#    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#    GNU General Public License for more details.
#
#    You should have received a copy of the GNU General Public License
#    along with this program.  If not, see <http://www.gnu.org/licenses/>.
#
#

cmake_minimum_required(VERSION 2.6...4.0)
project(SMPQ)
set(VERSION 1.7)

option(WITH_CMD "Enable smpq console application" ON)
option(WITH_KDE "Enable KDE4 kio_smpq plugin" ON)

if(WIN32)
	option(WITH_NSIS "Create Windows NSIS Installer" OFF)
endif(WIN32)

if(NOT STORMLIB_INCLUDE_DIR OR NOT STORMLIB_LIBRARY)

	find_path(STORMLIB_INCLUDE_DIR StormLib.h)
	find_library(STORMLIB_LIBRARY NAMES StormLib storm)

	if(NOT STORMLIB_INCLUDE_DIR)
		message(FATAL_ERROR "StormLib.h header file not found")
	endif(NOT STORMLIB_INCLUDE_DIR)

	if(NOT STORMLIB_LIBRARY)
		message(FATAL_ERROR "StormLib or storm library not found")
	endif(NOT STORMLIB_LIBRARY)

	try_compile(CHECK ${CMAKE_CURRENT_BINARY_DIR} ${CMAKE_CURRENT_SOURCE_DIR}/check.c COMPILE_DEFINITIONS -I${STORMLIB_INCLUDE_DIR})

	if(NOT CHECK)
		message(FATAL_ERROR "Found old StormLib version")
	endif(NOT CHECK)

	message(STATUS "Found StormLib header: ${STORMLIB_INCLUDE_DIR}/StormLib.h")
	message(STATUS "Found StormLib library: ${STORMLIB_LIBRARY}")

	mark_as_advanced(STORMLIB_INCLUDE_DIR STORMLIB_LIBRARY)

endif(NOT STORMLIB_INCLUDE_DIR OR NOT STORMLIB_LIBRARY)

include_directories(${STORMLIB_INCLUDE_DIR})

add_definitions(-DVERSION="${VERSION}")

set(SMPQ_SRCS
	append.c
	extract.c
	info.c
	listfiles.c
	main.c
	print.c
	remove.c
	rename.c
)

if(MSVC)

	set(SMPQ_SRCS
		${SMPQ_SRCS}
		dirname.c
	)

endif(MSVC)

set(MANGEN_SRCS
	mangen.c
)

set(KIO_SMPQ_SRCS
	kio_smpq.cpp
)

set(SMPQ_NSIS
	${CMAKE_CURRENT_SOURCE_DIR}/smpq.nsi
)

if(WITH_CMD)

	add_executable(smpq ${SMPQ_SRCS})
	target_link_libraries(smpq ${STORMLIB_LIBRARY})

	if(WIN32 AND NOT MSVC)
		set_target_properties(smpq PROPERTIES LINK_FLAGS -static)
		target_link_libraries(smpq wininet stdc++)
	endif(WIN32 AND NOT MSVC)

	install(TARGETS smpq DESTINATION bin)

	if(NOT CMAKE_CROSSCOMPILING)

		add_executable(mangen ${MANGEN_SRCS})
		add_custom_command(OUTPUT smpq.1 COMMAND mangen > smpq.1 DEPENDS mangen)
		add_custom_target(man ALL DEPENDS smpq.1)

		install(FILES ${CMAKE_CURRENT_BINARY_DIR}/smpq.1 DESTINATION share/man/man1)

	endif(NOT CMAKE_CROSSCOMPILING)

	if(WIN32 AND WITH_NSIS)

		find_package(SelfPackers REQUIRED)

		if(NOT MAKENSIS)

			find_program(MAKENSIS makensis)

			if(NOT MAKENSIS)
				message(FATAL_ERROR "makensis not found")
			else(NOT MAKENSIS)
				message(STATUS "Found makensis: ${MAKENSIS}")
			endif(NOT MAKENSIS)

			mark_as_advanced(MAKENSIS)

		endif(NOT MAKENSIS)

		if(CMAKE_SIZEOF_VOID_P STREQUAL 4)

			set(MAKENSIS_OUTPUT SMPQ-${VERSION}.exe)

		else(CMAKE_SIZEOF_VOID_P STREQUAL 4)

			set(MAKENSIS_FLAGS -DX86_64 ${MAKENSIS_FLAGS})
			set(MAKENSIS_OUTPUT SMPQ-${VERSION}-x86_64.exe)

		endif(CMAKE_SIZEOF_VOID_P STREQUAL 4)

		if(NOT CMAKE_VERBOSE_MAKEFILE)
			set(MAKENSIS_FLAGS -V2 -DQUIET ${MAKENSIS_FLAGS})
		endif(NOT CMAKE_VERBOSE_MAKEFILE)

		set(MAKENSIS_FLAGS ${MAKENSIS_FLAGS} -DUPX="${SELF_PACKER_FOR_EXECUTABLE}" -DVERSION="${VERSION}" -NOCD)

		file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/COPYING DESTINATION ${CMAKE_CURRENT_BINARY_DIR})
		file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/README DESTINATION ${CMAKE_CURRENT_BINARY_DIR})
		file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/CHANGELOG DESTINATION ${CMAKE_CURRENT_BINARY_DIR})

		if(CMAKE_SIZEOF_VOID_P STREQUAL 4)
			add_custom_command(TARGET smpq POST_BUILD
				COMMAND ${SELF_PACKER_FOR_EXECUTABLE}
				ARGS ${SELF_PACKER_FOR_EXECUTABLE_FLAGS} -qq -9 smpq${CMAKE_EXECUTABLE_SUFFIX}
			)
		endif(CMAKE_SIZEOF_VOID_P STREQUAL 4)

		add_custom_command(OUTPUT ${MAKENSIS_OUTPUT} COMMAND ${MAKENSIS} ARGS ${MAKENSIS_FLAGS} ${SMPQ_NSIS} DEPENDS smpq smpq.nsi COPYING README CHANGELOG)
		add_custom_target(installer ALL DEPENDS ${MAKENSIS_OUTPUT})

	endif(WIN32 AND WITH_NSIS)

endif(WITH_CMD)

if(WITH_KDE)

	find_package(KDE4 REQUIRED)
	include(KDE4Defaults)

	set(LIBRARY_OUTPUT_PATH ${CMAKE_BINARY_DIR})
	set_target_properties(KDE4__kdecore PROPERTIES IMPORTED_LINK_INTERFACE_LIBRARIES_DEBIAN "Qt4::QtCore")
	set_target_properties(KDE4__kio PROPERTIES IMPORTED_LINK_INTERFACE_LIBRARIES_DEBIAN "KDE4__kdecore")

	include_directories(${KDE4_INCLUDES})

	set(CMAKE_REQUIRED_INCLUDES "${CMAKE_REQUIRED_INCLUDES};${QT_INCLUDE_DIR}")
	check_symbol_exists(Q_OS_UNIX "QtCore/qglobal.h" Q_OS_UNIX)

	if(NOT Q_OS_UNIX)
		add_definitions(-DKDEDIR=${CMAKE_INSTALL_PREFIX})
	endif(NOT Q_OS_UNIX)

	kde4_add_plugin(kio_smpq ${KIO_SMPQ_SRCS})
	target_link_libraries(kio_smpq ${STORMLIB_LIBRARY} ${KDE4_KIO_LIBS})

	install(TARGETS kio_smpq DESTINATION ${PLUGIN_INSTALL_DIR})
	install(FILES smpq.protocol DESTINATION ${SERVICES_INSTALL_DIR})
	install(FILES smpq.xml DESTINATION ${XDG_MIME_INSTALL_DIR})

endif(WITH_KDE)
