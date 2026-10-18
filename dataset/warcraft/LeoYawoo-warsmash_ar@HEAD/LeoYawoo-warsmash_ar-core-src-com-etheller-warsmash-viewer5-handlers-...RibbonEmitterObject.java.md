---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/RibbonEmitterObject.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

import com.etheller.warsmash.viewer5.handlers.EmitterObject;
import com.hiveworkshop.rms.parsers.mdlx.AnimationMap;
import com.hiveworkshop.rms.parsers.mdlx.MdlxRibbonEmitter;

public class RibbonEmitterObject extends GenericObject implements EmitterObject {
    public final Layer layer;
    public final float heightAbove;
    public final float heightBelow;
    public final float alpha;
    public final float[] color;
    public final float lifeSpan;
    public final long textureSlot;
    public final long emissionRate;
    public final float gravity;
    public final long columns;
    public final long rows;


    public RibbonEmitterObject(final MdxModel model, final MdlxRibbonEmitter emitter, final int index) {
        super(model, emitter, index);

        this.layer = model.getMaterials().get(emitter.getMaterialId()).layers.get(0);
        this.heightAbove = emitter.getHeightAbove();
        this.heightBelow = emitter.getHeightBelow();
        this.alpha = emitter.getAlpha();
        this.color = emitter.getColor();
        this.lifeSpan = emitter.getLifeSpan();
        this.textureSlot = emitter.getTextureSlot();
        this.emissionRate = emitter.getEmissionRate();
        this.gravity = emitter.getGravity();
        this.columns = emitter.getColumns();
        this.rows = emitter.getRows();
    }

    public void getHeightBelow(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KRHB.getWar3id(), sequence, frame, counter, this.heightBelow);
    }

    public void getHeightAbove(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KRHA.getWar3id(), sequence, frame, counter, this.heightAbove);
    }

    public void getTextureSlot(final long[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KRTX.getWar3id(), sequence, frame, counter, 0);
    }

    public void getColor(final float[] out, final int sequence, final int frame, final int counter) {
        this.getVectorValue(out, AnimationMap.KRCO.getWar3id(), sequence, frame, counter, this.color);
    }

    public void getAlpha(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KRAL.getWar3id(), sequence, frame, counter, this.alpha);
    }

    @Override
    public void getVisibility(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KRVS.getWar3id(), sequence, frame, counter, 1f);
    }

    @Override
    public int getGeometryEmitterType() {
        return GeometryEmitterFuncs.EMITTER_RIBBON;
    }

    @Override
    public boolean ok() {
        return true;
    }
}
