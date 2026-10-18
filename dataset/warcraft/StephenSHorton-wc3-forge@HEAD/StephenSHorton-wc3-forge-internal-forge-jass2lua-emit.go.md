---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/jass2lua/emit.go"
ref: HEAD
---

package jass2lua

import (
	"fmt"
	"strings"
)

// TranspileScript translates a JASS source string to Lua.
//
// Conservative on errors: any unparseable construct produces a
// `--[[ jass2lua: <reason> ]] error("jass2lua failed")` placeholder so the
// rest of the file still translates and the user sees something actionable
// in the diff.
//
// NOTE: this returns a non-nil `err` ONLY for unrecoverable lex errors. Per-
// statement parse failures are recovered into RawStmt placeholders and do NOT
// surface here — callers that need to know how many statements failed must use
// TranspileScriptWithErrors, which returns the structured File.Errors list.
// This signature is preserved so the many existing callers/tests that treat a
// nil err as "clean enough to translate" keep their behavior.
func TranspileScript(jass string) (lua string, err error) {
	lua, _, err = TranspileScriptWithErrors(jass)
	return lua, err
}

// TranspileScriptWithErrors is the structured-diagnostics variant of
// TranspileScript. It returns the emitted Lua, the list of recovered parse
// errors (File.Errors — one entry per statement/top-level node the parser
// could not parse and turned into a RawStmt/error() placeholder), and a
// separate hard lex error.
//
// This is the source of truth for surfacing the REAL failure count to the
// convert dialog: each parse error here corresponds 1:1 to an inline
// `error("jass2lua failed: ...")` marker the emitter writes, so the section's
// reported error count finally matches reality instead of under-reporting to 0.
func TranspileScriptWithErrors(jass string) (lua string, parseErrors []string, lexErr error) {
	toks, err := Tokenize(jass)
	if err != nil {
		// Lex errors can't be recovered (we have no token stream to walk).
		// Surface the whole input as a placeholder so the diff still renders.
		return fmt.Sprintf("--[[ jass2lua lex error: %s ]]\nerror(%q)\n", err.Error(), "jass2lua failed: "+err.Error()), nil, err
	}
	file := Parse(toks)
	em := &emitter{}
	em.emitFile(file)
	return em.b.String(), file.Errors, nil
}

// TranspileFunction translates a JASS fragment for a single trigger's custom_text.
// Many maps' custom_text blocks contain bare statements (no surrounding
// function/endfunction), so this auto-detects the shape:
//
//   - If the input contains a top-level `function` keyword, treat as full
//     script and delegate to TranspileScript.
//   - Otherwise wrap as a body (`function Name() ... end`) named after `name`.
//
// Returns conservative output on errors (same contract as TranspileScript):
// a non-nil err when lexing failed OR when statement-list parsing produced
// recovered soft errors (joined with "; ").
func TranspileFunction(name, jass string) (lua string, err error) {
	lua, parseErrors, lexErr := TranspileFunctionWithErrors(name, jass)
	if lexErr != nil {
		return lua, lexErr
	}
	if len(parseErrors) > 0 {
		return lua, fmt.Errorf("%s", strings.Join(parseErrors, "; "))
	}
	return lua, nil
}

// TranspileFunctionWithErrors is the structured-diagnostics variant of
// TranspileFunction. It returns the emitted Lua, the per-statement parse-error
// list (one entry per RawStmt/error() marker emitted), and a separate hard lex
// error.
//
//   - Full-script fragment (top-level function/globals present) → delegate to
//     TranspileScriptWithErrors so its File.Errors surface too.
//   - Bare-statement fragment → auto-wrap as `function Name() ... end` and
//     collect the statement-list parser's soft errors.
//
// The convert orchestrator uses this so bare-statement custom_text / script
// sections report their REAL failure count (previously these recovered errors
// were collapsed into a single joined string or dropped entirely).
func TranspileFunctionWithErrors(name, jass string) (lua string, parseErrors []string, lexErr error) {
	if hasTopLevelFunction(jass) {
		return TranspileScriptWithErrors(jass)
	}
	// Auto-wrap as function body.
	toks, err := Tokenize(jass)
	if err != nil {
		return fmt.Sprintf("--[[ jass2lua lex error: %s ]]\nfunction %s()\n\terror(%q)\nend\n", err.Error(), sanitizeName(name), "jass2lua failed: "+err.Error()), nil, err
	}
	// Synthesize a wrapping function decl by manual statement-list parse.
	p := &parser{toks: toks}
	stmts, _, _ := p.parseStmtsUntilEOF()
	em := &emitter{}
	// Body-fragment scope: locals declared inside become entries in localTypes
	// (emitLocal registers them) so the integer-division fix can classify uses
	// of typed locals here too. No globals/params are visible in a fragment.
	em.localTypes = map[string]string{}
	em.writeLine(fmt.Sprintf("function %s()", sanitizeName(name)))
	em.indent++
	for _, s := range stmts {
		em.emitStmt(s)
	}
	em.indent--
	em.writeLine("end")
	// Filter empty / whitespace-only entries from the soft-error list —
	// otherwise an upstream bug pushing a blank entry would leak into the
	// transpile-preview UI as a blank diagnostic bullet. Phase 5 fix.
	cleaned := make([]string, 0, len(p.errs))
	for _, msg := range p.errs {
		if strings.TrimSpace(msg) == "" {
			continue
		}
		cleaned = append(cleaned, msg)
	}
	if len(cleaned) > 0 {
		return em.b.String(), cleaned, nil
	}
	return em.b.String(), nil, nil
}

