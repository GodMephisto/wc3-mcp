---
type: vendor-doc
source_repo: "https://github.com/darhanger/BLP_Converter"
source_path: "src/gui/MainWindow.cpp"
ref: HEAD
---

#include "gui/MainWindow.h"

#include "blp/BlpReader.h"
#include "core/Converter.h"
#include "gui/ConversionWorker.h"
#include "gui/LogWidget.h"
#include "gui/PreviewWidget.h"
#include "utils/FileUtils.h"

#include <QAbstractItemView>
#include <QBoxLayout>
#include <QButtonGroup>
#include <QCheckBox>
#include <QComboBox>
#include <QDragEnterEvent>
#include <QFileDialog>
#include <QFileInfo>
#include <QFont>
#include <QFormLayout>
#include <QFrame>
#include <QGroupBox>
#include <QIcon>
#include <QItemSelectionModel>
#include <QLabel>
#include <QLineEdit>
#include <QListView>
#include <QMimeData>
#include <QProgressBar>
#include <QPushButton>
#include <QSizePolicy>
#include <QSplitter>
#include <QThread>
#include <QToolButton>
#include <QUrl>

#include <algorithm>

namespace darblp::gui {

namespace {

QFrame *panelFrame()
{
    auto *frame = new QFrame;
    frame->setObjectName(QStringLiteral("Panel"));
    frame->setSizePolicy(QSizePolicy::Preferred, QSizePolicy::Preferred);
    return frame;
}

QLabel *sectionLabel(const QString &text)
{
    auto *label = new QLabel(text);
    QFont font = label->font();
    font.setPointSize(font.pointSize() + 1);
    font.setWeight(QFont::DemiBold);
    label->setFont(font);
    label->setSizePolicy(QSizePolicy::Minimum, QSizePolicy::Fixed);
    return label;
}

void configureToolbarButton(QToolButton *button, int width)
{
    button->setFixedSize(width, 36);
    button->setSizePolicy(QSizePolicy::Fixed, QSizePolicy::Fixed);
}

void configurePanelButton(QPushButton *button, int minimumWidth)
{
    button->setMinimumSize(minimumWidth, 36);
    button->setSizePolicy(QSizePolicy::Preferred, QSizePolicy::Fixed);
}

void configureField(QWidget *widget)
{
    widget->setMinimumHeight(32);
    widget->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Fixed);
}

} // namespace

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent)
{
    qRegisterMetaType<darblp::ConversionResult>("darblp::ConversionResult");

    setWindowTitle(QStringLiteral("DarBLP Converter"));
    setWindowIcon(QIcon(QStringLiteral(":/icons/app.png")));
    setAcceptDrops(true);
    setMinimumSize(1240, 840);

    auto *central = new QWidget(this);
    auto *rootLayout = new QVBoxLayout(central);
    rootLayout->setContentsMargins(16, 16, 16, 16);
    rootLayout->setSpacing(12);

    auto *mainSplitter = new QSplitter(Qt::Horizontal, central);
    mainSplitter->setChildrenCollapsible(false);
    mainSplitter->setHandleWidth(8);
    mainSplitter->addWidget(createLeftPanel());
    mainSplitter->addWidget(createCenterPanel());
    mainSplitter->addWidget(createRightPanel());
    mainSplitter->setStretchFactor(0, 0);
    mainSplitter->setStretchFactor(1, 1);
    mainSplitter->setStretchFactor(2, 0);
    mainSplitter->setSizes({300, 620, 350});

    rootLayout->addWidget(mainSplitter, 1);
    rootLayout->addWidget(createBottomPanel(), 0);
    setCentralWidget(central);

    connect(m_fileView->selectionModel(), &QItemSelectionModel::selectionChanged,
            this, &MainWindow::handleSelectionChanged);
}

void MainWindow::dragEnterEvent(QDragEnterEvent *event)
{
    if (event->mimeData()->hasUrls()) {
        event->acceptProposedAction();
    }
}

