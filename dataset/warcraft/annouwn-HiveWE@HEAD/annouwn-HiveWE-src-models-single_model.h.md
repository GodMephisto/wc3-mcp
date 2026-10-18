---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/models/single_model.h"
ref: HEAD
---

#pragma once

#include <QAbstractProxyModel>
#include <QIdentityProxyModel>
#include <QHeaderView>
#include <QSortFilterProxyModel>
#include <QStyledItemDelegate>

#include <string>
#include <string_view>
#include <vector>

import TableModel;
import SLK;

class SingleModel : public QAbstractProxyModel {
	Q_OBJECT
		
public:
	enum HeaderRole {
		DisplayNameRole = Qt::UserRole + 1,
		CategoryRole,
		MetaRole,
		ModifiedRole,
		CategoryStartRole,
		CommonRole,
	};

	QModelIndex mapFromSource(const QModelIndex& sourceIndex) const override;
	QModelIndex mapToSource(const QModelIndex& proxyIndex) const override;
	
	QVariant data(const QModelIndex& index, int role = Qt::DisplayRole) const override;
	QVariant headerData(int section, Qt::Orientation orientation, int role) const override;

	bool setData(const QModelIndex& index, const QVariant& value, int role = Qt::EditRole) override;

	int rowCount(const QModelIndex& parent = QModelIndex()) const override;
	int columnCount(const QModelIndex& parent = QModelIndex()) const override;

	QModelIndex index(int row, int column, const QModelIndex& parent = QModelIndex()) const override;
	QModelIndex parent(const QModelIndex& child) const override;

	void setSourceModel(QAbstractItemModel* sourceModel) override;

	explicit SingleModel(TableModel* table, QObject* parent = nullptr);
	void setID(const std::string id);
	std::string getID() const;

	struct Mapping {
		std::string key;
		std::string field;
		int level;
	};

	const std::vector<Mapping>& getMapping() const {
		return id_mapping;
	}

	int find_mapping_row(const std::string& key, int level) const;
	QString category_label(int row) const;
	QString display_label(int row) const;
	QString meta_label(int row) const;
	bool is_modified_row(int row) const;
	bool starts_new_category(int row) const;
	bool is_common_row(int row) const;

	slk::SLK* meta_slk;
private:
	void buildMapping();

	slk::SLK* slk;

	std::string id = "hpea";
	std::vector<Mapping> id_mapping;

	void sourceDataChanged(const QModelIndex& topLeft, const QModelIndex& bottomRight, const QVector<int>& roles);

};

class SingleModelFilter : public QSortFilterProxyModel {
	Q_OBJECT

	QString field_search;
	QString category_filter;
	bool modified_only = false;
	bool core_only = false;

	[[nodiscard]] const SingleModel* single_model() const;
	[[nodiscard]] bool filterAcceptsRow(int source_row, const QModelIndex& source_parent) const override;

  public:
	using QSortFilterProxyModel::QSortFilterProxyModel;

	QVariant headerData(int section, Qt::Orientation orientation, int role = Qt::DisplayRole) const override;
	void set_field_search(const QString& search);
	void set_category_filter(const QString& category);
	void set_modified_only(bool enabled);
	void set_core_only(bool enabled);
};

// Provides row headers that have alternate colors
class AlterHeader : public QHeaderView {
	Q_OBJECT

public:
	using QHeaderView::QHeaderView;
protected:
	void paintSection(QPainter* painter, const QRect& rect, int logicalIndex) const;
	QSize sectionSizeFromContents(int logicalIndex) const override;
};


class TableDelegate : public QStyledItemDelegate {
	Q_OBJECT

	SingleModel* single_model;

public:
	TableDelegate(SingleModel* single_model, QWidget* parent = nullptr);
	void paint(QPainter* painter, const QStyleOptionViewItem& option, const QModelIndex& index) const override;

	QWidget* createEditor(QWidget* parent, const QStyleOptionViewItem& option, const QModelIndex& index) const override;

	void setEditorData(QWidget* editor, const QModelIndex& index) const override;
	void setModelData(QWidget* editor, QAbstractItemModel* model, const QModelIndex& index) const override;

	void updateEditorGeometry(QWidget* editor, const QStyleOptionViewItem& option, const QModelIndex& index) const override;

	QWidget* create_list_editor(QWidget* parent) const;
	QWidget* create_model_editor(QWidget* parent) const;
	QWidget* create_hotkey_editor(QWidget* parent) const;
	QWidget* create_target_list_editor(QWidget* parent) const;
	QWidget* create_flags_editor(QWidget* parent, std::string_view type) const;
	QWidget* create_upgrade_list_editor(QWidget* parent) const;
	QWidget* create_unit_list_editor(QWidget* parent) const;
	QWidget* create_ability_list_editor(QWidget* parent) const;
	QWidget* create_buff_list_editor(QWidget* parent) const;
	QWidget* create_icon_editor(QWidget* parent) const;

	[[nodiscard]] QSize sizeHint(const QStyleOptionViewItem &option, const QModelIndex &index) const override;
};