// HasTopLevelDecl reports whether the source contains a top-level JASS
// declaration (function / globals / native). It is the exported form of
// hasTopLevelFunction, used by the convert orchestrator to decide whether a
// "script"-classified trigger body is a real full script (→ TranspileScript)
// or a bare statement list (→ TranspileFunction body-fragment parser). See
// triggers_convert.go's transpileSectionScript (P0#2).
func HasTopLevelDecl(jass string) bool { return hasTopLevelFunction(jass) }

// hasTopLevelFunction is a cheap pre-check used by TranspileFunction to decide
// whether the input is a full script or a body-only fragment. False positive
// at the line-start level is acceptable (worst case: we treat a body fragment
// as a script and the parser falls back to RawStmt for any non-decl).
func hasTopLevelFunction(jass string) bool {
	for _, line := range strings.Split(jass, "\n") {
		trim := strings.TrimSpace(line)
		if strings.HasPrefix(trim, "function ") || strings.HasPrefix(trim, "globals") ||
			strings.HasPrefix(trim, "constant function ") || strings.HasPrefix(trim, "native ") ||
			strings.HasPrefix(trim, "constant native ") {
			return true
		}
	}
	return false
}

// parseStmtsUntilEOF is the body-fragment entry — keeps parsing until TokEOF.
// Returns the collected statements, "" (no terminator), and any hard error.
func (p *parser) parseStmtsUntilEOF() ([]Stmt, string, error) {
	var out []Stmt
	for {
		if p.peek().Kind == TokEOL {
			p.advance()
			continue
		}
		if p.peek().Kind == TokEOF {
			return out, "", nil
		}
		startPos := p.pos
		s, err := p.parseStmt()
		if err != nil {
			p.errs = append(p.errs, err.Error())
			snippet := p.snippetFrom(startPos)
			out = append(out, &RawStmt{Reason: err.Error(), Snippet: snippet})
			p.skipToEOL()
			continue
		}
		out = append(out, s)
	}
}

// sanitizeName strips characters that can't appear in a Lua identifier so the
// auto-wrap shim doesn't produce malformed Lua.
func sanitizeName(s string) string {
	var b strings.Builder
	for i, r := range s {
		ok := r == '_' || (r >= 'a' && r <= 'z') || (r >= 'A' && r <= 'Z')
		if i > 0 && r >= '0' && r <= '9' {
			ok = true
		}
		if ok {
			b.WriteRune(r)
		} else {
			b.WriteByte('_')
		}
	}
	out := b.String()
	if out == "" || (out[0] >= '0' && out[0] <= '9') {
		out = "Trig_" + out
	}
	return out
}

// ---------------------------------------------------------------------------
// Emitter.
// ---------------------------------------------------------------------------

type emitter struct {
	b      strings.Builder
	indent int

	// Minimal type tables for the two semantic fixes (P1#3 integer division,
	// P1#4 typed-array defaults). The transpiler has no real type system, so
	// these record only the JASS-declared types we can see directly:
	//
	//   - globalTypes : name -> JASS type, from the globals block (file scope).
	//   - localTypes  : name -> JASS type, from the CURRENT function's params +
	//                   local decls. Rebuilt per function in emitFuncDecl and
	//                   cleared when the function ends.
	//
	// inferExprType walks an expression bottom-up against these tables (plus
	// literal typing and a small known-native return-type map) and returns one
	// of "integer" / "real" / "" (unknown). When it can't prove both operands
	// of a `/` are integers, the division stays float -- conservative by design.
	globalTypes map[string]string
	localTypes  map[string]string
}

// typeInteger / typeReal are the only two numeric classifications the division
// fix cares about. "" means "unknown / non-numeric" -- treated as not-provably-
// integer, so a `/` involving it stays float.
const (
	typeInteger = "integer"
	typeReal    = "real"
)

// typeString is the third classification, added for the string-concat fix
// (P2#9 C1). It is NOT a numeric type -- numericType() still maps it to "" so
// the integer-division path is completely unaffected. inferExprType returns it
// for string literals, string-typed vars/params/globals, and string-returning
// known natives, which lets a literal-free `strA + strB` emit `..` instead of
// the wrong Lua `+`.
const typeString = "string"

