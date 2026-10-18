---
type: vendor-doc
source_repo: "https://github.com/Jaccouille/wurst-pop-up-ui"
source_path: "wurst/PopUpTest.wurst"
ref: HEAD
---

package PopUpTest

import Assets
import ClosureEvents
import PopUpFrame

// Position of the initial pop up
@configurable let BOTTOM_RIGHT_OFFSET_X = 0.8
@configurable let BOTTOM_RIGHT_OFFSET_Y = 0.155

public function createPopUpBot(string iconLeft, string iconMiddle, string iconRight)
    // Display frame to every player
    for i = 0 to bj_MAX_PLAYERS - 1
        if players[i].isIngame()
            let initialPos = vec2(BOTTOM_RIGHT_OFFSET_X, BOTTOM_RIGHT_OFFSET_Y)
            let offScreenPos = initialPos - vec2(0, 0.03)
            new PopUpFrame(players[i], initialPos, offScreenPos, 10)
                ..setIcons(iconLeft, iconMiddle, iconRight)
                ..setScale(0.5)
                ..updateFramesPosition(vec2(0, 1))


init

    createUnit(players[0], 'ngme', vec2(-400, 0), 270..asAngleDegrees())
    for i = 0 to 10
        createItem('gold', vec2(0,0))
        createItem('mnst', vec2(0,0))
        createUnit(players[1], 'hpea', vec2(750, 0), 270..asAngleDegrees()).setMaxHP(1)

    EventListener.add(EVENT_PLAYER_UNIT_DEATH) ->
        let killerIcon = getAbilityIcon(EventData.getKillingUnit().getTypeId())
        let dyingIcon = getAbilityIcon(EventData.getDyingUnit().getTypeId())
        createPopUpBot(killerIcon, Icons.bTNAttack, dyingIcon)

    EventListener.add(EVENT_PLAYER_UNIT_PICKUP_ITEM) ->
        let itmIcon = EventData.getManipulatedItem().getIconPath()
        let unitIcon = getAbilityIcon(EventData.getManipulatingUnit().getTypeId())
        createPopUpBot(unitIcon, Icons.bTNPickUpItem, itmIcon)

    // Unit selling the item
    EventListener.add(EVENT_PLAYER_UNIT_PAWN_ITEM) ->
        let itmIcon = EventData.getSoldItem().getIconPath()
        let unitIcon = getAbilityIcon(EventData.getTriggerUnit().getTypeId())
        createPopUpBot(unitIcon, Icons.bTNMGExchange, itmIcon)

    EventListener.add(EVENT_PLAYER_ARROW_DOWN_UP) ->
        let unitIcon = getAbilityIcon(EventData.getManipulatingUnit().getTypeId())
        createPopUpBot(unitIcon, Icons.bTNMGExchange, Icons.bTNAcorn)
