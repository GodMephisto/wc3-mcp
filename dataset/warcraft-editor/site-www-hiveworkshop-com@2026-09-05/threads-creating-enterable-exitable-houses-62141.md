---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/creating-enterable-exitable-houses.62141/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Creating Enterable Exitable Houses
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/creating-enterable-exitable-houses.62141/>
#### Tekal

#### Tekal

 Level 4

JoinedDec 14, 2007Messages40

Resources: 1

#### Resources
Tutorials 1

//=======================================================//


//First Off This tutorial is public u can use it all u want and create some pimped out buildings that are enterable and can have shops inside of them and all kinds of stuf//


Requirements - Basic WE Knowledge,Medium GUI Knowlege,The Abilty To Download Files From wc3campaigns.net(OPTIONAL)

1 - Basics Of Creating The House

2 - The Triggering(G U I)

3 - Making a 2 Story+ House

4 - Creating Complex Houses

5 - Extra

Coming Soon»Demo Map,

First Off To Create This We Will Need To Go To »Object Editor»Units Tab»New Unit,First Off We Create A New Unit,Base it on iuno Peasant?»Name It HouseV000


^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^


if u dont know how to do this dont even go any further on this tut cuz ull never understand it


Ok Now Change Theese Values On The Unit To Theese Specififed


ok now that weve got that done find a spot in your map and place the new unit we just created,now we will create a region for the house,Create A Region The Exact Size Of The House And Name It...House000 or sumthing it doesnt realy matter


And Now So That Units cannot enter from where ever we will put Pathing Blockers To Evrywhere but where u want the units to enter at heres an example\/ \/ \/ \/


Its Optional To Create Line Of Sight Blockers Outside The Building So They Cant See Outside


Now Will Create Some Scenery Inside The House i suggjest using http://www.wc3campaigns.net/showthread.php?t=85128


Now Create A New doodad call it like floor,Set Its Pathing»Pathing Texture»None so the unit can walk on it,Set its minimum scale to 0.20 and maximum to 10.00 And Put The Floor In The Building And Scale It To The Correct Size To Fit Perfectly Like So


Put Tables Or Anything You Want Inside The Building.


Now For The Semi-Hard Stuf,If u dont know how to use gui im truly sorry

2.Ok Create A New Trigger Catagory Call It House Enteration,And Create A Trigger And Call It House Initilization,Make It Hide Evry Unit INSIDE The House DO NOT HIDE THE HOUSE!!!! u may need to temporarly move the house to select units inside!The Triger should look somthing Like so

House Init
Events
Map initialization

Conditions

Actions
Unit - Hide Paladin 0002 <gen>

2.1Create A New Trigger Called House0000E And Make It Look Like This

House0000E
Events
Unit - A unit enters House0000 <gen>

Conditions

Actions
Camera - Set the camera bounds for (Owner of (Entering unit)) to House0000 <gen>

Unit - Hide HouseV000 0000 <gen>

-------- Now Make Actions To Unhide All Units In The House --------

Unit - Unhide Paladin 0002 <gen>

2.2Now We Will Create a Trigger To Exit Name It House0000L The House It Should Look Somthing Like This\./

House0000L
Events
Unit - A unit leaves House0000 <gen>

Conditions

Actions
Camera - Set the camera bounds for (Owner of (Entering unit)) to (Playable map area)

Unit - Unhide HouseV000 0000 <gen>

-------- Now We Will Hide All Units In The House --------

Unit - Hide Paladin 0002 <gen>

3.Now We Are Going To Make This Building Have a Second Floor To Do So We Will Need a Stair Recource,I Suggjest The Stair Model From http://www.wc3campaigns.net/showthread.php?t=88797


Ok Now We Do The EXACT Same Thing For The Building


Create A New Doodad Called Stairs Make It Like Floor,(Same Scaling And Stuf) But Make The Model To The Stairs


Put The Stairs Some Where In Your House A Create a new region infront of the stairs name it sumthing to do with stairs,Make It about 4x4 or what ever side it needs to be for it to fit infront of the stairs example below


Now That Weve Got That Done Go To Somewhere In Your Map Maybe A Corner And Create A Place Like This I Used WEUnlimmited To Add The Abyss Tile So,You Can Use Boundary Or Use WEUnlimmited To Use Abyss i do beleave its on this site I Used The Wooden Floor Model I Shows U Earlier And Tinted It To 0red0blue0green and scaled it down to make the hole,And Create A Region Like This And Name It Sumthing Else

3.1The GUI Triggering For This Should Be Sumthing Like This


Going Up

Going Up0001
Events
Unit - A unit enters House0001Stairs <gen>

Conditions

Actions
Camera - Set the camera bounds for (Owner of (Entering unit)) to House0001UpStairs <gen>

Unit - Move (Entering unit) instantly to (Center of House0001UpStairs <gen>)

Going Down

Going Down0001
Events
Unit - A unit enters House0001StairsDown <gen>

Conditions

Actions
Camera - Set the camera bounds for (Owner of (Entering unit)) to House0001 <gen>

Unit - Move (Entering unit) instantly to (Center of House0001 <gen>)

4.Ok Now Ill Show U How To Make a Complex House(2 or more rooms and stuf like that) hes a semi-simple way to make Complex House..First Create Another House UNIT same specs as the other ones but this time for the model put it to sumthing like







 Code:

City Large Building

 Horizontal Or Verticle But Not Diagnal,now place it sum where in your map and put the tiles in it and evrything you want,As youll notice you cant make 1 region fit the whole building without making it go outside of the building this is where ill show u how to fix that problem,First Off Create A Region To Fit The First Part Of It Like So


Now We Will Create a Second Region Like This


Be SURE to Overlap The Two Regions Or It Wont Work


Ok,Now i will create the triggers and show you them

House0002E
Events
Unit - A unit enters House0002 1 <gen>

Conditions
(House0002 2 <gen> contains (Entering unit)) Equal to False

Actions
Camera - Set the camera bounds for (Owner of (Entering unit)) to House0002 1 <gen>

Unit - Hide HouseV001 0006 <gen>

House0002L
Events
Unit - A unit leaves House0002 1 <gen>

Conditions
(House0002 2 <gen> contains (Leaving unit)) Equal to False

Actions
Camera - Set the camera bounds for (Owner of (Entering unit)) to (Playable map area)

Unit - Hide HouseV001 0006 <gen>

Those Are Examples Of Trigers You Need To Make For The House To Be Like Mine

5.


Here Are Some Very Usefull Kits For Making Houses (all supported by wc3c)

http://www.wc3campaigns.net/showthread.php?t=90720

http://www.wc3campaigns.net/showthread.php?t=93560

http://www.wc3campaigns.net/showthread.php?t=90966

http://www.wc3campaigns.net/showthread.php?t=87171

http://www.wc3campaigns.net/showthread.php?t=86663

http://www.wc3campaigns.net/showthread.php?t=89524

http://www.wc3campaigns.net/showthread.php?t=88047

http://www.wc3campaigns.net/showthread.php?t=84738

http://www.wc3campaigns.net/showthread.php?t=79974

http://www.wc3campaigns.net/showthread.php?t=94799

http://www.wc3campaigns.net/showthread.php?t=86764

http://www.wc3campaigns.net/showthread.php?t=94305

http://www.wc3campaigns.net/showthread.php?t=81080

#### Attachments

1.png

 87.2 KB

 · Views: 787



2.PNG

 9.3 KB

 · Views: 814



3.PNG

 266.2 KB

 · Views: 1,086



4.PNG

 183.2 KB

 · Views: 856



5.PNG

 121.9 KB

 · Views: 827



6.PNG

 142.7 KB

 · Views: 683



7.PNG

 108.8 KB

 · Views: 567



8.PNG

 213.4 KB

 · Views: 572



9.PNG

 309 KB

 · Views: 595




 Last edited: Mar 3, 2008

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice job m8.

I think this is well written. Detach your signature first though.

Since the house will be a unit, you can always edit its vertex coloring (under animation) to make it transparent so you can see yourself walking.

Well, that's just what I prefer.

Anywho. Gj, and nice pictures.

#### Tekal

#### Tekal

 Level 4

JoinedDec 14, 2007Messages40

Resources: 1

#### Resources
Tutorials 1

Why thank you!,removed my sig

#### Tekal

#### Tekal

 Level 4

JoinedDec 14, 2007Messages40

Resources: 1

#### Resources
Tutorials 1

KaBump....................................

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Some things:

Please write normally, Not In All Caps Like This, and with proper spelling/grammar. If you wish, I could fix that up for you

You can always fade the house, that way you can easily tell where it is. (Set alpha to 80 or 90%)

Since you have divided your tutorial into sections, you could always use the Goto/Point tags. (Check out the vB code list to figure out how to use them)


Otherwise, it looks pretty good.

#### Tekal

#### Tekal

 Level 4

JoinedDec 14, 2007Messages40

Resources: 1

#### Resources
Tutorials 1

yes the decapitilization would be kool,i dont know how to do ...Goto tags and i dont know wat the vB code list is

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Include the DEMO map and I'll approve.

On a slightly note: you may find the use of Anchors to be quite useful since you start off with a "table of content".

#### Tekal

#### Tekal

 Level 4

JoinedDec 14, 2007Messages40

Resources: 1

#### Resources
Tutorials 1

i hope i still have it


EDIT:i dont D: i formatted. F YOU VIRUSES!!


 Last edited: Mar 21, 2008

#### Epic Producer

#### Epic Producer

 Level 5

JoinedApr 13, 2018Messages124

Where did u got the models . Cant seem to see any stairs or plank floors like that . I would appreciate it .

 You must log in or register to reply here.


### Similar threads

Locked

[JASS]
 JASS Crashing WE


Mech

Aug 26, 2007

Triggers & Scripts

Replies7Views1K

Aug 27, 2007
HINDYhat

Locked

[Trigger]
 House IN & OUT for multi-player


Weng

Sep 11, 2012

Triggers & Scripts

Replies5Views1K

Sep 15, 2012
mckill2009

 Basic Triggering


Ralle

Jan 3, 2006

Trigger (GUI) Editor Tutorials

Replies4Views39K

Jul 7, 2016
billel

Locked

[Trigger]
 Unit Transparency


NoobMapmaker

Sep 25, 2009

Triggers & Scripts

Replies2Views2K

Sep 25, 2009
NoobMapmaker

 Reviving Heroes at a Changeable Region


dOT145

Mar 23, 2010

Trigger (GUI) Editor Tutorials

Replies12Views10K

Apr 30, 2010
PurgeandFire

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