---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS2"
source_path: "src/main/java/ruke/vrj/translator/VariableStatement.java"
ref: HEAD
---

package ruke.vrj.translator;

import ruke.vrj.Symbol;
import ruke.vrj.SymbolFlag;
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
public class VariableStatement implements Expression {

  public final Symbol variable;
  public final Expression value;

  public VariableStatement(final Symbol variable, final Expression value) {
    this.variable = variable;
    this.value = value;
  }

  @Override
  public final String toString() {
    final Symbol type = this.variable.children.resolve(this.variable.type);
    final boolean isStruct = type.flags.contains(SymbolFlag.STRUCT);
    final String translatedType = isStruct ? "integer" : this.variable.type;

    String declaration = translatedType + " ";

    if (this.variable.flags.contains(SymbolFlag.LOCAL)) {
      declaration = "local " + declaration;
    }

    final boolean isProperty = this.variable.flags.contains(SymbolFlag.PROPERTY);
    final boolean notStatic = !this.variable.flags.contains(SymbolFlag.STATIC);

    if (this.variable.flags.contains(SymbolFlag.ARRAY) || (isProperty && notStatic)) {
      declaration += "array ";
    }

    declaration += NameGenerator.to(this.variable);

    if (this.value != null) {
      declaration += " = " + this.value.toString();
    }

    return declaration;
  }
}
