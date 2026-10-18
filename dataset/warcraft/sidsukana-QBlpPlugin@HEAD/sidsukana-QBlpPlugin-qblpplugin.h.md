---
type: vendor-doc
source_repo: "https://github.com/sidsukana/QBlpPlugin"
source_path: "qblpplugin.h"
ref: HEAD
---

#ifndef QBLPPLUGIN_H
#define QBLPPLUGIN_H

#include <QImageIOPlugin>


class QBlpPlugin : public QImageIOPlugin
{
    Q_OBJECT
#if QT_VERSION >= 0x050000
    Q_PLUGIN_METADATA(IID "org.qt-project.Qt.QImageIOHandlerFactoryInterface" FILE "QBlpPlugin.json")
#endif // QT_VERSION >= 0x050000
    
public:
    QBlpPlugin(QObject *parent = 0);
    ~QBlpPlugin();
    QStringList keys() const;
    Capabilities capabilities(QIODevice *device,
                    const QByteArray &format) const;
    QImageIOHandler *create(QIODevice *device,
                               const QByteArray &format = QByteArray()) const;
};

#endif // QBLPPLUGIN_H
