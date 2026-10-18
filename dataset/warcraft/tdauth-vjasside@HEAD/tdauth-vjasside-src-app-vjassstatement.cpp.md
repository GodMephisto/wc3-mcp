---
type: vendor-doc
source_repo: "https://github.com/tdauth/vjasside"
source_path: "src/app/vjassstatement.cpp"
ref: HEAD
---

#include "vjassstatement.h"

VJassStatement::VJassStatement(int line, int column, Type type) : VJassAst(line, column), type(type), hasElse(false)
{
}

VJassStatement::Type VJassStatement::getType() const {
    return type;
}

void VJassStatement::setHasElse(bool hasElse) {
    this->hasElse = hasElse;
}

bool VJassStatement::getHasElse() const {
    return hasElse;
}
