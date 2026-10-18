---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/ParticleEmitterObject.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

import com.etheller.warsmash.viewer5.handlers.EmitterObject;
import com.hiveworkshop.rms.parsers.mdlx.AnimationMap;
import com.hiveworkshop.rms.parsers.mdlx.MdlxParticleEmitter;

import java.util.Locale;

public class ParticleEmitterObject extends GenericObject implements EmitterObject {
    public final MdxModel internalModel;
    public final float speed;
    public final float latitude;
    public final float longitude;
    public final float lifeSpan;
    public final float gravity;
    public final float emissionRate;

    /**
     * No need to create instances of the internal model if it didn't load.
     * <p>
     * Such instances won't actually render, and who knows if the model will ever
     * load?
     */
    public final boolean ok;

    public ParticleEmitterObject(final MdxModel model, final MdlxParticleEmitter emitter, final int index) {
        super(model, emitter, index);

        this.internalModel = (MdxModel) model.viewer.load(
                emitter.getPath().replace("\\", "/").toLowerCase(Locale.US).replace(".mdl", ".mdx"), model.pathSolver,
                model.solverParams);
        this.speed = emitter.getSpeed();
        this.latitude = emitter.getLatitude();
        this.longitude = emitter.getLongitude();
        this.lifeSpan = emitter.getLifeSpan();
        this.gravity = emitter.getGravity();
        this.emissionRate = emitter.getEmissionRate();

        // Activate emitters based on this emitter object only when and if the internal
        // model loads successfully.
        // TODO async removed here
        this.ok = this.internalModel.ok;
    }

    public void getSpeed(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KPES.getWar3id(), sequence, frame, counter, this.speed);
    }

    public void getLatitude(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KPLT.getWar3id(), sequence, frame, counter, this.latitude);
    }

    public int getLongitude(final float[] out, final int sequence, final int frame, final int counter) {
        return this.getScalarValue(out, AnimationMap.KPLN.getWar3id(), sequence, frame, counter, this.longitude);
    }

    public void getLifeSpan(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KPEL.getWar3id(), sequence, frame, counter, this.lifeSpan);
    }

    public void getGravity(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KPEG.getWar3id(), sequence, frame, counter, this.gravity);
    }

    public void getEmissionRate(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KPEE.getWar3id(), sequence, frame, counter, this.emissionRate);
    }

    @Override
    public void getVisibility(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KPEV.getWar3id(), sequence, frame, counter, 1);
    }

    @Override
    public int getGeometryEmitterType() {
        throw new UnsupportedOperationException("ghostwolf doesnt have this in the JS");
    }

    @Override
    public boolean ok() {
        return this.ok;
    }
}
