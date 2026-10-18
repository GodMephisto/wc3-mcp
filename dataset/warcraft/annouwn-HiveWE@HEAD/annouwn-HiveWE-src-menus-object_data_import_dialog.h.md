---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/object_data_import_dialog.h"
ref: HEAD
---

#pragma once

#include <QDialog>
#include <memory>
#include <unordered_set>

struct ObjectDataImportPackage;
struct ObjectDataApplyPlan;
struct ImportValidationReport;

class QCheckBox;
class QTableWidget;
class QTabWidget;
class QDialogButtonBox;
class QPushButton;

class ObjectDataImportDialog : public QDialog {
	Q_OBJECT

public:
	enum class Result {
		cancelled,
		applied,
	};

	explicit ObjectDataImportDialog(ObjectDataImportPackage package, QWidget* parent = nullptr);
	~ObjectDataImportDialog() override;

	Result dialog_result() const {
		return result;
	}

private:
	std::unique_ptr<ObjectDataImportPackage> import_package;
	std::unique_ptr<ObjectDataApplyPlan> apply_plan;
	Result result = Result::cancelled;

	QTabWidget* tabs = nullptr;
	QTableWidget* validation_table = nullptr;
	QTableWidget* addition_table = nullptr;
	QTableWidget* conflict_table = nullptr;
	QCheckBox* proceed_with_warnings = nullptr;
	QPushButton* import_button = nullptr;

	void populate_validation_table(const ImportValidationReport& report);
	void populate_addition_table();
	void populate_conflict_table();
	std::unordered_set<std::string> selected_overwrite_ids() const;
	void update_import_enabled();
};
