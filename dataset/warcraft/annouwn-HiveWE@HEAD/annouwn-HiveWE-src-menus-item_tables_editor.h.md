---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/item_tables_editor.h"
ref: HEAD
---

#pragma once

#include <QDialog>

class QCheckBox;
class QComboBox;
class QLineEdit;
class QListWidget;
class QPushButton;

class ItemTablesEditor : public QDialog {
	Q_OBJECT

  public:
	explicit ItemTablesEditor(QWidget* parent = nullptr);

  private:
	void refresh_tables();
	void refresh_sets();
	void refresh_items();
	void refresh_assignment_controls();
	void refresh_button_state();
	void refresh_unit_type_list();

	int current_table_index() const;
	int current_set_index() const;

	QListWidget* tables = nullptr;
	QLineEdit* table_name = nullptr;
	QListWidget* sets = nullptr;
	QListWidget* items = nullptr;

	QPushButton* add_table = nullptr;
	QPushButton* delete_table = nullptr;
	QPushButton* add_set = nullptr;
	QPushButton* delete_set = nullptr;
	QPushButton* add_item = nullptr;
	QPushButton* edit_item = nullptr;
	QPushButton* delete_item = nullptr;

	QComboBox* assign_unit_type = nullptr;
	QComboBox* assign_table = nullptr;
	QPushButton* apply_to_type = nullptr;
	QCheckBox* clear_custom_drops = nullptr;
};
