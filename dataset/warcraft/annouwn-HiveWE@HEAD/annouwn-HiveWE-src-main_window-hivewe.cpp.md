---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/main_window/hivewe.cpp"
ref: HEAD
---

#include "HiveWE.h"
#include "ui_HiveWE.h"
#include "new_map_dialog.h"
#include "shortcut_config_dialog.h"
#define __STORMLIB_NO_STATIC_LINK__
#include "StormLib.h"

import std;
import Hierarchy;
import BinaryReader;
import MPQ;
import Camera;
import Globals;
import Map;
import Imports;
import <soil2/SOIL2.h>;
import MapGlobal;
import WorldUndoManager;
import SkinnedMeshGlobals;
import ResourceManager;
import WindowHandler;
import "pathing_palette.h";
import "object_editor/object_editor.h";
import "model_editor/model_editor.h";
import "model_editor/model_editor_glwidget.h";
import "tile_setter.h";
import "map_info_editor.h";
import "terrain_palette.h";
import "settings_editor.h";
import "tile_pather.h";
import "palette.h";
import "terrain_palette.h";
import "doodad_palette.h";
import "unit_palette.h";
import "region_palette.h";
import "object_editor/icon_view.h";
import "trigger_editor.h";
#include "QMessageBox"
#include "QProcess"
#include "QKeySequence"
#include "QString"
#include <QApplication>
#include <QDialog>
#include <QDialogButtonBox>
#include <QDir>
#include <QHBoxLayout>
#include <QVBoxLayout>
#include <QFrame>
#include <QLabel>
#include <QListWidget>
#include <QPushButton>
#include <QStyle>
#include <QScrollArea>
#include <QFileInfo>
#include <QMenu>
#include <QRegularExpression>
#include <QStringList>
#include <QTextDocument>
import "menus/gameplay_constants_editor.h";
import "menus/item_tables_editor.h";
import "menus/scenario_properties_editor.h";
import "asset_manager/asset_manager.h";
import "tooltip_editor/tooltip_editor.h";
import "map_protector/map_protector.h";
#include "object_data_import_dialog.h"
import ObjectDataIo;
import ScenarioDataIo;
import MapInfo;

namespace fs = std::filesystem;

namespace {
std::shared_ptr<mdx::MDX> load_preview_mdx(const fs::path& path) {
	auto result = hierarchy.open_file(path);
	if (!result) {
		std::println("Preview model load failed for {} ({})", path.string(), result.error());
		return nullptr;
	}

	return std::make_shared<mdx::MDX>(result.value());
}
}

HiveWE::HiveWE(QWidget* parent)
	: QMainWindow(parent), ui(std::make_unique<Ui::HiveWEClass>()) {
	setAutoFillBackground(true);

	// Buggy as of Qt 6.9.1. Likely requires 6.9.2 or later
	// setWindowFlag(Qt::ExpandedClientAreaHint, true);
	// setWindowFlag(Qt::NoTitleBarBackgroundHint, true);
	// setAttribute(Qt::WA_LayoutOnEntireRect, true);
	ui->setupUi(this);
	context = ui->widget;
	// Reasonable floor so the window can be resized but not collapsed to nothing.
	setMinimumSize(800, 600);
	setup_palette_sidebar();

	transient_notice = new QLabel(this);
	transient_notice->hide();
	transient_notice->setAttribute(Qt::WA_TransparentForMouseEvents);
	transient_notice->setAlignment(Qt::AlignCenter);
	transient_notice->setStyleSheet(
		"QLabel {"
		"background-color: rgba(20, 24, 30, 228);"
		"color: rgb(244, 247, 250);"
		"border: 1px solid rgba(255, 255, 255, 52);"
		"border-radius: 10px;"
		"padding: 18px 32px;"
		"font-size: 24px;"
		"font-weight: 700;"
		"}"
	);
	transient_notice_timer.setSingleShot(true);
	connect(&transient_notice_timer, &QTimer::timeout, transient_notice, &QLabel::hide);

	connect(ui->ribbon->undo, &QPushButton::clicked, [&]() {
		// ToDo: temporary, undoing should still allow a selection to persist
		if (map->brush) {
			map->brush->clear_selection();
		}

		auto context = WorldEditContext {
			.terrain = map->terrain,
			.units = map->units,
			.doodads = map->doodads,
			.regions = map->regions,
			.brush = map->brush,
			.pathing_map = map->pathing_map,
		};

		map->world_undo.undo(context);
	});
	connect(ui->ribbon->redo, &QPushButton::clicked, [&]() {
		// ToDo: temporary, undoing should still allow a selection to persist
		if (map->brush) {
			map->brush->clear_selection();
		}

		auto context = WorldEditContext {
			.terrain = map->terrain,
			.units = map->units,
			.doodads = map->doodads,
			.regions = map->regions,
			.brush = map->brush,
			.pathing_map = map->pathing_map,
		};

		map->world_undo.redo(context);
	});

	connect(new QShortcut(Qt::CTRL | Qt::Key_Z, this), &QShortcut::activated, ui->ribbon->undo, &QPushButton::click);
	connect(new QShortcut(Qt::CTRL | Qt::Key_Y, this), &QShortcut::activated, ui->ribbon->redo, &QPushButton::click);

	connect(ui->ribbon->units_visible, &QPushButton::toggled, [](bool checked) { map->render_units = checked; });
	connect(ui->ribbon->doodads_visible, &QPushButton::toggled, [](bool checked) { map->render_doodads = checked; });
	connect(ui->ribbon->pathing_visible, &QPushButton::toggled, [](bool checked) { map->render_pathing = checked; });
	connect(ui->ribbon->brush_visible, &QPushButton::toggled, [](bool checked) { map->render_brush = checked; });
	connect(ui->ribbon->lighting_visible, &QPushButton::toggled, [](bool checked) { map->render_lighting = checked; });
	connect(ui->ribbon->water_visible, &QPushButton::toggled, [](bool checked) { map->render_water = checked; });
	connect(ui->ribbon->click_helpers_visible, &QPushButton::toggled, [](bool checked) { map->render_click_helpers = checked; });
	connect(ui->ribbon->wireframe_visible, &QPushButton::toggled, [](bool checked) { map->render_wireframe = checked; });
	connect(ui->ribbon->debug_visible, &QPushButton::toggled, [](bool checked) { map->render_debug = checked; });
	connect(ui->ribbon->minimap_visible, &QPushButton::toggled, [&](bool checked) {
		(checked) ? minimap->show() : minimap->hide();
		if (sidebar_minimap_frame) {
			sidebar_minimap_frame->setVisible(checked);
		}
	});

	connect(new QShortcut(Qt::CTRL | Qt::Key_U, this), &QShortcut::activated, ui->ribbon->units_visible, &QPushButton::click);
	connect(new QShortcut(Qt::CTRL | Qt::Key_D, this), &QShortcut::activated, ui->ribbon->doodads_visible, &QPushButton::click);
	connect(new QShortcut(Qt::CTRL | Qt::Key_P, this), &QShortcut::activated, ui->ribbon->pathing_visible, &QPushButton::click);
	connect(new QShortcut(Qt::CTRL | Qt::Key_L, this), &QShortcut::activated, ui->ribbon->lighting_visible, &QPushButton::click);
	connect(new QShortcut(Qt::CTRL | Qt::Key_W, this), &QShortcut::activated, ui->ribbon->water_visible, &QPushButton::click);
	connect(new QShortcut(Qt::CTRL | Qt::Key_I, this), &QShortcut::activated, ui->ribbon->click_helpers_visible, &QPushButton::click);
	connect(new QShortcut(Qt::CTRL | Qt::Key_T, this), &QShortcut::activated, ui->ribbon->wireframe_visible, &QPushButton::click);
	connect(new QShortcut(Qt::Key_F3, this), &QShortcut::activated, ui->ribbon->debug_visible, &QPushButton::click);

	// Reload theme
	connect(new QShortcut(Qt::Key_F5, this), &QShortcut::activated, [&]() {
		QSettings settings;
		QFile file("data/themes/" + settings.value("theme").toString() + ".qss");
		const auto success = file.open(QFile::ReadOnly);
		if (!success) {
			std::println("Failed to open theme file");
			return;
		}
		QString StyleSheet = QLatin1String(file.readAll());

		qApp->setStyleSheet(StyleSheet);
	});

	connect(ui->ribbon->reset_camera, &QPushButton::clicked, [&]() { camera.reset(); });
	connect(new QShortcut(QKeySequence(Qt::CTRL | Qt::SHIFT | Qt::Key_C), this), &QShortcut::activated, ui->ribbon->reset_camera, &QPushButton::click);

	connect(ui->ribbon->import_heightmap, &QPushButton::clicked, this, &HiveWE::import_heightmap);

	connect(new QShortcut(QKeySequence(Qt::CTRL | Qt::Key_O), this, nullptr, nullptr, Qt::ApplicationShortcut), &QShortcut::activated, ui->ribbon->open_map_folder, &QToolButton::click);
	// connect(new QShortcut(QKeySequence(Qt::CTRL | Qt::Key_I), this, nullptr, nullptr, Qt::ApplicationShortcut), &QShortcut::activated, ui->ribbon->open_map_mpq, &QPushButton::click);
	connect(new QShortcut(QKeySequence(Qt::CTRL | Qt::Key_S), this, nullptr, nullptr, Qt::ApplicationShortcut), &QShortcut::activated, ui->ribbon->save_map, &QToolButton::click);
	connect(new QShortcut(QKeySequence(Qt::CTRL | Qt::SHIFT | Qt::Key_S), this, nullptr, nullptr, Qt::ApplicationShortcut), &QShortcut::activated, ui->ribbon->save_map_as, &QToolButton::click);

	connect(ui->ribbon->new_map, &QToolButton::clicked, this, &HiveWE::create_new_map);
	connect(ui->ribbon->open_map_folder, &QToolButton::clicked, this, &HiveWE::load_folder);
	connect(ui->ribbon->open_map_mpq, &QToolButton::clicked, this, &HiveWE::load_mpq);
	for (int i = 0; i < static_cast<int>(ui->ribbon->recent_maps.size()); ++i) {
		connect(ui->ribbon->recent_maps[i], &QToolButton::clicked, this, [this, i]() { open_recent_map(i); });
	}
	connect(ui->ribbon->save_map, &QToolButton::clicked, this, &HiveWE::save);
	connect(ui->ribbon->save_map_as, &QToolButton::clicked, this, &HiveWE::save_as);
	connect(ui->ribbon->export_mpq, &QToolButton::clicked, this, &HiveWE::export_mpq);
	connect(ui->ribbon->test_map, &QToolButton::clicked, this, &HiveWE::play_test);
	connect(ui->ribbon->quick_search, &QToolButton::clicked, this, [this]() { new GlobalSearchWidget(this); });
	connect(ui->ribbon->settings, &QToolButton::clicked, [&]() { new SettingsEditor(this); });
	connect(ui->ribbon->config, &QToolButton::clicked, [&]() { new ShortcutConfigDialog(this); });

	auto* export_menu = new QMenu(this);
	export_menu->addAction("Binary package", this, &HiveWE::export_object_data_binary);
	export_menu->addAction("Text package", this, &HiveWE::export_object_data_text);
	ui->ribbon->export_objects->setMenu(export_menu);
	ui->ribbon->export_objects->setPopupMode(QToolButton::InstantPopup);

	auto* import_menu = new QMenu(this);
	import_menu->addAction("Binary package", this, &HiveWE::import_object_data_binary);
	import_menu->addAction("Map folder", this, &HiveWE::import_object_data_map_folder);
	import_menu->addAction("Text package", this, &HiveWE::import_object_data_text);
	ui->ribbon->import_objects->setMenu(import_menu);
	ui->ribbon->import_objects->setPopupMode(QToolButton::InstantPopup);

	connect(ui->ribbon->switch_warcraft, &QToolButton::clicked, this, &HiveWE::switch_warcraft);
	connect(ui->ribbon->exit, &QToolButton::clicked, [&]() { QApplication::exit(); });

	connect(ui->ribbon->change_tileset, &QRibbonButton::clicked, [this]() { new TileSetter(this); });
	connect(ui->ribbon->change_tile_pathing, &QRibbonButton::clicked, [this]() { new TilePather(this); });

	connect(ui->ribbon->map_description, &QRibbonButton::clicked, [&]() { (new MapInfoEditor(this))->ui.tabs->setCurrentIndex(0); });
	connect(ui->ribbon->map_loading_screen, &QRibbonButton::clicked, [&]() { (new MapInfoEditor(this))->ui.tabs->setCurrentIndex(1); });
	connect(ui->ribbon->map_options, &QRibbonButton::clicked, [&]() { (new MapInfoEditor(this))->ui.tabs->setCurrentIndex(2); });
	// connect(ui, &QAction::triggered, [&]() { (new MapInfoEditor(this))->ui.tabs->setCurrentIndex(3); });

	ui->ribbon->terrain_palette->setCheckable(true);
	ui->ribbon->doodad_palette->setCheckable(true);
	ui->ribbon->unit_palette->setCheckable(true);
	ui->ribbon->region_palette->setCheckable(true);
	ui->ribbon->pathing_palette->setCheckable(true);
	connect(ui->ribbon->pathing_palette, &QRibbonButton::clicked, this, &HiveWE::toggle_pathing_sidebar);
	connect(ui->ribbon->region_palette, &QRibbonButton::clicked, this, &HiveWE::toggle_region_palette);

	connect(new QShortcut(QKeySequence(Qt::Key_T), this, nullptr, nullptr, Qt::WindowShortcut), &QShortcut::activated, this, &HiveWE::toggle_terrain_sidebar);
	connect(ui->ribbon->terrain_palette, &QRibbonButton::clicked, this, &HiveWE::toggle_terrain_sidebar);

	connect(new QShortcut(QKeySequence(Qt::Key_D), this, nullptr, nullptr, Qt::WindowShortcut), &QShortcut::activated, this, &HiveWE::toggle_doodad_palette);
	connect(ui->ribbon->doodad_palette, &QRibbonButton::clicked, this, &HiveWE::toggle_doodad_palette);

	connect(new QShortcut(QKeySequence(Qt::Key_U), this, nullptr, nullptr, Qt::WindowShortcut), &QShortcut::activated, this, &HiveWE::toggle_unit_palette);
	connect(ui->ribbon->unit_palette, &QRibbonButton::clicked, this, &HiveWE::toggle_unit_palette);

	connect(new QShortcut(QKeySequence(Qt::Key_P), this, nullptr, nullptr, Qt::WindowShortcut), &QShortcut::activated, this, &HiveWE::toggle_pathing_sidebar);
	connect(new QShortcut(QKeySequence(Qt::Key_R), this, nullptr, nullptr, Qt::WindowShortcut), &QShortcut::activated, this, &HiveWE::toggle_region_palette);

	connect(ui->ribbon->trigger_editor, &QRibbonButton::clicked, [this]() {
		bool created = false;
		const auto editor = window_handler.create_or_raise<TriggerEditor>(nullptr, created);
		connect(this, &HiveWE::saving_initiated, editor, &TriggerEditor::save_changes, Qt::UniqueConnection);
	});

	connect(ui->ribbon->object_editor, &QRibbonButton::clicked, [this]() {
		bool created = false;
		window_handler.create_or_raise<ObjectEditor>(nullptr, created);
	});
	connect(new QShortcut(QKeySequence(Qt::Key_O), this, nullptr, nullptr, Qt::ApplicationShortcut), &QShortcut::activated, [this]() {
		if (const auto open_editor = window_handler.get_open<ObjectEditor>(); open_editor.has_value() && *open_editor) {
			(*open_editor)->close();
			return;
		}

		bool created = false;
		window_handler.create_or_raise<ObjectEditor>(nullptr, created);
	});

	connect(ui->ribbon->model_editor, &QRibbonButton::clicked, [this]() {
		bool created = false;
		window_handler.create_or_raise<ModelEditor>(nullptr, created);
	});

	connect(ui->ribbon->gameplay_constants, &QRibbonButton::clicked, [this]() {
		bool created = false;
		window_handler.create_or_raise<GameplayConstantsEditor>(nullptr, created);
	});

	connect(ui->ribbon->item_tables, &QRibbonButton::clicked, [this]() {
		bool created = false;
		window_handler.create_or_raise<ItemTablesEditor>(nullptr, created);
	});

	connect(ui->ribbon->scenario_properties, &QRibbonButton::clicked, [this]() {
		bool created = false;
		window_handler.create_or_raise<ScenarioPropertiesEditor>(nullptr, created);
	});

	connect(ui->ribbon->asset_manager, &QRibbonButton::clicked, [this]() {
		bool created = false;
		window_handler.create_or_raise<AssetManager>(nullptr, created);
	});

	connect(ui->ribbon->tooltip_editor, &QRibbonButton::clicked, [this]() {
		bool created = false;
		window_handler.create_or_raise<TooltipEditor>(nullptr, created);
	});

	connect(ui->ribbon->map_protector, &QRibbonButton::clicked, [this]() {
		bool created = false;
		window_handler.create_or_raise<MapProtector>(nullptr, created);
	});

	restore_window_state();
	update_recent_maps_menu();

	connect(minimap, &Minimap::clicked, [](QPointF location) { camera.position = { location.x() * map->terrain.width, (1.0 - location.y()) * map->terrain.height, camera.position.z }; });
	ui->widget->makeCurrent();

	map = new Map();
	connect(&map->terrain, &Terrain::minimap_changed, minimap, &Minimap::set_minimap);
	connect(&map->terrain, &Terrain::tileset_changed, [&]() {
		if (terrain_palette) {
			terrain_palette->refresh();
		}
	});

	map->render_manager.resize_framebuffers(ui->widget->width(), ui->widget->height());
}

