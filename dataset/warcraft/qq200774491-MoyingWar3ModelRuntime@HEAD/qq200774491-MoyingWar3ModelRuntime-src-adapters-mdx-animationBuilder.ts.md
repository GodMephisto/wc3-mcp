---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/adapters/mdx/animationBuilder.ts"
ref: HEAD
---

/**
 * AnimationBuilder - 将 MDX 动画轨道转换为 Babylon.js Animation
 *
 * 该类负责处理魔兽争霸3的MDX模型动画数据，将其转换为Babylon.js引擎可识别的动画格式。
 * 支持位置(KGTR)、旋转(KGRT)、缩放(KGSC)三种基本动画轨道的转换。
 */

import { Animation } from '@babylonjs/core/Animations/animation';
import type { IAnimationKey } from '@babylonjs/core/Animations/animationKey';
import type { Bone } from '@babylonjs/core/Bones/bone';
import { Quaternion, Vector3 } from '@babylonjs/core/Maths/math.vector';
import type { Nullable } from '@babylonjs/core/types';
import Model from '../../parsers/mdlx/model';
import MdlxBone from '../../parsers/mdlx/bone';
import { Animation as MdlxAnimation, InterpolationType } from '../../parsers/mdlx/animations';
import Sequence from '../../parsers/mdlx/sequence';
import { SkeletonBuildResult } from './skeletonBuilder';
import { war3ToBabylonQuaternion, war3ToBabylonPosition } from '../../utils/coordinates';

import MdlxHelper from '../../parsers/mdlx/helper';
import MdlxAttachment from '../../parsers/mdlx/attachment';
import MdlxParticleEmitter2 from '../../parsers/mdlx/particleemitter2';
import MdlxLight from '../../parsers/mdlx/light';
import MdlxParticleEmitter from '../../parsers/mdlx/particleemitter';
import MdlxParticleEmitterPopcorn from '../../parsers/mdlx/particleemitterpopcorn';
import MdlxRibbonEmitter from '../../parsers/mdlx/ribbonemitter';
import MdlxEventObject from '../../parsers/mdlx/eventobject';
import MdlxCollisionShape from '../../parsers/mdlx/collisionshape';

/** 动画帧率：MDX使用毫秒为单位，因此帧率设置为1000 */
const FRAME_RATE = 1000;

/** 可动画对象类型：包括骨骼、辅助节点和挂接点 */
type AnimatedObject =
    | MdlxBone
    | MdlxHelper
    | MdlxAttachment
    | MdlxLight
    | MdlxParticleEmitter
    | MdlxParticleEmitter2
    | MdlxParticleEmitterPopcorn
    | MdlxRibbonEmitter
    | MdlxEventObject
    | MdlxCollisionShape;

export interface GlobalAnimationBinding {
    bone: Bone;
    animation: Animation;
    duration: number;
}

/**
 * 动画轨道类型常量
 * 使用 const assertion 提供更精确的类型
 */
const AnimationTrackType = {
    /** 位置轨道 */
    TRANSLATION: 'KGTR',
    /** 旋转轨道 */
    ROTATION: 'KGRT',
    /** 缩放轨道 */
    SCALING: 'KGSC',
} as const;

type AnimationTrackType = typeof AnimationTrackType[keyof typeof AnimationTrackType];

type TypedAnimationKey<T> = Omit<IAnimationKey, 'value' | 'inTangent' | 'outTangent'> & {
    value: T;
    inTangent?: T;
    outTangent?: T;
};

/**
 * 动画构建器类
 * 负责将MDX模型的动画数据转换为Babylon.js骨骼动画
 */
export class AnimationBuilder {

