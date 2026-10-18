---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/models/single_model.cpp"
ref: HEAD
---

#include "single_model.h"

#include "model_view.h"

#include <QPainter>
#include <QPlainTextEdit>
#include <QDialogButtonBox>
#include <QTimer>
#include <QSpinBox>
#include <QCheckBox>
#include <QListWidget>
#include <QPushButton>
#include <QTreeView>
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QSortFilterProxyModel>
#include <QMouseEvent>
#include <QTimer>
#include <QApplication>
#include <QRegularExpressionValidator>

#include "object_editor/icon_view.h"

import std;
import BaseTreeModel;
import AbilityTreeModel;
import BuffTreeModel;
import UpgradeTreeModel;
import UnitTreeModel;
import UnitSelector;
import Utilities;
import Globals;

class DownwardComboBox : public QComboBox {
  public:
	using QComboBox::QComboBox;

  protected:
	void mousePressEvent(QMouseEvent* event) override {
		QComboBox::mousePressEvent(event);
		QTimer::singleShot(0, this, [this]() {
			showPopup();
		});
	}

	void showPopup() override {
		setStyleSheet("QComboBox { combobox-popup: 0; }");
		QComboBox::showPopup();
	}
};

SingleModel::SingleModel(TableModel* table, QObject* parent) : QAbstractProxyModel(parent) {
	slk = table->slk;
	meta_slk = table->meta_slk;
	setSourceModel(table);
	connect(this, &SingleModel::dataChanged, [this](const auto& index) {
		if (!index.isValid()) {
			return;
		}
		if (id_mapping[index.row()].field == "levels" || id_mapping[index.row()].field == "maxlevel"
			|| id_mapping[index.row()].field == "numvar") {
			buildMapping();
		}
	});
}

QModelIndex SingleModel::mapFromSource(const QModelIndex& sourceIndex) const {
	if (!sourceIndex.isValid()) {
		return {};
	}

	if (sourceIndex.row() != slk->row_headers.at(id)) {
		std::print("Invalid ID for SLK {}\n", id);
		return {};
	}

	const std::string& field = slk->index_to_column.at(sourceIndex.column());
	for (size_t i = 0; i < id_mapping.size(); i++) {
		if (id_mapping[i].field == field) {
			//std::print("Found {} at {} {} {}\n", field, i, headerData(i, Qt::Vertical, Qt::DisplayRole).toString().toStdString(), meta_slk->data("displayname", id_mapping[i].key));
			return createIndex(i, 0);
		}
	}
	std::print("Not found {}\t{}\n", sourceIndex.row(), sourceIndex.column());
	return {};
}

QModelIndex SingleModel::mapToSource(const QModelIndex& proxyIndex) const {
	if (!proxyIndex.isValid()) {
		return {};
	}

	const std::string& column = id_mapping[proxyIndex.row()].field;
	return sourceModel()->index(slk->row_headers.at(id), slk->column_headers.at(column));
}

QVariant SingleModel::data(const QModelIndex& index, int role) const {
	if (role != Qt::ForegroundRole) {
		if (index.column() == 1) {
			switch (role) {
				case Qt::DisplayRole:
				case Qt::EditRole:
					return QString::fromStdString(meta_slk->data("description", id_mapping[index.row()].key));
				case Qt::CheckStateRole:
					return {};
				default:
					return QAbstractProxyModel::data(index, role);
			}
		} else {
			return QAbstractProxyModel::data(index, role);
		}
	}

	if (slk->shadow_data.contains(id) && slk->shadow_data.at(id).contains(id_mapping[index.row()].field)) {
		return QColor("violet");
	}

	return {};
}

QVariant SingleModel::headerData(int section, Qt::Orientation orientation, int role) const {
	if (orientation == Qt::Orientation::Vertical) {
		if (role == Qt::DisplayRole || role == DisplayNameRole) {
			return display_label(section);
		} else if (role == CategoryRole) {
			return category_label(section);
		} else if (role == MetaRole) {
			return meta_label(section);
		} else if (role == ModifiedRole) {
			return is_modified_row(section);
		} else if (role == CategoryStartRole) {
			return starts_new_category(section);
		} else if (role == CommonRole) {
			return is_common_row(section);
		} else if (role == Qt::ForegroundRole) {
			if (is_modified_row(section)) {
				return QColor("violet");
			} else {
				return QColor("white");
			}
		}
	} else if (role == Qt::DisplayRole) {
		return "Value";
	} else if (role == Qt::ForegroundRole) {
		if (orientation == Qt::Orientation::Vertical) {
			return is_modified_row(section) ? QColor("violet") : QColor("white");
		}
	}
	return QAbstractProxyModel::headerData(section, orientation, role);
}

bool SingleModel::setData(const QModelIndex& index, const QVariant& value, int role) {
	// Force a header redraw because the header text should change color depending on whether the data field is a custom value or not
	emit headerDataChanged(Qt::Orientation::Vertical, index.row(), index.row());
	return QAbstractProxyModel::setData(index, value, role);
}

int SingleModel::rowCount(const QModelIndex& parent) const {
	return id_mapping.size();
}

int SingleModel::columnCount(const QModelIndex& parent) const {
	if (meta_slk->column_headers.contains("description")) {
		return 2;
	} else {
		return 1;
	}
}

QModelIndex SingleModel::index(int row, int column, const QModelIndex& parent) const {
	return createIndex(row, column);
}

QModelIndex SingleModel::parent(const QModelIndex& child) const {
	return QModelIndex();
}

void SingleModel::setSourceModel(QAbstractItemModel* sourceModel) {
	beginResetModel();

	if (this->sourceModel()) {
		disconnect(sourceModel, &QAbstractItemModel::dataChanged, this, &SingleModel::sourceDataChanged);
	}

	QAbstractProxyModel::setSourceModel(sourceModel);

	connect(sourceModel, &QAbstractItemModel::dataChanged, this, &SingleModel::sourceDataChanged);

	endResetModel();
}

std::string SingleModel::getID() const {
	return id;
}

void SingleModel::setID(const std::string newID) {
	id = newID;
	buildMapping();
}

int SingleModel::find_mapping_row(const std::string& key, const int level) const {
	for (int i = 0; i < id_mapping.size(); ++i) {
		if (id_mapping[i].key == key && id_mapping[i].level == level) {
			return i;
		}
	}

	return -1;
}

QString SingleModel::category_label(const int row) const {
	const std::string_view category = world_edit_data.data<std::string_view>(
		"ObjectEditorCategories",
		meta_slk->data<std::string_view>("category", id_mapping[row].key)
	);
	return QString::fromStdString(string_replaced(category, "&", ""));
}

QString SingleModel::display_label(const int row) const {
	QString label = QString::fromUtf8(meta_slk->data<std::string_view>("displayname", id_mapping[row].key));
	if (label.isEmpty()) {
		label = QString::fromStdString(id_mapping[row].field);
	}
	return label;
}

QString SingleModel::meta_label(const int row) const {
	QStringList parts;
	const QString category = category_label(row);
	if (!category.isEmpty()) {
		parts.push_back(category);
	}
	if (id_mapping[row].level > 0) {
		parts.push_back("Level " + QString::number(id_mapping[row].level));
	}
	parts.push_back(QString::fromStdString(id_mapping[row].key));
	return parts.join("  •  ");
}

bool SingleModel::is_modified_row(const int row) const {
	return slk->shadow_data.contains(id) && slk->shadow_data.at(id).contains(id_mapping[row].field);
}

bool SingleModel::starts_new_category(const int row) const {
	return row == 0 || category_label(row - 1) != category_label(row);
}

bool SingleModel::is_common_row(const int row) const {
	const QString category = category_label(row);
	const QString display = display_label(row).toLower();

	if (is_modified_row(row)) {
		return true;
	}

	if (category == "Stats" || category == "Text" || category == "Techtree" || category == "Art") {
		return true;
	}

	return display.contains("name") || display.contains("tooltip") || display.contains("hotkey") || display.contains("mana")
		|| display.contains("cooldown") || display.contains("icon") || display.contains("requirements");
}

