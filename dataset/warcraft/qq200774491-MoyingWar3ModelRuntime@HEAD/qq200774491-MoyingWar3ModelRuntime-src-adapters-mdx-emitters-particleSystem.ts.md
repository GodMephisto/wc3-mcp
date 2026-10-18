---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/adapters/mdx/emitters/particleSystem.ts"
ref: HEAD
---

import { VertexBuffer } from '@babylonjs/core/Buffers/buffer';
import { Engine } from '@babylonjs/core/Engines/engine';
import { Color3 } from '@babylonjs/core/Maths/math.color';
import { Matrix, Vector3 } from '@babylonjs/core/Maths/math.vector';
import { ShaderMaterial } from '@babylonjs/core/Materials/shaderMaterial';
import { StandardMaterial } from '@babylonjs/core/Materials/standardMaterial';
import { Texture } from '@babylonjs/core/Materials/Textures/texture';
import { Mesh } from '@babylonjs/core/Meshes/mesh';
import { TransformNode } from '@babylonjs/core/Meshes/transformNode';
import { Scene } from '@babylonjs/core/scene';
import type Model from '../../../parsers/mdlx/model';
import ParticleEmitter2, {
    FilterMode as ParticleFilterMode,
    Flags as ParticleFlags,
    HeadOrTail,
} from '../../../parsers/mdlx/particleemitter2';
import RibbonEmitter from '../../../parsers/mdlx/ribbonemitter';
import replaceableIds from '../../../parsers/mdlx/replaceableids';
import type MdxTexture from '../../../parsers/mdlx/texture';
import type { War3ViewerWarning } from '../../../types';
import type { War3TextureLoader } from '../../textures/textureLoader';
import {
    findAnimation,
    findScalarKeyframe,
    getAnimationSampleFrame,
    sampleScalarAnimation,
    sampleVectorAnimation,
    type AnimationSampleClock,
} from '../animationSampler';
import type { BabylonMdxModel } from '../modelAdapter';
import type { SkeletonBuildResult } from '../skeletonBuilder';

export interface ParticleSystemOptions {
    scene: Scene;
    rootNode: BabylonMdxModel;
    mdxModel: Model;
    skeletonResult: SkeletonBuildResult | null;
    textureLoader: War3TextureLoader;
    modelPath?: string;
    signal?: AbortSignal;
    onWarning?: (warning: War3ViewerWarning) => void;
}

interface Particle {
    location: Vector3;
    velocity: Vector3;
    gravity: number;
    age: number;
    lifeSpan: number;
    angle: number;
}

interface RuntimeEmitter {
    emitter: ParticleEmitter2;
    node: TransformNode;
    mesh: Mesh;
    material: ShaderMaterial;
    particles: Particle[];
    spawnRemainder: number;
    lastWorldMatrix: Matrix | null;
    lastSquirtFrame: number | null;
    lastSquirtSampleFrame: number | null;
}

interface RibbonSegment {
    bottom: Vector3;
    top: Vector3;
    age: number;
}

interface RuntimeRibbonEmitter {
    emitter: RibbonEmitter;
    node: TransformNode;
    meshes: Mesh[];
    ownedMaterial: StandardMaterial | null;
    segments: RibbonSegment[];
    spawnRemainder: number;
}

interface ParticleTextureSource {
    path: string;
    texture?: MdxTexture;
}

interface ParticleVisual {
    scale: number;
    color: readonly [number, number, number, number];
    firstHalf: boolean;
    localT: number;
}

const MAX_PARTICLES_PER_EMITTER = 4096;
const MAX_SPAWN_PER_FRAME = 256;
const MAX_RIBBON_SEGMENTS_PER_EMITTER = 4096;
const MAX_DELTA_SECONDS = 0.1;
const EPSILON = 1e-6;
const PARTICLE_VERTEX_SHADER = `
precision highp float;
attribute vec3 position;
attribute vec2 uv;
attribute vec4 color;
uniform mat4 worldViewProjection;
varying vec2 vUV;
varying vec4 vColor;
void main(void) {
    gl_Position = worldViewProjection * vec4(position, 1.0);
    vUV = uv;
    vColor = color;
}`;
const PARTICLE_FRAGMENT_SHADER = `
precision highp float;
varying vec2 vUV;
varying vec4 vColor;
uniform sampler2D particleTexture;
uniform float hasTexture;
uniform float alphaCutoff;
uniform float colorScale;
void main(void) {
    vec4 texel = hasTexture > 0.5 ? texture2D(particleTexture, vUV) : vec4(1.0);
    vec4 outputColor = texel * vColor;
    if (outputColor.a < alphaCutoff) discard;
    gl_FragColor = vec4(outputColor.rgb * colorScale, outputColor.a);
}`;

