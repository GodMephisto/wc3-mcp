---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/item_drop_editor.cpp"
ref: HEAD
---

#include "item_drop_editor.h"

#include <QAbstractItemView>
#include <QButtonGroup>
#include <QComboBox>
#include <QDialog>
#include <QDialogButtonBox>
#include <QHBoxLayout>
#include <QLabel>
#include <QLineEdit>
#include <QListView>
#include <QRadioButton>
#include <QSpinBox>
#include <QVBoxLayout>

import std;
import Globals;
import ItemListModel;
import TableModel;

namespace {
struct RandomItemSpec {
	char item_class = 'Y';
	int level = -1;
};

constexpr std::array random_item_classes = {
	std::pair { 'Y', "Any Class" },
	std::pair { 'i', "Permanent" },
	std::pair { 'j', "Charged" },
	std::pair { 'k', "Powerup" },
	std::pair { 'l', "Artifact" },
	std::pair { 'm', "Purchasable" },
	std::pair { 'n', "Campaign" },
	std::pair { 'o', "Miscellaneous" },
};

QString item_name(const std::string_view id) {
	if (!items_slk.row_headers.contains(id)) {
		return QString::fromStdString(std::string(id));
	}

	if (items_slk.column_headers.contains("name")) {
		return items_table->data(id, "name").toString();
	}

	return QString::fromStdString(std::string(id));
}

QIcon item_icon(const std::string_view id) {
	if (!items_slk.row_headers.contains(id) || !items_slk.column_headers.contains("art")) {
		return {};
	}

	const auto index = items_table->index(items_slk.row_headers.at(id), items_slk.column_headers.at("art"));
	const QVariant decoration = items_table->data(index, Qt::DecorationRole);
	if (decoration.canConvert<QIcon>()) {
		return qvariant_cast<QIcon>(decoration);
	}
	return {};
}

std::optional<RandomItemSpec> parse_random_item_id(const std::string_view id) {
	if (id.size() != 4 || id[0] != 'Y' || id[2] != 'I') {
		return std::nullopt;
	}

	const auto found = std::ranges::find(random_item_classes, id[1], &std::pair<char, const char*>::first);
	if (found == random_item_classes.end()) {
		return std::nullopt;
	}

	RandomItemSpec spec;
	spec.item_class = id[1];
	spec.level = id[3] == '/' ? -1 : std::clamp(id[3] - '0', 0, 9);
	return spec;
}

std::string make_random_item_id(const RandomItemSpec& spec) {
	return std::string {
		'Y',
		spec.item_class,
		'I',
		static_cast<char>(spec.level < 0 ? '/' : '0' + std::clamp(spec.level, 0, 9)),
	};
}

QString random_item_description(const RandomItemSpec& spec) {
	const auto found = std::ranges::find(random_item_classes, spec.item_class, &std::pair<char, const char*>::first);
	const QString item_class = found == random_item_classes.end() ? "Unknown Class" : found->second;
	const QString level = spec.level < 0 ? "Any Level" : QString("Level %1").arg(spec.level);
	return QString("%1, %2").arg(item_class, level);
}
}

QString describe_item_drop_entry(const std::pair<int, std::string>& entry) {
	const int chance = entry.first;
	const std::string& id = entry.second;
	if (id.empty()) {
		return QString("%1%  None").arg(chance);
	}

	if (const auto random = parse_random_item_id(id)) {
		return QString("%1%  Random %2").arg(chance).arg(random_item_description(*random));
	}

	return QString("%1%  %2").arg(chance).arg(item_name(id));
}

QIcon icon_for_item_drop_entry(const std::pair<int, std::string>& entry) {
	if (entry.second.empty()) {
		return {};
	}

	if (parse_random_item_id(entry.second)) {
		return QIcon("data/icons/ribbon/objecteditor.png");
	}

	return item_icon(entry.second);
}

