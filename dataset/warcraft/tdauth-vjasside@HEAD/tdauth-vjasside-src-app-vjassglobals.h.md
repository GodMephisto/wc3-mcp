---
type: vendor-doc
source_repo: "https://github.com/tdauth/vjasside"
source_path: "src/app/vjassglobals.h"
ref: HEAD
---

#ifndef VJASSGLOBALS_H
#define VJASSGLOBALS_H

#include "vjassast.h"


class VJassGlobals: public VJassAst
{
public:
    VJassGlobals(int line, int column);
    virtual ~VJassGlobals();

    virtual QString toString() const override;
};

#endif // VJASSGLOBALS_H
