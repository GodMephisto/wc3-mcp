---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/preprocess/TextMacro.h"
ref: HEAD
---

#pragma once

#include "core/SourceManager.h"

#include <string>
#include <vector>

namespace vjassc {

enum class SyntaxMode {
    JassLike,
    Zinc,
};

struct LogicalLine {
    SyntaxMode mode = SyntaxMode::JassLike;
    SourceLocation loc;
    std::string text;
};

struct TextMacro {
    std::string name;
    std::vector<std::string> params;
    std::vector<LogicalLine> body;
    SourceLocation loc;
    bool once = false;
};

const char* modeName(SyntaxMode mode);

} // namespace vjassc
