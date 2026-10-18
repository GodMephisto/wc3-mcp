---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser"
source_path: "antlr4JS/Jass/JassParser.js"
ref: HEAD
---

// Generated from Jass/Jass.g4 by ANTLR 4.7.2
// jshint ignore: start
var antlr4 = require('antlr4/index');
var JassListener = require('./JassListener').JassListener;
var grammarFileName = "Jass.g4";


var serializedATN = ["\u0003\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964",
    "\u0003A\u0149\u0004\u0002\t\u0002\u0004\u0003\t\u0003\u0004\u0004\t",
    "\u0004\u0004\u0005\t\u0005\u0004\u0006\t\u0006\u0004\u0007\t\u0007\u0004",
    "\b\t\b\u0004\t\t\t\u0004\n\t\n\u0004\u000b\t\u000b\u0004\f\t\f\u0004",
    "\r\t\r\u0004\u000e\t\u000e\u0004\u000f\t\u000f\u0004\u0010\t\u0010\u0004",
    "\u0011\t\u0011\u0004\u0012\t\u0012\u0004\u0013\t\u0013\u0004\u0014\t",
    "\u0014\u0004\u0015\t\u0015\u0004\u0016\t\u0016\u0004\u0017\t\u0017\u0004",
    "\u0018\t\u0018\u0004\u0019\t\u0019\u0004\u001a\t\u001a\u0004\u001b\t",
    "\u001b\u0004\u001c\t\u001c\u0004\u001d\t\u001d\u0004\u001e\t\u001e\u0004",
    "\u001f\t\u001f\u0004 \t \u0003\u0002\u0005\u0002B\n\u0002\u0003\u0002",
    "\u0003\u0002\u0003\u0002\u0007\u0002G\n\u0002\f\u0002\u000e\u0002J\u000b",
    "\u0002\u0003\u0002\u0007\u0002M\n\u0002\f\u0002\u000e\u0002P\u000b\u0002",
    "\u0003\u0003\u0003\u0003\u0003\u0003\u0005\u0003U\n\u0003\u0003\u0004",
    "\u0003\u0004\u0003\u0004\u0003\u0004\u0003\u0004\u0003\u0005\u0003\u0005",
    "\u0003\u0005\u0003\u0005\u0003\u0005\u0003\u0005\u0003\u0006\u0003\u0006",
    "\u0003\u0006\u0003\u0006\u0003\u0006\u0003\u0006\u0003\u0006\u0003\u0006",
    "\u0003\u0006\u0003\u0006\u0007\u0006l\n\u0006\f\u0006\u000e\u0006o\u000b",
    "\u0006\u0003\u0007\u0005\u0007r\n\u0007\u0003\u0007\u0003\u0007\u0003",
    "\u0007\u0003\b\u0003\b\u0003\b\u0003\b\u0005\b{\n\b\u0003\b\u0003\b",
    "\u0003\b\u0005\b\u0080\n\b\u0003\t\u0003\t\u0003\t\u0003\t\u0003\t\u0003",
    "\t\u0007\t\u0088\n\t\f\t\u000e\t\u008b\u000b\t\u0003\n\u0005\n\u008e",
    "\n\n\u0003\n\u0003\n\u0003\n\u0003\n\u0003\n\u0003\n\u0003\n\u0003\n",
    "\u0003\u000b\u0003\u000b\u0003\u000b\u0003\u000b\u0007\u000b\u009c\n",
    "\u000b\f\u000b\u000e\u000b\u009f\u000b\u000b\u0003\f\u0003\f\u0003\f",
    "\u0003\f\u0005\f\u00a5\n\f\u0003\f\u0003\f\u0003\f\u0003\f\u0005\f\u00ab",
    "\n\f\u0003\r\u0003\r\u0003\r\u0007\r\u00b0\n\r\f\r\u000e\r\u00b3\u000b",
    "\r\u0003\u000e\u0003\u000e\u0003\u000e\u0003\u000e\u0003\u000e\u0003",
    "\u000e\u0003\u000e\u0005\u000e\u00bc\n\u000e\u0003\u000f\u0003\u000f",
    "\u0003\u000f\u0003\u000f\u0003\u000f\u0003\u000f\u0003\u000f\u0003\u000f",
    "\u0003\u000f\u0003\u000f\u0003\u000f\u0003\u000f\u0005\u000f\u00ca\n",
    "\u000f\u0003\u0010\u0003\u0010\u0003\u0010\u0003\u0010\u0005\u0010\u00d0",
    "\n\u0010\u0003\u0010\u0003\u0010\u0003\u0011\u0003\u0011\u0003\u0011",
    "\u0007\u0011\u00d7\n\u0011\f\u0011\u000e\u0011\u00da\u000b\u0011\u0003",
    "\u0012\u0003\u0012\u0003\u0012\u0003\u0012\u0003\u0012\u0003\u0012\u0005",
    "\u0012\u00e2\n\u0012\u0003\u0012\u0003\u0012\u0003\u0013\u0003\u0013",
    "\u0003\u0013\u0003\u0013\u0003\u0013\u0003\u0013\u0003\u0013\u0003\u0013",
    "\u0003\u0013\u0005\u0013\u00ef\n\u0013\u0005\u0013\u00f1\n\u0013\u0003",
    "\u0014\u0003\u0014\u0003\u0014\u0003\u0014\u0003\u0014\u0003\u0015\u0003",
    "\u0015\u0003\u0015\u0003\u0016\u0003\u0016\u0005\u0016\u00fd\n\u0016",
    "\u0003\u0017\u0003\u0017\u0003\u0017\u0003\u0017\u0003\u0017\u0005\u0017",
    "\u0104\n\u0017\u0003\u0018\u0003\u0018\u0003\u0018\u0003\u0018\u0003",
    "\u0018\u0003\u0018\u0003\u0018\u0003\u0018\u0003\u0018\u0003\u0018\u0003",
    "\u0018\u0003\u0018\u0003\u0018\u0005\u0018\u0113\n\u0018\u0003\u0018",
    "\u0003\u0018\u0003\u0018\u0003\u0018\u0003\u0018\u0003\u0018\u0003\u0018",
    "\u0003\u0018\u0003\u0018\u0003\u0018\u0003\u0018\u0003\u0018\u0003\u0018",
    "\u0003\u0018\u0003\u0018\u0007\u0018\u0124\n\u0018\f\u0018\u000e\u0018",
    "\u0127\u000b\u0018\u0003\u0019\u0003\u0019\u0003\u0019\u0005\u0019\u012c",
    "\n\u0019\u0003\u0019\u0003\u0019\u0003\u001a\u0003\u001a\u0003\u001a",
    "\u0003\u001a\u0003\u001a\u0003\u001b\u0003\u001b\u0003\u001b\u0003\u001c",
    "\u0003\u001c\u0003\u001c\u0003\u001c\u0003\u001c\u0005\u001c\u013d\n",
    "\u001c\u0003\u001d\u0003\u001d\u0003\u001e\u0003\u001e\u0003\u001e\u0003",
    "\u001e\u0003\u001f\u0003\u001f\u0003 \u0003 \u0003 \u0002\u0003.!\u0002",
    "\u0004\u0006\b\n\f\u000e\u0010\u0012\u0014\u0016\u0018\u001a\u001c\u001e",
    " \"$&(*,.02468:<>\u0002\u000b\u0004\u0002\u0004\u0004>>\u0003\u0002",
    "*+\u0003\u0002()\u0004\u0002,-01\u0003\u0002./\u0003\u0002\r\u000e\u0003",
    "\u00028;\u0003\u0002\t\n\u0004\u0002\u0003\b>>\u0002\u015b\u0002A\u0003",
    "\u0002\u0002\u0002\u0004T\u0003\u0002\u0002\u0002\u0006V\u0003\u0002",
    "\u0002\u0002\b[\u0003\u0002\u0002\u0002\nm\u0003\u0002\u0002\u0002\f",
    "q\u0003\u0002\u0002\u0002\u000ev\u0003\u0002\u0002\u0002\u0010\u0081",
    "\u0003\u0002\u0002\u0002\u0012\u008d\u0003\u0002\u0002\u0002\u0014\u009d",
    "\u0003\u0002\u0002\u0002\u0016\u00aa\u0003\u0002\u0002\u0002\u0018\u00b1",
    "\u0003\u0002\u0002\u0002\u001a\u00bb\u0003\u0002\u0002\u0002\u001c\u00c9",
    "\u0003\u0002\u0002\u0002\u001e\u00cb\u0003\u0002\u0002\u0002 \u00d3",
    "\u0003\u0002\u0002\u0002\"\u00db\u0003\u0002\u0002\u0002$\u00f0\u0003",
    "\u0002\u0002\u0002&\u00f2\u0003\u0002\u0002\u0002(\u00f7\u0003\u0002",
    "\u0002\u0002*\u00fa\u0003\u0002\u0002\u0002,\u00fe\u0003\u0002\u0002",
    "\u0002.\u0112\u0003\u0002\u0002\u00020\u0128\u0003\u0002\u0002\u0002",
    "2\u012f\u0003\u0002\u0002\u00024\u0134\u0003\u0002\u0002\u00026\u013c",
    "\u0003\u0002\u0002\u00028\u013e\u0003\u0002\u0002\u0002:\u0140\u0003",
    "\u0002\u0002\u0002<\u0144\u0003\u0002\u0002\u0002>\u0146\u0003\u0002",
    "\u0002\u0002@B\u0007?\u0002\u0002A@\u0003\u0002\u0002\u0002AB\u0003",
    "\u0002\u0002\u0002BH\u0003\u0002\u0002\u0002CD\u0005\u0004\u0003\u0002",
    "DE\u0007?\u0002\u0002EG\u0003\u0002\u0002\u0002FC\u0003\u0002\u0002",
    "\u0002GJ\u0003\u0002\u0002\u0002HF\u0003\u0002\u0002\u0002HI\u0003\u0002",
    "\u0002\u0002IN\u0003\u0002\u0002\u0002JH\u0003\u0002\u0002\u0002KM\u0005",
    "\u0012\n\u0002LK\u0003\u0002\u0002\u0002MP\u0003\u0002\u0002\u0002N",
    "L\u0003\u0002\u0002\u0002NO\u0003\u0002\u0002\u0002O\u0003\u0003\u0002",
    "\u0002\u0002PN\u0003\u0002\u0002\u0002QU\u0005\u0006\u0004\u0002RU\u0005",
    "\b\u0005\u0002SU\u0005\f\u0007\u0002TQ\u0003\u0002\u0002\u0002TR\u0003",
    "\u0002\u0002\u0002TS\u0003\u0002\u0002\u0002U\u0005\u0003\u0002\u0002",
    "\u0002VW\u0007\u001d\u0002\u0002WX\u0007>\u0002\u0002XY\u0007\u001e",
    "\u0002\u0002YZ\t\u0002\u0002\u0002Z\u0007\u0003\u0002\u0002\u0002[\\",
    "\u0007\u001f\u0002\u0002\\]\u0007?\u0002\u0002]^\u0005\n\u0006\u0002",
    "^_\u0007?\u0002\u0002_`\u0007 \u0002\u0002`\t\u0003\u0002\u0002\u0002",
    "ab\u0007!\u0002\u0002bc\u0005> \u0002cd\u0007>\u0002\u0002de\u00077",
    "\u0002\u0002ef\u0005.\u0018\u0002fg\u0007?\u0002\u0002gl\u0003\u0002",
    "\u0002\u0002hi\u0005\u0016\f\u0002ij\u0007?\u0002\u0002jl\u0003\u0002",
    "\u0002\u0002ka\u0003\u0002\u0002\u0002kh\u0003\u0002\u0002\u0002lo\u0003",
    "\u0002\u0002\u0002mk\u0003\u0002\u0002\u0002mn\u0003\u0002\u0002\u0002",
    "n\u000b\u0003\u0002\u0002\u0002om\u0003\u0002\u0002\u0002pr\u0007!\u0002",
    "\u0002qp\u0003\u0002\u0002\u0002qr\u0003\u0002\u0002\u0002rs\u0003\u0002",
    "\u0002\u0002st\u0007\"\u0002\u0002tu\u0005\u000e\b\u0002u\r\u0003\u0002",
    "\u0002\u0002vw\u0007>\u0002\u0002wz\u0007#\u0002\u0002x{\u0007$\u0002",
    "\u0002y{\u0005\u0010\t\u0002zx\u0003\u0002\u0002\u0002zy\u0003\u0002",
    "\u0002\u0002{|\u0003\u0002\u0002\u0002|\u007f\u0007\u0011\u0002\u0002",
    "}\u0080\u0005> \u0002~\u0080\u0007$\u0002\u0002\u007f}\u0003\u0002\u0002",
    "\u0002\u007f~\u0003\u0002\u0002\u0002\u0080\u000f\u0003\u0002\u0002",
    "\u0002\u0081\u0082\u0005> \u0002\u0082\u0089\u0007>\u0002\u0002\u0083",
    "\u0084\u00076\u0002\u0002\u0084\u0085\u0005> \u0002\u0085\u0086\u0007",
    ">\u0002\u0002\u0086\u0088\u0003\u0002\u0002\u0002\u0087\u0083\u0003",
    "\u0002\u0002\u0002\u0088\u008b\u0003\u0002\u0002\u0002\u0089\u0087\u0003",
    "\u0002\u0002\u0002\u0089\u008a\u0003\u0002\u0002\u0002\u008a\u0011\u0003",
    "\u0002\u0002\u0002\u008b\u0089\u0003\u0002\u0002\u0002\u008c\u008e\u0007",
    "!\u0002\u0002\u008d\u008c\u0003\u0002\u0002\u0002\u008d\u008e\u0003",
    "\u0002\u0002\u0002\u008e\u008f\u0003\u0002\u0002\u0002\u008f\u0090\u0007",
    "\u000f\u0002\u0002\u0090\u0091\u0005\u000e\b\u0002\u0091\u0092\u0007",
    "?\u0002\u0002\u0092\u0093\u0005\u0014\u000b\u0002\u0093\u0094\u0005",
    "\u0018\r\u0002\u0094\u0095\u0007%\u0002\u0002\u0095\u0096\u0007?\u0002",
    "\u0002\u0096\u0013\u0003\u0002\u0002\u0002\u0097\u0098\u0007&\u0002",
    "\u0002\u0098\u0099\u0005\u0016\f\u0002\u0099\u009a\u0007?\u0002\u0002",
    "\u009a\u009c\u0003\u0002\u0002\u0002\u009b\u0097\u0003\u0002\u0002\u0002",
    "\u009c\u009f\u0003\u0002\u0002\u0002\u009d\u009b\u0003\u0002\u0002\u0002",
    "\u009d\u009e\u0003\u0002\u0002\u0002\u009e\u0015\u0003\u0002\u0002\u0002",
    "\u009f\u009d\u0003\u0002\u0002\u0002\u00a0\u00a1\u0005> \u0002\u00a1",
    "\u00a4\u0007>\u0002\u0002\u00a2\u00a3\u00077\u0002\u0002\u00a3\u00a5",
    "\u0005.\u0018\u0002\u00a4\u00a2\u0003\u0002\u0002\u0002\u00a4\u00a5",
    "\u0003\u0002\u0002\u0002\u00a5\u00ab\u0003\u0002\u0002\u0002\u00a6\u00a7",
    "\u0005> \u0002\u00a7\u00a8\u0007\'\u0002\u0002\u00a8\u00a9\u0007>\u0002",
    "\u0002\u00a9\u00ab\u0003\u0002\u0002\u0002\u00aa\u00a0\u0003\u0002\u0002",
    "\u0002\u00aa\u00a6\u0003\u0002\u0002\u0002\u00ab\u0017\u0003\u0002\u0002",
    "\u0002\u00ac\u00ad\u0005\u001a\u000e\u0002\u00ad\u00ae\u0007?\u0002",
    "\u0002\u00ae\u00b0\u0003\u0002\u0002\u0002\u00af\u00ac\u0003\u0002\u0002",
    "\u0002\u00b0\u00b3\u0003\u0002\u0002\u0002\u00b1\u00af\u0003\u0002\u0002",
    "\u0002\u00b1\u00b2\u0003\u0002\u0002\u0002\u00b2\u0019\u0003\u0002\u0002",
    "\u0002\u00b3\u00b1\u0003\u0002\u0002\u0002\u00b4\u00bc\u0005\u001c\u000f",
    "\u0002\u00b5\u00bc\u0005\u001e\u0010\u0002\u00b6\u00bc\u0005\"\u0012",
    "\u0002\u00b7\u00bc\u0005&\u0014\u0002\u00b8\u00bc\u0005(\u0015\u0002",
    "\u00b9\u00bc\u0005*\u0016\u0002\u00ba\u00bc\u0005,\u0017\u0002\u00bb",
    "\u00b4\u0003\u0002\u0002\u0002\u00bb\u00b5\u0003\u0002\u0002\u0002\u00bb",
    "\u00b6\u0003\u0002\u0002\u0002\u00bb\u00b7\u0003\u0002\u0002\u0002\u00bb",
    "\u00b8\u0003\u0002\u0002\u0002\u00bb\u00b9\u0003\u0002\u0002\u0002\u00bb",
    "\u00ba\u0003\u0002\u0002\u0002\u00bc\u001b\u0003\u0002\u0002\u0002\u00bd",
    "\u00be\u0007\u001c\u0002\u0002\u00be\u00bf\u0007>\u0002\u0002\u00bf",
    "\u00c0\u00077\u0002\u0002\u00c0\u00ca\u0005.\u0018\u0002\u00c1\u00c2",
    "\u0007\u001c\u0002\u0002\u00c2\u00c3\u0007>\u0002\u0002\u00c3\u00c4",
    "\u00072\u0002\u0002\u00c4\u00c5\u0005.\u0018\u0002\u00c5\u00c6\u0007",
    "3\u0002\u0002\u00c6\u00c7\u00077\u0002\u0002\u00c7\u00c8\u0005.\u0018",
    "\u0002\u00c8\u00ca\u0003\u0002\u0002\u0002\u00c9\u00bd\u0003\u0002\u0002",
    "\u0002\u00c9\u00c1\u0003\u0002\u0002\u0002\u00ca\u001d\u0003\u0002\u0002",
    "\u0002\u00cb\u00cc\u0007\u0017\u0002\u0002\u00cc\u00cd\u0007>\u0002",
    "\u0002\u00cd\u00cf\u00074\u0002\u0002\u00ce\u00d0\u0005 \u0011\u0002",
    "\u00cf\u00ce\u0003\u0002\u0002\u0002\u00cf\u00d0\u0003\u0002\u0002\u0002",
    "\u00d0\u00d1\u0003\u0002\u0002\u0002\u00d1\u00d2\u00075\u0002\u0002",
    "\u00d2\u001f\u0003\u0002\u0002\u0002\u00d3\u00d8\u0005.\u0018\u0002",
    "\u00d4\u00d5\u00076\u0002\u0002\u00d5\u00d7\u0005.\u0018\u0002\u00d6",
    "\u00d4\u0003\u0002\u0002\u0002\u00d7\u00da\u0003\u0002\u0002\u0002\u00d8",
    "\u00d6\u0003\u0002\u0002\u0002\u00d8\u00d9\u0003\u0002\u0002\u0002\u00d9",
    "!\u0003\u0002\u0002\u0002\u00da\u00d8\u0003\u0002\u0002\u0002\u00db",
    "\u00dc\u0007\u0018\u0002\u0002\u00dc\u00dd\u0005.\u0018\u0002\u00dd",
    "\u00de\u0007\u0019\u0002\u0002\u00de\u00df\u0007?\u0002\u0002\u00df",
    "\u00e1\u0005\u0018\r\u0002\u00e0\u00e2\u0005$\u0013\u0002\u00e1\u00e0",
    "\u0003\u0002\u0002\u0002\u00e1\u00e2\u0003\u0002\u0002\u0002\u00e2\u00e3",
    "\u0003\u0002\u0002\u0002\u00e3\u00e4\u0007\u001a\u0002\u0002\u00e4#",
    "\u0003\u0002\u0002\u0002\u00e5\u00e6\u0007\u0016\u0002\u0002\u00e6\u00e7",
    "\u0007?\u0002\u0002\u00e7\u00f1\u0005\u0018\r\u0002\u00e8\u00e9\u0007",
    "\u0015\u0002\u0002\u00e9\u00ea\u0005.\u0018\u0002\u00ea\u00eb\u0007",
    "\u0019\u0002\u0002\u00eb\u00ec\u0007?\u0002\u0002\u00ec\u00ee\u0005",
    "\u0018\r\u0002\u00ed\u00ef\u0005$\u0013\u0002\u00ee\u00ed\u0003\u0002",
    "\u0002\u0002\u00ee\u00ef\u0003\u0002\u0002\u0002\u00ef\u00f1\u0003\u0002",
    "\u0002\u0002\u00f0\u00e5\u0003\u0002\u0002\u0002\u00f0\u00e8\u0003\u0002",
    "\u0002\u0002\u00f1%\u0003\u0002\u0002\u0002\u00f2\u00f3\u0007\u0013",
    "\u0002\u0002\u00f3\u00f4\u0007?\u0002\u0002\u00f4\u00f5\u0005\u0018",
    "\r\u0002\u00f5\u00f6\u0007\u0014\u0002\u0002\u00f6\'\u0003\u0002\u0002",
    "\u0002\u00f7\u00f8\u0007\u0012\u0002\u0002\u00f8\u00f9\u0005.\u0018",
    "\u0002\u00f9)\u0003\u0002\u0002\u0002\u00fa\u00fc\u0007\u0010\u0002",
    "\u0002\u00fb\u00fd\u0005.\u0018\u0002\u00fc\u00fb\u0003\u0002\u0002",
    "\u0002\u00fc\u00fd\u0003\u0002\u0002\u0002\u00fd+\u0003\u0002\u0002",
    "\u0002\u00fe\u0103\u0007\u001b\u0002\u0002\u00ff\u0104\u0005\u001c\u000f",
    "\u0002\u0100\u0104\u0005\u001e\u0010\u0002\u0101\u0104\u0005\"\u0012",
    "\u0002\u0102\u0104\u0005&\u0014\u0002\u0103\u00ff\u0003\u0002\u0002",
    "\u0002\u0103\u0100\u0003\u0002\u0002\u0002\u0103\u0101\u0003\u0002\u0002",
    "\u0002\u0103\u0102\u0003\u0002\u0002\u0002\u0104-\u0003\u0002\u0002",
    "\u0002\u0105\u0106\b\u0018\u0001\u0002\u0106\u0107\u0007(\u0002\u0002",
    "\u0107\u0113\u0005.\u0018\u0010\u0108\u0109\u0007)\u0002\u0002\u0109",
    "\u0113\u0005.\u0018\u000f\u010a\u010b\u0007\f\u0002\u0002\u010b\u0113",
    "\u0005.\u0018\u000e\u010c\u0113\u00050\u0019\u0002\u010d\u0113\u0005",
    "2\u001a\u0002\u010e\u0113\u00054\u001b\u0002\u010f\u0113\u0007>\u0002",
    "\u0002\u0110\u0113\u00056\u001c\u0002\u0111\u0113\u0005:\u001e\u0002",
    "\u0112\u0105\u0003\u0002\u0002\u0002\u0112\u0108\u0003\u0002\u0002\u0002",
    "\u0112\u010a\u0003\u0002\u0002\u0002\u0112\u010c\u0003\u0002\u0002\u0002",
    "\u0112\u010d\u0003\u0002\u0002\u0002\u0112\u010e\u0003\u0002\u0002\u0002",
    "\u0112\u010f\u0003\u0002\u0002\u0002\u0112\u0110\u0003\u0002\u0002\u0002",
    "\u0112\u0111\u0003\u0002\u0002\u0002\u0113\u0125\u0003\u0002\u0002\u0002",
    "\u0114\u0115\f\r\u0002\u0002\u0115\u0116\t\u0003\u0002\u0002\u0116\u0124",
    "\u0005.\u0018\u000e\u0117\u0118\f\f\u0002\u0002\u0118\u0119\t\u0004",
    "\u0002\u0002\u0119\u0124\u0005.\u0018\r\u011a\u011b\f\u000b\u0002\u0002",
    "\u011b\u011c\t\u0005\u0002\u0002\u011c\u0124\u0005.\u0018\f\u011d\u011e",
    "\f\n\u0002\u0002\u011e\u011f\t\u0006\u0002\u0002\u011f\u0124\u0005.",
    "\u0018\u000b\u0120\u0121\f\t\u0002\u0002\u0121\u0122\t\u0007\u0002\u0002",
    "\u0122\u0124\u0005.\u0018\n\u0123\u0114\u0003\u0002\u0002\u0002\u0123",
    "\u0117\u0003\u0002\u0002\u0002\u0123\u011a\u0003\u0002\u0002\u0002\u0123",
    "\u011d\u0003\u0002\u0002\u0002\u0123\u0120\u0003\u0002\u0002\u0002\u0124",
    "\u0127\u0003\u0002\u0002\u0002\u0125\u0123\u0003\u0002\u0002\u0002\u0125",
    "\u0126\u0003\u0002\u0002\u0002\u0126/\u0003\u0002\u0002\u0002\u0127",
    "\u0125\u0003\u0002\u0002\u0002\u0128\u0129\u0007>\u0002\u0002\u0129",
    "\u012b\u00074\u0002\u0002\u012a\u012c\u0005 \u0011\u0002\u012b\u012a",
    "\u0003\u0002\u0002\u0002\u012b\u012c\u0003\u0002\u0002\u0002\u012c\u012d",
    "\u0003\u0002\u0002\u0002\u012d\u012e\u00075\u0002\u0002\u012e1\u0003",
    "\u0002\u0002\u0002\u012f\u0130\u0007>\u0002\u0002\u0130\u0131\u0007",
    "2\u0002\u0002\u0131\u0132\u0005.\u0018\u0002\u0132\u0133\u00073\u0002",
    "\u0002\u01333\u0003\u0002\u0002\u0002\u0134\u0135\u0007\u000f\u0002",
    "\u0002\u0135\u0136\u0007>\u0002\u0002\u01365\u0003\u0002\u0002\u0002",
    "\u0137\u013d\u00058\u001d\u0002\u0138\u013d\u0007<\u0002\u0002\u0139",
    "\u013d\u0005<\u001f\u0002\u013a\u013d\u0007=\u0002\u0002\u013b\u013d",
    "\u0007\u000b\u0002\u0002\u013c\u0137\u0003\u0002\u0002\u0002\u013c\u0138",
    "\u0003\u0002\u0002\u0002\u013c\u0139\u0003\u0002\u0002\u0002\u013c\u013a",
    "\u0003\u0002\u0002\u0002\u013c\u013b\u0003\u0002\u0002\u0002\u013d7",
    "\u0003\u0002\u0002\u0002\u013e\u013f\t\b\u0002\u0002\u013f9\u0003\u0002",
    "\u0002\u0002\u0140\u0141\u00074\u0002\u0002\u0141\u0142\u0005.\u0018",
    "\u0002\u0142\u0143\u00075\u0002\u0002\u0143;\u0003\u0002\u0002\u0002",
    "\u0144\u0145\t\t\u0002\u0002\u0145=\u0003\u0002\u0002\u0002\u0146\u0147",
    "\t\n\u0002\u0002\u0147?\u0003\u0002\u0002\u0002\u001fAHNTkmqz\u007f",
    "\u0089\u008d\u009d\u00a4\u00aa\u00b1\u00bb\u00c9\u00cf\u00d8\u00e1\u00ee",
    "\u00f0\u00fc\u0103\u0112\u0123\u0125\u012b\u013c"].join("");


