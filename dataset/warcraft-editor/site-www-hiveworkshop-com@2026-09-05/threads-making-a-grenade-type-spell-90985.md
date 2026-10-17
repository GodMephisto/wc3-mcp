---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-a-grenade-type-spell.90985/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making a Grenade Type Spell
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-a-grenade-type-spell.90985/>
#### PrisonLove

#### PrisonLove

 Level 10

JoinedSep 29, 2006Messages447

Resources: 5

#### Resources
Maps 4Tutorials 1

#
 Grenade Type Spells

Okay I’m going to show you guys how to make a grenade type spell. This is not an introduction to dummy units or dummy spells, I’m assuming you know how to make them. You also need basic triggering knowledge for this.

What do I mean by this? Well, a grenade type spell (as far as I’m concerned) is a spell in which a unit throws a projectile to a target location, and then the projectile explodes, damaging everything in an area.

##
 1. Setting Everything Up

Now, to do this we’re going to need to make three abilities, and 2 dummy units. Here’s the spells we’ll need:

The caster’s dummy ability (based off of channel)

A projectile ability for one dummy (based off of acid bomb)

An ability to deal the damage for the other dummy (based off of thunderclap)

Then we need two dummies:

A traditional dummy with the locust ability

A copy of that dummy, except without locust (I’ll explain why later)

Now at this point you may be asking yourself, “why not just trigger the damage?”

Well, here’s my answer. It’s much easier just to create a separate ability with all of the levels and damage amounts you want, and then order a dummy unit to cast it. Otherwise, we’d have to come up with some type of formula and use if/then/else statements to trigger the damage. I think that having a dummy use and ability is much easier. The only drawback to this is that there is spell damage reduction. However, since we’re making a spell that should happen anyway. Also, if you’re a fan of using raw data in tooltips, it makes it easier to change the tooltips. We also avoid using unit groups which is a plus.

Okay now on to the good stuff.

I’m assuming you know how to make dummy casters. So make them, give one locust and keep the other one without locust but all the same properties. You need one without locust because we’re going to have dummy 1 “acidbomb” dummy two; therefore, dummy 2 needs to be vulnerable.

So for the custom acid bomb spell, make it so that it deals no damage, has no duration, no AoE (very important), no negative effects, etc. the only thing we need is the projectile art. Set it to what you want. I prefer the Human Battleship projectile. Make sure the spell can target friends (VERY important). Make sure the mana cost and cooldown are 0 and such.

Now we need to make our thunderclap spell.

Q: Why are we using thunderclap and not warstomp?

A: Well, this is a matter of preference but, Thunderclap will not break channeling spells so if you set the duration to .01 the side effects will not take place. Also you can set the debuffs to 0.

So for thunderclap, set the targets to what you want (I have it set so that it can target ALL ground units). Set the debuffs to 0 (unless you want a disorienting grenade) and set the damage to what you want. Make sure the mana cost and cooldown are 0 and such.

Now we want to make our dummy ability for the caster, complete with mana, cooldown, and tooltips. I suggest using channel with a targeting image. Make sure the AoE of your dummy ability and thunderclap are the same!

Okay so our abilities and dummy units are all set, now its time to start triggering!

##
 2. The Triggering

First we need to set up our basic spell trigger (I’ve titled my spell, Grenade Launch):

Grenade Launch
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Grenade Launch (Heavy Machine Gunner)

now it’s on to the slightly harder part – the actions. What we’re going to do is create two dummies. One at the location of the caster (which is dummy 1 mentioned before) and another at the target point of ability being cast (the vulnerable dummy). Then we’re just going to give them the needed abilities, set them to the needed levels, and give them some expiration timers to remove the leak.

So here’s what our actions are going to look like for now

Actions
Set TempPoint = (Position of (Triggering unit))

Set TempPoint2 = (Target point of ability being cast)

Unit - Create 1 dummy unit for (Owner of (Triggering unit)) at TempPoint facing Default building facing degrees

Set TempUnit = (Last created unit)

Unit - Add grenade projectile to TempUnit

