---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/adapters/mdx/billboardRuntime.ts"
ref: HEAD
---

import type { Bone } from '@babylonjs/core/Bones/bone';
import { Axis, Space } from '@babylonjs/core/Maths/math.axis';
import { Quaternion, Vector3 } from '@babylonjs/core/Maths/math.vector';
import type { TransformNode } from '@babylonjs/core/Meshes/transformNode';
import type { Scene } from '@babylonjs/core/scene';
import { Flags } from '../../parsers/mdlx/genericobject';
import type { SkeletonBuildResult } from './skeletonBuilder';

interface BillboardBinding {
    bone: Bone;
    flags: number;
    authoredRotation: Quaternion;
    appliedRotation: Quaternion | null;
}

type NullableObserver = ReturnType<Scene['onBeforeRenderObservable']['add']> | null;

const BILLBOARD_FLAGS = Flags.Billboarded
    | Flags.BillboardedLockX
    | Flags.BillboardedLockY
    | Flags.BillboardedLockZ;
const EPSILON = 1e-6;

/** Applies Warcraft III generic-node billboard flags after Babylon evaluates animations. */
export default class BillboardRuntime {
    private readonly bindings: BillboardBinding[];
    private observer: NullableObserver = null;
    private disposed = false;

    constructor(
        private readonly scene: Scene,
        private readonly rootNode: TransformNode,
        private readonly skeletonResult: SkeletonBuildResult,
    ) {
        this.bindings = [...skeletonResult.nodeInfoMap.values()]
            .filter(({ flags }) => (flags & BILLBOARD_FLAGS) !== 0)
            .map(({ bone, flags }) => ({
                bone,
                flags,
                authoredRotation: bone.getRotationQuaternion(Space.LOCAL),
                appliedRotation: null,
            }))
            .sort((left, right) => left.bone.getIndex() - right.bone.getIndex());
    }

    initialize(): void {
        if (!this.bindings.length || this.observer || this.disposed) return;
        this.observer = this.scene.onBeforeRenderObservable.add(() => this.update());
        this.update();
    }

    dispose(): void {
        if (this.disposed) return;
        this.disposed = true;
        if (this.observer) {
            this.scene.onBeforeRenderObservable.remove(this.observer);
            this.observer = null;
        }
        for (const binding of this.bindings) {
            binding.bone.setRotationQuaternion(binding.authoredRotation, Space.LOCAL);
        }
        this.skeletonResult.skeleton.computeAbsoluteMatrices(true);
    }

    private update(): void {
        const camera = this.scene.activeCamera;
        if (this.disposed || !camera) return;

        // Animation evaluation overwrites our previous result. Static nodes do not,
        // so distinguish an authored update from the rotation we applied last frame.
        for (const binding of this.bindings) {
            const current = binding.bone.getRotationQuaternion(Space.LOCAL);
            if (!binding.appliedRotation || !sameRotation(current, binding.appliedRotation)) {
                binding.authoredRotation.copyFrom(current);
            }
            binding.bone.setRotationQuaternion(binding.authoredRotation, Space.LOCAL);
            binding.appliedRotation = null;
        }
        this.skeletonResult.skeleton.computeAbsoluteMatrices(true);

        const cameraRotation = createWar3CameraRotation(camera.getDirection(Axis.Z), camera.getDirection(Axis.Y));
        const cameraForward = camera.getDirection(Axis.Z).normalize();

        for (const binding of this.bindings) {
            if ((binding.flags & Flags.Billboarded) !== 0) {
                // The source node's authored rotation remains relative to the camera,
                // while parent rotation is deliberately excluded.
                const target = cameraRotation.multiply(binding.authoredRotation).normalize();
                binding.bone.setRotationQuaternion(target, Space.WORLD, this.rootNode);
            } else {
                this.applyAxisLocked(binding, cameraForward);
            }

            binding.appliedRotation = binding.bone.getRotationQuaternion(Space.LOCAL);
            // A billboarded parent changes the basis used by its descendants.
            this.skeletonResult.skeleton.computeAbsoluteMatrices(true);
        }
    }

    private applyAxisLocked(binding: BillboardBinding, cameraForward: Vector3): void {
        const worldRotation = binding.bone.getRotationQuaternion(Space.WORLD, this.rootNode);
        const axis = lockedAxis(binding.flags).applyRotationQuaternion(worldRotation).normalize();
        const xAxis = Axis.X.applyRotationQuaternion(worldRotation).normalize();

        // Matches Warcraft III's axis-locked behavior: point the transformed local
        // X axis at the camera direction projected perpendicular to the locked axis.
        const target = cameraForward
            .scale(-1)
            .subtract(axis.scale(-Vector3.Dot(axis, cameraForward)));
        if (target.lengthSquared() < EPSILON) return;
        target.normalize();

        const delta = rotationBetween(xAxis, target);
        binding.bone.setRotationQuaternion(
            delta.multiply(worldRotation).normalize(),
            Space.WORLD,
            this.rootNode,
        );
    }
}

function createWar3CameraRotation(forwardValue: Vector3, upValue: Vector3): Quaternion {
    const xAxis = forwardValue.normalize().scale(-1);
    let zAxis = upValue.subtract(xAxis.scale(Vector3.Dot(upValue, xAxis)));
    if (zAxis.lengthSquared() < EPSILON) zAxis = Axis.Y.clone();
    zAxis.normalize();
    const yAxis = Vector3.Cross(zAxis, xAxis).normalize();
    zAxis = Vector3.Cross(xAxis, yAxis).normalize();
    return Quaternion.RotationQuaternionFromAxis(xAxis, yAxis, zAxis).normalize();
}

function lockedAxis(flags: number): Vector3 {
    if ((flags & Flags.BillboardedLockX) !== 0) return Axis.X;
    if ((flags & Flags.BillboardedLockY) !== 0) return Axis.Y;
    return Axis.Z;
}

function rotationBetween(from: Vector3, to: Vector3): Quaternion {
    const dot = Math.min(1, Math.max(-1, Vector3.Dot(from, to)));
    if (dot > 1 - EPSILON) return Quaternion.Identity();
    if (dot < -1 + EPSILON) {
        let axis = Vector3.Cross(from, Axis.X);
        if (axis.lengthSquared() < EPSILON) axis = Vector3.Cross(from, Axis.Y);
        return Quaternion.RotationAxis(axis.normalize(), Math.PI);
    }
    const cross = Vector3.Cross(from, to);
    return new Quaternion(cross.x, cross.y, cross.z, 1 + dot).normalize();
}

function sameRotation(left: Quaternion, right: Quaternion): boolean {
    return Math.abs(Quaternion.Dot(left, right)) > 1 - EPSILON;
}
