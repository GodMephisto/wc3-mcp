---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/importer/main.cpp"
ref: HEAD
---

#include <exception>
#include <filesystem>
#include <fmt/base.h>
#include <fmt/format.h>
#include <spdlog/spdlog.h>
#include <CLI/CLI.hpp>

#include <internal_use_only/config.hpp>
#include "assets_mpq_importer/mpq.hpp"
#include "importer.hpp"

using assmpq::importer::import_blp;
using assmpq::importer::import_mdx;
using assmpq::importer::import_w3e;
using assmpq::importer::import_shd;
using assmpq::importer::import_wpm;
using assmpq::importer::import_doo;

auto main(int argc, char* argv[])-> int
try {
    const auto app_description = fmt::format(
        "{} version {} importer\n"
        "* Extract and convert files from MPQ archive.\n"
        "* Convert MDX meshes to Wavefront OBJ, convert BLP textures to PNG or DDS(BC1,BC3,BC7) with mipmaps.\n"
        "* Extract and save w3e map files.\n",
        assets_mpq_importer::cmake::project_name, assets_mpq_importer::cmake::project_version);

    CLI::App app { app_description };
    assmpq::importer::ProgramOptions popt;

    app.set_version_flag("-v,--version", app_description);
    app.add_option("-i,--input", popt.input_mpq_file, "Input MPQ archive file name.")
        ->required()
        ->check(CLI::ExistingFile);
    app.add_option("-o,--output", popt.output_folder, "Output folder.")
        ->check(CLI::ExistingDirectory);
    app.add_option("-f,--filter", popt.pattern, "File extraction filter.");

    static const std::map<std::string, assmpq::blp::Compression> compression_map = {
        { "bc1", assmpq::blp::Compression::DDS_BC1 },
        { "bc3", assmpq::blp::Compression::DDS_BC3 },
        { "bc7", assmpq::blp::Compression::DDS_BC7 }
    };
    app.add_option("-c,--compression", popt.compression, "DDS compression format (BC1, BC3, BC7), BC3 by default. ")
        ->transform(CLI::CheckedTransformer(compression_map, CLI::ignore_case))
        ->default_val("bc3");

    app.add_flag("--regen-mipmap", popt.is_regen_mipmaps, "Dont use original mipmaps. Recompute it from the scratch.");
    app.add_flag("--nvtt", popt.is_nvtt, "Use Nvidia Texture Tools compressor. AMD Compressionator by default.");
    app.add_flag("-d,--dds", popt.is_dds, "Convert BLP textures to DDS format. Convert to PNG if not present.");
    app.add_flag("-e,--extract", popt.is_extract, "Don't convert the files. Just extract everything.");
    app.add_flag("-w,--w3e", popt.is_w3e_only, "Extract w3e map files only from w3m/w3x maps.");
    app.add_flag("--verbose", popt.is_verbose, "Enable verbose output.");

    CLI11_PARSE(app, argc, argv);

    spdlog::info("MPQ archive: {}", popt.input_mpq_file.string());

    const auto list_files = assmpq::mpq::list_mpq_files(popt.input_mpq_file, popt.pattern);
    if (!list_files.has_value()) {
        spdlog::error("Error extracting list file from MPQ archive: {}", list_files.error());
        return 1;
    }

    for (const auto& file : list_files.value()) {
        spdlog::info("File processing: {}", file.filename);

        const auto extracted_file = assmpq::mpq::extract_mpq_file(popt.input_mpq_file, file.filename);
        if (!extracted_file.has_value()) {
            spdlog::error("File extraction error: {}", extracted_file.error());
            continue;
        }

        auto archived_filename = file.filename;
        std::ranges::replace(archived_filename, '\\', '/');

        const std::filesystem::path archived_file_path = archived_filename;
        if (popt.is_extract) {
            import_save(extracted_file.value(), archived_file_path, popt);
            continue;
        }

        static const std::unordered_map<std::string, std::vector<assmpq::importer::import_func_t>> importers_mapper = {
            { ".blp", { import_blp }},
            { ".BLP", { import_blp }},
            { ".mdx", { import_mdx }},
            { ".MDX", { import_mdx }},
            { ".w3m", { import_w3e, import_shd, import_wpm, import_doo }},
            { ".W3M", { import_w3e, import_shd, import_wpm, import_doo }},
            { ".w3x", { import_w3e, import_shd, import_wpm, import_doo }},
            { ".W3X", { import_w3e, import_shd, import_wpm, import_doo }},
        };

        if (!importers_mapper.contains(archived_file_path.extension().string())) {
            spdlog::warn("Importer not found for extension: {}", archived_file_path.extension().string());
            continue;
        }

        auto coverterters = importers_mapper.at(archived_file_path.extension().string());
        for(const auto& coverter_fn : coverterters) {
            coverter_fn(extracted_file.value(), archived_file_path, popt);
        }
    }
} catch (const std::exception &e) {
    spdlog::error("Unhandled exception in main: {}", e.what());
}
