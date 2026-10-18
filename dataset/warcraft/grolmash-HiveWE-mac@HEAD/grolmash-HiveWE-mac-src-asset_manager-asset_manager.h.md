---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/asset_manager/asset_manager.h"
ref: HEAD
---

#pragma once

#include <QDialog>
#include <QHeaderView>
#include <QLabel>
#include <QLineEdit>
#include <QToolButton>
#include <QTreeView>
#include <QStandardItemModel>
#include <QSortFilterProxyModel>

class AssetFilterModel : public QSortFilterProxyModel {
	Q_OBJECT
  public:
	using QSortFilterProxyModel::QSortFilterProxyModel;

protected:
	bool lessThan(const QModelIndex& left, const QModelIndex& right) const override;
};

class AssetManager : public QDialog {
	Q_OBJECT
  public:
	explicit AssetManager(QWidget* parent = nullptr);

  private:
	void refresh() const;
	void update_status() const;
	void show_context_menu(const QPoint& pos);
	void open_in_editor(const QModelIndex& proxy_index) const;
	void remove_object_references(const std::string& id);

	QLineEdit* search_edit;
	QTreeView* tree_view;
	QLabel* status_label;
	QStandardItemModel* model;
	AssetFilterModel* filter_model;
};
