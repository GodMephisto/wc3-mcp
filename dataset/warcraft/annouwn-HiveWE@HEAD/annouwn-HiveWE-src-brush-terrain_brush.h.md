---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/brush/terrain_brush.h"
ref: HEAD
---

#pragma once

#include "brush.h"
#include "terrain_operators.h"

import Doodad;
import Terrain;
import TerrainUndo;
import WorldUndoManager;
import Rects;

class TerrainBrush: public Brush {
	// Friend declarations for terrain operators
	friend class TerrainOperator;
	friend class HeightOperator;
	friend class TextureOperator;
	friend class CliffOperator;
	friend class CellOperator;

  public:
	bool apply_tile_pathing = true;
	bool apply_cliff_pathing = true;
	bool apply_water_pathing = true;
	bool deform_water = false;
	bool deform_ground = true;

	bool enforce_water_height_limits = true;
	bool change_doodad_heights = true;
	bool relative_cliff_heights = false;

	bool dragging = false;
	bool dragged = false;

	TerrainBrush();
	~TerrainBrush() override;

	glm::vec2 get_position() const override;
	void mouse_release_event(QMouseEvent* event) override;
	void mouse_press_event(QMouseEvent* event, double frame_delta) override;
	void mouse_move_event(QMouseEvent* event, double frame_delta) override;

	void apply_begin() override;
	void apply(double frame_delta) override;
	void apply_end() override;
	void copy_selection() override;
	void cut_selection() override;
	void clear_selection() override;
	void place_clipboard() override;
	void render_selector() const override;
	void render_selection() const override;
	void render_clipboard() override;
	GLuint terrain_overlay_texture() const override;
	bool show_terrain_overlay() const override;

	void add_terrain_undo(WorldEditContext& ctx, const TerrainRect& area, TerrainUndoType type);
	void add_pathing_undo(WorldEditContext& ctx, const PathingRect& area);

	// all terrain operators
	CliffOperator cliff_operator;
	HeightOperator height_operator;
	TextureOperator texture_operator;
	CellOperator cell_operator;

	/// Deactivates the specified operator
	void deactivate_operator(TerrainOperator& target);

	/// Activates the target terrain operator. Also disables all
	/// active operators which cannot be used simultaneously
	void activate_operator(TerrainOperator& target);

	/// Returns true if two terrain operators are allowed to be active simultaneously
	bool can_combine(const TerrainOperator& a, const TerrainOperator& b) const;

	/// Returns the unclipped top-left corner of the brush area in terrain coordinates
	glm::ivec2 get_unclipped_pos() const;

  private:
	/// Area which was modified in the last operation in pathing map resolution
	PathingRect updated_area;

	// undo/redo stuff
	std::vector<Doodad> pre_change_doodads;
	std::map<int, Doodad> post_change_doodads;

	std::vector<Corner> old_corners;
	int old_corners_width = 0;
	int old_corners_height = 0;
	std::vector<uint8_t> old_pathing_cells_static;

	bool has_selection_area = false;
	TerrainRect selection_area;
	std::vector<uint8_t> selection_tile_mask;
	GLuint selection_texture = 0;

	bool has_clipboard = false;
	TerrainRect clipboard_tile_area;
	std::vector<Corner> clipboard_corners;
	std::vector<uint8_t> clipboard_pathing_cells_static;
	std::vector<uint8_t> clipboard_tile_mask;
	GLuint clipboard_texture = 0;

	std::array<std::reference_wrapper<TerrainOperator>, 4> terrain_operators;

	// checks if there is an active operator
	bool has_active_operators();
};
