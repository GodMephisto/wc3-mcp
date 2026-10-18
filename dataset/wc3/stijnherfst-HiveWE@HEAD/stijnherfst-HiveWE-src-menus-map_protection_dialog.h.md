---
type: vendor-doc
source_repo: "https://github.com/stijnherfst/HiveWE"
source_path: "src/menus/map_protection_dialog.h"
ref: HEAD
---

#pragma once

#include <QDialog>

class QCheckBox;

class MapProtectionDialog : public QDialog {
	Q_OBJECT

  public:
	explicit MapProtectionDialog(QWidget* parent = nullptr);

  private:
	QCheckBox* removeWorldEditorFiles = nullptr;
	QCheckBox* removeMetadata = nullptr;
	QCheckBox* obfuscateScript = nullptr;
	QCheckBox* junkHeaderOffset = nullptr;
	QCheckBox* encryptImports = nullptr;
	QCheckBox* askEachExport = nullptr;

	void save() const;
};
