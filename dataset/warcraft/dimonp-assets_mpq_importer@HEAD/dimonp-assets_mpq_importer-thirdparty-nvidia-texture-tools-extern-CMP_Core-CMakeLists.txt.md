---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/nvidia-texture-tools/extern/CMP_Core/CMakeLists.txt"
ref: HEAD
---

add_library(NVTT_CMP_Core STATIC "")

target_sources(NVTT_CMP_Core
               PRIVATE
                   shaders/BC1_Encode_kernel.h
                   shaders/BC1_Encode_kernel.cpp
                   shaders/BC2_Encode_kernel.h
                   shaders/BC2_Encode_kernel.cpp
                   shaders/BC3_Encode_kernel.h
                   shaders/BC3_Encode_kernel.cpp
                   shaders/BC4_Encode_kernel.h
                   shaders/BC4_Encode_kernel.cpp
                   shaders/BC5_Encode_kernel.h
                   shaders/BC5_Encode_kernel.cpp
                   shaders/BC6_Encode_kernel.h
                   shaders/BC6_Encode_kernel.cpp
                   shaders/BC7_Encode_Kernel.h
                   shaders/BC7_Encode_Kernel.cpp
                   shaders/BCn_Common_Kernel.h
                   shaders/Common_Def.h
                   )

target_include_directories(NVTT_CMP_Core
                           PRIVATE
                           shaders
                           source)
#add_subdirectory(test)

if (UNIX)
target_compile_definitions(NVTT_CMP_Core PRIVATE _LINUX ASPM_GPU)
endif()
