---
type: vendor-doc
source_repo: "https://github.com/thegraydot/mpqcli"
source_path: "src/util/string.cpp"
ref: HEAD
---

#include "util/string.h"

#include <algorithm>
#include <cctype>
#include <string>

namespace mpqcli {

std::string ToLower(const std::string &str) {
    std::string result = str;
    std::transform(result.begin(), result.end(), result.begin(),
                   [](unsigned char c) { return std::tolower(c); });
    return result;
}

} // namespace mpqcli
