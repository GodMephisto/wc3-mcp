---
type: vendor-doc
source_repo: "https://github.com/thegraydot/mpqcli"
source_path: "src/mpq/flags.h"
ref: HEAD
---

#ifndef MPQ_FLAGS_H
#define MPQ_FLAGS_H

#include <cstdint>
#include <string>

namespace mpqcli {

std::string GetFlagString(uint32_t flags);

} // namespace mpqcli

#endif
