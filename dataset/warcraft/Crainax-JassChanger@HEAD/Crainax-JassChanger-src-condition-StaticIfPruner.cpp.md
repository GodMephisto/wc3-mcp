---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/condition/StaticIfPruner.cpp"
ref: HEAD
---

#include "condition/StaticIfPruner.h"

#include "util/PathUtil.h"

#include <cctype>
#include <optional>
#include <sstream>
#include <string>

namespace vjassc {
namespace {

bool isIdentStart(char c) {
    return std::isalpha(static_cast<unsigned char>(c)) || c == '_' || c == '$';
}

bool isIdentPart(char c) {
    return std::isalnum(static_cast<unsigned char>(c)) || c == '_' || c == '$';
}

std::string readIdent(const std::string& text, size_t& index) {
    if (index >= text.size() || !isIdentStart(text[index])) {
        return {};
    }
    size_t start = index++;
    while (index < text.size() && isIdentPart(text[index])) {
        ++index;
    }
    return text.substr(start, index - start);
}

bool isCommentOrEmpty(const std::string& text) {
    std::string t = trim(text);
    return t.empty() || t.rfind("//", 0) == 0;
}

std::string stripLineComment(std::string text) {
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    for (size_t i = 0; i + 1 < text.size(); ++i) {
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
            continue;
        }
        if (c == '\'') {
            inRaw = true;
            continue;
        }
        if (c == '/' && text[i + 1] == '/') {
            return trim(std::string_view(text).substr(0, i));
        }
    }
    return trim(text);
}

std::string stripTrailingSemicolon(std::string text) {
    text = trim(text);
    if (!text.empty() && text.back() == ';') {
        text.pop_back();
    }
    return trim(text);
}

std::string parseAccessPrefix(std::string& text) {
    std::string t = trim(text);
    if (startsWithWord(t, "public")) {
        text = trim(std::string_view(t).substr(6));
        return "public";
    }
    if (startsWithWord(t, "private")) {
        text = trim(std::string_view(t).substr(7));
        return "private";
    }
    text = t;
    return {};
}

std::string parseNameAfterWord(std::string text, const std::string& word) {
    text = trim(text);
    if (!startsWithWord(text, word)) {
        return {};
    }
    text = trim(std::string_view(text).substr(word.size()));
    size_t index = 0;
    return readIdent(text, index);
}

std::optional<size_t> findWordPos(std::string_view text, std::string_view word) {
    for (size_t i = 0; i + word.size() <= text.size(); ++i) {
        if (text.substr(i, word.size()) != word) {
            continue;
        }
        bool left = i == 0 || !(std::isalnum(static_cast<unsigned char>(text[i - 1])) || text[i - 1] == '_');
        bool right = i + word.size() == text.size() ||
                     !(std::isalnum(static_cast<unsigned char>(text[i + word.size()])) || text[i + word.size()] == '_');
        if (left && right) {
            return i;
        }
    }
    return std::nullopt;
}

std::optional<size_t> findUnprotectedChar(std::string_view text, char needle, size_t start = 0) {
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    for (size_t i = start; i < text.size(); ++i) {
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
        } else if (c == needle) {
            return i;
        }
    }
    return std::nullopt;
}

std::optional<size_t> findMatchingParen(std::string_view text, size_t open) {
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    int depth = 0;
    for (size_t i = open; i < text.size(); ++i) {
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
            ++depth;
        } else if (c == ')') {
            --depth;
            if (depth == 0) {
                return i;
            }
        }
    }
    return std::nullopt;
}

bool startsStaticIf(std::string text) {
    text = trim(text);
    return startsWithWord(text, "static if");
}

bool startsElseIf(std::string text) {
    text = trim(text);
    return startsWithWord(text, "elseif");
}

bool startsElse(std::string text) {
    text = trim(text);
    return startsWithWord(text, "else");
}

bool startsEndIf(std::string text) {
    text = trim(text);
    return startsWithWord(text, "endif");
}

std::string jassStaticIfCondition(std::string text, bool elseifBranch) {
    text = trim(text);
    text = trim(std::string_view(text).substr(elseifBranch ? 6 : 9));
    auto thenPos = findWordPos(text, "then");
    if (thenPos) {
        text = trim(std::string_view(text).substr(0, *thenPos));
    }
    return text;
}

size_t countNonEmptyLines(const std::vector<LogicalLine>& lines) {
    size_t count = 0;
    for (const auto& line : lines) {
        if (!trim(line.text).empty()) {
            ++count;
        }
    }
    return count;
}

