---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/triggers_codegen_jass_test.go"
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

// JASS codegen tests — the native-JASS sibling of triggers_codegen_test.go.
// They reuse that file's stubTriggerData() + param helpers (same package) and
// assert the JASS-specific divergences from the Lua emitter:
//
//	statement calls       `call Foo(args)`     (Lua: bare `Foo(args)`)
//	assignments           `set x = y`          (Lua: `x = y`)
//	if/then/else          `... endif`          (Lua: `... end`)
//	loops                 `loop .. exitwhen .. endloop`  (Lua: `while .. do .. end`)
//	not-equal             `a != b`             (Lua: `a ~= b`)
//	string concat         `(a + b)`            (Lua: `(a .. b)`)
//	rawcodes              `'hfoo'`             (Lua: `FourCC("hfoo")`)
//	null / globals block  `globals .. endglobals`

// runOneTriggerJASS renders a single trigger through convertTriggerToJASS.
func runOneTriggerJASS(t *testing.T, in CodegenInputs) string {
	t.Helper()
	cg := &jassCodegen{in: in}
	var inits []string
	return cg.convertTriggerToJASS(&in.Triggers.Triggers[0], &inits)
}

// callConvertDirectlyJASS evaluates an ECA in expression position.
func callConvertDirectlyJASS(t *testing.T, td *wtg.TriggerData, eca wtg.ECA) string {
	t.Helper()
	cg := &jassCodegen{in: CodegenInputs{TD: td, Triggers: &wtg.Triggers{}}}
	out, err := cg.convertECAToJASS(&eca, "T", false)
	if err != nil {
		t.Fatalf("convertECAToJASS: %v", err)
	}
	return out
}

func TestJASSCodegen_SetVariable(t *testing.T) {
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{{
			Type: wtg.ECAAction, Name: "SetVariable", Enabled: true,
			Parameters: []wtg.Parameter{variableParam("counter"), intLiteralParam("42")},
		}},
	}, []wtg.Variable{{Name: "counter", Type: "integer"}})
	got := runOneTriggerJASS(t, in)
	if !strings.Contains(got, "set udg_counter = 42") {
		t.Fatalf("expected 'set udg_counter = 42':\n%s", got)
	}
}

func TestJASSCodegen_GenericCallPrefix(t *testing.T) {
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{makeECA("KillUnit", variableParam("victim"))},
	}, []wtg.Variable{{Name: "victim", Type: "unit"}})
	got := runOneTriggerJASS(t, in)
	if !strings.Contains(got, "call KillUnit(udg_victim)") {
		t.Fatalf("expected 'call KillUnit(udg_victim)':\n%s", got)
	}
}

func TestJASSCodegen_OperatorCompareNotEqualNoLuaSub(t *testing.T) {
	// JASS keeps `!=` (no Lua `~=` rewrite).
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{{
			Type: wtg.ECACondition, Name: "OperatorCompareInteger", Enabled: true,
			Parameters: []wtg.Parameter{intLiteralParam("3"), presetParam("IntNotEqual"), intLiteralParam("1")},
		}},
	}, nil)
	got := runOneTriggerJASS(t, in)
	if !strings.Contains(got, "3 != 1") {
		t.Fatalf("expected '3 != 1' (no ~= rewrite):\n%s", got)
	}
	if strings.Contains(got, "~=") {
		t.Fatalf("JASS must not contain Lua '~=':\n%s", got)
	}
	// Condition wrapper uses endif, not end.
	if !strings.Contains(got, "endif") {
		t.Fatalf("expected JASS 'endif' condition wrapper:\n%s", got)
	}
}

func TestJASSCodegen_OperatorString(t *testing.T) {
	td := stubTriggerData()
	got := callConvertDirectlyJASS(t, td, wtg.ECA{
		Type: wtg.ECACall, Name: "OperatorString", Enabled: true,
		Parameters: []wtg.Parameter{stringLiteralParam("foo"), stringLiteralParam("bar")},
	})
	if got != `("foo" + "bar")` {
		t.Fatalf(`expected '("foo" + "bar")', got %q`, got)
	}
}

func TestJASSCodegen_OperatorInt(t *testing.T) {
	td := stubTriggerData()
	got := callConvertDirectlyJASS(t, td, wtg.ECA{
		Type: wtg.ECACall, Name: "OperatorInt", Enabled: true,
		Parameters: []wtg.Parameter{intLiteralParam("2"), presetParam("IntPlus"), intLiteralParam("3")},
	})
	if got != "2 + 3" {
		t.Fatalf("expected '2 + 3', got %q", got)
	}
}

