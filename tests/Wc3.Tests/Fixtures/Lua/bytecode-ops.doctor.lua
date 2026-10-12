-- ops: translated from Lua 5.3 bytecode (12 functions)
local __mt, __tonumber, __floor, __ceil, __maxi, __mini, __error, __select =
  math.type, tonumber, math.floor, math.ceil, math.maxinteger, math.mininteger, error, select
local function __toint(v, mode)
  -- luaV_tointeger: an integer, or a float/string with an exact integer value (mode 1: floor, 2: ceil)
  if __mt(v) == 'integer' then return v end
  local n = __tonumber(v)
  if n == nil then return nil end
  if __mt(n) == 'integer' then return n end
  if n ~= n then return nil end
  local f = (mode == 2) and __ceil(n) or __floor(n)
  if f ~= f or f >= 2^63 or f < -2^63 then return nil end
  return __mt(f) == 'integer' and f or math.tointeger(f)
end
local function __forprep(init, limit, step)
  if __mt(init) == 'integer' and __mt(step) == 'integer' then
    local stop = false
    local il = __toint(limit, step < 0 and 2 or 1)
    if il == nil then
      local n = __tonumber(limit)
      if n ~= nil then
        if 0 < n then il = __maxi; if step < 0 then stop = true end
        else il = __mini; if step >= 0 then stop = true end end
      end
    end
    if il ~= nil then
      if stop then init = 0 end
      return init - step, il, step
    end
  end
  local nl = __tonumber(limit)
  if nl == nil then __error("'for' limit must be a number", 2) end
  local ns = __tonumber(step)
  if ns == nil then __error("'for' step must be a number", 2) end
  local ni = __tonumber(init)
  if ni == nil then __error("'for' initial value must be a number", 2) end
  return (ni + 0.0) - (ns + 0.0), nl + 0.0, ns + 0.0
end
local function __setlist(t, base, ...)
  for i = 1, __select('#', ...) do t[base + i] = (__select(i, ...)) end
end
local F = {}

