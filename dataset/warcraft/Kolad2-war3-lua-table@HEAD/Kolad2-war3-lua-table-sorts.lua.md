---
type: vendor-doc
source_repo: "https://github.com/Kolad2/war3-lua-table"
source_path: "sorts.lua"
ref: HEAD
---

do
    table = table or {}
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
    table.sorts = table.sorts or {}
    local sorts = table.sorts

    -- gravity -- полезная сортировка

    -- excluded
    --[[bogo, radix]]--

    -- not stable
    -- [[bucket]] --


    ---bubble
    ---@param array table
    function sorts.bubble(array)
        for i = 1, #array do
            for j = 1, #array - i do
                if array[j] > array[j + 1] then
                    array[j], array[j + 1] = array[j + 1], array[j]
                end
            end
        end
        return array
    end

    ---bucket
    ---@param array table
    ---@param slots number|nil
    function sorts.bucket(array, slots)
        local buckets = {}
        slots = 10 or slots

        -- Создаем пустые корзины
        for _ = 1, slots do
            table.insert(buckets, {})
        end

        -- Находим максимальное значение, чтобы правильно распределить элементы по корзинам
        local maxValue = table.max(array)

        -- Распределяем элементы по корзинам
        for _, value in ipairs(array) do
            local index = math.ceil((value / maxValue) * (slots - 1)) + 1
            table.insert(buckets[index], value)
        end

        -- Сортируем каждую корзину
        for i = 1, slots do
            buckets[i] = table.sorts.insertion(buckets[i])
        end

        -- Собираем обратно элементы из корзин
        local k = 1
        for i = 1, slots do
            for j = 1, #buckets[i] do
                array[k] = buckets[i][j]
                k = k + 1
            end
        end

        return array
    end

    ---quick
    ---@param array table
    ---@return table
    function sorts.quick(array)
        local function partition(low, high)
            local pivot = array[high]
            local i = low - 1
            for j = low, high - 1 do
                if array[j] <= pivot then
                    i = i + 1
                    array[i], array[j] = array[j], array[i]
                end
            end
            array[i + 1], array[high] = array[high], array[i + 1]
            return i + 1
        end
        local function sort(low, high)
            if low < high then
                local pivot = partition(low, high)
                sort(low, pivot - 1)
                sort(pivot + 1, high)
            end
            return array
        end
        return sort(1, #array)
    end

    ---insertion
    ---@param array table
    ---@return table
    function sorts.insertion(array)
        for i = 2, #array do
            local key = array[i]
            local j = i - 1
            while j > 0 and key < array[j] do
                array[j + 1] = array[j]
                j = j - 1
            end
            array[j + 1] = key
        end
        return array
    end

    ---circle
    ---@param array table
    ---@return table
    function sorts.circle(array)
        local function inner_circle(list, low, high, swaps)
            if low >= high then
                return swaps
            end
            local sub_high, sub_low = high, low
            --[[цикл, начинает с условно граничных значений,
            постепенно сужает границу и переставляет значения на границе
            --]]
            while low < high do
                if list[low] > list[high] then
                    list[low], list[high] = list[high], list[low]
                    swaps = swaps + 1
                end
                low = low + 1
                high = high - 1
            end
            --разделение массива на два подмассива
            swaps = inner_circle(list, sub_low, high, swaps)
            swaps = inner_circle(list, low, sub_high, swaps)
            return swaps
        end

        while inner_circle(array, 1, #array, 0) > 0 do end
        return array
    end

    ---cocktail
    ---@param array table
    ---@return table
    function sorts.cocktail(array)
        local swapped

        repeat
            swapped = false

            for i = 1, #array - 1 do
                if array[i] > array[i + 1] then
                    array[i], array[i + 1] = array[i + 1], array[i]
                    swapped = true
                end
            end

            if not swapped then
                break
            end

            for i = #array - 1, 1, -1 do
                if array[i] > array[i + 1] then
                    array[i], array[i + 1] = array[i + 1], array[i]
                    swapped = true
                end
            end
        until swapped == false

        return array
    end

    ---comb
    ---@param array table
    ---@return table
    function sorts.comb(array)
        local function get_next_gap(gap)
            gap = math.floor((gap * 10) / 13)
            if gap < 1 then
                return 1
            end
            return gap
        end
        local gap = #array
        local swapped = true
        while gap ~= 1 or swapped do
            gap = get_next_gap(gap)
            swapped = false
            for i = 1, #array - gap do
                if array[i] > array[i + gap] then
                    array[i], array[i + gap] = array[i + gap], array[i]
                    swapped = true
                end
            end
        end
        return array
    end

    ---cycle
    ---@param array table
    ---@return table
    function sorts.cycle(array)
        local start = 0

        while start < #array - 1 do
            local value = array[start + 1]

            local position = start
            local i = start + 1

            while i < #array do
                if array[i + 1] < value then
                    position = position + 1
                end

                i = i + 1
            end

            if position ~= start then
                while value == array[position + 1] do
                    position = position + 1
                end

                array[position + 1], value = value, array[position + 1]

                while position ~= start do
                    position = start
                    i = start + 1

                    while i < #array do
                        if array[i + 1] < value then
                            position = position + 1
                        end

                        i = i + 1
                    end

                    while value == array[position + 1] do
                        position = position + 1
                    end

                    array[position + 1], value = value, array[position + 1]
                end
            end

            start = start + 1
        end

        return array
    end

    ---gnome
    ---@param array table
    ---@return table
    function sorts.gnome(array)
        local i, j = 2, 3

        while i <= #array do
            if array[i-1] <= array[i] then
                i = j
                j = j + 1
            else
                array[i - 1], array[i] = array[i], array[i - 1]
                i = i - 1

                if i == 1 then
                    i = j
                    j = j + 1
                end
            end
        end

        return array
    end

    ---heap
    ---@param array table
    ---@return table
    function sorts.heap(array)
        local function heapify(size, i)
            local left = 2 * i
            local right = 2 * i + 1
            local largest
            if left <= size and array[left] > array[i] then
                largest = left
            else
                largest = i
            end
            if right <= size and array[right] > array[largest] then
                largest = right
            end
            if largest ~= i then
                array[i], array[largest] = array[largest], array[i]
                heapify(size, largest)
            end
        end
        local size = #array
        for i = math.floor(size / 2), 1, -1 do
            heapify(size, i)
        end
        for i = #array, 2, -1 do
            array[i], array[1] = array[1], array[i]
            size = size - 1
            heapify(size, 1)
        end
        return array
    end

    ---merge
    ---@param array table
    ---@return table
    function sorts.merge(array)
        local function mergeSort(low, mid, high)
            local n1 = mid - low + 1
            local n2 = high - mid
            local left = {}
            local right = {}

            for i = 1, n1 do
                left[i] = array[low + i - 1]
            end
            left[n1 + 1] = math.huge

            for j = 1, n2 do
                right[j] = array[mid + j]
            end
            right[n2 + 1] = math.huge

            local i = 1
            local j = 1

            for k = low, high do
                if left[i] <= right[j] then
                    array[k] = left[i]
                    i = i + 1
                else
                    array[k] = right[j]
                    j = j + 1
                end
            end
        end

        local function sort(low, high)
            if low < high then
                local mid = math.floor((low + high) / 2)
                sort(low, mid)
                sort(mid + 1, high)
                mergeSort(low, mid, high)
            end

            return array
        end

        return sort(1, #array)
    end

    ---pancake
    ---@param array table
    ---@return table
    function sorts.pancake(array)

        local function flip(array, i)
            local start = 1
            while start < i do
                array[i], array[start] = array[start], array[i]
                start = start + 1
                i = i - 1
            end
        end

        local function find_argmax(array, n)
            local max = 1

            for i = 1, n do
                if array[i] > array[max] then
                    max = i
                end
            end
            return max
        end

        local size = #array

        while size > 1 do
            local max = find_argmax(array, size)

            if max ~= size then
                flip(array, max)
                flip(array, size)
            end

            size = size - 1
        end

        return array
    end

    ---pigeonhole
    ---@param array table
    ---@return table
    function sorts.pigeonhole(array)
        local holes = {}

        for i = 1, #array do
            holes[i] = 0
        end

        for v,_ in ipairs(array) do
            holes[v] = holes[v] + 1
        end

        local i = 1
        for count = 1, #array do
            while holes[count] > 0 do
                holes[count] = holes[count] - 1
                array[i] = count
                i = i + 1
            end
        end
        return array
    end

    ---selection
    ---@param array table
    ---@return table
    function sorts.selection(array)
        for i = 1, #array do
            local min = i

            for j = i + 1, #array do
                if array[min] > array[j] then
                    min = j
                end
            end

            array[i], array[min] = array[min], array[i]
        end

        return array
    end

    ---shell
    ---@param array table
    ---@return table
    function sorts.shell(array)
        local gap = math.floor(#array / 2)

        while gap > 0 do
            local j = gap

            while j < #array do
                local i = j - gap

                while i >= 0 do
                    if array[i + gap + 1] > array[i + 1] then
                        break
                    else
                        array[i + gap + 1], array[i + 1] = array[i + 1], array[i + gap + 1]
                    end

                    i = i - gap
                end

                j = j + 1
            end

            gap = math.floor(gap / 2)
        end

        return array
    end

    ---stooge
    ---@param array table
    ---@return table
    function sorts.stooge(array)
        local function sort(low, high)
            if low >= high then
                return
            end

            if array[low] > array[high] then
                array[low], array[high] = array[high], array[low]
            end

            if high - low > 1 then
                local part = math.floor((high - low + 1) / 3)
                sort(low, high - part)
                sort(low + part, high)
                sort(low, high - part)
            end

            return array
        end

        return sort(1, #array)
    end
end