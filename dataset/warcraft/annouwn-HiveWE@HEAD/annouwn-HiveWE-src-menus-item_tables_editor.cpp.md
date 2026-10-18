---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/item_tables_editor.cpp"
ref: HEAD
---

#include "item_tables_editor.h"

#include "item_drop_editor.h"

#include <QCheckBox>
#include <QComboBox>
#include <QDialogButtonBox>
#include <QGridLayout>
#include <QGroupBox>
#include <QHBoxLayout>
#include <QLabel>
#include <QLineEdit>
#include <QListWidget>
#include <QMessageBox>
#include <QPushButton>
#include <QVBoxLayout>

import std;
import Globals;
import MapGlobal;
import TableModel;
import UnitsUndo;
import Utilities;

namespace {
constexpr int no_unit_item_table_pointer = -1;
constexpr int no_doodad_item_table_pointer = -1;

bool item_sets_equal(const std::vector<ItemSet>& left, const std::vector<ItemSet>& right) {
	if (left.size() != right.size()) {
		return false;
	}

	for (size_t i = 0; i < left.size(); ++i) {
		if (left[i].items != right[i].items) {
			return false;
		}
	}

	return true;
}

int next_item_table_creation_number() {
	int next_value = 0;
	for (const auto& table : map->info.random_item_tables) {
		next_value = std::max(next_value, table.creation_number);
	}
	return next_value + 1;
}

QString next_item_table_name() {
	int suffix = static_cast<int>(map->info.random_item_tables.size()) + 1;
	while (true) {
		const QString candidate = QString("Untitled Item Table %1").arg(suffix);
		const auto found = std::ranges::find_if(map->info.random_item_tables, [&](const auto& table) {
			return QString::fromStdString(table.name) == candidate;
		});
		if (found == map->info.random_item_tables.end()) {
			return candidate;
		}
		++suffix;
	}
}

QString unit_type_name(const std::string& id) {
	if (units_slk.row_headers.contains(id) && units_slk.column_headers.contains("name")) {
		const QString name = units_table->data(id, "name").toString();
		if (!name.isEmpty()) {
			return QString("%1 (%2)").arg(name, QString::fromStdString(id));
		}
	}

	return QString::fromStdString(id);
}
}

