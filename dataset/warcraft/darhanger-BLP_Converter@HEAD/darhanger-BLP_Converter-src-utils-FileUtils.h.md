---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/utils/FileUtils.h"
ref: HEAD
---

#pragma once

#include <QString>
#include <QStringList>

namespace darblp {

class FileUtils
{
public:
    static QStringList supportedExtensions();
    static bool isSupportedTexture(const QString &path);
    static QStringList collectSupportedFiles(const QString &path);
    static QString prettyFileSize(qint64 bytes);
};

} // namespace darblp
