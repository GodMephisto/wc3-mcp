---
type: vendor-doc
source_repo: "https://github.com/nightknighto/Warcraft-3-Reforged-UI-Designer"
source_path: "src/ts/Commands/SimpleCommand.ts"
ref: HEAD
---

import ChangeStack from '../Editor/ChangeStack'
import Actionable from './Actionable'
import Redoable from './Redoable'

export default abstract class SimpleCommand implements Actionable, Redoable {
    public redo(): void {
        ChangeStack.getInstance().pushUndoChange(this, false)
        this.pureAction()
    }

    public undo(): void {
        //When implementing, your own undo method, you MUST use super.undo() method call to update the changeStack
        ChangeStack.getInstance().pushRedoChange(this)
    }

    public abstract pureAction(): void

    public action(): void {
        ChangeStack.getInstance().pushUndoChange(this, true)
        this.pureAction()
    }
}
