---
type: vendor-doc
source_repo: "https://github.com/nightknighto/Warcraft-3-Reforged-UI-Designer"
source_path: "src/ts/Commands/Redoable.ts"
ref: HEAD
---

import Undoable from './Undoable'

export default interface Redoable extends Undoable {
    redo(): void
}