std::vector<LogicalLine> splitZincStatements(const LogicalLine& logical) {
    std::vector<LogicalLine> out;
    std::string current;
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    int parens = 0;
    int brackets = 0;
    int braces = 0;
    for (size_t i = 0; i < logical.text.size(); ++i) {
        char c = logical.text[i];
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
        } else if (c == '{') {
            ++braces;
        } else if (c == '}' && braces > 0) {
            --braces;
        }
        current.push_back(c);
        if (c == ';' && parens == 0 && brackets == 0 && braces == 0) {
            if (!trim(current).empty()) {
                out.push_back(LogicalLine{logical.mode, logical.loc, trim(current)});
            }
            current.clear();
        }
    }
    if (!trim(current).empty()) {
        out.push_back(LogicalLine{logical.mode, logical.loc, trim(current)});
    }
    return out;
}

void appendZincBodyLine(std::vector<LogicalLine>& body, const LogicalLine& logical, const std::string& text) {
    LogicalLine line{SyntaxMode::Zinc, logical.loc, trim(text)};
    for (auto split : splitZincStatements(line)) {
        body.push_back(std::move(split));
    }
}

bool collectZincBraceBlock(const LogicalLine& firstLine,
                           size_t followingIndex,
                           size_t openBrace,
                           const std::vector<LogicalLine>& lines,
                           std::vector<LogicalLine>& body,
                           std::string& remainder,
                           SourceLocation& remainderLoc,
                           size_t& nextIndex) {
    int depth = 1;
    std::string current;
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;

    auto scan = [&](const LogicalLine& logical, size_t start) -> bool {
        for (size_t i = start; i < logical.text.size(); ++i) {
            char c = logical.text[i];
            if (!inString && !inRaw && i + 1 < logical.text.size() && c == '/' && logical.text[i + 1] == '/') {
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
            if (c == '{') {
                ++depth;
                current.push_back(c);
                continue;
            }
            if (c == '}') {
                --depth;
                if (depth == 0) {
                    if (!trim(current).empty()) {
                        appendZincBodyLine(body, logical, current);
                    }
                    remainder = logical.text.substr(i + 1);
                    remainderLoc = logical.loc;
                    return true;
                }
                current.push_back(c);
                continue;
            }
            current.push_back(c);
        }
        if (!trim(current).empty()) {
            appendZincBodyLine(body, logical, current);
        }
        current.clear();
        return false;
    };

    if (scan(firstLine, openBrace + 1)) {
        nextIndex = followingIndex;
        return true;
    }
    for (size_t i = followingIndex; i < lines.size(); ++i) {
        if (scan(lines[i], 0)) {
            nextIndex = i + 1;
            return true;
        }
    }
    return false;
}

class PruneRunner {
public:
    PruneRunner(const StaticIfSymbols& symbols, Diagnostics& diagnostics) : symbols_(symbols), diagnostics_(diagnostics) {
        context_.libraries = symbols.libraries;
        context_.boolConstants = symbols.boolConstants;
        context_.debugMode = symbols.debugMode;
    }

    StaticIfPruneResult run(const std::vector<LogicalLine>& lines) {
        size_t index = 0;
        StaticIfPruneResult result;
        result.lines = pruneRange(lines, index);
        result.stats = stats_;
        return result;
    }

private:
    struct Branch {
        bool hasCondition = false;
        bool conditionValue = false;
        SourceLocation loc;
        std::vector<LogicalLine> lines;
    };

    bool evalCondition(const std::string& expression, SourceLocation loc) {
        BoolEvalResult result = evalBoolExpr(expression, context_);
        if (!result.ok) {
            diagnostics_.error(loc, "invalid static if expression '" + expression + "': " + result.error);
            return false;
        }
        for (const auto& unknown : result.unknownIdentifiers) {
            diagnostics_.warning(loc, "unknown static if identifier '" + unknown + "' evaluated as false");
        }
        return result.value;
    }

    std::vector<LogicalLine> pruneRange(const std::vector<LogicalLine>& lines, size_t& index) {
        std::vector<LogicalLine> out;
        while (index < lines.size()) {
            std::string t = trim(lines[index].text);
            if (isCommentOrEmpty(t)) {
                out.push_back(lines[index++]);
                continue;
            }
            if (lines[index].mode == SyntaxMode::JassLike && startsStaticIf(t)) {
                auto selected = parseJassStaticIf(lines, index);
                out.insert(out.end(), selected.begin(), selected.end());
                continue;
            }
            if (lines[index].mode == SyntaxMode::Zinc && startsStaticIf(t)) {
                auto selected = parseZincStaticIf(lines, index);
                out.insert(out.end(), selected.begin(), selected.end());
                continue;
            }
            out.push_back(lines[index++]);
        }
        return out;
    }

    std::vector<LogicalLine> parseJassStaticIf(const std::vector<LogicalLine>& lines, size_t& index) {
        std::vector<Branch> branches;
        Branch current;
        current.hasCondition = true;
        current.loc = lines[index].loc;
        current.conditionValue = evalCondition(jassStaticIfCondition(lines[index].text, false), lines[index].loc);
        ++stats_.staticIfs;
        ++index;

        int nested = 0;
        bool closed = false;
        while (index < lines.size()) {
            std::string t = trim(lines[index].text);
            bool isJass = lines[index].mode == SyntaxMode::JassLike;
            if (isJass && nested == 0 && startsElseIf(t)) {
                branches.push_back(std::move(current));
                current = Branch{};
                current.hasCondition = true;
                current.loc = lines[index].loc;
                current.conditionValue = evalCondition(jassStaticIfCondition(lines[index].text, true), lines[index].loc);
                ++index;
                continue;
            }
            if (isJass && nested == 0 && startsElse(t)) {
                branches.push_back(std::move(current));
                current = Branch{};
                current.hasCondition = false;
                current.loc = lines[index].loc;
                ++index;
                continue;
            }
            if (isJass && nested == 0 && startsEndIf(t)) {
                branches.push_back(std::move(current));
                ++index;
                closed = true;
                break;
            }
            if (isJass && startsStaticIf(t)) {
                ++nested;
            } else if (isJass && startsEndIf(t) && nested > 0) {
                --nested;
            }
            current.lines.push_back(lines[index++]);
        }

        if (!closed) {
            diagnostics_.error(current.loc, "missing endif for static if");
            branches.push_back(std::move(current));
        }

        std::vector<LogicalLine> selected;
        bool selectedValue = false;
        bool found = false;
        size_t selectedIndex = branches.size();
        for (size_t i = 0; i < branches.size(); ++i) {
            if (!branches[i].hasCondition || branches[i].conditionValue) {
                size_t branchIndex = 0;
                selected = pruneRange(branches[i].lines, branchIndex);
                selectedValue = branches[i].hasCondition ? branches[i].conditionValue : true;
                selectedIndex = i;
                found = true;
                break;
            }
        }
        if (found && selectedValue) {
            ++stats_.resolvedTrue;
        } else {
            ++stats_.resolvedFalse;
        }
        for (size_t i = 0; i < branches.size(); ++i) {
            if (i != selectedIndex) {
                stats_.prunedLines += countNonEmptyLines(branches[i].lines);
            }
        }
        return selected;
    }

    std::vector<LogicalLine> parseZincStaticIf(const std::vector<LogicalLine>& lines, size_t& index) {
        const LogicalLine& start = lines[index];
        std::string text = start.text;
        size_t staticPos = text.find("static if");
        size_t openParen = text.find('(', staticPos == std::string::npos ? 0 : staticPos + 9);
        if (openParen == std::string::npos) {
            diagnostics_.error(start.loc, "missing '(' for Zinc static if");
            ++index;
            return {};
        }
        auto closeParen = findMatchingParen(text, openParen);
        if (!closeParen) {
            diagnostics_.error(start.loc, "missing ')' for Zinc static if");
            ++index;
            return {};
        }
        std::string condition = trim(std::string_view(text).substr(openParen + 1, *closeParen - openParen - 1));
        auto openBrace = findUnprotectedChar(text, '{', *closeParen + 1);
        if (!openBrace) {
            diagnostics_.error(start.loc, "missing '{' for Zinc static if");
            ++index;
            return {};
        }

        std::vector<LogicalLine> trueLines;
        std::vector<LogicalLine> falseLines;
        std::string remainder;
        SourceLocation remainderLoc;
        size_t nextIndex = index + 1;
        if (!collectZincBraceBlock(start, index + 1, *openBrace, lines, trueLines, remainder, remainderLoc, nextIndex)) {
            diagnostics_.error(start.loc, "missing '}' for Zinc static if");
            index = lines.size();
            return {};
        }

        std::string after = trim(remainder);
        if (startsWithWord(after, "else")) {
            LogicalLine elseLine{SyntaxMode::Zinc, remainderLoc, after};
            std::string elseText = trim(std::string_view(after).substr(4));
            elseLine.text = elseText;
            auto elseBrace = findUnprotectedChar(elseLine.text, '{');
            if (!elseBrace) {
                diagnostics_.error(remainderLoc, "missing '{' for Zinc static if else");
            } else {
                std::string elseRemainder;
                SourceLocation elseRemainderLoc;
                size_t afterElseIndex = nextIndex;
                if (!collectZincBraceBlock(elseLine, nextIndex, *elseBrace, lines, falseLines, elseRemainder, elseRemainderLoc, afterElseIndex)) {
                    diagnostics_.error(remainderLoc, "missing '}' for Zinc static if else");
                    nextIndex = lines.size();
                } else {
                    nextIndex = afterElseIndex;
                }
            }
        } else if (nextIndex < lines.size()) {
            std::string next = trim(lines[nextIndex].text);
            if (startsWithWord(next, "else")) {
                LogicalLine elseLine = lines[nextIndex];
                elseLine.text = trim(std::string_view(next).substr(4));
                auto elseBrace = findUnprotectedChar(elseLine.text, '{');
                if (!elseBrace) {
                    diagnostics_.error(lines[nextIndex].loc, "missing '{' for Zinc static if else");
                    ++nextIndex;
                } else {
                    std::string elseRemainder;
                    SourceLocation elseRemainderLoc;
                    size_t afterElseIndex = nextIndex + 1;
                    if (!collectZincBraceBlock(elseLine, nextIndex + 1, *elseBrace, lines, falseLines, elseRemainder, elseRemainderLoc, afterElseIndex)) {
                        diagnostics_.error(lines[nextIndex].loc, "missing '}' for Zinc static if else");
                        nextIndex = lines.size();
                    } else {
                        nextIndex = afterElseIndex;
                    }
                }
            }
        }

        ++stats_.staticIfs;
        bool value = evalCondition(condition, start.loc);
        ++(value ? stats_.resolvedTrue : stats_.resolvedFalse);
        stats_.prunedLines += countNonEmptyLines(value ? falseLines : trueLines);

        index = nextIndex;
        std::vector<LogicalLine>& chosen = value ? trueLines : falseLines;
        size_t chosenIndex = 0;
        return pruneRange(chosen, chosenIndex);
    }

    const StaticIfSymbols& symbols_;
    Diagnostics& diagnostics_;
    BoolEvalContext context_;
    StaticIfStats stats_;
};

} // namespace

