---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "canvas/uiscene.cpp"
ref: HEAD
---

#include "uiscene.h"
#include "elements/uielement.h"
#include "elements/uielementdata.h"

#include <QGraphicsSceneMouseEvent>
#include <QGraphicsSceneDragDropEvent>
#include <QKeyEvent>
#include <QMimeData>
#include <QUrl>
#include <QFileInfo>
#include <QGraphicsItem>
#include <QGraphicsView>
#include <QPainter>
#include <QDir>
#include <algorithm>
#include "debuglog.h"

static constexpr double SCENE_W = 3840.0;   // 2x screen width
static constexpr double SCENE_H = 2160.0;   // 2x screen height
static constexpr double SCREEN_W = 1920.0;
static constexpr double SCREEN_H = 1080.0;

UiScene::UiScene(QObject *parent)
    : QGraphicsScene(parent)
{
    // Scene centered around visible screen (0,0)-(1920,1080)
    setSceneRect(-SCREEN_W / 2, -SCREEN_H / 2, SCENE_W, SCENE_H);
    setBackgroundBrush(QColor(26, 27, 38));

    connect(this, &QGraphicsScene::selectionChanged,
            this, &UiScene::onSelectionChanged);
}

void UiScene::drawForeground(QPainter *painter, const QRectF &rect)
{
    if (views().isEmpty()) return;

    // Draw guides in scene coordinates
    QPen guidePen(QColor(255, 80, 80, 200), 1, Qt::DashLine);
    QPen tempGuidePen(QColor(255, 80, 80, 120), 1, Qt::DotLine);
    painter->setPen(guidePen);

    for (const auto &g : m_guides) {
        if (g.horizontal) {
            if (g.pos >= rect.top() && g.pos <= rect.bottom())
                painter->drawLine(QPointF(rect.left(), g.pos), QPointF(rect.right(), g.pos));
        } else {
            if (g.pos >= rect.left() && g.pos <= rect.right())
                painter->drawLine(QPointF(g.pos, rect.top()), QPointF(g.pos, rect.bottom()));
        }
    }

    if (m_hasTempGuide) {
        painter->setPen(tempGuidePen);
        if (m_tempGuide.horizontal) {
            if (m_tempGuide.pos >= rect.top() && m_tempGuide.pos <= rect.bottom())
                painter->drawLine(QPointF(rect.left(), m_tempGuide.pos), QPointF(rect.right(), m_tempGuide.pos));
        } else {
            if (m_tempGuide.pos >= rect.left() && m_tempGuide.pos <= rect.right())
                painter->drawLine(QPointF(m_tempGuide.pos, rect.top()), QPointF(m_tempGuide.pos, rect.bottom()));
        }
    }

    // Switch to viewport pixel coordinates so text stays fixed on screen
    painter->save();
    painter->setTransform(QTransform());

    QFont font;
    font.setPixelSize(12);
    painter->setFont(font);

    QStringList hints = {
        "Ctrl+滚轮 - 缩放画布",
        "Ctrl+0 - 适应全部",
        "拖拽图片到画布 - 导入素材",
        "Delete - 删除选中元素"
    };

    QFontMetrics fm(font);
    int textW = 0;
    for (const auto &h : hints)
        textW = qMax(textW, fm.horizontalAdvance(h));
    int textH = fm.height() * hints.size() + 14;
    int margin = 12;

    QGraphicsView *view = views().first();
    int x = view->viewport()->width() - textW - margin * 2 - 16;
    int y = margin;

    // Semi-transparent background
    painter->fillRect(x, y, textW + margin * 2, textH, QColor(0, 0, 0, 160));
    painter->setPen(QColor(220, 220, 220));

    int lineY = y + 7 + fm.ascent();
    for (const auto &h : hints) {
        painter->drawText(x + margin, lineY, h);
        lineY += fm.height();
    }

    painter->restore();
}

