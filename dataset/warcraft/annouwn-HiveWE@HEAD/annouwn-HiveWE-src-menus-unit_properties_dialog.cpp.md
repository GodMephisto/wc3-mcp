---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/unit_properties_dialog.cpp"
ref: HEAD
---

#include "unit_properties_dialog.h"

#include <QCheckBox>
#include <QComboBox>
#include <QDialogButtonBox>
#include <QDoubleSpinBox>
#include <QFormLayout>
#include <QFrame>
#include <QGridLayout>
#include <QHBoxLayout>
#include <QIcon>
#include <QLabel>
#include <QListWidget>
#include <QPushButton>
#include <QSpinBox>
#include <QTabWidget>
#include <QVBoxLayout>

#include "item_drop_editor.h"
#include "item_tables_editor.h"

import std;
import Globals;
import MapGlobal;
import SLK;
import TableModel;
import UnitsUndo;
import Utilities;
import WindowHandler;
import <glm/glm.hpp>;

namespace {
constexpr int no_unit_item_table_pointer = -1;

std::string_view slk_string(const slk::SLK& slk, const std::string_view id, const std::initializer_list<std::string_view> fields) {
	for (const auto field : fields) {
		if (slk.column_headers.contains(field)) {
			return slk.data<std::string_view>(field, id);
		}
	}
	return {};
}

int slk_int(const slk::SLK& slk, const std::string_view id, const std::initializer_list<std::string_view> fields, const int fallback = 0) {
	for (const auto field : fields) {
		if (!slk.column_headers.contains(field)) {
			continue;
		}

		const std::string_view value = slk.data<std::string_view>(field, id);
		if (value.empty()) {
			continue;
		}

		int parsed = fallback;
		const auto [ptr, ec] = std::from_chars(value.data(), value.data() + value.size(), parsed);
		if (ec == std::errc()) {
			return parsed;
		}
	}

	return fallback;
}

QString unit_display_name(const Unit& unit) {
	if (units_slk.column_headers.contains("propernames")) {
		const auto proper_names = units_slk.data<std::string_view>("propernames", unit.id);
		if (!proper_names.empty()) {
			return QString::fromUtf8(proper_names).split(',').first();
		}
	}

	if (units_slk.column_headers.contains("name")) {
		return units_table->data(unit.id, "name").toString();
	}

	return QString::fromStdString(unit.id);
}

bool is_hero_unit(const Unit& unit) {
	return slk_int(units_slk, unit.id, {"STR", "str"}) > 0 || slk_int(units_slk, unit.id, {"AGI", "agi"}) > 0
		|| slk_int(units_slk, unit.id, {"INT", "int"}) > 0 || !slk_string(units_slk, unit.id, {"propernames"}).empty();
}

int base_hit_points(const Unit& unit) {
	return slk_int(units_slk, unit.id, {"HP", "hp"}, 1);
}

int base_mana(const Unit& unit) {
	return slk_int(units_slk, unit.id, {"manaN", "mana0", "Mana", "mana"}, 0);
}

int base_strength(const Unit& unit) {
	return slk_int(units_slk, unit.id, {"STR", "str"}, 0);
}

int base_agility(const Unit& unit) {
	return slk_int(units_slk, unit.id, {"AGI", "agi"}, 0);
}

int base_intelligence(const Unit& unit) {
	return slk_int(units_slk, unit.id, {"INT", "int"}, 0);
}

QIcon unit_icon(const Unit& unit) {
	if (units_slk.column_headers.contains("art")) {
		const auto index = units_table->index(units_slk.row_headers.at(unit.id), units_slk.column_headers.at("art"));
		const QVariant decoration = units_table->data(index, Qt::DecorationRole);
		if (decoration.canConvert<QIcon>()) {
			return qvariant_cast<QIcon>(decoration);
		}
	}

	return {};
}

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
}