func TestJASSCodegen_ForLoopA(t *testing.T) {
	body := wtg.Parameter{Type: wtg.ParamFunction, Value: "DoNothing", HasSubParameter: true,
		SubParameter: &wtg.ECA{Type: wtg.ECAAction, Name: "DoNothing", Enabled: true}}
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{makeECA("ForLoopA", intLiteralParam("1"), intLiteralParam("10"), body)},
	}, nil)
	got := runOneTriggerJASS(t, in)
	for _, want := range []string{
		"set bj_forLoopAIndex = 1",
		"exitwhen bj_forLoopAIndex > bj_forLoopAIndexEnd",
		"call DoNothing()",
		"set bj_forLoopAIndex = bj_forLoopAIndex + 1",
		"endloop",
	} {
		if !strings.Contains(got, want) {
			t.Fatalf("ForLoopA missing %q:\n%s", want, got)
		}
	}
}

func TestJASSCodegen_ForLoopVar(t *testing.T) {
	v := variableParam("i")
	body := wtg.Parameter{Type: wtg.ParamFunction, Value: "DoNothing", HasSubParameter: true,
		SubParameter: &wtg.ECA{Type: wtg.ECAAction, Name: "DoNothing", Enabled: true}}
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{makeECA("ForLoopVar", v, intLiteralParam("1"), intLiteralParam("10"), body)},
	}, []wtg.Variable{{Name: "i", Type: "integer"}})
	got := runOneTriggerJASS(t, in)
	if !strings.Contains(got, "set udg_i = 1") || !strings.Contains(got, "set udg_i = udg_i + 1") {
		t.Fatalf("ForLoopVar shape wrong:\n%s", got)
	}
}

func TestJASSCodegen_IfThenElse(t *testing.T) {
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
	got := runOneTriggerJASS(t, in)
	if !strings.Contains(got, "if (Trig_TestTrig_Wc3Forge") {
		t.Fatalf("expected 'if (Trig_TestTrig_Wc3Forge…':\n%s", got)
	}
	if !strings.Contains(got, "endif") {
		t.Fatalf("expected JASS 'endif':\n%s", got)
	}
	if !strings.Contains(got, "returns boolean") {
		t.Fatalf("expected boolean-returning condition helper:\n%s", got)
	}
	if !strings.Contains(got, "call DoNothing()") {
		t.Fatalf("expected 'call DoNothing()' branch body:\n%s", got)
	}
}

func TestJASSCodegen_IfThenElseMultiple(t *testing.T) {
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
	got := runOneTriggerJASS(t, in)
	if !strings.Contains(got, "not (1 > 0)") {
		t.Fatalf("expected 'not (1 > 0)' in condition closure:\n%s", got)
	}
	if !regexp.MustCompile(`if \(Trig_TestTrig_Wc3Forge\d+\(\)\) then`).MatchString(got) {
		t.Fatalf("expected 'if (…closure…()) then':\n%s", got)
	}
}

func TestJASSCodegen_WaitForCondition(t *testing.T) {
	cond := wtg.Parameter{Type: wtg.ParamFunction, HasSubParameter: true, Value: "GetTriggeringTrigger",
		SubParameter: &wtg.ECA{Type: wtg.ECACall, Name: "GetTriggeringTrigger", Enabled: true}}
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{makeECA("WaitForCondition", cond, intLiteralParam("0.1"))},
	}, nil)
	got := runOneTriggerJASS(t, in)
	if !strings.Contains(got, "exitwhen GetTriggeringTrigger()") {
		t.Fatalf("expected 'exitwhen GetTriggeringTrigger()':\n%s", got)
	}
	if !strings.Contains(got, "call TriggerSleepAction(RMaxBJ(bj_WAIT_FOR_COND_MIN_INTERVAL, 0.1))") {
		t.Fatalf("expected TriggerSleepAction body:\n%s", got)
	}
}

func TestJASSCodegen_AddTriggerEvent(t *testing.T) {
	getTrig := wtg.Parameter{Type: wtg.ParamFunction, HasSubParameter: true, Value: "GetTriggeringTrigger",
		SubParameter: &wtg.ECA{Type: wtg.ECACall, Name: "GetTriggeringTrigger", Enabled: true}}
	eventBind := wtg.Parameter{Type: wtg.ParamFunction, HasSubParameter: true, Value: "GetUnitLoc",
		SubParameter: &wtg.ECA{Type: wtg.ECACall, Name: "GetUnitLoc", Enabled: true,
			Parameters: []wtg.Parameter{variableParam("someUnit")}}}
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{makeECA("AddTriggerEvent", getTrig, eventBind)},
	}, []wtg.Variable{{Name: "someUnit", Type: "unit"}})
	got := runOneTriggerJASS(t, in)
	if !strings.Contains(got, "call GetUnitLoc(GetTriggeringTrigger(), udg_someUnit)") {
		t.Fatalf("expected injected first-arg with call prefix:\n%s", got)
	}
}

func TestJASSCodegen_CustomScriptCodeInlinedVerbatim(t *testing.T) {
	in := buildInputs(t, wtg.Trigger{
		ID: 1, Name: "T", IsEnabled: true, InitiallyOn: true,
		ECAs: []wtg.ECA{makeECA("CustomScriptCode", stringLiteralParam("call BJDebugMsg(\"hi\")"))},
	}, nil)
	got := runOneTriggerJASS(t, in)
	if !strings.Contains(got, `call BJDebugMsg("hi")`) {
		t.Fatalf("expected raw JASS custom-script inlined:\n%s", got)
	}
}

