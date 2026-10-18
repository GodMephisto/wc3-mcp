---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/preprocess/TextMacro.cpp"
ref: HEAD
---

#include "preprocess/TextMacro.h"

namespace vjassc {

const char* modeName(SyntaxMode mode) {
    return mode == SyntaxMode::Zinc ? "Zinc" : "JassLike";
}

} // namespace vjassc