ItemTablesEditor::ItemTablesEditor(QWidget* parent) : QDialog(parent) {
	setAttribute(Qt::WA_DeleteOnClose);
	setWindowTitle("Item Tables");
	resize(940, 640);

	auto* root = new QVBoxLayout(this);

	auto* content = new QHBoxLayout;
	root->addLayout(content, 1);

	auto* tables_group = new QGroupBox("Tables");
	auto* tables_layout = new QVBoxLayout(tables_group);
	tables = new QListWidget;
	tables_layout->addWidget(tables, 1);

	auto* table_buttons = new QHBoxLayout;
	add_table = new QPushButton("Add Table");
	delete_table = new QPushButton("Delete Table");
	table_buttons->addWidget(add_table);
	table_buttons->addWidget(delete_table);
	tables_layout->addLayout(table_buttons);
	content->addWidget(tables_group, 0);

	auto* editor_column = new QVBoxLayout;
	content->addLayout(editor_column, 1);

	auto* name_row = new QHBoxLayout;
	name_row->addWidget(new QLabel("Table Name"));
	table_name = new QLineEdit;
	name_row->addWidget(table_name, 1);
	editor_column->addLayout(name_row);

	auto* body = new QGridLayout;
	editor_column->addLayout(body, 1);

	auto* sets_group = new QGroupBox("Sets");
	auto* sets_layout = new QVBoxLayout(sets_group);
	sets = new QListWidget;
	sets_layout->addWidget(sets, 1);
	auto* set_buttons = new QHBoxLayout;
	add_set = new QPushButton("New Set");
	delete_set = new QPushButton("Delete Set");
	set_buttons->addWidget(add_set);
	set_buttons->addWidget(delete_set);
	sets_layout->addLayout(set_buttons);
	body->addWidget(sets_group, 0, 0);

	auto* items_group = new QGroupBox("Entries");
	auto* items_layout = new QVBoxLayout(items_group);
	items = new QListWidget;
	items_layout->addWidget(items, 1);
	auto* item_buttons = new QHBoxLayout;
	add_item = new QPushButton("New Item");
	edit_item = new QPushButton("Edit Item");
	delete_item = new QPushButton("Delete Item");
	item_buttons->addWidget(add_item);
	item_buttons->addWidget(edit_item);
	item_buttons->addWidget(delete_item);
	items_layout->addLayout(item_buttons);
	body->addWidget(items_group, 0, 1);

	auto* assign_group = new QGroupBox("Assign To Unit Type");
	auto* assign_layout = new QGridLayout(assign_group);
	assign_unit_type = new QComboBox;
	assign_table = new QComboBox;
	clear_custom_drops = new QCheckBox("Remove existing custom drop sets on affected units");
	clear_custom_drops->setChecked(true);
	apply_to_type = new QPushButton("Apply To Placed Units");
	assign_layout->addWidget(new QLabel("Unit Type"), 0, 0);
	assign_layout->addWidget(assign_unit_type, 0, 1);
	assign_layout->addWidget(new QLabel("Map Item Table"), 1, 0);
	assign_layout->addWidget(assign_table, 1, 1);
	assign_layout->addWidget(clear_custom_drops, 2, 0, 1, 2);
	assign_layout->addWidget(apply_to_type, 3, 1);
	editor_column->addWidget(assign_group);

	auto* buttons = new QDialogButtonBox(QDialogButtonBox::Close);
	root->addWidget(buttons);

	connect(buttons, &QDialogButtonBox::rejected, this, &QDialog::close);
	connect(tables, &QListWidget::currentRowChanged, this, [this](int) {
		refresh_sets();
		refresh_items();
		refresh_button_state();
	});
	connect(sets, &QListWidget::currentRowChanged, this, [this](int) {
		refresh_items();
		refresh_button_state();
	});
	connect(items, &QListWidget::currentRowChanged, this, [this](int) { refresh_button_state(); });
	connect(assign_unit_type, QOverload<int>::of(&QComboBox::currentIndexChanged), this, [this](int) { refresh_button_state(); });
	connect(assign_table, QOverload<int>::of(&QComboBox::currentIndexChanged), this, [this](int) { refresh_button_state(); });
	connect(table_name, &QLineEdit::textEdited, this, [this](const QString& value) {
		const int table_index = current_table_index();
		if (table_index < 0) {
			return;
		}
		map->info.random_item_tables[table_index].name = value.toStdString();
		if (auto* current = tables->currentItem()) {
			current->setText(value);
		}
		refresh_assignment_controls();
	});
	connect(add_table, &QPushButton::clicked, this, [this]() {
		auto& table = map->info.random_item_tables.emplace_back();
		table.creation_number = next_item_table_creation_number();
		table.name = next_item_table_name().toStdString();
		table.item_sets.push_back({});
		refresh_tables();
		tables->setCurrentRow(static_cast<int>(map->info.random_item_tables.size()) - 1);
		refresh_assignment_controls();
	});
	connect(delete_table, &QPushButton::clicked, this, [this]() {
		const int table_index = current_table_index();
		if (table_index < 0) {
			return;
		}

		const int creation_number = map->info.random_item_tables[table_index].creation_number;
		map->info.random_item_tables.erase(map->info.random_item_tables.begin() + table_index);

		for (auto& unit : map->units.units) {
			if (unit.item_table_pointer == creation_number) {
				unit.item_table_pointer = no_unit_item_table_pointer;
			}
		}

		for (auto& doodad : map->doodads.doodads) {
			if (doodad.item_table_pointer == creation_number) {
				doodad.item_table_pointer = no_doodad_item_table_pointer;
			}
		}

		refresh_tables();
		refresh_assignment_controls();
	});
	connect(add_set, &QPushButton::clicked, this, [this]() {
		const int table_index = current_table_index();
		if (table_index < 0) {
			return;
		}
		map->info.random_item_tables[table_index].item_sets.push_back({});
		refresh_sets();
		sets->setCurrentRow(static_cast<int>(map->info.random_item_tables[table_index].item_sets.size()) - 1);
	});
	connect(delete_set, &QPushButton::clicked, this, [this]() {
		const int table_index = current_table_index();
		const int set_index = current_set_index();
		if (table_index < 0 || set_index < 0) {
			return;
		}
		auto& item_sets = map->info.random_item_tables[table_index].item_sets;
		item_sets.erase(item_sets.begin() + set_index);
		refresh_sets();
		refresh_items();
	});
	connect(add_item, &QPushButton::clicked, this, [this]() {
		const int table_index = current_table_index();
		const int set_index = current_set_index();
		if (table_index < 0 || set_index < 0) {
			return;
		}
		std::pair<int, std::string> entry { 100, "" };
		if (!edit_item_drop_entry(entry, this)) {
			return;
		}
		map->info.random_item_tables[table_index].item_sets[set_index].items.push_back(entry);
		refresh_items();
		items->setCurrentRow(static_cast<int>(map->info.random_item_tables[table_index].item_sets[set_index].items.size()) - 1);
		refresh_sets();
	});
	connect(edit_item, &QPushButton::clicked, this, [this]() {
		const int table_index = current_table_index();
		const int set_index = current_set_index();
		const int item_index = items->currentRow();
		if (table_index < 0 || set_index < 0 || item_index < 0) {
			return;
		}
		auto& entry = map->info.random_item_tables[table_index].item_sets[set_index].items[item_index];
		if (!edit_item_drop_entry(entry, this)) {
			return;
		}
		refresh_items();
		refresh_sets();
	});
	connect(delete_item, &QPushButton::clicked, this, [this]() {
		const int table_index = current_table_index();
		const int set_index = current_set_index();
		const int item_index = items->currentRow();
		if (table_index < 0 || set_index < 0 || item_index < 0) {
			return;
		}
		auto& item_entries = map->info.random_item_tables[table_index].item_sets[set_index].items;
		item_entries.erase(item_entries.begin() + item_index);
		refresh_items();
		refresh_sets();
	});
	connect(apply_to_type, &QPushButton::clicked, this, [this]() {
		if (assign_unit_type->currentIndex() < 0 || assign_table->currentIndex() < 0) {
			return;
		}

		const std::string unit_id = assign_unit_type->currentData().toString().toStdString();
		const int creation_number = assign_table->currentData().toInt();
		const bool clear_custom = clear_custom_drops->isChecked();

		std::vector<Unit> old_units;
		std::vector<Unit> new_units;

		for (auto& unit : map->units.units) {
			if (unit.id != unit_id) {
				continue;
			}

			Unit old_state = unit;
			unit.item_table_pointer = creation_number < 0 ? no_unit_item_table_pointer : creation_number;
			if (clear_custom) {
				unit.item_sets.clear();
			}

			if (unit.item_table_pointer != old_state.item_table_pointer || !item_sets_equal(unit.item_sets, old_state.item_sets)) {
				old_units.push_back(old_state);
				new_units.push_back(unit);
			}
		}

		if (old_units.empty()) {
			return;
		}

		map->world_undo.new_undo_group();
		auto action = std::make_unique<UnitStateAction>();
		action->old_units = std::move(old_units);
		action->new_units = std::move(new_units);
		map->world_undo.add_undo_action(std::move(action));

		QMessageBox::information(this, "Item Tables", "Applied item table assignment to all placed units of that type.");
	});

	refresh_tables();
	refresh_unit_type_list();
	refresh_assignment_controls();
	refresh_button_state();
	show();
}

