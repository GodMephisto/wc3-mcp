---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/object_editor/object_editor.cpp"
ref: HEAD
---

#include "object_editor.h"

#include <QTableView>
#include <QLineEdit>
#include <QScrollArea>
#include <QPalette>

#include "DockAreaTitleBar.h"
#include "DockAreaTabBar.h"
#include <QToolBar>
#include <QDialogButtonBox>
#include <QSortFilterProxyModel>
#include <QAbstractProxyModel>
#include <QApplication>
#include <QClipboard>
#include <QPushButton>
#include <QItemSelection>
#include <QTimer>
#include <QLabel>
#include <QShortcut>
#include <QDialog>
#include <QToolButton>
#include <QScrollBar>
#include <QMenu>
#include <QMessageBox>
#include <QMimeData>
#include <QJsonDocument>
#include <QJsonObject>
#include <QSettings>
#include <QSet>
#include <QButtonGroup>
#include <QComboBox>
#include <QGridLayout>
#include <QStackedWidget>
#include <QFrame>
#include <QPlainTextEdit>
#include <QTextEdit>
#include <QAbstractSpinBox>
#include <QStyledItemDelegate>
#include <QStyle>

import std;
import UnitSelector;
import MapGlobal;
import Globals;
import ResourceManager;
import SlkConversions;
import "single_model.h";

namespace {
const BaseTreeItem* tree_item_from_index(const QModelIndex& index) {
	if (!index.isValid()) {
		return nullptr;
	}

	const QModelIndex source_index = [&]() -> QModelIndex {
		if (const auto* proxy = qobject_cast<const QAbstractProxyModel*>(index.model())) {
			return proxy->mapToSource(index);
		}
		return index;
	}();

	if (!source_index.isValid()) {
		return nullptr;
	}

	return static_cast<const BaseTreeItem*>(source_index.internalPointer());
}

QString tree_item_path(const QModelIndex& index) {
	QStringList parts;
	const auto* item = tree_item_from_index(index);
	while (item && item->parent) {
		if (item->baseCategory || item->subCategory) {
			parts.prepend(QString::fromStdString(item->label));
		}
		item = item->parent;
	}
	return parts.join('/');
}

void collect_expanded_paths(const QTreeView* view, const QModelIndex& parent, QStringList& paths) {
	const auto* model = view->model();
	for (int row = 0; row < model->rowCount(parent); ++row) {
		const QModelIndex child = model->index(row, 0, parent);
		if (!child.isValid()) {
			continue;
		}

		const auto* item = tree_item_from_index(child);
		if (!item || !(item->baseCategory || item->subCategory)) {
			continue;
		}

		if (view->isExpanded(child)) {
			paths.push_back(tree_item_path(child));
		}

		collect_expanded_paths(view, child, paths);
	}
}

void expand_saved_paths(QTreeView* view, const QModelIndex& parent, const QSet<QString>& paths) {
	const auto* model = view->model();
	for (int row = 0; row < model->rowCount(parent); ++row) {
		const QModelIndex child = model->index(row, 0, parent);
		if (!child.isValid()) {
			continue;
		}

		const auto* item = tree_item_from_index(child);
		if (!item || !(item->baseCategory || item->subCategory)) {
			continue;
		}

		const QString path = tree_item_path(child);
		if (paths.contains(path)) {
			view->expand(child);
		}

		expand_saved_paths(view, child, paths);
	}
}

bool is_focus_within(QWidget* focus_widget, QWidget* container) {
	return focus_widget && container && (focus_widget == container || container->isAncestorOf(focus_widget));
}

	QString category_key(const ObjectEditor::Category category) {
	switch (category) {
		case ObjectEditor::Category::unit:
			return "units";
		case ObjectEditor::Category::item:
			return "items";
		case ObjectEditor::Category::doodad:
			return "doodads";
		case ObjectEditor::Category::destructible:
			return "destructibles";
		case ObjectEditor::Category::ability:
			return "abilities";
		case ObjectEditor::Category::upgrade:
			return "upgrades";
		case ObjectEditor::Category::buff:
			return "buffs";
	}

	return "units";
}

QString category_label(const ObjectEditor::Category category) {
	switch (category) {
		case ObjectEditor::Category::unit:
			return "Units";
		case ObjectEditor::Category::item:
			return "Items";
		case ObjectEditor::Category::doodad:
			return "Doodads";
		case ObjectEditor::Category::destructible:
			return "Destructibles";
		case ObjectEditor::Category::ability:
			return "Abilities";
		case ObjectEditor::Category::upgrade:
			return "Upgrades";
		case ObjectEditor::Category::buff:
			return "Buffs";
	}

	return "Units";
}

QString field_type_group(const QString& type) {
	if (type == "int" || type == "real" || type == "unreal") {
		return "number";
	}

	return type;
}

bool field_types_compatible(const QString& source_type, const QString& target_type) {
	return field_type_group(source_type) == field_type_group(target_type);
}

bool is_sequential_custom_id(const std::string_view id) {
	return id.size() == 4 && std::isalpha(static_cast<unsigned char>(id[0])) && std::isdigit(static_cast<unsigned char>(id[1]))
		&& std::isdigit(static_cast<unsigned char>(id[2])) && std::isdigit(static_cast<unsigned char>(id[3]));
}

std::string next_sequential_custom_id(TableModel* table, const std::optional<std::string_view> source_id = std::nullopt) {
	char prefix = 0;
	if (source_id && is_sequential_custom_id(*source_id)) {
		prefix = static_cast<char>((*source_id)[0]);
	}

	if (!prefix) {
		std::string best_custom_id;
		for (const auto& [id, values] : table->slk->shadow_data) {
			if (!values.contains("oldid") || !is_sequential_custom_id(id)) {
				continue;
			}

			if (best_custom_id.empty() || id < best_custom_id) {
				best_custom_id = id;
			}
		}

		if (!best_custom_id.empty()) {
			prefix = best_custom_id.front();
		}
	}

	if (!prefix && source_id && !source_id->empty()) {
		prefix = static_cast<char>((*source_id)[0]);
	}

	if (!prefix) {
		prefix = 'A';
	}

	std::array<bool, 1000> used {};
	for (const auto& [id, index] : table->slk->row_headers) {
		if (!is_sequential_custom_id(id) || id.front() != prefix) {
			continue;
		}

		used[std::stoi(id.substr(1))] = true;
	}

	for (int number = 0; number < static_cast<int>(used.size()); ++number) {
		if (!used[number]) {
			return std::format("{}{:03d}", prefix, number);
		}
	}

	return {};
}

QString copied_name_with_suffix(QString name) {
	name = name.trimmed();
	if (name.isEmpty()) {
		return name;
	}
	if (!name.endsWith(" Copy", Qt::CaseInsensitive)) {
		name += " Copy";
	}
	return name;
}

bool is_text_input_widget(QWidget* widget) {
	return qobject_cast<QLineEdit*>(widget) || qobject_cast<QTextEdit*>(widget) || qobject_cast<QPlainTextEdit*>(widget)
		|| qobject_cast<QAbstractSpinBox*>(widget) || qobject_cast<QComboBox*>(widget);
}

void copy_selected_field_to_clipboard(QTableView* view) {
	if (!view) {
		return;
	}

	QModelIndex current = view->currentIndex();
	if (!current.isValid()) {
		return;
	}
	current = current.siblingAtColumn(0);

	const auto* filter_model = qobject_cast<const SingleModelFilter*>(view->model());
	if (!filter_model) {
		return;
	}

	const auto* single_model = qobject_cast<const SingleModel*>(filter_model->sourceModel());
	if (!single_model) {
		return;
	}

	const QModelIndex source_index = filter_model->mapToSource(current);
	if (!source_index.isValid()) {
		return;
	}

	const auto& mapping = single_model->getMapping();
	const QString field_type = QString::fromStdString(single_model->meta_slk->data("type", mapping[source_index.row()].key));
	QJsonObject payload;
	payload["type"] = field_type;
	payload["value"] = current.data(Qt::EditRole).toString();
	payload["field"] = QString::fromStdString(mapping[source_index.row()].key);

	auto* mime = new QMimeData;
	mime->setData("application/x-hivewe-object-field", QJsonDocument(payload).toJson(QJsonDocument::Compact));
	mime->setText(current.data(Qt::DisplayRole).toString());
	QApplication::clipboard()->setMimeData(mime);
}

void paste_selected_field_from_clipboard(QTableView* view) {
	if (!view) {
		return;
	}

	QModelIndex current = view->currentIndex();
	if (!current.isValid()) {
		return;
	}
	current = current.siblingAtColumn(0);

	const auto* filter_model = qobject_cast<const SingleModelFilter*>(view->model());
	if (!filter_model) {
		return;
	}

	const auto* single_model = qobject_cast<const SingleModel*>(filter_model->sourceModel());
	if (!single_model) {
		return;
	}

	const QModelIndex source_index = filter_model->mapToSource(current);
	if (!source_index.isValid()) {
		return;
	}

	const auto* mime = QApplication::clipboard()->mimeData();
	if (!mime) {
		return;
	}

	QString source_type;
	QString value;
	if (mime->hasFormat("application/x-hivewe-object-field")) {
		const QJsonDocument document = QJsonDocument::fromJson(mime->data("application/x-hivewe-object-field"));
		if (!document.isObject()) {
			return;
		}

		const QJsonObject payload = document.object();
		source_type = payload.value("type").toString();
		value = payload.value("value").toString();
	} else if (mime->hasText()) {
		source_type = "string";
		value = mime->text();
	} else {
		return;
	}

	const auto& mapping = single_model->getMapping();
	const QString target_type = QString::fromStdString(single_model->meta_slk->data("type", mapping[source_index.row()].key));
	if (!field_types_compatible(source_type, target_type)) {
		return;
	}

	view->model()->setData(current, value, Qt::EditRole);
}

class ObjectTreeDelegate : public QStyledItemDelegate {
  public:
	using QStyledItemDelegate::QStyledItemDelegate;

	QSize sizeHint(const QStyleOptionViewItem& option, const QModelIndex& index) const override {
		QSize size = QStyledItemDelegate::sizeHint(option, index);
		size.setHeight(std::max(size.height(), 24));
		return size;
	}

	QWidget* createEditor(QWidget* parent, const QStyleOptionViewItem& option, const QModelIndex& index) const override {
		QWidget* editor = QStyledItemDelegate::createEditor(parent, option, index);
		if (auto* line_edit = qobject_cast<QLineEdit*>(editor)) {
			line_edit->setFrame(true);
			line_edit->setClearButtonEnabled(false);
			line_edit->setAlignment(Qt::AlignLeft | Qt::AlignVCenter);
			line_edit->setTextMargins(4, 0, 4, 0);
		}
		return editor;
	}

	void updateEditorGeometry(QWidget* editor, const QStyleOptionViewItem& option, const QModelIndex&) const override {
		QRect rect = option.rect.adjusted(34, 0, -8, 2);
		if (const auto* item_view = qobject_cast<const QAbstractItemView*>(option.widget)) {
			rect.setRight(item_view->viewport()->width() - 8);
		}
		editor->setGeometry(rect);
	}

	void paint(QPainter* painter, const QStyleOptionViewItem& option, const QModelIndex& index) const override {
		QStyleOptionViewItem opt(option);
		initStyleOption(&opt, index);

		const QModelIndex source_index = [&]() -> QModelIndex {
			if (const auto* proxy = qobject_cast<const QAbstractProxyModel*>(index.model())) {
				return proxy->mapToSource(index);
			}
			return index;
		}();

		const auto* item = source_index.isValid() ? static_cast<const BaseTreeItem*>(source_index.internalPointer()) : nullptr;
		const bool is_folder = item && (item->baseCategory || item->subCategory);
		const bool is_custom = item && !is_folder && index.data(Qt::ForegroundRole).value<QColor>() == QColor("violet");
		const bool selected = opt.state.testFlag(QStyle::State_Selected);

		painter->save();
		if (selected) {
			painter->fillRect(opt.rect, opt.palette.highlight());
		} else if (opt.state.testFlag(QStyle::State_MouseOver)) {
			painter->fillRect(opt.rect, QColor(255, 255, 255, 10));
		}

		const QRect content_rect = opt.rect.adjusted(6, 0, -8, 0);
		QRect icon_rect(content_rect.left(), content_rect.top() + (content_rect.height() - 18) / 2, 18, 18);
		const QIcon icon = index.data(Qt::DecorationRole).value<QIcon>();
		icon.paint(painter, icon_rect);

		QString full_text = opt.text;
		QString main_text = full_text;
		QString id_text;
		if (!is_folder && full_text.endsWith(')')) {
			const int id_start = full_text.lastIndexOf(" (");
			if (id_start > 0) {
				main_text = full_text.first(id_start);
				id_text = full_text.mid(id_start + 1);
			}
		}

		int text_left = icon_rect.right() + 8;
		QRect text_rect = content_rect.adjusted(text_left - content_rect.left(), 0, 0, 0);
		QFont main_font = opt.font;
		if (is_folder && item->baseCategory) {
			main_font.setBold(true);
		}
		painter->setFont(main_font);
		painter->setPen(selected ? opt.palette.highlightedText().color() : opt.palette.text().color());

		if (id_text.isEmpty()) {
			painter->drawText(text_rect, Qt::AlignVCenter | Qt::AlignLeft, main_text);
		} else {
			QFontMetrics fm(main_font);
			const int main_width = fm.horizontalAdvance(main_text);
			const int gap = 6;
			painter->drawText(text_rect, Qt::AlignVCenter | Qt::AlignLeft, main_text);

			QRect id_rect = text_rect.adjusted(main_width + gap, 0, 0, 0);
			QColor id_color = selected ? opt.palette.highlightedText().color().lighter(115) : QColor(152, 158, 168);
			painter->setPen(id_color);
			painter->drawText(id_rect, Qt::AlignVCenter | Qt::AlignLeft, id_text);
		}

		if (is_custom && !selected) {
			QFont badge_font = opt.font;
			badge_font.setPointSize(std::max(7, badge_font.pointSize() - 1));
			painter->setFont(badge_font);
			const QString badge = "Custom";
			QFontMetrics badge_metrics(badge_font);
			const int badge_width = badge_metrics.horizontalAdvance(badge) + 12;
			QRect badge_rect(opt.rect.right() - badge_width - 8, opt.rect.top() + (opt.rect.height() - 16) / 2, badge_width, 16);
			painter->setPen(Qt::NoPen);
			painter->setBrush(QColor(214, 112, 255, 38));
			painter->drawRoundedRect(badge_rect, 8, 8);
			painter->setPen(QColor(224, 160, 255));
			painter->drawText(badge_rect, Qt::AlignCenter, badge);
		}

		painter->restore();
	}
};

std::string primary_name_field(TableModel* table) {
	if (table->slk->column_headers.contains("name")) {
		return "name";
	} else if (table->slk->column_headers.contains("name1")) {
		return "name1";
	} else if (table->slk->column_headers.contains("editorname")) {
		return "editorname";
	} else if (table->slk->column_headers.contains("bufftip")) {
		return "bufftip";
	}

	return {};
}

QString object_display_name(TableModel* table, const std::string& id) {
	const std::string field = primary_name_field(table);
	if (field.empty() || !table->slk->row_headers.contains(id)) {
		return {};
	}

	QString name = table->data(id, field, Qt::DisplayRole).toString();
	if (name.isEmpty() && field == "editorname") {
		name = table->data(id, "bufftip", Qt::DisplayRole).toString();
	}
	return name;
}

std::string base_object_id(TableModel* table, const std::string& id) {
	if (table->slk->shadow_data.contains(id) && table->slk->shadow_data.at(id).contains("oldid")) {
		return table->slk->shadow_data.at(id).at("oldid");
	}

	return {};
}

int modified_field_count(TableModel* table, const std::string& id) {
	if (!table->slk->shadow_data.contains(id)) {
		return 0;
	}

	int count = 0;
	for (const auto& [field, value] : table->slk->shadow_data.at(id)) {
		if (field != "oldid") {
			++count;
		}
	}
	return count;
}

int custom_object_count(TableModel* table) {
	int count = 0;
	for (const auto& [id, values] : table->slk->shadow_data) {
		if (values.contains("oldid")) {
			++count;
		}
	}
	return count;
}

QString object_count_label(TableModel* table) {
	return QString::number(table->rowCount()) + " total - " + QString::number(custom_object_count(table)) + " custom";
}

int visible_object_count(const QAbstractItemModel* model, const QModelIndex& parent = QModelIndex()) {
	int count = 0;
	for (int row = 0; row < model->rowCount(parent); ++row) {
		const QModelIndex child = model->index(row, 0, parent);
		if (!child.isValid()) {
			continue;
		}

		const auto* item = tree_item_from_index(child);
		if (!item) {
			continue;
		}

		if (item->baseCategory || item->subCategory) {
			count += visible_object_count(model, child);
		} else {
			++count;
		}
	}
	return count;
}

QString filtered_object_count_label(TableModel* table, const QAbstractItemModel* model, bool custom_only) {
	QString text = QString::number(visible_object_count(model)) + " shown - " + QString::number(table->rowCount()) + " total";
	if (custom_only) {
		text += " - custom only";
	}
	return text;
}

int details_header_width(const SingleModel* model, const QHeaderView* header) {
	if (!model || !header) {
		return 260;
	}

	QFont title_font = header->font();
	QFont meta_font = title_font;
	meta_font.setPointSize(std::max(7, meta_font.pointSize() - 1));
	QFont category_font = title_font;
	category_font.setBold(true);
	category_font.setPointSize(std::max(7, category_font.pointSize() - 1));

	const QFontMetrics title_metrics(title_font);
	const QFontMetrics meta_metrics(meta_font);
	const QFontMetrics category_metrics(category_font);
	const int margin = 2 * header->style()->pixelMetric(QStyle::PM_HeaderMargin, nullptr, header);
	int width = 260;

	for (int row = 0; row < model->rowCount(); ++row) {
		const int title_width = title_metrics.horizontalAdvance(model->display_label(row));
		const int meta_width = meta_metrics.horizontalAdvance(model->meta_label(row));
		const int category_width =
			model->starts_new_category(row) ? category_metrics.horizontalAdvance(model->category_label(row).toUpper()) : 0;
		width = std::max(width, std::max({title_width, meta_width, category_width}) + margin * 2 + 20);
	}

	return width;
}
}

