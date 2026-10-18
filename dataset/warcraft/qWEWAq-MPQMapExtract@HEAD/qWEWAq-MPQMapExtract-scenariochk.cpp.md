---
type: vendor-doc
source_repo: "https://github.com/qWEWAq/MPQMapExtract"
source_path: "scenariochk.cpp"
ref: HEAD
---

#include "scenariochk.h"
#include "mpqread.h"
#include <cstring>
#include <cctype>
#include <locale>
#include <cstdint>
#include <iostream>
#include <vector>
#include <iomanip>
#include <sstream>
bool DEBUG = false;

Section::Section(const char* n, uint32_t s, const std::vector<uint8_t>& d) : size(s), data(d) {
    std::memcpy(name, n, 4);
}

ScenarioChk::ScenarioChk(const std::string& input, bool isFreezed = true) {
    if (isFreezed) {
        parseMTMXSection(input);
        // processSections();
    }
    else parseSections(input);
}

// Function to parse sections from the input string
void ScenarioChk::parseSections(const std::string& input) {
    std::istringstream is(input, std::ios::binary);
    try {
        while (is) {
            char name[5];
            is.read(name, 4);
            if (is.eof()) break;
            name[4] = '\0';

            if (!is) break;
            if (is.gcount() != 4)  { throw std::runtime_error("Incomplete read for type name"); }

            uint32_t size;
            is.read(reinterpret_cast<char*>(&size), sizeof(size));
            std::vector<uint8_t> data(size);
            if (size > 0) {
                is.read(reinterpret_cast<char*>(data.data()), size);
                if (is.eof()) break;
                if (!is) throw std::runtime_error("Failed to read section data");
            }

            sections.emplace_back(name, size, data);
        }
    } catch (const std::runtime_error& e) {
        std::cerr << "Error Ocurred in ScenarioChk: " << e.what() << std::endl;
    } catch (const std::bad_alloc& e) {
        std::cerr << "Error Ocurred for malloc in ScenarioChk: " << e.what() << std::endl;
    }
}

// Function to parse sections from the input string
void ScenarioChk::parseMTMXSection(const std::string& input) {
    std::istringstream is(input, std::ios::binary);
    try {
        while (is) {
            // if (testCount > 50) break
            char name[5];
            is.read(name, 4);
            if (is.eof()) break;
            if (is.gcount() != 4) throw std::runtime_error("Incomplete read for type name");
            name[4] = '\0';

            uint32_t size;

            if (isFreezed()) {
                if (strncmp(name, "ISOM", 4) == 0) {
                    is.read(reinterpret_cast<char*>(&size), sizeof(size));
                    continue;
                }
            }
            if (strncmp(name, "STUB", 4) == 0) break;

            is.read(reinterpret_cast<char*>(&size), sizeof(size));
            std::vector<uint8_t> data(size);
            if (size > 0) {
                is.read(reinterpret_cast<char*>(data.data()), size);
                if (is.eof()) break;
                if (!is) throw std::runtime_error("Failed to read section data");
            }

            if (strncmp(name, "VER ", 4) == 0 ||
                strncmp(name, "VCOD", 4) == 0 ||
                strncmp(name, "OWNR", 4) == 0 ||
                strncmp(name, "SIDE", 4) == 0 ||
                strncmp(name, "COLR", 4) == 0 ||
                strncmp(name, "ERA ", 4) == 0 ||
                strncmp(name, "DIM ", 4) == 0 ||
                strncmp(name, "MTXM", 4) == 0 ||
                strncmp(name, "UNIT", 4) == 0 ||
                strncmp(name, "PUNI", 4) == 0 ||
                strncmp(name, "UNIx", 4) == 0 ||
                strncmp(name, "PUPx", 4) == 0 ||
                strncmp(name, "UPGx", 4) == 0 ||
                strncmp(name, "THG2", 4) == 0 ||
                strncmp(name, "MASK", 4) == 0 ||
                //// strncmp(name, "MRGN", 4) == 0 ||
                strncmp(name, "SPRP", 4) == 0 ||
                strncmp(name, "SIDE", 4) == 0 ||
                strncmp(name, "FORC", 4) == 0 ||
                strncmp(name, "PTEx", 4) == 0 ||
                strncmp(name, "TECx", 4) == 0 ||
                strncmp(name, "MBRF", 4) == 0 ||
                //// strncmp(name, "STRx", 4) == 0 ||
                //// strncmp(name, "TRIG", 4) == 0 ||
                strncmp(name, "UPRP", 4) == 0) {
                    sections.emplace_back(name, size, data);
            }
        }
    }
    catch (const std::runtime_error& e) {
        std::cout << "Error Ocurred in ScenarioChk: " << e.what() << std::endl;
    }
    catch (const std::bad_alloc& e) {
        std::cerr << "Error Ocurred for malloc in ScenarioChk: " << e.what() << std::endl;
    }
}

void ScenarioChk::swapSections(std::vector<Section> parsedSecions) {
    Section* SectionMTXM = nullptr;
    for(auto& section: sections) {
        for (auto& parsedsection : parsedSecions) {
            if (strcmp(section.name, parsedsection.name) == 0) {
                std::swap(section.size, parsedsection.size);
                std::swap(section.data, parsedsection.data);

                if (strcmp(parsedsection.name, "MTXM") == 0) {
                    // TODO: this does not handle doodad...
                    // Save MTXM data for TILE section
                    SectionMTXM = new Section("MTXM", section.size, section.data);
                }
            }

            
        }
    }

    // why loop serveral, but who cares
    for (auto& parsedsection : parsedSecions) {
        bool not_found = true;
        for (auto& section : sections) {
            if (strcmp(section.name, parsedsection.name) == 0) {
                not_found = false;
            }
        }
        if (not_found &&
            (strncmp(parsedsection.name, "VER ", 4) == 0) ||
            (strncmp(parsedsection.name, "VCOD", 4) == 0) ||
            (strncmp(parsedsection.name, "OWNR", 4) == 0) ||
            (strncmp(parsedsection.name, "SIDE", 4) == 0) ||
            (strncmp(parsedsection.name, "COLR", 4) == 0) ||
            (strncmp(parsedsection.name, "ERA ", 4) == 0) ||
            (strncmp(parsedsection.name, "DIM ", 4) == 0) ||
            (strncmp(parsedsection.name, "FORC", 4) == 0) ||
            (strncmp(parsedsection.name, "PTEx", 4) == 0) ||
            (strncmp(parsedsection.name, "TECx", 4) == 0) ||
            (strncmp(parsedsection.name, "MBRF", 4) == 0) ||
            (strncmp(parsedsection.name, "UPRP", 4) == 0)) {
                sections.push_back(parsedsection);
                not_found = true;
        }
    }

    for (auto& section : sections) {
        if (strcmp(section.name, "TILE") == 0) {
            if(SectionMTXM == nullptr) throw std::runtime_error("No MXTM section in scx file (WHAT?)");
            else section.data = SectionMTXM->data;
        }
    }
}

const std::vector<Section>& ScenarioChk::getSections() const {
    return sections;
}

std::string ScenarioChk::writeChunk() const {
    std::ostringstream os(std::ios::binary);
    for (const auto& section : sections) {
        os.write(section.name, 4);
        os.write(reinterpret_cast<const char*>(&section.size), sizeof(section.size));
        if (!section.data.empty()) {
            os.write(reinterpret_cast<const char*>(section.data.data()), section.size);
        }
    }
    return os.str();
}