/**
 * Runtime for Warcraft III ParticleEmitter2 nodes.
 *
 * Simulation stays in world space, while each emitter is represented by one
 * dynamic mesh. This preserves MDX billboard/tail behavior without creating a
 * Babylon mesh and material for every particle.
 */
export default class ParticleSystem {
    private readonly scene: Scene;
    private readonly rootNode: BabylonMdxModel;
    private readonly mdxModel: Model;
    private readonly skeletonResult: SkeletonBuildResult | null;
    private readonly textureLoader: War3TextureLoader;
    private readonly modelPath?: string;
    private readonly signal: AbortSignal;
    private readonly onWarning?: (warning: War3ViewerWarning) => void;
    private readonly emitters: RuntimeEmitter[] = [];
    private readonly ribbonEmitters: RuntimeRibbonEmitter[] = [];
    private observer: NullableObserver = null;
    private spawnEnabled = true;
    private globalTimeMs = 0;
    private disposed = false;

    constructor(options: ParticleSystemOptions) {
        this.scene = options.scene;
        this.rootNode = options.rootNode;
        this.mdxModel = options.mdxModel;
        this.skeletonResult = options.skeletonResult;
        this.textureLoader = options.textureLoader;
        this.modelPath = options.modelPath;
        this.signal = options.signal ?? new AbortController().signal;
        this.onWarning = options.onWarning;
    }

    get hasEmitters(): boolean {
        return this.emitters.length > 0 || this.ribbonEmitters.length > 0;
    }

    async initialize(): Promise<void> {
        const emitters = this.mdxModel.particleEmitters2.filter((emitter) => emitter.lifeSpan > 0);
        const runtimes = await Promise.all(emitters.map(async (emitter) => {
            const material = await this.createMaterial(emitter);
            if (this.signal.aborted) throw this.signal.reason;
            const node = this.createEmitterNode(emitter);
            const mesh = this.createEmitterMesh(emitter, material);
            return {
                emitter,
                node,
                mesh,
                material,
                particles: [],
                spawnRemainder: 0,
                lastWorldMatrix: null,
                lastSquirtFrame: null,
                lastSquirtSampleFrame: null,
            } satisfies RuntimeEmitter;
        }));
        const ribbonRuntimes = this.mdxModel.ribbonEmitters
            .filter((emitter) => emitter.lifeSpan > 0 && emitter.emissionRate > 0)
            .map((emitter) => this.createRibbonRuntime(emitter));

        if (this.disposed) {
            for (const runtime of runtimes) this.disposeRuntime(runtime);
            for (const runtime of ribbonRuntimes) this.disposeRibbonRuntime(runtime);
            return;
        }

        this.emitters.push(...runtimes);
        this.ribbonEmitters.push(...ribbonRuntimes);
        if (this.hasEmitters && !this.observer) {
            this.observer = this.scene.onBeforeRenderObservable.add(() => this.update());
        }
    }

    setSpawnEnabled(enabled: boolean): void {
        this.spawnEnabled = enabled;
    }

    dispose(): void {
        if (this.disposed) return;
        this.disposed = true;

        if (this.observer) {
            this.scene.onBeforeRenderObservable.remove(this.observer);
            this.observer = null;
        }

        for (const runtime of this.emitters) this.disposeRuntime(runtime);
        this.emitters.length = 0;
        for (const runtime of this.ribbonEmitters) this.disposeRibbonRuntime(runtime);
        this.ribbonEmitters.length = 0;
    }

    private disposeRuntime(runtime: RuntimeEmitter): void {
        runtime.particles.length = 0;
        runtime.mesh.dispose(false, false);
        runtime.node.dispose(false, false);
        runtime.material.dispose(false, false);
    }

    private disposeRibbonRuntime(runtime: RuntimeRibbonEmitter): void {
        runtime.segments.length = 0;
        for (const mesh of runtime.meshes) mesh.dispose(false, false);
        runtime.node.dispose(false, false);
        runtime.ownedMaterial?.dispose(false, false);
    }

    private update(): void {
        if (this.disposed) return;

        const deltaSeconds = Math.min(
            Math.max(this.scene.getEngine().getDeltaTime(), 0) / 1000,
            MAX_DELTA_SECONDS,
        );
        this.globalTimeMs += deltaSeconds * 1000;
        const clock = this.createAnimationClock();

        for (const runtime of this.emitters) {
            const worldMatrix = runtime.node.computeWorldMatrix(true).clone();
            this.applyModelSpaceDelta(runtime, worldMatrix);

            if (this.spawnEnabled) {
                this.emitParticles(runtime, worldMatrix, deltaSeconds, clock);
            }

            this.updateParticles(runtime, deltaSeconds);
            this.rebuildEmitterMesh(runtime);
            runtime.lastWorldMatrix = worldMatrix;
        }

        for (const runtime of this.ribbonEmitters) {
            this.updateRibbonEmitter(runtime, deltaSeconds, clock);
        }
    }

