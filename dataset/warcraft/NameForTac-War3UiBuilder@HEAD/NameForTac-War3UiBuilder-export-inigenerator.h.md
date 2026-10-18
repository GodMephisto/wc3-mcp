---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "export/inigenerator.h"
ref: HEAD
---

#ifndef INIGENERATOR_H
#define INIGENERATOR_H

#include <QObject>
#include <QString>
#include <QList>

struct UiElementData;

class IniGenerator : public QObject
{
    Q_OBJECT

public:
    explicit IniGenerator(QObject *parent = nullptr);
    ~IniGenerator() = default;

    bool generate(const QList<UiElementData> &elements,
                  const QString &projectDir,
                  const QString &outputPath,
                  bool war3Mode = false,
                  QString *errorMsg = nullptr);

private:
    QString propertyLine(const QString &prefix, const QString &key, const QString &value) const;
    void writeElement(QString &output, const UiElementData &element,
                      const QMap<QString, UiElementData> &dataMap,
                      QSet<QString> &written,
                      bool war3Mode) const;
    static QString makeTextureRef(const QString &texture);
};

#endif // INIGENERATOR_H
