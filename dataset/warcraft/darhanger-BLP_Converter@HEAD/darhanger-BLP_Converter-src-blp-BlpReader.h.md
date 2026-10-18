---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/blp/BlpReader.h"
ref: HEAD
---

#pragma once

#include "blp/BlpHeader.h"

#include <QImage>
#include <QString>

namespace darblp {

class BlpReader
{
public:
    static bool readHeader(const QString &path, BlpHeaderInfo &info, QString *errorMessage = nullptr);
    static bool readImage(const QString &path, QImage &image, QString *errorMessage = nullptr);

private:
    static bool readRawHeader(const QString &path, Blp2Header &header, qint64 &fileSize, QString *errorMessage);
};

} // namespace darblp
