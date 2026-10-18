---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/pathing_palette.h"
ref: HEAD
---

#pragma once

#include <QDialog>

#include "ui_pathing_palette.h"

#include "palette.h"
#include "pathing_brush.h"

class PathingPalette : public Palette {
	Q_OBJECT

public:
	explicit PathingPalette(QWidget* parent = nullptr);
	~PathingPalette();
	void activate_palette();
	PathingBrush& current_brush() { return brush; }
	void sync_brush_controls();

private:
	Ui::PathingPalette ui;
	PathingBrush brush;

	QRibbonTab* ribbon_tab = new QRibbonTab;
	QRibbonButton* selection_mode = new QRibbonButton;
	QRibbonButton* import_pathing = new QRibbonButton;
	QRibbonButton* export_pathing = new QRibbonButton;

public slots:
	void deactivate(QRibbonTab* tab) override;
};
