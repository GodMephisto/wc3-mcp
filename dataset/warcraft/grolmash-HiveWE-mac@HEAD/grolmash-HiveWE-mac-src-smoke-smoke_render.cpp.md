---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/smoke/smoke_render.cpp"
ref: HEAD
---

#include "smoke/smoke_tests.h"

#if HIVEWE_USE_METAL_RUNTIME
#include "model_editor/model_editor_metal_widget.h"
#include "object_editor/model_grid_metal_widget.h"
#include "rendering/metal_map_renderer.h"
#include "rendering/metal_particle_emitter1_model_frame.h"
#include "rendering/metal_surface.h"
#else
#include "model_editor/model_editor_glwidget.h"
#include "object_editor/model_grid_glwidget.h"
#endif

#include <QCoreApplication>
#include <QDebug>
#include <QDir>
#include <QEventLoop>
#include <QImage>
#include <QPaintEngine>
#include <QTemporaryDir>
#include <QTimer>
#include <QWidget>

#include <algorithm>
#include <array>
#include <cmath>
#include <cstdint>
#include <exception>
#include <filesystem>
#include <fstream>
#include <memory>
#include <optional>
#include <print>
#include <string>
#include <string_view>
#include <unordered_map>
#include <utility>
#include <vector>

#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/packing.hpp>

#include "std_module_compat.h"
import BinaryReader;
import BinaryWriter;
import Hierarchy;
import MDX;
import Utilities;
#if HIVEWE_USE_METAL_RUNTIME
import SkeletalModelInstance;
import SkinnedMesh;
#endif

namespace fs = std::filesystem;

namespace {
bool is_material_team_color_pixel(const std::uint8_t r, const std::uint8_t g, const std::uint8_t b) {
	return (r > 150u && g < 120u && b < 120u) || (b > 110u && r < 80u && g < 120u);
}

bool is_material_hd_green_pixel(const std::uint8_t r, const std::uint8_t g, const std::uint8_t b) {
	return g > 150u && r < 140u && b < 160u;
}

bool is_material_transparent_orange_pixel(const std::uint8_t r, const std::uint8_t g, const std::uint8_t b) {
	return r > 150u && g > 60u && g < 190u && b < 140u;
}

bool is_material_transparent_magenta_pixel(const std::uint8_t r, const std::uint8_t g, const std::uint8_t b) {
	return r > 120u && b > 100u && g < 140u;
}

bool is_material_alpha_cyan_pixel(const std::uint8_t r, const std::uint8_t g, const std::uint8_t b) {
	return g > 140u && b > 140u && r < 140u;
}

struct MaterialPreviewPixelCounts {
	std::size_t team_color = 0;
	std::size_t hd_green = 0;
	std::size_t transparent_orange = 0;
	std::size_t transparent_magenta = 0;
	std::size_t alpha_cyan = 0;
};

struct SmokePreviewPixelDiversity {
	std::size_t changed_pixels = 0;
	std::size_t significant_color_bins = 0;
	std::size_t dominant_bin_pixels = 0;
};

SmokePreviewPixelDiversity count_smoke_preview_pixel_diversity(
	const std::vector<std::uint8_t>& pixels,
	const int width,
	const int height,
	const std::uint8_t clear_r,
	const std::uint8_t clear_g,
	const std::uint8_t clear_b,
	const std::size_t r_offset,
	const std::size_t g_offset,
	const std::size_t b_offset
) {
	SmokePreviewPixelDiversity diversity;
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	std::array<std::size_t, 4096> color_bins {};
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4;
		const std::uint8_t r = pixels[offset + r_offset];
		const std::uint8_t g = pixels[offset + g_offset];
		const std::uint8_t b = pixels[offset + b_offset];
		const int delta = std::abs(static_cast<int>(r) - static_cast<int>(clear_r))
			+ std::abs(static_cast<int>(g) - static_cast<int>(clear_g)) + std::abs(static_cast<int>(b) - static_cast<int>(clear_b));
		if (delta <= 18) {
			continue;
		}

		++diversity.changed_pixels;
		const std::size_t bin = static_cast<std::size_t>(r / 16u) * 256u + static_cast<std::size_t>(g / 16u) * 16u
			+ static_cast<std::size_t>(b / 16u);
		++color_bins[bin];
	}

	for (const std::size_t bin_pixels : color_bins) {
		diversity.dominant_bin_pixels = std::max(diversity.dominant_bin_pixels, bin_pixels);
		if (bin_pixels >= 4u) {
			++diversity.significant_color_bins;
		}
	}
	return diversity;
}

bool validate_real_model_preview_pixel_diversity(
	const std::string_view backend,
	const std::string_view label,
	const std::vector<std::uint8_t>& pixels,
	const int width,
	const int height,
	const std::uint8_t clear_r,
	const std::uint8_t clear_g,
	const std::uint8_t clear_b,
	const std::size_t r_offset,
	const std::size_t g_offset,
	const std::size_t b_offset
) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("{} {} smoke failed: missing captured pixels for real model-preview diversity validation.", backend, label);
		return false;
	}

	const SmokePreviewPixelDiversity diversity =
		count_smoke_preview_pixel_diversity(pixels, width, height, clear_r, clear_g, clear_b, r_offset, g_offset, b_offset);
	const std::size_t min_changed_pixels = std::max<std::size_t>(64, pixel_count / 5000);
	const std::size_t min_color_bins = std::max<std::size_t>(16, std::min<std::size_t>(40, diversity.changed_pixels / 80));
	const std::size_t max_dominant_pixels = std::max<std::size_t>(32, diversity.changed_pixels * 7u / 10u);
	if (diversity.changed_pixels < min_changed_pixels || diversity.significant_color_bins < min_color_bins
		|| diversity.dominant_bin_pixels > max_dominant_pixels) {
		std::println(
			"{} {} smoke failed: real preview had {} changed pixels, {} significant color bins, and {} pixels in the dominant color bin; expected at least {} changed pixels, at least {} color bins, and at most {} dominant-bin pixels.",
			backend,
			label,
			diversity.changed_pixels,
			diversity.significant_color_bins,
			diversity.dominant_bin_pixels,
			min_changed_pixels,
			min_color_bins,
			max_dominant_pixels
		);
		return false;
	}

	std::println(
		"{} {} smoke validated real preview color diversity with {} changed pixels, {} significant color bins, and {} dominant-bin pixels.",
		backend,
		label,
		diversity.changed_pixels,
		diversity.significant_color_bins,
		diversity.dominant_bin_pixels
	);
	return true;
}

void count_material_preview_pixel(
	MaterialPreviewPixelCounts& counts,
	const std::uint8_t r,
	const std::uint8_t g,
	const std::uint8_t b
) {
	if (is_material_team_color_pixel(r, g, b)) {
		++counts.team_color;
	}
	if (is_material_hd_green_pixel(r, g, b)) {
		++counts.hd_green;
	}
	if (is_material_transparent_orange_pixel(r, g, b)) {
		++counts.transparent_orange;
	}
	if (is_material_transparent_magenta_pixel(r, g, b)) {
		++counts.transparent_magenta;
	}
	if (is_material_alpha_cyan_pixel(r, g, b)) {
		++counts.alpha_cyan;
	}
}

bool validate_material_preview_counts(
	const std::string_view backend,
	const std::string_view label,
	const MaterialPreviewPixelCounts& counts,
	const int width,
	const int height
) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	const std::size_t min_pixels = std::max<std::size_t>(24, pixel_count / 40000);
	if (counts.team_color < min_pixels || counts.hd_green < min_pixels || counts.transparent_orange < min_pixels
		|| counts.transparent_magenta < min_pixels || counts.alpha_cyan < min_pixels) {
		std::println(
			"{} {} smoke failed: saw team-color {}, HD green {}, transparent orange {}, transparent magenta {}, and alpha-key cyan {} pixels; expected at least {} for each synthetic material bucket.",
			backend,
			label,
			counts.team_color,
			counts.hd_green,
			counts.transparent_orange,
			counts.transparent_magenta,
			counts.alpha_cyan,
			min_pixels
		);
		return false;
	}

	std::println(
		"{} {} smoke captured {}x{} with {} team-color, {} HD green, {} transparent orange, {} transparent magenta, and {} alpha-key cyan pixels.",
		backend,
		label,
		width,
		height,
		counts.team_color,
		counts.hd_green,
		counts.transparent_orange,
		counts.transparent_magenta,
		counts.alpha_cyan
	);
	return true;
}

bool ensure_smoke_artifact_directory(const std::string_view backend, const QString& artifact_directory) {
	if (QDir(artifact_directory).exists() || QDir().mkpath(artifact_directory)) {
		return true;
	}

	std::println("{} smoke failed: unable to create artifact directory {}.", backend, artifact_directory.toStdString());
	return false;
}

#if HIVEWE_USE_METAL_RUNTIME
constexpr std::string_view k_model_preview_smoke_path = "units/human/footman/footman.mdx";
constexpr std::string_view k_material_preview_smoke_name = "smoke/material-preview.mdx";
constexpr std::string_view k_particle_preview_smoke_name = "smoke/particle-only.mdx";
constexpr double k_metal_smoke_artifact_scale = 1.0;

struct SmokePreviewClearColor {
	std::uint8_t b = 0;
	std::uint8_t g = 0;
	std::uint8_t r = 0;
};

std::uint8_t smoke_clear_channel(const float value) {
	return static_cast<std::uint8_t>(std::lround(std::clamp(value, 0.0f, 1.0f) * 255.0f));
}

SmokePreviewClearColor smoke_clear_color(const float r, const float g, const float b) {
	return SmokePreviewClearColor {
		.b = smoke_clear_channel(b),
		.g = smoke_clear_channel(g),
		.r = smoke_clear_channel(r),
	};
}

bool validate_smoke_preview_pixels(
	const std::string_view label,
	const std::vector<std::uint8_t>& pixels,
	const int width,
	const int height,
	const SmokePreviewClearColor clear_color
) {
	if (width <= 0 || height <= 0 || pixels.empty()) {
		std::println("Metal {} smoke failed: missing captured pixels.", label);
		return false;
	}

	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (pixels.size() < pixel_count * 4) {
		std::println(
			"Metal {} smoke failed: captured {} bytes for {}x{} BGRA frame.",
			label,
			pixels.size(),
			width,
			height
		);
		return false;
	}

	std::size_t changed_pixels = 0;
	int max_delta = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4;
		const int delta =
			std::abs(static_cast<int>(pixels[offset + 0]) - static_cast<int>(clear_color.b))
			+ std::abs(static_cast<int>(pixels[offset + 1]) - static_cast<int>(clear_color.g))
			+ std::abs(static_cast<int>(pixels[offset + 2]) - static_cast<int>(clear_color.r));
		if (delta > 18) {
			++changed_pixels;
		}
		max_delta = std::max(max_delta, delta);
	}

	const std::size_t min_changed_pixels = std::max<std::size_t>(64, pixel_count / 5000);
	if (changed_pixels < min_changed_pixels || max_delta < 30) {
		std::println(
			"Metal {} smoke failed: frame stayed too close to clear color ({} changed pixels, max delta {}, expected at least {}).",
			label,
			changed_pixels,
			max_delta,
			min_changed_pixels
		);
		return false;
	}

	std::println(
		"Metal {} smoke captured {}x{} with {} non-background pixels.",
		label,
		width,
		height,
		changed_pixels
	);
	return true;
}

bool validate_model_preview_frame_stats(
	const std::string_view label,
	const hive::metal::MetalModelFrameStats& stats,
	const bool require_model_draws = true,
	const bool require_alpha_tested_draw = false,
	const bool require_team_color_draw = false
) {
	const std::size_t total_model_draws = hive::metal::model_draw_count(stats);
	if (stats.mesh_count == 0 || stats.model_texture_count == 0 || (require_model_draws && total_model_draws == 0)
		|| (require_alpha_tested_draw && stats.alpha_tested_draw_count == 0)
		|| (require_team_color_draw && stats.team_color_draw_count == 0)) {
		std::println(
			"Metal {} smoke failed: widget frame had {} meshes, {} model textures, {} model draws, {} alpha-tested draws, and {} team-color draws.",
			label,
			stats.mesh_count,
			stats.model_texture_count,
			total_model_draws,
			stats.alpha_tested_draw_count,
			stats.team_color_draw_count
		);
		return false;
	}

	std::println(
		"Metal {} smoke widget frame stats: {} meshes, {} model textures, {} SD draws, {} HD draws, {} transparent SD draws, {} transparent HD draws, {} alpha-tested draws, {} team-color draws, {} particle draws.",
		label,
		stats.mesh_count,
		stats.model_texture_count,
		stats.opaque_sd_draw_count,
		stats.opaque_hd_draw_count,
		stats.transparent_sd_draw_count,
		stats.transparent_hd_draw_count,
		stats.alpha_tested_draw_count,
		stats.team_color_draw_count,
		stats.particle_draw_count
	);
	return true;
}

bool validate_model_editor_preview_stats(const ModelEditorMetalWidget::SmokePreviewStats& stats) {
	if (!stats.valid_sequence || stats.sequence_index < 0 || stats.geoset_count == 0 || stats.source_texture_count == 0) {
		std::println(
			"Metal model editor preview smoke failed: model '{}' had valid sequence {}, sequence {}, {} geosets, and {} source textures.",
			stats.model_name,
			stats.valid_sequence,
			stats.sequence_index,
			stats.geoset_count,
			stats.source_texture_count
		);
		return false;
	}

	return validate_model_preview_frame_stats("model editor preview", stats.frame, true, true, true);
}

bool validate_particle_preview_frame_stats(
	const std::string_view label,
	const hive::metal::MetalModelFrameStats& stats
) {
	if (stats.particle_draw_count < 6 || stats.particle_vertex_count == 0 || stats.particle_texture_count < 3 || stats.mesh_count < 2
		|| stats.model_texture_count < 2) {
		std::println(
			"Metal {} smoke failed: widget frame had {} PE1 spawned meshes, {} model textures, {} particle draws, {} particle vertices, and {} particle textures; expected spawned child/grandchild meshes plus parent and spawned-child PE2, PE1 TGA, and ribbon particle draws/textures.",
			label,
			stats.mesh_count,
			stats.model_texture_count,
			stats.particle_draw_count,
			stats.particle_vertex_count,
			stats.particle_texture_count
		);
		return false;
	}

	std::println(
		"Metal {} smoke widget frame stats: {} PE1 spawned meshes, {} model textures, {} particle draws, {} particle vertices, and {} particle textures.",
		label,
		stats.mesh_count,
		stats.model_texture_count,
		stats.particle_draw_count,
		stats.particle_vertex_count,
		stats.particle_texture_count
	);
	return true;
}

bool validate_model_editor_particle_preview_stats(const ModelEditorMetalWidget::SmokePreviewStats& stats) {
	if (!stats.valid_sequence || stats.sequence_index < 0 || stats.source_texture_count == 0) {
		std::println(
			"Metal model editor particle preview smoke failed: model '{}' had valid sequence {}, sequence {}, and {} source textures.",
			stats.model_name,
			stats.valid_sequence,
			stats.sequence_index,
			stats.source_texture_count
		);
		return false;
	}

	return validate_particle_preview_frame_stats("model editor particle preview", stats.frame);
}

bool validate_ribbon_preview_frame_stats(
	const std::string_view label,
	const hive::metal::MetalModelFrameStats& stats
) {
	if (stats.particle_draw_count == 0 || stats.particle_vertex_count == 0 || stats.particle_texture_count == 0) {
		std::println(
			"Metal {} smoke failed: widget frame had {} particle draws, {} particle vertices, and {} particle textures.",
			label,
			stats.particle_draw_count,
			stats.particle_vertex_count,
			stats.particle_texture_count
		);
		return false;
	}

	std::println(
		"Metal {} smoke widget frame stats: {} particle draws, {} particle vertices, and {} particle textures.",
		label,
		stats.particle_draw_count,
		stats.particle_vertex_count,
		stats.particle_texture_count
	);
	return true;
}

bool validate_model_editor_ribbon_preview_stats(const ModelEditorMetalWidget::SmokePreviewStats& stats) {
	if (!stats.valid_sequence || stats.sequence_index < 0 || stats.source_texture_count == 0) {
		std::println(
			"Metal model editor ribbon preview smoke failed: model '{}' had valid sequence {}, sequence {}, and {} source textures.",
			stats.model_name,
			stats.valid_sequence,
			stats.sequence_index,
			stats.source_texture_count
		);
		return false;
	}

	return validate_ribbon_preview_frame_stats("model editor ribbon preview", stats.frame);
}

bool validate_material_preview_frame_stats(
	const std::string_view label,
	const hive::metal::MetalModelFrameStats& stats
) {
	const bool missing_required_draw = stats.opaque_sd_draw_count == 0 || stats.opaque_hd_draw_count == 0
		|| stats.transparent_sd_draw_count == 0 || stats.transparent_hd_draw_count == 0
		|| stats.alpha_tested_draw_count == 0 || stats.team_color_draw_count == 0;
	if (stats.mesh_count == 0 || stats.model_texture_count == 0 || missing_required_draw) {
		std::println(
			"Metal {} smoke failed: widget frame had {} meshes, {} model textures, {} SD draws, {} HD draws, {} transparent SD draws, {} transparent HD draws, {} alpha-tested draws, and {} team-color draws.",
			label,
			stats.mesh_count,
			stats.model_texture_count,
			stats.opaque_sd_draw_count,
			stats.opaque_hd_draw_count,
			stats.transparent_sd_draw_count,
			stats.transparent_hd_draw_count,
			stats.alpha_tested_draw_count,
			stats.team_color_draw_count
		);
		return false;
	}

	std::println(
		"Metal {} smoke widget frame stats: {} meshes, {} model textures, {} SD draws, {} HD draws, {} transparent SD draws, {} transparent HD draws, {} alpha-tested draws, and {} team-color draws.",
		label,
		stats.mesh_count,
		stats.model_texture_count,
		stats.opaque_sd_draw_count,
		stats.opaque_hd_draw_count,
		stats.transparent_sd_draw_count,
		stats.transparent_hd_draw_count,
		stats.alpha_tested_draw_count,
		stats.team_color_draw_count
	);
	return true;
}

bool validate_model_editor_material_preview_stats(const ModelEditorMetalWidget::SmokePreviewStats& stats) {
	if (!stats.valid_sequence || stats.sequence_index < 0 || stats.geoset_count == 0 || stats.source_texture_count == 0) {
		std::println(
			"Metal model editor material preview smoke failed: model '{}' had valid sequence {}, sequence {}, {} geosets, and {} source textures.",
			stats.model_name,
			stats.valid_sequence,
			stats.sequence_index,
			stats.geoset_count,
			stats.source_texture_count
		);
		return false;
	}

	return validate_material_preview_frame_stats("model editor material preview", stats.frame);
}

bool validate_object_grid_preview_cell_stats(
	const ModelGridMetalWidget::SmokePreviewStats& stats,
	const fs::path& expected_path,
	const std::string_view label
) {
	if (
		stats.loaded_cell_count == 0 || stats.failed_cell_count != 0 || stats.rendered_cell_count != 1
		|| stats.first_rendered_path != expected_path
	) {
		std::println(
			"Metal {} smoke failed: loaded {} cells, failed {} cells, rendered {} cells, first rendered path '{}', expected '{}'.",
			label,
			stats.loaded_cell_count,
			stats.failed_cell_count,
			stats.rendered_cell_count,
			stats.first_rendered_path.generic_string(),
			expected_path.generic_string()
		);
		return false;
	}

	return true;
}

bool validate_object_grid_preview_stats(const ModelGridMetalWidget::SmokePreviewStats& stats) {
	const fs::path expected_path {std::string(k_model_preview_smoke_path)};
	if (!validate_object_grid_preview_cell_stats(stats, expected_path, "object-grid preview")) {
		return false;
	}

	return validate_model_preview_frame_stats("object-grid preview", stats.frame, true, true, true);
}

bool validate_object_grid_particle_preview_stats(
	const ModelGridMetalWidget::SmokePreviewStats& stats,
	const fs::path& expected_path
) {
	if (!validate_object_grid_preview_cell_stats(stats, expected_path, "object-grid particle preview")) {
		return false;
	}

	return validate_particle_preview_frame_stats("object-grid particle preview", stats.frame);
}

bool validate_object_grid_ribbon_preview_stats(
	const ModelGridMetalWidget::SmokePreviewStats& stats,
	const fs::path& expected_path
) {
	if (!validate_object_grid_preview_cell_stats(stats, expected_path, "object-grid ribbon preview")) {
		return false;
	}

	return validate_ribbon_preview_frame_stats("object-grid ribbon preview", stats.frame);
}

bool validate_object_grid_material_preview_stats(
	const ModelGridMetalWidget::SmokePreviewStats& stats,
	const fs::path& expected_path
) {
	if (!validate_object_grid_preview_cell_stats(stats, expected_path, "object-grid material preview")) {
		return false;
	}

	return validate_material_preview_frame_stats("object-grid material preview", stats.frame);
}

bool validate_object_grid_sequence_less_material_preview_stats(
	const ModelGridMetalWidget::SmokePreviewStats& stats,
	const fs::path& expected_path,
	const mdx::Extent& extent
) {
	if (!validate_object_grid_preview_cell_stats(stats, expected_path, "object-grid sequence-less material preview")) {
		return false;
	}

	constexpr float fov_rad = glm::radians(50.0f);
	const float expected_radius = std::max(
		std::isfinite(extent.bounds_radius) && extent.bounds_radius > 0.0f ? extent.bounds_radius * 1.1f : 0.0f,
		64.0f
	);
	const float expected_distance = expected_radius / std::sin(fov_rad * 0.5f);
	const float expected_fit_z = extent.minimum.z + (extent.maximum.z - extent.minimum.z) * 0.5f;
	if (
		std::abs(stats.first_rendered_fit_distance - expected_distance) > 0.5f
		|| std::abs(stats.first_rendered_fit_z - expected_fit_z) > 0.01f
	) {
		std::println(
			"Metal object-grid sequence-less material preview smoke failed: fit distance {} expected about {}, fit z {} expected {}.",
			stats.first_rendered_fit_distance,
			expected_distance,
			stats.first_rendered_fit_z,
			expected_fit_z
		);
		return false;
	}

	return validate_material_preview_frame_stats("object-grid sequence-less material preview", stats.frame);
}

bool validate_object_grid_multicell_preview_stats(
	const ModelGridMetalWidget::SmokePreviewStats& stats,
	const fs::path& expected_first_path
) {
	const bool invalid_frame = stats.frame.mesh_count < 2 || stats.frame.model_texture_count < 2 || stats.frame.opaque_sd_draw_count < 2;
	if (
		stats.loaded_cell_count < 2 || stats.failed_cell_count != 0 || stats.rendered_cell_count != 2
		|| stats.first_rendered_path != expected_first_path || invalid_frame
	) {
		std::println(
			"Metal object-grid multicell preview smoke failed: loaded {} cells, failed {} cells, rendered {} cells, first rendered path '{}', expected '{}', with {} meshes, {} textures, and {} SD draws.",
			stats.loaded_cell_count,
			stats.failed_cell_count,
			stats.rendered_cell_count,
			stats.first_rendered_path.generic_string(),
			expected_first_path.generic_string(),
			stats.frame.mesh_count,
			stats.frame.model_texture_count,
			stats.frame.opaque_sd_draw_count
		);
		return false;
	}

	std::println(
		"Metal object-grid multicell preview smoke widget frame stats: {} meshes, {} model textures, and {} SD draws.",
		stats.frame.mesh_count,
		stats.frame.model_texture_count,
		stats.frame.opaque_sd_draw_count
	);
	return true;
}

bool validate_object_grid_scrolled_preview_stats(
	const ModelGridMetalWidget::SmokePreviewStats& stats,
	const fs::path& expected_first_path
) {
	const bool partial_first_cell = stats.first_rendered_y < 0 && stats.first_rendered_y + stats.first_rendered_height > 0;
	const bool invalid_frame = stats.frame.mesh_count < 3 || stats.frame.model_texture_count < 3 || stats.frame.opaque_sd_draw_count < 3;
	if (
		stats.scroll_offset_y <= 0 || stats.content_height <= 320 || !partial_first_cell || stats.loaded_cell_count < 3
		|| stats.failed_cell_count != 0 || stats.rendered_cell_count < 3 || stats.first_rendered_path != expected_first_path
		|| invalid_frame
	) {
		std::println(
			"Metal object-grid scrolled preview smoke failed: scroll {}, content {}, loaded {}, failed {}, rendered {}, first cell '{}'"
			" at y {} height {}, expected partial '{}' with at least 3 meshes/textures/draws (got {}/{}/{}).",
			stats.scroll_offset_y,
			stats.content_height,
			stats.loaded_cell_count,
			stats.failed_cell_count,
			stats.rendered_cell_count,
			stats.first_rendered_path.generic_string(),
			stats.first_rendered_y,
			stats.first_rendered_height,
			expected_first_path.generic_string(),
			stats.frame.mesh_count,
			stats.frame.model_texture_count,
			stats.frame.opaque_sd_draw_count
		);
		return false;
	}

	std::println(
		"Metal object-grid scrolled preview smoke widget frame stats: scroll {}, content {}, first cell y {}, {} meshes, {} model textures, and {} SD draws.",
		stats.scroll_offset_y,
		stats.content_height,
		stats.first_rendered_y,
		stats.frame.mesh_count,
		stats.frame.model_texture_count,
		stats.frame.opaque_sd_draw_count
	);
	return true;
}

bool validate_object_grid_empty_preview_stats(const ModelGridMetalWidget::SmokePreviewStats& stats) {
	const bool has_model_work = stats.frame.mesh_count != 0 || stats.frame.model_texture_count != 0
		|| hive::metal::model_draw_count(stats.frame) != 0 || stats.frame.particle_draw_count != 0
		|| stats.frame.particle_vertex_count != 0 || stats.frame.particle_texture_count != 0;
	if (stats.loaded_cell_count != 0 || stats.failed_cell_count != 0 || stats.rendered_cell_count != 0 || has_model_work) {
		std::println(
			"Metal object-grid empty-filter preview smoke failed: loaded {} cells, failed {} cells, rendered {} cells, with {} meshes, {} model textures, {} model draws, {} particle draws, {} particle vertices, and {} particle textures.",
			stats.loaded_cell_count,
			stats.failed_cell_count,
			stats.rendered_cell_count,
			stats.frame.mesh_count,
			stats.frame.model_texture_count,
			hive::metal::model_draw_count(stats.frame),
			stats.frame.particle_draw_count,
			stats.frame.particle_vertex_count,
			stats.frame.particle_texture_count
		);
		return false;
	}

	std::println("Metal object-grid empty-filter preview smoke captured a cleared frame with no rendered cells.");
	return true;
}

bool validate_object_grid_load_failure_preview_stats(
	const ModelGridMetalWidget::SmokePreviewStats& stats,
	const fs::path& expected_failed_path
) {
	const bool has_model_work = stats.frame.mesh_count != 0 || stats.frame.model_texture_count != 0
		|| hive::metal::model_draw_count(stats.frame) != 0 || stats.frame.particle_draw_count != 0
		|| stats.frame.particle_vertex_count != 0 || stats.frame.particle_texture_count != 0;
	const std::string expected_failed_path_string = expected_failed_path.generic_string();
	const bool error_mentions_path = stats.first_failed_error.find(expected_failed_path_string) != std::string::npos;
	const bool error_is_generic_expected_access = stats.first_failed_error.find("bad access to std::expected") != std::string::npos;
	if (
		stats.loaded_cell_count != 0 || stats.failed_cell_count != 1 || stats.rendered_cell_count != 0
		|| stats.first_failed_path != expected_failed_path || stats.first_failed_error.empty() || !error_mentions_path
		|| error_is_generic_expected_access || has_model_work
	) {
		std::println(
			"Metal object-grid load-failure preview smoke failed: loaded {} cells, failed {} cells, rendered {} cells, first failed path '{}', expected '{}', error '{}', error mentions path {}, generic expected-access {}, with {} meshes, {} model textures, {} model draws, {} particle draws, {} particle vertices, and {} particle textures.",
			stats.loaded_cell_count,
			stats.failed_cell_count,
			stats.rendered_cell_count,
			stats.first_failed_path.generic_string(),
			expected_failed_path_string,
			stats.first_failed_error,
			error_mentions_path,
			error_is_generic_expected_access,
			stats.frame.mesh_count,
			stats.frame.model_texture_count,
			hive::metal::model_draw_count(stats.frame),
			stats.frame.particle_draw_count,
			stats.frame.particle_vertex_count,
			stats.frame.particle_texture_count
		);
		return false;
	}

	std::println(
		"Metal object-grid load-failure preview smoke captured failed cell '{}' with error '{}'.",
		stats.first_failed_path.generic_string(),
		stats.first_failed_error
	);
	return true;
}

std::shared_ptr<mdx::MDX> load_model_preview_smoke_model() {
	auto model_file = hierarchy.open_file(fs::path(std::string(k_model_preview_smoke_path)));
	if (!model_file) {
		std::println(
			"Metal model preview smoke failed: unable to load {}: {}",
			k_model_preview_smoke_path,
			model_file.error()
		);
		return nullptr;
	}

	try {
		BinaryReader reader = std::move(model_file.value());
		auto model = std::make_shared<mdx::MDX>(reader);
		model->name = std::string(k_model_preview_smoke_path);
		if (model->geosets.empty()) {
			std::println("Metal model preview smoke failed: {} has no geosets.", k_model_preview_smoke_path);
			return nullptr;
		}
		return model;
	} catch (const std::exception& error) {
		std::println(
			"Metal model preview smoke failed while parsing {}: {}",
			k_model_preview_smoke_path,
			error.what()
		);
		return nullptr;
	}
}

mdx::Extent make_particle_preview_extent() {
	mdx::Extent extent;
	extent.bounds_radius = 160.0f;
	extent.minimum = glm::vec3(-96.0f, -96.0f, -32.0f);
	extent.maximum = glm::vec3(96.0f, 96.0f, 128.0f);
	return extent;
}

mdx::Extent make_material_preview_extent() {
	mdx::Extent extent;
	extent.bounds_radius = 180.0f;
	extent.minimum = glm::vec3(-128.0f, -96.0f, -32.0f);
	extent.maximum = glm::vec3(128.0f, 96.0f, 128.0f);
	return extent;
}

mdx::Layer make_material_preview_layer(
	const bool hd,
	const std::uint32_t blend_mode,
	const std::vector<std::uint32_t>& texture_ids
) {
	mdx::Layer layer {};
	layer.blend_mode = blend_mode;
	layer.alpha = 1.0f;
	layer.shader = hd ? mdx::ShaderType::HD : mdx::ShaderType::SD;
	layer.shading_flags = mdx::Layer::ShadingFlags::unshaded | mdx::Layer::ShadingFlags::two_sided;
	for (const std::uint32_t texture_id : texture_ids) {
		mdx::LayerTexture texture {};
		texture.id = texture_id;
		layer.textures.push_back(texture);
	}
	return layer;
}

mdx::Track<std::uint32_t> make_material_preview_texture_id_track(const std::uint32_t frame, const std::uint32_t texture_id) {
	mdx::Track<std::uint32_t> track {};
	track.frame = static_cast<std::int32_t>(frame);
	track.value = texture_id;
	track.inTan = texture_id;
	track.outTan = texture_id;
	return track;
}

void add_particle_spawn_birth_stand_sequences(mdx::MDX& model) {
	mdx::Sequence birth {};
	birth.name = "Birth";
	birth.start_frame = 0;
	birth.end_frame = 100;
	birth.extent = model.extent;
	model.sequences.push_back(birth);

	mdx::Sequence stand {};
	stand.name = "Stand";
	stand.start_frame = 1000;
	stand.end_frame = 2000;
	stand.flags = mdx::Sequence::Flags::looping;
	stand.extent = model.extent;
	model.sequences.push_back(stand);
}

mdx::Track<glm::vec3> make_particle_spawn_translation_track(const std::int32_t frame, const glm::vec3& value) {
	mdx::Track<glm::vec3> track {};
	track.frame = frame;
	track.value = value;
	track.inTan = value;
	track.outTan = value;
	return track;
}

mdx::Geoset make_material_preview_geoset(
	const std::uint32_t material_id,
	const glm::vec3& offset,
	const mdx::Extent& extent,
	const float half_width = 34.0f,
	const float half_height = 28.0f
) {
	mdx::Geoset geoset {};
	geoset.vertices = {
		offset + glm::vec3(-half_width, -half_height, 0.0f),
		offset + glm::vec3(half_width, -half_height, 0.0f),
		offset + glm::vec3(-half_width, half_height, 0.0f),
		offset + glm::vec3(half_width, half_height, 0.0f),
	};
	geoset.normals.assign(geoset.vertices.size(), glm::vec3(0.0f, 0.0f, 1.0f));
	geoset.face_type_groups = {4u};
	geoset.face_groups = {6u};
	geoset.faces = {0u, 1u, 2u, 1u, 3u, 2u};
	geoset.material_id = material_id;
	geoset.selection_group = 0;
	geoset.selection_flags = 0;
	geoset.lod = 0;
	geoset.extent = extent;
	geoset.sequence_extents = {extent};
	geoset.tangents.assign(geoset.vertices.size(), glm::vec4(1.0f, 0.0f, 0.0f, 1.0f));
	geoset.skin.reserve(geoset.vertices.size() * 8u);
	for (std::size_t i = 0; i < geoset.vertices.size(); ++i) {
		geoset.skin.insert(geoset.skin.end(), {0u, 0u, 0u, 0u, 255u, 0u, 0u, 0u});
	}
	geoset.uv_sets.push_back({
		glm::vec2(0.0f, 1.0f),
		glm::vec2(1.0f, 1.0f),
		glm::vec2(0.0f, 0.0f),
		glm::vec2(1.0f, 0.0f),
	});
	return geoset;
}

