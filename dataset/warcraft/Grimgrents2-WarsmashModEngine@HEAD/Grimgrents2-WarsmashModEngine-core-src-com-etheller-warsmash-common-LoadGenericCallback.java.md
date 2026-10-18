---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/common/LoadGenericCallback.java"
ref: HEAD
---

package com.etheller.warsmash.common;

import java.io.InputStream;

public interface LoadGenericCallback {
	Object call(InputStream data); // TODO typing
}
