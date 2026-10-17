---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-make-a-campaign-ai.211549/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Make a Campaign AI
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-make-a-campaign-ai.211549/>
#### moyackx

#### moyackx

 Level 15

JoinedFeb 15, 2006Messages851

Resources: 9

#### Resources
Maps 4Spells and Systems 2Tutorials 3


 Last edited: Feb 5, 2019

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

Seriously, if only melee AI creation, its still best and easy to configure it in GUI mode, and

just do the rest of the attacking/engaging in triggers, but good tutorials anyway...

#### moyackx

#### moyackx

 Level 15

JoinedFeb 15, 2006Messages851

Resources: 9

#### Resources
Maps 4Spells and Systems 2Tutorials 3

mckill2009 said:

 Seriously, if only melee AI creation, its still best and easy to configure it in GUI mode, and


just do the rest of the attacking/engaging in triggers, but good tutorials anyway...


Click to expand...

Well, GUI AI editor has its limitations.

It only allows you to define a random selection of 3 heroes instead of 4.

If you define conditionals too long, it fails (it truncates the code generating errors)

And... it's slow to develop.


So... the best is to write you own AI Jasscraft and it's done


And with this you can optimize the AI process and add more awesome control over it.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great tutorial. Nice explanations and great for those who want to learn how to create an AI using the common.ai scripts. =)

This is in an approvable state, but two things should be fixed first:

1) The links to JassCraft and Jass Shop Pro link to random hive maps. (lol) Replace those links with the ones from wc3c.net

2) One of the JASS tags has everything on one line. Under the category "Attacking Strategy"

Once those changes have been made this will be approved!

#### moyackx

#### moyackx

 Level 15

JoinedFeb 15, 2006Messages851

Resources: 9

#### Resources
Maps 4Spells and Systems 2Tutorials 3

PurgeandFire111 said:

 Great tutorial. Nice explanations and great for those who want to learn how to create an AI using the common.ai scripts. =)


This is in an approvable state, but two things should be fixed first:


1) The links to JassCraft and Jass Shop Pro link to random hive maps. (lol) Replace those links with the ones from wc3c.net


2) One of the JASS tags has everything on one line. Under the category "Attacking Strategy"


Once those changes have been made this will be approved!


Click to expand...

About the second point, it seems it's a bug with the jass script of the site, I tried to edit it, but nothing changes

#### watermelon_1234

#### watermelon_1234

 Level 14

JoinedNov 18, 2007Messages1,084

Resources: 10

#### Resources
Spells and Systems 9JASS 1

 JASS:

function main takes nothing returns nothing
 local unit crp

// Building code goes here

//*** WAVE 1 ***
 call InitAssaultGroup()
 set crp = GetCreepCamp(0,9,false) // Stores the target unit
 call CampaignAttackerEx( 2, 3, 3, ARCHER )
 call CampaignAttackerEx( 0, 1, 2, HUNTRESS )
 call Sleep( M3 ) // Waits 3 minutes before attacking
 call AttackMoveKillA(crp) // Order assault group to attack that unit
endfunction

How about this? You could also try copying the code into something like WordPad before pasting it into your post.

#### moyackx

#### moyackx

 Level 15

JoinedFeb 15, 2006Messages851

Resources: 9

#### Resources
Maps 4Spells and Systems 2Tutorials 3

watermelon_1234 said:

 JASS:

function main takes nothing returns nothing
 local unit crp

// Building code goes here

//*** WAVE 1 ***
 call InitAssaultGroup()
 set crp = GetCreepCamp(0,9,false) // Stores the target unit
 call CampaignAttackerEx( 2, 3, 3, ARCHER )
 call CampaignAttackerEx( 0, 1, 2, HUNTRESS )
 call Sleep( M3 ) // Waits 3 minutes before attacking
 call AttackMoveKillA(crp) // Order assault group to attack that unit
endfunction

How about this? You could also try copying the code into something like WordPad before pasting it into your post.


Click to expand...


I already did this, and it didn't worked.... weird....

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Hmm.. I've tried it as well and it didn't work. Anyway, I've detached the signature and approved.

I'll try to work out a resolve for that bug or I will report it if I find time.

#### gjpaixao13

#### gjpaixao13

 Level 2

JoinedApr 13, 2011Messages13

Higher Hero Levels

Hello moyackx!!


Well, I have a question...I've benn messing with the W3E for some years(Since 2005), but I started to study vJass these days(I'm already a programmer for 4 years...but I was too lazy to leave GUI and learn vJass  )


Anyway, in the AI Editor, if I put more than 1 level to the ultimate spell, it breaks.As it breaks if I put more than 3 levels on others spells.


I'll try this tutorial after work when I get home, but if you see this question before maybe you can answer me


The question is...If I make an AI this way(triggers and vJass) will it break if I put more levels on the ultimate or other spells???

#### moyackx

#### moyackx

 Level 15

JoinedFeb 15, 2006Messages851

Resources: 9

#### Resources
Maps 4Spells and Systems 2Tutorials 3

Technically no, because with triggers you have more control over you do with units. AI coding is filled with more "hardcoded behavior" than normal triggering.

I should try an example though to see if it works.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

IDK if it's just me but the loop in this code doent work at all...


The AI builds and trains and first attack is OK, but it doent loop...

 JASS:

globals
 player AttackTo = Player(12) //attacking
endglobals

function ConfigureAI takes nothing returns nothing
 call SetSlowChopping( true )
 call SetPeonsRepair( true )
 call SetHeroesTakeItems (true)
 call SetHeroesBuyItems(true)
endfunction

function hero_levels takes nothing returns integer
 local integer hero = GetHeroId()
 local integer level = GetHeroLevelAI()
 local integer a = 0
 if hero == BLADE_MASTER then
 if level == 1 or level == 3 or level == 5 then
 set a = MIRROR_IMAGE
 endif
 if level == 2 or level == 4 or level == 7 then
 set a = CRITICAL_STRIKE
 endif
 if level >= 8 then
 set a = WIND_WALK
 endif
 if level == 6 then
 set a = BLADE_STORM
 endif
 endif
 return a
endfunction

