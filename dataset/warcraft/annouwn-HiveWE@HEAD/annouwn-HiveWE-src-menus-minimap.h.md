---
type: vendor-doc
source_repo: "https://github.com/annouwn/HiveWE"
source_path: "src/menus/minimap.h"
ref: HEAD
---

#pragma once

#include <QWidget>
#include <memory>

import Texture;

namespace Ui {
class Minimap;
}

class Minimap : public QWidget {
	Q_OBJECT

public:
	Minimap(QWidget* parent = nullptr);
	~Minimap() override;

public slots:
	void set_minimap(Texture texture);

signals:
	/// point contains the location clicked on the minimap in the range [0..1]
	void clicked(QPointF point);
private:
	std::unique_ptr<Ui::Minimap> ui;

	void mousePressEvent(QMouseEvent* event) override;
	void mouseReleaseEvent(QMouseEvent* event) override;
	void mouseMoveEvent(QMouseEvent* event) override;
};