HiveWE::~HiveWE() = default;

void HiveWE::setup_palette_sidebar() {
	auto* main_layout = qobject_cast<QVBoxLayout*>(centralWidget()->layout());
	main_layout->removeWidget(ui->widget);

	auto* content_layout = new QHBoxLayout;
	content_layout->setContentsMargins(0, 0, 0, 0);
	content_layout->setSpacing(0);
	content_layout->addWidget(ui->widget, 1);
	main_layout->addLayout(content_layout);

	sidebar_root = new QWidget(centralWidget());
	sidebar_root->setObjectName("paletteSidebar");
	sidebar_root->setSizePolicy(QSizePolicy::Fixed, QSizePolicy::Expanding);
	sidebar_root->setFixedWidth(400);
	sidebar_root->setStyleSheet(
		"#paletteSidebar { background: rgba(20, 23, 27, 236); border-left: 1px solid rgba(255, 255, 255, 18); }"
		"QWidget#paletteSidebarHeader { background: transparent; }"
		"QWidget#paletteSidebarModeBar { background: transparent; }"
		"QToolButton[sidebarMode=\"true\"] {"
		"background: rgba(37, 41, 47, 224);"
		"border: 1px solid rgba(255, 255, 255, 16);"
		"border-radius: 11px;"
		"color: rgb(192, 199, 208);"
		"font-size: 12px;"
		"font-weight: 600;"
		"padding: 7px 10px;"
		"text-align: left;"
		"}"
		"QToolButton[sidebarMode=\"true\"]:checked {"
		"background: rgba(70, 132, 214, 210);"
		"border: 1px solid rgba(95, 164, 255, 240);"
		"color: white;"
		"}"
		"QLabel[sidebarTitle=\"true\"] { font-size: 24px; font-weight: 700; color: rgb(241, 244, 247); }"
		"QLabel[sidebarDescription=\"true\"] { font-size: 13px; color: rgb(178, 187, 198); }"
		"QFrame[sidebarBody=\"true\"] { background: rgba(33, 37, 43, 230); border: 1px solid rgba(255, 255, 255, 16); border-radius: 16px; }"
		"QFrame[sidebarPaletteHost=\"true\"] { background: transparent; border: 0; }"
		"QFrame[sidebarUtilityPanel=\"true\"] { background: rgba(24, 27, 31, 200); border: 1px solid rgba(255, 255, 255, 14); border-radius: 12px; }"
		"QLabel[sidebarPreviewTitle=\"true\"] { color: rgb(241, 244, 247); font-size: 13px; font-weight: 700; }"
		"QLabel[sidebarPreviewPlaceholder=\"true\"] { color: rgb(132, 141, 153); font-size: 12px; }"
		"QLabel[sidebarHint=\"true\"] { font-size: 11px; color: rgb(132, 141, 153); padding-top: 2px; }"
		"#paletteSidebar QLineEdit, #paletteSidebar QComboBox, #paletteSidebar QSpinBox {"
		"background: rgba(24, 27, 31, 235);"
		"border: 1px solid rgba(255, 255, 255, 18);"
		"border-radius: 10px;"
		"padding: 6px 10px;"
		"color: rgb(232, 236, 241);"
		"selection-background-color: rgba(95, 164, 255, 210);"
		"}"
		"#paletteSidebar QLineEdit:focus, #paletteSidebar QComboBox:focus, #paletteSidebar QSpinBox:focus {"
		"border: 1px solid rgba(95, 164, 255, 235);"
		"}"
		"#paletteSidebar QListView, #paletteSidebar QListWidget {"
		"background: rgba(24, 27, 31, 210);"
		"border: 1px solid rgba(255, 255, 255, 14);"
		"border-radius: 12px;"
		"padding: 4px;"
		"color: rgb(232, 236, 241);"
		"outline: 0;"
		"}"
		"#paletteSidebar QListView::item, #paletteSidebar QListWidget::item {"
		"padding: 6px 8px;"
		"border-radius: 8px;"
		"}"
		"#paletteSidebar QListView::item:selected, #paletteSidebar QListWidget::item:selected {"
		"background: rgba(95, 164, 255, 210);"
		"color: white;"
		"}"
		"#paletteSidebar QPushButton, #paletteSidebar QToolButton, #paletteSidebar QRadioButton, #paletteSidebar QCheckBox {"
		"color: rgb(229, 233, 239);"
		"}"
		"#paletteSidebar QSlider::groove:horizontal {"
		"height: 6px;"
		"background: rgba(255, 255, 255, 20);"
		"border-radius: 999px;"
		"}"
		"#paletteSidebar QSlider::handle:horizontal {"
		"width: 16px;"
		"margin: -5px 0;"
		"background: rgb(95, 164, 255);"
		"border-radius: 8px;"
		"}"
		"#paletteSidebar QGroupBox {"
		"border: 0;"
		"margin-top: 6px;"
		"font-weight: 700;"
		"color: rgb(214, 221, 229);"
		"}"
		"#paletteSidebar QLabel { color: rgb(214, 221, 229); }"
	);

	auto* sidebar_layout = new QVBoxLayout(sidebar_root);
	sidebar_layout->setContentsMargins(12, 12, 12, 12);
	sidebar_layout->setSpacing(12);

	sidebar_mode_bar = new QWidget(sidebar_root);
	sidebar_mode_bar->setObjectName("paletteSidebarModeBar");
	auto* mode_layout = new QGridLayout(sidebar_mode_bar);
	mode_layout->setContentsMargins(0, 0, 0, 0);
	mode_layout->setSpacing(8);
	mode_layout->setHorizontalSpacing(8);
	mode_layout->setVerticalSpacing(8);

	auto create_mode_button = [this, mode_layout](const QString& text, const QIcon& icon, const auto slot, const int row, const int column) {
		auto* button = new QToolButton(sidebar_mode_bar);
		button->setProperty("sidebarMode", true);
		button->setCheckable(true);
		button->setToolButtonStyle(Qt::ToolButtonTextBesideIcon);
		button->setIcon(icon);
		button->setText(text);
		button->setAutoRaise(false);
		button->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Preferred);
		button->setMinimumHeight(38);
		button->setToolTip(text);
		mode_layout->addWidget(button, row, column);
		connect(button, &QToolButton::clicked, this, [this, slot]() { (this->*slot)(); });
		return button;
	};

	terrain_mode_button = create_mode_button("Terrain", QIcon("data/icons/ribbon/heightmap.png"), &HiveWE::toggle_terrain_sidebar, 0, 0);
	doodad_mode_button = create_mode_button("Doodads", QIcon("data/icons/ribbon/doodads.png"), &HiveWE::toggle_doodad_palette, 0, 1);
	unit_mode_button = create_mode_button("Units", QIcon("data/icons/ribbon/units.png"), &HiveWE::toggle_unit_palette, 0, 2);
	pathing_mode_button = create_mode_button("Pathing", QIcon("data/icons/ribbon/pathing.png"), &HiveWE::toggle_pathing_sidebar, 1, 0);
	region_mode_button = create_mode_button("Regions", QIcon("data/icons/ribbon/select.png"), &HiveWE::toggle_region_palette, 1, 1);
	mode_layout->setColumnStretch(0, 1);
	mode_layout->setColumnStretch(1, 1);
	mode_layout->setColumnStretch(2, 1);

	sidebar_minimap_frame = new QFrame(sidebar_root);
	sidebar_minimap_frame->setProperty("sidebarUtilityPanel", true);
	// Adaptive height: keeps the 196px look when there is room but can shrink on
	// short windows so it does not force the whole window's minimum height.
	sidebar_minimap_frame->setMinimumHeight(150);
	sidebar_minimap_frame->setMaximumHeight(196);
	sidebar_minimap_frame->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Preferred);
	auto* minimap_layout = new QVBoxLayout(sidebar_minimap_frame);
	minimap_layout->setContentsMargins(8, 8, 8, 8);
	minimap_layout->setSpacing(0);
	minimap->setParent(sidebar_minimap_frame);
	minimap->setMinimumHeight(130);
	minimap->setMaximumHeight(180);
	minimap->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Preferred);
	minimap_layout->addWidget(minimap);
	sidebar_minimap_frame->setVisible(ui->ribbon->minimap_visible->isChecked());
	minimap->setVisible(ui->ribbon->minimap_visible->isChecked());

	sidebar_preview_frame = new QFrame(sidebar_root);
	sidebar_preview_frame->setProperty("sidebarUtilityPanel", true);
	auto* preview_layout = new QVBoxLayout(sidebar_preview_frame);
	preview_layout->setContentsMargins(8, 8, 8, 8);
	preview_layout->setSpacing(6);

	sidebar_preview_title = new QLabel("Model Preview", sidebar_preview_frame);
	sidebar_preview_title->setProperty("sidebarPreviewTitle", true);
	sidebar_preview_title->setAlignment(Qt::AlignLeft | Qt::AlignVCenter);
	preview_layout->addWidget(sidebar_preview_title);

	sidebar_preview_placeholder = new QLabel("Select a doodad or unit to preview its model.", sidebar_preview_frame);
	sidebar_preview_placeholder->setProperty("sidebarPreviewPlaceholder", true);
	sidebar_preview_placeholder->setAlignment(Qt::AlignCenter);
	sidebar_preview_placeholder->setWordWrap(true);
	// Cap the placeholder so it can shrink on short windows instead of forcing a
	// large minimum height on the sidebar (and therefore the main window).
	sidebar_preview_placeholder->setMinimumHeight(60);
	sidebar_preview_placeholder->setMaximumHeight(150);
	preview_layout->addWidget(sidebar_preview_placeholder);

	sidebar_header = new QWidget(sidebar_root);
	sidebar_header->setObjectName("paletteSidebarHeader");
	auto* header_layout = new QVBoxLayout(sidebar_header);
	header_layout->setContentsMargins(0, 0, 0, 0);
	header_layout->setSpacing(6);

	sidebar_title = new QLabel(sidebar_header);
	sidebar_title->setProperty("sidebarTitle", true);
	sidebar_title->setAlignment(Qt::AlignLeft | Qt::AlignVCenter);

	sidebar_description = new QLabel(sidebar_header);
	sidebar_description->setProperty("sidebarDescription", true);
	sidebar_description->setWordWrap(true);
	sidebar_description->setAlignment(Qt::AlignLeft | Qt::AlignTop);

	header_layout->addWidget(sidebar_title);
	header_layout->addWidget(sidebar_description);

	auto* sidebar_body = new QFrame(sidebar_root);
	sidebar_body->setProperty("sidebarBody", true);
	auto* body_layout = new QVBoxLayout(sidebar_body);
	body_layout->setContentsMargins(14, 14, 14, 14);
	body_layout->setSpacing(0);

	sidebar_stack = new QStackedWidget(sidebar_body);
	sidebar_stack->setContentsMargins(0, 0, 0, 0);
	body_layout->addWidget(sidebar_stack);

	auto create_host = [this](const char* object_name) {
		auto* host = new QFrame(sidebar_stack);
		host->setObjectName(object_name);
		host->setProperty("sidebarPaletteHost", true);
		host->setFrameShape(QFrame::NoFrame);
		host->setContentsMargins(0, 0, 0, 0);
		auto* layout = new QVBoxLayout(host);
		layout->setContentsMargins(0, 0, 0, 0);
		layout->setSpacing(0);
		sidebar_stack->addWidget(host);
		return host;
	};

	terrain_host = create_host("terrainSidebarHost");
	doodad_host = create_host("doodadSidebarHost");
	unit_host = create_host("unitSidebarHost");
	pathing_host = create_host("pathingSidebarHost");
	region_host = create_host("regionSidebarHost");

	// Wrap the palette stack in a scroll area so a tall palette scrolls instead of
	// forcing the sidebar (and the main window) to a large minimum height. This is
	// what lets the window be resized vertically on short screens.
	auto* sidebar_scroll = new QScrollArea(sidebar_root);
	sidebar_scroll->setWidget(sidebar_body);
	sidebar_scroll->setWidgetResizable(true);
	sidebar_scroll->setFrameShape(QFrame::NoFrame);
	sidebar_scroll->setHorizontalScrollBarPolicy(Qt::ScrollBarAlwaysOff);
	sidebar_scroll->setVerticalScrollBarPolicy(Qt::ScrollBarAsNeeded);
	sidebar_scroll->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Expanding);
	sidebar_scroll->setStyleSheet("QScrollArea { background: transparent; border: 0; }");

	sidebar_hint = new QLabel(sidebar_root);
	sidebar_hint->setProperty("sidebarHint", true);
	sidebar_hint->setWordWrap(true);
	sidebar_hint->setAlignment(Qt::AlignLeft | Qt::AlignTop);

	sidebar_layout->addWidget(sidebar_mode_bar);
	sidebar_layout->addWidget(sidebar_minimap_frame);
	sidebar_layout->addWidget(sidebar_preview_frame);
	sidebar_layout->addWidget(sidebar_header);
	sidebar_layout->addWidget(sidebar_scroll, 1);
	sidebar_layout->addWidget(sidebar_hint);
	content_layout->addWidget(sidebar_root);

	sidebar_root->hide();
	update_active_palette_visuals();
}

