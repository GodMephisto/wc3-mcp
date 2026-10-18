---
type: vendor-doc
source_repo: "https://github.com/tdauth/vjasside"
source_path: "src/app/autocompletionpopup.h"
ref: HEAD
---

#ifndef AUTOCOMPLETIONPOPUP_H
#define AUTOCOMPLETIONPOPUP_H

#include <QTreeWidget>

class AutoCompletionPopup : public QTreeWidget
{
public:
    AutoCompletionPopup();

protected:
    virtual void keyPressEvent(QKeyEvent *event) override;
    virtual void mousePressEvent(QMouseEvent *event) override;
    virtual void focusOutEvent(QFocusEvent *event) override;
};

#endif // AUTOCOMPLETIONPOPUP_H
