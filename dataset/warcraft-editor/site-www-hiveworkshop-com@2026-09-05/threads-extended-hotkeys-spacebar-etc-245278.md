---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/extended-hotkeys-spacebar-etc.245278/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Extended Hotkeys Spacebar Etc
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/extended-hotkeys-spacebar-etc.245278/>
#### DysfunctionaI

#### DysfunctionaI

 Level 7

JoinedMar 6, 2006Messages282

Resources: 1

#### Resources
Tutorials 1

This is a list of never before seen hotkeys that you can use in your map!

 Code:

0 = SHIFT
32 = SPACE

65 = A
66 = B
67 = C
68 = D
69 = E
70 = F
71 = G
72 = H
73 = I
74 = J
75 = K
76 = L
77 = M
78 = N
79 = O
80 = P
81 = Q
82 = R
83 = S
84 = T
85 = U
86 = V
87 = W
88 = X
89 = Y
90 = Z

257 = NUMPAD 0
258 = NUMPAD 1
259 = NUMPAD 2
260 = NUMPAD 3
261 = NUMPAD 4
262 = NUMPAD 5
263 = NUMPAD 6
264 = NUMPAD 7
265 = NUMPAD 8
266 = NUMPAD 9

267 = NUMPAD +
268 = NUMPAD -
269 = NUMPAD *
270 = NUMPAD /
271 = NUMPAD .

272 = +
273 = -
274 = [
275 = ]
276 = \
277 = ;
278 = '
279 = ,
280 = .
281 = /

514 = BACKSPACE

516 = LEFTARROW
517 = UPARROW
518 = RIGHTARROW
519 = DOWNARROW

520 = INSERT
521 = DELETE
522 = HOME
523 = END
524 = PAGEUP
525 = PAGEDOWN

526 = CAPSLOCK
527 = NUMLOCK
528 = SCROLLLOCK

529 = PAUSE

774 = F7


There's some really cool ones in there if you get creative. How about SCROLLLOCK as some sort of toggle? :O

Here's How To Do It!

(credits to WaterKnight for easier implementation)


You need to replace your AbilityMetaData.slk to allow the World Editor to use numbers as hotkeys. Then you can set any hotkey in the Object Editor to any of these numbers that correspond to the key you want.

To replace your AbilityMetaData.slk:


Go to the Registry Editor by typing in "regedit" in the Start Menu.


Locate "HKEY_CURRENT_USER / Software / Blizzard Entertainment / Warcraft III"


Right click in the "Warcraft III" folder and select New -> DWORD (32-bit)


Name it "Allow Local Files" and then open it and set the value to 1


Here's how it looks:


Close the Registry Editor, and go to to your Warcraft III directory, usually at 'C:\Program Files\Warcraft III\'


Make a new folder there called "Units" and then download the attached AbilityMetaData.slk and put it in there.


You'll have to restart the World Editor for it to take effect. If you did it right, then the Hotkey field will be of type 'String':

#### Attachments

lol.png

 1.1 MB

 · Views: 3,056



AbilityMetaData.rar

 12.3 KB

 · Views: 570



EuLev8N.jpg

 116.9 KB

 · Views: 2,885




 Last edited: Feb 15, 2014

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Nice. How have you found out which keys are allowed/what they map to? Only Esc was always known because Blizzard used it too.


You should not take the data from war3.mpq but the latest instead (War3Patch.mpq). I want to note that overwriting the txt (profile) files ofc screws localization. It should be tested if those numeric hotkey representations can also be used in object editor files.

#### Crigges

#### Crigges

 Level 8

JoinedNov 20, 2011Messages202

Resources: 2

#### Resources
Maps 1Tutorials 1

Wow well written tutorial. Realy nice!!!

 How have you found out which keys are allowed/what they map to? Only Esc was always known because Blizzard used it too.


Click to expand...

^this

#### DysfunctionaI

#### DysfunctionaI

 Level 7

JoinedMar 6, 2006Messages282

Resources: 1

