---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "panels/propertypanel.cpp"
ref: HEAD
---

#include "propertypanel.h"

#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QFormLayout>
#include <QGroupBox>
#include <QPushButton>
#include <QHeaderView>
#include <QScrollArea>

PropertyPanel::PropertyPanel(QWidget *parent)
    : QWidget(parent)
{
    setupUi();
}

void PropertyPanel::setupUi()
{
    auto *outerLayout = new QVBoxLayout(this);
    outerLayout->setContentsMargins(0, 0, 0, 0);

    auto *scrollArea = new QScrollArea(this);
    scrollArea->setWidgetResizable(true);
    scrollArea->setFrameShape(QFrame::NoFrame);
    scrollArea->setHorizontalScrollBarPolicy(Qt::ScrollBarAlwaysOff);
    scrollArea->setStyleSheet("QScrollArea { background: transparent; }");

    auto *contentWidget = new QWidget(scrollArea);
    auto *mainLayout = new QVBoxLayout(contentWidget);
    mainLayout->setContentsMargins(4, 4, 4, 4);

    // Batch mode indicator (hidden by default)
    m_batchLabel = new QLabel(this);
    m_batchLabel->setStyleSheet(
        "QLabel { background-color: #3b3d5c; color: #7aa2f7; "
        "padding: 6px 8px; border-radius: 4px; font-weight: 600; "
        "font-size: 12px; }");
    m_batchLabel->setVisible(false);
    mainLayout->addWidget(m_batchLabel);

    // Basic properties group
    auto *basicGroup = new QGroupBox(tr("Basic Properties"), this);
    auto *formLayout = new QFormLayout(basicGroup);

    // Coordinate mode toggle
    m_coordMode = new QComboBox(this);
    m_coordMode->addItem("1920x1080 像素");
    m_coordMode->addItem("War3 (0-0.8, 0-0.6)");
    formLayout->addRow(tr("坐标模式:"), m_coordMode);

    m_nameEdit = new QLineEdit(this);
    formLayout->addRow(tr("Name:"), m_nameEdit);

    m_typeCombo = new QComboBox(this);
    m_typeCombo->addItems({"SIMPLEFRAME", "BACKDROP", "FRAME", "TEXT", "BUTTON", "EDITBOX", "SLIDER"});
    formLayout->addRow(tr("Type:"), m_typeCombo);

    m_xSpin = new QDoubleSpinBox(this);
    m_xSpin->setRange(-10000, 10000);
    m_xSpin->setDecimals(1);
    formLayout->addRow(tr("X:"), m_xSpin);

    m_ySpin = new QDoubleSpinBox(this);
    m_ySpin->setRange(-10000, 10000);
    m_ySpin->setDecimals(1);
    formLayout->addRow(tr("Y:"), m_ySpin);

    m_widthSpin = new QDoubleSpinBox(this);
    m_widthSpin->setRange(1, 10000);
    m_widthSpin->setDecimals(1);
    formLayout->addRow(tr("Width:"), m_widthSpin);

    m_heightSpin = new QDoubleSpinBox(this);
    m_heightSpin->setRange(1, 10000);
    m_heightSpin->setDecimals(1);
    formLayout->addRow(tr("Height:"), m_heightSpin);

    m_textureEdit = new QLineEdit(this);
    m_textureEdit->setPlaceholderText(tr("Path to texture image..."));
    formLayout->addRow(tr("Texture:"), m_textureEdit);

    m_parentCombo = new QComboBox(this);
    m_parentCombo->setEditable(true);
    m_parentCombo->setPlaceholderText(tr("(None - Root element)"));
    formLayout->addRow(tr("Parent:"), m_parentCombo);

    m_groupCombo = new QComboBox(this);
    m_groupCombo->setEditable(true);
    m_groupCombo->setPlaceholderText(tr("(No group)"));
    formLayout->addRow(tr("Group:"), m_groupCombo);

    mainLayout->addWidget(basicGroup);

    // War3 coordinate display (collapsible, selectable, editable)
    auto *war3Group = new QGroupBox(tr("锚点坐标 (9点)"), this);
    war3Group->setCheckable(true);
    auto *war3Layout = new QVBoxLayout(war3Group);
    m_war3Coords = new QTextEdit(this);
    m_war3Coords->setReadOnly(false);
    m_war3Coords->setPlainText("TOPLEFT   0.0, 0.0\n...");
    m_war3Coords->setFixedHeight(200);
    m_war3Coords->setStyleSheet("QTextEdit { font-family: 'Consolas', monospace; font-size: 11px; }");
    war3Layout->addWidget(m_war3Coords);
    connect(war3Group, &QGroupBox::toggled, m_war3Coords, &QWidget::setVisible);
    war3Group->setChecked(false);
    mainLayout->addWidget(war3Group);

    // Type-specific properties group
    m_typeSpecificGroup = new QGroupBox(tr("Type-Specific Properties"), this);
    auto *typeStackLayout = new QVBoxLayout(m_typeSpecificGroup);

    // BUTTON fields
    auto *buttonWidget = new QWidget(this);
    auto *buttonLayout = new QFormLayout(buttonWidget);
    buttonLayout->setContentsMargins(0, 0, 0, 0);
    m_normalTextureEdit = new QLineEdit(this);
    m_normalTextureEdit->setPlaceholderText(tr("Normal state texture..."));
    buttonLayout->addRow(tr("Normal Texture:"), m_normalTextureEdit);
    m_highlightTextureEdit = new QLineEdit(this);
    m_highlightTextureEdit->setPlaceholderText(tr("Highlight state texture..."));
    buttonLayout->addRow(tr("Highlight Texture:"), m_highlightTextureEdit);
    typeStackLayout->addWidget(buttonWidget);
    m_buttonWidget = buttonWidget;

    // MODEL fields
    auto *modelWidget = new QWidget(this);
    auto *modelLayout = new QFormLayout(modelWidget);
    modelLayout->setContentsMargins(0, 0, 0, 0);
    m_modelPathEdit = new QLineEdit(this);
    m_modelPathEdit->setPlaceholderText(tr("Path to .mdx model..."));
    modelLayout->addRow(tr("Model Path:"), m_modelPathEdit);
    typeStackLayout->addWidget(modelWidget);
    m_modelWidget = modelWidget;

    // TEXT fields
    auto *textWidget = new QWidget(this);
    auto *textLayout = new QFormLayout(textWidget);
    textLayout->setContentsMargins(0, 0, 0, 0);
    m_textContentEdit = new QLineEdit(this);
    m_textContentEdit->setPlaceholderText(tr("Text to display..."));
    textLayout->addRow(tr("Content:"), m_textContentEdit);
    m_fontSizeSpin = new QDoubleSpinBox(this);
    m_fontSizeSpin->setRange(1.0, 200.0);
    m_fontSizeSpin->setDecimals(0);
    m_fontSizeSpin->setValue(14.0);
    textLayout->addRow(tr("Font Size:"), m_fontSizeSpin);
    m_textColorEdit = new QLineEdit(this);
    m_textColorEdit->setPlaceholderText("#FFFFFF");
    textLayout->addRow(tr("Color:"), m_textColorEdit);
    typeStackLayout->addWidget(textWidget);
    m_textWidget = textWidget;

    mainLayout->addWidget(m_typeSpecificGroup);

    // Extension properties group
    auto *extraGroup = new QGroupBox(tr("Extended Properties"), this);
    auto *extraLayout = new QVBoxLayout(extraGroup);

    m_extraTable = new QTableWidget(0, 2, this);
    m_extraTable->setHorizontalHeaderLabels({tr("Key"), tr("Value")});
    m_extraTable->horizontalHeader()->setStretchLastSection(true);
    m_extraTable->setMinimumHeight(120);
    extraLayout->addWidget(m_extraTable);

    auto *btnLayout = new QHBoxLayout();
    auto *addBtn = new QPushButton(tr("+"), this);
    auto *removeBtn = new QPushButton(tr("-"), this);
    btnLayout->addWidget(addBtn);
    btnLayout->addWidget(removeBtn);
    btnLayout->addStretch();
    extraLayout->addLayout(btnLayout);

    mainLayout->addWidget(extraGroup);
    mainLayout->addStretch();

    scrollArea->setWidget(contentWidget);
    outerLayout->addWidget(scrollArea);

    // Wire up live edits
    connect(m_nameEdit, &QLineEdit::editingFinished, this, &PropertyPanel::onNameChanged);
    connect(m_typeCombo, &QComboBox::textActivated, this, [this](const QString &t) {
        m_currentData.type = t;
        updateTypeSpecificVisibility(t);
        emitPropertyChanged();
    });

    // Coordinate mode switch — refresh all displayed values
    connect(m_coordMode, QOverload<int>::of(&QComboBox::currentIndexChanged), this, [this](int) {
        refreshSpinRanges();
        if (!m_currentElementName.isEmpty()) {
            blockAllSignals(true);
            m_xSpin->setValue(toDisplayX(m_currentData.x));
            m_ySpin->setValue(toDisplayY(m_currentData.y));
            m_widthSpin->setValue(toDisplayW(m_currentData.width));
            m_heightSpin->setValue(toDisplayH(m_currentData.height));
            updateWar3Coords();
            blockAllSignals(false);
        }
    });

    connect(m_xSpin, QOverload<double>::of(&QDoubleSpinBox::valueChanged), this, [this](double v) {
        if (m_xSpin->hasFocus()) { m_currentData.x = fromDisplayX(v); emitPropertyChanged(); updateWar3Coords(); }
    });
    connect(m_ySpin, QOverload<double>::of(&QDoubleSpinBox::valueChanged), this, [this](double v) {
        if (m_ySpin->hasFocus()) { m_currentData.y = fromDisplayY(v); emitPropertyChanged(); updateWar3Coords(); }
    });
    connect(m_widthSpin, QOverload<double>::of(&QDoubleSpinBox::valueChanged), this, [this](double v) {
        if (m_widthSpin->hasFocus()) { m_currentData.width = fromDisplayW(v); emitPropertyChanged(); updateWar3Coords(); }
    });
    connect(m_heightSpin, QOverload<double>::of(&QDoubleSpinBox::valueChanged), this, [this](double v) {
        if (m_heightSpin->hasFocus()) { m_currentData.height = fromDisplayH(v); emitPropertyChanged(); updateWar3Coords(); }
    });
    connect(m_textureEdit, &QLineEdit::editingFinished, this, [this]() {
        m_currentData.texture = m_textureEdit->text();
        emitPropertyChanged();
    });
    connect(m_parentCombo, &QComboBox::textActivated, this, [this](const QString &p) {
        m_currentData.parent = p;
        emitPropertyChanged();
    });

    connect(m_groupCombo, &QComboBox::textActivated, this, [this](const QString &g) {
        m_currentData.group = g;
        emitPropertyChanged();
    });

    // Type-specific field connections
    connect(m_normalTextureEdit, &QLineEdit::editingFinished, this, [this]() {
        m_currentData.normalTexture = m_normalTextureEdit->text();
        emitPropertyChanged();
    });
    connect(m_highlightTextureEdit, &QLineEdit::editingFinished, this, [this]() {
        m_currentData.highlightTexture = m_highlightTextureEdit->text();
        emitPropertyChanged();
    });
    connect(m_modelPathEdit, &QLineEdit::editingFinished, this, [this]() {
        m_currentData.modelPath = m_modelPathEdit->text();
        emitPropertyChanged();
    });
    connect(m_textContentEdit, &QLineEdit::editingFinished, this, [this]() {
        m_currentData.textContent = m_textContentEdit->text();
        emitPropertyChanged();
    });
    connect(m_fontSizeSpin, QOverload<double>::of(&QDoubleSpinBox::valueChanged), this, [this](double v) {
        if (m_fontSizeSpin->hasFocus()) { m_currentData.fontSize = v; emitPropertyChanged(); }
    });
    connect(m_textColorEdit, &QLineEdit::editingFinished, this, [this]() {
        m_currentData.textColor = m_textColorEdit->text();
        emitPropertyChanged();
    });

    connect(addBtn, &QPushButton::clicked, this, [this]() {
        m_extraTable->insertRow(m_extraTable->rowCount());
    });
    connect(removeBtn, &QPushButton::clicked, this, [this]() {
        int row = m_extraTable->currentRow();
        if (row >= 0) m_extraTable->removeRow(row);
    });

    clearPanel();
    refreshSpinRanges();
}

