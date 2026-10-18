---
type: vendor-doc
source_repo: "https://github.com/BuloZB/W3xNameScanner"
source_path: "DlgWorker.cpp"
ref: HEAD
---

/*****************************************************************************/
/* DlgWorkerThread.cpp                    Copyright (c) Ladislav Zezula 2008 */
/*---------------------------------------------------------------------------*/
/* Worker dialog                                                             */
/*                                                                           */
/* Required resources:                                                       */
/*                                                                           */
/* * IDD_WORKER_THREAD  - Dialog template                                    */
/*  - IDC_TASK_NAME     - Text name of the task                              */
/*  - IDC_TASK_PROGRESS - Progress bar containing the amount of work done    */
/*  - IDCANCEL          - "Cancel" button                                    */
/*                                                                           */
/*---------------------------------------------------------------------------*/
/*   Date    Ver   Who  Comment                                              */
/* --------  ----  ---  -------                                              */
/* 21.03.08  1.00  Lad  The first version of DlgWorkerThread.cpp             */
/*****************************************************************************/

#include "MPQEditor.h"
#include "ITaskBarList.h"
#include "resource.h"

#ifndef PBM_SETMARQUEE
#define PBM_SETMARQUEE          (WM_USER+10)
#define PBS_MARQUEE             0x08
#endif

#define MBW_NORMAL       0
#define MBW_YNAC         1
#define MBW_ERROR        2

#define PROGRESS_FLAG_RANGE     0x00000001
#define PROGRESS_FLAG_POS       0x00000002
#define PROGRESS_FLAG_MARQUEE   0x00000004

//-----------------------------------------------------------------------------
// Local structures

struct TDialogData
{
    ITaskbarList3 * pTaskBarList;
    WORKERPROC pfnWorkerProc;
    TAnchors * pAnchors;
    LPCTSTR szTitle;
    LPVOID pvParam;
    HANDLE hThread;
    HWND hDlg;
    HWND hWndList;                          // HWND to the listview (if any)
    HWND hWndText;                          // HWND to the information text
    HWND hWndProgress;                      // HWND to the progress bar
    HWND hWndTop;                           // HWND associated with taskbar
    UINT nPrevIDText;
    int  nIconIndexOK;
    int  nIconIndexWarning;
    int  nIconIndexError;
    bool bWorkerWasCanceled;                // If TRUE, the worker was canceled
    bool bProgressBarIsRed;                 // If TRUE, the progress bar is red
    bool bShowAllFiles;                     // If TRUE, the dialog also shows all files
    bool bStayAlive;                        // If TRUE, the dialog doesn't close when the work is completed
    bool bMarquee;                          // If TRUE, the dialog is now in marquee mode

    ULONGLONG NumTotal;
    ULONG NumTotalShifted;
    ULONG NumDoneShifted;
    DWORD ShiftValue;
};

struct TProgressInfo
{
    ULONGLONG NumTotal;
    ULONGLONG NumDone;
    LPCTSTR szProgressText;
    DWORD dwFlags;
    bool bNewMarquee;
};

struct TMsgBoxData
{
    UINT_PTR nIDCaption;
    UINT_PTR nIDText;
    va_list argList;
    UINT   nResult;
    int    nError;
    int    nType;

    TCHAR szText[1];
};

//-----------------------------------------------------------------------------
// Interfaces

// Because various SDK versions either does or does not include this UID,
// we better include it under different symbol name
EXTERN_C const IID IID_ITaskbarList3_My = {0xEA1AFB91, 0x9E28, 0x4B86, {0x90, 0xE9, 0x9e, 0x9f, 0x8a, 0x5e, 0xef, 0xaf}};

static TListViewColumns Columns[] = 
{
    {IDS_FILE_NAME,    -1},
    {IDS_RAW_MD5,      80},
    {IDS_FILE_CRC32,   80},
    {IDS_FILE_MD5,     80},
    {IDS_SECTOR_CRC,   80},
    {0, 0}
};

// OldSetData: SetWindowLongPtr(hDlg, DWLP_USER, (SWL_LONG)(LONG_PTR)pData);
// OldGetData: (TDialogData *)(LONG_PTR)GetWindowLongPtr(hDlg, DWLP_USER);

static TDialogData * s_pData = NULL;
#define SET_DIALOG_DATA(hDlg,pData) s_pData = pData
#define GET_DIALOG_DATA(hDlg)  s_pData

//-----------------------------------------------------------------------------
// Local functions

static DWORD GetOptimalShiftForLargeBytes(ULONGLONG LargeValue)
{
    DWORD dwShift = 0;

    while(LargeValue >= 0x200)
    {
        LargeValue >>= 1;
        dwShift++;
    }

    return dwShift;
}

