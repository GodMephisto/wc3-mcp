---
type: vendor-doc
source_repo: "https://github.com/w4454962/jass-parser-luajit"
source_path: "src/main.lua"
ref: HEAD
---

package.path = package.path .. ';src\\?.lua;src\\?\\init.lua;'

function math.tointeger(num)
    return math.floor(num)
end 

function io.load(path)
    local file = io.open(path, "rb")
    if file == nil then 
        return 
    end 

    local str = file:read("*all")
    file:close()

    return str 
end 


function io.save(path, str)
    local file = io.open(path, "wb")
    if file then 
        file:write(str)
        file:close()
    end 
end 


function io.append(path, str)
    local file = io.open(path, "a+")
    if file then 
        file:write(str)
        file:close()
    end 
end 

local parser = require 'parser'

local dump = require 'dump'

local lpeglabel = require 'lpeglabel'

local common = io.load("war3\\24\\common.j")

local blizzard = io.load("war3\\24\\blizzard.j")
local war3map = io.load("war3\\24\\war3map.j")



local option = { }

local ast 

lpeglabel.setmaxstack(8192)

local clock = os.clock()

--语法解析
ast = parser.parser(common, 'common.j',   option)

if #option.errors == 0 then 
    print('common.j pass')
    ast = parser.parser(blizzard, 'blizzard.j', option)
end 

if #option.errors == 0 then 
    print('blizzard.j pass')
    ast = parser.parser(war3map, 'war3map.j', option)
end 


if #option.errors == 0 then 
    print('war3map.j pass')
end 

print(os.clock() - clock)


for k, v in ipairs(option.errors) do 
    print(v.level, v.file, v.line, v.pos, v.code)
end 

