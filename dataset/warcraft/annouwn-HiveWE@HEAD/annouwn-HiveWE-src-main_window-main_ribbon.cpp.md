---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/main_window/main_ribbon.cpp"
ref: HEAD
---

#include "main_ribbon.h"

#include <algorithm>
#include <QFrame>
#include <QHBoxLayout>
#include <QStyle>
#include <QTabBar>
#include <QVBoxLayout>

MainRibbon::MainRibbon(QWidget* parent) : QRibbon(parent) {
	auto set_tooltip = [](QWidget* widget, QString text) {
		widget->setToolTip(std::move(text));
		widget->setStatusTip(widget->toolTip());
	};

	auto style_ribbon_button = [](QToolButton* button, const Qt::ToolButtonStyle style = Qt::ToolButtonTextUnderIcon) {
		button->setToolButtonStyle(style);
		button->setIconSize({ 24, 24 });
		button->setMinimumSize(88, 68);
		button->setSizePolicy(QSizePolicy::Preferred, QSizePolicy::Preferred);
	};

	auto mirror_button = [style_ribbon_button](QAbstractButton* source, const QString& tooltip = QString()) {
		auto* proxy = new QToolButton;
		proxy->setText(source->text());
		proxy->setIcon(source->icon());
		proxy->setCheckable(source->isCheckable());
		proxy->setChecked(source->isChecked());
		proxy->setEnabled(source->isEnabled());
		proxy->setToolTip(tooltip.isEmpty() ? source->toolTip() : tooltip);
		proxy->setStatusTip(proxy->toolTip());
		style_ribbon_button(proxy);

		QObject::connect(proxy, &QToolButton::clicked, source, &QAbstractButton::click);
		QObject::connect(source, &QAbstractButton::toggled, proxy, &QToolButton::setChecked);

		return proxy;
	};

	auto make_section = [](const QString& title) {
		auto* section = new QRibbonSection;
		section->setText(title);
		return section;
	};

	setDocumentMode(true);
	tabBar()->setExpanding(false);
	tabBar()->setUsesScrollButtons(true);
	tabBar()->setDrawBase(false);
	setMovable(false);

	setStyleSheet(
		"MainRibbon {"
		"background: rgb(33, 39, 47);"
		"border: none;"
		"border-bottom: 1px solid rgba(255, 255, 255, 16);"
		"}"
		"MainRibbon::left-corner {"
		"background: rgb(33, 39, 47);"
		"border: none;"
		"}"
		"MainRibbon::pane { border: none; }"
		"MainRibbon QTabBar { background: rgb(33, 39, 47); border: none; color: rgb(180, 188, 198); }"
		"MainRibbon QTabBar::tab {"
		"background: transparent;"
		"color: rgb(180, 188, 198);"
		"padding: 10px 16px;"
		"margin: 6px 4px 0 0;"
		"border: none;"
		"border-top-left-radius: 10px;"
		"border-top-right-radius: 10px;"
		"font-size: 12px;"
		"font-weight: 700;"
		"}"
		"MainRibbon QTabBar::tab:selected {"
		"background: rgba(52, 60, 72, 220);"
		"color: rgb(245, 248, 252);"
		"border: none;"
		"}"
		"MainRibbon QTabBar::tab:hover:!selected { color: rgb(224, 229, 235); }"
		"QRibbonTab { background: rgba(33, 39, 47, 224); }"
		"QFrame#seperator { background: rgba(255, 255, 255, 16); max-width: 1px; margin: 10px 0 8px 0; }"
		"QRibbonSection QLabel { max-height: 0px; min-height: 0px; padding: 0px; margin: 0px; font-size: 0px; }"
		"MainRibbon QToolButton {"
		"background: transparent;"
		"border: 1px solid transparent;"
		"border-radius: 10px;"
		"padding: 6px 8px;"
		"color: rgb(232, 236, 241);"
		"}"
		"MainRibbon QToolButton:hover { background: rgba(255, 255, 255, 10); border-color: rgba(255, 255, 255, 16); }"
		"MainRibbon QToolButton:checked { background: rgba(92, 145, 224, 188); border-color: rgba(130, 184, 255, 235); }"
		"QToolButton#ribbonQuickAction {"
		"background: rgba(32, 38, 45, 235);"
		"border: 1px solid rgba(255, 255, 255, 16);"
		"border-radius: 10px;"
		"padding: 7px 12px;"
		"font-size: 12px;"
		"font-weight: 700;"
		"}"
		"QToolButton#ribbonQuickSearch {"
		"background: rgba(24, 29, 35, 240);"
		"border: 1px solid rgba(255, 255, 255, 14);"
		"border-radius: 11px;"
		"padding: 8px 14px;"
		"color: rgb(205, 214, 224);"
		"text-align: left;"
		"}"
		"QWidget#ribbonFileCorner {"
		"background: rgb(33, 39, 47);"
		"margin: 0px;"
		"padding: 0px;"
		"}"
		"QWidget#ribbonHeaderCorner { background: rgba(33, 39, 47, 224); }"
		"#ribbonFileButton {"
		"background: rgb(33, 39, 47);"
		"border: none;"
		"border-radius: 6px;"
		"color: rgb(200, 210, 220);"
		"padding: 8px 14px;"
		"font-size: 12px;"
		"font-weight: 700;"
		"}"
		"#ribbonFileButton:hover { background: rgb(52, 60, 72); }"
		"#ribbonFileButton:pressed { background: rgb(40, 48, 58); }"
		"#ribbonFileButton::menu-indicator { image: none; }"
		"QWidget#ribbonMapContext { background: transparent; }"
		"QLabel#ribbonMapTitle { color: rgb(245, 248, 252); font-size: 15px; font-weight: 800; }"
		"QLabel#ribbonMapMeta { color: rgb(156, 166, 178); font-size: 11px; }"
		"QLabel#ribbonMapBadge {"
		"background: rgba(85, 134, 96, 188);"
		"border: 1px solid rgba(146, 206, 159, 140);"
		"border-radius: 999px;"
		"padding: 3px 8px;"
		"color: rgb(242, 248, 244);"
		"font-size: 11px;"
		"font-weight: 700;"
		"}"
	);

	if (auto* file_btn = findChild<QRibbonFileButton*>("ribbonFileButton")) {
		file_btn->setPopupMode(QToolButton::InstantPopup);
		file_btn->setSizePolicy(QSizePolicy::Fixed, QSizePolicy::Fixed);
		file_btn->setStyleSheet(
			"#ribbonFileButton {"
			"background: rgba(52, 60, 72, 220);"
			"border: none;"
			"border-top-left-radius: 10px;"
			"border-top-right-radius: 10px;"
			"border-bottom-left-radius: 0px;"
			"border-bottom-right-radius: 0px;"
			"color: rgb(245, 248, 252);"
			"padding: 10px 16px;"
			"font-size: 12px;"
			"font-weight: 700;"
			"}"
			"#ribbonFileButton:hover { background: rgb(60, 69, 82); }"
			"#ribbonFileButton:pressed { background: rgb(40, 48, 58); }"
			"#ribbonFileButton::menu-indicator { image: none; width: 0; }"
		);
		file_btn->menu->setStyleSheet(
			"QRibbonMenu {"
			"background: rgb(33, 39, 47);"
			"border: 1px solid rgba(255, 255, 255, 16);"
			"}"
			"QRibbonMenu QToolButton {"
			"background: rgb(33, 39, 47);"
			"border: 1px solid transparent;"
			"color: rgb(232, 236, 241);"
			"padding-left: 10px;"
			"padding-right: 10px;"
			"height: 44px;"
			"width: 216px;"
			"}"
			"QRibbonMenu QToolButton:hover {"
			"background: rgb(52, 60, 72);"
			"}"
			"QRibbonMenu QToolButton:pressed {"
			"background: rgb(40, 48, 58);"
			"}"
			"QRibbonMenu QFrame {"
			"background: rgba(255, 255, 255, 16);"
			"border: none;"
			"max-height: 1px;"
			"margin-left: 8px;"
			"margin-right: 8px;"
			"}"
		);
	}

	auto* top_right = new QWidget(this);
	top_right->setObjectName("ribbonHeaderCorner");
	auto* top_right_layout = new QHBoxLayout(top_right);
	top_right_layout->setContentsMargins(12, 8, 8, 4);
	top_right_layout->setSpacing(10);

	auto* map_context = new QWidget(top_right);
	map_context->setObjectName("ribbonMapContext");
	auto* map_context_layout = new QVBoxLayout(map_context);
	map_context_layout->setContentsMargins(0, 0, 0, 0);
	map_context_layout->setSpacing(4);

	auto* title_row = new QHBoxLayout;
	title_row->setContentsMargins(0, 0, 0, 0);
	title_row->setSpacing(8);

	map_title->setObjectName("ribbonMapTitle");
	map_meta->setObjectName("ribbonMapMeta");
	map_badge->setObjectName("ribbonMapBadge");
	map_badge->setAlignment(Qt::AlignCenter);

	title_row->addWidget(map_title, 0, Qt::AlignVCenter);
	title_row->addWidget(map_badge, 0, Qt::AlignVCenter);
	title_row->addStretch(1);

	map_context_layout->addLayout(title_row);
	map_context_layout->addWidget(map_meta);

	auto* action_row = new QHBoxLayout;
	action_row->setContentsMargins(0, 0, 0, 0);
	action_row->setSpacing(8);

	QToolButton* quick_open = mirror_button(open_map_folder, "Open a map folder. Shortcut: Ctrl+O");
	QToolButton* quick_save = mirror_button(save_map, "Save the current map. Shortcut: Ctrl+S");
	QToolButton* quick_test = mirror_button(test_map, "Save and launch Warcraft III test mode.");

	for (auto* button : { quick_open, quick_save, quick_test }) {
		button->setObjectName("ribbonQuickAction");
		button->setToolButtonStyle(Qt::ToolButtonTextBesideIcon);
		button->setIconSize({ 18, 18 });
		button->setMinimumSize(0, 34);
		button->setSizePolicy(QSizePolicy::Minimum, QSizePolicy::Fixed);
	}

	quick_search->setObjectName("ribbonQuickSearch");
	quick_search->setText("Search actions  Shift Shift");
	quick_search->setIcon(style()->standardIcon(QStyle::SP_FileDialogContentsView));
	quick_search->setToolButtonStyle(Qt::ToolButtonTextBesideIcon);
	quick_search->setAutoRaise(false);
	quick_search->setMinimumHeight(36);
	quick_search->setToolTip("Open global search. Shortcut: Double Shift");
	quick_search->setStatusTip(quick_search->toolTip());

	action_row->addWidget(quick_open);
	action_row->addWidget(quick_save);
	action_row->addWidget(quick_test);
	action_row->addWidget(quick_search);

	top_right_layout->addWidget(map_context, 1);
	top_right_layout->addLayout(action_row);

	setCornerWidget(top_right, Qt::TopRightCorner);

	set_map_context("No map loaded", "Open a map folder or MPQ to start editing.", "Idle");

	// File menu actions
	new_map->setText("New Map");
	new_map->setIcon(QIcon("data/icons/ribbon/new.ico"));
	new_map->setIconSize({ 32, 32 });
	new_map->setToolButtonStyle(Qt::ToolButtonTextBesideIcon);
	addMenuItem(new_map);

	open_map_folder->setText("Open Map (Folder)");
	open_map_folder->setIcon(QIcon("data/icons/ribbon/open.png"));
	open_map_folder->setIconSize({ 32, 32 });
	open_map_folder->setToolButtonStyle(Qt::ToolButtonTextBesideIcon);
	addMenuItem(open_map_folder);

	open_map_mpq->setText("Open Map (MPQ)");
	open_map_mpq->setIcon(QIcon("data/icons/ribbon/open.png"));
	open_map_mpq->setIconSize({ 32, 32 });
	open_map_mpq->setToolButtonStyle(Qt::ToolButtonTextBesideIcon);
	addMenuItem(open_map_mpq);

	for (auto* recent_map : recent_maps) {
		recent_map->setIcon(QIcon("data/icons/ribbon/open.png"));
		recent_map->setIconSize({ 32, 32 });
		recent_map->setToolButtonStyle(Qt::ToolButtonTextBesideIcon);
		recent_map->hide();
		addMenuItem(recent_map);
	}

	save_map->setText("Save Map");
	save_map->setIcon(QIcon("data/icons/ribbon/save.png"));
	save_map->setIconSize({ 32, 32 });
	save_map->setToolButtonStyle(Qt::ToolButtonTextBesideIcon);
	addMenuItem(save_map);

	save_map_as->setText("Save Map as");
	save_map_as->setIcon(QIcon("data/icons/ribbon/saveas.png"));
	save_map_as->setIconSize({ 32, 32 });
	save_map_as->setToolButtonStyle(Qt::ToolButtonTextBesideIcon);
	addMenuItem(save_map_as);

	export_mpq->setText("Export MPQ");
	export_mpq->setIcon(QIcon("data/icons/ribbon/saveas.png"));
	export_mpq->setIconSize({ 32, 32 });
	export_mpq->setToolButtonStyle(Qt::ToolButtonTextBesideIcon);
	addMenuItem(export_mpq);

	test_map->setText("Test Map");
	test_map->setIcon(QIcon("data/icons/ribbon/test.ico"));
	test_map->setIconSize({ 32, 32 });
	test_map->setToolButtonStyle(Qt::ToolButtonTextBesideIcon);
	addMenuItem(test_map);

	settings->setText("Settings");
	settings->setIcon(QIcon("data/icons/ribbon/options.png"));
	settings->setIconSize({ 32, 32 });
	settings->setToolButtonStyle(Qt::ToolButtonTextBesideIcon);
	addMenuItem(settings);

	addMenuSeperator();

	exit->setText("Exit");
	exit->setIcon(QIcon("data/icons/ribbon/exit.ico"));
	exit->setIconSize({ 32, 32 });
	exit->setToolButtonStyle(Qt::ToolButtonTextBesideIcon);
	addMenuItem(exit);

	quick_open->setText("Open");
	quick_open->setIcon(open_map_folder->icon());
	quick_save->setText("Save");
	quick_save->setIcon(save_map->icon());
	quick_test->setText("Test Map");
	quick_test->setIcon(test_map->icon());

	// Shared action button styling
	for (auto* button : {
			 undo,
			 redo,
			 trigger_editor,
			 object_editor,
			 model_editor,
			 asset_manager,
			 tooltip_editor,
			 map_protector,
			 config,
			 export_objects,
			 import_objects,
			 terrain_palette,
			 doodad_palette,
			 unit_palette,
			 pathing_palette,
			 region_palette,
			 units_visible,
			 doodads_visible,
			 pathing_visible,
			 brush_visible,
			 lighting_visible,
			 water_visible,
			 click_helpers_visible,
			 wireframe_visible,
			 debug_visible,
			 minimap_visible,
			 reset_camera,
			 map_description,
			 map_loading_screen,
			 map_options,
			 gameplay_constants,
			 item_tables,
			 scenario_properties,
			 import_heightmap,
			 change_tileset,
			 change_tile_pathing,
			 switch_warcraft,
		 }) {
		style_ribbon_button(button);
	}

	undo->setIcon(QIcon("data/icons/ribbon/undo.png"));
	undo->setText("Undo");
	set_tooltip(undo, "Undo the last edit. Shortcut: Ctrl+Z");

	redo->setIcon(QIcon("data/icons/ribbon/redo.png"));
	redo->setText("Redo");
	set_tooltip(redo, "Redo the last undone edit. Shortcut: Ctrl+Y");

	trigger_editor->setIcon(QIcon("Data/Icons/Ribbon/triggereditor.png"));
	trigger_editor->setText("Trigger\nEditor");

	object_editor->setIcon(QIcon("data/icons/ribbon/objecteditor.png"));
	object_editor->setText("Object\nEditor");

	model_editor->setIcon(QIcon("data/icons/ribbon/model_editor.png"));
	model_editor->setText("Model\nEditor");

	asset_manager->setIcon(QIcon("data/icons/ribbon/asset_manager.png"));
	asset_manager->setText("Asset\nManager");

	tooltip_editor->setIcon(QIcon("data/icons/ribbon/objecteditor.png"));
	tooltip_editor->setText("Tooltip\nEditor");
	set_tooltip(tooltip_editor, "Open the Tooltip Editor to edit unit, item, and ability tooltips.");

	map_protector->setIcon(QIcon("data/icons/ribbon/saveas.png"));
	map_protector->setText("Map\nProtector");
	set_tooltip(map_protector, "Export the current map as a hardened, protected copy. The original map is never modified.");

	config->setIcon(QIcon("data/icons/ribbon/options.png"));
	config->setText("Config");
	config->setToolTip("Open the shortcut and behavior reference.");

	export_objects->setIcon(QIcon("data/icons/ribbon/saveas.png"));
	export_objects->setText("Export\nObjects");
	export_objects->setToolTip("Export Object Editor data to a portable binary or text package.");

	import_objects->setIcon(QIcon("data/icons/ribbon/open.png"));
	import_objects->setText("Import\nObjects");
	import_objects->setToolTip("Import Object Editor data. For text export, select the folder that contains manifest.json and the table/ directory.");

	terrain_palette->setIcon(QIcon("data/icons/ribbon/heightmap.png"));
	terrain_palette->setText("Terrain");
	set_tooltip(terrain_palette, "Show or hide the terrain sidebar. Shortcut: T");

	doodad_palette->setIcon(QIcon("data/icons/ribbon/doodads.png"));
	doodad_palette->setText("Doodads");
	set_tooltip(doodad_palette, "Show or hide the doodad sidebar. Shortcut: D");

	unit_palette->setIcon(QIcon("data/icons/ribbon/units.png"));
	unit_palette->setText("Units");
	set_tooltip(unit_palette, "Show or hide the unit sidebar. Shortcut: U");

	pathing_palette->setIcon(QIcon("data/icons/ribbon/pathing.png"));
	pathing_palette->setText("Pathing");
	set_tooltip(pathing_palette, "Show or hide the pathing sidebar. Shortcut: P");

	region_palette->setIcon(QIcon("data/icons/ribbon/select.png"));
	region_palette->setText("Regions");
	set_tooltip(region_palette, "Show or hide the region sidebar. Shortcut: R");

	units_visible->setIcon(QIcon("data/icons/ribbon/units.png"));
	units_visible->setText("Units");
	units_visible->setCheckable(true);
	units_visible->setChecked(true);
	set_tooltip(units_visible, "Show or hide units.\nShortcut: Ctrl+U");

	doodads_visible->setIcon(QIcon("data/icons/ribbon/doodads.png"));
	doodads_visible->setText("Doodads");
	doodads_visible->setCheckable(true);
	doodads_visible->setChecked(true);
	set_tooltip(doodads_visible, "Show or hide doodads.\nShortcut: Ctrl+D");

	pathing_visible->setIcon(QIcon("data/icons/ribbon/pathing.png"));
	pathing_visible->setText("Pathing");
	pathing_visible->setCheckable(true);
	set_tooltip(pathing_visible, "Show or hide the pathing overlay.\nShortcut: Ctrl+P");

	brush_visible->setIcon(QIcon("data/icons/ribbon/brush.png"));
	brush_visible->setText("Brush");
	brush_visible->setCheckable(true);
	brush_visible->setChecked(true);
	set_tooltip(brush_visible, "Show or hide the active brush overlay.");

	lighting_visible->setIcon(QIcon("data/icons/ribbon/lighting.png"));
	lighting_visible->setText("Lighting");
	lighting_visible->setCheckable(true);
	lighting_visible->setChecked(true);
	set_tooltip(lighting_visible, "Turn scene lighting on or off.\nShortcut: Ctrl+L");

	water_visible->setIcon(QIcon("data/icons/ribbon/water.png"));
	water_visible->setText("Water");
	water_visible->setCheckable(true);
	water_visible->setChecked(true);
	set_tooltip(water_visible, "Show or hide water.\nShortcut: Ctrl+W");

	click_helpers_visible->setIcon(QIcon("data/icons/ribbon/click_helpers.png"));
	click_helpers_visible->setText("Click\nHelpers");
	click_helpers_visible->setCheckable(true);
	click_helpers_visible->setChecked(false);
	set_tooltip(click_helpers_visible, "Show or hide click helper markers.\nShortcut: Ctrl+I");

	wireframe_visible->setIcon(QIcon("data/icons/ribbon/wireframe.png"));
	wireframe_visible->setText("Wireframe");
	wireframe_visible->setCheckable(true);
	set_tooltip(wireframe_visible, "Show or hide wireframe rendering.\nShortcut: Ctrl+T");

	debug_visible->setIcon(QIcon("data/icons/ribbon/debug.png"));
	debug_visible->setText("Debug");
	debug_visible->setCheckable(true);
	set_tooltip(debug_visible, "Show or hide debug rendering.\nShortcut: F3");

	minimap_visible->setIcon(QIcon("data/icons/ribbon/minimap.png"));
	minimap_visible->setText("Minimap");
	minimap_visible->setCheckable(true);
	minimap_visible->setChecked(false);
	set_tooltip(minimap_visible, "Show or hide the minimap.");

	reset_camera->setIcon(QIcon("data/icons/ribbon/reset.png"));
	reset_camera->setText("Reset");
	set_tooltip(reset_camera, "Reset the camera to its default view.\nShortcut: Ctrl+Shift+C");

	map_description->setIcon(QIcon("data/icons/ribbon/description.png"));
	map_description->setText("Description");

	map_loading_screen->setIcon(QIcon("data/icons/ribbon/loading.png"));
	map_loading_screen->setText("Loading\nScreen");

	map_options->setIcon(QIcon("data/icons/ribbon/options.png"));
	map_options->setText("Options");

	gameplay_constants->setIcon(QIcon("data/icons/ribbon/options.png"));
	gameplay_constants->setText("Gameplay\nConstants");

	item_tables->setIcon(QIcon("data/icons/ribbon/objecteditor.png"));
	item_tables->setText("Item\nTables");
	set_tooltip(item_tables, "Open the map item tables editor.");

	scenario_properties->setIcon(QIcon("data/icons/ribbon/units.png"));
	scenario_properties->setText("Players /\nForces");
	set_tooltip(scenario_properties, "Open the Players and Forces editor.");

	import_heightmap->setIcon(QIcon("data/icons/ribbon/heightmap.png"));
	import_heightmap->setText("Import\nHeightmap");

	change_tileset->setIcon(QIcon("data/icons/ribbon/tileset.png"));
	change_tileset->setText("Change\nTileset");

	change_tile_pathing->setIcon(QIcon("data/icons/ribbon/tileset.png"));
	change_tile_pathing->setText("Change Tile\nPathing");

	switch_warcraft->setIcon(QIcon("data/icons/ribbon/WarIII.ico"));
	switch_warcraft->setText("Change\nGame Folder");

	// Home tab
	auto* home_tab = new QRibbonTab;
	auto* history_section = make_section("History");
	history_section->addWidget(undo);
	history_section->addWidget(redo);

	auto* editor_section = make_section("Editors");
	editor_section->addWidget(trigger_editor);
	editor_section->addWidget(object_editor);
	editor_section->addWidget(model_editor);
	editor_section->addWidget(asset_manager);
	editor_section->addWidget(tooltip_editor);

	auto* config_section = make_section("Config");
	config_section->addWidget(export_objects);
	config_section->addWidget(import_objects);
	config_section->addWidget(map_protector);
	config_section->addWidget(config);

	home_tab->addSection(history_section);
	home_tab->addSection(editor_section);
	home_tab->addSection(config_section);

	// Terrain tab
	auto* terrain_tab = new QRibbonTab;
	auto* terrain_section = make_section("Palette");
	terrain_section->addWidget(terrain_palette);

	auto* terrain_tools_section = make_section("Terrain Tools");
	terrain_tools_section->addWidget(import_heightmap);
	terrain_tools_section->addWidget(change_tileset);
	terrain_tools_section->addWidget(change_tile_pathing);

	terrain_tab->addSection(terrain_section);
	terrain_tab->addSection(terrain_tools_section);

	// Doodads tab
	auto* doodads_tab = new QRibbonTab;
	auto* doodad_section = make_section("Palette");
	doodad_section->addWidget(doodad_palette);

	auto* doodad_tools_section = make_section("Viewport");
	doodad_tools_section->addWidget(mirror_button(doodads_visible, "Show or hide doodads.\nShortcut: Ctrl+D"));
	doodad_tools_section->addWidget(mirror_button(click_helpers_visible, "Show or hide click helper markers.\nShortcut: Ctrl+I"));
	doodad_tools_section->addWidget(mirror_button(object_editor, "Open the Object Editor. Shortcut: O"));

	doodads_tab->addSection(doodad_section);
	doodads_tab->addSection(doodad_tools_section);

	// Units tab
	auto* units_tab = new QRibbonTab;
	auto* unit_section = make_section("Palette");
	unit_section->addWidget(unit_palette);

	auto* unit_tools_section = make_section("Viewport");
	unit_tools_section->addWidget(mirror_button(units_visible, "Show or hide units.\nShortcut: Ctrl+U"));
	unit_tools_section->addWidget(mirror_button(minimap_visible, "Show or hide the minimap."));
	unit_tools_section->addWidget(mirror_button(object_editor, "Open the Object Editor. Shortcut: O"));

	auto* unit_data_section = make_section("Data");
	unit_data_section->addWidget(item_tables);

	units_tab->addSection(unit_section);
	units_tab->addSection(unit_tools_section);
	units_tab->addSection(unit_data_section);

	// Pathing tab
	auto* pathing_tab = new QRibbonTab;
	auto* pathing_section = make_section("Palette");
	pathing_section->addWidget(pathing_palette);

	auto* pathing_tools_section = make_section("Overlay");
	pathing_tools_section->addWidget(mirror_button(pathing_visible, "Show or hide the pathing overlay.\nShortcut: Ctrl+P"));
	pathing_tools_section->addWidget(mirror_button(brush_visible, "Show or hide the active brush overlay."));
	pathing_tools_section->addWidget(mirror_button(wireframe_visible, "Show or hide wireframe rendering.\nShortcut: Ctrl+T"));

	pathing_tab->addSection(pathing_section);
	pathing_tab->addSection(pathing_tools_section);

	// Regions tab
	auto* regions_tab = new QRibbonTab;
	auto* region_section = make_section("Palette");
	region_section->addWidget(region_palette);

	auto* region_tools_section = make_section("Map Context");
	region_tools_section->addWidget(mirror_button(map_description, "Open map description settings."));
	region_tools_section->addWidget(mirror_button(map_options, "Open map options."));
	region_tools_section->addWidget(mirror_button(minimap_visible, "Show or hide the minimap."));

	regions_tab->addSection(region_section);
	regions_tab->addSection(region_tools_section);

	// View tab
	auto* view_tab = new QRibbonTab;
	auto* visible_section = make_section("Visible");
	visible_section->addWidget(units_visible);
	visible_section->addWidget(doodads_visible);
	visible_section->addWidget(pathing_visible);
	visible_section->addWidget(brush_visible);
	visible_section->addWidget(lighting_visible);
	visible_section->addWidget(water_visible);
	visible_section->addWidget(click_helpers_visible);
	visible_section->addWidget(wireframe_visible);
	visible_section->addWidget(debug_visible);
	visible_section->addWidget(minimap_visible);

	auto* camera_section = make_section("Camera");
	camera_section->addWidget(reset_camera);

	view_tab->addSection(visible_section);
	view_tab->addSection(camera_section);

	// Map tab
	auto* map_tab = new QRibbonTab;
	auto* map_section = make_section("Map");
	map_section->addWidget(map_description);
	map_section->addWidget(map_loading_screen);
	map_section->addWidget(map_options);
	map_section->addWidget(gameplay_constants);

	auto* scenario_section = make_section("Scenario");
	scenario_section->addWidget(scenario_properties);

	auto* tools_section = make_section("Game");
	tools_section->addWidget(switch_warcraft);

	map_tab->addSection(map_section);
	map_tab->addSection(scenario_section);
	map_tab->addSection(tools_section);

	home_tab_index = addTab(home_tab, "Home");
	terrain_tab_index = addTab(terrain_tab, "Terrain");
	doodads_tab_index = addTab(doodads_tab, "Doodads");
	units_tab_index = addTab(units_tab, "Units");
	pathing_tab_index = addTab(pathing_tab, "Pathing");
	regions_tab_index = addTab(regions_tab, "Regions");
	view_tab_index = addTab(view_tab, "View");
	map_tab_index = addTab(map_tab, "Map");

	sync_file_corner_geometry();
}

