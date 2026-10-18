---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/runtime_config.h"
ref: HEAD
---

#pragma once

#if defined(__APPLE__) && defined(HIVEWE_ENABLE_METAL)
#define HIVEWE_USE_METAL_RUNTIME 1
#else
#define HIVEWE_USE_METAL_RUNTIME 0
#endif
