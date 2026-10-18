---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/TextureAnimation.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

import com.etheller.warsmash.util.RenderMathUtils;
import com.hiveworkshop.rms.parsers.mdlx.AnimationMap;
import com.hiveworkshop.rms.parsers.mdlx.MdlxTextureAnimation;

public class TextureAnimation extends AnimatedObject {

    public TextureAnimation(final MdxModel model, final MdlxTextureAnimation textureAnimation) {
        super(model, textureAnimation);

        this.addVariants(AnimationMap.KTAT.getWar3id(), "translation");
        this.addVariants(AnimationMap.KTAR.getWar3id(), "rotation");
        this.addVariants(AnimationMap.KTAS.getWar3id(), "scale");
    }

    public void getTranslation(final float[] out, final int sequence, final int frame, final int counter) {
        this.getVectorValue(out, AnimationMap.KTAT.getWar3id(), sequence, frame, counter,
                RenderMathUtils.FLOAT_VEC3_ZERO);
    }

    public void getRotation(final float[] out, final int sequence, final int frame, final int counter) {
        this.getQuatValue(out, AnimationMap.KTAR.getWar3id(), sequence, frame, counter,
                RenderMathUtils.FLOAT_QUAT_DEFAULT);
    }

    public void getScale(final float[] out, final int sequence, final int frame, final int counter) {
        this.getVectorValue(out, AnimationMap.KTAS.getWar3id(), sequence, frame, counter,
                RenderMathUtils.FLOAT_VEC3_ONE);
    }

    public boolean isTranslationVariant(final int sequence) {
        return this.isVariant(AnimationMap.KTAT.getWar3id(), sequence);
    }

    public boolean isRotationVariant(final int sequence) {
        return this.isVariant(AnimationMap.KTAR.getWar3id(), sequence);
    }

    public boolean isScaleVariant(final int sequence) {
        return this.isVariant(AnimationMap.KTAS.getWar3id(), sequence);
    }

}
