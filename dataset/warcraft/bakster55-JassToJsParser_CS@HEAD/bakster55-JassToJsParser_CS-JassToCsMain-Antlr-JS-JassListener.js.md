---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/Antlr/JS/JassListener.js"
ref: HEAD
---

// Generated from Jass.g4 by ANTLR 4.8
// jshint ignore: start
var antlr4 = require('antlr4/index');

// This class defines a complete listener for a parse tree produced by JassParser.
function JassListener() {
	antlr4.tree.ParseTreeListener.call(this);
	return this;
}

JassListener.prototype = Object.create(antlr4.tree.ParseTreeListener.prototype);
JassListener.prototype.constructor = JassListener;

// Enter a parse tree produced by JassParser#file.
JassListener.prototype.enterFile = function(ctx) {
};

// Exit a parse tree produced by JassParser#file.
JassListener.prototype.exitFile = function(ctx) {
};


// Enter a parse tree produced by JassParser#declr.
JassListener.prototype.enterDeclr = function(ctx) {
};

// Exit a parse tree produced by JassParser#declr.
JassListener.prototype.exitDeclr = function(ctx) {
};


// Enter a parse tree produced by JassParser#typedef.
JassListener.prototype.enterTypedef = function(ctx) {
};

// Exit a parse tree produced by JassParser#typedef.
JassListener.prototype.exitTypedef = function(ctx) {
};


// Enter a parse tree produced by JassParser#globals.
JassListener.prototype.enterGlobals = function(ctx) {
};

// Exit a parse tree produced by JassParser#globals.
JassListener.prototype.exitGlobals = function(ctx) {
};


// Enter a parse tree produced by JassParser#global_var_list.
JassListener.prototype.enterGlobal_var_list = function(ctx) {
};

// Exit a parse tree produced by JassParser#global_var_list.
JassListener.prototype.exitGlobal_var_list = function(ctx) {
};


// Enter a parse tree produced by JassParser#global_var_declr.
JassListener.prototype.enterGlobal_var_declr = function(ctx) {
};

// Exit a parse tree produced by JassParser#global_var_declr.
JassListener.prototype.exitGlobal_var_declr = function(ctx) {
};


// Enter a parse tree produced by JassParser#native_func.
JassListener.prototype.enterNative_func = function(ctx) {
};

// Exit a parse tree produced by JassParser#native_func.
JassListener.prototype.exitNative_func = function(ctx) {
};


// Enter a parse tree produced by JassParser#func_declr.
JassListener.prototype.enterFunc_declr = function(ctx) {
};

// Exit a parse tree produced by JassParser#func_declr.
JassListener.prototype.exitFunc_declr = function(ctx) {
};


// Enter a parse tree produced by JassParser#param_list.
JassListener.prototype.enterParam_list = function(ctx) {
};

// Exit a parse tree produced by JassParser#param_list.
JassListener.prototype.exitParam_list = function(ctx) {
};


// Enter a parse tree produced by JassParser#func.
JassListener.prototype.enterFunc = function(ctx) {
};

// Exit a parse tree produced by JassParser#func.
JassListener.prototype.exitFunc = function(ctx) {
};


// Enter a parse tree produced by JassParser#local_var_list.
JassListener.prototype.enterLocal_var_list = function(ctx) {
};

// Exit a parse tree produced by JassParser#local_var_list.
JassListener.prototype.exitLocal_var_list = function(ctx) {
};


// Enter a parse tree produced by JassParser#local_var_declr.
JassListener.prototype.enterLocal_var_declr = function(ctx) {
};

// Exit a parse tree produced by JassParser#local_var_declr.
JassListener.prototype.exitLocal_var_declr = function(ctx) {
};


// Enter a parse tree produced by JassParser#var_declr.
JassListener.prototype.enterVar_declr = function(ctx) {
};

// Exit a parse tree produced by JassParser#var_declr.
JassListener.prototype.exitVar_declr = function(ctx) {
};


// Enter a parse tree produced by JassParser#statement_list.
JassListener.prototype.enterStatement_list = function(ctx) {
};

// Exit a parse tree produced by JassParser#statement_list.
JassListener.prototype.exitStatement_list = function(ctx) {
};


// Enter a parse tree produced by JassParser#statement.
JassListener.prototype.enterStatement = function(ctx) {
};

// Exit a parse tree produced by JassParser#statement.
JassListener.prototype.exitStatement = function(ctx) {
};


// Enter a parse tree produced by JassParser#set.
JassListener.prototype.enterSet = function(ctx) {
};

// Exit a parse tree produced by JassParser#set.
JassListener.prototype.exitSet = function(ctx) {
};


