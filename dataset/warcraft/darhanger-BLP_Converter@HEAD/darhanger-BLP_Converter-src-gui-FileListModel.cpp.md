---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/gui/FileListModel.cpp"
ref: HEAD
---

#include "gui/FileListModel.h"

#include <QColor>
#include <QFileInfo>
#include <QFont>
#include <QPainter>
#include <QPixmap>
#include <QSet>

#include <utility>

namespace darblp::gui {

FileListModel::FileListModel(QObject *parent)
    : QAbstractListModel(parent)
{
}

int FileListModel::rowCount(const QModelIndex &parent) const
{
    return parent.isValid() ? 0 : m_entries.size();
}

QVariant FileListModel::data(const QModelIndex &index, int role) const
{
    if (!index.isValid() || index.row() < 0 || index.row() >= m_entries.size()) {
        return {};
    }

    const FileEntry &entry = m_entries.at(index.row());
    const QFileInfo info(entry.path);

    switch (role) {
    case Qt::DisplayRole:
        return QStringLiteral("%1\n%2").arg(info.fileName(), statusText(entry.status));
    case Qt::DecorationRole:
        if (!entry.thumbnail.isNull()) {
            return QPixmap::fromImage(entry.thumbnail.scaled(48, 48, Qt::KeepAspectRatio, Qt::SmoothTransformation));
        }
        return placeholderIcon(entry.path);
    case Qt::ToolTipRole:
        return entry.message.isEmpty() ? entry.path : QStringLiteral("%1\n%2").arg(entry.path, entry.message);
    case PathRole:
        return entry.path;
    case StatusRole:
        return statusText(entry.status);
    case MessageRole:
        return entry.message;
    default:
        return {};
    }
}

bool FileListModel::removeRows(int row, int count, const QModelIndex &parent)
{
    if (parent.isValid() || row < 0 || count <= 0 || row + count > m_entries.size()) {
        return false;
    }

    beginRemoveRows(parent, row, row + count - 1);
    for (int i = 0; i < count; ++i) {
        m_entries.removeAt(row);
    }
    endRemoveRows();
    return true;
}

QHash<int, QByteArray> FileListModel::roleNames() const
{
    QHash<int, QByteArray> roles = QAbstractListModel::roleNames();
    roles[PathRole] = "path";
    roles[StatusRole] = "status";
    roles[MessageRole] = "message";
    return roles;
}

void FileListModel::addFiles(const QStringList &paths)
{
    QSet<QString> known;
    for (const FileEntry &entry : std::as_const(m_entries)) {
        known.insert(entry.path);
    }

    QList<FileEntry> newEntries;
    for (const QString &path : paths) {
        const QString absolutePath = QFileInfo(path).absoluteFilePath();
        if (known.contains(absolutePath)) {
            continue;
        }

        known.insert(absolutePath);
        FileEntry entry;
        entry.path = absolutePath;
        newEntries.push_back(entry);
    }

    if (newEntries.isEmpty()) {
        return;
    }

    const int first = m_entries.size();
    beginInsertRows(QModelIndex(), first, first + newEntries.size() - 1);
    m_entries.append(newEntries);
    endInsertRows();
}

void FileListModel::clear()
{
    if (m_entries.isEmpty()) {
        return;
    }

    beginResetModel();
    m_entries.clear();
    endResetModel();
}

void FileListModel::setStatus(const QString &path, FileStatus status, const QString &message)
{
    const int row = indexOfPath(QFileInfo(path).absoluteFilePath());
    if (row < 0) {
        return;
    }

    m_entries[row].status = status;
    m_entries[row].message = message;
    const QModelIndex changed = index(row);
    emit dataChanged(changed, changed, {Qt::DisplayRole, Qt::ToolTipRole, StatusRole, MessageRole});
}

void FileListModel::setThumbnail(const QString &path, const QImage &image)
{
    const int row = indexOfPath(QFileInfo(path).absoluteFilePath());
    if (row < 0) {
        return;
    }

    m_entries[row].thumbnail = image;
    const QModelIndex changed = index(row);
    emit dataChanged(changed, changed, {Qt::DecorationRole});
}

FileEntry FileListModel::entryAt(int row) const
{
    if (row < 0 || row >= m_entries.size()) {
        return {};
    }
    return m_entries.at(row);
}

QStringList FileListModel::allPaths() const
{
    QStringList paths;
    paths.reserve(m_entries.size());
    for (const FileEntry &entry : m_entries) {
        paths.push_back(entry.path);
    }
    return paths;
}

QString FileListModel::statusText(FileStatus status) const
{
    switch (status) {
    case FileStatus::Waiting:
        return QStringLiteral("Waiting");
    case FileStatus::Converting:
        return QStringLiteral("Converting");
    case FileStatus::Converted:
        return QStringLiteral("Converted");
    case FileStatus::Error:
        return QStringLiteral("Error");
    }
    return QStringLiteral("Waiting");
}

int FileListModel::indexOfPath(const QString &path) const
{
    for (int i = 0; i < m_entries.size(); ++i) {
        if (m_entries.at(i).path == path) {
            return i;
        }
    }
    return -1;
}

QVariant FileListModel::placeholderIcon(const QString &path) const
{
    QPixmap pixmap(48, 48);
    pixmap.fill(Qt::transparent);

    QPainter painter(&pixmap);
    painter.setRenderHint(QPainter::Antialiasing);
    painter.setPen(Qt::NoPen);
    painter.setBrush(QColor(QStringLiteral("#252932")));
    painter.drawRoundedRect(QRectF(2, 2, 44, 44), 8, 8);
    painter.setBrush(QColor(QStringLiteral("#F28E26")));
    painter.drawRoundedRect(QRectF(7, 7, 34, 34), 6, 6);
    painter.setPen(QColor(QStringLiteral("#16181D")));
    painter.setFont(QFont(QStringLiteral("Segoe UI"), 8, QFont::DemiBold));
    painter.drawText(QRectF(4, 14, 40, 20),
                     Qt::AlignCenter,
                     QFileInfo(path).suffix().left(3).toUpper());

    return QVariant::fromValue(pixmap);
}

} // namespace darblp::gui
