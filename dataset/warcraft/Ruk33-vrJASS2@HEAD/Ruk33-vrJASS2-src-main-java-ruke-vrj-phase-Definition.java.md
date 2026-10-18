---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS2"
source_path: "src/main/java/ruke/vrj/phase/Definition.java"
ref: HEAD
---

package ruke.vrj.phase;

import com.google.common.collect.ImmutableSet;
import java.util.ArrayList;
import org.antlr.v4.runtime.ParserRuleContext;
import ruke.vrj.Symbol;
import ruke.vrj.SymbolFlag;
import ruke.vrj.SymbolTable;
import ruke.vrj.antlr.vrjBaseVisitor;
import ruke.vrj.antlr.vrjParser;
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
public class Definition extends vrjBaseVisitor<Symbol> {

  private final ArrayList<Result> results = new ArrayList<>();
  private SymbolTable symbols;

  public Definition(final SymbolTable symbols) {
    this.symbols = symbols;
  }

  public final ArrayList<Result> getResults() {
    return this.results;
  }

  private final void addAlreadyDefiedResult(
      final String source, final int line, final int start, final int end, final Symbol original) {
    String message;

    if (original.declaration == null) {
      message = "Symbol " + original.name + " is already defined";
    } else {
      message =
          String.format(
              "Symbol %s is already defined in %s(%d,%d,%d)",
              original.name,
              original.declaration.getStart().getInputStream().getSourceName(),
              original.declaration.getStart().getLine(),
              original.declaration.getStart().getCharPositionInLine(),
              original.declaration.getStart().getCharPositionInLine() + original.name.length());
    }

    this.results.add(new Result(source, line, start, end, message));
  }

  private final void addAlreadyDefiedResult(
      final ParserRuleContext ctx, final int start, final int end, final Symbol original) {
    this.addAlreadyDefiedResult(
        ctx.getStart().getInputStream().getSourceName(),
        ctx.getStart().getLine(),
        start,
        end,
        original);
  }

  @Override
  public Symbol visitLibraryDeclaration(vrjParser.LibraryDeclarationContext ctx) {
    final String name = ctx.name(0).getText();
    final Symbol defined = this.symbols.resolve(name);

    if (!defined.equals(Symbol.NOTHING)) {
      this.addAlreadyDefiedResult(
          ctx,
          ctx.name(0).getStart().getCharPositionInLine(),
          ctx.name(0).getStart().getCharPositionInLine() + name.length(),
          defined);

      return null;
    }

    final Symbol library =
        new Symbol(
            this.symbols.owner, name, "nothing", ImmutableSet.of(SymbolFlag.LIBRARY), ctx.name(0));

    if (ctx.initializer != null) {
      library.initializer = ctx.initializer.getText();
    }

    if (ctx.libraryRequirementsExpression() != null) {
      for (final NameContext requirementName : ctx.libraryRequirementsExpression().name()) {
        library.addExtends(requirementName.getText());
      }
    }

    this.symbols.define(library);

    final SymbolTable prevSymbols = this.symbols;
    this.symbols = library.children;

    this.visit(ctx.libraryBody());

    this.symbols = prevSymbols;

    return library;
  }

  @Override
  public Symbol visitScopeDeclaration(vrjParser.ScopeDeclarationContext ctx) {
    final String name = ctx.name(0).getText();
    final Symbol defined = this.symbols.resolve(name);

    if (!defined.equals(Symbol.NOTHING)) {
      this.addAlreadyDefiedResult(
          ctx,
          ctx.name(0).getStart().getCharPositionInLine(),
          ctx.name(0).getStart().getCharPositionInLine() + name.length(),
          defined);

      return null;
    }

    final Symbol scope =
        new Symbol(
            this.symbols.owner, name, "nothing", ImmutableSet.of(SymbolFlag.SCOPE), ctx.name(0));

    if (ctx.initializer != null) {
      scope.initializer = ctx.initializer.getText();
    }

    this.symbols.define(scope);

    final SymbolTable prevSymbols = this.symbols;
    this.symbols = scope.children;

    this.visit(ctx.scopeBody());

    this.symbols = prevSymbols;

    return scope;
  }