void PropertyPanel::showElement(const QString &name, const UiElementData &data)
{
    m_currentElementName = name;
    m_currentData = data;   // take a copy
    m_batchActive = false;
    m_batchNames.clear();

    blockAllSignals(true);
    m_batchLabel->setVisible(false);
    refreshSpinRanges();

    // Re-enable fields that may have been disabled in batch mode
    m_nameEdit->setEnabled(true);
    m_typeCombo->setEnabled(true);
    m_parentCombo->setEnabled(true);

    m_nameEdit->setText(m_currentData.name);
    m_typeCombo->setCurrentText(m_currentData.type);
    m_xSpin->setValue(toDisplayX(m_currentData.x));
    m_ySpin->setValue(toDisplayY(m_currentData.y));
    m_widthSpin->setValue(toDisplayW(m_currentData.width));
    m_heightSpin->setValue(toDisplayH(m_currentData.height));
    m_textureEdit->setText(m_currentData.texture);
    m_parentCombo->setCurrentText(m_currentData.parent);
    m_groupCombo->setCurrentText(m_currentData.group);

    // Type-specific fields
    m_normalTextureEdit->setText(m_currentData.normalTexture);
    m_highlightTextureEdit->setText(m_currentData.highlightTexture);
    m_modelPathEdit->setText(m_currentData.modelPath);
    m_textContentEdit->setText(m_currentData.textContent);
    m_fontSizeSpin->setValue(m_currentData.fontSize);
    m_textColorEdit->setText(m_currentData.textColor);
    updateTypeSpecificVisibility(m_currentData.type);

    m_extraTable->setRowCount(m_currentData.properties.size());
    int row = 0;
    for (auto it = m_currentData.properties.begin();
         it != m_currentData.properties.end(); ++it, ++row) {
        m_extraTable->setItem(row, 0, new QTableWidgetItem(it.key()));
        m_extraTable->setItem(row, 1, new QTableWidgetItem(it.value()));
    }

    updateWar3Coords();

    blockAllSignals(false);
    setEnabled(true);
}

