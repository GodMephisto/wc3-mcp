---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/emit_test.go"
ref: HEAD
---

package jass2lua

import (
	"strings"
	"testing"
)

// TestEmit_Ternary — `cond ? a : b` lowers to Lua's `(cond and a) or b`.
func TestEmit_Ternary(t *testing.T) {
	src := `function F takes nothing returns integer
return x ? 1 : 2
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "(x) and (1)") || !strings.Contains(got, "or (2)") {
		t.Errorf("expected `(x) and (1) or (2)` ternary lowering, got:\n%s", got)
	}
}

// TestEmit_TernaryFalseCaveat — if the then-arm looks boolean, the emitter
// appends an inline caveat comment about the false/nil unreliability.
func TestEmit_TernaryBooleanThenUsesTableForm(t *testing.T) {
	// `x > 0 ? a > 0 : a < 0` — then-arm is a comparison ⇒ caveat expected.
	src := `function F takes nothing returns boolean
return x > 0 ? a > 0 : a < 0
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "((x > 0) and {a > 0} or {a < 0})[1]") {
		t.Errorf("expected table-wrap ternary for boolean-then, got:\n%s", got)
	}
	// The old buggy caveat comment must NOT appear -- the form is now correct.
	if strings.Contains(got, "ternary:") {
		t.Errorf("table-wrap form should not emit the false/nil caveat, got:\n%s", got)
	}
}

// TestEmit_BitwiseOr — `a | b` emits as Lua 5.3+ `a | b` (pass-through).
func TestEmit_BitwiseOr(t *testing.T) {
	src := `function F takes nothing returns integer
return a | b
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, " | ") {
		t.Errorf("expected `|` operator, got:\n%s", got)
	}
}

// TestEmit_BitwiseAnd — `a & b` emits as Lua 5.3+ `a & b`.
func TestEmit_BitwiseAnd(t *testing.T) {
	src := `function F takes nothing returns integer
return a & b
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, " & ") {
		t.Errorf("expected `&` operator, got:\n%s", got)
	}
}

// TestEmit_BitwiseXor — JASS `^` (vJASS extension) → Lua `~` for binary
// position (Lua uses `^` for exponentiation, not XOR).
func TestEmit_BitwiseXor(t *testing.T) {
	src := `function F takes nothing returns integer
return a ^ b
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, " ~ ") {
		t.Errorf("expected `~` (Lua bitwise xor), got:\n%s", got)
	}
}

// TestEmit_BitwiseShift — `<<` / `>>` emit as Lua 5.3+ pass-through.
func TestEmit_BitwiseShift(t *testing.T) {
	src := `function F takes nothing returns integer
return (x << 2) + (y >> 1)
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "<<") || !strings.Contains(got, ">>") {
		t.Errorf("expected shift operators, got:\n%s", got)
	}
}

