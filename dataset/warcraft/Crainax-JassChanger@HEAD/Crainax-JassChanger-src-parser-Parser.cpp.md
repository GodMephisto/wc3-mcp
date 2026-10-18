---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/parser/Parser.cpp"
ref: HEAD
---

#include "parser/Parser.h"

#include "util/PathUtil.h"

#include <algorithm>
#include <cctype>
#include <optional>
#include <regex>
#include <sstream>
#include <unordered_set>

namespace vjassc {
namespace {

std::string firstWord(const std::string& text) {
    std::istringstream in(trim(text));
    std::string word;
    in >> word;
    return word;
}

bool isCommentOrEmpty(const std::string& text) {
    std::string t = trim(text);
    return t.empty() || t.rfind("//", 0) == 0 || t.rfind("/*", 0) == 0 ||
           t.rfind("*", 0) == 0 || t.rfind("*/", 0) == 0;
}

std::string diagnosticSnippet(const std::string& text) {
    std::string snippet = trim(text);
    constexpr size_t maxLen = 96;
    if (snippet.size() > maxLen) {
        snippet = snippet.substr(0, maxLen) + "...";
    }
    return snippet;
}

bool isIdentStart(char c) {
    return std::isalpha(static_cast<unsigned char>(c)) || c == '_' || c == '$';
}

bool isIdentPart(char c) {
    return std::isalnum(static_cast<unsigned char>(c)) || c == '_' || c == '$';
}

std::string stripStringRawcodeAndComment(const std::string& text) {
    std::string out;
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    for (size_t i = 0; i < text.size(); ++i) {
        if (!inString && !inRaw && i + 1 < text.size() && text[i] == '/' && text[i + 1] == '/') {
            break;
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

bool isStandaloneNumericLiteral(const std::string& text) {
    std::string t = trim(stripStringRawcodeAndComment(text));
    if (!t.empty() && t.back() == ';') {
        t.pop_back();
        t = trim(t);
    }
    if (t.empty()) {
        return false;
    }
    bool sawDigit = false;
    bool sawDot = false;
    size_t i = 0;
    if (t[i] == '+' || t[i] == '-') {
        ++i;
    }
    for (; i < t.size(); ++i) {
        unsigned char c = static_cast<unsigned char>(t[i]);
        if (std::isdigit(c)) {
            sawDigit = true;
            continue;
        }
        if (t[i] == '.' && !sawDot) {
            sawDot = true;
            continue;
        }
        return false;
    }
    return sawDigit;
}

std::string previousWordBefore(const std::string& text, size_t pos) {
    while (pos > 0 && std::isspace(static_cast<unsigned char>(text[pos - 1]))) {
        --pos;
    }
    size_t end = pos;
    while (pos > 0 && isIdentPart(text[pos - 1])) {
        --pos;
    }
    return text.substr(pos, end - pos);
}

size_t countAnonymousFunctions(const std::string& text) {
    std::string code = stripStringRawcodeAndComment(text);
    size_t count = 0;
    size_t pos = 0;
    while ((pos = code.find("function", pos)) != std::string::npos) {
        char before = pos > 0 ? code[pos - 1] : '\0';
        char afterWord = pos + 8 < code.size() ? code[pos + 8] : '\0';
        if (!isIdentPart(before) && !isIdentPart(afterWord)) {
            size_t after = pos + 8;
            while (after < code.size() && std::isspace(static_cast<unsigned char>(code[after]))) {
                ++after;
            }
            if (after < code.size() && code[after] == '(' && previousWordBefore(code, pos) != "extends") {
                ++count;
            }
        }
        pos += 8;
    }
    return count;
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

std::vector<FieldDecl> parseFieldDecls(const LogicalLine& logical);
std::string parseAccessPrefix(std::string& text);

bool startsAnyTypeDecl(const std::string& t) {
    static const std::unordered_set<std::string> types = {
        "integer", "real", "boolean", "string", "code", "handle", "unit", "player", "timer",
        "trigger", "effect", "group", "force", "rect", "location", "item", "destructable",
        "widget", "image", "sound", "region", "hashtable", "boolexpr", "dialog", "button"
    };
    std::string word = firstWord(t);
    return types.contains(word);
}

bool isZincGlobalDeclLine(const LogicalLine& line) {
    std::string accessText = trim(line.text);
    (void)parseAccessPrefix(accessText);
    if (startsWithWord(accessText, "struct") || startsWithWord(accessText, "function") ||
        startsWithWord(accessText, "method") || startsWithWord(accessText, "static method") ||
        startsWithWord(accessText, "module") || startsWithWord(accessText, "optional module") ||
        startsWithWord(accessText, "implement") || startsWithWord(accessText, "static if") ||
        startsWithWord(accessText, "if") || startsWithWord(accessText, "else") ||
        startsWithWord(accessText, "for") || startsWithWord(accessText, "while") ||
        startsWithWord(accessText, "return") || startsWithWord(accessText, "call") ||
        startsWithWord(accessText, "set")) {
        return false;
    }
    return startsAnyTypeDecl(accessText) || startsWithWord(accessText, "constant") ||
           !parseFieldDecls(line).empty();
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

std::string parseNameAfterWord(const std::string& text, const std::string& word) {
    std::string t = trim(text);
    if (!startsWithWord(t, word)) {
        return {};
    }
    t = trim(std::string_view(t).substr(word.size()));
    std::istringstream in(t);
    std::string name;
    in >> name;
    return name;
}

std::string parseZincFunctionName(const std::string& text) {
    std::string t = trim(text);
    std::string accessText = t;
    (void)parseAccessPrefix(accessText);
    if (!startsWithWord(accessText, "function")) {
        return {};
    }
    accessText = trim(std::string_view(accessText).substr(8));
    size_t open = accessText.find('(');
    if (open == std::string::npos) {
        std::istringstream in(accessText);
        std::string name;
        in >> name;
        return name;
    }
    return trim(std::string_view(accessText).substr(0, open));
}

void parseRequirementsAndInitializer(const std::string& tail, Decl& decl) {
    static const std::vector<std::string> depWords = {"requires", "needs", "uses"};
    std::string clean = tail;
    size_t brace = clean.find('{');
    if (brace != std::string::npos) {
        clean = clean.substr(0, brace);
    }

    size_t initPos = clean.find("initializer");
    if (initPos != std::string::npos) {
        std::istringstream initIn(clean.substr(initPos + 11));
        initIn >> decl.initializer;
        clean = clean.substr(0, initPos);
    }

    for (const auto& depWord : depWords) {
        size_t pos = clean.find(depWord);
        if (pos == std::string::npos) {
            continue;
        }
        std::string list = clean.substr(pos + depWord.size());
        for (const auto& otherWord : depWords) {
            size_t next = list.find(otherWord);
            if (next != std::string::npos) {
                list = list.substr(0, next);
            }
        }
        for (auto part : splitCommaListRespectingQuotes(list)) {
            part = trim(part);
            if (part.empty()) {
                continue;
            }
            bool optional = false;
            if (startsWithWord(part, "optional")) {
                optional = true;
                part = trim(std::string_view(part).substr(8));
            }
            std::istringstream in(part);
            std::string name;
            in >> name;
            if (!name.empty()) {
                decl.requirements.push_back(Requirement{name, optional});
            }
        }
    }
}

int braceDelta(const std::string& text) {
    std::string trimmed = trim(text);
    if (trimmed.rfind("/*", 0) == 0 || trimmed.rfind("*", 0) == 0 || trimmed.rfind("*/", 0) == 0) {
        return 0;
    }
    int delta = 0;
    bool inString = false;
    bool inRaw = false;
    bool inBlockComment = false;
    bool escaped = false;
    for (size_t i = 0; i < text.size(); ++i) {
        char c = text[i];
        if (!inString && !inRaw && i + 1 < text.size() && c == '/' && text[i + 1] == '/') {
            break;
        }
        if (!inString && !inRaw && !inBlockComment && i + 1 < text.size() && c == '/' && text[i + 1] == '*') {
            inBlockComment = true;
            ++i;
            continue;
        }
        if (inBlockComment) {
            if (i + 1 < text.size() && c == '*' && text[i + 1] == '/') {
                inBlockComment = false;
                ++i;
            }
            continue;
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
        } else if (c == '{') {
            ++delta;
        } else if (c == '}') {
            --delta;
        }
    }
    return delta;
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

std::vector<std::string> splitCommaListRespectingQuotesAndBrackets(std::string_view text) {
    std::vector<std::string> parts;
    std::string current;
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    int parens = 0;
    int brackets = 0;
    int braces = 0;
    for (size_t i = 0; i < text.size(); ++i) {
        char c = text[i];
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
        } else if (c == ',' && parens == 0 && brackets == 0 && braces == 0) {
            parts.push_back(trim(current));
            current.clear();
            continue;
        }
        current.push_back(c);
    }
    if (!current.empty() || !parts.empty()) {
        parts.push_back(trim(current));
    }
    return parts;
}

std::vector<std::string> splitSemicolonListRespectingQuotesAndBrackets(std::string_view text) {
    std::vector<std::string> parts;
    std::string current;
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    int parens = 0;
    int brackets = 0;
    int braces = 0;
    for (size_t i = 0; i < text.size(); ++i) {
        char c = text[i];
        if (!inString && !inRaw && i + 1 < text.size() && c == '/' && text[i + 1] == '/') {
            current.append(text.substr(i));
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
        } else if (c == '{') {
            ++braces;
        } else if (c == '}' && braces > 0) {
            --braces;
        } else if (c == ';' && parens == 0 && brackets == 0 && braces == 0) {
            if (!trim(current).empty()) {
                parts.push_back(trim(current));
            }
            current.clear();
            continue;
        }
        current.push_back(c);
    }
    if (!trim(current).empty()) {
        parts.push_back(trim(current));
    }
    return parts;
}

std::optional<size_t> findTopLevelChar(std::string_view text, char needle) {
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    int parens = 0;
    int brackets = 0;
    int braces = 0;
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
        } else if (c == '{') {
            ++braces;
        } else if (c == '}' && braces > 0) {
            --braces;
        } else if (c == needle && parens == 0 && brackets == 0 && braces == 0) {
            return i;
        }
    }
    return std::nullopt;
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

TypeRef parseTypeRef(std::string text, SourceLocation loc) {
    text = trim(text);
    TypeRef type;
    type.loc = loc;
    if (startsWithWord(text, "constant")) {
        text = trim(std::string_view(text).substr(8));
    }
    size_t idx = 0;
    type.name = readIdent(text, idx);
    std::string rest = trim(std::string_view(text).substr(idx));
    if (startsWithWord(rest, "array")) {
        type.isArray = true;
    }
    return type;
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
        int value = 0;
        if (!dimText.empty()) {
            try {
                value = std::stoi(dimText);
            } catch (...) {
                value = 0;
            }
        }
        dims.push_back(value);
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

std::vector<ParamDecl> parseParamList(std::string text, SourceLocation loc) {
    std::vector<ParamDecl> params;
    text = trim(text);
    if (text.empty() || text == "nothing") {
        return params;
    }
    for (auto part : splitCommaListRespectingQuotesAndBrackets(text)) {
        part = trim(part);
        if (part.empty()) {
            continue;
        }
        size_t idx = 0;
        std::string typeName = readIdent(part, idx);
        std::string rest = trim(std::string_view(part).substr(idx));
        bool arrayType = false;
        if (startsWithWord(rest, "array")) {
            arrayType = true;
            rest = trim(std::string_view(rest).substr(5));
        }
        size_t nameIdx = 0;
        std::string name = readIdent(rest, nameIdx);
        if (!typeName.empty() && !name.empty()) {
            params.push_back(ParamDecl{TypeRef{typeName, loc, arrayType}, name, loc});
        }
    }
    return params;
}

std::vector<ParamDecl> parseFunctionTypeParamList(std::string text, SourceLocation loc) {
    std::vector<ParamDecl> params;
    text = trim(text);
    if (text.empty() || text == "nothing") {
        return params;
    }
    size_t index = 0;
    for (auto part : splitCommaListRespectingQuotesAndBrackets(text)) {
        part = trim(part);
        if (part.empty()) {
            continue;
        }
        size_t typeIdx = 0;
        std::string typeName = readIdent(part, typeIdx);
        if (!typeName.empty()) {
            params.push_back(ParamDecl{TypeRef{typeName, loc, false}, "arg" + std::to_string(index), loc});
            ++index;
        }
    }
    return params;
}

bool isFunctionTypeDecl(std::string text) {
    (void)parseAccessPrefix(text);
    return startsWithWord(text, "type") && text.find("extends function") != std::string::npos;
}

std::vector<FieldDecl> parseFieldDecls(const LogicalLine& logical) {
    std::string rawLine = stripLineComment(logical.text);
    std::vector<std::string> semicolonParts = splitSemicolonListRespectingQuotesAndBrackets(rawLine);
    if (semicolonParts.size() > 1) {
        std::vector<FieldDecl> all;
        for (const auto& part : semicolonParts) {
            LogicalLine partLine = logical;
            partLine.text = part;
            auto fields = parseFieldDecls(partLine);
            all.insert(all.end(), fields.begin(), fields.end());
        }
        return all;
    }
    std::string line = stripTrailingSemicolon(rawLine);
    if (line.empty()) {
        return {};
    }
    std::string access = parseAccessPrefix(line);
    if (startsWithWord(line, "method") || startsWithWord(line, "static method") ||
        startsWithWord(line, "function") || startsWithWord(line, "struct") ||
        startsWithWord(line, "module") || startsWithWord(line, "implement") ||
        startsWithWord(line, "static if") ||
        startsWithWord(line, "stub") || startsWithWord(line, "operator") ||
        startsWithWord(line, "super")) {
        return {};
    }

    FieldDecl base;
    base.access = access;
    base.loc = logical.loc;
    bool changed = true;
    while (changed) {
        changed = false;
        if (startsWithWord(line, "static")) {
            base.isStatic = true;
            line = trim(std::string_view(line).substr(6));
            changed = true;
        } else if (startsWithWord(line, "constant")) {
            base.isConstant = true;
            line = trim(std::string_view(line).substr(8));
            changed = true;
        } else if (startsWithWord(line, "readonly")) {
            base.isReadonly = true;
            line = trim(std::string_view(line).substr(8));
            changed = true;
        }
    }

    size_t idx = 0;
    std::string typeName = readIdent(line, idx);
    if (typeName.empty()) {
        return {};
    }
    std::string rest = trim(std::string_view(line).substr(idx));
    bool typeArray = false;
    if (startsWithWord(rest, "array")) {
        typeArray = true;
        rest = trim(std::string_view(rest).substr(5));
    }
    if (rest.empty()) {
        return {};
    }

    std::vector<FieldDecl> fields;
    for (auto part : splitCommaListRespectingQuotesAndBrackets(rest)) {
        part = trim(part);
        if (part.empty()) {
            continue;
        }
        std::string declarator = part;
        std::string initializer;
        if (auto eq = findTopLevelChar(part, '=')) {
            declarator = trim(std::string_view(part).substr(0, *eq));
            initializer = trim(std::string_view(part).substr(*eq + 1));
        }
        size_t nameIdx = 0;
        std::string name = readIdent(declarator, nameIdx);
        if (name.empty()) {
            continue;
        }
        std::string suffix = trim(std::string_view(declarator).substr(nameIdx));
        FieldDecl field = base;
        field.name = name;
        field.type = TypeRef{typeName, logical.loc, typeArray};
        field.isArray = typeArray;
        field.initializer = initializer;
        std::vector<int> dims = parseArrayDimensions(suffix);
        if (suffix == "[]") {
            field.isArray = true;
            field.type.isArray = true;
        } else if (!dims.empty()) {
            field.isArray = true;
            field.isFixedArray = true;
            field.arrayDimensions = std::move(dims);
            field.fixedArraySize = arrayFlatSize(field.arrayDimensions);
            field.type.isArray = true;
            field.type.arrayDimensions = field.arrayDimensions;
        }
        fields.push_back(std::move(field));
    }
    return fields;
}

} // namespace

const char* declKindName(DeclKind kind) {
    switch (kind) {
    case DeclKind::GlobalBlock:
        return "GlobalBlock";
    case DeclKind::Native:
        return "Native";
    case DeclKind::TypeDecl:
        return "TypeDecl";
    case DeclKind::Function:
        return "Function";
    case DeclKind::Library:
        return "Library";
    case DeclKind::Scope:
        return "Scope";
    case DeclKind::Struct:
        return "Struct";
    case DeclKind::Module:
        return "Module";
    case DeclKind::FunctionInterface:
        return "FunctionInterface";
    case DeclKind::Unsupported:
        return "Unsupported";
    }
    return "Unsupported";
}

bool Program::hasUnsupported() const {
    auto walk = [](const auto& self, const std::vector<Decl>& decls) -> bool {
        for (const auto& decl : decls) {
            if (decl.kind == DeclKind::Unsupported) {
                return true;
            }
            if (self(self, decl.children)) {
                return true;
            }
        }
        return false;
    };
    return walk(walk, decls);
}

Parser::Parser(Diagnostics& diagnostics) : diagnostics_(diagnostics) {}

Program Parser::parse(const std::vector<LogicalLine>& lines) {
    Program program;
    stats_ = ParserStats{};
    preScanUnsupported(lines);
    size_t index = 0;
    program.decls = parseJassRange(lines, index, {});
    program.stats = stats_;
    return program;
}

void Parser::preScanUnsupported(const std::vector<LogicalLine>& lines) {
    for (const auto& logical : lines) {
        std::string t = trim(logical.text);
        if (isCommentOrEmpty(t)) {
            continue;
        }
        std::string accessText = t;
        (void)parseAccessPrefix(accessText);
        if (startsWithWord(accessText, "optional")) {
            accessText = trim(std::string_view(accessText).substr(8));
        }
        stats_.lambdas += countAnonymousFunctions(accessText);
    }
}

std::vector<Decl> Parser::parseJassRange(const std::vector<LogicalLine>& lines, size_t& index, const std::string& terminator) {
    std::vector<Decl> decls;
    while (index < lines.size()) {
        const auto& logical = lines[index];
        std::string t = trim(logical.text);
        if (!terminator.empty() && startsWithWord(t, terminator)) {
            ++index;
            break;
        }
        if (isCommentOrEmpty(t)) {
            ++index;
            continue;
        }

        if (logical.mode == SyntaxMode::Zinc) {
            std::string accessText = t;
            (void)parseAccessPrefix(accessText);
            if (startsWithWord(accessText, "library") || startsWithWord(accessText, "scope")) {
                decls.push_back(parseZincLibrary(lines, index));
                continue;
            }
            if (startsWithWord(accessText, "function interface") || isFunctionTypeDecl(t)) {
                decls.push_back(parseFunctionInterface(logical));
                ++index;
                continue;
            }
            if (startsWithWord(accessText, "function")) {
                decls.push_back(parseZincFunction(lines, index));
                continue;
            }
            if (startsWithWord(accessText, "struct")) {
                decls.push_back(parseZincStruct(lines, index));
                continue;
            }
            if (startsWithWord(accessText, "module")) {
                decls.push_back(parseZincModule(lines, index));
                continue;
            }
            if (startsWithWord(accessText, "method") || startsWithWord(accessText, "static method")) {
                diagnostics_.error(logical.loc, "method declaration outside struct");
                ++index;
                continue;
            }
            diagnostics_.error(logical.loc, "unexpected Zinc declaration/member: " + diagnosticSnippet(logical.text));
            ++index;
            continue;
        }

        if (startsWithWord(t, "library") || startsWithWord(t, "library_once")) {
            decls.push_back(parseJassLibraryOrScope(lines, index, false));
        } else if (startsWithWord(t, "scope")) {
            decls.push_back(parseJassLibraryOrScope(lines, index, true));
        } else if (startsWithWord(t, "globals")) {
            decls.push_back(parseJassGlobalBlock(lines, index));
        } else {
            std::string accessText = t;
            std::string access = parseAccessPrefix(accessText);
            (void)access;
            if (startsWithWord(accessText, "function interface") || isFunctionTypeDecl(t)) {
                decls.push_back(parseFunctionInterface(logical));
                ++index;
            } else if (startsWithWord(accessText, "function")) {
                decls.push_back(parseJassFunction(lines, index));
            } else if (startsWithWord(t, "native")) {
                Decl decl;
                decl.kind = DeclKind::Native;
                decl.mode = logical.mode;
                decl.loc = logical.loc;
                decl.name = parseNameAfterWord(t, "native");
                decl.lines.push_back(logical.text);
                ++stats_.natives;
                decls.push_back(std::move(decl));
                ++index;
            } else if (startsWithWord(t, "type")) {
                Decl decl;
                decl.kind = DeclKind::TypeDecl;
                decl.mode = logical.mode;
                decl.loc = logical.loc;
                decl.name = parseNameAfterWord(t, "type");
                decl.lines.push_back(logical.text);
                ++stats_.types;
                decls.push_back(std::move(decl));
                ++index;
            } else if (startsWithWord(accessText, "struct")) {
                decls.push_back(parseJassStruct(lines, index));
            } else if (startsWithWord(accessText, "method") || startsWithWord(accessText, "static method")) {
                diagnostics_.error(logical.loc, "method declaration outside struct");
                ++index;
            } else if (startsWithWord(accessText, "module")) {
                decls.push_back(parseJassModule(lines, index));
            } else if (startsWithWord(accessText, "static if")) {
                Decl decl;
                decl.kind = DeclKind::Unsupported;
                decl.loc = logical.loc;
                decl.unsupportedFeature = "static if";
                decl.lines.push_back(logical.text);
                decls.push_back(std::move(decl));
                ++index;
            } else {
                if (isStandaloneNumericLiteral(logical.text)) {
                    diagnostics_.error(logical.loc, "unexpected top-level numeric literal: " + diagnosticSnippet(logical.text));
                }
                ++index;
            }
        }
    }
    return decls;
}

Decl Parser::parseFunctionInterface(const LogicalLine& line) {
    std::string text = stripTrailingSemicolon(stripLineComment(line.text));
    std::string accessText = text;
    std::string access = parseAccessPrefix(accessText);

    Decl decl;
    decl.kind = DeclKind::FunctionInterface;
    decl.mode = line.mode;
    decl.loc = line.loc;
    decl.access = access;
    decl.interfaceReturnType = TypeRef{"nothing", line.loc, false};
    decl.lines.push_back(line.text);

    if (startsWithWord(accessText, "function interface")) {
        accessText = trim(std::string_view(accessText).substr(18));
        size_t nameIdx = 0;
        decl.name = readIdent(accessText, nameIdx);
        std::string tail = trim(std::string_view(accessText).substr(nameIdx));
        auto takes = findWordPos(tail, "takes");
        auto returns = findWordPos(tail, "returns");
        if (takes && returns && *returns > *takes) {
            std::string params = trim(std::string_view(tail).substr(*takes + 5, *returns - (*takes + 5)));
            decl.interfaceParams = parseParamList(params, line.loc);
            decl.interfaceReturnType = parseTypeRef(std::string(std::string_view(tail).substr(*returns + 7)), line.loc);
        } else {
            diagnostics_.error(line.loc, "malformed function interface declaration");
        }
    } else if (startsWithWord(accessText, "type")) {
        accessText = trim(std::string_view(accessText).substr(4));
        size_t nameIdx = 0;
        decl.name = readIdent(accessText, nameIdx);
        std::string tail = trim(std::string_view(accessText).substr(nameIdx));
        size_t extends = tail.find("extends function");
        if (extends == std::string::npos) {
            diagnostics_.error(line.loc, "malformed function type declaration");
        } else {
            std::string func = trim(std::string_view(tail).substr(extends + 16));
            size_t open = func.find('(');
            size_t close = open == std::string::npos ? std::string::npos : func.find(')', open);
            if (open != std::string::npos && close != std::string::npos) {
                decl.interfaceParams = parseFunctionTypeParamList(func.substr(open + 1, close - open - 1), line.loc);
                size_t arrow = func.find("->", close);
                if (arrow != std::string::npos) {
                    std::string ret = trim(std::string_view(func).substr(arrow + 2));
                    if (!ret.empty()) {
                        decl.interfaceReturnType = parseTypeRef(ret, line.loc);
                    }
                }
            } else {
                diagnostics_.error(line.loc, "malformed function type declaration");
            }
        }
    }
    if (decl.interfaceReturnType.name.empty()) {
        decl.interfaceReturnType = TypeRef{"nothing", line.loc, false};
    }
    ++stats_.functionInterfaces;
    return decl;
}

Decl Parser::parseJassFunction(const std::vector<LogicalLine>& lines, size_t& index) {
    const auto& start = lines[index];
    std::string header = trim(start.text);
    std::string accessText = header;
    std::string access = parseAccessPrefix(accessText);

    Decl decl;
    decl.kind = DeclKind::Function;
    decl.mode = start.mode;
    decl.loc = start.loc;
    decl.access = access;
    decl.name = parseNameAfterWord(accessText, "function");
    decl.lines.push_back(start.text);
    ++stats_.functions;
    ++index;
    while (index < lines.size()) {
        decl.lines.push_back(lines[index].text);
        if (startsWithWord(trim(lines[index].text), "endfunction")) {
            ++index;
            break;
        }
        ++index;
    }
    return decl;
}

Decl Parser::parseJassGlobalBlock(const std::vector<LogicalLine>& lines, size_t& index) {
    Decl decl;
    decl.kind = DeclKind::GlobalBlock;
    decl.mode = lines[index].mode;
    decl.loc = lines[index].loc;
    ++stats_.globalsBlocks;
    ++index;
    while (index < lines.size()) {
        std::string t = trim(lines[index].text);
        if (startsWithWord(t, "endglobals")) {
            ++index;
            break;
        }
        decl.lines.push_back(lines[index].text);
        ++index;
    }
    return decl;
}

Decl Parser::parseJassLibraryOrScope(const std::vector<LogicalLine>& lines, size_t& index, bool isScope) {
    const auto& start = lines[index];
    std::string t = trim(start.text);
    Decl decl;
    decl.kind = isScope ? DeclKind::Scope : DeclKind::Library;
    decl.mode = start.mode;
    decl.loc = start.loc;
    decl.libraryOnce = startsWithWord(t, "library_once");
    decl.name = parseNameAfterWord(t, isScope ? "scope" : (decl.libraryOnce ? "library_once" : "library"));
    size_t namePos = t.find(decl.name);
    if (namePos != std::string::npos) {
        parseRequirementsAndInitializer(t.substr(namePos + decl.name.size()), decl);
    }
    if (isScope) {
        ++stats_.scopes;
    } else if (decl.libraryOnce) {
        ++stats_.libraryOnce;
    } else {
        ++stats_.libraries;
    }
    ++index;
    decl.children = parseJassRange(lines, index, isScope ? "endscope" : "endlibrary");
    return decl;
}

Decl Parser::parseJassStruct(const std::vector<LogicalLine>& lines, size_t& index) {
    const auto& start = lines[index];
    std::string header = trim(start.text);
    std::string accessText = header;
    std::string access = parseAccessPrefix(accessText);
    if (!startsWithWord(accessText, "struct")) {
        diagnostics_.error(start.loc, "expected struct declaration");
        ++index;
        return Decl{};
    }
    accessText = trim(std::string_view(accessText).substr(6));
    size_t idx = 0;
    std::string name = readIdent(accessText, idx);
    std::string tail = trim(std::string_view(accessText).substr(idx));

    Decl decl;
    decl.kind = DeclKind::Struct;
    decl.mode = SyntaxMode::JassLike;
    decl.loc = start.loc;
    decl.access = access;
    decl.name = name;
    decl.lines.push_back(start.text);
    if (startsWithWord(tail, "extends")) {
        tail = trim(std::string_view(tail).substr(7));
        size_t extIdx = 0;
        decl.extendsName = readIdent(tail, extIdx);
        decl.isArrayStruct = decl.extendsName == "array";
        if (!decl.extendsName.empty() && decl.extendsName != "array") {
            diagnostics_.unsupported(start.loc, "struct extends " + decl.extendsName);
            Decl child;
            child.kind = DeclKind::Unsupported;
            child.mode = SyntaxMode::JassLike;
            child.loc = start.loc;
            child.unsupportedFeature = "struct extends";
            child.lines.push_back(start.text);
            decl.children.push_back(std::move(child));
        }
    }
    ++index;

    bool closed = false;
    while (index < lines.size()) {
        std::string t = trim(lines[index].text);
        if (isCommentOrEmpty(t)) {
            ++index;
            continue;
        }
        if (startsWithWord(t, "endstruct")) {
            decl.lines.push_back(lines[index].text);
            ++index;
            closed = true;
            break;
        }

        std::string memberText = t;
        (void)parseAccessPrefix(memberText);
        if (startsWithWord(memberText, "static method") || startsWithWord(memberText, "method")) {
            decl.methods.push_back(parseJassMethod(lines, index));
            continue;
        }
        if (startsWithWord(memberText, "function")) {
            diagnostics_.error(lines[index].loc, "function declaration inside struct is not supported");
            decl.children.push_back(parseUnsupportedBlock(lines, index, "function in struct", "endfunction"));
            continue;
        }
        if (startsWithWord(memberText, "module") || startsWithWord(memberText, "implement")) {
            decl.moduleUses.push_back(parseJassModuleUse(lines[index]));
            ++index;
            continue;
        }
        if (startsWithWord(memberText, "static if")) {
            decl.children.push_back(parseUnsupportedBlock(lines, index, "static if", "endif"));
            continue;
        }
        if (startsWithWord(memberText, "stub") || startsWithWord(memberText, "operator") ||
            startsWithWord(memberText, "super") || startsWithWord(memberText, "interface") ||
            startsWithWord(memberText, "delegate")) {
            Decl child;
            child.kind = DeclKind::Unsupported;
            child.mode = SyntaxMode::JassLike;
            child.loc = lines[index].loc;
            child.unsupportedFeature = firstWord(memberText);
            child.lines.push_back(lines[index].text);
            decl.children.push_back(std::move(child));
            ++index;
            continue;
        }
        auto fields = parseFieldDecls(lines[index]);
        decl.fields.insert(decl.fields.end(), fields.begin(), fields.end());
        ++index;
    }
    if (!closed) {
        diagnostics_.error(start.loc, "missing endstruct for struct '" + decl.name + "'");
    }
    return decl;
}

ModuleUseDecl Parser::parseJassModuleUse(const LogicalLine& line) {
    std::string text = stripTrailingSemicolon(stripLineComment(line.text));
    std::string accessText = text;
    (void)parseAccessPrefix(accessText);
    ModuleUseDecl use;
    use.loc = line.loc;
    use.mode = SyntaxMode::JassLike;
    if (startsWithWord(accessText, "implement")) {
        accessText = trim(std::string_view(accessText).substr(9));
        if (startsWithWord(accessText, "optional")) {
            use.optional = true;
            accessText = trim(std::string_view(accessText).substr(8));
        }
    } else {
        if (startsWithWord(accessText, "optional")) {
            use.optional = true;
            accessText = trim(std::string_view(accessText).substr(8));
        }
        if (startsWithWord(accessText, "module")) {
            accessText = trim(std::string_view(accessText).substr(6));
        }
    }
    size_t nameIdx = 0;
    use.name = readIdent(accessText, nameIdx);
    if (use.name.empty()) {
        diagnostics_.error(line.loc, "expected module name");
    }
    ++stats_.moduleUses;
    return use;
}

Decl Parser::parseJassModule(const std::vector<LogicalLine>& lines, size_t& index) {
    const auto& start = lines[index];
    std::string header = trim(start.text);
    std::string accessText = header;
    std::string access = parseAccessPrefix(accessText);
    if (!startsWithWord(accessText, "module")) {
        diagnostics_.error(start.loc, "expected module declaration");
        ++index;
        return Decl{};
    }
    accessText = trim(std::string_view(accessText).substr(6));
    size_t nameIdx = 0;
    std::string name = readIdent(accessText, nameIdx);

    Decl decl;
    decl.kind = DeclKind::Module;
    decl.mode = SyntaxMode::JassLike;
    decl.loc = start.loc;
    decl.access = access;
    decl.name = name;
    decl.lines.push_back(start.text);
    ++stats_.modules;
    ++index;

    bool closed = false;
    while (index < lines.size()) {
        std::string t = trim(lines[index].text);
        if (isCommentOrEmpty(t)) {
            ++index;
            continue;
        }
        if (startsWithWord(t, "endmodule")) {
            decl.lines.push_back(lines[index].text);
            ++index;
            closed = true;
            break;
        }

        std::string memberText = t;
        (void)parseAccessPrefix(memberText);
        if (startsWithWord(memberText, "static method") || startsWithWord(memberText, "method")) {
            decl.methods.push_back(parseJassMethod(lines, index));
            continue;
        }
        if (startsWithWord(memberText, "function")) {
            diagnostics_.error(lines[index].loc, "function declaration inside module is not supported");
            decl.children.push_back(parseUnsupportedBlock(lines, index, "function in module", "endfunction"));
            continue;
        }
        if (startsWithWord(memberText, "module") || startsWithWord(memberText, "implement")) {
            decl.moduleUses.push_back(parseJassModuleUse(lines[index]));
            ++index;
            continue;
        }
        if (startsWithWord(memberText, "stub") || startsWithWord(memberText, "operator") ||
            startsWithWord(memberText, "super") || startsWithWord(memberText, "interface") ||
            startsWithWord(memberText, "delegate")) {
            Decl child;
            child.kind = DeclKind::Unsupported;
            child.mode = SyntaxMode::JassLike;
            child.loc = lines[index].loc;
            child.unsupportedFeature = firstWord(memberText);
            child.lines.push_back(lines[index].text);
            decl.children.push_back(std::move(child));
            ++index;
            continue;
        }
        auto fields = parseFieldDecls(lines[index]);
        decl.fields.insert(decl.fields.end(), fields.begin(), fields.end());
        ++index;
    }
    if (!closed) {
        diagnostics_.error(start.loc, "missing endmodule for module '" + decl.name + "'");
    }
    return decl;
}

MethodDecl Parser::parseJassMethod(const std::vector<LogicalLine>& lines, size_t& index) {
    const auto& start = lines[index];
    std::string header = trim(start.text);
    std::string accessText = header;
    std::string access = parseAccessPrefix(accessText);
    MethodDecl method;
    method.mode = SyntaxMode::JassLike;
    method.loc = start.loc;
    method.access = access;
    if (startsWithWord(accessText, "static")) {
        method.isStatic = true;
        accessText = trim(std::string_view(accessText).substr(6));
    }
    if (!startsWithWord(accessText, "method")) {
        diagnostics_.error(start.loc, "expected method declaration");
        ++index;
        return method;
    }
    accessText = trim(std::string_view(accessText).substr(6));
    if (startsWithWord(accessText, "operator")) {
        method.isOperator = true;
    }
    size_t nameIdx = 0;
    method.name = readIdent(accessText, nameIdx);
    std::string tail = trim(std::string_view(accessText).substr(nameIdx));
    auto takes = findWordPos(tail, "takes");
    auto returns = findWordPos(tail, "returns");
    if (takes && returns && *returns > *takes) {
        std::string params = trim(std::string_view(tail).substr(*takes + 5, *returns - (*takes + 5)));
        method.params = parseParamList(params, start.loc);
        method.returnType = parseTypeRef(std::string(std::string_view(tail).substr(*returns + 7)), start.loc);
    } else {
        method.returnType = TypeRef{"nothing", start.loc, false};
    }
    if (method.returnType.name.empty()) {
        method.returnType = TypeRef{"nothing", start.loc, false};
    }
    method.isOnDestroy = method.name == "onDestroy";
    method.isOnInit = method.name == "onInit";
    ++index;
    bool closed = false;
    while (index < lines.size()) {
        std::string t = trim(lines[index].text);
        if (startsWithWord(t, "endmethod")) {
            ++index;
            closed = true;
            break;
        }
        method.bodyLines.push_back(lines[index]);
        ++index;
    }
    if (!closed) {
        diagnostics_.error(start.loc, "missing endmethod for method '" + method.name + "'");
    }
    return method;
}

Decl Parser::parseUnsupportedBlock(const std::vector<LogicalLine>& lines, size_t& index, const std::string& feature, const std::string& endWord) {
    Decl decl;
    decl.kind = DeclKind::Unsupported;
    decl.mode = lines[index].mode;
    decl.loc = lines[index].loc;
    decl.unsupportedFeature = feature;
    while (index < lines.size()) {
        std::string t = trim(lines[index].text);
        decl.lines.push_back(lines[index].text);
        ++index;
        if (startsWithWord(t, endWord)) {
            break;
        }
    }
    return decl;
}

Decl Parser::parseZincLibrary(const std::vector<LogicalLine>& lines, size_t& index) {
    const auto& start = lines[index];
    std::string header = trim(start.text);
    bool isScope = startsWithWord(header, "scope");
    Decl decl;
    decl.kind = isScope ? DeclKind::Scope : DeclKind::Library;
    decl.mode = SyntaxMode::Zinc;
    decl.loc = start.loc;
    decl.name = parseNameAfterWord(header, isScope ? "scope" : "library");
    size_t namePos = header.find(decl.name);
    if (namePos != std::string::npos) {
        parseRequirementsAndInitializer(header.substr(namePos + decl.name.size()), decl);
    }
    if (isScope) {
        ++stats_.scopes;
    } else {
        ++stats_.libraries;
    }

    std::vector<LogicalLine> body;
    int depth = 0;
    bool seenOpen = false;
    while (index < lines.size()) {
        std::string text = lines[index].text;
        int delta = braceDelta(text);
        if (!seenOpen) {
            size_t brace = text.find('{');
            if (brace != std::string::npos) {
                seenOpen = true;
                std::string after = text.substr(brace + 1);
                if (!trim(after).empty()) {
                    body.push_back(LogicalLine{SyntaxMode::Zinc, lines[index].loc, after});
                }
            }
        } else {
            if (depth + delta <= 0) {
                size_t close = text.rfind('}');
                if (close != std::string::npos) {
                    std::string before = text.substr(0, close);
                    if (!trim(before).empty()) {
                        body.push_back(LogicalLine{SyntaxMode::Zinc, lines[index].loc, before});
                    }
                }
                ++index;
                break;
            }
            body.push_back(lines[index]);
        }
        depth += delta;
        ++index;
    }
    decl.children = parseZincMembers(body);
    return decl;
}

std::vector<Decl> Parser::parseZincMembers(const std::vector<LogicalLine>& lines) {
    std::vector<Decl> decls;
    size_t index = 0;
    while (index < lines.size()) {
        std::string t = trim(lines[index].text);
        if (isCommentOrEmpty(t) || t == "}") {
            ++index;
            continue;
        }
        std::string accessText = t;
        std::string access = parseAccessPrefix(accessText);
        if (!access.empty() && !accessText.empty() && accessText.front() == '{') {
            std::vector<LogicalLine> body;
            bool seenOpen = false;
            int depth = 0;
            while (index < lines.size()) {
                const auto& logical = lines[index];
                std::string text = logical.text;
                if (!seenOpen) {
                    size_t brace = text.find('{');
                    if (brace != std::string::npos) {
                        seenOpen = true;
                        depth = 1;
                        std::string after = text.substr(brace + 1);
                        int afterDelta = braceDelta(after);
                        if (depth + afterDelta <= 0) {
                            size_t close = after.rfind('}');
                            std::string before = close == std::string::npos ? after : after.substr(0, close);
                            if (!trim(before).empty()) {
                                body.push_back(LogicalLine{SyntaxMode::Zinc, logical.loc, before});
                            }
                            ++index;
                            break;
                        }
                        if (!trim(after).empty()) {
                            body.push_back(LogicalLine{SyntaxMode::Zinc, logical.loc, after});
                        }
                        depth += afterDelta;
                    }
                    ++index;
                    continue;
                }

                int delta = braceDelta(text);
                if (depth + delta <= 0) {
                    size_t close = text.rfind('}');
                    std::string before = close == std::string::npos ? text : text.substr(0, close);
                    if (!trim(before).empty()) {
                        body.push_back(LogicalLine{SyntaxMode::Zinc, logical.loc, before});
                    }
                    ++index;
                    break;
                }
                body.push_back(logical);
                depth += delta;
                ++index;
            }

            auto nested = parseZincMembers(body);
            for (auto& child : nested) {
                if (child.access.empty()) {
                    child.access = access;
                }
                if (child.kind == DeclKind::GlobalBlock) {
                    child.access = access;
                }
                decls.push_back(std::move(child));
            }
            continue;
        }
        if (startsWithWord(accessText, "function interface") || isFunctionTypeDecl(t)) {
            Decl decl = parseFunctionInterface(lines[index]);
            decl.mode = SyntaxMode::Zinc;
            decl.access = access;
            decls.push_back(std::move(decl));
            ++index;
        } else if (startsWithWord(accessText, "function")) {
            Decl fn = parseZincFunction(lines, index);
            fn.access = access;
            decls.push_back(std::move(fn));
        } else if (startsWithWord(accessText, "struct")) {
            decls.push_back(parseZincStruct(lines, index));
        } else if (startsWithWord(accessText, "module")) {
            decls.push_back(parseZincModule(lines, index));
        } else if (startsWithWord(accessText, "method") || startsWithWord(accessText, "static method")) {
            diagnostics_.error(lines[index].loc, "method declaration outside struct");
            ++index;
        } else if (startsWithWord(accessText, "optional module")) {
            diagnostics_.error(lines[index].loc, "module use outside struct/module");
            ++index;
        } else if (startsWithWord(accessText, "static if")) {
            decls.push_back(parseZincUnsupportedBlock(lines, index, "static if"));
        } else if (isZincGlobalDeclLine(lines[index])) {
            Decl globals;
            globals.kind = DeclKind::GlobalBlock;
            globals.mode = SyntaxMode::Zinc;
            globals.loc = lines[index].loc;
            while (index < lines.size()) {
                if (!isZincGlobalDeclLine(lines[index])) {
                    break;
                }
                globals.lines.push_back(stripTrailingSemicolon(lines[index].text));
                ++index;
            }
            ++stats_.globalsBlocks;
            decls.push_back(std::move(globals));
        } else {
            diagnostics_.error(lines[index].loc, "unexpected Zinc declaration/member: " + diagnosticSnippet(lines[index].text));
            ++index;
        }
    }
    return decls;
}

Decl Parser::parseZincFunction(const std::vector<LogicalLine>& lines, size_t& index) {
    const auto& start = lines[index];
    Decl decl;
    decl.kind = DeclKind::Function;
    decl.mode = SyntaxMode::Zinc;
    decl.loc = start.loc;
    std::string header = trim(start.text);
    std::string accessText = header;
    decl.access = parseAccessPrefix(accessText);
    decl.name = parseZincFunctionName(header);
    decl.lines.push_back(header);
    ++stats_.functions;

    size_t openBrace = header.find('{');
    if (openBrace != std::string::npos) {
        std::string after = header.substr(openBrace + 1);
        int afterDelta = braceDelta(after);
        if (1 + afterDelta <= 0) {
            size_t closeBrace = after.rfind('}');
            std::string before = closeBrace == std::string::npos ? after : after.substr(0, closeBrace);
            if (!trim(before).empty()) {
                decl.lines.push_back(before);
            }
            ++index;
            return decl;
        }
        if (!trim(after).empty()) {
            decl.lines.push_back(after);
        }
    }

    int depth = braceDelta(header);
    ++index;
    while (index < lines.size()) {
        std::string line = lines[index].text;
        int delta = braceDelta(line);
        if (depth + delta <= 0) {
            size_t close = line.rfind('}');
            if (close != std::string::npos) {
                std::string before = line.substr(0, close);
                if (!trim(before).empty()) {
                    decl.lines.push_back(before);
                }
            }
            ++index;
            break;
        }
        decl.lines.push_back(line);
        depth += delta;
        ++index;
    }
    return decl;
}

Decl Parser::parseZincStruct(const std::vector<LogicalLine>& lines, size_t& index) {
    const auto& start = lines[index];
    std::string header = trim(start.text);
    std::string accessText = header;
    std::string access = parseAccessPrefix(accessText);
    if (!startsWithWord(accessText, "struct")) {
        diagnostics_.error(start.loc, "expected struct declaration");
        ++index;
        return Decl{};
    }
    accessText = trim(std::string_view(accessText).substr(6));
    size_t nameIdx = 0;
    std::string name = readIdent(accessText, nameIdx);
    std::string tail = trim(std::string_view(accessText).substr(nameIdx));

    Decl decl;
    decl.kind = DeclKind::Struct;
    decl.mode = SyntaxMode::Zinc;
    decl.loc = start.loc;
    decl.access = access;
    decl.name = name;
    decl.lines.push_back(start.text);
    if (tail.rfind("[]", 0) == 0) {
        decl.isArrayStruct = true;
        tail = trim(std::string_view(tail).substr(2));
    }
    if (startsWithWord(tail, "extends")) {
        tail = trim(std::string_view(tail).substr(7));
        size_t extIdx = 0;
        decl.extendsName = readIdent(tail, extIdx);
        decl.isArrayStruct = decl.extendsName == "array";
        if (!decl.extendsName.empty() && decl.extendsName != "array") {
            diagnostics_.unsupported(start.loc, "struct extends " + decl.extendsName);
            Decl child;
            child.kind = DeclKind::Unsupported;
            child.mode = SyntaxMode::Zinc;
            child.loc = start.loc;
            child.unsupportedFeature = "struct extends";
            child.lines.push_back(start.text);
            decl.children.push_back(std::move(child));
        }
    }

    std::vector<LogicalLine> body;
    bool seenOpen = false;
    int depth = 0;
    while (index < lines.size()) {
        const auto& logical = lines[index];
        std::string text = logical.text;
        int delta = braceDelta(text);
        if (!seenOpen) {
            size_t brace = text.find('{');
            if (brace != std::string::npos) {
                seenOpen = true;
                std::string after = text.substr(brace + 1);
                int afterDelta = braceDelta(after);
                depth = 1 + afterDelta;
                if (depth <= 0) {
                    size_t close = after.rfind('}');
                    std::string before = close == std::string::npos ? after : after.substr(0, close);
                    if (!trim(before).empty()) {
                        body.push_back(LogicalLine{SyntaxMode::Zinc, logical.loc, before});
                    }
                    ++index;
                    break;
                }
                if (!trim(after).empty()) {
                    body.push_back(LogicalLine{SyntaxMode::Zinc, logical.loc, after});
                }
            }
            ++index;
            continue;
        }
        if (depth + delta <= 0) {
            size_t close = text.rfind('}');
            std::string before = close == std::string::npos ? text : text.substr(0, close);
            if (!trim(before).empty()) {
                body.push_back(LogicalLine{SyntaxMode::Zinc, logical.loc, before});
            }
            ++index;
            break;
        }
        body.push_back(logical);
        depth += delta;
        ++index;
    }
    if (!seenOpen) {
        diagnostics_.error(start.loc, "missing '{' for struct '" + decl.name + "'");
    }

    size_t bodyIndex = 0;
    while (bodyIndex < body.size()) {
        std::string t = trim(body[bodyIndex].text);
        if (isCommentOrEmpty(t) || t == "}") {
            ++bodyIndex;
            continue;
        }
        std::string memberText = t;
        (void)parseAccessPrefix(memberText);
        if (!memberText.empty() && memberText.front() == '{') {
            ++bodyIndex;
            continue;
        }
        if (startsWithWord(memberText, "static method") || startsWithWord(memberText, "method")) {
            decl.methods.push_back(parseZincMethod(body, bodyIndex));
            continue;
        }
        if (startsWithWord(memberText, "module") || startsWithWord(memberText, "optional module") ||
            startsWithWord(memberText, "implement")) {
            decl.moduleUses.push_back(parseZincModuleUse(body[bodyIndex]));
            ++bodyIndex;
            continue;
        }
        if (startsWithWord(memberText, "static if")) {
            decl.children.push_back(parseZincUnsupportedBlock(body, bodyIndex, "static if"));
            continue;
        }
        if (startsWithWord(memberText, "stub") || startsWithWord(memberText, "operator") ||
            startsWithWord(memberText, "super") || startsWithWord(memberText, "interface") ||
            startsWithWord(memberText, "delegate")) {
            Decl child;
            child.kind = DeclKind::Unsupported;
            child.mode = SyntaxMode::Zinc;
            child.loc = body[bodyIndex].loc;
            child.unsupportedFeature = firstWord(memberText);
            child.lines.push_back(body[bodyIndex].text);
            decl.children.push_back(std::move(child));
            ++bodyIndex;
            continue;
        }
        auto fields = parseFieldDecls(body[bodyIndex]);
        if (fields.empty()) {
            diagnostics_.error(body[bodyIndex].loc, "unexpected Zinc module member: " + diagnosticSnippet(body[bodyIndex].text));
            ++bodyIndex;
            continue;
        }
        decl.fields.insert(decl.fields.end(), fields.begin(), fields.end());
        ++bodyIndex;
    }

    return decl;
}

ModuleUseDecl Parser::parseZincModuleUse(const LogicalLine& line) {
    std::string text = stripTrailingSemicolon(stripLineComment(line.text));
    std::string accessText = text;
    (void)parseAccessPrefix(accessText);
    ModuleUseDecl use;
    use.loc = line.loc;
    use.mode = SyntaxMode::Zinc;
    if (startsWithWord(accessText, "optional")) {
        use.optional = true;
        accessText = trim(std::string_view(accessText).substr(8));
    }
    if (startsWithWord(accessText, "module")) {
        accessText = trim(std::string_view(accessText).substr(6));
    } else if (startsWithWord(accessText, "implement")) {
        accessText = trim(std::string_view(accessText).substr(9));
        if (startsWithWord(accessText, "optional")) {
            use.optional = true;
            accessText = trim(std::string_view(accessText).substr(8));
        }
    }
    size_t nameIdx = 0;
    use.name = readIdent(accessText, nameIdx);
    if (use.name.empty()) {
        diagnostics_.error(line.loc, "expected module name");
    }
    ++stats_.moduleUses;
    return use;
}

Decl Parser::parseZincModule(const std::vector<LogicalLine>& lines, size_t& index) {
    const auto& start = lines[index];
    std::string header = trim(start.text);
    std::string accessText = header;
    std::string access = parseAccessPrefix(accessText);
    if (!startsWithWord(accessText, "module")) {
        diagnostics_.error(start.loc, "expected module declaration");
        ++index;
        return Decl{};
    }
    accessText = trim(std::string_view(accessText).substr(6));
    size_t nameIdx = 0;
    std::string name = readIdent(accessText, nameIdx);

    Decl decl;
    decl.kind = DeclKind::Module;
    decl.mode = SyntaxMode::Zinc;
    decl.loc = start.loc;
    decl.access = access;
    decl.name = name;
    decl.lines.push_back(start.text);
    ++stats_.modules;

    std::vector<LogicalLine> body;
    bool seenOpen = false;
    int depth = 0;
    while (index < lines.size()) {
        const auto& logical = lines[index];
        std::string text = logical.text;
        int delta = braceDelta(text);
        if (!seenOpen) {
            size_t brace = text.find('{');
            if (brace != std::string::npos) {
                seenOpen = true;
                std::string after = text.substr(brace + 1);
                int afterDelta = braceDelta(after);
                depth = 1 + afterDelta;
                if (depth <= 0) {
                    size_t close = after.rfind('}');
                    std::string before = close == std::string::npos ? after : after.substr(0, close);
                    if (!trim(before).empty()) {
                        body.push_back(LogicalLine{SyntaxMode::Zinc, logical.loc, before});
                    }
                    ++index;
                    break;
                }
                if (!trim(after).empty()) {
                    body.push_back(LogicalLine{SyntaxMode::Zinc, logical.loc, after});
                }
            }
            ++index;
            continue;
        }
        if (depth + delta <= 0) {
            size_t close = text.rfind('}');
            std::string before = close == std::string::npos ? text : text.substr(0, close);
            if (!trim(before).empty()) {
                body.push_back(LogicalLine{SyntaxMode::Zinc, logical.loc, before});
            }
            ++index;
            break;
        }
        body.push_back(logical);
        depth += delta;
        ++index;
    }
    if (!seenOpen) {
        diagnostics_.error(start.loc, "missing '{' for module '" + decl.name + "'");
    }

    size_t bodyIndex = 0;
    while (bodyIndex < body.size()) {
        std::string t = trim(body[bodyIndex].text);
        if (isCommentOrEmpty(t) || t == "}") {
            ++bodyIndex;
            continue;
        }
        std::string memberText = t;
        (void)parseAccessPrefix(memberText);
        if (!memberText.empty() && memberText.front() == '{') {
            ++bodyIndex;
            continue;
        }
        if (startsWithWord(memberText, "static method") || startsWithWord(memberText, "method")) {
            decl.methods.push_back(parseZincMethod(body, bodyIndex));
            continue;
        }
        if (startsWithWord(memberText, "module") || startsWithWord(memberText, "optional module") ||
            startsWithWord(memberText, "implement")) {
            decl.moduleUses.push_back(parseZincModuleUse(body[bodyIndex]));
            ++bodyIndex;
            continue;
        }
        if (startsWithWord(memberText, "stub") || startsWithWord(memberText, "operator") ||
            startsWithWord(memberText, "super") || startsWithWord(memberText, "interface") ||
            startsWithWord(memberText, "delegate")) {
            Decl child;
            child.kind = DeclKind::Unsupported;
            child.mode = SyntaxMode::Zinc;
            child.loc = body[bodyIndex].loc;
            child.unsupportedFeature = firstWord(memberText);
            child.lines.push_back(body[bodyIndex].text);
            decl.children.push_back(std::move(child));
            ++bodyIndex;
            continue;
        }
        auto fields = parseFieldDecls(body[bodyIndex]);
        if (fields.empty()) {
            diagnostics_.error(body[bodyIndex].loc, "unexpected Zinc struct member: " + diagnosticSnippet(body[bodyIndex].text));
            ++bodyIndex;
            continue;
        }
        decl.fields.insert(decl.fields.end(), fields.begin(), fields.end());
        ++bodyIndex;
    }

    return decl;
}

MethodDecl Parser::parseZincMethod(const std::vector<LogicalLine>& lines, size_t& index) {
    const auto& start = lines[index];
    std::string header = trim(start.text);
    std::string accessText = header;
    std::string access = parseAccessPrefix(accessText);
    MethodDecl method;
    method.mode = SyntaxMode::Zinc;
    method.loc = start.loc;
    method.access = access;
    if (startsWithWord(accessText, "static")) {
        method.isStatic = true;
        accessText = trim(std::string_view(accessText).substr(6));
    }
    if (!startsWithWord(accessText, "method")) {
        diagnostics_.error(start.loc, "expected method declaration");
        ++index;
        return method;
    }
    accessText = trim(std::string_view(accessText).substr(6));
    if (startsWithWord(accessText, "operator")) {
        method.isOperator = true;
    }
    size_t open = accessText.find('(');
    size_t close = open == std::string::npos ? std::string::npos : accessText.find(')', open);
    std::string namePart = open == std::string::npos ? accessText : std::string(accessText.substr(0, open));
    namePart = trim(namePart);
    size_t nameIdx = 0;
    method.name = readIdent(namePart, nameIdx);
    if (open != std::string::npos && close != std::string::npos) {
        method.params = parseParamList(accessText.substr(open + 1, close - open - 1), start.loc);
    }
    method.returnType = TypeRef{"nothing", start.loc, false};
    size_t arrow = close == std::string::npos ? std::string::npos : accessText.find("->", close);
    if (arrow != std::string::npos) {
        std::string after = trim(std::string_view(accessText).substr(arrow + 2));
        size_t brace = after.find('{');
        if (brace != std::string::npos) {
            after = trim(std::string_view(after).substr(0, brace));
        }
        if (!after.empty()) {
            method.returnType = parseTypeRef(after, start.loc);
        }
    }
    method.isOnDestroy = method.name == "onDestroy";
    method.isOnInit = method.name == "onInit";

    bool seenOpen = false;
    int depth = 0;
    while (index < lines.size()) {
        const auto& logical = lines[index];
        std::string text = logical.text;
        int delta = braceDelta(text);
        if (!seenOpen) {
            size_t brace = text.find('{');
            if (brace != std::string::npos) {
                seenOpen = true;
                std::string after = text.substr(brace + 1);
                int afterDelta = braceDelta(after);
                depth = 1 + afterDelta;
                if (depth <= 0) {
                    size_t closeBrace = after.rfind('}');
                    std::string before = closeBrace == std::string::npos ? after : after.substr(0, closeBrace);
                    if (!trim(before).empty()) {
                        method.bodyLines.push_back(LogicalLine{SyntaxMode::Zinc, logical.loc, before});
                    }
                    ++index;
                    break;
                }
                if (!trim(after).empty()) {
                    method.bodyLines.push_back(LogicalLine{SyntaxMode::Zinc, logical.loc, after});
                }
            }
            ++index;
            continue;
        }
        if (depth + delta <= 0) {
            size_t closeBrace = text.rfind('}');
            std::string before = closeBrace == std::string::npos ? text : text.substr(0, closeBrace);
            if (!trim(before).empty()) {
                method.bodyLines.push_back(LogicalLine{SyntaxMode::Zinc, logical.loc, before});
            }
            ++index;
            break;
        }
        method.bodyLines.push_back(logical);
        depth += delta;
        ++index;
    }
    if (!seenOpen) {
        diagnostics_.error(start.loc, "missing '{' for method '" + method.name + "'");
    }
    return method;
}

Decl Parser::parseZincUnsupportedBlock(const std::vector<LogicalLine>& lines, size_t& index, const std::string& feature) {
    Decl decl;
    decl.kind = DeclKind::Unsupported;
    decl.mode = SyntaxMode::Zinc;
    decl.loc = lines[index].loc;
    decl.unsupportedFeature = feature;
    int depth = 0;
    bool seenBrace = false;
    while (index < lines.size()) {
        std::string t = trim(lines[index].text);
        decl.lines.push_back(lines[index].text);
        int delta = braceDelta(t);
        if (t.find('{') != std::string::npos) {
            seenBrace = true;
        }
        depth += delta;
        ++index;
        if ((seenBrace && depth <= 0) || (!seenBrace && !t.empty() && t.back() == ';')) {
            break;
        }
    }
    return decl;
}

} // namespace vjassc
