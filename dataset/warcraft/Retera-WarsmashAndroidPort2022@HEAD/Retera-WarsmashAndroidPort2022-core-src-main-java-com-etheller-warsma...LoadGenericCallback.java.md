---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/common/LoadGenericCallback.java"
ref: HEAD
---

package com.etheller.warsmash.common;

import java.io.InputStream;

public interface LoadGenericCallback {
	Object call(InputStream data); // TODO typing
}
