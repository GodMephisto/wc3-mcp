---
type: vendor-doc
source_repo: "https://github.com/tdauth/vjasside"
source_path: "src/app/vjasskeyword.cpp"
ref: HEAD
---

#include "vjasskeyword.h"

VJassKeyword::VJassKeyword(int line, int column) : VJassAst(line, column)
{

}

void VJassKeyword::setKeyword(const QString &keyword) {
    this->keyword = keyword;
}

const QString& VJassKeyword::getKeyword() const {
    return keyword;
}

QString VJassKeyword::toString() const {
    return getKeyword();
}
