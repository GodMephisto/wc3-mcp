---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/rendering/metal_particle_emitter1_model_frame.h"
ref: HEAD
---

#pragma once

#include <algorithm>
#include <cmath>
#include <cctype>
#include <cstdint>
#include <numbers>
#include <string>
#include <string_view>
#include <vector>

#include <glm/gtc/quaternion.hpp>

#include "rendering/metal_map_renderer.h"
#include "utilities/particle_emitter1_orientation.h"

namespace hive::metal::detail {

inline constexpr int max_particle_emitter1_model_spawn_depth = 3;
inline constexpr float particle_emitter1_model_simulation_step = 1.0f / 30.0f;

struct ParticleEmitter1ModelPool {
	std::vector<glm::vec3> positions;
	std::vector<glm::vec3> velocities;
	std::vector<glm::mat3> orientations;
	std::vector<float> ages;
	std::vector<std::uint32_t> key_frames;
	std::vector<std::uint32_t> particle_ids;
	std::size_t alive_count = 0;
	float emission_accumulator = 0.0f;
	std::uint32_t rng_state = 1u;
	std::uint32_t next_particle_id = 1u;
};

struct ParticleEmitter1ModelEmitterState {
	std::vector<ParticleEmitter1ModelPool> pools;
};

struct ParticleEmitter1ModelRibbonPool {
	std::vector<glm::vec3> top_positions;
	std::vector<glm::vec3> bottom_positions;
	std::vector<glm::vec3> top_velocities;
	std::vector<glm::vec3> bottom_velocities;
	std::vector<glm::vec3> colors;
	std::vector<float> alphas;
	std::vector<float> ages;
	std::vector<std::uint32_t> texture_slots;
	std::size_t alive_count = 0;
	float emission_accumulator = 0.0f;
};

struct ParticleEmitter1ModelRibbonEmitterState {
	std::vector<ParticleEmitter1ModelRibbonPool> pools;
};

struct ParticleEmitter1ModelSkeleton {
	int sequence_index = -1;
	int current_frame = 0;
	int sequence_start = 0;
	int sequence_end = 0;
	int particle_frame_ms = 0;
	const std::vector<std::uint32_t>* global_sequences = nullptr;
	glm::mat4 matrix = glm::mat4(1.0f);
	std::vector<glm::mat4> world_matrices;
	ParticleEmitter1ModelEmitterState particle_emitters1;
	ParticleEmitter1ModelEmitterState particles;
	ParticleEmitter1ModelRibbonEmitterState ribbon_emitters;

	template <typename LayerT>
	float get_layer_visiblity(const LayerT& layer) const {
		return sample_track(layer.KMTA, layer.alpha);
	}

	template <typename GeosetAnimationT>
	glm::vec3 get_geoset_animation_color(const GeosetAnimationT& animation) const {
		const auto animation_color = glm::vec3(animation.color.b, animation.color.g, animation.color.r);
		const auto interpolated_color = sample_track(animation.KGAC, animation_color);
		return glm::vec3(interpolated_color.b, interpolated_color.g, interpolated_color.r);
	}

	template <typename GeosetAnimationT>
	float get_geoset_animation_visiblity(const GeosetAnimationT& animation) const {
		return sample_track(animation.KGAO, animation.alpha);
	}

