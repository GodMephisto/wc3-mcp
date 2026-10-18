---
type: vendor-doc
source_repo: "https://github.com/stijnherfst/HiveWE"
source_path: "src/menus/settings_editor.h"
ref: HEAD
---

#pragma once

#include "ui_settings_editor.h"

class SettingsEditor : public QDialog {
	Q_OBJECT

public:
	explicit SettingsEditor(QWidget* parent = nullptr);
private:
	Ui::SettingsEditor ui;
	void save() const;
};