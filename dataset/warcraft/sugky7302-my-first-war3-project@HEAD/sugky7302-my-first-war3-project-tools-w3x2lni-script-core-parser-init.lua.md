---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/core/parser/init.lua"
ref: HEAD
---

local parser = require 'parser.parser'
local checker = require 'parser.checker'

local api = {
    parser       = parser,
    checker      = checker,
}

return api