void MainWindow::dropEvent(QDropEvent *event)
{
    QStringList paths;
    for (const QUrl &url : event->mimeData()->urls()) {
        if (url.isLocalFile()) {
            paths.push_back(url.toLocalFile());
        }
    }

    addInputPaths(paths);
    event->acceptProposedAction();
}

void MainWindow::addFiles()
{
    const QString filter = QStringLiteral("Textures (*.png *.tga *.blp)");
    const QStringList files = QFileDialog::getOpenFileNames(this,
                                                            QStringLiteral("Add Files"),
                                                            QString(),
                                                            filter);
    addInputPaths(files);
}

void MainWindow::addFolder()
{
    const QString folder = QFileDialog::getExistingDirectory(this, QStringLiteral("Add Folder"));
    if (!folder.isEmpty()) {
        m_sourceRoot = QFileInfo(folder).absoluteFilePath();
        addInputPaths({folder});
    }
}

void MainWindow::removeSelected()
{
    QModelIndexList rows = m_fileView->selectionModel()->selectedRows();
    std::sort(rows.begin(), rows.end(), [](const QModelIndex &a, const QModelIndex &b) {
        return a.row() > b.row();
    });

    for (const QModelIndex &index : rows) {
        m_fileModel->removeRows(index.row(), 1);
    }

    if (m_fileModel->rowCount() == 0) {
        m_preview->clearImage();
        clearMetadata();
    }
}

void MainWindow::clearFiles()
{
    m_fileModel->clear();
    m_preview->clearImage();
    clearMetadata();
    m_progress->setValue(0);
    m_log->appendInfo(QStringLiteral("File list cleared."));
}

void MainWindow::convertSelected()
{
    startConversion(selectedPaths());
}

void MainWindow::convertAll()
{
    startConversion(m_fileModel->allPaths());
}

void MainWindow::browseOutputFolder()
{
    const QString folder = QFileDialog::getExistingDirectory(this, QStringLiteral("Output Folder"));
    if (!folder.isEmpty()) {
        m_outputFolderEdit->setText(QFileInfo(folder).absoluteFilePath());
    }
}

void MainWindow::handleSelectionChanged()
{
    const QModelIndexList rows = m_fileView->selectionModel()->selectedRows();
    if (rows.isEmpty()) {
        m_preview->clearImage();
        clearMetadata();
        return;
    }

    updateMetadataForPath(m_fileModel->entryAt(rows.first().row()).path);
}

QWidget *MainWindow::createLeftPanel()
{
    QFrame *panel = panelFrame();
    panel->setMinimumWidth(280);
    panel->setMaximumWidth(360);

    auto *layout = new QVBoxLayout(panel);
    layout->setContentsMargins(12, 12, 12, 12);
    layout->setSpacing(10);

    layout->addWidget(sectionLabel(QStringLiteral("Files")));

    m_fileModel = new FileListModel(this);
    m_fileView = new QListView(panel);
    m_fileView->setModel(m_fileModel);
    m_fileView->setSelectionMode(QAbstractItemView::ExtendedSelection);
    m_fileView->setIconSize(QSize(48, 48));
    m_fileView->setSpacing(2);
    m_fileView->setAlternatingRowColors(false);
    m_fileView->setHorizontalScrollBarPolicy(Qt::ScrollBarAlwaysOff);
    m_fileView->setTextElideMode(Qt::ElideMiddle);
    m_fileView->setWordWrap(true);
    layout->addWidget(m_fileView, 1);

    auto *buttons = new QHBoxLayout;
    buttons->setSpacing(8);
    auto *removeButton = new QPushButton(QStringLiteral("Remove"), panel);
    auto *clearButton = new QPushButton(QStringLiteral("Clear"), panel);
    configurePanelButton(removeButton, 96);
    configurePanelButton(clearButton, 96);
    buttons->addWidget(removeButton, 1);
    buttons->addWidget(clearButton, 1);
    layout->addLayout(buttons);

    connect(removeButton, &QPushButton::clicked, this, &MainWindow::removeSelected);
    connect(clearButton, &QPushButton::clicked, this, &MainWindow::clearFiles);

    return panel;
}

