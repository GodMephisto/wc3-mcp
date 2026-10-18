---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/animedit/WrongModeException.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.animedit;

public class WrongModeException extends RuntimeException {

	public WrongModeException() {
		super();
	}

	public WrongModeException(final String message, final Throwable cause, final boolean enableSuppression,
			final boolean writableStackTrace) {
		super(message, cause, enableSuppression, writableStackTrace);
	}

	public WrongModeException(final String message, final Throwable cause) {
		super(message, cause);
	}

	public WrongModeException(final String message) {
		super(message);
	}

	public WrongModeException(final Throwable cause) {
		super(cause);
	}

}