void SingleModel::buildMapping() {
	beginResetModel();
	id_mapping.clear();

	for (const auto& [key, index] : meta_slk->row_headers) {
		// Unit an ability SLKs contain usespecific, but only abilities use `usespecific`, the cell is always empty for units.
		const std::string_view use_specific = meta_slk->data<std::string_view>("usespecific", key);
		if (!use_specific.empty()) {
			std::string_view id_to_check = id;
			if (slk->shadow_data.contains(id) && slk->shadow_data.at(id).contains("oldid")) {
				id_to_check = slk->shadow_data.at(id).at("oldid");
			}

			// For abilities we also have to check the one this might be aliasing
			const std::string_view aliasing_ability = slk->data<std::string_view>("code", id);
			if (!use_specific.contains(id_to_check) && !use_specific.contains(aliasing_ability)) {
				continue;
			}
		}

		const std::string_view not_specific = meta_slk->data<std::string_view>("notspecific", key);
		if (!not_specific.empty()) {
			std::string_view id_to_check = id;
			if (slk->shadow_data.contains(id) && slk->shadow_data.at(id).contains("oldid")) {
				id_to_check = slk->shadow_data.at(id).at("oldid");
			}

			// For abilities we also have to check the one this might be aliasing
			const std::string_view aliasing_ability = slk->data<std::string_view>("code", id);
			if (not_specific.contains(id_to_check) && not_specific.contains(aliasing_ability)) {
				continue;
			}
		}

		std::string field_name = to_lowercase_copy(meta_slk->data<std::string_view>("field", key));
		if (meta_slk->column_headers.contains("data") && meta_slk->data<int>("data", key) > 0) {
			field_name += static_cast<char>('a' + (meta_slk->data<int>("data", key) - 1));
		}

		if (meta_slk->column_headers.contains("repeat") && meta_slk->data<int>("repeat", key) > 0) {
			int iterations = 1;
			if (slk->column_headers.contains("levels")) {
				iterations = slk->data<int>("levels", id);
			} else if (slk->column_headers.contains("numvar")) {
				iterations = slk->data<int>("numvar", id);
			} else if (slk->column_headers.contains("maxlevel")) {
				iterations = slk->data<int>("maxlevel", id);
			}
			iterations = std::clamp(iterations, 0, 64);

			for (int i = 0; i < iterations; i++) {
				std::string new_field_name;
				if (meta_slk->column_headers.contains("appendindex") && meta_slk->data<int>("appendindex", key) > 0) {
					if (i == 0) {
						new_field_name = field_name;
					} else {
						new_field_name = field_name + std::to_string(i);
					}
				} else {
					new_field_name = field_name + std::to_string(i + 1);
				}

				// We add a virtual column if it does not exist in the base table
				if (!slk->column_headers.contains(new_field_name)) {
					slk->add_column(new_field_name);
				}

				id_mapping.push_back({key, new_field_name, i + 1});
			}
		} else {
			// We add a virtual column if it does not exist in the base table
			if (!slk->column_headers.contains(field_name)) {
				slk->add_column(field_name);
			}

			id_mapping.push_back({key, field_name, 0});
		}
	}

	std::ranges::sort(id_mapping, [&](const auto& left, const auto& right) {
		auto normalize = [](std::string text) {
			std::erase(text, '&');
			return to_lowercase_copy(text);
		};

		const std::string left_display = std::string(meta_slk->data<std::string_view>("displayname", left.key));
		const std::string right_display = std::string(meta_slk->data<std::string_view>("displayname", right.key));
		const std::string left_category =
			std::string(world_edit_data.data<std::string_view>("ObjectEditorCategories", meta_slk->data<std::string_view>("category", left.key)));
		const std::string right_category =
			std::string(world_edit_data.data<std::string_view>("ObjectEditorCategories", meta_slk->data<std::string_view>("category", right.key)));

		const std::string left_group = normalize(left_category);
		const std::string right_group = normalize(right_category);
		if (left_group != right_group) {
			return left_group < right_group;
		}

		const std::string left_name = normalize(left_display);
		const std::string right_name = normalize(right_display);
		if (left_name != right_name) {
			return left_name < right_name;
		}

		if (left.level != right.level) {
			return left.level < right.level;
		}

		return left.field < right.field;
	});
	endResetModel();
}

void SingleModel::sourceDataChanged(const QModelIndex& topLeft, const QModelIndex& bottomRight, const QVector<int>& roles) {
	Q_ASSERT(topLeft.isValid() ? topLeft.model() == sourceModel() : true);
	Q_ASSERT(bottomRight.isValid() ? bottomRight.model() == sourceModel() : true);

	for (size_t i = topLeft.row(); i < bottomRight.row(); i++) {
		if (i == slk->row_headers.at(id)) {
			const auto top_left = mapFromSource(createIndex(i, topLeft.column()));
			const auto bottom_right = mapFromSource(createIndex(i, bottomRight.column()));
			emit dataChanged(top_left, bottom_right, roles);
			return;
		}
	}
}

const SingleModel* SingleModelFilter::single_model() const {
	return qobject_cast<const SingleModel*>(sourceModel());
}

bool SingleModelFilter::filterAcceptsRow(const int source_row, const QModelIndex& source_parent) const {
	const auto* model = single_model();
	if (!model) {
		return true;
	}

	if (!category_filter.isEmpty() && model->category_label(source_row) != category_filter) {
		return false;
	}

	if (modified_only && !model->is_modified_row(source_row)) {
		return false;
	}

	if (core_only && !model->is_common_row(source_row)) {
		return false;
	}

	if (field_search.isEmpty()) {
		return true;
	}

	const QString haystack = (model->display_label(source_row) + " " + model->meta_label(source_row) + " "
		+ model->data(model->index(source_row, 0), Qt::DisplayRole).toString())
							   .toLower();
	return haystack.contains(field_search);
}

QVariant SingleModelFilter::headerData(const int section, const Qt::Orientation orientation, const int role) const {
	if (orientation == Qt::Vertical) {
		if (const auto* model = single_model()) {
			const QModelIndex source_index = mapToSource(index(section, 0));
			if (source_index.isValid()) {
				return model->headerData(source_index.row(), orientation, role);
			}
		}
	}

	return QSortFilterProxyModel::headerData(section, orientation, role);
}

void SingleModelFilter::set_field_search(const QString& search) {
	beginFilterChange();
	field_search = search.trimmed().toLower();
	endFilterChange(Direction::Rows);
}

void SingleModelFilter::set_category_filter(const QString& category) {
	beginFilterChange();
	category_filter = category.trimmed();
	endFilterChange(Direction::Rows);
}

void SingleModelFilter::set_modified_only(const bool enabled) {
	beginFilterChange();
	modified_only = enabled;
	endFilterChange(Direction::Rows);
}

void SingleModelFilter::set_core_only(const bool enabled) {
	beginFilterChange();
	core_only = enabled;
	endFilterChange(Direction::Rows);
}

