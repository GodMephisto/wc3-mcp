---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/units/ObjectData.java"
ref: HEAD
---

package com.etheller.warsmash.units;

import java.util.Set;

public interface ObjectData {
	GameObject get(String id);

	void setValue(String id, String field, String value);

	Set<String> keySet();

	String getLocalizedString(String key);
}