    private updateRibbonEmitter(
        runtime: RuntimeRibbonEmitter,
        deltaSeconds: number,
        clock: AnimationSampleClock,
    ): void {
        for (const segment of runtime.segments) segment.age += deltaSeconds;
        runtime.segments = runtime.segments.filter((segment) => segment.age <= runtime.emitter.lifeSpan);

        const visibility = this.sampleEmitterValue(runtime.emitter, 'KRVS', 1, clock);
        if (this.spawnEnabled && visibility > 0) {
            runtime.spawnRemainder += Math.max(0, runtime.emitter.emissionRate) * deltaSeconds;
            const capacity = MAX_RIBBON_SEGMENTS_PER_EMITTER - runtime.segments.length;
            const count = Math.min(capacity, MAX_SPAWN_PER_FRAME, Math.floor(runtime.spawnRemainder));
            if (count > 0) {
                runtime.spawnRemainder -= count;
                const heightAbove = this.sampleEmitterValue(
                    runtime.emitter,
                    'KRHA',
                    runtime.emitter.heightAbove,
                    clock,
                );
                const heightBelow = this.sampleEmitterValue(
                    runtime.emitter,
                    'KRHB',
                    runtime.emitter.heightBelow,
                    clock,
                );
                const worldMatrix = runtime.node.computeWorldMatrix(true);
                for (let index = 0; index < count; index++) {
                    runtime.segments.push({
                        bottom: Vector3.TransformCoordinates(
                            new Vector3(0, -heightBelow, 0),
                            worldMatrix,
                        ),
                        top: Vector3.TransformCoordinates(
                            new Vector3(0, heightAbove, 0),
                            worldMatrix,
                        ),
                        age: 0,
                    });
                }
            }
        }

        this.rebuildRibbonMesh(runtime, clock);
    }

    private rebuildRibbonMesh(
        runtime: RuntimeRibbonEmitter,
        clock: AnimationSampleClock,
    ): void {
        if (runtime.segments.length < 2) {
            for (const mesh of runtime.meshes) mesh.setEnabled(false);
            return;
        }

        const emitter = runtime.emitter;
        const alpha = clamp(this.sampleEmitterValue(emitter, 'KRAL', emitter.alpha, clock), 0, 1);
        const color = sampleVectorAnimation(
            findAnimation(emitter.animations, 'KRCO'),
            emitter.color,
            clock,
        );
        const textureSlot = Math.max(0, Math.floor(this.sampleEmitterValue(
            emitter,
            'KRTX',
            emitter.textureSlot,
            clock,
        )));
        const rows = Math.max(1, emitter.rows);
        const columns = Math.max(1, emitter.columns);
        const column = textureSlot % columns;
        const row = Math.floor(textureSlot / columns) % rows;
        const uOffset = column / columns;
        const v0 = row / rows;
        const v1 = (row + 1) / rows;
        const positions: number[] = [];
        const uvs: number[] = [];
        const colors: number[] = [];
        const indices: number[] = [];

        for (const segment of runtime.segments) {
            const u = uOffset + clamp(segment.age / emitter.lifeSpan, 0, 1) / columns;
            positions.push(
                segment.bottom.x, segment.bottom.y, segment.bottom.z,
                segment.top.x, segment.top.y, segment.top.z,
            );
            uvs.push(u, v0, u, v1);
            const vertexColor = [
                finiteOr(color[0], 1),
                finiteOr(color[1], 1),
                finiteOr(color[2], 1),
                alpha,
            ];
            colors.push(...vertexColor, ...vertexColor);
        }

        for (let index = 0; index + 1 < runtime.segments.length; index++) {
            const bottom = index * 2;
            const top = bottom + 1;
            const nextBottom = bottom + 2;
            const nextTop = bottom + 3;
            indices.push(bottom, top, nextBottom, nextBottom, top, nextTop);
        }

        for (const mesh of runtime.meshes) {
            mesh.setVerticesData(VertexBuffer.PositionKind, positions, true, 3);
            mesh.setVerticesData(VertexBuffer.UVKind, uvs, true, 2);
            mesh.setVerticesData(VertexBuffer.ColorKind, colors, true, 4);
            mesh.setIndices(indices, positions.length / 3, true);
            mesh.setEnabled(true);
        }
    }

