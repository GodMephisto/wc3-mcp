---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/core/StringInterner.h"
ref: HEAD
---

#pragma once

#include <cstdint>
#include <string>
#include <string_view>
#include <unordered_map>
#include <vector>

namespace vjassc {

using SymbolId = uint32_t;

class StringInterner {
public:
    SymbolId intern(std::string_view text);
    std::string_view get(SymbolId id) const;

private:
    std::vector<std::string> strings_;
    std::unordered_map<std::string, SymbolId> ids_;
};

} // namespace vjassc
