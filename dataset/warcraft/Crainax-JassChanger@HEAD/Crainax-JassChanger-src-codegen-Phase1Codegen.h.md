---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/codegen/Phase1Codegen.h"
ref: HEAD
---

#pragma once

#include "core/Diagnostics.h"
#include "codegen/CodeWriter.h"
#include "parser/Ast.h"
#include "sema/LibraryGraph.h"
#include "sema/SymbolTable.h"

#include <array>
#include <filesystem>
#include <string>
#include <string_view>
#include <unordered_map>
#include <unordered_set>
#include <vector>

namespace vjassc {

using LocalTypeMap = std::unordered_map<std::string, std::string, TransparentStringHash, TransparentStringEqual>;

struct CodegenOptions {
    bool scanOnly = false;
    bool allowUnsupported = false;
    bool warnMode = false;
    bool fastMode = false;
    bool emitGeneratedEntityPlan = false;
    bool experimentalRecordedOrder = false;
    bool experimentalParallelLowering = false;
    bool experimentalBodyJobsSingleThread = false;
    bool experimentalIncrementalReuse = false;
    size_t parallelWorkers = 0;
    std::filesystem::path experimentalIncrementalCachePath;
};

struct CodegenPerformanceCounters {
    size_t linesVisited = 0;
    size_t regexCalls = 0;
    size_t memberAccessScans = 0;
    size_t structLookupCalls = 0;
    size_t functionLookupCalls = 0;
    size_t cachedRewriteHits = 0;
    size_t cachedRewriteMisses = 0;
    size_t lineFeatureScans = 0;
    size_t linesSkippedNoDotBracketCall = 0;
    size_t linesSkippedNoCurrentStruct = 0;
    size_t linesSkippedGeneratedSupport = 0;
    size_t structMethodLinesLowered = 0;
    size_t generatedSupportLinesEmitted = 0;
    size_t generatedSupportLinesLowered = 0;
    size_t receiverChainAttempts = 0;
    size_t receiverChainChanged = 0;
    size_t arrayAccessRewriteAttempts = 0;
    size_t arrayAccessRewriteChanged = 0;
    size_t functionOrderTokenScans = 0;
    size_t functionOrderEdges = 0;
    size_t functionOrderSccCount = 0;
    size_t bodyModeZincFunctions = 0;
    size_t bodyModeJassLikeFunctions = 0;
    size_t bodyModeZincMethods = 0;
    size_t bodyModeJassLikeMethods = 0;
    size_t bodyModeGeneratedBodies = 0;
    size_t bodyLowererZincBodies = 0;
    size_t bodyLowererJassLikeBodies = 0;
    size_t bodyLowererGeneratedBodies = 0;
    size_t bodyLowererZincLines = 0;
    size_t bodyLowererJassLikeLines = 0;
    size_t bodyLowererGeneratedLines = 0;
    size_t generatedLinesSkippedGenericLowering = 0;
    size_t tokenCacheBuilds = 0;
    size_t tokenCacheLines = 0;
    size_t tokenCacheHits = 0;
    size_t tokenCacheMisses = 0;
    size_t featureScansAvoided = 0;
    size_t functionOrderScansAvoided = 0;
    size_t zincFastPathLines = 0;
    size_t jassLikeFastPathLines = 0;
    size_t generatedFastPathLines = 0;
    size_t heavyLoweringAvoidedByMode = 0;
    size_t functionDependencyRecordedEdges = 0;
    size_t functionDependencyOutputScanEdges = 0;
    size_t functionDependencyMatchedEdges = 0;
    size_t functionDependencyMissingRecordedEdges = 0;
    size_t functionDependencyExtraRecordedEdges = 0;
    size_t functionDependencyWeakExecuteFuncEdges = 0;
    size_t functionDependencyRecordedOrderEdgesUsed = 0;
    size_t functionDependencyRecordedOrderFallbacks = 0;
    size_t experimentalBodyJobsSingleThread = 0;
    size_t experimentalParallelWorkers = 0;
    size_t experimentalParallelJobs = 0;
    size_t experimentalParallelCompletedJobs = 0;
    size_t experimentalParallelFailedJobs = 0;
    size_t experimentalParallelQueueMs = 0;
    size_t experimentalParallelWorkerTotalMs = 0;
    size_t experimentalParallelMergeMs = 0;
    size_t bodyCacheEnabled = 0;
    size_t bodyCacheLookups = 0;
    size_t bodyCacheHits = 0;
    size_t bodyCacheMisses = 0;
    size_t bodyCacheStores = 0;
    size_t bodyCacheBypassedUnsafe = 0;
    size_t bodyCacheReusedLines = 0;
    size_t bodyCacheStoredLines = 0;
    size_t methodPlanBuilt = 0;
    size_t methodPlanLinesSkippedNoCandidate = 0;
    size_t methodPlanBareFieldRewriteAttempts = 0;
    size_t methodPlanBareFieldRewriteChanged = 0;
    size_t methodPlanShadowSkips = 0;
};

struct CodegenResult {
    bool ok = false;
    std::string output;
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
    size_t functionInterfaceTargets = 0;
    size_t functionInterfaceCalls = 0;
    size_t functionObjectCalls = 0;
    size_t functionInterfaceMaxEvaluateDepth = 0;
    size_t functionInterfaceEvaluateTempLimit = 8;
    std::unordered_map<std::string, long long> passTimings;
    CodegenPerformanceCounters performanceCounters;
    std::string generatedEntityPlanJson;
};

class Phase1Codegen {
public:
    Phase1Codegen(Diagnostics& diagnostics, CodegenOptions options);
    CodegenResult generate(const Program& program);

private:
    struct ArrayShape {
        std::vector<int> dimensions;
        bool structInstanceField = false;
    };
    using ArrayShapeMap = std::unordered_map<std::string, ArrayShape, TransparentStringHash, TransparentStringEqual>;

