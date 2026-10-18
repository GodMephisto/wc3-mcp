---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/region_palette.h"
ref: HEAD
---

#pragma once

#include <QColor>
#include <QLabel>
#include <QLineEdit>
#include <QListWidget>
#include <QPushButton>
#include <QShortcut>
#include <QVBoxLayout>

#include "palette.h"
#include "region_brush.h"

class RegionPalette : public Palette {
  public:
	explicit RegionPalette(QWidget* parent = nullptr);
	~RegionPalette();

	void activate_palette();
	void deactivate(QRibbonTab* tab) override;
	RegionBrush& current_brush() { return brush; }

  private:
	RegionBrush brush;

	QListWidget* region_list = new QListWidget(this);
	QPushButton* new_region = new QPushButton("New Region", this);
	QPushButton* delete_region = new QPushButton("Delete Region", this);
	QLineEdit* name = new QLineEdit(this);
	QPushButton* color = new QPushButton("Choose Color", this);
	QFrame* selection_summary = nullptr;
	QLabel* selection_summary_title = nullptr;
	QLabel* selection_summary_meta = nullptr;

	QRibbonTab* ribbon_tab = new QRibbonTab;
	QRibbonButton* selection_mode = new QRibbonButton;
	QShortcut* change_mode_parent = nullptr;

	void refresh_region_list();
	void sync_region_fields();
	void update_color_button(const QColor& color);
};