#### Resources
Tutorials 1

WaterKnight said:

 Nice. How have you found out which keys are allowed/what they map to? Only Esc was always known because Blizzard used it too


Click to expand...

It started because I just wanted to change some of the hotkeys in CommandStrings.txt and I noticed that ESC was 512... to which I immediately wondered what the other five hundred numbers did (or more).

So I spent a few hours with 3 buildings selling units, all with a different hotkey, that's 36 hotkeys per game, and I used an AutoIt macro to press every key that a keyboard could press, per building. A peculiar way, but I got it done. I never tested past ~700 though, I think the only keys that aren't there are TAB CTRL ALT ~ and the F keys (and print screen, lol).

WaterKnight said:

 You should not take the data from war3.mpq but the latest instead (War3Patch.mpq). I want to note that overwriting the txt (profile) files ofc screws localization.


Click to expand...

Thanks, I really didn't know anything about that so I was hoping someone would say something. I'll update it.

WaterKnight said:

 It should be tested if those numeric hotkey representations can also be used in object editor files.


Click to expand...

Good idea, maybe we can do it straight in JNGP!

My one last thought was this: Sometimes hotkeys aren't the same for people who have a different language WC3 installed. I wonder what happens if you replace those letter hotkeys ( like A ) with a numbered one ( A = 65 ).

Edit: Yo, you CAN put these values in the object editor. I just had it open, attached Cheat Engine, and clicked to change hotkey, I typed a "Z" or something and changed it until I had the right address, then changed that Z to Hex(32) which is 20, and it worked when I tested the map. The problem is though, that input box only lets you type one character, so I couldn't enter the hex for any of the other keys because they were more than 1 byte.


 Last edited: Dec 11, 2013

#### Crigges

#### Crigges

 Level 8

JoinedNov 20, 2011Messages202

Resources: 2

#### Resources
Maps 1Tutorials 1

Somehow the list with the possible hotkeys got deleted.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

DysfunctionaI said:

 Edit: Yo, you CAN put these values in the object editor. I just had it open, attached Cheat Engine, and clicked to change hotkey, I typed a "Z" or something and changed it until I had the right address, then changed that Z to Hex(32) which is 20, and it worked when I tested the map. The problem is though, that input box only lets you type one character, so I couldn't enter the hex for any of the other keys because they were more than 1 byte.


Click to expand...

Rather than inputting them in the WE, you can edit the the files via hex editor. But we can also customize the meta slks in order to manipulate the editor the way to permit different field values, so the hotkey field can take full strings for example.

So I tested the cast hotkey for abilities myself: As a string value, it seems to allow all the keys. As an int value, although you present them as 32 bit, only the lower numbers worked (A-Z and space).

#### DysfunctionaI

#### DysfunctionaI

 Level 7

JoinedMar 6, 2006Messages282

Resources: 1

#### Resources
Tutorials 1

WaterKnight said:

 Rather than inputting them in the WE, you can edit the the files via hex editor. But we can also customize the meta slks in order to manipulate the editor the way to permit different field values, so the hotkey field can take full strings for example.


Click to expand...

I thought you couldn't hex edit map files and still have them open them in the editor?

And whoa, I'm gonna check out that slk stuff and see what I can come up with.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

That's a really cool find. Have some rep. Mind uploading a quick test map?

 I thought you couldn't hex edit map files and still have them open them in the editor?

And whoa, I'm gonna check out that slk stuff and see what I can come up with.


Click to expand...

It depends. SFMPQ might allow you, at your own discretion. Keep in mind that the edits you make won't update in the actual editor--you would have to reopen the map. So if you make an edit in the hex while your map is open, and then you save your map, it will remove the changes you made (the editor doesn't dynamically update like that). Anything using StormLib will throw an error or just won't work. It can still read them just fine, just can't write to it.

It could be a feature added to NewGen, if anyone is willing to code it.  It would behave similar to the "Edit Tileset" function, where after you make the changes, you have to close and reopen the map.

#### DysfunctionaI