void PropertyPanel::showBatch(const QStringList &names, const QList<UiElementData> &elements)
{
    if (elements.isEmpty()) return;

    m_batchActive = true;
    m_batchNames = names;
    m_currentElementName = names.first();
    m_currentData = elements.first();

    m_batchLabel->setText(tr("Batch: %1 elements selected").arg(names.size()));
    m_batchLabel->setVisible(true);

    blockAllSignals(true);
    refreshSpinRanges();

    // Show the first element's data as the template
    m_nameEdit->setText(tr("(multiple)"));
    m_typeCombo->setCurrentText(m_currentData.type);
    m_xSpin->setValue(toDisplayX(m_currentData.x));
    m_ySpin->setValue(toDisplayY(m_currentData.y));
    m_widthSpin->setValue(toDisplayW(m_currentData.width));
    m_heightSpin->setValue(toDisplayH(m_currentData.height));
    m_textureEdit->setText(m_currentData.texture);
    m_parentCombo->setCurrentText(QString());
    m_groupCombo->setCurrentText(QString());

    // Type-specific fields
    m_normalTextureEdit->setText(m_currentData.normalTexture);
    m_highlightTextureEdit->setText(m_currentData.highlightTexture);
    m_modelPathEdit->setText(m_currentData.modelPath);
    m_textContentEdit->setText(m_currentData.textContent);
    m_fontSizeSpin->setValue(m_currentData.fontSize);
    m_textColorEdit->setText(m_currentData.textColor);
    updateTypeSpecificVisibility(m_currentData.type);

    m_extraTable->setRowCount(0);

    // Disable fields that don't make sense for batch edit
    m_nameEdit->setEnabled(false);
    m_typeCombo->setEnabled(false);
    m_parentCombo->setEnabled(false);

    updateWar3Coords();

    blockAllSignals(false);
    setEnabled(true);
}

