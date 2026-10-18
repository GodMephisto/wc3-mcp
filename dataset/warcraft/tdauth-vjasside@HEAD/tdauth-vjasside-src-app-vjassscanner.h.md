---
type: vendor-doc
source_repo: "https://github.com/tdauth/vjasside"
source_path: "src/app/vjassscanner.h"
ref: HEAD
---

#ifndef VJASSSCANNER_H
#define VJASSSCANNER_H

#include <QList>

#include "vjasstoken.h"


class VJassScanner
{
public:
    VJassScanner();

    QList<VJassToken> scan(const QString &content, bool dropWhiteSpaces = true);
};

#endif // VJASSSCANNER_H
