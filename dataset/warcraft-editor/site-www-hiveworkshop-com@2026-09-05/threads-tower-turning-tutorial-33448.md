---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/tower-turning-tutorial.33448/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Tower Turning Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/tower-turning-tutorial.33448/>
#### DragoonZombie

#### DragoonZombie

 Level 11

JoinedMar 24, 2005Messages431

Resources: 3

#### Resources
Models 1Icons 1Tutorials 1

Tower Turning Tutorial


by DragoonZombie

Introductions:

In this tutorial I will show you how to make (non-)custom towers turn to his target when attacking.​

Requirements:

WarCraft 3 World Editor​

Step I:

Go to the Object Editor and create/edit the Root (Ancients) ability (Abilities -> Nightelf -> Units -> Root (Ancients)) like this:

​

Step II:

Now create a new Spell Book ability (Abilities -> Special -> Items -> Spellbook) and add our modified Root ability to it. Make sure that the Spellbook is a Unit ability by unchecking 'Stats - Item Ability'

​

Step III:

Now its time for the Tower.


I recommend using a UNIT as Tower (NOT Building). We will convert the unit to a building later. Because when we use a unit we have the standard settings optimized for the tower model and we can change the height of the tower.


Pick a random Unit you want to have as tower and give the tower the spell book ability we created. You also have to change the towers movement speed and turn rate (IMPORTANT). I set the movement speed to 1, so that the Tower is able to turn (if movement speed is 0, WE thinks that the unit/building/whatever is unable to move and disable its turning).


And the turn rate should at least be greater than 0, so it is able to turn (no wai!)


Now its time to convert the unit to a building.


That was the conversion. Simple eh?


You now have to give the tower the 'Spellbook' ability we made, via Triggers or in the object editor.


Since its a building now, you can change the Ground Texture and Pathing Map of the Tower.


In order to hide the Spell Book go to the Trigger Editor and disable the Spell Book ability for the owner of the tower:

Melee Initialization
Events
Map initialization

Conditions

Actions
Player - Disable Turning Tower for Player 1 (Red)

Step IV (optional):

To change the towers height you simply change the Movement - Type to Hover and fly height to the height you want.


Now edit Art - Elevation - Sample Points and set it to 0 and you're set.

​

Done.


Happy Mapping


 Last edited: Nov 11, 2009

#### Mini-Me

#### Mini-Me

 Level 17

JoinedOct 30, 2004Messages1,277

Resources: 10

#### Resources
Maps 10

Nifty trick that works.

-You should explain where you find those abilities. Example: Root --> Night Elf ---> Units

Spellbook --> Special ---> Items

-You also leave out some details, like you need to turn the Spellbook to an unit ability.

-The pictures in step III are bad. You cant make out much.

The trick itself is simple, people just need to know about it and I think this tutorial does its job well. Make those small improvements I mentioned and I would aprove this.

#### DragoonZombie

#### DragoonZombie

 Level 11

JoinedMar 24, 2005Messages431

Resources: 3

#### Resources
Models 1Icons 1Tutorials 1

wow, someone replied... even if its late. But i dont complain.

I edited the post, but what is wrong with the pictures in step three?

#### Mini-Me

#### Mini-Me

 Level 17

JoinedOct 30, 2004Messages1,277

Resources: 10

#### Resources
Maps 10

I guess they are ok . Well this gets my aproval.

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Tutorial Approved

~ Thread moved to General Mapping Tutorials.

#### Lingonberryjam

#### Lingonberryjam

 Level 6

JoinedFeb 25, 2005Messages230

Great turorial DragoonZombie!


All i can find is (what i think) two minor grammar flaws;


 DragoonZombie said:



 I set the movement speed to 1, that the Tower is able to turn (if movement speed is 0, WE things that the unit/building/whatever is unable to move and disable its turning).


Click to expand...


What i guess it should read is:


 DragoonZombie said:



 I set the movement speed to 1, so that the Tower is able to turn (if movement speed is 0, WE thinks that the unit/building/whatever is unable to move and disable its turning).


Click to expand...


... and one thing more. You say:


"You also have to change the towers movement speed and turn rate (IMPORTANT). "


... and say what you set the movement speed to, but not what you set the turn rate to. Guess all you have to say is that the turn rate should be > 0.


Since I'm working on a TD-wars and already had alot of towers using unitmodels this is just what i need!


Thanks for a great turorial!


EDIT-


Oh, forgot to mention... +rep!

#### DragoonZombie

#### DragoonZombie

 Level 11

JoinedMar 24, 2005Messages431

Resources: 3

#### Resources
Models 1Icons 1Tutorials 1

thanks. I know that my english isnt the best, but i just wanted to make it understandable. ( Edited )

I thought that turn rate > 0 is self-explanatory. Oo