static LPCTSTR GetVerifyResultString(
    DWORD dwVerifyResult,
    DWORD dwFlagsToCheck,
    DWORD dwErrorFlag)
{
    // Get general errors
    if(dwFlagsToCheck != VERIFY_FILE_HAS_RAW_MD5)
    {
        if(dwVerifyResult & VERIFY_OPEN_ERROR)
            return _T("<open error>");
        if(dwVerifyResult & VERIFY_READ_ERROR)
            return _T("<read error>");
    }

    // If the test was not performed at all
    if((dwVerifyResult & dwFlagsToCheck) == 0)
        return _T("n/a");

    // If the check failed
    if(dwVerifyResult & dwErrorFlag)
        return _T("Failed");

    // Otherwise, return "OK"
    return _T("OK");
}

static void SetProgressBarRed(TDialogData * pData)
{
    if(pData->bProgressBarIsRed == false)
    {
        SendMessage(pData->hWndProgress, PBM_SETBARCOLOR, 0, (LPARAM)RGB(255, 0, 0));
        SendMessage(pData->hWndProgress, PBM_SETSTATE, PBST_ERROR, 0);
        pData->bProgressBarIsRed = true;
    }
}

void SetProgressMarquee(TDialogData * pData, bool bNewMarquee)
{
    if(bNewMarquee != pData->bMarquee)
    {
        DWORD dwStyle = GetWindowLong(pData->hWndProgress, GWL_STYLE);

        // Modify the style of the progress bar
        dwStyle = (bNewMarquee) ? (dwStyle | PBS_MARQUEE) : (dwStyle & ~PBS_MARQUEE);
        SetWindowLong(pData->hWndProgress, GWL_STYLE, dwStyle);
        
        // Turn the marquee on or off
        SendMessage(pData->hWndProgress, PBM_SETMARQUEE, bNewMarquee ? TRUE : FALSE, 100);

        // Update the progress on the taskbar, if supported
        if(pData->pTaskBarList != NULL)
            pData->pTaskBarList->SetProgressState(pData->hWndTop, bNewMarquee ? TBPF_INDETERMINATE : TBPF_NORMAL);

        // Remember marquee state
        pData->bMarquee = bNewMarquee;
    }
}

static void AppendErrorText(LPTSTR szText, size_t nLength, size_t nTotalLength, int nError)
{
    LPCTSTR szMpqErrorText = NULL;
    DWORD dwErrorLength = 0;

    if(nError != ERROR_SUCCESS)
    {
        // Append newline
        if(nLength < nTotalLength)
            szText[nLength++] = _T('\n');
        szText += nLength;

        // Append error code
        switch(nError)
        {
            default:
                dwErrorLength = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, NULL, nError, 0, szText, (DWORD)(nTotalLength - nLength), NULL);
                if(dwErrorLength > 0)
                {
                    while(szText[dwErrorLength - 1] == 0x0A || szText[dwErrorLength - 1] == 0x0D)
                        dwErrorLength--;
                    szText[dwErrorLength] = 0;
                }
                break;

            case ERROR_AVI_FILE:
                szMpqErrorText = _T("The file is an AVI video.");
                break;

            case ERROR_UNKNOWN_FILE_KEY:
                szMpqErrorText = _T("Unknown file decryption key.");
                break;

            case ERROR_CHECKSUM_ERROR:
                szMpqErrorText = _T("File checksum error.");
                break;

            case ERROR_INTERNAL_FILE:
                szMpqErrorText = _T("The operation is not allowed on an internal file.");
                break;

            case ERROR_BASE_FILE_MISSING:
                szMpqErrorText = _T("The base file is missing.");
                break;

            case ERROR_MARKED_FOR_DELETE:
                szMpqErrorText = _T("The file in the MPQ is marked for delete.");
                break;

            case ERROR_FILE_INCOMPLETE:
                szMpqErrorText = _T("The MPQ archive is incomplete.");
                break;

            case ERROR_UNKNOWN_FILE_NAMES:
                szMpqErrorText = _T("At least one file has an unknown name.");
                break;
        }

        // Append the MPQ error text
        if(szMpqErrorText != NULL)
            _tcscpy(szText, szMpqErrorText);

        // Append default text, if none yet.
        if(szMpqErrorText == NULL && dwErrorLength == 0)
            _stprintf(szText, _T("Error code %u (0x%08lX)."), nError, nError);
    }
}

