---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/backend/cli/version.lua"
ref: HEAD
---

local cl = require 'share.changelog'
local messager = require 'share.messager'

return function()
    messager.raw('W3x2Lni v' .. cl[1].version)
    local ok, gl = pcall(require, 'share.gitlog')
    if ok then
        messager.raw('\ncommit: ' .. gl.commit)
        messager.raw('\ndate: ' .. gl.date)
    end
end
