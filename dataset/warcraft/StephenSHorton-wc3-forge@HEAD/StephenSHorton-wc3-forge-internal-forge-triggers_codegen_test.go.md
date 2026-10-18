---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/triggers_codegen_test.go"
ref: HEAD
---

package forge

import (
	"regexp"
	"strings"
	"testing"

	"github.com/StephenSHorton/wc3-forge/internal/formats/w3i"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wtg"
)

// Phase 3 codegen tests. One assertion per magic ECA case, plus one
// end-to-end snapshot test that builds a small *wtg.Triggers + walks the
// full GenerateLuaScript pipeline.
//
// Magic ECAs covered (Risk #4 — gui.cpp L102-413):
//
//	IfThenElse, IfThenElseMultiple,
//	ForLoopA, ForLoopAMultiple, ForLoopB, ForLoopBMultiple,
//	ForLoopVar, ForLoopVarMultiple,
//	AndMultiple, OrMultiple,
//	SetVariable, CommentString, CustomScriptCode,
//	WaitForCondition, OperatorCompare* (Integer/Real/Boolean/String),
//	OperatorInt, OperatorReal, OperatorString,
//	AddTriggerEvent, EnumDestructablesInCircleBJMultiple.
//
// The tests use a small synthesized TriggerData rather than the embedded
// snapshot — the snapshot is huge (689k) and tests want fast + isolated.

// stubTriggerData returns a *wtg.TriggerData with just enough rows to
// support the magic ECA cases. ArgumentCounts entries are populated via
// the same algorithm the real snapshot uses (computeArgumentCounts).
func stubTriggerData() *wtg.TriggerData {
	src := []byte(`
[TriggerActions]
IfThenElse=0,boolexpr,code,code
IfThenElseMultiple=0,nothing
ForLoopA=0,integer,integer,code
ForLoopAMultiple=0,integer,integer
ForLoopB=0,integer,integer,code
ForLoopBMultiple=0,integer,integer
ForLoopVar=0,integervar,integer,integer,code
ForLoopVarMultiple=0,integervar,integer,integer
SetVariable=0,VarAsString_Any,AnyType
CommentString=0,string
CustomScriptCode=0,string
WaitForCondition=0,boolexpr,real
DoNothing=0,nothing
AddTriggerEvent=0,trigger,event
EnumDestructablesInCircleBJMultiple=0,real,location
KillUnit=0,unit
_KillUnit_ScriptName=KillUnit
[TriggerEvents]
MapInitializationEvent=0,nothing
[TriggerConditions]
OperatorCompareInteger=0,integer,intcomparator,integer
OperatorCompareReal=0,real,realcomparator,real
OperatorCompareBoolean=0,boolean,booleancomparator,boolean
OperatorCompareString=0,string,stringcomparator,string
[TriggerCalls]
OperatorInt=0,1,integer,integer,intoperator,integer
OperatorReal=0,1,real,real,realoperator,real
OperatorString=0,1,string,string,string
GetTriggeringTrigger=0,1,trigger
GetUnitLoc=0,1,location,unit
[TriggerTypes]
integer=0,1,1,WESTRING_TRIGTYPE_integer
real=0,1,1,WESTRING_TRIGTYPE_real
boolean=0,1,1,WESTRING_TRIGTYPE_boolean
string=0,1,1,WESTRING_TRIGTYPE_string
unit=0,1,1,WESTRING_TRIGTYPE_unit
trigger=0,0,0,WESTRING_TRIGTYPE_trigger
[TriggerTypeDefaults]
integer=0
real=0
boolean=false
string=""
[TriggerParams]
IntGreaterThan=0,intcomparator,>,WESTRING_TRIGPARAM_INTGREATERTHAN
IntNotEqual=0,intcomparator,!=,WESTRING_TRIGPARAM_INTNOTEQUAL
IntPlus=0,intoperator,+,WESTRING_TRIGPARAM_INTPLUS
RealPlus=0,realoperator,+,WESTRING_TRIGPARAM_REALPLUS
`)
	td, err := wtg.ParseTriggerData(src)
	if err != nil {
		panic(err)
	}
	return td
}

