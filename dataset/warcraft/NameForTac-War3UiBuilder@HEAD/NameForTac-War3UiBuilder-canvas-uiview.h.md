---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "canvas/uiview.h"
ref: HEAD
---

#ifndef UIVIEW_H
#define UIVIEW_H

#include <QGraphicsView>
#include <QList>

class UiScene;

class UiView : public QGraphicsView
{
    Q_OBJECT

public:
    explicit UiView(UiScene *scene, QWidget *parent = nullptr);
    ~UiView() = default;

    static constexpr int RULER_SIZE = 20;

public slots:
    void zoomIn();
    void zoomOut();
    void fitAll();
    void setShowRulers(bool show);
    bool showRulers() const { return m_showRulers; }

protected:
    void wheelEvent(QWheelEvent *event) override;
    void drawBackground(QPainter *painter, const QRectF &rect) override;
    void paintEvent(QPaintEvent *event) override;
    void mousePressEvent(QMouseEvent *event) override;
    void mouseMoveEvent(QMouseEvent *event) override;
    void mouseReleaseEvent(QMouseEvent *event) override;

private:
    void drawRulers(QPainter &painter);
    bool isOnRuler(const QPoint &pos) const;
    QPointF scenePosFromViewport(const QPoint &vp) const;

signals:
    void zoomChanged(double factor);

private:
    double m_zoomFactor = 1.0;
    static constexpr double MIN_ZOOM = 0.1;
    static constexpr double MAX_ZOOM = 10.0;

    // Ruler visibility
    bool m_showRulers = true;

    // Guide drag state
    bool m_draggingGuide = false;
    bool m_guideIsHorizontal = false;
    QPoint m_dragStartVp;
};

#endif // UIVIEW_H
