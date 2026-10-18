---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/rendering/metal_model_frame_stats.h"
ref: HEAD
---

#pragma once

#include <cstddef>

namespace hive::metal {

struct MetalModelFrameStats {
	std::size_t mesh_count = 0;
	std::size_t model_texture_count = 0;
	std::size_t opaque_sd_draw_count = 0;
	std::size_t opaque_hd_draw_count = 0;
	std::size_t transparent_sd_draw_count = 0;
	std::size_t transparent_hd_draw_count = 0;
	std::size_t alpha_tested_draw_count = 0;
	std::size_t team_color_draw_count = 0;
	std::size_t particle_draw_count = 0;
	std::size_t particle_vertex_count = 0;
	std::size_t particle_texture_count = 0;
};

struct MetalTextureFallbackStats {
	std::size_t model_texture_fallback_count = 0;
	std::size_t particle_texture_fallback_count = 0;
};

[[nodiscard]]
inline std::size_t model_draw_count(const MetalModelFrameStats& stats) {
	return stats.opaque_sd_draw_count + stats.opaque_hd_draw_count + stats.transparent_sd_draw_count + stats.transparent_hd_draw_count;
}

} // namespace hive::metal
