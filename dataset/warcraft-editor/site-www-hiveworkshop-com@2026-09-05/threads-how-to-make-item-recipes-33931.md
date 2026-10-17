---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-make-item-recipes.33931/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Make Item Recipes
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-make-item-recipes.33931/>
#### Tooolbox

#### Tooolbox

 Level 5

JoinedJun 23, 2004Messages126

Resources: 2

#### Resources
Maps 1Tutorials 1

How To Make Item Recipes


In this tutorial I'm gonna go over the various possible recipe styles and how they are accomplished. If you've ever played DotA you'll note that there are 2 kinds of recipes: Manual Combine and Auto Combine. I'll cover both, and even a little extra.

A note on terminology: I will refer to Items that are a part of a recipe as "factors".

Another note: I have never looked at the DotA triggers, so I do not know if my methods are their methods.


Since DotA is such a well-known map, I figured that it would be a good idea to use their recipes as examples, since it is a stable platform that many can relate to.

Manual Combine Theory


The concept of a Manual Combine involves a Recipe Factor and any number of Ingredient Factors. For example, in making the Magic Sword you might have to have the Ingredients Sword and Magic Dust, as well as the Recipe Magic Sword.


Manual Combine is only used when the Recipe actually costs anything. You buy the Sword for 500g, the Magic Dust for 250g and the Magic Sword recipe for 250g, and they all fuse together to create the Magic Sword end result. In a case where the Recipe costs nothing, use Auto-Combine.

An example from DotA of a Manual Recipe, with the Factors. 2 Ingredients and the Recipe make the whole: Belt + Axe + Recipe = Sword.

Auto Combine Theory


The idea of Auto Combine uses any number of Ingredient Factors, but no Recipe Factor. As in DotA, it is best to have all Recipe Factors visible in a shop with a description of the ingredients so players can compile them, and this holds true for Auto-Combine recipes, except they should not be purchase-able since they are useless (in DotA they cost wood, which players never get).


Using the same example of the Magic Sword, if it were auto-combine, you need only collect the Factors (Sword and Magic Dust) and they would automatically fuse without a recipe. This means that there is no extra charge beyond the cost of the ingredients. An Auto-Combine item typically forms instantly whenever the Hero is carrying all of the ingredients. If you need to get a Recipe, even if it's free, it's technically not an auto-combiner.

An example from DotA of an Automatic Recipe, with the Factors. 2 Ingredients combine without an actual Recipe Factor to make the whole. Ring + Pendant = Sphere.

Manual vs. Auto Combine


Combining items can be a serious issue of balance, but luckily there are so very many ways to tweak it. When you combine two or more items together into a single item that holds the same stats, you are essentially freeing up inventory space. This is quite an asset in any type of Hero-based map, such as AoS or RPG. It's up to you to put a price tag on how much the extra inventory space is worth, and make the Recipe Factor cost that much.


There are a number of cases where you don't want to add any extra cost on, such as if the Product Item actually has (somewhat) less stats than the Ingredient Factors put together, or has a different effect. Also, if the number of Ingredients is large (say, 3+) the toll-taking effect of carrying around that many partially-useful items in your inventory might balance it out. Lastly, if some of the Ingredients do nothing on their own (maybe like Magic Dust in our example) you would not add price for the extra inventory slots gained by freeing up the spots where the useless items were. You would only add cost for the bonus given by that item.


(In the example, let's say Sword gives +10 damage and Magic Dust gives nothing. But put together, Magic Sword gives +10 damage and a Poison attack. You don't add anything for the inventory slots "freed" by combo-ing in Magic Dust, simply set the price of the stuff to however much a Poison attack is worth.)


