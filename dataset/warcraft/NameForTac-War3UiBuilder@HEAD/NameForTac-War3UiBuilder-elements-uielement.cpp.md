---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "elements/uielement.cpp"
ref: HEAD
---

#include "uielement.h"
#include "uielementdata.h"
#include "canvas/uiscene.h"

#include <QPainter>
#include <QGraphicsScene>
#include <QGraphicsView>
#include <QGraphicsSceneMouseEvent>
#include <QGraphicsSceneContextMenuEvent>
#include <QMenu>
#include <QAction>
#include <QInputDialog>
#include <QStyleOptionGraphicsItem>
#include <QCursor>
#include <QFileInfo>
#include <QFile>
#include <QImageReader>
#include <QDebug>
#include "debuglog.h"

#include "stb_image.h"

UiElement::UiElement(const UiElementData &data, QGraphicsItem *parent)
    : QGraphicsObject(parent), m_data(data)
{
    setFlags(ItemIsSelectable | ItemIsMovable | ItemSendsGeometryChanges);
    setAcceptHoverEvents(true);
    setCursor(Qt::ArrowCursor);
    setPos(data.x, data.y);

    // Texture is loaded by the scene after addItem()
    // so that prepareGeometryChange() / update() work correctly
}

QRectF UiElement::boundingRect() const
{
    double pen = 2.0;
    // Worst-case handle size: at min zoom 0.1x, handle = 14 / 0.1 = 140
    double extra = (HANDLE_SIZE / 0.1) + pen;
    return QRectF(-extra, -extra, m_data.width + 2 * extra, m_data.height + 2 * extra);
}

void UiElement::paint(QPainter *painter, const QStyleOptionGraphicsItem *option, QWidget *)
{
    if (!m_data.visible) return;

    painter->setRenderHint(QPainter::Antialiasing);

    QRectF rect(0, 0, m_data.width, m_data.height);

    // Draw background/texture
    if (!m_pixmap.isNull()) {
        painter->drawPixmap(rect, m_pixmap, m_pixmap.rect());
    } else {
        painter->setBrush(QColor(70, 70, 75));
        painter->setPen(QPen(QColor(120, 120, 130), 1));
        painter->drawRect(rect);

        // Draw placeholder text
        painter->setPen(QColor(180, 180, 190));
        painter->drawText(rect, Qt::AlignCenter, m_data.name);
    }

    // Render text for TEXT type elements
    if (m_data.type == "TEXT" && !m_data.textContent.isEmpty()) {
        painter->setPen(QColor(m_data.textColor));
        QFont font;
        font.setPixelSize(qMax(8, static_cast<int>(m_data.fontSize)));
        painter->setFont(font);
        painter->drawText(rect, Qt::AlignCenter, m_data.textContent);
    }

    // Draw border when selected
    if (isSelected()) {
        painter->setPen(QPen(QColor(0, 160, 255), 2));
        painter->setBrush(Qt::NoBrush);
        painter->drawRect(rect);

        // Draw resize handles
        drawHandles(painter);
    }
}

QPainterPath UiElement::shape() const
{
    if (!m_data.visible) return QPainterPath();

    QPainterPath path;
    path.addRect(0, 0, m_data.width, m_data.height);

    // Include all 8 resize handle areas so they are clickable
    double s = handleSize();
    double w = m_data.width;
    double h = m_data.height;
    // corners
    path.addRect(QRectF(-s / 2, -s / 2, s, s));
    path.addRect(QRectF(w - s / 2, -s / 2, s, s));
    path.addRect(QRectF(-s / 2, h - s / 2, s, s));
    path.addRect(QRectF(w - s / 2, h - s / 2, s, s));
    // edges
    path.addRect(QRectF(w / 2 - s / 2, -s / 2, s, s));
    path.addRect(QRectF(w / 2 - s / 2, h - s / 2, s, s));
    path.addRect(QRectF(-s / 2, h / 2 - s / 2, s, s));
    path.addRect(QRectF(w - s / 2, h / 2 - s / 2, s, s));

    return path;
}

