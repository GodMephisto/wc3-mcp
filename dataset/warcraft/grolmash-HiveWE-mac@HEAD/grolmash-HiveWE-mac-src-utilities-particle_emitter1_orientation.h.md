---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/utilities/particle_emitter1_orientation.h"
ref: HEAD
---

#pragma once

#include <cmath>

#include <glm/glm.hpp>

namespace hive {

inline glm::mat3 particle_emitter1_spawn_orientation(const glm::mat4& world_matrix) {
	const glm::mat3 basis(world_matrix);
	auto normalize_or = [](const glm::vec3& value, const glm::vec3& fallback) {
		const float length_squared = glm::dot(value, value);
		return length_squared > 1.0e-6f ? value * glm::inversesqrt(length_squared) : fallback;
	};
	auto perpendicular_fallback = [](const glm::vec3& axis) {
		const glm::vec3 reference = std::abs(axis.z) < 0.9f ? glm::vec3(0.0f, 0.0f, 1.0f) : glm::vec3(0.0f, 1.0f, 0.0f);
		return glm::normalize(glm::cross(reference, axis));
	};

	const glm::vec3 x = normalize_or(basis[0], glm::vec3(1.0f, 0.0f, 0.0f));
	const glm::vec3 y_projected = basis[1] - x * glm::dot(basis[1], x);
	glm::vec3 y = normalize_or(y_projected, perpendicular_fallback(x));
	glm::vec3 z = normalize_or(glm::cross(x, y), glm::vec3(0.0f, 0.0f, 1.0f));
	if (glm::dot(z, basis[2]) < 0.0f) {
		y = -y;
		z = -z;
	}
	return glm::mat3(x, y, z);
}

} // namespace hive
