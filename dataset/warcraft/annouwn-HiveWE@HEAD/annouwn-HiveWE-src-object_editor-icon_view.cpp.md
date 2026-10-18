---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/object_editor/icon_view.cpp"
ref: HEAD
---

#include "icon_view.h"

#include <QVBoxLayout>
#include <QPushButton>
#include <QListView>
#include <QFile>
#include <QJsonDocument>
#include <QJsonArray>
#include <QJsonObject>
#include <QLabel>
#include <QFileDialog>

import std;
import Hierarchy;
import Globals;
import ResourceManager;

namespace fs = std::filesystem;

std::unordered_map<std::string, std::shared_ptr<QIconResource>> icon_cache;

std::string normalized_icon_path(fs::path path) {
	return path.generic_string();
}

bool relative_path_stays_inside_root(const fs::path& path) {
	return !path.empty() && *path.begin() != "..";
}

std::optional<std::string> icon_path_relative_to_root(const fs::path& path, const fs::path& root) {
	if (root.empty()) {
		return {};
	}

	std::error_code ec;
	const fs::path normalized_root = fs::weakly_canonical(root, ec);
	if (ec) {
		return {};
	}

	const fs::path relative = path.lexically_relative(normalized_root);
	if (!relative_path_stays_inside_root(relative)) {
		return {};
	}

	return normalized_icon_path(relative);
}

std::optional<std::string> icon_path_for_selected_file(const fs::path& path) {
	std::error_code ec;
	const fs::path absolute_path = fs::weakly_canonical(path, ec);
	const fs::path normalized_path = ec ? fs::absolute(path) : absolute_path;

	if (auto relative_path = icon_path_relative_to_root(normalized_path, hierarchy.map_directory)) {
		return relative_path;
	}

	if (auto relative_path = icon_path_relative_to_root(normalized_path, hierarchy.root_directory)) {
		return relative_path;
	}

	if (hierarchy.file_exists(normalized_path)) {
		return normalized_icon_path(normalized_path);
	}

	return {};
}

