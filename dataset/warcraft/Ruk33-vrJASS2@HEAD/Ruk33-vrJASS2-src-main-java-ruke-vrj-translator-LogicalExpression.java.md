---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS2"
source_path: "src/main/java/ruke/vrj/translator/LogicalExpression.java"
ref: HEAD
---

package ruke.vrj.translator;

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
public class LogicalExpression implements Expression {

  public enum Operator {
    OR {
      @Override
      public final String toString() {
        return "or";
      }
    },
    AND {
      @Override
      public final String toString() {
        return "and";
      }
    },
  }

  public final Expression left;
  public final Expression right;
  public final Operator operator;

  /**
   * Create logical expression.
   *
   * @param left Left expression
   * @param operator Operator
   * @param right Right expression
   */
  public LogicalExpression(final Expression left, final Operator operator, final Expression right) {
    this.left = left;
    this.operator = operator;
    this.right = right;
  }

  @Override
  public final String toString() {
    return String.format(
        "%s %s %s", this.left.toString(), this.operator.toString(), this.right.toString());
  }
}