/// Manually color the headers because the default QHeaderView will only alternatively color the items
void AlterHeader::paintSection(QPainter* painter, const QRect& rect, int logicalIndex) const {
	bool is_selected = false;
	bool has_focus = false;

	if (const auto* item_view = qobject_cast<const QAbstractItemView*>(parentWidget())) {
		if (const auto* selection_model = item_view->selectionModel()) {
			is_selected = selection_model->isRowSelected(logicalIndex, QModelIndex()) || selection_model->currentIndex().row() == logicalIndex;
		}
		if (QWidget* focus_widget = QApplication::focusWidget()) {
			has_focus = focus_widget == item_view || item_view->isAncestorOf(focus_widget);
		}
	}

	const QPalette::ColorGroup color_group = has_focus ? QPalette::Active : QPalette::Inactive;
	painter->fillRect(rect, palette().color(color_group, is_selected ? QPalette::Highlight : QPalette::Base));

	const QString title = model()->headerData(logicalIndex, orientation(), SingleModel::DisplayNameRole).toString();
	const QString meta = model()->headerData(logicalIndex, orientation(), SingleModel::MetaRole).toString();
	const bool category_start = model()->headerData(logicalIndex, orientation(), SingleModel::CategoryStartRole).toBool();
	const QColor text_color = is_selected ? palette().color(color_group, QPalette::HighlightedText)
										  : model()->headerData(logicalIndex, orientation(), Qt::ForegroundRole).value<QColor>();
	const QColor meta_color = is_selected ? palette().color(color_group, QPalette::HighlightedText).lighter(120)
										  : palette().color(QPalette::Disabled, QPalette::Text);
	const int margin = 2 * style()->pixelMetric(QStyle::PM_HeaderMargin, nullptr, this);
	const QRect content_rect = rect.adjusted(margin, category_start ? 28 : 8, -8, -4);

	if (category_start) {
		const QString category = model()->headerData(logicalIndex, orientation(), SingleModel::CategoryRole).toString().toUpper();
		QFont category_font = font();
		category_font.setBold(true);
		category_font.setPointSize(std::max(7, category_font.pointSize() - 1));
		painter->setFont(category_font);
		painter->setPen(QPen(palette().color(QPalette::Disabled, QPalette::Text)));
		painter->drawText(rect.adjusted(margin, 4, -8, 0), Qt::AlignLeft | Qt::AlignTop, category);
		painter->setPen(QPen(QColor(255, 255, 255, 30)));
		painter->drawLine(rect.left(), rect.top() + 1, rect.right() + 1, rect.top() + 1);
	}

	QFont title_font = font();
	title_font.setBold(is_selected);
	painter->setFont(title_font);
	painter->setPen(QPen(text_color));
	painter->drawText(content_rect, Qt::AlignLeft | Qt::AlignTop, title);

	QFont meta_font = font();
	meta_font.setPointSize(std::max(7, meta_font.pointSize() - 1));
	painter->setFont(meta_font);
	painter->setPen(QPen(meta_color));
	painter->drawText(content_rect.adjusted(0, 22, 0, 0), Qt::AlignLeft | Qt::AlignTop, meta);
	painter->setPen(QPen(palette().color(QPalette::Mid)));
	painter->drawLine(rect.x(), rect.bottom(), rect.right(), rect.bottom());
}

QSize AlterHeader::sectionSizeFromContents(const int logicalIndex) const {
	QSize size = QHeaderView::sectionSizeFromContents(logicalIndex);
	size.setHeight(68);
	return size;
}

TableDelegate::TableDelegate(SingleModel* single_model, QWidget* parent) : QStyledItemDelegate(parent), single_model(single_model) {}

void TableDelegate::paint(QPainter* painter, const QStyleOptionViewItem& option, const QModelIndex& index) const {
	QStyleOptionViewItem opt(option);
	initStyleOption(&opt, index);

	if (const auto* item_view = qobject_cast<const QAbstractItemView*>(option.widget)) {
		if (item_view->currentIndex() == index) {
			if (QWidget* focus_widget = QApplication::focusWidget();
				focus_widget && focus_widget != item_view->viewport() && item_view->isAncestorOf(focus_widget)) {
				const QRect editor_rect = QRect(focus_widget->mapTo(item_view->viewport(), QPoint(0, 0)), focus_widget->size());
				if (editor_rect.intersects(opt.rect)) {
					painter->save();
					painter->fillRect(opt.rect, opt.state.testFlag(QStyle::State_Selected) ? opt.palette.highlight() : opt.palette.base());
					painter->restore();
					return;
				}
			}
		}
	}

	const bool is_modified = index.model()->headerData(index.row(), Qt::Vertical, SingleModel::ModifiedRole).toBool();
	const bool category_start = index.model()->headerData(index.row(), Qt::Vertical, SingleModel::CategoryStartRole).toBool();
	const bool selected = opt.state.testFlag(QStyle::State_Selected);
	const QRect full_rect = opt.rect;

	if (is_modified && !selected) {
		opt.backgroundBrush = QColor(84, 39, 96, 55);
	}

	if (category_start) {
		opt.rect.adjust(0, 28, 0, 0);
	}

	QStyledItemDelegate::paint(painter, opt, index);

	painter->save();
	if (is_modified) {
		painter->fillRect(QRect(full_rect.left(), full_rect.top(), 3, full_rect.height()), selected ? QColor(255, 255, 255, 180) : QColor(214, 112, 255, 180));
	}

	if (category_start) {
		painter->setPen(QPen(QColor(255, 255, 255, 30)));
		painter->drawLine(full_rect.topLeft(), full_rect.topRight());
	}
	painter->restore();
}

QSize TableDelegate::sizeHint(const QStyleOptionViewItem& option, const QModelIndex& index) const {
	QSize size = QStyledItemDelegate::sizeHint(option, index);
	QVariant decoration = index.data(Qt::DecorationRole);
	const bool has_icon = decoration.canConvert<QIcon>();
	const bool category_start = index.model()->headerData(index.row(), Qt::Vertical, SingleModel::CategoryStartRole).toBool();

	if (!has_icon) {
		size.rheight() += category_start ? 38 : 8;
	} else {
		size.rheight() += category_start ? 28 : 2;
	}

	return size;
}

QWidget* TableDelegate::createEditor(QWidget* parent, const QStyleOptionViewItem&, const QModelIndex& index) const {
	auto transformed_index = index;
	if (index.model() != single_model) {
		transformed_index = dynamic_cast<const QSortFilterProxyModel*>(index.model())->mapToSource(index);
	}
	const auto& mapping = single_model->getMapping();

	const std::string type = single_model->meta_slk->data("type", mapping[transformed_index.row()].key);
	const std::string minVal = single_model->meta_slk->data("minval", mapping[transformed_index.row()].key);
	const std::string maxVal = single_model->meta_slk->data("maxval", mapping[transformed_index.row()].key);

	if (type == "int") {
		QSpinBox* editor = new QSpinBox(parent);
		editor->setMinimum(INT_MIN);
		editor->setMaximum(INT_MAX);
		// handle empty minVal, maxVal
		// editor->setMinimum(std::stoi(minVal));
		// editor->setMaximum(std::stoi(maxVal));
		// editor->setSingleStep(std::clamp((std::stoi(maxVal) - std::stoi(minVal)) / 10, 1, 10));
		return editor;
	} else if (type == "real" || type == "unreal") {
		QDoubleSpinBox* editor = new QDoubleSpinBox(parent);
		editor->setMinimum(-99999999999.0);
		editor->setMaximum(99999999999.0);
		// editor->setMinimum(std::stod(minVal));
		// editor->setMaximum(std::stod(maxVal));
		// editor->setSingleStep(std::clamp((std::stod(maxVal) - std::stod(minVal)) / 10.0, 0.1, 10.0));
		return editor;
	} else if (type == "string") {
		QTextEdit* editor = new QTextEdit(parent);
		//		editor->setMaxLength(std::stoi(maxVal));
		return editor;
	} else if (type == "hotkey") {
		return create_hotkey_editor(parent);
	} else if (type == "model") {
		return create_model_editor(parent);
	} else if (type == "targetList") {
		return create_target_list_editor(parent);
	} else if (type == "unitList") {
		return create_unit_list_editor(parent);
	} else if (type == "upgradeList") {
		return create_upgrade_list_editor(parent);
	} else if (type == "abilityList" || type == "heroAbilityList" || type == "abilitySkinList") {
		return create_ability_list_editor(parent);
	} else if (type == "buffList") {
		return create_buff_list_editor(parent);
	} else if (type.ends_with("Flags") && unit_editor_data.section_exists(type)) {
		return create_flags_editor(parent, type);
	} else if (type.ends_with("List")) {
		return create_list_editor(parent);
	} else if (unit_editor_data.section_exists(type)) {
		DownwardComboBox* editor = new DownwardComboBox(parent);
		for (const auto& [key, value] : unit_editor_data.section(type)) {
			if (key == "NumValues" || key == "Sort" || key.ends_with("_Alt")) {
				continue;
			}

			QString displayText = QString::fromStdString(value[1]);
			displayText.replace('&', "");

			editor->addItem(displayText, QString::fromStdString(value[0]));
		}
		return editor;
	} else if (type == "icon") {
		return create_icon_editor(parent);
	} else if (type == "doodadCategory") {
		DownwardComboBox* editor = new DownwardComboBox(parent);
		for (const auto& [key, value] : world_edit_data.section("DoodadCategories")) {
			editor->addItem(QString::fromStdString(value[0]), QString::fromStdString(key));
		}
		return editor;
	} else if (type == "destructableCategory") {
		DownwardComboBox* editor = new DownwardComboBox(parent);
		for (const auto& [key, value] : world_edit_data.section("DestructibleCategories")) {
			editor->addItem(QString::fromStdString(value[0]), QString::fromStdString(key));
		}
		return editor;
	} else {
		return new QLineEdit(parent);
	}
}

