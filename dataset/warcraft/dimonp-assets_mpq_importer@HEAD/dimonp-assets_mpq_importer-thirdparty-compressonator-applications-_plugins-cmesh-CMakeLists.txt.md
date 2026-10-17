---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/compressonator/applications/_plugins/cmesh/CMakeLists.txt"
ref: HEAD
---


add_library(Plugin_CMesh INTERFACE)

add_subdirectory(mesh_compressor)
add_subdirectory(mesh_optimizer)

target_link_libraries(Plugin_CMesh INTERFACE 
    Plugin_CMesh_Mesh_Compressor
    Plugin_CMesh_Mesh_Optimizer
)

if (OPTION_CMP_DIRECTX)
    add_subdirectory(tootle)
    target_link_libraries(Plugin_CMesh INTERFACE 
        Plugin_CMesh_Tootle
    )
endif()

#set_target_properties(Plugin_CMesh PROPERTIES FOLDER ${FOLDER_NAME})
