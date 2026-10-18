---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/parser/Ast.h"
ref: HEAD
---

#pragma once

#include "core/SourceManager.h"
#include "preprocess/TextMacro.h"

#include <string>
#include <vector>

namespace vjassc {

enum class DeclKind {
    GlobalBlock,
    Native,
    TypeDecl,
    Function,
    Library,
    Scope,
    Struct,
    Module,
    FunctionInterface,
    Unsupported,
};

struct Requirement {
    std::string name;
    bool optional = false;
};

struct TypeRef {
    std::string name;
    SourceLocation loc;
    bool isArray = false;
    std::vector<int> arrayDimensions;
};

struct ParamDecl {
    TypeRef type;
    std::string name;
    SourceLocation loc;
};

struct FieldDecl {
    std::string name;
    TypeRef type;
    std::string access;
    SourceLocation loc;
    bool isStatic = false;
    bool isConstant = false;
    bool isReadonly = false;
    bool isArray = false;
    bool isFixedArray = false;
    int fixedArraySize = 0;
    std::vector<int> arrayDimensions;
    std::string initializer;
    std::string generatedName;
};

struct MethodDecl {
    std::string name;
    std::string access;
    SyntaxMode mode = SyntaxMode::JassLike;
    SourceLocation loc;
    bool isStatic = false;
    bool isOperator = false;
    bool isOnDestroy = false;
    bool isOnInit = false;
    std::vector<ParamDecl> params;
    TypeRef returnType;
    std::vector<LogicalLine> bodyLines;
    std::string generatedName;
    std::string moduleOriginName;
};

struct ModuleUseDecl {
    std::string name;
    SourceLocation loc;
    bool optional = false;
    SyntaxMode mode = SyntaxMode::JassLike;
};

struct Decl {
    DeclKind kind = DeclKind::Unsupported;
    SyntaxMode mode = SyntaxMode::JassLike;
    SourceLocation loc;
    std::string name;
    std::string access;
    std::string initializer;
    std::string unsupportedFeature;
    bool libraryOnce = false;
    bool isArrayStruct = false;
    std::string extendsName;
    std::vector<Requirement> requirements;
    std::vector<std::string> lines;
    std::vector<Decl> children;
    std::vector<FieldDecl> fields;
    std::vector<MethodDecl> methods;
    std::vector<ModuleUseDecl> moduleUses;
    std::vector<ParamDecl> interfaceParams;
    TypeRef interfaceReturnType;
    std::string generatedName;
    std::string prefix;
    std::string moduleOriginName;
};

struct ParserStats {
    size_t libraries = 0;
    size_t libraryOnce = 0;
    size_t scopes = 0;
    size_t globalsBlocks = 0;
    size_t natives = 0;
    size_t types = 0;
    size_t functions = 0;
    size_t functionInterfaces = 0;
    size_t modules = 0;
    size_t moduleUses = 0;
    size_t staticIfs = 0;
    size_t staticIfResolvedTrue = 0;
    size_t staticIfResolvedFalse = 0;
    size_t staticIfPrunedLines = 0;
    size_t moduleExpansions = 0;
    size_t structsUnsupported = 0;
    size_t methodsUnsupported = 0;
    size_t modulesUnsupported = 0;
    size_t staticIfUnsupported = 0;
    size_t functionInterfacesUnsupported = 0;
    size_t functionInterfaceTargets = 0;
    size_t functionInterfaceCalls = 0;
    size_t functionObjectCalls = 0;
    size_t lambdas = 0;
    size_t lambdasLowered = 0;
    size_t lambdasCodeContext = 0;
    size_t lambdasBoolexprContext = 0;
    size_t lambdasFunctionInterfaceContext = 0;
    size_t lambdasNativeCallbackContext = 0;
    size_t lambdasMethodCallbackContext = 0;
    size_t lambdasUnknownContext = 0;
    size_t lambdasCapturing = 0;
    size_t lambdasRejected = 0;
    size_t lambdasGeneratedFunctions = 0;
    size_t lambdasCapturingUnsupported = 0;
    size_t functionInterfaceMaxEvaluateDepth = 0;
    size_t functionInterfaceEvaluateTempLimit = 8;
    size_t prototypeWrappers = 0;
};

struct Program {
    std::vector<Decl> decls;
    ParserStats stats;

    bool hasUnsupported() const;
};

const char* declKindName(DeclKind kind);

} // namespace vjassc