void HiveWE::load_map(const fs::path& directory) {
	window_handler.close_all();
	ui->widget->makeCurrent();
	reset_map_session();

	map->load(directory);
	add_recent_map(directory);

	map->render_manager.resize_framebuffers(ui->widget->width(), ui->widget->height());
	update_map_context("Loaded");
}

void HiveWE::add_recent_map(const fs::path& directory) {
	QSettings settings;
	QStringList recent_maps = settings.value("recentMaps").toStringList();
	const QString map_path = QString::fromStdString(directory.lexically_normal().string());

	recent_maps.removeAll(map_path);
	recent_maps.prepend(map_path);

	while (recent_maps.size() > 3) {
		recent_maps.removeLast();
	}

	settings.setValue("recentMaps", recent_maps);
	update_recent_maps_menu();
}

void HiveWE::update_recent_maps_menu() {
	QSettings settings;
	QStringList recent_maps = settings.value("recentMaps").toStringList();

	int button_index = 0;
	for (const auto& map_path : recent_maps) {
		if (button_index >= static_cast<int>(ui->ribbon->recent_maps.size())) {
			break;
		}

		const fs::path directory = map_path.toStdString();
		if (!fs::exists(directory / "war3map.w3i")) {
			continue;
		}

		auto* button = ui->ribbon->recent_maps[button_index];
		const QFileInfo info(map_path);
		button->setText(info.fileName());
		button->setToolTip(map_path);
		button->setStatusTip(map_path);
		button->show();
		++button_index;
	}

	for (; button_index < static_cast<int>(ui->ribbon->recent_maps.size()); ++button_index) {
		ui->ribbon->recent_maps[button_index]->hide();
	}
}