    /**
     * 构建所有动画
     * 将MDX模型中的骨骼、辅助节点、挂接点的动画数据转换为Babylon骨骼动画
     * 
     * @param mdxModel - MDX模型数据
     * @param skeletonResult - 骨骼构建结果，包含skeleton和boneMap
     */
    buildAnimations(mdxModel: Model, skeletonResult: SkeletonBuildResult): GlobalAnimationBinding[] {
        const skeleton = skeletonResult.skeleton;
        const sequences = mdxModel.sequences || [];
        const globalBindings: GlobalAnimationBinding[] = [];

        // 构建包含所有可动画对象的数组（骨骼、辅助节点、挂接点）
        const allAnimatedObjects: AnimatedObject[] = [
            ...mdxModel.bones,
            ...mdxModel.helpers,
            ...mdxModel.attachments,
            ...mdxModel.lights,
            ...mdxModel.particleEmitters,
            ...mdxModel.particleEmitters2,
            ...mdxModel.particleEmittersPopcorn,
            ...mdxModel.ribbonEmitters,
            ...mdxModel.eventObjects,
            ...mdxModel.collisionShapes,
        ];

        // 遍历所有对象，为每个对象创建动画
        for (const objectData of allAnimatedObjects) {
            this.buildObjectAnimations(
                objectData,
                skeletonResult,
                sequences,
                mdxModel.globalSequences,
                globalBindings,
            );
        }

        // 创建全局动画范围
        skeleton.createAnimationRange('ALL', 0, this.getMaxSequenceFrame(sequences));
        return globalBindings;
    }

    /**
     * 为单个对象构建动画
     * 处理位置、旋转、缩放三种动画轨道
     * 
     * @param objectData - 动画对象数据（骨骼、辅助节点或挂接点）
     * @param skeletonResult - 骨骼构建结果
     */
    private buildObjectAnimations(
        objectData: AnimatedObject,
        skeletonResult: SkeletonBuildResult,
        sequences: Sequence[],
        globalSequences: number[],
        globalBindings: GlobalAnimationBinding[],
    ): void {
        const bone = skeletonResult.boneMap.get(objectData.objectId);
        if (!bone) {
            return;
        }

        // 创建位置动画（KGTR）
        const bindTranslation = skeletonResult.translationMap.get(objectData.objectId) ?? Vector3.Zero();
        const translationAnim = this.createTranslationAnimation(
            objectData,
            bone.name,
            sequences,
            bindTranslation
        );
        if (translationAnim) {
            this.registerAnimation(
                objectData,
                AnimationTrackType.TRANSLATION,
                bone,
                translationAnim,
                globalSequences,
                globalBindings,
            );
        }

        // 创建旋转动画（KGRT）
        const rotationAnim = this.createRotationAnimation(objectData, bone.name, sequences);
        if (rotationAnim) {
            this.registerAnimation(
                objectData,
                AnimationTrackType.ROTATION,
                bone,
                rotationAnim,
                globalSequences,
                globalBindings,
            );
        }

        // 创建缩放动画（KGSC）
        const scaleAnim = this.createScalingAnimation(objectData, bone.name, sequences);
        if (scaleAnim) {
            this.registerAnimation(
                objectData,
                AnimationTrackType.SCALING,
                bone,
                scaleAnim,
                globalSequences,
                globalBindings,
            );
        }
    }

    private registerAnimation(
        objectData: AnimatedObject,
        token: AnimationTrackType,
        bone: Bone,
        animation: Animation,
        globalSequences: number[],
        globalBindings: GlobalAnimationBinding[],
    ): void {
        const source = this.findAnimation(objectData, token);
        const duration = source && source.globalSequenceId >= 0
            ? globalSequences[source.globalSequenceId]
            : undefined;
        if (source && Number.isFinite(duration) && duration! > 0) {
            globalBindings.push({ bone, animation, duration: duration! });
        } else {
            bone.animations.push(animation);
        }
    }

    /**
     * 获取所有序列中的最大帧数
     * 用于确定动画的总时长
     * 
     * @param sequences - 动画序列数组
     * @returns 最大帧数
     */
    private getMaxSequenceFrame(sequences: Sequence[]): number {
        let maxFrame = 0;
        for (const seq of sequences) {
            if (seq.interval[1] > maxFrame) {
                maxFrame = seq.interval[1];
            }
        }
        return maxFrame;
    }

