---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-stack-standart-buttons-remove-patrol-hold-position-etc.261633/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Stack Standart Buttons Remove Patrol Hold Position Etc
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-stack-standart-buttons-remove-patrol-hold-position-etc.261633/>
#### ALEXz407

#### ALEXz407

 Level 7

JoinedAug 23, 2014Messages208

Resources: 2

#### Resources
Tutorials 2

##
Better way to hide ANY button founded by leandrotp. But it crash on MAC versions.

#
 Command Buttons Stacking

You do not have enough space on the unit's command card because standard buttons occupy it?

Now you can fix it! Fold unnecessary buttons under top-left corner button and free some space for abilities.

Bonus: hidden buttons are not activated when you press their shortcuts on the keyboard. You can assign a skill shortcut to the vacant key.

Note: these changes will apply to all units and all players, use wisely!

Screenshots

Download DEMO map

##
 How to use?

1. Make a copy of Units\CommandFunc.txt file from war3.mpq

2. Open it with Notepad

3. Change lines "Buttonpos=X,Y" to "Buttonpos=-2147483648,-2147483648" for all command buttons you want to stack.

4. Save file

5. Import file in your map

6. Change imported file full path to "Units\CommandFunc.txt" (Right click -> Modify File Properties... -> check Use custom path -> type new path in enabled textbox -> OK)

7. Save map and test how it works!

##
 Which command button will be shown on the top?

It seems that game places first loaded command button on the top. There are two stacking types:

- Type A - Attack, Attack Ground, Stop

- Type B - Move, Stop, Patrol, Hold Position

- Theoretically, Type C - Hero abilities

If first loaded unit:

- can move and can attack, game will place Movement buttons (Type B) first.

- can move and can not attack, game will place Movement buttons (Type B) first.

- can not move and can attack, game will place Attack buttons (Type A) first.

- can not move and can not attack (or command card hidden with Ward classification), game will ignore this unit.

In what order the game loads the units? I can not say exactly how this works, but: buildings loaded before units, and probably heroes loaded after buildings and before units.

Warning! Stacking order is always set to Type A after loading saved game. Keep it in mind if your map uses standart Save/Load.

##
 How to define stacking type?

1. Open Object Editor in World Editor (F6)

2. Open Units tab

3. Select: Standart Units -> Human -> Melee -> Buildings -> Guard Tower (hgtw)*

3.5 Copy it if you want use standart Guard Tower in map later.

4. Modify Standart Guard Tower:

- for Type A stacking you must keep tower attack ability. (when you select tower, you must see Attack button)

- for Type B stacking you must set Movement - Speed Base to 1 (or higher value). (when you select tower, you must see Move button)

4.5 You can change different values to make tower more "dummy", but keep in mind:

- Stats - Is a Building must be TRUE

- If you changing attack, watch attack availability in command card.

- Guard Tower must be visible for all players. However, you can replace model with dummy and make it unselectable with Locust.

5. Place edited Guard Tower on the map.

6. Change ownership of placed Guard Tower to Neutral Passive. (so it will not interfere normal gameplay)

7. Save map and test it!

*If you find an object that overrides Guard Tower, please write it in comments.

##
 Important info

Stacking order depends on placed on the map units at the game start.

Stacking order does not change during the game.

Buttons stacking works properly only in 0,0 slot of command card (top-left). Attempts to stack two command buttons in other slots led to crashes.

Minimal value for start stacking buttons is "Buttonpos=-2147483648,-2147483648" (closer to 0 values lead to crashes in some situations, still not sure what effect have closer to -inf values).

Hidden in the stack buttons can not be activated via shortcuts. Only displayed buttons work.

This method changes command card for all units and all players.

It seems that this method does not cause game crashes. Nevertheless, I advise to use it with caution as all "hacks".

Button stacking order from uppermost to the bottom for the normal unit

Type A

Attack

Attack

Ground

Stop

Move

Patrol

Hold

Position

Hero

abilities

Type B

Move

Stop

Patrol

Hold

Position

Attack

Attack

Ground

Hero

abilities

Button stacking order for stationary unit (Movement - Speed Base = 0)

Type A

Attack

Attack

Ground

Stop

Hero

abilities

Type B

Stop

Attack

Attack

Ground

Hero

abilities

Button stacking order for unit without attacks (Combat - Attacks Enabled = None)

Type A

Stop

Move

Patrol

Hold

Position

Hero

abilities

Type B

Move

Stop

Patrol

Hold

Position

Hero

abilities

Button stacking order for unit with hidden attacks (Combat - Attack 1&2 - Show UI = false)

Type A

Attack

Ground*

Stop

Move

Patrol

Hold

Position

Hero

abilities

Type B

Move

Stop

Patrol

Hold

Position

Attack

Ground*

Hero

abilities

*Attack ground does nothing when clicked/shortcutted, so you can use it for map rules info tooltip for example.

[CmdMove]

Art=CommandMove

Buttonpos=-2147483648,-2147483648

[CmdAttack]

Art=CommandAttack

Buttonpos=-2147483648,-2147483648

[CmdAttackGround]

Art=CommandAttackGround

Buttonpos=-2147483648,-2147483648

[CmdBuild]

Art=CommandBasicStruct

Buttonpos=0,2

[CmdBuildHuman]

Art=CommandBasicStructHuman

Buttonpos=0,2

[CmdBuildOrc]

Art=CommandBasicStructOrc

Buttonpos=0,2

[CmdBuildNightElf]

Art=CommandBasicStructNightElf

Buttonpos=0,2

[CmdBuildUndead]

Art=CommandBasicStructUndead

Buttonpos=0,2

[CmdCancel]

Art=CommandCancel

ButtonPos=3,2

[CmdCancelBuild]

Art=CommandCancel

ButtonPos=3,2

[CmdCancelTrain]

Art=CommandCancel

ButtonPos=3,2

[CmdCancelRevive]