int MessageBoxMpqError(HWND hWndParent, UINT_PTR nIDText, int nError, ...)
{
    LPCTSTR szTextFmt;
    va_list argList;
    TCHAR szTextFmtBuffer[256] = _T("");
    TCHAR szText[1024] = _T("");
    int nLength;

    // Get message text
    szTextFmt = (LPCTSTR)nIDText;
    if(IS_INTRESOURCE(nIDText))
    {
        if(LoadString(g_hInst, (UINT)nIDText, szTextFmtBuffer, _tsize(szTextFmtBuffer)) != 0)
            szTextFmt = szTextFmtBuffer;
        else
            szTextFmt = _T("* MESSAGE TEXT NOT LOADED *");
    }

    // Format the message text
    va_start(argList, nError);
    nLength = _vstprintf(szText, szTextFmt, argList);
    va_end(argList);

    // Append the error text
    AppendErrorText(szText, nLength, _tsize(szText), nError);

    // Show the messagebox
    return MessageBoxWithCheckBox(hWndParent, szText, _T("Error"), NULL, NULL, MB_OK | MB_ICONERROR);
}

//-----------------------------------------------------------------------------
// Worker thread wrapper

static DWORD WINAPI WorkerThreadProc(LPVOID pvParam)
{
    TDialogData * pData = (TDialogData *)pvParam;
    int nError = ERROR_INVALID_PARAMETER;

    if(pData->pfnWorkerProc != NULL)
        nError = pData->pfnWorkerProc(pData->hDlg, pData->pvParam);
    PostMessage(pData->hDlg, WM_WORK_COMPLETE, nError, 0);
    return 0;
}

//-----------------------------------------------------------------------------
// Message handlers

static int OnInitDialog(HWND hDlg, LPARAM lParam)
{
    TDialogData * pData = (TDialogData *)lParam;
    HIMAGELIST hIml;
    TAnchors * pAnchors = NULL;
    TCHAR szBuffer[256];
    HWND hListView;
    HWND hWndTop = hDlg;
    HICON hIcon;
    DWORD dwStyles;
    int cx = GetSystemMetrics(SM_CXSMICON);
    int cy = GetSystemMetrics(SM_CXSMICON);

    // Center the dialog to the parent
    CENTER_TO_PARENT(hDlg);

    // Apply the dialog data
    assert(s_pData == NULL);
    SET_DIALOG_DATA(hDlg, pData);

    // Get the handles to the progress bar and the information text
    pData->hWndList     = GetDlgItem(hDlg, IDC_VERIFY_RESULT);
    pData->hWndText     = GetDlgItem(hDlg, IDC_TASK_NAME);
    pData->hWndProgress = GetDlgItem(hDlg, IDC_TASK_PROGRESS);
    pData->hWndTop      = hWndTop;

    // Determine the taskbar window handle
    if((hWndTop = GetParent(hWndTop)) != NULL)
        pData->hWndTop = hWndTop;
    if((hWndTop = GetParent(hWndTop)) != NULL)
        pData->hWndTop = hWndTop;

    // Configure the resizing, if resizable
    dwStyles = GetWindowLong(hDlg, GWL_STYLE);
    if(dwStyles & WS_THICKFRAME)
    {
        pAnchors = new TAnchors();
        if(pAnchors != NULL)
        {
            pAnchors->AddAnchor(hDlg, IDC_VERIFY_RESULT, akAll);
            pAnchors->AddAnchor(hDlg, IDC_TASK_NAME, akLeft | akRight | akBottom);
            pAnchors->AddAnchor(hDlg, IDC_TASK_PROGRESS, akLeft | akRight | akBottom);
            pAnchors->AddAnchor(hDlg, IDCANCEL, akRight | akBottom);
            pData->pAnchors = pAnchors;
        }
    }

    // Set the title, if any
    if(pData->szTitle != NULL)
    {
        if(IS_INTRESOURCE(pData->szTitle))
        {
            LoadString(g_hInst, (UINT)(UINT_PTR)pData->szTitle, szBuffer, _tsize(szBuffer));
            SetWindowText(hDlg, szBuffer);
        }
        else
        {
            SetWindowText(hDlg, pData->szTitle);
        }
    }

    // If there is list view, initialize it
    hListView = GetDlgItem(hDlg, IDC_VERIFY_RESULT);
    if(hListView != NULL)
    {
        // Setup the list view
        ListView_CreateColumns(hListView, Columns);
        ListView_SetExtendedListViewStyle(hListView, LVS_EX_FULLROWSELECT | LVS_EX_INFOTIP | LVS_EX_LABELTIP);

        // Initialize the image list
        hIml = ImageList_Create(cx, cy, ILC_COLORDDB | ILC_MASK, 10, 4);
        ListView_SetImageList(hListView, hIml, LVSIL_SMALL);

        // Load the icons to the list
        hIcon = LoadIcon(NULL, MAKEINTRESOURCE(IDI_INFORMATION));
        pData->nIconIndexOK = ImageList_AddIcon(hIml, hIcon);
        hIcon = LoadIcon(NULL, MAKEINTRESOURCE(IDI_WARNING));
        pData->nIconIndexWarning = ImageList_AddIcon(hIml, hIcon);
        hIcon = LoadIcon(NULL, MAKEINTRESOURCE(IDI_ERROR));
        pData->nIconIndexError = ImageList_AddIcon(hIml, hIcon);
    }

    // For Windows 7, try to obtain ITaskBarList3 interface
    // that allows us to display progress bar on task bar
    CoCreateInstance(CLSID_TaskbarList, NULL, CLSCTX_ALL, IID_ITaskbarList3_My, (void **)&pData->pTaskBarList);
    
    // Initiate the worker thread
    PostMessage(hDlg, WM_INIT_COMPLETE, 0, 0);
    return TRUE;
}

