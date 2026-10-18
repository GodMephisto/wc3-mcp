---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/parsers/mdlx/animatedobject.ts"
ref: HEAD
---

import BinaryStream from '../common/binarystream';
import TokenStream from './tokenstream';
import { Animation } from './animations';
import animationMap, { type AnimationMapEntry, type AnimationMapKey } from './animationmap';

const resolveAnimationEntry = (name: string): AnimationMapEntry => {
    const entry = animationMap[name as AnimationMapKey];
    if (!entry) {
        throw new Error(`未知动画标记: ${name}`);
    }
    return entry;
};

/**
 * The parent class for all objects that have animated data in them.
 */
export default class AnimatedObject {
    animations: Animation[] = [];

    readAnimations(stream: BinaryStream, size: number): void {
        const end = stream.index + size;

        while (stream.index < end) {
            const name = stream.readBinary(4);
            const [, AnimationCtor] = resolveAnimationEntry(name);
            const animation = new AnimationCtor();

            animation.readMdx(stream, name);

            this.animations.push(animation);
        }
    }

    writeAnimations(stream: BinaryStream): void {
        for (const animation of this.animations) {
            animation.writeMdx(stream);
        }
    }

    /**
     * A wrapper around readBlock() which merges static tokens.
     * E.g.: static Color
     * This makes the condition blocks in the parent objects linear and simple.
     */
    * readAnimatedBlock(stream: TokenStream): Generator<string> {
        for (const token of stream.readBlock()) {
            if (token === 'static') {
                yield `static ${stream.read()}`;
            } else {
                yield token;
            }
        }
    }

    readAnimation(stream: TokenStream, name: string): void {
        const [, AnimationCtor] = resolveAnimationEntry(name);
        const animation = new AnimationCtor();

        animation.readMdl(stream, name);

        this.animations.push(animation);
    }

    writeAnimation(stream: TokenStream, name: string): boolean {
        for (const animation of this.animations) {
            if (animation.name === name) {
                const [token] = resolveAnimationEntry(name);
                animation.writeMdl(stream, token);
                return true;
            }
        }

        return false;
    }

    /**
     * AnimatedObject itself doesn't care about versions, however objects that inherit it do.
     */
    getByteLength(_version = 0): number {
        let size = 0;

        for (const animation of this.animations) {
            size += animation.getByteLength();
        }

        return size;
    }
}