std::shared_ptr<mdx::MDX> make_material_preview_smoke_model(const std::vector<fs::path>& texture_paths) {
	auto model = std::make_shared<mdx::MDX>();
	model->version = mdx::MDX::LATEST_MDX_VERSION;
	model->name = std::string(k_material_preview_smoke_name);
	model->blend_time = 150;
	model->extent = make_material_preview_extent();

	mdx::Sequence sequence {};
	sequence.name = "Stand";
	sequence.start_frame = 0;
	sequence.end_frame = 1000;
	sequence.flags = mdx::Sequence::Flags::looping;
	sequence.extent = model->extent;
	model->sequences.push_back(sequence);

	decltype(model->bones)::value_type bone {};
	bone.node.name = "SmokeMaterialBone";
	bone.node.id = 0;
	bone.node.parent_id = -1;
	bone.node.flags = mdx::Node::bone;
	bone.geoset_id = -1;
	bone.geoset_animation_id = -1;
	model->bones.push_back(bone);
	model->pivots.push_back(glm::vec3(0.0f));

	mdx::Texture team_color_texture {};
	team_color_texture.replaceable_id = 1;
	team_color_texture.flags = 0;
	model->textures.push_back(team_color_texture);
	for (const fs::path& texture_path : texture_paths) {
		mdx::Texture texture {};
		texture.replaceable_id = 0;
		texture.file_name = texture_path;
		texture.flags = 0;
		model->textures.push_back(texture);
	}

	auto add_material = [&](std::vector<mdx::Layer> layers) {
		mdx::Material material {};
		material.layers = std::move(layers);
		model->materials.push_back(std::move(material));
		return static_cast<std::uint32_t>(model->materials.size() - 1u);
	};

	mdx::Layer hd_layer = make_material_preview_layer(true, 0u, {2u, 2u, 3u, 4u, 5u, 6u});
	hd_layer.textures[0].KMTF.id = model->unique_tracks++;
	hd_layer.textures[0].KMTF.tracks = {make_material_preview_texture_id_track(0u, 1u)};

	const std::uint32_t team_material = add_material({make_material_preview_layer(false, 0u, {0u})});
	const std::uint32_t hd_material = add_material({hd_layer});
	const std::uint32_t transparent_sd_material = add_material({make_material_preview_layer(false, 2u, {7u})});
	const std::uint32_t transparent_hd_material = add_material({make_material_preview_layer(true, 2u, {8u, 9u, 10u, 11u, 12u, 13u})});
	const std::uint32_t alpha_key_material = add_material({make_material_preview_layer(false, 1u, {6u})});

	model->geosets.push_back(make_material_preview_geoset(team_material, glm::vec3(-24.0f, -24.0f, 24.0f), model->extent, 20.0f, 18.0f));
	model->geosets.push_back(make_material_preview_geoset(hd_material, glm::vec3(24.0f, -24.0f, 24.0f), model->extent, 20.0f, 18.0f));
	model->geosets.push_back(make_material_preview_geoset(transparent_sd_material, glm::vec3(48.0f, -24.0f, 24.0f), model->extent, 20.0f, 18.0f));
	model->geosets.push_back(make_material_preview_geoset(transparent_hd_material, glm::vec3(-24.0f, 24.0f, 24.0f), model->extent, 20.0f, 18.0f));
	model->geosets.push_back(make_material_preview_geoset(alpha_key_material, glm::vec3(24.0f, 24.0f, 24.0f), model->extent, 20.0f, 18.0f));
	model->geosets.push_back(make_material_preview_geoset(999u, glm::vec3(0.0f, 48.0f, 24.0f), model->extent, 10.0f, 10.0f));
	return model;
}

std::shared_ptr<mdx::MDX> make_multicell_replaceable_preview_smoke_model() {
	auto model = std::make_shared<mdx::MDX>();
	model->version = mdx::MDX::LATEST_MDX_VERSION;
	model->name = "smoke/multicell-shared-preview.mdx";
	model->blend_time = 150;
	model->extent = make_particle_preview_extent();

	mdx::Sequence sequence {};
	sequence.name = "Stand";
	sequence.start_frame = 0;
	sequence.end_frame = 1000;
	sequence.flags = mdx::Sequence::Flags::looping;
	sequence.extent = model->extent;
	model->sequences.push_back(sequence);

	decltype(model->bones)::value_type bone {};
	bone.node.name = "SmokeMulticellBone";
	bone.node.id = 0;
	bone.node.parent_id = -1;
	bone.node.flags = mdx::Node::bone;
	bone.geoset_id = -1;
	bone.geoset_animation_id = -1;
	model->bones.push_back(bone);
	model->pivots.push_back(glm::vec3(0.0f));

	mdx::Texture replaceable_texture {};
	replaceable_texture.replaceable_id = 31;
	replaceable_texture.flags = 0;
	model->textures.push_back(replaceable_texture);

	mdx::Material material {};
	material.layers = {make_material_preview_layer(false, 0u, {0u})};
	model->materials.push_back(material);
	model->geosets.push_back(make_material_preview_geoset(0u, glm::vec3(0.0f, 0.0f, 24.0f), model->extent));
	return model;
}

std::shared_ptr<mdx::MDX> make_particle_spawn_smoke_model(const fs::path& texture_path, const fs::path& spawned_model_path = {}) {
	auto model = std::make_shared<mdx::MDX>();
	model->version = mdx::MDX::LATEST_MDX_VERSION;
	model->name = "smoke/particle-spawn-child.mdx";
	model->blend_time = 150;
	model->extent = make_particle_preview_extent();

	add_particle_spawn_birth_stand_sequences(*model);

	decltype(model->bones)::value_type bone {};
	bone.node.name = "ParticleSpawnBone";
	bone.node.id = 0;
	bone.node.parent_id = -1;
	bone.node.flags = mdx::Node::bone;
	bone.node.KGTR.id = model->unique_tracks++;
	bone.node.KGTR.interpolation_type = mdx::InterpolationType::linear;
	bone.node.KGTR.tracks = {
		make_particle_spawn_translation_track(0, glm::vec3(-3.0f, 0.0f, 0.0f)),
		make_particle_spawn_translation_track(100, glm::vec3(0.0f, 0.0f, 0.0f)),
		make_particle_spawn_translation_track(1000, glm::vec3(0.0f, 0.0f, 0.0f)),
		make_particle_spawn_translation_track(2000, glm::vec3(6.0f, 0.0f, 0.0f)),
	};
	bone.geoset_id = -1;
	bone.geoset_animation_id = -1;
	model->bones.push_back(bone);
	model->pivots.push_back(glm::vec3(0.0f));

	mdx::Texture texture {};
	texture.replaceable_id = 0;
	texture.file_name = texture_path;
	texture.flags = 0;
	model->textures.push_back(texture);

	mdx::Material material {};
	material.layers = {make_material_preview_layer(false, 0u, {0u})};
	model->materials.push_back(material);
	const bool emits_nested_model = !spawned_model_path.empty();
	const glm::vec3 mesh_offset = emits_nested_model ? glm::vec3(-42.0f, 0.0f, 16.0f) : glm::vec3(64.0f, 0.0f, 24.0f);
	const float mesh_half_width = emits_nested_model ? 36.0f : 52.0f;
	const float mesh_half_height = emits_nested_model ? 30.0f : 42.0f;
	model->geosets.push_back(make_material_preview_geoset(0u, mesh_offset, model->extent, mesh_half_width, mesh_half_height));
	model->geosets.back().sequence_extents = {model->extent, model->extent};

	const auto add_particle_spawn_pivot = [&](const glm::vec3 pivot) {
		const int node_id = static_cast<int>(model->pivots.size());
		model->pivots.push_back(pivot);
		return node_id;
	};
	const int child_particle_node_id = add_particle_spawn_pivot(glm::vec3(-128.0f, -144.0f, 96.0f));
	const int child_tga_node_id = add_particle_spawn_pivot(glm::vec3(128.0f, -144.0f, 96.0f));
	const int child_ribbon_node_id = add_particle_spawn_pivot(glm::vec3(0.0f, -180.0f, 96.0f));

	mdx::ParticleEmitter2 child_emitter {};
	child_emitter.node.name = "SpawnedChildPe2";
	child_emitter.node.id = child_particle_node_id;
	child_emitter.node.parent_id = -1;
	child_emitter.node.flags = mdx::Node::emitter | mdx::Node::unshaded;
	child_emitter.speed = 0.0f;
	child_emitter.variation = 0.0f;
	child_emitter.latitude = 0.0f;
	child_emitter.gravity = 0.0f;
	child_emitter.life_span = 1.5f;
	child_emitter.emission_rate = 90.0f;
	child_emitter.length = 24.0f;
	child_emitter.width = 24.0f;
	child_emitter.filter_mode = 0;
	child_emitter.rows = 1;
	child_emitter.columns = 1;
	child_emitter.head_or_tail = 0;
	child_emitter.tail_length = 0.0f;
	child_emitter.time_middle = 0.5f;
	child_emitter.start_segment_color = glm::vec3(1.0f);
	child_emitter.middle_segment_color = glm::vec3(1.0f);
	child_emitter.end_segment_color = glm::vec3(1.0f);
	child_emitter.segment_alphas = glm::u8vec3(255u);
	child_emitter.segment_scaling = glm::vec3(22.0f, 24.0f, 18.0f);
	child_emitter.head_intervals = glm::uvec3(0u, 0u, 1u);
	child_emitter.head_decay_intervals = glm::uvec3(0u, 0u, 1u);
	child_emitter.tail_intervals = glm::uvec3(0u, 0u, 1u);
	child_emitter.tail_decay_intervals = glm::uvec3(0u, 0u, 1u);
	child_emitter.texture_id = 0;
	child_emitter.squirt = 0;
	child_emitter.priority_plane = 0;
	child_emitter.replaceable_id = 0;
	model->emitters2.push_back(child_emitter);

	decltype(model->emitters1)::value_type child_tga_emitter {};
	child_tga_emitter.node.name = "SpawnedChildLegacyTga";
	child_tga_emitter.node.id = child_tga_node_id;
	child_tga_emitter.node.parent_id = -1;
	child_tga_emitter.node.flags = mdx::Node::emitter | mdx::Node::unshaded | 0x10000;
	child_tga_emitter.emission_rate = 90.0f;
	child_tga_emitter.gravity = 0.0f;
	child_tga_emitter.longitude = 0.0f;
	child_tga_emitter.latitude = 0.0f;
	child_tga_emitter.path = texture_path.string();
	child_tga_emitter.life_span = 1.5f;
	child_tga_emitter.speed = 0.0f;
	model->emitters1.push_back(child_tga_emitter);

	decltype(model->ribbons)::value_type child_ribbon {};
	child_ribbon.node.name = "SpawnedChildRibbon";
	child_ribbon.node.id = child_ribbon_node_id;
	child_ribbon.node.parent_id = -1;
	child_ribbon.node.flags = mdx::Node::ribbon_emitter | mdx::Node::unshaded;
	child_ribbon.height_above = 36.0f;
	child_ribbon.height_below = 36.0f;
	child_ribbon.alpha = 1.0f;
	child_ribbon.color = glm::vec3(1.0f);
	child_ribbon.life_span = 1.5f;
	child_ribbon.texture_slot = 0u;
	child_ribbon.emission_rate = 90u;
	child_ribbon.rows = 1u;
	child_ribbon.columns = 1u;
	child_ribbon.material_id = 0u;
	child_ribbon.gravity = 0.0f;
	model->ribbons.push_back(child_ribbon);

	if (!spawned_model_path.empty()) {
		const int nested_emitter_node_id = add_particle_spawn_pivot(glm::vec3(0.0f));

		decltype(model->emitters1)::value_type nested_emitter {};
		nested_emitter.node.name = "NestedLegacyMdlParticles";
		nested_emitter.node.id = nested_emitter_node_id;
		nested_emitter.node.parent_id = -1;
		nested_emitter.node.flags = mdx::Node::emitter | mdx::Node::unshaded | 0x8000;
		nested_emitter.emission_rate = 90.0f;
		nested_emitter.gravity = 0.0f;
		nested_emitter.longitude = 0.0f;
		nested_emitter.latitude = 0.0f;
		nested_emitter.path = spawned_model_path.string();
		nested_emitter.life_span = 1.5f;
		nested_emitter.speed = 0.0f;
		model->emitters1.push_back(nested_emitter);
	}
	return model;
}

std::shared_ptr<mdx::MDX> make_particle_preview_smoke_model(
	const fs::path& pe2_texture_path,
	const fs::path& pe1_tga_texture_path,
	const fs::path& ribbon_texture_path,
	const fs::path& spawned_model_path
) {
	auto model = std::make_shared<mdx::MDX>();
	model->version = mdx::MDX::LATEST_MDX_VERSION;
	model->name = std::string(k_particle_preview_smoke_name);
	model->blend_time = 150;
	model->extent = make_particle_preview_extent();

	mdx::Sequence sequence {};
	sequence.name = "Stand";
	sequence.start_frame = 0;
	sequence.end_frame = 1000;
	sequence.flags = mdx::Sequence::Flags::looping;
	sequence.extent = model->extent;
	model->sequences.push_back(sequence);

	mdx::Texture texture {};
	texture.replaceable_id = 0;
	texture.file_name = pe2_texture_path;
	texture.flags = 0;
	model->textures.push_back(texture);

	model->pivots.push_back(glm::vec3(0.0f));
	model->pivots.push_back(glm::vec3(42.0f, 0.0f, 0.0f));
	model->pivots.push_back(glm::vec3(-42.0f, 0.0f, 0.0f));
	model->pivots.push_back(glm::vec3(0.0f));

	mdx::Texture ribbon_texture {};
	ribbon_texture.replaceable_id = 0;
	ribbon_texture.file_name = ribbon_texture_path;
	ribbon_texture.flags = 0;
	model->textures.push_back(ribbon_texture);

	mdx::Material ribbon_material {};
	ribbon_material.layers = {make_material_preview_layer(false, 0u, {1u})};
	model->materials.push_back(ribbon_material);

	mdx::ParticleEmitter2 emitter {};
	emitter.node.name = "SmokeParticles";
	emitter.node.id = 0;
	emitter.node.parent_id = -1;
	emitter.node.flags = mdx::Node::emitter | mdx::Node::unshaded;
	emitter.speed = 0.0f;
	emitter.variation = 0.0f;
	emitter.latitude = 0.0f;
	emitter.gravity = 0.0f;
	emitter.life_span = 2.0f;
	emitter.emission_rate = 120.0f;
	emitter.length = 8.0f;
	emitter.width = 8.0f;
	emitter.filter_mode = 0;
	emitter.rows = 1;
	emitter.columns = 1;
	emitter.head_or_tail = 0;
	emitter.tail_length = 0.0f;
	emitter.time_middle = 0.5f;
	emitter.start_segment_color = glm::vec3(1.0f, 0.25f, 0.10f);
	emitter.middle_segment_color = glm::vec3(1.0f, 0.65f, 0.08f);
	emitter.end_segment_color = glm::vec3(0.90f, 0.05f, 0.15f);
	emitter.segment_alphas = glm::u8vec3(255u, 255u, 220u);
	emitter.segment_scaling = glm::vec3(8.0f, 10.0f, 6.0f);
	emitter.head_intervals = glm::uvec3(0u, 0u, 1u);
	emitter.head_decay_intervals = glm::uvec3(0u, 0u, 1u);
	emitter.tail_intervals = glm::uvec3(0u, 0u, 1u);
	emitter.tail_decay_intervals = glm::uvec3(0u, 0u, 1u);
	emitter.texture_id = 0;
	emitter.squirt = 0;
	emitter.priority_plane = 0;
	emitter.replaceable_id = 0;
	model->emitters2.push_back(emitter);

	decltype(model->emitters1)::value_type legacy_emitter {};
	legacy_emitter.node.name = "LegacyTgaParticles";
	legacy_emitter.node.id = 1;
	legacy_emitter.node.parent_id = -1;
	legacy_emitter.node.flags = mdx::Node::emitter | mdx::Node::unshaded | 0x10000;
	legacy_emitter.emission_rate = 180.0f;
	legacy_emitter.gravity = 0.0f;
	legacy_emitter.longitude = 0.0f;
	legacy_emitter.latitude = 0.0f;
	legacy_emitter.path = pe1_tga_texture_path.string();
	legacy_emitter.life_span = 1.5f;
	legacy_emitter.speed = 560.0f;
	model->emitters1.push_back(legacy_emitter);

	if (!spawned_model_path.empty()) {
		decltype(model->emitters1)::value_type model_emitter {};
		model_emitter.node.name = "LegacyMdlParticles";
		model_emitter.node.id = 2;
		model_emitter.node.parent_id = -1;
		model_emitter.node.flags = mdx::Node::emitter | mdx::Node::unshaded | 0x8000;
		model_emitter.emission_rate = 90.0f;
		model_emitter.gravity = 0.0f;
		model_emitter.longitude = 0.0f;
		model_emitter.latitude = 0.0f;
		model_emitter.path = spawned_model_path.string();
		model_emitter.life_span = 1.5f;
		model_emitter.speed = 0.0f;
		model->emitters1.push_back(model_emitter);
	}

	decltype(model->ribbons)::value_type ribbon {};
	ribbon.node.name = "CyanRibbonParticles";
	ribbon.node.id = 3;
	ribbon.node.parent_id = -1;
	ribbon.node.flags = mdx::Node::ribbon_emitter | mdx::Node::unshaded;
	ribbon.node.KGTR.id = model->unique_tracks++;
	ribbon.node.KGTR.interpolation_type = mdx::InterpolationType::linear;
	ribbon.node.KGTR.tracks = {
		make_particle_spawn_translation_track(0, glm::vec3(-24.0f, -96.0f, 190.0f)),
		make_particle_spawn_translation_track(1000, glm::vec3(340.0f, -96.0f, 190.0f)),
	};
	ribbon.height_above = 12.0f;
	ribbon.height_below = 12.0f;
	ribbon.alpha = 1.0f;
	ribbon.color = glm::vec3(0.0f, 1.0f, 1.0f);
	ribbon.life_span = 2.0f;
	ribbon.texture_slot = 0u;
	ribbon.emission_rate = 60u;
	ribbon.rows = 1u;
	ribbon.columns = 1u;
	ribbon.material_id = 0u;
	ribbon.gravity = 18000.0f;
	model->ribbons.push_back(ribbon);

	return model;
}

std::shared_ptr<mdx::MDX> make_ribbon_preview_smoke_model(const fs::path& texture_path) {
	auto model = std::make_shared<mdx::MDX>();
	model->version = mdx::MDX::LATEST_MDX_VERSION;
	model->name = "smoke/ribbon-preview.mdx";
	model->blend_time = 150;
	model->extent = make_particle_preview_extent();

	mdx::Sequence sequence {};
	sequence.name = "Stand";
	sequence.start_frame = 0;
	sequence.end_frame = 1000;
	sequence.flags = mdx::Sequence::Flags::looping;
	sequence.extent = model->extent;
	model->sequences.push_back(sequence);

	mdx::Texture texture {};
	texture.replaceable_id = 0;
	texture.file_name = texture_path;
	texture.flags = 0;
	model->textures.push_back(texture);

	mdx::Material material {};
	material.layers = {make_material_preview_layer(false, 0u, {0u})};
	model->materials.push_back(material);
	model->pivots.push_back(glm::vec3(0.0f));

	decltype(model->ribbons)::value_type ribbon {};
	ribbon.node.name = "StandaloneCyanRibbon";
	ribbon.node.id = 0;
	ribbon.node.parent_id = -1;
	ribbon.node.flags = mdx::Node::ribbon_emitter | mdx::Node::unshaded;
	ribbon.node.KGTR.id = model->unique_tracks++;
	ribbon.node.KGTR.interpolation_type = mdx::InterpolationType::linear;
	ribbon.node.KGTR.tracks = {
		make_particle_spawn_translation_track(0, glm::vec3(-24.0f, 0.0f, 64.0f)),
		make_particle_spawn_translation_track(1000, glm::vec3(340.0f, 0.0f, 64.0f)),
	};
	ribbon.height_above = 48.0f;
	ribbon.height_below = 48.0f;
	ribbon.alpha = 1.0f;
	ribbon.color = glm::vec3(0.0f, 1.0f, 1.0f);
	ribbon.life_span = 2.0f;
	ribbon.texture_slot = 0u;
	ribbon.emission_rate = 120u;
	ribbon.rows = 1u;
	ribbon.columns = 1u;
	ribbon.material_id = 0u;
	ribbon.gravity = 18000.0f;
	model->ribbons.push_back(ribbon);

	return model;
}

bool write_smoke_tga_texture(
	const fs::path& texture_path,
	const std::uint8_t r,
	const std::uint8_t g,
	const std::uint8_t b,
	const std::uint8_t a
) {
	const std::array<std::uint8_t, 22> tga {
		0u, 0u, 2u, 0u, 0u, 0u, 0u, 0u, 0u, 0u, 0u,
		0u, 1u, 0u, 1u, 0u, 32u, 8u, b, g, r, a,
	};

	std::ofstream output(texture_path, std::ios::binary);
	if (!output) {
		std::println("Metal particle preview smoke failed: unable to write texture {}.", texture_path.string());
		return false;
	}
	output.write(reinterpret_cast<const char*>(tga.data()), static_cast<std::streamsize>(tga.size()));
	if (!output) {
		std::println("Metal particle preview smoke failed while writing texture {}.", texture_path.string());
		return false;
	}
	return true;
}

bool write_particle_preview_texture(const fs::path& texture_path) {
	return write_smoke_tga_texture(texture_path, 255u, 255u, 255u, 255u);
}

bool write_material_preview_textures(const std::vector<fs::path>& texture_paths) {
	static constexpr std::array<std::array<std::uint8_t, 4>, 13> colors {{
		{40u, 220u, 80u, 255u},
		{120u, 120u, 255u, 255u},
		{180u, 180u, 180u, 255u},
		{255u, 180u, 60u, 255u},
		{255u, 80u, 80u, 255u},
		{80u, 255u, 220u, 255u},
		{255u, 140u, 60u, 192u},
		{255u, 40u, 220u, 192u},
		{120u, 80u, 255u, 192u},
		{180u, 180u, 180u, 192u},
		{255u, 140u, 60u, 192u},
		{255u, 80u, 120u, 192u},
		{80u, 220u, 255u, 192u},
	}};
	if (texture_paths.size() != colors.size()) {
		std::println("Metal material preview smoke failed: expected {} textures, got {}.", colors.size(), texture_paths.size());
		return false;
	}

	for (std::size_t i = 0; i < texture_paths.size(); ++i) {
		const auto& color = colors[i];
		if (!write_smoke_tga_texture(texture_paths[i], color[0], color[1], color[2], color[3])) {
			return false;
		}
	}
	return true;
}

bool write_tinted_material_preview_textures(
	const std::vector<fs::path>& texture_paths,
	const std::array<std::uint8_t, 4>& color
) {
	if (texture_paths.size() != 13) {
		std::println("Metal material preview smoke failed: expected 13 tinted textures, got {}.", texture_paths.size());
		return false;
	}

	for (const auto& texture_path : texture_paths) {
		if (!write_smoke_tga_texture(texture_path, color[0], color[1], color[2], color[3])) {
			return false;
		}
	}
	return true;
}

bool write_preview_model(
	const std::shared_ptr<mdx::MDX>& model,
	const fs::path& model_path,
	const std::string_view label
) {
	if (!model) {
		return false;
	}

	const BinaryWriter writer = model->to_mdx();
	std::ofstream output(model_path, std::ios::binary);
	if (!output) {
		std::println("Metal {} smoke failed: unable to write model {}.", label, model_path.string());
		return false;
	}
	output.write(reinterpret_cast<const char*>(writer.buffer.data()), static_cast<std::streamsize>(writer.buffer.size()));
	if (!output) {
		std::println("Metal {} smoke failed while writing model {}.", label, model_path.string());
		return false;
	}
	return true;
}

template <typename Widget, typename CaptureFn>
bool capture_metal_preview_widget(
	const std::string_view label,
	Widget& widget,
	CaptureFn capture,
	const SmokePreviewClearColor clear_color
) {
	bool ok = false;
	bool completed = false;

	widget.show();
	widget.raise();
	widget.update();

	QEventLoop loop;
	QTimer::singleShot(250, &loop, [&] {
		QCoreApplication::processEvents(QEventLoop::AllEvents, 100);

		std::vector<std::uint8_t> pixels;
		int width = 0;
		int height = 0;
		const bool captured = capture(pixels, width, height);
		ok = captured && validate_smoke_preview_pixels(label, pixels, width, height, clear_color);
		completed = true;
		loop.quit();
	});
	QTimer::singleShot(5000, &loop, [&] {
		if (!completed) {
			std::println("Metal {} smoke failed: timed out waiting for a preview frame.", label);
			ok = false;
			loop.quit();
		}
	});
	loop.exec();

	widget.close();
	return ok;
}

class MetalSmokeSurfaceWidget final : public QWidget {
public:
	using QWidget::QWidget;

	QPaintEngine* paintEngine() const override {
		return nullptr;
	}
};

bool save_metal_smoke_artifact_png(
	const QString& artifact_directory,
	const std::string_view file_name,
	const std::vector<std::uint8_t>& bgra_pixels,
	const int width,
	const int height
) {
	if (artifact_directory.isEmpty()) {
		return true;
	}
	if (!ensure_smoke_artifact_directory("Metal", artifact_directory)) {
		return false;
	}

	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || bgra_pixels.size() < pixel_count * 4) {
		std::println("Metal smoke failed: missing pixels for artifact {}.", file_name);
		return false;
	}

	const QString output_path = QDir(artifact_directory).filePath(
		QString::fromUtf8(file_name.data(), static_cast<qsizetype>(file_name.size()))
	);
	const QImage image(
		bgra_pixels.data(),
		width,
		height,
		width * 4,
		QImage::Format_ARGB32
	);
	if (image.isNull() || !image.copy().save(output_path, "PNG")) {
		std::println("Metal smoke failed: unable to save artifact {}.", output_path.toStdString());
		return false;
	}

	qInfo() << "Captured Metal smoke artifact:" << output_path;
	return true;
}

bool is_smoke_red_orange_particle_pixel(const std::uint8_t b, const std::uint8_t g, const std::uint8_t r) {
	return r > 140u && g > 20u && g < 170u && b < 120u;
}

bool is_smoke_yellow_particle_pixel(const std::uint8_t b, const std::uint8_t g, const std::uint8_t r) {
	return r > 170u && g > 120u && b < 120u;
}

bool is_smoke_magenta_particle_pixel(const std::uint8_t b, const std::uint8_t g, const std::uint8_t r) {
	return r > 150u && b > 150u && g < 120u;
}

bool is_smoke_cyan_particle_pixel(const std::uint8_t b, const std::uint8_t g, const std::uint8_t r) {
	return b > 140u && g > 140u && r < 130u;
}

bool is_smoke_green_particle_model_pixel(const std::uint8_t b, const std::uint8_t g, const std::uint8_t r) {
	return g > 140u && r < 140u && b < 160u;
}

bool is_smoke_blue_particle_model_pixel(const std::uint8_t b, const std::uint8_t g, const std::uint8_t r) {
	return b > 140u && r < 140u && g < 160u;
}

bool is_smoke_green_material_pixel(const std::uint8_t b, const std::uint8_t g, const std::uint8_t r) {
	return g > 150u && r < 140u && b < 160u;
}

bool is_smoke_blue_material_pixel(const std::uint8_t b, const std::uint8_t g, const std::uint8_t r) {
	return b > 110u && r < 140u && g < 170u;
}

bool validate_material_preview_pixels(
	const std::string_view label,
	const std::vector<std::uint8_t>& pixels,
	const int width,
	const int height
) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("Metal {} smoke failed: missing captured pixels.", label);
		return false;
	}

	MaterialPreviewPixelCounts counts;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4;
		count_material_preview_pixel(counts, pixels[offset + 2], pixels[offset + 1], pixels[offset]);
	}

	return validate_material_preview_counts("Metal", label, counts, width, height);
}

bool validate_multicell_material_preview_pixels(
	const std::vector<std::uint8_t>& pixels,
	const int width,
	const int height,
	const int logical_width,
	const int cell_size,
	const int header_height
) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || logical_width <= 0 || pixels.size() < pixel_count * 4) {
		std::println("Metal object-grid multicell preview smoke failed: missing captured pixels.");
		return false;
	}

	const double scale = static_cast<double>(width) / static_cast<double>(logical_width);
	const int first_left = 0;
	const int first_right = std::min(width, static_cast<int>(std::ceil(static_cast<double>(cell_size) * scale)));
	const int second_left = first_right;
	const int second_right = std::min(width, static_cast<int>(std::ceil(static_cast<double>(cell_size * 2) * scale)));
	const int top = std::clamp(static_cast<int>(std::floor(static_cast<double>(header_height) * scale)), 0, height);
	const int bottom = std::clamp(static_cast<int>(std::ceil(static_cast<double>(header_height + cell_size) * scale)), top, height);

	std::size_t first_green_pixels = 0;
	std::size_t first_blue_pixels = 0;
	std::size_t second_green_pixels = 0;
	std::size_t second_blue_pixels = 0;
	for (int y = top; y < bottom; ++y) {
		for (int x = first_left; x < first_right; ++x) {
			const std::size_t offset = (static_cast<std::size_t>(y) * static_cast<std::size_t>(width) + static_cast<std::size_t>(x)) * 4u;
			const std::uint8_t b = pixels[offset];
			const std::uint8_t g = pixels[offset + 1];
			const std::uint8_t r = pixels[offset + 2];
			if (is_smoke_green_material_pixel(b, g, r)) {
				++first_green_pixels;
			}
			if (is_smoke_blue_material_pixel(b, g, r)) {
				++first_blue_pixels;
			}
		}
		for (int x = second_left; x < second_right; ++x) {
			const std::size_t offset = (static_cast<std::size_t>(y) * static_cast<std::size_t>(width) + static_cast<std::size_t>(x)) * 4u;
			const std::uint8_t b = pixels[offset];
			const std::uint8_t g = pixels[offset + 1];
			const std::uint8_t r = pixels[offset + 2];
			if (is_smoke_green_material_pixel(b, g, r)) {
				++second_green_pixels;
			}
			if (is_smoke_blue_material_pixel(b, g, r)) {
				++second_blue_pixels;
			}
		}
	}

	const std::size_t region_pixels =
		static_cast<std::size_t>(std::max(0, first_right - first_left)) * static_cast<std::size_t>(std::max(0, bottom - top));
	const std::size_t min_color_pixels = std::max<std::size_t>(32, region_pixels / 256);
	if (first_green_pixels < min_color_pixels || second_blue_pixels < min_color_pixels
		|| first_blue_pixels > first_green_pixels || second_green_pixels > second_blue_pixels) {
		std::println(
			"Metal object-grid multicell preview smoke failed: first cell had {} green and {} blue pixels, second cell had {} green and {} blue pixels, expected first green and second blue with at least {} target pixels.",
			first_green_pixels,
			first_blue_pixels,
			second_green_pixels,
			second_blue_pixels,
			min_color_pixels
		);
		return false;
	}

	std::println(
		"Metal object-grid multicell preview smoke captured first cell {} green/{} blue pixels and second cell {} green/{} blue pixels.",
		first_green_pixels,
		first_blue_pixels,
		second_green_pixels,
		second_blue_pixels
	);
	return true;
}

bool validate_object_grid_load_failure_pixels(const std::vector<std::uint8_t>& pixels, const int width, const int height) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("Metal object-grid load-failure preview smoke failed: missing captured pixels.");
		return false;
	}

	std::size_t error_text_pixels = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4u;
		const std::uint8_t b = pixels[offset];
		const std::uint8_t g = pixels[offset + 1u];
		const std::uint8_t r = pixels[offset + 2u];
		if (r >= 90u && g <= 70u && b <= 70u) {
			++error_text_pixels;
		}
	}

	const std::size_t min_error_pixels = std::max<std::size_t>(12u, pixel_count / 20000u);
	if (error_text_pixels < min_error_pixels) {
		std::println(
			"Metal object-grid load-failure preview smoke failed: saw {} red error-marker pixels, expected at least {}.",
			error_text_pixels,
			min_error_pixels
		);
		return false;
	}

	std::println("Metal object-grid load-failure preview smoke captured {} red error-marker pixels.", error_text_pixels);
	return true;
}

