---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/gui/push_error.lua"
ref: HEAD
---

fs = require 'bee.filesystem'
local process = require 'bee.subprocess'
local root = fs.current_path()

return function (err)
    local app = root:parent_path() / 'bin' / 'w3x2lni-lua.exe'
    local script = root / 'crashreport' / 'init.lua'
    local p = process.spawn {
        app:string(),
        '-E',
        '-e', ('package.cpath=[[%s]]'):format(package.cpath),
        '-e', ('package.path=[[%s]]'):format(package.path),
        script:string(),
        console = 'disable',
        stdin = true,
        cwd = root:string()
    }
    if not p then
        return
    end
    p.stdin:write(err)
    p.stdin:close()
    p:wait()
end