ObjectEditor::ObjectEditor(QWidget* parent) : QMainWindow(parent) {
	ui.setupUi(this);
	setAttribute(Qt::WA_DeleteOnClose);

	std::ifstream f("data/warcraft/ability_insights.json");
	ability_insights = nlohmann::json::parse(f);

	custom_unit_icon = resource_manager.load<QIconResource>(world_edit_data.data("WorldEditArt", "ToolBarIcon_OE_NewUnit")).value();
	custom_item_icon = resource_manager.load<QIconResource>(world_edit_data.data("WorldEditArt", "ToolBarIcon_OE_NewItem")).value();
	custom_doodad_icon = resource_manager.load<QIconResource>(world_edit_data.data("WorldEditArt", "ToolBarIcon_OE_NewDood")).value();
	custom_destructible_icon = resource_manager.load<QIconResource>(world_edit_data.data("WorldEditArt", "ToolBarIcon_OE_NewDest")).value();
	custom_ability_icon = resource_manager.load<QIconResource>(world_edit_data.data("WorldEditArt", "ToolBarIcon_OE_NewAbil")).value();
	custom_buff_icon = resource_manager.load<QIconResource>(world_edit_data.data("WorldEditArt", "ToolBarIcon_OE_NewBuff")).value();
	custom_upgrade_icon = resource_manager.load<QIconResource>(world_edit_data.data("WorldEditArt", "ToolBarIcon_OE_NewUpgr")).value();

	{
		QSettings settings;
		current_field_search = settings.value("ObjectEditor/fieldSearch").toString();
		current_field_category = settings.value("ObjectEditor/fieldCategory").toString();
		current_modified_only = settings.value("ObjectEditor/modifiedOnly", false).toBool();
		current_core_only = settings.value("ObjectEditor/coreOnly", false).toBool();

		const std::array<QString, 7> category_keys = {"units", "items", "doodads", "destructibles", "abilities", "upgrades", "buffs"};
		for (int i = 0; i < category_keys.size(); ++i) {
			browser_searches[i] = settings.value("ObjectEditor/browser/" + category_keys[i] + "/search").toString();
			browser_custom_only[i] = settings.value("ObjectEditor/browser/" + category_keys[i] + "/customOnly", false).toBool();
		}

		const int bookmark_count = settings.beginReadArray("ObjectEditor/bookmarks");
		for (int i = 0; i < bookmark_count; ++i) {
			settings.setArrayIndex(i);
			const QString category = settings.value("category").toString();
			const std::string id = settings.value("id").toString().toStdString();
			const QString name = settings.value("name").toString();
			if (!id.empty()) {
				const auto parsed_category =
					category == "items"		 ? Category::item
					: category == "doodads"	 ? Category::doodad
					: category == "destructibles" ? Category::destructible
					: category == "abilities"	 ? Category::ability
					: category == "upgrades"	 ? Category::upgrade
					: category == "buffs"		 ? Category::buff
											 : Category::unit;
				object_bookmarks.push_back({parsed_category, id, name});
			}
		}
		settings.endArray();
	}

	dock_manager = new ads::CDockManager;
	dock_manager->setStyleSheet("");
	setCentralWidget(dock_manager);
	setStyleSheet(
		"ObjectEditor { background: rgb(24, 29, 35); color: rgb(232, 236, 241); }"
		"ObjectEditor QWidget { color: rgb(232, 236, 241); }"
		"ObjectEditor QAbstractScrollArea, ObjectEditor QScrollArea, ObjectEditor QFrame { background: transparent; }"
		"ObjectEditor QLineEdit, ObjectEditor QComboBox {"
		"background: rgb(24, 29, 35);"
		"border: 1px solid rgba(255, 255, 255, 16);"
		"border-radius: 8px;"
		"padding: 6px 10px;"
		"color: rgb(232, 236, 241);"
		"selection-background-color: rgb(92, 145, 224);"
		"selection-color: rgb(245, 248, 252);"
		"}"
		"ObjectEditor QLineEdit:focus, ObjectEditor QComboBox:focus { border-color: rgba(130, 184, 255, 235); }"
		"ObjectEditor QComboBox::drop-down { border: none; width: 22px; }"
		"ObjectEditor QComboBox QAbstractItemView {"
		"background: rgb(24, 29, 35);"
		"border: 1px solid rgba(255, 255, 255, 16);"
		"selection-background-color: rgb(52, 60, 72);"
		"selection-color: rgb(245, 248, 252);"
		"}"
		"ObjectEditor QPushButton, ObjectEditor QToolButton { color: rgb(232, 236, 241); }"
		"ObjectEditor QPushButton {"
		"background: rgb(32, 38, 45);"
		"border: 1px solid rgba(255, 255, 255, 16);"
		"border-radius: 8px;"
		"padding: 7px 12px;"
		"}"
		"ObjectEditor QPushButton:hover { background: rgb(52, 60, 72); border-color: rgba(255, 255, 255, 24); }"
		"ObjectEditor QPushButton:pressed { background: rgb(40, 48, 58); }"
		"ObjectEditor QTreeView, ObjectEditor QTableView {"
		"background: rgb(24, 29, 35);"
		"alternate-background-color: rgb(28, 34, 41);"
		"border: 1px solid rgba(255, 255, 255, 16);"
		"border-radius: 10px;"
		"gridline-color: rgba(255, 255, 255, 10);"
		"selection-background-color: rgb(52, 60, 72);"
		"selection-color: rgb(245, 248, 252);"
		"}"
		"ObjectEditor QTreeView::item, ObjectEditor QTableView::item { padding: 2px 6px; }"
		"ObjectEditor QTreeView::item:hover, ObjectEditor QTableView::item:hover { background: rgba(255, 255, 255, 8); }"
		"ObjectEditor QHeaderView::section {"
		"background: rgb(32, 38, 45);"
		"border: none;"
		"border-bottom: 1px solid rgba(255, 255, 255, 16);"
		"padding: 6px 8px;"
		"color: rgb(205, 214, 224);"
		"}"
		"ObjectEditor QScrollBar:vertical { background: rgb(24, 29, 35); width: 12px; margin: 4px 0; }"
		"ObjectEditor QScrollBar::handle:vertical { background: rgba(255, 255, 255, 18); border-radius: 6px; min-height: 28px; }"
		"ObjectEditor QScrollBar::handle:vertical:hover { background: rgba(255, 255, 255, 28); }"
		"ObjectEditor QScrollBar:horizontal { background: rgb(24, 29, 35); height: 12px; margin: 0 4px; }"
		"ObjectEditor QScrollBar::handle:horizontal { background: rgba(255, 255, 255, 18); border-radius: 6px; min-width: 28px; }"
		"ObjectEditor QScrollBar::handle:horizontal:hover { background: rgba(255, 255, 255, 28); }"
		"ObjectEditor QScrollBar::add-line, ObjectEditor QScrollBar::sub-line, ObjectEditor QScrollBar::add-page, ObjectEditor QScrollBar::sub-page { background: transparent; border: none; }"
		"#objectEditorSummary { border: 1px solid rgba(255, 255, 255, 16); border-radius: 8px; background: rgb(32, 38, 45); }"
		"#objectEditorSummaryTitle { font-size: 16px; font-weight: 600; }"
		"#objectEditorSummaryMeta { color: rgb(182, 188, 198); }"
		"#objectEditorSummaryKey { color: rgb(154, 160, 170); }"
		"#objectEditorSummaryValue { color: rgb(216, 220, 226); }"
		"QToolButton#objectEditorSummaryAction { border: 1px solid rgba(255, 255, 255, 16); border-radius: 8px; padding: 8px 12px; background: rgb(32, 38, 45); min-width: 128px; }"
		"QToolButton#objectEditorSummaryAction:hover { background: rgb(52, 60, 72); border-color: rgba(255, 255, 255, 24); }"
		"QToolButton#objectEditorSummaryAction:checked { background: rgba(92, 145, 224, 188); color: rgb(245, 248, 252); border-color: rgba(130, 184, 255, 235); }"
		"#objectEditorSelectionStrip { border: 1px solid rgba(255, 255, 255, 16); border-radius: 8px; background: rgb(32, 38, 45); }"
		"#objectEditorSelectionTitle { font-size: 13px; font-weight: 600; }"
		"#objectEditorSelectionMeta { color: rgb(168, 174, 184); }"
		"#objectEditorInsights { border: 1px solid rgba(130, 184, 255, 90); border-radius: 8px; background: rgba(92, 145, 224, 0.10); }"
		"#objectEditorInsightsTitle { font-size: 13px; font-weight: 600; }"
		"#objectEditorInsightsMeta { color: rgb(168, 174, 184); }"
		"#objectEditorEmptyState { border: 1px dashed rgba(255, 255, 255, 20); border-radius: 10px; background: rgb(28, 34, 41); }"
		"#objectEditorEmptyTitle { font-size: 14px; font-weight: 600; }"
		"#objectEditorEmptyMeta { color: rgb(168, 174, 184); }"
		"#objectEditorFilterState { color: rgb(168, 174, 184); }"
		"#objectEditorFilterBar, #objectEditorBookmarksBar {"
		"background: rgb(32, 38, 45);"
		"border: 1px solid rgba(255, 255, 255, 16);"
		"border-radius: 8px;"
		"}"
		"#objectEditorInspectorBar {"
		"background: rgb(32, 38, 45);"
		"border: 1px solid rgba(255, 255, 255, 16);"
		"border-radius: 8px;"
		"}"
		"#objectEditorPill { border: 1px solid rgba(255, 255, 255, 16); border-radius: 10px; padding: 2px 8px; background: rgba(255, 255, 255, 0.04); }"
		"#objectEditorFieldSearch { padding: 4px 8px; }"
		"#objectEditorSectionFilter { padding: 4px 8px; min-width: 150px; }"
		"#objectEditorSectionStrip { background: rgb(32, 38, 45); border: 1px solid rgba(255, 255, 255, 16); border-radius: 8px; }"
		"QLineEdit[class='fieldSearch'] { padding: 4px 8px; }"
		"#objectEditorBrowserBar { background: rgb(32, 38, 45); border-bottom: 1px solid rgba(255, 255, 255, 16); }"
		"#objectEditorBrowserStack {"
		"background: rgb(24, 29, 35);"
		"border: 1px solid rgba(255, 255, 255, 16);"
		"border-radius: 10px;"
		"}"
		"#objectEditorBrowserContainer { background: rgb(24, 29, 35); }"
		"#objectEditorDetailsRoot, #objectEditorDetailsScrollContainer { background: rgb(24, 29, 35); }"
		"#objectEditorBrowserTitle { font-size: 14px; font-weight: 600; }"
		"#objectEditorBrowserMeta { color: rgb(168, 174, 184); }"
		"#objectEditorInspectorMeta { color: rgb(168, 174, 184); }"
		"QToolBar { border: 0; spacing: 8px; padding: 0; background: rgb(32, 38, 45); }"
		"ads--CDockAreaWidget { background: rgb(24, 29, 35); }"
		"ads--CDockAreaTitleBar { background: rgb(32, 38, 45); border-bottom: 1px solid rgba(255, 255, 255, 16); }"
		"ads--CDockAreaTabBar { background: rgb(32, 38, 45); }"
		"ads--CDockAreaWidget[focused=\"true\"] ads--CDockAreaTitleBar { background: rgb(32, 38, 45); border-bottom: 1px solid rgba(255, 255, 255, 16); }"
		"ads--CDockAreaWidget[focused=\"true\"] ads--CDockAreaTabBar { background: rgb(32, 38, 45); }"
		"ads--CDockWidgetTab { background: transparent; border: 0; border-radius: 6px; margin: 4px 2px; padding: 4px 10px; }"
		"ads--CDockWidgetTab:hover { background: rgba(255, 255, 255, 0.05); }"
		"ads--CDockWidgetTab[activeTab=\"true\"] { background: rgba(92, 145, 224, 188); color: rgb(245, 248, 252); }"
		"QToolButton#objectEditorChip { border: 1px solid rgba(255, 255, 255, 16); border-radius: 11px; padding: 3px 10px; background: rgb(32, 38, 45); }"
		"QToolButton#objectEditorChip:hover { background: rgb(52, 60, 72); border-color: rgba(255, 255, 255, 24); }"
		"QToolButton#objectEditorChip:checked { background: rgba(92, 145, 224, 188); color: rgb(245, 248, 252); border-color: rgba(130, 184, 255, 235); }"
		"QTreeView { outline: 0; }"
	);
	dock_manager->setStyleSheet(styleSheet());

	details_dock = new ads::CDockWidget(dock_manager, "Object Data");
	details_dock->setFeature(ads::CDockWidget::NoTab, true);
	dock_area = dock_manager->setCentralWidget(details_dock);
	reset_details_panel();

	unitTreeModel = new UnitTreeModel(this);
	itemTreeModel = new ItemTreeModel(this);
	doodadTreeModel = new DoodadTreeModel(this);
	destructibleTreeModel = new DestructibleTreeModel(this);
	abilityTreeModel = new AbilityTreeModel(this);
	upgradeTreeModel = new UpgradeTreeModel(this);
	buffTreeModel = new BuffTreeModel(this);

	addTypeTreeView(unitTreeModel, unitTreeFilter, units_table, unit_explorer, custom_unit_icon->icon, "Units", Category::unit);
	addTypeTreeView(itemTreeModel, itemTreeFilter, items_table, item_explorer, custom_item_icon->icon, "Items", Category::item);
	addTypeTreeView(
		doodadTreeModel,
		doodadTreeFilter,
		doodads_table,
		doodad_explorer,
		custom_doodad_icon->icon,
		"Doodads",
		Category::doodad
	);
	addTypeTreeView(
		destructibleTreeModel,
		destructibleTreeFilter,
		destructibles_table,
		destructible_explorer,
		custom_destructible_icon->icon,
		"Destructibles",
		Category::destructible
	);
	addTypeTreeView(
		abilityTreeModel,
		abilityTreeFilter,
		abilities_table,
		ability_explorer,
		custom_ability_icon->icon,
		"Abilities",
		Category::ability
	);
	addTypeTreeView(
		upgradeTreeModel,
		upgradeTreeFilter,
		upgrade_table,
		upgrade_explorer,
		custom_upgrade_icon->icon,
		"Upgrades",
		Category::upgrade
	);
	addTypeTreeView(buffTreeModel, buffTreeFilter, buff_table, buff_explorer, custom_buff_icon->icon, "Buffs", Category::buff);

	QSettings settings;
	explorer_area->setCurrentIndex(settings.value("ObjectEditor/currentTab", 0).toInt());
	// Set initial sizes, the second size doesn't really matter with only 2 dock areas
	dock_manager->setSplitterSizes(explorer_area, {645, 9999});

	connect(unit_explorer, &QTreeView::clicked, [&](const QModelIndex& index) {
		itemClicked(unit_explorer, unitTreeFilter, units_table, index);
	});
	connect(item_explorer, &QTreeView::clicked, [&](const QModelIndex& index) {
		itemClicked(item_explorer, itemTreeFilter, items_table, index);
	});
	connect(doodad_explorer, &QTreeView::clicked, [&](const QModelIndex& index) {
		itemClicked(doodad_explorer, doodadTreeFilter, doodads_table, index);
	});
	connect(destructible_explorer, &QTreeView::clicked, [&](const QModelIndex& index) {
		itemClicked(destructible_explorer, destructibleTreeFilter, destructibles_table, index);
	});
	connect(ability_explorer, &QTreeView::clicked, [&](const QModelIndex& index) {
		itemClicked(ability_explorer, abilityTreeFilter, abilities_table, index);
	});
	connect(upgrade_explorer, &QTreeView::clicked, [&](const QModelIndex& index) {
		itemClicked(upgrade_explorer, upgradeTreeFilter, upgrade_table, index);
	});
	connect(buff_explorer, &QTreeView::clicked, [&](const QModelIndex& index) {
		itemClicked(buff_explorer, buffTreeFilter, buff_table, index);
	});

	connect(new QShortcut(QKeySequence(Qt::CTRL | Qt::Key_F), this), &QShortcut::activated, [&]() {
		auto edit = explorer_area->currentDockWidget()->findChild<QLineEdit*>("search");
		edit->setFocus();
		edit->selectAll();
	});
	connect(new QShortcut(QKeySequence(Qt::CTRL | Qt::SHIFT | Qt::Key_F), this), &QShortcut::activated, [this]() {
		if (auto* edit = details_dock->widget() ? details_dock->widget()->findChild<QLineEdit*>("objectEditorFieldSearch") : nullptr) {
			edit->setFocus();
			edit->selectAll();
		}
	});
	connect(new QShortcut(QKeySequence(Qt::ALT | Qt::Key_Left), this), &QShortcut::activated, this, [this]() {
		navigate_object_history(-1);
	});
	connect(new QShortcut(QKeySequence(Qt::ALT | Qt::Key_Right), this), &QShortcut::activated, this, [this]() {
		navigate_object_history(1);
	});
	connect(new QShortcut(QKeySequence(Qt::Key_O), this), &QShortcut::activated, this, &QWidget::close);
	connect(qApp, &QApplication::focusChanged, this, [this](QWidget*, QWidget*) {
		refresh_details_focus_visuals();
	});
	qApp->installEventFilter(this);

	show();
}

