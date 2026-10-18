---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/util/PathUtil.cpp"
ref: HEAD
---

#include "util/PathUtil.h"

#include <algorithm>
#include <cctype>

namespace vjassc {

std::string ltrim(std::string_view text) {
    size_t i = 0;
    while (i < text.size() && std::isspace(static_cast<unsigned char>(text[i]))) {
        ++i;
    }
    return std::string(text.substr(i));
}

std::string rtrim(std::string_view text) {
    size_t end = text.size();
    while (end > 0 && std::isspace(static_cast<unsigned char>(text[end - 1]))) {
        --end;
    }
    return std::string(text.substr(0, end));
}

std::string trim(std::string_view text) {
    size_t start = 0;
    while (start < text.size() && std::isspace(static_cast<unsigned char>(text[start]))) {
        ++start;
    }
    size_t end = text.size();
    while (end > start && std::isspace(static_cast<unsigned char>(text[end - 1]))) {
        --end;
    }
    return std::string(text.substr(start, end - start));
}

bool startsWithWord(std::string_view text, std::string_view word) {
    if (text.size() < word.size() || text.substr(0, word.size()) != word) {
        return false;
    }
    if (text.size() == word.size()) {
        return true;
    }
    unsigned char c = static_cast<unsigned char>(text[word.size()]);
    return !(std::isalnum(c) || c == '_');
}

bool iequals(std::string_view a, std::string_view b) {
    if (a.size() != b.size()) {
        return false;
    }
    for (size_t i = 0; i < a.size(); ++i) {
        if (std::tolower(static_cast<unsigned char>(a[i])) != std::tolower(static_cast<unsigned char>(b[i]))) {
            return false;
        }
    }
    return true;
}

std::filesystem::path weakCanonical(const std::filesystem::path& path) {
    std::error_code ec;
    auto result = std::filesystem::weakly_canonical(path, ec);
    return ec ? std::filesystem::absolute(path) : result;
}

std::string pathToGenericString(const std::filesystem::path& path) {
    return path.generic_string();
}

std::vector<std::string> splitCommaListRespectingQuotes(std::string_view text) {
    std::vector<std::string> parts;
    std::string current;
    bool inString = false;
    bool inRaw = false;
    int parens = 0;
    for (size_t i = 0; i < text.size(); ++i) {
        char c = text[i];
        if (c == '"' && !inRaw) {
            inString = !inString;
            current.push_back(c);
            continue;
        }
        if (c == '\'' && !inString) {
            inRaw = !inRaw;
            current.push_back(c);
            continue;
        }
        if (!inString && !inRaw) {
            if (c == '(') {
                ++parens;
            } else if (c == ')' && parens > 0) {
                --parens;
            } else if (c == ',' && parens == 0) {
                parts.push_back(trim(current));
                current.clear();
                continue;
            }
        }
        current.push_back(c);
    }
    if (!current.empty() || !parts.empty()) {
        parts.push_back(trim(current));
    }
    return parts;
}

std::string stripOuterQuotes(std::string text) {
    text = trim(text);
    if (text.size() >= 2 && text.front() == '"' && text.back() == '"') {
        std::string out;
        out.reserve(text.size() - 2);
        for (size_t i = 1; i + 1 < text.size(); ++i) {
            if (text[i] == '\\' && i + 2 < text.size()) {
                out.push_back(text[++i]);
            } else {
                out.push_back(text[i]);
            }
        }
        return out;
    }
    return text;
}

} // namespace vjassc
