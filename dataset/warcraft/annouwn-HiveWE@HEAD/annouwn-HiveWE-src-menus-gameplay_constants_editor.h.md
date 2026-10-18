---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/gameplay_constants_editor.h"
ref: HEAD
---

#pragma once

#include <QDialog>
#include <QLineEdit>

class GameplayConstantsEditor : public QDialog {
	Q_OBJECT
public:
	explicit GameplayConstantsEditor(QWidget* parent = nullptr);

private:
	QLineEdit* search;
};
