---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/altering-hero-stats-for-noobs.233834/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Altering Hero Stats for Noobs
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/altering-hero-stats-for-noobs.233834/>
#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

ALTERING STATS FOR NOOBS

~Created by Archange1

REQUIREMENTS

A medium-level understanding of the World Editor

Understanding of the creation of basic Events, Conditions, and Actions

The ability to read and process information

An idea of what you want to do in your map

CREDITS

Chaosy

PurgeandFire111

[TD]
I
NTRODUCTION


The purpose of this tutorial is to show you how to add stats from a hero using a variety of methods.


This is different from the normal green text that you'll see next to a hero's stats.


This is a PERMANENT bonus.


Despite the title, I WILL be showing other methods of doing such things with simple ease.


I'll also be explaining how to remove stats and possibly re-add the stat points to the hero.OVERALL DIFFICULTY - EASY
[/td][/tr]

METHOD ONE - Adding Stats Using Hero AbilitiesMETHOD TWO - Adding Stats Using Dialog Buttons

~Method Difficulty - EASY~Method Difficulty - EASY

With this method, every time an ability is learned by the hero, their chosen stat will increase by X,


with X being a number of your choosing to increase the stats by.


To start, you'll want to create 3 dummy abilities based off of Attribute Bonus.


The base ability doesn't HAVE to be attribute bonus, simply one that can be 'hidden'.


Next, set the max level of each ability to the game's max hero level divided by 3.


If you don't set the max level high enough, some stat points may be lost.


To avoid this, have enough total available levels to exceed the maximum hero level.


Next you'll create a trigger similar to this, changing values as needed.

Stat Gain
Events
Unit - A unit Learns a skill

Conditions
Or - Any (Conditions) are true
Conditions
(Learned Hero Skill) Equal to Gain Strength

(Learned Hero Skill) Equal to Gain Agility

(Learned Hero Skill) Equal to Gain Intelligence

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Learned Hero Skill) Equal to Gain Strength

Then - Actions
Hero - Modify Strength of (Triggering unit): Add X

Unit - Remove Gain Strength from (Triggering unit)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Learned Hero Skill) Equal to Gain Agility

Then - Actions
Hero - Modify Agility of (Triggering unit): Add X

Unit - Remove Gain Agility from (Triggering unit)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Learned Hero Skill) Equal to Gain Intelligence

Then - Actions
Hero - Modify Intelligence of (Triggering unit): Add X

Unit - Remove Gain Intelligence from (Triggering unit)

Else - Actions


Now let me explain this a little bit.


This part is used for making sure the trigger isn't fired every time a unit gains an ability.

Or - Any (Conditions) are true
Conditions
(Learned Hero Skill) Equal to Gain Strength

(Learned Hero Skill) Equal to Gain Agility

(Learned Hero Skill) Equal to Gain Intelligence


It saves on memory a little bit, but really isn't required.


This next part is used for ensuring it's choosing the correct stat.

(Learned Hero Skill) Equal to Gain Stat


That part IS required, so don't miss it.


After that, we go through the actualy setting of the stats, and the removal of the ability.

Then - Actions
Hero - Modify Stat of (Triggering unit): Add X

Unit - Remove Gain Stat from (Triggering unit)


Now, obviously here, "Stat" is the stat that you want to increase.


It corresponds to the ability you used to trigger the increase.


If you want to be able to SUBTRACT stats from the hero, simply change "Add" to "Subtract" like the example below.

Hero - Modify Stat of (Triggering unit): Subtract X


If you want to refund the Attribute Point back to the hero create the following action.

Hero - Modify unspent skill points of (Triggering unit): Add 1 points


You'll need to associate this with a seperate ability, likely just a copy with a different name/description.

This method is STRICTLY an example, and can easily be improved upon.


It's used more to show you the things to consider when making your own system.


This system essentially does the same thing as method one,


without tying up the option to use hero abilities.


It creates a simple dialog with four buttons allowing you to gain stats a different way.


The downside to this is it requires more than one trigger to function.


To start, you should create a few variables that I've listed for you below.

NameTypeArray

HeroUnitFalse

DialogDialogFalse

DialogButtonDialog ButtonFalse

AttPointIntIntegerFalse


After creating all the variables, you'll need to configure your dialog similar to this.

DialogCreate
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
Set Hero = Paladin 0001 <gen>

Dialog - Clear Dialog

Dialog - Hide Dialog for Player 1 (Red)

Dialog - Change the title of Dialog to (Available Attribute Points: + (String(AttPointInt)))

