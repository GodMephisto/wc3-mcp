---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "src/trigger_editor/variable_editor.h"
ref: HEAD
---

#pragma once

#include <qwidget.h>

#include "ui_variable_editor.h"

import Triggers;

class VariableEditor : public QWidget {
	Q_OBJECT
public:
	Ui::VariableEditor ui;
	VariableEditor(TriggerVariable& variable);
};
