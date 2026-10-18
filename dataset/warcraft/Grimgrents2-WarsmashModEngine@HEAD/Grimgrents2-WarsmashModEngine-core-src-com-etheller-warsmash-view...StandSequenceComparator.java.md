---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/StandSequenceComparator.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x;

import java.util.Comparator;

public class StandSequenceComparator implements Comparator<IndexedSequence> {
	@Override
	public int compare(final IndexedSequence a, final IndexedSequence b) {
		return (int) Math.signum(b.sequence.getRarity() - a.sequence.getRarity());
	}

}
