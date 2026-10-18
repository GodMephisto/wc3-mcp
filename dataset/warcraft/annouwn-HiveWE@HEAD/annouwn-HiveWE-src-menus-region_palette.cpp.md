---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/region_palette.cpp"
ref: HEAD
---

#include "region_palette.h"

#include <QColorDialog>
#include <QFormLayout>
#include <QHBoxLayout>
#include <QSignalBlocker>

import std;
import Camera;
import MapGlobal;

RegionPalette::RegionPalette(QWidget* parent) : Palette(parent) {
	setObjectName("regionPalette");
	monitor_activation();

	auto* layout = new QVBoxLayout(this);
	layout->setContentsMargins(0, 0, 0, 0);
	layout->setSpacing(10);
	setStyleSheet(
		"QPushButton { color: rgb(229, 233, 239); }"
		"QLabel { color: rgb(214, 221, 229); }"
	);

	region_list->setSelectionMode(QAbstractItemView::SingleSelection);
	region_list->setObjectName("regionList");
	region_list->setSpacing(4);
	region_list->setUniformItemSizes(true);
	name->setObjectName("regionNameField");
	color->setObjectName("regionColorButton");
	new_region->setText("New");
	delete_region->setText("Delete");

	selection_summary = new QFrame(this);
	selection_summary->setObjectName("regionSelectionSummary");
	selection_summary->setStyleSheet(
		"QFrame#regionSelectionSummary { background: rgba(24, 27, 31, 200); border: 1px solid rgba(255, 255, 255, 14); border-radius: 12px; }"
		"QLabel#regionSelectionSummaryLabel { color: rgb(140, 149, 162); font-size: 10px; font-weight: 600; letter-spacing: 0.08em; }"
		"QLabel#regionSelectionSummaryTitle { color: rgb(241, 244, 247); font-size: 14px; font-weight: 700; }"
		"QLabel#regionSelectionSummaryMeta { color: rgb(176, 185, 196); font-size: 12px; }"
	);
	auto* selection_summary_layout = new QVBoxLayout(selection_summary);
	selection_summary_layout->setContentsMargins(12, 12, 12, 12);
	selection_summary_layout->setSpacing(4);
	auto* selection_summary_label = new QLabel("ACTIVE REGION", selection_summary);
	selection_summary_label->setObjectName("regionSelectionSummaryLabel");
	selection_summary_title = new QLabel("No region selected", selection_summary);
	selection_summary_title->setObjectName("regionSelectionSummaryTitle");
	selection_summary_meta = new QLabel("Create a region or select one from the list to edit its properties.", selection_summary);
	selection_summary_meta->setObjectName("regionSelectionSummaryMeta");
	selection_summary_meta->setWordWrap(true);
	selection_summary_layout->addWidget(selection_summary_label);
	selection_summary_layout->addWidget(selection_summary_title);
	selection_summary_layout->addWidget(selection_summary_meta);

	auto* button_row = new QHBoxLayout;
	button_row->setContentsMargins(0, 0, 0, 0);
	button_row->addWidget(new_region);
	button_row->addWidget(delete_region);

	auto* form = new QFormLayout;
	form->setContentsMargins(0, 0, 0, 0);
	form->setVerticalSpacing(8);
	form->setHorizontalSpacing(10);
	form->addRow("Name", name);
	form->addRow("Color", color);

	layout->addWidget(region_list, 1);
	layout->addWidget(selection_summary);
	layout->addLayout(button_row);
	layout->addLayout(form);

	change_mode_parent = new QShortcut(Qt::Key_Space, window(), nullptr, nullptr, Qt::ShortcutContext::WindowShortcut);

	selection_mode->setText("Selection\nMode");
	selection_mode->setIcon(QIcon("data/icons/ribbon/select.png"));
	selection_mode->setCheckable(true);
	selection_mode->setChecked(true);

	auto* selection_section = new QRibbonSection;
	selection_section->setText("Selection");
	selection_section->addWidget(selection_mode);

	auto* region_section = new QRibbonSection;
	region_section->setText("Regions");

	auto* new_region_button = new QRibbonButton;
	new_region_button->setText("New\nRegion");
	new_region_button->setIcon(QIcon("data/icons/ribbon/new.ico"));
	region_section->addWidget(new_region_button);

	auto* delete_region_button = new QRibbonButton;
	delete_region_button->setText("Delete\nRegion");
	delete_region_button->setIcon(QIcon("data/icons/ribbon/exit.ico"));
	region_section->addWidget(delete_region_button);

	ribbon_tab->addSection(selection_section);
	ribbon_tab->addSection(region_section);

	connect(selection_mode, &QRibbonButton::toggled, [&]() { brush.switch_mode(); });
	connect(change_mode_parent, &QShortcut::activated, selection_mode, &QToolButton::click);

	connect(new_region_button, &QRibbonButton::clicked, &brush, &RegionBrush::create_region_at_cursor);
	connect(delete_region_button, &QRibbonButton::clicked, &brush, &RegionBrush::delete_selection);
	connect(new_region, &QPushButton::clicked, &brush, &RegionBrush::create_region_at_cursor);
	connect(delete_region, &QPushButton::clicked, &brush, &RegionBrush::delete_selection);

	connect(region_list, &QListWidget::currentRowChanged, &brush, &RegionBrush::select_region);
	connect(region_list, &QListWidget::itemDoubleClicked, this, [this](QListWidgetItem* item) {
		if (!item) {
			return;
		}

		const int row = region_list->row(item);
		if (row < 0 || row >= static_cast<int>(map->regions.regions.size())) {
			return;
		}

		if (brush.selected_index() != row) {
			brush.select_region(row);
		}

		const Region& region = map->regions.regions[row];
		camera.position.x = (region.left + region.right) * 0.5f;
		camera.position.y = (region.bottom + region.top) * 0.5f;
	});
	connect(&brush, &Brush::selection_changed, this, [this]() {
		refresh_region_list();
		sync_region_fields();
	});

	connect(name, &QLineEdit::editingFinished, this, [this]() { brush.set_selected_name(name->text()); });
	connect(color, &QPushButton::clicked, this, [this]() {
		const auto* selected = brush.selected_region();
		const QColor current = selected ? QColor(static_cast<int>(selected->color.r), static_cast<int>(selected->color.g), static_cast<int>(selected->color.b))
										: QColor(255, 120, 210);
		const QColor chosen = QColorDialog::getColor(current, this);
		if (!chosen.isValid()) {
			return;
		}
		brush.set_selected_color(chosen);
		update_color_button(chosen);
	});

	name->setPlaceholderText("Region name");

	refresh_region_list();
	sync_region_fields();
}