Art=CommandCancel

ButtonPos=3,2

[CmdHoldPos]

Art=CommandHoldPosition

Buttonpos=-2147483648,-2147483648

[CmdPatrol]

Art=CommandPatrol

Buttonpos=-2147483648,-2147483648

[CmdPurchase]

Art=CommandPurchase

Buttonpos=0,0

[CmdRally]

Art=CommandRally

Buttonpos=3,1

PlacementModel=UI\Feedback\RallyPoint\RallyPoint.mdl

[CmdSelectSkill]

Art=CommandNewSkill

Buttonpos=-2147483648,-2147483648

[CmdStop]

Art=CommandStop

Buttonpos=-2147483648,-2147483648

//[CmdUnivAgi]

//Art=

//Buttonpos=2,0

//[CmdUnivInt]

//Art=

//Buttonpos=1,0

//[CmdUnivStr]

//Art=

//Buttonpos=0,0

##
 What can you do with it?

 AZERTY friendly spell layout for 8 spells

 Layout with 11 free slots (10+1 for heroes)

Configure as you wish!

CommandFunc_ERTYDFGH.txt

CommandFunc_10_11.txt

##
 Bonus

Additional information that can be used with Command Buttons Stacking.

###
 How to make custom descriptions for standart command buttons or remove shortcuts?

1. Make a copy of Units\CommandStrings.txt file from war3.mpq

2. Open it with Notepad

3. Change lines "Tip", "Ubertip", "Hotkey" for all command buttons you want to customize.

- Tip. Name of the command button (1 line)

- Ubertip. Description of the command button (several lines, make line break with "|n")

- Hotkey. One letter or numeric key code. Leave it empty if you want remove hotkey from button.

[CmdAttackGround]

Tip=|cffffcc00Standart Command Buttons Stacking

Ubertip="All standart command card buttons (even hero abilities select button, for demo purposes) are hidden under this modified Attack Ground standart button.|n|nYou can customize which standart buttons should be stacked in Units\CommandFunc.txt|n|nYou can change standart buttons descriptions and remove hotkeys for them in Units\CommandStrings.txt|n|nMore info at Hiveworkshop forums!|n|n|cffffcc00Author: ALEXz407"

Hotkey=

4. Save file

5. Import file in your map

6. Change imported file full path to "Units\CommandStrings.txt" (Right click -> Modify File Properties... -> check Use custom path -> type new path in enabled textbox -> OK)

7. Save map and test how it works!

###
 How to change standart command button icon?

1. Open Game Interface window in World Editor (Advanced -> Game Interface...)

2. Check "Use Custom Game Interface"

3. You can now select new icons for standart buttons!

Icon - Command - Attack Ground = Engineering Upgrade <Normal> (ReplaceableTextures\CommandButtons\PASBTNEngineeringUpgrade.blp)

4. Save map and test how it works!


 Last edited: Jun 19, 2016

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

As much as I like this discovery: you should have asked a mod to move your thread in the lab section instead of creating a new one.

#### ALEXz407

#### ALEXz407

 Level 7

JoinedAug 23, 2014Messages208

Resources: 2

#### Resources
Tutorials 2

If you are using Command Buttons Stacking in your map, please share link to it here. It will be interesting to see how it works in real project.

Zwiebelchen said:

 As much as I like this discovery: you should have asked a mod to move your thread in the lab section instead of creating a new one.


Click to expand...

I created a new thread for 2 reasons:

1. Thread in [thread=261559]The Lab[/thread] shows the research process, when this thread in Tutorials has final information and step by step instructions.

2. I afraid that moving thread will change thread URL and all direct links on attachments will "fall off".


 Last edited: Jan 21, 2015

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

Figured out how to get Type C yet?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Very awesome find. This is probably one of the most commonly requested things, and I haven't seen anyone get it below 3 before. And the tutorial itself is excellent. There is no reason not to approve it!

#### daverave1212

#### daverave1212

 Level 11

JoinedApr 9, 2010Messages425

Resources: 8

#### Resources
Skins 2Tools 2Maps 3Tutorials 1

Woho, nice and extremely useful

#### ALEXz407

#### ALEXz407

 Level 7

JoinedAug 23, 2014Messages208

Resources: 2

#### Resources
Tutorials 2

leandrotp discover way to hide any button completely. But it lead to crashes on MAC versions.

And his discovery a lot easier. You just need to set the button X position to 0 and Y to -11.

Or for standart command card buttons (example for Move):

 Code:

[CmdMove]
Art=CommandMove
Buttonpos=0,-11


 Last edited: Jun 19, 2016

#### leandrotp

#### leandrotp

 Level 9

JoinedJul 30, 2012Messages156

Resources: 1

#### Resources
Tutorials 1

Well, I still couldn't find a way to make my method work in macs, I didn't give up yet, but while I don't get it, your method is the preferred one, so I'd like to add some things about it.

Whenever a button is going to be displayed, the game calls a function to determine its location. This function takes the desired X and Y position, and attempts to find an available slot for that button.

The function takes the Y value, multiplies it by 16, and adds it to a memory address. Then it reads the DWORD at the resulting address and adds it to the value of X multiplied by 4.

The result of this operation will be the address of a structure that represents one of the 12 slots. I will call this structure UI_SLOT

The UI_SLOT structure has a "occupied" property located at offset 0x94. If UI_SLOT.occupied is false, that slot will be used for the button.

However, if UI_SLOT.occupied is true, the function will begin a loop to find the nearest available slot

The loop starts at the originally desired position, and goes until the (3,2) position. If it finds an available slot there, it will be used.

If the function does not find an available slot with this, then it will begin a new loop, but in reverse. This loop starts at the original position and goes backwards, until the (0.0) position.

If the function does not find an avaliable slot after these two loops, that button will not be displayed.
Now, see what happens when you set any of them to the extreme negatives:

The value -2147483648 (in hex 0x80000000) will be mutiplied by 16, and with this, the most significant bit will be lost, and the result is 0.

The X value will be multiplied by 4, and the result will also be 0.

So the result of this operation will be the address of the UI_SLOT structure of the (0,0) slot.

When the first button is placed, that slot is available and it will be used. For the next button, it will not be available anymore and the function goes to the loop.

The first loop ends when Y>3, but this an unsigned comparison. Therefore, the value 0x80000000 will always be greater than 3, and this loop will end before the first step.

Then it goes to the reverse loop, which ends when Y<0. As -2147483648 is always lower than 0, this loop won't get a chance to run as well.
As you can see, the result is that all buttons, with the exception of the first, will not be displayed. This is 100% guaranteed to work, in any ocasion.

As a consequence of this, it's possible to stack all buttons in ANY position, not just (0,0). But this is how you should do it:


Buttonpos=X,Y

-2147483648   1      2      3   X / Y

-268435456*

-268435455

-268435454

Unfortunately your method requires at least 1 button to be displayed, and can't hide the cancel button. I'm still trying to make my method work on mac, but maybe it's not possible at all because of different memory layout, I can't be sure for now.

Update: You can also use the value -268435456 for the Y position only instead of the maximum negative. This value will be accepted by the World Editor, so you can use it to hide normal abilities too.

##
 Bonus

You can make ANY ability appear on top of all stacked buttons!!!


I just found what would be the "Type C" stacking that you mentioned: If you add ANY ability to the 1st unit of the map (in your test map, that would be the Guard Tower), and remove the movement and attack of that unit, then that ability will appear on top of the stacked buttons!


And you can do that to any number of abilities: just add those abilities to the Guard Tower, and disable its movement and attack. I didn't test this behaviour when loading saved games though.


 Last edited: Jul 20, 2015

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

leandrotp said:

##
 Bonus

The "Hero Awaken" ability ('Aawa') takes precedence over ALL buttons, and will override them if they are stacked in a slot. So if you need a command card with 12 spells, you can use 'Aawa' as a base for one of them.

It's a bit weird to get it working, you need to give a dummy dead hero to all players, and the tooltip of the ability will be the 'revive' tooltip of that hero. This ability can provide up to 4 buttons, and these buttons have maximum priority in the command card. If you want a passive button, you can also use Orb of Slow with 'Aawa' as effect ability.

When I get home I'll make a testmap for demonstration.


Click to expand...

Yeah but why would you do that if you can just use the remaining button in 0,0 as your dummy anyway?

If for example "move" is the remaining visible button at 0,0, just redirect all "move" orders to the spell you want to cast.

#### leandrotp

#### leandrotp

 Level 9

JoinedJul 30, 2012Messages156

Resources: 1

#### Resources
Tutorials 1

Zwiebelchen said:

 Yeah but why would you do that if you can just use the remaining button in 0,0 as your dummy anyway?


If for example "move" is the remaining visible button at 0,0, just redirect all "move" orders to the spell you want to cast.


Click to expand...


Maybe you would want an instant or passive spell instead of a targeted one... I just wanted to show that the possibility is there, if anyone ever needs it.

#### leandrotp

#### leandrotp

 Level 9

JoinedJul 30, 2012Messages156

Resources: 1

#### Resources
Tutorials 1

Updated my post with new findings. Now it's possible to put ANY ability on top of the stacked buttons.

And I still haven't been able to install Mac OS in my computer, so for now, forget the other method. This one is already good enough, and 100% safe.


 Last edited: Jul 20, 2015

#### ALEXz407

#### ALEXz407

 Level 7

JoinedAug 23, 2014Messages208

Resources: 2

#### Resources
Tutorials 2

leandrotp said:

 Updated my post with new findings...


Click to expand...

I experiment a little bit with nearest values and found an interesting thigs:

1. World Editor working oddly with big values, it will round them with 16 (and after that with 32) steps. So you can not use -268435455 and -268435454 for Y, it will be rounded down to -268435456.

2. And accidentally I found that Y -268435440 represent hero buttons. So you can place shortcuts here. And they will stack too! 12 slots is real!

Button positions from top (F1) to bottom (seventh) icons

 Code:

Buttonpos=24,-268435440
Buttonpos=20,-268435440
Buttonpos=16,-268435440
Buttonpos=12,-268435440
Buttonpos=8,-268435440
Buttonpos=4,-268435440
Buttonpos=0,-268435440

I notice that hero buttons handled by the game in different way than command card, here is main difference:

- Buttons will appear on the screen when friendly unit with corresponding abilities selected.

- Green highlight around standart buttons doesn't updated and always freezed in first unit selected state (i.e. if unit stays - Stop button will be green always)

- Shortcuts for sidebar buttons doesn't work, you can only click on them.

- First placed button will hold place and doesn't overrided by other buttons.

- If you deselect a unit, buttons will stay on the screen. You can click on that buttons for other unit and if it can handle fired order id, it will use corresponding skill.

- Button will update if selected unit change corresponding ability (noticed this with hero abilities learning)

- Hero portraits will stay on top of stacked with this approach buttons.

- Sidebar buttons can not show cooldowns, but possibly can show numbers (like hero ability points).

- Layout refreshed when hero dies, revives, hired, gains/spends ability points.

So you can grab all buttons and place them under F1 hero, add a dummy hero (i.e. Game Rules, Inventory, etc.) and everything will work.

Also you do not have to bother with stacking order, because all buttons will be hidden under hero portrait.

This approach needs to be tested, but so far everything looks stable.

#### Attachments

shortcuts_to_portraits.jpg

 732.3 KB

 · Views: 3,108




 Last edited: Jul 23, 2015

#### Ezekiel12

#### Ezekiel12

 Level 12

JoinedMar 13, 2012Messages1,121

 2. And accidentally I found that Y -268435440 represent hero buttons.


Click to expand...