bool validate_particle_preview_pixels(
	const std::string_view label,
	const std::vector<std::uint8_t>& pixels,
	const int width,
	const int height,
	const bool require_warm_particles = true,
	const bool require_pe1_tga = true,
	const bool require_spawned_child = true,
	const bool require_spawned_grandchild = true
) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("Metal {} smoke failed: missing captured pixels.", label);
		return false;
	}

	std::size_t warm_particle_pixels = 0;
	std::size_t pe1_tga_pixels = 0;
	std::size_t spawned_child_pixels = 0;
	std::size_t spawned_grandchild_pixels = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4;
		const std::uint8_t b = pixels[offset];
		const std::uint8_t g = pixels[offset + 1];
		const std::uint8_t r = pixels[offset + 2];
		if (is_smoke_red_orange_particle_pixel(b, g, r) || is_smoke_yellow_particle_pixel(b, g, r)) {
			++warm_particle_pixels;
		}
		if (is_smoke_magenta_particle_pixel(b, g, r)) {
			++pe1_tga_pixels;
		}
		if (is_smoke_green_particle_model_pixel(b, g, r)) {
			++spawned_child_pixels;
		}
		if (is_smoke_blue_particle_model_pixel(b, g, r)) {
			++spawned_grandchild_pixels;
		}
	}

	const std::size_t min_warm_pixels = std::max<std::size_t>(64, pixel_count / 5000);
	const std::size_t min_feature_pixels = std::max<std::size_t>(24, pixel_count / 12000);
	const bool missing_warm = require_warm_particles && warm_particle_pixels < min_warm_pixels;
	const bool missing_pe1_tga = require_pe1_tga && pe1_tga_pixels < min_feature_pixels;
	const bool missing_child = require_spawned_child && spawned_child_pixels < min_feature_pixels;
	const bool missing_grandchild = require_spawned_grandchild && spawned_grandchild_pixels < min_feature_pixels;
	if (missing_warm || missing_pe1_tga || missing_child || missing_grandchild) {
		std::println(
			"Metal {} smoke failed: saw {} warm PE2, {} magenta PE1 TGA, {} green spawned-child, and {} blue spawned-grandchild pixels; expected at least {}, {}, {}, and {} for required buckets.",
			label,
			warm_particle_pixels,
			pe1_tga_pixels,
			spawned_child_pixels,
			spawned_grandchild_pixels,
			min_warm_pixels,
			min_feature_pixels,
			min_feature_pixels,
			min_feature_pixels
		);
		return false;
	}

	std::println(
		"Metal {} smoke captured {} warm PE2, {} magenta PE1 TGA, {} green spawned-child, and {} blue spawned-grandchild pixels.",
		label,
		warm_particle_pixels,
		pe1_tga_pixels,
		spawned_child_pixels,
		spawned_grandchild_pixels
	);
	return true;
}

bool validate_ribbon_preview_pixels(
	const std::string_view label,
	const std::vector<std::uint8_t>& pixels,
	const int width,
	const int height
) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("Metal {} smoke failed: missing captured pixels.", label);
		return false;
	}

	std::size_t ribbon_pixels = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4;
		const std::uint8_t b = pixels[offset];
		const std::uint8_t g = pixels[offset + 1];
		const std::uint8_t r = pixels[offset + 2];
		if (is_smoke_cyan_particle_pixel(b, g, r)) {
			++ribbon_pixels;
		}
	}

	const std::size_t min_ribbon_pixels = std::max<std::size_t>(24, pixel_count / 12000);
	if (ribbon_pixels < min_ribbon_pixels) {
		std::println(
			"Metal {} smoke failed: saw {} cyan ribbon pixels, expected at least {}.",
			label,
			ribbon_pixels,
			min_ribbon_pixels
		);
		return false;
	}

	std::println("Metal {} smoke captured {} cyan ribbon pixels.", label, ribbon_pixels);
	return true;
}

bool validate_terrain_material_smoke_pixels(const std::vector<std::uint8_t>& pixels, const int width, const int height) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("Metal terrain material/cliff smoke failed: missing captured pixels.");
		return false;
	}

	std::size_t red_ground_pixels = 0;
	std::size_t green_ground_pixels = 0;
	std::size_t blue_cliff_pixels = 0;
	std::size_t magenta_cliff_pixels = 0;
	std::size_t cyan_water_pixels = 0;
	std::size_t translucent_cyan_water_pixels = 0;
	std::size_t warm_particle_pixels = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4;
		const std::uint8_t b = pixels[offset + 0];
		const std::uint8_t g = pixels[offset + 1];
		const std::uint8_t r = pixels[offset + 2];
		const std::uint8_t a = pixels[offset + 3];
		if (r > 150u && g < 100u && b < 100u) {
			++red_ground_pixels;
		}
		if (g > 150u && r < 100u && b < 100u) {
			++green_ground_pixels;
		}
		if (b > 140u && g > 40u && g < 180u && r < 100u) {
			++blue_cliff_pixels;
		}
		if (r > 150u && b > 150u && g < 100u) {
			++magenta_cliff_pixels;
		}
		if (g > 150u && b > 150u && r < 80u) {
			++cyan_water_pixels;
			if (a < 252u) {
				++translucent_cyan_water_pixels;
			}
		}
		if (is_smoke_red_orange_particle_pixel(b, g, r) || is_smoke_yellow_particle_pixel(b, g, r)) {
			++warm_particle_pixels;
		}
	}

	const std::size_t min_ground_layer_pixels = std::max<std::size_t>(512, pixel_count / 128);
	const std::size_t min_cliff_layer_pixels = std::max<std::size_t>(256, pixel_count / 256);
	const std::size_t min_water_pixels = std::max<std::size_t>(256, pixel_count / 128);
	const std::size_t min_translucent_water_pixels = std::max<std::size_t>(128, min_water_pixels / 2);
	const std::size_t min_particle_pixels = std::max<std::size_t>(256, pixel_count / 256);
	if (red_ground_pixels < min_ground_layer_pixels || green_ground_pixels < min_ground_layer_pixels
		|| blue_cliff_pixels < min_cliff_layer_pixels || magenta_cliff_pixels < min_cliff_layer_pixels
		|| cyan_water_pixels < min_water_pixels || translucent_cyan_water_pixels < min_translucent_water_pixels
		|| warm_particle_pixels < min_particle_pixels) {
		std::println(
			"Metal terrain material/cliff smoke failed: saw {} red ground, {} green ground, {} blue cliff, {} magenta cliff, {} cyan water, {} translucent cyan water, and {} warm particle pixels, expected at least {}, {}, {}, {}, {}, {}, and {}.",
			red_ground_pixels,
			green_ground_pixels,
			blue_cliff_pixels,
			magenta_cliff_pixels,
			cyan_water_pixels,
			translucent_cyan_water_pixels,
			warm_particle_pixels,
			min_ground_layer_pixels,
			min_ground_layer_pixels,
			min_cliff_layer_pixels,
			min_cliff_layer_pixels,
			min_water_pixels,
			min_translucent_water_pixels,
			min_particle_pixels
		);
		return false;
	}

	std::println(
		"Metal terrain material/cliff smoke captured {}x{} with {} red ground, {} green ground, {} blue cliff, {} magenta cliff, {} cyan water, {} translucent cyan water, and {} warm particle pixels.",
		width,
		height,
		red_ground_pixels,
		green_ground_pixels,
		blue_cliff_pixels,
		magenta_cliff_pixels,
		cyan_water_pixels,
		translucent_cyan_water_pixels,
		warm_particle_pixels
	);
	return true;
}

bool validate_terrain_material_fallback_pixels(const std::vector<std::uint8_t>& pixels, const int width, const int height) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("Metal terrain material/cliff smoke failed: missing fallback captured pixels.");
		return false;
	}

	std::size_t blue_cliff_pixels = 0;
	std::size_t magenta_cliff_pixels = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4;
		const std::uint8_t b = pixels[offset + 0];
		const std::uint8_t g = pixels[offset + 1];
		const std::uint8_t r = pixels[offset + 2];
		if (b > 140u && g > 40u && g < 180u && r < 100u) {
			++blue_cliff_pixels;
		}
		if (r > 150u && b > 150u && g < 100u) {
			++magenta_cliff_pixels;
		}
	}

	const std::size_t max_real_cliff_pixels = std::max<std::size_t>(16, pixel_count / 4096);
	if (blue_cliff_pixels > max_real_cliff_pixels || magenta_cliff_pixels > max_real_cliff_pixels) {
		std::println(
			"Metal terrain material/cliff smoke failed: fallback frame still contained {} blue and {} magenta real-cliff pixels after real textures were removed.",
			blue_cliff_pixels,
			magenta_cliff_pixels
		);
		return false;
	}

	std::println(
		"Metal terrain material/cliff fallback captured {}x{} with {} blue and {} magenta real-cliff pixels.",
		width,
		height,
		blue_cliff_pixels,
		magenta_cliff_pixels
	);
	return true;
}

bool render_terrain_material_smoke_frame(
	MetalSurface& surface,
	hive::metal::MetalMapRenderer& renderer,
	QWidget& widget,
	std::vector<std::uint8_t>& captured_bgra,
	int& captured_width,
	int& captured_height
) {
	if (!surface.initialize(reinterpret_cast<void*>(widget.winId()))) {
		std::println("Metal terrain material/cliff smoke failed: unable to initialize Metal surface.");
		return false;
	}

	surface.resize(widget.width(), widget.height(), k_metal_smoke_artifact_scale);

	hive::metal::TerrainTextureCpuFrame textures;
	textures.revision = 0;
	textures.ground_tile_size = 2;
	textures.ground_layer_count = 2;
	textures.ground_texture_layer_offsets = {0u};
	textures.ground_rgba8 = {
		242u,
		18u,
		10u,
		255u,
		230u,
		32u,
		20u,
		255u,
		250u,
		20u,
		24u,
		255u,
		235u,
		42u,
		16u,
		255u,
		20u,
		235u,
		28u,
		255u,
		36u,
		220u,
		40u,
		255u,
		24u,
		245u,
		32u,
		255u,
		48u,
		230u,
		50u,
		255u,
	};
	textures.cliff_width = 2;
	textures.cliff_height = 2;
	textures.cliff_layer_count = 2;
	textures.cliff_rgba8 = {
		8u,
		96u,
		245u,
		255u,
		14u,
		120u,
		255u,
		255u,
		20u,
		80u,
		220u,
		255u,
		10u,
		140u,
		245u,
		255u,
		245u,
		20u,
		245u,
		255u,
		220u,
		28u,
		235u,
		255u,
		255u,
		36u,
		230u,
		255u,
		235u,
		24u,
		255u,
		255u,
	};
	textures.water_width = 2;
	textures.water_height = 2;
	textures.water_layer_count = 2;
	textures.water_rgba8 = {
		255u,
		20u,
		20u,
		255u,
		235u,
		25u,
		30u,
		255u,
		245u,
		30u,
		20u,
		255u,
		255u,
		40u,
		35u,
		255u,
		0u,
		255u,
		255u,
		255u,
		0u,
		235u,
		255u,
		255u,
		0u,
		255u,
		235u,
		255u,
		0u,
		245u,
		245u,
		255u,
	};

	std::vector<glm::vec3> cliff_vertices {
		glm::vec3(-19.2f, 19.2f, 12.0f),
		glm::vec3(-19.2f, 108.8f, 12.0f),
		glm::vec3(-108.8f, 19.2f, 12.0f),
	};
	std::vector<glm::vec2> cliff_uvs {
		glm::vec2(0.0f, 1.0f),
		glm::vec2(1.0f, 1.0f),
		glm::vec2(0.0f, 0.0f),
	};
	std::vector<glm::vec3> cliff_normals {
		glm::vec3(0.0f, 0.0f, 1.0f),
		glm::vec3(0.0f, 0.0f, 1.0f),
		glm::vec3(0.0f, 0.0f, 1.0f),
	};
	std::vector<std::uint16_t> cliff_indices {0u, 1u, 2u};
	static constexpr std::uint8_t smoke_cliff_mesh_key = 0;

	hive::metal::MetalMapFrame frame;
	frame.camera.projection_view = hive::metal::opengl_clip_depth_to_metal(glm::ortho(-0.25f, 2.25f, -0.25f, 2.25f, -1.0f, 1.0f));
	frame.terrain.width = 3;
	frame.terrain.height = 3;
	frame.terrain.corner_ground_heights = {
		0.0f,
		0.0f,
		0.0f,
		0.0f,
		0.0f,
		0.0f,
		0.0f,
		0.0f,
		0.0f,
	};
	frame.terrain.final_ground_heights = frame.terrain.corner_ground_heights;
	// The transparent sentinel layer exercises the material skip path; the opaque sampled layer keeps the expected color stable.
	frame.terrain.ground_texture_indices = {
		glm::uvec4(0u, 0u, 0u, 0u),
		glm::uvec4(0u, 0xFFFFu, 0u, (1u << 16u)),
		glm::uvec4(0u, 0u, 0u, (1u << 16u)),
		glm::uvec4(0u, 0u, 0u, 0u),
	};
	frame.terrain.ground_exists = {1u, 1u, 1u, 0u};
	frame.terrain.corner_water_heights = {
		-0.25f,
		-0.25f,
		-0.25f,
		-0.25f,
		-0.25f,
		-0.25f,
		-0.25f,
		-0.25f,
		-0.25f,
	};
	frame.terrain.water_exists = {0u, 0u, 0u, 0u, 0u, 0u, 0u, 0u, 1u};
	frame.textures = &textures;
	frame.cliffs.meshes.push_back(hive::metal::CliffMeshCpuFrame {
		.key = &smoke_cliff_mesh_key,
		.revision = 1,
		.vertices = &cliff_vertices,
		.uvs = &cliff_uvs,
		.normals = &cliff_normals,
		.indices = &cliff_indices,
	});
	frame.cliffs.instances = {
		glm::vec4(0.0f, 0.0f, 0.0f, 0.0f),
		glm::vec4(1.0f, 0.0f, 0.0f, 1.0f),
	};
	frame.cliffs.draws.push_back(hive::metal::CliffDrawCpuFrame {
		.mesh_index = 0,
		.instance_offset = 0,
		.instance_count = 2,
		.index_count = static_cast<std::uint32_t>(cliff_indices.size()),
	});

	static constexpr std::uint8_t smoke_particle_texture_key = 1;
	const std::vector<std::uint8_t> smoke_particle_texture {
		255u,
		255u,
		255u,
		255u,
	};
	frame.particles.positions = {
		glm::vec4(0.20f, 0.20f, 0.50f, 1.0f),
		glm::vec4(0.95f, 0.20f, 0.50f, 1.0f),
		glm::vec4(0.20f, 0.95f, 0.50f, 1.0f),
		glm::vec4(0.95f, 0.20f, 0.50f, 1.0f),
		glm::vec4(0.95f, 0.95f, 0.50f, 1.0f),
		glm::vec4(0.20f, 0.95f, 0.50f, 1.0f),
	};
	frame.particles.uvs = {
		glm::vec2(0.0f, 1.0f),
		glm::vec2(1.0f, 1.0f),
		glm::vec2(0.0f, 0.0f),
		glm::vec2(1.0f, 1.0f),
		glm::vec2(1.0f, 0.0f),
		glm::vec2(0.0f, 0.0f),
	};
	frame.particles.colors = {
		glm::vec4(1.0f, 0.25f, 0.10f, 1.0f),
		glm::vec4(1.0f, 0.45f, 0.05f, 1.0f),
		glm::vec4(0.90f, 0.05f, 0.15f, 1.0f),
		glm::vec4(1.0f, 0.45f, 0.05f, 1.0f),
		glm::vec4(1.0f, 0.85f, 0.15f, 1.0f),
		glm::vec4(0.90f, 0.05f, 0.15f, 1.0f),
	};
	frame.particles.textures.push_back(hive::metal::ModelTextureCpuSource {
		.key = &smoke_particle_texture_key,
		.width = 1,
		.height = 1,
		.channels = 4,
		.wrap_flags = 0,
		.srgb = false,
		.rgba8 = &smoke_particle_texture,
	});
	frame.particles.draws.push_back(hive::metal::ParticleEmitter2DrawCpuFrame {
		.first_vertex = 0,
		.vertex_count = static_cast<std::uint32_t>(frame.particles.positions.size()),
		.texture_index = 0,
		.filter_mode = 0,
	});

	frame.show_lighting = false;
	frame.show_water = true;
	frame.water_texture_index = 1;
	frame.shallow_color_min = glm::vec4(1.0f, 1.0f, 1.0f, 0.95f);
	frame.shallow_color_max = frame.shallow_color_min;
	frame.deep_color_min = frame.shallow_color_min;
	frame.deep_color_max = frame.shallow_color_min;

	auto render_once = [&](const bool capture) {
		MetalSurface::Frame surface_frame = surface.begin_frame(0.02f, 0.02f, 0.02f, 1.0f);
		if (!surface_frame) {
			std::println("Metal terrain material/cliff smoke failed: unable to begin Metal frame.");
			return false;
		}

		hive::metal::FrameContext frame_context {
			.device = surface_frame.device_opaque(),
			.command_queue = surface_frame.command_queue_opaque(),
			.command_buffer = surface_frame.command_buffer_opaque(),
			.render_pass_descriptor = surface_frame.render_pass_descriptor_opaque(),
			.drawable_width = surface_frame.drawable_width(),
			.drawable_height = surface_frame.drawable_height(),
			.scale = surface_frame.scale(),
		};

		bool render_ok = true;
		if (!renderer.render(frame_context, frame)) {
			std::println("Metal terrain material/cliff smoke failed: {}", renderer.last_error());
			render_ok = false;
		}

		std::vector<std::uint8_t> frame_pixels;
		const bool present_ok = capture ? surface.end_frame(std::move(surface_frame), &frame_pixels)
										: surface.end_frame(std::move(surface_frame));
		if (!present_ok) {
			std::println("Metal terrain material/cliff smoke failed: unable to present/read back frame.");
			return false;
		}

		if (capture) {
			captured_bgra = std::move(frame_pixels);
			captured_width = frame_context.drawable_width;
			captured_height = frame_context.drawable_height;
		}
		return render_ok;
	};

	frame.textures = nullptr;
	if (!render_once(false)) {
		return false;
	}

	frame.textures = &textures;
	if (!render_once(true)) {
		return false;
	}
	std::vector<std::uint8_t> real_texture_bgra = captured_bgra;
	const int real_texture_width = captured_width;
	const int real_texture_height = captured_height;

	frame.textures = nullptr;
	if (!render_once(true) || !validate_terrain_material_fallback_pixels(captured_bgra, captured_width, captured_height)) {
		return false;
	}

	captured_bgra = std::move(real_texture_bgra);
	captured_width = real_texture_width;
	captured_height = real_texture_height;
	return true;
}

bool run_metal_terrain_material_smoke_impl(QWidget* parent, const QString& artifact_directory) {
	MetalSmokeSurfaceWidget terrain_preview(parent);
	terrain_preview.setAttribute(Qt::WA_NativeWindow);
	terrain_preview.setAttribute(Qt::WA_PaintOnScreen);
	terrain_preview.setAttribute(Qt::WA_NoSystemBackground);
	terrain_preview.setAttribute(Qt::WA_OpaquePaintEvent);
	terrain_preview.resize(256, 256);

	MetalSurface surface;
	hive::metal::MetalMapRenderer renderer;
	std::vector<std::uint8_t> pixels;
	int width = 0;
	int height = 0;
	const bool frame_ok = capture_metal_preview_widget(
		"terrain material/cliff renderer",
		terrain_preview,
		[&](std::vector<std::uint8_t>& captured_pixels, int& captured_width, int& captured_height) {
			const bool rendered = render_terrain_material_smoke_frame(surface, renderer, terrain_preview, captured_pixels, captured_width, captured_height);
			pixels = captured_pixels;
			width = captured_width;
			height = captured_height;
			return rendered;
		},
		smoke_clear_color(0.02f, 0.02f, 0.02f)
	);

	const bool pixels_ok = frame_ok && validate_terrain_material_smoke_pixels(pixels, width, height);
	return pixels_ok && save_metal_smoke_artifact_png(artifact_directory, "metal-map-terrain-cliffs-particles.png", pixels, width, height);
}

bool validate_pathing_overlay_smoke_pixels(const std::vector<std::uint8_t>& pixels, const int width, const int height) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("Metal pathing overlay smoke failed: missing captured pixels.");
		return false;
	}

	std::size_t red_pathing_pixels = 0;
	std::size_t green_pathing_pixels = 0;
	std::size_t blue_pathing_pixels = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4;
		const std::uint8_t b = pixels[offset + 0];
		const std::uint8_t g = pixels[offset + 1];
		const std::uint8_t r = pixels[offset + 2];
		if (r > 90u && g < 50u && b < 50u) {
			++red_pathing_pixels;
		}
		if (g > 90u && r < 50u && b < 50u) {
			++green_pathing_pixels;
		}
		if (b > 90u && r < 50u && g < 50u) {
			++blue_pathing_pixels;
		}
	}

	const std::size_t min_pixels = std::max<std::size_t>(256, pixel_count / 32);
	if (red_pathing_pixels < min_pixels || green_pathing_pixels < min_pixels || blue_pathing_pixels < min_pixels) {
		std::println(
			"Metal pathing overlay smoke failed: saw {} red, {} green, and {} blue pathing pixels, expected at least {} of each.",
			red_pathing_pixels,
			green_pathing_pixels,
			blue_pathing_pixels,
			min_pixels
		);
		return false;
	}

	std::println(
		"Metal pathing overlay smoke captured {}x{} with {} red, {} green, and {} blue pathing pixels.",
		width,
		height,
		red_pathing_pixels,
		green_pathing_pixels,
		blue_pathing_pixels
	);
	return true;
}

bool run_metal_pathing_overlay_smoke_impl(QWidget* parent, const QString& artifact_directory) {
	MetalSmokeSurfaceWidget pathing_preview(parent);
	pathing_preview.setAttribute(Qt::WA_NativeWindow);
	pathing_preview.setAttribute(Qt::WA_PaintOnScreen);
	pathing_preview.setAttribute(Qt::WA_NoSystemBackground);
	pathing_preview.setAttribute(Qt::WA_OpaquePaintEvent);
	pathing_preview.resize(256, 256);

	MetalSurface surface;
	hive::metal::MetalMapRenderer renderer;
	std::vector<std::uint8_t> pixels;
	int width = 0;
	int height = 0;
	const bool frame_ok = capture_metal_preview_widget(
		"pathing overlay renderer",
		pathing_preview,
		[&](std::vector<std::uint8_t>& captured_pixels, int& captured_width, int& captured_height) {
			if (!surface.initialize(reinterpret_cast<void*>(pathing_preview.winId()))) {
				std::println("Metal pathing overlay smoke failed: unable to initialize Metal surface.");
				return false;
			}
			surface.resize(pathing_preview.width(), pathing_preview.height(), k_metal_smoke_artifact_scale);
			MetalSurface::Frame surface_frame = surface.begin_frame(0.0f, 0.0f, 0.0f, 1.0f);
			if (!surface_frame) {
				std::println("Metal pathing overlay smoke failed: unable to begin Metal frame.");
				return false;
			}

			hive::metal::TerrainTextureCpuFrame textures;
			textures.ground_tile_size = 1;
			textures.ground_layer_count = 1;
			textures.ground_texture_layer_offsets = {0u};
			textures.ground_rgba8 = {0u, 0u, 0u, 255u};

			hive::metal::MetalMapFrame frame;
			frame.camera.projection_view = hive::metal::opengl_clip_depth_to_metal(glm::ortho(0.0f, 1.0f, 0.0f, 1.0f, -1.0f, 1.0f));
			frame.terrain.width = 2;
			frame.terrain.height = 2;
			frame.terrain.corner_ground_heights = {0.0f, 0.0f, 0.0f, 0.0f};
			frame.terrain.final_ground_heights = frame.terrain.corner_ground_heights;
			frame.terrain.ground_texture_indices = {glm::uvec4(0u)};
			frame.terrain.ground_exists = {1u};
			frame.terrain.corner_water_heights = frame.terrain.corner_ground_heights;
			frame.terrain.water_exists = {0u, 0u, 0u, 0u};
			frame.pathing.width = 4;
			frame.pathing.height = 4;
			frame.pathing.cells_static.assign(16, 0u);
			frame.pathing.cells_dynamic.assign(16, 0u);
			for (int y = 0; y < frame.pathing.height; ++y) {
				for (int x = 0; x < frame.pathing.width; ++x) {
					const std::size_t index = static_cast<std::size_t>(y * frame.pathing.width + x);
					if (x < 2 && y < 2) {
						frame.pathing.cells_static[index] = 0x2u;
					} else if (x >= 2 && y < 2) {
						frame.pathing.cells_dynamic[index] = 0x4u;
					} else if (x < 2) {
						frame.pathing.cells_static[index] = 0x8u;
					}
				}
			}
			frame.textures = &textures;
			frame.show_pathing = true;
			frame.show_lighting = false;
			frame.show_water = false;

			hive::metal::FrameContext frame_context {
				.device = surface_frame.device_opaque(),
				.command_queue = surface_frame.command_queue_opaque(),
				.command_buffer = surface_frame.command_buffer_opaque(),
				.render_pass_descriptor = surface_frame.render_pass_descriptor_opaque(),
				.drawable_width = surface_frame.drawable_width(),
				.drawable_height = surface_frame.drawable_height(),
				.scale = surface_frame.scale(),
			};

			bool render_ok = true;
			if (!renderer.render(frame_context, frame)) {
				std::println("Metal pathing overlay smoke failed: {}", renderer.last_error());
				render_ok = false;
			}

			std::vector<std::uint8_t> frame_pixels;
			if (!surface.end_frame(std::move(surface_frame), &frame_pixels)) {
				std::println("Metal pathing overlay smoke failed: unable to present/read back frame.");
				return false;
			}
			captured_pixels = std::move(frame_pixels);
			captured_width = frame_context.drawable_width;
			captured_height = frame_context.drawable_height;
			pixels = captured_pixels;
			width = captured_width;
			height = captured_height;
			return render_ok;
		},
		smoke_clear_color(0.0f, 0.0f, 0.0f)
	);

	const bool pixels_ok = frame_ok && validate_pathing_overlay_smoke_pixels(pixels, width, height);
	return pixels_ok && save_metal_smoke_artifact_png(artifact_directory, "metal-map-pathing-overlay.png", pixels, width, height);
}

bool validate_cliff_pathing_overlay_smoke_pixels(const std::vector<std::uint8_t>& pixels, const int width, const int height) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("Metal cliff pathing overlay smoke failed: missing captured pixels.");
		return false;
	}

	std::size_t dynamic_green_cliff_pixels = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4;
		const std::uint8_t b = pixels[offset + 0];
		const std::uint8_t g = pixels[offset + 1];
		const std::uint8_t r = pixels[offset + 2];
		if (g > 90u && r < 50u && b < 50u) {
			++dynamic_green_cliff_pixels;
		}
	}

	const std::size_t min_pixels = std::max<std::size_t>(256, pixel_count / 128);
	if (dynamic_green_cliff_pixels < min_pixels) {
		std::println(
			"Metal cliff pathing overlay smoke failed: saw {} dynamic-green cliff pixels, expected at least {}.",
			dynamic_green_cliff_pixels,
			min_pixels
		);
		return false;
	}

	std::println(
		"Metal cliff pathing overlay smoke captured {}x{} with {} dynamic-green cliff pixels.",
		width,
		height,
		dynamic_green_cliff_pixels
	);
	return true;
}

bool run_metal_cliff_pathing_overlay_smoke_impl(QWidget* parent, const QString& artifact_directory) {
	MetalSmokeSurfaceWidget cliff_pathing_preview(parent);
	cliff_pathing_preview.setAttribute(Qt::WA_NativeWindow);
	cliff_pathing_preview.setAttribute(Qt::WA_PaintOnScreen);
	cliff_pathing_preview.setAttribute(Qt::WA_NoSystemBackground);
	cliff_pathing_preview.setAttribute(Qt::WA_OpaquePaintEvent);
	cliff_pathing_preview.resize(256, 256);

	MetalSurface surface;
	hive::metal::MetalMapRenderer renderer;
	std::vector<std::uint8_t> pixels;
	int width = 0;
	int height = 0;
	const bool frame_ok = capture_metal_preview_widget(
		"cliff pathing overlay renderer",
		cliff_pathing_preview,
		[&](std::vector<std::uint8_t>& captured_pixels, int& captured_width, int& captured_height) {
			if (!surface.initialize(reinterpret_cast<void*>(cliff_pathing_preview.winId()))) {
				std::println("Metal cliff pathing overlay smoke failed: unable to initialize Metal surface.");
				return false;
			}
			surface.resize(cliff_pathing_preview.width(), cliff_pathing_preview.height(), k_metal_smoke_artifact_scale);
			MetalSurface::Frame surface_frame = surface.begin_frame(0.0f, 0.0f, 0.0f, 1.0f);
			if (!surface_frame) {
				std::println("Metal cliff pathing overlay smoke failed: unable to begin Metal frame.");
				return false;
			}

			hive::metal::TerrainTextureCpuFrame textures;
			textures.ground_tile_size = 1;
			textures.ground_layer_count = 1;
			textures.ground_texture_layer_offsets = {0u};
			textures.ground_rgba8 = {0u, 0u, 0u, 255u};
			textures.cliff_width = 1;
			textures.cliff_height = 1;
			textures.cliff_layer_count = 1;
			textures.cliff_rgba8 = {0u, 0u, 0u, 255u};

			std::vector<glm::vec3> cliff_vertices {
				glm::vec3(-19.2f, 19.2f, 16.0f),
				glm::vec3(-19.2f, 108.8f, 16.0f),
				glm::vec3(-108.8f, 19.2f, 16.0f),
			};
			std::vector<glm::vec2> cliff_uvs {
				glm::vec2(0.0f, 1.0f),
				glm::vec2(1.0f, 1.0f),
				glm::vec2(0.0f, 0.0f),
			};
			std::vector<glm::vec3> cliff_normals {
				glm::vec3(0.0f, 0.0f, 1.0f),
				glm::vec3(0.0f, 0.0f, 1.0f),
				glm::vec3(0.0f, 0.0f, 1.0f),
			};
			std::vector<std::uint16_t> cliff_indices {0u, 1u, 2u};
			static constexpr std::uint8_t cliff_pathing_mesh_key = 0;

			hive::metal::MetalMapFrame frame;
			frame.camera.projection_view = hive::metal::opengl_clip_depth_to_metal(glm::ortho(0.0f, 1.0f, 0.0f, 1.0f, -1.0f, 1.0f));
			frame.terrain.width = 2;
			frame.terrain.height = 2;
			frame.terrain.corner_ground_heights = {0.0f, 0.0f, 0.0f, 0.0f};
			frame.terrain.final_ground_heights = frame.terrain.corner_ground_heights;
			frame.terrain.ground_texture_indices = {glm::uvec4(0u)};
			frame.terrain.ground_exists = {0u};
			frame.terrain.corner_water_heights = frame.terrain.corner_ground_heights;
			frame.terrain.water_exists = {0u, 0u, 0u, 0u};
			frame.pathing.width = 4;
			frame.pathing.height = 4;
			frame.pathing.cells_static.assign(16, 0u);
			frame.pathing.cells_dynamic.assign(16, 0x4u);
			frame.textures = &textures;
			frame.cliffs.meshes.push_back(hive::metal::CliffMeshCpuFrame {
				.key = &cliff_pathing_mesh_key,
				.revision = 1,
				.vertices = &cliff_vertices,
				.uvs = &cliff_uvs,
				.normals = &cliff_normals,
				.indices = &cliff_indices,
			});
			frame.cliffs.instances = {glm::vec4(0.0f, 0.0f, 0.0f, 0.0f)};
			frame.cliffs.draws.push_back(hive::metal::CliffDrawCpuFrame {
				.mesh_index = 0,
				.instance_offset = 0,
				.instance_count = 1,
				.index_count = static_cast<std::uint32_t>(cliff_indices.size()),
			});
			frame.show_pathing = true;
			frame.show_lighting = false;
			frame.show_water = false;

			hive::metal::FrameContext frame_context {
				.device = surface_frame.device_opaque(),
				.command_queue = surface_frame.command_queue_opaque(),
				.command_buffer = surface_frame.command_buffer_opaque(),
				.render_pass_descriptor = surface_frame.render_pass_descriptor_opaque(),
				.drawable_width = surface_frame.drawable_width(),
				.drawable_height = surface_frame.drawable_height(),
				.scale = surface_frame.scale(),
			};

			bool render_ok = true;
			if (!renderer.render(frame_context, frame)) {
				std::println("Metal cliff pathing overlay smoke failed: {}", renderer.last_error());
				render_ok = false;
			}

			std::vector<std::uint8_t> frame_pixels;
			if (!surface.end_frame(std::move(surface_frame), &frame_pixels)) {
				std::println("Metal cliff pathing overlay smoke failed: unable to present/read back frame.");
				return false;
			}
			captured_pixels = std::move(frame_pixels);
			captured_width = frame_context.drawable_width;
			captured_height = frame_context.drawable_height;
			pixels = captured_pixels;
			width = captured_width;
			height = captured_height;
			return render_ok;
		},
		smoke_clear_color(0.0f, 0.0f, 0.0f)
	);

	const bool pixels_ok = frame_ok && validate_cliff_pathing_overlay_smoke_pixels(pixels, width, height);
	return pixels_ok && save_metal_smoke_artifact_png(artifact_directory, "metal-cliff-pathing-overlay.png", pixels, width, height);
}

