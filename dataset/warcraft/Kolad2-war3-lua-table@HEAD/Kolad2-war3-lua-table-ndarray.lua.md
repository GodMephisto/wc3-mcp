---
type: vendor-doc
source_repo: "https://github.com/Kolad2/war3-lua-table"
source_path: "ndarray.lua"
ref: HEAD
---

do -- require "table"

    --- генератор нулей
    local empty = math.empty or function()
        return nil
    end

    local function tostring2d(ndarray)
        local _str = "{"
        for i = 1, #ndarray do
            _str = _str .. table.tostring(ndarray[i])
            if i < #ndarray then
                _str = _str .. ",\n "
            else
                _str = _str .. "}"
            end
        end
        return _str
    end

    ---------------- Class NDArray API ----------------------------
    ---@type NDArrayClass
    ---@overload fun(shape:table): NDArray
    NDArray = NDArray or {}

    ---@class NDArray
    ---@field fill fun(self:NDArray, value:any):table
    ---@field tostring fun(self:NDArray):string
    ---@field shape table
    ---@field ndim number
    local object = {}

    ---@class NDArrayClass
    ---@field emptify fun(self:NDArrayClass, ndarray:NDArray):NDArray
    ---@field create fun(self:NDArrayClass, shape:table):NDArray
    local class = {}
    ---------------- object methods ---------------------------------
    ---fill
    ---@param value table
    ---@return NDArray
    function object:fill(value)
        NDArray:fill_nda(self, self.shape, value)
        return self
    end


    function object:tostring()
        if self.ndim == 1 then
            return table.tostring(self)
        end
        if self.ndim == 2 then
            return tostring2d(self)
        end
        if self.ndim == 3 then
            local str = "{\n\n"
            for i = 1, #self do
                str = str .. tostring2d(self[i]) .. ",\n\n"
            end
            return str
        end
    end

    local object_meta = {
        __index = object,
        __tostring = object.tostring
    }

    ---create
    ---@param shape table
    ---@return NDArray
    function class:create(shape)
        local obj = {} ---@type NDArray
        obj.shape = shape
        obj.ndim = #obj.shape
        self:emptify(obj)
        return setmetatable(obj, object_meta)
    end

    ---------------- class methods ------------------------
    ---emptify
    ---@param ndarray NDArray
    ---@return NDArray
    function class:emptify(ndarray)
        local shape = ndarray.shape

        if #shape == n then
            return table.fill(ndarray, empty, shape[n])
        end

        local old_stack = {}
        local new_stack = {}
        table.insert(old_stack, ndarray)
        --
        for n = 1, #shape - 1 do
            for _, _ndarray in ipairs(old_stack) do
                table.fill(_ndarray, table.empty, shape[n])
                for _, item in ipairs(_ndarray) do
                    table.insert(new_stack, item)
                end
            end
            old_stack = new_stack
            new_stack = {}
        end
    end

    function class:fill_nda(array, shape, value, n)
        n = n or 1
        if #shape == n then table.fill(array, value, shape[n]) return end
        for i = 1, shape[n] do
            self:fill_nda(array[i], shape, value, n + 1)
        end
    end


    setmetatable(NDArray, {
        __index = class,
        __call = class.create
    })
end