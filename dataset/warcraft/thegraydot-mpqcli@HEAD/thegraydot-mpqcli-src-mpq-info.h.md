---
type: vendor-doc
source_repo: "https://github.com/thegraydot/mpqcli"
source_path: "src/mpq/info.h"
ref: HEAD
---

#ifndef MPQ_INFO_H
#define MPQ_INFO_H

#include <optional>
#include <ostream>
#include <string>

#include <StormLib.h>

namespace mpqcli {

void PrintMpqInfo(HANDLE archive, const std::optional<std::string> &info_property,
                  std::ostream &out);

} // namespace mpqcli

#endif