// TestEmit_UnaryBitNot — `~x` emits as Lua `~x`.
func TestEmit_UnaryBitNot(t *testing.T) {
	src := `function F takes nothing returns integer
return ~x
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "~x") {
		t.Errorf("expected unary `~x`, got:\n%s", got)
	}
}

// TestEmit_NULSourceStillTranspiles — confirms that a NUL-bearing source
// flows through the transpile pipeline without erroring.
func TestEmit_NULSourceStillTranspiles(t *testing.T) {
	src := "function Foo \x00 takes nothing returns nothing\n\x00\nendfunction\n"
	lua, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile must not error on NUL source: %v", err)
	}
	if !strings.Contains(lua, "function Foo()") {
		t.Errorf("expected Foo() in output, got:\n%s", lua)
	}
}

// ---------------------------------------------------------------------------
// P1#3 — integer division: JASS `/` between two integers truncates toward
// zero. Emitted as R2I(a / b) (R2I truncates toward zero exactly like JASS,
// unlike Lua `//` which floors toward -inf). Mixed int/real stays float `/`.
// ---------------------------------------------------------------------------

// TestEmit_IntDiv_FearModulo is the exact failing function from the analysis:
//
//	function FearSystem__modulo takes integer a, integer b returns integer
//	    return a - ( a / b ) * b
//
// Before the fix this emitted `a - ((a / b) * b)` which returns 0.0 for ALL
// inputs (float division). After the fix the inner integer `a / b` becomes
// `R2I(a / b)`, restoring the remainder semantics.
func TestEmit_IntDiv_FearModulo(t *testing.T) {
	src := `function FearSystem__modulo takes integer a,integer b returns integer
return a - ( a / b ) * b
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "R2I(a / b)") {
		t.Errorf("expected integer division wrapped as R2I(a / b), got:\n%s", got)
	}
	// Document/verify the runtime semantics the lowering targets: R2I truncates
	// toward zero, so `a - R2I(a/b)*b` is the true JASS remainder.
	luaModulo := func(a, b int) int {
		q := truncTowardZero(a, b) // == R2I(a / b)
		return a - q*b
	}
	if v := luaModulo(7, 2); v != 1 {
		t.Errorf("FearSystem__modulo(7,2): want 1, got %d", v)
	}
	if v := luaModulo(-7, 2); v != -1 {
		t.Errorf("FearSystem__modulo(-7,2): want -1 (trunc toward zero), got %d", v)
	}
}

// truncTowardZero mirrors WC3-Lua R2I(a / b): float-divide then truncate toward
// zero. Go's integer `/` already truncates toward zero, matching R2I exactly.
func truncTowardZero(a, b int) int { return a / b }

// TestEmit_IntDiv_RealStaysFloat — a division with a real operand must NOT be
// wrapped in R2I; it stays Lua float `/`.
func TestEmit_IntDiv_RealStaysFloat(t *testing.T) {
	src := `function F takes real x, integer n returns real
return x / n
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if strings.Contains(got, "R2I(") {
		t.Errorf("real-typed division must stay float (no R2I), got:\n%s", got)
	}
	if !strings.Contains(got, "x / n") {
		t.Errorf("expected float `x / n`, got:\n%s", got)
	}
}

// TestEmit_IntDiv_RealLiteralStaysFloat — two real literals divide as float.
func TestEmit_IntDiv_RealLiteralStaysFloat(t *testing.T) {
	src := `function F takes nothing returns real
return 7.0 / 2.0
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if strings.Contains(got, "R2I(") {
		t.Errorf("real-literal division must stay float, got:\n%s", got)
	}
}

// TestEmit_IntDiv_IntLiterals — two integer literals divide with truncation.
func TestEmit_IntDiv_IntLiterals(t *testing.T) {
	src := `function F takes nothing returns integer
return 7 / 2
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "R2I(7 / 2)") {
		t.Errorf("expected R2I(7 / 2), got:\n%s", got)
	}
}

// TestEmit_IntDiv_CounterLocal — an integer counter `i / 10` (local declared
// `integer`) wraps in R2I.
func TestEmit_IntDiv_CounterLocal(t *testing.T) {
	src := `function F takes nothing returns nothing
local integer i = 0
set i = i / 10
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "R2I(i / 10)") {
		t.Errorf("expected integer counter `i / 10` wrapped as R2I(i / 10), got:\n%s", got)
	}
}

// TestEmit_IntDiv_CompoundOperandParens — a division operand that is itself a
// binary expression must KEEP its grouping parens inside the R2I wrap. Regression
// guard: the R2I path must use emitExprWithParens (not emitExpr) for both operands,
// or `i / (i2 - 1)` reassociates to `R2I(i / i2 - 1)` == (i/i2)-1 — valid Lua, wrong
// math. Corrupted 34 real gameplay formulas in the Enfo map before this fix.
func TestEmit_IntDiv_CompoundOperandParens(t *testing.T) {
	src := `function F takes nothing returns nothing
local integer i = 0
local integer i2 = 0
set i = i / ( i2 - 1 )
set i = ( i2 - 1 ) / i
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "R2I(i / (i2 - 1))") {
		t.Errorf("compound RIGHT operand lost its parens; want R2I(i / (i2 - 1)), got:\n%s", got)
	}
	if !strings.Contains(got, "R2I((i2 - 1) / i)") {
		t.Errorf("compound LEFT operand lost its parens; want R2I((i2 - 1) / i), got:\n%s", got)
	}
}

// TestEmit_IntDiv_GlobalInteger — division of an integer global wraps too.
func TestEmit_IntDiv_GlobalInteger(t *testing.T) {
	src := `globals
integer udg_total
endglobals
function F takes nothing returns integer
return udg_total / 4
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "R2I(udg_total / 4)") {
		t.Errorf("expected R2I(udg_total / 4), got:\n%s", got)
	}
}

// TestEmit_IntDiv_UnknownStaysFloat — when an operand's type can't be proven
// integer (a bare ident with no decl in scope), the division stays float.
// Conservative: never guess.
func TestEmit_IntDiv_UnknownStaysFloat(t *testing.T) {
	src := `function F takes nothing returns nothing
set x = y / z
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if strings.Contains(got, "R2I(") {
		t.Errorf("unknown-typed division must stay float (no R2I), got:\n%s", got)
	}
}