    private createAnimationClock(): AnimationSampleClock {
        const activeAnimation = this.rootNode.activeAnimation;
        const sequence = this.rootNode.currentSequence ?? null;
        const fallbackFrame = sequence?.interval[0] ?? 0;
        const frame = activeAnimation && Number.isFinite(activeAnimation.masterFrame)
            ? activeAnimation.masterFrame
            : fallbackFrame;

        return {
            frame,
            sequence,
            globalTimeMs: this.globalTimeMs,
            globalSequences: this.mdxModel.globalSequences,
        };
    }

    private emitParticles(
        runtime: RuntimeEmitter,
        worldMatrix: Matrix,
        deltaSeconds: number,
        clock: AnimationSampleClock,
    ): void {
        const emitter = runtime.emitter;
        const visibility = this.sampleEmitterValue(emitter, 'KP2V', 1, clock);
        if (visibility <= 0) return;

        const emissionAnimation = findAnimation(emitter.animations, 'KP2E');
        if (emitter.squirt && emissionAnimation) {
            const sampleFrame = getAnimationSampleFrame(emissionAnimation, clock);
            if (
                sampleFrame !== null
                && runtime.lastSquirtSampleFrame !== null
                && sampleFrame + EPSILON < runtime.lastSquirtSampleFrame
            ) {
                runtime.lastSquirtFrame = null;
            }
            runtime.lastSquirtSampleFrame = sampleFrame;

            const keyframe = findScalarKeyframe(emissionAnimation, clock);
            if (keyframe && keyframe.frame !== runtime.lastSquirtFrame) {
                runtime.lastSquirtFrame = keyframe.frame;
                runtime.spawnRemainder += Math.max(0, keyframe.value);
            }
        } else {
            const emissionRate = this.sampleEmitterValue(
                emitter,
                'KP2E',
                emitter.emissionRate,
                clock,
            );
            runtime.spawnRemainder += Math.max(0, emissionRate) * deltaSeconds;
        }

        const capacity = MAX_PARTICLES_PER_EMITTER - runtime.particles.length;
        const count = Math.min(
            capacity,
            MAX_SPAWN_PER_FRAME,
            Math.floor(runtime.spawnRemainder),
        );
        if (count <= 0) return;

        runtime.spawnRemainder -= count;
        runtime.spawnRemainder = Math.min(runtime.spawnRemainder, MAX_PARTICLES_PER_EMITTER);

        const width = Math.max(0, this.sampleEmitterValue(emitter, 'KP2W', emitter.width, clock));
        const length = Math.max(0, this.sampleEmitterValue(emitter, 'KP2N', emitter.length, clock));
        const speed = this.sampleEmitterValue(emitter, 'KP2S', emitter.speed, clock);
        const variation = Math.max(0, this.sampleEmitterValue(emitter, 'KP2R', emitter.variation, clock));
        const latitude = this.degToRad(Math.max(0, this.sampleEmitterValue(emitter, 'KP2L', emitter.latitude, clock)));
        const gravity = this.sampleEmitterValue(emitter, 'KP2G', emitter.gravity, clock);

        for (let index = 0; index < count; index++) {
            runtime.particles.push(this.createParticle(
                emitter,
                worldMatrix,
                width,
                length,
                speed,
                variation,
                latitude,
                gravity,
            ));
        }
    }

    private createParticle(
        emitter: ParticleEmitter2,
        worldMatrix: Matrix,
        width: number,
        length: number,
        speed: number,
        variation: number,
        latitude: number,
        gravity: number,
    ): Particle {
        const localLocation = new Vector3(
            this.randomInRange(-width, width),
            this.randomInRange(-length, length),
            0,
        );
        const location = Vector3.TransformCoordinates(localLocation, worldMatrix);

        const angle = this.randomInRange(0, Math.PI * 2);
        const coneAngle = this.randomInRange(0, latitude);
        const coneRadius = Math.sin(coneAngle);
        const localDirection = new Vector3(
            coneRadius * Math.cos(angle),
            coneRadius * Math.sin(angle),
            Math.cos(coneAngle),
        );
        if ((emitter.flags & ParticleFlags.LineEmitter) !== 0) {
            localDirection.x = 0;
        }

        const actualSpeed = speed * (1 + this.randomInRange(-variation, variation));
        localDirection.scaleInPlace(actualSpeed);
        const velocity = Vector3.TransformNormal(localDirection, worldMatrix);

        return {
            location,
            velocity,
            gravity,
            age: 0,
            lifeSpan: emitter.lifeSpan,
            angle,
        };
    }