Unit - Add a 6.00 second Generic expiration timer to TempUnit

Unit - Create 1 dummy unit (vulnerable) for (Owner of (Triggering unit)) at TempPoint2 facing Default building facing degrees

Set TempUnit2 = (Last created unit)

Unit - Add grenade explosion to TempUnit2

Unit - Set level of grenade explosion for TempUnit2 to (Level of Grenade Launch (Heavy Machine Gunner) for (Triggering unit))

Unit - Add a 6.00 second Generic expiration timer to TempUnit2

So all we’ve done is set the points, add the units, add the abilities and the expiration timers, and set the dummies to variables. We need to set them to variables because in a few lines we’re going to have a wait, at which point we can no longer use Last Created Unit. Its also just much easier to refer to the dummies if their in variables.

The next part of this trigger will be creating the projectile and dealing the damage. Now, what we’re going to do is order TempUnit to “acidbomb” TempUnit2. This will give us our projectile. Then we’re going to wait the distance between TempPoint and TempPoint2 divided by the projectile speed of the acid bomb projectile. Then we just order TempUnit2 to “thunderclap” and add some nifty special effects. Not too hard right. So lets take a look at the actions now:

Actions
Set TempPoint = (Position of (Triggering unit))

Set TempPoint2 = (Target point of ability being cast)

Unit - Create 1 dummy unit for (Owner of (Triggering unit)) at TempPoint facing Default building facing degrees

Set TempUnit = (Last created unit)

Unit - Add grenade projectile to TempUnit

Unit - Add a 6.00 second Generic expiration timer to TempUnit

Unit - Create 1 dummy unit (vulnerable) for (Owner of (Triggering unit)) at TempPoint2 facing Default building facing degrees

Set TempUnit2 = (Last created unit)

Unit - Add grenade explosion to TempUnit2

Unit - Set level of grenade explosion for TempUnit2 to (Level of Grenade Launch (Heavy Machine Gunner) for (Triggering unit))

Unit - Add a 6.00 second Generic expiration timer to TempUnit2

Unit - Order TempUnit to Neutral Alchemist - Acid Bomb TempUnit2

Wait ((Distance between TempPoint and TempPoint2) / 700.00) seconds

Unit - Order TempUnit2 to Human Mountain King - Thunder Clap

Special Effect - Create a special effect at TempPoint2 using war3mapImported\ShrapnelShards.mdx

Special Effect - Destroy (Last created special effect)

Special Effect - Create a special effect at TempPoint2 using Abilities\Spells\Other\Incinerate\FireLordDeathExplode.mdl

Special Effect - Destroy (Last created special effect)

Now you may have noticed something odd in that trigger:

Special Effect - Create a special effect at TempPoint2 using war3mapImported\ShrapnelShards.mdx
This special effect is one I imported for the purpose of my spell. If you don’t have a custom explosion to use I suggest using neutral building explosion.

Now just for one more thing. We need to remove all of those nasty leaks. So we add this to the end of the code:

Custom script: call RemoveLocation(udg_TempPoint)

Custom script: call RemoveLocation(udg_TempPoint2)
so lets take a look at the whole trigger:

Grenade Launch
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Grenade Launch (Heavy Machine Gunner)

Actions
Set TempPoint = (Position of (Triggering unit))

Set TempPoint2 = (Target point of ability being cast)

Unit - Create 1 dummy unit for (Owner of (Triggering unit)) at TempPoint facing Default building facing degrees

Set TempUnit = (Last created unit)

Unit - Add grenade projectile to TempUnit

Unit - Add a 6.00 second Generic expiration timer to TempUnit

Unit - Create 1 dummy unit (vulnerable) for (Owner of (Triggering unit)) at TempPoint2 facing Default building facing degrees

Set TempUnit2 = (Last created unit)

Unit - Add grenade explosion to TempUnit2

Unit - Set level of grenade explosion for TempUnit2 to (Level of Grenade Launch (Heavy Machine Gunner) for (Triggering unit))

Unit - Add a 6.00 second Generic expiration timer to TempUnit2

