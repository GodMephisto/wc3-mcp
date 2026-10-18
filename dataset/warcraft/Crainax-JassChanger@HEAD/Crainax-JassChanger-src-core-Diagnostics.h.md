---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/core/Diagnostics.h"
ref: HEAD
---

#pragma once

#include "core/SourceManager.h"

#include <iosfwd>
#include <string>
#include <vector>

namespace vjassc {

enum class Severity {
    Note,
    Warning,
    Error,
};

struct Diagnostic {
    Severity severity = Severity::Error;
    SourceLocation loc;
    std::string message;
    bool unsupported = false;
};

class Diagnostics {
public:
    void report(Severity severity, SourceLocation loc, std::string message, bool unsupported = false);
    void note(SourceLocation loc, std::string message);
    void warning(SourceLocation loc, std::string message);
    void error(SourceLocation loc, std::string message);
    void unsupported(SourceLocation loc, std::string feature);

    bool hasErrors() const;
    size_t errorCount() const;
    size_t warningCount() const;
    size_t unsupportedCount() const;
    const std::vector<Diagnostic>& all() const { return diagnostics_; }
    void print(const SourceManager& sources, std::ostream& out) const;

private:
    std::vector<Diagnostic> diagnostics_;
};

const char* severityName(Severity severity);

} // namespace vjassc
