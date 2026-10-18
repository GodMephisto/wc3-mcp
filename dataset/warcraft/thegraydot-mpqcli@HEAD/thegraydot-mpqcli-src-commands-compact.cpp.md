---
type: vendor-doc
source_repo: "https://github.com/thegraydot/mpqcli"
source_path: "src/commands/compact.cpp"
ref: HEAD
---

#include "commands/compact.h"

#include <ostream>

#include "mpq/archive.h"
#include "mpq/compact.h"

namespace mpqcli {

void Compact(const CompactOptions &options, std::ostream &err) {
    Archive archive = Archive::Open(options.archive, 0);
    CompactMpqArchive(archive.Handle(), options.listfile, err);
    archive.Close();
}

} // namespace mpqcli
