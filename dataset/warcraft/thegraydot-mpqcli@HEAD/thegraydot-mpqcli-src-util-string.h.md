---
type: vendor-doc
source_repo: "https://github.com/thegraydot/mpqcli"
source_path: "src/util/string.h"
ref: HEAD
---

#ifndef UTIL_STRING_H
#define UTIL_STRING_H

#include <string>

namespace mpqcli {

/// Returns str with every ASCII letter lowercased
std::string ToLower(const std::string &str);

} // namespace mpqcli

#endif
