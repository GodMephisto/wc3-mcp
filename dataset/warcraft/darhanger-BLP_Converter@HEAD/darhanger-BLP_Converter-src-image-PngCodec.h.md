---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/image/PngCodec.h"
ref: HEAD
---

#pragma once

#include <QImage>
#include <QString>

namespace darblp {

class PngCodec
{
public:
    static bool read(const QString &path, QImage &image, QString *errorMessage = nullptr);
    static bool write(const QString &path, const QImage &image, QString *errorMessage = nullptr);
};

} // namespace darblp
