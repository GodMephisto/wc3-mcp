---
type: vendor-doc
source_repo: "https://github.com/Jaccouille/wurst-debug-ui"
source_path: "wurst/ObjectSpawner.wurst"
ref: HEAD
---

package ObjectSpawner

import ObjectPreview
import ClosureKeyPresses
import ClosureEvents
import HashMap

@configurable constant rotationDegrees = 30

public enum debugTypes
    UNIT
    ITEM

class ObjectSpawnerDebug
    vec2 mousePos
    DummyFx debugPreviewFx
    ItemFx itemFx
    player _player
    player owner
    EventListener mouseEvent
    bool isActive
    debugTypes debugType
    int spawnCount = 1
    texttag spawnCountTag = null


    construct(int objectId, debugTypes debugType)
        this._player = EventData.getTriggerPlayer()
        this.owner =  EventData.getTriggerPlayer()
        this.debugType = debugType
        this.spawnCountTag = CreateTextTag()
            ..setColor(255, 0, 0, 255)
            ..setPermanent(true)
        setupOsKey()
        setupListener()
        createObjectPreview(objectId, debugType)

    function setupOsKey()
        onKeyPress(this._player, OSKEY_1, () -> setOwner(players[0]))
        onKeyPress(this._player, OSKEY_2, () -> setOwner(players[1]))
        onKeyPress(this._player, OSKEY_3, () -> setOwner(players[2]))
        onKeyPress(this._player, OSKEY_4, () -> setOwner(players[3]))
        onKeyPress(this._player, OSKEY_5, () -> setOwner(players[4]))
        onKeyPress(this._player, OSKEY_6, () -> setOwner(players[5]))
        onKeyPress(this._player, OSKEY_7, () -> setOwner(players[6]))
        onKeyPress(this._player, OSKEY_8, () -> setOwner(players[7]))
        onKeyPress(this._player, OSKEY_9, () -> setOwner(players[8]))
        onKeyPress(this._player, OSKEY_0, () -> setOwner(players[9]))

        onKeyPress(OSKEY_1, OSKEY_META.CTRL, () -> setOwner(players[10]))
        onKeyPress(OSKEY_2, OSKEY_META.CTRL, () -> setOwner(players[11]))
        onKeyPress(OSKEY_3, OSKEY_META.CTRL, () -> setOwner(players[12]))

        onKeyPress(this._player, OSKEY_ADD, () -> addSpawnCount(1))
        onKeyPress(this._player, OSKEY_SUBTRACT, () -> addSpawnCount(-1))

        onKeyPress(this._player, OSKEY_MULTIPLY) ->
            if this.isActive
                this.rotateFx(rotationDegrees)
        onKeyPress(this._player, OSKEY_DIVIDE) ->
            if this.isActive
                this.rotateFx(-rotationDegrees)

    function setupListener()
        EventListener.add(EVENT_PLAYER_MOUSE_MOVE) ->
            if EventData.getTriggerPlayer() == this._player and this.isActive
                this.mousePos = vec2(BlzGetTriggerPlayerMouseX(),
                                     BlzGetTriggerPlayerMouseY())
                this.debugPreviewFx.setXY(this.mousePos)
                this.spawnCountTag.setPos(this.mousePos.toVec3())


        mouseEvent = EventListener.add(EVENT_PLAYER_MOUSE_DOWN) ->
            if EventData.getTriggerPlayer() == this._player and this.isActive
                if BlzGetTriggerPlayerMouseButton() == MOUSE_BUTTON_TYPE_LEFT
                    for i = 1 to spawnCount
                        this.debugPreviewFx.spawnObject()
                else if BlzGetTriggerPlayerMouseButton() == MOUSE_BUTTON_TYPE_RIGHT
                    this.disable()

    function createObjectPreview(int objectId, debugTypes debugType)
        this.debugType = debugType
        if this.isActive
            this.debugPreviewFx.setFxSkin(objectId)
            return
        this.isActive = true
        this.owner = this._player
        switch this.debugType
            case UNIT
                debugPreviewFx = new UnitFx(this.mousePos, objectId, this._player, 270..asAngleDegrees())
            case ITEM
                debugPreviewFx = new ItemFx(this.mousePos, objectId, this._player)

    function setOwner(player owner)
        if this.isActive
            this.owner = owner
            this.debugPreviewFx.setOwner(owner)

    function rotateFx(int degrees)
        if this.debugType == UNIT
            this.debugPreviewFx.setXYAngle(
                (angle(this.debugPreviewFx.getXYAngle().radians() + degrees * DEGTORAD)))

    function addSpawnCount(int count)
        this.spawnCount += count
        if spawnCount > 1
            this.spawnCountTag.setText("x" + this.spawnCount.toString(), 12)
        else
            this.spawnCountTag.setText("", 12)

    function disable()
        if this.isActive
            this.isActive = false
            this.spawnCount = 1
            this.spawnCountTag.setText("", 12)
            this.debugPreviewFx.hiddenDestroy()


let playerToDebugSpawner = new HashMap<player, ObjectSpawnerDebug>

public function createObjectSpawner(int objectId, debugTypes debugType)
    let _player = EventData.getTriggerPlayer()
    if not playerToDebugSpawner.has(_player)
        playerToDebugSpawner.put(_player, new ObjectSpawnerDebug(objectId, debugType))
    else
        playerToDebugSpawner.get(_player).createObjectPreview(objectId, debugType)