void TableDelegate::setEditorData(QWidget* editor, const QModelIndex& index) const {
	auto transformed_index = index;
	if (index.model() != single_model) {
		transformed_index = dynamic_cast<const QSortFilterProxyModel*>(index.model())->mapToSource(index);
	}
	const std::string_view type =
		single_model->meta_slk->data<std::string_view>("type", single_model->getMapping()[transformed_index.row()].key);

	if (type == "int") {
		dynamic_cast<QSpinBox*>(editor)->setValue(index.data(Qt::EditRole).toInt());
	} else if (type == "real" || type == "unreal") {
		dynamic_cast<QDoubleSpinBox*>(editor)->setValue(index.data(Qt::EditRole).toDouble());
	} else if (type == "string") {
		// A hack to resolve TRIGSTR. The downside of taking the Display value is that we overwrite the TRIGSTR reference
		dynamic_cast<QTextEdit*>(editor)->setText(index.data(Qt::DisplayRole).toString());
	} else if (type == "hotkey") {
		dynamic_cast<QLineEdit*>(editor)->setText(index.data(Qt::DisplayRole).toString().trimmed().left(1).toUpper());
	} else if (type == "targetList") {
		const auto parts = index.data(Qt::EditRole).toString().split(',');
		for (const auto& i : parts) {
			QCheckBox* box = editor->findChild<QCheckBox*>(i);
			if (box) {
				box->setChecked(true);
			}
		}
	} else if (type.ends_with("Flags") && unit_editor_data.section_exists(type)) {
		const int mask = index.data(Qt::EditRole).toInt();
		for (auto* box : editor->findChildren<QCheckBox*>()) {
			const int bit = 1 << box->property("flagValue").toInt();
			box->setChecked((mask & bit) != 0);
		}
	} else if (type == "model") {
		ModelView* list = editor->findChild<ModelView*>("modelView");
		list->set_current_model_path(index.data(Qt::EditRole).toString());
	} else if (type == "unitList") {
		QListWidget* list = editor->findChild<QListWidget*>("unitList");

		const auto ids = index.data(Qt::EditRole).toString().split(',', Qt::SkipEmptyParts);
		for (const auto& id : ids) {
			QListWidgetItem* item = new QListWidgetItem;
			item->setText(units_table->data(id.toStdString(), "name").toString());
			item->setIcon(units_table->data(id.toStdString(), "art", Qt::DecorationRole).value<QIcon>());
			item->setData(Qt::StatusTipRole, id);
			list->addItem(item);
		}
	} else if (type == "upgradeList") {
		QListWidget* list = editor->findChild<QListWidget*>("upgradeList");

		const auto ids = index.data(Qt::EditRole).toString().split(',', Qt::SkipEmptyParts);
		for (const auto& id : ids) {
			QListWidgetItem* item = new QListWidgetItem;
			item->setText(upgrade_table->data(id.toStdString(), "name1").toString());
			item->setIcon(upgrade_table->data(id.toStdString(), "art1", Qt::DecorationRole).value<QIcon>());
			item->setData(Qt::StatusTipRole, id);
			list->addItem(item);
		}
	} else if (type == "abilityList" || type == "heroAbilityList" || type == "abilitySkinList") {
		QListWidget* list = editor->findChild<QListWidget*>("abilityList");

		const auto ids = index.data(Qt::EditRole).toString().split(',', Qt::SkipEmptyParts);
		for (const auto& id : ids) {
			QListWidgetItem* item = new QListWidgetItem;
			item->setText(abilities_table->data(id.toStdString(), "name").toString());
			item->setIcon(abilities_table->data(id.toStdString(), "art", Qt::DecorationRole).value<QIcon>());
			item->setData(Qt::StatusTipRole, id);
			list->addItem(item);
		}
	} else if (type == "buffList") {
		QListWidget* list = editor->findChild<QListWidget*>("buffList");

		const auto ids = index.data(Qt::EditRole).toString().split(',', Qt::SkipEmptyParts);
		for (const auto& id : ids) {
			const std::string raw_id = id.toStdString();
			QString name = buff_table->data(raw_id, "editorname").toString();
			if (name.isEmpty()) {
				name = buff_table->data(raw_id, "bufftip").toString();
			}

			QListWidgetItem* item = new QListWidgetItem;
			item->setText(name);
			item->setIcon(buff_table->data(raw_id, "buffart", Qt::DecorationRole).value<QIcon>());
			item->setData(Qt::StatusTipRole, id);
			list->addItem(item);
		}
	} else if (type == "stringList") {
		// A hack to resolve TRIGSTR. The downside of taking the Display value is that we overwrite the TRIGSTR reference
		editor->findChild<QPlainTextEdit*>("editor")->setPlainText(index.data(Qt::DisplayRole).toString());
	} else if (type.ends_with("List")) {
		editor->findChild<QPlainTextEdit*>("editor")->setPlainText(index.data(Qt::EditRole).toString());
	} else if (unit_editor_data.section_exists(type)) {
		const auto combo = dynamic_cast<QComboBox*>(editor);
		// Find the item with the right userdata and set it as current index
		for (int i = 0; i < combo->count(); i++) {
			if (combo->itemData(i, Qt::UserRole).toString() == index.data(Qt::EditRole).toString()) {
				combo->setCurrentIndex(i);
			}
		}
		QTimer::singleShot(0, combo, [combo]() {
			combo->showPopup();
		});
	} else if (type == "icon") {
		IconView* list = editor->findChild<IconView*>("iconView");
		list->setCurrentIconPath(index.data(Qt::EditRole).toString());
	} else if (type == "doodadCategory") {
		const auto combo = dynamic_cast<QComboBox*>(editor);
		for (int i = 0; i < combo->count(); i++) {
			if (combo->itemData(i, Qt::UserRole).toString() == index.data(Qt::EditRole).toString()) {
				combo->setCurrentIndex(i);
			}
		}
		QTimer::singleShot(0, combo, [combo]() {
			combo->showPopup();
		});
	} else if (type == "destructableCategory") {
		const auto combo = dynamic_cast<QComboBox*>(editor);
		for (int i = 0; i < combo->count(); i++) {
			if (combo->itemData(i, Qt::UserRole).toString() == index.data(Qt::EditRole).toString()) {
				combo->setCurrentIndex(i);
			}
		}
		QTimer::singleShot(0, combo, [combo]() {
			combo->showPopup();
		});
	} else {
		dynamic_cast<QLineEdit*>(editor)->setText(index.data(Qt::EditRole).toString());
	}
}

