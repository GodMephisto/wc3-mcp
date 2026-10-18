---
type: vendor-doc
source_repo: "https://github.com/nightknighto/Warcraft-3-Reforged-UI-Designer"
source_path: "src/ts/Commands/Undo.ts"
ref: HEAD
---

import { ICallableDivInstance } from '../ClassesAndFunctions/ICallableDivInstance'
import ChangeStack from '../Editor/ChangeStack'

export default class Undo implements ICallableDivInstance {
    public run(): void {
        ChangeStack.getInstance().undo()
    }
}
