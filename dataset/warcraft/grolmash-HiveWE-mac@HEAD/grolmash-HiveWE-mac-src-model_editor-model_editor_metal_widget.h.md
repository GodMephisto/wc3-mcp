---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/model_editor/model_editor_metal_widget.h"
ref: HEAD
---

#pragma once

#include "runtime_config.h"

#if HIVEWE_USE_METAL_RUNTIME

#include <cstdint>
#include <memory>
#include <optional>
#include <string>
#include <utility>
#include <vector>

#include <QElapsedTimer>
#include <QTimer>
#include <QWidget>

#include <glm/glm.hpp>

#include "model_editor/model_editor_camera.h"
#include "rendering/metal_model_frame_stats.h"
#include "rendering/metal_surface.h"

class QComboBox;
class QFrame;
class QLabel;

namespace hive::metal {
class IModelFrameRenderer;
} // namespace hive::metal

import MDX;
import SkeletalModelInstance;
import SkinnedMesh;

class ModelEditorMetalWidget : public QWidget {
	Q_OBJECT

  public:
	ModelEditorMetalWidget() = delete;
	explicit ModelEditorMetalWidget(
		QWidget* parent,
		std::shared_ptr<mdx::MDX> model,
		std::optional<std::pair<int, std::string>> replaceable_id_override = std::nullopt
	);
	~ModelEditorMetalWidget() override;

	struct SmokePreviewStats {
		std::string model_name;
		std::size_t geoset_count = 0;
		std::size_t source_texture_count = 0;
		int sequence_index = -1;
		bool valid_sequence = false;
		hive::metal::MetalModelFrameStats frame;
	};

	bool smoke_capture_preview(std::vector<std::uint8_t>& captured_bgra, int& captured_width, int& captured_height);
	[[nodiscard]] const SmokePreviewStats& smoke_preview_stats() const {
		return last_smoke_stats;
	}
	void set_smoke_capture_mode(bool enabled);

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
	void rebuild_mesh();
	void recenter_camera();
	bool draw_frame(std::vector<std::uint8_t>* captured_bgra = nullptr, int* captured_width = nullptr, int* captured_height = nullptr);
	void initialize_controls();
	void populate_controls();
	void update_controls();
	void layout_controls();
	void open_mdl_in_text_editor();
	void save_mdx();
	void set_sequence(int index);

	std::shared_ptr<mdx::MDX> mdx;
	std::shared_ptr<SkinnedMesh> mesh;
	std::optional<std::pair<int, std::string>> replaceable_id_override;
	SkeletalModelInstance skeleton;
	InputHandler input_handler;
	ModelEditorCamera camera;

	QElapsedTimer elapsed_timer;
	QTimer frame_timer;
	QFrame* controls_panel = nullptr;
	QLabel* fps_label = nullptr;
	QLabel* model_name_label = nullptr;
	QLabel* geometry_label = nullptr;
	QLabel* extent_label = nullptr;
	QLabel* sequence_info_label = nullptr;
	QLabel* current_frame_label = nullptr;
	QComboBox* sequence_combo = nullptr;
	MetalSurface surface;
	std::unique_ptr<hive::metal::IModelFrameRenderer> renderer;
	SmokePreviewStats last_smoke_stats;

	bool surface_initialized = false;
	bool smoke_capture_mode = false;
	double delta = 0.0;
};

#endif
