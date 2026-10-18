---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/smoke/smoke_save_reload.cpp"
ref: HEAD
---

#include "smoke/smoke_tests.h"

#include "main_window/hivewe.h"

#if HIVEWE_USE_METAL_RUNTIME
#include "rendering/metal_map_frame_builders.h"
#include "rendering/metal_particle_emitter1_model_frame.h"
#endif

#define __STORMLIB_NO_STATIC_LINK__
#include "StormLib.h"

#include <QDebug>
#include <QTemporaryDir>

#include <algorithm>
#include <array>
#include <cstdint>
#include <filesystem>
#include <format>
#include <fstream>
#include <initializer_list>
#include <memory>
#include <optional>
#include <span>
#include <string>
#include <string_view>
#include <type_traits>
#include <vector>

#include <glm/glm.hpp>

#include "std_module_compat.h"
import Map;
import Doodad;
import Triggers;
import Units;
import PathingMap;
import MapGlobal;
import Globals;
import Utilities;
import SLK;
import MPQ;
import Terrain;

namespace fs = std::filesystem;

namespace {
constexpr std::uint64_t k_fnv_offset = 14695981039346656037ull;
constexpr std::uint64_t k_fnv_prime = 1099511628211ull;

void smoke_hash_bytes(std::uint64_t& hash, const void* data, const std::size_t size) {
	const auto* bytes = static_cast<const std::uint8_t*>(data);
	for (std::size_t i = 0; i < size; ++i) {
		hash ^= bytes[i];
		hash *= k_fnv_prime;
	}
}

template <typename T>
void smoke_hash_value(std::uint64_t& hash, const T& value) requires std::is_trivially_copyable_v<T> {
	smoke_hash_bytes(hash, &value, sizeof(T));
}

void smoke_hash_string(std::uint64_t& hash, const std::string& value) {
	smoke_hash_bytes(hash, value.data(), value.size());
	const std::uint8_t terminator = 0;
	smoke_hash_bytes(hash, &terminator, sizeof(terminator));
}

template <typename T>
void smoke_hash_vector(std::uint64_t& hash, const std::vector<T>& values) requires std::is_trivially_copyable_v<T> {
	smoke_hash_value(hash, values.size());
	if (!values.empty()) {
		const auto bytes = std::as_bytes(std::span(values));
		smoke_hash_bytes(hash, bytes.data(), bytes.size());
	}
}

void smoke_hash_item_sets(std::uint64_t& hash, const std::vector<ItemSet>& item_sets) {
	smoke_hash_value(hash, item_sets.size());
	for (const auto& item_set : item_sets) {
		smoke_hash_value(hash, item_set.items.size());
		for (const auto& [chance, id] : item_set.items) {
			smoke_hash_value(hash, chance);
			smoke_hash_string(hash, id);
		}
	}
}

void smoke_hash_unit(std::uint64_t& hash, const Unit& unit) {
	smoke_hash_string(hash, unit.id);
	smoke_hash_value(hash, unit.variation);
	smoke_hash_value(hash, unit.position);
	smoke_hash_value(hash, unit.angle);
	smoke_hash_value(hash, unit.scale);
	smoke_hash_string(hash, unit.skin_id);
	smoke_hash_value(hash, unit.flags);
	smoke_hash_value(hash, unit.player);
	smoke_hash_value(hash, unit.health);
	smoke_hash_value(hash, unit.mana);
	smoke_hash_value(hash, unit.item_table_pointer);
	smoke_hash_item_sets(hash, unit.item_sets);
	smoke_hash_value(hash, unit.gold);
	smoke_hash_value(hash, unit.target_acquisition);
	smoke_hash_value(hash, unit.level);
	smoke_hash_value(hash, unit.strength);
	smoke_hash_value(hash, unit.agility);
	smoke_hash_value(hash, unit.intelligence);
	smoke_hash_value(hash, unit.items.size());
	for (const auto& [slot, id] : unit.items) {
		smoke_hash_value(hash, slot);
		smoke_hash_string(hash, id);
	}
	smoke_hash_value(hash, unit.abilities.size());
	for (const auto& [id, autocast, level] : unit.abilities) {
		smoke_hash_string(hash, id);
		smoke_hash_value(hash, autocast);
		smoke_hash_value(hash, level);
	}
	smoke_hash_value(hash, unit.random_type);
	smoke_hash_vector(hash, unit.random);
	smoke_hash_value(hash, unit.custom_color);
	smoke_hash_value(hash, unit.waygate);
	smoke_hash_value(hash, unit.creation_number);
}

void smoke_hash_doodad(std::uint64_t& hash, const Doodad& doodad) {
	smoke_hash_string(hash, doodad.id);
	smoke_hash_string(hash, doodad.skin_id);
	smoke_hash_value(hash, doodad.variation);
	smoke_hash_value(hash, doodad.position);
	smoke_hash_value(hash, doodad.scale);
	smoke_hash_value(hash, doodad.angle);
	const int state = static_cast<int>(doodad.state);
	smoke_hash_value(hash, state);
	smoke_hash_value(hash, doodad.life);
	smoke_hash_value(hash, doodad.item_table_pointer);
	smoke_hash_item_sets(hash, doodad.item_sets);
	smoke_hash_value(hash, doodad.creation_number);
}

void smoke_hash_special_doodad(std::uint64_t& hash, const SpecialDoodad& doodad) {
	smoke_hash_string(hash, doodad.id);
	smoke_hash_value(hash, doodad.variation);
	smoke_hash_value(hash, doodad.position);
	smoke_hash_value(hash, doodad.old_position);
}

void smoke_hash_trigger_parameter(std::uint64_t& hash, const TriggerParameter& parameter);

void smoke_hash_eca(std::uint64_t& hash, const ECA& eca) {
	const int type = static_cast<int>(eca.type);
	smoke_hash_value(hash, type);
	smoke_hash_value(hash, eca.group);
	smoke_hash_string(hash, eca.name);
	smoke_hash_value(hash, eca.enabled);
	smoke_hash_value(hash, eca.parameters.size());
	for (const auto& parameter : eca.parameters) {
		smoke_hash_trigger_parameter(hash, parameter);
	}
	smoke_hash_value(hash, eca.ecas.size());
	for (const auto& child : eca.ecas) {
		smoke_hash_eca(hash, child);
	}
}

void smoke_hash_trigger_parameter(std::uint64_t& hash, const TriggerParameter& parameter) {
	const int type = static_cast<int>(parameter.type);
	smoke_hash_value(hash, type);
	smoke_hash_value(hash, parameter.unknown);
	smoke_hash_string(hash, parameter.value);
	smoke_hash_value(hash, parameter.has_sub_parameter);
	if (parameter.has_sub_parameter) {
		smoke_hash_eca(hash, parameter.sub_parameter);
	}
	smoke_hash_value(hash, parameter.is_array);
	smoke_hash_value(hash, parameter.parameters.size());
	for (const auto& child : parameter.parameters) {
		smoke_hash_trigger_parameter(hash, child);
	}
}

void smoke_hash_triggers(std::uint64_t& hash, const Triggers& triggers) {
	smoke_hash_string(hash, triggers.global_jass_comment);
	smoke_hash_string(hash, triggers.global_jass);
	smoke_hash_value(hash, triggers.categories.size());
	for (const auto& category : triggers.categories) {
		const int classifier = static_cast<int>(category.classifier);
		smoke_hash_value(hash, classifier);
		smoke_hash_value(hash, category.id);
		smoke_hash_string(hash, category.name);
		smoke_hash_value(hash, category.open_state);
		smoke_hash_value(hash, category.is_comment);
		smoke_hash_value(hash, category.parent_id);
	}
	smoke_hash_value(hash, triggers.variables.size());
	for (const auto& variable : triggers.variables) {
		smoke_hash_string(hash, variable.name);
		smoke_hash_string(hash, variable.type);
		smoke_hash_value(hash, variable.unknown);
		smoke_hash_value(hash, variable.is_array);
		smoke_hash_value(hash, variable.array_size);
		smoke_hash_value(hash, variable.is_initialized);
		smoke_hash_string(hash, variable.initial_value);
		smoke_hash_value(hash, variable.id);
		smoke_hash_value(hash, variable.parent_id);
	}
	smoke_hash_value(hash, triggers.triggers.size());
	for (const auto& trigger : triggers.triggers) {
		const int classifier = static_cast<int>(trigger.classifier);
		smoke_hash_value(hash, classifier);
		smoke_hash_value(hash, trigger.id);
		smoke_hash_value(hash, trigger.parent_id);
		smoke_hash_string(hash, trigger.name);
		smoke_hash_string(hash, trigger.description);
		smoke_hash_string(hash, trigger.custom_text);
		smoke_hash_value(hash, trigger.is_comment);
		smoke_hash_value(hash, trigger.is_enabled);
		smoke_hash_value(hash, trigger.is_script);
		smoke_hash_value(hash, trigger.initially_on);
		smoke_hash_value(hash, trigger.run_on_initialization);
		smoke_hash_value(hash, trigger.ecas.size());
		for (const auto& eca : trigger.ecas) {
			smoke_hash_eca(hash, eca);
		}
	}
}

template <typename MapInfoT>
void smoke_hash_map_info(std::uint64_t& hash, const MapInfoT& info) {
	smoke_hash_value(hash, info.map_version);
	smoke_hash_value(hash, MapInfoT::write_editor_version);
	smoke_hash_value(hash, MapInfoT::write_game_version_major);
	smoke_hash_value(hash, MapInfoT::write_game_version_minor);
	smoke_hash_value(hash, MapInfoT::write_game_version_patch);
	smoke_hash_value(hash, MapInfoT::write_game_version_build);
	smoke_hash_string(hash, info.name);
	smoke_hash_string(hash, info.author);
	smoke_hash_string(hash, info.description);
	smoke_hash_string(hash, info.suggested_players);

	smoke_hash_value(hash, info.camera_left_bottom);
	smoke_hash_value(hash, info.camera_right_top);
	smoke_hash_value(hash, info.camera_left_top);
	smoke_hash_value(hash, info.camera_right_bottom);
	smoke_hash_value(hash, info.camera_complements);
	smoke_hash_value(hash, info.playable_width);
	smoke_hash_value(hash, info.playable_height);

	smoke_hash_value(hash, info.hide_minimap_preview);
	smoke_hash_value(hash, info.modif_ally_priorities);
	smoke_hash_value(hash, info.melee_map);
	smoke_hash_value(hash, info.unknown);
	smoke_hash_value(hash, info.masked_area_partially_visible);
	smoke_hash_value(hash, info.fixed_player_settings);
	smoke_hash_value(hash, info.custom_forces);
	smoke_hash_value(hash, info.custom_techtree);
	smoke_hash_value(hash, info.custom_abilities);
	smoke_hash_value(hash, info.custom_upgrades);
	smoke_hash_value(hash, info.unknown2);
	smoke_hash_value(hash, info.cliff_shore_waves);
	smoke_hash_value(hash, info.rolling_shore_waves);
	smoke_hash_value(hash, info.unknown3);
	smoke_hash_value(hash, info.unknown4);
	smoke_hash_value(hash, info.item_classification);
	smoke_hash_value(hash, info.water_tinting);
	smoke_hash_value(hash, info.accurate_probability_for_calculations);
	smoke_hash_value(hash, info.custom_ability_skins);
	smoke_hash_value(hash, info.disable_deny_icon);
	smoke_hash_value(hash, info.force_default_zoom);
	smoke_hash_value(hash, info.force_max_zoom);
	smoke_hash_value(hash, info.force_min_zoom);

	smoke_hash_value(hash, info.loading_screen_number);
	smoke_hash_string(hash, info.loading_screen_model);
	smoke_hash_string(hash, info.loading_screen_text);
	smoke_hash_string(hash, info.loading_screen_title);
	smoke_hash_string(hash, info.loading_screen_subtitle);
	smoke_hash_value(hash, info.game_data_set);
	smoke_hash_string(hash, info.prologue_screen_model);
	smoke_hash_string(hash, info.prologue_text);
	smoke_hash_string(hash, info.prologue_title);
	smoke_hash_string(hash, info.prologue_subtitle);

	smoke_hash_value(hash, info.fog_style);
	smoke_hash_value(hash, info.fog_start_z_height);
	smoke_hash_value(hash, info.fog_end_z_height);
	smoke_hash_value(hash, info.fog_density);
	smoke_hash_value(hash, info.fog_color);
	smoke_hash_value(hash, info.weather_id);
	smoke_hash_string(hash, info.custom_sound_environment);
	smoke_hash_value(hash, info.custom_light_tileset);
	smoke_hash_value(hash, info.water_color);
	smoke_hash_value(hash, info.lua);
	smoke_hash_value(hash, info.supported_modes);
	smoke_hash_value(hash, info.game_data_version);
	smoke_hash_value(hash, info.default_cam_distance);
	smoke_hash_value(hash, info.max_cam_distance);
	smoke_hash_value(hash, info.min_cam_distance);

	smoke_hash_value(hash, info.players.size());
	for (const auto& player : info.players) {
		smoke_hash_value(hash, player.internal_number);
		smoke_hash_value(hash, static_cast<int>(player.type));
		smoke_hash_value(hash, static_cast<int>(player.race));
		smoke_hash_value(hash, player.fixed_start_position);
		smoke_hash_string(hash, player.name);
		smoke_hash_value(hash, player.starting_position);
		smoke_hash_value(hash, player.ally_low_priorities_flags);
		smoke_hash_value(hash, player.ally_high_priorities_flags);
		smoke_hash_value(hash, player.enemy_low_priorities_flags);
		smoke_hash_value(hash, player.enemy_high_priorities_flags);
	}

	smoke_hash_value(hash, info.forces.size());
	for (const auto& force : info.forces) {
		smoke_hash_value(hash, force.allied);
		smoke_hash_value(hash, force.allied_victory);
		smoke_hash_value(hash, force.share_vision);
		smoke_hash_value(hash, force.share_unit_control);
		smoke_hash_value(hash, force.share_advanced_unit_control);
		smoke_hash_value(hash, force.player_masks);
		smoke_hash_string(hash, force.name);
	}

	smoke_hash_value(hash, info.available_upgrades.size());
	for (const auto& upgrade : info.available_upgrades) {
		smoke_hash_value(hash, upgrade.player_flags);
		smoke_hash_string(hash, upgrade.id);
		smoke_hash_value(hash, upgrade.level);
		smoke_hash_value(hash, upgrade.availability);
	}

	smoke_hash_value(hash, info.available_tech.size());
	for (const auto& tech : info.available_tech) {
		smoke_hash_value(hash, tech.player_flags);
		smoke_hash_string(hash, tech.id);
	}

	smoke_hash_value(hash, info.random_unit_tables.size());
	for (const auto& table : info.random_unit_tables) {
		smoke_hash_value(hash, table.creation_number);
		smoke_hash_string(hash, table.name);
		smoke_hash_vector(hash, table.positions);
		smoke_hash_value(hash, table.lines.size());
		for (const auto& line : table.lines) {
			smoke_hash_value(hash, line.chance);
			smoke_hash_value(hash, line.ids.size());
			for (const auto& id : line.ids) {
				smoke_hash_string(hash, id);
			}
		}
	}

	smoke_hash_value(hash, info.random_item_tables.size());
	for (const auto& table : info.random_item_tables) {
		smoke_hash_value(hash, table.creation_number);
		smoke_hash_string(hash, table.name);
		smoke_hash_item_sets(hash, table.item_sets);
	}
}

std::uint64_t smoke_hash_file_contents(const fs::path& path) {
	std::uint64_t hash = k_fnv_offset;
	std::ifstream file(path, std::ios::binary);
	std::array<char, 4096> buffer {};
	while (file) {
		file.read(buffer.data(), static_cast<std::streamsize>(buffer.size()));
		const std::streamsize bytes_read = file.gcount();
		if (bytes_read > 0) {
			smoke_hash_bytes(hash, buffer.data(), static_cast<std::size_t>(bytes_read));
		}
	}
	return hash;
}

std::vector<std::string> smoke_import_manifest(const fs::path& root) {
	std::vector<std::string> manifest;
	if (!fs::exists(root)) {
		return manifest;
	}

	for (const auto& entry : fs::recursive_directory_iterator(root)) {
		if (!entry.is_regular_file()) {
			continue;
		}
		const std::string file_name = entry.path().filename().string();
		if (map && map->imports.blacklist.contains(file_name)) {
			continue;
		}
		manifest.push_back(std::format(
			"{}:{}:{:016x}",
			entry.path().lexically_relative(root).generic_string(),
			fs::file_size(entry.path()),
			smoke_hash_file_contents(entry.path())
		));
	}
	std::ranges::sort(manifest);
	return manifest;
}

std::uint64_t smoke_hash_import_manifest(const std::vector<std::string>& manifest) {
	std::uint64_t hash = k_fnv_offset;
	smoke_hash_value(hash, manifest.size());
	for (const auto& path : manifest) {
		smoke_hash_string(hash, path);
	}
	return hash;
}

void smoke_hash_object_table(
	std::uint64_t& hash,
	const std::string_view table_name,
	const slk::SLK& table,
	std::size_t& row_count,
	std::size_t& field_count
) {
	smoke_hash_string(hash, std::string(table_name));

	std::vector<std::string> row_ids;
	row_ids.reserve(table.shadow_data.size());
	for (const auto& [row_id, _] : table.shadow_data) {
		row_ids.push_back(row_id);
	}
	std::ranges::sort(row_ids);

	smoke_hash_value(hash, row_ids.size());
	row_count += row_ids.size();
	for (const std::string& row_id : row_ids) {
		smoke_hash_string(hash, row_id);
		const auto& fields = table.shadow_data.at(row_id);

		std::vector<std::string> field_ids;
		field_ids.reserve(fields.size());
		for (const auto& [field_id, _] : fields) {
			field_ids.push_back(field_id);
		}
		std::ranges::sort(field_ids);

		smoke_hash_value(hash, field_ids.size());
		field_count += field_ids.size();
		for (const std::string& field_id : field_ids) {
			smoke_hash_string(hash, field_id);
			smoke_hash_string(hash, fields.at(field_id));
		}
	}
}

std::vector<fs::path> smoke_saved_map_file_paths(const fs::path& root) {
	std::vector<fs::path> paths;
	if (!fs::exists(root)) {
		return paths;
	}

	for (const auto& entry : fs::recursive_directory_iterator(root)) {
		if (entry.is_regular_file()) {
			paths.push_back(entry.path().lexically_relative(root));
		}
	}
	std::ranges::sort(paths, {}, [](const fs::path& path) {
		return path.generic_string();
	});
	return paths;
}

bool smoke_save_rewrites_file(const std::string_view file_name) {
	static constexpr std::array<std::string_view, 30> rewritten_files {
		"war3map.doo",
		"war3map.imp",
		"war3map.j",
		"war3map.lua",
		"war3map.shd",
		"war3map.w3a",
		"war3map.w3b",
		"war3map.w3c",
		"war3map.w3d",
		"war3map.w3e",
		"war3map.w3h",
		"war3map.w3i",
		"war3map.w3q",
		"war3map.w3r",
		"war3map.w3s",
		"war3map.w3t",
		"war3map.w3u",
		"war3map.wct",
		"war3map.wpm",
		"war3map.wtg",
		"war3map.wts",
		"war3mapMisc.txt",
		"war3mapSkin.w3a",
		"war3mapSkin.w3b",
		"war3mapSkin.w3d",
		"war3mapSkin.w3h",
		"war3mapSkin.w3q",
		"war3mapSkin.w3t",
		"war3mapSkin.w3u",
		"war3mapUnits.doo",
	};
	return std::ranges::find(rewritten_files, file_name) != rewritten_files.end();
}

bool validate_smoke_saved_map_manifest(
	const fs::path& source_map_path,
	const fs::path& saved_map_path,
	const bool lua,
	std::string& reason
) {
	if (!fs::exists(saved_map_path)) {
		reason = std::format("saved map directory {} does not exist.", saved_map_path.string());
		return false;
	}

	const std::vector<fs::path> source_files = smoke_saved_map_file_paths(source_map_path);
	std::size_t byte_preserved_source_files = 0;
	std::vector<fs::path> byte_preserved_source_file_paths;
	for (const auto& source_file : source_files) {
		const fs::path source_path = source_map_path / source_file;
		const fs::path saved_path = saved_map_path / source_file;
		if (!fs::exists(saved_path)) {
			reason = std::format("source map file {} was not preserved.", source_file.generic_string());
			return false;
		}
		if (smoke_save_rewrites_file(source_file.filename().generic_string())) {
			continue;
		}

		std::error_code source_size_error;
		std::error_code saved_size_error;
		const std::uintmax_t source_size = fs::file_size(source_path, source_size_error);
		const std::uintmax_t saved_size = fs::file_size(saved_path, saved_size_error);
		if (source_size_error || saved_size_error || source_size != saved_size) {
			reason = std::format(
				"copied source map file {} changed size across save ({} -> {}).",
				source_file.generic_string(),
				source_size_error ? 0 : source_size,
				saved_size_error ? 0 : saved_size
			);
			return false;
		}

		const std::uint64_t source_hash = smoke_hash_file_contents(source_path);
		const std::uint64_t saved_hash = smoke_hash_file_contents(saved_path);
		if (source_hash != saved_hash) {
			reason = std::format(
				"copied source map file {} changed contents across save ({:016x} -> {:016x}).",
				source_file.generic_string(),
				source_hash,
				saved_hash
			);
			return false;
		}
		++byte_preserved_source_files;
		byte_preserved_source_file_paths.push_back(source_file);
	}

	static constexpr std::array<std::string_view, 3> required_byte_preserved_source_files {
		"conversation.json",
		"imports/smoke_import.txt",
		"war3mapMap.blp",
	};
	for (const std::string_view required_file : required_byte_preserved_source_files) {
		const fs::path required_path(required_file);
		if (!fs::exists(source_map_path / required_path)) {
			continue;
		}
		const std::string required_generic_path = required_path.generic_string();
		const bool byte_checked = std::ranges::find(
									  byte_preserved_source_file_paths,
									  required_generic_path,
									  [](const fs::path& path) {
										  return path.generic_string();
									  }
								  )
			!= byte_preserved_source_file_paths.end();
		if (!byte_checked) {
			reason = std::format("source map file {} must remain byte-preserved across save.", required_file);
			return false;
		}
	}

	std::vector<fs::path> required_map_files {
		"war3map.w3e",
		"war3map.w3i",
		"war3map.wpm",
		"war3map.shd",
		"war3map.w3r",
		"war3map.w3c",
		"war3map.w3s",
		"war3map.doo",
		"war3mapUnits.doo",
		"war3map.wts",
		"war3map.wtg",
		"war3map.wct",
		"war3map.imp",
		"war3mapMisc.txt",
		lua ? fs::path("war3map.lua") : fs::path("war3map.j"),
	};

	for (const auto& file : required_map_files) {
		const fs::path saved_file = saved_map_path / file;
		std::error_code size_error;
		const bool exists = fs::exists(saved_file);
		const std::uintmax_t file_size = exists ? fs::file_size(saved_file, size_error) : 0;
		if (!exists || size_error || file_size == 0) {
			reason = std::format("required saved map file {} is missing or empty.", file.generic_string());
			return false;
		}
	}

	std::println(
		"Smoke save/reload saved map manifest preserved {} source files, byte-checked {} copied files, and validated {} generated files.",
		source_files.size(),
		byte_preserved_source_files,
		required_map_files.size()
	);
	return true;
}

#ifdef _WIN32
std::wstring smoke_storm_native_path(const fs::path& path) {
	return path.wstring();
}
#else
std::string smoke_storm_native_path(const fs::path& path) {
	return path.string();
}
#endif

bool export_smoke_map_archive(const fs::path& source_map_path, const fs::path& archive_path, std::string& reason) {
	const std::vector<fs::path> source_files = smoke_saved_map_file_paths(source_map_path);
	if (source_files.empty()) {
		reason = std::format("source map directory {} has no files to archive.", source_map_path.string());
		return false;
	}

	std::error_code remove_error;
	fs::remove(archive_path, remove_error);
	if (remove_error) {
		reason = std::format("unable to remove existing archive {}: {}", archive_path.string(), remove_error.message());
		return false;
	}
	fs::create_directories(archive_path.parent_path());

	HANDLE handle = nullptr;
	const auto native_archive_path = smoke_storm_native_path(archive_path);
	const DWORD max_file_count = static_cast<DWORD>(std::max<std::size_t>(source_files.size(), 1));
	if (!SFileCreateArchive(
			native_archive_path.c_str(),
			MPQ_CREATE_LISTFILE | MPQ_CREATE_ATTRIBUTES,
			max_file_count,
			&handle
		)) {
		reason = std::format("StormLib failed to create archive {}: {}", archive_path.string(), GetLastError());
		return false;
	}

	const auto close_archive = [&handle] {
		if (handle != nullptr) {
			SFileCloseArchive(handle);
			handle = nullptr;
		}
	};

	for (const auto& relative_path : source_files) {
		const fs::path source_path = source_map_path / relative_path;
		const auto native_source_path = smoke_storm_native_path(source_path);
		const std::string archive_member_name = relative_path.generic_string();
		if (!SFileAddFileEx(
				handle,
				native_source_path.c_str(),
				archive_member_name.c_str(),
				MPQ_FILE_COMPRESS,
				MPQ_COMPRESSION_ZLIB,
				MPQ_COMPRESSION_NEXT_SAME
			)) {
			reason = std::format(
				"StormLib failed to add {} as {}: {}",
				source_path.string(),
				archive_member_name,
				GetLastError()
			);
			close_archive();
			return false;
		}
	}

	if (!SFileCompactArchive(handle, nullptr, false)) {
		reason = std::format("StormLib failed to compact archive {}: {}", archive_path.string(), GetLastError());
		close_archive();
		return false;
	}
	close_archive();

	std::error_code size_error;
	const std::uintmax_t archive_size = fs::file_size(archive_path, size_error);
	if (size_error || archive_size == 0) {
		reason = std::format("exported archive {} is missing or empty.", archive_path.string());
		return false;
	}

	std::println(
		"Smoke MPQ export/reload archived {} files into {} ({} bytes).",
		source_files.size(),
		archive_path.string(),
		archive_size
	);
	return true;
}

std::optional<std::string> first_smoke_slk_row(const slk::SLK& table) {
	if (table.index_to_row.empty()) {
		return {};
	}

	std::vector<std::string> ids;
	ids.reserve(table.index_to_row.size());
	for (const auto& [_, id] : table.index_to_row) {
		if (!id.empty()) {
			ids.push_back(id);
		}
	}
	if (ids.empty()) {
		return {};
	}
	std::ranges::sort(ids);
	return ids.front();
}

bool smoke_id_excluded(const std::string& id, const std::initializer_list<std::string_view> excluded_ids) {
	return std::ranges::any_of(excluded_ids, [&](const std::string_view excluded_id) {
		return id == excluded_id;
	});
}

std::optional<std::string> first_smoke_slk_row_excluding(
	const slk::SLK& table,
	const std::initializer_list<std::string_view> excluded_ids
) {
	if (table.index_to_row.empty()) {
		return {};
	}

	std::vector<std::string> ids;
	ids.reserve(table.index_to_row.size());
	for (const auto& [_, id] : table.index_to_row) {
		if (!id.empty() && !smoke_id_excluded(id, excluded_ids)) {
			ids.push_back(id);
		}
	}
	if (ids.empty()) {
		return {};
	}
	std::ranges::sort(ids);
	return ids.front();
}

std::optional<std::string> smoke_slk_row_or_first(const slk::SLK& table, const std::string_view preferred_id) {
	if (table.row_headers.contains(preferred_id)) {
		return std::string(preferred_id);
	}
	return first_smoke_slk_row(table);
}

std::optional<std::string> smoke_unit_slk_row() {
	for (const std::string_view preferred_id : {"hfoo", "opeo", "ewsp", "uaco"}) {
		if (units_slk.row_headers.contains(preferred_id)) {
			return std::string(preferred_id);
		}
	}
	return first_smoke_slk_row_excluding(units_slk, {"sloc", "uDNR", "bDNR"});
}

std::optional<std::string> smoke_item_slk_row() {
	if (items_slk.row_headers.contains("ratf") && !units_slk.row_headers.contains("ratf")) {
		return std::string("ratf");
	}

	std::vector<std::string> ids;
	ids.reserve(items_slk.index_to_row.size());
	for (const auto& [_, id] : items_slk.index_to_row) {
		if (!id.empty() && !units_slk.row_headers.contains(id)) {
			ids.push_back(id);
		}
	}
	if (ids.empty()) {
		return {};
	}
	std::ranges::sort(ids);
	return ids.front();
}

struct SmokeMutationEffects {
	bool edited_terrain = false;
	bool edited_static_pathing = false;
	bool edited_shadow_map = false;
	bool added_unit = false;
	bool added_item = false;
	bool added_doodad = false;
	bool edited_map_info = false;
	bool edited_triggers = false;
	bool added_import = false;
	bool edited_object_table = false;
	bool edited_misc = false;
};

bool apply_smoke_map_mutation(const fs::path& map_path, SmokeMutationEffects& effects, std::string& reason) {
	if (!map || !map->loaded) {
		reason = "no loaded map is available to mutate.";
		return false;
	}

	if (map->terrain.corner_height.empty() || map->terrain.corner_ground_texture.empty()) {
		reason = "loaded map has no terrain corners to mutate.";
		return false;
	}

	const std::size_t terrain_index = map->terrain.corner_height.size() / 2;
	const float terrain_delta = map->terrain.corner_height[terrain_index] >= Terrain::max_ground_height - 1.0f ? -0.75f : 0.75f;
	map->terrain.corner_height[terrain_index] =
		std::clamp(map->terrain.corner_height[terrain_index] + terrain_delta, Terrain::min_ground_height, Terrain::max_ground_height);
	effects.edited_terrain = true;
	if (!map->terrain.corner_water_height.empty()) {
		map->terrain.corner_water_height[terrain_index] = map->terrain.corner_height[terrain_index] + 1.25f;
	}
	if (!map->terrain.corner_water.empty()) {
		map->terrain.corner_water[terrain_index] = 1;
	}
	if (!map->terrain.corner_ground_variation.empty()) {
		map->terrain.corner_ground_variation[terrain_index] = (map->terrain.corner_ground_variation[terrain_index] + 1) % 16;
	}
	if (!map->terrain.corner_cliff_variation.empty()) {
		map->terrain.corner_cliff_variation[terrain_index] = (map->terrain.corner_cliff_variation[terrain_index] + 1) % 16;
	}
	if (!map->terrain.corner_layer_height.empty()) {
		map->terrain.corner_layer_height[terrain_index] = std::min<std::uint8_t>(
			Terrain::max_layer_height,
			static_cast<std::uint8_t>(map->terrain.corner_layer_height[terrain_index] + 1)
		);
	}
	if (map->terrain.tileset_ids.size() > 1) {
		map->terrain.corner_ground_texture[terrain_index] =
			static_cast<std::uint8_t>((map->terrain.corner_ground_texture[terrain_index] + 1) % map->terrain.tileset_ids.size());
	}
	if (!map->terrain.corner_cliff_texture.empty() && map->terrain.cliffset_ids.size() > 1) {
		map->terrain.corner_cliff_texture[terrain_index] =
			static_cast<std::uint8_t>((map->terrain.corner_cliff_texture[terrain_index] + 1) % map->terrain.cliffset_ids.size());
	}

	if (map->pathing_map.pathing_cells_static.empty()) {
		reason = "loaded map has no static pathing cells to mutate.";
		return false;
	}
	const std::size_t pathing_index = map->pathing_map.pathing_cells_static.size() / 2;
	map->pathing_map.pathing_cells_static[pathing_index] ^= PathingMap::unwalkable | PathingMap::unbuildable;
	map->pathing_map.upload_static_pathing();
	effects.edited_static_pathing = true;

	const auto& shadow_cells = map->shadow_map.data();
	if (shadow_cells.empty()) {
		reason = "loaded map has no shadow map cells to mutate.";
		return false;
	}
	const std::size_t shadow_index = shadow_cells.size() / 2;
	if (!map->shadow_map.set_cell(shadow_index, static_cast<std::uint8_t>(shadow_cells[shadow_index] ^ 0x7Fu))) {
		reason = "unable to mutate shadow map cell.";
		return false;
	}
	effects.edited_shadow_map = true;

	const float unit_x = std::max(1.0f, map->terrain.width * 0.5f);
	const float unit_y = std::max(1.0f, map->terrain.height * 0.5f);
	const std::optional<std::string> unit_id = smoke_unit_slk_row();
	if (unit_id) {
		Unit smoke_unit;
		smoke_unit.id = *unit_id;
		smoke_unit.skin_id = *unit_id;
		smoke_unit.position = glm::vec3(unit_x, unit_y, map->terrain.interpolated_height(unit_x, unit_y, true));
		smoke_unit.angle = 0.37f;
		smoke_unit.scale = glm::vec3(1.0f, 1.1f, 0.9f);
		smoke_unit.player = 0;
		smoke_unit.health = 42;
		smoke_unit.mana = 13;
		smoke_unit.random_type = 0;
		smoke_unit.random = {0, 0, 0, 0};
		map->units.units.push_back(std::move(smoke_unit));
		effects.added_unit = true;

		if (units_slk.field_to_meta_id(units_meta_slk, "name", *unit_id)) {
			units_slk.set_shadow_data(
				"name",
				*unit_id,
				std::format("HiveWE Smoke Mutated Unit {}", map->units.units.size())
			);
			effects.edited_object_table = true;
		}
	}
	const std::optional<std::string> item_id = smoke_item_slk_row();
	if (item_id) {
		Unit smoke_item;
		smoke_item.id = *item_id;
		smoke_item.skin_id = *item_id;
		smoke_item.position = glm::vec3(unit_x + 0.75f, unit_y, map->terrain.interpolated_height(unit_x + 0.75f, unit_y, true));
		smoke_item.angle = 1.125f;
		smoke_item.scale = glm::vec3(0.85f);
		smoke_item.player = 15;
		smoke_item.health = -1;
		smoke_item.mana = -1;
		smoke_item.random_type = 0;
		smoke_item.random = {0, 0, 0, 0};
		map->units.items.push_back(std::move(smoke_item));
		effects.added_item = true;
	}
	const std::optional<std::string> doodad_id = smoke_slk_row_or_first(doodads_slk, "ZZCH");
	if (doodad_id) {
		Doodad smoke_doodad;
		smoke_doodad.id = *doodad_id;
		smoke_doodad.skin_id = *doodad_id;
		smoke_doodad.variation = 0;
		smoke_doodad.position = glm::vec3(unit_x - 0.75f, unit_y, map->terrain.interpolated_height(unit_x - 0.75f, unit_y, false));
		smoke_doodad.scale = glm::vec3(1.15f, 0.95f, 1.05f);
		smoke_doodad.angle = 0.625f;
		smoke_doodad.state = Doodad::State::visible_non_solid;
		smoke_doodad.life = 77;
		smoke_doodad.creation_number = ++Doodad::auto_increment;
		map->doodads.doodads.push_back(std::move(smoke_doodad));
		effects.added_doodad = true;
	}

	map->info.author += " [HiveWE smoke mutation]";
	map->info.description += "\nHiveWE smoke mutation validates serialized metadata.";
	map->info.fog_density = std::clamp(map->info.fog_density + 0.01f, 0.0f, 1.0f);
	map->info.water_color = glm::u8vec4(33, 88, 177, 210);
	if (!map->info.players.empty()) {
		map->info.players.front().name = "HiveWE Smoke Player";
		map->info.players.front().fixed_start_position = 1;
	}
	if (!map->info.forces.empty()) {
		map->info.forces.front().name = "HiveWE Smoke Force";
		map->info.forces.front().share_vision = !map->info.forces.front().share_vision;
	}
	effects.edited_map_info = true;

	{
		auto& region = map->regions.regions.empty() ? map->regions.regions.emplace_back() : map->regions.regions.front();
		region.left = std::clamp(unit_x - 1.5f, 0.0f, static_cast<float>(std::max(1, map->terrain.width)));
		region.right = std::clamp(unit_x + 1.5f, region.left + 0.25f, static_cast<float>(std::max(2, map->terrain.width + 1)));
		region.bottom = std::clamp(unit_y - 1.0f, 0.0f, static_cast<float>(std::max(1, map->terrain.height)));
		region.top = std::clamp(unit_y + 1.0f, region.bottom + 0.25f, static_cast<float>(std::max(2, map->terrain.height + 1)));
		region.name = std::format("HiveWE Smoke Region {}", map->regions.regions.size());
		region.creation_number += 1000;
		region.weather_id = "SNls";
		region.ambient_id = "HiveWESmokeAmbient";
		region.color = glm::vec3(41.0f, 132.0f, 213.0f);
	}
	{
		auto& camera = map->cameras.cameras.empty() ? map->cameras.cameras.emplace_back() : map->cameras.cameras.front();
		camera.target_x = unit_x + 0.5f;
		camera.target_y = unit_y + 0.25f;
		camera.z_offset = 2.0f;
		camera.rotation += 3.5f;
		camera.angle_of_attack = 304.0f;
		camera.distance = 1650.0f;
		camera.roll = 1.25f;
		camera.fov = 70.0f;
		camera.far_z = 6000.0f;
		camera.near_z = 90.0f;
		camera.local_pitch = 2.0f;
		camera.local_yaw = 4.0f;
		camera.local_roll = 6.0f;
		camera.name = std::format("HiveWE Smoke Camera {}", map->cameras.cameras.size());
	}
	{
		auto& sound = map->sounds.sounds.empty() ? map->sounds.sounds.emplace_back() : map->sounds.sounds.front();
		sound.name = std::format("HiveWE Smoke Sound {}", map->sounds.sounds.size());
		sound.file = "Sound\\HiveWE\\Smoke.wav";
		sound.eax_effect = "DefaultEAXON";
		sound.looping = !sound.looping;
		sound.is_3d = true;
		sound.stop_out_of_range = true;
		sound.music = false;
		sound.fade_in_rate = 125;
		sound.fade_out_rate = 250;
		sound.volume = 96;
		sound.pitch = 1.125f;
		sound.pitch_variance = 0.0625f;
		sound.priority = 7;
		sound.channel = 1;
		sound.min_distance = 350.0f;
		sound.max_distance = 3200.0f;
		sound.distance_cutoff = 4500.0f;
		sound.cone_inside = 90.0f;
		sound.cone_outside = 180.0f;
		sound.cone_outside_volume = 64;
		sound.cone_orientation_x = 1.0f;
		sound.cone_orientation_y = 0.0f;
		sound.cone_orientation_z = 0.0f;
		if (map->sounds.version >= 2) {
			sound.editor_data.variable_name = "gg_snd_HiveWESmoke";
			sound.editor_data.label = "HiveWE Smoke Sound";
			sound.editor_data.category = "Smoke";
			sound.editor_data.sound_name = sound.name;
			sound.editor_data.sound_type = "Default";
			sound.editor_data.source = "HiveWE";
			sound.editor_data.format = "WAV";
		}
	}
	effects.edited_misc = true;

	map->triggers.global_jass_comment = "HiveWE smoke mutation global comment";
	map->triggers.global_jass = map->info.lua
		? "function HiveWESmokeMutation()\nend\n"
		: "function HiveWESmokeMutation takes nothing returns nothing\nendfunction\n";
	if (!map->triggers.variables.empty()) {
		map->triggers.variables.front().initial_value = "HiveWE smoke mutation";
		map->triggers.variables.front().is_initialized = true;
	}
	if (!map->triggers.categories.empty()) {
		map->triggers.categories.front().name += " Smoke";
	}
	if (!map->triggers.triggers.empty()) {
		map->triggers.triggers.front().description += "\nHiveWE smoke mutation touched this trigger.";
	}
	effects.edited_triggers = true;

	const fs::path import_path = map_path / "imports" / "smoke_mutation.txt";
	fs::create_directories(import_path.parent_path());
	std::ofstream import_file(import_path, std::ios::binary);
	if (!import_file) {
		reason = std::format("unable to create smoke import file {}.", import_path.string());
		return false;
	}
	import_file << "HiveWE smoke mutation import payload\n";
	import_file.close();
	if (!import_file) {
		reason = std::format("unable to write smoke import file {}.", import_path.string());
		return false;
	}
	effects.added_import = true;

	return true;
}

struct SmokeMapSnapshot {
	int terrain_width = 0;
	int terrain_height = 0;
	int pathing_width = 0;
	int pathing_height = 0;
	std::size_t shadow_width = 0;
	std::size_t shadow_height = 0;
	std::size_t unit_count = 0;
	std::size_t item_count = 0;
	std::size_t doodad_count = 0;
	std::size_t special_doodad_count = 0;
	std::size_t region_count = 0;
	std::size_t camera_count = 0;
	std::size_t sound_count = 0;
	std::size_t trigger_category_count = 0;
	std::size_t trigger_variable_count = 0;
	std::size_t trigger_count = 0;
	std::size_t player_count = 0;
	std::size_t force_count = 0;
	std::size_t import_count = 0;
	std::size_t object_modification_row_count = 0;
	std::size_t object_modification_field_count = 0;
	bool lua = false;
	std::uint64_t terrain_hash = 0;
	std::uint64_t static_pathing_hash = 0;
	std::uint64_t dynamic_pathing_hash = 0;
	std::uint64_t shadow_hash = 0;
	std::uint64_t unit_hash = 0;
	std::uint64_t doodad_hash = 0;
	std::uint64_t map_info_hash = 0;
	std::uint64_t misc_hash = 0;
	std::uint64_t trigger_hash = 0;
	std::uint64_t import_hash = 0;
	std::uint64_t object_modification_hash = 0;
	std::vector<std::string> imports;
};

SmokeMapSnapshot make_smoke_map_snapshot(const Map& current_map, const fs::path& map_path) {
	SmokeMapSnapshot snapshot;
	snapshot.terrain_width = current_map.terrain.width;
	snapshot.terrain_height = current_map.terrain.height;
	snapshot.pathing_width = current_map.pathing_map.width;
	snapshot.pathing_height = current_map.pathing_map.height;
	snapshot.shadow_width = current_map.shadow_map.map_width();
	snapshot.shadow_height = current_map.shadow_map.map_height();
	snapshot.unit_count = current_map.units.units.size();
	snapshot.item_count = current_map.units.items.size();
	snapshot.doodad_count = current_map.doodads.doodads.size();
	snapshot.special_doodad_count = current_map.doodads.special_doodads.size();
	snapshot.region_count = current_map.regions.regions.size();
	snapshot.camera_count = current_map.cameras.cameras.size();
	snapshot.sound_count = current_map.sounds.sounds.size();
	snapshot.trigger_category_count = current_map.triggers.categories.size();
	snapshot.trigger_variable_count = current_map.triggers.variables.size();
	snapshot.trigger_count = current_map.triggers.triggers.size();
	snapshot.player_count = current_map.info.players.size();
	snapshot.force_count = current_map.info.forces.size();
	snapshot.lua = current_map.info.lua;
	snapshot.imports = smoke_import_manifest(map_path);
	snapshot.import_count = snapshot.imports.size();
	snapshot.import_hash = smoke_hash_import_manifest(snapshot.imports);
	snapshot.object_modification_hash = k_fnv_offset;
	smoke_hash_object_table(
		snapshot.object_modification_hash,
		"units",
		units_slk,
		snapshot.object_modification_row_count,
		snapshot.object_modification_field_count
	);
	smoke_hash_object_table(
		snapshot.object_modification_hash,
		"items",
		items_slk,
		snapshot.object_modification_row_count,
		snapshot.object_modification_field_count
	);
	smoke_hash_object_table(
		snapshot.object_modification_hash,
		"doodads",
		doodads_slk,
		snapshot.object_modification_row_count,
		snapshot.object_modification_field_count
	);
	smoke_hash_object_table(
		snapshot.object_modification_hash,
		"destructibles",
		destructibles_slk,
		snapshot.object_modification_row_count,
		snapshot.object_modification_field_count
	);
	smoke_hash_object_table(
		snapshot.object_modification_hash,
		"abilities",
		abilities_slk,
		snapshot.object_modification_row_count,
		snapshot.object_modification_field_count
	);
	smoke_hash_object_table(
		snapshot.object_modification_hash,
		"buffs",
		buff_slk,
		snapshot.object_modification_row_count,
		snapshot.object_modification_field_count
	);
	smoke_hash_object_table(
		snapshot.object_modification_hash,
		"upgrades",
		upgrade_slk,
		snapshot.object_modification_row_count,
		snapshot.object_modification_field_count
	);

	snapshot.terrain_hash = k_fnv_offset;
	smoke_hash_value(snapshot.terrain_hash, current_map.terrain.tileset);
	smoke_hash_value(snapshot.terrain_hash, current_map.terrain.offset);
	smoke_hash_vector(snapshot.terrain_hash, current_map.terrain.corner_height);
	smoke_hash_vector(snapshot.terrain_hash, current_map.terrain.corner_water_height);
	smoke_hash_vector(snapshot.terrain_hash, current_map.terrain.corner_ground_texture);
	smoke_hash_vector(snapshot.terrain_hash, current_map.terrain.corner_ground_variation);
	smoke_hash_vector(snapshot.terrain_hash, current_map.terrain.corner_cliff_variation);
	smoke_hash_vector(snapshot.terrain_hash, current_map.terrain.corner_cliff_texture);
	smoke_hash_vector(snapshot.terrain_hash, current_map.terrain.corner_layer_height);
	smoke_hash_vector(snapshot.terrain_hash, current_map.terrain.corner_map_edge);
	smoke_hash_vector(snapshot.terrain_hash, current_map.terrain.corner_ramp);
	smoke_hash_vector(snapshot.terrain_hash, current_map.terrain.corner_blight);
	smoke_hash_vector(snapshot.terrain_hash, current_map.terrain.corner_water);
	smoke_hash_vector(snapshot.terrain_hash, current_map.terrain.corner_boundary);
	smoke_hash_vector(snapshot.terrain_hash, current_map.terrain.corner_cliff);
	smoke_hash_vector(snapshot.terrain_hash, current_map.terrain.corner_romp);
	smoke_hash_vector(snapshot.terrain_hash, current_map.terrain.corner_special_doodad);

	snapshot.static_pathing_hash = k_fnv_offset;
	smoke_hash_vector(snapshot.static_pathing_hash, current_map.pathing_map.pathing_cells_static);
	snapshot.dynamic_pathing_hash = k_fnv_offset;
	smoke_hash_vector(snapshot.dynamic_pathing_hash, current_map.pathing_map.pathing_cells_dynamic);

	snapshot.shadow_hash = k_fnv_offset;
	smoke_hash_vector(snapshot.shadow_hash, current_map.shadow_map.data());

	snapshot.unit_hash = k_fnv_offset;
	for (const auto& unit : current_map.units.units) {
		smoke_hash_unit(snapshot.unit_hash, unit);
	}
	for (const auto& item : current_map.units.items) {
		smoke_hash_unit(snapshot.unit_hash, item);
	}

	snapshot.doodad_hash = k_fnv_offset;
	for (const auto& doodad : current_map.doodads.doodads) {
		smoke_hash_doodad(snapshot.doodad_hash, doodad);
	}
	for (const auto& doodad : current_map.doodads.special_doodads) {
		smoke_hash_special_doodad(snapshot.doodad_hash, doodad);
	}

	snapshot.map_info_hash = k_fnv_offset;
	smoke_hash_map_info(snapshot.map_info_hash, current_map.info);

	snapshot.misc_hash = k_fnv_offset;
	for (const auto& region : current_map.regions.regions) {
		smoke_hash_value(snapshot.misc_hash, region.left);
		smoke_hash_value(snapshot.misc_hash, region.right);
		smoke_hash_value(snapshot.misc_hash, region.top);
		smoke_hash_value(snapshot.misc_hash, region.bottom);
		smoke_hash_string(snapshot.misc_hash, region.name);
		smoke_hash_value(snapshot.misc_hash, region.creation_number);
		smoke_hash_string(snapshot.misc_hash, region.weather_id);
		smoke_hash_string(snapshot.misc_hash, region.ambient_id);
		smoke_hash_value(snapshot.misc_hash, region.color);
	}
	for (const auto& camera_entry : current_map.cameras.cameras) {
		smoke_hash_value(snapshot.misc_hash, camera_entry.target_x);
		smoke_hash_value(snapshot.misc_hash, camera_entry.target_y);
		smoke_hash_value(snapshot.misc_hash, camera_entry.z_offset);
		smoke_hash_value(snapshot.misc_hash, camera_entry.rotation);
		smoke_hash_value(snapshot.misc_hash, camera_entry.angle_of_attack);
		smoke_hash_value(snapshot.misc_hash, camera_entry.distance);
		smoke_hash_value(snapshot.misc_hash, camera_entry.roll);
		smoke_hash_value(snapshot.misc_hash, camera_entry.fov);
		smoke_hash_value(snapshot.misc_hash, camera_entry.far_z);
		smoke_hash_value(snapshot.misc_hash, camera_entry.near_z);
		smoke_hash_value(snapshot.misc_hash, camera_entry.local_pitch);
		smoke_hash_value(snapshot.misc_hash, camera_entry.local_yaw);
		smoke_hash_value(snapshot.misc_hash, camera_entry.local_roll);
		smoke_hash_string(snapshot.misc_hash, camera_entry.name);
	}
	smoke_hash_value(snapshot.misc_hash, current_map.sounds.version);
	for (const auto& sound : current_map.sounds.sounds) {
		smoke_hash_string(snapshot.misc_hash, sound.name);
		smoke_hash_string(snapshot.misc_hash, sound.file);
		smoke_hash_string(snapshot.misc_hash, sound.eax_effect);
		smoke_hash_value(snapshot.misc_hash, sound.looping);
		smoke_hash_value(snapshot.misc_hash, sound.is_3d);
		smoke_hash_value(snapshot.misc_hash, sound.stop_out_of_range);
		smoke_hash_value(snapshot.misc_hash, sound.music);
		smoke_hash_value(snapshot.misc_hash, sound.fade_in_rate);
		smoke_hash_value(snapshot.misc_hash, sound.fade_out_rate);
		smoke_hash_value(snapshot.misc_hash, sound.volume);
		smoke_hash_value(snapshot.misc_hash, sound.pitch);
		smoke_hash_value(snapshot.misc_hash, sound.pitch_variance);
		smoke_hash_value(snapshot.misc_hash, sound.priority);
		smoke_hash_value(snapshot.misc_hash, sound.channel);
		smoke_hash_value(snapshot.misc_hash, sound.min_distance);
		smoke_hash_value(snapshot.misc_hash, sound.max_distance);
		smoke_hash_value(snapshot.misc_hash, sound.distance_cutoff);
		smoke_hash_value(snapshot.misc_hash, sound.cone_inside);
		smoke_hash_value(snapshot.misc_hash, sound.cone_outside);
		smoke_hash_value(snapshot.misc_hash, sound.cone_outside_volume);
		smoke_hash_value(snapshot.misc_hash, sound.cone_orientation_x);
		smoke_hash_value(snapshot.misc_hash, sound.cone_orientation_y);
		smoke_hash_value(snapshot.misc_hash, sound.cone_orientation_z);
		if (current_map.sounds.version >= 2) {
			smoke_hash_string(snapshot.misc_hash, sound.editor_data.variable_name);
			smoke_hash_string(snapshot.misc_hash, sound.editor_data.label);
			smoke_hash_string(snapshot.misc_hash, sound.editor_data.category);
			smoke_hash_value(snapshot.misc_hash, sound.editor_data.unknown0);
			smoke_hash_string(snapshot.misc_hash, sound.editor_data.sound_name);
			smoke_hash_value(snapshot.misc_hash, sound.editor_data.unknown1);
			smoke_hash_string(snapshot.misc_hash, sound.editor_data.sound_type);
			smoke_hash_value(snapshot.misc_hash, sound.editor_data.unknown2);
			smoke_hash_string(snapshot.misc_hash, sound.editor_data.source);
			smoke_hash_string(snapshot.misc_hash, sound.editor_data.sound_environment);
			smoke_hash_string(snapshot.misc_hash, sound.editor_data.format);
			smoke_hash_string(snapshot.misc_hash, sound.editor_data.dialogue_text);
			if (current_map.sounds.version >= 3) {
				smoke_hash_value(snapshot.misc_hash, sound.editor_data.unknown3);
			}
		}
	}

	snapshot.trigger_hash = k_fnv_offset;
	smoke_hash_triggers(snapshot.trigger_hash, current_map.triggers);
	return snapshot;
}

bool compare_smoke_map_snapshots(const SmokeMapSnapshot& before, const SmokeMapSnapshot& after, std::string& reason) {
	auto compare = [&](const auto& left, const auto& right, const std::string_view label) {
		if (left == right) {
			return true;
		}
		reason = std::format("{} changed across save/reload.", label);
		return false;
	};

	return compare(before.terrain_width, after.terrain_width, "terrain width")
		&& compare(before.terrain_height, after.terrain_height, "terrain height")
		&& compare(before.pathing_width, after.pathing_width, "pathing width")
		&& compare(before.pathing_height, after.pathing_height, "pathing height")
		&& compare(before.shadow_width, after.shadow_width, "shadow width")
		&& compare(before.shadow_height, after.shadow_height, "shadow height")
		&& compare(before.unit_count, after.unit_count, "unit count")
		&& compare(before.item_count, after.item_count, "item count")
		&& compare(before.doodad_count, after.doodad_count, "doodad count")
		&& compare(before.special_doodad_count, after.special_doodad_count, "special doodad count")
		&& compare(before.region_count, after.region_count, "region count")
		&& compare(before.camera_count, after.camera_count, "camera count")
		&& compare(before.sound_count, after.sound_count, "sound count")
		&& compare(before.trigger_category_count, after.trigger_category_count, "trigger category count")
		&& compare(before.trigger_variable_count, after.trigger_variable_count, "trigger variable count")
		&& compare(before.trigger_count, after.trigger_count, "trigger count")
		&& compare(before.player_count, after.player_count, "player count")
		&& compare(before.force_count, after.force_count, "force count")
		&& compare(before.import_count, after.import_count, "import count")
		&& compare(before.object_modification_row_count, after.object_modification_row_count, "object modification row count")
		&& compare(before.object_modification_field_count, after.object_modification_field_count, "object modification field count")
		&& compare(before.terrain_hash, after.terrain_hash, "terrain data")
		&& compare(before.static_pathing_hash, after.static_pathing_hash, "static pathing data")
		&& compare(before.dynamic_pathing_hash, after.dynamic_pathing_hash, "dynamic pathing data")
		&& compare(before.shadow_hash, after.shadow_hash, "shadow map data")
		&& compare(before.unit_hash, after.unit_hash, "unit/item data")
		&& compare(before.doodad_hash, after.doodad_hash, "doodad data")
		&& compare(before.map_info_hash, after.map_info_hash, "map info data")
		&& compare(before.misc_hash, after.misc_hash, "region/camera/sound data")
		&& compare(before.trigger_hash, after.trigger_hash, "trigger data")
		&& compare(before.object_modification_hash, after.object_modification_hash, "object modification table data")
		&& compare(before.import_hash, after.import_hash, "import manifest");
}

bool validate_smoke_mutation_effects(
	const SmokeMapSnapshot& before,
	const SmokeMapSnapshot& after,
	const SmokeMutationEffects& effects,
	std::string& reason
) {
	auto changed = [&](const auto& left, const auto& right, const std::string_view label) {
		if (left != right) {
			return true;
		}
		reason = std::format("mutation did not change {}.", label);
		return false;
	};

	if (effects.edited_terrain && !changed(before.terrain_hash, after.terrain_hash, "terrain data")) {
		return false;
	}
	if (effects.edited_static_pathing && !changed(before.static_pathing_hash, after.static_pathing_hash, "static pathing data")) {
		return false;
	}
	if (effects.edited_shadow_map && !changed(before.shadow_hash, after.shadow_hash, "shadow map data")) {
		return false;
	}
	if (effects.added_unit && !changed(before.unit_count, after.unit_count, "unit count")) {
		return false;
	}
	if (effects.added_item && !changed(before.item_count, after.item_count, "item count")) {
		return false;
	}
	if ((effects.added_unit || effects.added_item) && !changed(before.unit_hash, after.unit_hash, "unit/item data")) {
		return false;
	}
	if (effects.added_doodad && !changed(before.doodad_count, after.doodad_count, "doodad count")) {
		return false;
	}
	if (effects.added_doodad && !changed(before.doodad_hash, after.doodad_hash, "doodad data")) {
		return false;
	}
	if (effects.edited_map_info && !changed(before.map_info_hash, after.map_info_hash, "map info data")) {
		return false;
	}
	if (effects.edited_triggers && !changed(before.trigger_hash, after.trigger_hash, "trigger data")) {
		return false;
	}
	if (effects.added_import && !changed(before.import_count, after.import_count, "import count")) {
		return false;
	}
	if (effects.added_import && !changed(before.import_hash, after.import_hash, "import manifest")) {
		return false;
	}
	if (effects.edited_object_table && !changed(before.object_modification_hash, after.object_modification_hash, "object modification table data")) {
		return false;
	}
	if (effects.edited_misc && !changed(before.misc_hash, after.misc_hash, "region/camera/sound data")) {
		return false;
	}

	return true;
}

#if HIVEWE_USE_METAL_RUNTIME
struct SmokeMetalFrameCamera {
	glm::vec3 position = glm::vec3(0.0f, 0.0f, 512.0f);
	glm::vec3 direction = glm::normalize(glm::vec3(0.35f, 0.35f, -1.0f));
	glm::vec3 X = glm::vec3(1.0f, 0.0f, 0.0f);
	glm::vec3 Y = glm::vec3(0.0f, 0.0f, 1.0f);
	float distance = 1650.0f;

