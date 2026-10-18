---
type: vendor-doc
source_repo: "https://github.com/qq200774491/war3map_w3r"
source_path: "src/w3r_parser.cpp"
ref: HEAD
---

#include "w3r_parser.h"

#include <iostream>

namespace w3r {

std::string W3RParser::NormalizeFourCC(const std::string &value) {
  std::string out = value;
  while (!out.empty() && out.back() == '\0') {
    out.pop_back();
  }
  return out;
}

bool W3RParser::Load(const std::string &path) {
  try {
    auto data = LoadBinaryFile(path);
    return Load(data);
  } catch (const std::exception &e) {
    std::cerr << "W3RParser: Failed to open file: " << path << " - " << e.what()
              << std::endl;
    return false;
  }
}

bool W3RParser::Load(const std::vector<u8> &data) {
  BinaryReader reader(data);
  return Load(reader);
}

bool W3RParser::Load(BinaryReader &reader) {
  regions_.clear();

  try {
    version_ = reader.readI32();
    const u32 region_count = reader.readU32();
    regions_.reserve(region_count);

    for (u32 i = 0; i < region_count; ++i) {
      W3RRegion region{};
      region.left = reader.readF32();
      region.bottom = reader.readF32();
      region.right = reader.readF32();
      region.top = reader.readF32();
      region.name = reader.readNullTerminatedString();
      region.creation_number = reader.readU32();
      region.weather_effect_id = NormalizeFourCC(reader.readString(4));
      region.ambient_sound = reader.readNullTerminatedString();
      reader.readBytes(region.color.data(), region.color.size());

      regions_.push_back(std::move(region));
    }
  } catch (const std::exception &e) {
    std::cerr << "W3RParser: Parse failed: " << e.what() << std::endl;
    return false;
  }

  return true;
}

} // namespace w3r