function main takes nothing returns nothing
 call CampaignAI( BURROW, function hero_levels )
 call ConfigureAI( )
 //============================
 call SetBuildUnit( 3, PEON )
 call SetBuildUnit( 1, GREAT_HALL )
 call SetBuildUnit( 2, BURROW )
 call SetBuildUnit( 1, ORC_ALTAR )
 call SetBuildUnit( 4, PEON )
 call SetBuildUnit( 3, BURROW )
 call SetBuildUnit( 1, ORC_BARRACKS )
 call SetBuildUnit( 5, PEON )
 call SetBuildUnit( 1, BLADE_MASTER )
 call SetBuildUnit( 1, GRUNT )
 call SetBuildUnit( 4, BURROW )
 call SetBuildUnit( 6, PEON )
 call SetBuildUnit( 2, ORC_BARRACKS )
 call SetBuildUnit( 2, GRUNT )
 call SetBuildUnit( 3, GRUNT )
 call SetBuildUnit( 4, GRUNT )
 call SetBuildUnit( 5, GRUNT )
 call SetBuildUnit( 6, GRUNT )
 call SetBuildUnit( 7, GRUNT )
 loop
 call InitAssaultGroup()
 call CampaignAttackerEx( 1, 1, 1, BLADE_MASTER )
 call CampaignAttackerEx( 3, 3, 3, GRUNT )
 call SuicideOnPlayerEx( 10, 10, 10, AttackTo )
 //============================
 call InitAssaultGroup()
 call CampaignAttackerEx( 1, 1, 1, BLADE_MASTER )
 call CampaignAttackerEx( 4, 4, 4, GRUNT )
 call SuicideOnPlayerEx( 10, 10, 10, AttackTo )
 //============================
 call InitAssaultGroup()
 call CampaignAttackerEx( 1, 1, 1, BLADE_MASTER )
 call CampaignAttackerEx( 5, 5, 5, GRUNT )
 call SuicideOnPlayerEx( 10, 10, 10, AttackTo )
 //============================
 call InitAssaultGroup()
 call CampaignAttackerEx( 1, 1, 1, BLADE_MASTER )
 call CampaignAttackerEx( 6, 6, 6, GRUNT )
 call SuicideOnPlayerEx( 10, 10, 10, AttackTo )
 endloop
endfunction

#### moyackx

#### moyackx

 Level 15

JoinedFeb 15, 2006Messages851

Resources: 9

#### Resources
Maps 4Spells and Systems 2Tutorials 3

mckill2009 said:

 IDK if it's just me but the loop in this code doent work at all...


The AI builds and trains and first attack is OK, but it doent loop...

 JASS:

globals
 player AttackTo = Player(12) //attacking
endglobals

function ConfigureAI takes nothing returns nothing
 call SetSlowChopping( true )
 call SetPeonsRepair( true )
 call SetHeroesTakeItems (true)
 call SetHeroesBuyItems(true)
endfunction

function hero_levels takes nothing returns integer
 local integer hero = GetHeroId()
 local integer level = GetHeroLevelAI()
 local integer a = 0
 if hero == BLADE_MASTER then
 if level == 1 or level == 3 or level == 5 then
 set a = MIRROR_IMAGE
 endif
 if level == 2 or level == 4 or level == 7 then
 set a = CRITICAL_STRIKE
 endif
 if level >= 8 then
 set a = WIND_WALK
 endif
 if level == 6 then
 set a = BLADE_STORM
 endif
 endif
 return a
endfunction

function main takes nothing returns nothing
 call CampaignAI( BURROW, function hero_levels )
 call ConfigureAI( )
 //============================
 call SetBuildUnit( 3, PEON )
 call SetBuildUnit( 1, GREAT_HALL )
 call SetBuildUnit( 2, BURROW )
 call SetBuildUnit( 1, ORC_ALTAR )
 call SetBuildUnit( 4, PEON )
 call SetBuildUnit( 3, BURROW )
 call SetBuildUnit( 1, ORC_BARRACKS )
 call SetBuildUnit( 5, PEON )
 call SetBuildUnit( 1, BLADE_MASTER )
 call SetBuildUnit( 1, GRUNT )
 call SetBuildUnit( 4, BURROW )
 call SetBuildUnit( 6, PEON )
 call SetBuildUnit( 2, ORC_BARRACKS )
 call SetBuildUnit( 2, GRUNT )
 call SetBuildUnit( 3, GRUNT )
 call SetBuildUnit( 4, GRUNT )
 call SetBuildUnit( 5, GRUNT )
 call SetBuildUnit( 6, GRUNT )
 call SetBuildUnit( 7, GRUNT )
 loop
 call InitAssaultGroup()
 call CampaignAttackerEx( 1, 1, 1, BLADE_MASTER )
 call CampaignAttackerEx( 3, 3, 3, GRUNT )
 call SuicideOnPlayerEx( 10, 10, 10, AttackTo )
 //============================
 call InitAssaultGroup()
 call CampaignAttackerEx( 1, 1, 1, BLADE_MASTER )
 call CampaignAttackerEx( 4, 4, 4, GRUNT )
 call SuicideOnPlayerEx( 10, 10, 10, AttackTo )
 //============================
 call InitAssaultGroup()
 call CampaignAttackerEx( 1, 1, 1, BLADE_MASTER )
 call CampaignAttackerEx( 5, 5, 5, GRUNT )
 call SuicideOnPlayerEx( 10, 10, 10, AttackTo )
 //============================
 call InitAssaultGroup()
 call CampaignAttackerEx( 1, 1, 1, BLADE_MASTER )
 call CampaignAttackerEx( 6, 6, 6, GRUNT )
 call SuicideOnPlayerEx( 10, 10, 10, AttackTo )
 endloop
endfunction

Click to expand...


According to your code you give 10 seconds to the units to do the attack. SuicideOnPlayerEx does a wait in the for that time while the AI does the job.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

moyackx said:

 According to your code you give 10 seconds to the units to do the attack. SuicideOnPlayerEx does a wait in the for that time while the AI does the job.


Click to expand...

Sorry I really didnt get your answer, you mean to say that I should increase the wait?, I already did that for 60,120 etc... and it doesnt work...


SuicideOnPlayerEx calls many sub functions so I went to the root of it (CommonSuicideOnPlayer), unfortunately IDK how those function works after that...


And again, ONLY the loop doesnt work, the first attack is OK...

#### moyackx

#### moyackx

 Level 15

JoinedFeb 15, 2006Messages851

Resources: 9

#### Resources
Maps 4Spells and Systems 2Tutorials 3

mckill2009 said:

 Sorry I really didnt get your answer, you mean to say that I should increase the wait?, I already did that for 60,120 etc... and it doesnt work...


SuicideOnPlayerEx calls many sub functions so I went to the root of it (CommonSuicideOnPlayer), unfortunately IDK how those function works after that...


And again, ONLY the loop doesnt work, the first attack is OK...


Click to expand...

I mean this :


Change this: call SuicideOnPlayerEx( 10, 10, 10, AttackTo ) // right now the AI wait 10 seconds, you need minutes in order to let the AI do the job


For this: call SuicideOnPlayerEx( M10, M10, M10, AttackTo )


And tell me if it works. As far as I remember, SuicideOnPlayerEx does a sleep internally.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

Like I said:

 I already did that for 60,120 etc... and it doesnt work...


Click to expand...

coz M1=60, M2=120

I think it doesnt really matter if it's in seconds or minutes coz if I put 1 second, the AI will almost attack immediately on first wave, next waves no more...

EDIT:

Tested it again for 5 seconds, works for first, second waits for at least 3 minutes, the rest loop waits for 5 seconds only...

The second wait is now bugged with minutes of interval, IDK why...


 Last edited: Aug 30, 2012

#### Starquizer

#### Starquizer

 Level 16

JoinedMar 3, 2006Messages1,564

Resources: 7

#### Resources
Maps 1Spells and Systems 3Tutorials 3

