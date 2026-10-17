---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "CMakeLists.txt"
ref: HEAD
---

cmake_minimum_required(VERSION 3.29)

# Hack because vcpkg doesn't allow you to just declare that you want to be static on all platforms
# Linux and MacOS are static by default
# if(WIN32)
#    SET(VCPKG_TARGET_TRIPLET "x64-windows-static")
# endif()

project (HiveWE CXX)

include(GNUInstallDirs)

if(APPLE)
	option(HIVEWE_ENABLE_METAL "Build macOS Metal renderer support" ON)
else()
	option(HIVEWE_ENABLE_METAL "Build macOS Metal renderer support" OFF)
endif()

if(APPLE AND HIVEWE_ENABLE_METAL)
	enable_language(OBJCXX)
endif()

set(HIVEWE_BUILD_OPENGL_RENDERER ON)
if(APPLE AND HIVEWE_ENABLE_METAL)
	set(HIVEWE_BUILD_OPENGL_RENDERER OFF)
endif()

if(APPLE)
	# QtGui and SOIL2 can ask CMake for OpenGL even when HiveWE's OpenGL
	# renderer is disabled. Pin those transitive lookups to Apple's framework so
	# Metal builds never pick up XQuartz libGL from /usr/X11R6.
	find_library(HIVEWE_MACOS_OPENGL_FRAMEWORK OpenGL REQUIRED)
	set(OPENGL_gl_LIBRARY "${HIVEWE_MACOS_OPENGL_FRAMEWORK}" CACHE FILEPATH "OpenGL GL library" FORCE)
	set(OPENGL_INCLUDE_DIR "${HIVEWE_MACOS_OPENGL_FRAMEWORK}" CACHE PATH "OpenGL include directory" FORCE)
endif()

set(CMAKE_INCLUDE_CURRENT_DIR ON)
set(CMAKE_CXX_EXTENSIONS OFF)

set(HIVEWE_QT_COMPONENTS Core Gui Widgets)
if(HIVEWE_BUILD_OPENGL_RENDERER)
	list(APPEND HIVEWE_QT_COMPONENTS OpenGL OpenGLWidgets)
endif()

find_package(Qt6 COMPONENTS ${HIVEWE_QT_COMPONENTS} REQUIRED)
find_package(Bullet CONFIG REQUIRED)
find_package(glm CONFIG REQUIRED)
find_package(soil2 CONFIG REQUIRED)
find_package(stormlib CONFIG REQUIRED)
find_package(BZip2 REQUIRED) # Required by Stormlib
find_package(CascLib CONFIG REQUIRED)
find_package(JPEG REQUIRED)
find_package(qtadvanceddocking-qt6 CONFIG REQUIRED)
find_package(absl CONFIG REQUIRED)
find_package(TBB CONFIG REQUIRED)
find_package(libjpeg-turbo CONFIG REQUIRED)
find_package(outcome CONFIG REQUIRED)
find_package(unordered_dense CONFIG REQUIRED)
find_package(nlohmann_json CONFIG REQUIRED)
find_package(mimalloc CONFIG REQUIRED)
find_package(unofficial-qscintilla CONFIG REQUIRED)
find_package(tracy CONFIG REQUIRED)
if(HIVEWE_BUILD_OPENGL_RENDERER)
	find_package(glad CONFIG REQUIRED)
	find_package(imgui CONFIG REQUIRED)
endif()

qt_standard_project_setup()

option(HIVEWE_BUILD_TESTS "Build HiveWE unit tests" ON)

add_library(HiveWE_lib STATIC)
add_executable(HiveWE
	${QT_RESOURCES}
)

if(APPLE)
	set_target_properties(HiveWE PROPERTIES
		MACOSX_BUNDLE TRUE
		MACOSX_BUNDLE_BUNDLE_NAME "HiveWE"
		MACOSX_BUNDLE_GUI_IDENTIFIER "org.hivewe.HiveWE"
		MACOSX_BUNDLE_SHORT_VERSION_STRING "0.11"
		MACOSX_BUNDLE_BUNDLE_VERSION "0.11.0"
	)
endif()

add_subdirectory(src)