void HiveWE::open_recent_map(int index) {
	QSettings settings;
	const QStringList recent_maps = settings.value("recentMaps").toStringList();
	if (index < 0 || index >= recent_maps.size()) {
		return;
	}

	const fs::path directory = recent_maps[index].toStdString();
	if (!fs::exists(directory / "war3map.w3i")) {
		QMessageBox::information(this, "Opening map failed", "The recent map path no longer exists.");
		update_recent_maps_menu();
		return;
	}

	if (!confirm_map_replacement("open another map")) {
		return;
	}

	load_map(directory);
}

void HiveWE::load_folder() {
	QSettings settings;

	QString folder_name = QFileDialog::getExistingDirectory(this, "Open Map Directory",
															settings.value("openDirectory", QDir::current().path()).toString(),
															QFileDialog::ShowDirsOnly | QFileDialog::DontResolveSymlinks);

	if (folder_name == "") {
		return;
	}

	settings.setValue("openDirectory", folder_name);

	fs::path directory = folder_name.toStdString();

	if (!fs::exists(directory / "war3map.w3i")) {
		QMessageBox::information(this, "Opening map failed", "Opening the map failed. Select a map that is saved in folder mode or use the Open Map (MPQ) option");
		return;
	}

	if (!confirm_map_replacement("open another map")) {
		return;
	}

	QMessageBox* loading_box = new QMessageBox(QMessageBox::Icon::Information, "Loading Map", "Loading " + QString::fromStdString(directory.filename().string()));
	loading_box->show();

	load_map(directory);

	loading_box->close();
	delete loading_box;
}

/// Load MPQ will extract all files from the archive in a user specified location
void HiveWE::load_mpq() {
	QSettings settings;

	// Choose an MPQ
	QString file_name = QFileDialog::getOpenFileName(this, "Open File",
													 settings.value("openDirectory", QDir::current().path()).toString(),
													 "Warcraft III Scenario (*.w3m *.w3x)");

	if (file_name.isEmpty()) {
		return;
	}

	settings.setValue("openDirectory", file_name);

	fs::path mpq_path = file_name.toStdWString();

	if (!confirm_map_replacement("open another map")) {
		return;
	}

	mpq::MPQ mpq;
	bool opened = mpq.open(mpq_path);
	if (!opened) {
		const auto message = std::format("Opening the map archive failed. It might be opened in another program.\nError Code {}", GetLastError());
		QMessageBox::critical(this, "Opening map failed", QString::fromStdString(message));
		return;
	}

	fs::path unpack_location = QFileDialog::getExistingDirectory(
								   this, "Choose Unpacking Location",
								   settings.value("openDirectory", QDir::current().path()).toString(),
								   QFileDialog::ShowDirsOnly | QFileDialog::DontResolveSymlinks)
								   .toStdString();

	if (unpack_location.empty()) {
		return;
	}

	fs::path final_directory = unpack_location / mpq_path.stem();

	try {
		fs::create_directory(final_directory);
	} catch (std::filesystem::filesystem_error& e) {
		QMessageBox::critical(this, "Error creating directory", "Failed to create the directory to unpack into with error:\n" + QString::fromStdString(e.what()), QMessageBox::StandardButton::Ok, QMessageBox::StandardButton::Ok);
		return;
	}

	bool unpacked = mpq.unpack(final_directory);
	if (!unpacked) {
		QMessageBox::critical(this, "Unpacking failed", "There was an error unpacking the archive.");
		std::println("{}", GetLastError());
		return;
	}

	// mpq.unpack() enumerates the archive via its (listfile); archives that don't have one
	// (stripped by some map protectors, or simply never written) still unpack, but unnamed
	// entries land on disk under fabricated names instead of e.g. "war3map.w3i". The core
	// map files have a fixed, well-known set of names, so fetch them directly by name too -
	// MPQ files are looked up by name hash, so this recovers them regardless of listfile
	// presence.
	for (const std::string& name : map->imports.blacklist) {
		mpq.extract_file(name, final_directory / name);
	}

	load_map(final_directory);
}

void HiveWE::save() {
	save_current_map();
};

void HiveWE::save_as() {
	save_current_map_as();
}

bool HiveWE::save_current_map() {
	if (!map || !map->loaded) {
		return false;
	}

	if (map->filesystem_path.empty()) {
		return save_current_map_as();
	}

	emit saving_initiated();
	if (!map->save(map->filesystem_path)) {
		return false;
	}

	show_transient_notice("Map saved");
	update_map_context("Saved");
	return true;
}

bool HiveWE::save_current_map_as() {
	if (!map || !map->loaded) {
		return false;
	}

	QSettings settings;
	fs::path file_name = QFileDialog::getExistingDirectory(this, "Choose Save Location",
														   settings.value("openDirectory", QDir::current().path()).toString(),
														   QFileDialog::ShowDirsOnly | QFileDialog::DontResolveSymlinks)
							 .toStdString();

	if (file_name.empty()) {
		return false;
	}

	settings.setValue("openDirectory", QString::fromStdString(file_name.string()));

	emit saving_initiated();
	if (!map->filesystem_path.empty() && fs::exists(file_name) && fs::equivalent(file_name, map->filesystem_path)) {
		if (!map->save(map->filesystem_path)) {
			return false;
		}
	} else {
		const QString fallback_name = sanitized_folder_name(QString::fromStdString(map->name));
		const fs::path target_directory = file_name / fallback_name.toStdString();
		fs::create_directories(target_directory);

		hierarchy.map_directory = target_directory;
		if (map->filesystem_path.empty()) {
			map->filesystem_path = fs::absolute(target_directory) / "";
			map->name = (*--(--map->filesystem_path.end())).string();
			hierarchy.map_directory = map->filesystem_path;
			if (!map->save(map->filesystem_path)) {
				return false;
			}
		} else {
			if (!map->save(target_directory)) {
				return false;
			}
		}
	}

	show_transient_notice("Map saved");
	update_map_context("Saved");
	return true;
}

void HiveWE::export_mpq() {
	if (!save_current_map()) {
		return;
	}

	QSettings settings;
	const QString directory = settings.value("openDirectory", QDir::current().path()).toString() + "/" + QString::fromStdString(map->filesystem_path.filename().string());
	std::wstring file_name = QFileDialog::getSaveFileName(this, "Export Map to MPQ", directory, "Warcraft III Scenario (*.w3x)").toStdWString();

	if (file_name.empty()) {
		return;
	}

	fs::remove(file_name);

	uint64_t file_count = std::distance(fs::recursive_directory_iterator{ map->filesystem_path }, {});

	HANDLE handle;
	bool open = SFileCreateArchive(file_name.c_str(), MPQ_CREATE_LISTFILE | MPQ_CREATE_ATTRIBUTES, file_count, &handle);
	if (!open) {
		QMessageBox::critical(this, "Exporting failed", "There was an error creating the archive.");
		std::println("{}", GetLastError());
		return;
	}

	for (const auto& entry : fs::recursive_directory_iterator(map->filesystem_path)) {
		if (entry.is_regular_file()) {
			bool success = SFileAddFileEx(handle, entry.path().c_str(), entry.path().lexically_relative(map->filesystem_path).string().c_str(), MPQ_FILE_COMPRESS, MPQ_COMPRESSION_ZLIB, MPQ_COMPRESSION_NEXT_SAME);
			if (!success) {
				std::println("Error {} adding file {}", GetLastError(), entry.path().string());
			}
		}
	}
	SFileCompactArchive(handle, nullptr, false);
	SFileCloseArchive(handle);
}

void HiveWE::play_test() {
	if (!save_current_map()) {
		return;
	}
	QProcess* warcraft = new QProcess;
	const QString warcraft_path = QString::fromStdString(fs::canonical(hierarchy.root_directory / "x86_64" / "Warcraft III.exe").string());
	QStringList arguments;
	arguments << "-launch"
			  << "-loadfile" << QString::fromStdString(fs::canonical(map->filesystem_path).string());

	QSettings settings;
	if (settings.value("testArgs").toString() != "")
		arguments << settings.value("testArgs").toString().split(' ');

	warcraft->start(warcraft_path, arguments);
}

