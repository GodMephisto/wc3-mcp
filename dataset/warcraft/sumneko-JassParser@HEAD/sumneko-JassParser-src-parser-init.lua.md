---
type: vendor-doc
source_repo: "https://github.com/sumneko/JassParser"
source_path: "src/parser/init.lua"
ref: HEAD
---

local parser = require 'parser.parser'
local checker = require 'parser.checker'

local api = {
    parser       = parser,
    checker      = checker,
}

return api
