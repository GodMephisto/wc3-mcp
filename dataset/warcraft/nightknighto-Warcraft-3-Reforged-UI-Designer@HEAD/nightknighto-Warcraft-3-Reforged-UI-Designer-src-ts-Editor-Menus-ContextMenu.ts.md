---
type: vendor-doc
source_repo: "https://github.com/nightknighto/Warcraft-3-Reforged-UI-Designer"
source_path: "src/ts/Editor/Menus/ContextMenu.ts"
ref: HEAD
---

import { Menu, MenuItem, BrowserWindow } from 'electron'

export class ContextMenu {
    private mainWindow: BrowserWindow
    private contextMenu: Menu

    constructor(mainWindow: BrowserWindow) {
        this.mainWindow = mainWindow
        this.contextMenu = new Menu()
        this.populateContextMenu()
    }

    private populateContextMenu(): void {
        let item = new MenuItem({ label: 'Delete', id: 'Delete' })
        item.click = () => {
            this.mainWindow.webContents.send('Delete')
        }
        this.contextMenu.append(item)

        item = new MenuItem({ label: 'Duplicate', id: 'Duplicate' })
        item.click = () => {
            this.mainWindow.webContents.send('Duplicate')
        }
        this.contextMenu.append(item)

        item = new MenuItem({ label: 'CircularArray', id: 'CircularArray' })
        item.click = () => {
            this.mainWindow.webContents.send('CircularArray')
        }
        this.contextMenu.append(item)

        item = new MenuItem({ label: 'TableArray', id: 'TableArray' })
        item.click = () => {
            this.mainWindow.webContents.send('TableArray')
        }
        this.contextMenu.append(item)
    }

    public showContextMenu(): void {
        this.contextMenu.popup()
    }
}
