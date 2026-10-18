---
type: vendor-doc
source_repo: "https://github.com/thegraydot/mpqcli"
source_path: "src/mpq/read.h"
ref: HEAD
---

#ifndef MPQ_READ_H
#define MPQ_READ_H

#include <memory>
#include <ostream>

#include <StormLib.h>

namespace mpqcli {

std::unique_ptr<char[]> ReadArchivedFile(HANDLE archive, const char *file_name,
                                         unsigned int *file_size, LCID preferred_locale,
                                         std::ostream &err);

} // namespace mpqcli

#endif
