---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/image_io.h"
ref: HEAD
---

#pragma once

#include <cstdint>
#include <string>
#include <vector>

class BlpApi;

struct RgbaImage {
    int width = 0;                   ///< Image width in pixels.
    int height = 0;                  ///< Image height in pixels.
    std::vector<uint8_t> pixels;     ///< Tightly-packed RGBA8 pixel data, row-major.
};

struct ImageMeta {
    int width = 0;                   ///< Image width in pixels.
    int height = 0;                  ///< Image height in pixels.
    std::string format;              ///< Lowercase format string, e.g. "png", "blp", "dds".
    uint64_t fileSize = 0;           ///< Size of the source file in bytes.
};

/**
 * @brief  Load an image from disk into an RGBA8 buffer.
 * @param  path      UTF-8 path to the source file; BLP, DDS, PNG, JPG, TGA and
 *                   BMP are supported. BLP and DDS files are also identified by magic bytes.
 * @param  outImage  Receives decoded pixel data on success; must not be null.
 * @param  outMeta   Optional; receives width, height, format and file size.
 * @param  outError  Optional; receives a human-readable error message on failure.
 * @param  blpApi    Codec context used for BLP decoding; must not be null when a
 *                   BLP file is encountered.
 * @return true on success; false on any I/O or decode error.
 */
bool load_image_file(
    const std::string& path,
    RgbaImage* outImage,
    ImageMeta* outMeta,
    std::string* outError,
    BlpApi* blpApi
);

/**
 * @brief  Encode and write an RGBA8 image to disk.
 * @param  outputPath  UTF-8 destination path; parent directories are created
 *                     automatically.
 * @param  format      Target format string (case-insensitive): "png", "jpg",
 *                     "bmp", "tga", "dds", or "blp".
 * @param  image       Source RGBA8 image; must have valid dimensions and pixels.
 * @param  quality     JPEG / BLP quality [1..100]; ignored for lossless formats.
 * @param  mipCount    Number of mip levels to generate for BLP output; ignored
 *                     for all other formats.
 * @param  outError    Optional; receives a human-readable error message on failure.
 * @param  blpApi      Codec context used for BLP encoding; must not be null when
 *                     format is "blp".
 * @return true on success; false on any encode or I/O error.
 */
bool write_image_file(
    const std::string& outputPath,
    const std::string& format,
    const RgbaImage& image,
    int quality,
    int mipCount,
    std::string* outError,
    BlpApi* blpApi
);
