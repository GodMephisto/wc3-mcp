---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/sema/ModuleExpander.cpp"
ref: HEAD
---

#include "sema/ModuleExpander.h"

#include <algorithm>
#include <unordered_set>

namespace vjassc {

Program ModuleExpander::expand(const Program& program, Diagnostics& diagnostics, ModuleExpansionStats& stats) const {
    Program expanded = program;
    ModuleTable table;
    table.build(expanded, diagnostics);
    expandDecls(expanded.decls, nullptr, table, diagnostics, stats);
    expanded.stats.moduleExpansions = stats.expansions;
    return expanded;
}

void ModuleExpander::expandDecls(std::vector<Decl>& decls,
                                 const Decl* container,
                                 const ModuleTable& table,
                                 Diagnostics& diagnostics,
                                 ModuleExpansionStats& stats) const {
    for (auto& decl : decls) {
        const Decl* nextContainer = (decl.kind == DeclKind::Library || decl.kind == DeclKind::Scope) ? &decl : container;
        expandDecls(decl.children, nextContainer, table, diagnostics, stats);
        if (decl.kind == DeclKind::Struct) {
            expandStruct(decl, nextContainer, table, diagnostics, stats);
        }
    }
}

void ModuleExpander::expandStruct(Decl& decl,
                                  const Decl* container,
                                  const ModuleTable& table,
                                  Diagnostics& diagnostics,
                                  ModuleExpansionStats& stats) const {
    if (decl.moduleUses.empty()) {
        return;
    }

    ExpandedMembers expandedMembers;
    std::vector<const Decl*> stack;
    std::unordered_set<const Decl*> usedModules;
    for (const auto& use : decl.moduleUses) {
        appendModuleUse(use, container, table, diagnostics, stats, stack, usedModules, expandedMembers);
    }

    std::unordered_set<std::string> fieldNames;
    std::unordered_set<std::string> methodNames;
    for (const auto& field : decl.fields) {
        fieldNames.insert(field.name);
    }
    for (const auto& method : decl.methods) {
        methodNames.insert(method.name);
    }

    std::vector<FieldDecl> fields;
    for (auto& field : expandedMembers.fields) {
        if (!fieldNames.insert(field.name).second) {
            diagnostics.error(field.loc, "duplicate field '" + field.name + "' after module expansion in struct '" + decl.name + "'");
            continue;
        }
        fields.push_back(std::move(field));
    }
    fields.insert(fields.end(), decl.fields.begin(), decl.fields.end());
    decl.fields = std::move(fields);

    std::vector<MethodDecl> methods;
    for (auto& method : expandedMembers.methods) {
        if (!methodNames.insert(method.name).second) {
            diagnostics.error(method.loc, "duplicate method '" + method.name + "' after module expansion in struct '" + decl.name + "'");
            continue;
        }
        methods.push_back(std::move(method));
    }
    methods.insert(methods.end(), decl.methods.begin(), decl.methods.end());
    decl.methods = std::move(methods);
}

ModuleExpander::ExpandedMembers ModuleExpander::collectModuleMembers(const Decl& module,
                                                                     const Decl* container,
                                                                     const ModuleTable& table,
                                                                     Diagnostics& diagnostics,
                                                                     ModuleExpansionStats& stats,
                                                                     std::vector<const Decl*>& stack) const {
    if (std::find(stack.begin(), stack.end(), &module) != stack.end()) {
        diagnostics.error(module.loc, "module expansion cycle involving '" + module.name + "'");
        return {};
    }

    stack.push_back(&module);
    ExpandedMembers out;
    std::unordered_set<const Decl*> usedModules;
    for (const auto& use : module.moduleUses) {
        appendModuleUse(use, container, table, diagnostics, stats, stack, usedModules, out);
    }

    for (const auto& field : module.fields) {
        out.fields.push_back(field);
    }
    for (const auto& method : module.methods) {
        MethodDecl clone = method;
        clone.moduleOriginName = module.name;
        if (clone.isOnInit || clone.isOnDestroy) {
            clone.name = module.name + "__" + clone.name;
        }
        out.methods.push_back(std::move(clone));
    }
    stack.pop_back();
    return out;
}

void ModuleExpander::appendModuleUse(const ModuleUseDecl& use,
                                     const Decl* container,
                                     const ModuleTable& table,
                                     Diagnostics& diagnostics,
                                     ModuleExpansionStats& stats,
                                     std::vector<const Decl*>& stack,
                                     std::unordered_set<const Decl*>& usedModules,
                                     ExpandedMembers& out) const {
    const ModuleInfo* info = table.resolve(use.name, container, diagnostics, use.loc, use.optional);
    if (!info || !info->decl) {
        if (use.optional) {
            ++stats.optionalMissing;
        }
        return;
    }
    if (!usedModules.insert(info->decl).second) {
        ++stats.duplicateUsesIgnored;
        return;
    }
    ++stats.expansions;
    ExpandedMembers members = collectModuleMembers(*info->decl, info->container, table, diagnostics, stats, stack);
    out.fields.insert(out.fields.end(), members.fields.begin(), members.fields.end());
    out.methods.insert(out.methods.end(), members.methods.begin(), members.methods.end());
}

} // namespace vjassc