// helper: build a tiny CodegenInputs with the stub TD and a single trigger.
// Lua flag is set to true so the codegen doesn't reject the call.
func buildInputs(t *testing.T, tr wtg.Trigger, vars []wtg.Variable) CodegenInputs {
	t.Helper()
	return CodegenInputs{
		Triggers: &wtg.Triggers{
			Triggers:  []wtg.Trigger{tr},
			Variables: vars,
		},
		Info: &w3i.Info{Lua: true},
		TD:   stubTriggerData(),
	}
}

// runOneTrigger calls convertTriggerToLua on a single trigger and returns
// the rendered Lua. Useful for per-magic-case assertions that don't want
// the full file scaffold.
func runOneTrigger(t *testing.T, in CodegenInputs) string {
	t.Helper()
	cg := &luaCodegen{in: in}
	var inits []string
	return cg.convertTriggerToLua(&in.Triggers.Triggers[0], &inits)
}

// makeECA builds a single Action ECA with no parameters.
func makeECA(name string, params ...wtg.Parameter) wtg.ECA {
	return wtg.ECA{Type: wtg.ECAAction, Name: name, Enabled: true, Parameters: params}
}

func presetParam(value string) wtg.Parameter {
	return wtg.Parameter{Type: wtg.ParamPreset, Value: value}
}
func intLiteralParam(value string) wtg.Parameter {
	return wtg.Parameter{Type: wtg.ParamString, Value: value}
}
func stringLiteralParam(value string) wtg.Parameter {
	return wtg.Parameter{Type: wtg.ParamString, Value: value}
}
func variableParam(name string) wtg.Parameter {
	return wtg.Parameter{Type: wtg.ParamVariable, Value: name}
}

// -------- per-magic-ECA tests --------

func TestCodegen_IfThenElse(t *testing.T) {
	// IfThenElse takes 3 params: boolexpr, code, code. Use sub-function
	// references for clarity.
	cond := wtg.Parameter{Type: wtg.ParamFunction, Value: "GetTriggeringTrigger", HasSubParameter: true,
		SubParameter: &wtg.ECA{Type: wtg.ECACall, Name: "GetTriggeringTrigger", Enabled: true}}
	thenBranch := wtg.Parameter{Type: wtg.ParamFunction, Value: "DoNothing", HasSubParameter: true,
		SubParameter: &wtg.ECA{Type: wtg.ECAAction, Name: "DoNothing", Enabled: true}}
	elseBranch := wtg.Parameter{Type: wtg.ParamFunction, Value: "DoNothing", HasSubParameter: true,
		SubParameter: &wtg.ECA{Type: wtg.ECAAction, Name: "DoNothing", Enabled: true}}
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "TestTrig", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{makeECA("IfThenElse", cond, thenBranch, elseBranch)},
	}, nil)
	got := runOneTrigger(t, in)
	if !strings.Contains(got, "if (Trig_TestTrig_Wc3Forge") {
		t.Fatalf("expected if (Trig_TestTrig_Wc3Forge…) in:\n%s", got)
	}
	if !strings.Contains(got, "DoNothing()") {
		t.Fatalf("expected DoNothing() body:\n%s", got)
	}
}

func TestCodegen_IfThenElseMultiple(t *testing.T) {
	// Multi-child IfThenElseMultiple: 1 condition + 1 then-action (group=1) +
	// 1 else-action (group=0).
	thenAct := wtg.ECA{Type: wtg.ECAAction, Name: "DoNothing", Enabled: true, Group: 1}
	elseAct := wtg.ECA{Type: wtg.ECAAction, Name: "DoNothing", Enabled: true, Group: 0}
	condChild := wtg.ECA{Type: wtg.ECACondition, Name: "OperatorCompareInteger", Enabled: true,
		Parameters: []wtg.Parameter{intLiteralParam("1"), presetParam("IntGreaterThan"), intLiteralParam("0")}}
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "TestTrig", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{{
			Type: wtg.ECAAction, Name: "IfThenElseMultiple", Enabled: true,
			Children: []wtg.ECA{condChild, thenAct, elseAct},
		}},
	}, nil)
	got := runOneTrigger(t, in)
	if !strings.Contains(got, "not (1 > 0)") {
		t.Fatalf("expected nested 'not (1 > 0)' inside the closure (HiveWE lowers conds as `if not (X)` returns false):\n%s", got)
	}
	// Must produce an if/then/else around the action body.
	if !regexp.MustCompile(`if \(Trig_TestTrig_Wc3Forge\d+\(\)\) then`).MatchString(got) {
		t.Fatalf("expected if (…closure call…) then in:\n%s", got)
	}
}

