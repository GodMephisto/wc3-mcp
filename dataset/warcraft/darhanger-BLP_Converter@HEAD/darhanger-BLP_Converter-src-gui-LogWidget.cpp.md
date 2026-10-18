---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/gui/LogWidget.cpp"
ref: HEAD
---

#include "gui/LogWidget.h"

#include "utils/Logger.h"

#include <QSizePolicy>

namespace darblp::gui {

LogWidget::LogWidget(QWidget *parent)
    : QTextEdit(parent)
{
    setReadOnly(true);
    setMinimumHeight(82);
    setMaximumHeight(92);
    setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Fixed);
    setPlaceholderText(QStringLiteral("Log"));
}

void LogWidget::appendInfo(const QString &message)
{
    append(darblp::Logger::line(message).toHtmlEscaped());
}

void LogWidget::appendSuccess(const QString &message)
{
    append(QStringLiteral("<span style=\"color:#4ADE80;\">%1</span>")
        .arg(darblp::Logger::success(message).toHtmlEscaped()));
}

void LogWidget::appendError(const QString &message)
{
    append(QStringLiteral("<span style=\"color:#FF5C5C;\">%1</span>")
        .arg(darblp::Logger::error(message).toHtmlEscaped()));
}

} // namespace darblp::gui