// knownNativeReturns maps a small, safe set of stock WC3 natives/BJs to their
// return type so expressions like `R2I(x) / n` or `GetUnitState(...) / 2` can
// be classified. Intentionally tiny and high-confidence: an unlisted function
// yields an unknown type, which keeps its enclosing division as float. Adding
// a name here can only ever turn a float `/` into a (correct) integer `/` when
// BOTH operands are provably integer -- never the other way.
//
// String-returning entries (P2#9 C1) are likewise high-confidence stock
// converters/getters. They feed ONLY the `+` -> `..` concat decision; an
// unlisted function stays unknown, so a `+` involving it keeps `+`.
var knownNativeReturns = map[string]string{
	// Integer-returning natives/BJs.
	"R2I":                 typeInteger,
	"StringLength":        typeInteger,
	"GetRandomInt":        typeInteger,
	"GetPlayerId":         typeInteger,
	"GetHandleId":         typeInteger,
	"GetUnitTypeId":       typeInteger,
	"GetTriggerEvalCount": typeInteger,
	"ModuloInteger":       typeInteger,
	"GetHeroLevel":        typeInteger,
	"GetUnitLevel":        typeInteger,
	"S2I":                 typeInteger,
	// Real-returning natives/BJs.
	"I2R":           typeReal,
	"GetRandomReal": typeReal,
	"GetUnitState":  typeReal,
	"GetWidgetLife": typeReal,
	"ModuloReal":    typeReal,
	"SquareRoot":    typeReal,
	"Pow":           typeReal,
	"Sin":           typeReal,
	"Cos":           typeReal,
	"Tan":           typeReal,
	"S2R":           typeReal,
	// String-returning natives/BJs (P2#9 C1). Only used to route `+` -> `..`.
	"I2S":                      typeString,
	"R2S":                      typeString,
	"R2SW":                     typeString,
	"GetUnitName":              typeString,
	"GetHeroProperName":        typeString,
	"GetPlayerName":            typeString,
	"GetObjectName":            typeString,
	"GetLocalizedString":       typeString,
	"SubString":                typeString,
	"GetEventPlayerChatString": typeString,
}

// lookupVarType resolves an identifier's JASS type from the local table first
// (a local/param shadows a global of the same name in JASS), then globals.
// Returns "" when unknown.
func (e *emitter) lookupVarType(name string) string {
	if e.localTypes != nil {
		if t, ok := e.localTypes[name]; ok {
			return t
		}
	}
	if e.globalTypes != nil {
		if t, ok := e.globalTypes[name]; ok {
			return t
		}
	}
	return ""
}

// inferExprType classifies an expression as "integer", "real", or "" (unknown /
// non-numeric). It is deliberately conservative: any node it cannot prove is
// integer-or-real returns "". This is the whole basis of the integer-division
// fix -- only a division whose BOTH operands infer to "integer" is rewritten.
func (e *emitter) inferExprType(x Expr) string {
	switch n := x.(type) {
	case *IntLit:
		return typeInteger
	case *RealLit:
		return typeReal
	case *RawLit:
		// A JASS rawcode ('hpea') is an integer; emitted as FourCC("..."),
		// whose WC3-Lua return is an integer.
		return typeInteger
	case *StringLit:
		return typeString
	case *BoolLit, *NullLit:
		return "" // non-numeric, non-string
	case *Ident:
		return classifyType(e.lookupVarType(n.Name))
	case *CallExpr:
		return classifyType(knownNativeReturns[n.Func])
	case *IndexExpr:
		// Array element type == the array variable's declared element type.
		if base, ok := n.Base.(*Ident); ok {
			return classifyType(e.lookupVarType(base.Name))
		}
		return ""
	case *UnaryExpr:
		switch n.Op {
		case "-", "~":
			// Arithmetic / bitwise unary preserves numeric type.
			return e.inferExprType(n.X)
		}
		return "" // `not` -> boolean
	case *BinaryExpr:
		return e.inferBinaryType(n)
	case *TernaryExpr:
		// Both arms should share a type; only commit when they agree.
		tt := e.inferExprType(n.Then)
		et := e.inferExprType(n.Else)
		if tt == et {
			return tt
		}
		return ""
	}
	return ""
}

// inferBinaryType infers the result type of a binary expression. Comparison /
// logical operators are boolean (-> ""). Arithmetic follows JASS promotion:
// int op int -> int; anything involving a real (and only reals/ints) -> real.
// `/` likewise (its emitted form differs, but the TYPE it yields is int when
// both sides are int). Unknown operands poison the result to "".
func (e *emitter) inferBinaryType(n *BinaryExpr) string {
	switch n.Op {
	case "==", "!=", "<", "<=", ">", ">=", "and", "or":
		return "" // boolean
	}
	lt := e.inferExprType(n.L)
	rt := e.inferExprType(n.R)
	switch n.Op {
	case "+":
		// `+` is overloaded in JASS: numeric add OR string concat. If EITHER
		// operand is provably string the whole expression is a string (so a
		// chain `a + b + c` of string parts classifies as string and routes
		// through `..`). Otherwise fall through to the numeric rules below.
		if lt == typeString || rt == typeString {
			return typeString
		}
		fallthrough
	case "-", "*", "/", "%", "|", "&", "^", "<<", ">>":
		if lt == typeInteger && rt == typeInteger {
			return typeInteger
		}
		if (lt == typeInteger || lt == typeReal) && (rt == typeInteger || rt == typeReal) {
			return typeReal
		}
		return ""
	}
	return ""
}

