---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/blp/BlpDecoder.h"
ref: HEAD
---

#pragma once

#include "blp/BlpHeader.h"

#include <QImage>
#include <QString>

namespace darblp {

class BlpDecoder
{
public:
    static bool decode(const QString &path,
                       const Blp2Header &header,
                       QImage &image,
                       QString *errorMessage = nullptr);
};

} // namespace darblp
