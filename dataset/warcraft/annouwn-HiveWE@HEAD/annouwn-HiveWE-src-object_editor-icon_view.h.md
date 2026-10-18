---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/object_editor/icon_view.h"
ref: HEAD
---

#pragma once

#include <unordered_map>
#include <memory>
#include <filesystem>

#include <QDialog>
#include <QComboBox>
#include <QLineEdit>
#include <QListView>
#include <QPushButton>
#include <QAbstractListModel>
#include <QSortFilterProxyModel>

import QIconResource;

class IconModel : public QAbstractListModel {
	int rowCount(const QModelIndex& parent = QModelIndex()) const override {
		return static_cast<int>(icons.size());
	}

public:
	explicit IconModel(QObject* parent = nullptr);

	QVariant data(const QModelIndex& index, int role = Qt::DisplayRole) const override;

	std::vector<std::pair<std::string, QString>> icons;
	void addIconsFromFolder(const std::filesystem::path& folder);
};

class IconView : public QWidget {
	Q_OBJECT

	QComboBox* type = new QComboBox;
	QLineEdit* search = new QLineEdit;
	QLineEdit* folderPath = new QLineEdit;
	QPushButton* browseFolder = new QPushButton("Folder...");
	QListView* view = new QListView;
	QLineEdit* finalPath = new QLineEdit;

	QSortFilterProxyModel* filter = new QSortFilterProxyModel;
	IconModel* model;

public:
	IconView(QWidget* parent = nullptr);

	QString currentIconPath();
	void setCurrentIconPath(QString path);
};
