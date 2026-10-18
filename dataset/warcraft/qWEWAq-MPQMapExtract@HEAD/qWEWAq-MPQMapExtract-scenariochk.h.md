---
type: vendor-doc
source_repo: "https://github.com/qWEWAq/MPQMapExtract"
source_path: "scenariochk.h"
ref: HEAD
---

#ifndef SCENARIOCHK_H
#define SCENARIOCHK_H

#include <iostream>
#include <vector>
#include <string>
#include <cstdint>
#include <sstream>

struct Section {
    char name[4];             // 4-byte name
    uint32_t size;            // 4-byte section size
    std::vector<uint8_t> data; // Section data

    Section(const char* n, uint32_t s, const std::vector<uint8_t>& d);
};

class ScenarioChk {
public:
    explicit ScenarioChk(const std::string& input, bool isFreezed);
    const std::vector<Section>& getSections() const;
    std::string writeChunk() const;
    std::vector<Section> sections;
    void swapSections(std::vector<Section> parsedSecions);

private:
    void parseMTMXSection(const std::string& input);
    void parseSections(const std::string& input);
};

#endif // SCENARIOCHK_H
