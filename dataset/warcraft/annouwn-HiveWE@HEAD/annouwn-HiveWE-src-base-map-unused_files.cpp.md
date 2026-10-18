---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/base/map/unused_files.cpp"
ref: HEAD
---


module;

#include <QObject>

module Map;

import std;
import MDX;

namespace fs = std::filesystem;

std::vector<FileUsage> Map::get_file_usage() const {
	// First, get all .MDX files referenced by the map
	// Then load each of them and save the resources references by them
	// Deal with game overrides somehow

	const auto total_start = std::chrono::steady_clock::now();
	auto start = std::chrono::steady_clock::now();

	hive::unordered_map<std::string, std::unordered_set<std::string>> resources;

	// Match key for comparing a referenced path against a file on disk. WC3 paths are case-insensitive
	// and a model may be referenced as .mdl while imported as .mdx (or vice versa), so we lowercase and
	// unify the model extension. Only used for matching, never for display or deletion.
	const auto match_key = [](std::string path) {
		std::ranges::transform(path, path.begin(), [](const unsigned char c) {
			return c == '\\' ? '/' : static_cast<char>(std::tolower(c));
		});
		if (path.ends_with(".mdl")) {
			path = path.substr(0, path.size() - 4) + ".mdx";
		}
		return path;
	};

	// Real, displayable relative path (forward slashes, original case + extension).
	const auto display_path = [](std::string path) {
		normalize_path_to_forward_slash(path);
		return path;
	};

	// Files placed under these stock game-asset roots override game content by path (terrain tiles,
	// cliffs, team colours, ...). The map references them implicitly rather than via an object, so we
	// treat anything under them as a used override instead of flagging it as unused.
	const auto is_stock_override_path = [](const std::string& lowercase_path) {
		static constexpr std::array<std::string_view, 2> roots = {"terrainart/", "replaceabletextures/"};
		return std::ranges::any_of(roots, [&](std::string_view root) { return lowercase_path.starts_with(root); });
	};

	const auto find_references = [&](const slk::SLK& slk, const std::vector<std::string>& keys) {
		for (const auto& [id, values] : slk.shadow_data) {
			for (const auto& key : keys) {
				if (auto found = values.find(key); found != values.end()) {
					resources[match_key(found->second)].emplace(id);
				}
			}
		}
	};

	find_references(units_slk, {"file", "portrait", "specialart", "missileart1", "missileart2", "art", "pathtex"});
	find_references(items_slk, {"file", "art"});
	find_references(destructibles_slk, {"file", "pathtex", "pathtexdeath"});
	find_references(doodads_slk, {"file", "pathtex"});
	find_references(abilities_slk, {"targetart", "effectart", "specialart", "art", "researchart"});
	find_references(buff_slk, {"targetart", "missileart", "specialart", "buffart"});
	// Todo, all icon levels
	// find_references(upgrade_slk, {"file"});

	if (!info.loading_screen_model.empty() && info.loading_screen_number == -1) {
		resources[match_key(info.loading_screen_model)].emplace("loadingscreen");
	}

	for (const auto& i : sounds.sounds) {
		resources[match_key(i.file)].emplace(i.name);
	}

	std::println(
		"Find references: {:>5}ms",
		std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::steady_clock::now() - start).count()
	);
	start = std::chrono::steady_clock::now();

	// Enumerate every file in the map folder, keeping both its real path (for display/deletion) and its
	// match key (for case-insensitive comparison against references).
	struct MapFile {
		std::string display;
		std::string key;
	};
	std::vector<MapFile> files;
	for (const auto& i : fs::recursive_directory_iterator(filesystem_path)) {
		if (i.is_regular_file()) {
			const std::string file_name = i.path().filename().string();
			if (imports.blacklist.contains(file_name)) {
				continue;
			}
			const std::string rel = i.path().lexically_relative(filesystem_path).string();
			files.push_back({display_path(rel), match_key(rel)});
		}
	}

	// Scan every model in the map for the textures/attachments/emitters it references, so a texture
	// counts as used if any model references it (not just models reachable from the Object Editor).
	std::vector<std::string> models;
	for (const auto& file : files) {
		if (file.key.ends_with(".mdx")) {
			models.push_back(file.display);
		}
	}

	hive::unordered_map<std::string, std::unordered_set<std::string>> referenced_resources;
	std::mutex mutex;

	std::for_each(std::execution::par_unseq, models.begin(), models.end(), [&](const std::string& model_path) {
		mdx::MDX mdx;
		try {
			auto mdx_content = hierarchy.open_file(model_path);
			if (!mdx_content) {
				return;
			}
			mdx = mdx::MDX(mdx_content.value());
		} catch (const std::exception& e) {
			std::println("Exception loading mdx: {} with error: {}", model_path, e.what());
			return;
		}

		const auto guard = std::scoped_lock(mutex);
		for (const auto& texture : mdx.textures) {
			referenced_resources[match_key(texture.file_name.string())].emplace(model_path);
		}

		for (const auto& attachment : mdx.attachments) {
			referenced_resources[match_key(attachment.path)].emplace(model_path);
		}

		for (const auto& emitter : mdx.emitters1) {
			referenced_resources[match_key(emitter.path)].emplace(model_path);
		}
	});

	std::println(
		"Find MDX references: {:>5}ms",
		std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::steady_clock::now() - start).count()
	);
	start = std::chrono::steady_clock::now();

	for (const auto& [path, values] : referenced_resources) {
		resources[path].insert(values.begin(), values.end());
	}

	std::string script_file_name;
	if (info.lua) {
		script_file_name = "war3map.lua";
	} else {
		script_file_name = "war3map.j";
	}

	const auto binary = read_file(filesystem_path / script_file_name);
	std::string map_script;
	if (!binary) {
		std::println("Error reading map script. Save your map first.");
	} else {
		const auto a = binary.value();
		map_script = std::string(a.buffer.begin(), a.buffer.end());
		std::ranges::transform(map_script, map_script.begin(), [](const unsigned char c) { return static_cast<char>(std::tolower(c)); });
	}

	std::vector<FileUsage> result;
	result.reserve(files.size());
	for (const auto& [display, key] : files) {
		FileUsage usage;
		usage.path = display;

		std::error_code ec;
		usage.size = fs::file_size(filesystem_path / display, ec);
		if (ec) {
			usage.size = 0;
		}

		// Files that override a stock game asset by path (e.g. TerrainArt/*) are used implicitly even
		// though nothing in the map references them.
		usage.is_override = hierarchy.game_file_exists(display) || is_stock_override_path(key);

		if (const auto found = resources.find(key); found != resources.end()) {
			usage.used_by = found->second;
		} else if (!map_script.empty()) {
			// The map script references files by literal path; check both separator styles (lowercased).
			std::string forward = key;
			std::string backward = key;
			std::ranges::replace(backward, '/', '\\');
			if (map_script.contains(forward) || map_script.contains(backward)) {
				usage.used_by.emplace("map script");
			}
		}
		result.push_back(std::move(usage));
	}

	std::println(
		"Find unused files: {:>5}ms",
		std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::steady_clock::now() - start).count()
	);
	std::println(
		"Total: {:>5}ms",
		std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::steady_clock::now() - total_start).count()
	);

	return result;
}
