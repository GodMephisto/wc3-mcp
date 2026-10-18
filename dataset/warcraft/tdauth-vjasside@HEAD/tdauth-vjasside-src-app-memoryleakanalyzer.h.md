---
type: vendor-doc
source_repo: "https://github.com/tdauth/vjasside"
source_path: "src/app/memoryleakanalyzer.h"
ref: HEAD
---

#ifndef MEMORYLEAKANALYZER_H
#define MEMORYLEAKANALYZER_H

#include "vjassast.h"
#include "vjassglobal.h"

/**
 * @brief Overapproximates possible memory leaks by looking for memory release calls with matching identifiers.
 */
class MemoryLeakAnalyzer
{
public:
    static const QMap<QString, QString> LEAKING_TYPES;

    MemoryLeakAnalyzer(VJassAst *ast);

    const QList<VJassGlobal*>& getGlobals() const;

private:
    QList<VJassGlobal*> globals;
};

#endif // MEMORYLEAKANALYZER_H