UnitPropertiesDialog::UnitPropertiesDialog(Unit* unit, QWidget* parent) : QDialog(parent), unit(unit) {
	setModal(true);
	setWindowTitle("Unit Properties");
	resize(680, 520);

	setStyleSheet(
		"QDialog {"
		"background: rgb(18, 24, 32);"
		"color: rgb(232, 237, 242);"
		"}"
		"#unitPropertiesHero {"
		"background: rgb(24, 31, 42);"
		"border: 1px solid rgba(255,255,255,18);"
		"border-radius: 14px;"
		"}"
		"#unitPropertiesTitle {"
		"font-size: 22px;"
		"font-weight: 700;"
		"color: rgb(245, 247, 250);"
		"}"
		"#unitPropertiesMeta {"
		"font-size: 12px;"
		"color: rgb(161, 172, 185);"
		"}"
		"QLabel#unitPropertiesIcon {"
		"background: rgb(12, 17, 24);"
		"border: 1px solid rgba(255,255,255,24);"
		"border-radius: 10px;"
		"}"
		"QTabWidget::pane {"
		"border: 1px solid rgba(255,255,255,18);"
		"background: rgb(23, 30, 40);"
		"border-radius: 12px;"
		"top: -1px;"
		"}"
		"QTabBar::tab {"
		"background: rgb(28, 36, 47);"
		"color: rgb(201, 210, 220);"
		"padding: 10px 16px;"
		"margin-right: 6px;"
		"border-top-left-radius: 10px;"
		"border-top-right-radius: 10px;"
		"}"
		"QTabBar::tab:selected {"
		"background: rgb(53, 97, 148);"
		"color: rgb(247, 249, 251);"
		"}"
		"QFrame#unitPropertiesPanel {"
		"background: rgb(26, 34, 45);"
		"border: 1px solid rgba(255,255,255,14);"
		"border-radius: 12px;"
		"}"
		"QLabel#unitPropertiesSection {"
		"font-size: 11px;"
		"font-weight: 700;"
		"letter-spacing: 0.08em;"
		"color: rgb(149, 195, 244);"
		"}"
		"QComboBox, QSpinBox, QDoubleSpinBox {"
		"min-height: 34px;"
		"background: rgb(12, 18, 25);"
		"border: 1px solid rgba(255,255,255,20);"
		"border-radius: 9px;"
		"padding: 2px 10px;"
		"color: rgb(239, 243, 247);"
		"}"
		"QComboBox:focus, QSpinBox:focus, QDoubleSpinBox:focus {"
		"border: 1px solid rgb(92, 157, 226);"
		"}"
		"QCheckBox {"
		"color: rgb(229, 233, 238);"
		"spacing: 8px;"
		"}"
		"QDialogButtonBox QPushButton {"
		"min-width: 110px;"
		"min-height: 38px;"
		"border-radius: 10px;"
		"padding: 0 16px;"
		"background: rgb(29, 38, 49);"
		"border: 1px solid rgba(255,255,255,18);"
		"color: rgb(238, 242, 246);"
		"font-weight: 600;"
		"}"
		"QDialogButtonBox QPushButton:default {"
		"background: rgb(70, 126, 192);"
		"border-color: rgb(112, 172, 242);"
		"}"
	);

	auto* root = new QVBoxLayout(this);
	root->setContentsMargins(20, 20, 20, 20);
	root->setSpacing(16);

	QFrame* hero = new QFrame;
	hero->setObjectName("unitPropertiesHero");
	auto* hero_layout = new QHBoxLayout(hero);
	hero_layout->setContentsMargins(16, 16, 16, 16);
	hero_layout->setSpacing(14);

	icon = new QLabel;
	icon->setObjectName("unitPropertiesIcon");
	icon->setFixedSize(64, 64);
	icon->setAlignment(Qt::AlignCenter);
	icon->setPixmap(unit_icon(*unit).pixmap(48, 48));
	hero_layout->addWidget(icon, 0, Qt::AlignTop);

	auto* hero_text = new QVBoxLayout;
	hero_text->setSpacing(4);
	title = new QLabel(unit_display_name(*unit));
	title->setObjectName("unitPropertiesTitle");
	meta = new QLabel(QString("%1  |  %2").arg(QString::fromStdString(unit->id)).arg(is_hero_unit(*unit) ? "Hero" : "Unit"));
	meta->setObjectName("unitPropertiesMeta");
	hero_text->addWidget(title);
	hero_text->addWidget(meta);
	hero_text->addStretch();
	hero_layout->addLayout(hero_text, 1);

	root->addWidget(hero);

	tabs = new QTabWidget;
	auto* general_tab = new QWidget;
	auto* general_layout = new QVBoxLayout(general_tab);
	general_layout->setContentsMargins(16, 16, 16, 16);
	general_layout->setSpacing(14);

	QFrame* identity_panel = new QFrame;
	identity_panel->setObjectName("unitPropertiesPanel");
	auto* identity_layout = new QFormLayout(identity_panel);
	identity_layout->setContentsMargins(14, 14, 14, 14);
	identity_layout->setHorizontalSpacing(14);
	identity_layout->setVerticalSpacing(12);

	player = new QComboBox;
	for (const auto& player_info : map->info.players) {
		std::string color_lookup = std::to_string(player_info.internal_number);
		if (color_lookup.size() == 1) {
			color_lookup = "0" + color_lookup;
		}
		const auto player_name = std::format(
			"{} ({})",
			map->trigger_strings.string(player_info.name),
			world_edit_strings.data("WorldEditStrings", "WESTRING_UNITCOLOR_" + color_lookup)
		);
		player->addItem(QString::fromStdString(player_name), player_info.internal_number);
	}
	player->addItem("Neutral Hostile", 24);
	player->addItem("Neutral Passive", 27);
	if (const int index = player->findData(unit->player); index != -1) {
		player->setCurrentIndex(index);
	}

	facing = new QDoubleSpinBox;
	facing->setRange(0.0, 359.99);
	facing->setDecimals(2);
	facing->setSingleStep(15.0);
	facing->setValue(glm::degrees(unit->angle));

	identity_layout->addRow("Player", player);
	identity_layout->addRow("Facing (deg)", facing);
	general_layout->addWidget(identity_panel);

	auto* stats_row = new QHBoxLayout;
	stats_row->setSpacing(14);

	QFrame* state_panel = new QFrame;
	state_panel->setObjectName("unitPropertiesPanel");
	auto* state_layout = new QGridLayout(state_panel);
	state_layout->setContentsMargins(14, 14, 14, 14);
	state_layout->setHorizontalSpacing(10);
	state_layout->setVerticalSpacing(12);

	auto* state_label = new QLabel("STATE");
	state_label->setObjectName("unitPropertiesSection");
	state_layout->addWidget(state_label, 0, 0, 1, 3);

	const int hp_maximum = std::max(1, base_hit_points(*unit));
	hit_points_percent = new QSpinBox;
	hit_points_percent->setRange(1, 100);
	hit_points_percent->setValue(unit->health == -1 ? 100 : unit->health);
	max_hit_points = new QLabel(QString("of %1").arg(hp_maximum));
	max_hit_points->setObjectName("unitPropertiesMeta");

	const int mana_maximum = std::max(0, base_mana(*unit));
	mana_points = new QSpinBox;
	mana_points->setRange(0, std::max(99999, mana_maximum));
	mana_points->setValue(unit->mana == -1 ? mana_maximum : unit->mana);
	max_mana = new QLabel(QString("of %1").arg(mana_maximum));
	max_mana->setObjectName("unitPropertiesMeta");

	level = new QSpinBox;
	level->setRange(1, 99);
	level->setValue(std::max(1, unit->level));

	state_layout->addWidget(new QLabel("Hit Points %"), 1, 0);
	state_layout->addWidget(hit_points_percent, 1, 1);
	state_layout->addWidget(max_hit_points, 1, 2);
	state_layout->addWidget(new QLabel("Mana Points"), 2, 0);
	state_layout->addWidget(mana_points, 2, 1);
	state_layout->addWidget(max_mana, 2, 2);
	state_layout->addWidget(new QLabel("Level"), 3, 0);
	state_layout->addWidget(level, 3, 1);
	stats_row->addWidget(state_panel, 1);

	QFrame* attributes_panel = new QFrame;
	attributes_panel->setObjectName("unitPropertiesPanel");
	auto* attributes_layout = new QGridLayout(attributes_panel);
	attributes_layout->setContentsMargins(14, 14, 14, 14);
	attributes_layout->setHorizontalSpacing(10);
	attributes_layout->setVerticalSpacing(12);

	auto* attributes_label = new QLabel("ATTRIBUTES");
	attributes_label->setObjectName("unitPropertiesSection");
	attributes_layout->addWidget(attributes_label, 0, 0, 1, 2);

	default_attributes = new QCheckBox("Use Default Attributes");
	const bool using_defaults = unit->strength == 0 && unit->agility == 0 && unit->intelligence == 0;
	default_attributes->setChecked(using_defaults);
	attributes_layout->addWidget(default_attributes, 1, 0, 1, 2);

	strength = new QSpinBox;
	strength->setRange(0, 999);
	strength->setValue(using_defaults ? base_strength(*unit) : unit->strength);

	agility = new QSpinBox;
	agility->setRange(0, 999);
	agility->setValue(using_defaults ? base_agility(*unit) : unit->agility);

	intelligence = new QSpinBox;
	intelligence->setRange(0, 999);
	intelligence->setValue(using_defaults ? base_intelligence(*unit) : unit->intelligence);

	attributes_layout->addWidget(new QLabel("Strength"), 2, 0);
	attributes_layout->addWidget(strength, 2, 1);
	attributes_layout->addWidget(new QLabel("Agility"), 3, 0);
	attributes_layout->addWidget(agility, 3, 1);
	attributes_layout->addWidget(new QLabel("Intelligence"), 4, 0);
	attributes_layout->addWidget(intelligence, 4, 1);
	stats_row->addWidget(attributes_panel, 1);

	general_layout->addLayout(stats_row);
	general_layout->addStretch();

	tabs->addTab(general_tab, "General");

	edited_item_table_pointer = unit->item_table_pointer;
	edited_item_sets = unit->item_sets;

	auto* drops_tab = new QWidget;
	auto* drops_layout = new QVBoxLayout(drops_tab);
	drops_layout->setContentsMargins(16, 16, 16, 16);
	drops_layout->setSpacing(14);

	QFrame* mode_panel = new QFrame;
	mode_panel->setObjectName("unitPropertiesPanel");
	auto* mode_layout = new QGridLayout(mode_panel);
	mode_layout->setContentsMargins(14, 14, 14, 14);
	mode_layout->setHorizontalSpacing(12);
	mode_layout->setVerticalSpacing(12);

	auto* drops_label = new QLabel("ITEMS DROPPED");
	drops_label->setObjectName("unitPropertiesSection");
	mode_layout->addWidget(drops_label, 0, 0, 1, 2);

	item_drop_mode = new QComboBox;
	item_drop_mode->addItem("None");
	item_drop_mode->addItem("Use Item Table From Map");
	item_drop_mode->addItem("Use Custom Item Sets");
	mode_layout->addWidget(new QLabel("Drop Mode"), 1, 0);
	mode_layout->addWidget(item_drop_mode, 1, 1);

	map_item_table = new QComboBox;
	mode_layout->addWidget(new QLabel("Map Item Table"), 2, 0);
	mode_layout->addWidget(map_item_table, 2, 1);

	auto* edit_map_tables = new QPushButton("Edit Map Item Tables");
	mode_layout->addWidget(edit_map_tables, 3, 1, 1, 1, Qt::AlignLeft);

	apply_item_drops_to_type = new QCheckBox("Apply item drop settings to all placed units of this type");
	mode_layout->addWidget(apply_item_drops_to_type, 4, 0, 1, 2);
	drops_layout->addWidget(mode_panel);

	auto* custom_layout = new QHBoxLayout;
	custom_layout->setSpacing(14);

	QFrame* sets_panel = new QFrame;
	sets_panel->setObjectName("unitPropertiesPanel");
	auto* sets_layout = new QVBoxLayout(sets_panel);
	sets_layout->setContentsMargins(14, 14, 14, 14);
	sets_layout->setSpacing(10);
	sets_layout->addWidget(new QLabel("Sets"));
	custom_sets = new QListWidget;
	sets_layout->addWidget(custom_sets, 1);
	auto* set_buttons = new QHBoxLayout;
	add_set = new QPushButton("New Set");
	delete_set = new QPushButton("Delete Set");
	set_buttons->addWidget(add_set);
	set_buttons->addWidget(delete_set);
	sets_layout->addLayout(set_buttons);
	custom_layout->addWidget(sets_panel, 1);

	QFrame* items_panel = new QFrame;
	items_panel->setObjectName("unitPropertiesPanel");
	auto* items_layout = new QVBoxLayout(items_panel);
	items_layout->setContentsMargins(14, 14, 14, 14);
	items_layout->setSpacing(10);
	items_layout->addWidget(new QLabel("Items"));
	custom_items = new QListWidget;
	items_layout->addWidget(custom_items, 1);
	auto* item_buttons = new QHBoxLayout;
	add_item = new QPushButton("New Item");
	edit_item = new QPushButton("Edit Item");
	delete_item = new QPushButton("Delete Item");
	item_buttons->addWidget(add_item);
	item_buttons->addWidget(edit_item);
	item_buttons->addWidget(delete_item);
	items_layout->addLayout(item_buttons);
	custom_layout->addWidget(items_panel, 1);

	drops_layout->addLayout(custom_layout, 1);
	tabs->addTab(drops_tab, "Items Dropped");
	root->addWidget(tabs, 1);

	auto* buttons = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
	buttons->button(QDialogButtonBox::Ok)->setText("Apply");
	buttons->button(QDialogButtonBox::Ok)->setDefault(true);
	root->addWidget(buttons);

	connect(default_attributes, &QCheckBox::toggled, this, [this](bool) { refresh_attribute_state(); });
	connect(item_drop_mode, QOverload<int>::of(&QComboBox::currentIndexChanged), this, [this](int) { refresh_item_drop_mode_state(); });
	connect(custom_sets, &QListWidget::currentRowChanged, this, [this](int) {
		refresh_custom_items();
		refresh_item_drop_mode_state();
	});
	connect(custom_items, &QListWidget::currentRowChanged, this, [this](int) { refresh_item_drop_mode_state(); });
	connect(edit_map_tables, &QPushButton::clicked, this, [this]() {
		bool created = false;
		auto* editor = window_handler.create_or_raise<ItemTablesEditor>(this, created);
		connect(editor, &QObject::destroyed, this, [this]() { refresh_map_item_table_options(); }, Qt::UniqueConnection);
	});
	connect(add_set, &QPushButton::clicked, this, [this]() {
		edited_item_sets.push_back({});
		refresh_custom_item_sets();
		custom_sets->setCurrentRow(static_cast<int>(edited_item_sets.size()) - 1);
		refresh_item_drop_mode_state();
	});
	connect(delete_set, &QPushButton::clicked, this, [this]() {
		const int set_index = custom_sets->currentRow();
		if (set_index < 0) {
			return;
		}
		edited_item_sets.erase(edited_item_sets.begin() + set_index);
		refresh_custom_item_sets();
		refresh_custom_items();
		refresh_item_drop_mode_state();
	});
	connect(add_item, &QPushButton::clicked, this, [this]() {
		const int set_index = custom_sets->currentRow();
		if (set_index < 0) {
			return;
		}
		std::pair<int, std::string> entry { 100, "" };
		if (!edit_item_drop_entry(entry, this)) {
			return;
		}
		edited_item_sets[set_index].items.push_back(entry);
		refresh_custom_items();
		custom_items->setCurrentRow(static_cast<int>(edited_item_sets[set_index].items.size()) - 1);
		refresh_custom_item_sets();
		refresh_item_drop_mode_state();
	});
	connect(edit_item, &QPushButton::clicked, this, [this]() {
		const int set_index = custom_sets->currentRow();
		const int item_index = custom_items->currentRow();
		if (set_index < 0 || item_index < 0) {
			return;
		}
		auto& entry = edited_item_sets[set_index].items[item_index];
		if (!edit_item_drop_entry(entry, this)) {
			return;
		}
		refresh_custom_items();
		refresh_custom_item_sets();
	});
	connect(delete_item, &QPushButton::clicked, this, [this]() {
		const int set_index = custom_sets->currentRow();
		const int item_index = custom_items->currentRow();
		if (set_index < 0 || item_index < 0) {
			return;
		}
		auto& entries = edited_item_sets[set_index].items;
		entries.erase(entries.begin() + item_index);
		refresh_custom_items();
		refresh_custom_item_sets();
		refresh_item_drop_mode_state();
	});
	connect(buttons, &QDialogButtonBox::accepted, this, &UnitPropertiesDialog::apply_changes);
	connect(buttons, &QDialogButtonBox::rejected, this, &QDialog::reject);

	if (!is_hero_unit(*unit)) {
		default_attributes->setChecked(true);
		default_attributes->setEnabled(false);
	}

	if (!edited_item_sets.empty()) {
		item_drop_mode->setCurrentIndex(2);
	} else if (edited_item_table_pointer != no_unit_item_table_pointer) {
		item_drop_mode->setCurrentIndex(1);
	} else {
		item_drop_mode->setCurrentIndex(0);
	}

	refresh_map_item_table_options();
	refresh_custom_item_sets();
	refresh_custom_items();
	refresh_attribute_state();
	refresh_item_drop_mode_state();
}