Fun never ends with this game. Can you use normal hotkeys (m for move, a for attack) for the abilities on the left bar? If you have one, can you share your testmap?

#### ALEXz407

#### ALEXz407

 Level 7

JoinedAug 23, 2014Messages208

Resources: 2

#### Resources
Tutorials 2

Ezekiel12 said:

 Fun never ends with this game. Can you use normal hotkeys (m for move, a for attack) for the abilities on the left bar? If you have one, can you share your testmap?


Click to expand...

Nope, only clicks.

Here is updated demo with new approach, all buttons (except Hero ability) are hidden under first hero portrait: View attachment CommandButtonsStackingDemo2.w3x

Now we need testers with MAC-s, I hope this method will not crash

#### LordDz

#### LordDz

 Level 25

JoinedMay 11, 2007Messages4,643

Resources: 8

#### Resources
Maps 8

Just tested, it also works for units.

So if you set the Peasant Art - Button Position Y to -268435440, you can click that portrait to train a peasant when selecting a town hall.

It also works for buildings, so you can build buildings with this. Just use a peasant, use his build once and the buildings will stick to the sidebar and can be built. Hotkeys doesn't seem to work for them tho.

And.. Since the buildings are not in the build menu anymore, I guess you can fit in more buildings to be built from a worker.

The game crashes if two different units/abilities has the same button position.

But yeah, I guess it can be useful for some abilities, like Call of Arms.

EDIT: The buttons are removed when you train/revive a hero, and will be added back again when selecting that unit that has the abilities.

Same thing when a hero learns an ability.

So I guess you could modify which icons to show by triggers.

(Sorry if this was already known, just testing on my own).

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Whoa! That is a really cool find. Nice job!

#### ALEXz407

#### ALEXz407

 Level 7

JoinedAug 23, 2014Messages208

Resources: 2

#### Resources
Tutorials 2

LordDz said:

 The game crashes if two different units/abilities has the same button position.


Click to expand...

Interestingly, I can not make game crash with two different units on same button position. Do you use Mac PC?

LordDz said:

 EDIT: The buttons are removed when you train/revive a hero, and will be added back again when selecting that unit that has the abilities.


Click to expand...

Buttons also refreshed when hero gains/spends an ability point (even if currently dead). So far everything looks good: just add 1 dummy, or track current hero and use trigger that do +1 -1 ability point to any player's hero every N milliseconds.

Here is updated example with 12+5 buildings for peasant: View attachment CommandButtonsStackingDemo2edit.w3x

It would be awesome if someone find way to remove black bars under buttons, i hope this texture doesn't used everywhere and can be easily replaced with 1x1 px transparent dummy.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Could you provide a test map with the method that you know crashes on a mac? I have a mac, but I am running it through Wineskin since wc3 has so many issues on the latest OS (so technically it is the Windows client). If it crashes, then I wouldn't mind testing whichever test maps you send my way.

EDIT: The map in post #14 loads and runs fine. The map in #17 goes into some sort of infinite loop and crashes.

#### LordDz

#### LordDz

 Level 25

JoinedMay 11, 2007Messages4,643

Resources: 8

#### Resources
Maps 8

Ok, the reason it crashed was because I had not setup the X position of the button.

So when it's set it doesn't crash.

I'm using PC.

#### ALEXz407

#### ALEXz407

 Level 7

JoinedAug 23, 2014Messages208

Resources: 2

#### Resources
Tutorials 2

PurgeandFire said:

 EDIT: The map in post #14 loads and runs fine. The map in #17 goes into some sort of infinite loop and crashes.


Click to expand...

Then I guess we have method that can hide any standart button under hero portrait.

And possibly way to have 12+6 buttons, but it needs debugging. leandrotp is a true genius at that part. Maybe he could find safe values that can be used both on PC and MAC.

EDIT: Can you check these maps? I reduce number of used slots in each map, so theoretically we can find maximum available sidebar buttons for Mac:

7. View attachment CommandButtonsStackingDemo2_7.w3x

6. View attachment CommandButtonsStackingDemo2_6.w3x

5. View attachment CommandButtonsStackingDemo2_5.w3x

4. View attachment CommandButtonsStackingDemo2_4.w3x

3. View attachment CommandButtonsStackingDemo2_3.w3x

2. View attachment CommandButtonsStackingDemo2_2.w3x

1. View attachment CommandButtonsStackingDemo2_1.w3x

To activate sidebar slots just need to open peasant's build menu.


 Last edited: Jul 23, 2015

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

It drops to about 1 fps for all of them as soon as the map starts, so I had to exit.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

holy amazeballs!

I think this is easily the biggest WC3 exploit I've seen in a while. It's a shame that hotkeys don't work on these sidebar buttons. That could have been THE way to create an MMO-like ability HUD.

It's still awesome though. I really have to check out if I can get a possible use out of this (probably not, as I use the quickbar for displaying allied heroes) ... there must be a way to use this for something really cool.

#### ALEXz407

#### ALEXz407

 Level 7

JoinedAug 23, 2014Messages208

Resources: 2

#### Resources
Tutorials 2

PurgeandFire said:

 It drops to about 1 fps for all of them as soon as the map starts, so I had to exit.


Click to expand...

So it doesn't crash? I guess this happens because I use "Time - Periodic Event" with 0.00 value (tested it on PC - 10000 sidebar refresh actions per second) maybe on MAC it executed even more times.

Here is another test map: View attachment Test_1.w3x - sidebar refresh starts after 5 seconds of gametime and executed every 0.03 s (33 times per second).

Zwiebelchen said:

 It's still awesome though. I really have to check out if I can get a possible use out of this (probably not, as I use the quickbar for displaying allied heroes) ... there must be a way to use this for something really cool.


Click to expand...

For example you can make "Battle Mode" button. So default sidebar buttons will be:

1. Hero

2. Backpack

3. Material Bag

