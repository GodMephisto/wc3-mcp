---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/blizzard/casc/nio/MalformedCASCStructureException.java"
ref: HEAD
---

package com.hiveworkshop.blizzard.casc.nio;

import java.io.IOException;

public class MalformedCASCStructureException extends IOException {
	private static final long serialVersionUID = -5323382445554597608L;

	public MalformedCASCStructureException(String message) {
		super(message);
	}

	public MalformedCASCStructureException(String message, Throwable cause) {
		super(message, cause);
	}
}