void UnitPropertiesDialog::refresh_attribute_state() const {
	const bool enabled = default_attributes->isEnabled() && !default_attributes->isChecked();
	strength->setEnabled(enabled);
	agility->setEnabled(enabled);
	intelligence->setEnabled(enabled);
}

void UnitPropertiesDialog::refresh_item_drop_mode_state() {
	const bool using_map_table = item_drop_mode->currentIndex() == 1;
	const bool using_custom_sets = item_drop_mode->currentIndex() == 2;
	const bool has_set = custom_sets->currentRow() >= 0;
	const bool has_item = custom_items->currentRow() >= 0;

	map_item_table->setEnabled(using_map_table);
	custom_sets->setEnabled(using_custom_sets);
	custom_items->setEnabled(using_custom_sets && has_set);
	add_set->setEnabled(using_custom_sets);
	delete_set->setEnabled(using_custom_sets && has_set);
	add_item->setEnabled(using_custom_sets && has_set);
	edit_item->setEnabled(using_custom_sets && has_item);
	delete_item->setEnabled(using_custom_sets && has_item);
}

void UnitPropertiesDialog::refresh_map_item_table_options() {
	const int selected_pointer = map_item_table->currentData().isValid() ? map_item_table->currentData().toInt() : edited_item_table_pointer;

	map_item_table->clear();
	map_item_table->addItem("No Item Table", no_unit_item_table_pointer);
	for (const auto& table : map->info.random_item_tables) {
		map_item_table->addItem(QString::fromStdString(table.name), table.creation_number);
	}

	if (const int index = map_item_table->findData(selected_pointer); index != -1) {
		map_item_table->setCurrentIndex(index);
	} else {
		map_item_table->setCurrentIndex(0);
	}
}

