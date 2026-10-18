---
type: vendor-doc
source_repo: "https://github.com/WarRaft/tree-sitter-vjass"
source_path: "bindings/c/tree_sitter/tree-sitter-vjass.h"
ref: HEAD
---

#ifndef TREE_SITTER_VJASS_H_
#define TREE_SITTER_VJASS_H_

typedef struct TSLanguage TSLanguage;

#ifdef __cplusplus
extern "C" {
#endif

const TSLanguage *tree_sitter_vjass(void);

#ifdef __cplusplus
}
#endif

#endif // TREE_SITTER_VJASS_H_
