---
type: vendor-doc
source_repo: "https://github.com/Kolad2/war3-lua-table"
source_path: "meta.lua"
ref: HEAD
---

do
    table = table or {}

    table.meta = {
        __index = table,
        __tostring = table.tostring,
        __add = nil,             -- сложение
        __sub = nil,             -- вычитание
        __mul = table.multiply,  -- умножение
        __div = table.divide     -- деление
    }

    function table:setmetatable(tbl)
        return setmetatable(tbl, self.meta)
    end

    setmetatable(table, {
        __call = table.setmetatable
    })

end
