---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/blp/blp_codec.h"
ref: HEAD
---

// Self-contained BLP1 codec (decode ported from ShadowEngine BlpDecoder,
// encode is a new implementation).
// Supports only War3 BLP1: paletted (alpha 0/1/4/8-bit) and JPEG-content variants.
// BLP2 is explicitly rejected.
#pragma once

#include <cstdint>
#include <optional>
#include <string>
#include <vector>

namespace blpcodec {

/// Maximum number of mipmap levels in a BLP1 file.
constexpr int MAX_MIPS = 16;

/// Raw decoded image in RGBA byte order.
struct RawImage {
    uint32_t width = 0;                ///< Image width in pixels.
    uint32_t height = 0;               ///< Image height in pixels.
    std::vector<uint8_t> rgba;         ///< Packed pixels: width * height * 4 bytes, RGBA order.
};

/**
 * @brief  Returns true if @p data begins with the BLP1 magic number.
 * @param  data  Pointer to the raw file bytes.
 * @param  size  Number of bytes available at @p data.
 * @return true if the first 4 bytes equal the BLP1 signature; false otherwise.
 * @note   Only inspects the magic — does not validate the rest of the header.
 */
bool is_blp1(const uint8_t* data, size_t size);

/**
 * @brief  Returns true if @p data begins with any BLP magic number (BLP1 or BLP2).
 * @param  data  Pointer to the raw file bytes.
 * @param  size  Number of bytes available at @p data.
 * @return true if the first 4 bytes equal either the BLP1 or BLP2 signature.
 * @note   Intended for file-type detection only; does not imply decode support.
 */
bool is_blp_magic(const uint8_t* data, size_t size);

/**
 * @brief  Decodes a single mipmap level from a BLP1 file.
 * @param  data      Pointer to the raw BLP file bytes.
 * @param  size      Number of bytes available at @p data.
 * @param  mipLevel  Zero-based mip index (0 = largest/full-resolution level).
 * @param  outError  If non-null, receives a human-readable error message on failure.
 * @return Decoded RGBA image on success; nullopt on failure.
 * @note   BLP2 files are rejected with an error; @p mipLevel must be < MAX_MIPS.
 */
std::optional<RawImage> decode_mip(const uint8_t* data, size_t size,
                                   uint32_t mipLevel, std::string* outError);

/**
 * @brief  Decodes the first valid mipmap level found in a BLP1 file.
 * @param  data      Pointer to the raw BLP file bytes.
 * @param  size      Number of bytes available at @p data.
 * @param  outError  If non-null, receives a human-readable error message on failure.
 * @return Decoded RGBA image from the first successfully decoded mip level; nullopt on failure.
 * @note   Iterates from level 0 upward, tolerating individual corrupt mip entries.
 *         Falls back to a forced level-0 attempt if no valid mip entries are found.
 */
std::optional<RawImage> decode(const uint8_t* data, size_t size, std::string* outError);

/**
 * @brief  Encodes an RGBA image into a JPEG-content BLP1 byte stream.
 * @param  rgba      Pointer to the source pixels in RGBA order (4 bytes per pixel).
 * @param  width     Image width in pixels.
 * @param  height    Image height in pixels.
 * @param  quality   JPEG quality (1–100); values outside this range are clamped.
 * @param  mipCount  Number of mip levels to generate (1–MAX_MIPS); clamped to the
 *                   maximum full chain length for the given dimensions.
 * @param  outBlp    Receives the encoded BLP1 file bytes on success.
 * @param  outError  If non-null, receives a human-readable error message on failure.
 * @return true on success; false on failure (outBlp is cleared).
 * @note   Each mip is written as a self-contained JPEG (jpegHeaderSize = 0).
 *         Encoding uses TJCS_CMYK with TJSAMP_444 so that the four JPEG components
 *         carry B/G/R/A raw channel data without colour-space conversion — matching
 *         the War3 BLP1 JPEG-content layout expected by the game engine.
 */
bool encode_jpeg_blp1(const uint8_t* rgba, uint32_t width, uint32_t height,
                      int quality, int mipCount,
                      std::vector<uint8_t>& outBlp, std::string* outError);

} // namespace blpcodec
