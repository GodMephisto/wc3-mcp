---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/backend/command.lua"
ref: HEAD
---

local res = {}
res[#res+1] = arg[1]
for i = 2, #arg do
    local cmd = arg[i]
    if cmd:sub(1, 1) == '-' then
        local k, v
        local pos = cmd:find('=', 1, true)
        if pos then
            k = cmd:sub(2, pos - 1)
            v = cmd:sub(pos + 1)
        else
            k = cmd:sub(2)
            v = true
        end
        res[k] = v
    else
        res[#res+1] = cmd
    end
end

return res