void HiveWE::create_new_map() {
	if (!confirm_map_replacement("create a new map")) {
		return;
	}

	NewMapDialog dialog(this);
	if (dialog.exec() != QDialog::Accepted) {
		return;
	}

	const fs::path template_directory = fs::path("data") / "test map";
	if (!fs::exists(template_directory / "war3map.w3i")) {
		QMessageBox::critical(this, "Template map missing", "The bundled blank map template could not be found in data/test map.");
		return;
	}

	reset_map_session();
	map->load(template_directory);

	const NewMapDialog::Result settings = dialog.result();
	const int desired_width = settings.width + 1;
	const int desired_height = settings.height + 1;
	const int width_delta = desired_width - map->terrain.width;
	const int height_delta = desired_height - map->terrain.height;
	const int delta_left = width_delta / 2;
	const int delta_right = width_delta - delta_left;
	const int delta_bottom = height_delta / 2;
	const int delta_top = height_delta - delta_bottom;
	if (width_delta != 0 || height_delta != 0) {
		map->resize(delta_left, delta_right, delta_top, delta_bottom);
	}

	std::vector<std::string> tileset_ids;
	for (const auto& [id, row] : map->terrain.terrain_slk.row_headers) {
		if (!id.empty() && id.front() == settings.tileset) {
			tileset_ids.push_back(id);
		}
	}
	std::sort(tileset_ids.begin(), tileset_ids.end());

	std::vector<std::string> cliffset_ids;
	for (const auto& [id, row] : map->terrain.cliff_slk.row_headers) {
		if (!id.empty() && id.front() == settings.tileset) {
			cliffset_ids.push_back(id);
		}
	}
	std::sort(cliffset_ids.begin(), cliffset_ids.end());

	if (tileset_ids.empty()) {
		QMessageBox::critical(this, "New map failed", "The selected tileset does not have valid terrain data.");
		return;
	}
	if (cliffset_ids.empty()) {
		cliffset_ids = map->terrain.cliffset_ids;
	}

	map->terrain.tileset = settings.tileset;
	map->terrain.cliffset_ids = cliffset_ids;
	map->terrain.change_tileset(tileset_ids, std::vector<int>(map->terrain.tileset_ids.size(), 0));

	const int terrain_width = map->terrain.width;
	const int terrain_height = map->terrain.height;
	const size_t total_corners = static_cast<size_t>(terrain_width * terrain_height);
	map->terrain.resize_corner_arrays(total_corners);

	for (int y = 0; y < terrain_height; ++y) {
		for (int x = 0; x < terrain_width; ++x) {
			const size_t idx = map->terrain.ci(x, y);
			map->terrain.corner_map_edge[idx] = x == 0 || y == 0 || x == terrain_width - 1 || y == terrain_height - 1;
			map->terrain.corner_ground_texture[idx] = 0;
			map->terrain.corner_ground_variation[idx] = 0;
			map->terrain.corner_cliff_texture[idx] = 15;
			map->terrain.corner_layer_height[idx] = 2;
		}
	}

	map->terrain.update_cliff_meshes({0, 0, terrain_width, terrain_height});
	map->terrain.update_ground_heights({0, 0, terrain_width, terrain_height});
	map->terrain.update_ground_exists({0, 0, terrain_width - 1, terrain_height - 1});
	map->terrain.update_ground_textures({0, 0, terrain_width, terrain_height});
	map->terrain.update_water({0, 0, terrain_width, terrain_height});
	map->terrain.update_minimap();

	map->pathing_map.resize(terrain_width * 4, terrain_height * 4);
	std::fill(map->pathing_map.pathing_cells_static.begin(), map->pathing_map.pathing_cells_static.end(), 0);
	std::fill(map->pathing_map.pathing_cells_dynamic.begin(), map->pathing_map.pathing_cells_dynamic.end(), 0);
	map->pathing_map.upload_static_pathing();
	map->pathing_map.upload_dynamic_pathing();
	map->shadow_map.resize((terrain_width - 1) * 4, (terrain_height - 1) * 4);

	int unplayable_left = 6;
	int unplayable_right = 6;
	int unplayable_bottom = 4;
	int unplayable_top = 8;
	map->set_playable_area(unplayable_left, unplayable_right, unplayable_top, unplayable_bottom);

	map->doodads.doodads.clear();
	map->doodads.special_doodads.clear();
	map->units.units.clear();
	map->units.items.clear();
	map->regions.regions.clear();
	map->cameras.cameras.clear();
	map->sounds.sounds.clear();

	const int default_active_player_count = std::min(4, static_cast<int>(map->info.players.size()));
	for (size_t i = 0; i < map->info.players.size(); ++i) {
		auto& player = map->info.players[i];
		player.internal_number = static_cast<int>(i);
		player.type = static_cast<int>(i) < default_active_player_count ? PlayerType::human : PlayerType::neutral;
		player.race = PlayerRace::selectable;
		player.fixed_start_position = 1;
	}

	const float left = static_cast<float>(unplayable_left + 4);
	const float bottom = static_cast<float>(unplayable_bottom + 4);
	const float right = static_cast<float>(terrain_width - 1 - unplayable_right - 4);
	const float top = static_cast<float>(terrain_height - 1 - unplayable_top - 4);
	const glm::vec2 center((left + right) * 0.5f, (bottom + top) * 0.5f);
	const float radius = std::max(6.f, std::min(right - left, top - bottom) * 0.38f);
	for (int i = 0; i < default_active_player_count; ++i) {
		const float angle = static_cast<float>(i) * static_cast<float>(std::numbers::pi_v<float> * 2.0) / default_active_player_count;
		glm::vec2 start = center + glm::vec2(std::cos(angle), std::sin(angle)) * radius;
		start.x = std::clamp(start.x, left, right);
		start.y = std::clamp(start.y, bottom, top);

		map->info.players[i].starting_position = start * 128.f + map->terrain.offset;
		[[maybe_unused]] Unit& start_location =
			map->units.add_start_location(i, {start.x, start.y, map->terrain.interpolated_height(start.x, start.y, true)});
	}

	map->trigger_strings.set_string(map->info.name, settings.map_name.toStdString());
	map->trigger_strings.set_string(map->info.description, "");
	map->trigger_strings.set_string(map->info.author, "");
	map->trigger_strings.set_string(map->info.suggested_players, "Custom");
	map->info.melee_map = default_active_player_count >= 2;
	map->filesystem_path.clear();
	map->name = sanitized_folder_name(settings.map_name).toStdString();
	map->world_undo.clear_all_undo();

	camera.position = glm::vec3(terrain_width / 2.f, terrain_height / 2.f, 0.f);
	camera.position.z = map->terrain.interpolated_height(camera.position.x, camera.position.y, true);
	map->render_manager.resize_framebuffers(ui->widget->width(), ui->widget->height());
	update_map_context("Draft", "Unsaved map");
	show_transient_notice("New map created");
}

void HiveWE::closeEvent(QCloseEvent* event) {
	if (!confirm_map_replacement("quit")) {
		event->ignore();
		return;
	}

	QApplication::closeAllWindows();
	event->accept();
}

void HiveWE::resizeEvent(QResizeEvent* event) {
	QMainWindow::resizeEvent(event);
	position_transient_notice();
	QTimer::singleShot(0, [&] { save_window_state(); });
}

void HiveWE::moveEvent(QMoveEvent* event) {
	QMainWindow::moveEvent(event);
	position_transient_notice();
	QTimer::singleShot(0, [&] { save_window_state(); });
}

void HiveWE::switch_warcraft() {
	QSettings settings;
	fs::path directory;
	do {
		directory = QFileDialog::getExistingDirectory(this, "Select Warcraft Directory", "/home", QFileDialog::ShowDirsOnly | QFileDialog::DontResolveSymlinks).toStdWString();
		if (directory == "") {
			directory = settings.value("warcraftDirectory").toString().toStdString();
		}
	} while (!hierarchy.open_casc(directory));

	if (directory != hierarchy.warcraft_directory) {
		settings.setValue("warcraftDirectory", QString::fromStdString(directory.string()));
	}
}

// ToDo move to terrain class?
void HiveWE::import_heightmap() {
	QMessageBox::information(this, "Heightmap information", "Will read the red channel and map this onto the range -16 to +16");
	QSettings settings;
	const QString directory = settings.value("openDirectory", QDir::current().path()).toString() + "/" + QString::fromStdString(map->filesystem_path.filename().string());

	QString file_name = QFileDialog::getOpenFileName(this, "Open Heightmap Image", directory);

	if (file_name == "") {
		return;
	}

	int width;
	int height;
	int channels;
	uint8_t* image_data = SOIL_load_image(file_name.toStdString().c_str(), &width, &height, &channels, SOIL_LOAD_AUTO);

	if (width != map->terrain.width || height != map->terrain.height) {
		QMessageBox::warning(this, "Incorrect Image Size", QString("Image Size: %1x%2 does not match terrain size: %3x%4").arg(QString::number(width), QString::number(height), QString::number(map->terrain.width), QString::number(map->terrain.height)));
		return;
	}

	for (int j = 0; j < height; j++) {
		for (int i = 0; i < width; i++) {
			map->terrain.corner_height[map->terrain.ci(i, j)] = (image_data[((height - 1 - j) * width + i) * channels] - 128.f) / 8.f;
		}
	}

	map->terrain.update_ground_heights({ 0, 0, width, height });
	delete image_data;
}

