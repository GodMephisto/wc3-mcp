---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/validation/OutputAnalysis.cpp"
ref: HEAD
---

#include "validation/OutputAnalysis.h"

#include "util/PathUtil.h"

#include <algorithm>
#include <cctype>
#include <filesystem>
#include <fstream>
#include <regex>
#include <sstream>
#include <unordered_map>
#include <unordered_set>
#include <utility>

namespace vjassc {
namespace {

bool isWordPart(char c) {
    return std::isalnum(static_cast<unsigned char>(c)) || c == '_' || c == '$';
}

bool containsWord(const std::string& text, const std::string& word) {
    size_t pos = 0;
    while ((pos = text.find(word, pos)) != std::string::npos) {
        char before = pos > 0 ? text[pos - 1] : '\0';
        char after = pos + word.size() < text.size() ? text[pos + word.size()] : '\0';
        if (!isWordPart(before) && !isWordPart(after)) {
            return true;
        }
        pos += word.size();
    }
    return false;
}

std::string stripProtected(std::string_view line) {
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

std::string stripLineCommentPreservingLiterals(std::string_view line) {
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    for (size_t i = 0; i < line.size(); ++i) {
        if (!inString && !inRaw && i + 1 < line.size() && line[i] == '/' && line[i + 1] == '/') {
            return std::string(line.substr(0, i));
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
        } else if (inRaw) {
            if (c == '\'') {
                inRaw = false;
            }
        } else if (c == '"') {
            inString = true;
        } else if (c == '\'') {
            inRaw = true;
        }
    }
    return std::string(line);
}

bool containsAnonymousFunctionSyntax(const std::string& text) {
    size_t pos = 0;
    while ((pos = text.find("function", pos)) != std::string::npos) {
        char before = pos > 0 ? text[pos - 1] : '\0';
        char after = pos + 8 < text.size() ? text[pos + 8] : '\0';
        if (!isWordPart(before) && !isWordPart(after)) {
            size_t p = pos + 8;
            while (p < text.size() && std::isspace(static_cast<unsigned char>(text[p]))) {
                ++p;
            }
            if (p < text.size() && text[p] == '(') {
                return true;
            }
        }
        pos += 8;
    }
    return false;
}

bool hasChainedIndexing(const std::string& text) {
    bool sawClose = false;
    for (char c : text) {
        if (std::isspace(static_cast<unsigned char>(c))) {
            continue;
        }
        if (sawClose) {
            if (c == '[') {
                return true;
            }
            sawClose = false;
        }
        if (c == ']') {
            sawClose = true;
        }
    }
    return false;
}

bool hasTopLevelComma(const std::string& text) {
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    int parens = 0;
    int brackets = 0;
    for (size_t i = 0; i < text.size(); ++i) {
        char c = text[i];
        if (!inString && !inRaw && i + 1 < text.size() && c == '/' && text[i + 1] == '/') {
            return false;
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
        } else if (c == ',' && parens == 0 && brackets == 0) {
            return true;
        }
    }
    return false;
}

bool hasIndexedStructMemberResidue(const std::string& text) {
    static const std::regex pattern(R"(\[[^\]]+\]\s*\.\s*[A-Za-z_$])");
    return std::regex_search(text, pattern);
}

bool hasMethodChainCallResultResidue(const std::string& text) {
    static const std::regex callResultPattern(R"((\)|\])\s*\.\s*[A-Za-z_$])");
    static const std::regex orphanCallPattern(R"(^\s*call\s+\.\s*[A-Za-z_$])");
    return std::regex_search(text, callResultPattern) || std::regex_search(text, orphanCallPattern);
}

std::string knownSourceSymbolInLine(const std::string& text) {
    for (const char* symbol : {"HASH_TIMER", "HASH_ABILITY", "uiHT", "KEY_TOTAL"}) {
        if (containsWord(text, symbol)) {
            return symbol;
        }
    }
    return {};
}

std::string callbackTargetInLine(const std::string& text) {
    static const std::regex pattern(R"(TriggerAdd(?:Action|Condition)\s*\([^,]+,\s*(?:Condition\s*\(\s*)?function\s+([A-Za-z_$][A-Za-z0-9_$]*))");
    std::smatch match;
    if (std::regex_search(text, match, pattern)) {
        return match[1].str();
    }
    return {};
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

bool isValidJassFunctionStatementShape(std::string_view statement) {
    std::string t = trim(statement);
    while (startsWithWord(t, "debug")) {
        t = trim(std::string_view(t).substr(5));
    }
    if (t.empty()) {
        return true;
    }
    if (startsWithWord(t, "local") ||
        startsWithWord(t, "set") ||
        startsWithWord(t, "call") ||
        startsWithWord(t, "return") ||
        startsWithWord(t, "if") ||
        startsWithWord(t, "elseif") ||
        startsWithWord(t, "exitwhen")) {
        return true;
    }
    return t == "else" || t == "endif" || t == "loop" || t == "endloop";
}

bool hasInlineZincControlResidue(const std::string& text) {
    std::string t = trim(text);
    if (!startsWithWord(t, "if")) {
        return false;
    }
    size_t open = t.find('(');
    if (open == std::string::npos) {
        return false;
    }
    int depth = 0;
    size_t close = std::string::npos;
    for (size_t i = open; i < t.size(); ++i) {
        if (t[i] == '(') {
            ++depth;
        } else if (t[i] == ')') {
            --depth;
            if (depth == 0) {
                close = i;
                break;
            }
        }
    }
    if (close == std::string::npos) {
        return false;
    }
    std::string after = trim(std::string_view(t).substr(close + 1));
    if (startsWithWord(after, "then")) {
        return false;
    }
    return startsWithWord(after, "return") || startsWithWord(after, "call") || startsWithWord(after, "set");
}

std::string functionNameFromHeader(std::string_view line) {
    size_t pos = 0;
    while (pos < line.size() && std::isspace(static_cast<unsigned char>(line[pos]))) {
        ++pos;
    }
    while (pos < line.size() && !std::isspace(static_cast<unsigned char>(line[pos]))) {
        ++pos;
    }
    while (pos < line.size() && std::isspace(static_cast<unsigned char>(line[pos]))) {
        ++pos;
    }
    size_t start = pos;
    while (pos < line.size() && isWordPart(line[pos])) {
        ++pos;
    }
    return start == pos ? std::string{} : std::string(line.substr(start, pos - start));
}

std::string returnTypeFromHeader(std::string_view line) {
    size_t pos = line.find(" returns ");
    if (pos == std::string::npos) {
        return {};
    }
    return trim(std::string_view(line).substr(pos + 9));
}

std::string declarationNameInGlobals(std::string_view line) {
    std::istringstream in{std::string(line)};
    std::string word;
    std::string type;
    std::string name;
    in >> word;
    if (word == "constant") {
        in >> type;
    } else {
        type = word;
    }
    in >> name;
    if (name == "array") {
        in >> name;
    }
    if (name.empty()) {
        return {};
    }
    size_t bracket = name.find('[');
    if (bracket != std::string::npos) {
        name = name.substr(0, bracket);
    }
    size_t eq = name.find('=');
    if (eq != std::string::npos) {
        name = name.substr(0, eq);
    }
    return name;
}

void addIssue(OutputSyntaxReport& report, std::string check, size_t line, std::string message, std::string_view snippet) {
    report.ok = false;
    report.issues.push_back(ValidationIssue{std::move(check), line, std::move(message), trim(snippet)});
}

void addResidue(OutputSyntaxReport& report, const std::string& label) {
    if (std::find(report.residualSourceForms.begin(), report.residualSourceForms.end(), label) ==
        report.residualSourceForms.end()) {
        report.residualSourceForms.push_back(label);
    }
    ++report.metrics.sourceFormResidues;
}

std::string readTextFile(const std::filesystem::path& path) {
    std::ifstream in(path, std::ios::binary);
    std::ostringstream buffer;
    buffer << in.rdbuf();
    return buffer.str();
}

bool containsCallInMain(std::string_view output, const std::string& callName) {
    std::istringstream in{std::string(output)};
    std::string line;
    bool inMain = false;
    while (std::getline(in, line)) {
        std::string t = trim(stripProtected(line));
        if (startsWithWord(t, "function main")) {
            inMain = true;
            continue;
        }
        if (inMain && startsWithWord(t, "endfunction")) {
            return false;
        }
        if (inMain && t.find("call " + callName + "(") != std::string::npos) {
            return true;
        }
    }
    return false;
}

std::string blankProtectedForAnalysis(std::string_view line) {
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

struct ForwardReferenceReports {
    std::vector<ValidationIssue> functions;
    std::vector<ValidationIssue> lambdas;
};

template <typename Lines>
ForwardReferenceReports findForwardFunctionReferences(const Lines& lines) {
    std::unordered_map<std::string, size_t> definitions;
    for (size_t i = 0; i < lines.size(); ++i) {
        std::string t = trim(stripProtected(lines[i]));
        if (startsWithWord(t, "function")) {
            std::string name = functionNameFromHeader(t);
            if (!name.empty()) {
                definitions[name] = i + 1;
            }
        }
    }

    ForwardReferenceReports reports;
    std::string currentFunction;
    for (size_t i = 0; i < lines.size(); ++i) {
        std::string t = trim(stripProtected(lines[i]));
        if (startsWithWord(t, "function")) {
            currentFunction = functionNameFromHeader(t);
            continue;
        }
        if (startsWithWord(t, "endfunction")) {
            currentFunction.clear();
            continue;
        }
        if (currentFunction.empty()) {
            continue;
        }
        std::string code = blankProtectedForAnalysis(lines[i]);
        auto add = [&](const std::string& name) {
            if (name.empty() || name == currentFunction) {
                return;
            }
            auto def = definitions.find(name);
            if (def != definitions.end() && def->second > i + 1) {
                const bool lambda = name.rfind("vjlambda__", 0) == 0;
                auto& issues = lambda ? reports.lambdas : reports.functions;
                issues.push_back(ValidationIssue{lambda ? "forwardLambdaReference" : "forwardFunctionReference",
                                                 i + 1,
                                                 "reference before declaration: " + name + " in " + currentFunction,
                                                 trim(lines[i])});
            }
        };
        for (size_t pos = 0; pos < code.size();) {
            if (!isWordPart(code[pos]) || std::isdigit(static_cast<unsigned char>(code[pos]))) {
                ++pos;
                continue;
            }
            size_t start = pos++;
            while (pos < code.size() && isWordPart(code[pos])) {
                ++pos;
            }
            std::string name = code.substr(start, pos - start);
            size_t after = pos;
            while (after < code.size() && std::isspace(static_cast<unsigned char>(code[after]))) {
                ++after;
            }
            if (name == "function") {
                while (after < code.size() && !isWordPart(code[after])) {
                    ++after;
                }
                if (after < code.size() && !std::isdigit(static_cast<unsigned char>(code[after]))) {
                    size_t targetStart = after++;
                    while (after < code.size() && isWordPart(code[after])) {
                        ++after;
                    }
                    add(code.substr(targetStart, after - targetStart));
                }
                continue;
            }
            if (after < code.size() && code[after] == '(' &&
                name != "if" && name != "call" && name != "set" && name != "return" &&
                name != "loop" && name != "exitwhen") {
                add(name);
            }
        }
    }
    return reports;
}

size_t linePositionInMain(std::string_view output, const std::string& callName) {
    std::istringstream in{std::string(output)};
    std::string line;
    bool inMain = false;
    size_t pos = 0;
    size_t lineNo = 0;
    while (std::getline(in, line)) {
        ++lineNo;
        std::string t = trim(stripProtected(line));
        if (startsWithWord(t, "function main")) {
            inMain = true;
            continue;
        }
        if (inMain && startsWithWord(t, "endfunction")) {
            return 0;
        }
        if (inMain) {
            ++pos;
            if (t.find("call " + callName + "(") != std::string::npos) {
                (void)lineNo;
                return pos;
            }
        }
    }
    return 0;
}

} // namespace

OutputMetrics collectOutputMetrics(std::string_view output) {
    OutputMetrics metrics;
    metrics.bytes = output.size();
    std::istringstream in{std::string(output)};
    std::string line;
    bool inGlobals = false;
    std::unordered_set<std::string> functions;
    std::unordered_set<std::string> globals;
    std::unordered_set<std::string> natives;
    while (std::getline(in, line)) {
        ++metrics.lines;
        std::string t = trim(stripProtected(line));
        if (t.empty()) {
            continue;
        }
        if (t == "globals") {
            ++metrics.globalsBlocks;
            inGlobals = true;
            continue;
        }
        if (t == "endglobals") {
            ++metrics.endglobals;
            inGlobals = false;
            continue;
        }
        if (startsWithWord(t, "native")) {
            ++metrics.natives;
            std::string name = functionNameFromHeader(t);
            if (!name.empty() && !natives.insert(name).second) {
                ++metrics.duplicateNativeNames;
            }
            continue;
        }
        if (startsWithWord(t, "type")) {
            ++metrics.types;
            continue;
        }
        if (startsWithWord(t, "function")) {
            ++metrics.functions;
            std::string name = functionNameFromHeader(t);
            if (name == "main") {
                metrics.hasMain = true;
            } else if (name == "config") {
                metrics.hasConfig = true;
            }
            if (name.rfind("vjlambda__", 0) == 0) {
                ++metrics.generatedLambdaFunctions;
            }
            if (name.rfind("s__", 0) == 0 || name.rfind("sc__", 0) == 0 || name.rfind("si__", 0) == 0) {
                ++metrics.structSupportFunctions;
            }
            if (name.rfind("vjfi__", 0) == 0 &&
                (name.find("__wrapper") != std::string::npos ||
                 name.find("__condition_wrapper") != std::string::npos ||
                 name.find("__action_wrapper") != std::string::npos)) {
                ++metrics.functionInterfaceWrappers;
            }
            if (name.rfind("vjassc__init_", 0) == 0) {
                metrics.initHelperNames.push_back(name);
            }
            if (!name.empty() && !functions.insert(name).second) {
                ++metrics.duplicateFunctionNames;
            }
            continue;
        }
        if (inGlobals) {
            ++metrics.globalsDeclarations;
            std::string name = declarationNameInGlobals(t);
            if (!name.empty() && !globals.insert(name).second) {
                ++metrics.duplicateGlobalNames;
            }
        }
    }
    return metrics;
}

OutputSyntaxReport analyzeOutputSyntaxLite(std::string_view output) {
    OutputSyntaxReport report;
    report.ran = true;
    report.metrics.bytes = output.size();
    std::vector<std::string_view> lines = splitLinesView(output);
    bool inGlobals = false;
    bool inFunction = false;
    bool sawExecutableInFunction = false;
    bool sawReturnInFunction = false;
    std::string currentReturnType;
    std::string currentFunctionName;
    size_t currentFunctionLine = 0;
    size_t functionDepth = 0;
    size_t lineNo = 0;
    std::unordered_set<std::string> functions;
    std::unordered_set<std::string> globals;
    std::unordered_set<std::string> natives;
    const std::vector<std::string> forbiddenWords = {
        "library", "endlibrary", "scope", "endscope", "struct", "endstruct",
        "method", "endmethod", "module", "endmodule", "implement", "delegate",
        "stub", "super", "operator", "interface"
    };
    for (std::string_view line : lines) {
        ++lineNo;
        ++report.metrics.lines;
        std::string rawTrim = trim(line);
        std::string code = stripProtected(line);
        std::string t = trim(code);
        if (t.empty()) {
            continue;
        }
        if (t == "globals") {
            ++report.metrics.globalsBlocks;
            inGlobals = true;
            continue;
        }
        if (t == "endglobals") {
            ++report.metrics.endglobals;
            inGlobals = false;
            continue;
        }
        if (startsWithWord(t, "native")) {
            ++report.metrics.natives;
            std::string name = functionNameFromHeader(t);
            if (!name.empty() && !natives.insert(name).second) {
                ++report.metrics.duplicateNativeNames;
            }
            continue;
        }
        if (startsWithWord(t, "type")) {
            ++report.metrics.types;
            continue;
        }
        if (startsWithWord(t, "function")) {
            ++report.metrics.functions;
            std::string name = functionNameFromHeader(t);
            if (name == "main") {
                report.metrics.hasMain = true;
            } else if (name == "config") {
                report.metrics.hasConfig = true;
            }
            if (name.rfind("vjlambda__", 0) == 0) {
                ++report.metrics.generatedLambdaFunctions;
            }
            if (name.rfind("s__", 0) == 0 || name.rfind("sc__", 0) == 0 || name.rfind("si__", 0) == 0) {
                ++report.metrics.structSupportFunctions;
            }
            if (name.rfind("vjfi__", 0) == 0 &&
                (name.find("__wrapper") != std::string::npos ||
                 name.find("__condition_wrapper") != std::string::npos ||
                 name.find("__action_wrapper") != std::string::npos)) {
                ++report.metrics.functionInterfaceWrappers;
            }
            if (name.rfind("vjassc__init_", 0) == 0) {
                report.metrics.initHelperNames.push_back(name);
            }
            if (!name.empty() && !functions.insert(name).second) {
                ++report.metrics.duplicateFunctionNames;
            }
        } else if (inGlobals) {
            ++report.metrics.globalsDeclarations;
            std::string name = declarationNameInGlobals(t);
            if (!name.empty() && !globals.insert(name).second) {
                ++report.metrics.duplicateGlobalNames;
            }
        }
        if (rawTrim.rfind("//!", 0) == 0) {
            addResidue(report, "//! directive");
            addIssue(report, "noBangDirective", lineNo, "residual //! directive", line);
        }
        for (const auto& word : forbiddenWords) {
            if (containsWord(t, word)) {
                addResidue(report, word);
                addIssue(report, "noSourceForms", lineNo, "residual source form '" + word + "'", line);
                break;
            }
        }
        if (t.find("static if") != std::string::npos) {
            addResidue(report, "static if");
            addIssue(report, "noSourceForms", lineNo, "residual static if", line);
        }
        if (t.find("function interface") != std::string::npos) {
            addResidue(report, "function interface");
            addIssue(report, "noSourceForms", lineNo, "residual function interface", line);
        }
        if (containsAnonymousFunctionSyntax(t)) {
            addResidue(report, "anonymous function");
            addIssue(report, "noAnonymousFunction", lineNo, "residual anonymous function syntax", line);
        }
        if (t.find('{') != std::string::npos || t.find('}') != std::string::npos || t.find(';') != std::string::npos ||
            t.find("->") != std::string::npos || t.find("=>") != std::string::npos) {
            addResidue(report, "zinc syntax");
            addIssue(report, "noZincSyntax", lineNo, "residual Zinc syntax token", line);
        }
        if (t.find("thistype") != std::string::npos) {
            addResidue(report, "thistype");
            addIssue(report, "noThistype", lineNo, "residual thistype", line);
        }
        if (t.find(".execute(") != std::string::npos || t.find(".evaluate(") != std::string::npos) {
            addResidue(report, "function object member");
            addIssue(report, "noFunctionObjectMembers", lineNo, "residual function-object member access", line);
        }
        if (t.find("||") != std::string::npos || t.find("&&") != std::string::npos) {
            addIssue(report, "noZincBooleanOperators", lineNo, "residual Zinc boolean operator", line);
        }
        if (t.find("/*") != std::string::npos || t.find("*/") != std::string::npos) {
            addIssue(report, "noBlockCommentLeak", lineNo, "residual block comment marker", line);
        }
        if (hasChainedIndexing(t)) {
            addIssue(report, "noMultidimensionalArray", lineNo, "residual chained array indexing", line);
        }
        if (t.find(']') != std::string::npos && t.find('.') != std::string::npos &&
            hasIndexedStructMemberResidue(t)) {
            ValidationIssue issue{"indexedStructMemberResidue", lineNo, "residual indexed struct member access", trim(line)};
            report.indexedStructMemberResidues.push_back(issue);
            addIssue(report, issue.check, issue.line, issue.message, issue.snippet);
        }
        if (hasInlineZincControlResidue(t)) {
            ValidationIssue issue{"inlineZincControlResidue", lineNo, "residual inline Zinc control form", trim(line)};
            report.inlineZincControlResidues.push_back(issue);
            addIssue(report, issue.check, issue.line, issue.message, issue.snippet);
        }
        if ((t.find(").") != std::string::npos || t.find("].") != std::string::npos ||
             t.find("call .") != std::string::npos) &&
            hasMethodChainCallResultResidue(t)) {
            report.methodChainCallResultResidues.push_back(ValidationIssue{
                "methodChainCallResultResidue",
                lineNo,
                "method chain on call or indexed result remains",
                trim(line),
            });
        }
        if (std::string symbol = knownSourceSymbolInLine(t); !symbol.empty()) {
            report.unresolvedKnownSourceSymbols.push_back(ValidationIssue{
                "knownSourceSymbol",
                lineNo,
                "known source/environment symbol remains: " + symbol,
                trim(line),
            });
        }
        if (t.find("TriggerAdd") != std::string::npos && t.find("vjlambda__") != std::string::npos) {
            if (std::string callbackTarget = callbackTargetInLine(t); !callbackTarget.empty() &&
                callbackTarget.rfind("vjlambda__", 0) == 0) {
                report.callbackCodeSignatureResidues.push_back(ValidationIssue{
                    "callbackCodeSignatureCandidate",
                    lineNo,
                    "lambda target is passed as a raw code callback: " + callbackTarget,
                    trim(line),
                });
            }
        }
        if (startsWithWord(t, "function")) {
            if (inFunction) {
                addIssue(report, "noNestedFunction", lineNo, "function declaration nested in function", line);
            }
            inFunction = true;
            ++functionDepth;
            sawExecutableInFunction = false;
            sawReturnInFunction = false;
            currentReturnType = returnTypeFromHeader(t);
            currentFunctionName = functionNameFromHeader(t);
            currentFunctionLine = lineNo;
            if (t.find(" takes ") == std::string::npos || currentReturnType.empty()) {
                addIssue(report, "validFunctionHeader", lineNo, "invalid function header", line);
            }
            continue;
        }
        if (startsWithWord(t, "endfunction")) {
            if (!inFunction || functionDepth == 0) {
                addIssue(report, "balancedFunctions", lineNo, "endfunction without function", line);
            } else {
                if (!currentReturnType.empty() && currentReturnType != "nothing" && !sawReturnInFunction) {
                    report.returnMismatchLikelyResidues.push_back(ValidationIssue{
                        "missingReturnCandidate",
                        currentFunctionLine,
                        "function returning " + currentReturnType + " has no explicit return: " + currentFunctionName,
                        currentFunctionName,
                    });
                }
                --functionDepth;
            }
            inFunction = functionDepth > 0;
            sawExecutableInFunction = false;
            sawReturnInFunction = false;
            currentReturnType.clear();
            currentFunctionName.clear();
            currentFunctionLine = 0;
            continue;
        }
        if (inFunction) {
            if (!isValidJassFunctionStatementShape(t)) {
                addIssue(report, "statementShape", lineNo, "invalid JASS statement shape", line);
            }
            if (startsWithWord(t, "local")) {
                if (hasTopLevelComma(trim(std::string_view(t).substr(5)))) {
                    ValidationIssue issue{"commaSeparatedLocal", lineNo, "comma-separated local declaration remains", trim(line)};
                    report.commaLocalResidues.push_back(issue);
                    addIssue(report, issue.check, issue.line, issue.message, issue.snippet);
                }
                if (sawExecutableInFunction) {
                    addIssue(report, "localOrder", lineNo, "local declaration after executable statement", line);
                }
            } else {
                sawExecutableInFunction = true;
            }
            if (startsWithWord(t, "return")) {
                sawReturnInFunction = true;
                std::string returnLine = trim(stripLineCommentPreservingLiterals(line));
                std::string expr = trim(std::string_view(returnLine).substr(6));
                if (currentReturnType == "nothing" && !expr.empty()) {
                    addIssue(report, "returnType", lineNo, "return value in returns nothing function", line);
                } else if (!currentReturnType.empty() && currentReturnType != "nothing" && expr.empty()) {
                    addIssue(report, "returnType", lineNo, "empty return in non-nothing function", line);
                }
            }
        }
        if (startsWithWord(t, "set") && t.find("= call ") != std::string::npos) {
            addIssue(report, "setCallSyntax", lineNo, "set uses call syntax in assignment", line);
        }
        if (t.find("= call ") != std::string::npos) {
            addIssue(report, "callInAssignment", lineNo, "call used in assignment value", line);
        }
    }
    if (report.metrics.globalsBlocks != 1) {
        addIssue(report, "singleGlobalsBlock", 0, "expected exactly one globals block", "");
    }
    if (report.metrics.endglobals != report.metrics.globalsBlocks || report.metrics.endglobals == 0) {
        addIssue(report, "balancedGlobals", 0, "missing or unbalanced endglobals", "");
    }
    if (functionDepth != 0) {
        addIssue(report, "balancedFunctions", 0, "missing endfunction", "");
    }
    if (report.metrics.duplicateFunctionNames != 0) {
        addIssue(report, "duplicateFunctionNames", 0, "duplicate function declarations remain", "");
    }
    if (report.metrics.duplicateGlobalNames != 0) {
        addIssue(report, "duplicateGlobalNames", 0, "duplicate global declarations remain", "");
    }
    ForwardReferenceReports forwardReferences = findForwardFunctionReferences(lines);
    report.forwardFunctionReferences = std::move(forwardReferences.functions);
    report.forwardLambdaReferences = std::move(forwardReferences.lambdas);
    for (const auto& issue : report.forwardFunctionReferences) {
        report.trueFunctionCycles.push_back(ValidationIssue{
            "trueFunctionCycleCandidate",
            issue.line,
            issue.message,
            issue.snippet,
        });
    }
    return report;
}

std::vector<ValidationIssue> findInvalidJassStatementShapes(std::string_view output) {
    std::vector<ValidationIssue> issues;
    bool inFunction = false;
    size_t functionDepth = 0;
    size_t lineNo = 0;
    for (size_t pos = 0; pos <= output.size();) {
        size_t next = output.find('\n', pos);
        std::string_view line = next == std::string_view::npos
                                    ? output.substr(pos)
                                    : output.substr(pos, next - pos);
        if (!line.empty() && line.back() == '\r') {
            line.remove_suffix(1);
        }
        pos = next == std::string_view::npos ? output.size() + 1 : next + 1;
        ++lineNo;
        std::string t = trim(stripProtected(line));
        if (t.empty()) {
            continue;
        }
        if (startsWithWord(t, "function")) {
            inFunction = true;
            ++functionDepth;
            continue;
        }
        if (startsWithWord(t, "endfunction")) {
            if (functionDepth > 0) {
                --functionDepth;
            }
            inFunction = functionDepth > 0;
            continue;
        }
        if (inFunction && !isValidJassFunctionStatementShape(t)) {
            issues.push_back(ValidationIssue{"statementShape", lineNo, "invalid JASS statement shape", trim(line)});
        }
    }
    return issues;
}

InitValidationReport analyzeInitIntegrity(std::string_view output) {
    InitValidationReport report;
    OutputMetrics metrics = collectOutputMetrics(output);
    report.hasMain = metrics.hasMain;
    report.hasConfig = metrics.hasConfig;
    for (const auto& helper : metrics.initHelperNames) {
        if (helper == "vjassc__init_structs") {
            report.hasStructInit = true;
        } else if (helper == "vjassc__init_function_interfaces") {
            report.hasFunctionInterfaceInit = true;
        } else if (helper == "vjassc__init_libraries") {
            report.hasLibraryInit = true;
        }
    }
    report.mainCallsStructInit = containsCallInMain(output, "vjassc__init_structs");
    report.mainCallsFunctionInterfaceInit = !report.hasFunctionInterfaceInit ||
                                            containsCallInMain(output, "vjassc__init_function_interfaces");
    report.mainCallsLibraryInit = containsCallInMain(output, "vjassc__init_libraries");
    size_t structPos = linePositionInMain(output, "vjassc__init_structs");
    size_t ifacePos = linePositionInMain(output, "vjassc__init_function_interfaces");
    size_t libPos = linePositionInMain(output, "vjassc__init_libraries");
    report.structBeforeFunctionInterface = structPos != 0 && (ifacePos == 0 || structPos < ifacePos);
    report.functionInterfaceBeforeLibrary = libPos != 0 && (ifacePos == 0 || ifacePos < libPos);

    std::istringstream in{std::string(output)};
    std::string line;
    bool inLibraryInit = false;
    bool inStructInit = false;
    while (std::getline(in, line)) {
        std::string t = trim(stripProtected(line));
        if (startsWithWord(t, "function vjassc__init_libraries")) {
            inLibraryInit = true;
            continue;
        }
        if (startsWithWord(t, "function vjassc__init_structs")) {
            inStructInit = true;
            continue;
        }
        if (startsWithWord(t, "endfunction")) {
            inLibraryInit = false;
            inStructInit = false;
            continue;
        }
        if (inLibraryInit && (startsWithWord(t, "call") || t.find("ExecuteFunc(") != std::string::npos)) {
            ++report.libraryInitializerCount;
        }
        if (inStructInit && startsWithWord(t, "call")) {
            ++report.structInitializerCount;
        }
    }
    if (!report.hasMain) {
        report.issues.push_back("missing main");
    }
    if (!report.hasConfig) {
        report.issues.push_back("missing config");
    }
    if (!report.hasStructInit || !report.mainCallsStructInit) {
        report.issues.push_back("struct init helper missing or not called by main");
    }
    if (report.hasFunctionInterfaceInit && !report.mainCallsFunctionInterfaceInit) {
        report.issues.push_back("function interface init helper not called by main");
    }
    if (!report.hasLibraryInit || !report.mainCallsLibraryInit) {
        report.issues.push_back("library init helper missing or not called by main");
    }
    if (!report.structBeforeFunctionInterface || !report.functionInterfaceBeforeLibrary) {
        report.issues.push_back("main init helper order is not struct -> function interface -> library");
    }
    return report;
}

ComparisonReport compareWithReference(std::string_view generated, const std::filesystem::path& referencePath) {
    ComparisonReport report;
    report.referencePath = referencePath;
    report.generated = collectOutputMetrics(generated);
    if (referencePath.empty() || !std::filesystem::exists(referencePath)) {
        report.referenceFound = false;
        report.notes.push_back("JassHelper reference output was not found.");
        return report;
    }
    report.referenceFound = true;
    std::string reference = readTextFile(referencePath);
    report.reference = collectOutputMetrics(reference);
    if (report.generated.functions < report.reference.functions) {
        report.notes.push_back("Generated output has fewer functions than the JassHelper reference; verify wrapper generation and init helpers.");
    }
    if (report.generated.globalsBlocks != 1 || report.reference.globalsBlocks != 1) {
        report.notes.push_back("Generated/reference globals block counts differ from the expected single-block layout.");
    }
    if (!report.generated.hasMain || !report.generated.hasConfig) {
        report.notes.push_back("Generated output is missing main or config.");
    }
    return report;
}

std::string previewText(std::string_view text, size_t maxBytes) {
    if (text.size() <= maxBytes) {
        return std::string(text);
    }
    return std::string(text.substr(0, maxBytes)) + "\n... truncated ...";
}

} // namespace vjassc
