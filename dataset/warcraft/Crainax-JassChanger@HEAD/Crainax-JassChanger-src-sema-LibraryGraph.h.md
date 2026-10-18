---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/sema/LibraryGraph.h"
ref: HEAD
---

#pragma once

#include "core/Diagnostics.h"
#include "parser/Ast.h"

#include <string>
#include <unordered_map>
#include <unordered_set>
#include <vector>

namespace vjassc {

struct LibraryGraphResult {
    std::vector<const Decl*> sortedLibraries;
    std::unordered_set<const Decl*> skippedLibraryOnceDuplicates;
    std::vector<std::string> missingOptionalLibraries;
};

class LibraryGraph {
public:
    LibraryGraph(Diagnostics& diagnostics);
    LibraryGraphResult sort(const Program& program);

private:
    void collectLibraries(const std::vector<Decl>& decls, LibraryGraphResult& result);
    void visit(const Decl* library, LibraryGraphResult& result);

    Diagnostics& diagnostics_;
    std::unordered_map<std::string, const Decl*> byName_;
    std::unordered_map<std::string, int> state_;
    std::vector<const Decl*> order_;
};

} // namespace vjassc
