---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/gui/PreviewWidget.cpp"
ref: HEAD
---

#include "gui/PreviewWidget.h"

#include "blp/MipmapGenerator.h"

#include <QColor>
#include <QFont>
#include <QPainter>
#include <QSizePolicy>
#include <QtMath>

namespace darblp::gui {

PreviewWidget::PreviewWidget(QWidget *parent)
    : QWidget(parent)
{
    setMinimumSize(360, 260);
    setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Expanding);
}

void PreviewWidget::setImage(const QImage &image)
{
    m_image = image;
    update();
}

void PreviewWidget::clearImage()
{
    m_image = QImage();
    update();
}

void PreviewWidget::setMode(PreviewMode mode)
{
    m_mode = mode;
    update();
}

void PreviewWidget::fitToWindow()
{
    m_fit = true;
    update();
}

void PreviewWidget::actualSize()
{
    m_fit = false;
    m_zoom = 1.0;
    update();
}

void PreviewWidget::zoomIn()
{
    m_fit = false;
    m_zoom = qMin(8.0, m_zoom * 1.25);
    update();
}

void PreviewWidget::zoomOut()
{
    m_fit = false;
    m_zoom = qMax(0.125, m_zoom / 1.25);
    update();
}

void PreviewWidget::paintEvent(QPaintEvent *event)
{
    Q_UNUSED(event)

    QPainter painter(this);
    painter.setRenderHint(QPainter::Antialiasing);
    painter.fillRect(rect(), QColor(QStringLiteral("#16181D")));

    const QRect content = rect().adjusted(18, 18, -18, -18);
    painter.setPen(QColor(QStringLiteral("#2E333D")));
    painter.setBrush(QColor(QStringLiteral("#1B1D23")));
    painter.drawRoundedRect(content, 8, 8);

    const QRect imageRect = content.adjusted(14, 14, -14, -14);
    drawCheckerboard(painter, imageRect);

    if (m_image.isNull()) {
        painter.setPen(QColor(QStringLiteral("#9DA3AF")));
        painter.setFont(QFont(QStringLiteral("Segoe UI"), 13, QFont::Medium));
        painter.drawText(imageRect, Qt::AlignCenter, QStringLiteral("No preview"));
        return;
    }

    if (m_mode == PreviewMode::Mipmaps) {
        drawMipmaps(painter, imageRect);
    } else {
        drawSingleImage(painter, imageRect, imageForCurrentMode());
    }
}

QImage PreviewWidget::imageForCurrentMode() const
{
    if (m_mode != PreviewMode::Alpha || m_image.isNull()) {
        return m_image;
    }

    const QImage source = m_image.convertToFormat(QImage::Format_RGBA8888);
    QImage alpha(source.size(), QImage::Format_Grayscale8);

    for (int y = 0; y < source.height(); ++y) {
        const QRgb *src = reinterpret_cast<const QRgb *>(source.constScanLine(y));
        uchar *dst = alpha.scanLine(y);
        for (int x = 0; x < source.width(); ++x) {
            dst[x] = qAlpha(src[x]);
        }
    }

    return alpha;
}

void PreviewWidget::drawCheckerboard(QPainter &painter, const QRect &rect) const
{
    painter.save();
    painter.setClipRect(rect);
    const int cell = 16;
    for (int y = rect.top(); y < rect.bottom(); y += cell) {
        for (int x = rect.left(); x < rect.right(); x += cell) {
            const bool light = ((x / cell) + (y / cell)) % 2 == 0;
            painter.fillRect(QRect(x, y, cell, cell),
                             light ? QColor(QStringLiteral("#30343D")) : QColor(QStringLiteral("#242832")));
        }
    }
    painter.restore();
}

void PreviewWidget::drawSingleImage(QPainter &painter, const QRect &rect, const QImage &image) const
{
    if (image.isNull()) {
        return;
    }

    painter.save();
    painter.setClipRect(rect);

    QSize targetSize = image.size();
    if (m_fit) {
        targetSize.scale(rect.size(), Qt::KeepAspectRatio);
    } else {
        targetSize = QSize(qRound(image.width() * m_zoom), qRound(image.height() * m_zoom));
    }

    const QPoint topLeft(rect.center().x() - targetSize.width() / 2,
                         rect.center().y() - targetSize.height() / 2);
    painter.drawImage(QRect(topLeft, targetSize), image);
    painter.restore();
}

void PreviewWidget::drawMipmaps(QPainter &painter, const QRect &rect) const
{
    const QVector<QImage> mips = darblp::MipmapGenerator::generate(m_image);
    if (mips.isEmpty()) {
        return;
    }

    painter.save();
    painter.setClipRect(rect);

    const int gap = 10;
    const int columns = qMax(1, rect.width() / 160);
    const int tileWidth = qMax(90, (rect.width() - gap * (columns - 1)) / columns);
    int x = rect.left();
    int y = rect.top();
    int rowHeight = 0;

    painter.setFont(QFont(QStringLiteral("Segoe UI"), 8));
    for (int i = 0; i < mips.size(); ++i) {
        QImage mip = mips.at(i);
        QSize size = mip.size();
        size.scale(QSize(tileWidth, tileWidth), Qt::KeepAspectRatio);
        const QRect tile(x, y, tileWidth, tileWidth + 24);
        const QRect imageTarget(x + (tileWidth - size.width()) / 2, y, size.width(), size.height());
        painter.drawImage(imageTarget, mip);
        painter.setPen(QColor(QStringLiteral("#E8E8E8")));
        painter.drawText(QRect(x, y + tileWidth + 4, tileWidth, 18),
                         Qt::AlignCenter,
                         QStringLiteral("%1 x %2").arg(mip.width()).arg(mip.height()));

        rowHeight = qMax(rowHeight, tile.height());
        x += tileWidth + gap;
        if (x + tileWidth > rect.right()) {
            x = rect.left();
            y += rowHeight + gap;
            rowHeight = 0;
        }

        if (y > rect.bottom()) {
            break;
        }
    }

    painter.restore();
}

} // namespace darblp::gui