void PropertyPanel::updateParentList(const QStringList &parentNames)
{
    QString currentParent = m_parentCombo->currentText();
    m_parentCombo->clear();
    m_parentCombo->addItem("");
    m_parentCombo->addItems(parentNames);
    if (m_parentCombo->findText(currentParent) >= 0)
        m_parentCombo->setCurrentText(currentParent);
}

void PropertyPanel::updateGroupList(const QStringList &groups)
{
    QString currentGroup = m_groupCombo->currentText();
    m_groupCombo->clear();
    m_groupCombo->addItem("");
    m_groupCombo->addItems(groups);
    if (m_groupCombo->findText(currentGroup) >= 0)
        m_groupCombo->setCurrentText(currentGroup);
}

void PropertyPanel::clearPanel()
{
    m_currentElementName.clear();
    m_currentData = UiElementData();
    m_offsetX = 0.0;
    m_offsetY = 0.0;
    m_batchActive = false;
    m_batchNames.clear();

    blockAllSignals(true);
    m_batchLabel->setVisible(false);
    m_coordMode->setCurrentIndex(0);
    refreshSpinRanges();
    m_typeCombo->setCurrentIndex(0);
    m_xSpin->setValue(0);
    m_ySpin->setValue(0);
    m_widthSpin->setValue(100);
    m_heightSpin->setValue(100);
    m_textureEdit->clear();
    m_parentCombo->clear();
    m_groupCombo->clear();
    m_war3Coords->clear();
    // Type-specific
    m_normalTextureEdit->clear();
    m_highlightTextureEdit->clear();
    m_modelPathEdit->clear();
    m_textContentEdit->clear();
    m_fontSizeSpin->setValue(14.0);
    m_textColorEdit->clear();
    m_typeSpecificGroup->setVisible(false);
    m_extraTable->setRowCount(0);
    blockAllSignals(false);

    setEnabled(false);
}

