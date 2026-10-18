---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/unit_palette.cpp"
ref: HEAD
---

#include "unit_palette.h"

#include <QComboBox>
#include <QLineEdit>
#include <QListView>
#include <QAbstractProxyModel>
#include <QSortFilterProxyModel>

//#include "globals.h"
//#include <map_global.h>
#include <object_editor.h>

import std;
import TableModel;
import WindowHandler;
import MapGlobal;
import Globals;

UnitPalette::UnitPalette(QWidget* parent) : Palette(parent) {
	ui.setupUi(this);
	monitor_activation();
	setObjectName("unitPalette");
	ui.verticalLayout->setSpacing(10);
	ui.label->setText("Owner");
	ui.label->setObjectName("unitOwnerLabel");
	ui.player->setObjectName("unitPlayerFilter");

	for (const auto& player : map->info.players) {
		std::string color_lookup = std::to_string(player.internal_number);
		if (color_lookup.size() == 1) {
			color_lookup = "0" + color_lookup;
		}

		const auto player_name = std::format("{} ({})", map->trigger_strings.string(player.name), world_edit_strings.data("WorldEditStrings", "WESTRING_UNITCOLOR_" + color_lookup));

		ui.player->addItem(QString::fromStdString(player_name), player.internal_number);
	}
	ui.player->addItem("Neutral Hostile", 24);
	ui.player->addItem("Neutral Passive", 27);

	QRibbonSection* selection_section = new QRibbonSection;
	selection_section->setText("Selection");

	selection_mode->setText("Selection\nMode");
	selection_mode->setIcon(QIcon("data/icons/Ribbon/select.png"));
	selection_mode->setCheckable(true);
	selection_section->addWidget(selection_mode);

	selector = new UnitSelector(this);
	selector->setObjectName("selector");
	ui.verticalLayout->addWidget(selector);

	selection_summary = new QFrame(this);
	selection_summary->setObjectName("unitSelectionSummary");
	selection_summary->setStyleSheet(
		"QFrame#unitSelectionSummary { background: rgba(24, 27, 31, 200); border: 1px solid rgba(255, 255, 255, 14); border-radius: 12px; }"
		"QLabel#unitSelectionSummaryLabel { color: rgb(140, 149, 162); font-size: 10px; font-weight: 600; letter-spacing: 0.08em; }"
		"QLabel#unitSelectionSummaryTitle { color: rgb(241, 244, 247); font-size: 14px; font-weight: 700; }"
		"QLabel#unitSelectionSummaryMeta { color: rgb(176, 185, 196); font-size: 12px; }"
	);
	auto* selection_summary_layout = new QVBoxLayout(selection_summary);
	selection_summary_layout->setContentsMargins(12, 12, 12, 12);
	selection_summary_layout->setSpacing(4);
	auto* selection_summary_label = new QLabel("ACTIVE SELECTION", selection_summary);
	selection_summary_label->setObjectName("unitSelectionSummaryLabel");
	selection_summary_title = new QLabel("No unit selected", selection_summary);
	selection_summary_title->setObjectName("unitSelectionSummaryTitle");
	selection_summary_meta = new QLabel("Choose a unit from the list to set the current placement target.", selection_summary);
	selection_summary_meta->setObjectName("unitSelectionSummaryMeta");
	selection_summary_meta->setWordWrap(true);
	selection_summary_layout->addWidget(selection_summary_label);
	selection_summary_layout->addWidget(selection_summary_title);
	selection_summary_layout->addWidget(selection_summary_meta);
	ui.verticalLayout->addWidget(selection_summary);
	selection_summary->show();

	find_this = new QShortcut(QKeySequence(Qt::CTRL | Qt::Key_F), this, nullptr, nullptr, Qt::ShortcutContext::WindowShortcut);
	find_parent = new QShortcut(QKeySequence(Qt::CTRL | Qt::Key_F), parent, nullptr, nullptr, Qt::ShortcutContext::WindowShortcut);
	selection_mode->setShortCut(Qt::Key_Space, { window() });

	current_selection_section = new QRibbonSection;
	current_selection_section->setText("Current Selection");
	current_selection_section->setEnabled(false);

	QSmallRibbonButton* edit_in_oe = new QSmallRibbonButton;
	edit_in_oe->setText("Edit in OE");
	edit_in_oe->setIcon(QIcon("data/icons/Ribbon/objecteditor.png"));

	QSmallRibbonButton* select_in_palette = new QSmallRibbonButton;
	select_in_palette->setText("Select in Palette");
	select_in_palette->setToolTip("Or click the unit with middle mouse button");
	select_in_palette->setIcon(QIcon("data/icons/Ribbon/units.png"));

	QVBoxLayout* info_layout = new QVBoxLayout;
	info_layout->addWidget(selection_name);
	info_layout->addWidget(edit_in_oe);
	info_layout->addWidget(select_in_palette);

	current_selection_section->addLayout(info_layout);

	ribbon_tab->addSection(selection_section);
	ribbon_tab->addSection(current_selection_section);
	
	connect(selection_mode, &QRibbonButton::toggled, [&]() { brush.switch_mode(); });

	connect(ui.player, QOverload<int>::of(&QComboBox::currentIndexChanged), [&]() {
		brush.player_id = ui.player->currentData().toInt();
	});

	connect(find_this, &QShortcut::activated, [&]() {
		activateWindow();
		selector->search->setFocus();
		selector->search->selectAll();
	});

	connect(find_parent, &QShortcut::activated, [&]() {
		activateWindow();
		selector->search->setFocus();
		selector->search->selectAll();
	});

	
	connect(selector, &UnitSelector::unitSelected, [&](const std::string& id) { 
		brush.set_unit(id); 
		QString preview_title;
		if (units_slk.row_headers.contains(id)) {
			preview_title = units_table->data(id, "name").toString();
		} else if (items_slk.row_headers.contains(id)) {
			preview_title = items_table->data(id, "name").toString();
		} else {
			preview_title = QString::fromStdString(id);
		}
		emit preview_unit_changed(QString::fromStdString(id), preview_title);
		selection_mode->setChecked(false);
	});

	connect(edit_in_oe, &QSmallRibbonButton::clicked, [&]() {
		bool created;
		const auto editor = window_handler.create_or_raise<ObjectEditor>(nullptr, created);
		const Unit* unit = *brush.selections.begin();
		if (unit->id == "sloc") {
			return;
		}
		if (items_slk.row_headers.contains(unit->id)) {
			editor->select_id(ObjectEditor::Category::item, unit->id);
		} else {
			editor->select_id(ObjectEditor::Category::unit, unit->id);
		}
	});

	connect(select_in_palette, &QSmallRibbonButton::clicked, [&]() {
		const Unit* unit= *brush.selections.begin();
		if (unit->id == "sloc") {
			return;
		}
		select_id_in_palette(unit->id);
	});

	connect(&brush, &UnitBrush::selection_changed, this, &UnitPalette::update_selection_info);
	monitor_activation();
}