    struct FieldInfo {
        const FieldDecl* decl = nullptr;
        std::string name;
        std::string typeName;
        std::string generatedName;
        bool isStatic = false;
        bool isArray = false;
        bool isFixedArray = false;
        int fixedArraySize = 0;
        std::vector<int> arrayDimensions;
    };

    struct MethodInfo {
        const MethodDecl* decl = nullptr;
        std::string name;
        std::string generatedName;
        bool isStatic = false;
        bool isOnDestroy = false;
        bool isOnInit = false;
    };

    struct StructInfo {
        const Decl* decl = nullptr;
        const Decl* container = nullptr;
        const Decl* libraryContainer = nullptr;
        std::string originalName;
        std::string generatedName;
        std::string prefix;
        int typeId = 0;
        bool isArrayStruct = false;
        std::vector<FieldInfo> fields;
        std::vector<MethodInfo> methods;
        std::unordered_map<std::string, size_t, TransparentStringHash, TransparentStringEqual> fieldIndex;
        std::unordered_map<std::string, size_t, TransparentStringHash, TransparentStringEqual> methodIndex;
        std::array<bool, 256> fieldFirstChars{};
        std::array<bool, 256> methodFirstChars{};
    };

    struct StructInitializerInfo {
        std::string name;
        const Decl* libraryContainer = nullptr;
    };

    struct FunctionSignature {
        std::vector<std::string> paramTypes;
        std::string returnType = "nothing";
    };

    enum class GeneratedKind {
        None,
        StructAllocate,
        StructCreate,
        StructDestroy,
        StructDeallocate,
        StructOnDestroyWrapper,
        StructOnInitWrapper,
        FunctionInterfaceWrapper,
        LambdaWrapper,
        CycleBridge,
    };

    enum class BodyMode {
        JassLike,
        Zinc,
        Generated,
    };

    struct FunctionInfo {
        std::string sourceName;
        std::string finalName;
        FunctionSignature signature;
        bool isStaticMethod = false;
        GeneratedKind generatedKind = GeneratedKind::None;
    };

    struct InterfaceTarget {
        std::string finalName;
        int id = 0;
        bool needsCondition = false;
        bool needsAction = false;
    };

    struct FunctionInterfaceInfo {
        const Decl* decl = nullptr;
        const Decl* container = nullptr;
        std::string sourceName;
        std::string finalName;
        FunctionSignature signature;
        std::vector<InterfaceTarget> targets;
        std::unordered_map<std::string, size_t> targetIndexByFinalName;
        bool allTargetsNeedCondition = false;
        bool allTargetsNeedAction = false;
        bool syntheticFunctionObject = false;
    };

