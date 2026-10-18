---
type: vendor-doc
source_repo: "https://github.com/warcraft-iii/war3map.cpp"
source_path: "include/war3map.cpp/JassAPI.h"
ref: HEAD
---

﻿#pragma once

#include <cstdint>
#include <type_traits>
#include <string>
#include <functional>
#include <Windows.h>

#define WA3MAPCPP_FORCE_INLINE __forceinline
#define WAR3MAP_FUNC __declspec(dllexport) __stdcall
#define JASSAPI __cdecl

using JCALLBACK = std::function<void()>;

namespace war3mapcpp::detail
{
    namespace
    {
        template <class OutputClass, class InputClass>
        struct same_size
        {
            static const bool value =
                (!std::is_reference<InputClass>::value && sizeof(OutputClass) == sizeof(InputClass)) || (std::is_reference<InputClass>::value && sizeof(OutputClass) == sizeof(std::add_pointer<InputClass>::type));
        };

        template <class OutputClass, class InputClass>
        union cast_union
        {
            OutputClass out;
            InputClass in;
        };

        template <class Argument>
        inline uintptr_t cast(const Argument input, typename std::enable_if<std::is_function<typename std::remove_reference<Argument>::type>::value, void>::type * = 0)
        {
            cast_union<uintptr_t, Argument> u;
            static_assert(std::is_pod<Argument>::value, "Argument is not a pod.");
            static_assert((sizeof(Argument) == sizeof(u)) && (sizeof(Argument) == sizeof(uintptr_t)), "Argument and uintptr_t are not the same size.");
            u.in = input;
            return u.out;
        }

        template <class Argument>
        inline uintptr_t cast(const Argument input, typename std::enable_if<!std::is_function<typename std::remove_reference<Argument>::type>::value && same_size<uintptr_t, Argument>::value, void>::type * = 0)
        {
            cast_union<uintptr_t, Argument> u;
            static_assert(std::is_pod<Argument>::value, "Argument is not a pod.");
            u.in = input;
            return u.out;
        }

        template <class Argument>
        inline uintptr_t cast(const Argument input, typename std::enable_if<!std::is_function<typename std::remove_reference<Argument>::type>::value && !same_size<uintptr_t, Argument>::value, void>::type * = 0)
        {
            static_assert(std::is_pod<Argument>::value, "Argument is not a pod.");
            static_assert(sizeof(Argument) < sizeof(uintptr_t), "Argument can not be converted to uintptr_t.");
            return static_cast<uintptr_t>(input);
        }

        template <typename Arg>
        struct cast_type
        {
            typedef uintptr_t type;
        };

        template <typename R, typename F, typename... Args>
        inline R std_call(F f, Args... args)
        {
            return (reinterpret_cast<R(__stdcall *)(typename cast_type<Args>::type... args)>(f))(cast(args)...);
        }

        template <typename R, typename F, typename... Args>
        inline R fast_call(F f, Args... args)
        {
            return (reinterpret_cast<R(__fastcall *)(typename cast_type<Args>::type... args)>(f))(cast(args)...);
        }

        template <typename R, typename F, typename... Args>
        inline R c_call(F f, Args... args)
        {
            return (reinterpret_cast<R(__cdecl *)(typename cast_type<Args>::type... args)>(f))(cast(args)...);
        }

        template <typename R, typename F, typename This, typename... Args>
        inline R this_call(F f, This t, Args... args)
        {
            return (reinterpret_cast<R(__fastcall *)(typename cast_type<This>::type, void *, typename cast_type<Args>::type... args)>(f))(cast(t), 0, cast(args)...);
        }
    }

    namespace invoke
    {
        template <class _Tp>
        inline constexpr _Tp &&Forward(_Tp &__t) noexcept { return static_cast<_Tp &&>(__t); }

        template <class _Tp>
        inline constexpr _Tp &&Forward(_Tp &&__t) noexcept { return static_cast<_Tp &&>(__t); }

        static uintptr_t GetAddress(uintptr_t offset)
        {
            static uintptr_t gamedll = (uintptr_t)GetModuleHandleA("Game.dll");
            return gamedll + offset;
        }

        static uintptr_t FindNative(const char *name)
        {
            auto nativeFunc = war3mapcpp::detail::fast_call<int>(GetAddress(0x7E2FE0), name);
            if (nativeFunc)
            {
                return *(uintptr_t *)(nativeFunc + 0x1c);
            }
            return 0;
        }

        // 参数转发工具：将 float 左值转换为 float*
        template <typename T>
        decltype(auto) forward_arg(T &&arg)
        {
            using DecayT = std::decay_t<T>;
            if constexpr (std::is_same_v<DecayT, float>)
            {
                // 确保只有左值会被转换
                static_assert(
                    std::is_lvalue_reference_v<decltype(arg)>,
                    "Float parameters must be lvalues to convert to pointers.");
                return &arg; // 返回 float*
            }
            else if constexpr (std::is_same_v<DecayT, JCALLBACK>)
            {
                return war3mapcpp::api::CreateCode(arg);
            }
            else
            {
                return std::forward<T>(arg); // 其他类型原样转发
            }
        };

        template <typename ReturnType>
        struct api
        {
            template <typename... Args>
            static WA3MAPCPP_FORCE_INLINE auto call(uintptr_t address, Args &&...args) -> ReturnType
            {
                ReturnType ret_value = war3mapcpp::detail::c_call<ReturnType>(address, forward_arg(invoke::Forward<Args>(args))...);
                return ret_value;
            }
        };

        template <>
        struct api<void>
        {
            template <typename... Args>
            static WA3MAPCPP_FORCE_INLINE auto call(uintptr_t address, Args &&...args) -> void
            {
                war3mapcpp::detail::c_call<void>(address, forward_arg(invoke::Forward<Args>(args))...);
            }
        };
    }
}