var atn = new antlr4.atn.ATNDeserializer().deserialize(serializedATN);

var decisionsToDFA = atn.decisionToState.map( function(ds, index) { return new antlr4.dfa.DFA(ds, index); });

var sharedContextCache = new antlr4.PredictionContextCache();

var literalNames = [ null, "'code'", "'handle'", "'integer'", "'real'", 
                     "'boolean'", "'string'", "'true'", "'false'", "'null'", 
                     "'not'", "'and'", "'or'", "'function'", "'return'", 
                     "'returns'", "'exitwhen'", "'loop'", "'endloop'", "'elseif'", 
                     "'else'", "'call'", "'if'", "'then'", "'endif'", "'debug'", 
                     "'set'", "'type'", "'extends'", "'globals'", "'endglobals'", 
                     "'constant'", "'native'", "'takes'", "'nothing'", "'endfunction'", 
                     "'local'", "'array'", "'+'", "'-'", "'*'", "'/'", "'>'", 
                     "'<'", "'=='", "'!='", "'>='", "'<='", "'['", "']'", 
                     "'('", "')'", "','", "'='" ];

var symbolicNames = [ null, "K_CODE", "K_HANDLE", "K_INTEGER", "K_REAL", 
                      "K_BOOLEAN", "K_STRING", "K_TRUE", "K_FALSE", "K_NULL", 
                      "K_NOT", "K_AND", "K_OR", "K_FUNCTION", "K_RETURN", 
                      "K_RETURNS", "K_EXITWHEN", "K_LOOP", "K_ENDLOOP", 
                      "K_ELSEIF", "K_ELSE", "K_CALL", "K_IF", "K_THEN", 
                      "K_ENDIF", "K_DEBUG", "K_SET", "K_TYPE", "K_EXTENDS", 
                      "K_GLOBALS", "K_ENDGLOBALS", "K_CONSTANT", "K_NATIVE", 
                      "K_TAKES", "K_NOTHING", "K_ENDFUNCTION", "K_LOCAL", 
                      "K_ARRAY", "ADD", "SUB", "MULT", "DIV", "GT", "LT", 
                      "EQ", "NEQ", "GTEQ", "LTEQ", "O_BRACK", "C_BRACK", 
                      "O_PAR", "C_PAR", "COMMA", "ASSIGN", "DECIMAL", "OCTAL", 
                      "HEX", "FOURCC", "REAL_CONST", "STRING_CONST", "ID", 
                      "NEWLINE", "SPACE", "COMMENT" ];