Set replacements is not working, why ?

Take a look at this thread I make.

#### Wa666r

#### Wa666r

 Level 33

JoinedFeb 19, 2011Messages875

Resources: 32

#### Resources
Packs 1Maps 28User Interface 3

Help me out?

So I've only started with vJass programming. I know how to deal with the AI editor but its very choppy and usually doesn't work for too long.


But anyways, I made the code for humans but for some reason they dont build or do anything when I test them. Could you find the problem and help me out?


(Sorry I'm don't usually post much on this website, so unsure how to upload the files)


------------------------------------------------------------------------


globals


 player MyVictim = Player( 0 )


endglobals


function ConfigureAI takes nothing returns nothing


 call SetSlowChopping( true )


 call SetPeonsRepair( true )


endfunction


function main takes nothing returns nothing


 call CampaignAI( 'hhou', null )


 call ConfigureAI( )


 // **********************************


 // * Building Strategy *


 // **********************************


 call SetReplacements( 1, 2, 3 )


 call SetBuildUnitEx( 1, 1, 1, 'htow' )


 call SetBuildUnit( 7, 'hpea' )


 call SetBuildUnitEx( 2, 6, 6, 'hhou' )


 call SetBuildUnitEx( 1, 1, 2, 'hbar' )


 call SetBuildUnit( 1, 'halt' )


 call SetBuildUnit( 1, 'hbla' )


 call SetBuildUnit( 1, 'hlum' )


 call SetBuildUnitEx( 0, 3, 3, 'nntg' )


 call SetBuildUnit( 3, 'htwt' )


 call SetBuildUnit( 1, 'hvlt' )


 call CampaignDefenderEx( 2, 6, 6, 'hfoo' )


 call CampaignDefenderEx( 1, 'hrif' )


 // Teir 2 buildings


 call SetBuildUnitEx( 1, 1, 2, 'hkee' )


 call SetBuildUnit( 1, 'harm' )


 call SetBuildUnitEx( 1, 1, 2, 'hars' )


 call SetBuildUnit( 3, 'hhou' )


 call SetBuildUnitEx( 0, 3, 3, 'hgtw' )


 // Tier 3 buildings


 call SetBuildUnit( 1, 'hcas' )


 call SetBuildUnitEx( 0, 1, 1, 'hgra' )


// **********************************


 // * End Building Strategy *


 // **********************************


 // **********************************


 // * Attack Strategy *


 // **********************************


 //*** WAVE 1 ***


 call InitAssaultGroup()


 call CampaignAttackerEx( 2, 5, 5, 'hfoo' )


 call SuicideOnPlayerEx( M3, M2, M2, MyVictim )


 //*** WAVE 2 *** Between 2 or 3 minutes after Wave 1


 call InitAssaultGroup()


 call CampaignAttackerEx( 3, 4, 4, 'hrif' )


 call CampaignAttackerEx( 1, 2, 3, 'hmpr' )


 call SuicideOnPlayerEx( M3, M2, M2, MyVictim )


 //*** WAVE 3 *** Between 2 or 3 minutes after Wave 2


 call InitAssaultGroup()


 call CampaignAttackerEx( 3, 5, 5, 'hfoo' )


 call CampaignAttackerEx( 1, 3, 3, 'hsor' )


 call SuicideOnPlayerEx( M3, M2, M2, MyVictim )


 //*** WAVE 4 *** Between 2 or 3 minutes after Wave 3


 call InitAssaultGroup()


 call CampaignAttackerEx( 3, 4, 4, 'hrif' )


 call CampaignAttackerEx( 1, 2, 2, 'hkni' )


 call SuicideOnPlayerEx( M3, M2, M2, MyVictim )


 //** Animal War Upgrade


 call SetBuildUpgrEx( 1, 1, 1, 'Rhan' )


 call SetBuildUpgrEx( 1, 1, 1, 'Rhde' )


 //*** Basic upgrades from Black Smith ***


 call SetBuildUpgrEx( 1, 2, 3, 'Rhme' )


 call SetBuildUpgrEx( 1, 2, 3, 'Rhar' )


 //*** WAVE 5 *** Between 3 or 4 minutes after Wave 4


 call InitAssaultGroup()


 call CampaignAttackerEx( 3, 3, 3, 'hkni' )


 call CampaignAttackerEx( 2, 2, 2, 'hmpr' )


 call SuicideOnPlayerEx( M4, M3, M3, MyVictim )


 //*** WAVE 6 *** Between 3 or 4 minutes after Wave 5


 call InitAssaultGroup()


 call CampaignAttackerEx( 2, 3, 3, 'hfoo' )


 call CampaignAttackerEx( 1, 1, 1, 'hrif' )


 call CampaignAttackerEx( 1, 3, 3, 'hsor' )


 call SuicideOnPlayerEx( M4, M3, M3, MyVictim )


 //*** WAVE 7 *** Between 3 or 5 minutes after Wave 6


 call InitAssaultGroup()


 call CampaignAttackerEx( 2, 10, 10, 'hfoo' )


 call CampaignAttackerEx( 1, 1, 1, 'Hvsh' )


 call SuicideOnPlayerEx( M5, M4, M3, MyVictim )


 //*** WAVE 8 *** Between 3 or 5 minutes after Wave 7


 call InitAssaultGroup()


 call CampaignAttackerEx( 2, 2, 2, 'hkni' )


 call CampaignAttackerEx( 1, 3, 1, 'hmpr' )


 call CampaignAttackerEx( 1, 2, 1, 'hsor' )


 //*** WAVE 9 *** Between 4 or 6 minutes after Wave 8


 call InitAssaultGroup()


 call CampaignAttackerEx( 2, 2, 2, 'hfoo' )


 call CampaignAttackerEx( 1, 3, 3, 'hkni' )


 call CampaignAttackerEx( 1, 3, 3, 'hrif')


 call CampaignAttackerEx( 0, 2, 2, 'hmpr' )


 call CampaignAttackerEx( 0, 2, 2, 'hmtm' )


 call SuicideOnPlayerEx( M6, M5, M4, MyVictim )


endloop


 // **********************************


 // * End Attack Strategy *


 // **********************************


endfunction


---------------------------------------------------------------------------

#### Starquizer

#### Starquizer

 Level 16

JoinedMar 3, 2006Messages1,564

Resources: 7

#### Resources
Maps 1Spells and Systems 3Tutorials 3

you have a syntax error at line 5 before the end of the script "endloop": you didn't start the loop in the first place.

 JASS:

loop
 // some actions ...
endloop

you are making

 JASS:

// some actions
endloop

I would suggest you do this for every AI you make to know it has no errors, add this line under the main function

 JASS:

function main takes nothing returns nothing
 call DisplayTextToPlayer(Player(0),0,0,"AI started")
 // Start you AI

endfunction

When you run the game and you get the message "AI started" then your file will work properly, otherwise, it contains an error(s).

#### moyackx

#### moyackx

 Level 15

JoinedFeb 15, 2006Messages851

Resources: 9

