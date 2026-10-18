---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/ffi/sleep.lua"
ref: HEAD
---

local ffi = require 'ffi'
ffi.cdef[[
    void Sleep(unsigned long dwMilliseconds);
]]

return function (time)
    ffi.C.Sleep(time)
end
