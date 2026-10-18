---
type: vendor-doc
source_repo: "https://github.com/tdauth/vjasside"
source_path: "src/app/vjassparser.h"
ref: HEAD
---

#ifndef VJASSPARSER_H
#define VJASSPARSER_H

#include <QList>

#include "vjassast.h"


class VJassParser
{
public:
    VJassParser();

    VJassAst* parse(const QList<VJassToken> &tokens);
};

#endif // VJASSPARSER_H
