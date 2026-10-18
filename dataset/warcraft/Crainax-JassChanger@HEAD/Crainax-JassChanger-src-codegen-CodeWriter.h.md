---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/codegen/CodeWriter.h"
ref: HEAD
---

#pragma once

#include <array>
#include <string>
#include <string_view>

namespace vjassc {

class FastCodeWriter {
public:
    explicit FastCodeWriter(size_t reserveBytes = 0);

    void reserve(size_t bytes);
    void clear();
    void writeln(std::string_view line = {});
    void writeRaw(std::string_view text);
    void indent();
    void dedent();
    size_t lineCount() const;
    std::string str() const;
    std::string take();

private:
    std::string_view indentText() const;

    std::string out_;
    int indent_ = 0;
    size_t lineCount_ = 0;
    std::array<std::string, 16> indentCache_;
};

using CodeWriter = FastCodeWriter;

} // namespace vjassc
