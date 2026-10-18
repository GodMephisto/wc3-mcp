---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "cmake/_FORTIFY_SOURCE.hpp"
ref: HEAD
---

#ifdef _FORTIFY_SOURCE
#if _FORTIFY_SOURCE < 3
#undef _FORTIFY_SOURCE
#define _FORTIFY_SOURCE 3
#endif
#else
#define _FORTIFY_SOURCE 3
#endif
