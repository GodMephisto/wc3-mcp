---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/etheller/util/FallibleCallback.java"
ref: HEAD
---

package com.etheller.util;

public interface FallibleCallback<TYPE> {
	void success(TYPE type);

	void failure(String message);
}
