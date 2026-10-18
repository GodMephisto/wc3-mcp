---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/core/ImageData.h"
ref: HEAD
---

#pragma once

#include <QImage>
#include <QString>

namespace darblp {

class ImageData
{
public:
    ImageData() = default;
    explicit ImageData(QImage image);

    bool isValid() const;
    bool hasAlpha() const;

    int width() const;
    int height() const;

    QImage image;
    QString sourcePath;
    QString format;
    QString compression;
    int mipmapCount = 1;
    int alphaDepth = 0;
    qint64 fileSize = 0;
};

} // namespace darblp