// ---------------------------------------------------------------------------
// P1#4 — typed array defaults: JASS arrays default unset slots to the element
// type's zero value (0 / 0.0 / false / null). Emitted via __jarray(default)
// for numeric/boolean element types; non-numeric stays `{}` (nil == null).
// ---------------------------------------------------------------------------

// TestEmit_ArrayDefault_IntegerGlobal is the exact FearCounter case: a fresh
// integer-array read-before-write must yield 0, not nil, so `FearCounter[ud]+1`
// is 1 instead of an "arithmetic on nil" crash.
func TestEmit_ArrayDefault_IntegerGlobal(t *testing.T) {
	src := `globals
integer array FearCounter
endglobals
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "FearCounter = __jarray(0)") {
		t.Errorf("expected `FearCounter = __jarray(0)`, got:\n%s", got)
	}
}

// TestEmit_ArrayDefault_BooleanGlobal — boolean array defaults to false.
func TestEmit_ArrayDefault_BooleanGlobal(t *testing.T) {
	src := `globals
boolean array flags
endglobals
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "flags = __jarray(false)") {
		t.Errorf("expected `flags = __jarray(false)`, got:\n%s", got)
	}
}

// TestEmit_ArrayDefault_RealGlobal — real array defaults to 0.0.
func TestEmit_ArrayDefault_RealGlobal(t *testing.T) {
	src := `globals
real array dmg
endglobals
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "dmg = __jarray(0.0)") {
		t.Errorf("expected `dmg = __jarray(0.0)`, got:\n%s", got)
	}
}

// TestEmit_ArrayDefault_HandleStaysEmpty — a handle-typed array keeps `{}`
// because its JASS default is null and Lua's nil-for-unset matches exactly.
func TestEmit_ArrayDefault_HandleStaysEmpty(t *testing.T) {
	src := `globals
unit array units
string array names
endglobals
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "units = {}") {
		t.Errorf("expected handle array `units = {}`, got:\n%s", got)
	}
	if !strings.Contains(got, "names = {}") {
		t.Errorf("expected string array `names = {}`, got:\n%s", got)
	}
}

// TestEmit_ArrayDefault_LocalInteger — local integer arrays get __jarray(0) too.
func TestEmit_ArrayDefault_LocalInteger(t *testing.T) {
	src := `function F takes nothing returns nothing
local integer array buckets
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "local buckets = __jarray(0)") {
		t.Errorf("expected `local buckets = __jarray(0)`, got:\n%s", got)
	}
}

// TestEmit_ArrayDefault_SetThenRead — writing then reading returns the set
// value (the metatable __index only fires for UNSET keys). The emitter writes
// the assignment unchanged; this asserts the write path is untouched.
func TestEmit_ArrayDefault_SetThenRead(t *testing.T) {
	src := `globals
integer array FearCounter
endglobals
function F takes nothing returns integer
set FearCounter[5] = 42
return FearCounter[5]
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "FearCounter = __jarray(0)") {
		t.Errorf("expected typed-array init, got:\n%s", got)
	}
	if !strings.Contains(got, "FearCounter[5] = 42") {
		t.Errorf("expected write `FearCounter[5] = 42` unchanged, got:\n%s", got)
	}
	if !strings.Contains(got, "return FearCounter[5]") {
		t.Errorf("expected read `return FearCounter[5]` unchanged, got:\n%s", got)
	}
}

// TestEmit_ArrayDefault_IndexReadInfersIntForDivision — reading an integer-array
// element is integer-typed, so dividing two such reads wraps in R2I.
func TestEmit_ArrayDefault_IndexReadInfersIntForDivision(t *testing.T) {
	src := `globals
integer array steps
endglobals
function F takes nothing returns integer
return steps[1] / steps[2]
endfunction
`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "R2I(steps[1] / steps[2])") {
		t.Errorf("expected integer-array element division wrapped, got:\n%s", got)
	}
}

// ---------------------------------------------------------------------------
// P2#9 C1 — string concat: JASS `+` is overloaded (numeric add OR string
// concat). The emitter must emit Lua `..` whenever an operand is provably a
// string, even with NO string literal in sight (e.g. two string-typed params).
// Numeric `+` must stay `+`. Unknown stays `+` (conservative).
// ---------------------------------------------------------------------------

