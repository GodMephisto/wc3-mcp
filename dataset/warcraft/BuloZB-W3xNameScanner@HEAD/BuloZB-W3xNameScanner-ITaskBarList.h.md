---
type: vendor-doc
source_repo: "https://github.com/BuloZB/W3xNameScanner"
source_path: "ITaskBarList.h"
ref: HEAD
---

/*****************************************************************************/
/* ITaskBarList.h                         Copyright (c) Ladislav Zezula 2010 */
/*---------------------------------------------------------------------------*/
/* Description:                                                              */
/*---------------------------------------------------------------------------*/
/*   Date    Ver   Who  Comment                                              */
/* --------  ----  ---  -------                                              */
/* 26.03.10  1.00  Lad  The first version of ITaskBarList.h                  */
/*****************************************************************************/

#ifndef __ITASKBARLIST_H__
#define __ITASKBARLIST_H__

#ifndef __ITaskbarList_FWD_DEFINED__
#define __ITaskbarList_FWD_DEFINED__
typedef interface ITaskbarList ITaskbarList;
#endif 	/* __ITaskbarList_FWD_DEFINED__ */


#ifndef __ITaskbarList2_FWD_DEFINED__
#define __ITaskbarList2_FWD_DEFINED__
typedef interface ITaskbarList2 ITaskbarList2;
#endif 	/* __ITaskbarList2_FWD_DEFINED__ */


#ifndef __ITaskbarList3_FWD_DEFINED__
#define __ITaskbarList3_FWD_DEFINED__
typedef interface ITaskbarList3 ITaskbarList3;
#endif 	/* __ITaskbarList3_FWD_DEFINED__ */


#ifndef __ITaskbarList4_FWD_DEFINED__
#define __ITaskbarList4_FWD_DEFINED__
typedef interface ITaskbarList4 ITaskbarList4;
#endif 	/* __ITaskbarList4_FWD_DEFINED__ */

#ifndef __ITaskbarList_INTERFACE_DEFINED__
#define __ITaskbarList_INTERFACE_DEFINED__

/* interface ITaskbarList */
/* [object][uuid] */ 