#### Resources
Maps 4Spells and Systems 2Tutorials 3

Starquizer... you won me in the answer

#### Wa666r

#### Wa666r

 Level 33

JoinedFeb 19, 2011Messages875

Resources: 32

#### Resources
Packs 1Maps 28User Interface 3

Sorry again I tried to fix the other Ai but it didn't work, so I decided to mimic your Ai but instead with humans. Needless it doesn't work and I do not why?


When I test that Ai in the Ai Editor, the night even ai works while my human ai just mines gold. So could you help a beginner out again?


//******************************************************************************


//* Human Campaign AI *


//* By wa666r *


//******************************************************************************


//*


globals


 player MyVictim = Player( 0 )


endglobals


function ConfigureAI takes nothing returns nothing


 call SetSlowChopping( false )


 call SetPeonsRepair( true )


endfunction


function hero_levels takes nothing returns integer


 local integer hero = GetHeroId()


 local integer level = GetHeroLevelAI()


 local integer a = 0


 if hero == 'Hpal' then


 if level == 1 or level == 3 or level == 5 then


 set a = 'AHhb'


 endif


 if level == 2 or level == 4 or level == 7 then


 set a = 'AHds'


 endif


 if level >= 8 then


 set a = 'AHad'


 endif


 if level == 6 then


 set a = 'AHre'


 endif


 endif


 if hero == 'Hamg' then


 if level == 1 or level == 3 or level == 5 then


 set a = 'AHbz'


 endif


 if level == 2 or level == 4 or level == 7 then


 set a = 'AHad'


 endif


 if level >= 8 then


 set a = 'AHwe'


 endif


 if level == 6 then


 set a = 'AHmt'


 endif


 endif


 return a


endfunction


function main takes nothing returns nothing


 call CampaignAI( 'hhou', function hero_levels )


 call ConfigureAI( )


 // **********************************


 // * Building Strategy *


 // **********************************


 //


 // Tier 1 Buildings


 call SetReplacements( 1, 2, 3 )


 call SetBuildUnitEx( 1, 1, 1, 'htow' )


 call SetBuildUnit( 15, 'hpea' )


 call SetBuildUnitEx( 1, 2, 3, 'hhou' )


 call SetBuildUnitEx( 1, 2, 2, 'hbar' )


 call SetBuildUnit( 1, )


 call SetBuildUnit( 1, 'hbla' )


 call CampaignDefenderEx( 2, 2, 3, 'hfoo' )


 call CampaignDefenderEx( 1, 1, 1, 'hrif' )


 call SetBuildUnitEx( 1, 2, 3, 'hwtw' )


 // Tier 2 buildings


 call SetBuildUnit( 1, 'hkee' )


 call SetBuildUnitEx( 1, 2, 2, 'hars' )


 call SetBuildUnit( 1, 'harm' )


 // Tier 3 buildings


 call SetBuildUnit( 1, 'hcas' )


 call SetBuildUnitEx( 0, 1, 1, 'hcas' )


 // **********************************


 // * End Building Strategy *


 // **********************************


 // **********************************


 // * Attack Strategy *


 // **********************************


 //*** WAVE 1 *** AI will begin to attack in 5 minutes


 call InitAssaultGroup()


 call CampaignAttackerEx( 2, 3, 3, 'hfoo')


 call CampaignAttackerEx( 0, 1, 2, 'hrif' )


 call SuicideOnPlayerEx( M5, M5, M5, MyVictim )


 //*** Basic upgrades from HUNTERS HALL ***


 call SetBuildUpgrEx( 1, 2, 3, 'Rhme' )


 call SetBuildUpgrEx( 1, 2, 3, 'Rhar' )


 call SetBuildUpgrEx( 1, 2, 3, 'Rhla' )


 call SetBuildUpgrEx( 1, 2, 3, 'Rhra' )


 //*** WAVE 2 *** Between 2 or 3 minutes after Wave 1


 call InitAssaultGroup()


 call CampaignAttackerEx( 3, 4, 4, 'hfoo' )


 call CampaignAttackerEx( 1, 2, 3, 'hrif' )


 call SuicideOnPlayerEx( M3, M2, M2, MyVictim )


 //*** WAVE 3 *** Between 2 or 3 minutes after Wave 2


 call InitAssaultGroup()


 call CampaignAttackerEx( 3, 4, 4, 'hfoo' )


 call CampaignAttackerEx( 1, 2, 3, 'hrif' )


 call CampaignAttackerEx( 1, 1, 1, 'Hpal' )


 call SuicideOnPlayerEx( M3, M2, M2, MyVictim )


 //*** WAVE 4 *** Between 2 or 3 minutes after Wave 3


 call InitAssaultGroup()


 call CampaignAttackerEx( 3, 4, 4, 'hfoo' )


 call CampaignAttackerEx( 1, 2, 3, 'hrif' )


 call CampaignAttackerEx( 0, 1, 1, 'hmtm' )


 call CampaignAttackerEx( 1, 1, 1, 'Hpal' )


 call SuicideOnPlayerEx( M3, M2, M2, MyVictim )


 //*** Dryad Upgrade


 call SetBuildUpgrEx( 1, 1, 1, 'Rhde' )


 //*** WAVE 5 *** Between 3 or 4 minutes after Wave 4


 call InitAssaultGroup()


 call CampaignAttackerEx( 3, 3, 3, 'hfoo' )


 call CampaignAttackerEx( 2, 2, 2, 'hrif' )


 call CampaignAttackerEx( 1, 1, 1, 'Hpal' )


 call CampaignAttackerEx( 0, 1, 2, 'hmpr' )


 call SuicideOnPlayerEx( M4, M3, M3, MyVictim )


 //*** Druid of the Claw Upgrade


 call SetBuildUpgrEx( 0, 1, 2, 'Rhpt' )


 //*** WAVE 6 *** Between 3 or 4 minutes after Wave 5


 call InitAssaultGroup()


 call CampaignAttackerEx( 2, 2, 2, 'Hrif' )


 call CampaignAttackerEx( 1, 1, 1, 'Hpal' )


 call CampaignAttackerEx( 1, 1, 1, 'Hamg' )


 call CampaignAttackerEx( 1, 2, 3, 'hmpr' )


 call CampaignAttackerEx( 0, 1, 2, 'hkni' )


 call SuicideOnPlayerEx( M4, M3, M3, MyVictim )


 //*** chimaera Upgrade


 call SetBuildUpgrEx( 0, 0, 1, 'Rhri' )


 loop //Init the infinite attack loop


 //*** WAVE 7 *** Between 3 or 5 minutes after Wave 6


 call InitAssaultGroup()


 call CampaignAttackerEx( 2, 2, 2, 'hfoo' )


 call CampaignAttackerEx( 1, 1, 1, 'Hpal' )


 call CampaignAttackerEx( 1, 1, 1, 'Hamg' )


 call CampaignAttackerEx( 1, 2, 3, 'hkni' )


 call CampaignAttackerEx( 0, 1, 2, 'hrif' )


 call CampaignAttackerEx( 0, 1, 2, 'hsor' )


 call SuicideOnPlayerEx( M5, M4, M3, MyVictim )


 //*** WAVE 8 *** Between 3 or 5 minutes after Wave 7


 call InitAssaultGroup()


 call CampaignAttackerEx( 2, 2, 2, 'hfoo' )


 call CampaignAttackerEx( 1, 1, 1, 'Hpak')


 call CampaignAttackerEx( 1, 1, 1, 'Hamg' )


 call CampaignAttackerEx( 1, 2, 3, 'hmpr' )


 call CampaignAttackerEx( 0, 1, 2, 'hsor' )


 call SuicideOnPlayerEx( M5, M4, M3, MyVictim )


 //*** WAVE 9 *** Between 4 or 6 minutes after Wave 8


 call InitAssaultGroup()


 call CampaignAttackerEx( 2, 2, 2, 'hrif' )


 call CampaignAttackerEx( 1, 1, 1, 'Hpal' )


 call CampaignAttackerEx( 1, 1, 1, 'Hamg' )


 call CampaignAttackerEx( 1, 2, 3, 'hfoo' )


 call CampaignAttackerEx( 0, 1, 2, 'hsor' )


 call CampaignAttackerEx( 0, 1, 2, 'hmpr' )


 call CampaignAttackerEx( 1, 1, 2, 'hspt' )


 call CampaignAttackerEx( 0, 1, 2, 'hgry' )


 call SuicideOnPlayerEx( M6, M5, M4, MyVictim )


 endloop


 // **********************************


 // * Attack Strategy never ends *


 // **********************************


