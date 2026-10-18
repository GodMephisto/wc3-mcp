---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/core/Converter.h"
ref: HEAD
---

#pragma once

#include "core/ConversionOptions.h"
#include "core/ConversionResult.h"
#include "core/ImageData.h"

#include <QString>
#include <QStringList>
#include <QVector>

namespace darblp {

class Converter
{
public:
    ConversionResult convertFile(const QString &inputPath,
                                 const ConversionOptions &options,
                                 const QString &explicitOutputPath = QString()) const;

    QVector<ConversionResult> convertBatch(const QStringList &inputPaths,
                                           const ConversionOptions &options) const;

    bool loadImage(const QString &path, ImageData &image, QString *errorMessage = nullptr) const;

private:
    QString buildOutputPath(const QString &inputPath,
                            const ConversionOptions &options,
                            const QString &explicitOutputPath) const;
    static QImage prepareForExport(const QImage &image, const ConversionOptions &options);
    static int nextPowerOfTwo(int value);
};

} // namespace darblp
