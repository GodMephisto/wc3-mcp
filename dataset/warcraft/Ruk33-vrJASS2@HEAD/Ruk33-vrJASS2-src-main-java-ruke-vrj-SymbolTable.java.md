---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS2"
source_path: "src/main/java/ruke/vrj/SymbolTable.java"
ref: HEAD
---

package ruke.vrj;

import com.google.common.base.Splitter;
import com.google.common.collect.ImmutableMap;
import java.util.List;

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
public class SymbolTable {

  public final SymbolTable parent;
  public final Symbol owner;
  private ImmutableMap<String, Symbol> symbols;

  /**
   * Create symbol table.
   *
   * @param parent Parent of the symbol table
   * @param owner Symbol owning symbol table (for example, an struct)
   */
  public SymbolTable(final SymbolTable parent, final Symbol owner) {
    this.parent = parent;
    this.owner = owner;
    this.symbols = ImmutableMap.of();

    if (owner != null) {
      this.define(owner);
    }
  }

  /**
   * Create symbol table without parent & owner.
   */
  public SymbolTable() {
    this(null, null);
  }

  /**
   * Copy only the symbols of a SymbolTable (ignore parent & owner).
   *
   * @param copy Which symbol table to copy
   * @return Copied symbol table
   */
  public static SymbolTable copyOf(final SymbolTable copy) {
    final SymbolTable result = new SymbolTable();
    result.symbols = ImmutableMap.copyOf(copy.symbols);
    return result;
  }

  public final void define(final String name, final Symbol symbol) {
    this.symbols =
        ImmutableMap.<String, Symbol>builder().putAll(this.symbols).put(name, symbol).build();
  }

  public final void define(final Symbol symbol) {
    this.define(symbol.name, symbol);
  }

  /**
   * Find symbol in symbol table.
   *
   * @param name Name to search for
   * @return Symbol (Symbol.NOTHING is returned if the symbol wasn't found)
   */
  public final Symbol resolve(final String name) {
    if (name.contains(".")) {
      // ( 1 ) some.chain.expression
      // ( 2 ) search "some" in the current symbol table
      // ( 3 ) make a copy of the symbol table of "some" so we can't search in parents
      // ( 4 ) repeat
      final List<String> names = Splitter.on(".").splitToList(name);
      final List<String> restChain = names.subList(1, names.size());
      final Symbol firstInChain = this.resolve(names.get(0));

      return SymbolTable.copyOf(firstInChain.children).resolve(String.join(".", restChain));
    }

    Symbol result = this.symbols.getOrDefault(name, Symbol.NOTHING);

    if (result.equals(Symbol.NOTHING) && this.parent != null) {
      result = this.parent.resolve(name);
    }

    return result;
  }
}
