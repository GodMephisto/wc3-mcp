---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/lexer/Lexer.cpp"
ref: HEAD
---

#include "lexer/Lexer.h"

#include <cctype>
#include <string_view>
#include <unordered_set>

namespace vjassc {
namespace {

const std::unordered_set<std::string>& keywords() {
    static const std::unordered_set<std::string> set = {
        "type", "extends", "native", "globals", "endglobals", "function", "takes", "returns",
        "endfunction", "local", "set", "call", "return", "if", "then", "elseif", "else",
        "endif", "loop", "endloop", "exitwhen", "library", "library_once", "endlibrary",
        "scope", "endscope", "initializer", "requires", "needs", "uses", "optional",
        "public", "private", "constant", "array", "debug", "keyword", "struct", "endstruct",
        "method", "endmodule", "endmethod", "static", "module", "implement", "interface",
        "endinterface", "while", "for", "break", "continue", "integer", "real", "boolean",
        "string", "code", "handle", "nothing", "true", "false", "and", "or", "not"
    };
    return set;
}

bool isIdentStart(char c) {
    return std::isalpha(static_cast<unsigned char>(c)) || c == '_' || c == '$';
}

bool isIdentPart(char c) {
    return std::isalnum(static_cast<unsigned char>(c)) || c == '_' || c == '$';
}

Token makeToken(TokenKind kind, SyntaxMode mode, SourceLocation loc, std::string text) {
    return Token{kind, mode, loc, std::move(text)};
}

} // namespace

const char* tokenKindName(TokenKind kind) {
    switch (kind) {
    case TokenKind::EndOfFile:
        return "EndOfFile";
    case TokenKind::Identifier:
        return "Identifier";
    case TokenKind::IntegerLiteral:
        return "IntegerLiteral";
    case TokenKind::RealLiteral:
        return "RealLiteral";
    case TokenKind::StringLiteral:
        return "StringLiteral";
    case TokenKind::RawCodeLiteral:
        return "RawCodeLiteral";
    case TokenKind::Keyword:
        return "Keyword";
    case TokenKind::LParen:
        return "LParen";
    case TokenKind::RParen:
        return "RParen";
    case TokenKind::LBrace:
        return "LBrace";
    case TokenKind::RBrace:
        return "RBrace";
    case TokenKind::LBracket:
        return "LBracket";
    case TokenKind::RBracket:
        return "RBracket";
    case TokenKind::Comma:
        return "Comma";
    case TokenKind::Dot:
        return "Dot";
    case TokenKind::Colon:
        return "Colon";
    case TokenKind::Semicolon:
        return "Semicolon";
    case TokenKind::Plus:
        return "Plus";
    case TokenKind::Minus:
        return "Minus";
    case TokenKind::Star:
        return "Star";
    case TokenKind::Slash:
        return "Slash";
    case TokenKind::Percent:
        return "Percent";
    case TokenKind::Assign:
        return "Assign";
    case TokenKind::PlusAssign:
        return "PlusAssign";
    case TokenKind::MinusAssign:
        return "MinusAssign";
    case TokenKind::StarAssign:
        return "StarAssign";
    case TokenKind::SlashAssign:
        return "SlashAssign";
    case TokenKind::EqualEqual:
        return "EqualEqual";
    case TokenKind::NotEqual:
        return "NotEqual";
    case TokenKind::Less:
        return "Less";
    case TokenKind::LessEqual:
        return "LessEqual";
    case TokenKind::Greater:
        return "Greater";
    case TokenKind::GreaterEqual:
        return "GreaterEqual";
    case TokenKind::Arrow:
        return "Arrow";
    case TokenKind::And:
        return "And";
    case TokenKind::Or:
        return "Or";
    case TokenKind::Not:
        return "Not";
    case TokenKind::LineComment:
        return "LineComment";
    case TokenKind::Unknown:
        return "Unknown";
    }
    return "Unknown";
}

bool isKeywordText(std::string_view text) {
    return keywords().contains(std::string(text));
}

Lexer::Lexer(Diagnostics& diagnostics) : diagnostics_(diagnostics) {}

LexerResult Lexer::lex(const std::vector<LogicalLine>& lines) {
    LexerResult result;
    int blockCommentDepth = 0;

    for (const auto& logical : lines) {
        const std::string& line = logical.text;
        size_t i = 0;
        while (i < line.size()) {
            SourceLocation loc = logical.loc;
            loc.column = static_cast<uint32_t>(i + 1);
            loc.offset = static_cast<uint32_t>(i);
            char c = line[i];

            if (blockCommentDepth > 0) {
                if (i + 1 < line.size() && line[i] == '/' && line[i + 1] == '*') {
                    ++blockCommentDepth;
                    i += 2;
                } else if (i + 1 < line.size() && line[i] == '*' && line[i + 1] == '/') {
                    --blockCommentDepth;
                    i += 2;
                } else {
                    ++i;
                }
                continue;
            }

            if (std::isspace(static_cast<unsigned char>(c))) {
                ++i;
                continue;
            }

            if (i + 1 < line.size() && line[i] == '/' && line[i + 1] == '/') {
                break;
            }
            if (i + 1 < line.size() && line[i] == '/' && line[i + 1] == '*') {
                blockCommentDepth = 1;
                i += 2;
                continue;
            }

            if (isIdentStart(c)) {
                size_t start = i++;
                while (i < line.size() && isIdentPart(line[i])) {
                    ++i;
                }
                std::string text = line.substr(start, i - start);
                TokenKind kind = isKeywordText(text) ? TokenKind::Keyword : TokenKind::Identifier;
                if (text == "and") {
                    kind = TokenKind::And;
                } else if (text == "or") {
                    kind = TokenKind::Or;
                } else if (text == "not") {
                    kind = TokenKind::Not;
                }
                result.tokens.push_back(makeToken(kind, logical.mode, loc, text));
                continue;
            }

            if (std::isdigit(static_cast<unsigned char>(c))) {
                size_t start = i++;
                bool real = false;
                while (i < line.size() && std::isdigit(static_cast<unsigned char>(line[i]))) {
                    ++i;
                }
                if (i < line.size() && line[i] == '.') {
                    real = true;
                    ++i;
                    while (i < line.size() && std::isdigit(static_cast<unsigned char>(line[i]))) {
                        ++i;
                    }
                }
                result.tokens.push_back(makeToken(real ? TokenKind::RealLiteral : TokenKind::IntegerLiteral,
                                                  logical.mode, loc, line.substr(start, i - start)));
                continue;
            }

            if (c == '"') {
                size_t start = i++;
                bool closed = false;
                bool escaped = false;
                while (i < line.size()) {
                    char d = line[i++];
                    if (escaped) {
                        escaped = false;
                    } else if (d == '\\') {
                        escaped = true;
                    } else if (d == '"') {
                        closed = true;
                        break;
                    }
                }
                if (!closed) {
                    diagnostics_.error(loc, "unterminated string literal");
                }
                result.tokens.push_back(makeToken(TokenKind::StringLiteral, logical.mode, loc, line.substr(start, i - start)));
                continue;
            }

            if (c == '\'') {
                size_t start = i++;
                bool closed = false;
                while (i < line.size()) {
                    if (line[i++] == '\'') {
                        closed = true;
                        break;
                    }
                }
                if (!closed) {
                    diagnostics_.error(loc, "unterminated rawcode literal");
                }
                result.tokens.push_back(makeToken(TokenKind::RawCodeLiteral, logical.mode, loc, line.substr(start, i - start)));
                continue;
            }

            auto two = i + 1 < line.size() ? line.substr(i, 2) : std::string();
            if (two == "->") {
                result.tokens.push_back(makeToken(TokenKind::Arrow, logical.mode, loc, two));
                i += 2;
            } else if (two == "+=") {
                result.tokens.push_back(makeToken(TokenKind::PlusAssign, logical.mode, loc, two));
                i += 2;
            } else if (two == "-=") {
                result.tokens.push_back(makeToken(TokenKind::MinusAssign, logical.mode, loc, two));
                i += 2;
            } else if (two == "*=") {
                result.tokens.push_back(makeToken(TokenKind::StarAssign, logical.mode, loc, two));
                i += 2;
            } else if (two == "/=") {
                result.tokens.push_back(makeToken(TokenKind::SlashAssign, logical.mode, loc, two));
                i += 2;
            } else if (two == "==") {
                result.tokens.push_back(makeToken(TokenKind::EqualEqual, logical.mode, loc, two));
                i += 2;
            } else if (two == "!=") {
                result.tokens.push_back(makeToken(TokenKind::NotEqual, logical.mode, loc, two));
                i += 2;
            } else if (two == "<=") {
                result.tokens.push_back(makeToken(TokenKind::LessEqual, logical.mode, loc, two));
                i += 2;
            } else if (two == ">=") {
                result.tokens.push_back(makeToken(TokenKind::GreaterEqual, logical.mode, loc, two));
                i += 2;
            } else if (two == "&&") {
                result.tokens.push_back(makeToken(TokenKind::And, logical.mode, loc, two));
                i += 2;
            } else if (two == "||") {
                result.tokens.push_back(makeToken(TokenKind::Or, logical.mode, loc, two));
                i += 2;
            } else {
                TokenKind kind = TokenKind::Unknown;
                switch (c) {
                case '(':
                    kind = TokenKind::LParen;
                    break;
                case ')':
                    kind = TokenKind::RParen;
                    break;
                case '{':
                    kind = TokenKind::LBrace;
                    break;
                case '}':
                    kind = TokenKind::RBrace;
                    break;
                case '[':
                    kind = TokenKind::LBracket;
                    break;
                case ']':
                    kind = TokenKind::RBracket;
                    break;
                case ',':
                    kind = TokenKind::Comma;
                    break;
                case '.':
                    kind = TokenKind::Dot;
                    break;
                case ':':
                    kind = TokenKind::Colon;
                    break;
                case ';':
                    kind = TokenKind::Semicolon;
                    break;
                case '+':
                    kind = TokenKind::Plus;
                    break;
                case '-':
                    kind = TokenKind::Minus;
                    break;
                case '*':
                    kind = TokenKind::Star;
                    break;
                case '/':
                    kind = TokenKind::Slash;
                    break;
                case '%':
                    kind = TokenKind::Percent;
                    break;
                case '=':
                    kind = TokenKind::Assign;
                    break;
                case '<':
                    kind = TokenKind::Less;
                    break;
                case '>':
                    kind = TokenKind::Greater;
                    break;
                case '!':
                    kind = TokenKind::Not;
                    break;
                default:
                    diagnostics_.warning(loc, std::string("unknown character '") + c + "'");
                    break;
                }
                result.tokens.push_back(makeToken(kind, logical.mode, loc, std::string(1, c)));
                ++i;
            }
        }
    }

    result.tokens.push_back(Token{TokenKind::EndOfFile, SyntaxMode::JassLike, SourceLocation{}, ""});
    if (blockCommentDepth > 0) {
        diagnostics_.error(SourceLocation{}, "unterminated block comment");
    }
    return result;
}

} // namespace vjassc
