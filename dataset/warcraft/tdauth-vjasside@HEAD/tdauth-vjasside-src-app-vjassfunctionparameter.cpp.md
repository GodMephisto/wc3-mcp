---
type: vendor-doc
source_repo: "https://github.com/tdauth/vjasside"
source_path: "src/app/vjassfunctionparameter.cpp"
ref: HEAD
---

#include "vjassfunctionparameter.h"

VJassFunctionParameter::VJassFunctionParameter(int line, int column, const QString &type, const QString &name) : VJassAst(line, column), type(type), name(name)
{

}

QString VJassFunctionParameter::toString() const {
    return type + " " + name;
}
