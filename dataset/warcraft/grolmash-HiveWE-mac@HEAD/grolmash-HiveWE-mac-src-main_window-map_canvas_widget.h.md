---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/main_window/map_canvas_widget.h"
ref: HEAD
---

#pragma once

#include "runtime_config.h"

#define HIVEWE_USE_METAL_MAP_CANVAS HIVEWE_USE_METAL_RUNTIME

#include <QElapsedTimer>
#include <QWidget>

#include <cstddef>
#include <cstdint>
#include <memory>
#include <optional>
#include <vector>

class QLabel;
class QString;
class Brush;

#if HIVEWE_USE_METAL_MAP_CANVAS
#include "rendering/metal_surface.h"

#include <QTimer>
#include <glm/glm.hpp>

namespace hive::metal {
class IMapFrameRenderer;
struct SelectionOverlayCpuFrame;
struct TerrainTextureCpuFrame;
} // namespace hive::metal

class MetalMapCanvas : public QWidget {
  public:
	explicit MetalMapCanvas(QWidget* parent);
	~MetalMapCanvas() override;

	void invalidate_map_resources();
	bool save_frame_png(const QString& file_path);
	bool smoke_render_modes(const QString& artifact_directory);
	bool smoke_pick_rendered_objects();

	QPaintEngine* paintEngine() const override {
		return nullptr;
	}

  protected:
	void showEvent(QShowEvent* event) override;
	void resizeEvent(QResizeEvent* event) override;
	void paintEvent(QPaintEvent* event) override;

	void keyPressEvent(QKeyEvent* event) override;
	void keyReleaseEvent(QKeyEvent* event) override;
	void mouseMoveEvent(QMouseEvent* event) override;
	void mousePressEvent(QMouseEvent* event) override;
	void mouseReleaseEvent(QMouseEvent* event) override;
	void wheelEvent(QWheelEvent* event) override;

  private:
	bool ensure_surface();
	void update_camera_aspect_ratio();
	bool draw_frame(std::vector<std::uint8_t>* captured_bgra = nullptr, int* captured_width = nullptr, int* captured_height = nullptr);
	void initialize_debug_overlay();
	void update_debug_overlay();
	void layout_debug_overlay();
	bool ensure_click_helper_mesh();
	void install_picking_callbacks();
	hive::metal::SelectionOverlayCpuFrame make_selection_overlay_frame();
	std::optional<std::size_t> pick_unit_id(glm::vec2 mouse_position);
	std::optional<std::size_t> pick_doodad_id(glm::vec2 mouse_position);

	QElapsedTimer elapsed_timer;
	QTimer frame_timer;
	QLabel* debug_overlay = nullptr;
	MetalSurface surface;
	std::unique_ptr<hive::metal::IMapFrameRenderer> map_renderer;
	std::unique_ptr<hive::metal::TerrainTextureCpuFrame> terrain_texture_frame;
	std::shared_ptr<void> click_helper_mesh;
	std::vector<double> debug_frametimes;
	const void* terrain_texture_owner = nullptr;
	std::uint64_t observed_terrain_texture_revision = 0;
	bool click_helper_load_failed = false;
	bool surface_initialized = false;
	double delta = 0.0;
};
#else
#include <glad/glad.h>
#ifndef QT_NO_OPENGL
#define HIVEWE_UNDEFINE_QT_NO_OPENGL_AFTER_QOPENGLWIDGET
#define QT_NO_OPENGL
#endif
#include <QOpenGLWidget>
#ifdef HIVEWE_UNDEFINE_QT_NO_OPENGL_AFTER_QOPENGLWIDGET
#undef QT_NO_OPENGL
#undef HIVEWE_UNDEFINE_QT_NO_OPENGL_AFTER_QOPENGLWIDGET
#endif

class OpenGLMapCanvas : public QOpenGLWidget {
  public:
	QElapsedTimer elapsed_timer;
	GLuint vao;

	double delta = 0.0;

	explicit OpenGLMapCanvas(QWidget* parent);
	~OpenGLMapCanvas() override = default;

	void initializeGL() override;
	void resizeGL(int w, int h) override;
	void paintGL() override;
	bool smoke_render_modes(const QString& artifact_directory);
	bool smoke_pick_rendered_objects();

	void keyPressEvent(QKeyEvent* event) override;
	void keyReleaseEvent(QKeyEvent* event) override;
	void mouseMoveEvent(QMouseEvent* event) override;
	void mousePressEvent(QMouseEvent* event) override;
	void mouseReleaseEvent(QMouseEvent* event) override;
	void wheelEvent(QWheelEvent* event) override;
};
#endif

class MapCanvasWidget : public QWidget {
	Q_OBJECT

public:
	explicit MapCanvasWidget(QWidget* parent);
	~MapCanvasWidget() override = default;

#if !HIVEWE_USE_METAL_MAP_CANVAS
	void makeCurrent();
	void doneCurrent();
#endif

	void invalidate_map_resources();
	bool save_rendered_frame(const QString& file_path);
	bool smoke_render_modes(const QString& artifact_directory);
	bool smoke_pick_rendered_objects();

  private:
#if HIVEWE_USE_METAL_MAP_CANVAS
	MetalMapCanvas* canvas = nullptr;
#else
	OpenGLMapCanvas* canvas = nullptr;
#endif
};
