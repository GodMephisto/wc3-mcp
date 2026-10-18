---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/adapters/mdx/modelAnimationRuntime.ts"
ref: HEAD
---

import { VertexBuffer } from '@babylonjs/core/Buffers/buffer';
import type { BaseTexture } from '@babylonjs/core/Materials/Textures/baseTexture';
import { StandardMaterial } from '@babylonjs/core/Materials/standardMaterial';
import { Texture } from '@babylonjs/core/Materials/Textures/texture';
import type { Mesh } from '@babylonjs/core/Meshes/mesh';
import type { Scene } from '@babylonjs/core/scene';
import type GeosetAnimation from '../../parsers/mdlx/geosetanimation';
import type Layer from '../../parsers/mdlx/layer';
import type Model from '../../parsers/mdlx/model';
import type TextureAnimation from '../../parsers/mdlx/textureanimation';
import {
    findAnimation,
    sampleScalarAnimation,
    sampleVectorAnimation,
    type AnimationSampleClock,
} from './animationSampler';
import type { BabylonMdxModel } from './modelAdapter';

interface GeosetBinding {
    source: GeosetAnimation;
    mesh: Mesh;
    colors: Float32Array;
}

interface LayerBinding {
    source: Layer;
    material: StandardMaterial;
    textureAnimation?: TextureAnimation;
    textures: Map<number, BaseTexture>;
    textureCopies: Map<BaseTexture, BaseTexture>;
}

export interface ModelAnimationRuntimeOptions {
    loadTexture?: (textureId: number) => Promise<BaseTexture | null>;
}

type NullableObserver = ReturnType<Scene['onBeforeRenderObservable']['add']> | null;

const VISIBILITY_EPSILON = 1e-6;

export default class ModelAnimationRuntime {
    private readonly geosets: GeosetBinding[] = [];
    private readonly layers: LayerBinding[] = [];
    private observer: NullableObserver = null;
    private globalTimeMs = 0;
    private disposed = false;

    constructor(
        private readonly scene: Scene,
        private readonly rootNode: BabylonMdxModel,
        private readonly model: Model,
        private readonly options: ModelAnimationRuntimeOptions = {},
    ) {}

    async initialize(): Promise<void> {
        for (const source of this.model.geosetAnimations) {
            const mesh = this.rootNode.meshes[source.geosetId];
            if (!mesh) continue;
            this.geosets.push({
                source,
                mesh,
                colors: new Float32Array(mesh.getTotalVertices() * 4),
            });
        }

        for (let materialIndex = 0; materialIndex < this.model.materials.length; materialIndex++) {
            const sourceMaterial = this.model.materials[materialIndex];
            const runtimeLayers = this.rootNode.materialLayers?.[materialIndex] ?? [];
            for (let layerIndex = 0; layerIndex < sourceMaterial.layers.length; layerIndex++) {
                const source = sourceMaterial.layers[layerIndex];
                const material = runtimeLayers[layerIndex];
                if (!material) continue;
                this.layers.push({
                    source,
                    material,
                    textureAnimation: this.model.textureAnimations[source.textureAnimationId],
                    textures: new Map(),
                    textureCopies: new Map(),
                });
            }
        }

        if ((this.geosets.length || this.layers.length) && !this.observer) {
            this.observer = this.scene.onBeforeRenderObservable.add(() => this.update());
            this.update(0);
        }

        await Promise.allSettled(this.layers.map((binding) => this.loadAnimatedTextures(binding)));
        if (!this.disposed) this.update(0);
    }

    dispose(): void {
        this.disposed = true;
        if (this.observer) {
            this.scene.onBeforeRenderObservable.remove(this.observer);
            this.observer = null;
        }
        this.geosets.length = 0;
        for (const binding of this.layers) {
            for (const texture of binding.textureCopies.values()) texture.dispose();
            binding.textureCopies.clear();
        }
        this.layers.length = 0;
    }

    private update(deltaMs = this.scene.getEngine().getDeltaTime()): void {
        this.globalTimeMs += Math.max(0, deltaMs);
        const clock = this.createClock();
        for (const binding of this.geosets) this.updateGeoset(binding, clock);
        for (const binding of this.layers) this.updateLayer(binding, clock);
    }

    private createClock(): AnimationSampleClock {
        const activeAnimation = this.rootNode.activeAnimation;
        const sequence = this.rootNode.currentSequence ?? null;
        const frame = activeAnimation && Number.isFinite(activeAnimation.masterFrame)
            ? activeAnimation.masterFrame
            : sequence?.interval[0] ?? 0;
        return {
            frame,
            sequence,
            globalTimeMs: this.globalTimeMs,
            globalSequences: this.model.globalSequences,
        };
    }

    private updateGeoset(binding: GeosetBinding, clock: AnimationSampleClock): void {
        const { source, mesh, colors } = binding;
        const alpha = clamp(sampleScalarAnimation(
            findAnimation(source.animations, 'KGAO'),
            source.alpha,
            clock,
        ), 0, 1);
        mesh.visibility = alpha;
        mesh.setEnabled(alpha > VISIBILITY_EPSILON);

        const usesColor = (source.flags & 0x2) !== 0
            || !!findAnimation(source.animations, 'KGAC');
        const color = usesColor
            ? sampleVectorAnimation(findAnimation(source.animations, 'KGAC'), source.color, clock)
            : new Float32Array([1, 1, 1]);
        for (let index = 0; index < colors.length; index += 4) {
            colors[index] = finiteOr(color[2], 1);
            colors[index + 1] = finiteOr(color[1], 1);
            colors[index + 2] = finiteOr(color[0], 1);
            colors[index + 3] = 1;
        }
        if (mesh.isVerticesDataPresent(VertexBuffer.ColorKind)) {
            mesh.updateVerticesData(VertexBuffer.ColorKind, colors);
        } else {
            mesh.setVerticesData(VertexBuffer.ColorKind, colors, true, 4);
        }
        mesh.hasVertexAlpha = true;
    }

