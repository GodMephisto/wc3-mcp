---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS2"
source_path: "src/main/java/ruke/vrj/translator/FunctionDefinition.java"
ref: HEAD
---

package ruke.vrj.translator;

import com.google.common.collect.ImmutableList;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import ruke.vrj.Symbol;
import ruke.vrj.SymbolFlag;
import ruke.vrj.translator.ComparisonExpression.Operator;
import ruke.vrj.util.NameGenerator;

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
public class FunctionDefinition implements Expression {

  public final Symbol function;
  public final ImmutableList<Expression> statements;

  public FunctionDefinition(final Symbol function, final Collection<Expression> statements) {
    this.function = function;
    this.statements = ImmutableList.copyOf(statements);
  }

  private final Expression buildDynamicDispatchBlock(Collection<Expression> args) {
    final List<Expression> statements = new ArrayList<>(this.function.implementations.size());
    final boolean needsReturn = !this.function.type.equals("nothing");
    Expression condition;

    /*
     * if false then
     *  empty because im lazy as f*ck
     * else if vtype == IMPLEMENTATION_A_ID then
     *  return/call implementationA(args)
     * else if vtype == IMPLEMENTATION_B_ID then
     *  return/call implementationB(args)
     * else if ...
     * endif
     */
    for (final Symbol implementation : this.function.implementations) {
      condition =
          new ComparisonExpression(
              new RawExpression("vtype"),
              Operator.EQUAL,
              new RawExpression(implementation.parent.id));

      statements.add(
          new ElseIfStatement(
              condition,
              ImmutableList.of(
                  needsReturn
                      ? new ReturnStatement(new FunctionExpression(implementation, args))
                      : new FunctionStatement(new FunctionExpression(implementation, args)))));
    }

    return new IfStatement(new RawExpression("false"), statements);
  }

  @Override
  public final String toString() {
    final Symbol type = this.function.children.resolve(this.function.type);
    final boolean isStruct = type.flags.contains(SymbolFlag.STRUCT);
    final String translatedType = isStruct ? "integer" : this.function.type;
    final Collection<String> params = new ArrayList<>(this.function.params.size());
    final Collection<String> body = new ArrayList<>(this.statements.size());
    final Collection<Expression> args = new ArrayList<>();

    if (!this.function.implementations.isEmpty()) {
      for (final Symbol arg : this.function.params) {
        args.add(new VariableExpression(arg, null));
      }

      params.add("integer vtype");
      body.add(this.buildDynamicDispatchBlock(args).toString());
    }

    if (this.function.params.isEmpty()) {
      params.add("nothing");
    } else {
      for (final Symbol param : this.function.params) {
        params.add(new VariableStatement(param, null).toString());
      }
    }

    for (final Expression statement : this.statements) {
      body.add(statement.toString());
    }

    return String.format(
        "function %s takes %s returns %s\n" + "%s\n" + "endfunction",
        NameGenerator.to(this.function),
        String.join(", ", params),
        translatedType,
        String.join("\n", body));
  }
}