void UiScene::addElementFromTexture(const QString &texturePath, QPointF pos)
{
    UiElementData data;
    data.name = uniqueName(QFileInfo(texturePath).completeBaseName());
    data.type = "BACKDROP";
    data.texture = texturePath;
    data.x = pos.isNull() ? 0 : pos.x() - 100;
    data.y = pos.isNull() ? 0 : pos.y() - 50;
    data.width = 200;
    data.height = 100;
    data.parent = "";
    data.hasTexture = true;

    auto *element = new UiElement(data);
    m_elements.append(element);
    addItem(element);

    // Load texture after item is in scene so geometry updates work
    if (!element->loadTexture(texturePath)) {
        QString reason = tr("无法加载图片：%1").arg(texturePath);
        reason += tr("\n\n诊断日志：%1/War3UiBuilder_debug.log").arg(QDir::tempPath());
        emit textureLoadFailed(texturePath, reason);
    }

    connectElementSignals(element);

    emit elementDataChanged();
    emit elementCountChanged(m_elements.size());
}

void UiScene::addElementFromData(const UiElementData &data)
{
    auto *element = new UiElement(data);
    m_elements.append(element);
    addItem(element);

    if (element->elementData().hasTexture) {
        element->loadTexture(element->elementData().texture);
    }

    connectElementSignals(element);

    emit elementDataChanged();
    emit elementCountChanged(m_elements.size());
}

void UiScene::deleteSelected()
{
    QList<QGraphicsItem *> items = selectedItems();
    for (auto *item : items) {
        auto *uiEl = uiElementFromItem(item);
        if (uiEl) {
            m_elements.removeOne(uiEl);
            removeItem(uiEl);
            delete uiEl;
        }
    }
    emit elementDataChanged();
    emit elementCountChanged(m_elements.size());
}

void UiScene::clearAll()
{
    for (auto *el : m_elements) {
        removeItem(el);
        delete el;
    }
    m_elements.clear();
    m_groups.clear();
    emit elementDataChanged();
    emit elementCountChanged(0);
}

void UiScene::selectElementByName(const QString &name)
{
    clearSelection();
    for (auto *el : m_elements) {
        if (el->elementData().name == name) {
            el->setSelected(true);
            break;
        }
    }
}

QList<UiElementData> UiScene::allElementData() const
{
    QList<UiElementData> result;
    for (const auto *el : m_elements) {
        result.append(el->elementData());
    }
    return result;
}

QList<UiElementData> UiScene::selectedElementData() const
{
    QList<UiElementData> result;
    for (auto *item : selectedItems()) {
        auto *el = uiElementFromItem(item);
        if (el) result.append(el->elementData());
    }
    return result;
}

double UiScene::snapToGrid(double val) const
{
    if (m_snapGridSize <= 1) return val;
    return qRound(val / m_snapGridSize) * m_snapGridSize;
}

UiElementData UiScene::elementDataByName(const QString &name) const
{
    for (const auto *el : m_elements) {
        if (el->elementData().name == name) {
            return el->elementData();
        }
    }
    return UiElementData();
}

void UiScene::updateElementProperty(const QString &name, const UiElementData &data)
{
    // Check for cycle if parent changed
    if (data.parent != name && !data.parent.isEmpty() && wouldCreateCycle(name, data.parent))
        return;

    for (auto *el : m_elements) {
        if (el->elementData().name == name) {
            el->updateData(data);
            // Update children's parent references when element is renamed
            if (name != data.name) {
                for (auto *child : m_elements) {
                    auto childData = child->elementData();
                    if (childData.parent == name) {
                        childData.parent = data.name;
                        child->updateData(childData);
                    }
                }
            }
            // Propagate visible/locked state to all descendants
            propagateStateToChildren(name);
            emit elementDataChanged();
            return;
        }
    }
}

bool UiScene::setElementParent(const QString &childName, const QString &newParent)
{
    if (childName == newParent) return false;
    if (wouldCreateCycle(childName, newParent)) return false;

    for (auto *el : m_elements) {
        if (el->elementData().name == childName) {
            auto data = el->elementData();
            data.parent = newParent;
            el->updateData(data);
            emit elementDataChanged();
            return true;
        }
    }
    return false;
}

