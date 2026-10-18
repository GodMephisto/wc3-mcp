---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/rendering/metal_map_renderer.h"
ref: HEAD
---

#pragma once

#include <algorithm>
#include <array>
#include <cmath>
#include <cstddef>
#include <cstdint>
#include <iterator>
#include <limits>
#include <memory>
#include <optional>
#include <string>
#include <type_traits>
#include <unordered_map>
#include <utility>
#include <vector>

#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>

#include "rendering/metal_model_frame_stats.h"

namespace hive::metal {

[[nodiscard]]
inline std::optional<std::size_t> checked_frame_element_count(const int width, const int height) {
	if (width <= 0 || height <= 0) {
		return std::nullopt;
	}
	const std::size_t w = static_cast<std::size_t>(width);
	const std::size_t h = static_cast<std::size_t>(height);
	constexpr std::size_t max_element_count = static_cast<std::size_t>(std::numeric_limits<int>::max());
	if (w > max_element_count / h) {
		return std::nullopt;
	}
	return w * h;
}

[[nodiscard]]
inline std::optional<std::size_t> checked_terrain_tile_count(const int width, const int height) {
	if (width <= 1 || height <= 1) {
		return std::nullopt;
	}
	return checked_frame_element_count(width - 1, height - 1);
}

[[nodiscard]]
inline std::optional<std::size_t> checked_frame_byte_count(const int width, const int height, const int channels) {
	if (channels <= 0) {
		return std::nullopt;
	}
	const auto pixel_count = checked_frame_element_count(width, height);
	if (!pixel_count) {
		return std::nullopt;
	}
	const std::size_t channel_count = static_cast<std::size_t>(channels);
	if (*pixel_count > std::numeric_limits<std::size_t>::max() / channel_count) {
		return std::nullopt;
	}
	return *pixel_count * channel_count;
}

[[nodiscard]]
inline std::optional<std::size_t> checked_rgba8_bytes_per_row(const int width) {
	return checked_frame_byte_count(width, 1, 4);
}

[[nodiscard]]
inline bool r8_texture_source_size_valid(const int width, const int height, const std::size_t value_count) {
	const auto pixel_count = checked_frame_element_count(width, height);
	return pixel_count && value_count == *pixel_count;
}

struct FrameContext {
	// Opaque Objective-C objects consumed by metal_map_renderer.mm.
	// Expected types: id<MTLDevice>, id<MTLCommandQueue>, id<MTLCommandBuffer>, and MTLRenderPassDescriptor*.
	void* device = nullptr;
	void* command_queue = nullptr;
	void* command_buffer = nullptr;
	void* render_pass_descriptor = nullptr;

	int drawable_width = 0;
	int drawable_height = 0;
	double scale = 1.0;

	// Pixel-space subregion for renderers that share one drawable across several preview cells.
	// Non-positive sizes mean "use the full drawable".
	int viewport_x = 0;
	int viewport_y = 0;
	int viewport_width = 0;
	int viewport_height = 0;
	int scissor_x = 0;
	int scissor_y = 0;
	int scissor_width = 0;
	int scissor_height = 0;
	bool load_existing_render_target = false;

	// MTLPixelFormatBGRA8Unorm and MTLPixelFormatDepth32Float.
	std::uint32_t color_pixel_format = 80;
	std::uint32_t depth_pixel_format = 252;
};

struct CameraUniforms {
	glm::mat4 projection_view = glm::mat4(1.0f);
};

[[nodiscard]]
inline glm::mat4 opengl_clip_depth_to_metal(const glm::mat4& projection_view) {
	glm::mat4 clip_depth = glm::mat4(1.0f);
	clip_depth[2][2] = 0.5f;
	clip_depth[3][2] = 0.5f;
	return clip_depth * projection_view;
}

struct BrushMask {
	const void* source_key = nullptr;
	std::uint64_t revision = 0;
	int width = 0;
	int height = 0;
	glm::vec2 world_position = glm::vec2(0.0f);
	std::vector<glm::u8vec4> rgba8;

	[[nodiscard]]
	bool valid() const {
		const auto pixel_count = checked_frame_element_count(width, height);
		return pixel_count && rgba8.size() == *pixel_count;
	}
};

struct TerrainCpuFrame {
	const void* source_key = nullptr;
	int width = 0;
	int height = 0;
	bool includes_ground = true;
	bool includes_water = true;
	std::uint64_t ground_height_revision = 0;
	std::uint64_t final_ground_height_revision = 0;
	std::uint64_t ground_texture_data_revision = 0;
	std::uint64_t ground_exists_revision = 0;
	std::uint64_t water_height_revision = 0;
	std::uint64_t water_exists_revision = 0;

	std::vector<float> corner_ground_heights;
	std::vector<float> final_ground_heights;
	std::vector<glm::uvec4> ground_texture_indices;
	std::vector<std::uint32_t> ground_exists;

	std::vector<float> corner_water_heights;
	std::vector<std::uint32_t> water_exists;

	[[nodiscard]]
	bool valid() const {
		const auto corner_count = checked_frame_element_count(width, height);
		const auto tile_count = checked_terrain_tile_count(width, height);

		return corner_count && tile_count && corner_ground_heights.size() == *corner_count
			&& final_ground_heights.size() == *corner_count && ground_texture_indices.size() == *tile_count
			&& ground_exists.size() == *tile_count && corner_water_heights.size() == *corner_count
			&& water_exists.size() == *corner_count && std::ranges::all_of(ground_exists, [](const std::uint32_t value) {
				return value <= 1u;
			}) && std::ranges::all_of(water_exists, [](const std::uint32_t value) {
				return value <= 1u;
			});
	}
};

struct CliffMeshCpuFrame {
	const void* key = nullptr;
	std::uint64_t revision = 0;
	const std::vector<glm::vec3>* vertices = nullptr;
	const std::vector<glm::vec2>* uvs = nullptr;
	const std::vector<glm::vec3>* normals = nullptr;
	const std::vector<std::uint16_t>* indices = nullptr;

	[[nodiscard]]
	bool valid() const {
		return key != nullptr && vertices != nullptr && uvs != nullptr && normals != nullptr && indices != nullptr && !vertices->empty()
			&& uvs->size() == vertices->size() && normals->size() == vertices->size() && !indices->empty()
			&& std::ranges::all_of(*indices, [this](const std::uint16_t index) {
				   return static_cast<std::size_t>(index) < vertices->size();
			   });
	}
};

struct CliffDrawCpuFrame {
	std::uint32_t mesh_index = 0;
	std::uint32_t instance_offset = 0;
	std::uint32_t instance_count = 0;
	std::uint32_t index_count = 0;
};

struct CliffCpuFrame {
	std::vector<CliffMeshCpuFrame> meshes;
	std::vector<glm::vec4> instances;
	std::vector<CliffDrawCpuFrame> draws;

	[[nodiscard]]
	bool has_draws() const {
		return !draws.empty();
	}
};

inline bool cliff_cpu_frame_valid(const CliffCpuFrame& frame) {
	if (!frame.has_draws()) {
		return true;
	}
	if (frame.meshes.empty() || frame.instances.empty()) {
		return false;
	}
	for (const CliffMeshCpuFrame& mesh : frame.meshes) {
		if (!mesh.valid()) {
			return false;
		}
	}
	for (const CliffDrawCpuFrame& draw : frame.draws) {
		if (draw.mesh_index >= frame.meshes.size() || draw.instance_count == 0u || draw.index_count == 0u
			|| draw.index_count % 3u != 0u) {
			return false;
		}
		if (draw.instance_offset > frame.instances.size()
			|| static_cast<std::size_t>(draw.instance_count) > frame.instances.size() - static_cast<std::size_t>(draw.instance_offset)) {
			return false;
		}
		const CliffMeshCpuFrame& mesh = frame.meshes[draw.mesh_index];
		if (static_cast<std::size_t>(draw.index_count) > mesh.indices->size()) {
			return false;
		}
	}
	return true;
}

struct PathingCpuFrame {
	const void* source_key = nullptr;
	int width = 0;
	int height = 0;
	std::uint64_t static_revision = 0;
	std::uint64_t dynamic_revision = 0;
	std::vector<std::uint8_t> cells_static;
	std::vector<std::uint8_t> cells_dynamic;

	[[nodiscard]]
	bool valid() const {
		const auto cell_count = checked_frame_element_count(width, height);
		return cell_count && cells_static.size() == *cell_count && cells_dynamic.size() == *cell_count;
	}
};

struct TerrainTextureCpuFrame {
	std::uint64_t revision = 0;

	int ground_tile_size = 0;
	int ground_layer_count = 0;
	std::vector<std::uint32_t> ground_texture_layer_offsets;
	std::vector<std::uint8_t> ground_rgba8;

	int water_width = 0;
	int water_height = 0;
	int water_layer_count = 0;
	std::vector<std::uint8_t> water_rgba8;

	int cliff_width = 0;
	int cliff_height = 0;
	int cliff_layer_count = 0;
	std::vector<std::uint8_t> cliff_rgba8;

	[[nodiscard]]
	static std::optional<std::size_t> rgba8_layer_byte_size(const int width, const int height) {
		return checked_frame_byte_count(width, height, 4);
	}

	[[nodiscard]]
	static std::optional<std::size_t> rgba8_total_byte_size(const int width, const int height, const int layers) {
		if (layers <= 0) {
			return std::nullopt;
		}
		const auto layer_size = rgba8_layer_byte_size(width, height);
		if (!layer_size) {
			return std::nullopt;
		}
		const std::size_t layer_count = static_cast<std::size_t>(layers);
		if (*layer_size > std::numeric_limits<std::size_t>::max() / layer_count) {
			return std::nullopt;
		}
		return *layer_size * layer_count;
	}

	[[nodiscard]]
	bool ground_valid() const {
		const auto total_size = rgba8_total_byte_size(ground_tile_size, ground_tile_size, ground_layer_count);
		return ground_tile_size > 0 && ground_layer_count > 0 && !ground_texture_layer_offsets.empty()
			&& ground_texture_layer_offsets.front() == 0u && std::ranges::is_sorted(ground_texture_layer_offsets)
			&& std::ranges::all_of(ground_texture_layer_offsets, [this](const std::uint32_t offset) {
				   return offset < static_cast<std::uint32_t>(ground_layer_count);
			   })
			&& total_size && ground_rgba8.size() == *total_size;
	}

	[[nodiscard]]
	bool water_valid() const {
		const auto total_size = rgba8_total_byte_size(water_width, water_height, water_layer_count);
		return water_width > 0 && water_height > 0 && water_layer_count > 0 && total_size && water_rgba8.size() == *total_size;
	}

	[[nodiscard]]
	bool cliff_valid() const {
		const auto total_size = rgba8_total_byte_size(cliff_width, cliff_height, cliff_layer_count);
		return cliff_width > 0 && cliff_height > 0 && cliff_layer_count > 0 && total_size && cliff_rgba8.size() == *total_size;
	}
};

struct ModelTextureCpuSource {
	const void* key = nullptr;
	int width = 0;
	int height = 0;
	int channels = 0;
	int wrap_flags = 0;
	bool srgb = false;
	const std::vector<std::uint8_t>* rgba8 = nullptr;

	[[nodiscard]]
	bool valid() const {
		const auto expected_size = checked_frame_byte_count(width, height, channels);
		return key != nullptr && rgba8 != nullptr && channels >= 3 && wrap_flags >= 0 && wrap_flags <= 3 && expected_size
			&& rgba8->size() >= *expected_size;
	}
};

struct ModelMeshCpuFrame {
	const void* key = nullptr;
	std::uint64_t revision = 0;
	const std::vector<glm::uvec2>* vertices_snorm = nullptr;
	const std::vector<std::uint32_t>* uvs_snorm = nullptr;
	const std::vector<std::uint32_t>* normals_oct_snorm = nullptr;
	const std::vector<glm::vec4>* tangents = nullptr;
	const std::vector<glm::u8vec4>* skin_weights = nullptr;
	const std::vector<std::uint16_t>* indices = nullptr;
	std::vector<ModelTextureCpuSource> textures;

	[[nodiscard]]
	bool valid() const {
		return key != nullptr && vertices_snorm != nullptr && uvs_snorm != nullptr && normals_oct_snorm != nullptr
			&& tangents != nullptr && skin_weights != nullptr && indices != nullptr && !vertices_snorm->empty()
			&& uvs_snorm->size() == vertices_snorm->size() && normals_oct_snorm->size() == vertices_snorm->size()
			&& tangents->size() == vertices_snorm->size() && skin_weights->size() == vertices_snorm->size() * 2
			&& !indices->empty();
	}
};

struct ModelDrawCallCpuFrame {
	std::uint32_t mesh_index = 0;
	std::uint32_t index_count = 0;
	std::uint32_t first_index = 0;
	std::int32_t base_vertex = 0;

	std::uint32_t instance_offset = 0;
	std::uint32_t bone_offset = 0;
	std::uint32_t bone_count = 0;
	std::uint32_t layer_color_offset = 0;
	std::uint32_t layer_skip_count = 0;
	std::uint32_t layer_index_local = 0;

	std::array<std::uint32_t, 6> texture_indices = {0, 0, 0, 0, 0, 0};
	float alpha_test = 0.01f;
	float sort_distance = 0.0f;
	glm::vec4 hd_material_params0 = glm::vec4(1.0f, 0.0f, 0.0f, 0.0f);
	glm::vec4 hd_material_params1 = glm::vec4(0.0f);
	std::uint32_t layer_lit = 1;
	std::uint32_t is_team_color = 0;
	std::uint32_t pick_id = 0;

	std::uint32_t source_blend_factor = 1;
	std::uint32_t destination_blend_factor = 0;
	std::uint32_t cull_face = 1;
	std::uint32_t depth_test = 1;
	std::uint32_t depth_mask = 1;
	std::uint32_t picking_depth_mask = 1;
};

struct ModelCpuFrame {
	std::vector<ModelMeshCpuFrame> meshes;
	std::vector<glm::mat4> instance_matrices;
	std::vector<std::uint32_t> team_color_indexes;
	std::vector<glm::mat4> bone_matrices;
	std::vector<glm::vec4> layer_colors;
	std::vector<ModelDrawCallCpuFrame> opaque_sd_draws;
	std::vector<ModelDrawCallCpuFrame> opaque_hd_draws;
	std::vector<ModelDrawCallCpuFrame> transparent_sd_draws;
	std::vector<ModelDrawCallCpuFrame> transparent_hd_draws;
	std::vector<ModelDrawCallCpuFrame> picking_draws;