if(APPLE)
	set(HIVEWE_CLI_TEMPLATE "${PROJECT_SOURCE_DIR}/tools/hivewe.in")

	# Keep the lowercase launcher out of directories containing the HiveWE
	# executable; default macOS filesystems are case-insensitive.
	file(MAKE_DIRECTORY "${CMAKE_CURRENT_BINARY_DIR}/cli")
	set(HIVEWE_BUILD_CLI_SCRIPT "${CMAKE_CURRENT_BINARY_DIR}/cli/hivewe")
	set(HIVEWE_CLI_APP "${CMAKE_CURRENT_BINARY_DIR}/HiveWE.app")
	configure_file("${HIVEWE_CLI_TEMPLATE}" "${HIVEWE_BUILD_CLI_SCRIPT}" @ONLY)
	file(CHMOD "${HIVEWE_BUILD_CLI_SCRIPT}"
		PERMISSIONS
			OWNER_READ OWNER_WRITE OWNER_EXECUTE
			GROUP_READ GROUP_EXECUTE
			WORLD_READ WORLD_EXECUTE
	)

	set(HIVEWE_BUNDLE_CLI_SCRIPT "${CMAKE_CURRENT_BINARY_DIR}/hivewe-bundle")
	set(HIVEWE_CLI_APP "__HIVEWE_SCRIPT_DIR__/../..")
	configure_file("${HIVEWE_CLI_TEMPLATE}" "${HIVEWE_BUNDLE_CLI_SCRIPT}" @ONLY)
	file(CHMOD "${HIVEWE_BUNDLE_CLI_SCRIPT}"
		PERMISSIONS
			OWNER_READ OWNER_WRITE OWNER_EXECUTE
			GROUP_READ GROUP_EXECUTE
			WORLD_READ WORLD_EXECUTE
	)
endif()

target_include_directories(HiveWE_lib PUBLIC
	"src"
	"src/base"
	"src/brush"
	"src/custom_widgets"
	"src/file_formats"
	"src/main_window"
	"src/menus"
	"src/models"
	"src/model_editor"
	"src/object_editor"
	"src/trigger_editor"
	"src/resources"
	"src/resources/skinned_mesh"
	"src/utilities"

	${BULLET_INCLUDE_DIR}
)

target_link_libraries(HiveWE_lib PUBLIC
	mimalloc
	Qt6::Gui
	Qt6::Widgets
	glm::glm
	StormLib::storm
	BZip2::BZip2
	JPEG::JPEG
	absl::base
	absl::strings
	$<IF:$<TARGET_EXISTS:libjpeg-turbo::turbojpeg>,libjpeg-turbo::turbojpeg,libjpeg-turbo::turbojpeg-static>
	ads::qtadvanceddocking-qt6
	TBB::tbb
	outcome::hl
	unordered_dense::unordered_dense
	nlohmann_json::nlohmann_json
	unofficial::qscintilla::qscintilla
	${BULLET_LIBRARIES}
)

target_link_libraries(HiveWE_lib PRIVATE
	soil2::soil2
)

if(HIVEWE_BUILD_OPENGL_RENDERER)
	target_link_libraries(HiveWE_lib PUBLIC
		Qt6::OpenGL
		Qt6::OpenGLWidgets
		glad::glad
		imgui::imgui
	)
	target_compile_definitions(HiveWE_lib PUBLIC HIVEWE_BUILD_OPENGL_RENDERER)
endif()

if(APPLE AND HIVEWE_ENABLE_METAL)
	find_library(APPKIT_FRAMEWORK AppKit REQUIRED)
	find_library(METAL_FRAMEWORK Metal REQUIRED)
	find_library(QUARTZCORE_FRAMEWORK QuartzCore REQUIRED)

	set_target_properties(HiveWE_lib PROPERTIES
		OBJCXX_STANDARD 23
		OBJCXX_STANDARD_REQUIRED ON
		OBJCXX_EXTENSIONS OFF
	)

	target_link_libraries(HiveWE_lib PUBLIC
		${APPKIT_FRAMEWORK}
		${METAL_FRAMEWORK}
		${QUARTZCORE_FRAMEWORK}
	)

	target_compile_definitions(HiveWE_lib PUBLIC HIVEWE_ENABLE_METAL)
endif()

