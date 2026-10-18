---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/asset_manager/asset_manager.cpp"
ref: HEAD
---

#include "asset_manager.h"

#include <QApplication>
#include <QSizePolicy>
#include <QFileIconProvider>
#include <QHBoxLayout>
#include <QMenu>
#include <QMessageBox>
#include <QPushButton>
#include <QStyle>
#include <QVBoxLayout>
#include <QComboBox>
#include <QFile>
#include <QStandardItem>

import std;
import SLK;
import Map;
import MapGlobal;
import Globals;
import TableModel;
import ResourceManager;
import QIconResource;
import WindowHandler;
import "object_editor/object_editor.h";

namespace fs = std::filesystem;

using namespace asset;

QIcon get_file_icon(const std::string& path) {
	static const QIcon model_icon = QApplication::style()->standardIcon(QStyle::SP_FileDialogDetailedView);
	static const QIcon image_icon = QApplication::style()->standardIcon(QStyle::SP_DesktopIcon);
	static const QIcon sound_icon = QApplication::style()->standardIcon(QStyle::SP_MediaVolume);
	static const QIcon file_icon = QFileIconProvider().icon(QFileIconProvider::File);

	std::string ext = fs::path(path).extension().string();
	for (auto& c : ext) {
		c = static_cast<char>(std::tolower(static_cast<unsigned char>(c)));
	}

	if (ext == ".mdx" || ext == ".mdl") {
		return model_icon;
	}
	if (ext == ".blp" || ext == ".tga" || ext == ".dds" || ext == ".png" || ext == ".jpg" || ext == ".jpeg") {
		return image_icon;
	}
	if (ext == ".wav" || ext == ".mp3" || ext == ".flac" || ext == ".ogg") {
		return sound_icon;
	}
	return file_icon;
}

QString file_type_label(const std::string& path) {
	std::string ext = fs::path(path).extension().string();
	for (auto& c : ext) {
		c = static_cast<char>(std::tolower(static_cast<unsigned char>(c)));
	}
	if (ext == ".mdx" || ext == ".mdl") {
		return "Model";
	}
	if (ext == ".blp" || ext == ".tga" || ext == ".dds" || ext == ".png" || ext == ".jpg" || ext == ".jpeg") {
		return "Texture";
	}
	if (ext == ".wav" || ext == ".mp3" || ext == ".flac" || ext == ".ogg") {
		return "Sound";
	}
	if (ext.size() > 1) {
		return QString::fromStdString(ext.substr(1)).toUpper();
	}
	return "File";
}

QString format_size(qulonglong bytes) {
	constexpr double KB = 1024.0;
	const double b = static_cast<double>(bytes);
	if (b >= KB * KB * KB) {
		return QString::number(b / (KB * KB * KB), 'f', 1) + " GB";
	}
	if (b >= KB * KB) {
		return QString::number(b / (KB * KB), 'f', 1) + " MB";
	}
	if (b >= KB) {
		return QString::number(b / KB, 'f', 1) + " KB";
	}
	return QString::number(bytes) + " B";
}

QIcon get_object_icon(const TableModel* table, const std::string_view id, const std::string_view art_field) {
	const QVariant v = table->data(id, art_field, Qt::DecorationRole);
	if (v.isValid() && !v.isNull()) {
		return v.value<QIcon>();
	}
	return {};
}

QString get_object_name(const TableModel* table, const std::string_view id, const std::string_view name_field) {
	const QVariant v = table->data(id, name_field, Qt::DisplayRole);
	if (v.isValid() && !v.isNull()) {
		return v.toString();
	}
	return QString::fromStdString(std::string(id));
}

struct ObjectInfo {
	QString display_name;
	QIcon icon;
	int category = -1; // matches ObjectEditor::Category, -1 = not a named object
};