	[[nodiscard]]
	bool has_draws() const {
		return !opaque_sd_draws.empty() || !opaque_hd_draws.empty() || !transparent_sd_draws.empty() || !transparent_hd_draws.empty()
			|| !picking_draws.empty();
	}
};

namespace detail {
inline constexpr std::uint32_t max_model_team_color_index = 27u;
inline constexpr std::uint32_t max_model_pick_id = 0x00FFFFFFu;

[[nodiscard]]
inline bool model_blend_factor_valid(std::uint32_t value);
}

inline bool model_draw_call_valid(const ModelCpuFrame& frame, const ModelDrawCallCpuFrame& draw) {
	if (draw.index_count == 0u || draw.index_count % 3u != 0u || draw.mesh_index >= frame.meshes.size()) {
		return false;
	}
	if (!detail::model_blend_factor_valid(draw.source_blend_factor)
		|| !detail::model_blend_factor_valid(draw.destination_blend_factor) || draw.cull_face > 1u || draw.depth_test > 1u
		|| draw.depth_mask > 1u || draw.picking_depth_mask > 1u) {
		return false;
	}
	if (draw.layer_lit > 1u || draw.is_team_color > 1u || draw.pick_id > detail::max_model_pick_id) {
		return false;
	}
	if (draw.instance_offset >= frame.instance_matrices.size() || draw.instance_offset >= frame.team_color_indexes.size()) {
		return false;
	}
	if (draw.bone_count == 0u || draw.bone_offset > frame.bone_matrices.size()
		|| static_cast<std::size_t>(draw.bone_count) > frame.bone_matrices.size() - static_cast<std::size_t>(draw.bone_offset)) {
		return false;
	}
	if (draw.layer_skip_count == 0u || draw.layer_index_local >= draw.layer_skip_count || draw.layer_color_offset > frame.layer_colors.size()
		|| static_cast<std::size_t>(draw.layer_skip_count) > frame.layer_colors.size() - static_cast<std::size_t>(draw.layer_color_offset)) {
		return false;
	}
	if (draw.base_vertex < 0) {
		return false;
	}

	const ModelMeshCpuFrame& mesh = frame.meshes[draw.mesh_index];
	if (!mesh.valid()) {
		return false;
	}
	const std::size_t first_index = static_cast<std::size_t>(draw.first_index);
	const std::size_t index_count = static_cast<std::size_t>(draw.index_count);
	if (first_index > mesh.indices->size() || index_count > mesh.indices->size() - first_index) {
		return false;
	}

	const std::size_t base_vertex = static_cast<std::size_t>(draw.base_vertex);
	if (base_vertex >= mesh.vertices_snorm->size()) {
		return false;
	}

	const auto begin = mesh.indices->begin() + static_cast<std::ptrdiff_t>(first_index);
	const auto end = begin + static_cast<std::ptrdiff_t>(index_count);
	return std::ranges::all_of(begin, end, [&](const std::uint16_t index) {
		const std::size_t local_vertex = static_cast<std::size_t>(index);
		if (local_vertex >= mesh.vertices_snorm->size() - base_vertex) {
			return false;
		}

		const std::size_t vertex_index = base_vertex + local_vertex;
		const glm::u8vec4& bone_ids = (*mesh.skin_weights)[vertex_index * 2u];
		const glm::u8vec4& weights = (*mesh.skin_weights)[vertex_index * 2u + 1u];
		for (int channel = 0; channel < 4; ++channel) {
			if (weights[channel] != 0u && static_cast<std::uint32_t>(bone_ids[channel]) >= draw.bone_count) {
				return false;
			}
		}
		return true;
	});
}

inline bool model_cpu_frame_valid(const ModelCpuFrame& frame) {
	if (!frame.has_draws()) {
		return true;
	}
	if (frame.instance_matrices.empty() || frame.team_color_indexes.size() != frame.instance_matrices.size()
		|| frame.bone_matrices.empty() || frame.layer_colors.empty()) {
		return false;
	}
	if (std::ranges::any_of(frame.team_color_indexes, [](const std::uint32_t team_color_index) {
			return team_color_index > detail::max_model_team_color_index;
		})) {
		return false;
	}
	for (const ModelMeshCpuFrame& mesh : frame.meshes) {
		if (!mesh.valid()) {
			return false;
		}
	}
	const auto draws_valid = [&frame](const std::vector<ModelDrawCallCpuFrame>& draws) {
		for (const ModelDrawCallCpuFrame& draw : draws) {
			if (!model_draw_call_valid(frame, draw)) {
				return false;
			}
		}
		return true;
	};
	return draws_valid(frame.opaque_sd_draws) && draws_valid(frame.opaque_hd_draws) && draws_valid(frame.transparent_sd_draws)
		&& draws_valid(frame.transparent_hd_draws) && draws_valid(frame.picking_draws);
}

struct ParticleEmitter2DrawCpuFrame {
	std::uint32_t first_vertex = 0;
	std::uint32_t vertex_count = 0;
	std::uint32_t texture_index = 0;
	std::uint32_t filter_mode = 0;
	std::uint32_t priority_plane = 0;
	float sort_distance = 0.0f;
};

struct ParticleEmitter2CpuFrame {
	std::vector<glm::vec4> positions;
	std::vector<glm::vec2> uvs;
	std::vector<glm::vec4> colors;
	std::vector<ModelTextureCpuSource> textures;
	std::vector<ParticleEmitter2DrawCpuFrame> draws;

	[[nodiscard]]
	bool has_draws() const {
		return !draws.empty();
	}
};

inline bool particle_emitter2_frame_valid(const ParticleEmitter2CpuFrame& frame) {
	if (frame.positions.size() != frame.uvs.size() || frame.positions.size() != frame.colors.size()) {
		return false;
	}

	const std::size_t vertex_count = frame.positions.size();
	for (const ParticleEmitter2DrawCpuFrame& draw : frame.draws) {
		if (draw.vertex_count < 3u || draw.vertex_count % 3u != 0u) {
			return false;
		}
		if (static_cast<std::size_t>(draw.texture_index) >= frame.textures.size()) {
			return false;
		}
		if (draw.filter_mode > 4u) {
			return false;
		}

		const std::size_t first = static_cast<std::size_t>(draw.first_vertex);
		const std::size_t count = static_cast<std::size_t>(draw.vertex_count);
		if (first > vertex_count || count > vertex_count - first) {
			return false;
		}
	}

	return true;
}

struct SelectionOverlayDrawCpuFrame {
	std::uint32_t first_vertex = 0;
	std::uint32_t vertex_count = 0;
	std::uint32_t primitive_type = 0;
	std::uint32_t style = 0;
};

struct SelectionOverlayCpuFrame {
	std::vector<glm::vec4> positions;
	std::vector<glm::vec2> uvs;
	std::vector<glm::vec4> colors;
	std::vector<SelectionOverlayDrawCpuFrame> draws;

	[[nodiscard]]
	bool has_draws() const {
		return !draws.empty();
	}
};

inline bool selection_overlay_frame_valid(const SelectionOverlayCpuFrame& frame) {
	if (frame.positions.size() != frame.uvs.size() || frame.positions.size() != frame.colors.size()) {
		return false;
	}

	const std::size_t vertex_count = frame.positions.size();
	for (const SelectionOverlayDrawCpuFrame& draw : frame.draws) {
		if (draw.vertex_count == 0u || draw.primitive_type > 1u || draw.style > 1u) {
			return false;
		}

		const std::size_t first = static_cast<std::size_t>(draw.first_vertex);
		const std::size_t count = static_cast<std::size_t>(draw.vertex_count);
		if (first > vertex_count || count > vertex_count - first) {
			return false;
		}

		if (draw.primitive_type == 0u && draw.vertex_count < 2u) {
			return false;
		}
		if (draw.primitive_type == 1u && (draw.vertex_count < 3u || draw.vertex_count % 3u != 0u)) {
			return false;
		}
	}

	return true;
}

struct MetalMapFrame {
	CameraUniforms camera;
	TerrainCpuFrame terrain;
	PathingCpuFrame pathing;
	BrushMask brush;
	const TerrainTextureCpuFrame* textures = nullptr;
	CliffCpuFrame cliffs;
	ModelCpuFrame models;
	ParticleEmitter2CpuFrame particles;
	SelectionOverlayCpuFrame selection_overlay;

	bool show_pathing = false;
	bool show_lighting = true;
	bool show_brush = false;
	bool show_water = true;
	bool show_wireframe = false;