int ItemTablesEditor::current_table_index() const {
	return tables->currentRow();
}

int ItemTablesEditor::current_set_index() const {
	return sets->currentRow();
}

void ItemTablesEditor::refresh_tables() {
	const int current_row = tables->currentRow();
	tables->clear();
	for (const auto& table : map->info.random_item_tables) {
		auto* item = new QListWidgetItem(QString::fromStdString(table.name), tables);
		item->setData(Qt::UserRole, table.creation_number);
	}

	if (!map->info.random_item_tables.empty()) {
		tables->setCurrentRow(std::clamp(current_row, 0, static_cast<int>(map->info.random_item_tables.size()) - 1));
	} else {
		table_name->clear();
	}
}

void ItemTablesEditor::refresh_sets() {
	const int table_index = current_table_index();
	const int current_row = sets->currentRow();

	sets->clear();
	if (table_index < 0) {
		table_name->clear();
		return;
	}

	const auto& table = map->info.random_item_tables[table_index];
	table_name->setText(QString::fromStdString(table.name));

	for (size_t i = 0; i < table.item_sets.size(); ++i) {
		int total_chance = 0;
		for (const auto& [chance, id] : table.item_sets[i].items) {
			total_chance += chance;
		}
		sets->addItem(QString("Set %1 (Total: %2%)").arg(i + 1).arg(total_chance));
	}

	if (!table.item_sets.empty()) {
		sets->setCurrentRow(std::clamp(current_row, 0, static_cast<int>(table.item_sets.size()) - 1));
	}
}