bool UiElement::loadTexture(const QString &path)
{
    QFileInfo fi(path);
    debugLog(QString("[loadTexture] path=%1 exists=%2 size=%3")
        .arg(path).arg(fi.exists()).arg(fi.size()));

    if (!fi.exists()) {
        qWarning().noquote() << "[UiElement] File not found:" << path;
        debugLog("[loadTexture] FAIL: file not found");
        return false;
    }

    QByteArray path8 = path.toUtf8();

    // Approach 1: QImageReader (PNG/JPEG/BMP/GIF — formats built into QtGui)
    QImageReader reader(path);
    {
        QImage image = reader.read();
        if (!image.isNull()) {
            debugLog(QString("[loadTexture] QImageReader OK: %1x%2").arg(image.width()).arg(image.height()));
            applyTexture(image, path);
            return true;
        }
        debugLog(QString("[loadTexture] QImageReader FAIL: %1").arg(reader.errorString()));
    }

    // Approach 2: stb_image (TGA, DDS, HDR, PSD, and many more)
    // Use QFile to read file into memory first (handles Windows Unicode paths)
    QFile file(path);
    if (file.open(QIODevice::ReadOnly)) {
        QByteArray fileData = file.readAll();
        file.close();

        int w = 0, h = 0, channels = 0;
        unsigned char *data = stbi_load_from_memory(
            (const unsigned char *)fileData.constData(),
            fileData.size(), &w, &h, &channels, 4);
        if (data) {
            debugLog(QString("[loadTexture] stb_image OK: %1x%2 ch=%3").arg(w).arg(h).arg(channels));
            QImage image(data, w, h, QImage::Format_RGBA8888);
            // QImage does NOT own the data, so we must copy it
            QImage copy = image.copy();
            stbi_image_free(data);
            applyTexture(copy, path);
            return true;
        }
        debugLog(QString("[loadTexture] stb_image FAIL: %1").arg(stbi_failure_reason()));
    } else {
        debugLog(QString("[loadTexture] QFile open failed: %1").arg(file.errorString()));
    }

    // Both failed - log diagnostics
    qWarning().noquote() << "[UiElement] Failed to load image:" << path;
    qWarning().noquote() << "  QImageReader error:" << reader.errorString();
    qWarning().noquote() << "  stb_image error:" << stbi_failure_reason();
    qWarning().noquote() << "  File size:" << fi.size() << "bytes";

    debugLog("[loadTexture] ALL LOADERS FAILED");
    return false;
}

void UiElement::applyTexture(const QImage &image, const QString &path)
{
    m_pixmap = QPixmap::fromImage(image);
    m_data.hasTexture = true;
    m_data.texture = path;
    m_data.width = image.width();
    m_data.height = image.height();
    prepareGeometryChange();
    update();
    emit elementResized(m_data.name, m_data.width, m_data.height);
}

void UiElement::updateData(const UiElementData &data)
{
    bool needsReload = data.hasTexture && data.texture != m_data.texture;
    prepareGeometryChange();
    m_data = data;
    if (needsReload) {
        loadTexture(m_data.texture);
    }
    setPos(data.x, data.y);
    update();
}

QVariant UiElement::itemChange(GraphicsItemChange change, const QVariant &value)
{
    if (change == ItemPositionChange && m_data.locked) {
        return pos(); // reject position change when locked
    }
    if (change == ItemPositionHasChanged) {
        QPointF newPos = pos();
        double dx = newPos.x() - m_data.x;
        double dy = newPos.y() - m_data.y;
        m_data.x = newPos.x();
        m_data.y = newPos.y();
        // Don't propagate to children during resize (setPos called from resize handler)
        if (!m_isResizing) {
            emit elementMoved(m_data.name, m_data.x, m_data.y, dx, dy);
        }
    }
    return QGraphicsObject::itemChange(change, value);
}

void UiElement::mousePressEvent(QGraphicsSceneMouseEvent *event)
{
    if (m_data.locked) {
        QGraphicsObject::mousePressEvent(event);
        return;
    }

    if (event->button() == Qt::LeftButton) {
        m_resizeEdge = hitTest(event->pos());
        if (m_resizeEdge != None) {
            m_dragging = true;
            m_isResizing = true;
            m_dragStart = event->scenePos();
            m_origSize = QSizeF(m_data.width, m_data.height);
            m_origPos = pos();
        } else {
            QGraphicsObject::mousePressEvent(event);
            if (isSelected()) {
                m_dragging = true;
                m_dragStart = event->scenePos();
                m_origPos = pos();
            }
        }
    }
}