void PropertyPanel::setReferenceOffset(double ox, double oy)
{
    m_offsetX = ox;
    m_offsetY = oy;
}

void PropertyPanel::onNameChanged()
{
    QString newName = m_nameEdit->text().trimmed();
    if (newName.isEmpty() || newName == m_currentElementName) return;

    QString oldName = m_currentElementName;
    m_currentData.name = newName;
    m_currentElementName = newName;
    emit propertyChanged(oldName, m_currentData);
}

void PropertyPanel::updateWar3Coords()
{
    double px = m_currentData.x;
    double py = m_currentData.y;
    double pw = m_currentData.width;
    double ph = m_currentData.height;

    struct Anchor { const char *name; double ax, ay; };
    Anchor anchors[9] = {
        {"TOPLEFT",     px,      py},
        {"TOP",         px+pw/2, py},
        {"TOPRIGHT",    px+pw,   py},
        {"LEFT",        px,      py+ph/2},
        {"CENTER",      px+pw/2, py+ph/2},
        {"RIGHT",       px+pw,   py+ph/2},
        {"BOTTOMLEFT",  px,      py+ph},
        {"BOTTOM",      px+pw/2, py+ph},
        {"BOTTOMRIGHT", px+pw,   py+ph},
    };

    bool war3 = isWar3Mode();
    QString text;
    for (int i = 0; i < 9; ++i) {
        text += QString("%1  %2, %3\n")
            .arg(anchors[i].name, -12)
            .arg(toDisplayX(anchors[i].ax), 8, 'f', war3 ? 4 : 1)
            .arg(toDisplayY(anchors[i].ay), 8, 'f', war3 ? 4 : 1);
    }

    m_war3Coords->setText(text);
}

bool PropertyPanel::isWar3Mode() const
{
    return m_coordMode->currentIndex() == 1;
}

double PropertyPanel::toDisplayX(double px) const
{
    double rel = px - m_offsetX;
    return isWar3Mode() ? rel / 1920.0 * 0.8 : rel;
}

double PropertyPanel::toDisplayY(double py) const
{
    if (isWar3Mode()) {
        // War3 Y is from bottom-left; convert from top-left, then subtract parent offset
        double wy = (1080.0 - py) / 1080.0 * 0.6;
        if (m_offsetY != 0.0) {
            wy -= (1080.0 - m_offsetY) / 1080.0 * 0.6;
        }
        return wy;
    }
    return py - m_offsetY;
}

