---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-plus"
source_path: "README.md"
ref: HEAD
---

# vjass-plus
a vJASS+ -> vJASS converter

`python vjassp.py 'somepath\script.jp'`

## Supports

**Make simple. Keep jass'n**

- conditional & mass import
- Indent based syntax
- Anonymous scope (content)
- Anonymous multi initializer
- local variable hoisting
- omittable call, set
- simplified function declacation
- new operators
  - `is`, `is not`, `+=...`, `++...`
- aliasing types
  - default aliases
  - `integer -> int` / `string -> str` / `boolean -> bool` / `void -> nothing` / `table -> hashtable`

## vJass+ Form

```python
# index allocator module
int next   = 0
int size   = 0
int stack  = []

allocate() -> integer:
    if size > 0:
        size--
        return stack[size]
    else:
        next++
        return next

deallocate(integer id):
    stack[size] = id
    size++

# internal module
table   table   ~ {}
timer   timers  = []
bool    exists  = []
tick    last    = none

int     INITIAL_POOL_AMOUNT ~ 512
init:
    int i = 1
    until i > INITIAL_POOL_AMOUNT:
        timers[i] = CreateTimer()
        SaveInteger(table, none, GetHandleId(timers[i]), i)
        i++

global:
    alias tick extends integer

api:
    # Tick - 생성
    Create() -> tick:
        last = allocate()
        if timers[last] is null:
            timers[last] = CreateTimer()
            SaveInteger(table, none, GetHandleId(timers[last]), last)
        exists[last] = true
        return last
    
    # Tick - 파괴
    Destroy(tick whichTick):
        if exists[whichTick]:
            exists[whichTick] = false
            PauseTimer(timers[whichTick])
            deallocate(whichTick)

    # Tick - 실행
    Start(tick whichTick, real timeout, bool periodic, code handlerFunc):
        TimerStart(timers[whichTick], timeout, periodic, handlerFunc)

    # Tick - 한 번 실행
    Once(tick whichTick, real timeout, code handlerFunc):
        TimerStart(timers[whichTick], timeout, false, handlerFunc)

    # Tick - 반복 실행
    Periodic(tick whichTick, real timeout, code handlerFunc):
        TimerStart(timers[whichTick], timeout, true, handlerFunc)

    # Tick - 일시정지
    # * 일시정지 상태가 아닌 tick 개체를 일시정지할 경우 문제가 발생할 수 있음
    Pause(tick whichTick):
        PauseTimer(timers[whichTick])

    # Tick - 재개
    # * 일시정지 상태가 아닌 tick 개체를 재개할 경우 문제가 발생할 수 있음
    Resume(tick whichTick):
        ResumeTimer(timers[whichTick])

    # Tick - 만료된 tick 가져오기
    # * 해당 함수가 호출된 시점에 만료된 tick 개체가 존재하지 않으면 0 반환
    Expired() -> tick:
        return LoadInteger(table, none, GetHandleId(GetExpiredTimer()))

```

```python
import "tick.jp"

content:
    
    tick contentTick

    onInterval():
        BJDebugMsg("WOW! 0.50 seconds passed!")

    init:
        contentTick = TickCreate()
        TickStart(contentTick, 0.50, true, function onInterval)
```

## vJass Form

```
library tick initializer onInit
    globals
        private integer next = 0
        private integer size = 0
        private integer array stack
    endglobals
    private function allocate takes nothing returns integer
        if size > 0 then
            set size = size - 1
            return stack[size]
        else
            set next = next + 1
            return next
        endif
    endfunction
    private function deallocate takes integer id returns nothing
        set stack[size] = id
        set size = size + 1
    endfunction
    globals
        private constant hashtable table = InitHashtable()
        private timer array timers
        private boolean array exists
        private integer last = 0
        private constant integer INITIAL_POOL_AMOUNT = 512
    endglobals
    private function VJPI34CDDC7755DF4761 takes nothing returns nothing
        local integer i = 1
        loop
            exitwhen i > INITIAL_POOL_AMOUNT
            set timers[i] = CreateTimer()
            call SaveInteger(table, 0, GetHandleId(timers[i]), i)
            set i = i + 1
        endloop
    endfunction
    function TickCreate takes nothing returns integer
        set last = allocate()
        if timers[last] == null then
            set timers[last] = CreateTimer()
            call SaveInteger(table, 0, GetHandleId(timers[last]), last)
        endif
        set exists[last] = true
        return last
    endfunction
    function TickDestroy takes integer whichTick returns nothing
        if exists[whichTick] then
            set exists[whichTick] = false
            call PauseTimer(timers[whichTick])
            call deallocate(whichTick)
        endif
    endfunction
    function TickStart takes integer whichTick, real timeout, boolean periodic, code handlerFunc returns nothing
        call TimerStart(timers[whichTick], timeout, periodic, handlerFunc)
    endfunction
    function TickOnce takes integer whichTick, real timeout, code handlerFunc returns nothing
        call TickStart(whichTick, timeout, false, handlerFunc)
    endfunction
    function TickPeriodic takes integer whichTick, real timeout, code handlerFunc returns nothing
        call TickStart(whichTick, timeout, true, handlerFunc)
    endfunction
    function TickPause takes integer whichTick returns nothing
        call PauseTimer(timers[whichTick])
    endfunction
    function TickResume takes integer whichTick returns nothing
        call ResumeTimer(timers[whichTick])
    endfunction
    function GetExpiredTick takes nothing returns integer
        return LoadInteger(table, 0, GetHandleId(GetExpiredTimer()))
    endfunction
    private function onInit takes nothing returns nothing
        call VJPI34CDDC7755DF4761()
    endfunction
endlibrary
scope VJPS18F46CB218474831 initializer onInit
    globals
        private integer contentTick
    endglobals
    private function onInterval takes nothing returns nothing
        call BJDebugMsg("WOW! 0.50 seconds passed!")
    endfunction
    private function VJPIEE9374976E9E40A1 takes nothing returns nothing
        set contentTick = TickCreate()
        call TickStart(contentTick, 0.50, true, function onInterval)
    endfunction
    private function onInit takes nothing returns nothing
        call VJPIEE9374976E9E40A1()
    endfunction
endscope
```
