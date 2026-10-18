---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/etheller/util/Pair.java"
ref: HEAD
---

package com.etheller.util;

public final class Pair<FIRST, SECOND> {
	private final FIRST first;
	private final SECOND second;

	public Pair(final FIRST first, final SECOND second) {
		this.first = first;
		this.second = second;
	}

	public FIRST getFirst() {
		return first;
	}

	public SECOND getSecond() {
		return second;
	}
}
