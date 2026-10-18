---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS2"
source_path: "src/main/java/ruke/vrj/phase/Translate.java"
ref: HEAD
---

package ruke.vrj.phase;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import ruke.vrj.Symbol;
import ruke.vrj.SymbolFlag;
import ruke.vrj.SymbolTable;
import ruke.vrj.antlr.vrjBaseVisitor;
import ruke.vrj.antlr.vrjParser;
import ruke.vrj.antlr.vrjParser.ExpressionContext;
import ruke.vrj.antlr.vrjParser.FunctionDeclarationContext;
import ruke.vrj.antlr.vrjParser.GlobalDeclarationContext;
import ruke.vrj.antlr.vrjParser.MethodDeclarationContext;
import ruke.vrj.antlr.vrjParser.StatementContext;
import ruke.vrj.antlr.vrjParser.StructDeclarationContext;
import ruke.vrj.antlr.vrjParser.TopDeclarationContext;
import ruke.vrj.translator.AssignmentStatement;
import ruke.vrj.translator.ChainExpression;
import ruke.vrj.translator.ChainableExpression;
import ruke.vrj.translator.CodeExpression;
import ruke.vrj.translator.ComparisonExpression;
import ruke.vrj.translator.ElseIfStatement;
import ruke.vrj.translator.ElseStatement;
import ruke.vrj.translator.ExitWhenStatement;
import ruke.vrj.translator.Expression;
import ruke.vrj.translator.FunctionDefinition;
import ruke.vrj.translator.FunctionExpression;
import ruke.vrj.translator.FunctionStatement;
import ruke.vrj.translator.GlobalDefinition;
import ruke.vrj.translator.IfStatement;
import ruke.vrj.translator.LogicalExpression;
import ruke.vrj.translator.LoopStatement;
import ruke.vrj.translator.MathExpression;
import ruke.vrj.translator.MathExpression.Operator;
import ruke.vrj.translator.NegativeExpression;
import ruke.vrj.translator.NotExpression;
import ruke.vrj.translator.ParenthesisExpression;
import ruke.vrj.translator.RawExpression;
import ruke.vrj.translator.ReturnStatement;
import ruke.vrj.translator.StatementList;
import ruke.vrj.translator.VariableExpression;
import ruke.vrj.translator.VariableStatement;
import sun.reflect.generics.reflectiveObjects.NotImplementedException;

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
public class Translate extends vrjBaseVisitor<Expression> {

  private SymbolTable symbols;
  private final Collection<Expression> globals;

  public Translate(final SymbolTable symbols) {
    this.symbols = symbols;
    this.globals = new ArrayList<>();
  }

  @Override
  public Expression visitInit(vrjParser.InitContext ctx) {
    final Collection<String> program = new ArrayList<>(ctx.topDeclaration().size());

    for (final TopDeclarationContext declaration : ctx.topDeclaration()) {
      final Expression statement = this.visit(declaration);

      if (statement != null) {
        program.add(statement.toString());
      }
    }

    return new RawExpression(
        new GlobalDefinition(this.globals) + "\n" + String.join("\n", program));
  }

  @Override
  public Expression visitLibraryBody(vrjParser.LibraryBodyContext ctx) {
    final Collection<Expression> statements = new ArrayList<>();

    if (ctx.globalDeclaration() != null) {
      for (final GlobalDeclarationContext global : ctx.globalDeclaration()) {
        this.visit(global);
      }
    }

    if (ctx.functionDeclaration() != null) {
      for (final FunctionDeclarationContext function : ctx.functionDeclaration()) {
        statements.add(this.visit(function));
      }
    }

    if (ctx.structDeclaration() != null) {
      for (final StructDeclarationContext struct : ctx.structDeclaration()) {
        statements.add(this.visit(struct));
      }
    }

    if (ctx.scopeDeclaration() != null) {
      for (final vrjParser.ScopeDeclarationContext scope : ctx.scopeDeclaration()) {
        statements.add(this.visit(scope));
      }
    }

    return new StatementList(statements);
  }

