---
type: vendor-doc
source_repo: "https://github.com/sidsukana/QBlpPlugin"
source_path: "qblpplugin.cpp"
ref: HEAD
---

#include "qblpplugin.h"
#include "qblphandler.h"

QBlpPlugin::QBlpPlugin(QObject *parent) :
    QImageIOPlugin(parent)
{
}

QBlpPlugin::~QBlpPlugin()
{
}

QStringList QBlpPlugin::keys() const
{
    return QStringList() << "blp";
}

QImageIOPlugin::Capabilities QBlpPlugin::capabilities(
    QIODevice *device, const QByteArray &format) const
{
    if (format == "blp")
        return Capabilities(CanRead);//TODO: add CanWrite support

    if (!(format.isEmpty() && device->isOpen()))
        return {};

    Capabilities cap;
    if (device->isReadable() && QBlpHandler::canRead(device))
        cap |= CanRead;
    return cap;
}

QImageIOHandler *QBlpPlugin::create(
    QIODevice *device, const QByteArray &format) const
{
    QImageIOHandler *handler = new QBlpHandler;
    handler->setDevice(device);
    handler->setFormat(format);
    return handler;
}

#if QT_VERSION < 0x050000
Q_EXPORT_STATIC_PLUGIN(QBlpPlugin)
Q_EXPORT_PLUGIN2(QBlp, QBlpPlugin)
#endif // QT_VERSION < 0x050000