// TestEmit_Concat_LiteralFreeStringParams is the core C1 case: two string-typed
// params with NO string literal. Before the fix this wrongly emitted Lua `+`
// (runtime "attempt to perform arithmetic on a string"). After: `..`.
func TestEmit_Concat_LiteralFreeStringParams(t *testing.T) {
	src := `function F takes string a, string b returns string
	return a + b
	endfunction
	`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "a .. b") {
		t.Errorf("literal-free string concat must emit `..`, got:\n%s", got)
	}
	if strings.Contains(got, "a + b") {
		t.Errorf("string concat must NOT emit `+`, got:\n%s", got)
	}
}

// TestEmit_Concat_StringGlobal — a string-typed global concatenated with an
// unknown ident still routes through `..` (one provably-string operand is
// enough).
func TestEmit_Concat_StringGlobal(t *testing.T) {
	src := `globals
	string udg_msg
	endglobals
	function F takes nothing returns string
	return udg_msg + suffix
	endfunction
	`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "udg_msg .. suffix") {
		t.Errorf("string-global concat must emit `..`, got:\n%s", got)
	}
}

// TestEmit_Concat_StringNativeReturn — a string-returning known native
// (I2S/GetUnitName) makes the whole `+` a concat even with an unknown other
// operand.
func TestEmit_Concat_StringNativeReturn(t *testing.T) {
	src := `function F takes nothing returns nothing
	call BJDebugMsg("level " + I2S(lvl))
	endfunction
	`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	// Both the literal hint and the I2S string-return route this to `..`.
	if !strings.Contains(got, `"level " .. I2S(lvl)`) {
		t.Errorf("string-native concat must emit `..`, got:\n%s", got)
	}
}

// TestEmit_Concat_GetUnitNameNoLiteral — `GetUnitName(u) + GetUnitName(v)`:
// neither operand is a literal, both are string-returning natives ⇒ `..`.
func TestEmit_Concat_GetUnitNameNoLiteral(t *testing.T) {
	src := `function F takes unit u, unit v returns string
	return GetUnitName(u) + GetUnitName(v)
	endfunction
	`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "GetUnitName(u) .. GetUnitName(v)") {
		t.Errorf("two string-native concat must emit `..`, got:\n%s", got)
	}
}

// TestEmit_Concat_IntegerAddStaysPlus — two integer-typed params added stays
// Lua `+` (NOT `..`). Regression guard: the string fix must not steal numeric
// add.
func TestEmit_Concat_IntegerAddStaysPlus(t *testing.T) {
	src := `function F takes integer a, integer b returns integer
	return a + b
	endfunction
	`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "a + b") {
		t.Errorf("integer add must stay `+`, got:\n%s", got)
	}
	if strings.Contains(got, "a .. b") {
		t.Errorf("integer add must NOT become `..`, got:\n%s", got)
	}
}

// TestEmit_Concat_RealAddStaysPlus — real `+` stays `+`.
func TestEmit_Concat_RealAddStaysPlus(t *testing.T) {
	src := `function F takes real x, real y returns real
	return x + y
	endfunction
	`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "x + y") || strings.Contains(got, "x .. y") {
		t.Errorf("real add must stay `+`, got:\n%s", got)
	}
}

// TestEmit_Concat_IntLiteralAddStaysPlus — two integer literals add as `+`.
func TestEmit_Concat_IntLiteralAddStaysPlus(t *testing.T) {
	src := `function F takes nothing returns integer
	return 1 + 2
	endfunction
	`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "1 + 2") {
		t.Errorf("int-literal add must stay `+`, got:\n%s", got)
	}
}

// TestEmit_Concat_UnknownStaysPlus — neither operand provably string nor
// numeric ⇒ unknown ⇒ keep `+` (conservative; matches pre-fix behavior so
// already-correct numeric uses of unknown idents don't regress).
func TestEmit_Concat_UnknownStaysPlus(t *testing.T) {
	src := `function F takes nothing returns nothing
	set z = x + y
	endfunction
	`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "x + y") {
		t.Errorf("unknown-typed `+` must stay `+` (conservative), got:\n%s", got)
	}
}

// TestEmit_Concat_OneStringLiteralUnchanged is the DO-NO-HARM guard: the
// pre-existing one-string-literal heuristic must emit EXACTLY as before
// (`"x=" .. n`). This already worked; the type-based path must not change it.
func TestEmit_Concat_OneStringLiteralUnchanged(t *testing.T) {
	src := `function F takes integer n returns string
	return "x=" + n
	endfunction
	`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, `"x=" .. n`) {
		t.Errorf("one-string-literal concat must stay `..`, got:\n%s", got)
	}
}

