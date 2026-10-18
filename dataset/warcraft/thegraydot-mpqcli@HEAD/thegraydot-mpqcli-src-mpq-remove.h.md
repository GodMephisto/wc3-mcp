---
type: vendor-doc
source_repo: "https://github.com/thegraydot/mpqcli"
source_path: "src/mpq/remove.h"
ref: HEAD
---

#ifndef MPQ_REMOVE_H
#define MPQ_REMOVE_H

#include <ostream>
#include <string>

#include <StormLib.h>

namespace mpqcli {

int RemoveFile(HANDLE archive, const std::string &archive_file_path, LCID locale,
               std::ostream &err);

} // namespace mpqcli

#endif
