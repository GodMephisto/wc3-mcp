---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "export/fdfgenerator.h"
ref: HEAD
---

#ifndef FDFGENERATOR_H
#define FDFGENERATOR_H

#include <QObject>
#include <QString>
#include <QList>
#include <QMap>
#include <QSet>

#include "elements/uielementdata.h"

class FdfGenerator : public QObject
{
    Q_OBJECT

public:
    explicit FdfGenerator(QObject *parent = nullptr);
    ~FdfGenerator() = default;

    bool exportFdf(const QList<UiElementData> &elements,
                   const QString &outputPath,
                   bool war3Mode = false,
                   QString *errorMsg = nullptr);

    QList<UiElementData> importFdf(const QString &filePath,
                                   QString *errorMsg = nullptr);

private:
    void writeElement(QString &output, const UiElementData &element,
                      const QMap<QString, UiElementData> &dataMap,
                      QSet<QString> &written,
                      int indent, bool war3Mode) const;

    // Parsing helpers
    struct FdfToken {
        enum Type { BlockStart, BlockEnd, Identifier, String, Comma, Semicolon, Equals, Eof };
        Type type;
        QString value;
    };

    class FdfParser {
    public:
        FdfParser(const QString &source);
        FdfToken nextToken();
        FdfToken peekToken();
        void consumeToken();

    private:
        QString m_source;
        int m_pos = 0;
        FdfToken m_peeked;
        bool m_hasPeeked = false;
        void skipWhitespaceAndComments();
    };

    static QString escapeString(const QString &s);
    static QString makeTextureRef(const QString &texture);
    static UiElementData parseFrameBlock(FdfParser &parser, QList<UiElementData> &result,
    const QString &frameType = QString());
};

#endif // FDFGENERATOR_H