Dialog - Create a dialog button for Dialog labelled Strength

Set DialogButton[1] = (Last created dialog Button)

Dialog - Create a dialog button for Dialog labelled Agility

Set DialogButton[2] = (Last created dialog Button)

Dialog - Create a dialog button for Dialog labelled Intelligence

Set DialogButton[3] = (Last created dialog Button)

Dialog - Create a dialog button for Dialog labelled Cancel

Set DialogButton[4] = (Last created dialog Button)


"Hero" is the unit you want to gain the stats.


You don't have to set it here, but keep in mind it will be used later.


Next you want to create the trigger that will show the dialog.


Again, this is a really simple version, and can easily be improved upon.

Dialog Show
Events
Player - Player 1 (Red) skips a cinematic sequence

Conditions

Actions
Dialog - Show Dialog for Player 1 (Red)


With this, when the trigger player presses ESC then the dialog will be shown.


It's really as simple as that, although you can put more into it.


Now, before we go into the dialog buttons, we'll need to create a counter.


The example below is an EXTREMELY basic counter but it works well enough.

PointTracking
Events
Unit - A unit Gains a level

Conditions
((Triggering unit) is A Hero) Equal to True

Actions
Set AttPointInt = (AttPointInt + 1)


Every time your hero gains a level, they'll get one more point to spend.


Next you'll want to create the handle for the buttons.


This will make the dialog buttons actually do something when they're clicked.

Dialog Activate
Events
Dialog - A dialog button is clicked for Dialog

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to DialogButton[1]

AttPointInt Greater than 0

Then - Actions
Set AttPointInt = (AttPointInt - 1)

Hero - Modify Strength of Hero: Add 1

Dialog - Hide Dialog for Player 1 (Red)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to DialogButton[2]

AttPointInt Greater than 0

Then - Actions
Set AttPointInt = (AttPointInt - 1)

Hero - Modify Agility of Hero: Add 1

Dialog - Hide Dialog for Player 1 (Red)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to DialogButton[3]

AttPointInt Greater than 0

Then - Actions
Set AttPointInt = (AttPointInt - 1)

Hero - Modify Intelligence of Hero: Add 1

Dialog - Hide Dialog for Player 1 (Red)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to DialogButton[4]

Then - Actions
Dialog - Hide Dialog for Player 1 (Red)

Else - Actions


This will work, but I must express that this is only a BASIC model of the function.

OTHER TUTORIALS & SYSTEMS

These are related, and may be of some use to those seeking out this sort of information.

 |

 JASS:

 Custom Stat System (CSS) v1.5g[/url] |
