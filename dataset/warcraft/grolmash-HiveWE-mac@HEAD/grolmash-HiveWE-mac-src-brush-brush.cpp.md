---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/brush/brush.cpp"
ref: HEAD
---

#include "brush.h"

#include <glm/glm.hpp>
#include "std_module_compat.h"
import Camera;
import Globals;

Brush::Brush() {
	set_size(size);
}

glm::vec2 Brush::get_position() const {
	const bool even_width = size.x % (2 * static_cast<std::size_t>(size_granularity)) == 0;
	const bool even_height = size.y % (2 * static_cast<std::size_t>(size_granularity)) == 0;

	glm::vec2 even = glm::floor(glm::vec2(input_handler.mouse_world) * position_granularity + 0.5f) / position_granularity;
	glm::vec2 uneven =
		glm::floor(glm::vec2(input_handler.mouse_world) * position_granularity) / position_granularity + (1.f / position_granularity / 2.f);

	if (brush_type == Type::corner) {
		std::swap(even, uneven);
	}

	glm::vec2 result;
	if (even_width) {
		result.x = even.x;
	} else {
		result.x = uneven.x;
	}

	if (even_height) {
		result.y = even.y;
	} else {
		result.y = uneven.y;
	}

	return result;
}

void Brush::set_size(const glm::ivec2 new_size) {
	size = glm::clamp(new_size * size_granularity, 1, 999);
	set_shape(shape);
}

void Brush::set_shape(const Shape new_shape) {
	std::vector<glm::u8vec4> brush(size.x * size.y, {0, 0, 0, 0});

	shape = new_shape;

	for (int i = 0; i < size.x; i++) {
		for (int j = 0; j < size.y; j++) {
			if (contains(glm::ivec2(i, j) / size_granularity)) {
				brush[j * size.x + i] = brush_color;
			} else {
				brush[j * size.x + i] = {0, 0, 0, 0};
			}
		}
	}

	set_brush_mask(size, std::move(brush));
}

void Brush::set_brush_mask(const glm::ivec2 mask_size, std::vector<glm::u8vec4> pixels) {
	brush_mask_size_pixels = mask_size;
	brush_mask = std::move(pixels);
	++brush_mask_revision_value;
}

/// Whether the brush shape contains the point, Arguments in brush coordinates
bool Brush::contains(const glm::ivec2 pos) const {
	switch (shape) {
		case Shape::square:
			return true;
		case Shape::circle: {
			const int half_size = (size.x / 2) / size_granularity;
			const int distance = (pos.x - half_size) * (pos.x - half_size) + (pos.y - half_size) * (pos.y - half_size);
			return distance <= half_size * half_size;
		}
		case Shape::diamond:
			const int half_size = (size.x / 2) / size_granularity;
			return std::abs(pos.x - half_size) + std::abs(pos.y - half_size) <= half_size;
	}
	return true;
}

void Brush::increase_size(const int new_size) {
	set_size(size / size_granularity + new_size);
}

void Brush::decrease_size(const int new_size) {
	set_size(size / size_granularity - new_size);
}

void Brush::switch_mode() {
	if (mode != Mode::placement && mode != Mode::selection) {
		mode = return_mode;
	}
	mode = (mode == Mode::placement) ? Mode::selection : Mode::placement;

	clear_selection();
	selection_started = false;
}

void Brush::key_press_event(QKeyEvent* event) {
	switch (event->key()) {
		case Qt::Key_Escape:
			clear_selection();
			break;
		case Qt::Key_Equal:
			increase_size(2);
			break;
		case Qt::Key_Minus:
			decrease_size(2);
			break;
		case Qt::Key_Delete:
			delete_selection();
			break;
		case Qt::Key_X:
			if (event->modifiers() & Qt::ControlModifier) {
				cut_selection();
			}
			break;
		case Qt::Key_C:
			if (event->modifiers() & Qt::ControlModifier) {
				copy_selection();
			}
			break;
		case Qt::Key_V:
			if (event->modifiers() & Qt::ControlModifier) {
				return_mode = mode;
				mode = Mode::pasting;
			}
			break;
	}
}

void Brush::mouse_move_event(QMouseEvent* event, double frame_delta) {
	if (event->buttons() == Qt::LeftButton) {
		if (mode == Mode::placement && (can_place() || event->modifiers() & Qt::ShiftModifier)) {
			apply(frame_delta);
		}
	}
}

void Brush::mouse_press_event(QMouseEvent* event, double frame_delta) {
	if (event->button() != Qt::LeftButton) {
		return;
	}

	if (!event->modifiers()) {
		clear_selection();
	}

	if (mode == Mode::selection && !(event->modifiers() & Qt::ControlModifier)) {
		if (!selection_started) {
			selection_started = true;
			selection_start = input_handler.mouse_world;
		}
	} else if (mode == Mode::placement) {
		// Check if eligible for placement
		if (event->button() == Qt::LeftButton) {
			apply_begin();
			if (can_place() || event->modifiers() & Qt::ShiftModifier) {
				apply(0.5);
			}
		}
	} else if (mode == Mode::pasting && (can_place() || event->modifiers() & Qt::ShiftModifier)) {
		clear_selection();
		place_clipboard();
		mode = Mode::selection;
	}
}

void Brush::mouse_release_event(QMouseEvent* event) {
	if (mode == Mode::selection) {
		selection_started = false;
	} else if (mode == Mode::placement) {
		if (event->button() == Qt::LeftButton) {
			apply_end();
		}
	}
}

void Brush::render() {
	if (mode == Mode::placement) {
		render_brush();
	}
	if (mode == Mode::pasting) {
		render_clipboard();
	}
}

void Brush::append_selection_overlay(BrushOverlayFrame& frame) const {
	if (mode == Mode::selection && selection_started) {
		frame.append_selector(selection_start, input_handler.mouse_world);
	}
}

void Brush::render_brush() {}

#include "moc_brush.cpp"