func TestCodegen_ForLoopA(t *testing.T) {
	body := wtg.Parameter{Type: wtg.ParamFunction, Value: "DoNothing", HasSubParameter: true,
		SubParameter: &wtg.ECA{Type: wtg.ECAAction, Name: "DoNothing", Enabled: true}}
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{makeECA("ForLoopA", intLiteralParam("1"), intLiteralParam("10"), body)},
	}, nil)
	got := runOneTrigger(t, in)
	if !strings.Contains(got, "bj_forLoopAIndex = 1") {
		t.Fatalf("expected bj_forLoopAIndex = 1:\n%s", got)
	}
	if !strings.Contains(got, "bj_forLoopAIndex <= bj_forLoopAIndexEnd") {
		t.Fatalf("expected while-loop guard:\n%s", got)
	}
	if !strings.Contains(got, "bj_forLoopAIndex = bj_forLoopAIndex + 1") {
		t.Fatalf("expected increment:\n%s", got)
	}
}

func TestCodegen_ForLoopAMultiple(t *testing.T) {
	body := wtg.ECA{Type: wtg.ECAAction, Name: "DoNothing", Enabled: true}
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{{
			Type: wtg.ECAAction, Name: "ForLoopAMultiple", Enabled: true,
			Parameters: []wtg.Parameter{intLiteralParam("1"), intLiteralParam("3")},
			Children:   []wtg.ECA{body},
		}},
	}, nil)
	got := runOneTrigger(t, in)
	if !strings.Contains(got, "bj_forLoopAIndex = 1") || !strings.Contains(got, "DoNothing()") {
		t.Fatalf("ForLoopAMultiple shape wrong:\n%s", got)
	}
}

func TestCodegen_ForLoopB(t *testing.T) {
	body := wtg.Parameter{Type: wtg.ParamFunction, Value: "DoNothing", HasSubParameter: true,
		SubParameter: &wtg.ECA{Type: wtg.ECAAction, Name: "DoNothing", Enabled: true}}
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{makeECA("ForLoopB", intLiteralParam("5"), intLiteralParam("9"), body)},
	}, nil)
	got := runOneTrigger(t, in)
	if !strings.Contains(got, "bj_forLoopBIndex") {
		t.Fatalf("expected bj_forLoopBIndex:\n%s", got)
	}
}

func TestCodegen_ForLoopBMultiple(t *testing.T) {
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{{
			Type: wtg.ECAAction, Name: "ForLoopBMultiple", Enabled: true,
			Parameters: []wtg.Parameter{intLiteralParam("0"), intLiteralParam("2")},
			Children:   []wtg.ECA{{Type: wtg.ECAAction, Name: "DoNothing", Enabled: true}},
		}},
	}, nil)
	got := runOneTrigger(t, in)
	if !strings.Contains(got, "bj_forLoopBIndex = 0") {
		t.Fatalf("expected bj_forLoopBIndex = 0:\n%s", got)
	}
}

func TestCodegen_ForLoopVar(t *testing.T) {
	v := variableParam("i")
	body := wtg.Parameter{Type: wtg.ParamFunction, Value: "DoNothing", HasSubParameter: true,
		SubParameter: &wtg.ECA{Type: wtg.ECAAction, Name: "DoNothing", Enabled: true}}
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{makeECA("ForLoopVar", v, intLiteralParam("1"), intLiteralParam("10"), body)},
	}, []wtg.Variable{{Name: "i", Type: "integer"}})
	got := runOneTrigger(t, in)
	if !strings.Contains(got, "udg_i = 1") {
		t.Fatalf("expected udg_i = 1:\n%s", got)
	}
	if !strings.Contains(got, "udg_i = udg_i + 1") {
		t.Fatalf("expected udg_i increment:\n%s", got)
	}
}

