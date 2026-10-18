---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/gl/WebGL.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.gl;

import com.badlogic.gdx.graphics.GL20;
import com.badlogic.gdx.graphics.Pixmap;
import com.badlogic.gdx.graphics.glutils.ShaderProgram;
import com.etheller.warsmash.viewer5.Texture;
import com.etheller.warsmash.viewer5.deprecated.ShaderUnitDeprecated;

import java.util.HashMap;
import java.util.Map;

/**
 * This needs a rename. Just a ripoff of ghostwolf's wrapper utility class, it's
 * a utility, not a webgl
 */
public class WebGL {
    public final com.badlogic.gdx.graphics.Texture emptyTexture;
    public final GL20 gl;
    public final Map<Integer, ShaderUnitDeprecated> shaderUnits;
    public final Map<Integer, ShaderProgram> shaderPrograms;
    public ShaderProgram currentShaderProgram;
    public final String floatPrecision;
    public final ANGLEInstancedArrays instancedArrays;

    public WebGL(final GL20 gl) {
        gl.glDepthFunc(GL20.GL_LEQUAL);
        gl.glEnable(GL20.GL_DEPTH_TEST);

        // TODO here ghostwolf throws exceptions for unsupported versions of opengl

        this.gl = gl;

        this.shaderUnits = new HashMap<>();

        this.shaderPrograms = new HashMap<>();

        this.currentShaderProgram = null;
        this.floatPrecision = "precision mediump float;\n";

        final Pixmap imageData = new Pixmap(2, 2, Pixmap.Format.RGBA8888);
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 2; j++) {
                imageData.drawPixel(i, j, 0x000000FF);
            }
        }
        this.emptyTexture = new com.badlogic.gdx.graphics.Texture(imageData);
        this.instancedArrays = Extensions.angleInstancedArrays;
    }

    public ShaderProgram createShaderProgram(String vertexSrc, String fragmentSrc) {
        vertexSrc = vertexSrc.replace("mediump", "");
        fragmentSrc = fragmentSrc.replace("mediump", "");
        final Map<Integer, ShaderProgram> shaderPrograms = this.shaderPrograms;

        final int hash = stringHash(vertexSrc + fragmentSrc);
        ShaderProgram.pedantic = false;
        if (!shaderPrograms.containsKey(hash)) {
            shaderPrograms.put(hash, new ShaderProgram(vertexSrc, fragmentSrc));
        }

        final ShaderProgram shaderProgram = shaderPrograms.get(hash);

        if (shaderProgram.isCompiled()) {
            return shaderProgram;
        } else {
            System.err.println(shaderProgram.getLog());
            throw new IllegalStateException("Bad shader");
        }
    }

    public void enableVertexAttribs(final int start, final int end) {
        final GL20 gl = this.gl;

        for (int i = start; i < end; i++) {
            gl.glEnableVertexAttribArray(i);
        }
    }

    public void disableVertexAttribs(final int start, final int end) {
        final GL20 gl = this.gl;

        for (int i = start; i < end; i++) {
            gl.glDisableVertexAttribArray(i);
        }
    }

    public void useShaderProgram(final ShaderProgram shaderProgram) {
        final ShaderProgram currentShaderProgram = this.currentShaderProgram;

        if ((shaderProgram != null) && shaderProgram.isCompiled() && (shaderProgram != currentShaderProgram)) {
            int oldAttribs = 0;
            final int newAttribs = shaderProgram.getAttributes().length;

            if (currentShaderProgram != null) {
                oldAttribs = currentShaderProgram.getAttributes().length;
            }

            shaderProgram.begin();

            if (newAttribs > oldAttribs) {
                this.enableVertexAttribs(oldAttribs, newAttribs);
            } else if (newAttribs < oldAttribs) {
                this.disableVertexAttribs(newAttribs, oldAttribs);
            }

            this.currentShaderProgram = shaderProgram;
        } else if (shaderProgram == null) {
            int oldAttribs = 0;
            final int newAttribs = 0;

            if (currentShaderProgram != null) {
                oldAttribs = currentShaderProgram.getAttributes().length;
                currentShaderProgram.end();
            }

            if (newAttribs < oldAttribs) {
                this.disableVertexAttribs(newAttribs, oldAttribs);
            }

            this.currentShaderProgram = null;
        }
    }

    public void bindTexture(final Texture texture, final int unit) {
        final GL20 gl = this.gl;

        gl.glActiveTexture(GL20.GL_TEXTURE0 + unit);

        if (texture != null /* && texture.ok */) {
            texture.internalBind();
        } else {
            this.emptyTexture.bind();
        }
    }

    private int stringHash(final String src) {
        return src.hashCode();
    }
}
