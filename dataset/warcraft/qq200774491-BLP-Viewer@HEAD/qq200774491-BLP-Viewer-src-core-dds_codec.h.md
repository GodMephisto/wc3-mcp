---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/dds_codec.h"
ref: HEAD
---

#pragma once

#include <cstddef>
#include <cstdint>
#include <optional>
#include <string>
#include <vector>

namespace ddscodec {

struct RawImage {
    uint32_t width = 0;
    uint32_t height = 0;
    std::vector<uint8_t> rgba;
};

/** Return true when the buffer starts with a DDS file signature. */
bool is_dds(const uint8_t* data, size_t size);

/**
 * Decode the first 2D surface and highest-resolution mip into RGBA8.
 * Supports legacy and DX10 DDS headers, uncompressed RGB(A), DXT1/3/5,
 * and BC1/2/3/4/5 (UNORM and BC4/5 SNORM).
 */
std::optional<RawImage> decode(const uint8_t* data, size_t size, std::string* outError);

/** Encode an RGBA8 image as a lossless, widely-compatible BGRA8 DDS. */
bool encode_bgra8(const uint8_t* rgba, uint32_t width, uint32_t height,
                  std::vector<uint8_t>& outDds, std::string* outError);

} // namespace ddscodec
