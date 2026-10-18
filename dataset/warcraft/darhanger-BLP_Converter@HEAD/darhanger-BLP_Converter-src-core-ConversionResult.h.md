---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/core/ConversionResult.h"
ref: HEAD
---

#pragma once

#include <QMetaType>
#include <QString>

namespace darblp {

struct ConversionResult
{
    bool success = false;
    QString inputPath;
    QString outputPath;
    QString errorMessage;
};

} // namespace darblp

Q_DECLARE_METATYPE(darblp::ConversionResult)
