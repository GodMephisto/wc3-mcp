---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/asset_manager/asset_manager.h"
ref: HEAD
---

#pragma once

#include <vector>

#include <QDialog>
#include <QHeaderView>
#include <QLabel>
#include <QLineEdit>
#include <QComboBox>
#include <QPushButton>
#include <QToolButton>
#include <QTreeView>
#include <QStandardItemModel>
#include <QSortFilterProxyModel>
#include <QStringList>

// Shared item data roles / column indices for the Asset Manager model.
namespace asset {
	constexpr int IsUnusedRole = Qt::UserRole;       // bool, on file items
	constexpr int ObjectIdRole = Qt::UserRole + 1;   // QString, on child (reference) items
	constexpr int CategoryRole = Qt::UserRole + 2;   // int, on child items (-1 = not an object)
	constexpr int SortKeyRole = Qt::UserRole + 3;    // int, on usage-count items (sort order)
	constexpr int IsOverrideRole = Qt::UserRole + 4; // bool, on file items
	constexpr int SizeRole = Qt::UserRole + 5;       // qulonglong bytes, on file items

	constexpr int FileColumn = 0;
	constexpr int TypeColumn = 1;
	constexpr int UsageColumn = 2;
} // namespace asset

class AssetFilterModel : public QSortFilterProxyModel {
	Q_OBJECT
  public:
	using QSortFilterProxyModel::QSortFilterProxyModel;

	void set_only_unused(bool value);

  protected:
	bool lessThan(const QModelIndex& left, const QModelIndex& right) const override;
	bool filterAcceptsRow(int source_row, const QModelIndex& source_parent) const override;

  private:
	bool only_unused = false;
};

class AssetManager : public QDialog {
	Q_OBJECT
  public:
	explicit AssetManager(QWidget* parent = nullptr);

  private:
	void refresh() const;
	void update_status() const;
	void update_selection() const;
	void select_all_unused();
	void delete_selected();
	void show_context_menu(const QPoint& pos);
	void open_in_editor(const QModelIndex& proxy_index) const;
	void remove_object_references(const std::string& id);

	// Moves the given map-relative files to the OS recycle bin and removes their rows. Returns the
	// list of files that could not be moved (for error reporting).
	QStringList trash_files(const std::vector<class QStandardItem*>& file_items);

	QLineEdit* search_edit;
	QComboBox* filter_combo;
	QTreeView* tree_view;
	QLabel* stats_label;
	QLabel* selection_label;
	QPushButton* select_unused_button;
	QPushButton* delete_button;
	QStandardItemModel* model;
	AssetFilterModel* filter_model;
};
