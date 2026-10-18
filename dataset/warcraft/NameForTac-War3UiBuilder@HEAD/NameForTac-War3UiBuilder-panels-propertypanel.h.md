---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "panels/propertypanel.h"
ref: HEAD
---

#ifndef PROPERTYPANEL_H
#define PROPERTYPANEL_H

#include <QWidget>
#include <QLineEdit>
#include <QDoubleSpinBox>
#include <QComboBox>
#include <QGroupBox>
#include <QTableWidget>
#include <QLabel>
#include <QTextEdit>

#include "elements/uielementdata.h"

class PropertyPanel : public QWidget
{
    Q_OBJECT

public:
    explicit PropertyPanel(QWidget *parent = nullptr);
    ~PropertyPanel() = default;

    void showElement(const QString &name, const UiElementData &data);
    void showBatch(const QStringList &names, const QList<UiElementData> &elements);
    void setReferenceOffset(double ox, double oy);
    void updateParentList(const QStringList &parentNames);
    void updateGroupList(const QStringList &groups);
    void clearPanel();

signals:
    void propertyChanged(const QString &name, const UiElementData &data);
    void batchPropertyChanged(const QStringList &names, const UiElementData &data);

private slots:
    void onNameChanged();

private:
    void setupUi();
    void emitPropertyChanged();
    void blockAllSignals(bool block);

    QString m_currentElementName;
    UiElementData m_currentData;     // working copy of element data
    double m_offsetX = 0.0;
    double m_offsetY = 0.0;
    bool m_batchActive = false;
    QStringList m_batchNames;

    void updateWar3Coords();
    bool isWar3Mode() const;
    double toDisplayX(double px) const;
    double toDisplayY(double py) const;
    double toDisplayW(double pw) const;
    double toDisplayH(double ph) const;
    double fromDisplayX(double v) const;
    double fromDisplayY(double v) const;
    double fromDisplayW(double v) const;
    double fromDisplayH(double v) const;
    void refreshSpinRanges();

    // Batch mode indicator
    QLabel *m_batchLabel;

    // Basic properties
    QLineEdit *m_nameEdit;
    QComboBox *m_typeCombo;
    QComboBox *m_coordMode;
    QDoubleSpinBox *m_xSpin;
    QDoubleSpinBox *m_ySpin;
    QDoubleSpinBox *m_widthSpin;
    QDoubleSpinBox *m_heightSpin;
    QLineEdit *m_textureEdit;
    QComboBox *m_parentCombo;
    QComboBox *m_groupCombo;

    // War3 coordinate display
    QTextEdit *m_war3Coords;

    // Extension properties
    QTableWidget *m_extraTable;

    // Type-specific properties
    QGroupBox *m_typeSpecificGroup;
    QWidget *m_buttonWidget = nullptr;
    QWidget *m_modelWidget = nullptr;
    QWidget *m_textWidget = nullptr;
    // BUTTON
    QLineEdit *m_normalTextureEdit;
    QLineEdit *m_highlightTextureEdit;
    // MODEL
    QLineEdit *m_modelPathEdit;
    // TEXT
    QLineEdit *m_textContentEdit;
    QDoubleSpinBox *m_fontSizeSpin;
    QLineEdit *m_textColorEdit;

    void updateTypeSpecificVisibility(const QString &type);
};

#endif // PROPERTYPANEL_H