void UiElement::mouseReleaseEvent(QGraphicsSceneMouseEvent *event)
{
    if (event->button() == Qt::LeftButton) {
        m_dragging = false;
        m_resizeEdge = None;
        m_isResizing = false;
    }
    QGraphicsObject::mouseReleaseEvent(event);

    // Snap to grid and guides after drag
    auto *uiScene = qobject_cast<UiScene *>(scene());
    if (uiScene) {
        double nx = pos().x();
        double ny = pos().y();

        // Grid snap
        if (uiScene->snapGridSize() > 1) {
            nx = uiScene->snapToGrid(nx);
            ny = uiScene->snapToGrid(ny);
        }

        // Guide snap
        QPointF guideSnap = uiScene->snapToGuides(nx, ny, m_data.width, m_data.height);
        nx = guideSnap.x();
        ny = guideSnap.y();

        if (nx != pos().x() || ny != pos().y()) {
            setPos(nx, ny);
        }
    }

    // Push undo state after all position changes (including grid snap)
    if (pos() != m_origPos) {
        emit dragFinished(m_data.name);
    }
}

void UiElement::mouseMoveEvent(QGraphicsSceneMouseEvent *event)
{
    if (m_dragging && m_resizeEdge != None) {
        QPointF delta = event->scenePos() - m_dragStart;

        // Calculate new width/height with minimum constraints
        double newW = m_data.width;
        double newH = m_data.height;

        if (m_resizeEdge & Right) {
            newW = qMax(20.0, m_origSize.width() + delta.x());
        } else if (m_resizeEdge & Left) {
            newW = qMax(20.0, m_origSize.width() - delta.x());
        }

        if (m_resizeEdge & Bottom) {
            newH = qMax(20.0, m_origSize.height() + delta.y());
        } else if (m_resizeEdge & Top) {
            newH = qMax(20.0, m_origSize.height() - delta.y());
        }

        // Update position for left/top edge resize
        double newX = m_data.x;
        double newY = m_data.y;
        if (m_resizeEdge & Left) {
            newX = m_origPos.x() - (newW - m_origSize.width());
        }
        if (m_resizeEdge & Top) {
            newY = m_origPos.y() - (newH - m_origSize.height());
        }

        m_data.width = newW;
        m_data.height = newH;
        m_data.x = newX;
        m_data.y = newY;

        // Snap resize to grid
        auto *uiScene = qobject_cast<UiScene *>(scene());
        if (uiScene && uiScene->snapGridSize() > 1) {
            m_data.width = uiScene->snapToGrid(m_data.width);
            m_data.height = uiScene->snapToGrid(m_data.height);
            if (m_resizeEdge & Left) {
                m_data.x = uiScene->snapToGrid(m_data.x);
            }
            if (m_resizeEdge & Top) {
                m_data.y = uiScene->snapToGrid(m_data.y);
            }
        }

        prepareGeometryChange();
        setPos(m_data.x, m_data.y);
        update();
        emit elementResized(m_data.name, m_data.width, m_data.height);
    } else {
        QGraphicsObject::mouseMoveEvent(event);
    }
}