StaticIfSymbols StaticIfSymbolCollector::collect(const std::vector<LogicalLine>& lines,
                                                 bool debugMode,
                                                 Diagnostics& diagnostics) const {
    StaticIfSymbols symbols;
    symbols.debugMode = debugMode;
    BoolEvalContext context;
    context.debugMode = debugMode;

    for (const auto& logical : lines) {
        std::string t = stripLineComment(logical.text);
        if (t.empty()) {
            continue;
        }
        std::string accessText = t;
        (void)parseAccessPrefix(accessText);
        if (startsWithWord(accessText, "library_once")) {
            std::string name = parseNameAfterWord(accessText, "library_once");
            if (!name.empty()) {
                symbols.libraries.insert(name);
            }
        } else if (startsWithWord(accessText, "library")) {
            std::string name = parseNameAfterWord(accessText, "library");
            if (!name.empty()) {
                symbols.libraries.insert(name);
            }
        }
    }

    context.libraries = symbols.libraries;
    for (const auto& logical : lines) {
        std::string t = stripTrailingSemicolon(stripLineComment(logical.text));
        if (t.empty()) {
            continue;
        }
        std::string accessText = t;
        (void)parseAccessPrefix(accessText);
        if (!startsWithWord(accessText, "constant")) {
            continue;
        }
        accessText = trim(std::string_view(accessText).substr(8));
        if (!startsWithWord(accessText, "boolean")) {
            continue;
        }
        accessText = trim(std::string_view(accessText).substr(7));
        size_t nameIdx = 0;
        std::string name = readIdent(accessText, nameIdx);
        std::string rest = trim(std::string_view(accessText).substr(nameIdx));
        size_t eq = rest.find('=');
        if (name.empty() || eq == std::string::npos) {
            continue;
        }
        std::string expr = trim(std::string_view(rest).substr(eq + 1));
        BoolEvalResult evaluated = evalBoolExpr(expr, context);
        if (!evaluated.ok) {
            diagnostics.warning(logical.loc, "constant boolean '" + name + "' has unsupported static if expression: " + evaluated.error);
            continue;
        }
        symbols.boolConstants[name] = evaluated.value;
        context.boolConstants[name] = evaluated.value;
    }

    return symbols;
}

StaticIfPruneResult StaticIfPruner::prune(const std::vector<LogicalLine>& lines,
                                          const StaticIfSymbols& symbols,
                                          Diagnostics& diagnostics) const {
    PruneRunner runner(symbols, diagnostics);
    return runner.run(lines);
}

} // namespace vjassc
