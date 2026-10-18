---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/blp/MipmapGenerator.h"
ref: HEAD
---

#pragma once

#include <QImage>
#include <QVector>

namespace darblp {

class MipmapGenerator
{
public:
    static QVector<QImage> generate(const QImage &baseImage);
};

} // namespace darblp
