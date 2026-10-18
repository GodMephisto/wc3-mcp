---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/brush/region_brush.h"
ref: HEAD
---

#pragma once

#include "brush.h"

#include <QColor>

import Regions;
import RegionsUndo;

class RegionBrush : public Brush {
  public:
	enum class DragHandle {
		none,
		move,
		left,
		right,
		top,
		bottom,
		top_left,
		top_right,
		bottom_left,
		bottom_right
	};

	RegionBrush();

	void mouse_press_event(QMouseEvent* event, double frame_delta) override;
	void mouse_move_event(QMouseEvent* event, double frame_delta) override;
	void mouse_release_event(QMouseEvent* event) override;
	void render_brush() override;
	void render_selection() const override;
	void delete_selection() override;
	void clear_selection() override;
	void apply(double frame_delta) override;

	int selected_index() const;
	Region* selected_region();
	const Region* selected_region() const;

	void select_region(int index);
	void create_region_at_cursor();
	void set_selected_name(const QString& name);
	void set_selected_weather_id(const QString& weather_id);
	void set_selected_ambient_id(const QString& ambient_id);
	void set_selected_color(const QColor& color);
	Qt::CursorShape cursor_shape() const;

  private:
	int selection_index = -1;
	DragHandle drag_handle = DragHandle::none;
	glm::vec2 drag_start_mouse = glm::vec2(0.f);
	Region drag_start_region;
	bool region_geometry_changed(const Region& lhs, const Region& rhs) const;
	int region_at_point(const glm::vec2& point) const;

	bool contains(const Region& region, const glm::vec2& point) const;
	DragHandle resolve_drag_handle(const Region& region, const glm::vec2& point) const;
	void clamp_region(Region& region) const;
	Region make_region(glm::vec2 start, glm::vec2 end) const;
	std::string next_region_name() const;
	int next_creation_number() const;
	void emit_selection_change();
	void draw_region(const Region& region, const glm::u8vec4& color, bool filled) const;
};