func TestJASSCodegen_RawcodeLiteral(t *testing.T) {
	// FourCC code-type params emit a 'xxxx' rawcode literal, not FourCC("xxxx").
	cg := &jassCodegen{in: CodegenInputs{TD: stubTriggerData(), Triggers: &wtg.Triggers{}}}
	got, err := cg.resolveParameter(&wtg.Parameter{Type: wtg.ParamString, Value: "hfoo"}, "T", "unitcode", false)
	if err != nil {
		t.Fatalf("resolveParameter: %v", err)
	}
	if got != "'hfoo'" {
		t.Fatalf("expected rawcode 'hfoo', got %q", got)
	}
}

func TestJASSCodegen_RefusesLuaMap(t *testing.T) {
	_, err := GenerateJASSScript(CodegenInputs{
		Triggers: &wtg.Triggers{},
		Info:     &w3i.Info{Lua: true},
		TD:       stubTriggerData(),
	})
	if err == nil {
		t.Fatalf("expected error when GenerateJASSScript called on a Lua map")
	}
}

func TestScriptNeedsRegen(t *testing.T) {
	const jassMarker = JASSCodegenMarkerLine + "\nglobals\nendglobals\n"
	const luaMarker = CodegenMarkerLine + "\n"
	const foreign = "// hand-authored or JassHelper output\nfunction main takes nothing returns nothing\nendfunction\n"
	cases := []struct {
		name           string
		handRolled     bool
		triggersEdited bool
		isLua          bool
		existing       string
		existingOK     bool
		want           bool
	}{
		{"hand-rolled never regen", true, true, false, "", false, false},
		{"edited triggers → regen (jass)", false, true, false, foreign, true, true},
		{"edited triggers → regen (lua)", false, true, true, foreign, true, true},
		{"unedited + foreign jass → PRESERVE", false, false, false, foreign, true, false},
		{"unedited + foreign lua → PRESERVE", false, false, true, foreign, true, false},
		{"unedited + codegen jass → regen", false, false, false, jassMarker, true, true},
		{"unedited + codegen lua → regen", false, false, true, luaMarker, true, true},
		{"unedited + absent script → regen", false, false, false, "", false, true},
		{"unedited + empty script → regen", false, false, false, "", true, true},
	}
	for _, c := range cases {
		t.Run(c.name, func(t *testing.T) {
			got := scriptNeedsRegen(c.handRolled, c.triggersEdited, c.isLua, []byte(c.existing), c.existingOK)
			if got != c.want {
				t.Fatalf("scriptNeedsRegen = %v, want %v", got, c.want)
			}
		})
	}
}

func TestJASSCodegen_FullScriptShape(t *testing.T) {
	in := CodegenInputs{
		Info: &w3i.Info{Lua: false, Name: "TestMap", Description: "desc",
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
					CustomText: "function ScriptThing_extra takes nothing returns nothing\n\tcall BJDebugMsg(\"hi\")\nendfunction\n",
				},
			},
		},
	}
	out, err := GenerateJASSScript(in)
	if err != nil {
		t.Fatalf("GenerateJASSScript: %v", err)
	}
	mustContain := []string{
		"// generated by wc3-forge",
		"globals",
		"integer udg_counter = 0",
		"string array udg_names",
		"trigger gg_trg_OnInit = null",
		"endglobals",
		"function InitGlobals takes nothing returns nothing",
		"function CreateAllUnits takes nothing returns nothing",
		"function CreateAllDestructables takes nothing returns nothing",
		"function InitTrig_OnInit takes nothing returns nothing",
		"set gg_trg_OnInit = CreateTrigger()",
		"function InitTrig_Loop takes nothing returns nothing",
		"function InitCustomTriggers takes nothing returns nothing",
		"\tcall InitTrig_OnInit()",
		"function RunInitializationTriggers takes nothing returns nothing",
		"\tcall ConditionalTriggerExecute(gg_trg_OnInit)",
		"function ScriptThing_extra takes nothing returns nothing", // custom JASS inlined verbatim
		"function main takes nothing returns nothing",
		"function config takes nothing returns nothing",
		`call SetMapName("TestMap")`,
	}
	for _, sub := range mustContain {
		if !strings.Contains(out, sub) {
			t.Errorf("missing %q in generated JASS", sub)
		}
	}
	// No Lua-isms should leak in.
	for _, banned := range []string{"__jarray", " ~= ", " .. ", "= nil\n"} {
		if strings.Contains(out, banned) {
			t.Errorf("unexpected Lua-ism %q in generated JASS", banned)
		}
	}
	if testing.Verbose() {
		t.Logf("generated JASS:\n%s", out)
	}
}
