---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-warcraft-3-reforged-localized-dialogues.334726/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Warcraft 3 Reforged Localized Dialogues
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-warcraft-3-reforged-localized-dialogues.334726/>
#### InsaneMonster

#### InsaneMonster

Patron

 Level 39

JoinedJul 20, 2011Messages672

Resources: 23

#### Resources
Tools 1Maps 3Reforged HD Models 10Sounds 4JASS 1Lua 1Tutorials 3

##
 Warcraft 3 Re-Reforged


WARNING: This tutorial can be applied only to Warcraft 3 Reforged!

GOAL

To show how to localizeWarcraft 3 Reforged dialogues. Dialogues are the entities used to play sound with linked facial animation, as explained in this other tutorial:

How to: Warcraft 3 Reforged Animated Portraits

KNOWLEDGE REQUIRED

How to create and edit animated portraits in Warcraft 3 Reforged World Editor (OPTIONAL)

How to localize Warcraft 3 Reforged maps' strings

Basic knowledge of the Trigger Editor

Basic knowledge of the Object Editor
TOOLS REQUIRED

Warcraft 3 Reforged World Editor

TABLE OF CONTENT

BACKGROUND (OPTIONAL)

CREATE DIALOGUES ITEMS

ASSIGN ITEMS PROPERTIES TO DIALOGUES

BACKGROUND (OPTIONAL)

This section provides insights into why this workaround is necessary with respect to the usual localization capabilities offered by Warcraft 3 Reforged.

To localize assets in Warcraft 3 Reforged is as easy as selecting the appropriate locale in the Asset Manager. To localize strings is just slightly more complex, requiring first to export the map native strings and then import back each localized version as war3map.wts files with the Asset Manager, setting the appropriate locale as per every other asset.

There are, however, some problems with dialogues.

The really bad news is that dialogues texts don't show up in the .wts file exported by the editor (dialogues are throughly explained in this tutorial about animated portraits in Reforged)

Some may say: "Wait, Blizzard's official campaigns dialogues texts do show up!" and you would be right.

Problem is, to make such dialogues Blizzard used something called the Dialogue Editor, which is a cut feature in the editor released to public, our editor, probably because it was very unstable. The functions we all use to set up dialogues texts don't add any string to the .wts file.

"So we are doomed!" many would say.

No, we just need to find a good workaround. First of all, we need to understand when a localized asset, in this case the .wts file, is loaded. This is done before the map is initialized, when the engine checks for the current locale in the settings and it loads all the assets accordingly.

"So, if we can use our dialogues functions with strings already localized, it could work?" you might ask, and the answer is yes.

Now, we need to think about something that has many text fields in which we can store all the dialogue information (speaker name and spoken text) and that is for sure available in the .wts file. This information will be transferred at runtime to the dialogue through the functions we all use and love. I think the best choice for this very something are items.

The tutorial that follows is about just that.

CREATE DIALOGUES ITEMS

This section show how to create dialogue items with the format I use in Warcraft 3 Re-Reforged. It should give you an idea on how these items are defined.

This will be very short, for it's very easy. Basically, I use the dialogue sound variable name as the item name. Then I use the description as the dialogue spoken text and, finally, the basic tooltip as speaker name.

These fields can easily be retrieved as strings in the GUI Trigger editor, as I will show in the section below.

ASSIGN ITEMS PROPERTIES TO DIALOGUES

This section show how to use dialogues GUI functions to assign the localized strings stored in the items to the dialogues at runtime.

As it's possible to see in each and every Warcraft 3 Re-Reforged map, for each additional (not already inserted into the map in Blizzard's default campaigns) dialogue the following is used, during initialization of the map:

The first custom script loads the facial animation for the dialogue, as explained here.

The second action creates the item with the same name of the dialogue sound. Then, speaker name and dialogue text (or spoken text) are assigned from the item properties, which are localized already at the time of executing this (indeed, remember that localized assets are loaded before initialization).

Finally, the item is removed, making this system completely transparent to the player.

CONCLUSIONS
And... this is it people!

You can now localized your animated portaits. I know it's a workaround, but it works quite well and it provides a minimum overhead. Hopefully, one day we will have the dialogue editor and stuff like this could be deprecated.

Hope you liked the tutorial, have fun localizing these dialogues!

#### Attachments

LocalizedDialoguesExampleMap.w3m

 35.8 KB

 · Views: 103




 Last edited: Jan 28, 2022

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Layout is good, not necessarily what I would do myself but not anything to complain about for sure.

I have two.. concerns, for lack of better words.

1. I think it would be better to make a trigger that handles the data.

set dialog_item = captainwaycry

run trigger LoadItemData

Cinematic set speaker name to dialog_name

Cinematic set dialog text to dialog_text

where dialog_text and dialog_name would be global variables set in the LoadItemData trigger.

Reason is because the DRY principle, Don't Repeat Yourself and if you use this trick a lot it should make your code a fair bit cleaner.

2. This one I am slightly unsure of but, you do not need the animation right? at the top of the tutorial you state that this knowledge is needed but you do not explain why.

2b. If the animation is indeed required I think you should explain why. Maybe its stated somewhere in the other tutorial but that's a big block of text to go through and I feel this should be easier to see.

