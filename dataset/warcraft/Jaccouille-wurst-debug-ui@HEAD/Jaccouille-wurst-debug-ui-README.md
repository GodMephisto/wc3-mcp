---
type: vendor-doc
source_repo: "https://github.com/Jaccouille/wurst-debug-ui"
source_path: "README.md"
ref: HEAD
---

# Wurst Debug User Interface
This repository show case an example of debug UI for Warcraft 3 coded in Wurst.
It features an object previewer class used to place Warcraft 3 game objects
(Units, Items, Destructable in the future?).

The goal is to make the debugging phase more comfortable by spawning objects
through mouse click rather than type debugging commands.

![22-05-21-20-28-45](https://user-images.githubusercontent.com/7768858/169666522-920610ea-5330-43c0-80d9-07f9a9302875.gif)


## Installation
Add this repository to your wurst dependencies by adding this line in your wurst.build file under `dependencies` section

`- https://github.com/Frotty/wurst-table-layout:main`

Or execute this command in a terminal:

`grill install https://github.com/Jaccouille/wurst-debug-ui:main`

Then 'reload window' in vscode.

## How to use

In a wurst file:

```
import ObjectSpawner


init
    createObjectSpawner('hfoo', debugTypes.UNIT)
```
Will create a footman preview model located at your mouse cursor position, just like in the World Editor. You can press left-click to create the unit at your cursor position or right-click to cancel the spawn instance/previewer.

Items can be spawned with the following call:<br>
```createObjectSpawner(yourItemId, debugTypes.ITEM)```

You can change the unit owner by pressing `0 to 9` hotkey.<br>
You can rotate the object by pressing `*`.<br>
You can increase/decrease the spawn count of the object by pressing `+` or `-`.

## How is this possible?
ObjectSpawner package simply create a unit/item at cursor position and update its position on cursor movement.

## User Interface
The debug user interface was made with https://github.com/Frotty/wurst-table-layout<br>
Code is here [DebugUI](https://github.com/Jaccouille/wurst-debug-ui/blob/main/wurst/DebugUI.wurst), this was made for showcase purpose, go and do your own implementation.

### TODO
Maybe add some to way to debug abilities, use a dummy unit to cast the abilities, if it's a passive, click on a unit to give him the passive ability.