F[0] = function(U) return function(...)
local r1, r2, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r13, r15, r17, r18, r19, r20, r21, r22, r24, r25, r26, r27, r28, r29
local b0 = {}
local b14 = {}
local b16 = {}
local b23 = {}
b0[1] = {}
r1 = F[1]({U[1], b0})
r2 = 7
r3 = 3
r4 = 2.5
r5 = r1
r6 = r2 + r3
r7 = r2 - r3
r8 = r2 * r3
r9 = r2 % r3
r10 = r2 ^ r3
r11 = r2 / r3
r12 = r2 // r3
r13 = r2 & r3
b14[1] = r2 | r3
r15 = r2 ~ r3
b16[1] = r2 << r3
r17 = r2 >> 1
r18 = -r2
r19 = ~r2
r20 = not r2
r21 = "abc"
r21 = #r21
r5(r6, r7, r8, r9, r10, r11, r12, r13, b14[1], r15, b16[1], r17, r18, r19, r20, r21)
r5 = r1
r6 = r4 + 1
r7 = r4 // 1
r8 = (-4)
r9 = 2
r10 = (-0.5)
r11 = U[1][1]["math"]
r11 = r11["maxinteger"]
r11 = r11 + 1
r12 = (1/0)
r13 = 0.0
r13 = -r13
b14[1] = 0.30000000000000004
r5(r6, r7, r8, r9, r10, r11, r12, r13, b14[1])
r5 = r1
r6 = "x"
r7 = r2
r8 = "y"
r9 = r4
r6 = r6 .. r7 .. r8 .. r9
if not (r2 == r3) then goto L46 end
goto L47
::L46::
r7 = false goto L48
::L47::
r7 = true
::L48::
if not (r2 < r3) then goto L50 end
goto L51
::L50::
r8 = false goto L52
::L51::
r8 = true
::L52::
if not (r2 <= r3) then goto L54 end
goto L55
::L54::
r9 = false goto L56
::L55::
r9 = true
::L56::
if r2 == r3 then goto L58 end
goto L59
::L58::
r10 = false goto L60
::L59::
r10 = true
::L60::
if not ("a" < "b") then goto L62 end
goto L63
::L62::
r11 = false goto L64
::L63::
r11 = true
::L64::
r5(r6, r7, r8, r9, r10, r11)
r5 = 0
r6 = 1
r7 = 10
r8 = 1
r6, r7, r8 = __forprep(r6, r7, r8) goto L71
::L70::
r5 = r5 + r9
::L71::
r6 = r6 + r8 if (0 < r8 and r6 <= r7) or (not (0 < r8) and r7 <= r6) then r9 = r6 goto L70 end
r6 = 10
r7 = 1
r8 = (-3)
r6, r7, r8 = __forprep(r6, r7, r8) goto L77
::L76::
r5 = r5 + r9
::L77::
r6 = r6 + r8 if (0 < r8 and r6 <= r7) or (not (0 < r8) and r7 <= r6) then r9 = r6 goto L76 end
r6 = 1.0
r7 = 2.0
r8 = 0.25
r6, r7, r8 = __forprep(r6, r7, r8) goto L83
::L82::
r5 = r5 + r9
::L83::
r6 = r6 + r8 if (0 < r8 and r6 <= r7) or (not (0 < r8) and r7 <= r6) then r9 = r6 goto L82 end
r6 = 1
r7 = 0
r8 = 1
r6, r7, r8 = __forprep(r6, r7, r8) goto L89
::L88::
r5 = r5 + 1000
::L89::
r6 = r6 + r8 if (0 < r8 and r6 <= r7) or (not (0 < r8) and r7 <= r6) then r9 = r6 goto L88 end
r6 = r1
r7 = r5
r6(r7)
r6 = {}
r7 = 1
r8 = 120
r9 = 1
r7, r8, r9 = __forprep(r7, r8, r9) goto L100
::L98::
r11 = r10 * 2
r6[r10] = r11
::L100::
r7 = r7 + r9 if (0 < r9 and r7 <= r8) or (not (0 < r9) and r8 <= r7) then r10 = r7 goto L98 end
r7 = {}
r8 = 1
r9 = 2
r10 = 3
r11 = U[1][1]["table"]
r11 = r11["unpack"]
r12 = r6
__setlist(r7, 0, r8, r9, r10, r11(r12))
r8 = r1
r9 = #r7
r10 = r7[60]
r11 = r7[123]
r8(r9, r10, r11)
r8 = {}
r9 = U[1][1]["pairs"]
r10 = {}
r10["x"] = 1
r10["y"] = 2
r9, r10, r11 = r9(r10)
goto L128
::L122::
b14[1] = #r8
b14[1] = b14[1] + 1
r15 = r12
b16[1] = r13
r15 = r15 .. b16[1]
r8[b14[1]] = r15
::L128::
r12, r13 = r9(r10, r11)
if r12 ~= nil then r11 = r12 goto L122 end
r9 = U[1][1]["table"]
r9 = r9["sort"]
r10 = r8
r9(r10)
r9 = r1
r10 = U[1][1]["table"]
r10 = r10["concat"]
r11 = r8
r12 = ","
r9(r10(r11, r12))
r9 = U[1][1]["ipairs"]
r10 = {}
r11 = "a"
r12 = "b"
r10[1] = r11 r10[2] = r12
r9, r10, r11 = r9(r10)
goto L152
::L148::
b14[1] = r1
r15 = r12
b16[1] = r13
b14[1](r15, b16[1])
::L152::
r12, r13 = r9(r10, r11)
if r12 ~= nil then r11 = r12 goto L148 end
r9 = F[2]({})
r10 = r9
r10 = r10()
r11 = r9
r11 = r11()
r12 = r1
r13 = r10
r13 = r13()
b14[1] = r10
b14[1] = b14[1]()
r15 = r11
r12(r13, b14[1], r15())
r12 = {}
r13 = 1
b14[1] = 3
r15 = 1
r13, b14[1], r15 = __forprep(r13, b14[1], r15) goto L175
::L172::
r17 = F[4]({b16})
r12[b16[1]] = r17
b16 = {b16[1]} b23 = {b23[1]} goto L175
::L175::
r13 = r13 + r15 if (0 < r15 and r13 <= b14[1]) or (not (0 < r15) and b14[1] <= r13) then b16[1] = r13 goto L172 end
r13 = r1
b14[1] = r12[1]
b14[1] = b14[1]()
r15 = r12[2]
r15 = r15()
b16[1] = r12[3]
r13(b14[1], r15, b16[1]())
r13 = F[5]({U[1]})
b14[1] = r1
r15 = r13
b16[1] = 1
r17 = nil
r18 = 3
b14[1](r15(b16[1], r17, r18))
b14[1] = F[6]({b14})
r15 = r1
b16[1] = b14[1]
r17 = 1000
r15(b16[1](r17))
r15 = {}
r15["v"] = 5
b16[1] = F[7]({})
r15["get"] = b16[1]
b16[1] = r1
r18 = r15 r17 = r18["get"]
r19 = 2
b16[1](r17(r18, r19))
if 5 < r2 then goto L209 end
goto L212
::L209::
b16[1] = "big"
if not b16[1] then goto L212 end
goto L213
::L212::
b16[1] = "small"
::L213::
r17 = false
if 100 < r2 then goto L216 end
goto L218
::L216::
r18 = 1
goto L220
::L218::
r18 = false goto L220
r18 = true
::L220::
r19 = r1
r20 = b16[1]
r21 = r17
r22 = r18
r19(r20, r21, r22)
r19 = {}
r20 = 1
r21 = 70
r22 = 1
r20, r21, r22 = __forprep(r20, r21, r22) goto L236
::L230::
r24 = #r19
r24 = r24 + 1
r25 = "k"
r26 = b23[1]
r25 = r25 .. r26
r19[r24] = r25
::L236::
r20 = r20 + r22 if (0 < r22 and r20 <= r21) or (not (0 < r22) and r21 <= r20) then b23[1] = r20 goto L230 end
r20 = "\000\001\n\009\"\\\255"
r21 = r1
r22 = #r20
r24 = r20 b23[1] = r24["byte"]
r25 = 1
r26 = (-1)
r21(r22, b23[1](r24, r25, r26))
r21 = U[1][1]["pcall"]
r22 = F[8]({})
r21, r22 = r21(r22)
b23[1] = r1
r24 = r21
b23[1](r24)
b23[1] = 1
r24 = F[9]({b23})
r25 = r24
r25()
r25 = r1
r26 = b23[1]
r25(r26)
b23 = {b23[1]} goto L263
goto L263
b23[1] = r1
r24 = "not printed"
b23[1](r24)
::L263::
b23[1] = U[1][1]["setmetatable"]
r24 = {}
r25 = {}
r26 = F[10]({})
r25["__index"] = r26
r26 = F[11]({})
r25["__add"] = r26
b23[1] = b23[1](r24, r25)
r24 = r1
r25 = b23[1]["hi"]
r26 = b23[1] + 1
r24(r25, r26)
r24 = r1
r25 = U[1][1]["string"]
r25 = r25["format"]
r26 = "%5.2f|%d|%s"
r27 = 3.14159
r28 = 42
r29 = "z"
r24(r25(r26, r27, r28, r29))
r24 = U[1][1]["print"]
r25 = U[1][1]["table"]
r25 = r25["concat"]
r26 = b0[1]
r27 = " "
r24(r25(r26, r27))
do return end
end end
F[1] = function(U) return function(...)
local r0, r1, r2, r3, r4, r5, r6, r7
r0 = U[1][1]["table"]
r0 = r0["pack"]
r0 = r0(...)
r1 = 1
r2 = r0["n"]
r3 = 1
r1, r2, r3 = __forprep(r1, r2, r3) goto L15
::L8::
r5 = U[2][1]
r5 = #r5
r5 = r5 + 1
r6 = U[1][1]["tostring"]
r7 = r0[r4]
r6 = r6(r7)
U[2][1][r5] = r6
::L15::
r1 = r1 + r3 if (0 < r3 and r1 <= r2) or (not (0 < r3) and r2 <= r1) then r4 = r1 goto L8 end
do return end
end end
F[2] = function(U) return function()
local r1
local b0 = {}
b0[1] = 0
r1 = F[3]({b0})
do return r1 end
do return end
end end
F[3] = function(U) return function()
local r0, r1
r0 = U[1][1]
r0 = r0 + 1
U[1][1] = r0
r0 = U[1][1]
do return r0 end
do return end
end end
F[4] = function(U) return function()
local r0, r1
r0 = U[1][1]
do return r0 end
do return end
end end
F[5] = function(U) return function(...)
local r0, r1, r2
r0 = U[1][1]["select"]
r1 = "#"
r0 = r0(r1, ...)
do return r0, ... end
do return end
end end
F[6] = function(U) return function(r0)
local r1, r2
if r0 == 0 then goto L2 end
goto L4
::L2::
r1 = "done"
do return r1 end
::L4::
r1 = U[1][1]
r2 = r0 - 1
do return r1(r2) end
do return end
end end
F[7] = function(U) return function(r0, r1)
local r2
r2 = r0["v"]
r2 = r2 + r1
do return r2 end
do return end
end end
F[8] = function(U) return function()
local r0, r1
r0 = nil
r1 = r0["x"]
do return r1 end
do return end
end end
F[9] = function(U) return function()
local r0, r1
r0 = U[1][1]
r0 = r0 + 1
U[1][1] = r0
r0 = U[1][1]
do return r0 end
do return end
end end
F[10] = function(U) return function(r0, r1)
local r2, r3
r2 = r1
r3 = "!"
r2 = r2 .. r3
do return r2 end
do return end
end end
F[11] = function(U) return function()
local r0, r1
r0 = 42
do return r0 end
do return end
end end
return function(ENV, ...) return F[0]({{ENV}})(...) end
