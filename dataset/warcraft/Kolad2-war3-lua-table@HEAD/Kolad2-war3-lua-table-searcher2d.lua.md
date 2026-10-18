---
type: vendor-doc
source_repo: "https://github.com/Kolad2/war3-lua-table"
source_path: "searcher2d.lua"
ref: HEAD
---


do
    table = table or {}
    table.tools = table.tools or {}
    local tools = table.tools
    tools.Searcher2d = tools.Searcher2d or {}
    
    ---@class Searcher2d
    ---@overload fun(xmin:number, xmax:number, ymin:number, ymax:number):Searcher2d
    local Searcher2d = tools.Searcher2d or {}

    Searcher2d.__meta = {
        __index = Searcher2d
    }

    Searcher2d.create = function(cls, xmin, xmax, ymin, ymax)
        local obj = setmetatable({}, cls.__meta)
        local cell_height = 1
        local cell_width = 1
        obj.grid = Grid(xmin, xmax, ymin, ymax, cell_width, cell_height)
        -- for _, item in ipairs(locations) do obj:insert(item) end
        return obj
    end

    function Searcher2d:insert(item)
        self.grid:insert(item.x, item.y, item)
    end

    function Searcher2d:insert_list(list_items)
        for _, item in ipairs(list_items) do
            self:insert(item)
        end
    end

    function Searcher2d:insert(item)
        self.grid:insert(item.x, item.y, item)
    end

    function Searcher2d:is_in_box(item, xmin, xmax, ymin, ymax)
        local x, y = item.x, item.y
        return x > xmin and x <= xmax and
               y > ymin and y <= ymax
    end

    function Searcher2d:is_in_range(item, x_0, y_0, r)
        local dx, dy = item.x - x_0, item.y - y_0
        return dx*dx + dy*dy <= r*r
    end

    function Searcher2d:get_cells_within_box(xmin, xmax, ymin, ymax)
        return self.grid:get_cells_within_box(xmin, xmax, ymin, ymax)
    end

    function Searcher2d:find_in_box(xmin, xmax, ymin, ymax)
        local cells = self:get_cells_within_box(xmin, xmax, ymin, ymax)
        local items = table.empty()
        for _, cell in ipairs(cells) do
            for _, item in ipairs(cell) do
                if self:is_in_box(item, xmin, xmax, ymin, ymax) then
                    table.insert(items, item)
                end
            end
        end
        return items
    end

    setmetatable(Searcher2d,{ __call = Searcher2d.create})
end

