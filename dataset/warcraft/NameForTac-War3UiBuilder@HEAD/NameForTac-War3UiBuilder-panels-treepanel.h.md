---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "panels/treepanel.h"
ref: HEAD
---

#ifndef TREEPANEL_H
#define TREEPANEL_H

#include <QWidget>
#include <QTreeWidget>
#include <QList>
#include <QLineEdit>

struct UiElementData;

class TreePanel : public QWidget
{
    Q_OBJECT

public:
    explicit TreePanel(QWidget *parent = nullptr);
    ~TreePanel() = default;

    void rebuildTree(const QList<UiElementData> &elements, const QStringList &groups = {});
    void selectElement(const QString &name);
    void refreshTree(const QList<UiElementData> &elements, const QStringList &groups = {});
    void clearPanel();

signals:
    void elementSelected(const QString &name);
    void visibilityToggled(const QString &name, bool visible);
    void lockToggled(const QString &name, bool locked);
    void parentChanged(const QString &elementName, const QString &newParent);
    void renameRequested(const QString &oldName, const QString &newName);
    void deleteRequested(const QString &name);

    // Group operations
    void groupCreated(const QString &name);
    void groupDeleted(const QString &name);
    void groupRenamed(const QString &oldName, const QString &newName);
    void groupAssignmentChanged(const QString &elementName, const QString &groupName);

private slots:
    void onItemClicked(QTreeWidgetItem *item, int column);
    void onItemDoubleClicked(QTreeWidgetItem *item, int column);
    void onCustomContextMenu(const QPoint &pos);
    void processPendingDrop();
    void onSearchChanged(const QString &text);

private:
    QTreeWidgetItem *findItemByName(const QString &name) const;
    void buildTreeRecursive(QTreeWidgetItem *parent, const QString &parentName,
                            const QMap<QString, UiElementData> &dataMap,
                            QMap<QString, QTreeWidgetItem *> &itemMap);
    void setItemStatus(QTreeWidgetItem *item, bool visible, bool locked);
    void scanTreeState(QTreeWidgetItem *item, const QString &parentName,
                       QMap<QString, QString> &state) const;
    void saveTreeState();
    bool m_checkPending = false;
    QMap<QString, QString> m_lastTreeState;
    QTreeWidget *m_tree;
    QLineEdit *m_searchEdit;
};

#endif // TREEPANEL_H