MainRibbon::~MainRibbon() {
}

void MainRibbon::set_map_context(const QString& title, const QString& meta, const QString& badge) {
	map_title->setText(title);
	map_meta->setText(meta);
	map_badge->setText(badge);
	map_badge->setVisible(!badge.isEmpty());
}

void MainRibbon::show_tab(const HeaderTab tab) {
	switch (tab) {
	case HeaderTab::home:
		setCurrentIndex(home_tab_index);
		break;
	case HeaderTab::terrain:
		setCurrentIndex(terrain_tab_index);
		break;
	case HeaderTab::doodads:
		setCurrentIndex(doodads_tab_index);
		break;
	case HeaderTab::units:
		setCurrentIndex(units_tab_index);
		break;
	case HeaderTab::pathing:
		setCurrentIndex(pathing_tab_index);
		break;
	case HeaderTab::regions:
		setCurrentIndex(regions_tab_index);
		break;
	case HeaderTab::view:
		setCurrentIndex(view_tab_index);
		break;
	case HeaderTab::map:
		setCurrentIndex(map_tab_index);
		break;
	}
}

void MainRibbon::resizeEvent(QResizeEvent* event) {
	QRibbon::resizeEvent(event);
	sync_file_corner_geometry();
}

void MainRibbon::showEvent(QShowEvent* event) {
	QRibbon::showEvent(event);
	sync_file_corner_geometry();
}

void MainRibbon::sync_file_corner_geometry() {
	auto* file_corner = findChild<QWidget*>("ribbonFileCorner");
	auto* file_btn = findChild<QRibbonFileButton*>("ribbonFileButton");
	auto* file_corner_layout = qobject_cast<QHBoxLayout*>(file_corner ? file_corner->layout() : nullptr);

	if (!file_corner || !file_btn || !file_corner_layout || tabBar()->count() == 0) {
		return;
	}

	const QRect first_tab_rect = tabBar()->tabRect(0);
	const int top_margin = std::max(0, first_tab_rect.y());
	const int right_margin = 4;
	const int tab_row_height = std::max(first_tab_rect.height(), tabBar()->height() - top_margin);

	file_corner_layout->setContentsMargins(0, top_margin, right_margin, 0);
	file_corner->setFixedHeight(tabBar()->height());
	file_btn->setFixedHeight(tab_row_height);
}