bool ObjectEditor::restore_tree_state(const QString& key, QTreeView* view) const {
	QSettings settings;
	const QStringList expanded_paths = settings.value("ObjectEditor/" + key + "/expandedPaths").toStringList();
	if (expanded_paths.isEmpty()) {
		return false;
	}

	QSet<QString> expanded_path_set;
	for (const auto& path : expanded_paths) {
		expanded_path_set.insert(path);
	}

	view->collapseAll();
	expand_saved_paths(view, QModelIndex(), expanded_path_set);
	return true;
}

void ObjectEditor::save_tree_state(const QString& key, const QTreeView* view) const {
	QStringList expanded_paths;
	collect_expanded_paths(view, QModelIndex(), expanded_paths);

	QSettings settings;
	settings.setValue("ObjectEditor/" + key + "/expandedPaths", expanded_paths);
}

void ObjectEditor::closeEvent(QCloseEvent* event) {
	QSettings settings;
	settings.setValue("ObjectEditor/currentTab", explorer_area ? explorer_area->currentIndex() : 0);
	settings.setValue("ObjectEditor/fieldSearch", current_field_search);
	settings.setValue("ObjectEditor/fieldCategory", current_field_category);
	settings.setValue("ObjectEditor/modifiedOnly", current_modified_only);
	settings.setValue("ObjectEditor/coreOnly", current_core_only);
	const std::array<QString, 7> category_keys = {"units", "items", "doodads", "destructibles", "abilities", "upgrades", "buffs"};
	for (int i = 0; i < category_keys.size(); ++i) {
		settings.setValue("ObjectEditor/browser/" + category_keys[i] + "/search", browser_searches[i]);
		settings.setValue("ObjectEditor/browser/" + category_keys[i] + "/customOnly", browser_custom_only[i]);
	}
	settings.beginWriteArray("ObjectEditor/bookmarks");
	for (int i = 0; i < object_bookmarks.size(); ++i) {
		settings.setArrayIndex(i);
		settings.setValue("category", category_key(object_bookmarks[i].category));
		settings.setValue("id", QString::fromStdString(object_bookmarks[i].id));
		settings.setValue("name", object_bookmarks[i].name);
	}
	settings.endArray();
	save_tree_state("units", unit_explorer);
	save_tree_state("items", item_explorer);
	save_tree_state("doodads", doodad_explorer);
	save_tree_state("destructibles", destructible_explorer);
	save_tree_state("abilities", ability_explorer);
	save_tree_state("upgrades", upgrade_explorer);
	save_tree_state("buffs", buff_explorer);

	QMainWindow::closeEvent(event);
}

void ObjectEditor::itemClicked(QTreeView* view, const QSortFilterProxyModel* model, TableModel* table, const QModelIndex& index) {
	const BaseTreeItem* item = static_cast<BaseTreeItem*>(model->mapToSource(index).internalPointer());
	if (item->baseCategory || item->subCategory) {
		return;
	}

	if (auto* area = details_dock->widget() ? details_dock->widget()->findChild<QScrollArea*>("objectEditorDetailsScrollArea") : nullptr) {
		current_details_scroll_y = area->verticalScrollBar()->value();
	}

	current_explorer_view = view;
	current_category =
		view == unit_explorer		 ? Category::unit
		: view == item_explorer		 ? Category::item
		: view == doodad_explorer	 ? Category::doodad
		: view == destructible_explorer ? Category::destructible
		: view == ability_explorer	 ? Category::ability
		: view == upgrade_explorer	 ? Category::upgrade
									 : Category::buff;
	open_by_id(table, item->id, index.data(Qt::DisplayRole).toString(), index.data(Qt::DecorationRole).value<QIcon>());
}

void ObjectEditor::reset_details_panel() {
	QLabel* image = new QLabel();
	image->setPixmap(QPixmap("data/icons/object_editor/background.png"));
	image->setAlignment(Qt::AlignCenter);
	details_dock->setWidget(image);
	details_dock->setWindowTitle("Object Data");
	details_dock->setIcon(QIcon());
	current_details_id.clear();
	current_details_view = nullptr;
	current_details_scroll_y = 0;
}

void ObjectEditor::refresh_details_focus_visuals() const {
	if (!current_details_view) {
		return;
	}

	current_details_view->viewport()->update();
	if (auto* header = current_details_view->verticalHeader()) {
		header->viewport()->update();
	}
}

void ObjectEditor::push_object_history(const Category category, const std::string& id, const QString& name) {
	if (history_navigation) {
		return;
	}

	if (object_history_index >= 0 && object_history_index < object_history.size()) {
		const auto& current = object_history[object_history_index];
		if (current.category == category && current.id == id) {
			return;
		}
	}

	if (object_history_index + 1 < object_history.size()) {
		object_history.erase(object_history.begin() + object_history_index + 1, object_history.end());
	}

	object_history.push_back({category, id, name});
	if (object_history.size() > 12) {
		object_history.erase(object_history.begin());
	}
	object_history_index = static_cast<int>(object_history.size()) - 1;
}

void ObjectEditor::navigate_object_history(const int delta) {
	if (object_history.empty()) {
		return;
	}

	const int next_index = object_history_index + delta;
	if (next_index < 0 || next_index >= object_history.size()) {
		return;
	}

	history_navigation = true;
	object_history_index = next_index;
	const auto entry = object_history[object_history_index];
	select_id(entry.category, entry.id);
	history_navigation = false;
}

bool ObjectEditor::is_object_bookmarked(const Category category, const std::string& id) const {
	return std::ranges::any_of(object_bookmarks, [&](const auto& bookmark) {
		return bookmark.category == category && bookmark.id == id;
	});
}

void ObjectEditor::toggle_object_bookmark(const Category category, const std::string& id, const QString& name) {
	if (const auto it = std::ranges::find_if(object_bookmarks, [&](const auto& bookmark) {
			return bookmark.category == category && bookmark.id == id;
		});
		it != object_bookmarks.end()) {
		object_bookmarks.erase(it);
	} else {
		object_bookmarks.push_back({category, id, name});
		if (object_bookmarks.size() > 16) {
			object_bookmarks.erase(object_bookmarks.begin());
		}
	}
}

TableModel* ObjectEditor::table_for_category(const Category category) const {
	switch (category) {
		case Category::unit:
			return units_table;
		case Category::item:
			return items_table;
		case Category::doodad:
			return doodads_table;
		case Category::destructible:
			return destructibles_table;
		case Category::ability:
			return abilities_table;
		case Category::upgrade:
			return upgrade_table;
		case Category::buff:
			return buff_table;
	}

	return nullptr;
}

BaseTreeModel* ObjectEditor::tree_model_for_category(const Category category) const {
	switch (category) {
		case Category::unit:
			return unitTreeModel;
		case Category::item:
			return itemTreeModel;
		case Category::doodad:
			return doodadTreeModel;
		case Category::destructible:
			return destructibleTreeModel;
		case Category::ability:
			return abilityTreeModel;
		case Category::upgrade:
			return upgradeTreeModel;
		case Category::buff:
			return buffTreeModel;
	}

	return nullptr;
}

BaseFilter* ObjectEditor::filter_for_category(const Category category) const {
	switch (category) {
		case Category::unit:
			return unitTreeFilter;
		case Category::item:
			return itemTreeFilter;
		case Category::doodad:
			return doodadTreeFilter;
		case Category::destructible:
			return destructibleTreeFilter;
		case Category::ability:
			return abilityTreeFilter;
		case Category::upgrade:
			return upgradeTreeFilter;
		case Category::buff:
			return buffTreeFilter;
	}

	return nullptr;
}

QTreeView* ObjectEditor::view_for_category(const Category category) const {
	switch (category) {
		case Category::unit:
			return unit_explorer;
		case Category::item:
			return item_explorer;
		case Category::doodad:
			return doodad_explorer;
		case Category::destructible:
			return destructible_explorer;
		case Category::ability:
			return ability_explorer;
		case Category::upgrade:
			return upgrade_explorer;
		case Category::buff:
			return buff_explorer;
	}

	return nullptr;
}

QString ObjectEditor::category_item_label(const Category category) const {
	switch (category) {
		case Category::unit:
			return "Unit";
		case Category::item:
			return "Item";
		case Category::doodad:
			return "Doodad";
		case Category::destructible:
			return "Destructible";
		case Category::ability:
			return "Ability";
		case Category::upgrade:
			return "Upgrade";
		case Category::buff:
			return "Buff";
	}

	return "Object";
}

void ObjectEditor::copy_object_entry_to_clipboard(const Category category, const std::string& id) const {
	QJsonObject payload;
	payload["category"] = category_key(category);
	payload["id"] = QString::fromStdString(id);

	auto* mime = new QMimeData;
	mime->setData("application/x-hivewe-object-entry", QJsonDocument(payload).toJson(QJsonDocument::Compact));
	mime->setText(QString::fromStdString(id));
	QApplication::clipboard()->setMimeData(mime);
}

void ObjectEditor::copy_selected_object_entry(QTreeView* view, BaseFilter* filter, const Category category) {
	if (!view || !filter) {
		return;
	}

	QModelIndex index = view->currentIndex();
	if ((!index.isValid() || !view->selectionModel()) && view->selectionModel() && !view->selectionModel()->selectedRows().isEmpty()) {
		index = view->selectionModel()->selectedRows().front();
	}
	if (!index.isValid()) {
		return;
	}

	const auto* tree_item = static_cast<const BaseTreeItem*>(filter->mapToSource(index).internalPointer());
	if (!tree_item || tree_item->baseCategory || tree_item->subCategory) {
		return;
	}

	current_explorer_view = view;
	current_category = category;
	copy_object_entry_to_clipboard(category, tree_item->id);
}