    /**
     * 在对象的动画数据中查找指定类型的动画轨道
     * 
     * @param objectData - 动画对象数据
     * @param token - 动画轨道类型标识（如'KGTR'、'KGRT'、'KGSC'）
     * @returns 找到的动画数据，未找到则返回null
     */
    private findAnimation(objectData: AnimatedObject, token: string): Nullable<MdlxAnimation> {
        return objectData.animations.find((anim) => anim.name === token) ?? null;
    }

    /**
     * 创建位置动画
     * 将MDX的KGTR轨道转换为Babylon位置动画，并应用坐标系转换
     *
     * @param objectData - 动画对象数据
     * @param boneName - 骨骼名称
     * @param baseOffset - 基础偏移量（可选）
     * @returns 位置动画对象，无动画数据则返回null
     */
    private createTranslationAnimation(
        objectData: AnimatedObject,
        boneName: string,
        sequences: Sequence[],
        bindTranslation: Vector3,
    ): Nullable<Animation> {
        // 将MDX坐标系（右手）转换为Babylon坐标系（左手）：X轴取反
        const converter = (vec: Float32Array): Vector3 => war3ToBabylonPosition(vec[0], vec[1], vec[2]);

        return this.createVectorAnimation(
            objectData,
            AnimationTrackType.TRANSLATION,
            `${boneName}_position`,
            'position',
            converter,
            sequences,
            bindTranslation,
            bindTranslation
        );
    }

    /**
     * 创建旋转动画
     * 将MDX的KGRT轨道转换为Babylon四元数旋转动画
     * 
     * @param objectData - 动画对象数据
     * @param boneName - 骨骼名称
     * @returns 旋转动画对象，无动画数据则返回null
     */
    private createRotationAnimation(
        objectData: AnimatedObject,
        boneName: string,
        sequences: Sequence[]
    ): Nullable<Animation> {
        const animData = this.findAnimation(objectData, AnimationTrackType.ROTATION);
        if (!animData || !animData.frames.length) {
            return null;
        }

        const keys = this.convertQuaternionKeys(animData, sequences);
        const animation = new Animation(
            `${boneName}_rotation`,
            'rotationQuaternion',
            FRAME_RATE,
            Animation.ANIMATIONTYPE_QUATERNION,
            Animation.ANIMATIONLOOPMODE_CYCLE
        );
        animation.setKeys(keys);
        return animation;
    }

    /**
     * 创建缩放动画
     * 将MDX的KGSC轨道转换为Babylon缩放动画
     * 
     * @param objectData - 动画对象数据
     * @param boneName - 骨骼名称
     * @returns 缩放动画对象，无动画数据则返回null
     */
    private createScalingAnimation(
        objectData: AnimatedObject,
        boneName: string,
        sequences: Sequence[]
    ): Nullable<Animation> {
        // 缩放不需要坐标系转换
        const converter = (vec: Float32Array) => new Vector3(vec[0], vec[1], vec[2]);

        return this.createVectorAnimation(
            objectData,
            AnimationTrackType.SCALING,
            `${boneName}_scaling`,
            'scaling',
            converter,
            sequences,
            new Vector3(1, 1, 1)
        );
    }