  @Override
  public Symbol visitStructDeclaration(vrjParser.StructDeclarationContext ctx) {
    final String name = ctx.name().getText();
    final Symbol defined = this.symbols.resolve(name);

    if (!defined.equals(Symbol.NOTHING)) {
      this.addAlreadyDefiedResult(
          ctx,
          ctx.name().getStart().getCharPositionInLine(),
          ctx.name().getStart().getCharPositionInLine() + name.length(),
          defined);

      return defined;
    }

    final Symbol struct =
        new Symbol(this.symbols.owner, name, name, ImmutableSet.of(SymbolFlag.STRUCT), ctx.name());

    final Symbol thistype =
        new Symbol(
            struct, "thistype", struct.name, ImmutableSet.of(SymbolFlag.VARIABLE), ctx.name());

    struct.children.define(thistype);

    if (ctx.extendsFromExpression() != null) {
      struct.addExtends(ctx.extendsFromExpression().getText());
    }

    this.symbols.define(struct);

    final SymbolTable prevSymbols = this.symbols;
    this.symbols = struct.children;

    this.visit(ctx.structBody());

    this.symbols = prevSymbols;

    return struct;
  }

  @Override
  public Symbol visitNonArrayVariableDeclaration(vrjParser.NonArrayVariableDeclarationContext ctx) {
    final String name = ctx.name().getText();
    final String type = ctx.type().getText();
    final Symbol defined = this.symbols.resolve(name);

    if (!defined.equals(Symbol.NOTHING)) {
      this.addAlreadyDefiedResult(
          ctx,
          ctx.name().getStart().getCharPositionInLine(),
          ctx.name().getStart().getCharPositionInLine() + name.length(),
          defined);

      return null;
    }

    final Symbol variable =
        new Symbol(
            this.symbols.owner, name, type, ImmutableSet.of(SymbolFlag.VARIABLE), ctx.name());

    this.symbols.define(variable);

    return variable;
  }

  @Override
  public Symbol visitArrayVariableDeclaration(vrjParser.ArrayVariableDeclarationContext ctx) {
    final String name = ctx.name().getText();
    final String type = ctx.type().getText();
    final Symbol defined = this.symbols.resolve(name);

    if (!defined.equals(Symbol.NOTHING)) {
      this.addAlreadyDefiedResult(
          ctx,
          ctx.name().getStart().getCharPositionInLine(),
          ctx.name().getStart().getCharPositionInLine() + name.length(),
          defined);

      return null;
    }

    final Symbol variable =
        new Symbol(
            this.symbols.owner,
            name,
            type,
            ImmutableSet.of(SymbolFlag.VARIABLE, SymbolFlag.ARRAY),
            ctx.name());

    this.symbols.define(variable);

    return variable;
  }

  @Override
  public Symbol visitPropertyDeclaration(vrjParser.PropertyDeclarationContext ctx) {
    final Symbol property = this.visit(ctx.variableDeclaration());

    if (property != null) {
      property.addFlag(SymbolFlag.GLOBAL);
      property.addFlag(SymbolFlag.PROPERTY);
    }

    return property;
  }

  @Override
  public Symbol visitFunctionSignature(vrjParser.FunctionSignatureContext ctx) {
    final String name = ctx.name().getText();
    final String type = ctx.type().getText();
    final Symbol defined = this.symbols.resolve(name);

    if (!defined.equals(Symbol.NOTHING)) {
      this.addAlreadyDefiedResult(
          ctx,
          ctx.name().getStart().getCharPositionInLine(),
          ctx.name().getStart().getCharPositionInLine() + name.length(),
          defined);

      return null;
    }

    final Symbol function =
        new Symbol(
            this.symbols.owner, name, type, ImmutableSet.of(SymbolFlag.FUNCTION), ctx.name());

    this.symbols.define(function);

    final SymbolTable prevSymbols = this.symbols;
    this.symbols = function.children;

    this.visit(ctx.paramList());

    this.symbols = prevSymbols;

    return function;
  }

