---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/introducing-multinaming-hiding-inventory-of-enemies-more.307043/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Introducing Multinaming Hiding Inventory of Enemies More
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/introducing-multinaming-hiding-inventory-of-enemies-more.307043/>
#### Gaerik

#### Gaerik

 Level 2

JoinedJul 11, 2018Messages4

Resources: 1

#### Resources
Tutorials 1

INTRODUCTION

Long post incoming since I intend to make it as friendly as possible for learning mapmakers, but I'm trying to make it easier to read and shuffle through by dividing it into topics. This thread will explain how to use WorldEditor's new object-altering triggers to create multiple and subjective layers of string object data.

For applications, as a lone mind I cannot possibly even think of being exhaustive enough, therefore I'll only provide the example of hidden enemy inventories (a feature left to be desired by Blizzard since ticking off the Gameplay Constants value hides it for everyone, not just enemies).

Feel free to post ideas for more applications and ask me to list them here. I will, however, not make more demonstration maps.

BACKGROUND

I'm sure most (if not every single one) of you are aware of the recent patches' additions to World Editor. Personally, the fact that Blizzard is picking up Warcraft III again to update fills me with joy and hope. You wouldn't expect a megacompany to invest time and money in an old game without plans for the future, of course, and that is enough to fill my nights with dreams of a Remastered version, just like what happened to Starcraft.

Amongst the delicious JASS and vJASS changes, there was one addition that I think went partly unnoticed, one set of functions that even received the attention of GUI triggering as actions. That is, of course the ability to straightforward alter object data using triggers during the course of a game. This can be done, for example, by changing an Hero's name with "Hero - Set Hero Proper Name", or a unit's damage output with "Unit - Set Base Damage".

However, I'm here to explain that using the object editor through triggers as you play a map is only a portion of what those functions have to offer.

Also, please note that this is a new finding using new functions and, as such, it may be prone to bugs and error. Although I can claim with safety that I've tested it in all ways I could think of without encountering problems, one tester is never enough.

THE GetLocalPlayer() FUNCTION

For those of you familiar with JASS, you might've already figured out where I'm going with this just by looking at the topic's name. If you did, be my guest to skip it and jump right to the next one, where I go into more detail about how this MultiNaming thing works.

And for those that stayed, I request patience for this brief, reductionistic and pragmatic explanation: GetLocalPlayer() is a short JASS native that may be used to run separate sets of actions for different players in the same game.

If, for example, one lists GetLocalPlayer() == Player(0) as a condition for a trigger, it will only run for Player 1 (in JASS Player(0) means the first player). Note that I'm not saying it will run the trigger taking into consideration only Player 1, but that all its actions will only execute in Red/Player 1's instance of the game. So, in short, for the other players not detected as "Player 1" it would be as if that trigger never ran at all.

This, of course, is as dangerous as it looks, since running things differently for players can cause serious problems relating to disconnections and desyncs. If you command a script to create units only after checking if the local player is Red, for example, it will surely lead to desynchronization with the resulting gameplay-altering data unpairing (one local instance of the game will have more units than the others).

I will opt to not go into much detail as how to safely use GetLocalPlayer() for all intents and purposes, partly because I've already found threads devoted to the subject and it not being the goal of this post (its objective instead being the announcement of my findings & demonstrating how they may be applied), and partly because I myself am still doing research on the matter.

MultiNaming

Despite all the desync problems GetLocalPlayer() might cause when dealing with most handles and other dangerous stuff, it is relatively safe to use and may develop into a wonderful tool when employed to some non-agents, such as names and descriptions of some objects like abilities and items.

This is what I conveniently choose to call here MultiNaming - the technique of using the GetLocalPlayer() native to have objects carrying multiple names in different local instances of the same game session. As simple as it sounds, it may be used for a diversity of features, from simply having resources be available in multiple languages at the same time to making players not able to read the items an enemy possesses in his inventory.

In order to achieve this, you merely have to invoke the GetLocalPlayer() native as a condition and run the naming script matching the player whose local instance you want to change. In GUI, it can be done as simply as through the use of two Custom Script actions before and after the desired actions. For example:

Custom script: if GetLocalPlayer() == Player(8) then