double PropertyPanel::toDisplayW(double pw) const
{
    return isWar3Mode() ? pw / 1920.0 * 0.8 : pw;
}

double PropertyPanel::toDisplayH(double ph) const
{
    return isWar3Mode() ? ph / 1080.0 * 0.6 : ph;
}

double PropertyPanel::fromDisplayX(double v) const
{
    double rel = isWar3Mode() ? v / 0.8 * 1920.0 : v;
    return rel + m_offsetX;
}

double PropertyPanel::fromDisplayY(double v) const
{
    if (isWar3Mode()) {
        // v is (possibly parent-relative) War3 Y; convert back to top-left pixel Y
        double wy = v;
        if (m_offsetY != 0.0) {
            wy += (1080.0 - m_offsetY) / 1080.0 * 0.6;
        }
        return 1080.0 - wy / 0.6 * 1080.0;
    }
    return v + m_offsetY;
}

double PropertyPanel::fromDisplayW(double v) const
{
    return isWar3Mode() ? v / 0.8 * 1920.0 : v;
}

double PropertyPanel::fromDisplayH(double v) const
{
    return isWar3Mode() ? v / 0.6 * 1080.0 : v;
}

void PropertyPanel::refreshSpinRanges()
{
    if (isWar3Mode()) {
        for (auto *s : {m_xSpin, m_ySpin, m_widthSpin, m_heightSpin}) {
            s->setRange(-10.0, 10.0);
            s->setDecimals(4);
            s->setSingleStep(0.01);
        }
    } else {
        for (auto *s : {m_xSpin, m_ySpin}) {
            s->setRange(-10000.0, 10000.0);
            s->setDecimals(1);
            s->setSingleStep(1.0);
        }
        m_widthSpin->setRange(1.0, 10000.0);
        m_widthSpin->setDecimals(1);
        m_widthSpin->setSingleStep(1.0);
        m_heightSpin->setRange(1.0, 10000.0);
        m_heightSpin->setDecimals(1);
        m_heightSpin->setSingleStep(1.0);
    }
}

void PropertyPanel::updateTypeSpecificVisibility(const QString &type)
{
    bool isButton = (type == "BUTTON");
    bool isModel = (type == "MODEL");
    bool isText = (type == "TEXT");
    bool hasSpecific = isButton || isModel || isText;

    if (m_buttonWidget) m_buttonWidget->setVisible(isButton);
    if (m_modelWidget) m_modelWidget->setVisible(isModel);
    if (m_textWidget) m_textWidget->setVisible(isText);
    m_typeSpecificGroup->setVisible(hasSpecific);

    if (isButton)
        m_typeSpecificGroup->setTitle(tr("Button Properties"));
    else if (isModel)
        m_typeSpecificGroup->setTitle(tr("Model Properties"));
    else if (isText)
        m_typeSpecificGroup->setTitle(tr("Text Properties"));
    else
        m_typeSpecificGroup->setTitle(tr("Type-Specific Properties"));
}

void PropertyPanel::emitPropertyChanged()
{
    if (m_batchActive) {
        emit batchPropertyChanged(m_batchNames, m_currentData);
    } else {
        emit propertyChanged(m_currentElementName, m_currentData);
    }
}

void PropertyPanel::blockAllSignals(bool block)
{
    m_nameEdit->blockSignals(block);
    m_coordMode->blockSignals(block);
    m_typeCombo->blockSignals(block);
    m_xSpin->blockSignals(block);
    m_ySpin->blockSignals(block);
    m_widthSpin->blockSignals(block);
    m_heightSpin->blockSignals(block);
    m_textureEdit->blockSignals(block);
    m_parentCombo->blockSignals(block);
    m_groupCombo->blockSignals(block);
    m_normalTextureEdit->blockSignals(block);
    m_highlightTextureEdit->blockSignals(block);
    m_modelPathEdit->blockSignals(block);
    m_textContentEdit->blockSignals(block);
    m_fontSizeSpin->blockSignals(block);
    m_textColorEdit->blockSignals(block);
    m_extraTable->blockSignals(block);
}