void UnitPropertiesDialog::refresh_custom_item_sets() {
	const int current_row = custom_sets->currentRow();
	custom_sets->clear();

	for (size_t i = 0; i < edited_item_sets.size(); ++i) {
		int total_chance = 0;
		for (const auto& [chance, id] : edited_item_sets[i].items) {
			total_chance += chance;
		}
		custom_sets->addItem(QString("Set %1 (Total: %2%)").arg(i + 1).arg(total_chance));
	}

	if (!edited_item_sets.empty()) {
		custom_sets->setCurrentRow(std::clamp(current_row, 0, static_cast<int>(edited_item_sets.size()) - 1));
	}
}

void UnitPropertiesDialog::refresh_custom_items() {
	const int set_index = custom_sets->currentRow();
	const int current_row = custom_items->currentRow();
	custom_items->clear();

	if (set_index < 0 || set_index >= static_cast<int>(edited_item_sets.size())) {
		return;
	}

	for (const auto& entry : edited_item_sets[set_index].items) {
		auto* item = new QListWidgetItem(icon_for_item_drop_entry(entry), describe_item_drop_entry(entry), custom_items);
		item->setToolTip(describe_item_drop_entry(entry));
	}

	if (!edited_item_sets[set_index].items.empty()) {
		custom_items->setCurrentRow(std::clamp(current_row, 0, static_cast<int>(edited_item_sets[set_index].items.size()) - 1));
	}
}

