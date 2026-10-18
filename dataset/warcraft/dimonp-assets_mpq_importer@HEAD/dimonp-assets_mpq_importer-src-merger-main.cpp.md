---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "src/merger/main.cpp"
ref: HEAD
---

#include <algorithm>
#include <string>
#include <fstream>
#include <vector>
#include <filesystem>
#include <fmt/base.h>
#include <fmt/format.h>
#include <spdlog/spdlog.h>
#include <CLI/CLI.hpp>
#include <nlohmann/json.hpp>

#if defined(__GNUC__) && !defined(__clang__)
// Disable a false positive warning which is a bug in the compiler's static analysis
#pragma GCC diagnostic push
#pragma GCC diagnostic ignored "-Wmaybe-uninitialized"
#endif
#include <regex>

#include <internal_use_only/config.hpp>
#include "merger.hpp"

/**
 * @brief Command line options for the mesh merger application
 *
 * This structure holds all the command line parameters that can be passed
 * to the application to control its behavior.
 */
struct ProgramOptions {
    std::filesystem::path input_folder;  ///< Input directory with cliff/ramp obj meshes
    std::filesystem::path output_folder;   ///< Output folder for merged meshes and JSON files
    std::string filename_pattern;         ///< Regex template for mesh filenames
    std::string geoset_name;              ///< Input geoset name
    std::optional<float> scale_factor;     ///< Optional mesh scale factor
    bool is_append = false;               ///< Append keys to existing JSON file
    bool is_ramp = false;                 ///< Process ramp geoset instead of cliff
    bool is_verbose = false;              ///< Enable verbose output
};

/**
 * @brief Add a group entry to the JSON document
 *
 * Adds a mapping between a group index and its corresponding geokey
 * to the JSON document.
 *
 * @param doc Reference to the JSON document to update
 * @param group_idx Index of the group
 * @param geo_key Geometry key for the group
 */
static inline void add_json_group(
    nlohmann::ordered_json& doc,
    size_t group_idx,
    uint32_t geo_key)
{
    doc[std::to_string(group_idx)] = std::to_string(geo_key);
}

/**
 * @brief Apply transformations to a mesh
 *
 * @param mesh_data Reference to the mesh data to transform
 * @param popt Program options containing transformation parameters
 */
static void transform_mesh(assmpq::merger::MeshData& mesh_data, const ProgramOptions& popt)
{
    transform_mesh_to_base_xz(mesh_data);

    if (popt.scale_factor.has_value()) {
        scale_mesh(mesh_data, popt.scale_factor.value());
    }
}

/**
 * @brief Process a mesh and add it to the appropriate group
 *
 * Processes either a ramp or cliff mesh, splitting ramp meshes into two parts
 * if needed, and adds the mesh(es) to the mesh group with appropriate geometry keys.
 *
 * @param all_mesh_group Reference to the collection of all mesh groups
 * @param mesh_data Reference to the mesh data to process
 * @param json_groups Reference to the JSON groups document to update
 * @param extracted_geo_name The extracted geometry name from the file
 * @param popt Program options controlling processing behavior
 */
static void process_mesh(
    assmpq::merger::MeshGroups& all_mesh_group,
    assmpq::merger::MeshData& mesh_data,
    nlohmann::ordered_json& json_groups,
    const std::string& extracted_geo_name,
    const ProgramOptions& popt)
{
    if (popt.is_ramp) { // process ramp meshes
        assmpq::merger::MeshData mesh_data0;
        assmpq::merger::MeshData mesh_data1;

        if (split_ramp_mesh(mesh_data, mesh_data0, mesh_data1)) {
            // first group
            all_mesh_group.push_back(mesh_data0);
            const uint32_t geo_key0 = assmpq::merger::get_ramp_key_from_geo_name(extracted_geo_name, 0);
            add_json_group(json_groups, all_mesh_group.size() - 1, geo_key0);

            // second group
            all_mesh_group.push_back(mesh_data1);
            const uint32_t geo_key1 = assmpq::merger::get_ramp_key_from_geo_name(extracted_geo_name, 1);
            add_json_group(json_groups, all_mesh_group.size() - 1, geo_key1);
        } else {
            spdlog::error("Failed to split ramp geo: {}", extracted_geo_name);
            return;
        }
    } else { // process cliff mesh
        const uint32_t geo_key = assmpq::merger::get_cliff_key_from_geo_name(extracted_geo_name);

        all_mesh_group.push_back(mesh_data);
        add_json_group(json_groups, all_mesh_group.size() - 1, geo_key);

        if (popt.is_verbose) {
            spdlog::info("Append cliff mesh {} (key:{}, vertices:{}, normals:{}, uvs:{}, triangles:{}, geo:{})\n",
                all_mesh_group.size(), geo_key,
                mesh_data.vertices.size(), mesh_data.normals.size(), mesh_data.uvs.size(),
                mesh_data.faces.size(), extracted_geo_name);
        }
    }
}

