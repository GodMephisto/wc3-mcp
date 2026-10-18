---
type: vendor-doc
source_repo: "https://github.com/nightknighto/Warcraft-3-Reforged-UI-Designer"
source_path: "src/ts/app.ts"
ref: HEAD
---

import { app, BrowserWindow } from 'electron'
import Main from './Main'

Main.init(app, BrowserWindow, { width: 1500, height: 950, devTools: true })
