---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/share/check_lni_mark.lua"
ref: HEAD
---

return function (buf)
    if not buf then
        return false
    end
    if buf:sub(9, 12) == 'W2L\x01' then
        return true
    end
    return false
end