std::uint32_t pack_smoke_model_uv(const glm::vec2 uv) {
	return glm::packSnorm2x16((uv + 1.0f) / 8.0f);
}

std::uint32_t pack_smoke_model_normal(const glm::vec3 normal) {
	return glm::packSnorm2x16(float32x3_to_oct(glm::normalize(normal)));
}

bool validate_model_material_smoke_pixels(const std::vector<std::uint8_t>& pixels, const int width, const int height) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("Metal model material smoke failed: missing captured pixels.");
		return false;
	}

	std::size_t sd_team_pixels = 0;
	std::size_t hd_material_pixels = 0;
	std::size_t transparent_sd_pixels = 0;
	std::size_t transparent_hd_pixels = 0;
	std::size_t alpha_key_pass_pixels = 0;
	std::size_t alpha_key_rejected_pixels = 0;
	std::size_t no_depth_probe_pixels = 0;
	std::size_t hd_normal_pixels = 0;
	std::size_t hd_team_mask_pixels = 0;
	std::size_t hd_emissive_pixels = 0;
	std::size_t hd_fresnel_pixels = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4;
		const std::uint8_t b = pixels[offset + 0];
		const std::uint8_t g = pixels[offset + 1];
		const std::uint8_t r = pixels[offset + 2];
		if (r > 150u && g < 80u && b < 80u) {
			++sd_team_pixels;
		}
		if (g > 150u && r < 80u && b < 80u) {
			++hd_material_pixels;
		}
		if (b > 110u && r < 80u && g < 120u) {
			++transparent_sd_pixels;
		}
		if (r > 110u && b > 110u && g < 100u) {
			++transparent_hd_pixels;
		}
		if (g > 150u && b > 150u && r < 80u) {
			++alpha_key_pass_pixels;
		}
		if (r > 150u && g > 150u && b < 80u) {
			++alpha_key_rejected_pixels;
		}
		if (r > 200u && g > 90u && g < 170u && b < 80u) {
			++no_depth_probe_pixels;
		}
		if (r > 20u && r < 80u && g < 35u && b < 35u) {
			++hd_normal_pixels;
		}
		if (r > 55u && r < 125u && g < 45u && b > 95u && b < 170u) {
			++hd_team_mask_pixels;
		}
		if (b > 200u && r < 45u && g < 45u) {
			++hd_emissive_pixels;
		}
		if (r > 90u && r < 150u && g > 160u && b > 60u && b < 120u) {
			++hd_fresnel_pixels;
		}
	}

	const std::size_t min_pixels = std::max<std::size_t>(96, pixel_count / 400);
	if (sd_team_pixels < min_pixels || hd_material_pixels < min_pixels || transparent_sd_pixels < min_pixels
		|| transparent_hd_pixels < min_pixels || alpha_key_pass_pixels < min_pixels || alpha_key_rejected_pixels > 8u
		|| no_depth_probe_pixels < min_pixels || hd_normal_pixels < min_pixels || hd_team_mask_pixels < min_pixels
		|| hd_emissive_pixels < min_pixels || hd_fresnel_pixels < min_pixels) {
		std::println(
			"Metal model material smoke failed: saw SD team {}, HD material {}, transparent SD {}, transparent HD {}, alpha-key pass {}, rejected {}, no-depth probe {}, HD normal {}, HD team-mask {}, HD emissive {}, and HD fresnel {} pixels; expected at least {} for rendered classes and at most 8 rejected pixels.",
			sd_team_pixels,
			hd_material_pixels,
			transparent_sd_pixels,
			transparent_hd_pixels,
			alpha_key_pass_pixels,
			alpha_key_rejected_pixels,
			no_depth_probe_pixels,
			hd_normal_pixels,
			hd_team_mask_pixels,
			hd_emissive_pixels,
			hd_fresnel_pixels,
			min_pixels
		);
		return false;
	}

	std::println(
		"Metal model material smoke captured {}x{} with {} SD team, {} HD material, {} transparent SD, {} transparent HD, {} alpha-key pass, {} rejected alpha-key, {} no-depth probe, {} HD normal, {} HD team-mask, {} HD emissive, and {} HD fresnel pixels.",
		width,
		height,
		sd_team_pixels,
		hd_material_pixels,
		transparent_sd_pixels,
		transparent_hd_pixels,
		alpha_key_pass_pixels,
		alpha_key_rejected_pixels,
		no_depth_probe_pixels,
		hd_normal_pixels,
		hd_team_mask_pixels,
		hd_emissive_pixels,
		hd_fresnel_pixels
	);
	return true;
}

bool render_model_material_smoke_frame(
	MetalSurface& surface,
	hive::metal::MetalMapRenderer& renderer,
	QWidget& widget,
	std::vector<std::uint8_t>& captured_bgra,
	int& captured_width,
	int& captured_height
) {
	if (!surface.initialize(reinterpret_cast<void*>(widget.winId()))) {
		std::println("Metal model material smoke failed: unable to initialize Metal surface.");
		return false;
	}

	surface.resize(widget.width(), widget.height(), k_metal_smoke_artifact_scale);
	MetalSurface::Frame surface_frame = surface.begin_frame(0.04f, 0.04f, 0.04f, 1.0f);
	if (!surface_frame) {
		std::println("Metal model material smoke failed: unable to begin Metal frame.");
		return false;
	}

	hive::metal::FrameContext frame_context {
		.device = surface_frame.device_opaque(),
		.command_queue = surface_frame.command_queue_opaque(),
		.command_buffer = surface_frame.command_buffer_opaque(),
		.render_pass_descriptor = surface_frame.render_pass_descriptor_opaque(),
		.drawable_width = surface_frame.drawable_width(),
		.drawable_height = surface_frame.drawable_height(),
		.scale = surface_frame.scale(),
	};

	std::vector<glm::uvec2> vertices {
		pack_vec3_to_uvec2(glm::vec3(-0.18f, -0.18f, 0.5f), 8192.0f),
		pack_vec3_to_uvec2(glm::vec3(0.18f, -0.18f, 0.5f), 8192.0f),
		pack_vec3_to_uvec2(glm::vec3(-0.18f, 0.18f, 0.5f), 8192.0f),
	};
	std::vector<std::uint32_t> uvs {
		pack_smoke_model_uv(glm::vec2(0.0f, 1.0f)),
		pack_smoke_model_uv(glm::vec2(1.0f, 1.0f)),
		pack_smoke_model_uv(glm::vec2(0.0f, 0.0f)),
	};
	const std::uint32_t normal = pack_smoke_model_normal(glm::vec3(0.0f, 0.0f, 1.0f));
	std::vector<std::uint32_t> normals {normal, normal, normal};
	std::vector<glm::vec4> tangents {
		glm::vec4(1.0f, 0.0f, 0.0f, 1.0f),
		glm::vec4(1.0f, 0.0f, 0.0f, 1.0f),
		glm::vec4(1.0f, 0.0f, 0.0f, 1.0f),
	};
	std::vector<glm::u8vec4> skin_weights {
		glm::u8vec4(0u, 0u, 0u, 0u),
		glm::u8vec4(255u, 0u, 0u, 0u),
		glm::u8vec4(0u, 0u, 0u, 0u),
		glm::u8vec4(255u, 0u, 0u, 0u),
		glm::u8vec4(0u, 0u, 0u, 0u),
		glm::u8vec4(255u, 0u, 0u, 0u),
	};
	std::vector<std::uint16_t> indices {0u, 1u, 2u};

	std::vector<std::uint8_t> white_texture {255u, 255u, 255u, 255u};
	std::vector<std::uint8_t> green_texture;
	std::vector<std::uint8_t> blue_transparent_texture {20u, 70u, 255u, 204u};
	std::vector<std::uint8_t> magenta_transparent_texture {245u, 30u, 245u, 204u};
	std::vector<std::uint8_t> cyan_alpha_key_texture {20u, 235u, 235u, 255u};
	std::vector<std::uint8_t> yellow_rejected_alpha_texture {245u, 245u, 20u, 64u};
	std::vector<std::uint8_t> orange_depth_probe_texture {255u, 128u, 20u, 255u};
	std::vector<std::uint8_t> red_lit_texture {255u, 0u, 0u, 255u};
	std::vector<std::uint8_t> black_lit_texture {0u, 0u, 0u, 255u};
	std::vector<std::uint8_t> flat_normal_texture {128u, 128u, 255u, 255u};
	std::vector<std::uint8_t> side_normal_texture {255u, 128u, 128u, 255u};
	std::vector<std::uint8_t> no_team_orm_texture {0u, 0u, 0u, 0u};
	std::vector<std::uint8_t> full_team_orm_texture {0u, 0u, 0u, 255u};
	std::vector<std::uint8_t> red_team_mask_texture {255u, 0u, 0u, 255u};
	std::vector<std::uint8_t> blue_emissive_texture {0u, 0u, 255u, 255u};
	static constexpr std::uint8_t white_texture_key = 0;
	static constexpr std::uint8_t green_texture_key = 1;
	static constexpr std::uint8_t blue_texture_key = 2;
	static constexpr std::uint8_t magenta_texture_key = 3;
	static constexpr std::uint8_t cyan_texture_key = 4;
	static constexpr std::uint8_t yellow_texture_key = 5;
	static constexpr std::uint8_t orange_texture_key = 6;
	static constexpr std::uint8_t red_lit_texture_key = 7;
	static constexpr std::uint8_t black_lit_texture_key = 8;
	static constexpr std::uint8_t flat_normal_texture_key = 9;
	static constexpr std::uint8_t side_normal_texture_key = 10;
	static constexpr std::uint8_t no_team_orm_texture_key = 11;
	static constexpr std::uint8_t full_team_orm_texture_key = 12;
	static constexpr std::uint8_t red_team_mask_texture_key = 13;
	static constexpr std::uint8_t blue_emissive_texture_key = 14;
	static constexpr std::uint8_t smoke_model_mesh_key = 0;

	auto texture_source = [](const void* key, const std::vector<std::uint8_t>& rgba8) {
		return hive::metal::ModelTextureCpuSource {
			.key = key,
			.width = 1,
			.height = 1,
			.channels = 4,
			.wrap_flags = 0,
			.srgb = false,
			.rgba8 = &rgba8,
		};
	};

	hive::metal::MetalModelFrame model_frame;
	model_frame.camera.projection_view = glm::mat4(1.0f);
	model_frame.show_lighting = true;
	model_frame.models.meshes.push_back(hive::metal::ModelMeshCpuFrame {
		.key = &smoke_model_mesh_key,
		.revision = 1,
		.vertices_snorm = &vertices,
		.uvs_snorm = &uvs,
		.normals_oct_snorm = &normals,
		.tangents = &tangents,
		.skin_weights = &skin_weights,
		.indices = &indices,
		.textures = {
			texture_source(&white_texture_key, white_texture),
			texture_source(&green_texture_key, green_texture),
			texture_source(&blue_texture_key, blue_transparent_texture),
			texture_source(&magenta_texture_key, magenta_transparent_texture),
			texture_source(&cyan_texture_key, cyan_alpha_key_texture),
			texture_source(&yellow_texture_key, yellow_rejected_alpha_texture),
			texture_source(&orange_texture_key, orange_depth_probe_texture),
			texture_source(&red_lit_texture_key, red_lit_texture),
			texture_source(&black_lit_texture_key, black_lit_texture),
			texture_source(&flat_normal_texture_key, flat_normal_texture),
			texture_source(&side_normal_texture_key, side_normal_texture),
			texture_source(&no_team_orm_texture_key, no_team_orm_texture),
			texture_source(&full_team_orm_texture_key, full_team_orm_texture),
			texture_source(&red_team_mask_texture_key, red_team_mask_texture),
			texture_source(&blue_emissive_texture_key, blue_emissive_texture),
		},
	});
	model_frame.models.instance_matrices = {
		glm::translate(glm::mat4(1.0f), glm::vec3(-0.55f, 0.35f, 0.0f)),
		glm::translate(glm::mat4(1.0f), glm::vec3(0.25f, 0.35f, 0.0f)),
		glm::translate(glm::mat4(1.0f), glm::vec3(-0.55f, -0.45f, 0.0f)),
		glm::translate(glm::mat4(1.0f), glm::vec3(0.25f, -0.45f, 0.0f)),
		glm::translate(glm::mat4(1.0f), glm::vec3(-0.15f, -0.05f, 0.0f)),
		glm::translate(glm::mat4(1.0f), glm::vec3(0.55f, -0.05f, 0.0f)),
		glm::translate(glm::mat4(1.0f), glm::vec3(0.62f, 0.62f, -0.30f)),
		glm::translate(glm::mat4(1.0f), glm::vec3(0.62f, 0.62f, 0.30f)),
		glm::translate(glm::mat4(1.0f), glm::vec3(-0.82f, 0.78f, 0.0f)),
		glm::translate(glm::mat4(1.0f), glm::vec3(-0.32f, 0.78f, 0.0f)),
		glm::translate(glm::mat4(1.0f), glm::vec3(0.18f, 0.78f, 0.0f)),
		glm::translate(glm::mat4(1.0f), glm::vec3(0.68f, 0.78f, 0.0f)),
	};
	model_frame.models.team_color_indexes = {0u, 1u, 2u, 3u, 4u, 5u, 6u, 7u, 8u, 3u, 10u, 11u};
	model_frame.models.bone_matrices = {glm::mat4(1.0f)};
	model_frame.models.layer_colors = {
		glm::vec4(1.0f),
		glm::vec4(1.0f),
		glm::vec4(1.0f),
		glm::vec4(1.0f),
		glm::vec4(1.0f),
		glm::vec4(1.0f),
		glm::vec4(1.0f),
		glm::vec4(1.0f),
		glm::vec4(1.0f),
		glm::vec4(1.0f),
		glm::vec4(1.0f),
		glm::vec4(1.0f),
	};

	auto make_draw = [](const std::uint32_t instance_offset, const std::uint32_t layer_color_offset, const std::uint32_t texture_index) {
		hive::metal::ModelDrawCallCpuFrame draw;
		draw.mesh_index = 0;
		draw.index_count = 3;
		draw.instance_offset = instance_offset;
		draw.bone_offset = 0;
		draw.bone_count = 1;
		draw.layer_color_offset = layer_color_offset;
		draw.layer_skip_count = 1;
		draw.layer_index_local = 0;
		draw.texture_indices = {texture_index, texture_index, texture_index, texture_index, texture_index, texture_index};
		draw.alpha_test = 0.01f;
		draw.layer_lit = 0u;
		draw.cull_face = 0u;
		draw.depth_test = 0u;
		draw.depth_mask = 0u;
		draw.picking_depth_mask = 0u;
		return draw;
	};

	hive::metal::ModelDrawCallCpuFrame sd_team_draw = make_draw(0u, 0u, 0u);
	sd_team_draw.is_team_color = 1u;
	model_frame.models.opaque_sd_draws.push_back(sd_team_draw);

	model_frame.models.opaque_hd_draws.push_back(make_draw(1u, 1u, 1u));

	hive::metal::ModelDrawCallCpuFrame hd_normal_draw = make_draw(8u, 8u, 7u);
	hd_normal_draw.texture_indices = {7u, 10u, 11u, 8u, 8u, 8u};
	hd_normal_draw.layer_lit = 1u;
	model_frame.models.opaque_hd_draws.push_back(hd_normal_draw);

	hive::metal::ModelDrawCallCpuFrame hd_team_mask_draw = make_draw(9u, 9u, 0u);
	hd_team_mask_draw.texture_indices = {0u, 9u, 12u, 8u, 13u, 8u};
	hd_team_mask_draw.layer_lit = 1u;
	model_frame.models.opaque_hd_draws.push_back(hd_team_mask_draw);

	hive::metal::ModelDrawCallCpuFrame hd_emissive_draw = make_draw(10u, 10u, 8u);
	hd_emissive_draw.texture_indices = {8u, 10u, 11u, 14u, 8u, 8u};
	hd_emissive_draw.layer_lit = 1u;
	model_frame.models.opaque_hd_draws.push_back(hd_emissive_draw);

	hive::metal::ModelDrawCallCpuFrame hd_fresnel_draw = make_draw(11u, 11u, 8u);
	hd_fresnel_draw.texture_indices = {8u, 10u, 11u, 8u, 8u, 8u};
	hd_fresnel_draw.layer_lit = 1u;
	hd_fresnel_draw.hd_material_params0 = glm::vec4(0.0f, 1.0f, 0.0f, 0.0f);
	hd_fresnel_draw.hd_material_params1 = glm::vec4(0.45f, 0.85f, 0.35f, 0.0f);
	model_frame.models.opaque_hd_draws.push_back(hd_fresnel_draw);

	hive::metal::ModelDrawCallCpuFrame alpha_key_pass_draw = make_draw(4u, 4u, 4u);
	alpha_key_pass_draw.alpha_test = 0.75f;
	model_frame.models.opaque_sd_draws.push_back(alpha_key_pass_draw);

	hive::metal::ModelDrawCallCpuFrame alpha_key_reject_draw = make_draw(5u, 5u, 5u);
	alpha_key_reject_draw.alpha_test = 0.75f;
	model_frame.models.opaque_hd_draws.push_back(alpha_key_reject_draw);

	hive::metal::ModelDrawCallCpuFrame no_depth_writer_draw = make_draw(6u, 6u, 0u);
	no_depth_writer_draw.depth_test = 0u;
	no_depth_writer_draw.depth_mask = 1u;
	model_frame.models.opaque_sd_draws.push_back(no_depth_writer_draw);

	hive::metal::ModelDrawCallCpuFrame no_depth_probe_draw = make_draw(7u, 7u, 6u);
	no_depth_probe_draw.depth_test = 1u;
	no_depth_probe_draw.depth_mask = 1u;
	model_frame.models.opaque_sd_draws.push_back(no_depth_probe_draw);

	hive::metal::ModelDrawCallCpuFrame transparent_sd_draw = make_draw(2u, 2u, 2u);
	transparent_sd_draw.source_blend_factor = 4u;
	transparent_sd_draw.destination_blend_factor = 5u;
	transparent_sd_draw.depth_mask = 0u;
	model_frame.models.transparent_sd_draws.push_back(transparent_sd_draw);

	hive::metal::ModelDrawCallCpuFrame transparent_hd_draw = make_draw(3u, 3u, 3u);
	transparent_hd_draw.source_blend_factor = 4u;
	transparent_hd_draw.destination_blend_factor = 5u;
	transparent_hd_draw.depth_mask = 0u;
	model_frame.models.transparent_hd_draws.push_back(transparent_hd_draw);

	bool render_ok = true;
	if (!renderer.render_models(frame_context, model_frame)) {
		std::println("Metal model material smoke failed during fallback texture warmup: {}", renderer.last_error());
		render_ok = false;
	}
	if (renderer.last_texture_fallback_stats().model_texture_fallback_count == 0u) {
		std::println("Metal model material smoke failed: warmup did not record the expected missing model texture fallback.");
		render_ok = false;
	}

	green_texture = {20u, 235u, 30u, 255u};
	if (!renderer.render_models(frame_context, model_frame)) {
		std::println("Metal model material smoke failed after late texture availability: {}", renderer.last_error());
		render_ok = false;
	}
	if (renderer.last_texture_fallback_stats().model_texture_fallback_count != 0u) {
		std::println(
			"Metal model material smoke failed: {} model texture fallback(s) remained after late texture availability.",
			renderer.last_texture_fallback_stats().model_texture_fallback_count
		);
		render_ok = false;
	}

	std::vector<std::uint8_t> frame_pixels;
	const bool present_ok = surface.end_frame(std::move(surface_frame), &frame_pixels);
	if (!present_ok) {
		std::println("Metal model material smoke failed: unable to present/read back frame.");
		return false;
	}

	captured_bgra = std::move(frame_pixels);
	captured_width = frame_context.drawable_width;
	captured_height = frame_context.drawable_height;
	return render_ok;
}

bool run_metal_model_material_smoke(QWidget* parent, const QString& artifact_directory) {
	MetalSmokeSurfaceWidget model_preview(parent);
	model_preview.setAttribute(Qt::WA_NativeWindow);
	model_preview.setAttribute(Qt::WA_PaintOnScreen);
	model_preview.setAttribute(Qt::WA_NoSystemBackground);
	model_preview.setAttribute(Qt::WA_OpaquePaintEvent);
	model_preview.resize(256, 256);

	MetalSurface surface;
	hive::metal::MetalMapRenderer renderer;
	std::vector<std::uint8_t> pixels;
	int width = 0;
	int height = 0;
	const bool frame_ok = capture_metal_preview_widget(
		"model material renderer",
		model_preview,
		[&](std::vector<std::uint8_t>& captured_pixels, int& captured_width, int& captured_height) {
			const bool rendered = render_model_material_smoke_frame(surface, renderer, model_preview, captured_pixels, captured_width, captured_height);
			pixels = captured_pixels;
			width = captured_width;
			height = captured_height;
			return rendered;
		},
		smoke_clear_color(0.04f, 0.04f, 0.04f)
	);

	const bool pixels_ok = frame_ok && validate_model_material_smoke_pixels(pixels, width, height);
	return pixels_ok && save_metal_smoke_artifact_png(artifact_directory, "metal-model-materials.png", pixels, width, height);
}

bool validate_model_depth_isolation_smoke_pixels(const std::vector<std::uint8_t>& pixels, const int width, const int height) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("Metal model depth-isolation smoke failed: missing captured pixels.");
		return false;
	}

	std::size_t retained_green_pixels = 0;
	std::size_t depth_cleared_blue_pixels = 0;
	std::size_t stale_red_center_pixels = 0;
	for (int y = 0; y < height; ++y) {
		for (int x = 0; x < width; ++x) {
			const std::size_t offset = (static_cast<std::size_t>(y) * static_cast<std::size_t>(width) + static_cast<std::size_t>(x)) * 4;
			const std::uint8_t b = pixels[offset + 0];
			const std::uint8_t g = pixels[offset + 1];
			const std::uint8_t r = pixels[offset + 2];
			const bool in_left_marker = x < width / 3 && y > height / 8 && y < (height * 7) / 8;
			const bool in_center = x > width / 3 && x < (width * 2) / 3 && y > height / 3 && y < (height * 2) / 3;
			if (in_left_marker && g > 150u && r < 90u && b < 90u) {
				++retained_green_pixels;
			}
			if (in_center && b > 150u && r < 90u && g < 120u) {
				++depth_cleared_blue_pixels;
			}
			if (in_center && r > 150u && g < 90u && b < 90u) {
				++stale_red_center_pixels;
			}
		}
	}

	const std::size_t min_marker_pixels = std::max<std::size_t>(512, pixel_count / 160);
	const std::size_t min_center_pixels = std::max<std::size_t>(512, pixel_count / 120);
	if (retained_green_pixels < min_marker_pixels || depth_cleared_blue_pixels < min_center_pixels || stale_red_center_pixels > 16u) {
		std::println(
			"Metal model depth-isolation smoke failed: saw {} retained green marker pixels, {} blue depth-clear pixels, and {} stale red center pixels; expected at least {} green, at least {} blue, and at most 16 stale red pixels.",
			retained_green_pixels,
			depth_cleared_blue_pixels,
			stale_red_center_pixels,
			min_marker_pixels,
			min_center_pixels
		);
		return false;
	}

	std::println(
		"Metal model depth-isolation smoke captured {}x{} with {} retained color-load marker pixels and {} depth-clear overwrite pixels.",
		width,
		height,
		retained_green_pixels,
		depth_cleared_blue_pixels
	);
	return true;
}

bool render_model_depth_isolation_smoke_frame(
	MetalSurface& surface,
	hive::metal::MetalMapRenderer& renderer,
	QWidget& widget,
	std::vector<std::uint8_t>& captured_bgra,
	int& captured_width,
	int& captured_height
) {
	if (!surface.initialize(reinterpret_cast<void*>(widget.winId()))) {
		std::println("Metal model depth-isolation smoke failed: unable to initialize Metal surface.");
		return false;
	}

	surface.resize(widget.width(), widget.height(), k_metal_smoke_artifact_scale);
	MetalSurface::Frame surface_frame = surface.begin_frame(0.02f, 0.02f, 0.02f, 1.0f);
	if (!surface_frame) {
		std::println("Metal model depth-isolation smoke failed: unable to begin Metal frame.");
		return false;
	}

	hive::metal::FrameContext frame_context {
		.device = surface_frame.device_opaque(),
		.command_queue = surface_frame.command_queue_opaque(),
		.command_buffer = surface_frame.command_buffer_opaque(),
		.render_pass_descriptor = surface_frame.render_pass_descriptor_opaque(),
		.drawable_width = surface_frame.drawable_width(),
		.drawable_height = surface_frame.drawable_height(),
		.scale = surface_frame.scale(),
	};

	std::vector<glm::uvec2> vertices;
	std::vector<std::uint32_t> uvs;
	std::vector<std::uint32_t> normals;
	std::vector<glm::vec4> tangents;
	std::vector<glm::u8vec4> skin_weights;
	std::vector<std::uint16_t> indices;
	vertices.reserve(12);
	uvs.reserve(12);
	normals.reserve(12);
	tangents.reserve(12);
	skin_weights.reserve(24);
	indices.reserve(18);

	const std::uint32_t normal = pack_smoke_model_normal(glm::vec3(0.0f, 0.0f, 1.0f));
	auto append_vertex = [&](const glm::vec3 position, const glm::vec2 uv) {
		vertices.push_back(pack_vec3_to_uvec2(position, 8192.0f));
		uvs.push_back(pack_smoke_model_uv(uv));
		normals.push_back(normal);
		tangents.emplace_back(1.0f, 0.0f, 0.0f, 1.0f);
		skin_weights.emplace_back(0u, 0u, 0u, 0u);
		skin_weights.emplace_back(255u, 0u, 0u, 0u);
	};
	auto append_quad = [&](const float left, const float bottom, const float right, const float top, const float depth) {
		const std::uint16_t base = static_cast<std::uint16_t>(vertices.size());
		append_vertex(glm::vec3(left, bottom, depth), glm::vec2(0.0f, 1.0f));
		append_vertex(glm::vec3(right, bottom, depth), glm::vec2(1.0f, 1.0f));
		append_vertex(glm::vec3(left, top, depth), glm::vec2(0.0f, 0.0f));
		append_vertex(glm::vec3(right, top, depth), glm::vec2(1.0f, 0.0f));
		indices.insert(indices.end(), {base, static_cast<std::uint16_t>(base + 1u), static_cast<std::uint16_t>(base + 2u),
									   static_cast<std::uint16_t>(base + 2u), static_cast<std::uint16_t>(base + 1u), static_cast<std::uint16_t>(base + 3u)});
	};

	append_quad(-0.38f, -0.38f, 0.38f, 0.38f, 0.20f);
	append_quad(-0.88f, -0.82f, -0.58f, 0.82f, 0.25f);
	append_quad(-0.38f, -0.38f, 0.38f, 0.38f, 0.80f);

	std::vector<std::uint8_t> red_texture {235u, 32u, 32u, 255u};
	std::vector<std::uint8_t> green_texture {32u, 225u, 48u, 255u};
	std::vector<std::uint8_t> blue_texture {32u, 64u, 240u, 255u};
	static constexpr std::uint8_t red_texture_key = 0;
	static constexpr std::uint8_t green_texture_key = 1;
	static constexpr std::uint8_t blue_texture_key = 2;
	static constexpr std::uint8_t depth_isolation_mesh_key = 0;

	auto texture_source = [](const void* key, const std::vector<std::uint8_t>& rgba8) {
		return hive::metal::ModelTextureCpuSource {
			.key = key,
			.width = 1,
			.height = 1,
			.channels = 4,
			.wrap_flags = 0,
			.srgb = false,
			.rgba8 = &rgba8,
		};
	};
	auto make_draw = [](const std::uint32_t first_index, const std::uint32_t texture_index) {
		hive::metal::ModelDrawCallCpuFrame draw;
		draw.mesh_index = 0;
		draw.index_count = 6;
		draw.first_index = first_index;
		draw.instance_offset = 0;
		draw.bone_offset = 0;
		draw.bone_count = 1;
		draw.layer_color_offset = 0;
		draw.layer_skip_count = 1;
		draw.layer_index_local = 0;
		draw.texture_indices = {texture_index, texture_index, texture_index, texture_index, texture_index, texture_index};
		draw.alpha_test = 0.01f;
		draw.layer_lit = 0u;
		draw.cull_face = 0u;
		draw.depth_test = 1u;
		draw.depth_mask = 1u;
		draw.picking_depth_mask = 1u;
		return draw;
	};

	hive::metal::MetalModelFrame first_frame;
	first_frame.camera.projection_view = glm::mat4(1.0f);
	first_frame.show_lighting = false;
	first_frame.models.meshes.push_back(hive::metal::ModelMeshCpuFrame {
		.key = &depth_isolation_mesh_key,
		.revision = 1,
		.vertices_snorm = &vertices,
		.uvs_snorm = &uvs,
		.normals_oct_snorm = &normals,
		.tangents = &tangents,
		.skin_weights = &skin_weights,
		.indices = &indices,
		.textures = {
			texture_source(&red_texture_key, red_texture),
			texture_source(&green_texture_key, green_texture),
			texture_source(&blue_texture_key, blue_texture),
		},
	});
	first_frame.models.instance_matrices = {glm::mat4(1.0f)};
	first_frame.models.team_color_indexes = {0u};
	first_frame.models.bone_matrices = {glm::mat4(1.0f)};
	first_frame.models.layer_colors = {glm::vec4(1.0f)};
	first_frame.models.opaque_sd_draws.push_back(make_draw(0u, 0u));
	first_frame.models.opaque_sd_draws.push_back(make_draw(6u, 1u));

	hive::metal::MetalModelFrame second_frame = first_frame;
	second_frame.models.opaque_sd_draws.clear();
	second_frame.models.opaque_sd_draws.push_back(make_draw(12u, 2u));

	bool render_ok = true;
	if (!renderer.render_models(frame_context, first_frame)) {
		std::println("Metal model depth-isolation smoke failed during first model pass: {}", renderer.last_error());
		render_ok = false;
	}

	frame_context.load_existing_render_target = true;
	if (!renderer.render_models(frame_context, second_frame)) {
		std::println("Metal model depth-isolation smoke failed during color-load/depth-clear pass: {}", renderer.last_error());
		render_ok = false;
	}

	std::vector<std::uint8_t> frame_pixels;
	const bool present_ok = surface.end_frame(std::move(surface_frame), &frame_pixels);
	if (!present_ok) {
		std::println("Metal model depth-isolation smoke failed: unable to present/read back frame.");
		return false;
	}

	captured_bgra = std::move(frame_pixels);
	captured_width = frame_context.drawable_width;
	captured_height = frame_context.drawable_height;
	return render_ok;
}

bool run_metal_model_depth_isolation_smoke(QWidget* parent, const QString& artifact_directory) {
	MetalSmokeSurfaceWidget model_preview(parent);
	model_preview.setAttribute(Qt::WA_NativeWindow);
	model_preview.setAttribute(Qt::WA_PaintOnScreen);
	model_preview.setAttribute(Qt::WA_NoSystemBackground);
	model_preview.setAttribute(Qt::WA_OpaquePaintEvent);
	model_preview.resize(256, 256);

	MetalSurface surface;
	hive::metal::MetalMapRenderer renderer;
	std::vector<std::uint8_t> pixels;
	int width = 0;
	int height = 0;
	const bool frame_ok = capture_metal_preview_widget(
		"model depth-isolation renderer",
		model_preview,
		[&](std::vector<std::uint8_t>& captured_pixels, int& captured_width, int& captured_height) {
			const bool rendered =
				render_model_depth_isolation_smoke_frame(surface, renderer, model_preview, captured_pixels, captured_width, captured_height);
			pixels = captured_pixels;
			width = captured_width;
			height = captured_height;
			return rendered;
		},
		smoke_clear_color(0.02f, 0.02f, 0.02f)
	);

	const bool pixels_ok = frame_ok && validate_model_depth_isolation_smoke_pixels(pixels, width, height);
	return pixels_ok && save_metal_smoke_artifact_png(artifact_directory, "metal-model-depth-isolation.png", pixels, width, height);
}

bool validate_selection_overlay_smoke_pixels(const std::vector<std::uint8_t>& pixels, const int width, const int height) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("Metal selection overlay smoke failed: missing captured pixels.");
		return false;
	}

	std::size_t cyan_overlay_pixels = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4;
		const std::uint8_t b = pixels[offset + 0];
		const std::uint8_t g = pixels[offset + 1];
		const std::uint8_t r = pixels[offset + 2];
		if (g > 150u && b > 150u && r < 80u) {
			++cyan_overlay_pixels;
		}
	}

	const std::size_t min_pixels = std::max<std::size_t>(256, pixel_count / 16);
	if (cyan_overlay_pixels < min_pixels) {
		std::println(
			"Metal selection overlay smoke failed: saw {} cyan overlay pixels, expected at least {}.",
			cyan_overlay_pixels,
			min_pixels
		);
		return false;
	}

	std::println(
		"Metal selection overlay smoke captured {}x{} with {} cyan overlay pixels.",
		width,
		height,
		cyan_overlay_pixels
	);
	return true;
}