QWidget *MainWindow::createCenterPanel()
{
    QFrame *panel = panelFrame();
    panel->setMinimumWidth(460);
    panel->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Preferred);

    auto *layout = new QVBoxLayout(panel);
    layout->setContentsMargins(12, 12, 12, 12);
    layout->setSpacing(10);

    auto *toolbar = new QHBoxLayout;
    toolbar->setSpacing(6);
    toolbar->addWidget(sectionLabel(QStringLiteral("Preview")), 0, Qt::AlignVCenter);

    auto *modeGroup = new QButtonGroup(panel);
    modeGroup->setExclusive(true);

    auto *colorButton = new QToolButton(panel);
    colorButton->setText(QStringLiteral("Color"));
    colorButton->setCheckable(true);
    colorButton->setChecked(true);
    configureToolbarButton(colorButton, 68);
    colorButton->setToolTip(QStringLiteral("Show color preview"));

    auto *alphaButton = new QToolButton(panel);
    alphaButton->setText(QStringLiteral("Alpha"));
    alphaButton->setCheckable(true);
    configureToolbarButton(alphaButton, 68);
    alphaButton->setToolTip(QStringLiteral("Show alpha channel"));

    auto *mipButton = new QToolButton(panel);
    mipButton->setText(QStringLiteral("Mipmaps"));
    mipButton->setCheckable(true);
    configureToolbarButton(mipButton, 92);
    mipButton->setToolTip(QStringLiteral("Show generated mipmaps"));

    modeGroup->addButton(colorButton, static_cast<int>(PreviewMode::Color));
    modeGroup->addButton(alphaButton, static_cast<int>(PreviewMode::Alpha));
    modeGroup->addButton(mipButton, static_cast<int>(PreviewMode::Mipmaps));

    toolbar->addStretch(1);
    toolbar->addWidget(colorButton, 0, Qt::AlignVCenter);
    toolbar->addWidget(alphaButton, 0, Qt::AlignVCenter);
    toolbar->addWidget(mipButton, 0, Qt::AlignVCenter);

    auto *fitButton = new QToolButton(panel);
    fitButton->setText(QStringLiteral("Fit"));
    configureToolbarButton(fitButton, 54);
    fitButton->setToolTip(QStringLiteral("Fit preview to panel"));
    auto *actualButton = new QToolButton(panel);
    actualButton->setText(QStringLiteral("100%"));
    configureToolbarButton(actualButton, 62);
    actualButton->setToolTip(QStringLiteral("Show at actual size"));
    auto *zoomOutButton = new QToolButton(panel);
    zoomOutButton->setText(QStringLiteral("-"));
    configureToolbarButton(zoomOutButton, 42);
    zoomOutButton->setToolTip(QStringLiteral("Zoom out"));
    auto *zoomInButton = new QToolButton(panel);
    zoomInButton->setText(QStringLiteral("+"));
    configureToolbarButton(zoomInButton, 42);
    zoomInButton->setToolTip(QStringLiteral("Zoom in"));

    toolbar->addSpacing(8);
    toolbar->addWidget(fitButton, 0, Qt::AlignVCenter);
    toolbar->addWidget(actualButton, 0, Qt::AlignVCenter);
    toolbar->addWidget(zoomOutButton, 0, Qt::AlignVCenter);
    toolbar->addWidget(zoomInButton, 0, Qt::AlignVCenter);
    layout->addLayout(toolbar);

    m_preview = new PreviewWidget(panel);
    layout->addWidget(m_preview, 1);

    connect(modeGroup, &QButtonGroup::idClicked, this, [this](int id) {
        m_preview->setMode(static_cast<PreviewMode>(id));
    });
    connect(fitButton, &QToolButton::clicked, m_preview, &PreviewWidget::fitToWindow);
    connect(actualButton, &QToolButton::clicked, m_preview, &PreviewWidget::actualSize);
    connect(zoomOutButton, &QToolButton::clicked, m_preview, &PreviewWidget::zoomOut);
    connect(zoomInButton, &QToolButton::clicked, m_preview, &PreviewWidget::zoomIn);

    return panel;
}

