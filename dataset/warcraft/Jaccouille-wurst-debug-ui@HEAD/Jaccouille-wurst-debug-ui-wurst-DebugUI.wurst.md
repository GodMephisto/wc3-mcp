---
type: vendor-doc
source_repo: "https://github.com/Jaccouille/wurst-debug-ui"
source_path: "wurst/DebugUI.wurst"
ref: HEAD
---

package DebugUI

import TableLayout
import ClosureTimers
import LinkedList
import ObjectSpawner
import idsList

let humanUnit         = new PaletteSection("Units"         , humanUnitList)
let humanHero         = new PaletteSection("Heroes"        , humanHeroList)
let humanBuildings    = new PaletteSection("Buildings"     , humanBuildingsList)
let humanSpecial      = new PaletteSection("Special"       , humanSpecialList)
let orcUnit           = new PaletteSection("Units"         , orcUnitList)
let orcHero           = new PaletteSection("Heroes"        , orcHeroList)
let orcBuildings      = new PaletteSection("Buildings"     , orcBuildingsList)
let orcSpecial        = new PaletteSection("Special"       , orcSpecialList)
let undeadUnit        = new PaletteSection("Units"         , undeadUnitList)
let undeadHero        = new PaletteSection("Heroes"        , undeadHeroList)
let undeadBuildings   = new PaletteSection("Buildings"     , undeadBuildingsList)
let undeadSpecial     = new PaletteSection("Special"       , undeadSpecialList)
let nightElfUnit      = new PaletteSection("Units"         , nightElfUnitList)
let nightElfHero      = new PaletteSection("Heroes"        , nightElfHeroList)
let nightElfBuildings = new PaletteSection("Buildings"     , nightElfBuildingsList)
let nightElfSpecial   = new PaletteSection("Special"       , nightElfSpecialList)
let artifact          = new PaletteSection("Artifact"      , artifactList)
let campaign          = new PaletteSection("Campaign"      , campaignList)

let DOC_TEXT = "" +
"Click to show/hide the debug panel.\n"+
"This is a panel emulating the world editor Unit/Item window palette.\n"+
"You can select objects and place them on the terrain.\n\n"+
"Hotkeys\n".color(color(255,  99,  71))+
"Following hotkeys are similar to those from the editor.\n"+
"Press 0 to 9 to change the unit owner (Player 1, 2 3...).\n"+
"Press * to rotate the unit/item angle.\n"+
"Press + or - to increase/decrease the amount of object to spawn."

class PaletteSection
    string header
    LinkedList<int> idsList

    construct(string header, LinkedList<int> idsList)
        this.header = header
        this.idsList = idsList


constant SECTION_TOP_SPACING = 0.0045
constant SECTION_BOTTOM_SPACING = 0.0025
constant FRAME_SIZE = vec2(0.3, 0.28)
constant MAX_CELL = 10
constant CONTENT_FRAME_SIZE = vec2(0.005, -0.08)


function TableLayout.addPaletteSection(PaletteSection content, int maxCell, debugTypes dt)
    this.padBot(0.01)
    this.row()
    this.add(p(content.header))
    this.row()
    for elem in content.idsList
        if content.idsList.indexOf(elem) % maxCell == 0
            this..row()
        let btnFrame = imgBtn(BlzGetAbilityIcon(elem))
        btnFrame.createToolTipFrame(GetAbilityName(elem), BlzGetAbilityExtendedTooltip(elem, 1))
        btnFrame.onClick(() -> createObjectSpawner(elem, dt))
        this.add(btnFrame)


function framehandle.createToolTipFrame(string title, string textContent)
    let tooltip = createFrame("BoxedText", GAME_UI, 0, 0)
    let text = getFrame("BoxedTextValue")

    getFrame("BoxedTextTitle").setText(title)

    text..setText(textContent)
        ..setSize(0.3, 0)
        ..setPoint(FRAMEPOINT_TOPRIGHT, this, FRAMEPOINT_TOPLEFT, -0.001, -0.052)
    tooltip.setPoint(FRAMEPOINT_TOPLEFT, text, FRAMEPOINT_TOPLEFT, -0.01, 0.01)
    tooltip.setPoint(FRAMEPOINT_BOTTOMRIGHT, text, FRAMEPOINT_BOTTOMRIGHT, 0.01, -0.025)
    this.setTooltip(tooltip)

function createFrameTemplate(string name, vec2 size, vec2 pos, framehandle parent) returns framehandle
    return createFrame("FRAME", name, parent, "", 0)
            ..setPoint(FRAMEPOINT_TOP, parent, FRAMEPOINT_TOP, pos)
            ..setSize(size.x, size.y)
            ..hide()

function showFrame(framehandle baseFrame, string frameName)
    for idx = 0 to baseFrame.getChildrenCount()
        let childFrame = baseFrame.getChild(idx)
        if childFrame.getName().startsWith("contentFrame")
            childFrame.hide()
        if childFrame.getName() == frameName
            childFrame.show()


