---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/brush/brush.h"
ref: HEAD
---

#pragma once

#include <QMouseEvent>
#include <QKeyEvent>

#include <glm/glm.hpp>

#include <cstdint>
#include <string_view>
#include <vector>

struct BrushOverlayDraw {
	std::uint32_t first_vertex = 0;
	std::uint32_t vertex_count = 0;
	std::uint32_t primitive_type = 0; // 0 = line strip, 1 = triangles
	std::uint32_t style = 0; // 0 = solid color, 1 = ring mask
};

struct BrushOverlayFrame {
	std::vector<glm::vec4> positions;
	std::vector<glm::vec2> uvs;
	std::vector<glm::vec4> colors;
	std::vector<BrushOverlayDraw> draws;

	void append_vertex(const glm::vec3& position, const glm::vec2& uv, const glm::vec4& color) {
		positions.emplace_back(position, 1.0f);
		uvs.push_back(uv);
		colors.push_back(color);
	}

	void append_selector(const glm::vec3& start, const glm::vec3& end) {
		const std::uint32_t first_vertex = static_cast<std::uint32_t>(positions.size());
		const glm::vec4 color(0.0f, 1.0f, 0.0f, 1.0f);
		append_vertex(glm::vec3(start.x, start.y, start.z), glm::vec2(0.0f), color);
		append_vertex(glm::vec3(end.x, start.y, start.z), glm::vec2(0.0f), color);
		append_vertex(glm::vec3(end.x, end.y, start.z), glm::vec2(0.0f), color);
		append_vertex(glm::vec3(start.x, end.y, start.z), glm::vec2(0.0f), color);
		append_vertex(glm::vec3(start.x, start.y, start.z), glm::vec2(0.0f), color);
		draws.push_back(BrushOverlayDraw {
			.first_vertex = first_vertex,
			.vertex_count = 5,
			.primitive_type = 0,
			.style = 0,
		});
	}

	void append_selection_circle(const glm::vec3& position, const float scale) {
		const std::uint32_t first_vertex = static_cast<std::uint32_t>(positions.size());
		const glm::vec4 color(0.0f, 1.0f, 0.0f, 0.75f);
		const glm::vec3 min = position - glm::vec3(scale * 0.5f, scale * 0.5f, 0.0f);
		const glm::vec3 max = min + glm::vec3(scale, scale, 0.0f);
		append_vertex(glm::vec3(min.x, min.y, position.z), glm::vec2(0.0f, 0.0f), color);
		append_vertex(glm::vec3(min.x, max.y, position.z), glm::vec2(0.0f, 1.0f), color);
		append_vertex(glm::vec3(max.x, min.y, position.z), glm::vec2(1.0f, 0.0f), color);
		append_vertex(glm::vec3(max.x, min.y, position.z), glm::vec2(1.0f, 0.0f), color);
		append_vertex(glm::vec3(min.x, max.y, position.z), glm::vec2(0.0f, 1.0f), color);
		append_vertex(glm::vec3(max.x, max.y, position.z), glm::vec2(1.0f, 1.0f), color);
		draws.push_back(BrushOverlayDraw {
			.first_vertex = first_vertex,
			.vertex_count = 6,
			.primitive_type = 1,
			.style = 1,
		});
	}
};

class Brush: public QObject {
	Q_OBJECT

  public:
	enum class Shape {
		square,
		circle,
		diamond
	};

	enum class Mode {
		placement,
		selection,
		pasting
	};

	enum class Type {
		corner,
		cell
	};

	Brush();

	virtual glm::vec2 get_position() const;
	virtual void set_size(glm::ivec2 size);
	virtual void set_shape(Shape shape);
	virtual void increase_size(int size);
	virtual void decrease_size(int size);
	virtual bool contains(glm::ivec2 pos) const;

	virtual void switch_mode();

	Mode get_mode() {
		return mode;
	}

	[[nodiscard]]
	bool selection_in_progress() const {
		return selection_started;
	}

	[[nodiscard]]
	glm::vec3 selection_start_position() const {
		return selection_start;
	}

	virtual void key_press_event(QKeyEvent* event);

	virtual void key_release_event(QKeyEvent* event) {}

	virtual void mouse_move_event(QMouseEvent* event, double frame_delta);
	virtual void mouse_press_event(QMouseEvent* event, double frame_delta);
	virtual void mouse_release_event(QMouseEvent* event);

	virtual void delete_selection() {}

	virtual void copy_selection() {}

	virtual void cut_selection() {}

	virtual void clear_selection() {}

	virtual void place_clipboard() {}

	virtual void clear_clipboard() {}

	void render();
	virtual void append_selection_overlay(BrushOverlayFrame& frame) const;

	virtual void render_clipboard() {}

	virtual void render_brush();

	[[nodiscard]]
	const std::vector<glm::u8vec4>& brush_mask_pixels() const {
		return brush_mask;
	}

	[[nodiscard]]
	glm::ivec2 brush_mask_size() const {
		return brush_mask_size_pixels;
	}

	[[nodiscard]]
	std::uint64_t brush_mask_revision() const {
		return brush_mask_revision_value;
	}

	virtual bool can_place() {
		return true;
	}

	virtual void apply_begin() {}

	virtual void apply(double frame_delta) = 0;

	virtual void apply_end() {}

  protected:
	Shape shape = Shape::circle;
	Mode mode = Mode::placement;
	Mode return_mode = Mode::placement;
	Type brush_type = Type::corner;

	/// The color used to render the brush which currently mimics the WC3 default
	glm::u8vec4 brush_color = {0, 255, 0, 128};

	/// How many quarter tiles fit into one size unit for this brush. Terrain brush is 4, pathing brush is 1.
	int size_granularity = 1;
	/// The granularity of the grid that the position will be snapped to. There will be 1 / position_granularity cells in a tile
	float position_granularity = 1.f;
	/// Size in 1/4ths of a tile (corresponding to the pathing map tile size which is the smallest)
	glm::ivec2 size = glm::ivec2(1);

	bool selection_started = false;
	glm::vec3 selection_start;

	void set_brush_mask(glm::ivec2 mask_size, std::vector<glm::u8vec4> pixels);

	std::vector<glm::u8vec4> brush_mask;
	glm::ivec2 brush_mask_size_pixels = glm::ivec2(0);
	std::uint64_t brush_mask_revision_value = 0;

  public slots:

	virtual void unselect_id(std::string_view id) {}

  signals:
	void selection_changed();
};