(Just to demo the other end of the spectrum, if Sword gives +10 damage and Magic Dust gives Poison attack, and Magic Sword gives +10 damage and Poison attack...then you make Magic Dust cost the price of Poison attack, and the Magic Sword Recipe cost X gold, where X is the price you determine it's worth to free up an inventory slot by combining the two items.)

Manual Combine Setup


To create a Manual Combine recipe, the items you need are the Ingredient Factors, (once again, Sword and Magic Dust) a Mask Recipe, a Scroll Recipe, (made up terms, use whatever you like) and finally the actual item.


You may have noticed in DotA that in the shops, all of the recipes look like their component items. Yet when they are purchased without the sufficient Factors, they look like Scrolls in your inventory. I call the shop recipe the "Mask" and the inventory recipe the "Scroll". They are actually identical in every respect, including cost, except that one has the icon of the end product item and the other one has a scroll icon.

An example from DotA of the "mask recipe", while in the shop. Looks like the Finished Item so that different recipes in the shop can be distinguished easily, and has a particular gold cost.

An example from DotA of the "scroll recipe", while in an inventory. Has a Scroll Icon to prevent the carrier for mistaking it for the actual item, and has the same gold cost as the "mask recipe" so that it can be sold back properly, if the owner doesn't end up using it.


The simple way this is triggered is that when you buy the Mask Recipe from the shop, it is instantly replaced with the Scroll Recipe version. The Scroll Recipe version is what is handled and checked for and whatnot in all of your Recipe Triggers. Since the prices of the two are the same, you can sell back the Scroll Recipe just as you would have the Mask Recipe. This method adds more baggage, but is simply more aesthetically pleasing and not confusing, either. (If you make the recipe look just like a Scroll, it will be hard to instantly see what you want in the shop. If you make the recipe look like the item in your inventory, it can confuse you into thinking you have the item.)


Then, when all the ingredients are put together and the Recipe is activated, remove it all and put in the end product item. I did consider simply having the Recipe be the end product and removing it if you did not have all the ingredients, but I realized that the sell-back price of the item would be half of the cost of the Recipe Factor, without taking into account the cost of the Ingredient Factors. So this is the best method.

An example from DotA of the End Product of the recipe. Note that the gold cost totals the cost of the entire recipe (give or take a tad) so that it can be sold back properly, and you are returned money for all of the ingredients.

Auto Combine Setup


Auto Combine setup is easier, you simply need the Mask Recipe since there's no Scroll Recipe to be had. When they get all the Ingredient Factors in their inventory, remove and add the End Product item, voila.


To make the Mask Recipe un-buyable, your best bet is to make it cost something the player doesn't have. The "cleanest" method is to make it cost (one) wood, but depending on your map that might not be feasible. Alternately, you could make it cost an impractical amount of gold, and if you wanted to be safe, set up a trigger so that whenever someone bought one of the Auto-Combine Mask Recipes, give 'em their money back and remove it.

An example from DotA of the mask recipe of an Auto-Combine item, while in the shop. Looks like the Finished Item so that different recipes in the shop can be distinguished easily, and has no cost (they have special methods in DotA to do this, it actually costs lumber but the lumber icon has been toyed with).

An example from DotA of the End Product of an Auto-Combine recipe.

Manual Combine Triggers


Now we're getting to the meat of the matter. How do you actually do it? There are many specific methods and techniques and variations, all depending on what you really want. I'm not going to explore them all-that would take forever. Instead, I'm going to give you an example. This is one way a Manual Combine recipe could be done (and probably the way I would do it).


Mask2Scroll: Replacing the Mask Recipe with the Scroll Recipe.

GoP Mask2Scroll
Events
Unit - A unit Acquires an item

Conditions
(Item-type of (Item being manipulated)) Equal to Gloves of Power (Mask Recipe)

Actions
Item - Remove (Item being manipulated)

Hero - Create Recipe: Gloves of Power and give it to (Hero manipulating item)

Combine: Combining the items.

GoP Combine
Events
Unit - A unit Acquires an item

Conditions
Or - Any (Conditions) are true
Conditions
(Item-type of (Item being manipulated)) Equal to Gauntlets of Ogre Strength +3

(Item-type of (Item being manipulated)) Equal to Gloves of Haste

(Item-type of (Item being manipulated)) Equal to Recipe: Gloves of Power

((Hero manipulating item) has an item of type Gauntlets of Ogre Strength +3) Equal to True

((Hero manipulating item) has an item of type Gloves of Haste) Equal to True

((Hero manipulating item) has an item of type Recipe: Gloves of Power) Equal to True

Actions
Set Ingredients = 3

Set RecipeFactor[1] = Gauntlets of Ogre Strength +3

Set RecipeFactor[2] = Gloves of Haste

Set RecipeFactor[3] = Recipe: Gloves of Power

For each (Integer A) from 1 to Ingredients, do (Actions)
Loop - Actions
Set FactorCleared[(Integer A)] = False

For each (Integer A) from 1 to 6, do (Actions)
Loop - Actions
For each (Integer B) from 1 to Ingredients, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item-type of (Item carried by (Hero manipulating item) in slot (Integer A))) Equal to RecipeFactor[(Integer B)]

FactorCleared[(Integer B)] Equal to False

Then - Actions
Item - Remove (Item carried by (Hero manipulating item) in slot (Integer A))

Set FactorCleared[(Integer B)] = True

Else - Actions

Hero - Create Gloves of Power and give it to (Hero manipulating item)

Here we have the Gloves of Power with the Ingredient Factors of Gauntlets of Ogre Strength +3 & Gloves of Haste and a Recipe Factor.


You may note how I did a check for each type of item at the beginning of the trigger. It might be a good idea if you are going to do a lot of these to somehow assign them to array variables instead of checking each one specifically. There are several ways to do this, but that's a variation that's up to you.

Auto Combine Triggers


Auto-Combine in terms of triggers is almost identical to Manual Combine; Auto-Combine just doesn't need the Mask2Scroll trigger. Here is my example of an Auto-Combine trigger:

RoIS Combine
Events
Unit - A unit Acquires an item

Conditions
Or - Any (Conditions) are true
Conditions
(Item-type of (Item being manipulated)) Equal to Ring of Protection +2