QWidget *MainWindow::createRightPanel()
{
    QFrame *panel = panelFrame();
    panel->setMinimumWidth(340);
    panel->setMaximumWidth(420);

    auto *layout = new QVBoxLayout(panel);
    layout->setContentsMargins(12, 12, 12, 12);
    layout->setSpacing(10);

    auto *infoGroup = new QGroupBox(QStringLiteral("File Info"), panel);
    infoGroup->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Maximum);
    auto *infoForm = new QFormLayout(infoGroup);
    infoForm->setContentsMargins(14, 20, 14, 14);
    infoForm->setLabelAlignment(Qt::AlignLeft);
    infoForm->setFormAlignment(Qt::AlignTop);
    infoForm->setFieldGrowthPolicy(QFormLayout::AllNonFixedFieldsGrow);
    infoForm->setRowWrapPolicy(QFormLayout::DontWrapRows);
    infoForm->setHorizontalSpacing(14);
    infoForm->setVerticalSpacing(6);

    m_formatValue = createValueLabel();
    m_widthValue = createValueLabel();
    m_heightValue = createValueLabel();
    m_mipmapsValue = createValueLabel();
    m_alphaDepthValue = createValueLabel();
    m_compressionValue = createValueLabel();
    m_fileSizeValue = createValueLabel();

    infoForm->addRow(QStringLiteral("Format"), m_formatValue);
    infoForm->addRow(QStringLiteral("Width"), m_widthValue);
    infoForm->addRow(QStringLiteral("Height"), m_heightValue);
    infoForm->addRow(QStringLiteral("Mipmaps"), m_mipmapsValue);
    infoForm->addRow(QStringLiteral("Alpha depth"), m_alphaDepthValue);
    infoForm->addRow(QStringLiteral("Compression"), m_compressionValue);
    infoForm->addRow(QStringLiteral("File size"), m_fileSizeValue);

    auto *settingsGroup = new QGroupBox(QStringLiteral("Export"), panel);
    settingsGroup->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Expanding);
    auto *settingsLayout = new QVBoxLayout(settingsGroup);
    settingsLayout->setContentsMargins(14, 18, 14, 12);
    settingsLayout->setSpacing(8);

    auto *settingsFormContainer = new QWidget(settingsGroup);
    settingsFormContainer->setObjectName(QStringLiteral("TransparentPanel"));
    settingsFormContainer->setMinimumHeight(102);
    settingsFormContainer->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Fixed);

    auto *settingsForm = new QFormLayout(settingsFormContainer);
    settingsForm->setContentsMargins(0, 0, 0, 0);
    settingsForm->setLabelAlignment(Qt::AlignLeft);
    settingsForm->setFormAlignment(Qt::AlignTop);
    settingsForm->setFieldGrowthPolicy(QFormLayout::AllNonFixedFieldsGrow);
    settingsForm->setRowWrapPolicy(QFormLayout::DontWrapRows);
    settingsForm->setHorizontalSpacing(14);
    settingsForm->setVerticalSpacing(8);
    m_outputFormatCombo = new QComboBox(settingsGroup);
    m_outputFormatCombo->addItem(QStringLiteral("PNG"), static_cast<int>(OutputFormat::Png));
    m_outputFormatCombo->addItem(QStringLiteral("TGA"), static_cast<int>(OutputFormat::Tga));
    m_outputFormatCombo->addItem(QStringLiteral("BLP"), static_cast<int>(OutputFormat::Blp));
    m_outputFormatCombo->setCurrentIndex(2);

    m_compressionCombo = new QComboBox(settingsGroup);
    m_compressionCombo->addItem(QStringLiteral("Auto"), static_cast<int>(BlpCompression::Auto));
    m_compressionCombo->addItem(QStringLiteral("DXT1"), static_cast<int>(BlpCompression::Dxt1));
    m_compressionCombo->addItem(QStringLiteral("DXT3"), static_cast<int>(BlpCompression::Dxt3));
    m_compressionCombo->addItem(QStringLiteral("DXT5"), static_cast<int>(BlpCompression::Dxt5));
    configureField(m_outputFormatCombo);
    configureField(m_compressionCombo);

    auto *blpVersionLabel = new QLabel(QStringLiteral("BLP2"), settingsFormContainer);
    blpVersionLabel->setObjectName(QStringLiteral("Muted"));
    blpVersionLabel->setAlignment(Qt::AlignLeft | Qt::AlignVCenter);
    blpVersionLabel->setMinimumHeight(22);
    blpVersionLabel->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Fixed);
    settingsForm->addRow(QStringLiteral("Output format"), m_outputFormatCombo);
    settingsForm->addRow(QStringLiteral("BLP version"), blpVersionLabel);
    settingsForm->addRow(QStringLiteral("Compression"), m_compressionCombo);
    settingsLayout->addWidget(settingsFormContainer);

    m_generateMipmapsCheck = new QCheckBox(QStringLiteral("Generate mipmaps"), settingsGroup);
    m_generateMipmapsCheck->setChecked(true);
    m_preserveAlphaCheck = new QCheckBox(QStringLiteral("Preserve alpha"), settingsGroup);
    m_preserveAlphaCheck->setChecked(true);
    m_resizePowerOfTwoCheck = new QCheckBox(QStringLiteral("Resize to power of two"), settingsGroup);
    m_keepFolderStructureCheck = new QCheckBox(QStringLiteral("Keep folder structure"), settingsGroup);
    m_keepFolderStructureCheck->setChecked(true);

    settingsLayout->addWidget(m_generateMipmapsCheck);
    settingsLayout->addWidget(m_preserveAlphaCheck);
    settingsLayout->addWidget(m_resizePowerOfTwoCheck);
    settingsLayout->addWidget(m_keepFolderStructureCheck);

    auto *folderRow = new QHBoxLayout;
    folderRow->setSpacing(8);
    m_outputFolderEdit = new QLineEdit(settingsGroup);
    m_outputFolderEdit->setPlaceholderText(QStringLiteral("Output folder"));
    auto *browseButton = new QPushButton(QStringLiteral("Browse"), settingsGroup);
    configureField(m_outputFolderEdit);
    configurePanelButton(browseButton, 86);
    folderRow->addWidget(m_outputFolderEdit, 1);
    folderRow->addWidget(browseButton);
    settingsLayout->addLayout(folderRow);
    connect(browseButton, &QPushButton::clicked, this, &MainWindow::browseOutputFolder);

    layout->addWidget(infoGroup, 0);
    layout->addWidget(settingsGroup, 1);

    clearMetadata();
    return panel;
}