void ObjectEditor::duplicate_object_entry(
	QTreeView* view,
	BaseFilter* filter,
	BaseTreeModel* tree_model,
	TableModel* table,
	const QString& name,
	const std::string& source_id
) {
	if (!view || !filter || !tree_model || !table || !table->slk->row_headers.contains(source_id)) {
		return;
	}

	QDialog dialog(this, Qt::WindowTitleHint | Qt::WindowCloseButtonHint);
	dialog.setWindowModality(Qt::WindowModality::WindowModal);
	dialog.setWindowTitle("Copy " + name);

	QLabel* source_label = new QLabel("Source: " + (object_display_name(table, source_id).isEmpty()
		? QString::fromStdString(source_id)
		: object_display_name(table, source_id) + " (" + QString::fromStdString(source_id) + ")"));
	source_label->setWordWrap(true);

	QLineEdit* id = new QLineEdit;
	id->setPlaceholderText("Free ID");
	id->setText(QString::fromStdString(next_sequential_custom_id(table, source_id)));
	id->setFont(QFont("consolas"));

	QAction* id_error_icon = id->addAction(dialog.style()->standardIcon(QStyle::SP_MessageBoxCritical), QLineEdit::TrailingPosition);
	id_error_icon->setVisible(false);

	QDialogButtonBox* button_box = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
	connect(button_box, &QDialogButtonBox::accepted, &dialog, &QDialog::accept);
	connect(button_box, &QDialogButtonBox::rejected, &dialog, &QDialog::reject);

	auto validate = [table, id, button_box, id_error_icon]() {
		const std::string new_id = id->text().toStdString();
		if (new_id.size() != 4) {
			id_error_icon->setVisible(true);
			id_error_icon->setToolTip("ID must be exactly 4 characters long.");
			id->setStyleSheet("QLineEdit { border: 1px solid #e74c3c; }");
			button_box->button(QDialogButtonBox::Ok)->setEnabled(false);
		} else if (table->slk->row_headers.contains(new_id)) {
			id_error_icon->setVisible(true);
			id_error_icon->setToolTip(QString::fromStdString(std::format("ID '{}' is already in use.", new_id)));
			id->setStyleSheet("QLineEdit { border: 1px solid #e74c3c; }");
			button_box->button(QDialogButtonBox::Ok)->setEnabled(false);
		} else {
			id_error_icon->setVisible(false);
			id->setStyleSheet("");
			button_box->button(QDialogButtonBox::Ok)->setEnabled(true);
		}
	};

	QVBoxLayout* layout = new QVBoxLayout(&dialog);
	layout->addWidget(source_label);
	layout->addWidget(id);
	layout->addWidget(button_box);

	connect(id, &QLineEdit::textChanged, &dialog, [validate](const QString&) {
		validate();
	});
	validate();

	if (dialog.exec() != QDialog::Accepted) {
		return;
	}

	const std::string new_id = id->text().toStdString();
	table->copyRow(source_id, new_id);

	if (const std::string name_field = primary_name_field(table); !name_field.empty() && table->slk->column_headers.contains(name_field)) {
		const QString copied_name = copied_name_with_suffix(object_display_name(table, source_id));
		if (!copied_name.isEmpty()) {
			table->setData(table->index(table->slk->row_headers.at(new_id), table->slk->column_headers.at(name_field)), copied_name, Qt::EditRole);
		}
	}

	const QModelIndex source_index = table->rowIDToIndex(new_id);
	const QModelIndex tree_index = tree_model->mapFromSource(source_index);
	const QModelIndex filtered_index = filter->mapFromSource(tree_index);
	if (!filtered_index.isValid()) {
		return;
	}

	if (const QModelIndex parent = filtered_index.parent(); parent.isValid()) {
		view->expand(parent);
	}
	view->setCurrentIndex(filtered_index);
	view->selectionModel()->select(filtered_index, QItemSelectionModel::ClearAndSelect | QItemSelectionModel::Rows);
	view->scrollTo(filtered_index, QAbstractItemView::ScrollHint::PositionAtCenter);
	itemClicked(view, filter, table, filtered_index);
}

void ObjectEditor::begin_rename_selected_object() {
	QTreeView* target_view = nullptr;
	if (QWidget* focus_widget = QApplication::focusWidget()) {
		for (QTreeView* candidate : {unit_explorer, item_explorer, doodad_explorer, destructible_explorer, ability_explorer, upgrade_explorer, buff_explorer}) {
			if (candidate && is_focus_within(focus_widget, candidate)) {
				target_view = candidate;
				break;
			}
		}
	}

	if (!target_view && explorer_area && explorer_area->currentDockWidget()) {
		target_view = explorer_area->currentDockWidget()->findChild<QTreeView*>();
	}

	if (!target_view) {
		target_view = current_explorer_view;
	}

	if (!target_view) {
		return;
	}

	current_explorer_view = target_view;
	current_category =
		target_view == unit_explorer		 ? Category::unit
		: target_view == item_explorer		 ? Category::item
		: target_view == doodad_explorer	 ? Category::doodad
		: target_view == destructible_explorer ? Category::destructible
		: target_view == ability_explorer	 ? Category::ability
		: target_view == upgrade_explorer	 ? Category::upgrade
										 : Category::buff;

	const auto* filter = qobject_cast<BaseFilter*>(target_view->model());
	if (!filter) {
		return;
	}

	QModelIndex index = target_view->currentIndex();
	if ((!index.isValid() || !target_view->selectionModel()) && target_view->selectionModel() && !target_view->selectionModel()->selectedRows().isEmpty()) {
		index = target_view->selectionModel()->selectedRows().front();
	}
	if (!index.isValid()) {
		return;
	}

	const auto* tree_item = static_cast<const BaseTreeItem*>(filter->mapToSource(index).internalPointer());
	if (!tree_item || tree_item->baseCategory || tree_item->subCategory) {
		return;
	}

	target_view->setCurrentIndex(index);
	target_view->scrollTo(index, QAbstractItemView::ScrollHint::PositionAtCenter);
	target_view->setFocus(Qt::OtherFocusReason);

	const QPersistentModelIndex persistent_index(index);
	QTimer::singleShot(0, target_view, [target_view, persistent_index]() {
		if (!persistent_index.isValid()) {
			return;
		}

		target_view->edit(persistent_index);
		if (auto* line_edit = qobject_cast<QLineEdit*>(QApplication::focusWidget())) {
			line_edit->selectAll();
		}
	});
}

void ObjectEditor::delete_selected_object_entries(QTreeView* view, BaseFilter* filter, TableModel* table, const QString& name) {
	if (!view || !filter || !table || !view->selectionModel()) {
		return;
	}

	std::vector<std::string> ids_to_delete;
	QStringList labels_to_delete;
	QModelIndexList selection = view->selectionModel()->selectedRows();
	if (selection.empty() && view->currentIndex().isValid()) {
		selection.push_back(view->currentIndex());
	}

	for (const auto& index : selection) {
		const auto* tree_item = static_cast<const BaseTreeItem*>(filter->mapToSource(index).internalPointer());
		if (!tree_item || tree_item->baseCategory || tree_item->subCategory) {
			continue;
		}

		if (!table->slk->shadow_data.contains(tree_item->id) || !table->slk->shadow_data.at(tree_item->id).contains("oldid")) {
			continue;
		}

		if (std::ranges::find(ids_to_delete, tree_item->id) != ids_to_delete.end()) {
			continue;
		}

		ids_to_delete.push_back(tree_item->id);
		const QString display_name = object_display_name(table, tree_item->id);
		labels_to_delete.push_back(
			display_name.isEmpty() ? QString::fromStdString(tree_item->id) : display_name + " (" + QString::fromStdString(tree_item->id) + ")"
		);
	}

	if (ids_to_delete.empty()) {
		return;
	}

	QDialog confirm(this, Qt::WindowTitleHint | Qt::WindowCloseButtonHint);
	confirm.setWindowModality(Qt::WindowModality::WindowModal);
	confirm.setWindowTitle("Remove " + name);

	QVBoxLayout* layout = new QVBoxLayout(&confirm);
	layout->setContentsMargins(18, 16, 18, 16);
	layout->setSpacing(12);

	const QString singular_name = name.endsWith("ies") ? name.left(name.size() - 3) + "y" : name.chopped(name.endsWith("s") ? 1 : 0);
	QLabel* prompt = new QLabel(
		ids_to_delete.size() == 1 ? "Remove this custom " + singular_name.toLower() + "?" : "Remove these custom " + name.toLower() + "?"
	);
	prompt->setWordWrap(true);
	layout->addWidget(prompt);

	QLabel* details = new QLabel(labels_to_delete.join("\n"));
	details->setWordWrap(true);
	layout->addWidget(details);

	QDialogButtonBox* buttons = new QDialogButtonBox(QDialogButtonBox::Cancel);
	QPushButton* remove_button = buttons->addButton("Remove", QDialogButtonBox::AcceptRole);
	buttons->button(QDialogButtonBox::Cancel)->setAutoDefault(false);
	remove_button->setDefault(true);
	connect(buttons, &QDialogButtonBox::accepted, &confirm, &QDialog::accept);
	connect(buttons, &QDialogButtonBox::rejected, &confirm, &QDialog::reject);
	layout->addWidget(buttons);

	if (confirm.exec() != QDialog::Accepted) {
		return;
	}

	for (const auto& id : ids_to_delete) {
		if (current_details_id == id) {
			reset_details_panel();
		}
		table->deleteRow(id);
	}
}

void ObjectEditor::paste_copied_object() {
	const auto* mime = QApplication::clipboard()->mimeData();
	if (!mime || !mime->hasFormat("application/x-hivewe-object-entry")) {
		return;
	}

	const QJsonDocument document = QJsonDocument::fromJson(mime->data("application/x-hivewe-object-entry"));
	if (!document.isObject()) {
		return;
	}

	const QJsonObject payload = document.object();
	if (payload.value("category").toString() != category_key(current_category)) {
		return;
	}

	const std::string source_id = payload.value("id").toString().toStdString();
	duplicate_object_entry(
		view_for_category(current_category),
		filter_for_category(current_category),
		tree_model_for_category(current_category),
		table_for_category(current_category),
		category_item_label(current_category),
		source_id
	);
}