(Item-type of (Item being manipulated)) Equal to Ring of Regeneration

((Hero manipulating item) has an item of type Ring of Protection +2) Equal to True

((Hero manipulating item) has an item of type Ring of Regeneration) Equal to True

Actions
Set Ingredients = 2

Set RecipeFactor[1] = Ring of Protection +2

Set RecipeFactor[2] = Ring of Regeneration

For each (Integer A) from 1 to Ingredients, do (Actions)
Loop - Actions
Set FactorCleared[(Integer A)] = False

For each (Integer A) from 1 to 6, do (Actions)
Loop - Actions
For each (Integer B) from 1 to Ingredients, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item-type of (Item carried by (Hero manipulating item) in slot (Integer A))) Equal to RecipeFactor[(Integer B)]

FactorCleared[(Integer B)] Equal to False

Then - Actions
Item - Remove (Item carried by (Hero manipulating item) in slot (Integer A))

Set FactorCleared[(Integer B)] = True

Else - Actions

Hero - Create Ring of Inner Strength and give it to (Hero manipulating item)

I reiterate that this is almost identical to the above Manual Combine trigger, it simply has one less ingredient though.


Now that we have a second trigger you can note some contrasts. I use the Ingredients integer variable and the RecipeFactor Item-Type Array variable to go along with For Loops and simply streamline the process and make it more efficient. You can take this to the next level as I mentioned earlier by saving all the data to variables in an Initialization trigger, and simply reading off those for everything, including the Trigger's Conditions.


In closing, I'd like to refer you to Diablo-dk's Recipe System that is here at Hive itself, in case you don't feel like triggering your own unique system.


Even if you use a premade system and don't design your own I hope you at least take away from this tutorial a good concept of the balance issues surrounding Item Recipes.


And for any of those that want to see the Demo Map I created for this tutorial, here it is: View attachment Item Recipes.w3x

GST_Nemisis said:

 toolbox describes the theory of item recipies very well but the triggers he shows are bugged and not effective in all situations. i would suggest using these two triggers:

Razorbrain said:

Combination
Events
Unit - A unit Aquires an item

Conditions
((Triggering unit) has an item of type Item1) equal to True

((Triggering unit) has an item of type Item2) equal to True

((Triggering unit) has an item of type Recipe) equal to True

Actions
Item - Remove (Item carried by (Triggering unit) of type Item1)

Item - Remove (Item carried by (Triggering unit) of type Item2)

Item - Remove (Item carried by (Triggering unit) of type Recipe)

Hero - Create Weapon and give it to (Triggering unit)

Click to expand...


this one by razorbrain can be used to automatically create an item when a unit picks up two items of a different type. with this trigger you must also have a recipie item, but if you remove the lines "((Triggering unit) has an item of type Recipe) equal to True" and "Item - Remove (Item carried by (Triggering unit) of type Recipe)" it will do it when the unit automatically when the unit aquires the items.


and:

GST_Nemisis said:

Untitled Trigger 001
Events
Unit - A unit Acquires an item

Conditions
(Item-type of (Item being manipulated)) Equal to Claws of Attack +15

Actions
For each (Integer A) from 1 to 6, do (If ((Item-type of (Item carried by (Hero manipulating item) in slot (Integer A))) Equal to Claws of Attack +15) then do (Set ItemAmount = (ItemAmount + 1)) else do (Do nothing))

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ItemAmount Equal to 2

Then - Actions
Hero - Drop (Item carried by (Hero manipulating item) of type Claws of Attack +15) from (Hero manipulating item)

Item - Remove (Last dropped item)

Hero - Drop (Item carried by (Hero manipulating item) of type Claws of Attack +15) from (Hero manipulating item)

Item - Remove (Last dropped item)

Hero - Create Crown of Kings +5 and give it to (Hero manipulating item)

Else - Actions
Set ItemAmount = 0

Click to expand...


this can be used to turn two items of the same type into another item (for example Claws of Attack +15 + Claws of Attack +15 = Crown of Kings +5). this happens automatically but by adding the condition and action mentioned above you can make it require a recipie item. for this you need the integer variable ItemAmount.


Click to expand...


 Last edited by a moderator: Feb 6, 2008

#### paskovich

#### paskovich

 Level 11

JoinedJul 12, 2005Messages764

Resources: 3

#### Resources
Maps 1Tutorials 2

This can be useful, BUT it does not cover how to make recipies. I mean the trigger part. If you don't want to complicate this tutorial with long triggers, you can attach a link to Diablo-dk's recipie system which is a child's play to implement and use, however, it is written in jass.

http://hiveworkshop.com/resources_new/spells/940/

#### Tooolbox

#### Tooolbox

 Level 5

JoinedJun 23, 2004Messages126

Resources: 2

#### Resources
Maps 1Tutorials 1

So far I have covered the theory of Recipes, I do plan on covering the triggers with my own set, given both as examples and in a map.