// numericType normalizes a JASS type name to "integer" / "real" / "". Only the
// two numeric primitives matter for the division fix; every other type (string,
// boolean, handle subtypes, code, unknown) is non-numeric -> "". This is the
// classifier the integer-division path uses, so it MUST keep treating string as
// "" -- otherwise a `/` could misfire on a string-typed operand.
func numericType(t string) string {
	switch t {
	case typeInteger:
		return typeInteger
	case typeReal:
		return typeReal
	}
	return ""
}

// classifyType normalizes a JASS type name to one of "integer" / "real" /
// "string" / "" (unknown / other). It is the superset classifier used by
// inferExprType for var/native lookups: it adds the string case on top of
// numericType so the concat fix can route `+` -> `..`. Every non-numeric,
// non-string type (boolean, handle subtypes, code, user structs, unknown)
// stays "" -- conservative, so an ambiguous `+` keeps Lua `+`.
func classifyType(t string) string {
	if t == typeString {
		return typeString
	}
	return numericType(t)
}

func (e *emitter) writeIndent() {
	for i := 0; i < e.indent; i++ {
		e.b.WriteByte('\t')
	}
}

func (e *emitter) writeLine(s string) {
	e.writeIndent()
	e.b.WriteString(s)
	e.b.WriteByte('\n')
}

// defaultInitForType returns the Lua literal a global of the given JASS type
// should initialize to when there's no explicit initializer.
func defaultInitForType(t string) string {
	switch t {
	case "integer":
		return "0"
	case "real":
		return "0.0"
	case "string":
		return `""`
	case "boolean":
		return "false"
	case "code":
		return "nil"
	}
	// All handle subtypes default to nil.
	return "nil"
}

// arrayInitForType returns the Lua initializer for a JASS array of the given
// element type (P1#4). Numeric/boolean arrays get a __jarray(default) so reads
// of unset indices return the JASS zero value (0 / 0.0 / false) instead of a
// bare Lua `{}`'s nil -- which crashes "arithmetic on a nil value" the first
// time an untouched slot is read (e.g. `FearCounter[ud] + 1` on a fresh ud).
// Everything else (handles, string, code, unknown user types) keeps a bare
// `{}` -- their JASS default is null, and Lua's nil-for-unset matches exactly,
// so no metatable is needed.
//
// __jarray is part of the WC3 Lua runtime (blizzard.lua):
//
//	function __jarray(default) return setmetatable({}, {__index = function() return default end}) end
//
// The GUI codegen (triggers_codegen.go) already emits udg_* arrays via
// __jarray, so this stays consistent with the rest of the generated script.
func arrayInitForType(t string) string {
	switch t {
	case "integer":
		return "__jarray(0)"
	case "real":
		return "__jarray(0.0)"
	case "boolean":
		return "__jarray(false)"
	}
	// string / code / handle subtypes / unknown user types -> null default,
	// which Lua's nil-for-unset already provides.
	return "{}"
}

func (e *emitter) emitFile(f *File) {
	// First pass: collect globals' declared types so the division/array fixes
	// can classify references to file-scope variables anywhere below.
	e.collectGlobalTypes(f)
	for _, item := range f.Items {
		switch n := item.(type) {
		case *CommentStmt:
			e.writeLine("--" + n.Text)
		case *GlobalsBlock:
			e.emitGlobalsBlock(n)
		case *FuncDecl:
			e.emitFuncDecl(n)
		case *TypeDecl:
			e.writeLine(fmt.Sprintf("-- type %s extends %s (no Lua equivalent; declared for parity)", n.Name, n.Extends))
		case *RawStmt:
			e.emitRaw(n)
		case *StrayBlock:
			e.emitStrayBlock(n)
		default:
			e.writeLine(fmt.Sprintf("-- jass2lua: unhandled top-level node %T", item))
		}
	}
}

// emitStrayBlock writes a recovered run of file-scope statements (B1). The run
// is wrapped in a Lua `do ... end` so the emitted statements stay syntactically
// valid at chunk scope, prefixed by a single diagnostic comment. The body
// statements transpile through the normal emitStmt path, so well-formed
// statements produce correct Lua even though the run as a whole was a recovery.
//
// A fresh localTypes scope is used while emitting the body so the
// integer-division/array-default fixes can classify locals declared inside the
// injected run (these statements have no enclosing function header to seed it).
func (e *emitter) emitStrayBlock(sb *StrayBlock) {
	e.writeLine("-- [[ jass2lua: " + sb.Reason + " ]]")
	e.writeLine("do")
	e.indent++
	saved := e.localTypes
	e.localTypes = map[string]string{}
	for _, s := range sb.Body {
		e.emitStmt(s)
	}
	e.localTypes = saved
	e.indent--
	e.writeLine("end")
}

// collectGlobalTypes records every globals-block declaration's name -> JASS type
// so inferExprType can resolve file-scope variable references. Cheap, runs once
// per file before emission. Array globals are recorded under their ELEMENT
// type, which is exactly what an IndexExpr read should infer to.
func (e *emitter) collectGlobalTypes(f *File) {
	if e.globalTypes == nil {
		e.globalTypes = map[string]string{}
	}
	for _, item := range f.Items {
		gb, ok := item.(*GlobalsBlock)
		if !ok {
			continue
		}
		for _, d := range gb.Decls {
			e.globalTypes[d.Name] = d.Type
		}
	}
}