ObjectInfo resolve_used_by_id(const std::string& id) {
	if (id == "loadingscreen") {
		return {"Loading Screen", QApplication::style()->standardIcon(QStyle::SP_DesktopIcon), -1};
	}
	if (id == "map script") {
		return {"Map Script", QApplication::style()->standardIcon(QStyle::SP_FileDialogDetailedView), -1};
	}
	// MDX transitive reference (path contains a slash)
	if (id.contains('/')) {
		return {QString::fromStdString(id), QFileIconProvider().icon(QFileIconProvider::File), -1};
	}
	const auto display = [&](const QString& name) {
		return name + " (" + QString::fromStdString(id) + ")";
	};
	// Load the category icon used by DoodadTreeModel / DestructibleTreeModel
	const auto category_icon = [](const std::string& section, char cat) -> QIcon {
		for (const auto& [key, value] : world_edit_data.section(section)) {
			if (!key.empty() && key.front() == cat) {
				return resource_manager.load<QIconResource>(value[1]).value()->icon;
			}
		}
		return {};
	};
	if (units_slk.row_headers.contains(id)) {
		return {
			display(get_object_name(units_table, id, "name")),
			get_object_icon(units_table, id, "art"),
			static_cast<int>(ObjectEditor::Category::unit)
		};
	}
	if (items_slk.row_headers.contains(id)) {
		return {
			display(get_object_name(items_table, id, "name")),
			get_object_icon(items_table, id, "art"),
			static_cast<int>(ObjectEditor::Category::item)
		};
	}
	if (abilities_slk.row_headers.contains(id)) {
		return {
			display(get_object_name(abilities_table, id, "name")),
			get_object_icon(abilities_table, id, "art"),
			static_cast<int>(ObjectEditor::Category::ability)
		};
	}
	if (destructibles_slk.row_headers.contains(id)) {
		const std::string_view cat = destructibles_slk.data<std::string_view>("category", id);
		return {
			display(get_object_name(destructibles_table, id, "name")),
			cat.empty() ? QIcon {} : category_icon("DestructibleCategories", cat.front()),
			static_cast<int>(ObjectEditor::Category::destructible)
		};
	}
	if (doodads_slk.row_headers.contains(id)) {
		const std::string_view cat = doodads_slk.data<std::string_view>("category", id);
		return {
			display(get_object_name(doodads_table, id, "name")),
			cat.empty() ? QIcon {} : category_icon("DoodadCategories", cat.front()),
			static_cast<int>(ObjectEditor::Category::doodad)
		};
	}
	if (buff_slk.row_headers.contains(id)) {
		QString name = get_object_name(buff_table, id, "editorname");
		if (name.isEmpty() || name == QString::fromStdString(id)) {
			name = get_object_name(buff_table, id, "bufftip");
		}
		return {display(name), get_object_icon(buff_table, id, "buffart"), static_cast<int>(ObjectEditor::Category::buff)};
	}
	if (upgrade_slk.row_headers.contains(id)) {
		return {
			display(get_object_name(upgrade_table, id, "name1")),
			get_object_icon(upgrade_table, id, "art1"),
			static_cast<int>(ObjectEditor::Category::upgrade)
		};
	}
	// Fallback: likely a sound name
	return {QString::fromStdString(id), QApplication::style()->standardIcon(QStyle::SP_MediaVolume), -1};
}

void AssetFilterModel::set_only_unused(bool value) {
	if (only_unused == value) {
		return;
	}
	beginFilterChange();
	only_unused = value;
	endFilterChange(Direction::Rows);
}

bool AssetFilterModel::lessThan(const QModelIndex& left, const QModelIndex& right) const {
	// Sort the Usages column by an explicit rank (unused first, then by usage count, overrides last),
	// breaking ties alphabetically by file path.
	if (left.column() == UsageColumn) {
		const int lk = left.data(SortKeyRole).toInt();
		const int rk = right.data(SortKeyRole).toInt();
		if (lk != rk) {
			return lk < rk;
		}
		const QString lp = left.siblingAtColumn(FileColumn).data(Qt::DisplayRole).toString();
		const QString rp = right.siblingAtColumn(FileColumn).data(Qt::DisplayRole).toString();
		return lp.localeAwareCompare(rp) < 0;
	}
	return QSortFilterProxyModel::lessThan(left, right);
}

