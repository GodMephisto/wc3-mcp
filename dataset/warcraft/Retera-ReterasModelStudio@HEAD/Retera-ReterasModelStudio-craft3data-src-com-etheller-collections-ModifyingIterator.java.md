---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/etheller/collections/ModifyingIterator.java"
ref: HEAD
---

package com.etheller.collections;

import java.util.Iterator;

public interface ModifyingIterator<TYPE> extends Iterator<TYPE> {
	void delete();
}
