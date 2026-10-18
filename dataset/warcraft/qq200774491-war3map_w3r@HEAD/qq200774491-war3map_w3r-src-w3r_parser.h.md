---
type: vendor-doc
source_repo: "https://github.com/qq200774491/war3map_w3r"
source_path: "src/w3r_parser.h"
ref: HEAD
---

#pragma once

#include "binary_reader.h"

#include <array>
#include <string>
#include <vector>

namespace w3r {

struct W3RRegion {
  f32 left = 0.0f;
  f32 bottom = 0.0f;
  f32 right = 0.0f;
  f32 top = 0.0f;
  std::string name;
  u32 creation_number = 0;
  std::string weather_effect_id;
  std::string ambient_sound;
  std::array<u8, 4> color{}; // BGRA
};

class W3RParser {
public:
  W3RParser() = default;

  bool Load(const std::string &path);
  bool Load(const std::vector<u8> &data);
  bool Load(BinaryReader &reader);

  i32 version() const { return version_; }

  const std::vector<W3RRegion> &regions() const { return regions_; }

private:
  static std::string NormalizeFourCC(const std::string &value);

  i32 version_ = 0;
  std::vector<W3RRegion> regions_;
};

} // namespace w3r
