---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/file_formats/mdx/utilities.cpp"
ref: HEAD
---

module;
#include "std_module_compat.h"

#include "glm/glm.hpp"

module MDX;


namespace mdx {
	namespace {
		std::vector<glm::u8vec4> fallback_skin_weights(const std::size_t vertex_count) {
			std::vector<glm::u8vec4> skin_weights;
			skin_weights.reserve(vertex_count * 2);
			for (std::size_t i = 0; i < vertex_count; ++i) {
				skin_weights.emplace_back(0u, 0u, 0u, 0u);
				skin_weights.emplace_back(255u, 0u, 0u, 0u);
			}
			return skin_weights;
		}
	}

	/// Will not retain sequences and bones because there is no straightforward way to merge them
	void MDX::merge_with(const MDX& mdx, const glm::mat4& transform) {
		MDX new_mdx = mdx;
		for (auto& geoset : new_mdx.geosets) {
			geoset.material_id += materials.size();

			geoset.matrix_groups.clear();
			geoset.matrix_indices.clear();
			geoset.vertex_groups.clear();

			geoset.sequence_extents.resize(1);

			for (auto& vertex : geoset.vertices) {
				vertex = transform * glm::vec4(vertex, 1.f);
			}
			for (auto& normal : geoset.normals) {
				normal = glm::normalize(normal * glm::mat3(transform));
			}

			// Zero skin weights because we will have only 1 bone
			geoset.skin = std::vector<std::uint8_t>(geoset.vertices.size() * 8, 0);
			// Set the contribution of the first bone to 255
			for (std::size_t i = 0; i < geoset.skin.size(); i += 8) {
				geoset.skin[i + 4] = 255;
			}
		}

		for (auto& material : new_mdx.materials) {
			for (auto& layer : material.layers) {
				layer.texture_animation_id = 0;
				layer.texture_animation_id += texture_animations.size();
				for (auto& texture : layer.textures) {
					texture.id += textures.size();
				}
			}
		}

		// for (auto & emitter : new_mdx.emitters2) {
		// 	emitter.texture_id += textures.size();
		// }
		//
		// for (auto & ribbon : new_mdx.ribbons) {
		// 	ribbon.material_id += materials.size();
		// }

		geosets.append_range(new_mdx.geosets);
		// sequences.append_range(new_mdx.sequences);
		// global_sequences.append_range(new_mdx.global_sequences);
		animations.append_range(new_mdx.animations);
		// bones.append_range(new_mdx.bones);
		materials.append_range(new_mdx.materials);
		textures.append_range(new_mdx.textures);
		// lights.append_range(new_mdx.lights);
		// help_bones.append_range(new_mdx.help_bones);
		// attachments.append_range(new_mdx.attachments);
		// pivots.append_range(new_mdx.pivots);
		// emitters1.append_range(new_mdx.emitters1);
		// emitters2.append_range(new_mdx.emitters2);
		// ribbons.append_range(new_mdx.ribbons);
		// event_objects.append_range(new_mdx.event_objects);
		// collision_shapes.append_range(new_mdx.collision_shapes);
		// corn_emitters.append_range(new_mdx.corn_emitters);
		// facefxes.append_range(new_mdx.facefxes);
		// cameras.append_range(new_mdx.cameras);
		// bind_poses.append_range(new_mdx.bind_poses);
		// texture_animations.append_range(new_mdx.texture_animations);

		// Just to be sure
		fix_up();
	}

	/// Technically SD supports infinite bones per vertex, but we limit it to 4 like HD does.
	/// This could cause graphical inconsistencies with the game, but after more than 4 bones the contribution per bone is low enough that we don't care
	std::vector<glm::u8vec4> MDX::matrix_groups_as_skin_weights(const Geoset& geoset) {
		const std::size_t vertex_count = geoset.vertices.size();
		if (vertex_count == 0 || geoset.matrix_groups.empty()) {
			return fallback_skin_weights(vertex_count);
		}

		std::vector<glm::u8vec4> groups;
		groups.reserve(geoset.matrix_groups.size());
		std::vector<glm::u8vec4> weights;
		weights.reserve(geoset.matrix_groups.size());

		std::size_t bone_offset = 0;
		for (const auto& group_size : geoset.matrix_groups) {
			const int bone_count = std::min(group_size, 4u);
			if (bone_count <= 0 || bone_offset + static_cast<std::size_t>(bone_count) > geoset.matrix_indices.size()) {
				groups.emplace_back(0u, 0u, 0u, 0u);
				weights.emplace_back(255u, 0u, 0u, 0u);
				bone_offset = std::min(bone_offset + static_cast<std::size_t>(group_size), geoset.matrix_indices.size());
				continue;
			}

			glm::uvec4 indices(0);
			glm::uvec4 weightss(0);

			const int weight = 255 / bone_count;
			for (std::size_t j = 0; j < bone_count; j++) {
				indices[j] = geoset.matrix_indices[bone_offset + j];
				weightss[j] = weight;
			}
			weightss[0] += 255 % bone_count;

			groups.push_back(indices);
			weights.push_back(weightss);
			bone_offset += group_size;
		}

		std::vector<glm::u8vec4> skin_weights;
		skin_weights.reserve(vertex_count * 2);
		for (std::size_t vertex_index = 0; vertex_index < vertex_count; ++vertex_index) {
			const std::uint8_t vertex_group = vertex_index < geoset.vertex_groups.size() ? geoset.vertex_groups[vertex_index] : 0u;
			if (vertex_group < groups.size()) {
				skin_weights.push_back(groups[vertex_group]);
				skin_weights.push_back(weights[vertex_group]);
			} else {
				skin_weights.emplace_back(0u, 0u, 0u, 0u);
				skin_weights.emplace_back(255u, 0u, 0u, 0u);
			}
		}

		return skin_weights;
	}

