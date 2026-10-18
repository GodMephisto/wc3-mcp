---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/sema/ModuleTable.cpp"
ref: HEAD
---

#include "sema/ModuleTable.h"

namespace vjassc {
namespace {

std::string joinScope(const std::vector<std::string>& scopePath) {
    std::string out;
    for (size_t i = 0; i < scopePath.size(); ++i) {
        if (i > 0) {
            out += "_";
        }
        out += scopePath[i];
    }
    return out;
}

bool visibleFrom(const ModuleInfo& info, const Decl* currentContainer) {
    if (info.container == currentContainer) {
        return true;
    }
    if (info.container == nullptr) {
        return true;
    }
    return info.access != "private";
}

} // namespace

void ModuleTable::build(const Program& program, Diagnostics& diagnostics) {
    modules_.clear();
    byName_.clear();
    qualified_.clear();
    std::vector<std::string> scopePath;
    collect(program.decls, nullptr, scopePath, diagnostics);
}

void ModuleTable::collect(const std::vector<Decl>& decls,
                          const Decl* container,
                          std::vector<std::string>& scopePath,
                          Diagnostics& diagnostics) {
    for (const auto& decl : decls) {
        const Decl* nextContainer = container;
        bool pushed = false;
        if (decl.kind == DeclKind::Library || decl.kind == DeclKind::Scope) {
            scopePath.push_back(decl.name);
            pushed = true;
            nextContainer = &decl;
        }
        if (decl.kind == DeclKind::Module) {
            std::string qualified = joinScope(scopePath);
            if (!qualified.empty()) {
                qualified += "_";
            }
            qualified += decl.name;
            if (!qualified_.insert(qualified).second) {
                diagnostics.warning(decl.loc, "duplicate module '" + qualified + "' ignored");
            } else {
                size_t index = modules_.size();
                modules_.push_back(ModuleInfo{&decl, container, decl.name, decl.access, qualified, decl.loc});
                byName_[decl.name].push_back(index);
            }
        }
        collect(decl.children, nextContainer, scopePath, diagnostics);
        if (pushed) {
            scopePath.pop_back();
        }
    }
}

const ModuleInfo* ModuleTable::resolve(std::string_view name,
                                       const Decl* currentContainer,
                                       Diagnostics& diagnostics,
                                       SourceLocation loc,
                                       bool optional) const {
    auto it = byName_.find(std::string(name));
    if (it == byName_.end()) {
        if (!optional) {
            diagnostics.error(loc, "required module not found: " + std::string(name));
        }
        return nullptr;
    }

    const ModuleInfo* match = nullptr;
    for (size_t index : it->second) {
        const ModuleInfo& candidate = modules_[index];
        if (!visibleFrom(candidate, currentContainer)) {
            continue;
        }
        if (match != nullptr) {
            diagnostics.error(loc, "ambiguous module reference: " + std::string(name));
            return nullptr;
        }
        match = &candidate;
    }
    if (!match && !optional) {
        diagnostics.error(loc, "required module not visible: " + std::string(name));
    }
    return match;
}

} // namespace vjassc

