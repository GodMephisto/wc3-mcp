---
type: vendor-doc
source_repo: "https://github.com/stijnherfst/HiveWE"
source_path: "src/main_window/map_widget.h"
ref: HEAD
---

#pragma once

#include <QWidget>

/// Hosts the Vulkan map viewport in the main window. The viewport is a native window, so widgets
/// shown on top of it (like the minimap) must be native too.
class MapWidget : public QWidget {
	Q_OBJECT

  public:
	explicit MapWidget(QWidget* parent = nullptr);
};
