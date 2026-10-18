---
type: vendor-doc
source_repo: "https://github.com/ladislav-zezula/wcx_mpq"
source_path: "DllMain.cpp"
ref: HEAD
---

/*****************************************************************************/
/* DllMain.cpp                            Copyright (c) Ladislav Zezula 2003 */
/*---------------------------------------------------------------------------*/
/* Implementation of DLL Entry point for the WCX_MPQ plugin.                 */
/*---------------------------------------------------------------------------*/
/*   Date    Ver   Who  Comment                                              */
/* --------  ----  ---  -------                                              */
/* 13.07.03  1.00  Lad  The first version of DllMain.cpp                     */
/*****************************************************************************/

#include "wcx_mpq.h"
#include "resource.h"

int WINAPI DllMain(HINSTANCE hInstDll, DWORD fdwReason, LPVOID /* lpvReserved */)
{
    switch(fdwReason)
    {
        case DLL_PROCESS_ATTACH:
            InitInstance(hInstDll);
            break;

        case DLL_PROCESS_DETACH:
            g_hInst = NULL;
            break;
    }
    return TRUE;
}