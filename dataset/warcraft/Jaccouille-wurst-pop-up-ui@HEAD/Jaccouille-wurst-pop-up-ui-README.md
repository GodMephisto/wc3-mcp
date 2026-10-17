---
type: vendor-doc
source_repo: "https://github.com/Jaccouille/wurst-pop-up-ui"
source_path: "README.md"
ref: HEAD
---

# wurst-pop-up-ui
Create a pop up frame upon triggered event

[Custom fdf file](https://github.com/Jaccouille/wurst-pop-up-ui/blob/main/imports/PopUp.fdf) define a frame with 1 background and 3 icons, centered icon has its borders removed for clarity, remove [this line](https://github.com/Jaccouille/wurst-pop-up-ui/blob/main/imports/PopUp.fdf#L32) if you want to keep the borders


For some reasons it doesn't work as a dependency, you need to add the .toc and .fdf files from the imports folder into your own project's imports folder.

Demo: https://github.com/Jaccouille/wurst-pop-up-ui/blob/main/wurst/PopUpTest.wurst
![](https://thumbs.gfycat.com/BlissfulLoathsomeHarvestmen-size_restricted.gif)

Still a bit clunky, haven't tested on multiplayer yet.
Only tested on Patch 1.32.9
