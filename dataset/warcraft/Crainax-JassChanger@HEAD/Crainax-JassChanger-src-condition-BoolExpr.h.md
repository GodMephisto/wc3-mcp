---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/condition/BoolExpr.h"
ref: HEAD
---

#pragma once

#include <string>
#include <string_view>
#include <unordered_map>
#include <unordered_set>
#include <vector>

namespace vjassc {

struct BoolEvalContext {
    std::unordered_set<std::string> libraries;
    std::unordered_map<std::string, bool> boolConstants;
    bool debugMode = false;
};

struct BoolEvalResult {
    bool ok = false;
    bool value = false;
    std::string error;
    std::vector<std::string> unknownIdentifiers;
};

BoolEvalResult evalBoolExpr(std::string_view expr, const BoolEvalContext& context);

} // namespace vjassc

