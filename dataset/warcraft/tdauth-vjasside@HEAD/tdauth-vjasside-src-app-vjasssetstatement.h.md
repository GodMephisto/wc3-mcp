---
type: vendor-doc
source_repo: "https://github.com/tdauth/vjasside"
source_path: "src/app/vjasssetstatement.h"
ref: HEAD
---

#ifndef VJASSSETSTATEMENT_H
#define VJASSSETSTATEMENT_H

#include "vjassstatement.h"

class VJassSetStatement : public VJassStatement
{
public:
    VJassSetStatement(int line, int column);
};

#endif // VJASSSETSTATEMENT_H
