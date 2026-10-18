---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "canvas/uiview.cpp"
ref: HEAD
---

#include "uiview.h"
#include "uiscene.h"

#include <QWheelEvent>
#include <QPainter>
#include <QScrollBar>
#include <QMouseEvent>
#include <QToolTip>

UiView::UiView(UiScene *scene, QWidget *parent)
    : QGraphicsView(scene, parent)
{
    setRenderHints(QPainter::Antialiasing | QPainter::SmoothPixmapTransform);
    setViewportUpdateMode(QGraphicsView::FullViewportUpdate);
    setDragMode(QGraphicsView::RubberBandDrag);
    setTransformationAnchor(QGraphicsView::AnchorUnderMouse);
    setResizeAnchor(QGraphicsView::AnchorUnderMouse);
    setAcceptDrops(true);
    setHorizontalScrollBarPolicy(Qt::ScrollBarAsNeeded);
    setVerticalScrollBarPolicy(Qt::ScrollBarAsNeeded);
    setMouseTracking(true);
}

void UiView::zoomIn()
{
    const double factor = 1.25;
    if (m_zoomFactor * factor <= MAX_ZOOM) {
        scale(factor, factor);
        m_zoomFactor *= factor;
        emit zoomChanged(m_zoomFactor);
    }
}

void UiView::zoomOut()
{
    const double factor = 1.0 / 1.25;
    if (m_zoomFactor * factor >= MIN_ZOOM) {
        scale(factor, factor);
        m_zoomFactor *= factor;
        emit zoomChanged(m_zoomFactor);
    }
}

void UiView::setShowRulers(bool show)
{
    m_showRulers = show;
    viewport()->update();
}

void UiView::fitAll()
{
    UiScene *s = qobject_cast<UiScene *>(scene());
    if (s && !s->items().isEmpty()) {
        fitInView(s->itemsBoundingRect(), Qt::KeepAspectRatio);
        m_zoomFactor = transform().m11();
    } else {
        resetTransform();
        m_zoomFactor = 1.0;
    }
    emit zoomChanged(m_zoomFactor);
}

void UiView::wheelEvent(QWheelEvent *event)
{
    if (event->modifiers() & Qt::ControlModifier) {
        const double factor = event->angleDelta().y() > 0 ? 1.15 : 1.0 / 1.15;
        double newZoom = m_zoomFactor * factor;
        if (newZoom >= MIN_ZOOM && newZoom <= MAX_ZOOM) {
            scale(factor, factor);
            m_zoomFactor = newZoom;
            emit zoomChanged(m_zoomFactor);
        }
        event->accept();
    } else {
        QGraphicsView::wheelEvent(event);
    }
}

static constexpr double SCREEN_W = 1920.0;
static constexpr double SCREEN_H = 1080.0;

void UiView::drawBackground(QPainter *painter, const QRectF &rect)
{
    // Dark background for off-screen area
    painter->fillRect(rect, QColor(45, 45, 48));

    // White background for the visible screen area
    QRectF screenRect(0, 0, SCREEN_W, SCREEN_H);
    painter->fillRect(screenRect, QColor(245, 245, 240));

    // Grid
    const int gridSize = 20;
    QPen gridPen(QColor(80, 80, 85), 0.5);

    qreal left = int(rect.left()) - (int(rect.left()) % gridSize);
    qreal top = int(rect.top()) - (int(rect.top()) % gridSize);

    QVector<QLineF> lines;
    for (qreal x = left; x < rect.right(); x += gridSize) {
        lines.append(QLineF(x, rect.top(), x, rect.bottom()));
    }
    for (qreal y = top; y < rect.bottom(); y += gridSize) {
        lines.append(QLineF(rect.left(), y, rect.right(), y));
    }
    painter->setPen(gridPen);
    painter->drawLines(lines);

    // Screen border
    painter->setPen(QPen(QColor(0, 160, 255), 2));
    painter->setBrush(Qt::NoBrush);
    painter->drawRect(screenRect);

    // Screen dimension label
    painter->setPen(QColor(0, 160, 255));
    painter->drawText(screenRect.adjusted(-200, -24, -4, 0),
                      Qt::AlignRight | Qt::AlignBottom,
                      QString("Screen 1920 x 1080"));
}

static constexpr int RULER = UiView::RULER_SIZE;

void UiView::paintEvent(QPaintEvent *event)
{
    // Draw the scene first
    QGraphicsView::paintEvent(event);

    if (!m_showRulers) return;

    // Draw rulers on top
    QPainter painter(viewport());
    drawRulers(painter);
}