    struct LambdaInfo {
        std::string name;
        SourceLocation loc;
        const StructInfo* currentStruct = nullptr;
        const Decl* container = nullptr;
        std::vector<ParamDecl> params;
        TypeRef returnType;
        std::vector<std::string> bodyLines;
    };

    struct LoweringContext {
        const StructInfo* currentStruct = nullptr;
        const Decl* container = nullptr;
        LocalTypeMap* localTypes = nullptr;
        ArrayShapeMap* localArrayShapes = nullptr;
        std::vector<std::string>* tempLocals = nullptr;
        BodyMode mode = BodyMode::JassLike;
        std::string currentFunctionName;
        int tempCounter = 0;
    };

    struct IdentifierToken {
        size_t start = 0;
        size_t end = 0;
        bool precededByDot = false;
    };

    struct LineTokenCache {
        std::string line;
        bool hasDot = false;
        bool hasBracket = false;
        bool hasParen = false;
        bool hasCallWord = false;
        bool hasSetWord = false;
        bool hasLocalWord = false;
        bool hasReturnWord = false;
        bool hasFunctionWord = false;
        bool hasLambdaStart = false;
        bool hasThisWord = false;
        bool hasThistypeWord = false;
        bool hasNameToken = false;
        bool hasExecuteEvaluate = false;
        bool hasBooleanOperators = false;
        bool hasStringOrRawcode = false;
        bool hasGeneratedStructPrefix = false;
        std::vector<IdentifierToken> identifiers;
        std::vector<size_t> parenPositions;
        std::vector<size_t> bracketPositions;
    };

    struct LineFeatures {
        bool hasDot = false;
        bool hasBracket = false;
        bool hasParen = false;
        bool hasCall = false;
        bool hasSet = false;
        bool hasLocal = false;
        bool hasReturn = false;
        bool hasFunctionKeyword = false;
        bool hasLambdaStart = false;
        bool hasThis = false;
        bool hasThistype = false;
        bool hasNameToken = false;
        bool hasExecuteEvaluate = false;
        bool hasBooleanOperators = false;
        bool hasKnownArrayReceiver = false;
        bool hasPotentialStructDot = false;
        bool hasPossibleStructMember = false;
        bool hasGeneratedStructPrefix = false;
        bool hasStringOrRawcode = false;
        std::vector<const FieldInfo*> currentStructFields;
        std::vector<const MethodInfo*> currentStructMethods;
    };

    void emitGlobals(const Program& program, const LibraryGraphResult& graph);
    void emitDeclGlobals(const Decl& decl, const Decl* container);
    void emitStructGlobals();
    void emitStructGlobalBlock(const StructInfo& info);
    void emitFunctionInterfaceGlobals();
    void emitTypesAndNatives(const Program& program, const LibraryGraphResult& graph, DeclKind kind);
    void emitTypeOrNative(const Decl& decl, DeclKind kind);
    void emitFunctions(const Program& program, const LibraryGraphResult& graph);
    void emitDeclFunctions(const Decl& decl, const Decl* container);
    void emitJassFunction(const Decl& decl, const Decl* container, bool injectInit);
    void emitZincFunction(const Decl& decl, const Decl* container);
    void emitGeneratedLambdas();
    void emitFunctionInterfaceRuntime();
    void emitStructFunctions();
    void emitStructSupportFunctions(const StructInfo& info);
    void emitStructGeneratedSupport(const StructInfo& info, const MethodInfo* customCreate);
    void emitStructLifecycleSupport(const StructInfo& info,
                                    const std::unordered_set<const MethodDecl*>& emittedBeforeDestroy,
                                    bool needsDeallocate);
    void emitStructSourceMethods(const StructInfo& info,
                                 std::unordered_set<const MethodDecl*>& emittedBeforeDestroy,
                                 bool onDestroyOnly);
    void emitStructMethod(const StructInfo& info, const MethodInfo& method);
    void emitInitHelper(const LibraryGraphResult& graph, const Program& program);
    void collectInitializers(const Decl& decl, const Decl* container);
    void collectRootInitializers(const std::vector<Decl>& decls);
    void collectStructs(const std::vector<Decl>& decls, const Decl* container, const Decl* libraryContainer);
    void collectStruct(const Decl& decl, const Decl* container, const Decl* libraryContainer);
    void collectFunctionInterfaces(const std::vector<Decl>& decls, const Decl* container);
    void collectFunctionObjectEvaluateInterfaces(const std::vector<Decl>& decls,
                                                 const Decl* container,
                                                 const StructInfo* currentStruct);
    void collectFunctionObjectEvaluateInterfaceFromLine(const std::string& line, const StructInfo* currentStruct);
    void collectFunctions(const std::vector<Decl>& decls, const Decl* container);
    void collectFunction(const Decl& decl, const Decl* container);
    void rememberFunctionInterfaceParamTypes(const std::string& sourceName,
                                             const std::string& finalName,
                                             const std::vector<std::string>& paramTypes,
                                             const Decl* declContainer);

