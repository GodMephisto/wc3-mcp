---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/brush/region_brush.cpp"
ref: HEAD
---

#include "region_brush.h"

import std;
import Camera;
import MapGlobal;
import OpenGLUtilities;
import <glad/glad.h>;
import <glm/glm.hpp>;
import <glm/gtc/matrix_transform.hpp>;

namespace {
constexpr float handle_threshold = 0.45f;
constexpr float min_region_size = 0.25f;

glm::vec2 mouse_world_xy() {
	return glm::vec2(input_handler.mouse_world);
}

float region_area(const Region& region) {
	return std::abs((region.right - region.left) * (region.top - region.bottom));
}
}

RegionBrush::RegionBrush() {
	mode = Mode::selection;
	return_mode = Mode::selection;
	brush_color = {255, 120, 210, 110};
}

int RegionBrush::selected_index() const {
	return selection_index;
}

Region* RegionBrush::selected_region() {
	if (selection_index < 0 || selection_index >= static_cast<int>(map->regions.regions.size())) {
		return nullptr;
	}
	return &map->regions.regions[selection_index];
}

const Region* RegionBrush::selected_region() const {
	if (selection_index < 0 || selection_index >= static_cast<int>(map->regions.regions.size())) {
		return nullptr;
	}
	return &map->regions.regions[selection_index];
}

void RegionBrush::select_region(int index) {
	if (index < 0 || index >= static_cast<int>(map->regions.regions.size())) {
		clear_selection();
		return;
	}

	selection_index = index;
	emit_selection_change();
}

void RegionBrush::create_region_at_cursor() {
	const glm::vec2 center = mouse_world_xy();
	const glm::vec2 half_extent(3.f, 3.f);
	map->regions.regions.push_back(make_region(center - half_extent, center + half_extent));
	selection_index = static_cast<int>(map->regions.regions.size()) - 1;
	emit_selection_change();
}

void RegionBrush::set_selected_name(const QString& name) {
	if (auto* region = selected_region()) {
		region->name = name.toStdString();
		emit_selection_change();
	}
}

void RegionBrush::set_selected_weather_id(const QString& weather_id) {
	if (auto* region = selected_region()) {
		region->weather_id = weather_id.toStdString();
		emit_selection_change();
	}
}

void RegionBrush::set_selected_ambient_id(const QString& ambient_id) {
	if (auto* region = selected_region()) {
		region->ambient_id = ambient_id.toStdString();
		emit_selection_change();
	}
}

void RegionBrush::set_selected_color(const QColor& color) {
	if (auto* region = selected_region()) {
		region->color = {color.red(), color.green(), color.blue()};
		emit_selection_change();
	}
}

Qt::CursorShape RegionBrush::cursor_shape() const {
	if (mode != Mode::selection) {
		return Qt::ArrowCursor;
	}

	const glm::vec2 mouse = mouse_world_xy();
	const int hovered_index = region_at_point(mouse);
	if (hovered_index < 0) {
		return Qt::ArrowCursor;
	}

	const DragHandle handle = resolve_drag_handle(map->regions.regions[hovered_index], mouse);
	switch (handle) {
		case DragHandle::move:
			return Qt::SizeAllCursor;
		case DragHandle::left:
		case DragHandle::right:
			return Qt::SizeHorCursor;
		case DragHandle::top:
		case DragHandle::bottom:
			return Qt::SizeVerCursor;
		case DragHandle::top_left:
		case DragHandle::bottom_right:
			return Qt::SizeFDiagCursor;
		case DragHandle::top_right:
		case DragHandle::bottom_left:
			return Qt::SizeBDiagCursor;
		case DragHandle::none:
		default:
			return Qt::ArrowCursor;
	}
}

void RegionBrush::mouse_press_event(QMouseEvent* event, double frame_delta) {
	if (event->button() != Qt::LeftButton) {
		return;
	}

	if (mode == Mode::placement) {
		selection_started = true;
		selection_start = input_handler.mouse_world;
		return;
	}

	if (mode != Mode::selection) {
		return;
	}

	const glm::vec2 mouse = mouse_world_xy();
	const int hit_index = region_at_point(mouse);

	if (hit_index < 0) {
		clear_selection();
		return;
	}

	selection_index = hit_index;
	drag_start_mouse = mouse;
	drag_start_region = map->regions.regions[selection_index];
	drag_handle = resolve_drag_handle(drag_start_region, mouse);
	emit_selection_change();
}