	template <typename TrackHeaderT, typename ValueT>
	ValueT sample_track(const TrackHeaderT& header, const ValueT& default_value) const;
};

inline float particle_emitter1_hermite(const float a, const float a_out_tan, const float b_in_tan, const float b, const float t) {
	const float tt = t * t;
	return (a * (tt * (2.0f * t - 3.0f) + 1.0f))
		+ (a_out_tan * (tt * (t - 2.0f) + t))
		+ (b_in_tan * (tt * (t - 1.0f)))
		+ (b * (tt * (3.0f - 2.0f * t)));
}

inline float particle_emitter1_bezier(const float a, const float a_out_tan, const float b_in_tan, const float b, const float t) {
	const float inv_t = 1.0f - t;
	const float tt = t * t;
	const float inv_tt = inv_t * inv_t;
	return (a * inv_tt * inv_t) + (a_out_tan * 3.0f * t * inv_tt) + (b_in_tan * 3.0f * tt * inv_t) + (b * tt * t);
}

inline float particle_emitter1_interpolate_value(
	const float start,
	const float out_tan,
	const float in_tan,
	const float end,
	const float t,
	const int interpolation_type
) {
	switch (interpolation_type) {
		case 1:
			return glm::mix(start, end, t);
		case 2:
			return particle_emitter1_hermite(start, out_tan, in_tan, end, t);
		case 3:
			return particle_emitter1_bezier(start, out_tan, in_tan, end, t);
		default:
			return start;
	}
}

inline glm::vec3 particle_emitter1_interpolate_value(
	const glm::vec3& start,
	const glm::vec3& out_tan,
	const glm::vec3& in_tan,
	const glm::vec3& end,
	const float t,
	const int interpolation_type
) {
	switch (interpolation_type) {
		case 1:
			return glm::mix(start, end, t);
		case 2:
			return glm::vec3(
				particle_emitter1_hermite(start.x, out_tan.x, in_tan.x, end.x, t),
				particle_emitter1_hermite(start.y, out_tan.y, in_tan.y, end.y, t),
				particle_emitter1_hermite(start.z, out_tan.z, in_tan.z, end.z, t)
			);
		case 3:
			return glm::vec3(
				particle_emitter1_bezier(start.x, out_tan.x, in_tan.x, end.x, t),
				particle_emitter1_bezier(start.y, out_tan.y, in_tan.y, end.y, t),
				particle_emitter1_bezier(start.z, out_tan.z, in_tan.z, end.z, t)
			);
		default:
			return start;
	}
}

inline glm::quat particle_emitter1_interpolate_value(
	const glm::quat& start,
	const glm::quat& out_tan,
	const glm::quat& in_tan,
	const glm::quat& end,
	const float t,
	const int interpolation_type
) {
	switch (interpolation_type) {
		case 1:
			return glm::slerp(start, end, t);
		case 2:
		case 3: {
			const glm::quat linear = glm::slerp(start, end, t);
			const glm::quat tangent = glm::slerp(out_tan, in_tan, t);
			return glm::slerp(linear, tangent, 2.0f * t * (1.0f - t));
		}
		default:
			return start;
	}
}

template <typename ValueT>
ValueT particle_emitter1_interpolate_value(
	const ValueT& start,
	const ValueT&,
	const ValueT&,
	const ValueT&,
	float,
	int
) {
	return start;
}

template <typename TrackHeaderT, typename ValueT>
ValueT ParticleEmitter1ModelSkeleton::sample_track(const TrackHeaderT& header, const ValueT& default_value) const {
	if (header.tracks.empty() || sequence_index < 0) {
		return default_value;
	}

	int local_current_frame = current_frame;
	int local_sequence_start = sequence_start;
	int local_sequence_end = sequence_end;
	if (header.global_sequence_ID >= 0 && global_sequences != nullptr
		&& static_cast<std::size_t>(header.global_sequence_ID) < global_sequences->size()) {
		local_sequence_start = 0;
		local_sequence_end = static_cast<int>((*global_sequences)[header.global_sequence_ID]);
		local_current_frame = local_sequence_end == 0 ? 0 : particle_frame_ms % local_sequence_end;
	}

	int start = -1;
	int end = 0;
	for (int i = 0; i < static_cast<int>(header.tracks.size()); ++i) {
		const auto& track = header.tracks[i];
		if (track.frame > local_sequence_end) {
			break;
		}
		if (track.frame >= local_sequence_start && start == -1) {
			start = i;
		}
		end = i;
	}
	if (start == -1) {
		return default_value;
	}
	if (start == end) {
		return header.tracks[start].value;
	}

	int left = start;
	int right = start + 1;
	while (right <= end && header.tracks[right].frame < local_current_frame) {
		left = right;
		++right;
		if (right <= end && header.tracks[right].frame == local_current_frame) {
			left = right;
		}
	}

	const bool past_end = header.tracks[end].frame < local_current_frame;
	const bool before_start = header.tracks[start].frame > local_current_frame;
	if (past_end || before_start || right > end) {
		left = end;
		right = start;
	}

	int floor_time = header.tracks[left].frame;
	const int ceil_time = header.tracks[right].frame;
	const ValueT floor_value = header.tracks[left].value;
	const ValueT ceil_value = header.tracks[right].value;
	const ValueT floor_out_tan = header.tracks[left].outTan;
	const ValueT ceil_in_tan = header.tracks[right].inTan;

	int time_between_frames = ceil_time - floor_time;
	if (time_between_frames < 0) {
		time_between_frames += (local_sequence_end - local_sequence_start);
		if (local_current_frame < floor_time) {
			floor_time = ceil_time;
		}
	}
	const float t = time_between_frames == 0 ? 0.0f : ((local_current_frame - floor_time) / static_cast<float>(time_between_frames));
	return particle_emitter1_interpolate_value(
		floor_value,
		floor_out_tan,
		ceil_in_tan,
		ceil_value,
		t,
		static_cast<int>(header.interpolation_type)
	);
}

template <typename ModelT, typename FuncT>
void for_each_particle_emitter1_model_node(const ModelT& model, const FuncT& func) {
	for (const auto& node : model.bones) {
		func(node.node);
	}
	for (const auto& node : model.lights) {
		func(node.node);
	}
	for (const auto& node : model.help_bones) {
		func(node);
	}
	for (const auto& node : model.attachments) {
		func(node.node);
	}
	for (const auto& node : model.emitters1) {
		func(node.node);
	}
	for (const auto& node : model.emitters2) {
		func(node.node);
	}
	for (const auto& node : model.ribbons) {
		func(node.node);
	}
	for (const auto& node : model.event_objects) {
		func(node.node);
	}
	for (const auto& node : model.collision_shapes) {
		func(node.node);
	}
	for (const auto& node : model.corn_emitters) {
		func(node.node);
	}
}

inline glm::mat4 particle_emitter1_model_node_matrix(
	const glm::quat& rotation,
	const glm::vec3& translation,
	const glm::vec3& scale,
	const glm::vec3& pivot
) {
	glm::mat4 out(1.0f);
	const float x = rotation.x;
	const float y = rotation.y;
	const float z = rotation.z;
	const float w = rotation.w;
	const float x2 = x + x;
	const float y2 = y + y;
	const float z2 = z + z;
	const float xx = x * x2;
	const float xy = x * y2;
	const float xz = x * z2;
	const float yy = y * y2;
	const float yz = y * z2;
	const float zz = z * z2;
	const float wx = w * x2;
	const float wy = w * y2;
	const float wz = w * z2;

	out[0][0] = (1.0f - (yy + zz)) * scale.x;
	out[0][1] = (xy + wz) * scale.y;
	out[0][2] = (xz - wy) * scale.z;
	out[1][0] = (xy - wz) * scale.x;
	out[1][1] = (1.0f - (xx + zz)) * scale.y;
	out[1][2] = (yz + wx) * scale.z;
	out[2][0] = (xz + wy) * scale.x;
	out[2][1] = (yz - wx) * scale.y;
	out[2][2] = (1.0f - (xx + yy)) * scale.z;
	out[3][0] = translation.x + pivot.x - (out[0][0] * pivot.x + out[1][0] * pivot.y + out[2][0] * pivot.z);
	out[3][1] = translation.y + pivot.y - (out[0][1] * pivot.x + out[1][1] * pivot.y + out[2][1] * pivot.z);
	out[3][2] = translation.z + pivot.z - (out[0][2] * pivot.x + out[1][2] * pivot.y + out[2][2] * pivot.z);
	return out;
}

inline std::string particle_emitter1_lower_sequence_name(std::string name) {
	std::ranges::transform(name, name.begin(), [](unsigned char c) { return std::tolower(c); });
	return name;
}

template <typename ModelT>
int particle_emitter1_find_sequence_index(const ModelT& model, const std::string_view token) {
	for (std::size_t i = 0; i < model.sequences.size(); ++i) {
		if (particle_emitter1_lower_sequence_name(model.sequences[i].name).contains(token)) {
			return static_cast<int>(i);
		}
	}
	return -1;
}

template <typename SequenceT>
float particle_emitter1_sequence_duration_seconds(const SequenceT& sequence) {
	if (sequence.end_frame <= sequence.start_frame) {
		return 0.0f;
	}
	return static_cast<float>(sequence.end_frame - sequence.start_frame) / 1000.0f;
}

struct ParticleEmitter1ModelSequenceSelection {
	int index = -1;
	float local_age_seconds = 0.0f;
};

template <typename ModelT>
ParticleEmitter1ModelSequenceSelection particle_emitter1_model_sequence_selection(
	const ModelT& model,
	const float particle_age_seconds
) {
	if (model.sequences.empty()) {
		return {};
	}

	const int birth_index = particle_emitter1_find_sequence_index(model, "birth");
	const int stand_index = particle_emitter1_find_sequence_index(model, "stand");
	if (birth_index >= 0) {
		const float birth_duration = particle_emitter1_sequence_duration_seconds(model.sequences[birth_index]);
		if (stand_index >= 0 && birth_duration > 0.0f && particle_age_seconds >= birth_duration) {
			return {
				.index = stand_index,
				.local_age_seconds = particle_age_seconds - birth_duration,
			};
		}
		return {
			.index = birth_index,
			.local_age_seconds = particle_age_seconds,
		};
	}

	if (stand_index >= 0) {
		return {
			.index = stand_index,
			.local_age_seconds = particle_age_seconds,
		};
	}

	return {
		.index = 0,
		.local_age_seconds = particle_age_seconds,
	};
}

template <typename EmitterT>
std::size_t particle_emitter1_model_pool_capacity(const EmitterT& emitter) {
	float peak_rate = emitter.emission_rate;
	for (const auto& track : emitter.KPEE.tracks) {
		peak_rate = std::max(peak_rate, track.value);
	}

	float peak_life_span = emitter.life_span;
	for (const auto& track : emitter.KPEL.tracks) {
		peak_life_span = std::max(peak_life_span, track.value);
	}

	const float capacity = std::ceil(1.15f * peak_rate * peak_life_span);
	return capacity > 0.0f ? static_cast<std::size_t>(capacity) : 0u;
}

template <typename PoolT>
void particle_emitter1_model_reset_pool(PoolT& pool, const std::size_t capacity, const std::uint32_t seed) {
	pool.positions.assign(capacity, glm::vec3(0.0f));
	pool.velocities.assign(capacity, glm::vec3(0.0f));
	if constexpr (requires { pool.orientations; }) {
		pool.orientations.assign(capacity, glm::mat3(1.0f));
	}
	pool.ages.assign(capacity, 0.0f);
	pool.key_frames.assign(capacity, 0u);
	pool.particle_ids.assign(capacity, 0u);
	pool.alive_count = 0;
	pool.emission_accumulator = 0.0f;
	pool.rng_state = seed == 0u ? 1u : seed;
	pool.next_particle_id = 1u;
}

struct ParticleEmitter1ModelFrameParams {
	float emission_rate = 0.0f;
	float gravity = 0.0f;
	float longitude = 0.0f;
	float latitude = 0.0f;
	float life_span = 0.0f;
	float speed = 0.0f;
	float visibility = 1.0f;
	glm::mat4 world_matrix = glm::mat4(1.0f);
};

template <typename ModelT>
std::size_t particle_emitter1_model_node_count(const ModelT& model) {
	std::size_t node_count = 0;
	for_each_particle_emitter1_model_node(model, [&](const auto& node) {
		if (node.id >= 0) {
			node_count = std::max(node_count, static_cast<std::size_t>(node.id + 1));
		}
	});
	return node_count;
}

template <typename ModelT>
void particle_emitter1_model_configure_sequence_frame(
	ParticleEmitter1ModelSkeleton& skeleton,
	const ModelT& model,
	const int sequence_index,
	const float local_age_seconds,
	const float total_age_seconds
) {
	skeleton.sequence_index = sequence_index;
	if (skeleton.sequence_index < 0 || static_cast<std::size_t>(skeleton.sequence_index) >= model.sequences.size()) {
		return;
	}

	const int total_elapsed = std::max(0, static_cast<int>(std::round(total_age_seconds * 1000.0f)));
	const int sequence_elapsed = std::max(0, static_cast<int>(std::round(local_age_seconds * 1000.0f)));
	const auto& sequence = model.sequences[skeleton.sequence_index];
	skeleton.sequence_start = static_cast<int>(sequence.start_frame);
	skeleton.sequence_end = static_cast<int>(sequence.end_frame);
	const int duration = std::max(0, skeleton.sequence_end - skeleton.sequence_start);
	const int offset = duration == 0 ? 0 : ((sequence.flags & 1u) != 0u ? std::min(sequence_elapsed, duration) : sequence_elapsed % duration);
	skeleton.current_frame = skeleton.sequence_start + offset;
	skeleton.particle_frame_ms = total_elapsed;
	skeleton.global_sequences = &model.global_sequences;
}

template <typename ModelT>
glm::vec3 particle_emitter1_model_pivot_or_origin(const ModelT& model, const std::size_t node_id) {
	return node_id < model.pivots.size() ? model.pivots[node_id] : glm::vec3(0.0f);
}

template <typename ModelT>
void particle_emitter1_model_update_nodes(ParticleEmitter1ModelSkeleton& skeleton, const ModelT& model) {
	std::vector<std::uint8_t> node_state(skeleton.world_matrices.size(), 0u);
	auto update_node = [&](auto&& self, const auto& node) -> void {
		if (node.id < 0 || static_cast<std::size_t>(node.id) >= skeleton.world_matrices.size()) {
			return;
		}

		const std::size_t node_id = static_cast<std::size_t>(node.id);
		if (node_state[node_id] == 2u) {
			return;
		}
		if (node_state[node_id] == 1u) {
			return;
		}
		node_state[node_id] = 1u;

		const bool has_parent = node.parent_id >= 0
			&& static_cast<std::size_t>(node.parent_id) < skeleton.world_matrices.size()
			&& static_cast<std::size_t>(node.parent_id) != node_id;
		if (has_parent) {
			for_each_particle_emitter1_model_node(model, [&](const auto& candidate) {
				if (candidate.id == node.parent_id) {
					self(self, candidate);
				}
			});
		}

		const glm::vec3 pivot = particle_emitter1_model_pivot_or_origin(model, node_id);
		const glm::vec3 translation = skeleton.sample_track(node.KGTR, glm::vec3(0.0f));
		const glm::quat rotation = skeleton.sample_track(node.KGRT, glm::quat(1.0f, 0.0f, 0.0f, 0.0f));
		const glm::vec3 scale = skeleton.sample_track(node.KGSC, glm::vec3(1.0f));
		glm::mat4 node_matrix = particle_emitter1_model_node_matrix(rotation, translation, scale, pivot);
		if (has_parent) {
			node_matrix = skeleton.world_matrices[node.parent_id] * node_matrix;
		}
		skeleton.world_matrices[node_id] = node_matrix;
		node_state[node_id] = 2u;
	};
	for_each_particle_emitter1_model_node(model, [&](const auto& node) {
		update_node(update_node, node);
	});
}

template <typename PoolT>
void particle_emitter1_model_update_pool(PoolT& pool, const float delta, const ParticleEmitter1ModelFrameParams& params) {
	const std::size_t capacity = pool.positions.size();
	if (capacity == 0 || params.life_span <= 0.0f) {
		return;
	}

	const float dt = std::clamp(delta, 0.0f, 0.5f);
	const float az = -params.gravity;
	const float half_az_dt2 = 0.5f * az * dt * dt;

	std::size_t i = 0;
	while (i < pool.alive_count) {
		pool.ages[i] += dt;
		if (pool.ages[i] >= params.life_span) {
			const std::size_t last = --pool.alive_count;
			if (i != last) {
				pool.positions[i] = pool.positions[last];
				pool.velocities[i] = pool.velocities[last];
				if constexpr (requires { pool.orientations; }) {
					pool.orientations[i] = pool.orientations[last];
				}
				pool.ages[i] = pool.ages[last];
				pool.key_frames[i] = pool.key_frames[last];
				pool.particle_ids[i] = pool.particle_ids[last];
			}
			continue;
		}

		pool.positions[i].x += pool.velocities[i].x * dt;
		pool.positions[i].y += pool.velocities[i].y * dt;
		pool.positions[i].z += pool.velocities[i].z * dt + half_az_dt2;
		pool.velocities[i].z += az * dt;
		++i;
	}

	const float visible_rate = params.emission_rate * params.visibility;
	if (visible_rate <= 0.0f) {
		return;
	}

	auto rand_bits = [](PoolT& p) -> std::uint32_t {
		std::uint32_t x = p.rng_state;
		x ^= x << 13;
		x ^= x >> 17;
		x ^= x << 5;
		if (x == 0u) {
			x = 1u;
		}
		p.rng_state = x;
		return x;
	};
	auto rand01 = [&](PoolT& p) -> float {
		return static_cast<float>(rand_bits(p)) * (1.0f / 4294967296.0f);
	};
	auto rand_signed = [&](PoolT& p) -> float {
		return rand01(p) * 2.0f - 1.0f;
	};

	auto spawn = [&]() {
		if (pool.alive_count >= capacity) {
			return;
		}

		const std::size_t slot = pool.alive_count++;
		const float latitude = glm::radians(params.latitude) * rand_signed(pool);
		const float longitude = glm::radians(params.longitude) * rand_signed(pool);
		float vx = params.speed * std::sin(latitude);
		const float vz = params.speed * std::cos(latitude);
		const float vy = vx * std::sin(longitude);
		vx = vx * std::cos(longitude);
		const glm::vec3 local_velocity(vx, vy, vz);
		const glm::vec3 center = glm::vec3(params.world_matrix * glm::vec4(0.0f, 0.0f, 0.0f, 1.0f));

		pool.positions[slot] = center;
		pool.velocities[slot] = glm::mat3(params.world_matrix) * local_velocity;
		if constexpr (requires { pool.orientations; }) {
			pool.orientations[slot] = hive::particle_emitter1_spawn_orientation(params.world_matrix);
		}
		pool.ages[slot] = 0.0f;
		pool.key_frames[slot] = 0u;
		pool.particle_ids[slot] = pool.next_particle_id++;
	};

	pool.emission_accumulator += dt * visible_rate;
	while (pool.emission_accumulator >= 1.0f && pool.alive_count < capacity) {
		spawn();
		pool.emission_accumulator -= 1.0f;
	}
}

template <typename EmitterT>
std::size_t particle_emitter2_model_pool_capacity(const EmitterT& emitter) {
	float peak_rate = emitter.emission_rate;
	for (const auto& track : emitter.KP2E.tracks) {
		peak_rate = std::max(peak_rate, track.value);
	}

	const float capacity = std::ceil(1.15f * peak_rate * emitter.life_span);
	return capacity > 0.0f ? static_cast<std::size_t>(capacity) : 0u;
}

struct ParticleEmitter2ModelFrameParams {
	float emission_rate = 0.0f;
	float speed = 0.0f;
	float variation = 0.0f;
	float latitude = 0.0f;
	float gravity = 0.0f;
	float width = 0.0f;
	float length = 0.0f;
	float visibility = 1.0f;
	glm::mat4 world_matrix = glm::mat4(1.0f);
	bool sequence_just_wrapped = false;
};

template <typename PoolT, typename EmitterT>
void particle_emitter2_model_update_pool(
	PoolT& pool,
	const float delta,
	const EmitterT& emitter,
	const ParticleEmitter2ModelFrameParams& params
) {
	const std::size_t capacity = pool.positions.size();
	if (capacity == 0 || emitter.life_span <= 0.0f) {
		return;
	}

	if (params.sequence_just_wrapped) {
		pool.alive_count = 0;
		pool.emission_accumulator = 0.0f;
		pool.next_particle_id = 1u;
	}

	const float dt = std::clamp(delta, 0.0f, 0.5f);
	const float life_span = emitter.life_span;
	const float keyframe_end[2] = {emitter.time_middle * life_span, life_span};
	const std::uint32_t flags = static_cast<std::uint32_t>(emitter.node.flags);
	const bool model_space = (flags & particle_flag_model_space) != 0u;
	const bool line_emitter = (flags & particle_flag_line_emitter) != 0u;

	auto rand_bits = [](PoolT& p) -> std::uint32_t {
		std::uint32_t x = p.rng_state;
		x ^= x << 13;
		x ^= x >> 17;
		x ^= x << 5;
		if (x == 0u) {
			x = 1u;
		}
		p.rng_state = x;
		return x;
	};
	auto rand01 = [&](PoolT& p) -> float {
		return static_cast<float>(rand_bits(p)) * (1.0f / 4294967296.0f);
	};
	auto rand_signed = [&](PoolT& p) -> float {
		return rand01(p) * 2.0f - 1.0f;
	};

	auto spawn = [&](const float elapsed) {
		if (pool.alive_count >= capacity) {
			return;
		}

		const std::size_t slot = pool.alive_count++;
		const float age = elapsed * rand01(pool);
		const glm::vec3 local_position(params.width * 0.5f * rand_signed(pool), params.length * 0.5f * rand_signed(pool), 0.0f);
		const float rot_y = params.latitude * rand_signed(pool);
		const float rot_z = line_emitter ? 0.0f : std::numbers::pi_v<float> * rand_signed(pool);
		const float speed_jittered = params.speed * (1.0f + params.variation * rand_signed(pool));
		float vx = speed_jittered * std::sin(rot_y);
		const float vz = speed_jittered * std::cos(rot_y);
		const float vy = vx * std::sin(rot_z);
		vx = vx * std::cos(rot_z);
		const glm::vec3 local_velocity(vx, vy, vz);

		if (model_space) {
			pool.positions[slot] = local_position;
			pool.velocities[slot] = local_velocity;
		} else {
			pool.positions[slot] = glm::vec3(params.world_matrix * glm::vec4(local_position, 1.0f));
			pool.velocities[slot] = glm::mat3(params.world_matrix) * local_velocity;
		}
		pool.ages[slot] = age;
		pool.key_frames[slot] = 0u;
		pool.particle_ids[slot] = pool.next_particle_id++;
	};

	if (emitter.squirt != 0u && params.sequence_just_wrapped && params.visibility > 0.0f) {
		int burst = static_cast<int>(params.emission_rate);
		while (burst-- > 0 && pool.alive_count < capacity) {
			spawn(0.0f);
		}
	}

	pool.emission_accumulator += dt * params.emission_rate * params.visibility;
	while (pool.emission_accumulator >= 1.0f && pool.alive_count < capacity) {
		spawn(dt);
		pool.emission_accumulator -= 1.0f;
	}

	const float az = -params.gravity;
	const float half_az_dt2 = 0.5f * az * dt * dt;
	std::size_t i = 0;
	while (i < pool.alive_count) {
		pool.ages[i] += dt;
		while (pool.key_frames[i] < 2u && pool.ages[i] > keyframe_end[pool.key_frames[i]]) {
			++pool.key_frames[i];
		}

		if (pool.ages[i] >= life_span || pool.key_frames[i] >= 2u) {
			const std::size_t last = --pool.alive_count;
			if (i != last) {
				pool.positions[i] = pool.positions[last];
				pool.velocities[i] = pool.velocities[last];
				pool.ages[i] = pool.ages[last];
				pool.key_frames[i] = pool.key_frames[last];
				pool.particle_ids[i] = pool.particle_ids[last];
			}
			continue;
		}

		pool.positions[i].x += pool.velocities[i].x * dt;
		pool.positions[i].y += pool.velocities[i].y * dt;
		pool.positions[i].z += pool.velocities[i].z * dt + half_az_dt2;
		pool.velocities[i].z += az * dt;
		++i;
	}
}

template <typename EmitterT>
std::size_t ribbon_emitter_model_pool_capacity(const EmitterT& emitter) {
	const float capacity = std::ceil(1.15f * static_cast<float>(emitter.emission_rate) * emitter.life_span) + 2.0f;
	return capacity > 0.0f ? static_cast<std::size_t>(capacity) : 0u;
}

inline void particle_emitter1_model_reset_ribbon_pool(
	ParticleEmitter1ModelRibbonPool& pool,
	const std::size_t capacity
) {
	pool.top_positions.assign(capacity, glm::vec3(0.0f));
	pool.bottom_positions.assign(capacity, glm::vec3(0.0f));
	pool.top_velocities.assign(capacity, glm::vec3(0.0f));
	pool.bottom_velocities.assign(capacity, glm::vec3(0.0f));
	pool.colors.assign(capacity, glm::vec3(1.0f));
	pool.alphas.assign(capacity, 1.0f);
	pool.ages.assign(capacity, 0.0f);
	pool.texture_slots.assign(capacity, 0u);
	pool.alive_count = 0;
	pool.emission_accumulator = 0.0f;
}

struct ParticleEmitter1ModelRibbonFrameParams {
	float emission_rate = 0.0f;
	float life_span = 0.0f;
	float gravity = 0.0f;
	float visibility = 1.0f;
	float height_above = 0.0f;
	float height_below = 0.0f;
	float alpha = 1.0f;
	glm::vec3 color = glm::vec3(1.0f);
	std::uint32_t texture_slot = 0u;
	glm::mat4 world_matrix = glm::mat4(1.0f);
	bool sequence_just_wrapped = false;
};

inline void particle_emitter1_model_update_ribbon_pool(
	ParticleEmitter1ModelRibbonPool& pool,
	const float delta,
	const ParticleEmitter1ModelRibbonFrameParams& params
) {
	const std::size_t capacity = pool.top_positions.size();
	if (capacity == 0 || params.life_span <= 0.0f) {
		return;
	}

	if (params.sequence_just_wrapped) {
		pool.alive_count = 0;
		pool.emission_accumulator = 0.0f;
	}

	const float dt = std::clamp(delta, 0.0f, 0.5f);
	const float az = -params.gravity;
	const float half_az_dt2 = 0.5f * az * dt * dt;

	std::size_t write_index = 0;
	for (std::size_t read_index = 0; read_index < pool.alive_count; ++read_index) {
		const float age = pool.ages[read_index] + dt;
		if (age >= params.life_span) {
			continue;
		}

		glm::vec3 top_position = pool.top_positions[read_index];
		glm::vec3 bottom_position = pool.bottom_positions[read_index];
		glm::vec3 top_velocity = pool.top_velocities[read_index];
		glm::vec3 bottom_velocity = pool.bottom_velocities[read_index];

		top_position.z += top_velocity.z * dt + half_az_dt2;
		bottom_position.z += bottom_velocity.z * dt + half_az_dt2;
		top_velocity.z += az * dt;
		bottom_velocity.z += az * dt;

		pool.top_positions[write_index] = top_position;
		pool.bottom_positions[write_index] = bottom_position;
		pool.top_velocities[write_index] = top_velocity;
		pool.bottom_velocities[write_index] = bottom_velocity;
		pool.colors[write_index] = pool.colors[read_index];
		pool.alphas[write_index] = pool.alphas[read_index];
		pool.ages[write_index] = age;
		pool.texture_slots[write_index] = pool.texture_slots[read_index];
		++write_index;
	}
	pool.alive_count = write_index;

	const float visible_rate = params.emission_rate * params.visibility;
	const bool visible = params.visibility > 0.001f && params.alpha > 0.001f && visible_rate > 0.0f;
	if (!visible) {
		return;
	}

	auto drop_oldest = [&]() {
		if (pool.alive_count == 0) {
			return;
		}
		for (std::size_t i = 1; i < pool.alive_count; ++i) {
			const std::size_t dst = i - 1;
			pool.top_positions[dst] = pool.top_positions[i];
			pool.bottom_positions[dst] = pool.bottom_positions[i];
			pool.top_velocities[dst] = pool.top_velocities[i];
			pool.bottom_velocities[dst] = pool.bottom_velocities[i];
			pool.colors[dst] = pool.colors[i];
			pool.alphas[dst] = pool.alphas[i];
			pool.ages[dst] = pool.ages[i];
			pool.texture_slots[dst] = pool.texture_slots[i];
		}
		--pool.alive_count;
	};

	auto spawn = [&]() {
		if (pool.alive_count >= capacity) {
			drop_oldest();
		}
		if (pool.alive_count >= capacity) {
			return;
		}

		glm::vec3 up = glm::mat3(params.world_matrix) * glm::vec3(0.0f, 0.0f, 1.0f);
		if (glm::dot(up, up) <= 1e-6f) {
			up = glm::vec3(0.0f, 0.0f, 1.0f);
		} else {
			up = glm::normalize(up);
		}

		const glm::vec3 center = glm::vec3(params.world_matrix * glm::vec4(0.0f, 0.0f, 0.0f, 1.0f));
		const std::size_t slot = pool.alive_count++;
		pool.top_positions[slot] = center + up * params.height_above;
		pool.bottom_positions[slot] = center - up * params.height_below;
		pool.top_velocities[slot] = glm::vec3(0.0f);
		pool.bottom_velocities[slot] = glm::vec3(0.0f);
		pool.colors[slot] = params.color;
		pool.alphas[slot] = params.alpha * params.visibility;
		pool.ages[slot] = 0.0f;
		pool.texture_slots[slot] = params.texture_slot;
	};

	const bool seeded_empty_trail = pool.alive_count == 0;
	if (seeded_empty_trail) {
		spawn();
	}
	if (seeded_empty_trail) {
		return;
	}

	pool.emission_accumulator += dt * visible_rate;
	while (pool.emission_accumulator >= 1.0f) {
		spawn();
		pool.emission_accumulator -= 1.0f;
	}
}

template <typename ModelT>
void simulate_particle_emitter1_model_emitters(
	ParticleEmitter1ModelSkeleton& skeleton,
	const ModelT& model,
	const float total_particle_age_seconds,
	const float local_particle_age_seconds
) {
	skeleton.particle_emitters1.pools.resize(model.emitters1.size());
	if (local_particle_age_seconds <= 0.0f || skeleton.sequence_index < 0) {
		return;
	}

	const int step_count = std::max(1, static_cast<int>(std::ceil(local_particle_age_seconds / particle_emitter1_model_simulation_step)));
	const float step = local_particle_age_seconds / static_cast<float>(step_count);
	const float total_age_offset = std::max(0.0f, total_particle_age_seconds - local_particle_age_seconds);

	for (std::size_t emitter_index = 0; emitter_index < model.emitters1.size(); ++emitter_index) {
		const auto& emitter = model.emitters1[emitter_index];
		const std::size_t capacity = particle_emitter1_model_pool_capacity(emitter);
		auto& pool = skeleton.particle_emitters1.pools[emitter_index];
		particle_emitter1_model_reset_pool(pool, capacity, static_cast<std::uint32_t>(emitter_index) * 0x85EBCA6Bu + 0xA5A5A5A5u);
		if (emitter.node.id < 0 || capacity == 0) {
			continue;
		}

		const std::size_t node_id = static_cast<std::size_t>(emitter.node.id);
		if (node_id >= skeleton.world_matrices.size()) {
			continue;
		}

		for (int step_index = 0; step_index < step_count; ++step_index) {
			const float local_age = step * static_cast<float>(step_index + 1);
			ParticleEmitter1ModelSkeleton step_skeleton;
			step_skeleton.matrix = skeleton.matrix;
			particle_emitter1_model_configure_sequence_frame(
				step_skeleton,
				model,
				skeleton.sequence_index,
				local_age,
				total_age_offset + local_age
			);
			step_skeleton.world_matrices.assign(skeleton.world_matrices.size(), glm::mat4(1.0f));
			particle_emitter1_model_update_nodes(step_skeleton, model);

			ParticleEmitter1ModelFrameParams params {};
			params.emission_rate = step_skeleton.sample_track(emitter.KPEE, emitter.emission_rate);
			params.gravity = step_skeleton.sample_track(emitter.KPEG, emitter.gravity);
			params.longitude = step_skeleton.sample_track(emitter.KPLN, emitter.longitude);
			params.latitude = step_skeleton.sample_track(emitter.KPLT, emitter.latitude);
			params.life_span = step_skeleton.sample_track(emitter.KPEL, emitter.life_span);
			params.speed = step_skeleton.sample_track(emitter.KPES, emitter.speed);
			params.visibility = step_skeleton.sample_track(emitter.KPEV, 1.0f);
			params.world_matrix =
				step_skeleton.world_matrices[node_id] * glm::translate(glm::mat4(1.0f), particle_emitter1_model_pivot_or_origin(model, node_id));
			particle_emitter1_model_update_pool(pool, step, params);
		}
	}
}

template <typename ModelT>
void simulate_particle_emitter2_model_emitters(
	ParticleEmitter1ModelSkeleton& skeleton,
	const ModelT& model,
	const float total_particle_age_seconds,
	const float local_particle_age_seconds
) {
	skeleton.particles.pools.resize(model.emitters2.size());
	if (local_particle_age_seconds <= 0.0f || skeleton.sequence_index < 0) {
		return;
	}

	const int step_count = std::max(1, static_cast<int>(std::ceil(local_particle_age_seconds / particle_emitter1_model_simulation_step)));
	const float step = local_particle_age_seconds / static_cast<float>(step_count);
	const float total_age_offset = std::max(0.0f, total_particle_age_seconds - local_particle_age_seconds);

	for (std::size_t emitter_index = 0; emitter_index < model.emitters2.size(); ++emitter_index) {
		const auto& emitter = model.emitters2[emitter_index];
		const std::size_t capacity = particle_emitter2_model_pool_capacity(emitter);
		auto& pool = skeleton.particles.pools[emitter_index];
		particle_emitter1_model_reset_pool(pool, capacity, static_cast<std::uint32_t>(emitter_index) * 0x9E3779B9u + 1u);
		if (emitter.node.id < 0 || capacity == 0) {
			continue;
		}

		const std::size_t node_id = static_cast<std::size_t>(emitter.node.id);
		if (node_id >= skeleton.world_matrices.size()) {
			continue;
		}

		for (int step_index = 0; step_index < step_count; ++step_index) {
			const float local_age = step * static_cast<float>(step_index + 1);
			ParticleEmitter1ModelSkeleton step_skeleton;
			step_skeleton.matrix = skeleton.matrix;
			particle_emitter1_model_configure_sequence_frame(
				step_skeleton,
				model,
				skeleton.sequence_index,
				local_age,
				total_age_offset + local_age
			);
			step_skeleton.world_matrices.assign(skeleton.world_matrices.size(), glm::mat4(1.0f));
			particle_emitter1_model_update_nodes(step_skeleton, model);

			ParticleEmitter2ModelFrameParams params {};
			params.emission_rate = step_skeleton.sample_track(emitter.KP2E, emitter.emission_rate);
			params.speed = step_skeleton.sample_track(emitter.KP2S, emitter.speed);
			params.variation = step_skeleton.sample_track(emitter.KP2R, emitter.variation);
			params.latitude = glm::radians(step_skeleton.sample_track(emitter.KP2L, emitter.latitude));
			params.gravity = step_skeleton.sample_track(emitter.KP2G, emitter.gravity);
			params.width = step_skeleton.sample_track(emitter.KP2W, emitter.width);
			params.length = step_skeleton.sample_track(emitter.KP2N, emitter.length);
			params.visibility = step_skeleton.sample_track(emitter.KP2V, 1.0f);
			params.world_matrix =
				step_skeleton.world_matrices[node_id] * glm::translate(glm::mat4(1.0f), particle_emitter1_model_pivot_or_origin(model, node_id));
			params.sequence_just_wrapped = step_index == 0;
			particle_emitter2_model_update_pool(pool, step, emitter, params);
		}
	}
}

template <typename ModelT>
void simulate_ribbon_model_emitters(
	ParticleEmitter1ModelSkeleton& skeleton,
	const ModelT& model,
	const float total_particle_age_seconds,
	const float local_particle_age_seconds
) {
	skeleton.ribbon_emitters.pools.resize(model.ribbons.size());
	if (local_particle_age_seconds <= 0.0f || skeleton.sequence_index < 0) {
		return;
	}

	const int step_count = std::max(1, static_cast<int>(std::ceil(local_particle_age_seconds / particle_emitter1_model_simulation_step)));
	const float step = local_particle_age_seconds / static_cast<float>(step_count);
	const float total_age_offset = std::max(0.0f, total_particle_age_seconds - local_particle_age_seconds);

	for (std::size_t ribbon_index = 0; ribbon_index < model.ribbons.size(); ++ribbon_index) {
		const auto& ribbon = model.ribbons[ribbon_index];
		const std::size_t capacity = ribbon_emitter_model_pool_capacity(ribbon);
		auto& pool = skeleton.ribbon_emitters.pools[ribbon_index];
		particle_emitter1_model_reset_ribbon_pool(pool, capacity);
		if (ribbon.node.id < 0 || capacity == 0) {
			continue;
		}

		const std::size_t node_id = static_cast<std::size_t>(ribbon.node.id);
		if (node_id >= skeleton.world_matrices.size()) {
			continue;
		}

		for (int step_index = 0; step_index < step_count; ++step_index) {
			const float local_age = step * static_cast<float>(step_index + 1);
			ParticleEmitter1ModelSkeleton step_skeleton;
			step_skeleton.matrix = skeleton.matrix;
			particle_emitter1_model_configure_sequence_frame(
				step_skeleton,
				model,
				skeleton.sequence_index,
				local_age,
				total_age_offset + local_age
			);
			step_skeleton.world_matrices.assign(skeleton.world_matrices.size(), glm::mat4(1.0f));
			particle_emitter1_model_update_nodes(step_skeleton, model);

			ParticleEmitter1ModelRibbonFrameParams params {};
			params.emission_rate = static_cast<float>(ribbon.emission_rate);
			params.life_span = ribbon.life_span;
			params.gravity = ribbon.gravity;
			params.visibility = step_skeleton.sample_track(ribbon.KRVS, 1.0f);
			params.height_above = step_skeleton.sample_track(ribbon.KRHA, ribbon.height_above);
			params.height_below = step_skeleton.sample_track(ribbon.KRHB, ribbon.height_below);
			params.alpha = step_skeleton.sample_track(ribbon.KRAL, ribbon.alpha);
			params.color = step_skeleton.sample_track(ribbon.KRCO, ribbon.color);
			params.texture_slot = step_skeleton.sample_track(ribbon.KRTX, ribbon.texture_slot);
			params.world_matrix =
				step_skeleton.world_matrices[node_id] * glm::translate(glm::mat4(1.0f), particle_emitter1_model_pivot_or_origin(model, node_id));
			params.sequence_just_wrapped = step_index == 0;
			particle_emitter1_model_update_ribbon_pool(pool, step, params);
		}
	}
}

template <typename MeshPtrT>
ParticleEmitter1ModelSkeleton make_particle_emitter1_model_skeleton(
	const MeshPtrT& mesh_ptr,
	const glm::mat4& matrix,
	const float particle_age_seconds
) {
	ParticleEmitter1ModelSkeleton skeleton;
	skeleton.matrix = matrix;
	if (mesh_ptr && mesh_ptr->mdx) {
		const auto& model = *mesh_ptr->mdx;
		const ParticleEmitter1ModelSequenceSelection selection =
			particle_emitter1_model_sequence_selection(model, particle_age_seconds);
		particle_emitter1_model_configure_sequence_frame(skeleton, model, selection.index, selection.local_age_seconds, particle_age_seconds);
		skeleton.world_matrices.assign(particle_emitter1_model_node_count(model), glm::mat4(1.0f));
		particle_emitter1_model_update_nodes(skeleton, model);

		simulate_particle_emitter1_model_emitters(skeleton, model, particle_age_seconds, selection.local_age_seconds);
		simulate_particle_emitter2_model_emitters(skeleton, model, particle_age_seconds, selection.local_age_seconds);
		simulate_ribbon_model_emitters(skeleton, model, particle_age_seconds, selection.local_age_seconds);
	}
	return skeleton;
}

template <typename CameraT, typename MeshPtrT, typename SkeletonT>
void append_particle_emitter1_model_instances_recursive(
	ModelCpuFrame& frame,
	std::unordered_map<const void*, std::uint32_t>& mesh_indices,
	const CameraT& camera,
	const MeshPtrT& mesh_ptr,
	const SkeletonT& skeleton,
	const glm::vec3& color,
	const std::uint32_t team_color_index,
	const bool preserve_declaration_order,
	const std::uint32_t pick_id,
	const int depth
) {
	if (depth >= max_particle_emitter1_model_spawn_depth) {
		return;
	}
	if (!mesh_ptr || !mesh_ptr->mdx || mesh_ptr->mdx->emitters1.empty()) {
		return;
	}
	if constexpr (!(requires { skeleton.particle_emitters1.pools; } && requires { mesh_ptr->emitter1_model_meshes; })) {
		return;
	} else {
		const auto& mdx = *mesh_ptr->mdx;
		const bool has_valid_sequence = skeleton.sequence_index >= 0
			&& static_cast<std::size_t>(skeleton.sequence_index) < mdx.sequences.size();
		const auto& extent = has_valid_sequence ? mdx.sequences[static_cast<std::size_t>(skeleton.sequence_index)].extent : mdx.extent;
		if (!camera.inside_frustrum_transform(extent.minimum, extent.maximum, skeleton.matrix)) {
			return;
		}

		for (std::size_t emitter_index = 0; emitter_index < mdx.emitters1.size(); ++emitter_index) {
			const auto& emitter = mdx.emitters1[emitter_index];
			const std::uint32_t flags = static_cast<std::uint32_t>(emitter.node.flags);
			if (emitter.node.id < 0 || (flags & particle_emitter1_uses_tga) != 0u || emitter_index >= skeleton.particle_emitters1.pools.size()
				|| emitter_index >= mesh_ptr->emitter1_model_meshes.size()) {
				continue;
			}
			if ((flags & particle_emitter1_uses_mdl) == 0u && emitter.path.empty()) {
				continue;
			}

			const auto& child_mesh = mesh_ptr->emitter1_model_meshes[emitter_index];
			if (!child_mesh) {
				continue;
			}

			const auto& pool = skeleton.particle_emitters1.pools[emitter_index];
			const std::size_t alive = std::min(std::min(pool.alive_count, pool.positions.size()), pool.ages.size());
			for (std::size_t particle_index = 0; particle_index < alive; ++particle_index) {
				glm::mat4 particle_transform = glm::translate(glm::mat4(1.0f), pool.positions[particle_index]);
				if constexpr (requires { pool.orientations; }) {
					if (particle_index < pool.orientations.size()) {
						glm::mat4 orientation_transform(1.0f);
						orientation_transform[0] = glm::vec4(pool.orientations[particle_index][0], 0.0f);
						orientation_transform[1] = glm::vec4(pool.orientations[particle_index][1], 0.0f);
						orientation_transform[2] = glm::vec4(pool.orientations[particle_index][2], 0.0f);
						particle_transform *= orientation_transform;
					}
				}
				const glm::mat4 child_matrix = skeleton.matrix * particle_transform;
				const float particle_age = pool.ages[particle_index];
				const ParticleEmitter1ModelSkeleton child_skeleton =
					make_particle_emitter1_model_skeleton(child_mesh, child_matrix, particle_age);
				append_model_instance(
					frame,
					mesh_indices,
					camera,
					child_mesh,
					child_skeleton,
					color,
					team_color_index,
					pick_id,
					preserve_declaration_order
				);
				append_model_bounds_picking_instance(frame, mesh_indices, camera, child_mesh, child_skeleton, team_color_index, pick_id);
				append_particle_emitter1_model_instances_recursive(
					frame,
					mesh_indices,
					camera,
					child_mesh,
					child_skeleton,
					color,
					team_color_index,
					preserve_declaration_order,
					pick_id,
					depth + 1
				);
			}
		}
	}
}

template <typename CameraT, typename MeshPtrT, typename SkeletonT>
void append_particle_emitter1_model_particle_like_emitters_recursive(
	ParticleEmitter2CpuFrame& frame,
	std::unordered_map<const void*, std::uint32_t>& texture_indices,
	const CameraT& camera,
	const MeshPtrT& mesh_ptr,
	const SkeletonT& skeleton,
	const int depth
) {
	if (depth >= max_particle_emitter1_model_spawn_depth) {
		return;
	}
	if (!mesh_ptr || !mesh_ptr->mdx || mesh_ptr->mdx->emitters1.empty()) {
		return;
	}
	if constexpr (!(requires { skeleton.particle_emitters1.pools; } && requires { mesh_ptr->emitter1_model_meshes; })) {
		return;
	} else {
		const auto& mdx = *mesh_ptr->mdx;
		const bool has_valid_sequence = skeleton.sequence_index >= 0
			&& static_cast<std::size_t>(skeleton.sequence_index) < mdx.sequences.size();
		const auto& extent = has_valid_sequence ? mdx.sequences[static_cast<std::size_t>(skeleton.sequence_index)].extent : mdx.extent;
		if (!camera.inside_frustrum_transform(extent.minimum, extent.maximum, skeleton.matrix)) {
			return;
		}

		for (std::size_t emitter_index = 0; emitter_index < mdx.emitters1.size(); ++emitter_index) {
			const auto& emitter = mdx.emitters1[emitter_index];
			const std::uint32_t flags = static_cast<std::uint32_t>(emitter.node.flags);
			if (emitter.node.id < 0 || (flags & particle_emitter1_uses_tga) != 0u || emitter_index >= skeleton.particle_emitters1.pools.size()
				|| emitter_index >= mesh_ptr->emitter1_model_meshes.size()) {
				continue;
			}
			if ((flags & particle_emitter1_uses_mdl) == 0u && emitter.path.empty()) {
				continue;
			}

			const auto& child_mesh = mesh_ptr->emitter1_model_meshes[emitter_index];
			if (!child_mesh) {
				continue;
			}

			const auto& pool = skeleton.particle_emitters1.pools[emitter_index];
			const std::size_t alive = std::min(std::min(pool.alive_count, pool.positions.size()), pool.ages.size());
			for (std::size_t particle_index = 0; particle_index < alive; ++particle_index) {
				glm::mat4 particle_transform = glm::translate(glm::mat4(1.0f), pool.positions[particle_index]);
				if constexpr (requires { pool.orientations; }) {
					if (particle_index < pool.orientations.size()) {
						glm::mat4 orientation_transform(1.0f);
						orientation_transform[0] = glm::vec4(pool.orientations[particle_index][0], 0.0f);
						orientation_transform[1] = glm::vec4(pool.orientations[particle_index][1], 0.0f);
						orientation_transform[2] = glm::vec4(pool.orientations[particle_index][2], 0.0f);
						particle_transform *= orientation_transform;
					}
				}

				const glm::mat4 child_matrix = skeleton.matrix * particle_transform;
				const float particle_age = pool.ages[particle_index];
				const ParticleEmitter1ModelSkeleton child_skeleton =
					make_particle_emitter1_model_skeleton(child_mesh, child_matrix, particle_age);
				append_particle_like_emitters(frame, texture_indices, camera, child_mesh, child_skeleton);
				append_particle_emitter1_model_particle_like_emitters_recursive(
					frame,
					texture_indices,
					camera,
					child_mesh,
					child_skeleton,
					depth + 1
				);
			}
		}
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
	const std::uint32_t team_color_index,
	const bool preserve_declaration_order,
	const std::uint32_t pick_id
) {
	append_particle_emitter1_model_instances_recursive(
		frame,
		mesh_indices,
		camera,
		mesh_ptr,
		skeleton,
		color,
		team_color_index,
		preserve_declaration_order,
		pick_id,
		0
	);
}

template <typename CameraT, typename MeshPtrT, typename SkeletonT>
void append_particle_emitter1_model_particle_like_emitters(
	ParticleEmitter2CpuFrame& frame,
	std::unordered_map<const void*, std::uint32_t>& texture_indices,
	const CameraT& camera,
	const MeshPtrT& mesh_ptr,
	const SkeletonT& skeleton
) {
	append_particle_emitter1_model_particle_like_emitters_recursive(
		frame,
		texture_indices,
		camera,
		mesh_ptr,
		skeleton,
		0
	);
}

} // namespace hive::metal::detail
