---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/palette.h"
ref: HEAD
---

#pragma once

#include <QDialog>
#include <QShortcut>

#include <vector>

import QRibbon;

/// Palette is the base for all other palette kinds and facilitates things like brush switching and shortcut management
class Palette : public QDialog {
	Q_OBJECT

public:
	Palette(QWidget* parent = nullptr);
	~Palette();

	std::vector<QShortcut*> shortcuts;
	void monitor_activation(QWidget* widget = nullptr);

	//void addShortcut(const QKeySequence sequence, const std::vector<QWidget*>& attach_to);

signals:
	void ribbon_tab_requested(QRibbonTab* tab, QString name);
	void activated();

public slots:
	virtual void deactivate(QRibbonTab* tab) = 0;

protected:
	bool eventFilter(QObject* watched, QEvent* event) override;
};