Unit - Order TempUnit to Neutral Alchemist - Acid Bomb TempUnit2

Wait ((Distance between TempPoint and TempPoint2) / 700.00) seconds

Unit - Order TempUnit2 to Human Mountain King - Thunder Clap

Special Effect - Create a special effect at TempPoint2 using war3mapImported\ShrapnelShards.mdx

Special Effect - Destroy (Last created special effect)

Special Effect - Create a special effect at TempPoint2 using Abilities\Spells\Other\Incinerate\FireLordDeathExplode.mdl

Special Effect - Destroy (Last created special effect)

Custom script: call RemoveLocation(udg_TempPoint)

Custom script: call RemoveLocation(udg_TempPoint2)

Congratulations! You’ve just made a grenade type spell. But those of you who are more advanced may be saying to yourself “wait, that’s not MUI!” and you’re right it’s not. So the next part of this tutorial will show you how to make it MUI. You could make this MPI by simply making the variables arrays but I’m going to show you MUI, and I’m going to be using JASS.

##
 3. Making it MUI

Don’t be frightened. This is extremely basic JASS and does not require any third party editor stuff like newgen or vJASS. THIS IS NOT AN INTRODUCTION TO JASS. So if that’s what you’re looking for please don’t read on, although I strongly believe that even those with no knowledge of JASS may be able to do this,

Okay, first things first, lets go to edit and convert out trigger to custom text (be sure to save a GUI copy). We should get something like this:

 JASS:

function Trig_Grenade_Launch_Conditions takes nothing returns boolean
 if ( not ( GetSpellAbilityId() == 'A00J' ) ) then
 return false
 endif
 return true
endfunction

function Trig_Grenade_Launch_Actions takes nothing returns nothing
 set udg_TempPoint = GetUnitLoc(GetTriggerUnit())
 set udg_TempPoint2 = GetSpellTargetLoc()
 call CreateNUnitsAtLoc( 1, 'h005', GetOwningPlayer(GetTriggerUnit()), udg_TempPoint, bj_UNIT_FACING )
 set udg_TempUnit = GetLastCreatedUnit()
 call UnitAddAbilityBJ( 'A00K', udg_TempUnit )
 call UnitApplyTimedLifeBJ( 6.00, 'BTLF', udg_TempUnit )
 call CreateNUnitsAtLoc( 1, 'h006', GetOwningPlayer(GetTriggerUnit()), udg_TempPoint2, bj_UNIT_FACING )
 set udg_TempUnit2 = GetLastCreatedUnit()
 call UnitAddAbilityBJ( 'A00L', udg_TempUnit2 )
 call SetUnitAbilityLevelSwapped( 'A00L', udg_TempUnit2, GetUnitAbilityLevelSwapped('A00J', GetTriggerUnit()) )
 call UnitApplyTimedLifeBJ( 6.00, 'BTLF', udg_TempUnit2 )
 call IssueTargetOrderBJ( udg_TempUnit, "acidbomb", udg_TempUnit2 )
 call TriggerSleepAction( ( DistanceBetweenPoints(udg_TempPoint, udg_TempPoint2) / 700.00 ) )
 call IssueImmediateOrderBJ( udg_TempUnit2, "thunderclap" )
 call AddSpecialEffectLocBJ( udg_TempPoint2, "war3mapImported\\ShrapnelShards.mdx" )
 call DestroyEffectBJ( GetLastCreatedEffectBJ() )
 call AddSpecialEffectLocBJ( udg_TempPoint2, "Abilities\\Spells\\Other\\Incinerate\\FireLordDeathExplode.mdl" )
 call DestroyEffectBJ( GetLastCreatedEffectBJ() )
 call RemoveLocation(udg_TempPoint)
 call RemoveLocation(udg_TempPoint2)
endfunction