// TestEmit_Concat_StringChainLiteralFree — a 3-way literal-free chain of
// string params concatenates fully (`a .. b .. c`). Exercises the
// inferBinaryType `+`-returns-string path so the chain classifies as string
// at every node.
func TestEmit_Concat_StringChainLiteralFree(t *testing.T) {
	src := `function F takes string a, string b, string c returns string
	return a + b + c
	endfunction
	`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "a .. b") || !strings.Contains(got, ".. c") {
		t.Errorf("literal-free string chain must use `..` throughout, got:\n%s", got)
	}
	if strings.Contains(got, " + ") {
		t.Errorf("no `+` should survive in an all-string chain, got:\n%s", got)
	}
}

// ---------------------------------------------------------------------------
// P2#9 C2 — ternary correctness: `cond ? then : else`. The bare
// `(cond and then) or else` idiom returns `else` when `then` is false/nil. The
// fix emits the table-wrap form `((cond) and {then} or {else})[1]` when `then`
// could be falsy, and keeps the cheap idiom (byte-identical) when `then` is
// provably truthy.
// ---------------------------------------------------------------------------

// TestEmit_Ternary_FalseThenReturnsThen is the core C2 case: a literal `false`
// then-branch must return `then` (false), NOT `else`. We verify both the
// emitted FORM and the Lua-equivalent runtime SEMANTICS.
func TestEmit_Ternary_FalseThenReturnsThen(t *testing.T) {
	// `cond ? false : true` — with cond true, JASS yields false. The broken
	// idiom `(cond and false) or true` yields true (wrong).
	src := `function F takes boolean cond returns boolean
	return cond ? false : true
	endfunction
	`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "and {false} or {true})[1]") {
		t.Errorf("false-then ternary must use table-wrap form, got:\n%s", got)
	}
	// Demonstrate the semantics the table-wrap form restores vs the broken
	// and/or idiom, for cond == true:
	//   broken:   (true and false) or true  == true   (WRONG)
	//   tablewrap:((true and {false} or {true})[1]) == false (CORRECT)
	const condTrue = true
	broken := func() bool {
		// (cond and then) or else, with then=false, else=true
		if condTrue && false {
			return false
		}
		return true // the `or else` fallback fires on falsy then
	}
	if broken() != true {
		t.Fatalf("sanity: broken idiom should mis-return true")
	}
	tablewrap := func() bool {
		// {then}/{else} are always-truthy tables; [1] unwraps the real value.
		var picked []bool
		if condTrue {
			picked = []bool{false}
		} else {
			picked = []bool{true}
		}
		return picked[0]
	}
	if tablewrap() != false {
		t.Errorf("table-wrap form must return then==false for cond==true, got %v", tablewrap())
	}
}

// TestEmit_Ternary_NumericThenKeepsAndOr is the DO-NO-HARM guard: a
// provably-truthy then-arm (integer literal) keeps the cheap `(cond and then)
// or else` idiom byte-for-byte — NOT the table-wrap form. (Mirrors the existing
// TestEmit_Ternary expectation, asserted here against the new code path.)
func TestEmit_Ternary_NumericThenKeepsAndOr(t *testing.T) {
	src := `function F takes nothing returns integer
	return x ? 1 : 2
	endfunction
	`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "((x) and (1) or (2))") {
		t.Errorf("numeric-then ternary must keep cheap and/or idiom, got:\n%s", got)
	}
	if strings.Contains(got, "{1}") || strings.Contains(got, "[1]") {
		t.Errorf("provably-truthy then must NOT use table-wrap form, got:\n%s", got)
	}
}

// TestEmit_Ternary_StringThenKeepsAndOr — a string literal then-arm is truthy
// (even "" is truthy in Lua), so the cheap idiom is kept.
func TestEmit_Ternary_StringThenKeepsAndOr(t *testing.T) {
	src := `function F takes boolean cond returns string
	return cond ? "yes" : "no"
	endfunction
	`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, `(cond) and ("yes") or ("no")`) {
		t.Errorf("string-then ternary must keep cheap and/or idiom, got:\n%s", got)
	}
}

// TestEmit_Ternary_NullThenUsesTableForm — a `null` then-arm lowers to Lua
// `nil` (falsy), so it must use the correct table-wrap form to actually return
// nil instead of the else-branch.
func TestEmit_Ternary_NullThenUsesTableForm(t *testing.T) {
	src := `function F takes boolean cond returns unit
	return cond ? null : u
	endfunction
	`
	got, err := TranspileScript(src)
	if err != nil {
		t.Fatalf("transpile err: %v", err)
	}
	if !strings.Contains(got, "and {nil} or {u})[1]") {
		t.Errorf("null-then ternary must use table-wrap form, got:\n%s", got)
	}
}