    private applyModelSpaceDelta(runtime: RuntimeEmitter, worldMatrix: Matrix): void {
        if (
            (runtime.emitter.flags & ParticleFlags.ModelSpace) === 0
            || !runtime.lastWorldMatrix
            || !runtime.particles.length
        ) {
            return;
        }

        const inversePrevious = runtime.lastWorldMatrix.clone();
        inversePrevious.invert();
        for (const particle of runtime.particles) {
            const localPosition = Vector3.TransformCoordinates(particle.location, inversePrevious);
            const localVelocity = Vector3.TransformNormal(particle.velocity, inversePrevious);
            particle.location = Vector3.TransformCoordinates(localPosition, worldMatrix);
            particle.velocity = Vector3.TransformNormal(localVelocity, worldMatrix);
        }
    }

    private updateParticles(runtime: RuntimeEmitter, deltaSeconds: number): void {
        const particles = runtime.particles;
        for (let index = particles.length - 1; index >= 0; index--) {
            const particle = particles[index];
            particle.age += deltaSeconds;
            if (particle.age >= particle.lifeSpan) {
                particles[index] = particles[particles.length - 1];
                particles.pop();
                continue;
            }

            particle.velocity.y -= particle.gravity * deltaSeconds;
            particle.location.addInPlace(particle.velocity.scale(deltaSeconds));
        }
    }

    private rebuildEmitterMesh(runtime: RuntimeEmitter): void {
        const mesh = runtime.mesh;
        if (!runtime.particles.length) {
            mesh.setEnabled(false);
            return;
        }

        const camera = this.scene.activeCamera;
        if (!camera) {
            mesh.setEnabled(false);
            return;
        }

        const cameraWorld = camera.getWorldMatrix();
        const cameraRight = Vector3.TransformNormal(Vector3.Right(), cameraWorld).normalize();
        const cameraUp = Vector3.TransformNormal(Vector3.Up(), cameraWorld).normalize();
        const cameraPosition = camera.globalPosition;
        const particles = (runtime.emitter.flags & ParticleFlags.SortPrimsFarZ) !== 0
            ? [...runtime.particles].sort((left, right) => (
                Vector3.DistanceSquared(right.location, cameraPosition)
                - Vector3.DistanceSquared(left.location, cameraPosition)
            ))
            : runtime.particles;

        const positions: number[] = [];
        const uvs: number[] = [];
        const colors: number[] = [];
        const indices: number[] = [];
        const emitter = runtime.emitter;

        for (const particle of particles) {
            const visual = this.getParticleVisual(particle, emitter);
            if (emitter.headOrTail === HeadOrTail.Tail || emitter.headOrTail === HeadOrTail.Both) {
                this.appendTailQuad(
                    positions,
                    uvs,
                    colors,
                    indices,
                    particle,
                    emitter,
                    visual,
                    cameraPosition,
                    cameraRight,
                    cameraUp,
                );
            }
            if (emitter.headOrTail === HeadOrTail.Head || emitter.headOrTail === HeadOrTail.Both) {
                this.appendHeadQuad(
                    positions,
                    uvs,
                    colors,
                    indices,
                    particle,
                    emitter,
                    visual,
                    cameraRight,
                    cameraUp,
                );
            }
        }

        if (!positions.length) {
            mesh.setEnabled(false);
            return;
        }

        mesh.setVerticesData(VertexBuffer.PositionKind, positions, true, 3);
        mesh.setVerticesData(VertexBuffer.UVKind, uvs, true, 2);
        mesh.setVerticesData(VertexBuffer.ColorKind, colors, true, 4);
        mesh.setIndices(indices, positions.length / 3, true);
        mesh.setEnabled(true);
    }

    private appendHeadQuad(
        positions: number[],
        uvs: number[],
        colors: number[],
        indices: number[],
        particle: Particle,
        emitter: ParticleEmitter2,
        visual: ParticleVisual,
        cameraRight: Vector3,
        cameraUp: Vector3,
    ): void {
        const center = particle.location;
        const scale = visual.scale;
        let p0: Vector3;
        let p1: Vector3;
        let p2: Vector3;
        let p3: Vector3;

        if ((emitter.flags & ParticleFlags.XYQuad) !== 0) {
            const cosine = Math.cos(particle.angle);
            const sine = Math.sin(particle.angle);
            const rotate = (x: number, y: number): Vector3 => new Vector3(
                (x * cosine - y * sine) * scale,
                0,
                -(x * sine + y * cosine) * scale,
            );
            p0 = center.add(rotate(-1, 1));
            p1 = center.add(rotate(-1, -1));
            p2 = center.add(rotate(1, 1));
            p3 = center.add(rotate(1, -1));
        } else {
            const right = cameraRight.scale(scale);
            const up = cameraUp.scale(scale);
            p0 = center.subtract(right).add(up);
            p1 = center.subtract(right).subtract(up);
            p2 = center.add(right).add(up);
            p3 = center.add(right).subtract(up);
        }

        this.appendQuad(
            positions,
            uvs,
            colors,
            indices,
            p0,
            p1,
            p2,
            p3,
            this.getParticleUv(emitter, HeadOrTail.Head, visual.firstHalf, visual.localT),
            visual.color,
        );
    }

