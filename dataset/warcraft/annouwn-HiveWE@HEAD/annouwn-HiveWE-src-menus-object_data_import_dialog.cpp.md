---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/object_data_import_dialog.cpp"
ref: HEAD
---

#include "object_data_import_dialog.h"

#include <QAbstractItemView>
#include <QCheckBox>
#include <QDialogButtonBox>
#include <QFontMetrics>
#include <QHeaderView>
#include <QLabel>
#include <QPushButton>
#include <QTableWidget>
#include <QTabWidget>
#include <QVBoxLayout>

import std;
import ObjectDataIo;

namespace {

QString severity_label(const ValidationSeverity severity) {
	switch (severity) {
		case ValidationSeverity::error:
			return "Error";
		case ValidationSeverity::warning:
			return "Warning";
		case ValidationSeverity::info:
			return "Info";
	}
	return "Info";
}

QString category_label(const ObjectDataCategory category) {
	if (const auto* descriptor = find_object_category_descriptor(category)) {
		return QString::fromStdString(descriptor->label);
	}
	return "Object";
}

} // namespace

ObjectDataImportDialog::~ObjectDataImportDialog() = default;

ObjectDataImportDialog::ObjectDataImportDialog(ObjectDataImportPackage package, QWidget* parent)
	: QDialog(parent), import_package(std::make_unique<ObjectDataImportPackage>(std::move(package))) {
	setWindowTitle("Import Object Data");
	resize(1100, 560);

	apply_plan = std::make_unique<ObjectDataApplyPlan>();

	tabs = new QTabWidget(this);

	validation_table = new QTableWidget(0, 5, this);
	validation_table->setHorizontalHeaderLabels({ "Severity", "Category", "Object", "Field", "Message" });
	validation_table->horizontalHeader()->setStretchLastSection(true);
	validation_table->setSelectionBehavior(QAbstractItemView::SelectRows);
	validation_table->setEditTriggers(QAbstractItemView::NoEditTriggers);

	addition_table = new QTableWidget(0, 3, this);
	addition_table->setHorizontalHeaderLabels({ "Category", "Name", "ID" });
	addition_table->setSelectionBehavior(QAbstractItemView::SelectRows);
	addition_table->setEditTriggers(QAbstractItemView::NoEditTriggers);
	addition_table->setWordWrap(false);
	addition_table->setTextElideMode(Qt::ElideNone);

	auto* addition_header = addition_table->horizontalHeader();
	addition_header->setStretchLastSection(false);
	addition_header->setSectionResizeMode(0, QHeaderView::ResizeToContents);
	addition_header->setSectionResizeMode(1, QHeaderView::Stretch);
	addition_header->setSectionResizeMode(2, QHeaderView::ResizeToContents);

	conflict_table = new QTableWidget(0, 4, this);
	conflict_table->setHorizontalHeaderLabels({ "Category", "Name", "ID", "Overwrite" });
	conflict_table->setSelectionBehavior(QAbstractItemView::SelectRows);
	conflict_table->setWordWrap(false);
	conflict_table->setTextElideMode(Qt::ElideNone);

	auto* conflict_header = conflict_table->horizontalHeader();
	conflict_header->setStretchLastSection(false);
	conflict_header->setSectionResizeMode(0, QHeaderView::ResizeToContents);
	conflict_header->setSectionResizeMode(1, QHeaderView::Stretch);
	conflict_header->setSectionResizeMode(2, QHeaderView::ResizeToContents);
	conflict_header->setSectionResizeMode(3, QHeaderView::Fixed);
	conflict_header->resizeSection(3, 88);

	tabs->addTab(validation_table, "Validation");
	tabs->addTab(addition_table, "New Objects");
	tabs->addTab(conflict_table, "Conflicts");

	proceed_with_warnings = new QCheckBox("Proceed despite warnings", this);
	proceed_with_warnings->setChecked(false);

	auto* button_box = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel, this);
	import_button = button_box->button(QDialogButtonBox::Ok);
	import_button->setText("Import");

	connect(button_box, &QDialogButtonBox::accepted, this, [this]() {
		if (!import_button->isEnabled()) {
			return;
		}

		*apply_plan = build_import_plan(*import_package, selected_overwrite_ids());
		const ImportValidationReport dry_run = dry_run_import_plan(*apply_plan);
		if (dry_run.has_blocking_errors()) {
			populate_validation_table(dry_run);
			tabs->setCurrentIndex(0);
			update_import_enabled();
			return;
		}

		if (!apply_object_data_plan(*apply_plan)) {
			populate_validation_table(ImportValidationReport{ {
				ValidationIssue{ ValidationSeverity::error, "apply", {}, {}, "Import failed and was rolled back.", {} },
			} });
			tabs->setCurrentIndex(0);
			update_import_enabled();
			return;
		}

		result = Result::applied;
		accept();
	});
	connect(button_box, &QDialogButtonBox::rejected, this, &QDialog::reject);
	connect(proceed_with_warnings, &QCheckBox::toggled, this, &ObjectDataImportDialog::update_import_enabled);

	auto* layout = new QVBoxLayout(this);
	layout->addWidget(new QLabel("Review validation results and choose which conflicting objects to overwrite.", this));
	layout->addWidget(tabs, 1);
	layout->addWidget(proceed_with_warnings);
	layout->addWidget(button_box);

	*apply_plan = build_import_plan(*import_package, {});
	populate_validation_table(apply_plan->validation);
	populate_addition_table();
	populate_conflict_table();
	update_import_enabled();

	if (apply_plan->validation.has_blocking_errors()) {
		tabs->setCurrentIndex(0);
	} else if (!apply_plan->conflicts.empty()) {
		tabs->setCurrentIndex(2);
	} else if (!apply_plan->additions.empty()) {
		tabs->setCurrentIndex(1);
	}
}

