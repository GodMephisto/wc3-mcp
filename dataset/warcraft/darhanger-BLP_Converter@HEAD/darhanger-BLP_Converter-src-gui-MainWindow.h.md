---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/gui/MainWindow.h"
ref: HEAD
---

#pragma once

#include "core/ConversionOptions.h"
#include "gui/FileListModel.h"

#include <QMainWindow>

class QCheckBox;
class QComboBox;
class QDragEnterEvent;
class QDropEvent;
class QLabel;
class QLineEdit;
class QListView;
class QProgressBar;
class QPushButton;

namespace darblp::gui {

class LogWidget;
class PreviewWidget;

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget *parent = nullptr);

protected:
    void dragEnterEvent(QDragEnterEvent *event) override;
    void dropEvent(QDropEvent *event) override;

private slots:
    void addFiles();
    void addFolder();
    void removeSelected();
    void clearFiles();
    void convertSelected();
    void convertAll();
    void browseOutputFolder();
    void handleSelectionChanged();

private:
    QWidget *createLeftPanel();
    QWidget *createCenterPanel();
    QWidget *createRightPanel();
    QWidget *createBottomPanel();
    QLabel *createValueLabel() const;

    void addInputPaths(const QStringList &paths);
    void updateMetadataForPath(const QString &path);
    void clearMetadata();
    void startConversion(const QStringList &files);
    void setControlsEnabled(bool enabled);
    QStringList selectedPaths() const;
    ConversionOptions currentOptions() const;

    FileListModel *m_fileModel = nullptr;
    QListView *m_fileView = nullptr;
    PreviewWidget *m_preview = nullptr;
    LogWidget *m_log = nullptr;
    QProgressBar *m_progress = nullptr;

    QLabel *m_formatValue = nullptr;
    QLabel *m_widthValue = nullptr;
    QLabel *m_heightValue = nullptr;
    QLabel *m_mipmapsValue = nullptr;
    QLabel *m_alphaDepthValue = nullptr;
    QLabel *m_compressionValue = nullptr;
    QLabel *m_fileSizeValue = nullptr;

    QComboBox *m_outputFormatCombo = nullptr;
    QComboBox *m_compressionCombo = nullptr;
    QCheckBox *m_generateMipmapsCheck = nullptr;
    QCheckBox *m_preserveAlphaCheck = nullptr;
    QCheckBox *m_resizePowerOfTwoCheck = nullptr;
    QCheckBox *m_keepFolderStructureCheck = nullptr;
    QLineEdit *m_outputFolderEdit = nullptr;

    QPushButton *m_addFilesButton = nullptr;
    QPushButton *m_addFolderButton = nullptr;
    QPushButton *m_convertSelectedButton = nullptr;
    QPushButton *m_convertAllButton = nullptr;

    QString m_sourceRoot;
    bool m_conversionRunning = false;
};

} // namespace darblp::gui
