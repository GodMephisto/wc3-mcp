---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/GenericGroup.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

import java.util.ArrayList;
import java.util.List;

import com.badlogic.gdx.math.Matrix4;

public abstract class GenericGroup {
	public final List<Integer> objects;

	public abstract void render(MdxComplexInstance instance, Matrix4 mvp);

	public GenericGroup() {
		this.objects = new ArrayList<>(); // TODO IntArrayList
	}

}
