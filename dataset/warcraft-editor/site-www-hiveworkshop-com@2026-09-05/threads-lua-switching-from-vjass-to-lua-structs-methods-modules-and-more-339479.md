---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/lua-switching-from-vjass-to-lua-structs-methods-modules-and-more.339479/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Lua Switching from vJASS to Lua Structs Methods Modules and More
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/lua-switching-from-vjass-to-lua-structs-methods-modules-and-more.339479/>
#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

This tutorial exists to help guide a user who already knows about vJass to understand how those vJass principles work in the Lua environment.

If you just want a transpiler to do the job for you, vJass2Lua has you covered:

I'll cover basic JASS conversion first, and then later on will go into detail about vJass constructs and what their Lua equivalents are.

The vJass2Lua Runtime Plugin is useful for simplifying understandings to make the transition a bit more streamlined. I'll cover basic Lua as much as possible, but some vJass contructs require backend tooling to reduce the complexity of each individual Lua script or component.

I've taken many examples 1-to-1 from the JassHelper Manual to create this transitioning guide.

Basic JASS to Lua

Unlike in JASS and vJass, there are no "endfunction" or "endif" or "endloop" considerations. In Lua, you simply write "end".

Functions are very similar between JASS and Lua:

 JASS:

function Foo takes nothing returns nothing
endfunction

"takes" becomes "(" and "returns" becomes ")". Instead of writing "nothing", just leave it blank. You do not declare a return value in Lua(unless you want to annotate it using Emmy styling), so a function that returns an integer looks exactly the same as a function that returns nothing.

 Lua:

function Foo()
end

 JASS:

//Jass comment
/* vJass
 blockquote
*/

 Lua:

--Lua comment
--[[ Lua
blockquote
]]

 JASS:

if something then
elseif something_else then
else
 some_other_else
endif

Replace "endif" with "end" and your if/else structures are converted to Lua. The fact that "elseif" is a keyword in Lua as well as in JASS is convenient.

 Lua:

if something then
elseif something_else then
else
 some_other_else
end

Lua types do not need to be manually designated, since Lua treats variables as generic pointers rather than limiting them to just one type. However, it's a very good idea to keep track of types, if not just for the sake of being able to identify them later on.

 JASS:

function Foo takes unit u, boolean b returns handle
 return null
endfunction

->









 Lua:

---@param u unit
---@param b boolean
function Foo(u, b)
 return nil --not actually needed, you can just leave the return statement and the function will indeed return nil regardless.
end

Note that the type of comments above are styled as Emmy Annotation. This enables powerful features in code editing tools such as VSCode. Please see @Eikonium's tutorial on Emmy Annotation for further reference: [Lua] Emmy Annotation.

vJass2Lua makes this process far less painful to deal with, as it will automatically add Emmy Annotation to the top of your Lua functions.

The "set" and "call" keywords don't exist in Lua, as they are implied based on what you're doing.

On the other hand, "local" works very similarly, but is far more powerful at the same time.

 JASS:

local integer x = 20
local integer y = 20
set x = 10
set y = 10
call BJDebugMsg("x=" + I2S(x) + ", y=" + I2S(y))

->









 Lua:

local x = 20
local y = 20
x = 10
y = 10
print("x=" .. x .. ", y=" .. y)

Note that concatenation looks different, and you don't need I2S.

Lua uses the "number" type to refer to either a real or an integer. Be mindful of this when using division to make sure your integers are truncated and not processed as real values.

 JASS:

local integer i = 10/6

->









 Lua:

local i = 10//6

