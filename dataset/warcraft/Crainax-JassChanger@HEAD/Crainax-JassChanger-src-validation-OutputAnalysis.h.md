---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/validation/OutputAnalysis.h"
ref: HEAD
---

#pragma once

#include <filesystem>
#include <string>
#include <string_view>
#include <vector>

namespace vjassc {

struct ValidationIssue {
    std::string check;
    size_t line = 0;
    std::string message;
    std::string snippet;
};

struct OutputMetrics {
    size_t bytes = 0;
    size_t lines = 0;
    size_t globalsBlocks = 0;
    size_t endglobals = 0;
    size_t globalsDeclarations = 0;
    size_t natives = 0;
    size_t types = 0;
    size_t functions = 0;
    size_t generatedLambdaFunctions = 0;
    size_t structSupportFunctions = 0;
    size_t functionInterfaceWrappers = 0;
    size_t sourceFormResidues = 0;
    size_t duplicateFunctionNames = 0;
    size_t duplicateGlobalNames = 0;
    size_t duplicateNativeNames = 0;
    bool hasMain = false;
    bool hasConfig = false;
    std::vector<std::string> initHelperNames;
};

struct OutputSyntaxReport {
    bool ran = false;
    bool ok = true;
    OutputMetrics metrics;
    std::vector<std::string> residualSourceForms;
    std::vector<ValidationIssue> issues;
    std::vector<ValidationIssue> forwardFunctionReferences;
    std::vector<ValidationIssue> forwardLambdaReferences;
    std::vector<ValidationIssue> commaLocalResidues;
    std::vector<ValidationIssue> indexedStructMemberResidues;
    std::vector<ValidationIssue> inlineZincControlResidues;
    std::vector<ValidationIssue> methodChainCallResultResidues;
    std::vector<ValidationIssue> callbackCodeSignatureResidues;
    std::vector<ValidationIssue> unresolvedKnownSourceSymbols;
    std::vector<ValidationIssue> returnMismatchLikelyResidues;
    std::vector<ValidationIssue> trueFunctionCycles;
};

struct InitValidationReport {
    bool hasMain = false;
    bool hasConfig = false;
    bool hasStructInit = false;
    bool hasFunctionInterfaceInit = false;
    bool hasLibraryInit = false;
    bool mainCallsStructInit = false;
    bool mainCallsFunctionInterfaceInit = false;
    bool mainCallsLibraryInit = false;
    bool structBeforeFunctionInterface = false;
    bool functionInterfaceBeforeLibrary = false;
    size_t libraryInitializerCount = 0;
    size_t structInitializerCount = 0;
    std::vector<std::string> issues;
};

struct ComparisonReport {
    std::filesystem::path referencePath;
    bool referenceFound = false;
    OutputMetrics generated;
    OutputMetrics reference;
    std::vector<std::string> notes;
};

OutputSyntaxReport analyzeOutputSyntaxLite(std::string_view output);
std::vector<ValidationIssue> findInvalidJassStatementShapes(std::string_view output);
InitValidationReport analyzeInitIntegrity(std::string_view output);
OutputMetrics collectOutputMetrics(std::string_view output);
ComparisonReport compareWithReference(std::string_view generated, const std::filesystem::path& referencePath);
std::string previewText(std::string_view text, size_t maxBytes = 4096);

} // namespace vjassc