4. Talents Archive? (you can split them into 2 spellbooks: base and advanced - 24 talents total (or even add nested spellbooks pagination, then there is no limits!). And also add Learn talent button here, that will open hidden hero skills menu. And because you make talents choice one from three, you can place talent learn buttons to sidebar aswell)

5. Usefull Things (revive mercenary, roll, etc)

6. Game Options

7. Battle Mode

When you click on Battle Mode, all spellbook abilities (Talents Archive, Usefull Things, Game Options) and Material Bag (probably with Backpack for total 6 hero slots?) removed (just remove corresponding spellbook abilities and refresh sidebar with +/-1 skill point) and your Party interface activates. Battle Mode replaced with Peaceful Mode to make vice versa actions. This could even trigger automatically when entering/leaving battle, but I think it's better to add as option, not default behaviour.

Sidebar spellbooks can contain sidebar buttons with "Cancel" at the bottom (so only 1 cancel + 3 buttons very comfortable for clicking + 12 normal buttons in command card).

Also all buttons (+learned talents) can be repeated in backpack for user's convenience. (will require additional dummy abilities with normal positions and shortcuts ofc)

So far I see three main problems:

- Mac compatibility testing.

- These black bars under sidebar buttons.

- Sidebar clumsiness with low refresh rate.

+ It needs at least 1 hero for each player to hide stacked standart buttons and refresh sidebar.


 Last edited: Jul 24, 2015

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

ALEXz407 said:

 So it doesn't crash? I guess this happens because I use "Time - Periodic Event" with 0.00 value (tested it on PC - 10000 sidebar refresh actions per second) maybe on MAC it executed even more times.


Here is another test map: View attachment 146904 - sidebar refresh starts after 5 seconds of gametime and executed every 0.03 s (33 times per second).


For example you can make "Battle Mode" button. So default sidebar buttons will be:


1. Hero


2. Backpack


3. Material Bag


4. Talents Archive? (you can split them into 2 spellbooks: base and advanced - 24 talents total (or even add nested spellbooks pagination, then there is no limits!). And also add Learn talent button here, that will open hidden hero skills menu. And because you make talents choice one from three, you can place talent learn buttons to sidebar aswell)


5. Usefull Things (revive mercenary, roll, etc)


6. Game Options


7. Battle Mode


When you click on Battle Mode, all spellbook abilities (Talents Archive, Usefull Things, Game Options) and Material Bag (probably with Backpack for total 6 hero slots?) removed (just remove corresponding spellbook abilities and refresh sidebar with +/-1 skill point) and your Party interface activates. Battle Mode replaced with Peaceful Mode to make vice versa actions. This could even trigger automatically when entering/leaving battle, but I think it's better to add as option, not default behaviour.


Sidebar spellbooks can contain sidebar buttons with "Cancel" at the bottom (so only 1 cancel + 3 buttons very comfortable for clicking + 12 normal buttons in command card).


Also all buttons (+learned talents) can be repeated in backpack for user's convenience. (will require additional dummy abilities with normal positions and shortcuts ofc)


So far I see three main problems:


- Mac compatibility testing.


- These black bars under sidebar buttons.


- Sidebar clumsiness with low refresh rate.


+ It needs at least 1 hero for each player to hide stacked standart buttons and refresh sidebar.


Click to expand...

Yeah I actually had a similar idea... the problem is that it would be unintuitve to have the controls placed there. For someone who digs self-explanatory systems like me, this is not easy to get by.

#### ALEXz407

#### ALEXz407

 Level 7

JoinedAug 23, 2014Messages208

Resources: 2

#### Resources
Tutorials 2

Zwiebelchen said:

 Yeah I actually had a similar idea... the problem is that it would be unintuitve to have the controls placed there. For someone who digs self-explanatory systems like me, this is not easy to get by.


Click to expand...

I guess things that looks like buttons and appear on the screen when you select hero (and disappear when you select other unit) are pretty noticeable and self-explanatory. Just tell the player that he can click on them (introduction quest for example or big splashscreen with arrows pointing on buttons/backpack/etc and text descriptions, displayed when hero picked).


Perhaps splitted buttons (i.e. spellbook with 3 buttons on sidebar and 4 on command card) may be confusing, because player will notice changes only in sidebar zone and command card will be in blind-spot.


And tooltips appearing near to command card when you hover over sidebar buttons are little uncomfortable, but we can not change this - frameDef can not be modified at runtime.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Hmm... I evaluated some possible designs for that in my head and just couldn't find any layout that was somewhat satisfying.

But I could imagine this being amazing for 3rd person shooters or fullscreen systems. For example, you could use the skill point number of the icon to display ammo or just use those buttons like tabs that select specific sub-menus in FSI.

But keep experimenting guys! Maybe you can find something even cooler, like having abilities displayed on the right side of the screen aswell.

Also, have you found a method to replace the idle worker button with this? This could be really interesting, as you can't have a hero button there by normal means, so you could use it to trigger an interface element or have a spellbook or menu placed there.

You can already do this with the idle worker button, but it introduces a nasty delay due to the selection event. Having an actual ability there would remove this lag.

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

Wait... so I can have 12(command card)+7(heroes)+6(inventory) buttons per unit? Omfg

EDIT: The F1-7 hotkeys don't work either for those abilities. Need to find a way.

EDIT2: Found a way to make the hero buttons show cooldowns and the bluish thing for when they can't be cast due to mana. Just level up the hero.

EDIT3: Found a way to make hero buttons castable by hotkey. If you have an ability with the same order ID in the command card, it will use its hotkey for both. While one is uncastable the other will use its hotkey instead.


 Last edited: Jul 24, 2015

#### ALEXz407

#### ALEXz407

 Level 7

JoinedAug 23, 2014Messages208

Resources: 2

#### Resources
Tutorials 2

Xonok said:

 EDIT: The F1-7 hotkeys don't work either for those abilities. Need to find a way.