target_compile_options(HiveWE_lib PUBLIC
	$<$<CXX_COMPILER_ID:MSVC>:/Zc:__cplusplus /MP /sdl /diagnostics:caret /permissive->
	$<$<CXX_COMPILER_ID:Clang>:-Wextra -Wpedantic -Werror -Wno-multichar -Wno-unused-parameter -Wno-unused-result -Wno-unused-value -Wno-sign-compare -Wno-switch -Wno-c2y-extensions -Wno-deprecated-this-capture>
	$<$<CXX_COMPILER_ID:GNU>:-Wall -Wextra -Wpedantic -Werror -Wno-multichar>
)

target_compile_definitions(HiveWE_lib PUBLIC
	$<$<CXX_COMPILER_ID:MSVC>:UNICODE _UNICODE>
	$<$<CXX_COMPILER_ID:Clang>:TBB_SUPPRESS_DEPRECATED_MESSAGES>
	$<$<CXX_COMPILER_ID:GNU>:TBB_SUPPRESS_DEPRECATED_MESSAGES>
)

if(TRACY_ENABLE)
	target_link_libraries(HiveWE_lib PUBLIC Tracy::TracyClient)
	target_compile_definitions(HiveWE_lib PUBLIC TRACY_ENABLE)
else()
	# Headers must still resolve so `#include <tracy/Tracy.hpp>` (with no-op
	# macros) compiles
	target_include_directories(HiveWE_lib PUBLIC
		$<TARGET_PROPERTY:Tracy::TracyClient,INTERFACE_INCLUDE_DIRECTORIES>)
endif()

if(TRACY_NO_EXIT)
	target_compile_definitions(HiveWE_lib PUBLIC TRACY_NO_EXIT)
endif()

if(WIN32)
	target_link_libraries(HiveWE_lib PUBLIC CascLib::casc)
else()
	target_link_libraries(HiveWE_lib PUBLIC CascLib::casc_static)
endif()

target_compile_features(HiveWE_lib PUBLIC cxx_std_23)

target_link_libraries(HiveWE PRIVATE HiveWE_lib)

target_include_directories(HiveWE PRIVATE
	"${CMAKE_CURRENT_BINARY_DIR}/HiveWE_lib_autogen/include"
)

if(APPLE)
	set_source_files_properties(
		"${CMAKE_CURRENT_BINARY_DIR}/HiveWE_lib_autogen/mocs_compilation.cpp"
		"${CMAKE_CURRENT_BINARY_DIR}/HiveWE_autogen/mocs_compilation.cpp"
		PROPERTIES CXX_SCAN_FOR_MODULES ON
	)
endif()

if(HIVEWE_BUILD_TESTS)
	enable_testing()
	add_subdirectory(tests)
endif()

if(APPLE)
	add_custom_target(HiveWE_build_provenance
		COMMAND ${CMAKE_COMMAND}
			-DHIVEWE_SOURCE_DIR=${PROJECT_SOURCE_DIR}
			-DHIVEWE_OUTPUT_FILE=$<TARGET_BUNDLE_DIR:HiveWE>/Contents/Resources/hivewe-build-provenance.json
			-DHIVEWE_BUILD_TYPE=${CMAKE_BUILD_TYPE}
			-DHIVEWE_ENABLE_METAL=$<BOOL:${HIVEWE_ENABLE_METAL}>
			-DHIVEWE_BUILD_OPENGL_RENDERER=$<BOOL:${HIVEWE_BUILD_OPENGL_RENDERER}>
			-P "${PROJECT_SOURCE_DIR}/tools/write_build_provenance.cmake"
		COMMENT "Writing HiveWE app build provenance"
		VERBATIM
	)
	add_dependencies(HiveWE HiveWE_build_provenance)

	add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
		COMMAND ${CMAKE_COMMAND}
			-E make_directory
		"$<TARGET_BUNDLE_DIR:HiveWE>/Contents/Resources"
		COMMAND ${CMAKE_COMMAND}
		-E create_symlink
		"${PROJECT_SOURCE_DIR}/data"
		"$<TARGET_BUNDLE_DIR:HiveWE>/Contents/Resources/data"
	)
	add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
		COMMAND ${CMAKE_COMMAND}
			-E copy_if_different
		"${HIVEWE_BUNDLE_CLI_SCRIPT}"
		"$<TARGET_BUNDLE_DIR:HiveWE>/Contents/Resources/hivewe"
	)
