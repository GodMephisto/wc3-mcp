---
type: vendor-doc
source_repo: "https://github.com/qq200774491/war3map_w3r"
source_path: "src/lua_generator.h"
ref: HEAD
---

#pragma once

#include "w3r_parser.h"

#include <string>

namespace w3r {

class LuaGenerator {
public:
  explicit LuaGenerator(const W3RParser &parser);

  std::string Generate() const;
  bool WriteToFile(const std::string &path) const;

private:
  void WriteHeader(std::string &out) const;
  void WriteClassDef(std::string &out) const;
  void WriteRegions(std::string &out) const;
  void WriteRegion(std::string &out, const W3RRegion &region, bool last) const;

  static std::string EscapeLuaString(const std::string &str);
  static std::string FormatFloat(f32 value);

  const W3RParser &parser_;
};

} // namespace w3r
