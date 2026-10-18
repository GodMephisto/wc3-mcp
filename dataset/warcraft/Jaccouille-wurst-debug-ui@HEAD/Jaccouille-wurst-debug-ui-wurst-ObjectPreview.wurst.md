---
type: vendor-doc
source_repo: "https://github.com/Jaccouille/wurst-debug-ui"
source_path: "wurst/ObjectPreview.wurst"
ref: HEAD
---

package ObjectPreview

import DummyRecycler

public interface DummyFx
    function setXY(vec2 pos)
    function spawnObject()
    function setFxSkin(int skinId)
    function setOwner(player owner)
    function getXYAngle() returns angle
    function setXYAngle(angle value)
    function setHandler(player handler)
    function hiddenDestroy()

public class ItemFx implements DummyFx
    player handler
    private item dummy	= null

    construct(vec2 pos, int itemId, player handler)
        this.handler = handler
        dummy = BlzCreateItemWithSkin(itemId, pos.x, pos.y, itemId)

    /** Set the position to the given xy vector tuple */
    override function setXY(vec2 pos)
        dummy.setPos(pos)

    override function spawnObject()
        createItem(dummy.getTypeId(), dummy.getPos())

    override function setFxSkin(int skinId)
        BlzSetItemSkin(this.dummy, skinId)

    override function setOwner(player owner)
        printTimedToPlayer("Cannot set a color for an item.", 7, this.handler)

    override function getXYAngle() returns angle
        return 270..asAngleDegrees()

    override function setXYAngle(angle value)
        printTimedToPlayer("Cannot set angle for an item.", 7, this.handler)

    override function setHandler(player handler)
        this.handler = handler

    function getDummy() returns item
        return this.dummy

    ondestroy
        if dummy != null
            dummy.remove()


    /** Destroys the Fx object without showing the effect's death animation
    Sort of a workaround by setting the position to the top right corner, hopefully not visible.*/
    override function hiddenDestroy()
        dummy.setPos(vec2(2147483647, 2147483647))
        destroy this

public class UnitFx implements DummyFx
    player handler
    private unit 	dummy	= null

    construct(vec2 pos, int unitId, player handler, angle facing)
        this.dummy = DummyRecycler.get(pos, facing)
        this.handler = handler
        setOwner(handler)
        setFxSkin(unitId)

    /** Get the dummyunit */
    function getDummy() returns unit
        return dummy

    /** Get the xy angle */
    override function getXYAngle() returns angle
        return dummy.getFacingAngle()

    /** Set the xy angle */
    override function setXYAngle(angle value)
        dummy.setFacing(value)

    /** Set the position to the given xy vector tuple */
    override function setXY(vec2 pos)
        dummy.setPos(pos)

    override function spawnObject()
        createUnit(dummy.getOwner(), BlzGetUnitSkin(dummy), dummy.getPos(), dummy.getFacingAngle())

    override function setFxSkin(int unitId)
        BlzSetUnitSkin(dummy, unitId)

    override function setOwner(player owner)
        getDummy().setOwner(owner, true)

    override function setHandler(player handler)
        this.handler = handler

    ondestroy
        DummyRecycler.recycle(dummy, 1.)


    /** Destroys the Fx object without showing the effect's death animation
    Sort of a workaround by setting the position to the top right corner, hopefully not visible.*/
    override function hiddenDestroy()
        dummy.setPos(vec2(2147483647, 2147483647))
        destroy this
