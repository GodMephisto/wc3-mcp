---
type: vendor-doc
source_repo: "https://github.com/sidsukana/QBlpPlugin"
source_path: "qblphandler.cpp"
ref: HEAD
---

#include <squish/squish.h>

#include "qblphandler.h"

QBlpHandler::QBlpHandler()
{
}

QBlpHandler::~QBlpHandler()
{
}

bool QBlpHandler::canRead() const
{
    return canRead(device());
}

bool QBlpHandler::read(QImage *image)
{
    QByteArray data = device()->readAll();

    if (data.size() == 0) {
        return false;
    }

    BLPHeader* header = reinterpret_cast<BLPHeader *>(data.data());

    if (qstrncmp(header->magic, BLP_MAGIC, 4) != 0) {
        return false;
    }

    if (header->type != 1) {
        return false;
    }

    quint8* imageData = reinterpret_cast<quint8 *>(data.data() + header->mipmapOffset[0]);
    QVector<quint8> uncompressed(header->width * header->height * 4);

    quint8 dxtVer;
    switch (header->alphaType)
    {
        case 0: dxtVer = squish::kDxt1; break;
        case 1: dxtVer = squish::kDxt3; break;
        case 7: dxtVer = squish::kDxt5; break;
        default: dxtVer = squish::kDxt1; break;
    }

    squish::DecompressImage(uncompressed.data(), header->width, header->height, imageData, dxtVer);

    QImage result(uncompressed.data(), header->width, header->height, QImage::Format_ARGB32);
    *image = result.rgbSwapped();
    return true;
}

bool QBlpHandler::canRead(QIODevice *device)
{
    return device->peek(4) == "BLP2";
}

QVariant QBlpHandler::option(ImageOption option) const
{
    if (option == Size) {
        QByteArray data = device()->readAll();

        if (data.size() == 0) {
            return QVariant();
        }

        BLPHeader* header = reinterpret_cast<BLPHeader *>(data.data());

        if (qstrncmp(header->magic, BLP_MAGIC, 4) != 0) {
            return QVariant();
        }

        if (header->type != 1) {
            return QVariant();
        }

        return QSize(header->width, header->height);
    }
    return QVariant();
}

bool QBlpHandler::supportsOption(ImageOption option) const
{
    return option == Size;
}