In addition to inlind symbols like floor division (//) and modulo (%), Lua also gives you access to its powerful "math" library, detailed here: Lua - Math library

Loops are trickier as "exitwhen" doesn't have a Lua equivalent. You have to use if/else statements and the "break" keyword, or you can completely change your loop using one of Lua's many interesting loop styles.

 JASS:

loop
 exitwhen condition
endloop

 Lua:

while true do
 if condition then break end
end

--As you get more advanced, the above Lua could be simplified to:

while not condition do
end

--or

repeat
until condition

--or if it is a loop from 1-10 (including both 1 and 10):

for i = 1, 10 do
end

Operators like "!=" and "+" (when it comes to concatenation) in JASS are different in Lua.

 JASS:

if "foo" + "bar" != "bar" + "foo" then
endif

 Lua:

if "foo" .. "bar" ~= "bar" .. "foo" then
end

Replace != with "~=" to update those operators. Looking for strings to amend their concatenation is much trickier, so I recommend including this short script near the top of your trigger list (note that it is already provided in the vJass2Lua runtime plugin):

 Lua:

getmetatable("").__add = function(obj, obj2) return obj .. obj2 end

JASS has an 'Abil' type of syntax that takes a short string and converts it to an integer. Lua treats apostrophes and quotation marks as the same thing (they are simply strings), so Blizzard introduced a "FourCC" function in order to deal with these.

 Lua:

function FourCC(id)
 return 0x1000000 * string.byte(id:sub(1,1)) +
 0x10000 * string.byte(id:sub(2,2)) +
 0x100 * string.byte(id:sub(3,3)) +
 string.byte(id:sub(4,4))
end

Single characters (denoted by 'A' for example) are treated as integers in JASS, but would be treated as strings by Lua. These can be parsed by Lua in real-time via ('A'):byte() or string.byte('A'). vJass2Lua does the integer conversion for you, so would turn 'A' into 65.

Just FYI, hidden inside an old "cheat.j" file, Blizzard had packaged a reverser that takes an integer and converts it back to a string:

 JASS:

//===========================================================================
// Convert a integer id value into a 4-letter id code.
//
function DebugIdInteger2IdString takes integer value returns string
 local string charMap = ".................................!.#$%&'()*+,-./0123456789:;<=>.@ABCDEFGHIJKLMNOPQRSTUVWXYZ[.]^_`abcdefghijklmnopqrstuvwxyz{|}~................................................................................................................................."
 local string result = ""
 local integer remainingValue = value
 local integer charValue
 local integer byteno

 set byteno = 0
 loop
 set charValue = ModuloInteger(remainingValue, 256)
 set remainingValue = remainingValue / 256
 set result = SubString(charMap, charValue, charValue + 1) + result

 set byteno = byteno + 1
 exitwhen byteno == 4
 endloop
 return result
endfunction

In Lua, this is much simpler as Lua provides "char" as the inverse of "byte", completely eliminating any need for the TheDamien/Nestharus's vJass ASCII library.

 Lua:

--===========================================================================
-- Convert an integer id value into a 4-letter id code.
--
---@param value integer
---@return string
function DebugIdInteger2IdString(value)
 local result = ""
 for byteno=1,4 do
 result = string.char(value % 256) .. result
 value = value // 256
 end
 return result
end

Basic vJass to Lua

Global declaration is much more liberal in Lua compared to vJass.

 Lua:

MyGlobal = "some value"

Native declaration is not needed in Lua. Natives like UnitAlive already exist.

Scopes are much more versatile in Lua. Anything with an "end" keyword that follows it is its own scope.

 JASS:

scope A

endscope

->

 Lua:

do
 --this is a scope
end

if true then
 --also a scope
end

function foo()
 --also a scope
end

 JASS:

scope privatetest
 globals
 private integer N=0
 endglobals
 private function x takes nothing returns nothing
 set N=N+1
 endfunction

 function privatetest takes nothing returns nothing
 call x()
 call x()
 endfunction
endscope

->

 Lua:

do
 local N=0
 local function x()
 N=N+1
 end
 function privatetest() --a global function
 x()
 x()
 end
end

Arrays in vJass are all Tables in Lua. Their declaration is very different as you are not declaring the variable as an array, but rather assigning a variable to an array. This offers MUCH more flexibility for the user:

 JASS:

globals
 private integer array MyArray
endglobals

 Lua:

local MyArray = {} --The curly brackets create an empty array. You can also fill in some stuff inside of them at the same time if you want, but you don't have to.

vJass's Dynamic arrays, array structs, hashtables (including the legendary Table library), and sized arrays are all completely made obsolete by Lua tables. For backwards-compatibility, I have included Lua Table as well as fully-supported backwards-compatibility with Vexorian's Table (which was not originally possible due to unfortunate naming choices made early in NewTable's lifespan).

Libraries work a lot differently in Lua. Unlike in vJass, the script holding library A would normally need to be declared below the script containing library B (if B requires A) if they need to reference each other during the initialization phase.

 JASS:

library A requires B

endlibrary

->

 Lua:

if B then

end

Initialization is extremely dynamic in Lua and isn't bound by the limits of JASS "initializer functions".

 JASS:

scope A initializer Init
 private function Init takes nothing returns nothing
 //do stuff
 endfunction
endscope

->

 Lua:

do
 --do stuff
end

Note that, for more powerful initialization or to ensure a script isn't running until a later point, it is recommended to use [Lua] - Global Initialization

Constant variables and readonly variables are not supported in Lua. However, with some heavy tooling, it is possible to imitate the syntax, as demonstrated in the vJass2Lua Runtime Plugin.

"Textmacros" work much differently in Lua as they do not occur during compile-time. However, the Lua "workarounds" offer significantly more power as they can be created in realtime.

 JASS:

//! textmacro Increase takes TYPEWORD
function IncreaseStored$TYPEWORD$ takes gamecache g, string m, string l returns nothing
 call Store$TYPEWORD$(g,m,l,GetStored$TYPEWORD$(g,m,l)+1)
endfunction
//! endtextmacro

//! runtextmacro Increase("Integer")
//! runtextmacro Increase("Real")

->

 Lua:

local function IncreaseMacro(typeword)
 _G["IncreaseStored"..typeword] = function(g, m, l)
 _G["Store"..typeword](g, m, l, _G["GetStored"..typeword](g, m, l) + 1)
 end
end
IncreaseMacro("Integer")
IncreaseMacro("Real")

Note that _G["varName"] is the true syntax for reading or writing a global variable in Lua. Global variable read/write without it is just Lua syntactical sugar.

We can therefore access global variables in realtime via string referencing and alternate between similar names via concatenation.

Certain hacky textmacros and modules that add half-blocks, like what Nestharus created to make Constant Timer Loop incredibly efficient, cannot be done in a practical sense with Lua. Lua cannot arbitrarily insert half-blocks without the entire script itself being contained as a string that gets concatenated with the macros.

To get around this limitation, you can use a Lua-based system such as Timer Queue, Timed Call and Echo, or even classic "TriggerSleepAction" when combined with Precice PolledWait.

"Keyword" in vJass is just a declared Lua variable that gets assigned later. Check the below two comparisons:

 JASS:

scope
private keyword B

function A takes real x returns real
 if(GetRandomInt(0,1)==0) then
 return B.evaluate(x*0.02)
 endif
 return x
endfunction

function B takes real x returns real
 if(GetRandomInt(0,1)==1) then
 return A(x*1000.)
 endif
 return x
endfunction
endscope

->









 Lua:

do
local B --no "keyword" keyword is needed. Just need to declare B to begin with.
function A(x)
 if GetRandomInt(0,1)==0 then
 return B(x*0.2)
 end
 return x
end
B = function(x) --a function can be assigned to any variable, and can even returned by a function.
 if GetRandomInt(0,1)==1 then
 return A(x*1000)
 end
 return x
end
end

Hooks are WAY more powerful in Lua - they are everything I had wished that they had been in vJass and much more. vJass hooks can't stop the original native from being called, they can only be hooked "before", and they can't change the parameters.

 JASS:

function onRemoval takes unit u returns nothing
 call BJDebugMsg("unit is being removed!")
endfunction

hook RemoveUnit onRemoval

->

 Lua:

local removeUnit = RemoveUnit
function RemoveUnit(u)
 print("unit is being removed!")
 removeUnit(u) --make sure to call this or the unit won't actually be removed.
end

Note that for extra hook compatibility with other resources, you'd want to use [Lua] - Hook

You can even hook regular variables with Global Variable Remapper, which can turn GUI variable assignment into function calls. That opens up a whole universe of new possbilities of what can be done within GUI.

Structs

Structs are more complicated to convert 1-2-1, but the most basic structs would not look much different than simple Lua tables. As previously mentioned, the vJass2Lua Runtime Plugin provides the entire vJass Struct API.

Each of the below methods, unless stated otherwise, are using my Struct library to simplify the process:

 Lua:

MyStruct = Struct() --works like vJass "struct MyStruct"
local newInstance = MyStruct.create() --works like "local MyStruct newInstance = MyStruct.create()"

newInstance.destroy() --works like "call newInstance.destroy()"

MyStruct:destroy() --actually destroys the parent struct, rather than the struct member. This isn't possible in vJass. I've never wished for the feature, but it's possible to do it.

Unlike in vJass, member variables do not need to be independently declared. They will materialize into existence as soon as you initialize them to something:

 Lua:

local thistype = Struct()
thistype.x = 10.
local this = thistype.create()

Note that if you want something to be initialized to a certain value when a struct instance is created, you can either overload "create" (and call "allocate" from within), overload the onCreate stub method, or vJass2Lua will simply allow it to be referenced as a default value as it inherits defaults from its "parent struct".

 Lua:

function thistype:onCreate()
 self.x = 10.
end

If you want to simply identify that a struct has certain fields, you can use Emmy Annotation to write comments about it: [Lua] Emmy Annotation

"thistype" is another vJass construct that can be loosely imitated in Lua:

 Lua:

do
 MyStruct = Struct() --public
 local thistype = MyStruct --internal reference to MyStruct
 thistype.memberA = 7
 thistype.memberB = 8

 function thistype.methodA() end
 function thistype:methodB() end
end

There is no inferred "thistype" otherwise, as the struct itself is not a "scope" itself. However, I think the example above covers all needs for "thistype" outside of Modules, which I've covered in a section further down.

However, let's say you want to reduce the verbosity of your script and don't want to keep having to write "thistype" or "structName" behind every struct member. Struct version 1.0 has you covered with the following syntax:

 Lua:

do local _ENV = Struct.environment(myStruct)
 x = 10
 y = 100 --assigns myStruct.x to 10 and myStruct.y to 100.
end

Yes, it's a bit ugly at the top, but what's happening is that you're overriding a hidden table called "_ENV" which is (usually) the same table as "_G". However, "_ENV" is the pointer to "_G" that declares new globals to be assigned to it. Manipulating _ENV would normally remove your access to the _G table, however doing so in this verbose way will retain your access to the _G table when you need to reference it, thanks to fancy metatable syntax. I found the core mechanics of the approach in Egor Skriptunoff's post here, but I've placed most of the complexity behind the scenes in order to reduce the verbosity needed on the part of the end user.

Struct methods in vJass have static methods and non-static methods. The syntax in Lua is a bit different, but has the same features:

 Lua:

do
 local thistype = Struct()
 function thistype:method() --declaring with the ":" rather than a "." declares a hidden "first" parameter called "self".
 print(self)
 end
 function thistype.staticMethod() --no hidden parameters
 print("static method")
 end
 thistype.staticMethod() --works
 local this = thistype.create()
 this.staticMethod() --works too

 thistype.method() --"self" will be "nil" here.
 thistype:method() --will treat "self" as "thistype".
 this.method() --"self" is nil here.
 this:method() --treats "self" as "this"
 this.method(this) --this works by forcing "this" as "self" without using the colon syntax.
end

Operator overloading is really inefficient and should be used sparingly.

It adds a lot of bloat to the code and only makes it slightly easier on the end user by not requiring them to use parenthesis.

vJass offers nice keywords to make it obvious what is happening with the code, but Lua doesn't really have a clean approach.

 JASS:

struct MyStruct
method operator x takes nothing returns real
 return GetUnitX(this.unit)
endmethod
method operator x= takes real value returns nothing
 call SetUnitX(this.unit, value)
endmethod
method operator y takes nothing returns real
 return GetUnitY(this.unit)
endmethod
method operator y= takes real value returns nothing
 call SetUnitY(this.unit, value)
endmethod
endstruct

->









 Lua:

do --simpler approach that requires the user to use () at the end:
 local thistype = Struct()

 thistype:_operatorget("x", function() return GetUnitX(self.unit) end)
 thistype:_operatorget("y", function() return GetUnitY(self.unit) end)

 thistype:_operatorset("x", function(val) SetUnitX(self.unit, val) end)
 thistype:_operatorset("y", function(val) SetUnitY(self.unit, val) end)
end

Extending structs works really smoothly in Lua. Again, using Struct library in the example below to make the syntax more vJass-y.

 JASS:

struct A
 integer x
 integer y

 method setxy takes integer cx, integer cy returns nothing
 set this.x=cx
 set this.y=cy
 endmethod
endstruct

struct B extends A
 integer z
 method setxyz takes integer cx, integer cy, integer cz returns nothing
 call this.setxy(cx,cy) //we can use A's members
 set this.z=cz
 endmethod
endstruct

->

 Lua:

A = Struct()
function A:setxy(x, y)
 self.x, self.y = x, y
end

B = Struct(A)

function B:setxyz(x, y, z)
 self:setxy(x, y)
 self.z = z
end

"delegate" is a vJass feature that allows to inherit the properties of another struct, even if that struct is already extending another.

Rather than keeping this keyword, I'm recycling the vJass "extends" keyword to fill this void. This way, the language is clear about what the method is doing. Delegate was always a weird one to me.

 Lua:

A = Struct()
B = Struct()
C = Struct(A) --struct C now extends A, but wants to extend B as well
C.extends(B) --struct C now also extends struct B.

As mentioned previously, tables govern almost everything in Lua. Structs, themselves, are tables.

vJass has some keywords like ".typeid", ".super" and ".getType()", but Lua comparisons are so easy to achieve that it is mostly unnecessary to port over these keywords.

For lack of a better function, I've included "super" as a Struct member that gets created if a child struct has a parent declared.

 Code:

A = Struct()
B = Struct(A)
function A.something() print "A something" end
function B.something() print "B something" end --B has overridden A's stub method "something"

B.super.something() --calls A.something()

Stub methods and interfaces don't require any special keywords in Lua, because any member/function can just be overwritten.

Just assign them initially to a function that does either nothing or just some other default behavior.

 Lua:

A = Struct()
A.onFinish = DoNothing --this functions identically to an interface
A.finish = function(self) self:onFinish() end --methods and stub methods are the same thing in Lua.

B = Struct(A)
B.onFinish = function() print "onFinish" end

B:finish() --prints "onFinish"

"module" is a unique vJass thing which has the following properties, that are easily imitated by Lua.

1) it is a textmacro that infers that there is an existing "thistype" to work with that would be declared by the implementing struct.

2) can have its own private members