    /**
     * 创建向量类型动画（通用方法）
     * 处理位置和缩放动画的创建逻辑
     * 
     * @param objectData - 动画对象数据
     * @param token - 动画轨道类型标识
     * @param name - 动画名称
     * @param property - 要动画的属性名（'position'或'scaling'）
     * @param converter - 坐标转换函数
     * @param baseOffset - 基础偏移量（可选）
     * @returns 向量动画对象，无动画数据则返回null
     */
    private createVectorAnimation(
        objectData: AnimatedObject,
        token: string,
        name: string,
        property: 'position' | 'scaling',
        converter: (vec: Float32Array) => Vector3,
        sequences: Sequence[],
        defaultValue: Vector3,
        baseOffset?: Vector3
    ): Nullable<Animation> {
        const animData = this.findAnimation(objectData, token);
        if (!animData || !animData.frames.length) {
            return null;
        }

        const keys = this.convertVectorKeys(animData, converter, sequences, defaultValue, baseOffset);
        const animation = new Animation(
            name,
            property,
            FRAME_RATE,
            Animation.ANIMATIONTYPE_VECTOR3,
            Animation.ANIMATIONLOOPMODE_CYCLE
        );
        animation.setKeys(keys);
        return animation;
    }

    /**
     * 转换向量关键帧
     * 将MDX的向量动画数据转换为Babylon的关键帧格式
     * 
     * @param animData - MDX动画数据
     * @param converter - 坐标转换函数
     * @param baseOffset - 基础偏移量（可选）
     * @returns 关键帧数组
     */
    private convertVectorKeys(
        animData: MdlxAnimation,
        converter: (vec: Float32Array) => Vector3,
        sequences: Sequence[],
        defaultValue: Vector3,
        baseOffset?: Vector3
    ): TypedAnimationKey<Vector3>[] {
        const rawKeys: TypedAnimationKey<Vector3>[] = [];
        const frames = animData.frames;
        const values = animData.values as Float32Array[];

        for (let i = 0; i < frames.length; i++) {
            const frame = frames[i];
            const value = converter(values[i]);

            // 如果有基础偏移，应用偏移
            if (baseOffset) {
                value.addInPlace(baseOffset);
            }

            rawKeys.push({ frame, value });

            // 处理DontInterp插值类型：在下一帧前插入重复关键帧，实现阶跃效果
            if (this.shouldAddDontInterpKey(animData, i, frames.length)) {
                const nextFrame = frames[i + 1] - 0.001;
                rawKeys.push({ frame: nextFrame, value: value.clone() });
            }
        }

        if (
            animData.interpolationType === InterpolationType.Hermite
            || animData.interpolationType === InterpolationType.Bezier
        ) {
            this.applyVectorTangents(rawKeys, animData, converter, baseOffset);
        }

        return animData.globalSequenceId >= 0
            ? rawKeys
            : this.scopeVectorKeysToSequences(rawKeys, sequences, defaultValue);
    }

    private applyVectorTangents(
        keys: TypedAnimationKey<Vector3>[],
        animData: MdlxAnimation,
        converter: (vec: Float32Array) => Vector3,
        baseOffset?: Vector3,
    ): void {
        const inTans = animData.inTans as Float32Array[];
        const outTans = animData.outTans as Float32Array[];

        for (let index = 0; index < keys.length - 1; index++) {
            const left = keys[index];
            const right = keys[index + 1];
            const frameDelta = right.frame - left.frame;
            if (frameDelta <= 0 || !outTans[index] || !inTans[index + 1]) continue;

            const rawOut = converter(outTans[index]);
            const rawIn = converter(inTans[index + 1]);
            if (animData.interpolationType === InterpolationType.Bezier) {
                if (baseOffset) {
                    rawOut.addInPlace(baseOffset);
                    rawIn.addInPlace(baseOffset);
                }
                left.outTangent = rawOut.subtract(left.value).scale(3 / frameDelta);
                right.inTangent = right.value.subtract(rawIn).scale(3 / frameDelta);
            } else {
                left.outTangent = rawOut.scale(1 / frameDelta);
                right.inTangent = rawIn.scale(1 / frameDelta);
            }
        }
    }

