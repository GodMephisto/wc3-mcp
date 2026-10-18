---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/blp/DxtCodec.h"
ref: HEAD
---

#pragma once

#include <QByteArray>
#include <QImage>
#include <QString>

namespace darblp {

enum class DxtFormat
{
    Dxt1,
    Dxt3,
    Dxt5
};

class DxtCodec
{
public:
    static bool decode(const QByteArray &blocks,
                       int width,
                       int height,
                       DxtFormat format,
                       QImage &image,
                       QString *errorMessage = nullptr);

    static bool encode(const QImage &image,
                       DxtFormat format,
                       QByteArray &blocks,
                       QString *errorMessage = nullptr);
};

} // namespace darblp
