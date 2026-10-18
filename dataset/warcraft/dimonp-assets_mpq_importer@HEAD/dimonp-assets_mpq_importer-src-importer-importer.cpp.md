---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/importer/importer.cpp"
ref: HEAD
---

#include <fstream>
#include <filesystem>
#include <spdlog/spdlog.h>
#include "importer.hpp"

#include "assets_mpq_importer/blp.hpp"
#include "assets_mpq_importer/mdlx.hpp"
#include "assets_mpq_importer/w3m.hpp"

namespace assmpq::importer {

/**
 * @brief Save file data to the specified output path
 * @param file_data The file data to save
 * @param archived_file_path The path where the file should be saved
 * @param popt Program options containing output folder and other settings
 * @return true if file was saved successfully, false otherwise
 */
auto import_save(const assmpq::FileData& file_data, const std::filesystem::path& archived_file_path, const ProgramOptions& popt)-> bool
{
    auto output_filename = popt.output_folder / archived_file_path;

    std::filesystem::create_directories(output_filename.parent_path());
    std::ofstream output_file(output_filename, std::ios::out | std::ios::binary);
    if (!output_file) {
        spdlog::error("File write error: {}", output_filename.string());
        std::perror("System error message");
        return false;
    }

    output_file.write( file_data.data(), static_cast<std::streamsize>(file_data.size()));
    output_file.close();

    spdlog::info("File {} saved.", output_filename.string());

    return true;
}

/**
 * @brief Import and convert BLP texture file to either DDS or PNG format
 * @param file_data The BLP file data to convert
 * @param archived_file_path The original file path in the archive
 * @param popt Program options containing conversion settings
 * @return true if conversion and save was successful, false otherwise
 */
auto import_blp(const assmpq::FileData& file_data, const std::filesystem::path& archived_file_path, const ProgramOptions& popt)-> bool
{
    const auto converted_file_data = [&file_data, &popt]() {
        if (popt.is_dds && popt.is_nvtt) {
            return assmpq::blp::convert_blp_to_dds_texture_nvtt(file_data, popt.compression, popt.is_regen_mipmaps);
        } else if (popt.is_dds) {
            return assmpq::blp::convert_blp_to_dds_texture_amdc(file_data, popt.compression, popt.is_regen_mipmaps);
        } else {
            return assmpq::blp::convert_blp_to_png_image(file_data);
        }
    }();

    if (!converted_file_data.has_value()) {
        spdlog::error("File convertation error: {}", archived_file_path.string());
        return false;
    }

    auto output_path = archived_file_path;
    output_path.replace_extension(popt.is_dds ? "dds" : "png");
    return import_save(converted_file_data.value(), output_path, popt);
}

/**
 * @brief Import and convert MDX model file to OBJ mesh format
 * @param file_data The MDX file data to convert
 * @param archived_file_path The original file path in the archive
 * @param popt Program options
 * @return true if conversion and save was successful, false otherwise
 */
auto import_mdx(const assmpq::FileData& file_data, const std::filesystem::path& archived_file_path, const ProgramOptions& popt)-> bool
{
    const auto converted_file_data = assmpq::mdlx::convert_mdlx_to_obj_mesh(archived_file_path.stem().string(), file_data);
    if (!converted_file_data.has_value()) {
        spdlog::error("File convertation error: {}", archived_file_path.string());
        return false;
    }

    auto output_path = archived_file_path;
    output_path.replace_extension("obj");
    return import_save(converted_file_data.value(), output_path, popt);
}

/**
 * @brief Import and extract W3E map environment file
 * @param file_data The map file data to extract
 * @param archived_file_path The original file path in the archive
 * @param popt Program options
 * @return true if extraction and save was successful, false otherwise
 */
auto import_w3e(const assmpq::FileData& file_data, const std::filesystem::path& archived_file_path, const ProgramOptions& popt)-> bool
{
    const auto extracted_file_data = assmpq::w3m::extract_w3e_file(file_data);
    if (!extracted_file_data.has_value()) {
        spdlog::error("File convertation error: {}", archived_file_path.string());
        return false;
    }

    auto output_path = archived_file_path;
    output_path.replace_extension("w3e");
    return import_save(extracted_file_data.value(), output_path, popt);
}

/**
 * @brief Import and extract SHD shadows map file
 * @param file_data The map file data to extract
 * @param archived_file_path The original file path in the archive
 * @param popt Program options
 * @return true if extraction and save was successful, false otherwise
 */
auto import_shd(const assmpq::FileData& file_data, const std::filesystem::path& archived_file_path, const ProgramOptions& popt)-> bool
{
    if (popt.is_w3e_only) {
        return true;
    }

    const auto extracted_file_data = assmpq::w3m::extract_shd_file(file_data);
    if (!extracted_file_data.has_value()) {
        spdlog::error("File convertation error: {}", archived_file_path.string());
        return false;
    }

    auto output_path = archived_file_path;
    output_path.replace_extension("shd");
    return import_save(extracted_file_data.value(), output_path, popt);
}

/**
 * @brief Import and extract WPM path map file
 * @param file_data The map file data to extract
 * @param archived_file_path The original file path in the archive
 * @param popt Program options
 * @return true if extraction and save was successful, false otherwise
 */
auto import_wpm(const assmpq::FileData& file_data, const std::filesystem::path& archived_file_path, const ProgramOptions& popt)-> bool
{
    if (popt.is_w3e_only) {
        return true;
    }

    const auto extracted_file_data = assmpq::w3m::extract_wpm_file(file_data);
    if (!extracted_file_data.has_value()) {
        spdlog::error("File convertation error: {}", archived_file_path.string());
        return false;
    }

    auto output_path = archived_file_path;
    output_path.replace_extension("wpm");
    return import_save(extracted_file_data.value(), output_path, popt);
}

/**
 * @brief Import and extract DOO tree doodas file
 * @param file_data The map file data to extract
 * @param archived_file_path The original file path in the archive
 * @param popt Program options
 * @return true if extraction and save was successful, false otherwise
 */
auto import_doo(const assmpq::FileData& file_data, const std::filesystem::path& archived_file_path, const ProgramOptions& popt)-> bool
{
    if (popt.is_w3e_only) {
        return true;
    }

    const auto extracted_file_data = assmpq::w3m::extract_doo_file(file_data);
    if (!extracted_file_data.has_value()) {
        spdlog::error("File convertation error: {}", archived_file_path.string());
        return false;
    }

    auto output_path = archived_file_path;
    output_path.replace_extension("doo");
    return import_save(extracted_file_data.value(), output_path, popt);
}

} // namespace assmpq::importer
