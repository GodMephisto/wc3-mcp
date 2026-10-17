---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/demystifying-hashtables.354000/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Demystifying Hashtables
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/demystifying-hashtables.354000/>
#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

Introduction

Hashtables are notoriously difficult to understand for those who have never worked with them, since they are an abstract concept that has a very unintuitive implementation in GUI. While there are already tutorials on using hashtables in GUI, I have the sense that the most difficult part about hashtables is not learning to work with them, but just understanding the basic concept; to have that first aha moment. Therefore, this tutorial will have a more long-winded approach in explaining the concept and only touch on the GUI-specifics briefly. For that, I will refer you to other tutorials at the end.

We will start our journey at Excel sheets (or OpenOffice tables, Google Docs tables etc.), then move on to Lua tables, since they're simpler than hashtables and much easier to understand, before we then finally arrive at GUI hashtables.

Excel Sheets

Meet Alice and Bob. They're a couple living in a small fishing town in northern Lordaeron.

For our RPG, we want to create a table containing background information about all the NPCs in our world. In an offline pen-and-paper RPG, we might achieve this with an Excel sheet that all players can pull up on their phone at any time. The columns represent our characters and the rows represent different attributes that we want to store.

Now, if we want to find out what Alice's occupation is, we just need to go to the Alice column and the occupation row and we will find the information there. Note that the order of our columns and our rows and even what we chose to be our columns vs. what we chose to be our rows is arbitrary. This table contains the same information as the one above:

This will be important later on.

Lua Tables

Tables in Lua are often praised as being one of its best features for how powerful and how easy to use they are. They are what is called a dictionary or associative array. In a regular array, like the ones we have in GUI, we can only use integers as the "keys" for our array. A key is a position at which data is stored. For example:

Set VariableSet NPCs[1] = Alice

Set VariableSet NPCs[2] = Bob
The [1] and [2] are the keys and Alice and Bob are the values of our array.

In Lua, we can use anything as keys for our array, including Alice and Bob. So, we can do something like this, where we use the unit variables udg_Alice and udg_Bob directly as the keys of our array:

 Lua:

Occupation[udg_Alice] = "Tailor"
Occupation[udg_Bob] = "Fisherman"

However, this table contains only the occupation, not age and spouse. To create a data table that contains all of the information, we need to create a two-dimensional table, much like our Excel sheet.

 Lua:

BackGroundInformation["Age"][udg_Alice] = 26
BackGroundInformation["Age"][udg_Bob] = 32
BackGroundInformation["Spouse"][udg_Alice] = udg_Bob
BackGroundInformation["Spouse"][udg_Bob] = udg_Alice
BackGroundInformation["Occupation"][udg_Alice] = "Tailor"
BackGroundInformation["Occupation"][udg_Bob] = "Fisherman"

Here, we have not one, but two keys. We use strings as the parent key (representing the rows of the Excel sheet), and the unit variables Alice and Bob as the child keys (representing the columns).

Because mapmakers do not have the option to use units directly as keys in arrays in GUI, the most common workaround has always been to use Unit Indexers. These systems use the Custom Value of units to assign a unique integer to each unit on the map. This integer is used as the key in the arrays to find where the data is stored for that unit. Going back to our Excel sheet example, in Lua, if we want to find Alice's occupation, we just tell our computer to find the column "Alice", while in GUI, we have to tell our computer specifically that information for Alice is stored at column C.

Hashtables

Unlike Lua tables, hashtables only take integers as keys, but we can still use hashtables to achieve exactly what we achieved with the Lua tables with one extra step.

First, we create our hashtable:

Hashtable - Create a hashtable

Set VariableSet BackgroundInformation = (Last created hashtable)
We want to store "Tailor" as Alice's occupation. We go to the GUI Action "Hashtable - Save String". This is what pops up:

This might as well be Chinese. Save Value as Value of Value?!?!?!? What the hell is that supposed to mean? No idea how that wording made it into the editor. It will be much clearer once we rephrase this as such:

