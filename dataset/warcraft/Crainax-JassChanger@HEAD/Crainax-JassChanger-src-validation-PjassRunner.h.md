---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/validation/PjassRunner.h"
ref: HEAD
---

#pragma once

#include <filesystem>
#include <string>
#include <unordered_map>
#include <vector>

namespace vjassc {

struct PjassOptions {
    std::filesystem::path pjassPath;
    std::filesystem::path commonPath;
    std::filesystem::path blizzardPath;
    std::filesystem::path scriptPath;
    std::filesystem::path stdoutPath;
    std::filesystem::path stderrPath;
    std::vector<std::string> allowedExternalFunctions;
    long long timeoutMs = 30000;
    size_t exampleLimit = 20;
};

struct PjassErrorExample {
    size_t generatedLine = 0;
    std::string functionName;
    std::string generatedText;
    std::string message;
    std::vector<std::string> excerpt;
};

struct PjassErrorGroup {
    std::string kind;
    size_t count = 0;
    size_t firstLine = 0;
    std::string firstMessage;
    std::vector<PjassErrorExample> examples;
};

struct PjassResult {
    bool requested = false;
    bool ran = false;
    bool ok = false;
    int exitCode = -1;
    std::string commandLine;
    std::filesystem::path stdoutPath;
    std::filesystem::path stderrPath;
    std::filesystem::path validationEnvStubPath;
    std::string stdoutText;
    std::string stderrText;
    std::string error;
    long long elapsedMs = 0;
    std::vector<std::string> allowedExternalFunctions;
    std::unordered_map<std::string, size_t> errorSummary;
    std::vector<PjassErrorGroup> errorGroups;
};

struct PjassResolvedPaths {
    bool ok = false;
    std::string error;
    std::filesystem::path pjassPath;
    std::filesystem::path commonPath;
    std::filesystem::path blizzardPath;
};

PjassResolvedPaths resolvePjassPaths(const std::filesystem::path& cwd,
                                     const std::filesystem::path& explicitPjass,
                                     const std::filesystem::path& explicitCommon,
                                     const std::filesystem::path& explicitBlizzard);
PjassResult runPjass(const PjassOptions& options);
std::unordered_map<std::string, size_t> classifyPjassErrors(const std::string& text);
std::vector<PjassErrorGroup> groupPjassErrors(const std::string& text,
                                              const std::string& generatedOutput,
                                              size_t exampleLimit = 20);

} // namespace vjassc
