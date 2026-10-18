---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/backend/init.lua"
ref: HEAD
---

local command = require 'backend.command'
local messager = require 'share.messager'
local lang = require 'share.lang'
local act = command[1]

if not act then
    act = 'help'
end
if package.searchpath('backend.cli.' .. act, package.path) then
    local fn = require('backend.cli.' .. act)
    if type(fn) == 'function' then
        fn(command)
    end
else
    messager.raw(lang.raw.INVALID:format(act))
end
