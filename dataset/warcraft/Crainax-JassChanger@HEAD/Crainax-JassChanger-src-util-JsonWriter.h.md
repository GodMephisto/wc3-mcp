---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/util/JsonWriter.h"
ref: HEAD
---

#pragma once

#include <iosfwd>
#include <string>
#include <string_view>

namespace vjassc {

std::string jsonEscape(std::string_view text);
void writeJsonString(std::ostream& out, std::string_view text);

} // namespace vjassc