IconModel::IconModel(QObject* parent) : QAbstractListModel(parent) {
	//std::unordered_map<std::string, QString> icons_map;

	//for (const auto& [key, values] : units_slk.base_data) {
	//	if (!values.contains("art")) {
	//		continue;
	//	}

	//	//std::string art_path = values.at("art");
	//	std::string art_path = to_lowercase_copy(values.at("art"));
	//	art_path = fs::path(art_path).replace_extension("").string();
	//	art_path = split(art_path, ',').front();

	//	if (icons_map.contains(art_path)) {
	//		QString& data = icons_map.at(art_path);
	//		QString new_data = QString::fromStdString(values.at("name"));
	//		if (!data.contains(new_data)) {
	//			data += ", " + new_data;
	//		}
	//	} else {
	//		icons_map.emplace(art_path, QString::fromStdString(values.at("name")));
	//	}
	//}

	//for (const auto& [key, values] : items_slk.base_data) {
	//	if (!values.contains("art")) {
	//		continue;
	//	}

	//	//std::string art_path = values.at("art");
	//	std::string art_path = to_lowercase_copy(values.at("art"));
	//	art_path = fs::path(art_path).replace_extension("").string();
	//	art_path = split(art_path, ',').front();

	//	if (icons_map.contains(art_path)) {
	//		QString& data = icons_map.at(art_path);
	//		QString new_data = QString::fromStdString(values.at("name"));
	//		if (!data.contains(new_data)) {
	//			data += ", " + new_data;
	//		}
	//	} else {
	//		icons_map.emplace(art_path, QString::fromStdString(values.at("name")));
	//	}
	//}

	//for (const auto& [key, values] : buff_slk.base_data) {
	//	if (!values.contains("buffart")) {
	//		continue;
	//	}

	//	QString tags;
	//	if (!values.contains("bufftip") && !values.contains("editorname")) {
	//		std::print("Missing buff name: {}\n", key);
	//		continue;
	//	} else if (!values.contains("bufftip")) {
	//		tags = QString::fromStdString(values.at("editorname"));
	//	} else {
	//		tags = QString::fromStdString(values.at("bufftip"));
	//	}
	//	
	//	//std::string art_path = values.at("buffart");
	//	std::string art_path = to_lowercase_copy(values.at("buffart"));
	//	art_path = fs::path(art_path).replace_extension("").string();
	//	art_path = split(art_path, ',').front();

	//	if (icons_map.contains(art_path)) {
	//		QString& data = icons_map.at(art_path);

	//		if (!data.contains(tags)) {
	//			data += ", " + tags;
	//		}
	//	} else {
	//		icons_map.emplace(art_path, tags);
	//	}
	//}

	//for (const auto& [key, values] : abilities_slk.base_data) {
	//	if (!values.contains("art")) {
	//		continue;
	//	}

	//	QString tags;
	//	if (values.contains("name")) {
	//		tags = QString::fromStdString(values.at("name"));
	//	} else if (values.contains("tip")) {
	//		tags = QString::fromStdString(values.at("tip"));
	//	} else {
	//		std::print("Missing ability name: {}\n", key);
	//		continue;
	//	}
	//	std::string art_path = to_lowercase_copy(values.at("art"));
	//	art_path = fs::path(art_path).replace_extension("").string();

	//	//std::string art_path = values.at("art");
	//	art_path = split(art_path, ',').front();

	//	if (icons_map.contains(art_path)) {
	//		QString& data = icons_map.at(art_path);
	//		if (!data.contains(tags)) {
	//			data += ", " + tags;
	//		}
	//	} else {
	//		icons_map.emplace(art_path, tags);
	//	}
	//}

	//for (const auto& i : fs::directory_iterator("C:/Users/User/Desktop/Warcraft/MPQContent/1.32.x/_hd.w3mod/replaceabletextures/commandbuttons")) {
	//	fs::path path = i.path().lexically_relative("C:/Users/User/Desktop/Warcraft/MPQContent/1.32.x/_hd.w3mod");
	//	path.replace_extension("");
	//	if (icons_map.contains(path.string())) {
	//		continue;
	//	}

	//	icons_map.emplace(path.string(), QString::fromStdString(path.stem().string()).remove(0, 3));
	//}

	//QJsonDocument json;
	//QJsonArray array;
	//for (const auto& [key, value] : icons_map) {
	//	icons.push_back({ key, value });
	//	QJsonObject object;
	//	object["src"] = QString::fromStdString(key);
	//	
	//	QJsonArray raaaa;
	//	auto parts = value.split(", ");
	//	for (const auto& i : parts) {
	//		raaaa.append(i);
	//	}
	//	object["tags"] = raaaa;
	//
	//	array.append(object);
	//}
	//json.setArray(array);
	//std::ofstream file("C:/Users/User/stack/Projects/HiveWE/HiveWE/data/warcraft/icon_tags.json");
	//std::string output = json.toJson().toStdString();
	//file.write(output.data(), output.size());
	//file.close();

	QFile file(fs::path("data/warcraft/icon_tags.json"));
	if (!file.open(QIODevice::ReadOnly)) {
		std::print("Error opening icon_tags.json");
		return;
	}

	QJsonParseError error;
	QJsonDocument json = QJsonDocument::fromJson(file.readAll(), &error);
	file.close();

	if (json.isNull()) {
		std::print("Error parsing icon_tags.json: {}", error.errorString().toStdString());
	}

	for (const auto& i : json.array()) {
		QJsonObject object = i.toObject();
		
		QJsonArray tags = object["tags"].toArray();

		QString text;
		if (tags.size()) {
			text = tags.at(0).toString();
			for (int i = 1; i < tags.size(); i++) {
				text += ", " + tags.at(i).toString();
			}
		}

		std::string string_path = object["src"].toString().toStdString() + ".dds";
		if (!hierarchy.file_exists(string_path)) {
			continue;
		}
		icons.emplace_back(string_path, text);
	}
}