bool UiScene::wouldCreateCycle(const QString &childName, const QString &potentialParent) const
{
    // Check if setting potentialParent as childName's parent would create a loop
    QString current = potentialParent;
    while (!current.isEmpty()) {
        if (current == childName) return true;
        // Find current's parent
        bool found = false;
        for (const auto *el : m_elements) {
            if (el->elementData().name == current) {
                current = el->elementData().parent;
                found = true;
                break;
            }
        }
        if (!found) break;
    }
    return false;
}

void UiScene::loadFromData(const QList<UiElementData> &elements)
{
    clearAll();
    for (const auto &data : elements) {
        addElementFromData(data);
    }
}

void UiScene::deleteElement(const QString &name)
{
    for (auto *el : m_elements) {
        if (el->elementData().name == name) {
            m_elements.removeOne(el);
            removeItem(el);
            delete el;
            break;
        }
    }
    emit elementDataChanged();
    emit elementCountChanged(m_elements.size());
}

void UiScene::renameElement(const QString &oldName, const QString &newName)
{
    if (oldName == newName || newName.isEmpty()) return;
    for (auto *el : m_elements) {
        if (el->elementData().name == oldName) {
            auto data = el->elementData();
            data.name = newName;
            el->updateData(data);
            break;
        }
    }
    // Update parent references for children
    for (auto *el : m_elements) {
        auto data = el->elementData();
        if (data.parent == oldName) {
            data.parent = newName;
            el->updateData(data);
        }
    }
    emit elementDataChanged();
}

void UiScene::duplicateElement(const QString &name)
{
    for (const auto *el : m_elements) {
        if (el->elementData().name == name) {
            auto data = el->elementData();
            data.name = uniqueName(name + "_copy");
            data.x += 20.0;
            data.y += 20.0;
            addElementFromData(data);
            break;
        }
    }
}

void UiScene::moveToRoot(const QString &name)
{
    for (auto *el : m_elements) {
        if (el->elementData().name == name) {
            auto data = el->elementData();
            data.parent.clear();
            el->updateData(data);
            break;
        }
    }
    emit elementDataChanged();
}

void UiScene::bringToFront(const QString &name)
{
    double maxZ = 0;
    for (const auto *el : m_elements) {
        if (el->elementData().name != name)
            maxZ = qMax(maxZ, el->zValue());
    }
    for (auto *el : m_elements) {
        if (el->elementData().name == name) {
            el->setZValue(maxZ + 100);
            break;
        }
    }
    emit elementDataChanged();
}

void UiScene::sendToBack(const QString &name)
{
    double minZ = 0;
    for (const auto *el : m_elements) {
        if (el->elementData().name != name)
            minZ = qMin(minZ, el->zValue());
    }
    for (auto *el : m_elements) {
        if (el->elementData().name == name) {
            el->setZValue(minZ - 100);
            break;
        }
    }
    emit elementDataChanged();
}

QList<UiElement *> UiScene::selectedUiElements() const
{
    QList<UiElement *> result;
    for (auto *item : selectedItems()) {
        auto *el = uiElementFromItem(item);
        if (el) result.append(el);
    }
    return result;
}

void UiScene::alignLeft()
{
    auto sel = selectedUiElements();
    if (sel.size() < 2) return;
    double ref = sel.first()->elementData().x;
    for (auto *el : sel) {
        auto d = el->elementData();
        d.x = ref;
        el->updateData(d);
    }
    emit elementDataChanged();
}

void UiScene::alignRight()
{
    auto sel = selectedUiElements();
    if (sel.size() < 2) return;
    double ref = sel.first()->elementData().x + sel.first()->elementData().width;
    for (auto *el : sel) {
        auto d = el->elementData();
        d.x = ref - d.width;
        el->updateData(d);
    }
    emit elementDataChanged();
}

void UiScene::alignTop()
{
    auto sel = selectedUiElements();
    if (sel.size() < 2) return;
    double ref = sel.first()->elementData().y;
    for (auto *el : sel) {
        auto d = el->elementData();
        d.y = ref;
        el->updateData(d);
    }
    emit elementDataChanged();
}

