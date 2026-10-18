---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/blp/BlpReader.cpp"
ref: HEAD
---

#include "blp/BlpReader.h"

#include "blp/BlpDecoder.h"

#include <QDataStream>
#include <QFile>
#include <QFileInfo>

namespace darblp {

bool BlpReader::readHeader(const QString &path, BlpHeaderInfo &info, QString *errorMessage)
{
    Blp2Header header;
    qint64 fileSize = 0;
    if (!readRawHeader(path, header, fileSize, errorMessage)) {
        return false;
    }

    int mipCount = 0;
    for (int i = 0; i < BlpMaxMipmaps; ++i) {
        if (header.mipOffsets[i] != 0 && header.mipSizes[i] != 0) {
            ++mipCount;
        }
    }

    info.version = QStringLiteral("BLP2");
    info.width = header.width;
    info.height = header.height;
    info.mipmapCount = mipCount;
    info.alphaDepth = header.alphaDepth;
    info.compression = blpCompressionName(header.encoding, header.alphaDepth, header.alphaEncoding);
    info.fileSize = fileSize;
    return true;
}

bool BlpReader::readImage(const QString &path, QImage &image, QString *errorMessage)
{
    Blp2Header header;
    qint64 fileSize = 0;
    if (!readRawHeader(path, header, fileSize, errorMessage)) {
        return false;
    }

    Q_UNUSED(fileSize)
    return BlpDecoder::decode(path, header, image, errorMessage);
}

bool BlpReader::readRawHeader(const QString &path, Blp2Header &header, qint64 &fileSize, QString *errorMessage)
{
    QFile file(path);
    if (!file.open(QIODevice::ReadOnly)) {
        if (errorMessage) {
            *errorMessage = file.errorString();
        }
        return false;
    }

    fileSize = file.size();
    QDataStream stream(&file);
    stream.setByteOrder(QDataStream::LittleEndian);

    if (stream.readRawData(header.magic, 4) != 4
        || header.magic[0] != 'B'
        || header.magic[1] != 'L'
        || header.magic[2] != 'P'
        || header.magic[3] != '2') {
        if (errorMessage) {
            *errorMessage = QStringLiteral("Unsupported BLP file. Only BLP2 headers are recognized in this build.");
        }
        return false;
    }

    stream >> header.type;
    stream >> header.encoding;
    stream >> header.alphaDepth;
    stream >> header.alphaEncoding;
    stream >> header.hasMipmaps;
    stream >> header.width;
    stream >> header.height;

    for (quint32 &offset : header.mipOffsets) {
        stream >> offset;
    }

    for (quint32 &size : header.mipSizes) {
        stream >> size;
    }

    for (quint32 &color : header.palette) {
        stream >> color;
    }

    if (stream.status() != QDataStream::Ok) {
        if (errorMessage) {
            *errorMessage = QStringLiteral("BLP2 header is truncated or invalid.");
        }
        return false;
    }

    return true;
}

} // namespace darblp
