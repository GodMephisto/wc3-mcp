---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/util/PathUtil.h"
ref: HEAD
---

#pragma once

#include <filesystem>
#include <string>
#include <string_view>
#include <vector>

namespace vjassc {

std::string trim(std::string_view text);
std::string ltrim(std::string_view text);
std::string rtrim(std::string_view text);
bool startsWithWord(std::string_view text, std::string_view word);
bool iequals(std::string_view a, std::string_view b);
std::filesystem::path weakCanonical(const std::filesystem::path& path);
std::string pathToGenericString(const std::filesystem::path& path);
std::vector<std::string> splitCommaListRespectingQuotes(std::string_view text);
std::string stripOuterQuotes(std::string text);

} // namespace vjassc
