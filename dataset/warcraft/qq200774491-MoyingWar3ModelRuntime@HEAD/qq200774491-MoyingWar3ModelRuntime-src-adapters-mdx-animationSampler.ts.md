---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/adapters/mdx/animationSampler.ts"
ref: HEAD
---

import type { Animation } from '../../parsers/mdlx/animations';
import { InterpolationType } from '../../parsers/mdlx/animations';
import type Sequence from '../../parsers/mdlx/sequence';

export interface AnimationSampleClock {
    frame: number;
    sequence: Sequence | null;
    globalTimeMs: number;
    globalSequences: readonly number[];
}

export interface ScalarKeyframe {
    index: number;
    frame: number;
    value: number;
}

interface TrackWindow {
    frame: number;
    first: number;
    last: number;
}

export function findAnimation(
    animations: readonly Animation[],
    name: string,
): Animation | undefined {
    return animations.find((animation) => animation.name === name);
}

export function sampleScalarAnimation(
    animation: Animation | undefined,
    defaultValue: number,
    clock: AnimationSampleClock,
): number {
    if (!animation?.frames.length) {
        return defaultValue;
    }

    const window = getTrackWindow(animation, clock);
    if (!window) {
        return defaultValue;
    }

    const { frame, first, last } = window;
    if (first === last || frame <= animation.frames[first]) {
        return scalarValue(animation, first, defaultValue);
    }
    if (frame >= animation.frames[last]) {
        return scalarValue(animation, last, defaultValue);
    }

    const right = upperBound(animation.frames, frame, first, last + 1);
    const left = Math.max(first, right - 1);
    if (right > last) {
        return scalarValue(animation, left, defaultValue);
    }

    const leftFrame = animation.frames[left];
    const rightFrame = animation.frames[right];
    const span = rightFrame - leftFrame;
    if (span <= 0 || animation.interpolationType === InterpolationType.DontInterp) {
        return scalarValue(animation, left, defaultValue);
    }

    const t = clamp((frame - leftFrame) / span, 0, 1);
    const a = scalarValue(animation, left, defaultValue);
    const b = scalarValue(animation, right, a);

    if (animation.interpolationType === InterpolationType.Hermite) {
        const outTan = scalarArrayValue(animation.outTans, left, a);
        const inTan = scalarArrayValue(animation.inTans, right, b);
        return hermite(a, outTan, inTan, b, t);
    }
    if (animation.interpolationType === InterpolationType.Bezier) {
        const outTan = scalarArrayValue(animation.outTans, left, a);
        const inTan = scalarArrayValue(animation.inTans, right, b);
        return bezier(a, outTan, inTan, b, t);
    }

    return lerp(a, b, t);
}

export function sampleVectorAnimation(
    animation: Animation | undefined,
    defaultValue: ArrayLike<number>,
    clock: AnimationSampleClock,
): Float32Array {
    const result = Float32Array.from(defaultValue);
    if (!animation?.frames.length) {
        return result;
    }

    const window = getTrackWindow(animation, clock);
    if (!window) {
        return result;
    }

    const { frame, first, last } = window;
    if (first === last || frame <= animation.frames[first]) {
        return vectorValue(animation.values, first, result);
    }
    if (frame >= animation.frames[last]) {
        return vectorValue(animation.values, last, result);
    }

    const right = upperBound(animation.frames, frame, first, last + 1);
    const left = Math.max(first, right - 1);
    if (right > last) {
        return vectorValue(animation.values, left, result);
    }

    const leftFrame = animation.frames[left];
    const rightFrame = animation.frames[right];
    const span = rightFrame - leftFrame;
    const a = vectorValue(animation.values, left, result);
    if (span <= 0 || animation.interpolationType === InterpolationType.DontInterp) {
        return a;
    }

    const t = clamp((frame - leftFrame) / span, 0, 1);
    const b = vectorValue(animation.values, right, a);
    const value = new Float32Array(result.length);
    const outTan = vectorValue(animation.outTans, left, a);
    const inTan = vectorValue(animation.inTans, right, b);

    for (let component = 0; component < value.length; component++) {
        if (animation.interpolationType === InterpolationType.Hermite) {
            value[component] = hermite(a[component], outTan[component], inTan[component], b[component], t);
        } else if (animation.interpolationType === InterpolationType.Bezier) {
            value[component] = bezier(a[component], outTan[component], inTan[component], b[component], t);
        } else {
            value[component] = lerp(a[component], b[component], t);
        }
    }

    return value;
}

