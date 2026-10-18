---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/blp/BlpEncoder.h"
ref: HEAD
---

#pragma once

#include "core/ConversionOptions.h"

#include <QImage>
#include <QString>

namespace darblp {

class BlpEncoder
{
public:
    static bool encode(const QString &path,
                       const QImage &image,
                       const ConversionOptions &options,
                       QString *errorMessage = nullptr);
};

} // namespace darblp
