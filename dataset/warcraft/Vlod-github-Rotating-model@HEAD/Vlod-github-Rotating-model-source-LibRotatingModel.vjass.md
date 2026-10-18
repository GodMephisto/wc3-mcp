---
type: vendor-doc
source_repo: "https://github.com/Vlod-github/Rotating-model"
source_path: "source/LibRotatingModel.vjass"
ref: HEAD
---

library LibRotatingModel

struct RotatingModel
    real x
    real y
    real z = 50.
    real scale = 1.
    real radius = 150.
    integer count = 3
    integer red = 255
    integer green = 255
    integer blue = 255
    integer alpha = 255
    playercolor pcolor = ConvertPlayerColor(12)
    string model = "Abilities\\Weapons\\GlaiveMissile\\GlaiveMissile.mdl"
    unit target
    unit array unit[10]
    real array angle[10]
    real dangle = 0.05
    real timeout = 0.03125
    timer timer = CreateTimer()
    static integer unitid = 'h000'
    private static hashtable H = InitHashtable()
    private static location L = Location(0,0)
    
    private method update takes nothing returns nothing
        local integer cnt = .count
        if .target != null then
            set .x = GetUnitX(target)
            set .y = GetUnitY(target)
        endif
        loop
        exitwhen cnt <= 0
            set .angle[cnt] = .angle[cnt] + .dangle
            call SetUnitX( .unit[cnt], .x + .radius*Cos(.angle[cnt]) )
            call SetUnitY( .unit[cnt], .y + .radius*Sin(.angle[cnt]) )
        set cnt = cnt - 1
        endloop
    endmethod
    
    private static method timerUpdate takes nothing returns nothing
        local thistype val = LoadInteger(thistype.H, GetHandleId(GetExpiredTimer()), 0)
        call val.update()
    endmethod
    
    private method createUnit takes real startAngle returns nothing
        local integer cnt = .count
        local real dangle = 6.283/cnt
        local real height
        if cnt > .unit.size then
            set cnt = .unit.size
            set .count = cnt
        endif
        loop
        exitwhen cnt <= 0
            set .angle[cnt] = startAngle
            set startAngle = startAngle + dangle
            set .unit[cnt] = CreateUnit(Player(PLAYER_NEUTRAL_PASSIVE), thistype.unitid, .x, .y, 0.)
            call AddSpecialEffectTarget(.model, .unit[cnt], "chest")
            call SetUnitColor(.unit[cnt], .pcolor)
            call SetUnitVertexColor(.unit[cnt], .red, .green, .blue, .alpha)
            call SetUnitScale(.unit[cnt], .scale, .scale, .scale)
            call MoveLocation(thistype.L, .x, .y)
            set height = GetLocationZ(thistype.L)
            call SetUnitFlyHeight(.unit[cnt], height + .z, 0.)
        set cnt = cnt - 1
        endloop
    endmethod
    
    method start takes real x, real y returns nothing
        set .x = x
        set .y = y
        call .createUnit(GetRandomReal(0., 6.283))
        call SaveInteger(thistype.H, GetHandleId(.timer), 0, this)
        call TimerStart(.timer, .timeout, true, function thistype.timerUpdate)
    endmethod
    
    method startUnit takes unit target returns nothing
        set .target = target
        call .start(GetUnitX(target), GetUnitY(target))
    endmethod
    
    private method removeUnit takes nothing returns nothing
        loop
        exitwhen .count <= 0
            call RemoveUnit(.unit[.count])
            set .unit[.count] = null
        set .count = .count - 1
        endloop
    endmethod
    
    method destroy takes nothing returns nothing
        call PauseTimer(.timer)
        call FlushChildHashtable(thistype.H, GetHandleId(.timer))
        call DestroyTimer(.timer)
        set .timer = null
        set .target = null
        call .removeUnit()
        call .deallocate()
    endmethod
    
    method pause takes nothing returns nothing
        call PauseTimer(.timer)
    endmethod
    
    method resume takes nothing returns nothing
        call ResumeTimer(.timer)
    endmethod
    
    method reCount takes integer count returns nothing
        local real angle = .angle[1]
        call PauseTimer(.timer)
        call .removeUnit()
        set .count = count
        call .createUnit(angle)
        call TimerStart(.timer, .timeout, true, function thistype.timerUpdate)
    endmethod
    
    method reScale takes real scale returns nothing
        local integer cnt = .count
        set .scale = scale
        loop
        exitwhen cnt <= 0
            call SetUnitScale(.unit[cnt], scale, scale, scale)
        set cnt = cnt - 1
        endloop
    endmethod
    
    method reColor takes integer red, integer green, integer blue, integer alpha returns nothing
        local integer cnt = .count
        set .red = red
        set .green = green
        set .blue = blue
        set .alpha = alpha
        loop
        exitwhen cnt <= 0
            call SetUnitVertexColor(.unit[cnt], red, green, blue, alpha)
        set cnt = cnt - 1
        endloop
    endmethod
    
    method rePlayerColor takes playercolor pcolor returns nothing
        local integer cnt = .count
        set .pcolor = pcolor
        loop
        exitwhen cnt <= 0
            call SetUnitColor(.unit[cnt], pcolor)
        set cnt = cnt - 1
        endloop
    endmethod
    
endstruct

endlibrary