Custom script: set udg_LocalPlayerName = GetPlayerName(GetLocalPlayer())

Game - Set Tooltip of UltimateAbility to LocalPlayerName for level 1

Custom script: endif
This will lead to Player 9 (again, Player 0 = 1) having the variable ability's tooltip changed to match the chosen player's name, but only for his particular instance of the game. The other players will see the same ability assigned to the UltimateAbility variable retain its default Object Editor tooltip. Since the number of agents in all of instances of the game remains the same (only thing that changed was descriptive text of the ability), no desyncs will be caused.

Here's an example of how this may be done to have the same exact ability, in the same exact game, display descriptions in English and Portuguese, for different players (even though there is no Portuguese WC3 client):

HIDDEN ENEMY INVENTORY

Warcraft III's World Editor has a Gameplay Constant called "Enemy Inventory Display" that may be ticked as True or False. However, setting the value as False will hide an hero's inventory from all other players, regardless if they are enemies or not.

This can be circumvented by using the MultiNaming technique herein introduced to change the name, description and iconpath of all items an enemy Hero picks up, restoring their original data only when they are dropped. This, of course, is done only to a player locally through the use of the GetLocalPlayer() native.

As I've mentioned in the WARNING section, doing this in version 1.29 of Warcraft III is dangerous and leads to all items of the same type being renamed. This can be shown in the following line of 1.30's bug correction changelog:

[World Editor] Renaming an Item no longer changes all Items’ names

Note that this could be done much easier by using the renaming functions whenever the enemy hero is selected instead. But sadly it doesn't work that way since the flag from unit selection EVENT_PLAYER_UNIT_SELECTED, takes too long to be recognized after a player selects an unit, giving him more than enough time to glance at the items. Now you wouldn't want a player to have 0,5 seconds to look at the items before they are masked, would you?

This can be avoided by using MultiNaming when a Hero picks/drops an item, and there are really only three things you need to change in order to hide an enemy's inventory: the item's name, its icon and its description. Those are the only pieces of information displayed when you select enemy Heroes to check their inventories - thus, changing just them is more than enough.

1. HIDING ITEM ON PICKUP

Ideally, you'd want to do this every time a Hero picks up an item, locally renaming an item's attributes for all players that Hero's owner has as an enemy. Sounds complex? Well, not quite. It may be done by simply setting a trigger up like this, in GUI:

Events
Unit - A unit Acquires an item

Conditions

Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Owner of (Hero manipulating item)) is an enemy of (Picked player)) Equal to True

Then - Actions
Custom script: if GetLocalPlayer() == GetEnumPlayer() then

Item - Set Name of (Item being manipulated) to Unknown Item

Item - Set Extended Tooltip of (Item being manipulated) to You do not know what item this is!

Item - Set Icon Path of (Item being manipulated) to ReplaceableTextures \CommandButtons\BTNSelectHeroOff.blp

Custom script: endif

Else - Actions

Again, you want this to be done locally to a player (not to everyone), which is why the trigger checks through a custom script (not available in GUI otherwise) if the local player is the player you want to hide the items from, thus only executing its actions for his game session.

Good, good, now whenever a player uses his units to pick up an item, it will be hidden for all those he sees as an enemy. But we're not done yet, now we want to make it so that this masked item of ours may be unmasked. This could be done at any time, but for now, let's say we want to unmask an item whenever an Hero drops it.

2. SHOWING ITEM UPON DROP

For this, we would need a way to access the item's data, since for the player it was hid from it would be permanently lost. We could create variables to store information such as an item's name and description everytime it is masked, but we can be smarter than that - instead, it would be easier to just create another item of the same type as the masked item, copy its data to the item to be unmasked, and then remove the item we just created.

We'd end up with a GUI trigger like this:

Events
Unit - A unit Loses an item

Conditions

Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Owner of (Hero manipulating item)) is an enemy of (Picked player)) Equal to True

Then - Actions
Item - Create (Item-type of (Item being manipulated)) at ((Center of (Playable map area)) offset by (99999.00, 99999.00))

Item - Hide (Last created item)

Custom script: if GetLocalPlayer() == udg_PlayerToMask then

Item - Set Name of (Item being manipulated) to (Name of (Last created item))

