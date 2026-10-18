---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/gui/Theme.h"
ref: HEAD
---

#pragma once

class QApplication;

namespace darblp::gui {

class Theme
{
public:
    static void apply(QApplication &app);
};

} // namespace darblp::gui