var ruleNames =  [ "file", "declr", "typedef", "globals", "global_var_list", 
                   "native_func", "func_declr", "param_list", "func", "local_var_list", 
                   "var_declr", "statement_list", "statement", "set", "call", 
                   "args", "ifthenelse", "else_clause", "loop", "exitwhen", 
                   "return_stat", "debug", "expr", "func_call", "array_ref", 
                   "func_ref", "constant", "int_const", "parens", "bool_const", 
                   "type" ];

function JassParser (input) {
	antlr4.Parser.call(this, input);
    this._interp = new antlr4.atn.ParserATNSimulator(this, atn, decisionsToDFA, sharedContextCache);
    this.ruleNames = ruleNames;
    this.literalNames = literalNames;
    this.symbolicNames = symbolicNames;
    return this;
}

JassParser.prototype = Object.create(antlr4.Parser.prototype);
JassParser.prototype.constructor = JassParser;

Object.defineProperty(JassParser.prototype, "atn", {
	get : function() {
		return atn;
	}
});

JassParser.EOF = antlr4.Token.EOF;
JassParser.K_CODE = 1;
JassParser.K_HANDLE = 2;
JassParser.K_INTEGER = 3;
JassParser.K_REAL = 4;
JassParser.K_BOOLEAN = 5;
JassParser.K_STRING = 6;
JassParser.K_TRUE = 7;
JassParser.K_FALSE = 8;
JassParser.K_NULL = 9;
JassParser.K_NOT = 10;
JassParser.K_AND = 11;
JassParser.K_OR = 12;
JassParser.K_FUNCTION = 13;
JassParser.K_RETURN = 14;
JassParser.K_RETURNS = 15;
JassParser.K_EXITWHEN = 16;
JassParser.K_LOOP = 17;
JassParser.K_ENDLOOP = 18;
JassParser.K_ELSEIF = 19;
JassParser.K_ELSE = 20;
JassParser.K_CALL = 21;
JassParser.K_IF = 22;
JassParser.K_THEN = 23;
JassParser.K_ENDIF = 24;
JassParser.K_DEBUG = 25;
JassParser.K_SET = 26;
JassParser.K_TYPE = 27;
JassParser.K_EXTENDS = 28;
JassParser.K_GLOBALS = 29;
JassParser.K_ENDGLOBALS = 30;
JassParser.K_CONSTANT = 31;
JassParser.K_NATIVE = 32;
JassParser.K_TAKES = 33;
JassParser.K_NOTHING = 34;
JassParser.K_ENDFUNCTION = 35;
JassParser.K_LOCAL = 36;
JassParser.K_ARRAY = 37;
JassParser.ADD = 38;
JassParser.SUB = 39;
JassParser.MULT = 40;
JassParser.DIV = 41;
JassParser.GT = 42;
JassParser.LT = 43;
JassParser.EQ = 44;
JassParser.NEQ = 45;
JassParser.GTEQ = 46;
JassParser.LTEQ = 47;
JassParser.O_BRACK = 48;
JassParser.C_BRACK = 49;
JassParser.O_PAR = 50;
JassParser.C_PAR = 51;
JassParser.COMMA = 52;
JassParser.ASSIGN = 53;
JassParser.DECIMAL = 54;
JassParser.OCTAL = 55;
JassParser.HEX = 56;
JassParser.FOURCC = 57;
JassParser.REAL_CONST = 58;
JassParser.STRING_CONST = 59;
JassParser.ID = 60;
JassParser.NEWLINE = 61;
JassParser.SPACE = 62;
JassParser.COMMENT = 63;

JassParser.RULE_file = 0;
JassParser.RULE_declr = 1;
JassParser.RULE_typedef = 2;
JassParser.RULE_globals = 3;
JassParser.RULE_global_var_list = 4;
JassParser.RULE_native_func = 5;
JassParser.RULE_func_declr = 6;
JassParser.RULE_param_list = 7;
JassParser.RULE_func = 8;
JassParser.RULE_local_var_list = 9;
JassParser.RULE_var_declr = 10;
JassParser.RULE_statement_list = 11;
JassParser.RULE_statement = 12;
JassParser.RULE_set = 13;
JassParser.RULE_call = 14;
JassParser.RULE_args = 15;
JassParser.RULE_ifthenelse = 16;
JassParser.RULE_else_clause = 17;
JassParser.RULE_loop = 18;
JassParser.RULE_exitwhen = 19;
JassParser.RULE_return_stat = 20;
JassParser.RULE_debug = 21;
JassParser.RULE_expr = 22;
JassParser.RULE_func_call = 23;
JassParser.RULE_array_ref = 24;
JassParser.RULE_func_ref = 25;
JassParser.RULE_constant = 26;
JassParser.RULE_int_const = 27;
JassParser.RULE_parens = 28;
JassParser.RULE_bool_const = 29;
JassParser.RULE_type = 30;


function FileContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_file;
    return this;
}

FileContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
FileContext.prototype.constructor = FileContext;

FileContext.prototype.NEWLINE = function(i) {
	if(i===undefined) {
		i = null;
	}
    if(i===null) {
        return this.getTokens(JassParser.NEWLINE);
    } else {
        return this.getToken(JassParser.NEWLINE, i);
    }
};


FileContext.prototype.declr = function(i) {
    if(i===undefined) {
        i = null;
    }
    if(i===null) {
        return this.getTypedRuleContexts(DeclrContext);
    } else {
        return this.getTypedRuleContext(DeclrContext,i);
    }
};

FileContext.prototype.func = function(i) {
    if(i===undefined) {
        i = null;
    }
    if(i===null) {
        return this.getTypedRuleContexts(FuncContext);
    } else {
        return this.getTypedRuleContext(FuncContext,i);
    }
};

FileContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterFile(this);
	}
};

FileContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitFile(this);
	}
};




JassParser.FileContext = FileContext;

