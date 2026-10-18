---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "panels/treepanel.cpp"
ref: HEAD
---

#include "treepanel.h"
#include "elements/uielementdata.h"

#include <QVBoxLayout>
#include <QHeaderView>
#include <QMenu>
#include <QInputDialog>
#include <QMessageBox>
#include <QTimer>
#include <QTreeWidgetItemIterator>
#include <QPainter>

static constexpr int COL_NAME    = 0;
static constexpr int COL_VISIBLE = 1;
static constexpr int COL_LOCK    = 2;
static constexpr int COL_COUNT   = 3;

static constexpr int TYPE_ELEMENT = 0;
static constexpr int TYPE_GROUP   = 1;

TreePanel::TreePanel(QWidget *parent)
    : QWidget(parent)
{
    auto *layout = new QVBoxLayout(this);
    layout->setContentsMargins(0, 0, 0, 0);

    m_searchEdit = new QLineEdit(this);
    m_searchEdit->setPlaceholderText(tr("Search elements..."));
    m_searchEdit->setClearButtonEnabled(true);
    layout->addWidget(m_searchEdit);

    m_tree = new QTreeWidget(this);
    m_tree->setColumnCount(COL_COUNT);
    m_tree->setHeaderLabels({tr("Elements"), QString(), QString()});
    m_tree->setAnimated(true);
    m_tree->setContextMenuPolicy(Qt::CustomContextMenu);
    m_tree->setDragDropMode(QAbstractItemView::InternalMove);

    m_tree->header()->setStretchLastSection(false);
    m_tree->header()->setSectionResizeMode(COL_NAME, QHeaderView::Stretch);
    m_tree->header()->setSectionResizeMode(COL_VISIBLE, QHeaderView::Fixed);
    m_tree->header()->setSectionResizeMode(COL_LOCK, QHeaderView::Fixed);
    m_tree->setColumnWidth(COL_VISIBLE, 24);
    m_tree->setColumnWidth(COL_LOCK, 24);

    layout->addWidget(m_tree);

    connect(m_searchEdit, &QLineEdit::textChanged, this, &TreePanel::onSearchChanged);
    connect(m_tree, &QTreeWidget::itemClicked, this, &TreePanel::onItemClicked);
    connect(m_tree, &QTreeWidget::itemDoubleClicked, this, &TreePanel::onItemDoubleClicked);
    connect(m_tree, &QTreeWidget::customContextMenuRequested, this, &TreePanel::onCustomContextMenu);
    auto *model = m_tree->model();
    connect(model, &QAbstractItemModel::rowsMoved, this, [this]() {
        QTimer::singleShot(0, this, &TreePanel::processPendingDrop);
    });
    connect(model, &QAbstractItemModel::rowsInserted, this, [this]() {
        QTimer::singleShot(0, this, &TreePanel::processPendingDrop);
    });
    connect(model, &QAbstractItemModel::rowsRemoved, this, [this]() {
        QTimer::singleShot(0, this, &TreePanel::processPendingDrop);
    });
}

static QIcon makeIcon(const QColor &fill, bool rounded = false)
{
    QPixmap pix(20, 20);
    pix.fill(Qt::transparent);
    QPainter p(&pix);
    p.setRenderHint(QPainter::Antialiasing);
    p.setPen(Qt::NoPen);
    p.setBrush(fill);
    if (rounded)
        p.drawRoundedRect(2, 2, 16, 16, 4, 4);
    else
        p.drawEllipse(2, 2, 16, 16);
    p.end();
    return QIcon(pix);
}

void TreePanel::setItemStatus(QTreeWidgetItem *item, bool visible, bool locked)
{
    static QIcon visIcon = makeIcon(QColor(0, 200, 80));
    static QIcon hidIcon = makeIcon(QColor(80, 80, 90));
    static QIcon lockIcon = makeIcon(QColor(255, 180, 60), true);
    static QIcon unlockIcon = makeIcon(QColor(80, 80, 90), true);
    item->setIcon(COL_VISIBLE, visible ? visIcon : hidIcon);
    item->setIcon(COL_LOCK, locked ? lockIcon : unlockIcon);
    item->setData(COL_VISIBLE, Qt::UserRole, visible);
    item->setData(COL_LOCK, Qt::UserRole, locked);
    QFont f = item->font(COL_NAME);
    f.setStrikeOut(!visible);
    item->setFont(COL_NAME, f);
}