endfunction

#### moyackx

#### moyackx

 Level 15

JoinedFeb 15, 2006Messages851

Resources: 9

#### Resources
Maps 4Spells and Systems 2Tutorials 3

As I thought, it was a code error (check the line 63, with the comment):









 JASS:

//******************************************************************************
//* Human Campaign AI *
//* By wa666r *
//******************************************************************************
//*
globals
player MyVictim = Player( 0 )
endglobals

function ConfigureAI takes nothing returns nothing
call SetSlowChopping( false )
call SetPeonsRepair( true )
endfunction

function hero_levels takes nothing returns integer
local integer hero = GetHeroId()
local integer level = GetHeroLevelAI()
local integer a = 0
if hero == 'Hpal' then
if level == 1 or level == 3 or level == 5 then
set a = 'AHhb'
endif
if level == 2 or level == 4 or level == 7 then
set a = 'AHds'
endif
if level >= 8 then
set a = 'AHad'
endif
if level == 6 then
set a = 'AHre'
endif
endif
if hero == 'Hamg' then
if level == 1 or level == 3 or level == 5 then
set a = 'AHbz'
endif
if level == 2 or level == 4 or level == 7 then
set a = 'AHad'
endif
if level >= 8 then
set a = 'AHwe'
endif
if level == 6 then
set a = 'AHmt'
endif
endif
return a
endfunction

function main takes nothing returns nothing
call CampaignAI( 'hhou', function hero_levels )
call ConfigureAI( )
// **********************************
// * Building Strategy *
// **********************************
//
// Tier 1 Buildings
call SetReplacements( 1, 2, 3 )
call SetBuildUnitEx( 1, 1, 1, 'htow' )
call SetBuildUnit( 15, 'hpea' )
call SetBuildUnitEx( 1, 2, 3, 'hhou' )
call SetBuildUnitEx( 1, 2, 2, 'hbar' )
call SetBuildUnit( 1, ) //Here's your mistake.... missing arguments...
call SetBuildUnit( 1, 'hbla' )
call CampaignDefenderEx( 2, 2, 3, 'hfoo' )
call CampaignDefenderEx( 1, 1, 1, 'hrif' )
call SetBuildUnitEx( 1, 2, 3, 'hwtw' )
// Tier 2 buildings
call SetBuildUnit( 1, 'hkee' )
call SetBuildUnitEx( 1, 2, 2, 'hars' )
call SetBuildUnit( 1, 'harm' )
// Tier 3 buildings
call SetBuildUnit( 1, 'hcas' )
call SetBuildUnitEx( 0, 1, 1, 'hcas' )
// **********************************
// * End Building Strategy *
// **********************************
// **********************************
// * Attack Strategy *
// **********************************
//*** WAVE 1 *** AI will begin to attack in 5 minutes
call InitAssaultGroup()
call CampaignAttackerEx( 2, 3, 3, 'hfoo')
call CampaignAttackerEx( 0, 1, 2, 'hrif' )
call SuicideOnPlayerEx( M5, M5, M5, MyVictim )
//*** Basic upgrades from HUNTERS HALL ***
call SetBuildUpgrEx( 1, 2, 3, 'Rhme' )
call SetBuildUpgrEx( 1, 2, 3, 'Rhar' )
call SetBuildUpgrEx( 1, 2, 3, 'Rhla' )
call SetBuildUpgrEx( 1, 2, 3, 'Rhra' )
//*** WAVE 2 *** Between 2 or 3 minutes after Wave 1
call InitAssaultGroup()
call CampaignAttackerEx( 3, 4, 4, 'hfoo' )
call CampaignAttackerEx( 1, 2, 3, 'hrif' )
call SuicideOnPlayerEx( M3, M2, M2, MyVictim )
//*** WAVE 3 *** Between 2 or 3 minutes after Wave 2
call InitAssaultGroup()
call CampaignAttackerEx( 3, 4, 4, 'hfoo' )
call CampaignAttackerEx( 1, 2, 3, 'hrif' )
call CampaignAttackerEx( 1, 1, 1, 'Hpal' )
call SuicideOnPlayerEx( M3, M2, M2, MyVictim )
//*** WAVE 4 *** Between 2 or 3 minutes after Wave 3
call InitAssaultGroup()
call CampaignAttackerEx( 3, 4, 4, 'hfoo' )
call CampaignAttackerEx( 1, 2, 3, 'hrif' )
call CampaignAttackerEx( 0, 1, 1, 'hmtm' )
call CampaignAttackerEx( 1, 1, 1, 'Hpal' )
call SuicideOnPlayerEx( M3, M2, M2, MyVictim )
//*** Dryad Upgrade
call SetBuildUpgrEx( 1, 1, 1, 'Rhde' )
//*** WAVE 5 *** Between 3 or 4 minutes after Wave 4
call InitAssaultGroup()
call CampaignAttackerEx( 3, 3, 3, 'hfoo' )
call CampaignAttackerEx( 2, 2, 2, 'hrif' )
call CampaignAttackerEx( 1, 1, 1, 'Hpal' )
call CampaignAttackerEx( 0, 1, 2, 'hmpr' )
call SuicideOnPlayerEx( M4, M3, M3, MyVictim )
//*** Druid of the Claw Upgrade
call SetBuildUpgrEx( 0, 1, 2, 'Rhpt' )
//*** WAVE 6 *** Between 3 or 4 minutes after Wave 5
call InitAssaultGroup()
call CampaignAttackerEx( 2, 2, 2, 'Hrif' )
call CampaignAttackerEx( 1, 1, 1, 'Hpal' )
call CampaignAttackerEx( 1, 1, 1, 'Hamg' )
call CampaignAttackerEx( 1, 2, 3, 'hmpr' )
call CampaignAttackerEx( 0, 1, 2, 'hkni' )
call SuicideOnPlayerEx( M4, M3, M3, MyVictim )
//*** chimaera Upgrade
call SetBuildUpgrEx( 0, 0, 1, 'Rhri' )
loop //Init the infinite attack loop
//*** WAVE 7 *** Between 3 or 5 minutes after Wave 6
call InitAssaultGroup()
call CampaignAttackerEx( 2, 2, 2, 'hfoo' )
call CampaignAttackerEx( 1, 1, 1, 'Hpal' )
call CampaignAttackerEx( 1, 1, 1, 'Hamg' )
call CampaignAttackerEx( 1, 2, 3, 'hkni' )
call CampaignAttackerEx( 0, 1, 2, 'hrif' )
call CampaignAttackerEx( 0, 1, 2, 'hsor' )
call SuicideOnPlayerEx( M5, M4, M3, MyVictim )
//*** WAVE 8 *** Between 3 or 5 minutes after Wave 7
call InitAssaultGroup()
call CampaignAttackerEx( 2, 2, 2, 'hfoo' )
call CampaignAttackerEx( 1, 1, 1, 'Hpak')
call CampaignAttackerEx( 1, 1, 1, 'Hamg' )
call CampaignAttackerEx( 1, 2, 3, 'hmpr' )
call CampaignAttackerEx( 0, 1, 2, 'hsor' )
call SuicideOnPlayerEx( M5, M4, M3, MyVictim )
//*** WAVE 9 *** Between 4 or 6 minutes after Wave 8
call InitAssaultGroup()
call CampaignAttackerEx( 2, 2, 2, 'hrif' )
call CampaignAttackerEx( 1, 1, 1, 'Hpal' )
call CampaignAttackerEx( 1, 1, 1, 'Hamg' )
call CampaignAttackerEx( 1, 2, 3, 'hfoo' )
call CampaignAttackerEx( 0, 1, 2, 'hsor' )
call CampaignAttackerEx( 0, 1, 2, 'hmpr' )
call CampaignAttackerEx( 1, 1, 2, 'hspt' )
call CampaignAttackerEx( 0, 1, 2, 'hgry' )
call SuicideOnPlayerEx( M6, M5, M4, MyVictim )
endloop
// **********************************
// * Attack Strategy never ends *
// **********************************
endfunction