Click to expand...

I guess this happens because F1-F4 buttons read pointer to unit (hero) from specified location. So when we place skills here, it just points to nothing.

Black background for bars displayed here with same reason - game can not retrieve unit, so check "is hero dead" returns 0, and hp/mp values are 0 - so we can see a blank bars.

Xonok said:

 EDIT2: Found a way to make the hero buttons show cooldowns and the bluish thing for when they can't be cast due to mana. Just level up the hero.


Click to expand...

You can also just give and take away 1 skill point to player's hero, this will refresh everything in HeroBar.

Xonok said:

 EDIT3: Found a way to make hero buttons castable by hotkey. If you have an ability with the same order ID in the command card, it will use its hotkey for both. While one is uncastable the other will use its hotkey instead.


Click to expand...

Sounds good.

After searching for barewords in game.dll, I can guess that Blizz use these names for UI elements:

HeroBar - 7 buttons at top left

PeonBar - 1 button at bottom left

CommandBar - 12 buttons bottom right

Inventory - 6 buttons near to CommandBar

Too bad what everything is compiled here, so I can not find used texture or understand logic (maybe it just hardcoded fill with #000 for these backgrounds).

P.S. I found that black background under HeroBar buttons disappers when hero dies, but idk how it can be simulated without a hero and for all buttons, not only for one.

#### leandrotp

#### leandrotp

 Level 9

JoinedJul 30, 2012Messages156

Resources: 1

#### Resources
Tutorials 1

ALEXz407 said:

 2. And accidentally I found that Y -268435440 represent hero buttons.


Click to expand...


Forget about that man, I had also found it in my testings, but didn't post it because it will surely crash on mac. The reason it works in windows is because the game uses Storm.dll to allocate memory, and Storm allocates similar objects near each other (based on class name).


But the mac version uses the operating system's malloc to allocate everything, so the objects are placed in unpredictable places in the memory. I didn't really test it in mac because I didn't suceed in installing Mac OS, but I'm pretty sure it will crash unless you are extremely lucky. (But anyway, it would be technically unsafe, the only thing 100% safe is to stack buttons into one of the 12 slots)

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

I don't care about macs, but I do have one problem. For some reason the hero icon buttons have started to disappear once in a while. I don't know what causes it though.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Macs are basicly not supported by WC3 anyway. Sad but true. Having optimized textures with less mipmaps for example will crash macs aswell. All the good stuff doesn't work on macs either, like enabling local files.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

@Alex: Tested it. It worked fine with 33 fps.

As for the issue around Macs:

 Battle.net doesn't work for Mavericks (10.9) and above.

 You can't even install the native wc3 binaries unless you're running 10.6 or below with PPC architecture (or Rosetta if you're on 10.6 with intel).

 w3arena's mac client runs on WINE internally.

For those reasons, most Mac users have had to switch to a wineskin configuration or they run wc3 through bootcamp -> Windows partition. The number of mac wc3 players are already small, and I doubt that anyone using a mac would want to sit on a 10.6 upgrade forever unless it is a really old computer. I don't think it is necessary to cater to those few people. Most ppl I've encountered on reddit or w/e asking about macs ended up using Wineskin. As long as they have a suitable alternative, it should be fine.

 Macs are basicly not supported by WC3 anyway. Sad but true. Having optimized textures with less mipmaps for example will crash macs aswell. All the good stuff doesn't work on macs either, like enabling local files.


Click to expand...

Local files work through Wineskin, thankfully.  The low mipmap blp thing is still an issue though, even on Wineskin.

#### leandrotp

#### leandrotp

 Level 9

JoinedJul 30, 2012Messages156

Resources: 1

#### Resources
Tutorials 1

PurgeandFire said:

 @Alex: Tested it. It worked fine with 33 fps.


Click to expand...


Did you test with the native Mac version of warcraft, or with the wineskin version? And what version of Mac OS are you running?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I ran it with Wineskin (OS X 10.10 Yosemite), sorry if I was misleading with my post.

You are probably correct in your suspicions. My post meant to outline why it might not be necessary to even support Macs to begin with as long as they have a suitable alternative.

#### ALEXz407

#### ALEXz407

 Level 7

JoinedAug 23, 2014Messages208

Resources: 2

#### Resources
Tutorials 2

So, I have bad and good news.

Bad => HeroBar is compiled into game.dll and can not be modified with Jass functions.

Good => We have a workaround for this!

Just look at this: View attachment Test_2.w3x

And how it works:

- Game redraws HeroBar when any hero in game obtain skillpoint. You can add dummy hero for neutral passive and run refresh trigger for him.

- Black bars under buttons removed when player's hero dies, and they stay hidden even if you remove hero from the game. You can add 7 dummy heroes for each player, kill them and then just remove from the game.

- Also, because we use only PC safe code in map, we can use 0,-11 to hide unwanted command card buttons completely and make 7 buttons universal sidebar.

P.S. I think to make separate tutorial for this feature, with nice demo map.

UPD: Wow! Noticed that game crash after playsession with sidebar buttons. Right after closing scores screen.

UPD2: I guess game tries to wipe heroes data after playsession, but our HeroBar doesn't contain real heroes data and crash happens.


 Last edited: Jul 25, 2015

#### leandrotp

#### leandrotp

 Level 9

JoinedJul 30, 2012Messages156

Resources: 1

#### Resources
Tutorials 1

ALEXz407 said:

 So, I have bad and good news.


Bad => HeroBar is compiled into game.dll and can not be modified with Jass functions.


Good => We have a workaround for this!


Just look at this: View attachment 146926


And how it works:


- Game redraws HeroBar when any hero in game obtain skillpoint. You can add dummy hero for neutral passive and run refresh trigger for him.


- Black bars under buttons removed when player's hero dies, and they stay hidden even if you remove hero from the game. You can add 7 dummy heroes for each player, kill them and then just remove from the game.


- Also, because we use only PC safe code in map, we can use 0,-11 to hide unwanted command card buttons completely and make 7 buttons universal sidebar.


P.S. I think to make separate tutorial for this feature, with nice demo map.


UPD: Wow! Noticed that game crash after playsession with sidebar buttons. Right after closing scores screen.


UPD2: I guess game tries to wipe heroes data after playsession, but our HeroBar doesn't contain real heroes data and crash happens.


Click to expand...


I was able to reproduce the crash in your test map and debugged it: it's caused by the Repair ability. I don't really know why, but it seems that this ability can't be placed on the sidebar. Try changing its position back to the command card and tell me if the crashes happen.

#### ALEXz407

#### ALEXz407

 Level 7

JoinedAug 23, 2014Messages208

Resources: 2

#### Resources
Tutorials 2

leandrotp said:

 I was able to reproduce the crash in your test map and debugged it: it's caused by the Repair ability. I don't really know why, but it seems that this ability can't be placed on the sidebar. Try changing its position back to the command card and tell me if the crashes happen.


Click to expand...

Looks like some buttons are safe for placing in sidebar, but certain buttons cause crash. For example in demo map if last opened sidebar was peasant's skills - game will crash, but if you open peasant's Build menu - game will end normally.Demo map updated, now there are only safe skills

UPD: Looks like problem happens with activating abilities and buying items.

Standart command button - ok

Build - ok

Train - ok

Upgrade - ok

Item in Shop - crash after score screen

Learn Hero Ability - ok

Normal Hero Ability - crash after score screen

Autocast Hero Ability - crash after score screen

Passive Hero Ability - ok

Normal Unit Ability - crash after score screen

Autocast Unit Ability - crash after score screen

Passive Unit Ability - ok

Active Item Ability - crash after score screen

Passive Item Ability - ok

Doesn't work on position of abilities placed inside spellbooks! - so idea with nested sidebar spellbooks isn't possible.

So far I see one dirty solution for this: player types "quit", game disables selection, forces player to select dummy unit with 7 safe sidebar buttons, shows defeat for him and after that enables selection.

UPD2: Hmm, Reincarnation ability cause crashes after score screen too. I guess all things with cooldowns add unsafe pointers that game tries to clear after score screen. Will try to figure how to avoid this.

UPD3: Seems like all cooldown logic is compiled in game.dll and can not be modified. I guess game saves pointer to a timer for each cooldown, that cleared when score screen displayed, and when game tries to clear all data connected with hero bar it also uses pointer to a wrong location in memory and crash happens. Pretty interesting that game work with placed not hero buttons here anyway.

UPD4: Just noticed that spellbook shows only 11 abilities, even if you totally hide Cancel button. I guess this is hardcoded limitation.

So sidebar can be used to:

- Show useful command buttons that doesn't require often use via shortcuts (i.e. Attack for friendly fire, or Patrol/Hold Position)

- Add up to 7 buildings in Build menu

- Add up to 7 trainable units in buildings

- Add up to 7 upgrades in buildings

- Move hero skill learning to sidebar

- Display important passives (i.e. game rules, or maybe different passives that indicates something and changed with triggers)

- Add up to 7 safe active abilities without cooldowns for any unit:

>> Reveal (Andt) as dummy for ground-target skills.

>> Defend, Magic Defense as dummy for toggleable skills (also it fire order id even if unit is stunned).

>> Shadow Meld as dummy for non-target skills.

>> Root as dummy for unit <-> structure skills.

+ You can make sidebar buttons with any shape using alpha channel, because HeroBar doesn't render any backgrounds behind buttons. ( it's possible to make interesting hero portraits with this knowledge, just don't forget about non-removable black bar behind HP/MP, hero button shiny square glow and skill points counter in bottom right corner )

And if you think that game crash after play session is ok, or you can handle it with tricky triggers:

- Add up to 7 abilities with cooldowns for any unit

And I recommend not to move/hide cancel button, it doesn't give much bonuses (only +1 slot in build menu), but really annoying when you can not close opened spellbook with ESC button.

UPD5: Demo map now use only safe skills - View attachment Sidebar.w3x

P.S. Sidebar works a bit weird with standart save game option: game will save, but if there are ability buttons - game will crash on load. Interesting that game will load if there are only building buttons, but after that everything turns unstable and game can crash in next playsession. So sidebar is suitable for multiplayer or arcade maps only.


 Last edited: Jul 26, 2015

#### Emm-A-

#### Emm-A-

 Level 14

JoinedJul 1, 2008Messages1,314

Resources: 1

#### Resources
Spells 1

Guys, thanks a lot for these amazing findings and elaboration

Just a note: building the buildings from the sidebar does not work for me (newest wc3 patch, win7 64bit, started from JNGP newest version)


 Last edited: Mar 10, 2016

#### Ivorywinter

#### Ivorywinter

 Level 3

JoinedJul 28, 2010Messages38

The latest patch removed this exploit. Does anyone have a solution?

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

Ivorywinter said:

 The latest patch removed this exploit. Does anyone have a solution?

Click to expand...

Yeah, use a spellbook or dummy unit with 0 movement speed as interface with rally as ability for copying point-orders. Both require a little bit of triggering though spellbook does actually cost quite a bit of object editor work if your using over 100 abilities.

Might be other possibilities however I don't have enough knowledge about them yet to explain.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

Ivorywinter said:

 The latest patch removed this exploit. Does anyone have a solution?

Click to expand...

yeah, obvious solution - don't be a blind monkey, you don't need to follow patches at all. there's tons places which won't move from 26/27a

#### LordDz

#### LordDz

 Level 25

JoinedMay 11, 2007Messages4,643

Resources: 8

#### Resources
Maps 8

Ivorywinter said:

 The latest patch removed this exploit. Does anyone have a solution?

Click to expand...

Wait for the next patch, they've said they're going to whitelist this.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

LordDz said:

 Wait for the next patch, they've said they're going to whitelist this.


Click to expand...

nope, they only said that about hiding buttons, not stacking. also author didnt say what exactly he misses

#### Ivorywinter

#### Ivorywinter

 Level 3

JoinedJul 28, 2010Messages38

I've found a solution in Demo Map - Full Command Cart | The Helper

It does a brilliant thing where it takes away a player's alliance control over their own units and uses a spell with a "smart" base order id in place of actual smart orders, which allows them to act normally with triggers. Plus you won't even have a single standard button remaining! However, it's a little difficult to work with since units will issue double smart orders and fire off events twice, and they won't have voice responses, but it's perfectly fine otherwise with a few workarounds. Not sure how well it works with ordering several units at once.

#### leandrotp

#### leandrotp

 Level 9

JoinedJul 30, 2012Messages156

Resources: 1

#### Resources
Tutorials 1

Ivorywinter said:

 Not sure how well it works with ordering several units at once.


Click to expand...

That system makes it completely impossible for the player to select multiple units at once. So it's useless except for maps where the player doesn't need to control more than one unit.

LordDz said:

 Wait for the next patch, they've said they're going to whitelist this.


Click to expand...

Can someone (@Kam?) please tell Blizzard that whitelist is not the solution? I've read the topic at battle.net, where he says they're gonna whitelist some values. First, the (0, -11) combination IS NOT SAFE, and should not be whitelisted. Second, there are many combinations of values that allow for button manipulation, like -2147483648,-268435456 or -268435440, and by now many maps are using different numbers, so it would be difficult for them to whitelist every possible combination.

In my opinion, the best solutiion would be not to touch it at all, and revert it to what it was before 1.27b, I mean, no safety checks (just let the community care about that). But if they are really paranoid with this thing, they should implement a check that uses a bitmask, to allow only the highest negative values, instead of manually whitelisting each number.

Btw, I really think they should keep this thing like it was before the patch. I've been thinking of a way to combine this trick with memory reading, to implement the Hero Bar buttons in a way that would be 100% safe. I'm pretty sure it would be the best for everyone it there was no safety check on button positions.

#### Kam

#### Kam

 Level 45

JoinedAug 3, 2004Messages2,912

Resources: 44

#### Resources
Models 10Icons 2Maps 29Reforged HD Models 1Ported Models 1Tutorials 1

leandrotp said:

 That system makes it completely impossible for the player to select multiple units at once. So it's useless except for maps where the player doesn't need to control more than one unit.

Can someone (@Kam?) please tell Blizzard that whitelist is not the solution? I've read the topic at battle.net, where he says they're gonna whitelist some values. First, the (0, -11) combination IS NOT SAFE, and should not be whitelisted. Second, there are many combinations of values that allow for button manipulation, like -2147483648,-268435456 or -268435440, and by now many maps are using different numbers, so it would be difficult for them to whitelist every possible combination.

In my opinion, the best solutiion would be not to touch it at all, and revert it to what it was before 1.27b, I mean, no safety checks (just let the community care about that). But if they are really paranoid with this thing, they should implement a check that uses a bitmask, to allow only the highest negative values, instead of manually whitelisting each number.

Btw, I really think they should keep this thing like it was before the patch. I've been thinking of a way to combine this trick with memory reading, to implement the Hero Bar buttons in a way that would be 100% safe. I'm pretty sure it would be the best for everyone it there was no safety check on button positions.


Click to expand...

I have passed it on. We have discussed this before so I know it's being worked on in some capacity.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

well I'd like to have invalid params just to drop hackers who're able to see that icon on enemy hero. but who I am, blizzard have greater deeds to do, like dx11 support for extra shaders on a fukin fire sprites

#### Kam

#### Kam

 Level 45

JoinedAug 3, 2004Messages2,912

Resources: 44

#### Resources
Models 10Icons 2Maps 29Reforged HD Models 1Ported Models 1Tutorials 1

@leandrotp

A response from Blizzard per your request:

"

I am whitelisting the value, but the internal logic is different

If it sees the value it doesn't attempt to register the button at all

So from the outside it looks like the trick still will work; but in reality it is not actually do the same thing that it was doing in 1.27a

I didn't want to introduce a new api and I didn't want to break maps that used it.

"

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

Kam said:

 I didn't want to introduce a new api


Click to expand...

nice attitude we have there. goddamit, this company used to be best of the best :C

#### Kam

#### Kam

 Level 45

JoinedAug 3, 2004Messages2,912

Resources: 44

#### Resources
Models 10Icons 2Maps 29Reforged HD Models 1Ported Models 1Tutorials 1

DracoL1ch said:

 nice attitude we have there. goddamit, this company used to be best of the best :C


Click to expand...

The primary concern in all of this is to leave previous maps in tact. I am not familiar with the intimate details, but the decision was made to ensure continuing functionality. I can assure you this team is very dedicated and there is a good reason for the decisions they make.

 You must log in or register to reply here.


### Similar threads

Locked

 Removing standart command buttons with CommandFunc.txt


ALEXz407

Jan 16, 2015

Advanced Scripting

Replies19Views11K

Feb 17, 2015
edo494

Locked

 How to move button positions of Atack/Stop/Hero Abilities


ALiEN95

Jul 14, 2010

World Editor Help Zone

Replies8Views8K

Jul 18, 2010
-SoulBurn

Locked

 New Hide command buttons


kingkwong

Feb 2, 2016

World Editor Help Zone

Replies2Views3K

Feb 6, 2016
kingkwong

Locked

 How can I change buttonpos forcibly?


WilliamBz

Sep 28, 2024

World Editor Help Zone

Replies1Views742

Sep 30, 2024
HerlySQR

Locked

 Button stacking *patched* ?


mietek

Dec 14, 2016

Warcraft Discussion

Replies16Views4K

Dec 19, 2016
Dr Super Good

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