	bool inside_frustrum_transform(const glm::vec3&, const glm::vec3&, const glm::mat4&) const {
		return true;
	}
};

bool validate_metal_model_draws(
	const hive::metal::ModelCpuFrame& frame,
	const std::vector<hive::metal::ModelDrawCallCpuFrame>& draws,
	const std::string_view label,
	std::string& reason
) {
	for (const hive::metal::ModelDrawCallCpuFrame& draw : draws) {
		if (draw.mesh_index >= frame.meshes.size()) {
			reason = std::format("{} references mesh index {} but only {} meshes exist.", label, draw.mesh_index, frame.meshes.size());
			return false;
		}

		const hive::metal::ModelMeshCpuFrame& mesh = frame.meshes[draw.mesh_index];
		const std::size_t index_count = mesh.indices != nullptr ? mesh.indices->size() : 0u;
		const std::size_t first_index = static_cast<std::size_t>(draw.first_index);
		const std::size_t draw_index_count = static_cast<std::size_t>(draw.index_count);
		if (draw.index_count == 0u || first_index > index_count || draw_index_count > index_count - first_index) {
			reason = std::format(
				"{} has invalid index range first={} count={} for mesh with {} indices.",
				label,
				draw.first_index,
				draw.index_count,
				index_count
			);
			return false;
		}

		if (draw.instance_offset >= frame.instance_matrices.size()) {
			reason = std::format(
				"{} references instance offset {} but only {} instances exist.",
				label,
				draw.instance_offset,
				frame.instance_matrices.size()
			);
			return false;
		}

		const std::size_t bone_offset = static_cast<std::size_t>(draw.bone_offset);
		const std::size_t bone_count = static_cast<std::size_t>(draw.bone_count);
		if (draw.bone_count == 0u || bone_offset > frame.bone_matrices.size()
			|| bone_count > frame.bone_matrices.size() - bone_offset) {
			reason = std::format(
				"{} has invalid bone range offset={} count={} for {} matrices.",
				label,
				draw.bone_offset,
				draw.bone_count,
				frame.bone_matrices.size()
			);
			return false;
		}

		const std::size_t layer_offset = static_cast<std::size_t>(draw.layer_color_offset);
		const std::size_t layer_count = static_cast<std::size_t>(draw.layer_skip_count);
		if (draw.layer_skip_count == 0u || layer_offset > frame.layer_colors.size()
			|| layer_count > frame.layer_colors.size() - layer_offset) {
			reason = std::format(
				"{} has invalid layer-color range offset={} count={} for {} colors.",
				label,
				draw.layer_color_offset,
				draw.layer_skip_count,
				frame.layer_colors.size()
			);
			return false;
		}

		if (!hive::metal::detail::model_blend_factor_valid(draw.source_blend_factor)
			|| !hive::metal::detail::model_blend_factor_valid(draw.destination_blend_factor)) {
			reason = std::format(
				"{} has invalid blend factors src={} dst={}.",
				label,
				draw.source_blend_factor,
				draw.destination_blend_factor
			);
			return false;
		}

		for (std::size_t texture_slot = 0; texture_slot < draw.texture_indices.size(); ++texture_slot) {
			const std::uint32_t texture_index = draw.texture_indices[texture_slot];
			if (texture_index >= mesh.textures.size()) {
				reason = std::format(
					"{} texture slot {} references texture {} but mesh {} only has {} textures.",
					label,
					texture_slot,
					texture_index,
					draw.mesh_index,
					mesh.textures.size()
				);
				return false;
			}
			if (!mesh.textures[texture_index].valid()) {
				reason = std::format(
					"{} texture slot {} references incomplete texture {} on mesh {}.",
					label,
					texture_slot,
					texture_index,
					draw.mesh_index
				);
				return false;
			}
		}
	}

	return true;
}

bool validate_metal_model_frame(const hive::metal::ModelCpuFrame& frame, std::string& reason) {
	if (!hive::metal::model_cpu_frame_valid(frame)) {
		reason = std::format(
			"model frame fails shared Metal renderer validation with {} meshes, {} instances, {} bones, and {} draw calls.",
			frame.meshes.size(),
			frame.instance_matrices.size(),
			frame.bone_matrices.size(),
			frame.opaque_sd_draws.size() + frame.opaque_hd_draws.size() + frame.transparent_sd_draws.size()
				+ frame.transparent_hd_draws.size() + frame.picking_draws.size()
		);
		return false;
	}

	if (frame.has_draws()) {
		if (frame.instance_matrices.empty() || frame.team_color_indexes.size() != frame.instance_matrices.size()
			|| frame.bone_matrices.empty() || frame.layer_colors.empty()) {
			reason = std::format(
				"model frame has {} instances, {} team-color indexes, {} bone matrices, and {} layer colors.",
				frame.instance_matrices.size(),
				frame.team_color_indexes.size(),
				frame.bone_matrices.size(),
				frame.layer_colors.size()
			);
			return false;
		}
	}

	for (std::size_t mesh_index = 0; mesh_index < frame.meshes.size(); ++mesh_index) {
		if (!frame.meshes[mesh_index].valid()) {
			reason = std::format("model mesh frame {} is incomplete.", mesh_index);
			return false;
		}
	}

	return validate_metal_model_draws(frame, frame.opaque_sd_draws, "opaque SD model draw", reason)
		&& validate_metal_model_draws(frame, frame.opaque_hd_draws, "opaque HD model draw", reason)
		&& validate_metal_model_draws(frame, frame.transparent_sd_draws, "transparent SD model draw", reason)
		&& validate_metal_model_draws(frame, frame.transparent_hd_draws, "transparent HD model draw", reason)
		&& validate_metal_model_draws(frame, frame.picking_draws, "picking model draw", reason);
}

bool validate_metal_cliff_frame(const hive::metal::CliffCpuFrame& frame, std::string& reason) {
	if (!hive::metal::cliff_cpu_frame_valid(frame)) {
		reason = std::format(
			"cliff frame fails shared Metal renderer validation with {} meshes, {} instances, and {} draw calls.",
			frame.meshes.size(),
			frame.instances.size(),
			frame.draws.size()
		);
		return false;
	}

	for (std::size_t mesh_index = 0; mesh_index < frame.meshes.size(); ++mesh_index) {
		if (!frame.meshes[mesh_index].valid()) {
			reason = std::format("cliff mesh frame {} is incomplete.", mesh_index);
			return false;
		}
	}

	for (const hive::metal::CliffDrawCpuFrame& draw : frame.draws) {
		if (draw.mesh_index >= frame.meshes.size()) {
			reason = std::format("cliff draw references mesh index {} but only {} meshes exist.", draw.mesh_index, frame.meshes.size());
			return false;
		}

		const hive::metal::CliffMeshCpuFrame& mesh = frame.meshes[draw.mesh_index];
		const std::size_t index_count = mesh.indices != nullptr ? mesh.indices->size() : 0u;
		if (draw.index_count == 0u || draw.index_count > index_count) {
			reason = std::format("cliff draw has {} indices for mesh with {} indices.", draw.index_count, index_count);
			return false;
		}

		const std::size_t instance_offset = static_cast<std::size_t>(draw.instance_offset);
		const std::size_t instance_count = static_cast<std::size_t>(draw.instance_count);
		if (draw.instance_count == 0u || instance_offset > frame.instances.size()
			|| instance_count > frame.instances.size() - instance_offset) {
			reason = std::format(
				"cliff draw has invalid instance range offset={} count={} for {} instances.",
				draw.instance_offset,
				draw.instance_count,
				frame.instances.size()
			);
			return false;
		}
	}

	return true;
}

bool validate_metal_post_reload_renderer_frames(const std::string_view label, std::string& reason) {
	if (!map || !map->loaded) {
		reason = "map is not loaded.";
		return false;
	}

	const hive::metal::TerrainCpuFrame terrain_frame = hive::metal::make_terrain_cpu_frame(map->terrain);
	if (!terrain_frame.valid()) {
		reason = std::format(
			"terrain frame is invalid for {}x{} map with {} ground heights and {} ground texture tiles.",
			terrain_frame.width,
			terrain_frame.height,
			terrain_frame.corner_ground_heights.size(),
			terrain_frame.ground_texture_indices.size()
		);
		return false;
	}

	const hive::metal::PathingCpuFrame pathing_frame = hive::metal::make_pathing_cpu_frame(map->pathing_map);
	if (!pathing_frame.valid()) {
		reason = std::format(
			"pathing frame is invalid for {}x{} pathing map with {} static and {} dynamic cells.",
			pathing_frame.width,
			pathing_frame.height,
			pathing_frame.cells_static.size(),
			pathing_frame.cells_dynamic.size()
		);
		return false;
	}

	const hive::metal::TerrainTextureCpuFrame texture_frame = hive::metal::make_terrain_texture_cpu_frame(map->terrain);
	const auto& ground_textures = map->terrain.ground_texture_resources();
	if (!ground_textures.empty() && !texture_frame.ground_valid()) {
		reason = std::format("ground material texture frame is invalid for {} source textures.", ground_textures.size());
		return false;
	}
	if (texture_frame.ground_valid() && texture_frame.ground_texture_layer_offsets.size() != ground_textures.size()) {
		reason = std::format(
			"ground material texture frame has {} slot offsets for {} source textures.",
			texture_frame.ground_texture_layer_offsets.size(),
			ground_textures.size()
		);
		return false;
	}
	if (!map->terrain.water_textures.empty() && !texture_frame.water_valid()) {
		reason = std::format("water material texture frame is invalid for {} source textures.", map->terrain.water_textures.size());
		return false;
	}
	if (texture_frame.water_valid()
		&& texture_frame.water_layer_count != static_cast<int>(map->terrain.water_textures.size())) {
		reason = std::format(
			"water material texture frame has {} layers for {} source textures.",
			texture_frame.water_layer_count,
			map->terrain.water_textures.size()
		);
		return false;
	}
	if (!map->terrain.cliff_textures.empty() && !texture_frame.cliff_valid()) {
		reason = std::format("cliff material texture frame is invalid for {} source textures.", map->terrain.cliff_textures.size());
		return false;
	}
	if (texture_frame.cliff_valid()
		&& texture_frame.cliff_layer_count != static_cast<int>(map->terrain.cliff_textures.size())) {
		reason = std::format(
			"cliff material texture frame has {} layers for {} source textures.",
			texture_frame.cliff_layer_count,
			map->terrain.cliff_textures.size()
		);
		return false;
	}
	hive::metal::MetalMapFrame material_frame;
	material_frame.terrain = terrain_frame;
	material_frame.textures = &texture_frame;
	if (texture_frame.water_layer_count > 0) {
		material_frame.water_texture_index = static_cast<std::uint32_t>(
			std::clamp(static_cast<int>(map->terrain.current_texture), 0, texture_frame.water_layer_count - 1)
		);
	}
	if (!hive::metal::metal_map_frame_texture_indices_valid(material_frame)) {
		reason = "map material frame has terrain or water texture indices outside the uploaded texture frame.";
		return false;
	}

	const hive::metal::CliffCpuFrame cliff_frame = hive::metal::make_cliff_cpu_frame(map->terrain);
	if (!validate_metal_cliff_frame(cliff_frame, reason)) {
		return false;
	}

	const SmokeMetalFrameCamera camera;
	const hive::metal::ModelCpuFrame model_frame = hive::metal::make_model_cpu_frame(*map, camera);
	if (!validate_metal_model_frame(model_frame, reason)) {
		return false;
	}

	const hive::metal::ModelCpuFrame unit_picking_frame = hive::metal::make_unit_picking_cpu_frame(*map, camera);
	if (!validate_metal_model_frame(unit_picking_frame, reason)) {
		reason = std::format("unit picking frame is invalid: {}", reason);
		return false;
	}

	const hive::metal::ModelCpuFrame doodad_picking_frame = hive::metal::make_doodad_picking_cpu_frame(*map, camera);
	if (!validate_metal_model_frame(doodad_picking_frame, reason)) {
		reason = std::format("doodad picking frame is invalid: {}", reason);
		return false;
	}

	const hive::metal::ParticleEmitter2CpuFrame particle_frame = hive::metal::make_particle_emitter2_cpu_frame(*map, camera);
	if (!hive::metal::particle_emitter2_frame_valid(particle_frame)) {
		reason = std::format(
			"particle frame has {} positions, {} UVs, {} colors, {} textures, and {} draws.",
			particle_frame.positions.size(),
			particle_frame.uvs.size(),
			particle_frame.colors.size(),
			particle_frame.textures.size(),
			particle_frame.draws.size()
		);
		return false;
	}
	for (std::size_t texture_index = 0; texture_index < particle_frame.textures.size(); ++texture_index) {
		if (!particle_frame.textures[texture_index].valid()) {
			reason = std::format("particle texture {} is incomplete.", texture_index);
			return false;
		}
	}

	qInfo() << QString::fromStdString(std::string(label))
			<< "validated Metal post-reload renderer frames:"
			<< "terrain" << terrain_frame.width << "x" << terrain_frame.height
			<< "groundLayers" << texture_frame.ground_layer_count
			<< "waterLayers" << texture_frame.water_layer_count
			<< "cliffLayers" << texture_frame.cliff_layer_count
			<< "cliffDraws" << cliff_frame.draws.size()
			<< "modelMeshes" << model_frame.meshes.size()
			<< "modelDraws"
			<< (model_frame.opaque_sd_draws.size() + model_frame.opaque_hd_draws.size()
				+ model_frame.transparent_sd_draws.size() + model_frame.transparent_hd_draws.size())
			<< "unitPickingDraws" << unit_picking_frame.picking_draws.size()
			<< "doodadPickingDraws" << doodad_picking_frame.picking_draws.size()
			<< "particleDraws" << particle_frame.draws.size();
	return true;
}
#else
bool validate_metal_post_reload_renderer_frames(const std::string_view, std::string&) {
	return true;
}
#endif

} // namespace