else()
	add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
		COMMAND ${CMAKE_COMMAND}
		-E create_symlink
		"${PROJECT_SOURCE_DIR}/data"
		"$<TARGET_FILE_DIR:HiveWE>/data"
	)
endif()

# vcpkg's tracy port installs the Debug DLL under debug/bin/Debug/ (extra
# subfolder), which VCPKG_APPLOCAL_DEPS doesn't pick up. Copy it manually using
# the imported target's known location.
if(TRACY_ENABLE)
	add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
		COMMAND ${CMAKE_COMMAND} -E copy_if_different
		"$<TARGET_FILE:Tracy::TracyClient>"
		"$<TARGET_FILE_DIR:HiveWE>"
	)
endif()

if(WIN32)
	if (${CMAKE_BUILD_TYPE} STREQUAL "Debug")
		set(QT_VCPKG_PATH "${CMAKE_CURRENT_BINARY_DIR}/vcpkg_installed/x64-windows/debug")
	else()
		set(QT_VCPKG_PATH "${CMAKE_CURRENT_BINARY_DIR}/vcpkg_installed/x64-windows")
	endif()

	file(COPY "${QT_VCPKG_PATH}/Qt6/plugins/platforms/" DESTINATION "${CMAKE_CURRENT_BINARY_DIR}/${CMAKE_BUILD_TYPE}/platforms")
	file(COPY "${QT_VCPKG_PATH}/Qt6/plugins/styles/" DESTINATION "${CMAKE_CURRENT_BINARY_DIR}/${CMAKE_BUILD_TYPE}/styles")
	file(COPY "${QT_VCPKG_PATH}/Qt6/plugins/imageformats/" DESTINATION "${CMAKE_CURRENT_BINARY_DIR}/${CMAKE_BUILD_TYPE}/imageformats")
endif()

if(APPLE)
	install(CODE [[
		get_filename_component(HIVEWE_INSTALLED_BUNDLE "${CMAKE_INSTALL_PREFIX}/HiveWE.app" ABSOLUTE)
		file(REMOVE_RECURSE "${HIVEWE_INSTALLED_BUNDLE}/Contents/Resources/data")
	]])
endif()

install(TARGETS HiveWE
	BUNDLE DESTINATION .
	RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR}
)

