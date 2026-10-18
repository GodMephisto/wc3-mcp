---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS2"
source_path: "src/main/java/ruke/vrj/Main.java"
ref: HEAD
---

package ruke.vrj;

import static java.util.concurrent.TimeUnit.MILLISECONDS;
import static java.util.concurrent.TimeUnit.NANOSECONDS;

import com.google.common.base.Charsets;
import com.google.common.io.Files;
import java.io.File;
import ruke.vrj.compiler.Compiler;

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
public class Main {

  /**
   * Starting point, lets do some magic.
   *
   * @param args Arguments
   */
  public static void main(String[] args) {
    final Compiler compiler = new Compiler();

    try {
      for (final String arg : args) {
        final String toCompile = Files.toString(new File(arg), Charsets.UTF_8);
        final String compiled = compiler.compile(toCompile);
        final long start = System.nanoTime();

        if (compiler.getResults().isEmpty()) {
          System.out.println(compiled);
        } else {
          System.out.println(compiler.getResults());
        }

        System.out.println(
            "Compiled in "
                + MILLISECONDS.convert(System.nanoTime() - start, NANOSECONDS)
                + " milliseconds");
      }
    } catch (Exception e) {
      e.printStackTrace();
    }
  }
}
