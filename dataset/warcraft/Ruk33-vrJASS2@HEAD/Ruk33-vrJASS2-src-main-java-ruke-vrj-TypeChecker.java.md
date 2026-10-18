---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS2"
source_path: "src/main/java/ruke/vrj/TypeChecker.java"
ref: HEAD
---

package ruke.vrj;

import java.util.ArrayList;

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
public class TypeChecker {

  /**
   * Check if symbol is valid number (integer or real).
   *
   * @param expression To check
   * @return True if valid number
   */
  public static boolean isValidNumber(final Symbol expression) {
    return "integer".equals(expression.type) || "real".equals(expression.type);
  }

  /**
   * Check if valid function call (args count match, types, etc.).
   *
   * @param function Function to check
   * @param args Arguments
   * @return True if valid function call
   */
  public static boolean isValidFunctionCall(final Symbol function, final ArrayList<Symbol> args) {
    if (function.params.size() != args.size()) {
      return false;
    }

    final int functionParamsCount = function.params.size();

    for (int i = 0; i < functionParamsCount; i++) {
      if (args.get(i).flags.contains(SymbolFlag.ARRAY)) {
        return false;
      }

      if (!TypeChecker.compatible(function.params.get(i), args.get(i))) {
        return false;
      }
    }

    return true;
  }

  /**
   * Check if symbol is a valid array index.
   *
   * @param index Which symbol to validate
   * @return True if valid array index
   */
  public static boolean isValidArrayIndex(final Symbol index) {
    if (!"integer".equals(index.type)) {
      return false;
    }

    return !index.flags.contains(SymbolFlag.ARRAY);
  }

  /**
   * Check if symbol is struct.
   *
   * @param struct To check for
   * @return True if struct
   */
  public static boolean isStruct(final Symbol struct) {
    return struct.flags.contains(SymbolFlag.STRUCT);
  }

  /**
   * Check if symbol is a library.
   *
   * @param library To check for
   * @return True if library
   */
  public static boolean isLibrary(final Symbol library) {
    return library.flags.contains(SymbolFlag.LIBRARY);
  }

  /**
   * Check if symbol is a valid initializer.
   * 1) Is a function/method
   * 2) Takes no params
   * 3) It is static if method
   *
   * @param initializer To check for
   * @return True if valid initializer
   */
  public static boolean isValidInitializer(final Symbol initializer) {
    if (!initializer.flags.contains(SymbolFlag.FUNCTION)) {
      return false;
    }

    if (!initializer.params.isEmpty()) {
      return false;
    }

    if (initializer.parent != null) {
      final boolean parentIsStruct = TypeChecker.isStruct(initializer.parent);
      final boolean initializerNotStatic = !initializer.flags.contains(SymbolFlag.STATIC);

      if (parentIsStruct && initializerNotStatic) {
        return false;
      }
    }

    return true;
  }

  /**
   * Check if symbols are compatible.
   *
   * @param symbolA Symbol a
   * @param symbolB Symbol b
   * @return True if compatible
   */
  public static boolean compatible(final Symbol symbolA, final Symbol symbolB) {
    final Symbol typeA = symbolA.getTypes().get(0);

    for (final Symbol typeB : symbolB.getTypes()) {
      if (typeA.equals(typeB)) {
        return true;
      }
    }

    return false;
  }
}