func (e *emitter) emitGlobalsBlock(g *GlobalsBlock) {
	for _, d := range g.Decls {
		// Constant: emit a comment marker since WC3's Lua is 5.3 (no <const>).
		prefix := ""
		if d.Constant {
			prefix = "-- constant\n"
		}
		var init string
		if d.Array {
			// P1#4: JASS arrays default unset slots to the element type's zero
			// value (0 / 0.0 / false / null). A bare Lua `{}` returns nil for
			// unset indices. arrayInitForType emits __jarray(default) for
			// numeric/boolean element types so reads of untouched indices yield
			// the JASS default; non-numeric element types keep `{}` (null
			// default == Lua nil).
			init = arrayInitForType(d.Type)
		} else if d.Init != nil {
			init = e.emitExpr(d.Init)
		} else {
			init = defaultInitForType(d.Type)
		}
		if prefix != "" {
			e.writeIndent()
			e.b.WriteString(prefix)
		}
		e.writeLine(fmt.Sprintf("%s = %s", d.Name, init))
	}
}

func (e *emitter) emitFuncDecl(f *FuncDecl) {
	if f.Native {
		// Natives can't be redeclared in Lua-only WC3 maps (they're bound at
		// runtime by the engine). Emit as a comment so the diff is still
		// informative.
		var params []string
		for _, p := range f.Params {
			params = append(params, p.Name)
		}
		e.writeLine(fmt.Sprintf("-- native %s(%s) returns %s", f.Name, strings.Join(params, ", "), retName(f.Returns)))
		return
	}
	var params []string
	for _, p := range f.Params {
		params = append(params, p.Name)
	}
	// Build the per-function local/param type table for inferExprType. Params
	// are known up front; locals are added as their `local` decls are emitted
	// (emitLocal registers them) so a use after the decl resolves correctly.
	e.localTypes = make(map[string]string, len(f.Params))
	for _, p := range f.Params {
		e.localTypes[p.Name] = p.Type
	}
	e.writeLine(fmt.Sprintf("function %s(%s)", f.Name, strings.Join(params, ", ")))
	e.indent++
	for _, s := range f.Body {
		e.emitStmt(s)
	}
	e.indent--
	e.writeLine("end")
	e.b.WriteByte('\n')
	// Clear so a following declaration doesn't see this function's locals.
	e.localTypes = nil
}

func retName(r string) string {
	if r == "" {
		return "nothing"
	}
	return r
}

func (e *emitter) emitStmt(s Stmt) {
	switch n := s.(type) {
	case *CommentStmt:
		e.writeLine("--" + n.Text)
	case *LocalStmt:
		e.emitLocal(n.Decl)
	case *SetStmt:
		e.writeLine(fmt.Sprintf("%s = %s", e.emitLhs(n.Target), e.emitExpr(n.Value)))
	case *CallStmt:
		e.writeLine(fmt.Sprintf("%s(%s)", n.Func, e.emitArgList(n.Args)))
	case *MemberCallStmt:
		// recv.method(args) or recv:method(args) depending on Colon flag.
		sep := "."
		if n.Call.Colon {
			sep = ":"
		}
		e.writeLine(fmt.Sprintf("%s%s%s(%s)", e.emitExpr(n.Call.Recv), sep, n.Call.Method, e.emitArgList(n.Call.Args)))
	case *IfStmt:
		e.emitIf(n)
	case *LoopStmt:
		e.emitLoop(n)
	case *ExitWhenStmt:
		// `exitwhen cond` → `if cond then break end`.
		e.writeLine(fmt.Sprintf("if %s then break end", e.emitExpr(n.Cond)))
	case *ReturnStmt:
		if n.Value == nil {
			e.writeLine("return")
		} else {
			e.writeLine("return " + e.emitExpr(n.Value))
		}
	case *RawStmt:
		e.emitRaw(n)
	default:
		e.writeLine(fmt.Sprintf("-- jass2lua: unhandled stmt %T", s))
	}
}

func (e *emitter) emitLocal(d LocalDecl) {
	// Register the local's declared type so later expressions in this function
	// can classify references to it (drives the integer-division fix). Stored
	// under the element type for arrays -- an IndexExpr read infers to that.
	if e.localTypes != nil {
		e.localTypes[d.Name] = d.Type
	}
	if d.Array {
		// Typed default init (P1#4) so reads of unset indices return the JASS
		// zero value instead of nil (same rationale as the globals-array path).
		init := arrayInitForType(d.Type)
		if init == "{}" {
			e.writeLine(fmt.Sprintf("local %s = {} -- jass: %s array", d.Name, d.Type))
		} else {
			e.writeLine(fmt.Sprintf("local %s = %s -- jass: %s array", d.Name, init, d.Type))
		}
		return
	}
	if d.Init == nil {
		e.writeLine(fmt.Sprintf("local %s = %s", d.Name, defaultInitForType(d.Type)))
		return
	}
	e.writeLine(fmt.Sprintf("local %s = %s", d.Name, e.emitExpr(d.Init)))
}