JassParser.prototype.file = function() {

    var localctx = new FileContext(this, this._ctx, this.state);
    this.enterRule(localctx, 0, JassParser.RULE_file);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 63;
        this._errHandler.sync(this);
        _la = this._input.LA(1);
        if(_la===JassParser.NEWLINE) {
            this.state = 62;
            this.match(JassParser.NEWLINE);
        }

        this.state = 70;
        this._errHandler.sync(this);
        var _alt = this._interp.adaptivePredict(this._input,1,this._ctx)
        while(_alt!=2 && _alt!=antlr4.atn.ATN.INVALID_ALT_NUMBER) {
            if(_alt===1) {
                this.state = 65;
                this.declr();
                this.state = 66;
                this.match(JassParser.NEWLINE); 
            }
            this.state = 72;
            this._errHandler.sync(this);
            _alt = this._interp.adaptivePredict(this._input,1,this._ctx);
        }

        this.state = 76;
        this._errHandler.sync(this);
        _la = this._input.LA(1);
        while(_la===JassParser.K_FUNCTION || _la===JassParser.K_CONSTANT) {
            this.state = 73;
            this.func();
            this.state = 78;
            this._errHandler.sync(this);
            _la = this._input.LA(1);
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function DeclrContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_declr;
    return this;
}

DeclrContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
DeclrContext.prototype.constructor = DeclrContext;

DeclrContext.prototype.typedef = function() {
    return this.getTypedRuleContext(TypedefContext,0);
};

DeclrContext.prototype.globals = function() {
    return this.getTypedRuleContext(GlobalsContext,0);
};

DeclrContext.prototype.native_func = function() {
    return this.getTypedRuleContext(Native_funcContext,0);
};

DeclrContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterDeclr(this);
	}
};

DeclrContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitDeclr(this);
	}
};




JassParser.DeclrContext = DeclrContext;

JassParser.prototype.declr = function() {

    var localctx = new DeclrContext(this, this._ctx, this.state);
    this.enterRule(localctx, 2, JassParser.RULE_declr);
    try {
        this.state = 82;
        this._errHandler.sync(this);
        switch(this._input.LA(1)) {
        case JassParser.K_TYPE:
            this.enterOuterAlt(localctx, 1);
            this.state = 79;
            this.typedef();
            break;
        case JassParser.K_GLOBALS:
            this.enterOuterAlt(localctx, 2);
            this.state = 80;
            this.globals();
            break;
        case JassParser.K_CONSTANT:
        case JassParser.K_NATIVE:
            this.enterOuterAlt(localctx, 3);
            this.state = 81;
            this.native_func();
            break;
        default:
            throw new antlr4.error.NoViableAltException(this);
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function TypedefContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_typedef;
    return this;
}

TypedefContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
TypedefContext.prototype.constructor = TypedefContext;

TypedefContext.prototype.K_TYPE = function() {
    return this.getToken(JassParser.K_TYPE, 0);
};

TypedefContext.prototype.ID = function(i) {
	if(i===undefined) {
		i = null;
	}
    if(i===null) {
        return this.getTokens(JassParser.ID);
    } else {
        return this.getToken(JassParser.ID, i);
    }
};


TypedefContext.prototype.K_EXTENDS = function() {
    return this.getToken(JassParser.K_EXTENDS, 0);
};

TypedefContext.prototype.K_HANDLE = function() {
    return this.getToken(JassParser.K_HANDLE, 0);
};

TypedefContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterTypedef(this);
	}
};

TypedefContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitTypedef(this);
	}
};




JassParser.TypedefContext = TypedefContext;

JassParser.prototype.typedef = function() {

    var localctx = new TypedefContext(this, this._ctx, this.state);
    this.enterRule(localctx, 4, JassParser.RULE_typedef);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 84;
        this.match(JassParser.K_TYPE);
        this.state = 85;
        this.match(JassParser.ID);
        this.state = 86;
        this.match(JassParser.K_EXTENDS);
        this.state = 87;
        _la = this._input.LA(1);
        if(!(_la===JassParser.K_HANDLE || _la===JassParser.ID)) {
        this._errHandler.recoverInline(this);
        }
        else {
        	this._errHandler.reportMatch(this);
            this.consume();
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function GlobalsContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_globals;
    return this;
}

GlobalsContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
GlobalsContext.prototype.constructor = GlobalsContext;

GlobalsContext.prototype.K_GLOBALS = function() {
    return this.getToken(JassParser.K_GLOBALS, 0);
};

GlobalsContext.prototype.NEWLINE = function(i) {
	if(i===undefined) {
		i = null;
	}
    if(i===null) {
        return this.getTokens(JassParser.NEWLINE);
    } else {
        return this.getToken(JassParser.NEWLINE, i);
    }
};


GlobalsContext.prototype.global_var_list = function() {
    return this.getTypedRuleContext(Global_var_listContext,0);
};

GlobalsContext.prototype.K_ENDGLOBALS = function() {
    return this.getToken(JassParser.K_ENDGLOBALS, 0);
};

GlobalsContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterGlobals(this);
	}
};

GlobalsContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitGlobals(this);
	}
};




JassParser.GlobalsContext = GlobalsContext;

JassParser.prototype.globals = function() {

    var localctx = new GlobalsContext(this, this._ctx, this.state);
    this.enterRule(localctx, 6, JassParser.RULE_globals);
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 89;
        this.match(JassParser.K_GLOBALS);
        this.state = 90;
        this.match(JassParser.NEWLINE);
        this.state = 91;
        this.global_var_list();
        this.state = 92;
        this.match(JassParser.NEWLINE);
        this.state = 93;
        this.match(JassParser.K_ENDGLOBALS);
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function Global_var_listContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_global_var_list;
    return this;
}

Global_var_listContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Global_var_listContext.prototype.constructor = Global_var_listContext;

Global_var_listContext.prototype.K_CONSTANT = function(i) {
	if(i===undefined) {
		i = null;
	}
    if(i===null) {
        return this.getTokens(JassParser.K_CONSTANT);
    } else {
        return this.getToken(JassParser.K_CONSTANT, i);
    }
};


Global_var_listContext.prototype.type = function(i) {
    if(i===undefined) {
        i = null;
    }
    if(i===null) {
        return this.getTypedRuleContexts(TypeContext);
    } else {
        return this.getTypedRuleContext(TypeContext,i);
    }
};

Global_var_listContext.prototype.ID = function(i) {
	if(i===undefined) {
		i = null;
	}
    if(i===null) {
        return this.getTokens(JassParser.ID);
    } else {
        return this.getToken(JassParser.ID, i);
    }
};


Global_var_listContext.prototype.ASSIGN = function(i) {
	if(i===undefined) {
		i = null;
	}
    if(i===null) {
        return this.getTokens(JassParser.ASSIGN);
    } else {
        return this.getToken(JassParser.ASSIGN, i);
    }
};


Global_var_listContext.prototype.expr = function(i) {
    if(i===undefined) {
        i = null;
    }
    if(i===null) {
        return this.getTypedRuleContexts(ExprContext);
    } else {
        return this.getTypedRuleContext(ExprContext,i);
    }
};

Global_var_listContext.prototype.NEWLINE = function(i) {
	if(i===undefined) {
		i = null;
	}
    if(i===null) {
        return this.getTokens(JassParser.NEWLINE);
    } else {
        return this.getToken(JassParser.NEWLINE, i);
    }
};


Global_var_listContext.prototype.var_declr = function(i) {
    if(i===undefined) {
        i = null;
    }
    if(i===null) {
        return this.getTypedRuleContexts(Var_declrContext);
    } else {
        return this.getTypedRuleContext(Var_declrContext,i);
    }
};

Global_var_listContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterGlobal_var_list(this);
	}
};

Global_var_listContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitGlobal_var_list(this);
	}
};




JassParser.Global_var_listContext = Global_var_listContext;

JassParser.prototype.global_var_list = function() {

    var localctx = new Global_var_listContext(this, this._ctx, this.state);
    this.enterRule(localctx, 8, JassParser.RULE_global_var_list);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 107;
        this._errHandler.sync(this);
        _la = this._input.LA(1);
        while((((_la) & ~0x1f) == 0 && ((1 << _la) & ((1 << JassParser.K_CODE) | (1 << JassParser.K_HANDLE) | (1 << JassParser.K_INTEGER) | (1 << JassParser.K_REAL) | (1 << JassParser.K_BOOLEAN) | (1 << JassParser.K_STRING) | (1 << JassParser.K_CONSTANT))) !== 0) || _la===JassParser.ID) {
            this.state = 105;
            this._errHandler.sync(this);
            switch(this._input.LA(1)) {
            case JassParser.K_CONSTANT:
                this.state = 95;
                this.match(JassParser.K_CONSTANT);
                this.state = 96;
                this.type();
                this.state = 97;
                this.match(JassParser.ID);
                this.state = 98;
                this.match(JassParser.ASSIGN);
                this.state = 99;
                this.expr(0);
                this.state = 100;
                this.match(JassParser.NEWLINE);
                break;
            case JassParser.K_CODE:
            case JassParser.K_HANDLE:
            case JassParser.K_INTEGER:
            case JassParser.K_REAL:
            case JassParser.K_BOOLEAN:
            case JassParser.K_STRING:
            case JassParser.ID:
                this.state = 102;
                this.var_declr();
                this.state = 103;
                this.match(JassParser.NEWLINE);
                break;
            default:
                throw new antlr4.error.NoViableAltException(this);
            }
            this.state = 109;
            this._errHandler.sync(this);
            _la = this._input.LA(1);
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function Native_funcContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_native_func;
    return this;
}

Native_funcContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Native_funcContext.prototype.constructor = Native_funcContext;

Native_funcContext.prototype.K_NATIVE = function() {
    return this.getToken(JassParser.K_NATIVE, 0);
};

Native_funcContext.prototype.func_declr = function() {
    return this.getTypedRuleContext(Func_declrContext,0);
};

Native_funcContext.prototype.K_CONSTANT = function() {
    return this.getToken(JassParser.K_CONSTANT, 0);
};

Native_funcContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterNative_func(this);
	}
};

Native_funcContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitNative_func(this);
	}
};




JassParser.Native_funcContext = Native_funcContext;

JassParser.prototype.native_func = function() {

    var localctx = new Native_funcContext(this, this._ctx, this.state);
    this.enterRule(localctx, 10, JassParser.RULE_native_func);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 111;
        this._errHandler.sync(this);
        _la = this._input.LA(1);
        if(_la===JassParser.K_CONSTANT) {
            this.state = 110;
            this.match(JassParser.K_CONSTANT);
        }

        this.state = 113;
        this.match(JassParser.K_NATIVE);
        this.state = 114;
        this.func_declr();
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function Func_declrContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_func_declr;
    return this;
}

Func_declrContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Func_declrContext.prototype.constructor = Func_declrContext;

Func_declrContext.prototype.ID = function() {
    return this.getToken(JassParser.ID, 0);
};

Func_declrContext.prototype.K_TAKES = function() {
    return this.getToken(JassParser.K_TAKES, 0);
};

Func_declrContext.prototype.K_RETURNS = function() {
    return this.getToken(JassParser.K_RETURNS, 0);
};

Func_declrContext.prototype.K_NOTHING = function(i) {
	if(i===undefined) {
		i = null;
	}
    if(i===null) {
        return this.getTokens(JassParser.K_NOTHING);
    } else {
        return this.getToken(JassParser.K_NOTHING, i);
    }
};


Func_declrContext.prototype.param_list = function() {
    return this.getTypedRuleContext(Param_listContext,0);
};

Func_declrContext.prototype.type = function() {
    return this.getTypedRuleContext(TypeContext,0);
};

Func_declrContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterFunc_declr(this);
	}
};

Func_declrContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitFunc_declr(this);
	}
};




JassParser.Func_declrContext = Func_declrContext;