void UnitPropertiesDialog::apply_changes() {
	const Unit old_state = *unit;

	unit->player = player->currentData().toInt();
	unit->angle = glm::radians(static_cast<float>(facing->value()));
	unit->health = hit_points_percent->value();
	unit->mana = mana_points->value();
	unit->level = level->value();

	if (default_attributes->isChecked()) {
		unit->strength = 0;
		unit->agility = 0;
		unit->intelligence = 0;
	} else {
		unit->strength = strength->value();
		unit->agility = agility->value();
		unit->intelligence = intelligence->value();
	}

	if (item_drop_mode->currentIndex() == 0) {
		unit->item_table_pointer = no_unit_item_table_pointer;
		unit->item_sets.clear();
	} else if (item_drop_mode->currentIndex() == 1) {
		unit->item_table_pointer = map_item_table->currentData().toInt();
		unit->item_sets.clear();
	} else {
		unit->item_table_pointer = no_unit_item_table_pointer;
		unit->item_sets = edited_item_sets;
	}

	std::vector<Unit> old_units;
	std::vector<Unit> new_units;

	const bool current_unit_changed = unit->player != old_state.player || std::abs(unit->angle - old_state.angle) > 0.0001f || unit->health != old_state.health
		|| unit->mana != old_state.mana || unit->level != old_state.level || unit->strength != old_state.strength
		|| unit->agility != old_state.agility || unit->intelligence != old_state.intelligence || unit->item_table_pointer != old_state.item_table_pointer
		|| !item_sets_equal(unit->item_sets, old_state.item_sets);

	if (current_unit_changed) {
		old_units.push_back(old_state);
		unit->update();
		new_units.push_back(*unit);
	}

	if (apply_item_drops_to_type->isChecked()) {
		for (auto& other : map->units.units) {
			if (&other == unit || other.id != unit->id) {
				continue;
			}

			Unit old_other = other;
			other.item_table_pointer = unit->item_table_pointer;
			other.item_sets = unit->item_sets;

			if (other.item_table_pointer != old_other.item_table_pointer || !item_sets_equal(other.item_sets, old_other.item_sets)) {
				old_units.push_back(old_other);
				new_units.push_back(other);
			}
		}
	}

	if (old_units.empty()) {
		accept();
		return;
	}

	map->world_undo.new_undo_group();
	auto action = std::make_unique<UnitStateAction>();
	action->old_units = std::move(old_units);
	action->new_units = std::move(new_units);
	map->world_undo.add_undo_action(std::move(action));

	accept();
}