#### Lingonberryjam

#### Lingonberryjam

 Level 6

JoinedFeb 25, 2005Messages230

That it should have > 0 is self-explanatory, indeed , but if you have the tower based on a unit it already has a turning rate. And then you say "you must change the turn rate (IMPORTANT)". It clearly doesnt have to be changed, just to be > 0.

#### Zanam

#### Zanam

 Level 4

JoinedAug 7, 2007Messages75

Resources: 1

#### Resources
Models 1

I really liked this idea, so I went ahead and tried it out. The tower turning works great, however there is one problem I noticed, but it may be because of something I missed when turning my unit into a tower. You can stack the towers on top of each other, when building a new one, the square doesn't register a building there and allows you to build away.


Is there a way to prevent this?

#### DragoonZombie

#### DragoonZombie

 Level 11

JoinedMar 24, 2005Messages431

Resources: 3

#### Resources
Models 1Icons 1Tutorials 1

you have to make sure that you set the 'Pathing Map'

#### Zanam

#### Zanam

 Level 4

JoinedAug 7, 2007Messages75

Resources: 1

#### Resources
Models 1

I have, it's the same pathing map as the other towers in this map have. A barrel doodad, making it a 1x1 tile. The rest of the towers don't stack, but this one seems to be stubborn

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

I love this tutorial.

...just thought I'd say that.

#### Cleo

C

#### Cleo

I also got a Problem: when i upgrade a tower (wuth the turning ability), it wont attack automatically any more. I have to give it the attack order once on a target, after that it will attack as normal every enemy that comes in range.

Does any1 have the same Prob?

#### Rebuke

R

#### Rebuke

If your towers are stacking you have to do the following in the object editor


Art -> Ground Textures -> Arcane Tower, Cannon Tower, Guard Tower, Farm...


Pathing -> Pathing Map -> Arcane Tower, Cannon Tower, Guard Tower, Farm...


Pathing -> Placement requires -> Buildable


Your towers shouldn't stack anymore.

#### GamerSmurf

#### GamerSmurf

 Level 4

JoinedOct 31, 2013Messages55

Resources: 1

#### Resources
Maps 1

DragoonZombie said:

 I recommend using a UNIT as Tower (NOT Building)


Click to expand...

Do you have to use a unit? I have 70 tower in my TD and it would be great to make them able to turn. When I follow the tutorial on a Guard Tower with a unit model, the game closes as soon I strat building the tower.

Have I done something wrong or do you have to use a unit and turn it into a building?

#### Slaani

#### Slaani

 Level 6

JoinedJul 19, 2018Messages16

Resources: 3

#### Resources
Skins 3

Let's pop this 13 yo thread! Necroposting at its finest, wonder if anyone is going to answer.

Okay, so semirelated topic, (this is on Warcraft 1.30, because I don't trust newer versions) I'm attempting to make a ground submerged tower.

Looks fine in Editor:

but not so much when I run it in game:

I've achieved the ground submergence by altering the "Movement - Height" stat of the building Unit in Object Editor. Any tips on how to achieve what I'm aiming for?

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

movement height hasn't been working since I dunno, 1.27 or something?

I'd say a better model with only the cannon would do the job. I'm sure some of the modeller can easily cut it.

#### Daniel Kopp

#### Daniel Kopp

 Level 2

JoinedNov 16, 2019Messages12

Tried this little trick out on the TD Im making and it works like a charm!

It's a good fix for the "Reforged units bending i half" problem..

I did do one thing diiffrent, I did not go the spellbook route, instead just moved the Icon away like this:

I guess it's pritty much the same.

Thanks!

#### Lezeires

#### Lezeires

 Level 1

JoinedMay 23, 2018Messages2

Resources: 1

#### Resources
Maps 1

just asking, why tf do we need to create the speelbook thing? cant the tower be turned just from the turn rate and movement speed alone?

 You must log in or register to reply here.


### Similar threads

D

Locked

 Turning Structures


Deleted member 219079

Dec 22, 2016

World Editor Help Zone

Replies2Views1K

Dec 22, 2016
Deleted member 219079

D

 Basics of a Tower Defense


ragingspeedhorn

Mar 2, 2006

Misc/General Mapping Tutorials

Replies8Views48K

Mar 11, 2015
Embraced

Locked

 Tiny problem.


harkal

Sep 5, 2009

World Editor Help Zone

Replies18Views2K

Sep 7, 2009
harkal

Locked

 Rifleman set as building not turning


Voltron212

Mar 21, 2024

World Editor Help Zone

Replies10Views2K

Mar 23, 2024
Voltron212

 How to make a building float


Wing-Span

Jan 14, 2026

Tutorial Submission

Replies2Views512

Jan 16, 2026
Wing-Span

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