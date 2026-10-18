---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/newstuff/listener/ClonedNodeNamePicker.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.newstuff.listener;

import java.util.Collection;
import java.util.Map;

import com.hiveworkshop.wc3.mdl.IdObject;

public interface ClonedNodeNamePicker {
	Map<IdObject, String> pickNames(Collection<IdObject> clonedNodes);
}
