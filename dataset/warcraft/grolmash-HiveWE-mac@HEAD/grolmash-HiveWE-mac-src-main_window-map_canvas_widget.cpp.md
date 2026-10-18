---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/main_window/map_canvas_widget.cpp"
ref: HEAD
---

#include "map_canvas_widget.h"

#include <QDir>
#include <QKeyEvent>
#include <QImage>
#include <QLabel>
#include <QFont>
#include <QMouseEvent>
#if !HIVEWE_USE_METAL_MAP_CANVAS
#include <QOpenGLContext>
#include <QSurfaceFormat>
#endif
#include <QPainter>
#include <QPaintEvent>
#include <QResizeEvent>
#include <QShowEvent>
#include <QTimer>
#include <QVBoxLayout>
#include <QWheelEvent>

#include <tracy/Tracy.hpp>

#include "brush.h"
#include <algorithm>
#include <array>
#include <cmath>
#include <cstdlib>
#include <cstring>
#include <limits>
#include <numeric>
#include <string_view>
#include <utility>
#include <glm/gtc/matrix_transform.hpp>
#if !HIVEWE_USE_METAL_MAP_CANVAS
#include <glad/glad.h>
#include <glm/glm.hpp>
#endif
#include "std_module_compat.h"
#if !HIVEWE_USE_METAL_MAP_CANVAS
import OpenGLShapeResources;
#endif
#if HIVEWE_USE_METAL_MAP_CANVAS
#include "rendering/metal_map_frame_renderer.h"
#include "rendering/metal_map_frame_builders.h"
#include "rendering/metal_particle_emitter1_model_frame.h"
#endif
import Camera;
import Rects;
import MapGlobal;
import Globals;
import ResourceManager;
import Doodad;
import Doodads;
import SkeletalModelInstance;
import SkinnedMesh;
import Units;
import Utilities;
import Hierarchy;

namespace {
bool doodad_id_uses_click_helper(const std::string_view id) {
	const std::string key(id);
	if (doodads_slk.row_headers.contains(key)) {
		return doodads_slk.data<bool>("useclickhelper", key);
	}
	return destructibles_slk.data<bool>("useclickhelper", key);
}

bool doodad_uses_click_helper(const Doodad& doodad) {
	return doodad_id_uses_click_helper(doodad.id);
}

bool map_canvas_smoke_capture_mode = false;

double map_canvas_frame_delta(QElapsedTimer& elapsed_timer) {
	const double elapsed = elapsed_timer.nsecsElapsed() / 1'000'000'000.0;
	elapsed_timer.start();
	return map_canvas_smoke_capture_mode ? 0.0 : elapsed;
}

#if HIVEWE_USE_METAL_MAP_CANVAS
double metal_map_canvas_surface_scale(const QWidget* widget) {
	return map_canvas_smoke_capture_mode ? 1.0 : widget->devicePixelRatioF();
}

bool has_pickable_particle_emitter1_model_spawns(
	const std::shared_ptr<SkinnedMesh>& mesh,
	const SkeletalModelInstance& skeleton
) {
	if (!mesh || !mesh->mdx || mesh->mdx->emitters1.empty() || skeleton.sequence_index < 0) {
		return false;
	}

	for (std::size_t emitter_index = 0; emitter_index < mesh->mdx->emitters1.size(); ++emitter_index) {
		const auto& emitter = mesh->mdx->emitters1[emitter_index];
		const std::uint32_t flags = static_cast<std::uint32_t>(emitter.node.flags);
		if (emitter.node.id < 0 || (flags & hive::metal::detail::particle_emitter1_uses_tga) != 0u) {
			continue;
		}
		if ((flags & hive::metal::detail::particle_emitter1_uses_mdl) == 0u && emitter.path.empty()) {
			continue;
		}
		if (emitter_index >= mesh->emitter1_model_meshes.size() || !mesh->emitter1_model_meshes[emitter_index]) {
			continue;
		}
		if (emitter_index >= skeleton.particle_emitters1.pools.size()) {
			continue;
		}
		const auto& pool = skeleton.particle_emitters1.pools[emitter_index];
		if (pool.alive_count > 0 && !pool.positions.empty()) {
			return true;
		}
	}
	return false;
}
#endif

SkeletalModelInstance make_click_helper_skeleton(const std::shared_ptr<SkinnedMesh>& click_helper, const glm::mat4& matrix) {
	SkeletalModelInstance skeleton(click_helper->mdx);
	skeleton.matrix = matrix;
	skeleton.update(0.016f);
	return skeleton;
}

SkeletalModelInstance make_start_location_marker_skeleton(const std::shared_ptr<SkinnedMesh>& marker_mesh, const Unit& unit) {
	SkeletalModelInstance skeleton(marker_mesh->mdx);
	skeleton.update_location(unit.position, unit.angle, glm::vec3(4.0f / 128.0f));
	skeleton.update(0.016f);
	return skeleton;
}

#if HIVEWE_USE_METAL_MAP_CANVAS
template <typename MeshIndicesT>
void append_start_location_marker(
	hive::metal::ModelCpuFrame& frame,
	MeshIndicesT& mesh_indices,
	const Camera& camera,
	const std::shared_ptr<SkinnedMesh>& marker_mesh,
	const Unit& unit,
	const std::uint32_t pick_id = 0u
) {
	if (unit.id != "sloc" || !marker_mesh || !marker_mesh->has_mesh || !marker_mesh->mdx) {
		return;
	}

	SkeletalModelInstance marker_skeleton = make_start_location_marker_skeleton(marker_mesh, unit);
	hive::metal::detail::append_model_instance(
		frame,
		mesh_indices,
		camera,
		marker_mesh.get(),
		marker_skeleton,
		glm::vec3(0.25f, 0.65f, 1.0f),
		static_cast<std::uint32_t>(std::max(0, unit.player)),
		pick_id
	);
}
#endif
} // namespace

