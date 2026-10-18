---
type: vendor-doc
source_repo: "https://github.com/qq200774491/war3map_w3r"
source_path: "src/main.cpp"
ref: HEAD
---

#include "lua_generator.h"
#include "w3r_parser.h"

#include <filesystem>
#include <iostream>
#include <string>

namespace fs = std::filesystem;

static void PrintUsage(const char *program) {
  std::cerr << "Usage: " << program << " <input.w3r> [output.lua]" << std::endl;
  std::cerr << std::endl;
  std::cerr << "  Parse a Warcraft III war3map.w3r region file and generate"
            << std::endl;
  std::cerr << "  a Lua data file with LuaCATS type annotations." << std::endl;
  std::cerr << std::endl;
  std::cerr << "  If output path is omitted, writes to <input_stem>.lua"
            << std::endl;
}

int main(int argc, char *argv[]) {
  if (argc < 2) {
    PrintUsage(argv[0]);
    return 1;
  }

  std::string input_path = argv[1];
  std::string output_path;

  if (argc >= 3) {
    output_path = argv[2];
  } else {
    fs::path p(input_path);
    output_path = (p.parent_path() / p.stem()).string() + ".lua";
  }

  std::cout << "Parsing: " << input_path << std::endl;

  w3r::W3RParser parser;
  if (!parser.Load(input_path)) {
    std::cerr << "Error: Failed to parse " << input_path << std::endl;
    return 1;
  }

  std::cout << "Version: " << parser.version() << std::endl;
  std::cout << "Regions: " << parser.regions().size() << std::endl;

  w3r::LuaGenerator generator(parser);
  if (!generator.WriteToFile(output_path)) {
    std::cerr << "Error: Failed to write " << output_path << std::endl;
    return 1;
  }

  std::cout << "Output:  " << output_path << std::endl;
  return 0;
}
