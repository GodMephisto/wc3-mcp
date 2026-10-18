---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/core/StringInterner.cpp"
ref: HEAD
---

#include "core/StringInterner.h"

namespace vjassc {

SymbolId StringInterner::intern(std::string_view text) {
    auto it = ids_.find(std::string(text));
    if (it != ids_.end()) {
        return it->second;
    }
    strings_.push_back(std::string(text));
    SymbolId id = static_cast<SymbolId>(strings_.size());
    ids_.emplace(strings_.back(), id);
    return id;
}

std::string_view StringInterner::get(SymbolId id) const {
    if (id == 0 || id > strings_.size()) {
        return {};
    }
    return strings_[id - 1];
}

} // namespace vjassc
