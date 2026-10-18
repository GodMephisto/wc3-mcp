---
type: vendor-doc
source_repo: "https://github.com/Kolad2/war3-lua-table"
source_path: "stats/exponential.lua"
ref: HEAD
---

do
    stats = stats or {}

    ---@class DistributionExponential
    stats.exp = {}
    local exp = stats.exp
    exp.__index = exp

    local function cdf(x, lambda) return 1 - math.exp( - x / lambda) end
    local function pdf(x, lambda) return math.exp(- x / lambda) / lambda  end

    ---@param cls DistributionExponential
    ---@param lambda number
    ---@param min number|nil
    ---@param max number|nil
    ---@return DistributionExponential
    function exp.create(cls, lambda, min, max)
        local obj = setmetatable({}, cls)
        obj.min = min
        obj.max = max
        obj.cdf_min = 0
        obj.cdf_max = 1
        obj.lambda = lambda
        if min then obj.cdf_min = cdf(min, lambda) end
        if max then obj.cdf_max = cdf(max, lambda) end
        obj.norm = obj.cdf_max - obj.cdf_min
        return obj
    end

    function exp:rvs()
        local v = 1 - (math.random() * self.norm + self.cdf_min)
        v = - self.lambda * math.log(v)
        return v
    end

    function exp:cdf(x)
        if x < self.min then return 0 end
        if x > self.max then return 1 end
        return (cdf(x, self.lambda) - self.cdf_min)/self.norm
    end

    function exp:pdf(x)
        if x < self.min then return 0 end
        if x > self.max then return 0 end
        return pdf(x, self.lambda)/self.norm
    end

    setmetatable(exp, {__call = exp.create})
end