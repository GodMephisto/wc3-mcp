---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "panels/texturepanel.h"
ref: HEAD
---

#ifndef TEXTUREPANEL_H
#define TEXTUREPANEL_H

#include <QWidget>
#include <QListWidget>
#include <QString>
#include <QPushButton>

class TexturePanel : public QWidget
{
    Q_OBJECT

public:
    explicit TexturePanel(QWidget *parent = nullptr);
    ~TexturePanel() = default;

    void loadTextures(const QString &texturesDir);
    void clearPanel();

signals:
    void textureSelected(const QString &texturePath);

private:
    void setupUi();
    void onItemClicked(QListWidgetItem *item);

    QListWidget *m_listWidget;
    QPushButton *m_refreshBtn;
    QString m_texturesDir;
};

#endif // TEXTUREPANEL_H