3) can call stub methods within the struct

4) can have its own initializer

5) can only be run once

1) I've already shown an example of how to do textmacro parameters to access variable names. We wouldn't use "thistype", but can just pass a parameter.

2) Easy thanks to Lua locals

3) Easy thanks to ease of function overriding

4) Lua can initialize any time, anywhere.

5) this kind of protection was almost never needed in vJass, but in the example below I'll show it in action.

I've made it possible to use very similar terminology and syntax thanks to the Struct library:

 JASS:

module MyRepeatModule

 method repeat1000 takes nothing returns nothing
 local integer i=0
 loop
 exitwhen i==1000
 call this.sub() //a method that is expected
 //to exist in the struct
 set i=i+1
 endloop
 endmethod

endmodule

// the struct :
struct MyStruct
 method sub takes nothing returns nothing
 call BJDebugMsg("Hello world")
 endmethod
 implement MyRepeatModule //adds the module.

endstruct

function MyTest takes MyStruct ms returns nothing
 call ms.repeat1000() //will call ms.sub 1000 times.
endfunction

->

 Lua:

vJass.module("myModule", function(struct)
 struct.sub = struct.sub or DoNothing --initialization in Lua is way better, and here we even check if the method exists.
 --best part, even if the method is declared below this module, it will be reassigned from DoNothing to the user's function.

 function struct:repeat1000() --here we declare a new function as a member of the struct from within this function. Lua is awesome.
 for i=1, 1000 do
 self:sub()
 end
 end
end)

