---
type: vendor-doc
source_repo: "https://github.com/Kolad2/war3-lua-table"
source_path: "observer.lua"
ref: HEAD
---

do
    table = table or {}
    table.tools = table.tools or {}

    ---@class Observer
    ---@field subscribe fun(obj: Observer, subscriber: function)
    ---@field unsubscribe fun(obj: Observer, subscriber: function)
    ---@field publish fun(obj: Observer,...)
    ---@field notify fun(obj: Observer)
    ---@overload fun():Observer
    table.tools.Observer = table.tools.Observer or {}
    

    local Observer = table.tools.Observer or {} ---@type Observer
    -- метатаблица для экземпляров
    Observer.__meta = {}
    Observer.__meta.__index = Observer

    ------------------ статические методы -----------------------

    ---@param cls Observer
    function Observer.init(cls)
        cls.publish_callback = function(sub,...) sub(...) end
        cls.notify_callback  = function(sub) sub() end
        return cls
    end

    ---@param cls Observer
    function Observer.__create(cls)
        local obj = setmetatable({}, cls.__meta)
        obj.subscribers = set()
        return obj
    end

    ---@param cls Observer
    function Observer.create(cls)
        cls.create = cls.__create
        cls:init()
        return cls.__create(cls)
    end

    ------------------ методы класса -----------------------
    -- подписка на наблюдателя
    ---@param subscriber function
    function Observer:subscribe(subscriber)
        return self.subscribers:insert(subscriber)
    end

    -- отписка от наблюдателя
    ---@param subscriber function
    function Observer:unsubscribe(subscriber)
        self.subscribers:remove(subscriber)
    end

    -- публикация подписчикам
    function Observer:publish(...)
        self.subscribers:each(self.publish_callback,...)
    end
    
    -- раздача копий подписчикам
    ---@param obj Observer
    Observer.distribute = function(obj,...)
        local data = {...}
        obj.subscribers:each(
            function(sub, data)
                local data_copy = table.deepcopy(data)                
                sub(table.unpack(data_copy)) 
            end,
            data
        )
    end
    
    -- уведомление подписчиков
    function Observer:notify()
        self.subscribers:each(self.notify_callback)
    end

    -- установка метатаблицы для класса
    setmetatable(Observer,{
                __call = Observer.create
            })

end