    private updateLayer(binding: LayerBinding, clock: AnimationSampleClock): void {
        const { source, material } = binding;
        material.alpha = clamp(sampleScalarAnimation(
            findAnimation(source.animations, 'KMTA'),
            source.alpha,
            clock,
        ), 0, 1);

        const emissiveGain = Math.max(0, sampleScalarAnimation(
            findAnimation(source.animations, 'KMTE'),
            source.emissiveGain,
            clock,
        ));
        if (source.emissiveGain !== 1 || findAnimation(source.animations, 'KMTE')) {
            material.emissiveColor.set(emissiveGain, emissiveGain, emissiveGain);
        }

        const textureIdAnimation = findAnimation(source.animations, 'KMTF');
        if (textureIdAnimation) {
            const textureId = Math.max(0, Math.floor(sampleScalarAnimation(
                textureIdAnimation,
                source.textureId,
                clock,
            )));
            const texture = binding.textures.get(textureId);
            if (texture && material.diffuseTexture !== texture) {
                const previous = material.diffuseTexture;
                material.diffuseTexture = texture;
                if (material.emissiveTexture === previous) material.emissiveTexture = texture;
                if (material.opacityTexture === previous) material.opacityTexture = texture;
            }
        }

        if (binding.textureAnimation) {
            this.updateTextureAnimation(binding, binding.textureAnimation, clock);
        }
    }

    private async loadAnimatedTextures(binding: LayerBinding): Promise<void> {
        const animation = findAnimation(binding.source.animations, 'KMTF');
        if (!animation || !this.options.loadTexture) return;
        const textureIds = new Set<number>([binding.source.textureId]);
        for (const value of animation.values) {
            const textureId = value[0];
            if (Number.isInteger(textureId) && textureId >= 0 && textureId < this.model.textures.length) {
                textureIds.add(textureId);
            }
        }

        await Promise.all([...textureIds].map(async (textureId) => {
            const texture = await this.options.loadTexture!(textureId);
            if (texture && !this.disposed) {
                binding.textures.set(textureId, this.copyTexture(binding, texture));
            }
        }));
    }

    private updateTextureAnimation(
        binding: LayerBinding,
        animation: TextureAnimation,
        clock: AnimationSampleClock,
    ): void {
        const { material } = binding;
        this.isolateMaterialTextures(binding);
        const translation = sampleVectorAnimation(
            findAnimation(animation.animations, 'KTAT'),
            [0, 0, 0],
            clock,
        );
        const rotation = sampleVectorAnimation(
            findAnimation(animation.animations, 'KTAR'),
            [0, 0, 0, 1],
            clock,
        );
        const scaling = sampleVectorAnimation(
            findAnimation(animation.animations, 'KTAS'),
            [1, 1, 1],
            clock,
        );
        const angle = quaternionZAngle(rotation);
        const textures = new Set([
            material.diffuseTexture,
            material.opacityTexture,
            material.emissiveTexture,
        ]);
        for (const texture of textures) {
            if (!(texture instanceof Texture)) continue;
            texture.uOffset = finiteOr(translation[0], 0);
            texture.vOffset = finiteOr(translation[1], 0);
            texture.uScale = finiteOr(scaling[0], 1);
            texture.vScale = finiteOr(scaling[1], 1);
            texture.wAng = angle;
        }
    }

    private isolateMaterialTextures(binding: LayerBinding): void {
        const { material } = binding;
        const diffuse = material.diffuseTexture;
        const opacity = material.opacityTexture;
        const emissive = material.emissiveTexture;
        if (diffuse) material.diffuseTexture = this.copyTexture(binding, diffuse);
        if (opacity) material.opacityTexture = this.copyTexture(binding, opacity);
        if (emissive) material.emissiveTexture = this.copyTexture(binding, emissive);
    }

    private copyTexture(binding: LayerBinding, texture: BaseTexture): BaseTexture {
        if (!(texture instanceof Texture)) return texture;
        for (const copy of binding.textureCopies.values()) {
            if (copy === texture) return texture;
        }
        const existing = binding.textureCopies.get(texture);
        if (existing) return existing;
        const copy = texture.clone();
        if (!copy) return texture;
        copy.name = texture.name;
        binding.textureCopies.set(texture, copy);
        return copy;
    }
}

function quaternionZAngle(value: ArrayLike<number>): number {
    const x = finiteOr(value[0], 0);
    const y = finiteOr(value[1], 0);
    const z = finiteOr(value[2], 0);
    const w = finiteOr(value[3], 1);
    return Math.atan2(2 * (w * z + x * y), 1 - 2 * (y * y + z * z));
}

function finiteOr(value: number | undefined, fallback: number): number {
    return Number.isFinite(value) ? value! : fallback;
}

function clamp(value: number, minimum: number, maximum: number): number {
    return Math.min(maximum, Math.max(minimum, value));
}