bool AssetFilterModel::filterAcceptsRow(int source_row, const QModelIndex& source_parent) const {
	// Child (reference) rows follow their parent's visibility.
	if (source_parent.isValid()) {
		return true;
	}

	const QModelIndex index = sourceModel()->index(source_row, FileColumn, source_parent);

	if (only_unused && !index.data(IsUnusedRole).toBool()) {
		return false;
	}

	const QRegularExpression re = filterRegularExpression();
	if (re.pattern().isEmpty()) {
		return true;
	}

	if (index.data(Qt::DisplayRole).toString().contains(re)) {
		return true;
	}

	// Also match if any referenced object/file under this entry matches.
	const int children = sourceModel()->rowCount(index);
	for (int i = 0; i < children; i++) {
		if (sourceModel()->index(i, FileColumn, index).data(Qt::DisplayRole).toString().contains(re)) {
			return true;
		}
	}
	return false;
}

AssetManager::AssetManager(QWidget* parent) : QDialog(parent) {
	setAttribute(Qt::WA_DeleteOnClose);
	setWindowTitle("Asset Manager");
	resize(640, 820);

	auto* layout = new QVBoxLayout(this);

	// Header: stats on the left, rescan on the right
	auto* header_row = new QHBoxLayout;
	stats_label = new QLabel(this);
	QFont stats_font = stats_label->font();
	stats_font.setBold(true);
	stats_label->setFont(stats_font);
	header_row->addWidget(stats_label);
	header_row->addStretch();

	auto* refresh_button = new QPushButton(this);
	refresh_button->setIcon(QIcon("data/icons/asset_manager/refresh.png"));
	refresh_button->setIconSize(QSize(16, 16));
	refresh_button->setText("Rescan");
	header_row->addWidget(refresh_button);
	layout->addLayout(header_row);

	// Search + info
	auto* search_row = new QHBoxLayout;
	search_edit = new QLineEdit(this);
	search_edit->setPlaceholderText("Search files...");
	search_edit->setClearButtonEnabled(true);
	search_row->addWidget(search_edit);

	filter_combo = new QComboBox(this);
	filter_combo->addItem("All files");
	filter_combo->addItem("Unused only");
	search_row->addWidget(filter_combo);

	auto* info_button = new QLabel(this);
	info_button->setPixmap(QApplication::style()->standardIcon(QStyle::SP_MessageBoxInformation).pixmap(QSize(16, 16)));
	info_button->setToolTip(
		"Usage detection is not perfect and can show files as unused even if they're actually used.\n"
		"It has difficulty detecting references made from the map script if they don't use forward slashes.\n"
		"Deleting moves files to the recycle bin, but double-check before deleting in bulk!"
	);
	search_row->addWidget(info_button);
	layout->addLayout(search_row);

	model = new QStandardItemModel(this);

	filter_model = new AssetFilterModel(this);
	filter_model->setSourceModel(model);
	filter_model->setFilterCaseSensitivity(Qt::CaseInsensitive);
	filter_model->setFilterKeyColumn(FileColumn);

	tree_view = new QTreeView(this);
	tree_view->setModel(filter_model);
	tree_view->setUniformRowHeights(true);
	tree_view->setContextMenuPolicy(Qt::CustomContextMenu);
	tree_view->setSortingEnabled(true);
	tree_view->setSelectionBehavior(QAbstractItemView::SelectRows);
	tree_view->setSelectionMode(QAbstractItemView::ExtendedSelection);
	tree_view->header()->setStretchLastSection(false);
	layout->addWidget(tree_view);

	// Bottom action bar
	auto* action_row = new QHBoxLayout;
	selection_label = new QLabel(this);
	action_row->addWidget(selection_label);
	action_row->addStretch();

	select_unused_button = new QPushButton("Select all unused", this);
	action_row->addWidget(select_unused_button);

	delete_button = new QPushButton(this);
	delete_button->setIcon(QApplication::style()->standardIcon(QStyle::SP_TrashIcon));
	delete_button->setText("Delete");
	delete_button->setStyleSheet("QPushButton:enabled { color: #d9534f; font-weight: bold; }");
	action_row->addWidget(delete_button);
	layout->addLayout(action_row);

	connect(search_edit, &QLineEdit::textChanged, filter_model, &QSortFilterProxyModel::setFilterFixedString);
	connect(filter_combo, QOverload<int>::of(&QComboBox::currentIndexChanged), [this](int index) {
		filter_model->set_only_unused(index == 1);
	});
	connect(refresh_button, &QPushButton::clicked, this, &AssetManager::refresh);
	connect(select_unused_button, &QPushButton::clicked, this, &AssetManager::select_all_unused);
	connect(delete_button, &QPushButton::clicked, this, &AssetManager::delete_selected);
	connect(model, &QStandardItemModel::itemChanged, this, [this](QStandardItem*) {
		update_selection();
	});

	// When objects are deleted in the Object Editor, remove their references from the tree.
	// We use rowsAboutToBeRemoved so the SLK index_to_row mapping is still intact.
	const auto make_removal_handler = [this](const slk::SLK& slk) {
		return [this, &slk](const QModelIndex&, const int first, const int last) {
			for (int i = first; i <= last; i++) {
				remove_object_references(slk.index_to_row.at(i));
			}
		};
	};
	connect(units_table, &QAbstractItemModel::rowsAboutToBeRemoved, this, make_removal_handler(units_slk));
	connect(items_table, &QAbstractItemModel::rowsAboutToBeRemoved, this, make_removal_handler(items_slk));
	connect(abilities_table, &QAbstractItemModel::rowsAboutToBeRemoved, this, make_removal_handler(abilities_slk));
	connect(doodads_table, &QAbstractItemModel::rowsAboutToBeRemoved, this, make_removal_handler(doodads_slk));
	connect(destructibles_table, &QAbstractItemModel::rowsAboutToBeRemoved, this, make_removal_handler(destructibles_slk));
	connect(buff_table, &QAbstractItemModel::rowsAboutToBeRemoved, this, make_removal_handler(buff_slk));
	connect(tree_view, &QTreeView::customContextMenuRequested, this, &AssetManager::show_context_menu);
	connect(tree_view, &QTreeView::doubleClicked, this, &AssetManager::open_in_editor);

	refresh();
	show();
}

