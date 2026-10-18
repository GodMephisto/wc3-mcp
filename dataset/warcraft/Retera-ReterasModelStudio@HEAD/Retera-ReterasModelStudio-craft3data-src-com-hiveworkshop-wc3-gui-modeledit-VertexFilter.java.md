---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/VertexFilter.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit;

import com.hiveworkshop.wc3.mdl.Vertex;

public interface VertexFilter<TYPE extends Vertex> {
	boolean isAccepted(TYPE vertex);

	public VertexFilter<Vertex> IDENTITY = new VertexFilter<Vertex>() {
		@Override
		public boolean isAccepted(final Vertex vertex) {
			return true;
		}
	};
}