  @Override
  public Expression visitScopeBody(vrjParser.ScopeBodyContext ctx) {
    final Collection<Expression> statements = new ArrayList<>();

    if (ctx.globalDeclaration() != null) {
      for (final GlobalDeclarationContext global : ctx.globalDeclaration()) {
        this.visit(global);
      }
    }

    if (ctx.functionDeclaration() != null) {
      for (final FunctionDeclarationContext function : ctx.functionDeclaration()) {
        statements.add(this.visit(function));
      }
    }

    if (ctx.structDeclaration() != null) {
      for (final StructDeclarationContext struct : ctx.structDeclaration()) {
        statements.add(this.visit(struct));
      }
    }

    if (ctx.scopeDeclaration() != null) {
      for (final vrjParser.ScopeDeclarationContext scope : ctx.scopeDeclaration()) {
        statements.add(this.visit(scope));
      }
    }

    return new StatementList(statements);
  }

  @Override
  public Expression visitScopeDeclaration(vrjParser.ScopeDeclarationContext ctx) {
    final String name = ctx.name(0).getText();
    final Symbol scope = this.symbols.resolve(name);

    final SymbolTable prevSymbols = this.symbols;
    this.symbols = scope.children;

    final Expression result = this.visit(ctx.scopeBody());

    this.symbols = prevSymbols;

    return result;
  }

  @Override
  public Expression visitLibraryDeclaration(vrjParser.LibraryDeclarationContext ctx) {
    final String name = ctx.name(0).getText();
    final Symbol library = this.symbols.resolve(name);

    final SymbolTable prevSymbols = this.symbols;
    this.symbols = library.children;

    final Expression result = this.visit(ctx.libraryBody());

    this.symbols = prevSymbols;

    return result;
  }

  @Override
  public Expression visitPropertyDeclaration(vrjParser.PropertyDeclarationContext ctx) {
    return this.visit(ctx.variableDeclaration());
  }

  @Override
  public Expression visitMethodDeclaration(vrjParser.MethodDeclarationContext ctx) {
    final String name = ctx.functionSignature().name().getText();
    final Symbol method = this.symbols.resolve(name);
    final Collection<Expression> statements = new ArrayList<>(ctx.statements().statement().size());

    final SymbolTable prevSymbols = this.symbols;
    this.symbols = method.children;

    for (final StatementContext statement : ctx.statements().statement()) {
      statements.add(this.visit(statement));
    }

    this.symbols = prevSymbols;

    return new FunctionDefinition(method, statements);
  }

  @Override
  public Expression visitStructBody(vrjParser.StructBodyContext ctx) {
    final List<String> methods = new ArrayList<>();

    if (ctx.propertyDeclaration() != null) {
      for (final vrjParser.PropertyDeclarationContext property : ctx.propertyDeclaration()) {
        this.visit(property);
      }
    }

    if (ctx.methodDeclaration() != null) {
      for (final MethodDeclarationContext method : ctx.methodDeclaration()) {
        methods.add(this.visit(method).toString());
      }
    }

    return new RawExpression(String.join("\n", methods));
  }

  @Override
  public Expression visitStructDeclaration(vrjParser.StructDeclarationContext ctx) {
    final String name = ctx.name().getText();
    final Symbol struct = this.symbols.resolve(name);

    final SymbolTable prevSymbols = this.symbols;
    this.symbols = struct.children;

    final Expression result = this.visit(ctx.structBody());

    this.symbols = prevSymbols;

    return result;
  }

  @Override
  public Expression visitFunctionDeclaration(vrjParser.FunctionDeclarationContext ctx) {
    final String name = ctx.functionSignature().name().getText();
    final Symbol function = this.symbols.resolve(name);
    final Collection<Expression> statements = new ArrayList<>(ctx.statements().statement().size());

    final SymbolTable prevSymbols = this.symbols;
    this.symbols = function.children;

    for (final StatementContext statement : ctx.statements().statement()) {
      statements.add(this.visit(statement));
    }

    this.symbols = prevSymbols;

    return new FunctionDefinition(function, statements);
  }

  @Override
  public Expression visitParenthesisExpression(vrjParser.ParenthesisExpressionContext ctx) {
    return new ParenthesisExpression(this.visit(ctx.expression()));
  }

  @Override
  public Expression visitNegativeExpression(vrjParser.NegativeExpressionContext ctx) {
    return new NegativeExpression(this.visit(ctx.expression()));
  }

  @Override
  public Expression visitNotExpression(vrjParser.NotExpressionContext ctx) {
    return new NotExpression(this.visit(ctx.expression()));
  }

