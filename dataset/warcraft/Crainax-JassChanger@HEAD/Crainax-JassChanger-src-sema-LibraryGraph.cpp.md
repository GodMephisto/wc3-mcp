---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/sema/LibraryGraph.cpp"
ref: HEAD
---

#include "sema/LibraryGraph.h"

namespace vjassc {

LibraryGraph::LibraryGraph(Diagnostics& diagnostics) : diagnostics_(diagnostics) {}

LibraryGraphResult LibraryGraph::sort(const Program& program) {
    byName_.clear();
    state_.clear();
    order_.clear();
    LibraryGraphResult result;
    collectLibraries(program.decls, result);
    for (const auto* library : order_) {
        visit(library, result);
    }
    return result;
}

void LibraryGraph::collectLibraries(const std::vector<Decl>& decls, LibraryGraphResult& result) {
    for (const auto& decl : decls) {
        if (decl.kind == DeclKind::Library) {
            auto it = byName_.find(decl.name);
            if (it != byName_.end()) {
                if (decl.libraryOnce) {
                    result.skippedLibraryOnceDuplicates.insert(&decl);
                    diagnostics_.warning(decl.loc, "duplicate library_once '" + decl.name + "' ignored");
                } else {
                    diagnostics_.error(decl.loc, "duplicate library '" + decl.name + "'");
                }
            } else {
                byName_[decl.name] = &decl;
                order_.push_back(&decl);
            }
        }
        collectLibraries(decl.children, result);
    }
}

void LibraryGraph::visit(const Decl* library, LibraryGraphResult& result) {
    int& mark = state_[library->name];
    if (mark == 2) {
        return;
    }
    if (mark == 1) {
        diagnostics_.error(library->loc, "cyclic library dependency involving '" + library->name + "'");
        return;
    }
    mark = 1;
    for (const auto& req : library->requirements) {
        auto it = byName_.find(req.name);
        if (it == byName_.end()) {
            if (req.optional) {
                result.missingOptionalLibraries.push_back(req.name);
            } else {
                diagnostics_.error(library->loc, "required library not found: " + req.name);
            }
            continue;
        }
        visit(it->second, result);
    }
    mark = 2;
    for (const Decl* existing : result.sortedLibraries) {
        if (existing == library) {
            return;
        }
    }
    result.sortedLibraries.push_back(library);
}

} // namespace vjassc
