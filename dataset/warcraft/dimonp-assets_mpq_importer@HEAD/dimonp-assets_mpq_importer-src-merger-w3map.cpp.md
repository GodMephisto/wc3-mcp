---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/merger/w3map.cpp"
ref: HEAD
---

#include <string>
#include <unordered_map>
#include <gsl/gsl>

#include "merger.hpp"

namespace assmpq::merger {

struct CliffKey {
    uint8_t la0, la1, la2, la3, var;
};
struct RampKey {
    uint8_t la0, la1, la2, la3, var, par;
};


// NOLINTBEGIN(cppcoreguidelines-avoid-magic-numbers, readability-magic-numbers)
static inline
auto pack_cliff_key(uint8_t la0, uint8_t la1, uint8_t la2, uint8_t la3, uint8_t var) -> uint32_t
{
    return static_cast<uint32_t>( la0 & 0b11)
         | static_cast<uint32_t>((la1 & 0b11) << 2)
         | static_cast<uint32_t>((la2 & 0b11) << 4)
         | static_cast<uint32_t>((la3 & 0b11) << 6)
         | static_cast<uint32_t>((var & 0b11) << 8);
}

static inline
auto unpack_cliff_key(uint32_t key) -> CliffKey
{
    return {
        .la0  = static_cast<uint8_t>( key       & 0b11),
        .la1  = static_cast<uint8_t>((key >> 2) & 0b11),
        .la2  = static_cast<uint8_t>((key >> 4) & 0b11),
        .la3  = static_cast<uint8_t>((key >> 6) & 0b11),
        .var =  static_cast<uint8_t>((key >> 8) & 0b11)
    };
}

static inline
auto pack_ramp_key(uint8_t la0, uint8_t la1, uint8_t la2, uint8_t la3, uint8_t var, uint8_t par) -> uint32_t
{
    return static_cast<uint32_t>( la0  & 0b1111)
         | static_cast<uint32_t>((la1  & 0b1111) << 4)
         | static_cast<uint32_t>((la2  & 0b1111) << 8)
         | static_cast<uint32_t>((la3  & 0b1111) << 12)
         | static_cast<uint32_t>((var  & 0b1111) << 16)
         | static_cast<uint32_t>((par  & 0b1111) << 20);
}

static inline
auto unpack_ramp_key(uint32_t key) -> RampKey
{
    return {
        .la0 = static_cast<uint8_t>( key        & 0b1111),
        .la1 = static_cast<uint8_t>((key >> 4)  & 0b1111),
        .la2 = static_cast<uint8_t>((key >> 8)  & 0b1111),
        .la3 = static_cast<uint8_t>((key >> 12) & 0b1111),
        .var = static_cast<uint8_t>((key >> 16) & 0b1111),
        .par = static_cast<uint8_t>((key >> 20) & 0b1111)
    };
}
// NOLINTEND(cppcoreguidelines-avoid-magic-numbers, readability-magic-numbers)

static inline
auto decode_geo_char(char char_code) -> uint8_t
{
    static const std::unordered_map<char, uint8_t> char_to_code = {
        { '0', '\0' }, { 'a', '\0' }, { 'A', '\0' },
        { '1', '\1' }, { 'b', '\1' }, { 'B', '\1' },
        { '2', '\2' }, { 'c', '\2' }, { 'C', '\2' },
        { '3', '\3' },
        { 'l', '\4' }, { 'L', '\4' },
        { 'h', '\5' }, { 'H', '\5' },
        { 'x', '\6' }, { 'X', '\6' },
    };
    return char_to_code.at(char_code);
}

static inline
auto encode_geo_char(uint8_t code) -> char
{
    static const std::unordered_map<uint8_t, char> code_to_char = {
        { '\0', 'A' },
        { '\1', 'B' },
        { '\2', 'C' },
        { '\3', 'L' },
        { '\4', 'H' },
        { '\5', 'X' },
    };
    return code_to_char.at(code);
}

auto get_cliff_key_from_geo_name(const std::string &str_key) -> uint32_t
{
    Ensures(str_key.length() >= 5);
    return pack_cliff_key(
        decode_geo_char(static_cast<char>(str_key[3])),
        decode_geo_char(static_cast<char>(str_key[2])),
        decode_geo_char(static_cast<char>(str_key[1])),
        decode_geo_char(static_cast<char>(str_key[0])),
        decode_geo_char(static_cast<char>(str_key[4]))
    );
}

auto convert_cliff_key_to_string(uint32_t key) -> std::string
{
    const CliffKey s_key = unpack_cliff_key(key);
    return std::string {
        encode_geo_char(s_key.la3),
        encode_geo_char(s_key.la2),
        encode_geo_char(s_key.la1),
        encode_geo_char(s_key.la0),
        static_cast<char>('0' + s_key.var),
    };
}

auto get_ramp_key_from_geo_name(const std::string &str_key, uint8_t part) -> uint32_t
{
    Ensures(str_key.length() >= 5);
    return pack_ramp_key(
        decode_geo_char(static_cast<char>(str_key[3])),
        decode_geo_char(static_cast<char>(str_key[2])),
        decode_geo_char(static_cast<char>(str_key[1])),
        decode_geo_char(static_cast<char>(str_key[0])),
        decode_geo_char(static_cast<char>(str_key[4])),
        part
    );
}

auto convert_ramp_key_to_string(uint32_t key) -> std::string
{
    const RampKey s_key = unpack_ramp_key(key);
    return std::string {
        encode_geo_char(s_key.la3),
        encode_geo_char(s_key.la2),
        encode_geo_char(s_key.la1),
        encode_geo_char(s_key.la0),
        static_cast<char>('0' + s_key.var),
        static_cast<char>('0' + s_key.par),
    };
}


} // namespace assmpq::merger