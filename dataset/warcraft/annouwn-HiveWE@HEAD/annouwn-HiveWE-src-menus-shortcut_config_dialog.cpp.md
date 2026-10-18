---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/shortcut_config_dialog.cpp"
ref: HEAD
---

#include "shortcut_config_dialog.h"

#include <QDialogButtonBox>
#include <QHeaderView>
#include <QLabel>
#include <QTableWidget>
#include <QTableWidgetItem>
#include <QVBoxLayout>

namespace {
struct ShortcutEntry {
	QString description;
	QString shortcut;
};

struct ShortcutCategory {
	QString name;
	std::vector<ShortcutEntry> entries;
};

const std::array shortcut_categories = {
	ShortcutCategory {
		"Main Window",
		{
			{"Open global search.", "Double Shift"},
			{"Undo the last action.", "Ctrl+Z"},
			{"Redo the last undone action.", "Ctrl+Y"},
			{"Open a map folder.", "Ctrl+O"},
			{"Save the current map.", "Ctrl+S"},
			{"Save the current map under a new folder.", "Ctrl+Shift+S"},
			{"Reset the camera to its default view.", "Ctrl+Shift+C"},
			{"Open the terrain palette.", "T"},
			{"Open the doodad palette.", "D"},
			{"Open the unit palette.", "U"},
			{"Open the region palette.", "R"},
			{"Open or close the object editor.", "O"},
			{"Open the pathing palette.", "P"},
			{"Show or hide units.", "Ctrl+U"},
			{"Show or hide doodads.", "Ctrl+D"},
			{"Show or hide the pathing overlay.", "Ctrl+P"},
			{"Turn lighting on or off.", "Ctrl+L"},
			{"Show or hide water.", "Ctrl+W"},
			{"Show or hide click helper markers.", "Ctrl+I"},
			{"Show or hide wireframe rendering.", "Ctrl+T"},
			{"Show or hide debug rendering.", "F3"},
			{"Reload the active theme.", "F5"},
		},
	},
	ShortcutCategory {
		"Palette",
		{
			{"Doodad palette: Focus the search field.", "Ctrl+F"},
			{"Unit palette: Focus the search field.", "Ctrl+F"},
		},
	},
	ShortcutCategory {
		"Brush",
		{
			{"Increase brush size.", "="},
			{"Decrease brush size.", "-"},
			{"Terrain brush: Change brush size without hovering the slider.", "Shift + Mouse Wheel"},
			{"Brush selection: Clear the current selection.", "Esc"},
			{"Brush selection: Delete the current selection.", "Delete"},
			{"Brush selection: Cut the current selection.", "Ctrl+X"},
			{"Brush selection: Copy the current selection.", "Ctrl+C"},
			{"Brush selection: Enter paste mode for the clipboard selection.", "Ctrl+V"},
		},
	},
	ShortcutCategory {
		"Doodads",
		{
			{"Doodad or unit selection: Select all placed objects of the active type.", "Ctrl+A"},
			{"Doodad or unit selection: Move selected objects in small steps with the numpad.", "Numpad 1-9"},
			{"Doodad selection: Raise absolute height.", "Ctrl+Page Up"},
			{"Doodad selection: Lower absolute height.", "Ctrl+Page Down"},
			{"Doodad selection: Increase Z scale.", "Page Up"},
			{"Doodad selection: Decrease Z scale.", "Page Down"},
		},
	},
	ShortcutCategory {
		"Trigger",
		{
			{"Trigger editor: Open global search.", "Double Shift"},
			{"Trigger editor: Focus the search window.", "Ctrl+F"},
			{"Trigger editor: Generate script.", "Ctrl+F10"},
		},
	},
	ShortcutCategory {
		"Misc",
		{
			{"Toggle selection mode in supported editors and palettes.", "Space"},
			{"Object editor: Open global search.", "Double Shift"},
			{"Object editor: Focus the active search field.", "Ctrl+F"},
		},
	},
};

int row_count() {
	int count = 0;
	for (const auto& category : shortcut_categories) {
		count += 1 + static_cast<int>(category.entries.size());
	}
	return count;
}
}

ShortcutConfigDialog::ShortcutConfigDialog(QWidget* parent) : QDialog(parent) {
	setAttribute(Qt::WA_DeleteOnClose);
	setWindowTitle("Config");
	resize(860, 640);

	auto* layout = new QVBoxLayout(this);

	auto* intro = new QLabel("Current shortcut and input reference.");
	layout->addWidget(intro);

	auto* table = new QTableWidget(row_count(), 2, this);
	table->setHorizontalHeaderLabels({"Description", "Shortcut"});
	table->verticalHeader()->setVisible(false);
	table->setEditTriggers(QAbstractItemView::NoEditTriggers);
	table->setSelectionBehavior(QAbstractItemView::SelectRows);
	table->setSelectionMode(QAbstractItemView::SingleSelection);
	table->setWordWrap(true);
	table->setAlternatingRowColors(false);
	table->horizontalHeader()->setStretchLastSection(false);
	table->horizontalHeader()->setSectionResizeMode(0, QHeaderView::Stretch);
	table->horizontalHeader()->setSectionResizeMode(1, QHeaderView::Fixed);
	table->setColumnWidth(1, 220);

	const auto data_background = palette().base();
	const auto header_background = palette().alternateBase();

	int row = 0;
	for (const auto& category : shortcut_categories) {
		table->setSpan(row, 0, 1, 2);
		auto* header = new QTableWidgetItem(category.name);
		header->setFlags(Qt::ItemIsEnabled);
		header->setTextAlignment(Qt::AlignLeft | Qt::AlignVCenter);
		header->setBackground(header_background);
		table->setItem(row, 0, header);
		++row;

		for (const auto& entry : category.entries) {
			auto* description = new QTableWidgetItem(entry.description);
			auto* shortcut = new QTableWidgetItem(entry.shortcut);
			description->setBackground(data_background);
			shortcut->setBackground(data_background);
			table->setItem(row, 0, description);
			table->setItem(row, 1, shortcut);
			++row;
		}
	}

	layout->addWidget(table);

	auto* button_box = new QDialogButtonBox(QDialogButtonBox::Close, this);
	connect(button_box, &QDialogButtonBox::rejected, this, &QDialog::close);
	connect(button_box, &QDialogButtonBox::accepted, this, &QDialog::close);
	layout->addWidget(button_box);

	show();
}