static INT_PTR OnSize(HWND hDlg)
{
    TDialogData * pData = GET_DIALOG_DATA(hDlg);

    UNREFERENCED_PARAMETER(hDlg);

    // Rearrange the entire dialog
    if(pData != NULL && pData->pAnchors != NULL)
        pData->pAnchors->OnSize();

    // Rearrange the columns in list view
    if(pData->hWndList != NULL)
        ListView_ResizeColumns(pData->hWndList, Columns);

    return FALSE;
}

static INT_PTR OnGetMinMaxInfo(HWND hDlg, LPARAM lParam)
{
    TDialogData * pData = GET_DIALOG_DATA(hDlg);

    UNREFERENCED_PARAMETER(hDlg);

    if(pData != NULL && pData->pAnchors != NULL)
        pData->pAnchors->OnGetMinMaxInfo(lParam);

    return FALSE;
}

static INT_PTR OnInitComplete(HWND hDlg)
{
    TDialogData * pData = GET_DIALOG_DATA(hDlg);
    DWORD dwThreadID;
    int nError = ERROR_SUCCESS;

    // Fill the structure for worker thread data
    pData->hDlg = hDlg;

    // Initialize progress state on the task bar
    if(pData->pTaskBarList != NULL)
        pData->pTaskBarList->SetProgressState(pData->hWndTop, TBPF_NORMAL);

    // Create the worker thread
    pData->hThread = CreateThread(NULL, 0, WorkerThreadProc, pData, 0, &dwThreadID);
    if(pData->hThread == NULL)
        nError = GetLastError();

    // If something went wrong, terminate the work
    if(nError != ERROR_SUCCESS)
        PostMessage(hDlg, WM_WORK_COMPLETE, 0, 0);
    return TRUE;
}

static INT_PTR OnVerifyResult(HWND hDlg, DWORD dwVerifyResult, LPCTSTR szFileName)
{
    TDialogData * pData = GET_DIALOG_DATA(hDlg);
    LPCTSTR szItemText;
    HWND hListView = GetDlgItem(hDlg, IDC_VERIFY_RESULT);
    bool bShowThisFile = false;
    int iSubItem = 0;
    int nItem;
    int nIcon = pData->nIconIndexOK;

    // List view must exist at this point
    assert(hListView != NULL);

    // If the file is OK and we don't have to show all files, do nothing
    bShowThisFile = ((dwVerifyResult & VERIFY_FILE_ERROR_MASK) != 0);

    // Get the icon
    if(dwVerifyResult & (VERIFY_FILE_SECTOR_CRC_ERROR | VERIFY_FILE_CHECKSUM_ERROR | VERIFY_FILE_RAW_MD5_ERROR))
        nIcon = pData->nIconIndexWarning;
    if(dwVerifyResult & (VERIFY_OPEN_ERROR | VERIFY_READ_ERROR | VERIFY_FILE_MD5_ERROR))
        nIcon = pData->nIconIndexError;

    // Only show "OK" files if needed
    if(bShowThisFile || pData->bShowAllFiles)
    {
        // Insert the main item
        szItemText = szFileName;
        nItem = InsertLVItem(hListView, nIcon, szItemText, 0);
        iSubItem++;

        // Insert the "Raw data MD5"
        szItemText = GetVerifyResultString(dwVerifyResult, VERIFY_FILE_HAS_RAW_MD5, VERIFY_FILE_RAW_MD5_ERROR);
        ListView_SetItemText(hListView, nItem, iSubItem, (LPTSTR)szItemText);
        iSubItem++;

        // Insert the "File CRC"
        szItemText = GetVerifyResultString(dwVerifyResult, VERIFY_FILE_HAS_CHECKSUM, VERIFY_FILE_CHECKSUM_ERROR);
        ListView_SetItemText(hListView, nItem, iSubItem, (LPTSTR)szItemText);
        iSubItem++;

        // Insert the text "MD5"
        szItemText = GetVerifyResultString(dwVerifyResult, VERIFY_FILE_HAS_MD5, VERIFY_FILE_MD5_ERROR);
        ListView_SetItemText(hListView, nItem, iSubItem, (LPTSTR)szItemText);
        iSubItem++;

        // Insert the text "Sector CRC"
        szItemText = GetVerifyResultString(dwVerifyResult, VERIFY_FILE_HAS_SECTOR_CRC, VERIFY_FILE_SECTOR_CRC_ERROR);
        ListView_SetItemText(hListView, nItem, iSubItem, (LPTSTR)szItemText);
        iSubItem++;
    }

    // If an error happened, turn progress bar red
    if(nIcon == pData->nIconIndexError)
        SetProgressBarRed(pData);
    return TRUE;
}

