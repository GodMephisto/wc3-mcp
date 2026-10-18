---
type: vendor-doc
source_repo: "https://github.com/tdauth/vjasside"
source_path: "src/app/vjasssetstatement.cpp"
ref: HEAD
---

#include "vjasssetstatement.h"

VJassSetStatement::VJassSetStatement(int line, int column) : VJassStatement(line, column, VJassStatement::Set)
{
}