EXTERN_C const IID IID_ITaskbarList;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("56FDF342-FD6D-11d0-958A-006097C9A090")
    ITaskbarList : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE HrInit( void) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE AddTab( 
            /* [in] */ HWND hwnd) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DeleteTab( 
            /* [in] */ HWND hwnd) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ActivateTab( 
            /* [in] */ HWND hwnd) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetActiveAlt( 
            /* [in] */ HWND hwnd) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ITaskbarListVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ITaskbarList * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ITaskbarList * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ITaskbarList * This);
        
        HRESULT ( STDMETHODCALLTYPE *HrInit )( 
            ITaskbarList * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddTab )( 
            ITaskbarList * This,
            /* [in] */ HWND hwnd);
        
        HRESULT ( STDMETHODCALLTYPE *DeleteTab )( 
            ITaskbarList * This,
            /* [in] */ HWND hwnd);
        
        HRESULT ( STDMETHODCALLTYPE *ActivateTab )( 
            ITaskbarList * This,
            /* [in] */ HWND hwnd);
        
        HRESULT ( STDMETHODCALLTYPE *SetActiveAlt )( 
            ITaskbarList * This,
            /* [in] */ HWND hwnd);
        
        END_INTERFACE
    } ITaskbarListVtbl;

    interface ITaskbarList
    {
        CONST_VTBL struct ITaskbarListVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ITaskbarList_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ITaskbarList_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ITaskbarList_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ITaskbarList_HrInit(This)	\
    ( (This)->lpVtbl -> HrInit(This) ) 

#define ITaskbarList_AddTab(This,hwnd)	\
    ( (This)->lpVtbl -> AddTab(This,hwnd) ) 

#define ITaskbarList_DeleteTab(This,hwnd)	\
    ( (This)->lpVtbl -> DeleteTab(This,hwnd) ) 

#define ITaskbarList_ActivateTab(This,hwnd)	\
    ( (This)->lpVtbl -> ActivateTab(This,hwnd) ) 

#define ITaskbarList_SetActiveAlt(This,hwnd)	\
    ( (This)->lpVtbl -> SetActiveAlt(This,hwnd) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ITaskbarList_INTERFACE_DEFINED__ */


#ifndef __ITaskbarList2_INTERFACE_DEFINED__
#define __ITaskbarList2_INTERFACE_DEFINED__

/* interface ITaskbarList2 */
/* [object][uuid] */ 


EXTERN_C const IID IID_ITaskbarList2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("602D4995-B13A-429b-A66E-1935E44F4317")
    ITaskbarList2 : public ITaskbarList
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE MarkFullscreenWindow( 
            /* [in] */ HWND hwnd,
            /* [in] */ BOOL fFullscreen) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ITaskbarList2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ITaskbarList2 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ITaskbarList2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ITaskbarList2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *HrInit )( 
            ITaskbarList2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddTab )( 
            ITaskbarList2 * This,
            /* [in] */ HWND hwnd);
        
        HRESULT ( STDMETHODCALLTYPE *DeleteTab )( 
            ITaskbarList2 * This,
            /* [in] */ HWND hwnd);
        
        HRESULT ( STDMETHODCALLTYPE *ActivateTab )( 
            ITaskbarList2 * This,
            /* [in] */ HWND hwnd);
        
        HRESULT ( STDMETHODCALLTYPE *SetActiveAlt )( 
            ITaskbarList2 * This,
            /* [in] */ HWND hwnd);
        
        HRESULT ( STDMETHODCALLTYPE *MarkFullscreenWindow )( 
            ITaskbarList2 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ BOOL fFullscreen);
        
        END_INTERFACE
    } ITaskbarList2Vtbl;

    interface ITaskbarList2
    {
        CONST_VTBL struct ITaskbarList2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ITaskbarList2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ITaskbarList2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ITaskbarList2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ITaskbarList2_HrInit(This)	\
    ( (This)->lpVtbl -> HrInit(This) ) 

#define ITaskbarList2_AddTab(This,hwnd)	\
    ( (This)->lpVtbl -> AddTab(This,hwnd) ) 

#define ITaskbarList2_DeleteTab(This,hwnd)	\
    ( (This)->lpVtbl -> DeleteTab(This,hwnd) ) 

#define ITaskbarList2_ActivateTab(This,hwnd)	\
    ( (This)->lpVtbl -> ActivateTab(This,hwnd) ) 

#define ITaskbarList2_SetActiveAlt(This,hwnd)	\
    ( (This)->lpVtbl -> SetActiveAlt(This,hwnd) ) 


#define ITaskbarList2_MarkFullscreenWindow(This,hwnd,fFullscreen)	\
    ( (This)->lpVtbl -> MarkFullscreenWindow(This,hwnd,fFullscreen) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ITaskbarList2_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_shobjidl_0000_0093 */
/* [local] */ 

#ifdef MIDL_PASS
typedef IUnknown *HIMAGELIST;

#endif

#define THBN_CLICKED        0x1800

extern RPC_IF_HANDLE __MIDL_itf_shobjidl_0000_0093_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_shobjidl_0000_0093_v0_0_s_ifspec;

#ifndef __ITaskbarList3_INTERFACE_DEFINED__
#define __ITaskbarList3_INTERFACE_DEFINED__

/* interface ITaskbarList3 */
/* [object][uuid] */ 

typedef /* [v1_enum] */ 
enum TBPFLAG
    {	TBPF_NOPROGRESS	= 0,
	TBPF_INDETERMINATE	= 0x1,
	TBPF_NORMAL	= 0x2,
	TBPF_ERROR	= 0x4,
	TBPF_PAUSED	= 0x8
    } 	TBPFLAG;

EXTERN_C const IID IID_ITaskbarList3;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")
    ITaskbarList3 : public ITaskbarList2
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetProgressValue( 
            /* [in] */ HWND hwnd,
            /* [in] */ ULONGLONG ullCompleted,
            /* [in] */ ULONGLONG ullTotal) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetProgressState( 
            /* [in] */ HWND hwnd,
            /* [in] */ TBPFLAG tbpFlags) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE RegisterTab( 
            /* [in] */ HWND hwndTab,
            /* [in] */ HWND hwndMDI) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UnregisterTab( 
            /* [in] */ HWND hwndTab) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetTabOrder( 
            /* [in] */ HWND hwndTab,
            /* [in] */ HWND hwndInsertBefore) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetTabActive( 
            /* [in] */ HWND hwndTab,
            /* [in] */ HWND hwndMDI,
            /* [in] */ DWORD dwReserved) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ThumbBarAddButtons( 
            /* [in] */ HWND hwnd,
            /* [in] */ UINT cButtons,
            /* [size_is][in] */ LPVOID pButton) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ThumbBarUpdateButtons( 
            /* [in] */ HWND hwnd,
            /* [in] */ UINT cButtons,
            /* [size_is][in] */ LPVOID pButton) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE ThumbBarSetImageList( 
            /* [in] */ HWND hwnd,
            /* [in] */ HIMAGELIST himl) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetOverlayIcon( 
            /* [in] */ HWND hwnd,
            /* [in] */ HICON hIcon,
            /* [string][unique][in] */ LPCWSTR pszDescription) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetThumbnailTooltip( 
            /* [in] */ HWND hwnd,
            /* [string][unique][in] */ LPCWSTR pszTip) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE SetThumbnailClip( 
            /* [in] */ HWND hwnd,
            /* [in] */ RECT *prcClip) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ITaskbarList3Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ITaskbarList3 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ITaskbarList3 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ITaskbarList3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *HrInit )( 
            ITaskbarList3 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddTab )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwnd);
        
        HRESULT ( STDMETHODCALLTYPE *DeleteTab )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwnd);
        
        HRESULT ( STDMETHODCALLTYPE *ActivateTab )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwnd);
        
        HRESULT ( STDMETHODCALLTYPE *SetActiveAlt )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwnd);
        
        HRESULT ( STDMETHODCALLTYPE *MarkFullscreenWindow )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ BOOL fFullscreen);
        
        HRESULT ( STDMETHODCALLTYPE *SetProgressValue )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ ULONGLONG ullCompleted,
            /* [in] */ ULONGLONG ullTotal);
        
        HRESULT ( STDMETHODCALLTYPE *SetProgressState )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ TBPFLAG tbpFlags);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterTab )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwndTab,
            /* [in] */ HWND hwndMDI);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterTab )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwndTab);
        
        HRESULT ( STDMETHODCALLTYPE *SetTabOrder )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwndTab,
            /* [in] */ HWND hwndInsertBefore);
        
        HRESULT ( STDMETHODCALLTYPE *SetTabActive )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwndTab,
            /* [in] */ HWND hwndMDI,
            /* [in] */ DWORD dwReserved);
        
        HRESULT ( STDMETHODCALLTYPE *ThumbBarAddButtons )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ UINT cButtons,
            /* [size_is][in] */ LPVOID pButton);
        
        HRESULT ( STDMETHODCALLTYPE *ThumbBarUpdateButtons )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ UINT cButtons,
            /* [size_is][in] */ LPVOID pButton);
        
        HRESULT ( STDMETHODCALLTYPE *ThumbBarSetImageList )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ HIMAGELIST himl);
        
        HRESULT ( STDMETHODCALLTYPE *SetOverlayIcon )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ HICON hIcon,
            /* [string][unique][in] */ LPCWSTR pszDescription);
        
        HRESULT ( STDMETHODCALLTYPE *SetThumbnailTooltip )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwnd,
            /* [string][unique][in] */ LPCWSTR pszTip);
        
        HRESULT ( STDMETHODCALLTYPE *SetThumbnailClip )( 
            ITaskbarList3 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ RECT *prcClip);
        
        END_INTERFACE
    } ITaskbarList3Vtbl;

    interface ITaskbarList3
    {
        CONST_VTBL struct ITaskbarList3Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ITaskbarList3_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ITaskbarList3_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ITaskbarList3_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ITaskbarList3_HrInit(This)	\
    ( (This)->lpVtbl -> HrInit(This) ) 

#define ITaskbarList3_AddTab(This,hwnd)	\
    ( (This)->lpVtbl -> AddTab(This,hwnd) ) 

#define ITaskbarList3_DeleteTab(This,hwnd)	\
    ( (This)->lpVtbl -> DeleteTab(This,hwnd) ) 

#define ITaskbarList3_ActivateTab(This,hwnd)	\
    ( (This)->lpVtbl -> ActivateTab(This,hwnd) ) 

#define ITaskbarList3_SetActiveAlt(This,hwnd)	\
    ( (This)->lpVtbl -> SetActiveAlt(This,hwnd) ) 


#define ITaskbarList3_MarkFullscreenWindow(This,hwnd,fFullscreen)	\
    ( (This)->lpVtbl -> MarkFullscreenWindow(This,hwnd,fFullscreen) ) 


#define ITaskbarList3_SetProgressValue(This,hwnd,ullCompleted,ullTotal)	\
    ( (This)->lpVtbl -> SetProgressValue(This,hwnd,ullCompleted,ullTotal) ) 

#define ITaskbarList3_SetProgressState(This,hwnd,tbpFlags)	\
    ( (This)->lpVtbl -> SetProgressState(This,hwnd,tbpFlags) ) 

#define ITaskbarList3_RegisterTab(This,hwndTab,hwndMDI)	\
    ( (This)->lpVtbl -> RegisterTab(This,hwndTab,hwndMDI) ) 

#define ITaskbarList3_UnregisterTab(This,hwndTab)	\
    ( (This)->lpVtbl -> UnregisterTab(This,hwndTab) ) 

#define ITaskbarList3_SetTabOrder(This,hwndTab,hwndInsertBefore)	\
    ( (This)->lpVtbl -> SetTabOrder(This,hwndTab,hwndInsertBefore) ) 

#define ITaskbarList3_SetTabActive(This,hwndTab,hwndMDI,dwReserved)	\
    ( (This)->lpVtbl -> SetTabActive(This,hwndTab,hwndMDI,dwReserved) ) 

#define ITaskbarList3_ThumbBarAddButtons(This,hwnd,cButtons,pButton)	\
    ( (This)->lpVtbl -> ThumbBarAddButtons(This,hwnd,cButtons,pButton) ) 

#define ITaskbarList3_ThumbBarUpdateButtons(This,hwnd,cButtons,pButton)	\
    ( (This)->lpVtbl -> ThumbBarUpdateButtons(This,hwnd,cButtons,pButton) ) 

#define ITaskbarList3_ThumbBarSetImageList(This,hwnd,himl)	\
    ( (This)->lpVtbl -> ThumbBarSetImageList(This,hwnd,himl) ) 

#define ITaskbarList3_SetOverlayIcon(This,hwnd,hIcon,pszDescription)	\
    ( (This)->lpVtbl -> SetOverlayIcon(This,hwnd,hIcon,pszDescription) ) 

#define ITaskbarList3_SetThumbnailTooltip(This,hwnd,pszTip)	\
    ( (This)->lpVtbl -> SetThumbnailTooltip(This,hwnd,pszTip) ) 

#define ITaskbarList3_SetThumbnailClip(This,hwnd,prcClip)	\
    ( (This)->lpVtbl -> SetThumbnailClip(This,hwnd,prcClip) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ITaskbarList3_INTERFACE_DEFINED__ */


#ifndef __ITaskbarList4_INTERFACE_DEFINED__
#define __ITaskbarList4_INTERFACE_DEFINED__

/* interface ITaskbarList4 */
/* [object][uuid] */ 

typedef /* [v1_enum] */ 
enum STPFLAG
    {	STPF_NONE	= 0,
	STPF_USEAPPTHUMBNAILALWAYS	= 0x1,
	STPF_USEAPPTHUMBNAILWHENACTIVE	= 0x2,
	STPF_USEAPPPEEKALWAYS	= 0x4,
	STPF_USEAPPPEEKWHENACTIVE	= 0x8
    } 	STPFLAG;

EXTERN_C const IID IID_ITaskbarList4;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("c43dc798-95d1-4bea-9030-bb99e2983a1a")
    ITaskbarList4 : public ITaskbarList3
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE SetTabProperties( 
            /* [in] */ HWND hwndTab,
            /* [in] */ STPFLAG stpFlags) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ITaskbarList4Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ITaskbarList4 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ITaskbarList4 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ITaskbarList4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *HrInit )( 
            ITaskbarList4 * This);
        
        HRESULT ( STDMETHODCALLTYPE *AddTab )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwnd);
        
        HRESULT ( STDMETHODCALLTYPE *DeleteTab )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwnd);
        
        HRESULT ( STDMETHODCALLTYPE *ActivateTab )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwnd);
        
        HRESULT ( STDMETHODCALLTYPE *SetActiveAlt )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwnd);
        
        HRESULT ( STDMETHODCALLTYPE *MarkFullscreenWindow )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ BOOL fFullscreen);
        
        HRESULT ( STDMETHODCALLTYPE *SetProgressValue )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ ULONGLONG ullCompleted,
            /* [in] */ ULONGLONG ullTotal);
        
        HRESULT ( STDMETHODCALLTYPE *SetProgressState )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ TBPFLAG tbpFlags);
        
        HRESULT ( STDMETHODCALLTYPE *RegisterTab )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwndTab,
            /* [in] */ HWND hwndMDI);
        
        HRESULT ( STDMETHODCALLTYPE *UnregisterTab )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwndTab);
        
        HRESULT ( STDMETHODCALLTYPE *SetTabOrder )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwndTab,
            /* [in] */ HWND hwndInsertBefore);
        
        HRESULT ( STDMETHODCALLTYPE *SetTabActive )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwndTab,
            /* [in] */ HWND hwndMDI,
            /* [in] */ DWORD dwReserved);
        
        HRESULT ( STDMETHODCALLTYPE *ThumbBarAddButtons )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ UINT cButtons,
            /* [size_is][in] */ LPVOID pButton);
        
        HRESULT ( STDMETHODCALLTYPE *ThumbBarUpdateButtons )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ UINT cButtons,
            /* [size_is][in] */ LPVOID pButton);
        
        HRESULT ( STDMETHODCALLTYPE *ThumbBarSetImageList )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ HIMAGELIST himl);
        
        HRESULT ( STDMETHODCALLTYPE *SetOverlayIcon )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ HICON hIcon,
            /* [string][unique][in] */ LPCWSTR pszDescription);
        
        HRESULT ( STDMETHODCALLTYPE *SetThumbnailTooltip )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwnd,
            /* [string][unique][in] */ LPCWSTR pszTip);
        
        HRESULT ( STDMETHODCALLTYPE *SetThumbnailClip )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwnd,
            /* [in] */ RECT *prcClip);
        
        HRESULT ( STDMETHODCALLTYPE *SetTabProperties )( 
            ITaskbarList4 * This,
            /* [in] */ HWND hwndTab,
            /* [in] */ STPFLAG stpFlags);
        
        END_INTERFACE
    } ITaskbarList4Vtbl;

    interface ITaskbarList4
    {
        CONST_VTBL struct ITaskbarList4Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ITaskbarList4_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ITaskbarList4_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ITaskbarList4_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ITaskbarList4_HrInit(This)	\
    ( (This)->lpVtbl -> HrInit(This) ) 

#define ITaskbarList4_AddTab(This,hwnd)	\
    ( (This)->lpVtbl -> AddTab(This,hwnd) ) 

#define ITaskbarList4_DeleteTab(This,hwnd)	\
    ( (This)->lpVtbl -> DeleteTab(This,hwnd) ) 

#define ITaskbarList4_ActivateTab(This,hwnd)	\
    ( (This)->lpVtbl -> ActivateTab(This,hwnd) ) 

#define ITaskbarList4_SetActiveAlt(This,hwnd)	\
    ( (This)->lpVtbl -> SetActiveAlt(This,hwnd) ) 


#define ITaskbarList4_MarkFullscreenWindow(This,hwnd,fFullscreen)	\
    ( (This)->lpVtbl -> MarkFullscreenWindow(This,hwnd,fFullscreen) ) 


#define ITaskbarList4_SetProgressValue(This,hwnd,ullCompleted,ullTotal)	\
    ( (This)->lpVtbl -> SetProgressValue(This,hwnd,ullCompleted,ullTotal) ) 

#define ITaskbarList4_SetProgressState(This,hwnd,tbpFlags)	\
    ( (This)->lpVtbl -> SetProgressState(This,hwnd,tbpFlags) ) 

#define ITaskbarList4_RegisterTab(This,hwndTab,hwndMDI)	\
    ( (This)->lpVtbl -> RegisterTab(This,hwndTab,hwndMDI) ) 

#define ITaskbarList4_UnregisterTab(This,hwndTab)	\
    ( (This)->lpVtbl -> UnregisterTab(This,hwndTab) ) 

#define ITaskbarList4_SetTabOrder(This,hwndTab,hwndInsertBefore)	\
    ( (This)->lpVtbl -> SetTabOrder(This,hwndTab,hwndInsertBefore) ) 

#define ITaskbarList4_SetTabActive(This,hwndTab,hwndMDI,dwReserved)	\
    ( (This)->lpVtbl -> SetTabActive(This,hwndTab,hwndMDI,dwReserved) ) 

#define ITaskbarList4_ThumbBarAddButtons(This,hwnd,cButtons,pButton)	\
    ( (This)->lpVtbl -> ThumbBarAddButtons(This,hwnd,cButtons,pButton) ) 

#define ITaskbarList4_ThumbBarUpdateButtons(This,hwnd,cButtons,pButton)	\
    ( (This)->lpVtbl -> ThumbBarUpdateButtons(This,hwnd,cButtons,pButton) ) 

#define ITaskbarList4_ThumbBarSetImageList(This,hwnd,himl)	\
    ( (This)->lpVtbl -> ThumbBarSetImageList(This,hwnd,himl) ) 

#define ITaskbarList4_SetOverlayIcon(This,hwnd,hIcon,pszDescription)	\
    ( (This)->lpVtbl -> SetOverlayIcon(This,hwnd,hIcon,pszDescription) ) 

#define ITaskbarList4_SetThumbnailTooltip(This,hwnd,pszTip)	\
    ( (This)->lpVtbl -> SetThumbnailTooltip(This,hwnd,pszTip) ) 

#define ITaskbarList4_SetThumbnailClip(This,hwnd,prcClip)	\
    ( (This)->lpVtbl -> SetThumbnailClip(This,hwnd,prcClip) ) 


#define ITaskbarList4_SetTabProperties(This,hwndTab,stpFlags)	\
    ( (This)->lpVtbl -> SetTabProperties(This,hwndTab,stpFlags) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */

#endif 	/* __ITaskbarList4_INTERFACE_DEFINED__ */
#endif // __ITASKBARLIST_H__
