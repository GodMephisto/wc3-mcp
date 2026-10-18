---
type: vendor-doc
source_repo: "https://github.com/Kolad2/war3-lua-table"
source_path: "dict.lua"
ref: HEAD
---

do
    ---@class dict
    ---@field create fun(self:dict)
    ---@field set fun(self:dict, key:any, item:any):void
    ---@field remove fun(self:dict, key:number|string):void
    ---@field has fun(self:dict, key:number|string):boolean
    ---@field get fun(self:dict, key:number|string):any
    ---@field each fun(self:dict, callback:fun(key:number|string, value:any)): void
    ---@field keys fun(self:dict): table
    ---@field values fun(self:dict): table
    ---@field size fun(self:dict): number
    ---@field clear fun(self:dict): void
    dict = dict or {}


    ---set
    ---@param key any
    ---@param item any
    function dict:set(key, item)
        self.data[key] = item
    end

    ---remove
    ---@param key any
    function dict:remove(key)
        self.data[key] = nil
    end

    ---has
    ---@param key any
    function dict:has(key)
        return (self.data[key] or false) ~= false
    end

    ---get
    ---@param key any
    ---@return any
    function dict:get(key)
        return self.data[key]
    end

    -- Итератор
    ---@param callback fun(key:any, value:any)
    function dict:each(callback)
        for key, value in pairs(self.data) do
            callback(key, value)
        end
    end

    -- Получение ключей
    ---@return table
    function dict:keys()
        local keys = {}
        for key in pairs(self.data) do
            table.insert(keys, key)
        end
        return keys
    end


    -- Получение значений
    ---@return table
    function dict:values()
        local values = {}
        for _, value in pairs(self.data) do
            table.insert(values, value)
        end
        return values
    end


    -- Размер словаря
    ---@return number
    function dict:size()
        local count = 0
        for _ in pairs(self.data) do
            count = count + 1
        end
        return count
    end

    -- Очистка словаря
    function dict:clear()
        for key, _ in pairs(self.data) do
            self.data[key] = nil
        end
    end

    dict.__meta = {
        __index = dict
    }

    ---create
    ---@param tbl table|nil
    ---@return dict
    function dict:create(tbl)
        local obj = {
            data = tbl or {}
        } ---@type dict
        return setmetatable(obj, self.__meta)
    end

    local dict_meta = {
        __call = dict.create
    }
    setmetatable(dict, dict_meta)
end
