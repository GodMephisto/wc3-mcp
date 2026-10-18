---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/backend/w2l_path.lua"
ref: HEAD
---

fs = require 'bee.filesystem'

return fs.current_path():parent_path()
