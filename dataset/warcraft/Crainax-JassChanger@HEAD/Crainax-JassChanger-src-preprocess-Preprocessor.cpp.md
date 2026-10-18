---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/preprocess/Preprocessor.cpp"
ref: HEAD
---

#include "preprocess/Preprocessor.h"

#include "util/PathUtil.h"

#include <algorithm>
#include <filesystem>
#include <regex>
#include <sstream>

namespace vjassc {
namespace {

bool startsDirective(const std::string& trimmed, const std::string& name) {
    if (trimmed.rfind("//!", 0) != 0) {
        return false;
    }
    return startsWithWord(trim(trimmed.substr(3)), name);
}

std::string directiveBody(const std::string& trimmed) {
    if (trimmed.rfind("//!", 0) != 0) {
        return {};
    }
    return trim(trimmed.substr(3));
}

bool parseQuotedPath(std::string_view text, std::string& path) {
    size_t first = text.find('"');
    if (first == std::string_view::npos) {
        return false;
    }
    std::string value;
    bool escaped = false;
    for (size_t i = first + 1; i < text.size(); ++i) {
        char c = text[i];
        if (escaped) {
            value.push_back(c);
            escaped = false;
        } else if (c == '\\') {
            escaped = true;
        } else if (c == '"') {
            path = value;
            return true;
        } else {
            value.push_back(c);
        }
    }
    return false;
}

std::string firstWord(std::string_view text) {
    std::istringstream in{std::string(trim(text))};
    std::string word;
    in >> word;
    return word;
}

bool lineStartsDebug(const std::string& line, size_t& prefixStart, size_t& prefixEnd) {
    size_t i = 0;
    while (i < line.size() && std::isspace(static_cast<unsigned char>(line[i]))) {
        ++i;
    }
    prefixStart = i;
    std::string_view rest(line.data() + i, line.size() - i);
    if (!startsWithWord(rest, "debug")) {
        return false;
    }
    prefixEnd = i + 5;
    if (prefixEnd < line.size() && line[prefixEnd] == ' ') {
        ++prefixEnd;
    }
    return true;
}

std::string stripBlockCommentsPreservingLiterals(const std::string& line, bool& inBlockComment) {
    std::string out;
    out.reserve(line.size());
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    for (size_t i = 0; i < line.size();) {
        if (inBlockComment) {
            if (i + 1 < line.size() && line[i] == '*' && line[i + 1] == '/') {
                out.push_back(' ');
                out.push_back(' ');
                i += 2;
                inBlockComment = false;
            } else {
                out.push_back(' ');
                ++i;
            }
            continue;
        }

        if (!inString && !inRaw && i + 1 < line.size() && line[i] == '/' && line[i + 1] == '/') {
            out.append(line.substr(i));
            break;
        }

        char c = line[i];
        if (inString) {
            out.push_back(c);
            if (escaped) {
                escaped = false;
            } else if (c == '\\') {
                escaped = true;
            } else if (c == '"') {
                inString = false;
            }
            ++i;
            continue;
        }
        if (inRaw) {
            out.push_back(c);
            if (c == '\'') {
                inRaw = false;
            }
            ++i;
            continue;
        }
        if (c == '"') {
            inString = true;
            out.push_back(c);
            ++i;
            continue;
        }
        if (c == '\'') {
            inRaw = true;
            out.push_back(c);
            ++i;
            continue;
        }
        if (i + 1 < line.size() && c == '/' && line[i + 1] == '*') {
            out.push_back(' ');
            out.push_back(' ');
            i += 2;
            inBlockComment = true;
            continue;
        }
        out.push_back(c);
        ++i;
    }
    return out;
}

std::vector<std::string> parseTakesParams(std::string_view text) {
    std::string body = trim(text);
    size_t takes = body.find("takes");
    if (takes == std::string::npos) {
        return {};
    }
    std::string after = trim(std::string_view(body).substr(takes + 5));
    return splitCommaListRespectingQuotes(after);
}

} // namespace

Preprocessor::Preprocessor(SourceManager& sources, Diagnostics& diagnostics, PreprocessOptions options)
    : sources_(sources), diagnostics_(diagnostics), options_(std::move(options)) {}

PreprocessResult Preprocessor::run(const std::filesystem::path& inputPath) {
    PreprocessResult result;
    processFile(inputPath, SyntaxMode::JassLike, false, result.lines);
    result.stats = stats_;
    return result;
}

bool Preprocessor::processFile(const std::filesystem::path& path, SyntaxMode initialMode, bool forcedZincMode, std::vector<LogicalLine>& out) {
    auto canonical = weakCanonical(path);
    std::string canonicalKey = pathToGenericString(canonical);
    if (imported_.contains(canonicalKey)) {
        return true;
    }
    imported_.insert(canonicalKey);

    std::string error;
    auto fileId = sources_.loadFile(canonical, error);
    if (!fileId) {
        diagnostics_.error(SourceLocation{}, error);
        return false;
    }

    SyntaxMode mode = initialMode;
    bool zincOpenedByDirective = false;
    bool inNovjass = false;
    bool inMacro = false;
    bool inBlockComment = false;
    TextMacro currentMacro;

    const auto fileLines = sources_.lines(*fileId);
    for (size_t lineIndex = 0; lineIndex < fileLines.size(); ++lineIndex) {
        std::string line = stripBlockCommentsPreservingLiterals(std::string(fileLines[lineIndex]), inBlockComment);
        SourceLocation loc{*fileId, static_cast<uint32_t>(lineIndex + 1), 1, 0};
        std::string left = ltrim(line);
        std::string directive = directiveBody(left);

        if (inNovjass) {
            if (startsDirective(left, "novjass")) {
                diagnostics_.warning(loc, "nested novjass block is ignored");
                continue;
            }
            if (startsDirective(left, "endnovjass")) {
                inNovjass = false;
            }
            continue;
        }

        if (inMacro) {
            if (startsDirective(left, "endtextmacro")) {
                auto it = macros_.find(currentMacro.name);
                if (it != macros_.end()) {
                    if (currentMacro.once || it->second.once) {
                        diagnostics_.warning(loc, "duplicate textmacro_once '" + currentMacro.name + "' ignored");
                    } else {
                        diagnostics_.error(loc, "duplicate textmacro '" + currentMacro.name + "'");
                    }
                } else {
                    macros_.emplace(currentMacro.name, currentMacro);
                    ++stats_.textmacros;
                }
                currentMacro = TextMacro{};
                inMacro = false;
                continue;
            }
            if (startsDirective(left, "textmacro") || startsDirective(left, "textmacro_once")) {
                diagnostics_.error(loc, "nested textmacro declaration is not supported");
                continue;
            }
            currentMacro.body.push_back(LogicalLine{mode, loc, line});
            continue;
        }

        if (left.rfind("//!", 0) == 0) {
            if (startsWithWord(directive, "novjass")) {
                inNovjass = true;
                continue;
            }
            if (startsWithWord(directive, "endnovjass")) {
                diagnostics_.warning(loc, "endnovjass without matching novjass");
                continue;
            }
            if (startsWithWord(directive, "zinc")) {
                if (mode == SyntaxMode::Zinc) {
                    diagnostics_.warning(loc, "nested zinc block");
                }
                mode = SyntaxMode::Zinc;
                zincOpenedByDirective = true;
                ++stats_.zincBlocks;
                out.push_back(LogicalLine{SyntaxMode::JassLike, loc, "// [mode: zinc begin]"});
                continue;
            }
            if (startsWithWord(directive, "endzinc")) {
                if (mode != SyntaxMode::Zinc) {
                    diagnostics_.warning(loc, "endzinc without matching zinc");
                }
                mode = SyntaxMode::JassLike;
                zincOpenedByDirective = false;
                out.push_back(LogicalLine{SyntaxMode::JassLike, loc, "// [mode: zinc end]"});
                continue;
            }
            if (startsWithWord(directive, "import")) {
                bool importZinc = false;
                std::string importDirective = trim(std::string_view(directive).substr(6));
                if (startsWithWord(importDirective, "zinc")) {
                    importZinc = true;
                    importDirective = trim(std::string_view(importDirective).substr(4));
                }
                std::string importedPath;
                if (!parseQuotedPath(importDirective, importedPath)) {
                    diagnostics_.error(loc, "malformed import directive");
                    continue;
                }
                auto resolved = resolveImport(canonical.parent_path(), importedPath);
                if (resolved.empty()) {
                    diagnostics_.error(loc, "import not found: " + importedPath);
                    continue;
                }
                processFile(resolved, importZinc ? SyntaxMode::Zinc : mode, importZinc, out);
                continue;
            }
            if (startsWithWord(directive, "textmacro_once")) {
                TextMacro macro;
                if (parseTextmacroHeader(std::string_view(directive).substr(14), macro, true, loc)) {
                    currentMacro = std::move(macro);
                    inMacro = true;
                }
                continue;
            }
            if (startsWithWord(directive, "textmacro")) {
                TextMacro macro;
                if (parseTextmacroHeader(std::string_view(directive).substr(9), macro, false, loc)) {
                    currentMacro = std::move(macro);
                    inMacro = true;
                }
                continue;
            }
            if (startsWithWord(directive, "runtextmacro")) {
                handleRunTextmacro(std::string_view(directive).substr(12), loc, mode, out);
                continue;
            }
            if (startsWithWord(directive, "external") || startsWithWord(directive, "externalblock") ||
                startsWithWord(directive, "loaddata")) {
                std::string feature = firstWord(directive);
                diagnostics_.unsupported(loc, feature);
                continue;
            }
        }

        size_t debugPrefixStart = 0;
        size_t debugPrefixEnd = 0;
        if (lineStartsDebug(line, debugPrefixStart, debugPrefixEnd)) {
            if (!options_.debugMode) {
                continue;
            }
            line.erase(debugPrefixStart, debugPrefixEnd - debugPrefixStart);
        }

        out.push_back(LogicalLine{mode, loc, line});
    }

    if (inNovjass) {
        diagnostics_.error(SourceLocation{*fileId, static_cast<uint32_t>(fileLines.size()), 1, 0}, "missing endnovjass");
    }
    if (inMacro) {
        diagnostics_.error(currentMacro.loc, "missing endtextmacro for '" + currentMacro.name + "'");
    }
    if (zincOpenedByDirective && !forcedZincMode) {
        diagnostics_.error(SourceLocation{*fileId, static_cast<uint32_t>(fileLines.size()), 1, 0}, "missing endzinc");
    }

    return true;
}

void Preprocessor::handleRunTextmacro(std::string_view directive, SourceLocation loc, SyntaxMode mode, std::vector<LogicalLine>& out) {
    ++stats_.runtextmacros;
    std::string name;
    bool optional = false;
    auto args = parseRunTextmacroArgs(directive, name, optional);
    if (name.empty()) {
        diagnostics_.error(loc, "malformed runtextmacro");
        return;
    }
    auto it = macros_.find(name);
    if (it == macros_.end()) {
        if (!optional) {
            diagnostics_.error(loc, "unknown textmacro '" + name + "'");
        }
        return;
    }
    if (std::find(expansionStack_.begin(), expansionStack_.end(), name) != expansionStack_.end()) {
        diagnostics_.error(loc, "recursive textmacro expansion for '" + name + "'");
        return;
    }
    const TextMacro& macro = it->second;
    if (args.size() != macro.params.size()) {
        diagnostics_.error(loc, "textmacro '" + name + "' expected " + std::to_string(macro.params.size()) +
                                    " arguments, got " + std::to_string(args.size()));
        return;
    }

    expansionStack_.push_back(name);
    for (const auto& bodyLine : macro.body) {
        out.push_back(LogicalLine{bodyLine.mode == SyntaxMode::Zinc ? SyntaxMode::Zinc : mode,
                                  loc,
                                  replaceMacroParams(bodyLine.text, macro, args)});
    }
    expansionStack_.pop_back();
}

std::filesystem::path Preprocessor::resolveImport(const std::filesystem::path& importingDir, const std::string& importPath) const {
    std::vector<std::filesystem::path> candidates;
    std::filesystem::path requested(importPath);
    if (requested.is_absolute()) {
        candidates.push_back(requested);
    } else {
        candidates.push_back(importingDir / requested);
        for (const auto& dir : options_.importPaths) {
            candidates.push_back(dir / requested);
        }
        candidates.push_back(std::filesystem::current_path() / requested);
    }
    for (const auto& candidate : candidates) {
        std::error_code ec;
        if (std::filesystem::exists(candidate, ec)) {
            return weakCanonical(candidate);
        }
    }
    return {};
}

bool Preprocessor::parseTextmacroHeader(std::string_view directive, TextMacro& macro, bool once, SourceLocation loc) const {
    std::string body = trim(directive);
    std::istringstream in(body);
    if (!(in >> macro.name)) {
        diagnostics_.error(loc, "textmacro missing name");
        return false;
    }
    macro.once = once;
    macro.loc = loc;
    size_t takes = body.find("takes");
    if (takes != std::string::npos) {
        macro.params = parseTakesParams(body);
    }
    return true;
}

std::vector<std::string> Preprocessor::parseRunTextmacroArgs(std::string_view text, std::string& name, bool& optional) const {
    std::string body = trim(text);
    if (startsWithWord(body, "optional")) {
        optional = true;
        body = trim(std::string_view(body).substr(8));
    }
    size_t open = body.find('(');
    size_t close = body.rfind(')');
    if (open == std::string::npos || close == std::string::npos || close < open) {
        name = trim(body);
        return {};
    }
    name = trim(std::string_view(body).substr(0, open));
    std::string argsText = std::string(std::string_view(body).substr(open + 1, close - open - 1));
    if (trim(argsText).empty()) {
        return {};
    }
    auto args = splitCommaListRespectingQuotes(argsText);
    for (auto& arg : args) {
        arg = stripOuterQuotes(arg);
    }
    return args;
}

std::string Preprocessor::replaceMacroParams(std::string text, const TextMacro& macro, const std::vector<std::string>& args) const {
    for (size_t i = 0; i < macro.params.size(); ++i) {
        std::string needle = "$" + macro.params[i] + "$";
        size_t pos = 0;
        while ((pos = text.find(needle, pos)) != std::string::npos) {
            text.replace(pos, needle.size(), args[i]);
            pos += args[i].size();
        }
    }
    return text;
}

} // namespace vjassc