I may mention Diablo's recipe system as well, though.

Edit: Props to Wolverabid for showing me the ATTACH tag!

Bump: Triggers and map added. Would appreciate feedback or suggestions for more content. If there are none, this tutorial is done.

#### paskovich

#### paskovich

 Level 11

JoinedJul 12, 2005Messages764

Resources: 3

#### Resources
Maps 1Tutorials 2

Perfect!  Good job!


Only one thing:

 In closing, I'd like to refer you to Diablo-dk's Recipe System ... in case you don't feel like doing your own recipes.


Click to expand...

This may be confusing.. You CAN make your own recipes with that system. Perhaps, you should put a "trigger" or "system" word after "your own recipes".


-> "in case you don't feel like doing your own recipe triggers".

#### F4talWr4th

#### F4talWr4th

 Level 6

JoinedAug 5, 2006Messages126

Resources: 2

#### Resources
Maps 1Tutorials 1

Yes, useful but some people don't want their recipes to be combined automatically and it is possible by using an ability bar of course.

#### Tooolbox

#### Tooolbox

 Level 5

JoinedJun 23, 2004Messages126

Resources: 2

#### Resources
Maps 1Tutorials 1

@ Paskovich, Thanks!  I fixed the wording in regards to Diablo-dk's system.

@ FatalWrath, People don't want their recipes to be combined automatically? I went over both Manual Combine and Auto-Combine. Are you referring to something I'm going to dub Ability-Combine, such as having all the ingredients and then using an ability to fuse them? If you think it's really that important I may insert a how-to in that department, but it would be good if you could be more specific as to what you mean.

#### Diablo-dk

#### Diablo-dk

 Level 10

JoinedNov 10, 2004Messages351

Resources: 3

#### Resources
Maps 1Spells and Systems 1Tutorials 1

For the best RPG recipe system use http://www.wc3campaigns.net/showthread.php?t=87309 though this does require the newest version of CSCache(Requires jasshelper)

Also, you may want to fix this:

when you create a recipe with 2 or more of the same item type it will bug.

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

maybe just like this?

Combination
Events
Unit - A unit Aquires an item

Conditions
((Triggering unit) has an item of type Item1) equal to True

((Triggering unit) has an item of type Item2) equal to True

((Triggering unit) has an item of type Recipe) equal to True

Actions
Item - Remove (Item carried by (Triggering unit) of type Item1)

Item - Remove (Item carried by (Triggering unit) of type Item2)

Item - Remove (Item carried by (Triggering unit) of type Recipe)

Hero - Create Weapon and give it to (Triggering unit)

#### GST_Nemisis

#### GST_Nemisis

 Level 21

JoinedJan 5, 2005Messages3,515

Resources: 1

#### Resources
Tutorials 1

Chronos said:

 Great that enough simple, but i am thinking one thing of those trigger


how to make recipe like:


first item: fire


second item: fire


item that comes when two of those are merged: Super fire


but when i tried that it made stupid thingy when i got one fire it cam to super fire. How i make it to require two fires?


Click to expand...


famouspker had this problem yesterday so i made this for him:

Untitled Trigger 001
Events
Unit - A unit Acquires an item

Conditions
(Item-type of (Item being manipulated)) Equal to Claws of Attack +15

