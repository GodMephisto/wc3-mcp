---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/Antlr/JS/JassVisitor.js"
ref: HEAD
---

// Generated from Jass.g4 by ANTLR 4.8
// jshint ignore: start
var antlr4 = require('antlr4/index');

// This class defines a complete generic visitor for a parse tree produced by JassParser.

function JassVisitor() {
	antlr4.tree.ParseTreeVisitor.call(this);
	return this;
}

JassVisitor.prototype = Object.create(antlr4.tree.ParseTreeVisitor.prototype);
JassVisitor.prototype.constructor = JassVisitor;

// Visit a parse tree produced by JassParser#file.
JassVisitor.prototype.visitFile = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#declr.
JassVisitor.prototype.visitDeclr = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#typedef.
JassVisitor.prototype.visitTypedef = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#globals.
JassVisitor.prototype.visitGlobals = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#global_var_list.
JassVisitor.prototype.visitGlobal_var_list = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#global_var_declr.
JassVisitor.prototype.visitGlobal_var_declr = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#native_func.
JassVisitor.prototype.visitNative_func = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#func_declr.
JassVisitor.prototype.visitFunc_declr = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#param_list.
JassVisitor.prototype.visitParam_list = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#func.
JassVisitor.prototype.visitFunc = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#local_var_list.
JassVisitor.prototype.visitLocal_var_list = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#local_var_declr.
JassVisitor.prototype.visitLocal_var_declr = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#var_declr.
JassVisitor.prototype.visitVar_declr = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#statement_list.
JassVisitor.prototype.visitStatement_list = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#statement.
JassVisitor.prototype.visitStatement = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#set.
JassVisitor.prototype.visitSet = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#call.
JassVisitor.prototype.visitCall = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#args.
JassVisitor.prototype.visitArgs = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#ifthenelse.
JassVisitor.prototype.visitIfthenelse = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#else_clause.
JassVisitor.prototype.visitElse_clause = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#loop.
JassVisitor.prototype.visitLoop = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#exitwhen.
JassVisitor.prototype.visitExitwhen = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#return_stat.
JassVisitor.prototype.visitReturn_stat = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#debug.
JassVisitor.prototype.visitDebug = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#expr.
JassVisitor.prototype.visitExpr = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#func_call.
JassVisitor.prototype.visitFunc_call = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#array_ref.
JassVisitor.prototype.visitArray_ref = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#func_ref.
JassVisitor.prototype.visitFunc_ref = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#constant.
JassVisitor.prototype.visitConstant = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#int_const.
JassVisitor.prototype.visitInt_const = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#parens.
JassVisitor.prototype.visitParens = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#boolConst.
JassVisitor.prototype.visitBoolConst = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#stringConst.
JassVisitor.prototype.visitStringConst = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#type.
JassVisitor.prototype.visitType = function(ctx) {
  return this.visitChildren(ctx);
};


// Visit a parse tree produced by JassParser#id.
JassVisitor.prototype.visitId = function(ctx) {
  return this.visitChildren(ctx);
};



exports.JassVisitor = JassVisitor;