void TableDelegate::setModelData(QWidget* editor, QAbstractItemModel* model, const QModelIndex& index) const {
	auto transformed_index = index;
	if (model != single_model) {
		transformed_index = dynamic_cast<QSortFilterProxyModel*>(model)->mapToSource(index);
	}
	const std::string_view type =
		single_model->meta_slk->data<std::string_view>("type", single_model->getMapping()[transformed_index.row()].key);

	if (type == "int") {
		model->setData(index, static_cast<QSpinBox*>(editor)->value());
	} else if (type == "real" || type == "unreal") {
		model->setData(index, static_cast<QDoubleSpinBox*>(editor)->value());
	} else if (type == "string") {
		auto text = static_cast<QTextEdit*>(editor)->toPlainText();
		text.replace('\n', "|n");
		model->setData(index, text, Qt::EditRole);
	} else if (type == "hotkey") {
		model->setData(index, static_cast<QLineEdit*>(editor)->text().trimmed().left(1).toUpper(), Qt::EditRole);
	} else if (type == "model") {
		ModelView* list = editor->findChild<ModelView*>("modelView");
		model->setData(index, list->current_model_path());
	} else if (type == "unitList") {
		QListWidget* list = editor->findChild<QListWidget*>("unitList");

		QString result;
		for (int i = 0; i < list->count(); i++) {
			QListWidgetItem* item = list->item(i);
			if (!result.isEmpty()) {
				result += ',';
			}
			result += item->data(Qt::StatusTipRole).toString();
		}
		model->setData(index, result);
	} else if (type == "abilityList" || type == "heroAbilityList" || type == "abilitySkinList") {
		QListWidget* list = editor->findChild<QListWidget*>("abilityList");

		QString result;
		for (int i = 0; i < list->count(); i++) {
			QListWidgetItem* item = list->item(i);
			if (!result.isEmpty()) {
				result += ',';
			}
			result += item->data(Qt::StatusTipRole).toString();
		}
		model->setData(index, result);
	} else if (type == "buffList") {
		QListWidget* list = editor->findChild<QListWidget*>("buffList");

		QString result;
		for (int i = 0; i < list->count(); i++) {
			QListWidgetItem* item = list->item(i);
			if (!result.isEmpty()) {
				result += ',';
			}
			result += item->data(Qt::StatusTipRole).toString();
		}
		model->setData(index, result);
	} else if (type == "upgradeList") {
		QListWidget* list = editor->findChild<QListWidget*>("upgradeList");

		QString result;
		for (int i = 0; i < list->count(); i++) {
			QListWidgetItem* item = list->item(i);
			if (!result.isEmpty()) {
				result += ',';
			}
			result += item->data(Qt::StatusTipRole).toString();
		}
		model->setData(index, result);
	} else if (type == "targetList") {
		QString result;
		for (const auto& [key, value] : unit_editor_data.section(type)) {
			if (key == "NumValues" || key == "Sort" || key.ends_with("_Alt")) {
				continue;
			}

			QCheckBox* box = editor->findChild<QCheckBox*>(QString::fromStdString(value[0]));
			if (box && box->isChecked()) {
				if (!result.isEmpty()) {
					result += ",";
				}
				result += QString::fromStdString(value[0]);
			}
		}
		model->setData(index, result, Qt::EditRole);
	} else if (type.ends_with("Flags") && unit_editor_data.section_exists(type)) {
		int result = 0;
		for (auto* box : editor->findChildren<QCheckBox*>()) {
			if (box->isChecked()) {
				result |= 1 << box->property("flagValue").toInt();
			}
		}
		model->setData(index, result, Qt::EditRole);
	} else if (type.ends_with("List")) {
		model->setData(index, editor->findChild<QPlainTextEdit*>("editor")->toPlainText());
	} else if (unit_editor_data.section_exists(type)) {
		auto combo = static_cast<QComboBox*>(editor);
		model->setData(index, combo->currentData());
	} else if (type == "icon") {
		IconView* list = editor->findChild<IconView*>("iconView");
		model->setData(index, list->currentIconPath());
	} else if (type == "doodadCategory") {
		auto combo = static_cast<QComboBox*>(editor);
		model->setData(index, combo->currentData());
	} else if (type == "destructableCategory") {
		auto combo = static_cast<QComboBox*>(editor);
		model->setData(index, combo->currentData());
	} else {
		model->setData(index, static_cast<QLineEdit*>(editor)->text());
	}
}

void TableDelegate::updateEditorGeometry(QWidget* editor, const QStyleOptionViewItem& option, const QModelIndex&) const {
	if (dynamic_cast<QDialog*>(editor)) {
	} else {
		editor->setGeometry(option.rect);
	}
}

QWidget* TableDelegate::create_list_editor(QWidget* parent) const {
	auto editor = new QWidget(parent);

	QDialog* dialog = new QDialog(editor, Qt::WindowTitleHint | Qt::WindowMaximizeButtonHint | Qt::WindowCloseButtonHint);
	dialog->setAttribute(Qt::WA_DeleteOnClose);
	dialog->setWindowModality(Qt::WindowModality::WindowModal);

	QVBoxLayout* layout = new QVBoxLayout(dialog);

	QPlainTextEdit* text_edit = new QPlainTextEdit;
	text_edit->setObjectName("editor");

	QDialogButtonBox* buttonBox = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
	connect(buttonBox, &QDialogButtonBox::accepted, dialog, &QDialog::accept);
	connect(buttonBox, &QDialogButtonBox::rejected, dialog, &QDialog::reject);

	layout->addWidget(text_edit);
	layout->addWidget(buttonBox);

	connect(dialog, &QDialog::accepted, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->commitData(editor);
		emit delegate->closeEditor(editor);
	});

	connect(dialog, &QDialog::rejected, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->closeEditor(editor);
	});

	dialog->show();

	return editor;
}

QWidget* TableDelegate::create_model_editor(QWidget* parent) const {
	auto editor = new QWidget(parent);

	QDialog* dialog = new QDialog(editor, Qt::WindowTitleHint | Qt::WindowMaximizeButtonHint | Qt::WindowCloseButtonHint);
	dialog->setAttribute(Qt::WA_DeleteOnClose);
	dialog->resize(1200, 720);
	dialog->setWindowModality(Qt::WindowModality::WindowModal);

	// Force native window, otherwise ModelView causes switch to OpenGL-capable
	// surface which unmaps the window and shows white flash.
	dialog->setAttribute(Qt::WA_NativeWindow);
	(void)dialog->winId();

	ModelView* view = new ModelView();
	view->setObjectName("modelView");

	QDialogButtonBox* buttonBox = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
	connect(buttonBox, &QDialogButtonBox::accepted, dialog, &QDialog::accept);
	connect(buttonBox, &QDialogButtonBox::rejected, dialog, &QDialog::reject);

	connect(dialog, &QDialog::accepted, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->commitData(editor);
		emit delegate->closeEditor(editor);
	});

	connect(dialog, &QDialog::rejected, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->closeEditor(editor);
	});

	QVBoxLayout* layout = new QVBoxLayout(dialog);
	layout->addWidget(view);
	layout->addWidget(buttonBox);

	dialog->show();

	return editor;
}

QWidget* TableDelegate::create_hotkey_editor(QWidget* parent) const {
	QLineEdit* editor = new QLineEdit(parent);
	editor->setMaxLength(1);
	editor->setValidator(new QRegularExpressionValidator(QRegularExpression("[A-Za-z]?"), editor));
	editor->setAlignment(Qt::AlignCenter);
	editor->setClearButtonEnabled(true);
	connect(editor, &QLineEdit::textChanged, editor, [editor](const QString& text) {
		const QString upper = text.left(1).toUpper();
		if (text != upper) {
			editor->setText(upper);
		}
	});
	return editor;
}

