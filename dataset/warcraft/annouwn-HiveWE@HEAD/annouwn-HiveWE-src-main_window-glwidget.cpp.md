---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/main_window/glwidget.cpp"
ref: HEAD
---

#include "glwidget.h"
#include "doodad_brush.h"
#include "pathing_brush.h"
#include "region_brush.h"
#include "terrain_brush.h"
#include "unit_brush.h"
#include "unit_properties_dialog.h"

#include <QTimer>
#include <QPainter>

#include <tracy/Tracy.hpp>

import std;
import OpenGLUtilities;
import Camera;
import MapGlobal;
import <glad/glad.h>;
import <glm/glm.hpp>;

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

GLWidget::GLWidget(QWidget* parent) : QOpenGLWidget(parent) {
	setMouseTracking(true);
	setFocus();
	setFocusPolicy(Qt::WheelFocus);

	connect(this, &QOpenGLWidget::frameSwapped, [&]() {
		update();
	});
}

void GLWidget::initializeGL() {
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

	shapes.init();
}

void GLWidget::resizeGL(const int w, const int h) {
	glViewport(0, 0, w, h);

	delta = elapsed_timer.nsecsElapsed() / 1'000'000'000.0;
	camera.aspect_ratio = static_cast<double>(w) / h;

	if (!map || !map->loaded) {
		return;
	}
	camera.update(delta);
	map->render_manager.resize_framebuffers(w, h);
}

void GLWidget::paintGL() {
	delta = elapsed_timer.nsecsElapsed() / 1'000'000'000.0;
	elapsed_timer.start();

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

	if (map->render_debug) {
		QPainter p(this);
		p.setPen(QColor(Qt::GlobalColor::white));
		p.setFont(QFont("Arial", 10, 100, false));
		QFont font("Consolas");
		font.setStyleHint(QFont::Monospace);
		p.setFont(font);

		// General info
		auto fmt_vec3 = [](const char* label, glm::vec3 v) {
			return std::format("{:<20} X:{:>6.3f}  Y:{:>6.3f}  Z:{:>6.3f}", label, v.x, v.y, v.z);
		};

		const auto to_wc3_position = [](glm::vec3 v) {
			return glm::vec3 {
				v.x * 128.f + map->terrain.offset.x,
				v.y * 128.f + map->terrain.offset.y,
				v.z * 128.f,
			};
		};

		constexpr int debug_left_column_x = 10;
		constexpr int debug_right_column_x = 500;

		p.drawText(debug_left_column_x, 20, QString::fromStdString(fmt_vec3("Mouse WC3 Position", to_wc3_position(input_handler.mouse_world))));
		p.drawText(debug_left_column_x, 35, QString::fromStdString(fmt_vec3("Camera WC3 Position", to_wc3_position(camera.position))));
		p.drawText(debug_left_column_x, 50, QString::fromStdString(std::format("Camera Horizontal Angle: {:.4f}", camera.horizontal_angle)));
		p.drawText(debug_left_column_x, 65, QString::fromStdString(std::format("Camera Vertical Angle: {:.4f}", camera.vertical_angle)));

		const glm::ivec2 terrain_index = input_handler.mouse_world;
		const auto corner = map->terrain.get_corner(terrain_index.x, terrain_index.y);
		p.drawText(debug_right_column_x, 20, QString::fromStdString(std::format("Tile info")));
		p.drawText(debug_right_column_x, 35, QString::fromStdString(std::format("Cliff: {}", corner.cliff)));
		p.drawText(debug_right_column_x, 50, QString::fromStdString(std::format("Blight: {}", corner.blight)));
		p.drawText(debug_right_column_x, 65, QString::fromStdString(std::format("Boundary: {}", corner.boundary)));
		p.drawText(debug_right_column_x, 80, QString::fromStdString(std::format("Height: {}", corner.height)));
		p.drawText(debug_right_column_x, 95, QString::fromStdString(std::format("Ramp: {}", corner.ramp)));
		p.drawText(debug_right_column_x, 110, QString::fromStdString(std::format("Romp: {}", corner.romp)));
		p.drawText(debug_right_column_x, 125, QString::fromStdString(std::format("Water: {}", corner.water)));
		p.drawText(debug_right_column_x, 140, QString::fromStdString(std::format("Water height: {}", corner.water_height)));

		p.end();

		// Set changed state back
		glEnable(GL_DEPTH_TEST);
		glDepthFunc(GL_LEQUAL);
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
	}

	FrameMark;
}

