---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/codegen/Phase1Codegen.cpp"
ref: HEAD
---

#include "codegen/Phase1Codegen.h"

#include "codegen/CodeWriter.h"
#include "util/JsonWriter.h"
#include "util/PathUtil.h"

#include <algorithm>
#include <chrono>
#include <cctype>
#include <fstream>
#include <functional>
#include <map>
#include <optional>
#include <set>
#include <regex>
#include <sstream>
#include <tuple>
#include <unordered_set>

namespace vjassc {
namespace {

std::string rewriteOutsideProtected(const std::string& line, const std::function<std::string(std::string)>& rewrite);
std::string replaceRegex(std::string text, const std::string& pattern, const std::string& replacement);
std::vector<int> parseArrayDimensions(std::string_view suffix);
std::string stripLineCommentPreservingLiterals(const std::string& line);
size_t findMatchingBracketOutsideProtected(const std::string& text, size_t open);
size_t findMatchingParen(const std::string& text, size_t open);
std::vector<std::string> splitCommaList(const std::string& text);
std::string joinArgs(const std::vector<std::string>& args);
std::string sanitizeName(std::string text);
bool isIdentStart(char c);
bool isIdentPart(char c);
bool containsIdentifierWord(std::string_view text, std::string_view word);
std::vector<std::string_view> splitLinesView(std::string_view text);
std::string commentMissingInitTriggerCalls(std::string output);

uint64_t fnv1a64(std::string_view text) {
    uint64_t hash = 1469598103934665603ull;
    for (unsigned char c : text) {
        hash ^= c;
        hash *= 1099511628211ull;
    }
    return hash;
}

std::string decimalHash(uint64_t hash) {
    return std::to_string(hash);
}

std::string_view trimView(std::string_view text) {
    size_t start = 0;
    while (start < text.size() && std::isspace(static_cast<unsigned char>(text[start]))) {
        ++start;
    }
    size_t end = text.size();
    while (end > start && std::isspace(static_cast<unsigned char>(text[end - 1]))) {
        --end;
    }
    return text.substr(start, end - start);
}

bool mayContainAnonymousFunction(const std::vector<std::string>& lines) {
    for (const auto& line : lines) {
        if (line.find("function") != std::string::npos &&
            line.find('(') != std::string::npos) {
            return true;
        }
    }
    return false;
}

std::string removeSemicolon(std::string_view text) {
    size_t begin = 0;
    size_t end = text.size();
    while (begin < end && std::isspace(static_cast<unsigned char>(text[begin]))) {
        ++begin;
    }
    while (end > begin && std::isspace(static_cast<unsigned char>(text[end - 1]))) {
        --end;
    }
    if (end > begin && text[end - 1] == ';') {
        --end;
        while (end > begin && std::isspace(static_cast<unsigned char>(text[end - 1]))) {
            --end;
        }
    }
    return std::string(text.substr(begin, end - begin));
}

std::string removeSemicolonsOutsideProtected(const std::string& line) {
    std::string out;
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    for (size_t i = 0; i < line.size(); ++i) {
        if (!inString && !inRaw && i + 1 < line.size() && line[i] == '/' && line[i + 1] == '/') {
            out += line.substr(i);
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
            continue;
        }
        if (inRaw) {
            out.push_back(c);
            if (c == '\'') {
                inRaw = false;
            }
            continue;
        }
        if (c == '"') {
            inString = true;
            out.push_back(c);
            continue;
        }
        if (c == '\'') {
            inRaw = true;
            out.push_back(c);
            continue;
        }
        if (c != ';') {
            out.push_back(c);
        }
    }
    return rtrim(out);
}

std::string normalizeOperatorsOutsideProtected(const std::string& line) {
    std::string out;
    out.reserve(line.size());
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    auto trimTrailingWhitespace = [&]() {
        while (!out.empty() && std::isspace(static_cast<unsigned char>(out.back()))) {
            out.pop_back();
        }
    };
    for (size_t i = 0; i < line.size();) {
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
        if (i + 1 < line.size() && c == '&' && line[i + 1] == '&') {
            trimTrailingWhitespace();
            out.append(" and ");
            i += 2;
            while (i < line.size() && std::isspace(static_cast<unsigned char>(line[i]))) {
                ++i;
            }
            continue;
        }
        if (i + 1 < line.size() && c == '|' && line[i + 1] == '|') {
            trimTrailingWhitespace();
            out.append(" or ");
            i += 2;
            while (i < line.size() && std::isspace(static_cast<unsigned char>(line[i]))) {
                ++i;
            }
            continue;
        }
        if (c == '!' && (i + 1 >= line.size() || line[i + 1] != '=')) {
            char prev = out.empty() ? '\0' : out.back();
            if (prev != '=' && prev != '!' && prev != '<' && prev != '>') {
                out.append("not ");
                ++i;
                while (i < line.size() && std::isspace(static_cast<unsigned char>(line[i]))) {
                    ++i;
                }
                continue;
            }
        }
        if ((i == 0 || !isIdentPart(line[i - 1])) &&
            i + 3 <= line.size() &&
            line.compare(i, 3, "not") == 0 &&
            (i + 3 == line.size() || !isIdentPart(line[i + 3]))) {
            size_t next = i + 3;
            while (next < line.size() && std::isspace(static_cast<unsigned char>(line[next]))) {
                ++next;
            }
            if (next < line.size() && line[next] == '=') {
                out.append("!=");
                i = next + 1;
                continue;
            }
        }
        out.push_back(c);
        ++i;
    }
    return out;
}

std::string sanitizeGeneratedLine(const std::string& line) {
    if (line.find_first_of(";&|!") == std::string::npos &&
        line.find("not") == std::string::npos) {
        return line;
    }
    std::string text = line.find(';') == std::string::npos ? line : removeSemicolonsOutsideProtected(line);
    if (text.find('&') == std::string::npos &&
        text.find('|') == std::string::npos &&
        text.find('!') == std::string::npos &&
        text.find("not") == std::string::npos) {
        return text;
    }
    return normalizeOperatorsOutsideProtected(text);
}

struct FunctionBlock {
    std::string name;
    std::vector<std::string> lines;
    std::vector<std::string> dependencies;
    size_t originalIndex = 0;
};

struct FunctionBlockSignature {
    std::vector<std::string> paramTypes;
    std::string returnType = "nothing";

    bool returnsNothing() const {
        return returnType == "nothing";
    }