    private appendTailQuad(
        positions: number[],
        uvs: number[],
        colors: number[],
        indices: number[],
        particle: Particle,
        emitter: ParticleEmitter2,
        visual: ParticleVisual,
        cameraPosition: Vector3,
        cameraRight: Vector3,
        cameraUp: Vector3,
    ): void {
        const tailOffset = particle.velocity.scale(-emitter.tailLength);
        const toCamera = cameraPosition.subtract(particle.location);
        let cross = Vector3.Cross(particle.velocity, toCamera);
        if (cross.lengthSquared() < EPSILON) cross = Vector3.Cross(particle.velocity, cameraUp);
        if (cross.lengthSquared() < EPSILON) cross = cameraRight.clone();
        cross.normalize().scaleInPlace(visual.scale);

        const p0 = particle.location.add(cross);
        const p1 = particle.location.subtract(cross);
        const p2 = p0.add(tailOffset);
        const p3 = p1.add(tailOffset);
        this.appendQuad(
            positions,
            uvs,
            colors,
            indices,
            p0,
            p1,
            p2,
            p3,
            this.getParticleUv(emitter, HeadOrTail.Tail, visual.firstHalf, visual.localT),
            visual.color,
        );
    }

    private appendQuad(
        positions: number[],
        uvs: number[],
        colors: number[],
        indices: number[],
        p0: Vector3,
        p1: Vector3,
        p2: Vector3,
        p3: Vector3,
        uv: readonly [number, number, number, number],
        color: readonly [number, number, number, number],
    ): void {
        const vertexOffset = positions.length / 3;
        positions.push(
            p0.x, p0.y, p0.z,
            p1.x, p1.y, p1.z,
            p2.x, p2.y, p2.z,
            p3.x, p3.y, p3.z,
        );
        uvs.push(
            uv[0], uv[1],
            uv[0], uv[3],
            uv[2], uv[1],
            uv[2], uv[3],
        );
        for (let index = 0; index < 4; index++) colors.push(...color);
        indices.push(
            vertexOffset,
            vertexOffset + 1,
            vertexOffset + 2,
            vertexOffset + 2,
            vertexOffset + 1,
            vertexOffset + 3,
        );
    }

    private getParticleVisual(particle: Particle, emitter: ParticleEmitter2): ParticleVisual {
        const globalT = clamp(particle.age / Math.max(particle.lifeSpan, EPSILON), 0, 1);
        const middle = clamp(emitter.timeMiddle, 0, 1);
        const firstHalf = globalT < middle;
        const localT = firstHalf
            ? (middle > EPSILON ? globalT / middle : 0)
            : (1 - middle > EPSILON ? (globalT - middle) / (1 - middle) : 0);
        const start = firstHalf ? 0 : 1;
        const end = firstHalf ? 1 : 2;

        const scale = lerp(
            finiteOr(emitter.segmentScaling[start], 1),
            finiteOr(emitter.segmentScaling[end], 1),
            localT,
        );
        const startColor = emitter.segmentColors[start];
        const endColor = emitter.segmentColors[end];
        const alpha = lerp(
            finiteOr(emitter.segmentAlphas[start], 255),
            finiteOr(emitter.segmentAlphas[end], 255),
            localT,
        ) / 255;

        return {
            scale: Math.max(0, scale),
            color: [
                lerp(finiteOr(startColor[0], 1), finiteOr(endColor[0], 1), localT),
                lerp(finiteOr(startColor[1], 1), finiteOr(endColor[1], 1), localT),
                lerp(finiteOr(startColor[2], 1), finiteOr(endColor[2], 1), localT),
                clamp(alpha, 0, 1),
            ],
            firstHalf,
            localT: clamp(localT, 0, 1),
        };
    }

    private getParticleUv(
        emitter: ParticleEmitter2,
        type: HeadOrTail.Head | HeadOrTail.Tail,
        firstHalf: boolean,
        localT: number,
    ): readonly [number, number, number, number] {
        const intervals = type === HeadOrTail.Tail ? emitter.tailIntervals : emitter.headIntervals;
        const interval = intervals[firstHalf ? 0 : 1];
        const rows = Math.max(1, emitter.rows);
        const columns = Math.max(1, emitter.columns);
        const maxFrame = rows * columns - 1;
        const firstFrame = finiteOr(interval[0], 0);
        const lastFrame = finiteOr(interval[1], firstFrame);
        const frame = clamp(Math.round(lerp(firstFrame, lastFrame, localT)), 0, maxFrame);
        const column = frame % columns;
        const row = Math.floor(frame / columns);
        return [
            column / columns,
            row / rows,
            (column + 1) / columns,
            (row + 1) / rows,
        ];
    }

