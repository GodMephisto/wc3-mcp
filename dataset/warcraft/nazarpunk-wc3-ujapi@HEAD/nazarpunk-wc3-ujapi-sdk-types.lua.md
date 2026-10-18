---
type: vendor-doc
source_repo: "https://github.com/nazarpunk/wc3-ujapi"
source_path: "sdk/types.lua"
ref: HEAD
---

---@class handle:number
---@class real:number
---@class integer:number
---@class code:function

-- https://github.com/Ev3nt/war3_lua/blob/763c5d3b04dc6db60cad9ed27ab3e8e588b67581/Src/LuaFunctions.cpp#L751

---@param raw string
---@return integer
function FourCC(raw) end

---@param id integer
---@return string
function IdToString(id) end

---@param s string
---@return integer
function StringToId(s) end

---@return boolean
function IsDevMode() end

---@return boolean
function IsUjAPI() end

function printc(...) end

---@param fromRoot boolean
---@return string
function GetMapFileName(fromRoot) end