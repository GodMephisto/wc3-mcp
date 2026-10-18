---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "include/assets_mpq_importer/blp.hpp"
ref: HEAD
---

#ifndef ASSMPQ_BLP_H_
#define ASSMPQ_BLP_H_

#include <expected>

#include "assets_mpq_importer/blp_library_export.hpp"
#include "assmpq.hpp"

namespace assmpq::blp {

/// Number of channels in RGBA color format (Red, Green, Blue, Alpha)
inline constexpr int kRgbaChannels = 4;

/// Texture compression formats supported for DDS output
enum class Compression: int { // NOLINT
    /// DDS BC1 compression format (DXT1) - good for simple textures without alpha
    DDS_BC1,
    /// DDS BC3 compression format (DXT5) - good for textures with alpha channel
    DDS_BC3,
    /// DDS BC7 compression format - highest quality compression for textures
    DDS_BC7
};

/**
 * Converts a BLP texture file to PNG image format
 * @param blp_file The BLP file data to convert
 * @param mipmap_idx The mipmap level index to extract (default: 0 for highest resolution)
 * @return PNG image data on success, or error message on failure
 */
[[nodiscard]] BLP_LIBRARY_EXPORT auto convert_blp_to_png_image(
    const FileData& blp_file,
    size_t mipmap_idx = 0
)-> std::expected<FileData, ErrorMessage>;

/**
 * Converts a BLP texture file to DDS texture format with specified compression
 * This function uses Nvidia Texture Tools library backend.
 * @param blp_file The BLP file data to convert
 * @param compression The DDS compression format to use (default: DDS_BC3)
 * @param regen_mipmaps Whether to generate mipmaps from scratch (default: false)
 * @return DDS texture data on success, or error message on failure
 */
[[nodiscard]] BLP_LIBRARY_EXPORT auto convert_blp_to_dds_texture_nvtt(
    const FileData& blp_file,
    const Compression& compression = Compression::DDS_BC3,
    bool regen_mipmaps = false
)-> std::expected<FileData, ErrorMessage>;

/**
 * Converts a BLP texture file to DDS texture format with specified compression
 * This function uses AMD Compressionator library backend.
 * @param blp_file The BLP file data to convert
 * @param compression The DDS compression format to use (default: DDS_BC3)
 * @param regen_mipmaps Whether to generate mipmaps from scratch (default: false)
 * @return DDS texture data on success, or error message on failure
 */
[[nodiscard]] BLP_LIBRARY_EXPORT auto convert_blp_to_dds_texture_amdc(
    const FileData& blp_file,
    const Compression& compression = Compression::DDS_BC3,
    bool regen_mipmaps = false
)-> std::expected<FileData, ErrorMessage>;

} // namespace assmpq::blp

#endif // ASSMPQ_BLP_H_