bool render_selection_overlay_smoke_frame(
	MetalSurface& surface,
	hive::metal::MetalMapRenderer& renderer,
	QWidget& widget,
	std::vector<std::uint8_t>& captured_bgra,
	int& captured_width,
	int& captured_height
) {
	if (!surface.initialize(reinterpret_cast<void*>(widget.winId()))) {
		std::println("Metal selection overlay smoke failed: unable to initialize Metal surface.");
		return false;
	}

	surface.resize(widget.width(), widget.height(), k_metal_smoke_artifact_scale);
	MetalSurface::Frame surface_frame = surface.begin_frame(0.04f, 0.04f, 0.04f, 1.0f);
	if (!surface_frame) {
		std::println("Metal selection overlay smoke failed: unable to begin Metal frame.");
		return false;
	}

	hive::metal::FrameContext frame_context {
		.device = surface_frame.device_opaque(),
		.command_queue = surface_frame.command_queue_opaque(),
		.command_buffer = surface_frame.command_buffer_opaque(),
		.render_pass_descriptor = surface_frame.render_pass_descriptor_opaque(),
		.drawable_width = surface_frame.drawable_width(),
		.drawable_height = surface_frame.drawable_height(),
		.scale = surface_frame.scale(),
	};

	std::vector<glm::uvec2> model_vertices {
		pack_vec3_to_uvec2(glm::vec3(-0.75f, -0.75f, 0.45f), 8192.0f),
		pack_vec3_to_uvec2(glm::vec3(0.75f, -0.75f, 0.45f), 8192.0f),
		pack_vec3_to_uvec2(glm::vec3(-0.75f, 0.75f, 0.45f), 8192.0f),
		pack_vec3_to_uvec2(glm::vec3(0.75f, 0.75f, 0.45f), 8192.0f),
	};
	std::vector<std::uint32_t> model_uvs {
		pack_smoke_model_uv(glm::vec2(0.0f, 1.0f)),
		pack_smoke_model_uv(glm::vec2(1.0f, 1.0f)),
		pack_smoke_model_uv(glm::vec2(0.0f, 0.0f)),
		pack_smoke_model_uv(glm::vec2(1.0f, 0.0f)),
	};
	const std::uint32_t model_normal = pack_smoke_model_normal(glm::vec3(0.0f, 0.0f, 1.0f));
	std::vector<std::uint32_t> model_normals {model_normal, model_normal, model_normal, model_normal};
	std::vector<glm::vec4> model_tangents {
		glm::vec4(1.0f, 0.0f, 0.0f, 1.0f),
		glm::vec4(1.0f, 0.0f, 0.0f, 1.0f),
		glm::vec4(1.0f, 0.0f, 0.0f, 1.0f),
		glm::vec4(1.0f, 0.0f, 0.0f, 1.0f),
	};
	std::vector<glm::u8vec4> model_skin_weights {
		glm::u8vec4(0u, 0u, 0u, 0u),
		glm::u8vec4(255u, 0u, 0u, 0u),
		glm::u8vec4(0u, 0u, 0u, 0u),
		glm::u8vec4(255u, 0u, 0u, 0u),
		glm::u8vec4(0u, 0u, 0u, 0u),
		glm::u8vec4(255u, 0u, 0u, 0u),
		glm::u8vec4(0u, 0u, 0u, 0u),
		glm::u8vec4(255u, 0u, 0u, 0u),
	};
	std::vector<std::uint16_t> model_indices {0u, 1u, 2u, 1u, 3u, 2u};
	std::vector<std::uint8_t> model_texture {255u, 24u, 24u, 255u};
	static constexpr std::uint8_t overlay_model_mesh_key = 0;
	static constexpr std::uint8_t overlay_model_texture_key = 0;

	hive::metal::MetalMapFrame overlay_frame;
	overlay_frame.camera.projection_view = glm::mat4(1.0f);
	overlay_frame.terrain.width = 2;
	overlay_frame.terrain.height = 2;
	overlay_frame.terrain.corner_ground_heights = {0.0f, 0.0f, 0.0f, 0.0f};
	overlay_frame.terrain.final_ground_heights = overlay_frame.terrain.corner_ground_heights;
	overlay_frame.terrain.ground_texture_indices = {glm::uvec4(0u, 0u, 0u, 0u)};
	overlay_frame.terrain.ground_exists = {0u};
	overlay_frame.terrain.corner_water_heights = overlay_frame.terrain.corner_ground_heights;
	overlay_frame.terrain.water_exists = {0u, 0u, 0u, 0u};
	overlay_frame.show_lighting = false;
	overlay_frame.show_water = false;
	overlay_frame.models.meshes.push_back(hive::metal::ModelMeshCpuFrame {
		.key = &overlay_model_mesh_key,
		.revision = 1,
		.vertices_snorm = &model_vertices,
		.uvs_snorm = &model_uvs,
		.normals_oct_snorm = &model_normals,
		.tangents = &model_tangents,
		.skin_weights = &model_skin_weights,
		.indices = &model_indices,
		.textures = {
			hive::metal::ModelTextureCpuSource {
				.key = &overlay_model_texture_key,
				.width = 1,
				.height = 1,
				.channels = 4,
				.wrap_flags = 0,
				.srgb = false,
				.rgba8 = &model_texture,
			},
		},
	});
	overlay_frame.models.instance_matrices = {glm::mat4(1.0f)};
	overlay_frame.models.team_color_indexes = {0u};
	overlay_frame.models.bone_matrices = {glm::mat4(1.0f)};
	overlay_frame.models.layer_colors = {glm::vec4(1.0f)};
	hive::metal::ModelDrawCallCpuFrame model_draw;
	model_draw.mesh_index = 0;
	model_draw.index_count = static_cast<std::uint32_t>(model_indices.size());
	model_draw.instance_offset = 0;
	model_draw.bone_offset = 0;
	model_draw.bone_count = 1;
	model_draw.layer_color_offset = 0;
	model_draw.layer_skip_count = 1;
	model_draw.layer_index_local = 0;
	model_draw.texture_indices = {0u, 0u, 0u, 0u, 0u, 0u};
	model_draw.alpha_test = 0.01f;
	model_draw.layer_lit = 0u;
	model_draw.cull_face = 0u;
	model_draw.depth_test = 0u;
	model_draw.depth_mask = 0u;
	model_draw.picking_depth_mask = 0u;
	overlay_frame.models.opaque_sd_draws.push_back(model_draw);
	overlay_frame.selection_overlay.positions = {
		glm::vec4(-0.70f, -0.70f, 0.5f, 1.0f),
		glm::vec4(0.70f, -0.70f, 0.5f, 1.0f),
		glm::vec4(-0.70f, 0.70f, 0.5f, 1.0f),
		glm::vec4(0.70f, -0.70f, 0.5f, 1.0f),
		glm::vec4(0.70f, 0.70f, 0.5f, 1.0f),
		glm::vec4(-0.70f, 0.70f, 0.5f, 1.0f),
	};
	overlay_frame.selection_overlay.uvs = {
		glm::vec2(0.0f, 1.0f),
		glm::vec2(1.0f, 1.0f),
		glm::vec2(0.0f, 0.0f),
		glm::vec2(1.0f, 1.0f),
		glm::vec2(1.0f, 0.0f),
		glm::vec2(0.0f, 0.0f),
	};
	overlay_frame.selection_overlay.colors = {
		glm::vec4(0.0f, 0.95f, 1.0f, 1.0f),
		glm::vec4(0.0f, 0.95f, 1.0f, 1.0f),
		glm::vec4(0.0f, 0.95f, 1.0f, 1.0f),
		glm::vec4(0.0f, 0.95f, 1.0f, 1.0f),
		glm::vec4(0.0f, 0.95f, 1.0f, 1.0f),
		glm::vec4(0.0f, 0.95f, 1.0f, 1.0f),
	};
	overlay_frame.selection_overlay.draws.push_back(hive::metal::SelectionOverlayDrawCpuFrame {
		.first_vertex = 0,
		.vertex_count = static_cast<std::uint32_t>(overlay_frame.selection_overlay.positions.size()),
		.primitive_type = 1,
		.style = 0,
	});

	bool render_ok = true;
	if (!renderer.render(frame_context, overlay_frame)) {
		std::println("Metal selection overlay smoke failed: {}", renderer.last_error());
		render_ok = false;
	}

	std::vector<std::uint8_t> frame_pixels;
	const bool present_ok = surface.end_frame(std::move(surface_frame), &frame_pixels);
	if (!present_ok) {
		std::println("Metal selection overlay smoke failed: unable to present/read back frame.");
		return false;
	}

	captured_bgra = std::move(frame_pixels);
	captured_width = frame_context.drawable_width;
	captured_height = frame_context.drawable_height;
	return render_ok;
}

bool run_metal_selection_overlay_smoke(QWidget* parent, const QString& artifact_directory) {
	MetalSmokeSurfaceWidget overlay_preview(parent);
	overlay_preview.setAttribute(Qt::WA_NativeWindow);
	overlay_preview.setAttribute(Qt::WA_PaintOnScreen);
	overlay_preview.setAttribute(Qt::WA_NoSystemBackground);
	overlay_preview.setAttribute(Qt::WA_OpaquePaintEvent);
	overlay_preview.resize(256, 256);

	MetalSurface surface;
	hive::metal::MetalMapRenderer renderer;
	std::vector<std::uint8_t> pixels;
	int width = 0;
	int height = 0;
	const bool frame_ok = capture_metal_preview_widget(
		"selection overlay renderer",
		overlay_preview,
		[&](std::vector<std::uint8_t>& captured_pixels, int& captured_width, int& captured_height) {
			const bool rendered =
				render_selection_overlay_smoke_frame(surface, renderer, overlay_preview, captured_pixels, captured_width, captured_height);
			pixels = captured_pixels;
			width = captured_width;
			height = captured_height;
			return rendered;
		},
		smoke_clear_color(0.04f, 0.04f, 0.04f)
	);

	const bool pixels_ok = frame_ok && validate_selection_overlay_smoke_pixels(pixels, width, height);
	return pixels_ok && save_metal_smoke_artifact_png(artifact_directory, "metal-selection-overlay.png", pixels, width, height);
}

bool validate_particle_smoke_pixels(const std::vector<std::uint8_t>& pixels, const int width, const int height) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("Metal particle smoke failed: missing captured pixels.");
		return false;
	}

	std::size_t warm_particle_pixels = 0;
	std::size_t red_orange_pixels = 0;
	std::size_t yellow_pixels = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4;
		const std::uint8_t b = pixels[offset + 0];
		const std::uint8_t g = pixels[offset + 1];
		const std::uint8_t r = pixels[offset + 2];
		const bool red_orange = is_smoke_red_orange_particle_pixel(b, g, r);
		const bool yellow = is_smoke_yellow_particle_pixel(b, g, r);
		if (red_orange || yellow) {
			++warm_particle_pixels;
		}
		if (red_orange) {
			++red_orange_pixels;
		}
		if (yellow) {
			++yellow_pixels;
		}
	}

	const std::size_t min_warm_pixels = std::max<std::size_t>(1024, pixel_count / 64);
	const std::size_t min_tint_pixels = std::max<std::size_t>(128, pixel_count / 256);
	if (warm_particle_pixels < min_warm_pixels || red_orange_pixels < min_tint_pixels || yellow_pixels < min_tint_pixels) {
		std::println(
			"Metal particle smoke failed: saw {} warm, {} red/orange, and {} yellow pixels; expected at least {}, {}, and {}.",
			warm_particle_pixels,
			red_orange_pixels,
			yellow_pixels,
			min_warm_pixels,
			min_tint_pixels,
			min_tint_pixels
		);
		return false;
	}

	std::println(
		"Metal particle smoke captured {}x{} with {} warm, {} red/orange, and {} yellow pixels.",
		width,
		height,
		warm_particle_pixels,
		red_orange_pixels,
		yellow_pixels
	);
	return true;
}

bool run_metal_particle_renderer_smoke(QWidget* parent, const QString& artifact_directory) {
	MetalSmokeSurfaceWidget particle_preview(parent);
	particle_preview.setAttribute(Qt::WA_NativeWindow);
	particle_preview.setAttribute(Qt::WA_PaintOnScreen);
	particle_preview.setAttribute(Qt::WA_NoSystemBackground);
	particle_preview.setAttribute(Qt::WA_OpaquePaintEvent);
	particle_preview.resize(256, 256);

	MetalSurface particle_surface;
	hive::metal::MetalMapRenderer particle_renderer;
	std::vector<std::uint8_t> particle_pixels;
	int particle_width = 0;
	int particle_height = 0;
	const bool particle_ok = capture_metal_preview_widget(
		"particle renderer",
		particle_preview,
		[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
			if (!particle_surface.initialize(reinterpret_cast<void*>(particle_preview.winId()))) {
				std::println("Metal particle renderer smoke failed: unable to initialize Metal surface.");
				return false;
			}

			particle_surface.resize(particle_preview.width(), particle_preview.height(), k_metal_smoke_artifact_scale);
			MetalSurface::Frame surface_frame = particle_surface.begin_frame(0.08f, 0.08f, 0.08f, 1.0f);
			if (!surface_frame) {
				std::println("Metal particle renderer smoke failed: unable to begin Metal frame.");
				return false;
			}

			hive::metal::FrameContext frame_context {
				.device = surface_frame.device_opaque(),
				.command_queue = surface_frame.command_queue_opaque(),
				.command_buffer = surface_frame.command_buffer_opaque(),
				.render_pass_descriptor = surface_frame.render_pass_descriptor_opaque(),
				.drawable_width = surface_frame.drawable_width(),
				.drawable_height = surface_frame.drawable_height(),
				.scale = surface_frame.scale(),
			};

			static constexpr std::uint8_t smoke_particle_red_texture_key = 0;
			static constexpr std::uint8_t smoke_particle_yellow_texture_key = 1;
			std::vector<std::uint8_t> smoke_particle_red_texture;
			std::vector<std::uint8_t> smoke_particle_yellow_texture;

			hive::metal::MetalModelFrame particle_frame;
			particle_frame.camera.projection_view = glm::mat4(1.0f);
			particle_frame.show_lighting = false;
			particle_frame.particles.positions = {
				glm::vec4(-0.72f, -0.65f, 0.0f, 1.0f),
				glm::vec4(-0.05f, -0.65f, 0.0f, 1.0f),
				glm::vec4(-0.72f, 0.65f, 0.0f, 1.0f),
				glm::vec4(-0.05f, -0.65f, 0.0f, 1.0f),
				glm::vec4(-0.05f, 0.65f, 0.0f, 1.0f),
				glm::vec4(-0.72f, 0.65f, 0.0f, 1.0f),
				glm::vec4(0.05f, -0.65f, 0.0f, 1.0f),
				glm::vec4(0.72f, -0.65f, 0.0f, 1.0f),
				glm::vec4(0.05f, 0.65f, 0.0f, 1.0f),
				glm::vec4(0.72f, -0.65f, 0.0f, 1.0f),
				glm::vec4(0.72f, 0.65f, 0.0f, 1.0f),
				glm::vec4(0.05f, 0.65f, 0.0f, 1.0f),
			};
			particle_frame.particles.uvs = {
				glm::vec2(0.0f, 1.0f),
				glm::vec2(1.0f, 1.0f),
				glm::vec2(0.0f, 0.0f),
				glm::vec2(1.0f, 1.0f),
				glm::vec2(1.0f, 0.0f),
				glm::vec2(0.0f, 0.0f),
				glm::vec2(0.0f, 1.0f),
				glm::vec2(1.0f, 1.0f),
				glm::vec2(0.0f, 0.0f),
				glm::vec2(1.0f, 1.0f),
				glm::vec2(1.0f, 0.0f),
				glm::vec2(0.0f, 0.0f),
			};
			particle_frame.particles.colors = {
				glm::vec4(1.0f),
				glm::vec4(1.0f),
				glm::vec4(1.0f),
				glm::vec4(1.0f),
				glm::vec4(1.0f),
				glm::vec4(1.0f),
				glm::vec4(1.0f),
				glm::vec4(1.0f),
				glm::vec4(1.0f),
				glm::vec4(1.0f),
				glm::vec4(1.0f),
				glm::vec4(1.0f),
			};
			particle_frame.particles.textures.push_back(hive::metal::ModelTextureCpuSource {
				.key = &smoke_particle_red_texture_key,
				.width = 1,
				.height = 1,
				.channels = 4,
				.wrap_flags = 0,
				.srgb = false,
				.rgba8 = &smoke_particle_red_texture,
			});
			particle_frame.particles.textures.push_back(hive::metal::ModelTextureCpuSource {
				.key = &smoke_particle_yellow_texture_key,
				.width = 1,
				.height = 1,
				.channels = 4,
				.wrap_flags = 0,
				.srgb = false,
				.rgba8 = &smoke_particle_yellow_texture,
			});
			particle_frame.particles.draws.push_back(hive::metal::ParticleEmitter2DrawCpuFrame {
				.first_vertex = 0,
				.vertex_count = 6,
				.texture_index = 0,
				.filter_mode = 0,
			});
			particle_frame.particles.draws.push_back(hive::metal::ParticleEmitter2DrawCpuFrame {
				.first_vertex = 6,
				.vertex_count = 6,
				.texture_index = 1,
				.filter_mode = 0,
			});

			bool render_ok = true;
			if (!particle_renderer.render_models(frame_context, particle_frame)) {
				std::println("Metal particle renderer smoke failed during fallback texture warmup: {}", particle_renderer.last_error());
				render_ok = false;
			}
			if (particle_renderer.last_texture_fallback_stats().particle_texture_fallback_count != 2u) {
				std::println(
					"Metal particle renderer smoke failed: warmup recorded {} particle texture fallback(s), expected 2.",
					particle_renderer.last_texture_fallback_stats().particle_texture_fallback_count
				);
				render_ok = false;
			}

			smoke_particle_red_texture = {255u, 96u, 20u, 255u};
			smoke_particle_yellow_texture = {255u, 220u, 20u, 255u};
			if (!particle_renderer.render_models(frame_context, particle_frame)) {
				std::println("Metal particle renderer smoke failed after late texture availability: {}", particle_renderer.last_error());
				render_ok = false;
			}
			if (particle_renderer.last_texture_fallback_stats().particle_texture_fallback_count != 0u) {
				std::println(
					"Metal particle renderer smoke failed: {} particle texture fallback(s) remained after late texture availability.",
					particle_renderer.last_texture_fallback_stats().particle_texture_fallback_count
				);
				render_ok = false;
			}

			std::vector<std::uint8_t> frame_pixels;
			const bool present_ok = particle_surface.end_frame(std::move(surface_frame), &frame_pixels);
			if (!present_ok) {
				std::println("Metal particle renderer smoke failed: unable to present/read back frame.");
				return false;
			}

			pixels = std::move(frame_pixels);
			width = frame_context.drawable_width;
			height = frame_context.drawable_height;
			particle_pixels = pixels;
			particle_width = width;
			particle_height = height;
			return render_ok;
		},
		smoke_clear_color(0.08f, 0.08f, 0.08f)
	);

	const bool particle_pixels_ok = particle_ok && validate_particle_smoke_pixels(particle_pixels, particle_width, particle_height);
	return particle_pixels_ok && save_metal_smoke_artifact_png(
		artifact_directory,
		"metal-particles.png",
		particle_pixels,
		particle_width,
		particle_height
	);
}

bool validate_pe1_spawn_render_pixels(const std::vector<std::uint8_t>& pixels, const int width, const int height) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("Metal PE1 spawned-model render smoke failed: missing captured pixels.");
		return false;
	}

	std::size_t pe1_tga_pixels = 0;
	std::size_t spawned_child_pixels = 0;
	std::size_t spawned_grandchild_pixels = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4;
		const std::uint8_t b = pixels[offset + 0];
		const std::uint8_t g = pixels[offset + 1];
		const std::uint8_t r = pixels[offset + 2];
		if (is_smoke_magenta_particle_pixel(b, g, r)) {
			++pe1_tga_pixels;
		}
		if (is_smoke_green_particle_model_pixel(b, g, r)) {
			++spawned_child_pixels;
		}
		if (is_smoke_blue_particle_model_pixel(b, g, r)) {
			++spawned_grandchild_pixels;
		}
	}

	const std::size_t min_pixels = std::max<std::size_t>(96, pixel_count / 512);
	const std::size_t min_particle_pixels = std::max<std::size_t>(24, pixel_count / 12000);
	if (pe1_tga_pixels < min_particle_pixels || spawned_child_pixels < min_pixels || spawned_grandchild_pixels < min_pixels) {
		std::println(
			"Metal PE1 spawned-model render smoke failed: saw {} magenta PE1 TGA, {} green child, and {} blue grandchild pixels; expected at least {}, {}, and {}.",
			pe1_tga_pixels,
			spawned_child_pixels,
			spawned_grandchild_pixels,
			min_particle_pixels,
			min_pixels,
			min_pixels
		);
		return false;
	}

	std::println(
		"Metal PE1 spawned-model render smoke captured {}x{} with {} magenta PE1 TGA, {} green child, and {} blue grandchild pixels.",
		width,
		height,
		pe1_tga_pixels,
		spawned_child_pixels,
		spawned_grandchild_pixels
	);
	return true;
}

bool run_metal_pe1_spawned_model_render_smoke(QWidget* parent, const QString& artifact_directory) {
	QTemporaryDir synthetic_model_dir(QDir::tempPath() + QStringLiteral("/HiveWE-pe1-spawn-render-XXXXXX"));
	if (!synthetic_model_dir.isValid()) {
		std::println("Metal PE1 spawned-model render smoke failed: unable to create a temporary model directory.");
		return false;
	}

	const fs::path synthetic_path = fs::path(synthetic_model_dir.path().toStdString());
	const fs::path particle_texture_path = synthetic_path / "particle-smoke.tga";
	const fs::path particle_pe1_tga_texture_path = synthetic_path / "particle-pe1-tga-magenta.tga";
	const fs::path particle_ribbon_texture_path = synthetic_path / "particle-ribbon-cyan.tga";
	const fs::path particle_spawn_child_texture_path = synthetic_path / "particle-spawn-child-green.tga";
	const fs::path particle_spawn_grandchild_texture_path = synthetic_path / "particle-spawn-grandchild-blue.tga";
	const fs::path particle_model_path = synthetic_path / "particle-only.mdx";
	const fs::path particle_spawn_model_path = synthetic_path / "particle-spawn-child.mdx";
	const fs::path particle_spawn_grandchild_model_path = synthetic_path / "particle-spawn-grandchild.mdx";

	const std::shared_ptr<mdx::MDX> particle_spawn_grandchild_model = make_particle_spawn_smoke_model(particle_spawn_grandchild_texture_path);
	const std::shared_ptr<mdx::MDX> particle_spawn_model =
		make_particle_spawn_smoke_model(particle_spawn_child_texture_path, particle_spawn_grandchild_model_path);
	const std::shared_ptr<mdx::MDX> particle_model = make_particle_preview_smoke_model(
		particle_texture_path,
		particle_pe1_tga_texture_path,
		particle_ribbon_texture_path,
		particle_spawn_model_path
	);

	const bool files_ok = write_particle_preview_texture(particle_texture_path)
		&& write_smoke_tga_texture(particle_pe1_tga_texture_path, 255u, 0u, 255u, 255u)
		&& write_smoke_tga_texture(particle_ribbon_texture_path, 0u, 255u, 255u, 255u)
		&& write_smoke_tga_texture(particle_spawn_child_texture_path, 40u, 220u, 80u, 255u)
		&& write_smoke_tga_texture(particle_spawn_grandchild_texture_path, 40u, 80u, 255u, 255u)
		&& write_preview_model(particle_spawn_grandchild_model, particle_spawn_grandchild_model_path, "PE1 spawn render grandchild")
		&& write_preview_model(particle_spawn_model, particle_spawn_model_path, "PE1 spawn render child")
		&& write_preview_model(particle_model, particle_model_path, "PE1 spawn render parent");
	if (!files_ok || !particle_model) {
		return false;
	}

	std::shared_ptr<SkinnedMesh> mesh;
	try {
		mesh = std::make_shared<SkinnedMesh>(particle_model, std::nullopt);
	} catch (const std::exception& error) {
		std::println("Metal PE1 spawned-model render smoke failed while loading synthetic mesh: {}", error.what());
		return false;
	}

	const glm::vec3 camera_position(0.0f, -320.0f, 160.0f);
	const glm::vec3 camera_direction = glm::normalize(glm::vec3(0.0f, 1.0f, -0.25f));
	struct SmokeMapCamera {
		glm::vec3 position = glm::vec3(0.0f);
		glm::vec3 direction = glm::vec3(0.0f, 1.0f, 0.0f);
		glm::vec3 X = glm::vec3(1.0f, 0.0f, 0.0f);
		glm::vec3 Y = glm::vec3(0.0f, 0.0f, 1.0f);
		float distance = 0.0f;

		bool inside_frustrum_transform(const glm::vec3&, const glm::vec3&, const glm::mat4&) const {
			return true;
		}
	};
	struct SmokeMapUnit {
		std::string id = "hfoo";
		std::shared_ptr<SkinnedMesh> mesh;
		SkeletalModelInstance skeleton;
		glm::vec3 color = glm::vec3(1.0f);
		std::uint32_t player = 0u;
	};
	struct SmokeMapUnits {
		std::vector<SmokeMapUnit> units;
		std::vector<SmokeMapUnit> items;
	};
	struct SmokeMapDoodad {
		std::shared_ptr<SkinnedMesh> mesh;
		SkeletalModelInstance skeleton;
		glm::vec3 color = glm::vec3(1.0f);
	};
	struct SmokeMapDoodads {
		std::vector<SmokeMapDoodad> doodads;
		std::vector<SmokeMapDoodad> special_doodads;
	};
	struct SmokeMap {
		bool render_units = true;
		bool render_doodads = true;
		SmokeMapUnits units;
		SmokeMapDoodads doodads;
	};
	const auto make_spawn_skeleton = [&](const glm::vec3& position) {
		SkeletalModelInstance instance(mesh->mdx);
		instance.update_location(position, 0.0f, glm::vec3(1.0f));
		for (int i = 0; i < 4; ++i) {
			instance.update(1.0 / 30.0);
		}
		return instance;
	};
	SmokeMap smoke_map;
	smoke_map.units.units.push_back(SmokeMapUnit {
		.id = "hfoo",
		.mesh = mesh,
		.skeleton = make_spawn_skeleton(glm::vec3(-42.0f, 0.0f, 0.0f)),
		.color = glm::vec3(1.0f),
		.player = 0u,
	});
	smoke_map.doodads.doodads.push_back(SmokeMapDoodad {
		.mesh = mesh,
		.skeleton = make_spawn_skeleton(glm::vec3(42.0f, 0.0f, 0.0f)),
		.color = glm::vec3(1.0f),
	});
	const SmokeMapCamera smoke_camera {
		.position = camera_position,
		.direction = camera_direction,
		.X = glm::vec3(1.0f, 0.0f, 0.0f),
		.Y = glm::vec3(0.0f, 0.0f, 1.0f),
		.distance = 360.0f,
	};
	hive::metal::ModelCpuFrame model_frame = hive::metal::make_model_cpu_frame(smoke_map, smoke_camera);
	const std::size_t model_draw_count = model_frame.opaque_sd_draws.size() + model_frame.opaque_hd_draws.size()
		+ model_frame.transparent_sd_draws.size() + model_frame.transparent_hd_draws.size();
	if (model_frame.meshes.size() < 2u || model_draw_count < 4u) {
		std::println(
			"Metal PE1 spawned-model render smoke failed: map-object model frame had {} meshes and {} visible draws; expected spawned child and grandchild model draws from unit and doodad paths.",
			model_frame.meshes.size(),
			model_draw_count
		);
		return false;
	}
	hive::metal::ParticleEmitter2CpuFrame particle_frame = hive::metal::make_particle_emitter2_cpu_frame(smoke_map, smoke_camera);
	if (!hive::metal::particle_emitter2_frame_valid(particle_frame) || particle_frame.draws.size() < 6u
		|| particle_frame.positions.empty() || particle_frame.textures.size() < 3u) {
		std::println(
			"Metal PE1 spawned-model render smoke failed: map-object particle frame had {} draws, {} vertices, and {} textures; expected parent and spawned-child PE2, PE1 TGA, and ribbon particle data.",
			particle_frame.draws.size(),
			particle_frame.positions.size(),
			particle_frame.textures.size()
		);
		return false;
	}

	MetalSmokeSurfaceWidget model_preview(parent);
	model_preview.setAttribute(Qt::WA_NativeWindow);
	model_preview.setAttribute(Qt::WA_PaintOnScreen);
	model_preview.setAttribute(Qt::WA_NoSystemBackground);
	model_preview.setAttribute(Qt::WA_OpaquePaintEvent);
	model_preview.resize(256, 256);

	MetalSurface surface;
	hive::metal::MetalMapRenderer renderer;
	std::vector<std::uint8_t> pixels;
	int width = 0;
	int height = 0;
	const bool frame_ok = capture_metal_preview_widget(
		"PE1 spawned-model render",
		model_preview,
		[&](std::vector<std::uint8_t>& captured_pixels, int& captured_width, int& captured_height) {
			if (!surface.initialize(reinterpret_cast<void*>(model_preview.winId()))) {
				std::println("Metal PE1 spawned-model render smoke failed: unable to initialize Metal surface.");
				return false;
			}

			surface.resize(model_preview.width(), model_preview.height(), k_metal_smoke_artifact_scale);
			MetalSurface::Frame surface_frame = surface.begin_frame(0.02f, 0.02f, 0.02f, 1.0f);
			if (!surface_frame) {
				std::println("Metal PE1 spawned-model render smoke failed: unable to begin Metal frame.");
				return false;
			}

			hive::metal::TerrainTextureCpuFrame textures;
			textures.ground_tile_size = 1;
			textures.ground_layer_count = 1;
			textures.ground_texture_layer_offsets = {0u};
			textures.ground_rgba8 = {0u, 0u, 0u, 255u};

			hive::metal::MetalMapFrame frame;
			frame.camera.projection_view = hive::metal::opengl_clip_depth_to_metal(glm::ortho(-180.0f, 220.0f, -150.0f, 150.0f, -512.0f, 512.0f));
			frame.terrain.width = 2;
			frame.terrain.height = 2;
			frame.terrain.corner_ground_heights = {0.0f, 0.0f, 0.0f, 0.0f};
			frame.terrain.final_ground_heights = frame.terrain.corner_ground_heights;
			frame.terrain.ground_texture_indices = {glm::uvec4(0u)};
			frame.terrain.ground_exists = {0u};
			frame.terrain.corner_water_heights = frame.terrain.corner_ground_heights;
			frame.terrain.water_exists = {0u, 0u, 0u, 0u};
			frame.textures = &textures;
			frame.models = model_frame;
			frame.particles = particle_frame;
			frame.show_lighting = false;
			frame.show_water = false;

			hive::metal::FrameContext frame_context {
				.device = surface_frame.device_opaque(),
				.command_queue = surface_frame.command_queue_opaque(),
				.command_buffer = surface_frame.command_buffer_opaque(),
				.render_pass_descriptor = surface_frame.render_pass_descriptor_opaque(),
				.drawable_width = surface_frame.drawable_width(),
				.drawable_height = surface_frame.drawable_height(),
				.scale = surface_frame.scale(),
			};

			bool render_ok = true;
			if (!renderer.render(frame_context, frame)) {
				std::println("Metal PE1 spawned-model render smoke failed: {}", renderer.last_error());
				render_ok = false;
			}

			std::vector<std::uint8_t> frame_pixels;
			if (!surface.end_frame(std::move(surface_frame), &frame_pixels)) {
				std::println("Metal PE1 spawned-model render smoke failed: unable to present/read back frame.");
				return false;
			}

			captured_pixels = std::move(frame_pixels);
			captured_width = frame_context.drawable_width;
			captured_height = frame_context.drawable_height;
			pixels = captured_pixels;
			width = captured_width;
			height = captured_height;
			return render_ok;
		},
		smoke_clear_color(0.02f, 0.02f, 0.02f)
	);

	const bool pixels_ok = frame_ok && validate_pe1_spawn_render_pixels(pixels, width, height);
	return pixels_ok && save_metal_smoke_artifact_png(artifact_directory, "metal-pe1-spawn-render.png", pixels, width, height);
}

