---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/viewer5/gl/ANGLEInstancedArrays.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.gl;

/**
 * TODO what is this?
 */
public interface ANGLEInstancedArrays {

	void glVertexAttribDivisorANGLE(int index, int divisor);

	void glDrawArraysInstancedANGLE(int mode, int first, int count, int instanceCount);

	void glDrawElementsInstancedANGLE(int mode, int count, int type, int indicesOffset, int instanceCount);
}
