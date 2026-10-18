---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/blp/BlpWriter.h"
ref: HEAD
---

#pragma once

#include "core/ConversionOptions.h"

#include <QImage>
#include <QString>

namespace darblp {

class BlpWriter
{
public:
    static bool writeImage(const QString &path,
                           const QImage &image,
                           const ConversionOptions &options,
                           QString *errorMessage = nullptr);
};

} // namespace darblp
