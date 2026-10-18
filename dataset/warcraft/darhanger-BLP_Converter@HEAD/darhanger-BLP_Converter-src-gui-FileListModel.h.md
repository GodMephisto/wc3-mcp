---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/gui/FileListModel.h"
ref: HEAD
---

#pragma once

#include <QAbstractListModel>
#include <QImage>
#include <QString>
#include <QStringList>

namespace darblp::gui {

enum class FileStatus
{
    Waiting,
    Converting,
    Converted,
    Error
};

struct FileEntry
{
    QString path;
    FileStatus status = FileStatus::Waiting;
    QString message;
    QImage thumbnail;
};

class FileListModel : public QAbstractListModel
{
    Q_OBJECT

public:
    enum Roles
    {
        PathRole = Qt::UserRole + 1,
        StatusRole,
        MessageRole
    };

    explicit FileListModel(QObject *parent = nullptr);

    int rowCount(const QModelIndex &parent = QModelIndex()) const override;
    QVariant data(const QModelIndex &index, int role = Qt::DisplayRole) const override;
    bool removeRows(int row, int count, const QModelIndex &parent = QModelIndex()) override;
    QHash<int, QByteArray> roleNames() const override;

    void addFiles(const QStringList &paths);
    void clear();
    void setStatus(const QString &path, FileStatus status, const QString &message = QString());
    void setThumbnail(const QString &path, const QImage &image);

    FileEntry entryAt(int row) const;
    QStringList allPaths() const;
    QString statusText(FileStatus status) const;

private:
    int indexOfPath(const QString &path) const;
    QVariant placeholderIcon(const QString &path) const;

    QList<FileEntry> m_entries;
};

} // namespace darblp::gui