func TestCodegen_ForLoopVarMultiple(t *testing.T) {
	v := variableParam("i")
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{{
			Type: wtg.ECAAction, Name: "ForLoopVarMultiple", Enabled: true,
			Parameters: []wtg.Parameter{v, intLiteralParam("1"), intLiteralParam("3")},
			Children:   []wtg.ECA{{Type: wtg.ECAAction, Name: "DoNothing", Enabled: true}},
		}},
	}, []wtg.Variable{{Name: "i", Type: "integer"}})
	got := runOneTrigger(t, in)
	if !strings.Contains(got, "udg_i = 1") || !strings.Contains(got, "DoNothing()") {
		t.Fatalf("ForLoopVarMultiple shape wrong:\n%s", got)
	}
}

func TestCodegen_AndMultiple(t *testing.T) {
	cond := wtg.ECA{Type: wtg.ECACondition, Name: "OperatorCompareInteger", Enabled: true,
		Parameters: []wtg.Parameter{intLiteralParam("1"), presetParam("IntGreaterThan"), intLiteralParam("0")}}
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{{
			Type: wtg.ECACondition, Name: "AndMultiple", Enabled: true,
			Children: []wtg.ECA{cond, cond},
		}},
	}, nil)
	got := runOneTrigger(t, in)
	// AndMultiple emits a per-cond not(...) guard returning false, else return true.
	if !strings.Contains(got, "not(1 > 0)") {
		t.Fatalf("expected AndMultiple guard 'not(1 > 0)':\n%s", got)
	}
	if !strings.Contains(got, "return true") {
		t.Fatalf("expected return true after guards:\n%s", got)
	}
}

func TestCodegen_OrMultiple(t *testing.T) {
	cond := wtg.ECA{Type: wtg.ECACondition, Name: "OperatorCompareInteger", Enabled: true,
		Parameters: []wtg.Parameter{intLiteralParam("1"), presetParam("IntGreaterThan"), intLiteralParam("0")}}
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{{
			Type: wtg.ECACondition, Name: "OrMultiple", Enabled: true,
			Children: []wtg.ECA{cond, cond},
		}},
	}, nil)
	got := runOneTrigger(t, in)
	if !strings.Contains(got, "if (1 > 0) then") {
		t.Fatalf("expected OrMultiple if-condition emit:\n%s", got)
	}
	if !strings.Contains(got, "return false") {
		t.Fatalf("expected fallthrough return false:\n%s", got)
	}
}

func TestCodegen_SetVariable(t *testing.T) {
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{{
			Type: wtg.ECAAction, Name: "SetVariable", Enabled: true,
			Parameters: []wtg.Parameter{variableParam("counter"), intLiteralParam("42")},
		}},
	}, []wtg.Variable{{Name: "counter", Type: "integer"}})
	got := runOneTrigger(t, in)
	if !strings.Contains(got, "udg_counter = 42") {
		t.Fatalf("expected udg_counter = 42:\n%s", got)
	}
}

func TestCodegen_CommentString(t *testing.T) {
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{makeECA("CommentString", stringLiteralParam("hello world"))},
	}, nil)
	got := runOneTrigger(t, in)
	if !strings.Contains(got, "-- ") || !strings.Contains(got, "hello world") {
		t.Fatalf("expected Lua comment for CommentString:\n%s", got)
	}
}

func TestCodegen_CustomScriptCode(t *testing.T) {
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{makeECA("CustomScriptCode", stringLiteralParam("print(\"hi\")"))},
	}, nil)
	got := runOneTrigger(t, in)
	if !strings.Contains(got, `print("hi")`) {
		t.Fatalf("expected raw inline print(\"hi\"):\n%s", got)
	}
}

