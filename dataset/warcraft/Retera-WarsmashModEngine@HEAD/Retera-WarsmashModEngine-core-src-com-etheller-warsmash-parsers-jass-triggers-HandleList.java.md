---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/parsers/jass/triggers/HandleList.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.jass.triggers;

import java.util.ArrayList;

import com.etheller.interpreter.ast.util.CHandle;
import com.etheller.interpreter.ast.value.JassValue;

public class HandleList extends ArrayList<JassValue> implements CHandle {
	private final int handleId;

	public HandleList(final int handleId) {
		this.handleId = handleId;
	}

	@Override
	public int getHandleId() {
		return this.handleId;
	}

}
