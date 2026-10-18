---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "managers/projectmanager.h"
ref: HEAD
---

#ifndef PROJECTMANAGER_H
#define PROJECTMANAGER_H

#include <QObject>
#include <QString>
#include <QStringList>
#include <QList>

#include "elements/uielementdata.h"

class IniGenerator;
class FdfGenerator;

class ProjectManager : public QObject
{
    Q_OBJECT

public:
    explicit ProjectManager(QObject *parent = nullptr);
    ~ProjectManager() = default;

    void newProject();
    bool saveProject(const QString &path, QString *errorMsg = nullptr);
    bool loadProject(const QString &path, QString *errorMsg = nullptr);
    bool exportIni(const QString &path, bool war3Mode = false, QString *errorMsg = nullptr);
    bool exportFdf(const QString &path, bool war3Mode = false, QString *errorMsg = nullptr);
    QList<UiElementData> importFdf(const QString &path, QString *errorMsg = nullptr);

    struct ImportResult {
        QStringList imported;
        QStringList failed;
    };
    ImportResult importImages(const QStringList &sourcePaths, const QString &targetDir);

    void setElements(const QList<UiElementData> &elements) { m_elements = elements; }
    const QList<UiElementData> &elements() const { return m_elements; }

    void setGroups(const QStringList &groups) { m_groups = groups; }
    QStringList groups() const { return m_groups; }

    QString projectDir() const { return m_projectDir; }

private:
    QList<UiElementData> m_elements;
    QStringList m_groups;
    QString m_projectDir;
    QString m_projectFile;
    IniGenerator *m_iniGenerator;
    FdfGenerator *m_fdfGenerator;
};

#endif // PROJECTMANAGER_H