void ObjectEditor::open_by_id(TableModel* table, const std::string& id, const QString& name, QIcon icon) {
	if (current_details_id == id) {
		details_dock->raise();
		return;
	}

	push_object_history(current_category, id, name);

	QVBoxLayout* top_layout = new QVBoxLayout;
	top_layout->setContentsMargins(0, 0, 0, 0);
	top_layout->setSpacing(8);

	QVBoxLayout* scroll_layout = new QVBoxLayout;
	scroll_layout->setContentsMargins(0, 0, 0, 0);
	scroll_layout->setSpacing(8);

	const auto found = ability_insights.find(id);
	if (found != ability_insights.end()) {
		QFrame* insights = new QFrame;
		insights->setObjectName("objectEditorInsights");
		QVBoxLayout* insights_layout = new QVBoxLayout(insights);
		insights_layout->setContentsMargins(12, 10, 12, 10);
		insights_layout->setSpacing(8);

		QLabel* title = new QLabel(QString::fromUtf8((*found)["name"].get<std::string_view>()));
		title->setObjectName("objectEditorInsightsTitle");

		QWidget* tags_wrap = new QWidget;
		QHBoxLayout* tags_layout = new QHBoxLayout(tags_wrap);
		tags_layout->setContentsMargins(0, 0, 0, 0);
		tags_layout->setSpacing(6);
		for (const auto& tag : (*found)["tags"]) {
			QLabel* tag_label = new QLabel(QString::fromUtf8(tag.get<std::string_view>()));
			tag_label->setObjectName("objectEditorPill");
			tags_layout->addWidget(tag_label);
		}
		tags_layout->addStretch();

		QLabel* label = new QLabel(QString::fromUtf8((*found)["raw_text"].get<std::string_view>()));
		label->setWordWrap(true);

		QLabel* latest_tested_version =
			new QLabel("Latest tested version: " + QString::fromUtf8((*found)["latest_tested_version"].get<std::string_view>()));
		latest_tested_version->setObjectName("objectEditorInsightsMeta");

		QLabel* link = new QLabel(
			"Fix mistakes or add info directly in <a href=\"https://docs.google.com/document/d/1z17FTnhyfVL87tJgLmwWks3Low6TuQ0tjfKHXBELWpo/edit\">the Google Docs</a>."
		);
		link->setOpenExternalLinks(true);

		insights_layout->addWidget(title);
		insights_layout->addWidget(tags_wrap);
		insights_layout->addWidget(label);
		insights_layout->addWidget(latest_tested_version);
		insights_layout->addWidget(link);

		scroll_layout->addWidget(insights);
	}

	SingleModel* single_model = new SingleModel(table, this);
	single_model->setID(id);
	SingleModelFilter* filter_model = new SingleModelFilter(this);
	filter_model->setSourceModel(single_model);
	filter_model->set_field_search(current_field_search);
	filter_model->set_category_filter(current_field_category);
	filter_model->set_modified_only(current_modified_only);
	filter_model->set_core_only(current_core_only);

	TableDelegate* delegate = new TableDelegate(single_model);

	QFrame* summary = new QFrame;
	summary->setObjectName("objectEditorSummary");
	summary->setSizePolicy(QSizePolicy::Preferred, QSizePolicy::Fixed);
	QHBoxLayout* summary_layout = new QHBoxLayout(summary);
	summary_layout->setContentsMargins(12, 10, 12, 10);
	summary_layout->setSpacing(10);

	QLabel* summary_icon = new QLabel;
	summary_icon->setPixmap(icon.pixmap(56, 56));
	summary_icon->setFixedSize(64, 64);
	summary_icon->setAlignment(Qt::AlignCenter);

	QVBoxLayout* summary_text_layout = new QVBoxLayout;
	summary_text_layout->setContentsMargins(0, 0, 0, 0);
	summary_text_layout->setSpacing(3);

	QLabel* summary_title = new QLabel(name);
	summary_title->setObjectName("objectEditorSummaryTitle");
	summary_title->setWordWrap(true);

	const std::string base_id = base_object_id(table, id);
	const QString base_name = base_id.empty() ? QString() : object_display_name(table, base_id);
	const int modified_count = modified_field_count(table, id);
	QHBoxLayout* title_row = new QHBoxLayout;
	title_row->setContentsMargins(0, 0, 0, 0);
	title_row->setSpacing(8);
	title_row->addWidget(summary_title);
	if (modified_count > 0) {
		QLabel* modified_badge = new QLabel("Modified");
		modified_badge->setObjectName("objectEditorPill");
		title_row->addWidget(modified_badge, 0, Qt::AlignVCenter);
	}
	title_row->addStretch();

	const QString base_label = base_id.empty() ? "Base object" :
		(base_name.isEmpty() ? QString::fromStdString(base_id) : base_name + " (" + QString::fromStdString(base_id) + ")");

	auto make_summary_row = [](const QString& key, const QString& value) {
		QHBoxLayout* row = new QHBoxLayout;
		row->setContentsMargins(0, 0, 0, 0);
		row->setSpacing(10);
		QLabel* key_label = new QLabel(key);
		key_label->setObjectName("objectEditorSummaryKey");
		key_label->setMinimumWidth(68);
		QLabel* value_label = new QLabel(value);
		value_label->setObjectName("objectEditorSummaryValue");
		value_label->setWordWrap(true);
		row->addWidget(key_label, 0, Qt::AlignTop);
		row->addWidget(value_label, 1);
		return row;
	};

	summary_text_layout->addLayout(title_row);
	summary_text_layout->addLayout(make_summary_row("Raw ID", QString::fromStdString(id)));
	summary_text_layout->addLayout(make_summary_row("Parent", category_label(current_category)));
	summary_text_layout->addLayout(make_summary_row("Base Object", base_label));

	QGridLayout* summary_actions = new QGridLayout;
	summary_actions->setContentsMargins(0, 0, 0, 0);
	summary_actions->setHorizontalSpacing(8);
	summary_actions->setVerticalSpacing(8);

	QToolButton* copy_id = new QToolButton;
	copy_id->setObjectName("objectEditorSummaryAction");
	copy_id->setText("Copy ID");
	copy_id->setToolTip("Copy this object's raw ID to the clipboard.");
	copy_id->setAutoRaise(true);
	connect(copy_id, &QToolButton::clicked, this, [id]() {
		QApplication::clipboard()->setText(QString::fromStdString(id));
	});

	QToolButton* open_parent = new QToolButton;
	open_parent->setObjectName("objectEditorSummaryAction");
	open_parent->setText("Open Parent");
	open_parent->setToolTip("Open the base object this custom object is derived from.");
	open_parent->setAutoRaise(true);
	open_parent->setEnabled(!base_id.empty());
	connect(open_parent, &QToolButton::clicked, this, [this, base_id]() {
		if (!base_id.empty()) {
			select_id(current_category, base_id);
		}
	});

	QToolButton* show_modified_fields = new QToolButton;
	show_modified_fields->setObjectName("objectEditorSummaryAction");
	show_modified_fields->setText("Modified Fields");
	show_modified_fields->setToolTip("Filter the inspector to fields changed on this object.");
	show_modified_fields->setCheckable(true);
	show_modified_fields->setChecked(current_modified_only && !current_core_only);

	QToolButton* bookmark_object = new QToolButton;
	bookmark_object->setObjectName("objectEditorSummaryAction");
	bookmark_object->setText(is_object_bookmarked(current_category, id) ? "Unbookmark" : "Bookmark");
	bookmark_object->setToolTip("Add or remove this object from the quick bookmark list.");
	connect(bookmark_object, &QToolButton::clicked, this, [this, id, name]() {
		toggle_object_bookmark(current_category, id, name);
		select_id(current_category, id);
	});

	summary_actions->addWidget(copy_id, 0, 0);
	summary_actions->addWidget(open_parent, 0, 1);
	summary_actions->addWidget(bookmark_object, 1, 0);
	summary_actions->addWidget(show_modified_fields, 1, 1);

	summary_layout->addWidget(summary_icon);
	summary_layout->addLayout(summary_text_layout, 1);
	summary_layout->addLayout(summary_actions);

	QWidget* bookmark_objects_bar = new QWidget;
	bookmark_objects_bar->setObjectName("objectEditorBookmarksBar");
	QHBoxLayout* bookmark_objects_layout = new QHBoxLayout(bookmark_objects_bar);
	bookmark_objects_layout->setContentsMargins(10, 8, 10, 8);
	bookmark_objects_layout->setSpacing(6);

	QLabel* bookmark_objects_label = new QLabel("Bookmarks");
	bookmark_objects_label->setObjectName("objectEditorFilterState");
	bookmark_objects_layout->addWidget(bookmark_objects_label);

	int bookmark_count = 0;
	for (const auto& bookmark : object_bookmarks) {
		if (bookmark.category != current_category) {
			continue;
		}

		QToolButton* bookmark_button = new QToolButton;
		bookmark_button->setObjectName("objectEditorChip");
		bookmark_button->setText(bookmark.name.isEmpty() ? QString::fromStdString(bookmark.id) : bookmark.name);
		bookmark_button->setToolTip("Open bookmarked object: " + QString::fromStdString(bookmark.id));
		bookmark_button->setCheckable(false);
		bookmark_objects_layout->addWidget(bookmark_button);
		connect(bookmark_button, &QToolButton::clicked, this, [this, bookmark]() {
			select_id(bookmark.category, bookmark.id);
		});

		if (++bookmark_count >= 3) {
			break;
		}
	}
	if (bookmark_count == 0) {
		QLabel* no_bookmarks = new QLabel("No bookmarks yet");
		no_bookmarks->setObjectName("objectEditorFilterState");
		bookmark_objects_layout->addWidget(no_bookmarks);
	}
	bookmark_objects_layout->addStretch();

	QWidget* inspector_bar = new QWidget;
	inspector_bar->setObjectName("objectEditorInspectorBar");
	inspector_bar->setSizePolicy(QSizePolicy::Preferred, QSizePolicy::Fixed);
	QHBoxLayout* inspector_bar_layout = new QHBoxLayout(inspector_bar);
	inspector_bar_layout->setContentsMargins(10, 8, 10, 8);
	inspector_bar_layout->setSpacing(8);

	QLineEdit* field_search = new QLineEdit;
	field_search->setObjectName("objectEditorFieldSearch");
	field_search->setPlaceholderText("Search fields, ids, or descriptions");
	field_search->setClearButtonEnabled(true);
	field_search->setText(current_field_search);

	QComboBox* section_filter = new QComboBox;
	section_filter->setObjectName("objectEditorSectionFilter");
	section_filter->addItem("All sections", QString());
	{
		QStringList categories;
		for (int row = 0; row < single_model->rowCount(); ++row) {
			const QString category = single_model->category_label(row);
			if (!category.isEmpty() && !categories.contains(category)) {
				categories.push_back(category);
			}
		}
		for (const auto& category : categories) {
			section_filter->addItem(category, category);
		}
	}
	{
		const int category_index = section_filter->findData(current_field_category);
		section_filter->setCurrentIndex(category_index >= 0 ? category_index : 0);
	}

	QToolButton* modified_chip = new QToolButton;
	modified_chip->setObjectName("objectEditorChip");
	modified_chip->setText("Modified");
	modified_chip->setToolTip("Show only fields that differ from the base object.");
	modified_chip->setCheckable(true);
	modified_chip->setChecked(current_modified_only);

	QToolButton* core_chip = new QToolButton;
	core_chip->setObjectName("objectEditorChip");
	core_chip->setText("Core");
	core_chip->setToolTip("Show a reduced set of commonly edited fields.");
	core_chip->setCheckable(true);
	core_chip->setChecked(current_core_only);

	QLabel* inspector_meta = new QLabel;
	inspector_meta->setObjectName("objectEditorInspectorMeta");

	inspector_bar_layout->addWidget(field_search, 1);
	inspector_bar_layout->addWidget(section_filter);
	inspector_bar_layout->addWidget(inspector_meta);
	inspector_bar_layout->addWidget(modified_chip);
	inspector_bar_layout->addWidget(core_chip);

	QWidget* section_strip = new QWidget;
	section_strip->setObjectName("objectEditorSectionStrip");
	section_strip->setSizePolicy(QSizePolicy::Preferred, QSizePolicy::Fixed);
	QHBoxLayout* section_strip_layout = new QHBoxLayout(section_strip);
	section_strip_layout->setContentsMargins(8, 6, 8, 6);
	section_strip_layout->setSpacing(6);

	QButtonGroup* section_group = new QButtonGroup(section_strip);
	section_group->setExclusive(true);
	QList<QToolButton*> section_buttons;

	const auto add_section_button = [&](const QString& label, const QString& value) {
		QToolButton* button = new QToolButton;
		button->setObjectName("objectEditorChip");
		button->setText(label);
		button->setToolTip(value.isEmpty() ? "Show fields from all sections." : "Show only fields from the " + label + " section.");
		button->setCheckable(true);
		button->setProperty("sectionValue", value);
		section_group->addButton(button);
		section_strip_layout->addWidget(button);
		section_buttons.push_back(button);
		return button;
	};

	add_section_button("All", QString());
	for (int i = 1; i < section_filter->count(); ++i) {
		add_section_button(section_filter->itemText(i), section_filter->itemData(i).toString());
	}
	section_strip_layout->addStretch();

	QWidget* filter_state_bar = new QWidget;
	filter_state_bar->setObjectName("objectEditorFilterBar");
	filter_state_bar->setSizePolicy(QSizePolicy::Preferred, QSizePolicy::Fixed);
	QHBoxLayout* filter_state_layout = new QHBoxLayout(filter_state_bar);
	filter_state_layout->setContentsMargins(10, 8, 10, 8);
	filter_state_layout->setSpacing(8);

	QLabel* filter_state = new QLabel;
	filter_state->setObjectName("objectEditorFilterState");

	QToolButton* clear_filter_state = new QToolButton;
	clear_filter_state->setText("Reset Filters");
	clear_filter_state->setToolTip("Clear the current field search, section filter, and filter chips.");
	clear_filter_state->setAutoRaise(true);

	filter_state_layout->addWidget(filter_state);
	filter_state_layout->addStretch();
	filter_state_layout->addWidget(clear_filter_state);

	QTableView* view = new QTableView;
	view->setItemDelegate(delegate);
	view->horizontalHeader()->hide();
	view->setAlternatingRowColors(false);
	view->setVerticalHeader(new AlterHeader(Qt::Vertical, view));
	view->setSelectionBehavior(QAbstractItemView::SelectRows);
	view->setSelectionMode(QAbstractItemView::SingleSelection);
	view->setTabKeyNavigation(false);
	view->verticalHeader()->setSectionsClickable(true);
	view->verticalHeader()->setSectionResizeMode(QHeaderView::ResizeMode::ResizeToContents);
	view->verticalHeader()->setMinimumSectionSize(46);
	view->horizontalHeader()->setSectionResizeMode(QHeaderView::ResizeMode::Stretch);
	view->setIconSize({24, 24});
	view->setWordWrap(true);
	view->setSizeAdjustPolicy(QAbstractScrollArea::SizeAdjustPolicy::AdjustToContents);
	view->setStyleSheet("QTableView::item { padding-left: 8px; padding-right: 8px; }");
	{
		QPalette palette = view->palette();
		palette.setColor(QPalette::Base, QColor(24, 29, 35));
		palette.setColor(QPalette::AlternateBase, QColor(28, 34, 41));
		palette.setColor(QPalette::Text, QColor(232, 236, 241));
		palette.setColor(QPalette::WindowText, QColor(232, 236, 241));
		palette.setColor(QPalette::Mid, QColor(70, 79, 92));
		palette.setColor(QPalette::Highlight, QColor(52, 60, 72));
		palette.setColor(QPalette::HighlightedText, QColor(245, 248, 252));
		palette.setColor(QPalette::Inactive, QPalette::Highlight, QColor(52, 60, 72));
		palette.setColor(QPalette::Inactive, QPalette::HighlightedText, QColor(232, 236, 241));
		view->setPalette(palette);
		view->verticalHeader()->setPalette(palette);
		view->verticalHeader()->viewport()->setAutoFillBackground(true);
		QPalette header_palette = view->verticalHeader()->viewport()->palette();
		header_palette.setColor(QPalette::Window, QColor(32, 38, 45));
		view->verticalHeader()->viewport()->setPalette(header_palette);
	}
	view->setModel(filter_model);
	const int field_column_width = details_header_width(single_model, view->verticalHeader());
	view->verticalHeader()->setFixedWidth(field_column_width);
	current_details_view = view;
	connect(new QShortcut(QKeySequence::Copy, view), &QShortcut::activated, view, [view]() {
		copy_selected_field_to_clipboard(view);
	});
	connect(new QShortcut(QKeySequence::Paste, view), &QShortcut::activated, view, [view]() {
		paste_selected_field_from_clipboard(view);
	});
	view->installEventFilter(this);
	view->viewport()->installEventFilter(this);
	const auto update_inspector_meta = [filter_model, single_model, inspector_meta]() {
		inspector_meta->setText(QString::number(filter_model->rowCount()) + " of " + QString::number(single_model->rowCount()) + " fields");
	};
	update_inspector_meta();
	connect(field_search, &QLineEdit::textChanged, this, [this, filter_model](const QString& text) {
		current_field_search = text;
		filter_model->set_field_search(text);
	});
	connect(section_filter, &QComboBox::currentIndexChanged, this, [this, filter_model, section_filter](int) {
		current_field_category = section_filter->currentData().toString();
		filter_model->set_category_filter(current_field_category);
	});
	connect(section_filter, &QComboBox::currentIndexChanged, section_strip, [section_buttons, section_filter](int) {
		const QString value = section_filter->currentData().toString();
		for (auto* button : section_buttons) {
			button->setChecked(button->property("sectionValue").toString() == value);
		}
	});
	connect(section_group, &QButtonGroup::buttonToggled, section_strip, [section_filter](QAbstractButton* button, bool checked) {
		if (!checked) {
			return;
		}

		const QString value = button->property("sectionValue").toString();
		const int index = section_filter->findData(value);
		if (index >= 0 && section_filter->currentIndex() != index) {
			section_filter->setCurrentIndex(index);
		}
	});
	connect(modified_chip, &QToolButton::toggled, this, [this, filter_model](const bool checked) {
		current_modified_only = checked;
		filter_model->set_modified_only(checked);
	});
	connect(core_chip, &QToolButton::toggled, this, [this, filter_model](const bool checked) {
		current_core_only = checked;
		filter_model->set_core_only(checked);
	});
	connect(show_modified_fields, &QToolButton::clicked, this, [modified_chip, core_chip, field_search, show_modified_fields]() {
		field_search->clear();
		if (modified_chip->isChecked() && !core_chip->isChecked()) {
			modified_chip->setChecked(false);
		} else {
			core_chip->setChecked(false);
			modified_chip->setChecked(true);
		}
		show_modified_fields->setChecked(modified_chip->isChecked() && !core_chip->isChecked());
	});
	connect(modified_chip, &QToolButton::toggled, show_modified_fields, [show_modified_fields, modified_chip, core_chip](bool) {
		show_modified_fields->setChecked(modified_chip->isChecked() && !core_chip->isChecked());
	});
	connect(core_chip, &QToolButton::toggled, show_modified_fields, [show_modified_fields, modified_chip, core_chip](bool) {
		show_modified_fields->setChecked(modified_chip->isChecked() && !core_chip->isChecked());
	});
	connect(clear_filter_state, &QToolButton::clicked, this, [field_search, section_filter, modified_chip, core_chip]() {
		field_search->clear();
		section_filter->setCurrentIndex(0);
		modified_chip->setChecked(false);
		core_chip->setChecked(false);
	});
	connect(filter_model, &QAbstractItemModel::modelReset, inspector_meta, update_inspector_meta);
	connect(filter_model, &QAbstractItemModel::rowsInserted, inspector_meta, [update_inspector_meta](const QModelIndex&, int, int) {
		update_inspector_meta();
	});
	connect(filter_model, &QAbstractItemModel::rowsRemoved, inspector_meta, [update_inspector_meta](const QModelIndex&, int, int) {
		update_inspector_meta();
	});
	connect(filter_model, &QAbstractItemModel::layoutChanged, inspector_meta, update_inspector_meta);
	connect(field_search, &QLineEdit::textChanged, inspector_meta, [update_inspector_meta](const QString&) {
		update_inspector_meta();
	});
	connect(section_filter, &QComboBox::currentIndexChanged, inspector_meta, [update_inspector_meta](int) {
		update_inspector_meta();
	});
	connect(modified_chip, &QToolButton::toggled, inspector_meta, [update_inspector_meta](bool) {
		update_inspector_meta();
	});
	connect(core_chip, &QToolButton::toggled, inspector_meta, [update_inspector_meta](bool) {
		update_inspector_meta();
	});
	const auto update_filter_state = [field_search, section_filter, modified_chip, core_chip, filter_state, clear_filter_state]() {
		QStringList parts;
		if (!field_search->text().trimmed().isEmpty()) {
			parts.push_back("Search: " + field_search->text().trimmed());
		}
		if (!section_filter->currentData().toString().isEmpty()) {
			parts.push_back("Section: " + section_filter->currentText());
		}
		if (modified_chip->isChecked()) {
			parts.push_back("Modified");
		}
		if (core_chip->isChecked()) {
			parts.push_back("Core");
		}

		const bool has_filters = !parts.isEmpty();
		filter_state->setText(has_filters ? parts.join(" - ") : "No active field filters");
		clear_filter_state->setVisible(has_filters);
	};
	update_filter_state();
	connect(field_search, &QLineEdit::textChanged, filter_state_bar, [update_filter_state](const QString&) {
		update_filter_state();
	});
	connect(section_filter, &QComboBox::currentIndexChanged, filter_state_bar, [update_filter_state](int) {
		update_filter_state();
	});
	connect(modified_chip, &QToolButton::toggled, filter_state_bar, [update_filter_state](bool) {
		update_filter_state();
	});
	connect(core_chip, &QToolButton::toggled, filter_state_bar, [update_filter_state](bool) {
		update_filter_state();
	});
	{
		const int category_index = section_filter->currentIndex();
		const QString value = section_filter->itemData(category_index).toString();
		for (auto* button : section_buttons) {
			button->setChecked(button->property("sectionValue").toString() == value);
		}
	}

	connect(view->selectionModel(), &QItemSelectionModel::currentChanged, this, [this, filter_model, single_model](const QModelIndex& current, const QModelIndex&) {
		if (!current.isValid()) {
			current_detail_key.clear();
			current_detail_level = -1;
			return;
		}

		const QModelIndex source_index = filter_model->mapToSource(current);
		if (!source_index.isValid()) {
			return;
		}

		const auto& mapping = single_model->getMapping();
		current_detail_key = mapping[source_index.row()].key;
		current_detail_level = mapping[source_index.row()].level;
	});
	connect(view, &QTableView::pressed, this, [this](const QModelIndex&) {
		refresh_details_focus_visuals();
	});

	if (!current_detail_key.empty()) {
		const int source_row = single_model->find_mapping_row(current_detail_key, current_detail_level);
		if (source_row >= 0) {
			const QModelIndex filtered_index = filter_model->mapFromSource(single_model->index(source_row, 0));
			if (filtered_index.isValid()) {
				view->setCurrentIndex(filtered_index);
				view->selectionModel()->select(filtered_index, QItemSelectionModel::ClearAndSelect | QItemSelectionModel::Rows);
			}
		}
	} else if (filter_model->rowCount() > 0) {
		const QModelIndex first_index = filter_model->index(0, 0);
		view->setCurrentIndex(first_index);
		view->selectionModel()->select(first_index, QItemSelectionModel::ClearAndSelect | QItemSelectionModel::Rows);
	}

	connect(view->verticalHeader(), &QHeaderView::sectionPressed, view, [view, filter_model](int section) {
		const QModelIndex index = filter_model->index(section, 0);
		view->setCurrentIndex(index);
		view->selectionModel()->select(index, QItemSelectionModel::ClearAndSelect | QItemSelectionModel::Rows);
		view->setFocus(Qt::FocusReason::MouseFocusReason);
	});

	QWidget* column_header = new QWidget;
	column_header->setObjectName("objectEditorColumnHeader");
	column_header->setSizePolicy(QSizePolicy::Preferred, QSizePolicy::Fixed);
	column_header->setStyleSheet(
		"#objectEditorColumnHeader {"
		"background: rgb(32, 38, 45);"
		"border-bottom: 1px solid rgba(255, 255, 255, 16);"
		"}"
	);

	QHBoxLayout* column_header_layout = new QHBoxLayout(column_header);
	column_header_layout->setContentsMargins(0, 0, 0, 0);
	column_header_layout->setSpacing(0);

	QLabel* name_header = new QLabel("Field");
	name_header->setAlignment(Qt::AlignLeft | Qt::AlignVCenter);
	name_header->setContentsMargins(8, 4, 8, 4);

	QFrame* header_separator = new QFrame;
	header_separator->setFrameShape(QFrame::VLine);
	header_separator->setFrameShadow(QFrame::Sunken);

	QLabel* value_header = new QLabel("Value");
	value_header->setAlignment(Qt::AlignLeft | Qt::AlignVCenter);
	value_header->setContentsMargins(8, 4, 8, 4);

	column_header_layout->addWidget(name_header);
	column_header_layout->addWidget(header_separator);
	column_header_layout->addWidget(value_header, 1);

	const auto sync_column_header = [view, name_header]() {
		name_header->setFixedWidth(view->verticalHeader()->width());
	};
	connect(view->verticalHeader(), &QHeaderView::geometriesChanged, column_header, sync_column_header);
	sync_column_header();

	QWidget* table_panel = new QWidget;
	QVBoxLayout* table_panel_layout = new QVBoxLayout(table_panel);
	table_panel_layout->setContentsMargins(0, 0, 0, 0);
	table_panel_layout->setSpacing(0);
	table_panel_layout->addWidget(column_header);
	table_panel_layout->addWidget(view, 1);

	QFrame* empty_state = new QFrame;
	empty_state->setObjectName("objectEditorEmptyState");
	QVBoxLayout* empty_state_layout = new QVBoxLayout(empty_state);
	empty_state_layout->setContentsMargins(24, 24, 24, 24);
	empty_state_layout->setSpacing(10);

	QLabel* empty_title = new QLabel("No fields match the current filters");
	empty_title->setObjectName("objectEditorEmptyTitle");

	QLabel* empty_meta = new QLabel("Try a broader search, switch sections, or clear the active filter chips.");
	empty_meta->setObjectName("objectEditorEmptyMeta");
	empty_meta->setWordWrap(true);

	QPushButton* clear_filters = new QPushButton("Clear Filters");
	clear_filters->setToolTip("Clear the current field search, section filter, and filter chips.");
	connect(clear_filters, &QPushButton::clicked, this, [field_search, section_filter, modified_chip, core_chip]() {
		field_search->clear();
		section_filter->setCurrentIndex(0);
		modified_chip->setChecked(false);
		core_chip->setChecked(false);
	});

	empty_state_layout->addStretch();
	empty_state_layout->addWidget(empty_title, 0, Qt::AlignHCenter);
	empty_state_layout->addWidget(empty_meta, 0, Qt::AlignHCenter);
	empty_state_layout->addWidget(clear_filters, 0, Qt::AlignHCenter);
	empty_state_layout->addStretch();

	QStackedWidget* inspector_stack = new QStackedWidget;
	inspector_stack->addWidget(table_panel);
	inspector_stack->addWidget(empty_state);
	inspector_stack->setCurrentWidget(table_panel);

	const auto update_inspector_state = [filter_model, inspector_stack, table_panel, empty_state]() {
		inspector_stack->setCurrentWidget(filter_model->rowCount() > 0 ? table_panel : empty_state);
	};
	update_inspector_state();
	connect(filter_model, &QAbstractItemModel::modelReset, inspector_stack, update_inspector_state);
	connect(filter_model, &QAbstractItemModel::rowsInserted, inspector_stack, [update_inspector_state](const QModelIndex&, int, int) {
		update_inspector_state();
	});
	connect(filter_model, &QAbstractItemModel::rowsRemoved, inspector_stack, [update_inspector_state](const QModelIndex&, int, int) {
		update_inspector_state();
	});
	connect(filter_model, &QAbstractItemModel::layoutChanged, inspector_stack, update_inspector_state);
	connect(field_search, &QLineEdit::textChanged, inspector_stack, [update_inspector_state](const QString&) {
		update_inspector_state();
	});
	connect(section_filter, &QComboBox::currentIndexChanged, inspector_stack, [update_inspector_state](int) {
		update_inspector_state();
	});
	connect(modified_chip, &QToolButton::toggled, inspector_stack, [update_inspector_state](bool) {
		update_inspector_state();
	});
	connect(core_chip, &QToolButton::toggled, inspector_stack, [update_inspector_state](bool) {
		update_inspector_state();
	});

	QFrame* field_selection_strip = new QFrame;
	field_selection_strip->setObjectName("objectEditorSelectionStrip");
	field_selection_strip->setSizePolicy(QSizePolicy::Preferred, QSizePolicy::Fixed);
	QHBoxLayout* field_selection_layout = new QHBoxLayout(field_selection_strip);
	field_selection_layout->setContentsMargins(10, 8, 10, 8);
	field_selection_layout->setSpacing(10);

	QVBoxLayout* field_selection_text = new QVBoxLayout;
	field_selection_text->setContentsMargins(0, 0, 0, 0);
	field_selection_text->setSpacing(2);

	QLabel* field_selection_title = new QLabel("No field selected");
	field_selection_title->setObjectName("objectEditorSelectionTitle");

	QLabel* field_selection_meta = new QLabel("Select a field to see its raw id and state.");
	field_selection_meta->setObjectName("objectEditorSelectionMeta");
	field_selection_meta->setWordWrap(true);

	field_selection_text->addWidget(field_selection_title);
	field_selection_text->addWidget(field_selection_meta);

	QLabel* field_state_badge = new QLabel("Base");
	field_state_badge->setObjectName("objectEditorPill");

	QToolButton* copy_field_id = new QToolButton;
	copy_field_id->setText("Copy Field ID");
	copy_field_id->setToolTip("Copy the selected field's raw metadata ID.");
	copy_field_id->setAutoRaise(true);
	copy_field_id->setEnabled(false);

	field_selection_layout->addLayout(field_selection_text, 1);
	field_selection_layout->addWidget(field_state_badge);
	field_selection_layout->addWidget(copy_field_id);

	const auto update_field_selection_strip = [filter_model, single_model, view, field_selection_title, field_selection_meta, field_state_badge, copy_field_id]() {
		const QModelIndex current = view->currentIndex();
		if (!current.isValid()) {
			field_selection_title->setText("No field selected");
			field_selection_meta->setText("Select a field to see its raw id and state.");
			field_state_badge->setText("Base");
			copy_field_id->setEnabled(false);
			copy_field_id->disconnect();
			return;
		}

		const QModelIndex source_index = filter_model->mapToSource(current);
		if (!source_index.isValid()) {
			return;
		}

		const QString title = single_model->display_label(source_index.row());
		const QString meta = single_model->meta_label(source_index.row());
		const bool modified = single_model->is_modified_row(source_index.row());
		const QString field_id = QString::fromStdString(single_model->getMapping()[source_index.row()].key);

		field_selection_title->setText(title);
		field_selection_meta->setText(meta);
		field_state_badge->setText(modified ? "Modified" : "Base");
		copy_field_id->setEnabled(true);
		copy_field_id->disconnect();
		QObject::connect(copy_field_id, &QToolButton::clicked, copy_field_id, [field_id]() {
			QApplication::clipboard()->setText(field_id);
		});
	};
	update_field_selection_strip();
	connect(view->selectionModel(), &QItemSelectionModel::currentChanged, field_selection_strip, [update_field_selection_strip](const QModelIndex&, const QModelIndex&) {
		update_field_selection_strip();
	});
	connect(filter_model, &QAbstractItemModel::modelReset, field_selection_strip, update_field_selection_strip);
	connect(filter_model, &QAbstractItemModel::layoutChanged, field_selection_strip, update_field_selection_strip);

	top_layout->addWidget(summary);
	top_layout->addWidget(bookmark_objects_bar);
	top_layout->addWidget(inspector_bar);
	top_layout->addWidget(section_strip);
	top_layout->addWidget(filter_state_bar);

	scroll_layout->addWidget(inspector_stack);
	scroll_layout->addWidget(field_selection_strip);
	scroll_layout->setStretch(0, 1);
	scroll_layout->setStretch(1, 0);

	QWidget* scroll_container = new QWidget;
	scroll_container->setObjectName("objectEditorDetailsScrollContainer");
	scroll_container->setAttribute(Qt::WA_StyledBackground, true);
	scroll_container->setLayout(scroll_layout);

	QScrollArea* area = new QScrollArea;
	area->setObjectName("objectEditorDetailsScrollArea");
	area->setStyleSheet(
		"QScrollArea#objectEditorDetailsScrollArea { background: rgb(24, 29, 35); border: none; }"
		"QScrollArea#objectEditorDetailsScrollArea > QWidget > QWidget { background: rgb(24, 29, 35); }"
	);
	area->setWidget(scroll_container);
	area->setWidgetResizable(true);

	QWidget* root = new QWidget;
	root->setObjectName("objectEditorDetailsRoot");
	root->setAttribute(Qt::WA_StyledBackground, true);
	QVBoxLayout* root_layout = new QVBoxLayout(root);
	root_layout->setContentsMargins(0, 0, 0, 0);
	root_layout->setSpacing(8);
	root_layout->addLayout(top_layout);
	root_layout->addWidget(area, 1);

	details_dock->setWidget(root);
	details_dock->setWindowTitle(name);
	details_dock->setIcon(icon);
	current_details_id = id;

	// Keep the user's current details scroll position when switching objects.
	// On first open, scroll past the ability insights section.
	if (current_details_scroll_y > 0) {
		area->verticalScrollBar()->setValue(current_details_scroll_y);
	} else {
		const int y = view->mapTo(scroll_container, QPoint(0, 0)).y();
		area->verticalScrollBar()->setValue(y);
	}
}

