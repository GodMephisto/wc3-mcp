---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS2"
source_path: "src/main/java/ruke/vrj/compiler/Result.java"
ref: HEAD
---

package ruke.vrj.compiler;

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
public class Result {

  public final String source;
  public final int line;
  public final int start;
  public final int end;
  public final String message;

  /**
   * Create compiler result.
   *
   * @param source Path to file
   * @param line Line
   * @param start Column start
   * @param end Column end
   * @param message Result message
   */
  public Result(
      final String source,
      final int line,
      final int start,
      final int end,
      final String message
  ) {
    this.source = source;
    this.line = line;
    this.start = start;
    this.end = end;
    this.message = message;
  }

  /**
   * Print result as string.
   *
   * @return Result
   */
  public final String toString() {
    final String result = String.format(
        "%s(%d,%d,%d): %s", this.source, this.line, this.start, this.end, this.message
    );

    return result.replaceAll("<unknown>", "");
  }
}