void TreePanel::rebuildTree(const QList<UiElementData> &elements, const QStringList &groups)
{
    m_tree->clear();
    if (elements.isEmpty() && groups.isEmpty()) {
        m_lastTreeState.clear();
        return;
    }

    QMap<QString, UiElementData> dataMap;
    for (const auto &el : elements)
        dataMap[el.name] = el;

    QMap<QString, QTreeWidgetItem *> itemMap;

    // Create group header items (sorted)
    QMap<QString, QTreeWidgetItem *> groupItems;
    for (const auto &g : groups) {
        auto *gItem = new QTreeWidgetItem(m_tree);
        gItem->setText(COL_NAME, g);
        gItem->setData(COL_NAME, Qt::UserRole, g);
        gItem->setData(COL_NAME, Qt::UserRole + 1, TYPE_GROUP);
        QFont gf = gItem->font(COL_NAME);
        gf.setBold(true);
        gItem->setFont(COL_NAME, gf);
        gItem->setForeground(COL_NAME, QColor(122, 162, 247));
        gItem->setFlags(gItem->flags() & ~(Qt::ItemIsDragEnabled | Qt::ItemIsDropEnabled));
        gItem->setChildIndicatorPolicy(QTreeWidgetItem::ShowIndicator);
        gItem->setExpanded(true);
        groupItems[g] = gItem;
    }

    // Root elements (ungrouped, no parent)
    for (const auto &el : elements) {
        if (!el.group.isEmpty()) continue; // handled below
        if (el.parent.isEmpty() || !dataMap.contains(el.parent)) {
            auto *item = new QTreeWidgetItem(m_tree);
            item->setText(COL_NAME, el.name);
            item->setData(COL_NAME, Qt::UserRole, el.name);
            item->setData(COL_NAME, Qt::UserRole + 1, TYPE_ELEMENT);
            item->setToolTip(COL_NAME, QString("%1 (%2x%3)").arg(el.name).arg(el.width).arg(el.height));
            setItemStatus(item, el.visible, el.locked);
            itemMap[el.name] = item;
        }
    }

    // Children of ungrouped root elements
    for (const auto &el : elements) {
        if (!el.group.isEmpty()) continue;
        if (!el.parent.isEmpty() && itemMap.contains(el.parent)) {
            auto *child = new QTreeWidgetItem(itemMap[el.parent]);
            child->setText(COL_NAME, el.name);
            child->setData(COL_NAME, Qt::UserRole, el.name);
            child->setData(COL_NAME, Qt::UserRole + 1, TYPE_ELEMENT);
            child->setToolTip(COL_NAME, QString("%1 (%2x%3)").arg(el.name).arg(el.width).arg(el.height));
            setItemStatus(child, el.visible, el.locked);
            itemMap[el.name] = child;
        }
    }

    // Elements in groups — put all under their group header, flat
    for (const auto &g : groups) {
        if (!groupItems.contains(g)) continue;
        for (const auto &el : elements) {
            if (el.group == g) {
                auto *child = new QTreeWidgetItem(groupItems[g]);
                child->setText(COL_NAME, el.name);
                child->setData(COL_NAME, Qt::UserRole, el.name);
                child->setData(COL_NAME, Qt::UserRole + 1, TYPE_ELEMENT);
                child->setToolTip(COL_NAME, QString("%1 (%2x%3)").arg(el.name).arg(el.width).arg(el.height));
                setItemStatus(child, el.visible, el.locked);
                itemMap[el.name] = child;
            }
        }
    }

    m_tree->expandAll();
    saveTreeState();
}

void TreePanel::selectElement(const QString &name)
{
    auto *item = findItemByName(name);
    if (item) {
        m_tree->setCurrentItem(item);
    }
}

void TreePanel::refreshTree(const QList<UiElementData> &elements, const QStringList &groups)
{
    auto *current = m_tree->currentItem();
    QString selectedName;
    if (current)
        selectedName = current->data(COL_NAME, Qt::UserRole).toString();

    rebuildTree(elements, groups);

    if (!selectedName.isEmpty()) {
        auto *item = findItemByName(selectedName);
        if (item)
            m_tree->setCurrentItem(item);
    }
}

