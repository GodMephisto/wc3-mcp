---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/shortcut_config_dialog.h"
ref: HEAD
---

#pragma once

#include <QDialog>

class ShortcutConfigDialog : public QDialog {
	Q_OBJECT

public:
	explicit ShortcutConfigDialog(QWidget* parent = nullptr);
};