static INT_PTR OnSetProgress(HWND hDlg, TProgressInfo * pInfo)
{
    TDialogData * pData = GET_DIALOG_DATA(hDlg);
    DWORD NumDoneShifted;

    UNREFERENCED_PARAMETER(hDlg);

    //
    // Part 1: Set the progress range
    //

    if(pInfo->dwFlags & PROGRESS_FLAG_RANGE)
    {
        // Special case: Set marquee
        if(pInfo->NumTotal != 0)
        {
            // If we have marquee, clear it
            SetProgressMarquee(pData, false);

            // Set the new range to the progress bar
            if(pInfo->NumTotal != pData->NumTotal)
            {
                // Calculate the shift value and remember the new total value
                pData->ShiftValue = GetOptimalShiftForLargeBytes(pInfo->NumTotal);
                pData->NumTotal = pInfo->NumTotal;
                
                // Also create shifted versions
                pData->NumTotalShifted = (DWORD)(pInfo->NumTotal >> pData->ShiftValue);
                pData->NumDoneShifted = 0;

                // Update the range of the progress bar
                SendMessage(pData->hWndProgress, PBM_SETRANGE32, 0, (DWORD)pData->NumTotalShifted);
                
                // Also reset the position, if we are not going to set it manually
                if(!(pInfo->dwFlags & PROGRESS_FLAG_POS))
                    SendMessage(pData->hWndProgress, PBM_SETPOS, 0, 0);
            }
        }
        else
        {
            // Special case: Set marquee
            SetProgressMarquee(pData, true);
        }
    }

    //
    // Part 2: Set the progress position
    //

    if(pInfo->dwFlags & PROGRESS_FLAG_POS)
    {
        // Calculate the new value for NumDoneShifted
        NumDoneShifted = (DWORD)(pInfo->NumDone >> pData->ShiftValue);
        if(NumDoneShifted != pData->NumDoneShifted)
        {
            // Remember the new progress value
            pData->NumDoneShifted = NumDoneShifted;

            // Update the progress indicator on the dialog.
            // Do that twice in order to fix the value in the progress bar.
            SendMessage(pData->hWndProgress, PBM_SETPOS, pData->NumDoneShifted, 0);

            // Update the progress on the taskbar, if supported
            if(pData->pTaskBarList != NULL)
                pData->pTaskBarList->SetProgressValue(pData->hWndTop, pData->NumDoneShifted, pData->NumTotalShifted);
        }
    }

    //
    // Part 3: Set the progress marquee
    //

    if(pInfo->dwFlags & PROGRESS_FLAG_MARQUEE)
    {
        SetProgressMarquee(pData, pInfo->bNewMarquee);
    }

    //
    // Part 4: Set the progress text
    //

    if(pInfo->szProgressText != NULL)
    {
        LPCTSTR szProgressText = pInfo->szProgressText;
        TCHAR szBuffer[256];

        // If the progress text is given by ID, update it
        if(IS_INTRESOURCE(szProgressText))
        {
            LoadString(g_hInst, (UINT)(UINT_PTR)pInfo->szProgressText, szBuffer, _tsize(szBuffer));
            szProgressText = szBuffer;
        }

        // Update the text in the dialog
        SetWindowText(pData->hWndText, szProgressText);
    }

    return TRUE;    
}


