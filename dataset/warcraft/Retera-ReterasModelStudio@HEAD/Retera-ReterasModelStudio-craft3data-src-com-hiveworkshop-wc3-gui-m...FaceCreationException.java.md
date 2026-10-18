---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/FaceCreationException.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit;

public class FaceCreationException extends RuntimeException {

	public FaceCreationException() {
		super();
	}

	public FaceCreationException(final String message, final Throwable cause, final boolean enableSuppression,
			final boolean writableStackTrace) {
		super(message, cause, enableSuppression, writableStackTrace);
	}

	public FaceCreationException(final String message, final Throwable cause) {
		super(message, cause);
	}

	public FaceCreationException(final String message) {
		super(message);
	}

	public FaceCreationException(final Throwable cause) {
		super(cause);
	}

}