void TreePanel::clearPanel()
{
    m_tree->clear();
    m_lastTreeState.clear();
}

void TreePanel::processPendingDrop()
{
    if (m_checkPending) return;
    m_checkPending = true;

    QMap<QString, QString> currentState;
    scanTreeState(m_tree->invisibleRootItem(), QString(), currentState);

    for (auto it = currentState.begin(); it != currentState.end(); ++it) {
        QString prevParent = m_lastTreeState.value(it.key());
        if (prevParent != it.value()) {
            m_checkPending = false;
            emit parentChanged(it.key(), it.value());
            break;
        }
    }

    m_lastTreeState.clear();
    scanTreeState(m_tree->invisibleRootItem(), QString(), m_lastTreeState);
    m_checkPending = false;
}

void TreePanel::scanTreeState(QTreeWidgetItem *item, const QString &parentName,
                               QMap<QString, QString> &state) const
{
    for (int i = 0; i < item->childCount(); ++i) {
        QTreeWidgetItem *child = item->child(i);
        int type = child->data(COL_NAME, Qt::UserRole + 1).toInt();
        if (type != TYPE_ELEMENT) continue; // skip group headers
        QString name = child->data(COL_NAME, Qt::UserRole).toString();
        if (!name.isEmpty()) {
            state[name] = parentName;
        }
        scanTreeState(child, name, state);
    }
}

void TreePanel::saveTreeState()
{
    m_lastTreeState.clear();
    scanTreeState(m_tree->invisibleRootItem(), QString(), m_lastTreeState);
}

void TreePanel::onItemClicked(QTreeWidgetItem *item, int column)
{
    if (!item) return;

    int type = item->data(COL_NAME, Qt::UserRole + 1).toInt();
    if (type != TYPE_ELEMENT) return; // group header — ignore click

    QString name = item->data(COL_NAME, Qt::UserRole).toString();

    if (column == COL_VISIBLE) {
        bool vis = !item->data(COL_VISIBLE, Qt::UserRole).toBool();
        setItemStatus(item, vis, item->data(COL_LOCK, Qt::UserRole).toBool());
        emit visibilityToggled(name, vis);
    } else if (column == COL_LOCK) {
        bool locked = !item->data(COL_LOCK, Qt::UserRole).toBool();
        setItemStatus(item, item->data(COL_VISIBLE, Qt::UserRole).toBool(), locked);
        emit lockToggled(name, locked);
    } else {
        emit elementSelected(name);
    }
}

void TreePanel::onItemDoubleClicked(QTreeWidgetItem *item, int)
{
    if (!item) return;
    int type = item->data(COL_NAME, Qt::UserRole + 1).toInt();

    if (type == TYPE_GROUP) {
        // Rename group
        QString oldName = item->data(COL_NAME, Qt::UserRole).toString();
        bool ok;
        QString newName = QInputDialog::getText(this, tr("Rename Group"),
                                                  tr("Group name:"), QLineEdit::Normal,
                                                  oldName, &ok);
        if (ok && !newName.isEmpty() && newName != oldName) {
            emit groupRenamed(oldName, newName);
        }
        return;
    }

    QString oldName = item->data(COL_NAME, Qt::UserRole).toString();
    bool ok;
    QString newName = QInputDialog::getText(this, tr("Rename"),
                                              tr("Element name:"), QLineEdit::Normal,
                                              oldName, &ok);
    if (ok && !newName.isEmpty() && newName != oldName) {
        emit renameRequested(oldName, newName);
    }
}