void ObjectEditor::addTypeTreeView(
	BaseTreeModel* treeModel,
	BaseFilter*& filter,
	TableModel* table,
	QTreeView* view,
	QIcon icon,
	QString name,
	Category category
) {
	treeModel->setSourceModel(table);
	filter = new BaseFilter;
	filter->slk = table->slk;
	filter->setRecursiveFilteringEnabled(true);
	filter->setFilterCaseSensitivity(Qt::CaseSensitivity::CaseInsensitive);
	filter->setDynamicSortFilter(true);
	filter->setSourceModel(treeModel);
	filter->sort(0, Qt::AscendingOrder);
	view->setModel(filter);
	view->header()->hide();
	view->header()->setStretchLastSection(true);
	view->header()->setSectionResizeMode(0, QHeaderView::Stretch);
	view->setContextMenuPolicy(Qt::CustomContextMenu);
	view->setItemDelegate(new ObjectTreeDelegate(view));
	view->setSelectionBehavior(QAbstractItemView::SelectRows);
	view->setSelectionMode(QAbstractItemView::ExtendedSelection);
	view->setEditTriggers(QAbstractItemView::EditKeyPressed);
	view->setUniformRowHeights(true);
	view->setIndentation(18);
	view->setIconSize({18, 18});
	view->setAnimated(true);
	view->setAllColumnsShowFocus(true);
	view->installEventFilter(this);
	view->viewport()->installEventFilter(this);
	view->setStyleSheet(
		"QTreeView {"
		"border: 1px solid rgba(255, 255, 255, 16);"
		"border-radius: 10px;"
		"background: rgb(24, 29, 35);"
		"padding: 4px 0;"
		"}"
		"QTreeView::item { padding: 2px 4px; }"
	);
	view->collapseAll();
	connect(view->selectionModel(), &QItemSelectionModel::selectionChanged, [=, this](const QItemSelection&, const QItemSelection&) {
		const QModelIndex current = view->selectionModel()->currentIndex();
		if (!current.isValid()) {
			return;
		}

		itemClicked(view, filter, table, current);
	});

	const std::function<bool(const QModelIndex&)> expand_custom_branch = [&](const QModelIndex& parent) -> bool {
		bool subtree_has_custom = false;

		for (int row = 0; row < filter->rowCount(parent); ++row) {
			const QModelIndex child = filter->index(row, 0, parent);
			if (!child.isValid()) {
				continue;
			}

			const auto* tree_item = static_cast<const BaseTreeItem*>(filter->mapToSource(child).internalPointer());
			if (!tree_item) {
				continue;
			}

			const bool child_has_custom = (tree_item->baseCategory || tree_item->subCategory) ? expand_custom_branch(child)
																								  : table->slk->shadow_data.contains(tree_item->id)
																										&& table->slk->shadow_data.at(tree_item->id).contains("oldid");

			if (child_has_custom) {
				subtree_has_custom = true;
				if (tree_item->customFolder) {
					view->expand(child);
				}
			}
		}

		return subtree_has_custom;
	};
	if (!restore_tree_state(name.toLower(), view)) {
		expand_custom_branch(QModelIndex());
	}

	const auto current_object_item = [view, filter]() -> const BaseTreeItem* {
		const QModelIndex current = view->currentIndex();
		if (!current.isValid()) {
			return nullptr;
		}

		return static_cast<const BaseTreeItem*>(filter->mapToSource(current).internalPointer());
	};

	QShortcut* delete_shortcut = new QShortcut(QKeySequence(Qt::Key_Delete), view);
	delete_shortcut->setContext(Qt::WidgetWithChildrenShortcut);
	connect(delete_shortcut, &QShortcut::activated, this, [this, view, filter, table, name, category]() {
		current_explorer_view = view;
		current_category = category;
		delete_selected_object_entries(view, filter, table, name);
	});

	connect(view, &QTreeView::customContextMenuRequested, [=, this](const QPoint& pos) {
		QMenu menu;
		QAction* addAction = menu.addAction("Add " + name);
		QAction* duplicateAction = menu.addAction("Duplicate " + name);
		QAction* copyAction = menu.addAction("Copy " + name);
		QAction* pasteAction = menu.addAction("Paste " + name);
		QAction* removeAction = menu.addAction("Remove " + name);

		if (category == Category::doodad) {
			QAction* convert_to_destructible = menu.addAction("Convert to destructible");

			{
				const auto selection = view->selectionModel()->selectedIndexes();
				if (selection.isEmpty()) {
					convert_to_destructible->setDisabled(true);
				} else {
					const auto treeItem = static_cast<BaseTreeItem*>(filter->mapToSource(selection.front()).internalPointer());
					if (treeItem->id.empty()) {
						convert_to_destructible->setDisabled(true);
					}
				}
			}

			connect(convert_to_destructible, &QAction::triggered, [=, this]() {
				QModelIndexList selection = view->selectionModel()->selectedIndexes();

				for (const auto& i : selection) {
					BaseTreeItem* treeItem = static_cast<BaseTreeItem*>(filter->mapToSource(i).internalPointer());
					if (treeItem->id.empty()) {
						continue;
					}
					std::string new_id;
					destructibles_table->addRow([&] {
						// We pick a barrel (LTbr) for no good reason
						new_id = convert_doodad_to_destructible(treeItem->id, "LTbr");
					});

					const auto index = destructibles_table->rowIDToIndex(new_id);
					const auto index2 = destructibleTreeModel->mapFromSource(index);
					const auto index3 = destructibleTreeFilter->mapFromSource(index2);
					itemClicked(destructible_explorer, destructibleTreeFilter, destructibles_table, index3);
				}
			});
		}

		QModelIndexList selection = view->selectionModel()->selectedIndexes();
		if (selection.empty()) {
			duplicateAction->setDisabled(true);
		} else {
			BaseTreeItem* treeItem = static_cast<BaseTreeItem*>(filter->mapToSource(selection.front()).internalPointer());
			if (treeItem->id.empty()) {
				duplicateAction->setDisabled(true);
			}
		}
		if (selection.empty()) {
			copyAction->setDisabled(true);
		} else {
			BaseTreeItem* treeItem = static_cast<BaseTreeItem*>(filter->mapToSource(selection.front()).internalPointer());
			if (treeItem->id.empty()) {
				copyAction->setDisabled(true);
			}
		}

		{
			const auto* mime = QApplication::clipboard()->mimeData();
			bool can_paste = false;
			if (mime && mime->hasFormat("application/x-hivewe-object-entry")) {
				const QJsonDocument document = QJsonDocument::fromJson(mime->data("application/x-hivewe-object-entry"));
				if (document.isObject()) {
					const QJsonObject payload = document.object();
					const std::string source_id = payload.value("id").toString().toStdString();
					can_paste = payload.value("category").toString() == category_key(category) && table->slk->row_headers.contains(source_id);
				}
			}
			pasteAction->setEnabled(can_paste);
		}

		if (selection.empty()) {
			removeAction->setDisabled(true);
		} else {
			BaseTreeItem* treeItem = static_cast<BaseTreeItem*>(filter->mapToSource(selection.front()).internalPointer());
			if (!table->slk->shadow_data.contains(treeItem->id) || !table->slk->shadow_data.at(treeItem->id).contains("oldid")) {
				removeAction->setDisabled(true);
			}
		}

		connect(copyAction, &QAction::triggered, [this, view, filter, category]() {
			copy_selected_object_entry(view, filter, category);
		});
		connect(duplicateAction, &QAction::triggered, [this, view, current_object_item, category, filter, treeModel, table, name]() {
			const auto* tree_item = current_object_item();
			if (!tree_item || tree_item->baseCategory || tree_item->subCategory) {
				return;
			}

			duplicate_object_entry(view, filter, treeModel, table, name, tree_item->id);
		});
		connect(pasteAction, &QAction::triggered, [this]() {
			paste_copied_object();
		});

		// add new object
		connect(addAction, &QAction::triggered, [=, this]() {
			QDialog* selectdialog = new QDialog(this, Qt::WindowTitleHint | Qt::WindowMaximizeButtonHint | Qt::WindowCloseButtonHint);
			selectdialog->resize(300, 560);
			selectdialog->setWindowModality(Qt::WindowModality::WindowModal);

			QLineEdit* nameEdit = new QLineEdit;
			nameEdit->setPlaceholderText("New name");
			nameEdit->setReadOnly(true);

			QLineEdit* id = new QLineEdit;
			id->setPlaceholderText("Free ID");
			id->setText(QString::fromStdString(next_sequential_custom_id(table)));
			id->setFont(QFont("consolas"));

			// error icon inside the id field, hidden by default
			QAction* idErrorIcon =
				id->addAction(selectdialog->style()->standardIcon(QStyle::SP_MessageBoxCritical), QLineEdit::TrailingPosition);
			idErrorIcon->setVisible(false);
			idErrorIcon->setToolTip("");

			QHBoxLayout* nameLayout = new QHBoxLayout;
			nameLayout->addWidget(nameEdit, 3);
			nameLayout->addWidget(id, 1);

			BaseFilter* sub_filter = new BaseFilter;
			sub_filter->slk = table->slk;
			sub_filter->setRecursiveFilteringEnabled(true);
			sub_filter->setFilterCaseSensitivity(Qt::CaseSensitivity::CaseInsensitive);
			sub_filter->setSourceModel(treeModel);

			QLineEdit* search = new QLineEdit;
			search->setPlaceholderText("Search " + name);

			QTreeView* sub_view = new QTreeView;
			sub_view->setModel(sub_filter);
			sub_view->setUniformRowHeights(true);
			sub_view->header()->hide();
			sub_view->expandAll();

			QDialogButtonBox* buttonBox = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
			connect(buttonBox, &QDialogButtonBox::accepted, selectdialog, &QDialog::accept);
			connect(buttonBox, &QDialogButtonBox::rejected, selectdialog, &QDialog::reject);
			buttonBox->button(QDialogButtonBox::Ok)->setEnabled(false);

			QVBoxLayout* selectlayout = new QVBoxLayout(selectdialog);
			selectlayout->addLayout(nameLayout);
			selectlayout->addWidget(search);
			selectlayout->addWidget(sub_view);
			selectlayout->addWidget(buttonBox);

			// validation lambda to check if input is valid
			auto is_valid = [sub_view, sub_filter, table, id, buttonBox, idErrorIcon]() {
				std::string new_id = id->text().toStdString();
				bool wrong_size = new_id.size() != 4;
				bool is_duplicate = table->slk->row_headers.contains(new_id);

				// check if an actual object (not a folder) is selected
				bool object_selected = false;
				QModelIndex current = sub_view->selectionModel()->currentIndex();
				if (current.isValid()) {
					const BaseTreeItem* treeItem = static_cast<BaseTreeItem*>(sub_filter->mapToSource(current).internalPointer());
					object_selected = !treeItem->baseCategory && !treeItem->subCategory;
				}

				// update UI based on validation
				if (wrong_size) {
					idErrorIcon->setVisible(true);
					idErrorIcon->setToolTip("ID must be exactly 4 characters long.");
					id->setStyleSheet("QLineEdit { border: 1px solid #e74c3c; }");
					buttonBox->button(QDialogButtonBox::Ok)->setEnabled(false);
				} else if (is_duplicate) {
					idErrorIcon->setVisible(true);
					idErrorIcon->setToolTip(QString::fromStdString(std::format("ID '{}' is already in use.", new_id)));
					id->setStyleSheet("QLineEdit { border: 1px solid #e74c3c; }");
					buttonBox->button(QDialogButtonBox::Ok)->setEnabled(false);
				} else if (!object_selected) {
					idErrorIcon->setVisible(false);
					id->setStyleSheet("");
					buttonBox->button(QDialogButtonBox::Ok)->setEnabled(false);
				} else {
					// all valid
					idErrorIcon->setVisible(false);
					id->setStyleSheet("");
					buttonBox->button(QDialogButtonBox::Ok)->setEnabled(true);
				}
			};

			connect(search, &QLineEdit::textChanged, [=](const QString& string) {
				sub_filter->setFilterFixedString(string);
				sub_view->expandAll();
			});

			connect(
				sub_view->selectionModel(),
				&QItemSelectionModel::currentChanged,
				[table, sub_filter, filter, id, nameEdit, is_valid](const QModelIndex& current, const QModelIndex& previous) {
					if (!current.isValid()) {
						return;
					}
					nameEdit->setText(sub_filter->data(current).toString());
					const BaseTreeItem* treeItem = static_cast<BaseTreeItem*>(sub_filter->mapToSource(current).internalPointer());
					if (!(treeItem->baseCategory || treeItem->subCategory)) {
						id->setText(QString::fromStdString(next_sequential_custom_id(table, treeItem->id)));
					}
					is_valid();
				}
			);

			// id input field
			connect(id, &QLineEdit::textChanged, [is_valid](const QString& text) {
				is_valid();
			});

			auto select = [view, table, sub_filter, filter, selectdialog, id, treeModel](const QModelIndex& index) {
				if (id->text().size() != 4) {
					return;
				}

				// check if another object with selected ID already exists
				std::string new_id = id->text().toStdString();
				if (table->slk->row_headers.contains(new_id)) {
					return;
				}

				const BaseTreeItem* treeItem = static_cast<BaseTreeItem*>(sub_filter->mapToSource(index).internalPointer());
				if (treeItem->baseCategory || treeItem->subCategory) {
					return;
				}

				selectdialog->close();
				table->copyRow(treeItem->id, id->text().toStdString());

				QModelIndex new_index = filter->mapFromSource(treeModel->mapFromSource(table->index(table->rowCount() - 1, 0)));
				view->setCurrentIndex(new_index);
				view->scrollTo(new_index, QAbstractItemView::ScrollHint::PositionAtCenter);
			};

			connect(sub_view, &QTreeView::activated, [select](const QModelIndex& index) {
				select(index);
			});

			connect(selectdialog, &QDialog::accepted, [sub_view, select]() {
				auto indices = sub_view->selectionModel()->selectedIndexes();
				if (indices.empty()) {
					return;
				}

				select(indices.front());
			});

			connect(new QShortcut(QKeySequence(Qt::CTRL | Qt::Key_F), selectdialog), &QShortcut::activated, [=]() {
				search->setFocus();
				search->selectAll();
			});

			selectdialog->show();
			search->setFocus();
		});

		connect(removeAction, &QAction::triggered, [=, this]() {
			delete_selected_object_entries(view, filter, table, name);
		});

		menu.exec(view->mapToGlobal(pos));
	});

	QLineEdit* search = new QLineEdit;
	search->setObjectName("search");
	search->setProperty("class", "fieldSearch");
	search->setClearButtonEnabled(true);
	search->setPlaceholderText("Search " + name);
	search->setText(browser_searches[static_cast<int>(category)]);

	QToolButton* all_objects = new QToolButton;
	all_objects->setObjectName("objectEditorChip");
	all_objects->setText("All");
	all_objects->setToolTip("Show all objects in this browser.");
	all_objects->setCheckable(true);

	QToolButton* custom_objects = new QToolButton;
	custom_objects->setObjectName("objectEditorChip");
	custom_objects->setText("Custom");
	custom_objects->setToolTip("Show only custom " + name.toLower() + " objects.");
	custom_objects->setCheckable(true);
	custom_objects->setChecked(browser_custom_only[static_cast<int>(category)]);
	all_objects->setChecked(!browser_custom_only[static_cast<int>(category)]);

	QButtonGroup* mode_group = new QButtonGroup(view);
	mode_group->setExclusive(true);
	mode_group->addButton(all_objects);
	mode_group->addButton(custom_objects);
	connect(custom_objects, &QToolButton::toggled, [=, this](bool checked) {
		browser_custom_only[static_cast<int>(category)] = checked;
		filter->setFilterCustom(checked);
		if (!checked) {
			view->expandAll();
		}
	});

	QLabel* browser_meta = new QLabel(object_count_label(table));
	browser_meta->setObjectName("objectEditorBrowserMeta");

	QWidget* browser_bar = new QWidget;
	browser_bar->setObjectName("objectEditorBrowserBar");
	QVBoxLayout* browser_layout = new QVBoxLayout(browser_bar);
	browser_layout->setContentsMargins(10, 10, 10, 8);
	browser_layout->setSpacing(8);

	QHBoxLayout* browser_title_row = new QHBoxLayout;
	browser_title_row->setContentsMargins(0, 0, 0, 0);
	browser_title_row->setSpacing(8);

	QLabel* browser_icon = new QLabel;
	browser_icon->setPixmap(icon.pixmap(18, 18));
	browser_icon->setFixedSize(20, 20);
	browser_icon->setAlignment(Qt::AlignCenter);

	QLabel* browser_title = new QLabel(name);
	browser_title->setObjectName("objectEditorBrowserTitle");

	browser_title_row->addWidget(browser_icon);
	browser_title_row->addWidget(browser_title);
	browser_title_row->addStretch();
	browser_title_row->addWidget(browser_meta);

	QHBoxLayout* browser_controls_row = new QHBoxLayout;
	browser_controls_row->setContentsMargins(0, 0, 0, 0);
	browser_controls_row->setSpacing(8);
	browser_controls_row->addWidget(search, 1);
	browser_controls_row->addWidget(all_objects);
	browser_controls_row->addWidget(custom_objects);

	QToolButton* focus_current = new QToolButton;
	focus_current->setText("Reveal");
	focus_current->setToolTip("Scroll the tree to the current selection and focus it.");
	focus_current->setAutoRaise(true);
	connect(focus_current, &QToolButton::clicked, view, [view]() {
		const QModelIndex current = view->currentIndex();
		if (current.isValid()) {
			view->scrollTo(current, QAbstractItemView::ScrollHint::PositionAtCenter);
			view->setFocus(Qt::FocusReason::OtherFocusReason);
		}
	});

	QToolButton* clear_browser_state = new QToolButton;
	clear_browser_state->setText("Reset");
	clear_browser_state->setToolTip("Clear the current browser search and custom-only filter.");
	clear_browser_state->setAutoRaise(true);
	connect(clear_browser_state, &QToolButton::clicked, this, [search, all_objects, custom_objects]() {
		search->clear();
		custom_objects->setChecked(false);
		all_objects->setChecked(true);
	});

	browser_title_row->addWidget(focus_current);

	browser_controls_row->addWidget(clear_browser_state);

	browser_layout->addLayout(browser_title_row);
	browser_layout->addLayout(browser_controls_row);

	QFrame* browser_empty = new QFrame;
	browser_empty->setObjectName("objectEditorEmptyState");
	QVBoxLayout* browser_empty_layout = new QVBoxLayout(browser_empty);
	browser_empty_layout->setContentsMargins(24, 24, 24, 24);
	browser_empty_layout->setSpacing(10);

	QLabel* browser_empty_title = new QLabel("No objects match the current browser filters");
	browser_empty_title->setObjectName("objectEditorEmptyTitle");

	QLabel* browser_empty_meta = new QLabel("Try a broader search or disable the custom-only filter.");
	browser_empty_meta->setObjectName("objectEditorEmptyMeta");
	browser_empty_meta->setWordWrap(true);

	QPushButton* clear_browser_filters = new QPushButton("Clear Browser Filters");
	clear_browser_filters->setToolTip("Clear the current browser search and custom-only filter.");
	connect(clear_browser_filters, &QPushButton::clicked, this, [search, all_objects, custom_objects]() {
		search->clear();
		custom_objects->setChecked(false);
		all_objects->setChecked(true);
	});

	browser_empty_layout->addStretch();
	browser_empty_layout->addWidget(browser_empty_title, 0, Qt::AlignHCenter);
	browser_empty_layout->addWidget(browser_empty_meta, 0, Qt::AlignHCenter);
	browser_empty_layout->addWidget(clear_browser_filters, 0, Qt::AlignHCenter);
	browser_empty_layout->addStretch();

	QStackedWidget* browser_stack = new QStackedWidget;
	browser_stack->setObjectName("objectEditorBrowserStack");
	browser_stack->addWidget(view);
	browser_stack->addWidget(browser_empty);

	QFrame* browser_selection_strip = new QFrame;
	browser_selection_strip->setObjectName("objectEditorSelectionStrip");
	browser_selection_strip->setSizePolicy(QSizePolicy::Preferred, QSizePolicy::Fixed);
	QHBoxLayout* browser_selection_layout = new QHBoxLayout(browser_selection_strip);
	browser_selection_layout->setContentsMargins(10, 8, 10, 8);
	browser_selection_layout->setSpacing(10);

	QLabel* browser_selection_icon = new QLabel;
	browser_selection_icon->setFixedSize(24, 24);
	browser_selection_icon->setAlignment(Qt::AlignCenter);

	QVBoxLayout* browser_selection_text = new QVBoxLayout;
	browser_selection_text->setContentsMargins(0, 0, 0, 0);
	browser_selection_text->setSpacing(2);

	QLabel* browser_selection_title = new QLabel("No object selected");
	browser_selection_title->setObjectName("objectEditorSelectionTitle");

	QLabel* browser_selection_meta = new QLabel("Select an object to preview its raw ID and editor state.");
	browser_selection_meta->setObjectName("objectEditorSelectionMeta");
	browser_selection_meta->setWordWrap(true);

	browser_selection_text->addWidget(browser_selection_title);
	browser_selection_text->addWidget(browser_selection_meta);

	QLabel* browser_selection_badge = new QLabel("Base");
	browser_selection_badge->setObjectName("objectEditorPill");

	QToolButton* copy_object_id = new QToolButton;
	copy_object_id->setText("Copy ID");
	copy_object_id->setToolTip("Copy the selected object's raw ID to the clipboard.");
	copy_object_id->setAutoRaise(true);
	copy_object_id->setEnabled(false);

	browser_selection_layout->addWidget(browser_selection_icon);
	browser_selection_layout->addLayout(browser_selection_text, 1);
	browser_selection_layout->addWidget(browser_selection_badge);
	browser_selection_layout->addWidget(copy_object_id);

	const auto update_browser_meta = [table, filter, browser_meta, custom_objects]() {
		browser_meta->setText(filtered_object_count_label(table, filter, custom_objects->isChecked()));
	};
	const auto update_browser_state = [filter, browser_stack, view, browser_empty]() {
		browser_stack->setCurrentWidget(visible_object_count(filter) > 0 ? view : browser_empty);
	};
	connect(search, &QLineEdit::textChanged, [=, this](const QString& string) {
		browser_searches[static_cast<int>(category)] = string;
		filter->setFilterFixedString(string);
		view->expandAll();
	});

	const auto update_browser_selection_strip = [view, filter, table, browser_selection_icon, browser_selection_title, browser_selection_meta, browser_selection_badge, copy_object_id]() {
		const QModelIndex current = view->currentIndex();
		if (!current.isValid()) {
			browser_selection_icon->clear();
			browser_selection_title->setText("No object selected");
			browser_selection_meta->setText("Select an object to preview its raw ID and editor state.");
			browser_selection_badge->setText("Base");
			copy_object_id->setEnabled(false);
			copy_object_id->disconnect();
			return;
		}

		const auto* tree_item = static_cast<const BaseTreeItem*>(filter->mapToSource(current).internalPointer());
		if (!tree_item || tree_item->baseCategory || tree_item->subCategory) {
			browser_selection_icon->clear();
			browser_selection_title->setText("Folder selected");
			browser_selection_meta->setText("Choose an object row to preview its id and status.");
			browser_selection_badge->setText("Folder");
			copy_object_id->setEnabled(false);
			copy_object_id->disconnect();
			return;
		}

		const QString raw_id = QString::fromStdString(tree_item->id);
		const bool custom = table->slk->shadow_data.contains(tree_item->id) && table->slk->shadow_data.at(tree_item->id).contains("oldid");
		browser_selection_icon->setPixmap(current.data(Qt::DecorationRole).value<QIcon>().pixmap(22, 22));
		browser_selection_title->setText(current.data(Qt::DisplayRole).toString());
		browser_selection_meta->setText(raw_id + (custom ? "  -  Custom object" : "  -  Base object"));
		browser_selection_badge->setText(custom ? "Custom" : "Base");
		copy_object_id->setEnabled(true);
		copy_object_id->disconnect();
		QObject::connect(copy_object_id, &QToolButton::clicked, copy_object_id, [raw_id]() {
			QApplication::clipboard()->setText(raw_id);
		});
	};

	QToolBar* bar = new QToolBar;
	bar->setMovable(false);
	bar->addWidget(browser_bar);

	QWidget* browser_container = new QWidget;
	browser_container->setObjectName("objectEditorBrowserContainer");
	browser_container->setAttribute(Qt::WA_StyledBackground, true);
	QVBoxLayout* browser_container_layout = new QVBoxLayout(browser_container);
	browser_container_layout->setContentsMargins(0, 0, 0, 0);
	browser_container_layout->setSpacing(8);
	browser_container_layout->addWidget(browser_selection_strip);
	browser_container_layout->addWidget(browser_stack, 1);

	ads::CDockWidget* tab = new ads::CDockWidget(dock_manager, name);
	tab->setToolBar(bar);
	tab->setWidget(browser_container);
	tab->setFeature(ads::CDockWidget::DockWidgetClosable, false);
	tab->setFeature(ads::CDockWidget::DockWidgetAlwaysCloseAndDelete, false);
	tab->setIcon(icon);
	if (explorer_area == nullptr) {
		explorer_area = dock_manager->addDockWidget(ads::LeftDockWidgetArea, tab);
	} else {
		dock_manager->addDockWidget(ads::CenterDockWidgetArea, tab, explorer_area);
	}

	if (explorer_area != nullptr) {
		if (auto* title_bar = explorer_area->titleBar()) {
			title_bar->setAttribute(Qt::WA_StyledBackground, true);
			title_bar->setStyleSheet(
				"ads--CDockAreaTitleBar {"
				"background: rgb(32, 38, 45);"
				"border-bottom: 1px solid rgba(255, 255, 255, 16);"
				"}"
			);

			if (auto* tab_bar = title_bar->tabBar()) {
				tab_bar->setAttribute(Qt::WA_StyledBackground, true);
				tab_bar->viewport()->setAutoFillBackground(true);

				QPalette viewport_palette = tab_bar->viewport()->palette();
				viewport_palette.setColor(QPalette::Window, QColor(32, 38, 45));
				tab_bar->viewport()->setPalette(viewport_palette);

				tab_bar->setStyleSheet(
					"ads--CDockAreaTabBar {"
					"background: rgb(32, 38, 45);"
					"border: none;"
					"}"
					"ads--CDockAreaTabBar > QWidget {"
					"background: rgb(32, 38, 45);"
					"}"
					"ads--CDockAreaTabBar > QWidget > QWidget {"
					"background: rgb(32, 38, 45);"
					"}"
				);
			}
		}
	}

	filter->setFilterCustom(custom_objects->isChecked());
	filter->setFilterFixedString(search->text());
	update_browser_meta();
	update_browser_state();
	update_browser_selection_strip();
	connect(filter, &QAbstractItemModel::modelReset, browser_meta, update_browser_meta);
	connect(filter, &QAbstractItemModel::modelReset, browser_stack, update_browser_state);
	connect(filter, &QAbstractItemModel::modelReset, browser_selection_strip, update_browser_selection_strip);
	connect(filter, &QAbstractItemModel::rowsInserted, browser_meta, [update_browser_meta](const QModelIndex&, int, int) {
		update_browser_meta();
	});
	connect(filter, &QAbstractItemModel::rowsInserted, browser_stack, [update_browser_state](const QModelIndex&, int, int) {
		update_browser_state();
	});
	connect(filter, &QAbstractItemModel::rowsRemoved, browser_meta, [update_browser_meta](const QModelIndex&, int, int) {
		update_browser_meta();
	});
	connect(filter, &QAbstractItemModel::rowsRemoved, browser_stack, [update_browser_state](const QModelIndex&, int, int) {
		update_browser_state();
	});
	connect(filter, &QAbstractItemModel::layoutChanged, browser_meta, update_browser_meta);
	connect(filter, &QAbstractItemModel::layoutChanged, browser_stack, update_browser_state);
	connect(filter, &QAbstractItemModel::layoutChanged, browser_selection_strip, update_browser_selection_strip);
	connect(search, &QLineEdit::textChanged, browser_meta, [update_browser_meta](const QString&) {
		update_browser_meta();
	});
	connect(search, &QLineEdit::textChanged, browser_stack, [update_browser_state](const QString&) {
		update_browser_state();
	});
	connect(search, &QLineEdit::textChanged, browser_selection_strip, [update_browser_selection_strip](const QString&) {
		update_browser_selection_strip();
	});
	connect(custom_objects, &QToolButton::toggled, browser_meta, [update_browser_meta](bool) {
		update_browser_meta();
	});
	connect(custom_objects, &QToolButton::toggled, browser_stack, [update_browser_state](bool) {
		update_browser_state();
	});
	connect(custom_objects, &QToolButton::toggled, browser_selection_strip, [update_browser_selection_strip](bool) {
		update_browser_selection_strip();
	});
	connect(view->selectionModel(), &QItemSelectionModel::currentChanged, browser_selection_strip, [update_browser_selection_strip](const QModelIndex&, const QModelIndex&) {
		update_browser_selection_strip();
	});
}