I suggest to use Jasscraft: http://www.wc3c.net/showthread.php?t=80051&highlight=jasscraft as your AI editor, it helps detecting typos because it uses pjass as syntax checker. when you test it, ensure you set common.ai and common.j in the settings dialog (F8) in the "jass enchantements" tab. This in order to do the proper debugging

#### Wa666r

#### Wa666r

 Level 33

JoinedFeb 19, 2011Messages875

Resources: 32

#### Resources
Packs 1Maps 28User Interface 3

It works!!

Thanks a lot man!

But hey, how would you upgrade towers, would it just be: call SetBuildUnit( 1, 'hgtw''' ) ?

#### Starquizer

#### Starquizer

 Level 16

JoinedMar 3, 2006Messages1,564

Resources: 7

#### Resources
Maps 1Spells and Systems 3Tutorials 3

Wa666r said:

 It works!!

Thanks a lot man!

But hey, how would you upgrade towers, would it just be: call SetBuildUnit( 1, 'hgtw''' ) ?


Click to expand...

yes.

#### Wa666r

#### Wa666r

 Level 33

JoinedFeb 19, 2011Messages875

Resources: 32

#### Resources
Packs 1Maps 28User Interface 3

Hey man, its me again, I was wondering if there is any special codes I would have to put for a costume race.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Wa666r said:

 Hey man, its me again, I was wondering if there is any special codes I would have to put for a costume race.


Click to expand...

No, you don't need special codes for JASS based AI, but remember, JASS AI used Rawcode, be sure to check the rawcode of every costum unit you use, since incorrect rawcode causes the AI to break and don't work.

If I'm wrong, I'm very sorry, that's because I don't make Camp. AI's, so very sorry if that happened.

~DaffaMage


 Last edited: Apr 19, 2013

#### moyackx

#### moyackx

 Level 15

JoinedFeb 15, 2006Messages851

Resources: 9

#### Resources
Maps 4Spells and Systems 2Tutorials 3

Wa666r said:

 Hey man, its me again, I was wondering if there is any special codes I would have to put for a costume race.


Click to expand...

In the object Editor Press CTRL+D and check the rawcodes of the units/buildings/abilities. Those can be used in your custom race and it will work for you.

#### Starquizer

#### Starquizer

 Level 16

JoinedMar 3, 2006Messages1,564

Resources: 7

#### Resources
Maps 1Spells and Systems 3Tutorials 3

moyackx said:

 In the object Editor Press CTRL+D and check the rawcodes of the units/buildings/abilities. Those can be used in your custom race and it will work for you.


Click to expand...

moyackx said:

 Starquizer... you won me in the answer

Click to expand...

You won me in the answer, this time.

Now, we are even

#### Wa666r

#### Wa666r

 Level 33

JoinedFeb 19, 2011Messages875

Resources: 32

#### Resources
Packs 1Maps 28User Interface 3

What does it mean when the custom AI gathering 10 gold instead of the slow harvest 1 gold? Even if I have this: call SetSlowChopping( true )

#### Starquizer

#### Starquizer

 Level 16

JoinedMar 3, 2006Messages1,564

Resources: 7

#### Resources
Maps 1Spells and Systems 3Tutorials 3

Wa666r said:

 What does it mean when the custom AI gathering 10 gold instead of the slow harvest 1 gold? Even if I have this: call SetSlowChopping( true )


Click to expand...

You may have errors in the script and it is not running at all, which will make the AI player do nothing except gathering resources.

#### KONSTANTIN

#### KONSTANTIN

 Level 2

JoinedJun 29, 2013Messages13

hi. first of all i wanna thank moya for this nice guide, cuz im a total newb when it comes to ai and jass.


i wanted to make campaign ai that doesnt mine gold, with custom units from a modified race that will attack occasionaly when triggered. this is my script


of course i gave him gold and lumber, when runs out it recharges again. i made a modified structure Orc Fortress that would make him primary food limit of 90, without need to build extra burrows, but then i just changed it to regular burrows in his camp.


//============================================================================


// SKELETAL ORCS -- purple player -- AI Script


//============================================================================


globals


 Player Kurac = Player(0)


endglobals


//============================================================================


// main


//============================================================================


