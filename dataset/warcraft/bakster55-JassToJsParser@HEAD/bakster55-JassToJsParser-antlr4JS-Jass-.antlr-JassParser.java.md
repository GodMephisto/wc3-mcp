---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser"
source_path: "antlr4JS/Jass/.antlr/JassParser.java"
ref: HEAD
---

// Generated from d:\JASS_TO_JS_PARSER\antlr4JS\Jass\Jass.g4 by ANTLR 4.7.1
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class JassParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.7.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		K_CODE=1, K_HANDLE=2, K_INTEGER=3, K_REAL=4, K_BOOLEAN=5, K_STRING=6, 
		K_TRUE=7, K_FALSE=8, K_NULL=9, K_NOT=10, K_AND=11, K_OR=12, K_FUNCTION=13, 
		K_RETURN=14, K_RETURNS=15, K_EXITWHEN=16, K_LOOP=17, K_ENDLOOP=18, K_ELSEIF=19, 
		K_ELSE=20, K_CALL=21, K_IF=22, K_THEN=23, K_ENDIF=24, K_DEBUG=25, K_SET=26, 
		K_TYPE=27, K_EXTENDS=28, K_GLOBALS=29, K_ENDGLOBALS=30, K_CONSTANT=31, 
		K_NATIVE=32, K_TAKES=33, K_NOTHING=34, K_ENDFUNCTION=35, K_LOCAL=36, K_ARRAY=37, 
		ADD=38, SUB=39, MULT=40, DIV=41, GT=42, LT=43, EQ=44, NEQ=45, GTEQ=46, 
		LTEQ=47, O_BRACK=48, C_BRACK=49, O_PAR=50, C_PAR=51, COMMA=52, ASSIGN=53, 
		DECIMAL=54, OCTAL=55, HEX=56, FOURCC=57, REAL_CONST=58, STRING_CONST=59, 
		ID=60, NEWLINE=61, SPACE=62, COMMENT=63;
	public static final int
		RULE_file = 0, RULE_declr = 1, RULE_typedef = 2, RULE_globals = 3, RULE_global_var_list = 4, 
		RULE_native_func = 5, RULE_func_declr = 6, RULE_param_list = 7, RULE_func = 8, 
		RULE_local_var_list = 9, RULE_var_declr = 10, RULE_statement_list = 11, 
		RULE_statement = 12, RULE_set = 13, RULE_call = 14, RULE_args = 15, RULE_ifthenelse = 16, 
		RULE_else_clause = 17, RULE_loop = 18, RULE_exitwhen = 19, RULE_return_stat = 20, 
		RULE_debug = 21, RULE_expr = 22, RULE_func_call = 23, RULE_array_ref = 24, 
		RULE_func_ref = 25, RULE_constant = 26, RULE_int_const = 27, RULE_parens = 28, 
		RULE_bool_const = 29, RULE_type = 30;
	public static final String[] ruleNames = {
		"file", "declr", "typedef", "globals", "global_var_list", "native_func", 
		"func_declr", "param_list", "func", "local_var_list", "var_declr", "statement_list", 
		"statement", "set", "call", "args", "ifthenelse", "else_clause", "loop", 
		"exitwhen", "return_stat", "debug", "expr", "func_call", "array_ref", 
		"func_ref", "constant", "int_const", "parens", "bool_const", "type"
	};

	private static final String[] _LITERAL_NAMES = {
		null, "'code'", "'handle'", "'integer'", "'real'", "'boolean'", "'string'", 
		"'true'", "'false'", "'null'", "'not'", "'and'", "'or'", "'function'", 
		"'return'", "'returns'", "'exitwhen'", "'loop'", "'endloop'", "'elseif'", 
		"'else'", "'call'", "'if'", "'then'", "'endif'", "'debug'", "'set'", "'type'", 
		"'extends'", "'globals'", "'endglobals'", "'constant'", "'native'", "'takes'", 
		"'nothing'", "'endfunction'", "'local'", "'array'", "'+'", "'-'", "'*'", 
		"'/'", "'>'", "'<'", "'=='", "'!='", "'>='", "'<='", "'['", "']'", "'('", 
		"')'", "','", "'='"
	};
	private static final String[] _SYMBOLIC_NAMES = {
		null, "K_CODE", "K_HANDLE", "K_INTEGER", "K_REAL", "K_BOOLEAN", "K_STRING", 
		"K_TRUE", "K_FALSE", "K_NULL", "K_NOT", "K_AND", "K_OR", "K_FUNCTION", 
		"K_RETURN", "K_RETURNS", "K_EXITWHEN", "K_LOOP", "K_ENDLOOP", "K_ELSEIF", 
		"K_ELSE", "K_CALL", "K_IF", "K_THEN", "K_ENDIF", "K_DEBUG", "K_SET", "K_TYPE", 
		"K_EXTENDS", "K_GLOBALS", "K_ENDGLOBALS", "K_CONSTANT", "K_NATIVE", "K_TAKES", 
		"K_NOTHING", "K_ENDFUNCTION", "K_LOCAL", "K_ARRAY", "ADD", "SUB", "MULT", 
		"DIV", "GT", "LT", "EQ", "NEQ", "GTEQ", "LTEQ", "O_BRACK", "C_BRACK", 
		"O_PAR", "C_PAR", "COMMA", "ASSIGN", "DECIMAL", "OCTAL", "HEX", "FOURCC", 
		"REAL_CONST", "STRING_CONST", "ID", "NEWLINE", "SPACE", "COMMENT"
	};
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}

	@Override
	public String getGrammarFileName() { return "Jass.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public JassParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}
	public static class FileContext extends ParserRuleContext {
		public List<TerminalNode> NEWLINE() { return getTokens(JassParser.NEWLINE); }
		public TerminalNode NEWLINE(int i) {
			return getToken(JassParser.NEWLINE, i);
		}
		public List<DeclrContext> declr() {
			return getRuleContexts(DeclrContext.class);
		}
		public DeclrContext declr(int i) {
			return getRuleContext(DeclrContext.class,i);
		}
		public List<FuncContext> func() {
			return getRuleContexts(FuncContext.class);
		}
		public FuncContext func(int i) {
			return getRuleContext(FuncContext.class,i);
		}
		public FileContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_file; }
	}

	public final FileContext file() throws RecognitionException {
		FileContext _localctx = new FileContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_file);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(63);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==NEWLINE) {
				{
				setState(62);
				match(NEWLINE);
				}
			}

			setState(70);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,1,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(65);
					declr();
					setState(66);
					match(NEWLINE);
					}
					} 
				}
				setState(72);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,1,_ctx);
			}
			setState(76);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==K_FUNCTION || _la==K_CONSTANT) {
				{
				{
				setState(73);
				func();
				}
				}
				setState(78);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class DeclrContext extends ParserRuleContext {
		public TypedefContext typedef() {
			return getRuleContext(TypedefContext.class,0);
		}
		public GlobalsContext globals() {
			return getRuleContext(GlobalsContext.class,0);
		}
		public Native_funcContext native_func() {
			return getRuleContext(Native_funcContext.class,0);
		}
		public DeclrContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_declr; }
	}

	public final DeclrContext declr() throws RecognitionException {
		DeclrContext _localctx = new DeclrContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_declr);
		try {
			setState(82);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case K_TYPE:
				enterOuterAlt(_localctx, 1);
				{
				setState(79);
				typedef();
				}
				break;
			case K_GLOBALS:
				enterOuterAlt(_localctx, 2);
				{
				setState(80);
				globals();
				}
				break;
			case K_CONSTANT:
			case K_NATIVE:
				enterOuterAlt(_localctx, 3);
				{
				setState(81);
				native_func();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class TypedefContext extends ParserRuleContext {
		public TerminalNode K_TYPE() { return getToken(JassParser.K_TYPE, 0); }
		public List<TerminalNode> ID() { return getTokens(JassParser.ID); }
		public TerminalNode ID(int i) {
			return getToken(JassParser.ID, i);
		}
		public TerminalNode K_EXTENDS() { return getToken(JassParser.K_EXTENDS, 0); }
		public TerminalNode K_HANDLE() { return getToken(JassParser.K_HANDLE, 0); }
		public TypedefContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_typedef; }
	}

	public final TypedefContext typedef() throws RecognitionException {
		TypedefContext _localctx = new TypedefContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_typedef);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(84);
			match(K_TYPE);
			setState(85);
			match(ID);
			setState(86);
			match(K_EXTENDS);
			setState(87);
			_la = _input.LA(1);
			if ( !(_la==K_HANDLE || _la==ID) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class GlobalsContext extends ParserRuleContext {
		public TerminalNode K_GLOBALS() { return getToken(JassParser.K_GLOBALS, 0); }
		public List<TerminalNode> NEWLINE() { return getTokens(JassParser.NEWLINE); }
		public TerminalNode NEWLINE(int i) {
			return getToken(JassParser.NEWLINE, i);
		}
		public Global_var_listContext global_var_list() {
			return getRuleContext(Global_var_listContext.class,0);
		}
		public TerminalNode K_ENDGLOBALS() { return getToken(JassParser.K_ENDGLOBALS, 0); }
		public GlobalsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_globals; }
	}

	public final GlobalsContext globals() throws RecognitionException {
		GlobalsContext _localctx = new GlobalsContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_globals);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(89);
			match(K_GLOBALS);
			setState(90);
			match(NEWLINE);
			setState(91);
			global_var_list();
			setState(92);
			match(NEWLINE);
			setState(93);
			match(K_ENDGLOBALS);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Global_var_listContext extends ParserRuleContext {
		public List<TerminalNode> K_CONSTANT() { return getTokens(JassParser.K_CONSTANT); }
		public TerminalNode K_CONSTANT(int i) {
			return getToken(JassParser.K_CONSTANT, i);
		}
		public List<TypeContext> type() {
			return getRuleContexts(TypeContext.class);
		}
		public TypeContext type(int i) {
			return getRuleContext(TypeContext.class,i);
		}
		public List<TerminalNode> ID() { return getTokens(JassParser.ID); }
		public TerminalNode ID(int i) {
			return getToken(JassParser.ID, i);
		}
		public List<TerminalNode> ASSIGN() { return getTokens(JassParser.ASSIGN); }
		public TerminalNode ASSIGN(int i) {
			return getToken(JassParser.ASSIGN, i);
		}
		public List<ExprContext> expr() {
			return getRuleContexts(ExprContext.class);
		}
		public ExprContext expr(int i) {
			return getRuleContext(ExprContext.class,i);
		}
		public List<TerminalNode> NEWLINE() { return getTokens(JassParser.NEWLINE); }
		public TerminalNode NEWLINE(int i) {
			return getToken(JassParser.NEWLINE, i);
		}
		public List<Var_declrContext> var_declr() {
			return getRuleContexts(Var_declrContext.class);
		}
		public Var_declrContext var_declr(int i) {
			return getRuleContext(Var_declrContext.class,i);
		}
		public Global_var_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_global_var_list; }
	}

	public final Global_var_listContext global_var_list() throws RecognitionException {
		Global_var_listContext _localctx = new Global_var_listContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_global_var_list);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(107);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << K_CODE) | (1L << K_HANDLE) | (1L << K_INTEGER) | (1L << K_REAL) | (1L << K_BOOLEAN) | (1L << K_STRING) | (1L << K_CONSTANT) | (1L << ID))) != 0)) {
				{
				setState(105);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case K_CONSTANT:
					{
					{
					setState(95);
					match(K_CONSTANT);
					setState(96);
					type();
					setState(97);
					match(ID);
					setState(98);
					match(ASSIGN);
					setState(99);
					expr(0);
					setState(100);
					match(NEWLINE);
					}
					}
					break;
				case K_CODE:
				case K_HANDLE:
				case K_INTEGER:
				case K_REAL:
				case K_BOOLEAN:
				case K_STRING:
				case ID:
					{
					{
					setState(102);
					var_declr();
					setState(103);
					match(NEWLINE);
					}
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				}
				setState(109);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Native_funcContext extends ParserRuleContext {
		public TerminalNode K_NATIVE() { return getToken(JassParser.K_NATIVE, 0); }
		public Func_declrContext func_declr() {
			return getRuleContext(Func_declrContext.class,0);
		}
		public TerminalNode K_CONSTANT() { return getToken(JassParser.K_CONSTANT, 0); }
		public Native_funcContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_native_func; }
	}

	public final Native_funcContext native_func() throws RecognitionException {
		Native_funcContext _localctx = new Native_funcContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_native_func);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(111);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==K_CONSTANT) {
				{
				setState(110);
				match(K_CONSTANT);
				}
			}

			setState(113);
			match(K_NATIVE);
			setState(114);
			func_declr();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Func_declrContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(JassParser.ID, 0); }
		public TerminalNode K_TAKES() { return getToken(JassParser.K_TAKES, 0); }
		public TerminalNode K_RETURNS() { return getToken(JassParser.K_RETURNS, 0); }
		public List<TerminalNode> K_NOTHING() { return getTokens(JassParser.K_NOTHING); }
		public TerminalNode K_NOTHING(int i) {
			return getToken(JassParser.K_NOTHING, i);
		}
		public Param_listContext param_list() {
			return getRuleContext(Param_listContext.class,0);
		}
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public Func_declrContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_func_declr; }
	}

	public final Func_declrContext func_declr() throws RecognitionException {
		Func_declrContext _localctx = new Func_declrContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_func_declr);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(116);
			match(ID);
			setState(117);
			match(K_TAKES);
			setState(120);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case K_NOTHING:
				{
				setState(118);
				match(K_NOTHING);
				}
				break;
			case K_CODE:
			case K_HANDLE:
			case K_INTEGER:
			case K_REAL:
			case K_BOOLEAN:
			case K_STRING:
			case ID:
				{
				setState(119);
				param_list();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			setState(122);
			match(K_RETURNS);
			setState(125);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case K_CODE:
			case K_HANDLE:
			case K_INTEGER:
			case K_REAL:
			case K_BOOLEAN:
			case K_STRING:
			case ID:
				{
				setState(123);
				type();
				}
				break;
			case K_NOTHING:
				{
				setState(124);
				match(K_NOTHING);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Param_listContext extends ParserRuleContext {
		public List<TypeContext> type() {
			return getRuleContexts(TypeContext.class);
		}
		public TypeContext type(int i) {
			return getRuleContext(TypeContext.class,i);
		}
		public List<TerminalNode> ID() { return getTokens(JassParser.ID); }
		public TerminalNode ID(int i) {
			return getToken(JassParser.ID, i);
		}
		public List<TerminalNode> COMMA() { return getTokens(JassParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(JassParser.COMMA, i);
		}
		public Param_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_param_list; }
	}

	public final Param_listContext param_list() throws RecognitionException {
		Param_listContext _localctx = new Param_listContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_param_list);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(127);
			type();
			setState(128);
			match(ID);
			setState(135);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(129);
				match(COMMA);
				setState(130);
				type();
				setState(131);
				match(ID);
				}
				}
				setState(137);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FuncContext extends ParserRuleContext {
		public TerminalNode K_FUNCTION() { return getToken(JassParser.K_FUNCTION, 0); }
		public Func_declrContext func_declr() {
			return getRuleContext(Func_declrContext.class,0);
		}
		public List<TerminalNode> NEWLINE() { return getTokens(JassParser.NEWLINE); }
		public TerminalNode NEWLINE(int i) {
			return getToken(JassParser.NEWLINE, i);
		}
		public Local_var_listContext local_var_list() {
			return getRuleContext(Local_var_listContext.class,0);
		}
		public Statement_listContext statement_list() {
			return getRuleContext(Statement_listContext.class,0);
		}
		public TerminalNode K_ENDFUNCTION() { return getToken(JassParser.K_ENDFUNCTION, 0); }
		public TerminalNode K_CONSTANT() { return getToken(JassParser.K_CONSTANT, 0); }
		public FuncContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_func; }
	}

	public final FuncContext func() throws RecognitionException {
		FuncContext _localctx = new FuncContext(_ctx, getState());
		enterRule(_localctx, 16, RULE_func);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(139);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==K_CONSTANT) {
				{
				setState(138);
				match(K_CONSTANT);
				}
			}

			setState(141);
			match(K_FUNCTION);
			setState(142);
			func_declr();
			setState(143);
			match(NEWLINE);
			setState(144);
			local_var_list();
			setState(145);
			statement_list();
			setState(146);
			match(K_ENDFUNCTION);
			setState(147);
			match(NEWLINE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Local_var_listContext extends ParserRuleContext {
		public List<TerminalNode> K_LOCAL() { return getTokens(JassParser.K_LOCAL); }
		public TerminalNode K_LOCAL(int i) {
			return getToken(JassParser.K_LOCAL, i);
		}
		public List<Var_declrContext> var_declr() {
			return getRuleContexts(Var_declrContext.class);
		}
		public Var_declrContext var_declr(int i) {
			return getRuleContext(Var_declrContext.class,i);
		}
		public List<TerminalNode> NEWLINE() { return getTokens(JassParser.NEWLINE); }
		public TerminalNode NEWLINE(int i) {
			return getToken(JassParser.NEWLINE, i);
		}
		public Local_var_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_local_var_list; }
	}

	public final Local_var_listContext local_var_list() throws RecognitionException {
		Local_var_listContext _localctx = new Local_var_listContext(_ctx, getState());
		enterRule(_localctx, 18, RULE_local_var_list);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(155);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==K_LOCAL) {
				{
				{
				setState(149);
				match(K_LOCAL);
				setState(150);
				var_declr();
				setState(151);
				match(NEWLINE);
				}
				}
				setState(157);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Var_declrContext extends ParserRuleContext {
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public TerminalNode ID() { return getToken(JassParser.ID, 0); }
		public TerminalNode ASSIGN() { return getToken(JassParser.ASSIGN, 0); }
		public ExprContext expr() {
			return getRuleContext(ExprContext.class,0);
		}
		public TerminalNode K_ARRAY() { return getToken(JassParser.K_ARRAY, 0); }
		public Var_declrContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_var_declr; }
	}

	public final Var_declrContext var_declr() throws RecognitionException {
		Var_declrContext _localctx = new Var_declrContext(_ctx, getState());
		enterRule(_localctx, 20, RULE_var_declr);
		int _la;
		try {
			setState(168);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,13,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(158);
				type();
				setState(159);
				match(ID);
				setState(162);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==ASSIGN) {
					{
					setState(160);
					match(ASSIGN);
					setState(161);
					expr(0);
					}
				}

				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(164);
				type();
				setState(165);
				match(K_ARRAY);
				setState(166);
				match(ID);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Statement_listContext extends ParserRuleContext {
		public List<StatementContext> statement() {
			return getRuleContexts(StatementContext.class);
		}
		public StatementContext statement(int i) {
			return getRuleContext(StatementContext.class,i);
		}
		public List<TerminalNode> NEWLINE() { return getTokens(JassParser.NEWLINE); }
		public TerminalNode NEWLINE(int i) {
			return getToken(JassParser.NEWLINE, i);
		}
		public Statement_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_statement_list; }
	}

	public final Statement_listContext statement_list() throws RecognitionException {
		Statement_listContext _localctx = new Statement_listContext(_ctx, getState());
		enterRule(_localctx, 22, RULE_statement_list);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(175);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << K_RETURN) | (1L << K_EXITWHEN) | (1L << K_LOOP) | (1L << K_CALL) | (1L << K_IF) | (1L << K_DEBUG) | (1L << K_SET))) != 0)) {
				{
				{
				setState(170);
				statement();
				setState(171);
				match(NEWLINE);
				}
				}
				setState(177);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class StatementContext extends ParserRuleContext {
		public SetContext set() {
			return getRuleContext(SetContext.class,0);
		}
		public CallContext call() {
			return getRuleContext(CallContext.class,0);
		}
		public IfthenelseContext ifthenelse() {
			return getRuleContext(IfthenelseContext.class,0);
		}
		public LoopContext loop() {
			return getRuleContext(LoopContext.class,0);
		}
		public ExitwhenContext exitwhen() {
			return getRuleContext(ExitwhenContext.class,0);
		}
		public Return_statContext return_stat() {
			return getRuleContext(Return_statContext.class,0);
		}
		public DebugContext debug() {
			return getRuleContext(DebugContext.class,0);
		}
		public StatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_statement; }
	}

	public final StatementContext statement() throws RecognitionException {
		StatementContext _localctx = new StatementContext(_ctx, getState());
		enterRule(_localctx, 24, RULE_statement);
		try {
			setState(185);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case K_SET:
				enterOuterAlt(_localctx, 1);
				{
				setState(178);
				set();
				}
				break;
			case K_CALL:
				enterOuterAlt(_localctx, 2);
				{
				setState(179);
				call();
				}
				break;
			case K_IF:
				enterOuterAlt(_localctx, 3);
				{
				setState(180);
				ifthenelse();
				}
				break;
			case K_LOOP:
				enterOuterAlt(_localctx, 4);
				{
				setState(181);
				loop();
				}
				break;
			case K_EXITWHEN:
				enterOuterAlt(_localctx, 5);
				{
				setState(182);
				exitwhen();
				}
				break;
			case K_RETURN:
				enterOuterAlt(_localctx, 6);
				{
				setState(183);
				return_stat();
				}
				break;
			case K_DEBUG:
				enterOuterAlt(_localctx, 7);
				{
				setState(184);
				debug();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class SetContext extends ParserRuleContext {
		public TerminalNode K_SET() { return getToken(JassParser.K_SET, 0); }
		public TerminalNode ID() { return getToken(JassParser.ID, 0); }
		public TerminalNode ASSIGN() { return getToken(JassParser.ASSIGN, 0); }
		public List<ExprContext> expr() {
			return getRuleContexts(ExprContext.class);
		}
		public ExprContext expr(int i) {
			return getRuleContext(ExprContext.class,i);
		}
		public TerminalNode O_BRACK() { return getToken(JassParser.O_BRACK, 0); }
		public TerminalNode C_BRACK() { return getToken(JassParser.C_BRACK, 0); }
		public SetContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_set; }
	}

	public final SetContext set() throws RecognitionException {
		SetContext _localctx = new SetContext(_ctx, getState());
		enterRule(_localctx, 26, RULE_set);
		try {
			setState(199);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,16,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(187);
				match(K_SET);
				setState(188);
				match(ID);
				setState(189);
				match(ASSIGN);
				setState(190);
				expr(0);
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(191);
				match(K_SET);
				setState(192);
				match(ID);
				setState(193);
				match(O_BRACK);
				setState(194);
				expr(0);
				setState(195);
				match(C_BRACK);
				setState(196);
				match(ASSIGN);
				setState(197);
				expr(0);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class CallContext extends ParserRuleContext {
		public TerminalNode K_CALL() { return getToken(JassParser.K_CALL, 0); }
		public TerminalNode ID() { return getToken(JassParser.ID, 0); }
		public TerminalNode O_PAR() { return getToken(JassParser.O_PAR, 0); }
		public TerminalNode C_PAR() { return getToken(JassParser.C_PAR, 0); }
		public ArgsContext args() {
			return getRuleContext(ArgsContext.class,0);
		}
		public CallContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_call; }
	}

	public final CallContext call() throws RecognitionException {
		CallContext _localctx = new CallContext(_ctx, getState());
		enterRule(_localctx, 28, RULE_call);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(201);
			match(K_CALL);
			setState(202);
			match(ID);
			setState(203);
			match(O_PAR);
			setState(205);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << K_TRUE) | (1L << K_FALSE) | (1L << K_NULL) | (1L << K_NOT) | (1L << K_FUNCTION) | (1L << ADD) | (1L << SUB) | (1L << O_PAR) | (1L << DECIMAL) | (1L << OCTAL) | (1L << HEX) | (1L << FOURCC) | (1L << REAL_CONST) | (1L << STRING_CONST) | (1L << ID))) != 0)) {
				{
				setState(204);
				args();
				}
			}

			setState(207);
			match(C_PAR);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ArgsContext extends ParserRuleContext {
		public List<ExprContext> expr() {
			return getRuleContexts(ExprContext.class);
		}
		public ExprContext expr(int i) {
			return getRuleContext(ExprContext.class,i);
		}
		public List<TerminalNode> COMMA() { return getTokens(JassParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(JassParser.COMMA, i);
		}
		public ArgsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_args; }
	}

	public final ArgsContext args() throws RecognitionException {
		ArgsContext _localctx = new ArgsContext(_ctx, getState());
		enterRule(_localctx, 30, RULE_args);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(209);
			expr(0);
			setState(214);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(210);
				match(COMMA);
				setState(211);
				expr(0);
				}
				}
				setState(216);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IfthenelseContext extends ParserRuleContext {
		public TerminalNode K_IF() { return getToken(JassParser.K_IF, 0); }
		public ExprContext expr() {
			return getRuleContext(ExprContext.class,0);
		}
		public TerminalNode K_THEN() { return getToken(JassParser.K_THEN, 0); }
		public TerminalNode NEWLINE() { return getToken(JassParser.NEWLINE, 0); }
		public Statement_listContext statement_list() {
			return getRuleContext(Statement_listContext.class,0);
		}
		public TerminalNode K_ENDIF() { return getToken(JassParser.K_ENDIF, 0); }
		public Else_clauseContext else_clause() {
			return getRuleContext(Else_clauseContext.class,0);
		}
		public IfthenelseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_ifthenelse; }
	}

	public final IfthenelseContext ifthenelse() throws RecognitionException {
		IfthenelseContext _localctx = new IfthenelseContext(_ctx, getState());
		enterRule(_localctx, 32, RULE_ifthenelse);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(217);
			match(K_IF);
			setState(218);
			expr(0);
			setState(219);
			match(K_THEN);
			setState(220);
			match(NEWLINE);
			setState(221);
			statement_list();
			setState(223);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==K_ELSEIF || _la==K_ELSE) {
				{
				setState(222);
				else_clause();
				}
			}

			setState(225);
			match(K_ENDIF);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Else_clauseContext extends ParserRuleContext {
		public TerminalNode K_ELSE() { return getToken(JassParser.K_ELSE, 0); }
		public TerminalNode NEWLINE() { return getToken(JassParser.NEWLINE, 0); }
		public Statement_listContext statement_list() {
			return getRuleContext(Statement_listContext.class,0);
		}
		public TerminalNode K_ELSEIF() { return getToken(JassParser.K_ELSEIF, 0); }
		public ExprContext expr() {
			return getRuleContext(ExprContext.class,0);
		}
		public TerminalNode K_THEN() { return getToken(JassParser.K_THEN, 0); }
		public Else_clauseContext else_clause() {
			return getRuleContext(Else_clauseContext.class,0);
		}
		public Else_clauseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_else_clause; }
	}

	public final Else_clauseContext else_clause() throws RecognitionException {
		Else_clauseContext _localctx = new Else_clauseContext(_ctx, getState());
		enterRule(_localctx, 34, RULE_else_clause);
		int _la;
		try {
			setState(238);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case K_ELSE:
				enterOuterAlt(_localctx, 1);
				{
				setState(227);
				match(K_ELSE);
				setState(228);
				match(NEWLINE);
				setState(229);
				statement_list();
				}
				break;
			case K_ELSEIF:
				enterOuterAlt(_localctx, 2);
				{
				setState(230);
				match(K_ELSEIF);
				setState(231);
				expr(0);
				setState(232);
				match(K_THEN);
				setState(233);
				match(NEWLINE);
				setState(234);
				statement_list();
				setState(236);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==K_ELSEIF || _la==K_ELSE) {
					{
					setState(235);
					else_clause();
					}
				}

				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class LoopContext extends ParserRuleContext {
		public TerminalNode K_LOOP() { return getToken(JassParser.K_LOOP, 0); }
		public TerminalNode NEWLINE() { return getToken(JassParser.NEWLINE, 0); }
		public Statement_listContext statement_list() {
			return getRuleContext(Statement_listContext.class,0);
		}
		public TerminalNode K_ENDLOOP() { return getToken(JassParser.K_ENDLOOP, 0); }
		public LoopContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_loop; }
	}

	public final LoopContext loop() throws RecognitionException {
		LoopContext _localctx = new LoopContext(_ctx, getState());
		enterRule(_localctx, 36, RULE_loop);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(240);
			match(K_LOOP);
			setState(241);
			match(NEWLINE);
			setState(242);
			statement_list();
			setState(243);
			match(K_ENDLOOP);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ExitwhenContext extends ParserRuleContext {
		public TerminalNode K_EXITWHEN() { return getToken(JassParser.K_EXITWHEN, 0); }
		public ExprContext expr() {
			return getRuleContext(ExprContext.class,0);
		}
		public ExitwhenContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_exitwhen; }
	}

	public final ExitwhenContext exitwhen() throws RecognitionException {
		ExitwhenContext _localctx = new ExitwhenContext(_ctx, getState());
		enterRule(_localctx, 38, RULE_exitwhen);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(245);
			match(K_EXITWHEN);
			setState(246);
			expr(0);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Return_statContext extends ParserRuleContext {
		public TerminalNode K_RETURN() { return getToken(JassParser.K_RETURN, 0); }
		public ExprContext expr() {
			return getRuleContext(ExprContext.class,0);
		}
		public Return_statContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_return_stat; }
	}

	public final Return_statContext return_stat() throws RecognitionException {
		Return_statContext _localctx = new Return_statContext(_ctx, getState());
		enterRule(_localctx, 40, RULE_return_stat);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(248);
			match(K_RETURN);
			setState(250);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << K_TRUE) | (1L << K_FALSE) | (1L << K_NULL) | (1L << K_NOT) | (1L << K_FUNCTION) | (1L << ADD) | (1L << SUB) | (1L << O_PAR) | (1L << DECIMAL) | (1L << OCTAL) | (1L << HEX) | (1L << FOURCC) | (1L << REAL_CONST) | (1L << STRING_CONST) | (1L << ID))) != 0)) {
				{
				setState(249);
				expr(0);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class DebugContext extends ParserRuleContext {
		public TerminalNode K_DEBUG() { return getToken(JassParser.K_DEBUG, 0); }
		public SetContext set() {
			return getRuleContext(SetContext.class,0);
		}
		public CallContext call() {
			return getRuleContext(CallContext.class,0);
		}
		public IfthenelseContext ifthenelse() {
			return getRuleContext(IfthenelseContext.class,0);
		}
		public LoopContext loop() {
			return getRuleContext(LoopContext.class,0);
		}
		public DebugContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_debug; }
	}

	public final DebugContext debug() throws RecognitionException {
		DebugContext _localctx = new DebugContext(_ctx, getState());
		enterRule(_localctx, 42, RULE_debug);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(252);
			match(K_DEBUG);
			setState(257);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case K_SET:
				{
				setState(253);
				set();
				}
				break;
			case K_CALL:
				{
				setState(254);
				call();
				}
				break;
			case K_IF:
				{
				setState(255);
				ifthenelse();
				}
				break;
			case K_LOOP:
				{
				setState(256);
				loop();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ExprContext extends ParserRuleContext {
		public TerminalNode ADD() { return getToken(JassParser.ADD, 0); }
		public List<ExprContext> expr() {
			return getRuleContexts(ExprContext.class);
		}
		public ExprContext expr(int i) {
			return getRuleContext(ExprContext.class,i);
		}
		public TerminalNode SUB() { return getToken(JassParser.SUB, 0); }
		public TerminalNode K_NOT() { return getToken(JassParser.K_NOT, 0); }
		public Func_callContext func_call() {
			return getRuleContext(Func_callContext.class,0);
		}
		public Array_refContext array_ref() {
			return getRuleContext(Array_refContext.class,0);
		}
		public Func_refContext func_ref() {
			return getRuleContext(Func_refContext.class,0);
		}
		public TerminalNode ID() { return getToken(JassParser.ID, 0); }
		public ConstantContext constant() {
			return getRuleContext(ConstantContext.class,0);
		}
		public ParensContext parens() {
			return getRuleContext(ParensContext.class,0);
		}
		public TerminalNode MULT() { return getToken(JassParser.MULT, 0); }
		public TerminalNode DIV() { return getToken(JassParser.DIV, 0); }
		public TerminalNode GT() { return getToken(JassParser.GT, 0); }
		public TerminalNode LT() { return getToken(JassParser.LT, 0); }
		public TerminalNode GTEQ() { return getToken(JassParser.GTEQ, 0); }
		public TerminalNode LTEQ() { return getToken(JassParser.LTEQ, 0); }
		public TerminalNode EQ() { return getToken(JassParser.EQ, 0); }
		public TerminalNode NEQ() { return getToken(JassParser.NEQ, 0); }
		public TerminalNode K_AND() { return getToken(JassParser.K_AND, 0); }
		public TerminalNode K_OR() { return getToken(JassParser.K_OR, 0); }
		public ExprContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expr; }
	}

	public final ExprContext expr() throws RecognitionException {
		return expr(0);
	}

	private ExprContext expr(int _p) throws RecognitionException {
		ParserRuleContext _parentctx = _ctx;
		int _parentState = getState();
		ExprContext _localctx = new ExprContext(_ctx, _parentState);
		ExprContext _prevctx = _localctx;
		int _startState = 44;
		enterRecursionRule(_localctx, 44, RULE_expr, _p);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(272);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,24,_ctx) ) {
			case 1:
				{
				setState(260);
				match(ADD);
				setState(261);
				expr(14);
				}
				break;
			case 2:
				{
				setState(262);
				match(SUB);
				setState(263);
				expr(13);
				}
				break;
			case 3:
				{
				setState(264);
				match(K_NOT);
				setState(265);
				expr(12);
				}
				break;
			case 4:
				{
				setState(266);
				func_call();
				}
				break;
			case 5:
				{
				setState(267);
				array_ref();
				}
				break;
			case 6:
				{
				setState(268);
				func_ref();
				}
				break;
			case 7:
				{
				setState(269);
				match(ID);
				}
				break;
			case 8:
				{
				setState(270);
				constant();
				}
				break;
			case 9:
				{
				setState(271);
				parens();
				}
				break;
			}
			_ctx.stop = _input.LT(-1);
			setState(291);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,26,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					if ( _parseListeners!=null ) triggerExitRuleEvent();
					_prevctx = _localctx;
					{
					setState(289);
					_errHandler.sync(this);
					switch ( getInterpreter().adaptivePredict(_input,25,_ctx) ) {
					case 1:
						{
						_localctx = new ExprContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_expr);
						setState(274);
						if (!(precpred(_ctx, 11))) throw new FailedPredicateException(this, "precpred(_ctx, 11)");
						setState(275);
						_la = _input.LA(1);
						if ( !(_la==MULT || _la==DIV) ) {
						_errHandler.recoverInline(this);
						}
						else {
							if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
							_errHandler.reportMatch(this);
							consume();
						}
						setState(276);
						expr(12);
						}
						break;
					case 2:
						{
						_localctx = new ExprContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_expr);
						setState(277);
						if (!(precpred(_ctx, 10))) throw new FailedPredicateException(this, "precpred(_ctx, 10)");
						setState(278);
						_la = _input.LA(1);
						if ( !(_la==ADD || _la==SUB) ) {
						_errHandler.recoverInline(this);
						}
						else {
							if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
							_errHandler.reportMatch(this);
							consume();
						}
						setState(279);
						expr(11);
						}
						break;
					case 3:
						{
						_localctx = new ExprContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_expr);
						setState(280);
						if (!(precpred(_ctx, 9))) throw new FailedPredicateException(this, "precpred(_ctx, 9)");
						setState(281);
						_la = _input.LA(1);
						if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << GT) | (1L << LT) | (1L << GTEQ) | (1L << LTEQ))) != 0)) ) {
						_errHandler.recoverInline(this);
						}
						else {
							if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
							_errHandler.reportMatch(this);
							consume();
						}
						setState(282);
						expr(10);
						}
						break;
					case 4:
						{
						_localctx = new ExprContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_expr);
						setState(283);
						if (!(precpred(_ctx, 8))) throw new FailedPredicateException(this, "precpred(_ctx, 8)");
						setState(284);
						_la = _input.LA(1);
						if ( !(_la==EQ || _la==NEQ) ) {
						_errHandler.recoverInline(this);
						}
						else {
							if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
							_errHandler.reportMatch(this);
							consume();
						}
						setState(285);
						expr(9);
						}
						break;
					case 5:
						{
						_localctx = new ExprContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_expr);
						setState(286);
						if (!(precpred(_ctx, 7))) throw new FailedPredicateException(this, "precpred(_ctx, 7)");
						setState(287);
						_la = _input.LA(1);
						if ( !(_la==K_AND || _la==K_OR) ) {
						_errHandler.recoverInline(this);
						}
						else {
							if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
							_errHandler.reportMatch(this);
							consume();
						}
						setState(288);
						expr(8);
						}
						break;
					}
					} 
				}
				setState(293);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,26,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			unrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public static class Func_callContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(JassParser.ID, 0); }
		public TerminalNode O_PAR() { return getToken(JassParser.O_PAR, 0); }
		public TerminalNode C_PAR() { return getToken(JassParser.C_PAR, 0); }
		public ArgsContext args() {
			return getRuleContext(ArgsContext.class,0);
		}
		public Func_callContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_func_call; }
	}

	public final Func_callContext func_call() throws RecognitionException {
		Func_callContext _localctx = new Func_callContext(_ctx, getState());
		enterRule(_localctx, 46, RULE_func_call);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(294);
			match(ID);
			setState(295);
			match(O_PAR);
			setState(297);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << K_TRUE) | (1L << K_FALSE) | (1L << K_NULL) | (1L << K_NOT) | (1L << K_FUNCTION) | (1L << ADD) | (1L << SUB) | (1L << O_PAR) | (1L << DECIMAL) | (1L << OCTAL) | (1L << HEX) | (1L << FOURCC) | (1L << REAL_CONST) | (1L << STRING_CONST) | (1L << ID))) != 0)) {
				{
				setState(296);
				args();
				}
			}

			setState(299);
			match(C_PAR);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Array_refContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(JassParser.ID, 0); }
		public TerminalNode O_BRACK() { return getToken(JassParser.O_BRACK, 0); }
		public ExprContext expr() {
			return getRuleContext(ExprContext.class,0);
		}
		public TerminalNode C_BRACK() { return getToken(JassParser.C_BRACK, 0); }
		public Array_refContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_array_ref; }
	}

	public final Array_refContext array_ref() throws RecognitionException {
		Array_refContext _localctx = new Array_refContext(_ctx, getState());
		enterRule(_localctx, 48, RULE_array_ref);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(301);
			match(ID);
			setState(302);
			match(O_BRACK);
			setState(303);
			expr(0);
			setState(304);
			match(C_BRACK);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Func_refContext extends ParserRuleContext {
		public TerminalNode K_FUNCTION() { return getToken(JassParser.K_FUNCTION, 0); }
		public TerminalNode ID() { return getToken(JassParser.ID, 0); }
		public Func_refContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_func_ref; }
	}

	public final Func_refContext func_ref() throws RecognitionException {
		Func_refContext _localctx = new Func_refContext(_ctx, getState());
		enterRule(_localctx, 50, RULE_func_ref);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(306);
			match(K_FUNCTION);
			setState(307);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ConstantContext extends ParserRuleContext {
		public Int_constContext int_const() {
			return getRuleContext(Int_constContext.class,0);
		}
		public TerminalNode REAL_CONST() { return getToken(JassParser.REAL_CONST, 0); }
		public Bool_constContext bool_const() {
			return getRuleContext(Bool_constContext.class,0);
		}
		public TerminalNode STRING_CONST() { return getToken(JassParser.STRING_CONST, 0); }
		public TerminalNode K_NULL() { return getToken(JassParser.K_NULL, 0); }
		public ConstantContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_constant; }
	}

	public final ConstantContext constant() throws RecognitionException {
		ConstantContext _localctx = new ConstantContext(_ctx, getState());
		enterRule(_localctx, 52, RULE_constant);
		try {
			setState(314);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case DECIMAL:
			case OCTAL:
			case HEX:
			case FOURCC:
				enterOuterAlt(_localctx, 1);
				{
				setState(309);
				int_const();
				}
				break;
			case REAL_CONST:
				enterOuterAlt(_localctx, 2);
				{
				setState(310);
				match(REAL_CONST);
				}
				break;
			case K_TRUE:
			case K_FALSE:
				enterOuterAlt(_localctx, 3);
				{
				setState(311);
				bool_const();
				}
				break;
			case STRING_CONST:
				enterOuterAlt(_localctx, 4);
				{
				setState(312);
				match(STRING_CONST);
				}
				break;
			case K_NULL:
				enterOuterAlt(_localctx, 5);
				{
				setState(313);
				match(K_NULL);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Int_constContext extends ParserRuleContext {
		public TerminalNode DECIMAL() { return getToken(JassParser.DECIMAL, 0); }
		public TerminalNode OCTAL() { return getToken(JassParser.OCTAL, 0); }
		public TerminalNode HEX() { return getToken(JassParser.HEX, 0); }
		public TerminalNode FOURCC() { return getToken(JassParser.FOURCC, 0); }
		public Int_constContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_int_const; }
	}

	public final Int_constContext int_const() throws RecognitionException {
		Int_constContext _localctx = new Int_constContext(_ctx, getState());
		enterRule(_localctx, 54, RULE_int_const);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(316);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << DECIMAL) | (1L << OCTAL) | (1L << HEX) | (1L << FOURCC))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ParensContext extends ParserRuleContext {
		public TerminalNode O_PAR() { return getToken(JassParser.O_PAR, 0); }
		public ExprContext expr() {
			return getRuleContext(ExprContext.class,0);
		}
		public TerminalNode C_PAR() { return getToken(JassParser.C_PAR, 0); }
		public ParensContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parens; }
	}

	public final ParensContext parens() throws RecognitionException {
		ParensContext _localctx = new ParensContext(_ctx, getState());
		enterRule(_localctx, 56, RULE_parens);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(318);
			match(O_PAR);
			setState(319);
			expr(0);
			setState(320);
			match(C_PAR);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Bool_constContext extends ParserRuleContext {
		public TerminalNode K_TRUE() { return getToken(JassParser.K_TRUE, 0); }
		public TerminalNode K_FALSE() { return getToken(JassParser.K_FALSE, 0); }
		public Bool_constContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_bool_const; }
	}

	public final Bool_constContext bool_const() throws RecognitionException {
		Bool_constContext _localctx = new Bool_constContext(_ctx, getState());
		enterRule(_localctx, 58, RULE_bool_const);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(322);
			_la = _input.LA(1);
			if ( !(_la==K_TRUE || _la==K_FALSE) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class TypeContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(JassParser.ID, 0); }
		public TerminalNode K_CODE() { return getToken(JassParser.K_CODE, 0); }
		public TerminalNode K_HANDLE() { return getToken(JassParser.K_HANDLE, 0); }
		public TerminalNode K_INTEGER() { return getToken(JassParser.K_INTEGER, 0); }
		public TerminalNode K_REAL() { return getToken(JassParser.K_REAL, 0); }
		public TerminalNode K_BOOLEAN() { return getToken(JassParser.K_BOOLEAN, 0); }
		public TerminalNode K_STRING() { return getToken(JassParser.K_STRING, 0); }
		public TypeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_type; }
	}

	public final TypeContext type() throws RecognitionException {
		TypeContext _localctx = new TypeContext(_ctx, getState());
		enterRule(_localctx, 60, RULE_type);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(324);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << K_CODE) | (1L << K_HANDLE) | (1L << K_INTEGER) | (1L << K_REAL) | (1L << K_BOOLEAN) | (1L << K_STRING) | (1L << ID))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public boolean sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 22:
			return expr_sempred((ExprContext)_localctx, predIndex);
		}
		return true;
	}
	private boolean expr_sempred(ExprContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0:
			return precpred(_ctx, 11);
		case 1:
			return precpred(_ctx, 10);
		case 2:
			return precpred(_ctx, 9);
		case 3:
			return precpred(_ctx, 8);
		case 4:
			return precpred(_ctx, 7);
		}
		return true;
	}

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\3A\u0149\4\2\t\2\4"+
		"\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13\t"+
		"\13\4\f\t\f\4\r\t\r\4\16\t\16\4\17\t\17\4\20\t\20\4\21\t\21\4\22\t\22"+
		"\4\23\t\23\4\24\t\24\4\25\t\25\4\26\t\26\4\27\t\27\4\30\t\30\4\31\t\31"+
		"\4\32\t\32\4\33\t\33\4\34\t\34\4\35\t\35\4\36\t\36\4\37\t\37\4 \t \3\2"+
		"\5\2B\n\2\3\2\3\2\3\2\7\2G\n\2\f\2\16\2J\13\2\3\2\7\2M\n\2\f\2\16\2P\13"+
		"\2\3\3\3\3\3\3\5\3U\n\3\3\4\3\4\3\4\3\4\3\4\3\5\3\5\3\5\3\5\3\5\3\5\3"+
		"\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\7\6l\n\6\f\6\16\6o\13\6\3\7\5\7"+
		"r\n\7\3\7\3\7\3\7\3\b\3\b\3\b\3\b\5\b{\n\b\3\b\3\b\3\b\5\b\u0080\n\b\3"+
		"\t\3\t\3\t\3\t\3\t\3\t\7\t\u0088\n\t\f\t\16\t\u008b\13\t\3\n\5\n\u008e"+
		"\n\n\3\n\3\n\3\n\3\n\3\n\3\n\3\n\3\n\3\13\3\13\3\13\3\13\7\13\u009c\n"+
		"\13\f\13\16\13\u009f\13\13\3\f\3\f\3\f\3\f\5\f\u00a5\n\f\3\f\3\f\3\f\3"+
		"\f\5\f\u00ab\n\f\3\r\3\r\3\r\7\r\u00b0\n\r\f\r\16\r\u00b3\13\r\3\16\3"+
		"\16\3\16\3\16\3\16\3\16\3\16\5\16\u00bc\n\16\3\17\3\17\3\17\3\17\3\17"+
		"\3\17\3\17\3\17\3\17\3\17\3\17\3\17\5\17\u00ca\n\17\3\20\3\20\3\20\3\20"+
		"\5\20\u00d0\n\20\3\20\3\20\3\21\3\21\3\21\7\21\u00d7\n\21\f\21\16\21\u00da"+
		"\13\21\3\22\3\22\3\22\3\22\3\22\3\22\5\22\u00e2\n\22\3\22\3\22\3\23\3"+
		"\23\3\23\3\23\3\23\3\23\3\23\3\23\3\23\5\23\u00ef\n\23\5\23\u00f1\n\23"+
		"\3\24\3\24\3\24\3\24\3\24\3\25\3\25\3\25\3\26\3\26\5\26\u00fd\n\26\3\27"+
		"\3\27\3\27\3\27\3\27\5\27\u0104\n\27\3\30\3\30\3\30\3\30\3\30\3\30\3\30"+
		"\3\30\3\30\3\30\3\30\3\30\3\30\5\30\u0113\n\30\3\30\3\30\3\30\3\30\3\30"+
		"\3\30\3\30\3\30\3\30\3\30\3\30\3\30\3\30\3\30\3\30\7\30\u0124\n\30\f\30"+
		"\16\30\u0127\13\30\3\31\3\31\3\31\5\31\u012c\n\31\3\31\3\31\3\32\3\32"+
		"\3\32\3\32\3\32\3\33\3\33\3\33\3\34\3\34\3\34\3\34\3\34\5\34\u013d\n\34"+
		"\3\35\3\35\3\36\3\36\3\36\3\36\3\37\3\37\3 \3 \3 \2\3.!\2\4\6\b\n\f\16"+
		"\20\22\24\26\30\32\34\36 \"$&(*,.\60\62\64\668:<>\2\13\4\2\4\4>>\3\2*"+
		"+\3\2()\4\2,-\60\61\3\2./\3\2\r\16\3\28;\3\2\t\n\4\2\3\b>>\2\u015b\2A"+
		"\3\2\2\2\4T\3\2\2\2\6V\3\2\2\2\b[\3\2\2\2\nm\3\2\2\2\fq\3\2\2\2\16v\3"+
		"\2\2\2\20\u0081\3\2\2\2\22\u008d\3\2\2\2\24\u009d\3\2\2\2\26\u00aa\3\2"+
		"\2\2\30\u00b1\3\2\2\2\32\u00bb\3\2\2\2\34\u00c9\3\2\2\2\36\u00cb\3\2\2"+
		"\2 \u00d3\3\2\2\2\"\u00db\3\2\2\2$\u00f0\3\2\2\2&\u00f2\3\2\2\2(\u00f7"+
		"\3\2\2\2*\u00fa\3\2\2\2,\u00fe\3\2\2\2.\u0112\3\2\2\2\60\u0128\3\2\2\2"+
		"\62\u012f\3\2\2\2\64\u0134\3\2\2\2\66\u013c\3\2\2\28\u013e\3\2\2\2:\u0140"+
		"\3\2\2\2<\u0144\3\2\2\2>\u0146\3\2\2\2@B\7?\2\2A@\3\2\2\2AB\3\2\2\2BH"+
		"\3\2\2\2CD\5\4\3\2DE\7?\2\2EG\3\2\2\2FC\3\2\2\2GJ\3\2\2\2HF\3\2\2\2HI"+
		"\3\2\2\2IN\3\2\2\2JH\3\2\2\2KM\5\22\n\2LK\3\2\2\2MP\3\2\2\2NL\3\2\2\2"+
		"NO\3\2\2\2O\3\3\2\2\2PN\3\2\2\2QU\5\6\4\2RU\5\b\5\2SU\5\f\7\2TQ\3\2\2"+
		"\2TR\3\2\2\2TS\3\2\2\2U\5\3\2\2\2VW\7\35\2\2WX\7>\2\2XY\7\36\2\2YZ\t\2"+
		"\2\2Z\7\3\2\2\2[\\\7\37\2\2\\]\7?\2\2]^\5\n\6\2^_\7?\2\2_`\7 \2\2`\t\3"+
		"\2\2\2ab\7!\2\2bc\5> \2cd\7>\2\2de\7\67\2\2ef\5.\30\2fg\7?\2\2gl\3\2\2"+
		"\2hi\5\26\f\2ij\7?\2\2jl\3\2\2\2ka\3\2\2\2kh\3\2\2\2lo\3\2\2\2mk\3\2\2"+
		"\2mn\3\2\2\2n\13\3\2\2\2om\3\2\2\2pr\7!\2\2qp\3\2\2\2qr\3\2\2\2rs\3\2"+
		"\2\2st\7\"\2\2tu\5\16\b\2u\r\3\2\2\2vw\7>\2\2wz\7#\2\2x{\7$\2\2y{\5\20"+
		"\t\2zx\3\2\2\2zy\3\2\2\2{|\3\2\2\2|\177\7\21\2\2}\u0080\5> \2~\u0080\7"+
		"$\2\2\177}\3\2\2\2\177~\3\2\2\2\u0080\17\3\2\2\2\u0081\u0082\5> \2\u0082"+
		"\u0089\7>\2\2\u0083\u0084\7\66\2\2\u0084\u0085\5> \2\u0085\u0086\7>\2"+
		"\2\u0086\u0088\3\2\2\2\u0087\u0083\3\2\2\2\u0088\u008b\3\2\2\2\u0089\u0087"+
		"\3\2\2\2\u0089\u008a\3\2\2\2\u008a\21\3\2\2\2\u008b\u0089\3\2\2\2\u008c"+
		"\u008e\7!\2\2\u008d\u008c\3\2\2\2\u008d\u008e\3\2\2\2\u008e\u008f\3\2"+
		"\2\2\u008f\u0090\7\17\2\2\u0090\u0091\5\16\b\2\u0091\u0092\7?\2\2\u0092"+
		"\u0093\5\24\13\2\u0093\u0094\5\30\r\2\u0094\u0095\7%\2\2\u0095\u0096\7"+
		"?\2\2\u0096\23\3\2\2\2\u0097\u0098\7&\2\2\u0098\u0099\5\26\f\2\u0099\u009a"+
		"\7?\2\2\u009a\u009c\3\2\2\2\u009b\u0097\3\2\2\2\u009c\u009f\3\2\2\2\u009d"+
		"\u009b\3\2\2\2\u009d\u009e\3\2\2\2\u009e\25\3\2\2\2\u009f\u009d\3\2\2"+
		"\2\u00a0\u00a1\5> \2\u00a1\u00a4\7>\2\2\u00a2\u00a3\7\67\2\2\u00a3\u00a5"+
		"\5.\30\2\u00a4\u00a2\3\2\2\2\u00a4\u00a5\3\2\2\2\u00a5\u00ab\3\2\2\2\u00a6"+
		"\u00a7\5> \2\u00a7\u00a8\7\'\2\2\u00a8\u00a9\7>\2\2\u00a9\u00ab\3\2\2"+
		"\2\u00aa\u00a0\3\2\2\2\u00aa\u00a6\3\2\2\2\u00ab\27\3\2\2\2\u00ac\u00ad"+
		"\5\32\16\2\u00ad\u00ae\7?\2\2\u00ae\u00b0\3\2\2\2\u00af\u00ac\3\2\2\2"+
		"\u00b0\u00b3\3\2\2\2\u00b1\u00af\3\2\2\2\u00b1\u00b2\3\2\2\2\u00b2\31"+
		"\3\2\2\2\u00b3\u00b1\3\2\2\2\u00b4\u00bc\5\34\17\2\u00b5\u00bc\5\36\20"+
		"\2\u00b6\u00bc\5\"\22\2\u00b7\u00bc\5&\24\2\u00b8\u00bc\5(\25\2\u00b9"+
		"\u00bc\5*\26\2\u00ba\u00bc\5,\27\2\u00bb\u00b4\3\2\2\2\u00bb\u00b5\3\2"+
		"\2\2\u00bb\u00b6\3\2\2\2\u00bb\u00b7\3\2\2\2\u00bb\u00b8\3\2\2\2\u00bb"+
		"\u00b9\3\2\2\2\u00bb\u00ba\3\2\2\2\u00bc\33\3\2\2\2\u00bd\u00be\7\34\2"+
		"\2\u00be\u00bf\7>\2\2\u00bf\u00c0\7\67\2\2\u00c0\u00ca\5.\30\2\u00c1\u00c2"+
		"\7\34\2\2\u00c2\u00c3\7>\2\2\u00c3\u00c4\7\62\2\2\u00c4\u00c5\5.\30\2"+
		"\u00c5\u00c6\7\63\2\2\u00c6\u00c7\7\67\2\2\u00c7\u00c8\5.\30\2\u00c8\u00ca"+
		"\3\2\2\2\u00c9\u00bd\3\2\2\2\u00c9\u00c1\3\2\2\2\u00ca\35\3\2\2\2\u00cb"+
		"\u00cc\7\27\2\2\u00cc\u00cd\7>\2\2\u00cd\u00cf\7\64\2\2\u00ce\u00d0\5"+
		" \21\2\u00cf\u00ce\3\2\2\2\u00cf\u00d0\3\2\2\2\u00d0\u00d1\3\2\2\2\u00d1"+
		"\u00d2\7\65\2\2\u00d2\37\3\2\2\2\u00d3\u00d8\5.\30\2\u00d4\u00d5\7\66"+
		"\2\2\u00d5\u00d7\5.\30\2\u00d6\u00d4\3\2\2\2\u00d7\u00da\3\2\2\2\u00d8"+
		"\u00d6\3\2\2\2\u00d8\u00d9\3\2\2\2\u00d9!\3\2\2\2\u00da\u00d8\3\2\2\2"+
		"\u00db\u00dc\7\30\2\2\u00dc\u00dd\5.\30\2\u00dd\u00de\7\31\2\2\u00de\u00df"+
		"\7?\2\2\u00df\u00e1\5\30\r\2\u00e0\u00e2\5$\23\2\u00e1\u00e0\3\2\2\2\u00e1"+
		"\u00e2\3\2\2\2\u00e2\u00e3\3\2\2\2\u00e3\u00e4\7\32\2\2\u00e4#\3\2\2\2"+
		"\u00e5\u00e6\7\26\2\2\u00e6\u00e7\7?\2\2\u00e7\u00f1\5\30\r\2\u00e8\u00e9"+
		"\7\25\2\2\u00e9\u00ea\5.\30\2\u00ea\u00eb\7\31\2\2\u00eb\u00ec\7?\2\2"+
		"\u00ec\u00ee\5\30\r\2\u00ed\u00ef\5$\23\2\u00ee\u00ed\3\2\2\2\u00ee\u00ef"+
		"\3\2\2\2\u00ef\u00f1\3\2\2\2\u00f0\u00e5\3\2\2\2\u00f0\u00e8\3\2\2\2\u00f1"+
		"%\3\2\2\2\u00f2\u00f3\7\23\2\2\u00f3\u00f4\7?\2\2\u00f4\u00f5\5\30\r\2"+
		"\u00f5\u00f6\7\24\2\2\u00f6\'\3\2\2\2\u00f7\u00f8\7\22\2\2\u00f8\u00f9"+
		"\5.\30\2\u00f9)\3\2\2\2\u00fa\u00fc\7\20\2\2\u00fb\u00fd\5.\30\2\u00fc"+
		"\u00fb\3\2\2\2\u00fc\u00fd\3\2\2\2\u00fd+\3\2\2\2\u00fe\u0103\7\33\2\2"+
		"\u00ff\u0104\5\34\17\2\u0100\u0104\5\36\20\2\u0101\u0104\5\"\22\2\u0102"+
		"\u0104\5&\24\2\u0103\u00ff\3\2\2\2\u0103\u0100\3\2\2\2\u0103\u0101\3\2"+
		"\2\2\u0103\u0102\3\2\2\2\u0104-\3\2\2\2\u0105\u0106\b\30\1\2\u0106\u0107"+
		"\7(\2\2\u0107\u0113\5.\30\20\u0108\u0109\7)\2\2\u0109\u0113\5.\30\17\u010a"+
		"\u010b\7\f\2\2\u010b\u0113\5.\30\16\u010c\u0113\5\60\31\2\u010d\u0113"+
		"\5\62\32\2\u010e\u0113\5\64\33\2\u010f\u0113\7>\2\2\u0110\u0113\5\66\34"+
		"\2\u0111\u0113\5:\36\2\u0112\u0105\3\2\2\2\u0112\u0108\3\2\2\2\u0112\u010a"+
		"\3\2\2\2\u0112\u010c\3\2\2\2\u0112\u010d\3\2\2\2\u0112\u010e\3\2\2\2\u0112"+
		"\u010f\3\2\2\2\u0112\u0110\3\2\2\2\u0112\u0111\3\2\2\2\u0113\u0125\3\2"+
		"\2\2\u0114\u0115\f\r\2\2\u0115\u0116\t\3\2\2\u0116\u0124\5.\30\16\u0117"+
		"\u0118\f\f\2\2\u0118\u0119\t\4\2\2\u0119\u0124\5.\30\r\u011a\u011b\f\13"+
		"\2\2\u011b\u011c\t\5\2\2\u011c\u0124\5.\30\f\u011d\u011e\f\n\2\2\u011e"+
		"\u011f\t\6\2\2\u011f\u0124\5.\30\13\u0120\u0121\f\t\2\2\u0121\u0122\t"+
		"\7\2\2\u0122\u0124\5.\30\n\u0123\u0114\3\2\2\2\u0123\u0117\3\2\2\2\u0123"+
		"\u011a\3\2\2\2\u0123\u011d\3\2\2\2\u0123\u0120\3\2\2\2\u0124\u0127\3\2"+
		"\2\2\u0125\u0123\3\2\2\2\u0125\u0126\3\2\2\2\u0126/\3\2\2\2\u0127\u0125"+
		"\3\2\2\2\u0128\u0129\7>\2\2\u0129\u012b\7\64\2\2\u012a\u012c\5 \21\2\u012b"+
		"\u012a\3\2\2\2\u012b\u012c\3\2\2\2\u012c\u012d\3\2\2\2\u012d\u012e\7\65"+
		"\2\2\u012e\61\3\2\2\2\u012f\u0130\7>\2\2\u0130\u0131\7\62\2\2\u0131\u0132"+
		"\5.\30\2\u0132\u0133\7\63\2\2\u0133\63\3\2\2\2\u0134\u0135\7\17\2\2\u0135"+
		"\u0136\7>\2\2\u0136\65\3\2\2\2\u0137\u013d\58\35\2\u0138\u013d\7<\2\2"+
		"\u0139\u013d\5<\37\2\u013a\u013d\7=\2\2\u013b\u013d\7\13\2\2\u013c\u0137"+
		"\3\2\2\2\u013c\u0138\3\2\2\2\u013c\u0139\3\2\2\2\u013c\u013a\3\2\2\2\u013c"+
		"\u013b\3\2\2\2\u013d\67\3\2\2\2\u013e\u013f\t\b\2\2\u013f9\3\2\2\2\u0140"+
		"\u0141\7\64\2\2\u0141\u0142\5.\30\2\u0142\u0143\7\65\2\2\u0143;\3\2\2"+
		"\2\u0144\u0145\t\t\2\2\u0145=\3\2\2\2\u0146\u0147\t\n\2\2\u0147?\3\2\2"+
		"\2\37AHNTkmqz\177\u0089\u008d\u009d\u00a4\u00aa\u00b1\u00bb\u00c9\u00cf"+
		"\u00d8\u00e1\u00ee\u00f0\u00fc\u0103\u0112\u0123\u0125\u012b\u013c";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}