Actions
For each (Integer A) from 1 to 6, do (If ((Item-type of (Item carried by (Hero manipulating item) in slot (Integer A))) Equal to Claws of Attack +15) then do (Set ItemAmount[(Player number of (Owner of (Hero manipulating item)))] = (ItemAmount[(Player number of (Owner of (Hero manipulating item)

Game - Display to (All players) the text: (String(ItemAmount[1]))

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ItemAmount[(Player number of (Owner of (Hero manipulating item)))] Equal to 2

Then - Actions
Hero - Drop (Item carried by (Hero manipulating item) of type Claws of Attack +15) from (Hero manipulating item)

Item - Remove (Last dropped item)

Hero - Drop (Item carried by (Hero manipulating item) of type Claws of Attack +15) from (Hero manipulating item)

Item - Remove (Last dropped item)

Hero - Create Crown of Kings +5 and give it to (Hero manipulating item)

Else - Actions
Set ItemAmount[(Player number of (Owner of (Hero manipulating item)))] = 0

it uses claws of attack +15 and makes crown of kings +5 when you aquire two of them. it uses the variable ItemAmount which will need to be an array for each item and you will need ItemAmount1 and ItemAmount2 ect for all your players. i came up with this off the top of my head in about 5 minutes, so there may be a better solution but this still works.

#### l3lack-Knight

L

#### l3lack-Knight

Oh man whats with all those commands it will take forever to make a map with many items if you make it that hard to combine the items ' I made them in a easier way and it works just fine :


GoP Combine


 Events


 Unit - A unit Acquires an item


 Conditions


 (Item-type of (Item being manipulated)) Equal to Gauntlets of Ogre Strength +3


 (Item-type of (Item being manipulated)) Equal to Gloves of Haste


 (Item-type of (Item being manipulated)) Equal to Recipe: Gloves of Power


 Actions


If (All Conditions are True) then do (Then Actions) else do (Else Actions)


If - Conditions


 ((Hero manipulating item) has an item of type Gauntlets of Ogre Strength +3) Equal to True


 ((Hero manipulating item) has an item of type Gloves of Haste) Equal to True


 ((Hero manipulating item) has an item of type Recipe: Gloves of Power) Equal to True


 Then - Actions


 Item - Remove (Item carried by (Hero manipulating item) of type Gauntlets of Ogre Strength +3 >>........ you do the same thing 4 the recipe and any other items / or if you dont got a recipe simply don't add it at all , both in the conditions and events <<


 Hero - Create Gloves of Power >> the real item that works not the recipe << and give it to (Hero manipulating item)


>> then if you wish add a sound ( example : Sound Play Tomes like in DotA and a special effect like a Lightning Bolt of the Level Up SFX to make it like DotA )


The items combine just as well like TOOLBOX made them but i think that his way will take longer BTW i astually made my item combinations 4 my map like this :


 Sword of Sargeras


 Events


 Unit - A unit owned by Player 2 (Blue) Acquires an item


 Unit - A unit owned by Player 3 (Teal) Acquires an item


 Unit - A unit owned by Player 4 (Purple) Acquires an item


 Unit - A unit owned by Player 5 (Yellow) Acquires an item


 Unit - A unit owned by Player 6 (Orange) Acquires an item


 Unit - A unit owned by Player 8 (Pink) Acquires an item


 Unit - A unit owned by Player 9 (Gray) Acquires an item


 Unit - A unit owned by Player 10 (Light Blue) Acquires an item


 Unit - A unit owned by Player 11 (Dark Green) Acquires an item


 Unit - A unit owned by Player 12 (Brown) Acquires an item


 Conditions


 ((Triggering unit) has an item of type Buriza) Equal to True


 ((Triggering unit) has an item of type HyperStone) Equal to True


 ((Triggering unit) has an item of type |cff8b00ffSword of Sargeras|r|n) Equal to True


 Actions


 Set Manipulate[1] = (Hero manipulating item)


 Item - Remove (Item carried by (Hero manipulating item) of type Buriza)


 Item - Remove (Item carried by (Hero manipulating item) of type HyperStone)


 Item - Remove (Item carried by (Hero manipulating item) of type |cff8b00ffSword of Sargeras|r|n)


 Hero - Create |cff8b00ffSword of Sargeras|r|n and give it to (Hero manipulating item)


 Sound - Play Tomes <gen>


 Special Effect - Create a special effect attached to the origin of Manipulate[0] using Abilities\Spells\Other\Levelup\LevelupCaster.mdl


 Wait 1.77 seconds


 Sound - Stop Tomes <gen> Immediately


And btw i cant seem to make it have 2 items at the same time like the Eye of Skadi in DotA . As soon as i aquire 1 Ultimate Orb i can simply but the recipe and it will combine ( even tough i only have 1 Ultimate Orb ) . Pls hlp if you can .


 Last edited by a moderator: Sep 10, 2007

#### GST_Nemisis

#### GST_Nemisis

 Level 21

JoinedJan 5, 2005Messages3,515

Resources: 1

#### Resources
Tutorials 1

after thinking breifley about the trigger i posted earlier i figured a much shorter way of doing it because of how the variable used is temporary and regularly re-written it wouldnt matter if it was an array. remember this is used for combining two items of the same type:

Untitled Trigger 001
Events
Unit - A unit Acquires an item

Conditions
(Item-type of (Item being manipulated)) Equal to Claws of Attack +15

Actions
For each (Integer A) from 1 to 6, do (If ((Item-type of (Item carried by (Hero manipulating item) in slot (Integer A))) Equal to Claws of Attack +15) then do (Set ItemAmount = (ItemAmount + 1)) else do (Do nothing))

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ItemAmount Equal to 2

Then - Actions
Hero - Drop (Item carried by (Hero manipulating item) of type Claws of Attack +15) from (Hero manipulating item)

Item - Remove (Last dropped item)

Hero - Drop (Item carried by (Hero manipulating item) of type Claws of Attack +15) from (Hero manipulating item)

Item - Remove (Last dropped item)

Hero - Create Crown of Kings +5 and give it to (Hero manipulating item)

Else - Actions
Set ItemAmount = 0

this used in combination with razorbrains trigger is the best and simplest recipie system imo.

#### GST_Nemisis

#### GST_Nemisis

 Level 21

JoinedJan 5, 2005Messages3,515

Resources: 1

#### Resources
Tutorials 1

can i suggest that the trigger that is written on the first page is reviewed and updated because as has been stated several times in this thread there are some flaws with the trigger on the first page and solutions have been provided by myself, razorbrain and others.

#### klemen

#### klemen

 Level 8

JoinedSep 24, 2007Messages303

Resources: 1

#### Resources
Maps 1

Agree.

And the bugs that appear (including mine) should be fixed. I believe there are users on the hive which could get enlighted and solve it.

EDIT: Now I don't get it. I tested the map Item recipies and if I drop the gloves and repick it, it is not replaced with recipi. While testing my map and dropping and repicking the combined item it is replaced. I copied the trigger and I didn't change anything but items. Variables are the same too.

So what the???

Is there anything else but those 2 triggers? I don't see another reason. Toolbox had to add something more then just this 2 triggers. It makes sence that gloves of power are replaced after repicking them from the ground because of the first trigger (which is exactly what happens in my map but in his map it doesn't...???).


 Last edited: Oct 15, 2007

#### busterkomo

#### busterkomo

 Level 17

JoinedJun 17, 2007Messages1,433

Resources: 5

#### Resources
JASS 1Tutorials 4

The variable "Ingredients" isn't needed.

#### GST_Nemisis

#### GST_Nemisis

 Level 21

JoinedJan 5, 2005Messages3,515

Resources: 1

#### Resources
Tutorials 1

MateuszD said:

 and how can i do that the hero can have only one type of this item e.g. hero buys 2 claws of attack than comes massages u cant have 2 same items and removes one of them


Click to expand...


this has already been discussed and i have provided a solution for this on the previous pages. also to hvo-busterkomo the trigger on the first page is buggy and better solutions have already been provided on the second and third pages, use them if u can. the first post should be fixed tbh.

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

~ Thread moved to Tutorial Submission forum pending updates.

#### GST_Nemisis

#### GST_Nemisis

 Level 21

JoinedJan 5, 2005Messages3,515

Resources: 1

#### Resources
Tutorials 1

toolbox describes the theory of item recipies very well but the triggers he shows are bugged and not effective in all situations. i would suggest using these two triggers:

Razorbrain said:

Combination
Events
Unit - A unit Aquires an item

Conditions
((Triggering unit) has an item of type Item1) equal to True

((Triggering unit) has an item of type Item2) equal to True

((Triggering unit) has an item of type Recipe) equal to True

Actions
Item - Remove (Item carried by (Triggering unit) of type Item1)

Item - Remove (Item carried by (Triggering unit) of type Item2)

Item - Remove (Item carried by (Triggering unit) of type Recipe)

Hero - Create Weapon and give it to (Triggering unit)

Click to expand...


this one by razorbrain can be used to automatically create an item when a unit picks up two items of a different type. with this trigger you must also have a recipie item, but if you remove the lines "((Triggering unit) has an item of type Recipe) equal to True" and "Item - Remove (Item carried by (Triggering unit) of type Recipe)" it will do it when the unit automatically when the unit aquires the items.


and:

GST_Nemisis said:

Untitled Trigger 001
Events
Unit - A unit Acquires an item

Conditions
(Item-type of (Item being manipulated)) Equal to Claws of Attack +15

Actions
For each (Integer A) from 1 to 6, do (If ((Item-type of (Item carried by (Hero manipulating item) in slot (Integer A))) Equal to Claws of Attack +15) then do (Set ItemAmount = (ItemAmount + 1)) else do (Do nothing))

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ItemAmount Equal to 2

Then - Actions
Hero - Drop (Item carried by (Hero manipulating item) of type Claws of Attack +15) from (Hero manipulating item)

Item - Remove (Last dropped item)

Hero - Drop (Item carried by (Hero manipulating item) of type Claws of Attack +15) from (Hero manipulating item)

Item - Remove (Last dropped item)

Hero - Create Crown of Kings +5 and give it to (Hero manipulating item)

Else - Actions
Set ItemAmount = 0

Click to expand...


this can be used to turn two items of the same type into another item (for example Claws of Attack +15 + Claws of Attack +15 = Crown of Kings +5). this happens automatically but by adding the condition and action mentioned above you can make it require a recipie item. for this you need the integer variable ItemAmount.

[Edit by Wolve] ~ Tutorial edited and (re)approved.


 Last edited by a moderator: Feb 6, 2008

#### KaiserG

#### KaiserG

 Level 6

JoinedFeb 12, 2008Messages207

 very useful guide. +REP

#### Chief-Oblivion

#### Chief-Oblivion

 Level 11

JoinedMar 20, 2008Messages485

Resources: 11

#### Resources
Icons 9Spells 1Tutorials 1

I didn't find his way very good.. it's really confusing, this is the easiest way Iv'e seen for an "AUTO-COMBINE!!!" recipe


Events


 Unit - A unit Acquires an item


Conditions


Actions


 If(All Conditions are True) then do (Then Actions) else do (Else Actions)


 If - Conditions


 ((Hero Manipulating item) has an item of type Boots of Speed) Equal to True


((Hero Manipulating item) has an item of type Slippers of Agility +3 Equal to True


 Then - Actions


Item - Remove (item carried by (hero manipulating item) of type Boots of Speed)


Item - Remove (item carried by (hero manipulating item) of type Slippers of Agility +3)


Item - Create Boots of Quel'Thalas +6 at (position of (hero manipulating item))


Hero - Give (Last created item) to (hero manipulating item)


Else - Actions


Do Nothing


That can be shaped and molded into more than just 2 items in a recipe, i used it for 5 items into 1, and it works great, if you want to make it take longer or something like that so it doesn't go into your inventory instantly, you're going to have to make another trigger, those trigger's aren't hard though, i started playing with triggers yesterday and i can already do quite a bit.

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

that is like exactly what i did


except mine was leak free


the one posted by GST is for when there are several items of the same type in a recipe

#### GodWasHere

#### GodWasHere

 Level 6

JoinedMar 7, 2008Messages40

Resources: 3

#### Resources
Models 1Icons 1Skins 1

But how do i make it do a effect like in dota, like a massive beam.

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Simply request the spell at Request forum or make an active spell with target and change the effect model to starfall or something like that.

#### Decist

#### Decist

 Level 3

JoinedApr 5, 2008Messages41

nice. thanks for the help

#### cHeRoL

#### cHeRoL

 Level 11

JoinedMar 30, 2008Messages650

Resources: 5

#### Resources
Models 1Maps 2Spells and Systems 2

I will try,but is 2 hard for me!

#### Simon559

#### Simon559

 Level 4

JoinedApr 11, 2007Messages97

Set FactorCleared[(Integer A)] = False


How do you get it to False?! help

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

you make it a Boolean variable?

#### Dreadnought[dA]

#### Dreadnought[dA]

 Level 12

JoinedFeb 23, 2007Messages1,030

Resources: 1

#### Resources
Tutorials 1

If you need to check for multiples, you can just use greater than 1, then remove 2 of the claws of attack. That way if you have 6 claws of attack, it will still convert 2 of them.

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Well this is nonsense crap :S sry

#### falen13

F

#### falen13

I will gonna try this one one thing can you make there i can found condition the place becouse its fuuck off then you neeead search for 2 minutes one stupid shit

#### cHeRoL

#### cHeRoL

 Level 11

JoinedMar 30, 2008Messages650

Resources: 5

#### Resources
Models 1Maps 2Spells and Systems 2

Use this

Hey,use this,it works,i'm using it

Events

Unit - A unit Acquires an item

Conditions

(Item-type of (Item being manipulated)) Equal to Claws of Attack +15

Actions

For each (Integer A) from 1 to 6, do (If ((Item-type of (Item carried by (Hero manipulating item) in slot (Integer A))) Equal to Claws of Attack +15) then do (Set ItemAmount = (ItemAmount + 1)) else do (Do nothing))

If (All Conditions are True) then do (Then Actions) else do (Else Actions)

If - Conditions

ItemAmount Equal to 2

Then - Actions

Hero - Drop (Item carried by (Hero manipulating item) of type Claws of Attack +15) from (Hero manipulating item)

Item - Remove (Last dropped item)

Hero - Drop (Item carried by (Hero manipulating item) of type Claws of Attack +15) from (Hero manipulating item)

Item - Remove (Last dropped item)

Hero - Create Crown of Kings +5 and give it to (Hero manipulating item)

Else - Actions

Set ItemAmount = 0
And this it works 100%,I'm using it too.

Events

Unit - A unit Aquires an item

Conditions

((Triggering unit) has an item of type Item1) equal to True

((Triggering unit) has an item of type Item2) equal to True

((Triggering unit) has an item of type Recipe) equal to True

Actions

Item - Remove (Item carried by (Triggering unit) of type Item1)

Item - Remove (Item carried by (Triggering unit) of type Item2)

Item - Remove (Item carried by (Triggering unit) of type Recipe)

Hero - Create Weapon and give it to (Triggering unit)

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

you realise that those triggers are exact copies of ours, right?

#### lw.warlord

L

#### lw.warlord

Guys pls tell me how to do this :


Hero - Create Recipe: Gloves of Power and give it to (Hero manipulating item)


So the trigger goes in Create Action -> Hero -> Create item for hero -> and to pick the item which in this case it's recipe. And there is no option recipe there . If it's variable pls tell me how to do it . Pls PM me asap ! Some1 answer me by message pls


!peace

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

or maybe i could just tell you in the thread right here?


the Recipe: Gloves of Power is either a custom item or an item with that as a name. Its not an original Wc3 item


ex:


Original wc3 Item : Gauntlets of Strength +3


now, i can go into the Object Editor, and go to the items section


now, i can select Gauntlets of Strength +3 and rename them to whatever i want, like Recipe: Gloves of Power, if i wish, or Razorbrains Ultimate Gauntlets of Strength

#### cHeRoL

#### cHeRoL

 Level 11

JoinedMar 30, 2008Messages650

Resources: 5

#### Resources
Models 1Maps 2Spells and Systems 2

the original item is not gauntles of Strengh +3 is Gauntles of Brutal Strengh +3

#### lw.warlord

L

#### lw.warlord

umm, ok who cares wether gloves of power is original or not original recipe ?! Just answer my question some1 ok? I will rate

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

then, what do you mean by "and there is no option recipe there"?


its not a variable, i can tell you that already

#### cHeRoL

#### cHeRoL

 Level 11

JoinedMar 30, 2008Messages650

Resources: 5

#### Resources
Models 1Maps 2Spells and Systems 2

Man,is easy,just look there and follow the instructions.

#### StasMaN

#### StasMaN

 Level 13

JoinedMar 14, 2008Messages847

Resources: 7

#### Resources
Maps 6Tutorials 1

cherol , you should try doing something useful to gain reputation or help others . Not just fill every thread with your useless thoughts

#### cHeRoL

#### cHeRoL

 Level 11

JoinedMar 30, 2008Messages650

Resources: 5

#### Resources
Models 1Maps 2Spells and Systems 2

Leave me alone.

#### StasMaN

#### StasMaN

 Level 13

JoinedMar 14, 2008Messages847

Resources: 7

#### Resources
Maps 6Tutorials 1

Btw what types of variables are they ?

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

that depends, what trigger are you reffering to?

#### Flash.Kisame

#### Flash.Kisame

 Level 7

JoinedApr 6, 2008Messages126

Resources: 2

#### Resources
Maps 2

Very nice trigger. Im sure you help a lot of mapper (incuding me^^) thx for posting it.

#### StasMaN

#### StasMaN

 Level 13

JoinedMar 14, 2008Messages847

Resources: 7

#### Resources
Maps 6Tutorials 1

Managed to use it without variables ;D

#### Kallimachos

#### Kallimachos

 Level 12

JoinedJul 26, 2007Messages164

Resources: 16

#### Resources
Icons 4Maps 12

hey guys, thx for your tutorials, but i couldnt find one of type:


2x Item of type A

+Recipe = other item,


which means that u need a recipe and two similar items. therefore i modified the triggers, maybe someone could find a use for this:


example from my developing map CRA:


2x Grunt Axe + Gauntlets of Strength + Double-edged Axe (Recipe) = Double-edged Axe

#### Attachments

aaa tutorial recipes.JPG

 83.8 KB

 · Views: 488




 Last edited: Jul 4, 2008

#### pyropk95

#### pyropk95

 Level 9

JoinedFeb 5, 2008Messages383

Resources: 5

#### Resources
Skins 1Maps 4

i know how to make a recipe thing...just go on youtube and type wc3 editor recipe..thats how i learned all my basics (youtube )

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

Kallimachos said:

 hey guys, thx for your tutorials, but i couldnt find one of type:


2x Item of type A

+Recipe = other item,


which means that u need a recipe and two similar items. therefore i modified the triggers, maybe someone could find a use for this:


example from my developing map CRA:


2x Grunt Axe + Gauntlets of Strength + Double-edged Axe (Recipe) = Double-edged Axe


Click to expand...

 we actually did one of those

#### Kallimachos

#### Kallimachos

 Level 12

JoinedJul 26, 2007Messages164

Resources: 16

#### Resources
Icons 4Maps 12

Razorbrain said:

 we actually did one of those

Click to expand...

not really? i could not find it anywhere.

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

at the very bottom of the first post there is one

#### TheBlooddancer

#### TheBlooddancer

 Level 22

JoinedJun 24, 2008Messages3,041

Resources: 2

#### Resources
Maps 1Tutorials 1

Isnt there an easier way to do 2 of same item = One item..?


Yours seem way to frustrating.


I hope there is another..


(MY example)


Samurai Sword + Samurai Sword = Dual Samurai Sword.

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

which one are you reffering to?

 You must log in or register to reply here.


### Similar threads

Locked

 Item Recipe - Combining two same items with one different items


myronbeg

Aug 30, 2011

Triggers & Scripts

Replies4Views3K

Sep 1, 2011
UnHolyDarKness

Locked

[Solved]
 Item Recipe x 2 mantle's of int


Space_Ship

Oct 8, 2011

World Editor Help Zone

Replies2Views2K

Oct 9, 2011
Space_Ship

Locked

[Trigger]
 Full inventory and item recipes(dota)


AlwaysAndForever

Jan 29, 2019

Triggers & Scripts

Replies14Views4K

Feb 3, 2019
Lake

Locked

[Solved]
 Recipe Item...


pplDotA

Dec 28, 2018

World Editor Help Zone

Replies5Views2K

Dec 29, 2018
pplDotA

Poll

 How to make an Item Recipe System - Like DotA's


thereallywhitekid

May 27, 2009

Tutorial Archive

Replies1Views2K

May 29, 2009
Pyritie

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