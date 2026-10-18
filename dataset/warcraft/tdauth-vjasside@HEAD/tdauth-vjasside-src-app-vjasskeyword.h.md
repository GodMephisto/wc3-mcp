---
type: vendor-doc
source_repo: "https://github.com/tdauth/vjasside"
source_path: "src/app/vjasskeyword.h"
ref: HEAD
---

#ifndef VJASSKEYWORD_H
#define VJASSKEYWORD_H

#include "vjassast.h"

class VJassKeyword : public VJassAst
{
public:
    VJassKeyword(int line, int column);

    void setKeyword(const QString &keyword);
    const QString& getKeyword() const;

    QString toString() const override;

private:
    QString keyword;
};

#endif // VJASSKEYWORD_H