JassParser.prototype.func_declr = function() {

    var localctx = new Func_declrContext(this, this._ctx, this.state);
    this.enterRule(localctx, 12, JassParser.RULE_func_declr);
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 116;
        this.match(JassParser.ID);
        this.state = 117;
        this.match(JassParser.K_TAKES);
        this.state = 120;
        this._errHandler.sync(this);
        switch(this._input.LA(1)) {
        case JassParser.K_NOTHING:
            this.state = 118;
            this.match(JassParser.K_NOTHING);
            break;
        case JassParser.K_CODE:
        case JassParser.K_HANDLE:
        case JassParser.K_INTEGER:
        case JassParser.K_REAL:
        case JassParser.K_BOOLEAN:
        case JassParser.K_STRING:
        case JassParser.ID:
            this.state = 119;
            this.param_list();
            break;
        default:
            throw new antlr4.error.NoViableAltException(this);
        }
        this.state = 122;
        this.match(JassParser.K_RETURNS);
        this.state = 125;
        this._errHandler.sync(this);
        switch(this._input.LA(1)) {
        case JassParser.K_CODE:
        case JassParser.K_HANDLE:
        case JassParser.K_INTEGER:
        case JassParser.K_REAL:
        case JassParser.K_BOOLEAN:
        case JassParser.K_STRING:
        case JassParser.ID:
            this.state = 123;
            this.type();
            break;
        case JassParser.K_NOTHING:
            this.state = 124;
            this.match(JassParser.K_NOTHING);
            break;
        default:
            throw new antlr4.error.NoViableAltException(this);
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function Param_listContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_param_list;
    return this;
}

Param_listContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Param_listContext.prototype.constructor = Param_listContext;

Param_listContext.prototype.type = function(i) {
    if(i===undefined) {
        i = null;
    }
    if(i===null) {
        return this.getTypedRuleContexts(TypeContext);
    } else {
        return this.getTypedRuleContext(TypeContext,i);
    }
};

Param_listContext.prototype.ID = function(i) {
	if(i===undefined) {
		i = null;
	}
    if(i===null) {
        return this.getTokens(JassParser.ID);
    } else {
        return this.getToken(JassParser.ID, i);
    }
};


Param_listContext.prototype.COMMA = function(i) {
	if(i===undefined) {
		i = null;
	}
    if(i===null) {
        return this.getTokens(JassParser.COMMA);
    } else {
        return this.getToken(JassParser.COMMA, i);
    }
};


Param_listContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterParam_list(this);
	}
};

Param_listContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitParam_list(this);
	}
};




JassParser.Param_listContext = Param_listContext;

JassParser.prototype.param_list = function() {

    var localctx = new Param_listContext(this, this._ctx, this.state);
    this.enterRule(localctx, 14, JassParser.RULE_param_list);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 127;
        this.type();
        this.state = 128;
        this.match(JassParser.ID);
        this.state = 135;
        this._errHandler.sync(this);
        _la = this._input.LA(1);
        while(_la===JassParser.COMMA) {
            this.state = 129;
            this.match(JassParser.COMMA);
            this.state = 130;
            this.type();
            this.state = 131;
            this.match(JassParser.ID);
            this.state = 137;
            this._errHandler.sync(this);
            _la = this._input.LA(1);
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function FuncContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_func;
    return this;
}

FuncContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
FuncContext.prototype.constructor = FuncContext;

FuncContext.prototype.K_FUNCTION = function() {
    return this.getToken(JassParser.K_FUNCTION, 0);
};

FuncContext.prototype.func_declr = function() {
    return this.getTypedRuleContext(Func_declrContext,0);
};

FuncContext.prototype.NEWLINE = function(i) {
	if(i===undefined) {
		i = null;
	}
    if(i===null) {
        return this.getTokens(JassParser.NEWLINE);
    } else {
        return this.getToken(JassParser.NEWLINE, i);
    }
};


FuncContext.prototype.local_var_list = function() {
    return this.getTypedRuleContext(Local_var_listContext,0);
};

FuncContext.prototype.statement_list = function() {
    return this.getTypedRuleContext(Statement_listContext,0);
};

FuncContext.prototype.K_ENDFUNCTION = function() {
    return this.getToken(JassParser.K_ENDFUNCTION, 0);
};

FuncContext.prototype.K_CONSTANT = function() {
    return this.getToken(JassParser.K_CONSTANT, 0);
};

FuncContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterFunc(this);
	}
};

FuncContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitFunc(this);
	}
};




JassParser.FuncContext = FuncContext;

JassParser.prototype.func = function() {

    var localctx = new FuncContext(this, this._ctx, this.state);
    this.enterRule(localctx, 16, JassParser.RULE_func);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 139;
        this._errHandler.sync(this);
        _la = this._input.LA(1);
        if(_la===JassParser.K_CONSTANT) {
            this.state = 138;
            this.match(JassParser.K_CONSTANT);
        }

        this.state = 141;
        this.match(JassParser.K_FUNCTION);
        this.state = 142;
        this.func_declr();
        this.state = 143;
        this.match(JassParser.NEWLINE);
        this.state = 144;
        this.local_var_list();
        this.state = 145;
        this.statement_list();
        this.state = 146;
        this.match(JassParser.K_ENDFUNCTION);
        this.state = 147;
        this.match(JassParser.NEWLINE);
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function Local_var_listContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_local_var_list;
    return this;
}

Local_var_listContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Local_var_listContext.prototype.constructor = Local_var_listContext;

Local_var_listContext.prototype.K_LOCAL = function(i) {
	if(i===undefined) {
		i = null;
	}
    if(i===null) {
        return this.getTokens(JassParser.K_LOCAL);
    } else {
        return this.getToken(JassParser.K_LOCAL, i);
    }
};


Local_var_listContext.prototype.var_declr = function(i) {
    if(i===undefined) {
        i = null;
    }
    if(i===null) {
        return this.getTypedRuleContexts(Var_declrContext);
    } else {
        return this.getTypedRuleContext(Var_declrContext,i);
    }
};

Local_var_listContext.prototype.NEWLINE = function(i) {
	if(i===undefined) {
		i = null;
	}
    if(i===null) {
        return this.getTokens(JassParser.NEWLINE);
    } else {
        return this.getToken(JassParser.NEWLINE, i);
    }
};


Local_var_listContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterLocal_var_list(this);
	}
};

Local_var_listContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitLocal_var_list(this);
	}
};




JassParser.Local_var_listContext = Local_var_listContext;

JassParser.prototype.local_var_list = function() {

    var localctx = new Local_var_listContext(this, this._ctx, this.state);
    this.enterRule(localctx, 18, JassParser.RULE_local_var_list);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 155;
        this._errHandler.sync(this);
        _la = this._input.LA(1);
        while(_la===JassParser.K_LOCAL) {
            this.state = 149;
            this.match(JassParser.K_LOCAL);
            this.state = 150;
            this.var_declr();
            this.state = 151;
            this.match(JassParser.NEWLINE);
            this.state = 157;
            this._errHandler.sync(this);
            _la = this._input.LA(1);
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function Var_declrContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_var_declr;
    return this;
}

Var_declrContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Var_declrContext.prototype.constructor = Var_declrContext;

Var_declrContext.prototype.type = function() {
    return this.getTypedRuleContext(TypeContext,0);
};

Var_declrContext.prototype.ID = function() {
    return this.getToken(JassParser.ID, 0);
};

Var_declrContext.prototype.ASSIGN = function() {
    return this.getToken(JassParser.ASSIGN, 0);
};

Var_declrContext.prototype.expr = function() {
    return this.getTypedRuleContext(ExprContext,0);
};

Var_declrContext.prototype.K_ARRAY = function() {
    return this.getToken(JassParser.K_ARRAY, 0);
};

Var_declrContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterVar_declr(this);
	}
};

Var_declrContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitVar_declr(this);
	}
};




JassParser.Var_declrContext = Var_declrContext;

JassParser.prototype.var_declr = function() {

    var localctx = new Var_declrContext(this, this._ctx, this.state);
    this.enterRule(localctx, 20, JassParser.RULE_var_declr);
    var _la = 0; // Token type
    try {
        this.state = 168;
        this._errHandler.sync(this);
        var la_ = this._interp.adaptivePredict(this._input,13,this._ctx);
        switch(la_) {
        case 1:
            this.enterOuterAlt(localctx, 1);
            this.state = 158;
            this.type();
            this.state = 159;
            this.match(JassParser.ID);
            this.state = 162;
            this._errHandler.sync(this);
            _la = this._input.LA(1);
            if(_la===JassParser.ASSIGN) {
                this.state = 160;
                this.match(JassParser.ASSIGN);
                this.state = 161;
                this.expr(0);
            }

            break;

        case 2:
            this.enterOuterAlt(localctx, 2);
            this.state = 164;
            this.type();
            this.state = 165;
            this.match(JassParser.K_ARRAY);
            this.state = 166;
            this.match(JassParser.ID);
            break;

        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function Statement_listContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_statement_list;
    return this;
}

Statement_listContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Statement_listContext.prototype.constructor = Statement_listContext;

Statement_listContext.prototype.statement = function(i) {
    if(i===undefined) {
        i = null;
    }
    if(i===null) {
        return this.getTypedRuleContexts(StatementContext);
    } else {
        return this.getTypedRuleContext(StatementContext,i);
    }
};

Statement_listContext.prototype.NEWLINE = function(i) {
	if(i===undefined) {
		i = null;
	}
    if(i===null) {
        return this.getTokens(JassParser.NEWLINE);
    } else {
        return this.getToken(JassParser.NEWLINE, i);
    }
};


Statement_listContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterStatement_list(this);
	}
};

Statement_listContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitStatement_list(this);
	}
};




JassParser.Statement_listContext = Statement_listContext;

JassParser.prototype.statement_list = function() {

    var localctx = new Statement_listContext(this, this._ctx, this.state);
    this.enterRule(localctx, 22, JassParser.RULE_statement_list);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 175;
        this._errHandler.sync(this);
        _la = this._input.LA(1);
        while((((_la) & ~0x1f) == 0 && ((1 << _la) & ((1 << JassParser.K_RETURN) | (1 << JassParser.K_EXITWHEN) | (1 << JassParser.K_LOOP) | (1 << JassParser.K_CALL) | (1 << JassParser.K_IF) | (1 << JassParser.K_DEBUG) | (1 << JassParser.K_SET))) !== 0)) {
            this.state = 170;
            this.statement();
            this.state = 171;
            this.match(JassParser.NEWLINE);
            this.state = 177;
            this._errHandler.sync(this);
            _la = this._input.LA(1);
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function StatementContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_statement;
    return this;
}

StatementContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
StatementContext.prototype.constructor = StatementContext;

StatementContext.prototype.set = function() {
    return this.getTypedRuleContext(SetContext,0);
};

StatementContext.prototype.call = function() {
    return this.getTypedRuleContext(CallContext,0);
};

StatementContext.prototype.ifthenelse = function() {
    return this.getTypedRuleContext(IfthenelseContext,0);
};

StatementContext.prototype.loop = function() {
    return this.getTypedRuleContext(LoopContext,0);
};

StatementContext.prototype.exitwhen = function() {
    return this.getTypedRuleContext(ExitwhenContext,0);
};

StatementContext.prototype.return_stat = function() {
    return this.getTypedRuleContext(Return_statContext,0);
};

StatementContext.prototype.debug = function() {
    return this.getTypedRuleContext(DebugContext,0);
};

StatementContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterStatement(this);
	}
};

StatementContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitStatement(this);
	}
};




JassParser.StatementContext = StatementContext;

JassParser.prototype.statement = function() {

    var localctx = new StatementContext(this, this._ctx, this.state);
    this.enterRule(localctx, 24, JassParser.RULE_statement);
    try {
        this.state = 185;
        this._errHandler.sync(this);
        switch(this._input.LA(1)) {
        case JassParser.K_SET:
            this.enterOuterAlt(localctx, 1);
            this.state = 178;
            this.set();
            break;
        case JassParser.K_CALL:
            this.enterOuterAlt(localctx, 2);
            this.state = 179;
            this.call();
            break;
        case JassParser.K_IF:
            this.enterOuterAlt(localctx, 3);
            this.state = 180;
            this.ifthenelse();
            break;
        case JassParser.K_LOOP:
            this.enterOuterAlt(localctx, 4);
            this.state = 181;
            this.loop();
            break;
        case JassParser.K_EXITWHEN:
            this.enterOuterAlt(localctx, 5);
            this.state = 182;
            this.exitwhen();
            break;
        case JassParser.K_RETURN:
            this.enterOuterAlt(localctx, 6);
            this.state = 183;
            this.return_stat();
            break;
        case JassParser.K_DEBUG:
            this.enterOuterAlt(localctx, 7);
            this.state = 184;
            this.debug();
            break;
        default:
            throw new antlr4.error.NoViableAltException(this);
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function SetContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_set;
    return this;
}

SetContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
SetContext.prototype.constructor = SetContext;

SetContext.prototype.K_SET = function() {
    return this.getToken(JassParser.K_SET, 0);
};

SetContext.prototype.ID = function() {
    return this.getToken(JassParser.ID, 0);
};

SetContext.prototype.ASSIGN = function() {
    return this.getToken(JassParser.ASSIGN, 0);
};

SetContext.prototype.expr = function(i) {
    if(i===undefined) {
        i = null;
    }
    if(i===null) {
        return this.getTypedRuleContexts(ExprContext);
    } else {
        return this.getTypedRuleContext(ExprContext,i);
    }
};

SetContext.prototype.O_BRACK = function() {
    return this.getToken(JassParser.O_BRACK, 0);
};

SetContext.prototype.C_BRACK = function() {
    return this.getToken(JassParser.C_BRACK, 0);
};

SetContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterSet(this);
	}
};

SetContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitSet(this);
	}
};




JassParser.SetContext = SetContext;

JassParser.prototype.set = function() {

    var localctx = new SetContext(this, this._ctx, this.state);
    this.enterRule(localctx, 26, JassParser.RULE_set);
    try {
        this.state = 199;
        this._errHandler.sync(this);
        var la_ = this._interp.adaptivePredict(this._input,16,this._ctx);
        switch(la_) {
        case 1:
            this.enterOuterAlt(localctx, 1);
            this.state = 187;
            this.match(JassParser.K_SET);
            this.state = 188;
            this.match(JassParser.ID);
            this.state = 189;
            this.match(JassParser.ASSIGN);
            this.state = 190;
            this.expr(0);
            break;

        case 2:
            this.enterOuterAlt(localctx, 2);
            this.state = 191;
            this.match(JassParser.K_SET);
            this.state = 192;
            this.match(JassParser.ID);
            this.state = 193;
            this.match(JassParser.O_BRACK);
            this.state = 194;
            this.expr(0);
            this.state = 195;
            this.match(JassParser.C_BRACK);
            this.state = 196;
            this.match(JassParser.ASSIGN);
            this.state = 197;
            this.expr(0);
            break;

        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function CallContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_call;
    return this;
}

CallContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
CallContext.prototype.constructor = CallContext;

CallContext.prototype.K_CALL = function() {
    return this.getToken(JassParser.K_CALL, 0);
};

CallContext.prototype.ID = function() {
    return this.getToken(JassParser.ID, 0);
};

CallContext.prototype.O_PAR = function() {
    return this.getToken(JassParser.O_PAR, 0);
};

CallContext.prototype.C_PAR = function() {
    return this.getToken(JassParser.C_PAR, 0);
};

CallContext.prototype.args = function() {
    return this.getTypedRuleContext(ArgsContext,0);
};

CallContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterCall(this);
	}
};

CallContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitCall(this);
	}
};




JassParser.CallContext = CallContext;

JassParser.prototype.call = function() {

    var localctx = new CallContext(this, this._ctx, this.state);
    this.enterRule(localctx, 28, JassParser.RULE_call);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 201;
        this.match(JassParser.K_CALL);
        this.state = 202;
        this.match(JassParser.ID);
        this.state = 203;
        this.match(JassParser.O_PAR);
        this.state = 205;
        this._errHandler.sync(this);
        _la = this._input.LA(1);
        if((((_la) & ~0x1f) == 0 && ((1 << _la) & ((1 << JassParser.K_TRUE) | (1 << JassParser.K_FALSE) | (1 << JassParser.K_NULL) | (1 << JassParser.K_NOT) | (1 << JassParser.K_FUNCTION))) !== 0) || ((((_la - 38)) & ~0x1f) == 0 && ((1 << (_la - 38)) & ((1 << (JassParser.ADD - 38)) | (1 << (JassParser.SUB - 38)) | (1 << (JassParser.O_PAR - 38)) | (1 << (JassParser.DECIMAL - 38)) | (1 << (JassParser.OCTAL - 38)) | (1 << (JassParser.HEX - 38)) | (1 << (JassParser.FOURCC - 38)) | (1 << (JassParser.REAL_CONST - 38)) | (1 << (JassParser.STRING_CONST - 38)) | (1 << (JassParser.ID - 38)))) !== 0)) {
            this.state = 204;
            this.args();
        }

        this.state = 207;
        this.match(JassParser.C_PAR);
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function ArgsContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_args;
    return this;
}

ArgsContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
ArgsContext.prototype.constructor = ArgsContext;

ArgsContext.prototype.expr = function(i) {
    if(i===undefined) {
        i = null;
    }
    if(i===null) {
        return this.getTypedRuleContexts(ExprContext);
    } else {
        return this.getTypedRuleContext(ExprContext,i);
    }
};

ArgsContext.prototype.COMMA = function(i) {
	if(i===undefined) {
		i = null;
	}
    if(i===null) {
        return this.getTokens(JassParser.COMMA);
    } else {
        return this.getToken(JassParser.COMMA, i);
    }
};


ArgsContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterArgs(this);
	}
};

ArgsContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitArgs(this);
	}
};




JassParser.ArgsContext = ArgsContext;

JassParser.prototype.args = function() {

    var localctx = new ArgsContext(this, this._ctx, this.state);
    this.enterRule(localctx, 30, JassParser.RULE_args);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 209;
        this.expr(0);
        this.state = 214;
        this._errHandler.sync(this);
        _la = this._input.LA(1);
        while(_la===JassParser.COMMA) {
            this.state = 210;
            this.match(JassParser.COMMA);
            this.state = 211;
            this.expr(0);
            this.state = 216;
            this._errHandler.sync(this);
            _la = this._input.LA(1);
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function IfthenelseContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_ifthenelse;
    return this;
}

IfthenelseContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
IfthenelseContext.prototype.constructor = IfthenelseContext;

IfthenelseContext.prototype.K_IF = function() {
    return this.getToken(JassParser.K_IF, 0);
};

IfthenelseContext.prototype.expr = function() {
    return this.getTypedRuleContext(ExprContext,0);
};

IfthenelseContext.prototype.K_THEN = function() {
    return this.getToken(JassParser.K_THEN, 0);
};

IfthenelseContext.prototype.NEWLINE = function() {
    return this.getToken(JassParser.NEWLINE, 0);
};

IfthenelseContext.prototype.statement_list = function() {
    return this.getTypedRuleContext(Statement_listContext,0);
};

IfthenelseContext.prototype.K_ENDIF = function() {
    return this.getToken(JassParser.K_ENDIF, 0);
};

IfthenelseContext.prototype.else_clause = function() {
    return this.getTypedRuleContext(Else_clauseContext,0);
};

IfthenelseContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterIfthenelse(this);
	}
};

IfthenelseContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitIfthenelse(this);
	}
};




JassParser.IfthenelseContext = IfthenelseContext;

JassParser.prototype.ifthenelse = function() {

    var localctx = new IfthenelseContext(this, this._ctx, this.state);
    this.enterRule(localctx, 32, JassParser.RULE_ifthenelse);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 217;
        this.match(JassParser.K_IF);
        this.state = 218;
        this.expr(0);
        this.state = 219;
        this.match(JassParser.K_THEN);
        this.state = 220;
        this.match(JassParser.NEWLINE);
        this.state = 221;
        this.statement_list();
        this.state = 223;
        this._errHandler.sync(this);
        _la = this._input.LA(1);
        if(_la===JassParser.K_ELSEIF || _la===JassParser.K_ELSE) {
            this.state = 222;
            this.else_clause();
        }

        this.state = 225;
        this.match(JassParser.K_ENDIF);
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function Else_clauseContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_else_clause;
    return this;
}

Else_clauseContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Else_clauseContext.prototype.constructor = Else_clauseContext;

Else_clauseContext.prototype.K_ELSE = function() {
    return this.getToken(JassParser.K_ELSE, 0);
};

Else_clauseContext.prototype.NEWLINE = function() {
    return this.getToken(JassParser.NEWLINE, 0);
};

Else_clauseContext.prototype.statement_list = function() {
    return this.getTypedRuleContext(Statement_listContext,0);
};

Else_clauseContext.prototype.K_ELSEIF = function() {
    return this.getToken(JassParser.K_ELSEIF, 0);
};

Else_clauseContext.prototype.expr = function() {
    return this.getTypedRuleContext(ExprContext,0);
};

Else_clauseContext.prototype.K_THEN = function() {
    return this.getToken(JassParser.K_THEN, 0);
};

Else_clauseContext.prototype.else_clause = function() {
    return this.getTypedRuleContext(Else_clauseContext,0);
};

Else_clauseContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterElse_clause(this);
	}
};

Else_clauseContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitElse_clause(this);
	}
};




JassParser.Else_clauseContext = Else_clauseContext;

JassParser.prototype.else_clause = function() {

    var localctx = new Else_clauseContext(this, this._ctx, this.state);
    this.enterRule(localctx, 34, JassParser.RULE_else_clause);
    var _la = 0; // Token type
    try {
        this.state = 238;
        this._errHandler.sync(this);
        switch(this._input.LA(1)) {
        case JassParser.K_ELSE:
            this.enterOuterAlt(localctx, 1);
            this.state = 227;
            this.match(JassParser.K_ELSE);
            this.state = 228;
            this.match(JassParser.NEWLINE);
            this.state = 229;
            this.statement_list();
            break;
        case JassParser.K_ELSEIF:
            this.enterOuterAlt(localctx, 2);
            this.state = 230;
            this.match(JassParser.K_ELSEIF);
            this.state = 231;
            this.expr(0);
            this.state = 232;
            this.match(JassParser.K_THEN);
            this.state = 233;
            this.match(JassParser.NEWLINE);
            this.state = 234;
            this.statement_list();
            this.state = 236;
            this._errHandler.sync(this);
            _la = this._input.LA(1);
            if(_la===JassParser.K_ELSEIF || _la===JassParser.K_ELSE) {
                this.state = 235;
                this.else_clause();
            }

            break;
        default:
            throw new antlr4.error.NoViableAltException(this);
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function LoopContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_loop;
    return this;
}

LoopContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
LoopContext.prototype.constructor = LoopContext;

LoopContext.prototype.K_LOOP = function() {
    return this.getToken(JassParser.K_LOOP, 0);
};

LoopContext.prototype.NEWLINE = function() {
    return this.getToken(JassParser.NEWLINE, 0);
};

LoopContext.prototype.statement_list = function() {
    return this.getTypedRuleContext(Statement_listContext,0);
};

LoopContext.prototype.K_ENDLOOP = function() {
    return this.getToken(JassParser.K_ENDLOOP, 0);
};

LoopContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterLoop(this);
	}
};

LoopContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitLoop(this);
	}
};




JassParser.LoopContext = LoopContext;

JassParser.prototype.loop = function() {

    var localctx = new LoopContext(this, this._ctx, this.state);
    this.enterRule(localctx, 36, JassParser.RULE_loop);
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 240;
        this.match(JassParser.K_LOOP);
        this.state = 241;
        this.match(JassParser.NEWLINE);
        this.state = 242;
        this.statement_list();
        this.state = 243;
        this.match(JassParser.K_ENDLOOP);
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function ExitwhenContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_exitwhen;
    return this;
}

ExitwhenContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
ExitwhenContext.prototype.constructor = ExitwhenContext;

ExitwhenContext.prototype.K_EXITWHEN = function() {
    return this.getToken(JassParser.K_EXITWHEN, 0);
};

ExitwhenContext.prototype.expr = function() {
    return this.getTypedRuleContext(ExprContext,0);
};

ExitwhenContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterExitwhen(this);
	}
};

ExitwhenContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitExitwhen(this);
	}
};




JassParser.ExitwhenContext = ExitwhenContext;

JassParser.prototype.exitwhen = function() {

    var localctx = new ExitwhenContext(this, this._ctx, this.state);
    this.enterRule(localctx, 38, JassParser.RULE_exitwhen);
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 245;
        this.match(JassParser.K_EXITWHEN);
        this.state = 246;
        this.expr(0);
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function Return_statContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_return_stat;
    return this;
}

Return_statContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Return_statContext.prototype.constructor = Return_statContext;

Return_statContext.prototype.K_RETURN = function() {
    return this.getToken(JassParser.K_RETURN, 0);
};

Return_statContext.prototype.expr = function() {
    return this.getTypedRuleContext(ExprContext,0);
};

Return_statContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterReturn_stat(this);
	}
};

Return_statContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitReturn_stat(this);
	}
};