if(APPLE)
	install(PROGRAMS "${HIVEWE_BUNDLE_CLI_SCRIPT}"
		DESTINATION HiveWE.app/Contents/Resources
		RENAME hivewe
	)

	install(CODE [[
		get_filename_component(HIVEWE_INSTALLED_BUNDLE "${CMAKE_INSTALL_PREFIX}/HiveWE.app" ABSOLUTE)
		file(REMOVE_RECURSE "${HIVEWE_INSTALLED_BUNDLE}/Contents/Resources/data")
	]])

	install(DIRECTORY data/
		DESTINATION HiveWE.app/Contents/Resources/data
		PATTERN "*.j" EXCLUDE
	)
	install(FILES
		data/warcraft/scripts/common.j
		data/warcraft/scripts/blizzard.j
		DESTINATION HiveWE.app/Contents/Resources/data/warcraft/scripts
	)

	get_target_property(HIVEWE_QT_CORE_TYPE Qt6::Core TYPE)
	if(NOT HIVEWE_QT_CORE_TYPE STREQUAL "STATIC_LIBRARY")
		qt_generate_deploy_app_script(
			TARGET HiveWE
			OUTPUT_SCRIPT HIVEWE_DEPLOY_SCRIPT
			NO_TRANSLATIONS
		)
		install(SCRIPT ${HIVEWE_DEPLOY_SCRIPT})
	endif()

	install(CODE [[
		include(BundleUtilities)
		set(BU_CHMOD_BUNDLE_ITEMS ON)
		get_filename_component(HIVEWE_INSTALLED_BUNDLE "${CMAKE_INSTALL_PREFIX}/HiveWE.app" ABSOLUTE)
		fixup_bundle("${HIVEWE_INSTALLED_BUNDLE}" "" "")
	]])

	if(HIVEWE_ENABLE_METAL)
		install(CODE [[
		get_filename_component(HIVEWE_INSTALLED_BUNDLE "${CMAKE_INSTALL_PREFIX}/HiveWE.app" ABSOLUTE)
		file(GLOB HIVEWE_FORBIDDEN_BUNDLE_DYLIBS
			"${HIVEWE_INSTALLED_BUNDLE}/Contents/Frameworks/libGL*.dylib"
			"${HIVEWE_INSTALLED_BUNDLE}/Contents/Frameworks/libglapi*.dylib"
			"${HIVEWE_INSTALLED_BUNDLE}/Contents/Frameworks/libX*.dylib"
			"${HIVEWE_INSTALLED_BUNDLE}/Contents/Frameworks/libxcb*.dylib"
		)
		if(HIVEWE_FORBIDDEN_BUNDLE_DYLIBS)
			message(STATUS "Removing unused XQuartz/OpenGL dylibs from Metal app bundle: ${HIVEWE_FORBIDDEN_BUNDLE_DYLIBS}")
			file(REMOVE ${HIVEWE_FORBIDDEN_BUNDLE_DYLIBS})
		endif()

		file(GLOB HIVEWE_FORBIDDEN_BUNDLE_DYLIBS
			"${HIVEWE_INSTALLED_BUNDLE}/Contents/Frameworks/libGL*.dylib"
			"${HIVEWE_INSTALLED_BUNDLE}/Contents/Frameworks/libglapi*.dylib"
			"${HIVEWE_INSTALLED_BUNDLE}/Contents/Frameworks/libX*.dylib"
			"${HIVEWE_INSTALLED_BUNDLE}/Contents/Frameworks/libxcb*.dylib"
		)
		if(HIVEWE_FORBIDDEN_BUNDLE_DYLIBS)
			message(FATAL_ERROR "Metal app bundle still contains XQuartz/OpenGL dylibs: ${HIVEWE_FORBIDDEN_BUNDLE_DYLIBS}")
		endif()

		file(GLOB_RECURSE HIVEWE_BUNDLE_LINKABLES
			LIST_DIRECTORIES false
			"${HIVEWE_INSTALLED_BUNDLE}/Contents/MacOS/*"
			"${HIVEWE_INSTALLED_BUNDLE}/Contents/Frameworks/*.dylib"
			"${HIVEWE_INSTALLED_BUNDLE}/Contents/PlugIns/*.dylib"
			"${HIVEWE_INSTALLED_BUNDLE}/Contents/PlugIns/*.so"
		)
		foreach(HIVEWE_BUNDLE_LINKABLE IN LISTS HIVEWE_BUNDLE_LINKABLES)
			execute_process(
				COMMAND /usr/bin/otool -L "${HIVEWE_BUNDLE_LINKABLE}"
				RESULT_VARIABLE HIVEWE_OTOOL_RESULT
				OUTPUT_VARIABLE HIVEWE_OTOOL_OUTPUT
				ERROR_QUIET
			)
			if(HIVEWE_OTOOL_RESULT EQUAL 0 AND HIVEWE_OTOOL_OUTPUT MATCHES "(/opt/X11|/usr/X11|XQuartz|@executable_path/\\.\\./Frameworks/lib(GL|glapi|X|xcb)|libGL\\.|libglapi\\.|libX11|libXext|libXau|libxcb)")
				message(FATAL_ERROR "Metal app bundle links an XQuartz/OpenGL dylib from ${HIVEWE_BUNDLE_LINKABLE}:\n${HIVEWE_OTOOL_OUTPUT}")
			endif()
		endforeach()
		]])
	endif()

	install(CODE [[
		get_filename_component(HIVEWE_INSTALLED_BUNDLE "${CMAKE_INSTALL_PREFIX}/HiveWE.app" ABSOLUTE)
		execute_process(
			COMMAND /usr/bin/codesign --force --deep --sign - "${HIVEWE_INSTALLED_BUNDLE}"
			RESULT_VARIABLE HIVEWE_CODESIGN_RESULT
		)
		if(NOT HIVEWE_CODESIGN_RESULT EQUAL 0)
			message(FATAL_ERROR "codesign failed for ${HIVEWE_INSTALLED_BUNDLE}")
		endif()
	]])

	set(CPACK_PACKAGE_NAME "HiveWE")
	set(CPACK_PACKAGE_VENDOR "HiveWE")
	set(CPACK_PACKAGE_VERSION "0.11.0")
	set(CPACK_GENERATOR "DragNDrop")
	include(CPack)
endif()