static INT_PTR OnMessageBox(HWND hDlg, TMsgBoxData * pMsgData)
{
    LPCTSTR szTextFmt;
    TCHAR szText[1024];
    TCHAR szTextFmtBuffer[256] = _T("");

    // Create the message text
    szTextFmt = (LPCTSTR)pMsgData->nIDText;
    if(IS_INTRESOURCE(pMsgData->nIDText))
    {
        if(LoadString(g_hInst, (UINT)pMsgData->nIDText, szTextFmtBuffer, _tsize(szTextFmtBuffer)) != 0)
            szTextFmt = szTextFmtBuffer;
        else
            szTextFmt = _T("* MESSAGE TEXT NOT LOADED *");
    }

    // Format the text
    _vstprintf(szText, szTextFmt, pMsgData->argList);

    // Show the message box as-is
    switch(pMsgData->nType)
    {
        case MBW_NORMAL:

            pMsgData->nResult = MessageBoxRc(hDlg,
                                             pMsgData->nIDCaption,
                                   (UINT_PTR)szText);
            return TRUE;
            
        case MBW_YNAC:

            pMsgData->nResult = MessageBoxYANC(hDlg,
                                               pMsgData->nIDCaption,
                                     (UINT_PTR)szText);
            return TRUE;

        case MBW_ERROR:

            // If there was an error, turn progress bar to red
            if(pMsgData->nError != ERROR_SUCCESS)
                SetProgressBarRed(GET_DIALOG_DATA(hDlg));

            // Show the error box
            pMsgData->nResult = MessageBoxMpqError(hDlg,
                                      (UINT_PTR)szText,
                                                pMsgData->nError);
            return TRUE;
    }

    // Should never happen
    assert(false);
    return TRUE;
}

static INT_PTR OnShowModalDialog(HWND hWnd, LPARAM lParam)
{
    SHOWMODALDIALOG pfnShowModalDialog;

    // Execute the modal dialog
    pfnShowModalDialog = (SHOWMODALDIALOG)lParam;
    if(pfnShowModalDialog != NULL)
        pfnShowModalDialog(hWnd);

    return 0;
}

static INT_PTR OnWorkComplete(HWND hDlg, int nError)
{
    TDialogData * pData = GET_DIALOG_DATA(hDlg);
    HWND hWndChild;
    UINT nIDText;

    // Close handle to the worker thread
    if(pData->hThread != NULL)
        CloseHandle(pData->hThread);
    pData->hThread = NULL;

    // If we have to stay alive, do it
    if(pData->bStayAlive)
    {
        // Set the proper progress text
        nIDText = (nError == ERROR_CANCELLED) ? IDS_CANCELLED : IDS_DONE;
        SetWorkerProgressText(hDlg, MAKEINTRESOURCE(nIDText));
        
        // Change the "Cancel" button to "Close"
        hWndChild = GetDlgItem(hDlg, IDCANCEL);
        SetWindowTextRc(hWndChild, IDS_CLOSE);
        return TRUE;
    }

    EndDialog(hDlg, nError);
    return TRUE;
}

static void OnGoToFile(HWND hDlg)
{
    TDialogData * pData = GET_DIALOG_DATA(hDlg);
    TTreeItem * pTreeItem;
    HWND hListView = GetDlgItem(hDlg, IDC_VERIFY_RESULT);
    int nSelected;

    // If the worker thread is running, do nothing
    if(pData->hThread != NULL)
        return;

    // Get the selected item
    nSelected = ListView_GetNextItem(hListView, -1, LVNI_SELECTED);
    if(nSelected == -1)
        return;

    // Retrieve the item
    pTreeItem = (TTreeItem *)ListView_GetItemParam(hListView, nSelected);
    if(pTreeItem != NULL)
    {
        PostMessage(pData->hWndTop, WM_GO_TO_FILE, 0, (LPARAM)pTreeItem);
        EndDialog(hDlg, IDOK);
    }
}

static void OnCancelClick(HWND hDlg)
{
    TDialogData * pData = GET_DIALOG_DATA(hDlg);
    
    if(pData->hThread != NULL)
    {
        SetWindowTextRc(pData->hWndText, IDS_CANCELLING);
        pData->bWorkerWasCanceled = true;
    }
    else
    {
        EndDialog(hDlg, IDCANCEL);
    }
}

static int OnCommand(HWND hDlg, UINT nNotify, UINT_PTR nIDCtrl)
{
    if(nNotify == BN_CLICKED)
    {
        switch(nIDCtrl)
        {
            case IDOK:
            case IDCANCEL:
                OnCancelClick(hDlg);
                return TRUE;
        }
    }

    return FALSE;
}

static int OnSysCommand(HWND /* hDlg */, WPARAM wParam, LPARAM /* lParam */)
{
    TDialogData * pData;

    if(wParam == SC_MINIMIZE)
    {
        pData = GET_DIALOG_DATA(hDlg);
        ShowWindow(pData->hWndTop, SW_SHOWMINIMIZED);
        return TRUE;
    }
    return FALSE;
}