void GLWidget::keyPressEvent(QKeyEvent* event) {
	if (!map) {
		return;
	}

	input_handler.keys_pressed.emplace(event->key());

	if (map->brush) {
		map->brush->key_press_event(event);
	}
	QOpenGLWidget::keyPressEvent(event);
}

void GLWidget::keyReleaseEvent(QKeyEvent* event) {
	if (!map) {
		return;
	}

	input_handler.keys_pressed.erase(event->key());

	if (map->brush) {
		map->brush->key_release_event(event);
	}
	QOpenGLWidget::keyReleaseEvent(event);
}

void GLWidget::mouseMoveEvent(QMouseEvent* event) {
	if (!map) {
		return;
	}

	input_handler.mouse_move_event(event);
	camera.mouse_move_event(event);

	if (map->brush) {
		map->brush->mouse_move_event(event, delta);
	}

	if (auto* region_brush = dynamic_cast<RegionBrush*>(map->brush)) {
		setCursor(region_brush->cursor_shape());
	} else {
		unsetCursor();
	}
}

void GLWidget::mousePressEvent(QMouseEvent* event) {
	if (!map) {
		return;
	}

	camera.mouse_press_event(event);
	if (map->brush) {
		makeCurrent();
		map->brush->mouse_press_event(event, delta);
	}
}

void GLWidget::mouseDoubleClickEvent(QMouseEvent* event) {
	if (!map || event->button() != Qt::LeftButton) {
		QOpenGLWidget::mouseDoubleClickEvent(event);
		return;
	}

	input_handler.mouse_move_event(event);

	if (auto* unit_brush = dynamic_cast<UnitBrush*>(map->brush);
		unit_brush && unit_brush->get_mode() == Brush::Mode::selection && input_handler.mouse.y > 0.f) {
		makeCurrent();
		const auto id = map->render_manager.pick_unit_id_under_mouse(map->units, input_handler.mouse);
		if (id.has_value()) {
			Unit& unit = map->units.units[id.value()];
			if (unit.id != "sloc") {
				UnitPropertiesDialog dialog(&unit, this);
				dialog.exec();
			}
			event->accept();
			return;
		}
	}

	if (auto* doodad_brush = dynamic_cast<DoodadBrush*>(map->brush);
		doodad_brush && doodad_brush->get_mode() == Brush::Mode::selection && input_handler.mouse.y > 0.f) {
		makeCurrent();
		if (map->render_manager.pick_doodad_id_under_mouse(map->doodads, input_handler.mouse).has_value()) {
			event->accept();
			return;
		}
	}

	QOpenGLWidget::mouseDoubleClickEvent(event);
}

void GLWidget::mouseReleaseEvent(QMouseEvent* event) {
	if (!map) {
		return;
	}
	camera.mouse_release_event(event);
	if (map->brush) {
		map->brush->mouse_release_event(event);
	}

	if (auto* region_brush = dynamic_cast<RegionBrush*>(map->brush)) {
		setCursor(region_brush->cursor_shape());
	} else {
		unsetCursor();
	}
}

void GLWidget::wheelEvent(QWheelEvent* event) {
	if (!map) {
		return;
	}

	if (event->modifiers() & Qt::ShiftModifier) {
		if (auto* doodad_brush = dynamic_cast<DoodadBrush*>(map->brush); doodad_brush && !doodad_brush->selections.empty()) {
			if (event->angleDelta().y() > 0) {
				doodad_brush->scale_selection(1.1f);
			} else if (event->angleDelta().y() < 0) {
				doodad_brush->scale_selection(1.f / 1.1f);
			}
			event->accept();
			return;
		}

		if (dynamic_cast<TerrainBrush*>(map->brush)) {
			if (event->angleDelta().y() > 0) {
				map->brush->increase_size(2);
			} else if (event->angleDelta().y() < 0) {
				map->brush->decrease_size(2);
			}
			event->accept();
			return;
		}

		if (dynamic_cast<PathingBrush*>(map->brush)) {
			if (event->angleDelta().y() > 0) {
				map->brush->increase_size(2);
			} else if (event->angleDelta().y() < 0) {
				map->brush->decrease_size(2);
			}
			event->accept();
			return;
		}
	}

	camera.mouse_scroll_event(event);
}
