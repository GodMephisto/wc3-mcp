---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS2"
source_path: "src/main/java/ruke/vrj/util/NameGenerator.java"
ref: HEAD
---

package ruke.vrj.util;

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
public class NameGenerator {

  /**
   * Generate name for symbol taking in consideration its parents.
   *
   * @param symbol Symbol to generate a name for
   * @return Generated name in the format parentName_symbolName
   */
  public static String to(final Symbol symbol) {
    final Symbol parent = symbol.parent;

    if (parent != null) {
      final boolean mustUsePrefix =
          parent.flags.contains(SymbolFlag.STRUCT)
              || parent.flags.contains(SymbolFlag.LIBRARY)
              || parent.flags.contains(SymbolFlag.SCOPE);

      if (mustUsePrefix) {
        return String.format("%s_%s", NameGenerator.to(parent), symbol.name);
      }
    }

    return symbol.name;
  }
}