void ObjectDataImportDialog::populate_validation_table(const ImportValidationReport& report) {
	validation_table->setRowCount(static_cast<int>(report.issues.size()));
	for (int row = 0; row < static_cast<int>(report.issues.size()); ++row) {
		const ValidationIssue& issue = report.issues.at(row);
		validation_table->setItem(row, 0, new QTableWidgetItem(severity_label(issue.severity)));
		validation_table->setItem(row, 1, new QTableWidgetItem(QString::fromStdString(issue.category)));
		validation_table->setItem(row, 2, new QTableWidgetItem(QString::fromStdString(issue.object_id)));
		validation_table->setItem(row, 3, new QTableWidgetItem(QString::fromStdString(issue.field)));
		validation_table->setItem(row, 4, new QTableWidgetItem(QString::fromStdString(issue.message)));
	}
}

void ObjectDataImportDialog::populate_addition_table() {
	addition_table->setRowCount(static_cast<int>(apply_plan->additions.size()));
	for (int row = 0; row < static_cast<int>(apply_plan->additions.size()); ++row) {
		const ImportConflict& addition = apply_plan->additions.at(row);
		addition_table->setItem(row, 0, new QTableWidgetItem(category_label(addition.category)));
		addition_table->setItem(row, 1, new QTableWidgetItem(QString::fromStdString(addition.display_name)));
		addition_table->setItem(row, 2, new QTableWidgetItem(QString::fromStdString(addition.id)));
	}

	int name_column_width = 320;
	const QFontMetrics name_metrics(addition_table->font());
	for (int row = 0; row < addition_table->rowCount(); ++row) {
		if (const QTableWidgetItem* name_item = addition_table->item(row, 1)) {
			name_column_width = std::max(name_column_width, name_metrics.horizontalAdvance(name_item->text()) + 24);
		}
	}
	addition_table->setColumnWidth(1, name_column_width);

	tabs->setTabText(1, QString("New Objects (%1)").arg(apply_plan->additions.size()));
}

void ObjectDataImportDialog::populate_conflict_table() {
	conflict_table->setRowCount(static_cast<int>(apply_plan->conflicts.size()));
	for (int row = 0; row < static_cast<int>(apply_plan->conflicts.size()); ++row) {
		const ImportConflict& conflict = apply_plan->conflicts.at(row);
		conflict_table->setItem(row, 0, new QTableWidgetItem(category_label(conflict.category)));
		conflict_table->setItem(row, 1, new QTableWidgetItem(QString::fromStdString(conflict.display_name)));
		conflict_table->setItem(row, 2, new QTableWidgetItem(QString::fromStdString(conflict.id)));

		auto* overwrite = new QTableWidgetItem;
		overwrite->setFlags(Qt::ItemIsUserCheckable | Qt::ItemIsEnabled);
		overwrite->setCheckState(Qt::Checked);
		conflict_table->setItem(row, 3, overwrite);
	}

	int name_column_width = 320;
	const QFontMetrics name_metrics(conflict_table->font());
	for (int row = 0; row < conflict_table->rowCount(); ++row) {
		if (const QTableWidgetItem* name_item = conflict_table->item(row, 1)) {
			name_column_width = std::max(name_column_width, name_metrics.horizontalAdvance(name_item->text()) + 24);
		}
	}
	conflict_table->setColumnWidth(1, name_column_width);

	tabs->setTabText(2, QString("Conflicts (%1)").arg(apply_plan->conflicts.size()));
}

std::unordered_set<std::string> ObjectDataImportDialog::selected_overwrite_ids() const {
	std::unordered_set<std::string> overwrite_ids;
	for (int row = 0; row < conflict_table->rowCount(); ++row) {
		if (conflict_table->item(row, 3)->checkState() == Qt::Checked) {
			overwrite_ids.insert(conflict_table->item(row, 2)->text().toStdString());
		}
	}
	return overwrite_ids;
}

void ObjectDataImportDialog::update_import_enabled() {
	const bool has_errors = apply_plan->validation.has_blocking_errors();
	const bool warnings_ok = !apply_plan->validation.has_warnings() || proceed_with_warnings->isChecked();
	import_button->setEnabled(!has_errors && warnings_ok);
	proceed_with_warnings->setVisible(apply_plan->validation.has_warnings());
}
