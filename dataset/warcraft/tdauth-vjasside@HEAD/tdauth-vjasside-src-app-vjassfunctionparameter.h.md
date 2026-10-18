---
type: vendor-doc
source_repo: "https://github.com/tdauth/vjasside"
source_path: "src/app/vjassfunctionparameter.h"
ref: HEAD
---

#ifndef VJASSFUNCTIONPARAMETER_H
#define VJASSFUNCTIONPARAMETER_H

#include <QString>

#include "vjassast.h"

class VJassFunctionParameter : public VJassAst
{
public:
    VJassFunctionParameter(int line, int column, const QString &type, const QString &name);

    virtual QString toString() const override;

private:
    QString type;
    QString name;
};

#endif // VJASSFUNCTIONPARAMETER_H