// Marshall an API call to the single API invocation function using the name of the API as an identifier
#define WAR3MAPCPP_CALL_JASS(func, ...)                               \
    static auto addr = war3mapcpp::detail::invoke::FindNative(#func); \
    return war3mapcpp::detail::invoke::api<decltype(func(__VA_ARGS__))>::call(addr, ##__VA_ARGS__)

#define _jstr(str) war3mapcpp::api::Str2JStr(war3mapcpp::api::RCString{}, str)

namespace war3mapcpp::api
{
    union DWFP
    {
        unsigned int dw;
        float fl;
    };
    using CJassStringSID = int;
    using HLIGHTNING = int;
    using HEFFECT = int;
    using HWEATHEREFFECT = int;
    using HBOOLEXPR = int;
    using HLOCATION = int;
    using HCONDITIONFUNC = int;
    using HAIDIFFICULTY = int;
    using HALLIANCETYPE = int;
    using HATTACKTYPE = int;
    using HBLENDMODE = int;
    using HCAMERAFIELD = int;
    using HDAMAGETYPE = int;
    using HDIALOGEVENT = int;
    using HEFFECTTYPE = int;
    using HFGAMESTATE = int;
    using HFOGSTATE = int;
    using HGAMEDIFFICULTY = int;
    using HGAMEEVENT = int;
    using HGAMESPEED = int;
    using HGAMETYPE = int;
    using HIGAMESTATE = int;
    using HITEMTYPE = int;
    using HLIMITOP = int;
    using HMAPCONTROL = int;
    using HMAPDENSITY = int;
    using HMAPFLAG = int;
    using HMAPSETTING = int;
    using HMAPVISIBILITY = int;
    using HPATHINGTYPE = int;
    using HPLACEMENT = int;
    using HPLAYERCOLOR = int;
    using HPLAYEREVENT = int;
    using HPLAYERGAMERESULT = int;
    using HPLAYERSCORE = int;
    using HPLAYERSLOTSTATE = int;
    using HPLAYERSTATE = int;
    using HPLAYERUNITEVENT = int;
    using HRACE = int;
    using HRACEPREFERENCE = int;
    using HRARITYCONTROL = int;
    using HSOUNDTYPE = int;
    using HSTARTLOCPRIO = int;
    using HTEXMAPFLAGS = int;
    using HUNITEVENT = int;
    using HUNITSTATE = int;
    using HUNITTYPE = int;
    using HVERSION = int;
    using HVOLUMEGROUP = int;
    using HWEAPONTYPE = int;
    using HWIDGETEVENT = int;
    using HUNIT = int;
    using HCAMERASETUP = int;
    using HDESTRUCTABLE = int;
    using HDEFEATCONDITION = int;
    using HFOGMODIFIER = int;
    using HFORCE = int;
    using HGROUP = int;
    using HIMAGE = int;
    using HITEM = int;
    using HITEMPOOL = int;
    using HLEADERBOARD = int;
    using HSOUND = int;
    using HMULTIBOARD = int;
    using HQUEST = int;
    using HREGION = int;
    using HTEXTTAG = int;
    using HTIMER = int;
    using HTIMERDIALOG = int;
    using HTRACKABLE = int;
    using HTRIGGER = int;
    using HUBERSPLAT = int;
    using HUNITPOOL = int;
    using HBUTTON = int;
    using HDIALOG = int;
    using HFILTERFUNC = int;
    using HPLAYER = int;
    using HGAMESTATE = int;
    using HWIDGET = int;
    using HABILITY = int;
    using HEVENTID = int;
    using HRECT = int;
    using HGAMECACHE = int;
    using HHASHTABLE = int;
    using HMULTIBOARDITEM = int;
    using HQUESTITEM = int;
    using HTRIGGERACTION = int;
    using HTRIGGERCONDITION = int;
    using HEVENT = int;
    using HTERRAINDEFORMATION = int;
    using CODE = int;
    using HHANDLE = int;
    using HAGENT = int;
    using BOOLEAN = int;

    struct CStringRep // the actual class name is CStringRep ;)
    {
        DWORD vtable;   // 0x00
        DWORD refCount; // 0x04 ?
        DWORD dwUnk1;   // 0x08 ?
        DWORD pUnk2;    // 0x0C ?
        DWORD pUnk3;    // 0x10 ?
        DWORD pUnk4;    // 0x14 this-0xC .o0
        DWORD pUnk5;    // 0x18
        char *data;     // 0x1C ...
    };
    struct RCString
    {
        DWORD vtable = 0;
        DWORD refcnt = 0;
        CStringRep *data = nullptr;
    };
    static_assert(sizeof(RCString) == 0xC, "RCString size mismatch!");
    using CJassString = RCString *;

    CJassString Str2JStr(RCString &rc, const char *str)
    {
        static uintptr_t addr = war3mapcpp::detail::invoke::GetAddress(0x0506D0);
        static uintptr_t vftable = war3mapcpp::detail::invoke::GetAddress(0x952F7C);
        rc.vtable = vftable;
        return war3mapcpp::detail::this_call<CJassString>(addr, &rc, str);
    }
    CJassString Str2JStr(RCString &rc, const std::string &str)
    {
        return Str2JStr(rc, str.c_str());
    }

    // wc3 hash algo
    uint32_t string_hash(const char *str)
    {
        static auto addr = war3mapcpp::detail::invoke::GetAddress(0x120694);
        return war3mapcpp::detail::c_call<uint32_t>(addr, str);
    }

    struct node;

    struct node_2
    {
        node_2 *lft_;
        node *rht_;
        const char *str_;
    };

    struct node_1
    {
        node_1 *next_;
        node *prev_;
        node_2 *lft_;
        node *rht_;
        const char *str_;
    };

    struct node
    {
        uint32_t hash_;
        node_1 *next_;
        node *prev_;
        node_2 *lft_;
        node *rht_;
        uint32_t key;

        bool is_vaild() const
        {
            return ((intptr_t)this > 0x10000);
        }
    };

    template <class Node = node>
    struct table
    {
        template <class Node = node>
        struct entry
        {
            uint32_t step;
            node_1 *tail;
            Node *head;

            node *convert(Node *ptr) const
            {
                return (node *)((uintptr_t)ptr + step - 4);
            }
        };

        uint32_t unk0;
        uint32_t step;
        uint32_t tail;
        Node *head;
        uint32_t unk4;
        uint32_t unk5;
        uint32_t unk6;
        entry<Node> *buckets;
        uint32_t unk8;
        uint32_t mask;

        Node *find(uint32_t hash)
        {
            Node *fnode_ptr = nullptr;

            if (mask == 0xFFFFFFFF)
                return nullptr;

            fnode_ptr = buckets[hash & mask].head;

            if (!fnode_ptr->is_vaild())
                return nullptr;

            for (;;)
            {
                if (fnode_ptr->hash_ == hash)
                    return fnode_ptr;
                fnode_ptr = (Node *)(uintptr_t)(fnode_ptr->prev_);

                if (!fnode_ptr->is_vaild())
                    return nullptr;
            }
        }

        Node *find(const char *str)
        {
            uint32_t hash;
            Node *fnode_ptr = nullptr;

            if (mask == 0xFFFFFFFF)
                return nullptr;

            hash = string_hash(str);

            fnode_ptr = buckets[hash & mask].head;

            if (!fnode_ptr->is_vaild())
                return nullptr;

            for (;;)
            {
                if (fnode_ptr->hash_ == hash)
                {
                    if ((const char *)fnode_ptr->key == str)
                        return fnode_ptr;

                    if (0 == strcmp((const char *)fnode_ptr->key, str))
                        return fnode_ptr;
                }
                fnode_ptr = (Node *)(uintptr_t)(fnode_ptr->prev_);

                if (!fnode_ptr->is_vaild())
                    return nullptr;
            }
        }
    };

    struct reverse_node : public node
    {
        uint32_t value;
    };

    struct reverse_table
    {
        typedef table<reverse_node> table_t;

        uint32_t unk0_;
        uint32_t size;
        reverse_node **node_array_;
        uint32_t unk3_;
        table_t table_;

        reverse_node *at(uint32_t index)
        {
            return node_array_[index];
        }

        reverse_node *find(uint32_t hash)
        {
            return table_.find(hash);
        }

        reverse_node *find(const char *str)
        {
            return table_.find(str);
        }
    };

    struct JassNativeNode
    {
        LPVOID vtable;
        UINT32 hash_, unused2, unused3, unused4;
        _Maybenull_ JassNativeNode *nxtNode;
        char *fnName;
        PROC fnAddr;
        UINT32 argCount;
        char *protoStr;
        UINT32 argNameArrCount, argNameArrNonNullCount;
        _Maybenull_ char **fnArgNames; // c_str array
        UINT32 unused5, retType;
    };

    static size_t callbackTopCount = 0;
    static std::unordered_map<size_t, JCALLBACK> callbackMap;
    static std::unordered_map<size_t, CODE> callbackCodeMap;
    static std::unordered_map<size_t, size_t *> callbackCodes;
    constexpr int HOOK_NATIVE_CALLBACK_MAGIC = 'KKMD';
    PROC origIsUnitType = nullptr;

    size_t __cdecl HookNativeIsUnitType(size_t arg1, size_t arg2)
    {
        if (arg2 == HOOK_NATIVE_CALLBACK_MAGIC)
        {
            callbackMap[arg1]();
            return 0;
        }
        else
        {
            return reinterpret_cast<size_t(__cdecl *)(size_t, size_t)>(origIsUnitType)(arg1, arg2);
        }
    }

    void InstallCodeCallback()
    {
        auto ptr = war3mapcpp::detail::fast_call<JassNativeNode *>(war3mapcpp::detail::invoke::GetAddress(0x7E2FE0), "IsUnitType");
        if (ptr)
        {
            if (!origIsUnitType)
            {
                origIsUnitType = ptr->fnAddr;
            }
            ptr->fnAddr = (PROC)HookNativeIsUnitType;
        }
    }

    void UnInstallCodeCallback()
    {
        if (origIsUnitType)
        {
            auto ptr = war3mapcpp::detail::fast_call<JassNativeNode *>(war3mapcpp::detail::invoke::GetAddress(0x7E2FE0), "IsUnitType");
            if (ptr)
            {
                ptr->fnAddr = origIsUnitType;
            }
            origIsUnitType = nullptr;
        }
        for (auto &item : callbackCodes)
        {
            delete[] item.second;
        }
        callbackMap.clear();
        callbackCodes.clear();
        callbackCodeMap.clear();
    }

    uintptr_t MemRead(uintptr_t address)
    {
        return *(uintptr_t *)address;
    }

    uintptr_t GetInstance(int index)
    {
        return war3mapcpp::detail::fast_call<uintptr_t>(war3mapcpp::detail::invoke::GetAddress(0x04EFB0), index);
    }

    uintptr_t GetCurrentVM(int index = 1)
    {
        auto ins = GetInstance(5);
        ins = MemRead(ins + 0x90);
        return MemRead(ins + 0x4 * index);
    }

    reverse_table *GetSymbolTable()
    {
        return (reverse_table *)MemRead(MemRead(GetCurrentVM() + 0x2858) + 0x8);
    }

    uintptr_t GetCurrentCodeRelativeAddress()
    {
        auto vm = GetCurrentVM();
        auto code = MemRead(vm + 0x2884 + 0x4);
        return MemRead(code + 0x8);
    }

    CODE CreateCode(const JCALLBACK &callback)
    {
        static auto runOnce = false;
        static uintptr_t hookNativeFuncId = 0;
        if (!runOnce)
        {
            runOnce = true;
            hookNativeFuncId = GetSymbolTable()->find("IsUnitType")->value;
            InstallCodeCallback();
        }

        auto it = std::find_if(callbackMap.begin(), callbackMap.end(), [&](const auto &pair)
                               { return pair.second.target<JCALLBACK>() == callback.target<JCALLBACK>(); });
        if (it != callbackMap.end())
        {
            return callbackCodeMap[it->first];
        }

        callbackMap[++callbackTopCount] = callback;

        auto bytecodeBuff = new uintptr_t[]{
            0,
            0x0CD00400, callbackTopCount,           // mov reg, callbackTopCount
            0x13D00000, 0x00000000,                 // push reg
            0x0CD00400, HOOK_NATIVE_CALLBACK_MAGIC, // mov reg, HOOK_NATIVE_CALLBACK_MAGIC
            0x13D00000, 0x00000000,                 // push reg
            0x15000000, hookNativeFuncId,           // call_native hookNativeFuncId
            0x0D00D000, 0x00000000,                 // set ret
            0x27000000, 0x00000000,                 // ret
        };

        bytecodeBuff[0] = uintptr_t(bytecodeBuff) + 4;
        callbackCodes[callbackTopCount] = bytecodeBuff;

        CODE ret = uintptr_t((uintptr_t(bytecodeBuff) - GetCurrentCodeRelativeAddress()) / 4);
        callbackCodeMap[callbackTopCount] = ret;
        return ret;
    }

    WA3MAPCPP_FORCE_INLINE int JASSAPI AbilityId(CJassString abilityIdString) { WAR3MAPCPP_CALL_JASS(AbilityId, abilityIdString); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI AbilityId2String(int AbilID) { WAR3MAPCPP_CALL_JASS(AbilityId2String, AbilID); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI Acos(float x) { WAR3MAPCPP_CALL_JASS(Acos, x); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI AddAssault(int arg1, int arg2) { WAR3MAPCPP_CALL_JASS(AddAssault, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI AddDefenders(int arg1, int arg2) { WAR3MAPCPP_CALL_JASS(AddDefenders, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI AddGuardPost(int arg1, float arg2, float arg3) { WAR3MAPCPP_CALL_JASS(AddGuardPost, arg1, arg2, arg3); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI AddHeroXP(HUNIT hero, int xpToAdd, BOOLEAN showEyeCandy) { WAR3MAPCPP_CALL_JASS(AddHeroXP, hero, xpToAdd, showEyeCandy); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI AddIndicator(HWIDGET widget, int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(AddIndicator, widget, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI AddItemToAllStock(int itemId, int currentStock, int stockMax) { WAR3MAPCPP_CALL_JASS(AddItemToAllStock, itemId, currentStock, stockMax); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI AddItemToStock(HUNIT unit, int itemId, int currentStock, int stockMax) { WAR3MAPCPP_CALL_JASS(AddItemToStock, unit, itemId, currentStock, stockMax); }
    WA3MAPCPP_FORCE_INLINE HLIGHTNING JASSAPI AddLightning(CJassString codeName, BOOLEAN checkVisibility, float x1, float y1, float x2, float y2) { WAR3MAPCPP_CALL_JASS(AddLightning, codeName, checkVisibility, x1, y1, x2, y2); }
    WA3MAPCPP_FORCE_INLINE HLIGHTNING JASSAPI AddLightningEx(CJassString codeName, BOOLEAN checkVisibility, float x1, float y1, float z1, float x2, float y2, float z2) { WAR3MAPCPP_CALL_JASS(AddLightningEx, codeName, checkVisibility, x1, y1, z1, x2, y2, z2); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI AddPlayerTechResearched(HPLAYER player, int techid, int levels) { WAR3MAPCPP_CALL_JASS(AddPlayerTechResearched, player, techid, levels); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI AddResourceAmount(HUNIT unit, int amount) { WAR3MAPCPP_CALL_JASS(AddResourceAmount, unit, amount); }
    WA3MAPCPP_FORCE_INLINE HEFFECT JASSAPI AddSpecialEffect(CJassString modelName, float x, float y) { WAR3MAPCPP_CALL_JASS(AddSpecialEffect, modelName, x, y); }
    WA3MAPCPP_FORCE_INLINE HEFFECT JASSAPI AddSpecialEffectLoc(CJassString modelName, HLOCATION where) { WAR3MAPCPP_CALL_JASS(AddSpecialEffectLoc, modelName, where); }
    WA3MAPCPP_FORCE_INLINE HEFFECT JASSAPI AddSpecialEffectTarget(CJassString modelName, HWIDGET targetWidget, CJassString attachPointName) { WAR3MAPCPP_CALL_JASS(AddSpecialEffectTarget, modelName, targetWidget, attachPointName); }
    WA3MAPCPP_FORCE_INLINE HEFFECT JASSAPI AddSpellEffect(CJassString abilityString, HEFFECTTYPE t, float x, float y) { WAR3MAPCPP_CALL_JASS(AddSpellEffect, abilityString, t, x, y); }
    WA3MAPCPP_FORCE_INLINE HEFFECT JASSAPI AddSpellEffectById(int AbilID, HEFFECTTYPE t, float x, float y) { WAR3MAPCPP_CALL_JASS(AddSpellEffectById, AbilID, t, x, y); }
    WA3MAPCPP_FORCE_INLINE HEFFECT JASSAPI AddSpellEffectByIdLoc(int AbilID, HEFFECTTYPE t, HLOCATION where) { WAR3MAPCPP_CALL_JASS(AddSpellEffectByIdLoc, AbilID, t, where); }
    WA3MAPCPP_FORCE_INLINE HEFFECT JASSAPI AddSpellEffectLoc(CJassString abilityString, HEFFECTTYPE t, HLOCATION where) { WAR3MAPCPP_CALL_JASS(AddSpellEffectLoc, abilityString, t, where); }
    WA3MAPCPP_FORCE_INLINE HEFFECT JASSAPI AddSpellEffectTarget(CJassString modelName, HEFFECTTYPE t, HWIDGET targetWidget, CJassString attachPoint) { WAR3MAPCPP_CALL_JASS(AddSpellEffectTarget, modelName, t, targetWidget, attachPoint); }
    WA3MAPCPP_FORCE_INLINE HEFFECT JASSAPI AddSpellEffectTargetById(int AbilID, HEFFECTTYPE t, HWIDGET targetWidget, CJassString attachPoint) { WAR3MAPCPP_CALL_JASS(AddSpellEffectTargetById, AbilID, t, targetWidget, attachPoint); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI AddUnitAnimationProperties(HUNIT unit, CJassString animProperties, BOOLEAN add) { WAR3MAPCPP_CALL_JASS(AddUnitAnimationProperties, unit, animProperties, add); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI AddUnitToAllStock(int unitId, int currentStock, int stockMax) { WAR3MAPCPP_CALL_JASS(AddUnitToAllStock, unitId, currentStock, stockMax); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI AddUnitToStock(HUNIT unit, int unitId, int currentStock, int stockMax) { WAR3MAPCPP_CALL_JASS(AddUnitToStock, unit, unitId, currentStock, stockMax); }
    WA3MAPCPP_FORCE_INLINE HWEATHEREFFECT JASSAPI AddWeatherEffect(HRECT where, int effectID) { WAR3MAPCPP_CALL_JASS(AddWeatherEffect, where, effectID); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI AdjustCameraField(HCAMERAFIELD field, float offset, float duration) { WAR3MAPCPP_CALL_JASS(AdjustCameraField, field, offset, duration); }
    WA3MAPCPP_FORCE_INLINE HBOOLEXPR JASSAPI And(HBOOLEXPR operandA, HBOOLEXPR operandB) { WAR3MAPCPP_CALL_JASS(And, operandA, operandB); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI Asin(float y) { WAR3MAPCPP_CALL_JASS(Asin, y); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI Atan(float x) { WAR3MAPCPP_CALL_JASS(Atan, x); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI Atan2(float y, float x) { WAR3MAPCPP_CALL_JASS(Atan2, y, x); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI AttachSoundToUnit(HSOUND soundHandle, HUNIT unit) { WAR3MAPCPP_CALL_JASS(AttachSoundToUnit, soundHandle, unit); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI AttackMoveKill(HUNIT arg1) { WAR3MAPCPP_CALL_JASS(AttackMoveKill, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI AttackMoveXY(int arg1, int arg2) { WAR3MAPCPP_CALL_JASS(AttackMoveXY, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CachePlayerHeroData(HPLAYER player) { WAR3MAPCPP_CALL_JASS(CachePlayerHeroData, player); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CameraSetSmoothingFactor(float factor) { WAR3MAPCPP_CALL_JASS(CameraSetSmoothingFactor, factor); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CameraSetSourceNoise(float mag, float velocity) { WAR3MAPCPP_CALL_JASS(CameraSetSourceNoise, mag, velocity); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CameraSetSourceNoiseEx(float mag, float velocity, BOOLEAN vertOnly) { WAR3MAPCPP_CALL_JASS(CameraSetSourceNoiseEx, mag, velocity, vertOnly); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CameraSetTargetNoise(float mag, float velocity) { WAR3MAPCPP_CALL_JASS(CameraSetTargetNoise, mag, velocity); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CameraSetTargetNoiseEx(float mag, float velocity, BOOLEAN vertOnly) { WAR3MAPCPP_CALL_JASS(CameraSetTargetNoiseEx, mag, velocity, vertOnly); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CameraSetupApply(HCAMERASETUP Setup, BOOLEAN doPan, BOOLEAN panTimed) { WAR3MAPCPP_CALL_JASS(CameraSetupApply, Setup, doPan, panTimed); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CameraSetupApplyForceDuration(HCAMERASETUP Setup, BOOLEAN doPan, float forceDuration) { WAR3MAPCPP_CALL_JASS(CameraSetupApplyForceDuration, Setup, doPan, forceDuration); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CameraSetupApplyForceDurationWithZ(HCAMERASETUP Setup, float zDestOffset, float forceDuration) { WAR3MAPCPP_CALL_JASS(CameraSetupApplyForceDurationWithZ, Setup, zDestOffset, forceDuration); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CameraSetupApplyWithZ(HCAMERASETUP Setup, float zDestOffset) { WAR3MAPCPP_CALL_JASS(CameraSetupApplyWithZ, Setup, zDestOffset); }
    WA3MAPCPP_FORCE_INLINE HLOCATION JASSAPI CameraSetupGetDestPositionLoc(HCAMERASETUP Setup) { WAR3MAPCPP_CALL_JASS(CameraSetupGetDestPositionLoc, Setup); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI CameraSetupGetDestPositionX(HCAMERASETUP Setup) { WAR3MAPCPP_CALL_JASS(CameraSetupGetDestPositionX, Setup); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI CameraSetupGetDestPositionY(HCAMERASETUP Setup) { WAR3MAPCPP_CALL_JASS(CameraSetupGetDestPositionY, Setup); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI CameraSetupGetField(HCAMERASETUP Setup, HCAMERAFIELD field) { WAR3MAPCPP_CALL_JASS(CameraSetupGetField, Setup, field); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CameraSetupSetDestPosition(HCAMERASETUP Setup, float x, float y, float duration) { WAR3MAPCPP_CALL_JASS(CameraSetupSetDestPosition, Setup, x, y, duration); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CameraSetupSetField(HCAMERASETUP Setup, HCAMERAFIELD field, float value, float duration) { WAR3MAPCPP_CALL_JASS(CameraSetupSetField, Setup, field, value, duration); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI CaptainAtGoal() { WAR3MAPCPP_CALL_JASS(CaptainAtGoal); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CaptainAttack(float arg1, float arg2) { WAR3MAPCPP_CALL_JASS(CaptainAttack, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CaptainGoHome() { WAR3MAPCPP_CALL_JASS(CaptainGoHome); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI CaptainGroupSize() { WAR3MAPCPP_CALL_JASS(CaptainGroupSize); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI CaptainInCombat(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(CaptainInCombat, arg1); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI CaptainIsEmpty() { WAR3MAPCPP_CALL_JASS(CaptainIsEmpty); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI CaptainIsFull() { WAR3MAPCPP_CALL_JASS(CaptainIsFull); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI CaptainIsHome() { WAR3MAPCPP_CALL_JASS(CaptainIsHome); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI CaptainReadiness() { WAR3MAPCPP_CALL_JASS(CaptainReadiness); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI CaptainReadinessHP() { WAR3MAPCPP_CALL_JASS(CaptainReadinessHP); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI CaptainReadinessMa() { WAR3MAPCPP_CALL_JASS(CaptainReadinessMa); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI CaptainRetreating() { WAR3MAPCPP_CALL_JASS(CaptainRetreating); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CaptainVsPlayer(HPLAYER arg1) { WAR3MAPCPP_CALL_JASS(CaptainVsPlayer, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CaptainVsUnits(HPLAYER arg1) { WAR3MAPCPP_CALL_JASS(CaptainVsUnits, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ChangeLevel(CJassString newLevel, BOOLEAN doScoreScreen) { WAR3MAPCPP_CALL_JASS(ChangeLevel, newLevel, doScoreScreen); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI Cheat(CJassString cheatStr) { WAR3MAPCPP_CALL_JASS(Cheat, cheatStr); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI ChooseRandomCreep(int level) { WAR3MAPCPP_CALL_JASS(ChooseRandomCreep, level); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI ChooseRandomItem(int level) { WAR3MAPCPP_CALL_JASS(ChooseRandomItem, level); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI ChooseRandomItemEx(HITEMTYPE Type, int level) { WAR3MAPCPP_CALL_JASS(ChooseRandomItemEx, Type, level); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI ChooseRandomNPBuilding() { WAR3MAPCPP_CALL_JASS(ChooseRandomNPBuilding); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ClearCaptainTargets() { WAR3MAPCPP_CALL_JASS(ClearCaptainTargets); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ClearHarvestAI() { WAR3MAPCPP_CALL_JASS(ClearHarvestAI); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ClearMapMusic() { WAR3MAPCPP_CALL_JASS(ClearMapMusic); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ClearSelection() { WAR3MAPCPP_CALL_JASS(ClearSelection); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ClearStackedSound(CJassString arg1, float arg2, float arg3) { WAR3MAPCPP_CALL_JASS(ClearStackedSound, arg1, arg2, arg3); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ClearStackedSoundRect(CJassString arg1, HRECT arg2) { WAR3MAPCPP_CALL_JASS(ClearStackedSoundRect, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ClearTextMessages() { WAR3MAPCPP_CALL_JASS(ClearTextMessages); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CommandAI(HPLAYER num, int command, int data) { WAR3MAPCPP_CALL_JASS(CommandAI, num, command, data); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI CommandsWaiting() { WAR3MAPCPP_CALL_JASS(CommandsWaiting); }
    WA3MAPCPP_FORCE_INLINE HCONDITIONFUNC JASSAPI Condition(JCALLBACK func) { WAR3MAPCPP_CALL_JASS(Condition, func); }
    WA3MAPCPP_FORCE_INLINE HAIDIFFICULTY JASSAPI ConvertAIDifficulty(int i) { WAR3MAPCPP_CALL_JASS(ConvertAIDifficulty, i); }
    WA3MAPCPP_FORCE_INLINE HALLIANCETYPE JASSAPI ConvertAllianceType(int i) { WAR3MAPCPP_CALL_JASS(ConvertAllianceType, i); }
    WA3MAPCPP_FORCE_INLINE HATTACKTYPE JASSAPI ConvertAttackType(int i) { WAR3MAPCPP_CALL_JASS(ConvertAttackType, i); }
    WA3MAPCPP_FORCE_INLINE HBLENDMODE JASSAPI ConvertBlendMode(int i) { WAR3MAPCPP_CALL_JASS(ConvertBlendMode, i); }
    WA3MAPCPP_FORCE_INLINE HCAMERAFIELD JASSAPI ConvertCameraField(int i) { WAR3MAPCPP_CALL_JASS(ConvertCameraField, i); }
    WA3MAPCPP_FORCE_INLINE HDAMAGETYPE JASSAPI ConvertDamageType(int i) { WAR3MAPCPP_CALL_JASS(ConvertDamageType, i); }
    WA3MAPCPP_FORCE_INLINE HDIALOGEVENT JASSAPI ConvertDialogEvent(int i) { WAR3MAPCPP_CALL_JASS(ConvertDialogEvent, i); }
    WA3MAPCPP_FORCE_INLINE HEFFECTTYPE JASSAPI ConvertEffectType(int i) { WAR3MAPCPP_CALL_JASS(ConvertEffectType, i); }
    WA3MAPCPP_FORCE_INLINE HFGAMESTATE JASSAPI ConvertFGameState(int i) { WAR3MAPCPP_CALL_JASS(ConvertFGameState, i); }
    WA3MAPCPP_FORCE_INLINE HFOGSTATE JASSAPI ConvertFogState(int i) { WAR3MAPCPP_CALL_JASS(ConvertFogState, i); }
    WA3MAPCPP_FORCE_INLINE HGAMEDIFFICULTY JASSAPI ConvertGameDifficulty(int i) { WAR3MAPCPP_CALL_JASS(ConvertGameDifficulty, i); }
    WA3MAPCPP_FORCE_INLINE HGAMEEVENT JASSAPI ConvertGameEvent(int i) { WAR3MAPCPP_CALL_JASS(ConvertGameEvent, i); }
    WA3MAPCPP_FORCE_INLINE HGAMESPEED JASSAPI ConvertGameSpeed(int i) { WAR3MAPCPP_CALL_JASS(ConvertGameSpeed, i); }
    WA3MAPCPP_FORCE_INLINE HGAMETYPE JASSAPI ConvertGameType(int i) { WAR3MAPCPP_CALL_JASS(ConvertGameType, i); }
    WA3MAPCPP_FORCE_INLINE HIGAMESTATE JASSAPI ConvertIGameState(int i) { WAR3MAPCPP_CALL_JASS(ConvertIGameState, i); }
    WA3MAPCPP_FORCE_INLINE HITEMTYPE JASSAPI ConvertItemType(int i) { WAR3MAPCPP_CALL_JASS(ConvertItemType, i); }
    WA3MAPCPP_FORCE_INLINE HLIMITOP JASSAPI ConvertLimitOp(int i) { WAR3MAPCPP_CALL_JASS(ConvertLimitOp, i); }
    WA3MAPCPP_FORCE_INLINE HMAPCONTROL JASSAPI ConvertMapControl(int i) { WAR3MAPCPP_CALL_JASS(ConvertMapControl, i); }
    WA3MAPCPP_FORCE_INLINE HMAPDENSITY JASSAPI ConvertMapDensity(int i) { WAR3MAPCPP_CALL_JASS(ConvertMapDensity, i); }
    WA3MAPCPP_FORCE_INLINE HMAPFLAG JASSAPI ConvertMapFlag(int i) { WAR3MAPCPP_CALL_JASS(ConvertMapFlag, i); }
    WA3MAPCPP_FORCE_INLINE HMAPSETTING JASSAPI ConvertMapSetting(int i) { WAR3MAPCPP_CALL_JASS(ConvertMapSetting, i); }
    WA3MAPCPP_FORCE_INLINE HMAPVISIBILITY JASSAPI ConvertMapVisibility(int i) { WAR3MAPCPP_CALL_JASS(ConvertMapVisibility, i); }
    WA3MAPCPP_FORCE_INLINE HPATHINGTYPE JASSAPI ConvertPathingType(int i) { WAR3MAPCPP_CALL_JASS(ConvertPathingType, i); }
    WA3MAPCPP_FORCE_INLINE HPLACEMENT JASSAPI ConvertPlacement(int i) { WAR3MAPCPP_CALL_JASS(ConvertPlacement, i); }
    WA3MAPCPP_FORCE_INLINE HPLAYERCOLOR JASSAPI ConvertPlayerColor(int i) { WAR3MAPCPP_CALL_JASS(ConvertPlayerColor, i); }
    WA3MAPCPP_FORCE_INLINE HPLAYEREVENT JASSAPI ConvertPlayerEvent(int i) { WAR3MAPCPP_CALL_JASS(ConvertPlayerEvent, i); }
    WA3MAPCPP_FORCE_INLINE HPLAYERGAMERESULT JASSAPI ConvertPlayerGameResult(int i) { WAR3MAPCPP_CALL_JASS(ConvertPlayerGameResult, i); }
    WA3MAPCPP_FORCE_INLINE HPLAYERSCORE JASSAPI ConvertPlayerScore(int i) { WAR3MAPCPP_CALL_JASS(ConvertPlayerScore, i); }
    WA3MAPCPP_FORCE_INLINE HPLAYERSLOTSTATE JASSAPI ConvertPlayerSlotState(int i) { WAR3MAPCPP_CALL_JASS(ConvertPlayerSlotState, i); }
    WA3MAPCPP_FORCE_INLINE HPLAYERSTATE JASSAPI ConvertPlayerState(int i) { WAR3MAPCPP_CALL_JASS(ConvertPlayerState, i); }
    WA3MAPCPP_FORCE_INLINE HPLAYERUNITEVENT JASSAPI ConvertPlayerUnitEvent(int i) { WAR3MAPCPP_CALL_JASS(ConvertPlayerUnitEvent, i); }
    WA3MAPCPP_FORCE_INLINE HRACE JASSAPI ConvertRace(int i) { WAR3MAPCPP_CALL_JASS(ConvertRace, i); }
    WA3MAPCPP_FORCE_INLINE HRACEPREFERENCE JASSAPI ConvertRacePref(int i) { WAR3MAPCPP_CALL_JASS(ConvertRacePref, i); }
    WA3MAPCPP_FORCE_INLINE HRARITYCONTROL JASSAPI ConvertRarityControl(int i) { WAR3MAPCPP_CALL_JASS(ConvertRarityControl, i); }
    WA3MAPCPP_FORCE_INLINE HSOUNDTYPE JASSAPI ConvertSoundType(int i) { WAR3MAPCPP_CALL_JASS(ConvertSoundType, i); }
    WA3MAPCPP_FORCE_INLINE HSTARTLOCPRIO JASSAPI ConvertStartLocPrio(int i) { WAR3MAPCPP_CALL_JASS(ConvertStartLocPrio, i); }
    WA3MAPCPP_FORCE_INLINE HTEXMAPFLAGS JASSAPI ConvertTexMapFlags(int i) { WAR3MAPCPP_CALL_JASS(ConvertTexMapFlags, i); }
    WA3MAPCPP_FORCE_INLINE HUNITEVENT JASSAPI ConvertUnitEvent(int i) { WAR3MAPCPP_CALL_JASS(ConvertUnitEvent, i); }
    WA3MAPCPP_FORCE_INLINE HUNITSTATE JASSAPI ConvertUnitState(int i) { WAR3MAPCPP_CALL_JASS(ConvertUnitState, i); }
    WA3MAPCPP_FORCE_INLINE HUNITTYPE JASSAPI ConvertUnitType(int i) { WAR3MAPCPP_CALL_JASS(ConvertUnitType, i); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI ConvertUnits(int arg1, int arg2) { WAR3MAPCPP_CALL_JASS(ConvertUnits, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE HVERSION JASSAPI ConvertVersion(int i) { WAR3MAPCPP_CALL_JASS(ConvertVersion, i); }
    WA3MAPCPP_FORCE_INLINE HVOLUMEGROUP JASSAPI ConvertVolumeGroup(int i) { WAR3MAPCPP_CALL_JASS(ConvertVolumeGroup, i); }
    WA3MAPCPP_FORCE_INLINE HWEAPONTYPE JASSAPI ConvertWeaponType(int i) { WAR3MAPCPP_CALL_JASS(ConvertWeaponType, i); }
    WA3MAPCPP_FORCE_INLINE HWIDGETEVENT JASSAPI ConvertWidgetEvent(int i) { WAR3MAPCPP_CALL_JASS(ConvertWidgetEvent, i); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI CopySaveGame(CJassString sourceSaveName, CJassString destSaveName) { WAR3MAPCPP_CALL_JASS(CopySaveGame, sourceSaveName, destSaveName); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI Cos(float radians) { WAR3MAPCPP_CALL_JASS(Cos, radians); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI CreateBlightedGoldmine(HPLAYER id, float x, float y, float face) { WAR3MAPCPP_CALL_JASS(CreateBlightedGoldmine, id, x, y, face); }
    WA3MAPCPP_FORCE_INLINE HCAMERASETUP JASSAPI CreateCameraSetup() { WAR3MAPCPP_CALL_JASS(CreateCameraSetup); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CreateCaptains() { WAR3MAPCPP_CALL_JASS(CreateCaptains); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI CreateCorpse(HPLAYER player, int unitid, float x, float y, float face) { WAR3MAPCPP_CALL_JASS(CreateCorpse, player, unitid, x, y, face); }
    WA3MAPCPP_FORCE_INLINE HDESTRUCTABLE JASSAPI CreateDeadDestructable(int objectid, float x, float y, float face, float scale, int variation) { WAR3MAPCPP_CALL_JASS(CreateDeadDestructable, objectid, x, y, face, scale, variation); }
    WA3MAPCPP_FORCE_INLINE HDESTRUCTABLE JASSAPI CreateDeadDestructableZ(int objectid, float x, float y, float z, float face, float scale, int variation) { WAR3MAPCPP_CALL_JASS(CreateDeadDestructableZ, objectid, x, y, z, face, scale, variation); }
    WA3MAPCPP_FORCE_INLINE HDEFEATCONDITION JASSAPI CreateDefeatCondition() { WAR3MAPCPP_CALL_JASS(CreateDefeatCondition); }
    WA3MAPCPP_FORCE_INLINE HDESTRUCTABLE JASSAPI CreateDestructable(int objectid, float x, float y, float face, float scale, int variation) { WAR3MAPCPP_CALL_JASS(CreateDestructable, objectid, x, y, face, scale, variation); }
    WA3MAPCPP_FORCE_INLINE HDESTRUCTABLE JASSAPI CreateDestructableZ(int objectid, float x, float y, float z, float face, float scale, int variation) { WAR3MAPCPP_CALL_JASS(CreateDestructableZ, objectid, x, y, z, face, scale, variation); }
    WA3MAPCPP_FORCE_INLINE HFOGMODIFIER JASSAPI CreateFogModifierRadius(HPLAYER forWhichPlayer, HFOGSTATE State, float centerx, float centerY, float radius, BOOLEAN useSharedVision, BOOLEAN afterUnits) { WAR3MAPCPP_CALL_JASS(CreateFogModifierRadius, forWhichPlayer, State, centerx, centerY, radius, useSharedVision, afterUnits); }
    WA3MAPCPP_FORCE_INLINE HFOGMODIFIER JASSAPI CreateFogModifierRadiusLoc(HPLAYER forWhichPlayer, HFOGSTATE State, HLOCATION center, float radius, BOOLEAN useSharedVision, BOOLEAN afterUnits) { WAR3MAPCPP_CALL_JASS(CreateFogModifierRadiusLoc, forWhichPlayer, State, center, radius, useSharedVision, afterUnits); }
    WA3MAPCPP_FORCE_INLINE HFOGMODIFIER JASSAPI CreateFogModifierRect(HPLAYER forWhichPlayer, HFOGSTATE State, HRECT where, BOOLEAN useSharedVision, BOOLEAN afterUnits) { WAR3MAPCPP_CALL_JASS(CreateFogModifierRect, forWhichPlayer, State, where, useSharedVision, afterUnits); }
    WA3MAPCPP_FORCE_INLINE HFORCE JASSAPI CreateForce() { WAR3MAPCPP_CALL_JASS(CreateForce); }
    WA3MAPCPP_FORCE_INLINE HGROUP JASSAPI CreateGroup() { WAR3MAPCPP_CALL_JASS(CreateGroup); }
    WA3MAPCPP_FORCE_INLINE HIMAGE JASSAPI CreateImage(CJassString file, float sizeX, float sizeY, float sizeZ, float posX, float posY, float posZ, float originX, float originY, float originZ, int imageType) { WAR3MAPCPP_CALL_JASS(CreateImage, file, sizeX, sizeY, sizeZ, posX, posY, posZ, originX, originY, originZ, imageType); }
    WA3MAPCPP_FORCE_INLINE HITEM JASSAPI CreateItem(int itemid, float x, float y) { WAR3MAPCPP_CALL_JASS(CreateItem, itemid, x, y); }
    WA3MAPCPP_FORCE_INLINE HITEMPOOL JASSAPI CreateItemPool() { WAR3MAPCPP_CALL_JASS(CreateItemPool); }
    WA3MAPCPP_FORCE_INLINE HLEADERBOARD JASSAPI CreateLeaderboard() { WAR3MAPCPP_CALL_JASS(CreateLeaderboard); }
    WA3MAPCPP_FORCE_INLINE HSOUND JASSAPI CreateMIDISound(CJassString soundLabel, int fadeInRate, int fadeOutRate) { WAR3MAPCPP_CALL_JASS(CreateMIDISound, soundLabel, fadeInRate, fadeOutRate); }
    WA3MAPCPP_FORCE_INLINE HMULTIBOARD JASSAPI CreateMultiboard() { WAR3MAPCPP_CALL_JASS(CreateMultiboard); }
    WA3MAPCPP_FORCE_INLINE HQUEST JASSAPI CreateQuest() { WAR3MAPCPP_CALL_JASS(CreateQuest); }
    WA3MAPCPP_FORCE_INLINE HREGION JASSAPI CreateRegion() { WAR3MAPCPP_CALL_JASS(CreateRegion); }
    WA3MAPCPP_FORCE_INLINE HSOUND JASSAPI CreateSound(CJassString fileName, BOOLEAN looping, BOOLEAN is3D, BOOLEAN stopwhenoutofrange, int fadeInRate, int fadeOutRate, CJassString eaxSetting) { WAR3MAPCPP_CALL_JASS(CreateSound, fileName, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate, eaxSetting); }
    WA3MAPCPP_FORCE_INLINE HSOUND JASSAPI CreateSoundFilenameWithLabel(CJassString fileName, BOOLEAN looping, BOOLEAN is3D, BOOLEAN stopwhenoutofrange, int fadeInRate, int fadeOutRate, CJassString SLKEntryName) { WAR3MAPCPP_CALL_JASS(CreateSoundFilenameWithLabel, fileName, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate, SLKEntryName); }
    WA3MAPCPP_FORCE_INLINE HSOUND JASSAPI CreateSoundFromLabel(CJassString soundLabel, BOOLEAN looping, BOOLEAN is3D, BOOLEAN stopwhenoutofrange, int fadeInRate, int fadeOutRate) { WAR3MAPCPP_CALL_JASS(CreateSoundFromLabel, soundLabel, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate); }
    WA3MAPCPP_FORCE_INLINE HTEXTTAG JASSAPI CreateTextTag() { WAR3MAPCPP_CALL_JASS(CreateTextTag); }
    WA3MAPCPP_FORCE_INLINE HTIMER JASSAPI CreateTimer() { WAR3MAPCPP_CALL_JASS(CreateTimer); }
    WA3MAPCPP_FORCE_INLINE HTIMERDIALOG JASSAPI CreateTimerDialog(HTIMER t) { WAR3MAPCPP_CALL_JASS(CreateTimerDialog, t); }
    WA3MAPCPP_FORCE_INLINE HTRACKABLE JASSAPI CreateTrackable(CJassString trackableModelPath, float x, float y, float facing) { WAR3MAPCPP_CALL_JASS(CreateTrackable, trackableModelPath, x, y, facing); }
    WA3MAPCPP_FORCE_INLINE HTRIGGER JASSAPI CreateTrigger() { WAR3MAPCPP_CALL_JASS(CreateTrigger); }
    WA3MAPCPP_FORCE_INLINE HUBERSPLAT JASSAPI CreateUbersplat(float x, float y, CJassString name, int red, int green, int blue, int alpha, BOOLEAN forcePaused, BOOLEAN noBirthTime) { WAR3MAPCPP_CALL_JASS(CreateUbersplat, x, y, name, red, green, blue, alpha, forcePaused, noBirthTime); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI CreateUnit(HPLAYER id, int unitid, float x, float y, float face) { WAR3MAPCPP_CALL_JASS(CreateUnit, id, unitid, x, y, face); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI CreateUnitAtLoc(HPLAYER id, int unitid, HLOCATION Location, float face) { WAR3MAPCPP_CALL_JASS(CreateUnitAtLoc, id, unitid, Location, face); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI CreateUnitAtLocByName(HPLAYER id, CJassString unitname, HLOCATION Location, float face) { WAR3MAPCPP_CALL_JASS(CreateUnitAtLocByName, id, unitname, Location, face); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI CreateUnitByName(HPLAYER player, CJassString unitname, float x, float y, float face) { WAR3MAPCPP_CALL_JASS(CreateUnitByName, player, unitname, x, y, face); }
    WA3MAPCPP_FORCE_INLINE HUNITPOOL JASSAPI CreateUnitPool() { WAR3MAPCPP_CALL_JASS(CreateUnitPool); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI CreepsOnMap() { WAR3MAPCPP_CALL_JASS(CreepsOnMap); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI CripplePlayer(HPLAYER player, HFORCE toWhichPlayers, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(CripplePlayer, player, toWhichPlayers, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DebugBreak(int arg1) { WAR3MAPCPP_CALL_JASS(DebugBreak, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DebugFI(CJassString arg1, int arg2) { WAR3MAPCPP_CALL_JASS(DebugFI, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DebugS(CJassString arg1) { WAR3MAPCPP_CALL_JASS(DebugS, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DebugUnitID(CJassString arg1, int arg2) { WAR3MAPCPP_CALL_JASS(DebugUnitID, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI DecUnitAbilityLevel(HUNIT unit, int abilcode) { WAR3MAPCPP_CALL_JASS(DecUnitAbilityLevel, unit, abilcode); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DefeatConditionSetDescription(HDEFEATCONDITION Condition, CJassString description) { WAR3MAPCPP_CALL_JASS(DefeatConditionSetDescription, Condition, description); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DefineStartLocation(int StartLoc, float x, float y) { WAR3MAPCPP_CALL_JASS(DefineStartLocation, StartLoc, x, y); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DefineStartLocationLoc(int StartLoc, HLOCATION Location) { WAR3MAPCPP_CALL_JASS(DefineStartLocationLoc, StartLoc, Location); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI Deg2Rad(float degrees) { WAR3MAPCPP_CALL_JASS(Deg2Rad, degrees); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyBoolExpr(HBOOLEXPR e) { WAR3MAPCPP_CALL_JASS(DestroyBoolExpr, e); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyCondition(HCONDITIONFUNC c) { WAR3MAPCPP_CALL_JASS(DestroyCondition, c); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyDefeatCondition(HDEFEATCONDITION Condition) { WAR3MAPCPP_CALL_JASS(DestroyDefeatCondition, Condition); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyEffect(HEFFECT Effect) { WAR3MAPCPP_CALL_JASS(DestroyEffect, Effect); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyFilter(HFILTERFUNC f) { WAR3MAPCPP_CALL_JASS(DestroyFilter, f); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyFogModifier(HFOGMODIFIER FogModifier) { WAR3MAPCPP_CALL_JASS(DestroyFogModifier, FogModifier); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyForce(HFORCE Force) { WAR3MAPCPP_CALL_JASS(DestroyForce, Force); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyGroup(HGROUP Group) { WAR3MAPCPP_CALL_JASS(DestroyGroup, Group); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyImage(HIMAGE Image) { WAR3MAPCPP_CALL_JASS(DestroyImage, Image); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyItemPool(HITEMPOOL ItemPool) { WAR3MAPCPP_CALL_JASS(DestroyItemPool, ItemPool); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyLeaderboard(HLEADERBOARD lb) { WAR3MAPCPP_CALL_JASS(DestroyLeaderboard, lb); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI DestroyLightning(HLIGHTNING Bolt) { WAR3MAPCPP_CALL_JASS(DestroyLightning, Bolt); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyMultiboard(HMULTIBOARD lb) { WAR3MAPCPP_CALL_JASS(DestroyMultiboard, lb); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyQuest(HQUEST Quest) { WAR3MAPCPP_CALL_JASS(DestroyQuest, Quest); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyTextTag(HTEXTTAG t) { WAR3MAPCPP_CALL_JASS(DestroyTextTag, t); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyTimer(HTIMER Timer) { WAR3MAPCPP_CALL_JASS(DestroyTimer, Timer); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyTimerDialog(HTIMERDIALOG Dialog) { WAR3MAPCPP_CALL_JASS(DestroyTimerDialog, Dialog); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyTrigger(HTRIGGER Trigger) { WAR3MAPCPP_CALL_JASS(DestroyTrigger, Trigger); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyUbersplat(HUBERSPLAT Splat) { WAR3MAPCPP_CALL_JASS(DestroyUbersplat, Splat); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestroyUnitPool(HUNITPOOL Pool) { WAR3MAPCPP_CALL_JASS(DestroyUnitPool, Pool); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DestructableRestoreLife(HDESTRUCTABLE d, float life, BOOLEAN birth) { WAR3MAPCPP_CALL_JASS(DestructableRestoreLife, d, life, birth); }
    WA3MAPCPP_FORCE_INLINE HBUTTON JASSAPI DialogAddButton(HDIALOG Dialog, CJassString buttonText, int hotkey) { WAR3MAPCPP_CALL_JASS(DialogAddButton, Dialog, buttonText, hotkey); }
    WA3MAPCPP_FORCE_INLINE HBUTTON JASSAPI DialogAddQuitButton(HDIALOG Dialog, BOOLEAN doScoreScreen, CJassString buttonText, int hotkey) { WAR3MAPCPP_CALL_JASS(DialogAddQuitButton, Dialog, doScoreScreen, buttonText, hotkey); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DialogClear(HDIALOG Dialog) { WAR3MAPCPP_CALL_JASS(DialogClear, Dialog); }
    WA3MAPCPP_FORCE_INLINE HDIALOG JASSAPI DialogCreate() { WAR3MAPCPP_CALL_JASS(DialogCreate); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DialogDestroy(HDIALOG Dialog) { WAR3MAPCPP_CALL_JASS(DialogDestroy, Dialog); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DialogDisplay(HPLAYER player, HDIALOG Dialog, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(DialogDisplay, player, Dialog, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DialogSetAsync(HDIALOG arg1) { WAR3MAPCPP_CALL_JASS(DialogSetAsync, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DialogSetMessage(HDIALOG Dialog, CJassString messageText) { WAR3MAPCPP_CALL_JASS(DialogSetMessage, Dialog, messageText); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DisablePathing() { WAR3MAPCPP_CALL_JASS(DisablePathing); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DisableRestartMission(BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(DisableRestartMission, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DisableTrigger(HTRIGGER Trigger) { WAR3MAPCPP_CALL_JASS(DisableTrigger, Trigger); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DisplayCineFilter(BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(DisplayCineFilter, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DisplayLoadDialog() { WAR3MAPCPP_CALL_JASS(DisplayLoadDialog); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DisplayText(int arg1, CJassString arg2) { WAR3MAPCPP_CALL_JASS(DisplayText, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DisplayTextI(int arg1, CJassString arg2, int arg3) { WAR3MAPCPP_CALL_JASS(DisplayTextI, arg1, arg2, arg3); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DisplayTextII(int arg1, CJassString arg2, int arg3, int arg4) { WAR3MAPCPP_CALL_JASS(DisplayTextII, arg1, arg2, arg3, arg4); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DisplayTextIII(int arg1, CJassString arg2, int arg3, int arg4, int arg5) { WAR3MAPCPP_CALL_JASS(DisplayTextIII, arg1, arg2, arg3, arg4, arg5); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DisplayTextToPlayer(HPLAYER toPlayer, float x, float y, CJassString message) { WAR3MAPCPP_CALL_JASS(DisplayTextToPlayer, toPlayer, x, y, message); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DisplayTimedTextFromPlayer(HPLAYER toPlayer, float x, float y, float duration, CJassString message) { WAR3MAPCPP_CALL_JASS(DisplayTimedTextFromPlayer, toPlayer, x, y, duration, message); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DisplayTimedTextToPlayer(HPLAYER toPlayer, float x, float y, float duration, CJassString message) { WAR3MAPCPP_CALL_JASS(DisplayTimedTextToPlayer, toPlayer, x, y, duration, message); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI DoAiScriptDebug() { WAR3MAPCPP_CALL_JASS(DoAiScriptDebug); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI DoNotSaveReplay() { WAR3MAPCPP_CALL_JASS(DoNotSaveReplay); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI EnableDragSelect(BOOLEAN state, BOOLEAN ui) { WAR3MAPCPP_CALL_JASS(EnableDragSelect, state, ui); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI EnableMinimapFilterButtons(BOOLEAN enableAlly, BOOLEAN enableCreep) { WAR3MAPCPP_CALL_JASS(EnableMinimapFilterButtons, enableAlly, enableCreep); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI EnableOcclusion(BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(EnableOcclusion, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI EnablePreSelect(BOOLEAN state, BOOLEAN ui) { WAR3MAPCPP_CALL_JASS(EnablePreSelect, state, ui); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI EnableSelect(BOOLEAN state, BOOLEAN ui) { WAR3MAPCPP_CALL_JASS(EnableSelect, state, ui); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI EnableTrigger(HTRIGGER Trigger) { WAR3MAPCPP_CALL_JASS(EnableTrigger, Trigger); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI EnableUserControl(BOOLEAN b) { WAR3MAPCPP_CALL_JASS(EnableUserControl, b); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI EnableUserUI(BOOLEAN b) { WAR3MAPCPP_CALL_JASS(EnableUserUI, b); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI EnableWeatherEffect(HWEATHEREFFECT Effect, BOOLEAN enable) { WAR3MAPCPP_CALL_JASS(EnableWeatherEffect, Effect, enable); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI EnableWorldFogBoundary(BOOLEAN b) { WAR3MAPCPP_CALL_JASS(EnableWorldFogBoundary, b); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI EndCinematicScene() { WAR3MAPCPP_CALL_JASS(EndCinematicScene); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI EndGame(BOOLEAN doScoreScreen) { WAR3MAPCPP_CALL_JASS(EndGame, doScoreScreen); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI EndThematicMusic() { WAR3MAPCPP_CALL_JASS(EndThematicMusic); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI EnumDestructablesInRect(HRECT r, HBOOLEXPR filter, JCALLBACK actionFunc) { WAR3MAPCPP_CALL_JASS(EnumDestructablesInRect, r, filter, actionFunc); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI EnumItemsInRect(HRECT r, HBOOLEXPR filter, JCALLBACK actionFunc) { WAR3MAPCPP_CALL_JASS(EnumItemsInRect, r, filter, actionFunc); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ExecuteFunc(CJassString funcName) { WAR3MAPCPP_CALL_JASS(ExecuteFunc, funcName); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI FillGuardPosts() { WAR3MAPCPP_CALL_JASS(FillGuardPosts); }
    WA3MAPCPP_FORCE_INLINE HFILTERFUNC JASSAPI Filter(JCALLBACK func) { WAR3MAPCPP_CALL_JASS(Filter, func); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI FinishUbersplat(HUBERSPLAT Splat) { WAR3MAPCPP_CALL_JASS(FinishUbersplat, Splat); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI FirstOfGroup(HGROUP Group) { WAR3MAPCPP_CALL_JASS(FirstOfGroup, Group); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI FlashQuestDialogButton() { WAR3MAPCPP_CALL_JASS(FlashQuestDialogButton); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI FlushChildHashtable(HHASHTABLE table, int parentKey) { WAR3MAPCPP_CALL_JASS(FlushChildHashtable, table, parentKey); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI FlushGameCache(HGAMECACHE cache) { WAR3MAPCPP_CALL_JASS(FlushGameCache, cache); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI FlushParentHashtable(HHASHTABLE table) { WAR3MAPCPP_CALL_JASS(FlushParentHashtable, table); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI FlushStoredBoolean(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(FlushStoredBoolean, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI FlushStoredInteger(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(FlushStoredInteger, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI FlushStoredMission(HGAMECACHE cache, CJassString missionKey) { WAR3MAPCPP_CALL_JASS(FlushStoredMission, cache, missionKey); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI FlushStoredReal(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(FlushStoredReal, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI FlushStoredString(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(FlushStoredString, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI FlushStoredUnit(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(FlushStoredUnit, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI FogEnable(BOOLEAN enable) { WAR3MAPCPP_CALL_JASS(FogEnable, enable); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI FogMaskEnable(BOOLEAN enable) { WAR3MAPCPP_CALL_JASS(FogMaskEnable, enable); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI FogModifierStart(HFOGMODIFIER FogModifier) { WAR3MAPCPP_CALL_JASS(FogModifierStart, FogModifier); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI FogModifierStop(HFOGMODIFIER FogModifier) { WAR3MAPCPP_CALL_JASS(FogModifierStop, FogModifier); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ForForce(HFORCE Force, JCALLBACK callback) { WAR3MAPCPP_CALL_JASS(ForForce, Force, callback); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ForGroup(HGROUP Group, JCALLBACK callback) { WAR3MAPCPP_CALL_JASS(ForGroup, Group, callback); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ForceAddPlayer(HFORCE Force, HPLAYER player) { WAR3MAPCPP_CALL_JASS(ForceAddPlayer, Force, player); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ForceCampaignSelectScreen() { WAR3MAPCPP_CALL_JASS(ForceCampaignSelectScreen); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ForceCinematicSubtitles(BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(ForceCinematicSubtitles, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ForceClear(HFORCE Force) { WAR3MAPCPP_CALL_JASS(ForceClear, Force); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ForceEnumAllies(HFORCE Force, HPLAYER player, HBOOLEXPR filter) { WAR3MAPCPP_CALL_JASS(ForceEnumAllies, Force, player, filter); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ForceEnumEnemies(HFORCE Force, HPLAYER player, HBOOLEXPR filter) { WAR3MAPCPP_CALL_JASS(ForceEnumEnemies, Force, player, filter); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ForceEnumPlayers(HFORCE Force, HBOOLEXPR filter) { WAR3MAPCPP_CALL_JASS(ForceEnumPlayers, Force, filter); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ForceEnumPlayersCounted(HFORCE Force, HBOOLEXPR filter, int countLimit) { WAR3MAPCPP_CALL_JASS(ForceEnumPlayersCounted, Force, filter, countLimit); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ForcePlayerStartLocation(HPLAYER player, int startLocIndex) { WAR3MAPCPP_CALL_JASS(ForcePlayerStartLocation, player, startLocIndex); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ForceQuestDialogUpdate() { WAR3MAPCPP_CALL_JASS(ForceQuestDialogUpdate); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ForceRemovePlayer(HFORCE Force, HPLAYER player) { WAR3MAPCPP_CALL_JASS(ForceRemovePlayer, Force, player); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ForceUICancel() { WAR3MAPCPP_CALL_JASS(ForceUICancel); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ForceUIKey(CJassString key) { WAR3MAPCPP_CALL_JASS(ForceUIKey, key); }
    WA3MAPCPP_FORCE_INLINE HAIDIFFICULTY JASSAPI GetAIDifficulty(HPLAYER num) { WAR3MAPCPP_CALL_JASS(GetAIDifficulty, num); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI GetAbilityEffect(CJassString abilityString, HEFFECTTYPE t, int index) { WAR3MAPCPP_CALL_JASS(GetAbilityEffect, abilityString, t, index); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI GetAbilityEffectById(int AbilID, HEFFECTTYPE t, int index) { WAR3MAPCPP_CALL_JASS(GetAbilityEffectById, AbilID, t, index); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI GetAbilitySound(CJassString abilityString, HSOUNDTYPE t) { WAR3MAPCPP_CALL_JASS(GetAbilitySound, abilityString, t); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI GetAbilitySoundById(int AbilID, HSOUNDTYPE t) { WAR3MAPCPP_CALL_JASS(GetAbilitySoundById, AbilID, t); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetAiPlayer() { WAR3MAPCPP_CALL_JASS(GetAiPlayer); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetAllianceTarget() { WAR3MAPCPP_CALL_JASS(GetAllianceTarget); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetAllyColorFilterState() { WAR3MAPCPP_CALL_JASS(GetAllyColorFilterState); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetAttacker() { WAR3MAPCPP_CALL_JASS(GetAttacker); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetBuilding(HPLAYER arg1) { WAR3MAPCPP_CALL_JASS(GetBuilding, arg1); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetBuyingUnit() { WAR3MAPCPP_CALL_JASS(GetBuyingUnit); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetCameraBoundMaxX() { WAR3MAPCPP_CALL_JASS(GetCameraBoundMaxX); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetCameraBoundMaxY() { WAR3MAPCPP_CALL_JASS(GetCameraBoundMaxY); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetCameraBoundMinX() { WAR3MAPCPP_CALL_JASS(GetCameraBoundMinX); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetCameraBoundMinY() { WAR3MAPCPP_CALL_JASS(GetCameraBoundMinY); }
    WA3MAPCPP_FORCE_INLINE HLOCATION JASSAPI GetCameraEyePositionLoc() { WAR3MAPCPP_CALL_JASS(GetCameraEyePositionLoc); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetCameraEyePositionX() { WAR3MAPCPP_CALL_JASS(GetCameraEyePositionX); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetCameraEyePositionY() { WAR3MAPCPP_CALL_JASS(GetCameraEyePositionY); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetCameraEyePositionZ() { WAR3MAPCPP_CALL_JASS(GetCameraEyePositionZ); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetCameraField(HCAMERAFIELD field) { WAR3MAPCPP_CALL_JASS(GetCameraField, field); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetCameraMargin(int Margin) { WAR3MAPCPP_CALL_JASS(GetCameraMargin, Margin); }
    WA3MAPCPP_FORCE_INLINE HLOCATION JASSAPI GetCameraTargetPositionLoc() { WAR3MAPCPP_CALL_JASS(GetCameraTargetPositionLoc); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetCameraTargetPositionX() { WAR3MAPCPP_CALL_JASS(GetCameraTargetPositionX); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetCameraTargetPositionY() { WAR3MAPCPP_CALL_JASS(GetCameraTargetPositionY); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetCameraTargetPositionZ() { WAR3MAPCPP_CALL_JASS(GetCameraTargetPositionZ); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetCancelledStructure() { WAR3MAPCPP_CALL_JASS(GetCancelledStructure); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetChangingUnit() { WAR3MAPCPP_CALL_JASS(GetChangingUnit); }
    WA3MAPCPP_FORCE_INLINE HPLAYER JASSAPI GetChangingUnitPrevOwner() { WAR3MAPCPP_CALL_JASS(GetChangingUnitPrevOwner); }
    WA3MAPCPP_FORCE_INLINE HBUTTON JASSAPI GetClickedButton() { WAR3MAPCPP_CALL_JASS(GetClickedButton); }
    WA3MAPCPP_FORCE_INLINE HDIALOG JASSAPI GetClickedDialog() { WAR3MAPCPP_CALL_JASS(GetClickedDialog); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetConstructedStructure() { WAR3MAPCPP_CALL_JASS(GetConstructedStructure); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetConstructingStructure() { WAR3MAPCPP_CALL_JASS(GetConstructingStructure); }
    WA3MAPCPP_FORCE_INLINE HMAPDENSITY JASSAPI GetCreatureDensity() { WAR3MAPCPP_CALL_JASS(GetCreatureDensity); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetCreepCamp(int arg1, int arg2, BOOLEAN arg3) { WAR3MAPCPP_CALL_JASS(GetCreepCamp, arg1, arg2, arg3); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI GetCreepCampFilterState() { WAR3MAPCPP_CALL_JASS(GetCreepCampFilterState); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI GetCustomCampaignButtonVisible(int Button) { WAR3MAPCPP_CALL_JASS(GetCustomCampaignButtonVisible, Button); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetDecayingUnit() { WAR3MAPCPP_CALL_JASS(GetDecayingUnit); }
    WA3MAPCPP_FORCE_INLINE HGAMEDIFFICULTY JASSAPI GetDefaultDifficulty() { WAR3MAPCPP_CALL_JASS(GetDefaultDifficulty); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetDestructableLife(HDESTRUCTABLE d) { WAR3MAPCPP_CALL_JASS(GetDestructableLife, d); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetDestructableMaxLife(HDESTRUCTABLE d) { WAR3MAPCPP_CALL_JASS(GetDestructableMaxLife, d); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI GetDestructableName(HDESTRUCTABLE d) { WAR3MAPCPP_CALL_JASS(GetDestructableName, d); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetDestructableOccluderHeight(HDESTRUCTABLE d) { WAR3MAPCPP_CALL_JASS(GetDestructableOccluderHeight, d); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetDestructableTypeId(HDESTRUCTABLE d) { WAR3MAPCPP_CALL_JASS(GetDestructableTypeId, d); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetDestructableX(HDESTRUCTABLE d) { WAR3MAPCPP_CALL_JASS(GetDestructableX, d); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetDestructableY(HDESTRUCTABLE d) { WAR3MAPCPP_CALL_JASS(GetDestructableY, d); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetDetectedUnit() { WAR3MAPCPP_CALL_JASS(GetDetectedUnit); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetDyingUnit() { WAR3MAPCPP_CALL_JASS(GetDyingUnit); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetEnemyBase() { WAR3MAPCPP_CALL_JASS(GetEnemyBase); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetEnemyExpansion() { WAR3MAPCPP_CALL_JASS(GetEnemyExpansion); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetEnemyPower() { WAR3MAPCPP_CALL_JASS(GetEnemyPower); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetEnteringUnit() { WAR3MAPCPP_CALL_JASS(GetEnteringUnit); }
    WA3MAPCPP_FORCE_INLINE HDESTRUCTABLE JASSAPI GetEnumDestructable() { WAR3MAPCPP_CALL_JASS(GetEnumDestructable); }
    WA3MAPCPP_FORCE_INLINE HITEM JASSAPI GetEnumItem() { WAR3MAPCPP_CALL_JASS(GetEnumItem); }
    WA3MAPCPP_FORCE_INLINE HPLAYER JASSAPI GetEnumPlayer() { WAR3MAPCPP_CALL_JASS(GetEnumPlayer); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetEnumUnit() { WAR3MAPCPP_CALL_JASS(GetEnumUnit); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetEventDamage() { WAR3MAPCPP_CALL_JASS(GetEventDamage); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetEventDamageSource() { WAR3MAPCPP_CALL_JASS(GetEventDamageSource); }
    WA3MAPCPP_FORCE_INLINE HPLAYER JASSAPI GetEventDetectingPlayer() { WAR3MAPCPP_CALL_JASS(GetEventDetectingPlayer); }
    WA3MAPCPP_FORCE_INLINE HGAMESTATE JASSAPI GetEventGameState() { WAR3MAPCPP_CALL_JASS(GetEventGameState); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI GetEventPlayerChatString() { WAR3MAPCPP_CALL_JASS(GetEventPlayerChatString); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI GetEventPlayerChatStringMatched() { WAR3MAPCPP_CALL_JASS(GetEventPlayerChatStringMatched); }
    WA3MAPCPP_FORCE_INLINE HPLAYERSTATE JASSAPI GetEventPlayerState() { WAR3MAPCPP_CALL_JASS(GetEventPlayerState); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetEventTargetUnit() { WAR3MAPCPP_CALL_JASS(GetEventTargetUnit); }
    WA3MAPCPP_FORCE_INLINE HUNITSTATE JASSAPI GetEventUnitState() { WAR3MAPCPP_CALL_JASS(GetEventUnitState); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetExpansionFoe() { WAR3MAPCPP_CALL_JASS(GetExpansionFoe); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetExpansionPeon() { WAR3MAPCPP_CALL_JASS(GetExpansionPeon); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetExpansionX() { WAR3MAPCPP_CALL_JASS(GetExpansionX); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetExpansionY() { WAR3MAPCPP_CALL_JASS(GetExpansionY); }
    WA3MAPCPP_FORCE_INLINE HTIMER JASSAPI GetExpiredTimer() { WAR3MAPCPP_CALL_JASS(GetExpiredTimer); }
    WA3MAPCPP_FORCE_INLINE HDESTRUCTABLE JASSAPI GetFilterDestructable() { WAR3MAPCPP_CALL_JASS(GetFilterDestructable); }
    WA3MAPCPP_FORCE_INLINE HITEM JASSAPI GetFilterItem() { WAR3MAPCPP_CALL_JASS(GetFilterItem); }
    WA3MAPCPP_FORCE_INLINE HPLAYER JASSAPI GetFilterPlayer() { WAR3MAPCPP_CALL_JASS(GetFilterPlayer); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetFilterUnit() { WAR3MAPCPP_CALL_JASS(GetFilterUnit); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetFloatGameState(HFGAMESTATE FloatGameState) { WAR3MAPCPP_CALL_JASS(GetFloatGameState, FloatGameState); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetFoodMade(int unitId) { WAR3MAPCPP_CALL_JASS(GetFoodMade, unitId); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetFoodUsed(int unitId) { WAR3MAPCPP_CALL_JASS(GetFoodUsed, unitId); }
    WA3MAPCPP_FORCE_INLINE HGAMEDIFFICULTY JASSAPI GetGameDifficulty() { WAR3MAPCPP_CALL_JASS(GetGameDifficulty); }
    WA3MAPCPP_FORCE_INLINE HPLACEMENT JASSAPI GetGamePlacement() { WAR3MAPCPP_CALL_JASS(GetGamePlacement); }
    WA3MAPCPP_FORCE_INLINE HGAMESPEED JASSAPI GetGameSpeed() { WAR3MAPCPP_CALL_JASS(GetGameSpeed); }
    WA3MAPCPP_FORCE_INLINE HGAMETYPE JASSAPI GetGameTypeSelected() { WAR3MAPCPP_CALL_JASS(GetGameTypeSelected); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetGoldOwned() { WAR3MAPCPP_CALL_JASS(GetGoldOwned); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetHandleId(HHANDLE h) { WAR3MAPCPP_CALL_JASS(GetHandleId, h); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetHeroAgi(HUNIT hero, BOOLEAN includeBonuses) { WAR3MAPCPP_CALL_JASS(GetHeroAgi, hero, includeBonuses); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetHeroId() { WAR3MAPCPP_CALL_JASS(GetHeroId); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetHeroInt(HUNIT hero, BOOLEAN includeBonuses) { WAR3MAPCPP_CALL_JASS(GetHeroInt, hero, includeBonuses); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetHeroLevel(HUNIT hero) { WAR3MAPCPP_CALL_JASS(GetHeroLevel, hero); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetHeroLevelAI() { WAR3MAPCPP_CALL_JASS(GetHeroLevelAI); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI GetHeroProperName(HUNIT hero) { WAR3MAPCPP_CALL_JASS(GetHeroProperName, hero); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetHeroSkillPoints(HUNIT hero) { WAR3MAPCPP_CALL_JASS(GetHeroSkillPoints, hero); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetHeroStr(HUNIT hero, BOOLEAN includeBonuses) { WAR3MAPCPP_CALL_JASS(GetHeroStr, hero, includeBonuses); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetHeroXP(HUNIT hero) { WAR3MAPCPP_CALL_JASS(GetHeroXP, hero); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetIntegerGameState(HIGAMESTATE IntegerGameState) { WAR3MAPCPP_CALL_JASS(GetIntegerGameState, IntegerGameState); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetIssuedOrderId() { WAR3MAPCPP_CALL_JASS(GetIssuedOrderId); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetItemCharges(HITEM item) { WAR3MAPCPP_CALL_JASS(GetItemCharges, item); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetItemLevel(HITEM item) { WAR3MAPCPP_CALL_JASS(GetItemLevel, item); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI GetItemName(HITEM item) { WAR3MAPCPP_CALL_JASS(GetItemName, item); }
    WA3MAPCPP_FORCE_INLINE HPLAYER JASSAPI GetItemPlayer(HITEM item) { WAR3MAPCPP_CALL_JASS(GetItemPlayer, item); }
    WA3MAPCPP_FORCE_INLINE HITEMTYPE JASSAPI GetItemType(HITEM item) { WAR3MAPCPP_CALL_JASS(GetItemType, item); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetItemTypeId(HITEM i) { WAR3MAPCPP_CALL_JASS(GetItemTypeId, i); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetItemUserData(HITEM item) { WAR3MAPCPP_CALL_JASS(GetItemUserData, item); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetItemX(HITEM i) { WAR3MAPCPP_CALL_JASS(GetItemX, i); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetItemY(HITEM i) { WAR3MAPCPP_CALL_JASS(GetItemY, i); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetKillingUnit() { WAR3MAPCPP_CALL_JASS(GetKillingUnit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetLastCommand() { WAR3MAPCPP_CALL_JASS(GetLastCommand); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetLastData() { WAR3MAPCPP_CALL_JASS(GetLastData); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetLearnedSkill() { WAR3MAPCPP_CALL_JASS(GetLearnedSkill); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetLearnedSkillLevel() { WAR3MAPCPP_CALL_JASS(GetLearnedSkillLevel); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetLearningUnit() { WAR3MAPCPP_CALL_JASS(GetLearningUnit); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetLeavingUnit() { WAR3MAPCPP_CALL_JASS(GetLeavingUnit); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetLevelingUnit() { WAR3MAPCPP_CALL_JASS(GetLevelingUnit); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetLightningColorA(HLIGHTNING Bolt) { WAR3MAPCPP_CALL_JASS(GetLightningColorA, Bolt); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetLightningColorB(HLIGHTNING Bolt) { WAR3MAPCPP_CALL_JASS(GetLightningColorB, Bolt); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetLightningColorG(HLIGHTNING Bolt) { WAR3MAPCPP_CALL_JASS(GetLightningColorG, Bolt); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetLightningColorR(HLIGHTNING Bolt) { WAR3MAPCPP_CALL_JASS(GetLightningColorR, Bolt); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetLoadedUnit() { WAR3MAPCPP_CALL_JASS(GetLoadedUnit); }
    WA3MAPCPP_FORCE_INLINE HPLAYER JASSAPI GetLocalPlayer() { WAR3MAPCPP_CALL_JASS(GetLocalPlayer); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetLocalizedHotkey(CJassString source) { WAR3MAPCPP_CALL_JASS(GetLocalizedHotkey, source); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI GetLocalizedString(CJassString source) { WAR3MAPCPP_CALL_JASS(GetLocalizedString, source); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetLocationX(HLOCATION Location) { WAR3MAPCPP_CALL_JASS(GetLocationX, Location); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetLocationY(HLOCATION Location) { WAR3MAPCPP_CALL_JASS(GetLocationY, Location); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetLocationZ(HLOCATION Location) { WAR3MAPCPP_CALL_JASS(GetLocationZ, Location); }
    WA3MAPCPP_FORCE_INLINE HITEM JASSAPI GetManipulatedItem() { WAR3MAPCPP_CALL_JASS(GetManipulatedItem); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetManipulatingUnit() { WAR3MAPCPP_CALL_JASS(GetManipulatingUnit); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetMegaTarget() { WAR3MAPCPP_CALL_JASS(GetMegaTarget); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetMinesOwned() { WAR3MAPCPP_CALL_JASS(GetMinesOwned); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetNextExpansion() { WAR3MAPCPP_CALL_JASS(GetNextExpansion); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI GetObjectName(int objectId) { WAR3MAPCPP_CALL_JASS(GetObjectName, objectId); }
    WA3MAPCPP_FORCE_INLINE HLOCATION JASSAPI GetOrderPointLoc() { WAR3MAPCPP_CALL_JASS(GetOrderPointLoc); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetOrderPointX() { WAR3MAPCPP_CALL_JASS(GetOrderPointX); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetOrderPointY() { WAR3MAPCPP_CALL_JASS(GetOrderPointY); }
    WA3MAPCPP_FORCE_INLINE HWIDGET JASSAPI GetOrderTarget() { WAR3MAPCPP_CALL_JASS(GetOrderTarget); }
    WA3MAPCPP_FORCE_INLINE HDESTRUCTABLE JASSAPI GetOrderTargetDestructable() { WAR3MAPCPP_CALL_JASS(GetOrderTargetDestructable); }
    WA3MAPCPP_FORCE_INLINE HITEM JASSAPI GetOrderTargetItem() { WAR3MAPCPP_CALL_JASS(GetOrderTargetItem); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetOrderTargetUnit() { WAR3MAPCPP_CALL_JASS(GetOrderTargetUnit); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetOrderedUnit() { WAR3MAPCPP_CALL_JASS(GetOrderedUnit); }
    WA3MAPCPP_FORCE_INLINE HPLAYER JASSAPI GetOwningPlayer(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetOwningPlayer, unit); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI GetPlayerAlliance(HPLAYER sourcePlayer, HPLAYER otherPlayer, HALLIANCETYPE AllianceSetting) { WAR3MAPCPP_CALL_JASS(GetPlayerAlliance, sourcePlayer, otherPlayer, AllianceSetting); }
    WA3MAPCPP_FORCE_INLINE HPLAYERCOLOR JASSAPI GetPlayerColor(HPLAYER player) { WAR3MAPCPP_CALL_JASS(GetPlayerColor, player); }
    WA3MAPCPP_FORCE_INLINE HMAPCONTROL JASSAPI GetPlayerController(HPLAYER player) { WAR3MAPCPP_CALL_JASS(GetPlayerController, player); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetPlayerHandicap(HPLAYER player) { WAR3MAPCPP_CALL_JASS(GetPlayerHandicap, player); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetPlayerHandicapXP(HPLAYER player) { WAR3MAPCPP_CALL_JASS(GetPlayerHandicapXP, player); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetPlayerId(HPLAYER player) { WAR3MAPCPP_CALL_JASS(GetPlayerId, player); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI GetPlayerName(HPLAYER player) { WAR3MAPCPP_CALL_JASS(GetPlayerName, player); }
    WA3MAPCPP_FORCE_INLINE HRACE JASSAPI GetPlayerRace(HPLAYER player) { WAR3MAPCPP_CALL_JASS(GetPlayerRace, player); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetPlayerScore(HPLAYER player, HPLAYERSCORE PlayerScore) { WAR3MAPCPP_CALL_JASS(GetPlayerScore, player, PlayerScore); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI GetPlayerSelectable(HPLAYER player) { WAR3MAPCPP_CALL_JASS(GetPlayerSelectable, player); }
    WA3MAPCPP_FORCE_INLINE HPLAYERSLOTSTATE JASSAPI GetPlayerSlotState(HPLAYER player) { WAR3MAPCPP_CALL_JASS(GetPlayerSlotState, player); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetPlayerStartLocation(HPLAYER player) { WAR3MAPCPP_CALL_JASS(GetPlayerStartLocation, player); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetPlayerStartLocationX(HPLAYER arg1) { WAR3MAPCPP_CALL_JASS(GetPlayerStartLocationX, arg1); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetPlayerStartLocationY(HPLAYER arg1) { WAR3MAPCPP_CALL_JASS(GetPlayerStartLocationY, arg1); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetPlayerState(HPLAYER player, HPLAYERSTATE PlayerState) { WAR3MAPCPP_CALL_JASS(GetPlayerState, player, PlayerState); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetPlayerStructureCount(HPLAYER player, BOOLEAN includeIncomplete) { WAR3MAPCPP_CALL_JASS(GetPlayerStructureCount, player, includeIncomplete); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetPlayerTaxRate(HPLAYER sourcePlayer, HPLAYER otherPlayer, HPLAYERSTATE Resource) { WAR3MAPCPP_CALL_JASS(GetPlayerTaxRate, sourcePlayer, otherPlayer, Resource); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetPlayerTeam(HPLAYER player) { WAR3MAPCPP_CALL_JASS(GetPlayerTeam, player); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetPlayerTechCount(HPLAYER player, int techid, BOOLEAN specificonly) { WAR3MAPCPP_CALL_JASS(GetPlayerTechCount, player, techid, specificonly); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetPlayerTechMaxAllowed(HPLAYER player, int techid) { WAR3MAPCPP_CALL_JASS(GetPlayerTechMaxAllowed, player, techid); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI GetPlayerTechResearched(HPLAYER player, int techid, BOOLEAN specificonly) { WAR3MAPCPP_CALL_JASS(GetPlayerTechResearched, player, techid, specificonly); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetPlayerTypedUnitCount(HPLAYER player, CJassString unitName, BOOLEAN includeIncomplete, BOOLEAN includeUpgrades) { WAR3MAPCPP_CALL_JASS(GetPlayerTypedUnitCount, player, unitName, includeIncomplete, includeUpgrades); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetPlayerUnitCount(HPLAYER player, BOOLEAN includeIncomplete) { WAR3MAPCPP_CALL_JASS(GetPlayerUnitCount, player, includeIncomplete); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetPlayerUnitTypeCount(HPLAYER arg1, int arg2) { WAR3MAPCPP_CALL_JASS(GetPlayerUnitTypeCount, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetPlayers() { WAR3MAPCPP_CALL_JASS(GetPlayers); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetRandomInt(int lowBound, int highBound) { WAR3MAPCPP_CALL_JASS(GetRandomInt, lowBound, highBound); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetRandomReal(float lowBound, float highBound) { WAR3MAPCPP_CALL_JASS(GetRandomReal, lowBound, highBound); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetRectCenterX(HRECT Rect) { WAR3MAPCPP_CALL_JASS(GetRectCenterX, Rect); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetRectCenterY(HRECT Rect) { WAR3MAPCPP_CALL_JASS(GetRectCenterY, Rect); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetRectMaxX(HRECT Rect) { WAR3MAPCPP_CALL_JASS(GetRectMaxX, Rect); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetRectMaxY(HRECT Rect) { WAR3MAPCPP_CALL_JASS(GetRectMaxY, Rect); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetRectMinX(HRECT Rect) { WAR3MAPCPP_CALL_JASS(GetRectMinX, Rect); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetRectMinY(HRECT Rect) { WAR3MAPCPP_CALL_JASS(GetRectMinY, Rect); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetRescuer() { WAR3MAPCPP_CALL_JASS(GetRescuer); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetResearched() { WAR3MAPCPP_CALL_JASS(GetResearched); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetResearchingUnit() { WAR3MAPCPP_CALL_JASS(GetResearchingUnit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetResourceAmount(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetResourceAmount, unit); }
    WA3MAPCPP_FORCE_INLINE HMAPDENSITY JASSAPI GetResourceDensity() { WAR3MAPCPP_CALL_JASS(GetResourceDensity); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetRevivableUnit() { WAR3MAPCPP_CALL_JASS(GetRevivableUnit); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetRevivingUnit() { WAR3MAPCPP_CALL_JASS(GetRevivingUnit); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI GetSaveBasicFilename() { WAR3MAPCPP_CALL_JASS(GetSaveBasicFilename); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetSellingUnit() { WAR3MAPCPP_CALL_JASS(GetSellingUnit); }
    WA3MAPCPP_FORCE_INLINE HITEM JASSAPI GetSoldItem() { WAR3MAPCPP_CALL_JASS(GetSoldItem); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetSoldUnit() { WAR3MAPCPP_CALL_JASS(GetSoldUnit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetSoundDuration(HSOUND soundHandle) { WAR3MAPCPP_CALL_JASS(GetSoundDuration, soundHandle); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetSoundFileDuration(CJassString musicFileName) { WAR3MAPCPP_CALL_JASS(GetSoundFileDuration, musicFileName); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI GetSoundIsLoading(HSOUND soundHandle) { WAR3MAPCPP_CALL_JASS(GetSoundIsLoading, soundHandle); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI GetSoundIsPlaying(HSOUND soundHandle) { WAR3MAPCPP_CALL_JASS(GetSoundIsPlaying, soundHandle); }
    WA3MAPCPP_FORCE_INLINE HABILITY JASSAPI GetSpellAbility() { WAR3MAPCPP_CALL_JASS(GetSpellAbility); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetSpellAbilityId() { WAR3MAPCPP_CALL_JASS(GetSpellAbilityId); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetSpellAbilityUnit() { WAR3MAPCPP_CALL_JASS(GetSpellAbilityUnit); }
    WA3MAPCPP_FORCE_INLINE HDESTRUCTABLE JASSAPI GetSpellTargetDestructable() { WAR3MAPCPP_CALL_JASS(GetSpellTargetDestructable); }
    WA3MAPCPP_FORCE_INLINE HITEM JASSAPI GetSpellTargetItem() { WAR3MAPCPP_CALL_JASS(GetSpellTargetItem); }
    WA3MAPCPP_FORCE_INLINE HLOCATION JASSAPI GetSpellTargetLoc() { WAR3MAPCPP_CALL_JASS(GetSpellTargetLoc); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetSpellTargetUnit() { WAR3MAPCPP_CALL_JASS(GetSpellTargetUnit); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetSpellTargetX() { WAR3MAPCPP_CALL_JASS(GetSpellTargetX); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetSpellTargetY() { WAR3MAPCPP_CALL_JASS(GetSpellTargetY); }
    WA3MAPCPP_FORCE_INLINE HSTARTLOCPRIO JASSAPI GetStartLocPrio(int StartLoc, int prioSlotIndex) { WAR3MAPCPP_CALL_JASS(GetStartLocPrio, StartLoc, prioSlotIndex); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetStartLocPrioSlot(int StartLoc, int prioSlotIndex) { WAR3MAPCPP_CALL_JASS(GetStartLocPrioSlot, StartLoc, prioSlotIndex); }
    WA3MAPCPP_FORCE_INLINE HLOCATION JASSAPI GetStartLocationLoc(int StartLocation) { WAR3MAPCPP_CALL_JASS(GetStartLocationLoc, StartLocation); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetStartLocationX(int StartLocation) { WAR3MAPCPP_CALL_JASS(GetStartLocationX, StartLocation); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetStartLocationY(int StartLocation) { WAR3MAPCPP_CALL_JASS(GetStartLocationY, StartLocation); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI GetStoredBoolean(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(GetStoredBoolean, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetStoredInteger(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(GetStoredInteger, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetStoredReal(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(GetStoredReal, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI GetStoredString(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(GetStoredString, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetSummonedUnit() { WAR3MAPCPP_CALL_JASS(GetSummonedUnit); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetSummoningUnit() { WAR3MAPCPP_CALL_JASS(GetSummoningUnit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetTeams() { WAR3MAPCPP_CALL_JASS(GetTeams); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetTerrainCliffLevel(float x, float y) { WAR3MAPCPP_CALL_JASS(GetTerrainCliffLevel, x, y); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetTerrainType(float x, float y) { WAR3MAPCPP_CALL_JASS(GetTerrainType, x, y); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetTerrainVariance(float x, float y) { WAR3MAPCPP_CALL_JASS(GetTerrainVariance, x, y); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetTimeOfDayScale() { WAR3MAPCPP_CALL_JASS(GetTimeOfDayScale); }
    WA3MAPCPP_FORCE_INLINE HPLAYER JASSAPI GetTournamentFinishNowPlayer() { WAR3MAPCPP_CALL_JASS(GetTournamentFinishNowPlayer); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetTournamentFinishNowRule() { WAR3MAPCPP_CALL_JASS(GetTournamentFinishNowRule); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetTournamentFinishSoonTimeRemaining() { WAR3MAPCPP_CALL_JASS(GetTournamentFinishSoonTimeRemaining); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetTournamentScore(HPLAYER player) { WAR3MAPCPP_CALL_JASS(GetTournamentScore, player); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetTownUnitCount(int arg1, int arg2, BOOLEAN arg3) { WAR3MAPCPP_CALL_JASS(GetTownUnitCount, arg1, arg2, arg3); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetTrainedUnit() { WAR3MAPCPP_CALL_JASS(GetTrainedUnit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetTrainedUnitType() { WAR3MAPCPP_CALL_JASS(GetTrainedUnitType); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetTransportUnit() { WAR3MAPCPP_CALL_JASS(GetTransportUnit); }
    WA3MAPCPP_FORCE_INLINE HDESTRUCTABLE JASSAPI GetTriggerDestructable() { WAR3MAPCPP_CALL_JASS(GetTriggerDestructable); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetTriggerEvalCount(HTRIGGER Trigger) { WAR3MAPCPP_CALL_JASS(GetTriggerEvalCount, Trigger); }
    WA3MAPCPP_FORCE_INLINE HEVENTID JASSAPI GetTriggerEventId() { WAR3MAPCPP_CALL_JASS(GetTriggerEventId); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetTriggerExecCount(HTRIGGER Trigger) { WAR3MAPCPP_CALL_JASS(GetTriggerExecCount, Trigger); }
    WA3MAPCPP_FORCE_INLINE HPLAYER JASSAPI GetTriggerPlayer() { WAR3MAPCPP_CALL_JASS(GetTriggerPlayer); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetTriggerUnit() { WAR3MAPCPP_CALL_JASS(GetTriggerUnit); }
    WA3MAPCPP_FORCE_INLINE HWIDGET JASSAPI GetTriggerWidget() { WAR3MAPCPP_CALL_JASS(GetTriggerWidget); }
    WA3MAPCPP_FORCE_INLINE HREGION JASSAPI GetTriggeringRegion() { WAR3MAPCPP_CALL_JASS(GetTriggeringRegion); }
    WA3MAPCPP_FORCE_INLINE HTRACKABLE JASSAPI GetTriggeringTrackable() { WAR3MAPCPP_CALL_JASS(GetTriggeringTrackable); }
    WA3MAPCPP_FORCE_INLINE HTRIGGER JASSAPI GetTriggeringTrigger() { WAR3MAPCPP_CALL_JASS(GetTriggeringTrigger); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUnitAbilityLevel(HUNIT unit, int abilcode) { WAR3MAPCPP_CALL_JASS(GetUnitAbilityLevel, unit, abilcode); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetUnitAcquireRange(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitAcquireRange, unit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUnitBuildTime(int arg1) { WAR3MAPCPP_CALL_JASS(GetUnitBuildTime, arg1); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUnitCount(int arg1) { WAR3MAPCPP_CALL_JASS(GetUnitCount, arg1); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUnitCountDone(int arg1) { WAR3MAPCPP_CALL_JASS(GetUnitCountDone, arg1); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUnitCurrentOrder(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitCurrentOrder, unit); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetUnitDefaultAcquireRange(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitDefaultAcquireRange, unit); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetUnitDefaultFlyHeight(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitDefaultFlyHeight, unit); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetUnitDefaultMoveSpeed(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitDefaultMoveSpeed, unit); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetUnitDefaultPropWindow(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitDefaultPropWindow, unit); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetUnitDefaultTurnSpeed(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitDefaultTurnSpeed, unit); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetUnitFacing(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitFacing, unit); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetUnitFlyHeight(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitFlyHeight, unit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUnitFoodMade(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitFoodMade, unit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUnitFoodUsed(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitFoodUsed, unit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUnitGoldCost(int arg1) { WAR3MAPCPP_CALL_JASS(GetUnitGoldCost, arg1); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUnitLevel(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitLevel, unit); }
    WA3MAPCPP_FORCE_INLINE HLOCATION JASSAPI GetUnitLoc(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitLoc, unit); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetUnitMoveSpeed(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitMoveSpeed, unit); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI GetUnitName(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitName, unit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUnitPointValue(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitPointValue, unit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUnitPointValueByType(int unitType) { WAR3MAPCPP_CALL_JASS(GetUnitPointValueByType, unitType); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetUnitPropWindow(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitPropWindow, unit); }
    WA3MAPCPP_FORCE_INLINE HRACE JASSAPI GetUnitRace(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitRace, unit); }
    WA3MAPCPP_FORCE_INLINE HDESTRUCTABLE JASSAPI GetUnitRallyDestructable(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitRallyDestructable, unit); }
    WA3MAPCPP_FORCE_INLINE HLOCATION JASSAPI GetUnitRallyPoint(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitRallyPoint, unit); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI GetUnitRallyUnit(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitRallyUnit, unit); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetUnitState(HUNIT unit, HUNITSTATE UnitState) { WAR3MAPCPP_CALL_JASS(GetUnitState, unit, UnitState); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetUnitTurnSpeed(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitTurnSpeed, unit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUnitTypeId(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitTypeId, unit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUnitUserData(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitUserData, unit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUnitWoodCost(int arg1) { WAR3MAPCPP_CALL_JASS(GetUnitWoodCost, arg1); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetUnitX(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitX, unit); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetUnitY(HUNIT unit) { WAR3MAPCPP_CALL_JASS(GetUnitY, unit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUpgradeGoldCost(int arg1) { WAR3MAPCPP_CALL_JASS(GetUpgradeGoldCost, arg1); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUpgradeLevel(int arg1) { WAR3MAPCPP_CALL_JASS(GetUpgradeLevel, arg1); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI GetUpgradeWoodCost(int arg1) { WAR3MAPCPP_CALL_JASS(GetUpgradeWoodCost, arg1); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetWidgetLife(HWIDGET widget) { WAR3MAPCPP_CALL_JASS(GetWidgetLife, widget); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetWidgetX(HWIDGET widget) { WAR3MAPCPP_CALL_JASS(GetWidgetX, widget); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI GetWidgetY(HWIDGET widget) { WAR3MAPCPP_CALL_JASS(GetWidgetY, widget); }
    WA3MAPCPP_FORCE_INLINE HPLAYER JASSAPI GetWinningPlayer() { WAR3MAPCPP_CALL_JASS(GetWinningPlayer); }
    WA3MAPCPP_FORCE_INLINE HRECT JASSAPI GetWorldBounds() { WAR3MAPCPP_CALL_JASS(GetWorldBounds); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI GroupAddUnit(HGROUP Group, HUNIT unit) { WAR3MAPCPP_CALL_JASS(GroupAddUnit, Group, unit); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI GroupClear(HGROUP Group) { WAR3MAPCPP_CALL_JASS(GroupClear, Group); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI GroupEnumUnitsInRange(HGROUP Group, float x, float y, float radius, HBOOLEXPR filter) { WAR3MAPCPP_CALL_JASS(GroupEnumUnitsInRange, Group, x, y, radius, filter); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI GroupEnumUnitsInRangeCounted(HGROUP Group, float x, float y, float radius, HBOOLEXPR filter, int countLimit) { WAR3MAPCPP_CALL_JASS(GroupEnumUnitsInRangeCounted, Group, x, y, radius, filter, countLimit); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI GroupEnumUnitsInRangeOfLoc(HGROUP Group, HLOCATION Location, float radius, HBOOLEXPR filter) { WAR3MAPCPP_CALL_JASS(GroupEnumUnitsInRangeOfLoc, Group, Location, radius, filter); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI GroupEnumUnitsInRangeOfLocCounted(HGROUP Group, HLOCATION Location, float radius, HBOOLEXPR filter, int countLimit) { WAR3MAPCPP_CALL_JASS(GroupEnumUnitsInRangeOfLocCounted, Group, Location, radius, filter, countLimit); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI GroupEnumUnitsInRect(HGROUP Group, HRECT r, HBOOLEXPR filter) { WAR3MAPCPP_CALL_JASS(GroupEnumUnitsInRect, Group, r, filter); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI GroupEnumUnitsInRectCounted(HGROUP Group, HRECT r, HBOOLEXPR filter, int countLimit) { WAR3MAPCPP_CALL_JASS(GroupEnumUnitsInRectCounted, Group, r, filter, countLimit); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI GroupEnumUnitsOfPlayer(HGROUP Group, HPLAYER player, HBOOLEXPR filter) { WAR3MAPCPP_CALL_JASS(GroupEnumUnitsOfPlayer, Group, player, filter); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI GroupEnumUnitsOfType(HGROUP Group, CJassString unitname, HBOOLEXPR filter) { WAR3MAPCPP_CALL_JASS(GroupEnumUnitsOfType, Group, unitname, filter); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI GroupEnumUnitsOfTypeCounted(HGROUP Group, CJassString unitname, HBOOLEXPR filter, int countLimit) { WAR3MAPCPP_CALL_JASS(GroupEnumUnitsOfTypeCounted, Group, unitname, filter, countLimit); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI GroupEnumUnitsSelected(HGROUP Group, HPLAYER player, HBOOLEXPR filter) { WAR3MAPCPP_CALL_JASS(GroupEnumUnitsSelected, Group, player, filter); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI GroupImmediateOrder(HGROUP Group, CJassString order) { WAR3MAPCPP_CALL_JASS(GroupImmediateOrder, Group, order); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI GroupImmediateOrderById(HGROUP Group, int order) { WAR3MAPCPP_CALL_JASS(GroupImmediateOrderById, Group, order); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI GroupPointOrder(HGROUP Group, CJassString order, float x, float y) { WAR3MAPCPP_CALL_JASS(GroupPointOrder, Group, order, x, y); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI GroupPointOrderById(HGROUP Group, int order, float x, float y) { WAR3MAPCPP_CALL_JASS(GroupPointOrderById, Group, order, x, y); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI GroupPointOrderByIdLoc(HGROUP Group, int order, HLOCATION Location) { WAR3MAPCPP_CALL_JASS(GroupPointOrderByIdLoc, Group, order, Location); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI GroupPointOrderLoc(HGROUP Group, CJassString order, HLOCATION Location) { WAR3MAPCPP_CALL_JASS(GroupPointOrderLoc, Group, order, Location); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI GroupRemoveUnit(HGROUP Group, HUNIT unit) { WAR3MAPCPP_CALL_JASS(GroupRemoveUnit, Group, unit); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI GroupTargetOrder(HGROUP Group, CJassString order, HWIDGET targetWidget) { WAR3MAPCPP_CALL_JASS(GroupTargetOrder, Group, order, targetWidget); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI GroupTargetOrderById(HGROUP Group, int order, HWIDGET targetWidget) { WAR3MAPCPP_CALL_JASS(GroupTargetOrderById, Group, order, targetWidget); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI GroupTimedLife(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(GroupTimedLife, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI HarvestGold(int arg1, int arg2) { WAR3MAPCPP_CALL_JASS(HarvestGold, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI HarvestWood(int arg1, int arg2) { WAR3MAPCPP_CALL_JASS(HarvestWood, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI HaveSavedBoolean(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(HaveSavedBoolean, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI HaveSavedHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(HaveSavedHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI HaveSavedInteger(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(HaveSavedInteger, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI HaveSavedReal(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(HaveSavedReal, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI HaveSavedString(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(HaveSavedString, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI HaveStoredBoolean(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(HaveStoredBoolean, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI HaveStoredInteger(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(HaveStoredInteger, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI HaveStoredReal(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(HaveStoredReal, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI HaveStoredString(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(HaveStoredString, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI HaveStoredUnit(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(HaveStoredUnit, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI I2R(int i) { WAR3MAPCPP_CALL_JASS(I2R, i); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI I2S(int i) { WAR3MAPCPP_CALL_JASS(I2S, i); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI IgnoredUnits(int arg1) { WAR3MAPCPP_CALL_JASS(IgnoredUnits, arg1); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI IncUnitAbilityLevel(HUNIT unit, int abilcode) { WAR3MAPCPP_CALL_JASS(IncUnitAbilityLevel, unit, abilcode); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI InitAssault() { WAR3MAPCPP_CALL_JASS(InitAssault); }
    WA3MAPCPP_FORCE_INLINE HGAMECACHE JASSAPI InitGameCache(CJassString campaignFile) { WAR3MAPCPP_CALL_JASS(InitGameCache, campaignFile); }
    WA3MAPCPP_FORCE_INLINE HHASHTABLE JASSAPI InitHashtable() { WAR3MAPCPP_CALL_JASS(InitHashtable); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsCineFilterDisplayed() { WAR3MAPCPP_CALL_JASS(IsCineFilterDisplayed); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsDestructableInvulnerable(HDESTRUCTABLE d) { WAR3MAPCPP_CALL_JASS(IsDestructableInvulnerable, d); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsFogEnabled() { WAR3MAPCPP_CALL_JASS(IsFogEnabled); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsFogMaskEnabled() { WAR3MAPCPP_CALL_JASS(IsFogMaskEnabled); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsFoggedToPlayer(float x, float y, HPLAYER player) { WAR3MAPCPP_CALL_JASS(IsFoggedToPlayer, x, y, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsGameTypeSupported(HGAMETYPE GameType) { WAR3MAPCPP_CALL_JASS(IsGameTypeSupported, GameType); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsHeroUnitId(int unitId) { WAR3MAPCPP_CALL_JASS(IsHeroUnitId, unitId); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsItemIdPawnable(int itemId) { WAR3MAPCPP_CALL_JASS(IsItemIdPawnable, itemId); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsItemIdPowerup(int itemId) { WAR3MAPCPP_CALL_JASS(IsItemIdPowerup, itemId); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsItemIdSellable(int itemId) { WAR3MAPCPP_CALL_JASS(IsItemIdSellable, itemId); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsItemInvulnerable(HITEM item) { WAR3MAPCPP_CALL_JASS(IsItemInvulnerable, item); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsItemOwned(HITEM item) { WAR3MAPCPP_CALL_JASS(IsItemOwned, item); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsItemPawnable(HITEM item) { WAR3MAPCPP_CALL_JASS(IsItemPawnable, item); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsItemPowerup(HITEM item) { WAR3MAPCPP_CALL_JASS(IsItemPowerup, item); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsItemSellable(HITEM item) { WAR3MAPCPP_CALL_JASS(IsItemSellable, item); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsItemVisible(HITEM item) { WAR3MAPCPP_CALL_JASS(IsItemVisible, item); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsLeaderboardDisplayed(HLEADERBOARD lb) { WAR3MAPCPP_CALL_JASS(IsLeaderboardDisplayed, lb); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsLocationFoggedToPlayer(HLOCATION Location, HPLAYER player) { WAR3MAPCPP_CALL_JASS(IsLocationFoggedToPlayer, Location, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsLocationInRegion(HREGION Region, HLOCATION Location) { WAR3MAPCPP_CALL_JASS(IsLocationInRegion, Region, Location); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsLocationMaskedToPlayer(HLOCATION Location, HPLAYER player) { WAR3MAPCPP_CALL_JASS(IsLocationMaskedToPlayer, Location, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsLocationVisibleToPlayer(HLOCATION Location, HPLAYER player) { WAR3MAPCPP_CALL_JASS(IsLocationVisibleToPlayer, Location, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsMapFlagSet(HMAPFLAG MapFlag) { WAR3MAPCPP_CALL_JASS(IsMapFlagSet, MapFlag); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsMaskedToPlayer(float x, float y, HPLAYER player) { WAR3MAPCPP_CALL_JASS(IsMaskedToPlayer, x, y, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsMultiboardDisplayed(HMULTIBOARD lb) { WAR3MAPCPP_CALL_JASS(IsMultiboardDisplayed, lb); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsMultiboardMinimized(HMULTIBOARD lb) { WAR3MAPCPP_CALL_JASS(IsMultiboardMinimized, lb); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsNoDefeatCheat() { WAR3MAPCPP_CALL_JASS(IsNoDefeatCheat); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsNoVictoryCheat() { WAR3MAPCPP_CALL_JASS(IsNoVictoryCheat); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsPlayerAlly(HPLAYER player, HPLAYER otherPlayer) { WAR3MAPCPP_CALL_JASS(IsPlayerAlly, player, otherPlayer); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsPlayerEnemy(HPLAYER player, HPLAYER otherPlayer) { WAR3MAPCPP_CALL_JASS(IsPlayerEnemy, player, otherPlayer); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsPlayerInForce(HPLAYER player, HFORCE Force) { WAR3MAPCPP_CALL_JASS(IsPlayerInForce, player, Force); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsPlayerObserver(HPLAYER player) { WAR3MAPCPP_CALL_JASS(IsPlayerObserver, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsPlayerRacePrefSet(HPLAYER player, HRACEPREFERENCE pref) { WAR3MAPCPP_CALL_JASS(IsPlayerRacePrefSet, player, pref); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsPointBlighted(float x, float y) { WAR3MAPCPP_CALL_JASS(IsPointBlighted, x, y); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsPointInRegion(HREGION Region, float x, float y) { WAR3MAPCPP_CALL_JASS(IsPointInRegion, Region, x, y); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsQuestCompleted(HQUEST Quest) { WAR3MAPCPP_CALL_JASS(IsQuestCompleted, Quest); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsQuestDiscovered(HQUEST Quest) { WAR3MAPCPP_CALL_JASS(IsQuestDiscovered, Quest); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsQuestEnabled(HQUEST Quest) { WAR3MAPCPP_CALL_JASS(IsQuestEnabled, Quest); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsQuestFailed(HQUEST Quest) { WAR3MAPCPP_CALL_JASS(IsQuestFailed, Quest); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsQuestItemCompleted(HQUESTITEM QuestItem) { WAR3MAPCPP_CALL_JASS(IsQuestItemCompleted, QuestItem); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsQuestRequired(HQUEST Quest) { WAR3MAPCPP_CALL_JASS(IsQuestRequired, Quest); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsSuspendedXP(HUNIT hero) { WAR3MAPCPP_CALL_JASS(IsSuspendedXP, hero); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsTerrainPathable(float x, float y, HPATHINGTYPE t) { WAR3MAPCPP_CALL_JASS(IsTerrainPathable, x, y, t); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsTimerDialogDisplayed(HTIMERDIALOG Dialog) { WAR3MAPCPP_CALL_JASS(IsTimerDialogDisplayed, Dialog); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsTowered(HUNIT arg1) { WAR3MAPCPP_CALL_JASS(IsTowered, arg1); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsTriggerEnabled(HTRIGGER Trigger) { WAR3MAPCPP_CALL_JASS(IsTriggerEnabled, Trigger); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsTriggerWaitOnSleeps(HTRIGGER Trigger) { WAR3MAPCPP_CALL_JASS(IsTriggerWaitOnSleeps, Trigger); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnit(HUNIT unit, HUNIT SpecifiedUnit) { WAR3MAPCPP_CALL_JASS(IsUnit, unit, SpecifiedUnit); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitAlly(HUNIT unit, HPLAYER player) { WAR3MAPCPP_CALL_JASS(IsUnitAlly, unit, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitDetected(HUNIT unit, HPLAYER player) { WAR3MAPCPP_CALL_JASS(IsUnitDetected, unit, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitEnemy(HUNIT unit, HPLAYER player) { WAR3MAPCPP_CALL_JASS(IsUnitEnemy, unit, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitFogged(HUNIT unit, HPLAYER player) { WAR3MAPCPP_CALL_JASS(IsUnitFogged, unit, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitHidden(HUNIT unit) { WAR3MAPCPP_CALL_JASS(IsUnitHidden, unit); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitIdType(int unitId, HUNITTYPE UnitType) { WAR3MAPCPP_CALL_JASS(IsUnitIdType, unitId, UnitType); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitIllusion(HUNIT unit) { WAR3MAPCPP_CALL_JASS(IsUnitIllusion, unit); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitInForce(HUNIT unit, HFORCE Force) { WAR3MAPCPP_CALL_JASS(IsUnitInForce, unit, Force); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitInGroup(HUNIT unit, HGROUP Group) { WAR3MAPCPP_CALL_JASS(IsUnitInGroup, unit, Group); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitInRange(HUNIT unit, HUNIT otherUnit, float distance) { WAR3MAPCPP_CALL_JASS(IsUnitInRange, unit, otherUnit, distance); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitInRangeLoc(HUNIT unit, HLOCATION Location, float distance) { WAR3MAPCPP_CALL_JASS(IsUnitInRangeLoc, unit, Location, distance); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitInRangeXY(HUNIT unit, float x, float y, float distance) { WAR3MAPCPP_CALL_JASS(IsUnitInRangeXY, unit, x, y, distance); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitInRegion(HREGION Region, HUNIT unit) { WAR3MAPCPP_CALL_JASS(IsUnitInRegion, Region, unit); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitInTransport(HUNIT unit, HUNIT Transport) { WAR3MAPCPP_CALL_JASS(IsUnitInTransport, unit, Transport); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitInvisible(HUNIT unit, HPLAYER player) { WAR3MAPCPP_CALL_JASS(IsUnitInvisible, unit, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitLoaded(HUNIT unit) { WAR3MAPCPP_CALL_JASS(IsUnitLoaded, unit); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitMasked(HUNIT unit, HPLAYER player) { WAR3MAPCPP_CALL_JASS(IsUnitMasked, unit, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitOwnedByPlayer(HUNIT unit, HPLAYER player) { WAR3MAPCPP_CALL_JASS(IsUnitOwnedByPlayer, unit, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitPaused(HUNIT hero) { WAR3MAPCPP_CALL_JASS(IsUnitPaused, hero); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitRace(HUNIT unit, HRACE Race) { WAR3MAPCPP_CALL_JASS(IsUnitRace, unit, Race); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitSelected(HUNIT unit, HPLAYER player) { WAR3MAPCPP_CALL_JASS(IsUnitSelected, unit, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitType(HUNIT unit, HUNITTYPE UnitType) { WAR3MAPCPP_CALL_JASS(IsUnitType, unit, UnitType); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsUnitVisible(HUNIT unit, HPLAYER player) { WAR3MAPCPP_CALL_JASS(IsUnitVisible, unit, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IsVisibleToPlayer(float x, float y, HPLAYER player) { WAR3MAPCPP_CALL_JASS(IsVisibleToPlayer, x, y, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssueBuildOrder(HUNIT Peon, CJassString unitToBuild, float x, float y) { WAR3MAPCPP_CALL_JASS(IssueBuildOrder, Peon, unitToBuild, x, y); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssueBuildOrderById(HUNIT Peon, int unitId, float x, float y) { WAR3MAPCPP_CALL_JASS(IssueBuildOrderById, Peon, unitId, x, y); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssueImmediateOrder(HUNIT unit, CJassString order) { WAR3MAPCPP_CALL_JASS(IssueImmediateOrder, unit, order); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssueImmediateOrderById(HUNIT unit, int order) { WAR3MAPCPP_CALL_JASS(IssueImmediateOrderById, unit, order); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssueInstantPointOrder(HUNIT unit, CJassString order, float x, float y, HWIDGET instantTargetWidget) { WAR3MAPCPP_CALL_JASS(IssueInstantPointOrder, unit, order, x, y, instantTargetWidget); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssueInstantPointOrderById(HUNIT unit, int order, float x, float y, HWIDGET instantTargetWidget) { WAR3MAPCPP_CALL_JASS(IssueInstantPointOrderById, unit, order, x, y, instantTargetWidget); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssueInstantTargetOrder(HUNIT unit, CJassString order, HWIDGET targetWidget, HWIDGET instantTargetWidget) { WAR3MAPCPP_CALL_JASS(IssueInstantTargetOrder, unit, order, targetWidget, instantTargetWidget); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssueInstantTargetOrderById(HUNIT unit, int order, HWIDGET targetWidget, HWIDGET instantTargetWidget) { WAR3MAPCPP_CALL_JASS(IssueInstantTargetOrderById, unit, order, targetWidget, instantTargetWidget); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssueNeutralImmediateOrder(HPLAYER forWhichPlayer, HUNIT neutralStructure, CJassString unitToBuild) { WAR3MAPCPP_CALL_JASS(IssueNeutralImmediateOrder, forWhichPlayer, neutralStructure, unitToBuild); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssueNeutralImmediateOrderById(HPLAYER forWhichPlayer, HUNIT neutralStructure, int unitId) { WAR3MAPCPP_CALL_JASS(IssueNeutralImmediateOrderById, forWhichPlayer, neutralStructure, unitId); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssueNeutralPointOrder(HPLAYER forWhichPlayer, HUNIT neutralStructure, CJassString unitToBuild, float x, float y) { WAR3MAPCPP_CALL_JASS(IssueNeutralPointOrder, forWhichPlayer, neutralStructure, unitToBuild, x, y); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssueNeutralPointOrderById(HPLAYER forWhichPlayer, HUNIT neutralStructure, int unitId, float x, float y) { WAR3MAPCPP_CALL_JASS(IssueNeutralPointOrderById, forWhichPlayer, neutralStructure, unitId, x, y); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssueNeutralTargetOrder(HPLAYER forWhichPlayer, HUNIT neutralStructure, CJassString unitToBuild, HWIDGET target) { WAR3MAPCPP_CALL_JASS(IssueNeutralTargetOrder, forWhichPlayer, neutralStructure, unitToBuild, target); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssueNeutralTargetOrderById(HPLAYER forWhichPlayer, HUNIT neutralStructure, int unitId, HWIDGET target) { WAR3MAPCPP_CALL_JASS(IssueNeutralTargetOrderById, forWhichPlayer, neutralStructure, unitId, target); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssuePointOrder(HUNIT unit, CJassString order, float x, float y) { WAR3MAPCPP_CALL_JASS(IssuePointOrder, unit, order, x, y); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssuePointOrderById(HUNIT unit, int order, float x, float y) { WAR3MAPCPP_CALL_JASS(IssuePointOrderById, unit, order, x, y); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssuePointOrderByIdLoc(HUNIT unit, int order, HLOCATION Location) { WAR3MAPCPP_CALL_JASS(IssuePointOrderByIdLoc, unit, order, Location); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssuePointOrderLoc(HUNIT unit, CJassString order, HLOCATION Location) { WAR3MAPCPP_CALL_JASS(IssuePointOrderLoc, unit, order, Location); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssueTargetOrder(HUNIT unit, CJassString order, HWIDGET targetWidget) { WAR3MAPCPP_CALL_JASS(IssueTargetOrder, unit, order, targetWidget); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI IssueTargetOrderById(HUNIT unit, int order, HWIDGET targetWidget) { WAR3MAPCPP_CALL_JASS(IssueTargetOrderById, unit, order, targetWidget); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ItemPoolAddItemType(HITEMPOOL ItemPool, int itemId, float weight) { WAR3MAPCPP_CALL_JASS(ItemPoolAddItemType, ItemPool, itemId, weight); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ItemPoolRemoveItemType(HITEMPOOL ItemPool, int itemId) { WAR3MAPCPP_CALL_JASS(ItemPoolRemoveItemType, ItemPool, itemId); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI KillDestructable(HDESTRUCTABLE d) { WAR3MAPCPP_CALL_JASS(KillDestructable, d); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI KillSoundWhenDone(HSOUND soundHandle) { WAR3MAPCPP_CALL_JASS(KillSoundWhenDone, soundHandle); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI KillUnit(HUNIT unit) { WAR3MAPCPP_CALL_JASS(KillUnit, unit); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardAddItem(HLEADERBOARD lb, CJassString label, int value, HPLAYER p) { WAR3MAPCPP_CALL_JASS(LeaderboardAddItem, lb, label, value, p); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardClear(HLEADERBOARD lb) { WAR3MAPCPP_CALL_JASS(LeaderboardClear, lb); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardDisplay(HLEADERBOARD lb, BOOLEAN show) { WAR3MAPCPP_CALL_JASS(LeaderboardDisplay, lb, show); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI LeaderboardGetItemCount(HLEADERBOARD lb) { WAR3MAPCPP_CALL_JASS(LeaderboardGetItemCount, lb); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI LeaderboardGetLabelText(HLEADERBOARD lb) { WAR3MAPCPP_CALL_JASS(LeaderboardGetLabelText, lb); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI LeaderboardGetPlayerIndex(HLEADERBOARD lb, HPLAYER p) { WAR3MAPCPP_CALL_JASS(LeaderboardGetPlayerIndex, lb, p); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI LeaderboardHasPlayerItem(HLEADERBOARD lb, HPLAYER p) { WAR3MAPCPP_CALL_JASS(LeaderboardHasPlayerItem, lb, p); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardRemoveItem(HLEADERBOARD lb, int index) { WAR3MAPCPP_CALL_JASS(LeaderboardRemoveItem, lb, index); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardRemovePlayerItem(HLEADERBOARD lb, HPLAYER p) { WAR3MAPCPP_CALL_JASS(LeaderboardRemovePlayerItem, lb, p); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardSetItemLabel(HLEADERBOARD lb, int item, CJassString val) { WAR3MAPCPP_CALL_JASS(LeaderboardSetItemLabel, lb, item, val); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardSetItemLabelColor(HLEADERBOARD lb, int item, int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(LeaderboardSetItemLabelColor, lb, item, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardSetItemStyle(HLEADERBOARD lb, int item, BOOLEAN showLabel, BOOLEAN showValue, BOOLEAN showIcon) { WAR3MAPCPP_CALL_JASS(LeaderboardSetItemStyle, lb, item, showLabel, showValue, showIcon); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardSetItemValue(HLEADERBOARD lb, int item, int val) { WAR3MAPCPP_CALL_JASS(LeaderboardSetItemValue, lb, item, val); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardSetItemValueColor(HLEADERBOARD lb, int item, int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(LeaderboardSetItemValueColor, lb, item, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardSetLabel(HLEADERBOARD lb, CJassString label) { WAR3MAPCPP_CALL_JASS(LeaderboardSetLabel, lb, label); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardSetLabelColor(HLEADERBOARD lb, int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(LeaderboardSetLabelColor, lb, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardSetSizeByItemCount(HLEADERBOARD lb, int count) { WAR3MAPCPP_CALL_JASS(LeaderboardSetSizeByItemCount, lb, count); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardSetStyle(HLEADERBOARD lb, BOOLEAN showLabel, BOOLEAN showNames, BOOLEAN showValues, BOOLEAN showIcons) { WAR3MAPCPP_CALL_JASS(LeaderboardSetStyle, lb, showLabel, showNames, showValues, showIcons); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardSetValueColor(HLEADERBOARD lb, int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(LeaderboardSetValueColor, lb, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardSortItemsByLabel(HLEADERBOARD lb, BOOLEAN ascending) { WAR3MAPCPP_CALL_JASS(LeaderboardSortItemsByLabel, lb, ascending); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardSortItemsByPlayer(HLEADERBOARD lb, BOOLEAN ascending) { WAR3MAPCPP_CALL_JASS(LeaderboardSortItemsByPlayer, lb, ascending); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LeaderboardSortItemsByValue(HLEADERBOARD lb, BOOLEAN ascending) { WAR3MAPCPP_CALL_JASS(LeaderboardSortItemsByValue, lb, ascending); }
    WA3MAPCPP_FORCE_INLINE HABILITY JASSAPI LoadAbilityHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadAbilityHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI LoadBoolean(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadBoolean, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HBOOLEXPR JASSAPI LoadBooleanExprHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadBooleanExprHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HBUTTON JASSAPI LoadButtonHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadButtonHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HDEFEATCONDITION JASSAPI LoadDefeatConditionHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadDefeatConditionHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HDESTRUCTABLE JASSAPI LoadDestructableHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadDestructableHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HDIALOG JASSAPI LoadDialogHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadDialogHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HEFFECT JASSAPI LoadEffectHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadEffectHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HFOGMODIFIER JASSAPI LoadFogModifierHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadFogModifierHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HFOGSTATE JASSAPI LoadFogStateHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadFogStateHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HFORCE JASSAPI LoadForceHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadForceHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LoadGame(CJassString saveFileName, BOOLEAN doScoreScreen) { WAR3MAPCPP_CALL_JASS(LoadGame, saveFileName, doScoreScreen); }
    WA3MAPCPP_FORCE_INLINE HGROUP JASSAPI LoadGroupHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadGroupHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HHASHTABLE JASSAPI LoadHashtableHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadHashtableHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HIMAGE JASSAPI LoadImageHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadImageHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI LoadInteger(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadInteger, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HITEM JASSAPI LoadItemHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadItemHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HITEMPOOL JASSAPI LoadItemPoolHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadItemPoolHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HLEADERBOARD JASSAPI LoadLeaderboardHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadLeaderboardHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HLIGHTNING JASSAPI LoadLightningHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadLightningHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HLOCATION JASSAPI LoadLocationHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadLocationHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HMULTIBOARD JASSAPI LoadMultiboardHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadMultiboardHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HMULTIBOARDITEM JASSAPI LoadMultiboardItemHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadMultiboardItemHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HPLAYER JASSAPI LoadPlayerHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadPlayerHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HQUEST JASSAPI LoadQuestHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadQuestHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HQUESTITEM JASSAPI LoadQuestItemHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadQuestItemHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI LoadReal(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadReal, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HRECT JASSAPI LoadRectHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadRectHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HREGION JASSAPI LoadRegionHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadRegionHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HSOUND JASSAPI LoadSoundHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadSoundHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI LoadStr(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadStr, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HTEXTTAG JASSAPI LoadTextTagHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadTextTagHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HTIMERDIALOG JASSAPI LoadTimerDialogHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadTimerDialogHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HTIMER JASSAPI LoadTimerHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadTimerHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HTRACKABLE JASSAPI LoadTrackableHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadTrackableHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HTRIGGERACTION JASSAPI LoadTriggerActionHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadTriggerActionHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HTRIGGERCONDITION JASSAPI LoadTriggerConditionHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadTriggerConditionHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI LoadTriggerEventHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadTriggerEventHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HTRIGGER JASSAPI LoadTriggerHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadTriggerHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HUBERSPLAT JASSAPI LoadUbersplatHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadUbersplatHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI LoadUnitHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadUnitHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HUNITPOOL JASSAPI LoadUnitPoolHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadUnitPoolHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE HWIDGET JASSAPI LoadWidgetHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(LoadWidgetHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI LoadZepWave(int arg1, int arg2) { WAR3MAPCPP_CALL_JASS(LoadZepWave, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE HLOCATION JASSAPI Location(float x, float y) { WAR3MAPCPP_CALL_JASS(Location, x, y); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI MeleeDifficulty() { WAR3MAPCPP_CALL_JASS(MeleeDifficulty); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI MergeUnits(int arg1, int arg2, int arg3, int arg4) { WAR3MAPCPP_CALL_JASS(MergeUnits, arg1, arg2, arg3, arg4); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI MoveLightning(HLIGHTNING Bolt, BOOLEAN checkVisibility, float x1, float y1, float x2, float y2) { WAR3MAPCPP_CALL_JASS(MoveLightning, Bolt, checkVisibility, x1, y1, x2, y2); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI MoveLightningEx(HLIGHTNING Bolt, BOOLEAN checkVisibility, float x1, float y1, float z1, float x2, float y2, float z2) { WAR3MAPCPP_CALL_JASS(MoveLightningEx, Bolt, checkVisibility, x1, y1, z1, x2, y2, z2); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MoveLocation(HLOCATION Location, float newX, float newY) { WAR3MAPCPP_CALL_JASS(MoveLocation, Location, newX, newY); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MoveRectTo(HRECT Rect, float newCenterX, float newCenterY) { WAR3MAPCPP_CALL_JASS(MoveRectTo, Rect, newCenterX, newCenterY); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MoveRectToLoc(HRECT Rect, HLOCATION newCenterLoc) { WAR3MAPCPP_CALL_JASS(MoveRectToLoc, Rect, newCenterLoc); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardClear(HMULTIBOARD lb) { WAR3MAPCPP_CALL_JASS(MultiboardClear, lb); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardDisplay(HMULTIBOARD lb, BOOLEAN show) { WAR3MAPCPP_CALL_JASS(MultiboardDisplay, lb, show); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI MultiboardGetColumnCount(HMULTIBOARD lb) { WAR3MAPCPP_CALL_JASS(MultiboardGetColumnCount, lb); }
    WA3MAPCPP_FORCE_INLINE HMULTIBOARDITEM JASSAPI MultiboardGetItem(HMULTIBOARD lb, int row, int column) { WAR3MAPCPP_CALL_JASS(MultiboardGetItem, lb, row, column); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI MultiboardGetRowCount(HMULTIBOARD lb) { WAR3MAPCPP_CALL_JASS(MultiboardGetRowCount, lb); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI MultiboardGetTitleText(HMULTIBOARD lb) { WAR3MAPCPP_CALL_JASS(MultiboardGetTitleText, lb); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardMinimize(HMULTIBOARD lb, BOOLEAN minimize) { WAR3MAPCPP_CALL_JASS(MultiboardMinimize, lb, minimize); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardReleaseItem(HMULTIBOARDITEM mbi) { WAR3MAPCPP_CALL_JASS(MultiboardReleaseItem, mbi); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardSetColumnCount(HMULTIBOARD lb, int count) { WAR3MAPCPP_CALL_JASS(MultiboardSetColumnCount, lb, count); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardSetItemIcon(HMULTIBOARDITEM mbi, CJassString iconFileName) { WAR3MAPCPP_CALL_JASS(MultiboardSetItemIcon, mbi, iconFileName); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardSetItemStyle(HMULTIBOARDITEM mbi, BOOLEAN showValue, BOOLEAN showIcon) { WAR3MAPCPP_CALL_JASS(MultiboardSetItemStyle, mbi, showValue, showIcon); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardSetItemValue(HMULTIBOARDITEM mbi, CJassString val) { WAR3MAPCPP_CALL_JASS(MultiboardSetItemValue, mbi, val); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardSetItemValueColor(HMULTIBOARDITEM mbi, int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(MultiboardSetItemValueColor, mbi, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardSetItemWidth(HMULTIBOARDITEM mbi, float width) { WAR3MAPCPP_CALL_JASS(MultiboardSetItemWidth, mbi, width); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardSetItemsIcon(HMULTIBOARD lb, CJassString iconPath) { WAR3MAPCPP_CALL_JASS(MultiboardSetItemsIcon, lb, iconPath); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardSetItemsStyle(HMULTIBOARD lb, BOOLEAN showValues, BOOLEAN showIcons) { WAR3MAPCPP_CALL_JASS(MultiboardSetItemsStyle, lb, showValues, showIcons); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardSetItemsValue(HMULTIBOARD lb, CJassString value) { WAR3MAPCPP_CALL_JASS(MultiboardSetItemsValue, lb, value); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardSetItemsValueColor(HMULTIBOARD lb, int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(MultiboardSetItemsValueColor, lb, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardSetItemsWidth(HMULTIBOARD lb, float width) { WAR3MAPCPP_CALL_JASS(MultiboardSetItemsWidth, lb, width); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardSetRowCount(HMULTIBOARD lb, int count) { WAR3MAPCPP_CALL_JASS(MultiboardSetRowCount, lb, count); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardSetTitleText(HMULTIBOARD lb, CJassString label) { WAR3MAPCPP_CALL_JASS(MultiboardSetTitleText, lb, label); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardSetTitleTextColor(HMULTIBOARD lb, int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(MultiboardSetTitleTextColor, lb, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI MultiboardSuppressDisplay(BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(MultiboardSuppressDisplay, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI NewSoundEnvironment(CJassString environmentName) { WAR3MAPCPP_CALL_JASS(NewSoundEnvironment, environmentName); }
    WA3MAPCPP_FORCE_INLINE HBOOLEXPR JASSAPI Not(HBOOLEXPR operand) { WAR3MAPCPP_CALL_JASS(Not, operand); }
    WA3MAPCPP_FORCE_INLINE HBOOLEXPR JASSAPI Or(HBOOLEXPR operandA, HBOOLEXPR operandB) { WAR3MAPCPP_CALL_JASS(Or, operandA, operandB); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI OrderId(CJassString orderIdString) { WAR3MAPCPP_CALL_JASS(OrderId, orderIdString); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI OrderId2String(int orderId) { WAR3MAPCPP_CALL_JASS(OrderId2String, orderId); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PanCameraTo(float x, float y) { WAR3MAPCPP_CALL_JASS(PanCameraTo, x, y); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PanCameraToTimed(float x, float y, float duration) { WAR3MAPCPP_CALL_JASS(PanCameraToTimed, x, y, duration); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PanCameraToTimedWithZ(float x, float y, float zOffsetDest, float duration) { WAR3MAPCPP_CALL_JASS(PanCameraToTimedWithZ, x, y, zOffsetDest, duration); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PanCameraToWithZ(float x, float y, float zOffsetDest) { WAR3MAPCPP_CALL_JASS(PanCameraToWithZ, x, y, zOffsetDest); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PauseCompAI(HPLAYER p, BOOLEAN pause) { WAR3MAPCPP_CALL_JASS(PauseCompAI, p, pause); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PauseGame(BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(PauseGame, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PauseTimer(HTIMER Timer) { WAR3MAPCPP_CALL_JASS(PauseTimer, Timer); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PauseUnit(HUNIT unit, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(PauseUnit, unit, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PingMinimap(float x, float y, float duration) { WAR3MAPCPP_CALL_JASS(PingMinimap, x, y, duration); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PingMinimapEx(float x, float y, float duration, int red, int green, int blue, BOOLEAN extraEffects) { WAR3MAPCPP_CALL_JASS(PingMinimapEx, x, y, duration, red, green, blue, extraEffects); }
    WA3MAPCPP_FORCE_INLINE HITEM JASSAPI PlaceRandomItem(HITEMPOOL ItemPool, float x, float y) { WAR3MAPCPP_CALL_JASS(PlaceRandomItem, ItemPool, x, y); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI PlaceRandomUnit(HUNITPOOL Pool, HPLAYER forWhichPlayer, float x, float y, float facing) { WAR3MAPCPP_CALL_JASS(PlaceRandomUnit, Pool, forWhichPlayer, x, y, facing); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PlayCinematic(CJassString movieName) { WAR3MAPCPP_CALL_JASS(PlayCinematic, movieName); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PlayModelCinematic(CJassString modelName) { WAR3MAPCPP_CALL_JASS(PlayModelCinematic, modelName); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PlayMusic(CJassString musicName) { WAR3MAPCPP_CALL_JASS(PlayMusic, musicName); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PlayMusicEx(CJassString musicName, int frommsecs, int fadeinmsecs) { WAR3MAPCPP_CALL_JASS(PlayMusicEx, musicName, frommsecs, fadeinmsecs); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PlayThematicMusic(CJassString musicFileName) { WAR3MAPCPP_CALL_JASS(PlayThematicMusic, musicFileName); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PlayThematicMusicEx(CJassString musicFileName, int frommsecs) { WAR3MAPCPP_CALL_JASS(PlayThematicMusicEx, musicFileName, frommsecs); }
    WA3MAPCPP_FORCE_INLINE HPLAYER JASSAPI Player(int number) { WAR3MAPCPP_CALL_JASS(Player, number); }
    WA3MAPCPP_FORCE_INLINE HLEADERBOARD JASSAPI PlayerGetLeaderboard(HPLAYER toPlayer) { WAR3MAPCPP_CALL_JASS(PlayerGetLeaderboard, toPlayer); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PlayerSetLeaderboard(HPLAYER toPlayer, HLEADERBOARD lb) { WAR3MAPCPP_CALL_JASS(PlayerSetLeaderboard, toPlayer, lb); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PopLastCommand() { WAR3MAPCPP_CALL_JASS(PopLastCommand); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI Pow(float x, float power) { WAR3MAPCPP_CALL_JASS(Pow, x, power); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI Preload(CJassString filename) { WAR3MAPCPP_CALL_JASS(Preload, filename); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PreloadEnd(float timeout) { WAR3MAPCPP_CALL_JASS(PreloadEnd, timeout); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PreloadEndEx() { WAR3MAPCPP_CALL_JASS(PreloadEndEx); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PreloadGenClear() { WAR3MAPCPP_CALL_JASS(PreloadGenClear); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PreloadGenEnd(CJassString filename) { WAR3MAPCPP_CALL_JASS(PreloadGenEnd, filename); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PreloadGenStart() { WAR3MAPCPP_CALL_JASS(PreloadGenStart); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PreloadRefresh() { WAR3MAPCPP_CALL_JASS(PreloadRefresh); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PreloadStart() { WAR3MAPCPP_CALL_JASS(PreloadStart); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI Preloader(CJassString filename) { WAR3MAPCPP_CALL_JASS(Preloader, filename); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI PurchaseZeppelin() { WAR3MAPCPP_CALL_JASS(PurchaseZeppelin); }
    WA3MAPCPP_FORCE_INLINE HQUESTITEM JASSAPI QuestCreateItem(HQUEST Quest) { WAR3MAPCPP_CALL_JASS(QuestCreateItem, Quest); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI QuestItemSetCompleted(HQUESTITEM QuestItem, BOOLEAN completed) { WAR3MAPCPP_CALL_JASS(QuestItemSetCompleted, QuestItem, completed); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI QuestItemSetDescription(HQUESTITEM QuestItem, CJassString description) { WAR3MAPCPP_CALL_JASS(QuestItemSetDescription, QuestItem, description); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI QuestSetCompleted(HQUEST Quest, BOOLEAN completed) { WAR3MAPCPP_CALL_JASS(QuestSetCompleted, Quest, completed); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI QuestSetDescription(HQUEST Quest, CJassString description) { WAR3MAPCPP_CALL_JASS(QuestSetDescription, Quest, description); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI QuestSetDiscovered(HQUEST Quest, BOOLEAN discovered) { WAR3MAPCPP_CALL_JASS(QuestSetDiscovered, Quest, discovered); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI QuestSetEnabled(HQUEST Quest, BOOLEAN enabled) { WAR3MAPCPP_CALL_JASS(QuestSetEnabled, Quest, enabled); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI QuestSetFailed(HQUEST Quest, BOOLEAN failed) { WAR3MAPCPP_CALL_JASS(QuestSetFailed, Quest, failed); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI QuestSetIconPath(HQUEST Quest, CJassString iconPath) { WAR3MAPCPP_CALL_JASS(QuestSetIconPath, Quest, iconPath); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI QuestSetRequired(HQUEST Quest, BOOLEAN required) { WAR3MAPCPP_CALL_JASS(QuestSetRequired, Quest, required); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI QuestSetTitle(HQUEST Quest, CJassString title) { WAR3MAPCPP_CALL_JASS(QuestSetTitle, Quest, title); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI QueueDestructableAnimation(HDESTRUCTABLE d, CJassString Animation) { WAR3MAPCPP_CALL_JASS(QueueDestructableAnimation, d, Animation); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI QueueUnitAnimation(HUNIT unit, CJassString Animation) { WAR3MAPCPP_CALL_JASS(QueueUnitAnimation, unit, Animation); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI R2I(float r) { WAR3MAPCPP_CALL_JASS(R2I, r); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI R2S(float r) { WAR3MAPCPP_CALL_JASS(R2S, r); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI R2SW(float r, int width, int precision) { WAR3MAPCPP_CALL_JASS(R2SW, r, width, precision); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI Rad2Deg(float radians) { WAR3MAPCPP_CALL_JASS(Rad2Deg, radians); }
    WA3MAPCPP_FORCE_INLINE HRECT JASSAPI Rect(float minx, float miny, float maxx, float maxy) { WAR3MAPCPP_CALL_JASS(Rect, minx, miny, maxx, maxy); }
    WA3MAPCPP_FORCE_INLINE HRECT JASSAPI RectFromLoc(HLOCATION min, HLOCATION max) { WAR3MAPCPP_CALL_JASS(RectFromLoc, min, max); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RecycleGuardPosition(HUNIT hUnit) { WAR3MAPCPP_CALL_JASS(RecycleGuardPosition, hUnit); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RegionAddCell(HREGION Region, float x, float y) { WAR3MAPCPP_CALL_JASS(RegionAddCell, Region, x, y); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RegionAddCellAtLoc(HREGION Region, HLOCATION Location) { WAR3MAPCPP_CALL_JASS(RegionAddCellAtLoc, Region, Location); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RegionAddRect(HREGION Region, HRECT r) { WAR3MAPCPP_CALL_JASS(RegionAddRect, Region, r); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RegionClearCell(HREGION Region, float x, float y) { WAR3MAPCPP_CALL_JASS(RegionClearCell, Region, x, y); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RegionClearCellAtLoc(HREGION Region, HLOCATION Location) { WAR3MAPCPP_CALL_JASS(RegionClearCellAtLoc, Region, Location); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RegionClearRect(HREGION Region, HRECT r) { WAR3MAPCPP_CALL_JASS(RegionClearRect, Region, r); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RegisterStackedSound(HSOUND soundHandle, BOOLEAN byPosition, float rectwidth, float rectheight) { WAR3MAPCPP_CALL_JASS(RegisterStackedSound, soundHandle, byPosition, rectwidth, rectheight); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ReloadGame() { WAR3MAPCPP_CALL_JASS(ReloadGame); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI ReloadGameCachesFromDisk() { WAR3MAPCPP_CALL_JASS(ReloadGameCachesFromDisk); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveAllGuardPositions(HPLAYER num) { WAR3MAPCPP_CALL_JASS(RemoveAllGuardPositions, num); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveDestructable(HDESTRUCTABLE d) { WAR3MAPCPP_CALL_JASS(RemoveDestructable, d); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveGuardPosition(HUNIT hUnit) { WAR3MAPCPP_CALL_JASS(RemoveGuardPosition, hUnit); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveInjuries() { WAR3MAPCPP_CALL_JASS(RemoveInjuries); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveItem(HITEM item) { WAR3MAPCPP_CALL_JASS(RemoveItem, item); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveItemFromAllStock(int itemId) { WAR3MAPCPP_CALL_JASS(RemoveItemFromAllStock, itemId); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveItemFromStock(HUNIT unit, int itemId) { WAR3MAPCPP_CALL_JASS(RemoveItemFromStock, unit, itemId); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveLocation(HLOCATION Location) { WAR3MAPCPP_CALL_JASS(RemoveLocation, Location); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemovePlayer(HPLAYER player, HPLAYERGAMERESULT gameResult) { WAR3MAPCPP_CALL_JASS(RemovePlayer, player, gameResult); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveRect(HRECT Rect) { WAR3MAPCPP_CALL_JASS(RemoveRect, Rect); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveRegion(HREGION Region) { WAR3MAPCPP_CALL_JASS(RemoveRegion, Region); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI RemoveSaveDirectory(CJassString sourceDirName) { WAR3MAPCPP_CALL_JASS(RemoveSaveDirectory, sourceDirName); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveSavedBoolean(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(RemoveSavedBoolean, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveSavedHandle(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(RemoveSavedHandle, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveSavedInteger(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(RemoveSavedInteger, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveSavedReal(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(RemoveSavedReal, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveSavedString(HHASHTABLE table, int parentKey, int childKey) { WAR3MAPCPP_CALL_JASS(RemoveSavedString, table, parentKey, childKey); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveSiege() { WAR3MAPCPP_CALL_JASS(RemoveSiege); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveUnit(HUNIT unit) { WAR3MAPCPP_CALL_JASS(RemoveUnit, unit); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveUnitFromAllStock(int unitId) { WAR3MAPCPP_CALL_JASS(RemoveUnitFromAllStock, unitId); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveUnitFromStock(HUNIT unit, int unitId) { WAR3MAPCPP_CALL_JASS(RemoveUnitFromStock, unit, unitId); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RemoveWeatherEffect(HWEATHEREFFECT Effect) { WAR3MAPCPP_CALL_JASS(RemoveWeatherEffect, Effect); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI RenameSaveDirectory(CJassString sourceDirName, CJassString destDirName) { WAR3MAPCPP_CALL_JASS(RenameSaveDirectory, sourceDirName, destDirName); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ResetCaptainLocs() { WAR3MAPCPP_CALL_JASS(ResetCaptainLocs); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ResetTerrainFog() { WAR3MAPCPP_CALL_JASS(ResetTerrainFog); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ResetToGameCamera(float duration) { WAR3MAPCPP_CALL_JASS(ResetToGameCamera, duration); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ResetTrigger(HTRIGGER Trigger) { WAR3MAPCPP_CALL_JASS(ResetTrigger, Trigger); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ResetUbersplat(HUBERSPLAT Splat) { WAR3MAPCPP_CALL_JASS(ResetUbersplat, Splat); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ResetUnitLookAt(HUNIT unit) { WAR3MAPCPP_CALL_JASS(ResetUnitLookAt, unit); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI RestartGame(BOOLEAN doScoreScreen) { WAR3MAPCPP_CALL_JASS(RestartGame, doScoreScreen); }
    WA3MAPCPP_FORCE_INLINE HUNIT JASSAPI RestoreUnit(HGAMECACHE cache, CJassString missionKey, CJassString key, HPLAYER forWhichPlayer, float x, float y, float facing) { WAR3MAPCPP_CALL_JASS(RestoreUnit, cache, missionKey, key, forWhichPlayer, x, y, facing); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ResumeMusic() { WAR3MAPCPP_CALL_JASS(ResumeMusic); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ResumeTimer(HTIMER Timer) { WAR3MAPCPP_CALL_JASS(ResumeTimer, Timer); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ReturnGuardPosts() { WAR3MAPCPP_CALL_JASS(ReturnGuardPosts); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI ReviveHero(HUNIT hero, float x, float y, BOOLEAN doEyecandy) { WAR3MAPCPP_CALL_JASS(ReviveHero, hero, x, y, doEyecandy); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI ReviveHeroLoc(HUNIT hero, HLOCATION loc, BOOLEAN doEyecandy) { WAR3MAPCPP_CALL_JASS(ReviveHeroLoc, hero, loc, doEyecandy); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI S2I(CJassString s) { WAR3MAPCPP_CALL_JASS(S2I, s); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI S2R(CJassString s) { WAR3MAPCPP_CALL_JASS(S2R, s); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveAbilityHandle(HHASHTABLE table, int parentKey, int childKey, HABILITY Ability) { WAR3MAPCPP_CALL_JASS(SaveAbilityHandle, table, parentKey, childKey, Ability); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveAgentHandle(HHASHTABLE table, int parentKey, int childKey, HAGENT Agent) { WAR3MAPCPP_CALL_JASS(SaveAgentHandle, table, parentKey, childKey, Agent); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SaveBoolean(HHASHTABLE table, int parentKey, int childKey, BOOLEAN value) { WAR3MAPCPP_CALL_JASS(SaveBoolean, table, parentKey, childKey, value); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveBooleanExprHandle(HHASHTABLE table, int parentKey, int childKey, HBOOLEXPR Boolexpr) { WAR3MAPCPP_CALL_JASS(SaveBooleanExprHandle, table, parentKey, childKey, Boolexpr); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveButtonHandle(HHASHTABLE table, int parentKey, int childKey, HBUTTON Button) { WAR3MAPCPP_CALL_JASS(SaveButtonHandle, table, parentKey, childKey, Button); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveDefeatConditionHandle(HHASHTABLE table, int parentKey, int childKey, HDEFEATCONDITION Defeatcondition) { WAR3MAPCPP_CALL_JASS(SaveDefeatConditionHandle, table, parentKey, childKey, Defeatcondition); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveDestructableHandle(HHASHTABLE table, int parentKey, int childKey, HDESTRUCTABLE Destructable) { WAR3MAPCPP_CALL_JASS(SaveDestructableHandle, table, parentKey, childKey, Destructable); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveDialogHandle(HHASHTABLE table, int parentKey, int childKey, HDIALOG Dialog) { WAR3MAPCPP_CALL_JASS(SaveDialogHandle, table, parentKey, childKey, Dialog); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveEffectHandle(HHASHTABLE table, int parentKey, int childKey, HEFFECT Effect) { WAR3MAPCPP_CALL_JASS(SaveEffectHandle, table, parentKey, childKey, Effect); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveFogModifierHandle(HHASHTABLE table, int parentKey, int childKey, HFOGMODIFIER FogModifier) { WAR3MAPCPP_CALL_JASS(SaveFogModifierHandle, table, parentKey, childKey, FogModifier); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveFogStateHandle(HHASHTABLE table, int parentKey, int childKey, HFOGSTATE FogState) { WAR3MAPCPP_CALL_JASS(SaveFogStateHandle, table, parentKey, childKey, FogState); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveForceHandle(HHASHTABLE table, int parentKey, int childKey, HFORCE Force) { WAR3MAPCPP_CALL_JASS(SaveForceHandle, table, parentKey, childKey, Force); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SaveGame(CJassString saveFileName) { WAR3MAPCPP_CALL_JASS(SaveGame, saveFileName); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveGameCache(HGAMECACHE Cache) { WAR3MAPCPP_CALL_JASS(SaveGameCache, Cache); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveGameExists(CJassString saveName) { WAR3MAPCPP_CALL_JASS(SaveGameExists, saveName); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveGroupHandle(HHASHTABLE table, int parentKey, int childKey, HGROUP Group) { WAR3MAPCPP_CALL_JASS(SaveGroupHandle, table, parentKey, childKey, Group); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveHashtableHandle(HHASHTABLE table, int parentKey, int childKey, HHASHTABLE Hashtable) { WAR3MAPCPP_CALL_JASS(SaveHashtableHandle, table, parentKey, childKey, Hashtable); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveImageHandle(HHASHTABLE table, int parentKey, int childKey, HIMAGE Image) { WAR3MAPCPP_CALL_JASS(SaveImageHandle, table, parentKey, childKey, Image); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SaveInteger(HHASHTABLE table, int parentKey, int childKey, int value) { WAR3MAPCPP_CALL_JASS(SaveInteger, table, parentKey, childKey, value); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveItemHandle(HHASHTABLE table, int parentKey, int childKey, HITEM item) { WAR3MAPCPP_CALL_JASS(SaveItemHandle, table, parentKey, childKey, item); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveItemPoolHandle(HHASHTABLE table, int parentKey, int childKey, HITEMPOOL Itempool) { WAR3MAPCPP_CALL_JASS(SaveItemPoolHandle, table, parentKey, childKey, Itempool); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveLeaderboardHandle(HHASHTABLE table, int parentKey, int childKey, HLEADERBOARD Leaderboard) { WAR3MAPCPP_CALL_JASS(SaveLeaderboardHandle, table, parentKey, childKey, Leaderboard); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveLightningHandle(HHASHTABLE table, int parentKey, int childKey, HLIGHTNING Lightning) { WAR3MAPCPP_CALL_JASS(SaveLightningHandle, table, parentKey, childKey, Lightning); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveLocationHandle(HHASHTABLE table, int parentKey, int childKey, HLOCATION Location) { WAR3MAPCPP_CALL_JASS(SaveLocationHandle, table, parentKey, childKey, Location); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveMultiboardHandle(HHASHTABLE table, int parentKey, int childKey, HMULTIBOARD Multiboard) { WAR3MAPCPP_CALL_JASS(SaveMultiboardHandle, table, parentKey, childKey, Multiboard); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveMultiboardItemHandle(HHASHTABLE table, int parentKey, int childKey, HMULTIBOARDITEM Multiboarditem) { WAR3MAPCPP_CALL_JASS(SaveMultiboardItemHandle, table, parentKey, childKey, Multiboarditem); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SavePlayerHandle(HHASHTABLE table, int parentKey, int childKey, HPLAYER player) { WAR3MAPCPP_CALL_JASS(SavePlayerHandle, table, parentKey, childKey, player); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveQuestHandle(HHASHTABLE table, int parentKey, int childKey, HQUEST Quest) { WAR3MAPCPP_CALL_JASS(SaveQuestHandle, table, parentKey, childKey, Quest); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveQuestItemHandle(HHASHTABLE table, int parentKey, int childKey, HQUESTITEM Questitem) { WAR3MAPCPP_CALL_JASS(SaveQuestItemHandle, table, parentKey, childKey, Questitem); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SaveReal(HHASHTABLE table, int parentKey, int childKey, float value) { WAR3MAPCPP_CALL_JASS(SaveReal, table, parentKey, childKey, value); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveRectHandle(HHASHTABLE table, int parentKey, int childKey, HRECT Rect) { WAR3MAPCPP_CALL_JASS(SaveRectHandle, table, parentKey, childKey, Rect); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveRegionHandle(HHASHTABLE table, int parentKey, int childKey, HREGION Region) { WAR3MAPCPP_CALL_JASS(SaveRegionHandle, table, parentKey, childKey, Region); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveSoundHandle(HHASHTABLE table, int parentKey, int childKey, HSOUND Sound) { WAR3MAPCPP_CALL_JASS(SaveSoundHandle, table, parentKey, childKey, Sound); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveStr(HHASHTABLE table, int parentKey, int childKey, CJassString value) { WAR3MAPCPP_CALL_JASS(SaveStr, table, parentKey, childKey, value); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveTextTagHandle(HHASHTABLE table, int parentKey, int childKey, HTEXTTAG Texttag) { WAR3MAPCPP_CALL_JASS(SaveTextTagHandle, table, parentKey, childKey, Texttag); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveTimerDialogHandle(HHASHTABLE table, int parentKey, int childKey, HTIMERDIALOG Timerdialog) { WAR3MAPCPP_CALL_JASS(SaveTimerDialogHandle, table, parentKey, childKey, Timerdialog); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveTimerHandle(HHASHTABLE table, int parentKey, int childKey, HTIMER Timer) { WAR3MAPCPP_CALL_JASS(SaveTimerHandle, table, parentKey, childKey, Timer); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveTrackableHandle(HHASHTABLE table, int parentKey, int childKey, HTRACKABLE Trackable) { WAR3MAPCPP_CALL_JASS(SaveTrackableHandle, table, parentKey, childKey, Trackable); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveTriggerActionHandle(HHASHTABLE table, int parentKey, int childKey, HTRIGGERACTION Triggeraction) { WAR3MAPCPP_CALL_JASS(SaveTriggerActionHandle, table, parentKey, childKey, Triggeraction); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveTriggerConditionHandle(HHASHTABLE table, int parentKey, int childKey, HTRIGGERCONDITION Triggercondition) { WAR3MAPCPP_CALL_JASS(SaveTriggerConditionHandle, table, parentKey, childKey, Triggercondition); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveTriggerEventHandle(HHASHTABLE table, int parentKey, int childKey, HEVENT Event) { WAR3MAPCPP_CALL_JASS(SaveTriggerEventHandle, table, parentKey, childKey, Event); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveTriggerHandle(HHASHTABLE table, int parentKey, int childKey, HTRIGGER Trigger) { WAR3MAPCPP_CALL_JASS(SaveTriggerHandle, table, parentKey, childKey, Trigger); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveUbersplatHandle(HHASHTABLE table, int parentKey, int childKey, HUBERSPLAT Ubersplat) { WAR3MAPCPP_CALL_JASS(SaveUbersplatHandle, table, parentKey, childKey, Ubersplat); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveUnitHandle(HHASHTABLE table, int parentKey, int childKey, HUNIT unit) { WAR3MAPCPP_CALL_JASS(SaveUnitHandle, table, parentKey, childKey, unit); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveUnitPoolHandle(HHASHTABLE table, int parentKey, int childKey, HUNITPOOL Unitpool) { WAR3MAPCPP_CALL_JASS(SaveUnitPoolHandle, table, parentKey, childKey, Unitpool); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SaveWidgetHandle(HHASHTABLE table, int parentKey, int childKey, HWIDGET widget) { WAR3MAPCPP_CALL_JASS(SaveWidgetHandle, table, parentKey, childKey, widget); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SelectHeroSkill(HUNIT hero, int abilcode) { WAR3MAPCPP_CALL_JASS(SelectHeroSkill, hero, abilcode); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SelectUnit(HUNIT unit, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SelectUnit, unit, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetAllItemTypeSlots(int slots) { WAR3MAPCPP_CALL_JASS(SetAllItemTypeSlots, slots); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetAllUnitTypeSlots(int slots) { WAR3MAPCPP_CALL_JASS(SetAllUnitTypeSlots, slots); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetAllianceTarget(HUNIT arg1) { WAR3MAPCPP_CALL_JASS(SetAllianceTarget, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetAllyColorFilterState(int state) { WAR3MAPCPP_CALL_JASS(SetAllyColorFilterState, state); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetAltMinimapIcon(CJassString iconPath) { WAR3MAPCPP_CALL_JASS(SetAltMinimapIcon, iconPath); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetAmphibious() { WAR3MAPCPP_CALL_JASS(SetAmphibious); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetBlight(HPLAYER player, float x, float y, float radius, BOOLEAN addBlight) { WAR3MAPCPP_CALL_JASS(SetBlight, player, x, y, radius, addBlight); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetBlightLoc(HPLAYER player, HLOCATION Location, float radius, BOOLEAN addBlight) { WAR3MAPCPP_CALL_JASS(SetBlightLoc, player, Location, radius, addBlight); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetBlightPoint(HPLAYER player, float x, float y, BOOLEAN addBlight) { WAR3MAPCPP_CALL_JASS(SetBlightPoint, player, x, y, addBlight); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetBlightRect(HPLAYER player, HRECT r, BOOLEAN addBlight) { WAR3MAPCPP_CALL_JASS(SetBlightRect, player, r, addBlight); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCameraBounds(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4) { WAR3MAPCPP_CALL_JASS(SetCameraBounds, x1, y1, x2, y2, x3, y3, x4, y4); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCameraField(HCAMERAFIELD field, float value, float duration) { WAR3MAPCPP_CALL_JASS(SetCameraField, field, value, duration); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCameraOrientController(HUNIT unit, float xoffset, float yoffset) { WAR3MAPCPP_CALL_JASS(SetCameraOrientController, unit, xoffset, yoffset); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCameraPosition(float x, float y) { WAR3MAPCPP_CALL_JASS(SetCameraPosition, x, y); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCameraQuickPosition(float x, float y) { WAR3MAPCPP_CALL_JASS(SetCameraQuickPosition, x, y); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCameraRotateMode(float x, float y, float radiansToSweep, float duration) { WAR3MAPCPP_CALL_JASS(SetCameraRotateMode, x, y, radiansToSweep, duration); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCameraTargetController(HUNIT unit, float xoffset, float yoffset, BOOLEAN inheritOrientation) { WAR3MAPCPP_CALL_JASS(SetCameraTargetController, unit, xoffset, yoffset, inheritOrientation); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCampaignAI() { WAR3MAPCPP_CALL_JASS(SetCampaignAI); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCampaignAvailable(int campaignNumber, BOOLEAN available) { WAR3MAPCPP_CALL_JASS(SetCampaignAvailable, campaignNumber, available); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCampaignMenuRace(HRACE r) { WAR3MAPCPP_CALL_JASS(SetCampaignMenuRace, r); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCampaignMenuRaceEx(int campaignIndex) { WAR3MAPCPP_CALL_JASS(SetCampaignMenuRaceEx, campaignIndex); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCaptainChanges(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(SetCaptainChanges, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCaptainHome(int arg1, float arg2, float arg3) { WAR3MAPCPP_CALL_JASS(SetCaptainHome, arg1, arg2, arg3); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCineFilterBlendMode(HBLENDMODE Mode) { WAR3MAPCPP_CALL_JASS(SetCineFilterBlendMode, Mode); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCineFilterDuration(float duration) { WAR3MAPCPP_CALL_JASS(SetCineFilterDuration, duration); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCineFilterEndColor(int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(SetCineFilterEndColor, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCineFilterEndUV(float minu, float minv, float maxu, float maxv) { WAR3MAPCPP_CALL_JASS(SetCineFilterEndUV, minu, minv, maxu, maxv); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCineFilterStartColor(int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(SetCineFilterStartColor, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCineFilterStartUV(float minu, float minv, float maxu, float maxv) { WAR3MAPCPP_CALL_JASS(SetCineFilterStartUV, minu, minv, maxu, maxv); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCineFilterTexMapFlags(HTEXMAPFLAGS Flags) { WAR3MAPCPP_CALL_JASS(SetCineFilterTexMapFlags, Flags); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCineFilterTexture(CJassString filename) { WAR3MAPCPP_CALL_JASS(SetCineFilterTexture, filename); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCinematicCamera(CJassString cameraModelFile) { WAR3MAPCPP_CALL_JASS(SetCinematicCamera, cameraModelFile); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCinematicScene(int portraitUnitId, HPLAYERCOLOR color, CJassString speakerTitle, CJassString text, float sceneDuration, float voiceoverDuration) { WAR3MAPCPP_CALL_JASS(SetCinematicScene, portraitUnitId, color, speakerTitle, text, sceneDuration, voiceoverDuration); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCreatureDensity(HMAPDENSITY density) { WAR3MAPCPP_CALL_JASS(SetCreatureDensity, density); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCreepCampFilterState(BOOLEAN state) { WAR3MAPCPP_CALL_JASS(SetCreepCampFilterState, state); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetCustomCampaignButtonVisible(int Button, BOOLEAN visible) { WAR3MAPCPP_CALL_JASS(SetCustomCampaignButtonVisible, Button, visible); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetDayNightModels(CJassString terrainDNCFile, CJassString unitDNCFile) { WAR3MAPCPP_CALL_JASS(SetDayNightModels, terrainDNCFile, unitDNCFile); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetDefaultDifficulty(HGAMEDIFFICULTY g) { WAR3MAPCPP_CALL_JASS(SetDefaultDifficulty, g); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetDefendPlayer(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(SetDefendPlayer, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetDestructableAnimation(HDESTRUCTABLE d, CJassString Animation) { WAR3MAPCPP_CALL_JASS(SetDestructableAnimation, d, Animation); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetDestructableAnimationSpeed(HDESTRUCTABLE d, float speedFactor) { WAR3MAPCPP_CALL_JASS(SetDestructableAnimationSpeed, d, speedFactor); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetDestructableInvulnerable(HDESTRUCTABLE d, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetDestructableInvulnerable, d, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetDestructableLife(HDESTRUCTABLE d, float life) { WAR3MAPCPP_CALL_JASS(SetDestructableLife, d, life); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetDestructableMaxLife(HDESTRUCTABLE d, float max) { WAR3MAPCPP_CALL_JASS(SetDestructableMaxLife, d, max); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetDestructableOccluderHeight(HDESTRUCTABLE d, float height) { WAR3MAPCPP_CALL_JASS(SetDestructableOccluderHeight, d, height); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetDoodadAnimation(float x, float y, float radius, int doodadID, BOOLEAN nearestOnly, CJassString animName, BOOLEAN animRandom) { WAR3MAPCPP_CALL_JASS(SetDoodadAnimation, x, y, radius, doodadID, nearestOnly, animName, animRandom); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetDoodadAnimationRect(HRECT r, int doodadID, CJassString animName, BOOLEAN animRandom) { WAR3MAPCPP_CALL_JASS(SetDoodadAnimationRect, r, doodadID, animName, animRandom); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetEdCinematicAvailable(int campaignNumber, BOOLEAN available) { WAR3MAPCPP_CALL_JASS(SetEdCinematicAvailable, campaignNumber, available); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SetExpansion(HUNIT arg1, int arg2) { WAR3MAPCPP_CALL_JASS(SetExpansion, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetFloatGameState(HFGAMESTATE FloatGameState, float value) { WAR3MAPCPP_CALL_JASS(SetFloatGameState, FloatGameState, value); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetFogStateRadius(HPLAYER forWhichPlayer, HFOGSTATE State, float centerx, float centerY, float radius, BOOLEAN useSharedVision) { WAR3MAPCPP_CALL_JASS(SetFogStateRadius, forWhichPlayer, State, centerx, centerY, radius, useSharedVision); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetFogStateRadiusLoc(HPLAYER forWhichPlayer, HFOGSTATE State, HLOCATION center, float radius, BOOLEAN useSharedVision) { WAR3MAPCPP_CALL_JASS(SetFogStateRadiusLoc, forWhichPlayer, State, center, radius, useSharedVision); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetFogStateRect(HPLAYER forWhichPlayer, HFOGSTATE State, HRECT where, BOOLEAN useSharedVision) { WAR3MAPCPP_CALL_JASS(SetFogStateRect, forWhichPlayer, State, where, useSharedVision); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetGameDifficulty(HGAMEDIFFICULTY difficulty) { WAR3MAPCPP_CALL_JASS(SetGameDifficulty, difficulty); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetGamePlacement(HPLACEMENT PlacementType) { WAR3MAPCPP_CALL_JASS(SetGamePlacement, PlacementType); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetGameSpeed(HGAMESPEED speed) { WAR3MAPCPP_CALL_JASS(SetGameSpeed, speed); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetGameTypeSupported(HGAMETYPE GameType, BOOLEAN value) { WAR3MAPCPP_CALL_JASS(SetGameTypeSupported, GameType, value); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetGroupsFlee(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(SetGroupsFlee, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetHeroAgi(HUNIT hero, int newAgi, BOOLEAN permanent) { WAR3MAPCPP_CALL_JASS(SetHeroAgi, hero, newAgi, permanent); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetHeroInt(HUNIT hero, int newInt, BOOLEAN permanent) { WAR3MAPCPP_CALL_JASS(SetHeroInt, hero, newInt, permanent); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetHeroLevel(HUNIT hero, int level, BOOLEAN showEyeCandy) { WAR3MAPCPP_CALL_JASS(SetHeroLevel, hero, level, showEyeCandy); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetHeroLevels(JCALLBACK arg1) { WAR3MAPCPP_CALL_JASS(SetHeroLevels, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetHeroStr(HUNIT hero, int newStr, BOOLEAN permanent) { WAR3MAPCPP_CALL_JASS(SetHeroStr, hero, newStr, permanent); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetHeroXP(HUNIT hero, int newXpVal, BOOLEAN showEyeCandy) { WAR3MAPCPP_CALL_JASS(SetHeroXP, hero, newXpVal, showEyeCandy); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetHeroesBuyItems(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(SetHeroesBuyItems, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetHeroesFlee(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(SetHeroesFlee, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetHeroesTakeItems(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(SetHeroesTakeItems, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetIgnoreInjured(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(SetIgnoreInjured, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetImageAboveWater(HIMAGE Image, BOOLEAN flag, BOOLEAN useWaterAlpha) { WAR3MAPCPP_CALL_JASS(SetImageAboveWater, Image, flag, useWaterAlpha); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetImageColor(HIMAGE Image, int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(SetImageColor, Image, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetImageConstantHeight(HIMAGE Image, BOOLEAN flag, float height) { WAR3MAPCPP_CALL_JASS(SetImageConstantHeight, Image, flag, height); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetImagePosition(HIMAGE Image, float x, float y, float z) { WAR3MAPCPP_CALL_JASS(SetImagePosition, Image, x, y, z); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetImageRender(HIMAGE Image, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetImageRender, Image, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetImageRenderAlways(HIMAGE Image, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetImageRenderAlways, Image, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetImageType(HIMAGE Image, int imageType) { WAR3MAPCPP_CALL_JASS(SetImageType, Image, imageType); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetIntegerGameState(HIGAMESTATE IntegerGameState, int value) { WAR3MAPCPP_CALL_JASS(SetIntegerGameState, IntegerGameState, value); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetIntroShotModel(CJassString introModelPath) { WAR3MAPCPP_CALL_JASS(SetIntroShotModel, introModelPath); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetIntroShotText(CJassString introText) { WAR3MAPCPP_CALL_JASS(SetIntroShotText, introText); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetItemCharges(HITEM item, int charges) { WAR3MAPCPP_CALL_JASS(SetItemCharges, item, charges); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetItemDropID(HITEM item, int unitId) { WAR3MAPCPP_CALL_JASS(SetItemDropID, item, unitId); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetItemDropOnDeath(HITEM item, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetItemDropOnDeath, item, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetItemDroppable(HITEM i, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetItemDroppable, i, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetItemInvulnerable(HITEM item, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetItemInvulnerable, item, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetItemPawnable(HITEM i, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetItemPawnable, i, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetItemPlayer(HITEM item, HPLAYER player, BOOLEAN changeColor) { WAR3MAPCPP_CALL_JASS(SetItemPlayer, item, player, changeColor); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetItemPosition(HITEM i, float x, float y) { WAR3MAPCPP_CALL_JASS(SetItemPosition, i, x, y); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetItemTypeSlots(HUNIT unit, int slots) { WAR3MAPCPP_CALL_JASS(SetItemTypeSlots, unit, slots); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetItemUserData(HITEM item, int data) { WAR3MAPCPP_CALL_JASS(SetItemUserData, item, data); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetItemVisible(HITEM item, BOOLEAN show) { WAR3MAPCPP_CALL_JASS(SetItemVisible, item, show); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SetLightningColor(HLIGHTNING Bolt, float r, float g, float b, float a) { WAR3MAPCPP_CALL_JASS(SetLightningColor, Bolt, r, g, b, a); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetMapDescription(CJassString description) { WAR3MAPCPP_CALL_JASS(SetMapDescription, description); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetMapFlag(HMAPFLAG MapFlag, BOOLEAN value) { WAR3MAPCPP_CALL_JASS(SetMapFlag, MapFlag, value); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetMapMusic(CJassString musicName, BOOLEAN random, int index) { WAR3MAPCPP_CALL_JASS(SetMapMusic, musicName, random, index); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetMapName(CJassString name) { WAR3MAPCPP_CALL_JASS(SetMapName, name); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetMeleeAI() { WAR3MAPCPP_CALL_JASS(SetMeleeAI); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetMissionAvailable(int campaignNumber, int missionNumber, BOOLEAN available) { WAR3MAPCPP_CALL_JASS(SetMissionAvailable, campaignNumber, missionNumber, available); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetMusicPlayPosition(int millisecs) { WAR3MAPCPP_CALL_JASS(SetMusicPlayPosition, millisecs); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetMusicVolume(int volume) { WAR3MAPCPP_CALL_JASS(SetMusicVolume, volume); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetNewHeroes(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(SetNewHeroes, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetOpCinematicAvailable(int campaignNumber, BOOLEAN available) { WAR3MAPCPP_CALL_JASS(SetOpCinematicAvailable, campaignNumber, available); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPeonsRepair(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(SetPeonsRepair, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerAbilityAvailable(HPLAYER player, int abilid, BOOLEAN avail) { WAR3MAPCPP_CALL_JASS(SetPlayerAbilityAvailable, player, abilid, avail); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerAlliance(HPLAYER sourcePlayer, HPLAYER otherPlayer, HALLIANCETYPE AllianceSetting, BOOLEAN value) { WAR3MAPCPP_CALL_JASS(SetPlayerAlliance, sourcePlayer, otherPlayer, AllianceSetting, value); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerColor(HPLAYER player, HPLAYERCOLOR color) { WAR3MAPCPP_CALL_JASS(SetPlayerColor, player, color); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerController(HPLAYER player, HMAPCONTROL controlType) { WAR3MAPCPP_CALL_JASS(SetPlayerController, player, controlType); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerHandicap(HPLAYER player, float handicap) { WAR3MAPCPP_CALL_JASS(SetPlayerHandicap, player, handicap); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerHandicapXP(HPLAYER player, float handicap) { WAR3MAPCPP_CALL_JASS(SetPlayerHandicapXP, player, handicap); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerName(HPLAYER player, CJassString name) { WAR3MAPCPP_CALL_JASS(SetPlayerName, player, name); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerOnScoreScreen(HPLAYER player, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetPlayerOnScoreScreen, player, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerRacePreference(HPLAYER player, HRACEPREFERENCE RacePreference) { WAR3MAPCPP_CALL_JASS(SetPlayerRacePreference, player, RacePreference); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerRaceSelectable(HPLAYER player, BOOLEAN value) { WAR3MAPCPP_CALL_JASS(SetPlayerRaceSelectable, player, value); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerStartLocation(HPLAYER player, int startLocIndex) { WAR3MAPCPP_CALL_JASS(SetPlayerStartLocation, player, startLocIndex); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerState(HPLAYER player, HPLAYERSTATE PlayerState, int value) { WAR3MAPCPP_CALL_JASS(SetPlayerState, player, PlayerState, value); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerTaxRate(HPLAYER sourcePlayer, HPLAYER otherPlayer, HPLAYERSTATE Resource, int rate) { WAR3MAPCPP_CALL_JASS(SetPlayerTaxRate, sourcePlayer, otherPlayer, Resource, rate); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerTeam(HPLAYER player, int Team) { WAR3MAPCPP_CALL_JASS(SetPlayerTeam, player, Team); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerTechMaxAllowed(HPLAYER player, int techid, int maximum) { WAR3MAPCPP_CALL_JASS(SetPlayerTechMaxAllowed, player, techid, maximum); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerTechResearched(HPLAYER player, int techid, int setToLevel) { WAR3MAPCPP_CALL_JASS(SetPlayerTechResearched, player, techid, setToLevel); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayerUnitsOwner(HPLAYER player, int newOwner) { WAR3MAPCPP_CALL_JASS(SetPlayerUnitsOwner, player, newOwner); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetPlayers(int playercount) { WAR3MAPCPP_CALL_JASS(SetPlayers, playercount); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SetProduce(int arg1, int arg2, int arg3) { WAR3MAPCPP_CALL_JASS(SetProduce, arg1, arg2, arg3); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetRandomPaths(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(SetRandomPaths, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetRandomSeed(int seed) { WAR3MAPCPP_CALL_JASS(SetRandomSeed, seed); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetRect(HRECT Rect, float minx, float miny, float maxx, float maxy) { WAR3MAPCPP_CALL_JASS(SetRect, Rect, minx, miny, maxx, maxy); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetRectFromLoc(HRECT Rect, HLOCATION min, HLOCATION max) { WAR3MAPCPP_CALL_JASS(SetRectFromLoc, Rect, min, max); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetReplacementCount(int arg1) { WAR3MAPCPP_CALL_JASS(SetReplacementCount, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetReservedLocalHeroButtons(int reserved) { WAR3MAPCPP_CALL_JASS(SetReservedLocalHeroButtons, reserved); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetResourceAmount(HUNIT unit, int amount) { WAR3MAPCPP_CALL_JASS(SetResourceAmount, unit, amount); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetResourceDensity(HMAPDENSITY density) { WAR3MAPCPP_CALL_JASS(SetResourceDensity, density); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetSkyModel(CJassString skyModelFile) { WAR3MAPCPP_CALL_JASS(SetSkyModel, skyModelFile); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetSlowChopping(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(SetSlowChopping, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetSmartArtillery(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(SetSmartArtillery, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetSoundChannel(HSOUND soundHandle, int channel) { WAR3MAPCPP_CALL_JASS(SetSoundChannel, soundHandle, channel); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetSoundConeAngles(HSOUND soundHandle, float inside, float outside, int outsideVolume) { WAR3MAPCPP_CALL_JASS(SetSoundConeAngles, soundHandle, inside, outside, outsideVolume); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetSoundConeOrientation(HSOUND soundHandle, float x, float y, float z) { WAR3MAPCPP_CALL_JASS(SetSoundConeOrientation, soundHandle, x, y, z); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetSoundDistanceCutoff(HSOUND soundHandle, float cutoff) { WAR3MAPCPP_CALL_JASS(SetSoundDistanceCutoff, soundHandle, cutoff); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetSoundDistances(HSOUND soundHandle, float minDist, float maxDist) { WAR3MAPCPP_CALL_JASS(SetSoundDistances, soundHandle, minDist, maxDist); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetSoundDuration(HSOUND soundHandle, int duration) { WAR3MAPCPP_CALL_JASS(SetSoundDuration, soundHandle, duration); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetSoundParamsFromLabel(HSOUND soundHandle, CJassString soundLabel) { WAR3MAPCPP_CALL_JASS(SetSoundParamsFromLabel, soundHandle, soundLabel); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetSoundPitch(HSOUND soundHandle, float pitch) { WAR3MAPCPP_CALL_JASS(SetSoundPitch, soundHandle, pitch); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetSoundPlayPosition(HSOUND soundHandle, int millisecs) { WAR3MAPCPP_CALL_JASS(SetSoundPlayPosition, soundHandle, millisecs); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetSoundPosition(HSOUND soundHandle, float x, float y, float z) { WAR3MAPCPP_CALL_JASS(SetSoundPosition, soundHandle, x, y, z); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetSoundVelocity(HSOUND soundHandle, float x, float y, float z) { WAR3MAPCPP_CALL_JASS(SetSoundVelocity, soundHandle, x, y, z); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetSoundVolume(HSOUND soundHandle, int volume) { WAR3MAPCPP_CALL_JASS(SetSoundVolume, soundHandle, volume); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetStackedSound(CJassString arg1, float arg2, float arg3) { WAR3MAPCPP_CALL_JASS(SetStackedSound, arg1, arg2, arg3); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetStackedSoundRect(CJassString arg1, HRECT arg2) { WAR3MAPCPP_CALL_JASS(SetStackedSoundRect, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetStagePoint(float arg1, float arg2) { WAR3MAPCPP_CALL_JASS(SetStagePoint, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetStartLocPrio(int StartLoc, int prioSlotIndex, int otherStartLocIndex, HSTARTLOCPRIO priority) { WAR3MAPCPP_CALL_JASS(SetStartLocPrio, StartLoc, prioSlotIndex, otherStartLocIndex, priority); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetStartLocPrioCount(int StartLoc, int prioSlotCount) { WAR3MAPCPP_CALL_JASS(SetStartLocPrioCount, StartLoc, prioSlotCount); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTargetHeroes(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(SetTargetHeroes, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTeams(int teamcount) { WAR3MAPCPP_CALL_JASS(SetTeams, teamcount); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTerrainFog(float a, float b, float c, float d, float e) { WAR3MAPCPP_CALL_JASS(SetTerrainFog, a, b, c, d, e); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTerrainFogEx(int style, float zstart, float zend, float density, float red, float green, float blue) { WAR3MAPCPP_CALL_JASS(SetTerrainFogEx, style, zstart, zend, density, red, green, blue); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTerrainPathable(float x, float y, HPATHINGTYPE t, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetTerrainPathable, x, y, t, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTerrainType(float x, float y, int terrainType, int variation, int area, int shape) { WAR3MAPCPP_CALL_JASS(SetTerrainType, x, y, terrainType, variation, area, shape); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTextTagAge(HTEXTTAG t, float age) { WAR3MAPCPP_CALL_JASS(SetTextTagAge, t, age); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTextTagColor(HTEXTTAG t, int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(SetTextTagColor, t, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTextTagFadepoint(HTEXTTAG t, float fadepoint) { WAR3MAPCPP_CALL_JASS(SetTextTagFadepoint, t, fadepoint); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTextTagLifespan(HTEXTTAG t, float lifespan) { WAR3MAPCPP_CALL_JASS(SetTextTagLifespan, t, lifespan); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTextTagPermanent(HTEXTTAG t, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetTextTagPermanent, t, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTextTagPos(HTEXTTAG t, float x, float y, float heightOffset) { WAR3MAPCPP_CALL_JASS(SetTextTagPos, t, x, y, heightOffset); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTextTagPosUnit(HTEXTTAG t, HUNIT unit, float heightOffset) { WAR3MAPCPP_CALL_JASS(SetTextTagPosUnit, t, unit, heightOffset); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTextTagSuspended(HTEXTTAG t, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetTextTagSuspended, t, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTextTagText(HTEXTTAG t, CJassString s, float height) { WAR3MAPCPP_CALL_JASS(SetTextTagText, t, s, height); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTextTagVelocity(HTEXTTAG t, float xvel, float yvel) { WAR3MAPCPP_CALL_JASS(SetTextTagVelocity, t, xvel, yvel); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTextTagVisibility(HTEXTTAG t, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetTextTagVisibility, t, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetThematicMusicPlayPosition(int millisecs) { WAR3MAPCPP_CALL_JASS(SetThematicMusicPlayPosition, millisecs); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTimeOfDayScale(float r) { WAR3MAPCPP_CALL_JASS(SetTimeOfDayScale, r); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetTutorialCleared(BOOLEAN cleared) { WAR3MAPCPP_CALL_JASS(SetTutorialCleared, cleared); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUbersplatRender(HUBERSPLAT Splat, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetUbersplatRender, Splat, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUbersplatRenderAlways(HUBERSPLAT Splat, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetUbersplatRenderAlways, Splat, flag); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI SetUnitAbilityLevel(HUNIT unit, int abilcode, int level) { WAR3MAPCPP_CALL_JASS(SetUnitAbilityLevel, unit, abilcode, level); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitAcquireRange(HUNIT unit, float newAcquireRange) { WAR3MAPCPP_CALL_JASS(SetUnitAcquireRange, unit, newAcquireRange); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitAnimation(HUNIT unit, CJassString Animation) { WAR3MAPCPP_CALL_JASS(SetUnitAnimation, unit, Animation); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitAnimationByIndex(HUNIT unit, int Animation) { WAR3MAPCPP_CALL_JASS(SetUnitAnimationByIndex, unit, Animation); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitAnimationWithRarity(HUNIT unit, CJassString Animation, HRARITYCONTROL rarity) { WAR3MAPCPP_CALL_JASS(SetUnitAnimationWithRarity, unit, Animation, rarity); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitBlendTime(HUNIT unit, float blendTime) { WAR3MAPCPP_CALL_JASS(SetUnitBlendTime, unit, blendTime); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitColor(HUNIT unit, HPLAYERCOLOR Color) { WAR3MAPCPP_CALL_JASS(SetUnitColor, unit, Color); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitCreepGuard(HUNIT unit, BOOLEAN creepGuard) { WAR3MAPCPP_CALL_JASS(SetUnitCreepGuard, unit, creepGuard); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitExploded(HUNIT unit, BOOLEAN exploded) { WAR3MAPCPP_CALL_JASS(SetUnitExploded, unit, exploded); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitFacing(HUNIT unit, float facingAngle) { WAR3MAPCPP_CALL_JASS(SetUnitFacing, unit, facingAngle); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitFacingTimed(HUNIT unit, float facingAngle, float duration) { WAR3MAPCPP_CALL_JASS(SetUnitFacingTimed, unit, facingAngle, duration); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitFlyHeight(HUNIT unit, float newHeight, float rate) { WAR3MAPCPP_CALL_JASS(SetUnitFlyHeight, unit, newHeight, rate); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitFog(float a, float b, float c, float d, float e) { WAR3MAPCPP_CALL_JASS(SetUnitFog, a, b, c, d, e); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitInvulnerable(HUNIT unit, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetUnitInvulnerable, unit, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitLookAt(HUNIT unit, CJassString Bone, HUNIT lookAtTarget, float offsetX, float offsetY, float offsetZ) { WAR3MAPCPP_CALL_JASS(SetUnitLookAt, unit, Bone, lookAtTarget, offsetX, offsetY, offsetZ); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitMoveSpeed(HUNIT unit, float newSpeed) { WAR3MAPCPP_CALL_JASS(SetUnitMoveSpeed, unit, newSpeed); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitOwner(HUNIT unit, HPLAYER player, BOOLEAN changeColor) { WAR3MAPCPP_CALL_JASS(SetUnitOwner, unit, player, changeColor); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitPathing(HUNIT unit, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetUnitPathing, unit, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitPosition(HUNIT unit, float newX, float newY) { WAR3MAPCPP_CALL_JASS(SetUnitPosition, unit, newX, newY); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitPositionLoc(HUNIT unit, HLOCATION Location) { WAR3MAPCPP_CALL_JASS(SetUnitPositionLoc, unit, Location); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitPropWindow(HUNIT unit, float newPropWindowAngle) { WAR3MAPCPP_CALL_JASS(SetUnitPropWindow, unit, newPropWindowAngle); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitRescuable(HUNIT unit, HPLAYER byWhichPlayer, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SetUnitRescuable, unit, byWhichPlayer, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitRescueRange(HUNIT unit, float range) { WAR3MAPCPP_CALL_JASS(SetUnitRescueRange, unit, range); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitScale(HUNIT unit, float scaleX, float scaleY, float scaleZ) { WAR3MAPCPP_CALL_JASS(SetUnitScale, unit, scaleX, scaleY, scaleZ); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitState(HUNIT unit, HUNITSTATE UnitState, float newVal) { WAR3MAPCPP_CALL_JASS(SetUnitState, unit, UnitState, newVal); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitTimeScale(HUNIT unit, float timeScale) { WAR3MAPCPP_CALL_JASS(SetUnitTimeScale, unit, timeScale); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitTurnSpeed(HUNIT unit, float newTurnSpeed) { WAR3MAPCPP_CALL_JASS(SetUnitTurnSpeed, unit, newTurnSpeed); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitTypeSlots(HUNIT unit, int slots) { WAR3MAPCPP_CALL_JASS(SetUnitTypeSlots, unit, slots); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitUseFood(HUNIT unit, BOOLEAN useFood) { WAR3MAPCPP_CALL_JASS(SetUnitUseFood, unit, useFood); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitUserData(HUNIT unit, int data) { WAR3MAPCPP_CALL_JASS(SetUnitUserData, unit, data); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitVertexColor(HUNIT unit, int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(SetUnitVertexColor, unit, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitX(HUNIT unit, float newX) { WAR3MAPCPP_CALL_JASS(SetUnitX, unit, newX); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitY(HUNIT unit, float newY) { WAR3MAPCPP_CALL_JASS(SetUnitY, unit, newY); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetUnitsFlee(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(SetUnitsFlee, arg1); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SetUpgrade(int arg1) { WAR3MAPCPP_CALL_JASS(SetUpgrade, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetWatchMegaTargets(BOOLEAN arg1) { WAR3MAPCPP_CALL_JASS(SetWatchMegaTargets, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetWaterBaseColor(int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(SetWaterBaseColor, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetWaterDeforms(BOOLEAN val) { WAR3MAPCPP_CALL_JASS(SetWaterDeforms, val); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SetWidgetLife(HWIDGET widget, float newLife) { WAR3MAPCPP_CALL_JASS(SetWidgetLife, widget, newLife); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ShiftTownSpot(float arg1, float arg2) { WAR3MAPCPP_CALL_JASS(ShiftTownSpot, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ShowDestructable(HDESTRUCTABLE d, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(ShowDestructable, d, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ShowImage(HIMAGE Image, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(ShowImage, Image, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ShowInterface(BOOLEAN flag, float fadeDuration) { WAR3MAPCPP_CALL_JASS(ShowInterface, flag, fadeDuration); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ShowUbersplat(HUBERSPLAT Splat, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(ShowUbersplat, Splat, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI ShowUnit(HUNIT unit, BOOLEAN show) { WAR3MAPCPP_CALL_JASS(ShowUnit, unit, show); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI Sin(float radians) { WAR3MAPCPP_CALL_JASS(Sin, radians); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI Sleep(float arg1) { WAR3MAPCPP_CALL_JASS(Sleep, arg1); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI SquareRoot(float x) { WAR3MAPCPP_CALL_JASS(SquareRoot, x); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI StartCampaignAI(HPLAYER num, CJassString script) { WAR3MAPCPP_CALL_JASS(StartCampaignAI, num, script); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI StartGetEnemyBase() { WAR3MAPCPP_CALL_JASS(StartGetEnemyBase); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI StartMeleeAI(HPLAYER num, CJassString script) { WAR3MAPCPP_CALL_JASS(StartMeleeAI, num, script); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI StartSound(HSOUND soundHandle) { WAR3MAPCPP_CALL_JASS(StartSound, soundHandle); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI StartThread(JCALLBACK arg1) { WAR3MAPCPP_CALL_JASS(StartThread, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI StopCamera() { WAR3MAPCPP_CALL_JASS(StopCamera); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI StopGathering() { WAR3MAPCPP_CALL_JASS(StopGathering); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI StopMusic(BOOLEAN fadeOut) { WAR3MAPCPP_CALL_JASS(StopMusic, fadeOut); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI StopSound(HSOUND soundHandle, BOOLEAN killWhenDone, BOOLEAN fadeOut) { WAR3MAPCPP_CALL_JASS(StopSound, soundHandle, killWhenDone, fadeOut); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI StoreBoolean(HGAMECACHE cache, CJassString missionKey, CJassString key, BOOLEAN value) { WAR3MAPCPP_CALL_JASS(StoreBoolean, cache, missionKey, key, value); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI StoreInteger(HGAMECACHE cache, CJassString missionKey, CJassString key, int value) { WAR3MAPCPP_CALL_JASS(StoreInteger, cache, missionKey, key, value); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI StoreReal(HGAMECACHE cache, CJassString missionKey, CJassString key, float value) { WAR3MAPCPP_CALL_JASS(StoreReal, cache, missionKey, key, value); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI StoreString(HGAMECACHE cache, CJassString missionKey, CJassString key, CJassString value) { WAR3MAPCPP_CALL_JASS(StoreString, cache, missionKey, key, value); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI StoreUnit(HGAMECACHE cache, CJassString missionKey, CJassString key, HUNIT unit) { WAR3MAPCPP_CALL_JASS(StoreUnit, cache, missionKey, key, unit); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI StringCase(CJassString source, BOOLEAN upper) { WAR3MAPCPP_CALL_JASS(StringCase, source, upper); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI StringHash(CJassString s) { WAR3MAPCPP_CALL_JASS(StringHash, s); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI StringLength(CJassString s) { WAR3MAPCPP_CALL_JASS(StringLength, s); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI SubString(CJassString source, int start, int end) { WAR3MAPCPP_CALL_JASS(SubString, source, start, end); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SuicidePlayer(HPLAYER arg1, BOOLEAN arg2) { WAR3MAPCPP_CALL_JASS(SuicidePlayer, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI SuicidePlayerUnits(HPLAYER arg1, BOOLEAN arg2) { WAR3MAPCPP_CALL_JASS(SuicidePlayerUnits, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SuicideUnit(int arg1, int arg2) { WAR3MAPCPP_CALL_JASS(SuicideUnit, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SuicideUnitEx(int arg1, int arg2, int arg3) { WAR3MAPCPP_CALL_JASS(SuicideUnitEx, arg1, arg2, arg3); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SuspendHeroXP(HUNIT hero, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(SuspendHeroXP, hero, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SuspendTimeOfDay(BOOLEAN b) { WAR3MAPCPP_CALL_JASS(SuspendTimeOfDay, b); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SyncSelections() { WAR3MAPCPP_CALL_JASS(SyncSelections); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SyncStoredBoolean(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(SyncStoredBoolean, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SyncStoredInteger(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(SyncStoredInteger, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SyncStoredReal(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(SyncStoredReal, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SyncStoredString(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(SyncStoredString, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI SyncStoredUnit(HGAMECACHE cache, CJassString missionKey, CJassString key) { WAR3MAPCPP_CALL_JASS(SyncStoredUnit, cache, missionKey, key); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI Tan(float radians) { WAR3MAPCPP_CALL_JASS(Tan, radians); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TeleportCaptain(float arg1, float arg2) { WAR3MAPCPP_CALL_JASS(TeleportCaptain, arg1, arg2); }
    WA3MAPCPP_FORCE_INLINE HTERRAINDEFORMATION JASSAPI TerrainDeformCrater(float x, float y, float radius, float depth, int duration, BOOLEAN permanent) { WAR3MAPCPP_CALL_JASS(TerrainDeformCrater, x, y, radius, depth, duration, permanent); }
    WA3MAPCPP_FORCE_INLINE HTERRAINDEFORMATION JASSAPI TerrainDeformRandom(float x, float y, float radius, float minDelta, float maxDelta, int duration, int updateInterval) { WAR3MAPCPP_CALL_JASS(TerrainDeformRandom, x, y, radius, minDelta, maxDelta, duration, updateInterval); }
    WA3MAPCPP_FORCE_INLINE HTERRAINDEFORMATION JASSAPI TerrainDeformRipple(float x, float y, float radius, float depth, int duration, int count, float spaceWaves, float timeWaves, float radiusStartPct, BOOLEAN limitNeg) { WAR3MAPCPP_CALL_JASS(TerrainDeformRipple, x, y, radius, depth, duration, count, spaceWaves, timeWaves, radiusStartPct, limitNeg); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TerrainDeformStop(HTERRAINDEFORMATION deformation, int duration) { WAR3MAPCPP_CALL_JASS(TerrainDeformStop, deformation, duration); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TerrainDeformStopAll() { WAR3MAPCPP_CALL_JASS(TerrainDeformStopAll); }
    WA3MAPCPP_FORCE_INLINE HTERRAINDEFORMATION JASSAPI TerrainDeformWave(float x, float y, float dirX, float dirY, float distance, float speed, float radius, float depth, int trailTime, int count) { WAR3MAPCPP_CALL_JASS(TerrainDeformWave, x, y, dirX, dirY, distance, speed, radius, depth, trailTime, count); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TimerDialogDisplay(HTIMERDIALOG Dialog, BOOLEAN display) { WAR3MAPCPP_CALL_JASS(TimerDialogDisplay, Dialog, display); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TimerDialogSetRealTimeRemaining(HTIMERDIALOG Dialog, float timeRemaining) { WAR3MAPCPP_CALL_JASS(TimerDialogSetRealTimeRemaining, Dialog, timeRemaining); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TimerDialogSetSpeed(HTIMERDIALOG Dialog, float speedMultFactor) { WAR3MAPCPP_CALL_JASS(TimerDialogSetSpeed, Dialog, speedMultFactor); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TimerDialogSetTimeColor(HTIMERDIALOG Dialog, int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(TimerDialogSetTimeColor, Dialog, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TimerDialogSetTitle(HTIMERDIALOG Dialog, CJassString title) { WAR3MAPCPP_CALL_JASS(TimerDialogSetTitle, Dialog, title); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TimerDialogSetTitleColor(HTIMERDIALOG Dialog, int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(TimerDialogSetTitleColor, Dialog, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI TimerGetElapsed(HTIMER Timer) { WAR3MAPCPP_CALL_JASS(TimerGetElapsed, Timer); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI TimerGetRemaining(HTIMER Timer) { WAR3MAPCPP_CALL_JASS(TimerGetRemaining, Timer); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI TimerGetTimeout(HTIMER Timer) { WAR3MAPCPP_CALL_JASS(TimerGetTimeout, Timer); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TimerStart(HTIMER Timer, float timeout, BOOLEAN periodic, JCALLBACK handlerFunc) { WAR3MAPCPP_CALL_JASS(TimerStart, Timer, timeout, periodic, handlerFunc); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI TownHasHall(int arg1) { WAR3MAPCPP_CALL_JASS(TownHasHall, arg1); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI TownHasMine(int arg1) { WAR3MAPCPP_CALL_JASS(TownHasMine, arg1); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI TownThreatened() { WAR3MAPCPP_CALL_JASS(TownThreatened); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI TownWithMine() { WAR3MAPCPP_CALL_JASS(TownWithMine); }
    WA3MAPCPP_FORCE_INLINE HTRIGGERACTION JASSAPI TriggerAddAction(HTRIGGER Trigger, JCALLBACK actionFunc) { WAR3MAPCPP_CALL_JASS(TriggerAddAction, Trigger, actionFunc); }
    WA3MAPCPP_FORCE_INLINE HTRIGGERCONDITION JASSAPI TriggerAddCondition(HTRIGGER Trigger, HBOOLEXPR condition) { WAR3MAPCPP_CALL_JASS(TriggerAddCondition, Trigger, condition); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TriggerClearActions(HTRIGGER Trigger) { WAR3MAPCPP_CALL_JASS(TriggerClearActions, Trigger); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TriggerClearConditions(HTRIGGER Trigger) { WAR3MAPCPP_CALL_JASS(TriggerClearConditions, Trigger); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI TriggerEvaluate(HTRIGGER Trigger) { WAR3MAPCPP_CALL_JASS(TriggerEvaluate, Trigger); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TriggerExecute(HTRIGGER Trigger) { WAR3MAPCPP_CALL_JASS(TriggerExecute, Trigger); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TriggerExecuteWait(HTRIGGER Trigger) { WAR3MAPCPP_CALL_JASS(TriggerExecuteWait, Trigger); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterDeathEvent(HTRIGGER Trigger, HWIDGET widget) { WAR3MAPCPP_CALL_JASS(TriggerRegisterDeathEvent, Trigger, widget); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterDialogButtonEvent(HTRIGGER Trigger, HBUTTON Button) { WAR3MAPCPP_CALL_JASS(TriggerRegisterDialogButtonEvent, Trigger, Button); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterDialogEvent(HTRIGGER Trigger, HDIALOG Dialog) { WAR3MAPCPP_CALL_JASS(TriggerRegisterDialogEvent, Trigger, Dialog); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterEnterRegion(HTRIGGER Trigger, HREGION Region, HBOOLEXPR filter) { WAR3MAPCPP_CALL_JASS(TriggerRegisterEnterRegion, Trigger, Region, filter); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterFilterUnitEvent(HTRIGGER Trigger, HUNIT unit, HUNITEVENT Event, HBOOLEXPR filter) { WAR3MAPCPP_CALL_JASS(TriggerRegisterFilterUnitEvent, Trigger, unit, Event, filter); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterGameEvent(HTRIGGER Trigger, HGAMEEVENT GameEvent) { WAR3MAPCPP_CALL_JASS(TriggerRegisterGameEvent, Trigger, GameEvent); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterGameStateEvent(HTRIGGER Trigger, HGAMESTATE State, HLIMITOP opcode, float limitval) { WAR3MAPCPP_CALL_JASS(TriggerRegisterGameStateEvent, Trigger, State, opcode, limitval); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterLeaveRegion(HTRIGGER Trigger, HREGION Region, HBOOLEXPR filter) { WAR3MAPCPP_CALL_JASS(TriggerRegisterLeaveRegion, Trigger, Region, filter); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterPlayerAllianceChange(HTRIGGER Trigger, HPLAYER player, HALLIANCETYPE Alliance) { WAR3MAPCPP_CALL_JASS(TriggerRegisterPlayerAllianceChange, Trigger, player, Alliance); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterPlayerChatEvent(HTRIGGER Trigger, HPLAYER player, CJassString chatMessageToDetect, BOOLEAN exactMatchOnly) { WAR3MAPCPP_CALL_JASS(TriggerRegisterPlayerChatEvent, Trigger, player, chatMessageToDetect, exactMatchOnly); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterPlayerEvent(HTRIGGER Trigger, HPLAYER player, HPLAYEREVENT PlayerEvent) { WAR3MAPCPP_CALL_JASS(TriggerRegisterPlayerEvent, Trigger, player, PlayerEvent); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterPlayerStateEvent(HTRIGGER Trigger, HPLAYER player, HPLAYERSTATE State, HLIMITOP opcode, float limitval) { WAR3MAPCPP_CALL_JASS(TriggerRegisterPlayerStateEvent, Trigger, player, State, opcode, limitval); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterPlayerUnitEvent(HTRIGGER Trigger, HPLAYER player, HPLAYERUNITEVENT PlayerUnitEvent, HBOOLEXPR filter) { WAR3MAPCPP_CALL_JASS(TriggerRegisterPlayerUnitEvent, Trigger, player, PlayerUnitEvent, filter); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterTimerEvent(HTRIGGER Trigger, float timeout, BOOLEAN periodic) { WAR3MAPCPP_CALL_JASS(TriggerRegisterTimerEvent, Trigger, timeout, periodic); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterTimerExpireEvent(HTRIGGER Trigger, HTIMER t) { WAR3MAPCPP_CALL_JASS(TriggerRegisterTimerExpireEvent, Trigger, t); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterTrackableHitEvent(HTRIGGER Trigger, HTRACKABLE t) { WAR3MAPCPP_CALL_JASS(TriggerRegisterTrackableHitEvent, Trigger, t); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterTrackableTrackEvent(HTRIGGER Trigger, HTRACKABLE t) { WAR3MAPCPP_CALL_JASS(TriggerRegisterTrackableTrackEvent, Trigger, t); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterUnitEvent(HTRIGGER Trigger, HUNIT unit, HUNITEVENT Event) { WAR3MAPCPP_CALL_JASS(TriggerRegisterUnitEvent, Trigger, unit, Event); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterUnitInRange(HTRIGGER Trigger, HUNIT unit, float range, HBOOLEXPR filter) { WAR3MAPCPP_CALL_JASS(TriggerRegisterUnitInRange, Trigger, unit, range, filter); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterUnitStateEvent(HTRIGGER Trigger, HUNIT unit, HUNITSTATE State, HLIMITOP opcode, float limitval) { WAR3MAPCPP_CALL_JASS(TriggerRegisterUnitStateEvent, Trigger, unit, State, opcode, limitval); }
    WA3MAPCPP_FORCE_INLINE HEVENT JASSAPI TriggerRegisterVariableEvent(HTRIGGER Trigger, CJassString varName, HLIMITOP opcode, float limitval) { WAR3MAPCPP_CALL_JASS(TriggerRegisterVariableEvent, Trigger, varName, opcode, limitval); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TriggerRemoveAction(HTRIGGER Trigger, HTRIGGERACTION Action) { WAR3MAPCPP_CALL_JASS(TriggerRemoveAction, Trigger, Action); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TriggerRemoveCondition(HTRIGGER Trigger, HTRIGGERCONDITION Condition) { WAR3MAPCPP_CALL_JASS(TriggerRemoveCondition, Trigger, Condition); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TriggerSleepAction(float timeout) { WAR3MAPCPP_CALL_JASS(TriggerSleepAction, timeout); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TriggerSyncReady() { WAR3MAPCPP_CALL_JASS(TriggerSyncReady); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TriggerSyncStart() { WAR3MAPCPP_CALL_JASS(TriggerSyncStart); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TriggerWaitForSound(HSOUND s, float offset) { WAR3MAPCPP_CALL_JASS(TriggerWaitForSound, s, offset); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI TriggerWaitOnSleeps(HTRIGGER Trigger, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(TriggerWaitOnSleeps, Trigger, flag); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitAddAbility(HUNIT unit, int AbilID) { WAR3MAPCPP_CALL_JASS(UnitAddAbility, unit, AbilID); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitAddIndicator(HUNIT unit, int red, int green, int blue, int alpha) { WAR3MAPCPP_CALL_JASS(UnitAddIndicator, unit, red, green, blue, alpha); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitAddItem(HUNIT unit, HITEM item) { WAR3MAPCPP_CALL_JASS(UnitAddItem, unit, item); }
    WA3MAPCPP_FORCE_INLINE HITEM JASSAPI UnitAddItemById(HUNIT unit, int itemId) { WAR3MAPCPP_CALL_JASS(UnitAddItemById, unit, itemId); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitAddItemToSlotById(HUNIT unit, int itemId, int itemSlot) { WAR3MAPCPP_CALL_JASS(UnitAddItemToSlotById, unit, itemId, itemSlot); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitAddSleep(HUNIT unit, BOOLEAN add) { WAR3MAPCPP_CALL_JASS(UnitAddSleep, unit, add); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitAddSleepPerm(HUNIT unit, BOOLEAN add) { WAR3MAPCPP_CALL_JASS(UnitAddSleepPerm, unit, add); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitAddType(HUNIT unit, HUNITTYPE UnitType) { WAR3MAPCPP_CALL_JASS(UnitAddType, unit, UnitType); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitAlive(HUNIT arg1) { WAR3MAPCPP_CALL_JASS(UnitAlive, arg1); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitApplyTimedLife(HUNIT unit, int buffId, float duration) { WAR3MAPCPP_CALL_JASS(UnitApplyTimedLife, unit, buffId, duration); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitCanSleep(HUNIT unit) { WAR3MAPCPP_CALL_JASS(UnitCanSleep, unit); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitCanSleepPerm(HUNIT unit) { WAR3MAPCPP_CALL_JASS(UnitCanSleepPerm, unit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI UnitCountBuffsEx(HUNIT unit, BOOLEAN removePositive, BOOLEAN removeNegative, BOOLEAN magic, BOOLEAN physical, BOOLEAN timedLife, BOOLEAN aura, BOOLEAN autoDispel) { WAR3MAPCPP_CALL_JASS(UnitCountBuffsEx, unit, removePositive, removeNegative, magic, physical, timedLife, aura, autoDispel); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitDamagePoint(HUNIT unit, float delay, float radius, float x, float y, float amount, BOOLEAN attack, BOOLEAN ranged, HATTACKTYPE attackType, HDAMAGETYPE damageType, HWEAPONTYPE weaponType) { WAR3MAPCPP_CALL_JASS(UnitDamagePoint, unit, delay, radius, x, y, amount, attack, ranged, attackType, damageType, weaponType); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitDamageTarget(HUNIT unit, HWIDGET target, float amount, BOOLEAN attack, BOOLEAN ranged, HATTACKTYPE attackType, HDAMAGETYPE damageType, HWEAPONTYPE weaponType) { WAR3MAPCPP_CALL_JASS(UnitDamageTarget, unit, target, amount, attack, ranged, attackType, damageType, weaponType); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitDropItemPoint(HUNIT unit, HITEM item, float x, float y) { WAR3MAPCPP_CALL_JASS(UnitDropItemPoint, unit, item, x, y); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitDropItemSlot(HUNIT unit, HITEM item, int slot) { WAR3MAPCPP_CALL_JASS(UnitDropItemSlot, unit, item, slot); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitDropItemTarget(HUNIT unit, HITEM item, HWIDGET target) { WAR3MAPCPP_CALL_JASS(UnitDropItemTarget, unit, item, target); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitHasBuffsEx(HUNIT unit, BOOLEAN removePositive, BOOLEAN removeNegative, BOOLEAN magic, BOOLEAN physical, BOOLEAN timedLife, BOOLEAN aura, BOOLEAN autoDispel) { WAR3MAPCPP_CALL_JASS(UnitHasBuffsEx, unit, removePositive, removeNegative, magic, physical, timedLife, aura, autoDispel); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitHasItem(HUNIT unit, HITEM item) { WAR3MAPCPP_CALL_JASS(UnitHasItem, unit, item); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI UnitId(CJassString unitIdString) { WAR3MAPCPP_CALL_JASS(UnitId, unitIdString); }
    WA3MAPCPP_FORCE_INLINE CJassStringSID JASSAPI UnitId2String(int unitId) { WAR3MAPCPP_CALL_JASS(UnitId2String, unitId); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitIgnoreAlarm(HUNIT unit, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(UnitIgnoreAlarm, unit, flag); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitIgnoreAlarmToggled(HUNIT unit) { WAR3MAPCPP_CALL_JASS(UnitIgnoreAlarmToggled, unit); }
    WA3MAPCPP_FORCE_INLINE int JASSAPI UnitInventorySize(HUNIT unit) { WAR3MAPCPP_CALL_JASS(UnitInventorySize, unit); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitInvis(HUNIT arg1) { WAR3MAPCPP_CALL_JASS(UnitInvis, arg1); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitIsSleeping(HUNIT unit) { WAR3MAPCPP_CALL_JASS(UnitIsSleeping, unit); }
    WA3MAPCPP_FORCE_INLINE HITEM JASSAPI UnitItemInSlot(HUNIT unit, int itemSlot) { WAR3MAPCPP_CALL_JASS(UnitItemInSlot, unit, itemSlot); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitMakeAbilityPermanent(HUNIT unit, BOOLEAN permanent, int AbilID) { WAR3MAPCPP_CALL_JASS(UnitMakeAbilityPermanent, unit, permanent, AbilID); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitModifySkillPoints(HUNIT hero, int skillPointDelta) { WAR3MAPCPP_CALL_JASS(UnitModifySkillPoints, hero, skillPointDelta); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitPauseTimedLife(HUNIT unit, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(UnitPauseTimedLife, unit, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitPoolAddUnitType(HUNITPOOL Pool, int unitId, float weight) { WAR3MAPCPP_CALL_JASS(UnitPoolAddUnitType, Pool, unitId, weight); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitPoolRemoveUnitType(HUNITPOOL Pool, int unitId) { WAR3MAPCPP_CALL_JASS(UnitPoolRemoveUnitType, Pool, unitId); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitRemoveAbility(HUNIT unit, int AbilID) { WAR3MAPCPP_CALL_JASS(UnitRemoveAbility, unit, AbilID); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitRemoveBuffs(HUNIT unit, BOOLEAN removePositive, BOOLEAN removeNegative) { WAR3MAPCPP_CALL_JASS(UnitRemoveBuffs, unit, removePositive, removeNegative); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitRemoveBuffsEx(HUNIT unit, BOOLEAN removePositive, BOOLEAN removeNegative, BOOLEAN magic, BOOLEAN physical, BOOLEAN timedLife, BOOLEAN aura, BOOLEAN autoDispel) { WAR3MAPCPP_CALL_JASS(UnitRemoveBuffsEx, unit, removePositive, removeNegative, magic, physical, timedLife, aura, autoDispel); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitRemoveItem(HUNIT unit, HITEM item) { WAR3MAPCPP_CALL_JASS(UnitRemoveItem, unit, item); }
    WA3MAPCPP_FORCE_INLINE HITEM JASSAPI UnitRemoveItemFromSlot(HUNIT unit, int itemSlot) { WAR3MAPCPP_CALL_JASS(UnitRemoveItemFromSlot, unit, itemSlot); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitRemoveType(HUNIT unit, HUNITTYPE UnitType) { WAR3MAPCPP_CALL_JASS(UnitRemoveType, unit, UnitType); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitResetCooldown(HUNIT unit) { WAR3MAPCPP_CALL_JASS(UnitResetCooldown, unit); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitSetConstructionProgress(HUNIT unit, int constructionPercentage) { WAR3MAPCPP_CALL_JASS(UnitSetConstructionProgress, unit, constructionPercentage); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitSetUpgradeProgress(HUNIT unit, int upgradePercentage) { WAR3MAPCPP_CALL_JASS(UnitSetUpgradeProgress, unit, upgradePercentage); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitSetUsesAltIcon(HUNIT unit, BOOLEAN flag) { WAR3MAPCPP_CALL_JASS(UnitSetUsesAltIcon, unit, flag); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitShareVision(HUNIT unit, HPLAYER player, BOOLEAN share) { WAR3MAPCPP_CALL_JASS(UnitShareVision, unit, player, share); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitStripHeroLevel(HUNIT hero, int howManyLevels) { WAR3MAPCPP_CALL_JASS(UnitStripHeroLevel, hero, howManyLevels); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitSuspendDecay(HUNIT unit, BOOLEAN suspend) { WAR3MAPCPP_CALL_JASS(UnitSuspendDecay, unit, suspend); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitUseItem(HUNIT unit, HITEM item) { WAR3MAPCPP_CALL_JASS(UnitUseItem, unit, item); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitUseItemPoint(HUNIT unit, HITEM item, float x, float y) { WAR3MAPCPP_CALL_JASS(UnitUseItemPoint, unit, item, x, y); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI UnitUseItemTarget(HUNIT unit, HITEM item, HWIDGET target) { WAR3MAPCPP_CALL_JASS(UnitUseItemTarget, unit, item, target); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnitWakeUp(HUNIT unit) { WAR3MAPCPP_CALL_JASS(UnitWakeUp, unit); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI UnregisterStackedSound(HSOUND soundHandle, BOOLEAN byPosition, float rectwidth, float rectheight) { WAR3MAPCPP_CALL_JASS(UnregisterStackedSound, soundHandle, byPosition, rectwidth, rectheight); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI Unsummon(HUNIT arg1) { WAR3MAPCPP_CALL_JASS(Unsummon, arg1); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI VersionCompatible(HVERSION Version) { WAR3MAPCPP_CALL_JASS(VersionCompatible, Version); }
    WA3MAPCPP_FORCE_INLINE HVERSION JASSAPI VersionGet() { WAR3MAPCPP_CALL_JASS(VersionGet); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI VersionSupported(HVERSION Version) { WAR3MAPCPP_CALL_JASS(VersionSupported, Version); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI VolumeGroupReset() { WAR3MAPCPP_CALL_JASS(VolumeGroupReset); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI VolumeGroupSetVolume(HVOLUMEGROUP vgroup, float scale) { WAR3MAPCPP_CALL_JASS(VolumeGroupSetVolume, vgroup, scale); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI WaitGetEnemyBase() { WAR3MAPCPP_CALL_JASS(WaitGetEnemyBase); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI WaygateActivate(HUNIT waygate, BOOLEAN activate) { WAR3MAPCPP_CALL_JASS(WaygateActivate, waygate, activate); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI WaygateGetDestinationX(HUNIT waygate) { WAR3MAPCPP_CALL_JASS(WaygateGetDestinationX, waygate); }
    WA3MAPCPP_FORCE_INLINE DWFP JASSAPI WaygateGetDestinationY(HUNIT waygate) { WAR3MAPCPP_CALL_JASS(WaygateGetDestinationY, waygate); }
    WA3MAPCPP_FORCE_INLINE BOOLEAN JASSAPI WaygateIsActive(HUNIT waygate) { WAR3MAPCPP_CALL_JASS(WaygateIsActive, waygate); }
    WA3MAPCPP_FORCE_INLINE void JASSAPI WaygateSetDestination(HUNIT waygate, float x, float y) { WAR3MAPCPP_CALL_JASS(WaygateSetDestination, waygate, x, y); }
}
