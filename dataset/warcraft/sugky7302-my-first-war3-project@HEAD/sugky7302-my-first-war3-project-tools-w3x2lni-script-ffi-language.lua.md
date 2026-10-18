---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/ffi/language.lua"
ref: HEAD
---

local ffi = require 'ffi'

ffi.cdef[[
unsigned long __stdcall GetUserDefaultLCID();
int __stdcall GetLocaleInfoA(unsigned long Locale, unsigned long LCType, char* lpLCData, int cchData);
]]

local LOCALE_SISO639LANGNAME  = 0x00000059
local LOCALE_SISO3166CTRYNAME = 0x0000005A

local function getLocaleInfo(locale, type)
    local buf = ffi.new('char[9]')
    local len = ffi.C.GetLocaleInfoA(locale, type, buf, 9)
    if len <= 1 then
        return
    end
    return ffi.string(buf, len-1)
end

return function(name)
    local locale = ffi.C.GetUserDefaultLCID()
    local lang = getLocaleInfo(locale, LOCALE_SISO639LANGNAME)
    local ctry = getLocaleInfo(locale, LOCALE_SISO3166CTRYNAME)
    if not lang or not ctry then
        return
    end
    return lang .. ctry
end
