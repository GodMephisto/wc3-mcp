-- Exercises every opcode family. Its output must not change when the bytecode is translated back.
local out = {}
local function p(...) local t = table.pack(...) for i = 1, t.n do out[#out + 1] = tostring(t[i]) end end
local a, b, f = 7, 3, 2.5
p(a + b, a - b, a * b, a % b, a ^ b, a / b, a // b, a & b, a | b, a ~ b, a << b, a >> 1, -a, ~a, not a, #"abc")
p(f + 1, f // 1, -7 // 2, -7 % 3, 7.5 % -2, math.maxinteger + 1, 1e300 * 1e10, -0.0, 0.1 + 0.2)
p("x" .. a .. "y" .. f, a == b, a < b, a <= b, a ~= b, "a" < "b")
local s = 0
for i = 1, 10 do s = s + i end
for i = 10, 1, -3 do s = s + i end
for i = 1.0, 2.0, 0.25 do s = s + i end
for i = 1, 0 do s = s + 1000 end
p(s)
local t = {}
for i = 1, 120 do t[i] = i * 2 end
local u = {1, 2, 3, table.unpack(t)}
p(#u, u[60], u[123])
local keys = {}
for k, v in pairs({x = 1, y = 2}) do keys[#keys + 1] = k .. v end
table.sort(keys)
p(table.concat(keys, ","))
for i, v in ipairs({"a", "b"}) do p(i, v) end
local function counter()
  local n = 0
  return function() n = n + 1 return n end
end
local c1, c2 = counter(), counter()
p(c1(), c1(), c2())
local fns = {}
for i = 1, 3 do fns[i] = function() return i end end
p(fns[1](), fns[2](), fns[3]())
local function va(...) return select("#", ...), ... end
p(va(1, nil, 3))
local function tail(n) if n == 0 then return "done" end return tail(n - 1) end
p(tail(1000))
local obj = {v = 5}
function obj:get(k) return self.v + k end
p(obj:get(2))
local x = a > 5 and "big" or "small"
local y = nil or false
local z = (a > 100) and 1
p(x, y, z)
local big = {}
for i = 1, 70 do big[#big + 1] = "k" .. i end
local str = "\0\1\n\t\"\\\255"
p(#str, str:byte(1, -1))
local ok, err = pcall(function() local q = nil return q.x end)
p(ok)
do
  local w = 1
  local function g() w = w + 1 return w end
  g()
  p(w)
end
goto skip
p("not printed")
::skip::
local m = setmetatable({}, {__index = function(_, k) return k .. "!" end, __add = function() return 42 end})
p(m.hi, m + 1)
p(string.format("%5.2f|%d|%s", 3.14159, 42, "z"))
print(table.concat(out, " "))