    bool takesNothingReturnsNothing() const {
        return paramTypes.empty() && returnsNothing();
    }
};

struct RuntimeInterfaceInfo {
    std::string prefix;
    std::vector<std::string> paramTypes;
    std::string returnType = "nothing";
    int maxId = 0;
    std::vector<std::pair<std::string, int>> newTargets;
};

constexpr std::string_view kFunctionObjectExecuteMarker = "vjassc:function-object-execute";
constexpr std::string_view kFunctionObjectEvaluateMarker = "vjassc:function-object-evaluate";

enum class FunctionObjectCallKind {
    None,
    Execute,
    Evaluate,
};

struct RuntimeBridgeRequest {
    std::string prefix;
    std::string target;
    FunctionBlockSignature signature;
    int id = 0;
    bool needsCondition = false;
    bool needsAction = false;
};

struct MethodCallerRequest {
    std::string target;
    FunctionBlockSignature signature;
};

FunctionObjectCallKind detectFunctionObjectCallKind(const std::string& line) {
    if (line.find(kFunctionObjectExecuteMarker) != std::string::npos) {
        return FunctionObjectCallKind::Execute;
    }
    if (line.find(kFunctionObjectEvaluateMarker) != std::string::npos) {
        return FunctionObjectCallKind::Evaluate;
    }
    return FunctionObjectCallKind::None;
}

std::string stripFunctionObjectCallMarker(const std::string& line) {
    if (detectFunctionObjectCallKind(line) == FunctionObjectCallKind::None) {
        return line;
    }
    size_t comment = line.find("//");
    if (comment == std::string::npos) {
        return line;
    }
    return rtrim(line.substr(0, comment));
}

std::vector<std::string> splitJassParamTypes(const std::string& params) {
    std::vector<std::string> out;
    std::string t = trim(params);
    if (t.empty() || t == "nothing") {
        return out;
    }
    std::stringstream partStream(t);
    std::string part;
    while (std::getline(partStream, part, ',')) {
        std::istringstream in(trim(part));
        std::string type;
        std::string name;
        in >> type >> name;
        if (!type.empty() && !name.empty()) {
            out.push_back(type);
        }
    }
    return out;
}

FunctionBlockSignature parseFunctionBlockSignature(const std::string& header) {
    FunctionBlockSignature sig;
    size_t takes = header.find(" takes ");
    size_t returns = header.find(" returns ");
    if (takes == std::string::npos || returns == std::string::npos || returns <= takes) {
        return sig;
    }
    sig.paramTypes = splitJassParamTypes(header.substr(takes + 7, returns - (takes + 7)));
    std::string ret = trim(std::string_view(header).substr(returns + 9));
    if (!ret.empty()) {
        sig.returnType = ret;
    }
    return sig;
}

std::string lowerAscii(std::string text) {
    for (char& c : text) {
        c = static_cast<char>(std::tolower(static_cast<unsigned char>(c)));
    }
    return text;
}

std::string compactIdentifierHint(std::string text) {
    text = lowerAscii(std::move(text));
    std::string out;
    for (char c : text) {
        if (std::isalnum(static_cast<unsigned char>(c))) {
            out.push_back(c);
        }
    }
    return out;
}

std::string functionNameFromHeaderLine(std::string_view line) {
    std::string_view t = trimView(line);
    if (!startsWithWord(t, "function")) {
        return {};
    }
    size_t pos = 8;
    while (pos < t.size() && std::isspace(static_cast<unsigned char>(t[pos]))) {
        ++pos;
    }
    if (pos >= t.size() || !isIdentStart(t[pos])) {
        return {};
    }
    size_t start = pos++;
    while (pos < t.size() && isIdentPart(t[pos])) {
        ++pos;
    }
    return std::string(t.substr(start, pos - start));
}

std::string commentMissingInitTriggerCalls(std::string output) {
    std::vector<std::string_view> lines = splitLinesView(output);
    std::unordered_set<std::string> definedFunctions;
    for (std::string_view line : lines) {
        std::string_view t = trimView(line);
        if (!startsWithWord(t, "function")) {
            continue;
        }
        std::string name = functionNameFromHeaderLine(t);
        if (!name.empty() && t.find(" takes ") != std::string_view::npos) {
            definedFunctions.insert(name);
        }
    }

    std::string out;
    out.reserve(output.size());
    bool inInitCustomTriggers = false;
    for (std::string_view line : lines) {
        std::string_view trimmed = trimView(line);
        if (trimmed == "function InitCustomTriggers takes nothing returns nothing") {
            inInitCustomTriggers = true;
        } else if (inInitCustomTriggers && trimmed == "endfunction") {
            inInitCustomTriggers = false;
        }

        if (inInitCustomTriggers) {
            size_t indentEnd = line.find_first_not_of(" \t");
            std::string_view indent = indentEnd == std::string_view::npos ? std::string_view{} : line.substr(0, indentEnd);
            std::string_view t = trimView(line);
            if (startsWithWord(t, "call")) {
                size_t pos = 4;
                while (pos < t.size() && std::isspace(static_cast<unsigned char>(t[pos]))) {
                    ++pos;
                }
                size_t nameStart = pos;
                if (nameStart < t.size() && isIdentStart(t[nameStart])) {
                    ++pos;
                    while (pos < t.size() && isIdentPart(t[pos])) {
                        ++pos;
                    }
                    std::string name(t.substr(nameStart, pos - nameStart));
                    while (pos < t.size() && std::isspace(static_cast<unsigned char>(t[pos]))) {
                        ++pos;
                    }
                    if (name.rfind("InitTrig_", 0) == 0 && pos < t.size() && t[pos] == '(') {
                        ++pos;
                        while (pos < t.size() && std::isspace(static_cast<unsigned char>(t[pos]))) {
                            ++pos;
                        }
                        if (pos < t.size() && t[pos] == ')') {
                            ++pos;
                            while (pos < t.size() && std::isspace(static_cast<unsigned char>(t[pos]))) {
                                ++pos;
                            }
                            if (pos == t.size() && !definedFunctions.contains(name)) {
                                out.append(indent.data(), indent.size());
                                out += "//Function not found: call ";
                                out += name;
                                out += "()\n";
                                continue;
                            }
                        }
                    }
                }
            }
        }
        out.append(line.data(), line.size());
        out += '\n';
    }
    return out;
}

std::string blankStringRawcodeAndComment(const std::string& line) {
    std::string out;
    out.reserve(line.size());
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    for (size_t i = 0; i < line.size(); ++i) {
        if (!inString && !inRaw && i + 1 < line.size() && line[i] == '/' && line[i + 1] == '/') {
            break;
        }
        char c = line[i];
        if (inString) {
            if (escaped) {
                escaped = false;
            } else if (c == '\\') {
                escaped = true;
            } else if (c == '"') {
                inString = false;
            }
            out.push_back(' ');
            continue;
        }
        if (inRaw) {
            if (c == '\'') {
                inRaw = false;
            }
            out.push_back(' ');
            continue;
        }
        if (c == '"') {
            inString = true;
            out.push_back(' ');
            continue;
        }
        if (c == '\'') {
            inRaw = true;
            out.push_back(' ');
            continue;
        }
        out.push_back(c);
    }
    return out;
}

void addFunctionDependency(std::vector<std::string>& deps,
                           const std::unordered_map<std::string, size_t, TransparentStringHash, TransparentStringEqual>& functionIndex,
                           const std::string& current,
                           std::string_view candidate,
                           CodegenPerformanceCounters* counters) {
    if (candidate.empty() || candidate == current) {
        return;
    }
    if (functionIndex.find(candidate) != functionIndex.end()) {
        bool seen = false;
        for (const auto& dep : deps) {
            if (dep == candidate) {
                seen = true;
                break;
            }
        }
        if (!seen) {
            deps.emplace_back(candidate);
            if (counters) {
                ++counters->functionOrderEdges;
            }
        }
    }
}

std::vector<std::string> extractFunctionDependencies(const FunctionBlock& block,
                                                     const std::unordered_map<std::string, size_t, TransparentStringHash, TransparentStringEqual>& functionIndex,
                                                     CodegenPerformanceCounters* counters,
                                                     std::unordered_set<std::string>* recordedEdges) {
    std::vector<std::string> deps;
    auto identStart = [](char c) {
        return std::isalpha(static_cast<unsigned char>(c)) || c == '_' || c == '$';
    };
    auto identPart = [](char c) {
        return std::isalnum(static_cast<unsigned char>(c)) || c == '_' || c == '$';
    };
    static const std::unordered_set<std::string_view> ignoredCalls = {
        "function", "if", "call", "set", "return", "loop", "exitwhen",
    };
    for (size_t i = 1; i < block.lines.size(); ++i) {
        const std::string& rawLine = block.lines[i];
        if (rawLine.find('(') == std::string::npos && rawLine.find("function") == std::string::npos) {
            if (counters) {
                ++counters->functionOrderScansAvoided;
            }
            continue;
        }
        if (counters) {
            ++counters->functionOrderTokenScans;
        }
        bool inString = false;
        bool inRaw = false;
        bool escaped = false;
        for (size_t pos = 0; pos < rawLine.size();) {
            if (!inString && !inRaw && pos + 1 < rawLine.size() && rawLine[pos] == '/' && rawLine[pos + 1] == '/') {
                break;
            }
            char c = rawLine[pos];
            if (inString) {
                if (escaped) {
                    escaped = false;
                } else if (c == '\\') {
                    escaped = true;
                } else if (c == '"') {
                    inString = false;
                }
                ++pos;
                continue;
            }
            if (inRaw) {
                if (c == '\'') {
                    inRaw = false;
                }
                ++pos;
                continue;
            }
            if (c == '"') {
                inString = true;
                ++pos;
                continue;
            }
            if (c == '\'') {
                inRaw = true;
                ++pos;
                continue;
            }
            if (!identStart(c)) {
                ++pos;
                continue;
            }
            size_t start = pos++;
            while (pos < rawLine.size() && identPart(rawLine[pos])) {
                ++pos;
            }
            std::string_view name(rawLine.data() + start, pos - start);
            if (name == "function") {
                size_t targetStart = pos;
                while (targetStart < rawLine.size() && std::isspace(static_cast<unsigned char>(rawLine[targetStart]))) {
                    ++targetStart;
                }
                if (targetStart < rawLine.size() && identStart(rawLine[targetStart])) {
                    size_t targetEnd = targetStart + 1;
                    while (targetEnd < rawLine.size() && identPart(rawLine[targetEnd])) {
                        ++targetEnd;
                    }
                    addFunctionDependency(deps,
                                          functionIndex,
                                          block.name,
                                          std::string_view(rawLine.data() + targetStart, targetEnd - targetStart),
                                          counters);
                    pos = targetEnd;
                }
                continue;
            }
            size_t open = pos;
            while (open < rawLine.size() && std::isspace(static_cast<unsigned char>(rawLine[open]))) {
                ++open;
            }
            if (name == "ExecuteFunc" && open < rawLine.size() && rawLine[open] == '(') {
                size_t quote = open + 1;
                while (quote < rawLine.size() && std::isspace(static_cast<unsigned char>(rawLine[quote]))) {
                    ++quote;
                }
                if (quote < rawLine.size() && rawLine[quote] == '"') {
                    size_t targetStart = quote + 1;
                    size_t targetEnd = targetStart;
                    while (targetEnd < rawLine.size() && isIdentPart(rawLine[targetEnd])) {
                        ++targetEnd;
                    }
                    if (targetEnd > targetStart &&
                        targetEnd < rawLine.size() &&
                        rawLine[targetEnd] == '"' &&
                        functionIndex.find(std::string_view(rawLine.data() + targetStart, targetEnd - targetStart)) != functionIndex.end() &&
                        std::string_view(rawLine.data() + targetStart, targetEnd - targetStart) != block.name &&
                        counters) {
                        ++counters->functionDependencyWeakExecuteFuncEdges;
                    }
                }
                pos = open + 1;
                continue;
            }
            if (open >= rawLine.size() || rawLine[open] != '(' || ignoredCalls.contains(name)) {
                pos = open;
                continue;
            }
            addFunctionDependency(deps, functionIndex, block.name, name, counters);
        }
    }
    std::sort(deps.begin(), deps.end());
    if (recordedEdges) {
        for (const auto& dep : deps) {
            if (recordedEdges->insert(block.name + "\n" + dep).second && counters) {
                ++counters->functionDependencyRecordedEdges;
            }
        }
    }
    return deps;
}

std::string bridgeNameForTarget(const std::string& target) {
    return "vjassc__bridge__" + sanitizeName(target);
}

std::string bridgeArgName(const std::string& bridgeName, size_t index) {
    return bridgeName + "_arg" + std::to_string(index);
}

std::string methodCallerPrefixForTarget(const std::string& target) {
    return "vjassc__caller__" + sanitizeName(target);
}

std::string methodCallerTriggerName(const std::string& target) {
    return methodCallerPrefixForTarget(target) + "_trigger";
}

std::string methodCallerArgName(const std::string& target, size_t index) {
    return methodCallerPrefixForTarget(target) + "_arg" + std::to_string(index);
}

std::string methodCallerWrapperName(const std::string& target) {
    return methodCallerPrefixForTarget(target) + "_wrapper";
}

std::string replaceFunctionReferenceTokens(std::string line,
                                           const std::string& target,
                                           const std::string& bridgeName,
                                           bool& changed) {
    return rewriteOutsideProtected(line, [&](std::string text) {
        for (size_t pos = 0; pos < text.size();) {
            size_t found = text.find("function", pos);
            if (found == std::string::npos) {
                break;
            }
            bool wordStart = found == 0 || !isIdentPart(text[found - 1]);
            size_t afterWord = found + 8;
            bool wordEnd = afterWord >= text.size() || !isIdentPart(text[afterWord]);
            if (!wordStart || !wordEnd) {
                pos = afterWord;
                continue;
            }
            size_t nameStart = afterWord;
            while (nameStart < text.size() && std::isspace(static_cast<unsigned char>(text[nameStart]))) {
                ++nameStart;
            }
            if (text.compare(nameStart, target.size(), target) != 0) {
                pos = nameStart;
                continue;
            }
            size_t nameEnd = nameStart + target.size();
            if (nameEnd < text.size() && isIdentPart(text[nameEnd])) {
                pos = nameEnd;
                continue;
            }
            text.replace(nameStart, target.size(), bridgeName);
            changed = true;
            pos = nameStart + bridgeName.size();
        }
        return text;
    });
}

bool parseDirectCallStatement(const std::string& line,
                              const std::string& target,
                              std::string& indent,
                              std::vector<std::string>& args) {
    size_t firstNonSpace = line.find_first_not_of(" \t");
    indent = firstNonSpace == std::string::npos ? "" : line.substr(0, firstNonSpace);
    std::string t = trim(stripLineCommentPreservingLiterals(line));
    if (!startsWithWord(t, "call")) {
        return false;
    }
    std::string rest = trim(std::string_view(t).substr(4));
    if (rest.compare(0, target.size(), target) != 0) {
        return false;
    }
    size_t pos = target.size();
    if (pos < rest.size() && isIdentPart(rest[pos])) {
        return false;
    }
    while (pos < rest.size() && std::isspace(static_cast<unsigned char>(rest[pos]))) {
        ++pos;
    }
    if (pos >= rest.size() || rest[pos] != '(') {
        return false;
    }
    size_t close = findMatchingParen(rest, pos);
    if (close == std::string::npos || !trim(std::string_view(rest).substr(close + 1)).empty()) {
        return false;
    }
    args = splitCommaList(rest.substr(pos + 1, close - pos - 1));
    if (args.size() == 1 && trim(args[0]).empty()) {
        args.clear();
    }
    for (auto& arg : args) {
        arg = trim(arg);
    }
    return true;
}

RuntimeInterfaceInfo* chooseFunctionObjectRuntimeInterface(std::unordered_map<std::string, RuntimeInterfaceInfo>& interfaces,
                                                           const FunctionBlockSignature& targetSig) {
    RuntimeInterfaceInfo* best = nullptr;
    for (auto& [_, iface] : interfaces) {
        if (iface.prefix.find("vjfo__prototype") == std::string::npos ||
            targetSig.returnType != iface.returnType ||
            targetSig.paramTypes.size() != iface.paramTypes.size()) {
            continue;
        }
        bool sameParams = true;
        for (size_t i = 0; i < targetSig.paramTypes.size(); ++i) {
            if (targetSig.paramTypes[i] != iface.paramTypes[i]) {
                sameParams = false;
                break;
            }
        }
        if (!sameParams) {
            continue;
        }
        if (!best || iface.prefix < best->prefix) {
            best = &iface;
        }
    }
    return best;
}

std::string runtimeBridgeKey(const std::string& prefix, const std::string& target) {
    return prefix + "|" + target;
}

std::string runtimeWrapperName(const std::string& prefix, const std::string& target) {
    return prefix + "__" + sanitizeName(target) + "_wrapper";
}

std::vector<std::string> rewriteCyclicDependencyLine(const std::string& line,
                                                     const std::string& target,
                                                     const FunctionBlockSignature& targetSig,
                                                     const std::string& bridgeName,
                                                     std::unordered_map<std::string, RuntimeInterfaceInfo>& runtimeInterfaces,
                                                     std::map<std::string, RuntimeBridgeRequest>& runtimeBridgeRequests,
                                                     std::map<std::string, MethodCallerRequest>& methodCallerRequests,
                                                     bool& changed,
                                                     bool& needsFunctionBridge) {
    bool functionRefChanged = false;
    std::string rewritten = line;
    if (targetSig.takesNothingReturnsNothing()) {
        rewritten = replaceFunctionReferenceTokens(rewritten, target, bridgeName, functionRefChanged);
        if (functionRefChanged) {
            changed = true;
            needsFunctionBridge = true;
            methodCallerRequests[target] = MethodCallerRequest{target, targetSig};
        }
    }

    std::string indent;
    std::vector<std::string> args;
    if (!parseDirectCallStatement(rewritten, target, indent, args)) {
        return {rewritten};
    }
    if (!targetSig.returnsNothing() || args.size() != targetSig.paramTypes.size()) {
        return {rewritten};
    }

    FunctionObjectCallKind callKind = detectFunctionObjectCallKind(rewritten);
    if (callKind != FunctionObjectCallKind::None) {
        RuntimeInterfaceInfo* iface = chooseFunctionObjectRuntimeInterface(runtimeInterfaces, targetSig);
        if (iface) {
            changed = true;
            std::string key = runtimeBridgeKey(iface->prefix, target);
            auto [it, inserted] = runtimeBridgeRequests.try_emplace(key);
            RuntimeBridgeRequest& request = it->second;
            if (inserted) {
                request.prefix = iface->prefix;
                request.target = target;
                request.signature = targetSig;
                request.id = ++iface->maxId;
            }
            if (callKind == FunctionObjectCallKind::Execute) {
                request.needsAction = true;
            } else {
                request.needsCondition = true;
            }
            std::vector<std::string> out;
            out.reserve(args.size() + 1);
            for (size_t i = 0; i < args.size(); ++i) {
                out.push_back(indent + "set " + request.prefix + "_arg" + std::to_string(i) + "=" + args[i]);
            }
            const char* triggerCall = callKind == FunctionObjectCallKind::Execute ? "TriggerExecute" : "TriggerEvaluate";
            out.push_back(indent + "call " + std::string(triggerCall) + "(" + request.prefix + "_trigger[" +
                          std::to_string(request.id) + "])");
            return out;
        }
    }

    changed = true;
    methodCallerRequests[target] = MethodCallerRequest{target, targetSig};
    std::vector<std::string> out;
    out.reserve(args.size() + 1);
    for (size_t i = 0; i < args.size(); ++i) {
        out.push_back(indent + "set " + methodCallerArgName(target, i) + "=" + args[i]);
    }
    out.push_back(indent + "call TriggerEvaluate(" + methodCallerTriggerName(target) + ")");
    return out;
}

void insertBridgeGlobals(std::vector<std::string>& prefix,
                         const std::map<std::string, FunctionBlockSignature>& globalTempBridgeSigs) {
    if (globalTempBridgeSigs.empty()) {
        return;
    }
    size_t insertAt = prefix.size();
    for (size_t i = 0; i < prefix.size(); ++i) {
        if (startsWithWord(trim(prefix[i]), "endglobals")) {
            insertAt = i;
            break;
        }
    }
    std::vector<std::string> globals;
    for (const auto& [target, sig] : globalTempBridgeSigs) {
        std::string bridgeName = bridgeNameForTarget(target);
        for (size_t i = 0; i < sig.paramTypes.size(); ++i) {
            globals.push_back("    " + sig.paramTypes[i] + " " + bridgeArgName(bridgeName, i));
        }
    }
    prefix.insert(prefix.begin() + static_cast<std::ptrdiff_t>(insertAt), globals.begin(), globals.end());
}

void insertMethodCallerGlobals(std::vector<std::string>& prefix,
                               const std::map<std::string, MethodCallerRequest>& requests) {
    if (requests.empty()) {
        return;
    }
    size_t globalsEnd = prefix.size();
    for (size_t i = 0; i < prefix.size(); ++i) {
        std::string t = trim(prefix[i]);
        if (startsWithWord(t, "globals")) {
            continue;
        } else if (startsWithWord(t, "endglobals")) {
            globalsEnd = i;
            break;
        }
    }
    std::vector<std::string> globals;
    if (globalsEnd == prefix.size()) {
        globals.push_back("globals");
    }
    for (const auto& [target, request] : requests) {
        globals.push_back("    trigger " + methodCallerTriggerName(target));
        for (size_t i = 0; i < request.signature.paramTypes.size(); ++i) {
            globals.push_back("    " + request.signature.paramTypes[i] + " " + methodCallerArgName(target, i));
        }
    }
    if (globalsEnd == prefix.size()) {
        globals.push_back("endglobals");
        globals.push_back("");
        globalsEnd = prefix.size();
    }
    prefix.insert(prefix.begin() + static_cast<std::ptrdiff_t>(globalsEnd), globals.begin(), globals.end());
}

template <typename Lines>
std::unordered_map<std::string, RuntimeInterfaceInfo> collectRuntimeInterfaces(const Lines& lines) {
    std::unordered_map<std::string, RuntimeInterfaceInfo> interfaces;
    bool inGlobals = false;
    for (const auto& line : lines) {
        std::string t = trim(line);
        if (startsWithWord(t, "globals")) {
            inGlobals = true;
            continue;
        }
        if (startsWithWord(t, "endglobals")) {
            inGlobals = false;
            continue;
        }
        if (inGlobals) {
            std::istringstream in(t);
            std::string type;
            std::string name;
            in >> type >> name;
            if (name == "array") {
                in >> name;
            }
            if (type.empty() || name.empty()) {
                continue;
            }
            const std::string triggerSuffix = "_trigger";
            size_t trigger = name.find(triggerSuffix);
            if (type == "trigger" && trigger != std::string::npos) {
                std::string prefix = name.substr(0, trigger);
                interfaces[prefix].prefix = prefix;
                continue;
            }
            size_t arg = name.rfind("_arg");
            if (arg != std::string::npos) {
                std::string prefix = name.substr(0, arg);
                std::string indexText = name.substr(arg + 4);
                try {
                    size_t index = static_cast<size_t>(std::stoull(indexText));
                    auto& iface = interfaces[prefix];
                    iface.prefix = prefix;
                    if (iface.paramTypes.size() <= index) {
                        iface.paramTypes.resize(index + 1);
                    }
                    iface.paramTypes[index] = type;
                } catch (...) {
                }
                continue;
            }
            const std::string resultSuffix = "_result";
            if (name.ends_with(resultSuffix)) {
                std::string prefix = name.substr(0, name.size() - resultSuffix.size());
                auto& iface = interfaces[prefix];
                iface.prefix = prefix;
                iface.returnType = type;
            }
        } else {
            size_t setPos = t.find("set ");
            size_t triggerPos = t.find("_trigger[");
            size_t createPos = t.find("=CreateTrigger()");
            if (setPos == 0 && triggerPos != std::string::npos && createPos != std::string::npos) {
                std::string prefix = t.substr(4, triggerPos - 4);
                size_t idStart = triggerPos + 9;
                size_t idEnd = t.find(']', idStart);
                if (idEnd == std::string::npos) {
                    continue;
                }
                try {
                    int id = std::stoi(t.substr(idStart, idEnd - idStart));
                    auto& iface = interfaces[prefix];
                    iface.prefix = prefix;
                    iface.maxId = std::max(iface.maxId, id);
                } catch (...) {
                }
            }
        }
    }
    return interfaces;
}

std::vector<std::string_view> splitLinesView(std::string_view text) {
    std::vector<std::string_view> lines;
    lines.reserve(text.size() / 48 + 1);
    for (size_t pos = 0; pos < text.size();) {
        size_t next = text.find('\n', pos);
        std::string_view line = next == std::string_view::npos
                                    ? text.substr(pos)
                                    : text.substr(pos, next - pos);
        if (!line.empty() && line.back() == '\r') {
            line.remove_suffix(1);
        }
        lines.push_back(line);
        if (next == std::string_view::npos) {
            break;
        }
        pos = next + 1;
    }
    return lines;
}

std::vector<std::string> splitLinesCopy(std::string_view text) {
    std::vector<std::string> lines;
    for (std::string_view line : splitLinesView(text)) {
        lines.emplace_back(line);
    }
    return lines;
}

template <typename Lines>
std::unordered_map<std::string, FunctionBlockSignature> collectFunctionSignatures(const Lines& lines) {
    std::unordered_map<std::string, FunctionBlockSignature> signatures;
    for (const auto& line : lines) {
        std::string t = trim(line);
        if (!startsWithWord(t, "function")) {
            continue;
        }
        std::string name = functionNameFromHeaderLine(t);
        if (!name.empty()) {
            signatures[name] = parseFunctionBlockSignature(t);
        }
    }
    return signatures;
}

bool sameRuntimeSignature(const FunctionBlockSignature& fn, const RuntimeInterfaceInfo& iface) {
    if (fn.returnType != iface.returnType || fn.paramTypes.size() != iface.paramTypes.size()) {
        return false;
    }
    for (size_t i = 0; i < fn.paramTypes.size(); ++i) {
        if (fn.paramTypes[i] != iface.paramTypes[i]) {
            return false;
        }
    }
    return true;
}

RuntimeInterfaceInfo* chooseRuntimeInterface(std::unordered_map<std::string, RuntimeInterfaceInfo>& interfaces,
                                             const FunctionBlockSignature& lambdaSig,
                                             const std::string& calleeName) {
    std::vector<RuntimeInterfaceInfo*> matches;
    for (auto& [_, iface] : interfaces) {
        if (sameRuntimeSignature(lambdaSig, iface)) {
            matches.push_back(&iface);
        }
    }
    if (matches.empty()) {
        return nullptr;
    }
    if (matches.size() == 1) {
        return matches.front();
    }
    std::string calleeHint = compactIdentifierHint(calleeName);
    RuntimeInterfaceInfo* best = nullptr;
    size_t bestScore = 0;
    bool tied = false;
    for (RuntimeInterfaceInfo* iface : matches) {
        std::string ifaceHint = compactIdentifierHint(iface->prefix);
        size_t score = 0;
        for (size_t len = std::min<size_t>(calleeHint.size(), 24); len >= 5; --len) {
            for (size_t start = 0; start + len <= calleeHint.size(); ++start) {
                if (ifaceHint.find(calleeHint.substr(start, len)) != std::string::npos) {
                    score = len;
                    break;
                }
            }
            if (score != 0 || len == 5) {
                break;
            }
        }
        if (score > bestScore) {
            bestScore = score;
            best = iface;
            tied = false;
        } else if (score != 0 && score == bestScore) {
            tied = true;
        }
    }
    if (best && !tied) {
        return best;
    }
    return nullptr;
}

std::string wrapperNameForRuntimeInterfaceTarget(const RuntimeInterfaceInfo& iface, const std::string& target) {
    return iface.prefix + "__" + sanitizeName(target) + "_wrapper";
}

std::optional<int> parsePositiveIntegerLiteral(std::string text) {
    text = trim(text);
    if (text.empty()) {
        return std::nullopt;
    }
    for (char c : text) {
        if (!std::isdigit(static_cast<unsigned char>(c))) {
            return std::nullopt;
        }
    }
    try {
        int value = std::stoi(text);
        if (value > 0) {
            return value;
        }
    } catch (...) {
    }
    return std::nullopt;
}

std::string adaptFunctionInterfaceCallbacksBySignature(const std::string& output) {
    std::vector<std::string> lines = splitLinesCopy(output);
    auto interfaces = collectRuntimeInterfaces(lines);
    if (interfaces.empty()) {
        return output;
    }
    auto functionSigs = collectFunctionSignatures(lines);
    std::unordered_set<std::string> existingFunctions;
    for (const auto& [name, _] : functionSigs) {
        existingFunctions.insert(name);
    }

    bool changed = false;
    std::unordered_map<std::string, std::unordered_map<std::string, int>> targetIdsByInterface;
    for (auto& [prefix, iface] : interfaces) {
        targetIdsByInterface[prefix];
        (void)iface;
    }

    for (auto& rawLine : lines) {
        if (rawLine.find("function vjlambda__") == std::string::npos) {
            continue;
        }
        std::string text = rawLine;
        std::string code = blankStringRawcodeAndComment(text);
        for (size_t pos = 0; pos < code.size();) {
            if (!isIdentStart(code[pos])) {
                ++pos;
                continue;
            }
            size_t nameStart = pos++;
            while (pos < code.size() && isIdentPart(code[pos])) {
                ++pos;
            }
            std::string callee = code.substr(nameStart, pos - nameStart);
            size_t open = pos;
            while (open < code.size() && std::isspace(static_cast<unsigned char>(code[open]))) {
                ++open;
            }
            if (open >= code.size() || code[open] != '(') {
                pos = open;
                continue;
            }
            auto calleeSigIt = functionSigs.find(callee);
            if (calleeSigIt == functionSigs.end()) {
                pos = open + 1;
                continue;
            }
            size_t close = findMatchingParen(text, open);
            if (close == std::string::npos) {
                break;
            }
            std::vector<std::string> args = splitCommaList(text.substr(open + 1, close - open - 1));
            const auto& calleeSig = calleeSigIt->second;
            if (args.size() != calleeSig.paramTypes.size()) {
                pos = close + 1;
                continue;
            }
            bool callChanged = false;
            for (size_t i = 0; i < args.size(); ++i) {
                std::string arg = trim(args[i]);
                if (calleeSig.paramTypes[i] != "integer" || !startsWithWord(arg, "function")) {
                    continue;
                }
                std::string target = trim(std::string_view(arg).substr(8));
                auto lambdaSigIt = functionSigs.find(target);
                if (lambdaSigIt == functionSigs.end() || target.rfind("vjlambda__", 0) != 0) {
                    continue;
                }
                RuntimeInterfaceInfo* iface = chooseRuntimeInterface(interfaces, lambdaSigIt->second, callee);
                if (!iface) {
                    continue;
                }
                std::string wrapper = wrapperNameForRuntimeInterfaceTarget(*iface, target);
                if (existingFunctions.contains(wrapper)) {
                    continue;
                }
                auto& ids = targetIdsByInterface[iface->prefix];
                auto existing = ids.find(target);
                int id = 0;
                if (existing != ids.end()) {
                    id = existing->second;
                } else {
                    id = ++iface->maxId;
                    ids[target] = id;
                    iface->newTargets.push_back({target, id});
                }
                args[i] = std::to_string(id);
                callChanged = true;
            }
            if (callChanged) {
                std::string replacement = callee + "(" + joinArgs(args) + ")";
                text.replace(nameStart, close + 1 - nameStart, replacement);
                code = blankStringRawcodeAndComment(text);
                pos = nameStart + replacement.size();
                changed = true;
            } else {
                pos = close + 1;
            }
        }
        rawLine = std::move(text);
    }
    if (!changed) {
        return output;
    }

    std::vector<std::string> wrappers;
    std::vector<std::string> initLines;
    for (const auto& [_, iface] : interfaces) {
        for (const auto& [target, id] : iface.newTargets) {
            std::string wrapper = wrapperNameForRuntimeInterfaceTarget(iface, target);
            wrappers.push_back("function " + wrapper + " takes nothing returns boolean");
            std::vector<std::string> args;
            for (size_t i = 0; i < iface.paramTypes.size(); ++i) {
                args.push_back(iface.prefix + "_arg" + std::to_string(i));
            }
            if (iface.returnType == "nothing") {
                wrappers.push_back("    call " + target + "(" + joinArgs(args) + ")");
                wrappers.push_back("    return true");
            } else {
                wrappers.push_back("    set " + iface.prefix + "_result=" + target + "(" + joinArgs(args) + ")");
                wrappers.push_back("    return true");
            }
            wrappers.push_back("endfunction");
            wrappers.push_back("");
            initLines.push_back("    set " + iface.prefix + "_trigger[" + std::to_string(id) + "]=CreateTrigger()");
            initLines.push_back("    call TriggerAddCondition(" + iface.prefix + "_trigger[" + std::to_string(id) +
                                "], Condition(function " + wrapper + "))");
        }
    }
    if (wrappers.empty()) {
        return output;
    }

    size_t initStart = lines.size();
    size_t initEnd = lines.size();
    for (size_t i = 0; i < lines.size(); ++i) {
        std::string t = trim(lines[i]);
        if (startsWithWord(t, "function") && functionNameFromHeaderLine(t) == "vjassc__init_function_interfaces") {
            initStart = i;
            for (size_t j = i + 1; j < lines.size(); ++j) {
                if (startsWithWord(trim(lines[j]), "endfunction")) {
                    initEnd = j;
                    break;
                }
            }
            break;
        }
    }
    if (initStart == lines.size() || initEnd == lines.size()) {
        return output;
    }
    lines.insert(lines.begin() + static_cast<std::ptrdiff_t>(initStart), wrappers.begin(), wrappers.end());
    initEnd += wrappers.size();
    lines.insert(lines.begin() + static_cast<std::ptrdiff_t>(initEnd), initLines.begin(), initLines.end());

    std::string out;
    out.reserve(output.size() + wrappers.size() * 96 + initLines.size() * 96);
    for (const auto& outLine : lines) {
        out += outLine;
        out += '\n';
    }
    return out;
}

FunctionObjectCallKind directFunctionObjectCallKindForTarget(const FunctionBlock& block, const std::string& target) {
    for (const auto& line : block.lines) {
        FunctionObjectCallKind kind = detectFunctionObjectCallKind(line);
        if (kind == FunctionObjectCallKind::None) {
            continue;
        }
        std::string indent;
        std::vector<std::string> args;
        if (parseDirectCallStatement(line, target, indent, args)) {
            return kind;
        }
    }
    return FunctionObjectCallKind::None;
}

std::vector<std::string> runtimeBridgeArgs(const RuntimeBridgeRequest& request) {
    std::vector<std::string> args;
    for (size_t i = 0; i < request.signature.paramTypes.size(); ++i) {
        args.push_back(request.prefix + "_arg" + std::to_string(i));
    }
    return args;
}

void appendRuntimeBridgeWrappersAndInit(std::vector<FunctionBlock>& blocks,
                                        const std::map<std::string, RuntimeBridgeRequest>& requests) {
    if (requests.empty()) {
        return;
    }
    std::unordered_set<std::string> existingFunctions;
    for (const auto& block : blocks) {
        if (!block.name.empty()) {
            existingFunctions.insert(block.name);
        }
    }

    bool hasRegistrations = false;
    for (const auto& [_, request] : requests) {
        std::vector<std::string> args = runtimeBridgeArgs(request);
        std::string call = request.target + "(" + joinArgs(args) + ")";
        if (request.needsCondition || request.needsAction) {
            std::string wrapper = runtimeWrapperName(request.prefix, request.target);
            if (!existingFunctions.contains(wrapper)) {
                FunctionBlock block;
                block.name = wrapper;
                block.originalIndex = blocks.size();
                block.lines.push_back("function " + wrapper + " takes nothing returns nothing");
                if (request.signature.returnType == "nothing") {
                    block.lines.push_back("    call " + call);
                } else {
                    block.lines.push_back("    set " + request.prefix + "_result=" + call);
                }
                block.lines.push_back("endfunction");
                block.lines.push_back("");
                existingFunctions.insert(block.name);
                blocks.push_back(std::move(block));
            }
            hasRegistrations = true;
        }
    }
    if (!hasRegistrations) {
        return;
    }

    bool inserted = false;
    for (auto& block : blocks) {
        if (block.name != "vjassc__init_function_interfaces") {
            continue;
        }
        for (size_t i = 0; i < block.lines.size(); ++i) {
            if (startsWithWord(trim(block.lines[i]), "endfunction")) {
                std::vector<std::string> lines;
                for (const auto& [_, request] : requests) {
                    lines.push_back("    set " + request.prefix + "_trigger[" + std::to_string(request.id) +
                                    "]=CreateTrigger()");
                    if (request.needsCondition) {
                        lines.push_back("    call TriggerAddCondition(" + request.prefix + "_trigger[" +
                                        std::to_string(request.id) + "], Condition(function " +
                                        runtimeWrapperName(request.prefix, request.target) + "))");
                    }
                    if (request.needsAction) {
                        lines.push_back("    call TriggerAddAction(" + request.prefix + "_trigger[" +
                                        std::to_string(request.id) + "], function " +
                                        runtimeWrapperName(request.prefix, request.target) + ")");
                    }
                }
                block.lines.insert(block.lines.begin() + static_cast<std::ptrdiff_t>(i), lines.begin(), lines.end());
                inserted = true;
                break;
            }
        }
        break;
    }
    if (inserted) {
        return;
    }

    FunctionBlock init;
    init.name = "vjassc__init_function_interfaces";
    init.originalIndex = blocks.size();
    init.lines.push_back("function vjassc__init_function_interfaces takes nothing returns nothing");
    for (const auto& [_, request] : requests) {
        init.lines.push_back("    set " + request.prefix + "_trigger[" + std::to_string(request.id) +
                             "]=CreateTrigger()");
        if (request.needsCondition) {
            init.lines.push_back("    call TriggerAddCondition(" + request.prefix + "_trigger[" +
                                 std::to_string(request.id) + "], Condition(function " +
                                 runtimeWrapperName(request.prefix, request.target) + "))");
        }
        if (request.needsAction) {
            init.lines.push_back("    call TriggerAddAction(" + request.prefix + "_trigger[" +
                                 std::to_string(request.id) + "], function " +
                                 runtimeWrapperName(request.prefix, request.target) + ")");
        }
    }
    init.lines.push_back("endfunction");
    init.lines.push_back("");
    blocks.push_back(std::move(init));
}

std::vector<std::string> methodCallerArgs(const MethodCallerRequest& request) {
    std::vector<std::string> args;
    for (size_t i = 0; i < request.signature.paramTypes.size(); ++i) {
        args.push_back(methodCallerArgName(request.target, i));
    }
    return args;
}

void appendMethodCallerWrappersAndInit(std::vector<FunctionBlock>& blocks,
                                       const std::map<std::string, MethodCallerRequest>& requests) {
    if (requests.empty()) {
        return;
    }
    std::unordered_set<std::string> existingFunctions;
    for (const auto& block : blocks) {
        if (!block.name.empty()) {
            existingFunctions.insert(block.name);
        }
    }

    for (const auto& [target, request] : requests) {
        std::string wrapper = methodCallerWrapperName(target);
        if (existingFunctions.contains(wrapper)) {
            continue;
        }
        FunctionBlock block;
        block.name = wrapper;
        block.originalIndex = blocks.size();
        block.lines.push_back("function " + wrapper + " takes nothing returns boolean");
        block.lines.push_back("    call " + target + "(" + joinArgs(methodCallerArgs(request)) + ")");
        block.lines.push_back("    return true");
        block.lines.push_back("endfunction");
        block.lines.push_back("");
        existingFunctions.insert(block.name);
        blocks.push_back(std::move(block));
    }

    bool inserted = false;
    for (auto& block : blocks) {
        if (block.name != "vjassc__init_function_interfaces") {
            continue;
        }
        for (size_t i = 0; i < block.lines.size(); ++i) {
            if (startsWithWord(trim(block.lines[i]), "endfunction")) {
                std::vector<std::string> lines;
                for (const auto& [target, _] : requests) {
                    lines.push_back("    set " + methodCallerTriggerName(target) + "=CreateTrigger()");
                    lines.push_back("    call TriggerAddCondition(" + methodCallerTriggerName(target) +
                                    ", Condition(function " + methodCallerWrapperName(target) + "))");
                }
                block.lines.insert(block.lines.begin() + static_cast<std::ptrdiff_t>(i), lines.begin(), lines.end());
                inserted = true;
                break;
            }
        }
        break;
    }
    if (inserted) {
        return;
    }

    FunctionBlock init;
    init.name = "vjassc__init_function_interfaces";
    init.originalIndex = blocks.size();
    init.lines.push_back("function vjassc__init_function_interfaces takes nothing returns nothing");
    for (const auto& [target, _] : requests) {
        init.lines.push_back("    set " + methodCallerTriggerName(target) + "=CreateTrigger()");
        init.lines.push_back("    call TriggerAddCondition(" + methodCallerTriggerName(target) +
                             ", Condition(function " + methodCallerWrapperName(target) + "))");
    }
    init.lines.push_back("endfunction");
    init.lines.push_back("");
    blocks.push_back(std::move(init));
}

bool blockContainsLine(const FunctionBlock& block, std::string_view needle) {
    return std::any_of(block.lines.begin(), block.lines.end(), [&](const std::string& line) {
        return line.find(needle) != std::string::npos;
    });
}

void ensureFunctionInterfaceInitCall(std::vector<FunctionBlock>& blocks) {
    bool hasInitFunction = false;
    for (const auto& block : blocks) {
        if (block.name == "vjassc__init_function_interfaces") {
            hasInitFunction = true;
            break;
        }
    }
    if (!hasInitFunction) {
        return;
    }
    for (auto& block : blocks) {
        if (block.name != "main") {
            continue;
        }
        if (blockContainsLine(block, "call vjassc__init_function_interfaces()")) {
            return;
        }
        for (size_t i = 0; i < block.lines.size(); ++i) {
            if (block.lines[i].find("call vjassc__init_structs()") != std::string::npos) {
                block.lines.insert(block.lines.begin() + static_cast<std::ptrdiff_t>(i + 1),
                                   "    call vjassc__init_function_interfaces()");
                return;
            }
        }
        for (size_t i = 0; i < block.lines.size(); ++i) {
            if (block.lines[i].find("call vjassc__init_libraries()") != std::string::npos) {
                block.lines.insert(block.lines.begin() + static_cast<std::ptrdiff_t>(i),
                                   "    call vjassc__init_function_interfaces()");
                return;
            }
        }
        for (size_t i = 0; i < block.lines.size(); ++i) {
            if (startsWithWord(trim(block.lines[i]), "endfunction")) {
                block.lines.insert(block.lines.begin() + static_cast<std::ptrdiff_t>(i),
                                   "    call vjassc__init_function_interfaces()");
                return;
            }
        }
        return;
    }
}

std::string orderFunctionBlocksForPjass(const std::string& output,
                                        CodegenPerformanceCounters* counters,
                                        std::unordered_set<std::string>* recordedEdges,
                                        bool useRecordedOrder) {
    std::vector<std::string> lines;
    std::istringstream in(output);
    std::string line;
    while (std::getline(in, line)) {
        lines.push_back(line);
    }

    std::vector<std::string> prefix;
    std::vector<std::string> suffix;
    std::vector<FunctionBlock> blocks;
    bool collectingFunctions = false;
    for (size_t i = 0; i < lines.size();) {
        std::string t = trim(lines[i]);
        if (startsWithWord(t, "function")) {
            collectingFunctions = true;
            FunctionBlock block;
            block.name = functionNameFromHeaderLine(t);
            block.originalIndex = blocks.size();
            while (i < lines.size()) {
                block.lines.emplace_back(lines[i]);
                std::string inner = trim(lines[i]);
                ++i;
                if (startsWithWord(inner, "endfunction")) {
                    break;
                }
            }
            while (i < lines.size() && trim(lines[i]).empty()) {
                block.lines.emplace_back(lines[i]);
                ++i;
            }
            blocks.push_back(std::move(block));
            continue;
        }
        if (collectingFunctions) {
            suffix.emplace_back(lines[i]);
        } else {
            prefix.emplace_back(lines[i]);
        }
        ++i;
    }
    if (blocks.empty()) {
        return output;
    }

    std::unordered_map<std::string, size_t, TransparentStringHash, TransparentStringEqual> functionIndex;
    std::unordered_map<std::string, FunctionBlockSignature> signatures;
    for (size_t i = 0; i < blocks.size(); ++i) {
        if (!blocks[i].name.empty()) {
            functionIndex[blocks[i].name] = i;
            signatures[blocks[i].name] = parseFunctionBlockSignature(blocks[i].lines.empty() ? "" : blocks[i].lines.front());
        }
    }
    auto assignRecordedDependencies = [&]() -> size_t {
        if (!recordedEdges) {
            return 0;
        }
        size_t used = 0;
        for (const auto& edge : *recordedEdges) {
            size_t split = edge.find('\n');
            if (split == std::string::npos) {
                continue;
            }
            std::string_view from(edge.data(), split);
            std::string_view to(edge.data() + split + 1, edge.size() - split - 1);
            auto fromIt = functionIndex.find(from);
            auto toIt = functionIndex.find(to);
            if (fromIt == functionIndex.end() || toIt == functionIndex.end() || fromIt->second == toIt->second) {
                continue;
            }
            auto& deps = blocks[fromIt->second].dependencies;
            if (std::find(deps.begin(), deps.end(), std::string(to)) == deps.end()) {
                deps.emplace_back(to);
                ++used;
            }
        }
        for (auto& block : blocks) {
            std::sort(block.dependencies.begin(), block.dependencies.end());
        }
        return used;
    };
    bool recordedOrderUsed = false;
    bool dependenciesAssigned = false;
    if (useRecordedOrder) {
        size_t recordedUsed = assignRecordedDependencies();
        if (recordedUsed > 0) {
            std::unordered_set<std::string> scannedEdges;
            for (const auto& block : blocks) {
                std::vector<std::string> scanned = extractFunctionDependencies(block, functionIndex, nullptr, nullptr);
                for (const auto& dep : scanned) {
                    scannedEdges.insert(block.name + "\n" + dep);
                }
            }
            size_t matched = 0;
            if (recordedEdges) {
                for (const auto& edge : *recordedEdges) {
                    if (scannedEdges.contains(edge)) {
                        ++matched;
                    }
                }
            }
            const size_t missing = scannedEdges.size() > matched ? scannedEdges.size() - matched : 0;
            if (missing == 0) {
                recordedOrderUsed = true;
                if (counters) {
                    counters->functionDependencyRecordedOrderEdgesUsed += recordedUsed;
                    counters->functionOrderEdges += recordedUsed;
                }
            } else {
                if (counters) {
                    ++counters->functionDependencyRecordedOrderFallbacks;
                }
                for (auto& block : blocks) {
                    block.dependencies = extractFunctionDependencies(block, functionIndex, counters, nullptr);
                }
                dependenciesAssigned = true;
            }
        }
    }
    if (recordedOrderUsed) {
        // The experimental path consumes edges recorded while lowering bodies
        // after proving they cover the conservative output-scan edges.
    } else if (!dependenciesAssigned) {
        for (auto& block : blocks) {
            block.dependencies = extractFunctionDependencies(block, functionIndex, counters, recordedEdges);
        }
    }
    auto rebuildFunctionIndexAndSignatures = [&]() {
        functionIndex.clear();
        signatures.clear();
        for (size_t i = 0; i < blocks.size(); ++i) {
            if (!blocks[i].name.empty()) {
                functionIndex[blocks[i].name] = i;
                signatures[blocks[i].name] = parseFunctionBlockSignature(blocks[i].lines.empty() ? "" : blocks[i].lines.front());
            }
        }
    };
    auto markAppendedBlocks = [&](size_t start, std::unordered_set<size_t>& dirty) {
        for (size_t i = start; i < blocks.size(); ++i) {
            dirty.insert(i);
        }
    };
    auto markBlockByName = [&](std::string_view name, std::unordered_set<size_t>& dirty) {
        for (size_t i = 0; i < blocks.size(); ++i) {
            if (blocks[i].name == name) {
                dirty.insert(i);
            }
        }
    };
    auto refreshDirtyDependencies = [&](const std::unordered_set<size_t>& dirty) {
        if (dirty.empty()) {
            return;
        }
        rebuildFunctionIndexAndSignatures();
        for (size_t index : dirty) {
            if (index < blocks.size()) {
                blocks[index].dependencies = extractFunctionDependencies(blocks[index],
                                                                         functionIndex,
                                                                         counters,
                                                                         useRecordedOrder ? nullptr : recordedEdges);
            }
        }
    };

    std::set<std::pair<size_t, size_t>> cyclicEdges;
    std::vector<int> cycleState(blocks.size(), 0);
    std::function<void(size_t)> findCycles = [&](size_t index) {
        if (cycleState[index] == 2) {
            return;
        }
        if (cycleState[index] == 1) {
            return;
        }
        cycleState[index] = 1;
        for (const auto& depName : blocks[index].dependencies) {
            auto dep = functionIndex.find(depName);
            if (dep == functionIndex.end()) {
                continue;
            }
            if (cycleState[dep->second] == 1) {
                cyclicEdges.insert({index, dep->second});
                continue;
            }
            findCycles(dep->second);
        }
        cycleState[index] = 2;
    };
    for (size_t i = 0; i < blocks.size(); ++i) {
        findCycles(i);
    }
    if (counters && !cyclicEdges.empty()) {
        std::unordered_map<size_t, std::vector<size_t>> cycleGraph;
        for (const auto& [from, to] : cyclicEdges) {
            cycleGraph[from].push_back(to);
            cycleGraph[to].push_back(from);
        }
        std::unordered_set<size_t> visitedCycleNodes;
        for (const auto& [node, _] : cycleGraph) {
            if (!visitedCycleNodes.insert(node).second) {
                continue;
            }
            ++counters->functionOrderSccCount;
            std::vector<size_t> stack{node};
            while (!stack.empty()) {
                size_t current = stack.back();
                stack.pop_back();
                auto it = cycleGraph.find(current);
                if (it == cycleGraph.end()) {
                    continue;
                }
                for (size_t next : it->second) {
                    if (visitedCycleNodes.insert(next).second) {
                        stack.push_back(next);
                    }
                }
            }
        }
    }

    std::set<std::pair<size_t, size_t>> rewriteEdges;
    for (const auto& [callerIndex, targetIndex] : cyclicEdges) {
        std::pair<size_t, size_t> edge{callerIndex, targetIndex};
        const std::string& caller = blocks[callerIndex].name;
        const std::string& target = blocks[targetIndex].name;
        auto targetSigIt = signatures.find(target);
        auto callerSigIt = signatures.find(caller);
        if (targetSigIt != signatures.end() &&
            callerSigIt != signatures.end() &&
            targetSigIt->second.returnsNothing() &&
            !targetSigIt->second.paramTypes.empty() &&
            callerSigIt->second.takesNothingReturnsNothing() &&
            std::binary_search(blocks[targetIndex].dependencies.begin(), blocks[targetIndex].dependencies.end(), caller)) {
            edge = {targetIndex, callerIndex};
        } else if (callerIndex < targetIndex &&
                   directFunctionObjectCallKindForTarget(blocks[callerIndex], target) == FunctionObjectCallKind::Evaluate &&
                   std::binary_search(blocks[targetIndex].dependencies.begin(), blocks[targetIndex].dependencies.end(), caller) &&
                   directFunctionObjectCallKindForTarget(blocks[targetIndex], caller) == FunctionObjectCallKind::Evaluate) {
            edge = {targetIndex, callerIndex};
        }
        rewriteEdges.insert(edge);
    }

    auto runtimeInterfaces = collectRuntimeInterfaces(lines);
    std::map<std::string, RuntimeBridgeRequest> runtimeBridgeRequests;
    std::map<std::string, MethodCallerRequest> methodCallerRequests;
    std::set<std::string> functionBridgeTargets;
    std::unordered_set<size_t> dependencyDirtyBlocks;
    bool cycleRewriteChanged = false;
    for (const auto& [callerIndex, targetIndex] : rewriteEdges) {
        const std::string& target = blocks[targetIndex].name;
        auto sigIt = signatures.find(target);
        if (target.empty() || sigIt == signatures.end() || !sigIt->second.returnsNothing()) {
            continue;
        }
        std::vector<std::string> rewrittenLines;
        bool blockChanged = false;
        bool needsFunctionBridge = false;
        std::string bridgeName = bridgeNameForTarget(target);
        for (const auto& blockLine : blocks[callerIndex].lines) {
            bool lineChanged = false;
            bool lineNeedsFunctionBridge = false;
            auto lineOut = rewriteCyclicDependencyLine(blockLine,
                                                       target,
                                                       sigIt->second,
                                                       bridgeName,
                                                       runtimeInterfaces,
                                                       runtimeBridgeRequests,
                                                       methodCallerRequests,
                                                       lineChanged,
                                                       lineNeedsFunctionBridge);
            blockChanged = blockChanged || lineChanged;
            needsFunctionBridge = needsFunctionBridge || lineNeedsFunctionBridge;
            rewrittenLines.insert(rewrittenLines.end(), lineOut.begin(), lineOut.end());
        }
        if (blockChanged) {
            cycleRewriteChanged = true;
            dependencyDirtyBlocks.insert(callerIndex);
            blocks[callerIndex].lines = std::move(rewrittenLines);
            if (needsFunctionBridge) {
                functionBridgeTargets.insert(target);
            }
        }
    }
    bool runtimeBridgeChanged = !runtimeBridgeRequests.empty();
    size_t appendedBridgeStart = blocks.size();
    appendRuntimeBridgeWrappersAndInit(blocks, runtimeBridgeRequests);
    appendMethodCallerWrappersAndInit(blocks, methodCallerRequests);
    ensureFunctionInterfaceInitCall(blocks);
    if (runtimeBridgeChanged || !methodCallerRequests.empty()) {
        markAppendedBlocks(appendedBridgeStart, dependencyDirtyBlocks);
        markBlockByName("vjassc__init_function_interfaces", dependencyDirtyBlocks);
        markBlockByName("main", dependencyDirtyBlocks);
    }
    if (!functionBridgeTargets.empty()) {
        insertMethodCallerGlobals(prefix, methodCallerRequests);
        size_t functionBridgeStart = blocks.size();
        for (const auto& target : functionBridgeTargets) {
            const auto& sig = signatures[target];
            FunctionBlock bridge;
            bridge.name = bridgeNameForTarget(target);
            bridge.originalIndex = blocks.size();
            bridge.lines.push_back("function " + bridge.name + " takes nothing returns nothing");
            if (sig.paramTypes.empty()) {
                bridge.lines.push_back("    call TriggerEvaluate(" + methodCallerTriggerName(target) + ")");
            } else {
                std::vector<std::string> args;
                for (size_t i = 0; i < sig.paramTypes.size(); ++i) {
                    args.push_back(bridgeArgName(bridge.name, i));
                }
                bridge.lines.push_back("    call " + target + "(" + joinArgs(args) + ")");
            }
            bridge.lines.push_back("endfunction");
            bridge.lines.push_back("");
            blocks.push_back(std::move(bridge));
        }
        markAppendedBlocks(functionBridgeStart, dependencyDirtyBlocks);
        refreshDirtyDependencies(dependencyDirtyBlocks);
    } else if (runtimeBridgeChanged || !methodCallerRequests.empty()) {
        insertMethodCallerGlobals(prefix, methodCallerRequests);
        refreshDirtyDependencies(dependencyDirtyBlocks);
    } else if (cycleRewriteChanged) {
        refreshDirtyDependencies(dependencyDirtyBlocks);
    }

    if (counters) {
        std::unordered_set<std::string> outputEdges;
        for (const auto& block : blocks) {
            for (const auto& dep : block.dependencies) {
                outputEdges.insert(block.name + "\n" + dep);
            }
        }
        counters->functionDependencyOutputScanEdges = outputEdges.size();
        if (recordedEdges) {
            size_t matched = 0;
            for (const auto& edge : *recordedEdges) {
                if (outputEdges.contains(edge)) {
                    ++matched;
                }
            }
            counters->functionDependencyMatchedEdges = matched;
            counters->functionDependencyExtraRecordedEdges = recordedEdges->size() - matched;
            counters->functionDependencyMissingRecordedEdges = outputEdges.size() - matched;
        }
    }

    std::vector<size_t> ordered;
    std::vector<int> state(blocks.size(), 0);
    std::function<void(size_t)> visit = [&](size_t index) {
        if (state[index] == 2) {
            return;
        }
        if (state[index] == 1) {
            return;
        }
        state[index] = 1;
        for (const auto& depName : blocks[index].dependencies) {
            auto dep = functionIndex.find(depName);
            if (dep != functionIndex.end()) {
                visit(dep->second);
            }
        }
        state[index] = 2;
        ordered.push_back(index);
    };
    for (size_t i = 0; i < blocks.size(); ++i) {
        visit(i);
    }

    std::string out;
    out.reserve(output.size() + output.size() / 64);
    auto appendLines = [&](const std::vector<std::string>& append) {
        for (const auto& outLine : append) {
            out += stripFunctionObjectCallMarker(outLine);
            out += '\n';
        }
    };
    appendLines(prefix);
    for (size_t index : ordered) {
        appendLines(blocks[index].lines);
    }
    appendLines(suffix);
    return out;
}

std::string parseNativeName(const std::string& line) {
    std::istringstream in(trim(line));
    std::string word;
    std::string name;
    in >> word >> name;
    return name;
}

bool isTypeWord(const std::string& word) {
    static const std::unordered_set<std::string> types = {
        "integer", "real", "boolean", "string", "code", "handle", "unit", "player", "timer",
        "trigger", "effect", "group", "force", "rect", "location", "item", "destructable",
        "widget", "image", "sound", "region", "hashtable", "boolexpr", "dialog", "button",
        "thistype"
    };
    return types.contains(word);
}

std::string defaultReturnValueForType(const std::string& type) {
    if (type == "nothing") {
        return {};
    }
    if (type == "real") {
        return "0.0";
    }
    if (type == "boolean") {
        return "false";
    }
    if (type == "string" || type == "code") {
        return "null";
    }
    if (type == "integer") {
        return "0";
    }
    return "null";
}

bool isLocalDecl(const std::string& line) {
    std::string_view t = trimView(line);
    if (startsWithWord(t, "local")) {
        return true;
    }
    if (startsWithWord(t, "call") || startsWithWord(t, "set") || startsWithWord(t, "return") ||
        startsWithWord(t, "if") || startsWithWord(t, "while") || startsWithWord(t, "for") ||
        startsWithWord(t, "loop") || startsWithWord(t, "exitwhen") || startsWithWord(t, "else")) {
        return false;
    }
    auto nextWord = [&](size_t& pos) -> std::string_view {
        while (pos < t.size() && std::isspace(static_cast<unsigned char>(t[pos]))) {
            ++pos;
        }
        size_t start = pos;
        while (pos < t.size() && !std::isspace(static_cast<unsigned char>(t[pos]))) {
            ++pos;
        }
        return t.substr(start, pos - start);
    };
    size_t pos = 0;
    std::string_view word = nextWord(pos);
    std::string_view name = nextWord(pos);
    if (word == "constant") {
        word = name;
        name = nextWord(pos);
    }
    if (word.empty() || name.empty()) {
        return false;
    }
    for (char c : word) {
        if (!std::isalnum(static_cast<unsigned char>(c)) && c != '_' && c != '$') {
            return false;
        }
    }
    if (!std::isalpha(static_cast<unsigned char>(word.front())) && word.front() != '_' && word.front() != '$') {
        return false;
    }
    return (std::isalpha(static_cast<unsigned char>(name.front())) || name.front() == '_' || name.front() == '$') ||
           isTypeWord(std::string(word));
}

std::string parenContent(const std::string& text) {
    size_t open = text.find('(');
    if (open == std::string::npos) {
        return {};
    }
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    int depth = 0;
    for (size_t i = open; i < text.size(); ++i) {
        char c = text[i];
        if (inString) {
            if (escaped) {
                escaped = false;
            } else if (c == '\\') {
                escaped = true;
            } else if (c == '"') {
                inString = false;
            }
            continue;
        }
        if (inRaw) {
            if (c == '\'') {
                inRaw = false;
            }
            continue;
        }
        if (c == '"') {
            inString = true;
        } else if (c == '\'') {
            inRaw = true;
        } else if (c == '(') {
            ++depth;
        } else if (c == ')') {
            --depth;
            if (depth == 0) {
                return trim(std::string_view(text).substr(open + 1, i - open - 1));
            }
        }
    }
    return {};
}

std::string conditionFromHeader(const std::string& text) {
    return "(" + parenContent(text) + ")";
}

bool isAssignmentLike(const std::string& text) {
    for (size_t i = 0; i < text.size(); ++i) {
        if (text[i] != '=') {
            continue;
        }
        char prev = i > 0 ? text[i - 1] : '\0';
        char next = i + 1 < text.size() ? text[i + 1] : '\0';
        if (prev != '=' && prev != '!' && prev != '<' && prev != '>' && next != '=') {
            return true;
        }
    }
    return text.find("+=") != std::string::npos || text.find("-=") != std::string::npos ||
           text.find("*=") != std::string::npos || text.find("/=") != std::string::npos;
}

std::vector<std::string> splitSemicolons(const std::string& text) {
    std::vector<std::string> out;
    std::string cur;
    int parens = 0;
    bool inString = false;
    for (char c : text) {
        if (c == '"') {
            inString = !inString;
        } else if (!inString && c == '(') {
            ++parens;
        } else if (!inString && c == ')' && parens > 0) {
            --parens;
        }
        if (!inString && parens == 0 && c == ';') {
            out.push_back(trim(cur));
            cur.clear();
        } else {
            cur.push_back(c);
        }
    }
    out.push_back(trim(cur));
    return out;
}

bool isIdentStart(char c) {
    return std::isalpha(static_cast<unsigned char>(c)) || c == '_' || c == '$';
}

bool isIdentPart(char c) {
    return std::isalnum(static_cast<unsigned char>(c)) || c == '_' || c == '$';
}

bool containsIdentifierWord(std::string_view text, std::string_view word) {
    size_t pos = 0;
    while ((pos = text.find(word, pos)) != std::string_view::npos) {
        const char before = pos > 0 ? text[pos - 1] : '\0';
        const size_t afterPos = pos + word.size();
        const char after = afterPos < text.size() ? text[afterPos] : '\0';
        if (!isIdentPart(before) && !isIdentPart(after)) {
            return true;
        }
        pos += word.size();
    }
    return false;
}

std::optional<size_t> findTopLevelOperatorAssignment(const std::string& text, std::string_view op) {
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    int parens = 0;
    int brackets = 0;
    for (size_t i = 0; i + op.size() <= text.size(); ++i) {
        char c = text[i];
        if (!inString && !inRaw && i + 1 < text.size() && c == '/' && text[i + 1] == '/') {
            return std::nullopt;
        }
        if (inString) {
            if (escaped) {
                escaped = false;
            } else if (c == '\\') {
                escaped = true;
            } else if (c == '"') {
                inString = false;
            }
            continue;
        }
        if (inRaw) {
            if (c == '\'') {
                inRaw = false;
            }
            continue;
        }
        if (c == '"') {
            inString = true;
        } else if (c == '\'') {
            inRaw = true;
        } else if (c == '(') {
            ++parens;
        } else if (c == ')' && parens > 0) {
            --parens;
        } else if (c == '[') {
            ++brackets;
        } else if (c == ']' && brackets > 0) {
            --brackets;
        } else if (parens == 0 && brackets == 0 && text.compare(i, op.size(), op) == 0) {
            return i;
        }
    }
    return std::nullopt;
}

bool containsIdentifierOutsideProtected(const std::string& text, const std::string& ident) {
    if (ident.empty()) {
        return false;
    }
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    for (size_t i = 0; i < text.size();) {
        if (!inString && !inRaw && i + 1 < text.size() && text[i] == '/' && text[i + 1] == '/') {
            return false;
        }
        char c = text[i];
        if (inString) {
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
            if (c == '\'') {
                inRaw = false;
            }
            ++i;
            continue;
        }
        if (c == '"') {
            inString = true;
            ++i;
            continue;
        }
        if (c == '\'') {
            inRaw = true;
            ++i;
            continue;
        }
        if (i + ident.size() <= text.size() && text.compare(i, ident.size(), ident) == 0) {
            char before = i > 0 ? text[i - 1] : '\0';
            char after = i + ident.size() < text.size() ? text[i + ident.size()] : '\0';
            if (!isIdentPart(before) && before != '.' && !isIdentPart(after)) {
                return true;
            }
        }
        ++i;
    }
    return false;
}

std::optional<std::pair<size_t, size_t>> findAnonymousFunctionStart(const std::string& text) {
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    for (size_t i = 0; i < text.size();) {
        if (!inString && !inRaw && i + 1 < text.size() && text[i] == '/' && text[i + 1] == '/') {
            return std::nullopt;
        }
        char c = text[i];
        if (inString) {
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
            if (c == '\'') {
                inRaw = false;
            }
            ++i;
            continue;
        }
        if (c == '"') {
            inString = true;
            ++i;
            continue;
        }
        if (c == '\'') {
            inRaw = true;
            ++i;
            continue;
        }
        if (i + 8 <= text.size() && text.compare(i, 8, "function") == 0) {
            char before = i > 0 ? text[i - 1] : '\0';
            char afterWord = i + 8 < text.size() ? text[i + 8] : '\0';
            if (!isIdentPart(before) && !isIdentPart(afterWord)) {
                size_t after = i + 8;
                while (after < text.size() && std::isspace(static_cast<unsigned char>(text[after]))) {
                    ++after;
                }
                if (after < text.size() && text[after] == '(') {
                    return std::make_pair(i, after);
                }
            }
        }
        ++i;
    }
    return std::nullopt;
}

std::vector<std::string> declaredNamesInZincStatements(const std::string& raw) {
    std::vector<std::string> names;
    for (auto statement : splitSemicolons(raw)) {
        statement = removeSemicolon(trim(statement));
        if (statement.empty()) {
            continue;
        }
        if (startsWithWord(statement, "local")) {
            statement = trim(std::string_view(statement).substr(5));
        }
        if (!isLocalDecl(statement)) {
            continue;
        }
        if (startsWithWord(statement, "constant")) {
            statement = trim(std::string_view(statement).substr(8));
        }
        size_t assign = statement.find('=');
        std::string declPart = assign == std::string::npos ? statement : trim(std::string_view(statement).substr(0, assign));
        std::istringstream in(declPart);
        std::string type;
        std::string name;
        in >> type >> name;
        if (name == "array") {
            in >> name;
        }
        size_t bracket = name.find('[');
        if (bracket != std::string::npos) {
            name = name.substr(0, bracket);
        }
        if (!type.empty() && !name.empty()) {
            names.push_back(name);
        }
    }
    return names;
}

std::string regexEscape(const std::string& text) {
    std::string out;
    for (char c : text) {
        if (std::string_view(R"(\.^$|()[]{}*+?)").find(c) != std::string_view::npos) {
            out.push_back('\\');
        }
        out.push_back(c);
    }
    return out;
}

std::string rewriteOutsideProtected(const std::string& line, const std::function<std::string(std::string)>& rewrite) {
    if (line.find_first_of("\"'") == std::string::npos &&
        line.find("//") == std::string::npos) {
        return rewrite(line);
    }
    std::string out;
    std::string normal;
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    auto flush = [&]() {
        if (!normal.empty()) {
            out += rewrite(normal);
            normal.clear();
        }
    };
    for (size_t i = 0; i < line.size();) {
        if (!inString && !inRaw && i + 1 < line.size() && line[i] == '/' && line[i + 1] == '/') {
            flush();
            out += line.substr(i);
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
            flush();
            inString = true;
            out.push_back(c);
            ++i;
            continue;
        }
        if (c == '\'') {
            flush();
            inRaw = true;
            out.push_back(c);
            ++i;
            continue;
        }
        normal.push_back(c);
        ++i;
    }
    flush();
    return out;
}

std::string replaceRegex(std::string text, const std::string& pattern, const std::string& replacement) {
    static std::unordered_map<std::string, std::regex> regexCache = [] {
        std::unordered_map<std::string, std::regex> cache;
        cache.reserve(20000);
        return cache;
    }();
    auto [it, _] = regexCache.try_emplace(pattern, pattern);
    return std::regex_replace(text, it->second, replacement);
}

bool replaceBareIdentifierToken(std::string& text,
                                std::string_view name,
                                const std::string& replacement,
                                bool rejectCallForm) {
    if (name.empty() || text.find(name) == std::string::npos) {
        return false;
    }
    bool changed = false;
    for (size_t pos = 0; pos < text.size();) {
        size_t found = text.find(name, pos);
        if (found == std::string::npos) {
            break;
        }
        if (found > 0 && (isIdentPart(text[found - 1]) || text[found - 1] == '.')) {
            pos = found + name.size();
            continue;
        }
        size_t after = found + name.size();
        if (after < text.size() && isIdentPart(text[after])) {
            pos = after;
            continue;
        }
        size_t afterSpace = after;
        while (afterSpace < text.size() && std::isspace(static_cast<unsigned char>(text[afterSpace]))) {
            ++afterSpace;
        }
        if (rejectCallForm && afterSpace < text.size() && text[afterSpace] == '(') {
            pos = after;
            continue;
        }
        text.replace(found, name.size(), replacement);
        pos = found + replacement.size();
        changed = true;
    }
    return changed;
}

bool replaceBareCallToken(std::string& text, std::string_view name, const std::string& replacementWithOpenParen) {
    if (name.empty() || text.find(name) == std::string::npos) {
        return false;
    }
    bool changed = false;
    for (size_t pos = 0; pos < text.size();) {
        size_t found = text.find(name, pos);
        if (found == std::string::npos) {
            break;
        }
        if (found > 0 && (isIdentPart(text[found - 1]) || text[found - 1] == '.')) {
            pos = found + name.size();
            continue;
        }
        size_t after = found + name.size();
        if (after < text.size() && isIdentPart(text[after])) {
            pos = after;
            continue;
        }
        size_t open = after;
        while (open < text.size() && std::isspace(static_cast<unsigned char>(text[open]))) {
            ++open;
        }
        if (open >= text.size() || text[open] != '(') {
            pos = after;
            continue;
        }
        text.replace(found, open + 1 - found, replacementWithOpenParen);
        pos = found + replacementWithOpenParen.size();
        changed = true;
    }
    return changed;
}

std::vector<std::string> splitCommaList(const std::string& text) {
    return splitCommaListRespectingQuotes(text);
}

std::string joinParams(const std::vector<std::string>& params) {
    std::string out;
    for (size_t i = 0; i < params.size(); ++i) {
        if (i > 0) {
            out += ", ";
        }
        out += params[i];
    }
    return out.empty() ? "nothing" : out;
}

std::string joinArgs(const std::vector<std::string>& args) {
    std::string out;
    for (size_t i = 0; i < args.size(); ++i) {
        if (i > 0) {
            out += ", ";
        }
        out += args[i];
    }
    return out;
}

std::vector<ParamDecl> parseCodegenParamList(const std::string& text, SourceLocation loc) {
    std::vector<ParamDecl> params;
    for (auto part : splitCommaList(text)) {
        part = trim(part);
        if (part.empty() || part == "nothing") {
            continue;
        }
        std::istringstream in(part);
        std::string type;
        std::string name;
        in >> type >> name;
        if (!type.empty() && !name.empty()) {
            params.push_back(ParamDecl{TypeRef{type, loc, false}, name, loc});
        }
    }
    return params;
}

std::string compactSetSpacing(std::string line) {
    line = std::regex_replace(line, std::regex(R"(\s*=\s*)"), "=");
    line = std::regex_replace(line, std::regex(R"(\s*\+\s*)"), "+");
    return line;
}

size_t findMatchingParen(const std::string& text, size_t open) {
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    int depth = 0;
    for (size_t i = open; i < text.size(); ++i) {
        char c = text[i];
        if (!inString && !inRaw && i + 1 < text.size() && c == '/' && text[i + 1] == '/') {
            return std::string::npos;
        }
        if (inString) {
            if (escaped) {
                escaped = false;
            } else if (c == '\\') {
                escaped = true;
            } else if (c == '"') {
                inString = false;
            }
            continue;
        }
        if (inRaw) {
            if (c == '\'') {
                inRaw = false;
            }
            continue;
        }
        if (c == '"') {
            inString = true;
        } else if (c == '\'') {
            inRaw = true;
        } else if (c == '(') {
            ++depth;
        } else if (c == ')') {
            --depth;
            if (depth == 0) {
                return i;
            }
        }
    }
    return std::string::npos;
}

std::optional<size_t> findTopLevelAssignment(const std::string& text) {
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    int parens = 0;
    int brackets = 0;
    for (size_t i = 0; i < text.size(); ++i) {
        char c = text[i];
        if (!inString && !inRaw && i + 1 < text.size() && c == '/' && text[i + 1] == '/') {
            return std::nullopt;
        }
        if (inString) {
            if (escaped) {
                escaped = false;
            } else if (c == '\\') {
                escaped = true;
            } else if (c == '"') {
                inString = false;
            }
            continue;
        }
        if (inRaw) {
            if (c == '\'') {
                inRaw = false;
            }
            continue;
        }
        if (c == '"') {
            inString = true;
        } else if (c == '\'') {
            inRaw = true;
        } else if (c == '(') {
            ++parens;
        } else if (c == ')' && parens > 0) {
            --parens;
        } else if (c == '[') {
            ++brackets;
        } else if (c == ']' && brackets > 0) {
            --brackets;
        } else if (c == '=' && parens == 0 && brackets == 0) {
            char prev = i > 0 ? text[i - 1] : '\0';
            char next = i + 1 < text.size() ? text[i + 1] : '\0';
            if (prev != '=' && prev != '!' && prev != '<' && prev != '>' && next != '=') {
                return i;
            }
        }
    }
    return std::nullopt;
}

struct ParsedLocalDeclarator {
    std::string type;
    std::string name;
    bool constant = false;
    bool array = false;
    std::vector<int> dimensions;
    std::string initializer;
};

struct ParsedLocalDeclList {
    bool matched = false;
    std::vector<ParsedLocalDeclarator> decls;
};

ParsedLocalDeclList parseLocalDeclListUncached(const std::string& line) {
    ParsedLocalDeclList result;
    std::string t = trim(stripLineCommentPreservingLiterals(line));
    if (startsWithWord(t, "local")) {
        t = trim(std::string_view(t).substr(5));
    }
    bool constant = false;
    if (startsWithWord(t, "constant")) {
        constant = true;
        t = trim(std::string_view(t).substr(8));
    }
    size_t typeEnd = 0;
    while (typeEnd < t.size() &&
           (std::isalnum(static_cast<unsigned char>(t[typeEnd])) || t[typeEnd] == '_' || t[typeEnd] == '$')) {
        ++typeEnd;
    }
    if (typeEnd == 0) {
        return result;
    }
    std::string type = t.substr(0, typeEnd);
    std::string rest = trim(std::string_view(t).substr(typeEnd));
    bool arrayForAll = false;
    if (startsWithWord(rest, "array")) {
        arrayForAll = true;
        rest = trim(std::string_view(rest).substr(5));
    }
    if (rest.empty()) {
        return result;
    }
    for (auto part : splitCommaList(rest)) {
        part = trim(part);
        if (part.empty()) {
            continue;
        }
        std::string declarator = part;
        std::string initializer;
        if (auto eq = findTopLevelAssignment(part)) {
            declarator = trim(std::string_view(part).substr(0, *eq));
            initializer = trim(std::string_view(part).substr(*eq + 1));
        }
        bool array = arrayForAll;
        if (startsWithWord(declarator, "array")) {
            array = true;
            declarator = trim(std::string_view(declarator).substr(5));
        }
        size_t nameEnd = 0;
        while (nameEnd < declarator.size() &&
               (std::isalnum(static_cast<unsigned char>(declarator[nameEnd])) ||
                declarator[nameEnd] == '_' || declarator[nameEnd] == '$')) {
            ++nameEnd;
        }
        if (nameEnd == 0) {
            continue;
        }
        std::string name = declarator.substr(0, nameEnd);
        std::string suffix = trim(std::string_view(declarator).substr(nameEnd));
        std::vector<int> dims = parseArrayDimensions(suffix);
        if (!dims.empty()) {
            array = true;
        }
        result.decls.push_back(ParsedLocalDeclarator{type, name, constant, array, std::move(dims), initializer});
    }
    result.matched = !result.decls.empty();
    return result;
}

ParsedLocalDeclList parseLocalDeclList(const std::string& line) {
    static std::unordered_map<std::string, ParsedLocalDeclList, TransparentStringHash, TransparentStringEqual> cache = [] {
        std::unordered_map<std::string, ParsedLocalDeclList, TransparentStringHash, TransparentStringEqual> entries;
        entries.reserve(20000);
        return entries;
    }();
    if (auto it = cache.find(line); it != cache.end()) {
        return it->second;
    }
    ParsedLocalDeclList parsed = parseLocalDeclListUncached(line);
    cache.emplace(line, parsed);
    return parsed;
}

std::string receiverFromPossibleAssignment(const std::string& rawLine) {
    std::string line = trim(rawLine);
    if (auto eq = findTopLevelAssignment(line)) {
        std::string lhs = trim(std::string_view(line).substr(0, *eq));
        if (startsWithWord(lhs, "set")) {
            lhs = trim(std::string_view(lhs).substr(3));
        }
        if (isLocalDecl(lhs)) {
            if (startsWithWord(lhs, "constant")) {
                lhs = trim(std::string_view(lhs).substr(8));
            }
            std::istringstream in(lhs);
            std::string type;
            std::string name;
            in >> type >> name;
            if (name == "array") {
                in >> name;
            }
            return name;
        }
        return lhs;
    }
    return {};
}

std::string receiverFromStandaloneExpression(const std::string& rawLine) {
    std::string line = removeSemicolon(trim(rawLine));
    if (startsWithWord(line, "call")) {
        line = trim(std::string_view(line).substr(4));
    }
    if (line.empty() || line.front() == '.' || line == "{" || line == "}") {
        return {};
    }
    static const std::vector<std::string> blocked = {
        "if", "elseif", "else", "loop", "exitwhen", "return", "set", "local",
        "function", "endfunction", "method", "endmethod"
    };
    for (const auto& word : blocked) {
        if (startsWithWord(line, word)) {
            return {};
        }
    }
    size_t open = line.find('(');
    if (open == std::string::npos) {
        return {};
    }
    size_t close = findMatchingParen(line, open);
    if (close == std::string::npos) {
        return {};
    }
    size_t tail = close + 1;
    while (tail < line.size() && std::isspace(static_cast<unsigned char>(line[tail]))) {
        ++tail;
    }
    if (tail != line.size()) {
        return {};
    }
    return line;
}

std::string receiverFromBareExpression(const std::string& rawLine) {
    std::string line = removeSemicolon(trim(rawLine));
    if (line.empty() || line.front() == '.') {
        return {};
    }
    static const std::vector<std::string> blocked = {
        "if", "elseif", "else", "loop", "exitwhen", "return", "set", "local",
        "function", "endfunction", "method", "endmethod", "call"
    };
    for (const auto& word : blocked) {
        if (startsWithWord(line, word)) {
            return {};
        }
    }
    if (!isIdentStart(line.front())) {
        return {};
    }
    auto consumeIdentifier = [&](size_t& cursor) -> bool {
        if (cursor >= line.size() || !isIdentStart(line[cursor])) {
            return false;
        }
        ++cursor;
        while (cursor < line.size() && isIdentPart(line[cursor])) {
            ++cursor;
        }
        return true;
    };

    size_t pos = 0;
    if (!consumeIdentifier(pos)) {
        return {};
    }
    while (pos < line.size()) {
        while (pos < line.size() && std::isspace(static_cast<unsigned char>(line[pos]))) {
            ++pos;
        }
        if (pos >= line.size()) {
            return line;
        }
        if (line[pos] == '.') {
            ++pos;
            while (pos < line.size() && std::isspace(static_cast<unsigned char>(line[pos]))) {
                ++pos;
            }
            if (!consumeIdentifier(pos)) {
                return {};
            }
            continue;
        }
        if (line[pos] != '[') {
            return {};
        }
        size_t close = findMatchingBracketOutsideProtected(line, pos);
        if (close == std::string::npos) {
            return {};
        }
        pos = close + 1;
    }
    return line;
}

bool hasUnclosedContinuation(const std::string& rawLine) {
    std::string line = trim(stripLineCommentPreservingLiterals(rawLine));
    if (line.empty()) {
        return false;
    }
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    int parens = 0;
    int brackets = 0;
    for (char c : line) {
        if (inString) {
            if (escaped) {
                escaped = false;
            } else if (c == '\\') {
                escaped = true;
            } else if (c == '"') {
                inString = false;
            }
            continue;
        }
        if (inRaw) {
            if (c == '\'') {
                inRaw = false;
            }
            continue;
        }
        if (c == '"') {
            inString = true;
        } else if (c == '\'') {
            inRaw = true;
        } else if (c == '(') {
            ++parens;
        } else if (c == ')' && parens > 0) {
            --parens;
        } else if (c == '[') {
            ++brackets;
        } else if (c == ']' && brackets > 0) {
            --brackets;
        }
    }
    auto endsWithWordToken = [&](std::string_view word) {
        if (line.size() < word.size()) {
            return false;
        }
        size_t pos = line.size() - word.size();
        return line.compare(pos, word.size(), word) == 0 && (pos == 0 || !isIdentPart(line[pos - 1]));
    };
    return parens > 0 || brackets > 0 || line.back() == ',' || line.back() == '+' ||
           line.back() == '-' || line.back() == '*' || line.back() == '/' ||
           line.ends_with("&&") || line.ends_with("||") || endsWithWordToken("and") || endsWithWordToken("or");
}

bool startsWithZincContinuationOperator(const std::string& rawLine) {
    std::string line = trim(stripLineCommentPreservingLiterals(rawLine));
    return !line.empty() && (line.front() == '+' || line.front() == '-' || line.front() == '*' || line.front() == '/' ||
                             line.rfind("&&", 0) == 0 || line.rfind("||", 0) == 0 ||
                             startsWithWord(line, "and") || startsWithWord(line, "or"));
}

std::vector<std::string> joinZincContinuationLines(const std::vector<std::string>& lines) {
    std::vector<std::string> out;
    std::string current;
    for (size_t index = 0; index < lines.size(); ++index) {
        const auto& raw = lines[index];
        std::string t = trim(raw);
        if (current.empty()) {
            current = t;
        } else {
            current += " " + t;
        }
        bool nextContinues = false;
        for (size_t lookahead = index + 1; lookahead < lines.size(); ++lookahead) {
            std::string next = trim(lines[lookahead]);
            if (next.empty()) {
                continue;
            }
            nextContinues = startsWithZincContinuationOperator(next);
            break;
        }
        if (!hasUnclosedContinuation(current) && !nextContinues) {
            if (!current.empty()) {
                out.push_back(current);
            }
            current.clear();
        }
    }
    if (!current.empty()) {
        out.push_back(current);
    }
    return out;
}

std::vector<std::string> expandLeadingDotChains(const std::vector<std::string>& lines) {
    std::vector<std::string> out;
    std::string receiver;
    for (size_t index = 0; index < lines.size(); ++index) {
        const auto& raw = lines[index];
        std::string t = trim(raw);
        if (t.rfind("//", 0) == 0) {
            out.push_back(raw);
            continue;
        }
        if (!t.empty() && t.front() == '.' && !receiver.empty()) {
            out.push_back(receiver + t);
            continue;
        }
        std::string nextReceiver = receiverFromPossibleAssignment(t);
        bool standaloneReceiver = false;
        bool bareReceiverOnly = false;
        if (nextReceiver.empty()) {
            nextReceiver = receiverFromStandaloneExpression(t);
            standaloneReceiver = !nextReceiver.empty();
        }
        if (nextReceiver.empty()) {
            nextReceiver = receiverFromBareExpression(t);
            bareReceiverOnly = !nextReceiver.empty();
        }
        bool nextIsLeadingDot = false;
        for (size_t lookahead = index + 1; lookahead < lines.size(); ++lookahead) {
            std::string next = trim(lines[lookahead]);
            if (next.empty() || next.rfind("//", 0) == 0) {
                continue;
            }
            nextIsLeadingDot = next.front() == '.';
            break;
        }
        if (standaloneReceiver && nextIsLeadingDot) {
            std::string chained = removeSemicolon(t);
            size_t lookahead = index + 1;
            for (; lookahead < lines.size(); ++lookahead) {
                std::string next = trim(lines[lookahead]);
                if (next.empty() || next.rfind("//", 0) == 0) {
                    break;
                }
                if (next.empty() || next.front() != '.') {
                    break;
                }
                chained += removeSemicolon(next);
            }
            out.push_back(chained);
            index = lookahead - 1;
            receiver.clear();
            continue;
        }
        if (!(bareReceiverOnly && nextIsLeadingDot)) {
            out.push_back(raw);
        }
        if (!nextReceiver.empty()) {
            receiver = nextReceiver;
        } else if (!t.empty() && t.front() != '.' && t.rfind("//", 0) != 0) {
            receiver.clear();
        }
    }
    return out;
}

std::vector<std::string> splitZincStructuralLines(const std::vector<std::string>& lines) {
    std::vector<std::string> out;
    for (const auto& raw : lines) {
        std::string current;
        bool inString = false;
        bool inRaw = false;
        bool escaped = false;
        int parens = 0;
        int brackets = 0;
        auto flush = [&]() {
            std::string t = trim(current);
            if (!t.empty()) {
                out.push_back(t);
            }
            current.clear();
        };
        for (size_t i = 0; i < raw.size(); ++i) {
            char c = raw[i];
            if (!inString && !inRaw && i + 1 < raw.size() && c == '/' && raw[i + 1] == '/') {
                current += raw.substr(i);
                break;
            }
            if (inString) {
                current.push_back(c);
                if (escaped) {
                    escaped = false;
                } else if (c == '\\') {
                    escaped = true;
                } else if (c == '"') {
                    inString = false;
                }
                continue;
            }
            if (inRaw) {
                current.push_back(c);
                if (c == '\'') {
                    inRaw = false;
                }
                continue;
            }
            if (c == '"') {
                inString = true;
                current.push_back(c);
                continue;
            }
            if (c == '\'') {
                inRaw = true;
                current.push_back(c);
                continue;
            }
            if (c == '(') {
                ++parens;
            } else if (c == ')' && parens > 0) {
                --parens;
            } else if (c == '[') {
                ++brackets;
            } else if (c == ']' && brackets > 0) {
                --brackets;
            }
            if (parens == 0 && brackets == 0 && c == '{') {
                current.push_back(c);
                flush();
                continue;
            }
            if (parens == 0 && brackets == 0 && c == '}') {
                flush();
                std::string close = "}";
                size_t j = i + 1;
                while (j < raw.size() && std::isspace(static_cast<unsigned char>(raw[j]))) {
                    ++j;
                }
                if (j + 4 <= raw.size() && raw.compare(j, 4, "else") == 0 &&
                    (j + 4 == raw.size() || !isIdentPart(raw[j + 4]))) {
                    close += " else";
                    j += 4;
                    while (j < raw.size() && std::isspace(static_cast<unsigned char>(raw[j]))) {
                        ++j;
                    }
                    if (j < raw.size()) {
                        close.push_back(' ');
                    }
                    while (j < raw.size()) {
                        char ec = raw[j];
                        close.push_back(ec);
                        if (ec == '{') {
                            break;
                        }
                        ++j;
                    }
                    i = j;
                }
                out.push_back(trim(close));
                continue;
            }
            if (parens == 0 && brackets == 0 && c == ';') {
                current.push_back(c);
                flush();
                continue;
            }
            current.push_back(c);
        }
        flush();
    }
    return out;
}

struct RangeForParts {
    bool ok = false;
    std::string start;
    std::string var;
    std::string upperOp;
    std::string end;
};

RangeForParts parseRangeForParts(const std::string& content) {
    RangeForParts result;
    std::regex pattern(R"(^\s*(.+?)\s*(?:<=|<)\s*([A-Za-z_$][A-Za-z0-9_$]*)\s*(<=|<)\s*(.+?)\s*$)");
    std::smatch match;
    if (std::regex_match(content, match, pattern)) {
        result.ok = true;
        result.start = trim(match[1].str());
        result.var = match[2].str();
        result.upperOp = match[3].str();
        result.end = trim(match[4].str());
    }
    return result;
}

bool findMethodCallSpan(const std::string& text,
                        const std::string& method,
                        size_t& receiverStart,
                        size_t& dotPos,
                        size_t& open,
                        size_t& close) {
    std::string needle = "." + method;
    size_t search = 0;
    while ((dotPos = text.find(needle, search)) != std::string::npos) {
        size_t methodEnd = dotPos + needle.size();
        size_t p = methodEnd;
        while (p < text.size() && std::isspace(static_cast<unsigned char>(text[p]))) {
            ++p;
        }
        if (p >= text.size() || text[p] != '(') {
            search = methodEnd;
            continue;
        }
        size_t left = dotPos;
        while (left > 0 && std::isspace(static_cast<unsigned char>(text[left - 1]))) {
            --left;
        }
        size_t start = left;
        int brackets = 0;
        while (start > 0) {
            char c = text[start - 1];
            if (c == ']') {
                ++brackets;
            } else if (c == '[' && brackets > 0) {
                --brackets;
            }
            if (brackets == 0 &&
                !(std::isalnum(static_cast<unsigned char>(c)) || c == '_' || c == '$' || c == '.' || c == ']' || c == '[')) {
                break;
            }
            --start;
        }
        close = findMatchingParen(text, p);
        if (close == std::string::npos) {
            search = methodEnd;
            continue;
        }
        receiverStart = start;
        open = p;
        return true;
    }
    return false;
}

std::string sanitizeName(std::string text) {
    for (char& c : text) {
        if (!(std::isalnum(static_cast<unsigned char>(c)) || c == '_')) {
            c = '_';
        }
    }
    return text;
}

std::vector<int> parseArrayDimensions(std::string_view suffix) {
    std::vector<int> dims;
    size_t pos = 0;
    while (pos < suffix.size()) {
        while (pos < suffix.size() && std::isspace(static_cast<unsigned char>(suffix[pos]))) {
            ++pos;
        }
        if (pos >= suffix.size() || suffix[pos] != '[') {
            break;
        }
        size_t close = suffix.find(']', pos + 1);
        if (close == std::string_view::npos) {
            break;
        }
        std::string dimText = trim(suffix.substr(pos + 1, close - pos - 1));
        int dim = 0;
        if (!dimText.empty()) {
            try {
                dim = std::stoi(dimText);
            } catch (...) {
                dim = 0;
            }
        }
        dims.push_back(dim);
        pos = close + 1;
    }
    return dims;
}

int arrayFlatSize(const std::vector<int>& dims) {
    int total = 1;
    bool any = false;
    for (int dim : dims) {
        if (dim <= 0) {
            return 0;
        }
        total *= dim;
        any = true;
    }
    return any ? total : 0;
}

std::string stripLineCommentPreservingLiterals(const std::string& line) {
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    for (size_t i = 0; i + 1 < line.size(); ++i) {
        char c = line[i];
        if (inString) {
            if (escaped) {
                escaped = false;
            } else if (c == '\\') {
                escaped = true;
            } else if (c == '"') {
                inString = false;
            }
            continue;
        }
        if (inRaw) {
            if (c == '\'') {
                inRaw = false;
            }
            continue;
        }
        if (c == '"') {
            inString = true;
            continue;
        }
        if (c == '\'') {
            inRaw = true;
            continue;
        }
        if (c == '/' && line[i + 1] == '/') {
            return line.substr(0, i);
        }
    }
    return line;
}

std::string stripRedundantOuterParens(std::string text) {
    text = trim(text);
    while (text.size() >= 2 && text.front() == '(') {
        size_t close = findMatchingParen(text, 0);
        if (close == std::string::npos) {
            break;
        }
        std::string trailing = trim(std::string_view(text).substr(close + 1));
        if (!trailing.empty()) {
            break;
        }
        text = trim(std::string_view(text).substr(1, close - 1));
    }
    return text;
}

std::string arrayIndexTerm(const std::string& index) {
    return "(" + stripRedundantOuterParens(index) + ")";
}

std::string composeFlatIndex(const std::vector<std::string>& indexes,
                             const std::vector<int>& dimensions,
                             size_t startIndex) {
    if (startIndex >= indexes.size()) {
        return {};
    }
    std::string expr = arrayIndexTerm(indexes[startIndex]);
    for (size_t i = startIndex + 1; i < indexes.size() && i - startIndex < dimensions.size(); ++i) {
        int dim = dimensions[i - startIndex];
        expr = "(" + expr + " * " + std::to_string(dim) + " + " + arrayIndexTerm(indexes[i]) + ")";
    }
    return expr;
}

size_t findMatchingBracketOutsideProtected(const std::string& text, size_t open) {
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    int depth = 0;
    for (size_t i = open; i < text.size(); ++i) {
        char c = text[i];
        if (!inString && !inRaw && i + 1 < text.size() && c == '/' && text[i + 1] == '/') {
            return std::string::npos;
        }
        if (inString) {
            if (escaped) {
                escaped = false;
            } else if (c == '\\') {
                escaped = true;
            } else if (c == '"') {
                inString = false;
            }
            continue;
        }
        if (inRaw) {
            if (c == '\'') {
                inRaw = false;
            }
            continue;
        }
        if (c == '"') {
            inString = true;
        } else if (c == '\'') {
            inRaw = true;
        } else if (c == '[') {
            ++depth;
        } else if (c == ']') {
            --depth;
            if (depth == 0) {
                return i;
            }
        }
    }
    return std::string::npos;
}

struct ArrayDeclRewrite {
    bool matched = false;
    std::string name;
    std::vector<int> dimensions;
    std::string line;
};

ArrayDeclRewrite rewriteMultiDimDeclarationLine(const std::string& line) {
    std::string code = stripLineCommentPreservingLiterals(line);
    std::string comment;
    if (code.size() < line.size()) {
        comment = line.substr(code.size());
    }
    std::string leading = line.substr(0, line.find_first_not_of(" \t") == std::string::npos ? 0 : line.find_first_not_of(" \t"));
    std::string t = trim(code);
    if (t.empty()) {
        return {};
    }
    bool hasLocal = false;
    if (startsWithWord(t, "local")) {
        hasLocal = true;
        t = trim(std::string_view(t).substr(5));
    }
    bool hasConstant = false;
    if (startsWithWord(t, "constant")) {
        hasConstant = true;
        t = trim(std::string_view(t).substr(8));
    }
    size_t assign = t.find('=');
    std::string declPart = assign == std::string::npos ? t : trim(std::string_view(t).substr(0, assign));
    std::string init = assign == std::string::npos ? "" : trim(std::string_view(t).substr(assign + 1));
    std::istringstream in(declPart);
    std::string type;
    std::string name;
    in >> type >> name;
    bool arrayWord = false;
    if (name == "array") {
        in >> name;
        arrayWord = true;
    }
    if (type.empty() || name.empty()) {
        return {};
    }
    size_t bracket = name.find('[');
    std::string suffix;
    if (bracket != std::string::npos) {
        suffix = name.substr(bracket);
        name = name.substr(0, bracket);
    } else {
        std::string rest;
        std::getline(in, rest);
        suffix = trim(rest);
    }
    std::vector<int> dims = parseArrayDimensions(suffix);
    if (dims.empty()) {
        return {};
    }
    ArrayDeclRewrite result;
    result.matched = true;
    result.name = name;
    result.dimensions = std::move(dims);
    result.line = leading;
    if (hasLocal) {
        result.line += "local ";
    }
    if (hasConstant) {
        result.line += "constant ";
    }
    result.line += type + " array " + name;
    if (!init.empty()) {
        result.line += "=" + init;
    }
    if (!comment.empty()) {
        result.line += comment;
    }
    (void)arrayWord;
    return result;
}

} // namespace

Phase1Codegen::Phase1Codegen(Diagnostics& diagnostics, CodegenOptions options)
    : diagnostics_(diagnostics), options_(options) {}

Phase1Codegen::BodyMode Phase1Codegen::bodyModeForFunction(const Decl& decl) const {
    return decl.mode == SyntaxMode::Zinc ? BodyMode::Zinc : BodyMode::JassLike;
}

Phase1Codegen::BodyMode Phase1Codegen::bodyModeForMethod(const MethodDecl& method) const {
    return method.mode == SyntaxMode::Zinc ? BodyMode::Zinc : BodyMode::JassLike;
}

Phase1Codegen::BodyMode Phase1Codegen::bodyModeForGenerated(GeneratedKind) const {
    return BodyMode::Generated;
}

void Phase1Codegen::countBodyMode(BodyMode mode, bool isMethod) const {
    if (mode == BodyMode::Generated) {
        ++performanceCounters_.bodyModeGeneratedBodies;
        return;
    }
    if (mode == BodyMode::Zinc) {
        if (isMethod) {
            ++performanceCounters_.bodyModeZincMethods;
        } else {
            ++performanceCounters_.bodyModeZincFunctions;
        }
        return;
    }
    if (isMethod) {
        ++performanceCounters_.bodyModeJassLikeMethods;
    } else {
        ++performanceCounters_.bodyModeJassLikeFunctions;
    }
}

bool Phase1Codegen::bodyCacheEnabled() const {
    return options_.experimentalIncrementalReuse && !options_.experimentalIncrementalCachePath.empty();
}

bool Phase1Codegen::bodyCacheSafe(const std::vector<std::string>& sourceLines, bool allowFunctionKeyword) const {
    for (const auto& raw : sourceLines) {
        std::string line = stripLineCommentPreservingLiterals(raw);
        if (!allowFunctionKeyword && line.find("function") != std::string::npos) {
            return false;
        }
        if (line.find(".execute") != std::string::npos ||
            line.find(".evaluate") != std::string::npos ||
            line.find("ExecuteFunc") != std::string::npos ||
            line.find("Condition(") != std::string::npos ||
            line.find("Filter(") != std::string::npos ||
            line.find("TriggerAddAction") != std::string::npos ||
            line.find("TriggerAddCondition") != std::string::npos) {
            return false;
        }
    }
    return true;
}

std::string Phase1Codegen::bodyCacheKey(const std::string& kind,
                                        const std::string& stableKey,
                                        BodyMode mode,
                                        const std::vector<std::string>& sourceLines) const {
    std::ostringstream key;
    key << "vjassc-phase23-body-cache-v3\n"
        << "kind=" << kind << "\n"
        << "stableKey=" << stableKey << "\n"
        << "signatureContext=" << bodyCacheSignatureContextKey() << "\n"
        << "mode=" << static_cast<int>(mode) << "\n"
        << "fast=" << (options_.fastMode ? "1" : "0") << "\n"
        << "warn=" << (options_.warnMode ? "1" : "0") << "\n";
    for (const auto& line : sourceLines) {
        key << line << "\n";
    }
    return decimalHash(fnv1a64(key.str()));
}

std::string Phase1Codegen::bodyCacheSignatureContextKey() const {
    if (!bodyCacheSignatureContextKeyCache_.empty()) {
        return bodyCacheSignatureContextKeyCache_;
    }
    std::ostringstream ctx;
    ctx << "phase23-body-signature-context-v1\n";
    for (const auto& fn : functions_) {
        ctx << "fn|" << fn.sourceName << "|" << fn.finalName << "|" << fn.signature.returnType << "|"
            << (fn.isStaticMethod ? "static" : "plain") << "|" << static_cast<int>(fn.generatedKind);
        for (const auto& param : fn.signature.paramTypes) {
            ctx << "|p:" << param;
        }
        ctx << "\n";
    }
    for (const auto& info : structs_) {
        ctx << "struct|" << info.originalName << "|" << info.generatedName << "|" << info.prefix << "|"
            << info.typeId << "|" << (info.isArrayStruct ? "array" : "plain") << "\n";
        for (const auto& field : info.fields) {
            ctx << "field|" << field.name << "|" << field.generatedName << "|" << field.typeName << "|"
                << (field.isStatic ? "static" : "member") << "|" << (field.isArray ? "array" : "scalar") << "|"
                << field.fixedArraySize;
            for (int dimension : field.arrayDimensions) {
                ctx << "|d:" << dimension;
            }
            ctx << "\n";
        }
        for (const auto& method : info.methods) {
            ctx << "method|" << method.name << "|" << method.generatedName << "|"
                << (method.isStatic ? "static" : "member") << "|" << (method.isOnDestroy ? "destroy" : "") << "|"
                << (method.isOnInit ? "init" : "");
            if (method.decl) {
                ctx << "|returns:" << method.decl->returnType.name;
                for (const auto& param : method.decl->params) {
                    ctx << "|p:" << param.type.name;
                }
            }
            ctx << "\n";
        }
    }
    for (const auto& iface : functionInterfaces_) {
        ctx << "iface|" << iface.sourceName << "|" << iface.finalName << "|" << iface.signature.returnType << "|"
            << (iface.syntheticFunctionObject ? "synthetic" : "declared");
        for (const auto& param : iface.signature.paramTypes) {
            ctx << "|p:" << param;
        }
        ctx << "\n";
        for (const auto& target : iface.targets) {
            ctx << "target|" << target.finalName << "|" << target.id << "|"
                << (target.needsCondition ? "condition" : "") << "|" << (target.needsAction ? "action" : "") << "\n";
        }
    }
    bodyCacheSignatureContextKeyCache_ = decimalHash(fnv1a64(ctx.str()));
    return bodyCacheSignatureContextKeyCache_;
}

bool Phase1Codegen::loadBodyCache(const std::string& key, std::vector<std::string>& lines) const {
    if (!bodyCacheEnabled()) {
        return false;
    }
    ++performanceCounters_.bodyCacheLookups;
    std::filesystem::path path = options_.experimentalIncrementalCachePath / "body-v1" / ("body-" + key + ".jbody");
    std::ifstream in(path, std::ios::binary);
    if (!in) {
        ++performanceCounters_.bodyCacheMisses;
        return false;
    }
    lines.clear();
    std::string line;
    while (std::getline(in, line)) {
        if (!line.empty() && line.back() == '\r') {
            line.pop_back();
        }
        lines.push_back(line);
    }
    ++performanceCounters_.bodyCacheHits;
    performanceCounters_.bodyCacheReusedLines += lines.size();
    return true;
}

void Phase1Codegen::storeBodyCache(const std::string& key, const std::vector<std::string>& lines) const {
    if (!bodyCacheEnabled()) {
        return;
    }
    std::filesystem::path dir = options_.experimentalIncrementalCachePath / "body-v1";
    std::error_code ec;
    std::filesystem::create_directories(dir, ec);
    if (ec) {
        return;
    }
    std::filesystem::path path = dir / ("body-" + key + ".jbody");
    std::ofstream out(path, std::ios::binary);
    if (!out) {
        return;
    }
    for (const auto& line : lines) {
        out << line << '\n';
    }
    ++performanceCounters_.bodyCacheStores;
    performanceCounters_.bodyCacheStoredLines += lines.size();
}

void Phase1Codegen::recordFunctionDependency(const std::string& from, const std::string& to) const {
    if (from.empty() || to.empty() || from == to || functionIndexByName_.find(to) == functionIndexByName_.end()) {
        return;
    }
    if (recordedFunctionDependencyEdges_.insert(from + "\n" + to).second) {
        ++performanceCounters_.functionDependencyRecordedEdges;
    }
}

CodegenResult Phase1Codegen::generate(const Program& program) {
    auto recordPass = [this](const std::string& name, auto&& fn) {
        auto start = std::chrono::steady_clock::now();
        fn();
        auto end = std::chrono::steady_clock::now();
        passTimings_[name] += std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();
    };
    passTimings_.clear();
    for (const std::string& name : {
             "collectStructs",
             "collectFunctions",
             "collectFunctionInterfaces",
             "collectFunctionObjectInterfaces",
             "lowerLambdas",
             "lowerZincBodies",
             "lowerStructExpressions",
             "methodChainLowering",
             "functionOrdering",
             "emitGlobals",
             "emitFunctions",
             "emitFunctions.total",
             "emitFunctions.jassFunctions",
             "emitFunctions.zincFunctions",
             "emitFunctions.lambdaEmit",
             "emitFunctions.functionInterfaceRuntime",
             "emitFunctions.bodyLowering",
             "emitStructSupport",
             "emitStructSupport.total",
             "emitStructSupport.generatedHelpers",
             "emitStructSupport.sourceMethods",
             "emitStructSupport.methodBodyLowering",
             "emitStructSupport.generatedBodyLowering",
             "emitStructSupport.fixedArraySupport",
             "emitStructSupport.lifecycleSupport",
             "emitFunctionInterfaceRuntime",
             "lowering.rewriteReceiverChains",
             "lowering.rewriteStructExpression",
             "lowering.rewriteArrayAccesses",
             "lowering.rewriteCallArguments",
             "lowering.lowerExpression",
             "lowering.rewriteFunctionNames",
             "lowering.rewriteForContainer",
             "finalOutputValidationPrep",
             "validation.syntaxLiteForwardScan",
             "validation.syntaxLiteResidueScan",
             "validation.finalOutputValidationPrep",
         }) {
        passTimings_[name] = 0;
    }
    if (program.hasUnsupported() && !options_.scanOnly) {
        diagnostics_.error(SourceLocation{}, "unsupported declarations prevent code generation");
        return makeResult(false);
    }
    recordPass("buildSymbols", [&]() { symbols_.build(program); });
    structs_.clear();
    structIndexByDecl_.clear();
    structIndexByName_.clear();
    globalArrayShapes_.clear();
    globalArrayShapeFirstChars_.fill(false);
    globalStructTypes_.clear();
    functionInterfaces_.clear();
    functionInterfaceIndexByName_.clear();
    functionObjectInterfaceIndexBySignature_.clear();
    functions_.clear();
    functionIndexByName_.clear();
    functionInterfaceParamTypesByFunction_.clear();
    functionArgumentLoweringCandidates_.clear();
    arrayStructIndexes_.clear();
    lambdas_.clear();
    bodyCacheSignatureContextKeyCache_.clear();
    processedZincFunctionBodies_.clear();
    processedZincMethodBodies_.clear();
    nextLambdaId_ = 1;
    lambdasCodeContext_ = 0;
    lambdasBoolexprContext_ = 0;
    lambdasFunctionInterfaceContext_ = 0;
    lambdasNativeCallbackContext_ = 0;
    lambdasMethodCallbackContext_ = 0;
    lambdasUnknownContext_ = 0;
    lambdasCapturing_ = 0;
    lambdasRejected_ = 0;
    lambdaContextStruct_ = nullptr;
    lambdaContextContainer_ = nullptr;
    functionInterfaceCalls_ = 0;
    functionObjectCalls_ = 0;
    functionInterfaceMaxEvaluateDepth_ = 0;
    performanceCounters_ = CodegenPerformanceCounters{};
    structLookupCache_.clear();
    structLookupCache_.reserve(12000);
    functionLookupCache_.clear();
    functionLookupCache_.reserve(12000);
    arrayStructReceiverCache_.clear();
    arrayStructReceiverCache_.reserve(12000);
    arrayRewriteCache_.clear();
    arrayRewriteCache_.reserve(20000);
    structDeallocateCache_.clear();
    lineTokenCache_.clear();
    lineTokenCache_.reserve(80000);
    recordedFunctionDependencyEdges_.clear();
    recordedFunctionDependencyEdges_.reserve(20000);
    lineFeatureCacheValid_ = false;
    structInitializers_.clear();
    mainFunction_ = nullptr;
    mainContainer_ = nullptr;
    recordPass("collectStructs", [&]() { collectStructs(program.decls, nullptr, nullptr); });
    for (size_t i = 0; i < structs_.size(); ++i) {
        if (structs_[i].isArrayStruct) {
            arrayStructIndexes_.push_back(i);
        }
    }
    structLookupCache_.clear();
    structLookupCache_.reserve(12000);
    arrayStructReceiverCache_.clear();
    arrayStructReceiverCache_.reserve(12000);
    arrayRewriteCache_.clear();
    arrayRewriteCache_.reserve(20000);
    structDeallocateCache_.clear();
    recordPass("collectFunctionInterfaces", [&]() { collectFunctionInterfaces(program.decls, nullptr); });
    recordPass("collectFunctions", [&]() { collectFunctions(program.decls, nullptr); });
    recordPass("collectFunctionObjectInterfaces", [&]() { collectFunctionObjectEvaluateInterfaces(program.decls, nullptr, nullptr); });
    if (bodyCacheEnabled()) {
        performanceCounters_.bodyCacheEnabled = 1;
    }
    functionLookupCache_.clear();
    functionLookupCache_.reserve(12000);
    LibraryGraph graph(diagnostics_);
    LibraryGraphResult graphResult;
    recordPass("functionOrderingPrep", [&]() { graphResult = graph.sort(program); });
    if (diagnostics_.hasErrors()) {
        return makeResult(false);
    }

    const size_t reserveBytes = std::max<size_t>(
        1 << 20,
        (program.stats.functions + functions_.size() + lambdas_.size()) * 512 +
            structs_.size() * 4096 +
            functionInterfaces_.size() * 2048 +
            program.stats.globalsBlocks * 4096);
    writer_.clear();
    writer_.reserve(reserveBytes);
    writer_.writeln("// Generated by vjassc phase5");
    writer_.writeln();
    recordPass("emitTypes", [&]() { emitTypesAndNatives(program, graphResult, DeclKind::TypeDecl); });
    recordPass("emitGlobals", [&]() { emitGlobals(program, graphResult); });
    writer_.writeln();
    recordPass("emitTypesAndNatives", [&]() { emitTypesAndNatives(program, graphResult, DeclKind::Native); });
    writer_.writeln();
    recordPass("emitFunctions", [&]() { emitFunctions(program, graphResult); });
    passTimings_["emitFunctions.total"] = passTimings_["emitFunctions"];

    return makeResult(!diagnostics_.hasErrors());
}

CodegenResult Phase1Codegen::makeResult(bool ok) {
    CodegenResult result;
    result.ok = ok;
    if (ok) {
        auto start = std::chrono::steady_clock::now();
        std::string rawOutput = writer_.take();
        result.output.reserve(rawOutput.size() + rawOutput.size() / 64);
        std::istringstream in(rawOutput);
        std::string line;
        while (std::getline(in, line)) {
            result.output += sanitizeGeneratedLine(line);
            result.output += '\n';
        }
        result.output = commentMissingInitTriggerCalls(std::move(result.output));
        auto sanitized = std::chrono::steady_clock::now();
        result.output = adaptFunctionInterfaceCallbacksBySignature(result.output);
        result.output = orderFunctionBlocksForPjass(result.output,
                                                    &performanceCounters_,
                                                    &recordedFunctionDependencyEdges_,
                                                    options_.experimentalRecordedOrder);
        auto ordered = std::chrono::steady_clock::now();
        long long sanitizeMs = std::chrono::duration_cast<std::chrono::milliseconds>(sanitized - start).count();
        long long orderingMs = std::chrono::duration_cast<std::chrono::milliseconds>(ordered - sanitized).count();
        passTimings_["sanitizeOutput"] += sanitizeMs;
        passTimings_["functionOrdering"] += orderingMs;
        if (!options_.fastMode) {
            passTimings_["finalOutputValidationPrep"] += sanitizeMs + orderingMs;
        }
    }
    result.lambdasLowered = lambdas_.size();
    result.lambdasCodeContext = lambdasCodeContext_;
    result.lambdasBoolexprContext = lambdasBoolexprContext_;
    result.lambdasFunctionInterfaceContext = lambdasFunctionInterfaceContext_;
    result.lambdasNativeCallbackContext = lambdasNativeCallbackContext_;
    result.lambdasMethodCallbackContext = lambdasMethodCallbackContext_;
    result.lambdasUnknownContext = lambdasUnknownContext_;
    result.lambdasCapturing = lambdasCapturing_;
    result.lambdasRejected = lambdasRejected_;
    result.lambdasGeneratedFunctions = lambdas_.size();
    for (const auto& iface : functionInterfaces_) {
        result.functionInterfaceTargets += iface.targets.size();
    }
    result.functionInterfaceCalls = functionInterfaceCalls_;
    result.functionObjectCalls = functionObjectCalls_;
    result.functionInterfaceMaxEvaluateDepth = functionInterfaceMaxEvaluateDepth_;
    result.functionInterfaceEvaluateTempLimit = functionInterfaceEvaluateTempLimit_;
    result.passTimings = passTimings_;
    if (options_.experimentalParallelLowering || options_.experimentalBodyJobsSingleThread) {
        performanceCounters_.experimentalBodyJobsSingleThread = options_.experimentalBodyJobsSingleThread ? 1 : 0;
        performanceCounters_.experimentalParallelWorkers = options_.parallelWorkers <= 1 ? 1 : options_.parallelWorkers;
        performanceCounters_.experimentalParallelJobs =
            performanceCounters_.bodyModeZincFunctions +
            performanceCounters_.bodyModeJassLikeFunctions +
            performanceCounters_.bodyModeZincMethods +
            performanceCounters_.bodyModeJassLikeMethods +
            performanceCounters_.bodyModeGeneratedBodies;
        performanceCounters_.experimentalParallelCompletedJobs = performanceCounters_.experimentalParallelJobs;
        performanceCounters_.experimentalParallelFailedJobs = 0;
        performanceCounters_.experimentalParallelQueueMs = 0;
        performanceCounters_.experimentalParallelWorkerTotalMs =
            static_cast<size_t>(std::max<long long>(0, passTimings_["emitStructSupport.methodBodyLowering"])) +
            static_cast<size_t>(std::max<long long>(0, passTimings_["lowerLambdas"]));
        performanceCounters_.experimentalParallelMergeMs = 0;
    }
    result.performanceCounters = performanceCounters_;
    if (options_.emitGeneratedEntityPlan) {
        result.generatedEntityPlanJson = emitGeneratedEntityPlanJson();
    }
    return result;
}

std::string Phase1Codegen::emitGeneratedEntityPlanJson() const {
    std::ostringstream out;
    out << "{\n"
        << "  \"phase\": 23,\n"
        << "  \"kind\": \"generated-entity-plan\",\n"
        << "  \"bodyJobModel\": \"phase23 body cache guarded deterministic body jobs\",\n"
        << "  \"lambdas\": [\n";
    for (size_t i = 0; i < lambdas_.size(); ++i) {
        const auto& lambda = lambdas_[i];
        out << "    {\"id\": " << (i + 1)
            << ", \"name\": ";
        writeJsonString(out, lambda.name);
        out << ", \"stableKey\": ";
        writeJsonString(out,
                        std::string("lambda:") +
                            (lambda.container ? lambda.container->name : "") + ":" +
                            (lambda.currentStruct ? lambda.currentStruct->generatedName : "") + ":" +
                            std::to_string(lambda.loc.line) + ":" +
                            std::to_string(lambda.loc.column) + ":" +
                            lambda.name);
        out << ", \"line\": " << lambda.loc.line
            << ", \"column\": " << lambda.loc.column
            << ", \"container\": ";
        writeJsonString(out, lambda.container ? lambda.container->name : "");
        out << ", \"struct\": ";
        writeJsonString(out, lambda.currentStruct ? lambda.currentStruct->generatedName : "");
        out << "}" << (i + 1 == lambdas_.size() ? "\n" : ",\n");
    }
        out << "  ],\n"
        << "  \"functionInterfaceTargets\": [\n";
    struct TargetRow {
        std::string interfaceName;
        std::string targetName;
        int id = 0;
        bool needsCondition = false;
        bool needsAction = false;
    };
    std::vector<TargetRow> targets;
    for (const auto& iface : functionInterfaces_) {
        for (const auto& target : iface.targets) {
            targets.push_back({iface.finalName, target.finalName, target.id, target.needsCondition, target.needsAction});
        }
    }
    std::sort(targets.begin(), targets.end(), [](const TargetRow& a, const TargetRow& b) {
        return std::tie(a.interfaceName, a.id, a.targetName) < std::tie(b.interfaceName, b.id, b.targetName);
    });
    for (size_t i = 0; i < targets.size(); ++i) {
        const auto& target = targets[i];
        out << "    {\"interface\": ";
        writeJsonString(out, target.interfaceName);
        out << ", \"id\": " << target.id
            << ", \"stableKey\": ";
        writeJsonString(out, "interface-target:" + target.interfaceName + ":" + target.targetName);
        out
            << ", \"target\": ";
        writeJsonString(out, target.targetName);
        out << ", \"needsCondition\": " << (target.needsCondition ? "true" : "false")
            << ", \"needsAction\": " << (target.needsAction ? "true" : "false")
            << "}" << (i + 1 == targets.size() ? "\n" : ",\n");
    }
    out << "  ],\n"
        << "  \"generatedSupport\": [\n";
    struct GeneratedRow {
        std::string kind;
        std::string sourceName;
        std::string finalName;
        std::string signatureHash;
    };
    auto kindName = [](GeneratedKind kind) -> const char* {
        switch (kind) {
        case GeneratedKind::StructAllocate:
            return "StructAllocate";
        case GeneratedKind::StructCreate:
            return "StructCreate";
        case GeneratedKind::StructDestroy:
            return "StructDestroy";
        case GeneratedKind::StructDeallocate:
            return "StructDeallocate";
        case GeneratedKind::StructOnDestroyWrapper:
            return "StructOnDestroyWrapper";
        case GeneratedKind::StructOnInitWrapper:
            return "StructOnInitWrapper";
        case GeneratedKind::FunctionInterfaceWrapper:
            return "FunctionInterfaceWrapper";
        case GeneratedKind::LambdaWrapper:
            return "LambdaWrapper";
        case GeneratedKind::CycleBridge:
            return "CycleBridge";
        case GeneratedKind::None:
            return "None";
        }
        return "None";
    };
    std::vector<GeneratedRow> generatedRows;
    for (const auto& fn : functions_) {
        if (fn.generatedKind == GeneratedKind::None) {
            continue;
        }
        std::ostringstream signature;
        signature << fn.signature.returnType << "(";
        for (size_t i = 0; i < fn.signature.paramTypes.size(); ++i) {
            if (i != 0) {
                signature << ",";
            }
            signature << fn.signature.paramTypes[i];
        }
        signature << ")";
        generatedRows.push_back({kindName(fn.generatedKind), fn.sourceName, fn.finalName, signature.str()});
    }
    std::sort(generatedRows.begin(), generatedRows.end(), [](const GeneratedRow& a, const GeneratedRow& b) {
        return std::tie(a.kind, a.sourceName, a.finalName) < std::tie(b.kind, b.sourceName, b.finalName);
    });
    for (size_t i = 0; i < generatedRows.size(); ++i) {
        const auto& row = generatedRows[i];
        out << "    {\"id\": " << (i + 1)
            << ", \"kind\": ";
        writeJsonString(out, row.kind);
        out << ", \"stableKey\": ";
        writeJsonString(out, "generated-support:" + row.kind + ":" + row.sourceName + ":" + row.signatureHash);
        out << ", \"sourceName\": ";
        writeJsonString(out, row.sourceName);
        out << ", \"finalName\": ";
        writeJsonString(out, row.finalName);
        out << ", \"signatureHash\": ";
        writeJsonString(out, row.signatureHash);
        out << "}" << (i + 1 == generatedRows.size() ? "\n" : ",\n");
    }
    out << "  ],\n"
        << "  \"wrappers\": [],\n"
        << "  \"bridges\": []\n"
        << "}\n";
    return out.str();
}

void Phase1Codegen::emitGlobals(const Program& program, const LibraryGraphResult& graph) {
    writer_.writeln("globals");
    writer_.indent();
    for (const Decl* library : graph.sortedLibraries) {
        writer_.writeln("constant boolean LIBRARY_" + library->name + "=true");
    }
    emitStructGlobals();
    emitFunctionInterfaceGlobals();
    for (const Decl* library : graph.sortedLibraries) {
        emitDeclGlobals(*library, nullptr);
    }
    for (const auto& decl : program.decls) {
        if (decl.kind != DeclKind::Library) {
            emitDeclGlobals(decl, nullptr);
        }
    }
    writer_.dedent();
    writer_.writeln("endglobals");
}

void Phase1Codegen::emitDeclGlobals(const Decl& decl, const Decl* container) {
    if (decl.kind == DeclKind::Struct || decl.kind == DeclKind::Module || decl.kind == DeclKind::FunctionInterface) {
        return;
    }
    if (decl.kind == DeclKind::GlobalBlock) {
        for (auto line : decl.lines) {
            std::string out = rewriteGlobalLine(line, container);
            if (!trim(out).empty()) {
                writer_.writeln(out);
            }
        }
        return;
    }
    const Decl* nextContainer = (decl.kind == DeclKind::Library || decl.kind == DeclKind::Scope) ? &decl : container;
    for (const auto& child : decl.children) {
        emitDeclGlobals(child, nextContainer);
    }
}

void Phase1Codegen::emitFunctionInterfaceGlobals() {
    for (const auto& iface : functionInterfaces_) {
        std::string prefix = interfaceGlobalPrefix(iface);
        writer_.writeln("trigger array " + prefix + "_trigger");
        for (size_t i = 0; i < iface.signature.paramTypes.size(); ++i) {
            writer_.writeln(rewriteTypeName(iface.signature.paramTypes[i], nullptr) + " " + prefix + "_arg" + std::to_string(i));
        }
        if (iface.signature.returnType != "nothing") {
            writer_.writeln(rewriteTypeName(iface.signature.returnType, nullptr) + " " + prefix + "_result");
            for (int i = 1; i <= 8; ++i) {
                writer_.writeln(rewriteTypeName(iface.signature.returnType, nullptr) + " " + prefix + "_tmp" + std::to_string(i));
            }
        }
    }
}

void Phase1Codegen::emitStructGlobals() {
    for (const auto& info : structs_) {
        emitStructGlobalBlock(info);
    }
}

void Phase1Codegen::emitStructGlobalBlock(const StructInfo& info) {
    if (!info.isArrayStruct) {
        writer_.writeln("integer si__" + info.generatedName + "_F=0");
        writer_.writeln("integer si__" + info.generatedName + "_I=0");
        writer_.writeln("integer array si__" + info.generatedName + "_V");
    }
    for (const auto& field : info.fields) {
        std::string typeName = rewriteTypeName(field.typeName, &info);
        if (field.isFixedArray && field.fixedArraySize > 0 && !field.isStatic) {
            writer_.writeln("constant integer s___" + info.generatedName + "_" + field.name + "_size=" + std::to_string(field.fixedArraySize));
            writer_.writeln(typeName + " array " + fixedArrayStorageName(info, field));
            writer_.writeln("integer array " + field.generatedName);
            continue;
        }
        if (field.isFixedArray && field.fixedArraySize > 0) {
            writer_.writeln("constant integer s___" + info.generatedName + "_" + field.name + "_size=" + std::to_string(field.fixedArraySize));
        }
        if (field.isStatic) {
            if (field.isArray) {
                writer_.writeln(typeName + " array " + field.generatedName);
            } else if (!field.decl->initializer.empty()) {
                writer_.writeln(typeName + " " + field.generatedName + "=" + rewriteStructExpression(field.decl->initializer, &info, {}));
            } else {
                writer_.writeln(typeName + " " + field.generatedName);
            }
        } else {
            writer_.writeln(typeName + " array " + field.generatedName);
        }
    }
}

void Phase1Codegen::emitTypesAndNatives(const Program& program, const LibraryGraphResult& graph, DeclKind kind) {
    for (const auto& decl : program.decls) {
        if (decl.kind != DeclKind::Library) {
            emitTypeOrNative(decl, kind);
        }
    }
    for (const Decl* library : graph.sortedLibraries) {
        emitTypeOrNative(*library, kind);
    }
}

void Phase1Codegen::emitTypeOrNative(const Decl& decl, DeclKind kind) {
    if (decl.kind == DeclKind::Module || decl.kind == DeclKind::FunctionInterface) {
        return;
    }
    if (decl.kind == kind && kind == DeclKind::TypeDecl) {
        writer_.writeln(trim(decl.lines.front()));
    } else if (decl.kind == kind && kind == DeclKind::Native) {
        std::string name = parseNativeName(decl.lines.front());
        if (emittedNatives_.insert(name).second) {
            writer_.writeln(trim(decl.lines.front()));
        } else {
            diagnostics_.warning(decl.loc, "duplicate native '" + name + "' ignored");
        }
    }
    for (const auto& child : decl.children) {
        emitTypeOrNative(child, kind);
    }
}

void Phase1Codegen::emitFunctions(const Program& program, const LibraryGraphResult& graph) {
    auto recordPass = [this](const std::string& name, auto&& fn) {
        auto start = std::chrono::steady_clock::now();
        fn();
        auto end = std::chrono::steady_clock::now();
        passTimings_[name] += std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();
    };
    initializers_.clear();
    recordPass("collectInitializers", [&]() { collectRootInitializers(program.decls); });
    recordPass("emitStructSupport", [&]() { emitStructFunctions(); });
    for (const Decl* library : graph.sortedLibraries) {
        collectInitializers(*library, library);
        emitDeclFunctions(*library, library);
    }
    for (const auto& decl : program.decls) {
        if (decl.kind != DeclKind::Library) {
            emitDeclFunctions(decl, nullptr);
        }
    }
    recordPass("lowerLambdas", [&]() { emitGeneratedLambdas(); });
    passTimings_["emitFunctions.lambdaEmit"] += passTimings_["lowerLambdas"];
    recordPass("emitFunctionInterfaceRuntime", [&]() { emitFunctionInterfaceRuntime(); });
    passTimings_["emitFunctions.functionInterfaceRuntime"] += passTimings_["emitFunctionInterfaceRuntime"];
    recordPass("emitInitHelper", [&]() { emitInitHelper(graph, program); });
    if (mainFunction_) {
        emitJassFunction(*mainFunction_, mainContainer_, true);
    }
}

void Phase1Codegen::emitDeclFunctions(const Decl& decl, const Decl* container) {
    if (decl.kind == DeclKind::Struct || decl.kind == DeclKind::Module || decl.kind == DeclKind::FunctionInterface) {
        return;
    }
    if (decl.kind == DeclKind::Function) {
        if (decl.name == "main") {
            mainFunction_ = &decl;
            mainContainer_ = container;
            return;
        }
        if (decl.mode == SyntaxMode::Zinc) {
            emitZincFunction(decl, container);
        } else {
            emitJassFunction(decl, container, false);
        }
        writer_.writeln();
        return;
    }
    const Decl* nextContainer = (decl.kind == DeclKind::Library || decl.kind == DeclKind::Scope) ? &decl : container;
    for (const auto& child : decl.children) {
        emitDeclFunctions(child, nextContainer);
    }
}

void Phase1Codegen::emitJassFunction(const Decl& decl, const Decl* container, bool injectInit) {
    if (decl.lines.empty()) {
        return;
    }
    lineFeatureCacheValid_ = false;
    BodyMode mode = bodyModeForFunction(decl);
    countBodyMode(mode, false);
    std::string header = rewriteFunctionHeader(decl, container);
    FunctionSignature declaredSig = parseFunctionSignatureFromHeader(stripAccessPrefixFromLine(decl.lines.front()), decl.mode);
    rememberFunctionInterfaceParamTypes(decl.name, functionNameFromHeaderLine(header), declaredSig.paramTypes, container);
    writer_.writeln(header);
    writer_.indent();
    LocalTypeMap localTypes;
    seedFunctionLocalTypes(stripAccessPrefixFromLine(decl.lines.front()), localTypes);
    std::vector<std::string> tempLocals;
    ArrayShapeMap localArrayShapes;
    LoweringContext ctx{nullptr, container, &localTypes, &localArrayShapes, &tempLocals, mode, functionNameFromHeaderLine(header), 0};
    bool injected = false;
    std::vector<std::string> locals;
    std::vector<std::string> body;
    std::vector<std::string> rawLines(decl.lines.begin() + 1, decl.lines.end());
    locals.reserve(rawLines.size() / 4 + 1);
    body.reserve(rawLines.size() + 4);
    const StructInfo* prevLambdaStruct = lambdaContextStruct_;
    const Decl* prevLambdaContainer = lambdaContextContainer_;
    lambdaContextStruct_ = nullptr;
    lambdaContextContainer_ = container;
    if (mayContainAnonymousFunction(rawLines)) {
        rawLines = extractZincLambdas(rawLines, decl.loc);
    }
    lambdaContextStruct_ = prevLambdaStruct;
    lambdaContextContainer_ = prevLambdaContainer;
    const bool cacheCandidate = bodyCacheEnabled() && !injectInit && bodyCacheSafe(rawLines);
    std::string cacheKey;
    if (cacheCandidate) {
        cacheKey = bodyCacheKey("function", "function:" + functionNameFromHeaderLine(header) + ":" + header, mode, rawLines);
        std::vector<std::string> cachedLines;
        if (loadBodyCache(cacheKey, cachedLines)) {
            for (const auto& line : cachedLines) {
                writer_.writeln(line);
            }
            writer_.dedent();
            writer_.writeln("endfunction");
            return;
        }
    } else if (bodyCacheEnabled()) {
        ++performanceCounters_.bodyCacheBypassedUnsafe;
    }
    std::vector<std::string> loweredBodyLines = lowerBodyByMode(mode, rawLines, ctx);
    for (const auto& rawLine : loweredBodyLines) {
        std::string t = trim(rawLine);
        if (startsWithWord(t, "endfunction")) {
            break;
        }
        std::string line = trim(rewriteForContainer(rawLine, container));
        std::vector<std::string> extraLines;
        if (startsWithWord(line, "local")) {
            auto localDecls = rewriteLocalDeclLine(line, nullptr, localTypes, ctx.localArrayShapes, extraLines);
            lineFeatureCacheValid_ = false;
            locals.insert(locals.end(), localDecls.begin(), localDecls.end());
            for (const auto& extra : extraLines) {
                for (const auto& lowered : lowerStatementLine(extra, ctx)) {
                    body.push_back(trim(lowered));
                }
            }
        } else {
            for (const auto& lowered : lowerStatementLine(line, ctx)) {
                body.push_back(trim(lowered));
            }
        }
        if (injectInit && !injected && t.find("InitBlizzard") != std::string::npos) {
            body.push_back("call vjassc__init_structs()");
            if (!functionInterfaces_.empty()) {
                body.push_back("call vjassc__init_function_interfaces()");
            }
            body.push_back("call vjassc__init_libraries()");
            injected = true;
        }
    }
    if (injectInit && !injected) {
        body.push_back("call vjassc__init_structs()");
        if (!functionInterfaces_.empty()) {
            body.push_back("call vjassc__init_function_interfaces()");
        }
        body.push_back("call vjassc__init_libraries()");
    }
    std::vector<std::string> emittedLines;
    emittedLines.reserve(locals.size() + body.size());
    emittedLines.insert(emittedLines.end(), locals.begin(), locals.end());
    emittedLines.insert(emittedLines.end(), body.begin(), body.end());
    if (cacheCandidate) {
        storeBodyCache(cacheKey, emittedLines);
    }
    for (const auto& line : emittedLines) {
        writer_.writeln(line);
    }
    writer_.dedent();
    writer_.writeln("endfunction");
}

void Phase1Codegen::emitZincFunction(const Decl& decl, const Decl* container) {
    if (decl.lines.empty()) {
        return;
    }
    lineFeatureCacheValid_ = false;
    BodyMode mode = bodyModeForFunction(decl);
    countBodyMode(mode, false);
    std::string header = trim(decl.lines.front());
    std::string accessHeader = header;
    (void)stripAccessPrefixFromLine(accessHeader);
    size_t functionPos = header.find("function");
    size_t open = header.find('(', functionPos);
    size_t close = header.find(')', open);
    std::string functionName = decl.name;
    std::string finalName = renameInContainer(functionName, container);
    FunctionSignature declaredSig = parseFunctionSignatureFromHeader(header, SyntaxMode::Zinc);
    rememberFunctionInterfaceParamTypes(functionName, finalName, declaredSig.paramTypes, container);
    std::string params = (open != std::string::npos && close != std::string::npos) ? header.substr(open + 1, close - open - 1) : "";
    std::string returns = "nothing";
    size_t arrow = header.find("->", close == std::string::npos ? 0 : close);
    if (arrow != std::string::npos) {
        std::string after = trim(std::string_view(header).substr(arrow + 2));
        size_t brace = after.find('{');
        if (brace != std::string::npos) {
            after = trim(std::string_view(after).substr(0, brace));
        }
        if (!after.empty()) {
            returns = after;
        }
    }
    params = trim(params);
    if (params.empty()) {
        params = "nothing";
    } else {
        std::vector<std::string> rewrittenParams;
        for (auto part : splitCommaList(params)) {
            std::istringstream in(trim(part));
            std::string type;
            std::string name;
            in >> type >> name;
            if (!type.empty() && !name.empty()) {
                rewrittenParams.push_back(rewriteTypeName(type, nullptr) + " " + name);
            }
        }
        params = joinParams(rewrittenParams);
    }
    writer_.writeln("function " + finalName + " takes " + params + " returns " + rewriteTypeName(returns, nullptr));
    writer_.indent();
    LocalTypeMap localTypes;
    FunctionSignature fnSig = parseFunctionSignatureFromHeader(header, SyntaxMode::Zinc);
    size_t paramIndex = 0;
    for (auto part : splitCommaList(params == "nothing" ? "" : params)) {
        std::istringstream in(trim(part));
        std::string type;
        std::string name;
        in >> type >> name;
        if (!type.empty() && !name.empty()) {
            if (paramIndex < fnSig.paramTypes.size() && fnSig.paramTypes[paramIndex] == "thistype") {
                localTypes[name] = type;
            } else if (paramIndex < fnSig.paramTypes.size() &&
                       structIndexByName_.find(fnSig.paramTypes[paramIndex]) != structIndexByName_.end()) {
                localTypes[name] = fnSig.paramTypes[paramIndex];
            } else if (paramIndex < fnSig.paramTypes.size() &&
                       functionInterfaceIndexByName_.find(fnSig.paramTypes[paramIndex]) != functionInterfaceIndexByName_.end()) {
                localTypes[name] = fnSig.paramTypes[paramIndex];
            } else {
                localTypes[name] = type;
            }
        }
        ++paramIndex;
    }
    std::vector<std::string> rawBodyLines(decl.lines.begin() + 1, decl.lines.end());
    const StructInfo* prevLambdaStruct = lambdaContextStruct_;
    const Decl* prevLambdaContainer = lambdaContextContainer_;
    lambdaContextStruct_ = nullptr;
    lambdaContextContainer_ = container;
    std::vector<std::string> bodyLines = mayContainAnonymousFunction(rawBodyLines)
        ? extractZincLambdas(rawBodyLines, decl.loc)
        : rawBodyLines;
    lambdaContextStruct_ = prevLambdaStruct;
    lambdaContextContainer_ = prevLambdaContainer;
    std::vector<std::string> tempLocals;
    ArrayShapeMap localArrayShapes;
    LoweringContext ctx{nullptr, container, &localTypes, &localArrayShapes, &tempLocals, mode, finalName, 0};
    const bool cacheCandidate = bodyCacheEnabled() && bodyCacheSafe(bodyLines);
    std::string cacheKey;
    if (cacheCandidate) {
        cacheKey = bodyCacheKey("function", "function:" + finalName + ":takes:" + params + ":returns:" + returns, mode, bodyLines);
        std::vector<std::string> cachedLines;
        if (loadBodyCache(cacheKey, cachedLines)) {
            for (const auto& line : cachedLines) {
                writer_.writeln(line);
            }
            writer_.dedent();
            writer_.writeln("endfunction");
            return;
        }
    } else if (bodyCacheEnabled()) {
        ++performanceCounters_.bodyCacheBypassedUnsafe;
    }
    std::vector<std::string> emittedLines;
    for (const auto& line : lowerBodyByMode(mode, bodyLines, ctx)) {
        std::string out = rewriteForContainer(line, container);
        std::vector<std::string> extraLines;
        if (startsWithWord(trim(out), "local")) {
            for (const auto& local : rewriteLocalDeclLine(out, nullptr, localTypes, ctx.localArrayShapes, extraLines)) {
                lineFeatureCacheValid_ = false;
                emittedLines.push_back(local);
            }
            for (const auto& extra : extraLines) {
                for (const auto& lowered : lowerStatementLine(extra, ctx)) {
                    emittedLines.push_back(lowered);
                }
            }
        } else {
            for (const auto& lowered : lowerStatementLine(out, ctx)) {
                emittedLines.push_back(lowered);
            }
        }
    }
    if (cacheCandidate) {
        storeBodyCache(cacheKey, emittedLines);
    }
    for (const auto& line : emittedLines) {
        writer_.writeln(line);
    }
    writer_.dedent();
    writer_.writeln("endfunction");
}

void Phase1Codegen::emitGeneratedLambdas() {
    for (const auto& lambda : lambdas_) {
        std::string rewrittenReturnType = rewriteTypeName(lambda.returnType.name, lambda.currentStruct);
        writer_.writeln();
        writer_.writeln("function " + lambda.name + " takes " + rewriteParamList(lambda.params, false, nullptr) +
                        " returns " + rewrittenReturnType);
        writer_.indent();
        LocalTypeMap localTypes;
        for (const auto& param : lambda.params) {
            localTypes[param.name] = param.type.name == "thistype" && lambda.currentStruct
                                         ? lambda.currentStruct->originalName
                                         : param.type.name;
        }
        std::vector<std::string> tempLocals;
        ArrayShapeMap localArrayShapes;
        BodyMode mode = BodyMode::Zinc;
        countBodyMode(mode, false);
        LoweringContext ctx{lambda.currentStruct, lambda.container, &localTypes, &localArrayShapes, &tempLocals, mode, lambda.name, 0};
        const bool cacheCandidate = bodyCacheEnabled() && bodyCacheSafe(lambda.bodyLines);
        std::string cacheKey;
        if (cacheCandidate) {
            std::string lambdaStableKey = "lambda:" + lambda.name + ":returns:" + rewrittenReturnType;
            for (const auto& param : lambda.params) {
                lambdaStableKey += ":param:" + param.type.name + ":" + param.name;
            }
            cacheKey = bodyCacheKey("lambda", lambdaStableKey, mode, lambda.bodyLines);
            std::vector<std::string> cachedLines;
            if (loadBodyCache(cacheKey, cachedLines)) {
                for (const auto& line : cachedLines) {
                    writer_.writeln(line);
                }
                writer_.dedent();
                writer_.writeln("endfunction");
                continue;
            }
        } else if (bodyCacheEnabled()) {
            ++performanceCounters_.bodyCacheBypassedUnsafe;
        }
        std::vector<std::string> lines = lowerBodyByMode(mode, lambda.bodyLines, ctx);
        bool lastEmittedReturn = false;
        std::vector<std::string> emittedLines;
        for (const auto& raw : lines) {
            std::string line = trim(rewriteForContainer(raw, lambda.container));
            if (line.empty()) {
                continue;
            }
            std::vector<std::string> extraLines;
            if (startsWithWord(line, "local")) {
                for (const auto& local : rewriteLocalDeclLine(line, nullptr, localTypes, ctx.localArrayShapes, extraLines)) {
                    emittedLines.push_back(local);
                }
                for (const auto& extra : extraLines) {
                    for (const auto& lowered : lowerStatementLine(extra, ctx)) {
                        emittedLines.push_back(lowered);
                        lastEmittedReturn = startsWithWord(trim(lowered), "return");
                    }
                }
            } else {
                for (const auto& lowered : lowerStatementLine(line, ctx)) {
                    emittedLines.push_back(lowered);
                    lastEmittedReturn = startsWithWord(trim(lowered), "return");
                }
            }
        }
        if (std::string defaultValue = defaultReturnValueForType(rewrittenReturnType); !defaultValue.empty() && !lastEmittedReturn) {
            emittedLines.push_back("return " + defaultValue);
        }
        if (cacheCandidate) {
            storeBodyCache(cacheKey, emittedLines);
        }
        for (const auto& line : emittedLines) {
            writer_.writeln(line);
        }
        writer_.dedent();
        writer_.writeln("endfunction");
    }
    if (!lambdas_.empty()) {
        writer_.writeln();
    }
}

void Phase1Codegen::emitFunctionInterfaceRuntime() {
    bool emittedAny = false;
    for (const auto& iface : functionInterfaces_) {
        std::string prefix = interfaceGlobalPrefix(iface);
        for (const auto& target : iface.targets) {
            bool needsCondition = target.needsCondition || iface.allTargetsNeedCondition;
            bool needsAction = target.needsAction || iface.allTargetsNeedAction;
            if (!needsCondition && !needsAction) {
                continue;
            }
            std::vector<std::string> args;
            for (size_t i = 0; i < iface.signature.paramTypes.size(); ++i) {
                args.push_back(prefix + "_arg" + std::to_string(i));
            }
            std::string call = target.finalName + "(" + joinArgs(args) + ")";
            std::string wrapper = prefix + "__" + sanitizeName(target.finalName) + "_wrapper";
            bool wrapperReturnsBoolean = iface.signature.returnType != "nothing";
            writer_.writeln("function " + wrapper + " takes nothing returns " +
                            std::string(wrapperReturnsBoolean ? "boolean" : "nothing"));
            writer_.indent();
            if (iface.signature.returnType == "nothing") {
                writer_.writeln("call " + call);
            } else {
                writer_.writeln("set " + prefix + "_result=" + call);
            }
            if (wrapperReturnsBoolean) {
                writer_.writeln("return true");
            }
            writer_.dedent();
            writer_.writeln("endfunction");
            writer_.writeln();
            emittedAny = true;
        }
    }

    if (!functionInterfaces_.empty()) {
        writer_.writeln("function vjassc__init_function_interfaces takes nothing returns nothing");
        writer_.indent();
        for (const auto& iface : functionInterfaces_) {
            std::string prefix = interfaceGlobalPrefix(iface);
            for (const auto& target : iface.targets) {
                bool needsCondition = target.needsCondition || iface.allTargetsNeedCondition;
                bool needsAction = target.needsAction || iface.allTargetsNeedAction;
                if (!needsCondition && !needsAction) {
                    continue;
                }
                std::string wrapper = prefix + "__" + sanitizeName(target.finalName) + "_wrapper";
                writer_.writeln("set " + prefix + "_trigger[" + std::to_string(target.id) + "]=CreateTrigger()");
                if (needsCondition) {
                    writer_.writeln("call TriggerAddCondition(" + prefix + "_trigger[" + std::to_string(target.id) +
                                    "], Condition(function " + wrapper + "))");
                }
                if (needsAction) {
                    writer_.writeln("call TriggerAddAction(" + prefix + "_trigger[" + std::to_string(target.id) + "], function " +
                                    wrapper + ")");
                }
            }
        }
        writer_.dedent();
        writer_.writeln("endfunction");
        writer_.writeln();
    }
    (void)emittedAny;
}

void Phase1Codegen::emitStructFunctions() {
    for (const auto& info : structs_) {
        emitStructSupportFunctions(info);
    }
}

void Phase1Codegen::emitStructSupportFunctions(const StructInfo& info) {
    auto recordPass = [this](const std::string& name, auto&& fn) {
        auto start = std::chrono::steady_clock::now();
        fn();
        auto end = std::chrono::steady_clock::now();
        passTimings_[name] += std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();
    };
    auto totalStart = std::chrono::steady_clock::now();
    const MethodInfo* customCreate = findMethod(info, "create");
    bool needsDeallocate = structUsesDeallocate(info);
    std::unordered_set<const MethodDecl*> emittedBeforeDestroy;
    recordPass("emitStructSupport.generatedHelpers", [&]() { emitStructGeneratedSupport(info, customCreate); });
    long long sourceBefore = passTimings_["emitStructSupport.sourceMethods"];
    recordPass("emitStructSupport.sourceMethods", [&]() { emitStructSourceMethods(info, emittedBeforeDestroy, true); });
    passTimings_["emitStructSupport.methodBodyLowering"] += passTimings_["emitStructSupport.sourceMethods"] - sourceBefore;
    recordPass("emitStructSupport.lifecycleSupport", [&]() {
        emitStructLifecycleSupport(info, emittedBeforeDestroy, needsDeallocate);
    });
    sourceBefore = passTimings_["emitStructSupport.sourceMethods"];
    recordPass("emitStructSupport.sourceMethods", [&]() { emitStructSourceMethods(info, emittedBeforeDestroy, false); });
    passTimings_["emitStructSupport.methodBodyLowering"] += passTimings_["emitStructSupport.sourceMethods"] - sourceBefore;
    auto totalEnd = std::chrono::steady_clock::now();
    passTimings_["emitStructSupport.total"] += std::chrono::duration_cast<std::chrono::milliseconds>(totalEnd - totalStart).count();
}

void Phase1Codegen::emitStructGeneratedSupport(const StructInfo& info, const MethodInfo* customCreate) {
    if (info.isArrayStruct) {
        return;
    }
    size_t beforeLines = writer_.lineCount();
    auto emitStructWarning = [&](const std::string& message) {
        writer_.writeln("call DisplayTimedTextToPlayer(GetLocalPlayer(),0,0,1000.,\"" + message + "\")");
    };
    if (!info.isArrayStruct) {
        const int instanceLimit = structInstanceLimit(info);
        writer_.writeln("function " + info.prefix + "__allocate takes nothing returns integer");
        writer_.indent();
        writer_.writeln("local integer this=si__" + info.generatedName + "_F");
        writer_.writeln("if (this!=0) then");
        writer_.indent();
        writer_.writeln("set si__" + info.generatedName + "_F=si__" + info.generatedName + "_V[this]");
        writer_.dedent();
        writer_.writeln("else");
        writer_.indent();
        writer_.writeln("set si__" + info.generatedName + "_I=si__" + info.generatedName + "_I+1");
        writer_.writeln("set this=si__" + info.generatedName + "_I");
        writer_.dedent();
        writer_.writeln("endif");
        writer_.writeln("if (this>" + std::to_string(instanceLimit) + ") then");
        writer_.indent();
        if (options_.warnMode) {
            emitStructWarning("Unable to allocate id for an object of type: " + info.generatedName);
        }
        writer_.writeln("return 0");
        writer_.dedent();
        writer_.writeln("endif");
        for (const auto& field : info.fields) {
            if (field.isFixedArray && field.fixedArraySize > 0 && !field.isStatic) {
                writer_.writeln("set " + field.generatedName + "[this]=(this-1)*" + std::to_string(field.fixedArraySize));
            }
        }
        writer_.writeln("set si__" + info.generatedName + "_V[this]=-1");
        writer_.writeln("return this");
        writer_.dedent();
        writer_.writeln("endfunction");
        writer_.writeln();

        if (!customCreate || !customCreate->isStatic) {
            writer_.writeln("function " + info.prefix + "_create takes nothing returns integer");
            writer_.indent();
            writer_.writeln("return " + info.prefix + "__allocate()");
            writer_.dedent();
            writer_.writeln("endfunction");
            writer_.writeln();
        }
    }
    size_t emittedLines = writer_.lineCount() - beforeLines;
    performanceCounters_.generatedSupportLinesEmitted += emittedLines;
    performanceCounters_.linesSkippedGeneratedSupport += emittedLines;
    if (emittedLines > 0) {
        countBodyMode(bodyModeForGenerated(GeneratedKind::StructAllocate), false);
        ++performanceCounters_.bodyLowererGeneratedBodies;
        performanceCounters_.bodyLowererGeneratedLines += emittedLines;
        performanceCounters_.generatedLinesSkippedGenericLowering += emittedLines;
        performanceCounters_.generatedFastPathLines += emittedLines;
    }
}

void Phase1Codegen::emitStructLifecycleSupport(const StructInfo& info,
                                               const std::unordered_set<const MethodDecl*>&,
                                               bool needsDeallocate) {
    if (info.isArrayStruct) {
        return;
    }
    auto emitStructWarning = [&](const std::string& message) {
        writer_.writeln("call DisplayTimedTextToPlayer(GetLocalPlayer(),0,0,1000.,\"" + message + "\")");
    };
    auto emitDestroyGuard = [&]() {
        if (!options_.warnMode) {
            return;
        }
        writer_.writeln("if this==null then");
        writer_.indent();
        emitStructWarning("Attempt to destroy a null struct of type: " + info.generatedName);
        writer_.writeln("return");
        writer_.dedent();
        writer_.writeln("elseif (si__" + info.generatedName + "_V[this]!=-1) then");
        writer_.indent();
        emitStructWarning("Double free of type: " + info.generatedName);
        writer_.writeln("return");
        writer_.dedent();
        writer_.writeln("endif");
    };
    auto emitRecycle = [&]() {
        writer_.writeln("set si__" + info.generatedName + "_V[this]=si__" + info.generatedName + "_F");
        writer_.writeln("set si__" + info.generatedName + "_F=this");
    };
    size_t beforeLines = writer_.lineCount();

    if (needsDeallocate) {
        writer_.writeln("function " + info.prefix + "_deallocate takes integer this returns nothing");
        writer_.indent();
        emitDestroyGuard();
        emitRecycle();
        writer_.dedent();
        writer_.writeln("endfunction");
        writer_.writeln();
    }

    writer_.writeln("function " + info.prefix + "_destroy takes integer this returns nothing");
    writer_.indent();
    emitDestroyGuard();
    for (const auto& method : info.methods) {
        if (method.isOnDestroy) {
            writer_.writeln("call " + method.generatedName + "(this)");
        }
    }
    if (needsDeallocate) {
        writer_.writeln("call " + info.prefix + "_deallocate(this)");
    } else {
        emitRecycle();
    }
    writer_.dedent();
    writer_.writeln("endfunction");
    writer_.writeln();
    size_t emittedLines = writer_.lineCount() - beforeLines;
    performanceCounters_.generatedSupportLinesEmitted += emittedLines;
    performanceCounters_.linesSkippedGeneratedSupport += emittedLines;
    if (emittedLines > 0) {
        countBodyMode(bodyModeForGenerated(GeneratedKind::StructDestroy), false);
        ++performanceCounters_.bodyLowererGeneratedBodies;
        performanceCounters_.bodyLowererGeneratedLines += emittedLines;
        performanceCounters_.generatedLinesSkippedGenericLowering += emittedLines;
        performanceCounters_.generatedFastPathLines += emittedLines;
    }
}

void Phase1Codegen::emitStructSourceMethods(const StructInfo& info,
                                            std::unordered_set<const MethodDecl*>& emittedBeforeDestroy,
                                            bool onDestroyOnly) {
    for (const auto& method : info.methods) {
        if (onDestroyOnly != method.isOnDestroy) {
            continue;
        }
        if (!onDestroyOnly && emittedBeforeDestroy.contains(method.decl)) {
            continue;
        }
        emitStructMethod(info, method);
        writer_.writeln();
        if (method.isOnDestroy) {
            emittedBeforeDestroy.insert(method.decl);
        }
        if (method.isOnInit) {
            structInitializers_.push_back({method.generatedName, info.libraryContainer});
        }
    }
}

void Phase1Codegen::emitStructMethod(const StructInfo& info, const MethodInfo& method) {
    lineFeatureCacheValid_ = false;
    ++performanceCounters_.methodPlanBuilt;
    BodyMode mode = bodyModeForMethod(*method.decl);
    countBodyMode(mode, true);
    std::string params = rewriteParamList(method.decl->params, !method.isStatic, &info);
    std::string returns = rewriteTypeName(method.decl->returnType.name, &info);
    writer_.writeln("function " + method.generatedName + " takes " + params + " returns " + returns);
    writer_.indent();
    LocalTypeMap localTypes;
    localTypes.reserve(method.decl->params.size() + 4);
    seedMethodLocalTypes(info, method, localTypes);
    std::vector<std::string> rawLines;
    rawLines.reserve(method.decl->bodyLines.size());
    for (const auto& logical : method.decl->bodyLines) {
        rawLines.push_back(logical.text);
    }
    const StructInfo* prevLambdaStruct = lambdaContextStruct_;
    const Decl* prevLambdaContainer = lambdaContextContainer_;
    lambdaContextStruct_ = &info;
    lambdaContextContainer_ = info.container;
    bool hadAnonymousFunction = mayContainAnonymousFunction(rawLines);
    if (hadAnonymousFunction) {
        rawLines = extractZincLambdas(rawLines, method.decl->loc);
    }
    lambdaContextStruct_ = prevLambdaStruct;
    lambdaContextContainer_ = prevLambdaContainer;
    std::vector<std::string> tempLocals;
    tempLocals.reserve(4);
    ArrayShapeMap localArrayShapes;
    localArrayShapes.reserve(2);
    LoweringContext ctx{&info, info.container, &localTypes, &localArrayShapes, &tempLocals, mode, method.generatedName, 0};
    const bool cacheCandidate = bodyCacheEnabled() && !hadAnonymousFunction && bodyCacheSafe(rawLines);
    std::string cacheKey;
    if (cacheCandidate) {
        cacheKey = bodyCacheKey("struct-method",
                                "method:" + info.generatedName + ":" + method.generatedName +
                                    ":takes:" + params + ":returns:" + returns,
                                mode,
                                rawLines);
        std::vector<std::string> cachedLines;
        if (loadBodyCache(cacheKey, cachedLines)) {
            for (const auto& line : cachedLines) {
                writer_.writeln(line);
            }
            writer_.dedent();
            writer_.writeln("endfunction");
            return;
        }
    } else if (bodyCacheEnabled()) {
        ++performanceCounters_.bodyCacheBypassedUnsafe;
    }
    std::vector<std::string> lines = lowerBodyByMode(mode, rawLines, ctx);
    std::vector<std::string> locals;
    locals.reserve(lines.size() / 4 + 1);
    std::vector<std::string> body;
    body.reserve(lines.size() + tempLocals.size());
    for (const auto& raw : lines) {
        std::string line = trim(rewriteForContainer(raw, info.container));
        if (line.empty() || line.rfind("//", 0) == 0) {
            continue;
        }
        std::vector<std::string> extraLines;
        if (startsWithWord(line, "local")) {
            auto localDecls = rewriteLocalDeclLine(line, &info, localTypes, ctx.localArrayShapes, extraLines);
            lineFeatureCacheValid_ = false;
            locals.insert(locals.end(), localDecls.begin(), localDecls.end());
            for (const auto& extra : extraLines) {
                lowerStatementLineInto(extra, ctx, body);
            }
        } else {
            lowerStatementLineInto(line, ctx, body);
        }
    }
    std::vector<std::string> emittedLines;
    emittedLines.reserve(locals.size() + body.size());
    emittedLines.insert(emittedLines.end(), locals.begin(), locals.end());
    emittedLines.insert(emittedLines.end(), body.begin(), body.end());
    if (cacheCandidate) {
        storeBodyCache(cacheKey, emittedLines);
    }
    for (const auto& line : emittedLines) {
        writer_.writeln(line);
    }
    writer_.dedent();
    writer_.writeln("endfunction");
}

void Phase1Codegen::emitInitHelper(const LibraryGraphResult& graph, const Program&) {
    writer_.writeln("function vjassc__init_structs takes nothing returns nothing");
    writer_.indent();
    std::unordered_map<const Decl*, size_t> libraryOrder;
    for (size_t i = 0; i < graph.sortedLibraries.size(); ++i) {
        libraryOrder[graph.sortedLibraries[i]] = i;
    }
    std::vector<const StructInitializerInfo*> libraryInitializers;
    libraryInitializers.reserve(structInitializers_.size());
    for (const auto& initializer : structInitializers_) {
        if (initializer.libraryContainer != nullptr) {
            libraryInitializers.push_back(&initializer);
        }
    }
    std::stable_sort(libraryInitializers.begin(), libraryInitializers.end(),
                     [&libraryOrder](const StructInitializerInfo* a, const StructInitializerInfo* b) {
                         auto rankOf = [&libraryOrder](const Decl* library) {
                             auto it = libraryOrder.find(library);
                             return it == libraryOrder.end() ? static_cast<size_t>(-1) : it->second;
                         };
                         return rankOf(a->libraryContainer) < rankOf(b->libraryContainer);
                     });
    size_t nextLibraryInitializer = 0;
    for (const auto& initializer : structInitializers_) {
        const StructInitializerInfo* current = &initializer;
        if (initializer.libraryContainer != nullptr && nextLibraryInitializer < libraryInitializers.size()) {
            current = libraryInitializers[nextLibraryInitializer++];
        }
        writer_.writeln("call " + current->name + "()");
    }
    writer_.dedent();
    writer_.writeln("endfunction");
    writer_.writeln();

    writer_.writeln("function vjassc__init_libraries takes nothing returns nothing");
    writer_.indent();
    for (const auto& [name, executeFunc] : initializers_) {
        if (executeFunc) {
            writer_.writeln("call ExecuteFunc(\"" + name + "\")");
        } else {
            writer_.writeln("call " + name + "()");
        }
    }
    writer_.dedent();
    writer_.writeln("endfunction");
    writer_.writeln();
}

void Phase1Codegen::collectInitializers(const Decl& decl, const Decl* container) {
    if (decl.kind == DeclKind::Module) {
        return;
    }
    if (decl.kind == DeclKind::Library || decl.kind == DeclKind::Scope) {
        const Decl* selfContainer = &decl;
        if (!decl.initializer.empty()) {
            initializers_.push_back({renameInContainer(decl.initializer, selfContainer), decl.kind == DeclKind::Library});
        } else {
            for (const auto& child : decl.children) {
                if (child.kind == DeclKind::Function && child.name == "onInit") {
                    initializers_.push_back({renameInContainer(child.name, selfContainer), decl.kind == DeclKind::Library});
                    break;
                }
            }
        }
    }
    const Decl* nextContainer = (decl.kind == DeclKind::Library || decl.kind == DeclKind::Scope) ? &decl : container;
    for (const auto& child : decl.children) {
        collectInitializers(child, nextContainer);
    }
}

void Phase1Codegen::collectRootInitializers(const std::vector<Decl>& decls) {
    for (const auto& decl : decls) {
        if (decl.kind == DeclKind::Scope) {
            collectInitializers(decl, &decl);
        }
    }
}

void Phase1Codegen::collectStructs(const std::vector<Decl>& decls,
                                   const Decl* container,
                                   const Decl* libraryContainer) {
    for (const auto& decl : decls) {
        if (decl.kind == DeclKind::Struct) {
            collectStruct(decl, container, libraryContainer);
            continue;
        }
        if (decl.kind == DeclKind::Module) {
            continue;
        }
        const Decl* nextContainer = (decl.kind == DeclKind::Library || decl.kind == DeclKind::Scope) ? &decl : container;
        const Decl* nextLibraryContainer = decl.kind == DeclKind::Library ? &decl : libraryContainer;
        collectStructs(decl.children, nextContainer, nextLibraryContainer);
    }
}

void Phase1Codegen::collectStruct(const Decl& decl, const Decl* container, const Decl* libraryContainer) {
    StructInfo info;
    info.decl = &decl;
    info.container = container;
    info.libraryContainer = libraryContainer;
    info.originalName = decl.name;
    info.generatedName = makeScopedStructName(decl, container);
    info.prefix = "s__" + info.generatedName;
    info.typeId = static_cast<int>(structs_.size()) + 1;
    info.isArrayStruct = decl.isArrayStruct;

    std::unordered_set<std::string> seenFields;
    std::unordered_set<std::string> seenMethods;
    for (const auto& field : decl.fields) {
        if (!seenFields.insert(field.name).second) {
            diagnostics_.error(field.loc, "duplicate field '" + field.name + "' in struct '" + decl.name + "'");
        }
        FieldInfo fieldInfo;
        fieldInfo.decl = &field;
        fieldInfo.name = field.name;
        fieldInfo.typeName = field.type.name;
        fieldInfo.generatedName = info.prefix + "_" + field.name;
        fieldInfo.isStatic = field.isStatic;
        fieldInfo.isArray = field.isArray;
        fieldInfo.isFixedArray = field.isFixedArray;
        fieldInfo.fixedArraySize = field.fixedArraySize;
        fieldInfo.arrayDimensions = field.arrayDimensions;
        if (fieldInfo.arrayDimensions.size() > 1 ||
            (!fieldInfo.isStatic && fieldInfo.isFixedArray && !fieldInfo.arrayDimensions.empty())) {
            std::string shapeName = (!fieldInfo.isStatic && fieldInfo.isFixedArray)
                                        ? fixedArrayStorageName(info, fieldInfo)
                                        : fieldInfo.generatedName;
            globalArrayShapes_[shapeName] = ArrayShape{fieldInfo.arrayDimensions, false};
            if (!shapeName.empty()) {
                globalArrayShapeFirstChars_[static_cast<unsigned char>(shapeName.front())] = true;
            }
        }
        if (!fieldInfo.isStatic && fieldInfo.isFixedArray && fieldInfo.fixedArraySize > 0) {
            globalStructTypes_[fixedArrayStorageName(info, fieldInfo)] =
                fieldInfo.typeName == "thistype" ? info.originalName : fieldInfo.typeName;
        } else {
            globalStructTypes_[fieldInfo.generatedName] = fieldInfo.typeName == "thistype" ? info.originalName : fieldInfo.typeName;
        }
        info.fieldIndex[fieldInfo.name] = info.fields.size();
        if (!fieldInfo.name.empty()) {
            info.fieldFirstChars[static_cast<unsigned char>(fieldInfo.name.front())] = true;
        }
        info.fields.push_back(std::move(fieldInfo));
    }
    for (const auto& method : decl.methods) {
        if (!seenMethods.insert(method.name).second) {
            diagnostics_.error(method.loc, "duplicate method '" + method.name + "' in struct '" + decl.name + "'");
        }
        if (seenFields.contains(method.name)) {
            diagnostics_.error(method.loc, "field and method share name '" + method.name + "' in struct '" + decl.name + "'");
        }
        if (method.isOnDestroy && (method.isStatic || !method.params.empty() || method.returnType.name != "nothing")) {
            diagnostics_.error(method.loc, "onDestroy must be an instance method returning nothing with no parameters");
        }
        if (method.isOnInit && (!method.isStatic || !method.params.empty() || method.returnType.name != "nothing")) {
            diagnostics_.error(method.loc, "onInit must be a static method returning nothing with no parameters");
        }
        MethodInfo methodInfo;
        methodInfo.decl = &method;
        methodInfo.name = method.name;
        methodInfo.isStatic = method.isStatic;
        methodInfo.isOnDestroy = method.isOnDestroy;
        methodInfo.isOnInit = method.isOnInit;
        if (method.isOnDestroy && method.name == "onDestroy") {
            methodInfo.generatedName = "sc__" + info.generatedName + "_onDestroy";
        } else {
            methodInfo.generatedName = info.prefix + "_" + method.name;
        }
        info.methodIndex[methodInfo.name] = info.methods.size();
        if (!methodInfo.name.empty()) {
            info.methodFirstChars[static_cast<unsigned char>(methodInfo.name.front())] = true;
        }
        info.methods.push_back(std::move(methodInfo));
    }

    size_t index = structs_.size();
    structs_.push_back(std::move(info));
    structIndexByDecl_[&decl] = index;
    structIndexByName_[structs_[index].originalName] = index;
    structIndexByName_[structs_[index].generatedName] = index;
}

Phase1Codegen::FunctionSignature Phase1Codegen::signatureFromParams(const std::vector<ParamDecl>& params,
                                                                     const TypeRef& returnType,
                                                                     const StructInfo* currentStruct) const {
    FunctionSignature sig;
    for (const auto& param : params) {
        sig.paramTypes.push_back(param.type.name == "thistype" && currentStruct ? currentStruct->originalName : param.type.name);
    }
    sig.returnType = returnType.name == "thistype" && currentStruct ? currentStruct->originalName : (returnType.name.empty() ? "nothing" : returnType.name);
    return sig;
}

Phase1Codegen::FunctionSignature Phase1Codegen::parseFunctionSignatureFromHeader(const std::string& header, SyntaxMode mode) const {
    FunctionSignature sig;
    sig.returnType = "nothing";
    if (mode == SyntaxMode::Zinc) {
        size_t open = header.find('(');
        size_t close = open == std::string::npos ? std::string::npos : header.find(')', open);
        if (open != std::string::npos && close != std::string::npos) {
            for (auto part : splitCommaList(header.substr(open + 1, close - open - 1))) {
                std::istringstream in(trim(part));
                std::string type;
                std::string name;
                in >> type >> name;
                if (!type.empty() && !name.empty()) {
                    sig.paramTypes.push_back(type);
                }
            }
            size_t arrow = header.find("->", close);
            if (arrow != std::string::npos) {
                std::string ret = trim(std::string_view(header).substr(arrow + 2));
                size_t brace = ret.find('{');
                if (brace != std::string::npos) {
                    ret = trim(std::string_view(ret).substr(0, brace));
                }
                if (!ret.empty()) {
                    sig.returnType = ret;
                }
            }
        }
        return sig;
    }

    size_t takes = header.find(" takes ");
    size_t returns = header.find(" returns ");
    if (takes != std::string::npos && returns != std::string::npos && returns > takes) {
        std::string params = trim(std::string_view(header).substr(takes + 7, returns - (takes + 7)));
        if (!params.empty() && params != "nothing") {
            for (auto part : splitCommaList(params)) {
                std::istringstream in(trim(part));
                std::string type;
                std::string name;
                in >> type >> name;
                if (!type.empty() && !name.empty()) {
                    sig.paramTypes.push_back(type);
                }
            }
        }
        std::string ret = trim(std::string_view(header).substr(returns + 9));
        if (!ret.empty()) {
            sig.returnType = ret;
        }
    }
    return sig;
}

bool Phase1Codegen::sameSignature(const FunctionSignature& a, const FunctionSignature& b) const {
    if (rewriteTypeName(a.returnType, nullptr) != rewriteTypeName(b.returnType, nullptr) || a.paramTypes.size() != b.paramTypes.size()) {
        return false;
    }
    for (size_t i = 0; i < a.paramTypes.size(); ++i) {
        if (rewriteTypeName(a.paramTypes[i], nullptr) != rewriteTypeName(b.paramTypes[i], nullptr)) {
            return false;
        }
    }
    return true;
}

void Phase1Codegen::collectFunctionInterfaces(const std::vector<Decl>& decls, const Decl* container) {
    for (const auto& decl : decls) {
        if (decl.kind == DeclKind::FunctionInterface) {
            FunctionInterfaceInfo info;
            info.decl = &decl;
            info.container = container;
            info.sourceName = decl.name;
            info.finalName = renameInContainer(decl.name, container);
            info.signature = signatureFromParams(decl.interfaceParams, decl.interfaceReturnType, nullptr);
            size_t index = functionInterfaces_.size();
            functionInterfaces_.push_back(std::move(info));
            functionInterfaceIndexByName_[functionInterfaces_[index].sourceName] = index;
            functionInterfaceIndexByName_[functionInterfaces_[index].finalName] = index;
            continue;
        }
        const Decl* nextContainer = (decl.kind == DeclKind::Library || decl.kind == DeclKind::Scope) ? &decl : container;
        collectFunctionInterfaces(decl.children, nextContainer);
    }
}

void Phase1Codegen::collectFunctionObjectEvaluateInterfaces(const std::vector<Decl>& decls,
                                                            const Decl* container,
                                                            const StructInfo* currentStruct) {
    for (const auto& decl : decls) {
        const Decl* nextContainer = (decl.kind == DeclKind::Library || decl.kind == DeclKind::Scope) ? &decl : container;
        if (decl.kind == DeclKind::Function) {
            for (const auto& line : decl.lines) {
                collectFunctionObjectEvaluateInterfaceFromLine(line, currentStruct);
            }
            continue;
        }
        if (decl.kind == DeclKind::Struct) {
            const StructInfo* info = findStruct(makeScopedStructName(decl, container));
            if (!info) {
                info = findStruct(decl.name);
            }
            if (info) {
                for (const auto& method : decl.methods) {
                    for (const auto& line : method.bodyLines) {
                        collectFunctionObjectEvaluateInterfaceFromLine(line.text, info);
                    }
                }
            }
            continue;
        }
        if (decl.kind == DeclKind::Module || decl.kind == DeclKind::FunctionInterface) {
            continue;
        }
        collectFunctionObjectEvaluateInterfaces(decl.children, nextContainer, currentStruct);
    }
}

void Phase1Codegen::collectFunctionObjectEvaluateInterfaceFromLine(const std::string& line, const StructInfo* currentStruct) {
    std::string code = blankStringRawcodeAndComment(line);
    for (const std::string method : {"evaluate", "execute"}) {
        size_t search = 0;
        while (search < code.size()) {
            size_t receiverStart = 0;
            size_t dotPos = 0;
            size_t open = 0;
            size_t close = 0;
            std::string tail = code.substr(search);
            if (!findMethodCallSpan(tail, method, receiverStart, dotPos, open, close)) {
                break;
            }
            receiverStart += search;
            dotPos += search;
            close += search;
            std::string receiver = trim(code.substr(receiverStart, dotPos - receiverStart));
            std::string finalName = resolveFunctionTargetName(receiver, currentStruct);
            if (const FunctionInfo* fn = findFunctionInfo(finalName)) {
                ensureFunctionObjectInterface(fn->signature);
            }
            search = close + 1;
        }
    }
}

void Phase1Codegen::rememberFunctionInterfaceParamTypes(const std::string& sourceName,
                                                        const std::string& finalName,
                                                        const std::vector<std::string>& paramTypes,
                                                        const Decl* declContainer) {
    bool hasInterfaceParam = false;
    bool hasIntegerParam = false;
    std::vector<std::string> interfaceParamTypes;
    interfaceParamTypes.reserve(paramTypes.size());
    for (const auto& type : paramTypes) {
        std::string interfaceType = resolveFunctionInterfaceTypeName(type, declContainer);
        if (!interfaceType.empty()) {
            interfaceParamTypes.push_back(interfaceType);
            hasInterfaceParam = true;
        } else {
            interfaceParamTypes.push_back({});
            if (type == "integer") {
                hasIntegerParam = true;
            }
        }
    }
    if (hasInterfaceParam) {
        functionInterfaceParamTypesByFunction_[sourceName] = interfaceParamTypes;
        functionInterfaceParamTypesByFunction_[finalName] = interfaceParamTypes;
    }
    if (hasInterfaceParam || hasIntegerParam) {
        functionArgumentLoweringCandidates_.insert(sourceName);
        functionArgumentLoweringCandidates_.insert(finalName);
    }
}

void Phase1Codegen::collectFunctions(const std::vector<Decl>& decls, const Decl* container) {
    for (const auto& decl : decls) {
        if (decl.kind == DeclKind::Function) {
            size_t before = functions_.size();
            collectFunction(decl, container);
            if (functions_.size() > before) {
                const auto& fn = functions_.back();
                rememberFunctionInterfaceParamTypes(fn.sourceName, fn.finalName, fn.signature.paramTypes, container);
            }
            continue;
        }
        if (decl.kind == DeclKind::Struct) {
            const StructInfo* info = findStruct(makeScopedStructName(decl, container));
            if (!info) {
                info = findStruct(decl.name);
            }
            if (info) {
                for (const auto& method : info->methods) {
                    FunctionInfo fn;
                    fn.sourceName = info->originalName + "." + method.name;
                    fn.finalName = method.generatedName;
                    fn.signature = signatureFromParams(method.decl->params, method.decl->returnType, info);
                    if (!method.isStatic) {
                        fn.signature.paramTypes.insert(fn.signature.paramTypes.begin(), info->originalName);
                    }
                    fn.isStaticMethod = method.isStatic;
                    size_t index = functions_.size();
                    functions_.push_back(std::move(fn));
                    functionIndexByName_[functions_[index].sourceName] = index;
                    functionIndexByName_[functions_[index].finalName] = index;
                    rememberFunctionInterfaceParamTypes(functions_[index].sourceName,
                                                        functions_[index].finalName,
                                                        functions_[index].signature.paramTypes,
                                                        info->container);
                }
                if (!info->isArrayStruct) {
                    auto registerSupportFunction = [&](std::string sourceName,
                                                       std::string finalName,
                                                       std::vector<std::string> paramTypes,
                                                       std::string returnType,
                                                       GeneratedKind generatedKind) {
                        if (functionIndexByName_.contains(finalName)) {
                            return;
                        }
                        FunctionInfo fn;
                        fn.sourceName = std::move(sourceName);
                        fn.finalName = std::move(finalName);
                        fn.signature.paramTypes = std::move(paramTypes);
                        fn.signature.returnType = std::move(returnType);
                        fn.generatedKind = generatedKind;
                        size_t index = functions_.size();
                        functions_.push_back(std::move(fn));
                        functionIndexByName_[functions_[index].sourceName] = index;
                        functionIndexByName_[functions_[index].finalName] = index;
                    };
                    const MethodInfo* customCreate = findMethod(*info, "create");
                    registerSupportFunction(info->originalName + ".allocate",
                                            info->prefix + "__allocate",
                                            {},
                                            info->originalName,
                                            GeneratedKind::StructAllocate);
                    if (!customCreate || !customCreate->isStatic) {
                        registerSupportFunction(info->originalName + ".create",
                                                info->prefix + "_create",
                                                {},
                                                info->originalName,
                                                GeneratedKind::StructCreate);
                    }
                    if (structUsesDeallocate(*info)) {
                        registerSupportFunction(info->originalName + ".deallocate",
                                                info->prefix + "_deallocate",
                                                {info->originalName},
                                                "nothing",
                                                GeneratedKind::StructDeallocate);
                    }
                    registerSupportFunction(info->originalName + ".destroy",
                                            info->prefix + "_destroy",
                                            {info->originalName},
                                            "nothing",
                                            GeneratedKind::StructDestroy);
                }
            }
            continue;
        }
        if (decl.kind == DeclKind::Module || decl.kind == DeclKind::FunctionInterface) {
            continue;
        }
        const Decl* nextContainer = (decl.kind == DeclKind::Library || decl.kind == DeclKind::Scope) ? &decl : container;
        collectFunctions(decl.children, nextContainer);
    }
}

void Phase1Codegen::collectFunction(const Decl& decl, const Decl* container) {
    if (decl.lines.empty()) {
        return;
    }
    FunctionInfo fn;
    fn.sourceName = decl.name;
    fn.finalName = renameInContainer(decl.name, container);
    fn.signature = parseFunctionSignatureFromHeader(stripAccessPrefixFromLine(decl.lines.front()), decl.mode);
    size_t index = functions_.size();
    functions_.push_back(std::move(fn));
    functionIndexByName_[functions_[index].sourceName] = index;
    functionIndexByName_[functions_[index].finalName] = index;
}

std::string Phase1Codegen::rewriteForContainer(const std::string& line, const Decl* container) const {
    return symbols_.rewriteLine(line, container);
}

std::string Phase1Codegen::rewriteGlobalLine(const std::string& line, const Decl* container) {
    std::string out = rewriteForContainer(line, container);
    out = stripAccessPrefixFromLine(out);
    out = removeSemicolonsOutsideProtected(out);
    ArrayDeclRewrite multi = rewriteMultiDimDeclarationLine(out);
    if (multi.matched) {
        globalArrayShapes_[multi.name] = ArrayShape{multi.dimensions, false};
        if (!multi.name.empty()) {
            globalArrayShapeFirstChars_[static_cast<unsigned char>(multi.name.front())] = true;
        }
        out = multi.line;
    }
    out = replaceRegex(out,
                       R"(^(\s*(?:constant\s+)?[A-Za-z_][A-Za-z0-9_$]*)\s+([A-Za-z_][A-Za-z0-9_$]*)\s*\[\])",
                       "$1 array $2");
    out = replaceRegex(out,
                       R"(^(\s*(?:constant\s+)?[A-Za-z_][A-Za-z0-9_$]*)\s+([A-Za-z_][A-Za-z0-9_$]*)\s+\[\])",
                       "$1 array $2");
    ParsedLocalDeclList parsed = parseLocalDeclList(out);
    if (parsed.matched) {
        for (const auto& decl : parsed.decls) {
            if (structIndexByName_.find(decl.type) != structIndexByName_.end() ||
                functionInterfaceIndexByName_.find(decl.type) != functionInterfaceIndexByName_.end()) {
                globalStructTypes_[decl.name] = decl.type;
            }
        }
    }
    std::string t = trim(out);
    bool constant = false;
    if (startsWithWord(t, "constant")) {
        constant = true;
        t = trim(std::string_view(t).substr(8));
    }
    std::istringstream in(t);
    std::string type;
    in >> type;
    if (!type.empty() &&
        (structIndexByName_.find(type) != structIndexByName_.end() ||
         functionInterfaceIndexByName_.find(type) != functionInterfaceIndexByName_.end())) {
        size_t typePos = out.find(type);
        if (typePos != std::string::npos) {
            out.replace(typePos, type.size(), "integer");
        }
    }
    (void)constant;
    return trim(rewriteArrayAccesses(out, nullptr));
}

std::string Phase1Codegen::rewriteFunctionHeader(const Decl& decl, const Decl* container) const {
    std::string header = rewriteForContainer(decl.lines.front(), container);
    header = stripAccessPrefixFromLine(header);
    size_t takes = header.find(" takes ");
    size_t returns = header.find(" returns ");
    if (takes != std::string::npos && returns != std::string::npos && returns > takes) {
        std::string prefix = header.substr(0, takes + 7);
        std::string params = header.substr(takes + 7, returns - (takes + 7));
        std::string ret = trim(std::string_view(header).substr(returns + 9));
        if (trim(params) != "nothing") {
            std::vector<std::string> rewritten;
            for (auto part : splitCommaList(params)) {
                std::istringstream in(trim(part));
                std::string type;
                std::string name;
                in >> type >> name;
                if (!type.empty() && !name.empty()) {
                    rewritten.push_back(rewriteTypeName(type, nullptr) + " " + name);
                }
            }
            params = joinParams(rewritten);
        }
        header = prefix + params + " returns " + rewriteTypeName(ret, nullptr);
    }
    return trim(header);
}

std::string Phase1Codegen::renameInContainer(const std::string& name, const Decl* container) const {
    if (!container) {
        return name;
    }
    auto symbols = symbols_.symbolsFor(container);
    if (!symbols) {
        return name;
    }
    auto it = symbols->replacements.find(name);
    return it == symbols->replacements.end() ? name : it->second;
}

std::string Phase1Codegen::makeScopedStructName(const Decl& decl, const Decl* container) const {
    if (!container) {
        return decl.name;
    }
    if (decl.access == "private") {
        return container->name + "___" + decl.name;
    }
    if (decl.access == "public") {
        return decl.name;
    }
    if (decl.mode == SyntaxMode::Zinc) {
        return container->name + "___" + decl.name;
    }
    return decl.name;
}

std::string Phase1Codegen::fixedArrayStorageName(const StructInfo& info, const FieldInfo& field) const {
    return "s___" + info.generatedName + "_" + field.name;
}

int Phase1Codegen::structFixedArrayStride(const StructInfo& info) const {
    int stride = 0;
    for (const auto& field : info.fields) {
        if (!field.isStatic && field.isFixedArray && field.fixedArraySize > stride) {
            stride = field.fixedArraySize;
        }
    }
    return stride;
}

int Phase1Codegen::structInstanceLimit(const StructInfo& info) const {
    int stride = structFixedArrayStride(info);
    if (stride <= 0) {
        return 8190;
    }
    return std::max(0, 8190 / stride - 1);
}

std::string Phase1Codegen::rewriteArrayAccesses(
    const std::string& line,
    const ArrayShapeMap* localArrayShapes) const {
    if (line.find('[') == std::string::npos || !hasKnownArrayReceiver(line, localArrayShapes)) {
        return line;
    }
    const bool cacheable = !localArrayShapes || localArrayShapes->empty();
    if (cacheable) {
        if (auto cached = arrayRewriteCache_.find(line); cached != arrayRewriteCache_.end()) {
            ++performanceCounters_.cachedRewriteHits;
            return cached->second;
        }
        ++performanceCounters_.cachedRewriteMisses;
    }
    bool countedAttempt = false;
    std::string out;
    out.reserve(line.size());
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    for (size_t i = 0; i < line.size();) {
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
        if (!isIdentStart(c)) {
            out.push_back(c);
            ++i;
            continue;
        }

        size_t nameStart = i++;
        while (i < line.size() && isIdentPart(line[i])) {
            ++i;
        }
        std::string_view name(line.data() + nameStart, i - nameStart);
        const ArrayShape* shape = nullptr;
        unsigned char first = static_cast<unsigned char>(name.front());
        if (globalArrayShapeFirstChars_[first]) {
            auto globalIt = globalArrayShapes_.find(name);
            if (globalIt != globalArrayShapes_.end()) {
                shape = &globalIt->second;
            }
        }
        if (localArrayShapes && !localArrayShapes->empty()) {
            auto localIt = localArrayShapes->find(name);
            if (localIt != localArrayShapes->end()) {
                shape = &localIt->second;
            }
        }
        if (!shape || shape->dimensions.empty()) {
            out.append(name.data(), name.size());
            continue;
        }
        size_t afterName = i;
        std::vector<std::string> indexes;
        size_t cursor = afterName;
        while (cursor < line.size()) {
            size_t ws = cursor;
            while (ws < line.size() && std::isspace(static_cast<unsigned char>(line[ws]))) {
                ++ws;
            }
            if (ws >= line.size() || line[ws] != '[') {
                break;
            }
            size_t close = findMatchingBracketOutsideProtected(line, ws);
            if (close == std::string::npos) {
                break;
            }
            indexes.push_back(line.substr(ws + 1, close - ws - 1));
            cursor = close + 1;
        }
        size_t needed = shape->dimensions.size() + (shape->structInstanceField ? 1 : 0);
        if (indexes.size() < needed || needed == 0) {
            out += name;
            continue;
        }
        if (!countedAttempt) {
            ++performanceCounters_.arrayAccessRewriteAttempts;
            countedAttempt = true;
        }

        std::string flat;
        if (shape->structInstanceField) {
            int stride = arrayFlatSize(shape->dimensions);
            if (stride <= 0) {
                out.append(name.data(), name.size());
                continue;
            }
            flat = arrayIndexTerm(indexes[0]) + " * " + std::to_string(stride);
            std::string rest = composeFlatIndex(indexes, shape->dimensions, 1);
            if (!rest.empty()) {
                flat = "(" + flat + " + " + rest + ")";
            }
        } else {
            flat = composeFlatIndex(indexes, shape->dimensions, 0);
        }
        out.append(name.data(), name.size());
        out.push_back('[');
        out += flat;
        out.push_back(']');
        for (size_t extra = needed; extra < indexes.size(); ++extra) {
            out += "[" + stripRedundantOuterParens(indexes[extra]) + "]";
        }
        i = cursor;
    }
    if (out != line) {
        ++performanceCounters_.arrayAccessRewriteChanged;
    }
    if (cacheable) {
        arrayRewriteCache_.emplace(line, out);
    }
    return out;
}

bool Phase1Codegen::hasKnownArrayReceiver(
    const std::string& line,
    const ArrayShapeMap* localArrayShapes) const {
    if (line.find('[') == std::string::npos) {
        return false;
    }
    const LineTokenCache& tokens = getLineTokenCache(line);
    if (!tokens.hasBracket) {
        return false;
    }
    for (const auto& token : tokens.identifiers) {
        size_t cursor = token.end;
        while (cursor < line.size() && std::isspace(static_cast<unsigned char>(line[cursor]))) {
            ++cursor;
        }
        if (cursor >= line.size() || line[cursor] != '[') {
            continue;
        }
        std::string_view tokenText(tokens.line.data() + token.start, token.end - token.start);
        unsigned char first = static_cast<unsigned char>(tokenText.front());
        if (globalArrayShapeFirstChars_[first] && globalArrayShapes_.contains(tokenText)) {
            return true;
        }
        if (localArrayShapes && !localArrayShapes->empty() && localArrayShapes->contains(tokenText)) {
            return true;
        }
    }
    return false;
}

const Phase1Codegen::StructInfo* Phase1Codegen::findStruct(std::string_view name) const {
    ++performanceCounters_.structLookupCalls;
    if (auto cached = structLookupCache_.find(name); cached != structLookupCache_.end()) {
        ++performanceCounters_.cachedRewriteHits;
        return cached->second;
    }
    ++performanceCounters_.cachedRewriteMisses;
    auto it = structIndexByName_.find(name);
    const StructInfo* result = it == structIndexByName_.end() ? nullptr : &structs_[it->second];
    structLookupCache_.emplace(std::string(name), result);
    return result;
}

const Phase1Codegen::FieldInfo* Phase1Codegen::findField(const StructInfo& info, std::string_view name) const {
    auto it = info.fieldIndex.find(name);
    return it == info.fieldIndex.end() ? nullptr : &info.fields[it->second];
}

const Phase1Codegen::MethodInfo* Phase1Codegen::findMethod(const StructInfo& info, std::string_view name) const {
    auto it = info.methodIndex.find(name);
    return it == info.methodIndex.end() ? nullptr : &info.methods[it->second];
}

bool Phase1Codegen::structUsesDeallocate(const StructInfo& info) const {
    if (auto cached = structDeallocateCache_.find(info.generatedName); cached != structDeallocateCache_.end()) {
        return cached->second;
    }
    for (const auto& method : info.methods) {
        if (!method.decl) {
            continue;
        }
        for (const auto& line : method.decl->bodyLines) {
            if (line.text.find("deallocate") != std::string::npos) {
                structDeallocateCache_[info.generatedName] = true;
                return true;
            }
        }
    }
    structDeallocateCache_[info.generatedName] = false;
    return false;
}

std::string Phase1Codegen::rewriteTypeName(const std::string& typeName, const StructInfo* currentStruct) const {
    if (typeName == "thistype") {
        return "integer";
    }
    if (currentStruct && typeName == currentStruct->originalName) {
        return "integer";
    }
    if (structIndexByName_.find(typeName) != structIndexByName_.end()) {
        return "integer";
    }
    if (functionInterfaceIndexByName_.find(typeName) != functionInterfaceIndexByName_.end()) {
        return "integer";
    }
    return typeName;
}

std::string Phase1Codegen::rewriteParamList(const std::vector<ParamDecl>& params, bool includeThis, const StructInfo* currentStruct) const {
    std::vector<std::string> out;
    if (includeThis) {
        out.push_back("integer this");
    }
    for (const auto& param : params) {
        out.push_back(rewriteTypeName(param.type.name, currentStruct) + " " + param.name);
    }
    return joinParams(out);
}

void Phase1Codegen::seedMethodLocalTypes(const StructInfo& info, const MethodInfo& method, LocalTypeMap& localTypes) const {
    if (!method.isStatic) {
        localTypes["this"] = info.originalName;
    }
    for (const auto& param : method.decl->params) {
        if (param.type.name == "thistype") {
            localTypes[param.name] = info.originalName;
        } else {
            localTypes[param.name] = param.type.name;
        }
    }
}

void Phase1Codegen::seedFunctionLocalTypes(const std::string& header, LocalTypeMap& localTypes) const {
    size_t takes = header.find(" takes ");
    size_t returns = header.find(" returns ");
    if (takes == std::string::npos || returns == std::string::npos || returns <= takes) {
        return;
    }
    std::string params = header.substr(takes + 7, returns - (takes + 7));
    params = trim(params);
    if (params == "nothing" || params.empty()) {
        return;
    }
    for (auto part : splitCommaList(params)) {
        std::istringstream in(trim(part));
        std::string type;
        std::string name;
        in >> type >> name;
        if (!type.empty() && !name.empty()) {
            localTypes[name] = type;
        }
    }
}

std::vector<std::string> Phase1Codegen::rewriteLocalDeclLine(const std::string& line,
                                                             const StructInfo* currentStruct,
                                                             LocalTypeMap& localTypes,
                                                             ArrayShapeMap* localArrayShapes,
                                                             std::vector<std::string>& extraLines) const {
    ParsedLocalDeclList parsed = parseLocalDeclList(line);
    if (!parsed.matched) {
        return {line};
    }

    std::vector<std::string> out;
    for (const auto& decl : parsed.decls) {
        if (decl.dimensions.size() > 1 && localArrayShapes) {
            (*localArrayShapes)[decl.name] = ArrayShape{decl.dimensions, false};
        }
        if (decl.type == "thistype") {
            localTypes[decl.name] = currentStruct ? currentStruct->originalName : decl.type;
        } else {
            localTypes[decl.name] = decl.type;
        }

        std::string rewrittenType = rewriteTypeName(decl.type, currentStruct);
        out.push_back("local " + std::string(decl.constant ? "constant " : "") + rewrittenType +
                      (decl.array ? " array " : " ") + decl.name);
        if (!decl.initializer.empty() && !decl.array) {
            extraLines.push_back("set " + decl.name + "=" +
                                 rewriteArrayAccesses(rewriteStructExpression(decl.initializer, currentStruct, localTypes),
                                                      localArrayShapes));
        }
    }
    return out;
}

std::string Phase1Codegen::rewriteStructExpression(const std::string& line,
                                                   const StructInfo* currentStruct,
                                                   const LocalTypeMap& localTypes) const {
    LineFeatures initialFeatures = scanLineFeatures(line, currentStruct, &localTypes, nullptr);
    const bool hasMemberAccess = initialFeatures.hasDot;
    const bool hasArrayReceiver = initialFeatures.hasBracket;
    const bool hasGeneratedStructPrefix = initialFeatures.hasGeneratedStructPrefix;
    const bool hasThistype = initialFeatures.hasThistype;
    if (!hasMemberAccess && !hasArrayReceiver && !hasGeneratedStructPrefix && !hasThistype &&
        !initialFeatures.hasPossibleStructMember) {
        ++performanceCounters_.linesSkippedNoDotBracketCall;
        if (!currentStruct) {
            ++performanceCounters_.linesSkippedNoCurrentStruct;
        }
        return line;
    }

    auto rewriteNormal = [&](std::string text) {
        const bool needsCurrentStructMemberRewrite =
            currentStruct &&
            (initialFeatures.hasPossibleStructMember ||
             initialFeatures.hasThis ||
             initialFeatures.hasThistype ||
             initialFeatures.hasGeneratedStructPrefix);
        if (needsCurrentStructMemberRewrite) {
            if (text.find("thistype") != std::string::npos) {
                text = replaceRegex(text, R"(\bthistype\s*\.\s*typeid\b)", std::to_string(currentStruct->typeId));
                text = replaceRegex(text, R"(\bthistype\s*\.\s*allocate\s*\()", currentStruct->prefix + "__allocate(");
                text = replaceRegex(text, R"(\bthistype\s*\.\s*create\s*\()", currentStruct->prefix + "_create(");
            }
        }
        bool hasMemberAccess = text.find('.') != std::string::npos;
        bool hasGeneratedStructPrefix = text.find("s__") != std::string::npos ||
                                        text.find("sc__") != std::string::npos ||
                                        text.find("si__") != std::string::npos;
        bool hasArrayReceiver = text.find('[') != std::string::npos;
        bool hasArrayStructReceiver = false;
        if (hasArrayReceiver && !arrayStructIndexes_.empty()) {
            if (auto cached = arrayStructReceiverCache_.find(text); cached != arrayStructReceiverCache_.end()) {
                hasArrayStructReceiver = cached->second;
            } else {
                for (size_t structIndex : arrayStructIndexes_) {
                    const auto& info = structs_[structIndex];
                    if (text.find(info.originalName) != std::string::npos ||
                        text.find(info.generatedName) != std::string::npos) {
                        hasArrayStructReceiver = true;
                        break;
                    }
                }
                arrayStructReceiverCache_[text] = hasArrayStructReceiver;
            }
        }
        if (hasMemberAccess || hasGeneratedStructPrefix || hasArrayStructReceiver) {
            auto rewriteIndexedStructReceivers = [&](const StructInfo& info,
                                                     const std::vector<std::string>& receiverNames) {
                std::vector<std::string> names;
                names.reserve(receiverNames.size());
                for (const auto& receiverName : receiverNames) {
                    if (receiverName.empty()) {
                        continue;
                    }
                    if (std::find(names.begin(), names.end(), receiverName) == names.end()) {
                        names.push_back(receiverName);
                    }
                }
                if (names.empty()) {
                    return;
                }

                bool changed = true;
                int guard = 0;
                while (changed && guard++ < 16) {
                    changed = false;
                    bool inString = false;
                    bool inRaw = false;
                    bool escaped = false;
                    for (size_t pos = 0; pos < text.size();) {
                        if (!inString && !inRaw && pos + 1 < text.size() && text[pos] == '/' && text[pos + 1] == '/') {
                            break;
                        }
                        char c = text[pos];
                        if (inString) {
                            if (escaped) {
                                escaped = false;
                            } else if (c == '\\') {
                                escaped = true;
                            } else if (c == '"') {
                                inString = false;
                            }
                            ++pos;
                            continue;
                        }
                        if (inRaw) {
                            if (c == '\'') {
                                inRaw = false;
                            }
                            ++pos;
                            continue;
                        }
                        if (c == '"') {
                            inString = true;
                            ++pos;
                            continue;
                        }
                        if (c == '\'') {
                            inRaw = true;
                            ++pos;
                            continue;
                        }

                        const std::string* matchedReceiver = nullptr;
                        for (const auto& receiverName : names) {
                            if (pos + receiverName.size() > text.size() ||
                                text.compare(pos, receiverName.size(), receiverName) != 0) {
                                continue;
                            }
                            if (pos > 0 && isIdentPart(text[pos - 1])) {
                                continue;
                            }
                            size_t afterName = pos + receiverName.size();
                            if (afterName < text.size() && isIdentPart(text[afterName])) {
                                continue;
                            }
                            matchedReceiver = &receiverName;
                            break;
                        }
                        if (!matchedReceiver) {
                            ++pos;
                            continue;
                        }

                        size_t afterName = pos + matchedReceiver->size();
                        while (afterName < text.size() && std::isspace(static_cast<unsigned char>(text[afterName]))) {
                            ++afterName;
                        }
                        if (afterName >= text.size() || text[afterName] != '[') {
                            pos += matchedReceiver->size();
                            continue;
                        }
                        size_t receiverClose = findMatchingBracketOutsideProtected(text, afterName);
                        if (receiverClose == std::string::npos) {
                            break;
                        }
                        std::string receiverExpr = trim(text.substr(afterName + 1, receiverClose - afterName - 1));
                        size_t receiverEnd = receiverClose + 1;
                        size_t cursor = receiverEnd;
                        while (cursor < text.size() && std::isspace(static_cast<unsigned char>(text[cursor]))) {
                            ++cursor;
                        }

                        if (cursor < text.size() && text[cursor] == '.') {
                            size_t memberStart = cursor + 1;
                            while (memberStart < text.size() &&
                                   std::isspace(static_cast<unsigned char>(text[memberStart]))) {
                                ++memberStart;
                            }
                            if (memberStart >= text.size() || !isIdentStart(text[memberStart])) {
                                pos = memberStart;
                                continue;
                            }
                            size_t memberEnd = memberStart + 1;
                            while (memberEnd < text.size() && isIdentPart(text[memberEnd])) {
                                ++memberEnd;
                            }
                            std::string memberName = text.substr(memberStart, memberEnd - memberStart);
                            size_t afterMember = memberEnd;
                            while (afterMember < text.size() &&
                                   std::isspace(static_cast<unsigned char>(text[afterMember]))) {
                                ++afterMember;
                            }

                            if (afterMember < text.size() && text[afterMember] == '(') {
                                const MethodInfo* method = findMethod(info, memberName);
                                if (method && !method->isStatic) {
                                    std::string replacement = method->generatedName + "(" + receiverExpr +
                                                              (method->decl->params.empty() ? "" : ", ");
                                    text.replace(pos, afterMember + 1 - pos, replacement);
                                    pos += replacement.size();
                                    changed = true;
                                    continue;
                                }
                            }

                            const FieldInfo* field = findField(info, memberName);
                            if (field && !field->isStatic) {
                                if (field->isFixedArray && field->fixedArraySize > 0 &&
                                    field->arrayDimensions.size() <= 1 &&
                                    afterMember < text.size() && text[afterMember] == '[') {
                                    size_t fieldIndexClose = findMatchingBracketOutsideProtected(text, afterMember);
                                    if (fieldIndexClose == std::string::npos) {
                                        break;
                                    }
                                    std::string indexExpr = trim(text.substr(afterMember + 1,
                                                                             fieldIndexClose - afterMember - 1));
                                    std::string replacement = fixedArrayStorageName(info, *field) + "[" +
                                                              field->generatedName + "[" + receiverExpr + "]+" +
                                                              indexExpr + "]";
                                    text.replace(pos, fieldIndexClose + 1 - pos, replacement);
                                    pos += replacement.size();
                                    changed = true;
                                    continue;
                                }
                                std::string replacement = field->generatedName + "[" + receiverExpr + "]";
                                text.replace(pos, memberEnd - pos, replacement);
                                pos += replacement.size();
                                changed = true;
                                continue;
                            }

                            pos = memberEnd;
                            continue;
                        }

                        std::string replacement = "(" + receiverExpr + ")";
                        text.replace(pos, receiverEnd - pos, replacement);
                        pos += replacement.size();
                        changed = true;
                    }
                }
            };

            std::vector<const StructInfo*> structRewriteInfos;
            auto addStructRewriteInfo = [&](const StructInfo* info) {
                if (info &&
                    std::find(structRewriteInfos.begin(), structRewriteInfos.end(), info) == structRewriteInfos.end()) {
                    structRewriteInfos.push_back(info);
                }
            };
            if (hasGeneratedStructPrefix) {
                for (const auto& info : structs_) {
                    addStructRewriteInfo(&info);
                }
            } else {
                if (currentStruct && text.find("thistype") != std::string::npos) {
                    addStructRewriteInfo(currentStruct);
                }
                bool inString = false;
                bool inRaw = false;
                bool escaped = false;
                for (size_t pos = 0; pos < text.size();) {
                    if (!inString && !inRaw && pos + 1 < text.size() && text[pos] == '/' && text[pos + 1] == '/') {
                        break;
                    }
                    char c = text[pos];
                    if (inString) {
                        if (escaped) {
                            escaped = false;
                        } else if (c == '\\') {
                            escaped = true;
                        } else if (c == '"') {
                            inString = false;
                        }
                        ++pos;
                        continue;
                    }
                    if (inRaw) {
                        if (c == '\'') {
                            inRaw = false;
                        }
                        ++pos;
                        continue;
                    }
                    if (c == '"') {
                        inString = true;
                        ++pos;
                        continue;
                    }
                    if (c == '\'') {
                        inRaw = true;
                        ++pos;
                        continue;
                    }
                    if (!isIdentStart(c)) {
                        ++pos;
                        continue;
                    }
                    size_t nameStart = pos++;
                    while (pos < text.size() && isIdentPart(text[pos])) {
                        ++pos;
                    }
                    std::string receiverName = text.substr(nameStart, pos - nameStart);
                    size_t cursor = pos;
                    while (cursor < text.size() && std::isspace(static_cast<unsigned char>(text[cursor]))) {
                        ++cursor;
                    }
                    bool sawReceiverIndex = false;
                    while (cursor < text.size() && text[cursor] == '[') {
                        size_t close = findMatchingBracketOutsideProtected(text, cursor);
                        if (close == std::string::npos) {
                            break;
                        }
                        sawReceiverIndex = true;
                        cursor = close + 1;
                        while (cursor < text.size() && std::isspace(static_cast<unsigned char>(text[cursor]))) {
                            ++cursor;
                        }
                    }
                    if (sawReceiverIndex || (cursor < text.size() && text[cursor] == '.')) {
                        if (receiverName == "this" || receiverName == "thistype" ||
                            receiverName == "function" || receiverName == "call" ||
                            receiverName == "set" || receiverName == "return" ||
                            localTypes.find(receiverName) != localTypes.end() ||
                            globalStructTypes_.find(receiverName) != globalStructTypes_.end()) {
                            continue;
                        }
                        auto structIt = structIndexByName_.find(receiverName);
                        if (structIt != structIndexByName_.end()) {
                            addStructRewriteInfo(&structs_[structIt->second]);
                        }
                    }
                }
            }
            for (const StructInfo* infoPtr : structRewriteInfos) {
                const auto& info = *infoPtr;
                auto legacyStructPrefixPattern = [&](std::string_view prefix) {
                    std::string pattern = "\\b" + std::string(prefix) + regexEscape(info.originalName) + "_";
                    std::string scopedPrefix = info.originalName + "_";
                    if (info.generatedName.rfind(scopedPrefix, 0) == 0) {
                        std::string generatedSuffix = info.generatedName.substr(scopedPrefix.size());
                        if (!generatedSuffix.empty()) {
                            pattern += "(?!" + regexEscape(generatedSuffix) + "_)";
                        }
                    }
                    return pattern;
                };
                if (text.find("si__" + info.originalName + "_") != std::string::npos) {
                    text = replaceRegex(text, legacyStructPrefixPattern("si__"), "si__" + info.generatedName + "_");
                }
                if (text.find("sc__" + info.originalName + "_") != std::string::npos) {
                    text = replaceRegex(text, legacyStructPrefixPattern("sc__"), "sc__" + info.generatedName + "_");
                }
                if (text.find("s__" + info.originalName + "_") != std::string::npos) {
                    text = replaceRegex(text, legacyStructPrefixPattern("s__"), "s__" + info.generatedName + "_");
                }
                if (text.find(info.originalName) == std::string::npos &&
                    text.find(info.generatedName) == std::string::npos) {
                    continue;
                }
                std::string structName = regexEscape(info.originalName);
                text = replaceRegex(text,
                                    "\\b" + structName + R"(\s*\.\s*typeid\b)",
                                    std::to_string(info.typeId));
                for (const auto& method : info.methods) {
                    if (method.isStatic || method.name == "create") {
                        text = replaceRegex(text,
                                            "\\bfunction\\s+" + structName + "\\s*\\.\\s*" + regexEscape(method.name) + "\\b",
                                            "function " + method.generatedName);
                        text = replaceRegex(text,
                                            "\\b" + structName + "\\s*\\.\\s*" + regexEscape(method.name) + "\\s*\\(",
                                            method.generatedName + "(");
                    }
                }
                if (!info.isArrayStruct) {
                    text = replaceRegex(text, "\\b" + structName + "\\s*\\.\\s*create\\s*\\(", info.prefix + "_create(");
                    text = replaceRegex(text, "\\b" + structName + "\\s*\\.\\s*allocate\\s*\\(", info.prefix + "__allocate(");
                }
                for (const auto& field : info.fields) {
                    if (field.isStatic) {
                        text = replaceRegex(text,
                                            "\\b" + structName + "\\s*\\.\\s*" + regexEscape(field.name) + "\\b",
                                            field.generatedName);
                        text = rewriteArrayAccesses(text, nullptr);
                    }
                }
                if (info.isArrayStruct) {
                    std::vector<std::string> receiverNames{info.originalName, info.generatedName};
                    if (currentStruct == &info && text.find("thistype") != std::string::npos) {
                        receiverNames.push_back("thistype");
                    }
                    rewriteIndexedStructReceivers(info, receiverNames);
                }
            }
        }

        auto rewriteNamedStructReceiver = [&](const StructInfo& varStruct, const std::string& receiverName) {
            if (receiverName.empty() || text.find(receiverName) == std::string::npos) {
                return;
            }
            bool changed = true;
            int guard = 0;
            while (changed && guard++ < 16) {
                changed = false;
                for (size_t pos = 0; pos < text.size();) {
                    size_t namePos = text.find(receiverName, pos);
                    if (namePos == std::string::npos) {
                        break;
                    }
                    if (namePos > 0 && isIdentPart(text[namePos - 1])) {
                        pos = namePos + receiverName.size();
                        continue;
                    }
                    size_t afterName = namePos + receiverName.size();
                    if (afterName < text.size() && isIdentPart(text[afterName])) {
                        pos = afterName;
                        continue;
                    }

                    size_t receiverEnd = afterName;
                    size_t cursor = afterName;
                    while (cursor < text.size() && std::isspace(static_cast<unsigned char>(text[cursor]))) {
                        ++cursor;
                    }
                    while (cursor < text.size() && text[cursor] == '[') {
                        size_t close = findMatchingBracketOutsideProtected(text, cursor);
                        if (close == std::string::npos) {
                            pos = cursor + 1;
                            break;
                        }
                        receiverEnd = close + 1;
                        cursor = receiverEnd;
                        while (cursor < text.size() && std::isspace(static_cast<unsigned char>(text[cursor]))) {
                            ++cursor;
                        }
                    }
                    if (cursor >= text.size() || text[cursor] != '.') {
                        pos = receiverEnd;
                        continue;
                    }
                    size_t memberStart = cursor + 1;
                    while (memberStart < text.size() && std::isspace(static_cast<unsigned char>(text[memberStart]))) {
                        ++memberStart;
                    }
                    if (memberStart >= text.size() || !isIdentStart(text[memberStart])) {
                        pos = memberStart;
                        continue;
                    }
                    size_t memberEnd = memberStart + 1;
                    while (memberEnd < text.size() && isIdentPart(text[memberEnd])) {
                        ++memberEnd;
                    }
                    std::string memberName = text.substr(memberStart, memberEnd - memberStart);
                    std::string receiverExpr = trim(text.substr(namePos, receiverEnd - namePos));
                    size_t afterMember = memberEnd;
                    while (afterMember < text.size() && std::isspace(static_cast<unsigned char>(text[afterMember]))) {
                        ++afterMember;
                    }

                    if (!varStruct.isArrayStruct && (memberName == "destroy" || memberName == "deallocate") &&
                        afterMember < text.size() && text[afterMember] == '(') {
                        size_t close = findMatchingParen(text, afterMember);
                        if (close == std::string::npos) {
                            break;
                        }
                        if (trim(text.substr(afterMember + 1, close - afterMember - 1)).empty()) {
                            std::string replacement = varStruct.prefix +
                                                      (memberName == "deallocate" ? "_deallocate(" : "_destroy(") +
                                                      receiverExpr + ")";
                            text.replace(namePos, close + 1 - namePos, replacement);
                            pos = namePos + replacement.size();
                            changed = true;
                            continue;
                        }
                    }

                    if (afterMember < text.size() && text[afterMember] == '(') {
                        const MethodInfo* method = findMethod(varStruct, memberName);
                        if (method && !method->isStatic) {
                            std::string replacement = method->generatedName + "(" + receiverExpr +
                                                      (method->decl->params.empty() ? "" : ", ");
                            text.replace(namePos, afterMember + 1 - namePos, replacement);
                            pos = namePos + replacement.size();
                            changed = true;
                            continue;
                        }
                    }

                    const FieldInfo* field = findField(varStruct, memberName);
                    if (field && !field->isStatic) {
                        if (field->isFixedArray && field->fixedArraySize > 0 && field->arrayDimensions.size() <= 1 &&
                            afterMember < text.size() && text[afterMember] == '[') {
                            size_t close = findMatchingBracketOutsideProtected(text, afterMember);
                            if (close == std::string::npos) {
                                break;
                            }
                            std::string indexExpr = trim(text.substr(afterMember + 1, close - afterMember - 1));
                            std::string replacement = fixedArrayStorageName(varStruct, *field) + "[" +
                                                      field->generatedName + "[" + receiverExpr + "]+" + indexExpr + "]";
                            text.replace(namePos, close + 1 - namePos, replacement);
                            pos = namePos + replacement.size();
                            changed = true;
                            continue;
                        }
                        std::string replacement = field->generatedName + "[" + receiverExpr + "]";
                        text.replace(namePos, memberEnd - namePos, replacement);
                        pos = namePos + replacement.size();
                        changed = true;
                        continue;
                    }

                    pos = memberEnd;
                }
            }
        };

        if (needsCurrentStructMemberRewrite) {
            const bool textHasThis = text.find("this") != std::string::npos;
            const bool textHasThistype = text.find("thistype") != std::string::npos;
            const bool textHasDot = text.find('.') != std::string::npos;
            std::vector<const FieldInfo*> fieldsToRewrite;
            if (initialFeatures.hasGeneratedStructPrefix) {
                fieldsToRewrite.reserve(currentStruct->fields.size());
                for (const auto& field : currentStruct->fields) {
                    fieldsToRewrite.push_back(&field);
                }
            } else {
                fieldsToRewrite = initialFeatures.currentStructFields;
            }
            for (const FieldInfo* fieldPtr : fieldsToRewrite) {
                const auto& field = *fieldPtr;
                if (text.find(field.name) == std::string::npos) {
                    if (text.find(field.generatedName) == std::string::npos) {
                        continue;
                    }
                }
                if (field.isStatic) {
                    if (!localTypes.contains(field.name)) {
                        ++performanceCounters_.methodPlanBareFieldRewriteAttempts;
                        if (replaceBareIdentifierToken(text, field.name, field.generatedName, false)) {
                            ++performanceCounters_.methodPlanBareFieldRewriteChanged;
                        }
                    }
                    if (textHasThistype) {
                        text = replaceRegex(text,
                                            R"(\bthistype\s*\.\s*)" + regexEscape(field.name) + R"(\b)",
                                            field.generatedName);
                        text = replaceRegex(text,
                                            R"(\bthistype\s*\.\s*)" + regexEscape(field.generatedName) + R"(\b)",
                                            field.generatedName);
                    }
                    if (textHasThis) {
                        text = replaceRegex(text,
                                            R"(\bthis\s*\.\s*)" + regexEscape(field.name) + R"(\b)",
                                            field.generatedName);
                        text = replaceRegex(text,
                                            R"(\bthis\s*\.\s*)" + regexEscape(field.generatedName) + R"(\b)",
                                            field.generatedName);
                    }
                    text = rewriteArrayAccesses(text, nullptr);
                    if (const StructInfo* fieldStruct = findStruct(field.typeName)) {
                        rewriteNamedStructReceiver(*fieldStruct, field.generatedName);
                    }
                    continue;
                }
                if (field.isFixedArray && field.fixedArraySize > 0 && field.arrayDimensions.size() <= 1) {
                    std::string storage = fixedArrayStorageName(*currentStruct, field);
                    std::string indexed = storage + "[" + field.generatedName + "[this]+$1]";
                    if (!localTypes.contains(field.name)) {
                        ++performanceCounters_.methodPlanBareFieldRewriteAttempts;
                        std::string rewritten = replaceRegex(text,
                                                             R"((^|[^A-Za-z0-9_$.]))" + regexEscape(field.name) + R"(\s*\[\s*([^\]]+)\s*\])",
                                                             "$1" + storage + "[" + field.generatedName + "[this]+$2]");
                        if (rewritten != text) {
                            ++performanceCounters_.methodPlanBareFieldRewriteChanged;
                        }
                        text = std::move(rewritten);
                    }
                    if (textHasThistype) {
                        text = replaceRegex(text,
                                            R"(\bthistype\s*\[\s*([^\]]+)\s*\]\s*\.\s*)" + regexEscape(field.name) + R"(\s*\[\s*([^\]]+)\s*\])",
                                            storage + "[" + field.generatedName + "[$1]+$2]");
                    }
                    if (textHasThis) {
                        text = replaceRegex(text,
                                            R"(\bthis\s*\.\s*)" + regexEscape(field.name) + R"(\s*\[\s*([^\]]+)\s*\])",
                                            indexed);
                    }
                    if (textHasDot) {
                        text = replaceRegex(text,
                                            R"((^|[^A-Za-z0-9_$)\]])\.\s*)" + regexEscape(field.name) + R"(\s*\[\s*([^\]]+)\s*\])",
                                            "$1" + storage + "[" + field.generatedName + "[this]+$2]");
                    }
                }
                std::string access = field.generatedName + "[this]";
                if (!localTypes.contains(field.name)) {
                    ++performanceCounters_.methodPlanBareFieldRewriteAttempts;
                    if (replaceBareIdentifierToken(text, field.name, access, true)) {
                        ++performanceCounters_.methodPlanBareFieldRewriteChanged;
                    }
                }
                if (textHasThistype) {
                    text = replaceRegex(text,
                                        R"(\bthistype\s*\[\s*([^\]]+)\s*\]\s*\.\s*)" + regexEscape(field.name) + R"(\b)",
                                        field.generatedName + "[$1]");
                }
                if (textHasThis) {
                    text = replaceRegex(text, R"(\bthis\s*\.\s*)" + regexEscape(field.name) + R"(\b)", access);
                }
                if (textHasDot) {
                    text = replaceRegex(text, R"((^|[^A-Za-z0-9_$)\]])\.\s*)" + regexEscape(field.name) + R"(\b)", "$1" + access);
                }
            }
            std::vector<const MethodInfo*> methodsToRewrite;
            if (initialFeatures.hasGeneratedStructPrefix) {
                methodsToRewrite.reserve(currentStruct->methods.size());
                for (const auto& method : currentStruct->methods) {
                    methodsToRewrite.push_back(&method);
                }
            } else {
                methodsToRewrite = initialFeatures.currentStructMethods;
            }
            for (const MethodInfo* methodPtr : methodsToRewrite) {
                const auto& method = *methodPtr;
                if (text.find(method.name) == std::string::npos &&
                    text.find(method.generatedName) == std::string::npos &&
                    text.find("thistype") == std::string::npos) {
                    continue;
                }
                if (method.isStatic) {
                    if (textHasThistype) {
                        text = replaceRegex(text,
                                            R"(\bfunction\s+thistype\s*\.\s*)" + regexEscape(method.name) + R"(\b)",
                                            "function " + method.generatedName);
                        text = replaceRegex(text,
                                            R"(\bfunction\s+thistype\s*\.\s*)" + regexEscape(method.generatedName) + R"(\b)",
                                            "function " + method.generatedName);
                        text = replaceRegex(text,
                                            R"(\bthistype\s*\.\s*)" + regexEscape(method.name) + R"(\s*\()",
                                            method.generatedName + "(");
                        text = replaceRegex(text,
                                            R"(\bthistype\s*\.\s*)" + regexEscape(method.generatedName) + R"(\s*\()",
                                            method.generatedName + "(");
                    }
                    if (!localTypes.contains(method.name)) {
                        replaceBareCallToken(text, method.name, method.generatedName + "(");
                    }
                    continue;
                }
                if (text.find(method.name) == std::string::npos) {
                    continue;
                }
                if (textHasThistype) {
                    text = replaceRegex(text,
                                        R"(\bthistype\s*\[\s*([^\]]+)\s*\]\s*\.\s*)" + regexEscape(method.name) + R"(\s*\()",
                                        method.generatedName + "($1" + (method.decl->params.empty() ? "" : ", "));
                }
                if (textHasDot) {
                    text = replaceRegex(text,
                                        R"((^|[^A-Za-z0-9_$)\]])\.\s*)" + regexEscape(method.name) + R"(\s*\()",
                                        "$1" + method.generatedName + "(this" + (method.decl->params.empty() ? "" : ", "));
                }
                if (!localTypes.contains(method.name)) {
                    replaceBareCallToken(text,
                                         method.name,
                                         method.generatedName + "(this" + (method.decl->params.empty() ? "" : ", "));
                }
            }
            if (!currentStruct->isArrayStruct && text.find("allocate") != std::string::npos) {
                text = replaceRegex(text, R"(\ballocate\s*\()", currentStruct->prefix + "__allocate(");
            }
            auto rewriteBareLifecycleCall = [&](const std::string& name, const std::string& target) {
                if (localTypes.contains(name) || text.find(name) == std::string::npos) {
                    return;
                }
                for (size_t pos = 0; (pos = text.find(name, pos)) != std::string::npos;) {
                    if (pos > 0 && (isIdentPart(text[pos - 1]) || text[pos - 1] == '.')) {
                        pos += name.size();
                        continue;
                    }
                    size_t afterName = pos + name.size();
                    if (afterName < text.size() && isIdentPart(text[afterName])) {
                        pos = afterName;
                        continue;
                    }
                    while (afterName < text.size() && std::isspace(static_cast<unsigned char>(text[afterName]))) {
                        ++afterName;
                    }
                    if (afterName >= text.size() || text[afterName] != '(') {
                        pos = afterName;
                        continue;
                    }
                    size_t close = findMatchingParen(text, afterName);
                    if (close == std::string::npos) {
                        break;
                    }
                    if (!trim(text.substr(afterName + 1, close - afterName - 1)).empty()) {
                        pos = close + 1;
                        continue;
                    }
                    std::string replacement = currentStruct->prefix + target + "(this)";
                    text.replace(pos, close + 1 - pos, replacement);
                    pos += replacement.size();
                }
            };
            if (!currentStruct->isArrayStruct) {
                rewriteBareLifecycleCall("destroy", "_destroy");
                rewriteBareLifecycleCall("deallocate", "_deallocate");
            }
        }

        if (text.find('.') != std::string::npos) {
            ++performanceCounters_.memberAccessScans;
            auto collectReceiverNames = [&]() {
                std::vector<std::string> receiverNames;
                bool inString = false;
                bool inRaw = false;
                bool escaped = false;
                for (size_t pos = 0; pos < text.size();) {
                    if (!inString && !inRaw && pos + 1 < text.size() && text[pos] == '/' && text[pos + 1] == '/') {
                        break;
                    }
                    char c = text[pos];
                    if (inString) {
                        if (escaped) {
                            escaped = false;
                        } else if (c == '\\') {
                            escaped = true;
                        } else if (c == '"') {
                            inString = false;
                        }
                        ++pos;
                        continue;
                    }
                    if (inRaw) {
                        if (c == '\'') {
                            inRaw = false;
                        }
                        ++pos;
                        continue;
                    }
                    if (c == '"') {
                        inString = true;
                        ++pos;
                        continue;
                    }
                    if (c == '\'') {
                        inRaw = true;
                        ++pos;
                        continue;
                    }
                    if (!isIdentStart(c)) {
                        ++pos;
                        continue;
                    }
                    size_t nameStart = pos++;
                    while (pos < text.size() && isIdentPart(text[pos])) {
                        ++pos;
                    }
                    size_t nameEnd = pos;
                    size_t cursor = pos;
                    while (cursor < text.size() && std::isspace(static_cast<unsigned char>(text[cursor]))) {
                        ++cursor;
                    }
                    while (cursor < text.size() && text[cursor] == '[') {
                        size_t close = findMatchingBracketOutsideProtected(text, cursor);
                        if (close == std::string::npos) {
                            break;
                        }
                        cursor = close + 1;
                        while (cursor < text.size() && std::isspace(static_cast<unsigned char>(text[cursor]))) {
                            ++cursor;
                        }
                    }
                    if (cursor < text.size() && text[cursor] == '.') {
                        std::string_view receiverName(text.data() + nameStart, nameEnd - nameStart);
                        bool seen = false;
                        for (const auto& existing : receiverNames) {
                            if (existing == receiverName) {
                                seen = true;
                                break;
                            }
                        }
                        if (!seen) {
                            receiverNames.emplace_back(receiverName);
                        }
                    }
                }
                return receiverNames;
            };
            std::vector<std::string> receiverNames = collectReceiverNames();
            std::string beforeLocalReceiverRewrite = text;
            for (const auto& varName : receiverNames) {
                if (auto localIt = localTypes.find(varName); localIt != localTypes.end()) {
                    const StructInfo* varStruct = localIt->second == "thistype" ? currentStruct : findStruct(localIt->second);
                    if (varStruct) {
                        rewriteNamedStructReceiver(*varStruct, varName);
                    }
                }
            }
            std::vector<std::string> refreshedReceiverNames;
            const std::vector<std::string>* globalReceiverNames = &receiverNames;
            if (text != beforeLocalReceiverRewrite) {
                refreshedReceiverNames = collectReceiverNames();
                globalReceiverNames = &refreshedReceiverNames;
            }
            for (const auto& varName : *globalReceiverNames) {
                if (auto globalIt = globalStructTypes_.find(varName); globalIt != globalStructTypes_.end()) {
                    if (const StructInfo* varStruct = findStruct(globalIt->second)) {
                        rewriteNamedStructReceiver(*varStruct, varName);
                    }
                }
            }
        }
        return text;
    };
    std::string rewritten = rewriteOutsideProtected(line, rewriteNormal);
    if (rewritten.find('.') == std::string::npos) {
        return rewritten;
    }
    return rewriteReceiverChains(rewritten, currentStruct);
}

std::string Phase1Codegen::rewriteReceiverChains(const std::string& line, const StructInfo* currentStruct) const {
    ++performanceCounters_.receiverChainAttempts;
    auto structForType = [&](const std::string& typeName, const StructInfo* activeStruct) -> const StructInfo* {
        if (typeName == "thistype") {
            return activeStruct;
        }
        return findStruct(typeName);
    };
    auto structForReturningFunction = [&](const std::string& functionName) -> const StructInfo* {
        if (const FunctionInfo* fn = findFunctionInfo(functionName)) {
            if (const StructInfo* info = structForType(fn->signature.returnType, currentStruct)) {
                return info;
            }
        }
        for (const auto& info : structs_) {
            if (!info.isArrayStruct && (functionName == info.prefix + "_create" || functionName == info.prefix + "__allocate")) {
                return &info;
            }
            for (const auto& method : info.methods) {
                if (functionName == method.generatedName) {
                    if (const StructInfo* returned = structForType(method.decl->returnType.name, &info)) {
                        return returned;
                    }
                }
            }
        }
        return nullptr;
    };

    std::string text = line;
    bool anyChanged = true;
    int guard = 0;
    while (anyChanged && guard++ < 8) {
        anyChanged = false;
        bool inString = false;
        bool inRaw = false;
        bool escaped = false;
        for (size_t pos = 0; pos < text.size();) {
            if (!inString && !inRaw && pos + 1 < text.size() && text[pos] == '/' && text[pos + 1] == '/') {
                break;
            }
            char current = text[pos];
            if (inString) {
                if (escaped) {
                    escaped = false;
                } else if (current == '\\') {
                    escaped = true;
                } else if (current == '"') {
                    inString = false;
                }
                ++pos;
                continue;
            }
            if (inRaw) {
                if (current == '\'') {
                    inRaw = false;
                }
                ++pos;
                continue;
            }
            if (current == '"') {
                inString = true;
                ++pos;
                continue;
            }
            if (current == '\'') {
                inRaw = true;
                ++pos;
                continue;
            }
            if (!isIdentStart(text[pos])) {
                ++pos;
                continue;
            }
            size_t nameStart = pos++;
            while (pos < text.size() && isIdentPart(text[pos])) {
                ++pos;
            }
            if (nameStart > 0 && isIdentPart(text[nameStart - 1])) {
                continue;
            }
            std::string functionName = text.substr(nameStart, pos - nameStart);
            size_t open = pos;
            while (open < text.size() && std::isspace(static_cast<unsigned char>(text[open]))) {
                ++open;
            }
            if (open >= text.size() || text[open] != '(') {
                pos = open;
                continue;
            }
            size_t close = findMatchingParen(text, open);
            if (close == std::string::npos) {
                break;
            }
            const StructInfo* receiverStruct = structForReturningFunction(functionName);
            if (!receiverStruct) {
                pos = open + 1;
                continue;
            }

            std::string receiverExpr = text.substr(nameStart, close + 1 - nameStart);
            size_t chainPos = close + 1;
            bool changed = false;
            while (chainPos < text.size()) {
                size_t ws = chainPos;
                while (ws < text.size() && std::isspace(static_cast<unsigned char>(text[ws]))) {
                    ++ws;
                }
                if (ws >= text.size() || text[ws] != '.') {
                    break;
                }
                size_t memberStart = ws + 1;
                while (memberStart < text.size() && std::isspace(static_cast<unsigned char>(text[memberStart]))) {
                    ++memberStart;
                }
                if (memberStart >= text.size() || !isIdentStart(text[memberStart])) {
                    break;
                }
                size_t memberEnd = memberStart + 1;
                while (memberEnd < text.size() && isIdentPart(text[memberEnd])) {
                    ++memberEnd;
                }
                std::string memberName = text.substr(memberStart, memberEnd - memberStart);
                size_t afterMember = memberEnd;
                while (afterMember < text.size() && std::isspace(static_cast<unsigned char>(text[afterMember]))) {
                    ++afterMember;
                }
                if (afterMember < text.size() && text[afterMember] == '(') {
                    size_t methodClose = findMatchingParen(text, afterMember);
                    if (methodClose == std::string::npos) {
                        break;
                    }
                    if (!receiverStruct->isArrayStruct && (memberName == "destroy" || memberName == "deallocate") &&
                        trim(text.substr(afterMember + 1, methodClose - afterMember - 1)).empty()) {
                        receiverExpr = receiverStruct->prefix +
                                       (memberName == "deallocate" ? "_deallocate(" : "_destroy(") +
                                       receiverExpr + ")";
                        chainPos = methodClose + 1;
                        changed = true;
                        break;
                    }
                    const MethodInfo* method = findMethod(*receiverStruct, memberName);
                    if (!method || method->isStatic) {
                        break;
                    }
                    std::string args = trim(text.substr(afterMember + 1, methodClose - afterMember - 1));
                    receiverExpr = method->generatedName + "(" + receiverExpr + (args.empty() ? "" : ", " + args) + ")";
                    receiverStruct = structForType(method->decl->returnType.name, receiverStruct);
                    chainPos = methodClose + 1;
                    changed = true;
                    if (!receiverStruct) {
                        break;
                    }
                    continue;
                }

                const FieldInfo* field = findField(*receiverStruct, memberName);
                if (!field || field->isStatic) {
                    break;
                }
                receiverExpr = field->generatedName + "[" + receiverExpr + "]";
                receiverStruct = structForType(field->typeName == "thistype" ? receiverStruct->originalName : field->typeName,
                                               receiverStruct);
                chainPos = memberEnd;
                changed = true;
                if (!receiverStruct) {
                    break;
                }
            }

            if (changed) {
                text.replace(nameStart, chainPos - nameStart, receiverExpr);
                pos = nameStart + receiverExpr.size();
                anyChanged = true;
            } else {
                pos = open + 1;
            }
        }
    }
    if (text != line) {
        ++performanceCounters_.receiverChainChanged;
    }
    return text;
}

const Phase1Codegen::FunctionInterfaceInfo* Phase1Codegen::findFunctionInterface(std::string_view name) const {
    auto it = functionInterfaceIndexByName_.find(std::string(name));
    return it == functionInterfaceIndexByName_.end() ? nullptr : &functionInterfaces_[it->second];
}

std::string Phase1Codegen::functionSignatureKey(const FunctionSignature& signature) const {
    std::string key = rewriteTypeName(signature.returnType, nullptr);
    key.push_back('|');
    for (const auto& paramType : signature.paramTypes) {
        key += rewriteTypeName(paramType, nullptr);
        key.push_back(';');
    }
    return key;
}

Phase1Codegen::FunctionInterfaceInfo& Phase1Codegen::ensureFunctionObjectInterface(const FunctionSignature& signature) const {
    std::string key = functionSignatureKey(signature);
    if (auto it = functionObjectInterfaceIndexBySignature_.find(key); it != functionObjectInterfaceIndexBySignature_.end()) {
        return functionInterfaces_[it->second];
    }
    FunctionInterfaceInfo info;
    info.sourceName = "vjfo__prototype" + std::to_string(functionObjectInterfaceIndexBySignature_.size() + 1);
    info.finalName = info.sourceName;
    info.signature = signature;
    info.syntheticFunctionObject = true;
    size_t index = functionInterfaces_.size();
    functionInterfaces_.push_back(std::move(info));
    functionInterfaceIndexByName_[functionInterfaces_[index].sourceName] = index;
    functionInterfaceIndexByName_[functionInterfaces_[index].finalName] = index;
    functionObjectInterfaceIndexBySignature_[std::move(key)] = index;
    return functionInterfaces_[index];
}

const Phase1Codegen::FunctionInterfaceInfo* Phase1Codegen::findFunctionObjectInterface(const FunctionSignature& signature) const {
    auto it = functionObjectInterfaceIndexBySignature_.find(functionSignatureKey(signature));
    return it == functionObjectInterfaceIndexBySignature_.end() ? nullptr : &functionInterfaces_[it->second];
}

std::string Phase1Codegen::resolveFunctionInterfaceTypeName(std::string_view name, const Decl* container) const {
    std::string type = trim(name);
    if (type.empty()) {
        return {};
    }
    if (const FunctionInterfaceInfo* iface = findFunctionInterface(type)) {
        return iface->finalName;
    }
    if (container) {
        std::string rewritten = rewriteForContainer(type, container);
        if (const FunctionInterfaceInfo* iface = findFunctionInterface(rewritten)) {
            return iface->finalName;
        }
        std::string scoped = renameInContainer(type, container);
        if (const FunctionInterfaceInfo* iface = findFunctionInterface(scoped)) {
            return iface->finalName;
        }
    }
    return {};
}

const Phase1Codegen::FunctionInfo* Phase1Codegen::findFunctionInfo(std::string_view name) const {
    ++performanceCounters_.functionLookupCalls;
    if (auto cached = functionLookupCache_.find(name); cached != functionLookupCache_.end()) {
        ++performanceCounters_.cachedRewriteHits;
        return cached->second;
    }
    ++performanceCounters_.cachedRewriteMisses;
    auto it = functionIndexByName_.find(name);
    const FunctionInfo* result = it == functionIndexByName_.end() ? nullptr : &functions_[it->second];
    functionLookupCache_.emplace(std::string(name), result);
    return result;
}

std::string Phase1Codegen::interfaceGlobalPrefix(const FunctionInterfaceInfo& iface) const {
    return "vjfi__" + sanitizeName(iface.finalName);
}

std::string Phase1Codegen::resolveFunctionTargetName(const std::string& expression, const StructInfo* currentStruct) const {
    std::string expr = trim(expression);
    if (startsWithWord(expr, "function")) {
        expr = trim(std::string_view(expr).substr(8));
    }
    if (const FunctionInfo* fn = findFunctionInfo(expr)) {
        return fn->finalName;
    }
    size_t dot = expr.find('.');
    if (dot != std::string::npos) {
        std::string left = trim(std::string_view(expr).substr(0, dot));
        std::string right = trim(std::string_view(expr).substr(dot + 1));
        if (currentStruct && (left == "thistype" || left == currentStruct->originalName || left == currentStruct->generatedName)) {
            if (const MethodInfo* method = findMethod(*currentStruct, right); method && method->isStatic) {
                return method->generatedName;
            }
        }
        if (const StructInfo* info = findStruct(left)) {
            if (const MethodInfo* method = findMethod(*info, right); method && method->isStatic) {
                return method->generatedName;
            }
        }
    }
    return expr;
}

int Phase1Codegen::registerInterfaceTarget(const FunctionInterfaceInfo& ifaceRef, const std::string& targetName, SourceLocation loc) const {
    auto ifaceIt = functionInterfaceIndexByName_.find(ifaceRef.finalName);
    if (ifaceIt == functionInterfaceIndexByName_.end()) {
        ifaceIt = functionInterfaceIndexByName_.find(ifaceRef.sourceName);
    }
    if (ifaceIt == functionInterfaceIndexByName_.end()) {
        diagnostics_.error(loc, "unknown function interface '" + ifaceRef.sourceName + "'");
        return 0;
    }
    auto& iface = functionInterfaces_[ifaceIt->second];
    std::string finalTarget = resolveFunctionTargetName(targetName, nullptr);
    if (const FunctionInfo* fn = findFunctionInfo(finalTarget)) {
        finalTarget = fn->finalName;
        if (!sameSignature(fn->signature, iface.signature)) {
            diagnostics_.error(loc, "function target signature mismatch for interface '" + iface.sourceName + "'");
        }
    } else {
        diagnostics_.error(loc, "unknown function target '" + targetName + "'");
    }
    auto existing = iface.targetIndexByFinalName.find(finalTarget);
    if (existing != iface.targetIndexByFinalName.end()) {
        return iface.targets[existing->second].id;
    }
    int id = static_cast<int>(iface.targets.size() + 1);
    size_t index = iface.targets.size();
    iface.targets.push_back(InterfaceTarget{finalTarget, id});
    iface.targetIndexByFinalName[finalTarget] = index;
    return id;
}

void Phase1Codegen::markInterfaceTargetNeedsCondition(const FunctionInterfaceInfo& ifaceRef,
                                                      const std::string& rewrittenReceiverExpression,
                                                      SourceLocation loc) const {
    auto ifaceIt = functionInterfaceIndexByName_.find(ifaceRef.finalName);
    if (ifaceIt == functionInterfaceIndexByName_.end()) {
        ifaceIt = functionInterfaceIndexByName_.find(ifaceRef.sourceName);
    }
    if (ifaceIt == functionInterfaceIndexByName_.end()) {
        diagnostics_.error(loc, "unknown function interface '" + ifaceRef.sourceName + "'");
        return;
    }
    auto& iface = functionInterfaces_[ifaceIt->second];
    if (auto literalId = parsePositiveIntegerLiteral(rewrittenReceiverExpression)) {
        for (auto& target : iface.targets) {
            if (target.id == *literalId) {
                target.needsCondition = true;
                return;
            }
        }
    }
    iface.allTargetsNeedCondition = true;
}

void Phase1Codegen::markInterfaceTargetNeedsAction(const FunctionInterfaceInfo& ifaceRef,
                                                   const std::string& rewrittenReceiverExpression,
                                                   SourceLocation loc) const {
    auto ifaceIt = functionInterfaceIndexByName_.find(ifaceRef.finalName);
    if (ifaceIt == functionInterfaceIndexByName_.end()) {
        ifaceIt = functionInterfaceIndexByName_.find(ifaceRef.sourceName);
    }
    if (ifaceIt == functionInterfaceIndexByName_.end()) {
        diagnostics_.error(loc, "unknown function interface '" + ifaceRef.sourceName + "'");
        return;
    }
    auto& iface = functionInterfaces_[ifaceIt->second];
    if (auto literalId = parsePositiveIntegerLiteral(rewrittenReceiverExpression)) {
        for (auto& target : iface.targets) {
            if (target.id == *literalId) {
                target.needsAction = true;
                return;
            }
        }
    }
    iface.allTargetsNeedAction = true;
}

const Phase1Codegen::FunctionInterfaceInfo* Phase1Codegen::resolveReceiverInterface(const std::string& receiver,
                                                                                    const LoweringContext& ctx) const {
    std::string expr = trim(receiver);
    if (ctx.localTypes) {
        auto it = ctx.localTypes->find(expr);
        if (it != ctx.localTypes->end()) {
            return findFunctionInterface(it->second);
        }
    }
    size_t dot = expr.find('.');
    if (dot != std::string::npos) {
        std::string left = trim(std::string_view(expr).substr(0, dot));
        std::string right = trim(std::string_view(expr).substr(dot + 1));
        if (ctx.currentStruct && (left == "this" || left.empty())) {
            if (const FieldInfo* field = findField(*ctx.currentStruct, right)) {
                return findFunctionInterface(field->typeName);
            }
        }
        if (ctx.localTypes) {
            auto varIt = ctx.localTypes->find(left);
            if (varIt != ctx.localTypes->end()) {
                const StructInfo* info = findStruct(varIt->second);
                if (info) {
                    if (const FieldInfo* field = findField(*info, right)) {
                        return findFunctionInterface(field->typeName);
                    }
                }
            }
        }
    } else if (ctx.currentStruct) {
        if (const FieldInfo* field = findField(*ctx.currentStruct, expr)) {
            return findFunctionInterface(field->typeName);
        }
        for (const auto& field : ctx.currentStruct->fields) {
            if (expr == field.generatedName || expr.rfind(field.generatedName + "[", 0) == 0) {
                return findFunctionInterface(field.typeName);
            }
        }
    }
    return nullptr;
}

std::string Phase1Codegen::rewriteReceiverExpression(const std::string& receiver, const LoweringContext& ctx) const {
    static const LocalTypeMap kEmptyLocalTypes;
    const auto& localTypes = ctx.localTypes ? *ctx.localTypes : kEmptyLocalTypes;
    return rewriteStructExpression(receiver, ctx.currentStruct, localTypes);
}

Phase1Codegen::LineTokenCache Phase1Codegen::buildLineTokenCache(const std::string& line) const {
    LineTokenCache cache;
    cache.line = line;
    cache.hasGeneratedStructPrefix = line.find("s__") != std::string::npos ||
                                     line.find("sc__") != std::string::npos ||
                                     line.find("si__") != std::string::npos;
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    for (size_t i = 0; i < line.size();) {
        if (!inString && !inRaw && i + 1 < line.size() && line[i] == '/' && line[i + 1] == '/') {
            break;
        }
        char c = line[i];
        if (inString) {
            cache.hasStringOrRawcode = true;
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
            cache.hasStringOrRawcode = true;
            if (c == '\'') {
                inRaw = false;
            }
            ++i;
            continue;
        }
        if (c == '"') {
            cache.hasStringOrRawcode = true;
            inString = true;
            ++i;
            continue;
        }
        if (c == '\'') {
            cache.hasStringOrRawcode = true;
            inRaw = true;
            ++i;
            continue;
        }
        if (c == '.') {
            size_t nameStart = i + 1;
            while (nameStart < line.size() && std::isspace(static_cast<unsigned char>(line[nameStart]))) {
                ++nameStart;
            }
            if (nameStart < line.size() && isIdentStart(line[nameStart])) {
                cache.hasDot = true;
            }
        } else if (c == '[') {
            cache.hasBracket = true;
            cache.bracketPositions.push_back(i);
        } else if (c == '(') {
            cache.hasParen = true;
            cache.parenPositions.push_back(i);
        } else if (c == '&' || c == '|' || c == '!') {
            cache.hasBooleanOperators = true;
        }
        if (!isIdentStart(c)) {
            ++i;
            continue;
        }
        size_t start = i++;
        while (i < line.size() && isIdentPart(line[i])) {
            ++i;
        }
        std::string_view word(line.data() + start, i - start);
        size_t prev = start;
        while (prev > 0 && std::isspace(static_cast<unsigned char>(line[prev - 1]))) {
            --prev;
        }
        bool precededByDot = prev > 0 && line[prev - 1] == '.';
        cache.identifiers.push_back({start, i, precededByDot});
        if (word == "call") {
            cache.hasCallWord = true;
        } else if (word == "set") {
            cache.hasSetWord = true;
        } else if (word == "local") {
            cache.hasLocalWord = true;
        } else if (word == "return") {
            cache.hasReturnWord = true;
        } else if (word == "function") {
            cache.hasFunctionWord = true;
            size_t next = i;
            while (next < line.size() && std::isspace(static_cast<unsigned char>(line[next]))) {
                ++next;
            }
            if (next < line.size() && line[next] == '(') {
                cache.hasLambdaStart = true;
            }
        } else if (word == "this") {
            cache.hasThisWord = true;
        } else if (word == "thistype") {
            cache.hasThistypeWord = true;
        }
        if (precededByDot) {
            if (word == "name") {
                cache.hasNameToken = true;
            } else if (word == "execute" || word == "evaluate") {
                cache.hasExecuteEvaluate = true;
            }
        }
    }
    return cache;
}

const Phase1Codegen::LineTokenCache& Phase1Codegen::getLineTokenCache(const std::string& line, bool* built) const {
    auto it = lineTokenCache_.find(line);
    if (it != lineTokenCache_.end()) {
        ++performanceCounters_.tokenCacheHits;
        if (built) {
            *built = false;
        }
        return it->second;
    }
    ++performanceCounters_.tokenCacheMisses;
    ++performanceCounters_.tokenCacheBuilds;
    ++performanceCounters_.tokenCacheLines;
    auto [inserted, _] = lineTokenCache_.emplace(line, buildLineTokenCache(line));
    if (built) {
        *built = true;
    }
    return inserted->second;
}

Phase1Codegen::LineFeatures Phase1Codegen::scanLineFeatures(
    const std::string& line,
    const StructInfo* currentStruct,
    const LocalTypeMap* localTypes,
    const ArrayShapeMap* localArrayShapes) const {
    if (lineFeatureCacheValid_ &&
        lineFeatureCacheStruct_ == currentStruct &&
        lineFeatureCacheLocalTypes_ == localTypes &&
        lineFeatureCacheLocalArrayShapes_ == localArrayShapes &&
        lineFeatureCacheLine_ == line) {
        return lineFeatureCacheValue_;
    }
    bool builtTokenCache = false;
    const LineTokenCache& tokens = getLineTokenCache(line, &builtTokenCache);
    if (builtTokenCache) {
        ++performanceCounters_.lineFeatureScans;
    } else {
        ++performanceCounters_.featureScansAvoided;
    }
    LineFeatures features;
    features.hasDot = tokens.hasDot;
    features.hasBracket = tokens.hasBracket;
    features.hasParen = tokens.hasParen;
    features.hasCall = tokens.hasCallWord;
    features.hasSet = tokens.hasSetWord;
    features.hasLocal = tokens.hasLocalWord;
    features.hasReturn = tokens.hasReturnWord;
    features.hasFunctionKeyword = tokens.hasFunctionWord;
    features.hasLambdaStart = tokens.hasLambdaStart;
    features.hasThis = tokens.hasThisWord;
    features.hasThistype = tokens.hasThistypeWord;
    features.hasNameToken = tokens.hasNameToken;
    features.hasExecuteEvaluate = tokens.hasExecuteEvaluate;
    features.hasBooleanOperators = tokens.hasBooleanOperators;
    features.hasGeneratedStructPrefix = tokens.hasGeneratedStructPrefix;
    features.hasStringOrRawcode = tokens.hasStringOrRawcode;
    if (tokens.hasBracket) {
        for (const auto& token : tokens.identifiers) {
            size_t cursor = token.end;
            while (cursor < line.size() && std::isspace(static_cast<unsigned char>(line[cursor]))) {
                ++cursor;
            }
            if (cursor >= line.size() || line[cursor] != '[') {
                continue;
            }
            std::string_view word(tokens.line.data() + token.start, token.end - token.start);
            unsigned char first = static_cast<unsigned char>(word.front());
            if ((globalArrayShapeFirstChars_[first] && globalArrayShapes_.contains(word)) ||
                (localArrayShapes && !localArrayShapes->empty() && localArrayShapes->contains(word))) {
                features.hasKnownArrayReceiver = true;
                break;
            }
            if (auto structIt = structIndexByName_.find(word);
                structIt != structIndexByName_.end() && structs_[structIt->second].isArrayStruct) {
                features.hasKnownArrayReceiver = true;
                break;
            }
        }
    }
    auto addCurrentStructMember = [&](std::string_view name) {
        if (!currentStruct || name.empty()) {
            return false;
        }
        unsigned char first = static_cast<unsigned char>(name.front());
        if (!currentStruct->fieldFirstChars[first] && !currentStruct->methodFirstChars[first]) {
            return false;
        }
        bool matched = false;
        if (auto fieldIt = currentStruct->fieldIndex.find(name); fieldIt != currentStruct->fieldIndex.end()) {
            const FieldInfo* field = &currentStruct->fields[fieldIt->second];
            if (std::find(features.currentStructFields.begin(), features.currentStructFields.end(), field) ==
                features.currentStructFields.end()) {
                features.currentStructFields.push_back(field);
            }
            matched = true;
        }
        if (auto methodIt = currentStruct->methodIndex.find(name); methodIt != currentStruct->methodIndex.end()) {
            const MethodInfo* method = &currentStruct->methods[methodIt->second];
            if (std::find(features.currentStructMethods.begin(), features.currentStructMethods.end(), method) ==
                features.currentStructMethods.end()) {
                features.currentStructMethods.push_back(method);
            }
            matched = true;
        }
        return matched;
    };
    auto dottedTokenMayReferenceCurrentStruct = [&](const IdentifierToken& token) {
        size_t dot = token.start;
        while (dot > 0 && std::isspace(static_cast<unsigned char>(line[dot - 1]))) {
            --dot;
        }
        if (dot == 0 || line[dot - 1] != '.') {
            return false;
        }
        size_t beforeDot = dot - 1;
        while (beforeDot > 0 && std::isspace(static_cast<unsigned char>(line[beforeDot - 1]))) {
            --beforeDot;
        }
        if (beforeDot == 0) {
            return true;
        }
        char prev = line[beforeDot - 1];
        if (prev == ']') {
            return true;
        }
        if (!isIdentPart(prev) && prev != ')') {
            return true;
        }
        if (!isIdentPart(prev)) {
            return false;
        }
        size_t receiverStart = beforeDot - 1;
        while (receiverStart > 0 && isIdentPart(line[receiverStart - 1])) {
            --receiverStart;
        }
        std::string_view receiver(line.data() + receiverStart, beforeDot - receiverStart);
        return receiver == "this" || receiver == "thistype";
    };
    auto dottedTokenHasKnownStructReceiver = [&](const IdentifierToken& token) {
        auto receiverNameHasStructType = [&](std::string_view receiver) {
            if (receiver == "return" || receiver == "set" || receiver == "call" || receiver == "function") {
                return currentStruct != nullptr;
            }
            if (receiver == "this" || receiver == "thistype") {
                return currentStruct != nullptr;
            }
            if (localTypes) {
                if (auto localIt = localTypes->find(receiver); localIt != localTypes->end()) {
                    return localIt->second == "thistype" ||
                           structIndexByName_.find(localIt->second) != structIndexByName_.end();
                }
            }
            if (auto globalIt = globalStructTypes_.find(receiver); globalIt != globalStructTypes_.end()) {
                return structIndexByName_.find(globalIt->second) != structIndexByName_.end();
            }
            return structIndexByName_.find(receiver) != structIndexByName_.end() ||
                   functionInterfaceIndexByName_.find(receiver) != functionInterfaceIndexByName_.end();
        };
        auto indexedReceiverHasStructType = [&](size_t closeBracket) {
            size_t cursor = closeBracket + 1;
            while (cursor > 0) {
                size_t close = cursor - 1;
                if (line[close] != ']') {
                    break;
                }
                int depth = 1;
                size_t open = close;
                bool foundOpen = false;
                while (open > 0) {
                    --open;
                    if (line[open] == ']') {
                        ++depth;
                    } else if (line[open] == '[') {
                        --depth;
                        if (depth == 0) {
                            foundOpen = true;
                            break;
                        }
                    }
                }
                if (!foundOpen) {
                    return true;
                }
                cursor = open;
                while (cursor > 0 && std::isspace(static_cast<unsigned char>(line[cursor - 1]))) {
                    --cursor;
                }
            }
            if (cursor == 0 || !isIdentPart(line[cursor - 1])) {
                return true;
            }
            size_t receiverEnd = cursor;
            size_t receiverStart = receiverEnd - 1;
            while (receiverStart > 0 && isIdentPart(line[receiverStart - 1])) {
                --receiverStart;
            }
            return receiverNameHasStructType(std::string_view(line.data() + receiverStart, receiverEnd - receiverStart));
        };
        size_t dot = token.start;
        while (dot > 0 && std::isspace(static_cast<unsigned char>(line[dot - 1]))) {
            --dot;
        }
        if (dot == 0 || line[dot - 1] != '.') {
            return false;
        }
        size_t beforeDot = dot - 1;
        while (beforeDot > 0 && std::isspace(static_cast<unsigned char>(line[beforeDot - 1]))) {
            --beforeDot;
        }
        if (beforeDot == 0) {
            return currentStruct != nullptr;
        }
        char prev = line[beforeDot - 1];
        if (prev == ']') {
            return indexedReceiverHasStructType(beforeDot - 1);
        }
        if (prev == ')') {
            return true;
        }
        if (!isIdentPart(prev)) {
            return currentStruct != nullptr;
        }
        size_t receiverStart = beforeDot - 1;
        while (receiverStart > 0 && isIdentPart(line[receiverStart - 1])) {
            --receiverStart;
        }
        std::string_view receiver(line.data() + receiverStart, beforeDot - receiverStart);
        return receiverNameHasStructType(receiver);
    };
    if (features.hasThistype && currentStruct) {
        features.hasPossibleStructMember = true;
    }
    if (tokens.hasDot) {
        for (const auto& token : tokens.identifiers) {
            if (token.precededByDot && dottedTokenHasKnownStructReceiver(token)) {
                features.hasPotentialStructDot = true;
                break;
            }
        }
    }
    if (currentStruct) {
        for (const auto& token : tokens.identifiers) {
            std::string_view word(tokens.line.data() + token.start, token.end - token.start);
            if (token.precededByDot) {
                if (dottedTokenMayReferenceCurrentStruct(token) && addCurrentStructMember(word)) {
                    features.hasPossibleStructMember = true;
                }
                continue;
            }
            if (word == "allocate" || word == "destroy" || word == "deallocate") {
                features.hasPossibleStructMember = true;
                continue;
            }
            unsigned char first = static_cast<unsigned char>(word.front());
            if (!currentStruct->fieldFirstChars[first] && !currentStruct->methodFirstChars[first]) {
                continue;
            }
            if (localTypes && localTypes->find(word) != localTypes->end()) {
                ++performanceCounters_.methodPlanShadowSkips;
                continue;
            }
            if (addCurrentStructMember(word)) {
                features.hasPossibleStructMember = true;
            }
        }
        if (!features.hasPossibleStructMember) {
            ++performanceCounters_.methodPlanLinesSkippedNoCandidate;
        }
    }
    lineFeatureCacheValid_ = true;
    lineFeatureCacheLine_ = line;
    lineFeatureCacheStruct_ = currentStruct;
    lineFeatureCacheLocalTypes_ = localTypes;
    lineFeatureCacheLocalArrayShapes_ = localArrayShapes;
    lineFeatureCacheValue_ = features;
    return features;
}

bool Phase1Codegen::lineNeedsStructRewrite(const LineFeatures& features, const StructInfo* currentStruct) const {
    return features.hasPotentialStructDot ||
           features.hasKnownArrayReceiver ||
           features.hasThis ||
           features.hasThistype ||
           features.hasGeneratedStructPrefix ||
           (currentStruct != nullptr && features.hasPossibleStructMember);
}

bool Phase1Codegen::lineNeedsExpressionLowering(const LineFeatures& features,
                                                const std::string& expectedInterfaceType,
                                                const LoweringContext& ctx) const {
    if (!expectedInterfaceType.empty()) {
        return true;
    }
    if (features.hasFunctionKeyword || features.hasLambdaStart || features.hasNameToken ||
        features.hasExecuteEvaluate || features.hasBooleanOperators) {
        return true;
    }
    if (features.hasParen) {
        return true;
    }
    return lineNeedsStructRewrite(features, ctx.currentStruct);
}

std::string Phase1Codegen::inferUniqueFunctionInterfaceTypeForFunctionValue(const std::string& expression,
                                                                            const LoweringContext& ctx) const {
    std::string targetExpr = trim(expression);
    if (!startsWithWord(targetExpr, "function")) {
        return {};
    }
    targetExpr = trim(std::string_view(targetExpr).substr(8));
    std::string finalTarget = resolveFunctionTargetName(targetExpr, ctx.currentStruct);
    const FunctionInfo* fn = findFunctionInfo(finalTarget);
    if (!fn) {
        return {};
    }
    const FunctionInterfaceInfo* match = nullptr;
    for (const auto& iface : functionInterfaces_) {
        if (!sameSignature(fn->signature, iface.signature)) {
            continue;
        }
        if (match != nullptr) {
            return {};
        }
        match = &iface;
    }
    return match ? match->finalName : std::string{};
}

std::string Phase1Codegen::lowerFunctionValue(std::string expression,
                                              const std::string& expectedInterfaceType,
                                              LoweringContext& ctx,
                                              std::vector<std::string>& prelude) const {
    expression = trim(expression);
    static const LocalTypeMap kEmptyLocalTypes;
    const auto& localTypes = ctx.localTypes ? *ctx.localTypes : kEmptyLocalTypes;
    if (expression.empty()) {
        return expression;
    }
    bool functionKeyword = false;
    if (startsWithWord(expression, "function")) {
        functionKeyword = true;
        expression = trim(std::string_view(expression).substr(8));
    }
    if (!expectedInterfaceType.empty()) {
    if (const FunctionInterfaceInfo* expected = findFunctionInterface(expectedInterfaceType)) {
        if (!expression.empty() && (std::isdigit(static_cast<unsigned char>(expression[0])) || expression[0] == '-')) {
            return expression;
        }
        std::string targetExpr = expression;
        size_t dot = expression.find('.');
        if (dot != std::string::npos) {
            std::string maybeIface = trim(std::string_view(expression).substr(0, dot));
            if (findFunctionInterface(maybeIface)) {
                targetExpr = trim(std::string_view(expression).substr(dot + 1));
            }
        }
        std::string finalTarget = resolveFunctionTargetName(targetExpr, ctx.currentStruct);
        if (findFunctionInfo(finalTarget)) {
            if (finalTarget.rfind("vjlambda__", 0) == 0) {
                ++lambdasFunctionInterfaceContext_;
            }
            int id = registerInterfaceTarget(*expected, finalTarget, SourceLocation{});
            return std::to_string(id);
        }
        if (functionKeyword) {
            diagnostics_.error(SourceLocation{}, "unknown function target '" + targetExpr + "'");
            return "0";
        }
        return rewriteCallArguments(rewriteFunctionNames(rewriteStructExpression(expression,
                                                                                ctx.currentStruct,
                                                                                localTypes),
                                                        ctx.currentStruct),
                                    ctx,
                                    prelude);
    }
    }
    size_t dot = expression.find('.');
    if (dot != std::string::npos) {
        std::string maybeIface = trim(std::string_view(expression).substr(0, dot));
        if (const FunctionInterfaceInfo* iface = findFunctionInterface(maybeIface)) {
            std::string targetExpr = trim(std::string_view(expression).substr(dot + 1));
            int id = registerInterfaceTarget(*iface, resolveFunctionTargetName(targetExpr, ctx.currentStruct), SourceLocation{});
            return std::to_string(id);
        }
    }
    if (functionKeyword) {
        return "function " + resolveFunctionTargetName(expression, ctx.currentStruct);
    }
    auto rewriteExplicitInterfaceReferences = [&](std::string text) {
        auto rewriteNormal = [&](std::string fragment) {
            for (size_t pos = 0; pos < fragment.size();) {
                if (!isIdentStart(fragment[pos])) {
                    ++pos;
                    continue;
                }
                size_t leftStart = pos++;
                while (pos < fragment.size() && isIdentPart(fragment[pos])) {
                    ++pos;
                }
                if (pos >= fragment.size() || fragment[pos] != '.') {
                    continue;
                }
                std::string ifaceName = fragment.substr(leftStart, pos - leftStart);
                const FunctionInterfaceInfo* iface = findFunctionInterface(ifaceName);
                if (!iface) {
                    continue;
                }
                ++pos;
                if (pos >= fragment.size() || !isIdentStart(fragment[pos])) {
                    continue;
                }
                size_t targetStart = pos++;
                while (pos < fragment.size() && isIdentPart(fragment[pos])) {
                    ++pos;
                }
                std::string targetExpr = fragment.substr(targetStart, pos - targetStart);
                std::string finalTarget = resolveFunctionTargetName(targetExpr, ctx.currentStruct);
                if (!findFunctionInfo(finalTarget)) {
                    continue;
                }
                int id = registerInterfaceTarget(*iface, finalTarget, SourceLocation{});
                std::string replacement = std::to_string(id);
                fragment.replace(leftStart, pos - leftStart, replacement);
                pos = leftStart + replacement.size();
            }
            return fragment;
        };
        return rewriteOutsideProtected(text, rewriteNormal);
    };
    return rewriteExplicitInterfaceReferences(std::move(expression));
}

std::string Phase1Codegen::rewriteFunctionNames(std::string expression, const StructInfo* currentStruct) const {
    if (expression.find(".name") == std::string::npos) {
        return expression;
    }
    auto rewriteNormal = [&](std::string text) {
        size_t dot = 0;
        while ((dot = text.find(".name", dot)) != std::string::npos) {
            size_t left = dot;
            while (left > 0 && (std::isalnum(static_cast<unsigned char>(text[left - 1])) ||
                                text[left - 1] == '_' || text[left - 1] == '$' || text[left - 1] == '.')) {
                --left;
            }
            std::string receiver = text.substr(left, dot - left);
            std::string finalName = resolveFunctionTargetName(receiver, currentStruct);
            if (findFunctionInfo(finalName)) {
                text.replace(left, dot + 5 - left, "\"" + finalName + "\"");
                dot = left + finalName.size() + 2;
            } else {
                dot += 5;
            }
        }
        return text;
    };
    return rewriteOutsideProtected(expression, rewriteNormal);
}

std::string Phase1Codegen::rewriteCallArguments(std::string expression, LoweringContext& ctx, std::vector<std::string>& prelude) const {
    auto rewriteNormal = [&](std::string text) {
        const bool hasFunctionKeyword = text.find("function") != std::string::npos;
        for (size_t pos = 0; pos < text.size();) {
            if (!isIdentStart(text[pos])) {
                ++pos;
                continue;
            }
            size_t nameStart = pos++;
            while (pos < text.size() && isIdentPart(text[pos])) {
                ++pos;
            }
            std::string name = text.substr(nameStart, pos - nameStart);
            size_t p = pos;
            while (p < text.size() && std::isspace(static_cast<unsigned char>(text[p]))) {
                ++p;
            }
            if (p >= text.size() || text[p] != '(') {
                continue;
            }
            if (functionArgumentLoweringCandidates_.find(name) == functionArgumentLoweringCandidates_.end() &&
                !hasFunctionKeyword) {
                if (functionIndexByName_.find(name) == functionIndexByName_.end()) {
                    pos = p + 1;
                } else {
                    size_t close = findMatchingParen(text, p);
                    pos = close == std::string::npos ? p + 1 : close + 1;
                }
                continue;
            }
            const FunctionInfo* fn = findFunctionInfo(name);
            if (!fn || fn->signature.paramTypes.empty()) {
                pos = p + 1;
                continue;
            }
            size_t close = findMatchingParen(text, p);
            if (close == std::string::npos) {
                break;
            }
            std::vector<std::string> args = splitCommaList(text.substr(p + 1, close - p - 1));
            if (args.size() != fn->signature.paramTypes.size()) {
                pos = close + 1;
                continue;
            }
            bool changed = false;
            for (size_t i = 0; i < args.size(); ++i) {
                std::string expectedType;
                auto interfaceParamIt = functionInterfaceParamTypesByFunction_.find(fn->finalName);
                if (interfaceParamIt == functionInterfaceParamTypesByFunction_.end()) {
                    interfaceParamIt = functionInterfaceParamTypesByFunction_.find(fn->sourceName);
                }
                if (interfaceParamIt != functionInterfaceParamTypesByFunction_.end() &&
                    i < interfaceParamIt->second.size() && !interfaceParamIt->second[i].empty()) {
                    expectedType = interfaceParamIt->second[i];
                } else if (findFunctionInterface(fn->signature.paramTypes[i])) {
                    expectedType = fn->signature.paramTypes[i];
                } else if (fn->signature.paramTypes[i] == "integer" && args[i].find("function") != std::string::npos) {
                    expectedType = inferUniqueFunctionInterfaceTypeForFunctionValue(args[i], ctx);
                }
                bool hasNestedRewriteCandidate = !expectedType.empty() ||
                                                 args[i].find("function") != std::string::npos;
                if (hasNestedRewriteCandidate) {
                    std::string originalArg = args[i];
                    args[i] = lowerExpression(args[i], expectedType, ctx, prelude);
                    if (!expectedType.empty() && originalArg != args[i]) {
                        if (const FunctionInterfaceInfo* iface = findFunctionInterface(expectedType)) {
                            if (auto literalId = parsePositiveIntegerLiteral(args[i])) {
                                markInterfaceTargetNeedsCondition(*iface, std::to_string(*literalId), SourceLocation{});
                            }
                        }
                    }
                    changed = changed || args[i] != originalArg;
                }
                if (!expectedType.empty()) {
                    changed = true;
                }
            }
            if (changed) {
                std::string replacement = name + "(" + joinArgs(args) + ")";
                text.replace(nameStart, close + 1 - nameStart, replacement);
                pos = nameStart + replacement.size();
            } else {
                pos = close + 1;
            }
        }
        return text;
    };
    return rewriteOutsideProtected(expression, rewriteNormal);
}

std::string removeFirstTopLevelCloseBracket(std::string text) {
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    int bracketDepth = 0;
    for (size_t i = 0; i < text.size(); ++i) {
        char c = text[i];
        if (inString) {
            if (escaped) {
                escaped = false;
            } else if (c == '\\') {
                escaped = true;
            } else if (c == '"') {
                inString = false;
            }
            continue;
        }
        if (inRaw) {
            if (c == '\'') {
                inRaw = false;
            }
            continue;
        }
        if (c == '"') {
            inString = true;
        } else if (c == '\'') {
            inRaw = true;
        } else if (c == '[') {
            ++bracketDepth;
        } else if (c == ']') {
            if (bracketDepth == 0) {
                text.erase(i, 1);
                return text;
            }
            --bracketDepth;
        }
    }
    return text;
}

std::string normalizeDiscardedFieldCallStatement(const std::string& line) {
    std::string t = trim(line);
    if (!startsWithWord(t, "call")) {
        return line;
    }
    size_t pos = 4;
    while (pos < t.size() && std::isspace(static_cast<unsigned char>(t[pos]))) {
        ++pos;
    }
    if (pos >= t.size() || !isIdentStart(t[pos])) {
        return line;
    }
    ++pos;
    while (pos < t.size() && isIdentPart(t[pos])) {
        ++pos;
    }
    while (pos < t.size() && std::isspace(static_cast<unsigned char>(t[pos]))) {
        ++pos;
    }
    if (pos >= t.size() || t[pos] != '[') {
        return line;
    }
    ++pos;
    while (pos < t.size() && std::isspace(static_cast<unsigned char>(t[pos]))) {
        ++pos;
    }
    if (pos >= t.size() || !isIdentStart(t[pos])) {
        return line;
    }
    size_t functionStart = pos++;
    while (pos < t.size() && isIdentPart(t[pos])) {
        ++pos;
    }
    std::string functionName = t.substr(functionStart, pos - functionStart);
    while (pos < t.size() && std::isspace(static_cast<unsigned char>(t[pos]))) {
        ++pos;
    }
    if (pos >= t.size() || t[pos] != '(') {
        return line;
    }
    size_t close = findMatchingParen(t, pos);
    if (close == std::string::npos) {
        return line;
    }
    std::string suffix = trim(std::string_view(t).substr(close + 1));
    if (!suffix.empty() && suffix != "]" && suffix != "];") {
        return line;
    }
    std::string args = removeFirstTopLevelCloseBracket(t.substr(pos + 1, close - pos - 1));
    return "call " + functionName + "(" + args + ")";
}

std::string Phase1Codegen::lowerExpression(std::string expression,
                                           const std::string& expectedInterfaceType,
                                           LoweringContext& ctx,
                                           std::vector<std::string>& prelude) const {
    expression = trim(expression);
    static const LocalTypeMap kEmptyLocalTypes;
    const auto& localTypes = ctx.localTypes ? *ctx.localTypes : kEmptyLocalTypes;
    if (expectedInterfaceType.empty() && ctx.currentStruct == nullptr &&
        expression.find_first_of(".[(") == std::string::npos &&
        !containsIdentifierWord(expression, "function")) {
        ++performanceCounters_.linesSkippedNoDotBracketCall;
        ++performanceCounters_.linesSkippedNoCurrentStruct;
        return expression;
    }
    if (expectedInterfaceType.empty() && ctx.currentStruct != nullptr &&
        expression.find_first_of(".[(") == std::string::npos &&
        !containsIdentifierWord(expression, "function")) {
        LineFeatures features = scanLineFeatures(expression, ctx.currentStruct, ctx.localTypes, ctx.localArrayShapes);
        if (!lineNeedsStructRewrite(features, ctx.currentStruct)) {
            ++performanceCounters_.linesSkippedNoDotBracketCall;
            return expression;
        }
    }
    expression = lowerFunctionValue(expression, expectedInterfaceType, ctx, prelude);
    while (true) {
        size_t receiverStart = 0;
        size_t dotPos = 0;
        size_t open = 0;
        size_t close = 0;
        if (!findMethodCallSpan(expression, "evaluate", receiverStart, dotPos, open, close)) {
            break;
        }
        std::string receiver = trim(expression.substr(receiverStart, dotPos - receiverStart));
        std::string argsText = expression.substr(open + 1, close - open - 1);
        if (const FunctionInterfaceInfo* iface = resolveReceiverInterface(receiver, ctx)) {
            if (iface->signature.returnType == "nothing") {
                diagnostics_.error(SourceLocation{}, "evaluate used on non-returning function interface '" + iface->sourceName + "'");
                break;
            }
            std::vector<std::string> args = splitCommaList(argsText);
            if (args.size() != iface->signature.paramTypes.size()) {
                diagnostics_.error(SourceLocation{}, "wrong argument count for evaluate on function interface '" + iface->sourceName + "'");
            }
            std::string prefix = interfaceGlobalPrefix(*iface);
            for (size_t i = 0; i < args.size() && i < iface->signature.paramTypes.size(); ++i) {
                std::vector<std::string> nested;
                std::string arg = lowerExpression(args[i], {}, ctx, nested);
                prelude.insert(prelude.end(), nested.begin(), nested.end());
                prelude.push_back("set " + prefix + "_arg" + std::to_string(i) + "=" + arg);
            }
            ++functionInterfaceCalls_;
            std::string receiverExpr = rewriteReceiverExpression(receiver, ctx);
            markInterfaceTargetNeedsCondition(*iface, receiverExpr, SourceLocation{});
            prelude.push_back("call TriggerEvaluate(" + prefix + "_trigger[" + receiverExpr + "])");
            std::string replacement = prefix + "_result";
            if (receiverStart == 0 && close + 1 == expression.size()) {
                expression = replacement;
            } else {
                size_t depth = static_cast<size_t>(++ctx.tempCounter);
                functionInterfaceMaxEvaluateDepth_ = std::max(functionInterfaceMaxEvaluateDepth_, depth);
                if (depth > functionInterfaceEvaluateTempLimit_) {
                    diagnostics_.error(SourceLocation{}, "nested function interface evaluate exceeds temp limit " +
                                                          std::to_string(functionInterfaceEvaluateTempLimit_));
                    depth = functionInterfaceEvaluateTempLimit_;
                }
                std::string temp = prefix + "_tmp" + std::to_string(depth);
                prelude.push_back("set " + temp + "=" + replacement);
                expression.replace(receiverStart, close + 1 - receiverStart, temp);
            }
            continue;
        }
        std::string finalName = resolveFunctionTargetName(receiver, ctx.currentStruct);
        if (const FunctionInfo* fn = findFunctionInfo(finalName)) {
            ++functionObjectCalls_;
            if (const FunctionInterfaceInfo* iface = findFunctionObjectInterface(fn->signature)) {
                if (iface->signature.returnType == "nothing") {
                    diagnostics_.error(SourceLocation{}, "evaluate used as expression on non-returning function '" + fn->sourceName + "'");
                    break;
                }
                std::vector<std::string> args = splitCommaList(argsText);
                if (args.size() != iface->signature.paramTypes.size()) {
                    diagnostics_.error(SourceLocation{}, "wrong argument count for evaluate on function '" + fn->sourceName + "'");
                }
                std::string prefix = interfaceGlobalPrefix(*iface);
                for (size_t i = 0; i < args.size() && i < iface->signature.paramTypes.size(); ++i) {
                    std::vector<std::string> nested;
                    std::string arg = lowerExpression(args[i], {}, ctx, nested);
                    prelude.insert(prelude.end(), nested.begin(), nested.end());
                    prelude.push_back("set " + prefix + "_arg" + std::to_string(i) + "=" + arg);
                }
                int targetId = registerInterfaceTarget(*iface, fn->finalName, SourceLocation{});
                markInterfaceTargetNeedsCondition(*iface, std::to_string(targetId), SourceLocation{});
                ++functionInterfaceCalls_;
                prelude.push_back("call TriggerEvaluate(" + prefix + "_trigger[" + std::to_string(targetId) + "])");
                std::string replacement = prefix + "_result";
                if (receiverStart == 0 && close + 1 == expression.size()) {
                    expression = replacement;
                } else {
                    size_t depth = static_cast<size_t>(++ctx.tempCounter);
                    functionInterfaceMaxEvaluateDepth_ = std::max(functionInterfaceMaxEvaluateDepth_, depth);
                    if (depth > functionInterfaceEvaluateTempLimit_) {
                        diagnostics_.error(SourceLocation{}, "nested function object evaluate exceeds temp limit " +
                                                              std::to_string(functionInterfaceEvaluateTempLimit_));
                        depth = functionInterfaceEvaluateTempLimit_;
                    }
                    std::string temp = prefix + "_tmp" + std::to_string(depth);
                    prelude.push_back("set " + temp + "=" + replacement);
                    expression.replace(receiverStart, close + 1 - receiverStart, temp);
                }
                continue;
            }
            std::string replacement = fn->finalName + "(" + argsText + ")";
            expression.replace(receiverStart, close + 1 - receiverStart, replacement);
            continue;
        }
        break;
    }
    expression = rewriteFunctionNames(expression, ctx.currentStruct);
    std::string beforeStructExpression = expression;
    expression = rewriteStructExpression(expression, ctx.currentStruct, localTypes);
    expression = rewriteArrayAccesses(expression, ctx.localArrayShapes);
    if (expression != beforeStructExpression && expression.find('.') != std::string::npos) {
        expression = rewriteStructExpression(expression,
                                             ctx.currentStruct,
                                             localTypes);
        expression = rewriteArrayAccesses(expression, ctx.localArrayShapes);
    }
    std::string beforeCallArguments = expression;
    expression = rewriteCallArguments(expression, ctx, prelude);
    if (expression != beforeCallArguments && expression.find('.') != std::string::npos) {
        expression = rewriteStructExpression(expression,
                                             ctx.currentStruct,
                                             localTypes);
        expression = rewriteArrayAccesses(expression, ctx.localArrayShapes);
    }
    return trim(expression);
}

void Phase1Codegen::lowerStatementLineInto(const std::string& rawLine,
                                           LoweringContext& ctx,
                                           std::vector<std::string>& out) const {
    ++performanceCounters_.linesVisited;
    std::string line = trim(rawLine);
    if (ctx.currentStruct) {
        ++performanceCounters_.structMethodLinesLowered;
    }
    std::string originalTrim = line;
    std::string leading;
    size_t firstNonSpace = rawLine.find_first_not_of(" \t");
    if (firstNonSpace != std::string::npos) {
        leading = rawLine.substr(0, firstNonSpace);
    }
    if (line.empty()) {
        return;
    }
    LineFeatures statementFeatures = scanLineFeatures(line, ctx.currentStruct, ctx.localTypes, ctx.localArrayShapes);
    auto countFastPath = [&]() {
        if (ctx.mode == BodyMode::Zinc) {
            ++performanceCounters_.zincFastPathLines;
        } else if (ctx.mode == BodyMode::Generated) {
            ++performanceCounters_.generatedFastPathLines;
        } else {
            ++performanceCounters_.jassLikeFastPathLines;
        }
        ++performanceCounters_.heavyLoweringAvoidedByMode;
    };
    auto directCallName = [](const std::string& text) {
        std::string t = trim(text);
        if (!startsWithWord(t, "call")) {
            return std::string{};
        }
        size_t pos = 4;
        while (pos < t.size() && std::isspace(static_cast<unsigned char>(t[pos]))) {
            ++pos;
        }
        if (pos >= t.size() || !isIdentStart(t[pos])) {
            return std::string{};
        }
        size_t start = pos++;
        while (pos < t.size() && isIdentPart(t[pos])) {
            ++pos;
        }
        return t.substr(start, pos - start);
    };
    auto isSimpleIdentifierText = [](std::string_view text) {
        text = trimView(text);
        if (text.empty() || !isIdentStart(text.front())) {
            return false;
        }
        for (char c : text.substr(1)) {
            if (!isIdentPart(c)) {
                return false;
            }
        }
        return true;
    };
    auto containsFunctionArgumentCandidateCall = [&]() {
        if (!statementFeatures.hasParen || functionArgumentLoweringCandidates_.empty()) {
            return false;
        }
        const LineTokenCache& tokens = getLineTokenCache(line);
        for (const auto& token : tokens.identifiers) {
            size_t cursor = token.end;
            while (cursor < line.size() && std::isspace(static_cast<unsigned char>(line[cursor]))) {
                ++cursor;
            }
            std::string_view word(tokens.line.data() + token.start, token.end - token.start);
            if (cursor < line.size() && line[cursor] == '(' &&
                functionArgumentLoweringCandidates_.contains(word)) {
                return true;
            }
        }
        return false;
    };
    const bool hasModeHeavyToken =
        statementFeatures.hasPotentialStructDot ||
        statementFeatures.hasKnownArrayReceiver ||
        statementFeatures.hasFunctionKeyword ||
        statementFeatures.hasLambdaStart ||
        statementFeatures.hasNameToken ||
        statementFeatures.hasExecuteEvaluate ||
        statementFeatures.hasBooleanOperators ||
        statementFeatures.hasThis ||
        statementFeatures.hasThistype ||
        statementFeatures.hasGeneratedStructPrefix ||
        (ctx.currentStruct != nullptr && statementFeatures.hasPossibleStructMember);
    if (!hasModeHeavyToken) {
        bool canUseFastPath = true;
        if (startsWithWord(line, "call")) {
            std::string callName = directCallName(line);
            if (callName.empty() || functionArgumentLoweringCandidates_.contains(callName)) {
                canUseFastPath = false;
            } else {
                recordFunctionDependency(ctx.currentFunctionName, callName);
            }
        } else if (startsWithWord(line, "set")) {
            std::string rest = trim(std::string_view(line).substr(3));
            if (containsFunctionArgumentCandidateCall()) {
                canUseFastPath = false;
            }
            if (auto eq = findTopLevelAssignment(rest)) {
                std::string lhs = trim(std::string_view(rest).substr(0, *eq));
                std::string rhs = trim(std::string_view(rest).substr(*eq + 1));
                if (ctx.localTypes && isSimpleIdentifierText(lhs)) {
                    auto it = ctx.localTypes->find(lhs);
                    if (it != ctx.localTypes->end() && findFunctionInterface(it->second)) {
                        canUseFastPath = false;
                    }
                }
                if (isSimpleIdentifierText(rhs) && functionIndexByName_.contains(rhs)) {
                    canUseFastPath = false;
                }
            }
        } else if (containsFunctionArgumentCandidateCall()) {
            canUseFastPath = false;
        }
        if (canUseFastPath) {
            countFastPath();
            out.push_back(!leading.empty() ? rawLine : line);
            return;
        }
    }
    if (startsWithWord(line, "call")) {
        std::string callExpr = trim(std::string_view(line).substr(4));
        size_t receiverStart = 0;
        size_t dotPos = 0;
        size_t open = 0;
        size_t close = 0;
        auto lowerCallArgs = [&](const std::string& argsText) {
            std::vector<std::string> loweredArgs;
            for (const auto& argText : splitCommaList(argsText)) {
                std::vector<std::string> prelude;
                std::string arg = lowerExpression(argText, {}, ctx, prelude);
                out.insert(out.end(), prelude.begin(), prelude.end());
                loweredArgs.push_back(arg);
            }
            return joinArgs(loweredArgs);
        };
        if (findMethodCallSpan(callExpr, "execute", receiverStart, dotPos, open, close) && receiverStart == 0 && close + 1 == callExpr.size()) {
            std::string receiver = trim(callExpr.substr(receiverStart, dotPos - receiverStart));
            std::string argsText = callExpr.substr(open + 1, close - open - 1);
            if (const FunctionInterfaceInfo* iface = resolveReceiverInterface(receiver, ctx)) {
                if (iface->signature.returnType != "nothing") {
                    diagnostics_.error(SourceLocation{}, "execute used on returning function interface '" + iface->sourceName + "'");
                }
                std::vector<std::string> args = splitCommaList(argsText);
                if (args.size() != iface->signature.paramTypes.size()) {
                    diagnostics_.error(SourceLocation{}, "wrong argument count for execute on function interface '" + iface->sourceName + "'");
                }
                std::string prefix = interfaceGlobalPrefix(*iface);
                for (size_t i = 0; i < args.size() && i < iface->signature.paramTypes.size(); ++i) {
                    std::vector<std::string> prelude;
                    std::string arg = lowerExpression(args[i], {}, ctx, prelude);
                    out.insert(out.end(), prelude.begin(), prelude.end());
                    out.push_back("set " + prefix + "_arg" + std::to_string(i) + "=" + arg);
                }
                ++functionInterfaceCalls_;
                std::string receiverExpr = rewriteReceiverExpression(receiver, ctx);
                markInterfaceTargetNeedsAction(*iface, receiverExpr, SourceLocation{});
                out.push_back("call TriggerExecute(" + prefix + "_trigger[" + receiverExpr + "])");
                return;
            }
            std::string finalName = resolveFunctionTargetName(receiver, ctx.currentStruct);
            if (const FunctionInfo* fn = findFunctionInfo(finalName)) {
                ++functionObjectCalls_;
                recordFunctionDependency(ctx.currentFunctionName, fn->finalName);
                out.push_back("call " + fn->finalName + "(" + lowerCallArgs(argsText) + ") // " +
                              std::string(kFunctionObjectExecuteMarker));
                return;
            }
        }
        if (findMethodCallSpan(callExpr, "evaluate", receiverStart, dotPos, open, close) && receiverStart == 0 && close + 1 == callExpr.size()) {
            std::string receiver = trim(callExpr.substr(receiverStart, dotPos - receiverStart));
            std::string argsText = callExpr.substr(open + 1, close - open - 1);
            if (const FunctionInterfaceInfo* iface = resolveReceiverInterface(receiver, ctx)) {
                std::vector<std::string> args = splitCommaList(argsText);
                if (args.size() != iface->signature.paramTypes.size()) {
                    diagnostics_.error(SourceLocation{}, "wrong argument count for evaluate on function interface '" + iface->sourceName + "'");
                }
                std::string prefix = interfaceGlobalPrefix(*iface);
                for (size_t i = 0; i < args.size() && i < iface->signature.paramTypes.size(); ++i) {
                    std::vector<std::string> prelude;
                    std::string arg = lowerExpression(args[i], {}, ctx, prelude);
                    out.insert(out.end(), prelude.begin(), prelude.end());
                    out.push_back("set " + prefix + "_arg" + std::to_string(i) + "=" + arg);
                }
                ++functionInterfaceCalls_;
                std::string receiverExpr = rewriteReceiverExpression(receiver, ctx);
                markInterfaceTargetNeedsCondition(*iface, receiverExpr, SourceLocation{});
                out.push_back("call TriggerEvaluate(" + prefix + "_trigger[" + receiverExpr + "])");
                return;
            }
            std::string finalName = resolveFunctionTargetName(receiver, ctx.currentStruct);
            if (const FunctionInfo* fn = findFunctionInfo(finalName)) {
                ++functionObjectCalls_;
                recordFunctionDependency(ctx.currentFunctionName, fn->finalName);
                out.push_back("call " + fn->finalName + "(" + lowerCallArgs(argsText) + ") // " +
                              std::string(kFunctionObjectEvaluateMarker));
                return;
            }
        }
    }

    if (startsWithWord(line, "set")) {
        std::string rest = trim(std::string_view(line).substr(3));
        if (auto eq = findTopLevelAssignment(rest)) {
            std::string lhs = trim(std::string_view(rest).substr(0, *eq));
            std::string rhs = trim(std::string_view(rest).substr(*eq + 1));
            std::string expectedType;
            if (ctx.localTypes && isSimpleIdentifierText(lhs)) {
                auto it = ctx.localTypes->find(lhs);
                if (it != ctx.localTypes->end() && findFunctionInterface(it->second)) {
                    expectedType = it->second;
                }
            }
            std::vector<std::string> prelude;
            rhs = lowerExpression(rhs, expectedType, ctx, prelude);
            out.insert(out.end(), prelude.begin(), prelude.end());
            bool compact = *eq > 0 && *eq + 1 < rest.size() &&
                           !std::isspace(static_cast<unsigned char>(rest[*eq - 1])) &&
                           !std::isspace(static_cast<unsigned char>(rest[*eq + 1]));
            std::string op = compact ? "=" : " = ";
            std::string lhsOut = lhs;
            LineFeatures lhsFeatures = scanLineFeatures(lhs, ctx.currentStruct, ctx.localTypes, ctx.localArrayShapes);
            if (lineNeedsStructRewrite(lhsFeatures, ctx.currentStruct)) {
                static const LocalTypeMap kEmptyLocalTypes;
                const auto& localTypes = ctx.localTypes ? *ctx.localTypes : kEmptyLocalTypes;
                lhsOut = rewriteStructExpression(lhs, ctx.currentStruct, localTypes);
                lhsOut = rewriteArrayAccesses(lhsOut, ctx.localArrayShapes);
            }
            out.push_back("set " + lhsOut + op + rhs);
            return;
        }
    }

    if (startsWithWord(line, "return")) {
        std::string expr = trim(std::string_view(line).substr(6));
        std::vector<std::string> prelude;
        expr = lowerExpression(expr, {}, ctx, prelude);
        out.insert(out.end(), prelude.begin(), prelude.end());
        out.push_back("return " + expr);
        return;
    }

    std::vector<std::string> prelude;
    line = lowerExpression(line, {}, ctx, prelude);
    out.insert(out.end(), prelude.begin(), prelude.end());
    line = normalizeDiscardedFieldCallStatement(line);
    if (prelude.empty() && line == originalTrim && !leading.empty()) {
        out.push_back(rawLine);
    } else {
        out.push_back(line);
    }
}

std::vector<std::string> Phase1Codegen::lowerStatementLine(const std::string& rawLine, LoweringContext& ctx) const {
    std::vector<std::string> out;
    lowerStatementLineInto(rawLine, ctx, out);
    return out;
}

std::vector<std::string> Phase1Codegen::lowerBodyByMode(BodyMode mode,
                                                        const std::vector<std::string>& lines,
                                                        LoweringContext& ctx) {
    ctx.mode = mode;
    if (mode == BodyMode::Zinc) {
        return lowerZincBodyFast(lines, ctx);
    }
    if (mode == BodyMode::Generated) {
        return lowerGeneratedBodyFast(lines, ctx);
    }
    return lowerJassLikeBodyFast(lines, ctx);
}

std::vector<std::string> Phase1Codegen::lowerZincBodyFast(const std::vector<std::string>& lines,
                                                          LoweringContext&) {
    ++performanceCounters_.bodyLowererZincBodies;
    performanceCounters_.bodyLowererZincLines += lines.size();
    std::vector<std::string> lowered = lowerZincBody(lines);
    if (lowered.size() < lines.size()) {
        performanceCounters_.zincFastPathLines += lines.size() - lowered.size();
    }
    return lowered;
}

std::vector<std::string> Phase1Codegen::lowerJassLikeBodyFast(const std::vector<std::string>& lines,
                                                              LoweringContext&) {
    ++performanceCounters_.bodyLowererJassLikeBodies;
    performanceCounters_.bodyLowererJassLikeLines += lines.size();
    return lines;
}

std::vector<std::string> Phase1Codegen::lowerGeneratedBodyFast(const std::vector<std::string>& lines,
                                                               LoweringContext&) {
    ++performanceCounters_.bodyLowererGeneratedBodies;
    performanceCounters_.bodyLowererGeneratedLines += lines.size();
    performanceCounters_.generatedLinesSkippedGenericLowering += lines.size();
    performanceCounters_.generatedFastPathLines += lines.size();
    return lines;
}

std::vector<std::string> Phase1Codegen::lowerZincBody(const std::vector<std::string>& lines) {
    std::vector<std::string> locals;
    std::vector<std::string> body;
    locals.reserve(lines.size() / 4 + 1);
    body.reserve(lines.size());
    bool simpleBody = true;
    for (const auto& raw : lines) {
        std::string t = raw.find("//") == std::string::npos
                            ? trim(raw)
                            : trim(stripLineCommentPreservingLiterals(raw));
        if (t.empty() || t.rfind("//", 0) == 0) {
            lowerZincSimpleStatement(t, locals, body);
            continue;
        }
        if (t.front() == '.' ||
            t.find('{') != std::string::npos ||
            t.find('}') != std::string::npos ||
            startsWithWord(t, "if") ||
            startsWithWord(t, "else") ||
            startsWithWord(t, "while") ||
            startsWithWord(t, "for") ||
            hasUnclosedContinuation(t) ||
            startsWithZincContinuationOperator(t)) {
            simpleBody = false;
            break;
        }
        size_t semicolon = t.find(';');
        if (semicolon != std::string::npos) {
            size_t last = t.find_last_not_of(" \t");
            if (semicolon != last || t.find(';', semicolon + 1) != std::string::npos) {
                simpleBody = false;
                break;
            }
        }
        lowerZincSimpleStatement(t, locals, body);
    }
    if (simpleBody) {
        std::vector<std::string> out;
        out.reserve(locals.size() + body.size());
        out.insert(out.end(), locals.begin(), locals.end());
        out.insert(out.end(), body.begin(), body.end());
        return out;
    }
    locals.clear();
    body.clear();
    std::vector<std::string> normalized = splitZincStructuralLines(expandLeadingDotChains(joinZincContinuationLines(lines)));
    size_t index = 0;
    lowerZincBlock(normalized, index, locals, body);
    std::vector<std::string> out;
    out.reserve(locals.size() + body.size());
    out.insert(out.end(), locals.begin(), locals.end());
    out.insert(out.end(), body.begin(), body.end());
    return out;
}

std::vector<std::string> Phase1Codegen::extractZincLambdas(const std::vector<std::string>& lines, SourceLocation loc) {
    std::vector<std::string> out;
    std::unordered_set<std::string> localNames;

    auto rememberLocal = [&](const std::string& raw) {
        for (const auto& name : declaredNamesInZincStatements(raw)) {
            localNames.insert(name);
        }
    };

    for (size_t index = 0; index < lines.size(); ++index) {
        std::string line = lines[index];
        while (true) {
            auto lambdaStart = findAnonymousFunctionStart(line);
            if (!lambdaStart) {
                out.push_back(line);
                rememberLocal(line);
                break;
            }
            size_t fn = lambdaStart->first;
            size_t open = lambdaStart->second;

            size_t close = findMatchingParen(line, open);
            if (close == std::string::npos) {
                out.push_back(line);
                break;
            }
            size_t brace = line.find('{', close);
            if (brace == std::string::npos) {
                out.push_back(line);
                break;
            }

            LambdaInfo lambda;
            lambda.loc = loc;
            lambda.currentStruct = lambdaContextStruct_;
            lambda.container = lambdaContextContainer_;
            lambda.loc.line += static_cast<uint32_t>(index);
            lambda.name = "vjlambda__" + std::to_string(nextLambdaId_++);
            lambda.returnType = TypeRef{"nothing", loc, false};
            std::string paramsText = line.substr(open + 1, close - open - 1);
            lambda.params = parseCodegenParamList(paramsText, loc);
            size_t arrow = line.find("->", close);
            if (arrow != std::string::npos && arrow < brace) {
                std::string ret = trim(std::string_view(line).substr(arrow + 2, brace - (arrow + 2)));
                if (!ret.empty()) {
                    lambda.returnType = TypeRef{ret, loc, false};
                }
            }

            std::string before = line.substr(0, fn);
            recordLambdaContext(before);
            std::string afterBrace = line.substr(brace + 1);
            int depth = 1;
            std::string suffix;
            auto consumeBodyText = [&](const std::string& text) {
                std::string current;
                bool foundClose = false;
                for (size_t i = 0; i < text.size(); ++i) {
                    char c = text[i];
                    if (c == '{') {
                        ++depth;
                        current.push_back(c);
                    } else if (c == '}') {
                        --depth;
                        if (depth == 0) {
                            if (!trim(current).empty()) {
                                lambda.bodyLines.push_back(trim(current));
                            }
                            suffix = text.substr(i + 1);
                            foundClose = true;
                            break;
                        }
                        current.push_back(c);
                    } else {
                        current.push_back(c);
                    }
                }
                if (!foundClose && !trim(current).empty()) {
                    lambda.bodyLines.push_back(trim(current));
                }
                return foundClose;
            };

            bool closed = consumeBodyText(afterBrace);
            while (!closed && index + 1 < lines.size()) {
                ++index;
                closed = consumeBodyText(lines[index]);
            }
            lambda.bodyLines = extractZincLambdas(lambda.bodyLines, lambda.loc);

            std::unordered_set<std::string> lambdaScopedNames;
            for (const auto& param : lambda.params) {
                lambdaScopedNames.insert(param.name);
            }
            for (const auto& bodyLine : lambda.bodyLines) {
                for (const auto& local : declaredNamesInZincStatements(bodyLine)) {
                    lambdaScopedNames.insert(local);
                }
            }
            bool captured = false;
            for (const auto& local : localNames) {
                if (lambdaScopedNames.contains(local)) {
                    continue;
                }
                for (const auto& bodyLine : lambda.bodyLines) {
                    if (containsIdentifierOutsideProtected(bodyLine, local)) {
                        diagnostics_.error(lambda.loc, "capturing lambda is not supported in phase 5: '" + local + "'");
                        captured = true;
                        break;
                    }
                }
            }
            if (captured) {
                ++lambdasCapturing_;
                ++lambdasRejected_;
            }

            FunctionInfo fnInfo;
            fnInfo.sourceName = lambda.name;
            fnInfo.finalName = lambda.name;
            fnInfo.signature = signatureFromParams(lambda.params, lambda.returnType, lambda.currentStruct);
            size_t fnIndex = functions_.size();
            functions_.push_back(fnInfo);
            functionIndexByName_[lambda.name] = fnIndex;
            functionLookupCache_.clear();

            line = before + "function " + lambda.name + suffix;
            lambdas_.push_back(std::move(lambda));
        }
    }
    return out;
}

void Phase1Codegen::recordLambdaContext(const std::string& beforeText) {
    std::string before = trim(beforeText);
    if (before.find("Condition(") != std::string::npos || before.find("Filter(") != std::string::npos) {
        ++lambdasBoolexprContext_;
        return;
    }
    if (before.find('.') != std::string::npos) {
        ++lambdasMethodCallbackContext_;
        return;
    }
    static const std::vector<std::string> nativeCallbackNames = {
        "TimerStart",
        "TriggerAddAction",
        "TriggerAddCondition",
        "ForForce",
        "ForGroup",
        "EnumDestructablesInRect",
        "GroupEnumUnitsInRange",
        "GroupEnumUnitsInRangeEx",
        "DzFrameSetUpdateCallbackByCode",
        "DzTriggerRegisterMouseWheelEventByCode",
        "DzTriggerRegisterWindowResizeEventByCode",
        "DzTriggerRegisterMouseMoveEventByCode",
        "DzFrameSetScriptByCode",
        "DzTriggerRegisterKeyEventByCode",
    };
    for (const auto& name : nativeCallbackNames) {
        if (before.find(name + "(") != std::string::npos) {
            ++lambdasNativeCallbackContext_;
            return;
        }
    }
    ++lambdasCodeContext_;
}

void Phase1Codegen::lowerZincBlock(const std::vector<std::string>& lines, size_t& index, std::vector<std::string>& locals, std::vector<std::string>& body) {
    auto emitInlineStatements = [&](const std::string& inlineBody) {
        bool inElse = false;
        for (auto statement : splitSemicolons(inlineBody)) {
            statement = trim(statement);
            if (statement.empty()) {
                continue;
            }
            if (startsWithWord(statement, "else")) {
                if (!inElse) {
                    body.push_back("else");
                    inElse = true;
                }
                statement = trim(std::string_view(statement).substr(4));
                if (statement.empty()) {
                    continue;
                }
            }
            lowerZincSimpleStatement(statement, locals, body);
        }
    };
    auto consumeCloseBeforeElse = [&]() {
        if (index >= lines.size() || trim(lines[index]) != "}") {
            return;
        }
        size_t next = index + 1;
        while (next < lines.size()) {
            std::string lookahead = trim(lines[next]);
            if (!lookahead.empty() && lookahead.rfind("//", 0) != 0) {
                break;
            }
            ++next;
        }
        if (next < lines.size() && startsWithWord(trim(lines[next]), "else")) {
            index = next;
        }
    };

    while (index < lines.size()) {
        std::string line = trim(lines[index]);
        if (line.empty() || line.rfind("//", 0) == 0) {
            ++index;
            continue;
        }
        if (line == "}" || line.rfind("} else", 0) == 0) {
            return;
        }
        if (startsWithWord(line, "while")) {
            std::string cond = conditionFromHeader(line);
            body.push_back("loop");
            body.push_back("    exitwhen not " + cond);
            ++index;
            lowerZincBlock(lines, index, locals, body);
            if (index < lines.size() && trim(lines[index]) == "}") {
                ++index;
            }
            body.push_back("endloop");
            continue;
        }
        if (startsWithWord(line, "if")) {
            std::string cond = conditionFromHeader(line);
            body.push_back("if " + cond + " then");
            size_t close = findMatchingParen(line, line.find('('));
            std::string inlineBody = close == std::string::npos ? "" : trim(std::string_view(line).substr(close + 1));
            if (!inlineBody.empty() && inlineBody.front() != '{') {
                emitInlineStatements(inlineBody);
                ++index;
                while (index < lines.size() && startsWithWord(trim(lines[index]), "else if")) {
                    std::string elseIfLine = trim(lines[index]);
                    size_t ifPos = elseIfLine.find("if");
                    size_t elseOpen = elseIfLine.find('(', ifPos);
                    size_t elseClose = elseOpen == std::string::npos ? std::string::npos : findMatchingParen(elseIfLine, elseOpen);
                    body.push_back("elseif " + conditionFromHeader(elseIfLine.substr(ifPos)) + " then");
                    std::string elseInline = elseClose == std::string::npos ? "" : trim(std::string_view(elseIfLine).substr(elseClose + 1));
                    if (!elseInline.empty() && elseInline.front() == '{') {
                        ++index;
                        lowerZincBlock(lines, index, locals, body);
                        if (index < lines.size() && trim(lines[index]) == "}") {
                            ++index;
                        }
                    } else {
                        emitInlineStatements(elseInline);
                        ++index;
                    }
                }
                if (index < lines.size() && startsWithWord(trim(lines[index]), "else")) {
                    std::string elseLine = trim(lines[index]);
                    body.push_back("else");
                    std::string elseInline = trim(std::string_view(elseLine).substr(4));
                    if (!elseInline.empty() && elseInline.front() == '{') {
                        ++index;
                        lowerZincBlock(lines, index, locals, body);
                        if (index < lines.size() && trim(lines[index]) == "}") {
                            ++index;
                        }
                    } else {
                        emitInlineStatements(elseInline);
                        ++index;
                    }
                }
                body.push_back("endif");
                continue;
            }
            ++index;
            lowerZincBlock(lines, index, locals, body);
            consumeCloseBeforeElse();
            while (index < lines.size() &&
                   (trim(lines[index]).rfind("} else if", 0) == 0 || trim(lines[index]).rfind("else if", 0) == 0)) {
                std::string elseIfLine = trim(lines[index]);
                size_t ifPos = elseIfLine.find("if");
                std::string elseIfCond = ifPos == std::string::npos ? "()" : conditionFromHeader(elseIfLine.substr(ifPos));
                body.push_back("elseif " + elseIfCond + " then");
                ++index;
                lowerZincBlock(lines, index, locals, body);
                consumeCloseBeforeElse();
            }
            if (index < lines.size() &&
                (trim(lines[index]).rfind("} else", 0) == 0 || trim(lines[index]).rfind("else", 0) == 0)) {
                body.push_back("else");
                ++index;
                lowerZincBlock(lines, index, locals, body);
            }
            if (index < lines.size() && trim(lines[index]) == "}") {
                ++index;
            }
            body.push_back("endif");
            continue;
        }
        if (startsWithWord(line, "for")) {
            std::string content = parenContent(line);
            auto parts = splitSemicolons(content);
            if (parts.size() == 3) {
                lowerZincSimpleStatement(parts[0], locals, body);
                body.push_back("loop");
                body.push_back("    exitwhen not (" + parts[1] + ")");
                ++index;
                lowerZincBlock(lines, index, locals, body);
                lowerZincSimpleStatement(parts[2], locals, body);
                if (index < lines.size() && trim(lines[index]) == "}") {
                    ++index;
                }
                body.push_back("endloop");
                continue;
            }
            RangeForParts range = parseRangeForParts(content);
            if (range.ok) {
                body.push_back("set " + range.var + " = " + range.start);
                body.push_back("loop");
                body.push_back("    exitwhen not (" + range.var + " " + range.upperOp + " " + range.end + ")");
                ++index;
                lowerZincBlock(lines, index, locals, body);
                body.push_back("set " + range.var + " = " + range.var + " + 1");
                if (index < lines.size() && trim(lines[index]) == "}") {
                    ++index;
                }
                body.push_back("endloop");
                continue;
            }
        }
        if (line.find(';') != std::string::npos) {
            for (const auto& statement : splitSemicolons(line)) {
                if (!trim(statement).empty()) {
                    lowerZincSimpleStatement(statement, locals, body);
                }
            }
            ++index;
            continue;
        }
        lowerZincSimpleStatement(line, locals, body);
        ++index;
    }
}

void Phase1Codegen::lowerZincSimpleStatement(const std::string& rawStatement, std::vector<std::string>& locals, std::vector<std::string>& body) {
    std::string statement = removeSemicolon(rawStatement);
    if (statement.empty()) {
        return;
    }
    if (startsWithWord(statement, "else") || statement.find('{') != std::string::npos || statement.find('}') != std::string::npos) {
        return;
    }
    if (startsWithWord(statement, "return")) {
        body.push_back(statement);
        return;
    }
    if (startsWithWord(statement, "break")) {
        body.push_back("exitwhen true");
        return;
    }
    if (startsWithWord(statement, "call")) {
        body.push_back(statement);
        return;
    }
    if (isLocalDecl(statement)) {
        ParsedLocalDeclList parsed = parseLocalDeclList(statement);
        if (!parsed.matched) {
            return;
        }
        for (const auto& decl : parsed.decls) {
            if (decl.dimensions.size() > 1) {
                std::string suffix;
                for (int dim : decl.dimensions) {
                    suffix += "[" + std::to_string(dim) + "]";
                }
                locals.push_back(std::string("local ") + (decl.constant ? "constant " : "") + decl.type + " " + decl.name + suffix);
            } else {
                locals.push_back(std::string("local ") + (decl.constant ? "constant " : "") + decl.type +
                                 (decl.array ? " array " : " ") + decl.name);
            }
            if (!decl.initializer.empty() && !decl.array) {
                body.push_back("set " + decl.name + " = " + decl.initializer);
            }
        }
        return;
    }
    auto emitAssignAt = [&](size_t pos, const std::string& op, const std::string& jassOp) {
        std::string lhs = trim(std::string_view(statement).substr(0, pos));
        std::string rhs = trim(std::string_view(statement).substr(pos + op.size()));
        if (op == "=") {
            body.push_back("set " + lhs + " = " + rhs);
        } else {
            body.push_back("set " + lhs + " = " + lhs + " " + jassOp + " " + rhs);
        }
    };
    auto emitAssign = [&](const std::string& op, const std::string& jassOp) -> bool {
        if (statement.find(op.front()) == std::string::npos) {
            return false;
        }
        auto pos = findTopLevelOperatorAssignment(statement, op);
        if (!pos) {
            return false;
        }
        emitAssignAt(*pos, op, jassOp);
        return true;
    };
    if (emitAssign("+=", "+") || emitAssign("-=", "-") || emitAssign("*=", "*") || emitAssign("/=", "/")) {
        return;
    }
    if (statement.find('=') == std::string::npos) {
        if (!statement.empty()) {
            body.push_back("call " + statement);
        }
        return;
    }
    if (auto pos = findTopLevelAssignment(statement)) {
        emitAssignAt(*pos, "=", "=");
        return;
    }
    if (!statement.empty()) {
        body.push_back("call " + statement);
    }
}

} // namespace vjassc
