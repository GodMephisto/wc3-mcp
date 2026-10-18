---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/new_map_dialog.cpp"
ref: HEAD
---

#include "new_map_dialog.h"

#include <QComboBox>
#include <QDialogButtonBox>
#include <QFrame>
#include <QGridLayout>
#include <QHBoxLayout>
#include <QLabel>
#include <QLineEdit>
#include <QPushButton>
#include <QVBoxLayout>

import std;
import Globals;
import OpenGLUtilities;
import ResourceManager;
import SLK;
import Texture;

namespace {
QFrame* create_section(const QString& title, const QString& subtitle) {
	QFrame* section = new QFrame;
	section->setObjectName("newMapSection");

	auto* layout = new QVBoxLayout(section);
	layout->setContentsMargins(18, 18, 18, 18);
	layout->setSpacing(12);

	QLabel* title_label = new QLabel(title);
	title_label->setObjectName("newMapSectionTitle");
	layout->addWidget(title_label);

	QLabel* subtitle_label = new QLabel(subtitle);
	subtitle_label->setObjectName("newMapSectionSubtitle");
	subtitle_label->setWordWrap(true);
	layout->addWidget(subtitle_label);

	return section;
}
}

NewMapDialog::NewMapDialog(QWidget* parent) : QDialog(parent) {
	setWindowTitle("Create New Map");
	setModal(true);
	resize(760, 520);

	setStyleSheet(
		"QDialog {"
		"background: rgb(17, 23, 31);"
		"color: rgb(234, 239, 244);"
		"}"
		"#newMapTitle {"
		"font-size: 26px;"
		"font-weight: 700;"
		"color: rgb(248, 250, 252);"
		"}"
		"#newMapIntro {"
		"font-size: 13px;"
		"color: rgb(164, 175, 188);"
		"}"
		"#newMapSection {"
		"background: rgb(24, 31, 41);"
		"border: 1px solid rgba(255, 255, 255, 18);"
		"border-radius: 14px;"
		"}"
		"#newMapSectionTitle {"
		"font-size: 15px;"
		"font-weight: 700;"
		"color: rgb(244, 247, 250);"
		"}"
		"#newMapSectionSubtitle {"
		"font-size: 12px;"
		"color: rgb(151, 163, 176);"
		"}"
		"#newMapSummaryCard {"
		"background: qlineargradient(x1:0, y1:0, x2:1, y2:1, stop:0 rgb(39, 61, 88), stop:1 rgb(21, 34, 49));"
		"border: 1px solid rgba(153, 199, 255, 64);"
		"border-radius: 14px;"
		"}"
		"#newMapTilesetPreview {"
		"background: rgb(20, 27, 36);"
		"border: 1px solid rgba(255, 255, 255, 18);"
		"border-radius: 12px;"
		"}"
		"#newMapTileThumb {"
		"background: rgba(255, 255, 255, 5);"
		"border: 1px solid rgba(255, 255, 255, 12);"
		"border-radius: 8px;"
		"padding: 2px;"
		"}"
		"#newMapSummaryLabel {"
		"font-size: 12px;"
		"font-weight: 600;"
		"letter-spacing: 0.08em;"
		"text-transform: uppercase;"
		"color: rgb(165, 205, 255);"
		"}"
		"#newMapSummaryValue {"
		"font-size: 20px;"
		"font-weight: 700;"
		"color: rgb(248, 250, 252);"
		"}"
		"QLabel {"
		"color: rgb(228, 233, 239);"
		"}"
		"QLineEdit, QComboBox, QSpinBox {"
		"min-height: 36px;"
		"padding: 4px 10px;"
		"background: rgb(14, 19, 27);"
		"border: 1px solid rgba(255, 255, 255, 22);"
		"border-radius: 9px;"
		"color: rgb(241, 245, 249);"
		"}"
		"QLineEdit:focus, QComboBox:focus, QSpinBox:focus {"
		"border: 1px solid rgb(92, 157, 226);"
		"}"
		"QComboBox::drop-down, QSpinBox::up-button, QSpinBox::down-button {"
		"border: 0px;"
		"width: 22px;"
		"}"
		"QDialogButtonBox QPushButton {"
		"min-width: 122px;"
		"min-height: 38px;"
		"padding: 0 16px;"
		"border-radius: 10px;"
		"border: 1px solid rgba(255, 255, 255, 22);"
		"background: rgb(28, 36, 47);"
		"color: rgb(236, 241, 246);"
		"font-weight: 600;"
		"}"
		"QDialogButtonBox QPushButton:hover {"
		"background: rgb(36, 46, 60);"
		"}"
		"QDialogButtonBox QPushButton:default {"
		"background: rgb(70, 126, 192);"
		"border-color: rgb(112, 172, 242);"
		"}"
	);

	auto* root = new QVBoxLayout(this);
	root->setContentsMargins(24, 24, 24, 24);
	root->setSpacing(18);

	QLabel* title = new QLabel("Start a new world");
	title->setObjectName("newMapTitle");
	root->addWidget(title);

	QLabel* intro = new QLabel("Set the map scale and starting tileset for the new map.");
	intro->setObjectName("newMapIntro");
	intro->setWordWrap(true);
	root->addWidget(intro);

	auto* content = new QHBoxLayout;
	content->setSpacing(18);
	root->addLayout(content, 1);

	QFrame* left_section = create_section("Map Setup", "Core identity and world scale for the new map.");
	content->addWidget(left_section, 3);

	auto* left_layout = qobject_cast<QVBoxLayout*>(left_section->layout());
	auto* form = new QGridLayout;
	form->setHorizontalSpacing(12);
	form->setVerticalSpacing(12);
	left_layout->addLayout(form);

	map_name = new QLineEdit("New Map");
	form->addWidget(new QLabel("Map Name"), 0, 0);
	form->addWidget(map_name, 0, 1, 1, 3);

	tileset = new QComboBox;
	for (const auto& [key, value] : world_edit_data.section("TileSets")) {
		tileset->addItem(QString::fromStdString(value[0]), QString::fromStdString(key));
	}
	form->addWidget(new QLabel("Tileset"), 1, 0);
	form->addWidget(tileset, 1, 1, 1, 3);

	width = new QComboBox;
	height = new QComboBox;
	for (int size = 32; size <= 480; size += 32) {
		const QString label = QString::number(size);
		width->addItem(label, size);
		height->addItem(label, size);
	}
	width->setCurrentText("64");
	height->setCurrentText("64");
	form->addWidget(new QLabel("Width"), 2, 0);
	form->addWidget(width, 2, 1);
	form->addWidget(new QLabel("Height"), 2, 2);
	form->addWidget(height, 2, 3);

	left_layout->addStretch();

	QFrame* right_section = create_section("Live Summary", "A quick read on the map footprint before you create it.");
	content->addWidget(right_section, 2);
	auto* right_layout = qobject_cast<QVBoxLayout*>(right_section->layout());

	QFrame* summary = new QFrame;
	summary->setObjectName("newMapSummaryCard");
	auto* summary_layout = new QVBoxLayout(summary);
	summary_layout->setContentsMargins(18, 18, 18, 18);
	summary_layout->setSpacing(8);

	QLabel* summary_label = new QLabel("Map Scale");
	summary_label->setObjectName("newMapSummaryLabel");
	summary_layout->addWidget(summary_label);

	size_description = new QLabel;
	size_description->setObjectName("newMapSummaryValue");
	summary_layout->addWidget(size_description);

	playable_summary = new QLabel;
	playable_summary->setWordWrap(true);
	playable_summary->setStyleSheet("color: rgb(213, 223, 234); font-size: 13px;");
	summary_layout->addWidget(playable_summary);

	QLabel* tileset_label = new QLabel("Tileset Preview");
	tileset_label->setObjectName("newMapSummaryLabel");
	summary_layout->addWidget(tileset_label);

	QFrame* preview_frame = new QFrame;
	preview_frame->setObjectName("newMapTilesetPreview");
	tileset_preview = new QGridLayout(preview_frame);
	tileset_preview->setContentsMargins(12, 12, 12, 12);
	tileset_preview->setHorizontalSpacing(8);
	tileset_preview->setVerticalSpacing(8);
	summary_layout->addWidget(preview_frame);

	right_layout->addWidget(summary);
	right_layout->addStretch();

	auto* buttons = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
	buttons->button(QDialogButtonBox::Ok)->setText("Create Map");
	buttons->button(QDialogButtonBox::Ok)->setDefault(true);
	root->addWidget(buttons);

	connect(buttons, &QDialogButtonBox::accepted, this, &QDialog::accept);
	connect(buttons, &QDialogButtonBox::rejected, this, &QDialog::reject);

	connect(tileset, &QComboBox::currentTextChanged, this, [this](const QString&) {
		refresh_summary();
		refresh_tileset_preview();
	});
	connect(width, &QComboBox::currentTextChanged, this, [this](const QString&) { refresh_summary(); });
	connect(height, &QComboBox::currentTextChanged, this, [this](const QString&) { refresh_summary(); });
	refresh_summary();
	refresh_tileset_preview();
}