    std::string rewriteForContainer(const std::string& line, const Decl* container) const;
    std::string rewriteGlobalLine(const std::string& line, const Decl* container);
    std::string rewriteFunctionHeader(const Decl& decl, const Decl* container) const;
    std::string renameInContainer(const std::string& name, const Decl* container) const;
    std::string makeScopedStructName(const Decl& decl, const Decl* container) const;
    std::string fixedArrayStorageName(const StructInfo& info, const FieldInfo& field) const;
    int structFixedArrayStride(const StructInfo& info) const;
    int structInstanceLimit(const StructInfo& info) const;
    std::string rewriteTypeName(const std::string& typeName, const StructInfo* currentStruct) const;
    std::string rewriteParamList(const std::vector<ParamDecl>& params, bool includeThis, const StructInfo* currentStruct) const;
    std::string rewriteStructExpression(const std::string& line,
                                        const StructInfo* currentStruct,
                                        const LocalTypeMap& localTypes) const;
    std::string rewriteReceiverChains(const std::string& line, const StructInfo* currentStruct) const;
    std::vector<std::string> rewriteLocalDeclLine(const std::string& line,
                                                  const StructInfo* currentStruct,
                                                  LocalTypeMap& localTypes,
                                                  ArrayShapeMap* localArrayShapes,
                                                  std::vector<std::string>& extraLines) const;
    std::string rewriteArrayAccesses(const std::string& line,
                                     const ArrayShapeMap* localArrayShapes) const;
    bool hasKnownArrayReceiver(const std::string& line,
                               const ArrayShapeMap* localArrayShapes) const;
    void lowerStatementLineInto(const std::string& line,
                                LoweringContext& ctx,
                                std::vector<std::string>& out) const;
    std::vector<std::string> lowerStatementLine(const std::string& line, LoweringContext& ctx) const;
    std::vector<std::string> lowerBodyByMode(BodyMode mode,
                                             const std::vector<std::string>& lines,
                                             LoweringContext& ctx);
    std::vector<std::string> lowerZincBodyFast(const std::vector<std::string>& lines,
                                               LoweringContext& ctx);
    std::vector<std::string> lowerJassLikeBodyFast(const std::vector<std::string>& lines,
                                                   LoweringContext& ctx);
    std::vector<std::string> lowerGeneratedBodyFast(const std::vector<std::string>& lines,
                                                    LoweringContext& ctx);
    std::string lowerExpression(std::string expression,
                                const std::string& expectedInterfaceType,
                                LoweringContext& ctx,
                                std::vector<std::string>& prelude) const;
    std::string inferUniqueFunctionInterfaceTypeForFunctionValue(const std::string& expression,
                                                                 const LoweringContext& ctx) const;
    std::string lowerFunctionValue(std::string expression,
                                   const std::string& expectedInterfaceType,
                                   LoweringContext& ctx,
                                   std::vector<std::string>& prelude) const;
    std::string rewriteCallArguments(std::string expression, LoweringContext& ctx, std::vector<std::string>& prelude) const;
    std::string rewriteFunctionNames(std::string expression, const StructInfo* currentStruct) const;
    LineFeatures scanLineFeatures(const std::string& line,
                                  const StructInfo* currentStruct,
                                  const LocalTypeMap* localTypes,
                                  const ArrayShapeMap* localArrayShapes) const;
    const LineTokenCache& getLineTokenCache(const std::string& line, bool* built = nullptr) const;
    LineTokenCache buildLineTokenCache(const std::string& line) const;
    BodyMode bodyModeForFunction(const Decl& decl) const;
    BodyMode bodyModeForMethod(const MethodDecl& method) const;
    BodyMode bodyModeForGenerated(GeneratedKind kind) const;
    void countBodyMode(BodyMode mode, bool isMethod) const;
    void recordFunctionDependency(const std::string& from, const std::string& to) const;
    bool lineNeedsExpressionLowering(const LineFeatures& features,
                                     const std::string& expectedInterfaceType,
                                     const LoweringContext& ctx) const;
    bool lineNeedsStructRewrite(const LineFeatures& features,
                                const StructInfo* currentStruct) const;
    const FunctionInterfaceInfo* resolveReceiverInterface(const std::string& receiver, const LoweringContext& ctx) const;
    std::string rewriteReceiverExpression(const std::string& receiver, const LoweringContext& ctx) const;
    int registerInterfaceTarget(const FunctionInterfaceInfo& iface, const std::string& targetName, SourceLocation loc) const;
    void markInterfaceTargetNeedsCondition(const FunctionInterfaceInfo& iface,
                                           const std::string& rewrittenReceiverExpression,
                                           SourceLocation loc) const;
    void markInterfaceTargetNeedsAction(const FunctionInterfaceInfo& iface,
                                        const std::string& rewrittenReceiverExpression,
                                        SourceLocation loc) const;
    const FunctionInterfaceInfo* findFunctionInterface(std::string_view name) const;
    FunctionInterfaceInfo& ensureFunctionObjectInterface(const FunctionSignature& signature) const;
    const FunctionInterfaceInfo* findFunctionObjectInterface(const FunctionSignature& signature) const;
    std::string functionSignatureKey(const FunctionSignature& signature) const;
    std::string resolveFunctionInterfaceTypeName(std::string_view name, const Decl* container) const;
    const FunctionInfo* findFunctionInfo(std::string_view name) const;
    std::string resolveFunctionTargetName(const std::string& expression, const StructInfo* currentStruct) const;
    bool sameSignature(const FunctionSignature& a, const FunctionSignature& b) const;
    FunctionSignature signatureFromParams(const std::vector<ParamDecl>& params, const TypeRef& returnType, const StructInfo* currentStruct) const;
    FunctionSignature parseFunctionSignatureFromHeader(const std::string& header, SyntaxMode mode) const;
    std::string interfaceGlobalPrefix(const FunctionInterfaceInfo& iface) const;
    void seedFunctionLocalTypes(const std::string& header, LocalTypeMap& localTypes) const;
    void seedMethodLocalTypes(const StructInfo& info, const MethodInfo& method, LocalTypeMap& localTypes) const;
    const StructInfo* findStruct(std::string_view name) const;
    const FieldInfo* findField(const StructInfo& info, std::string_view name) const;
    const MethodInfo* findMethod(const StructInfo& info, std::string_view name) const;
    bool structUsesDeallocate(const StructInfo& info) const;
    bool bodyCacheEnabled() const;
    bool bodyCacheSafe(const std::vector<std::string>& sourceLines, bool allowFunctionKeyword = false) const;
    std::string bodyCacheKey(const std::string& kind,
                             const std::string& stableKey,
                             BodyMode mode,
                             const std::vector<std::string>& sourceLines) const;
    std::string bodyCacheSignatureContextKey() const;
    bool loadBodyCache(const std::string& key, std::vector<std::string>& lines) const;
    void storeBodyCache(const std::string& key, const std::vector<std::string>& lines) const;