export function findScalarKeyframe(
    animation: Animation | undefined,
    clock: AnimationSampleClock,
): ScalarKeyframe | null {
    if (!animation?.frames.length) {
        return null;
    }

    const window = getTrackWindow(animation, clock);
    if (!window) {
        return null;
    }

    const { frame, first, last } = window;
    let index = upperBound(animation.frames, frame, first, last + 1) - 1;
    if (index < first) index = first;
    if (index > last) index = last;

    return {
        index,
        frame: animation.frames[index],
        value: scalarValue(animation, index, 0),
    };
}

export function getAnimationSampleFrame(
    animation: Animation,
    clock: AnimationSampleClock,
): number | null {
    return getTrackWindow(animation, clock)?.frame ?? null;
}

function getTrackWindow(
    animation: Animation,
    clock: AnimationSampleClock,
): TrackWindow | null {
    let frame = clock.frame;
    let from = Number.NEGATIVE_INFINITY;
    let to = Number.POSITIVE_INFINITY;

    if (animation.globalSequenceId >= 0) {
        const duration = clock.globalSequences[animation.globalSequenceId];
        if (!Number.isFinite(duration) || duration <= 0) {
            return null;
        }
        frame = positiveModulo(clock.globalTimeMs, duration);
        from = 0;
        to = duration;
    } else if (clock.sequence) {
        from = clock.sequence.interval[0];
        to = clock.sequence.interval[1];
    }

    const first = lowerBound(animation.frames, from, 0, animation.frames.length);
    const last = upperBound(animation.frames, to, first, animation.frames.length) - 1;
    if (first >= animation.frames.length || last < first) {
        return null;
    }

    return { frame, first, last };
}

function scalarValue(animation: Animation, index: number, fallback: number): number {
    return scalarArrayValue(animation.values, index, fallback);
}

function scalarArrayValue(
    values: readonly (Uint32Array | Float32Array)[],
    index: number,
    fallback: number,
): number {
    const value = values[index]?.[0];
    return Number.isFinite(value) ? value : fallback;
}

function vectorValue(
    values: readonly (Uint32Array | Float32Array)[],
    index: number,
    fallback: Float32Array,
): Float32Array {
    const source = values[index];
    const value = new Float32Array(fallback.length);
    for (let component = 0; component < value.length; component++) {
        const candidate = source?.[component];
        value[component] = Number.isFinite(candidate) ? candidate : fallback[component];
    }
    return value;
}

function lowerBound(
    values: readonly number[],
    target: number,
    start: number,
    end: number,
): number {
    let low = start;
    let high = end;
    while (low < high) {
        const middle = low + ((high - low) >> 1);
        if (values[middle] < target) low = middle + 1;
        else high = middle;
    }
    return low;
}

function upperBound(
    values: readonly number[],
    target: number,
    start: number,
    end: number,
): number {
    let low = start;
    let high = end;
    while (low < high) {
        const middle = low + ((high - low) >> 1);
        if (values[middle] <= target) low = middle + 1;
        else high = middle;
    }
    return low;
}

function hermite(a: number, outTan: number, inTan: number, b: number, t: number): number {
    const t2 = t * t;
    const t3 = t2 * t;
    return (2 * t3 - 3 * t2 + 1) * a
        + (t3 - 2 * t2 + t) * outTan
        + (-2 * t3 + 3 * t2) * b
        + (t3 - t2) * inTan;
}

function bezier(a: number, outTan: number, inTan: number, b: number, t: number): number {
    const inverse = 1 - t;
    return inverse * inverse * inverse * a
        + 3 * inverse * inverse * t * outTan
        + 3 * inverse * t * t * inTan
        + t * t * t * b;
}

function lerp(a: number, b: number, t: number): number {
    return a + (b - a) * t;
}

function positiveModulo(value: number, modulus: number): number {
    return ((value % modulus) + modulus) % modulus;
}

function clamp(value: number, minimum: number, maximum: number): number {
    return Math.min(maximum, Math.max(minimum, value));
}