void IconModel::addIconsFromFolder(const fs::path& folder) {
	std::error_code ec;
	if (!fs::exists(folder, ec) || !fs::is_directory(folder, ec)) {
		return;
	}

	std::unordered_set<std::string> existing;
	std::vector<std::pair<std::string, QString>> discovered;
	for (const auto& entry : fs::recursive_directory_iterator(folder, fs::directory_options::skip_permission_denied, ec)) {
		std::error_code file_ec;
		if (ec || !entry.is_regular_file(file_ec) || file_ec) {
			continue;
		}

		const fs::path path = entry.path();
		std::string extension = path.extension().string();
		std::ranges::transform(extension, extension.begin(), [](unsigned char c) {
			return static_cast<char>(std::tolower(c));
		});
		if (extension != ".blp" && extension != ".dds") {
			continue;
		}

		const auto icon_path = icon_path_for_selected_file(path);
		if (!icon_path || existing.contains(*icon_path) || !hierarchy.file_exists(*icon_path)) {
			continue;
		}

		existing.insert(*icon_path);
		discovered.emplace_back(*icon_path, QString::fromStdString(path.stem().string() + " (" + *icon_path + ")"));
	}

	beginResetModel();
	icons = std::move(discovered);
	endResetModel();
}

QVariant IconModel::data(const QModelIndex& index, int role) const {
	switch (role) {
		case Qt::DecorationRole: {
			std::string string_path = icons[index.row()].first;

			if (icon_cache.contains(string_path)) {
				return icon_cache.at(string_path)->icon;
			} else if (auto icon = resource_manager.load<QIconResource>(string_path)) {
				icon_cache.emplace(string_path, *icon);
				return icon_cache.at(string_path)->icon;
			}
			return {};
		}
		case Qt::ToolTipRole:
			return icons[index.row()].second;
		case Qt::EditRole:
			return QString::fromStdString(icons[index.row()].first);
	}

	return {};
}

IconView::IconView(QWidget* parent) : QWidget(parent) {
	model = new IconModel;

	filter->setSourceModel(model);
	filter->setFilterCaseSensitivity(Qt::CaseInsensitive);
	filter->setFilterRole(Qt::ItemDataRole::ToolTipRole);

	view->setViewMode(QListView::IconMode);
	view->setIconSize(QSize(64, 64));
	view->setResizeMode(QListView::ResizeMode::Adjust);
	view->setUniformItemSizes(true);
	view->setWordWrap(true);
	view->setWrapping(true);
	view->setModel(filter);

	QVBoxLayout* layout = new QVBoxLayout;
	layout->setContentsMargins(0, 0, 0, 0);
	//layout->addWidget(type);
	layout->addWidget(search);

	QHBoxLayout* folderLayout = new QHBoxLayout;
	folderLayout->addWidget(folderPath);
	folderLayout->addWidget(browseFolder);
	layout->addLayout(folderLayout);

	layout->addWidget(view);

	QHBoxLayout* hlayout = new QHBoxLayout;
	hlayout->addWidget(new QLabel("Path"));
	hlayout->addWidget(finalPath);
	//layout->addWidget(finalPath);
	layout->addLayout(hlayout);
	setLayout(layout);

	type->addItem("Units");
	type->addItem("Items");
	type->addItem("Abilities");
	type->addItem("Upgrades");
	type->addItem("Buffs");
	search->setPlaceholderText("Search Icons");
	folderPath->setPlaceholderText("Scan folder for .blp/.dds icons");

	//connect(type, &QComboBox::currentTextChanged, filter, &QSortFilterProxyModel::setFilterFixedString);
	connect(search, &QLineEdit::textEdited, filter, &QSortFilterProxyModel::setFilterFixedString);
	connect(browseFolder, &QPushButton::clicked, this, [this]() {
		const QString start = folderPath->text().isEmpty()
			? QString::fromStdString(hierarchy.map_directory.empty() ? fs::current_path().string() : hierarchy.map_directory.string())
			: folderPath->text();
		const QString selected = QFileDialog::getExistingDirectory(this, "Select Icon Folder", start);
		if (selected.isEmpty()) {
			return;
		}

		folderPath->setText(selected);
		model->addIconsFromFolder(selected.toStdString());
		filter->invalidate();
	});

	connect(view->selectionModel(), &QItemSelectionModel::selectionChanged, [&]() {
		if (!view->currentIndex().isValid()) {
			finalPath->clear();
			return;
		}
		
		finalPath->setText(filter->data(view->currentIndex(), Qt::EditRole).toString());
	});
}

QString IconView::currentIconPath() {
	return finalPath->text();
}

void IconView::setCurrentIconPath(QString path) {
	finalPath->setText(path);
}
