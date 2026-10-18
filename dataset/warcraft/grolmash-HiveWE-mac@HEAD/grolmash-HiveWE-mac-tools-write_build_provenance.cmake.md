---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "tools/write_build_provenance.cmake"
ref: HEAD
---

function(hivewe_json_escape output input)
	set(value "${input}")
	string(REPLACE "\\" "\\\\" value "${value}")
	string(REPLACE "\"" "\\\"" value "${value}")
	string(REPLACE "\n" "\\n" value "${value}")
	string(REPLACE "\r" "\\r" value "${value}")
	set(${output} "${value}" PARENT_SCOPE)
endfunction()

if(NOT DEFINED HIVEWE_SOURCE_DIR)
	message(FATAL_ERROR "HIVEWE_SOURCE_DIR is required")
endif()
if(NOT DEFINED HIVEWE_OUTPUT_FILE)
	message(FATAL_ERROR "HIVEWE_OUTPUT_FILE is required")
endif()

execute_process(
	COMMAND git -C "${HIVEWE_SOURCE_DIR}" rev-parse --show-toplevel
	RESULT_VARIABLE HIVEWE_GIT_ROOT_RESULT
	OUTPUT_VARIABLE HIVEWE_GIT_ROOT
	ERROR_QUIET
	OUTPUT_STRIP_TRAILING_WHITESPACE
)
execute_process(
	COMMAND git -C "${HIVEWE_SOURCE_DIR}" rev-parse HEAD
	RESULT_VARIABLE HIVEWE_GIT_HEAD_RESULT
	OUTPUT_VARIABLE HIVEWE_GIT_HEAD
	ERROR_QUIET
	OUTPUT_STRIP_TRAILING_WHITESPACE
)
execute_process(
	COMMAND git -C "${HIVEWE_SOURCE_DIR}" rev-parse --abbrev-ref HEAD
	RESULT_VARIABLE HIVEWE_GIT_BRANCH_RESULT
	OUTPUT_VARIABLE HIVEWE_GIT_BRANCH
	ERROR_QUIET
	OUTPUT_STRIP_TRAILING_WHITESPACE
)
execute_process(
	COMMAND git -C "${HIVEWE_SOURCE_DIR}" status --short
	RESULT_VARIABLE HIVEWE_GIT_STATUS_RESULT
	OUTPUT_VARIABLE HIVEWE_GIT_STATUS
	ERROR_QUIET
	OUTPUT_STRIP_TRAILING_WHITESPACE
)

if(NOT HIVEWE_GIT_ROOT_RESULT EQUAL 0)
	set(HIVEWE_GIT_ROOT "")
endif()
if(NOT HIVEWE_GIT_HEAD_RESULT EQUAL 0)
	set(HIVEWE_GIT_HEAD "")
endif()
if(NOT HIVEWE_GIT_BRANCH_RESULT EQUAL 0)
	set(HIVEWE_GIT_BRANCH "")
endif()
if(NOT HIVEWE_GIT_STATUS_RESULT EQUAL 0)
	set(HIVEWE_GIT_STATUS "")
endif()

if(HIVEWE_GIT_STATUS STREQUAL "")
	set(HIVEWE_GIT_DIRTY "false")
else()
	set(HIVEWE_GIT_DIRTY "true")
endif()

hivewe_json_escape(HIVEWE_GIT_ROOT_JSON "${HIVEWE_GIT_ROOT}")
hivewe_json_escape(HIVEWE_GIT_HEAD_JSON "${HIVEWE_GIT_HEAD}")
hivewe_json_escape(HIVEWE_GIT_BRANCH_JSON "${HIVEWE_GIT_BRANCH}")
hivewe_json_escape(HIVEWE_GIT_STATUS_JSON "${HIVEWE_GIT_STATUS}")
hivewe_json_escape(HIVEWE_BUILD_TYPE_JSON "${HIVEWE_BUILD_TYPE}")
hivewe_json_escape(HIVEWE_ENABLE_METAL_JSON "${HIVEWE_ENABLE_METAL}")
hivewe_json_escape(HIVEWE_BUILD_OPENGL_RENDERER_JSON "${HIVEWE_BUILD_OPENGL_RENDERER}")

get_filename_component(HIVEWE_OUTPUT_DIR "${HIVEWE_OUTPUT_FILE}" DIRECTORY)
file(MAKE_DIRECTORY "${HIVEWE_OUTPUT_DIR}")
file(WRITE "${HIVEWE_OUTPUT_FILE}" "{\n")
file(APPEND "${HIVEWE_OUTPUT_FILE}" "  \"schema\": 1,\n")
file(APPEND "${HIVEWE_OUTPUT_FILE}" "  \"label\": \"hivewe-app-build-provenance\",\n")
file(APPEND "${HIVEWE_OUTPUT_FILE}" "  \"source_revision\": {\n")
file(APPEND "${HIVEWE_OUTPUT_FILE}" "    \"root\": \"${HIVEWE_GIT_ROOT_JSON}\",\n")
file(APPEND "${HIVEWE_OUTPUT_FILE}" "    \"head\": \"${HIVEWE_GIT_HEAD_JSON}\",\n")
file(APPEND "${HIVEWE_OUTPUT_FILE}" "    \"branch\": \"${HIVEWE_GIT_BRANCH_JSON}\",\n")
file(APPEND "${HIVEWE_OUTPUT_FILE}" "    \"dirty\": ${HIVEWE_GIT_DIRTY},\n")
file(APPEND "${HIVEWE_OUTPUT_FILE}" "    \"status_short\": \"${HIVEWE_GIT_STATUS_JSON}\"\n")
file(APPEND "${HIVEWE_OUTPUT_FILE}" "  },\n")
file(APPEND "${HIVEWE_OUTPUT_FILE}" "  \"cmake\": {\n")
file(APPEND "${HIVEWE_OUTPUT_FILE}" "    \"build_type\": \"${HIVEWE_BUILD_TYPE_JSON}\",\n")
file(APPEND "${HIVEWE_OUTPUT_FILE}" "    \"enable_metal\": \"${HIVEWE_ENABLE_METAL_JSON}\",\n")
file(APPEND "${HIVEWE_OUTPUT_FILE}" "    \"build_opengl_renderer\": \"${HIVEWE_BUILD_OPENGL_RENDERER_JSON}\"\n")
file(APPEND "${HIVEWE_OUTPUT_FILE}" "  }\n")
file(APPEND "${HIVEWE_OUTPUT_FILE}" "}\n")