Item - Set Description of (Item being manipulated) to (Description of (Last created item))

Item - Set Icon Path of (Item being manipulated) to (Icon of (Last created item))

Custom script: endif

Item - Remove (Last created item)

Else - Actions

That's it, done! The item will now be masked whenever an enemy picks it up, and unmasked whenever it is dropped. Note that in the trigger we create and remove the item outside the GetLocalPlayer() scope. If we were to put it inside, it would cause us to create or remove an item only in the local player's instance of the game, obviously causing a desync.

If you don't want to run a trigger every time an item is picked up, you may set a condition to only fire it whenever the item being picked up is a masked item. But beware! Since the trigger deals with Creating/Removing items, you'd need to make sure the status of an item as masked or unmasked is tracked globally, so that the above trigger would still fire for all players - thus avoiding desyncs. In order to not make this tutorial longer than it already intends to be, I will not be thorough in explaining how to achieve this. Instead, I'll upload a map containing an example of how it may be done using the same item's tooltip (useless data for our purposes, since it is only displayed when the item in question is being viewed in a shop).

3. THE CHARGED ITEMS ISSUE

Hold up, though! Before you go on using this in your maps, there is one small obstacle to overcome. Here, see this?

This happens when an item with charges is picked up. Now, depending on the kind of map you're making, this may not be a problem. If it is, though, don't worry, there's an easy solution available, and that is because you can also locally change an item's number of charges without leading to desync. So, you merely have to return to the trigger you just made for whenever a player picks up an item and insert the following action into the GetLocalPlayer() scope:

Item - Set charges remaining in (Item being manipulated) to 0
With this, the information regarding the item's number of charges can be easily hidden from the display, leaving a nude red interrogation mark just like any other masked item.

But that's not all! You see, by doing this we create another problem: in Warcraft III, whenever a charged item with only one remaining charge is used, it is removed from the game. This won't happen automatically to the player we just hid the charged item from - so, if an enemy Hero uses an item with a single charge making it go away, it'll still exist for our local player, causing a serious desync problem!

So, in order to fix that we need to globally and constantly store information relative to an item's charge. In the demonstration map I uploaded here, I do that by assigning that information to the item's custom value, and making it so an item is removed from the game if its custom value ever reaches zero (therefore balancing the number of agents in each local instance of the game before a desync can occur).

In order to achieve this in our example, simply go tothe trigger we've just made that fires whenever an Hero picks up an item and, as its first and foremost action (also before the start of the GetLocalPlayer() scope), have a function to set its Custom Value to its charges:

Item - Set the custom value of (Item being manipulated) to (Charges remaining in (Item being manipulated))
Then, before the end of the GetLocalPlayer() scope of the second trigger we've made to fire when a Hero drops an item, we change our item's remaining stacks based on its Custom Value:

Item - Set charges remaining in (Item being manipulated) to (Custom value of (Item being manipulated))
Now, an item's number of charges will be masked and unmasked when picked and dropped by an enemy Hero, respectively. But remember, we still have to make it so that the item's Custom Value (in this example the only way for our local player to keep track of charges without visually seeing them) is constantly updated whenever that item is used. For this, we may create a third and final trigger:

Events
Unit - A unit Uses an item

Conditions
Conditions
(Item-class of (Item being manipulated)) Equal to Charged

(Item-class of (Item being manipulated)) Equal to Purchasable

Actions
Item - Set the custom value of (Item being manipulated) to ((Custom value of (Item being manipulated)) - 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Custom value of (Item being manipulated)) Less than or equal to 0

Then - Actions
Item - Remove (Item being manipulated)

Else - Actions

