---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/scenario_properties_editor.h"
ref: HEAD
---

#pragma once

#include <QDialog>
#include <QTreeWidget>

class QCheckBox;
class QPushButton;
class QTableWidget;
class QTableWidgetItem;
class QTabWidget;
class QDragMoveEvent;
class QDropEvent;

/// A QTreeWidget where forces are top-level items and their member players are
/// draggable leaves. Dropping a player leaf onto a different force reassigns it.
class ForceTreeWidget: public QTreeWidget {
	Q_OBJECT

  public:
	explicit ForceTreeWidget(QWidget* parent = nullptr);

  signals:
	void playerDropped(int player_internal_number, int target_force_row);

  protected:
	void dragMoveEvent(QDragMoveEvent* event) override;
	void dropEvent(QDropEvent* event) override;
};

class ScenarioPropertiesEditor: public QDialog {
	Q_OBJECT

  public:
	explicit ScenarioPropertiesEditor(QWidget* parent = nullptr);

	/// Refreshes every control from map->info. Used after an external mutation
	/// (e.g. a Scenario Data import) so an already-open dialog doesn't go stale.
	void refresh_all();

  private:
	void ensure_all_player_slots_exist();

	void build_players_tab(QWidget* tab);
	void refresh_players_table();
	void reset_players_to_defaults();
	void apply_player_row_active_style(int row, bool is_active);

	void build_forces_tab(QWidget* tab);
	void refresh_forces_tree();
	void refresh_forces_flags_table();
	void ensure_every_player_has_a_force();
	void sync_forces_after_controller_change();
	void add_force();
	void remove_force();
	void rename_selected_force();
	void on_player_dropped(int player_internal_number, int target_force_row);
	void update_force_buttons_enabled();

	QTabWidget* tabs = nullptr;

	QTableWidget* players_table = nullptr;

	ForceTreeWidget* forces_tree = nullptr;
	QTableWidget* forces_flags_table = nullptr;
	QPushButton* add_force_button = nullptr;
	QPushButton* remove_force_button = nullptr;
	QPushButton* rename_force_button = nullptr;
	QCheckBox* use_custom_forces_checkbox = nullptr;
	QCheckBox* fixed_player_settings_checkbox = nullptr;

	// Guards against refresh_*() re-entering the itemChanged/toggled handlers
	// while it is repopulating widgets from map->info.
	bool updating_ui = false;
};