void ObjectEditor::select_id(Category category, const std::string& id) {
	explorer_area->setCurrentIndex(static_cast<int>(category));
	current_category = category;
	const auto edit = explorer_area->currentDockWidget()->findChild<QLineEdit*>("search");
	edit->clear();

	switch (category) {
		case Category::unit: {
			const auto index = unitTreeFilter->mapFromSource(unitTreeModel->getIdIndex(id));
			unit_explorer->setCurrentIndex(index);
			unit_explorer->scrollTo(index, QAbstractItemView::ScrollHint::PositionAtCenter);
			emit unit_explorer->clicked(index);
			break;
		}
		case Category::item: {
			const auto index = itemTreeFilter->mapFromSource(itemTreeModel->getIdIndex(id));
			item_explorer->setCurrentIndex(index);
			item_explorer->scrollTo(index, QAbstractItemView::ScrollHint::PositionAtCenter);
			emit item_explorer->clicked(index);
			break;
		}
		case Category::doodad: {
			const auto index = doodadTreeFilter->mapFromSource(doodadTreeModel->getIdIndex(id));
			doodad_explorer->setCurrentIndex(index);
			doodad_explorer->scrollTo(index, QAbstractItemView::ScrollHint::PositionAtCenter);
			emit doodad_explorer->clicked(index);
			break;
		}
		case Category::destructible: {
			const auto index = destructibleTreeFilter->mapFromSource(destructibleTreeModel->getIdIndex(id));
			destructible_explorer->setCurrentIndex(index);
			destructible_explorer->scrollTo(index, QAbstractItemView::PositionAtCenter);
			emit destructible_explorer->clicked(index);
			break;
		}
		case Category::ability: {
			const auto index = abilityTreeFilter->mapFromSource(abilityTreeModel->getIdIndex(id));
			ability_explorer->setCurrentIndex(index);
			ability_explorer->scrollTo(index, QAbstractItemView::ScrollHint::PositionAtCenter);
			emit ability_explorer->clicked(index);
			break;
		}
		case Category::upgrade: {
			const auto index = upgradeTreeFilter->mapFromSource(upgradeTreeModel->getIdIndex(id));
			upgrade_explorer->setCurrentIndex(index);
			upgrade_explorer->scrollTo(index, QAbstractItemView::ScrollHint::PositionAtCenter);
			emit upgrade_explorer->clicked(index);
			break;
		}
		case Category::buff: {
			const auto index = buffTreeFilter->mapFromSource(buffTreeModel->getIdIndex(id));
			buff_explorer->setCurrentIndex(index);
			buff_explorer->scrollTo(index, QAbstractItemView::ScrollHint::PositionAtCenter);
			emit buff_explorer->clicked(index);
			break;
		}
	}
}