SmokeSaveReloadResult run_smoke_save_reload(HiveWE& window) {
	SmokeSaveReloadResult result;
	if (!map || !map->loaded) {
		qCritical() << "Smoke save/reload requested before the startup map was loaded.";
		return result;
	}

	result.temp_dir = std::make_unique<QTemporaryDir>();
	if (!result.temp_dir->isValid()) {
		qCritical() << "Unable to create temporary directory for smoke save/reload.";
		result.temp_dir.reset();
		return result;
	}

	const fs::path original_map_path = map->filesystem_path;
	const SmokeMapSnapshot pre_save_snapshot = make_smoke_map_snapshot(*map, original_map_path);
	const std::string saved_map_name = map->name.empty() ? "HiveWE-smoke-map" : map->name;
	const fs::path saved_map_path = fs::path(result.temp_dir->path().toStdString()) / saved_map_name;
	if (!map->save(saved_map_path)) {
		qCritical() << "Smoke save/reload failed while saving to" << QString::fromStdString(saved_map_path.string());
		return result;
	}

	std::string saved_manifest_error;
	if (!validate_smoke_saved_map_manifest(original_map_path, saved_map_path, pre_save_snapshot.lua, saved_manifest_error)) {
		qCritical() << "Smoke save/reload saved map manifest failed:"
					<< QString::fromStdString(saved_manifest_error);
		return result;
	}

	window.load_map(saved_map_path);
	if (!map || !map->loaded) {
		qCritical() << "Smoke save/reload failed while reloading" << QString::fromStdString(saved_map_path.string());
		return result;
	}

	const SmokeMapSnapshot post_reload_snapshot = make_smoke_map_snapshot(*map, saved_map_path);
	std::string snapshot_mismatch;
	if (!compare_smoke_map_snapshots(pre_save_snapshot, post_reload_snapshot, snapshot_mismatch)) {
		qCritical() << "Smoke save/reload state validation failed:" << QString::fromStdString(snapshot_mismatch);
		return result;
	}
	std::string renderer_frame_error;
	if (!validate_metal_post_reload_renderer_frames("Smoke save/reload", renderer_frame_error)) {
		qCritical() << "Smoke save/reload Metal renderer-frame validation failed:"
					<< QString::fromStdString(renderer_frame_error);
		return result;
	}

	qInfo() << "Smoke save/reload succeeded:" << QString::fromStdString(saved_map_path.string());
	qInfo() << "Smoke save/reload validated terrain, static/dynamic pathing, shadows, objects, object modifications, triggers, imports, map metadata, and Metal renderer frames.";
	result.ok = true;
	return result;
}

