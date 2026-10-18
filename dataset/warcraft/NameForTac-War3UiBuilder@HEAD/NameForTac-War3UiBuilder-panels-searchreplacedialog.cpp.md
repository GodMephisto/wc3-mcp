---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "panels/searchreplacedialog.cpp"
ref: HEAD
---

#include "searchreplacedialog.h"

#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QFormLayout>
#include <QLabel>
#include <QGroupBox>
#include <QMessageBox>

SearchReplaceDialog::SearchReplaceDialog(const QList<UiElementData> &allElements,
                                          const QList<UiElementData> &selectedElements,
                                          QWidget *parent)
    : QDialog(parent)
    , m_allElements(allElements)
    , m_selectedElements(selectedElements)
{
    setWindowTitle(tr("Search & Replace Properties"));
    setMinimumSize(520, 420);
    resize(560, 480);

    auto *mainLayout = new QVBoxLayout(this);

    // Search row
    auto *formLayout = new QFormLayout;
    m_searchEdit = new QLineEdit(this);
    m_searchEdit->setPlaceholderText(tr("Search text..."));
    formLayout->addRow(tr("Search:"), m_searchEdit);

    m_replaceEdit = new QLineEdit(this);
    m_replaceEdit->setPlaceholderText(tr("Replace with..."));
    formLayout->addRow(tr("Replace:"), m_replaceEdit);
    mainLayout->addLayout(formLayout);

    // Options row
    auto *optionsLayout = new QHBoxLayout;

    optionsLayout->addWidget(new QLabel(tr("Scope:")));
    m_scopeCombo = new QComboBox(this);
    m_scopeCombo->addItem(tr("All Elements"));
    m_scopeCombo->addItem(tr("Selected Only"));
    optionsLayout->addWidget(m_scopeCombo);

    optionsLayout->addWidget(new QLabel(tr("Match:")));
    m_matchCombo = new QComboBox(this);
    m_matchCombo->addItem(tr("Contains"));
    m_matchCombo->addItem(tr("Exact"));
    optionsLayout->addWidget(m_matchCombo);

    optionsLayout->addWidget(new QLabel(tr("Search in:")));
    m_fieldCombo = new QComboBox(this);
    m_fieldCombo->addItem(tr("Element Names"));
    m_fieldCombo->addItem(tr("Property Values"));
    m_fieldCombo->addItem(tr("Both"));
    optionsLayout->addWidget(m_fieldCombo);

    optionsLayout->addStretch();
    mainLayout->addLayout(optionsLayout);

    // Results list
    m_resultList = new QListWidget(this);
    m_resultList->setAlternatingRowColors(true);
    m_resultList->setStyleSheet(
        "QListWidget { font-size: 12px; }"
        "QListWidget::item { padding: 4px 6px; }"
        "QListWidget::item:alternate { background-color: #232433; }");
    mainLayout->addWidget(m_resultList, 1);

    // Buttons
    auto *btnLayout = new QHBoxLayout;
    m_findNextBtn = new QPushButton(tr("Find &Next"), this);
    m_findAllBtn = new QPushButton(tr("Find &All"), this);
    m_replaceBtn = new QPushButton(tr("&Replace"), this);
    m_replaceAllBtn = new QPushButton(tr("Replace &All"), this);
    auto *closeBtn = new QPushButton(tr("Close"), this);

    m_replaceBtn->setEnabled(false);
    m_replaceAllBtn->setEnabled(false);

    btnLayout->addWidget(m_findNextBtn);
    btnLayout->addWidget(m_findAllBtn);
    btnLayout->addWidget(m_replaceBtn);
    btnLayout->addWidget(m_replaceAllBtn);
    btnLayout->addStretch();
    btnLayout->addWidget(closeBtn);
    mainLayout->addLayout(btnLayout);

    // Connections
    connect(m_findNextBtn, &QPushButton::clicked, this, &SearchReplaceDialog::onFindNext);
    connect(m_findAllBtn, &QPushButton::clicked, this, &SearchReplaceDialog::onFindAll);
    connect(m_replaceBtn, &QPushButton::clicked, this, &SearchReplaceDialog::onReplace);
    connect(m_replaceAllBtn, &QPushButton::clicked, this, &SearchReplaceDialog::onReplaceAll);
    connect(closeBtn, &QPushButton::clicked, this, &QDialog::close);
    connect(m_searchEdit, &QLineEdit::returnPressed, this, &SearchReplaceDialog::onFindNext);
    connect(m_resultList, &QListWidget::currentRowChanged, this, [this](int row) {
        m_currentResult = row;
        m_replaceBtn->setEnabled(row >= 0);
    });
    connect(m_resultList, &QListWidget::itemSelectionChanged, this, [this]() {
        m_replaceAllBtn->setEnabled(m_resultList->count() > 0);
    });
}

bool SearchReplaceDialog::matches(const QString &text) const
{
    if (text.isEmpty()) return false;
    bool exact = (m_matchCombo->currentIndex() == 1);
    QString searchText = m_searchEdit->text();
    if (exact)
        return text == searchText;
    else
        return text.contains(searchText, Qt::CaseInsensitive);
}

