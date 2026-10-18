---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/main_window/hivewe.h"
ref: HEAD
---

#pragma once

#include <filesystem>

namespace fs = std::filesystem;

#include <QMainWindow>
#include <QFileDialog>
#include <QSettings>
#include <QObject>
#include <QTimer>
#include <QElapsedTimer>
#include <QGridLayout>
#include <QLabel>
#include <QMenu>
#include <QPainter>
#include <QKeyEvent>
#include <QStackedWidget>
#include <QToolButton>
#include <QString>
#include <memory>

#include "global_search.h"

import QRibbon;
import WindowHandler;
import <glm/glm.hpp>;
import <glm/gtc/matrix_transform.hpp>;
import <glm/gtc/quaternion.hpp>;
import "palette.h";
import "minimap.h";

namespace Ui {
class HiveWEClass;
}

namespace mdx {
class MDX;
}

class TerrainPalette;
class PathingPalette;
class DoodadPalette;
class UnitPalette;
class RegionPalette;
class ModelEditorGLWidget;

class HiveWE : public QMainWindow {
	Q_OBJECT

public:
	explicit HiveWE(QWidget* parent = nullptr);
	~HiveWE();

	void load_map(const fs::path& directory);

	void load_folder();
	void load_mpq();
	void save();
	void save_as();
	void export_mpq();
	void play_test();

private:
	enum class SidebarPaletteView {
		none,
		terrain,
		doodad,
		unit,
		pathing,
		region,
	};

	std::unique_ptr<Ui::HiveWEClass> ui;
	QRibbonTab* current_custom_tab = nullptr;
	Minimap* minimap = new Minimap(this);

	QElapsedTimer double_shift_timer;

	void keyPressEvent(QKeyEvent* event) override {
		if (event->key() == Qt::Key_Shift && !event->isAutoRepeat()) {
			if (double_shift_timer.isValid() && double_shift_timer.elapsed() < 400) {

				GlobalSearchWidget search_widget = new GlobalSearchWidget(this);
				double_shift_timer.invalidate();
			} else {
				double_shift_timer.start();
			}
		}
		QMainWindow::keyPressEvent(event);
	}

	void closeEvent(QCloseEvent* event) override;
	void resizeEvent(QResizeEvent* event) override;
	void moveEvent(QMoveEvent* event) override;

	void switch_warcraft();
	void import_heightmap();
	void export_object_data_binary();
	void export_object_data_text();
	void import_object_data_binary();
	void import_object_data_map_folder();
	void import_object_data_text();
	void create_new_map();
	void save_window_state();
	void restore_window_state();
	void add_recent_map(const fs::path& directory);
	void update_recent_maps_menu();
	void open_recent_map(int index);
	void show_transient_notice(const QString& text, int timeout_ms = 1800);
	void position_transient_notice();
	void setup_palette_sidebar();
	void update_active_palette_visuals();
	void hide_palette_sidebar();
	void show_palette_view(SidebarPaletteView view, bool show_feedback = true);
	void ensure_palette_view(SidebarPaletteView view);
	void reset_map_session();
	void update_map_context(const QString& status, const QString& detail_override = {});
	bool save_current_map();
	bool save_current_map_as();
	bool confirm_map_replacement(const QString& action);
	static QString sanitized_folder_name(const QString& map_name);
	Palette* palette_for_view(SidebarPaletteView view) const;
	QWidget* host_for_view(SidebarPaletteView view) const;
	QString sidebar_title_for_view(SidebarPaletteView view) const;
	QString sidebar_description_for_view(SidebarPaletteView view) const;
	QString sidebar_hint_for_view(SidebarPaletteView view) const;
	QString sidebar_feedback_label(SidebarPaletteView view) const;
	void activate_palette(Palette* palette, bool show_feedback = true);
	void toggle_terrain_sidebar();
	void toggle_pathing_sidebar();
	void toggle_doodad_palette();
	void toggle_unit_palette();
	void toggle_region_palette();
	void update_palette_model_preview(std::shared_ptr<mdx::MDX> model, const QString& title);
	void update_doodad_model_preview(const QString& id, int variation, const QString& title);
	void update_unit_model_preview(const QString& id, const QString& title);

	/// Adds the tab to the ribbon and sets the current index to this tab
	void set_current_custom_tab(QRibbonTab* tab, QString name);
	void remove_custom_tab();

	QLabel* transient_notice = nullptr;
	QTimer transient_notice_timer;
	QWidget* sidebar_root = nullptr;
	QWidget* sidebar_header = nullptr;
	QWidget* sidebar_mode_bar = nullptr;
	QLabel* sidebar_title = nullptr;
	QLabel* sidebar_description = nullptr;
	QLabel* sidebar_hint = nullptr;
	QFrame* sidebar_minimap_frame = nullptr;
	QFrame* sidebar_preview_frame = nullptr;
	QLabel* sidebar_preview_title = nullptr;
	QLabel* sidebar_preview_placeholder = nullptr;
	ModelEditorGLWidget* sidebar_preview = nullptr;
	QStackedWidget* sidebar_stack = nullptr;
	QToolButton* terrain_mode_button = nullptr;
	QToolButton* doodad_mode_button = nullptr;
	QToolButton* unit_mode_button = nullptr;
	QToolButton* pathing_mode_button = nullptr;
	QToolButton* region_mode_button = nullptr;
	QFrame* doodad_host = nullptr;
	QFrame* unit_host = nullptr;
	QFrame* terrain_host = nullptr;
	QFrame* pathing_host = nullptr;
	QFrame* region_host = nullptr;
	SidebarPaletteView current_sidebar_view = SidebarPaletteView::none;
	TerrainPalette* terrain_palette = nullptr;
	PathingPalette* pathing_palette = nullptr;
	DoodadPalette* doodad_palette = nullptr;
	UnitPalette* unit_palette = nullptr;
	RegionPalette* region_palette = nullptr;

signals:
	void tileset_changed();
	void palette_changed(QRibbonTab* tab);

	void saving_initiated();
};
