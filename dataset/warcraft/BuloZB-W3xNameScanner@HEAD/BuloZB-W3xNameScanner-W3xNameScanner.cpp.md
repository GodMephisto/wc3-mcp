---
type: vendor-doc
source_repo: "https://github.com/BuloZB/W3xNameScanner"
source_path: "W3xNameScanner.cpp"
ref: HEAD
---

/*****************************************************************************/
/* W3xNameScanner.cpp                     Copyright (c) Ladislav Zezula 2015 */
/*---------------------------------------------------------------------------*/
/* Description :                                                             */
/*---------------------------------------------------------------------------*/
/*   Date    Ver   Who  Comment                                              */
/* --------  ----  ---  -------                                              */
/* 08.11.07  1.00  Lad  The first version of W3xNameScanner.cpp              */
/*****************************************************************************/

#include "MPQEditor.h"
#include "resource.h"

#pragma comment(lib, "comctl32.lib")

//-----------------------------------------------------------------------------
// Global variables

HINSTANCE g_hInst;
HANDLE g_hHeap;

//-----------------------------------------------------------------------------
// WinMain

int WINAPI _tWinMain(HINSTANCE hInst, HINSTANCE, LPTSTR, int)
{
    LPCTSTR szMpqName = (__argc >= 2) ? __targv[1] : NULL;
    LPCTSTR szLstName = (__argc >= 3) ? __targv[2] : NULL;

    g_hInst = hInst;
    g_hHeap = GetProcessHeap();
    InitCommonControls();
    
    NameScannerDialog(NULL, szMpqName, szLstName);
}
