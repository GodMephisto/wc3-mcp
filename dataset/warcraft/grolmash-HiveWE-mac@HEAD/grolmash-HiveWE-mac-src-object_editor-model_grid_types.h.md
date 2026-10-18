---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/object_editor/model_grid_types.h"
ref: HEAD
---

#pragma once

#include <filesystem>
#include <optional>
#include <string>
#include <utility>

enum class ModelCategory {
	Abilities,
	Buildings,
	Doodads,
	Environment,
	Objects,
	SharedModels,
	Units,
	Map,
	Count
};

struct ModelEntry {
	std::filesystem::path path;
	ModelCategory category;
	std::optional<std::pair<int, std::string>> replaceable_id_override = std::nullopt;
};

inline std::string model_grid_skinned_mesh_cache_identifier(const std::optional<std::pair<int, std::string>>& replaceable_id_override) {
	if (!replaceable_id_override) {
		return "";
	}
	return "replaceable:" + std::to_string(replaceable_id_override->first) + ":" + replaceable_id_override->second;
}
