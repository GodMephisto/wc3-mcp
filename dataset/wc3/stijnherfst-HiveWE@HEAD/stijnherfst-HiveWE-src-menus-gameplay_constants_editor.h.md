---
type: vendor-doc
source_repo: "https://github.com/stijnherfst/HiveWE"
source_path: "src/menus/gameplay_constants_editor.h"
ref: HEAD
---

#pragma once

#include <QDialog>
#include <QLineEdit>

import GameplayConstants;
import TriggerStrings;

class GameplayConstantsEditor : public QDialog {
	Q_OBJECT
public:
	GameplayConstantsEditor(QWidget* parent, GameplayConstants& constants, TriggerStrings& trigger_strings);

private:
	QLineEdit* search;
};
