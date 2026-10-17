---
type: vendor-doc
source_repo: "https://github.com/Gohagga/STK_vJass"
source_path: "README.md"
ref: HEAD
---

# STK_vJass

Talent system for Warcraft III games written in vJass, superset of a Warcraft III proprietary scripting syntax, JASS2.

## Features

- **Single rank** and **multi rank** talents. **Each rank is defined separately** and can have its own text/icon/behavior
- **Talent point tracking**, each talent/rank can have a **varying talent point cost** (0, 1 or more)
- **Requirements** (custom logic based) talent disable, the user defined text reason will be **shown in the tooltip**
- **Talent Dependencies**, in cardinal direction, talent can require its adjacent talents to have a specified level, this is shown in interface as "links" or "lines"
- **Temporary talent point allocation**, points can be put into tree and reset as many times as needed before "Confirm" is clicked, only then all the effects will apply
- **Custom Logic binding** on **Activation** (when talent effects are applied), **Allocation** (when talent is taken before confirming) and **Requirements**
- Talent-tree based **configurable rows and columns count**, for example Pyromancer can have 6 rows, 3 columns and Fighter can have 4x4
- **Grid based** talent positioning (X, Y) and **automatic button placement** based on rows/columns
- Can support **multiple talent tree panels** opened at the same time such as 3-spec in wow classic
- **Talent tree reset**, deactivates all talents and refunds all points spent.
- Provides **Save-Load API** for integration with save-load systems

## To Install

### Set up the system

Copy all scripts from /STK folder into your map.
Adjust code in your Setup trigger so it works for your map.

### Assign talent trees to a hero

When a hero is chosen, first create a talent tree for them and store it into an integer variable

> set udg_TempTalentTree =  Shepherd_Shepherd.create(GetTriggerUnit())

Then initialize the talent tree and set the unit owner player's UI to watch that talent tree.

> call STK_AssignTalentTree(1, GetTriggerUnit(), udg_TempTalentTree)

If a different player has to see/modify the same talent tree, adjust and execute next

> call STK_PlayerLookAtUnitsTree(Player(0), GetTriggerUnit())

Make sure it works and then continue onto the actual consumption of the system.

When your player is looking at a certain unit's talent tree, call this line to open the UI
> call STK_OpenTalentScreen(GetTriggerPlayer())

Watch out, it might not work on Map Initialization, so try 0 seconds passed.

### Create a talent tree implementation and hook it up

/Example/Shepherd script contains one way of implementing a talent tree struct. Define the name, add talents, define their effects etc. Hook it up to the system.