| [url=https://www.hiveworkshop.com/forums/trigger-gui-editor-tutorials-279/mui-spells-using-artificial-waits-223315/]MUI Spells Using Artificial Waits[/url] |
|[url=https://www.hiveworkshop.com/forums/spells-569/stats-system-1-04-a-213828/]Stats System [1.04] by .OmG.[/url] |[/center][/td][/tr][/TABLE][/center]


 Last edited: May 21, 2013

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

~Reserved


 Last edited: May 10, 2013

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

There I am!

Since the trigger is simple I couldn't find something wrong there.

And you know which tutorial I suggested you to link.

Great job so far... sadly creating it made me sleep xd

edit: tell me the problem on steam I might have a solution


 Last edited: May 1, 2013

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

You love noobs

I would suggest that looking up this system : https://www.hiveworkshop.com/forums...css-v1-5g-229885/?prev=d=list&r=20&u=Doomlord

Because this system allows you to modify an attribute

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

But when noobs see the word Jass, they run screaming into their holes.

This is specifically for them, otherwise I would have done some improving upon the systems.

#### Binsu

#### Binsu

 Level 30

JoinedNov 29, 2012Messages6,631

Resources: 5

#### Resources
Maps 4Templates 1

Glory to the noobs!


On to topic, very useful for those NOOBS who dont know how to do that. But still helps a little for those that know still.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

It looks really nice right now (aesthetically and writing-wise). I think I need just a few points of clarification though:

 To start, you'll want to create 3 dummy abilities based off of Attribute Bonus.


Click to expand...

- Why do you need to base it off Attribute Bonus? Couldn't it work with basing it off of channel? If it is for convenience (as in, you have to modify fewer fields), then you may want to mention that.

Er, actually that is about it.

As for some suggestions (if you end up using that reserved post, or extending the tutorial), I think it would be really cool to cover adding attributes such that they show up as "+ 5" in the command card. For example, when you get claws of attack, it'll show up as "+ 11" damage (or something similar) in green text next to the unit's attack damage. You can achieve this with item abilities. See this system for a reference:

http://www.wc3c.net/showthread.php?t=107940

You don't actually have to delve in the code, but if you look in the object editor you can see how the item abilities work.

However, that is a bit complicated. Again, you don't have to, but it would be a cool tutorial since I haven't seen anyone cover it yet. I don't remember what items they base it off of, but you basically just add one to the unit and they'll keep the stat bonus. (remove it to remove the bonus) You don't have to make the ones like bonusmod does it (with +2, 4, 8, 16, etc.. bonuses), you can just show an example with maybe +5 or +10 for a certain stat.

All things considered, this tutorial looks approvable in its current state.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Archange1 said:

 But when noobs see the word Jass, they run screaming into their holes.


This is specifically for them, otherwise I would have done some improving upon the systems.


Click to expand...


So they're scared? No!!!

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2


 PurgeandFire111 said:



 - Why do you need to base it off Attribute Bonus? Couldn't it work with basing it off of channel? If it is for convenience (as in, you have to modify fewer fields), then you may want to mention that.


Click to expand...

It doesn't HAVE to be, simply another ability that can be hidden from the command card.

Even tho the system removes the ability after learning it, sometimes lag will cause it to temporarily appear in the command card.

That, and it may push or pull other ability icons around if it's not hidden.

In any case, I updated the comment there.


 PurgeandFire111 said:



 I think it would be really cool to cover adding attributes such that they show up as "+ 5" in the command card.


Click to expand...

Isn't that what the Attribute Bonus ability does? Or did I just misunderstand you... ;p

Although, it could be useful for altering damage & armor that way, rather than using auras.

I think I might include that.


 PurgeandFire111 said:



 All things considered, this tutorial looks approvable in its current state.

Click to expand...

Grazie! <3

EDIT: I also added a tutorial and another system to the list, as they both could be used to create something similar to this.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Archange1 said:

 It doesn't HAVE to be, simply another ability that can be hidden from the command card.


Even tho the system removes the ability after learning it, sometimes lag will cause it to temporarily appear in the command card.


That, and it may push or pull other ability icons around if it's not hidden.


In any case, I updated the comment there.

Click to expand...


Oh I see. I didn't notice that attribute bonus had that field.


 Archange1 said:



 Isn't that what the Attribute Bonus ability does? Or did I just misunderstand you... ;p


Although, it could be useful for altering damage & armor that way, rather than using auras.


I think I might include that.


Click to expand...


Yeah, it does. I didn't test it before. But the issue I have is that I don't see the point of the trigger. The attribute bonus ability gives the stats, but then you remove it and modify the stat with the trigger. (and thus, it loses the +3 green part next to the stat) So I don't see why the ability "middle-man" has to be there aside from being learned.


Although, that also implies that the trigger can be considered a middle man. Since the spell is hidden, you shouldn't have to go through that process, right? *Unless you don't want it to have that green text. Then I understand.


I hope you understand the source of my confusion. Do you have a test map?

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

Yeah the whole point is to eliminate that gross green text.

It makes it easier for ORPG's namely, which is what I happen to be working on.

It happens to be a dumbed-down version of the system I'm using.

The purpose is to avoid having to seperate the stat bonus that WOULD show up in green text from Attribute Bonus, from item stat bonuses.

This makes it a permanent addition to the hero instead of just a temporary +X next to the stat.

As it's not temporarily given to the hero, it can be saved / loaded a whole lot easier without having to mess around with saving the levels of each ability.

As for a test map, I can give you an earlier version of my ORPG before I improved on the system. It's fairly close to this now.

I'd prefer to give it to you in PM tho, so just let me know if you'd like it.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Ah, no need. I see what you mean now, and it makes sense.

You may want to mention that in the tutorial, just to people will be clear why you go through the process of having the dummy ability + the trigger.

Also, is this finished or WIP?

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

Okay, I updated the introduction with that.

This tutorial is finished, although if you had any more suggestions I would be more than happy to add them.

I would like to point out that posting a tutorial about something so basic was pretty embarassing!

But I got enough questions about this kind of thing I figured it would be a good idea. x)

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Approved.

And no worries about the tutorial being about a simple subject. As long as it is useful (which it is), then it is fine.

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

Lol thank you.

I appreciate the suggestions you've given!

#### dart_vaider

#### dart_vaider

 Level 2

JoinedJul 18, 2011Messages15

Resources: 1

#### Resources
Tutorials 1

Request form #DH156C

INQUIRY

On the subject of the proceeding of "METHOD ONE - Adding Stats Using Hero Abilities", derived from the tutorial topic "Altering Hero Stats For Noobs", submitted by user designation "Archange1".


Is the specification of that method to be executed once, and only once, for each heroic statistic, when learned, or is it intended to be a repeating action with multiple iterations for each heroic ability score?

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

dart_vaider said:

INQUIRY

On the subject of the proceeding of "METHOD ONE - Adding Stats Using Hero Abilities", derived from the tutorial topic "Altering Hero Stats For Noobs", submitted by user designation "Archange1".


Is the specification of that method to be executed once, and only once, for each heroic statistic, when learned, or is it intended to be a repeating action with multiple iterations for each heroic ability score?

Click to expand...


Shit.. I thought I was fancy.


In method one, the stat only goes up by ONE every time you level the ability. However, there are as many levels as you need for each ability, so you could increase the hero's stats as many times as you need.


The value of the increase, which is one in my example, could easily be changed.


Not sure if that's what you were asking, but let me know if you still need help.

#### dart_vaider

#### dart_vaider

 Level 2

JoinedJul 18, 2011Messages15

Resources: 1

#### Resources
Tutorials 1

Archange1 said:

 Shit.. I thought I was fancy.


In method one, the stat only goes up by ONE every time you level the ability. However, there are as many levels as you need for each ability, so you could increase the hero's stats as many times as you need.


The value of the increase, which is one in my example, could easily be changed.


Not sure if that's what you were asking, but let me know if you still need help.

Click to expand...

Indeed, you have provided an answer to my query.


However I believe there is a problem with the aforementioned "Method One".


The trigger states that after learning said ability, for example "Strength Gain", the Strength value of the hero is incremented and the ability is removed. This would entirely remove the ability, barring any future possibility for advancing its levels.


Therefore the hero will only be able to increase his strength once.

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

No that's not true.

When you remove the ability, it doesn't remove the option to learn it because it's a hero ability.

What it does is removes the current value of the ability on the unit, without altering the unit's ability to learn said ability. c;

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

laugh at For Noobs part xD

nice tutorial

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

Daffa the Mage said:

 laugh at For Noobs part xD


nice tutorial

Click to expand...


well shanks c;

#### dart_vaider

#### dart_vaider

 Level 2

JoinedJul 18, 2011Messages15

Resources: 1

#### Resources
Tutorials 1

Archange1 said:

 No that's not true.


When you remove the ability, it doesn't remove the option to learn it because it's a hero ability.


What it does is removes the current value of the ability on the unit, without altering the unit's ability to learn said ability. c;


Click to expand...

Indeed you are correct.


It seems I have committed a most grave error, forgive me.

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

dart_vaider said:

Indeed you are correct.


It seems I have committed a most grave error, forgive me.

Click to expand...


its ok :]

