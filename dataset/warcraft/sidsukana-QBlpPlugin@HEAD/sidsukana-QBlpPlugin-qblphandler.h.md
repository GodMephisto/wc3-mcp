---
type: vendor-doc
source_repo: "https://github.com/sidsukana/QBlpPlugin"
source_path: "qblphandler.h"
ref: HEAD
---

#ifndef QBLPHANDLER_H
#define QBLPHANDLER_H

#include <QImageIOHandler>
#include <QImage>
#include <QVariant>

#define BLP_MAGIC "BLP2"

struct BLPHeader
{
    char magic[4];
    quint32 type;
    quint8 compression;
    quint8 alphaDepth;
    quint8 alphaType;
    quint8 hasMips;
    quint32 width;
    quint32 height;
    quint32 mipmapOffset[16];
    quint32 mipmapLength[16];
};

class QBlpHandler : public QImageIOHandler
{
public:
    QBlpHandler();
    ~QBlpHandler();

    bool canRead() const;
    bool read(QImage *image);
    //bool write(const QImage &image);

    static bool canRead(QIODevice *device);

    QVariant option(ImageOption option) const;
    //void setOption(ImageOption option, const QVariant &value);
    bool supportsOption(ImageOption option) const;

};

#endif // QBLPHANDLER_H