void UiScene::alignBottom()
{
    auto sel = selectedUiElements();
    if (sel.size() < 2) return;
    double ref = sel.first()->elementData().y + sel.first()->elementData().height;
    for (auto *el : sel) {
        auto d = el->elementData();
        d.y = ref - d.height;
        el->updateData(d);
    }
    emit elementDataChanged();
}

void UiScene::alignCenterH()
{
    auto sel = selectedUiElements();
    if (sel.size() < 2) return;
    double ref = sel.first()->elementData().x + sel.first()->elementData().width / 2.0;
    for (auto *el : sel) {
        auto d = el->elementData();
        d.x = ref - d.width / 2.0;
        el->updateData(d);
    }
    emit elementDataChanged();
}

void UiScene::alignCenterV()
{
    auto sel = selectedUiElements();
    if (sel.size() < 2) return;
    double ref = sel.first()->elementData().y + sel.first()->elementData().height / 2.0;
    for (auto *el : sel) {
        auto d = el->elementData();
        d.y = ref - d.height / 2.0;
        el->updateData(d);
    }
    emit elementDataChanged();
}

void UiScene::distributeH()
{
    auto sel = selectedUiElements();
    if (sel.size() < 3) return;
    std::sort(sel.begin(), sel.end(), [](UiElement *a, UiElement *b) {
        return a->elementData().x < b->elementData().x;
    });
    double first = sel.first()->elementData().x;
    double last = sel.last()->elementData().x + sel.last()->elementData().width;
    double totalWidth = last - first;
    double usedWidth = 0;
    for (auto *el : sel)
        usedWidth += el->elementData().width;
    double gap = (totalWidth - usedWidth) / (sel.size() - 1);
    double pos = first;
    for (auto *el : sel) {
        auto d = el->elementData();
        d.x = pos;
        el->updateData(d);
        pos += d.width + gap;
    }
    emit elementDataChanged();
}

void UiScene::distributeV()
{
    auto sel = selectedUiElements();
    if (sel.size() < 3) return;
    std::sort(sel.begin(), sel.end(), [](UiElement *a, UiElement *b) {
        return a->elementData().y < b->elementData().y;
    });
    double first = sel.first()->elementData().y;
    double last = sel.last()->elementData().y + sel.last()->elementData().height;
    double totalHeight = last - first;
    double usedHeight = 0;
    for (auto *el : sel)
        usedHeight += el->elementData().height;
    double gap = (totalHeight - usedHeight) / (sel.size() - 1);
    double pos = first;
    for (auto *el : sel) {
        auto d = el->elementData();
        d.y = pos;
        el->updateData(d);
        pos += d.height + gap;
    }
    emit elementDataChanged();
}

void UiScene::mousePressEvent(QGraphicsSceneMouseEvent *event)
{
    QGraphicsScene::mousePressEvent(event);
}

void UiScene::keyPressEvent(QKeyEvent *event)
{
    if (event->key() == Qt::Key_Delete || event->key() == Qt::Key_Backspace) {
        deleteSelected();
    } else {
        QGraphicsScene::keyPressEvent(event);
    }
}

void UiScene::dragEnterEvent(QGraphicsSceneDragDropEvent *event)
{
    if (event->mimeData()->hasUrls()) {
        event->acceptProposedAction();
    } else {
        QGraphicsScene::dragEnterEvent(event);
    }
}

void UiScene::dragMoveEvent(QGraphicsSceneDragDropEvent *event)
{
    if (event->mimeData()->hasUrls()) {
        event->acceptProposedAction();
    } else {
        QGraphicsScene::dragMoveEvent(event);
    }
}