func TestCodegen_WaitForCondition(t *testing.T) {
	cond := wtg.Parameter{Type: wtg.ParamFunction, HasSubParameter: true, Value: "GetTriggeringTrigger",
		SubParameter: &wtg.ECA{Type: wtg.ECACall, Name: "GetTriggeringTrigger", Enabled: true}}
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{makeECA("WaitForCondition", cond, intLiteralParam("0.1"))},
	}, nil)
	got := runOneTrigger(t, in)
	if !strings.Contains(got, "while (not (GetTriggeringTrigger())) do") {
		t.Fatalf("expected WaitForCondition while-not:\n%s", got)
	}
	if !strings.Contains(got, "TriggerSleepAction(RMaxBJ(bj_WAIT_FOR_COND_MIN_INTERVAL, 0.1))") {
		t.Fatalf("expected TriggerSleepAction body:\n%s", got)
	}
}

func TestCodegen_OperatorCompareInteger(t *testing.T) {
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{{
			Type: wtg.ECACondition, Name: "OperatorCompareInteger", Enabled: true,
			Parameters: []wtg.Parameter{intLiteralParam("3"), presetParam("IntGreaterThan"), intLiteralParam("1")},
		}},
	}, nil)
	got := runOneTrigger(t, in)
	if !strings.Contains(got, "3 > 1") {
		t.Fatalf("expected '3 > 1':\n%s", got)
	}
}

func TestCodegen_OperatorCompareNotEqualLuaSubstitution(t *testing.T) {
	// `!=` must become `~=` in Lua per gui.cpp L341-344.
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{{
			Type: wtg.ECACondition, Name: "OperatorCompareInteger", Enabled: true,
			Parameters: []wtg.Parameter{intLiteralParam("3"), presetParam("IntNotEqual"), intLiteralParam("1")},
		}},
	}, nil)
	got := runOneTrigger(t, in)
	if !strings.Contains(got, "3 ~= 1") {
		t.Fatalf("expected != → ~= rewrite:\n%s", got)
	}
}

// callConvertDirectly tests an ECA expression-shape via convertECAToLua
// without going through convertTriggerToLua's event/cond/action filter
// (Operators are ECACall — invoked from inside other ECAs, not at top
// level on their own).
func callConvertDirectly(t *testing.T, td *wtg.TriggerData, eca wtg.ECA) string {
	t.Helper()
	cg := &luaCodegen{in: CodegenInputs{TD: td, Triggers: &wtg.Triggers{}}}
	out, err := cg.convertECAToLua(&eca, "T", false)
	if err != nil {
		t.Fatalf("convertECAToLua: %v", err)
	}
	return out
}

func TestCodegen_OperatorInt(t *testing.T) {
	td := stubTriggerData()
	got := callConvertDirectly(t, td, wtg.ECA{
		Type: wtg.ECACall, Name: "OperatorInt", Enabled: true,
		Parameters: []wtg.Parameter{intLiteralParam("2"), presetParam("IntPlus"), intLiteralParam("3")},
	})
	if got != "2 + 3" {
		t.Fatalf("expected '2 + 3', got %q", got)
	}
}

func TestCodegen_OperatorReal(t *testing.T) {
	td := stubTriggerData()
	got := callConvertDirectly(t, td, wtg.ECA{
		Type: wtg.ECACall, Name: "OperatorReal", Enabled: true,
		Parameters: []wtg.Parameter{intLiteralParam("1.5"), presetParam("RealPlus"), intLiteralParam("2.5")},
	})
	if got != "1.5 + 2.5" {
		t.Fatalf("expected '1.5 + 2.5', got %q", got)
	}
}

func TestCodegen_OperatorString(t *testing.T) {
	td := stubTriggerData()
	got := callConvertDirectly(t, td, wtg.ECA{
		Type: wtg.ECACall, Name: "OperatorString", Enabled: true,
		Parameters: []wtg.Parameter{stringLiteralParam("foo"), stringLiteralParam("bar")},
	})
	if got != `("foo" .. "bar")` {
		t.Fatalf("expected '(\"foo\" .. \"bar\")', got %q", got)
	}
}