  @Override
  public Expression visitModuloExpression(vrjParser.ModuloExpressionContext ctx) {
    final Expression left = this.visit(ctx.left);
    final Expression right = this.visit(ctx.right);
    return new MathExpression(left, Operator.MODULO, right);
  }

  @Override
  public Expression visitDivMultExpression(vrjParser.DivMultExpressionContext ctx) {
    final Expression left = this.visit(ctx.left);
    final Expression right = this.visit(ctx.right);
    Operator operator = Operator.DIVISION;

    if ("*".equals(ctx.operator.getText())) {
      operator = Operator.MULTIPLICATION;
    }

    return new MathExpression(left, operator, right);
  }

  @Override
  public Expression visitSumSubExpression(vrjParser.SumSubExpressionContext ctx) {
    final Expression left = this.visit(ctx.left);
    final Expression right = this.visit(ctx.right);
    Operator operator = Operator.ADDITION;

    if ("-".equals(ctx.operator.getText())) {
      operator = Operator.SUBTRACTION;
    }

    return new MathExpression(left, operator, right);
  }

  @Override
  public Expression visitChainExpression(vrjParser.ChainExpressionContext ctx) {
    final Collection<ChainableExpression> chain = new ArrayList<>(ctx.expression().size());
    final SymbolTable prevSymbols = this.symbols;

    ChainableExpression memberExpression = null;
    Symbol memberSymbol = null;

    for (final ExpressionContext member : ctx.expression()) {
      memberExpression = (ChainableExpression) this.visit(member);
      memberSymbol = memberExpression.getSymbol();

      chain.add(memberExpression);

      // Make a copy to avoid searching in parents (we are only interested in children)
      this.symbols = SymbolTable.copyOf(memberSymbol.children.resolve(memberSymbol.type).children);
    }

    this.symbols = prevSymbols;

    return new ChainExpression(chain);
  }

  @Override
  public Expression visitVariableExpression(vrjParser.VariableExpressionContext ctx) {
    final String name = ctx.name().getText();
    final Symbol variable = this.symbols.resolve(name);
    Expression index = null;

    if (ctx.index != null) {
      index = this.visit(ctx.index);
    }

    return new VariableExpression(variable, index);
  }

  @Override
  public Expression visitFunctionExpression(vrjParser.FunctionExpressionContext ctx) {
    final String name = ctx.name().getText();
    final Symbol function = this.symbols.resolve(name);
    final Collection<Expression> args = new ArrayList<>(function.params.size());

    if (ctx.arguments().expressionList() != null) {
      for (final ExpressionContext arg : ctx.arguments().expressionList().expression()) {
        args.add(this.visit(arg));
      }
    }

    return new FunctionExpression(function, args);
  }

  @Override
  public Expression visitComparisonExpression(vrjParser.ComparisonExpressionContext ctx) {
    final Expression left = this.visit(ctx.left);
    final Expression right = this.visit(ctx.right);
    ComparisonExpression.Operator operator;

    switch (ctx.operator.getText()) {
      case "==":
        operator = ComparisonExpression.Operator.EQUAL;
        break;
      case "!=":
        operator = ComparisonExpression.Operator.NOT_EQUAL;
        break;
      case "<=":
        operator = ComparisonExpression.Operator.LOWER_THAN_EQUAL;
        break;
      case "<":
        operator = ComparisonExpression.Operator.LOWER_THAN;
        break;
      case ">=":
        operator = ComparisonExpression.Operator.GREATER_THAN_EQUAL;
        break;
      case ">":
        operator = ComparisonExpression.Operator.GREATER_THAN;
        break;
      default:
        throw new NotImplementedException();
    }

    return new ComparisonExpression(left, operator, right);
  }

  @Override
  public Expression visitLogicalExpression(vrjParser.LogicalExpressionContext ctx) {
    final Expression left = this.visit(ctx.left);
    final Expression right = this.visit(ctx.right);
    LogicalExpression.Operator operator;

    switch (ctx.operator.getText()) {
      case "or":
        operator = LogicalExpression.Operator.OR;
        break;
      case "and":
        operator = LogicalExpression.Operator.AND;
        break;
      default:
        throw new NotImplementedException();
    }

    return new LogicalExpression(left, operator, right);
  }

  @Override
  public Expression visitCodeExpression(vrjParser.CodeExpressionContext ctx) {
    return new CodeExpression(this.visit(ctx.code));
  }