// Enter a parse tree produced by JassParser#call.
JassListener.prototype.enterCall = function(ctx) {
};

// Exit a parse tree produced by JassParser#call.
JassListener.prototype.exitCall = function(ctx) {
};


// Enter a parse tree produced by JassParser#args.
JassListener.prototype.enterArgs = function(ctx) {
};

// Exit a parse tree produced by JassParser#args.
JassListener.prototype.exitArgs = function(ctx) {
};


// Enter a parse tree produced by JassParser#ifthenelse.
JassListener.prototype.enterIfthenelse = function(ctx) {
};

// Exit a parse tree produced by JassParser#ifthenelse.
JassListener.prototype.exitIfthenelse = function(ctx) {
};


// Enter a parse tree produced by JassParser#else_clause.
JassListener.prototype.enterElse_clause = function(ctx) {
};

// Exit a parse tree produced by JassParser#else_clause.
JassListener.prototype.exitElse_clause = function(ctx) {
};


// Enter a parse tree produced by JassParser#loop.
JassListener.prototype.enterLoop = function(ctx) {
};

// Exit a parse tree produced by JassParser#loop.
JassListener.prototype.exitLoop = function(ctx) {
};


// Enter a parse tree produced by JassParser#exitwhen.
JassListener.prototype.enterExitwhen = function(ctx) {
};

// Exit a parse tree produced by JassParser#exitwhen.
JassListener.prototype.exitExitwhen = function(ctx) {
};


// Enter a parse tree produced by JassParser#return_stat.
JassListener.prototype.enterReturn_stat = function(ctx) {
};

// Exit a parse tree produced by JassParser#return_stat.
JassListener.prototype.exitReturn_stat = function(ctx) {
};


// Enter a parse tree produced by JassParser#debug.
JassListener.prototype.enterDebug = function(ctx) {
};

// Exit a parse tree produced by JassParser#debug.
JassListener.prototype.exitDebug = function(ctx) {
};


// Enter a parse tree produced by JassParser#expr.
JassListener.prototype.enterExpr = function(ctx) {
};

// Exit a parse tree produced by JassParser#expr.
JassListener.prototype.exitExpr = function(ctx) {
};


// Enter a parse tree produced by JassParser#func_call.
JassListener.prototype.enterFunc_call = function(ctx) {
};

// Exit a parse tree produced by JassParser#func_call.
JassListener.prototype.exitFunc_call = function(ctx) {
};


// Enter a parse tree produced by JassParser#array_ref.
JassListener.prototype.enterArray_ref = function(ctx) {
};

// Exit a parse tree produced by JassParser#array_ref.
JassListener.prototype.exitArray_ref = function(ctx) {
};


// Enter a parse tree produced by JassParser#func_ref.
JassListener.prototype.enterFunc_ref = function(ctx) {
};

// Exit a parse tree produced by JassParser#func_ref.
JassListener.prototype.exitFunc_ref = function(ctx) {
};


// Enter a parse tree produced by JassParser#constant.
JassListener.prototype.enterConstant = function(ctx) {
};

// Exit a parse tree produced by JassParser#constant.
JassListener.prototype.exitConstant = function(ctx) {
};


// Enter a parse tree produced by JassParser#int_const.
JassListener.prototype.enterInt_const = function(ctx) {
};

// Exit a parse tree produced by JassParser#int_const.
JassListener.prototype.exitInt_const = function(ctx) {
};


// Enter a parse tree produced by JassParser#parens.
JassListener.prototype.enterParens = function(ctx) {
};

// Exit a parse tree produced by JassParser#parens.
JassListener.prototype.exitParens = function(ctx) {
};


// Enter a parse tree produced by JassParser#boolConst.
JassListener.prototype.enterBoolConst = function(ctx) {
};

// Exit a parse tree produced by JassParser#boolConst.
JassListener.prototype.exitBoolConst = function(ctx) {
};


// Enter a parse tree produced by JassParser#stringConst.
JassListener.prototype.enterStringConst = function(ctx) {
};

// Exit a parse tree produced by JassParser#stringConst.
JassListener.prototype.exitStringConst = function(ctx) {
};


// Enter a parse tree produced by JassParser#type.
JassListener.prototype.enterType = function(ctx) {
};

// Exit a parse tree produced by JassParser#type.
JassListener.prototype.exitType = function(ctx) {
};


// Enter a parse tree produced by JassParser#id.
JassListener.prototype.enterId = function(ctx) {
};

// Exit a parse tree produced by JassParser#id.
JassListener.prototype.exitId = function(ctx) {
};



exports.JassListener = JassListener;