QWidget* TableDelegate::create_target_list_editor(QWidget* parent) const {
	auto editor = new QWidget(parent);

	QDialog* dialog = new QDialog(editor, Qt::WindowTitleHint | Qt::WindowMaximizeButtonHint | Qt::WindowCloseButtonHint);
	dialog->setAttribute(Qt::WA_DeleteOnClose);
	dialog->setWindowModality(Qt::WindowModality::WindowModal);

	QVBoxLayout* layout = new QVBoxLayout(dialog);
	QGridLayout* flow = new QGridLayout;

	for (const auto& [key, value] : unit_editor_data.section("targetList")) {
		if (key == "NumValues" || key == "Sort" || key.ends_with("_Alt")) {
			continue;
		}

		QString displayText = QString::fromStdString(value[1]);
		displayText.replace('&', "");

		QCheckBox* flag = new QCheckBox(displayText);
		flag->setObjectName(QString::fromStdString(value[0]));

		flow->addWidget(flag);
	}

	QDialogButtonBox* buttonBox = new QDialogButtonBox;
	buttonBox->setStandardButtons(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
	connect(buttonBox, &QDialogButtonBox::accepted, dialog, &QDialog::accept);
	connect(buttonBox, &QDialogButtonBox::rejected, dialog, &QDialog::reject);

	layout->addLayout(flow);
	layout->addWidget(buttonBox);

	connect(dialog, &QDialog::accepted, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->commitData(editor);
		emit delegate->closeEditor(editor);
	});

	connect(dialog, &QDialog::rejected, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->closeEditor(editor);
	});

	dialog->show();

	return editor;
}

QWidget* TableDelegate::create_flags_editor(QWidget* parent, std::string_view type) const {
	auto editor = new QWidget(parent);

	QDialog* dialog = new QDialog(editor, Qt::WindowTitleHint | Qt::WindowMaximizeButtonHint | Qt::WindowCloseButtonHint);
	dialog->setAttribute(Qt::WA_DeleteOnClose);
	dialog->setWindowModality(Qt::WindowModality::WindowModal);

	QVBoxLayout* layout = new QVBoxLayout(dialog);
	QGridLayout* flow = new QGridLayout;

	int row = 0;
	for (const auto& [key, value] : unit_editor_data.section(type)) {
		if (key == "NumValues" || key == "Sort" || key.ends_with("_Alt") || value.empty()) {
			continue;
		}

		QString displayText = QString::fromStdString(value[1]);
		displayText.replace('&', "");

		QCheckBox* flag = new QCheckBox(displayText);
		flag->setProperty("flagValue", std::stoi(value[0]));
		flow->addWidget(flag, row++, 0);
	}

	QDialogButtonBox* buttonBox = new QDialogButtonBox;
	buttonBox->setStandardButtons(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
	connect(buttonBox, &QDialogButtonBox::accepted, dialog, &QDialog::accept);
	connect(buttonBox, &QDialogButtonBox::rejected, dialog, &QDialog::reject);

	layout->addLayout(flow);
	layout->addWidget(buttonBox);

	connect(dialog, &QDialog::accepted, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->commitData(editor);
		emit delegate->closeEditor(editor);
	});

	connect(dialog, &QDialog::rejected, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->closeEditor(editor);
	});

	dialog->show();

	return editor;
}

QWidget* TableDelegate::create_upgrade_list_editor(QWidget* parent) const {
	auto editor = new QWidget(parent);

	QDialog* dialog = new QDialog(editor, Qt::WindowTitleHint | Qt::WindowMaximizeButtonHint | Qt::WindowCloseButtonHint);
	dialog->setAttribute(Qt::WA_DeleteOnClose);
	dialog->resize(256, 360);
	dialog->setWindowModality(Qt::WindowModality::WindowModal);

	QVBoxLayout* layout = new QVBoxLayout(dialog);

	QListWidget* list = new QListWidget;
	list->setObjectName("upgradeList");
	list->setIconSize(QSize(32, 32));
	list->setDragDropMode(QAbstractItemView::DragDropMode::InternalMove);
	layout->addWidget(list);

	QHBoxLayout* hbox = new QHBoxLayout;

	QPushButton* add = new QPushButton("Add");
	QPushButton* remove = new QPushButton("Remove");
	remove->setDisabled(true);
	hbox->addWidget(add);
	hbox->addWidget(remove);
	layout->addLayout(hbox);
	connect(add, &QPushButton::clicked, [=]() {
		QDialog* selectdialog = new QDialog(dialog, Qt::WindowTitleHint | Qt::WindowMaximizeButtonHint | Qt::WindowCloseButtonHint);
		selectdialog->resize(300, 560);
		selectdialog->setWindowModality(Qt::WindowModality::WindowModal);

		QVBoxLayout* selectlayout = new QVBoxLayout(selectdialog);

		UpgradeTreeModel* uupgradeTreeModel = new UpgradeTreeModel(dialog);
		uupgradeTreeModel->setSourceModel(upgrade_table);
		QSortFilterProxyModel* filter = new QSortFilterProxyModel;
		filter->setSourceModel(uupgradeTreeModel);
		filter->setRecursiveFilteringEnabled(true);
		filter->setFilterCaseSensitivity(Qt::CaseSensitivity::CaseInsensitive);

		QLineEdit* search = new QLineEdit;
		search->setPlaceholderText("Search Upgrades");
		QTreeView* view = new QTreeView;
		view->setModel(filter);
		view->header()->hide();
		view->setSelectionBehavior(QAbstractItemView::SelectRows);
		view->setSelectionMode(QAbstractItemView::ExtendedSelection);
		view->expandAll();

		connect(search, &QLineEdit::textChanged, filter, QOverload<const QString&>::of(&QSortFilterProxyModel::setFilterFixedString));

		selectlayout->addWidget(search);
		selectlayout->addWidget(view);

		QDialogButtonBox* buttonBox2 = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
		connect(buttonBox2, &QDialogButtonBox::accepted, selectdialog, &QDialog::accept);
		connect(buttonBox2, &QDialogButtonBox::rejected, selectdialog, &QDialog::reject);
		selectlayout->addWidget(buttonBox2);

		auto add = [filter, list, selectdialog](const QModelIndex& index) {
			QModelIndex sourceIndex = filter->mapToSource(index);
			BaseTreeItem* treeItem = static_cast<BaseTreeItem*>(sourceIndex.internalPointer());
			if (treeItem->baseCategory || treeItem->subCategory) {
				return;
			}

			QListWidgetItem* item = new QListWidgetItem;
			item->setData(Qt::StatusTipRole, QString::fromStdString(treeItem->id));
			item->setText(upgrade_table->data(treeItem->id, "name1").toString());
			item->setIcon(upgrade_table->data(treeItem->id, "art1", Qt::DecorationRole).value<QIcon>());
			list->addItem(item);
		};

		connect(view, &QTreeView::activated, [=](const QModelIndex& index) {
			add(index);
			selectdialog->close();
		});

		connect(selectdialog, &QDialog::accepted, [=]() {
			for (const auto& i : view->selectionModel()->selectedIndexes()) {
				add(i);
			}
			selectdialog->close();
		});

		selectdialog->show();
		selectdialog->move(dialog->geometry().topRight() + QPoint(10, dialog->geometry().height() - selectdialog->geometry().height()));
	});

	connect(remove, &QPushButton::clicked, [=]() {
		for (auto i : list->selectedItems()) {
			delete i;
		}
	});
	connect(list, &QListWidget::itemSelectionChanged, [=]() {
		remove->setEnabled(list->selectedItems().size() > 0);
	});

	QDialogButtonBox* buttonBox = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
	connect(buttonBox, &QDialogButtonBox::accepted, dialog, &QDialog::accept);
	connect(buttonBox, &QDialogButtonBox::rejected, dialog, &QDialog::reject);
	layout->addWidget(buttonBox);

	connect(dialog, &QDialog::accepted, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->commitData(editor);
		emit delegate->closeEditor(editor);
	});

	connect(dialog, &QDialog::rejected, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->closeEditor(editor);
	});

	dialog->show();

	return editor;
}

