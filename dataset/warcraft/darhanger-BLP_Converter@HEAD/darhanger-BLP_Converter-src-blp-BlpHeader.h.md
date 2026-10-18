---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/blp/BlpHeader.h"
ref: HEAD
---

#pragma once

#include <QtGlobal>
#include <QString>

namespace darblp {

constexpr int BlpMaxMipmaps = 16;

struct Blp2Header
{
    char magic[4] = {'B', 'L', 'P', '2'};
    quint32 type = 1;
    quint8 encoding = 2;
    quint8 alphaDepth = 8;
    quint8 alphaEncoding = 7;
    quint8 hasMipmaps = 1;
    quint32 width = 0;
    quint32 height = 0;
    quint32 mipOffsets[BlpMaxMipmaps] = {};
    quint32 mipSizes[BlpMaxMipmaps] = {};
    quint32 palette[256] = {};
};

struct BlpHeaderInfo
{
    QString version;
    quint32 width = 0;
    quint32 height = 0;
    int mipmapCount = 0;
    int alphaDepth = 0;
    QString compression;
    qint64 fileSize = 0;
};

inline QString blpCompressionName(quint8 encoding, quint8 alphaDepth, quint8 alphaEncoding)
{
    if (encoding != 2) {
        return QStringLiteral("BLP2 encoding %1").arg(encoding);
    }

    if (alphaDepth == 0) {
        return QStringLiteral("DXT1");
    }

    if (alphaEncoding == 1) {
        return QStringLiteral("DXT3");
    }

    if (alphaEncoding == 7) {
        return QStringLiteral("DXT5");
    }

    return QStringLiteral("DXT alpha %1").arg(alphaEncoding);
}

} // namespace darblp