func (e *emitter) emitIf(s *IfStmt) {
	e.writeLine(fmt.Sprintf("if %s then", e.emitExpr(s.Cond)))
	e.indent++
	for _, st := range s.Then {
		e.emitStmt(st)
	}
	e.indent--
	for _, ef := range s.Elifs {
		e.writeLine(fmt.Sprintf("elseif %s then", e.emitExpr(ef.Cond)))
		e.indent++
		for _, st := range ef.Body {
			e.emitStmt(st)
		}
		e.indent--
	}
	if s.Else != nil {
		e.writeLine("else")
		e.indent++
		for _, st := range s.Else {
			e.emitStmt(st)
		}
		e.indent--
	}
	e.writeLine("end")
}

func (e *emitter) emitLoop(s *LoopStmt) {
	e.writeLine("while true do")
	e.indent++
	for _, st := range s.Body {
		e.emitStmt(st)
	}
	e.indent--
	e.writeLine("end")
}

func (e *emitter) emitRaw(r *RawStmt) {
	// Multi-line snippet → single-line comment. Replace ]] to avoid breaking
	// the comment block.
	snippet := strings.ReplaceAll(r.Snippet, "]]", "] ]")
	e.writeLine(fmt.Sprintf("--[[ jass2lua: %s | source: %s ]]", r.Reason, snippet))
	e.writeLine(fmt.Sprintf("error(%q)", "jass2lua failed: "+r.Reason))
}

func (e *emitter) emitLhs(l Lhs) string {
	switch n := l.(type) {
	case *Ident:
		return n.Name
	case *IndexExpr:
		return e.emitExpr(n.Base) + "[" + e.emitExpr(n.Index) + "]"
	case *MemberExpr:
		// LHS member access — always dot in Lua (colon is call-only).
		return e.emitExpr(n.Base) + "." + n.Member
	}
	return "--[[lhs?]]"
}

func (e *emitter) emitArgList(args []Expr) string {
	parts := make([]string, 0, len(args))
	for _, a := range args {
		parts = append(parts, e.emitExpr(a))
	}
	return strings.Join(parts, ", ")
}

