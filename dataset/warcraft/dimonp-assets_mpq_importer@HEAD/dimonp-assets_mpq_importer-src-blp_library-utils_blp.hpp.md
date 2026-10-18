---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/blp_library/utils_blp.hpp"
ref: HEAD
---

#ifndef ASSMPQ_UTILS_BLP_H_
#define ASSMPQ_UTILS_BLP_H_

#include <vector>
#include <blp/blp.hpp>

namespace assmpq::blp {

/**
 * @brief Extracts RGBA color data from a BLP mipmap as a vector of 32-bit unsigned integers.
 *
 * This function converts the color data from a BLP mipmap into a contiguous buffer of RGBA values.
 * Each color value is byte-swapped for proper endianness.
 *
 * @param mipmap The BLP mipmap to extract color data from
 * @return std::vector<uint32_t> A vector containing the RGBA color data
 */
auto get_mipmap_buffer_rgba(const wc3lib::blp::Blp::MipMap& mipmap)-> std::vector<uint32_t>;

/**
 * @brief Extracts RGBA color data from a paletted BLP mipmap using a color palette.
 *
 * This function converts the color data from a paletted BLP mipmap into a contiguous buffer
 * of RGBA values by looking up each pixel's color in the provided palette.
 * Each color value is byte-swapped for proper endianness.
 *
 * @param mipmap The paletted BLP mipmap to extract color data from
 * @param palette The color palette to use for converting palette indices to RGBA values
 * @return std::vector<uint32_t> A vector containing the RGBA color data
 */
auto get_paletted_mipmap_buffer_rgba(
    const wc3lib::blp::Blp::MipMap& mipmap,
    const  wc3lib::blp::Blp::ColorPtr& palette)-> std::vector<uint32_t>;

/**
 * @brief Extracts RGBA color data from a BLP mipmap as floating point values.
 *
 * This function converts the color data from a BLP mipmap into separate channels of
 * floating point values in the range [0, 1] for each of RGBA components.
 *
 * @param mipmap The BLP mipmap to extract color data from
 * @return std::vector<float> A vector containing the RGBA color data as floating point values
 */
auto get_mipmap_buffer_float(const wc3lib::blp::Blp::MipMap& mipmap)-> std::vector<float>;

/**
 * @brief Extracts RGBA color data from a paletted BLP mipmap as floating point values.
 *
 * This function converts the color data from a paletted BLP mipmap into separate channels of
 * floating point values in the range [0, 1] for each of RGBA components by looking up
 * each pixel's color in the provided palette.
 *
 * @param mipmap The paletted BLP mipmap to extract color data from
 * @param palette The color palette to use for converting palette indices to RGBA values
 * @return std::vector<float> A vector containing the RGBA color data as floating point values
 */
auto get_paletted_mipmap_buffer_float(
    const wc3lib::blp::Blp::MipMap& mipmap,
    const  wc3lib::blp::Blp::ColorPtr& palette)-> std::vector<float>;

} // namespace assmpq::blp

#endif // ASSMPQ_UTILS_BLP_H_
