---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/rendering/metal_map_frame_builders.h"
ref: HEAD
---

#pragma once

#include "rendering/metal_map_renderer.h"

namespace hive::metal {

template <typename BrushT>
BrushMask make_brush_mask_cpu_frame(const BrushT& brush) {
	const glm::ivec2 size = brush.brush_mask_size();
	return BrushMask {
		.source_key = &brush,
		.revision = brush.brush_mask_revision(),
		.width = size.x,
		.height = size.y,
		.world_position = brush.get_position(),
		.rgba8 = brush.brush_mask_pixels(),
	};
}

template <typename BrushOverlayFrameT>
SelectionOverlayCpuFrame make_selection_overlay_cpu_frame(const BrushOverlayFrameT& brush_frame) {
	SelectionOverlayCpuFrame frame;
	frame.positions = brush_frame.positions;
	frame.uvs = brush_frame.uvs;
	frame.colors = brush_frame.colors;
	frame.draws.reserve(brush_frame.draws.size());
	for (const auto& draw : brush_frame.draws) {
		frame.draws.push_back(SelectionOverlayDrawCpuFrame {
			.first_vertex = draw.first_vertex,
			.vertex_count = draw.vertex_count,
			.primitive_type = draw.primitive_type,
			.style = draw.style,
		});
	}
	return frame;
}

template <typename CameraT, typename MeshT, typename SkeletonT>
void append_queued_model_to_map_frames(
	ModelCpuFrame& models,
	ParticleEmitter2CpuFrame& particles,
	std::unordered_map<const void*, std::uint32_t>& mesh_indices,
	std::unordered_map<const void*, std::uint32_t>& particle_texture_indices,
	const CameraT& camera,
	MeshT& skinned_mesh,
	const SkeletonT& skeleton,
	const glm::vec3& color,
	const std::uint32_t team_color_index
) {
	detail::append_model_instance(models, mesh_indices, camera, &skinned_mesh, skeleton, color, team_color_index);
	detail::append_particle_emitter1_model_instances(models, mesh_indices, camera, &skinned_mesh, skeleton, color, team_color_index);
	detail::append_particle_like_emitters(particles, particle_texture_indices, camera, &skinned_mesh, skeleton);
	detail::append_particle_emitter1_model_particle_like_emitters(particles, particle_texture_indices, camera, &skinned_mesh, skeleton);
}

struct TerrainBuildOptions {
	bool include_ground = true;
	bool include_water = true;
};

template <typename TerrainT>
TerrainCpuFrame make_terrain_cpu_frame(const TerrainT& terrain, TerrainBuildOptions options = {}) {
	TerrainCpuFrame frame;
	frame.source_key = &terrain;
	frame.width = terrain.width;
	frame.height = terrain.height;
	frame.includes_ground = options.include_ground;
	frame.includes_water = options.include_water;
	frame.ground_height_revision = terrain.ground_height_revision_value();
	frame.final_ground_height_revision = terrain.final_ground_height_revision_value();
	frame.ground_texture_data_revision = terrain.ground_texture_data_revision_value();
	frame.ground_exists_revision = terrain.ground_exists_revision_value();
	frame.water_height_revision = terrain.water_height_revision_value();
	frame.water_exists_revision = terrain.water_exists_revision_value();

	if (frame.width <= 1 || frame.height <= 1) {
		return frame;
	}

	const auto corner_count = checked_frame_element_count(frame.width, frame.height);
	const auto tile_count = checked_terrain_tile_count(frame.width, frame.height);
	if (!corner_count || !tile_count) {
		return frame;
	}

	frame.corner_ground_heights = terrain.corner_height;
	frame.final_ground_heights = options.include_ground ? terrain.final_ground_heights() : std::vector<float>(*corner_count);
	frame.ground_texture_indices =
		options.include_ground ? terrain.ground_texture_render_list() : std::vector<glm::uvec4>(*tile_count, glm::uvec4(0xFFFFu));
	frame.ground_exists = options.include_ground ? terrain.ground_exists_render_data() : std::vector<std::uint32_t>(*tile_count, 0u);
	frame.corner_water_heights = terrain.corner_water_height;
	frame.water_exists = options.include_water ? terrain.water_exists_render_data() : std::vector<std::uint32_t>(*corner_count, 0u);

	return frame;
}

template <typename TerrainT>
CliffCpuFrame make_cliff_cpu_frame(const TerrainT& terrain) {
	CliffCpuFrame frame;
	std::unordered_map<const void*, std::uint32_t> mesh_indices;

	for (const auto& cliff : terrain.cliffs) {
		if (cliff.x < 0 || cliff.y < 0 || cliff.x + 1 >= terrain.width || cliff.y + 1 >= terrain.height || cliff.z < 0
			|| static_cast<std::size_t>(cliff.z) >= terrain.cliff_meshes.size()) {
			continue;
		}

		const std::size_t bl = terrain.ci(cliff.x, cliff.y);
		if (terrain.corner_special_doodad[bl]) {
			continue;
		}

		const auto& mesh = terrain.cliff_meshes[static_cast<std::size_t>(cliff.z)];
		if (!mesh || mesh->cpu_vertices.empty() || mesh->cpu_indices.empty()) {
			continue;
		}

		const void* key = mesh.get();
		std::uint32_t mesh_index = 0;
		if (const auto found = mesh_indices.find(key); found != mesh_indices.end()) {
			mesh_index = found->second;
		} else {
			mesh_index = static_cast<std::uint32_t>(frame.meshes.size());
			mesh_indices.emplace(key, mesh_index);
			frame.meshes.push_back(CliffMeshCpuFrame {
				.key = key,
				.revision = mesh->cpu_revision,
				.vertices = &mesh->cpu_vertices,
				.uvs = &mesh->cpu_uvs,
				.normals = &mesh->cpu_normals,
				.indices = &mesh->cpu_indices,
			});
		}

		const std::size_t br = terrain.ci(cliff.x + 1, cliff.y);
		const std::size_t tl = terrain.ci(cliff.x, cliff.y + 1);
		const std::size_t tr = terrain.ci(cliff.x + 1, cliff.y + 1);
		const float base_layer =
			static_cast<float>(std::min({terrain.corner_layer_height[bl], terrain.corner_layer_height[br],
										 terrain.corner_layer_height[tl], terrain.corner_layer_height[tr]}));
		const std::uint32_t cliff_texture = terrain.corner_cliff_texture[bl];
		const std::uint32_t texture_layer = cliff_texture < terrain.cliff_textures.size() ? cliff_texture : 0u;
		const std::uint32_t instance_offset = static_cast<std::uint32_t>(frame.instances.size());
		frame.instances.emplace_back(static_cast<float>(cliff.x), static_cast<float>(cliff.y), base_layer - 2.0f, texture_layer);

		frame.draws.push_back(CliffDrawCpuFrame {
			.mesh_index = mesh_index,
			.instance_offset = instance_offset,
			.instance_count = 1,
			.index_count = static_cast<std::uint32_t>(mesh->cpu_indices.size()),
		});
	}

	return frame;
}

namespace detail {

template <typename TextureT>
bool append_rgba8_region(
	std::vector<std::uint8_t>& out,
	const TextureT& texture,
	const int src_x,
	const int src_y,
	const int width,
	const int height
) {
	const auto append_size = checked_frame_byte_count(width, height, 4);
	if (!append_size || out.size() > std::numeric_limits<std::size_t>::max() - *append_size) {
		return false;
	}

	const std::size_t start = out.size();
	out.resize(start + *append_size);

	for (int y = 0; y < height; ++y) {
		for (int x = 0; x < width; ++x) {
			const std::size_t src = (static_cast<std::size_t>(src_y + y) * static_cast<std::size_t>(texture.width)
									 + static_cast<std::size_t>(src_x + x))
				* static_cast<std::size_t>(texture.channels);
			const std::size_t dst =
				start + (static_cast<std::size_t>(y) * static_cast<std::size_t>(width) + static_cast<std::size_t>(x)) * 4u;
			out[dst + 0] = texture.data[src + 0];
			out[dst + 1] = texture.data[src + 1];
			out[dst + 2] = texture.data[src + 2];
			out[dst + 3] = texture.channels >= 4 ? texture.data[src + 3] : 255;
		}
	}
	return true;
}

template <typename TextureT>
bool append_rgba8_padded_layer(std::vector<std::uint8_t>& out, const TextureT& texture, const int width, const int height) {
	const auto append_size = checked_frame_byte_count(width, height, 4);
	if (!append_size || out.size() > std::numeric_limits<std::size_t>::max() - *append_size) {
		return false;
	}

	const std::size_t start = out.size();
	out.resize(start + *append_size);

	for (int y = 0; y < height; ++y) {
		const int source_y = y % texture.height;
		for (int x = 0; x < width; ++x) {
			const int source_x = x % texture.width;
			const std::size_t src =
				(static_cast<std::size_t>(source_y) * static_cast<std::size_t>(texture.width) + static_cast<std::size_t>(source_x))
				* static_cast<std::size_t>(texture.channels);
			const std::size_t dst =
				start + (static_cast<std::size_t>(y) * static_cast<std::size_t>(width) + static_cast<std::size_t>(x)) * 4u;
			out[dst + 0] = texture.data[src + 0];
			out[dst + 1] = texture.data[src + 1];
			out[dst + 2] = texture.data[src + 2];
			out[dst + 3] = texture.channels >= 4 ? texture.data[src + 3] : 255;
		}
	}
	return true;
}

inline bool append_rgba8_fallback_layer(std::vector<std::uint8_t>& out, const int width, const int height) {
	const auto append_size = checked_frame_byte_count(width, height, 4);
	if (!append_size || out.size() > std::numeric_limits<std::size_t>::max() - *append_size) {
		return false;
	}

	const std::size_t start = out.size();
	out.resize(start + *append_size);

	for (std::size_t offset = start; offset < out.size(); offset += 4) {
		out[offset + 0] = 255u;
		out[offset + 1] = 0u;
		out[offset + 2] = 255u;
		out[offset + 3] = 255u;
	}
	return true;
}

inline bool append_rgba8_fallback_layers(std::vector<std::uint8_t>& out, const int width, const int height, const int count) {
	for (int i = 0; i < count; ++i) {
		if (!append_rgba8_fallback_layer(out, width, height)) {
			return false;
		}
	}
	return true;
}

inline std::optional<int> checked_tile_span(const int tile_size, const int tile_count) {
	if (tile_size <= 0 || tile_count <= 0 || tile_size > std::numeric_limits<int>::max() / tile_count) {
		return std::nullopt;
	}
	return tile_size * tile_count;
}

inline bool texture_dimension_covers_tiles(const int dimension, const int tile_size, const int tile_count) {
	const auto span = checked_tile_span(tile_size, tile_count);
	return span && dimension >= *span;
}

template <typename TextureT>
bool texture_has_rgba_source(const TextureT& texture) {
	if (texture.width <= 0 || texture.height <= 0 || texture.channels < 3 || texture.data.empty()) {
		return false;
	}
	const auto expected_size = checked_frame_byte_count(texture.width, texture.height, texture.channels);
	return expected_size && texture.data.size() >= *expected_size;
}

template <typename TerrainT>
std::uint64_t terrain_texture_revision(const TerrainT& terrain) {
	if constexpr (requires { terrain.texture_revision; }) {
		return terrain.texture_revision;
	} else if constexpr (requires { terrain.texture_revision_value(); }) {
		return terrain.texture_revision_value();
	} else {
		return 0;
	}
}

} // namespace detail

template <typename TerrainT>
TerrainTextureCpuFrame make_terrain_texture_cpu_frame(const TerrainT& terrain) {
	TerrainTextureCpuFrame frame;
	frame.revision = detail::terrain_texture_revision(terrain);

	const auto& ground_textures = terrain.ground_texture_resources();
	for (const auto& texture : ground_textures) {
		if (texture && texture->tile_size > 0 && detail::texture_has_rgba_source(*texture)
			&& detail::texture_dimension_covers_tiles(texture->width, texture->tile_size, texture->extended ? 8 : 4)
			&& detail::texture_dimension_covers_tiles(texture->height, texture->tile_size, 4)) {
			frame.ground_tile_size = texture->tile_size;
			break;
		}
	}
	if (frame.ground_tile_size == 0) {
		for (const auto& texture : ground_textures) {
			if (texture && texture->tile_size > 0) {
				frame.ground_tile_size = texture->tile_size;
				break;
			}
		}
	}
	if (frame.ground_tile_size == 0 && !ground_textures.empty()) {
		frame.ground_tile_size = 1;
	}
	for (const auto& texture : ground_textures) {
		frame.ground_texture_layer_offsets.push_back(static_cast<std::uint32_t>(frame.ground_layer_count));
		const bool extended = texture && texture->extended;
		const bool valid = texture && texture->tile_size == frame.ground_tile_size && detail::texture_has_rgba_source(*texture)
			&& detail::texture_dimension_covers_tiles(texture->width, frame.ground_tile_size, extended ? 8 : 4)
			&& detail::texture_dimension_covers_tiles(texture->height, frame.ground_tile_size, 4);
		if (!valid) {
			const int fallback_layer_count = extended ? 32 : 16;
			if (detail::append_rgba8_fallback_layers(
					frame.ground_rgba8,
					frame.ground_tile_size,
					frame.ground_tile_size,
					fallback_layer_count
				)) {
				frame.ground_layer_count += fallback_layer_count;
			}
			continue;
		}

		for (int tile_y = 0; tile_y < 4; ++tile_y) {
			for (int tile_x = 0; tile_x < 4; ++tile_x) {
				if (detail::append_rgba8_region(
					frame.ground_rgba8,
					*texture,
					tile_x * frame.ground_tile_size,
					tile_y * frame.ground_tile_size,
					frame.ground_tile_size,
					frame.ground_tile_size
				)) {
					++frame.ground_layer_count;
				}
			}
		}

		if (texture->extended) {
			for (int tile_y = 0; tile_y < 4; ++tile_y) {
				for (int tile_x = 0; tile_x < 4; ++tile_x) {
					if (detail::append_rgba8_region(
						frame.ground_rgba8,
						*texture,
						(tile_x + 4) * frame.ground_tile_size,
						tile_y * frame.ground_tile_size,
						frame.ground_tile_size,
						frame.ground_tile_size
					)) {
						++frame.ground_layer_count;
					}
				}
			}
		}
	}

	for (const auto& texture : terrain.water_textures) {
		if (texture && detail::texture_has_rgba_source(*texture)) {
			frame.water_width = texture->width;
			frame.water_height = texture->height;
			break;
		}
	}
	if (frame.water_width == 0 && frame.water_height == 0 && !terrain.water_textures.empty()) {
		frame.water_width = 1;
		frame.water_height = 1;
	}
	for (const auto& texture : terrain.water_textures) {
		const bool valid = texture && detail::texture_has_rgba_source(*texture) && frame.water_width == texture->width
			&& frame.water_height == texture->height;
		const bool appended = valid ? detail::append_rgba8_region(frame.water_rgba8, *texture, 0, 0, frame.water_width, frame.water_height)
									: detail::append_rgba8_fallback_layer(frame.water_rgba8, frame.water_width, frame.water_height);
		if (appended) {
			++frame.water_layer_count;
		}
	}

	for (const auto& texture : terrain.cliff_textures) {
		if (texture && detail::texture_has_rgba_source(*texture)) {
			frame.cliff_width = std::max(frame.cliff_width, texture->width);
			frame.cliff_height = std::max(frame.cliff_height, texture->height);
		}
	}
	if (frame.cliff_width == 0 && frame.cliff_height == 0 && !terrain.cliff_textures.empty()) {
		frame.cliff_width = 1;
		frame.cliff_height = 1;
	}
	if (frame.cliff_width > 0 && frame.cliff_height > 0) {
		for (const auto& texture : terrain.cliff_textures) {
			const bool appended = texture && detail::texture_has_rgba_source(*texture)
				? detail::append_rgba8_padded_layer(frame.cliff_rgba8, *texture, frame.cliff_width, frame.cliff_height)
				: detail::append_rgba8_fallback_layer(frame.cliff_rgba8, frame.cliff_width, frame.cliff_height);
			if (appended) {
				++frame.cliff_layer_count;
			}
		}
	}

	return frame;
}

template <typename PathingMapT>
PathingCpuFrame make_pathing_cpu_frame(const PathingMapT& pathing_map) {
	return PathingCpuFrame {
		.source_key = &pathing_map,
		.width = pathing_map.width,
		.height = pathing_map.height,
		.static_revision = pathing_map.static_revision(),
		.dynamic_revision = pathing_map.dynamic_revision(),
		.cells_static = pathing_map.pathing_cells_static,
		.cells_dynamic = pathing_map.pathing_cells_dynamic,
	};
}

} // namespace hive::metal
