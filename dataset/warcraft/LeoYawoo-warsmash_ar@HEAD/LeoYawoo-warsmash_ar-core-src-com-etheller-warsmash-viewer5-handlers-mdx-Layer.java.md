---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/Layer.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

import com.badlogic.gdx.graphics.GL20;
import com.badlogic.gdx.graphics.glutils.ShaderProgram;
import com.hiveworkshop.rms.parsers.mdlx.AnimationMap;
import com.hiveworkshop.rms.parsers.mdlx.MdlxLayer;

/**
 * An MDX layer.
 */
public class Layer extends AnimatedObject {
    public final int index;
    public final int priorityPlane;
    public final int filterMode;
    public final int textureId;
    public final int coordId;
    public final float alpha;
    public final int unshaded;
    public final int sphereEnvironmentMap;
    public final int twoSided;
    public final int unfogged;
    public final int noDepthTest;
    public final int noDepthSet;
    public final boolean depthMaskValue;
    public int blendSrc;
    public int blendDst;
    public final boolean blended;
    public TextureAnimation textureAnimation;

    public Layer(final MdxModel model, final MdlxLayer layer, final int layerId, final int priorityPlane) {
        super(model, layer);

        final MdlxLayer.FilterMode filterMode = layer.getFilterMode();
        final int textureAnimationId = layer.getTextureAnimationId();
        final GL20 gl = model.viewer.gl;

        this.index = layerId;
        this.priorityPlane = priorityPlane;
        this.filterMode = filterMode.ordinal();
        this.textureId = layer.getTextureId();
        this.coordId = (int) layer.getCoordId();
        this.alpha = layer.getAlpha();

        final int flags = layer.getFlags();

        this.unshaded = flags & 0x1;
        this.sphereEnvironmentMap = flags & 0x2;
        this.twoSided = flags & 0x10;
        this.unfogged = flags & 0x20;
        this.noDepthTest = flags & 0x40;
        this.noDepthSet = flags & 0x80;

        this.depthMaskValue = ((filterMode == MdlxLayer.FilterMode.NONE)
                || (filterMode == MdlxLayer.FilterMode.TRANSPARENT));

        this.blendSrc = 0;
        this.blendDst = 0;
        this.blended = (filterMode.ordinal() > 1);

        if (this.blended) {
            final int[] result = FilterMode.layerFilterMode(filterMode);
            this.blendSrc = result[0];
            this.blendDst = result[1];
        }

        if (textureAnimationId != -1) {
            final TextureAnimation textureAnimation = model.getTextureAnimations().get(textureAnimationId);

            if (textureAnimation != null) {
                this.textureAnimation = textureAnimation;
            }
        }

        this.addVariants(AnimationMap.KMTA.getWar3id(), "alpha");
        this.addVariants(AnimationMap.KMTF.getWar3id(), "textureId");
    }

    public void bind(final ShaderProgram shader) {
        final GL20 gl = this.model.viewer.gl;

        // gl.uniform1f(shader.uniforms.u_unshaded, this.unshaded);
        shader.setUniformf("u_filterMode", this.filterMode);

        if (this.blended) {
            gl.glEnable(GL20.GL_BLEND);
            gl.glBlendFunc(this.blendSrc, this.blendDst);
        } else {
            gl.glDisable(GL20.GL_BLEND);
        }

        if (this.twoSided != 0) {
            gl.glDisable(GL20.GL_CULL_FACE);
        } else {
            gl.glEnable(GL20.GL_CULL_FACE);
        }

        if (this.noDepthTest != 0) {
            gl.glDisable(GL20.GL_DEPTH_TEST);
        } else {
            gl.glEnable(GL20.GL_DEPTH_TEST);
        }

        if (this.noDepthSet != 0) {
            gl.glDepthMask(false);
        } else {
            gl.glDepthMask(this.depthMaskValue);
        }
    }

    public void bindBlended(final ShaderProgram shader) {
        final GL20 gl = this.model.viewer.gl;

        // gl.uniform1f(shader.uniforms.u_unshaded, this.unshaded);
        shader.setUniformf("u_filterMode", this.filterMode);

        gl.glEnable(GL20.GL_BLEND);
        if ((this.blendSrc == 0) && (this.blendDst == 0)) {
            gl.glBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
        } else {
            gl.glBlendFunc(this.blendSrc, this.blendDst);
        }

        if (this.twoSided != 0) {
            gl.glDisable(GL20.GL_CULL_FACE);
        } else {
            gl.glEnable(GL20.GL_CULL_FACE);
        }

        if (this.noDepthTest != 0) {
            gl.glDisable(GL20.GL_DEPTH_TEST);
        } else {
            gl.glEnable(GL20.GL_DEPTH_TEST);
        }

        if (this.noDepthSet != 0) {
            gl.glDepthMask(false);
        } else {
            gl.glDepthMask(this.depthMaskValue);
        }
    }

    public void getAlpha(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KMTA.getWar3id(), sequence, frame, counter, this.alpha);
    }

    public void getTextureId(final long[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KMTF.getWar3id(), sequence, frame, counter, this.textureId);
    }
}