void AssetManager::refresh() const {
	model->clear();
	model->setHorizontalHeaderLabels({"File", "Type", "Usages"});
	tree_view->header()->setSectionResizeMode(FileColumn, QHeaderView::Stretch);
	tree_view->header()->setSectionResizeMode(TypeColumn, QHeaderView::ResizeToContents);
	tree_view->header()->setSectionResizeMode(UsageColumn, QHeaderView::ResizeToContents);

	auto results = map->get_file_usage();

	constexpr QColor orange(200, 120, 0);
	constexpr QColor gray(128, 128, 128);

	for (const auto& [path, used_by, is_override, size] : results) {
		const bool is_unused = used_by.empty() && !is_override;

		auto* file_item = new QStandardItem(get_file_icon(path), QString::fromStdString(path));
		file_item->setEditable(false);
		file_item->setCheckable(true);
		file_item->setCheckState(Qt::Unchecked);
		file_item->setData(is_unused, IsUnusedRole);
		file_item->setData(is_override, IsOverrideRole);
		file_item->setData(static_cast<qulonglong>(size), SizeRole);

		auto* type_item = new QStandardItem(is_override ? "Override" : file_type_label(path));
		type_item->setEditable(false);

		auto* count_item = new QStandardItem(is_override ? "—" : QString::number(used_by.size()));
		count_item->setEditable(false);
		count_item->setTextAlignment(Qt::AlignRight | Qt::AlignVCenter);
		// Sort rank: unused first, then by usage count, overrides last.
		const int sort_key = is_unused ? -1 : (is_override ? std::numeric_limits<int>::max() : static_cast<int>(used_by.size()));
		count_item->setData(sort_key, SortKeyRole);

		if (is_unused) {
			file_item->setForeground(orange);
			type_item->setForeground(orange);
			count_item->setForeground(orange);
		} else if (is_override) {
			file_item->setForeground(gray);
			type_item->setForeground(gray);
			count_item->setForeground(gray);
		}

		for (const auto& id : used_by) {
			const auto& info = resolve_used_by_id(id);

			auto* child_item = new QStandardItem(info.display_name);
			child_item->setEditable(false);
			if (!info.icon.isNull()) {
				child_item->setIcon(info.icon);
			}
			child_item->setData(QString::fromStdString(id), ObjectIdRole);
			child_item->setData(info.category, CategoryRole);

			file_item->appendRow(child_item);
		}

		model->appendRow({file_item, type_item, count_item});
	}

	tree_view->sortByColumn(UsageColumn, Qt::AscendingOrder);

	update_status();
	update_selection();
}