UnitPalette::~UnitPalette() {
	map->brush = nullptr;
	delete ribbon_tab;
}

void UnitPalette::activate_palette() {
	find_this->setEnabled(true);
	find_parent->setEnabled(true);
	selection_mode->enableShortcuts();
	if (brush.get_mode() != Brush::Mode::selection) {
		selection_mode->setChecked(true);
	}
	map->brush = &brush;
	emit ribbon_tab_requested(ribbon_tab, "Unit Palette");
}

void UnitPalette::select_id_in_palette(std::string id) {
	selector->search->clear();

	const auto index = selector->filter_model->mapFromSource(selector->list_model->mapFromSource(units_table->rowIDToIndex(id)));
	selector->units->setCurrentIndex(index);
	emit selector->unitSelected(id);
}

void UnitPalette::deactivate(QRibbonTab* tab) {
	if (tab != ribbon_tab) {
		brush.clear_selection();
		selection_mode->disableShortcuts();
		find_this->setEnabled(false);
		find_parent->setEnabled(false);
	}
}

void UnitPalette::update_selection_info() {
	if (brush.selections.empty()) {
		if (current_selection_section->isEnabled()) {
			current_selection_section->setEnabled(false);
		}
		selection_name->setText("");
		selection_summary_title->setText("No unit selected");
		selection_summary_meta->setText("Choose a unit from the list to set the current placement target.");
	} else {
		if (!current_selection_section->isEnabled()) {
			current_selection_section->setEnabled(true);
		}
		const Unit& unit = **brush.selections.begin();

		bool same_object = true;
		for (const auto& i : brush.selections) {
			same_object = same_object && i->id == unit.id;
		}

		// Set the name
		if (same_object) {
			if (unit.id == "sloc") {
				selection_name->setText(QString("Player Start Location (%1)").arg(unit.player + 1));
				selection_summary_title->setText(selection_name->text());
				selection_summary_meta->setText("Special map start location | Selection-only entity");
				emit preview_unit_changed("", selection_summary_title->text());
			} else {
				auto index = units_table->index(units_slk.row_headers.at(unit.id), units_slk.column_headers.at("name"));
				selection_name->setText(units_table->data(index).toString());
				selection_summary_title->setText(selection_name->text());
				selection_summary_meta->setText(
					QString("%1 | %2 selected").arg(QString::fromStdString(unit.id)).arg(brush.selections.size())
				);
				emit preview_unit_changed(QString::fromStdString(unit.id), selection_name->text());
			}
		} else {
			selection_name->setText("Various");
			selection_summary_title->setText("Mixed selection");
			selection_summary_meta->setText(QString("%1 units selected").arg(brush.selections.size()));
		}
	}
}
