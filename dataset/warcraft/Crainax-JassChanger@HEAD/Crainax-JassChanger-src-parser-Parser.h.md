---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/parser/Parser.h"
ref: HEAD
---

#pragma once

#include "core/Diagnostics.h"
#include "parser/Ast.h"
#include "preprocess/TextMacro.h"

#include <vector>

namespace vjassc {

class Parser {
public:
    Parser(Diagnostics& diagnostics);
    Program parse(const std::vector<LogicalLine>& lines);

private:
    void preScanUnsupported(const std::vector<LogicalLine>& lines);
    std::vector<Decl> parseJassRange(const std::vector<LogicalLine>& lines, size_t& index, const std::string& terminator);
    Decl parseFunctionInterface(const LogicalLine& line);
    Decl parseJassFunction(const std::vector<LogicalLine>& lines, size_t& index);
    Decl parseJassGlobalBlock(const std::vector<LogicalLine>& lines, size_t& index);
    Decl parseJassLibraryOrScope(const std::vector<LogicalLine>& lines, size_t& index, bool isScope);
    Decl parseJassStruct(const std::vector<LogicalLine>& lines, size_t& index);
    Decl parseJassModule(const std::vector<LogicalLine>& lines, size_t& index);
    MethodDecl parseJassMethod(const std::vector<LogicalLine>& lines, size_t& index);
    ModuleUseDecl parseJassModuleUse(const LogicalLine& line);
    Decl parseUnsupportedBlock(const std::vector<LogicalLine>& lines, size_t& index, const std::string& feature, const std::string& endWord);

    Decl parseZincLibrary(const std::vector<LogicalLine>& lines, size_t& index);
    std::vector<Decl> parseZincMembers(const std::vector<LogicalLine>& lines);
    Decl parseZincFunction(const std::vector<LogicalLine>& lines, size_t& index);
    Decl parseZincStruct(const std::vector<LogicalLine>& lines, size_t& index);
    Decl parseZincModule(const std::vector<LogicalLine>& lines, size_t& index);
    MethodDecl parseZincMethod(const std::vector<LogicalLine>& lines, size_t& index);
    ModuleUseDecl parseZincModuleUse(const LogicalLine& line);
    Decl parseZincUnsupportedBlock(const std::vector<LogicalLine>& lines, size_t& index, const std::string& feature);

    Diagnostics& diagnostics_;
    ParserStats stats_;
};

} // namespace vjassc
