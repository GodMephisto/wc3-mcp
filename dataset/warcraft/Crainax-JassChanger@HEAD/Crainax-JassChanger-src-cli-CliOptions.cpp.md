---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/cli/CliOptions.cpp"
ref: HEAD
---

#include "cli/CliOptions.h"

#include <iostream>
#include <utility>

namespace vjassc {
namespace {

bool needsValue(const std::string& arg) {
    return arg == "-o" || arg == "--emit-preprocessed" || arg == "--emit-tokens" ||
           arg == "--emit-ast" || arg == "--emit-expanded-ast" || arg == "--emit-stats" ||
           arg == "--emit-validation-report" || arg == "--emit-performance-report" ||
           arg == "--emit-dependency-report" ||
           arg == "--emit-generated-entity-plan" ||
           arg == "--emit-incremental-report" || arg == "--emit-incremental-state" ||
           arg == "--compare-incremental-state" || arg == "--experimental-incremental-cache" ||
           arg == "--incremental-mode" || arg == "--parallel-workers" ||
           arg == "--benchmark-repeat" || arg == "--benchmark-warmup" ||
           arg == "--emit-benchmark-report" || arg == "--pjass" || arg == "--common" ||
           arg == "--blizzard" || arg == "--compare-jasshelper" || arg == "--pjass-timeout-ms" ||
           arg == "--import-path" || arg == "--analyze-pjass-log" ||
           arg == "--validate-existing-output" || arg == "--emit-pjass-examples" ||
           arg == "--pjass-allow-external" || arg == "--allow-external-init" ||
           arg == "--mode";
}

} // namespace

const char* compileModeName(CompileMode mode) {
    switch (mode) {
    case CompileMode::Legacy:
        return "legacy";
    case CompileMode::Fast:
        return "fast";
    case CompileMode::Validate:
        return "validate";
    case CompileMode::FullValidation:
        return "full-validation";
    }
    return "legacy";
}

CliParseResult parseCli(int argc, char** argv) {
    CliParseResult result;
    auto& opt = result.options;

    for (int i = 1; i < argc; ++i) {
        std::string arg = argv[i];
        auto requireValue = [&](const std::string& name, std::filesystem::path& target) -> bool {
            if (i + 1 >= argc) {
                result.error = "missing value for " + name;
                return false;
            }
            target = argv[++i];
            return true;
        };

        if (arg == "--help" || arg == "-h") {
            opt.showHelp = true;
        } else if (arg == "--version") {
            opt.showVersion = true;
        } else if (arg == "--mode") {
            std::filesystem::path value;
            if (!requireValue(arg, value)) {
                return result;
            }
            std::string mode = value.string();
            if (mode == "fast") {
                opt.mode = CompileMode::Fast;
            } else if (mode == "validate") {
                opt.mode = CompileMode::Validate;
            } else if (mode == "full-validation") {
                opt.mode = CompileMode::FullValidation;
            } else {
                result.error = "invalid mode for --mode: " + mode + " (expected fast, validate, or full-validation)";
                return result;
            }
        } else if (arg == "-o") {
            if (!requireValue(arg, opt.outputPath)) {
                return result;
            }
        } else if (arg == "--debug") {
            opt.debugMode = true;
        } else if (arg == "--release") {
            opt.debugMode = false;
        } else if (arg == "-warn" || arg == "--warn") {
            opt.warnMode = true;
        } else if (arg == "--scan-only") {
            opt.scanOnly = true;
        } else if (arg == "--allow-unsupported") {
            opt.allowUnsupported = true;
        } else if (arg == "--check-output-syntax-lite") {
            opt.checkOutputSyntaxLite = true;
        } else if (arg == "--validate-pjass") {
            opt.validatePjass = true;
        } else if (arg == "--experimental-recorded-order") {
            opt.experimentalRecordedOrder = true;
        } else if (arg == "--experimental-parallel-lowering") {
            opt.experimentalParallelLowering = true;
        } else if (arg == "--experimental-body-jobs-single-thread") {
            opt.experimentalBodyJobsSingleThread = true;
        } else if (arg == "--parallel-workers") {
            std::filesystem::path value;
            if (!requireValue(arg, value)) {
                return result;
            }
            try {
                opt.parallelWorkers = static_cast<size_t>(std::stoull(value.string()));
            } catch (...) {
                result.error = "invalid numeric value for " + arg + ": " + value.string();
                return result;
            }
        } else if (arg == "--benchmark-repeat") {
            std::filesystem::path value;
            if (!requireValue(arg, value)) {
                return result;
            }
            try {
                opt.benchmarkRepeat = static_cast<size_t>(std::stoull(value.string()));
            } catch (...) {
                result.error = "invalid numeric value for " + arg + ": " + value.string();
                return result;
            }
            if (opt.benchmarkRepeat == 0) {
                result.error = "invalid numeric value for " + arg + ": " + value.string();
                return result;
            }
        } else if (arg == "--benchmark-warmup") {
            std::filesystem::path value;
            if (!requireValue(arg, value)) {
                return result;
            }
            try {
                opt.benchmarkWarmup = static_cast<size_t>(std::stoull(value.string()));
            } catch (...) {
                result.error = "invalid numeric value for " + arg + ": " + value.string();
                return result;
            }
        } else if (arg == "--emit-benchmark-report") {
            if (!requireValue(arg, opt.emitBenchmarkReportPath)) {
                return result;
            }
        } else if (arg == "--analyze-pjass-log") {
            if (!requireValue(arg, opt.analyzePjassLogPath)) {
                return result;
            }
        } else if (arg == "--validate-existing-output") {
            if (!requireValue(arg, opt.validateExistingOutputPath)) {
                return result;
            }
        } else if (arg == "--emit-pjass-examples") {
            std::filesystem::path value;
            if (!requireValue(arg, value)) {
                return result;
            }
            try {
                opt.emitPjassExamples = static_cast<size_t>(std::stoull(value.string()));
            } catch (...) {
                result.error = "invalid numeric value for " + arg + ": " + value.string();
                return result;
            }
        } else if (arg == "--pjass-allow-external" || arg == "--allow-external-init") {
            std::filesystem::path value;
            if (!requireValue(arg, value)) {
                return result;
            }
            std::string name = value.string();
            if (name.empty()) {
                result.error = "empty value for " + arg;
                return result;
            }
            opt.pjassAllowedExternalFunctions.push_back(std::move(name));
        } else if (arg == "--pjass") {
            if (!requireValue(arg, opt.pjassPath)) {
                return result;
            }
        } else if (arg == "--common") {
            if (!requireValue(arg, opt.commonPath)) {
                return result;
            }
        } else if (arg == "--blizzard") {
            if (!requireValue(arg, opt.blizzardPath)) {
                return result;
            }
        } else if (arg == "--emit-validation-report") {
            if (!requireValue(arg, opt.emitValidationReportPath)) {
                return result;
            }
        } else if (arg == "--emit-performance-report") {
            if (!requireValue(arg, opt.emitPerformanceReportPath)) {
                return result;
            }
        } else if (arg == "--emit-dependency-report") {
            if (!requireValue(arg, opt.emitDependencyReportPath)) {
                return result;
            }
        } else if (arg == "--emit-generated-entity-plan") {
            if (!requireValue(arg, opt.emitGeneratedEntityPlanPath)) {
                return result;
            }
        } else if (arg == "--emit-incremental-report") {
            if (!requireValue(arg, opt.emitIncrementalReportPath)) {
                return result;
            }
        } else if (arg == "--emit-incremental-state") {
            if (!requireValue(arg, opt.emitIncrementalStatePath)) {
                return result;
            }
        } else if (arg == "--compare-incremental-state") {
            if (!requireValue(arg, opt.compareIncrementalStatePath)) {
                return result;
            }
        } else if (arg == "--experimental-incremental-cache") {
            if (!requireValue(arg, opt.experimentalIncrementalCachePath)) {
                return result;
            }
        } else if (arg == "--incremental-mode") {
            std::filesystem::path value;
            if (!requireValue(arg, value)) {
                return result;
            }
            opt.incrementalMode = value.string();
            if (opt.incrementalMode != "report" && opt.incrementalMode != "reuse") {
                result.error = "invalid mode for --incremental-mode: " + opt.incrementalMode + " (expected report or reuse)";
                return result;
            }
        } else if (arg == "--compare-jasshelper") {
            if (!requireValue(arg, opt.compareJasshelperPath)) {
                return result;
            }
        } else if (arg == "--pjass-timeout-ms") {
            std::filesystem::path value;
            if (!requireValue(arg, value)) {
                return result;
            }
            try {
                opt.pjassTimeoutMs = std::stoll(value.string());
            } catch (...) {
                result.error = "invalid numeric value for " + arg + ": " + value.string();
                return result;
            }
            if (opt.pjassTimeoutMs <= 0) {
                result.error = "invalid numeric value for " + arg + ": " + value.string();
                return result;
            }
        } else if (arg == "--emit-preprocessed") {
            if (!requireValue(arg, opt.emitPreprocessedPath)) {
                return result;
            }
        } else if (arg == "--emit-tokens") {
            if (!requireValue(arg, opt.emitTokensPath)) {
                return result;
            }
        } else if (arg == "--emit-ast") {
            if (!requireValue(arg, opt.emitAstPath)) {
                return result;
            }
        } else if (arg == "--emit-expanded-ast") {
            if (!requireValue(arg, opt.emitExpandedAstPath)) {
                return result;
            }
        } else if (arg == "--emit-stats") {
            if (!requireValue(arg, opt.emitStatsPath)) {
                return result;
            }
        } else if (arg == "--import-path") {
            std::filesystem::path path;
            if (!requireValue(arg, path)) {
                return result;
            }
            opt.importPaths.push_back(path);
        } else if (!arg.empty() && arg[0] == '-') {
            result.error = "unknown option: " + arg;
            return result;
        } else if (!opt.inputPath.empty()) {
            result.error = "multiple input files provided: " + opt.inputPath.string() + " and " + arg;
            return result;
        } else {
            opt.inputPath = arg;
        }

        if (needsValue(arg) && i < argc && std::string(argv[i]).empty()) {
            result.error = "empty value for " + arg;
            return result;
        }
    }

    const bool offlineMode = !opt.analyzePjassLogPath.empty() || !opt.validateExistingOutputPath.empty();
    if (!opt.showHelp && !opt.showVersion && !offlineMode && opt.inputPath.empty()) {
        result.error = "missing input file";
        return result;
    }

    if (!opt.scanOnly && !opt.showHelp && !opt.showVersion && !offlineMode && opt.outputPath.empty()) {
            const bool emitsOnly = !opt.emitPreprocessedPath.empty() || !opt.emitTokensPath.empty() ||
                               !opt.emitAstPath.empty() || !opt.emitExpandedAstPath.empty() ||
                               !opt.emitStatsPath.empty() || !opt.emitPerformanceReportPath.empty() ||
                               !opt.emitDependencyReportPath.empty() ||
                               !opt.emitGeneratedEntityPlanPath.empty() ||
                               !opt.emitIncrementalReportPath.empty() || !opt.emitIncrementalStatePath.empty() ||
                               !opt.emitBenchmarkReportPath.empty();
        if (!emitsOnly) {
            result.error = "missing output path; use -o <output.j> or --scan-only";
            return result;
        }
    }

    result.ok = true;
    return result;
}

void printHelp(std::ostream& out) {
    out << "vjassc phase23 - vJass/Zinc to JASS compiler prototype\n"
        << "\n"
        << "Usage:\n"
        << "  vjassc <input.j> -o <output.j> [--debug|--release]\n"
        << "  vjassc <input.j> --scan-only [--allow-unsupported]\n"
        << "\n"
        << "Options:\n"
        << "  -o <path>                    Write generated JASS\n"
        << "  --mode <mode>                fast, validate, or full-validation validation profile\n"
        << "  --debug                      Keep debug lines and strip the debug prefix\n"
        << "  --release                    Drop debug lines (default)\n"
        << "  -warn, --warn                Emit runtime struct allocation/destroy warnings\n"
        << "  --scan-only                  Read, preprocess, lex, parse, and emit diagnostics/stats only\n"
        << "  --emit-preprocessed <path>   Write preprocessed logical source\n"
        << "  --emit-tokens <path>         Write token stream\n"
        << "  --emit-ast <path>            Write AST dump\n"
        << "  --emit-expanded-ast <path>   Write AST after module expansion\n"
        << "  --emit-stats <path>          Write JSON statistics\n"
        << "  --emit-validation-report <path> Write JSON validation report\n"
        << "  --emit-performance-report <path> Write performance JSON report\n"
        << "  --emit-dependency-report <path> Write dependency recorder JSON report\n"
        << "  --emit-generated-entity-plan <path> Write deterministic generated entity plan JSON\n"
        << "  --emit-incremental-report <path> Write read-only incremental chunk reuse report\n"
        << "  --emit-incremental-state <path> Write read-only incremental chunk state\n"
        << "  --compare-incremental-state <path> Compare incremental report against a prior state\n"
        << "  --experimental-recorded-order Use recorded dependency edges for function ordering\n"
        << "  --experimental-parallel-lowering Enable deterministic parallel-lowering experiment metadata\n"
        << "  --experimental-body-jobs-single-thread Route through the deterministic body job model\n"
        << "  --parallel-workers <n>        Worker count for experimental parallel lowering metadata\n"
        << "  --experimental-incremental-cache <dir> Enable experimental incremental cache directory\n"
        << "  --incremental-mode <mode>     report or reuse for experimental incremental cache\n"
        << "  --benchmark-repeat <n>        Benchmark repeat metadata for external benchmark scripts\n"
        << "  --benchmark-warmup <n>        Benchmark warmup metadata for external benchmark scripts\n"
        << "  --emit-benchmark-report <path> Write single-run benchmark-compatible JSON report\n"
        << "  --import-path <dir>          Add import search directory; may be repeated\n"
        << "  --allow-unsupported          Allow unsupported declarations during scan-only\n"
        << "  --check-output-syntax-lite   Fail if generated output still contains known high-level syntax\n"
        << "  --validate-pjass             Validate generated output with PJASS\n"
        << "  --analyze-pjass-log <path>   Parse an existing PJASS log without codegen\n"
        << "  --validate-existing-output <path> Validate an existing generated JASS file without codegen\n"
        << "  --emit-pjass-examples <n>    Include up to n examples per PJASS group in validation reports\n"
        << "  --pjass-allow-external <name> Add a validation-only noarg/nothing PJASS stub before the generated script; may be repeated\n"
        << "  --allow-external-init <name> Alias for --pjass-allow-external\n"
        << "  --pjass <path>               Path to pjass executable\n"
        << "  --common <path>              Path to common.j for PJASS\n"
        << "  --blizzard <path>            Path to blizzard.j for PJASS\n"
        << "  --pjass-timeout-ms <number>  PJASS timeout metadata value (default: 30000)\n"
        << "  --compare-jasshelper <path>  Compare generated output structure to JassHelper output\n"
        << "  --version                    Print version\n"
        << "  --help                       Print this help\n";
}

void printVersion(std::ostream& out) {
    out << "vjassc phase23 0.23.1\n";
}

} // namespace vjassc