bool make_alpha_picking_debug_pixels(
	const std::vector<std::uint8_t>& raw_bgra_pixels,
	const int width,
	const int height,
	std::vector<std::uint8_t>& debug_bgra_pixels
) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || raw_bgra_pixels.size() < pixel_count * 4u) {
		std::println("Metal alpha picking smoke failed: missing picking readback pixels.");
		return false;
	}

	debug_bgra_pixels.assign(pixel_count * 4u, 255u);
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4u;
		const std::uint32_t id = static_cast<std::uint32_t>(raw_bgra_pixels[offset + 2u])
			| (static_cast<std::uint32_t>(raw_bgra_pixels[offset + 1u]) << 8u)
			| (static_cast<std::uint32_t>(raw_bgra_pixels[offset + 0u]) << 16u);

		std::uint8_t debug_b = 0u;
		std::uint8_t debug_g = 0u;
		std::uint8_t debug_r = 0u;
		switch (id) {
			case 1u:
				debug_b = 255u;
				debug_r = 255u;
				break;
			case 2u:
				debug_r = 255u;
				break;
			case 3u:
				debug_g = 255u;
				break;
			case 4u:
				debug_b = 255u;
				break;
			default:
				break;
		}

		debug_bgra_pixels[offset + 0u] = debug_b;
		debug_bgra_pixels[offset + 1u] = debug_g;
		debug_bgra_pixels[offset + 2u] = debug_r;
		debug_bgra_pixels[offset + 3u] = 255u;
	}
	return true;
}

bool validate_alpha_picking_debug_pixels(const std::vector<std::uint8_t>& pixels, const int width, const int height) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4u) {
		std::println("Metal alpha picking smoke failed: missing debug artifact pixels.");
		return false;
	}

	std::size_t leaked_transparent_pixels = 0;
	std::size_t opaque_pixels = 0;
	std::size_t top_pixels = 0;
	std::size_t bottom_pixels = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4u;
		const std::uint8_t b = pixels[offset + 0u];
		const std::uint8_t g = pixels[offset + 1u];
		const std::uint8_t r = pixels[offset + 2u];
		if (r > 200u && b > 200u && g < 40u) {
			++leaked_transparent_pixels;
		} else if (r > 200u && g < 40u && b < 40u) {
			++opaque_pixels;
		} else if (g > 200u && r < 40u && b < 40u) {
			++top_pixels;
		} else if (b > 200u && r < 40u && g < 40u) {
			++bottom_pixels;
		}
	}

	const std::size_t min_pixels = std::max<std::size_t>(128u, pixel_count / 512u);
	if (leaked_transparent_pixels > 0u || opaque_pixels < min_pixels || top_pixels < min_pixels || bottom_pixels < min_pixels) {
		std::println(
			"Metal alpha picking smoke failed: saw {} leaked transparent, {} opaque, {} top, and {} bottom debug pixels, expected no leaks and at least {} of each visible bucket.",
			leaked_transparent_pixels,
			opaque_pixels,
			top_pixels,
			bottom_pixels,
			min_pixels
		);
		return false;
	}

	std::println(
		"Metal alpha picking smoke captured {}x{} debug readback with {} opaque, {} top, and {} bottom pick pixels.",
		width,
		height,
		opaque_pixels,
		top_pixels,
		bottom_pixels
	);
	return true;
}

bool make_pe1_spawn_picking_debug_pixels(
	const std::vector<std::uint8_t>& raw_bgra_pixels,
	const int width,
	const int height,
	std::vector<std::uint8_t>& debug_bgra_pixels,
	const std::uint32_t expected_raw_pick_id
) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || raw_bgra_pixels.size() < pixel_count * 4u) {
		std::println("Metal PE1 spawned-model picking smoke failed: missing picking readback pixels.");
		return false;
	}

	debug_bgra_pixels.assign(pixel_count * 4u, 255u);
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4u;
		const std::uint32_t id = static_cast<std::uint32_t>(raw_bgra_pixels[offset + 2u])
			| (static_cast<std::uint32_t>(raw_bgra_pixels[offset + 1u]) << 8u)
			| (static_cast<std::uint32_t>(raw_bgra_pixels[offset + 0u]) << 16u);

		std::uint8_t debug_b = 0u;
		std::uint8_t debug_g = 0u;
		std::uint8_t debug_r = 0u;
		if (id == expected_raw_pick_id) {
			debug_g = 255u;
		} else if (id != 0u) {
			debug_r = 255u;
		}

		debug_bgra_pixels[offset + 0u] = debug_b;
		debug_bgra_pixels[offset + 1u] = debug_g;
		debug_bgra_pixels[offset + 2u] = debug_r;
		debug_bgra_pixels[offset + 3u] = 255u;
	}
	return true;
}

bool validate_pe1_spawn_picking_debug_pixels(const std::vector<std::uint8_t>& pixels, const int width, const int height) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4u) {
		std::println("Metal PE1 spawned-model picking smoke failed: missing debug artifact pixels.");
		return false;
	}

	std::size_t spawned_child_pixels = 0;
	std::size_t unexpected_pick_pixels = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4u;
		const std::uint8_t b = pixels[offset + 0u];
		const std::uint8_t g = pixels[offset + 1u];
		const std::uint8_t r = pixels[offset + 2u];
		if (g > 200u && r < 40u && b < 40u) {
			++spawned_child_pixels;
		} else if (r > 200u && g < 40u && b < 40u) {
			++unexpected_pick_pixels;
		}
	}

	const std::size_t min_pixels = std::max<std::size_t>(128u, pixel_count / 512u);
	if (spawned_child_pixels < min_pixels || unexpected_pick_pixels > 0u) {
		std::println(
			"Metal PE1 spawned-model picking smoke failed: saw {} spawned-child pick pixels and {} unexpected pick pixels, expected at least {} spawned-child pixels and no unexpected picks.",
			spawned_child_pixels,
			unexpected_pick_pixels,
			min_pixels
		);
		return false;
	}

	std::println(
		"Metal PE1 spawned-model picking smoke captured {}x{} debug readback with {} spawned-child pick pixels.",
		width,
		height,
		spawned_child_pixels
	);
	return true;
}

bool make_meshless_bounds_picking_debug_pixels(
	const std::vector<std::uint8_t>& raw_bgra_pixels,
	const int width,
	const int height,
	std::vector<std::uint8_t>& debug_bgra_pixels,
	const std::uint32_t expected_raw_pick_id
) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || raw_bgra_pixels.size() < pixel_count * 4u) {
		std::println("Metal meshless bounds picking smoke failed: missing picking readback pixels.");
		return false;
	}

	debug_bgra_pixels.assign(pixel_count * 4u, 255u);
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4u;
		const std::uint32_t id = static_cast<std::uint32_t>(raw_bgra_pixels[offset + 2u])
			| (static_cast<std::uint32_t>(raw_bgra_pixels[offset + 1u]) << 8u)
			| (static_cast<std::uint32_t>(raw_bgra_pixels[offset + 0u]) << 16u);

		std::uint8_t debug_b = 0u;
		std::uint8_t debug_g = 0u;
		std::uint8_t debug_r = 0u;
		if (id == expected_raw_pick_id) {
			debug_b = 255u;
		} else if (id != 0u) {
			debug_r = 255u;
		}

		debug_bgra_pixels[offset + 0u] = debug_b;
		debug_bgra_pixels[offset + 1u] = debug_g;
		debug_bgra_pixels[offset + 2u] = debug_r;
		debug_bgra_pixels[offset + 3u] = 255u;
	}
	return true;
}

bool validate_meshless_bounds_picking_debug_pixels(const std::vector<std::uint8_t>& pixels, const int width, const int height) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4u) {
		std::println("Metal meshless bounds picking smoke failed: missing debug artifact pixels.");
		return false;
	}

	std::size_t bounds_pixels = 0;
	std::size_t unexpected_pick_pixels = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4u;
		const std::uint8_t b = pixels[offset + 0u];
		const std::uint8_t g = pixels[offset + 1u];
		const std::uint8_t r = pixels[offset + 2u];
		if (b > 200u && r < 40u && g < 40u) {
			++bounds_pixels;
		} else if (r > 200u && g < 40u && b < 40u) {
			++unexpected_pick_pixels;
		}
	}

	const std::size_t min_pixels = std::max<std::size_t>(128u, pixel_count / 512u);
	if (bounds_pixels < min_pixels || unexpected_pick_pixels > 0u) {
		std::println(
			"Metal meshless bounds picking smoke failed: saw {} bounds pick pixels and {} unexpected pick pixels, expected at least {} bounds pixels and no unexpected picks.",
			bounds_pixels,
			unexpected_pick_pixels,
			min_pixels
		);
		return false;
	}

	std::println(
		"Metal meshless bounds picking smoke captured {}x{} debug readback with {} bounds pick pixels.",
		width,
		height,
		bounds_pixels
	);
	return true;
}

bool run_metal_alpha_picking_smoke(QWidget* parent, const QString& artifact_directory) {
	MetalSmokeSurfaceWidget picking_preview(parent);
	picking_preview.setAttribute(Qt::WA_NativeWindow);
	picking_preview.setAttribute(Qt::WA_PaintOnScreen);
	picking_preview.setAttribute(Qt::WA_NoSystemBackground);
	picking_preview.setAttribute(Qt::WA_OpaquePaintEvent);
	picking_preview.resize(256, 256);
	picking_preview.show();
	picking_preview.raise();
	picking_preview.update();

	bool ok = false;
	bool completed = false;
	QEventLoop loop;
	QTimer::singleShot(250, &loop, [&] {
		QCoreApplication::processEvents(QEventLoop::AllEvents, 100);

		MetalSurface surface;
		if (!surface.initialize(reinterpret_cast<void*>(picking_preview.winId()))) {
			std::println("Metal alpha picking smoke failed: unable to initialize Metal surface.");
			completed = true;
			loop.quit();
			return;
		}
		static constexpr double forced_pick_scale = 2.0;
		surface.resize(picking_preview.width(), picking_preview.height(), forced_pick_scale);
		MetalSurface::Frame surface_frame = surface.begin_frame(0.0f, 0.0f, 0.0f, 1.0f);
		if (!surface_frame) {
			std::println("Metal alpha picking smoke failed: unable to begin Metal frame.");
			completed = true;
			loop.quit();
			return;
		}

		hive::metal::FrameContext frame_context {
			.device = surface_frame.device_opaque(),
			.command_queue = surface_frame.command_queue_opaque(),
			.command_buffer = surface_frame.command_buffer_opaque(),
			.render_pass_descriptor = surface_frame.render_pass_descriptor_opaque(),
			.drawable_width = surface_frame.drawable_width(),
			.drawable_height = surface_frame.drawable_height(),
			.scale = surface_frame.scale(),
		};

		std::vector<glm::uvec2> vertices;
		std::vector<std::uint32_t> uvs;
		std::vector<std::uint16_t> indices;
		auto append_quad = [&](const float left, const float right, const float bottom, const float top) {
			const std::uint16_t base = static_cast<std::uint16_t>(vertices.size());
			vertices.push_back(pack_vec3_to_uvec2(glm::vec3(left, bottom, 0.45f), 8192.0f));
			vertices.push_back(pack_vec3_to_uvec2(glm::vec3(right, bottom, 0.45f), 8192.0f));
			vertices.push_back(pack_vec3_to_uvec2(glm::vec3(left, top, 0.45f), 8192.0f));
			vertices.push_back(pack_vec3_to_uvec2(glm::vec3(right, top, 0.45f), 8192.0f));
			uvs.push_back(pack_smoke_model_uv(glm::vec2(0.0f, 1.0f)));
			uvs.push_back(pack_smoke_model_uv(glm::vec2(1.0f, 1.0f)));
			uvs.push_back(pack_smoke_model_uv(glm::vec2(0.0f, 0.0f)));
			uvs.push_back(pack_smoke_model_uv(glm::vec2(1.0f, 0.0f)));
			const std::uint32_t first_index = static_cast<std::uint32_t>(indices.size());
			indices.insert(indices.end(), {base, static_cast<std::uint16_t>(base + 1u), static_cast<std::uint16_t>(base + 2u),
										   static_cast<std::uint16_t>(base + 1u), static_cast<std::uint16_t>(base + 3u),
										   static_cast<std::uint16_t>(base + 2u)});
			return first_index;
		};
		const std::uint32_t transparent_first_index = append_quad(-0.78f, -0.08f, -0.65f, 0.65f);
		const std::uint32_t opaque_first_index = append_quad(0.08f, 0.78f, -0.65f, 0.65f);
		const std::uint32_t top_first_index = append_quad(-0.06f, 0.06f, 0.72f, 0.92f);
		const std::uint32_t bottom_first_index = append_quad(-0.06f, 0.06f, -0.92f, -0.72f);
		const std::uint32_t normal = pack_smoke_model_normal(glm::vec3(0.0f, 0.0f, 1.0f));
		std::vector<std::uint32_t> normals(vertices.size(), normal);
		std::vector<glm::vec4> tangents(vertices.size(), glm::vec4(1.0f, 0.0f, 0.0f, 1.0f));
		std::vector<glm::u8vec4> skin_weights;
		skin_weights.reserve(vertices.size() * 2u);
		for (std::size_t i = 0; i < vertices.size(); ++i) {
			skin_weights.push_back(glm::u8vec4(0u, 0u, 0u, 0u));
			skin_weights.push_back(glm::u8vec4(255u, 0u, 0u, 0u));
		}
		static constexpr std::uint8_t transparent_texture_key = 0;
		static constexpr std::uint8_t opaque_texture_key = 1;
		static constexpr std::uint8_t alpha_pick_mesh_key = 2;
		std::vector<std::uint8_t> transparent_texture {255u, 255u, 255u, 64u};
		std::vector<std::uint8_t> opaque_texture {255u, 255u, 255u, 255u};
		auto texture_source = [](const void* key, const std::vector<std::uint8_t>& rgba8) {
			return hive::metal::ModelTextureCpuSource {
				.key = key,
				.width = 1,
				.height = 1,
				.channels = 4,
				.wrap_flags = 0,
				.srgb = false,
				.rgba8 = &rgba8,
			};
		};

		hive::metal::ModelCpuFrame model_frame;
		model_frame.meshes.push_back(hive::metal::ModelMeshCpuFrame {
			.key = &alpha_pick_mesh_key,
			.revision = 1,
			.vertices_snorm = &vertices,
			.uvs_snorm = &uvs,
			.normals_oct_snorm = &normals,
			.tangents = &tangents,
			.skin_weights = &skin_weights,
			.indices = &indices,
			.textures = {
				texture_source(&transparent_texture_key, transparent_texture),
				texture_source(&opaque_texture_key, opaque_texture),
			},
		});
		model_frame.instance_matrices = {glm::mat4(1.0f)};
		model_frame.team_color_indexes = {0u};
		model_frame.bone_matrices = {glm::mat4(1.0f)};
		model_frame.layer_colors = {glm::vec4(1.0f)};

		auto make_picking_draw = [](const std::uint32_t first_index, const std::uint32_t texture_index, const std::uint32_t pick_id) {
			hive::metal::ModelDrawCallCpuFrame draw;
			draw.mesh_index = 0;
			draw.index_count = 6;
			draw.first_index = first_index;
			draw.instance_offset = 0;
			draw.bone_offset = 0;
			draw.bone_count = 1;
			draw.layer_color_offset = 0;
			draw.layer_skip_count = 1;
			draw.layer_index_local = 0;
			draw.texture_indices = {texture_index, texture_index, texture_index, texture_index, texture_index, texture_index};
			draw.alpha_test = 0.75f;
			draw.pick_id = pick_id;
			draw.cull_face = 0u;
			draw.depth_test = 0u;
			draw.depth_mask = 0u;
			draw.picking_depth_mask = 0u;
			return draw;
		};
		model_frame.picking_draws.push_back(make_picking_draw(transparent_first_index, 0u, 1u));
		model_frame.picking_draws.push_back(make_picking_draw(opaque_first_index, 1u, 2u));
		model_frame.picking_draws.push_back(make_picking_draw(top_first_index, 1u, 3u));
		model_frame.picking_draws.push_back(make_picking_draw(bottom_first_index, 1u, 4u));

		hive::metal::MetalMapRenderer renderer;
		const hive::metal::CameraUniforms camera {glm::mat4(1.0f)};
		const float logical_width = static_cast<float>(frame_context.drawable_width) / static_cast<float>(std::max(frame_context.scale, 1.0));
		const float logical_height = static_cast<float>(frame_context.drawable_height) / static_cast<float>(std::max(frame_context.scale, 1.0));
		const float pick_scale = static_cast<float>(std::max(frame_context.scale, 1.0));
		const auto transparent_pick = renderer.pick_model_id(frame_context, model_frame, camera, glm::vec2(logical_width * 0.25f, logical_height * 0.50f));
		const auto opaque_pick = renderer.pick_model_id(frame_context, model_frame, camera, glm::vec2(logical_width * 0.75f, logical_height * 0.50f));
		const auto top_pick = renderer.pick_model_id(frame_context, model_frame, camera, glm::vec2(logical_width * 0.50f, logical_height * 0.10f));
		const auto bottom_pick = renderer.pick_model_id(frame_context, model_frame, camera, glm::vec2(logical_width * 0.50f, logical_height * 0.90f));
		hive::metal::FrameContext subregion_context = frame_context;
		subregion_context.viewport_x = frame_context.drawable_width / 4;
		subregion_context.viewport_y = frame_context.drawable_height / 4;
		subregion_context.viewport_width = std::max(1, frame_context.drawable_width / 2);
		subregion_context.viewport_height = std::max(1, frame_context.drawable_height / 2);
		subregion_context.scissor_x = subregion_context.viewport_x;
		subregion_context.scissor_y = subregion_context.viewport_y;
		subregion_context.scissor_width = subregion_context.viewport_width;
		subregion_context.scissor_height = subregion_context.viewport_height;
		const auto subregion_opaque_pick = renderer.pick_model_id(
			subregion_context,
			model_frame,
			camera,
			glm::vec2(
				static_cast<float>(
					static_cast<double>(subregion_context.viewport_x)
					+ static_cast<double>(subregion_context.viewport_width) * 0.75
				) / pick_scale,
				static_cast<float>(
					static_cast<double>(subregion_context.viewport_y)
					+ static_cast<double>(subregion_context.viewport_height) * 0.50
				) / pick_scale
			)
		);
		const auto subregion_outside_pick = renderer.pick_model_id(
			subregion_context,
			model_frame,
			camera,
			glm::vec2(
				static_cast<float>(subregion_context.viewport_x / 2) / pick_scale,
				static_cast<float>(
					static_cast<double>(subregion_context.viewport_y)
					+ static_cast<double>(subregion_context.viewport_height) * 0.50
				) / pick_scale
			)
		);
		std::vector<std::uint8_t> raw_picking_pixels;
		std::vector<std::uint8_t> debug_picking_pixels;
		int picking_width = 0;
		int picking_height = 0;
		bool picking_artifact_ok = renderer.capture_model_picking_frame(frame_context, model_frame, camera, raw_picking_pixels, picking_width, picking_height);
		if (!picking_artifact_ok) {
			std::println("Metal alpha picking smoke failed: {}.", renderer.last_error());
		} else {
			picking_artifact_ok = make_alpha_picking_debug_pixels(raw_picking_pixels, picking_width, picking_height, debug_picking_pixels)
				&& validate_alpha_picking_debug_pixels(debug_picking_pixels, picking_width, picking_height)
				&& save_metal_smoke_artifact_png(
					artifact_directory,
					"metal-alpha-picking.png",
					debug_picking_pixels,
					picking_width,
					picking_height
				);
		}
		std::vector<std::uint8_t> ignored_pixels;
		const bool present_ok = surface.end_frame(std::move(surface_frame), &ignored_pixels);
		if (!present_ok) {
			std::println("Metal alpha picking smoke failed: unable to present/read back frame.");
		}

		if (transparent_pick) {
			std::println("Metal alpha picking smoke failed: transparent alpha-tested quad returned pick {}.", *transparent_pick);
		}
		if (opaque_pick != std::optional<std::uint32_t>(1u)) {
			std::println(
				"Metal alpha picking smoke failed: opaque alpha-tested quad returned {}, expected 1.",
				opaque_pick ? std::to_string(*opaque_pick) : std::string("no pick")
			);
		}
		if (top_pick != std::optional<std::uint32_t>(2u)) {
			std::println(
				"Metal alpha picking smoke failed: scaled top-origin probe returned {}, expected 2.",
				top_pick ? std::to_string(*top_pick) : std::string("no pick")
			);
		}
		if (bottom_pick != std::optional<std::uint32_t>(3u)) {
			std::println(
				"Metal alpha picking smoke failed: scaled bottom-origin probe returned {}, expected 3.",
				bottom_pick ? std::to_string(*bottom_pick) : std::string("no pick")
			);
		}
		if (subregion_opaque_pick != std::optional<std::uint32_t>(1u)) {
			std::println(
				"Metal alpha picking smoke failed: sub-viewport opaque probe returned {}, expected 1.",
				subregion_opaque_pick ? std::to_string(*subregion_opaque_pick) : std::string("no pick")
			);
		}
		if (subregion_outside_pick) {
			std::println(
				"Metal alpha picking smoke failed: sub-viewport outside probe returned {}, expected no pick.",
				*subregion_outside_pick
			);
		}

		const bool forced_scale_ok = frame_context.scale >= forced_pick_scale - 0.01
			&& frame_context.drawable_width >= static_cast<int>(std::floor(static_cast<double>(picking_preview.width()) * forced_pick_scale))
			&& frame_context.drawable_height >= static_cast<int>(std::floor(static_cast<double>(picking_preview.height()) * forced_pick_scale));
		if (!forced_scale_ok) {
			std::println(
				"Metal alpha picking smoke failed: requested {}x scale but got scale {} and drawable {}x{} for widget {}x{}.",
				forced_pick_scale,
				frame_context.scale,
				frame_context.drawable_width,
				frame_context.drawable_height,
				picking_preview.width(),
				picking_preview.height()
			);
		}

		ok = present_ok && forced_scale_ok && !transparent_pick && opaque_pick == std::optional<std::uint32_t>(1u)
			&& top_pick == std::optional<std::uint32_t>(2u) && bottom_pick == std::optional<std::uint32_t>(3u)
			&& subregion_opaque_pick == std::optional<std::uint32_t>(1u) && !subregion_outside_pick && picking_artifact_ok;
		if (ok) {
			std::println(
				"Metal alpha picking smoke rejected transparent alpha-tested pixels and validated scaled top-left and sub-viewport pick coordinates."
			);
		}
		completed = true;
		loop.quit();
	});
	QTimer::singleShot(5000, &loop, [&] {
		if (!completed) {
			std::println("Metal alpha picking smoke failed: timed out waiting for a preview surface.");
			ok = false;
			loop.quit();
		}
	});
	loop.exec();

	picking_preview.close();
	return ok;
}

bool run_metal_pe1_spawned_model_picking_smoke(QWidget* parent, const QString& artifact_directory) {
	MetalSmokeSurfaceWidget picking_preview(parent);
	picking_preview.setAttribute(Qt::WA_NativeWindow);
	picking_preview.setAttribute(Qt::WA_PaintOnScreen);
	picking_preview.setAttribute(Qt::WA_NoSystemBackground);
	picking_preview.setAttribute(Qt::WA_OpaquePaintEvent);
	picking_preview.resize(256, 256);
	picking_preview.show();
	picking_preview.raise();
	picking_preview.update();

	bool ok = false;
	bool completed = false;
	QEventLoop loop;
	QTimer::singleShot(250, &loop, [&] {
		QCoreApplication::processEvents(QEventLoop::AllEvents, 100);

		MetalSurface surface;
		if (!surface.initialize(reinterpret_cast<void*>(picking_preview.winId()))) {
			std::println("Metal PE1 spawned-model picking smoke failed: unable to initialize Metal surface.");
			completed = true;
			loop.quit();
			return;
		}

		static constexpr double forced_pick_scale = 2.0;
		surface.resize(picking_preview.width(), picking_preview.height(), forced_pick_scale);
		MetalSurface::Frame surface_frame = surface.begin_frame(0.0f, 0.0f, 0.0f, 1.0f);
		if (!surface_frame) {
			std::println("Metal PE1 spawned-model picking smoke failed: unable to begin Metal frame.");
			completed = true;
			loop.quit();
			return;
		}

		hive::metal::FrameContext frame_context {
			.device = surface_frame.device_opaque(),
			.command_queue = surface_frame.command_queue_opaque(),
			.command_buffer = surface_frame.command_buffer_opaque(),
			.render_pass_descriptor = surface_frame.render_pass_descriptor_opaque(),
			.drawable_width = surface_frame.drawable_width(),
			.drawable_height = surface_frame.drawable_height(),
			.scale = surface_frame.scale(),
		};

		std::vector<glm::uvec2> vertices {
			pack_vec3_to_uvec2(glm::vec3(-0.22f, -0.22f, 0.45f), 8192.0f),
			pack_vec3_to_uvec2(glm::vec3(0.22f, -0.22f, 0.45f), 8192.0f),
			pack_vec3_to_uvec2(glm::vec3(-0.22f, 0.22f, 0.45f), 8192.0f),
			pack_vec3_to_uvec2(glm::vec3(0.22f, 0.22f, 0.45f), 8192.0f),
		};
		std::vector<std::uint32_t> uvs {
			pack_smoke_model_uv(glm::vec2(0.0f, 1.0f)),
			pack_smoke_model_uv(glm::vec2(1.0f, 1.0f)),
			pack_smoke_model_uv(glm::vec2(0.0f, 0.0f)),
			pack_smoke_model_uv(glm::vec2(1.0f, 0.0f)),
		};
		const std::uint32_t normal = pack_smoke_model_normal(glm::vec3(0.0f, 0.0f, 1.0f));
		std::vector<std::uint32_t> normals(vertices.size(), normal);
		std::vector<glm::vec4> tangents(vertices.size(), glm::vec4(1.0f, 0.0f, 0.0f, 1.0f));
		std::vector<glm::u8vec4> skin_weights;
		skin_weights.reserve(vertices.size() * 2u);
		for (std::size_t i = 0; i < vertices.size(); ++i) {
			skin_weights.push_back(glm::u8vec4(0u, 0u, 0u, 0u));
			skin_weights.push_back(glm::u8vec4(255u, 0u, 0u, 0u));
		}
		std::vector<std::uint16_t> indices {0u, 1u, 2u, 1u, 3u, 2u};
		static constexpr std::uint8_t spawned_model_mesh_key = 0;
		static constexpr std::uint8_t spawned_model_texture_key = 1;
		std::vector<std::uint8_t> opaque_texture {255u, 255u, 255u, 255u};
		hive::metal::ModelCpuFrame model_frame;
		model_frame.meshes.push_back(hive::metal::ModelMeshCpuFrame {
			.key = &spawned_model_mesh_key,
			.revision = 1,
			.vertices_snorm = &vertices,
			.uvs_snorm = &uvs,
			.normals_oct_snorm = &normals,
			.tangents = &tangents,
			.skin_weights = &skin_weights,
			.indices = &indices,
			.textures = {
				hive::metal::ModelTextureCpuSource {
					.key = &spawned_model_texture_key,
					.width = 1,
					.height = 1,
					.channels = 4,
					.wrap_flags = 0,
					.srgb = false,
					.rgba8 = &opaque_texture,
				},
			},
		});

		const glm::mat4 spawn_instance = glm::translate(glm::mat4(1.0f), glm::vec3(0.25f, -0.10f, 0.0f));
		const glm::mat4 child_bone = glm::translate(glm::mat4(1.0f), glm::vec3(0.15f, 0.05f, 0.0f));
		model_frame.instance_matrices = {spawn_instance};
		model_frame.team_color_indexes = {0u};
		model_frame.bone_matrices = {child_bone};
		model_frame.layer_colors = {glm::vec4(1.0f)};

		static constexpr std::uint32_t parent_pick_id = 17u;
		hive::metal::ModelDrawCallCpuFrame spawned_child_draw;
		spawned_child_draw.mesh_index = 0;
		spawned_child_draw.index_count = static_cast<std::uint32_t>(indices.size());
		spawned_child_draw.instance_offset = 0;
		spawned_child_draw.bone_offset = 0;
		spawned_child_draw.bone_count = 1;
		spawned_child_draw.layer_color_offset = 0;
		spawned_child_draw.layer_skip_count = 1;
		spawned_child_draw.layer_index_local = 0;
		spawned_child_draw.texture_indices = {0u, 0u, 0u, 0u, 0u, 0u};
		spawned_child_draw.alpha_test = 0.01f;
		spawned_child_draw.pick_id = parent_pick_id;
		spawned_child_draw.cull_face = 0u;
		spawned_child_draw.depth_test = 1u;
		spawned_child_draw.depth_mask = 1u;
		spawned_child_draw.picking_depth_mask = 1u;
		model_frame.picking_draws.push_back(spawned_child_draw);

		hive::metal::MetalMapRenderer renderer;
		const hive::metal::CameraUniforms camera {glm::mat4(1.0f)};
		const float logical_width = static_cast<float>(frame_context.drawable_width) / static_cast<float>(std::max(frame_context.scale, 1.0));
		const float logical_height = static_cast<float>(frame_context.drawable_height) / static_cast<float>(std::max(frame_context.scale, 1.0));
		const auto spawned_pick = renderer.pick_model_id(frame_context, model_frame, camera, glm::vec2(logical_width * 0.70f, logical_height * 0.525f));

		std::vector<std::uint8_t> raw_picking_pixels;
		std::vector<std::uint8_t> debug_picking_pixels;
		int picking_width = 0;
		int picking_height = 0;
		bool picking_artifact_ok =
			renderer.capture_model_picking_frame(frame_context, model_frame, camera, raw_picking_pixels, picking_width, picking_height);
		if (!picking_artifact_ok) {
			std::println("Metal PE1 spawned-model picking smoke failed: {}.", renderer.last_error());
		} else {
			picking_artifact_ok = make_pe1_spawn_picking_debug_pixels(
									  raw_picking_pixels,
									  picking_width,
									  picking_height,
									  debug_picking_pixels,
									  parent_pick_id
								  )
				&& validate_pe1_spawn_picking_debug_pixels(debug_picking_pixels, picking_width, picking_height)
				&& save_metal_smoke_artifact_png(
					artifact_directory,
					"metal-pe1-spawn-picking.png",
					debug_picking_pixels,
					picking_width,
					picking_height
				);
		}

		std::vector<std::uint8_t> ignored_pixels;
		const bool present_ok = surface.end_frame(std::move(surface_frame), &ignored_pixels);
		if (!present_ok) {
			std::println("Metal PE1 spawned-model picking smoke failed: unable to present/read back frame.");
		}

		const std::optional<std::uint32_t> expected_pick(parent_pick_id - 1u);
		if (spawned_pick != expected_pick) {
			std::println(
				"Metal PE1 spawned-model picking smoke failed: spawned child returned {}, expected parent pick index {}.",
				spawned_pick ? std::to_string(*spawned_pick) : std::string("no pick"),
				*expected_pick
			);
		}

		ok = present_ok && picking_artifact_ok && spawned_pick == expected_pick;
		if (ok) {
			std::println("Metal PE1 spawned-model picking smoke returned the parent pick index for spawned child geometry.");
		}
		completed = true;
		loop.quit();
	});
	QTimer::singleShot(5000, &loop, [&] {
		if (!completed) {
			std::println("Metal PE1 spawned-model picking smoke failed: timed out waiting for a preview surface.");
			ok = false;
			loop.quit();
		}
	});
	loop.exec();

	picking_preview.close();
	return ok;
}