	glm::vec3 light_direction = glm::normalize(glm::vec3(-1.0f, -1.0f, -1.0f));
	float water_offset = 0.0f;
	std::uint32_t water_texture_index = 0;
	glm::vec4 shallow_color_min = glm::vec4(0.30f, 0.55f, 0.65f, 0.55f);
	glm::vec4 shallow_color_max = glm::vec4(0.25f, 0.45f, 0.60f, 0.65f);
	glm::vec4 deep_color_min = glm::vec4(0.05f, 0.18f, 0.35f, 0.78f);
	glm::vec4 deep_color_max = glm::vec4(0.02f, 0.08f, 0.22f, 0.88f);
};

[[nodiscard]]
inline bool terrain_ground_material_indices_valid(const TerrainCpuFrame& terrain, const TerrainTextureCpuFrame& textures) {
	if (!textures.ground_valid()) {
		return true;
	}

	const std::size_t texture_count = textures.ground_texture_layer_offsets.size();
	const std::size_t layer_count = static_cast<std::size_t>(textures.ground_layer_count);
	const auto encoded_layer_valid = [&](const std::uint32_t encoded) {
		const std::uint32_t texture_index = encoded & 0xFFFFu;
		if (texture_index == 0xFFFFu) {
			return true;
		}
		if (static_cast<std::size_t>(texture_index) >= texture_count) {
			return false;
		}

		const std::size_t offset = static_cast<std::size_t>(textures.ground_texture_layer_offsets[texture_index]);
		const std::size_t next_offset =
			texture_index + 1u < texture_count
				? static_cast<std::size_t>(textures.ground_texture_layer_offsets[static_cast<std::size_t>(texture_index) + 1u])
				: layer_count;
		const std::size_t variation = static_cast<std::size_t>(encoded >> 16u);
		return offset < next_offset && next_offset <= layer_count && variation < next_offset - offset;
	};

	return std::ranges::all_of(terrain.ground_texture_indices, [&](const glm::uvec4& tile) {
		return encoded_layer_valid(tile.x) && encoded_layer_valid(tile.y) && encoded_layer_valid(tile.z) && encoded_layer_valid(tile.w);
	});
}

[[nodiscard]]
inline bool metal_map_frame_texture_indices_valid(const MetalMapFrame& frame) {
	if (frame.textures == nullptr) {
		return true;
	}
	if (!terrain_ground_material_indices_valid(frame.terrain, *frame.textures)) {
		return false;
	}
	if (frame.textures->water_valid()
		&& frame.water_texture_index >= static_cast<std::uint32_t>(frame.textures->water_layer_count)) {
		return false;
	}
	return true;
}

struct MetalModelFrame {
	CameraUniforms camera;
	ModelCpuFrame models;
	ParticleEmitter2CpuFrame particles;
	SelectionOverlayCpuFrame selection_overlay;
	glm::vec3 light_direction = glm::normalize(glm::vec3(1.0f, 1.0f, -3.0f));
	bool show_lighting = true;
};

inline void accumulate_model_frame_stats(MetalModelFrameStats& stats, const MetalModelFrame& frame) {
	stats.mesh_count += frame.models.meshes.size();
	for (const ModelMeshCpuFrame& mesh : frame.models.meshes) {
		stats.model_texture_count += mesh.textures.size();
	}

	const auto add_draws = [&](const std::vector<ModelDrawCallCpuFrame>& draws) {
		for (const ModelDrawCallCpuFrame& draw : draws) {
			if (draw.alpha_test >= 0.75f) {
				++stats.alpha_tested_draw_count;
			}
			if (draw.is_team_color != 0u) {
				++stats.team_color_draw_count;
			}
		}
	};

	stats.opaque_sd_draw_count += frame.models.opaque_sd_draws.size();
	stats.opaque_hd_draw_count += frame.models.opaque_hd_draws.size();
	stats.transparent_sd_draw_count += frame.models.transparent_sd_draws.size();
	stats.transparent_hd_draw_count += frame.models.transparent_hd_draws.size();
	add_draws(frame.models.opaque_sd_draws);
	add_draws(frame.models.opaque_hd_draws);
	add_draws(frame.models.transparent_sd_draws);
	add_draws(frame.models.transparent_hd_draws);

	stats.particle_draw_count += frame.particles.draws.size();
	stats.particle_vertex_count += frame.particles.positions.size();
	stats.particle_texture_count += frame.particles.textures.size();
}

inline MetalModelFrameStats summarize_model_frame(const MetalModelFrame& frame) {
	MetalModelFrameStats stats;
	accumulate_model_frame_stats(stats, frame);
	return stats;
}

namespace detail {

inline constexpr std::uint32_t model_blend_zero = 0u;
inline constexpr std::uint32_t model_blend_one = 1u;
inline constexpr std::uint32_t model_blend_one_minus_destination_color = 9u;

[[nodiscard]]
inline bool model_blend_factor_valid(const std::uint32_t value) {
	return value <= model_blend_one_minus_destination_color;
}

[[nodiscard]]
inline std::uint32_t normalized_model_blend_factor(const std::uint32_t value, const std::uint32_t fallback) {
	return model_blend_factor_valid(value) ? value : fallback;
}

[[nodiscard]]
inline std::uint32_t normalized_model_team_color_index(const std::uint32_t value) {
	return std::min(value, max_model_team_color_index);
}

inline bool draw_order_less(const ModelDrawCallCpuFrame& left, const ModelDrawCallCpuFrame& right) {
	if (left.source_blend_factor != right.source_blend_factor) {
		return left.source_blend_factor < right.source_blend_factor;
	}
	if (left.destination_blend_factor != right.destination_blend_factor) {
		return left.destination_blend_factor < right.destination_blend_factor;
	}
	if (left.cull_face != right.cull_face) {
		return left.cull_face < right.cull_face;
	}
	if (left.depth_test != right.depth_test) {
		return left.depth_test < right.depth_test;
	}
	if (left.depth_mask != right.depth_mask) {
		return left.depth_mask < right.depth_mask;
	}
	return left.mesh_index < right.mesh_index;
}

inline bool transparent_draw_order_less(const ModelDrawCallCpuFrame& left, const ModelDrawCallCpuFrame& right) {
	return left.sort_distance > right.sort_distance;
}

enum class ModelTransparentDrawFamily {
	SD,
	HD,
};

struct ModelTransparentDrawRef {
	ModelTransparentDrawFamily family = ModelTransparentDrawFamily::SD;
	std::size_t index = 0;
};

inline const ModelDrawCallCpuFrame& transparent_draw_ref(const ModelCpuFrame& frame, const ModelTransparentDrawRef ref) {
	return ref.family == ModelTransparentDrawFamily::SD ? frame.transparent_sd_draws[ref.index] : frame.transparent_hd_draws[ref.index];
}

inline std::vector<ModelTransparentDrawRef> transparent_model_draw_order(const ModelCpuFrame& frame) {
	std::vector<ModelTransparentDrawRef> order;
	order.reserve(frame.transparent_sd_draws.size() + frame.transparent_hd_draws.size());
	for (std::size_t index = 0; index < frame.transparent_sd_draws.size(); ++index) {
		order.push_back(ModelTransparentDrawRef {
			.family = ModelTransparentDrawFamily::SD,
			.index = index,
		});
	}
	for (std::size_t index = 0; index < frame.transparent_hd_draws.size(); ++index) {
		order.push_back(ModelTransparentDrawRef {
			.family = ModelTransparentDrawFamily::HD,
			.index = index,
		});
	}
	std::stable_sort(order.begin(), order.end(), [&frame](const ModelTransparentDrawRef left, const ModelTransparentDrawRef right) {
		return transparent_draw_order_less(transparent_draw_ref(frame, left), transparent_draw_ref(frame, right));
	});
	return order;
}

inline bool particle_draw_order_less(const ParticleEmitter2DrawCpuFrame& left, const ParticleEmitter2DrawCpuFrame& right) {
	if (left.priority_plane != right.priority_plane) {
		return left.priority_plane < right.priority_plane;
	}
	if (left.sort_distance != right.sort_distance) {
		return left.sort_distance > right.sort_distance;
	}
	return left.first_vertex < right.first_vertex;
}

enum class ModelDrawPass {
	Opaque,
	Transparent,
};

struct ModelLayerMaterialCpuFrame {
	float emissive_gain = 1.0f;
	glm::vec3 fresnel_color = glm::vec3(0.0f);
	float fresnel_opacity = 0.0f;
	float fresnel_team_color = 0.0f;
	float environment_factor = 0.0f;
};

constexpr std::uint32_t particle_flag_sort_primitives_far_z = 0x10000u;
constexpr std::uint32_t particle_flag_line_emitter = 0x20000u;
constexpr std::uint32_t particle_flag_model_space = 0x80000u;
constexpr std::uint32_t particle_flag_xy_quad = 0x100000u;
constexpr std::uint32_t particle_emitter1_uses_mdl = 0x8000u;
constexpr std::uint32_t particle_emitter1_uses_tga = 0x10000u;
constexpr float particle_epsilon = 1e-6f;
constexpr float particle_corner_xy[4][2] = {
	{-1.0f, 1.0f},
	{-1.0f, -1.0f},
	{1.0f, 1.0f},
	{1.0f, -1.0f},
};

inline float particle_key_frame_t(const float age, const float prev_end_time, const float end_time) {
	const float span = end_time - prev_end_time;
	const float raw = span > 0.0f ? (age - prev_end_time) / span : 0.0f;
	return std::clamp(raw, 0.0f, 1.0f) * 0.99f + 0.005f;
}

inline int particle_compute_cell(int start, int end, int repeat, float t) {
	const float r = repeat < 1 ? 1.0f : static_cast<float>(repeat);
	const int initial = end >= start ? start : start + 1;
	const int delta = end >= start ? end - start + 1 : end - start - 1;
	const float effective_t = r == 1.0f ? t : std::fmod(t * r, 1.0f);
	const float value = static_cast<float>(initial) + static_cast<float>(delta) * effective_t;
	return std::max(0, static_cast<int>(value));
}

template <typename MeshT>
ModelMeshCpuFrame make_model_mesh_cpu_frame(const MeshT& mesh) {
	ModelMeshCpuFrame frame;
	frame.key = &mesh;
	frame.revision = mesh.cpu_revision;
	frame.vertices_snorm = &mesh.cpu_vertices_snorm;
	frame.uvs_snorm = &mesh.cpu_uvs_snorm;
	frame.normals_oct_snorm = &mesh.cpu_normals_oct_snorm;
	frame.tangents = &mesh.cpu_tangents;
	frame.skin_weights = &mesh.cpu_skin_weights;
	frame.indices = &mesh.cpu_indices;
	frame.textures.reserve(mesh.textures.size());

	for (const auto& texture : mesh.textures) {
		if (!texture) {
			frame.textures.push_back({});
			continue;
		}
		frame.textures.push_back(ModelTextureCpuSource {
			.key = texture.get(),
			.width = texture->width,
			.height = texture->height,
			.channels = texture->channels,
			.wrap_flags = texture->wrap_flags,
			.srgb = texture->srgb,
			.rgba8 = &texture->data,
		});
	}

	return frame;
}

template <typename MeshT>
std::uint32_t ensure_model_mesh(
	ModelCpuFrame& frame,
	std::unordered_map<const void*, std::uint32_t>& mesh_indices,
	const MeshT& mesh
) {
	const void* key = &mesh;
	const auto found = mesh_indices.find(key);
	if (found != mesh_indices.end()) {
		return found->second;
	}

	const std::uint32_t mesh_index = static_cast<std::uint32_t>(frame.meshes.size());
	frame.meshes.push_back(make_model_mesh_cpu_frame(mesh));
	mesh_indices.emplace(key, mesh_index);
	return mesh_index;
}

template <typename SkeletonT, typename TrackT, typename TrackValueT>
TrackValueT interpolate_skeleton_track(
	const SkeletonT& skeleton,
	const TrackT& track,
	const TrackValueT& fallback
) {
	if constexpr (requires { skeleton.interpolate_keyframes(track, fallback); }) {
		return skeleton.interpolate_keyframes(track, fallback);
	} else if constexpr (requires { skeleton.sample_track(track, fallback); }) {
		return skeleton.sample_track(track, fallback);
	} else {
		return fallback;
	}
}

template <typename MeshT, typename SkeletonT, typename LayerTextureT>
std::uint32_t resolve_layer_texture_id(
	const MeshT& mesh,
	const SkeletonT& skeleton,
	const LayerTextureT& texture,
	const bool has_valid_sequence
) {
	const std::uint32_t texture_id = has_valid_sequence ? interpolate_skeleton_track(skeleton, texture.KMTF, texture.id) : texture.id;
	return texture_id < mesh.textures.size() ? texture_id : texture.id;
}

template <typename MeshT, typename GeosetT>
auto model_layers_for_geoset(const MeshT& mesh, const GeosetT& geoset)
	-> const std::remove_reference_t<decltype(mesh.mdx->materials[0].layers)>*;

template <typename MeshT, typename SkeletonT>
auto resolve_model_layer_texture_ids(const MeshT& mesh, const SkeletonT& skeleton, const bool has_valid_sequence) {
	auto layer_texture_ids = mesh.cpu_layer_texture_ids;
	std::uint32_t layer_index_local = 0;
	for (const auto& geoset : mesh.geosets) {
		const auto* layers = model_layers_for_geoset(mesh, geoset);
		if (layers == nullptr) {
			continue;
		}

		for (const auto& layer : *layers) {
			if (layer_index_local < layer_texture_ids.size()) {
				auto& ids = layer_texture_ids[layer_index_local];
				std::uint32_t* texture_slots = &ids.albedo;
				const std::uint32_t fallback_texture_id =
					layer.textures.empty() ? 0u : resolve_layer_texture_id(mesh, skeleton, layer.textures.front(), has_valid_sequence);
				std::fill(texture_slots, texture_slots + 6, fallback_texture_id);
				for (std::size_t slot = 0; slot < layer.textures.size() && slot < 6; ++slot) {
					texture_slots[slot] = resolve_layer_texture_id(mesh, skeleton, layer.textures[slot], has_valid_sequence);
				}
			}
			++layer_index_local;
		}
	}
	return layer_texture_ids;
}

template <typename MeshT, typename SkeletonT>
std::vector<ModelLayerMaterialCpuFrame> resolve_model_layer_materials(
	const MeshT& mesh,
	const SkeletonT& skeleton,
	const bool has_valid_sequence
) {
	std::vector<ModelLayerMaterialCpuFrame> layer_materials;
	layer_materials.reserve(mesh.cpu_layer_params.size());
	for (const auto& geoset : mesh.geosets) {
		const auto* layers = model_layers_for_geoset(mesh, geoset);
		if (layers == nullptr) {
			continue;
		}

		for (const auto& layer : *layers) {
			ModelLayerMaterialCpuFrame material;
			material.emissive_gain =
				has_valid_sequence ? interpolate_skeleton_track(skeleton, layer.KMTE, layer.emissive_gain) : layer.emissive_gain;
			material.fresnel_color =
				has_valid_sequence ? interpolate_skeleton_track(skeleton, layer.KFC3, layer.fresnel_color) : layer.fresnel_color;
			material.fresnel_opacity =
				has_valid_sequence ? interpolate_skeleton_track(skeleton, layer.KFCA, layer.fresnel_opacity) : layer.fresnel_opacity;
			material.fresnel_team_color =
				has_valid_sequence ? interpolate_skeleton_track(skeleton, layer.KFTC, layer.fresnel_team_color) : layer.fresnel_team_color;
			material.environment_factor = (layer.shading_flags & 0x2u) ? 1.0f : 0.0f;
			layer_materials.push_back(material);
		}
	}
	return layer_materials;
}

template <typename MeshT, typename LayerTextureIdsRangeT, typename EntryRangeT>
void append_model_draws(
	std::vector<ModelDrawCallCpuFrame>& out,
	const MeshT& mesh,
	const LayerTextureIdsRangeT& layer_texture_ids,
	const std::vector<ModelLayerMaterialCpuFrame>& layer_materials,
	const EntryRangeT& entries,
	const ModelDrawPass pass,
	const std::uint32_t mesh_index,
	const std::uint32_t instance_offset,
	const std::uint32_t bone_offset,
	const std::uint32_t bone_count,
	const std::uint32_t layer_color_offset,
	const std::uint32_t layer_skip_count,
	const std::uint32_t pick_id,
	const float sort_distance = 0.0f,
	const std::vector<glm::vec4>* layer_colors = nullptr
) {
	for (const auto& entry : entries) {
		const std::uint32_t source_blend_factor =
			normalized_model_blend_factor(static_cast<std::uint32_t>(entry.state.src_factor), model_blend_one);
		const std::uint32_t destination_blend_factor =
			normalized_model_blend_factor(static_cast<std::uint32_t>(entry.state.dst_factor), model_blend_zero);
		if (entry.layer_index_local >= layer_texture_ids.size() || entry.layer_index_local >= mesh.cpu_layer_params.size()) {
			continue;
		}
		if (pass == ModelDrawPass::Transparent && layer_colors != nullptr) {
			const std::size_t color_index = static_cast<std::size_t>(layer_color_offset + entry.layer_index_local);
			if (color_index < layer_colors->size() && (*layer_colors)[color_index].a <= 0.01f) {
				continue;
			}
		}

		const auto& ids = layer_texture_ids[entry.layer_index_local];
		const auto& params = mesh.cpu_layer_params[entry.layer_index_local];
		ModelDrawCallCpuFrame draw;
		draw.mesh_index = mesh_index;
		draw.index_count = entry.count;
		draw.first_index = entry.first_index;
		draw.base_vertex = entry.base_vertex;
		draw.instance_offset = instance_offset;
		draw.bone_offset = bone_offset;
		draw.bone_count = bone_count;
		draw.layer_color_offset = layer_color_offset;
		draw.layer_skip_count = layer_skip_count;
		draw.layer_index_local = entry.layer_index_local;
		draw.texture_indices = {ids.albedo, ids.normal, ids.orm, ids.emissive, ids.team_color, ids.environment};
		draw.alpha_test = params.alpha_test;
		draw.sort_distance = sort_distance;
		if (entry.layer_index_local < layer_materials.size()) {
			const ModelLayerMaterialCpuFrame& material = layer_materials[entry.layer_index_local];
			draw.hd_material_params0 =
				glm::vec4(material.emissive_gain, material.fresnel_opacity, material.fresnel_team_color, material.environment_factor);
			draw.hd_material_params1 = glm::vec4(material.fresnel_color, 0.0f);
		}
		draw.layer_lit = params.layer_lit;
		draw.is_team_color = params.is_team_color;
		draw.pick_id = pick_id;
		draw.source_blend_factor = source_blend_factor;
		draw.destination_blend_factor = destination_blend_factor;
		draw.cull_face = entry.state.cull_face ? 1u : 0u;
		draw.depth_test = entry.state.depth_test ? 1u : 0u;
		draw.depth_mask = pass == ModelDrawPass::Transparent ? 0u : (entry.state.depth_mask ? 1u : 0u);
		draw.picking_depth_mask = entry.state.depth_mask ? 1u : 0u;
		out.push_back(draw);
	}
}

template <typename MeshT>
const typename MeshT::DrawEntry* find_model_draw_entry(const MeshT& mesh, const std::uint32_t layer_index_local) {
	const auto find_in = [layer_index_local](const auto& entries) -> const typename MeshT::DrawEntry* {
		for (const auto& entry : entries) {
			if (entry.layer_index_local == layer_index_local) {
				return &entry;
			}
		}
		return nullptr;
	};
	if (const auto* entry = find_in(mesh.opaque_entries_sd)) {
		return entry;
	}
	if (const auto* entry = find_in(mesh.opaque_entries_hd)) {
		return entry;
	}
	if (const auto* entry = find_in(mesh.transparent_entries_sd)) {
		return entry;
	}
	return find_in(mesh.transparent_entries_hd);
}

template <typename MeshT, typename GeosetT>
auto model_layers_for_geoset(const MeshT& mesh, const GeosetT& geoset)
	-> const std::remove_reference_t<decltype(mesh.mdx->materials[0].layers)>* {
	if (!mesh.mdx) {
		return nullptr;
	}

	const auto material_id = geoset.material_id;
	using MaterialId = std::remove_cvref_t<decltype(material_id)>;
	if constexpr (std::is_signed_v<MaterialId>) {
		if (material_id < 0) {
			return nullptr;
		}
	}

	const std::size_t material_index = static_cast<std::size_t>(material_id);
	if (material_index >= mesh.mdx->materials.size()) {
		return nullptr;
	}

	const auto& layers = mesh.mdx->materials[material_index].layers;
	return layers.empty() ? nullptr : &layers;
}

template <typename MeshT, typename LayerTextureIdsRangeT>
void append_model_picking_draws(
	std::vector<ModelDrawCallCpuFrame>& out,
	const MeshT& mesh,
	const LayerTextureIdsRangeT& layer_texture_ids,
	const std::vector<ModelLayerMaterialCpuFrame>& layer_materials,
	const std::uint32_t mesh_index,
	const std::uint32_t instance_offset,
	const std::uint32_t bone_offset,
	const std::uint32_t bone_count,
	const std::uint32_t layer_color_offset,
	const std::uint32_t layer_skip_count,
	const std::uint32_t pick_id,
	const std::vector<glm::vec4>& layer_colors
) {
	if (pick_id == 0u) {
		return;
	}

	std::uint32_t layer_index_local = 0;
	for (const auto& geoset : mesh.geosets) {
		const auto* layers = model_layers_for_geoset(mesh, geoset);
		if (layers == nullptr) {
			continue;
		}
		for (std::uint32_t layer_offset = 0; layer_offset < layers->size(); ++layer_offset) {
			const std::uint32_t current_layer = layer_index_local + layer_offset;
			const std::size_t color_index = static_cast<std::size_t>(layer_color_offset + current_layer);
			if (color_index >= layer_colors.size() || current_layer >= mesh.cpu_layer_params.size()) {
				continue;
			}

			const float layer_alpha = layer_colors[color_index].a;
			const float alpha_test = mesh.cpu_layer_params[current_layer].alpha_test;
			if (layer_alpha <= 0.0f || layer_alpha < alpha_test) {
				continue;
			}

			const auto* entry = find_model_draw_entry(mesh, current_layer);
			if (entry != nullptr) {
				const std::size_t previous_draw_count = out.size();
				append_model_draws(
					out,
					mesh,
					layer_texture_ids,
					layer_materials,
					std::array<typename MeshT::DrawEntry, 1> {*entry},
					ModelDrawPass::Opaque,
					mesh_index,
					instance_offset,
					bone_offset,
					bone_count,
					layer_color_offset,
					layer_skip_count,
					pick_id
				);
				if (out.size() != previous_draw_count) {
					// OpenGL color picking draws only the first visible renderable layer for each geoset.
					break;
				}
			}
		}
		layer_index_local += static_cast<std::uint32_t>(layers->size());
	}
}

template <typename MeshT, typename LayerTextureIdsRangeT>
void append_model_declaration_order_draws(
	ModelCpuFrame& frame,
	const MeshT& mesh,
	const LayerTextureIdsRangeT& layer_texture_ids,
	const std::vector<ModelLayerMaterialCpuFrame>& layer_materials,
	const std::uint32_t mesh_index,
	const std::uint32_t instance_offset,
	const std::uint32_t bone_offset,
	const std::uint32_t bone_count,
	const std::uint32_t layer_color_offset,
	const std::uint32_t layer_skip_count,
	const std::uint32_t pick_id,
	const float sort_distance
) {
	std::uint32_t layer_index_local = 0;
	for (const auto& geoset : mesh.geosets) {
		const auto* layers = model_layers_for_geoset(mesh, geoset);
		if (layers == nullptr) {
			continue;
		}
		for (std::uint32_t layer_offset = 0; layer_offset < layers->size(); ++layer_offset) {
			const auto& layer = (*layers)[layer_offset];
			const std::uint32_t current_layer = layer_index_local + layer_offset;
			const auto* entry = find_model_draw_entry(mesh, current_layer);
			if (entry == nullptr) {
				continue;
			}

			const bool layer_is_opaque = layer.blend_mode == 0 || layer.blend_mode == 1;
			using ShaderType = std::remove_cvref_t<decltype(layer.shader)>;
			const bool layer_is_hd = layer.shader == ShaderType::HD;
			auto& target = layer_is_opaque ? (layer_is_hd ? frame.opaque_hd_draws : frame.opaque_sd_draws)
											: (layer_is_hd ? frame.transparent_hd_draws : frame.transparent_sd_draws);
			append_model_draws(
				target,
				mesh,
				layer_texture_ids,
				layer_materials,
				std::array<typename MeshT::DrawEntry, 1> {*entry},
				layer_is_opaque ? ModelDrawPass::Opaque : ModelDrawPass::Transparent,
				mesh_index,
				instance_offset,
				bone_offset,
				bone_count,
				layer_color_offset,
				layer_skip_count,
				pick_id,
				sort_distance,
				layer_is_opaque ? nullptr : &frame.layer_colors
			);
		}
		layer_index_local += static_cast<std::uint32_t>(layers->size());
	}
}

template <typename MeshT>
std::uint32_t model_skin_bone_matrix_count(const MeshT& mesh) {
	std::uint32_t bone_count = 0u;
	if (mesh.mdx) {
		for (const auto& bone : mesh.mdx->bones) {
			if (bone.node.id >= 0) {
				bone_count = std::max(bone_count, static_cast<std::uint32_t>(bone.node.id) + 1u);
			}
		}
	}

	for (std::size_t skin_index = 0; skin_index + 1 < mesh.cpu_skin_weights.size(); skin_index += 2) {
		const glm::u8vec4& bone_ids = mesh.cpu_skin_weights[skin_index];
		const glm::u8vec4& weights = mesh.cpu_skin_weights[skin_index + 1];
		for (int channel = 0; channel < 4; ++channel) {
			if (weights[channel] != 0u) {
				bone_count = std::max(bone_count, static_cast<std::uint32_t>(bone_ids[channel]) + 1u);
			}
		}
	}

	return std::max(bone_count, 1u);
}

struct BoundsPickingMeshStorage {
	std::vector<glm::uvec2> vertices_snorm;
	std::vector<std::uint32_t> uvs_snorm;
	std::vector<std::uint32_t> normals_oct_snorm;
	std::vector<glm::vec4> tangents;
	std::vector<glm::u8vec4> skin_weights;
	std::vector<std::uint16_t> indices;
};

inline glm::uvec2 pack_bounds_picking_vertex(const glm::vec3& vertex) {
	constexpr float pack_extent = 8192.0f;
	glm::vec3 normalized = glm::clamp(vertex / pack_extent, glm::vec3(-1.0f), glm::vec3(1.0f));
	normalized = (normalized + 1.0f) * 0.5f;
	const auto x = static_cast<std::uint64_t>(normalized.x * static_cast<float>((1ull << 21u) - 1u));
	const auto y = static_cast<std::uint64_t>(normalized.y * static_cast<float>((1ull << 21u) - 1u));
	const auto z = static_cast<std::uint64_t>(normalized.z * static_cast<float>((1ull << 22u) - 1u));
	const std::uint64_t packed = (x << 43u) | (y << 22u) | z;
	return glm::uvec2(static_cast<std::uint32_t>(packed & 0xFFFFFFFFull), static_cast<std::uint32_t>(packed >> 32u));
}

inline const BoundsPickingMeshStorage& bounds_picking_mesh_storage() {
	static const BoundsPickingMeshStorage storage = [] {
		BoundsPickingMeshStorage mesh;
		constexpr std::array<glm::vec3, 8> vertices = {
			glm::vec3(-1.0f, -1.0f, -1.0f),
			glm::vec3(1.0f, -1.0f, -1.0f),
			glm::vec3(1.0f, 1.0f, -1.0f),
			glm::vec3(-1.0f, 1.0f, -1.0f),
			glm::vec3(-1.0f, -1.0f, 1.0f),
			glm::vec3(1.0f, -1.0f, 1.0f),
			glm::vec3(1.0f, 1.0f, 1.0f),
			glm::vec3(-1.0f, 1.0f, 1.0f),
		};
		mesh.vertices_snorm.reserve(vertices.size());
		for (const glm::vec3& vertex : vertices) {
			mesh.vertices_snorm.push_back(pack_bounds_picking_vertex(vertex));
		}
		mesh.uvs_snorm.assign(vertices.size(), 0u);
		mesh.normals_oct_snorm.assign(vertices.size(), 0u);
		mesh.tangents.assign(vertices.size(), glm::vec4(1.0f, 0.0f, 0.0f, 1.0f));
		mesh.skin_weights.reserve(vertices.size() * 2u);
		for (std::size_t i = 0; i < vertices.size(); ++i) {
			mesh.skin_weights.emplace_back(0u, 0u, 0u, 0u);
			mesh.skin_weights.emplace_back(255u, 0u, 0u, 0u);
		}
		mesh.indices = {
			0u, 1u, 2u, 0u, 2u, 3u,
			4u, 6u, 5u, 4u, 7u, 6u,
			0u, 4u, 5u, 0u, 5u, 1u,
			1u, 5u, 6u, 1u, 6u, 2u,
			2u, 6u, 7u, 2u, 7u, 3u,
			3u, 7u, 4u, 3u, 4u, 0u,
		};
		return mesh;
	}();
	return storage;
}

inline std::uint32_t ensure_bounds_picking_mesh(
	ModelCpuFrame& frame,
	std::unordered_map<const void*, std::uint32_t>& mesh_indices
) {
	const BoundsPickingMeshStorage& storage = bounds_picking_mesh_storage();
	const void* key = &storage;
	if (const auto found = mesh_indices.find(key); found != mesh_indices.end()) {
		return found->second;
	}

	const std::uint32_t mesh_index = static_cast<std::uint32_t>(frame.meshes.size());
	frame.meshes.push_back(ModelMeshCpuFrame {
		.key = key,
		.revision = 1u,
		.vertices_snorm = &storage.vertices_snorm,
		.uvs_snorm = &storage.uvs_snorm,
		.normals_oct_snorm = &storage.normals_oct_snorm,
		.tangents = &storage.tangents,
		.skin_weights = &storage.skin_weights,
		.indices = &storage.indices,
		.textures = {},
	});
	mesh_indices.emplace(key, mesh_index);
	return mesh_index;
}

inline bool valid_bounds_extent(const glm::vec3& minimum, const glm::vec3& maximum) {
	return std::isfinite(minimum.x) && std::isfinite(minimum.y) && std::isfinite(minimum.z) && std::isfinite(maximum.x)
		&& std::isfinite(maximum.y) && std::isfinite(maximum.z) && minimum.x <= maximum.x && minimum.y <= maximum.y
		&& minimum.z <= maximum.z;
}

template <typename CameraT, typename MeshPtrT, typename SkeletonT>
void append_model_bounds_picking_instance(
	ModelCpuFrame& frame,
	std::unordered_map<const void*, std::uint32_t>& mesh_indices,
	const CameraT& camera,
	const MeshPtrT& mesh_ptr,
	const SkeletonT& skeleton,
	const std::uint32_t team_color_index,
	const std::uint32_t pick_id
) {
	if (pick_id == 0u || !mesh_ptr || mesh_ptr->has_mesh || !mesh_ptr->mdx) {
		return;
	}

	const bool has_valid_sequence = skeleton.sequence_index >= 0
		&& static_cast<std::size_t>(skeleton.sequence_index) < mesh_ptr->mdx->sequences.size();
	const auto& extent = has_valid_sequence ? mesh_ptr->mdx->sequences[skeleton.sequence_index].extent : mesh_ptr->mdx->extent;
	if (!valid_bounds_extent(extent.minimum, extent.maximum)
		|| !camera.inside_frustrum_transform(extent.minimum, extent.maximum, skeleton.matrix)) {
		return;
	}

	const glm::vec3 center = (extent.minimum + extent.maximum) * 0.5f;
	const glm::vec3 half_extent = glm::max((extent.maximum - extent.minimum) * 0.5f, glm::vec3(1.0f));
	const glm::mat4 bounds_matrix =
		skeleton.matrix * glm::translate(glm::mat4(1.0f), center) * glm::scale(glm::mat4(1.0f), half_extent);

	const std::uint32_t mesh_index = ensure_bounds_picking_mesh(frame, mesh_indices);
	const std::uint32_t instance_offset = static_cast<std::uint32_t>(frame.instance_matrices.size());
	const std::uint32_t bone_offset = static_cast<std::uint32_t>(frame.bone_matrices.size());
	const std::uint32_t layer_color_offset = static_cast<std::uint32_t>(frame.layer_colors.size());

	frame.instance_matrices.push_back(bounds_matrix);
	frame.team_color_indexes.push_back(normalized_model_team_color_index(team_color_index));
	frame.bone_matrices.push_back(glm::mat4(1.0f));
	frame.layer_colors.emplace_back(1.0f);

	ModelDrawCallCpuFrame draw;
	draw.mesh_index = mesh_index;
	draw.index_count = static_cast<std::uint32_t>(bounds_picking_mesh_storage().indices.size());
	draw.instance_offset = instance_offset;
	draw.bone_offset = bone_offset;
	draw.bone_count = 1u;
	draw.layer_color_offset = layer_color_offset;
	draw.layer_skip_count = 1u;
	draw.pick_id = pick_id;
	draw.cull_face = 0u;
	frame.picking_draws.push_back(draw);
}

template <typename CameraT, typename MeshPtrT, typename SkeletonT>
void append_model_instance(
	ModelCpuFrame& frame,
	std::unordered_map<const void*, std::uint32_t>& mesh_indices,
	const CameraT& camera,
	const MeshPtrT& mesh_ptr,
	const SkeletonT& skeleton,
	const glm::vec3& color,
	const std::uint32_t team_color_index,
	const std::uint32_t pick_id = 0,
	const bool preserve_declaration_order = false
) {
	if (!mesh_ptr || !mesh_ptr->has_mesh) {
		return;
	}

	const bool has_valid_sequence = skeleton.sequence_index >= 0
		&& static_cast<std::size_t>(skeleton.sequence_index) < mesh_ptr->mdx->sequences.size();
	const auto& extent = has_valid_sequence ? mesh_ptr->mdx->sequences[skeleton.sequence_index].extent : mesh_ptr->mdx->extent;
	if (!camera.inside_frustrum_transform(extent.minimum, extent.maximum, skeleton.matrix)) {
		return;
	}

	const std::uint32_t mesh_index = ensure_model_mesh(frame, mesh_indices, *mesh_ptr);
	const std::uint32_t instance_offset = static_cast<std::uint32_t>(frame.instance_matrices.size());
	const std::uint32_t bone_offset = static_cast<std::uint32_t>(frame.bone_matrices.size());
	const std::uint32_t layer_color_offset = static_cast<std::uint32_t>(frame.layer_colors.size());
	const std::uint32_t bone_count = model_skin_bone_matrix_count(*mesh_ptr);
	const std::uint32_t layer_skip_count = static_cast<std::uint32_t>(mesh_ptr->skip_count);
	const float sort_distance = glm::distance(camera.position - camera.direction * camera.distance, glm::vec3(skeleton.matrix[3]));

	frame.instance_matrices.push_back(skeleton.matrix);
	frame.team_color_indexes.push_back(normalized_model_team_color_index(team_color_index));
	if (has_valid_sequence) {
		const std::size_t available_bones = std::min<std::size_t>(skeleton.world_matrices.size(), bone_count);
		frame.bone_matrices.insert(frame.bone_matrices.end(), skeleton.world_matrices.begin(), skeleton.world_matrices.begin() + available_bones);
		frame.bone_matrices.insert(frame.bone_matrices.end(), static_cast<std::size_t>(bone_count) - available_bones, glm::mat4(1.0f));
	} else {
		frame.bone_matrices.insert(frame.bone_matrices.end(), bone_count, glm::mat4(1.0f));
	}

	for (const auto& geoset : mesh_ptr->geosets) {
		const auto* layers = model_layers_for_geoset(*mesh_ptr, geoset);
		if (layers == nullptr) {
			continue;
		}
		glm::vec3 layer_color = color;
		float geoset_visibility = 1.0f;
		if (geoset.geoset_anim && has_valid_sequence) {
			layer_color *= skeleton.get_geoset_animation_color(*geoset.geoset_anim);
			geoset_visibility = skeleton.get_geoset_animation_visiblity(*geoset.geoset_anim);
		}
		for (const auto& layer : *layers) {
			const float layer_visibility = has_valid_sequence ? skeleton.get_layer_visiblity(layer) : 1.0f;
			frame.layer_colors.emplace_back(layer_color, layer_visibility * geoset_visibility);
		}
	}

	const auto layer_texture_ids = resolve_model_layer_texture_ids(*mesh_ptr, skeleton, has_valid_sequence);
	const auto layer_materials = resolve_model_layer_materials(*mesh_ptr, skeleton, has_valid_sequence);

	if (preserve_declaration_order) {
		append_model_declaration_order_draws(
			frame,
			*mesh_ptr,
			layer_texture_ids,
			layer_materials,
			mesh_index,
			instance_offset,
			bone_offset,
			bone_count,
			layer_color_offset,
			layer_skip_count,
			pick_id,
			sort_distance
		);
	} else {
		append_model_draws(
			frame.opaque_sd_draws,
			*mesh_ptr,
			layer_texture_ids,
			layer_materials,
			mesh_ptr->opaque_entries_sd,
			ModelDrawPass::Opaque,
			mesh_index,
			instance_offset,
			bone_offset,
			bone_count,
			layer_color_offset,
			layer_skip_count,
			pick_id
		);
		append_model_draws(
			frame.opaque_hd_draws,
			*mesh_ptr,
			layer_texture_ids,
			layer_materials,
			mesh_ptr->opaque_entries_hd,
			ModelDrawPass::Opaque,
			mesh_index,
			instance_offset,
			bone_offset,
			bone_count,
			layer_color_offset,
			layer_skip_count,
			pick_id
		);
		append_model_draws(
			frame.transparent_sd_draws,
			*mesh_ptr,
			layer_texture_ids,
			layer_materials,
			mesh_ptr->transparent_entries_sd,
			ModelDrawPass::Transparent,
			mesh_index,
			instance_offset,
			bone_offset,
			bone_count,
			layer_color_offset,
			layer_skip_count,
			pick_id,
			sort_distance,
			&frame.layer_colors
		);
		append_model_draws(
			frame.transparent_hd_draws,
			*mesh_ptr,
			layer_texture_ids,
			layer_materials,
			mesh_ptr->transparent_entries_hd,
			ModelDrawPass::Transparent,
			mesh_index,
			instance_offset,
			bone_offset,
			bone_count,
			layer_color_offset,
			layer_skip_count,
			pick_id,
			sort_distance,
			&frame.layer_colors
		);
	}
	append_model_picking_draws(
		frame.picking_draws,
		*mesh_ptr,
		layer_texture_ids,
		layer_materials,
		mesh_index,
		instance_offset,
		bone_offset,
		bone_count,
		layer_color_offset,
		layer_skip_count,
		pick_id,
		frame.layer_colors
	);
}

template <typename TextureRangeT>
bool ensure_particle_texture(
	ParticleEmitter2CpuFrame& frame,
	std::unordered_map<const void*, std::uint32_t>& texture_indices,
	const TextureRangeT& textures,
	const std::uint32_t texture_id,
	std::uint32_t& out_texture_index
) {
	if (texture_id >= textures.size() || !textures[texture_id]) {
		return false;
	}

	const auto& texture = textures[texture_id];
	const void* key = texture.get();
	if (const auto found = texture_indices.find(key); found != texture_indices.end()) {
		out_texture_index = found->second;
		return true;
	}

	out_texture_index = static_cast<std::uint32_t>(frame.textures.size());
	texture_indices.emplace(key, out_texture_index);
	frame.textures.push_back(ModelTextureCpuSource {
		.key = key,
		.width = texture->width,
		.height = texture->height,
		.channels = texture->channels,
		.wrap_flags = texture->wrap_flags,
		.srgb = texture->srgb,
		.rgba8 = &texture->data,
	});
	return true;
}

inline const std::vector<std::uint8_t>& corn_fallback_particle_texture_rgba8() {
	static const std::vector<std::uint8_t> rgba8 = {255u, 255u, 255u, 255u};
	return rgba8;
}

inline const void* corn_fallback_particle_texture_key() {
	static const int key = 0;
	return &key;
}

inline std::uint32_t ensure_corn_fallback_particle_texture(
	ParticleEmitter2CpuFrame& frame,
	std::unordered_map<const void*, std::uint32_t>& texture_indices
) {
	const void* key = corn_fallback_particle_texture_key();
	if (const auto found = texture_indices.find(key); found != texture_indices.end()) {
		return found->second;
	}

	const std::uint32_t texture_index = static_cast<std::uint32_t>(frame.textures.size());
	texture_indices.emplace(key, texture_index);
	frame.textures.push_back(ModelTextureCpuSource {
		.key = key,
		.width = 1,
		.height = 1,
		.channels = 4,
		.wrap_flags = 0,
		.srgb = false,
		.rgba8 = &corn_fallback_particle_texture_rgba8(),
	});
	return texture_index;
}

inline std::uint32_t ribbon_filter_mode_for_blend_mode(const std::uint32_t blend_mode) {
	switch (blend_mode) {
		case 3u:
		case 4u:
			return 1u;
		case 5u:
			return 2u;
		case 6u:
			return 3u;
		case 1u:
			return 4u;
		case 0u:
		case 2u:
		default:
			return 0u;
	}
}

inline void append_particle_quad(
	ParticleEmitter2CpuFrame& frame,
	const glm::vec3& v0,
	const glm::vec3& v1,
	const glm::vec3& v2,
	const glm::vec3& v3,
	float u0,
	float v0u,
	float u1,
	float v1u,
	const glm::vec4& color
);

inline float particle_draw_sort_distance(
	const ParticleEmitter2CpuFrame& frame,
	std::uint32_t first_vertex,
	std::uint32_t vertex_count,
	const glm::vec3& camera_forward
);

template <typename ModelT>
glm::vec3 model_pivot_or_origin(const ModelT& model, const std::size_t node_id) {
	return node_id < model.pivots.size() ? model.pivots[node_id] : glm::vec3(0.0f);
}

template <typename CameraT, typename MeshPtrT, typename SkeletonT>
void append_corn_emitters(
	ParticleEmitter2CpuFrame& frame,
	std::unordered_map<const void*, std::uint32_t>& texture_indices,
	const CameraT& camera,
	const MeshPtrT& mesh_ptr,
	const SkeletonT& skeleton
) {
	if (!mesh_ptr || !mesh_ptr->mdx || mesh_ptr->mdx->corn_emitters.empty()) {
		return;
	}

	const bool has_valid_sequence = skeleton.sequence_index >= 0
		&& static_cast<std::size_t>(skeleton.sequence_index) < mesh_ptr->mdx->sequences.size();
	const auto& extent = has_valid_sequence ? mesh_ptr->mdx->sequences[static_cast<std::size_t>(skeleton.sequence_index)].extent
											: mesh_ptr->mdx->extent;
	if (!camera.inside_frustrum_transform(extent.minimum, extent.maximum, skeleton.matrix)) {
		return;
	}

	auto sample = [&](const auto& track, const auto& fallback) {
		return has_valid_sequence ? interpolate_skeleton_track(skeleton, track, fallback) : fallback;
	};

	glm::vec3 right = camera.X;
	if (glm::dot(right, right) <= particle_epsilon) {
		right = glm::vec3(1.0f, 0.0f, 0.0f);
	} else {
		right = glm::normalize(right);
	}

	glm::vec3 up = camera.Y;
	up -= right * glm::dot(up, right);
	if (glm::dot(up, up) <= particle_epsilon) {
		up = glm::vec3(0.0f, 0.0f, 1.0f);
	} else {
		up = glm::normalize(up);
	}

	const auto& mdx = *mesh_ptr->mdx;
	for (const auto& emitter : mdx.corn_emitters) {
		if (emitter.node.id < 0) {
			continue;
		}
		const std::size_t node_id = static_cast<std::size_t>(emitter.node.id);
		if (node_id >= skeleton.world_matrices.size()) {
			continue;
		}

		const float visibility = std::clamp(sample(emitter.KPPV, 1.0f), 0.0f, 1.0f);
		const float alpha = std::clamp(sample(emitter.KPPA, emitter.alpha), 0.0f, 1.0f) * visibility;
		if (alpha <= 0.001f) {
			continue;
		}

		const float life_span = std::max(0.0f, sample(emitter.KPPL, emitter.life_span));
		const float emission_rate = std::max(0.0f, sample(emitter.KPPE, emitter.emission_rate));
		if (life_span <= 0.0f && emission_rate <= 0.0f) {
			continue;
		}

		const float speed = sample(emitter.KPPS, emitter.speed);
		const glm::vec3 color = glm::clamp(sample(emitter.KPPC, emitter.color), glm::vec3(0.0f), glm::vec3(1.0f));
		const float scale = std::clamp(
			std::max({1.0f, std::abs(speed) * 0.05f, life_span * 4.0f, std::sqrt(emission_rate + 1.0f)}),
			1.0f,
			96.0f
		);

		const glm::mat4 emitter_model_matrix =
			skeleton.world_matrices[node_id] * glm::translate(glm::mat4(1.0f), model_pivot_or_origin(mdx, node_id));
		const glm::vec3 center = glm::vec3(skeleton.matrix * emitter_model_matrix * glm::vec4(0.0f, 0.0f, 0.0f, 1.0f));
		const glm::vec3 axis_x = right * scale;
		const glm::vec3 axis_y = up * scale;
		const glm::vec3 c0 = center + axis_x * particle_corner_xy[0][0] + axis_y * particle_corner_xy[0][1];
		const glm::vec3 c1 = center + axis_x * particle_corner_xy[1][0] + axis_y * particle_corner_xy[1][1];
		const glm::vec3 c2 = center + axis_x * particle_corner_xy[2][0] + axis_y * particle_corner_xy[2][1];
		const glm::vec3 c3 = center + axis_x * particle_corner_xy[3][0] + axis_y * particle_corner_xy[3][1];

		const std::uint32_t texture_index = ensure_corn_fallback_particle_texture(frame, texture_indices);
		const std::uint32_t first_vertex = static_cast<std::uint32_t>(frame.positions.size());
		append_particle_quad(frame, c0, c1, c2, c3, 0.0f, 0.0f, 1.0f, 1.0f, glm::vec4(color, alpha));
		const std::uint32_t vertex_count = static_cast<std::uint32_t>(frame.positions.size()) - first_vertex;
		if (vertex_count == 0u) {
			continue;
		}

		frame.draws.push_back(ParticleEmitter2DrawCpuFrame {
			.first_vertex = first_vertex,
			.vertex_count = vertex_count,
			.texture_index = texture_index,
			.filter_mode = 1u,
			.priority_plane = 0u,
			.sort_distance = particle_draw_sort_distance(frame, first_vertex, vertex_count, camera.direction),
		});
	}
}

template <typename CameraT, typename MeshPtrT, typename SkeletonT>
void append_particle_emitter1_model_instances(
	ModelCpuFrame& frame,
	std::unordered_map<const void*, std::uint32_t>& mesh_indices,
	const CameraT& camera,
	const MeshPtrT& mesh_ptr,
	const SkeletonT& skeleton,
	const glm::vec3& color,
	std::uint32_t team_color_index,
	bool preserve_declaration_order = false,
	std::uint32_t pick_id = 0
);

template <typename CameraT, typename MeshPtrT, typename SkeletonT>
void append_particle_emitter1_model_particle_like_emitters(
	ParticleEmitter2CpuFrame& frame,
	std::unordered_map<const void*, std::uint32_t>& texture_indices,
	const CameraT& camera,
	const MeshPtrT& mesh_ptr,
	const SkeletonT& skeleton
);

inline void append_particle_vertex(
	ParticleEmitter2CpuFrame& frame,
	const glm::vec3& position,
	const glm::vec2& uv,
	const glm::vec4& color
) {
	frame.positions.emplace_back(position, 1.0f);
	frame.uvs.push_back(uv);
	frame.colors.push_back(color);
}

inline void append_particle_quad(
	ParticleEmitter2CpuFrame& frame,
	const glm::vec3& v0,
	const glm::vec3& v1,
	const glm::vec3& v2,
	const glm::vec3& v3,
	const float u0,
	const float v0u,
	const float u1,
	const float v1u,
	const glm::vec4& color
) {
	append_particle_vertex(frame, v0, glm::vec2(u0, v0u), color);
	append_particle_vertex(frame, v1, glm::vec2(u0, v1u), color);
	append_particle_vertex(frame, v2, glm::vec2(u1, v0u), color);
	append_particle_vertex(frame, v2, glm::vec2(u1, v0u), color);
	append_particle_vertex(frame, v1, glm::vec2(u0, v1u), color);
	append_particle_vertex(frame, v3, glm::vec2(u1, v1u), color);
}

inline float particle_draw_sort_distance(
	const ParticleEmitter2CpuFrame& frame,
	const std::uint32_t first_vertex,
	const std::uint32_t vertex_count,
	const glm::vec3& camera_forward
) {
	if (vertex_count == 0u || first_vertex >= frame.positions.size()) {
		return 0.0f;
	}

	const std::size_t begin = static_cast<std::size_t>(first_vertex);
	const std::size_t end = std::min(frame.positions.size(), begin + static_cast<std::size_t>(vertex_count));
	glm::vec3 center(0.0f);
	for (std::size_t i = begin; i < end; ++i) {
		center += glm::vec3(frame.positions[i]);
	}
	center /= static_cast<float>(end - begin);
	return glm::dot(center, camera_forward);
}

template <typename EmitterT, typename PoolT>
void append_particle_emitter_vertices(
	ParticleEmitter2CpuFrame& frame,
	const EmitterT& emitter,
	const PoolT& pool,
	const glm::mat4& instance_matrix,
	const glm::mat4& emitter_model_matrix,
	const glm::vec3& camera_right,
	const glm::vec3& camera_up,
	const glm::vec3& camera_forward
) {
	const float life_span = emitter.life_span;
	const float middle_time = emitter.time_middle * life_span;
	const bool model_space = (emitter.node.flags & particle_flag_model_space) != 0u;
	const bool xy_quad = (emitter.node.flags & particle_flag_xy_quad) != 0u;
	const bool sort_z = (emitter.node.flags & particle_flag_sort_primitives_far_z) != 0u;
	const bool has_head = emitter.head_or_tail == 0u || emitter.head_or_tail == 2u;
	const bool has_tail = emitter.head_or_tail == 1u || emitter.head_or_tail == 2u;
	const std::uint32_t rows = emitter.rows == 0u ? 1u : emitter.rows;
	const std::uint32_t columns = emitter.columns == 0u ? 1u : emitter.columns;
	const float inv_columns = 1.0f / static_cast<float>(columns);
	const float inv_rows = 1.0f / static_cast<float>(rows);
	const float tail_length = emitter.tail_length;

	std::vector<std::pair<float, std::size_t>> sort_buffer;
	std::vector<std::size_t> order_indices;
	const std::size_t alive = std::min(
		std::min(pool.alive_count, pool.positions.size()),
		std::min(pool.velocities.size(), std::min(pool.ages.size(), pool.key_frames.size()))
	);

	auto particle_position = [&](const std::size_t index) {
		glm::vec3 position = pool.positions[index];
		if (model_space) {
			position = glm::vec3(emitter_model_matrix * glm::vec4(position, 1.0f));
		}
		return glm::vec3(instance_matrix * glm::vec4(position, 1.0f));
	};

	if (sort_z) {
		sort_buffer.reserve(alive);
		for (std::size_t index = 0; index < alive; ++index) {
			sort_buffer.emplace_back(glm::dot(particle_position(index), camera_forward), index);
		}
		std::sort(sort_buffer.begin(), sort_buffer.end(), [](const auto& left, const auto& right) {
			return left.first > right.first;
		});
		order_indices.reserve(alive);
		for (const auto& sorted : sort_buffer) {
			order_indices.push_back(sorted.second);
		}
	}

	for (std::size_t order_index = 0; order_index < alive; ++order_index) {
		const std::size_t particle_index = sort_z ? order_indices[order_index] : order_index;

		glm::vec3 world_position = pool.positions[particle_index];
		glm::vec3 world_velocity = pool.velocities[particle_index];
		if (model_space) {
			world_position = glm::vec3(emitter_model_matrix * glm::vec4(world_position, 1.0f));
			world_velocity = glm::mat3(emitter_model_matrix) * world_velocity;
		}
		world_position = glm::vec3(instance_matrix * glm::vec4(world_position, 1.0f));
		world_velocity = glm::mat3(instance_matrix) * world_velocity;

		const float age = pool.ages[particle_index];
		const std::uint32_t key_frame = pool.key_frames[particle_index] >= 2u ? 1u : pool.key_frames[particle_index];
		const float previous_end = key_frame == 0u ? 0.0f : middle_time;
		const float end_time = key_frame == 0u ? middle_time : life_span;
		const float t = particle_key_frame_t(age, previous_end, end_time);

		glm::vec3 color_a;
		glm::vec3 color_b;
		float alpha_a = 1.0f;
		float alpha_b = 1.0f;
		float scale_a = 1.0f;
		float scale_b = 1.0f;
		glm::uvec3 head_intervals;
		glm::uvec3 tail_intervals;
		if (key_frame == 0u) {
			color_a = emitter.start_segment_color;
			color_b = emitter.middle_segment_color;
			alpha_a = static_cast<float>(emitter.segment_alphas.x) / 255.0f;
			alpha_b = static_cast<float>(emitter.segment_alphas.y) / 255.0f;
			scale_a = emitter.segment_scaling.x;
			scale_b = emitter.segment_scaling.y;
			head_intervals = emitter.head_intervals;
			tail_intervals = emitter.tail_intervals;
		} else {
			color_a = emitter.middle_segment_color;
			color_b = emitter.end_segment_color;
			alpha_a = static_cast<float>(emitter.segment_alphas.y) / 255.0f;
			alpha_b = static_cast<float>(emitter.segment_alphas.z) / 255.0f;
			scale_a = emitter.segment_scaling.y;
			scale_b = emitter.segment_scaling.z;
			head_intervals = emitter.head_decay_intervals;
			tail_intervals = emitter.tail_decay_intervals;
		}

		const glm::vec3 rgb = color_a + (color_b - color_a) * t;
		const float alpha = alpha_a + (alpha_b - alpha_a) * t;
		const float scale = scale_a + (scale_b - scale_a) * t;
		const glm::vec4 vertex_color(rgb, alpha);

		glm::vec3 right = camera_right;
		glm::vec3 up = camera_up;
		if (xy_quad) {
			const glm::vec3 velocity_xy(world_velocity.x, world_velocity.y, 0.0f);
			const glm::vec3 perpendicular_xy(world_velocity.y, -world_velocity.x, 0.0f);
			const float magnitude2 = glm::dot(velocity_xy, velocity_xy);
			if (magnitude2 > particle_epsilon) {
				right = glm::normalize(perpendicular_xy);
				up = glm::normalize(velocity_xy);
			}
		}

		if (has_head) {
			const int cell = particle_compute_cell(static_cast<int>(head_intervals.x), static_cast<int>(head_intervals.y), static_cast<int>(head_intervals.z), t);
			const int column_index = cell % static_cast<int>(columns);
			const int row_index = cell / static_cast<int>(columns);
			const float u0 = column_index * inv_columns;
			const float v0u = row_index * inv_rows;
			const float u1 = u0 + inv_columns;
			const float v1u = v0u + inv_rows;
			const glm::vec3 axis_x = right * scale;
			const glm::vec3 axis_y = up * scale;
			const glm::vec3 c0 = world_position + axis_x * particle_corner_xy[0][0] + axis_y * particle_corner_xy[0][1];
			const glm::vec3 c1 = world_position + axis_x * particle_corner_xy[1][0] + axis_y * particle_corner_xy[1][1];
			const glm::vec3 c2 = world_position + axis_x * particle_corner_xy[2][0] + axis_y * particle_corner_xy[2][1];
			const glm::vec3 c3 = world_position + axis_x * particle_corner_xy[3][0] + axis_y * particle_corner_xy[3][1];
			append_particle_quad(frame, c0, c1, c2, c3, u0, v0u, u1, v1u, vertex_color);
		}

		if (has_tail) {
			const glm::vec3 negative_velocity = -world_velocity * tail_length;
			const float velocity_magnitude2 = glm::dot(negative_velocity, negative_velocity);
			if (velocity_magnitude2 <= particle_epsilon) {
				continue;
			}

			const glm::vec3 tail_end = world_position + negative_velocity;
			const glm::vec3 tail_direction = glm::normalize(negative_velocity);
			glm::vec3 perpendicular = glm::cross(tail_direction, camera_forward);
			float perpendicular_magnitude2 = glm::dot(perpendicular, perpendicular);
			if (perpendicular_magnitude2 < particle_epsilon) {
				const glm::vec3 alternate_up = std::abs(tail_direction.z) > 0.999f ? glm::vec3(0.0f, 1.0f, 0.0f)
																					: glm::vec3(0.0f, 0.0f, 1.0f);
				perpendicular = glm::cross(tail_direction, alternate_up);
				perpendicular_magnitude2 = glm::dot(perpendicular, perpendicular);
			}
			if (perpendicular_magnitude2 <= particle_epsilon) {
				continue;
			}

			perpendicular = glm::normalize(perpendicular);
			const int cell = particle_compute_cell(static_cast<int>(tail_intervals.x), static_cast<int>(tail_intervals.y), static_cast<int>(tail_intervals.z), t);
			const int column_index = cell % static_cast<int>(columns);
			const int row_index = cell / static_cast<int>(columns);
			const float u0 = column_index * inv_columns;
			const float v0u = row_index * inv_rows;
			const float u1 = u0 + inv_columns;
			const float v1u = v0u + inv_rows;
			const glm::vec3 width = perpendicular * scale;
			const glm::vec3 head_left = world_position - width;
			const glm::vec3 head_right = world_position + width;
			const glm::vec3 tail_left = tail_end - width;
			const glm::vec3 tail_right = tail_end + width;
			append_particle_vertex(frame, head_left, glm::vec2(u0, v0u), vertex_color);
			append_particle_vertex(frame, head_right, glm::vec2(u0, v1u), vertex_color);
			append_particle_vertex(frame, tail_left, glm::vec2(u1, v0u), vertex_color);
			append_particle_vertex(frame, tail_right, glm::vec2(u1, v1u), vertex_color);
			append_particle_vertex(frame, tail_left, glm::vec2(u1, v0u), vertex_color);
			append_particle_vertex(frame, head_right, glm::vec2(u0, v1u), vertex_color);
		}
	}
}

template <typename EmitterT, typename PoolT>
void append_particle_emitter1_vertices(
	ParticleEmitter2CpuFrame& frame,
	const EmitterT& emitter,
	const PoolT& pool,
	const glm::mat4& instance_matrix,
	const glm::vec3& camera_right,
	const glm::vec3& camera_up
) {
	if (emitter.life_span <= 0.0f) {
		return;
	}

	glm::vec3 right = camera_right;
	if (glm::dot(right, right) <= particle_epsilon) {
		right = glm::vec3(1.0f, 0.0f, 0.0f);
	} else {
		right = glm::normalize(right);
	}

	glm::vec3 up = camera_up;
	if (glm::dot(up, up) <= particle_epsilon) {
		up = glm::vec3(0.0f, 0.0f, 1.0f);
	} else {
		up = glm::normalize(up);
	}

	const float scale = std::max(1.0f, std::abs(emitter.speed) * 0.02f);
	const glm::mat3 instance_basis(instance_matrix);
	const std::size_t alive = std::min(std::min(pool.alive_count, pool.positions.size()), pool.ages.size());
	for (std::size_t particle_index = 0; particle_index < alive; ++particle_index) {
		const float alpha = 1.0f - std::clamp(pool.ages[particle_index] / emitter.life_span, 0.0f, 1.0f);
		if (alpha <= 0.001f) {
			continue;
		}

		const glm::vec3 world_position = glm::vec3(instance_matrix * glm::vec4(pool.positions[particle_index], 1.0f));
		glm::vec3 sprite_right = right;
		glm::vec3 sprite_up = up;
		if constexpr (requires { pool.orientations; }) {
			if (particle_index < pool.orientations.size()) {
				const glm::vec3 oriented_right = instance_basis * pool.orientations[particle_index][0];
				const float oriented_right_magnitude2 = glm::dot(oriented_right, oriented_right);
				if (oriented_right_magnitude2 > particle_epsilon) {
					const glm::vec3 normalized_right = oriented_right * glm::inversesqrt(oriented_right_magnitude2);
					const glm::vec3 oriented_up = instance_basis * pool.orientations[particle_index][1];
					const glm::vec3 orthogonal_up = oriented_up - normalized_right * glm::dot(oriented_up, normalized_right);
					const float oriented_up_magnitude2 = glm::dot(orthogonal_up, orthogonal_up);
					if (oriented_up_magnitude2 > particle_epsilon) {
						sprite_right = normalized_right;
						sprite_up = orthogonal_up * glm::inversesqrt(oriented_up_magnitude2);
					}
				}
			}
		}

		const glm::vec3 axis_x = sprite_right * scale;
		const glm::vec3 axis_y = sprite_up * scale;
		const glm::vec3 c0 = world_position + axis_x * particle_corner_xy[0][0] + axis_y * particle_corner_xy[0][1];
		const glm::vec3 c1 = world_position + axis_x * particle_corner_xy[1][0] + axis_y * particle_corner_xy[1][1];
		const glm::vec3 c2 = world_position + axis_x * particle_corner_xy[2][0] + axis_y * particle_corner_xy[2][1];
		const glm::vec3 c3 = world_position + axis_x * particle_corner_xy[3][0] + axis_y * particle_corner_xy[3][1];
		append_particle_quad(frame, c0, c1, c2, c3, 0.0f, 0.0f, 1.0f, 1.0f, glm::vec4(1.0f, 1.0f, 1.0f, alpha));
	}
}

template <typename RibbonT, typename PoolT>
void append_ribbon_trail_vertices(
	ParticleEmitter2CpuFrame& frame,
	const RibbonT& ribbon,
	const PoolT& pool,
	const glm::mat4& instance_matrix
) {
	const std::size_t alive = std::min(
		std::min(std::min(pool.alive_count, pool.top_positions.size()), std::min(pool.bottom_positions.size(), pool.ages.size())),
		std::min(std::min(pool.alphas.size(), pool.colors.size()), pool.texture_slots.size())
	);
	if (alive < 2 || ribbon.life_span <= 0.0f) {
		return;
	}

	const std::uint32_t rows = ribbon.rows == 0u ? 1u : ribbon.rows;
	const std::uint32_t columns = ribbon.columns == 0u ? 1u : ribbon.columns;
	const std::uint32_t cell_count = std::max(1u, rows * columns);

	auto atlas_uvs = [&](const std::uint32_t texture_slot) {
		const std::uint32_t cell = texture_slot % cell_count;
		const std::uint32_t column_index = cell % columns;
		const std::uint32_t row_index = cell / columns;
		const float u0 = static_cast<float>(column_index) / static_cast<float>(columns);
		const float v0u = static_cast<float>(row_index) / static_cast<float>(rows);
		const float u1 = u0 + 1.0f / static_cast<float>(columns);
		const float v1u = v0u + 1.0f / static_cast<float>(rows);
		return std::array<float, 4> {u0, v0u, u1, v1u};
	};

	auto point_color = [&](const std::size_t point_index) {
		const float life_t = std::clamp(pool.ages[point_index] / ribbon.life_span, 0.0f, 1.0f);
		const float alpha = pool.alphas[point_index] * (1.0f - life_t);
		return glm::vec4(pool.colors[point_index], alpha);
	};

	auto transform_point = [&](const glm::vec3& point) {
		return glm::vec3(instance_matrix * glm::vec4(point, 1.0f));
	};

	for (std::size_t point_index = 0; point_index + 1 < alive; ++point_index) {
		const glm::vec4 color0 = point_color(point_index);
		const glm::vec4 color1 = point_color(point_index + 1);
		if (color0.w <= 0.001f && color1.w <= 0.001f) {
			continue;
		}

		const auto [u0, v0u, u1, v1u] = atlas_uvs(pool.texture_slots[point_index]);
		const glm::vec3 top0 = transform_point(pool.top_positions[point_index]);
		const glm::vec3 bottom0 = transform_point(pool.bottom_positions[point_index]);
		const glm::vec3 top1 = transform_point(pool.top_positions[point_index + 1]);
		const glm::vec3 bottom1 = transform_point(pool.bottom_positions[point_index + 1]);

		append_particle_vertex(frame, top0, glm::vec2(u0, v0u), color0);
		append_particle_vertex(frame, bottom0, glm::vec2(u0, v1u), color0);
		append_particle_vertex(frame, top1, glm::vec2(u1, v0u), color1);
		append_particle_vertex(frame, top1, glm::vec2(u1, v0u), color1);
		append_particle_vertex(frame, bottom0, glm::vec2(u0, v1u), color0);
		append_particle_vertex(frame, bottom1, glm::vec2(u1, v1u), color1);
	}
}

template <typename CameraT, typename MeshPtrT, typename SkeletonT>
void append_particle_emitters1(
	ParticleEmitter2CpuFrame& frame,
	std::unordered_map<const void*, std::uint32_t>& texture_indices,
	const CameraT& camera,
	const MeshPtrT& mesh_ptr,
	const SkeletonT& skeleton
) {
	if (!mesh_ptr || !mesh_ptr->mdx || mesh_ptr->mdx->emitters1.empty()) {
		return;
	}
	if constexpr (!(requires { skeleton.particle_emitters1.pools; } && requires { mesh_ptr->emitter1_textures; })) {
		return;
	} else {
		const bool has_valid_sequence = skeleton.sequence_index >= 0
			&& static_cast<std::size_t>(skeleton.sequence_index) < mesh_ptr->mdx->sequences.size();
		const auto& extent = has_valid_sequence ? mesh_ptr->mdx->sequences[static_cast<std::size_t>(skeleton.sequence_index)].extent
												: mesh_ptr->mdx->extent;
		if (!camera.inside_frustrum_transform(extent.minimum, extent.maximum, skeleton.matrix)) {
			return;
		}

		const auto& mdx = *mesh_ptr->mdx;
		for (std::size_t emitter_index = 0; emitter_index < mdx.emitters1.size(); ++emitter_index) {
			const auto& emitter = mdx.emitters1[emitter_index];
			if (emitter.node.id < 0 || (static_cast<std::uint32_t>(emitter.node.flags) & particle_emitter1_uses_tga) == 0u
				|| emitter_index >= skeleton.particle_emitters1.pools.size()) {
				continue;
			}
			const auto& pool = skeleton.particle_emitters1.pools[emitter_index];
			if (pool.alive_count == 0) {
				continue;
			}

			std::uint32_t texture_index = 0;
			if (!ensure_particle_texture(frame, texture_indices, mesh_ptr->emitter1_textures, static_cast<std::uint32_t>(emitter_index), texture_index)) {
				continue;
			}

			const std::uint32_t first_vertex = static_cast<std::uint32_t>(frame.positions.size());
			append_particle_emitter1_vertices(frame, emitter, pool, skeleton.matrix, camera.X, camera.Y);
			const std::uint32_t vertex_count = static_cast<std::uint32_t>(frame.positions.size()) - first_vertex;
			if (vertex_count == 0) {
				continue;
			}

			frame.draws.push_back(ParticleEmitter2DrawCpuFrame {
				.first_vertex = first_vertex,
				.vertex_count = vertex_count,
				.texture_index = texture_index,
				.filter_mode = 0u,
				.priority_plane = 0u,
				.sort_distance = particle_draw_sort_distance(frame, first_vertex, vertex_count, camera.direction),
			});
		}
	}
}

template <typename MeshPtrT, typename SkeletonT>
void append_ribbon_emitters(
	ParticleEmitter2CpuFrame& frame,
	std::unordered_map<const void*, std::uint32_t>& texture_indices,
	const MeshPtrT& mesh_ptr,
	const SkeletonT& skeleton,
	const glm::vec3& camera_right,
	const glm::vec3& camera_forward
) {
	if (!mesh_ptr || !mesh_ptr->mdx || mesh_ptr->mdx->ribbons.empty()) {
		return;
	}

	const auto& mdx = *mesh_ptr->mdx;
	const bool has_valid_sequence = skeleton.sequence_index >= 0
		&& static_cast<std::size_t>(skeleton.sequence_index) < mdx.sequences.size();
	auto sample = [&](const auto& track, const auto& fallback) {
		return has_valid_sequence ? interpolate_skeleton_track(skeleton, track, fallback) : fallback;
	};
	for (std::size_t ribbon_index = 0; ribbon_index < mdx.ribbons.size(); ++ribbon_index) {
		const auto& ribbon = mdx.ribbons[ribbon_index];
		if (ribbon.node.id < 0) {
			continue;
		}
		const std::size_t node_id = static_cast<std::size_t>(ribbon.node.id);
		if (node_id >= skeleton.world_matrices.size() || ribbon.material_id >= mdx.materials.size()) {
			continue;
		}

		const auto& material = mdx.materials[ribbon.material_id];
		if (material.layers.empty() || material.layers[0].textures.empty()) {
			continue;
		}

		const auto& layer = material.layers[0];
		const std::uint32_t texture_id = layer.textures[0].id;
		std::uint32_t texture_index = 0;
		if (!ensure_particle_texture(frame, texture_indices, mesh_ptr->textures, texture_id, texture_index)) {
			continue;
		}

		const std::uint32_t first_vertex = static_cast<std::uint32_t>(frame.positions.size());
		bool emitted_history = false;
		if constexpr (requires { skeleton.ribbon_emitters.pools; }) {
			if (ribbon_index < skeleton.ribbon_emitters.pools.size()) {
				append_ribbon_trail_vertices(frame, ribbon, skeleton.ribbon_emitters.pools[ribbon_index], skeleton.matrix);
				emitted_history = static_cast<std::uint32_t>(frame.positions.size()) > first_vertex;
			}
		}

		if (!emitted_history) {
			const float visibility = sample(ribbon.KRVS, 1.0f);
			const float alpha = sample(ribbon.KRAL, ribbon.alpha) * visibility;
			if (alpha <= 0.001f) {
				continue;
			}

			const float height_above = sample(ribbon.KRHA, ribbon.height_above);
			const float height_below = sample(ribbon.KRHB, ribbon.height_below);
			const glm::vec3 color = sample(ribbon.KRCO, ribbon.color);
			const std::uint32_t texture_slot = sample(ribbon.KRTX, ribbon.texture_slot);
			const std::uint32_t rows = ribbon.rows == 0u ? 1u : ribbon.rows;
			const std::uint32_t columns = ribbon.columns == 0u ? 1u : ribbon.columns;
			const std::uint32_t cell_count = std::max(1u, rows * columns);
			const std::uint32_t cell = texture_slot % cell_count;
			const std::uint32_t column_index = cell % columns;
			const std::uint32_t row_index = cell / columns;
			const float u0 = static_cast<float>(column_index) / static_cast<float>(columns);
			const float v0u = static_cast<float>(row_index) / static_cast<float>(rows);
			const float u1 = u0 + 1.0f / static_cast<float>(columns);
			const float v1u = v0u + 1.0f / static_cast<float>(rows);

			const glm::mat4 ribbon_model_matrix =
				skeleton.world_matrices[node_id] * glm::translate(glm::mat4(1.0f), model_pivot_or_origin(mdx, node_id));
			const glm::vec3 center = glm::vec3(skeleton.matrix * ribbon_model_matrix * glm::vec4(0.0f, 0.0f, 0.0f, 1.0f));
			glm::vec3 up = glm::mat3(skeleton.matrix) * glm::mat3(ribbon_model_matrix) * glm::vec3(0.0f, 0.0f, 1.0f);
			if (glm::dot(up, up) <= particle_epsilon) {
				up = glm::vec3(0.0f, 0.0f, 1.0f);
			} else {
				up = glm::normalize(up);
			}

			glm::vec3 right = camera_right;
			if (glm::dot(right, right) <= particle_epsilon) {
				right = glm::vec3(1.0f, 0.0f, 0.0f);
			} else {
				right = glm::normalize(right);
			}

			const float half_width = std::max(0.5f, (std::abs(height_above) + std::abs(height_below)) * 0.05f);
			const glm::vec3 top = center + up * height_above;
			const glm::vec3 bottom = center - up * height_below;
			const glm::vec4 vertex_color(color, alpha);
			append_particle_quad(
				frame,
				top - right * half_width,
				bottom - right * half_width,
				top + right * half_width,
				bottom + right * half_width,
				u0,
				v0u,
				u1,
				v1u,
				vertex_color
			);
		}

		const std::uint32_t vertex_count = static_cast<std::uint32_t>(frame.positions.size()) - first_vertex;
		if (vertex_count == 0) {
			continue;
		}
		frame.draws.push_back(ParticleEmitter2DrawCpuFrame {
			.first_vertex = first_vertex,
			.vertex_count = vertex_count,
			.texture_index = texture_index,
			.filter_mode = ribbon_filter_mode_for_blend_mode(layer.blend_mode),
			.priority_plane = material.priority_plane,
			.sort_distance = particle_draw_sort_distance(frame, first_vertex, vertex_count, camera_forward),
		});
	}
}

template <typename CameraT, typename MeshPtrT, typename SkeletonT>
void append_particle_emitters2(
	ParticleEmitter2CpuFrame& frame,
	std::unordered_map<const void*, std::uint32_t>& texture_indices,
	const CameraT& camera,
	const MeshPtrT& mesh_ptr,
	const SkeletonT& skeleton
) {
	if (!mesh_ptr || !mesh_ptr->mdx || mesh_ptr->mdx->emitters2.empty()) {
		return;
	}

	const bool has_valid_sequence = skeleton.sequence_index >= 0
		&& static_cast<std::size_t>(skeleton.sequence_index) < mesh_ptr->mdx->sequences.size();
	const auto& extent = has_valid_sequence ? mesh_ptr->mdx->sequences[static_cast<std::size_t>(skeleton.sequence_index)].extent
											: mesh_ptr->mdx->extent;
	if (!camera.inside_frustrum_transform(extent.minimum, extent.maximum, skeleton.matrix)) {
		return;
	}

	const auto& mdx = *mesh_ptr->mdx;
	for (std::size_t emitter_index = 0; emitter_index < mdx.emitters2.size(); ++emitter_index) {
		const auto& emitter = mdx.emitters2[emitter_index];
		if (emitter.node.id < 0 || emitter_index >= skeleton.particles.pools.size()) {
			continue;
		}
		const std::size_t node_id = static_cast<std::size_t>(emitter.node.id);
		if (node_id >= skeleton.world_matrices.size()) {
			continue;
		}
		const auto& pool = skeleton.particles.pools[emitter_index];
		if (pool.alive_count == 0) {
			continue;
		}

		std::uint32_t texture_index = 0;
		if (emitter.replaceable_id != 0 && emitter_index < mesh_ptr->emitter2_replaceable_textures.size()
			&& mesh_ptr->emitter2_replaceable_textures[emitter_index]) {
			if (!ensure_particle_texture(
					frame,
					texture_indices,
					mesh_ptr->emitter2_replaceable_textures,
					static_cast<std::uint32_t>(emitter_index),
					texture_index
				)) {
				continue;
			}
		} else if (!ensure_particle_texture(frame, texture_indices, mesh_ptr->textures, emitter.texture_id, texture_index)) {
			continue;
		}

		const std::uint32_t first_vertex = static_cast<std::uint32_t>(frame.positions.size());
		const glm::mat4 emitter_model_matrix =
			skeleton.world_matrices[node_id] * glm::translate(glm::mat4(1.0f), model_pivot_or_origin(mdx, node_id));
		append_particle_emitter_vertices(
			frame,
			emitter,
			pool,
			skeleton.matrix,
			emitter_model_matrix,
			camera.X,
			camera.Y,
			camera.direction
		);
		const std::uint32_t vertex_count = static_cast<std::uint32_t>(frame.positions.size()) - first_vertex;
		if (vertex_count == 0) {
			continue;
		}

		frame.draws.push_back(ParticleEmitter2DrawCpuFrame {
			.first_vertex = first_vertex,
			.vertex_count = vertex_count,
			.texture_index = texture_index,
			.filter_mode = emitter.filter_mode,
			.priority_plane = emitter.priority_plane,
			.sort_distance = particle_draw_sort_distance(frame, first_vertex, vertex_count, camera.direction),
		});
	}
}

template <typename CameraT, typename MeshPtrT, typename SkeletonT>
void append_particle_like_emitters(
	ParticleEmitter2CpuFrame& frame,
	std::unordered_map<const void*, std::uint32_t>& texture_indices,
	const CameraT& camera,
	const MeshPtrT& mesh_ptr,
	const SkeletonT& skeleton
) {
	append_particle_emitters1(frame, texture_indices, camera, mesh_ptr, skeleton);
	append_particle_emitters2(frame, texture_indices, camera, mesh_ptr, skeleton);
	append_ribbon_emitters(frame, texture_indices, mesh_ptr, skeleton, camera.X, camera.direction);
	append_corn_emitters(frame, texture_indices, camera, mesh_ptr, skeleton);
}

} // namespace detail

inline void finalize_model_cpu_frame(ModelCpuFrame& frame) {
	std::stable_sort(frame.opaque_sd_draws.begin(), frame.opaque_sd_draws.end(), detail::draw_order_less);
	std::stable_sort(frame.opaque_hd_draws.begin(), frame.opaque_hd_draws.end(), detail::draw_order_less);
	std::stable_sort(frame.transparent_sd_draws.begin(), frame.transparent_sd_draws.end(), detail::transparent_draw_order_less);
	std::stable_sort(frame.transparent_hd_draws.begin(), frame.transparent_hd_draws.end(), detail::transparent_draw_order_less);
}

inline void finalize_particle_emitter2_cpu_frame(ParticleEmitter2CpuFrame& frame) {
	std::stable_sort(frame.draws.begin(), frame.draws.end(), detail::particle_draw_order_less);
}

template <typename MapT, typename CameraT>
ModelCpuFrame make_model_cpu_frame(const MapT& map, const CameraT& camera) {
	ModelCpuFrame frame;
	std::unordered_map<const void*, std::uint32_t> mesh_indices;

	if (map.render_doodads) {
		for (const auto& doodad : map.doodads.doodads) {
			detail::append_model_instance(frame, mesh_indices, camera, doodad.mesh, doodad.skeleton, doodad.color, 0);
			detail::append_particle_emitter1_model_instances(frame, mesh_indices, camera, doodad.mesh, doodad.skeleton, doodad.color, 0);
		}
		for (const auto& doodad : map.doodads.special_doodads) {
			detail::append_model_instance(frame, mesh_indices, camera, doodad.mesh, doodad.skeleton, glm::vec3(1.0f), 0);
			detail::append_particle_emitter1_model_instances(frame, mesh_indices, camera, doodad.mesh, doodad.skeleton, glm::vec3(1.0f), 0);
		}
	}

	if (map.render_units) {
		for (const auto& unit : map.units.units) {
			if (unit.id == "sloc") {
				continue;
			}
			detail::append_model_instance(frame, mesh_indices, camera, unit.mesh, unit.skeleton, unit.color, unit.player);
			detail::append_particle_emitter1_model_instances(frame, mesh_indices, camera, unit.mesh, unit.skeleton, unit.color, unit.player);
		}
		for (const auto& item : map.units.items) {
			detail::append_model_instance(frame, mesh_indices, camera, item.mesh, item.skeleton, item.color, item.player);
			detail::append_particle_emitter1_model_instances(frame, mesh_indices, camera, item.mesh, item.skeleton, item.color, item.player);
		}
	}

	finalize_model_cpu_frame(frame);
	return frame;
}

template <typename MapT, typename CameraT>
ParticleEmitter2CpuFrame make_particle_emitter2_cpu_frame(const MapT& map, const CameraT& camera) {
	ParticleEmitter2CpuFrame frame;
	std::unordered_map<const void*, std::uint32_t> texture_indices;

	if (map.render_doodads) {
		for (const auto& doodad : map.doodads.doodads) {
			detail::append_particle_like_emitters(frame, texture_indices, camera, doodad.mesh, doodad.skeleton);
			detail::append_particle_emitter1_model_particle_like_emitters(frame, texture_indices, camera, doodad.mesh, doodad.skeleton);
		}
		for (const auto& doodad : map.doodads.special_doodads) {
			detail::append_particle_like_emitters(frame, texture_indices, camera, doodad.mesh, doodad.skeleton);
			detail::append_particle_emitter1_model_particle_like_emitters(frame, texture_indices, camera, doodad.mesh, doodad.skeleton);
		}
	}

	if (map.render_units) {
		for (const auto& unit : map.units.units) {
			if (unit.id == "sloc") {
				continue;
			}
			detail::append_particle_like_emitters(frame, texture_indices, camera, unit.mesh, unit.skeleton);
			detail::append_particle_emitter1_model_particle_like_emitters(frame, texture_indices, camera, unit.mesh, unit.skeleton);
		}
		for (const auto& item : map.units.items) {
			detail::append_particle_like_emitters(frame, texture_indices, camera, item.mesh, item.skeleton);
			detail::append_particle_emitter1_model_particle_like_emitters(frame, texture_indices, camera, item.mesh, item.skeleton);
		}
	}

	finalize_particle_emitter2_cpu_frame(frame);
	return frame;
}

template <typename MeshPtrT, typename SkeletonT>
ModelCpuFrame make_model_preview_cpu_frame(
	const MeshPtrT& mesh,
	const SkeletonT& skeleton,
	const glm::vec3& camera_position,
	const glm::vec3& camera_direction,
	const float camera_distance,
	const glm::vec3& color = glm::vec3(1.0f),
	const std::uint32_t team_color_index = 0
) {
	struct PreviewCamera {
		glm::vec3 position;
		glm::vec3 direction;
		float distance = 0.0f;

		bool inside_frustrum_transform(const glm::vec3&, const glm::vec3&, const glm::mat4&) const {
			return true;
		}
	};

	ModelCpuFrame frame;
	std::unordered_map<const void*, std::uint32_t> mesh_indices;
	detail::append_model_instance(
		frame,
		mesh_indices,
		PreviewCamera {.position = camera_position, .direction = camera_direction, .distance = camera_distance},
		mesh,
		skeleton,
		color,
		team_color_index,
		0,
		true
	);
	detail::append_particle_emitter1_model_instances(
		frame,
		mesh_indices,
		PreviewCamera {.position = camera_position, .direction = camera_direction, .distance = camera_distance},
		mesh,
		skeleton,
		color,
		team_color_index,
		true
	);

	return frame;
}

template <typename MeshPtrT, typename SkeletonT>
ParticleEmitter2CpuFrame make_particle_emitter2_preview_cpu_frame(
	const MeshPtrT& mesh,
	const SkeletonT& skeleton,
	const glm::vec3& camera_right,
	const glm::vec3& camera_up,
	const glm::vec3& camera_forward
) {
	struct PreviewCamera {
		glm::vec3 X;
		glm::vec3 Y;
		glm::vec3 direction;

		bool inside_frustrum_transform(const glm::vec3&, const glm::vec3&, const glm::mat4&) const {
			return true;
		}
	};

	ParticleEmitter2CpuFrame frame;
	std::unordered_map<const void*, std::uint32_t> texture_indices;
	detail::append_particle_like_emitters(
		frame,
		texture_indices,
		PreviewCamera {.X = camera_right, .Y = camera_up, .direction = camera_forward},
		mesh,
		skeleton
	);
	detail::append_particle_emitter1_model_particle_like_emitters(
		frame,
		texture_indices,
		PreviewCamera {.X = camera_right, .Y = camera_up, .direction = camera_forward},
		mesh,
		skeleton
	);
	finalize_particle_emitter2_cpu_frame(frame);
	return frame;
}

template <typename MapT, typename CameraT>
ModelCpuFrame make_unit_picking_cpu_frame(const MapT& map, const CameraT& camera) {
	ModelCpuFrame frame;
	std::unordered_map<const void*, std::uint32_t> mesh_indices;

	for (std::size_t i = 0; i < map.units.units.size(); ++i) {
		const auto& unit = map.units.units[i];
		if (unit.id == "sloc") {
			continue;
		}
		const std::uint32_t pick_id = static_cast<std::uint32_t>(i + 1);
		detail::append_model_instance(frame, mesh_indices, camera, unit.mesh, unit.skeleton, unit.color, unit.player, pick_id);
		detail::append_model_bounds_picking_instance(frame, mesh_indices, camera, unit.mesh, unit.skeleton, unit.player, pick_id);
		detail::append_particle_emitter1_model_instances(frame, mesh_indices, camera, unit.mesh, unit.skeleton, unit.color, unit.player, false, pick_id);
	}

	return frame;
}

namespace detail {

template <typename UnitT>
std::uint32_t unit_team_color_index(const UnitT& unit) {
	if constexpr (requires { unit.player; }) {
		using PlayerT = std::remove_cvref_t<decltype(unit.player)>;
		if constexpr (std::is_signed_v<PlayerT>) {
			return unit.player < 0 ? 0u : static_cast<std::uint32_t>(unit.player);
		} else {
			return static_cast<std::uint32_t>(unit.player);
		}
	} else {
		return 0u;
	}
}

} // namespace detail

template <typename MapT, typename CameraT, typename MarkerMeshPtrT, typename MarkerSkeletonFactoryT>
ModelCpuFrame make_unit_picking_cpu_frame(
	const MapT& map,
	const CameraT& camera,
	const MarkerMeshPtrT& start_location_marker_mesh,
	MarkerSkeletonFactoryT&& make_start_location_marker_skeleton
) {
	ModelCpuFrame frame;
	std::unordered_map<const void*, std::uint32_t> mesh_indices;

	for (std::size_t i = 0; i < map.units.units.size(); ++i) {
		const auto& unit = map.units.units[i];
		const std::uint32_t pick_id = static_cast<std::uint32_t>(i + 1);
		if (unit.id == "sloc") {
			if (start_location_marker_mesh && start_location_marker_mesh->has_mesh && start_location_marker_mesh->mdx) {
				auto marker_skeleton = make_start_location_marker_skeleton(start_location_marker_mesh, unit);
				detail::append_model_instance(
					frame,
					mesh_indices,
					camera,
					start_location_marker_mesh,
					marker_skeleton,
					glm::vec3(0.25f, 0.65f, 1.0f),
					detail::unit_team_color_index(unit),
					pick_id
				);
			}
			continue;
		}
		detail::append_model_instance(frame, mesh_indices, camera, unit.mesh, unit.skeleton, unit.color, unit.player, pick_id);
		detail::append_model_bounds_picking_instance(frame, mesh_indices, camera, unit.mesh, unit.skeleton, unit.player, pick_id);
		detail::append_particle_emitter1_model_instances(frame, mesh_indices, camera, unit.mesh, unit.skeleton, unit.color, unit.player, false, pick_id);
	}

	return frame;
}

template <typename MapT, typename CameraT>
ModelCpuFrame make_doodad_picking_cpu_frame(const MapT& map, const CameraT& camera) {
	ModelCpuFrame frame;
	std::unordered_map<const void*, std::uint32_t> mesh_indices;

	for (std::size_t i = 0; i < map.doodads.doodads.size(); ++i) {
		const auto& doodad = map.doodads.doodads[i];
		const std::uint32_t pick_id = static_cast<std::uint32_t>(i + 1);
		detail::append_model_instance(frame, mesh_indices, camera, doodad.mesh, doodad.skeleton, doodad.color, 0, pick_id);
		detail::append_model_bounds_picking_instance(frame, mesh_indices, camera, doodad.mesh, doodad.skeleton, 0, pick_id);
		detail::append_particle_emitter1_model_instances(frame, mesh_indices, camera, doodad.mesh, doodad.skeleton, doodad.color, 0, false, pick_id);
	}

	return frame;
}

class MetalMapRenderer final {
public:
	MetalMapRenderer();
	~MetalMapRenderer();