    private sampleEmitterValue(
        emitter: ParticleEmitter2 | RibbonEmitter,
        animationName: string,
        defaultValue: number,
        clock: AnimationSampleClock,
    ): number {
        return sampleScalarAnimation(
            findAnimation(emitter.animations, animationName),
            defaultValue,
            clock,
        );
    }

    private createEmitterNode(emitter: ParticleEmitter2 | RibbonEmitter): TransformNode {
        const node = new TransformNode(
            `${this.rootNode.name}_${emitter.name || emitter.objectId}_Emitter`,
            this.scene,
        );
        node.parent = this.rootNode;

        const bone = this.skeletonResult?.boneMap.get(emitter.objectId);
        if (bone) {
            node.position.set(0, 0, 0);
            node.attachToBone(bone, this.rootNode);
        } else {
            const pivot = this.mdxModel.pivotPoints[emitter.objectId];
            node.position.set(
                pivot ? -(pivot[0] ?? 0) : 0,
                pivot ? pivot[1] ?? 0 : 0,
                pivot ? pivot[2] ?? 0 : 0,
            );
        }

        return node;
    }

    private createRibbonRuntime(emitter: RibbonEmitter): RuntimeRibbonEmitter {
        const layerMaterials = this.rootNode.materialLayers?.[emitter.materialId] ?? [];
        const fallbackSharedMaterial = this.rootNode.materials[emitter.materialId];
        const ownedMaterial = layerMaterials.length || fallbackSharedMaterial
            ? null
            : this.createFallbackRibbonMaterial(emitter);
        const materials = layerMaterials.length
            ? layerMaterials
            : [fallbackSharedMaterial ?? ownedMaterial!];
        const priorityPlane = this.mdxModel.materials[emitter.materialId]?.priorityPlane ?? 0;
        const meshes = materials.map((material, layerIndex) => {
            const layerSuffix = layerIndex === 0 ? '' : `_Layer_${layerIndex}`;
            const mesh = new Mesh(
                `${this.rootNode.name}_${emitter.name || emitter.objectId}_Ribbon${layerSuffix}`,
                this.scene,
            );
            mesh.material = material;
            mesh.hasVertexAlpha = true;
            mesh.isPickable = false;
            mesh.alwaysSelectAsActiveMesh = true;
            mesh.alphaIndex = priorityPlane + layerIndex;
            mesh.setEnabled(false);
            return mesh;
        });

        return {
            emitter,
            node: this.createEmitterNode(emitter),
            meshes,
            ownedMaterial,
            segments: [],
            spawnRemainder: 0,
        };
    }

    private createFallbackRibbonMaterial(emitter: RibbonEmitter): StandardMaterial {
        const material = new StandardMaterial(
            `${this.rootNode.name}_${emitter.name || emitter.objectId}_RibbonMat`,
            this.scene,
        );
        material.diffuseColor = Color3.White();
        material.emissiveColor = Color3.White();
        material.disableLighting = true;
        material.backFaceCulling = false;
        material.useAlphaFromDiffuseTexture = true;
        material.transparencyMode = StandardMaterial.MATERIAL_ALPHABLEND;
        material.alphaMode = Engine.ALPHA_COMBINE;
        material.disableDepthWrite = true;
        return material;
    }

    private createEmitterMesh(emitter: ParticleEmitter2, material: ShaderMaterial): Mesh {
        const mesh = new Mesh(
            `${this.rootNode.name}_${emitter.name || emitter.objectId}_Particles`,
            this.scene,
        );
        mesh.material = material;
        mesh.hasVertexAlpha = true;
        mesh.isPickable = false;
        mesh.alwaysSelectAsActiveMesh = true;
        mesh.alphaIndex = emitter.priorityPlane;
        mesh.setEnabled(false);
        return mesh;
    }

