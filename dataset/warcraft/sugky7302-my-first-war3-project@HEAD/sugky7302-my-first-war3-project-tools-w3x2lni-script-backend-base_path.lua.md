---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/backend/base_path.lua"
ref: HEAD
---

fs = require 'bee.filesystem'

if _W2L_DIR then
    return fs.path(_W2L_DIR)
end

return fs.current_path():parent_path()
