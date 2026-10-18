---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "include/assets_mpq_importer/w3m.hpp"
ref: HEAD
---

#ifndef ASSMPQ_W3M_H_
#define ASSMPQ_W3M_H_

#include <expected>

#include "assets_mpq_importer/w3m_library_export.hpp"
#include "assmpq.hpp"

namespace assmpq::w3m {

/**
 * @brief Extracts the W3E (environment) file from a W3M/W3X (Warcraft III map) file.
 *
 * @param w3m_file The raw data of the W3M file to extract the environment from.
 * @return std::expected<FileData, ErrorMessage> containing the extracted file data
 *         on success, or an error message on failure.
 */
[[nodiscard]] W3M_LIBRARY_EXPORT auto extract_w3e_file(const FileData& w3m_file)
    -> std::expected<FileData, ErrorMessage>;

/**
 * @brief Extracts the SHD (Shadow Map File) file from a W3M/W3X (Warcraft III map) file.
 *
 * @param w3m_file The raw data of the W3M file to extract the environment from.
 * @return std::expected<FileData, ErrorMessage> containing the extracted file data
 *         on success, or an error message on failure.
 */
[[nodiscard]] W3M_LIBRARY_EXPORT auto extract_shd_file(const FileData& w3m_file)
    -> std::expected<FileData, ErrorMessage>;

/**
 * @brief Extracts the WPM (The Path Map File) file from a W3M/W3X (Warcraft III map) file.
 *
 * @param w3m_file The raw data of the W3M file to extract the environment from.
 * @return std::expected<FileData, ErrorMessage> containing the extracted file data
 *         on success, or an error message on failure.
 */
[[nodiscard]] W3M_LIBRARY_EXPORT auto extract_wpm_file(const FileData& w3m_file)
    -> std::expected<FileData, ErrorMessage>;

/**
 * @brief Extracts the DOO (The doodad file for trees) file from a W3M/W3X (Warcraft III map) file.
 *
 * @param w3m_file The raw data of the W3M file to extract the environment from.
 * @return std::expected<FileData, ErrorMessage> containing the extracted file data
 *         on success, or an error message on failure.
 */
[[nodiscard]] W3M_LIBRARY_EXPORT auto extract_doo_file(const FileData& w3m_file)
    -> std::expected<FileData, ErrorMessage>;

} // namespace assmpq::w3m

#endif  /// ASSMPQ_W3M_H_