//===========================================================================
function InitTrig_Grenade_Launch takes nothing returns nothing
 set gg_trg_Grenade_Launch = CreateTrigger( )
 call TriggerRegisterAnyUnitEventBJ( gg_trg_Grenade_Launch, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition( gg_trg_Grenade_Launch, Condition( function Trig_Grenade_Launch_Conditions ) )
 call TriggerAddAction( gg_trg_Grenade_Launch, function Trig_Grenade_Launch_Actions )
endfunction

Now for those of you who are not skilled in JASS (not that I am), you may be saying to yourself “What the F***!” Don’t worry it’s not that bad. All we’re going to do is edit some of that text.

Before we start, I feel I should mention raw codes in this tutorial. A raw code is a series of letters and numbers (kind of like a serial number) that refers to a unit, ability, doodad, etc. Raw codes, when dealing with custom units and abilities, are generated in order. for instance, your first custom unit will be named 'h000'. So if you look at the raw code for my dummy unit (which is 'h005'), you can see that it is the sixth custom unit I've created in my map. I'll list all of the raw codes I"M using. YOURS WILL BE DIFFERENT, so don't fret when you see that. Just swap my raw codes with yours. You can find the raw codes for your units and abilities by going to the object editor and hitting ctrl+D. Everything will convert to its raw code.

‘h005’ = the locust dummy unit

‘h006’ = the dummy unit that is vulnerable

'A00J' = the caster's channel ability

'A00K' = the acid bomb ability

'A00L' = the thunderclap spell

You'll notice that these seem to be in order. The units are by number, and the abilities by alphabet. Clearly, i created channel first, then acid bomb, then thunderclap. For hard-coded (already existing) spells, the raw codes will look different and will not be ordered like this. If you create a GUI trigger first and then convert it, the raw codes should all be in place and there will be no need to change them.

Now we may continue.

The first step to making this MUI (and really the only one) is to add in the local variables. So take a look at the section of the trigger that says:

 JASS:

function Trig_Grenade_Launch_Actions takes nothing returns nothing

we’re going to be working with that section. Okay, now to add the local variables. Just under that heading ^^ we need to put:

 JASS:

 local unit caster = GetTriggerUnit()
 local unit TempUnit
 local unit TempUnit2
 local location TempPoint = GetUnitLoc(caster)
 local location TempPoint2 = GetSpellTargetLoc()

Now I’ll explain what all of that is. Local units are variables that are specific to each function. So everytime the trigger fires, no matter how many times at once, the local variables always refer to different units. So all we’ve done is set up our local variables. Now, you may have noticed that TempUnit and TempUnit2 aren’t set to anything. That is because we will set them later; however, all local variables must be declared in the beginning of a function so we have to state them, even though we are setting them later.

For setting the TempUnits, you would have to set them immediately after they are created like so:

 JASS:

 call CreateNUnitsAtLoc( 1, 'h005', GetOwningPlayer(GetTriggerUnit()), udg_TempPoint, bj_UNIT_FACING )
 set TempUnit = GetLastCreatedUnit()

So now all you would have to do to make this MUI is replace every instance of GetTriggerUnit(), udg_TempPoint, udg_TempPoint2, and GetLastCreatedUnit(), with their respected variables.

So after replacing everything, your trigger should look like this:

 JASS:

function Trig_Grenade_Launch_Conditions takes nothing returns boolean
 if ( not ( GetSpellAbilityId() == 'A00J' ) ) then
 return false
 endif
 return true
endfunction

function Trig_Grenade_Launch_Actions takes nothing returns nothing
 local unit caster = GetTriggerUnit()
 local unit TempUnit
 local unit TempUnit2
 local location TempPoint = GetUnitLoc(caster)
 local location TempPoint2 = GetSpellTargetLoc()
 call CreateNUnitsAtLoc( 1, 'h005', GetOwningPlayer(caster), TempPoint, bj_UNIT_FACING )
 set TempUnit = GetLastCreatedUnit()
 call UnitAddAbilityBJ( 'A00K', TempUnit )
 call UnitApplyTimedLifeBJ( 6.00, 'BTLF', TempUnit )
 call CreateNUnitsAtLoc( 1, 'h006', GetOwningPlayer(caster), TempPoint2, bj_UNIT_FACING )
 set TempUnit2 = GetLastCreatedUnit()
 call UnitAddAbilityBJ( 'A00L', TempUnit2 )
 call SetUnitAbilityLevelSwapped( 'A00L', TempUnit2, GetUnitAbilityLevelSwapped('A00J', caster) )
 call UnitApplyTimedLifeBJ( 6.00, 'BTLF', TempUnit2 )
 call IssueTargetOrderBJ( TempUnit, "acidbomb", TempUnit2 )
 call TriggerSleepAction( ( DistanceBetweenPoints(TempPoint, TempPoint2) / 700.00 ) )
 call IssueImmediateOrderBJ( TempUnit2, "thunderclap" )
 call AddSpecialEffectLocBJ( TempPoint2, "war3mapImported\\ShrapnelShards.mdx" )
 call DestroyEffectBJ( GetLastCreatedEffectBJ() )
 call AddSpecialEffectLocBJ( TempPoint2, "Abilities\\Spells\\Other\\Incinerate\\FireLordDeathExplode.mdl" )
 call DestroyEffectBJ( GetLastCreatedEffectBJ() )
 call RemoveLocation(TempPoint)
 call RemoveLocation(TempPoint2)
 set caster = null
 set TempUnit = null
 set TempUnit2 = null
endfunction

//===========================================================================
function InitTrig_Grenade_Launch takes nothing returns nothing
 set gg_trg_Grenade_Launch = CreateTrigger( )
 call TriggerRegisterAnyUnitEventBJ( gg_trg_Grenade_Launch, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition( gg_trg_Grenade_Launch, Condition( function Trig_Grenade_Launch_Conditions ) )
 call TriggerAddAction( gg_trg_Grenade_Launch, function Trig_Grenade_Launch_Actions )
endfunction

And there you go. You now have a leak free, MUI grenade type spell in JASS!

Some of you may have noticed that I’ve added those three null lines at the end of the trigger. This is done to prevent them from leaking, it’s not mandatory, but it’s definitely good for your map and something you should do.

Okay so you’re done. Or are you? We’re going to take this a step further and make the code much more efficient. This section is for those slightly more experienced JASSers so if you feel uneasy, do not read on. Once again, I am not giving a full JASS tutorial, it is assumed that some of these things are already known to the reader.

##
 4. Efficiency

All I’m going to do is swap some native functions for the BJ functions, and reduce that miserable conditions line.

Lets take a look at that condition function:

 JASS:

function Trig_Grenade_Launch_Conditions takes nothing returns boolean
 if ( not ( GetSpellAbilityId() == 'A00J' ) ) then
 return false
 endif
 return true
endfunction

Ew, that looks nasty and bulky. Lets change that around to look a bit cleaner

 JASS:

function Trig_Grenade_Launch_Conditions takes nothing returns boolean
 return GetSpellAbilityId() == 'A00J'
endfunction

ah, that looks much better and it does the same exact thing!

Now I’m going to show you how to make setting the TempUnits more effective and how to more effectively destroy special effects. They have the same concepts, we’re going to be combining two lines into one.

So let’s take a look at these two lines:

 JASS:

 call CreateNUnitsAtLoc( 1, 'h005', GetOwningPlayer(caster), TempPoint, bj_UNIT_FACING )
 set TempUnit = GetLastCreatedUnit()

We’re going to combine these two to set TempUnit while the unit is being created, instead of creating the unit and then setting TempUnit. The only purpose for this, as is everything in this section of the tutorial, is to make the code smaller and more efficient. So let’ synthesize those two lines into:

 JASS:

 set TempUnit = CreateNUnitsAtLoc( 1, 'h005', GetOwningPlayer(caster), TempPoint, bj_UNIT_FACING )

The same applies to destroying special effects:

 JASS:

 call DestroyEffectBJ( AddSpecialEffectLocBJ( TempPoint2, "war3mapImported\\ShrapnelShards.mdx" ) )

There, we’ve just saved a couple of lines of code.

Now to swap for BJs with natives. You may have noticed that some functions have a BJ after, such as:

 JASS:

 call UnitAddAbilityBJ( 'A00K', TempUnit )

So what? Well, a BJ function calls a native function into play, so therefore, we can make things run faster and smoother by calling the native instead of the BJ right? It’s like asking your mom to get you a drink from the fridge, you get the drink much faster if you get it yourself. You don’t have to wait for your mom to get it and bring it back. So what we’re going to do is fetch the function ourselves instead of asking mom to do it.

So the native function that UnitAddAbilityBJ calls is:

 JASS:

 call UnitAddAbility( TempUnit, 'A00K' )

wait a minute? That looks the same as the other except with no BJ. Well, it is, and the statements were swapped as well. I’m not going to list all of the natives for you, you can do that yourself. You may also have to rearrange the statements in order to get the natives to work. Just remember that native functions NEVER have a BJ in them. So atfter you do all of that your code should look like this:

 JASS:

function Trig_Grenade_Launch_Conditions takes nothing returns boolean
 return GetSpellAbilityId() == 'A00J'
endfunction

function Trig_Grenade_Launch_Actions takes nothing returns nothing
 local unit caster = GetTriggerUnit()
 local unit TempUnit
 local unit TempUnit2
 local location TempPoint = GetUnitLoc(caster)
 local location TempPoint2 = GetSpellTargetLoc()
 set TempUnit = CreateUnitAtLoc( GetOwningPlayer(caster), 'h005', TempPoint, 270.00 )
 call UnitAddAbility( TempUnit, 'A00K' )
 call UnitApplyTimedLife( TempUnit, 'BTLF', 6.00 )
 set TempUnit2 = CreateUnitAtLoc( GetOwningPlayer(caster), 'h006', TempPoint2, 270.00 )
 call UnitAddAbility( TempUnit2, 'A00L' )
 call SetUnitAbilityLevel( TempUnit2, 'A00L', GetUnitAbilityLevel(caster, 'A00J') )
 call UnitApplyTimedLife( TempUnit2, 'BTLF', 6.00 )
 call IssueTargetOrder( TempUnit, "acidbomb", TempUnit2 )
 call TriggerSleepAction( ( DistanceBetweenPoints(TempPoint, TempPoint2) / 650.00 ) )
 call IssueImmediateOrder( TempUnit2, "thunderclap" )
 call DestroyEffect( AddSpecialEffectLoc( "war3mapImported\\ShrapnelShards.mdx", TempPoint2 ) )
 call DestroyEffect( AddSpecialEffectLoc( "Abilities\\Spells\\Other\\Incinerate\\FireLordDeathExplode.mdl", TempPoint2 ) )
 call Rem oveLocation(TempPoint)
 call RemoveLocation(TempPoint2)
 set caster = null
 set TempUnit = null
 set TempUnit2 = null
endfunction

//===========================================================================
function InitTrig_Grenade_Launch takes nothing returns nothing
 set gg_trg_Grenade_Launch = CreateTrigger( )
 call TriggerRegisterAnyUnitEventBJ( gg_trg_Grenade_Launch, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition( gg_trg_Grenade_Launch, Condition( function Trig_Grenade_Launch_Conditions ) )
 call TriggerAddAction( gg_trg_Grenade_Launch, function Trig_Grenade_Launch_Actions )
endfunction

Congratulations you’re finished! That wasn’t so hard was it?

The specific grenade ability we made in this tutorial is an ability for a hero of mine in my map Space Orcs!, an AoS style map which will be out for Beta testing very soon so keep a look out. This code can be imported straight into any map, just make sure you have the custom abilities, and dummies set in. The two things that you have to look for if you copy and paste are:

1) Change the raw codes of dummy units and abilities

2) Make sure you replace my imported custom effect with something else otherwise you WILL get an error!

Hope someone found this useful!

I've attached a test map for those of you who want to try out the ability. The raw codes and special effects in this map are different because i added it after I did the tutorial, but the fundamentals are exactly the same.

Credit for the Tauren Marine model in the screenshots goes to General_Frank

Credit for the Shrapnel Explosion model in the screenshots goes to Will_the_Almighty

#### Attachments

Grenade_Launch_Test_Map.w3x

 23.2 KB

 · Views: 1,401




 Last edited by a moderator: Jul 19, 2008

#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


This tutorial is great but:

- Please attach all images to the thread instead of linking to photobucket.

- Please use the new [h1][h2][h3] tags, read the The Hive Workshop - A Warcraft III Modding Site - Announcements in Forum : Tutorial Submission.

- Please attach a test map, the JASS contains 'hard-coded' ability-names which you would probably like to define the names of in the start instead.

#### PrisonLove

#### PrisonLove

 Level 10

JoinedSep 29, 2006Messages447

Resources: 5

#### Resources
Maps 4Tutorials 1

Okay, I did the headers, attachments, and I put the raw code stuff towards the beginning of the JASS section because there is no need for them before that, as you wouldn't need the raw codes if you just did it in GUI. I'm working on the Demo Map Right now.

EDIT: okay I've uploaded a Test Map for this tutorial. Hope I've got everything now.


 Last edited: Jul 18, 2008

#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


The tutorial seems very good now. I like how you optimized the skill.

Also, I have set it to [Tutorial]. It is a How-To but the MUI part makes it a tutorial.

Approved

#### PrisonLove

#### PrisonLove

 Level 10

JoinedSep 29, 2006Messages447

Resources: 5

#### Resources
Maps 4Tutorials 1

Thanks Ralle. Yay my first tutorial lol. Hope people find this one useful. It works wonders for modern warfare type maps, or even exploding potions.

#### kewlthis

#### kewlthis

 Level 13

JoinedJun 5, 2008Messages504

Resources: 2

#### Resources
Maps 2

Its a nice tutorial, but you can make it easier way i think. Just try pocket factory ability (thats how i do it, figure out the rest by yourself).

#### PrisonLove

#### PrisonLove

 Level 10

JoinedSep 29, 2006Messages447

Resources: 5

#### Resources
Maps 4Tutorials 1

you can use pocket factory but then it does not give you the targeting image which is what i wanted for this particular spell, since it is an AoE spell. but if you want to use pocket factory to make it easier you can

#### kewlthis

#### kewlthis

 Level 13

JoinedJun 5, 2008Messages504

Resources: 2

#### Resources
Maps 2

PrisonLove said:

 you can use pocket factory but then it does not give you the targeting image which is what i wanted for this particular spell, since it is an AoE spell. but if you want to use pocket factory to make it easier you can


Click to expand...

Have you ever heard of channel spell or aoe spells? Well its true that pocket factory cant do everything but its 100% perfect for my grenade spells atleast and I find it much easier to do and can be easily made mui (i only do gui).

#### PrisonLove

#### PrisonLove

 Level 10

JoinedSep 29, 2006Messages447

Resources: 5

#### Resources
Maps 4Tutorials 1

kewlthis said:

 Have you ever heard of channel spell or aoe spells?...


Click to expand...

am i not using channel?

and if you want to use pocket factory that is just fine, but i wanted a targetting image for the spell so that requires a little more work. if you want to use pocket factory go right ahead. its not much more work to do it my way. and making it MUI would not be easier because you would still have the wait.

#### kewlthis

#### kewlthis

 Level 13

JoinedJun 5, 2008Messages504

Resources: 2

#### Resources
Maps 2

PrisonLove said:

 am i not using channel?

and if you want to use pocket factory that is just fine, but i wanted a targetting image for the spell so that requires a little more work. if you want to use pocket factory go right ahead. its not much more work to do it my way. and making it MUI would not be easier because you would still have the wait.


Click to expand...

You wouldnt need wait. I would post the trigger here if I wasnt so lazy, but i would do a grenade spell generally like this:

Trigger1:

unit start ability (channel)

create dummy unit

cast pocket factory

Trigger2:

unit dies

create effects (wot you want) at point of triggering unit

set unit group = units in range matching....

pick every unit in unit group

-damage picked unit

I quess you didnt notice what i was talking about. And then set pocket factory units decaying time to 0.01 or something. And now you have nice and simple grenade spell, without waits.

#### PrisonLove

#### PrisonLove

 Level 10

JoinedSep 29, 2006Messages447

Resources: 5

#### Resources
Maps 4Tutorials 1

hmmm, very nice i didnt think of that. but theres still the issue of the targeting image which is what I wanted. if you dont need it then hey, by all means use pocket factory

#### kewlthis

#### kewlthis

 Level 13

JoinedJun 5, 2008Messages504

Resources: 2

#### Resources
Maps 2

PrisonLove said:

 hmmm, very nice i didnt think of that. but theres still the issue of the targeting image which is what I wanted. if you dont need it then hey, by all means use pocket factory


Click to expand...


I dont get what targeting image you talking about...I can make exactly samekind of mui/gui spell with my system...

#### PrisonLove

#### PrisonLove

 Level 10

JoinedSep 29, 2006Messages447

Resources: 5

#### Resources
Maps 4Tutorials 1

targetting image being the AoE image that comes up when you click the spell, like with blizzard and such. you dont get that with pocket factory.

#### kewlthis

#### kewlthis

 Level 13

JoinedJun 5, 2008Messages504

Resources: 2

#### Resources
Maps 2

PrisonLove said:

 targetting image being the AoE image that comes up when you click the spell, like with blizzard and such. you dont get that with pocket factory.


Click to expand...


thats why you use channel as heros ability and create dummy that casts the pocket factory

#### PrisonLove

#### PrisonLove

 Level 10

JoinedSep 29, 2006Messages447

Resources: 5

#### Resources
Maps 4Tutorials 1

i see what youre saying now, yes you can do it that way.

#### Demonicdraenei

D

#### Demonicdraenei

kewlthis said:

 Trigger2:


unit dies


create effects (wot you want) at point of triggering unit


set unit group = units in range matching....


pick every unit in unit group


-damage picked unit


I quess you didnt notice what i was talking about. And then set pocket factory units decaying time to 0.01 or something. And now you have nice and simple grenade spell, without waits.


Click to expand...


Alternatively, you could set the pocket factory to create units that have aoe damage upon death. Set the explosion to as many as you want and how you want it to look, and you have a good grenade spell.


Triggering the damage won't allow the game to detect who is the killer. (I think theres a damage detection engine, I don't know.) Nonetheless, its still better to use spells to damage the targeted units.