bool run_metal_meshless_bounds_picking_smoke(QWidget* parent, const QString& artifact_directory) {
	struct MeshlessPickingMesh {
		bool has_mesh = false;
		std::shared_ptr<mdx::MDX> mdx;
	};

	struct MeshlessPickingSkeleton {
		int sequence_index = 0;
		glm::mat4 matrix = glm::mat4(1.0f);
	};

	struct MeshlessPickingCamera {
		bool inside_frustrum_transform(const glm::vec3&, const glm::vec3&, const glm::mat4&) const {
			return true;
		}
	};

	MetalSmokeSurfaceWidget picking_preview(parent);
	picking_preview.setAttribute(Qt::WA_NativeWindow);
	picking_preview.setAttribute(Qt::WA_PaintOnScreen);
	picking_preview.setAttribute(Qt::WA_NoSystemBackground);
	picking_preview.setAttribute(Qt::WA_OpaquePaintEvent);
	picking_preview.resize(256, 256);
	picking_preview.show();
	picking_preview.raise();
	picking_preview.update();

	bool ok = false;
	bool completed = false;
	QEventLoop loop;
	QTimer::singleShot(250, &loop, [&] {
		QCoreApplication::processEvents(QEventLoop::AllEvents, 100);

		MetalSurface surface;
		if (!surface.initialize(reinterpret_cast<void*>(picking_preview.winId()))) {
			std::println("Metal meshless bounds picking smoke failed: unable to initialize Metal surface.");
			completed = true;
			loop.quit();
			return;
		}

		static constexpr double forced_pick_scale = 2.0;
		surface.resize(picking_preview.width(), picking_preview.height(), forced_pick_scale);
		MetalSurface::Frame surface_frame = surface.begin_frame(0.0f, 0.0f, 0.0f, 1.0f);
		if (!surface_frame) {
			std::println("Metal meshless bounds picking smoke failed: unable to begin Metal frame.");
			completed = true;
			loop.quit();
			return;
		}

		hive::metal::FrameContext frame_context {
			.device = surface_frame.device_opaque(),
			.command_queue = surface_frame.command_queue_opaque(),
			.command_buffer = surface_frame.command_buffer_opaque(),
			.render_pass_descriptor = surface_frame.render_pass_descriptor_opaque(),
			.drawable_width = surface_frame.drawable_width(),
			.drawable_height = surface_frame.drawable_height(),
			.scale = surface_frame.scale(),
		};

		auto mesh = std::make_shared<MeshlessPickingMesh>();
		mesh->mdx = std::make_shared<mdx::MDX>();
		mesh->mdx->extent.bounds_radius = 96.0f;
		mesh->mdx->extent.minimum = glm::vec3(-0.45f, -0.30f, -0.50f);
		mesh->mdx->extent.maximum = glm::vec3(0.45f, 0.30f, 0.50f);
		mdx::Sequence sequence {};
		sequence.name = "Stand";
		sequence.extent = mesh->mdx->extent;
		mesh->mdx->sequences.push_back(sequence);

		MeshlessPickingSkeleton skeleton;
		skeleton.matrix = glm::translate(glm::mat4(1.0f), glm::vec3(0.12f, -0.08f, 0.0f));

		static constexpr std::uint32_t bounds_pick_id = 23u;
		hive::metal::ModelCpuFrame model_frame;
		std::unordered_map<const void*, std::uint32_t> mesh_indices;
		hive::metal::detail::append_model_bounds_picking_instance(
			model_frame,
			mesh_indices,
			MeshlessPickingCamera {},
			mesh,
			skeleton,
			0u,
			bounds_pick_id
		);

		if (model_frame.picking_draws.empty()) {
			std::println("Metal meshless bounds picking smoke failed: CPU frame emitted no bounds picking draw.");
			std::vector<std::uint8_t> ignored_pixels;
			surface.end_frame(std::move(surface_frame), &ignored_pixels);
			completed = true;
			loop.quit();
			return;
		}

		hive::metal::MetalMapRenderer renderer;
		const hive::metal::CameraUniforms camera {
			glm::translate(glm::mat4(1.0f), glm::vec3(0.0f, 0.0f, 0.25f))
				* glm::scale(glm::mat4(1.0f), glm::vec3(0.30f, 0.30f, 0.25f))
		};
		const float logical_width = static_cast<float>(frame_context.drawable_width) / static_cast<float>(std::max(frame_context.scale, 1.0));
		const float logical_height = static_cast<float>(frame_context.drawable_height) / static_cast<float>(std::max(frame_context.scale, 1.0));
		const auto bounds_pick = renderer.pick_model_id(frame_context, model_frame, camera, glm::vec2(logical_width * 0.54f, logical_height * 0.54f));

		std::vector<std::uint8_t> raw_picking_pixels;
		std::vector<std::uint8_t> debug_picking_pixels;
		int picking_width = 0;
		int picking_height = 0;
		bool picking_artifact_ok =
			renderer.capture_model_picking_frame(frame_context, model_frame, camera, raw_picking_pixels, picking_width, picking_height);
		if (!picking_artifact_ok) {
			std::println("Metal meshless bounds picking smoke failed: {}.", renderer.last_error());
		} else {
			picking_artifact_ok = make_meshless_bounds_picking_debug_pixels(
									  raw_picking_pixels,
									  picking_width,
									  picking_height,
									  debug_picking_pixels,
									  bounds_pick_id
								  )
				&& validate_meshless_bounds_picking_debug_pixels(debug_picking_pixels, picking_width, picking_height)
				&& save_metal_smoke_artifact_png(
					artifact_directory,
					"metal-meshless-bounds-picking.png",
					debug_picking_pixels,
					picking_width,
					picking_height
				);
		}

		std::vector<std::uint8_t> ignored_pixels;
		const bool present_ok = surface.end_frame(std::move(surface_frame), &ignored_pixels);
		if (!present_ok) {
			std::println("Metal meshless bounds picking smoke failed: unable to present/read back frame.");
		}

		const std::optional<std::uint32_t> expected_pick(bounds_pick_id - 1u);
		if (bounds_pick != expected_pick) {
			std::println(
				"Metal meshless bounds picking smoke failed: bounds draw returned {}, expected pick index {}.",
				bounds_pick ? std::to_string(*bounds_pick) : std::string("no pick"),
				*expected_pick
			);
		}

		ok = present_ok && picking_artifact_ok && bounds_pick == expected_pick;
		if (ok) {
			std::println("Metal meshless bounds picking smoke returned the meshless model bounds pick index.");
		}
		completed = true;
		loop.quit();
	});
	QTimer::singleShot(5000, &loop, [&] {
		if (!completed) {
			std::println("Metal meshless bounds picking smoke failed: timed out waiting for a preview surface.");
			ok = false;
			loop.quit();
		}
	});
	loop.exec();

	picking_preview.close();
	return ok;
}

bool run_metal_renderer_probe_smoke_impl(QWidget* parent, const QString& artifact_directory) {
	const bool terrain_material_ok = run_metal_terrain_material_smoke_impl(parent, artifact_directory);
	const bool pathing_overlay_ok = run_metal_pathing_overlay_smoke_impl(parent, artifact_directory);
	const bool cliff_pathing_overlay_ok = run_metal_cliff_pathing_overlay_smoke_impl(parent, artifact_directory);
	const bool model_material_ok = run_metal_model_material_smoke(parent, artifact_directory);
	const bool model_depth_isolation_ok = run_metal_model_depth_isolation_smoke(parent, artifact_directory);
	const bool alpha_picking_ok = run_metal_alpha_picking_smoke(parent, artifact_directory);
	const bool pe1_spawn_picking_ok = run_metal_pe1_spawned_model_picking_smoke(parent, artifact_directory);
	const bool pe1_spawn_render_ok = run_metal_pe1_spawned_model_render_smoke(parent, artifact_directory);
	const bool meshless_bounds_picking_ok = run_metal_meshless_bounds_picking_smoke(parent, artifact_directory);
	const bool selection_overlay_ok = run_metal_selection_overlay_smoke(parent, artifact_directory);
	const bool particle_ok = run_metal_particle_renderer_smoke(parent, artifact_directory);
	return terrain_material_ok && pathing_overlay_ok && cliff_pathing_overlay_ok && model_material_ok && alpha_picking_ok
		&& model_depth_isolation_ok && pe1_spawn_picking_ok && pe1_spawn_render_ok && meshless_bounds_picking_ok
		&& selection_overlay_ok && particle_ok;
}

bool run_metal_preview_probe_smoke_impl(QWidget* parent, const QString& artifact_directory) {
	QTemporaryDir synthetic_preview_dir(QDir::tempPath() + QStringLiteral("/HiveWE-synthetic-preview-probes-XXXXXX"));
	bool material_preview_files_ok = false;
	bool particle_preview_files_ok = false;
	bool ribbon_preview_files_ok = false;
	bool multicell_preview_files_ok = false;
	std::shared_ptr<mdx::MDX> material_model;
	std::shared_ptr<mdx::MDX> particle_model;
	std::shared_ptr<mdx::MDX> particle_spawn_model;
	std::shared_ptr<mdx::MDX> particle_spawn_grandchild_model;
	std::shared_ptr<mdx::MDX> ribbon_model;
	std::shared_ptr<mdx::MDX> multicell_model;
	std::shared_ptr<mdx::MDX> sequence_less_material_model;
	fs::path material_model_path;
	fs::path sequence_less_material_model_path;
	fs::path particle_model_path;
	fs::path particle_spawn_model_path;
	fs::path particle_spawn_grandchild_model_path;
	fs::path ribbon_model_path;
	fs::path multicell_model_path;
	fs::path multicell_green_team_color_path;
	fs::path multicell_blue_team_color_path;
	std::optional<std::pair<int, std::string>> team_color_override;
	if (!synthetic_preview_dir.isValid()) {
		std::println("Metal synthetic preview probe smoke failed: unable to create a temporary model directory.");
	} else {
		const fs::path synthetic_preview_path = fs::path(synthetic_preview_dir.path().toStdString());

		std::vector<fs::path> material_texture_paths;
		material_texture_paths.reserve(13);
		for (std::size_t i = 0; i < 13; ++i) {
			material_texture_paths.push_back(synthetic_preview_path / ("material-" + std::to_string(i) + ".tga"));
		}
		material_model_path = synthetic_preview_path / "material-preview.mdx";
		const fs::path team_color_texture_path = synthetic_preview_path / "team-color.tga";
		team_color_override = std::make_pair(1, team_color_texture_path.string());
		material_model = make_material_preview_smoke_model(material_texture_paths);
		sequence_less_material_model = std::make_shared<mdx::MDX>(*material_model);
		sequence_less_material_model->name = "smoke/sequence-less-material-preview.mdx";
		sequence_less_material_model->sequences.clear();
		sequence_less_material_model_path = synthetic_preview_path / "sequence-less-material-preview.mdx";
		material_preview_files_ok = write_smoke_tga_texture(team_color_texture_path, 255u, 32u, 32u, 255u)
			&& write_material_preview_textures(material_texture_paths)
			&& write_preview_model(material_model, material_model_path, "material preview")
			&& write_preview_model(sequence_less_material_model, sequence_less_material_model_path, "sequence-less material preview");

		const fs::path particle_texture_path = synthetic_preview_path / "particle-smoke.tga";
		const fs::path particle_pe1_tga_texture_path = synthetic_preview_path / "particle-pe1-tga-magenta.tga";
		const fs::path particle_ribbon_texture_path = synthetic_preview_path / "particle-ribbon-cyan.tga";
		const fs::path particle_spawn_child_texture_path = synthetic_preview_path / "particle-spawn-child-green.tga";
		const fs::path particle_spawn_grandchild_texture_path = synthetic_preview_path / "particle-spawn-grandchild-blue.tga";
		particle_model_path = synthetic_preview_path / "particle-only.mdx";
		particle_spawn_model_path = synthetic_preview_path / "particle-spawn-child.mdx";
		particle_spawn_grandchild_model_path = synthetic_preview_path / "particle-spawn-grandchild.mdx";
		particle_spawn_grandchild_model = make_particle_spawn_smoke_model(particle_spawn_grandchild_texture_path);
		particle_spawn_model = make_particle_spawn_smoke_model(particle_spawn_child_texture_path, particle_spawn_grandchild_model_path);
		particle_model = make_particle_preview_smoke_model(
			particle_texture_path,
			particle_pe1_tga_texture_path,
			particle_ribbon_texture_path,
			particle_spawn_model_path
		);
		particle_preview_files_ok = write_particle_preview_texture(particle_texture_path)
			&& write_smoke_tga_texture(particle_pe1_tga_texture_path, 255u, 0u, 255u, 255u)
			&& write_smoke_tga_texture(particle_ribbon_texture_path, 0u, 255u, 255u, 255u)
			&& write_smoke_tga_texture(particle_spawn_child_texture_path, 40u, 220u, 80u, 255u)
			&& write_smoke_tga_texture(particle_spawn_grandchild_texture_path, 40u, 80u, 255u, 255u)
			&& write_preview_model(particle_spawn_grandchild_model, particle_spawn_grandchild_model_path, "particle spawn grandchild preview")
			&& write_preview_model(particle_spawn_model, particle_spawn_model_path, "particle spawn child preview")
			&& write_preview_model(particle_model, particle_model_path, "particle preview");

		const fs::path ribbon_texture_path = synthetic_preview_path / "ribbon-preview-cyan.tga";
		ribbon_model_path = synthetic_preview_path / "ribbon-preview.mdx";
		ribbon_model = make_ribbon_preview_smoke_model(ribbon_texture_path);
		ribbon_preview_files_ok = write_smoke_tga_texture(ribbon_texture_path, 0u, 255u, 255u, 255u)
			&& write_preview_model(ribbon_model, ribbon_model_path, "ribbon preview");

		multicell_model_path = synthetic_preview_path / "multicell-shared-preview.mdx";
		multicell_green_team_color_path = synthetic_preview_path / "multicell-green-team-color.tga";
		multicell_blue_team_color_path = synthetic_preview_path / "multicell-blue-team-color.tga";
		multicell_model = make_multicell_replaceable_preview_smoke_model();
		multicell_preview_files_ok =
			write_smoke_tga_texture(multicell_green_team_color_path, 40u, 220u, 80u, 255u)
			&& write_smoke_tga_texture(multicell_blue_team_color_path, 40u, 80u, 255u, 255u)
			&& write_preview_model(multicell_model, multicell_model_path, "multicell shared material preview");
	}

	bool model_editor_material_artifact_ok = false;
	bool object_grid_material_artifact_ok = false;
	bool object_grid_sequence_less_material_ok = false;
	bool object_grid_multicell_artifact_ok = false;
	bool object_grid_scrolled_artifact_ok = false;
	bool object_grid_empty_filter_ok = false;
	bool object_grid_load_failure_ok = false;
	if (material_preview_files_ok && material_model) {
		ModelEditorMetalWidget model_editor_material_preview(parent, material_model, team_color_override);
		model_editor_material_preview.resize(512, 512);
		model_editor_material_preview.set_smoke_capture_mode(true);
		std::vector<std::uint8_t> model_editor_material_pixels;
		int model_editor_material_width = 0;
		int model_editor_material_height = 0;
		const bool model_editor_material_ok = capture_metal_preview_widget(
			"model editor material preview",
			model_editor_material_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = model_editor_material_preview.smoke_capture_preview(pixels, width, height);
				model_editor_material_pixels = pixels;
				model_editor_material_width = width;
				model_editor_material_height = height;
				return captured;
			},
			smoke_clear_color(0.3f, 0.3f, 0.3f)
		);
		const bool model_editor_material_stats_ok = model_editor_material_ok
			&& validate_model_editor_material_preview_stats(model_editor_material_preview.smoke_preview_stats());
		const bool model_editor_material_pixels_ok = model_editor_material_stats_ok && validate_material_preview_pixels(
			"model editor material preview",
			model_editor_material_pixels,
			model_editor_material_width,
			model_editor_material_height
		);
		model_editor_material_artifact_ok = model_editor_material_pixels_ok && save_metal_smoke_artifact_png(
			artifact_directory,
			"model-editor-material-preview.png",
			model_editor_material_pixels,
			model_editor_material_width,
			model_editor_material_height
		);

		const std::vector<ModelEntry> material_entries {
			ModelEntry {
				.path = material_model_path,
				.category = ModelCategory::Abilities,
				.replaceable_id_override = team_color_override,
			},
		};
		ModelGridMetalWidget object_grid_material_preview(material_entries, parent);
		object_grid_material_preview.resize(512, 320);
		object_grid_material_preview.set_smoke_capture_mode(true);
		std::vector<std::uint8_t> object_grid_material_pixels;
		int object_grid_material_width = 0;
		int object_grid_material_height = 0;
		const bool object_grid_material_ok = capture_metal_preview_widget(
			"object-grid material preview",
			object_grid_material_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = object_grid_material_preview.smoke_capture_preview(pixels, width, height);
				object_grid_material_pixels = pixels;
				object_grid_material_width = width;
				object_grid_material_height = height;
				return captured;
			},
			smoke_clear_color(0.15f, 0.15f, 0.15f)
		);
		const bool object_grid_material_stats_ok = object_grid_material_ok
			&& validate_object_grid_material_preview_stats(object_grid_material_preview.smoke_preview_stats(), material_model_path);
		const bool object_grid_material_pixels_ok = object_grid_material_stats_ok && validate_material_preview_pixels(
			"object-grid material preview",
			object_grid_material_pixels,
			object_grid_material_width,
			object_grid_material_height
		);
		object_grid_material_artifact_ok = object_grid_material_pixels_ok && save_metal_smoke_artifact_png(
			artifact_directory,
			"object-grid-material-preview.png",
			object_grid_material_pixels,
			object_grid_material_width,
			object_grid_material_height
		);

		const std::vector<ModelEntry> sequence_less_material_entries {
			ModelEntry {
				.path = sequence_less_material_model_path,
				.category = ModelCategory::Abilities,
				.replaceable_id_override = team_color_override,
			},
		};
		ModelGridMetalWidget object_grid_sequence_less_material_preview(sequence_less_material_entries, parent);
		object_grid_sequence_less_material_preview.resize(512, 320);
		object_grid_sequence_less_material_preview.set_smoke_capture_mode(true);
		std::vector<std::uint8_t> object_grid_sequence_less_material_pixels;
		int object_grid_sequence_less_material_width = 0;
		int object_grid_sequence_less_material_height = 0;
		const bool object_grid_sequence_less_material_captured = capture_metal_preview_widget(
			"object-grid sequence-less material preview",
			object_grid_sequence_less_material_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = object_grid_sequence_less_material_preview.smoke_capture_preview(pixels, width, height);
				object_grid_sequence_less_material_pixels = pixels;
				object_grid_sequence_less_material_width = width;
				object_grid_sequence_less_material_height = height;
				return captured;
			},
			smoke_clear_color(0.15f, 0.15f, 0.15f)
		);
		const bool object_grid_sequence_less_material_stats_ok = object_grid_sequence_less_material_captured
			&& sequence_less_material_model
			&& validate_object_grid_sequence_less_material_preview_stats(
				object_grid_sequence_less_material_preview.smoke_preview_stats(),
				sequence_less_material_model_path,
				sequence_less_material_model->extent
			);
		object_grid_sequence_less_material_ok = object_grid_sequence_less_material_stats_ok && validate_material_preview_pixels(
			"object-grid sequence-less material preview",
			object_grid_sequence_less_material_pixels,
			object_grid_sequence_less_material_width,
			object_grid_sequence_less_material_height
		);

		ModelGridMetalWidget object_grid_empty_preview(material_entries, parent);
		object_grid_empty_preview.resize(512, 320);
		object_grid_empty_preview.set_smoke_capture_mode(true);
		object_grid_empty_preview.set_search(QStringLiteral("__hivewe_no_matching_model__"));
		std::vector<std::uint8_t> object_grid_empty_pixels;
		int object_grid_empty_width = 0;
		int object_grid_empty_height = 0;
		const bool object_grid_empty_ok = capture_metal_preview_widget(
			"object-grid empty-filter preview",
			object_grid_empty_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = object_grid_empty_preview.smoke_capture_preview(pixels, width, height);
				object_grid_empty_pixels = pixels;
				object_grid_empty_width = width;
				object_grid_empty_height = height;
				return captured;
			},
			smoke_clear_color(0.0f, 0.0f, 0.0f)
		);
		const bool object_grid_empty_stats_ok = object_grid_empty_ok
			&& validate_object_grid_empty_preview_stats(object_grid_empty_preview.smoke_preview_stats());
		object_grid_empty_filter_ok = object_grid_empty_stats_ok && save_metal_smoke_artifact_png(
			artifact_directory,
			"object-grid-empty-filter-preview.png",
			object_grid_empty_pixels,
			object_grid_empty_width,
			object_grid_empty_height
		);

		const fs::path missing_model_path = material_model_path.parent_path() / "missing-object-grid-preview.mdx";
		const std::vector<ModelEntry> missing_entries {
			ModelEntry {
				.path = missing_model_path,
				.category = ModelCategory::Abilities,
			},
		};
		ModelGridMetalWidget object_grid_load_failure_preview(missing_entries, parent);
		object_grid_load_failure_preview.resize(512, 320);
		object_grid_load_failure_preview.set_smoke_capture_mode(true);
		std::vector<std::uint8_t> object_grid_load_failure_pixels;
		int object_grid_load_failure_width = 0;
		int object_grid_load_failure_height = 0;
		const bool object_grid_load_failure_captured = capture_metal_preview_widget(
			"object-grid load-failure preview",
			object_grid_load_failure_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = object_grid_load_failure_preview.smoke_capture_preview(pixels, width, height);
				object_grid_load_failure_pixels = pixels;
				object_grid_load_failure_width = width;
				object_grid_load_failure_height = height;
				return captured;
			},
			smoke_clear_color(0.15f, 0.15f, 0.15f)
		);
		const bool object_grid_load_failure_stats_ok = object_grid_load_failure_captured
			&& validate_object_grid_load_failure_preview_stats(object_grid_load_failure_preview.smoke_preview_stats(), missing_model_path);
		const bool object_grid_load_failure_pixels_ok =
			object_grid_load_failure_stats_ok
			&& validate_object_grid_load_failure_pixels(
				object_grid_load_failure_pixels,
				object_grid_load_failure_width,
				object_grid_load_failure_height
			);
		object_grid_load_failure_ok = object_grid_load_failure_pixels_ok && save_metal_smoke_artifact_png(
			artifact_directory,
			"object-grid-load-failure-preview.png",
			object_grid_load_failure_pixels,
			object_grid_load_failure_width,
			object_grid_load_failure_height
		);
	}
	if (multicell_preview_files_ok && multicell_model) {
		const std::vector<ModelEntry> multicell_entries {
			ModelEntry {
				.path = multicell_model_path,
				.category = ModelCategory::Abilities,
				.replaceable_id_override = std::make_pair(31, multicell_green_team_color_path.string()),
			},
			ModelEntry {
				.path = multicell_model_path,
				.category = ModelCategory::Abilities,
				.replaceable_id_override = std::make_pair(31, multicell_blue_team_color_path.string()),
			},
		};
		ModelGridMetalWidget object_grid_multicell_preview(multicell_entries, parent);
		object_grid_multicell_preview.resize(512, 320);
		object_grid_multicell_preview.set_smoke_capture_mode(true);
		std::vector<std::uint8_t> object_grid_multicell_pixels;
		int object_grid_multicell_width = 0;
		int object_grid_multicell_height = 0;
		const bool object_grid_multicell_ok = capture_metal_preview_widget(
			"object-grid multicell material preview",
			object_grid_multicell_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = object_grid_multicell_preview.smoke_capture_preview(pixels, width, height);
				object_grid_multicell_pixels = pixels;
				object_grid_multicell_width = width;
				object_grid_multicell_height = height;
				return captured;
			},
			smoke_clear_color(0.15f, 0.15f, 0.15f)
		);
		const bool object_grid_multicell_stats_ok = object_grid_multicell_ok
			&& validate_object_grid_multicell_preview_stats(
				object_grid_multicell_preview.smoke_preview_stats(),
				multicell_model_path
			);
		const bool object_grid_multicell_pixels_ok = object_grid_multicell_stats_ok && validate_multicell_material_preview_pixels(
			object_grid_multicell_pixels,
			object_grid_multicell_width,
			object_grid_multicell_height,
			object_grid_multicell_preview.width(),
			object_grid_multicell_preview.cell_pixel_size(),
			40
		);
		object_grid_multicell_artifact_ok = object_grid_multicell_pixels_ok && save_metal_smoke_artifact_png(
			artifact_directory,
			"object-grid-multicell-material-preview.png",
			object_grid_multicell_pixels,
			object_grid_multicell_width,
			object_grid_multicell_height
		);

		const std::vector<ModelEntry> scrolled_entries {
			ModelEntry {
				.path = multicell_model_path,
				.category = ModelCategory::Abilities,
				.replaceable_id_override = std::make_pair(31, multicell_green_team_color_path.string()),
			},
			ModelEntry {
				.path = multicell_model_path,
				.category = ModelCategory::Buildings,
				.replaceable_id_override = std::make_pair(31, multicell_blue_team_color_path.string()),
			},
			ModelEntry {
				.path = multicell_model_path,
				.category = ModelCategory::Doodads,
				.replaceable_id_override = std::make_pair(31, multicell_green_team_color_path.string()),
			},
		};
		ModelGridMetalWidget object_grid_scrolled_preview(scrolled_entries, parent);
		object_grid_scrolled_preview.resize(512, 320);
		object_grid_scrolled_preview.set_smoke_capture_mode(true);
		std::vector<std::uint8_t> object_grid_scrolled_pixels;
		int object_grid_scrolled_width = 0;
		int object_grid_scrolled_height = 0;
		constexpr int scrolled_preview_offset = 84;
		const bool object_grid_scrolled_ok = capture_metal_preview_widget(
			"object-grid scrolled material preview",
			object_grid_scrolled_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = object_grid_scrolled_preview.smoke_capture_preview_at_scroll(
					scrolled_preview_offset,
					pixels,
					width,
					height
				);
				object_grid_scrolled_pixels = pixels;
				object_grid_scrolled_width = width;
				object_grid_scrolled_height = height;
				return captured;
			},
			smoke_clear_color(0.15f, 0.15f, 0.15f)
		);
		const bool object_grid_scrolled_stats_ok = object_grid_scrolled_ok
			&& validate_object_grid_scrolled_preview_stats(
				object_grid_scrolled_preview.smoke_preview_stats(),
				multicell_model_path
			);
		object_grid_scrolled_artifact_ok = object_grid_scrolled_stats_ok && save_metal_smoke_artifact_png(
			artifact_directory,
			"object-grid-scrolled-material-preview.png",
			object_grid_scrolled_pixels,
			object_grid_scrolled_width,
			object_grid_scrolled_height
		);
	}

	bool model_editor_particle_artifact_ok = false;
	bool object_grid_particle_artifact_ok = false;
	if (particle_preview_files_ok && particle_model) {
		ModelEditorMetalWidget model_editor_particle_preview(parent, particle_model);
		model_editor_particle_preview.resize(512, 512);
		model_editor_particle_preview.set_smoke_capture_mode(true);
		std::vector<std::uint8_t> model_editor_particle_pixels;
		int model_editor_particle_width = 0;
		int model_editor_particle_height = 0;
		const bool model_editor_particle_ok = capture_metal_preview_widget(
			"model editor particle preview",
			model_editor_particle_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = model_editor_particle_preview.smoke_capture_preview(pixels, width, height);
				model_editor_particle_pixels = pixels;
				model_editor_particle_width = width;
				model_editor_particle_height = height;
				return captured;
			},
			smoke_clear_color(0.3f, 0.3f, 0.3f)
		);
		const bool model_editor_particle_stats_ok = model_editor_particle_ok
			&& validate_model_editor_particle_preview_stats(model_editor_particle_preview.smoke_preview_stats());
		const bool model_editor_particle_pixels_ok = model_editor_particle_stats_ok && validate_particle_preview_pixels(
			"model editor particle preview",
			model_editor_particle_pixels,
			model_editor_particle_width,
			model_editor_particle_height,
			true,
			true,
			true,
			false
		);
		const bool model_editor_particle_artifact_saved = model_editor_particle_stats_ok && save_metal_smoke_artifact_png(
			artifact_directory,
			"model-editor-particle-preview.png",
			model_editor_particle_pixels,
			model_editor_particle_width,
			model_editor_particle_height
		);
		model_editor_particle_artifact_ok = model_editor_particle_pixels_ok && model_editor_particle_artifact_saved;

		const std::vector<ModelEntry> particle_entries {
			ModelEntry {
				.path = particle_model_path,
				.category = ModelCategory::Abilities,
			},
		};
		ModelGridMetalWidget object_grid_particle_preview(particle_entries, parent);
		object_grid_particle_preview.resize(512, 320);
		object_grid_particle_preview.set_smoke_capture_mode(true);
		std::vector<std::uint8_t> object_grid_particle_pixels;
		int object_grid_particle_width = 0;
		int object_grid_particle_height = 0;
		const bool object_grid_particle_ok = capture_metal_preview_widget(
			"object-grid particle preview",
			object_grid_particle_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = object_grid_particle_preview.smoke_capture_preview(pixels, width, height);
				object_grid_particle_pixels = pixels;
				object_grid_particle_width = width;
				object_grid_particle_height = height;
				return captured;
			},
			smoke_clear_color(0.15f, 0.15f, 0.15f)
		);
		const bool object_grid_particle_stats_ok = object_grid_particle_ok
			&& validate_object_grid_particle_preview_stats(object_grid_particle_preview.smoke_preview_stats(), particle_model_path);
		const bool object_grid_particle_pixels_ok = object_grid_particle_stats_ok && validate_particle_preview_pixels(
			"object-grid particle preview",
			object_grid_particle_pixels,
			object_grid_particle_width,
			object_grid_particle_height,
			false,
			false,
			true,
			true
		);
		const bool object_grid_particle_artifact_saved = object_grid_particle_stats_ok && save_metal_smoke_artifact_png(
			artifact_directory,
			"object-grid-particle-preview.png",
			object_grid_particle_pixels,
			object_grid_particle_width,
			object_grid_particle_height
		);
		object_grid_particle_artifact_ok = object_grid_particle_pixels_ok && object_grid_particle_artifact_saved;
	}

	bool model_editor_ribbon_artifact_ok = false;
	bool object_grid_ribbon_artifact_ok = false;
	if (ribbon_preview_files_ok && ribbon_model) {
		ModelEditorMetalWidget model_editor_ribbon_preview(parent, ribbon_model);
		model_editor_ribbon_preview.resize(512, 512);
		model_editor_ribbon_preview.set_smoke_capture_mode(true);
		std::vector<std::uint8_t> model_editor_ribbon_pixels;
		int model_editor_ribbon_width = 0;
		int model_editor_ribbon_height = 0;
		const bool model_editor_ribbon_ok = capture_metal_preview_widget(
			"model editor ribbon preview",
			model_editor_ribbon_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = model_editor_ribbon_preview.smoke_capture_preview(pixels, width, height);
				model_editor_ribbon_pixels = pixels;
				model_editor_ribbon_width = width;
				model_editor_ribbon_height = height;
				return captured;
			},
			smoke_clear_color(0.3f, 0.3f, 0.3f)
		);
		const bool model_editor_ribbon_stats_ok = model_editor_ribbon_ok
			&& validate_model_editor_ribbon_preview_stats(model_editor_ribbon_preview.smoke_preview_stats());
		const bool model_editor_ribbon_pixels_ok = model_editor_ribbon_stats_ok && validate_ribbon_preview_pixels(
			"model editor ribbon preview",
			model_editor_ribbon_pixels,
			model_editor_ribbon_width,
			model_editor_ribbon_height
		);
		model_editor_ribbon_artifact_ok = model_editor_ribbon_pixels_ok && save_metal_smoke_artifact_png(
			artifact_directory,
			"model-editor-ribbon-preview.png",
			model_editor_ribbon_pixels,
			model_editor_ribbon_width,
			model_editor_ribbon_height
		);

		const std::vector<ModelEntry> ribbon_entries {
			ModelEntry {
				.path = ribbon_model_path,
				.category = ModelCategory::Abilities,
			},
		};
		ModelGridMetalWidget object_grid_ribbon_preview(ribbon_entries, parent);
		object_grid_ribbon_preview.resize(512, 320);
		object_grid_ribbon_preview.set_smoke_capture_mode(true);
		std::vector<std::uint8_t> object_grid_ribbon_pixels;
		int object_grid_ribbon_width = 0;
		int object_grid_ribbon_height = 0;
		const bool object_grid_ribbon_ok = capture_metal_preview_widget(
			"object-grid ribbon preview",
			object_grid_ribbon_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = object_grid_ribbon_preview.smoke_capture_preview(pixels, width, height);
				object_grid_ribbon_pixels = pixels;
				object_grid_ribbon_width = width;
				object_grid_ribbon_height = height;
				return captured;
			},
			smoke_clear_color(0.15f, 0.15f, 0.15f)
		);
		const bool object_grid_ribbon_stats_ok = object_grid_ribbon_ok
			&& validate_object_grid_ribbon_preview_stats(object_grid_ribbon_preview.smoke_preview_stats(), ribbon_model_path);
		const bool object_grid_ribbon_pixels_ok = object_grid_ribbon_stats_ok && validate_ribbon_preview_pixels(
			"object-grid ribbon preview",
			object_grid_ribbon_pixels,
			object_grid_ribbon_width,
			object_grid_ribbon_height
		);
		object_grid_ribbon_artifact_ok = object_grid_ribbon_pixels_ok && save_metal_smoke_artifact_png(
			artifact_directory,
			"object-grid-ribbon-preview.png",
			object_grid_ribbon_pixels,
			object_grid_ribbon_width,
			object_grid_ribbon_height
		);
	}

	return model_editor_material_artifact_ok && object_grid_material_artifact_ok && object_grid_empty_filter_ok
		&& object_grid_sequence_less_material_ok && object_grid_load_failure_ok
		&& model_editor_particle_artifact_ok && object_grid_particle_artifact_ok && model_editor_ribbon_artifact_ok
		&& object_grid_ribbon_artifact_ok && object_grid_multicell_artifact_ok && object_grid_scrolled_artifact_ok;
}

bool run_shared_model_preview_probe_smoke_impl(QWidget* parent, const QString& artifact_directory) {
	return run_metal_preview_probe_smoke_impl(parent, artifact_directory);
}