SmokeSaveReloadResult run_smoke_mutated_save_reload(HiveWE& window) {
	SmokeSaveReloadResult result;
	if (!map || !map->loaded) {
		qCritical() << "Smoke mutated save/reload requested before the startup map was loaded.";
		return result;
	}

	result.temp_dir = std::make_unique<QTemporaryDir>();
	if (!result.temp_dir->isValid()) {
		qCritical() << "Unable to create temporary directory for smoke mutated save/reload.";
		result.temp_dir.reset();
		return result;
	}

	const fs::path temp_root = fs::path(result.temp_dir->path().toStdString());
	const std::string map_name = map->name.empty() ? "HiveWE-smoke-map" : map->name;
	const fs::path staged_map_path = temp_root / "mutation-source" / map_name;
	fs::create_directories(staged_map_path.parent_path());
	if (!map->save(staged_map_path)) {
		qCritical() << "Smoke mutated save/reload failed while staging map at"
					<< QString::fromStdString(staged_map_path.string());
		return result;
	}

	window.load_map(staged_map_path);
	if (!map || !map->loaded) {
		qCritical() << "Smoke mutated save/reload failed while loading staged map"
					<< QString::fromStdString(staged_map_path.string());
		return result;
	}

	const SmokeMapSnapshot staged_snapshot = make_smoke_map_snapshot(*map, staged_map_path);
	SmokeMutationEffects mutation_effects;
	std::string mutation_error;
	if (!apply_smoke_map_mutation(staged_map_path, mutation_effects, mutation_error)) {
		qCritical() << "Smoke mutated save/reload failed while mutating staged map:"
					<< QString::fromStdString(mutation_error);
		return result;
	}

	const bool mutated_lua = map->info.lua;
	const fs::path normalized_map_path = temp_root / "mutation-normalized" / map_name;
	fs::create_directories(normalized_map_path.parent_path());
	if (!map->save(normalized_map_path)) {
		qCritical() << "Smoke mutated save/reload failed while saving normalized map to"
					<< QString::fromStdString(normalized_map_path.string());
		return result;
	}

	std::string normalized_manifest_error;
	if (!validate_smoke_saved_map_manifest(staged_map_path, normalized_map_path, mutated_lua, normalized_manifest_error)) {
		qCritical() << "Smoke mutated save/reload normalized map manifest failed:"
					<< QString::fromStdString(normalized_manifest_error);
		return result;
	}

	window.load_map(normalized_map_path);
	if (!map || !map->loaded) {
		qCritical() << "Smoke mutated save/reload failed while reloading normalized map"
					<< QString::fromStdString(normalized_map_path.string());
		return result;
	}

	const SmokeMapSnapshot normalized_mutated_snapshot = make_smoke_map_snapshot(*map, normalized_map_path);
	std::string missing_mutation;
	if (!validate_smoke_mutation_effects(staged_snapshot, normalized_mutated_snapshot, mutation_effects, missing_mutation)) {
		qCritical() << "Smoke mutated save/reload mutation validation failed:"
					<< QString::fromStdString(missing_mutation);
		return result;
	}

	const fs::path saved_map_path = temp_root / "mutation-saved" / map_name;
	fs::create_directories(saved_map_path.parent_path());
	if (!map->save(saved_map_path)) {
		qCritical() << "Smoke mutated save/reload failed while saving to"
					<< QString::fromStdString(saved_map_path.string());
		return result;
	}

	std::string saved_manifest_error;
	if (!validate_smoke_saved_map_manifest(normalized_map_path, saved_map_path, normalized_mutated_snapshot.lua, saved_manifest_error)) {
		qCritical() << "Smoke mutated save/reload saved map manifest failed:"
					<< QString::fromStdString(saved_manifest_error);
		return result;
	}

	window.load_map(saved_map_path);
	if (!map || !map->loaded) {
		qCritical() << "Smoke mutated save/reload failed while reloading"
					<< QString::fromStdString(saved_map_path.string());
		return result;
	}

	const SmokeMapSnapshot post_reload_snapshot = make_smoke_map_snapshot(*map, saved_map_path);
	std::string snapshot_mismatch;
	if (!compare_smoke_map_snapshots(normalized_mutated_snapshot, post_reload_snapshot, snapshot_mismatch)) {
		qCritical() << "Smoke mutated save/reload state validation failed:" << QString::fromStdString(snapshot_mismatch);
		return result;
	}
	std::string renderer_frame_error;
	if (!validate_metal_post_reload_renderer_frames("Smoke mutated save/reload", renderer_frame_error)) {
		qCritical() << "Smoke mutated save/reload Metal renderer-frame validation failed:"
					<< QString::fromStdString(renderer_frame_error);
		return result;
	}

	qInfo() << "Smoke mutated save/reload succeeded:" << QString::fromStdString(saved_map_path.string());
	qInfo() << "Smoke mutated save/reload validated edited terrain, static pathing, shadows, objects, object modifications, triggers, imports, map metadata, and Metal renderer frames.";
	result.ok = true;
	return result;
}

