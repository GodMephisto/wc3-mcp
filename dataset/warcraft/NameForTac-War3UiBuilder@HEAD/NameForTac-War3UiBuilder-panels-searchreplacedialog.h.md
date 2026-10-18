---
type: vendor-doc
source_repo: "https://github.com/NameForTac/War3UiBuilder"
source_path: "panels/searchreplacedialog.h"
ref: HEAD
---

#ifndef SEARCHREPLACEDIALOG_H
#define SEARCHREPLACEDIALOG_H

#include <QDialog>
#include <QLineEdit>
#include <QComboBox>
#include <QListWidget>
#include <QPushButton>
#include <QCheckBox>

#include "elements/uielementdata.h"

class SearchReplaceDialog : public QDialog
{
    Q_OBJECT

public:
    explicit SearchReplaceDialog(const QList<UiElementData> &allElements,
                                  const QList<UiElementData> &selectedElements,
                                  QWidget *parent = nullptr);

    struct Replacement {
        QString elementName;
        QString propertyKey;  // empty = element name itself
        QString oldValue;
        QString newValue;
    };
    const QList<Replacement> &appliedReplacements() const { return m_applied; }

private slots:
    void onFindNext();
    void onFindAll();
    void onReplace();
    void onReplaceAll();

private:
    enum Scope { AllElements, SelectedElements };
    enum MatchMode { Exact, Contains };
    enum SearchField { Names, Properties, Both };

    void search(QList<QListWidgetItem *> &results);
    bool matches(const QString &text) const;

    QLineEdit *m_searchEdit;
    QLineEdit *m_replaceEdit;
    QComboBox *m_scopeCombo;
    QComboBox *m_matchCombo;
    QComboBox *m_fieldCombo;
    QListWidget *m_resultList;
    QPushButton *m_findNextBtn;
    QPushButton *m_findAllBtn;
    QPushButton *m_replaceBtn;
    QPushButton *m_replaceAllBtn;

    QList<UiElementData> m_allElements;
    QList<UiElementData> m_selectedElements;
    QList<Replacement> m_applied;
    int m_currentResult = -1;
};

#endif // SEARCHREPLACEDIALOG_H
