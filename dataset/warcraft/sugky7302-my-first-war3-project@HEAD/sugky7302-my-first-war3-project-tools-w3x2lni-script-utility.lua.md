---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/utility.lua"
ref: HEAD
---

if io.load then
    return
end

function io.load(file_path)
    local f, e = io.open(file_path:string(), "rb")
    if f then
        if f:read(3) ~= '\xEF\xBB\xBF' then
            f:seek('set')
        end
        local content = f:read 'a'
        f:close()
        return content
    else
        return false, e
    end
end

function io.save(file_path, content)
    local f, e = io.open(file_path:string(), "wb")
    if f then
        f:write(content)
        f:close()
        return true
    else
        return false, e
    end
end
