---
type: vendor-doc
source_repo: "https://github.com/Kolad2/war3-lua-table"
source_path: "grid2d.lua"
ref: HEAD
---

do
    table = table or {}
    table.tools = table.tools or {}


    ---@class Grid2D
    table.tools.Grid2D = table.tools.Grid2D or {}
    local Grid2D = table.tools.Grid2D

    function Grid2D:get_indices(x, y)
        local j = math.floor((y - self.ymin) / self.cell_height) + 1
        local i = math.floor((x - self.xmin) / self.cell_width) + 1
        return i, j
    end

    function Grid2D:get_center(i, j)
        local x_c = self.xmin + (i - 0.5) * self.cell_width
        local y_c = self.ymin + (j - 0.5) * self.cell_height
        return x_c, y_c
    end

    function Grid2D:get_cells_within_circle(x_0, y_0, r)
        -- Определяем границы прямоугольника, охватывающего окружность
        -- Получаем индексы ячеек, охватывающих прямоугольник
        local i_min, j_min = self:get_indices(x_0 - r, y_0 - r)
        local i_max, j_max = self:get_indices(x_0 + r, y_0 + r)
        local r2 = r*r
        local cells = {}

        -- Проходим по всем ячейкам в пределах прямоугольника
        for i = i_min, i_max do
            for j = j_min, j_max do
                local cell = self.cells[i][j]
                local x_c, y_c = self:get_center(i, j)
                -- Получаем координаты границ текущей ячейки
                local cell_xmin = x_c - self.cell_width * 0.5
                local cell_xmax = x_c + self.cell_width * 0.5
                local cell_ymin = y_c - self.cell_height * 0.5
                local cell_ymax = y_c + self.cell_height * 0.5

                -- Находим ближайшую точку ячейки к центру окружности
                local dx_closest = math.max(cell_xmin, math.min(x_0, cell_xmax)) - x_0
                local dy_closest = math.max(cell_ymin, math.min(y_0, cell_ymax)) - y_0

                -- Если расстояние меньше или равно квадрату радиуса, ячейка пересекает окружность
                if dx_closest*dx_closest + dy_closest*dy_closest <= r2 then
                    table.insert(cells, cell)
                end
            end
        end

        return cells
    end

    function Grid2D:get_cells_within_box(xmin, xmax, ymin, ymax)
        local i_min, j_min = self:get_indices(xmin, ymin)
        local i_max, j_max = self:get_indices(xmax, ymax)
        local cells = table.empty()
        for i = i_min, i_max do
            for j = j_min, j_max do
                table.insert(cells, self.cells[i][j])
            end
        end
        return cells
    end

    function Grid2D:get_cell(x, y)
        local i, j = self:get_indices(x, y)
        if not self:is_in_bounds(i, j) then
            print("Grid error: попытка получить ячейку за границами сетки")
            return nil
        end
        return self.cells[i][j]
    end

    function Grid2D:insert(x, y, item)
        local cell = self:get_cell(x, y)
        table.insert(cell, item)
    end

    function Grid2D:get_neighbors(x, y)
        local neighbors = {}
        local i, j = self:get_indices(x, y)
        for di = -1, 1 do
            for dj = -1, 1 do
                local ni = i + di
                local nj = j + dj
                if (not (di == 0 and dj == 0)) and self:is_in_bounds(ni, nj) then --  and
                    local cell = self.cells[ni][nj]
                    if cell then
                        table.insert(neighbors, cell)
                    end
                end
            end
        end
        return neighbors
    end

    function Grid2D:is_in_bounds(i, j)
        return (j > 0 and j <= self.shape[1]) and (i >  0 and i <= self.shape[2])
    end

    function Grid2D:tostring()
        return self.cells:tostring()
    end



    Grid2D.__meta = {
        __index = Grid2D,
        __tostring = Grid2D.tostring
    }


    ---@return Grid2D
    function Grid2D.create(cls, shape, xmin, xmax, ymin, ymax)
        local grid = setmetatable({}, cls.__meta)
        grid.xmax = xmax
        grid.xmin = xmin
        grid.ymin = ymin
        grid.ymax = ymax
        grid.width  = xmax - xmin
        grid.height = ymax - ymin
        grid.cell_width  = grid.width/shape[1]
        grid.cell_height = grid.height/shape[2]
        grid.shape = shape
        grid.cells = NDArray(grid.shape):fill(table.empty)
        return grid
    end

    setmetatable(Grid2D,{ __call = Grid2D.create})
end
