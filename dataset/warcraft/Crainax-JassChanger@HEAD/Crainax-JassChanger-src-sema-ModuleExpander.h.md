---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/sema/ModuleExpander.h"
ref: HEAD
---

#pragma once

#include "core/Diagnostics.h"
#include "parser/Ast.h"
#include "sema/ModuleTable.h"

#include <unordered_set>
#include <vector>

namespace vjassc {

struct ModuleExpansionStats {
    size_t expansions = 0;
    size_t optionalMissing = 0;
    size_t duplicateUsesIgnored = 0;
};

class ModuleExpander {
public:
    Program expand(const Program& program, Diagnostics& diagnostics, ModuleExpansionStats& stats) const;

private:
    struct ExpandedMembers {
        std::vector<FieldDecl> fields;
        std::vector<MethodDecl> methods;
    };

    void expandDecls(std::vector<Decl>& decls, const Decl* container, const ModuleTable& table, Diagnostics& diagnostics, ModuleExpansionStats& stats) const;
    void expandStruct(Decl& decl, const Decl* container, const ModuleTable& table, Diagnostics& diagnostics, ModuleExpansionStats& stats) const;
    ExpandedMembers collectModuleMembers(const Decl& module,
                                         const Decl* container,
                                         const ModuleTable& table,
                                         Diagnostics& diagnostics,
                                         ModuleExpansionStats& stats,
                                         std::vector<const Decl*>& stack) const;
    void appendModuleUse(const ModuleUseDecl& use,
                         const Decl* container,
                         const ModuleTable& table,
                         Diagnostics& diagnostics,
                         ModuleExpansionStats& stats,
                         std::vector<const Decl*>& stack,
                         std::unordered_set<const Decl*>& usedModules,
                         ExpandedMembers& out) const;
};

} // namespace vjassc