init
    if (loadTOCFile("Templates.toc" )) == false
        print("Loading Templates Toc file failed")
    doAfter(1) ->
        let baseFrame = createFrame("FRAME", "baseFrame", GAME_UI, "", 0)
            ..setAbsPoint(FRAMEPOINT_TOP, vec2(0.6, 0.545))
            ..setSize(0.3, 0.38)

        createFrame("HeroPickMenuBackdrop", baseFrame, 0, 0)
            ..setAllPoints(baseFrame)

        createFrame("ScriptDialogButton", GAME_UI, 0, 0)
        ..setSize(0.12, 0.025)
        ..createToolTipFrame("Documentation", DOC_TEXT)
        ..setPoint(FRAMEPOINT_TOP, baseFrame, FRAMEPOINT_TOP, 0.0, 0.02)
        ..onClick() ->
            if baseFrame.isVisible()
                baseFrame.hide()
            else
                baseFrame.show()

        getFrame("ScriptDialogButtonText", 0)
            ..setText("Debug Panel")


        let headerFrame = createFrame("FRAME", "headerFrame", baseFrame, "", 0)
            ..setPoint(FRAMEPOINT_TOP, baseFrame, FRAMEPOINT_TOP)
            ..setSize(0.3, 0.07)
            ..show()

        let contentFrames = new LinkedList<framehandle>
            ..add(createFrameTemplate("contentFrameHuman"    , FRAME_SIZE, CONTENT_FRAME_SIZE, baseFrame)..show())
            ..add(createFrameTemplate("contentFrameOrc"      , FRAME_SIZE, CONTENT_FRAME_SIZE, baseFrame))
            ..add(createFrameTemplate("contentFrameUndead"   , FRAME_SIZE, CONTENT_FRAME_SIZE, baseFrame))
            ..add(createFrameTemplate("contentFrameNightElf" , FRAME_SIZE, CONTENT_FRAME_SIZE, baseFrame))
            ..add(createFrameTemplate("contentFrameItem"     , FRAME_SIZE, CONTENT_FRAME_SIZE, baseFrame))

        new TableLayout(0.3, 0.07)
            ..row()
            ..add(btn("Human")   ..setWidth(0.06)..onClick(() -> showFrame(baseFrame, contentFrames.get(0).getName()) ) )
            ..add(btn("Orc")     ..setWidth(0.06)..onClick(() -> showFrame(baseFrame, contentFrames.get(1).getName()) ) )
            ..add(btn("Undead")  ..setWidth(0.06)..onClick(() -> showFrame(baseFrame, contentFrames.get(2).getName()) ) )
            ..add(btn("NightElf")..setWidth(0.06)..onClick(() -> showFrame(baseFrame, contentFrames.get(3).getName()) ) )
            ..row()
            ..add(btn("Items")   ..setWidth(0.06)..onClick(() -> showFrame(baseFrame, contentFrames.get(4).getName()) ) )
            ..applyTo(headerFrame)

        // Human Frame
        new TableLayout(FRAME_SIZE.x, FRAME_SIZE.y)
            ..addPaletteSection(humanHero     , MAX_CELL, debugTypes.UNIT)
            ..addPaletteSection(humanUnit     , MAX_CELL, debugTypes.UNIT)
            ..addPaletteSection(humanBuildings, MAX_CELL, debugTypes.UNIT)
            ..addPaletteSection(humanSpecial  , MAX_CELL, debugTypes.UNIT)
            ..applyTo(contentFrames.get(0))

        // Orc Frame
        new TableLayout(FRAME_SIZE.x, FRAME_SIZE.y)
            ..addPaletteSection(orcHero     , MAX_CELL, debugTypes.UNIT)
            ..addPaletteSection(orcUnit     , MAX_CELL, debugTypes.UNIT)
            ..addPaletteSection(orcBuildings, MAX_CELL, debugTypes.UNIT)
            ..addPaletteSection(orcSpecial  , MAX_CELL, debugTypes.UNIT)
            ..applyTo(contentFrames.get(1))

        // Undead Frame
        new TableLayout(FRAME_SIZE.x, FRAME_SIZE.y)
            ..addPaletteSection(undeadHero     , MAX_CELL, debugTypes.UNIT)
            ..addPaletteSection(undeadUnit     , MAX_CELL, debugTypes.UNIT)
            ..addPaletteSection(undeadBuildings, MAX_CELL, debugTypes.UNIT)
            ..addPaletteSection(undeadSpecial  , MAX_CELL, debugTypes.UNIT)
            ..applyTo(contentFrames.get(2))

        // NightElf Frame
        new TableLayout(FRAME_SIZE.x, FRAME_SIZE.y)
            ..addPaletteSection(nightElfHero     , MAX_CELL, debugTypes.UNIT)
            ..addPaletteSection(nightElfUnit     , MAX_CELL, debugTypes.UNIT)
            ..addPaletteSection(nightElfBuildings, MAX_CELL, debugTypes.UNIT)
            ..addPaletteSection(nightElfSpecial  , MAX_CELL, debugTypes.UNIT)
            ..applyTo(contentFrames.get(3))

        // Items Frame
        new TableLayout(FRAME_SIZE.x, FRAME_SIZE.y)
            ..addPaletteSection(artifact, MAX_CELL, debugTypes.ITEM)
            ..addPaletteSection(campaign, MAX_CELL, debugTypes.ITEM)
            ..applyTo(contentFrames.get(4))