function ConfigureAI main takes nothing returns nothing


 call DisplayTextToPlayer(Player(0),0,0,"AI started")


 call CampaignAI(otrb,null)


 call SetReplacements(1,1,3)


 call CampaignDefenderEx( 3,3,5, nsko )


 call CampaignDefenderEx( 1,2,2, nsog )


 call CampaignDefenderEx( 0,0,1, nsoc )


 call CampaignDefenderEx( 0,0,1, nsm )


 call WaitForSignal()


 //*** WAVE 1 ***


 call InitAssaultGroup()


 call CampaignAttackerEx( 1, 1, 1, nsko )


 call CampaignAttackerEx( 3,3,5, nsog )


 call SuicideOnPlayer(M1, M1, M2, Kurac)


 //*** WAVE 2 ***


 call InitAssaultGroup()


 call CampaignAttackerEx( 6,6,8, nsko )


 call CampaignAttackerEx( 2,2,3, nsog )


 call SuicideOnPlayerEx(M1, M1, M2, Kurac)


 //*** WAVE 3 ***


 call InitAssaultGroup()


 call CampaignAttackerEx( 2,2,5, nsko )


 call CampaignAttackerEx( 1,1,3, ocat )


 call CampaignAttackerEx( 5,5,8, nsko )


 call SuicideOnPlayerEx(M1, M1, M2, Kurac)


 loop


 //*** WAVE 4+ ***


 call InitAssaultGroup()


 call CampaignAttackerEx( 5,5,7, nsko )


 call CampaignAttackerEx( 2,2,4, nsog )


 call SuicideOnPlayerEx(M1, M1, M2, Kurac)


 //*** WAVE 5+ ***


 call InitAssaultGroup()


 call CampaignAttackerEx( 2,2,3, nsko )


 call CampaignAttackerEx( 4,4,6, nsog )


 call CampaignAttackerEx( 1,1,1, nsoc )


 call SuicideOnPlayerEx(M1, M2, M2, Kurac)


 //*** WAVE 6+ ***


 call InitAssaultGroup()


 call CampaignAttackerEx( 6,6,8, nsko )


 call CampaignAttackerEx( 4,4,6, nsog )


 call SuicideOnPlayerEx(M1, M1, M2, Kurac)


 //*** WAVE 7+ ***


 call InitAssaultGroup()


 call CampaignAttackerEx( 4,4,6, nsko )


 call CampaignAttackerEx( 3,3,6, nsog )


 call CampaignAttackerEx( 2,2,3, nsoc )


 call CampaignAttackerEx( 0,0,1, ocat )


 call SuicideOnPlayerEx(M1, M1, M1, Kurac )


 endloop


endfunction


i am trying for days to get it to work reading here and there, but i cannot make him to work it wont do anything

#### moyackx

#### moyackx

 Level 15

JoinedFeb 15, 2006Messages851

Resources: 9

#### Resources
Maps 4Spells and Systems 2Tutorials 3

You forgot to add the marks to the names:

 JASS:

call CampaignAI('otrb',null)
call SetReplacements(1,1,3)

call CampaignDefenderEx( 3,3,5, 'nsko' )
call CampaignDefenderEx( 1,2,2, 'nsog' )
call CampaignDefenderEx( 0,0,1, 'nsoc' )
call CampaignDefenderEx( 0,0,1, 'nsm' ) // Here I'm pretty sure there's one mistake

Do this for every raw integer.

#### KONSTANTIN

#### KONSTANTIN

 Level 2

JoinedJun 29, 2013Messages13

tried it, didnt work. i downloaded jasscraft for error fixing and comparing to common.ai. tried multiple times checked, and checked again and again and nothing seems to work, then i just made it simple and used onyl 3 custom units for player with one producing structure. then i played with common.ai cause i saw blizzard done that when they used custom units in campaigns...


 constant integer SKEL_ORC = 'nsko'


 constant integer SKEL_ORC_GRUNT = 'nsog'


 constant integer SKEL_ORC_CHAMP = 'nsoc'


i insterted this in common.ai and saved thru mpq in frozen throne mpq file.


now my script looks like this


//============================================================================


// SKELETAL ORCS -- purple player -- AI Script


//============================================================================


globals


 player user = Player(1)


endglobals


//============================================================================


// main


//============================================================================


function ConfigureAI takes nothing returns nothing


 call CampaignAI(BURROW,null)


 call SetReplacements(1,1,3)


 call CampaignDefenderEx( 1, 2, 3, SKEL_ORC )


 call CampaignDefenderEx( 1, 2, 3, SKEL_ORC_GRUNT )


 call WaitForSignal()


 //*** WAVE 1 ***


 call InitAssaultGroup()


 call CampaignAttackerEx( 1, 1, 1, SKEL_ORC )


 call CampaignAttackerEx( 3,3,5, SKEL_ORC_GRUNT )


 call SuicideOnPlayer(M1, user )


 //*** WAVE 2 ***


 call InitAssaultGroup()


 call CampaignAttackerEx( 6,6,8, SKEL_ORC )


 call CampaignAttackerEx( 2,2,3, SKEL_ORC_GRUNT )


 call SuicideOnPlayerEx(M1, M1, M2, user )


 //*** WAVE 3 ***


 call InitAssaultGroup()


 call CampaignAttackerEx( 2,2,5, SKEL_ORC )


 call CampaignAttackerEx( 1,1,3, CATAPULT )


 call CampaignAttackerEx( 2,2,3, SKEL_ORC_GRUNT )


 call SuicideOnPlayerEx(M1, M1, M2, user )


 loop


 //*** WAVE 4+ ***


 call InitAssaultGroup()


 call CampaignAttackerEx( 5,5,7, SKEL_ORC )


 call CampaignAttackerEx( 2,2,4, SKEL_ORC_GRUNT )


 call SuicideOnPlayerEx(M1, M1, M2, user )


 //*** WAVE 5+ ***


 call InitAssaultGroup()


 call CampaignAttackerEx( 2,2,3, SKEL_ORC_GRUNT )


 call CampaignAttackerEx( 4,4,6, SKEL_ORC )


 call CampaignAttackerEx( 1,1,1, SKEL_ORC_CHAMP )


 call SuicideOnPlayerEx(M1, M2, M2, user )


 //*** WAVE 6+ ***


 call InitAssaultGroup()


 call CampaignAttackerEx( 6,6,8, SKEL_ORC )


 call CampaignAttackerEx( 4,4,6, SKEL_ORC_GRUNT )


 call SuicideOnPlayerEx(M1, M1, M2, user )


 //*** WAVE 7+ ***


 call InitAssaultGroup()


 call CampaignAttackerEx( 4,4,6, SKEL_ORC )


 call CampaignAttackerEx( 3,3,4, SKEL_ORC_GRUNT )


 call CampaignAttackerEx( 1,1,1, SKEL_ORC_CHAMP )


 call CampaignAttackerEx( 1,2,3, CATAPULT )


 call SuicideOnPlayerEx(M1, M1, M1, user )


 endloop


endfunction


jasscraft recognized this functions from common.ai and said there were no errors, but then it still doesnt work.. am i doing something wrong in the map itself maybe, cause i linked ai to player, and sent commands (0,0) ? it uses orcs structures with custom units, so i think its ok to link food producer to be orc burrow

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Where's the build order?

Also, use Jass tag when posting the code.

