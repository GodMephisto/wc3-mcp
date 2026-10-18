---
type: vendor-doc
source_repo: "https://github.com/Talv/stormex"
source_path: "include/cascfuse.hpp"
ref: HEAD
---

#pragma once

#include "storage.hpp"

int cascfs_mount(const std::string& mountPoint, HANDLE hStorage);
