---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "include/assets_mpq_importer/mpq.hpp"
ref: HEAD
---

#ifndef ASSMPQ_MPQ_H_
#define ASSMPQ_MPQ_H_

#include <vector>
#include <filesystem>
#include <expected>

#include "assets_mpq_importer/mpq_library_export.hpp"
#include "assmpq.hpp"

namespace assmpq::mpq {

/// @brief Represents an entry for a file within an MPQ archive
/// @details Contains the filename and size information for a file stored in an MPQ archive
struct FileEntry {
    std::string filename;
    std::streamsize size = 0;

    bool operator==(const FileEntry& other) const = default;
};

using ArchiveEntries = std::vector<FileEntry>;

/**
 *  @brief Lists files in an MPQ archive
 *  @param archive_path Path to the MPQ archive file
 *  @param mask Optional filter mask for file names (default: "")
 *  @return Expected containing a vector of FileEntry objects or an error message
 *  @details Retrieves a list of files contained in the specified Blizzard MPQ archive.
 *           If a mask is provided, only files matching the mask will be returned.
 */
[[nodiscard]] MPQ_LIBRARY_EXPORT auto list_mpq_files(const std::filesystem::path& archive_path, const std::string& mask = "")
    -> std::expected<ArchiveEntries, ErrorMessage>;

/**
 * @brief Extracts a file from an MPQ archive
 * @param archive_path Path to the MPQ archive file
 * @param filename Name of the file to extract from the archive
 * @return Expected containing the file data as a byte vector or an error message
 * @details Extracts the specified file from the Blizzard MPQ archive and returns its contents
 *          as a vector of bytes. If extraction fails, an error message is returned.
 */
[[nodiscard]] MPQ_LIBRARY_EXPORT auto extract_mpq_file(const std::filesystem::path& archive_path, const std::string& filename)
    -> std::expected<FileData, ErrorMessage>;

} // namespace assmpq::mpq

#endif  /// ASSMPQ_MPQ_H_
