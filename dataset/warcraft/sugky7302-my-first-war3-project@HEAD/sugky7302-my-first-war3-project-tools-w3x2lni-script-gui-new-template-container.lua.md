---
type: vendor-doc
source_repo: "https://github.com/sugky7302/my-first-war3-project"
source_path: "tools/w3x2lni/script/gui/new/template/container.lua"
ref: HEAD
---

local gui = require 'yue.gui'
local ca = require 'gui.new.common_attribute'

return function (t, data)
    local o = gui.Container.create()
    if t.style then
        o:setstyle(t.style)
    end
    local bind = {}
    ca.label_color(o, t, data, bind)
    return o
end