void RegionBrush::mouse_move_event(QMouseEvent* event, double frame_delta) {
	if (!(event->buttons() & Qt::LeftButton)) {
		return;
	}

	if (mode != Mode::selection || drag_handle == DragHandle::none) {
		return;
	}

	Region* region = selected_region();
	if (!region) {
		return;
	}

	const glm::vec2 mouse = mouse_world_xy();
	const glm::vec2 delta = mouse - drag_start_mouse;

	*region = drag_start_region;

	switch (drag_handle) {
		case DragHandle::move:
			region->left += delta.x;
			region->right += delta.x;
			region->bottom += delta.y;
			region->top += delta.y;
			break;
		case DragHandle::left:
			region->left += delta.x;
			break;
		case DragHandle::right:
			region->right += delta.x;
			break;
		case DragHandle::top:
			region->top += delta.y;
			break;
		case DragHandle::bottom:
			region->bottom += delta.y;
			break;
		case DragHandle::top_left:
			region->left += delta.x;
			region->top += delta.y;
			break;
		case DragHandle::top_right:
			region->right += delta.x;
			region->top += delta.y;
			break;
		case DragHandle::bottom_left:
			region->left += delta.x;
			region->bottom += delta.y;
			break;
		case DragHandle::bottom_right:
			region->right += delta.x;
			region->bottom += delta.y;
			break;
		case DragHandle::none:
			break;
	}

	clamp_region(*region);
	emit_selection_change();
}

void RegionBrush::mouse_release_event(QMouseEvent* event) {
	if (mode == Mode::placement) {
		if (selection_started) {
			selection_started = false;
			Region region = make_region(glm::vec2(selection_start), mouse_world_xy());
			if (region.right - region.left >= min_region_size && region.top - region.bottom >= min_region_size) {
				map->regions.regions.push_back(region);
				selection_index = static_cast<int>(map->regions.regions.size()) - 1;
				emit_selection_change();
			}
		}
		return;
	}

	if (drag_handle != DragHandle::none) {
		if (auto* region = selected_region(); region && region_geometry_changed(drag_start_region, *region)) {
			auto action = std::make_unique<RegionStateAction>();
			action->old_region = drag_start_region;
			action->new_region = *region;
			map->world_undo.new_undo_group();
			map->world_undo.add_undo_action(std::move(action));
		}
	}

	drag_handle = DragHandle::none;
}

void RegionBrush::render_brush() {
	if (selection_started) {
		render_selector();
	}
}

void RegionBrush::apply(double frame_delta) {
}

void RegionBrush::render_selection() const {
	for (int i = 0; i < static_cast<int>(map->regions.regions.size()); ++i) {
		const bool selected = i == selection_index;
		const auto& region = map->regions.regions[i];
		const glm::u8vec4 fill_color(
			static_cast<uint8_t>(std::clamp(region.color.r, 0.f, 255.f)),
			static_cast<uint8_t>(std::clamp(region.color.g, 0.f, 255.f)),
			static_cast<uint8_t>(std::clamp(region.color.b, 0.f, 255.f)),
			selected ? 120 : 72
		);
		const glm::u8vec4 outline_color = selected ? glm::u8vec4(255, 120, 210, 255) : glm::u8vec4(fill_color.r, fill_color.g, fill_color.b, 215);
		draw_region(region, fill_color, true);
		draw_region(region, outline_color, false);
	}
}

void RegionBrush::delete_selection() {
	if (selection_index < 0 || selection_index >= static_cast<int>(map->regions.regions.size())) {
		return;
	}

	map->regions.regions.erase(map->regions.regions.begin() + selection_index);
	selection_index = -1;
	drag_handle = DragHandle::none;
	emit_selection_change();
}

void RegionBrush::clear_selection() {
	if (selection_index == -1 && drag_handle == DragHandle::none) {
		return;
	}

	selection_index = -1;
	drag_handle = DragHandle::none;
	emit_selection_change();
}

bool RegionBrush::contains(const Region& region, const glm::vec2& point) const {
	return point.x >= region.left && point.x <= region.right && point.y >= region.bottom && point.y <= region.top;
}

RegionBrush::DragHandle RegionBrush::resolve_drag_handle(const Region& region, const glm::vec2& point) const {
	const bool near_left = std::abs(point.x - region.left) <= handle_threshold;
	const bool near_right = std::abs(point.x - region.right) <= handle_threshold;
	const bool near_bottom = std::abs(point.y - region.bottom) <= handle_threshold;
	const bool near_top = std::abs(point.y - region.top) <= handle_threshold;

	if (near_left && near_top) {
		return DragHandle::top_left;
	}
	if (near_right && near_top) {
		return DragHandle::top_right;
	}
	if (near_left && near_bottom) {
		return DragHandle::bottom_left;
	}
	if (near_right && near_bottom) {
		return DragHandle::bottom_right;
	}
	if (near_left) {
		return DragHandle::left;
	}
	if (near_right) {
		return DragHandle::right;
	}
	if (near_top) {
		return DragHandle::top;
	}
	if (near_bottom) {
		return DragHandle::bottom;
	}
	return DragHandle::move;
}

