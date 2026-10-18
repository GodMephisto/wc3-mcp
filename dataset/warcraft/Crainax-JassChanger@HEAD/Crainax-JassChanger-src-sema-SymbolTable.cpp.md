---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/sema/SymbolTable.cpp"
ref: HEAD
---

#include "sema/SymbolTable.h"

#include "util/PathUtil.h"

#include <cctype>
#include <sstream>

namespace vjassc {
namespace {

bool isIdentStart(char c) {
    return std::isalpha(static_cast<unsigned char>(c)) || c == '_' || c == '$';
}

bool isIdentPart(char c) {
    return std::isalnum(static_cast<unsigned char>(c)) || c == '_' || c == '$';
}

std::string joinScope(const std::vector<std::string>& scopePath) {
    std::string out;
    for (size_t i = 0; i < scopePath.size(); ++i) {
        if (i > 0) {
            out += "_";
        }
        out += scopePath[i];
    }
    return out;
}

std::string makeScopedGlobalName(const std::vector<std::string>& scopePath, std::string_view access, std::string_view name) {
    std::string scope = joinScope(scopePath);
    if (access == "private") {
        return scope + "__" + std::string(name);
    }
    if (access == "public") {
        return std::string(name);
    }
    return scope + "_" + std::string(name);
}

std::string parseAccess(std::string& line) {
    std::string t = trim(line);
    if (startsWithWord(t, "public")) {
        line = trim(std::string_view(t).substr(6));
        return "public";
    }
    if (startsWithWord(t, "private")) {
        line = trim(std::string_view(t).substr(7));
        return "private";
    }
    line = t;
    return {};
}

} // namespace

std::string extractGlobalName(std::string line, std::string& access) {
    access = parseAccess(line);
    std::istringstream in(line);
    std::string word;
    std::string maybeType;
    in >> word;
    if (word == "constant") {
        in >> maybeType;
    }
    std::string name;
    in >> name;
    if (name == "array") {
        in >> name;
    }
    size_t bracket = name.find('[');
    if (bracket != std::string::npos) {
        name = name.substr(0, bracket);
    }
    size_t assign = name.find('=');
    if (assign != std::string::npos) {
        name = name.substr(0, assign);
    }
    return name;
}

std::string stripAccessPrefixFromLine(std::string line) {
    (void)parseAccess(line);
    return line;
}

void SymbolTable::build(const Program& program) {
    scoped_.clear();
    publicReplacements_.clear();
    publicFirstChars_.fill(false);
    std::vector<std::string> scopePath;
    for (const auto& decl : program.decls) {
        if (decl.kind == DeclKind::Library || decl.kind == DeclKind::Scope) {
            buildInContainer(decl, scopePath);
        }
    }
}

const ScopedSymbolMap* SymbolTable::symbolsFor(const Decl* container) const {
    auto it = scoped_.find(container);
    return it == scoped_.end() ? nullptr : &it->second;
}

std::string SymbolTable::renameForDecl(const Decl& decl, const Decl* container) const {
    if (decl.access.empty() || !container) {
        return decl.name;
    }
    auto symbols = symbolsFor(container);
    if (!symbols) {
        return decl.name;
    }
    auto it = symbols->replacements.find(decl.name);
    return it == symbols->replacements.end() ? decl.name : it->second;
}

std::string SymbolTable::rewriteLine(const std::string& line, const Decl* container) const {
    auto symbols = symbolsFor(container);
    if ((!symbols || symbols->replacements.empty()) && publicReplacements_.empty()) {
        return line;
    }

    const bool hasScoped = symbols && !symbols->replacements.empty();
    const bool hasPublic = !publicReplacements_.empty();
    std::string out;
    size_t appendFrom = 0;
    bool inString = false;
    bool inRaw = false;
    bool escaped = false;
    for (size_t i = 0; i < line.size();) {
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
        if (isIdentStart(c)) {
            size_t start = i++;
            while (i < line.size() && isIdentPart(line[i])) {
                ++i;
            }
            std::string_view ident(line.data() + start, i - start);
            size_t prev = start;
            while (prev > 0 && std::isspace(static_cast<unsigned char>(line[prev - 1]))) {
                --prev;
            }
            if (prev > 0 && line[prev - 1] == '.') {
                continue;
            }
            unsigned char first = static_cast<unsigned char>(line[start]);
            if ((!hasScoped || !symbols->firstChars[first]) && (!hasPublic || !publicFirstChars_[first])) {
                continue;
            }
            const std::string* replacement = nullptr;
            if (symbols) {
                auto it = symbols->replacements.find(ident);
                if (it != symbols->replacements.end()) {
                    replacement = &it->second;
                }
            }
            if (!replacement) {
                auto publicIt = publicReplacements_.find(ident);
                if (publicIt != publicReplacements_.end()) {
                    replacement = &publicIt->second;
                }
            }
            if (replacement) {
                if (out.empty()) {
                    out.reserve(line.size() + replacement->size());
                }
                out.append(line, appendFrom, start - appendFrom);
                out += *replacement;
                appendFrom = i;
            }
            continue;
        }
        ++i;
    }
    if (out.empty()) {
        return line;
    }
    out.append(line, appendFrom, std::string::npos);
    return out;
}

void SymbolTable::buildInContainer(const Decl& container, std::vector<std::string>& scopePath) {
    scopePath.push_back(container.name);
    ScopedSymbolMap map;
    for (const auto& child : container.children) {
        if (child.kind == DeclKind::Function || child.kind == DeclKind::FunctionInterface) {
            std::string access = child.access;
            if (child.name == "onInit" || (access.empty() && container.mode == SyntaxMode::Zinc)) {
                access = "private";
            }
            if (access.empty()) {
                continue;
            }
            std::string scopedName = makeScopedName(scopePath, access, child.name);
            map.replacements[child.name] = scopedName;
            if (!child.name.empty()) {
                map.firstChars[static_cast<unsigned char>(child.name.front())] = true;
            }
            if (child.access == "public") {
                publicReplacements_[child.name] = scopedName;
                if (!child.name.empty()) {
                    publicFirstChars_[static_cast<unsigned char>(child.name.front())] = true;
                }
            }
        } else if (child.kind == DeclKind::GlobalBlock) {
            collectGlobalNames(child, scopePath, map);
        }
    }
    scoped_[&container] = std::move(map);

    for (const auto& child : container.children) {
        if (child.kind == DeclKind::Scope) {
            buildInContainer(child, scopePath);
        }
    }
    scopePath.pop_back();
}

std::string SymbolTable::makeScopedName(const std::vector<std::string>& scopePath, std::string_view access, std::string_view name) const {
    std::string scope = joinScope(scopePath);
    if (access == "private") {
        return scope + "___" + std::string(name);
    }
    if (access == "public") {
        return std::string(name);
    }
    return scope + "_" + std::string(name);
}

void SymbolTable::collectGlobalNames(const Decl& globalBlock, const std::vector<std::string>& scopePath, ScopedSymbolMap& map) {
    for (auto line : globalBlock.lines) {
        std::string access;
        std::string name = extractGlobalName(line, access);
        if (!name.empty()) {
            if (access.empty() && globalBlock.access.empty() && globalBlock.mode != SyntaxMode::Zinc) {
                continue;
            }
            std::string effectiveAccess = access.empty() ? (globalBlock.access.empty() ? "private" : globalBlock.access) : access;
            std::string scopedName = makeScopedGlobalName(scopePath, effectiveAccess, name);
            map.replacements[name] = scopedName;
            if (!name.empty()) {
                map.firstChars[static_cast<unsigned char>(name.front())] = true;
            }
            if (effectiveAccess == "public") {
                publicReplacements_[name] = scopedName;
                if (!name.empty()) {
                    publicFirstChars_[static_cast<unsigned char>(name.front())] = true;
                }
            }
        }
    }
}

} // namespace vjassc
