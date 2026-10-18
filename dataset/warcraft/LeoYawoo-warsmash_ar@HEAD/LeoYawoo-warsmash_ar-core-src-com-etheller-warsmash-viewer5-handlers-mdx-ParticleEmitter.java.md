---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/ParticleEmitter.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

public class ParticleEmitter extends MdxEmitter<MdxComplexInstance, ParticleEmitterObject, Particle> {

    private static final float[] emissionRateHeap = new float[1];

    public ParticleEmitter(final MdxComplexInstance instance, final ParticleEmitterObject emitterObject) {
        super(instance, emitterObject);
    }

    @Override
    protected void updateEmission(final float dt) {
        final MdxComplexInstance instance = this.instance;

        if (instance.allowParticleSpawn) {
            final ParticleEmitterObject emitterObject = this.emitterObject;

            emitterObject.getEmissionRate(emissionRateHeap, instance.sequence, instance.frame, instance.counter);

            this.currentEmission += emissionRateHeap[0] * dt;
        }
    }

    @Override
    protected void emit() {
        this.emitObject(0);
    }

    @Override
    protected Particle createObject() {
        return new Particle(this);
    }

}