QWidget *MainWindow::createBottomPanel()
{
    QFrame *panel = panelFrame();
    panel->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Fixed);

    auto *layout = new QVBoxLayout(panel);
    layout->setContentsMargins(12, 12, 12, 12);
    layout->setSpacing(8);

    auto *actions = new QHBoxLayout;
    actions->setSpacing(8);
    m_addFilesButton = new QPushButton(QStringLiteral("Add Files"), panel);
    m_addFolderButton = new QPushButton(QStringLiteral("Add Folder"), panel);
    m_convertSelectedButton = new QPushButton(QStringLiteral("Convert Selected"), panel);
    m_convertAllButton = new QPushButton(QStringLiteral("Convert All"), panel);
    m_convertAllButton->setObjectName(QStringLiteral("PrimaryButton"));
    configurePanelButton(m_addFilesButton, 112);
    configurePanelButton(m_addFolderButton, 116);
    configurePanelButton(m_convertSelectedButton, 150);
    configurePanelButton(m_convertAllButton, 120);

    actions->addWidget(m_addFilesButton);
    actions->addWidget(m_addFolderButton);
    actions->addStretch(1);
    actions->addWidget(m_convertSelectedButton);
    actions->addWidget(m_convertAllButton);

    m_progress = new QProgressBar(panel);
    m_progress->setRange(0, 100);
    m_progress->setValue(0);
    m_progress->setFormat(QStringLiteral("%p%"));
    m_progress->setFixedHeight(24);

    m_log = new LogWidget(panel);
    m_log->appendInfo(QStringLiteral("DarBLP Converter ready."));

    layout->addLayout(actions);
    layout->addWidget(m_progress);
    layout->addWidget(m_log);

    connect(m_addFilesButton, &QPushButton::clicked, this, &MainWindow::addFiles);
    connect(m_addFolderButton, &QPushButton::clicked, this, &MainWindow::addFolder);
    connect(m_convertSelectedButton, &QPushButton::clicked, this, &MainWindow::convertSelected);
    connect(m_convertAllButton, &QPushButton::clicked, this, &MainWindow::convertAll);

    return panel;
}

