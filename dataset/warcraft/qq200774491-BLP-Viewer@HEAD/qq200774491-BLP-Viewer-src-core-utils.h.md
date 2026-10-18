---
type: vendor-doc
source_repo: "https://github.com/qq200774491/BLP-Viewer"
source_path: "src/core/utils.h"
ref: HEAD
---

#pragma once

#include <cstdint>
#include <filesystem>
#include <string>
#include <vector>

/**
 * @brief  Returns true if value is a positive power of two.
 * @param  value  The integer to test; must be positive for a true result.
 * @return true when value > 0 and exactly one bit is set.
 */
constexpr bool is_power_of_two(int value) {
    return value > 0 && ((value & (value - 1)) == 0);
}

/**
 * @brief  Returns the power of two closest to value (ties round up).
 * @param  value  Input integer; values <= 0 return 1.
 * @return Nearest power of two in [1, 2^30].
 */
int nearest_power_of_two(int value);

/**
 * @brief  Returns the smallest power of two greater than or equal to value.
 * @param  value  Input integer; values <= 0 return 1.
 * @return Next (or equal) power of two in [1, 2^30].
 */
int next_power_of_two(int value);

/**
 * @brief  Computes the number of mipmap levels for a given image dimension.
 * @param  width   Image width in pixels.
 * @param  height  Image height in pixels.
 * @return Level count in [1, MAX_MIPMAP_COUNT].
 */
int auto_mip_count(int width, int height);

/**
 * @brief  Converts a UTF-8 string to a filesystem path.
 * @param  utf8Path  Null-terminated UTF-8 encoded path string.
 * @return Equivalent filesystem path; falls back to native encoding on error.
 */
std::filesystem::path fs_path_from_utf8(const std::string& utf8Path);

/**
 * @brief  Converts a filesystem path to a UTF-8 string.
 * @param  path  Filesystem path to convert.
 * @return UTF-8 encoded string; falls back to native encoding on error.
 */
std::string fs_path_to_utf8(const std::filesystem::path& path);

/**
 * @brief  Formats a byte count as a human-readable size string.
 * @param  bytes  Size in bytes.
 * @return String like "1.23 MB" or "512 B".
 */
std::string format_file_size(uint64_t bytes);

/**
 * @brief  Normalises an image format name: trims whitespace, lowercases, maps "jpeg" -> "jpg".
 * @param  format  Raw format string (e.g. "JPEG", " PNG ").
 * @return Canonical lowercase format name.
 */
std::string normalize_format(const std::string& format);

/**
 * @brief  Returns the list of file extensions supported by the viewer.
 * @return Vector of lowercase extensions without leading dot (e.g. "blp", "png").
 */
std::vector<std::string> supported_extensions();

/**
 * @brief  Returns true if the file at path has a supported extension.
 * @param  path  File path (UTF-8 encoded).
 * @return true when the extension matches one of supported_extensions().
 */
bool is_supported_file(const std::string& path);
