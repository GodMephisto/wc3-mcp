---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS2"
source_path: "src/main/java/ruke/vrj/translator/FunctionExpression.java"
ref: HEAD
---

package ruke.vrj.translator;

import com.google.common.collect.ImmutableList;
import java.util.ArrayList;
import java.util.Collection;
import ruke.vrj.Symbol;
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
public class FunctionExpression implements Expression, ChainableExpression {

  public final Symbol function;
  public final ImmutableList<Expression> args;

  public FunctionExpression(final Symbol function, final Collection<Expression> args) {
    this.function = function;
    this.args = args == null ? ImmutableList.of() : ImmutableList.copyOf(args);
  }

  @Override
  public final Symbol getSymbol() {
    return this.function;
  }

  @Override
  public final String toString() {
    final Collection<String> translatedArgs = new ArrayList<>(this.function.params.size());

    for (final Expression arg : this.args) {
      translatedArgs.add(arg.toString());
    }

    return String.format(
        "%s(%s)", NameGenerator.to(this.function), String.join(", ", translatedArgs));
  }
}