namespace {

QString object_data_default_directory() {
	QSettings settings;
	if (!map) {
		return settings.value("openDirectory", QDir::current().path()).toString();
	}
	return settings.value("openDirectory", QDir::current().path()).toString() + "/" + QString::fromStdString(map->filesystem_path.filename().string());
}

bool ensure_map_loaded_for_object_io(QWidget* parent) {
	if (map && map->loaded) {
		return true;
	}

	QMessageBox::warning(parent, "No Map Loaded", "Open a map before importing or exporting Object Editor data.");
	return false;
}

void show_import_validation_failure(QWidget* parent, const ImportValidationReport& report) {
	QStringList lines;
	for (const ValidationIssue& issue : report.issues) {
		if (issue.severity == ValidationSeverity::error) {
			lines.push_back(QString::fromStdString(issue.message));
		}
	}
	if (lines.isEmpty()) {
		lines.push_back("Import validation failed.");
	}
	QMessageBox::warning(parent, "Import Blocked", lines.join("\n"));
}

QString scenario_controller_label(const PlayerType type) {
	switch (type) {
		case PlayerType::human:
			return "User";
		case PlayerType::computer:
			return "Computer";
		case PlayerType::neutral:
			return "None";
		case PlayerType::rescuable:
			return "Rescuable";
	}
	return "?";
}

QString scenario_race_label(const PlayerRace race) {
	switch (race) {
		case PlayerRace::selectable:
			return "Selectable";
		case PlayerRace::human:
			return "Human";
		case PlayerRace::orc:
			return "Orc";
		case PlayerRace::undead:
			return "Undead";
		case PlayerRace::night_elf:
			return "Night Elf";
	}
	return "?";
}

/// Field-by-field diff between the live map->info and a parsed scenario.json, so the import
/// prompt can show what will actually change instead of a bare "N players, M forces" count.
/// Players match by internal_number (stable slot identity); forces have no persistent ID, so
/// they're matched positionally (by index) as a best-effort heuristic — accurate for the common
/// case of hand-editing an exported file without reordering entries.
QStringList compute_scenario_import_changes(const ScenarioImportData& scenario) {
	QStringList changes;

	for (const auto& imported_player : scenario.players) {
		const auto found = std::ranges::find_if(map->info.players, [&](const PlayerData& player) {
			return player.internal_number == imported_player.internal_number;
		});
		if (found == map->info.players.end()) {
			continue;
		}

		QStringList field_changes;
		const std::string current_name(map->trigger_strings.string(found->name));
		if (current_name != imported_player.name) {
			field_changes << QString("name '%1' → '%2'").arg(QString::fromStdString(current_name), QString::fromStdString(imported_player.name));
		}
		if (found->type != imported_player.controller) {
			field_changes << QString("controller %1 → %2")
								  .arg(scenario_controller_label(found->type), scenario_controller_label(imported_player.controller));
		}
		if (found->race != imported_player.race) {
			field_changes << QString("race %1 → %2").arg(scenario_race_label(found->race), scenario_race_label(imported_player.race));
		}
		const bool current_fixed_start = found->fixed_start_position != 0;
		if (current_fixed_start != imported_player.fixed_start_location) {
			field_changes << QString("fixed start %1 → %2")
								  .arg(current_fixed_start ? "on" : "off", imported_player.fixed_start_location ? "on" : "off");
		}

		if (!field_changes.isEmpty()) {
			changes << QString("Player %1 (%2): %3")
							.arg(imported_player.internal_number + 1)
							.arg(QString::fromStdString(current_name), field_changes.join(", "));
		}
	}

	const auto member_names = [](const std::set<int>& members) {
		QStringList names;
		for (const int internal_number : members) {
			const auto found = std::ranges::find_if(map->info.players, [&](const PlayerData& player) {
				return player.internal_number == internal_number;
			});
			names << (found != map->info.players.end() ? QString::fromStdString(std::string(map->trigger_strings.string(found->name)))
														 : QString("Player %1").arg(internal_number + 1));
		}
		return names.join(", ");
	};

	const size_t force_count = std::max(map->info.forces.size(), scenario.forces.size());
	for (size_t i = 0; i < force_count; ++i) {
		if (i >= map->info.forces.size()) {
			changes << QString("Force added: '%1'").arg(QString::fromStdString(scenario.forces[i].name));
			continue;
		}
		if (i >= scenario.forces.size()) {
			const std::string removed_force_name(map->trigger_strings.string(map->info.forces[i].name));
			changes << QString("Force removed: '%1'").arg(QString::fromStdString(removed_force_name));
			continue;
		}

		const auto& current_force = map->info.forces[i];
		const auto& imported_force = scenario.forces[i];

		QStringList force_changes;
		const std::string current_force_name(map->trigger_strings.string(current_force.name));
		if (current_force_name != imported_force.name) {
			force_changes << QString("name '%1' → '%2'").arg(QString::fromStdString(current_force_name), QString::fromStdString(imported_force.name));
		}

		std::set<int> current_members;
		for (const auto& player : map->info.players) {
			if (current_force.player_masks & (1 << player.internal_number)) {
				current_members.insert(player.internal_number);
			}
		}
		const std::set<int> imported_members(imported_force.players.begin(), imported_force.players.end());
		if (current_members != imported_members) {
			force_changes << QString("members [%1] → [%2]").arg(member_names(current_members), member_names(imported_members));
		}

		if (current_force.allied != imported_force.allied) {
			force_changes << QString("Allied %1").arg(imported_force.allied ? "enabled" : "disabled");
		}
		if (current_force.allied_victory != imported_force.allied_victory) {
			force_changes << QString("Allied Victory %1").arg(imported_force.allied_victory ? "enabled" : "disabled");
		}
		if (current_force.share_vision != imported_force.share_vision) {
			force_changes << QString("Share Vision %1").arg(imported_force.share_vision ? "enabled" : "disabled");
		}
		if (current_force.share_unit_control != imported_force.share_unit_control) {
			force_changes << QString("Share Unit Control %1").arg(imported_force.share_unit_control ? "enabled" : "disabled");
		}
		if (current_force.share_advanced_unit_control != imported_force.share_advanced_unit_control) {
			force_changes << QString("Share Adv. Unit Control %1").arg(imported_force.share_advanced_unit_control ? "enabled" : "disabled");
		}

		if (!force_changes.isEmpty()) {
			changes << QString("Force %1: %2").arg(QString::fromStdString(current_force_name), force_changes.join(", "));
		}
	}

	if (map->info.custom_forces != scenario.custom_forces) {
		changes << QString("Use Custom Forces %1").arg(scenario.custom_forces ? "enabled" : "disabled");
	}
	if (map->info.fixed_player_settings != scenario.fixed_player_settings) {
		changes << QString("Fixed Player Settings %1").arg(scenario.fixed_player_settings ? "enabled" : "disabled");
	}

	return changes;
}

/// Shows the actual field-level diff (not just a player/force count) and lets the user confirm
/// or cancel before anything is applied.
bool show_scenario_import_preview(QWidget* parent, const ScenarioImportData& scenario) {
	const QStringList changes = compute_scenario_import_changes(scenario);

	QDialog dialog(parent);
	dialog.setWindowTitle("Import Scenario Data");
	dialog.resize(700, 480);

	auto* layout = new QVBoxLayout(&dialog);
	layout->addWidget(new QLabel(
		changes.isEmpty() ? "This package's Scenario data (Players/Forces) is identical to the current map — nothing would change."
						  : "This package contains the following Players/Forces changes:"
	));

	auto* changes_list = new QListWidget;
	changes_list->addItems(changes);
	layout->addWidget(changes_list, 1);

	auto* buttons = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
	buttons->button(QDialogButtonBox::Ok)->setText("Apply");
	buttons->button(QDialogButtonBox::Ok)->setEnabled(!changes.isEmpty());
	layout->addWidget(buttons);
	QObject::connect(buttons, &QDialogButtonBox::accepted, &dialog, &QDialog::accept);
	QObject::connect(buttons, &QDialogButtonBox::rejected, &dialog, &QDialog::reject);

	return dialog.exec() == QDialog::Accepted;
}

/// Scenario (Players/Forces) data has no per-object conflict/merge semantics like the SLK-backed
/// object categories do, so it rides along as an optional wholesale-replace prompt instead of
/// going through ObjectDataImportDialog's per-object conflict review.
void prompt_apply_scenario_import(QWidget* parent, const std::optional<ScenarioImportData>& scenario) {
	if (!scenario || !map) {
		return;
	}

	if (!show_scenario_import_preview(parent, *scenario)) {
		return;
	}

	apply_scenario_import(*scenario, map->info, map->trigger_strings);

	if (const auto open_editor = window_handler.get_open<ScenarioPropertiesEditor>(); open_editor.has_value() && *open_editor) {
		(*open_editor)->refresh_all();
	}
}

} // namespace

void HiveWE::export_object_data_binary() {
	if (!ensure_map_loaded_for_object_io(this)) {
		return;
	}

	const QString directory = QFileDialog::getExistingDirectory(this, "Export Object Data (Binary)", object_data_default_directory());
	if (directory.isEmpty()) {
		return;
	}

	if (const auto result = export_binary_object_data(directory.toStdString()); !result) {
		QMessageBox::warning(this, "Export Failed", QString::fromStdString(result.error()));
		return;
	}

	show_transient_notice("Exported Object Editor binary package.");
}

void HiveWE::export_object_data_text() {
	if (!ensure_map_loaded_for_object_io(this)) {
		return;
	}

	const QString directory = QFileDialog::getExistingDirectory(this, "Export Object Data (Text)", object_data_default_directory());
	if (directory.isEmpty()) {
		return;
	}

	if (const auto result = export_text_object_data(directory.toStdString()); !result) {
		QMessageBox::warning(this, "Export Failed", QString::fromStdString(result.error()));
		return;
	}

	show_transient_notice("Exported Object Editor text package.");
}

void HiveWE::import_object_data_binary() {
	if (!ensure_map_loaded_for_object_io(this)) {
		return;
	}

	const QString directory = QFileDialog::getExistingDirectory(this, "Import Object Data (Binary)", object_data_default_directory());
	if (directory.isEmpty()) {
		return;
	}

	const auto loaded = load_binary_object_data(directory.toStdString(), true);
	if (!loaded) {
		show_import_validation_failure(this, loaded.error());
		return;
	}

	const std::optional<ScenarioImportData> scenario = loaded->scenario;

	ObjectDataImportDialog dialog(std::move(*loaded), this);
	if (dialog.exec() == QDialog::Accepted && dialog.dialog_result() == ObjectDataImportDialog::Result::applied) {
		show_transient_notice("Imported Object Editor data.");
	}

	prompt_apply_scenario_import(this, scenario);
}

void HiveWE::import_object_data_map_folder() {
	if (!ensure_map_loaded_for_object_io(this)) {
		return;
	}

	const QString directory = QFileDialog::getExistingDirectory(this, "Import Object Data (Map Folder)", object_data_default_directory());
	if (directory.isEmpty()) {
		return;
	}

	const auto loaded = load_binary_object_data(directory.toStdString(), false);
	if (!loaded) {
		show_import_validation_failure(this, loaded.error());
		return;
	}

	const std::optional<ScenarioImportData> scenario = loaded->scenario;

	ObjectDataImportDialog dialog(std::move(*loaded), this);
	if (dialog.exec() == QDialog::Accepted && dialog.dialog_result() == ObjectDataImportDialog::Result::applied) {
		show_transient_notice("Imported Object Editor data from map folder.");
	}

	prompt_apply_scenario_import(this, scenario);
}

void HiveWE::import_object_data_text() {
	if (!ensure_map_loaded_for_object_io(this)) {
		return;
	}

	const QString directory = QFileDialog::getExistingDirectory(this, "Import Object Data (Text)", object_data_default_directory());
	if (directory.isEmpty()) {
		return;
	}

	const auto loaded = load_text_object_data(directory.toStdString());
	if (!loaded) {
		show_import_validation_failure(this, loaded.error());
		return;
	}

	const std::optional<ScenarioImportData> scenario = loaded->scenario;

	ObjectDataImportDialog dialog(std::move(*loaded), this);
	if (dialog.exec() == QDialog::Accepted && dialog.dialog_result() == ObjectDataImportDialog::Result::applied) {
		show_transient_notice("Imported Object Editor text data.");
	}

	prompt_apply_scenario_import(this, scenario);
}

void HiveWE::save_window_state() {
	QSettings settings;

	if (!isMaximized()) {
		settings.setValue("MainWindow/geometry", saveGeometry());
	}

	settings.setValue("MainWindow/maximized", isMaximized());
	settings.setValue("MainWindow/windowState", saveState());
}

void HiveWE::restore_window_state() {
	QSettings settings;

	if (settings.contains("MainWindow/windowState")) {
		restoreGeometry(settings.value("MainWindow/geometry").toByteArray());
		restoreState(settings.value("MainWindow/windowState").toByteArray());
		if (settings.value("MainWindow/maximized").toBool()) {
			showMaximized();
		} else {
			showNormal();
		}
	} else {
		showMaximized();
	}
}

void HiveWE::set_current_custom_tab(QRibbonTab* tab, QString name) {
	if (current_custom_tab == tab) {
		return;
	}

	if (current_custom_tab != nullptr) {
		emit palette_changed(tab);
	}

	remove_custom_tab();
	current_custom_tab = tab;
	ui->ribbon->addTab(tab, name);
	ui->ribbon->setCurrentIndex(ui->ribbon->count() - 1);
}

void HiveWE::remove_custom_tab() {
	for (int i = 0; i < ui->ribbon->count(); i++) {
		if (ui->ribbon->widget(i) == current_custom_tab) {
			ui->ribbon->removeTab(i);
			current_custom_tab = nullptr;
			return;
		}
	}
}

Palette* HiveWE::palette_for_view(const SidebarPaletteView view) const {
	switch (view) {
	case SidebarPaletteView::terrain:
		return terrain_palette;
	case SidebarPaletteView::doodad:
		return doodad_palette;
	case SidebarPaletteView::unit:
		return unit_palette;
	case SidebarPaletteView::pathing:
		return pathing_palette;
	case SidebarPaletteView::region:
		return region_palette;
	case SidebarPaletteView::none:
		return nullptr;
	}

	return nullptr;
}

QWidget* HiveWE::host_for_view(const SidebarPaletteView view) const {
	switch (view) {
	case SidebarPaletteView::terrain:
		return terrain_host;
	case SidebarPaletteView::doodad:
		return doodad_host;
	case SidebarPaletteView::unit:
		return unit_host;
	case SidebarPaletteView::pathing:
		return pathing_host;
	case SidebarPaletteView::region:
		return region_host;
	case SidebarPaletteView::none:
		return nullptr;
	}

	return nullptr;
}

