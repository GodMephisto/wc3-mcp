---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/unit_properties_dialog.h"
ref: HEAD
---

#pragma once

#include <QDialog>
#include <vector>

import Units;
import Utilities;

class QCheckBox;
class QComboBox;
class QDoubleSpinBox;
class QLabel;
class QListWidget;
class QPushButton;
class QSpinBox;
class QTabWidget;

class UnitPropertiesDialog : public QDialog {
	Q_OBJECT

  public:
	UnitPropertiesDialog(Unit* unit, QWidget* parent = nullptr);

  private:
	void refresh_attribute_state() const;
	void refresh_item_drop_mode_state();
	void refresh_map_item_table_options();
	void refresh_custom_item_sets();
	void refresh_custom_items();
	void apply_changes();

	Unit* unit = nullptr;

	QLabel* title = nullptr;
	QLabel* meta = nullptr;
	QLabel* icon = nullptr;

	QTabWidget* tabs = nullptr;
	QComboBox* player = nullptr;
	QDoubleSpinBox* facing = nullptr;
	QSpinBox* hit_points_percent = nullptr;
	QLabel* max_hit_points = nullptr;
	QSpinBox* mana_points = nullptr;
	QLabel* max_mana = nullptr;
	QSpinBox* level = nullptr;
	QCheckBox* default_attributes = nullptr;
	QSpinBox* strength = nullptr;
	QSpinBox* agility = nullptr;
	QSpinBox* intelligence = nullptr;

	QComboBox* item_drop_mode = nullptr;
	QComboBox* map_item_table = nullptr;
	QListWidget* custom_sets = nullptr;
	QListWidget* custom_items = nullptr;
	QPushButton* add_set = nullptr;
	QPushButton* delete_set = nullptr;
	QPushButton* add_item = nullptr;
	QPushButton* edit_item = nullptr;
	QPushButton* delete_item = nullptr;
	QCheckBox* apply_item_drops_to_type = nullptr;

	int edited_item_table_pointer = -1;
	std::vector<ItemSet> edited_item_sets;
};