void UiScene::dropEvent(QGraphicsSceneDragDropEvent *event)
{
    debugLog("[dropEvent] triggered");

    if (event->mimeData()->hasUrls()) {
        const auto urls = event->mimeData()->urls();
        debugLog(QString("[dropEvent] hasUrls: %1 urls").arg(urls.size()));

        for (const QUrl &url : urls) {
            QString path = url.toLocalFile();
            QString ext = QFileInfo(path).suffix().toLower();
            debugLog(QString("[dropEvent] url=%1 path=%2 ext=%3").arg(url.toString(), path, ext));

            if (ext == "png" || ext == "jpg" || ext == "jpeg" || ext == "bmp" || ext == "tga" || ext == "dds") {
                addElementFromTexture(path, event->scenePos());
            } else {
                debugLog(QString("[dropEvent] unsupported extension: %1").arg(ext));
            }
        }
        event->acceptProposedAction();
    } else {
        debugLog("[dropEvent] no URLs in mime data");
        QGraphicsScene::dropEvent(event);
    }
}

UiElement *UiScene::uiElementFromItem(QGraphicsItem *item) const
{
    while (item && item->type() != UiElement::Type) {
        item = item->parentItem();
    }
    return static_cast<UiElement *>(item);
}

void UiScene::onSelectionChanged()
{
    QList<QGraphicsItem *> items = selectedItems();
    if (items.isEmpty()) {
        emit elementDeselected();
        return;
    }

    auto *el = uiElementFromItem(items.first());
    if (el) {
        emit elementSelected(el->elementData().name);
    }
}

void UiScene::connectElementSignals(UiElement *element)
{
    connect(element, &UiElement::elementMoved, this, [this](const QString &name, double x, double y, double dx, double dy) {
        emit elementDataChanged();
        if (!m_updatingChildPosition && (dx != 0 || dy != 0)) {
            m_updatingChildPosition = true;
            moveDescendants(name, dx, dy);
            m_updatingChildPosition = false;
        }
    });
    connect(element, &UiElement::elementResized, this, [this](const QString &, double, double) {
        emit elementDataChanged();
    });
    connect(element, &UiElement::dragFinished, this, [this](const QString &) {
        emit elementDragFinished();
    });
}

UiElement *UiScene::findByName(const QString &name) const
{
    for (auto *el : m_elements) {
        if (el->elementData().name == name)
            return el;
    }
    return nullptr;
}

void UiScene::moveDescendants(const QString &parentName, double dx, double dy)
{
    for (auto *el : m_elements) {
        if (el->elementData().parent == parentName) {
            el->setPos(el->pos() + QPointF(dx, dy));
            moveDescendants(el->elementData().name, dx, dy);
        }
    }
}

void UiScene::propagateStateToChildren(const QString &parentName)
{
    auto parent = findByName(parentName);
    if (!parent) return;
    bool pv = parent->elementData().visible;
    bool pl = parent->elementData().locked;

    for (auto *el : m_elements) {
        if (el->elementData().parent == parentName) {
            auto cd = el->elementData();
            cd.visible = pv;
            cd.locked = pl;
            el->updateData(cd);
            propagateStateToChildren(cd.name);
        }
    }
}

QString UiScene::generateElementName() const
{
    int counter = 1;
    QString base = "Element";
    QString name;

    do {
        name = QString("%1_%2").arg(base).arg(counter++);
    } while (std::any_of(m_elements.begin(), m_elements.end(),
                         [&](UiElement *el) { return el->elementData().name == name; }));

    return name;
}

QString UiScene::uniqueName(const QString &base) const
{
    if (std::none_of(m_elements.begin(), m_elements.end(),
                     [&](UiElement *el) { return el->elementData().name == base; })) {
        return base;
    }
    int counter = 1;
    QString name;
    do {
        name = QString("%1_%2").arg(base).arg(counter++);
    } while (std::any_of(m_elements.begin(), m_elements.end(),
                         [&](UiElement *el) { return el->elementData().name == name; }));
    return name;
}

static constexpr double GUIDE_SNAP_THRESHOLD = 6.0;