#### KONSTANTIN

#### KONSTANTIN

 Level 2

JoinedJun 29, 2013Messages13

as i said in previous post there is none. i need this ai only for attacking and defending without mining, repairing units. i mimicked the script from blizzard ai in wc3 when they used the script for "Corrupted Ancients" the exact same thing i want to use here.

oh and sry about not using code earlier...

#### KONSTANTIN

#### KONSTANTIN

 Level 2

JoinedJun 29, 2013Messages13

so... any ideas ?

#### moyackx

#### moyackx

 Level 15

JoinedFeb 15, 2006Messages851

Resources: 9

#### Resources
Maps 4Spells and Systems 2Tutorials 3

are there buidings that actually can train those skeletal orcs?? if not, then that's the reason of your issue

#### KONSTANTIN

#### KONSTANTIN

 Level 2

JoinedJun 29, 2013Messages13

of course. i set orc barracks to train them. they just wont do anything, wont move, wont train nothing.


just a thought did maybe blizzard set some special behaviour for custom units or something that i should be aware.. ?


here is a pic of ai triggering i did.

#### Attachments

dada.png

 179.4 KB

 · Views: 370



#### moyackx

#### moyackx

 Level 15

JoinedFeb 15, 2006Messages851

Resources: 9

#### Resources
Maps 4Spells and Systems 2Tutorials 3

Ok, I saw the issue:

function ConfigureAI takes nothing returns nothing

Change it by:

function main takes nothing returns nothing

#### KONSTANTIN

#### KONSTANTIN

 Level 2

JoinedJun 29, 2013Messages13

i get nothing...

#### moyackx

#### moyackx

 Level 15

JoinedFeb 15, 2006Messages851

Resources: 9

#### Resources
Maps 4Spells and Systems 2Tutorials 3

Ahhh... please remove call WaitForSignal()

#### KONSTANTIN

#### KONSTANTIN

 Level 2

JoinedJun 29, 2013Messages13

hey man IT WORKS FINNALY!


its not the signal, its just me FORGETING TO IMPORT COMMON.AI IN WC3 folder data. JESUS CHRIST. i was working from the start from my copied .mpq files in another directory and then when milions thing got complicated about directories being protected and taking ages to copy paste 400 mb mpq files over and over cause of testing i was just fuck it lets try one more time with raw names, and voila. moya im rly sry about wasting your time, i just started this map moding like 10 days ago, and it finnaly works after days of testing, im happy AS FUCK!!


ps i just found out that i actually DID copied mpq in wc3, its just when reading integers in jasscraft one name of one unit was marked in white while others were silver. and raw was right i checked! so i dunno why it didnt wanna get raw out of that unit...

#### Attachments

sad.png

 13.9 KB

 · Views: 394



#### Abextadoo

#### Abextadoo

 Level 3

JoinedNov 1, 2012Messages27

is there a place in the site where we can download pre-made AI? i've been looking for a while but it seems like no..

#### Starquizer

#### Starquizer

 Level 16

JoinedMar 3, 2006Messages1,564

Resources: 7

#### Resources
Maps 1Spells and Systems 3Tutorials 3

Abextadoo said:

 is there a place in the site where we can download pre-made AI? i've been looking for a while but it seems like no..


Click to expand...


I think there a few pre-made scripts but they are few as a few people care about AI, you will have to search the web for them but I think it is better to make your own script.

#### PrinceOfThaWest

#### PrinceOfThaWest

 Level 12

JoinedMar 6, 2008Messages1,055

This tutorial helped me a lot. However I managed to learn basic AI script thanks to Demo Custom Campaign made by Blizzard Entertainment. They provided basic script there which is used for Humans in first chapter. However yes, JASS AI making surely is the best. I'm still trying to get it under hand, so far so good.

#### Starquizer

#### Starquizer

 Level 16

JoinedMar 3, 2006Messages1,564

Resources: 7

#### Resources
Maps 1Spells and Systems 3Tutorials 3

Witch-King said:

 This tutorial helped me a lot. However I managed to learn basic AI script thanks to Demo Custom Campaign made by Blizzard Entertainment. They provided basic script there which is used for Humans in first chapter. However yes, JASS AI making surely is the best. I'm still trying to get it under hand, so far so good.


Click to expand...


If you also need to know the description of most of the AI natives go here (the AI manual) as well.


With those 2 tutorial you could master your AI scripting skills.

#### PrinceOfThaWest

#### PrinceOfThaWest

 Level 12

JoinedMar 6, 2008Messages1,055

Starquizer said:

 If you also need to know the description of most of the AI natives go here (the AI manual) as well.


With those 2 tutorial you could master your AI scripting skills.


Click to expand...


I am still sort of newbie for this whole JASS thing, to be honest. I barely know to code that much but I managed to compile some skills out of this tutorial and demo campaign made by Blizzard. There are still some things I'm yet about to learn when it comes to Campaign AI scripts but yeah, I'm on a good way.

#### MasterHaosis

#### MasterHaosis

Patron

 Level 46

JoinedSep 18, 2007Messages2,331

Resources: 37

#### Resources
Models 19Icons 15Maps 3

Witch-King said:

 and demo campaign made by Blizzard.


Click to expand...


Which demo campaign?

#### Starquizer

#### Starquizer

 Level 16

JoinedMar 3, 2006Messages1,564

Resources: 7

#### Resources
Maps 1Spells and Systems 3Tutorials 3

MasterHaosis said:

 Which demo campaign?


Click to expand...


I guess the Orc campaign that was in the demo of WarCraft 3, the one that Thrall stuck on an isle facing murlocs.

#### MasterHaosis

#### MasterHaosis

Patron

 Level 46

JoinedSep 18, 2007Messages2,331

Resources: 37

#### Resources
Models 19Icons 15Maps 3

Ah yes. And that campaign has imported custom AI?

#### PrinceOfThaWest

#### PrinceOfThaWest

 Level 12

JoinedMar 6, 2008Messages1,055

Yes, that campaign. And yeah, there are custom AI made by Blizzard specially for that campaign.

 You must log in or register to reply here.


### Similar threads

 Must Have Campaign AI Programming Techniques


Tommi Gustafsson

Jan 8, 2018

AI Tutorials

Replies5Views9K

Mar 31, 2023
Barade

Locked

[AI]
 Help making Campaign AI scripts


Gudys_Revenge

Sep 3, 2005

Triggers & Scripts

Replies0Views2K

Sep 3, 2005
Gudys_Revenge

Locked

[AI]
 How to make a Campaign AI ?


Jaakko

May 15, 2007

Triggers & Scripts

Replies1Views2K

May 16, 2007
LordDz

Locked

 Help? Creating AI for Campaign?


Glevion

Jul 16, 2016

World Editor Help Zone

Replies11Views5K

Jul 17, 2016
Shar Dundred

D

Locked

[AI]
 Campaign AI Won't Run


DoomCloudX

Apr 15, 2015

Triggers & Scripts

Replies0Views1K

Apr 15, 2015
DoomCloudX

D

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

AI Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top