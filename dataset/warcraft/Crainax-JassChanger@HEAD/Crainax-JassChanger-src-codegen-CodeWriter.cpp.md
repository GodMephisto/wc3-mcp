---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/codegen/CodeWriter.cpp"
ref: HEAD
---

#include "codegen/CodeWriter.h"

#include <utility>

namespace vjassc {

FastCodeWriter::FastCodeWriter(size_t reserveBytes) {
    indentCache_[0] = "";
    for (size_t i = 1; i < indentCache_.size(); ++i) {
        indentCache_[i] = indentCache_[i - 1] + "    ";
    }
    reserve(reserveBytes);
}

void FastCodeWriter::reserve(size_t bytes) {
    out_.reserve(bytes);
}

void FastCodeWriter::clear() {
    out_.clear();
    indent_ = 0;
    lineCount_ = 0;
}

std::string_view FastCodeWriter::indentText() const {
    if (indent_ <= 0) {
        return {};
    }
    if (static_cast<size_t>(indent_) < indentCache_.size()) {
        return indentCache_[static_cast<size_t>(indent_)];
    }
    return indentCache_.back();
}

void FastCodeWriter::writeln(std::string_view line) {
    if (indent_ > 0) {
        std::string_view cached = indentText();
        out_.append(cached.data(), cached.size());
        for (int i = static_cast<int>(indentCache_.size()); i <= indent_; ++i) {
            out_.append("    ");
        }
    }
    out_.append(line.data(), line.size());
    out_.push_back('\n');
    ++lineCount_;
}

void FastCodeWriter::writeRaw(std::string_view text) {
    out_.append(text.data(), text.size());
}

void FastCodeWriter::indent() {
    ++indent_;
}

void FastCodeWriter::dedent() {
    if (indent_ > 0) {
        --indent_;
    }
}

size_t FastCodeWriter::lineCount() const {
    return lineCount_;
}

std::string FastCodeWriter::str() const {
    return out_;
}

std::string FastCodeWriter::take() {
    indent_ = 0;
    return std::move(out_);
}

} // namespace vjassc