// emitExpr translates an expression to its Lua text form.
func (e *emitter) emitExpr(x Expr) string {
	switch n := x.(type) {
	case *IntLit:
		return n.Value
	case *RealLit:
		return n.Value
	case *StringLit:
		return luaStringLit(n.Value)
	case *BoolLit:
		if n.Value {
			return "true"
		}
		return "false"
	case *NullLit:
		return "nil"
	case *RawLit:
		// FourCC literal — WC3-Lua standard shim.
		return fmt.Sprintf(`FourCC(%q)`, n.Value)
	case *Ident:
		return n.Name
	case *IndexExpr:
		return e.emitExpr(n.Base) + "[" + e.emitExpr(n.Index) + "]"
	case *CallExpr:
		return fmt.Sprintf("%s(%s)", n.Func, e.emitArgList(n.Args))
	case *MemberExpr:
		// Field access — Lua dot syntax (Colon is call-only and ignored
		// here; the parser sets Colon=false for bare field access).
		return e.emitExpr(n.Base) + "." + n.Member
	case *MemberCallExpr:
		sep := "."
		if n.Colon {
			sep = ":"
		}
		return fmt.Sprintf("%s%s%s(%s)", e.emitExpr(n.Recv), sep, n.Method, e.emitArgList(n.Args))
	case *UnaryExpr:
		op := n.Op
		if op == "not" {
			return "not " + e.emitExprWithParens(n.X)
		}
		// Bitwise not (`~`) and unary minus emit straight through — Lua 5.3+
		// supports both with identical spelling.
		return op + e.emitExprWithParens(n.X)
	case *BinaryExpr:
		// String concat (P2#9 C1): JASS overloads `+` for numeric add AND
		// string concat; Lua needs `..` for the latter. Emit `..` when EITHER
		// operand is provably a string. Two independent signals:
		//   1. Literal hint (isStringExpr) -- a StringLit anywhere in the
		//      operand subtree. Preserved verbatim from the original heuristic
		//      so one-string-literal concats emit EXACTLY as before.
		//   2. Type inference -- an operand whose inferred type is `string`
		//      (string-typed var/param/global, or a string-returning known
		//      native like I2S/GetUnitName). This is what catches the
		//      literal-free `strA + strB` that previously emitted a wrong `+`.
		// Conservative: an operand of unknown type contributes neither signal,
		// so an ambiguous `+` still emits `+`.
		op := n.Op
		if op == "+" && (isStringExpr(n.L) || isStringExpr(n.R) ||
			e.inferExprType(n.L) == typeString || e.inferExprType(n.R) == typeString) {
			op = ".."
		} else if op == "/" && e.inferExprType(n.L) == typeInteger && e.inferExprType(n.R) == typeInteger {
			// Integer division (P1#3): JASS `/` between two integers truncates
			// TOWARD ZERO; Lua `/` is always float (so `a - (a/b)*b` returns
			// 0.0 for all inputs instead of the remainder). Lua's `//` FLOORS
			// toward -inf and DIFFERS for negatives (-7//2 == -4, but JASS
			// -7/2 == -3), so we can't use it. R2I truncates toward zero
			// exactly like JASS, so wrap the float division:
			// int/int `a / b` -> `R2I(a / b)`. R2I is a stock WC3 Lua native.
			// Mixed int/real falls through to plain float `/`.
			return "R2I(" + e.emitExprWithParens(n.L) + " / " + e.emitExprWithParens(n.R) + ")"
		} else {
			op = mapBinaryOp(op)
		}
		return e.emitExprWithParens(n.L) + " " + op + " " + e.emitExprWithParens(n.R)
	case *TernaryExpr:
		// Lua has no native ternary. The classic lowering
		// `(cond and then) or else` has a well-known bug: when `then`
		// evaluates to `false` or `nil` (both falsy in Lua) the `and` yields
		// the falsy value and `or` then returns `else` -- the wrong branch.
		//
		// P2#9 C2 fixes this with a two-form strategy that does NO HARM to the
		// already-correct cases:
		//
		//   - SAFE form (provably-truthy `then`): keep the exact
		//     `((cond) and (then) or (else))` idiom. A `then` arm whose value
		//     can never be `false`/`nil` (integer/real/string literal or
		//     typed expr, FourCC, a non-bool unary/arith result, etc.) selects
		//     correctly, so its emitted text is byte-for-byte unchanged.
		//
		//   - CORRECT form (`then` could be false/nil): wrap both arms in a
		//     1-element table and index `[1]`:
		//        ((cond) and {then} or {else})[1]
		//     A table is ALWAYS truthy, so `and` never short-circuits on a
		//     falsy `then` and the correct branch's table is always chosen;
		//     `[1]` then unwraps the real value (false/nil included). This is
		//     self-contained -- no runtime/prelude helper needed, consistent
		//     with how the rest of the emitter avoids new runtime deps.
		//
		// Short-circuit / side-effect note: Lua's `and`/`or` still pick which
		// TABLE to construct, so only the taken arm's constructor expression is
		// actually evaluated at runtime -- matching JASS ternary semantics (the
		// non-taken arm does not run). The two-form split also means the common,
		// already-correct ternaries keep their exact prior text (do-no-harm).
		cond := e.emitExpr(n.Cond)
		then := e.emitExpr(n.Then)
		els := e.emitExpr(n.Else)
		if ternaryThenMayBeFalsy(n.Then, e.inferExprType) {
			// Correct table-wrap form. No caveat comment needed -- this form
			// returns `then` faithfully even when `then` is false/nil.
			return "((" + cond + ") and {" + then + "} or {" + els + "})[1]"
		}
		// Safe form -- provably-truthy `then`, emitted exactly as before.
		return "((" + cond + ") and (" + then + ") or (" + els + "))"
	}
	return "--[[expr?]]"
}

// emitExprWithParens conservatively parenthesizes nested binary / ternary
// expressions so the emitted Lua keeps source precedence. Leaf nodes don't
// need them.
func (e *emitter) emitExprWithParens(x Expr) string {
	switch x.(type) {
	case *BinaryExpr, *TernaryExpr:
		return "(" + e.emitExpr(x) + ")"
	}
	return e.emitExpr(x)
}

// ternaryThenMayBeFalsy reports whether a ternary's `then` arm could evaluate
// to Lua `false` or `nil` -- the two values that break the bare
// `(cond and then) or else` idiom. It returns:
//
//   - false  => `then` is PROVABLY truthy, so the cheap `and/or` idiom is
//     correct and we emit it unchanged (do-no-harm: byte-identical output for
//     the cases that were already right -- numeric/string/handle-create arms).
//   - true   => `then` might be false/nil (a boolean expr, a null, or anything
//     of unknown type, e.g. a function whose return we can't classify), so we
//     fall back to the correct table-wrap form.
//
// Conservative by construction: anything not provably truthy returns true.
func ternaryThenMayBeFalsy(x Expr, infer func(Expr) string) bool {
	switch n := x.(type) {
	case *IntLit, *RealLit, *RawLit, *StringLit:
		// Numeric/string/rawcode literals are always truthy in Lua (even 0 and
		// "" are truthy -- only false/nil are falsy).
		return false
	case *BoolLit:
		// `true` is truthy; `false` is the canonical broken case.
		return !n.Value
	case *NullLit:
		// -> Lua nil, falsy.
		return true
	case *UnaryExpr:
		switch n.Op {
		case "-", "~":
			// Arithmetic / bitwise unary -> number, always truthy.
			return false
		}
		// `not x` -> boolean, may be false.
		return true
	case *BinaryExpr:
		switch n.Op {
		case "==", "!=", "<", "<=", ">", ">=", "and", "or":
			// Comparison / logical -> boolean, may be false.
			return true
		}
		// Arithmetic / concat -> number or string, always truthy.
		return false
	}
	// Ident / CallExpr / IndexExpr / MemberExpr / nested ternary: rely on the
	// type lattice. A provably integer/real/string value is truthy; anything
	// unknown (could be a boolean native, a null handle, etc.) is treated as
	// possibly-falsy so we pick the correct form.
	switch infer(x) {
	case typeInteger, typeReal, typeString:
		return false
	}
	return true
}

