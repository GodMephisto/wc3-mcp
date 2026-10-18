---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/blizzard/casc/nio/HashMismatchException.java"
ref: HEAD
---

package com.hiveworkshop.blizzard.casc.nio;

import java.io.IOException;

public class HashMismatchException extends IOException {
	private static final long serialVersionUID = -7133950344327038673L;

	public HashMismatchException() {
	}

	public HashMismatchException(String message) {
		super(message);
	}

}
