---
type: vendor-doc
source_repo: "https://github.com/tdauth/vjasside"
source_path: "src/app/vjassfunction.h"
ref: HEAD
---

#ifndef VJASSFUNCTION_H
#define VJASSFUNCTION_H

#include "vjassnative.h"


class VJassFunction : public VJassNative
{
public:
    VJassFunction(int line, int column);

    virtual QString toString() const override;
};

#endif // VJASSFUNCTION_H
