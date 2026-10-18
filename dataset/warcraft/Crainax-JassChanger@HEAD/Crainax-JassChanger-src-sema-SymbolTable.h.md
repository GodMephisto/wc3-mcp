---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/sema/SymbolTable.h"
ref: HEAD
---

#pragma once

#include "parser/Ast.h"

#include <array>
#include <string>
#include <string_view>
#include <unordered_map>
#include <vector>

namespace vjassc {

struct TransparentStringHash {
    using is_transparent = void;

    size_t operator()(std::string_view value) const noexcept {
        return std::hash<std::string_view>{}(value);
    }

    size_t operator()(const std::string& value) const noexcept {
        return std::hash<std::string_view>{}(value);
    }

    size_t operator()(const char* value) const noexcept {
        return std::hash<std::string_view>{}(value);
    }
};

struct TransparentStringEqual {
    using is_transparent = void;

    bool operator()(std::string_view lhs, std::string_view rhs) const noexcept {
        return lhs == rhs;
    }
};

using ReplacementMap = std::unordered_map<std::string, std::string, TransparentStringHash, TransparentStringEqual>;

struct ScopedSymbolMap {
    ReplacementMap replacements;
    std::array<bool, 256> firstChars{};
};

class SymbolTable {
public:
    void build(const Program& program);
    const ScopedSymbolMap* symbolsFor(const Decl* container) const;
    std::string renameForDecl(const Decl& decl, const Decl* container) const;
    std::string rewriteLine(const std::string& line, const Decl* container) const;

private:
    void buildInContainer(const Decl& container, std::vector<std::string>& scopePath);
    std::string makeScopedName(const std::vector<std::string>& scopePath, std::string_view access, std::string_view name) const;
    void collectGlobalNames(const Decl& globalBlock, const std::vector<std::string>& scopePath, ScopedSymbolMap& map);

    std::unordered_map<const Decl*, ScopedSymbolMap> scoped_;
    ReplacementMap publicReplacements_;
    std::array<bool, 256> publicFirstChars_{};
};

std::string extractGlobalName(std::string line, std::string& access);
std::string stripAccessPrefixFromLine(std::string line);

} // namespace vjassc
