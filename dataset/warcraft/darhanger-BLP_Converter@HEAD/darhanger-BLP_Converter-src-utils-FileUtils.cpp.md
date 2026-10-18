---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/utils/FileUtils.cpp"
ref: HEAD
---

#include "utils/FileUtils.h"

#include <QDir>
#include <QDirIterator>
#include <QFileInfo>
#include <QLocale>

namespace darblp {

QStringList FileUtils::supportedExtensions()
{
    return {QStringLiteral("png"), QStringLiteral("tga"), QStringLiteral("blp")};
}

bool FileUtils::isSupportedTexture(const QString &path)
{
    const QFileInfo info(path);
    return info.isFile() && supportedExtensions().contains(info.suffix().toLower());
}

QStringList FileUtils::collectSupportedFiles(const QString &path)
{
    QStringList files;
    const QFileInfo info(path);

    if (isSupportedTexture(path)) {
        files.push_back(info.absoluteFilePath());
        return files;
    }

    if (!info.isDir()) {
        return files;
    }

    QDirIterator iterator(info.absoluteFilePath(),
                          QDir::Files,
                          QDirIterator::Subdirectories);
    while (iterator.hasNext()) {
        const QString filePath = iterator.next();
        if (isSupportedTexture(filePath)) {
            files.push_back(QFileInfo(filePath).absoluteFilePath());
        }
    }
    files.sort(Qt::CaseInsensitive);
    return files;
}

QString FileUtils::prettyFileSize(qint64 bytes)
{
    return QLocale().formattedDataSize(bytes, 1, QLocale::DataSizeTraditionalFormat);
}

} // namespace darblp