#### Aetraxos

#### Aetraxos

 Level 3

JoinedAug 11, 2012Messages54

Thank you so much for the tutorial. Great help!

#### XxkosumaxX

#### XxkosumaxX

 Level 2

JoinedSep 21, 2015Messages15

Hello i have gone with implementing method 2 within my map and i have double checked my work. Everything works just fine and executes except the count on the dialog box menu. If i level twice, then the counter computes that i have two stat points, but the displayed menu count stays at zero. example: Available Stat Points 0. No matter how many times i level and gain the points...the visual count stays at zero. Any help towards this issue would be most appreciated. Thank you Hive Community for your time and effort!

 You must log in or register to reply here.


### Similar threads

 Dialog Buttons (Advanced Systems)


-Kobas-

Jan 3, 2011

Trigger (GUI) Editor Tutorials

Replies15Views24K

Jan 30, 2024
Njord

 Hero Selection Systems


Eleandor

Aug 20, 2007

Trigger (GUI) Editor Tutorials
2

Replies51Views81K

Feb 28, 2021
Caalanuaa

 Hero Selection Systems


Tonks

Mar 23, 2007

Trigger (GUI) Editor Tutorials

Replies12Views18K

Jun 8, 2023
dantles1992

Locked

[Trigger]
 A trigget Fill up


tw3ye

Feb 12, 2011

Triggers & Scripts

Replies7Views2K

Feb 12, 2011
tw3ye

Locked

[Trigger]
 Dialog for Gaining Attributes


HollowedByName

May 12, 2009

Triggers & Scripts

Replies2Views1K

May 12, 2009
HollowedByName

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Trigger (GUI) Editor Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top