QLabel *MainWindow::createValueLabel() const
{
    auto *label = new QLabel(QStringLiteral("-"));
    label->setObjectName(QStringLiteral("Muted"));
    label->setMinimumWidth(130);
    label->setWordWrap(true);
    label->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Preferred);
    label->setTextInteractionFlags(Qt::TextSelectableByMouse);
    return label;
}

void MainWindow::addInputPaths(const QStringList &paths)
{
    QStringList files;
    for (const QString &path : paths) {
        const QFileInfo info(path);
        if (info.isDir()) {
            if (m_sourceRoot.isEmpty()) {
                m_sourceRoot = info.absoluteFilePath();
            }
            files.append(FileUtils::collectSupportedFiles(info.absoluteFilePath()));
        } else {
            files.append(FileUtils::collectSupportedFiles(path));
        }
    }

    files.removeDuplicates();
    if (files.isEmpty()) {
        m_log->appendError(QStringLiteral("No supported textures found."));
        return;
    }

    const int before = m_fileModel->rowCount();
    m_fileModel->addFiles(files);
    const int added = m_fileModel->rowCount() - before;
    m_log->appendInfo(QStringLiteral("Added %1 file(s).").arg(added));

    if (!m_fileView->currentIndex().isValid() && m_fileModel->rowCount() > 0) {
        m_fileView->setCurrentIndex(m_fileModel->index(0, 0));
    }
}

void MainWindow::updateMetadataForPath(const QString &path)
{
    clearMetadata();

    const QFileInfo info(path);
    m_fileSizeValue->setText(FileUtils::prettyFileSize(info.size()));

    if (info.suffix().compare(QStringLiteral("blp"), Qt::CaseInsensitive) == 0) {
        BlpHeaderInfo header;
        QString error;
        if (BlpReader::readHeader(path, header, &error)) {
            m_formatValue->setText(header.version);
            m_widthValue->setText(QString::number(header.width));
            m_heightValue->setText(QString::number(header.height));
            m_mipmapsValue->setText(QString::number(header.mipmapCount));
            m_alphaDepthValue->setText(QString::number(header.alphaDepth));
            m_compressionValue->setText(header.compression);
        } else {
            m_formatValue->setText(QStringLiteral("BLP"));
            m_compressionValue->setText(error);
        }
        m_preview->clearImage();
        return;
    }

    Converter converter;
    ImageData image;
    QString error;
    if (!converter.loadImage(path, image, &error)) {
        m_formatValue->setText(info.suffix().toUpper());
        m_compressionValue->setText(error);
        m_preview->clearImage();
        return;
    }

    m_formatValue->setText(image.format);
    m_widthValue->setText(QString::number(image.width()));
    m_heightValue->setText(QString::number(image.height()));
    m_mipmapsValue->setText(QString::number(image.mipmapCount));
    m_alphaDepthValue->setText(QString::number(image.alphaDepth));
    m_compressionValue->setText(image.compression);
    m_preview->setImage(image.image);
    m_fileModel->setThumbnail(path, image.image);
}

void MainWindow::clearMetadata()
{
    const QList<QLabel *> labels = {
        m_formatValue,
        m_widthValue,
        m_heightValue,
        m_mipmapsValue,
        m_alphaDepthValue,
        m_compressionValue,
        m_fileSizeValue,
    };

    for (QLabel *label : labels) {
        if (label) {
            label->setText(QStringLiteral("-"));
        }
    }
}

