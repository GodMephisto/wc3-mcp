---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "managers/projectmanager.cpp"
ref: HEAD
---

#include "projectmanager.h"
#include "export/inigenerator.h"
#include "export/fdfgenerator.h"
#include "elements/uielementdata.h"

#include <QDir>
#include <QFile>
#include <QFileInfo>
#include <QJsonDocument>
#include <QJsonObject>
#include <QJsonArray>
#include <QDebug>

ProjectManager::ProjectManager(QObject *parent)
    : QObject(parent)
    , m_iniGenerator(new IniGenerator(this))
    , m_fdfGenerator(new FdfGenerator(this))
{
}

void ProjectManager::newProject()
{
    m_elements.clear();
    m_projectDir.clear();
    m_projectFile.clear();
}

bool ProjectManager::saveProject(const QString &path, QString *errorMsg)
{
    QJsonObject root;
    QJsonArray elementsArray;

    for (const auto &el : m_elements) {
        QJsonObject obj;
        obj["name"] = el.name;
        obj["type"] = el.type;
        obj["texture"] = el.texture;
        obj["hasTexture"] = el.hasTexture;
        obj["x"] = el.x;
        obj["y"] = el.y;
        obj["width"] = el.width;
        obj["height"] = el.height;
        obj["parent"] = el.parent;
        obj["group"] = el.group;

        // Type-specific properties
        obj["normalTexture"] = el.normalTexture;
        obj["highlightTexture"] = el.highlightTexture;
        obj["modelPath"] = el.modelPath;
        obj["textContent"] = el.textContent;
        obj["fontSize"] = el.fontSize;
        obj["textColor"] = el.textColor;

        QJsonObject props;
        for (auto it = el.properties.begin(); it != el.properties.end(); ++it) {
            props[it.key()] = it.value();
        }
        obj["properties"] = props;

        elementsArray.append(obj);
    }

    root["elements"] = elementsArray;
    root["version"] = "1.0";

    // Serialize groups
    QJsonArray groupsArray;
    for (const auto &g : m_groups)
        groupsArray.append(g);
    root["groups"] = groupsArray;

    QJsonDocument doc(root);

    QFile file(path);
    if (!file.open(QIODevice::WriteOnly | QIODevice::Text)) {
        if (errorMsg)
            *errorMsg = QString("Cannot write to file: %1").arg(path);
        qWarning() << "Failed to save project:" << path;
        return false;
    }

    file.write(doc.toJson(QJsonDocument::Indented));
    file.close();

    m_projectFile = path;
    m_projectDir = QFileInfo(path).absolutePath();

    return true;
}