bool edit_item_drop_entry(std::pair<int, std::string>& entry, QWidget* parent) {
	QDialog dialog(parent);
	dialog.setWindowTitle("Edit Drop Item");
	dialog.resize(560, 520);

	auto* layout = new QVBoxLayout(&dialog);

	auto* chance_row = new QHBoxLayout;
	auto* chance_label = new QLabel("Chance");
	auto* chance = new QSpinBox;
	chance->setRange(0, 100);
	chance->setValue(entry.first);
	chance->setSuffix("%");
	chance_row->addWidget(chance_label);
	chance_row->addWidget(chance);
	chance_row->addStretch();
	layout->addLayout(chance_row);

	auto* random_option = new QRadioButton("Random");
	auto* specific_option = new QRadioButton("Specific");
	auto* group = new QButtonGroup(&dialog);
	group->addButton(random_option);
	group->addButton(specific_option);

	layout->addWidget(specific_option);
	layout->addWidget(random_option);

	auto* random_row = new QHBoxLayout;
	auto* random_class = new QComboBox;
	auto* random_level = new QComboBox;
	for (const auto& [code, label] : random_item_classes) {
		random_class->addItem(label, QString(code));
	}
	random_level->addItem("Any Level", -1);
	for (int level_value = 0; level_value <= 9; ++level_value) {
		random_level->addItem(QString("Level %1").arg(level_value), level_value);
	}
	random_row->addSpacing(24);
	random_row->addWidget(random_class, 1);
	random_row->addWidget(random_level, 1);
	layout->addLayout(random_row);

	layout->addWidget(specific_option);

	auto* search = new QLineEdit;
	search->setPlaceholderText("Search items...");
	auto* items_view = new QListView;
	items_view->setSelectionMode(QAbstractItemView::SingleSelection);
	items_view->setIconSize({ 20, 20 });

	auto* item_model = new ItemListModel(&dialog);
	item_model->setSourceModel(items_table);
	auto* item_filter = new ItemListFilter(&dialog);
	item_filter->setSourceModel(item_model);
	item_filter->setFilterCaseSensitivity(Qt::CaseInsensitive);
	items_view->setModel(item_filter);

	layout->addSpacing(4);
	layout->addWidget(search);
	layout->addWidget(items_view, 1);

	auto* buttons = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
	layout->addWidget(buttons);

	const auto update_enabled_state = [&]() {
		random_class->setEnabled(random_option->isChecked());
		random_level->setEnabled(random_option->isChecked());
		search->setEnabled(specific_option->isChecked());
		items_view->setEnabled(specific_option->isChecked());
	};

	if (entry.second.empty()) {
		specific_option->setChecked(true);
	} else if (const auto random = parse_random_item_id(entry.second)) {
		random_option->setChecked(true);
		if (const int index = random_class->findData(QString(QChar(random->item_class))); index != -1) {
			random_class->setCurrentIndex(index);
		}
		if (const int index = random_level->findData(random->level); index != -1) {
			random_level->setCurrentIndex(index);
		}
	} else {
		specific_option->setChecked(true);
		for (int row = 0; row < item_filter->rowCount(); ++row) {
			const auto source = item_filter->mapToSource(item_filter->index(row, 0));
			if (items_slk.index_to_row.at(source.row()) == entry.second) {
				items_view->setCurrentIndex(item_filter->index(row, 0));
				break;
			}
		}
	}
	update_enabled_state();

	QObject::connect(search, &QLineEdit::textChanged, item_filter, &ItemListFilter::setFilterFixedString);
	QObject::connect(random_option, &QRadioButton::toggled, &dialog, update_enabled_state);
	QObject::connect(specific_option, &QRadioButton::toggled, &dialog, update_enabled_state);
	QObject::connect(buttons, &QDialogButtonBox::rejected, &dialog, &QDialog::reject);
	QObject::connect(buttons, &QDialogButtonBox::accepted, &dialog, [&]() {
		entry.first = chance->value();

		if (random_option->isChecked()) {
			entry.second = make_random_item_id(
				{ .item_class = random_class->currentData().toString().toStdString()[0], .level = random_level->currentData().toInt() }
			);
			dialog.accept();
			return;
		}

		if (!items_view->currentIndex().isValid()) {
			return;
		}

		const auto source = item_filter->mapToSource(items_view->currentIndex());
		entry.second = items_slk.index_to_row.at(source.row());
		dialog.accept();
	});

	return dialog.exec() == QDialog::Accepted;
}