func TestCodegen_AddTriggerEvent(t *testing.T) {
	// AddTriggerEvent param[0] is a trigger var; param[1] is an event-bind
	// sub-function call. We expect AddTriggerEvent to inject param[0] at
	// the front of param[1]'s argument list.
	getTrig := wtg.Parameter{Type: wtg.ParamFunction, HasSubParameter: true, Value: "GetTriggeringTrigger",
		SubParameter: &wtg.ECA{Type: wtg.ECACall, Name: "GetTriggeringTrigger", Enabled: true}}
	// Use GetUnitLoc as the "event" param (it parses to a function call we
	// can verify gets argument-injected).
	eventBind := wtg.Parameter{Type: wtg.ParamFunction, HasSubParameter: true, Value: "GetUnitLoc",
		SubParameter: &wtg.ECA{Type: wtg.ECACall, Name: "GetUnitLoc", Enabled: true,
			Parameters: []wtg.Parameter{variableParam("someUnit")}}}
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{makeECA("AddTriggerEvent", getTrig, eventBind)},
	}, []wtg.Variable{{Name: "someUnit", Type: "unit"}})
	got := runOneTrigger(t, in)
	if !strings.Contains(got, "GetUnitLoc(GetTriggeringTrigger(), udg_someUnit)") {
		t.Fatalf("expected injected first-arg in GetUnitLoc(…):\n%s", got)
	}
}

func TestCodegen_EnumDestructablesInCircleBJMultiple(t *testing.T) {
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{{
			Type: wtg.ECAAction, Name: "EnumDestructablesInCircleBJMultiple", Enabled: true,
			Parameters: []wtg.Parameter{intLiteralParam("500"), intLiteralParam("origin")},
			Children:   []wtg.ECA{{Type: wtg.ECAAction, Name: "DoNothing", Enabled: true}},
		}},
	}, nil)
	got := runOneTrigger(t, in)
	// Closure name + 3-arg call (radius, location, closure)
	if !strings.Contains(got, "EnumDestructablesInCircleBJMultiple(500, origin, Trig_T_Wc3Forge") {
		t.Fatalf("expected three-arg EnumDestructablesInCircleBJMultiple call:\n%s", got)
	}
}

// -------- generic fallback + scaffold tests --------

func TestCodegen_GenericFunctionEmit(t *testing.T) {
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{makeECA("KillUnit", variableParam("victim"))},
	}, []wtg.Variable{{Name: "victim", Type: "unit"}})
	got := runOneTrigger(t, in)
	if !strings.Contains(got, "KillUnit(udg_victim)") {
		t.Fatalf("expected generic KillUnit(udg_victim):\n%s", got)
	}
}

func TestCodegen_LuaOnlyRefusesJASS(t *testing.T) {
	_, err := GenerateLuaScript(CodegenInputs{
		Triggers: &wtg.Triggers{},
		Info:     &w3i.Info{Lua: false},
		TD:       stubTriggerData(),
	})
	if err != ErrLuaOnly {
		t.Fatalf("expected ErrLuaOnly when Info.Lua=false, got %v", err)
	}
}

