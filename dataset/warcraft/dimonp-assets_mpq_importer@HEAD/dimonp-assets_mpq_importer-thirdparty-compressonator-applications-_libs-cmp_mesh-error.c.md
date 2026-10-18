---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/compressonator/applications/_libs/cmp_mesh/error.c"
ref: HEAD
---

/************************************************************************************/ /**
// Copyright (c) 2006-2024 Advanced Micro Devices, Inc. All rights reserved.
/// \author AMD Developer Tools Team
/// \file
****************************************************************************************/
#include "tootlepch.h"

void error_output(const char* fmt, ...)
{
    // disable all console output if ERROR_SILENT is defined
#ifndef ERROR_SILENT
    va_list ap;
    va_start(ap, fmt);
    vfprintf(stderr, fmt, ap);
    va_end(ap);
#else
    (void)fmt;
#endif
}
