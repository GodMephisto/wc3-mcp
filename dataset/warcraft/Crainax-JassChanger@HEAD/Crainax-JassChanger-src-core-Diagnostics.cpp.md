---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/core/Diagnostics.cpp"
ref: HEAD
---

#include "core/Diagnostics.h"

#include <iostream>

namespace vjassc {

const char* severityName(Severity severity) {
    switch (severity) {
    case Severity::Note:
        return "note";
    case Severity::Warning:
        return "warning";
    case Severity::Error:
        return "error";
    }
    return "error";
}

void Diagnostics::report(Severity severity, SourceLocation loc, std::string message, bool unsupported) {
    diagnostics_.push_back(Diagnostic{severity, loc, std::move(message), unsupported});
}

void Diagnostics::note(SourceLocation loc, std::string message) {
    report(Severity::Note, loc, std::move(message));
}

void Diagnostics::warning(SourceLocation loc, std::string message) {
    report(Severity::Warning, loc, std::move(message));
}

void Diagnostics::error(SourceLocation loc, std::string message) {
    report(Severity::Error, loc, std::move(message));
}

void Diagnostics::unsupported(SourceLocation loc, std::string feature) {
    report(Severity::Warning, loc, "current phase does not lower '" + feature + "'; counted as unsupported declaration", true);
}

bool Diagnostics::hasErrors() const {
    for (const auto& diag : diagnostics_) {
        if (diag.severity == Severity::Error) {
            return true;
        }
    }
    return false;
}

size_t Diagnostics::errorCount() const {
    size_t count = 0;
    for (const auto& diag : diagnostics_) {
        if (diag.severity == Severity::Error) {
            ++count;
        }
    }
    return count;
}

size_t Diagnostics::warningCount() const {
    size_t count = 0;
    for (const auto& diag : diagnostics_) {
        if (diag.severity == Severity::Warning) {
            ++count;
        }
    }
    return count;
}

size_t Diagnostics::unsupportedCount() const {
    size_t count = 0;
    for (const auto& diag : diagnostics_) {
        if (diag.unsupported) {
            ++count;
        }
    }
    return count;
}

void Diagnostics::print(const SourceManager& sources, std::ostream& out) const {
    for (const auto& diag : diagnostics_) {
        out << sources.describe(diag.loc) << ": " << severityName(diag.severity) << ": " << diag.message << '\n';
    }
}

} // namespace vjassc
