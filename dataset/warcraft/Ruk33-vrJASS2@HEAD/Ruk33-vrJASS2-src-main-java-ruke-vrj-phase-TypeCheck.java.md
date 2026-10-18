---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS2"
source_path: "src/main/java/ruke/vrj/phase/TypeCheck.java"
ref: HEAD
---

package ruke.vrj.phase;

import java.util.ArrayList;
import org.antlr.v4.runtime.ParserRuleContext;
import ruke.vrj.Symbol;
import ruke.vrj.SymbolFlag;
import ruke.vrj.SymbolTable;
import ruke.vrj.TypeChecker;
import ruke.vrj.antlr.vrjBaseVisitor;
import ruke.vrj.antlr.vrjParser;
import ruke.vrj.antlr.vrjParser.ExpressionContext;
import ruke.vrj.antlr.vrjParser.NameContext;
import ruke.vrj.compiler.Result;

/**
 * MIT License
 *
 * Copyright (c) 2017 Franco Montenegro
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute,
 * sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
public class TypeCheck extends vrjBaseVisitor<Symbol> {

  private final ArrayList<Result> results = new ArrayList<>();
  private SymbolTable symbols;
  private Symbol integer;
  private Symbol real;
  private Symbol string;
  private Symbol code;
  private Symbol booleanType;
  private Symbol nullType;

  /**
   * Create a type check visitor.
   *
   * @param symbols Symbols
   */
  public TypeCheck(final SymbolTable symbols) {
    this.symbols = symbols;
    this.integer = symbols.resolve("integer");
    this.real = symbols.resolve("real");
    this.string = symbols.resolve("string");
    this.code = symbols.resolve("code");
    this.booleanType = symbols.resolve("boolean");
    this.nullType = symbols.resolve("null");
  }

  public ArrayList<Result> getResults() {
    return this.results;
  }

  private final void checkForNumeric(final ParserRuleContext ctx, final Symbol expression) {
    if (!TypeChecker.isValidNumber(expression)) {
      this.results.add(
          new Result(
              ctx.getStart().getInputStream().getSourceName(),
              ctx.getStart().getLine(),
              ctx.getStart().getCharPositionInLine(),
              ctx.getStart().getCharPositionInLine() + ctx.getText().length(),
              "Expected number"));
    }
  }

  private final void checkForBoolean(final ParserRuleContext ctx, final Symbol expression) {
    if (!TypeChecker.compatible(this.booleanType, expression)) {
      this.results.add(
          new Result(
              ctx.getStart().getInputStream().getSourceName(),
              ctx.getStart().getLine(),
              ctx.getStart().getCharPositionInLine(),
              ctx.getStart().getCharPositionInLine() + ctx.getText().length(),
              "Expected boolean expression"));
    }
  }

  @Override
  public Symbol visitLibraryDeclaration(vrjParser.LibraryDeclarationContext ctx) {
    final String name = ctx.name(0).getText();
    final Symbol library = this.symbols.resolve(name);

    if (!library.initializer.isEmpty()) {
      final Symbol initializer = library.children.resolve(library.initializer);

      if (!TypeChecker.isValidInitializer(initializer)) {
        this.results.add(
            new Result(
                ctx.getStart().getInputStream().getSourceName(),
                ctx.getStart().getLine(),
                ctx.initializer.getStart().getCharPositionInLine(),
                ctx.initializer.getStart().getCharPositionInLine() + library.initializer.length(),
                "Invalid initializer. It must be a function with no parameters"));
      }
    }

    if (!library.extendsFrom.isEmpty()) {
      Symbol requirement;

      for (final NameContext requirementName : ctx.libraryRequirementsExpression().name()) {
        requirement = library.children.resolve(requirementName.getText());

        if (!TypeChecker.isLibrary(requirement)) {
          this.results.add(
              new Result(
                  ctx.getStart().getInputStream().getSourceName(),
                  ctx.getStart().getLine(),
                  requirementName.getStart().getCharPositionInLine(),
                  requirementName.getStart().getCharPositionInLine()
                      + requirementName.getText().length(),
                  "Invalid library requirement. Only libraries are valid"));
        }
      }
    }

    final SymbolTable prevSymbols = this.symbols;
    this.symbols = library.children;

    this.visit(ctx.libraryBody());

    this.symbols = prevSymbols;

    return library;
  }

  @Override
  public Symbol visitStructDeclaration(vrjParser.StructDeclarationContext ctx) {
    final String name = ctx.name().getText();
    final Symbol struct = this.symbols.resolve(name);

    if (!struct.extendsFrom.isEmpty() && !struct.extendsFrom.get(0).equals("array")) {
      final Symbol parent = struct.children.resolve(struct.extendsFrom.get(0));

      if (!TypeChecker.isStruct(parent)) {
        this.results.add(
            new Result(
                ctx.getStart().getInputStream().getSourceName(),
                ctx.getStart().getLine(),
                ctx.extendsFromExpression().getStart().getCharPositionInLine(),
                ctx.extendsFromExpression().getStart().getCharPositionInLine()
                    + struct.extendsFrom.get(0).length(),
                "Structs can only extend from struct or array"));
      }
    }

    final SymbolTable prevSymbols = this.symbols;
    this.symbols = struct.children;

    this.visit(ctx.structBody());

    this.symbols = prevSymbols;

    return struct;
  }

  @Override
  public Symbol visitFunctionDeclaration(vrjParser.FunctionDeclarationContext ctx) {
    final String name = ctx.functionSignature().name().getText();
    final Symbol function = this.symbols.resolve(name);

    final SymbolTable prevSymbols = this.symbols;
    this.symbols = function.children;

    this.visit(ctx.functionSignature());
    this.visit(ctx.statements());

    this.symbols = prevSymbols;

    return function;
  }

  @Override
  public Symbol visitMethodDeclaration(vrjParser.MethodDeclarationContext ctx) {
    final String name = ctx.functionSignature().name().getText();
    final Symbol method = this.symbols.resolve(name);

    method.registerImplementationIfNecessary();

    if ("onInit".equals(name) && !TypeChecker.isValidInitializer(method)) {
      this.results.add(
          new Result(
              ctx.getStart().getInputStream().getSourceName(),
              ctx.getStart().getLine(),
              ctx.functionSignature().name().getStart().getCharPositionInLine(),
              ctx.functionSignature().name().getStart().getCharPositionInLine() + name.length(),
              "Struct initializer must be static and must take no parameters"));
    }

    final SymbolTable prevSymbols = this.symbols;
    this.symbols = method.children;

    this.visit(ctx.functionSignature());
    this.visit(ctx.statements());

    this.symbols = prevSymbols;

    return method;
  }

  @Override
  public Symbol visitParenthesisExpression(vrjParser.ParenthesisExpressionContext ctx) {
    return this.visit(ctx.expression());
  }

  @Override
  public Symbol visitNegativeExpression(vrjParser.NegativeExpressionContext ctx) {
    final Symbol a = this.visit(ctx.expression());
    this.checkForNumeric(ctx.expression(), a);
    return this.integer;
  }

  @Override
  public Symbol visitNotExpression(vrjParser.NotExpressionContext ctx) {
    this.visit(ctx.expression());
    return this.booleanType;
  }

  @Override
  public Symbol visitModuloExpression(vrjParser.ModuloExpressionContext ctx) {
    final Symbol a = this.visit(ctx.left);
    final Symbol b = this.visit(ctx.right);

    this.checkForNumeric(ctx.left, a);
    this.checkForNumeric(ctx.right, b);

    return this.integer;
  }

  @Override
  public Symbol visitDivMultExpression(vrjParser.DivMultExpressionContext ctx) {
    final Symbol a = this.visit(ctx.left);
    final Symbol b = this.visit(ctx.right);

    this.checkForNumeric(ctx.left, a);
    this.checkForNumeric(ctx.right, b);

    return this.integer;
  }

  @Override
  public Symbol visitSumSubExpression(vrjParser.SumSubExpressionContext ctx) {
    final Symbol a = this.visit(ctx.left);
    final Symbol b = this.visit(ctx.right);

    this.checkForNumeric(ctx.left, a);
    this.checkForNumeric(ctx.right, b);

    return this.integer;
  }

  @Override
  public Symbol visitChainExpression(vrjParser.ChainExpressionContext ctx) {
    final SymbolTable prevSymbols = this.symbols;
    Symbol symbol = null;

    for (final ExpressionContext member : ctx.expression()) {
      symbol = this.visit(member);

      if (symbol.equals(Symbol.NOTHING)) {
        break;
      }

      // Make a copy to avoid searching in parents (we are only interested in children)
      this.symbols = SymbolTable.copyOf(symbol.children.resolve(symbol.type).children);
    }

    this.symbols = prevSymbols;

    return symbol;
  }

  @Override
  public Symbol visitVariableExpression(vrjParser.VariableExpressionContext ctx) {
    final String name = ctx.name().getText();
    final Symbol variable = this.symbols.resolve(name);

    if (variable.equals(Symbol.NOTHING)) {
      this.results.add(
          new Result(
              ctx.getStart().getInputStream().getSourceName(),
              ctx.getStart().getLine(),
              ctx.name().getStart().getCharPositionInLine(),
              ctx.name().getStart().getCharPositionInLine() + name.length(),
              "Variable " + name + " is not defined"));
      return variable;
    }

    if (ctx.index != null) {
      if (!variable.flags.contains(SymbolFlag.ARRAY)) {
        this.results.add(
            new Result(
                ctx.getStart().getInputStream().getSourceName(),
                ctx.getStart().getLine(),
                ctx.getStart().getCharPositionInLine(),
                ctx.getStart().getCharPositionInLine() + ctx.getText().length(),
                "Variable " + name + " is not an array"));

        return variable;
      }

      final Symbol index = this.visit(ctx.index);

      if (!TypeChecker.isValidArrayIndex(index)) {
        this.results.add(
            new Result(
                ctx.getStart().getInputStream().getSourceName(),
                ctx.getStart().getLine(),
                ctx.index.getStart().getCharPositionInLine(),
                ctx.index.getStart().getCharPositionInLine() + ctx.index.getText().length(),
                "Array index must be integer"));
      } else {
        return this.symbols.resolve(index.type);
      }
    }

    return variable;
  }

  @Override
  public Symbol visitFunctionExpression(vrjParser.FunctionExpressionContext ctx) {
    final String name = ctx.name().getText();
    final Symbol function = this.symbols.resolve(name);

    if (function.equals(Symbol.NOTHING)) {
      this.results.add(
          new Result(
              ctx.getStart().getInputStream().getSourceName(),
              ctx.getStart().getLine(),
              ctx.name().getStart().getCharPositionInLine(),
              ctx.name().getStart().getCharPositionInLine() + name.length(),
              "Function " + name + " is not defined"));
    } else {
      this.visit(ctx.arguments());
    }

    return function;
  }

  @Override
  public Symbol visitComparisonExpression(vrjParser.ComparisonExpressionContext ctx) {
    final Symbol a = this.visit(ctx.left);
    final Symbol b = this.visit(ctx.right);

    switch (ctx.operator.getText()) {
      case "==":
      case "!=":
        if (!TypeChecker.compatible(a, b)) {
          this.results.add(
              new Result(
                  ctx.getStart().getInputStream().getSourceName(),
                  ctx.getStart().getLine(),
                  ctx.getStart().getCharPositionInLine(),
                  ctx.getStart().getCharPositionInLine() + ctx.getText().length(),
                  "Comparing incompatible types"));
        }
        break;
      case "<=":
      case ">=":
      case "<":
      case ">":
        this.checkForNumeric(ctx.left, a);
        this.checkForNumeric(ctx.right, b);
        break;
      default:
        break;
    }

    return this.booleanType;
  }

  @Override
  public Symbol visitLogicalExpression(vrjParser.LogicalExpressionContext ctx) {
    final Symbol a = this.visit(ctx.left);
    final Symbol b = this.visit(ctx.right);

    this.checkForBoolean(ctx.left, a);
    this.checkForBoolean(ctx.right, b);

    return this.booleanType;
  }

  @Override
  public Symbol visitCodeExpression(vrjParser.CodeExpressionContext ctx) {
    final Symbol function = this.visit(ctx.code);

    if (!function.equals(Symbol.NOTHING) && !function.flags.contains(SymbolFlag.FUNCTION)) {
      this.results.add(
          new Result(
              ctx.getStart().getInputStream().getSourceName(),
              ctx.getStart().getLine(),
              ctx.code.getStart().getCharPositionInLine(),
              ctx.code.getStart().getCharPositionInLine() + ctx.code.getText().length(),
              "Expected function"));
    }

    return this.code;
  }

  @Override
  public Symbol visitBooleanExpression(vrjParser.BooleanExpressionContext ctx) {
    return this.booleanType;
  }

  @Override
  public Symbol visitNullExpression(vrjParser.NullExpressionContext ctx) {
    return this.nullType;
  }

  @Override
  public Symbol visitStringExpression(vrjParser.StringExpressionContext ctx) {
    return this.string;
  }

  @Override
  public Symbol visitRealExpression(vrjParser.RealExpressionContext ctx) {
    return this.real;
  }

  @Override
  public Symbol visitIntegerExpression(vrjParser.IntegerExpressionContext ctx) {
    return this.integer;
  }

  @Override
  public Symbol visitNonArrayVariableDeclaration(vrjParser.NonArrayVariableDeclarationContext ctx) {
    final String name = ctx.name().getText();
    final Symbol variable = this.symbols.resolve(name);

    if (ctx.value != null) {
      final Symbol value = this.visit(ctx.value);

      if (!TypeChecker.compatible(variable, value)) {
        this.results.add(
            new Result(
                ctx.getStart().getInputStream().getSourceName(),
                ctx.getStart().getLine(),
                ctx.value.getStart().getCharPositionInLine(),
                ctx.value.getStart().getCharPositionInLine() + ctx.value.getText().length(),
                "Incompatible type"));
      }
    }

    return variable;
  }

  @Override
  public Symbol visitSetVariableStatement(vrjParser.SetVariableStatementContext ctx) {
    final Symbol variable = this.visit(ctx.variable);
    final Symbol value = this.visit(ctx.value);
    final Symbol valueType = this.symbols.resolve(value.type);

    if (!value.equals(Symbol.NOTHING) && !TypeChecker.compatible(variable, valueType)) {
      this.results.add(
          new Result(
              ctx.getStart().getInputStream().getSourceName(),
              ctx.getStart().getLine(),
              ctx.value.getStart().getCharPositionInLine(),
              ctx.value.getStart().getCharPositionInLine() + ctx.value.getText().length(),
              "Incompatible type"));
    }

    return variable;
  }

  @Override
  public Symbol visitFunctionCallStatement(vrjParser.FunctionCallStatementContext ctx) {
    final Symbol function = this.visit(ctx.function);

    if (!function.equals(Symbol.NOTHING) && !function.flags.contains(SymbolFlag.FUNCTION)) {
      this.results.add(
          new Result(
              ctx.getStart().getInputStream().getSourceName(),
              ctx.getStart().getLine(),
              ctx.function.getStart().getCharPositionInLine(),
              ctx.function.getStart().getCharPositionInLine() + ctx.function.getText().length(),
              "Not a function"));
    }

    return function;
  }

  @Override
  public Symbol visitExitWhenStatement(vrjParser.ExitWhenStatementContext ctx) {
    final Symbol condition = visit(ctx.condition);

    if (!TypeChecker.compatible(this.booleanType, condition)) {
      this.results.add(
          new Result(
              ctx.getStart().getInputStream().getSourceName(),
              ctx.getStart().getLine(),
              ctx.condition.getStart().getCharPositionInLine(),
              ctx.condition.getStart().getCharPositionInLine() + ctx.condition.getText().length(),
              "Exitwhen condition must be a boolean expression"));
    }

    return Symbol.NOTHING;
  }

  @Override
  public Symbol visitReturnStatement(vrjParser.ReturnStatementContext ctx) {
    final Symbol expectedValue = this.symbols.resolve(this.symbols.owner.type);
    final Symbol value = ctx.expression() == null ? Symbol.NOTHING : this.visit(ctx.expression());

    if (!TypeChecker.compatible(expectedValue, value)) {
      this.results.add(
          new Result(
              ctx.getStart().getInputStream().getSourceName(),
              ctx.getStart().getLine(),
              ctx.getStart().getCharPositionInLine(),
              ctx.getStart().getCharPositionInLine() + ctx.getText().length(),
              "Incompatible type"));
    }

    return expectedValue;
  }

  @Override
  public Symbol visitIfStatement(vrjParser.IfStatementContext ctx) {
    final Symbol condition = this.visit(ctx.condition);
    this.checkForBoolean(ctx.condition, condition);

    this.visit(ctx.statements());

    if (ctx.elseIfStatement() != null) {
      for (final vrjParser.ElseIfStatementContext elseif : ctx.elseIfStatement()) {
        this.visit(elseif);
      }
    }

    if (ctx.elseStatement() != null) {
      this.visit(ctx.elseStatement());
    }

    return Symbol.NOTHING;
  }

  @Override
  public Symbol visitElseIfStatement(vrjParser.ElseIfStatementContext ctx) {
    final Symbol condition = this.visit(ctx.condition);
    this.checkForBoolean(ctx.condition, condition);

    this.visit(ctx.statements());

    return Symbol.NOTHING;
  }
}
