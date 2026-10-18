---
type: vendor-doc
source_repo: "https://github.com/Slayer95/tree-sitter-jass"
source_path: "bindings/c/tree_sitter/tree-sitter-jass.h"
ref: HEAD
---

#ifndef TREE_SITTER_JASS_H_
#define TREE_SITTER_JASS_H_

typedef struct TSLanguage TSLanguage;

#ifdef __cplusplus
extern "C" {
#endif

const TSLanguage *tree_sitter_jass(void);

#ifdef __cplusplus
}
#endif

#endif // TREE_SITTER_JASS_H_