Hashtable - Save Tailor as (Child Key) of (Parent Key) in BackgroundInformation.
What child keys and a parent keys are, we looked at earlier in our Lua example. And going all the way back to the Excel sheet, we can rephrase it further to:

Hashtable - Save Tailor at (Row), (Column) in BackgroundInformation.
The two additional values we need to provide for the Hashtable - Save String function therefore specify the column and row of the cell at which we want to store our string. As I noted earlier, the choice of columns and rows is completely arbitrary. It is only important that we are consistent so we don't store data in and read data from the wrong cell.

Because a hashtable only uses integer keys, we have to use an extra step that we didn't have to with Lua tables. To this end, we can use two functions:

Hashtable - Get Handle Id

This function converts any handle (unit, destructable, special effect, timer etc.) into a unique integer. It is the most important function when working with hashtables. It has, however, a few annoying quirks in GUI.

Hashtable - Get String Id

Much like GetHandleId, but converts a string into a unique integer.

With these functions, we can finally store the data in our hashtable for Alice:

Unit - Create 1 Villager (Female) for Neutral Passive at MyLocation facing Default building facing degrees

Set VariableSet Alice = (Last created unit)

Custom script: set udg_ParentKey = GetHandleId(udg_Alice)

Hashtable - Save Tailor as (Key Occupation.) of udg_ParentKey in BackgroundInformation.

Hashtable - Save 26 as (Key Age.) of udg_ParentKey in BackgroundInformation.

Hashtable - Save Handle Of Bob as of (Key Spouse.) of udg_ParentKey in BackgroundInformation.

That we need to use a Custom Script line to convert the Alice unit variable to an integer is one of the annoying quirks of GUI hashtables. But we can use event responses such as Triggering Unit and Last Created Unit without the need for custom script.

You might ask why we don't just use Get Handle Id and Get String Id to convert our handles into integers and use those as the keys in a plain-old regular array. That's because the keys of regular JASS arrays are limited to the range 0 to 32768, while the keys of hashtables can take any value. These two data structures are optimized for different purposes.

If we want to read the data from the table, we use the Hashtable - LoadString function:

Custom script: udg_ParentKey = GetHandleId(udg_Alice)

Set VariableSet Occupation = (Load (Key Occupation.) of udg_ParentKey from BackgroundInformation.)

Why are hashtables useful?

Everything we've done so far could just as easily be achieved by using regular arrays and a Unit Indexer. But with hashtables, we can not only attach data to units, but to any handle. Specifically, we can store data attached to a timer. This is incredibly useful when coding in JASS to make spells MUI. We can store a unit to the key of a timer, then, when the timer expires, we can retrieve that unit and perform our actions on it. However, to get the same benefit in GUI requires a few more hoops to jump through.

We can also store data attached to unit type keys, ability type keys etc. And finally, we can store values to string keys. Take this trigger for example:

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Entered chat string) Equal to -pizza

Then - Actions
Trigger - Run CreatePizza <gen> (ignoring conditions)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Entered chat string) Equal to -spaghetti

Then - Actions
Trigger - Run CreateSpaghetti <gen> (ignoring conditions)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Entered chat string) Equal to -burger

Then - Actions
Trigger - Run CreateBurger <gen> (ignoring conditions)

Else - Actions

This trigger is supposed to allow a player to enter into the chat which type of food he wants for his character and then executes the correct trigger to create that food. As you can imagine, as the number of food choices grows, this trigger can get very long and unhandy, and also quite slow. With hashtables, after storing the triggers somewhere during map initialization, we can condense this giant block of code into a single line:

Hashtable - Save Handle OfCreatePizza <gen> as 0 of (Key -pizza.) in FoodTriggers.

Hashtable - Save Handle OfCreateSpaghetti <gen> as 0 of (Key -spaghetti.) in FoodTriggers.