#### Kayorge

#### Kayorge

 Level 2

JoinedNov 23, 2008Messages11

hey guys do you know how to optimize models and stuff to make it load faster

#### Pivius

#### Pivius

 Level 2

JoinedMay 26, 2014Messages10

Could you please explain the Channel Ability? What must be done so as to have a visible icon (cause, when I learn it, I can't see dat icon).

#### Bobastian

B

#### Bobastian

Pivius said:

 Could you please explain the Channel Ability? What must be done so as to have a visible icon (cause, when I learn it, I can't see dat icon).


Click to expand...

Go to the ability in the object editor and modify this field: Level 1 - Data - Options. It will show you a list of check boxes. Select visible and the ability will show.

For more information on the channel ability, refer to: Spells - Channel

 You must log in or register to reply here.


### Similar threads

Locked

 grenade launch issue


PrisonLove

Jun 23, 2008

World Editor Help Zone

Replies2Views1K

Jun 23, 2008
PrisonLove

Locked

[JASS]
 strange error


PrisonLove

Jul 6, 2008

Triggers & Scripts

Replies1Views1K

Jul 6, 2008
spiwn

Locked

[Solved]
 Does this trigger leak


sarumanthecursed

Jul 20, 2016

Triggers & Scripts

Replies5Views1K

Jul 21, 2016
sarumanthecursed

Locked

[Solved]
 Knockback isn't working for group


Jay the Editor

Mar 6, 2012

Triggers & Scripts

Replies10Views2K

Mar 7, 2012
Jay the Editor

Locked

[Trigger]
 Why does this lag?


Jaijaibinx

Jun 29, 2009

Triggers & Scripts

Replies8Views2K

Jul 1, 2009
darkreapers

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