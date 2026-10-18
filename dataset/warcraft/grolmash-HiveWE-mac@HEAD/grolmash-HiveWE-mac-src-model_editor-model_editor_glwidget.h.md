---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/model_editor/model_editor_glwidget.h"
ref: HEAD
---

#pragma once

#if defined(HIVEWE_BUILD_OPENGL_RENDERER)

#include <glad/glad.h>
#include <cstdint>
#include <memory>
#include <optional>
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
#include <string>
#include <utility>
#include <vector>
#include "qt_imgui/qt_imgui.h"
#include <model_editor/model_editor_camera.h>
#include <rendering/model_preview_renderer.h>

import EditableMesh;
import SkeletalModelInstance;
import MDX;

class ModelEditorGLWidget: public QOpenGLWidget {
	Q_OBJECT

  public:
	QElapsedTimer elapsed_timer;
	GLuint vao = 0;

	double delta = 0.0;

	ModelEditorGLWidget() = delete;
	explicit ModelEditorGLWidget(
		QWidget* parent,
		std::shared_ptr<mdx::MDX> model,
		std::optional<std::pair<int, std::string>> replaceable_id_override = std::nullopt
	);
	~ModelEditorGLWidget() override;

	void initializeGL() override;
	void resizeGL(int w, int h) override;
	void paintGL() override;
	bool smoke_capture_preview(std::vector<std::uint8_t>& captured_rgba, int& captured_width, int& captured_height);

	void keyPressEvent(QKeyEvent* event) override;
	void keyReleaseEvent(QKeyEvent* event) override;
	void mouseMoveEvent(QMouseEvent* event) override;
	void mousePressEvent(QMouseEvent* event) override;
	void mouseReleaseEvent(QMouseEvent* event) override;
	void wheelEvent(QWheelEvent* event) override;

	std::shared_ptr<mdx::MDX> mdx;
	std::shared_ptr<EditableMesh> mesh;
	std::optional<std::pair<int, std::string>> replaceable_id_override;
	SkeletalModelInstance skeleton;
	std::unique_ptr<IModelPreviewRenderer> preview_renderer;

	int64_t optimization_file_size_reduction = 0;
	float optimization_file_size_reduction_percent = 0.f;

	QtImGui::RenderRef ref = nullptr;
	ModelEditorCamera camera;

	void recenter_camera();
};

#endif