static int OnNotify(HWND hDlg, LPNMHDR pNMHDR)
{
    switch(pNMHDR->code)
    {
        case NM_DBLCLK:
            if(pNMHDR->idFrom == IDC_VERIFY_RESULT)
                OnGoToFile(hDlg);
            return TRUE;
    }

    return FALSE;
}

static int OnDestroy(HWND hDlg)
{
    TDialogData * pData = GET_DIALOG_DATA(hDlg);

    UNREFERENCED_PARAMETER(hDlg);

    // Free the ITaskbarList interface
    if(pData->pTaskBarList != NULL)
    {
        pData->pTaskBarList->SetProgressState(pData->hWndTop, TBPF_NOPROGRESS);
        pData->pTaskBarList->Release();
        pData->pTaskBarList = NULL;
    }

    // Delete the archor class
    if(pData->pAnchors != NULL)
        delete pData->pAnchors;
    pData->pAnchors = NULL;
    
    // Destrou the dialog data
    SET_DIALOG_DATA(hDlg, NULL);
    return FALSE;
}

static INT_PTR CALLBACK DialogProc(HWND hDlg, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    switch(uMsg)
    {
        case WM_INITDIALOG:
            return OnInitDialog(hDlg, lParam);

        case WM_SIZE:
            return OnSize(hDlg);

        case WM_GETMINMAXINFO:
            return OnGetMinMaxInfo(hDlg, lParam);

        case WM_INIT_COMPLETE:
            return OnInitComplete(hDlg);

        case WM_SET_VERIFY_RESULT:
            return OnVerifyResult(hDlg, (DWORD)wParam, (LPCTSTR)lParam);

        case WM_SET_PROGRESS:
            return OnSetProgress(hDlg, (TProgressInfo *)lParam);

        case WM_MESSAGE_BOX:
            return OnMessageBox(hDlg, (TMsgBoxData *)lParam);

        case WM_SHOW_MODAL_DIALOG:
            return OnShowModalDialog(hDlg, lParam);

        case WM_WORK_COMPLETE:
            return OnWorkComplete(hDlg, (int)wParam);

        case WM_COMMAND:
            return OnCommand(hDlg, WMC_NOTIFY(wParam), WMC_CTRLID(wParam));

        case WM_SYSCOMMAND:
            return OnSysCommand(hDlg, wParam, lParam);

        case WM_NOTIFY:
            return OnNotify(hDlg, (LPNMHDR)lParam);

        case WM_DESTROY:
            return OnDestroy(hDlg);
    }
    return FALSE;
}

//-----------------------------------------------------------------------------
// Public functions

void SetWorkerProgressRange(HWND hDlg, LPCTSTR szProgressText, ULONGLONG NumTotal)
{
    TProgressInfo Info = {0};

    Info.szProgressText = szProgressText;
    Info.NumTotal = NumTotal;
    Info.dwFlags = PROGRESS_FLAG_RANGE;
    OnSetProgress(hDlg, &Info);
//  SendMessage(hDlg, WM_SET_PROGRESS, 0, (LPARAM)&Info);
}

void SetWorkerProgressText(HWND hDlg, LPCTSTR szProgressText)
{
    TProgressInfo Info = {0};

    Info.szProgressText = szProgressText;
    Info.dwFlags = 0;
    OnSetProgress(hDlg, &Info);
//  SendMessage(hDlg, WM_SET_PROGRESS, 0, (LPARAM)&Info);
}

void SetWorkerProgress(HWND hDlg, LPCTSTR szProgressText, ULONGLONG NumDone)
{
    TProgressInfo Info = {0};

    Info.szProgressText = szProgressText;
    Info.NumDone = NumDone;
    Info.dwFlags = PROGRESS_FLAG_POS;
    OnSetProgress(hDlg, &Info);
//  SendMessage(hDlg, WM_SET_PROGRESS, 0, (LPARAM)&Info);
}

void SetWorkerMarquee(HWND hDlg, LPCTSTR szProgressText, BOOL bNewMarquee)
{
    TProgressInfo Info = {0};

    Info.szProgressText = szProgressText;
    Info.bNewMarquee = bNewMarquee ? true : false;
    Info.dwFlags = PROGRESS_FLAG_MARQUEE;
    OnSetProgress(hDlg, &Info);
//  SendMessage(hDlg, WM_SET_PROGRESS, 0, (LPARAM)&Info);
}

