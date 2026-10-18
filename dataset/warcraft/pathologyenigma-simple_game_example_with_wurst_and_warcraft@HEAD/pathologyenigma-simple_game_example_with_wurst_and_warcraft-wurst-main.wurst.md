---
type: vendor-doc
source_repo: "https://github.com/pathologyenigma/simple_game_example_with_wurst_and_warcraft"
source_path: "wurst/main.wurst"
ref: HEAD
---

package main
import charactorBuilder
import ClosureTimers
import ClosureEvents

init
    let test_unit = createUnit(players[0], 'hfoo', vec2(1, 1), angle(0))
    // use this just because init block runs
    // before the unitindexer
    doAfter(0.01) ->
        // if still nothing
        // then it deserves a wtf
        if test_unit.getData() == null
            print("wtf")
        else
            let builder = test_unit.builder()
            let data = test_unit.getData()
            builder.setDamageMitigate(88)
            data.showInfo()
            EventListener.add(test_unit, EVENT_UNIT_DAMAGED) -> 
                let miti = data.getDamageMitigate()
                let raw = GetEventDamage()
                let source = GetEventDamageSource()
                real toneUp = 0
                if source.getData() != null
                    toneUp = source.getData().getDamageToneUp()
                var res = raw * (toneUp - miti + 100) / 100
                if res <= 0
                    res = 1
                BlzSetEventDamage(res)
                if test_unit.isAlive()
                    let upgrader = data.getCumstomUpgrader<unit>("lvl_upgrader")
                    upgrader.gain_exp(10)
                test_unit.getData().showInfo()
            let another_test_unit = createUnit(players[0], 'hfoo', vec2(1, 1), angle(0))
            another_test_unit.setAttackCooldown(0.01, 0)