---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/gui/LogWidget.h"
ref: HEAD
---

#pragma once

#include <QTextEdit>

namespace darblp::gui {

class LogWidget : public QTextEdit
{
    Q_OBJECT

public:
    explicit LogWidget(QWidget *parent = nullptr);

public slots:
    void appendInfo(const QString &message);
    void appendSuccess(const QString &message);
    void appendError(const QString &message);
};

} // namespace darblp::gui
