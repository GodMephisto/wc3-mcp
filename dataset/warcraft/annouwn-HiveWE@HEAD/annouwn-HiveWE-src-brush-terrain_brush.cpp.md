---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/brush/terrain_brush.cpp"
ref: HEAD
---

#include "terrain_brush.h"
#include "terrain_operators.h"

import std;
import MapGlobal;
import Terrain;
import DoodadsUndo;
import PathingUndo;
import PathingMap;
import TerrainUndo;
import Camera;
import Rects;
import OpenGLUtilities;
import Globals;

namespace {
TerrainRect normalized_tile_area(const glm::vec2& a, const glm::vec2& b, const Terrain& terrain) {
	const int left = static_cast<int>(std::floor(std::min(a.x, b.x)));
	const int top = static_cast<int>(std::floor(std::min(a.y, b.y)));
	const int right = static_cast<int>(std::ceil(std::max(a.x, b.x)));
	const int bottom = static_cast<int>(std::ceil(std::max(a.y, b.y)));

	return TerrainRect(left, top, std::max(1, right - left), std::max(1, bottom - top)).intersected({0, 0, terrain.width - 1, terrain.height - 1});
}

TerrainRect corner_area_from_tile_area(const TerrainRect& tile_area) {
	return TerrainRect(tile_area.x(), tile_area.y(), tile_area.width() + 1, tile_area.height() + 1);
}

PathingRect pathing_area_from_tile_area(const TerrainRect& tile_area) {
	return PathingRect(tile_area.x() * 4, tile_area.y() * 4, tile_area.width() * 4, tile_area.height() * 4);
}

glm::vec2 tile_area_center(const TerrainRect& area) {
	return glm::vec2(area.x() + area.width() * 0.5f, area.y() + area.height() * 0.5f);
}

std::vector<uint8_t> selection_mask_for_area(const TerrainRect& area, Brush::Shape shape) {
	std::vector<uint8_t> mask(area.width() * area.height(), true);
	if (shape == Brush::Shape::square || area.width() <= 0 || area.height() <= 0) {
		return mask;
	}

	const float half_width = area.width() * 0.5f;
	const float half_height = area.height() * 0.5f;

	for (int y = 0; y < area.height(); ++y) {
		for (int x = 0; x < area.width(); ++x) {
			const float normalized_x = (x + 0.5f - half_width) / std::max(half_width, 0.5f);
			const float normalized_y = (y + 0.5f - half_height) / std::max(half_height, 0.5f);

			bool selected = true;
			if (shape == Brush::Shape::circle) {
				selected = normalized_x * normalized_x + normalized_y * normalized_y <= 1.f;
			} else if (shape == Brush::Shape::diamond) {
				selected = std::abs(normalized_x) + std::abs(normalized_y) <= 1.f;
			}

			mask[y * area.width() + x] = selected;
		}
	}

	return mask;
}

bool tile_is_selected(const std::vector<uint8_t>& mask, const int width, const int x, const int y) {
	return x >= 0 && y >= 0 && y * width + x < static_cast<int>(mask.size()) && mask[y * width + x];
}

bool corner_is_selected(const std::vector<uint8_t>& mask, const int width, const int height, const int x, const int y) {
	return tile_is_selected(mask, width, x, y) || tile_is_selected(mask, width, x - 1, y) || tile_is_selected(mask, width, x, y - 1)
		|| tile_is_selected(mask, width, x - 1, y - 1);
}

void rebuild_overlay_texture(GLuint& texture, const std::vector<uint8_t>& mask, const int width, const int height) {
	if (texture != 0) {
		glDeleteTextures(1, &texture);
		texture = 0;
	}

	if (width <= 0 || height <= 0 || mask.empty()) {
		return;
	}

	std::vector<glm::u8vec4> pixels(width * height, {0, 0, 0, 0});
	for (int y = 0; y < height; ++y) {
		for (int x = 0; x < width; ++x) {
			if (mask[y * width + x]) {
				pixels[y * width + x] = {255, 96, 160, 96};
			}
		}
	}

	glCreateTextures(GL_TEXTURE_2D, 1, &texture);
	glTextureParameteri(texture, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	glTextureParameteri(texture, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTextureParameteri(texture, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
	glTextureParameteri(texture, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
	glTextureStorage2D(texture, 1, GL_RGBA8, width * 4, height * 4);

	std::vector<glm::u8vec4> expanded(width * height * 16, {0, 0, 0, 0});
	for (int y = 0; y < height; ++y) {
		for (int x = 0; x < width; ++x) {
			for (int yy = 0; yy < 4; ++yy) {
				for (int xx = 0; xx < 4; ++xx) {
					expanded[((y * 4 + yy) * width * 4) + x * 4 + xx] = pixels[y * width + x];
				}
			}
		}
	}
	glTextureSubImage2D(texture, 0, 0, 0, width * 4, height * 4, GL_RGBA, GL_UNSIGNED_BYTE, expanded.data());
}
}

TerrainBrush::TerrainBrush() :
	Brush(),
	cliff_operator(*this),
	height_operator(*this),
	texture_operator(*this),
	cell_operator(*this),
	terrain_operators({height_operator, texture_operator, cliff_operator, cell_operator}) {
	position_granularity = 1.f;
	size_granularity = 4;
	brush_type = Brush::Type::corner;

	set_size(size);
}

TerrainBrush::~TerrainBrush() {
	if (selection_texture != 0) {
		glDeleteTextures(1, &selection_texture);
	}
	if (clipboard_texture != 0) {
		glDeleteTextures(1, &clipboard_texture);
	}
}

glm::vec2 TerrainBrush::get_position() const {
	if (mode == Mode::selection) {
		if (selection_started) {
			const TerrainRect area = normalized_tile_area(glm::vec2(selection_start), glm::vec2(input_handler.mouse_world), map->terrain);
			return tile_area_center(area);
		}
		if (has_selection_area) {
			return tile_area_center(selection_area);
		}
	}

	if (mode == Mode::pasting && has_clipboard) {
		return tile_area_center(TerrainRect(
			static_cast<int>(std::floor(input_handler.mouse_world.x)),
			static_cast<int>(std::floor(input_handler.mouse_world.y)),
			clipboard_tile_area.width(),
			clipboard_tile_area.height()
		));
	}

	return Brush::get_position();
}

void TerrainBrush::deactivate_operator(TerrainOperator& target) {
	target.is_active = false;
}

void TerrainBrush::activate_operator(TerrainOperator& target) {
	target.is_active = true;
	brush_type = target.brush_type;

	// deactivate incompatible operators
	for (TerrainOperator& op : terrain_operators) {
		if (&op != &target && !can_combine(target, op)) {
			op.is_active = false;
		}
	}
}

bool TerrainBrush::can_combine(const TerrainOperator& a, const TerrainOperator& b) const {
	using TI = std::type_index;
	using Pair = std::pair<TI, TI>;

	// pairs of operator types that are allowed to be active simultaneously
	static const std::array compatible = {
		Pair {typeid(TextureOperator), typeid(CliffOperator)},
	};

	return std::ranges::any_of(compatible, [ta = TI(typeid(a)), tb = TI(typeid(b))](const Pair& p) {
		return (p.first == ta && p.second == tb) || (p.first == tb && p.second == ta);
	});
}

void TerrainBrush::mouse_press_event(QMouseEvent* event, double frame_delta) {
	//if (event->button() == Qt::LeftButton && mode == Mode::selection && !event->modifiers() && input_handler.mouse.y > 0.f) {
	/*auto id = map->render_manager.pick_unit_id_under_mouse(map->units, input_handler.mouse);
		if (id) {
			Unit& unit = map->units.units[id.value()];
			selections = { &unit };
			dragging = true;
			drag_x_offset = input_handler.mouse_world.x - unit.position.x;
			drag_y_offset = input_handler.mouse_world.y - unit.position.y;
			return;
		}*/
	//}

	Brush::mouse_press_event(event, frame_delta);
}

void TerrainBrush::mouse_move_event(QMouseEvent* event, double frame_delta) {
	Brush::mouse_move_event(event, frame_delta);

	if (mode == Mode::selection && selection_started) {
		const TerrainRect area = normalized_tile_area(glm::vec2(selection_start), glm::vec2(input_handler.mouse_world), map->terrain);
		context->makeCurrent();
		rebuild_overlay_texture(selection_texture, selection_mask_for_area(area, shape), area.width(), area.height());
	}

	/*if (event->buttons() == Qt::LeftButton) {
		if (mode == Mode::selection) {
			if (dragging) {
				if (!dragged) {
					dragged = true;
					map->terrain_undo.new_undo_group();
					unit_state_undo = std::make_unique<UnitStateAction>();
					for (const auto& i : selections) {
						unit_state_undo->old_units.push_back(*i);
					}
				}
				for (auto& i : selections) {
					i->position.x = input_handler.mouse_world.x - drag_x_offset;
					i->position.y = input_handler.mouse_world.y - drag_y_offset;
					i->position.z = map->terrain.interpolated_height(i->position.x, i->position.y);
					i->update();
				}
			} else if (event->modifiers() & Qt::ControlModifier) {
				for (auto&& i : selections) {
					float target_rotation = std::atan2(input_handler.mouse_world.y - i->position.y, input_handler.mouse_world.x - i->position.x);
					if (target_rotation < 0) {
						target_rotation = (glm::pi<float>() + target_rotation) + glm::pi<float>();
					}

					i->angle = target_rotation;
					i->update();
				}
			} else if (selection_started) {
				const glm::vec2 size = glm::vec2(input_handler.mouse_world) - selection_start;
				selections = map->units.query_area({ selection_start.x, selection_start.y, size.x, size.y });
			}
		}
	}*/
}

void TerrainBrush::mouse_release_event(QMouseEvent* event) {
	if (mode == Mode::selection && selection_started) {
		selection_area = normalized_tile_area(glm::vec2(selection_start), glm::vec2(input_handler.mouse_world), map->terrain);
		has_selection_area = selection_area.width() > 0 && selection_area.height() > 0;
		selection_tile_mask = selection_mask_for_area(selection_area, shape);
		context->makeCurrent();
		rebuild_overlay_texture(selection_texture, selection_tile_mask, selection_area.width(), selection_area.height());
	}

	//dragging = false;
	//if (dragged) {
	//	dragged = false;
	//	for (const auto& i : selections) {
	//		unit_state_undo->new_units.push_back(*i);
	//	}
	//	map->terrain_undo.add_undo_action(std::move(unit_state_undo));
	//}

	Brush::mouse_release_event(event);
}

void TerrainBrush::copy_selection() {
	if (!has_selection_area) {
		return;
	}

	clipboard_tile_area = selection_area;
	clipboard_tile_mask = selection_tile_mask;
	clipboard_corners.clear();
	clipboard_pathing_cells_static.clear();

	const TerrainRect corner_area = corner_area_from_tile_area(selection_area);
	clipboard_corners.reserve(corner_area.width() * corner_area.height());
	for (int j = corner_area.top(); j <= corner_area.bottom(); ++j) {
		for (int i = corner_area.left(); i <= corner_area.right(); ++i) {
			clipboard_corners.push_back(map->terrain.get_corner(i, j));
		}
	}

	const PathingRect pathing_area = pathing_area_from_tile_area(selection_area);
	clipboard_pathing_cells_static.reserve(pathing_area.width() * pathing_area.height());
	for (int j = pathing_area.top(); j <= pathing_area.bottom(); ++j) {
		for (int i = pathing_area.left(); i <= pathing_area.right(); ++i) {
			clipboard_pathing_cells_static.push_back(map->pathing_map.pathing_cells_static[j * map->pathing_map.width + i]);
		}
	}

	has_clipboard = true;
	context->makeCurrent();
	rebuild_overlay_texture(clipboard_texture, clipboard_tile_mask, clipboard_tile_area.width(), clipboard_tile_area.height());
}

void TerrainBrush::cut_selection() {
	copy_selection();
}

void TerrainBrush::clear_selection() {
	has_selection_area = false;
	selection_tile_mask.clear();
}

void TerrainBrush::place_clipboard() {
	if (!has_clipboard) {
		return;
	}

	const TerrainRect destination_tile_area = TerrainRect(
		static_cast<int>(std::floor(input_handler.mouse_world.x)),
		static_cast<int>(std::floor(input_handler.mouse_world.y)),
		clipboard_tile_area.width(),
		clipboard_tile_area.height()
	).intersected({0, 0, map->terrain.width - 1, map->terrain.height - 1});

	if (destination_tile_area.width() <= 0 || destination_tile_area.height() <= 0) {
		return;
	}

	const int source_tile_x = destination_tile_area.x() - static_cast<int>(std::floor(input_handler.mouse_world.x));
	const int source_tile_y = destination_tile_area.y() - static_cast<int>(std::floor(input_handler.mouse_world.y));

	const TerrainRect destination_corner_area = corner_area_from_tile_area(destination_tile_area);
	const PathingRect destination_pathing_area = pathing_area_from_tile_area(destination_tile_area);
	const TerrainRect source_corner_area(source_tile_x, source_tile_y, destination_corner_area.width(), destination_corner_area.height());
	const PathingRect source_pathing_area(source_tile_x * 4, source_tile_y * 4, destination_pathing_area.width(), destination_pathing_area.height());

	map->world_undo.new_undo_group();

	auto terrain_undo = std::make_unique<TerrainGenericAction>();
	terrain_undo->area = destination_corner_area;
	terrain_undo->undo_type = TerrainUndoType::full;
	terrain_undo->old_corners.reserve(destination_corner_area.width() * destination_corner_area.height());
	terrain_undo->new_corners.reserve(destination_corner_area.width() * destination_corner_area.height());

	for (int j = 0; j < destination_corner_area.height(); ++j) {
		for (int i = 0; i < destination_corner_area.width(); ++i) {
			const int destination_x = destination_corner_area.x() + i;
			const int destination_y = destination_corner_area.y() + j;
			const int source_index = (source_corner_area.y() + j) * (clipboard_tile_area.width() + 1) + source_corner_area.x() + i;

			terrain_undo->old_corners.push_back(map->terrain.get_corner(destination_x, destination_y));
			if (corner_is_selected(
					clipboard_tile_mask,
					clipboard_tile_area.width(),
					clipboard_tile_area.height(),
					source_corner_area.x() + i,
					source_corner_area.y() + j
				)) {
				map->terrain.set_corner(destination_x, destination_y, clipboard_corners[source_index]);
			}
			terrain_undo->new_corners.push_back(map->terrain.get_corner(destination_x, destination_y));
		}
	}
	map->world_undo.add_undo_action(std::move(terrain_undo));

	auto pathing_undo = std::make_unique<PathingMapAction>();
	pathing_undo->area = destination_pathing_area;
	pathing_undo->old_pathing.reserve(destination_pathing_area.width() * destination_pathing_area.height());
	pathing_undo->new_pathing.reserve(destination_pathing_area.width() * destination_pathing_area.height());

	for (int j = 0; j < destination_pathing_area.height(); ++j) {
		for (int i = 0; i < destination_pathing_area.width(); ++i) {
			const int destination_x = destination_pathing_area.x() + i;
			const int destination_y = destination_pathing_area.y() + j;
			const int source_index = (source_pathing_area.y() + j) * (clipboard_tile_area.width() * 4) + source_pathing_area.x() + i;

			pathing_undo->old_pathing.push_back(map->pathing_map.pathing_cells_static[destination_y * map->pathing_map.width + destination_x]);
			if (tile_is_selected(
					clipboard_tile_mask,
					clipboard_tile_area.width(),
					(source_pathing_area.x() + i) / 4,
					(source_pathing_area.y() + j) / 4
				)) {
				map->pathing_map.pathing_cells_static[destination_y * map->pathing_map.width + destination_x] =
					clipboard_pathing_cells_static[source_index];
			}
			pathing_undo->new_pathing.push_back(map->pathing_map.pathing_cells_static[destination_y * map->pathing_map.width + destination_x]);
		}
	}
	map->world_undo.add_undo_action(std::move(pathing_undo));

	map->terrain.update_ground_heights(destination_corner_area);
	map->terrain.update_cliff_meshes(destination_corner_area);
	map->terrain.update_ground_textures(destination_corner_area);
	map->terrain.update_water(destination_corner_area);
	map->pathing_map.upload_static_pathing();
	map->terrain.update_minimap();
	map->units.update_area(
		TerrainRectF(
			destination_corner_area.x(),
			destination_corner_area.y(),
			destination_corner_area.width(),
			destination_corner_area.height()
		),
		map->terrain
	);

	selection_area = destination_tile_area;
	has_selection_area = true;
	selection_tile_mask.resize(destination_tile_area.width() * destination_tile_area.height());
	for (int y = 0; y < destination_tile_area.height(); ++y) {
		for (int x = 0; x < destination_tile_area.width(); ++x) {
			selection_tile_mask[y * destination_tile_area.width() + x] =
				clipboard_tile_mask[(source_tile_y + y) * clipboard_tile_area.width() + source_tile_x + x];
		}
	}
	context->makeCurrent();
	rebuild_overlay_texture(selection_texture, selection_tile_mask, selection_area.width(), selection_area.height());
}

void TerrainBrush::render_selector() const {}

void TerrainBrush::render_selection() const {
}

void TerrainBrush::render_clipboard() {
}

GLuint TerrainBrush::terrain_overlay_texture() const {
	if (mode == Mode::pasting && has_clipboard) {
		return clipboard_texture;
	}
	if (mode == Mode::selection && (selection_started || has_selection_area)) {
		return selection_texture;
	}
	return brush_texture;
}

bool TerrainBrush::show_terrain_overlay() const {
	if (mode == Mode::selection) {
		return selection_started || has_selection_area;
	}
	if (mode == Mode::pasting) {
		return has_clipboard;
	}
	return true;
}

bool TerrainBrush::has_active_operators() {
	for (TerrainOperator& op : terrain_operators) {
		if (op.is_enabled()) {
			return true;
		}
	}

	return false;
}

void TerrainBrush::apply_begin() {
	// do nothing if there are no active operators
	if (!has_active_operators()) {
		return;
	}

	const auto& terrain = map->terrain;
	const int width = terrain.width;
	const int height = terrain.height;

	const glm::ivec2 pos = get_unclipped_pos();
	TerrainRect area = TerrainRect(pos.x, pos.y, size.x / 4.f, size.y / 4.f).intersected({0, 0, width, height});
	updated_area = PathingRect();

	const int center_x = area.x() + area.width() * 0.5f;
	const int center_y = area.y() + area.height() * 0.5f;

	// setup for undo/redo — snapshot all corners
	map->world_undo.new_undo_group();
	old_corners_width = width;
	old_corners_height = height;
	old_corners.resize(width * height);
	for (int j = 0; j < height; j++) {
		for (int i = 0; i < width; i++) {
			old_corners[j * width + i] = terrain.get_corner(i, j);
		}
	}
	old_pathing_cells_static = map->pathing_map.pathing_cells_static;

	// apply all active operators
	for (TerrainOperator& op : terrain_operators) {
		if (op.is_enabled()) {
			op.apply_begin(area, center_x, center_y);
		}
	}
}

void TerrainBrush::apply(double frame_delta) {
	// do nothing if there are no active operators
	if (!has_active_operators()) {
		return;
	}

	auto& terrain = map->terrain;
	const int width = terrain.width;
	const int height = terrain.height;

	const glm::ivec2 pos = get_unclipped_pos();

	TerrainRect area = TerrainRect(pos.x, pos.y, size.x / 4.f, size.y / 4.f).intersected({0, 0, width, height});
	PathingRect affected_area = PathingRect();

	if (area.width() <= 0 || area.height() <= 0) {
		return;
	}

	// apply all active operators
	for (TerrainOperator& op : terrain_operators) {
		if (op.is_active) {
			affected_area =
				affected_area.united(op.apply(area, frame_delta)).intersected({0, 0, map->pathing_map.width, map->pathing_map.height});
		}
	}

	// entire area modified so far
	updated_area = updated_area.united(affected_area);

	// apply pathing
	constexpr uint8_t terrain_pathing_mask = PathingMap::unwalkable | PathingMap::unflyable | PathingMap::unbuildable
		| PathingMap::blight | PathingMap::unfloatable | PathingMap::unamphibious;
	for (size_t i = affected_area.x(); i <= affected_area.right(); i++) {
		for (size_t j = affected_area.y(); j <= affected_area.bottom(); j++) {
			map->pathing_map.pathing_cells_static[j * map->pathing_map.width + i] &= static_cast<uint8_t>(~terrain_pathing_mask);
			uint8_t mask = map->terrain.get_terrain_pathing(i, j, apply_tile_pathing, apply_cliff_pathing, apply_water_pathing);
			map->pathing_map.pathing_cells_static[j * map->pathing_map.width + i] |= mask;
		}
	}

	map->pathing_map.upload_static_pathing();

	if (height_operator.is_active || cliff_operator.is_active) {
		TerrainRectF object_area = updated_area.to_terrain_f();

		if (change_doodad_heights) {
			for (auto&& i : map->doodads.doodads) {
				if (!i.fixed_z && object_area.contains(i.position.x, i.position.y)) {
					if (std::find_if(
							pre_change_doodads.begin(),
							pre_change_doodads.end(),
							[i](const Doodad& doodad) {
								return doodad.creation_number == i.creation_number;
							}
						)
						== pre_change_doodads.end()) {
						pre_change_doodads.push_back(i);
					}
					i.snap_to_terrain(map->terrain);
					i.update(map->terrain);
					post_change_doodads[i.creation_number] = i;
				}
			}
		}

		map->units.update_area(object_area, map->terrain);
	}
}

void TerrainBrush::apply_end() {
	// do nothing if there are no active operators
	if (!has_active_operators()) {
		return;
	}

	WorldEditContext ctx {
		.terrain = map->terrain,
		.units = map->units,
		.doodads = map->doodads,
		.regions = map->regions,
		.brush = this,
		.pathing_map = map->pathing_map,
	};

	// apply all active operators
	for (TerrainOperator& op : terrain_operators) {
		if (op.is_active) {
			op.apply_end(ctx, updated_area);
		}
	}

	if (change_doodad_heights) {
		auto undo = std::make_unique<DoodadStateAction>();
		undo->old_doodads = pre_change_doodads;
		for (const auto& [id, doodad] : post_change_doodads) {
			undo->new_doodads.push_back(doodad);
		}
		pre_change_doodads.clear();
		post_change_doodads.clear();
		map->world_undo.add_undo_action(std::move(undo));
	}

	add_pathing_undo(ctx, updated_area);

	map->terrain.update_minimap();
}

/// Adds the undo to the current undo group
void TerrainBrush::add_terrain_undo(WorldEditContext& ctx, const TerrainRect& area, TerrainUndoType type) {
	auto undo_action = std::make_unique<TerrainGenericAction>();

	undo_action->area = area;
	undo_action->undo_type = type;

	// Copy old corners
	undo_action->old_corners.reserve(area.width() * area.height());
	for (int j = area.top(); j <= area.bottom(); j++) {
		for (int i = area.left(); i <= area.right(); i++) {
			undo_action->old_corners.push_back(old_corners[j * old_corners_width + i]);
		}
	}

	// Copy new corners
	undo_action->new_corners.reserve(area.width() * area.height());
	for (int j = area.top(); j <= area.bottom(); j++) {
		for (int i = area.left(); i <= area.right(); i++) {
			undo_action->new_corners.push_back(ctx.terrain.get_corner(i, j));
		}
	}

	map->world_undo.add_undo_action(std::move(undo_action));
}

void TerrainBrush::add_pathing_undo(WorldEditContext& ctx, const PathingRect& area) {
	auto undo_action = std::make_unique<PathingMapAction>();

	undo_action->area = area;
	const auto width = ctx.pathing_map.width;

	// Copy old corners
	undo_action->old_pathing.reserve(area.width() * area.height());
	for (int j = area.top(); j <= area.bottom(); j++) {
		for (int i = area.left(); i <= area.right(); i++) {
			undo_action->old_pathing.push_back(old_pathing_cells_static[j * width + i]);
		}
	}

	// Copy new corners
	undo_action->new_pathing.reserve(area.width() * area.height());
	for (int j = area.top(); j <= area.bottom(); j++) {
		for (int i = area.left(); i <= area.right(); i++) {
			undo_action->new_pathing.push_back(ctx.pathing_map.pathing_cells_static[j * width + i]);
		}
	}

	map->world_undo.add_undo_action(std::move(undo_action));
}

glm::ivec2 TerrainBrush::get_unclipped_pos() const {
	const glm::vec2 fpos = brush_type == Brush::Type::corner ? glm::vec2(input_handler.mouse_world) - size.x / 4.f / 2.f + 1.f
															 : glm::vec2(input_handler.mouse_world) - size.x / 4.f / 2.f + 0.5f;
	return glm::ivec2(glm::floor(fpos));
}
