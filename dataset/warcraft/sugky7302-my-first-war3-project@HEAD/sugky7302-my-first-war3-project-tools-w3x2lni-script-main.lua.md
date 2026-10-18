---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/main.lua"
ref: HEAD
---

if _W2L_MODE == 'CLI' then
    require 'backend'
    return
elseif _W2L_MODE == 'GUI' then
    require 'gui.new.main'
end