RegionPalette::~RegionPalette() {
	if (map->brush == &brush) {
		map->brush = nullptr;
	}
	delete change_mode_parent;
	delete ribbon_tab;
}

void RegionPalette::activate_palette() {
	change_mode_parent->setEnabled(true);
	if (brush.get_mode() != Brush::Mode::selection) {
		selection_mode->setChecked(true);
	}
	map->brush = &brush;
	emit ribbon_tab_requested(ribbon_tab, "Region Palette");
}

void RegionPalette::deactivate(QRibbonTab* tab) {
	if (tab != ribbon_tab) {
		change_mode_parent->setEnabled(false);
	}
}

void RegionPalette::refresh_region_list() {
	const QSignalBlocker blocker(region_list);
	const int selected = brush.selected_index();

	if (region_list->count() != static_cast<int>(map->regions.regions.size())) {
		region_list->clear();
		for (const auto& region : map->regions.regions) {
			region_list->addItem(QString::fromStdString(region.name));
		}
	} else {
		for (int i = 0; i < region_list->count(); ++i) {
			const QString label = QString::fromStdString(map->regions.regions[i].name);
			if (region_list->item(i)->text() != label) {
				region_list->item(i)->setText(label);
			}
		}
	}

	if (selected >= 0 && selected < region_list->count()) {
		region_list->setCurrentRow(selected);
	} else {
		region_list->setCurrentRow(-1);
	}
}

void RegionPalette::sync_region_fields() {
	const auto* selected = brush.selected_region();
	const QSignalBlocker name_blocker(name);

	const bool has_selection = selected != nullptr;
	name->setEnabled(has_selection);
	color->setEnabled(has_selection);
	delete_region->setEnabled(has_selection);

	if (!selected) {
		name->clear();
		update_color_button(QColor(80, 80, 80));
		selection_summary_title->setText("No region selected");
		selection_summary_meta->setText("Create a region or select one from the list to edit its properties.");
		return;
	}

	name->setText(QString::fromStdString(selected->name));
	update_color_button(QColor(static_cast<int>(selected->color.r), static_cast<int>(selected->color.g), static_cast<int>(selected->color.b)));
	selection_summary_title->setText(QString::fromStdString(selected->name));
	selection_summary_meta->setText(
		QString("Bounds: %1, %2 to %3, %4")
			.arg(selected->left)
			.arg(selected->bottom)
			.arg(selected->right)
			.arg(selected->top)
	);
}

void RegionPalette::update_color_button(const QColor& new_color) {
	const int brightness = static_cast<int>(new_color.red() * 0.299 + new_color.green() * 0.587 + new_color.blue() * 0.114);
	const QColor text_color = brightness > 160 ? QColor(Qt::black) : QColor(Qt::white);
	color->setStyleSheet(
		QString("QPushButton { background-color: %1; color: %2; }").arg(new_color.name(), text_color.name())
	);
}
