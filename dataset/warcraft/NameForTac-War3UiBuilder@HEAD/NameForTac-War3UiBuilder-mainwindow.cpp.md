---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "mainwindow.cpp"
ref: HEAD
---

#include "mainwindow.h"
#include "canvas/uiscene.h"
#include "canvas/uiview.h"
#include "elements/uielementdata.h"
#include "panels/treepanel.h"
#include "panels/propertypanel.h"
#include "panels/texturepanel.h"
#include "panels/searchreplacedialog.h"
#include "managers/projectmanager.h"
#include "managers/undomanager.h"

#include <QMenuBar>
#include <QToolBar>
#include <QStatusBar>
#include <QDockWidget>
#include <QFileDialog>
#include <QInputDialog>
#include <QMessageBox>
#include <QAction>
#include <QApplication>
#include <QLabel>
#include <QStyle>
#include <QSettings>
#include <QFileInfo>

// ── Dark theme stylesheet ──
static const char *DARK_STYLESHEET = R"(
    /* Base */
    QMainWindow, QWidget { background-color: #1a1b26; color: #c0caf5; }

    /* ── Menu Bar ── */
    QMenuBar {
        background-color: #16161e;
        border-bottom: 1px solid #32334a;
        padding: 2px 0;
    }
    QMenuBar::item {
        padding: 4px 12px;
        background: transparent;
        border-radius: 4px;
        margin: 2px 2px;
    }
    QMenuBar::item:selected { background: #3b3d5c; }
    QMenu {
        background-color: #1e1f2e;
        border: 1px solid #32334a;
        border-radius: 6px;
        padding: 4px;
    }
    QMenu::item {
        padding: 6px 24px;
        border-radius: 4px;
    }
    QMenu::item:selected { background: #3b3d5c; }
    QMenu::separator {
        height: 1px;
        background: #32334a;
        margin: 4px 8px;
    }

    /* ── Toolbar ── */
    QToolBar {
        background-color: #1e1f2e;
        border-bottom: 1px solid #32334a;
        padding: 4px 8px;
        spacing: 4px;
    }
    QToolBar QToolButton {
        background: transparent;
        border: 1px solid transparent;
        border-radius: 4px;
        padding: 4px 10px;
        color: #c0caf5;
        font-size: 12px;
    }
    QToolBar QToolButton:hover {
        background: #3b3d5c;
        border-color: #45476a;
    }
    QToolBar QToolButton:pressed {
        background: #45476a;
    }
    QToolBar QToolButton:disabled { color: #565f89; }

    /* ── Dock Widget ── */
    QDockWidget {
        titlebar-close-icon: none;
        titlebar-normal-icon: none;
    }
    QDockWidget::title {
        background-color: #1e1f2e;
        padding: 8px 12px;
        border-bottom: 1px solid #32334a;
        font-weight: 600;
        font-size: 12px;
        color: #a9b1d6;
    }

    /* ── Tree Widget (element tree panel) ── */
    QTreeWidget {
        background-color: #1a1b26;
        border: none;
        outline: none;
        font-size: 12px;
    }
    QTreeWidget::item {
        padding: 4px 6px;
        border-bottom: 1px solid #232433;
    }
    QTreeWidget::item:selected {
        background-color: #3b3d5c;
        color: #c0caf5;
    }
    QTreeWidget::item:hover {
        background-color: #2a2b3e;
    }
    QHeaderView::section {
        background-color: #1e1f2e;
        color: #a9b1d6;
        border: none;
        border-bottom: 1px solid #32334a;
        padding: 4px 8px;
        font-weight: 600;
    }

    /* ── Group Box (property panel) ── */
    QGroupBox {
        background-color: #1e1f2e;
        border: 1px solid #32334a;
        border-radius: 6px;
        margin-top: 10px;
        padding: 16px 12px 12px 12px;
        font-weight: 600;
        font-size: 12px;
        color: #a9b1d6;
    }
    QGroupBox::title {
        subcontrol-origin: margin;
        left: 12px;
        padding: 0 4px;
        color: #7aa2f7;
    }
    QGroupBox:checked { /* checkable group box */
        border-color: #45476a;
    }

    /* ── Input fields ── */
    QLineEdit, QDoubleSpinBox, QSpinBox {
        background-color: #24253a;
        border: 1px solid #32334a;
        border-radius: 4px;
        padding: 4px 8px;
        color: #c0caf5;
        font-size: 12px;
        min-height: 20px;
    }
    QLineEdit:focus, QDoubleSpinBox:focus, QSpinBox:focus {
        border-color: #7aa2f7;
    }
    QLineEdit:disabled, QDoubleSpinBox:disabled, QSpinBox:disabled {
        background-color: #1a1b26;
        color: #565f89;
    }

    /* ── Combo Box ── */
    QComboBox {
        background-color: #24253a;
        border: 1px solid #32334a;
        border-radius: 4px;
        padding: 4px 8px;
        color: #c0caf5;
        font-size: 12px;
        min-height: 20px;
    }
    QComboBox:focus { border-color: #7aa2f7; }
    QComboBox::drop-down {
        border: none;
        width: 20px;
    }
    QComboBox QAbstractItemView {
        background-color: #1e1f2e;
        border: 1px solid #32334a;
        selection-background-color: #3b3d5c;
        color: #c0caf5;
    }

    /* ── Buttons ── */
    QPushButton {
        background-color: #3b3d5c;
        border: 1px solid #45476a;
        border-radius: 4px;
        padding: 4px 12px;
        color: #c0caf5;
        font-size: 12px;
        min-height: 20px;
    }
    QPushButton:hover {
        background-color: #45476a;
        border-color: #565f89;
    }
    QPushButton:pressed {
        background-color: #565f89;
    }

    /* ── Table Widget ── */
    QTableWidget {
        background-color: #1a1b26;
        border: 1px solid #32334a;
        border-radius: 4px;
        font-size: 11px;
        gridline-color: #232433;
    }
    QTableWidget::item:selected {
        background-color: #3b3d5c;
    }
    QHeaderView::section {
        background-color: #1e1f2e;
        color: #a9b1d6;
        border: none;
        border-bottom: 1px solid #32334a;
        padding: 4px;
        font-weight: 600;
    }

    /* ── Text Edit (War3 coords) ── */
    QTextEdit {
        background-color: #1a1b26;
        border: 1px solid #32334a;
        border-radius: 4px;
        color: #c0caf5;
        font-size: 12px;
        padding: 4px;
    }

    /* ── Status Bar ── */
    QStatusBar {
        background-color: #1e1f2e;
        border-top: 1px solid #32334a;
        color: #565f89;
        font-size: 12px;
    }

    /* ── Scroll Bars ── */
    QScrollBar:vertical {
        background: #1a1b26;
        width: 10px;
        margin: 0;
    }
    QScrollBar::handle:vertical {
        background: #32334a;
        border-radius: 5px;
        min-height: 30px;
    }
    QScrollBar::handle:vertical:hover { background: #45476a; }
    QScrollBar::add-line:vertical, QScrollBar::sub-line:vertical { height: 0; }
    QScrollBar:horizontal {
        background: #1a1b26;
        height: 10px;
        margin: 0;
    }
    QScrollBar::handle:horizontal {
        background: #32334a;
        border-radius: 5px;
        min-width: 30px;
    }
    QScrollBar::handle:horizontal:hover { background: #45476a; }
    QScrollBar::add-line:horizontal, QScrollBar::sub-line:horizontal { width: 0; }

    /* ── Tool Tips ── */
    QToolTip {
        background-color: #1e1f2e;
        border: 1px solid #45476a;
        border-radius: 4px;
        padding: 4px 8px;
        color: #c0caf5;
        font-size: 12px;
    }
)";

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent)
{
    setWindowTitle("War3UiBuilder - Warcraft III UI Designer");
    resize(1280, 800);

    setStyleSheet(DARK_STYLESHEET);

    m_scene = new UiScene(this);
    m_view = new UiView(m_scene, this);
    m_treePanel = new TreePanel(this);
    m_propertyPanel = new PropertyPanel(this);
    m_texturePanel = new TexturePanel(this);
    m_projectManager = new ProjectManager(this);
    m_undoManager = new UndoManager(this);

    setupMenuBar();
    setupToolBar();
    setupCentralWidget();
    setupDockPanels();
    connectSignals();

    statusBar()->showMessage(tr("Ready"));

    // Zoom indicator in status bar
    m_zoomLabel = new QLabel(tr("100%"), this);
    m_zoomLabel->setMinimumWidth(60);
    m_zoomLabel->setAlignment(Qt::AlignCenter);
    statusBar()->addPermanentWidget(m_zoomLabel);
    connect(m_view, &UiView::zoomChanged, this, [this](double factor) {
        m_zoomLabel->setText(QString("%1%").arg(qRound(factor * 100.0)));
    });
}

MainWindow::~MainWindow() = default;

void MainWindow::setupMenuBar()
{
    // File menu
    QMenu *fileMenu = menuBar()->addMenu(tr("&File"));

    QAction *newAct = fileMenu->addAction(tr("&New Project"), this, &MainWindow::onNewProject);
    newAct->setShortcut(QKeySequence::New);

    QAction *openAct = fileMenu->addAction(tr("&Open Project..."), this, &MainWindow::onOpenProject);
    openAct->setShortcut(QKeySequence::Open);

    QAction *saveAct = fileMenu->addAction(tr("&Save Project"), this, &MainWindow::onSaveProject);
    saveAct->setShortcut(QKeySequence::Save);

    QAction *exportAct = fileMenu->addAction(tr("&Export INI..."), this, &MainWindow::onExportIni);
    exportAct->setShortcut(QKeySequence("Ctrl+E"));

    QAction *exportFdfAct = fileMenu->addAction(tr("Export F&DF..."), this, &MainWindow::onExportFdf);
    exportFdfAct->setShortcut(QKeySequence("Ctrl+Shift+E"));

    fileMenu->addAction(tr("&Import FDF..."), this, &MainWindow::onImportFdf);

    fileMenu->addSeparator();

    QAction *exitAct = fileMenu->addAction(tr("E&xit"), this, &QWidget::close);
    exitAct->setShortcut(QKeySequence::Quit);

    // Recent files
    fileMenu->addSeparator();
    m_recentMenu = fileMenu->addMenu(tr("Recent Files"));
    m_recentFiles = recentFiles();
    updateRecentFilesMenu();

    // Edit menu
    QMenu *editMenu = menuBar()->addMenu(tr("&Edit"));

    QAction *undoAct = editMenu->addAction(tr("&Undo"), this, &MainWindow::onUndo);
    undoAct->setShortcut(QKeySequence::Undo);

    QAction *redoAct = editMenu->addAction(tr("&Redo"), this, &MainWindow::onRedo);
    redoAct->setShortcut(QKeySequence("Ctrl+Shift+Z"));
    // Additional redo shortcut
    QAction *redoAlt = new QAction(tr("&Redo"), this);
    redoAlt->setShortcut(QKeySequence("Ctrl+Y"));
    connect(redoAlt, &QAction::triggered, this, &MainWindow::onRedo);
    addAction(redoAlt);

    editMenu->addSeparator();

    QAction *copyAct = editMenu->addAction(tr("&Copy"), this, &MainWindow::onCopy);
    copyAct->setShortcut(QKeySequence::Copy);

    QAction *cutAct = editMenu->addAction(tr("Cu&t"), this, &MainWindow::onCut);
    cutAct->setShortcut(QKeySequence::Cut);

    QAction *pasteAct = editMenu->addAction(tr("&Paste"), this, &MainWindow::onPaste);
    pasteAct->setShortcut(QKeySequence::Paste);

    editMenu->addSeparator();

    QAction *deleteAct = editMenu->addAction(tr("&Delete Selected"), this, &MainWindow::onDeleteSelected);
    deleteAct->setShortcut(QKeySequence::Delete);

    editMenu->addSeparator();

    editMenu->addAction(tr("Search &Replace..."), QKeySequence("Ctrl+H"), this, &MainWindow::onSearchReplace);

    editMenu->addSeparator();
    QMenu *alignMenu = editMenu->addMenu(tr("&Align"));
    alignMenu->addAction(tr("Left"), QKeySequence("Ctrl+Shift+L"), this, &MainWindow::onAlignLeft);
    alignMenu->addAction(tr("Right"), QKeySequence("Ctrl+Shift+R"), this, &MainWindow::onAlignRight);
    alignMenu->addAction(tr("Top"), QKeySequence("Ctrl+Shift+T"), this, &MainWindow::onAlignTop);
    alignMenu->addAction(tr("Bottom"), QKeySequence("Ctrl+Shift+B"), this, &MainWindow::onAlignBottom);
    alignMenu->addAction(tr("Center H"), QKeySequence("Ctrl+Shift+H"), this, &MainWindow::onAlignCenterH);
    alignMenu->addAction(tr("Center V"), QKeySequence("Ctrl+Shift+V"), this, &MainWindow::onAlignCenterV);
    alignMenu->addSeparator();
    alignMenu->addAction(tr("Distribute H"), QKeySequence("Ctrl+Shift+1"), this, &MainWindow::onDistributeH);
    alignMenu->addAction(tr("Distribute V"), QKeySequence("Ctrl+Shift+2"), this, &MainWindow::onDistributeV);

    // View menu
    QMenu *viewMenu = menuBar()->addMenu(tr("&View"));
    viewMenu->addAction(tr("Zoom &In"), QKeySequence::ZoomIn, m_view, &UiView::zoomIn);
    viewMenu->addAction(tr("Zoom &Out"), QKeySequence::ZoomOut, m_view, &UiView::zoomOut);
    viewMenu->addAction(tr("&Fit All"), QKeySequence("Ctrl+0"), m_view, &UiView::fitAll);
    viewMenu->addSeparator();
    viewMenu->addAction(tr("Grid Snap &Size..."), this, &MainWindow::onGridSnap);
    viewMenu->addSeparator();

    QAction *showRulersAct = viewMenu->addAction(tr("Show &Rulers"));
    showRulersAct->setCheckable(true);
    showRulersAct->setChecked(m_view->showRulers());
    connect(showRulersAct, &QAction::toggled, m_view, &UiView::setShowRulers);

    viewMenu->addAction(tr("&Clear Guides"), m_scene, &UiScene::clearGuides);

    // Help menu
    QMenu *helpMenu = menuBar()->addMenu(tr("&Help"));
    helpMenu->addAction(tr("&About"), this, &MainWindow::onAbout);
}

void MainWindow::setupToolBar()
{
    QToolBar *toolbar = addToolBar(tr("Main"));
    toolbar->setIconSize(QSize(24, 24));
    toolbar->setToolButtonStyle(Qt::ToolButtonTextBesideIcon);

    auto *newAct = toolbar->addAction(
        style()->standardIcon(QStyle::SP_FileIcon),
        tr("New"), this, &MainWindow::onNewProject);
    newAct->setToolTip(tr("New Project (Ctrl+N)"));

    auto *openAct = toolbar->addAction(
        style()->standardIcon(QStyle::SP_DialogOpenButton),
        tr("Open"), this, &MainWindow::onOpenProject);
    openAct->setToolTip(tr("Open Project (Ctrl+O)"));

    auto *saveAct = toolbar->addAction(
        style()->standardIcon(QStyle::SP_DialogSaveButton),
        tr("Save"), this, &MainWindow::onSaveProject);
    saveAct->setToolTip(tr("Save Project (Ctrl+S)"));

    toolbar->addSeparator();

    auto *importAct = toolbar->addAction(
        style()->standardIcon(QStyle::SP_FileDialogNewFolder),
        tr("Import"), this, &MainWindow::onImportImage);
    importAct->setToolTip(tr("Import images as UI elements"));

    auto *exportAct = toolbar->addAction(
        style()->standardIcon(QStyle::SP_DialogSaveButton),
        tr("Export INI"), this, &MainWindow::onExportIni);
    exportAct->setToolTip(tr("Export to Warcraft III INI format (Ctrl+E)"));

    auto *exportFdfAct = toolbar->addAction(
        style()->standardIcon(QStyle::SP_FileDialogDetailedView),
        tr("Export FDF"), this, &MainWindow::onExportFdf);
    exportFdfAct->setToolTip(tr("Export to Warcraft III FDF format (Ctrl+Shift+E)"));

    toolbar->addSeparator();

    auto *delAct = toolbar->addAction(
        style()->standardIcon(QStyle::SP_TrashIcon),
        tr("Delete"), this, &MainWindow::onDeleteSelected);
    delAct->setToolTip(tr("Delete selected element (Delete)"));
}

void MainWindow::setupCentralWidget()
{
    setCentralWidget(m_view);
}

void MainWindow::setupDockPanels()
{
    // Left dock - Tree panel
    QDockWidget *treeDock = new QDockWidget(tr("Element Tree"), this);
    treeDock->setWidget(m_treePanel);
    treeDock->setMinimumWidth(220);
    addDockWidget(Qt::LeftDockWidgetArea, treeDock);

    // Right dock - Property panel
    QDockWidget *propDock = new QDockWidget(tr("Properties"), this);
    propDock->setWidget(m_propertyPanel);
    propDock->setMinimumWidth(280);
    addDockWidget(Qt::RightDockWidgetArea, propDock);

    // Bottom-right dock - Texture browser
    QDockWidget *texDock = new QDockWidget(tr("Texture Browser"), this);
    texDock->setWidget(m_texturePanel);
    texDock->setMinimumWidth(200);
    addDockWidget(Qt::RightDockWidgetArea, texDock);
}

void MainWindow::connectSignals()
{
    // Texture loading failure notification — offer to relocate
    connect(m_scene, &UiScene::textureLoadFailed, this, [this](const QString &path, const QString &reason) {
        statusBar()->showMessage(reason, 10000);
        auto result = QMessageBox::warning(this, tr("Texture Load Failed"),
            tr("Failed to load texture:\n%1\n\n%2\n\nDo you want to locate the file manually?")
                .arg(path, reason),
            QMessageBox::Yes | QMessageBox::No, QMessageBox::No);
        if (result == QMessageBox::Yes) {
            QString newPath = QFileDialog::getOpenFileName(this, tr("Locate Texture"),
                QFileInfo(path).path(),
                tr("Images (*.png *.jpg *.jpeg *.bmp *.tga *.dds);;All Files (*)"));
            if (!newPath.isEmpty() && !m_selectedElementName.isEmpty()) {
                pushUndoState();
                UiElementData data = m_scene->elementDataByName(m_selectedElementName);
                if (data.isValid()) {
                    data.texture = newPath;
                    data.hasTexture = true;
                    m_scene->updateElementProperty(m_selectedElementName, data);
                    refreshPropertyPanel(m_selectedElementName);
                    statusBar()->showMessage(tr("Texture relocated: %1").arg(newPath), 5000);
                }
            }
        }
    });

    // Scene selection -> tree + property panel sync
    connect(m_scene, &UiScene::elementSelected, this, [this](const QString &name) {
        m_selectedElementName = name;
        m_treePanel->selectElement(name);
        // Check for multi-selection -> batch editing mode
        auto selected = m_scene->selectedElementData();
        if (selected.size() > 1) {
            QStringList names;
            for (const auto &d : selected)
                names.append(d.name);
            m_propertyPanel->updateGroupList(m_scene->groups());
            m_propertyPanel->showBatch(names, selected);
        } else {
            refreshPropertyPanel(name);
        }
    });
    connect(m_scene, &UiScene::elementDeselected, this, [this]() {
        m_selectedElementName.clear();
        m_propertyPanel->clearPanel();
    });
    connect(m_scene, &UiScene::elementDataChanged, this, [this]() {
        m_treePanel->refreshTree(m_scene->allElementData(), m_scene->groups());
        if (!m_selectedElementName.isEmpty()) {
            refreshPropertyPanel(m_selectedElementName);
        }
    });
    connect(m_scene, &UiScene::elementCountChanged, this, [this](int) {
        pushUndoState();
    });

    // Canvas context menu actions -> undo before modify
    connect(m_scene, &UiScene::contextActionTriggered, this, [this]() {
        pushUndoState();
    });

    // Canvas drag finished -> undo after drag
    connect(m_scene, &UiScene::elementDragFinished, this, [this]() {
        pushUndoState();
    });

    // Tree selection -> scene + property panel sync
    connect(m_treePanel, &TreePanel::elementSelected, this, [this](const QString &name) {
        m_scene->selectElementByName(name);
        m_selectedElementName = name;
        refreshPropertyPanel(name);
    });

    // Tree visibility/lock toggles -> scene update
    connect(m_treePanel, &TreePanel::visibilityToggled, this, [this](const QString &name, bool visible) {
        pushUndoState();
        UiElementData data = m_scene->elementDataByName(name);
        if (data.isValid()) {
            data.visible = visible;
            m_scene->updateElementProperty(name, data);
        }
    });
    connect(m_treePanel, &TreePanel::lockToggled, this, [this](const QString &name, bool locked) {
        pushUndoState();
        UiElementData data = m_scene->elementDataByName(name);
        if (data.isValid()) {
            data.locked = locked;
            m_scene->updateElementProperty(name, data);
        }
    });

    // Tree panel rename -> scene update
    connect(m_treePanel, &TreePanel::renameRequested, this, [this](const QString &oldName, const QString &newName) {
        pushUndoState();
        m_scene->renameElement(oldName, newName);
    });

    // Tree panel delete -> scene delete
    connect(m_treePanel, &TreePanel::deleteRequested, this, [this](const QString &name) {
        pushUndoState();
        m_scene->deleteElement(name);
        m_propertyPanel->clearPanel();
        m_treePanel->refreshTree(m_scene->allElementData());
    });

    // Tree panel drag-drop parent change -> scene update with cycle detection
    connect(m_treePanel, &TreePanel::parentChanged, this, [this](const QString &name, const QString &newParent) {
        pushUndoState();
        if (!m_scene->setElementParent(name, newParent)) {
            // Failed (cycle/self-parent) — rebuild tree to undo the visual move
            m_treePanel->refreshTree(m_scene->allElementData());
        }
    });

    // Tree panel group operations -> scene
    connect(m_treePanel, &TreePanel::groupCreated, m_scene, &UiScene::createGroup);
    connect(m_treePanel, &TreePanel::groupDeleted, m_scene, &UiScene::deleteGroup);
    connect(m_treePanel, &TreePanel::groupRenamed, m_scene, &UiScene::renameGroup);
    connect(m_treePanel, &TreePanel::groupAssignmentChanged, this, [this](const QString &elName, const QString &groupName) {
        pushUndoState();
        if (groupName.isEmpty())
            m_scene->removeFromGroup(elName);
        else
            m_scene->addToGroup(elName, groupName);
    });

    // Texture selection -> apply to selected element
    connect(m_texturePanel, &TexturePanel::textureSelected, this, [this](const QString &texturePath) {
        if (m_selectedElementName.isEmpty()) return;
        pushUndoState();
        UiElementData data = m_scene->elementDataByName(m_selectedElementName);
        if (data.isValid()) {
            data.texture = texturePath;
            data.hasTexture = true;
            m_scene->updateElementProperty(m_selectedElementName, data);
            refreshPropertyPanel(m_selectedElementName);
        }
    });

    // Property changes -> scene update
    connect(m_propertyPanel, &PropertyPanel::propertyChanged, this,
            [this](const QString &name, const UiElementData &data) {
        pushUndoState();
        m_scene->updateElementProperty(name, data);
        // If element was renamed, update selection tracking
        if (name != data.name) {
            m_selectedElementName = data.name;
        }
        m_treePanel->refreshTree(m_scene->allElementData());
    });

    // Batch property changes -> update all selected elements
    connect(m_propertyPanel, &PropertyPanel::batchPropertyChanged, this,
            [this](const QStringList &names, const UiElementData &data) {
        pushUndoState();
        for (const QString &name : names) {
            m_scene->updateElementProperty(name, data);
        }
        m_treePanel->refreshTree(m_scene->allElementData());
    });
}

void MainWindow::onNewProject()
{
    m_projectManager->newProject();
    m_scene->clearAll();
    m_treePanel->clearPanel();
    m_propertyPanel->clearPanel();
    m_texturePanel->clearPanel();
    m_undoManager->clear();
    m_projectManager->setGroups({});
    statusBar()->showMessage(tr("New project created"));
}

void MainWindow::onOpenProject()
{
    QString path = QFileDialog::getOpenFileName(this, tr("Open Project"),
                                                  QString(), tr("War3UI Project (*.wui)"));
    if (path.isEmpty()) return;

    QString errorMsg;
    if (m_projectManager->loadProject(path, &errorMsg)) {
        m_scene->loadFromData(m_projectManager->elements());
        m_scene->loadGroups(m_projectManager->groups());
        m_treePanel->rebuildTree(m_projectManager->elements(), m_projectManager->groups());
        refreshTexturePanel();
        statusBar()->showMessage(tr("Project loaded: ") + path);
        addRecentFile(path);
        if (!errorMsg.isEmpty()) {
            statusBar()->showMessage(errorMsg, 8000);
        }
    } else {
        QMessageBox::warning(this, tr("Load Error"),
            tr("Failed to load project file.\n\n%1").arg(errorMsg));
    }
}

void MainWindow::onSaveProject()
{
    QString path = QFileDialog::getSaveFileName(this, tr("Save Project"),
                                                  QString(), tr("War3UI Project (*.wui)"));
    if (path.isEmpty()) return;

    m_projectManager->setElements(m_scene->allElementData());
    m_projectManager->setGroups(m_scene->groups());
    QString errorMsg;
    if (m_projectManager->saveProject(path, &errorMsg)) {
        statusBar()->showMessage(tr("Project saved: ") + path);
        addRecentFile(path);
    } else {
        QMessageBox::warning(this, tr("Save Error"),
            tr("Failed to save project.\n\n%1").arg(errorMsg));
    }
}

void MainWindow::onExportIni()
{
    QStringList modes;
    modes << tr("1920×1080 像素坐标 (相对父级)")
          << tr("War3 归一化坐标 (相对父级)");
    bool ok;
    QString mode = QInputDialog::getItem(this, tr("选择导出坐标模式"),
                                           tr("坐标模式:"), modes, 0, false, &ok);
    if (!ok) return;
    bool war3Mode = (mode == modes[1]);

    QString path = QFileDialog::getSaveFileName(this, tr("Export INI"),
                                                  QString(), tr("INI File (*.ini)"));
    if (path.isEmpty()) return;

    m_projectManager->setElements(m_scene->allElementData());
    QString errorMsg;
    if (m_projectManager->exportIni(path, war3Mode, &errorMsg)) {
        statusBar()->showMessage(tr("INI exported: ") + path);
        QMessageBox::information(this, tr("Success"), tr("INI file exported successfully."));
    } else {
        QMessageBox::warning(this, tr("Export Error"),
            tr("Failed to export INI.\n\n%1").arg(errorMsg));
    }
}

void MainWindow::onExportFdf()
{
    QStringList modes;
    modes << tr("1920×1080 像素坐标 (相对父级)")
          << tr("War3 归一化坐标 (相对父级)");
    bool ok;
    QString mode = QInputDialog::getItem(this, tr("选择导出坐标模式"),
                                           tr("坐标模式:"), modes, 0, false, &ok);
    if (!ok) return;
    bool war3Mode = (mode == modes[1]);

    QString path = QFileDialog::getSaveFileName(this, tr("Export FDF"),
                                                  QString(), tr("FDF File (*.fdf)"));
    if (path.isEmpty()) return;

    m_projectManager->setElements(m_scene->allElementData());
    QString errorMsg;
    if (m_projectManager->exportFdf(path, war3Mode, &errorMsg)) {
        statusBar()->showMessage(tr("FDF exported: ") + path);
        QMessageBox::information(this, tr("Success"), tr("FDF file exported successfully."));
    } else {
        QMessageBox::warning(this, tr("Export Error"),
            tr("Failed to export FDF.\n\n%1").arg(errorMsg));
    }
}

void MainWindow::onImportFdf()
{
    QString path = QFileDialog::getOpenFileName(this, tr("Import FDF"),
                                                  QString(), tr("FDF File (*.fdf)"));
    if (path.isEmpty()) return;

    QString errorMsg;
    auto elements = m_projectManager->importFdf(path, &errorMsg);
    if (elements.isEmpty()) {
        QString msg = errorMsg.isEmpty() ? tr("No elements found in FDF file.") : errorMsg;
        QMessageBox::warning(this, tr("Import Error"), msg);
        return;
    }

    pushUndoState();
    for (const auto &el : elements) {
        m_scene->addElementFromData(el);
    }
    m_treePanel->rebuildTree(m_scene->allElementData(), m_scene->groups());
    statusBar()->showMessage(tr("Imported %1 elements from FDF").arg(elements.size()));
}

void MainWindow::onImportImage()
{
    QStringList paths = QFileDialog::getOpenFileNames(this, tr("Import Images"),
                                                       QString(),
                                                       tr("Images (*.png *.jpg *.jpeg *.bmp *.tga *.dds);;All Files (*)"));
    if (paths.isEmpty()) return;

    m_projectManager->setElements(m_scene->allElementData());
    QString projectDir = QFileDialog::getExistingDirectory(this, tr("Select project directory for images"));
    if (projectDir.isEmpty()) return;

    auto result = m_projectManager->importImages(paths, projectDir);
    for (const QString &texture : result.imported) {
        m_scene->addElementFromTexture(texture);
    }
    refreshTexturePanel();

    if (!result.failed.isEmpty()) {
        QStringList shortNames;
        for (const auto &f : result.failed)
            shortNames << QFileInfo(f).fileName();
        statusBar()->showMessage(
            tr("Imported %1 images, %2 failed").arg(result.imported.size()).arg(result.failed.size()), 8000);
        QMessageBox::warning(this, tr("Import Results"),
            tr("Successfully imported: %1\n\nFailed to import: %2\n\n%3")
                .arg(result.imported.size())
                .arg(result.failed.size())
                .arg(shortNames.join("\n")));
    } else {
        statusBar()->showMessage(tr("Imported %1 images").arg(result.imported.size()));
    }
}

void MainWindow::onDeleteSelected()
{
    pushUndoState();
    m_scene->deleteSelected();
    m_propertyPanel->clearPanel();
    m_treePanel->refreshTree(m_scene->allElementData());
}

void MainWindow::onCopy()
{
    m_clipboard = m_scene->selectedElementData();
    statusBar()->showMessage(tr("Copied %1 element(s)").arg(m_clipboard.size()));
}

void MainWindow::onCut()
{
    m_clipboard = m_scene->selectedElementData();
    if (!m_clipboard.isEmpty()) {
        pushUndoState();
        m_scene->deleteSelected();
        m_propertyPanel->clearPanel();
        m_treePanel->refreshTree(m_scene->allElementData());
        statusBar()->showMessage(tr("Cut %1 element(s)").arg(m_clipboard.size()));
    }
}

void MainWindow::onPaste()
{
    if (m_clipboard.isEmpty()) return;
    pushUndoState();
    for (const auto &data : m_clipboard) {
        UiElementData copy = data;
        copy.name = m_scene->uniqueName(data.name);
        copy.x += 20.0;
        copy.y += 20.0;
        copy.parent.clear();  // paste as root to avoid broken parent refs
        m_scene->addElementFromData(copy);
    }
    m_treePanel->refreshTree(m_scene->allElementData());
    statusBar()->showMessage(tr("Pasted %1 element(s)").arg(m_clipboard.size()));
}

void MainWindow::onGridSnap()
{
    bool ok;
    int size = QInputDialog::getInt(this, tr("Grid Snap Size"),
                                      tr("Snap grid size (pixels, 1 = off):"),
                                      m_scene->snapGridSize(), 1, 256, 1, &ok);
    if (ok) {
        m_scene->setSnapGridSize(size);
        statusBar()->showMessage(tr("Grid snap set to %1 px").arg(size));
    }
}

void MainWindow::onUndo()
{
    if (!m_undoManager->canUndo()) return;
    auto elements = m_undoManager->undo();
    m_scene->loadFromData(elements);
}

void MainWindow::onRedo()
{
    if (!m_undoManager->canRedo()) return;
    auto elements = m_undoManager->redo();
    m_scene->loadFromData(elements);
}

void MainWindow::pushUndoState()
{
    m_undoManager->pushState(m_scene->allElementData());
}

QStringList MainWindow::recentFiles() const
{
    QSettings settings;
    return settings.value("recentFiles").toStringList();
}

void MainWindow::addRecentFile(const QString &path)
{
    QStringList files = recentFiles();
    files.removeAll(path);
    files.prepend(path);
    while (files.size() > MAX_RECENT_FILES)
        files.removeLast();
    QSettings settings;
    settings.setValue("recentFiles", files);
    m_recentFiles = files;
    updateRecentFilesMenu();
}

void MainWindow::updateRecentFilesMenu()
{
    m_recentMenu->clear();
    if (m_recentFiles.isEmpty()) {
        m_recentMenu->addAction(tr("(No recent files)"))->setEnabled(false);
        return;
    }
    for (int i = 0; i < m_recentFiles.size(); ++i) {
        QString label = QString("&%1 %2").arg(i + 1).arg(m_recentFiles[i]);
        auto *act = m_recentMenu->addAction(label);
        act->setData(m_recentFiles[i]);
        connect(act, &QAction::triggered, this, &MainWindow::openRecentFile);
    }
    m_recentMenu->addSeparator();
    auto *clearAct = m_recentMenu->addAction(tr("Clear Recent Files"));
    connect(clearAct, &QAction::triggered, this, [this]() {
        QSettings settings;
        settings.remove("recentFiles");
        m_recentFiles.clear();
        updateRecentFilesMenu();
    });
}

void MainWindow::openRecentFile()
{
    auto *act = qobject_cast<QAction *>(sender());
    if (!act) return;
    QString path = act->data().toString();
    if (QFileInfo::exists(path)) {
        QString errorMsg;
        if (m_projectManager->loadProject(path, &errorMsg)) {
            m_scene->loadFromData(m_projectManager->elements());
            m_scene->loadGroups(m_projectManager->groups());
            m_treePanel->rebuildTree(m_projectManager->elements(), m_projectManager->groups());
            refreshTexturePanel();
            statusBar()->showMessage(tr("Project loaded: ") + path);
            addRecentFile(path);
            if (!errorMsg.isEmpty()) {
                statusBar()->showMessage(errorMsg, 8000);
            }
        } else {
            QMessageBox::warning(this, tr("Load Error"),
                tr("Failed to load project.\n\n%1").arg(errorMsg));
        }
    } else {
        QMessageBox::warning(this, tr("File Not Found"),
            tr("The file no longer exists:\n%1\n\nIt will be removed from recent files.").arg(path));
        addRecentFile(path); // removes it from the list
    }
}

void MainWindow::refreshPropertyPanel(const QString &name)
{
    UiElementData data = m_scene->elementDataByName(name);
    if (data.isValid()) {
        // If element has a parent, show coordinates relative to the parent
        double ox = 0.0, oy = 0.0;
        if (!data.parent.isEmpty()) {
            UiElementData parentData = m_scene->elementDataByName(data.parent);
            if (parentData.isValid()) {
                ox = parentData.x;
                oy = parentData.y;
            }
        }
        m_propertyPanel->setReferenceOffset(ox, oy);

        QStringList allNames;
        for (const auto &d : m_scene->allElementData()) {
            if (d.name != name) allNames.append(d.name);
        }
        m_propertyPanel->updateParentList(allNames);
        m_propertyPanel->updateGroupList(m_scene->groups());
        m_propertyPanel->showElement(name, data);
    }
}

void MainWindow::refreshTexturePanel()
{
    QString dir = m_projectManager->projectDir();
    if (!dir.isEmpty())
        m_texturePanel->loadTextures(dir + "/textures");
    else
        m_texturePanel->clearPanel();
}

void MainWindow::onAbout()
{
    QMessageBox::about(this, tr("About War3UiBuilder"),
                       tr("War3UiBuilder v1.0\n\n"
                          "A Warcraft III custom UI designer.\n"
                          "Design UI with images and export INI configuration."));
}

// ── Alignment slots ──
void MainWindow::onAlignLeft()      { pushUndoState(); m_scene->alignLeft(); }
void MainWindow::onAlignRight()     { pushUndoState(); m_scene->alignRight(); }
void MainWindow::onAlignTop()       { pushUndoState(); m_scene->alignTop(); }
void MainWindow::onAlignBottom()    { pushUndoState(); m_scene->alignBottom(); }
void MainWindow::onAlignCenterH()   { pushUndoState(); m_scene->alignCenterH(); }
void MainWindow::onAlignCenterV()   { pushUndoState(); m_scene->alignCenterV(); }
void MainWindow::onDistributeH()    { pushUndoState(); m_scene->distributeH(); }
void MainWindow::onDistributeV()    { pushUndoState(); m_scene->distributeV(); }

void MainWindow::onSearchReplace()
{
    auto allData = m_scene->allElementData();
    auto selectedData = m_scene->selectedElementData();
    if (allData.isEmpty()) {
        QMessageBox::information(this, tr("Search & Replace"),
            tr("No elements in the project to search."));
        return;
    }

    SearchReplaceDialog dialog(allData, selectedData, this);
    if (dialog.exec() == QDialog::Accepted || !dialog.appliedReplacements().isEmpty()) {
        const auto &replacements = dialog.appliedReplacements();
        if (replacements.isEmpty()) return;

        pushUndoState();

        // Group replacements by element name for efficient updates
        QMap<QString, UiElementData> modifiedElements;
        for (const auto &r : replacements) {
            if (!modifiedElements.contains(r.elementName))
                modifiedElements[r.elementName] = m_scene->elementDataByName(r.elementName);

            auto &data = modifiedElements[r.elementName];
            if (!data.isValid()) continue;

            if (r.propertyKey.isEmpty()) {
                // Rename element
                m_scene->renameElement(r.elementName, r.newValue);
            } else if (r.propertyKey == "Texture") {
                data.texture = r.newValue;
                data.hasTexture = true;
            } else if (r.propertyKey == "NormalTexture") {
                data.normalTexture = r.newValue;
            } else if (r.propertyKey == "HighlightTexture") {
                data.highlightTexture = r.newValue;
            } else if (r.propertyKey == "ModelPath") {
                data.modelPath = r.newValue;
            } else if (r.propertyKey == "TextContent") {
                data.textContent = r.newValue;
            } else if (r.propertyKey == "FontSize") {
                data.fontSize = r.newValue.toDouble();
            } else if (r.propertyKey == "TextColor") {
                data.textColor = r.newValue;
            } else {
                // Extended property
                data.properties[r.propertyKey] = r.newValue;
            }
        }

        // Write back all modified elements
        for (auto it = modifiedElements.begin(); it != modifiedElements.end(); ++it) {
            if (it.value().isValid())
                m_scene->updateElementProperty(it.key(), it.value());
        }

        m_treePanel->refreshTree(m_scene->allElementData(), m_scene->groups());
        statusBar()->showMessage(tr("Applied %1 replacement(s)").arg(replacements.size()), 5000);
    }
}