namespace {
struct SmokeFrameStats {
	std::uint64_t rgb_sum = 0;
	std::size_t varied_pixels = 0;
};

SmokeFrameStats smoke_frame_stats(const std::vector<std::uint8_t>& pixels) {
	SmokeFrameStats stats;
	if (pixels.size() < 4) {
		return stats;
	}

	const std::array first_rgb {
		pixels[0],
		pixels[1],
		pixels[2],
	};
	for (std::size_t i = 0; i + 3 < pixels.size(); i += 4) {
		stats.rgb_sum += pixels[i] + pixels[i + 1] + pixels[i + 2];
		if (pixels[i] != first_rgb[0] || pixels[i + 1] != first_rgb[1] || pixels[i + 2] != first_rgb[2]) {
			++stats.varied_pixels;
		}
	}
	return stats;
}

std::uint64_t smoke_frame_delta(const std::vector<std::uint8_t>& a, const std::vector<std::uint8_t>& b) {
	const std::size_t byte_count = std::min(a.size(), b.size());
	std::uint64_t delta = 0;
	for (std::size_t i = 0; i + 3 < byte_count; i += 4) {
		delta += static_cast<std::uint64_t>(std::abs(static_cast<int>(a[i]) - static_cast<int>(b[i])));
		delta += static_cast<std::uint64_t>(std::abs(static_cast<int>(a[i + 1]) - static_cast<int>(b[i + 1])));
		delta += static_cast<std::uint64_t>(std::abs(static_cast<int>(a[i + 2]) - static_cast<int>(b[i + 2])));
	}
	return delta;
}

bool validate_smoke_frame_varied(
	const std::string_view frame_label,
	const std::vector<std::uint8_t>& pixels,
	const int width,
	const int height
) {
	if (pixels.empty() || width <= 0 || height <= 0) {
		std::println("Smoke render frame '{}' failed: missing captured frame pixels.", frame_label);
		return false;
	}

	const SmokeFrameStats stats = smoke_frame_stats(pixels);
	const std::size_t pixel_count = pixels.size() / 4;
	const std::size_t minimum_varied_pixels = std::max<std::size_t>(128, pixel_count / 1000);
	if (stats.varied_pixels < minimum_varied_pixels) {
		std::println(
			"Smoke render frame '{}' failed: frame looks flat ({} varied pixels, expected at least {}).",
			frame_label,
			stats.varied_pixels,
			minimum_varied_pixels
		);
		return false;
	}

	return true;
}

bool validate_smoke_render_mode_frames(
	const std::string_view mode_label,
	const std::vector<std::uint8_t>& baseline_pixels,
	const std::vector<std::uint8_t>& mode_pixels,
	const int baseline_width,
	const int baseline_height,
	const int mode_width,
	const int mode_height
) {
	if (baseline_pixels.empty() || mode_pixels.empty() || baseline_width <= 0 || baseline_height <= 0 || mode_width <= 0 || mode_height <= 0) {
		std::println("Smoke render mode '{}' failed: missing captured frame pixels.", mode_label);
		return false;
	}
	if (baseline_width != mode_width || baseline_height != mode_height || baseline_pixels.size() != mode_pixels.size()) {
		std::println("Smoke render mode '{}' failed: baseline and mode frame dimensions differ.", mode_label);
		return false;
	}

	if (!validate_smoke_frame_varied(mode_label, mode_pixels, mode_width, mode_height)) {
		return false;
	}

	const SmokeFrameStats mode_stats = smoke_frame_stats(mode_pixels);
	const std::uint64_t delta = smoke_frame_delta(baseline_pixels, mode_pixels);
	const std::uint64_t minimum_delta = std::max<std::uint64_t>(1024, mode_pixels.size() / 8192);
	if (delta < minimum_delta) {
		std::println("Smoke render mode '{}' failed: toggle did not change the rendered frame enough (delta {}).", mode_label, delta);
		return false;
	}

	std::println(
		"Smoke render mode '{}' produced a varied frame ({} varied pixels, rgb sum {}, delta {}).",
		mode_label,
		mode_stats.varied_pixels,
		mode_stats.rgb_sum,
		delta
	);
	return true;
}

enum class SmokeArtifactPixelFormat {
	Rgba,
	Bgra,
};

std::array<std::uint8_t, 3> smoke_pixel_rgb(
	const std::vector<std::uint8_t>& pixels,
	const std::size_t offset,
	const SmokeArtifactPixelFormat pixel_format
) {
	if (pixel_format == SmokeArtifactPixelFormat::Bgra) {
		return {pixels[offset + 2], pixels[offset + 1], pixels[offset]};
	}
	return {pixels[offset], pixels[offset + 1], pixels[offset + 2]};
}

bool validate_smoke_brush_overlay_pixels(
	const std::vector<std::uint8_t>& baseline_pixels,
	const std::vector<std::uint8_t>& brush_pixels,
	const int baseline_width,
	const int baseline_height,
	const int brush_width,
	const int brush_height,
	const SmokeArtifactPixelFormat pixel_format
) {
	if (baseline_pixels.empty() || brush_pixels.empty() || baseline_width <= 0 || baseline_height <= 0 || brush_width <= 0 || brush_height <= 0) {
		std::println("Smoke render mode 'brush mask' failed: missing captured frame pixels.");
		return false;
	}
	if (baseline_width != brush_width || baseline_height != brush_height || baseline_pixels.size() != brush_pixels.size()) {
		std::println("Smoke render mode 'brush mask' failed: baseline and brush frame dimensions differ.");
		return false;
	}

	std::size_t magenta_pixels = 0;
	std::uint64_t magenta_delta = 0;
	for (std::size_t offset = 0; offset + 3 < brush_pixels.size(); offset += 4) {
		const auto baseline = smoke_pixel_rgb(baseline_pixels, offset, pixel_format);
		const auto brush = smoke_pixel_rgb(brush_pixels, offset, pixel_format);
		const int red_delta = static_cast<int>(brush[0]) - static_cast<int>(baseline[0]);
		const int blue_delta = static_cast<int>(brush[2]) - static_cast<int>(baseline[2]);
		if (brush[0] >= 120 && brush[2] >= 120 && brush[0] > brush[1] + 25 && brush[2] > brush[1] + 25 && red_delta > 25
			&& blue_delta > 25) {
			++magenta_pixels;
			magenta_delta += static_cast<std::uint64_t>(red_delta + blue_delta);
		}
	}

	const std::size_t pixel_count = brush_pixels.size() / 4;
	const std::size_t minimum_magenta_pixels = std::max<std::size_t>(24, pixel_count / 20'000);
	if (magenta_pixels < minimum_magenta_pixels) {
		std::println(
			"Smoke render mode 'brush mask' failed: saw {} magenta brush pixels, expected at least {}.",
			magenta_pixels,
			minimum_magenta_pixels
		);
		return false;
	}

	std::println(
		"Smoke render mode 'brush mask' validated {} magenta brush pixels with color delta {}.",
		magenta_pixels,
		magenta_delta
	);
	return true;
}

int smoke_channel_dominance(const std::array<std::uint8_t, 3>& rgb, const std::size_t channel) {
	const std::size_t other_a = channel == 0 ? 1 : 0;
	const std::size_t other_b = channel == 2 ? 1 : 2;
	return static_cast<int>(rgb[channel]) - std::max(static_cast<int>(rgb[other_a]), static_cast<int>(rgb[other_b]));
}

bool validate_smoke_pathing_overlay_pixels(
	const std::vector<std::uint8_t>& baseline_pixels,
	const std::vector<std::uint8_t>& pathing_pixels,
	const int baseline_width,
	const int baseline_height,
	const int pathing_width,
	const int pathing_height,
	const SmokeArtifactPixelFormat pixel_format
) {
	if (baseline_pixels.empty() || pathing_pixels.empty() || baseline_width <= 0 || baseline_height <= 0 || pathing_width <= 0
		|| pathing_height <= 0) {
		std::println("Smoke render mode 'pathing overlay' failed: missing captured frame pixels.");
		return false;
	}
	if (baseline_width != pathing_width || baseline_height != pathing_height || baseline_pixels.size() != pathing_pixels.size()) {
		std::println("Smoke render mode 'pathing overlay' failed: baseline and pathing frame dimensions differ.");
		return false;
	}

	std::array<std::size_t, 3> pathing_pixels_by_channel {};
	for (std::size_t offset = 0; offset + 3 < pathing_pixels.size(); offset += 4) {
		const auto baseline = smoke_pixel_rgb(baseline_pixels, offset, pixel_format);
		const auto pathing = smoke_pixel_rgb(pathing_pixels, offset, pixel_format);
		for (std::size_t channel = 0; channel < pathing_pixels_by_channel.size(); ++channel) {
			const int pathing_dominance = smoke_channel_dominance(pathing, channel);
			const int baseline_dominance = smoke_channel_dominance(baseline, channel);
			if (pathing[channel] >= 80 && pathing_dominance >= baseline_dominance + 25) {
				++pathing_pixels_by_channel[channel];
			}
		}
	}

	const std::size_t pixel_count = pathing_pixels.size() / 4;
	const std::size_t minimum_pathing_pixels = std::max<std::size_t>(24, pixel_count / 25'000);
	if (pathing_pixels_by_channel[0] < minimum_pathing_pixels || pathing_pixels_by_channel[1] < minimum_pathing_pixels
		|| pathing_pixels_by_channel[2] < minimum_pathing_pixels) {
		std::println(
			"Smoke render mode 'pathing overlay' failed: saw {} red, {} green, and {} blue pathing pixels, expected at least {} of each.",
			pathing_pixels_by_channel[0],
			pathing_pixels_by_channel[1],
			pathing_pixels_by_channel[2],
			minimum_pathing_pixels
		);
		return false;
	}

	std::println(
		"Smoke render mode 'pathing overlay' validated {} red, {} green, and {} blue pathing pixels.",
		pathing_pixels_by_channel[0],
		pathing_pixels_by_channel[1],
		pathing_pixels_by_channel[2]
	);
	return true;
}

bool save_smoke_frame_artifact(
	const QString& artifact_directory,
	const std::string_view file_name,
	const std::vector<std::uint8_t>& pixels,
	const int width,
	const int height,
	const SmokeArtifactPixelFormat pixel_format
) {
	if (artifact_directory.isEmpty()) {
		return true;
	}

	const std::size_t pixel_count = static_cast<std::size_t>(width) * static_cast<std::size_t>(height);
	if (width <= 0 || height <= 0 || pixels.size() < pixel_count * 4) {
		std::println("Smoke render artifact '{}' failed: missing captured pixels.", file_name);
		return false;
	}

	const QString output_path = QDir(artifact_directory).filePath(
		QString::fromUtf8(file_name.data(), static_cast<qsizetype>(file_name.size()))
	);
	const QImage::Format image_format = pixel_format == SmokeArtifactPixelFormat::Rgba
		? QImage::Format_RGBA8888
		: QImage::Format_ARGB32;
	const QImage image(
		pixels.data(),
		width,
		height,
		width * 4,
		image_format
	);
	if (image.isNull() || !image.copy().save(output_path, "PNG")) {
		std::println("Smoke render artifact '{}' failed: unable to save {}.", file_name, output_path.toStdString());
		return false;
	}

	std::println("Captured smoke render artifact: {}", output_path.toStdString());
	return true;
}

std::vector<std::uint8_t> image_pixels_rgba(const QImage& source) {
	const QImage image = source.convertToFormat(QImage::Format_RGBA8888);
	std::vector<std::uint8_t> pixels(static_cast<std::size_t>(image.width()) * static_cast<std::size_t>(image.height()) * 4);
	for (int y = 0; y < image.height(); ++y) {
		std::memcpy(
			pixels.data() + static_cast<std::size_t>(y) * static_cast<std::size_t>(image.width()) * 4,
			image.constScanLine(y),
			static_cast<std::size_t>(image.width()) * 4
		);
	}
	return pixels;
}

class SmokeRenderModesBrush final : public Brush {
  public:
	SmokeRenderModesBrush() {
		brush_color = {255, 0, 255, 220};
		set_shape(Shape::square);
		set_size(glm::ivec2(16));
	}

	void apply(double) override {
	}
};

enum class SmokeRenderMode {
	Pathing,
	Brush,
	Lighting,
	Water,
	Wireframe,
};

struct SmokeRenderModeCase {
	std::string_view label;
	SmokeRenderMode mode;
	std::string_view artifact_file_name;
};

struct SmokeRenderStateSnapshot {
	Camera camera_state = camera;
	bool terrain_water_state_valid = map != nullptr && map->loaded;
	std::vector<float> corner_water_height = terrain_water_state_valid ? map->terrain.corner_water_height : std::vector<float> {};
	std::vector<uint8_t> corner_water = terrain_water_state_valid ? map->terrain.corner_water : std::vector<uint8_t> {};
	float current_water_texture = terrain_water_state_valid ? map->terrain.current_texture : 0.0f;
	bool pathing_state_valid = map != nullptr && map->loaded;
	std::vector<uint8_t> pathing_cells_static = pathing_state_valid ? map->pathing_map.pathing_cells_static : std::vector<uint8_t> {};
	std::vector<uint8_t> pathing_cells_dynamic = pathing_state_valid ? map->pathing_map.pathing_cells_dynamic : std::vector<uint8_t> {};
	Brush* brush = map != nullptr ? map->brush : nullptr;
	bool render_units = map != nullptr && map->render_units;
	bool render_doodads = map != nullptr && map->render_doodads;
	bool render_pathing = map != nullptr && map->render_pathing;
	bool render_brush = map != nullptr && map->render_brush;
	bool render_click_helpers = map != nullptr && map->render_click_helpers;
	bool render_lighting = map != nullptr && map->render_lighting;
	bool render_water = map != nullptr && map->render_water;
	bool render_wireframe = map != nullptr && map->render_wireframe;
	std::size_t doodad_count = map != nullptr ? map->doodads.doodads.size() : 0;
	int doodad_auto_increment = Doodad::auto_increment;
	glm::vec3 mouse_world = input_handler.mouse_world;
	glm::vec3 previous_mouse_world = input_handler.previous_mouse_world;
	glm::vec2 previous_mouse = input_handler.previous_mouse;

	void restore() const {
		camera = camera_state;
		if (map == nullptr) {
			return;
		}
		if (terrain_water_state_valid && corner_water_height.size() == map->terrain.corner_water_height.size()
			&& corner_water.size() == map->terrain.corner_water.size()) {
			map->terrain.corner_water_height = corner_water_height;
			map->terrain.corner_water = corner_water;
			map->terrain.current_texture = current_water_texture;
			map->terrain.update_water(TerrainRect(0, 0, map->terrain.width, map->terrain.height));
		}
		if (pathing_state_valid && pathing_cells_static.size() == map->pathing_map.pathing_cells_static.size()
			&& pathing_cells_dynamic.size() == map->pathing_map.pathing_cells_dynamic.size()) {
			map->pathing_map.pathing_cells_static = pathing_cells_static;
			map->pathing_map.pathing_cells_dynamic = pathing_cells_dynamic;
			map->pathing_map.upload_static_pathing();
			map->pathing_map.upload_dynamic_pathing();
		}
		map->brush = brush;
		map->render_units = render_units;
		map->render_doodads = render_doodads;
		map->render_pathing = render_pathing;
		map->render_brush = render_brush;
		map->render_click_helpers = render_click_helpers;
		map->render_lighting = render_lighting;
		map->render_water = render_water;
		map->render_wireframe = render_wireframe;
		map->doodads.doodads.resize(doodad_count);
		Doodad::auto_increment = doodad_auto_increment;
		input_handler.mouse_world = mouse_world;
		input_handler.previous_mouse_world = previous_mouse_world;
		input_handler.previous_mouse = previous_mouse;
	}
};

glm::vec2 smoke_render_focus_position() {
	if (!map || !map->loaded || map->terrain.width <= 0 || map->terrain.height <= 0) {
		return glm::vec2(0.0f);
	}

	glm::vec2 water_sum(0.0f);
	std::size_t water_count = 0;
	for (int y = 0; y < map->terrain.height; ++y) {
		for (int x = 0; x < map->terrain.width; ++x) {
			if (map->terrain.corner_water[map->terrain.ci(x, y)]
				&& map->terrain.corner_final_water_height(x, y) > map->terrain.corner_final_ground_height(x, y)) {
				water_sum += glm::vec2(static_cast<float>(x), static_cast<float>(y));
				++water_count;
			}
		}
	}

	if (water_count > 0) {
		return water_sum / static_cast<float>(water_count);
	}

	return glm::vec2(
		static_cast<float>(map->terrain.width) * 0.5f,
		static_cast<float>(map->terrain.height) * 0.5f
	);
}

void ensure_smoke_water_patch(const glm::vec2& focus_position) {
	if (!map || !map->loaded || map->terrain.width <= 2 || map->terrain.height <= 2) {
		return;
	}

	// The bundled smoke map does not always show water from the startup camera.
	// Stage a temporary patch and restore it after the smoke captures.
	const int center_x = std::clamp(static_cast<int>(std::lround(focus_position.x)), 1, map->terrain.width - 2);
	const int center_y = std::clamp(static_cast<int>(std::lround(focus_position.y)), 1, map->terrain.height - 2);
	const int radius = std::clamp(std::min(map->terrain.width, map->terrain.height) / 12, 4, 10);
	const int left = std::max(0, center_x - radius);
	const int right = std::min(map->terrain.width - 1, center_x + radius);
	const int top = std::max(0, center_y - radius);
	const int bottom = std::min(map->terrain.height - 1, center_y + radius);

	for (int y = top; y <= bottom; ++y) {
		for (int x = left; x <= right; ++x) {
			const std::size_t idx = map->terrain.ci(x, y);
			map->terrain.corner_water[idx] = true;
			map->terrain.corner_water_height[idx] = map->terrain.corner_final_ground_height(x, y) - map->terrain.water_offset + 1.0f;
		}
	}
	map->terrain.update_water(TerrainRect(left, top, right - left + 1, bottom - top + 1));
}

void ensure_smoke_pathing_patch(const glm::vec2& focus_position) {
	if (!map || !map->loaded || map->pathing_map.width <= 4 || map->pathing_map.height <= 4
		|| map->pathing_map.pathing_cells_static.size() != static_cast<std::size_t>(map->pathing_map.width * map->pathing_map.height)
		|| map->pathing_map.pathing_cells_dynamic.size() != static_cast<std::size_t>(map->pathing_map.width * map->pathing_map.height)) {
		return;
	}

	const int center_x = std::clamp(static_cast<int>(std::lround(focus_position.x * 4.0f)), 2, map->pathing_map.width - 3);
	const int center_y = std::clamp(static_cast<int>(std::lround(focus_position.y * 4.0f)), 2, map->pathing_map.height - 3);
	const int horizontal_radius = std::clamp(map->pathing_map.width / 12, 12, 30);
	const int vertical_radius = std::clamp(map->pathing_map.height / 16, 8, 24);
	const int left = std::max(0, center_x - horizontal_radius);
	const int right = std::min(map->pathing_map.width - 1, center_x + horizontal_radius);
	const int top = std::max(0, center_y - vertical_radius);
	const int bottom = std::min(map->pathing_map.height - 1, center_y + vertical_radius);
	const int patch_width = std::max(1, right - left + 1);

	for (int y = top; y <= bottom; ++y) {
		for (int x = left; x <= right; ++x) {
			const std::size_t index = static_cast<std::size_t>(y * map->pathing_map.width + x);
			const int stripe = std::min(2, ((x - left) * 3) / patch_width);
			map->pathing_map.pathing_cells_static[index] &= static_cast<uint8_t>(~0x0Eu);
			map->pathing_map.pathing_cells_dynamic[index] &= static_cast<uint8_t>(~0x0Eu);
			if (stripe == 0) {
				map->pathing_map.pathing_cells_static[index] |= 0x02u;
			} else if (stripe == 1) {
				map->pathing_map.pathing_cells_dynamic[index] |= 0x04u;
			} else {
				map->pathing_map.pathing_cells_static[index] |= 0x08u;
			}
		}
	}
	map->pathing_map.upload_static_pathing();
	map->pathing_map.upload_dynamic_pathing();
}

bool prepare_smoke_render_baseline() {
	if (!map || !map->loaded || map->terrain.width <= 0 || map->terrain.height <= 0) {
		return false;
	}

	const glm::vec2 focus_position = smoke_render_focus_position();
	ensure_smoke_water_patch(focus_position);
	ensure_smoke_pathing_patch(focus_position);
	const float terrain_height = map->terrain.interpolated_height(focus_position.x, focus_position.y, true);

	camera.position = glm::vec3(focus_position, terrain_height);
	camera.distance = 20.0f;
	camera.horizontal_angle = 0.0f;
	camera.vertical_angle = -0.977f;
	camera.update(0.0);

	input_handler.mouse_world = glm::vec3(focus_position, terrain_height);
	input_handler.previous_mouse_world = input_handler.mouse_world;
	input_handler.previous_mouse = input_handler.mouse;

	map->terrain.current_texture = 0.0f;
	map->render_units = false;
	map->render_doodads = false;
	map->render_pathing = false;
	map->render_brush = false;
	map->render_click_helpers = false;
	map->render_lighting = true;
	map->render_water = true;
	map->render_wireframe = false;
	return true;
}

std::optional<std::string> smoke_click_helper_doodad_id() {
	const auto find_in_slk = [](const auto& slk) -> std::optional<std::string> {
		for (const auto& [id, _] : slk.row_headers) {
			try {
				if (!id.empty() && slk.template data<bool>("useclickhelper", id)
					&& !trimmed(slk.template data<std::string_view>("file", id)).empty()) {
					return id;
				}
			} catch (...) {
			}
		}
		return std::nullopt;
	};

	if (auto id = find_in_slk(doodads_slk)) {
		return id;
	}
	return find_in_slk(destructibles_slk);
}

std::optional<std::size_t> append_smoke_click_helper_doodad(const glm::vec2& focus_position, const bool helper_only = false) {
	if (!map || !map->loaded || map->terrain.width <= 0 || map->terrain.height <= 0) {
		return std::nullopt;
	}

	const std::optional<std::string> id = smoke_click_helper_doodad_id();
	if (!id) {
		std::println("Smoke click helpers failed: no helper-enabled doodad metadata was found.");
		return std::nullopt;
	}

	std::shared_ptr<SkinnedMesh> mesh = map->doodads.get_mesh(*id, 0);
	if (!mesh || !mesh->has_mesh || !mesh->mdx) {
		std::println("Smoke click helpers failed: unable to load helper-enabled doodad {}.", *id);
		return std::nullopt;
	}

	const float terrain_height = map->terrain.interpolated_height(focus_position.x, focus_position.y, true);
	Doodad doodad;
	doodad.id = *id;
	doodad.skin_id = *id;
	doodad.variation = 0;
	doodad.position = glm::vec3(focus_position, terrain_height);
	doodad.scale = glm::vec3(1.0f);
	doodad.angle = 0.0f;
	doodad.creation_number = ++Doodad::auto_increment;
	doodad.mesh = helper_only ? std::make_shared<SkinnedMesh>(*mesh) : mesh;
	if (helper_only) {
		doodad.mesh->has_mesh = false;
	}
	doodad.skeleton = SkeletalModelInstance(mesh->mdx);
	if (helper_only) {
		// Keep helper-only smoke coverage independent from arbitrary real SLK defScale values.
		doodad.skeleton.update_location(doodad.position, 0.0f, glm::vec3(4.0f / 128.0f));
		doodad.skeleton.update(0.016f);
	} else {
		doodad.update(map->terrain);
	}

	if (!doodad_id_uses_click_helper(doodad.id)) {
		std::println("Smoke click helpers failed: staged doodad {} does not use click helpers.", doodad.id);
		return std::nullopt;
	}

	map->doodads.doodads.push_back(std::move(doodad));
	return map->doodads.doodads.size() - 1;
}

template <typename CaptureFrame>
bool run_smoke_click_helpers(
	CaptureFrame&& capture_frame,
	const QString& artifact_directory,
	const SmokeArtifactPixelFormat pixel_format
) {
	const glm::vec2 focus_position = smoke_render_focus_position();
	if (!prepare_smoke_render_baseline() || !append_smoke_click_helper_doodad(focus_position, hierarchy.local_data)) {
		std::println("Smoke render mode 'click helpers' failed while preparing a helper-enabled doodad.");
		return false;
	}

	const float terrain_height = map->terrain.interpolated_height(focus_position.x, focus_position.y, true);
	camera.position = glm::vec3(focus_position, terrain_height);
	camera.update(0.0);
	map->render_doodads = true;
	map->render_click_helpers = false;

	std::vector<std::uint8_t> helpers_off_pixels;
	int helpers_off_width = 0;
	int helpers_off_height = 0;
	if (!capture_frame(helpers_off_pixels, helpers_off_width, helpers_off_height)) {
		std::println("Smoke render mode 'click helpers' failed while capturing helpers-off frame.");
		return false;
	}

	map->render_click_helpers = true;
	std::vector<std::uint8_t> helpers_on_pixels;
	int helpers_on_width = 0;
	int helpers_on_height = 0;
	if (!capture_frame(helpers_on_pixels, helpers_on_width, helpers_on_height)) {
		std::println("Smoke render mode 'click helpers' failed while capturing helpers-on frame.");
		return false;
	}

	const bool artifacts_saved = save_smoke_frame_artifact(
		artifact_directory,
		"map-render-click-helpers-off.png",
		helpers_off_pixels,
		helpers_off_width,
		helpers_off_height,
		pixel_format
	) && save_smoke_frame_artifact(
		artifact_directory,
		"map-render-click-helpers-on.png",
		helpers_on_pixels,
		helpers_on_width,
		helpers_on_height,
		pixel_format
	);

	const bool helper_frames_changed = validate_smoke_render_mode_frames(
		"click helpers",
		helpers_off_pixels,
		helpers_on_pixels,
		helpers_off_width,
		helpers_off_height,
		helpers_on_width,
		helpers_on_height
	);
	return artifacts_saved && helper_frames_changed;
}

void apply_smoke_render_mode(const SmokeRenderMode mode) {
	static SmokeRenderModesBrush smoke_brush;

	switch (mode) {
		case SmokeRenderMode::Pathing:
			map->render_pathing = true;
			map->render_water = false;
			break;
		case SmokeRenderMode::Brush:
			map->brush = &smoke_brush;
			map->render_brush = true;
			map->render_water = false;
			break;
		case SmokeRenderMode::Lighting:
			map->render_lighting = false;
			break;
		case SmokeRenderMode::Water:
			map->render_water = false;
			break;
		case SmokeRenderMode::Wireframe:
			map->render_wireframe = true;
			break;
	}
}

template <typename CaptureFrame>
bool run_smoke_render_modes(
	CaptureFrame&& capture_frame,
	const QString& artifact_directory,
	const SmokeArtifactPixelFormat pixel_format
) {
	SmokeRenderStateSnapshot snapshot;

	std::vector<std::uint8_t> baseline_pixels;
	int baseline_width = 0;
	int baseline_height = 0;
	const bool baseline_prepared = prepare_smoke_render_baseline();
	const bool baseline_captured = baseline_prepared && capture_frame(baseline_pixels, baseline_width, baseline_height);
	if (!baseline_prepared) {
		snapshot.restore();
		std::println("Smoke render modes failed while preparing the baseline render state.");
		return false;
	}
	if (!baseline_captured) {
		snapshot.restore();
		std::println("Smoke render modes failed while capturing the baseline frame.");
		return false;
	}
	if (!save_smoke_frame_artifact(
		artifact_directory,
		"map-render-baseline.png",
		baseline_pixels,
		baseline_width,
		baseline_height,
		pixel_format
	)) {
		snapshot.restore();
		return false;
	}
	if (!validate_smoke_frame_varied("baseline", baseline_pixels, baseline_width, baseline_height)) {
		snapshot.restore();
		return false;
	}

	bool ok = true;
	constexpr std::array cases {
		SmokeRenderModeCase {"pathing overlay", SmokeRenderMode::Pathing, "map-render-pathing-overlay.png"},
		SmokeRenderModeCase {"brush mask", SmokeRenderMode::Brush, "map-render-brush-mask.png"},
		SmokeRenderModeCase {"lighting toggle", SmokeRenderMode::Lighting, "map-render-lighting-toggle.png"},
		SmokeRenderModeCase {"water toggle", SmokeRenderMode::Water, "map-render-water-toggle.png"},
		SmokeRenderModeCase {"wireframe toggle", SmokeRenderMode::Wireframe, "map-render-wireframe-toggle.png"},
	};

	for (const SmokeRenderModeCase& smoke_case : cases) {
		std::vector<std::uint8_t> mode_pixels;
		int mode_width = 0;
		int mode_height = 0;
		const bool mode_prepared = prepare_smoke_render_baseline();
		if (mode_prepared) {
			apply_smoke_render_mode(smoke_case.mode);
		}
		const bool mode_captured = mode_prepared && capture_frame(mode_pixels, mode_width, mode_height);
		if (!mode_prepared) {
			std::println("Smoke render mode '{}' failed while preparing render flags.", smoke_case.label);
			ok = false;
			continue;
		}
		if (!mode_captured) {
			std::println("Smoke render mode '{}' failed while capturing the mode frame.", smoke_case.label);
			ok = false;
			continue;
		}

		const bool artifact_saved = save_smoke_frame_artifact(
			artifact_directory,
			smoke_case.artifact_file_name,
			mode_pixels,
			mode_width,
			mode_height,
			pixel_format
		);
		const bool mode_frame_valid = validate_smoke_render_mode_frames(
			smoke_case.label,
			baseline_pixels,
			mode_pixels,
			baseline_width,
			baseline_height,
			mode_width,
			mode_height
		);
		const bool brush_pixels_valid = smoke_case.mode != SmokeRenderMode::Brush
			|| validate_smoke_brush_overlay_pixels(
				baseline_pixels,
				mode_pixels,
				baseline_width,
				baseline_height,
				mode_width,
				mode_height,
				pixel_format
			);
		const bool pathing_pixels_valid = smoke_case.mode != SmokeRenderMode::Pathing
			|| validate_smoke_pathing_overlay_pixels(
				baseline_pixels,
				mode_pixels,
				baseline_width,
				baseline_height,
				mode_width,
				mode_height,
				pixel_format
			);
		ok = artifact_saved && mode_frame_valid && brush_pixels_valid && pathing_pixels_valid && ok;
	}

	ok = run_smoke_click_helpers(capture_frame, artifact_directory, pixel_format) && ok;

	snapshot.restore();
	return ok;
}

std::optional<glm::vec3> model_world_center(
	const std::shared_ptr<SkinnedMesh>& mesh,
	const SkeletalModelInstance& skeleton
) {
	if (!mesh || !mesh->has_mesh || !mesh->mdx) {
		return std::nullopt;
	}

	const bool has_valid_sequence = skeleton.sequence_index >= 0
		&& static_cast<std::size_t>(skeleton.sequence_index) < mesh->mdx->sequences.size();
	const auto& extent = has_valid_sequence ? mesh->mdx->sequences[skeleton.sequence_index].extent : mesh->mdx->extent;
	const glm::vec3 local_center = (extent.minimum + extent.maximum) * 0.5f;
	return glm::vec3(skeleton.matrix * glm::vec4(local_center, 1.0f));
}

std::optional<std::size_t> append_smoke_pick_unit(const std::shared_ptr<SkinnedMesh>& mesh) {
	if (!map || !mesh || !mesh->has_mesh || !mesh->mdx || map->terrain.width <= 0 || map->terrain.height <= 0) {
		return std::nullopt;
	}

	const glm::vec2 terrain_center(
		static_cast<float>(map->terrain.width) * 0.5f,
		static_cast<float>(map->terrain.height) * 0.5f
	);
	const float terrain_height = map->terrain.interpolated_height(terrain_center.x, terrain_center.y, true);

	Unit unit;
	unit.id = "_smk";
	unit.position = glm::vec3(terrain_center, terrain_height);
	unit.angle = 0.0f;
	unit.scale = glm::vec3(1.0f);
	unit.mesh = mesh;
	unit.color = glm::vec3(1.0f);
	unit.skeleton = SkeletalModelInstance(mesh->mdx);
	unit.skeleton.update_location(unit.position, 0.0f, glm::vec3(4.0f / 128.0f));
	unit.skeleton.update(0.016);

	map->units.units.push_back(std::move(unit));
	return map->units.units.size() - 1;
}

std::optional<std::size_t> append_smoke_pick_doodad(const std::shared_ptr<SkinnedMesh>& mesh) {
	if (!map || !mesh || !mesh->has_mesh || !mesh->mdx || map->terrain.width <= 0 || map->terrain.height <= 0) {
		return std::nullopt;
	}

	const glm::vec2 terrain_center(
		static_cast<float>(map->terrain.width) * 0.5f,
		static_cast<float>(map->terrain.height) * 0.5f
	);
	const float terrain_height = map->terrain.interpolated_height(terrain_center.x, terrain_center.y, true);

	Doodad doodad;
	doodad.id = "_smk";
	doodad.skin_id = "_smk";
	doodad.position = glm::vec3(terrain_center, terrain_height);
	doodad.scale = glm::vec3(1.0f);
	doodad.angle = 0.0f;
	doodad.creation_number = ++Doodad::auto_increment;
	doodad.mesh = mesh;
	doodad.color = glm::vec3(1.0f);
	doodad.skeleton = SkeletalModelInstance(mesh->mdx);
	doodad.skeleton.update_location(doodad.position, 0.0f, glm::vec3(4.0f / 128.0f));
	doodad.skeleton.update(0.016);

	map->doodads.doodads.push_back(std::move(doodad));
	return map->doodads.doodads.size() - 1;
}

std::vector<glm::vec2> projected_model_points(
	const std::shared_ptr<SkinnedMesh>& mesh,
	const SkeletalModelInstance& skeleton,
	const int width,
	const int height
) {
	if (!mesh || !mesh->has_mesh || !mesh->mdx || width <= 0 || height <= 0) {
		return {};
	}

	const bool has_valid_sequence = skeleton.sequence_index >= 0
		&& static_cast<std::size_t>(skeleton.sequence_index) < mesh->mdx->sequences.size();
	const auto& extent = has_valid_sequence ? mesh->mdx->sequences[skeleton.sequence_index].extent : mesh->mdx->extent;
	const glm::vec3 center = (extent.minimum + extent.maximum) * 0.5f;

	const std::array local_points {
		center,
		glm::vec3(center.x, center.y, extent.maximum.z),
		glm::vec3(center.x, center.y, extent.minimum.z),
		glm::vec3(0.0f),
	};

	std::vector<glm::vec2> points;
	points.reserve(local_points.size());
	for (const glm::vec3& local_point : local_points) {
		const glm::vec3 world_point = glm::vec3(skeleton.matrix * glm::vec4(local_point, 1.0f));
		const glm::vec3 screen_point = glm::project(
			world_point,
			camera.view,
			camera.projection,
			glm::vec4(0.0f, 0.0f, static_cast<float>(width), static_cast<float>(height))
		);

		if (!std::isfinite(screen_point.x) || !std::isfinite(screen_point.y) || !std::isfinite(screen_point.z)) {
			continue;
		}
		if (screen_point.z < 0.0f || screen_point.z > 1.0f) {
			continue;
		}
		if (screen_point.x < 0.0f || screen_point.x >= static_cast<float>(width) || screen_point.y < 0.0f
			|| screen_point.y >= static_cast<float>(height)) {
			continue;
		}

		points.emplace_back(screen_point.x, static_cast<float>(height) - screen_point.y);
	}

	return points;
}

std::vector<glm::vec2> projected_model_grid_points(
	const std::shared_ptr<SkinnedMesh>& mesh,
	const SkeletalModelInstance& skeleton,
	const int width,
	const int height
) {
	if (!mesh || !mesh->has_mesh || !mesh->mdx || width <= 0 || height <= 0) {
		return {};
	}

	const bool has_valid_sequence = skeleton.sequence_index >= 0
		&& static_cast<std::size_t>(skeleton.sequence_index) < mesh->mdx->sequences.size();
	const auto& extent = has_valid_sequence ? mesh->mdx->sequences[skeleton.sequence_index].extent : mesh->mdx->extent;
	const glm::vec3 min = extent.minimum;
	const glm::vec3 max = extent.maximum;
	const std::array local_corners {
		glm::vec3(min.x, min.y, min.z),
		glm::vec3(min.x, min.y, max.z),
		glm::vec3(min.x, max.y, min.z),
		glm::vec3(min.x, max.y, max.z),
		glm::vec3(max.x, min.y, min.z),
		glm::vec3(max.x, min.y, max.z),
		glm::vec3(max.x, max.y, min.z),
		glm::vec3(max.x, max.y, max.z),
	};

	glm::vec2 screen_min(std::numeric_limits<float>::max());
	glm::vec2 screen_max(std::numeric_limits<float>::lowest());
	bool projected_any = false;
	for (const glm::vec3& local_corner : local_corners) {
		const glm::vec3 world_point = glm::vec3(skeleton.matrix * glm::vec4(local_corner, 1.0f));
		const glm::vec3 screen_point = glm::project(
			world_point,
			camera.view,
			camera.projection,
			glm::vec4(0.0f, 0.0f, static_cast<float>(width), static_cast<float>(height))
		);

		if (!std::isfinite(screen_point.x) || !std::isfinite(screen_point.y) || !std::isfinite(screen_point.z)
			|| screen_point.z < 0.0f || screen_point.z > 1.0f) {
			continue;
		}

		const glm::vec2 mouse_point(screen_point.x, static_cast<float>(height) - screen_point.y);
		screen_min = glm::min(screen_min, mouse_point);
		screen_max = glm::max(screen_max, mouse_point);
		projected_any = true;
	}

	if (!projected_any) {
		return {};
	}

	screen_min = glm::clamp(screen_min - glm::vec2(8.0f), glm::vec2(0.0f), glm::vec2(width - 1, height - 1));
	screen_max = glm::clamp(screen_max + glm::vec2(8.0f), glm::vec2(0.0f), glm::vec2(width - 1, height - 1));
	if (screen_min.x > screen_max.x || screen_min.y > screen_max.y) {
		return {};
	}

	std::vector<glm::vec2> points;
	constexpr int grid_steps = 9;
	points.reserve(grid_steps * grid_steps);
	for (int y = 0; y < grid_steps; ++y) {
		for (int x = 0; x < grid_steps; ++x) {
			const glm::vec2 t(
				grid_steps > 1 ? static_cast<float>(x) / static_cast<float>(grid_steps - 1) : 0.5f,
				grid_steps > 1 ? static_cast<float>(y) / static_cast<float>(grid_steps - 1) : 0.5f
			);
			points.push_back(glm::mix(screen_min, screen_max, t));
		}
	}
	return points;
}

std::vector<glm::vec2> projected_click_helper_only_points(
	const Doodad& doodad,
	const std::shared_ptr<SkinnedMesh>& click_helper,
	const int width,
	const int height
) {
	if (!doodad.mesh || !doodad.mesh->has_mesh || !doodad.mesh->mdx || !click_helper || !click_helper->has_mesh || !click_helper->mdx
		|| width <= 0 || height <= 0) {
		return {};
	}

	const bool base_has_valid_sequence = doodad.skeleton.sequence_index >= 0
		&& static_cast<std::size_t>(doodad.skeleton.sequence_index) < doodad.mesh->mdx->sequences.size();
	const auto& base_extent = base_has_valid_sequence ? doodad.mesh->mdx->sequences[doodad.skeleton.sequence_index].extent : doodad.mesh->mdx->extent;
	const glm::vec3 base_min = base_extent.minimum;
	const glm::vec3 base_max = base_extent.maximum;
	const glm::vec3 helper_min = click_helper->mdx->extent.minimum;
	const glm::vec3 helper_max = click_helper->mdx->extent.maximum;
	const glm::vec3 helper_center = (helper_min + helper_max) * 0.5f;
	constexpr float epsilon = 1e-3f;

	const auto outside_base = [&](const glm::vec3& point) {
		return point.x < base_min.x - epsilon || point.x > base_max.x + epsilon || point.y < base_min.y - epsilon
			|| point.y > base_max.y + epsilon || point.z < base_min.z - epsilon || point.z > base_max.z + epsilon;
	};

	const std::array local_points {
		glm::vec3(helper_min.x, helper_center.y, helper_center.z),
		glm::vec3(helper_max.x, helper_center.y, helper_center.z),
		glm::vec3(helper_center.x, helper_min.y, helper_center.z),
		glm::vec3(helper_center.x, helper_max.y, helper_center.z),
		glm::vec3(helper_center.x, helper_center.y, helper_min.z),
		glm::vec3(helper_center.x, helper_center.y, helper_max.z),
		glm::vec3(helper_min.x, helper_min.y, helper_min.z),
		glm::vec3(helper_min.x, helper_min.y, helper_max.z),
		glm::vec3(helper_min.x, helper_max.y, helper_min.z),
		glm::vec3(helper_min.x, helper_max.y, helper_max.z),
		glm::vec3(helper_max.x, helper_min.y, helper_min.z),
		glm::vec3(helper_max.x, helper_min.y, helper_max.z),
		glm::vec3(helper_max.x, helper_max.y, helper_min.z),
		glm::vec3(helper_max.x, helper_max.y, helper_max.z),
	};

	std::vector<glm::vec2> points;
	points.reserve(local_points.size());
	for (const glm::vec3& local_point : local_points) {
		if (!outside_base(local_point)) {
			continue;
		}

		const glm::vec3 world_point = glm::vec3(doodad.skeleton.matrix * glm::vec4(local_point, 1.0f));
		const glm::vec3 screen_point = glm::project(
			world_point,
			camera.view,
			camera.projection,
			glm::vec4(0.0f, 0.0f, static_cast<float>(width), static_cast<float>(height))
		);

		if (!std::isfinite(screen_point.x) || !std::isfinite(screen_point.y) || !std::isfinite(screen_point.z)) {
			continue;
		}
		if (screen_point.z < 0.0f || screen_point.z > 1.0f) {
			continue;
		}
		if (screen_point.x < 0.0f || screen_point.x >= static_cast<float>(width) || screen_point.y < 0.0f
			|| screen_point.y >= static_cast<float>(height)) {
			continue;
		}

		points.emplace_back(screen_point.x, static_cast<float>(height) - screen_point.y);
	}

	return points;
}

std::vector<glm::vec2> projected_click_helper_grid_points(
	const Doodad& doodad,
	const std::shared_ptr<SkinnedMesh>& click_helper,
	const int width,
	const int height
) {
	if (!click_helper || !click_helper->has_mesh || !click_helper->mdx || width <= 0 || height <= 0) {
		return {};
	}

	const glm::vec3 helper_min = click_helper->mdx->extent.minimum;
	const glm::vec3 helper_max = click_helper->mdx->extent.maximum;
	const std::array local_corners {
		glm::vec3(helper_min.x, helper_min.y, helper_min.z),
		glm::vec3(helper_min.x, helper_min.y, helper_max.z),
		glm::vec3(helper_min.x, helper_max.y, helper_min.z),
		glm::vec3(helper_min.x, helper_max.y, helper_max.z),
		glm::vec3(helper_max.x, helper_min.y, helper_min.z),
		glm::vec3(helper_max.x, helper_min.y, helper_max.z),
		glm::vec3(helper_max.x, helper_max.y, helper_min.z),
		glm::vec3(helper_max.x, helper_max.y, helper_max.z),
	};

	glm::vec2 screen_min(std::numeric_limits<float>::max());
	glm::vec2 screen_max(std::numeric_limits<float>::lowest());
	bool projected_any = false;
	for (const glm::vec3& local_corner : local_corners) {
		const glm::vec3 world_point = glm::vec3(doodad.skeleton.matrix * glm::vec4(local_corner, 1.0f));
		const glm::vec3 screen_point = glm::project(
			world_point,
			camera.view,
			camera.projection,
			glm::vec4(0.0f, 0.0f, static_cast<float>(width), static_cast<float>(height))
		);

		if (!std::isfinite(screen_point.x) || !std::isfinite(screen_point.y) || !std::isfinite(screen_point.z)
			|| screen_point.z < 0.0f || screen_point.z > 1.0f) {
			continue;
		}

		const glm::vec2 mouse_point(screen_point.x, static_cast<float>(height) - screen_point.y);
		screen_min = glm::min(screen_min, mouse_point);
		screen_max = glm::max(screen_max, mouse_point);
		projected_any = true;
	}

	if (!projected_any) {
		return {};
	}

	screen_min = glm::clamp(screen_min - glm::vec2(8.0f), glm::vec2(0.0f), glm::vec2(width - 1, height - 1));
	screen_max = glm::clamp(screen_max + glm::vec2(8.0f), glm::vec2(0.0f), glm::vec2(width - 1, height - 1));
	if (screen_min.x > screen_max.x || screen_min.y > screen_max.y) {
		return {};
	}

	std::vector<glm::vec2> points;
	constexpr int grid_steps = 7;
	points.reserve(grid_steps * grid_steps);
	for (int y = 0; y < grid_steps; ++y) {
		for (int x = 0; x < grid_steps; ++x) {
			const glm::vec2 t(
				grid_steps > 1 ? static_cast<float>(x) / static_cast<float>(grid_steps - 1) : 0.5f,
				grid_steps > 1 ? static_cast<float>(y) / static_cast<float>(grid_steps - 1) : 0.5f
			);
			points.push_back(glm::mix(screen_min, screen_max, t));
		}
	}
	return points;
}
} // namespace

#if !HIVEWE_USE_METAL_MAP_CANVAS
void APIENTRY gl_debug_output(
	const GLenum source,
	const GLenum type,
	const GLuint id,
	const GLenum severity,
	const GLsizei,
	const GLchar* message,
	void*
) {
	// Skip buffer info messages, framebuffer info messages, texture usage state warning, redundant state change buffer
	if (id == 131185 // ?
		|| id == 131169 // ?
		|| id == 131204 // ?
		|| id == 8 // ?
		|| id == 131218) // Unexplainable performance warnings
	{
		return;
	}

	std::println("---------------");
	std::println("Debug message ({})", message);

	switch (source) {
		case GL_DEBUG_SOURCE_API:
			std::println("Source: API");
			break;
		case GL_DEBUG_SOURCE_WINDOW_SYSTEM:
			std::println("Source: Window System");
			break;
		case GL_DEBUG_SOURCE_SHADER_COMPILER:
			std::println("Source: Shader Compiler");
			break;
		case GL_DEBUG_SOURCE_THIRD_PARTY:
			std::println("Source: Third Party");
			break;
		case GL_DEBUG_SOURCE_APPLICATION:
			std::println("Source: Application");
			break;
		case GL_DEBUG_SOURCE_OTHER:
			std::println("Source: Other");
			break;
		default:
			break;
	}

	switch (type) {
		case GL_DEBUG_TYPE_ERROR:
			std::println("Type: Error");
			break;
		case GL_DEBUG_TYPE_DEPRECATED_BEHAVIOR:
			std::println("Type: Deprecated Behaviour");
			break;
		case GL_DEBUG_TYPE_UNDEFINED_BEHAVIOR:
			std::println("Type: Undefined Behaviour");
			break;
		case GL_DEBUG_TYPE_PORTABILITY:
			std::println("Type: Portability");
			break;
		case GL_DEBUG_TYPE_PERFORMANCE:
			std::println("Type: Performance");
			break;
		case GL_DEBUG_TYPE_MARKER:
			std::println("Type: Marker");
			break;
		case GL_DEBUG_TYPE_PUSH_GROUP:
			std::println("Type: Push Group");
			break;
		case GL_DEBUG_TYPE_POP_GROUP:
			std::println("Type: Pop Group");
			break;
		case GL_DEBUG_TYPE_OTHER:
			std::println("Type: Other");
			break;
		default:
			break;
	}

	switch (severity) {
		case GL_DEBUG_SEVERITY_HIGH:
			std::println("Severity: high");
			break;
		case GL_DEBUG_SEVERITY_MEDIUM:
			std::println("Severity: medium");
			break;
		case GL_DEBUG_SEVERITY_LOW:
			std::println("Severity: low");
			break;
		case GL_DEBUG_SEVERITY_NOTIFICATION:
			std::println("Severity: notification");
			break;
		default:
			break;
	}
}
#endif

MapCanvasWidget::MapCanvasWidget(QWidget* parent) : QWidget(parent) {
#if HIVEWE_USE_METAL_MAP_CANVAS
	canvas = new MetalMapCanvas(this);
#else
	canvas = new OpenGLMapCanvas(this);
#endif

	auto* layout = new QVBoxLayout(this);
	layout->setContentsMargins(0, 0, 0, 0);
	layout->setSpacing(0);
	layout->addWidget(canvas);

	setFocusProxy(canvas);
}

#if !HIVEWE_USE_METAL_MAP_CANVAS
void MapCanvasWidget::makeCurrent() {
	canvas->makeCurrent();
}

void MapCanvasWidget::doneCurrent() {
	canvas->doneCurrent();
}
#endif

void MapCanvasWidget::invalidate_map_resources() {
#if HIVEWE_USE_METAL_MAP_CANVAS
	canvas->invalidate_map_resources();
#endif
}

bool MapCanvasWidget::save_rendered_frame(const QString& file_path) {
	if (canvas == nullptr) {
		return false;
	}

#if HIVEWE_USE_METAL_MAP_CANVAS
	return canvas->save_frame_png(file_path);
#else
	const QImage image = canvas->grabFramebuffer();
	if (image.isNull()) {
		return false;
	}

	return image.save(file_path, "PNG");
#endif
}

bool MapCanvasWidget::smoke_render_modes(const QString& artifact_directory) {
	return canvas != nullptr && canvas->smoke_render_modes(artifact_directory);
}

bool MapCanvasWidget::smoke_pick_rendered_objects() {
	return canvas != nullptr && canvas->smoke_pick_rendered_objects();
}

#if HIVEWE_USE_METAL_MAP_CANVAS
MetalMapCanvas::MetalMapCanvas(QWidget* parent)
	: QWidget(parent),
	  map_renderer(hive::metal::make_map_frame_renderer()),
	  terrain_texture_frame(std::make_unique<hive::metal::TerrainTextureCpuFrame>()) {
	setAttribute(Qt::WA_NativeWindow);
	setAttribute(Qt::WA_PaintOnScreen);
	setAttribute(Qt::WA_NoSystemBackground);
	setAttribute(Qt::WA_OpaquePaintEvent);
	setAutoFillBackground(false);
	setMouseTracking(true);
	setFocus();
	setFocusPolicy(Qt::WheelFocus);

	elapsed_timer.start();
	initialize_debug_overlay();

	frame_timer.setInterval(16);
	connect(&frame_timer, &QTimer::timeout, this, [this] {
		update();
	});
	frame_timer.start();
}

MetalMapCanvas::~MetalMapCanvas() = default;

void MetalMapCanvas::invalidate_map_resources() {
	*terrain_texture_frame = {};
	terrain_texture_owner = nullptr;
	observed_terrain_texture_revision = 0;
	click_helper_mesh.reset();
	click_helper_load_failed = false;
	map_renderer->invalidate_resources();
}

bool MetalMapCanvas::ensure_surface() {
	if (surface_initialized) {
		return true;
	}

	surface_initialized = surface.initialize(reinterpret_cast<void*>(winId()));
	if (surface_initialized) {
		surface.resize(width(), height(), metal_map_canvas_surface_scale(this));
	}

	return surface_initialized;
}

void MetalMapCanvas::update_camera_aspect_ratio() {
	if (height() > 0) {
		camera.aspect_ratio = static_cast<double>(width()) / static_cast<double>(height());
	}
}

void MetalMapCanvas::initialize_debug_overlay() {
	debug_overlay = new QLabel(this);
	debug_overlay->setAttribute(Qt::WA_NativeWindow);
	debug_overlay->setAttribute(Qt::WA_TransparentForMouseEvents);
	debug_overlay->setAttribute(Qt::WA_StyledBackground);
	debug_overlay->setAlignment(Qt::AlignLeft | Qt::AlignTop);
	debug_overlay->setContentsMargins(10, 8, 10, 8);
	QFont font("Consolas");
	font.setStyleHint(QFont::Monospace);
	debug_overlay->setFont(font);
	debug_overlay->setStyleSheet(QStringLiteral(
		"QLabel { background-color: rgba(0, 0, 0, 180); color: rgb(255, 255, 255); border: 1px solid rgba(255, 255, 255, 80); }"
	));
	debug_overlay->hide();
}

void MetalMapCanvas::update_debug_overlay() {
	if (!debug_overlay) {
		return;
	}
	if (!map || !map->loaded || !map->render_debug || map->terrain.width <= 0 || map->terrain.height <= 0) {
		debug_overlay->hide();
		return;
	}

	debug_frametimes.push_back(delta);
	if (debug_frametimes.size() > 60) {
		debug_frametimes.erase(debug_frametimes.begin());
	}
	const double average_frametime =
		debug_frametimes.empty() ? 0.0 : std::accumulate(debug_frametimes.begin(), debug_frametimes.end(), 0.0) / debug_frametimes.size();

	const auto fmt_vec3 = [](const char* label, const glm::vec3 v) {
		return std::format("{:<20} X:{:>6.3f}  Y:{:>6.3f}  Z:{:>6.3f}", label, v.x, v.y, v.z);
	};
	const auto fmt_vec2 = [](const char* label, const glm::vec2 v) {
		return std::format("{:<20} X:{:>6.3f}  Y:{:>6.3f}", label, v.x, v.y);
	};

	std::string text = std::format("Total time: {:.2f}ms\n", average_frametime * 1000.0);
	text += fmt_vec3("Mouse World Position", input_handler.mouse_world) + "\n";
	text += fmt_vec3("Camera Position", camera.position) + "\n";
	if (map->brush) {
		text += fmt_vec2("Brush Grid Position", map->brush->get_position()) + "\n";
	}
	text += std::format("Camera Horizontal Angle: {:.4f}\n", camera.horizontal_angle);
	text += std::format("Camera Vertical Angle: {:.4f}\n\n", camera.vertical_angle);

	const int terrain_x = std::clamp(static_cast<int>(input_handler.mouse_world.x), 0, std::max(0, map->terrain.width - 1));
	const int terrain_y = std::clamp(static_cast<int>(input_handler.mouse_world.y), 0, std::max(0, map->terrain.height - 1));
	const auto corner = map->terrain.get_corner(terrain_x, terrain_y);
	text += std::format("Tile info ({}, {})\n", terrain_x, terrain_y);
	text += std::format("Cliff: {}\n", static_cast<int>(corner.cliff));
	text += std::format("Blight: {}\n", static_cast<int>(corner.blight));
	text += std::format("Boundary: {}\n", static_cast<int>(corner.boundary));
	text += std::format("Cliff texture: {}\n", static_cast<int>(corner.cliff_texture));
	text += std::format("Cliff variation: {}\n", static_cast<int>(corner.cliff_variation));
	text += std::format("Ground texture: {}\n", static_cast<int>(corner.ground_texture));
	text += std::format("Ground variation: {}\n", static_cast<int>(corner.ground_variation));
	text += std::format("Height: {}\n", corner.height);
	text += std::format("Layer height: {}\n", static_cast<int>(corner.layer_height));
	text += std::format("Map edge: {}\n", static_cast<int>(corner.map_edge));
	text += std::format("Ramp: {}\n", static_cast<int>(corner.ramp));
	text += std::format("Romp: {}\n", static_cast<int>(corner.romp));
	text += std::format("Special doodad: {}\n", static_cast<int>(corner.special_doodad));
	text += std::format("Water: {}\n", static_cast<int>(corner.water));
	text += std::format("Water height: {}", corner.water_height);

	debug_overlay->setText(QString::fromStdString(text));
	layout_debug_overlay();
	debug_overlay->raise();
	debug_overlay->show();
}

void MetalMapCanvas::layout_debug_overlay() {
	if (!debug_overlay) {
		return;
	}

	debug_overlay->adjustSize();
	const QSize hint = debug_overlay->sizeHint();
	const int overlay_width = std::min(hint.width(), std::max(120, width() - 20));
	const int overlay_height = std::min(hint.height(), std::max(80, height() - 20));
	debug_overlay->setGeometry(10, 10, overlay_width, overlay_height);
}

bool MetalMapCanvas::ensure_click_helper_mesh() {
	if (click_helper_mesh) {
		return true;
	}
	if (click_helper_load_failed) {
		return false;
	}

	auto result = resource_manager.load<SkinnedMesh>("Objects/InvalidObject/InvalidObject.mdx", "", std::nullopt);
	if (!result || !*result) {
		click_helper_load_failed = true;
		if (result) {
			std::println("Metal click helper load failed: empty mesh");
		} else {
			std::println("Metal click helper load failed: {}", result.error());
		}
		return false;
	}

	click_helper_mesh = *result;
	return true;
}

bool MetalMapCanvas::draw_frame(std::vector<std::uint8_t>* captured_bgra, int* captured_width, int* captured_height) {
	if (captured_width != nullptr) {
		*captured_width = 0;
	}
	if (captured_height != nullptr) {
		*captured_height = 0;
	}

	delta = map_canvas_frame_delta(elapsed_timer);
	update_camera_aspect_ratio();

	if (!map) {
		update_debug_overlay();
		FrameMark;
		return false;
	}

	map->update(delta, width(), height());
	bool rendered = false;

	if (ensure_surface()) {
		if (!map->loaded) {
			surface.draw_clear(0.035f, 0.04f, 0.05f, 1.0f);
			rendered = captured_bgra == nullptr;
		} else {
			MetalSurface::Frame frame = surface.begin_frame(0.035f, 0.04f, 0.05f, 1.0f);
			if (frame) {
				const int frame_width = frame.drawable_width();
				const int frame_height = frame.drawable_height();
				hive::metal::FrameContext frame_context {
					.device = frame.device_opaque(),
					.command_queue = frame.command_queue_opaque(),
					.command_buffer = frame.command_buffer_opaque(),
					.render_pass_descriptor = frame.render_pass_descriptor_opaque(),
					.drawable_width = frame.drawable_width(),
					.drawable_height = frame.drawable_height(),
					.scale = frame.scale(),
				};

				hive::metal::MetalMapFrame map_frame;
				map_frame.camera.projection_view = hive::metal::opengl_clip_depth_to_metal(camera.projection_view);
				map_frame.terrain = hive::metal::make_terrain_cpu_frame(map->terrain);
				map_frame.cliffs = hive::metal::make_cliff_cpu_frame(map->terrain);
				map_frame.pathing = hive::metal::make_pathing_cpu_frame(map->pathing_map);
				map_frame.models = hive::metal::make_model_cpu_frame(*map, camera);
				map_frame.particles = hive::metal::make_particle_emitter2_cpu_frame(*map, camera);
				std::unordered_map<const void*, std::uint32_t> brush_mesh_indices;
				std::unordered_map<const void*, std::uint32_t> brush_particle_texture_indices;
				auto append_click_helper = [&](const glm::mat4& model) {
					if (!ensure_click_helper_mesh()) {
						return;
					}
					const auto click_helper = std::static_pointer_cast<SkinnedMesh>(click_helper_mesh);
					SkeletalModelInstance helper_skeleton = make_click_helper_skeleton(click_helper, model);
					hive::metal::detail::append_model_instance(
						map_frame.models,
						brush_mesh_indices,
						camera,
						click_helper.get(),
						helper_skeleton,
						glm::vec3(1.0f),
						0
					);
				};
				map->render_manager->initialize_callbacks(
					[this](const Units&, const glm::vec2 mouse_position) -> std::optional<std::size_t> {
						return pick_unit_id(mouse_position);
					},
					[this](const Doodads&, const glm::vec2 mouse_position) -> std::optional<std::size_t> {
						return pick_doodad_id(mouse_position);
					},
					[&](SkinnedMesh& skinned_mesh, const SkeletalModelInstance& skeleton, const glm::vec3 color, const std::uint32_t team_color_index) {
						hive::metal::append_queued_model_to_map_frames(
							map_frame.models,
							map_frame.particles,
							brush_mesh_indices,
							brush_particle_texture_indices,
							camera,
							skinned_mesh,
							skeleton,
							color,
							team_color_index
						);
					},
					append_click_helper
				);
				if (map->render_doodads && map->render_click_helpers) {
					for (const auto& doodad : map->doodads.doodads) {
						if (!doodad_uses_click_helper(doodad)) {
							continue;
						}
						append_click_helper(doodad.skeleton.matrix);
					}
				}
				if (map->render_units) {
					if (ensure_click_helper_mesh()) {
						const auto start_location_marker = std::static_pointer_cast<SkinnedMesh>(click_helper_mesh);
						for (const auto& unit : map->units.units) {
							append_start_location_marker(map_frame.models, brush_mesh_indices, camera, start_location_marker, unit);
						}
					}
				}
				if (map->render_brush && map->brush) {
					if (map->brush->get_mode() == Brush::Mode::placement) {
						map->brush->render_brush();
					} else if (map->brush->get_mode() == Brush::Mode::pasting) {
						map->brush->render_clipboard();
					}
				}
				hive::metal::finalize_model_cpu_frame(map_frame.models);
				hive::metal::finalize_particle_emitter2_cpu_frame(map_frame.particles);
				install_picking_callbacks();
				map_frame.selection_overlay = make_selection_overlay_frame();
				const bool texture_owner_changed = terrain_texture_owner != map;
				const bool texture_revision_changed = observed_terrain_texture_revision != map->terrain.texture_revision;
				const bool texture_frame_missing_expected_ground = !terrain_texture_frame->ground_valid();
				const bool texture_frame_missing_expected_water =
					!map->terrain.water_textures.empty() && !terrain_texture_frame->water_valid();
				const bool texture_frame_missing_expected_cliff =
					!map->terrain.cliff_textures.empty() && !terrain_texture_frame->cliff_valid();
				if (texture_owner_changed || texture_revision_changed || texture_frame_missing_expected_ground
					|| texture_frame_missing_expected_water || texture_frame_missing_expected_cliff) {
					*terrain_texture_frame = hive::metal::make_terrain_texture_cpu_frame(map->terrain);
					terrain_texture_owner = map;
					observed_terrain_texture_revision = map->terrain.texture_revision;
					map_renderer->invalidate_resources();
				}
				map_frame.textures = terrain_texture_frame.get();
				map_frame.show_pathing = map->render_pathing;
				map_frame.show_lighting = map->render_lighting;
				map_frame.show_water = map->render_water;
				map_frame.show_wireframe = map->render_wireframe;
				map_frame.light_direction = map->light_direction;
				map_frame.water_offset = map->terrain.water_offset;
				if (terrain_texture_frame->water_layer_count > 0) {
					map_frame.water_texture_index = static_cast<std::uint32_t>(
						std::clamp(static_cast<int>(map->terrain.current_texture), 0, terrain_texture_frame->water_layer_count - 1)
					);
				}
				map_frame.shallow_color_min = map->terrain.shallow_color_min;
				map_frame.shallow_color_max = map->terrain.shallow_color_max;
				map_frame.deep_color_min = map->terrain.deep_color_min;
				map_frame.deep_color_max = map->terrain.deep_color_max;

				if (map->render_brush && map->brush && map->brush->get_mode() != Brush::Mode::selection) {
					map_frame.show_brush = true;
					map_frame.brush = hive::metal::make_brush_mask_cpu_frame(*map->brush);
				}

				bool render_ok = true;
				if (!map_renderer->render(frame_context, map_frame)) {
					std::println("Metal map renderer error: {}", map_renderer->last_error());
					render_ok = false;
				}
				std::vector<std::uint8_t> frame_pixels;
				const bool present_ok = surface.end_frame(std::move(frame), captured_bgra != nullptr ? &frame_pixels : nullptr);
				if (present_ok && captured_bgra != nullptr) {
					*captured_bgra = std::move(frame_pixels);
					if (captured_width != nullptr) {
						*captured_width = frame_width;
					}
					if (captured_height != nullptr) {
						*captured_height = frame_height;
					}
				}
				rendered = render_ok && present_ok;
			}
		}
	}

	update_debug_overlay();

	FrameMark;
	return rendered;
}

bool MetalMapCanvas::save_frame_png(const QString& file_path) {
	std::vector<std::uint8_t> pixels;
	int image_width = 0;
	int image_height = 0;
	if (!draw_frame(&pixels, &image_width, &image_height) || pixels.empty() || image_width <= 0 || image_height <= 0) {
		return false;
	}

	const QImage image(
		pixels.data(),
		image_width,
		image_height,
		image_width * 4,
		QImage::Format_ARGB32
	);
	if (image.isNull()) {
		return false;
	}

	return image.copy().save(file_path, "PNG");
}

bool MetalMapCanvas::smoke_render_modes(const QString& artifact_directory) {
	if (!map || !map->loaded) {
		std::println("Smoke render modes requested before the startup map was loaded.");
		return false;
	}

	const bool previous_smoke_capture_mode = map_canvas_smoke_capture_mode;
	map_canvas_smoke_capture_mode = true;
	if (surface_initialized) {
		surface.resize(width(), height(), 1.0);
	}

	const bool ok = run_smoke_render_modes(
		[this](std::vector<std::uint8_t>& pixels, int& width, int& height) {
			return draw_frame(&pixels, &width, &height);
		},
		artifact_directory,
		SmokeArtifactPixelFormat::Bgra
	);
	map_canvas_smoke_capture_mode = previous_smoke_capture_mode;
	if (surface_initialized) {
		surface.resize(width(), height(), metal_map_canvas_surface_scale(this));
	}
	return ok;
}

bool MetalMapCanvas::smoke_pick_rendered_objects() {
	if (!map || !map->loaded) {
		std::println("Smoke picking requested before the startup map was loaded.");
		return false;
	}

	const Camera camera_snapshot = camera;
	const std::size_t unit_count_snapshot = map->units.units.size();
	const std::size_t doodad_count_snapshot = map->doodads.doodads.size();
	const int unit_auto_increment_snapshot = Unit::auto_increment;
	const int doodad_auto_increment_snapshot = Doodad::auto_increment;
	const auto restore_smoke_state = [&]() {
		camera = camera_snapshot;
		map->units.units.resize(unit_count_snapshot);
		map->doodads.doodads.resize(doodad_count_snapshot);
		Unit::auto_increment = unit_auto_increment_snapshot;
		Doodad::auto_increment = doodad_auto_increment_snapshot;
	};

	bool attempted_unit_pick = false;
	bool attempted_doodad_pick = false;
	bool attempted_click_helper_pick = false;
	bool attempted_start_location_pick = false;
	bool start_location_pick_ok = false;
	const bool has_start_location = std::ranges::any_of(map->units.units, [](const Unit& unit) {
		return unit.id == "sloc";
	});
	const auto try_pick_unit_candidates = [&]() {
		bool picked_any_unit = false;
		for (std::size_t i = 0; i < map->units.units.size(); ++i) {
			const auto& unit = map->units.units[i];
			const bool is_start_location = unit.id == "sloc";
			std::shared_ptr<SkinnedMesh> pick_mesh = unit.mesh;
			SkeletalModelInstance pick_skeleton = unit.skeleton;
			if (is_start_location) {
				if (!ensure_click_helper_mesh()) {
					continue;
				}
				pick_mesh = std::static_pointer_cast<SkinnedMesh>(click_helper_mesh);
				pick_skeleton = make_start_location_marker_skeleton(pick_mesh, unit);
			} else if (!unit.mesh || !unit.mesh->has_mesh) {
				continue;
			}

			constexpr std::array offsets {
				glm::vec2(0.0f, 0.0f),
				glm::vec2(4.0f, 0.0f),
				glm::vec2(-4.0f, 0.0f),
				glm::vec2(0.0f, 4.0f),
				glm::vec2(0.0f, -4.0f),
				glm::vec2(8.0f, 0.0f),
				glm::vec2(-8.0f, 0.0f),
				glm::vec2(0.0f, 8.0f),
				glm::vec2(0.0f, -8.0f),
			};
			std::vector<glm::vec2> candidate_points = projected_model_points(pick_mesh, pick_skeleton, width(), height());
			std::vector<glm::vec2> grid_points = projected_model_grid_points(pick_mesh, pick_skeleton, width(), height());
			candidate_points.insert(candidate_points.end(), grid_points.begin(), grid_points.end());
			bool picked_current_unit = false;
			for (const glm::vec2 base_mouse_position : candidate_points) {
				for (const glm::vec2 offset : offsets) {
					const glm::vec2 mouse_position = base_mouse_position + offset;
					if (mouse_position.x < 0.0f || mouse_position.x >= static_cast<float>(width()) || mouse_position.y < 0.0f
						|| mouse_position.y >= static_cast<float>(height())) {
						continue;
					}

					attempted_unit_pick = true;
					attempted_start_location_pick = attempted_start_location_pick || is_start_location;
					const auto picked = pick_unit_id(mouse_position);
					if (picked && *picked == i) {
						std::println("Smoke picking selected unit {} ({}) at {}, {}.", i, unit.id, mouse_position.x, mouse_position.y);
						if (is_start_location) {
							start_location_pick_ok = true;
						} else {
							picked_any_unit = true;
						}
						if (picked_any_unit && (!has_start_location || start_location_pick_ok)) {
							return true;
						}
						picked_current_unit = true;
						break;
					}
				}
				if (picked_current_unit) {
					break;
				}
			}
		}
		return picked_any_unit || start_location_pick_ok;
	};

	const auto try_pick_doodad_candidates = [&]() {
		for (std::size_t i = 0; i < map->doodads.doodads.size(); ++i) {
			const auto& doodad = map->doodads.doodads[i];
			if (!doodad.mesh || !doodad.mesh->has_mesh) {
				continue;
			}

			std::vector<glm::vec2> candidate_points = projected_model_points(doodad.mesh, doodad.skeleton, width(), height());
			std::vector<glm::vec2> grid_points = projected_model_grid_points(doodad.mesh, doodad.skeleton, width(), height());
			candidate_points.insert(candidate_points.end(), grid_points.begin(), grid_points.end());
			for (const glm::vec2 mouse_position : candidate_points) {
				if (mouse_position.x < 0.0f || mouse_position.x >= static_cast<float>(width()) || mouse_position.y < 0.0f
					|| mouse_position.y >= static_cast<float>(height())) {
					continue;
				}

				attempted_doodad_pick = true;
				const auto picked = pick_doodad_id(mouse_position);
				if (picked && *picked == i) {
					std::println("Smoke picking selected doodad {} ({}) at {}, {}.", i, doodad.id, mouse_position.x, mouse_position.y);
					return true;
				}
			}
		}
		return false;
	};

	const auto focus_first_start_location = [&]() {
		for (const auto& unit : map->units.units) {
			if (unit.id == "sloc") {
				if (!ensure_click_helper_mesh()) {
					continue;
				}
				const auto marker_mesh = std::static_pointer_cast<SkinnedMesh>(click_helper_mesh);
				const SkeletalModelInstance marker_skeleton = make_start_location_marker_skeleton(marker_mesh, unit);
				if (const auto center = model_world_center(marker_mesh, marker_skeleton)) {
					camera.position = *center;
					return true;
				}
				continue;
			}
		}
		return false;
	};

	const auto focus_first_pickable_unit = [&]() {
		for (const auto& unit : map->units.units) {
			if (unit.id == "sloc") {
				continue;
			}
			if (const auto center = model_world_center(unit.mesh, unit.skeleton)) {
				camera.position = *center;
				return true;
			}
		}
		return false;
	};

	const auto focus_first_pickable_doodad = [&]() {
		for (const auto& doodad : map->doodads.doodads) {
			if (const auto center = model_world_center(doodad.mesh, doodad.skeleton)) {
				camera.position = *center;
				return true;
			}
		}

		return false;
	};

	const auto append_and_focus_smoke_unit_probe = [&]() {
		if (!ensure_click_helper_mesh()) {
			return false;
		}

		const auto smoke_mesh = std::static_pointer_cast<SkinnedMesh>(click_helper_mesh);
		const auto unit_index = append_smoke_pick_unit(smoke_mesh);
		if (!unit_index) {
			return false;
		}

		camera.position = map->units.units[*unit_index].position;
		std::println("Smoke picking added a transient unit probe because the map had no visible pickable model.");
		return true;
	};

	const auto append_and_focus_smoke_doodad_probe = [&]() {
		if (!ensure_click_helper_mesh()) {
			return false;
		}

		const auto smoke_mesh = std::static_pointer_cast<SkinnedMesh>(click_helper_mesh);
		const auto doodad_index = append_smoke_pick_doodad(smoke_mesh);
		if (!doodad_index) {
			return false;
		}

		camera.position = map->doodads.doodads[*doodad_index].position;
		std::println("Smoke picking added a transient doodad probe because the map had no visible pickable doodad.");
		return true;
	};

	const auto append_and_focus_smoke_click_helper_probe = [&]() -> std::optional<std::size_t> {
		if (!ensure_click_helper_mesh()) {
			return std::nullopt;
		}

		const glm::vec2 terrain_center(
			static_cast<float>(map->terrain.width) * 0.5f,
			static_cast<float>(map->terrain.height) * 0.5f
		);
		const float terrain_offset = std::clamp(
			static_cast<float>(std::min(map->terrain.width, map->terrain.height)) * 0.15f,
			4.0f,
			12.0f
		);
		const glm::vec2 helper_position(
			std::clamp(terrain_center.x + terrain_offset, 1.0f, static_cast<float>(std::max(1, map->terrain.width - 2))),
			std::clamp(terrain_center.y + terrain_offset, 1.0f, static_cast<float>(std::max(1, map->terrain.height - 2)))
		);
		const auto doodad_index = append_smoke_click_helper_doodad(helper_position, true);
		if (!doodad_index) {
			return std::nullopt;
		}

		camera.position = map->doodads.doodads[*doodad_index].position;
		std::println("Smoke picking added a transient helper-only doodad probe.");
		return doodad_index;
	};

	const auto try_pick_click_helper_candidate = [&](const std::size_t doodad_index) {
		if (doodad_index >= map->doodads.doodads.size() || !ensure_click_helper_mesh()) {
			return false;
		}

		const auto click_helper = std::static_pointer_cast<SkinnedMesh>(click_helper_mesh);
		auto& doodad = map->doodads.doodads[doodad_index];
		SkeletalModelInstance helper_skeleton = make_click_helper_skeleton(click_helper, doodad.skeleton.matrix);
		std::vector<glm::vec2> candidate_points = projected_model_points(click_helper, helper_skeleton, width(), height());
		std::vector<glm::vec2> helper_only_points = projected_click_helper_only_points(doodad, click_helper, width(), height());
		candidate_points.insert(candidate_points.end(), helper_only_points.begin(), helper_only_points.end());
		std::vector<glm::vec2> helper_grid_points = projected_click_helper_grid_points(doodad, click_helper, width(), height());
		candidate_points.insert(candidate_points.end(), helper_grid_points.begin(), helper_grid_points.end());

		constexpr std::array offsets {
			glm::vec2(0.0f, 0.0f),
			glm::vec2(4.0f, 0.0f),
			glm::vec2(-4.0f, 0.0f),
			glm::vec2(0.0f, 4.0f),
			glm::vec2(0.0f, -4.0f),
			glm::vec2(8.0f, 0.0f),
			glm::vec2(-8.0f, 0.0f),
			glm::vec2(0.0f, 8.0f),
			glm::vec2(0.0f, -8.0f),
		};
		for (const glm::vec2 base_mouse_position : candidate_points) {
			for (const glm::vec2 offset : offsets) {
				const glm::vec2 mouse_position = base_mouse_position + offset;
				if (mouse_position.x < 0.0f || mouse_position.x >= static_cast<float>(width()) || mouse_position.y < 0.0f
					|| mouse_position.y >= static_cast<float>(height())) {
					continue;
				}

				attempted_click_helper_pick = true;
				const std::string original_id = doodad.id;
				const std::string original_skin_id = doodad.skin_id;
				doodad.id = "_smk";
				doodad.skin_id = "_smk";
				const auto picked_without_helper = pick_doodad_id(mouse_position);
				doodad.id = original_id;
				doodad.skin_id = original_skin_id;
				if (picked_without_helper && *picked_without_helper == doodad_index) {
					continue;
				}

				const auto picked = pick_doodad_id(mouse_position);
				if (picked && *picked == doodad_index) {
					std::println(
						"Smoke picking selected helper-enabled doodad {} ({}) through click-helper-only space at {}, {}.",
						doodad_index,
						doodad.id,
						mouse_position.x,
						mouse_position.y
					);
					return true;
				}
			}
		}
		return false;
	};

	const auto draw_and_install_picking = [&](const std::string_view failure_context) {
		if (!draw_frame()) {
			std::println("Smoke picking failed {}.", failure_context);
			return false;
		}
		install_picking_callbacks();
		return true;
	};

	if (!draw_frame()) {
		std::println("Smoke picking failed before a rendered frame was available.");
		restore_smoke_state();
		return false;
	}
	install_picking_callbacks();
	bool unit_pick_ok = try_pick_unit_candidates();
	bool doodad_pick_ok = try_pick_doodad_candidates();

	if (has_start_location && !start_location_pick_ok && focus_first_start_location()) {
		if (!draw_and_install_picking("after focusing the camera on a starting location")) {
			restore_smoke_state();
			return false;
		}
		unit_pick_ok = try_pick_unit_candidates();
	}
	if (!unit_pick_ok && focus_first_pickable_unit()) {
		if (!draw_and_install_picking("after focusing the camera on a pickable unit")) {
			restore_smoke_state();
			return false;
		}
		unit_pick_ok = try_pick_unit_candidates();
	}
	if (!unit_pick_ok && append_and_focus_smoke_unit_probe()) {
		if (!draw_and_install_picking("after staging a unit probe")) {
			restore_smoke_state();
			return false;
		}
		unit_pick_ok = try_pick_unit_candidates();
	}

	if (!doodad_pick_ok && focus_first_pickable_doodad()) {
		if (!draw_and_install_picking("after focusing the camera on a pickable doodad")) {
			restore_smoke_state();
			return false;
		}
		doodad_pick_ok = try_pick_doodad_candidates();
	}
	if (!doodad_pick_ok && append_and_focus_smoke_doodad_probe()) {
		if (!draw_and_install_picking("after staging a doodad probe")) {
			restore_smoke_state();
			return false;
		}
		doodad_pick_ok = try_pick_doodad_candidates();
	}

	bool click_helper_pick_ok = false;
	if (const auto helper_doodad_index = append_and_focus_smoke_click_helper_probe()) {
		if (!draw_and_install_picking("after staging a helper-enabled doodad")) {
			restore_smoke_state();
			return false;
		}
		click_helper_pick_ok = try_pick_click_helper_candidate(*helper_doodad_index);
	}

	const bool picking_ok = unit_pick_ok && (!has_start_location || start_location_pick_ok) && doodad_pick_ok && click_helper_pick_ok;
	if (!picking_ok) {
		if (!unit_pick_ok) {
			std::println(
				"Smoke picking did not prove unit picking ({}).",
				attempted_unit_pick ? "projected unit candidates missed" : "no projected unit candidates"
			);
		}
		if (has_start_location && !start_location_pick_ok) {
			std::println(
				"Smoke picking did not prove starting-location picking ({}).",
				attempted_start_location_pick ? "projected starting-location candidates missed" : "no projected starting-location candidates"
			);
		}
		if (!doodad_pick_ok) {
			std::println(
				"Smoke picking did not prove doodad picking ({}).",
				attempted_doodad_pick ? "projected doodad candidates missed" : "no projected doodad candidates"
			);
		}
		if (!click_helper_pick_ok) {
			std::println(
				"Smoke picking did not prove helper-enabled doodad picking ({}).",
				attempted_click_helper_pick ? "projected click-helper candidates missed" : "no projected click-helper-only candidates"
			);
		}
	}

	restore_smoke_state();
	return picking_ok;
}

hive::metal::SelectionOverlayCpuFrame MetalMapCanvas::make_selection_overlay_frame() {
	if (!map || !map->brush || !map->render_brush) {
		return {};
	}

	BrushOverlayFrame brush_frame;
	map->brush->append_selection_overlay(brush_frame);
	return hive::metal::make_selection_overlay_cpu_frame(brush_frame);
}

void MetalMapCanvas::install_picking_callbacks() {
	if (!map) {
		return;
	}

	map->render_manager->initialize_callbacks(
		[this](const Units&, const glm::vec2 mouse_position) -> std::optional<std::size_t> {
			return pick_unit_id(mouse_position);
		},
		[this](const Doodads&, const glm::vec2 mouse_position) -> std::optional<std::size_t> {
			return pick_doodad_id(mouse_position);
		}
	);
}

std::optional<std::size_t> MetalMapCanvas::pick_unit_id(const glm::vec2 mouse_position) {
	if (!map || !map->loaded || !ensure_surface()) {
		return std::nullopt;
	}

	hive::metal::FrameContext frame_context {
		.device = surface.device_opaque(),
		.command_queue = surface.command_queue_opaque(),
		.drawable_width = surface.drawable_width(),
		.drawable_height = surface.drawable_height(),
		.scale = surface.scale(),
	};
	const hive::metal::CameraUniforms camera_uniforms {
		.projection_view = hive::metal::opengl_clip_depth_to_metal(camera.projection_view),
	};
	hive::metal::ModelCpuFrame picking_frame;
	if (ensure_click_helper_mesh()) {
		const auto start_location_marker = std::static_pointer_cast<SkinnedMesh>(click_helper_mesh);
		picking_frame = hive::metal::make_unit_picking_cpu_frame(
			*map,
			camera,
			start_location_marker,
			[](const std::shared_ptr<SkinnedMesh>& marker_mesh, const Unit& unit) {
				return make_start_location_marker_skeleton(marker_mesh, unit);
			}
		);
	} else {
		picking_frame = hive::metal::make_unit_picking_cpu_frame(*map, camera);
	}
	hive::metal::finalize_model_cpu_frame(picking_frame);
	const auto picked = map_renderer->pick_model_id(
		frame_context,
		picking_frame,
		camera_uniforms,
		mouse_position
	);
	if (!picked && !map_renderer->last_error().empty()) {
		std::println("Metal unit picking error: {}", map_renderer->last_error());
	}
	return picked ? std::optional<std::size_t>(*picked) : std::nullopt;
}

std::optional<std::size_t> MetalMapCanvas::pick_doodad_id(const glm::vec2 mouse_position) {
	if (!map || !map->loaded || !ensure_surface()) {
		return std::nullopt;
	}

	hive::metal::FrameContext frame_context {
		.device = surface.device_opaque(),
		.command_queue = surface.command_queue_opaque(),
		.drawable_width = surface.drawable_width(),
		.drawable_height = surface.drawable_height(),
		.scale = surface.scale(),
	};
	const hive::metal::CameraUniforms camera_uniforms {
		.projection_view = hive::metal::opengl_clip_depth_to_metal(camera.projection_view),
	};

	const double scale = frame_context.scale > 0.0 ? frame_context.scale : 1.0;
	const glm::vec3 window_position(
		static_cast<float>(static_cast<double>(mouse_position.x) * scale),
		static_cast<float>(static_cast<double>(frame_context.drawable_height) - static_cast<double>(mouse_position.y) * scale),
		1.0f
	);
	const glm::vec3 world_position = glm::unProject(
		window_position,
		camera.view,
		camera.projection,
		glm::vec4(0.0f, 0.0f, static_cast<float>(frame_context.drawable_width), static_cast<float>(frame_context.drawable_height))
	);
	const glm::vec3 ray_origin = camera.position - camera.direction * camera.distance;
	const glm::vec3 ray_direction = glm::normalize(world_position - ray_origin);

	hive::metal::ModelCpuFrame picking_frame;
	std::unordered_map<const void*, std::uint32_t> mesh_indices;
	const bool click_helper_available = ensure_click_helper_mesh();
	const auto click_helper = click_helper_available ? std::static_pointer_cast<SkinnedMesh>(click_helper_mesh) : std::shared_ptr<SkinnedMesh> {};
	for (std::size_t i = 0; i < map->doodads.doodads.size(); ++i) {
		const auto& doodad = map->doodads.doodads[i];
		const bool use_click_helper = click_helper_available && click_helper && click_helper->mdx && doodad_uses_click_helper(doodad);
		const bool has_doodad_mesh = doodad.mesh && doodad.mesh->has_mesh && doodad.mesh->mdx;
		const bool has_meshless_particle_bounds = doodad.mesh && !doodad.mesh->has_mesh && doodad.mesh->mdx
			&& (!doodad.mesh->mdx->emitters1.empty() || !doodad.mesh->mdx->emitters2.empty() || !doodad.mesh->mdx->ribbons.empty());
		const bool has_pe1_model_spawns = has_pickable_particle_emitter1_model_spawns(doodad.mesh, doodad.skeleton);
		if (!has_doodad_mesh && !has_meshless_particle_bounds && !use_click_helper && !has_pe1_model_spawns) {
			continue;
		}

		glm::vec3 local_min(0.0f);
		glm::vec3 local_max(0.0f);
		bool has_bounds = false;
		if (has_doodad_mesh || has_meshless_particle_bounds) {
			const bool has_valid_sequence = doodad.skeleton.sequence_index >= 0
				&& static_cast<std::size_t>(doodad.skeleton.sequence_index) < doodad.mesh->mdx->sequences.size();
			const auto& extent =
				has_valid_sequence ? doodad.mesh->mdx->sequences[doodad.skeleton.sequence_index].extent : doodad.mesh->mdx->extent;
			if (hive::metal::detail::valid_bounds_extent(extent.minimum, extent.maximum)) {
				local_min = extent.minimum;
				local_max = extent.maximum;
				has_bounds = true;
			}
		}
		if (use_click_helper) {
			if (has_bounds) {
				local_min = glm::min(local_min, click_helper->mdx->extent.minimum);
				local_max = glm::max(local_max, click_helper->mdx->extent.maximum);
			} else {
				local_min = click_helper->mdx->extent.minimum;
				local_max = click_helper->mdx->extent.maximum;
				has_bounds = true;
			}
		}
		if (!has_bounds && !has_pe1_model_spawns) {
			continue;
		}

		if (has_bounds) {
			glm::vec3 world_min;
			glm::vec3 world_max;
			transform_aabb_non_uniform(local_min, local_max, world_min, world_max, doodad.skeleton.matrix);
			if (!intersect_aabb(world_min, world_max, ray_origin, ray_direction) && !has_pe1_model_spawns) {
				continue;
			}
		}

		const std::uint32_t pick_id = static_cast<std::uint32_t>(i + 1);
		if (has_doodad_mesh) {
			hive::metal::detail::append_model_instance(
				picking_frame,
				mesh_indices,
				camera,
				doodad.mesh,
				doodad.skeleton,
				doodad.color,
				0,
				pick_id
			);
		}
		if (has_pe1_model_spawns) {
			hive::metal::detail::append_particle_emitter1_model_instances(
				picking_frame,
				mesh_indices,
				camera,
				doodad.mesh,
				doodad.skeleton,
				doodad.color,
				0,
				false,
				pick_id
			);
		}
		if (has_meshless_particle_bounds) {
			hive::metal::detail::append_model_bounds_picking_instance(
				picking_frame,
				mesh_indices,
				camera,
				doodad.mesh,
				doodad.skeleton,
				0,
				pick_id
			);
		}

		if (use_click_helper) {
			SkeletalModelInstance helper_skeleton = make_click_helper_skeleton(click_helper, doodad.skeleton.matrix);
			hive::metal::detail::append_model_instance(
				picking_frame,
				mesh_indices,
				camera,
				click_helper.get(),
				helper_skeleton,
				glm::vec3(1.0f),
				0,
				pick_id
			);
		}
	}
	hive::metal::finalize_model_cpu_frame(picking_frame);
	const auto picked = map_renderer->pick_model_id(
		frame_context,
		picking_frame,
		camera_uniforms,
		mouse_position
	);
	if (!picked && !map_renderer->last_error().empty()) {
		std::println("Metal doodad picking error: {}", map_renderer->last_error());
	}
	return picked ? std::optional<std::size_t>(*picked) : std::nullopt;
}

void MetalMapCanvas::showEvent(QShowEvent* event) {
	QWidget::showEvent(event);
	ensure_surface();
	layout_debug_overlay();
}

void MetalMapCanvas::resizeEvent(QResizeEvent* event) {
	QWidget::resizeEvent(event);
	update_camera_aspect_ratio();

	if (ensure_surface()) {
		surface.resize(width(), height(), metal_map_canvas_surface_scale(this));
	}
	layout_debug_overlay();
}

void MetalMapCanvas::paintEvent(QPaintEvent*) {
	draw_frame();
}

void MetalMapCanvas::keyPressEvent(QKeyEvent* event) {
	input_handler.keys_pressed.emplace(event->key());
	if (map && map->brush) {
		map->brush->key_press_event(event);
	}
	QWidget::keyPressEvent(event);
}

void MetalMapCanvas::keyReleaseEvent(QKeyEvent* event) {
	input_handler.keys_pressed.erase(event->key());
	if (map && map->brush) {
		map->brush->key_release_event(event);
	}
	QWidget::keyReleaseEvent(event);
}

void MetalMapCanvas::mouseMoveEvent(QMouseEvent* event) {
	input_handler.mouse_move_event(event);
	camera.mouse_move_event(event);
	if (map && map->brush) {
		map->brush->mouse_move_event(event, delta);
	}
}

void MetalMapCanvas::mousePressEvent(QMouseEvent* event) {
	camera.mouse_press_event(event);
	if (map && map->brush) {
		install_picking_callbacks();
		map->brush->mouse_press_event(event, delta);
	}
}

void MetalMapCanvas::mouseReleaseEvent(QMouseEvent* event) {
	camera.mouse_release_event(event);
	if (map && map->brush) {
		map->brush->mouse_release_event(event);
	}
}

void MetalMapCanvas::wheelEvent(QWheelEvent* event) {
	camera.mouse_scroll_event(event);
}
#else
OpenGLMapCanvas::OpenGLMapCanvas(QWidget* parent) : QOpenGLWidget(parent) {
	setMouseTracking(true);
	setFocus();
	setFocusPolicy(Qt::WheelFocus);

	connect(this, &QOpenGLWidget::frameSwapped, [&]() {
		update();
	});
}

void OpenGLMapCanvas::initializeGL() {
	const QOpenGLContext* context = QOpenGLContext::currentContext();
	if (!context) {
		std::println("OpenGL renderer startup failed: no current Qt OpenGL context.");
		std::exit(EXIT_FAILURE);
	}
	const QSurfaceFormat actual_format = context->format();
	if (actual_format.majorVersion() < 4 || (actual_format.majorVersion() == 4 && actual_format.minorVersion() < 5)) {
		std::println(
			"OpenGL renderer startup failed: HiveWE's legacy renderer requires OpenGL 4.5 core, but Qt created an OpenGL {}.{} context. On macOS this fallback is expected to be compile-only because Apple's OpenGL runtime exposes at most OpenGL 4.1.",
			actual_format.majorVersion(),
			actual_format.minorVersion()
		);
		std::exit(EXIT_FAILURE);
	}

	if (!gladLoadGL()) {
		std::println("Something went wrong initializing GLAD");
		exit(-1);
	}
	std::println("OpenGL {}.{}", GLVersion.major, GLVersion.minor);

	glEnable(GL_DEBUG_OUTPUT);
	glEnable(GL_DEBUG_OUTPUT_SYNCHRONOUS);
	glDebugMessageCallback(reinterpret_cast<GLDEBUGPROC>(gl_debug_output), nullptr);
	glDebugMessageControl(GL_DONT_CARE, GL_DONT_CARE, GL_DEBUG_SEVERITY_NOTIFICATION, 0, nullptr, false);
	glDebugMessageControl(GL_DONT_CARE, GL_DONT_CARE, GL_DEBUG_SEVERITY_LOW, 0, nullptr, true);
	glDebugMessageControl(GL_DONT_CARE, GL_DONT_CARE, GL_DEBUG_SEVERITY_MEDIUM, 0, nullptr, true);
	glDebugMessageControl(GL_DONT_CARE, GL_DONT_CARE, GL_DEBUG_SEVERITY_HIGH, 0, nullptr, true);

	glEnable(GL_DEPTH_TEST);
	glEnable(GL_CULL_FACE);
	glDepthFunc(GL_LEQUAL);
	glEnable(GL_BLEND);
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

	glClearColor(0, 0, 0, 1);

	glGenVertexArrays(1, &vao);
	glBindVertexArray(vao);

	int extension_count;
	glGetIntegerv(GL_NUM_EXTENSIONS, &extension_count);

	opengl_shapes.init();
}

void OpenGLMapCanvas::resizeGL(const int w, const int h) {
	glViewport(0, 0, w, h);

	delta = elapsed_timer.nsecsElapsed() / 1'000'000'000.0;
	camera.aspect_ratio = static_cast<double>(w) / h;

	if (!map || !map->loaded) {
		return;
	}
	camera.update(delta);
	map->render_manager->resize_framebuffers(w, h);
}

void OpenGLMapCanvas::paintGL() {
	delta = map_canvas_frame_delta(elapsed_timer);

	if (!map) {
		return;
	}

	map->update(delta, width(), height());

	//glEnable(GL_FRAMEBUFFER_SRGB);
	glEnable(GL_DEPTH_TEST);
	glDepthMask(true);
	glClearColor(0.f, 0.f, 0.f, 1.f);
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

	glBindVertexArray(vao);
	map->render();

	glBindVertexArray(0);

	if (map->render_debug && map->terrain.width > 0 && map->terrain.height > 0) {
		QPainter p(this);
		p.setPen(QColor(Qt::GlobalColor::white));
		p.setFont(QFont("Arial", 10, 100, false));
		QFont font("Consolas");
		font.setStyleHint(QFont::Monospace);
		p.setFont(font);

		// Rendering time
		static std::vector<double> frametimes;
		frametimes.push_back(delta);
		if (frametimes.size() > 60) {
			frametimes.erase(frametimes.begin());
		}
		float average_frametime = std::accumulate(frametimes.begin(), frametimes.end(), 0.f) / frametimes.size();
		p.drawText(10, 20, QString::fromStdString(std::format("Total time: {:.2f}ms", average_frametime * 1000.0)));

		// General info
		auto fmt_vec3 = [](const char* label, glm::vec3 v) {
			return std::format("{:<20} X:{:>6.3f}  Y:{:>6.3f}  Z:{:>6.3f}", label, v.x, v.y, v.z);
		};

		auto fmt_vec2 = [](const char* label, glm::vec2 v) {
			return std::format("{:<20} X:{:>6.3f}  Y:{:>6.3f}", label, v.x, v.y);
		};

		p.drawText(175, 20, QString::fromStdString(fmt_vec3("Mouse World Position", input_handler.mouse_world)));
		p.drawText(175, 35, QString::fromStdString(fmt_vec3("Camera Position", camera.position)));

		if (map->brush) {
			p.drawText(175, 50, QString::fromStdString(fmt_vec2("Brush Grid Position", map->brush->get_position())));
		}

		p.drawText(175, 65, QString::fromStdString(std::format("Camera Horizontal Angle: {:.4f}", camera.horizontal_angle)));
		p.drawText(175, 80, QString::fromStdString(std::format("Camera Vertical Angle: {:.4f}", camera.vertical_angle)));

		const int terrain_x = std::clamp(static_cast<int>(input_handler.mouse_world.x), 0, std::max(0, map->terrain.width - 1));
		const int terrain_y = std::clamp(static_cast<int>(input_handler.mouse_world.y), 0, std::max(0, map->terrain.height - 1));
		const auto corner = map->terrain.get_corner(terrain_x, terrain_y);
		p.drawText(550, 20, QString::fromStdString(std::format("Tile info ({}, {})", terrain_x, terrain_y)));
		p.drawText(550, 35, QString::fromStdString(std::format("Cliff: {}", corner.cliff)));
		p.drawText(550, 50, QString::fromStdString(std::format("Blight: {}", corner.blight)));
		p.drawText(550, 65, QString::fromStdString(std::format("Boundary: {}", corner.boundary)));
		p.drawText(550, 80, QString::fromStdString(std::format("Cliff texture: {}", corner.cliff_texture)));
		p.drawText(550, 95, QString::fromStdString(std::format("Cliff variation: {}", corner.cliff_variation)));
		p.drawText(550, 110, QString::fromStdString(std::format("Ground texture: {}", corner.ground_texture)));
		p.drawText(550, 125, QString::fromStdString(std::format("Ground variation: {}", corner.ground_variation)));
		p.drawText(550, 140, QString::fromStdString(std::format("Height: {}", corner.height)));
		p.drawText(550, 155, QString::fromStdString(std::format("Layer height: {}", corner.layer_height)));
		p.drawText(550, 170, QString::fromStdString(std::format("Map edge: {}", corner.map_edge)));
		p.drawText(550, 185, QString::fromStdString(std::format("Ramp: {}", corner.ramp)));
		p.drawText(550, 200, QString::fromStdString(std::format("Romp: {}", corner.romp)));
		p.drawText(550, 215, QString::fromStdString(std::format("Special doodad: {}", corner.special_doodad)));
		p.drawText(550, 230, QString::fromStdString(std::format("Water: {}", corner.water)));
		p.drawText(550, 245, QString::fromStdString(std::format("Water height: {}", corner.water_height)));

		p.end();

		// Set changed state back
		glEnable(GL_DEPTH_TEST);
		glDepthFunc(GL_LEQUAL);
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
	}

	FrameMark;
}

bool OpenGLMapCanvas::smoke_render_modes(const QString& artifact_directory) {
	if (!map || !map->loaded) {
		std::println("Smoke render modes requested before the startup map was loaded.");
		return false;
	}

	const bool previous_smoke_capture_mode = map_canvas_smoke_capture_mode;
	map_canvas_smoke_capture_mode = true;
	makeCurrent();
	const bool ok = run_smoke_render_modes(
		[this](std::vector<std::uint8_t>& pixels, int& width, int& height) {
			paintGL();
			const QImage image = grabFramebuffer();
			if (image.isNull()) {
				return false;
			}
			pixels = image_pixels_rgba(image);
			width = image.width();
			height = image.height();
			return true;
		},
		artifact_directory,
		SmokeArtifactPixelFormat::Rgba
	);
	map_canvas_smoke_capture_mode = previous_smoke_capture_mode;
	return ok;
}

bool OpenGLMapCanvas::smoke_pick_rendered_objects() {
	if (!map || !map->loaded) {
		std::println("Smoke picking requested before the startup map was loaded.");
		return false;
	}

	const Camera camera_snapshot = camera;
	const std::size_t unit_count_snapshot = map->units.units.size();
	const std::size_t doodad_count_snapshot = map->doodads.doodads.size();
	const int unit_auto_increment_snapshot = Unit::auto_increment;
	const int doodad_auto_increment_snapshot = Doodad::auto_increment;
	const auto restore_smoke_state = [&]() {
		camera = camera_snapshot;
		map->units.units.resize(unit_count_snapshot);
		map->doodads.doodads.resize(doodad_count_snapshot);
		Unit::auto_increment = unit_auto_increment_snapshot;
		Doodad::auto_increment = doodad_auto_increment_snapshot;
	};

	bool attempted_unit_pick = false;
	bool attempted_doodad_pick = false;
	bool attempted_click_helper_pick = false;
	bool attempted_start_location_pick = false;
	bool start_location_pick_ok = false;
	const bool has_start_location = std::ranges::any_of(map->units.units, [](const Unit& unit) {
		return unit.id == "sloc";
	});
	const auto load_smoke_probe_mesh = [&]() -> std::shared_ptr<SkinnedMesh> {
		auto result = resource_manager.load<SkinnedMesh>("Objects/InvalidObject/InvalidObject.mdx", "", std::nullopt);
		if (!result || !*result) {
			if (result) {
				std::println("Smoke picking probe mesh load failed: empty mesh");
			} else {
				std::println("Smoke picking probe mesh load failed: {}", result.error());
			}
			return {};
		}
		return *result;
	};

	const auto try_pick_unit_candidates = [&]() {
		bool picked_any_unit = false;
		for (std::size_t i = 0; i < map->units.units.size(); ++i) {
			const auto& unit = map->units.units[i];
			const bool is_start_location = unit.id == "sloc";
			std::shared_ptr<SkinnedMesh> pick_mesh = unit.mesh;
			SkeletalModelInstance pick_skeleton = unit.skeleton;
			if (is_start_location) {
				pick_mesh = load_smoke_probe_mesh();
				if (!pick_mesh || !pick_mesh->has_mesh || !pick_mesh->mdx) {
					continue;
				}
				pick_skeleton = make_start_location_marker_skeleton(pick_mesh, unit);
			} else if (!unit.mesh || !unit.mesh->has_mesh) {
				continue;
			}

			constexpr std::array offsets {
				glm::vec2(0.0f, 0.0f),
				glm::vec2(4.0f, 0.0f),
				glm::vec2(-4.0f, 0.0f),
				glm::vec2(0.0f, 4.0f),
				glm::vec2(0.0f, -4.0f),
				glm::vec2(8.0f, 0.0f),
				glm::vec2(-8.0f, 0.0f),
				glm::vec2(0.0f, 8.0f),
				glm::vec2(0.0f, -8.0f),
			};
			std::vector<glm::vec2> candidate_points = projected_model_points(pick_mesh, pick_skeleton, width(), height());
			std::vector<glm::vec2> grid_points = projected_model_grid_points(pick_mesh, pick_skeleton, width(), height());
			candidate_points.insert(candidate_points.end(), grid_points.begin(), grid_points.end());
			bool picked_current_unit = false;
			for (const glm::vec2 base_mouse_position : candidate_points) {
				for (const glm::vec2 offset : offsets) {
					const glm::vec2 mouse_position = base_mouse_position + offset;
					if (mouse_position.x < 0.0f || mouse_position.x >= static_cast<float>(width()) || mouse_position.y < 0.0f
						|| mouse_position.y >= static_cast<float>(height())) {
						continue;
					}

					attempted_unit_pick = true;
					attempted_start_location_pick = attempted_start_location_pick || is_start_location;
					input_handler.mouse = mouse_position;
					const auto picked = map->render_manager->pick_unit_id_under_mouse(map->units, mouse_position);
					if (picked && *picked == i) {
						std::println("Smoke picking selected unit {} ({}) at {}, {}.", i, unit.id, mouse_position.x, mouse_position.y);
						if (is_start_location) {
							start_location_pick_ok = true;
						} else {
							picked_any_unit = true;
						}
						if (picked_any_unit && (!has_start_location || start_location_pick_ok)) {
							return true;
						}
						picked_current_unit = true;
						break;
					}
				}
				if (picked_current_unit) {
					break;
				}
			}
		}
		return picked_any_unit || start_location_pick_ok;
	};

	const auto try_pick_doodad_candidates = [&]() {
		for (std::size_t i = 0; i < map->doodads.doodads.size(); ++i) {
			const auto& doodad = map->doodads.doodads[i];
			if (!doodad.mesh || !doodad.mesh->has_mesh) {
				continue;
			}

			for (const glm::vec2 mouse_position : projected_model_points(doodad.mesh, doodad.skeleton, width(), height())) {
				attempted_doodad_pick = true;
				input_handler.mouse = mouse_position;
				const auto picked = map->render_manager->pick_doodad_id_under_mouse(map->doodads, mouse_position);
				if (picked && *picked == i) {
					std::println("Smoke picking selected doodad {} ({}) at {}, {}.", i, doodad.id, mouse_position.x, mouse_position.y);
					return true;
				}
			}
		}
		return false;
	};

	const auto focus_first_start_location = [&]() {
		for (const auto& unit : map->units.units) {
			if (unit.id == "sloc") {
				const auto marker_mesh = load_smoke_probe_mesh();
				if (!marker_mesh || !marker_mesh->has_mesh || !marker_mesh->mdx) {
					continue;
				}
				const SkeletalModelInstance marker_skeleton = make_start_location_marker_skeleton(marker_mesh, unit);
				if (const auto center = model_world_center(marker_mesh, marker_skeleton)) {
					camera.position = *center;
					return true;
				}
				continue;
			}
		}
		return false;
	};

	const auto focus_first_pickable_unit = [&]() {
		for (const auto& unit : map->units.units) {
			if (unit.id == "sloc") {
				continue;
			}
			if (const auto center = model_world_center(unit.mesh, unit.skeleton)) {
				camera.position = *center;
				return true;
			}
		}
		return false;
	};

	const auto focus_first_pickable_doodad = [&]() {
		for (const auto& doodad : map->doodads.doodads) {
			if (const auto center = model_world_center(doodad.mesh, doodad.skeleton)) {
				camera.position = *center;
				return true;
			}
		}

		return false;
	};

	const auto append_and_focus_smoke_unit_probe = [&]() {
		const auto smoke_mesh = load_smoke_probe_mesh();
		if (!smoke_mesh) {
			return false;
		}

		const auto unit_index = append_smoke_pick_unit(smoke_mesh);
		if (!unit_index) {
			return false;
		}

		camera.position = map->units.units[*unit_index].position;
		std::println("Smoke picking added a transient unit probe because the map had no visible pickable model.");
		return true;
	};

	const auto append_and_focus_smoke_doodad_probe = [&]() {
		const auto smoke_mesh = load_smoke_probe_mesh();
		if (!smoke_mesh) {
			return false;
		}

		const auto doodad_index = append_smoke_pick_doodad(smoke_mesh);
		if (!doodad_index) {
			return false;
		}

		camera.position = map->doodads.doodads[*doodad_index].position;
		std::println("Smoke picking added a transient doodad probe because the map had no visible pickable doodad.");
		return true;
	};

	const auto append_and_focus_smoke_click_helper_probe = [&]() -> std::optional<std::size_t> {
		if (!load_smoke_probe_mesh()) {
			return std::nullopt;
		}

		const glm::vec2 terrain_center(
			static_cast<float>(map->terrain.width) * 0.5f,
			static_cast<float>(map->terrain.height) * 0.5f
		);
		const float terrain_offset = std::clamp(
			static_cast<float>(std::min(map->terrain.width, map->terrain.height)) * 0.15f,
			4.0f,
			12.0f
		);
		const glm::vec2 helper_position(
			std::clamp(terrain_center.x + terrain_offset, 1.0f, static_cast<float>(std::max(1, map->terrain.width - 2))),
			std::clamp(terrain_center.y + terrain_offset, 1.0f, static_cast<float>(std::max(1, map->terrain.height - 2)))
		);
		const auto doodad_index = append_smoke_click_helper_doodad(helper_position, true);
		if (!doodad_index) {
			return std::nullopt;
		}

		camera.position = map->doodads.doodads[*doodad_index].position;
		std::println("Smoke picking added a transient helper-only doodad probe.");
		return doodad_index;
	};

	const auto try_pick_click_helper_candidate = [&](const std::size_t doodad_index) {
		if (doodad_index >= map->doodads.doodads.size()) {
			return false;
		}

		const auto click_helper = load_smoke_probe_mesh();
		if (!click_helper || !click_helper->has_mesh || !click_helper->mdx) {
			return false;
		}

		auto& doodad = map->doodads.doodads[doodad_index];
		SkeletalModelInstance helper_skeleton = make_click_helper_skeleton(click_helper, doodad.skeleton.matrix);
		std::vector<glm::vec2> candidate_points = projected_model_points(click_helper, helper_skeleton, width(), height());
		std::vector<glm::vec2> helper_only_points = projected_click_helper_only_points(doodad, click_helper, width(), height());
		candidate_points.insert(candidate_points.end(), helper_only_points.begin(), helper_only_points.end());
		std::vector<glm::vec2> helper_grid_points = projected_click_helper_grid_points(doodad, click_helper, width(), height());
		candidate_points.insert(candidate_points.end(), helper_grid_points.begin(), helper_grid_points.end());

		constexpr std::array offsets {
			glm::vec2(0.0f, 0.0f),
			glm::vec2(4.0f, 0.0f),
			glm::vec2(-4.0f, 0.0f),
			glm::vec2(0.0f, 4.0f),
			glm::vec2(0.0f, -4.0f),
			glm::vec2(8.0f, 0.0f),
			glm::vec2(-8.0f, 0.0f),
			glm::vec2(0.0f, 8.0f),
			glm::vec2(0.0f, -8.0f),
		};
		for (const glm::vec2 base_mouse_position : candidate_points) {
			for (const glm::vec2 offset : offsets) {
				const glm::vec2 mouse_position = base_mouse_position + offset;
				if (mouse_position.x < 0.0f || mouse_position.x >= static_cast<float>(width()) || mouse_position.y < 0.0f
					|| mouse_position.y >= static_cast<float>(height())) {
					continue;
				}

				attempted_click_helper_pick = true;
				const std::string original_id = doodad.id;
				const std::string original_skin_id = doodad.skin_id;
				doodad.id = "_smk";
				doodad.skin_id = "_smk";
				input_handler.mouse = mouse_position;
				const auto picked_without_helper = map->render_manager->pick_doodad_id_under_mouse(map->doodads, mouse_position);
				doodad.id = original_id;
				doodad.skin_id = original_skin_id;
				if (picked_without_helper && *picked_without_helper == doodad_index) {
					continue;
				}

				input_handler.mouse = mouse_position;
				const auto picked = map->render_manager->pick_doodad_id_under_mouse(map->doodads, mouse_position);
				if (picked && *picked == doodad_index) {
					std::println(
						"Smoke picking selected helper-enabled doodad {} ({}) through click-helper-only space at {}, {}.",
						doodad_index,
						doodad.id,
						mouse_position.x,
						mouse_position.y
					);
					return true;
				}
			}
		}
		return false;
	};

	makeCurrent();
	paintGL();
	bool unit_pick_ok = try_pick_unit_candidates();
	bool doodad_pick_ok = try_pick_doodad_candidates();

	if (has_start_location && !start_location_pick_ok && focus_first_start_location()) {
		paintGL();
		unit_pick_ok = try_pick_unit_candidates();
	}

	if (!unit_pick_ok && (focus_first_pickable_unit() || append_and_focus_smoke_unit_probe())) {
		paintGL();
		unit_pick_ok = try_pick_unit_candidates();
	}

	if (!doodad_pick_ok && (focus_first_pickable_doodad() || append_and_focus_smoke_doodad_probe())) {
		paintGL();
		doodad_pick_ok = try_pick_doodad_candidates();
	}

	bool click_helper_pick_ok = false;
	if (const auto helper_doodad_index = append_and_focus_smoke_click_helper_probe()) {
		paintGL();
		click_helper_pick_ok = try_pick_click_helper_candidate(*helper_doodad_index);
	}

	const bool picking_ok = unit_pick_ok && (!has_start_location || start_location_pick_ok) && doodad_pick_ok && click_helper_pick_ok;
	if (!picking_ok) {
		if (!unit_pick_ok) {
			std::println(
				"Smoke picking did not prove unit picking ({}).",
				attempted_unit_pick ? "projected unit candidates missed" : "no projected unit candidates"
			);
		}
		if (has_start_location && !start_location_pick_ok) {
			std::println(
				"Smoke picking did not prove starting-location picking ({}).",
				attempted_start_location_pick ? "projected starting-location candidates missed" : "no projected starting-location candidates"
			);
		}
		if (!doodad_pick_ok) {
			std::println(
				"Smoke picking did not prove doodad picking ({}).",
				attempted_doodad_pick ? "projected doodad candidates missed" : "no projected doodad candidates"
			);
		}
		if (!click_helper_pick_ok) {
			std::println(
				"Smoke picking did not prove helper-enabled doodad picking ({}).",
				attempted_click_helper_pick ? "projected click-helper candidates missed" : "no projected click-helper-only candidates"
			);
		}
	}

	restore_smoke_state();
	return picking_ok;
}

void OpenGLMapCanvas::keyPressEvent(QKeyEvent* event) {
	if (!map) {
		return;
	}

	input_handler.keys_pressed.emplace(event->key());

	if (map->brush) {
		map->brush->key_press_event(event);
	}
	QOpenGLWidget::keyPressEvent(event);
}

void OpenGLMapCanvas::keyReleaseEvent(QKeyEvent* event) {
	if (!map) {
		return;
	}

	input_handler.keys_pressed.erase(event->key());

	if (map->brush) {
		map->brush->key_release_event(event);
	}
	QOpenGLWidget::keyReleaseEvent(event);
}

void OpenGLMapCanvas::mouseMoveEvent(QMouseEvent* event) {
	if (!map) {
		return;
	}

	input_handler.mouse_move_event(event);
	camera.mouse_move_event(event);

	if (map->brush) {
		map->brush->mouse_move_event(event, delta);
	}
}

void OpenGLMapCanvas::mousePressEvent(QMouseEvent* event) {
	if (!map) {
		return;
	}

	camera.mouse_press_event(event);
	if (map->brush) {
		makeCurrent();
		map->brush->mouse_press_event(event, delta);
	}
}

void OpenGLMapCanvas::mouseReleaseEvent(QMouseEvent* event) {
	if (!map) {
		return;
	}
	camera.mouse_release_event(event);
	if (map->brush) {
		map->brush->mouse_release_event(event);
	}
}

void OpenGLMapCanvas::wheelEvent(QWheelEvent* event) {
	if (!map) {
		return;
	}

	camera.mouse_scroll_event(event);
}
#endif

#include "moc_map_canvas_widget.cpp"
