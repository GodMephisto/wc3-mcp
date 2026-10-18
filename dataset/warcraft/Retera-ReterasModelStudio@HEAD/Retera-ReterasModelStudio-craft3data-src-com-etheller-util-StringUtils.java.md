---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/etheller/util/StringUtils.java"
ref: HEAD
---

package com.etheller.util;

public final class StringUtils {
	public static int countOccurrences(final String haystack, final char needle) {
		int count = 0;
		for (int i = 0; i < haystack.length(); i++) {
			if (haystack.charAt(i) == needle) {
				count++;
			}
		}
		return count;
	}

	private StringUtils() {
	}
}