JassParser.Return_statContext = Return_statContext;

JassParser.prototype.return_stat = function() {

    var localctx = new Return_statContext(this, this._ctx, this.state);
    this.enterRule(localctx, 40, JassParser.RULE_return_stat);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 248;
        this.match(JassParser.K_RETURN);
        this.state = 250;
        this._errHandler.sync(this);
        _la = this._input.LA(1);
        if((((_la) & ~0x1f) == 0 && ((1 << _la) & ((1 << JassParser.K_TRUE) | (1 << JassParser.K_FALSE) | (1 << JassParser.K_NULL) | (1 << JassParser.K_NOT) | (1 << JassParser.K_FUNCTION))) !== 0) || ((((_la - 38)) & ~0x1f) == 0 && ((1 << (_la - 38)) & ((1 << (JassParser.ADD - 38)) | (1 << (JassParser.SUB - 38)) | (1 << (JassParser.O_PAR - 38)) | (1 << (JassParser.DECIMAL - 38)) | (1 << (JassParser.OCTAL - 38)) | (1 << (JassParser.HEX - 38)) | (1 << (JassParser.FOURCC - 38)) | (1 << (JassParser.REAL_CONST - 38)) | (1 << (JassParser.STRING_CONST - 38)) | (1 << (JassParser.ID - 38)))) !== 0)) {
            this.state = 249;
            this.expr(0);
        }

    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function DebugContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_debug;
    return this;
}

DebugContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
DebugContext.prototype.constructor = DebugContext;

DebugContext.prototype.K_DEBUG = function() {
    return this.getToken(JassParser.K_DEBUG, 0);
};

DebugContext.prototype.set = function() {
    return this.getTypedRuleContext(SetContext,0);
};

DebugContext.prototype.call = function() {
    return this.getTypedRuleContext(CallContext,0);
};

DebugContext.prototype.ifthenelse = function() {
    return this.getTypedRuleContext(IfthenelseContext,0);
};

DebugContext.prototype.loop = function() {
    return this.getTypedRuleContext(LoopContext,0);
};

DebugContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterDebug(this);
	}
};

DebugContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitDebug(this);
	}
};




JassParser.DebugContext = DebugContext;

JassParser.prototype.debug = function() {

    var localctx = new DebugContext(this, this._ctx, this.state);
    this.enterRule(localctx, 42, JassParser.RULE_debug);
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 252;
        this.match(JassParser.K_DEBUG);
        this.state = 257;
        this._errHandler.sync(this);
        switch(this._input.LA(1)) {
        case JassParser.K_SET:
            this.state = 253;
            this.set();
            break;
        case JassParser.K_CALL:
            this.state = 254;
            this.call();
            break;
        case JassParser.K_IF:
            this.state = 255;
            this.ifthenelse();
            break;
        case JassParser.K_LOOP:
            this.state = 256;
            this.loop();
            break;
        default:
            throw new antlr4.error.NoViableAltException(this);
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function ExprContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_expr;
    return this;
}

ExprContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
ExprContext.prototype.constructor = ExprContext;

ExprContext.prototype.ADD = function() {
    return this.getToken(JassParser.ADD, 0);
};

ExprContext.prototype.expr = function(i) {
    if(i===undefined) {
        i = null;
    }
    if(i===null) {
        return this.getTypedRuleContexts(ExprContext);
    } else {
        return this.getTypedRuleContext(ExprContext,i);
    }
};

ExprContext.prototype.SUB = function() {
    return this.getToken(JassParser.SUB, 0);
};

ExprContext.prototype.K_NOT = function() {
    return this.getToken(JassParser.K_NOT, 0);
};

ExprContext.prototype.func_call = function() {
    return this.getTypedRuleContext(Func_callContext,0);
};

ExprContext.prototype.array_ref = function() {
    return this.getTypedRuleContext(Array_refContext,0);
};

ExprContext.prototype.func_ref = function() {
    return this.getTypedRuleContext(Func_refContext,0);
};

ExprContext.prototype.ID = function() {
    return this.getToken(JassParser.ID, 0);
};

ExprContext.prototype.constant = function() {
    return this.getTypedRuleContext(ConstantContext,0);
};

ExprContext.prototype.parens = function() {
    return this.getTypedRuleContext(ParensContext,0);
};

ExprContext.prototype.MULT = function() {
    return this.getToken(JassParser.MULT, 0);
};

ExprContext.prototype.DIV = function() {
    return this.getToken(JassParser.DIV, 0);
};

ExprContext.prototype.GT = function() {
    return this.getToken(JassParser.GT, 0);
};

ExprContext.prototype.LT = function() {
    return this.getToken(JassParser.LT, 0);
};

ExprContext.prototype.GTEQ = function() {
    return this.getToken(JassParser.GTEQ, 0);
};

ExprContext.prototype.LTEQ = function() {
    return this.getToken(JassParser.LTEQ, 0);
};

ExprContext.prototype.EQ = function() {
    return this.getToken(JassParser.EQ, 0);
};

ExprContext.prototype.NEQ = function() {
    return this.getToken(JassParser.NEQ, 0);
};

ExprContext.prototype.K_AND = function() {
    return this.getToken(JassParser.K_AND, 0);
};

ExprContext.prototype.K_OR = function() {
    return this.getToken(JassParser.K_OR, 0);
};

ExprContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterExpr(this);
	}
};

ExprContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitExpr(this);
	}
};



JassParser.prototype.expr = function(_p) {
	if(_p===undefined) {
	    _p = 0;
	}
    var _parentctx = this._ctx;
    var _parentState = this.state;
    var localctx = new ExprContext(this, this._ctx, _parentState);
    var _prevctx = localctx;
    var _startState = 44;
    this.enterRecursionRule(localctx, 44, JassParser.RULE_expr, _p);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 272;
        this._errHandler.sync(this);
        var la_ = this._interp.adaptivePredict(this._input,24,this._ctx);
        switch(la_) {
        case 1:
            this.state = 260;
            this.match(JassParser.ADD);
            this.state = 261;
            this.expr(14);
            break;

        case 2:
            this.state = 262;
            this.match(JassParser.SUB);
            this.state = 263;
            this.expr(13);
            break;

        case 3:
            this.state = 264;
            this.match(JassParser.K_NOT);
            this.state = 265;
            this.expr(12);
            break;

        case 4:
            this.state = 266;
            this.func_call();
            break;

        case 5:
            this.state = 267;
            this.array_ref();
            break;

        case 6:
            this.state = 268;
            this.func_ref();
            break;

        case 7:
            this.state = 269;
            this.match(JassParser.ID);
            break;

        case 8:
            this.state = 270;
            this.constant();
            break;

        case 9:
            this.state = 271;
            this.parens();
            break;

        }
        this._ctx.stop = this._input.LT(-1);
        this.state = 291;
        this._errHandler.sync(this);
        var _alt = this._interp.adaptivePredict(this._input,26,this._ctx)
        while(_alt!=2 && _alt!=antlr4.atn.ATN.INVALID_ALT_NUMBER) {
            if(_alt===1) {
                if(this._parseListeners!==null) {
                    this.triggerExitRuleEvent();
                }
                _prevctx = localctx;
                this.state = 289;
                this._errHandler.sync(this);
                var la_ = this._interp.adaptivePredict(this._input,25,this._ctx);
                switch(la_) {
                case 1:
                    localctx = new ExprContext(this, _parentctx, _parentState);
                    this.pushNewRecursionContext(localctx, _startState, JassParser.RULE_expr);
                    this.state = 274;
                    if (!( this.precpred(this._ctx, 11))) {
                        throw new antlr4.error.FailedPredicateException(this, "this.precpred(this._ctx, 11)");
                    }
                    this.state = 275;
                    _la = this._input.LA(1);
                    if(!(_la===JassParser.MULT || _la===JassParser.DIV)) {
                    this._errHandler.recoverInline(this);
                    }
                    else {
                    	this._errHandler.reportMatch(this);
                        this.consume();
                    }
                    this.state = 276;
                    this.expr(12);
                    break;

                case 2:
                    localctx = new ExprContext(this, _parentctx, _parentState);
                    this.pushNewRecursionContext(localctx, _startState, JassParser.RULE_expr);
                    this.state = 277;
                    if (!( this.precpred(this._ctx, 10))) {
                        throw new antlr4.error.FailedPredicateException(this, "this.precpred(this._ctx, 10)");
                    }
                    this.state = 278;
                    _la = this._input.LA(1);
                    if(!(_la===JassParser.ADD || _la===JassParser.SUB)) {
                    this._errHandler.recoverInline(this);
                    }
                    else {
                    	this._errHandler.reportMatch(this);
                        this.consume();
                    }
                    this.state = 279;
                    this.expr(11);
                    break;

                case 3:
                    localctx = new ExprContext(this, _parentctx, _parentState);
                    this.pushNewRecursionContext(localctx, _startState, JassParser.RULE_expr);
                    this.state = 280;
                    if (!( this.precpred(this._ctx, 9))) {
                        throw new antlr4.error.FailedPredicateException(this, "this.precpred(this._ctx, 9)");
                    }
                    this.state = 281;
                    _la = this._input.LA(1);
                    if(!(((((_la - 42)) & ~0x1f) == 0 && ((1 << (_la - 42)) & ((1 << (JassParser.GT - 42)) | (1 << (JassParser.LT - 42)) | (1 << (JassParser.GTEQ - 42)) | (1 << (JassParser.LTEQ - 42)))) !== 0))) {
                    this._errHandler.recoverInline(this);
                    }
                    else {
                    	this._errHandler.reportMatch(this);
                        this.consume();
                    }
                    this.state = 282;
                    this.expr(10);
                    break;

                case 4:
                    localctx = new ExprContext(this, _parentctx, _parentState);
                    this.pushNewRecursionContext(localctx, _startState, JassParser.RULE_expr);
                    this.state = 283;
                    if (!( this.precpred(this._ctx, 8))) {
                        throw new antlr4.error.FailedPredicateException(this, "this.precpred(this._ctx, 8)");
                    }
                    this.state = 284;
                    _la = this._input.LA(1);
                    if(!(_la===JassParser.EQ || _la===JassParser.NEQ)) {
                    this._errHandler.recoverInline(this);
                    }
                    else {
                    	this._errHandler.reportMatch(this);
                        this.consume();
                    }
                    this.state = 285;
                    this.expr(9);
                    break;

                case 5:
                    localctx = new ExprContext(this, _parentctx, _parentState);
                    this.pushNewRecursionContext(localctx, _startState, JassParser.RULE_expr);
                    this.state = 286;
                    if (!( this.precpred(this._ctx, 7))) {
                        throw new antlr4.error.FailedPredicateException(this, "this.precpred(this._ctx, 7)");
                    }
                    this.state = 287;
                    _la = this._input.LA(1);
                    if(!(_la===JassParser.K_AND || _la===JassParser.K_OR)) {
                    this._errHandler.recoverInline(this);
                    }
                    else {
                    	this._errHandler.reportMatch(this);
                        this.consume();
                    }
                    this.state = 288;
                    this.expr(8);
                    break;

                } 
            }
            this.state = 293;
            this._errHandler.sync(this);
            _alt = this._interp.adaptivePredict(this._input,26,this._ctx);
        }

    } catch( error) {
        if(error instanceof antlr4.error.RecognitionException) {
	        localctx.exception = error;
	        this._errHandler.reportError(this, error);
	        this._errHandler.recover(this, error);
	    } else {
	    	throw error;
	    }
    } finally {
        this.unrollRecursionContexts(_parentctx)
    }
    return localctx;
};


function Func_callContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_func_call;
    return this;
}

Func_callContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Func_callContext.prototype.constructor = Func_callContext;

Func_callContext.prototype.ID = function() {
    return this.getToken(JassParser.ID, 0);
};

Func_callContext.prototype.O_PAR = function() {
    return this.getToken(JassParser.O_PAR, 0);
};

Func_callContext.prototype.C_PAR = function() {
    return this.getToken(JassParser.C_PAR, 0);
};

Func_callContext.prototype.args = function() {
    return this.getTypedRuleContext(ArgsContext,0);
};

Func_callContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterFunc_call(this);
	}
};

Func_callContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitFunc_call(this);
	}
};




