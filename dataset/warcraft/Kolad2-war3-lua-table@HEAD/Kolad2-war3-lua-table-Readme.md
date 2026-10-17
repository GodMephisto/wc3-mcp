---
type: vendor-doc
source_repo: "https://github.com/Kolad2/war3-lua-table"
source_path: "Readme.md"
ref: HEAD
---




# Разширенная библитека таблиц table

![](logo/horizontal.svg)

Данная библитека table расширяет возможности текущей версии одноименной библеотеки в Warcraft 3 reforged.
Warcraft 3 reforged 1.36 содержит в lua библитеки table по умолчанию следующие ключи:
```lua
table = {"unpack", "insert", "pack", "sort", "concat", "move", "remove", "unpack"}
```
Функции работают в соответствии с [мануалом](https://www.lua.org/manual/5.4/manual.html#6.6). 
Список всех поддерживаемых функций библитеки репозитория расположен в разделе программного интерфейса - API.

Клонируйте репозиторий в свой проект:
```cmd
git clone https://github.com/Kolad2/war3-lua-table.git table
```

## Программный интерфейс (API) модуля table

```lua
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
```

# Модуль NDArray
## Описание модуля NDArray
Модуль многомерных табиц или массива.

Для инициализации данного модуля необходимо указать в качестве таблицы форму массива ```shape = {5,4,3,...}```.
Такая организация пзволяет релизовывать методы автоматического заполнения всей таблицы числами или через генераторы.
Все мульти таблицы хранятся только в числовых ключах, что решает вопрос десинхронизации.

Альтернативой данному модулю служит [MDTable](https://www.hiveworkshop.com/threads/multidimensional-table.353717/).
MDTable представляет собой класс мульти таблиц lua с синхронизацией pairs для онлайна в warcraft 3.
Однако есть некоторое отличие, в MDTable необходимо явно указать только размерность, что не позволяет
автоматически заполнять таблицу через пользовательский интерфейс. Также ключи могут быть не числовыми.

## Программный интерфейс (API) модуля NDArray

```lua
do
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
end 
```

# Модуль dict
## Описание модуля dict
dict это удобный и расширяемый модуль для работы со таблицами как словарями
на языке Lua. Модуль предоставляет объектно-ориентированный интерфейс,
который инкапсулирует операции над таблицами,
делая код более структурированным, читаемым и поддерживаемым. 
При этом централизованное управление операциями над 
словарем облегчает процесс отладки и отслеживания изменений в данных.

Хотя подобное возможно реализовать in-situ
и напрямую работать с таблицами, абстракция,
предоставляемая библиотекой, способствует более эффективной разработке,
снижает риск ошибок и облегчает сопровождение и масштабирование проектов.

1. Создание словаря:
```lua
local my_dict = dict()
```
2. Добавление или обновление элементов:
```lua
my_dict:set("newKey", "newValue")
```
3. Удаление элементов легко и безопасно удаляет элемент по заданному ключу.:
```lua
my_dict:remove("key1")
```
4. Проверка наличия ключа позволяет проверить, существует ли определённый ключ в словаре.:
```lua
if my_dict:has("key2") then 
  -- действия
end
```
5. Получение значения по ключу:
```lua
local value = my_dict:get("key2")
```
6. Итерация по элементам позволяет удобно перебрать все пары ключ-значение в словаре:
```lua
my_dict:each(function(key, value) print(key, value) end)
```
7. Получение списка ключей или значений:
```lua
local keys = my_dict:keys() 
local values = my_dict:values()
```
8. Определение размера словаря:
```lua
local count = my_dict:size()
```
9. Очистка словаря полностью очищает словарь от всех элементов:
```lua
my_dict:clear()
```

## Программный интерфейс (API) модуля dict

```lua
---@class dict
---@field create fun(self:dict)
---@field set fun(self:dict, key:number|string, item:any):void
---@field remove fun(self:dict, key:number|string):void
---@field has fun(self:dict, key:number|string):boolean
---@field get fun(self:dict, key:number|string):any
---@field each fun(self:dict, callback:fun(key:number|string, value:any)): void
---@field keys fun(self:dict): table
---@field values fun(self:dict): table
---@field size fun(self:dict): number
---@field clear fun(self:dict): void
dict = dict or {}
```

# Модуль set
## Программный интерфейс (API) модуля set

```lua
---@class set
---@field remove fun(self:set, item:any): boolean
---@field insert fun(self:set, item:any): boolean
---@field add fun(self:set, item:any): boolean
---@field has fun(self:set, item:any): boolean
---@field get_random fun(self:set): any
---@field remove_random fun(self:set)
---@field create fun(self:set, tbl:table): set
---@overload fun(tbl:table): set
set = set or {}
```

# Модуль publisher
## Программный интерфейс (API) модуля publisher
```lua
---@class Publisher
---@field subscribe fun(obj: Publisher, subscriber: function)
---@field unsubscribe fun(obj: Publisher, subscriber: function)
---@field publish fun(obj: Publisher,...)
---@overload fun():Publisher
Publisher = Publisher or {}
```


# Модуль table.sorts
## Описание модуля table.sorts
Модуль является адаптацией [репозитория](https://github.com/DervexDev/LuaSortingAlgorithms)
конвертированных из python алгоритмов сортировки. 

Следующие алгоритмы поддерживают дробные числа:
* Cycle Sort
* Gnome Sort
* Circle Sort
* Cocktail Sort
* Comb Sort
* Bubble Sort
* ShellSort
* Stooge Sort
* Heap Sort
* Insertion Sort
* Merge Sort
* Pancake Sort
* Quick Sort
* Bucket Sort - not recommended

Следующие алгоритмы поддерживают только целые числа:

* Pigeonhole Sort
## Текущий программный интерфейс (API) модуля table.sorts

```lua
---@field stooge fun(array:table):table
---@field shell fun(array:table):table
---@field pigeonhole fun(array:table):table -- integer only
---@field pancake fun(array:table):table
---@field merge fun(array:table):table
---@field heap fun(array:table):table
---@field gnome fun(array:table):table
---@field cycle fun(array:table):table
---@field comb fun(array:table):table
---@field cocktail fun(array:table):table
---@field circle fun(array:table):table
---@field insertion fun(array:table):table
---@field quick fun(array:table):table
---@field bucket fun(array:table):table
---@field bubble fun(array:table):table
---table.sorts = table.sorts or {}
```

# Использованные источники
- [lua-table](https://github.com/Luca96/lua-table/tree/master) - 
база функций расширяющих стандартный набор table c добавлением дополнительных свойств,
на момент использования исходников распространялся по лицензии MIT, включен в текст лицензии.
- [t-util](https://github.com/loominatrx/t-util/tree/main) - 
библитека функций расширяющих стандартный набор,
в данном репозитории использовались идеи из этого репозитория.
- [table-manual](https://www.lua.org/manual/5.4/manual.html#6.6) - 
официальный мануал по таблицам.
- [LuaSortingAlgorithms](https://github.com/DervexDev/LuaSortingAlgorithms) - 
репозиторий конвертированных из python алгоритмов сортировки, 
на момент написания распространялся без лицензии, функции были адаптированны для warcraft 3 reforged.
- [lua-set](https://github.com/wscherphof/lua-set/tree/master) -
  релизация множества для lua

# Полезные источники
- [[1]](https://github.com/nazarpunk/cheapack) -
cheapack - совсем не дешевый упаковщик *.lua файлов в *.wct файл Warcraft 3.
- [[2]](https://github.com/Indaxia/imp) IMP пакетный менеджер lua.
- [[3]](https://github.com/Indaxia/imp-lua-mm)
imp-lua-mm менеджер модулей (imp mm).
- [[4]](https://github.com/torch/xlua/tree/master)
xlua это база функций расширяющих стандартный набор.
- [[5]](https://www.hiveworkshop.com/threads/multidimensional-table.353717/)
MDTable класс мульти таблиц lua с синхронизацией pairs для онлайна в warcraft 3.
- [[6]](https://hpaulkeeler.com/simulating-poisson-random-variables-survey-methods/) 
статья про то какие бывают алгоритмы пуассоновского распределения и какие используются в какой мат. системе.
- [luaforwindows](https://github.com/rjpcomputing/luaforwindows) -
репозиторий хранящий Lua.exe файл для windows.