This little effort will make sure to remove a "charge" from a masked item whenever it is used, and, as explained, to destroy it if the "charge counter" ever reaches zero, saving our players from a desync issue. Note that the above trigger checks if the item is charged or puchasable before proceeding. This is done to avoid permanent items with abilities being removed after an use (so if your map deals with custom items don't forget to take the necessary precautions against them being mistaken by a trigger like this).

4. CONCLUSION

With all of that, you're up and set to go with your custom-made hidden inventory! Should you wish to apply this only to determined items, to a few chosen heroes or to some allies as well, you can easily do this by adding more conditions to the first two triggers I've exposed. Of course, since there are numerous ways to adapt the Hidden Enemy Inventory example herein demonstrated, I leave this homework to yourself, dear reader.

Finally, in case your map possesses dynamic alliances, make sure to make an additional trigger to hide all items currently held by Heroes for all enemies whenever allies are changed. There's a viewer-friendly GUI functionality for this in the test map I have attached to this thread, if you want to check it out for yourself.

Hope you've all enjoyed this little trick with GetLocalPlayer() and the renaming functions recently added to Warcraft III's World Editor. For any feedback or clarifications, make sure to leave a comment here.

Good luck, have fun, and enjoy the test map!

#### Attachments

MultiNamingTest - Hidden Enemy Inventory.w3x

 47.8 KB

 · Views: 527




 Last edited: Aug 19, 2018

#### nedio95

#### nedio95

 Level 12

JoinedMar 24, 2011Messages1,081

Resources: 1

#### Resources
Spells 1

Nice tutorial, I like it, explains the problem and solutions pretty well and simple to understand and I don't think that there has been one on this topic as that is pretty new.

Has good amount of screenshots and code snippets.

And, also, this new functionality is pretty promising, just have to be creative enough to apply it.

Things to look at (Nit-picking):

Warning, Introduction, Disclaimer, & Background - Those two feels like an off-topic. More like an explanation about yourself than the tutorial, may have been safely moved to Post#1 purely for information. What the first few words of a tutorial should be is a few quick words on what the reader is expected to learn. And maybe a content table ? Content table would be awesome, right on top, quick glance at it and you know what to expect from the read. The warning can be safely moved to the Topic Title.

MultiNaming screenshots - Those have a bit too much noise to sound ratio. The one under "The Cahrged Item Issue" is a lot better, shows only the information that is described and the reader needs to know.

Just to confirm - Have you tested all of this in multiplayer ? With 2-3-4 players all changing names and skills tt and picking and dropping items. What about exchanging items, Hero > Hero instead of Hero > Ground > Hero ?

PS: Posting on behalf of somebody else is against the rules... No idea how this is being enforced exactly...

ref: Site Rules

regards

-Ned

#### Gaerik

#### Gaerik

 Level 2

JoinedJul 11, 2018Messages4

Resources: 1

#### Resources
Tutorials 1

nedio95 said:

 Nice tutorial, I like it, explains the problem and solutions pretty well and simple to understand and I don't think that there has been one on this topic as that is pretty new.

Has good amount of screenshots and code snippets.

And, also, this new functionality is pretty promising, just have to be creative enough to apply it.

Things to look at (Nit-picking):

Warning, Introduction, Disclaimer, & Background - Those two feels like an off-topic. More like an explanation about yourself than the tutorial, may have been safely moved to Post#1 purely for information. What the first few words of a tutorial should be is a few quick words on what the reader is expected to learn. And maybe a content table ? Content table would be awesome, right on top, quick glance at it and you know what to expect from the read. The warning can be safely moved to the Topic Title.

MultiNaming screenshots - Those have a bit too much noise to sound ratio. The one under "The Cahrged Item Issue" is a lot better, shows only the information that is described and the reader needs to know.

Just to confirm - Have you tested all of this in multiplayer ? With 2-3-4 players all changing names and skills tt and picking and dropping items. What about exchanging items, Hero > Hero instead of Hero > Ground > Hero ?

PS: Posting on behalf of somebody else is against the rules... No idea how this is being enforced exactly...

ref: Site Rules

regards

-Ned


Click to expand...

Thanks for the reply, Ned!

I see your point. I'm going to hide the Disclaimer in a spoiler since it is pretty off-topic and has nothing to do with the tutorial itself. I think I should probably keep the Introduction to explain what the reader is about to learn and the Background to explain the new features, though. I'll keep the WARNING section at least until W3's stable version is updated.

I don't know how to do content tables, but I suppose I'll shuffle the forums for something similar so that I can copy.

I've kept the high noise to sound ratio simply so that the viewer could see that all those screenshots are part of the same game session, except that they were taken by different players. In the first image, for example, the first screenshot was taken by Player 2 (you can see this due to the hero icon), and the second was taken by Player 1.

I have tried multiplayer with 2 players, testing Hero > Ground > Hero, Hero > Hero and even Hero > Shop and Shop > Hero (for any gold issues). When I first started editing in functions there were plenty of desync problems but none of these were encountered so far in the current setup for the test map I've uploaded.
PS: Yeah, I learned that after some of the maps friends posted on my behalf were deleted from this site (so I stopped asking them to upload content). Didn't think too much of it, though.


 Last edited: Jul 12, 2018

#### nedio95

#### nedio95

 Level 12

JoinedMar 24, 2011Messages1,081

Resources: 1

#### Resources
Spells 1

@Gaerik On point 3, you could pop up your pain real quick and add some red circle to point the reader to the correct part of the image where they should focus their attention

About table of content, it could be just a bullet points like:

introductions

Hiding item on pick up

Revealing item on drop

Issues with charged items

Conclusions

Never forget, that most of the time the people who are reading those tutorials, and who need them the most, have little to none experience with WE.

PS: I don't thin that is particularly big issue as many people are doing it... kinda... without hiding at all.

regards

-Ned

#### disruptive_

#### disruptive_

 Level 8

JoinedMar 19, 2017Messages248

Nice input. When the new functions come out i was trying to make use of GetLocalPlayer blocks on item strings, but the current patch (non test) has those functions bugged, so i quitted madly.

I don't remeber i was using player enumerations tho. In the acquires item event shouldn't it be just "if IsUnitEnemy(GetManipulatingUnit(), GetLocalPlayer())"?.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

1.30 is out, might want to change the head note.

#### Gaerik

#### Gaerik

 Level 2

JoinedJul 11, 2018Messages4

Resources: 1

#### Resources
Tutorials 1

disruptive_ said:

 Nice input. When the new functions come out i was trying to make use of GetLocalPlayer blocks on item strings, but the current patch (non test) has those functions bugged, so i quitted madly.

I don't remeber i was using player enumerations tho. In the acquires item event shouldn't it be just "if IsUnitEnemy(GetManipulatingUnit(), GetLocalPlayer())"?.


Click to expand...

Could be done that way, although I'm keeping custom codes to the minimum possible in order to make it easier for unexperienced mapmakers to understand the function better.

Daffa the Mage said:

 1.30 is out, might want to change the head note.


Click to expand...

Just removed it, thanks!!

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

Did you tried it in multiplayer? Because Ive heard those SetTooltips have to be synced for no real reason

#### Gaerik

#### Gaerik

 Level 2

JoinedJul 11, 2018Messages4

Resources: 1

#### Resources
Tutorials 1

DracoL1ch said:

 Did you tried it in multiplayer? Because Ive heard those SetTooltips have to be synced for no real reason


Click to expand...

Yes. All of this guide was made after multiplayer testing with 2-3 players.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I like the outcome of this one.

However, the tutorial seems like a brick wall and very unfriendly to go through.

The big images should also be put into hidden tags.

Although it's only a question of restructuring, author is long gone so no updates will happen.

I am willing to make minor updates myself but this is way above my "paygrade".

So yeah.

@Gaerik poke me or Purge if you get back with intent to update this.

 You must log in or register to reply here.


### Similar threads

 Changeable Inventory


Lancer1111

Jun 5, 2009

Tutorial Archive

Replies27Views5K

Jun 23, 2009
Eleandor

 Backpack Inventory System for Rpg (Eazy, Simple, Compatible)


Master Zul'khan

Feb 11, 2023

Tutorial Archive

Replies9Views3K

Jun 13, 2024
Chaosy

Locked

 Inventory System


Koba10s

Mar 22, 2018

World Editor Help Zone

Replies1Views2K

Mar 22, 2018
apsyll

Locked

[Trigger]
 Inventory System Drop Check


SuperCoke

Jun 19, 2012

Triggers & Scripts

Replies3Views1K

Jun 21, 2012
SuperCoke

Locked

 Discovery: Hidden Inventory


Hoernchen

Jan 11, 2009

Triggers & Scripts

Replies2Views2K

Jan 11, 2009
-Berz-

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Tutorial Archive

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top