void AssetManager::update_status() const {
	const int total = model->rowCount();
	int unused = 0;
	qulonglong reclaimable = 0;
	for (int i = 0; i < total; i++) {
		const QStandardItem* item = model->item(i, FileColumn);
		if (item && item->data(IsUnusedRole).toBool()) {
			unused += 1;
			reclaimable += item->data(SizeRole).toULongLong();
		}
	}
	stats_label->setText(
		QString("%1 files · %2 unused · %3 reclaimable").arg(total).arg(unused).arg(format_size(reclaimable))
	);
}

void AssetManager::update_selection() const {
	int selected = 0;
	for (int i = 0; i < model->rowCount(); i++) {
		const QStandardItem* item = model->item(i, FileColumn);
		if (item && item->isCheckable() && item->checkState() == Qt::Checked) {
			selected += 1;
		}
	}
	selection_label->setText(selected == 0 ? QString("No files selected") : QString("%1 selected").arg(selected));
	delete_button->setEnabled(selected > 0);
}

void AssetManager::select_all_unused() {
	for (int i = 0; i < model->rowCount(); i++) {
		QStandardItem* item = model->item(i, FileColumn);
		if (item && item->data(IsUnusedRole).toBool()) {
			item->setCheckState(Qt::Checked);
		}
	}
}

QStringList AssetManager::trash_files(const std::vector<QStandardItem*>& file_items) {
	QStringList failures;
	std::vector<int> rows_to_remove;
	for (QStandardItem* item : file_items) {
		const QString rel = item->text();
		const fs::path full = map->filesystem_path / rel.toStdString();
		if (QFile::moveToTrash(QString::fromStdString(full.string()))) {
			rows_to_remove.push_back(item->row());
		} else {
			failures.push_back(rel);
		}
	}

	// Remove from the bottom up so row indices stay valid.
	std::ranges::sort(rows_to_remove, std::greater<>{});
	for (const int row : rows_to_remove) {
		model->removeRow(row);
	}
	return failures;
}

void AssetManager::delete_selected() {
	std::vector<QStandardItem*> checked;
	for (int i = 0; i < model->rowCount(); i++) {
		QStandardItem* item = model->item(i, FileColumn);
		if (item && item->checkState() == Qt::Checked) {
			checked.push_back(item);
		}
	}
	if (checked.empty()) {
		return;
	}

	const int answer = QMessageBox::question(
		this,
		"Delete files",
		QString("Move %1 file(s) to the recycle bin?").arg(checked.size()),
		QMessageBox::Yes | QMessageBox::No
	);
	if (answer != QMessageBox::Yes) {
		return;
	}

	const auto failures = trash_files(checked);
	if (!failures.empty()) {
		QMessageBox::warning(
			this,
			"Delete failed",
			QString("Could not delete %1 file(s):\n%2").arg(failures.size()).arg(failures.join("\n"))
		);
	}

	update_status();
	update_selection();
}

