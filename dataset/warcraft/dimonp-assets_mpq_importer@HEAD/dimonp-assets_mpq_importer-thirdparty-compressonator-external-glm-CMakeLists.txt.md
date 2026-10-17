---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/compressonator/external/glm/CMakeLists.txt"
ref: HEAD
---

message(STATUS "++++++++++++++++++ External-GLM" )

# -------------------------------------------------------------------------

#add_subdirectory(${PROJECT_SOURCE_DIR}/../common/lib/ext/glm ${PROJECT_SOURCE_DIR}/../common/lib/ext/glm EXCLUDE_FROM_ALL)

# -------------------------------------------------------------------------
# GLM
add_library(ExtGLM INTERFACE)


target_include_directories(ExtGLEW INTERFACE
    /usr/include/glm/
)

#target_include_directories(ExtGLEW INTERFACE
#    ${PROJECT_SOURCE_DIR}/../common/lib/ext/glm
#)


# Enable this for newer GLM libs (using older version 0.9.8.0 is been used)
# target_link_libraries(ExtGLM INTERFACE
#     glm::glm
# )