void UiElement::contextMenuEvent(QGraphicsSceneContextMenuEvent *event)
{
    setSelected(true);

    auto *uiScene = qobject_cast<UiScene *>(scene());
    if (!uiScene) return;

    QMenu menu;
    QAction *deleteAct = menu.addAction(tr("Delete"));
    QAction *renameAct = menu.addAction(tr("Rename..."));
    menu.addSeparator();
    QAction *addChildAct = menu.addAction(tr("Add Child"));
    QAction *dupAct = menu.addAction(tr("Duplicate"));
    menu.addSeparator();
    QAction *moveRootAct = menu.addAction(tr("Move to Root"));
    QAction *frontAct = menu.addAction(tr("Bring to Front"));
    QAction *backAct = menu.addAction(tr("Send to Back"));

    QAction *selected = menu.exec(event->screenPos());
    if (!selected) return;

    // Push undo state before modifying
    emit uiScene->contextActionTriggered();

    QString name = m_data.name;

    if (selected == deleteAct) {
        uiScene->deleteElement(name);
    } else if (selected == renameAct) {
        bool ok;
        QString newName = QInputDialog::getText(
            nullptr, tr("Rename Element"),
            tr("New name:"), QLineEdit::Normal, name, &ok);
        if (ok && !newName.isEmpty() && newName != name) {
            uiScene->renameElement(name, newName);
        }
    } else if (selected == addChildAct) {
        UiElementData childData;
        childData.name = uiScene->uniqueName("Child");
        childData.type = "SIMPLEFRAME";
        childData.x = m_data.x + 40;
        childData.y = m_data.y + 40;
        childData.width = 100;
        childData.height = 50;
        childData.parent = name;
        childData.visible = true;
        childData.locked = false;
        uiScene->addElementFromData(childData);
    } else if (selected == dupAct) {
        uiScene->duplicateElement(name);
    } else if (selected == moveRootAct) {
        uiScene->moveToRoot(name);
    } else if (selected == frontAct) {
        uiScene->bringToFront(name);
    } else if (selected == backAct) {
        uiScene->sendToBack(name);
    }
}

double UiElement::handleSize() const
{
    // Keep visual handle size consistent in screen pixels regardless of zoom
    double zoom = 1.0;
    if (scene() && !scene()->views().isEmpty()) {
        zoom = scene()->views().first()->transform().m11();
    }
    return HANDLE_SIZE / qMax(zoom, 0.01);
}

UiElement::ResizeEdge UiElement::hitTest(const QPointF &pos) const
{
    QRectF r(0, 0, m_data.width, m_data.height);
    double s = handleSize();

    // 4 corners (checked first for priority)
    QRectF tl(r.left() - s / 2, r.top() - s / 2, s, s);
    QRectF tr(r.right() - s / 2, r.top() - s / 2, s, s);
    QRectF bl(r.left() - s / 2, r.bottom() - s / 2, s, s);
    QRectF br(r.right() - s / 2, r.bottom() - s / 2, s, s);

    // 4 edges
    QRectF top(r.center().x() - s / 2, r.top() - s / 2, s, s);
    QRectF bottom(r.center().x() - s / 2, r.bottom() - s / 2, s, s);
    QRectF left(r.left() - s / 2, r.center().y() - s / 2, s, s);
    QRectF right(r.right() - s / 2, r.center().y() - s / 2, s, s);

    if (br.contains(pos)) return BottomRight;
    if (tr.contains(pos)) return TopRight;
    if (bl.contains(pos)) return BottomLeft;
    if (tl.contains(pos)) return TopLeft;
    if (right.contains(pos)) return Right;
    if (bottom.contains(pos)) return Bottom;
    if (left.contains(pos)) return Left;
    if (top.contains(pos)) return Top;

    return None;
}

void UiElement::drawHandles(QPainter *painter) const
{
    painter->setPen(QPen(Qt::white, 1));
    painter->setBrush(QColor(0, 160, 255));

    double s = handleSize();
    double w = m_data.width;
    double h = m_data.height;

    // 4 corners
    painter->drawRect(QRectF(-s / 2, -s / 2, s, s));                     // top-left
    painter->drawRect(QRectF(w - s / 2, -s / 2, s, s));                  // top-right
    painter->drawRect(QRectF(-s / 2, h - s / 2, s, s));                  // bottom-left
    painter->drawRect(QRectF(w - s / 2, h - s / 2, s, s));               // bottom-right
    // 4 edges
    painter->drawRect(QRectF(w / 2 - s / 2, -s / 2, s, s));             // top
    painter->drawRect(QRectF(w / 2 - s / 2, h - s / 2, s, s));          // bottom
    painter->drawRect(QRectF(-s / 2, h / 2 - s / 2, s, s));             // left
    painter->drawRect(QRectF(w - s / 2, h / 2 - s / 2, s, s));          // right
}