void RegionBrush::clamp_region(Region& region) const {
	const float max_x = static_cast<float>(map->terrain.width - 1);
	const float max_y = static_cast<float>(map->terrain.height - 1);

	if (drag_handle == DragHandle::move) {
		const float width = region.right - region.left;
		const float height = region.top - region.bottom;
		region.left = std::clamp(region.left, 0.f, max_x - width);
		region.right = region.left + width;
		region.bottom = std::clamp(region.bottom, 0.f, max_y - height);
		region.top = region.bottom + height;
		return;
	}

	region.left = std::clamp(region.left, 0.f, max_x);
	region.right = std::clamp(region.right, 0.f, max_x);
	region.bottom = std::clamp(region.bottom, 0.f, max_y);
	region.top = std::clamp(region.top, 0.f, max_y);

	if (region.right < region.left) {
		std::swap(region.right, region.left);
	}
	if (region.top < region.bottom) {
		std::swap(region.top, region.bottom);
	}

	if (region.right - region.left < min_region_size) {
		region.right = std::min(max_x, region.left + min_region_size);
	}
	if (region.top - region.bottom < min_region_size) {
		region.top = std::min(max_y, region.bottom + min_region_size);
	}
}

Region RegionBrush::make_region(glm::vec2 start, glm::vec2 end) const {
	Region region;
	region.left = std::min(start.x, end.x);
	region.right = std::max(start.x, end.x);
	region.bottom = std::min(start.y, end.y);
	region.top = std::max(start.y, end.y);
	region.name = next_region_name();
	region.creation_number = next_creation_number();
	region.weather_id = "";
	region.ambient_id = "";
	region.color = {255.f, 120.f, 210.f};
	clamp_region(region);
	return region;
}

std::string RegionBrush::next_region_name() const {
	int index = 1;
	while (true) {
		const std::string candidate = std::format("Region {}", index);
		const bool exists = std::ranges::any_of(map->regions.regions, [&](const Region& region) { return region.name == candidate; });
		if (!exists) {
			return candidate;
		}
		++index;
	}
}

int RegionBrush::next_creation_number() const {
	int max_value = 0;
	for (const auto& region : map->regions.regions) {
		max_value = std::max(max_value, region.creation_number);
	}
	return max_value + 1;
}

bool RegionBrush::region_geometry_changed(const Region& lhs, const Region& rhs) const {
	return lhs.left != rhs.left || lhs.right != rhs.right || lhs.top != rhs.top || lhs.bottom != rhs.bottom;
}

int RegionBrush::region_at_point(const glm::vec2& point) const {
	int hit_index = -1;
	float smallest_area = std::numeric_limits<float>::max();
	for (int i = 0; i < static_cast<int>(map->regions.regions.size()); ++i) {
		if (!contains(map->regions.regions[i], point)) {
			continue;
		}

		const float area = region_area(map->regions.regions[i]);
		if (area < smallest_area) {
			smallest_area = area;
			hit_index = i;
		}
	}
	return hit_index;
}

void RegionBrush::emit_selection_change() {
	emit selection_changed();
}

void RegionBrush::draw_region(const Region& region, const glm::u8vec4& color, const bool filled) const {
	glDisable(GL_DEPTH_TEST);
	selection_shader->use();

	glm::mat4 model(1.f);
	model = glm::translate(model, glm::vec3(region.left, region.bottom, 0.f));
	model = glm::scale(model, glm::vec3(region.right - region.left, region.top - region.bottom, 1.f));
	model = camera.projection_view * model;
	glUniformMatrix4fv(1, 1, GL_FALSE, &model[0][0]);
	glUniform4f(2, color.r / 255.f, color.g / 255.f, color.b / 255.f, color.a / 255.f);

	glEnableVertexAttribArray(0);
	glBindBuffer(GL_ARRAY_BUFFER, shapes.vertex_buffer);
	glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 0, nullptr);
	glDrawArrays(filled ? GL_TRIANGLE_FAN : GL_LINE_LOOP, 0, 4);
	glDisableVertexAttribArray(0);
	glEnable(GL_DEPTH_TEST);
}
