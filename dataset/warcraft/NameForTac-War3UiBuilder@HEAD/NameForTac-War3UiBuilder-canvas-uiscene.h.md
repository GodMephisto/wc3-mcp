---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "canvas/uiscene.h"
ref: HEAD
---

#ifndef UISCENE_H
#define UISCENE_H

#include <QGraphicsScene>
#include <QList>

class UiElement;
struct UiElementData;

class UiScene : public QGraphicsScene
{
    Q_OBJECT

public:
    explicit UiScene(QObject *parent = nullptr);
    ~UiScene() = default;

    void addElementFromTexture(const QString &texturePath, QPointF pos = {});
    void addElementFromData(const UiElementData &data);
    void deleteSelected();
    void clearAll();
    void selectElementByName(const QString &name);

    QList<UiElementData> allElementData() const;
    QList<UiElementData> selectedElementData() const;
    UiElementData elementDataByName(const QString &name) const;
    void updateElementProperty(const QString &name, const UiElementData &data);
    bool setElementParent(const QString &childName, const QString &newParent);
    bool wouldCreateCycle(const QString &childName, const QString &potentialParent) const;
    void loadFromData(const QList<UiElementData> &elements);
    void loadGroups(const QStringList &groups) { m_groups = groups; }
    QString uniqueName(const QString &base) const;

    void setSnapGridSize(int size) { m_snapGridSize = size; }
    int snapGridSize() const { return m_snapGridSize; }
    double snapToGrid(double val) const;
    QPointF snapToGuides(double x, double y, double w, double h) const;

    // Context menu operations
    void deleteElement(const QString &name);
    void renameElement(const QString &oldName, const QString &newName);
    void duplicateElement(const QString &name);
    void moveToRoot(const QString &name);
    void bringToFront(const QString &name);
    void sendToBack(const QString &name);

    // Alignment operations (operate on selected items)
    void alignLeft();
    void alignRight();
    void alignTop();
    void alignBottom();
    void alignCenterH();
    void alignCenterV();
    void distributeH();
    void distributeV();

    // Guide management
    void addGuide(double pos, bool horizontal);
    void removeGuide(double pos, bool horizontal);
    void clearGuides();
    void setTempGuide(double pos, bool horizontal);
    void clearTempGuide();

    // Group management
    QStringList groups() const;
    void createGroup(const QString &name);
    void deleteGroup(const QString &name);
    void renameGroup(const QString &oldName, const QString &newName);
    void addToGroup(const QString &elementName, const QString &groupName);
    void removeFromGroup(const QString &elementName);
    bool groupExists(const QString &name) const;

signals:
    void contextActionTriggered();
    void elementSelected(const QString &name);
    void elementDeselected();
    void elementDragFinished();
    void elementDataChanged();
    void elementCountChanged(int count);
    void textureLoadFailed(const QString &path, const QString &reason);

protected:
    void drawForeground(QPainter *painter, const QRectF &rect) override;
    void mousePressEvent(QGraphicsSceneMouseEvent *event) override;
    void keyPressEvent(QKeyEvent *event) override;
    void dragEnterEvent(QGraphicsSceneDragDropEvent *event) override;
    void dragMoveEvent(QGraphicsSceneDragDropEvent *event) override;
    void dropEvent(QGraphicsSceneDragDropEvent *event) override;

private:
    UiElement *uiElementFromItem(QGraphicsItem *item) const;
    QList<UiElement *> selectedUiElements() const;
    UiElement *findByName(const QString &name) const;
    void moveDescendants(const QString &parentName, double dx, double dy);
    void propagateStateToChildren(const QString &parentName);
    void onSelectionChanged();
    QString generateElementName() const;
    void connectElementSignals(UiElement *element);
    bool m_updatingChildPosition = false;
    int m_snapGridSize = 1;

    struct Guide {
        double pos;
        bool horizontal;
    };
    QList<Guide> m_guides;
    Guide m_tempGuide{0, false};
    bool m_hasTempGuide = false;

    QStringList m_groups;
    QList<UiElement *> m_elements;
};

#endif // UISCENE_H
