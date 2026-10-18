---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/object_editor/object_editor.h"
ref: HEAD
---

#pragma once

#include <QMainWindow>

#include "ui_object_editor.h"

#include "DockManager.h"
#include "DockAreaWidget.h"
#include <QCloseEvent>
#include <QElapsedTimer>
#include <QKeyEvent>
#include <QSortFilterProxyModel>
#include <QTreeView>
#include <QTableView>

#include "global_search.h"
#include "nlohmann/json.hpp"

#include <vector>
#include <string>
#include <memory>
#include <array>

import BaseTreeModel;
import AbilityTreeModel;
import DoodadTreeModel;
import BuffTreeModel;
import DestructibleTreeModel;
import UnitTreeModel;
import UpgradeTreeModel;
import ItemTreeModel;
import TableModel;
import QIconResource;

class ObjectEditor : public QMainWindow {
	Q_OBJECT

public:
	explicit ObjectEditor(QWidget* parent = nullptr);

	enum class Category {
		unit,
		item,
		doodad,
		destructible,
		ability,
		upgrade,
		buff
	};

	void select_id(Category category, const std::string& id);
	void open_by_id(TableModel* table, const std::string& id, const QString& name, QIcon icon);

private:
	Ui::ObjectEditor ui;

	ads::CDockManager* dock_manager;
	ads::CDockAreaWidget* dock_area = nullptr;
	ads::CDockAreaWidget* explorer_area = nullptr;
	ads::CDockWidget* details_dock = nullptr;
	std::string current_details_id;
	QTreeView* current_explorer_view = nullptr;
	QTableView* current_details_view = nullptr;
	Category current_category = Category::unit;
	std::string current_detail_key;
	int current_detail_level = -1;
	int current_details_scroll_y = 0;
	QString current_field_search;
	QString current_field_category;
	bool current_modified_only = false;
	bool current_core_only = false;
	std::array<QString, 7> browser_searches;
	std::array<bool, 7> browser_custom_only {};
	struct ObjectHistoryEntry {
		Category category;
		std::string id;
		QString name;
	};
	std::vector<ObjectHistoryEntry> object_history;
	std::vector<ObjectHistoryEntry> object_bookmarks;
	int object_history_index = -1;
	bool history_navigation = false;

	QTreeView* unit_explorer = new QTreeView;
	QTreeView* doodad_explorer = new QTreeView;
	QTreeView* item_explorer = new QTreeView;
	QTreeView* destructible_explorer = new QTreeView;
	QTreeView* ability_explorer = new QTreeView;
	QTreeView* upgrade_explorer = new QTreeView;
	QTreeView* buff_explorer = new QTreeView;
	
	UnitTreeModel* unitTreeModel;
	DoodadTreeModel* doodadTreeModel;
	DestructibleTreeModel* destructibleTreeModel;
	AbilityTreeModel* abilityTreeModel;
	ItemTreeModel* itemTreeModel;
	BuffTreeModel* buffTreeModel;
	UpgradeTreeModel* upgradeTreeModel;

	BaseFilter* unitTreeFilter;
	BaseFilter* doodadTreeFilter;
	BaseFilter* destructibleTreeFilter;
	BaseFilter* abilityTreeFilter;
	BaseFilter* itemTreeFilter;
	BaseFilter* buffTreeFilter;
	BaseFilter* upgradeTreeFilter;

	std::shared_ptr<QIconResource> custom_unit_icon;
	std::shared_ptr<QIconResource> custom_item_icon;
	std::shared_ptr<QIconResource> custom_doodad_icon;
	std::shared_ptr<QIconResource> custom_destructible_icon;
	std::shared_ptr<QIconResource> custom_ability_icon;
	std::shared_ptr<QIconResource> custom_buff_icon;
	std::shared_ptr<QIconResource> custom_upgrade_icon;

	nlohmann::json ability_insights;

	void itemClicked(QTreeView* view, const QSortFilterProxyModel* model, TableModel* table, const QModelIndex& index);
	void addTypeTreeView(BaseTreeModel* treeModel, BaseFilter*& filter, TableModel* table, QTreeView* view, QIcon icon, QString name, Category category);
	void reset_details_panel();
	void refresh_details_focus_visuals() const;
	bool restore_tree_state(const QString& key, QTreeView* view) const;
	void save_tree_state(const QString& key, const QTreeView* view) const;
	void push_object_history(Category category, const std::string& id, const QString& name);
	void navigate_object_history(int delta);
	bool is_object_bookmarked(Category category, const std::string& id) const;
	void toggle_object_bookmark(Category category, const std::string& id, const QString& name);
	TableModel* table_for_category(Category category) const;
	BaseTreeModel* tree_model_for_category(Category category) const;
	BaseFilter* filter_for_category(Category category) const;
	QTreeView* view_for_category(Category category) const;
	QString category_item_label(Category category) const;
	void copy_object_entry_to_clipboard(Category category, const std::string& id) const;
	void copy_selected_object_entry(QTreeView* view, BaseFilter* filter, Category category);
	void duplicate_object_entry(
		QTreeView* view,
		BaseFilter* filter,
		BaseTreeModel* tree_model,
		TableModel* table,
		const QString& name,
		const std::string& source_id
	);
	void begin_rename_selected_object();
	void paste_copied_object();
	void delete_selected_object_entries(QTreeView* view, BaseFilter* filter, TableModel* table, const QString& name);
protected:
	void closeEvent(QCloseEvent* event) override;
	bool eventFilter(QObject* watched, QEvent* event) override;
	bool focusNextPrevChild(bool next) override;
	void keyPressEvent(QKeyEvent* event) override;

private:
	QElapsedTimer double_shift_timer;
};