	MDX& MDX::calculate_extents() {
		auto pivot_for_node = [this](const int node_id) {
			return node_id >= 0 && static_cast<std::size_t>(node_id) < pivots.size()
				? pivots[static_cast<std::size_t>(node_id)]
				: glm::vec3(0.0f);
		};

		for (auto& geoset : geosets) {
			geoset.extent.minimum = glm::vec3(std::numeric_limits<float>::max());
			geoset.extent.maximum = glm::vec3(std::numeric_limits<float>::lowest());
			for (const auto& i : geoset.vertices) {
				geoset.extent.minimum = glm::min(geoset.extent.minimum, i);
				geoset.extent.maximum = glm::max(geoset.extent.maximum, i);
			}

			geoset.extent.bounds_radius =
				std::max(glm::distance(glm::vec3(0.0), geoset.extent.minimum), glm::distance(glm::vec3(0.0), geoset.extent.maximum));

			for (auto& extent : geoset.sequence_extents) {
				// Wrong because we should capture the min/max of the entire animation but that's kind of a pain to implement
				extent = geoset.extent;
			}

			extent.minimum = glm::min(extent.minimum, geoset.extent.minimum);
			extent.maximum = glm::max(extent.maximum, geoset.extent.maximum);
			extent.bounds_radius = std::max(extent.bounds_radius, geoset.extent.bounds_radius);
		}

		for (const auto& emitter : emitters2) {
			const float life_span = std::max(0.f, emitter.life_span);
			const float max_speed = std::abs(emitter.speed) * (1.f + std::max(0.f, emitter.variation));
			const float max_travel = max_speed * life_span;

			const float max_scale = std::max({
				std::abs(emitter.segment_scaling.x),
				std::abs(emitter.segment_scaling.y),
				std::abs(emitter.segment_scaling.z),
			});

			const float gravity_offset = 0.5f * std::abs(emitter.gravity) * life_span * life_span;

			const float half_width = std::abs(emitter.width) * 0.5f;
			const float half_length = std::abs(emitter.length) * 0.5f;

			const float r = max_travel + max_scale;
			const glm::vec3 spread(r + half_width, r + half_length, r + gravity_offset);

			const glm::vec3 pivot = pivot_for_node(emitter.node.id);
			const glm::vec3 emitter_min = pivot - spread;
			const glm::vec3 emitter_max = pivot + spread;

			extent.minimum = glm::min(extent.minimum, emitter_min);
			extent.maximum = glm::max(extent.maximum, emitter_max);
			extent.bounds_radius = std::max(
				extent.bounds_radius,
				std::max(glm::length(emitter_min), glm::length(emitter_max))
			);
		}

		for (const auto& emitter : corn_emitters) {
			const float life_span = std::max(0.f, emitter.life_span);
			const float emission_rate = std::max(0.f, emitter.emission_rate);
			if (life_span <= 0.f && emission_rate <= 0.f) {
				continue;
			}

			const float radius = std::clamp(
				std::max({1.f, std::abs(emitter.speed) * 0.05f, life_span * 4.f, std::sqrt(emission_rate + 1.f)}),
				1.f,
				96.f
			);
			const glm::vec3 pivot = pivot_for_node(emitter.node.id);
			const glm::vec3 spread(radius);
			const glm::vec3 emitter_min = pivot - spread;
			const glm::vec3 emitter_max = pivot + spread;

			extent.minimum = glm::min(extent.minimum, emitter_min);
			extent.maximum = glm::max(extent.maximum, emitter_max);
			extent.bounds_radius = std::max(
				extent.bounds_radius,
				std::max(glm::length(emitter_min), glm::length(emitter_max))
			);
		}

		for (auto& sequence : sequences) {
			// Wrong because we should capture the min/max of the entire animation but that's kind of a pain to implement
			sequence.extent = extent;
		}

		return *this;
	}
} // namespace mdx
