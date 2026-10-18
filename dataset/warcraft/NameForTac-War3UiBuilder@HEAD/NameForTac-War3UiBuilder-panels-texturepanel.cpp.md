---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "panels/texturepanel.cpp"
ref: HEAD
---

#include "texturepanel.h"

#include <QVBoxLayout>
#include <QLabel>
#include <QDir>
#include <QFileInfo>
#include <QIcon>
#include <QPixmap>
#include <QFileSystemWatcher>

TexturePanel::TexturePanel(QWidget *parent)
    : QWidget(parent)
{
    setupUi();
}

void TexturePanel::setupUi()
{
    auto *layout = new QVBoxLayout(this);
    layout->setContentsMargins(4, 4, 4, 4);
    layout->setSpacing(4);

    auto *header = new QLabel(tr("Textures"), this);
    header->setStyleSheet(
        "QLabel { color: #a9b1d6; font-weight: 600; font-size: 12px; "
        "padding: 4px 0; }");
    layout->addWidget(header);

    m_listWidget = new QListWidget(this);
    m_listWidget->setViewMode(QListView::IconMode);
    m_listWidget->setIconSize(QSize(64, 64));
    m_listWidget->setGridSize(QSize(80, 90));
    m_listWidget->setResizeMode(QListView::Adjust);
    m_listWidget->setWordWrap(true);
    m_listWidget->setSpacing(4);
    m_listWidget->setTextElideMode(Qt::ElideMiddle);
    m_listWidget->setHorizontalScrollBarPolicy(Qt::ScrollBarAlwaysOff);
    m_listWidget->setStyleSheet(
        "QListWidget { background-color: #1a1b26; border: 1px solid #32334a; "
        "border-radius: 4px; }"
        "QListWidget::item { padding: 4px; border-radius: 4px; }"
        "QListWidget::item:selected { background-color: #3b3d5c; }"
        "QListWidget::item:hover { background-color: #2a2b3e; }");

    connect(m_listWidget, &QListWidget::itemClicked,
            this, &TexturePanel::onItemClicked);

    layout->addWidget(m_listWidget);

    m_refreshBtn = new QPushButton(tr("Refresh"), this);
    connect(m_refreshBtn, &QPushButton::clicked, this, [this]() {
        if (!m_texturesDir.isEmpty())
            loadTextures(m_texturesDir);
    });
    layout->addWidget(m_refreshBtn);
}

void TexturePanel::loadTextures(const QString &texturesDir)
{
    m_texturesDir = texturesDir;
    m_listWidget->clear();

    if (texturesDir.isEmpty()) {
        auto *item = new QListWidgetItem(tr("No project open"));
        item->setFlags(item->flags() & ~Qt::ItemIsSelectable);
        m_listWidget->addItem(item);
        return;
    }

    QDir dir(texturesDir);
    if (!dir.exists()) {
        auto *item = new QListWidgetItem(tr("No textures folder"));
        item->setFlags(item->flags() & ~Qt::ItemIsSelectable);
        m_listWidget->addItem(item);
        return;
    }

    QStringList filters;
    filters << "*.png" << "*.jpg" << "*.jpeg" << "*.bmp"
            << "*.tga" << "*.dds" << "*.hdr";
    QFileInfoList files = dir.entryInfoList(filters, QDir::Files, QDir::Name);

    if (files.isEmpty()) {
        auto *item = new QListWidgetItem(tr("(empty)"));
        item->setFlags(item->flags() & ~Qt::ItemIsSelectable);
        m_listWidget->addItem(item);
        return;
    }

    for (const auto &fi : files) {
        QPixmap thumb(fi.absoluteFilePath());
        if (thumb.isNull()) continue;

        thumb = thumb.scaled(64, 64, Qt::KeepAspectRatio, Qt::SmoothTransformation);

        auto *item = new QListWidgetItem(QIcon(thumb), fi.fileName());
        item->setData(Qt::UserRole, fi.absoluteFilePath());
        item->setToolTip(fi.absoluteFilePath());
        m_listWidget->addItem(item);
    }
}

void TexturePanel::clearPanel()
{
    m_texturesDir.clear();
    m_listWidget->clear();
    auto *item = new QListWidgetItem(tr("No project open"));
    item->setFlags(item->flags() & ~Qt::ItemIsSelectable);
    m_listWidget->addItem(item);
}

void TexturePanel::onItemClicked(QListWidgetItem *item)
{
    if (!item) return;
    QString path = item->data(Qt::UserRole).toString();
    if (!path.isEmpty()) {
        emit textureSelected(path);
    }
}
