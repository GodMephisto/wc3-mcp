---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/condition/StaticIfPruner.h"
ref: HEAD
---

#pragma once

#include "condition/BoolExpr.h"
#include "core/Diagnostics.h"
#include "preprocess/TextMacro.h"

#include <vector>

namespace vjassc {

struct StaticIfSymbols {
    std::unordered_set<std::string> libraries;
    std::unordered_map<std::string, bool> boolConstants;
    bool debugMode = false;
};

struct StaticIfStats {
    size_t staticIfs = 0;
    size_t resolvedTrue = 0;
    size_t resolvedFalse = 0;
    size_t prunedLines = 0;
};

struct StaticIfPruneResult {
    std::vector<LogicalLine> lines;
    StaticIfStats stats;
};

class StaticIfSymbolCollector {
public:
    StaticIfSymbols collect(const std::vector<LogicalLine>& lines, bool debugMode, Diagnostics& diagnostics) const;
};

class StaticIfPruner {
public:
    StaticIfPruneResult prune(const std::vector<LogicalLine>& lines,
                              const StaticIfSymbols& symbols,
                              Diagnostics& diagnostics) const;
};

} // namespace vjassc