JassParser.Func_callContext = Func_callContext;

JassParser.prototype.func_call = function() {

    var localctx = new Func_callContext(this, this._ctx, this.state);
    this.enterRule(localctx, 46, JassParser.RULE_func_call);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 294;
        this.match(JassParser.ID);
        this.state = 295;
        this.match(JassParser.O_PAR);
        this.state = 297;
        this._errHandler.sync(this);
        _la = this._input.LA(1);
        if((((_la) & ~0x1f) == 0 && ((1 << _la) & ((1 << JassParser.K_TRUE) | (1 << JassParser.K_FALSE) | (1 << JassParser.K_NULL) | (1 << JassParser.K_NOT) | (1 << JassParser.K_FUNCTION))) !== 0) || ((((_la - 38)) & ~0x1f) == 0 && ((1 << (_la - 38)) & ((1 << (JassParser.ADD - 38)) | (1 << (JassParser.SUB - 38)) | (1 << (JassParser.O_PAR - 38)) | (1 << (JassParser.DECIMAL - 38)) | (1 << (JassParser.OCTAL - 38)) | (1 << (JassParser.HEX - 38)) | (1 << (JassParser.FOURCC - 38)) | (1 << (JassParser.REAL_CONST - 38)) | (1 << (JassParser.STRING_CONST - 38)) | (1 << (JassParser.ID - 38)))) !== 0)) {
            this.state = 296;
            this.args();
        }

        this.state = 299;
        this.match(JassParser.C_PAR);
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function Array_refContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_array_ref;
    return this;
}

Array_refContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Array_refContext.prototype.constructor = Array_refContext;

Array_refContext.prototype.ID = function() {
    return this.getToken(JassParser.ID, 0);
};

Array_refContext.prototype.O_BRACK = function() {
    return this.getToken(JassParser.O_BRACK, 0);
};

Array_refContext.prototype.expr = function() {
    return this.getTypedRuleContext(ExprContext,0);
};

Array_refContext.prototype.C_BRACK = function() {
    return this.getToken(JassParser.C_BRACK, 0);
};

Array_refContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterArray_ref(this);
	}
};

Array_refContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitArray_ref(this);
	}
};




JassParser.Array_refContext = Array_refContext;

JassParser.prototype.array_ref = function() {

    var localctx = new Array_refContext(this, this._ctx, this.state);
    this.enterRule(localctx, 48, JassParser.RULE_array_ref);
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 301;
        this.match(JassParser.ID);
        this.state = 302;
        this.match(JassParser.O_BRACK);
        this.state = 303;
        this.expr(0);
        this.state = 304;
        this.match(JassParser.C_BRACK);
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function Func_refContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_func_ref;
    return this;
}

Func_refContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Func_refContext.prototype.constructor = Func_refContext;

Func_refContext.prototype.K_FUNCTION = function() {
    return this.getToken(JassParser.K_FUNCTION, 0);
};

Func_refContext.prototype.ID = function() {
    return this.getToken(JassParser.ID, 0);
};

Func_refContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterFunc_ref(this);
	}
};

Func_refContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitFunc_ref(this);
	}
};




JassParser.Func_refContext = Func_refContext;

JassParser.prototype.func_ref = function() {

    var localctx = new Func_refContext(this, this._ctx, this.state);
    this.enterRule(localctx, 50, JassParser.RULE_func_ref);
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 306;
        this.match(JassParser.K_FUNCTION);
        this.state = 307;
        this.match(JassParser.ID);
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function ConstantContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_constant;
    return this;
}

ConstantContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
ConstantContext.prototype.constructor = ConstantContext;

ConstantContext.prototype.int_const = function() {
    return this.getTypedRuleContext(Int_constContext,0);
};

ConstantContext.prototype.REAL_CONST = function() {
    return this.getToken(JassParser.REAL_CONST, 0);
};

ConstantContext.prototype.bool_const = function() {
    return this.getTypedRuleContext(Bool_constContext,0);
};

ConstantContext.prototype.STRING_CONST = function() {
    return this.getToken(JassParser.STRING_CONST, 0);
};

ConstantContext.prototype.K_NULL = function() {
    return this.getToken(JassParser.K_NULL, 0);
};

ConstantContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterConstant(this);
	}
};

ConstantContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitConstant(this);
	}
};




JassParser.ConstantContext = ConstantContext;

JassParser.prototype.constant = function() {

    var localctx = new ConstantContext(this, this._ctx, this.state);
    this.enterRule(localctx, 52, JassParser.RULE_constant);
    try {
        this.state = 314;
        this._errHandler.sync(this);
        switch(this._input.LA(1)) {
        case JassParser.DECIMAL:
        case JassParser.OCTAL:
        case JassParser.HEX:
        case JassParser.FOURCC:
            this.enterOuterAlt(localctx, 1);
            this.state = 309;
            this.int_const();
            break;
        case JassParser.REAL_CONST:
            this.enterOuterAlt(localctx, 2);
            this.state = 310;
            this.match(JassParser.REAL_CONST);
            break;
        case JassParser.K_TRUE:
        case JassParser.K_FALSE:
            this.enterOuterAlt(localctx, 3);
            this.state = 311;
            this.bool_const();
            break;
        case JassParser.STRING_CONST:
            this.enterOuterAlt(localctx, 4);
            this.state = 312;
            this.match(JassParser.STRING_CONST);
            break;
        case JassParser.K_NULL:
            this.enterOuterAlt(localctx, 5);
            this.state = 313;
            this.match(JassParser.K_NULL);
            break;
        default:
            throw new antlr4.error.NoViableAltException(this);
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function Int_constContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_int_const;
    return this;
}

Int_constContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Int_constContext.prototype.constructor = Int_constContext;

Int_constContext.prototype.DECIMAL = function() {
    return this.getToken(JassParser.DECIMAL, 0);
};

Int_constContext.prototype.OCTAL = function() {
    return this.getToken(JassParser.OCTAL, 0);
};

Int_constContext.prototype.HEX = function() {
    return this.getToken(JassParser.HEX, 0);
};

Int_constContext.prototype.FOURCC = function() {
    return this.getToken(JassParser.FOURCC, 0);
};

Int_constContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterInt_const(this);
	}
};

Int_constContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitInt_const(this);
	}
};




JassParser.Int_constContext = Int_constContext;

JassParser.prototype.int_const = function() {

    var localctx = new Int_constContext(this, this._ctx, this.state);
    this.enterRule(localctx, 54, JassParser.RULE_int_const);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 316;
        _la = this._input.LA(1);
        if(!(((((_la - 54)) & ~0x1f) == 0 && ((1 << (_la - 54)) & ((1 << (JassParser.DECIMAL - 54)) | (1 << (JassParser.OCTAL - 54)) | (1 << (JassParser.HEX - 54)) | (1 << (JassParser.FOURCC - 54)))) !== 0))) {
        this._errHandler.recoverInline(this);
        }
        else {
        	this._errHandler.reportMatch(this);
            this.consume();
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function ParensContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_parens;
    return this;
}

ParensContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
ParensContext.prototype.constructor = ParensContext;

ParensContext.prototype.O_PAR = function() {
    return this.getToken(JassParser.O_PAR, 0);
};

ParensContext.prototype.expr = function() {
    return this.getTypedRuleContext(ExprContext,0);
};

ParensContext.prototype.C_PAR = function() {
    return this.getToken(JassParser.C_PAR, 0);
};

ParensContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterParens(this);
	}
};

ParensContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitParens(this);
	}
};




JassParser.ParensContext = ParensContext;

JassParser.prototype.parens = function() {

    var localctx = new ParensContext(this, this._ctx, this.state);
    this.enterRule(localctx, 56, JassParser.RULE_parens);
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 318;
        this.match(JassParser.O_PAR);
        this.state = 319;
        this.expr(0);
        this.state = 320;
        this.match(JassParser.C_PAR);
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function Bool_constContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_bool_const;
    return this;
}

Bool_constContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
Bool_constContext.prototype.constructor = Bool_constContext;

Bool_constContext.prototype.K_TRUE = function() {
    return this.getToken(JassParser.K_TRUE, 0);
};

Bool_constContext.prototype.K_FALSE = function() {
    return this.getToken(JassParser.K_FALSE, 0);
};

Bool_constContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterBool_const(this);
	}
};

Bool_constContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitBool_const(this);
	}
};




JassParser.Bool_constContext = Bool_constContext;

JassParser.prototype.bool_const = function() {

    var localctx = new Bool_constContext(this, this._ctx, this.state);
    this.enterRule(localctx, 58, JassParser.RULE_bool_const);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 322;
        _la = this._input.LA(1);
        if(!(_la===JassParser.K_TRUE || _la===JassParser.K_FALSE)) {
        this._errHandler.recoverInline(this);
        }
        else {
        	this._errHandler.reportMatch(this);
            this.consume();
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


function TypeContext(parser, parent, invokingState) {
	if(parent===undefined) {
	    parent = null;
	}
	if(invokingState===undefined || invokingState===null) {
		invokingState = -1;
	}
	antlr4.ParserRuleContext.call(this, parent, invokingState);
    this.parser = parser;
    this.ruleIndex = JassParser.RULE_type;
    return this;
}

TypeContext.prototype = Object.create(antlr4.ParserRuleContext.prototype);
TypeContext.prototype.constructor = TypeContext;

TypeContext.prototype.ID = function() {
    return this.getToken(JassParser.ID, 0);
};

TypeContext.prototype.K_CODE = function() {
    return this.getToken(JassParser.K_CODE, 0);
};

TypeContext.prototype.K_HANDLE = function() {
    return this.getToken(JassParser.K_HANDLE, 0);
};

TypeContext.prototype.K_INTEGER = function() {
    return this.getToken(JassParser.K_INTEGER, 0);
};

TypeContext.prototype.K_REAL = function() {
    return this.getToken(JassParser.K_REAL, 0);
};

TypeContext.prototype.K_BOOLEAN = function() {
    return this.getToken(JassParser.K_BOOLEAN, 0);
};

TypeContext.prototype.K_STRING = function() {
    return this.getToken(JassParser.K_STRING, 0);
};

TypeContext.prototype.enterRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.enterType(this);
	}
};

TypeContext.prototype.exitRule = function(listener) {
    if(listener instanceof JassListener ) {
        listener.exitType(this);
	}
};




JassParser.TypeContext = TypeContext;

JassParser.prototype.type = function() {

    var localctx = new TypeContext(this, this._ctx, this.state);
    this.enterRule(localctx, 60, JassParser.RULE_type);
    var _la = 0; // Token type
    try {
        this.enterOuterAlt(localctx, 1);
        this.state = 324;
        _la = this._input.LA(1);
        if(!((((_la) & ~0x1f) == 0 && ((1 << _la) & ((1 << JassParser.K_CODE) | (1 << JassParser.K_HANDLE) | (1 << JassParser.K_INTEGER) | (1 << JassParser.K_REAL) | (1 << JassParser.K_BOOLEAN) | (1 << JassParser.K_STRING))) !== 0) || _la===JassParser.ID)) {
        this._errHandler.recoverInline(this);
        }
        else {
        	this._errHandler.reportMatch(this);
            this.consume();
        }
    } catch (re) {
    	if(re instanceof antlr4.error.RecognitionException) {
	        localctx.exception = re;
	        this._errHandler.reportError(this, re);
	        this._errHandler.recover(this, re);
	    } else {
	    	throw re;
	    }
    } finally {
        this.exitRule();
    }
    return localctx;
};


JassParser.prototype.sempred = function(localctx, ruleIndex, predIndex) {
	switch(ruleIndex) {
	case 22:
			return this.expr_sempred(localctx, predIndex);
    default:
        throw "No predicate with index:" + ruleIndex;
   }
};

JassParser.prototype.expr_sempred = function(localctx, predIndex) {
	switch(predIndex) {
		case 0:
			return this.precpred(this._ctx, 11);
		case 1:
			return this.precpred(this._ctx, 10);
		case 2:
			return this.precpred(this._ctx, 9);
		case 3:
			return this.precpred(this._ctx, 8);
		case 4:
			return this.precpred(this._ctx, 7);
		default:
			throw "No predicate with index:" + predIndex;
	}
};


exports.JassParser = JassParser;
