---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/compressonator/applications/_libs/cmp_mesh/timer.h"
ref: HEAD
---

/************************************************************************************/ /**
// Copyright (c) 2006-2024 Advanced Micro Devices, Inc. All rights reserved.
/// \author AMD Developer Tools Team
/// \file
****************************************************************************************/
#ifndef _TIMER_H
#define _TIMER_H

class Timer
{
public:
    Timer();
    void   Reset(void);
    double GetElapsed(void);
    double Get(void);

private:
    double time;
};

#endif  // _TIME_H
