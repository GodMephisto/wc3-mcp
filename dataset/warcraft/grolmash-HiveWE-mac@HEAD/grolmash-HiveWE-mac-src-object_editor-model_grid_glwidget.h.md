---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/object_editor/model_grid_glwidget.h"
ref: HEAD
---

#pragma once

#if defined(HIVEWE_BUILD_OPENGL_RENDERER)

#include <glad/glad.h>
#include <bitset>
#include <cstdint>
#include <filesystem>
#include <memory>
#include <string>
#include <vector>
#include <QElapsedTimer>
#include <QObject>
#ifndef QT_NO_OPENGL
#define HIVEWE_UNDEFINE_QT_NO_OPENGL_AFTER_QOPENGLWIDGET
#define QT_NO_OPENGL
#endif
#include <QOpenGLWidget>
#ifdef HIVEWE_UNDEFINE_QT_NO_OPENGL_AFTER_QOPENGLWIDGET
#undef QT_NO_OPENGL
#undef HIVEWE_UNDEFINE_QT_NO_OPENGL_AFTER_QOPENGLWIDGET
#endif
#include <rendering/model_preview_renderer.h>

#include "object_editor/model_grid_types.h"

#include <glm/glm.hpp>
import EditableMesh;
import SkeletalModelInstance;
import MDX;

class ModelGridGLWidget : public QOpenGLWidget {
	Q_OBJECT

  public:
	ModelGridGLWidget() = delete;
	explicit ModelGridGLWidget(const std::vector<ModelEntry>& entries, QWidget* parent = nullptr);
	~ModelGridGLWidget() override;

	void initializeGL() override;
	void resizeGL(int w, int h) override;
	void paintGL() override;
	bool smoke_capture_preview(std::vector<std::uint8_t>& captured_rgba, int& captured_width, int& captured_height);

	bool event(QEvent* event) override;
	void mousePressEvent(QMouseEvent* event) override;
	void mouseDoubleClickEvent(QMouseEvent* event) override;
	void wheelEvent(QWheelEvent* event) override;

	int cell_pixel_size() const { return cell_size; }

  public slots:
	void set_scroll_offset(int y);
	void set_search(const QString& query);
	void set_categories(std::bitset<static_cast<std::size_t>(ModelCategory::Count)> mask);

  signals:
	void clicked(const std::filesystem::path& path);
	void double_clicked(const std::filesystem::path& path);
	void scroll_changed(int offset);
	void content_height_changed(int total_px);

  private:
	struct PreviewCell {
		std::filesystem::path path;
		ModelCategory category;
		std::optional<std::pair<int, std::string>> replaceable_id_override;
		std::shared_ptr<mdx::MDX> mdx;
		std::shared_ptr<EditableMesh> mesh;
		SkeletalModelInstance skeleton;
		float fit_distance = 0.f;
		glm::vec3 fit_position{ 0.f, 0.f, 0.f };
		bool loaded = false;
		bool load_failed = false;
		std::string load_error_message;
	};

	struct LayoutRow {
		enum class Kind { Header, Cells };
		Kind kind;
		ModelCategory category;
		int visible_offset = 0;
		int visible_count = 0;
		int y_top = 0;
		int height = 0;
	};

	std::vector<PreviewCell> all_cells;
	std::vector<int> visible_indices;
	std::vector<LayoutRow> layout;
	std::unique_ptr<IModelPreviewRenderer> preview_renderer;
	GLuint vao = 0;

	int cell_size = 128;
	int header_height = 40;
	int columns = 1;
	int scroll_offset_y = 0;

	std::string search_query;
	std::bitset<static_cast<std::size_t>(ModelCategory::Count)> category_mask;

	QElapsedTimer elapsed_timer;
	double delta = 0.0;

	void load_cell(PreviewCell& cell) const;
	[[nodiscard]] int cell_index_at(const QPoint& position) const;
	void rebuild_layout();
	int content_height_px() const;
	int max_scroll_offset() const;
	void emit_layout_change();
};

#endif
