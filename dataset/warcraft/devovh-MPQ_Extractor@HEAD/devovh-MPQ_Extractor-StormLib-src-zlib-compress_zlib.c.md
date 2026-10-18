---
type: vendor-doc
source_repo: "https://github.com/devovh/MPQ_Extractor"
source_path: "StormLib/src/zlib/compress_zlib.c"
ref: HEAD
---

// Some compilers (e.g. Visual Studio 2012) don't like the name conflict between
// zlib\compress.c and bzip2\compress.c. This file is plain wrapper for compress.c
// in order to create obj file with a different name.

#include "compress.c"
