---
type: vendor-doc
source_repo: "https://github.com/nightknighto/Warcraft-3-Reforged-UI-Designer"
source_path: "src/ts/ClassesAndFunctions/ICallableDivInstance.ts"
ref: HEAD
---

/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
/* eslint-disable @typescript-eslint/no-explicit-any */
export abstract class ICallableDivInstance {
    public run = () => {
        // Empty
    }

    public static call(ev: Event): void {
        ;((ev.currentTarget as any).instance as ICallableDivInstance).run()
    }

    public static setup(element: any, object: ICallableDivInstance): void {
        element.instance = object
    }
}
