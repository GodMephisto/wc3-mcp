---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS2"
source_path: "src/main/java/ruke/vrj/compiler/Compiler.java"
ref: HEAD
---

package ruke.vrj.compiler;

import java.util.ArrayList;
import org.antlr.v4.runtime.ANTLRInputStream;
import org.antlr.v4.runtime.CommonTokenStream;
import org.antlr.v4.runtime.Lexer;
import org.antlr.v4.runtime.TokenStream;
import ruke.vrj.Symbol;
import ruke.vrj.antlr.vrjLexer;
import ruke.vrj.antlr.vrjParser;
import ruke.vrj.phase.Definition;
import ruke.vrj.phase.Translate;
import ruke.vrj.phase.TypeCheck;

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
public class Compiler {

  private ArrayList<Result> results = new ArrayList<>();

  /**
   * Get results (errors, warnings, etc.) of compilation.
   *
   * @return Results
   */
  public ArrayList<Result> getResults() {
    return this.results;
  }

  /**
   * Compile from ANTLRInputStream.
   *
   * @param input Input to compile from
   * @return Compiled code (empty if it contains errors)
   */
  public String compile(ANTLRInputStream input) {
    final Lexer lexer = new vrjLexer(input);
    final TokenStream tokenStream = new CommonTokenStream(lexer);
    final vrjParser parser = new vrjParser(tokenStream);

    final Symbol main = new Symbol();

    final Definition definition = new Definition(main.children);
    final TypeCheck typecheck = new TypeCheck(main.children);

    definition.visit(parser.init());
    parser.reset();

    typecheck.visit(parser.init());

    this.results = new ArrayList<>();

    this.results.addAll(definition.getResults());
    this.results.addAll(typecheck.getResults());

    if (this.results.isEmpty()) {
      parser.reset();

      final Translate translate = new Translate(main.children);

      return translate.visit(parser.init()).toString();
    }

    return "";
  }

  /**
   * Compile from string.
   *
   * @param code To compile
   * @return Compiled code (empty if it contains errors)
   */
  public String compile(String code) {
    return compile(new ANTLRInputStream(code + "\n"));
  }
}