Hashtable - Save Handle OfCreateBurger <gen> as 0 of (Key -burger.) in FoodTriggers.

Trigger - Run (Load 0 of (Key (Entered chat string).) in FoodTriggers.) (ignoring conditions)
Because we don't need the child key for this problem, we just use 0 (you can also use 42 or -1000, it doesn't matter).

So far, we've only stored constants in a hashtable, but, of course, a hashtable can also be used to store dynamic data, for example the life left on a damage-absorbing shield buff that is active on a unit. To do this, we first load the current value from the hashtable, do our arithmetic, then write the modified value back into the same location in the hashtable.

Closing Thoughts

I hope I was successful in helping you wrap your head around hashtables. As you get more comfortable using hashtables, you will identify more and more problems that can be solved by using them - until you use them for everything!

For a more in-depth tutorial on how to work with hashtables in GUI, I refer you to A Complete Beginners Guide to Hashtables by Jazztastic and Hashtable and MUI by wyrmlord. As an exercise, you can try to create Alice and Bob in a map and write a function that prints out their background information when the player selects one of them.

P.S. Don't worry, Alice and Bob managed to flee by sea when the Scourge came.


 Last edited: Oct 11, 2025

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

While there are other hashtables tutorials around, but I feel like this is explained differently enough to where your approach might click with some people while the others would not.

For future readers I've always been fond of this one: Hashtables and MUI for a pure GUI user but I think your table visualization is really good.


 Last edited: May 29, 2024

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

Thanks for approving!

Chaosy said:

 For future readers I've always been fond of this one: Hashtables and MUI for a pure GUI user but I think your table visualization is really good.


Click to expand...

The tutorial you're mentioning is linked at the bottom .

#### Bain_Rebirth

#### Bain_Rebirth

 Level 10

JoinedOct 20, 2010Messages236

Resources: 1

#### Resources
Maps 1

Hey Antares! Appreciate this tutorial, though I've been using Hashtables for a while something I've found interesting is your mention of requiring the custom script to convert the handle. I have never done this nor run into an issue. I use a unit indexer and I save their custom value. Is what you are recommending only necessary if the mapmaker isn't using a unit indexer?

PS: Thank god Alice and Bob are ok.

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

Bain_Rebirth said:

 Hey Antares! Appreciate this tutorial, though I've been using Hashtables for a while something I've found interesting is your mention of requiring the custom script to convert the handle. I have never done this nor run into an issue. I use a unit indexer and I save their custom value. Is what you are recommending only necessary if the mapmaker isn't using a unit indexer?


Click to expand...

I'm not a GUI user, so I'm not an expert on the best practices therein. Using the index from a unit indexer as the key in a hashtable seems like a good way to get around the GUI restrictions. Just make sure that you don't access the wrong values when the unit indices get recycled because the old ones for that index are still present in the hashtable. I can add a sentence about that in the tutorial. Thanks for the suggestion!

 PS: Thank god Alice and Bob are ok.


Click to expand...

Damn scourge!

#### Retera

#### Retera

Banned

 Level 69

JoinedApr 19, 2008Messages2,951

Resources: 148

#### Resources
Models 91Icons 1Skins 8Tools 6Maps 14Templates 2Reforged HD Models 16Ported Models 3User Interface 1Sounds 5Tutorials 1

##
 Warsmash Mod Engine


Antares said:

 No idea how that wording made it into the editor.


Click to expand...

For what it's worth, I think this was added by Activision in a patch that was created in a hurry to fix a security vulnerability shortly after the Blizzard/Activision merger in 2009, and they were just adding script systems to match whatever Vexorian and friends asked for, or something like that. So, unlike the original Frozen Throne game, the focus was on making something that worked at all and not on a user friendly experience.

Like much of the scripting APIs added in Reforged, it was quite literally a hack added after-the-fact.

We can even see that the actual Frozen Throne API for saving game cache data has better naming, and says Value as Label of Category not Value as Value of Value:

#### Nichilus

#### Nichilus

 Level 34

JoinedSep 26, 2009Messages2,893

Resources: 2

#### Resources
Spells and Systems 2

I am not sure when this was introduced or if it was possible the entire time and just not noticed, but hashtables are not 2D arrays, they are 3D cubes, although the 3rd dimension is very limited.

The 3rd dimension is type - it is possible to store different types of values under same parent/child key. Only one value of given type can be stored in 3rd dimension.

The limitation is in exactly which types 3rd dimension supports. Only 5 types are supported: primitive types (integer, real, boolean, string) and handle.

Since all types in WC3 are handles (except for primitives) and only single handle can be stored in 3rd dimension at a time, this pretty much limits the usage.

Taking the Alice/Bob example, one can store and load Bob's person info like this:

Unit - Create 1 Villager (Male) for Neutral Passive at MyLocation facing Default building facing degrees

Set VariableSet Bob = (Last created unit)

-------- ---------------------------------------- --------

-------- Setup keys --------

Custom script: set udg_handleId = GetHandleId(udg_Bob)

Set VariableSet stringId = (Key PersonInfo.)

-------- ---------------------------------------- --------

-------- Store multiple data under same parent and child keys --------

Hashtable - Save 32 as stringId of handleId in BackgroundInformation.

Hashtable - Save Fisherman as stringId of handleId in BackgroundInformation.

Hashtable - Save Handle OfAlice as stringId of handleId in BackgroundInformation.

Hashtable - Save True as stringId of handleId in BackgroundInformation.

Hashtable - Save 92.40 as stringId of handleId in BackgroundInformation.

-------- ---------------------------------------- --------

-------- Load data into variables --------

Set VariableSet Age = (Load stringId of handleId from BackgroundInformation.) // this is integer variable

Set VariableSet Occupation = (Load stringId of handleId from BackgroundInformation.) // this is string variable

Set VariableSet Spouse = (Load stringId of handleId in BackgroundInformation.) // this is unit variable, unit is handle

Set VariableSet EnergyLevel = (Load stringId of handleId from BackgroundInformation.) // this is real variable

Set VariableSet HasChildren = (Load stringId of handleId from BackgroundInformation.) // this is boolean variable

-------- ---------------------------------------- --------

-------- Print values of variables --------

Game - Display to (All players) the text: (Age: + (String(Age)))

Game - Display to (All players) the text: (Occupation: + Occupation)

Game - Display to (All players) the text: (Spouse: + (Name of Spouse))

Game - Display to (All players) the text: (Energy level: + (String(EnergyLevel)))

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HasChildren Equal to True

Then - Actions
Game - Display to (All players) the text: Has children: True

Else - Actions
Game - Display to (All players) the text: Has children: False

The above will correctly print 32 for age, "Fisherman" for occupation, "Alice the Wife" for Spouse, 92.40 for energy level and "True" for has children.

In the below example I have added Bob's favorite weapon - the Searing Blade.

Unit - Create 1 Villager (Male) for Neutral Passive at MyLocation facing Default building facing degrees

Set VariableSet Bob = (Last created unit)

-------- ---------------------------------------- --------

-------- Setup keys --------

Custom script: set udg_handleId = GetHandleId(udg_Bob)

Set VariableSet stringId = (Key PersonInfo.)

-------- ---------------------------------------- --------

-------- Store multiple data under same parent and child keys --------

Hashtable - Save 32 as stringId of handleId in BackgroundInformation.

Hashtable - Save Fisherman as stringId of handleId in BackgroundInformation.

Hashtable - Save Handle OfAlice as stringId of handleId in BackgroundInformation.

Hashtable - Save True as stringId of handleId in BackgroundInformation.

Hashtable - Save 92.40 as stringId of handleId in BackgroundInformation.

Hashtable - Save Handle OfSearingBlade as stringId of handleId in BackgroundInformation.

-------- ---------------------------------------- --------

