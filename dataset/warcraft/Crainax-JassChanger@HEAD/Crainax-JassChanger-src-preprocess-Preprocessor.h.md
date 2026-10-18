---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/preprocess/Preprocessor.h"
ref: HEAD
---

#pragma once

#include "core/Diagnostics.h"
#include "preprocess/TextMacro.h"

#include <filesystem>
#include <string>
#include <unordered_map>
#include <unordered_set>
#include <vector>

namespace vjassc {

struct PreprocessOptions {
    bool debugMode = false;
    std::vector<std::filesystem::path> importPaths;
};

struct PreprocessStats {
    size_t zincBlocks = 0;
    size_t textmacros = 0;
    size_t runtextmacros = 0;
};

struct PreprocessResult {
    std::vector<LogicalLine> lines;
    PreprocessStats stats;
};

class Preprocessor {
public:
    Preprocessor(SourceManager& sources, Diagnostics& diagnostics, PreprocessOptions options);

    PreprocessResult run(const std::filesystem::path& inputPath);

private:
    bool processFile(const std::filesystem::path& path, SyntaxMode initialMode, bool forcedZincMode, std::vector<LogicalLine>& out);
    void handleRunTextmacro(std::string_view directive, SourceLocation loc, SyntaxMode mode, std::vector<LogicalLine>& out);
    std::filesystem::path resolveImport(const std::filesystem::path& importingFile, const std::string& importPath) const;
    bool parseTextmacroHeader(std::string_view directive, TextMacro& macro, bool once, SourceLocation loc) const;
    std::vector<std::string> parseRunTextmacroArgs(std::string_view text, std::string& name, bool& optional) const;
    std::string replaceMacroParams(std::string text, const TextMacro& macro, const std::vector<std::string>& args) const;

    SourceManager& sources_;
    Diagnostics& diagnostics_;
    PreprocessOptions options_;
    PreprocessStats stats_;
    std::unordered_map<std::string, TextMacro> macros_;
    std::unordered_set<std::string> imported_;
    std::vector<std::string> expansionStack_;
};

} // namespace vjassc