void MainWindow::startConversion(const QStringList &files)
{
    if (m_conversionRunning) {
        return;
    }

    if (files.isEmpty()) {
        m_log->appendError(QStringLiteral("No files selected for conversion."));
        return;
    }

    m_conversionRunning = true;
    setControlsEnabled(false);
    m_progress->setRange(0, files.size());
    m_progress->setValue(0);
    m_progress->setFormat(QStringLiteral("%v / %m"));

    const ConversionOptions options = currentOptions();
    auto *thread = new QThread(this);
    auto *worker = new ConversionWorker;
    worker->moveToThread(thread);

    connect(thread, &QThread::started, worker, [worker, files, options]() {
        worker->convert(files, options);
    });

    connect(worker, &ConversionWorker::fileStarted, this, [this](const QString &path, int index, int total) {
        Q_UNUSED(index)
        Q_UNUSED(total)
        m_fileModel->setStatus(path, FileStatus::Converting);
    });

    connect(worker, &ConversionWorker::fileFinished, this, [this](const ConversionResult &result, int index, int total) {
        Q_UNUSED(total)
        m_progress->setValue(index + 1);
        if (result.success) {
            m_fileModel->setStatus(result.inputPath, FileStatus::Converted, result.outputPath);
            m_log->appendSuccess(QStringLiteral("%1 -> %2").arg(QFileInfo(result.inputPath).fileName(),
                                                                 result.outputPath));
        } else {
            m_fileModel->setStatus(result.inputPath, FileStatus::Error, result.errorMessage);
            m_log->appendError(QStringLiteral("%1: %2").arg(QFileInfo(result.inputPath).fileName(),
                                                            result.errorMessage));
        }
    });

    connect(worker, &ConversionWorker::finished, this, [this, thread](int successCount, int errorCount, const QString &outputFolder) {
        m_conversionRunning = false;
        setControlsEnabled(true);
        m_progress->setFormat(QStringLiteral("Done"));
        const QString target = outputFolder.isEmpty() ? QStringLiteral("source folders") : outputFolder;
        m_log->appendInfo(QStringLiteral("Finished: %1 success, %2 error(s), output: %3")
                              .arg(successCount)
                              .arg(errorCount)
                              .arg(target));
        thread->quit();
    });

    connect(thread, &QThread::finished, worker, &QObject::deleteLater);
    connect(thread, &QThread::finished, thread, &QObject::deleteLater);

    thread->start();
}

void MainWindow::setControlsEnabled(bool enabled)
{
    m_addFilesButton->setEnabled(enabled);
    m_addFolderButton->setEnabled(enabled);
    m_convertSelectedButton->setEnabled(enabled);
    m_convertAllButton->setEnabled(enabled);
    m_outputFormatCombo->setEnabled(enabled);
    m_compressionCombo->setEnabled(enabled);
    m_generateMipmapsCheck->setEnabled(enabled);
    m_preserveAlphaCheck->setEnabled(enabled);
    m_resizePowerOfTwoCheck->setEnabled(enabled);
    m_keepFolderStructureCheck->setEnabled(enabled);
    m_outputFolderEdit->setEnabled(enabled);
}

QStringList MainWindow::selectedPaths() const
{
    QStringList paths;
    const QModelIndexList rows = m_fileView->selectionModel()->selectedRows();
    for (const QModelIndex &index : rows) {
        paths.push_back(m_fileModel->entryAt(index.row()).path);
    }
    return paths;
}

ConversionOptions MainWindow::currentOptions() const
{
    ConversionOptions options;
    options.outputFormat = static_cast<OutputFormat>(m_outputFormatCombo->currentData().toInt());
    options.compression = static_cast<BlpCompression>(m_compressionCombo->currentData().toInt());
    options.generateMipmaps = m_generateMipmapsCheck->isChecked();
    options.preserveAlpha = m_preserveAlphaCheck->isChecked();
    options.resizeToPowerOfTwo = m_resizePowerOfTwoCheck->isChecked();
    options.keepFolderStructure = m_keepFolderStructureCheck->isChecked();
    options.outputFolder = m_outputFolderEdit->text().trimmed();
    options.sourceRoot = m_sourceRoot;
    return options;
}

} // namespace darblp::gui
