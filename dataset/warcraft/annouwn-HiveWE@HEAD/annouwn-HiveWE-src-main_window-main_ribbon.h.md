---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/main_window/main_ribbon.h"
ref: HEAD
---

#pragma once

import QRibbon;

#include <array>
#include <QObject>
#include <QLabel>
#include <QResizeEvent>
#include <QShowEvent>
#include <QToolButton>

class MainRibbon : public QRibbon {
	Q_OBJECT

public:
	enum class HeaderTab {
		home,
		terrain,
		doodads,
		units,
		pathing,
		regions,
		view,
		map,
	};

	QRibbonButton* undo = new QRibbonButton;
	QRibbonButton* redo = new QRibbonButton;
	QRibbonButton* view_history = new QRibbonButton;


	QRibbonButton* copy = new QRibbonButton;
	QRibbonButton* paste = new QRibbonButton;

	QRibbonButton* units_visible = new QRibbonButton;
	QRibbonButton* doodads_visible = new QRibbonButton;
	QRibbonButton* pathing_visible = new QRibbonButton;
	QRibbonButton* brush_visible = new QRibbonButton;
	QRibbonButton* lighting_visible = new QRibbonButton;
	QRibbonButton* water_visible = new QRibbonButton;
	QRibbonButton* click_helpers_visible = new QRibbonButton;
	QRibbonButton* wireframe_visible = new QRibbonButton;
	QRibbonButton* debug_visible = new QRibbonButton;
	QRibbonButton* minimap_visible = new QRibbonButton;

	QRibbonButton* reset_camera = new QRibbonButton;

	QRibbonButton* map_description = new QRibbonButton;
	QRibbonButton* map_loading_screen = new QRibbonButton;
	QRibbonButton* map_options = new QRibbonButton;
	QRibbonButton* map_size_camera_bounds = new QRibbonButton;
	QRibbonButton* gameplay_constants = new QRibbonButton;
	QRibbonButton* item_tables = new QRibbonButton;
	QRibbonButton* scenario_properties = new QRibbonButton;

	QRibbonButton* import_heightmap = new QRibbonButton;
	QRibbonButton* change_tileset = new QRibbonButton;
	QRibbonButton* change_tile_pathing = new QRibbonButton;
	QRibbonButton* switch_warcraft = new QRibbonButton;

	QRibbonButton* trigger_editor = new QRibbonButton;
	QRibbonButton* object_editor = new QRibbonButton;
	QRibbonButton* model_editor = new QRibbonButton;
	QRibbonButton* asset_manager = new QRibbonButton;
	QRibbonButton* tooltip_editor = new QRibbonButton;
	QRibbonButton* map_protector = new QRibbonButton;
	QRibbonButton* config = new QRibbonButton;
	QRibbonButton* export_objects = new QRibbonButton;
	QRibbonButton* import_objects = new QRibbonButton;

	QRibbonButton* terrain_palette = new QRibbonButton;
	QRibbonButton* doodad_palette = new QRibbonButton;
	QRibbonButton* unit_palette = new QRibbonButton;
	QRibbonButton* pathing_palette = new QRibbonButton;
	QRibbonButton* region_palette = new QRibbonButton;

	QToolButton* new_map = new QToolButton;
	QToolButton* open_map_mpq = new QToolButton;
	QToolButton* open_map_folder = new QToolButton;
	std::array<QToolButton*, 3> recent_maps = {new QToolButton, new QToolButton, new QToolButton};

	QToolButton* save_map = new QToolButton;
	QToolButton* save_map_as = new QToolButton;
	QToolButton* export_mpq = new QToolButton;

	QToolButton* test_map = new QToolButton;
	QToolButton* settings = new QToolButton;
	QToolButton* exit = new QToolButton;
	QToolButton* quick_search = new QToolButton;
	QLabel* map_title = new QLabel;
	QLabel* map_meta = new QLabel;
	QLabel* map_badge = new QLabel;

	MainRibbon(QWidget* parent);
	~MainRibbon();

	void set_map_context(const QString& title, const QString& meta, const QString& badge);
	void show_tab(HeaderTab tab);

protected:
	void resizeEvent(QResizeEvent* event) override;
	void showEvent(QShowEvent* event) override;

private:
	void sync_file_corner_geometry();

	int home_tab_index = -1;
	int terrain_tab_index = -1;
	int doodads_tab_index = -1;
	int units_tab_index = -1;
	int pathing_tab_index = -1;
	int regions_tab_index = -1;
	int view_tab_index = -1;
	int map_tab_index = -1;
};
