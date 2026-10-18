---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/gui/ConversionWorker.h"
ref: HEAD
---

#pragma once

#include "core/ConversionOptions.h"
#include "core/ConversionResult.h"

#include <QObject>
#include <QStringList>

namespace darblp::gui {

class ConversionWorker : public QObject
{
    Q_OBJECT

public slots:
    void convert(QStringList files, darblp::ConversionOptions options);

signals:
    void fileStarted(QString path, int index, int total);
    void fileFinished(darblp::ConversionResult result, int index, int total);
    void finished(int successCount, int errorCount, QString outputFolder);
};

} // namespace darblp::gui