bool run_model_preview_smoke_impl(QWidget* parent, const QString& artifact_directory) {
	auto model = load_model_preview_smoke_model();
	if (!model) {
		return false;
	}

	ModelEditorMetalWidget model_editor_preview(parent, model);
	model_editor_preview.resize(512, 512);
	model_editor_preview.set_smoke_capture_mode(true);
	std::vector<std::uint8_t> model_editor_pixels;
	int model_editor_width = 0;
	int model_editor_height = 0;
	const bool model_editor_ok = capture_metal_preview_widget(
		"model editor preview",
		model_editor_preview,
		[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
			const bool captured = model_editor_preview.smoke_capture_preview(pixels, width, height);
			model_editor_pixels = pixels;
			model_editor_width = width;
			model_editor_height = height;
			return captured;
		},
		smoke_clear_color(0.3f, 0.3f, 0.3f)
	);
	const bool model_editor_stats_ok = model_editor_ok && validate_model_editor_preview_stats(model_editor_preview.smoke_preview_stats());
	const bool model_editor_pixels_ok = model_editor_stats_ok && validate_real_model_preview_pixel_diversity(
		"Metal",
		"model editor preview",
		model_editor_pixels,
		model_editor_width,
		model_editor_height,
		77u,
		77u,
		77u,
		2u,
		1u,
		0u
	);
	const bool model_editor_artifact_ok = model_editor_pixels_ok && save_metal_smoke_artifact_png(
		artifact_directory,
		"model-editor-preview.png",
		model_editor_pixels,
		model_editor_width,
		model_editor_height
	);

	const std::vector<ModelEntry> entries {
		ModelEntry {
			.path = fs::path(std::string(k_model_preview_smoke_path)),
			.category = ModelCategory::Units,
		},
	};
	ModelGridMetalWidget object_grid_preview(entries, parent);
	object_grid_preview.resize(512, 320);
	object_grid_preview.set_smoke_capture_mode(true);
	std::vector<std::uint8_t> object_grid_pixels;
	int object_grid_width = 0;
	int object_grid_height = 0;
	const bool object_grid_ok = capture_metal_preview_widget(
		"object-grid preview",
		object_grid_preview,
		[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
			const bool captured = object_grid_preview.smoke_capture_preview(pixels, width, height);
			object_grid_pixels = pixels;
			object_grid_width = width;
			object_grid_height = height;
			return captured;
		},
		smoke_clear_color(0.15f, 0.15f, 0.15f)
	);
	const bool object_grid_stats_ok = object_grid_ok && validate_object_grid_preview_stats(object_grid_preview.smoke_preview_stats());
	const bool object_grid_pixels_ok = object_grid_stats_ok && validate_real_model_preview_pixel_diversity(
		"Metal",
		"object-grid preview",
		object_grid_pixels,
		object_grid_width,
		object_grid_height,
		38u,
		38u,
		38u,
		2u,
		1u,
		0u
	);
	const bool object_grid_artifact_ok = object_grid_pixels_ok && save_metal_smoke_artifact_png(
		artifact_directory,
		"object-grid-preview.png",
		object_grid_pixels,
		object_grid_width,
		object_grid_height
	);

	const bool preview_probes_ok = run_metal_preview_probe_smoke_impl(parent, artifact_directory);
	const bool model_material_ok = run_metal_model_material_smoke(parent, artifact_directory);
	const bool model_depth_isolation_ok = run_metal_model_depth_isolation_smoke(parent, artifact_directory);
	const bool selection_overlay_ok = run_metal_selection_overlay_smoke(parent, artifact_directory);
	const bool particle_artifact_ok = run_metal_particle_renderer_smoke(parent, artifact_directory);
	return model_editor_artifact_ok && object_grid_artifact_ok && preview_probes_ok && model_material_ok && selection_overlay_ok
		&& model_depth_isolation_ok && particle_artifact_ok;
}
#else
constexpr std::string_view k_model_preview_smoke_path = "units/human/footman/footman.mdx";
constexpr std::string_view k_material_preview_smoke_name = "smoke/material-preview.mdx";
constexpr std::string_view k_particle_preview_smoke_name = "smoke/particle-only.mdx";

struct OpenGLSmokePreviewClearColor {
	std::uint8_t r = 0;
	std::uint8_t g = 0;
	std::uint8_t b = 0;
};

std::uint8_t opengl_smoke_clear_channel(const float value) {
	return static_cast<std::uint8_t>(std::lround(std::clamp(value, 0.0f, 1.0f) * 255.0f));
}

OpenGLSmokePreviewClearColor opengl_smoke_clear_color(const float r, const float g, const float b) {
	return OpenGLSmokePreviewClearColor {
		.r = opengl_smoke_clear_channel(r),
		.g = opengl_smoke_clear_channel(g),
		.b = opengl_smoke_clear_channel(b),
	};
}

bool validate_opengl_smoke_preview_pixels(
	const std::string_view label,
	const std::vector<std::uint8_t>& pixels,
	const int width,
	const int height,
	const OpenGLSmokePreviewClearColor clear_color
) {
	if (width <= 0 || height <= 0 || pixels.empty()) {
		std::println("OpenGL {} smoke failed: missing captured pixels.", label);
		return false;
	}

	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (pixels.size() < pixel_count * 4) {
		std::println(
			"OpenGL {} smoke failed: captured {} bytes for {}x{} RGBA frame.",
			label,
			pixels.size(),
			width,
			height
		);
		return false;
	}

	std::size_t changed_pixels = 0;
	int max_delta = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4;
		const int delta =
			std::abs(static_cast<int>(pixels[offset + 0]) - static_cast<int>(clear_color.r))
			+ std::abs(static_cast<int>(pixels[offset + 1]) - static_cast<int>(clear_color.g))
			+ std::abs(static_cast<int>(pixels[offset + 2]) - static_cast<int>(clear_color.b));
		if (delta > 18) {
			++changed_pixels;
		}
		max_delta = std::max(max_delta, delta);
	}

	const std::size_t min_changed_pixels = std::max<std::size_t>(64, pixel_count / 5000);
	if (changed_pixels < min_changed_pixels || max_delta < 30) {
		std::println(
			"OpenGL {} smoke failed: frame stayed too close to clear color ({} changed pixels, max delta {}, expected at least {}).",
			label,
			changed_pixels,
			max_delta,
			min_changed_pixels
		);
		return false;
	}

	std::println(
		"OpenGL {} smoke captured {}x{} with {} non-background pixels.",
		label,
		width,
		height,
		changed_pixels
	);
	return true;
}

std::shared_ptr<mdx::MDX> load_model_preview_smoke_model() {
	auto model_file = hierarchy.open_file(fs::path(std::string(k_model_preview_smoke_path)));
	if (!model_file) {
		std::println(
			"OpenGL model preview smoke failed: unable to load {}: {}",
			k_model_preview_smoke_path,
			model_file.error()
		);
		return nullptr;
	}

	try {
		BinaryReader reader = std::move(model_file.value());
		auto model = std::make_shared<mdx::MDX>(reader);
		model->name = std::string(k_model_preview_smoke_path);
		if (model->geosets.empty()) {
			std::println("OpenGL model preview smoke failed: {} has no geosets.", k_model_preview_smoke_path);
			return nullptr;
		}
		return model;
	} catch (const std::exception& error) {
		std::println(
			"OpenGL model preview smoke failed while parsing {}: {}",
			k_model_preview_smoke_path,
			error.what()
		);
		return nullptr;
	}
}

mdx::Extent make_opengl_material_preview_extent() {
	mdx::Extent extent;
	extent.bounds_radius = 160.0f;
	extent.minimum = glm::vec3(-96.0f, -96.0f, -32.0f);
	extent.maximum = glm::vec3(96.0f, 96.0f, 128.0f);
	return extent;
}

mdx::Layer make_opengl_material_preview_layer(
	const bool hd,
	const std::uint32_t blend_mode,
	const std::vector<std::uint32_t>& texture_ids
) {
	mdx::Layer layer {};
	layer.blend_mode = blend_mode;
	layer.alpha = 1.0f;
	layer.shader = hd ? mdx::ShaderType::HD : mdx::ShaderType::SD;
	layer.shading_flags = mdx::Layer::ShadingFlags::unshaded | mdx::Layer::ShadingFlags::two_sided;
	for (const std::uint32_t texture_id : texture_ids) {
		mdx::LayerTexture texture {};
		texture.id = texture_id;
		layer.textures.push_back(texture);
	}
	return layer;
}

mdx::Geoset make_opengl_material_preview_geoset(
	const std::uint32_t material_id,
	const glm::vec3& offset,
	const mdx::Extent& extent
) {
	mdx::Geoset geoset {};
	geoset.vertices = {
		offset + glm::vec3(-34.0f, -28.0f, 0.0f),
		offset + glm::vec3(34.0f, -28.0f, 0.0f),
		offset + glm::vec3(-34.0f, 28.0f, 0.0f),
		offset + glm::vec3(34.0f, 28.0f, 0.0f),
	};
	geoset.normals.assign(geoset.vertices.size(), glm::vec3(0.0f, 0.0f, 1.0f));
	geoset.face_type_groups = {4u};
	geoset.face_groups = {6u};
	geoset.faces = {0u, 1u, 2u, 1u, 3u, 2u};
	geoset.material_id = material_id;
	geoset.selection_group = 0;
	geoset.selection_flags = 0;
	geoset.lod = 0;
	geoset.extent = extent;
	geoset.sequence_extents = {extent};
	geoset.tangents.assign(geoset.vertices.size(), glm::vec4(1.0f, 0.0f, 0.0f, 1.0f));
	geoset.skin.reserve(geoset.vertices.size() * 8u);
	for (std::size_t i = 0; i < geoset.vertices.size(); ++i) {
		geoset.skin.insert(geoset.skin.end(), {0u, 0u, 0u, 0u, 255u, 0u, 0u, 0u});
	}
	geoset.uv_sets.push_back({
		glm::vec2(0.0f, 1.0f),
		glm::vec2(1.0f, 1.0f),
		glm::vec2(0.0f, 0.0f),
		glm::vec2(1.0f, 0.0f),
	});
	return geoset;
}

std::shared_ptr<mdx::MDX> make_opengl_material_preview_smoke_model(const std::vector<fs::path>& texture_paths) {
	auto model = std::make_shared<mdx::MDX>();
	model->version = mdx::MDX::LATEST_MDX_VERSION;
	model->name = std::string(k_material_preview_smoke_name);
	model->blend_time = 150;
	model->extent = make_opengl_material_preview_extent();

	mdx::Sequence sequence {};
	sequence.name = "Stand";
	sequence.start_frame = 0;
	sequence.end_frame = 1000;
	sequence.flags = mdx::Sequence::Flags::looping;
	sequence.extent = model->extent;
	model->sequences.push_back(sequence);

	decltype(model->bones)::value_type bone {};
	bone.node.name = "SmokeMaterialBone";
	bone.node.id = 0;
	bone.node.parent_id = -1;
	bone.node.flags = mdx::Node::bone;
	bone.geoset_id = -1;
	bone.geoset_animation_id = -1;
	model->bones.push_back(bone);
	model->pivots.push_back(glm::vec3(0.0f));

	mdx::Texture team_color_texture {};
	team_color_texture.replaceable_id = 1;
	team_color_texture.flags = 0;
	model->textures.push_back(team_color_texture);
	for (const fs::path& texture_path : texture_paths) {
		mdx::Texture texture {};
		texture.replaceable_id = 0;
		texture.file_name = texture_path;
		texture.flags = 0;
		model->textures.push_back(texture);
	}

	mdx::Material opaque_material {};
	opaque_material.layers = {
		make_opengl_material_preview_layer(false, 1u, {0u}),
		make_opengl_material_preview_layer(true, 0u, {1u, 2u, 3u, 4u, 5u, 6u}),
	};
	model->materials.push_back(opaque_material);

	mdx::Material transparent_material {};
	transparent_material.layers = {
		make_opengl_material_preview_layer(false, 2u, {7u}),
		make_opengl_material_preview_layer(true, 5u, {8u, 9u, 10u, 11u, 12u, 13u}),
	};
	model->materials.push_back(transparent_material);

	model->geosets.push_back(make_opengl_material_preview_geoset(0u, glm::vec3(-40.0f, 0.0f, 24.0f), model->extent));
	model->geosets.push_back(make_opengl_material_preview_geoset(1u, glm::vec3(40.0f, 0.0f, 24.0f), model->extent));
	return model;
}

std::shared_ptr<mdx::MDX> make_opengl_particle_preview_smoke_model(const fs::path& texture_path) {
	auto model = std::make_shared<mdx::MDX>();
	model->version = mdx::MDX::LATEST_MDX_VERSION;
	model->name = std::string(k_particle_preview_smoke_name);
	model->blend_time = 150;
	model->extent = make_opengl_material_preview_extent();

	mdx::Sequence sequence {};
	sequence.name = "Stand";
	sequence.start_frame = 0;
	sequence.end_frame = 1000;
	sequence.flags = mdx::Sequence::Flags::looping;
	sequence.extent = model->extent;
	model->sequences.push_back(sequence);

	mdx::Texture texture {};
	texture.replaceable_id = 0;
	texture.file_name = texture_path;
	texture.flags = 0;
	model->textures.push_back(texture);

	model->pivots.push_back(glm::vec3(0.0f));

	mdx::ParticleEmitter2 emitter {};
	emitter.node.name = "SmokeParticles";
	emitter.node.id = 0;
	emitter.node.parent_id = -1;
	emitter.node.flags = mdx::Node::emitter | mdx::Node::unshaded;
	emitter.speed = 0.0f;
	emitter.variation = 0.0f;
	emitter.latitude = 0.0f;
	emitter.gravity = 0.0f;
	emitter.life_span = 2.0f;
	emitter.emission_rate = 240.0f;
	emitter.length = 32.0f;
	emitter.width = 32.0f;
	emitter.filter_mode = 0;
	emitter.rows = 1;
	emitter.columns = 1;
	emitter.head_or_tail = 0;
	emitter.tail_length = 0.0f;
	emitter.time_middle = 0.5f;
	emitter.start_segment_color = glm::vec3(1.0f, 0.25f, 0.10f);
	emitter.middle_segment_color = glm::vec3(1.0f, 0.65f, 0.08f);
	emitter.end_segment_color = glm::vec3(0.90f, 0.05f, 0.15f);
	emitter.segment_alphas = glm::u8vec3(255u, 255u, 220u);
	emitter.segment_scaling = glm::vec3(36.0f, 48.0f, 24.0f);
	emitter.head_intervals = glm::uvec3(0u, 0u, 1u);
	emitter.head_decay_intervals = glm::uvec3(0u, 0u, 1u);
	emitter.tail_intervals = glm::uvec3(0u, 0u, 1u);
	emitter.tail_decay_intervals = glm::uvec3(0u, 0u, 1u);
	emitter.texture_id = 0;
	emitter.squirt = 0;
	emitter.priority_plane = 0;
	emitter.replaceable_id = 0;
	model->emitters2.push_back(emitter);

	return model;
}

bool write_opengl_smoke_tga_texture(
	const fs::path& texture_path,
	const std::uint8_t r,
	const std::uint8_t g,
	const std::uint8_t b,
	const std::uint8_t a
) {
	const std::array<std::uint8_t, 22> tga {
		0u, 0u, 2u, 0u, 0u, 0u, 0u, 0u, 0u, 0u, 0u,
		0u, 1u, 0u, 1u, 0u, 32u, 8u, b, g, r, a,
	};

	std::ofstream output(texture_path, std::ios::binary);
	if (!output) {
		std::println("OpenGL material preview smoke failed: unable to write texture {}.", texture_path.string());
		return false;
	}
	output.write(reinterpret_cast<const char*>(tga.data()), static_cast<std::streamsize>(tga.size()));
	if (!output) {
		std::println("OpenGL material preview smoke failed while writing texture {}.", texture_path.string());
		return false;
	}
	return true;
}

bool write_opengl_material_preview_textures(const std::vector<fs::path>& texture_paths) {
	static constexpr std::array<std::array<std::uint8_t, 4>, 13> colors {{
		{40u, 220u, 80u, 255u},
		{120u, 120u, 255u, 255u},
		{180u, 180u, 180u, 255u},
		{255u, 180u, 60u, 255u},
		{255u, 80u, 80u, 255u},
		{80u, 255u, 220u, 255u},
		{40u, 80u, 255u, 192u},
		{255u, 40u, 220u, 192u},
		{120u, 80u, 255u, 192u},
		{180u, 180u, 180u, 192u},
		{255u, 140u, 60u, 192u},
		{255u, 80u, 120u, 192u},
		{80u, 220u, 255u, 192u},
	}};
	if (texture_paths.size() != colors.size()) {
		std::println("OpenGL material preview smoke failed: expected {} textures, got {}.", colors.size(), texture_paths.size());
		return false;
	}

	for (std::size_t i = 0; i < texture_paths.size(); ++i) {
		const auto& color = colors[i];
		if (!write_opengl_smoke_tga_texture(texture_paths[i], color[0], color[1], color[2], color[3])) {
			return false;
		}
	}
	return true;
}

bool write_opengl_particle_preview_texture(const fs::path& texture_path) {
	return write_opengl_smoke_tga_texture(texture_path, 255u, 255u, 255u, 255u);
}

bool write_opengl_preview_model(
	const std::shared_ptr<mdx::MDX>& model,
	const fs::path& model_path,
	const std::string_view label
) {
	if (!model) {
		return false;
	}

	const BinaryWriter writer = model->to_mdx();
	std::ofstream output(model_path, std::ios::binary);
	if (!output) {
		std::println("OpenGL {} smoke failed: unable to write model {}.", label, model_path.string());
		return false;
	}
	output.write(reinterpret_cast<const char*>(writer.buffer.data()), static_cast<std::streamsize>(writer.buffer.size()));
	if (!output) {
		std::println("OpenGL {} smoke failed while writing model {}.", label, model_path.string());
		return false;
	}
	return true;
}

bool save_opengl_smoke_artifact_png(
	const QString& artifact_directory,
	const std::string_view file_name,
	const std::vector<std::uint8_t>& rgba_pixels,
	const int width,
	const int height
) {
	if (artifact_directory.isEmpty()) {
		return true;
	}
	if (!ensure_smoke_artifact_directory("OpenGL", artifact_directory)) {
		return false;
	}

	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || rgba_pixels.size() < pixel_count * 4) {
		std::println("OpenGL smoke failed: missing pixels for artifact {}.", file_name);
		return false;
	}

	const QString output_path = QDir(artifact_directory).filePath(
		QString::fromUtf8(file_name.data(), static_cast<qsizetype>(file_name.size()))
	);
	const QImage image(
		rgba_pixels.data(),
		width,
		height,
		width * 4,
		QImage::Format_RGBA8888
	);
	if (image.isNull() || !image.copy().save(output_path, "PNG")) {
		std::println("OpenGL smoke failed: unable to save artifact {}.", output_path.toStdString());
		return false;
	}

	qInfo() << "Captured OpenGL smoke artifact:" << output_path;
	return true;
}

bool is_opengl_smoke_green_material_pixel(const std::uint8_t r, const std::uint8_t g, const std::uint8_t b) {
	return g > 150u && r < 140u && b < 160u;
}

bool is_opengl_smoke_red_orange_particle_pixel(const std::uint8_t r, const std::uint8_t g, const std::uint8_t b) {
	return r > 140u && g > 20u && g < 170u && b < 120u;
}

bool is_opengl_smoke_yellow_particle_pixel(const std::uint8_t r, const std::uint8_t g, const std::uint8_t b) {
	return r > 170u && g > 120u && b < 120u;
}

bool validate_opengl_material_preview_pixels(
	const std::string_view label,
	const std::vector<std::uint8_t>& pixels,
	const int width,
	const int height
) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("OpenGL {} smoke failed: missing captured pixels.", label);
		return false;
	}

	MaterialPreviewPixelCounts counts;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4;
		count_material_preview_pixel(counts, pixels[offset], pixels[offset + 1], pixels[offset + 2]);
	}

	return validate_material_preview_counts("OpenGL", label, counts, width, height);
}

bool validate_opengl_particle_preview_pixels(
	const std::string_view label,
	const std::vector<std::uint8_t>& pixels,
	const int width,
	const int height
) {
	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("OpenGL {} smoke failed: missing captured pixels.", label);
		return false;
	}

	std::size_t warm_particle_pixels = 0;
	for (std::size_t i = 0; i < pixel_count; ++i) {
		const std::size_t offset = i * 4;
		const std::uint8_t r = pixels[offset];
		const std::uint8_t g = pixels[offset + 1];
		const std::uint8_t b = pixels[offset + 2];
		if (is_opengl_smoke_red_orange_particle_pixel(r, g, b) || is_opengl_smoke_yellow_particle_pixel(r, g, b)) {
			++warm_particle_pixels;
		}
	}

	const std::size_t min_warm_pixels = std::max<std::size_t>(64, pixel_count / 5000);
	if (warm_particle_pixels < min_warm_pixels) {
		std::println(
			"OpenGL {} smoke failed: saw {} warm particle pixels, expected at least {}.",
			label,
			warm_particle_pixels,
			min_warm_pixels
		);
		return false;
	}

	std::println("OpenGL {} smoke captured {} warm particle pixels.", label, warm_particle_pixels);
	return true;
}

template <typename Widget, typename CaptureFn>
bool capture_opengl_preview_widget(
	const std::string_view label,
	Widget& widget,
	CaptureFn capture,
	const OpenGLSmokePreviewClearColor clear_color
) {
	bool ok = false;
	bool completed = false;

	widget.show();
	widget.raise();
	widget.update();

	QEventLoop loop;
	QTimer::singleShot(250, &loop, [&] {
		QCoreApplication::processEvents(QEventLoop::AllEvents, 100);

		std::vector<std::uint8_t> pixels;
		int width = 0;
		int height = 0;
		const bool captured = capture(pixels, width, height);
		ok = captured && validate_opengl_smoke_preview_pixels(label, pixels, width, height, clear_color);
		completed = true;
		loop.quit();
	});
	QTimer::singleShot(5000, &loop, [&] {
		if (!completed) {
			std::println("OpenGL {} smoke failed: timed out waiting for a preview frame.", label);
			ok = false;
			loop.quit();
		}
	});
	loop.exec();

	widget.close();
	return ok;
}

bool run_model_preview_smoke_impl(
	QWidget* parent,
	const QString& artifact_directory,
	const bool include_warcraft_previews
) {
	bool model_editor_artifact_ok = true;
	bool object_grid_artifact_ok = true;
	if (include_warcraft_previews) {
		auto model = load_model_preview_smoke_model();
		if (!model) {
			return false;
		}

		ModelEditorGLWidget model_editor_preview(parent, model);
		model_editor_preview.resize(512, 512);
		std::vector<std::uint8_t> model_editor_pixels;
		int model_editor_width = 0;
		int model_editor_height = 0;
		const bool model_editor_ok = capture_opengl_preview_widget(
			"model editor preview",
			model_editor_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = model_editor_preview.smoke_capture_preview(pixels, width, height);
				model_editor_pixels = pixels;
				model_editor_width = width;
				model_editor_height = height;
				return captured;
			},
			opengl_smoke_clear_color(0.3f, 0.3f, 0.3f)
		);
		const bool model_editor_pixels_ok = model_editor_ok && validate_real_model_preview_pixel_diversity(
			"OpenGL",
			"model editor preview",
			model_editor_pixels,
			model_editor_width,
			model_editor_height,
			77u,
			77u,
			77u,
			0u,
			1u,
			2u
		);
		model_editor_artifact_ok = model_editor_pixels_ok && save_opengl_smoke_artifact_png(
			artifact_directory,
			"model-editor-preview.png",
			model_editor_pixels,
			model_editor_width,
			model_editor_height
		);

		const std::vector<ModelEntry> entries {
			ModelEntry {
				.path = fs::path(std::string(k_model_preview_smoke_path)),
				.category = ModelCategory::Units,
			},
		};
		ModelGridGLWidget object_grid_preview(entries, parent);
		object_grid_preview.resize(512, 320);
		std::vector<std::uint8_t> object_grid_pixels;
		int object_grid_width = 0;
		int object_grid_height = 0;
		const bool object_grid_ok = capture_opengl_preview_widget(
			"object-grid preview",
			object_grid_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = object_grid_preview.smoke_capture_preview(pixels, width, height);
				object_grid_pixels = pixels;
				object_grid_width = width;
				object_grid_height = height;
				return captured;
			},
			opengl_smoke_clear_color(0.15f, 0.15f, 0.15f)
		);
		const bool object_grid_pixels_ok = object_grid_ok && validate_real_model_preview_pixel_diversity(
			"OpenGL",
			"object-grid preview",
			object_grid_pixels,
			object_grid_width,
			object_grid_height,
			38u,
			38u,
			38u,
			0u,
			1u,
			2u
		);
		object_grid_artifact_ok = object_grid_pixels_ok && save_opengl_smoke_artifact_png(
			artifact_directory,
			"object-grid-preview.png",
			object_grid_pixels,
			object_grid_width,
			object_grid_height
		);
	}

	QTemporaryDir synthetic_preview_dir(QDir::tempPath() + QStringLiteral("/HiveWE-opengl-synthetic-preview-XXXXXX"));
	bool material_preview_files_ok = false;
	bool particle_preview_files_ok = false;
	std::shared_ptr<mdx::MDX> material_model;
	std::shared_ptr<mdx::MDX> particle_model;
	fs::path material_model_path;
	fs::path particle_model_path;
	std::optional<std::pair<int, std::string>> team_color_override;
	if (!synthetic_preview_dir.isValid()) {
		std::println("OpenGL synthetic preview smoke failed: unable to create a temporary model directory.");
	} else {
		const fs::path synthetic_preview_path = fs::path(synthetic_preview_dir.path().toStdString());

		std::vector<fs::path> material_texture_paths;
		material_texture_paths.reserve(13);
		for (std::size_t i = 0; i < 13; ++i) {
			material_texture_paths.push_back(synthetic_preview_path / ("material-" + std::to_string(i) + ".tga"));
		}
		material_model_path = synthetic_preview_path / "material-preview.mdx";
		const fs::path team_color_texture_path = synthetic_preview_path / "team-color.tga";
		team_color_override = std::make_pair(1, team_color_texture_path.string());
		material_model = make_opengl_material_preview_smoke_model(material_texture_paths);
		material_preview_files_ok = write_opengl_smoke_tga_texture(team_color_texture_path, 255u, 32u, 32u, 255u)
			&& write_opengl_material_preview_textures(material_texture_paths)
			&& write_opengl_preview_model(material_model, material_model_path, "material preview");

		const fs::path particle_texture_path = synthetic_preview_path / "particle-smoke.tga";
		particle_model_path = synthetic_preview_path / "particle-only.mdx";
		particle_model = make_opengl_particle_preview_smoke_model(particle_texture_path);
		particle_preview_files_ok = write_opengl_particle_preview_texture(particle_texture_path)
			&& write_opengl_preview_model(particle_model, particle_model_path, "particle preview");
	}

	bool model_editor_material_artifact_ok = false;
	bool object_grid_material_artifact_ok = false;
	if (material_preview_files_ok && material_model) {
		ModelEditorGLWidget model_editor_material_preview(parent, material_model, team_color_override);
		model_editor_material_preview.resize(512, 512);
		std::vector<std::uint8_t> model_editor_material_pixels;
		int model_editor_material_width = 0;
		int model_editor_material_height = 0;
		const bool model_editor_material_ok = capture_opengl_preview_widget(
			"model editor material preview",
			model_editor_material_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = model_editor_material_preview.smoke_capture_preview(pixels, width, height);
				model_editor_material_pixels = pixels;
				model_editor_material_width = width;
				model_editor_material_height = height;
				return captured;
			},
			opengl_smoke_clear_color(0.3f, 0.3f, 0.3f)
		);
		const bool model_editor_material_pixels_ok = model_editor_material_ok && validate_opengl_material_preview_pixels(
			"model editor material preview",
			model_editor_material_pixels,
			model_editor_material_width,
			model_editor_material_height
		);
		model_editor_material_artifact_ok = model_editor_material_pixels_ok && save_opengl_smoke_artifact_png(
			artifact_directory,
			"model-editor-material-preview.png",
			model_editor_material_pixels,
			model_editor_material_width,
			model_editor_material_height
		);

		const std::vector<ModelEntry> material_entries {
			ModelEntry {
				.path = material_model_path,
				.category = ModelCategory::Abilities,
				.replaceable_id_override = team_color_override,
			},
		};
		ModelGridGLWidget object_grid_material_preview(material_entries, parent);
		object_grid_material_preview.resize(512, 320);
		std::vector<std::uint8_t> object_grid_material_pixels;
		int object_grid_material_width = 0;
		int object_grid_material_height = 0;
		const bool object_grid_material_ok = capture_opengl_preview_widget(
			"object-grid material preview",
			object_grid_material_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = object_grid_material_preview.smoke_capture_preview(pixels, width, height);
				object_grid_material_pixels = pixels;
				object_grid_material_width = width;
				object_grid_material_height = height;
				return captured;
			},
			opengl_smoke_clear_color(0.15f, 0.15f, 0.15f)
		);
		const bool object_grid_material_pixels_ok = object_grid_material_ok && validate_opengl_material_preview_pixels(
			"object-grid material preview",
			object_grid_material_pixels,
			object_grid_material_width,
			object_grid_material_height
		);
		object_grid_material_artifact_ok = object_grid_material_pixels_ok && save_opengl_smoke_artifact_png(
			artifact_directory,
			"object-grid-material-preview.png",
			object_grid_material_pixels,
			object_grid_material_width,
			object_grid_material_height
		);
	}

	bool model_editor_particle_artifact_ok = false;
	bool object_grid_particle_artifact_ok = false;
	if (particle_preview_files_ok && particle_model) {
		ModelEditorGLWidget model_editor_particle_preview(parent, particle_model);
		model_editor_particle_preview.resize(512, 512);
		std::vector<std::uint8_t> model_editor_particle_pixels;
		int model_editor_particle_width = 0;
		int model_editor_particle_height = 0;
		const bool model_editor_particle_ok = capture_opengl_preview_widget(
			"model editor particle preview",
			model_editor_particle_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = model_editor_particle_preview.smoke_capture_preview(pixels, width, height);
				model_editor_particle_pixels = pixels;
				model_editor_particle_width = width;
				model_editor_particle_height = height;
				return captured;
			},
			opengl_smoke_clear_color(0.3f, 0.3f, 0.3f)
		);
		const bool model_editor_particle_pixels_ok = model_editor_particle_ok && validate_opengl_particle_preview_pixels(
			"model editor particle preview",
			model_editor_particle_pixels,
			model_editor_particle_width,
			model_editor_particle_height
		);
		model_editor_particle_artifact_ok = model_editor_particle_pixels_ok && save_opengl_smoke_artifact_png(
			artifact_directory,
			"model-editor-particle-preview.png",
			model_editor_particle_pixels,
			model_editor_particle_width,
			model_editor_particle_height
		);

		const std::vector<ModelEntry> particle_entries {
			ModelEntry {
				.path = particle_model_path,
				.category = ModelCategory::Abilities,
			},
		};
		ModelGridGLWidget object_grid_particle_preview(particle_entries, parent);
		object_grid_particle_preview.resize(512, 320);
		std::vector<std::uint8_t> object_grid_particle_pixels;
		int object_grid_particle_width = 0;
		int object_grid_particle_height = 0;
		const bool object_grid_particle_ok = capture_opengl_preview_widget(
			"object-grid particle preview",
			object_grid_particle_preview,
			[&](std::vector<std::uint8_t>& pixels, int& width, int& height) {
				const bool captured = object_grid_particle_preview.smoke_capture_preview(pixels, width, height);
				object_grid_particle_pixels = pixels;
				object_grid_particle_width = width;
				object_grid_particle_height = height;
				return captured;
			},
			opengl_smoke_clear_color(0.15f, 0.15f, 0.15f)
		);
		const bool object_grid_particle_pixels_ok = object_grid_particle_ok && validate_opengl_particle_preview_pixels(
			"object-grid particle preview",
			object_grid_particle_pixels,
			object_grid_particle_width,
			object_grid_particle_height
		);
		object_grid_particle_artifact_ok = object_grid_particle_pixels_ok && save_opengl_smoke_artifact_png(
			artifact_directory,
			"object-grid-particle-preview.png",
			object_grid_particle_pixels,
			object_grid_particle_width,
			object_grid_particle_height
		);
	}

	return model_editor_artifact_ok && object_grid_artifact_ok && model_editor_material_artifact_ok
		&& object_grid_material_artifact_ok && model_editor_particle_artifact_ok && object_grid_particle_artifact_ok;
}

bool run_shared_model_preview_probe_smoke_impl(QWidget* parent, const QString& artifact_directory) {
	return run_model_preview_smoke_impl(parent, artifact_directory, false);
}

bool run_model_preview_smoke_impl(QWidget* parent, const QString& artifact_directory) {
	return run_model_preview_smoke_impl(parent, artifact_directory, true);
}
#endif

} // namespace

#if HIVEWE_USE_METAL_RUNTIME
bool run_metal_terrain_material_smoke(QWidget* parent, const QString& artifact_directory) {
	return run_metal_terrain_material_smoke_impl(parent, artifact_directory);
}

bool run_metal_renderer_probe_smoke(QWidget* parent, const QString& artifact_directory) {
	return run_metal_renderer_probe_smoke_impl(parent, artifact_directory);
}

bool run_metal_preview_probe_smoke(QWidget* parent, const QString& artifact_directory) {
	return run_metal_preview_probe_smoke_impl(parent, artifact_directory);
}
#endif

bool run_model_preview_smoke(QWidget* parent, const QString& artifact_directory) {
	return run_model_preview_smoke_impl(parent, artifact_directory);
}

bool run_shared_model_preview_probe_smoke(QWidget* parent, const QString& artifact_directory) {
	return run_shared_model_preview_probe_smoke_impl(parent, artifact_directory);
}
