---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/ffi/getmodule.lua"
ref: HEAD
---

local ffi = require 'ffi'
local uni = require 'ffi.unicode'

ffi.cdef[[
    int GetModuleHandleW(const wchar_t* libname);
]]

return function(name)
    local wpath = uni.u2w(name)
    return ffi.C.GetModuleHandleW(wpath)
end