#### DysfunctionaI

 Level 7

JoinedMar 6, 2006Messages282

Resources: 1

#### Resources
Tutorials 1

PurgeandFire said:

 That's a really cool find. Have some rep. Mind uploading a quick test map?


It depends. SFMPQ might allow you, at your own discretion. Keep in mind that the edits you make won't update in the actual editor--you would have to reopen the map. So if you make an edit in the hex while your map is open, and then you save your map, it will remove the changes you made (the editor doesn't dynamically update like that). Anything using StormLib will throw an error or just won't work. It can still read them just fine, just can't write to it.


It could be a feature added to NewGen, if anyone is willing to code it.  It would behave similar to the "Edit Tileset" function, where after you make the changes, you have to close and reopen the map.


Click to expand...


Attached a map with some funny hotkeys.


SFMPQ eh? I'll have to try that; I didn't know the MPQ editor mattered. I always tried with MPQmaster and I could never open the map after I hex'd something, but I could play it on WC3 fine.


That'd be hella cool if it was in JNGP... I'm just happy that there's still information that can be uncovered even after 10 years lol. Though it's uses may be limited, it's pretty cool. SCROLLLOCK could be like a toggle or switch in a game... that'd be interesting. Same with CAPSLOCK and NUMLOCK but they actually do stuff, lol.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

yeah, we have kinda enough hotkeys anyways... the only real problem is that the UI can only contain so few abilities...

it would be better if we can find a way for wc3 to trigger events when we press any keyboard button... without relying on abilities since it is so much doable using dummy abilities...

#### GhostWolf

#### GhostWolf

 Level 29

JoinedJul 29, 2007Messages5,174

Resources: 2

#### Resources
Tools 1Tutorials 1

I imagine it's the normal virtual key numbers of Windows http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx


This also raises the question of whether these keys are portable (not that anyone cares about those silly Mac users).

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

but this doesnt work with custom abilities? since those wont exist in the txt file

#### DysfunctionaI

#### DysfunctionaI

 Level 7

JoinedMar 6, 2006Messages282

Resources: 1

#### Resources
Tutorials 1

Adiktuz said:

 yeah, we have kinda enough hotkeys anyways... the only real problem is that the UI can only contain so few abilities...


it would be better if we can find a way for wc3 to trigger events when we press any keyboard button... without relying on abilities since it is so much doable using dummy abilities...


Click to expand...


This is true, which is why these hotkeys uses are limited to very specific maps. Nevertheless, they exist.

GhostWolf said:

 I imagine it's the normal virtual key numbers of Windows http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx


This also raises the question of whether these keys are portable (not that anyone cares about those silly Mac users).


Click to expand...


I looked at that table along with like 4 other tables, and there were no patterns, which the exception of A-Z matching ASCII, and spacebar matching ASCII + those virtual key numbers.


About portability.. I'm highly interested in that as well, not just with macs, but with users with different languages installed. I made a map a while ago and one of the hotkeys was C. Everyone played it fine but a group of people all complained that the hotkey to them, was G. As it turned out, they were all from Germany! I wonder what would happen if I changed that C hotkey to it's decimal of 67.


Wait a sec, I finally did something that allowed the Object Editor to save these hotkeys.

WaterKnight said:

 Rather than inputting them in the WE, you can edit the the files via hex editor. But we can also customize the meta slks in order to manipulate the editor the way to permit different field values, so the hotkey field can take full strings for example.


So I tested the cast hotkey for abilities myself: As a string value, it seems to allow all the keys. As an int value, although you present them as 32 bit, only the lower numbers worked (A-Z and space).


Click to expand...


What meta slks are you referring to? I think I may have done basically that, but with Cheat Engine, allowing the Hotkey input field to accept strings, and it accepts these integers as strings. These means that it would be extremely easy for JNGP to implement this... check it out:


Map is attached to this post.

#### Attachments

yea.png

 57.4 KB

 · Views: 2,636



