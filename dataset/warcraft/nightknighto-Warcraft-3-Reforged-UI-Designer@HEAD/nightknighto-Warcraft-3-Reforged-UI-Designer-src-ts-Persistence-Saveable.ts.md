---
type: vendor-doc
source_repo: "https://github.com/nightknighto/Warcraft-3-Reforged-UI-Designer"
source_path: "src/ts/Persistence/Saveable.ts"
ref: HEAD
---

import SaveContainer from './SaveContainer'

export default interface Saveable {
    save(container: SaveContainer): void
}
