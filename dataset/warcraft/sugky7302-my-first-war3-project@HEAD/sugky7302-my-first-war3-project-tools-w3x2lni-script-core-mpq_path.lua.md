---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/core/mpq_path.lua"
ref: HEAD
---

return function ()
    local mt = {}
    local archive = nil
    function mt:open(path)
        archive = path
    end
    function mt:close(path)
        if archive == path then
            archive = nil
        end
    end
    function mt:each_path(callback)
        if archive then
            local buf = callback(archive)
            if buf then
                return buf
            end
        end
        local buf = callback('')
        if buf then
            return buf
        end
    end
    return mt
end
