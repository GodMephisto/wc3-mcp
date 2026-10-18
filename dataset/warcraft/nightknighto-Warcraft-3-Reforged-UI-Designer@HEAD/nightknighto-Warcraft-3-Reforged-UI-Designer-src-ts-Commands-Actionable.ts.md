---
type: vendor-doc
source_repo: "https://github.com/nightknighto/Warcraft-3-Reforged-UI-Designer"
source_path: "src/ts/Commands/Actionable.ts"
ref: HEAD
---

export default interface Actionable {
    action(): void
    pureAction(): void
}