myStruct = Struct() --the struct

vJass.implement("myModule", myStruct) --Adds the module

function myStruct:sub() print "Hello world" end

local ms = myStruct.create()
ms:repeat1000() --Will call ms.sub 1000 times.

vJass2Lua and its runtime plugin offer the full catalogue of features in the JassHelper Manual. I didn't document every single item in this tutorial (such as dynamic arrays or 2D arrays), but if you play around with vJass2Lua with some existing vJass code, you can see how they get converted.


 Last edited: Aug 13, 2022

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Fyi, I'm currently working on a Comprehensive Lua Tutorial specifically for Wc3 modding.

It's going to have a bigger scale and include everything from software requirements over Lua programming to Warcraft specific knowledge and resource spotlights.

It's like 66% finished, but I can't work on it until end of May.

Good thing, our tutorials currently don't overlap much and I'm lacking the in-depth vJass knowledge you have anyway, so I'll probably credit your Tutorial as a reference for Lua equivalents of vJass constructs, which will be a great enhancement.

Thanks for writing this down, I'll be coming back to this in a month or so.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Edit: This Macro portion has been submitted as a standalone tool: Convert vJass to Lua using NotePad++

I'm not finished with the below, but I'd like to share it with the community ahead of schedule. This is a Notepad++ macro that converts JASS code (and some vJass code) into Lua. For now, it can convert all JASS code into Lua except for declarations which are made without an assignment value (e.g. local integer i, will just remain local integer i). I haven't quite figured out how to use regex to capture that kind of a declaration, but honestly Lua will bug out if you just write "foo" as global without an assignment, so I'll have to tail it with "= nil" to keep it syntactically sound.