void ObjectEditor::keyPressEvent(QKeyEvent* event) {
	if (event->key() == Qt::Key_Shift && !event->isAutoRepeat()) {
		if (double_shift_timer.isValid() && double_shift_timer.elapsed() < 400) {
			new GlobalSearchWidget(this);
			double_shift_timer.invalidate();
		} else {
			double_shift_timer.start();
		}
	}

	QMainWindow::keyPressEvent(event);
}

bool ObjectEditor::eventFilter(QObject* watched, QEvent* event) {
	if (event->type() == QEvent::KeyPress || event->type() == QEvent::ShortcutOverride) {
		auto* key_event = static_cast<QKeyEvent*>(event);
		QWidget* focus_widget = QApplication::focusWidget();
		if (is_text_input_widget(focus_widget)) {
			return QMainWindow::eventFilter(watched, event);
		}

		const bool copy_key = key_event->matches(QKeySequence::Copy)
						   || (key_event->key() == Qt::Key_C && key_event->modifiers().testFlag(Qt::ControlModifier));
		const bool paste_key = key_event->matches(QKeySequence::Paste)
							|| (key_event->key() == Qt::Key_V && key_event->modifiers().testFlag(Qt::ControlModifier));
		const bool browser_key = copy_key || paste_key || key_event->key() == Qt::Key_Delete || key_event->key() == Qt::Key_F2;

		if (event->type() == QEvent::ShortcutOverride && browser_key && explorer_area && focus_widget
			&& is_focus_within(focus_widget, explorer_area->currentDockWidget())) {
			key_event->accept();
			return true;
		}

		const auto category_for_view = [this](QTreeView* view) {
			return view == unit_explorer		 ? Category::unit
				 : view == item_explorer		 ? Category::item
				 : view == doodad_explorer		 ? Category::doodad
				 : view == destructible_explorer ? Category::destructible
				 : view == ability_explorer		 ? Category::ability
				 : view == upgrade_explorer		 ? Category::upgrade
												 : Category::buff;
		};

		if (browser_key && explorer_area && focus_widget && is_focus_within(focus_widget, explorer_area->currentDockWidget())) {
			if (QTreeView* view = explorer_area->currentDockWidget()->findChild<QTreeView*>()) {
				const Category category = category_for_view(view);
				current_explorer_view = view;
				current_category = category;

				if (copy_key) {
					if (auto* filter = filter_for_category(category)) {
						copy_selected_object_entry(view, filter, category);
						key_event->accept();
						return true;
					}
				} else if (paste_key) {
					paste_copied_object();
					key_event->accept();
					return true;
				} else if (key_event->key() == Qt::Key_Delete) {
					delete_selected_object_entries(view, filter_for_category(category), table_for_category(category), category_label(category));
					key_event->accept();
					return true;
				} else if (key_event->key() == Qt::Key_F2) {
					begin_rename_selected_object();
					key_event->accept();
					return true;
				}
			}
		}

		if (current_details_view && is_focus_within(focus_widget, current_details_view)) {
			if (key_event->matches(QKeySequence::Copy)) {
				copy_selected_field_to_clipboard(current_details_view);
				key_event->accept();
				return true;
			}
			if (key_event->matches(QKeySequence::Paste)) {
				paste_selected_field_from_clipboard(current_details_view);
				key_event->accept();
				return true;
			}
		}
	}

	return QMainWindow::eventFilter(watched, event);
}

bool ObjectEditor::focusNextPrevChild(const bool next) {
	QWidget* focus_widget = QApplication::focusWidget();
	if (current_explorer_view && current_details_view) {
		if (is_focus_within(focus_widget, current_explorer_view)) {
			current_details_view->setFocus(next ? Qt::TabFocusReason : Qt::BacktabFocusReason);
			return true;
		}

		if (is_focus_within(focus_widget, current_details_view)) {
			current_explorer_view->setFocus(next ? Qt::TabFocusReason : Qt::BacktabFocusReason);
			return true;
		}
	}

	return QMainWindow::focusNextPrevChild(next);
}