QWidget* TableDelegate::create_unit_list_editor(QWidget* parent) const {
	auto editor = new QWidget(parent);

	QDialog* dialog = new QDialog(editor, Qt::WindowTitleHint | Qt::WindowMaximizeButtonHint | Qt::WindowCloseButtonHint);
	dialog->setAttribute(Qt::WA_DeleteOnClose);
	dialog->resize(256, 360);
	dialog->setWindowModality(Qt::WindowModality::WindowModal);

	QVBoxLayout* layout = new QVBoxLayout(dialog);

	QListWidget* list = new QListWidget;
	list->setObjectName("unitList");
	list->setIconSize(QSize(32, 32));
	list->setDragDropMode(QAbstractItemView::DragDropMode::InternalMove);
	layout->addWidget(list);

	QHBoxLayout* hbox = new QHBoxLayout;

	QPushButton* add = new QPushButton("Add");
	QPushButton* remove = new QPushButton("Remove");
	remove->setDisabled(true);
	hbox->addWidget(add);
	hbox->addWidget(remove);
	layout->addLayout(hbox);
	connect(add, &QPushButton::clicked, [=]() {
		QDialog* selectdialog = new QDialog(dialog, Qt::WindowTitleHint | Qt::WindowMaximizeButtonHint | Qt::WindowCloseButtonHint);
		selectdialog->resize(300, 560);
		selectdialog->setWindowModality(Qt::WindowModality::WindowModal);

		QVBoxLayout* selectlayout = new QVBoxLayout(selectdialog);

		UnitTreeModel* unitTreeModel = new UnitTreeModel(dialog);
		unitTreeModel->setSourceModel(units_table);
		QSortFilterProxyModel* filter = new QSortFilterProxyModel;
		filter->setSourceModel(unitTreeModel);
		filter->setRecursiveFilteringEnabled(true);
		filter->setFilterCaseSensitivity(Qt::CaseSensitivity::CaseInsensitive);

		QLineEdit* search = new QLineEdit;
		search->setPlaceholderText("Search Units");
		QTreeView* view = new QTreeView;
		view->setModel(filter);
		view->header()->hide();
		view->setSelectionBehavior(QAbstractItemView::SelectRows);
		view->setSelectionMode(QAbstractItemView::ExtendedSelection);
		view->expandAll();

		connect(search, &QLineEdit::textChanged, filter, QOverload<const QString&>::of(&QSortFilterProxyModel::setFilterFixedString));

		selectlayout->addWidget(search);
		selectlayout->addWidget(view);

		QDialogButtonBox* buttonBox2 = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
		connect(buttonBox2, &QDialogButtonBox::accepted, selectdialog, &QDialog::accept);
		connect(buttonBox2, &QDialogButtonBox::rejected, selectdialog, &QDialog::reject);
		selectlayout->addWidget(buttonBox2);

		auto add = [filter, list, selectdialog](const QModelIndex& index) {
			QModelIndex sourceIndex = filter->mapToSource(index);
			BaseTreeItem* treeItem = static_cast<BaseTreeItem*>(sourceIndex.internalPointer());
			if (treeItem->baseCategory || treeItem->subCategory) {
				return;
			}

			QListWidgetItem* item = new QListWidgetItem;
			item->setData(Qt::StatusTipRole, QString::fromStdString(treeItem->id));
			item->setText(units_table->data(treeItem->id, "name").toString());
			item->setIcon(units_table->data(treeItem->id, "art", Qt::DecorationRole).value<QIcon>());
			list->addItem(item);
		};

		connect(view, &QTreeView::activated, [=](const QModelIndex& index) {
			add(index);
			selectdialog->close();
		});

		connect(selectdialog, &QDialog::accepted, [=]() {
			for (const auto& i : view->selectionModel()->selectedIndexes()) {
				add(i);
			}
			selectdialog->close();
		});

		selectdialog->show();
		selectdialog->move(dialog->geometry().topRight() + QPoint(10, dialog->geometry().height() - selectdialog->geometry().height()));
	});

	connect(remove, &QPushButton::clicked, [=]() {
		for (auto i : list->selectedItems()) {
			delete i;
		}
	});
	connect(list, &QListWidget::itemSelectionChanged, [=]() {
		remove->setEnabled(list->selectedItems().size() > 0);
	});

	QDialogButtonBox* buttonBox = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
	connect(buttonBox, &QDialogButtonBox::accepted, dialog, &QDialog::accept);
	connect(buttonBox, &QDialogButtonBox::rejected, dialog, &QDialog::reject);
	layout->addWidget(buttonBox);

	connect(dialog, &QDialog::accepted, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->commitData(editor);
		emit delegate->closeEditor(editor);
	});

	connect(dialog, &QDialog::rejected, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->closeEditor(editor);
	});

	dialog->show();

	return editor;
}

QWidget* TableDelegate::create_ability_list_editor(QWidget* parent) const {
	auto editor = new QWidget(parent);

	QDialog* dialog = new QDialog(editor, Qt::Window | Qt::WindowTitleHint | Qt::WindowMaximizeButtonHint | Qt::WindowCloseButtonHint);
	dialog->setAttribute(Qt::WA_DeleteOnClose);
	dialog->resize(256, 360);
	dialog->setWindowModality(Qt::WindowModality::WindowModal);

	QVBoxLayout* layout = new QVBoxLayout(dialog);

	QListWidget* list = new QListWidget;
	list->setObjectName("abilityList");
	list->setIconSize(QSize(32, 32));
	list->setDragDropMode(QAbstractItemView::DragDropMode::InternalMove);
	list->setSelectionBehavior(QAbstractItemView::SelectRows);
	list->setSelectionMode(QAbstractItemView::ExtendedSelection);

	layout->addWidget(list);

	QHBoxLayout* hbox = new QHBoxLayout;

	QPushButton* add = new QPushButton("Add");
	QPushButton* remove = new QPushButton("Remove");
	remove->setDisabled(true);
	hbox->addWidget(add);
	hbox->addWidget(remove);
	layout->addLayout(hbox);
	connect(add, &QPushButton::clicked, [=]() {
		QDialog* selectdialog = new QDialog(dialog, Qt::WindowTitleHint | Qt::WindowMaximizeButtonHint | Qt::WindowCloseButtonHint);
		selectdialog->resize(300, 560);
		selectdialog->setWindowModality(Qt::WindowModality::WindowModal);

		QVBoxLayout* selectlayout = new QVBoxLayout(selectdialog);

		AbilityTreeModel* abilityTreeModel = new AbilityTreeModel(dialog);
		abilityTreeModel->setSourceModel(abilities_table);
		QSortFilterProxyModel* filter = new QSortFilterProxyModel;
		filter->setSourceModel(abilityTreeModel);
		filter->setRecursiveFilteringEnabled(true);
		filter->setFilterCaseSensitivity(Qt::CaseSensitivity::CaseInsensitive);

		QLineEdit* search = new QLineEdit;
		search->setPlaceholderText("Search Abilities");
		QTreeView* view = new QTreeView;
		view->setModel(filter);
		view->header()->hide();
		view->setSelectionBehavior(QAbstractItemView::SelectRows);
		view->setSelectionMode(QAbstractItemView::ExtendedSelection);
		view->expandAll();

		connect(search, &QLineEdit::textChanged, filter, QOverload<const QString&>::of(&QSortFilterProxyModel::setFilterFixedString));

		selectlayout->addWidget(search);
		selectlayout->addWidget(view);

		QDialogButtonBox* buttonBox2 = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
		connect(buttonBox2, &QDialogButtonBox::accepted, selectdialog, &QDialog::accept);
		connect(buttonBox2, &QDialogButtonBox::rejected, selectdialog, &QDialog::reject);
		selectlayout->addWidget(buttonBox2);

		auto add = [filter, list, selectdialog](const QModelIndex& index) {
			QModelIndex sourceIndex = filter->mapToSource(index);
			BaseTreeItem* treeItem = static_cast<BaseTreeItem*>(sourceIndex.internalPointer());
			if (treeItem->baseCategory || treeItem->subCategory) {
				return;
			}

			QListWidgetItem* item = new QListWidgetItem;
			item->setData(Qt::StatusTipRole, QString::fromStdString(treeItem->id));
			item->setText(abilities_table->data(treeItem->id, "name").toString());
			item->setIcon(abilities_table->data(treeItem->id, "art", Qt::DecorationRole).value<QIcon>());

			list->addItem(item);
		};

		connect(view, &QTreeView::activated, [=](const QModelIndex& index) {
			add(index);
			selectdialog->close();
		});

		connect(selectdialog, &QDialog::accepted, [=]() {
			for (const auto& i : view->selectionModel()->selectedIndexes()) {
				add(i);
			}
			selectdialog->close();
		});

		selectdialog->show();
		selectdialog->move(dialog->geometry().topRight() + QPoint(10, dialog->geometry().height() - selectdialog->geometry().height()));
	});
	connect(remove, &QPushButton::clicked, [=]() {
		for (auto i : list->selectedItems()) {
			delete i;
		}
	});
	connect(list, &QListWidget::itemSelectionChanged, [=]() {
		remove->setEnabled(list->selectedItems().size() > 0);
	});

	QDialogButtonBox* buttonBox = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
	connect(buttonBox, &QDialogButtonBox::accepted, dialog, &QDialog::accept);
	connect(buttonBox, &QDialogButtonBox::rejected, dialog, &QDialog::reject);
	layout->addWidget(buttonBox);

	connect(dialog, &QDialog::accepted, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->commitData(editor);
		emit delegate->closeEditor(editor);
	});

	connect(dialog, &QDialog::rejected, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->closeEditor(editor);
	});

	dialog->show();

	return editor;
}

