---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "managers/undomanager.h"
ref: HEAD
---

#ifndef UNDOMANAGER_H
#define UNDOMANAGER_H

#include <QObject>
#include <QList>

#include "elements/uielementdata.h"

class UndoManager : public QObject
{
    Q_OBJECT

public:
    explicit UndoManager(QObject *parent = nullptr);
    ~UndoManager() = default;

    void pushState(const QList<UiElementData> &elements);
    bool canUndo() const;
    bool canRedo() const;
    QList<UiElementData> undo();
    QList<UiElementData> redo();
    void clear();

signals:
    void stateChanged();

private:
    QList<QList<UiElementData>> m_stack;
    int m_index = -1;     // points to current state
    static constexpr int MAX_STEPS = 50;
};

#endif // UNDOMANAGER_H