void UiView::drawRulers(QPainter &painter)
{
    QRect vp = viewport()->rect();
    double zoom = m_zoomFactor;
    QPointF topLeft = mapToScene(QPoint(RULER, RULER));
    QPointF bottomRight = mapToScene(QPoint(vp.width(), vp.height()));

    // Ruler background
    painter.fillRect(0, 0, vp.width(), RULER, QColor(30, 31, 42));
    painter.fillRect(0, 0, RULER, vp.height(), QColor(30, 31, 42));
    painter.fillRect(0, 0, RULER, RULER, QColor(40, 41, 54)); // corner

    // Ruler border lines
    painter.setPen(QPen(QColor(50, 51, 64), 1));
    painter.drawLine(0, RULER, vp.width(), RULER);
    painter.drawLine(RULER, 0, RULER, vp.height());

    QFont font;
    font.setPixelSize(9);
    painter.setFont(font);

    // Determine tick interval based on zoom
    double tickStep = 100.0; // scene units between major ticks
    // Adjust tick step so ticks aren't too dense or sparse
    double pixelPerUnit = zoom;
    double desiredPixels = 80.0;
    double rawStep = desiredPixels / pixelPerUnit;
    double magnitudes[] = {1, 2, 5, 10, 20, 50, 100, 200, 500, 1000, 2000, 5000};
    tickStep = magnitudes[0];
    for (auto m : magnitudes) {
        if (m >= rawStep) { tickStep = m; break; }
    }

    auto drawTick = [&](double sceneX, double sceneY, int vpX, int vpY) {
        Q_UNUSED(sceneX); Q_UNUSED(sceneY);
        painter.setPen(QColor(120, 130, 160));

        // Horizontal ruler ticks
        if (vpX >= RULER && vpX <= vp.width()) {
            int majorTickSize = 12;
            painter.drawLine(vpX, 0, vpX, majorTickSize);
            // Tick label
            if (sceneX >= 0) {
                painter.drawText(vpX - 20, majorTickSize + 2, 40, RULER - majorTickSize - 2,
                                 Qt::AlignCenter, QString::number((int)sceneX));
            }
        }
        // Vertical ruler ticks
        if (vpY >= RULER && vpY <= vp.height()) {
            int majorTickSize = 12;
            painter.drawLine(0, vpY, majorTickSize, vpY);
            if (sceneY >= 0) {
                painter.save();
                painter.translate(2, vpY + 20);
                painter.rotate(-90);
                painter.drawText(0, -10, 40, RULER - majorTickSize - 2,
                                 Qt::AlignCenter, QString::number((int)sceneY));
                painter.restore();
            }
        }
        // Minor ticks
        painter.setPen(QColor(80, 90, 110));
        if (vpX >= RULER) painter.drawLine(vpX, 0, vpX, 6);
        if (vpY >= RULER) painter.drawLine(0, vpY, 6, vpY);
    };

    // Draw ticks along visible area
    if (zoom > 0) {
        // Horizontal ruler ticks
        double startX = qMax(0.0, topLeft.x() - fmod(topLeft.x(), tickStep));
        for (double sx = startX; sx <= bottomRight.x(); sx += tickStep) {
            QPoint vp = mapFromScene(QPointF(sx, 0));
            QPoint vp2 = mapFromScene(QPointF(0, sx));
            drawTick(sx, sx, vp.x(), vp2.y());
        }
    }
}

bool UiView::isOnRuler(const QPoint &pos) const
{
    if (!m_showRulers) return false;
    return pos.x() < RULER || pos.y() < RULER;
}

QPointF UiView::scenePosFromViewport(const QPoint &vp) const
{
    return mapToScene(vp);
}

void UiView::mousePressEvent(QMouseEvent *event)
{
    if (event->button() == Qt::LeftButton && isOnRuler(event->pos())) {
        m_draggingGuide = true;
        m_dragStartVp = event->pos();
        m_guideIsHorizontal = (event->pos().x() >= RULER);
        return;
    }
    QGraphicsView::mousePressEvent(event);
}

void UiView::mouseMoveEvent(QMouseEvent *event)
{
    if (m_draggingGuide) {
        QPointF scenePos = scenePosFromViewport(event->pos());
        auto *uiScene = qobject_cast<UiScene *>(scene());
        if (uiScene) {
            if (m_guideIsHorizontal) {
                uiScene->setTempGuide(scenePos.y(), true);
            } else {
                uiScene->setTempGuide(scenePos.x(), false);
            }
        }
        return;
    }
    QGraphicsView::mouseMoveEvent(event);
}

void UiView::mouseReleaseEvent(QMouseEvent *event)
{
    if (m_draggingGuide && event->button() == Qt::LeftButton) {
        m_draggingGuide = false;
        auto *uiScene = qobject_cast<UiScene *>(scene());
        if (uiScene) {
            QPointF scenePos = scenePosFromViewport(event->pos());
            if (m_guideIsHorizontal)
                uiScene->addGuide(scenePos.y(), true);
            else
                uiScene->addGuide(scenePos.x(), false);
            uiScene->clearTempGuide();
        }
        return;
    }
    QGraphicsView::mouseReleaseEvent(event);
}