void SearchReplaceDialog::search(QList<QListWidgetItem *> &results)
{
    results.clear();
    QString searchText = m_searchEdit->text();
    if (searchText.isEmpty()) return;

    const auto &elements = (m_scopeCombo->currentIndex() == SelectedElements)
                               ? m_selectedElements : m_allElements;
    int fieldMode = m_fieldCombo->currentIndex(); // 0=names, 1=properties, 2=both

    for (const auto &el : elements) {
        // Search element name
        if (fieldMode != 1 && matches(el.name)) {
            auto *item = new QListWidgetItem(
                QString("%1  (name)").arg(el.name));
            item->setData(Qt::UserRole, el.name);
            item->setData(Qt::UserRole + 1, QString());
            item->setData(Qt::UserRole + 2, el.name);
            results.append(item);
        }

        // Search in properties
        if (fieldMode != 0) {
            // Type-specific properties
            struct PropEntry { QString key; QString value; };
            QList<PropEntry> props;
            props.append({"Texture", el.texture});
            props.append({"NormalTexture", el.normalTexture});
            props.append({"HighlightTexture", el.highlightTexture});
            props.append({"ModelPath", el.modelPath});
            props.append({"TextContent", el.textContent});
            props.append({"TextColor", el.textColor});
            if (el.fontSize > 0)
                props.append({"FontSize", QString::number(el.fontSize, 'f', 0)});

            for (auto it = el.properties.begin(); it != el.properties.end(); ++it)
                props.append({it.key(), it.value()});

            for (const auto &p : props) {
                if (p.value.isEmpty()) continue;
                if (matches(p.value)) {
                    auto *item = new QListWidgetItem(
                        QString("%1  [%2]  %3").arg(el.name, p.key, p.value));
                    item->setData(Qt::UserRole, el.name);
                    item->setData(Qt::UserRole + 1, p.key);
                    item->setData(Qt::UserRole + 2, p.value);
                    results.append(item);
                }
            }
        }
    }
}

void SearchReplaceDialog::onFindNext()
{
    QList<QListWidgetItem *> allResults;
    search(allResults);

    m_resultList->clear();
    for (auto *item : allResults)
        m_resultList->addItem(item);

    if (m_resultList->count() > 0) {
        int next = (m_currentResult + 1) % m_resultList->count();
        m_resultList->setCurrentRow(next);
        m_replaceAllBtn->setEnabled(true);
    }
}

void SearchReplaceDialog::onFindAll()
{
    QList<QListWidgetItem *> allResults;
    search(allResults);

    m_resultList->clear();
    for (auto *item : allResults)
        m_resultList->addItem(item);

    m_currentResult = -1;
    m_replaceBtn->setEnabled(false);
    m_replaceAllBtn->setEnabled(m_resultList->count() > 0);
}

void SearchReplaceDialog::onReplace()
{
    auto *current = m_resultList->currentItem();
    if (!current) return;

    QString elName = current->data(Qt::UserRole).toString();
    QString propKey = current->data(Qt::UserRole + 1).toString();
    QString oldVal = current->data(Qt::UserRole + 2).toString();
    QString newVal = m_replaceEdit->text();

    m_applied.append({elName, propKey, oldVal, newVal});

    // Update the result list display
    if (propKey.isEmpty()) {
        // Element name was matched
        current->setText(QString("%1  (name)  ->  %2").arg(elName, newVal));
        current->setData(Qt::UserRole + 2, newVal);
    } else {
        current->setText(QString("%1  [%2]  %3  ->  %4").arg(elName, propKey, oldVal, newVal));
        current->setData(Qt::UserRole + 2, newVal);
    }
    current->setBackground(QColor(40, 80, 40));

    // Move to next result
    int next = m_currentResult + 1;
    if (next < m_resultList->count())
        m_resultList->setCurrentRow(next);
}

void SearchReplaceDialog::onReplaceAll()
{
    QString replaceText = m_replaceEdit->text();

    // Clear and re-run full search
    QList<QListWidgetItem *> allResults;
    search(allResults);

    for (auto *item : allResults) {
        QString elName = item->data(Qt::UserRole).toString();
        QString propKey = item->data(Qt::UserRole + 1).toString();
        QString oldVal = item->data(Qt::UserRole + 2).toString();

        m_applied.append({elName, propKey, oldVal, replaceText});

        if (propKey.isEmpty())
            item->setText(QString("%1  (name)  ->  %2").arg(elName, replaceText));
        else
            item->setText(QString("%1  [%2]  %3  ->  %4").arg(elName, propKey, oldVal, replaceText));
        item->setBackground(QColor(40, 80, 40));
    }

    // Replace list with modified items
    m_resultList->clear();
    for (auto *item : allResults)
        m_resultList->addItem(item);

    m_replaceAllBtn->setEnabled(false);
    m_replaceBtn->setEnabled(false);

    if (!allResults.isEmpty())
        QMessageBox::information(this, tr("Replace All"),
            tr("Replaced %1 occurrence(s).").arg(m_applied.size()));
}