QString HiveWE::sidebar_title_for_view(const SidebarPaletteView view) const {
	switch (view) {
	case SidebarPaletteView::terrain:
		return "Terrain";
	case SidebarPaletteView::doodad:
		return "Doodads";
	case SidebarPaletteView::unit:
		return "Units";
	case SidebarPaletteView::pathing:
		return "Pathing";
	case SidebarPaletteView::region:
		return "Regions";
	case SidebarPaletteView::none:
		return "";
	}

	return "";
}

QString HiveWE::sidebar_description_for_view(const SidebarPaletteView view) const {
	switch (view) {
	case SidebarPaletteView::terrain:
		return "Brush families, tiles, and terrain operations in one focused panel without stacking unrelated palettes below it.";
	case SidebarPaletteView::doodad:
		return "Search, filter, and place doodads with the active selection and placement rules kept in one consistent sidebar.";
	case SidebarPaletteView::unit:
		return "Pick units by owner and search context, then keep selection-aware placement tools close to the active asset.";
	case SidebarPaletteView::pathing:
		return "Pathing brush operations, mask selection, and import or export tools in a dedicated utility view.";
	case SidebarPaletteView::region:
		return "Region selection, creation, and property editing in one compact mode-specific inspector.";
	case SidebarPaletteView::none:
		return "";
	}

	return "";
}

QString HiveWE::sidebar_hint_for_view(const SidebarPaletteView view) const {
	switch (view) {
	case SidebarPaletteView::terrain:
		return "Shortcut: T. Space switches selection mode where relevant.";
	case SidebarPaletteView::doodad:
		return "Shortcut: D. Ctrl+F jumps to search. Middle-click can sync from the map selection.";
	case SidebarPaletteView::unit:
		return "Shortcut: U. Ctrl+F jumps to search and player ownership stays visible in the palette.";
	case SidebarPaletteView::pathing:
		return "Shortcut: P. Keep pathing operation and brush mask readable while the viewport remains dominant.";
	case SidebarPaletteView::region:
		return "Shortcut: R. Keep region properties close to the selected region instead of in a separate dialog flow.";
	case SidebarPaletteView::none:
		return "";
	}

	return "";
}

QString HiveWE::sidebar_feedback_label(const SidebarPaletteView view) const {
	switch (view) {
	case SidebarPaletteView::terrain:
		return "Terrain sidebar";
	case SidebarPaletteView::doodad:
		return "Doodad sidebar";
	case SidebarPaletteView::unit:
		return "Unit sidebar";
	case SidebarPaletteView::pathing:
		return "Pathing sidebar";
	case SidebarPaletteView::region:
		return "Region sidebar";
	case SidebarPaletteView::none:
		return "Sidebar";
	}

	return "Sidebar";
}

void HiveWE::ensure_palette_view(const SidebarPaletteView view) {
	if ((view == SidebarPaletteView::terrain || view == SidebarPaletteView::pathing) && !terrain_palette) {
		terrain_palette = new TerrainPalette(terrain_host);
		terrain_host->layout()->addWidget(terrain_palette);
		connect(terrain_palette, &Palette::activated, this, [this]() { activate_palette(terrain_palette); });
		connect(terrain_palette, &Palette::ribbon_tab_requested, this, &HiveWE::set_current_custom_tab);
		connect(this, &HiveWE::palette_changed, terrain_palette, &Palette::deactivate);
	}

	if ((view == SidebarPaletteView::terrain || view == SidebarPaletteView::pathing) && !pathing_palette) {
		pathing_palette = new PathingPalette(pathing_host);
		pathing_host->layout()->addWidget(pathing_palette);
		connect(pathing_palette, &Palette::activated, this, [this]() { activate_palette(pathing_palette); });
		connect(pathing_palette, &Palette::ribbon_tab_requested, this, &HiveWE::set_current_custom_tab);
		connect(this, &HiveWE::palette_changed, pathing_palette, &Palette::deactivate);
		connect(&pathing_palette->current_brush(), &Brush::size_changed, this, [this](glm::ivec2) {
			if (terrain_palette) {
				const QSignalBlocker blocker(terrain_palette->current_brush());
				terrain_palette->current_brush().set_size(pathing_palette->current_brush().get_size());
				terrain_palette->sync_brush_controls(map->brush);
			}
		});
		if (terrain_palette) {
			connect(&terrain_palette->current_brush(), &Brush::size_changed, this, [this](glm::ivec2) {
				if (pathing_palette) {
					const QSignalBlocker blocker(pathing_palette->current_brush());
					pathing_palette->current_brush().set_size(terrain_palette->current_brush().get_size());
					terrain_palette->sync_brush_controls(map->brush);
				}
			});
		}
	}

	if (view == SidebarPaletteView::doodad && !doodad_palette) {
		doodad_palette = new DoodadPalette(doodad_host);
		doodad_host->layout()->addWidget(doodad_palette);
		connect(doodad_palette, &Palette::activated, this, [this]() { activate_palette(doodad_palette); });
		connect(doodad_palette, &Palette::ribbon_tab_requested, this, &HiveWE::set_current_custom_tab);
		connect(doodad_palette, &DoodadPalette::preview_doodad_changed, this, &HiveWE::update_doodad_model_preview);
		connect(this, &HiveWE::palette_changed, doodad_palette, &Palette::deactivate);
	}

	if (view == SidebarPaletteView::unit && !unit_palette) {
		unit_palette = new UnitPalette(unit_host);
		unit_host->layout()->addWidget(unit_palette);
		connect(unit_palette, &Palette::activated, this, [this]() { activate_palette(unit_palette); });
		connect(unit_palette, &Palette::ribbon_tab_requested, this, &HiveWE::set_current_custom_tab);
		connect(unit_palette, &UnitPalette::preview_unit_changed, this, &HiveWE::update_unit_model_preview);
		connect(this, &HiveWE::palette_changed, unit_palette, &Palette::deactivate);
	}

	if (view == SidebarPaletteView::region && !region_palette) {
		region_palette = new RegionPalette(region_host);
		region_host->layout()->addWidget(region_palette);
		connect(region_palette, &Palette::activated, this, [this]() { activate_palette(region_palette); });
		connect(region_palette, &Palette::ribbon_tab_requested, this, &HiveWE::set_current_custom_tab);
		connect(this, &HiveWE::palette_changed, region_palette, &Palette::deactivate);
	}
}

void HiveWE::reset_map_session() {
	delete terrain_palette;
	terrain_palette = nullptr;
	delete pathing_palette;
	pathing_palette = nullptr;
	delete doodad_palette;
	doodad_palette = nullptr;
	delete unit_palette;
	unit_palette = nullptr;
	delete region_palette;
	region_palette = nullptr;
	remove_custom_tab();
	current_sidebar_view = SidebarPaletteView::none;
	sidebar_root->hide();
	update_palette_model_preview(nullptr, "Model Preview");
	ui->widget->makeCurrent();
	ui->ribbon->terrain_palette->setChecked(false);
	ui->ribbon->doodad_palette->setChecked(false);
	ui->ribbon->unit_palette->setChecked(false);
	ui->ribbon->pathing_palette->setChecked(false);
	ui->ribbon->region_palette->setChecked(false);
	update_active_palette_visuals();

	delete map;
	resource_manager.clear();
	skinned_mesh_globals.reset();
	map = new Map();

	connect(&map->terrain, &Terrain::minimap_changed, minimap, &Minimap::set_minimap);
	connect(&map->terrain, &Terrain::tileset_changed, [&]() {
		if (terrain_palette) {
			terrain_palette->refresh();
		}
	});
}

void HiveWE::update_map_context(const QString& status, const QString& detail_override) {
	QString detail = detail_override;
	QString map_title = QString::fromStdString(map->name);

	if (!map->filesystem_path.empty()) {
		const QFileInfo map_info(QString::fromStdString(map->filesystem_path.string()));
		setWindowTitle("HiveWE 0.21.1 - " + QString::fromStdString(map->filesystem_path.string()));
		map_title = map_info.fileName().isEmpty() ? map_title : map_info.fileName();
		if (detail.isEmpty()) {
			detail = map_info.absoluteFilePath();
		}
	} else {
		setWindowTitle("HiveWE 0.21.1 - " + map_title + " (Unsaved)");
		if (detail.isEmpty()) {
			detail = "Unsaved map";
		}
	}

	ui->ribbon->set_map_context(map_title, detail, status);
}

bool HiveWE::confirm_map_replacement(const QString& action) {
	if (!map || !map->loaded) {
		return true;
	}

	const bool has_unsaved_changes = map->filesystem_path.empty() || map->world_undo.has_pending_changes();
	if (!has_unsaved_changes) {
		return true;
	}

	QMessageBox message_box(this);
	message_box.setIcon(QMessageBox::Question);
	message_box.setWindowTitle("Unsaved changes");
	message_box.setText("The current map has unsaved changes.");
	message_box.setInformativeText("Do you want to save before you " + action + "?");
	message_box.setStandardButtons(QMessageBox::Save | QMessageBox::Discard | QMessageBox::Cancel);
	message_box.setDefaultButton(QMessageBox::Save);

	switch (message_box.exec()) {
		case QMessageBox::Save:
			return save_current_map();
		case QMessageBox::Discard:
			return true;
		default:
			return false;
	}
}

QString HiveWE::sanitized_folder_name(const QString& map_name) {
	QString sanitized = map_name.trimmed();
	if (sanitized.isEmpty()) {
		sanitized = "New Map";
	}

	sanitized.replace(QRegularExpression(R"([<>:"/\\|?*\x00-\x1F])"), "_");
	sanitized.replace(QRegularExpression(R"(\s+)"), " ");
	sanitized = sanitized.trimmed();
	if (sanitized.isEmpty()) {
		sanitized = "New Map";
	}

	return sanitized;
}

void HiveWE::update_active_palette_visuals() {
	const bool sidebar_visible = sidebar_root && sidebar_root->isVisible() && current_sidebar_view != SidebarPaletteView::none;
	const bool preview_visible = sidebar_visible && (current_sidebar_view == SidebarPaletteView::doodad || current_sidebar_view == SidebarPaletteView::unit);

	auto set_checked = [](QAbstractButton* button, const bool checked) {
		if (!button) {
			return;
		}

		const QSignalBlocker blocker(button);
		button->setChecked(checked);
	};

	set_checked(ui->ribbon->terrain_palette, sidebar_visible && current_sidebar_view == SidebarPaletteView::terrain);
	set_checked(ui->ribbon->doodad_palette, sidebar_visible && current_sidebar_view == SidebarPaletteView::doodad);
	set_checked(ui->ribbon->unit_palette, sidebar_visible && current_sidebar_view == SidebarPaletteView::unit);
	set_checked(ui->ribbon->pathing_palette, sidebar_visible && current_sidebar_view == SidebarPaletteView::pathing);
	set_checked(ui->ribbon->region_palette, sidebar_visible && current_sidebar_view == SidebarPaletteView::region);
	set_checked(terrain_mode_button, sidebar_visible && current_sidebar_view == SidebarPaletteView::terrain);
	set_checked(doodad_mode_button, sidebar_visible && current_sidebar_view == SidebarPaletteView::doodad);
	set_checked(unit_mode_button, sidebar_visible && current_sidebar_view == SidebarPaletteView::unit);
	set_checked(pathing_mode_button, sidebar_visible && current_sidebar_view == SidebarPaletteView::pathing);
	set_checked(region_mode_button, sidebar_visible && current_sidebar_view == SidebarPaletteView::region);

	if (sidebar_preview_frame) {
		sidebar_preview_frame->setVisible(preview_visible);
	}

	if (!sidebar_visible) {
		sidebar_title->clear();
		sidebar_description->clear();
		sidebar_hint->clear();
		return;
	}

	sidebar_title->setText(sidebar_title_for_view(current_sidebar_view));
	sidebar_description->setText(sidebar_description_for_view(current_sidebar_view));
	sidebar_hint->setText(sidebar_hint_for_view(current_sidebar_view));
}