    /**
     * 转换四元数关键帧
     * 将MDX的旋转动画数据转换为Babylon的四元数关键帧格式，并应用镜像变换
     *
     * @param animData - MDX动画数据
     * @returns 四元数关键帧数组
     */
    private convertQuaternionKeys(
        animData: MdlxAnimation,
        sequences: Sequence[]
    ): TypedAnimationKey<Quaternion>[] {
        const rawKeys: TypedAnimationKey<Quaternion>[] = [];
        const frames = animData.frames;
        const values = animData.values as Float32Array[];

        for (let i = 0; i < frames.length; i++) {
            const frame = frames[i];
            const quatArray = values[i];
            const quaternion = new Quaternion(quatArray[0], quatArray[1], quatArray[2], quatArray[3]);

            // 应用镜像变换以适配Babylon的坐标系
            const mirrored = war3ToBabylonQuaternion(quaternion);

            rawKeys.push({ frame, value: mirrored });

            // 处理DontInterp插值类型
            if (this.shouldAddDontInterpKey(animData, i, frames.length)) {
                const nextFrame = frames[i + 1] - 0.001;
                rawKeys.push({ frame: nextFrame, value: mirrored.clone() });
            }
        }

        return animData.globalSequenceId >= 0
            ? rawKeys
            : this.scopeQuaternionKeysToSequences(rawKeys, sequences, Quaternion.Identity());
    }

    private scopeVectorKeysToSequences(
        rawKeys: TypedAnimationKey<Vector3>[],
        sequences: Sequence[],
        defaultValue: Vector3
    ): TypedAnimationKey<Vector3>[] {
        return this.scopeKeysToSequences(rawKeys, sequences, defaultValue, (value) => value.clone());
    }

    private scopeQuaternionKeysToSequences(
        rawKeys: TypedAnimationKey<Quaternion>[],
        sequences: Sequence[],
        defaultValue: Quaternion
    ): TypedAnimationKey<Quaternion>[] {
        return this.scopeKeysToSequences(rawKeys, sequences, defaultValue, (value) => value.clone());
    }

    private scopeKeysToSequences<T>(
        rawKeys: TypedAnimationKey<T>[],
        sequences: Sequence[],
        defaultValue: T,
        cloneValue: (value: T) => T
    ): TypedAnimationKey<T>[] {
        if (!sequences.length) {
            return rawKeys;
        }

        const scoped: TypedAnimationKey<T>[] = [];

        for (const sequence of sequences) {
            const start = sequence.interval[0];
            const end = sequence.interval[1];
            const inside = rawKeys.filter((key) => key.frame >= start && key.frame <= end);

            if (!inside.length) {
                scoped.push({ frame: start, value: cloneValue(defaultValue) });
                scoped.push({ frame: end, value: cloneValue(defaultValue) });
                continue;
            }

            const constantValue = inside.length === 1 ? inside[0].value : null;
            const startValue = constantValue ?? inside[0].value;
            const endValue = constantValue ?? inside[inside.length - 1].value;

            if (inside[0].frame > start) {
                scoped.push({ frame: start, value: cloneValue(startValue) });
            }

            for (const key of inside) {
                scoped.push({
                    ...key,
                    value: cloneValue(key.value),
                    inTangent: key.inTangent ? cloneValue(key.inTangent) : undefined,
                    outTangent: key.outTangent ? cloneValue(key.outTangent) : undefined,
                });
            }

            if (inside[inside.length - 1].frame < end) {
                scoped.push({ frame: end, value: cloneValue(endValue) });
            }
        }

        scoped.sort((a, b) => a.frame - b.frame);
        return scoped;
    }

    /**
     * 判断是否需要添加DontInterp关键帧
     * DontInterp类型的动画不进行插值，需要在下一帧前添加重复关键帧
     *
     * @param animData - 动画数据
     * @param currentIndex - 当前关键帧索引
     * @param totalFrames - 总关键帧数量
     * @returns 是否需要添加DontInterp关键帧
     */
    private shouldAddDontInterpKey(
        animData: MdlxAnimation,
        currentIndex: number,
        totalFrames: number
    ): boolean {
        return animData.interpolationType === InterpolationType.DontInterp
            && currentIndex < totalFrames - 1;
    }
}
