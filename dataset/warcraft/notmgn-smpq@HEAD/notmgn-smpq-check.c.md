---
type: vendor-doc
source_repo: "https://github.com/notmgn/smpq"
source_path: "check.c"
ref: HEAD
---

#include <StormLib.h>

#if STORMLIB_VERSION < 0x0914
#error You have old Stormlib version
#endif

int main(void) {}
