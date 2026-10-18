---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/pathing_palette.cpp"
ref: HEAD
---

#include "pathing_palette.h"

#include <QImage>
#include <QSettings>
#include <QFileDialog>
#include <QMessageBox>

import std;
import MapGlobal;

namespace fs = std::filesystem;

PathingPalette::PathingPalette(QWidget *parent) : Palette(parent) {
	ui.setupUi(this);
	setObjectName("pathingPalette");
	monitor_activation();
	ui.verticalLayout->setContentsMargins(0, 0, 0, 0);
	ui.verticalLayout->setSpacing(10);
	ui.verticalLayout->setAlignment(Qt::AlignTop);
	ui.ToolTypeLayout->setSpacing(5);
	ui.horizontalLayout->setSpacing(5);
	ui.brushOperationLabel->setContentsMargins(0, 3, 0, 3);
	ui.brushTypeLabel->setContentsMargins(0, 5, 0, 3);
	ui.brushOperationLabel->setText("Operation");
	ui.brushTypeLabel->setText("Pathing Mask");
	ui.brushSizeLabel->setText("Brush Size");
	ui.verticalLayout->removeItem(ui.horizontalLayout_2);
	ui.verticalLayout->removeItem(ui.brushShapeLayout);
	ui.brushSize1->hide();
	ui.brushSize3->hide();
	ui.brushSize5->hide();
	ui.brushSize7->hide();
	ui.brushSize9->hide();
	ui.brushSize11->hide();
	ui.brushShapeCircle->hide();
	ui.brushShapeSquare->hide();
	ui.brushShapeDiamond->hide();
	ui.brushSize->setReadOnly(true);
	ui.brushSize->setButtonSymbols(QAbstractSpinBox::NoButtons);
	setStyleSheet(
		"QLabel { color: rgb(212, 219, 227); }"
		"QLabel#brushOperationLabel, QLabel#brushTypeLabel, QLabel#brushSizeLabel {"
		"font-size: 11px;"
		"font-weight: 600;"
		"color: rgb(140, 149, 162);"
		"}"
		"QPushButton, QRadioButton { color: rgb(232, 236, 241); }"
		"QPushButton {"
		"background: rgba(24, 27, 31, 205);"
		"border: 1px solid rgba(255, 255, 255, 16);"
		"border-radius: 10px;"
		"}"
		"QPushButton:hover { background: rgba(34, 38, 44, 220); }"
		"QPushButton:checked { background: rgba(95, 164, 255, 200); border: 1px solid rgba(95, 164, 255, 235); }"
		"QSpinBox {"
		"background: rgba(24, 27, 31, 235);"
		"border: 1px solid rgba(255, 255, 255, 18);"
		"border-radius: 10px;"
		"padding: 6px 10px;"
		"color: rgb(232, 236, 241);"
		"}"
		"QSlider::groove:horizontal { height: 6px; background: rgba(255, 255, 255, 20); border-radius: 999px; }"
		"QSlider::handle:horizontal { width: 16px; margin: -5px 0; background: rgb(95, 164, 255); border-radius: 8px; }"
	);

	QRibbonSection* selection_section = new QRibbonSection;
	selection_section->setText("Selection");

	selection_mode->setText("Selection\nMode");
	selection_mode->setIcon(QIcon("data/icons/Ribbon/select.png"));
	selection_mode->setCheckable(true);
	selection_mode->setEnabled(false);
	selection_section->addWidget(selection_mode);

	QRibbonSection* tools_section = new QRibbonSection;
	tools_section->setText("Tools");

	import_pathing->setText("Import\nPathing");
	import_pathing->setIcon(QIcon("data/icons/pathing_palette/import.png"));
	tools_section->addWidget(import_pathing);

	export_pathing->setText("Export\nPathing");
	export_pathing->setIcon(QIcon("data/icons/pathing_palette/export.png"));
	tools_section->addWidget(export_pathing);

	ribbon_tab->addSection(selection_section);
	ribbon_tab->addSection(tools_section);

	connect(ui.replaceType, &QPushButton::clicked, [&]() { brush.operation = PathingBrush::Operation::replace; });
	connect(ui.addType, &QPushButton::clicked, [&]() { brush.operation = PathingBrush::Operation::add; });
	connect(ui.removeType, &QPushButton::clicked, [&]() { brush.operation = PathingBrush::Operation::remove; });

	connect(ui.brushSizeSlider, &QSlider::valueChanged, [&](int value) {
		brush.set_size(glm::ivec2(value));
		ui.brushSize->setValue(value);
	});

	connect(&brush, &Brush::size_changed, this, [&](glm::ivec2) {
		sync_brush_controls();
	});

	connect(ui.brushTypeGroup, &QButtonGroup::buttonToggled, [&]() {
		brush.brush_mask = 0;

		if (ui.walkable->isChecked()) {
			brush.brush_mask |= 0b00000010;
		}
		if (ui.flyable->isChecked()) {
			brush.brush_mask |= 0b00000100;
		}
		if (ui.buildable->isChecked()) {
			brush.brush_mask |= 0b00001000;
		}
	});

	connect(import_pathing, &QSmallRibbonButton::clicked, [&]() {
		QSettings settings;
		const QString directory = settings.value("openDirectoryPathing", QDir::current().path()).toString();

		const QString file_name = QFileDialog::getOpenFileName(this, "Open Pathing Image", directory, "Images (*.png *.jpg *.jpeg *.bmp *.gif)");

		if (file_name == "") {
			return;
		}

		const fs::path path(file_name.toStdString());
		settings.setValue("openDirectoryPathing", QString::fromStdString(path.parent_path().string()));

		QImage image;
		if (!image.load(file_name)) {
			QMessageBox::critical(this, "Error", "Failed to open image");
		}
		image = image.convertToFormat(QImage::Format::Format_RGB888);
		image.flip(Qt::Orientation::Vertical);

		const bool success = map->pathing_map.from_rgb(std::span{const_cast<uint8_t*>(image.constBits()), static_cast<size_t>(image.sizeInBytes())});
		if (!success) {
			const auto msg = std::format("Failed to load image. It has to be a {}x{} RGB image", map->pathing_map.width, map->pathing_map.height);
			QMessageBox::critical(this, "Error", QString::fromStdString(msg));
		}
	});

	connect(export_pathing, &QSmallRibbonButton::clicked, [&]() {
		QSettings settings;
		const QString directory = settings.value("openDirectoryPathing", QDir::current().path()).toString() + "/pathing_map.png";

		const QString file_name = QFileDialog::getSaveFileName(this, "Open Heightmap Image", directory);

		if (file_name == "") {
			return;
		}

		const fs::path path(file_name.toStdString());
		settings.setValue("openDirectoryPathing", QString::fromStdString(path.parent_path().string()));

		const auto data = map->pathing_map.to_rgb();
		QImage image(data.data(), map->pathing_map.width, map->pathing_map.height, QImage::Format_RGB888);
		image.flip(Qt::Orientation::Vertical);
		if (!image.save(file_name, "PNG")) {
			QMessageBox::critical(this, "Error", "Failed to save image");
		}
	});

	sync_brush_controls();
	monitor_activation();
}

PathingPalette::~PathingPalette() {
	map->brush = nullptr;
	delete ribbon_tab;
}

void PathingPalette::activate_palette() {
	selection_mode->enableShortcuts();
	map->brush = &brush;
	sync_brush_controls();
}

void PathingPalette::deactivate(QRibbonTab* tab) {
	if (tab != ribbon_tab) {
		brush.clear_selection();
		selection_mode->disableShortcuts();
	}
}

void PathingPalette::sync_brush_controls() {
	const QSignalBlocker slider_blocker(ui.brushSizeSlider);
	const QSignalBlocker spinbox_blocker(ui.brushSize);
	const auto size = brush.get_size();

	ui.brushSizeSlider->setSingleStep(4);
	ui.brushSize->setSingleStep(4);
	ui.brushSizeSlider->setValue(size.x);
	ui.brushSize->setValue(size.x);
}
