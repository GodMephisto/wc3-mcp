---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/compressonator/applications/_libs/cmp_math/CMakeLists.txt"
ref: HEAD
---


add_library(CMP_Math)

if(CMP_HOST_WINDOWS)
    target_compile_definitions(CMP_Math PUBLIC
        -DCMP_USE_XMMINTRIN
        )
endif()

target_sources(CMP_Math PRIVATE

    cmp_math_common.cpp
    cmp_math_common.h
    cpu_extensions.cpp
    cpu_extensions.h
    jml.h
    jmlfuncs.cpp
    jmlfuncs.h
    jmlmatrix.h
    jmlscalar.h
    jmlssevec.h
    jmlvec2.h
    jmlvec3.h
    jrtcommon.h
    ${PROJECT_SOURCE_DIR}/cmp_framework/common/mathmacros.h
    tootlepch.h
)

target_include_directories(CMP_Math PUBLIC
    ./
    ${PROJECT_SOURCE_DIR}/cmp_core/shaders
    ${PROJECT_SOURCE_DIR}/cmp_core/source
)

target_link_libraries(CMP_Math PRIVATE
 #   CMP_Core
)

set_target_properties(CMP_Math PROPERTIES 
    FOLDER ${PROJECT_FOLDER_SDK_LIBS}
)