QPointF UiScene::snapToGuides(double x, double y, double w, double h) const
{
    double sx = x;
    double sy = y;

    for (const auto &g : m_guides) {
        if (g.horizontal) {
            // Snap top edge to guide
            if (qAbs(y - g.pos) < GUIDE_SNAP_THRESHOLD)
                sy = g.pos;
            // Snap bottom edge to guide
            else if (qAbs(y + h - g.pos) < GUIDE_SNAP_THRESHOLD)
                sy = g.pos - h;
        } else {
            // Snap left edge to guide
            if (qAbs(x - g.pos) < GUIDE_SNAP_THRESHOLD)
                sx = g.pos;
            // Snap right edge to guide
            else if (qAbs(x + w - g.pos) < GUIDE_SNAP_THRESHOLD)
                sx = g.pos - w;
        }
    }

    return QPointF(sx, sy);
}

// ── Guide management ──

void UiScene::addGuide(double pos, bool horizontal)
{
    // Don't add duplicate guides at the same position
    for (const auto &g : m_guides) {
        if (g.horizontal == horizontal && qAbs(g.pos - pos) < 2.0)
            return;
    }
    m_guides.append({pos, horizontal});
    update();
}

void UiScene::removeGuide(double pos, bool horizontal)
{
    for (int i = 0; i < m_guides.size(); ++i) {
        if (m_guides[i].horizontal == horizontal && qAbs(m_guides[i].pos - pos) < 2.0) {
            m_guides.removeAt(i);
            break;
        }
    }
    update();
}

void UiScene::clearGuides()
{
    m_guides.clear();
    update();
}

void UiScene::setTempGuide(double pos, bool horizontal)
{
    m_tempGuide = {pos, horizontal};
    m_hasTempGuide = true;
    update();
}

void UiScene::clearTempGuide()
{
    m_hasTempGuide = false;
    update();
}

// ── Group management ──

QStringList UiScene::groups() const
{
    // Collect groups from storage + elements
    QSet<QString> allGroups(m_groups.begin(), m_groups.end());
    for (const auto *el : m_elements) {
        if (!el->elementData().group.isEmpty())
            allGroups.insert(el->elementData().group);
    }
    QStringList sorted = allGroups.values();
    sorted.sort();
    return sorted;
}

void UiScene::createGroup(const QString &name)
{
    if (name.isEmpty() || m_groups.contains(name)) return;
    m_groups.append(name);
    emit elementDataChanged();
}

void UiScene::deleteGroup(const QString &name)
{
    m_groups.removeAll(name);
    // Remove group assignment from all elements
    for (auto *el : m_elements) {
        if (el->elementData().group == name) {
            auto data = el->elementData();
            data.group.clear();
            el->updateData(data);
        }
    }
    emit elementDataChanged();
}

void UiScene::renameGroup(const QString &oldName, const QString &newName)
{
    if (oldName == newName || newName.isEmpty()) return;
    if (m_groups.contains(oldName)) {
        m_groups.removeAll(oldName);
        m_groups.append(newName);
    }
    // Update elements in this group
    for (auto *el : m_elements) {
        if (el->elementData().group == oldName) {
            auto data = el->elementData();
            data.group = newName;
            el->updateData(data);
        }
    }
    emit elementDataChanged();
}

void UiScene::addToGroup(const QString &elementName, const QString &groupName)
{
    if (elementName.isEmpty() || groupName.isEmpty()) return;
    for (auto *el : m_elements) {
        if (el->elementData().name == elementName) {
            auto data = el->elementData();
            data.group = groupName;
            el->updateData(data);
            break;
        }
    }
    if (!m_groups.contains(groupName))
        m_groups.append(groupName);
    emit elementDataChanged();
}

void UiScene::removeFromGroup(const QString &elementName)
{
    for (auto *el : m_elements) {
        if (el->elementData().name == elementName) {
            auto data = el->elementData();
            data.group.clear();
            el->updateData(data);
            break;
        }
    }
    emit elementDataChanged();
}

bool UiScene::groupExists(const QString &name) const
{
    if (m_groups.contains(name)) return true;
    for (const auto *el : m_elements) {
        if (el->elementData().group == name)
            return true;
    }
    return false;
}
