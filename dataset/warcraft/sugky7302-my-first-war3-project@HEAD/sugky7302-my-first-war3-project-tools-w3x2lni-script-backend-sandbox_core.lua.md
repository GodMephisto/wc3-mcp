---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/backend/sandbox_core.lua"
ref: HEAD
---

local config = require 'share.config'
local root = require 'backend.w2l_path'
local data_load_path = root / config.global.data_load
local data_load = assert(load(io.load(data_load_path), '@'..data_load_path:string(), 't'))()

return (require 'backend.sandbox')('.\\core\\', io.open, {
    ['w3xparser'] = require 'w3xparser',
    ['lni']       = require 'lni',
    ['lpeglabel'] = require 'lpeglabel',
    ['lml']       = require 'lml',
    ['lang']      = require 'share.lang',
    ['data_load'] = data_load,
})