  @Override
  public Symbol visitMethodDeclaration(vrjParser.MethodDeclarationContext ctx) {
    final Symbol method = this.visit(ctx.functionSignature());

    if (method == null) {
      return null;
    }

    if (ctx.sstatic == null) {
      final Symbol _this =
          new Symbol(
              method,
              "this",
              this.symbols.owner.type,
              ImmutableSet.of(SymbolFlag.VARIABLE),
              ctx.functionSignature());

      if (method.children.resolve("this").equals(Symbol.NOTHING)) {
        method.addParam(_this);
      } else {
        this.addAlreadyDefiedResult(
            ctx,
            ctx.functionSignature().getStart().getCharPositionInLine(),
            ctx.functionSignature().getStart().getCharPositionInLine() + method.name.length(),
            method.children.resolve("this"));
      }
    } else {
      method.addFlag(SymbolFlag.STATIC);
    }

    if ("onInit".equals(method.name)) {
      this.symbols.owner.initializer = method.name;
    }

    final SymbolTable prevSymbols = this.symbols;
    this.symbols = method.children;

    this.visit(ctx.statements());

    this.symbols = prevSymbols;

    return method;
  }

  @Override
  public Symbol visitTypeDeclaration(vrjParser.TypeDeclarationContext ctx) {
    final String name = ctx.typeName.getText();
    final Symbol defined = this.symbols.resolve(name);

    if (!defined.equals(Symbol.NOTHING)) {
      this.addAlreadyDefiedResult(
          ctx,
          ctx.typeName.getStart().getCharPositionInLine(),
          ctx.typeName.getStart().getCharPositionInLine() + name.length(),
          defined);

      return null;
    }

    final Symbol type =
        new Symbol(this.symbols.owner, name, name, ImmutableSet.of(SymbolFlag.TYPE), ctx.typeName);

    if (ctx.typeExtends != null) {
      final String _extends = ctx.typeExtends.getText();
      type.addExtends(_extends);
    }

    this.symbols.define(type);

    return type;
  }

  @Override
  public Symbol visitParam(vrjParser.ParamContext ctx) {
    final String name = ctx.name().getText();
    final String type = ctx.type().getText();
    final Symbol defined = this.symbols.resolve(name);

    if (!defined.equals(Symbol.NOTHING)) {
      this.addAlreadyDefiedResult(
          ctx,
          ctx.name().getStart().getCharPositionInLine(),
          ctx.name().getStart().getCharPositionInLine() + name.length(),
          defined);

      return null;
    }

    final Symbol param =
        new Symbol(this.symbols.owner, name, type, ImmutableSet.of(SymbolFlag.VARIABLE), ctx);

    this.symbols.owner.addParam(param);

    return param;
  }

  @Override
  public Symbol visitNativeDeclaration(vrjParser.NativeDeclarationContext ctx) {
    return this.visit(ctx.functionSignature());
  }

  @Override
  public Symbol visitFunctionDeclaration(vrjParser.FunctionDeclarationContext ctx) {
    final Symbol function = this.visit(ctx.functionSignature());

    if (function == null) {
      return null;
    }

    final SymbolTable prevSymbols = this.symbols;
    this.symbols = function.children;

    this.visit(ctx.statements());

    this.symbols = prevSymbols;

    return function;
  }

  @Override
  public Symbol visitGlobalVariableDeclaration(vrjParser.GlobalVariableDeclarationContext ctx) {
    final Symbol variable = this.visit(ctx.variableDeclaration());

    if (variable != null) {
      variable.addFlag(SymbolFlag.GLOBAL);
    }

    return variable;
  }

  @Override
  public Symbol visitLocalVariableDeclaration(vrjParser.LocalVariableDeclarationContext ctx) {
    final Symbol variable = this.visit(ctx.variableDeclaration());

    if (variable != null) {
      variable.addFlag(SymbolFlag.LOCAL);
    }

    return variable;
  }
}