    private async createMaterial(emitter: ParticleEmitter2): Promise<ShaderMaterial> {
        const alphaKey = emitter.filterMode === ParticleFilterMode.AlphaKey;
        const modulate = emitter.filterMode === ParticleFilterMode.Modulate
            || emitter.filterMode === ParticleFilterMode.Modulate2x;
        const material = new ShaderMaterial(
            `${this.rootNode.name}_${emitter.name || emitter.objectId}_ParticleMat`,
            this.scene,
            {
                vertexSource: PARTICLE_VERTEX_SHADER,
                fragmentSource: PARTICLE_FRAGMENT_SHADER,
            },
            {
                attributes: ['position', 'uv', 'color'],
                uniforms: ['worldViewProjection', 'hasTexture', 'alphaCutoff', 'colorScale'],
                samplers: ['particleTexture'],
                // Warcraft III AlphaKey particles still use additive blending
                // after the alpha threshold removes their transparent texels.
                needAlphaBlending: true,
                needAlphaTesting: alphaKey,
            },
        );
        material.backFaceCulling = false;
        material.disableDepthWrite = true;
        material.forceDepthWrite = false;
        material.alphaMode = this.mapParticleAlphaMode(emitter.filterMode);
        material.setFloat('alphaCutoff', alphaKey ? 0.83 : modulate ? 0.01 : 0);
        material.setFloat(
            'colorScale',
            emitter.filterMode === ParticleFilterMode.Modulate2x ? 2 : 1,
        );

        const texture = await this.loadParticleTexture(emitter);
        if (texture) {
            texture.hasAlpha = true;
            texture.gammaSpace = false;
            material.setTexture('particleTexture', texture);
            material.setFloat('hasTexture', 1);
        } else {
            material.setFloat('hasTexture', 0);
        }

        return material;
    }

    private mapParticleAlphaMode(filterMode: ParticleFilterMode): number {
        switch (filterMode) {
            case ParticleFilterMode.Additive:
                return Engine.ALPHA_ADD;
            case ParticleFilterMode.Modulate:
            case ParticleFilterMode.Modulate2x:
                return Engine.ALPHA_MULTIPLY;
            case ParticleFilterMode.AlphaKey:
                return Engine.ALPHA_ADD;
            default:
                return Engine.ALPHA_COMBINE;
        }
    }

    private async loadParticleTexture(emitter: ParticleEmitter2): Promise<Texture | null> {
        const source = this.resolveTextureSource(emitter);
        if (!source) return null;

        try {
            const texture = await this.textureLoader.load({
                kind: 'particle-texture',
                requestedPath: source.path,
                candidatePaths: this.getCandidateTexturePaths(source.path),
                modelPath: this.modelPath ?? '',
                signal: this.signal,
                wrapMode: source.texture?.wrapMode ?? 0,
            });
            texture.hasAlpha = true;
            return texture as Texture;
        } catch (error) {
            if ((error as Error).name === 'AbortError') throw error;
            this.onWarning?.({
                code: 'missing-resource',
                message: `ParticleEmitter2 "${emitter.name || emitter.objectId}" is using an untextured fallback.`,
                requestedPath: source.path,
                cause: error,
            });
            return null;
        }
    }

    private resolveTextureSource(emitter: ParticleEmitter2): ParticleTextureSource | null {
        const texture = emitter.textureId >= 0 && emitter.textureId < this.mdxModel.textures.length
            ? this.mdxModel.textures[emitter.textureId]
            : undefined;
        const directPath = texture?.path?.trim();
        if (directPath) return { path: directPath, texture };

        const replaceableId = emitter.replaceableId || texture?.replaceableId || 0;
        const replaceablePath = replaceableIds[replaceableId];
        if (!replaceablePath) return null;
        return { path: `ReplaceableTextures/${replaceablePath}.blp`, texture };
    }

    private getCandidateTexturePaths(texturePath: string): string[] {
        const normalizedPath = texturePath.replace(/\\/g, '/');
        const textureFileName = normalizedPath.split('/').pop() || normalizedPath;
        const normalizedModelPath = this.modelPath?.replace(/\\/g, '/') ?? '';
        const modelSlash = normalizedModelPath.lastIndexOf('/');
        const modelDir = modelSlash >= 0 ? normalizedModelPath.slice(0, modelSlash) : '';
        const candidates: string[] = [];

        if (modelDir) {
            candidates.push(`${modelDir}/${normalizedPath}`);
            candidates.push(`${modelDir}/${textureFileName}`);
        }
        candidates.push(normalizedPath);
        return [...new Set(candidates)];
    }

    private randomInRange(minimum: number, maximum: number): number {
        return minimum + Math.random() * (maximum - minimum);
    }

    private degToRad(degrees: number): number {
        return degrees * Math.PI / 180;
    }
}

function finiteOr(value: number | undefined, fallback: number): number {
    return Number.isFinite(value) ? value! : fallback;
}

function lerp(a: number, b: number, t: number): number {
    return a + (b - a) * t;
}

function clamp(value: number, minimum: number, maximum: number): number {
    return Math.min(maximum, Math.max(minimum, value));
}

type NullableObserver = ReturnType<Scene['onBeforeRenderObservable']['add']> | null;