void AssetManager::remove_object_references(const std::string& id) {
	const QString qid = QString::fromStdString(id);
	constexpr QColor orange(200, 120, 0);

	for (int row = 0; row < model->rowCount(); row++) {
		QStandardItem* const file_item = model->item(row, FileColumn);
		if (!file_item) {
			continue;
		}

		bool changed = false;
		for (int child_row = file_item->rowCount() - 1; child_row >= 0; child_row--) {
			const QStandardItem* const child = file_item->child(child_row);
			if (child && child->data(ObjectIdRole).toString() == qid) {
				file_item->removeRow(child_row);
				changed = true;
			}
		}

		if (!changed) {
			continue;
		}

		const int new_count = file_item->rowCount();
		const bool is_override = file_item->data(IsOverrideRole).toBool();
		const bool is_now_unused = (new_count == 0) && !is_override;

		if (QStandardItem* count_item = model->item(row, UsageColumn)) {
			if (!is_override) {
				count_item->setText(QString::number(new_count));
				count_item->setData(is_now_unused ? -1 : new_count, SortKeyRole);
			}
			count_item->setData(is_now_unused ? QVariant(QBrush(orange)) : QVariant(), Qt::ForegroundRole);
		}
		if (QStandardItem* type_item = model->item(row, TypeColumn)) {
			type_item->setData(is_now_unused ? QVariant(QBrush(orange)) : QVariant(), Qt::ForegroundRole);
		}
		file_item->setData(is_now_unused, IsUnusedRole);
		file_item->setData(is_now_unused ? QVariant(QBrush(orange)) : QVariant(), Qt::ForegroundRole);
	}

	update_status();
}

void AssetManager::open_in_editor(const QModelIndex& proxy_index) const {
	if (!proxy_index.isValid()) {
		return;
	}
	const QModelIndex source_index = filter_model->mapToSource(proxy_index).siblingAtColumn(FileColumn);
	if (!source_index.parent().isValid()) {
		return; // root (file) item — nothing to open
	}
	QStandardItem* item = model->itemFromIndex(source_index);
	if (!item) {
		return;
	}
	const int category = item->data(CategoryRole).toInt();
	if (category < 0) {
		return;
	}
	const QString id = item->data(ObjectIdRole).toString();
	bool created = false;
	auto* editor = window_handler.create_or_raise<ObjectEditor>(nullptr, created);
	editor->select_id(static_cast<ObjectEditor::Category>(category), id.toStdString());
}

void AssetManager::show_context_menu(const QPoint& pos) {
	const QModelIndex proxy_index = tree_view->indexAt(pos);
	if (!proxy_index.isValid()) {
		return;
	}

	// Always work with column 0 so the file roles are accessible
	const QModelIndex source_index = filter_model->mapToSource(proxy_index).siblingAtColumn(FileColumn);
	QStandardItem* item = model->itemFromIndex(source_index);
	if (!item) {
		return;
	}

	QMenu menu;

	const bool is_root = !source_index.parent().isValid();
	if (is_root) {
		QAction* delete_action = menu.addAction(QApplication::style()->standardIcon(QStyle::SP_TrashIcon), "Delete file");
		connect(delete_action, &QAction::triggered, [this, item]() {
			const int answer = QMessageBox::question(
				this,
				"Delete file",
				QString("Move '%1' to the recycle bin?").arg(item->text()),
				QMessageBox::Yes | QMessageBox::No
			);
			if (answer != QMessageBox::Yes) {
				return;
			}
			const auto failures = trash_files({item});
			if (!failures.empty()) {
				QMessageBox::warning(
					this,
					"Delete failed",
					QString("Could not delete '%1'.").arg(failures.first())
				);
				return;
			}
			update_status();
			update_selection();
		});
	} else {
		const int category = item->data(CategoryRole).toInt();
		if (category >= 0) {
			QAction* open_action = menu.addAction("Open in Object Editor");
			connect(open_action, &QAction::triggered, [this, proxy_index]() {
				open_in_editor(proxy_index);
			});
		}
	}

	if (!menu.isEmpty()) {
		menu.exec(tree_view->viewport()->mapToGlobal(pos));
	}
}
