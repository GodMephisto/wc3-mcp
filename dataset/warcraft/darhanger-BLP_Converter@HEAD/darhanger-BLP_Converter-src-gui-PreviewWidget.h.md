---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/gui/PreviewWidget.h"
ref: HEAD
---

#pragma once

#include <QImage>
#include <QWidget>

namespace darblp::gui {

enum class PreviewMode
{
    Color,
    Alpha,
    Mipmaps
};

class PreviewWidget : public QWidget
{
    Q_OBJECT

public:
    explicit PreviewWidget(QWidget *parent = nullptr);

    void setImage(const QImage &image);
    void clearImage();
    void setMode(PreviewMode mode);
    void fitToWindow();
    void actualSize();
    void zoomIn();
    void zoomOut();

protected:
    void paintEvent(QPaintEvent *event) override;

private:
    QImage imageForCurrentMode() const;
    void drawCheckerboard(QPainter &painter, const QRect &rect) const;
    void drawSingleImage(QPainter &painter, const QRect &rect, const QImage &image) const;
    void drawMipmaps(QPainter &painter, const QRect &rect) const;

    QImage m_image;
    PreviewMode m_mode = PreviewMode::Color;
    double m_zoom = 1.0;
    bool m_fit = true;
};

} // namespace darblp::gui
