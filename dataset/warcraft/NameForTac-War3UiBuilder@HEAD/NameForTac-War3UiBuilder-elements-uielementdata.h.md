---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "elements/uielementdata.h"
ref: HEAD
---

#ifndef UIELEMENTDATA_H
#define UIELEMENTDATA_H

#include <QString>
#include <QMap>
#include <QPointF>
#include <QSizeF>

struct UiElementData
{
    QString name;
    QString type = "SIMPLEFRAME";
    QString texture;
    bool hasTexture = false;

    double x = 0.0;
    double y = 0.0;
    double width = 100.0;
    double height = 100.0;

    bool visible = true;
    bool locked = false;

    QString parent;

    // Organizational group (for tree panel, not export)
    QString group;

    // Type-specific properties
    QString normalTexture;      // BUTTON: normal state texture
    QString highlightTexture;   // BUTTON: highlighted state texture
    QString modelPath;          // MODEL: path to .mdx model
    QString textContent;        // TEXT: displayed text
    double fontSize = 14.0;     // TEXT: font size in pixels
    QString textColor = "#FFFFFF"; // TEXT: hex color

    QMap<QString, QString> properties;

    bool isValid() const { return !name.isEmpty(); }
};

#endif // UIELEMENTDATA_H