void ItemTablesEditor::refresh_items() {
	const int table_index = current_table_index();
	const int set_index = current_set_index();
	const int current_row = items->currentRow();

	items->clear();
	if (table_index < 0 || set_index < 0) {
		return;
	}

	const auto& set = map->info.random_item_tables[table_index].item_sets[set_index];
	for (const auto& entry : set.items) {
		auto* item = new QListWidgetItem(icon_for_item_drop_entry(entry), describe_item_drop_entry(entry), items);
		item->setToolTip(describe_item_drop_entry(entry));
	}

	if (!set.items.empty()) {
		items->setCurrentRow(std::clamp(current_row, 0, static_cast<int>(set.items.size()) - 1));
	}
}

void ItemTablesEditor::refresh_assignment_controls() {
	const int selected_creation_number = assign_table ? assign_table->currentData().toInt() : -1;

	assign_table->clear();
	assign_table->addItem("None", -1);
	for (const auto& table : map->info.random_item_tables) {
		assign_table->addItem(QString::fromStdString(table.name), table.creation_number);
	}

	if (const int index = assign_table->findData(selected_creation_number); index != -1) {
		assign_table->setCurrentIndex(index);
	} else if (assign_table->count() > 0) {
		assign_table->setCurrentIndex(0);
	}

	refresh_button_state();
}

void ItemTablesEditor::refresh_button_state() {
	const bool has_table = current_table_index() >= 0;
	const bool has_set = current_set_index() >= 0;
	const bool has_item = items->currentRow() >= 0;

	table_name->setEnabled(has_table);
	delete_table->setEnabled(has_table);
	add_set->setEnabled(has_table);
	delete_set->setEnabled(has_set);
	add_item->setEnabled(has_set);
	edit_item->setEnabled(has_item);
	delete_item->setEnabled(has_item);
	apply_to_type->setEnabled(assign_unit_type->count() > 0 && assign_table->count() > 0);
}

void ItemTablesEditor::refresh_unit_type_list() {
	assign_unit_type->clear();

	std::unordered_set<std::string> seen;
	std::vector<std::string> ids;
	for (const auto& unit : map->units.units) {
		if (seen.insert(unit.id).second) {
			ids.push_back(unit.id);
		}
	}

	std::ranges::sort(ids, [](const std::string& a, const std::string& b) {
		return unit_type_name(a).localeAwareCompare(unit_type_name(b)) < 0;
	});

	for (const auto& id : ids) {
		assign_unit_type->addItem(unit_type_name(id), QString::fromStdString(id));
	}
}
