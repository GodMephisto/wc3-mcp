---
type: vendor-doc
source_repo: "https://github.com/qq200774491/war3map_w3r"
source_path: "src/lua_generator.cpp"
ref: HEAD
---

#include "lua_generator.h"

#include <cmath>
#include <fstream>
#include <iomanip>
#include <iostream>
#include <sstream>

namespace w3r {

LuaGenerator::LuaGenerator(const W3RParser &parser) : parser_(parser) {}

std::string LuaGenerator::Generate() const {
  std::string out;
  out.reserve(4096);

  WriteHeader(out);
  WriteClassDef(out);
  WriteRegions(out);

  out += "return regions\n";
  return out;
}

bool LuaGenerator::WriteToFile(const std::string &path) const {
  std::ofstream file(path, std::ios::out | std::ios::trunc);
  if (!file.is_open()) {
    std::cerr << "LuaGenerator: Failed to open output file: " << path
              << std::endl;
    return false;
  }

  file << Generate();
  file.close();
  return true;
}

void LuaGenerator::WriteHeader(std::string &out) const {
  out += "-- Generated from: war3map.w3r (version ";
  out += std::to_string(parser_.version());
  out += ")\n\n";
}

void LuaGenerator::WriteClassDef(std::string &out) const {
  out += "---@class W3RColor\n";
  out += "---@field b integer \xe8\x93\x9d\xe8\x89\xb2\xe9\x80\x9a\xe9\x81\x93 (0-255)\n";
  out += "---@field g integer \xe7\xbb\xbf\xe8\x89\xb2\xe9\x80\x9a\xe9\x81\x93 (0-255)\n";
  out += "---@field r integer \xe7\xba\xa2\xe8\x89\xb2\xe9\x80\x9a\xe9\x81\x93 (0-255)\n";
  out += "---@field a integer \xe9\x80\x8f\xe6\x98\x8e\xe5\xba\xa6\xe9\x80\x9a\xe9\x81\x93 (0-255)\n";
  out += "\n";

  out += "---@class W3RRegion\n";
  out += "---@field left number \xe5\xb7\xa6\xe8\xbe\xb9\xe7\x95\x8c\xe5\x9d\x90\xe6\xa0\x87\n";
  out += "---@field bottom number \xe4\xb8\x8b\xe8\xbe\xb9\xe7\x95\x8c\xe5\x9d\x90\xe6\xa0\x87\n";
  out += "---@field right number \xe5\x8f\xb3\xe8\xbe\xb9\xe7\x95\x8c\xe5\x9d\x90\xe6\xa0\x87\n";
  out += "---@field top number \xe4\xb8\x8a\xe8\xbe\xb9\xe7\x95\x8c\xe5\x9d\x90\xe6\xa0\x87\n";
  out += "---@field name string \xe5\x8c\xba\xe5\x9f\x9f\xe5\x90\x8d\xe7\xa7\xb0\n";
  out += "---@field creation_number integer \xe5\x8c\xba\xe5\x9f\x9f\xe5\x94\xaf\xe4\xb8\x80\xe6\xa0\x87\xe8\xaf\x86\xe7\xac\xa6\n";
  out += "---@field weather_effect_id string \xe5\xa4\xa9\xe6\xb0\x94\xe6\x95\x88\xe6\x9e\x9c FourCC \xe7\xbc\x96\xe7\xa0\x81\n";
  out += "---@field ambient_sound string \xe7\x8e\xaf\xe5\xa2\x83\xe9\x9f\xb3\xe6\x95\x88\xe5\xbc\x95\xe7\x94\xa8 ID\n";
  out += "---@field color W3RColor \xe7\xbc\x96\xe8\xbe\x91\xe5\x99\xa8\xe6\x98\xbe\xe7\xa4\xba\xe9\xa2\x9c\xe8\x89\xb2 (BGRA)\n";
  out += "\n";
}

void LuaGenerator::WriteRegions(std::string &out) const {
  const auto &regions = parser_.regions();

  out += "---@type W3RRegion[]\n";
  out += "local regions = {\n";

  for (size_t i = 0; i < regions.size(); ++i) {
    bool last = (i == regions.size() - 1);
    WriteRegion(out, regions[i], last);
  }

  out += "}\n\n";
}

void LuaGenerator::WriteRegion(std::string &out, const W3RRegion &region,
                               bool last) const {
  out += "    {\n";
  out += "        left = " + FormatFloat(region.left) + ",\n";
  out += "        bottom = " + FormatFloat(region.bottom) + ",\n";
  out += "        right = " + FormatFloat(region.right) + ",\n";
  out += "        top = " + FormatFloat(region.top) + ",\n";
  out += "        name = \"" + EscapeLuaString(region.name) + "\",\n";
  out += "        creation_number = " + std::to_string(region.creation_number) +
         ",\n";
  out += "        weather_effect_id = \"" +
         EscapeLuaString(region.weather_effect_id) + "\",\n";
  out += "        ambient_sound = \"" + EscapeLuaString(region.ambient_sound) +
         "\",\n";
  out += "        color = { b = " + std::to_string(region.color[0]) +
         ", g = " + std::to_string(region.color[1]) +
         ", r = " + std::to_string(region.color[2]) +
         ", a = " + std::to_string(region.color[3]) + " },\n";
  out += "    }";
  out += last ? "\n" : ",\n";
}

std::string LuaGenerator::EscapeLuaString(const std::string &str) {
  std::string result;
  result.reserve(str.size());
  for (char c : str) {
    switch (c) {
    case '\\':
      result += "\\\\";
      break;
    case '"':
      result += "\\\"";
      break;
    case '\n':
      result += "\\n";
      break;
    case '\r':
      result += "\\r";
      break;
    case '\t':
      result += "\\t";
      break;
    case '\0':
      break;
    default:
      result += c;
      break;
    }
  }
  return result;
}

std::string LuaGenerator::FormatFloat(f32 value) {
  if (std::floor(value) == value && std::isfinite(value)) {
    std::ostringstream oss;
    oss << std::fixed << std::setprecision(1) << value;
    return oss.str();
  }
  std::ostringstream oss;
  oss << value;
  return oss.str();
}

} // namespace w3r
