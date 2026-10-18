---
type: vendor-doc
source_repo: "https://github.com/Kolad2/war3-lua-table"
source_path: "table.lua"
ref: HEAD
---

do
    --[[war3-lua-table (27.09.2024)]]--
    ---@class table
    ---@field unique fun(tbl: table): table, table
    ---@field find fun(tbl: table, value: any): table
    ---@field find_first fun(tbl: table, value: any, pos_start: number): table|nil
    ---@field get_meta_compatible fun(tbl:table,...):table
    ---@field merge fun(tbl:table,...):table
    ---@field get_random fun(tbl:table):any
    ---@field get fun(tbl:table, idx:number):any
    ---@field empty fun():table
    ---@field fill fun(tbl:table, value:number, pos_start:number, pos_end:number):table
    ---@field move fun(tbl:table, pos_start:number, pos_end:number, tbl_to:number, pos_to:number):table
    ---@field is_sorted fun(tbl:table):boolean
    ---@field copy fun(tbl:table):table
    ---@field slice fun(tbl:table, pos_start:number, pos_end:number):table
    ---@field multiply fun(tbl:table, value:number):table
    ---@field divide fun(tbl:table, value:number):table
    ---@field subtract fun(tbl:table, value:number):table
    ---@field add fun(tbl:table, value:number):table
    ---@field argmin fun(tbl:table):number
    ---@field argmax fun(tbl:table):number
    ---@field min fun(tbl:table):number
    ---@field max fun(tbl:table):number
    ---@field cumsum fun(tbl:table):table
    ---@field sum fun(tbl:table):number
    ---@field reverse fun(tbl:table):table
    ---@field shuffle fun(tbl:table):table
    ---@field remove_swap fun(tbl:table, idx:number):any
    ---@field remove fun(tbl:table, idx:number):any
    ---@field tostring fun(tbl:table):string
    ---@field pack fun(...):table
    ---@field unpack fun(tbl:table, start_pos:number, end_pos:number)
    ---@field insert fun(tbl:table, pos:number, value:number):table
    table = table or {}

    ---@return table
    table.empty = function()
        return {}
    end
    
    ---insert
    ---@param tbl table
    ---@param pos number
    ---@param value number
    ---@return table
    table.insert = table.insert or
    function(tbl, pos, value)
        -- Определяем, передано ли значение или только позиция
        if value == nil then
            pos, value = #tbl + 1, pos
        end
        -- Если pos выходит за границы текущей длины таблицы, добавляем в конец
        if pos > #tbl + 1 then
            pos = #tbl + 1
        end

        -- Сдвигаем элементы вправо, начиная с позиции pos
        for i = #tbl, pos, -1 do
            tbl[i + 1] = tbl[i]
        end
        -- Вставляем значение на позицию pos
        tbl[pos] = value
        return tbl
    end

    ---unpack
    ---@param tbl table
    ---@param start_pos number
    ---@param end_pos number
    ---@return table
    table.unpack = table.unpack or
    function(tbl, start_pos, end_pos)
        start_pos = start_pos or 1
        end_pos = end_pos or #tbl
        if start_pos <= end_pos then
            return tbl[start_pos], table.unpack(tbl, start_pos + 1, end_pos)
        end
    end

    ---pack
    ---@return table
    table.pack = table.pack or
    function(...)
        return { ... }
    end

    ---tostring
    ---@param tbl table
    ---@return table
    table.tostring = table.tostring or
    function(tbl)
        if getmetatable(tbl) and getmetatable(tbl).__tostring then return tostring(tbl) end
        if tbl == nil then return tostring(nil) end
        if type(tbl) ~= "table" then return tostring(tbl) end
        local str = "{"
        if #tbl == 0 then return str .. "}" end
        if #tbl == 1 then return str .. table.tostring(tbl[1]) .. "}" end
        str = str .. table.tostring(tbl[1])
        for i=2, #tbl do
            str = str .. "," .. table.tostring(tbl[i])
        end
        str = str .. "}"
        return str
    end

    ---remove
    ---@param tbl table
    ---@param tbl number|nil
    ---@return any
    table.remove = table.remove or function(tbl, idx)
        idx = idx or #tbl
        local item = tbl[idx]
        for i = idx, #tbl - 1 do
            tbl[i] = tbl[i + 1]
        end
        tbl[#tbl] = nil
        return item
    end


    ---remove_swap
    ---@param tbl table
    ---@param idx number
    ---@return any
    table.remove_swap = table.remove_swap or
    function(tbl, idx)
        idx = idx or #tbl
        local item = tbl[idx]
        tbl[idx]  = tbl[#tbl]
        tbl[#tbl] = nil
        return item
    end


    ---shuffle
    ---@param tbl table
    ---@return table
    table.shuffle = table.shuffle or
    function(tbl)
        -- алгоритм случайного тасования Дурштенфельда
        for i = #tbl, 2, -1 do
            local j = math.random(1, i)
            tbl[i], tbl[j] = tbl[j], tbl[i]
        end
    end

    ---reverse
    ---@param tbl table
    ---@return table
    table.reverse = table.reverse or
    function(tbl)
        if #tbl <= 1 then return end
        local n = math.floor(#tbl/2)
        for i=1, n do
            tbl[i], tbl[#tbl-i+1] = tbl[#tbl-i+1], tbl[i]
        end
    end

    ---sum
    ---@param tbl table
    ---@return number
    table.sum = table.sum or
    function(tbl)
        local sum = 0
        for i=1, #tbl do
            sum = sum + tbl[i]
        end
        return sum
    end

    ---cumsum
    ---@param tbl table
    ---@return table
    table.cumsum =
    function(tbl)
        local cs = table.empty()
        local sum = 0
        for i=1, #tbl do
            sum = sum + tbl[i]
            cs[i] = sum
        end
        return cs
    end

    ---max
    ---@param tbl table
    ---@return table
    table.max =
    function(tbl)
        local len = #tbl
        if len == 0 then return nil end
        if len == 1 then return tbl[1] end
        local max = tbl[1]
        for i=2, #tbl do
            if tbl[i] > max then max = tbl[i] end
        end
        return max
    end

    ---min
    ---@param tbl table
    ---@return table
    table.min =
    function(tbl)
        local len = #tbl
        if len == 0 then return nil end
        if len == 1 then return tbl[1] end
        local min = tbl[1]
        for i=2, #tbl do
            if tbl[i] < min then min = tbl[i] end
        end
        return min
    end

    ---argmax
    ---@param tbl table
    ---@return table
    table.argmax =
    function(tbl)
        local len = #tbl
        if len == 0 then return nil end
        if len == 1 then return 1 end
        local idx = 1
        for i=2, #tbl do
            if tbl[i] > tbl[idx] then idx = i end
        end
        return idx
    end

    ---argmin
    ---@param tbl table
    ---@return table
    table.argmin =
    function(tbl)
        local len = #tbl
        if len == 0 then return nil end
        if len == 1 then return 1 end
        local idx = 1
        for i=2, #tbl do
            if tbl[i] < tbl[idx] then idx = i end
        end
        return idx
    end

    
    ---add
    ---@param tbl table
    ---@param value number
    ---@return table
    table.add =
    function(tbl, value)
          for i=1, #tbl do
            tbl[i] = tbl[i] + value
          end
        return tbl
    end

    
    ---subtract
    ---@param tbl table
    ---@param val number
    ---@return table
    table.subtract =
    function(tbl, val)
        for i=1, #tbl do
            tbl[i] = tbl[i] - val
        end
        return tbl
    end

    
    ---multiply
    ---@param tbl table
    ---@param val number
    ---@return table
    table.multiply =
    function(tbl, val)
        for i=1, #tbl do
            tbl[i] = tbl[i] * val
        end
        return tbl
    end
    

    ---divide
    ---@param tbl table
    ---@param val number
    ---@return table
    table.divide =
    function(tbl, val)
        for i=1, #tbl do
            tbl[i] = tbl[i] / val
        end
        return tbl
    end

    ---slice
    ---@param tbl table
    ---@param pos_start number
    ---@param pos_end number
    table.slice =
    function(tbl, pos_start, pos_end)
        local slice = {}
        local length = pos_end - pos_start + 1
        local _pos_start = pos_start - 1
        for i = 1, length do
            slice[i] = tbl[_pos_start + i]
        end
        return slice
    end
    
    --------------- fuctional utils ------------
    table.each = function(tbl, callback,...)
        for key, value in pairs(tbl) do
            callback(key, value,...)
        end
    end
    
    table.ieach = function(tbl, callback,...)
        for i, value in ipairs(tbl) do
            callback(i, value,...)
        end
    end
    
    table.map = function(tbl, callback,...)
        local map = table.empty()
        for i, value in ipairs(tbl) do
            map[i] = callback(value,...)
        end
        return map
    end
    --------------------------------------------

    ---copy
    ---@param tbl table
    ---@return table
    table.copy = table.copy or
    function(tbl, callback)
        callback = callback or function(value) return value end
        local tbl_copy = table.map(tbl, callback)
        return setmetatable(tbl_copy, getmetatable(tbl))
    end
    
    
    ---deepcopy
    ---@param tbl table
    ---@return table
    table.deepcopy = table.deepcopy or
    function(tbl)
        return table.copy(tbl, function(value)
            if type(value) == "table" then
                local meta = getmetatable(tbl)
                local deepcopy = meta and meta.__deepcopy or table.deepcopy
                return deepcopy(value)
            else
                return value
            end
        end)
    end


    ---is_sorted
    ---@param tbl table
    ---@return boolean
    table.is_sorted =
    function(tbl)
        for i = 2, #tbl do
            if tbl[i - 1] >= tbl[i] then
                return false
            end
        end
        return true
    end

    ---move
    ---@param tbl table
    ---@param pos_start number
    ---@param pos_end number
    ---@param pos_to number
    ---@param tbl_to table
    ---@return table
    table.move =
    function(tbl, pos_start, pos_end,  pos_to, tbl_to)
        tbl_to = tbl_to or tbl
        pos_to = pos_to or 1
        
        local offset = pos_to - pos_start -- перекрытие регионов
        if offset > 0 then
            for i = pos_end, pos_start, -1 do
                tbl_to[i + offset] = tbl[i]
            end
        else
            for i = pos_start, pos_end do
                tbl_to[i + offset] = tbl[i]
            end
        end
        return tbl_to
    end

    ---fill
    ---@param tbl table
    ---@param value number|function
    ---@param pos_start number|nil
    ---@param pos_end number|nil
    table.fill =
    function(tbl, value, pos_start, pos_end)
        pos_start = pos_start or 1
        if pos_end == nil then pos_start, pos_end = 1, pos_start or #tbl end
        if type(value) == "number" then
            for i = pos_start, pos_end do
                tbl[i] = value
            end
        else
            for i = pos_start, pos_end do
                tbl[i] = value(i)
            end
        end
    end
    
    ---get
    ---@param tbl table
    ---@param idx number
    ---@return any
    table.get =
    function(tbl, idx)
        return tbl[idx]
    end


    ---rawget
    ---@param tbl table
    ---@param idx number
    ---@return any
    table.rawget =
    function(tbl, idx)
        return rawget(tbl, idx)
    end


    ---get_random
    ---@param tbl table
    ---@return any
    table.get_random =
    function(tbl)
        if #tbl == 0 then return nil end
        return tbl[math.random(1, #tbl)]
    end


    table.get_meta_compatible =
    function(tbl,...)
        local tbls = {tbl, ...}
        if #tbls == 1 then tbls = {table.unpack(tbl)} end
        
        local meta = getmetatable(tbls[1])
        for i = 1, #tbls do
            meta = meta or getmetatable(tbls[i])
            local compare_meta = getmetatable(tbls[i]) or meta
            if meta ~= compare_meta then
                print("Ошибка: метатаблицы не совместимы")
                return false
            end
        end
        return meta or getmetatable(tbl)
    end


    table.merge = function(tbl, ...)
        if tbl == nil then return nil end
        -- try to set common metatable
        local meta = table.get_meta_compatible(tbl,...)
        if meta == false then return nil end
        local merge = setmetatable({}, meta)
        --
        local tbls = {tbl, ...}
        if #tbls == 1 then tbls = tbl end
        --
        local k = 1
        for i = 1, #tbls do
            local _tbl = tbls[i]
            if type(_tbl) == "table" then
                table.move(_tbl, 1, #_tbl, k, merge)
                k = k + #_tbl
            elseif tbl then
                merge[k] = tbl
                k = k + 1
            end
        end
        
        return merge
    end

    ---unique
    ---@param tbl table
    ---@return table, dict
    table.unique =
    function(tbl)
        local index_dict = dict()
        local i = 1
        while i <= #tbl do
            local item = tbl[i]
            if not (index_dict:has(item)) then
                index_dict:set(item, i)
                i = i + 1
            else
                table.remove_swap(tbl, i)
            end
        end
        return tbl, index_dict
    end


    ---find_first
    ---@overload fun(tbl:table, item:any):number|nil
    ---@param tbl table
    ---@param item any
    ---@param pos_start|nil
    ---@return number|nil
    table.find_first = table.find_first or
    function(tbl, item, pos_start)
        pos_start = pos_start or 1
        for i =  pos_start, #tbl do
            if tbl[i] == item then
                return i
            end
        end
        return nil
    end

    ---find
    ---@param tbl table
    ---@param item any
    ---@return number[]
    table.find = table.find or
    function(tbl, item)
        local indices  = {}
        for i = 1, #tbl do
            if tbl[i] == item then
                table.insert(indices , i)
            end
        end
        return indices
    end

end
