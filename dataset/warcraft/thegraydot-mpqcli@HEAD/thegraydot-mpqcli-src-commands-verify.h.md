---
type: vendor-doc
source_repo: "https://github.com/thegraydot/mpqcli"
source_path: "src/commands/verify.h"
ref: HEAD
---

#ifndef COMMANDS_VERIFY_H
#define COMMANDS_VERIFY_H

#include <filesystem>
#include <ostream>

namespace mpqcli {

/// Settled arguments for the verify subcommand
struct VerifyOptions {
    std::filesystem::path archive;
    bool print_signature = false;
};

/// Checks the archive's signature, returning false when it is missing or invalid
bool Verify(const VerifyOptions &options, std::ostream &out, std::ostream &err);

} // namespace mpqcli

#endif
