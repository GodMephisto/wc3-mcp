---
type: vendor-doc
source_repo: "https://github.com/stijnherfst/HiveWE"
source_path: "src/render/vk_implementation.cpp"
ref: HEAD
---

// Single translation unit that compiles volk's function pointer table and VMA's implementation

#define VOLK_IMPLEMENTATION
#include <volk.h>

#define VMA_IMPLEMENTATION
#define VMA_STATIC_VULKAN_FUNCTIONS 0
#define VMA_DYNAMIC_VULKAN_FUNCTIONS 0
#include <vk_mem_alloc.h>