SmokeSaveReloadResult run_smoke_mpq_export_reload(HiveWE& window) {
	SmokeSaveReloadResult result;
	if (!map || !map->loaded) {
		qCritical() << "Smoke MPQ export/reload requested before the startup map was loaded.";
		return result;
	}

	result.temp_dir = std::make_unique<QTemporaryDir>();
	if (!result.temp_dir->isValid()) {
		qCritical() << "Unable to create temporary directory for smoke MPQ export/reload.";
		result.temp_dir.reset();
		return result;
	}

	const fs::path original_map_path = map->filesystem_path;
	const SmokeMapSnapshot pre_export_snapshot = make_smoke_map_snapshot(*map, original_map_path);
	const std::string saved_map_name = map->name.empty() ? "HiveWE-smoke-map" : map->name;
	const fs::path temp_root = fs::path(result.temp_dir->path().toStdString());
	const fs::path archive_source_path = temp_root / "archive-source" / saved_map_name;
	fs::create_directories(archive_source_path.parent_path());
	if (!map->save(archive_source_path)) {
		qCritical() << "Smoke MPQ export/reload failed while staging map at"
					<< QString::fromStdString(archive_source_path.string());
		return result;
	}

	std::string staged_manifest_error;
	if (!validate_smoke_saved_map_manifest(original_map_path, archive_source_path, pre_export_snapshot.lua, staged_manifest_error)) {
		qCritical() << "Smoke MPQ export/reload staged map manifest failed:"
					<< QString::fromStdString(staged_manifest_error);
		return result;
	}

	const fs::path archive_path = temp_root / (saved_map_name + ".w3x");
	std::string archive_error;
	if (!export_smoke_map_archive(archive_source_path, archive_path, archive_error)) {
		qCritical() << "Smoke MPQ export/reload export failed:" << QString::fromStdString(archive_error);
		return result;
	}

	mpq::MPQ archive;
	if (!archive.open(archive_path)) {
		qCritical() << "Smoke MPQ export/reload failed while reopening archive"
					<< QString::fromStdString(archive_path.string()) << "with error" << GetLastError();
		return result;
	}

	const fs::path extracted_map_path = temp_root / "archive-extracted" / saved_map_name;
	fs::create_directories(extracted_map_path);
	if (!archive.unpack(extracted_map_path)) {
		qCritical() << "Smoke MPQ export/reload failed while unpacking archive"
					<< QString::fromStdString(archive_path.string());
		return result;
	}
	archive.close();

	std::string extracted_manifest_error;
	if (!validate_smoke_saved_map_manifest(archive_source_path, extracted_map_path, pre_export_snapshot.lua, extracted_manifest_error)) {
		qCritical() << "Smoke MPQ export/reload extracted map manifest failed:"
					<< QString::fromStdString(extracted_manifest_error);
		return result;
	}

	window.load_map(extracted_map_path);
	if (!map || !map->loaded) {
		qCritical() << "Smoke MPQ export/reload failed while reloading extracted archive"
					<< QString::fromStdString(extracted_map_path.string());
		return result;
	}

	const SmokeMapSnapshot post_reload_snapshot = make_smoke_map_snapshot(*map, extracted_map_path);
	std::string snapshot_mismatch;
	if (!compare_smoke_map_snapshots(pre_export_snapshot, post_reload_snapshot, snapshot_mismatch)) {
		qCritical() << "Smoke MPQ export/reload state validation failed:" << QString::fromStdString(snapshot_mismatch);
		return result;
	}
	std::string renderer_frame_error;
	if (!validate_metal_post_reload_renderer_frames("Smoke MPQ export/reload", renderer_frame_error)) {
		qCritical() << "Smoke MPQ export/reload Metal renderer-frame validation failed:"
					<< QString::fromStdString(renderer_frame_error);
		return result;
	}

	qInfo() << "Smoke MPQ export/reload succeeded:" << QString::fromStdString(archive_path.string());
	qInfo() << "Smoke MPQ export/reload validated archive export, unpack, map reload, map state preservation, and Metal renderer frames.";
	result.ok = true;
	return result;
}
