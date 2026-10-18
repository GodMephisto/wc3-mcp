---
type: vendor-doc
source_repo: "https://github.com/nightknighto/Warcraft-3-Reforged-UI-Designer"
source_path: "src/ts/Commands/Redo.ts"
ref: HEAD
---

import { ICallableDivInstance } from '../ClassesAndFunctions/ICallableDivInstance'
import ChangeStack from '../Editor/ChangeStack'

export default class Redo implements ICallableDivInstance {
    public run(): void {
        ChangeStack.getInstance().redo()
    }
}