bool ProjectManager::loadProject(const QString &path, QString *errorMsg)
{
    QFile file(path);
    if (!file.open(QIODevice::ReadOnly | QIODevice::Text)) {
        if (errorMsg)
            *errorMsg = QString("Cannot open file: %1\n%2").arg(path, file.errorString());
        qWarning() << "Failed to load project:" << path;
        return false;
    }

    QByteArray data = file.readAll();
    file.close();

    if (data.isEmpty()) {
        if (errorMsg)
            *errorMsg = tr("File is empty: %1").arg(path);
        return false;
    }

    QJsonParseError parseError;
    QJsonDocument doc = QJsonDocument::fromJson(data, &parseError);

    if (parseError.error != QJsonParseError::NoError) {
        if (errorMsg)
            *errorMsg = tr("Invalid JSON at offset %1: %2\n\nFile: %3")
                .arg(parseError.offset)
                .arg(parseError.errorString())
                .arg(path);
        return false;
    }

    if (!doc.isObject()) {
        if (errorMsg)
            *errorMsg = tr("Project file root is not a JSON object: %1").arg(path);
        return false;
    }

    QJsonObject root = doc.object();

    if (!root.contains("elements")) {
        if (errorMsg)
            *errorMsg = tr("Project file is missing 'elements' array: %1").arg(path);
        return false;
    }

    QJsonArray elementsArray = root["elements"].toArray();
    if (elementsArray.isEmpty()) {
        if (errorMsg)
            *errorMsg = tr("Project file contains no elements (empty array).");
        // Not a fatal error — empty project is valid
    }

    m_elements.clear();
    int loadErrors = 0;

    for (const auto &val : elementsArray) {
        if (!val.isObject()) {
            ++loadErrors;
            continue;
        }
        QJsonObject obj = val.toObject();

        UiElementData el;

        // Validate required fields
        if (!obj.contains("name") || !obj["name"].isString()) {
            ++loadErrors;
            continue;
        }

        el.name = obj["name"].toString();
        el.type = obj["type"].toString("SIMPLEFRAME");
        el.texture = obj["texture"].toString();
        el.hasTexture = obj["hasTexture"].toBool(false);
        el.x = obj["x"].toDouble(0);
        el.y = obj["y"].toDouble(0);
        el.width = obj["width"].toDouble(100);
        el.height = obj["height"].toDouble(100);
        el.parent = obj["parent"].toString();
        el.group = obj["group"].toString();

        // Type-specific properties
        el.normalTexture = obj["normalTexture"].toString();
        el.highlightTexture = obj["highlightTexture"].toString();
        el.modelPath = obj["modelPath"].toString();
        el.textContent = obj["textContent"].toString();
        el.fontSize = obj["fontSize"].toDouble(14.0);
        el.textColor = obj["textColor"].toString("#FFFFFF");

        QJsonObject props = obj["properties"].toObject();
        for (auto it = props.begin(); it != props.end(); ++it) {
            el.properties[it.key()] = it.value().toString();
        }

        m_elements.append(el);
    }

    // Load groups
    m_groups.clear();
    QJsonArray groupsArray = root["groups"].toArray();
    for (const auto &g : groupsArray)
        m_groups.append(g.toString());

    if (loadErrors > 0 && errorMsg) {
        *errorMsg = tr("Loaded with %1 corrupted element(s) skipped.").arg(loadErrors);
    }

    m_projectFile = path;
    m_projectDir = QFileInfo(path).absolutePath();

    return true;
}

bool ProjectManager::exportIni(const QString &path, bool war3Mode, QString *errorMsg)
{
    return m_iniGenerator->generate(m_elements, m_projectDir, path, war3Mode, errorMsg);
}

bool ProjectManager::exportFdf(const QString &path, bool war3Mode, QString *errorMsg)
{
    return m_fdfGenerator->exportFdf(m_elements, path, war3Mode, errorMsg);
}

QList<UiElementData> ProjectManager::importFdf(const QString &path, QString *errorMsg)
{
    return m_fdfGenerator->importFdf(path, errorMsg);
}

ProjectManager::ImportResult ProjectManager::importImages(const QStringList &sourcePaths, const QString &targetDir)
{
    ImportResult result;

    QDir dir(targetDir);
    if (!dir.exists()) {
        if (!dir.mkpath(".")) {
            for (const auto &p : sourcePaths)
                result.failed.append(p);
            return result;
        }
    }

    // Create a textures subdirectory
    QString texturesDir = targetDir + "/textures";
    QDir texDir(texturesDir);
    if (!texDir.exists()) {
        texDir.mkpath(".");
    }

    for (const QString &sourcePath : sourcePaths) {
        QFileInfo fi(sourcePath);
        if (!fi.exists()) {
            result.failed.append(sourcePath);
            continue;
        }

        QString targetPath = texturesDir + "/" + fi.fileName();

        if (QFile::exists(targetPath)) {
            QFile::remove(targetPath);
        }

        if (QFile::copy(sourcePath, targetPath)) {
            result.imported.append(targetPath);
        } else {
            result.failed.append(sourcePath);
        }
    }

    m_projectDir = targetDir;
    return result;
}
