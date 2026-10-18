---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "managers/undomanager.cpp"
ref: HEAD
---

#include "undomanager.h"
#include "elements/uielementdata.h"

UndoManager::UndoManager(QObject *parent)
    : QObject(parent)
{
}

void UndoManager::pushState(const QList<UiElementData> &elements)
{
    // Discard any redo history beyond current index
    while (m_stack.size() > m_index + 1)
        m_stack.removeLast();

    m_stack.append(elements);
    m_index = m_stack.size() - 1;

    // Enforce max depth — discard oldest states
    while (m_stack.size() > MAX_STEPS) {
        m_stack.removeFirst();
        m_index--;
    }

    emit stateChanged();
}

bool UndoManager::canUndo() const
{
    // Need at least one state before the current index to undo TO
    return m_index > 0;
}

bool UndoManager::canRedo() const
{
    return m_index < m_stack.size() - 1;
}

QList<UiElementData> UndoManager::undo()
{
    if (!canUndo()) return {};
    m_index--;
    emit stateChanged();
    return m_stack[m_index];
}

QList<UiElementData> UndoManager::redo()
{
    if (!canRedo()) return {};
    m_index++;
    emit stateChanged();
    return m_stack[m_index];
}

void UndoManager::clear()
{
    m_stack.clear();
    m_index = -1;
    emit stateChanged();
}
