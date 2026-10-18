---
type: vendor-doc
source_repo: "https://github.com/ChiefOfGxBxL/WC3MapTranslator"
source_path: "src/AngleConverter.ts"
ref: HEAD
---

import { angle } from './CommonInterfaces';

export function deg2Rad(angleInDegrees: angle): number {
    return angleInDegrees * Math.PI / 180;
}

export function rad2Deg(angleInRadians: number): angle {
    return angleInRadians * 180 / Math.PI;
}