func TestCodegen_FullScriptShape(t *testing.T) {
	// End-to-end snapshot: 2 categories, 3 triggers (mix of GUI + script +
	// init-event), 2 variables. Confirms the global emit + InitTrig_* +
	// InitCustomTriggers + RunInitializationTriggers chain hangs together.
	in := CodegenInputs{
		Info: &w3i.Info{Lua: true, Name: "TestMap", Description: "desc",
			Players: []w3i.Player{{InternalNumber: 0, Type: w3i.PlayerType(0), Race: w3i.PlayerRace(1)}}},
		TD: stubTriggerData(),
		Triggers: &wtg.Triggers{
			Categories: []wtg.Category{
				{Classifier: wtg.ClassifierMap, ID: 0, ParentID: -1, Name: "Map Header"},
				{Classifier: wtg.ClassifierCategory, ID: 1, ParentID: 0, Name: "Main"},
			},
			Variables: []wtg.Variable{
				{Name: "counter", Type: "integer", ID: 100, ParentID: 1},
				{Name: "names", Type: "string", IsArray: true, ArraySize: 4, ID: 101, ParentID: 1},
			},
			Triggers: []wtg.Trigger{
				{
					Classifier: wtg.ClassifierGUI, ID: 10, ParentID: 1, Name: "OnInit", IsEnabled: true, InitiallyOn: true,
					ECAs: []wtg.ECA{
						{Type: wtg.ECAEvent, Name: "MapInitializationEvent", Enabled: true},
						{Type: wtg.ECAAction, Name: "SetVariable", Enabled: true,
							Parameters: []wtg.Parameter{variableParam("counter"), intLiteralParam("1")}},
					},
				},
				{
					Classifier: wtg.ClassifierGUI, ID: 11, ParentID: 1, Name: "Loop", IsEnabled: true, InitiallyOn: true,
					ECAs: []wtg.ECA{
						makeECA("ForLoopA", intLiteralParam("1"), intLiteralParam("5"),
							wtg.Parameter{Type: wtg.ParamFunction, HasSubParameter: true, Value: "DoNothing",
								SubParameter: &wtg.ECA{Type: wtg.ECAAction, Name: "DoNothing", Enabled: true}}),
					},
				},
				{
					Classifier: wtg.ClassifierScript, ID: 12, ParentID: 1, Name: "ScriptThing",
					IsEnabled: true, InitiallyOn: true, IsScript: true,
					CustomText: "function ScriptThing_extra()\n\tprint(\"hi\")\nend\n",
				},
			},
		},
	}
	out, err := GenerateLuaScript(in)
	if err != nil {
		t.Fatalf("GenerateLuaScript: %v", err)
	}
	mustContain := []string{
		"-- generated by wc3-forge",
		"udg_counter = 0",                       // top-level globals init
		"udg_names = __jarray",                  // array variable
		"gg_trg_OnInit = nil",                   // trigger global
		"function InitGlobals()",                // InitGlobals fn
		"function CreateAllUnits()",             // unit emit
		"function CreateAllDestructables()",     // doodad emit
		"function InitTrig_OnInit()",            // per-trigger init
		"function InitTrig_Loop()",
		"function InitCustomTriggers()",
		"\tInitTrig_OnInit()",
		"function RunInitializationTriggers()",
		"\tConditionalTriggerExecute(gg_trg_OnInit)", // MapInitializationEvent → run-init list
		"function ScriptThing_extra()",                // script trigger's custom_text inlined
		"function main()",
		"function config()",
		"SetMapName(\"TestMap\")",
	}
	for _, sub := range mustContain {
		if !strings.Contains(out, sub) {
			t.Errorf("missing %q in generated script (full output below)", sub)
		}
	}
	if testing.Verbose() {
		t.Logf("generated script:\n%s", out)
	}
}

func TestCodegen_SearchTriggers(t *testing.T) {
	s := &Session{}
	s.triggers = &wtg.Triggers{
		Categories: []wtg.Category{
			{ID: 1, ParentID: 0, Name: "MyCategory"},
		},
		Triggers: []wtg.Trigger{
			{ID: 10, ParentID: 1, Name: "FooTrigger", IsEnabled: true, ECAs: []wtg.ECA{
				{Type: wtg.ECAAction, Name: "KillUnit", Enabled: true,
					Parameters: []wtg.Parameter{{Type: wtg.ParamString, Value: "udg_bar"}}},
			}},
			{ID: 11, ParentID: 1, Name: "BarTrigger", IsEnabled: true,
				IsScript: true, CustomText: "-- custom\nprint(\"foo\")\n"},
		},
		Variables: []wtg.Variable{
			{ID: 20, Name: "barCounter", Type: "integer"},
		},
	}
	s.loaded = true
	hits := s.SearchTriggers("foo", 10)
	if len(hits) == 0 {
		t.Fatalf("expected at least one hit for 'foo'")
	}
	// First (highest scored) should be the FooTrigger name match.
	if hits[0].TriggerName != "FooTrigger" {
		t.Errorf("expected FooTrigger first, got %q", hits[0].TriggerName)
	}
}
