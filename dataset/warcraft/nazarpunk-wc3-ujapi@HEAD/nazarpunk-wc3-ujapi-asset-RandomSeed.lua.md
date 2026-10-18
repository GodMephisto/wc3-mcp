---
type: vendor-doc
source_repo: "https://github.com/nazarpunk/wc3-ujapi"
source_path: "asset/RandomSeed.lua"
ref: HEAD
---

---@class RandomSeed
RandomSeed = {};
RandomSeed.__index = RandomSeed;

---@param seed number
---@return RandomSeed
function RandomSeed:new(seed)
    local self = setmetatable({}, RandomSeed)
    -- 1103515245, 12345, 0x10000 from Ansi C
    -- 1664525, 1013904223, 0x10000 from Numerical Recipes
    -- 214013, 2531011, 0x10000 from MVC
    self.a = 1103515245;
    self.c = 12345;
    self.m = 0x10000;
    self.ic = self.c;
    self.x = seed;
    return self;
end

---@return number
function RandomSeed:uniform()
    local t = self.a * self.x + self.c;
    self.x = t % self.m;
    self.c = math.floor(t / self.m);
    return self.x / 0x10000;
end

---@param min number
---@param max number
---@return number
function RandomSeed:uniformInt(min, max)
    return math.floor(self.uniform() * (max - min + 1)) + min;
end

---@param min number
---@param max number
---@return number
function RandomSeed:uniformReal(min, max)
    return self.uniform() * (max - min) + min;
end

---@param factor number
---@return number
function RandomSeed:normal(factor)
    local rand = 0;
    for _ = 1, factor do
        rand = rand + self.uniform();
    end
    return rand / factor;
end

---@param min number
---@param max number
---@param factor number
---@return number
function RandomSeed:normalInt(min, max, factor)
    return math.floor(self.normal(factor) * (max - min + 1)) + min;
end

---@param min number
---@param max number
---@param factor number
---@return number
function RandomSeed:normalReal(min, max, factor)
    return self.normal(factor) * (max - min) + min
end