Extended Hotkeys - No Imports.w3x

 13 KB

 · Views: 484




 Last edited: Dec 12, 2013

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

DysfunctionaI said:

 What meta slks are you referring to?


Click to expand...


For example Units\AbilityMetaData.slk. Search for the entry "ahky", which is the normal hotkey field. Change attributes like "type" to "string", "maxVal" to the amount of chars you want to allow etc. Just view some of the other entries how they were realized. If you have local files on, you can put the edited meta slk into your wc3 directory Warcraft III\Units. Then restart the editor.

#### DysfunctionaI

#### DysfunctionaI

 Level 7

JoinedMar 6, 2006Messages282

Resources: 1

#### Resources
Tutorials 1

WaterKnight said:

 For example Units\AbilityMetaData.slk. Search for the entry "ahky", which is the normal hotkey field. Change attributes like "type" to "string", "maxVal" to the amount of chars you want to allow etc. Just view some of the other entries how they were realized. If you have local files on, you can put the edited meta slk into your wc3 directory Warcraft III\Units. Then restart the editor.


Click to expand...

That's really cool, thanks for telling me! Just another part of the WE I didn't know you could change. I wonder if there's any other useful thing you can make the WE do with those files... I can't recall any fields that I really needed to expand upon.

Edit: omg, so I just added every possible keystroke to a macro, and got the address of the location of Blizzard's hotkey in game, so I'm changing it, then pressing every single key, and I have a trigger that warns me if the Archmage casted anything. It's testing like 10 possible hotkeys a second... this list is gonna get bigger lol.

Sigh, I went to almost 5000 and only got one, 774 = F7. It makes sense, every other key is basically in use. F1-F6 are reserved for Heroes, F8 is Idle Worker, F9 is Quest, F10 is Menu, F11 is Player Menu, and F12 is Chat Log.


 Last edited: Dec 13, 2013

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

The 2nd question is if you can somehow in reverse simulate the pressure special keys via:

native ForceUIKey takes string key returns nothing


May ask our current native hackers to look into that.

#### DysfunctionaI

#### DysfunctionaI

 Level 7

JoinedMar 6, 2006Messages282

Resources: 1

#### Resources
Tutorials 1

WaterKnight said:

 The 2nd question is if you can somehow in reverse simulate the pressure special keys via:

native ForceUIKey takes string key returns nothing


May ask our current native hackers to look into that.


Click to expand...


Already tried it, and got no results.


Also did this:

 JASS:

function keypress takes nothing returns nothing
 call BJDebugMsg("key was pressed")
endfunction

function registershit takes nothing returns nothing
 local trigger t
 local integer i = 0

 loop
 exitwhen i > 6000

 set t = CreateTrigger()
 call TriggerRegisterPlayerEvent(t, Player(0), ConvertPlayerEvent( i ) )
 call TriggerAddAction( keypress )
 set t = null

 set i = i + 1
 endloop
endfunction


Also nothing, hah.


Also, UMSWE which is included in JNGP let's you set hotkeys as strings, so I guess I'll re-do the tutorial to either use UMSWE or use their own meta file with Use Local Files = 1.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Any plan on an update?

EDIT: It was fixed. Nice! Approved.


 Last edited: Feb 16, 2014

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

You could just make a demo map like you had before, you can copy and paste these values instead of having to do all that. Why not make dummy abilities with every new key?

#### Recom

#### Recom

 Level 3

JoinedJan 7, 2010Messages37

I created a map for the Berzerk and Channel ability with all the non-letter hotkeys.


Let me know if I messed something up!

#### Attachments

Extra Hotkeys Berzerk Channel.w3x

 25.4 KB

 · Views: 391



#### GywGod133

#### GywGod133

 Level 13

JoinedJul 16, 2012Messages679

Resources: 14

#### Resources
Icons 6Maps 4Spells and Systems 2Tutorials 2

This hotkey works on MULTIPLAYER??

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

Yup, lovely eh?

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Are there integer equivalent for other keys such as CTRL, ALT, TAB, Console??


EDIT:


