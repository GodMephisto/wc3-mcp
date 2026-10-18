---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser"
source_path: "antlr4JS/Jass/.antlr/JassLexer.java"
ref: HEAD
---

// Generated from d:\JASS_TO_JS_PARSER\antlr4JS\Jass\Jass.g4 by ANTLR 4.7.1
import org.antlr.v4.runtime.Lexer;
import org.antlr.v4.runtime.CharStream;
import org.antlr.v4.runtime.Token;
import org.antlr.v4.runtime.TokenStream;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.misc.*;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class JassLexer extends Lexer {
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
	public static String[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static String[] modeNames = {
		"DEFAULT_MODE"
	};

	public static final String[] ruleNames = {
		"K_CODE", "K_HANDLE", "K_INTEGER", "K_REAL", "K_BOOLEAN", "K_STRING", 
		"K_TRUE", "K_FALSE", "K_NULL", "K_NOT", "K_AND", "K_OR", "K_FUNCTION", 
		"K_RETURN", "K_RETURNS", "K_EXITWHEN", "K_LOOP", "K_ENDLOOP", "K_ELSEIF", 
		"K_ELSE", "K_CALL", "K_IF", "K_THEN", "K_ENDIF", "K_DEBUG", "K_SET", "K_TYPE", 
		"K_EXTENDS", "K_GLOBALS", "K_ENDGLOBALS", "K_CONSTANT", "K_NATIVE", "K_TAKES", 
		"K_NOTHING", "K_ENDFUNCTION", "K_LOCAL", "K_ARRAY", "ADD", "SUB", "MULT", 
		"DIV", "GT", "LT", "EQ", "NEQ", "GTEQ", "LTEQ", "O_BRACK", "C_BRACK", 
		"O_PAR", "C_PAR", "COMMA", "ASSIGN", "DECIMAL", "OCTAL", "HEX", "FOURCC", 
		"REAL_CONST", "STRING_CONST", "ID", "NEWLINE", "SPACE", "COMMENT"
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


	public JassLexer(CharStream input) {
		super(input);
		_interp = new LexerATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@Override
	public String getGrammarFileName() { return "Jass.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public String[] getChannelNames() { return channelNames; }

	@Override
	public String[] getModeNames() { return modeNames; }

	@Override
	public ATN getATN() { return _ATN; }

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\2A\u01f0\b\1\4\2\t"+
		"\2\4\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13"+
		"\t\13\4\f\t\f\4\r\t\r\4\16\t\16\4\17\t\17\4\20\t\20\4\21\t\21\4\22\t\22"+
		"\4\23\t\23\4\24\t\24\4\25\t\25\4\26\t\26\4\27\t\27\4\30\t\30\4\31\t\31"+
		"\4\32\t\32\4\33\t\33\4\34\t\34\4\35\t\35\4\36\t\36\4\37\t\37\4 \t \4!"+
		"\t!\4\"\t\"\4#\t#\4$\t$\4%\t%\4&\t&\4\'\t\'\4(\t(\4)\t)\4*\t*\4+\t+\4"+
		",\t,\4-\t-\4.\t.\4/\t/\4\60\t\60\4\61\t\61\4\62\t\62\4\63\t\63\4\64\t"+
		"\64\4\65\t\65\4\66\t\66\4\67\t\67\48\t8\49\t9\4:\t:\4;\t;\4<\t<\4=\t="+
		"\4>\t>\4?\t?\4@\t@\3\2\3\2\3\2\3\2\3\2\3\3\3\3\3\3\3\3\3\3\3\3\3\3\3\4"+
		"\3\4\3\4\3\4\3\4\3\4\3\4\3\4\3\5\3\5\3\5\3\5\3\5\3\6\3\6\3\6\3\6\3\6\3"+
		"\6\3\6\3\6\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\b\3\b\3\b\3\b\3\b\3\t\3\t\3\t"+
		"\3\t\3\t\3\t\3\n\3\n\3\n\3\n\3\n\3\13\3\13\3\13\3\13\3\f\3\f\3\f\3\f\3"+
		"\r\3\r\3\r\3\16\3\16\3\16\3\16\3\16\3\16\3\16\3\16\3\16\3\17\3\17\3\17"+
		"\3\17\3\17\3\17\3\17\3\20\3\20\3\20\3\20\3\20\3\20\3\20\3\20\3\21\3\21"+
		"\3\21\3\21\3\21\3\21\3\21\3\21\3\21\3\22\3\22\3\22\3\22\3\22\3\23\3\23"+
		"\3\23\3\23\3\23\3\23\3\23\3\23\3\24\3\24\3\24\3\24\3\24\3\24\3\24\3\25"+
		"\3\25\3\25\3\25\3\25\3\26\3\26\3\26\3\26\3\26\3\27\3\27\3\27\3\30\3\30"+
		"\3\30\3\30\3\30\3\31\3\31\3\31\3\31\3\31\3\31\3\32\3\32\3\32\3\32\3\32"+
		"\3\32\3\33\3\33\3\33\3\33\3\34\3\34\3\34\3\34\3\34\3\35\3\35\3\35\3\35"+
		"\3\35\3\35\3\35\3\35\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\37\3\37"+
		"\3\37\3\37\3\37\3\37\3\37\3\37\3\37\3\37\3\37\3 \3 \3 \3 \3 \3 \3 \3 "+
		"\3 \3!\3!\3!\3!\3!\3!\3!\3\"\3\"\3\"\3\"\3\"\3\"\3#\3#\3#\3#\3#\3#\3#"+
		"\3#\3$\3$\3$\3$\3$\3$\3$\3$\3$\3$\3$\3$\3%\3%\3%\3%\3%\3%\3&\3&\3&\3&"+
		"\3&\3&\3\'\3\'\3(\3(\3)\3)\3*\3*\3+\3+\3,\3,\3-\3-\3-\3.\3.\3.\3/\3/\3"+
		"/\3\60\3\60\3\60\3\61\3\61\3\62\3\62\3\63\3\63\3\64\3\64\3\65\3\65\3\66"+
		"\3\66\3\67\3\67\7\67\u0198\n\67\f\67\16\67\u019b\13\67\38\38\78\u019f"+
		"\n8\f8\168\u01a2\138\39\39\69\u01a6\n9\r9\169\u01a7\39\39\39\69\u01ad"+
		"\n9\r9\169\u01ae\59\u01b1\n9\3:\3:\3:\3:\3:\3:\3:\3;\6;\u01bb\n;\r;\16"+
		";\u01bc\3;\3;\7;\u01c1\n;\f;\16;\u01c4\13;\3;\3;\6;\u01c8\n;\r;\16;\u01c9"+
		"\5;\u01cc\n;\3<\3<\7<\u01d0\n<\f<\16<\u01d3\13<\3<\3<\3=\3=\7=\u01d9\n"+
		"=\f=\16=\u01dc\13=\3=\5=\u01df\n=\3>\6>\u01e2\n>\r>\16>\u01e3\3?\3?\3"+
		"?\3?\3@\3@\3@\3@\3@\3@\3@\3\u01d1\2A\3\3\5\4\7\5\t\6\13\7\r\b\17\t\21"+
		"\n\23\13\25\f\27\r\31\16\33\17\35\20\37\21!\22#\23%\24\'\25)\26+\27-\30"+
		"/\31\61\32\63\33\65\34\67\359\36;\37= ?!A\"C#E$G%I&K\'M(O)Q*S+U,W-Y.["+
		"/]\60_\61a\62c\63e\64g\65i\66k\67m8o9q:s;u<w=y>{?}@\177A\3\2\r\3\2\63"+
		";\3\2\62;\3\2\629\5\2\62;CHch\4\2ZZzz\3\2))\4\2C\\c|\6\2\62;C\\aac|\5"+
		"\2\62;C\\c|\4\2\f\f\17\17\4\2\13\13\"\"\2\u01fc\2\3\3\2\2\2\2\5\3\2\2"+
		"\2\2\7\3\2\2\2\2\t\3\2\2\2\2\13\3\2\2\2\2\r\3\2\2\2\2\17\3\2\2\2\2\21"+
		"\3\2\2\2\2\23\3\2\2\2\2\25\3\2\2\2\2\27\3\2\2\2\2\31\3\2\2\2\2\33\3\2"+
		"\2\2\2\35\3\2\2\2\2\37\3\2\2\2\2!\3\2\2\2\2#\3\2\2\2\2%\3\2\2\2\2\'\3"+
		"\2\2\2\2)\3\2\2\2\2+\3\2\2\2\2-\3\2\2\2\2/\3\2\2\2\2\61\3\2\2\2\2\63\3"+
		"\2\2\2\2\65\3\2\2\2\2\67\3\2\2\2\29\3\2\2\2\2;\3\2\2\2\2=\3\2\2\2\2?\3"+
		"\2\2\2\2A\3\2\2\2\2C\3\2\2\2\2E\3\2\2\2\2G\3\2\2\2\2I\3\2\2\2\2K\3\2\2"+
		"\2\2M\3\2\2\2\2O\3\2\2\2\2Q\3\2\2\2\2S\3\2\2\2\2U\3\2\2\2\2W\3\2\2\2\2"+
		"Y\3\2\2\2\2[\3\2\2\2\2]\3\2\2\2\2_\3\2\2\2\2a\3\2\2\2\2c\3\2\2\2\2e\3"+
		"\2\2\2\2g\3\2\2\2\2i\3\2\2\2\2k\3\2\2\2\2m\3\2\2\2\2o\3\2\2\2\2q\3\2\2"+
		"\2\2s\3\2\2\2\2u\3\2\2\2\2w\3\2\2\2\2y\3\2\2\2\2{\3\2\2\2\2}\3\2\2\2\2"+
		"\177\3\2\2\2\3\u0081\3\2\2\2\5\u0086\3\2\2\2\7\u008d\3\2\2\2\t\u0095\3"+
		"\2\2\2\13\u009a\3\2\2\2\r\u00a2\3\2\2\2\17\u00a9\3\2\2\2\21\u00ae\3\2"+
		"\2\2\23\u00b4\3\2\2\2\25\u00b9\3\2\2\2\27\u00bd\3\2\2\2\31\u00c1\3\2\2"+
		"\2\33\u00c4\3\2\2\2\35\u00cd\3\2\2\2\37\u00d4\3\2\2\2!\u00dc\3\2\2\2#"+
		"\u00e5\3\2\2\2%\u00ea\3\2\2\2\'\u00f2\3\2\2\2)\u00f9\3\2\2\2+\u00fe\3"+
		"\2\2\2-\u0103\3\2\2\2/\u0106\3\2\2\2\61\u010b\3\2\2\2\63\u0111\3\2\2\2"+
		"\65\u0117\3\2\2\2\67\u011b\3\2\2\29\u0120\3\2\2\2;\u0128\3\2\2\2=\u0130"+
		"\3\2\2\2?\u013b\3\2\2\2A\u0144\3\2\2\2C\u014b\3\2\2\2E\u0151\3\2\2\2G"+
		"\u0159\3\2\2\2I\u0165\3\2\2\2K\u016b\3\2\2\2M\u0171\3\2\2\2O\u0173\3\2"+
		"\2\2Q\u0175\3\2\2\2S\u0177\3\2\2\2U\u0179\3\2\2\2W\u017b\3\2\2\2Y\u017d"+
		"\3\2\2\2[\u0180\3\2\2\2]\u0183\3\2\2\2_\u0186\3\2\2\2a\u0189\3\2\2\2c"+
		"\u018b\3\2\2\2e\u018d\3\2\2\2g\u018f\3\2\2\2i\u0191\3\2\2\2k\u0193\3\2"+
		"\2\2m\u0195\3\2\2\2o\u019c\3\2\2\2q\u01b0\3\2\2\2s\u01b2\3\2\2\2u\u01cb"+
		"\3\2\2\2w\u01cd\3\2\2\2y\u01d6\3\2\2\2{\u01e1\3\2\2\2}\u01e5\3\2\2\2\177"+
		"\u01e9\3\2\2\2\u0081\u0082\7e\2\2\u0082\u0083\7q\2\2\u0083\u0084\7f\2"+
		"\2\u0084\u0085\7g\2\2\u0085\4\3\2\2\2\u0086\u0087\7j\2\2\u0087\u0088\7"+
		"c\2\2\u0088\u0089\7p\2\2\u0089\u008a\7f\2\2\u008a\u008b\7n\2\2\u008b\u008c"+
		"\7g\2\2\u008c\6\3\2\2\2\u008d\u008e\7k\2\2\u008e\u008f\7p\2\2\u008f\u0090"+
		"\7v\2\2\u0090\u0091\7g\2\2\u0091\u0092\7i\2\2\u0092\u0093\7g\2\2\u0093"+
		"\u0094\7t\2\2\u0094\b\3\2\2\2\u0095\u0096\7t\2\2\u0096\u0097\7g\2\2\u0097"+
		"\u0098\7c\2\2\u0098\u0099\7n\2\2\u0099\n\3\2\2\2\u009a\u009b\7d\2\2\u009b"+
		"\u009c\7q\2\2\u009c\u009d\7q\2\2\u009d\u009e\7n\2\2\u009e\u009f\7g\2\2"+
		"\u009f\u00a0\7c\2\2\u00a0\u00a1\7p\2\2\u00a1\f\3\2\2\2\u00a2\u00a3\7u"+
		"\2\2\u00a3\u00a4\7v\2\2\u00a4\u00a5\7t\2\2\u00a5\u00a6\7k\2\2\u00a6\u00a7"+
		"\7p\2\2\u00a7\u00a8\7i\2\2\u00a8\16\3\2\2\2\u00a9\u00aa\7v\2\2\u00aa\u00ab"+
		"\7t\2\2\u00ab\u00ac\7w\2\2\u00ac\u00ad\7g\2\2\u00ad\20\3\2\2\2\u00ae\u00af"+
		"\7h\2\2\u00af\u00b0\7c\2\2\u00b0\u00b1\7n\2\2\u00b1\u00b2\7u\2\2\u00b2"+
		"\u00b3\7g\2\2\u00b3\22\3\2\2\2\u00b4\u00b5\7p\2\2\u00b5\u00b6\7w\2\2\u00b6"+
		"\u00b7\7n\2\2\u00b7\u00b8\7n\2\2\u00b8\24\3\2\2\2\u00b9\u00ba\7p\2\2\u00ba"+
		"\u00bb\7q\2\2\u00bb\u00bc\7v\2\2\u00bc\26\3\2\2\2\u00bd\u00be\7c\2\2\u00be"+
		"\u00bf\7p\2\2\u00bf\u00c0\7f\2\2\u00c0\30\3\2\2\2\u00c1\u00c2\7q\2\2\u00c2"+
		"\u00c3\7t\2\2\u00c3\32\3\2\2\2\u00c4\u00c5\7h\2\2\u00c5\u00c6\7w\2\2\u00c6"+
		"\u00c7\7p\2\2\u00c7\u00c8\7e\2\2\u00c8\u00c9\7v\2\2\u00c9\u00ca\7k\2\2"+
		"\u00ca\u00cb\7q\2\2\u00cb\u00cc\7p\2\2\u00cc\34\3\2\2\2\u00cd\u00ce\7"+
		"t\2\2\u00ce\u00cf\7g\2\2\u00cf\u00d0\7v\2\2\u00d0\u00d1\7w\2\2\u00d1\u00d2"+
		"\7t\2\2\u00d2\u00d3\7p\2\2\u00d3\36\3\2\2\2\u00d4\u00d5\7t\2\2\u00d5\u00d6"+
		"\7g\2\2\u00d6\u00d7\7v\2\2\u00d7\u00d8\7w\2\2\u00d8\u00d9\7t\2\2\u00d9"+
		"\u00da\7p\2\2\u00da\u00db\7u\2\2\u00db \3\2\2\2\u00dc\u00dd\7g\2\2\u00dd"+
		"\u00de\7z\2\2\u00de\u00df\7k\2\2\u00df\u00e0\7v\2\2\u00e0\u00e1\7y\2\2"+
		"\u00e1\u00e2\7j\2\2\u00e2\u00e3\7g\2\2\u00e3\u00e4\7p\2\2\u00e4\"\3\2"+
		"\2\2\u00e5\u00e6\7n\2\2\u00e6\u00e7\7q\2\2\u00e7\u00e8\7q\2\2\u00e8\u00e9"+
		"\7r\2\2\u00e9$\3\2\2\2\u00ea\u00eb\7g\2\2\u00eb\u00ec\7p\2\2\u00ec\u00ed"+
		"\7f\2\2\u00ed\u00ee\7n\2\2\u00ee\u00ef\7q\2\2\u00ef\u00f0\7q\2\2\u00f0"+
		"\u00f1\7r\2\2\u00f1&\3\2\2\2\u00f2\u00f3\7g\2\2\u00f3\u00f4\7n\2\2\u00f4"+
		"\u00f5\7u\2\2\u00f5\u00f6\7g\2\2\u00f6\u00f7\7k\2\2\u00f7\u00f8\7h\2\2"+
		"\u00f8(\3\2\2\2\u00f9\u00fa\7g\2\2\u00fa\u00fb\7n\2\2\u00fb\u00fc\7u\2"+
		"\2\u00fc\u00fd\7g\2\2\u00fd*\3\2\2\2\u00fe\u00ff\7e\2\2\u00ff\u0100\7"+
		"c\2\2\u0100\u0101\7n\2\2\u0101\u0102\7n\2\2\u0102,\3\2\2\2\u0103\u0104"+
		"\7k\2\2\u0104\u0105\7h\2\2\u0105.\3\2\2\2\u0106\u0107\7v\2\2\u0107\u0108"+
		"\7j\2\2\u0108\u0109\7g\2\2\u0109\u010a\7p\2\2\u010a\60\3\2\2\2\u010b\u010c"+
		"\7g\2\2\u010c\u010d\7p\2\2\u010d\u010e\7f\2\2\u010e\u010f\7k\2\2\u010f"+
		"\u0110\7h\2\2\u0110\62\3\2\2\2\u0111\u0112\7f\2\2\u0112\u0113\7g\2\2\u0113"+
		"\u0114\7d\2\2\u0114\u0115\7w\2\2\u0115\u0116\7i\2\2\u0116\64\3\2\2\2\u0117"+
		"\u0118\7u\2\2\u0118\u0119\7g\2\2\u0119\u011a\7v\2\2\u011a\66\3\2\2\2\u011b"+
		"\u011c\7v\2\2\u011c\u011d\7{\2\2\u011d\u011e\7r\2\2\u011e\u011f\7g\2\2"+
		"\u011f8\3\2\2\2\u0120\u0121\7g\2\2\u0121\u0122\7z\2\2\u0122\u0123\7v\2"+
		"\2\u0123\u0124\7g\2\2\u0124\u0125\7p\2\2\u0125\u0126\7f\2\2\u0126\u0127"+
		"\7u\2\2\u0127:\3\2\2\2\u0128\u0129\7i\2\2\u0129\u012a\7n\2\2\u012a\u012b"+
		"\7q\2\2\u012b\u012c\7d\2\2\u012c\u012d\7c\2\2\u012d\u012e\7n\2\2\u012e"+
		"\u012f\7u\2\2\u012f<\3\2\2\2\u0130\u0131\7g\2\2\u0131\u0132\7p\2\2\u0132"+
		"\u0133\7f\2\2\u0133\u0134\7i\2\2\u0134\u0135\7n\2\2\u0135\u0136\7q\2\2"+
		"\u0136\u0137\7d\2\2\u0137\u0138\7c\2\2\u0138\u0139\7n\2\2\u0139\u013a"+
		"\7u\2\2\u013a>\3\2\2\2\u013b\u013c\7e\2\2\u013c\u013d\7q\2\2\u013d\u013e"+
		"\7p\2\2\u013e\u013f\7u\2\2\u013f\u0140\7v\2\2\u0140\u0141\7c\2\2\u0141"+
		"\u0142\7p\2\2\u0142\u0143\7v\2\2\u0143@\3\2\2\2\u0144\u0145\7p\2\2\u0145"+
		"\u0146\7c\2\2\u0146\u0147\7v\2\2\u0147\u0148\7k\2\2\u0148\u0149\7x\2\2"+
		"\u0149\u014a\7g\2\2\u014aB\3\2\2\2\u014b\u014c\7v\2\2\u014c\u014d\7c\2"+
		"\2\u014d\u014e\7m\2\2\u014e\u014f\7g\2\2\u014f\u0150\7u\2\2\u0150D\3\2"+
		"\2\2\u0151\u0152\7p\2\2\u0152\u0153\7q\2\2\u0153\u0154\7v\2\2\u0154\u0155"+
		"\7j\2\2\u0155\u0156\7k\2\2\u0156\u0157\7p\2\2\u0157\u0158\7i\2\2\u0158"+
		"F\3\2\2\2\u0159\u015a\7g\2\2\u015a\u015b\7p\2\2\u015b\u015c\7f\2\2\u015c"+
		"\u015d\7h\2\2\u015d\u015e\7w\2\2\u015e\u015f\7p\2\2\u015f\u0160\7e\2\2"+
		"\u0160\u0161\7v\2\2\u0161\u0162\7k\2\2\u0162\u0163\7q\2\2\u0163\u0164"+
		"\7p\2\2\u0164H\3\2\2\2\u0165\u0166\7n\2\2\u0166\u0167\7q\2\2\u0167\u0168"+
		"\7e\2\2\u0168\u0169\7c\2\2\u0169\u016a\7n\2\2\u016aJ\3\2\2\2\u016b\u016c"+
		"\7c\2\2\u016c\u016d\7t\2\2\u016d\u016e\7t\2\2\u016e\u016f\7c\2\2\u016f"+
		"\u0170\7{\2\2\u0170L\3\2\2\2\u0171\u0172\7-\2\2\u0172N\3\2\2\2\u0173\u0174"+
		"\7/\2\2\u0174P\3\2\2\2\u0175\u0176\7,\2\2\u0176R\3\2\2\2\u0177\u0178\7"+
		"\61\2\2\u0178T\3\2\2\2\u0179\u017a\7@\2\2\u017aV\3\2\2\2\u017b\u017c\7"+
		">\2\2\u017cX\3\2\2\2\u017d\u017e\7?\2\2\u017e\u017f\7?\2\2\u017fZ\3\2"+
		"\2\2\u0180\u0181\7#\2\2\u0181\u0182\7?\2\2\u0182\\\3\2\2\2\u0183\u0184"+
		"\7@\2\2\u0184\u0185\7?\2\2\u0185^\3\2\2\2\u0186\u0187\7>\2\2\u0187\u0188"+
		"\7?\2\2\u0188`\3\2\2\2\u0189\u018a\7]\2\2\u018ab\3\2\2\2\u018b\u018c\7"+
		"_\2\2\u018cd\3\2\2\2\u018d\u018e\7*\2\2\u018ef\3\2\2\2\u018f\u0190\7+"+
		"\2\2\u0190h\3\2\2\2\u0191\u0192\7.\2\2\u0192j\3\2\2\2\u0193\u0194\7?\2"+
		"\2\u0194l\3\2\2\2\u0195\u0199\t\2\2\2\u0196\u0198\t\3\2\2\u0197\u0196"+
		"\3\2\2\2\u0198\u019b\3\2\2\2\u0199\u0197\3\2\2\2\u0199\u019a\3\2\2\2\u019a"+
		"n\3\2\2\2\u019b\u0199\3\2\2\2\u019c\u01a0\7\62\2\2\u019d\u019f\t\4\2\2"+
		"\u019e\u019d\3\2\2\2\u019f\u01a2\3\2\2\2\u01a0\u019e\3\2\2\2\u01a0\u01a1"+
		"\3\2\2\2\u01a1p\3\2\2\2\u01a2\u01a0\3\2\2\2\u01a3\u01a5\7&\2\2\u01a4\u01a6"+
		"\t\5\2\2\u01a5\u01a4\3\2\2\2\u01a6\u01a7\3\2\2\2\u01a7\u01a5\3\2\2\2\u01a7"+
		"\u01a8\3\2\2\2\u01a8\u01b1\3\2\2\2\u01a9\u01aa\7\62\2\2\u01aa\u01ac\t"+
		"\6\2\2\u01ab\u01ad\t\5\2\2\u01ac\u01ab\3\2\2\2\u01ad\u01ae\3\2\2\2\u01ae"+
		"\u01ac\3\2\2\2\u01ae\u01af\3\2\2\2\u01af\u01b1\3\2\2\2\u01b0\u01a3\3\2"+
		"\2\2\u01b0\u01a9\3\2\2\2\u01b1r\3\2\2\2\u01b2\u01b3\t\7\2\2\u01b3\u01b4"+
		"\13\2\2\2\u01b4\u01b5\13\2\2\2\u01b5\u01b6\13\2\2\2\u01b6\u01b7\13\2\2"+
		"\2\u01b7\u01b8\t\7\2\2\u01b8t\3\2\2\2\u01b9\u01bb\t\3\2\2\u01ba\u01b9"+
		"\3\2\2\2\u01bb\u01bc\3\2\2\2\u01bc\u01ba\3\2\2\2\u01bc\u01bd\3\2\2\2\u01bd"+
		"\u01be\3\2\2\2\u01be\u01c2\7\60\2\2\u01bf\u01c1\t\3\2\2\u01c0\u01bf\3"+
		"\2\2\2\u01c1\u01c4\3\2\2\2\u01c2\u01c0\3\2\2\2\u01c2\u01c3\3\2\2\2\u01c3"+
		"\u01cc\3\2\2\2\u01c4\u01c2\3\2\2\2\u01c5\u01c7\7\60\2\2\u01c6\u01c8\t"+
		"\3\2\2\u01c7\u01c6\3\2\2\2\u01c8\u01c9\3\2\2\2\u01c9\u01c7\3\2\2\2\u01c9"+
		"\u01ca\3\2\2\2\u01ca\u01cc\3\2\2\2\u01cb\u01ba\3\2\2\2\u01cb\u01c5\3\2"+
		"\2\2\u01ccv\3\2\2\2\u01cd\u01d1\7$\2\2\u01ce\u01d0\13\2\2\2\u01cf\u01ce"+
		"\3\2\2\2\u01d0\u01d3\3\2\2\2\u01d1\u01d2\3\2\2\2\u01d1\u01cf\3\2\2\2\u01d2"+
		"\u01d4\3\2\2\2\u01d3\u01d1\3\2\2\2\u01d4\u01d5\7$\2\2\u01d5x\3\2\2\2\u01d6"+
		"\u01de\t\b\2\2\u01d7\u01d9\t\t\2\2\u01d8\u01d7\3\2\2\2\u01d9\u01dc\3\2"+
		"\2\2\u01da\u01d8\3\2\2\2\u01da\u01db\3\2\2\2\u01db\u01dd\3\2\2\2\u01dc"+
		"\u01da\3\2\2\2\u01dd\u01df\t\n\2\2\u01de\u01da\3\2\2\2\u01de\u01df\3\2"+
		"\2\2\u01dfz\3\2\2\2\u01e0\u01e2\t\13\2\2\u01e1\u01e0\3\2\2\2\u01e2\u01e3"+
		"\3\2\2\2\u01e3\u01e1\3\2\2\2\u01e3\u01e4\3\2\2\2\u01e4|\3\2\2\2\u01e5"+
		"\u01e6\t\f\2\2\u01e6\u01e7\3\2\2\2\u01e7\u01e8\b?\2\2\u01e8~\3\2\2\2\u01e9"+
		"\u01ea\7\61\2\2\u01ea\u01eb\7\61\2\2\u01eb\u01ec\3\2\2\2\u01ec\u01ed\n"+
		"\13\2\2\u01ed\u01ee\3\2\2\2\u01ee\u01ef\b@\2\2\u01ef\u0080\3\2\2\2\20"+
		"\2\u0199\u01a0\u01a7\u01ae\u01b0\u01bc\u01c2\u01c9\u01cb\u01d1\u01da\u01de"+
		"\u01e3\3\b\2\2";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}