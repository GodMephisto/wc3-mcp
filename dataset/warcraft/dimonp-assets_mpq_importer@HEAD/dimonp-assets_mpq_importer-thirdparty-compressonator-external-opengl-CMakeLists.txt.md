---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/compressonator/external/opengl/CMakeLists.txt"
ref: HEAD
---

message(STATUS "++++++++++++++++++ External-OPENGL" )

find_package(OpenGL REQUIRED)
set_target_properties(OpenGL::GL PROPERTIES IMPORTED_GLOBAL TRUE)

add_library(ExtOpenGL INTERFACE)

target_link_libraries(ExtOpenGL INTERFACE
    OpenGL::GL
)

target_include_directories(ExtOpenGL INTERFACE
    ${OPENGL_INCLUDE_DIR}
)
