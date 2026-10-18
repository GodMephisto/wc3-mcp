---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/util/QuadtreeIntersector.java"
ref: HEAD
---

package com.etheller.warsmash.util;

public interface QuadtreeIntersector<T> {
	/**
	 * Handles what to do when the intersector finds an intersecting object,
	 * returning true if we should stop the intersection test and process no more
	 * objects.
	 *
	 * @param intersectingObject
	 * @return
	 */
	boolean onIntersect(T intersectingObject);
}