// Message box intended for use by worker threads; it is shown by the main GUI,
// instead of by the worker thread. This was fixed for WINE, which can't properly
// handle child windows being shown improperly if they are created by different
// thread than the parent
int Worker_MessageBoxRc(HWND hParent, UINT_PTR nIDCaption, UINT_PTR nIDText, ...)
{
    TMsgBoxData Data;
    va_list argList;

    // Fill the data structure and send it to the main thread
    va_start(argList, nIDText);
    Data.nIDCaption = nIDCaption;
    Data.nIDText = nIDText;
    Data.argList = argList;
    Data.nType = MBW_NORMAL;
    SendMessage(hParent, WM_MESSAGE_BOX, 0, (LPARAM)&Data);
    va_end(argList);

    // Get the result
    return Data.nResult;
}

int Worker_MessageBoxYANC(HWND hParent, UINT_PTR nIDCaption, UINT_PTR nIDText, ...)
{
    TMsgBoxData Data;
    va_list argList;

    // Fill the data structure and send it to the main thread
    va_start(argList, nIDText);
    Data.nIDCaption = nIDCaption;
    Data.nIDText = nIDText;
    Data.argList = argList;
    Data.nType = MBW_YNAC;
    SendMessage(hParent, WM_MESSAGE_BOX, 0, (LPARAM)&Data);
    va_end(argList);

    // Get the result
    return Data.nResult;
}

int Worker_MessageBoxError(HWND hParent, UINT_PTR nIDText, int nError, ...)
{
    TMsgBoxData Data;
    va_list argList;

    // Fill the data structure and send it to the main thread
    va_start(argList, nError);
    Data.nIDCaption = 0;
    Data.nIDText = nIDText;
    Data.argList = argList;
    Data.nError = nError;
    Data.nType = MBW_ERROR;
    SendMessage(hParent, WM_MESSAGE_BOX, 0, (LPARAM)&Data);
    va_end(argList);

    // Get the result
    return Data.nResult;
}

void EnableWorkerCancelButton(HWND hDlg, BOOL bEnable)
{
    EnableDlgItems(hDlg, bEnable, IDCANCEL, 0);
}

bool WorkerWasCancelled(HWND hDlg)
{
    TDialogData * pData = GET_DIALOG_DATA(hDlg);

    UNREFERENCED_PARAMETER(hDlg);
    return (pData != NULL && pData->bWorkerWasCanceled) ? true : false;
}

void SetWorkerCancelState(HWND hDlg, bool bCancelState)
{
    TDialogData * pData = GET_DIALOG_DATA(hDlg);

    UNREFERENCED_PARAMETER(hDlg);
    if(pData != NULL)
        pData->bWorkerWasCanceled = bCancelState;
}

INT_PTR WorkerDialog(HWND hParent, UINT nIDTitle, WORKERPROC pfnWorkerProc, LPVOID pvParam)
{
    TDialogData Data;

    // Execute the worker dialog
    memset(&Data, 0, sizeof(TDialogData));
    Data.pfnWorkerProc = pfnWorkerProc;
    Data.pvParam       = pvParam;
    Data.szTitle       = MAKEINTRESOURCE(nIDTitle);
    return DialogBoxParam(g_hInst, MAKEINTRESOURCE(IDD_WORKER_THREAD), hParent, DialogProc, (LPARAM)&Data);
}

INT_PTR WorkerWaitForApp(HWND hParent, UINT nIDTitle, WORKERPROC pfnWorkerProc, LPVOID pvParam)
{
    TDialogData Data;

    // Execute the worker dialog
    memset(&Data, 0, sizeof(TDialogData));
    Data.pfnWorkerProc = pfnWorkerProc;
    Data.pvParam       = pvParam;
    Data.szTitle       = MAKEINTRESOURCE(nIDTitle);
    return DialogBoxParam(g_hInst, MAKEINTRESOURCE(IDD_WORKER_WAIT_APP), hParent, DialogProc, (LPARAM)&Data);
}

INT_PTR WorkerDialogVerify(HWND hParent, UINT nIDTitle, WORKERPROC pfnWorkerProc, LPVOID pvParam, bool bShowAllFiles)
{
    TDialogData Data;

    // Execute the worker dialog
    memset(&Data, 0, sizeof(TDialogData));
    Data.pfnWorkerProc = pfnWorkerProc;
    Data.pvParam       = pvParam;
    Data.szTitle       = MAKEINTRESOURCE(nIDTitle);
    Data.bShowAllFiles = bShowAllFiles;
    Data.bStayAlive    = TRUE;
    return DialogBoxParam(g_hInst, MAKEINTRESOURCE(IDD_WORKER_VERIFY), hParent, DialogProc, (LPARAM)&Data);
}
