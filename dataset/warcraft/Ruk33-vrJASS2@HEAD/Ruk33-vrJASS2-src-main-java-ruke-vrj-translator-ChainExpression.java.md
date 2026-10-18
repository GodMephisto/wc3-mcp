---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS2"
source_path: "src/main/java/ruke/vrj/translator/ChainExpression.java"
ref: HEAD
---

package ruke.vrj.translator;

import com.google.common.collect.ImmutableList;
import java.util.Collection;
import java.util.Stack;
import ruke.vrj.Symbol;
import ruke.vrj.SymbolFlag;

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
public class ChainExpression implements Expression, ChainableExpression {

  public final ImmutableList<ChainableExpression> expressions;

  public ChainExpression(final Collection<ChainableExpression> expressions) {
    this.expressions = ImmutableList.copyOf(expressions);
  }

  @Override
  public final Symbol getSymbol() {
    return this.expressions.get(this.expressions.size() - 1).getSymbol();
  }

  @Override
  public final String toString() {
    final ChainableExpression head = this.expressions.get(0);
    final Stack<Expression> args = new Stack<>();

    args.push(head);

    for (final ChainableExpression member :
        this.expressions.subList(1, this.expressions.size()).reverse()) {
      if (member.getSymbol().flags.contains(SymbolFlag.FUNCTION)) {
        args.add(new FunctionExpression(member.getSymbol(), args));
      } else {
        args.add(new VariableExpression(member.getSymbol(), args.peek()));
      }
    }

    return args.peek().toString();
  }
}
