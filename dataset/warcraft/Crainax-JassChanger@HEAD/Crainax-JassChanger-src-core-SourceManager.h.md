---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/core/SourceManager.h"
ref: HEAD
---

#pragma once

#include <cstdint>
#include <filesystem>
#include <optional>
#include <string>
#include <string_view>
#include <vector>

namespace vjassc {

struct SourceLocation {
    uint32_t fileId = 0;
    uint32_t line = 1;
    uint32_t column = 1;
    uint32_t offset = 0;
};

struct SourceFile {
    uint32_t id = 0;
    std::filesystem::path path;
    std::string text;
    std::vector<uint32_t> lineOffsets;
};

class SourceManager {
public:
    std::optional<uint32_t> loadFile(const std::filesystem::path& path, std::string& error);
    const SourceFile* getFile(uint32_t fileId) const;
    std::string getLineText(uint32_t fileId, uint32_t line) const;
    std::string describe(const SourceLocation& loc) const;
    std::vector<std::string_view> lines(uint32_t fileId) const;

    size_t fileCount() const { return files_.size(); }
    size_t totalBytes() const;
    size_t totalLines() const;
    long long readElapsedMs() const { return readElapsedMs_; }

private:
    std::vector<SourceFile> files_;
    long long readElapsedMs_ = 0;
};

std::string normalizeNewlines(std::string text);

} // namespace vjassc
