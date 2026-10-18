---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/backend/ydwe_path.lua"
ref: HEAD
---

return function()
    local registry = require 'bee.registry'
    local commands = registry.open [[HKEY_CURRENT_USER\SOFTWARE\Classes\YDWEMap\shell\run_war3\command]]
    if not commands then
        return
    end
    local command = commands['']
    if not command then
        return
    end
    local path = command:match '^"([^"]*)"'
    local ydwe = fs.path(path):parent_path()
    if fs.exists(ydwe / 'YDWE.exe') then
        return ydwe
    end
    local ydwe = ydwe:parent_path()
    if fs.exists(ydwe / 'YDWE.exe') then
        return ydwe
    end
end