void HiveWE::hide_palette_sidebar() {
	const SidebarPaletteView previous_view = current_sidebar_view;
	current_sidebar_view = SidebarPaletteView::none;
	sidebar_root->hide();

	if (auto* palette = palette_for_view(previous_view)) {
		if ((terrain_palette && palette == terrain_palette && map->brush == &terrain_palette->current_brush())
			|| (pathing_palette && palette == pathing_palette && map->brush == &pathing_palette->current_brush())
			|| (doodad_palette && palette == doodad_palette && map->brush == &doodad_palette->current_brush())
			|| (unit_palette && palette == unit_palette && map->brush == &unit_palette->current_brush())
			|| (region_palette && palette == region_palette && map->brush == &region_palette->current_brush())) {
			map->brush = nullptr;
		}
	}

	remove_custom_tab();
	update_active_palette_visuals();
}

void HiveWE::show_palette_view(const SidebarPaletteView view, const bool show_feedback) {
	ensure_palette_view(view);

	auto* host = host_for_view(view);
	auto* palette = palette_for_view(view);
	if (!host || !palette) {
		return;
	}

	current_sidebar_view = view;
	sidebar_stack->setCurrentWidget(host);
	sidebar_root->show();
	switch (view) {
	case SidebarPaletteView::terrain:
		ui->ribbon->show_tab(MainRibbon::HeaderTab::terrain);
		break;
	case SidebarPaletteView::doodad:
		ui->ribbon->show_tab(MainRibbon::HeaderTab::doodads);
		break;
	case SidebarPaletteView::unit:
		ui->ribbon->show_tab(MainRibbon::HeaderTab::units);
		break;
	case SidebarPaletteView::pathing:
		ui->ribbon->show_tab(MainRibbon::HeaderTab::pathing);
		break;
	case SidebarPaletteView::region:
		ui->ribbon->show_tab(MainRibbon::HeaderTab::regions);
		break;
	case SidebarPaletteView::none:
		break;
	}
	update_active_palette_visuals();
	activate_palette(palette, false);

	if (show_feedback) {
		show_transient_notice(sidebar_feedback_label(view), 850);
	}
}

void HiveWE::activate_palette(Palette* palette, const bool show_feedback) {
	bool already_active = false;
	if (terrain_palette && palette == terrain_palette) {
		already_active = map->brush == &terrain_palette->current_brush();
	} else if (pathing_palette && palette == pathing_palette) {
		already_active = map->brush == &pathing_palette->current_brush();
	} else if (doodad_palette && palette == doodad_palette) {
		already_active = map->brush == &doodad_palette->current_brush();
	} else if (unit_palette && palette == unit_palette) {
		already_active = map->brush == &unit_palette->current_brush();
	} else if (region_palette && palette == region_palette) {
		already_active = map->brush == &region_palette->current_brush();
	}

	if (terrain_palette && pathing_palette) {
		if (auto* terrain = dynamic_cast<TerrainPalette*>(palette)) {
			if (dynamic_cast<PathingBrush*>(map->brush)) {
				const QSignalBlocker blocker(terrain->current_brush());
				terrain->current_brush().set_size(pathing_palette->current_brush().get_size());
			}
		} else if (auto* pathing = dynamic_cast<PathingPalette*>(palette)) {
			if (dynamic_cast<TerrainBrush*>(map->brush)) {
				const QSignalBlocker blocker(pathing->current_brush());
				pathing->current_brush().set_size(terrain_palette->current_brush().get_size());
			}
		}
	}

	if (auto* terrain = dynamic_cast<TerrainPalette*>(palette)) {
		terrain->activate_palette();
	} else if (auto* pathing = dynamic_cast<PathingPalette*>(palette)) {
		pathing->activate_palette();
	} else if (auto* doodad = dynamic_cast<DoodadPalette*>(palette)) {
		doodad->activate_palette();
	} else if (auto* unit = dynamic_cast<UnitPalette*>(palette)) {
		unit->activate_palette();
	} else if (auto* region = dynamic_cast<RegionPalette*>(palette)) {
		region->activate_palette();
	}

	if (terrain_palette) {
		terrain_palette->sync_brush_controls(map->brush);
	}

	update_active_palette_visuals();

	if (!show_feedback || already_active) {
		return;
	}

	QString label = "Palette active";
	if (dynamic_cast<TerrainPalette*>(palette)) {
		label = "Terrain mode";
	} else if (dynamic_cast<PathingPalette*>(palette)) {
		label = "Pathing mode";
	} else if (dynamic_cast<DoodadPalette*>(palette)) {
		label = "Doodad mode";
	} else if (dynamic_cast<UnitPalette*>(palette)) {
		label = "Unit mode";
	} else if (dynamic_cast<RegionPalette*>(palette)) {
		label = "Region mode";
	}

	show_transient_notice(label, 850);
}

void HiveWE::toggle_terrain_sidebar() {
	if (sidebar_root->isVisible() && current_sidebar_view == SidebarPaletteView::terrain) {
		hide_palette_sidebar();
		show_transient_notice("Terrain sidebar hidden", 700);
		return;
	}

	show_palette_view(SidebarPaletteView::terrain, false);
	show_transient_notice("Terrain sidebar shown", 700);
}

void HiveWE::toggle_pathing_sidebar() {
	if (sidebar_root->isVisible() && current_sidebar_view == SidebarPaletteView::pathing) {
		hide_palette_sidebar();
		show_transient_notice("Pathing sidebar hidden", 700);
		return;
	}

	show_palette_view(SidebarPaletteView::pathing, false);
	show_transient_notice("Pathing sidebar shown", 700);
}

void HiveWE::toggle_doodad_palette() {
	if (sidebar_root->isVisible() && current_sidebar_view == SidebarPaletteView::doodad) {
		hide_palette_sidebar();
		show_transient_notice("Doodad sidebar hidden", 700);
		return;
	}

	show_palette_view(SidebarPaletteView::doodad, false);
	show_transient_notice("Doodad sidebar shown", 700);
}

void HiveWE::toggle_unit_palette() {
	if (sidebar_root->isVisible() && current_sidebar_view == SidebarPaletteView::unit) {
		hide_palette_sidebar();
		show_transient_notice("Unit sidebar hidden", 700);
		return;
	}

	show_palette_view(SidebarPaletteView::unit, false);
	show_transient_notice("Unit sidebar shown", 700);
}

void HiveWE::toggle_region_palette() {
	if (sidebar_root->isVisible() && current_sidebar_view == SidebarPaletteView::region) {
		hide_palette_sidebar();
		show_transient_notice("Region sidebar hidden", 700);
		return;
	}

	show_palette_view(SidebarPaletteView::region, false);
	show_transient_notice("Region sidebar shown", 700);
}

void HiveWE::update_palette_model_preview(std::shared_ptr<mdx::MDX> model, const QString& title) {
	if (!sidebar_preview_title || !sidebar_preview_placeholder) {
		return;
	}

	sidebar_preview_title->setText(title.isEmpty() ? "Model Preview" : title);
	if (!sidebar_preview && model) {
		sidebar_preview = new ModelEditorGLWidget(sidebar_preview_frame, nullptr, true);
		// Adaptive: keep a modest minimum so the preview can shrink on short windows.
		sidebar_preview->setMinimumHeight(80);
		sidebar_preview->setMaximumHeight(220);
		sidebar_preview->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Preferred);
		if (auto* preview_layout = qobject_cast<QVBoxLayout*>(sidebar_preview_frame->layout())) {
			preview_layout->insertWidget(1, sidebar_preview);
		}
	}

	if (!sidebar_preview) {
		sidebar_preview_placeholder->setVisible(true);
		sidebar_preview_placeholder->setText("No preview model available.");
		return;
	}

	sidebar_preview->set_model(std::move(model));

	const bool has_model = sidebar_preview->mdx != nullptr;
	sidebar_preview->setVisible(has_model);
	sidebar_preview_placeholder->setVisible(!has_model);
	if (!has_model) {
		sidebar_preview_placeholder->setText("No preview model available.");
	}
}

void HiveWE::update_doodad_model_preview(const QString& id, const int variation, const QString& title) {
	QTimer::singleShot(300, this, [this, id, variation, title]() {
		if (QApplication::activeModalWidget()) {
			QTimer::singleShot(300, this, [this, id, variation, title]() {
				update_doodad_model_preview(id, variation, title);
			});
			return;
		}

		if (id.isEmpty()) {
			update_palette_model_preview(nullptr, title);
			return;
		}

		const auto mesh = map->doodads.get_mesh(id.toStdString(), variation);
		update_palette_model_preview(load_preview_mdx(mesh->path), title);
	});
}

void HiveWE::update_unit_model_preview(const QString& id, const QString& title) {
	QTimer::singleShot(300, this, [this, id, title]() {
		if (QApplication::activeModalWidget()) {
			QTimer::singleShot(300, this, [this, id, title]() {
				update_unit_model_preview(id, title);
			});
			return;
		}

		if (id.isEmpty()) {
			update_palette_model_preview(nullptr, title);
			return;
		}

		const auto mesh = map->units.get_mesh(id.toStdString());
		update_palette_model_preview(load_preview_mdx(mesh->path), title);
	});
}

void HiveWE::show_transient_notice(const QString& text, int timeout_ms) {
	transient_notice->setText(text);
	transient_notice->adjustSize();
	transient_notice->resize(transient_notice->width() + 24, transient_notice->height() + 16);
	position_transient_notice();
	transient_notice->show();
	transient_notice->raise();
	transient_notice_timer.start(timeout_ms);
}

void HiveWE::position_transient_notice() {
	if (!transient_notice) {
		return;
	}

	const int x = (width() - transient_notice->width()) / 2;
	const int y = (height() - transient_notice->height()) / 2;
	transient_notice->move(std::max(0, x), std::max(0, y));
}