NewMapDialog::Result NewMapDialog::result() const {
	Result settings;
	settings.map_name = map_name->text().trimmed().isEmpty() ? "New Map" : map_name->text().trimmed();
	settings.tileset = tileset->currentData().toString().toStdString().front();
	settings.width = width->currentData().toInt();
	settings.height = height->currentData().toInt();
	return settings;
}

void NewMapDialog::refresh_summary() const {
	const Result settings = result();

	size_description->setText(QString("%1 x %2  %3").arg(settings.width).arg(settings.height).arg(size_descriptor(settings.width, settings.height)));

	const int playable_width = std::max(1, settings.width - 12);
	const int playable_height = std::max(1, settings.height - 12);

	playable_summary->setText(
		QString("%1 theme.\nPlayable space: %2 x %3.")
			.arg(tileset->currentText())
			.arg(playable_width)
			.arg(playable_height)
	);
}

void NewMapDialog::refresh_tileset_preview() const {
	if (!tileset_preview) {
		return;
	}

	QLayoutItem* item = nullptr;
	while ((item = tileset_preview->takeAt(0)) != nullptr) {
		if (QWidget* widget = item->widget()) {
			widget->deleteLater();
		}
		delete item;
	}

	const std::string selected_tileset = tileset->currentData().toString().toStdString();
	if (selected_tileset.empty()) {
		return;
	}

	slk::SLK terrain_slk("TerrainArt/Terrain.slk");
	std::vector<std::string> tile_ids;
	for (const auto& [id, row] : terrain_slk.row_headers) {
		if (!id.empty() && id.front() == selected_tileset.front()) {
			tile_ids.push_back(id);
		}
	}
	std::sort(tile_ids.begin(), tile_ids.end());

	int shown_tiles = 0;
	for (const auto& tile_id : tile_ids) {
		const auto texture_result = resource_manager.load<Texture>(terrain_slk.data("dir", tile_id) + "\\" + terrain_slk.data("file", tile_id));
		if (!texture_result.has_value()) {
			continue;
		}

		const auto& image = texture_result.value();
		QLabel* thumb = new QLabel;
		thumb->setObjectName("newMapTileThumb");
		thumb->setFixedSize(48, 48);
		thumb->setPixmap(ground_texture_to_icon(image->data.data(), image->width, image->height).pixmap(40, 40));
		thumb->setAlignment(Qt::AlignCenter);
		thumb->setToolTip(QString::fromUtf8(terrain_slk.data<std::string_view>("comment", tile_id)));

		tileset_preview->addWidget(thumb, shown_tiles / 4, shown_tiles % 4);
		++shown_tiles;

		if (shown_tiles >= 8) {
			break;
		}
	}
}

QString NewMapDialog::size_descriptor(const int width, const int height) {
	const int edge = std::max(width, height);
	if (edge <= 64) {
		return "Compact";
	}
	if (edge <= 96) {
		return "Standard";
	}
	if (edge <= 160) {
		return "Large";
	}
	if (edge <= 256) {
		return "Expansive";
	}
	return "Massive";
}
