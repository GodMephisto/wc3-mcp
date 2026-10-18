---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/object_editor/model_grid_metal_widget.h"
ref: HEAD
---

#pragma once

#include "runtime_config.h"

#if HIVEWE_USE_METAL_RUNTIME

#include <array>
#include <bitset>
#include <cstdint>
#include <filesystem>
#include <memory>
#include <string>
#include <vector>

#include <QElapsedTimer>
#include <QTimer>
#include <QWidget>

class QLabel;
class QEvent;

#include <glm/glm.hpp>

#include "object_editor/model_grid_types.h"
#include "rendering/metal_model_frame_stats.h"
#include "rendering/metal_surface.h"

namespace hive::metal {
class IModelFrameRenderer;
} // namespace hive::metal

import SkeletalModelInstance;
import SkinnedMesh;

class ModelGridMetalWidget : public QWidget {
	Q_OBJECT

  public:
	ModelGridMetalWidget() = delete;
	explicit ModelGridMetalWidget(const std::vector<ModelEntry>& entries, QWidget* parent = nullptr);
	~ModelGridMetalWidget() override;

	QPaintEngine* paintEngine() const override {
		return nullptr;
	}

	int cell_pixel_size() const {
		return cell_size;
	}

	struct SmokePreviewStats {
		std::size_t loaded_cell_count = 0;
		std::size_t failed_cell_count = 0;
		std::size_t rendered_cell_count = 0;
		std::filesystem::path first_rendered_path;
		std::filesystem::path first_failed_path;
		std::string first_failed_error;
		int first_rendered_x = 0;
		int first_rendered_y = 0;
		int first_rendered_width = 0;
		int first_rendered_height = 0;
		float first_rendered_fit_distance = 0.0f;
		float first_rendered_fit_z = 0.0f;
		int scroll_offset_y = 0;
		int content_height = 0;
		hive::metal::MetalModelFrameStats frame;
	};

	bool smoke_capture_preview(std::vector<std::uint8_t>& captured_bgra, int& captured_width, int& captured_height);
	bool smoke_capture_preview_at_scroll(
		int requested_scroll_offset_y,
		std::vector<std::uint8_t>& captured_bgra,
		int& captured_width,
		int& captured_height
	);
	[[nodiscard]] const SmokePreviewStats& smoke_preview_stats() const {
		return last_smoke_stats;
	}
	void set_smoke_capture_mode(bool enabled);

  public slots:
	void set_scroll_offset(int y);
	void set_search(const QString& query);
	void set_categories(std::bitset<static_cast<std::size_t>(ModelCategory::Count)> mask);

  signals:
	void clicked(const std::filesystem::path& path);
	void double_clicked(const std::filesystem::path& path);
	void scroll_changed(int offset);
	void content_height_changed(int total_px);

  protected:
	bool event(QEvent* event) override;
	void showEvent(QShowEvent* event) override;
	void resizeEvent(QResizeEvent* event) override;
	void paintEvent(QPaintEvent* event) override;
	void mousePressEvent(QMouseEvent* event) override;
	void mouseDoubleClickEvent(QMouseEvent* event) override;
	void wheelEvent(QWheelEvent* event) override;

  private:
	struct PreviewCell {
		std::filesystem::path path;
		ModelCategory category;
		std::optional<std::pair<int, std::string>> replaceable_id_override;
		std::shared_ptr<SkinnedMesh> mesh;
		SkeletalModelInstance skeleton;
		float fit_distance = 0.f;
		glm::vec3 fit_position{0.f, 0.f, 0.f};
		bool loaded = false;
		bool load_failed = false;
		std::string load_error_message;
	};

	struct LayoutRow {
		enum class Kind { Header, Cells };
		Kind kind = Kind::Cells;
		ModelCategory category = ModelCategory::Units;
		int visible_offset = 0;
		int visible_count = 0;
		int y_top = 0;
		int height = 0;
	};

	bool ensure_surface();
	bool draw_frame(std::vector<std::uint8_t>* captured_bgra = nullptr, int* captured_width = nullptr, int* captured_height = nullptr);
	void paint_capture_overlay(std::vector<std::uint8_t>& captured_bgra, int drawable_width, int drawable_height, double scale) const;
	void initialize_header_labels();
	void sync_header_labels();
	void load_cell(PreviewCell& cell) const;
	[[nodiscard]] int cell_index_at(const QPoint& position) const;
	void rebuild_layout();
	int content_height_px() const;
	int max_scroll_offset() const;
	void emit_layout_change();

	std::vector<PreviewCell> all_cells;
	std::vector<int> visible_indices;
	std::vector<LayoutRow> layout;
	std::vector<QLabel*> failed_cell_labels;

	QElapsedTimer elapsed_timer;
	QTimer frame_timer;
	std::array<QLabel*, static_cast<std::size_t>(ModelCategory::Count)> header_labels {};
	MetalSurface surface;
	std::unique_ptr<hive::metal::IModelFrameRenderer> renderer;
	SmokePreviewStats last_smoke_stats;

	int cell_size = 128;
	int header_height = 40;
	int columns = 1;
	int scroll_offset_y = 0;

	std::string search_query;
	std::bitset<static_cast<std::size_t>(ModelCategory::Count)> category_mask;
	bool surface_initialized = false;
	bool smoke_capture_mode = false;
	double delta = 0.0;
};

#endif
