---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/condition/BoolExpr.cpp"
ref: HEAD
---

#include "condition/BoolExpr.h"

#include <cctype>
#include <string>

namespace vjassc {
namespace {

enum class TokenKind {
    End,
    Identifier,
    True,
    False,
    DebugMode,
    Not,
    And,
    Or,
    LParen,
    RParen,
    Invalid,
};

struct Token {
    TokenKind kind = TokenKind::End;
    std::string text;
};

bool isIdentStart(char c) {
    return std::isalpha(static_cast<unsigned char>(c)) || c == '_';
}

bool isIdentPart(char c) {
    return std::isalnum(static_cast<unsigned char>(c)) || c == '_';
}

class Lexer {
public:
    explicit Lexer(std::string_view input) : input_(input) {}

    Token next() {
        while (pos_ < input_.size() && std::isspace(static_cast<unsigned char>(input_[pos_]))) {
            ++pos_;
        }
        if (pos_ >= input_.size()) {
            return Token{TokenKind::End, {}};
        }
        char c = input_[pos_];
        if (c == '(') {
            ++pos_;
            return Token{TokenKind::LParen, "("};
        }
        if (c == ')') {
            ++pos_;
            return Token{TokenKind::RParen, ")"};
        }
        if (c == '!') {
            ++pos_;
            return Token{TokenKind::Not, "!"};
        }
        if (c == '&' && pos_ + 1 < input_.size() && input_[pos_ + 1] == '&') {
            pos_ += 2;
            return Token{TokenKind::And, "&&"};
        }
        if (c == '|' && pos_ + 1 < input_.size() && input_[pos_ + 1] == '|') {
            pos_ += 2;
            return Token{TokenKind::Or, "||"};
        }
        if (isIdentStart(c)) {
            size_t start = pos_++;
            while (pos_ < input_.size() && isIdentPart(input_[pos_])) {
                ++pos_;
            }
            std::string ident(input_.substr(start, pos_ - start));
            if (ident == "true") {
                return Token{TokenKind::True, ident};
            }
            if (ident == "false") {
                return Token{TokenKind::False, ident};
            }
            if (ident == "DEBUG_MODE") {
                return Token{TokenKind::DebugMode, ident};
            }
            if (ident == "not") {
                return Token{TokenKind::Not, ident};
            }
            if (ident == "and") {
                return Token{TokenKind::And, ident};
            }
            if (ident == "or") {
                return Token{TokenKind::Or, ident};
            }
            return Token{TokenKind::Identifier, ident};
        }
        ++pos_;
        return Token{TokenKind::Invalid, std::string(1, c)};
    }

private:
    std::string_view input_;
    size_t pos_ = 0;
};

class Parser {
public:
    Parser(std::string_view input, const BoolEvalContext& context) : lexer_(input), context_(context) {
        current_ = lexer_.next();
    }

    BoolEvalResult parse() {
        BoolEvalResult result;
        result.ok = true;
        bool value = parseOr(result);
        if (result.ok && current_.kind != TokenKind::End) {
            result.ok = false;
            result.error = "unexpected token '" + current_.text + "'";
        }
        result.value = result.ok ? value : false;
        return result;
    }

private:
    void advance() {
        current_ = lexer_.next();
    }

    bool parseOr(BoolEvalResult& result) {
        bool value = parseAnd(result);
        while (result.ok && current_.kind == TokenKind::Or) {
            advance();
            bool rhs = parseAnd(result);
            value = value || rhs;
        }
        return value;
    }

    bool parseAnd(BoolEvalResult& result) {
        bool value = parseNot(result);
        while (result.ok && current_.kind == TokenKind::And) {
            advance();
            bool rhs = parseNot(result);
            value = value && rhs;
        }
        return value;
    }

    bool parseNot(BoolEvalResult& result) {
        bool negated = false;
        while (current_.kind == TokenKind::Not) {
            negated = !negated;
            advance();
        }
        bool value = parsePrimary(result);
        return negated ? !value : value;
    }

    bool parsePrimary(BoolEvalResult& result) {
        switch (current_.kind) {
        case TokenKind::True:
            advance();
            return true;
        case TokenKind::False:
            advance();
            return false;
        case TokenKind::DebugMode:
            advance();
            return context_.debugMode;
        case TokenKind::Identifier:
            return resolveIdentifier(result);
        case TokenKind::LParen: {
            advance();
            bool value = parseOr(result);
            if (!result.ok) {
                return false;
            }
            if (current_.kind != TokenKind::RParen) {
                result.ok = false;
                result.error = "missing ')'";
                return false;
            }
            advance();
            return value;
        }
        case TokenKind::Invalid:
            result.ok = false;
            result.error = "invalid token '" + current_.text + "'";
            return false;
        default:
            result.ok = false;
            result.error = "expected boolean expression";
            return false;
        }
    }

    bool resolveIdentifier(BoolEvalResult& result) {
        std::string name = current_.text;
        advance();
        constexpr std::string_view libraryPrefix = "LIBRARY_";
        if (name.rfind(libraryPrefix, 0) == 0) {
            std::string libraryName = name.substr(libraryPrefix.size());
            return context_.libraries.contains(libraryName);
        }
        auto it = context_.boolConstants.find(name);
        if (it != context_.boolConstants.end()) {
            return it->second;
        }
        result.unknownIdentifiers.push_back(name);
        return false;
    }

    Lexer lexer_;
    const BoolEvalContext& context_;
    Token current_;
};

} // namespace

BoolEvalResult evalBoolExpr(std::string_view expr, const BoolEvalContext& context) {
    Parser parser(expr, context);
    return parser.parse();
}

} // namespace vjassc
