---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/components/editors/FloatValuePanelListener.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.components.editors;

import com.hiveworkshop.wc3.mdl.AnimFlag;

public interface FloatValuePanelListener {
	void setStaticValue(float value);

	void setDynamicValue(AnimFlag animFlag);
}
