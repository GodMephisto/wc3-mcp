---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/core/ui-builder/init.lua"
ref: HEAD
---

return {
    old_reader = require 'ui-builder.old-reader',
    new_reader = require 'ui-builder.new-reader',
    old_writer = require 'ui-builder.old-writer',
    new_writer = require 'ui-builder.new-writer',
    merge = require 'ui-builder.merge',
}
