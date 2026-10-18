---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "export/inigenerator.cpp"
ref: HEAD
---

#include "inigenerator.h"
#include "elements/uielementdata.h"

#include <QFile>
#include <QFileInfo>
#include <QMap>
#include <QSet>
#include <QDebug>

IniGenerator::IniGenerator(QObject *parent)
    : QObject(parent)
{
}

bool IniGenerator::generate(const QList<UiElementData> &elements,
                             const QString &projectDir,
                             const QString &outputPath,
                             bool war3Mode,
                             QString *errorMsg)
{
    if (elements.isEmpty()) {
        QFile file(outputPath);
        if (!file.open(QIODevice::WriteOnly | QIODevice::Text)) {
            if (errorMsg)
                *errorMsg = QString("Cannot write to file: %1").arg(outputPath);
            return false;
        }
        file.write("[UI]\n");
        file.close();
        return true;
    }

    // Build lookup map
    QMap<QString, UiElementData> dataMap;
    for (const auto &el : elements) {
        dataMap[el.name] = el;
    }

    QSet<QString> written;
    QString output;
    output = "[UI]\n";

    // Find root elements (no parent) and write them first, then children
    for (const auto &el : elements) {
        if (el.parent.isEmpty() || !dataMap.contains(el.parent)) {
            writeElement(output, el, dataMap, written, war3Mode);
        }
    }

    // Write any remaining elements not yet written
    for (const auto &el : elements) {
        if (!written.contains(el.name)) {
            writeElement(output, el, dataMap, written, war3Mode);
        }
    }

    QFile file(outputPath);
    if (!file.open(QIODevice::WriteOnly | QIODevice::Text)) {
        if (errorMsg)
            *errorMsg = QString("Cannot write to file: %1").arg(outputPath);
        qWarning() << "Failed to write INI:" << outputPath;
        return false;
    }

    file.write(output.toUtf8());
    file.close();
    return true;
}

QString IniGenerator::propertyLine(const QString &prefix, const QString &key, const QString &value) const
{
    return QString("%1%2 = %3\n").arg(prefix, key, value);
}

QString IniGenerator::makeTextureRef(const QString &texture)
{
    QString ref = texture;
    if (ref.contains("textures/") || ref.contains("textures\\")) {
        int idx = ref.indexOf("textures");
        if (idx >= 0) {
            ref = "war3mapUI\\" + ref.mid(idx);
        }
    }
    ref.replace('/', '\\');
    return ref;
}

void IniGenerator::writeElement(QString &output, const UiElementData &element,
                                 const QMap<QString, UiElementData> &dataMap,
                                 QSet<QString> &written,
                                 bool war3Mode) const
{
    if (written.contains(element.name)) return;
    written.insert(element.name);

    QString prefix = QString("Frame_%1_").arg(element.name);

    output += propertyLine(prefix, "Type", element.type);

    // Calculate parent-relative coordinates in pixel space
    double relX = element.x;
    double relY = element.y;
    if (!element.parent.isEmpty() && dataMap.contains(element.parent)) {
        const auto &parent = dataMap[element.parent];
        relX -= parent.x;
        relY -= parent.y;
    }

    if (war3Mode) {
        output += propertyLine(prefix, "X",
            QString::number(relX / 1920.0 * 0.8, 'f', 4));
        // War3 Y uses bottom-left origin; convert from top-left relative offset
        output += propertyLine(prefix, "Y",
            QString::number(-relY / 1080.0 * 0.6, 'f', 4));
        output += propertyLine(prefix, "Width", QString::number(element.width / 1920.0 * 0.8, 'f', 4));
        output += propertyLine(prefix, "Height", QString::number(element.height / 1080.0 * 0.6, 'f', 4));
    } else {
        output += propertyLine(prefix, "X", QString::number(relX, 'f', 1));
        output += propertyLine(prefix, "Y", QString::number(relY, 'f', 1));
        output += propertyLine(prefix, "Width", QString::number(element.width, 'f', 1));
        output += propertyLine(prefix, "Height", QString::number(element.height, 'f', 1));
    }

    if (!element.parent.isEmpty()) {
        output += propertyLine(prefix, "Parent", element.parent);
    }

    if (element.hasTexture && !element.texture.isEmpty()) {
        output += propertyLine(prefix, "Texture", makeTextureRef(element.texture));
    }

    // Type-specific properties
    if (element.type == "BUTTON") {
        if (!element.normalTexture.isEmpty())
            output += propertyLine(prefix, "NormalTexture", makeTextureRef(element.normalTexture));
        if (!element.highlightTexture.isEmpty())
            output += propertyLine(prefix, "HighlightTexture", makeTextureRef(element.highlightTexture));
    } else if (element.type == "MODEL") {
        if (!element.modelPath.isEmpty())
            output += propertyLine(prefix, "Model", element.modelPath);
    } else if (element.type == "TEXT") {
        if (!element.textContent.isEmpty())
            output += propertyLine(prefix, "Text", element.textContent);
        if (element.fontSize > 0)
            output += propertyLine(prefix, "FontSize", QString::number(element.fontSize, 'f', 0));
        if (!element.textColor.isEmpty())
            output += propertyLine(prefix, "TextColor", element.textColor);
    }

    // Extended properties
    for (auto it = element.properties.begin(); it != element.properties.end(); ++it) {
        output += propertyLine(prefix, it.key(), it.value());
    }

    output += "\n";

    // Write children — lookup by parent field at runtime
    for (auto it = dataMap.constBegin(); it != dataMap.constEnd(); ++it) {
        if (!written.contains(it.key()) && it.value().parent == element.name) {
            writeElement(output, it.value(), dataMap, written, war3Mode);
        }
    }
}
