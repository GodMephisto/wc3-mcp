---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/tooltip_editor/tooltip_editor.h"
ref: HEAD
---

#pragma once

#include <QMainWindow>
#include <QListView>
#include <QTextEdit>
#include <QTextBrowser>
#include <QTabWidget>
#include <QSortFilterProxyModel>
#include <QLineEdit>
#include <QSpinBox>
#include <QLabel>
#include <QAbstractTableModel>
#include <QWidget>
#include <QColor>
#include <string>
#include <vector>

import SLK;
import UnitListModel;
import ItemListModel;
import AbilityListModel;
import TableModel;

class TemplateManager;

class TooltipEditor : public QMainWindow {
	Q_OBJECT

	struct TabData {
		slk::SLK* slk = nullptr;
		QAbstractTableModel* table = nullptr;    // Used for TRIGSTR_-resolved reads
		QSortFilterProxyModel* filter_model = nullptr;
		QListView* list_view = nullptr;
		QLineEdit* search_bar = nullptr;
		QLabel* tip_label = nullptr;
		QLabel* utip_label = nullptr;
		QTextEdit* tip_edit = nullptr;      // First tooltip field
		QTextEdit* ubertip_edit = nullptr;  // Second tooltip field
		QTextBrowser* preview = nullptr;
		QWidget* level_row = nullptr;            // Shown only for abilities
		QSpinBox* level_spin = nullptr;
		QWidget* fav_container = nullptr;        // Favorite template buttons next to utip label
		std::string current_id;
		int current_level = 1;
		bool leveled = false;
		std::string tip_field_name;    // Base SLK column name (level suffix added if leveled)
		std::string utip_field_name;   // Base SLK column name (level suffix added if leveled)
	};

	QTabWidget* tab_widget;
	TabData unit_tab;
	TabData item_tab;
	TabData ability_tab;

	QTextEdit* active_edit = nullptr;
	TemplateManager* template_mgr = nullptr;
	std::vector<QColor> custom_colors;
	QWidget* custom_color_widget = nullptr;  // Toolbar section for custom color buttons

	QWidget* build_tab(TabData* tab, slk::SLK& slk, QAbstractTableModel* table,
	                   QAbstractItemModel* list_model, bool leveled,
	                   const char* tip_label_text, const char* utip_label_text);
	void load_entry(TabData& tab, const std::string& id);
	void load_level(TabData& tab);
	void save_current(TabData& tab);
	void update_preview(TabData& tab);
	void insert_color_code(const QString& code);  // wraps selection or inserts at cursor
	void add_custom_color(const QColor& color);
	void rebuild_custom_colors();
	void rebuild_favorite_buttons();

	std::string resolve_tip_field(const TabData& tab) const;
	std::string resolve_utip_field(const TabData& tab) const;
	QString read_field(const TabData& tab, const std::string& field) const;

	static std::string meta_field_name(const slk::SLK& meta_slk, const std::string& code);
	static QString wc3_to_html(const QString& wc3_text);

public:
	explicit TooltipEditor(QWidget* parent = nullptr);

public slots:
	void apply_template(const QString& tip_text, const QString& ubertip_text);
};
