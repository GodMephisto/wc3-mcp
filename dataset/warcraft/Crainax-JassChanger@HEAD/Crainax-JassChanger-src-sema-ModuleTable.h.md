---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/sema/ModuleTable.h"
ref: HEAD
---

#pragma once

#include "core/Diagnostics.h"
#include "parser/Ast.h"

#include <string>
#include <string_view>
#include <unordered_map>
#include <unordered_set>
#include <vector>

namespace vjassc {

struct ModuleInfo {
    const Decl* decl = nullptr;
    const Decl* container = nullptr;
    std::string name;
    std::string access;
    std::string qualifiedName;
    SourceLocation loc;
};

class ModuleTable {
public:
    void build(const Program& program, Diagnostics& diagnostics);
    const ModuleInfo* resolve(std::string_view name,
                              const Decl* currentContainer,
                              Diagnostics& diagnostics,
                              SourceLocation loc,
                              bool optional) const;

private:
    void collect(const std::vector<Decl>& decls, const Decl* container, std::vector<std::string>& scopePath, Diagnostics& diagnostics);

    std::vector<ModuleInfo> modules_;
    std::unordered_map<std::string, std::vector<size_t>> byName_;
    std::unordered_set<std::string> qualified_;
};

} // namespace vjassc