QWidget* TableDelegate::create_buff_list_editor(QWidget* parent) const {
	auto editor = new QWidget(parent);

	QDialog* dialog = new QDialog(editor, Qt::Window | Qt::WindowTitleHint | Qt::WindowMaximizeButtonHint | Qt::WindowCloseButtonHint);
	dialog->setAttribute(Qt::WA_DeleteOnClose);
	dialog->resize(256, 360);
	dialog->setWindowModality(Qt::WindowModality::WindowModal);

	QVBoxLayout* layout = new QVBoxLayout(dialog);

	QListWidget* list = new QListWidget;
	list->setObjectName("buffList");
	list->setIconSize(QSize(32, 32));
	list->setDragDropMode(QAbstractItemView::DragDropMode::InternalMove);
	list->setSelectionBehavior(QAbstractItemView::SelectRows);
	list->setSelectionMode(QAbstractItemView::ExtendedSelection);
	layout->addWidget(list);

	QHBoxLayout* hbox = new QHBoxLayout;

	QPushButton* add = new QPushButton("Add");
	QPushButton* remove = new QPushButton("Remove");
	remove->setDisabled(true);
	hbox->addWidget(add);
	hbox->addWidget(remove);
	layout->addLayout(hbox);

	connect(add, &QPushButton::clicked, [=]() {
		QDialog* selectdialog = new QDialog(dialog, Qt::WindowTitleHint | Qt::WindowMaximizeButtonHint | Qt::WindowCloseButtonHint);
		selectdialog->resize(300, 560);
		selectdialog->setWindowModality(Qt::WindowModality::WindowModal);

		QVBoxLayout* selectlayout = new QVBoxLayout(selectdialog);

		BuffTreeModel* buffTreeModel = new BuffTreeModel(dialog);
		buffTreeModel->setSourceModel(buff_table);
		QSortFilterProxyModel* filter = new QSortFilterProxyModel;
		filter->setSourceModel(buffTreeModel);
		filter->setRecursiveFilteringEnabled(true);
		filter->setFilterCaseSensitivity(Qt::CaseSensitivity::CaseInsensitive);

		QLineEdit* search = new QLineEdit;
		search->setPlaceholderText("Search Buffs");
		QTreeView* view = new QTreeView;
		view->setModel(filter);
		view->header()->hide();
		view->setSelectionBehavior(QAbstractItemView::SelectRows);
		view->setSelectionMode(QAbstractItemView::ExtendedSelection);
		view->expandAll();

		connect(search, &QLineEdit::textChanged, filter, QOverload<const QString&>::of(&QSortFilterProxyModel::setFilterFixedString));

		selectlayout->addWidget(search);
		selectlayout->addWidget(view);

		QDialogButtonBox* buttonBox2 = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
		connect(buttonBox2, &QDialogButtonBox::accepted, selectdialog, &QDialog::accept);
		connect(buttonBox2, &QDialogButtonBox::rejected, selectdialog, &QDialog::reject);
		selectlayout->addWidget(buttonBox2);

		auto add = [filter, list, selectdialog](const QModelIndex& index) {
			QModelIndex sourceIndex = filter->mapToSource(index);
			BaseTreeItem* treeItem = static_cast<BaseTreeItem*>(sourceIndex.internalPointer());
			if (treeItem->baseCategory || treeItem->subCategory) {
				return;
			}

			QString name = buff_table->data(treeItem->id, "editorname").toString();
			if (name.isEmpty()) {
				name = buff_table->data(treeItem->id, "bufftip").toString();
			}

			QListWidgetItem* item = new QListWidgetItem;
			item->setData(Qt::StatusTipRole, QString::fromStdString(treeItem->id));
			item->setText(name);
			item->setIcon(buff_table->data(treeItem->id, "buffart", Qt::DecorationRole).value<QIcon>());
			list->addItem(item);
		};

		connect(view, &QTreeView::activated, [=](const QModelIndex& index) {
			add(index);
			selectdialog->close();
		});

		connect(selectdialog, &QDialog::accepted, [=]() {
			for (const auto& i : view->selectionModel()->selectedIndexes()) {
				add(i);
			}
			selectdialog->close();
		});

		selectdialog->show();
		selectdialog->move(dialog->geometry().topRight() + QPoint(10, dialog->geometry().height() - selectdialog->geometry().height()));
	});

	connect(remove, &QPushButton::clicked, [=]() {
		for (auto i : list->selectedItems()) {
			delete i;
		}
	});
	connect(list, &QListWidget::itemSelectionChanged, [=]() {
		remove->setEnabled(list->selectedItems().size() > 0);
	});

	QDialogButtonBox* buttonBox = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
	connect(buttonBox, &QDialogButtonBox::accepted, dialog, &QDialog::accept);
	connect(buttonBox, &QDialogButtonBox::rejected, dialog, &QDialog::reject);
	layout->addWidget(buttonBox);

	connect(dialog, &QDialog::accepted, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->commitData(editor);
		emit delegate->closeEditor(editor);
	});

	connect(dialog, &QDialog::rejected, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->closeEditor(editor);
	});

	dialog->show();

	return editor;
}

QWidget* TableDelegate::create_icon_editor(QWidget* parent) const {
	auto editor = new QWidget(parent);

	QDialog* dialog = new QDialog(editor, Qt::WindowTitleHint | Qt::WindowMaximizeButtonHint | Qt::WindowCloseButtonHint);
	dialog->setAttribute(Qt::WA_DeleteOnClose);
	dialog->resize(530, 512);
	dialog->setWindowModality(Qt::WindowModality::WindowModal);

	IconView* view = new IconView;
	view->setObjectName("iconView");

	QDialogButtonBox* buttonBox = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel);
	connect(buttonBox, &QDialogButtonBox::accepted, dialog, &QDialog::accept);
	connect(buttonBox, &QDialogButtonBox::rejected, dialog, &QDialog::reject);

	connect(dialog, &QDialog::accepted, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->commitData(editor);
		emit delegate->closeEditor(editor);
	});

	connect(dialog, &QDialog::rejected, [=]() {
		const auto delegate = const_cast<TableDelegate*>(this);
		emit delegate->closeEditor(editor);
	});

	QVBoxLayout* layout = new QVBoxLayout(dialog);
	layout->addWidget(view);
	layout->addWidget(buttonBox);

	dialog->show();

	return editor;
}
