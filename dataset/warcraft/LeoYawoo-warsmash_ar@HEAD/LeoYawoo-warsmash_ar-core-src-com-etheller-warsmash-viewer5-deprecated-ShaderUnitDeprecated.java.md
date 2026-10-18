---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/deprecated/ShaderUnitDeprecated.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.deprecated;

import com.badlogic.gdx.graphics.GL20;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.IntBuffer;

public class ShaderUnitDeprecated {

    public final boolean ok;

    public ShaderUnitDeprecated(final GL20 gl, final String src, final int type) {
        final int id = gl.glCreateShader(type);
        this.ok = false;

        gl.glShaderSource(id, src);
        gl.glCompileShader(id);

        final IntBuffer success = ByteBuffer.allocateDirect(8).order(ByteOrder.nativeOrder()).asIntBuffer();
        gl.glGetShaderiv(id, GL20.GL_COMPILE_STATUS, success);
        throw new UnsupportedOperationException("Not yet implemented, probably using library instead");
    }

}
