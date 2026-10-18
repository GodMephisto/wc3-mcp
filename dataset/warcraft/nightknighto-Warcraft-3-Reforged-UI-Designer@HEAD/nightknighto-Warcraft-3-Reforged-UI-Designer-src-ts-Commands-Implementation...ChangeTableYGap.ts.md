---
type: vendor-doc
source_repo: "https://github.com/nightknighto/Warcraft-3-Reforged-UI-Designer"
source_path: "src/ts/Commands/Implementation/Arrays/ChangeTableYGap.ts"
ref: HEAD
---

import { debugText } from '../../../ClassesAndFunctions/MiniFunctions'
import SimpleCommand from '../../SimpleCommand'
import TableArray from '../../../Editor/FrameLogic/Arrays/TableArray'

export default class ChangeTableYGap extends SimpleCommand {
    private array: TableArray
    private newGap: number
    private oldGap: number

    public constructor(array: TableArray, YGap: number) {
        super()

        this.array = array
        this.newGap = YGap
        this.oldGap = array.getYGap()

        return this
    }

    public pureAction(): void {
        this.array.setYGap(this.newGap)
    }

    public undo(): void {

        super.undo()
        const undoCommand = new ChangeTableYGap(this.array, this.oldGap);
        undoCommand.pureAction()
        debugText('Undid YGap change.')
    }

    public redo(): void {
        super.redo()
        debugText('Redid YGap change.')
    }
}
