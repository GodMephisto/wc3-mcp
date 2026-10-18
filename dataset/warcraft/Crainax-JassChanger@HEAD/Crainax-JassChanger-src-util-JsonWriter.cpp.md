---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/util/JsonWriter.cpp"
ref: HEAD
---

#include "util/JsonWriter.h"

#include <iomanip>
#include <ostream>
#include <sstream>

namespace vjassc {

std::string jsonEscape(std::string_view text) {
    std::ostringstream out;
    for (unsigned char c : text) {
        switch (c) {
        case '\\':
            out << "\\\\";
            break;
        case '"':
            out << "\\\"";
            break;
        case '\n':
            out << "\\n";
            break;
        case '\r':
            out << "\\r";
            break;
        case '\t':
            out << "\\t";
            break;
        default:
            if (c < 0x20) {
                out << "\\u" << std::hex << std::setw(4) << std::setfill('0') << static_cast<int>(c);
            } else {
                out << c;
            }
            break;
        }
    }
    return out.str();
}

void writeJsonString(std::ostream& out, std::string_view text) {
    out << '"' << jsonEscape(text) << '"';
}

} // namespace vjassc
