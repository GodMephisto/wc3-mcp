---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "mainwindow.h"
ref: HEAD
---

#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QList>
#include <QLabel>
#include <QMenu>

#include "elements/uielementdata.h"

class UiView;
class UiScene;
class TreePanel;
class PropertyPanel;
class TexturePanel;
class ProjectManager;
class UndoManager;

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget *parent = nullptr);
    ~MainWindow();

private slots:
    void onNewProject();
    void onOpenProject();
    void onSaveProject();
    void onExportIni();
    void onExportFdf();
    void onImportFdf();
    void onImportImage();
    void onDeleteSelected();
    void onCopy();
    void onCut();
    void onPaste();
    void onGridSnap();
    void onUndo();
    void onRedo();
    void onAbout();

    // Alignment
    void onAlignLeft();
    void onAlignRight();
    void onAlignTop();
    void onAlignBottom();
    void onAlignCenterH();
    void onAlignCenterV();
    void onDistributeH();
    void onDistributeV();

    void onSearchReplace();

private:
    void setupMenuBar();
    void setupToolBar();
    void setupDockPanels();
    void setupCentralWidget();
    void connectSignals();

    void refreshPropertyPanel(const QString &name);
    void refreshTexturePanel();

    UiScene *m_scene;
    UiView *m_view;
    TreePanel *m_treePanel;
    PropertyPanel *m_propertyPanel;
    TexturePanel *m_texturePanel;
    ProjectManager *m_projectManager;
    UndoManager *m_undoManager;
    QString m_selectedElementName;
    QList<UiElementData> m_clipboard;
    QLabel *m_zoomLabel;

    void pushUndoState();

    // Recent files
    void updateRecentFilesMenu();
    void openRecentFile();
    QStringList recentFiles() const;
    void addRecentFile(const QString &path);

    QMenu *m_recentMenu;
    QStringList m_recentFiles;
    static constexpr int MAX_RECENT_FILES = 9;
};

#endif // MAINWINDOW_H
