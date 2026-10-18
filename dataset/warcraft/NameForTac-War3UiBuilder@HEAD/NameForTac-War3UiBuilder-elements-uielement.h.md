---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "elements/uielement.h"
ref: HEAD
---

#ifndef UIELEMENT_H
#define UIELEMENT_H

#include <QGraphicsObject>
#include <QPixmap>
#include "uielementdata.h"

class UiElement : public QGraphicsObject
{
    Q_OBJECT

public:
    enum { Type = QGraphicsItem::UserType + 1 };
    int type() const override { return Type; }

    explicit UiElement(const UiElementData &data, QGraphicsItem *parent = nullptr);
    ~UiElement() override = default;

    QRectF boundingRect() const override;
    void paint(QPainter *painter, const QStyleOptionGraphicsItem *option, QWidget *widget) override;
    QPainterPath shape() const override;

    bool loadTexture(const QString &path);
    bool hasValidTexture() const { return !m_pixmap.isNull(); }
    const UiElementData &elementData() const { return m_data; }
    void updateData(const UiElementData &data);
    void applyTexture(const QImage &image, const QString &path);

signals:
    void elementMoved(const QString &name, double x, double y, double dx, double dy);
    void elementResized(const QString &name, double w, double h);
    void dragFinished(const QString &name);

protected:
    QVariant itemChange(GraphicsItemChange change, const QVariant &value) override;
    void mousePressEvent(QGraphicsSceneMouseEvent *event) override;
    void mouseReleaseEvent(QGraphicsSceneMouseEvent *event) override;
    void mouseMoveEvent(QGraphicsSceneMouseEvent *event) override;
    void contextMenuEvent(QGraphicsSceneContextMenuEvent *event) override;

private:
    UiElementData m_data;
    QPixmap m_pixmap;
    bool m_dragging = false;
    bool m_isResizing = false;
    QPointF m_dragStart;
    QPointF m_origPos;

    enum ResizeEdge {
        None      = 0,
        Left      = 1,
        Right     = 2,
        Top       = 4,
        Bottom    = 8,
        TopLeft   = Top | Left,      // 5
        TopRight  = Top | Right,     // 6
        BottomLeft = Bottom | Left,  // 9
        BottomRight = Bottom | Right // 10
    };
    ResizeEdge m_resizeEdge = None;
    QSizeF m_origSize;
    static constexpr double HANDLE_SIZE = 14.0;

    double handleSize() const;
    ResizeEdge hitTest(const QPointF &pos) const;
    void drawHandles(QPainter *painter) const;
};

#endif // UIELEMENT_H