auto main(int argc, char* argv[])-> int
try {
    const auto app_description = fmt::format(
        "{} version {} merger\n"
        "* Processes cliff or ramp meshes from OBJ files, applies transformations,\n"
        "* splits ramp meshes, and generates a merged multigroup mesh with corresponding\n"
        "* JSON metadata files containing geometry keys.\n",
        assets_mpq_importer::cmake::project_name, assets_mpq_importer::cmake::project_version);

    CLI::App app { app_description };
    ProgramOptions popt;

    app.set_version_flag("-v,--version", app_description);

    app.add_option("-i,--input", popt.input_folder, "Input directory with cliff/ramp obj meshes.")
        ->required()
        ->check(CLI::ExistingDirectory);

    app.add_option("-o,--output", popt.output_folder, "Output folder.")
        ->check(CLI::ExistingDirectory);

    app.add_option("-p,--pattern", popt.filename_pattern, "Regex template for mesh filenames. "
        "Used to obtain a filename key part: CityCliffsBABC0.mdx -> BABC0.");
    app.add_option("-n,--name", popt.geoset_name, "Output geoset name.");

    app.add_option("-s,--scale", popt.scale_factor, "Mesh scale factor.");

    app.add_flag("-a,--append", popt.is_append, "Append keys to existing JSON file.");
    app.add_flag("-r,--ramp", popt.is_ramp, "The input folder contains ramp geoset.");
    app.add_flag("--verbose", popt.is_verbose, "Enable verbose output.");

    CLI11_PARSE(app, argc, argv);

    nlohmann::ordered_json output_json_doc;

    const std::string output_json_filename = std::format("{}_keys.json", popt.geoset_name);

    spdlog::info("Process folder: {}", popt.input_folder.string());

    auto output_json_path = popt.output_folder / output_json_filename;

    if (popt.is_append) { // read an existing JSON file
        std::ifstream input_json_file(output_json_path);
        if (!input_json_file.is_open()) {
            spdlog::error("Error: Could not open JSON file: {}", output_json_path.string());
            return 1;
        }

        try {
            input_json_file >> output_json_doc;
        } catch (const nlohmann::json::exception& e) {
            spdlog::error("JSON parse error: {}", e.what());
            return 1;
        }
    }

    // pattern example: CityCliffsBABC0.mdx: "CityCliffs([a-zA-Z0-9]{5})\." -> BABC0
    const std::regex match_regex(popt.filename_pattern, std::regex::icase);
    std::smatch matches;

    // sort mesh file names
    std::vector<std::filesystem::directory_entry> sorted_entries;
    std::ranges::copy(std::filesystem::directory_iterator(popt.input_folder), std::back_inserter(sorted_entries));
    std::ranges::sort(sorted_entries,
        [](const std::filesystem::directory_entry& first, const std::filesystem::directory_entry& second) {
            return first.path().filename() < second.path().filename();
        });

    nlohmann::ordered_json json_groups = {};
    assmpq::merger::MeshGroups all_mesh_group;

    // loop for all meshes
    for (const auto& entry : sorted_entries) {
        std::string current_file_name = entry.path().string();

        // check if the name matches the geoset pattern and substr the geo name
        if (!std::regex_search(current_file_name, matches, match_regex)) {
            spdlog::error("The pattern does not match the file: {}", current_file_name);
            break;
        }

        if (matches.size() > 2) {
            spdlog::error("Too much pattern matches for the file {}", current_file_name);
            return 1;
        }

        std::string extracted_geo_name = matches[1].str();
        assmpq::merger::MeshGroups mesh_group;

        if (popt.is_verbose) {
            spdlog::info("-> Loading {}", entry.path().string());
            spdlog::info("   Group name: {}", extracted_geo_name);
        }

        if (!load_model(entry.path().string(), mesh_group)) {
            spdlog::error("Loading error {}", entry.path().string());
            return 1;
        }

        if (mesh_group.size() > 1) {
            spdlog::error("Too much shapes for cliff/ramp mesh: {}", current_file_name);
            return 1;
        }

        assmpq::merger::MeshData mesh_data = mesh_group.front();
        mesh_data.name = extracted_geo_name;

        // Apply all transforamtions to mesh
        transform_mesh(mesh_data, popt);

        // Split ramps and generate geo keys
        process_mesh(all_mesh_group, mesh_data, json_groups, extracted_geo_name, popt);
    }

    const std::string mesh_type = popt.is_ramp ? "ramps" : "cliffs";
    const std::string typed_geoset_name = std::format("{}_{}", popt.geoset_name, mesh_type);
    const std::string output_mesh_filename = std::format("{}.obj", typed_geoset_name);

    output_json_doc[typed_geoset_name]["count"] = std::to_string(all_mesh_group.size());
    output_json_doc[typed_geoset_name]["mesh"] = output_mesh_filename;
    output_json_doc[typed_geoset_name]["type"] = mesh_type;
    output_json_doc[typed_geoset_name]["groups"] = json_groups;

    const auto output_mesh_path = popt.output_folder / output_mesh_filename;
    spdlog::info("-> Saving {} mesh file: {}", mesh_type, output_mesh_path.string());
    spdlog::info("   Shapes count: {}", all_mesh_group.size());

    if (!save_model(output_mesh_path.string(), all_mesh_group)) {
        spdlog::error("Could not save mesh: {}", output_mesh_path.string());
        return 1;
    }

    spdlog::info("-> Saving json file: {}", output_json_path.string());
    std::ofstream output_json_file(output_json_path);
    if (!output_json_file.is_open()) {
        spdlog::error("Failed to open output JSON file");
        return 1;
    }

    output_json_file << std::setw(4) << output_json_doc;

} catch (const std::exception &e) {
    spdlog::error("Unhandled exception in main: {}", e.what());
}

#if defined(__GNUC__) && !defined(__clang__)
#pragma GCC diagnostic pop
#endif