Nevertheless, this will already save someone a lot of time with converting a JASS script to Lua, and it even adds Emmy Annotation as it moves along.

It takes approximately 1 second to parse 10,000 lines of code. Compare that to cJass2Lua, which takes about 5-10 minutes for the same file (Blizzard.j). Unlike cJass2Lua, anyone with even a slight amount of time or experience with XML files should be able to modify many of the elements in this code. I feel like this will enable even more users to adopt their scripts to use Lua. Additionally, it can be reversed with a simple Ctrl+Z which takes about the same amount of time. The complicated bits occur in the regex lines, for which I'll add documentation on at a later time.

How to install/run:

1) Install Notepad++ Downloads | Notepad++

2) Go to Run (Win+R) and search "%AppData%\Notepad++\" (without quotes)

3) Replace the 'shortcuts' XML file with the one provided in the ZIP file below

4) Run Notepad++ (first close it if you had it open)

5) Open a map scipt file or paste in some JASS code into the empty text area

5) Go to Macro at the top > click Bribe's vJass2Lua > voila.

 XML:

<Macro name="Bribe's vJass2Lua version 0.2" Ctrl="no" Alt="no" Shift="no" Key="0">
<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam=" set " />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam=" " />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam=" call " />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam=" " />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="loop" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="while true do" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="exitwhen([^\r\n]*)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="if$1 then break end" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="endloop" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="end" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="endfunction" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="end" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="\/\*" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="--\[\[" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="\*\/" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="\]\]" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="static[^\S\r\n]+if" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="if" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="static[^\S\r\n]*method[^\S\r\n]*operator[^\S\r\n]*" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="function --[[operator]] thistype." />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="method[^\S\r\n]*operator " />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="function --[[operator]] thistype:" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="function[^\S\r\n]*(\w+)[^\S\r\n]*\)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="$1\)" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="static[^\S\r\n]+method[^\S\r\n]+(\w+)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="function thistype\.$1" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="endmethod" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="end" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="method[^\S\r\n]+(\w+)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="function thistype\:$1" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="endif" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="end" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam=''([^"]{4})'' />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam='FourCC\("$1"\)' />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="!=" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="~=" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="library[^\S\r\n]+" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="do --library " />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="endlibrary" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="end --End of Library" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="scope[^\S\r\n]+" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="do --scope " />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="endscope" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="end --End of scope" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="endstruct" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="end --End of struct" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="module[^\S\r\n]+" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="do --module " />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="endmodule" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="end --End of module" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="[^\S\r\n]+textmacro[^\S\r\n]+" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="--[[textmacro " />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="endtextmacro" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="End of textmacro]]" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="//" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="--" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="private" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="local" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="public" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="--[[public]]" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="static[^\S\r\n]+" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="thistype\." />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="local[^\S\r\n]+thistype" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="--\[\[private\]\] thistype" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="local[^\S\r\n]+function[^\S\r\n]+thistype" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="--\[\[private\]\] function thistype" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="readonly" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="--[[readonly]]" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="([^\r\n]+) takes[^\S\r\n]+nothing[^\S\r\n]+returns([^\r\n]*)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="---@return$2\n$1\(\)" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="([^\r\n]+) takes[^\S\r\n]+(\w+)[^\S\r\n]+(\w+)[^\S\r\n]+returns([^\r\n]*)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="---@param $3 $2\n---@return$4\n$1\($3\)" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="([^\r\n]+) takes[^\S\r\n]+(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]+returns([^\r\n]*)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="---@param $3 $2\n---@param $5 $4\n---@return$6\n$1\($3, $5\)" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="([^\r\n]+) takes[^\S\r\n]+(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]+returns([^\r\n]*)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="---@param $3 $2\n---@param $5 $4\n---@param $7 $6\n---@return$8\n$1\($3, $5, $7\)" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="([^\r\n]+) takes[^\S\r\n]+(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]+returns([^\r\n]*)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="---@param $3 $2\n---@param $5 $4\n---@param $7 $6\n---@param $9 $8\n---@return$10\n$1\($3, $5, $7, $9\)" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="([^\r\n]+) takes[^\S\r\n]+(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]+returns([^\r\n]*)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="---@param $3 $2\n---@param $5 $4\n---@param $7 $6\n---@param $9 $8\n---@param $11 $10\n---@return$12\n$1\($3, $5, $7, $9, $11\)" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="([^\r\n]+) takes[^\S\r\n]+(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]+returns([^\r\n]*)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="---@param $3 $2\n---@param $5 $4\n---@param $7 $6\n---@param $9 $8\n---@param $11 $10\n---@param $13 $12\n---@return$14\n$1\($3, $5, $7, $9, $11, $13\)" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="([^\r\n]+) takes[^\S\r\n]+(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]+returns([^\r\n]*)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="---@param $3 $2\n---@param $5 $4\n---@param $7 $6\n---@param $9 $8\n---@param $11 $10\n---@param $13 $12\n---@param $15 $14\n---@return$16\n$1\($3, $5, $7, $9, $11, $13, $15\)" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="([^\r\n]+) takes[^\S\r\n]+(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]+returns([^\r\n]*)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="---@param $3 $2\n---@param $5 $4\n---@param $7 $6\n---@param $9 $8\n---@param $11 $10\n---@param $13 $12\n---@param $15 $14\n---@param $17 $16\n---@return$18\n$1\($3, $5, $7, $9, $11, $13, $15, $17\)" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="([^\r\n]+) takes[^\S\r\n]+(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]+returns([^\r\n]*)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="---@param $3 $2\n---@param $5 $4\n---@param $7 $6\n---@param $9 $8\n---@param $11 $10\n---@param $13 $12\n---@param $15 $14\n---@param $17 $16\n---@param $19 $18\n---@return$20\n$1\($3, $5, $7, $9, $11, $13, $15, $17, $19\)" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="([^\r\n]+) takes[^\S\r\n]+(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]+returns([^\r\n]*)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="---@param $3 $2\n---@param $5 $4\n---@param $7 $6\n---@param $9 $8\n---@param $11 $10\n---@param $13 $12\n---@param $15 $14\n---@param $17 $16\n---@param $19 $18\n---@param $21 $20\n---@return$22\n$1\($3, $5, $7, $9, $11, $13, $15, $17, $19, $21\)" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="([^\r\n]+) takes[^\S\r\n]+(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*,[^\S\r\n]*(\w+)[^\S\r\n]+(\w+)[^\S\r\n]+returns([^\r\n]*)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="---@param $3 $2\n---@param $5 $4\n---@param $7 $6\n---@param $9 $8\n---@param $11 $10\n---@param $13 $12\n---@param $15 $14\n---@param $17 $16\n---@param $19 $18\n---@param $21 $20\n---@param $23 $22\n---@return$24\n$1\($3, $5, $7, $9, $11, $13, $15, $17, $19, $21, $23\)" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="---@return nothing\n" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="//" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="--" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="globals" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="--globals" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="endglobals" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="--endglobals" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="constant" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="--[[constant]]" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="null" />
<Action type="3" message="1625" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="nil" />
<Action type="3" message="1702" wParam="0" lParam="771" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="(\w+)[^\S\r\n]+(\w+)[^\S\r\n]*\=[^=]([^\r\n]*)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="$2 = $3 ---@type $1" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="(\w+)[^\S\r\n]+array[^\S\r\n]+(\w+)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="$2 = {} ---@type $1\[\]" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="local[^\S\r\n]+(\w+)[^\S\r\n]+(\w+)([\r\n])" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="local $2 ---@type $1$3" />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />

<Action type="3" message="1700" wParam="0" lParam="0" sParam="" />
<Action type="3" message="1601" wParam="0" lParam="0" sParam="struct[^\S\r\n]+(\w+)" />
<Action type="3" message="1625" wParam="0" lParam="2" sParam="" />
<Action type="3" message="1602" wParam="0" lParam="0" sParam="$1 = {}\ndo local thistype = $1--struct " />
<Action type="3" message="1702" wParam="0" lParam="770" sParam="" />
<Action type="3" message="1701" wParam="0" lParam="1609" sParam="" />
</Macro>

#### Attachments

Bribe's vJass2Lua v0.2.zip

 1.9 KB

 · Views: 135




 Last edited: May 12, 2022

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Simple, useful.

I have a small nitpick in terms of how/where the comments are placed to explain the changes, it feels a bit crammed. I would also say I feel like some of them are not needed. "You just remove.." yeah I can see that, get what I mean?

However, I do not think any of those things are enough to make it stay unapparoved.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

This tutorial has been updated to include simpler terms where I had gone into too much detail, and expanded on areas where I didn't cover enough.

 You must log in or register to reply here.


### Similar threads

Locked

 vJass struct imitation in Lua


Bribe

May 6, 2022

Advanced Scripting

Replies0Views1K

May 6, 2022
Bribe

 vJASS OOP Lesson


aznricepuff

May 19, 2009

Scripting Tutorials

Replies28Views19K

Jun 2, 2010
Hashjie

Locked

 Modules for Wc3 Lua


Almia

Apr 30, 2019

Advanced Scripting

Replies1Views3K

Apr 30, 2019
Almia

Locked

 [Lua] Module System


Almia

Sep 1, 2021

Advanced Scripting

Replies10Views5K

Oct 24, 2022
Bribe

[Lua]
 vJass2Lua Runtime Plugin


Bribe

May 8, 2022

Graveyard

Replies12Views5K

Aug 19, 2022
Bribe

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top