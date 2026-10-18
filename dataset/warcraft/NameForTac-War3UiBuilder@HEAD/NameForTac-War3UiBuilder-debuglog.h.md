---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "debuglog.h"
ref: HEAD
---

#ifndef DEBUGLOG_H
#define DEBUGLOG_H

#include <QString>
#include <QFile>
#include <QDateTime>
#include <QDir>

inline void debugLog(const QString &msg)
{
    QString logPath = QDir::tempPath() + "/War3UiBuilder_debug.log";
    QFile f(logPath);
    if (f.open(QIODevice::Append | QIODevice::Text)) {
        f.write(QDateTime::currentDateTime().toString("yyyy-MM-dd hh:mm:ss.zzz ").toUtf8());
        f.write(msg.toUtf8());
        f.write("\n");
    }
}

#endif // DEBUGLOG_H