// isStringExpr is the heuristic for the `+` → `..` concat rewrite. Recursive
// into Binary so chains of string concats route through `..`.
func isStringExpr(x Expr) bool {
	switch n := x.(type) {
	case *StringLit:
		return true
	case *BinaryExpr:
		return n.Op == "+" && (isStringExpr(n.L) || isStringExpr(n.R))
	}
	return false
}

// mapBinaryOp translates JASS comparison operators to Lua. JASS uses `!=`;
// Lua uses `~=`. Bitwise operators (Phase 5) pass through mostly identical
// for Lua 5.3+ EXCEPT bitwise XOR: Lua uses `~` (which is also Lua's bitwise
// not unary), whereas vJASS / C-style bitwise XOR is `^`. We translate `^` →
// `~` for binary positions; the parser distinguishes unary-`~` from binary-
// `~` by context so the round-trip stays consistent.
//
// `|` (bitwise or), `&` (bitwise and), `<<` / `>>` (shifts) pass through as-is
// — Lua 5.3+ uses the same spelling.
func mapBinaryOp(op string) string {
	switch op {
	case "!=":
		return "~="
	case "^":
		// JASS `^` (bitwise xor, vJASS extension) → Lua `~` (binary form).
		return "~"
	}
	return op
}

// luaStringLit double-quotes a Lua string literal, escaping the bytes Lua
// requires (\, ", \n, \r, \t).
func luaStringLit(s string) string {
	var b strings.Builder
	b.WriteByte('"')
	for i := 0; i < len(s); i++ {
		c := s[i]
		switch c {
		case '\\':
			b.WriteString(`\\`)
		case '"':
			b.WriteString(`\"`)
		case '\n':
			b.WriteString(`\n`)
		case '\r':
			b.WriteString(`\r`)
		case '\t':
			b.WriteString(`\t`)
		default:
			b.WriteByte(c)
		}
	}
	b.WriteByte('"')
	return b.String()
}

// ---------------------------------------------------------------------------
// vJASS detection — exported helpers used by triggers_convert.go's refined
// blocker check.
// ---------------------------------------------------------------------------

// vJASSKeywords is the set of vJASS-only keywords whose presence in a script
// indicates the source needs the JassHelper preprocessor before it can be
// transpiled. Maps detected keyword → human-readable label for the blocker
// reason.
//
// NOTE: textmacro / endtextmacro / runtextmacro / runtextmacroonce live in
// `//!` comment lines and are consumed by the Preprocess pass (preprocess.go)
// BEFORE this scan runs. They're intentionally absent from this list so a
// textmacro-only map (Phase 1 of vJASS support) flows through cleanly.
//
// Phase 2 added the library/scope preprocessor (PreprocessLibScope) which
// runs AFTER textmacro expansion. It consumes library / endlibrary / scope /
// endscope plus the requires/needs/uses/initializer/optional keywords AND
// strips private/public visibility prefixes from inner decls.
//
// Phase 3 added the struct preprocessor (PreprocessStructs) which strips
// struct/endstruct blocks (with method/endmethod, static, delegate, readonly,
// thistype, onInit inside them) and replaces each with a marker comment that
// the codegen splices the emitted Lua at.
//
// Phase 4 added module / interface / define handling. The keyword scan now
// has no remaining blockers — every known vJASS construct is handled by a
// preprocessor pass. The list is kept (currently empty) as a safety net: if
// a future map uses a JassHelper construct we don't yet know about, adding
// it here is the gate.
var vJASSKeywords = []string{}

// FindVJASSKeyword scans the source for the first vJASS-only keyword. Returns
// the matched keyword and true if found; "" / false otherwise. Word-boundary
// match using the lexer so quoted strings and comments don't trip it.
//
// As of Phase 4 the keyword list is empty — every known vJASS construct has
// a preprocessor pass. The function still exists for a future safety-net
// keyword to be added without rippling through callers.
func FindVJASSKeyword(src string) (string, bool) {
	if len(vJASSKeywords) == 0 {
		return "", false
	}
	toks, err := Tokenize(src)
	if err != nil {
		// Lex failed — be conservative; report no keyword (the convert flow
		// can fall back to "unparseable as JASS" via the transpiler).
		return "", false
	}
	set := make(map[string]bool, len(vJASSKeywords))
	for _, k := range vJASSKeywords {
		set[k] = true
	}
	for _, t := range toks {
		// Treat both ident-shaped tokens (since these are NOT in pureKeywords)
		// and explicit keyword tokens uniformly — any of those would have
		// classified as TokIdent in our lexer (vJASS keywords aren't in the
		// pureKeywords map).
		if (t.Kind == TokIdent || t.Kind == TokKeyword) && set[t.Value] {
			return t.Value, true
		}
	}
	return "", false
}
