---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/core/SourceManager.cpp"
ref: HEAD
---

#include "core/SourceManager.h"

#include <chrono>
#include <fstream>
#include <sstream>

namespace vjassc {

std::string normalizeNewlines(std::string text) {
    std::string out;
    out.reserve(text.size());
    for (size_t i = 0; i < text.size(); ++i) {
        if (text[i] == '\r') {
            if (i + 1 < text.size() && text[i + 1] == '\n') {
                ++i;
            }
            out.push_back('\n');
        } else {
            out.push_back(text[i]);
        }
    }
    return out;
}

std::optional<uint32_t> SourceManager::loadFile(const std::filesystem::path& path, std::string& error) {
    auto start = std::chrono::steady_clock::now();
    std::ifstream in(path, std::ios::binary);
    if (!in) {
        error = "failed to open file: " + path.string();
        return std::nullopt;
    }

    std::ostringstream buffer;
    buffer << in.rdbuf();

    SourceFile file;
    file.id = static_cast<uint32_t>(files_.size() + 1);
    file.path = path;
    file.text = normalizeNewlines(buffer.str());
    file.lineOffsets.push_back(0);
    for (size_t i = 0; i < file.text.size(); ++i) {
        if (file.text[i] == '\n') {
            file.lineOffsets.push_back(static_cast<uint32_t>(i + 1));
        }
    }
    files_.push_back(std::move(file));
    readElapsedMs_ += std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::steady_clock::now() - start).count();
    return files_.back().id;
}

const SourceFile* SourceManager::getFile(uint32_t fileId) const {
    if (fileId == 0 || fileId > files_.size()) {
        return nullptr;
    }
    return &files_[fileId - 1];
}

std::string SourceManager::getLineText(uint32_t fileId, uint32_t line) const {
    const SourceFile* file = getFile(fileId);
    if (!file || line == 0 || line > file->lineOffsets.size()) {
        return {};
    }
    size_t start = file->lineOffsets[line - 1];
    size_t end = file->text.find('\n', start);
    if (end == std::string::npos) {
        end = file->text.size();
    }
    return file->text.substr(start, end - start);
}

std::string SourceManager::describe(const SourceLocation& loc) const {
    const SourceFile* file = getFile(loc.fileId);
    std::ostringstream out;
    out << (file ? file->path.string() : std::string("<unknown>")) << ':' << loc.line << ':' << loc.column;
    return out.str();
}

std::vector<std::string_view> SourceManager::lines(uint32_t fileId) const {
    std::vector<std::string_view> result;
    const SourceFile* file = getFile(fileId);
    if (!file) {
        return result;
    }
    result.reserve(file->lineOffsets.size());
    for (size_t i = 0; i < file->lineOffsets.size(); ++i) {
        size_t start = file->lineOffsets[i];
        size_t end = file->text.find('\n', start);
        if (end == std::string::npos) {
            end = file->text.size();
        }
        result.emplace_back(file->text.data() + start, end - start);
    }
    return result;
}

size_t SourceManager::totalBytes() const {
    size_t total = 0;
    for (const auto& file : files_) {
        total += file.text.size();
    }
    return total;
}

size_t SourceManager::totalLines() const {
    size_t total = 0;
    for (const auto& file : files_) {
        total += file.lineOffsets.size();
    }
    return total;
}

} // namespace vjassc