Nvm I read the whole 1st page. It seems these are the only available keys.


 Last edited: Aug 2, 2015

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

I wonder will this work if other player's wc3 doesn't have the custom metadata thingy?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Quilnez said:

 I wonder will this work if other player's wc3 doesn't have the custom metadata thingy?


Click to expand...


The metadata file is just for you so that you can input special values into the object editor.


The hotkeys work for players without any extra files.


But if you want to assign spells to those hotkeys (as a modder), then you have two choices: (1) use the SLK as described (2) edit the object file within the map (hex) and change the hotkey as necessary.

#### Marcraft22

#### Marcraft22

 Level 4

JoinedAug 9, 2012Messages27

Has anyone managed to get this to work as of the current patch? I put the .slk file in the Units folder (W3 root folder) but the hotkey metadata is still a character, not a string

#### hugope

#### hugope

 Level 7

JoinedJul 12, 2021Messages97

Resources: 1

#### Resources
Spells and Systems 1

DysfunctionaI said:

 This is true, which is why these hotkeys uses are limited to very specific maps. Nevertheless, they exist.

I looked at that table along with like 4 other tables, and there were no patterns, which the exception of A-Z matching ASCII, and spacebar matching ASCII + those virtual key numbers.

About portability.. I'm highly interested in that as well, not just with macs, but with users with different languages installed. I made a map a while ago and one of the hotkeys was C. Everyone played it fine but a group of people all complained that the hotkey to them, was G. As it turned out, they were all from Germany! I wonder what would happen if I changed that C hotkey to it's decimal of 67.

Wait a sec, I finally did something that allowed the Object Editor to save these hotkeys.

What meta slks are you referring to? I think I may have done basically that, but with Cheat Engine, allowing the Hotkey input field to accept strings, and it accepts these integers as strings. These means that it would be extremely easy for JNGP to implement this... check it out:

Map is attached to this post.


Click to expand...

I tested the attached map in this post, the hotkey Shift didn't work. The hotkey Space bar worked.

Recom said:

 I created a map for the Berzerk and Channel ability with all the non-letter hotkeys.

Let me know if I messed something up!


Click to expand...

I tested the attached map in this post. I tested all the hotkeys. The only one which worked was Space bar, all the other ones do not work. What a shame, I really was looking forward to have an ability with the hotkey Shift.

Marcraft22 said:

 Has anyone managed to get this to work as of the current patch? I put the .slk file in the Units folder (W3 root folder) but the hotkey metadata is still a character, not a string

Click to expand...

I think it won't work even if you are able to change the hotkeys to numbers.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

hugope said:

 I tested the attached map in this post, the hotkey Shift didn't work. The hotkey Space bar worked.

I tested the attached map in this post. I tested all the hotkeys. The only one which worked was Space bar, all the other ones do not work. What a shame, I really was looking forward to have an ability with the hotkey Shift.

I think it won't work even if you are able to change the hotkeys to numbers.


Click to expand...

You can thank Reforged for breaking this as usual.

 You must log in or register to reply here.


### Similar threads

Locked

 Binding Inventory Slots to Side Mouse Buttons


qbrtux

May 26, 2009

Warcraft Discussion

Replies3Views4K

Jun 26, 2009
qbrtux

Locked

 Hotkey Assignment World Editor


NME Ironarrow

May 23, 2020

World Editor Help Zone

Replies2Views1K

May 25, 2020
Uncle

Locked

[Trigger]
 Item Hotkey Modification


Kenchai

Oct 31, 2024

Triggers & Scripts

Replies2Views1K

Nov 1, 2024
Kenchai

Locked

 JetCraft


Illidan(Evil)X

Jun 26, 2013

StarCraft II Modding

Replies28Views8K

Apr 30, 2014
Heinvers

Locked

 Improved Custom Hotkeys Setup for QWEASZ


severed

Feb 27, 2017

Advanced Scripting

Replies8Views21K

Aug 19, 2018
excelcraft

E

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Misc/General Mapping Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top