  @Override
  public Expression visitBooleanExpression(vrjParser.BooleanExpressionContext ctx) {
    switch (ctx.getText()) {
      case "true":
        return new RawExpression("true");
      case "false":
        return new RawExpression("false");
      default:
        throw new NotImplementedException();
    }
  }

  @Override
  public Expression visitNullExpression(vrjParser.NullExpressionContext ctx) {
    return new RawExpression("null");
  }

  @Override
  public Expression visitStringExpression(vrjParser.StringExpressionContext ctx) {
    return new RawExpression(String.format("%s", ctx.getText()));
  }

  @Override
  public Expression visitRealExpression(vrjParser.RealExpressionContext ctx) {
    return new RawExpression(ctx.getText());
  }

  @Override
  public Expression visitIntegerExpression(vrjParser.IntegerExpressionContext ctx) {
    return new RawExpression(ctx.getText());
  }

  @Override
  public Expression visitGlobalVariableDeclaration(vrjParser.GlobalVariableDeclarationContext ctx) {
    return this.visit(ctx.variableDeclaration());
  }

  @Override
  public Expression visitLoopStatement(vrjParser.LoopStatementContext ctx) {
    final Collection<Expression> statements = new ArrayList<>(ctx.statements().statement().size());

    for (final StatementContext statement : ctx.statements().statement()) {
      statements.add(this.visit(statement));
    }

    return new LoopStatement(statements);
  }

  @Override
  public Expression visitElseIfStatement(vrjParser.ElseIfStatementContext ctx) {
    final Collection<Expression> statements = new ArrayList<>(ctx.statements().statement().size());

    for (final StatementContext statement : ctx.statements().statement()) {
      statements.add(this.visit(statement));
    }

    return new ElseIfStatement(this.visit(ctx.condition), statements);
  }

  @Override
  public Expression visitElseStatement(vrjParser.ElseStatementContext ctx) {
    final Collection<Expression> statements = new ArrayList<>(ctx.statements().statement().size());

    for (final StatementContext statement : ctx.statements().statement()) {
      statements.add(this.visit(statement));
    }

    return new ElseStatement(statements);
  }

  @Override
  public Expression visitIfStatement(vrjParser.IfStatementContext ctx) {
    final Collection<Expression> statements = new ArrayList<>(ctx.statements().statement().size());

    for (final StatementContext statement : ctx.statements().statement()) {
      statements.add(this.visit(statement));
    }

    return new IfStatement(this.visit(ctx.condition), statements);
  }

  @Override
  public Expression visitArrayVariableDeclaration(vrjParser.ArrayVariableDeclarationContext ctx) {
    final String name = ctx.name().getText();
    final Symbol variable = this.symbols.resolve(name);
    final Expression declaration = new VariableStatement(variable, null);

    if (variable.flags.contains(SymbolFlag.GLOBAL)) {
      this.globals.add(declaration);
      return null;
    }

    return declaration;
  }

  @Override
  public Expression visitNonArrayVariableDeclaration(
      vrjParser.NonArrayVariableDeclarationContext ctx) {
    final String name = ctx.name().getText();
    final Symbol variable = this.symbols.resolve(name);
    final Expression value = ctx.value == null ? null : this.visit(ctx.value);
    final Expression declaration = new VariableStatement(variable, value);

    if (variable.flags.contains(SymbolFlag.GLOBAL)) {
      this.globals.add(declaration);
      return null;
    }

    return declaration;
  }

  @Override
  public Expression visitLocalVariableDeclaration(vrjParser.LocalVariableDeclarationContext ctx) {
    return this.visit(ctx.variableDeclaration());
  }

  @Override
  public Expression visitSetVariableStatement(vrjParser.SetVariableStatementContext ctx) {
    return new AssignmentStatement(this.visit(ctx.variable), this.visit(ctx.value));
  }

  @Override
  public Expression visitExitWhenStatement(vrjParser.ExitWhenStatementContext ctx) {
    return new ExitWhenStatement(this.visit(ctx.condition));
  }

  @Override
  public Expression visitFunctionCallStatement(vrjParser.FunctionCallStatementContext ctx) {
    return new FunctionStatement(this.visit(ctx.expression()));
  }

  @Override
  public Expression visitReturnStatement(vrjParser.ReturnStatementContext ctx) {
    Expression value = null;

    if (ctx.expression() != null) {
      value = this.visit(ctx.expression());
    }

    return new ReturnStatement(value);
  }
}