	MetalMapRenderer(MetalMapRenderer&&) noexcept;
	MetalMapRenderer& operator=(MetalMapRenderer&&) noexcept;

	MetalMapRenderer(const MetalMapRenderer&) = delete;
	MetalMapRenderer& operator=(const MetalMapRenderer&) = delete;

	[[nodiscard]]
	bool initialize(const FrameContext& frame_context, std::string shader_path = "data/shaders/metal_map_renderer.metal");

	void invalidate_resources();

	// Encodes into frame_context.command_buffer. The caller owns present/commit.
	[[nodiscard]]
	bool render(const FrameContext& frame_context, const MetalMapFrame& frame);

	// Encodes model/particle/overlay passes into frame_context.command_buffer. The caller owns present/commit.
	[[nodiscard]]
	bool render_models(const FrameContext& frame_context, const MetalModelFrame& frame);

	// Encodes into frame_context.command_buffer, commits it, and waits for the readback.
	[[nodiscard]]
	std::optional<std::uint32_t> pick_model_id(
		const FrameContext& frame_context,
		const ModelCpuFrame& model_frame,
		const CameraUniforms& camera,
		glm::vec2 mouse_position
	);

	// Encodes the same offscreen color pass used by pick_model_id, commits it, waits, and reads it back as BGRA8 pixels.
	[[nodiscard]]
	bool capture_model_picking_frame(
		const FrameContext& frame_context,
		const ModelCpuFrame& model_frame,
		const CameraUniforms& camera,
		std::vector<std::uint8_t>& bgra_pixels,
		int& width,
		int& height
	);

	[[nodiscard]]
	const std::string& last_error() const;

	[[nodiscard]]
	const MetalTextureFallbackStats& last_texture_fallback_stats() const;

private:
	struct Impl;
	std::unique_ptr<Impl> impl_;
};

} // namespace hive::metal