void TreePanel::onCustomContextMenu(const QPoint &pos)
{
    auto *item = m_tree->itemAt(pos);
    if (!item) {
        // Click on empty area — show "Create Group" only
        QMenu menu;
        menu.addAction(tr("Create Group..."), this, [this]() {
            bool ok;
            QString name = QInputDialog::getText(this, tr("Create Group"),
                                                   tr("Group name:"), QLineEdit::Normal,
                                                   QString(), &ok);
            if (ok && !name.isEmpty())
                emit groupCreated(name);
        });
        menu.exec(m_tree->mapToGlobal(pos));
        return;
    }

    int type = item->data(COL_NAME, Qt::UserRole + 1).toInt();

    if (type == TYPE_GROUP) {
        QString groupName = item->data(COL_NAME, Qt::UserRole).toString();
        QMenu menu;
        menu.addAction(tr("Rename Group"), this, [this, groupName]() {
            bool ok;
            QString newName = QInputDialog::getText(this, tr("Rename Group"),
                                                      tr("Group name:"), QLineEdit::Normal,
                                                      groupName, &ok);
            if (ok && !newName.isEmpty() && newName != groupName)
                emit groupRenamed(groupName, newName);
        });
        menu.addAction(tr("Delete Group"), this, [this, groupName]() {
            emit groupDeleted(groupName);
        });
        menu.exec(m_tree->mapToGlobal(pos));
        return;
    }

    // Element context menu
    QString name = item->data(COL_NAME, Qt::UserRole).toString();

    QMenu menu;
    menu.addAction(tr("Rename"));
    menu.addAction(tr("Delete"));
    menu.addSeparator();

    // Groups submenu
    QMenu *groupMenu = menu.addMenu(tr("Add to Group"));
    // Dynamic groups — we'll populate from known groups. We need access to groups.
    // We'll populate from the tree: scan for group headers and also emit signals.
    // Actually, we emit a different signal approach: just emit a general signal.
    // The simpler approach: collect group names from tree items
    QStringList knownGroups;
    for (int i = 0; i < m_tree->topLevelItemCount(); ++i) {
        auto *top = m_tree->topLevelItem(i);
        if (top->data(COL_NAME, Qt::UserRole + 1).toInt() == TYPE_GROUP)
            knownGroups << top->data(COL_NAME, Qt::UserRole).toString();
    }

    if (knownGroups.isEmpty()) {
        groupMenu->addAction(tr("(No groups)"))->setEnabled(false);
    } else {
        for (const auto &g : knownGroups) {
            groupMenu->addAction(g, this, [this, name, g]() {
                emit groupAssignmentChanged(name, g);
            });
        }
    }

    QAction *removeAct = menu.addAction(tr("Remove from Group"));
    removeAct->setEnabled(false);
    // Check if already in a group
    if (item->parent() && item->parent()->data(COL_NAME, Qt::UserRole + 1).toInt() == TYPE_GROUP) {
        removeAct->setEnabled(true);
        connect(removeAct, &QAction::triggered, this, [this, name]() {
            emit groupAssignmentChanged(name, QString());
        });
    }

    QAction *selected = menu.exec(m_tree->mapToGlobal(pos));
    if (!selected) return;

    if (selected->text() == tr("Rename")) {
        onItemDoubleClicked(item, 0);
    } else if (selected->text() == tr("Delete")) {
        emit deleteRequested(name);
    }
}

QTreeWidgetItem *TreePanel::findItemByName(const QString &name) const
{
    QList<QTreeWidgetItem *> items = m_tree->findItems(name, Qt::MatchExactly | Qt::MatchRecursive, COL_NAME);
    for (auto *item : items) {
        if (item->data(COL_NAME, Qt::UserRole + 1).toInt() == TYPE_ELEMENT)
            return item;
    }
    return nullptr;
}

void TreePanel::onSearchChanged(const QString &text)
{
    if (text.isEmpty()) {
        QTreeWidgetItemIterator it(m_tree);
        while (*it) {
            (*it)->setHidden(false);
            ++it;
        }
        m_tree->expandAll();
        return;
    }

    QTreeWidgetItemIterator it(m_tree);
    while (*it) {
        int type = (*it)->data(COL_NAME, Qt::UserRole + 1).toInt();
        if (type == TYPE_GROUP) {
            // Show group if any child matches (handled below by child logic)
            (*it)->setHidden(true);
            (*it)->setExpanded(true);
        } else {
            QString elName = (*it)->data(COL_NAME, Qt::UserRole).toString();
            bool match = elName.contains(text, Qt::CaseInsensitive);
            (*it)->setHidden(!match);
            if (match) {
                QTreeWidgetItem *parent = (*it)->parent();
                while (parent) {
                    parent->setHidden(false);
                    parent->setExpanded(true);
                    parent = parent->parent();
                }
            }
        }
        ++it;
    }
}