-------- Load data into variables --------

Set VariableSet Age = (Load stringId of handleId from BackgroundInformation.) // this is integer variable

Set VariableSet Occupation = (Load stringId of handleId from BackgroundInformation.) // this is string variable

Set VariableSet Spouse = (Load stringId of handleId in BackgroundInformation.) // this is unit variable, unit is handle

Set VariableSet EnergyLevel = (Load stringId of handleId from BackgroundInformation.) // this is real variable

Set VariableSet HasChildren = (Load stringId of handleId from BackgroundInformation.) // this is boolean variable

Set VariableSet FavoriteWeapon = (Load stringId of handleId in BackgroundInformation.) // this is item variable, item is handle

-------- ---------------------------------------- --------

-------- Print values of variables --------

Game - Display to (All players) the text: (Age: + (String(Age)))

Game - Display to (All players) the text: (Occupation: + Occupation)

Game - Display to (All players) the text: (Spouse: + (Name of Spouse))

Game - Display to (All players) the text: (Energy level: + (String(EnergyLevel)))

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HasChildren Equal to True

Then - Actions
Game - Display to (All players) the text: Has children: True

Else - Actions
Game - Display to (All players) the text: Has children: False

Game - Display to (All players) the text: (Favorite weapon: + (Name of FavoriteWeapon))

Since Searing Blade is an item and item is a handle, storing it will overwrite the other handle I have saved before it: Alice.

So when loading unit into "Spouse" variable and printing that unit's name, nothing will be loaded and no name will be printed.

However it will correctly load Bob's favorite weapon and print its name, as that was the last stored handle.

Similar limitations apply for example to storing unit-type, as that is just an integer under the hood, so it would overwrite what we stored as Age.

@Antares I think it might be worthwhile if you update your tutorial to describe how clearing values from hashtable works:

Hashtable - Clear some_hashtable

Hashtable - Clear all child hashtables of child some_key in some_hashtable.
And perhaps point out the importance of choosing what should be the parent key and what should be the child key as doing the following may be ok:

Custom script: set udg_handleId = GetHandleId(udg_Alice)

Hashtable - Save 26 as (Key Age.) of handleId in BackgroundInformation.

...

Hashtable - Clear all child hashtables of child handleId in BackgroundInformation.
But the below could be disastrous:

Custom script: set udg_handleId = GetHandleId(udg_Alice)

Hashtable - Save 26 as handleId of (Key Age.) in BackgroundInformation.

...

Hashtable - Clear all child hashtables of child (Key Age.) in BackgroundInformation.

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

While you can make use of the "third dimension" of hashtables, there isn't really any significant benefit to doing so, and I think doesn't need be included or be encouraged. Most people using hashtables are not aware of this functionality and aren't missing out on anything.

The difference between parent and child keys should be covered in the follow-up tutorials I linked at the end, I think. However, I just saw that I put the units as the child key in the examples. That's of course not how you should do it. I fixed that.

Thanks!

 You must log in or register to reply here.


### Similar threads

 Hashtables and MUI


wyrmlord

Jun 26, 2009

Trigger (GUI) Editor Tutorials
345

Replies245Views126K

Jul 29, 2022
Chaosy

 General Spell Creation


Chaosy

Jun 11, 2013

Trigger (GUI) Editor Tutorials

Replies35Views10K

Feb 8, 2014
PurgeandFire

 A Complete Beginners Guide to Hashtables


Jazztastic

Jun 15, 2011

Trigger (GUI) Editor Tutorials
2

Replies57Views45K

Nov 12, 2024
PurgeandFire

 How to create an artificial level-up system


frettory

Nov 22, 2012

Trigger (GUI) Editor Tutorials

Replies14Views9K

Mar 13, 2013
frettory

 Trackables


D4RK_G4ND4LF

Aug 25, 2010

Trigger (GUI) Editor Tutorials

Replies8Views11K

Feb 4, 2015
Tal'Rarity

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