2c. If the animation is optional I think that should be made clear as well

I also think an attached map example would be useful.

#### InsaneMonster

#### InsaneMonster

Patron

 Level 39

JoinedJul 20, 2011Messages672

Resources: 23

#### Resources
Tools 1Maps 3Reforged HD Models 10Sounds 4JASS 1Lua 1Tutorials 3

##
 Warcraft 3 Re-Reforged


Chaosy said:

 Layout is good, not necessarily what I would do myself but not anything to complain about for sure.

I have two.. concerns, for lack of better words.

1. I think it would be better to make a trigger that handles the data.

set dialog_item = captainwaycry

run trigger LoadItemData

Cinematic set speaker name to dialog_name

Cinematic set dialog text to dialog_text

where dialog_text and dialog_name would be global variables set in the LoadItemData trigger.

Reason is because the DRY principle, Don't Repeat Yourself and if you use this trick a lot it should make your code a fair bit cleaner.

2. This one I am slightly unsure of but, you do not need the animation right? at the top of the tutorial you state that this knowledge is needed but you do not explain why.

2b. If the animation is indeed required I think you should explain why. Maybe its stated somewhere in the other tutorial but that's a big block of text to go through and I feel this should be easier to see.

2c. If the animation is optional I think that should be made clear as well

I also think an attached map example would be useful.


Click to expand...

Ok I will answer to your concerns:

1) This is a stilystic choice, also as you can see from my trigger you need to change the sound variable associated with the dialogue each time you load a new item. So you are removing 2 lines of code by adding 1 trigger, 1 line of code to run this trigger and 3 global variables (speaker name, spoken text, item). Consider that the facial animations I cannot load using the item variable because I need to call a custom script, I don't see this as an objective improvement, especially since this code is supposed to be run only in the initialization of the map. So no, I don't agree on that and while your alternative is definitely viable it smells of over engineering to me. If you want I can provide it as an alternative, with a screenshot and a brief explanation.

2) Ok, yes this is unclear. Basically a dialogue is an instance of a sound, a spoken text, a speaker name and a facial animation, as explained in the other tutorial I linked. We are localizing dialogues here, so this is why I am linking the other tutorial. I will make sure to make this clearer though in the following days, thanks.

I will provide an example map in the following days!

#### InsaneMonster

#### InsaneMonster

Patron

 Level 39

JoinedJul 20, 2011Messages672

Resources: 23

#### Resources
Tools 1Maps 3Reforged HD Models 10Sounds 4JASS 1Lua 1Tutorials 3

##
 Warcraft 3 Re-Reforged


Chaosy said:

 Layout is good, not necessarily what I would do myself but not anything to complain about for sure.

I have two.. concerns, for lack of better words.

1. I think it would be better to make a trigger that handles the data.

set dialog_item = captainwaycry

run trigger LoadItemData

Cinematic set speaker name to dialog_name

Cinematic set dialog text to dialog_text

where dialog_text and dialog_name would be global variables set in the LoadItemData trigger.

Reason is because the DRY principle, Don't Repeat Yourself and if you use this trick a lot it should make your code a fair bit cleaner.

2. This one I am slightly unsure of but, you do not need the animation right? at the top of the tutorial you state that this knowledge is needed but you do not explain why.

2b. If the animation is indeed required I think you should explain why. Maybe its stated somewhere in the other tutorial but that's a big block of text to go through and I feel this should be easier to see.

2c. If the animation is optional I think that should be made clear as well

I also think an attached map example would be useful.


Click to expand...

Hey there @Chaosy , I know it took a long time, but I've added the example map and the fact animated potraits are optional. I will not add the alternative example because I don't really think it is required and it just add more global variables. In a true case you don't want to add a bunch of global variables just for one trigger, and if you want to also add animated potraits (which I think it's the case in real scenario), your DRY approach is not feasible as shown in the example map.

The example map is localized in english and italian.

Let me know if you can approve this tutorial now.

#### OutsiderXE

#### OutsiderXE

 Level 34

JoinedSep 28, 2004Messages1,520

Resources: 26

#### Resources
Maps 4Reforged HD Icons 22

##
 Lore and Legends


... or use Externalized string()

It will create the text in the war3map.wts and you don't need to bother with creating items.

 You must log in or register to reply here.


### Similar threads

 How to: Warcraft 3 Reforged Animated Portraits


InsaneMonster

Jun 17, 2020

Misc/General Mapping Tutorials

Replies18Views13K

May 29, 2023
InsaneMonster

 How to make Cinematics in Warcraft III - Complete Series


Turnro

Apr 8, 2022

Misc/General Mapping Tutorials

Replies0Views5K

Apr 8, 2022
Turnro

Locked

 Cinematic - Play Dialogue with shown text on Screen


DarkIllidan

Feb 4, 2022

World Editor Help Zone

Replies0Views1K

Feb 4, 2022
DarkIllidan

 How to: Warcraft 3 Reforged Facial Expressions


InsaneMonster

Aug 17, 2021

Misc/General Mapping Tutorials

Replies12Views4K

Feb 9, 2022
InsaneMonster

 modify narrator text prologue orc


Formo

Feb 21, 2026

World Editor Help Zone

Replies4Views298

Feb 22, 2026
Ravager16829

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