    std::vector<std::string> lowerZincBody(const std::vector<std::string>& lines);
    std::vector<std::string> extractZincLambdas(const std::vector<std::string>& lines, SourceLocation loc);
    void recordLambdaContext(const std::string& beforeText);
    CodegenResult makeResult(bool ok);
    std::string emitGeneratedEntityPlanJson() const;
    void lowerZincBlock(const std::vector<std::string>& lines, size_t& index, std::vector<std::string>& locals, std::vector<std::string>& body);
    void lowerZincSimpleStatement(const std::string& statement, std::vector<std::string>& locals, std::vector<std::string>& body);

    Diagnostics& diagnostics_;
    CodegenOptions options_;
    SymbolTable symbols_;
    CodeWriter writer_;
    std::unordered_set<std::string> emittedNatives_;
    std::vector<std::pair<std::string, bool>> initializers_;
    std::vector<StructInitializerInfo> structInitializers_;
    std::vector<StructInfo> structs_;
    std::unordered_map<const Decl*, size_t> structIndexByDecl_;
    std::unordered_map<std::string, size_t, TransparentStringHash, TransparentStringEqual> structIndexByName_;
    ArrayShapeMap globalArrayShapes_;
    std::array<bool, 256> globalArrayShapeFirstChars_{};
    LocalTypeMap globalStructTypes_;
    mutable std::vector<FunctionInterfaceInfo> functionInterfaces_;
    mutable std::unordered_map<std::string, size_t, TransparentStringHash, TransparentStringEqual> functionInterfaceIndexByName_;
    mutable std::unordered_map<std::string, size_t> functionObjectInterfaceIndexBySignature_;
    std::vector<FunctionInfo> functions_;
    std::unordered_map<std::string, size_t, TransparentStringHash, TransparentStringEqual> functionIndexByName_;
    std::unordered_map<std::string, std::vector<std::string>> functionInterfaceParamTypesByFunction_;
    std::unordered_set<std::string, TransparentStringHash, TransparentStringEqual> functionArgumentLoweringCandidates_;
    std::vector<size_t> arrayStructIndexes_;
    std::vector<LambdaInfo> lambdas_;
    mutable std::string bodyCacheSignatureContextKeyCache_;
    std::unordered_map<const Decl*, std::vector<std::string>> processedZincFunctionBodies_;
    std::unordered_map<const MethodDecl*, std::vector<std::string>> processedZincMethodBodies_;
    size_t nextLambdaId_ = 1;
    size_t lambdasCodeContext_ = 0;
    size_t lambdasBoolexprContext_ = 0;
    mutable size_t lambdasFunctionInterfaceContext_ = 0;
    size_t lambdasNativeCallbackContext_ = 0;
    size_t lambdasMethodCallbackContext_ = 0;
    size_t lambdasUnknownContext_ = 0;
    size_t lambdasCapturing_ = 0;
    size_t lambdasRejected_ = 0;
    const StructInfo* lambdaContextStruct_ = nullptr;
    const Decl* lambdaContextContainer_ = nullptr;
    mutable size_t functionInterfaceCalls_ = 0;
    mutable size_t functionObjectCalls_ = 0;
    mutable size_t functionInterfaceMaxEvaluateDepth_ = 0;
    static constexpr size_t functionInterfaceEvaluateTempLimit_ = 8;
    std::unordered_map<std::string, long long> passTimings_;
    mutable CodegenPerformanceCounters performanceCounters_;
    mutable std::unordered_map<std::string, const StructInfo*, TransparentStringHash, TransparentStringEqual> structLookupCache_;
    mutable std::unordered_map<std::string, const FunctionInfo*, TransparentStringHash, TransparentStringEqual> functionLookupCache_;
    mutable std::unordered_map<std::string, bool, TransparentStringHash, TransparentStringEqual> arrayStructReceiverCache_;
    mutable std::unordered_map<std::string, std::string, TransparentStringHash, TransparentStringEqual> arrayRewriteCache_;
    mutable std::unordered_map<std::string, bool> structDeallocateCache_;
    mutable std::unordered_map<std::string, LineTokenCache> lineTokenCache_;
    mutable std::unordered_set<std::string> recordedFunctionDependencyEdges_;
    mutable bool lineFeatureCacheValid_ = false;
    mutable std::string lineFeatureCacheLine_;
    mutable const StructInfo* lineFeatureCacheStruct_ = nullptr;
    mutable const LocalTypeMap* lineFeatureCacheLocalTypes_ = nullptr;
    mutable const ArrayShapeMap* lineFeatureCacheLocalArrayShapes_ = nullptr;
    mutable LineFeatures lineFeatureCacheValue_;
    const Decl* mainFunction_ = nullptr;
    const Decl* mainContainer_ = nullptr;
};

} // namespace vjassc
