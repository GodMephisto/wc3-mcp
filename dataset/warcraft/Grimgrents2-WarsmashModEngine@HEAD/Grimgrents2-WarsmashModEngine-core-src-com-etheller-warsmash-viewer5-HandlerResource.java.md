---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/HandlerResource.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5;

import com.etheller.warsmash.viewer5.handlers.ResourceHandler;

public abstract class HandlerResource<HANDLER extends ResourceHandler> extends Resource {
	public final HANDLER handler;

	public HandlerResource(final ModelViewer viewer, final String extension, final PathSolver pathSolver,
			final String fetchUrl, final HANDLER handler) {
		super(viewer, extension, pathSolver, fetchUrl);
		this.handler = handler;
	}

}
