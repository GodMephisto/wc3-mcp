---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/IndexedSequence.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x;

import com.etheller.warsmash.viewer5.handlers.mdx.Sequence;

public class IndexedSequence {
	public final Sequence sequence;
	public final int index;

	public IndexedSequence(final Sequence sequence, final int index) {
		this.sequence = sequence;
		this.index = index;
	}
}
