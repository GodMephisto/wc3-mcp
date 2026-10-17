---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-warcraft-3-reforged-facial-expressions.334620/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Warcraft 3 Reforged Facial Expressions
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-warcraft-3-reforged-facial-expressions.334620/>
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

To show a very hidden feature of the FaceFX animations in Warcraft 3 Reforged animated potraits: facial expression. Through this feature it's possible to inject some very required emotions into the characters faces.

KNOWLEDGE REQUIRED

How to create and edit animated portraits in Warcraft 3 Reforged World Editor (How to: Warcraft 3 Reforged Animated Portraits)
TOOLS REQUIRED

Warcraft 3 Reforged World Editor

TABLE OF CONTENT

BACKGROUND (OPTIONAL)

FACEFX CURVES

IN-GAME IMPLEMENTATION

BACKGROUND (OPTIONAL)

This section provides insights into what the facial expressions feature is and where you can find it's very limited usage within Warcraft 3 Reforged campaigns.

One of the main complaints Warcraft 3 Reforged characters received was they looked emotionless.

While this is true to a certain exent, I always believed realistic characters with proper lip-sync cannot look as expressive as stylized characters for which to talk meant to repeatedly move their mouth and head to a ridicoulus extent. Now, I know it looked very nice, but just because classic graphics is... what it is, and as such this kind of exaggerated motion would look really bad on modern, realistic faces.

However, there is an hidden feature, simple yet quite tedious to implement, that can help on that regard, by making the characters faces adapt to the mood of their spoken text. Or, well, at least to what the programmer want this mood to be.

This feature was introduced to me by @Kam, mentioning they have been using it on the first human missions... and nowhere else. From the very limited samples I could find inside the very first human chapters, I found a set of useful commands and tricks I am going to share with you all.

FACEFX CURVES TEXT TAGS

This section explains the technology behind the facial expressions and it links the appropriate documentation.

I will keep this section short, but I feel it is required to fully understand this tutorial. The technology behind facial expression is part of the FaceFX software. Specifically, it's called FaceFX Curves Text Tags. This technology allows to inject a customizable behaviour into the spoken text associated with a dialogue.

By writing some keywords in the text, called tags, the behaviour of the facial animation linked with such text is changed. Bezier curves with four possible configuration are used to interpolate between a zero level of application of such behaviour and a certain multiplier, also customizable on a per tag basis.

Refer to the linked article by FaceFX for additional details.

IN-GAME IMPLEMENTATION

This section shows the available behaviours of FaceFX Curves Text Tags within Warcraft 3 Reforged and some usage samples.

One of the most important thing you have to know is that behaviors themeselves are not configurable by us and have been designed by Blizzard animators. The only thing we can do, is to decide when and how to use them.

There is no documentation about this feature available, so I had to search into provided examples to find the available tags. The one I have found are the following:

Angry

Sad

Happy

Surprised

Scared

Squint
They seems to me pretty self-explanatory. Some examples, using sentences from Warcraft 3 Re-Reforged, are the following:

["Angry" type=quad v2=1.5 v3=3.0 easein=0.5 easeout=0.5]I don't know what this is all about, but I'll play along.[/"Angry"]

["Happy" type=quad v2=3.0 v3=0.5 easein=0.5 easeout=1.0]Greetings, son of Durotan. I knew you'd find your way.[/"Happy"]

["Happy" type=lt v2=1.5 easein=0.25 easeout=0.25]I know many things, young warchief, about you and your people.[/"Happy"] ["Angry" type=quad v2=2.0 v3=2.0 easein=0.25 easeout=1.0]My identity, at this time, is unimportant. What is important is that you rally your people and leave these shores immediately.[/"Angry"]

["Squint" type=lt v2=1.5 easein=0.25 easeout=0.25]Human?[/"Squint"] ["Scared" type=quad v2=2.0 v3=1.0 easein=1.0 easeout=1.0]I left my humanity behind long ago. I am something... different now. Know that I have seen the future and beheld the great burning shadow that is coming to consume this world. You sense it as well, don't you?[/"Scared"]

["Happy" type=lt v2=4.0 easein=0.5 easeout=1.0]It was lucky we happened onto this cove.[/"Happy"] ["Scared" type=quad v2=2.0 v3=4.5 easein=0.5 easeout=0.5]I don't think our ships would have held out much longer in that storm.[/"Scared"]

["Sad" type=quad v2=2.0 v3=4.5 easein=0.5 easeout=0.5]Warchief, half of the ships are missing,[/"Sad"] ["Angry" type=rt v2=2.5 easein=0.75 easeout=0.25]and the rest are badly damaged.[/"Angry"]

["Surprised" type=quad v2=4.0 v3=1.5 easein=0.5 easeout=0.25]We had better get settled, then.[/"Surprised"] ["Sad" type=rt v2=2.5 easein=0.75 easeout=0.25]We may be on this island for awhile.[/"Sad"]
I would advise to use high values for the v2, v3 parameters in order to exaggerate the effect. It's not always necessary, as shown in some of these sentences, but it can definitely help in many situations.

Also keep in mind that easein and easeout parameters are expressed in seconds.

Finally, expect the effect to be very subtle, hard to notice if you don't compare side by side. But, I can assure you, the effect is there. To see if it's working, look at the eyes, they move much more expressively when appropriate values and tags are used.

Now, one last warning: this feature only works when cinematic mode is active. If you try to use it during gameplay, it doesn't work properly and the tag text is fully displayed. In cinematics, though, it's processed correctly and it's hidden from the spoken text.

CONCLUSIONS
And... this is it people!

I hope that this feature can help in giving a bit more life to your characters animations in cutscenes. Let me know if you discover additional tags, and I'll add them to this tutorial.

Have fun with their faces!

#### BogdanW3

#### BogdanW3

 Level 20

JoinedFeb 25, 2013Messages359

Resources: 5

#### Resources
Tools 2Tutorials 3

Good tutorial, straight to the point and gives examples from the game itself. Things I have to add is that it's also possible to define one curve text inside another: ["Happy" type=lt easein=0.25 easeout=1.5 easeout=2.0 v2=0.6 v3=0.0]Welcome, Prince Arthas. The men and I are ["Angry" type=lt easein=0.25 v2=0.7 v3=0.0]honored [/"Angry"]by your presence.[/"Happy"] and that the facial animset data is in conversation.json, which might be needed to properly hook up FAFX of models to the premade animation sets.

These keys are to be placed in the conversation text inside war3map.wts so it's localizable, they work in locales.

For more examples and the whole dialogue system in action, especially how conversation.json+war3map.w3s+war3map.wts+war3map.wtg (conversation data the game uses, sound data the game uses, strings file containing text for the dialogue, and the triggers) work, Human01 of the Reforged campaign is a good start (check both the main wts and the one in esMX.w3mod locale to see that it can be done in locales too)

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


BogdanW3 said:

 Good tutorial, straight to the point and gives examples from the game itself. Things I have to add is that it's also possible to define one curve text inside another: ["Happy" type=lt easein=0.25 easeout=1.5 easeout=2.0 v2=0.6 v3=0.0]Welcome, Prince Arthas. The men and I are ["Angry" type=lt easein=0.25 v2=0.7 v3=0.0]honored [/"Angry"]by your presence.[/"Happy"] and that the facial animset data is in conversation.json, which might be needed to properly hook up FAFX of models to the premade animation sets.

These keys are to be placed in the conversation text inside war3map.wts so it's localizable, they work in locales.

For more examples and the whole dialogue system in action, especially how conversation.json+war3map.w3s+war3map.wts+war3map.wtg (conversation data the game uses, sound data the game uses, strings file containing text for the dialogue, and the triggers) work, Human01 of the Reforged campaign is a good start (check both the main wts and the one in esMX.w3mod locale to see that it can be done in locales too)


Click to expand...

I will provide soon a tutorial about localizing dialogues, which requires a workaround since the texts associated with dialogues do not compare in the .wts. I use a trick with items in order to use dialogues as official maps do.


 Last edited: Aug 17, 2021

#### Kam

#### Kam

 Level 45

JoinedAug 3, 2004Messages2,912

Resources: 44

#### Resources
Models 10Icons 2Maps 29Reforged HD Models 1Ported Models 1Tutorials 1

Thank you for writing this tutorial!

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


Kam said:

 Thank you for writing this tutorial!


Click to expand...

If you want to expand on something, feel free to do so!

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


@BogdanW3 as promised:

###
 How to: Warcraft 3 Reforged Localized Dialogues


WARNING: This tutorial can be applied only to Warcraft 3 Reforged! GOAL To show how to localize Warcraft 3 Reforged dialogues. Dialogues are the entities used to play sound with linked facial animation, as explained in this other tutorial: How to: Warcraft 3 Reforged Animated Portraits...

 www.hiveworkshop.com


#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Not much to complain about here really. As far as visuals go, awesome.

I know nothing about animations so I cannot say much about the workflow itself.

I poked Moonman about it and he said that at the very least the method shown works and in terms of more in-depth feedback you're the authority on the subject so I'll give you the benefit of the doubt.

So with that in mind, approved.

#### DarkIllidan

#### DarkIllidan

 Level 12

JoinedApr 27, 2009Messages237

Resources: 4

#### Resources
Maps 2Reforged HD Models 2

Where does one insert?

["Angry" type=quad v2=1.5 v3=3.0 easein=0.5 easeout=0.5]Text from the sound file[/"Angry"]
Taking into account the Localized Dialogues Tutorial above. Inside item description 'Text - Description' ?

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


DarkIllidan said:

 Where does one insert?

["Angry" type=quad v2=1.5 v3=3.0 easein=0.5 easeout=0.5]Text from the sound file[/"Angry"]
Taking into account the Localized Dialogues Tutorial above. Inside item description 'Text - Description' ?


Click to expand...

I usually do this inside the .wts file.

#### DarkIllidan

#### DarkIllidan

 Level 12

JoinedApr 27, 2009Messages237

Resources: 4

#### Resources
Maps 2Reforged HD Models 2

I have done everything. Even Dialogues are written by themselves in conversation.json. Could you explain me more how to write .wts file? Still a bit confused after reading what you written about war3map.wts .

I insert a blank war3map.wts inside?

When I go inside your map Localized Dialogue map and your first Chapter of Lordaeron I only see other languages inside your war3map.wts files?

#### Attachments

conversation.json

 310 bytes

 · Views: 82



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


DarkIllidan said:

 I have done everything. Even Dialogues are written by themselves in conversation.json. Could you explain me more how to write .wts file? Still a bit confused after reading what you written about war3map.wts .

I insert a blank war3map.wts inside?

When I go inside your map Localized Dialogue map and your first Chapter of Lordaeron I only see other languages inside your war3map.wts files?


Click to expand...

File -> Export strings (to export)

File -> Import strings (to import)

This is your english .wts file. Then you can localize it in a text editor and import the localized files as you find in my maps.

#### DarkIllidan

#### DarkIllidan

 Level 12

JoinedApr 27, 2009Messages237

Resources: 4

#### Resources
Maps 2Reforged HD Models 2

Somehow all names of my custom units disappeared so I could not export it. Thanks for help!

Would love example map for this!

Facial Expressions are very subtle on High Elf Villager or I didn't successed in making them work.


 Last edited: Feb 7, 2022

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


DarkIllidan said:

 Somehow all names of my custom units disappeared so I could not export it. Thanks for help!

Would love example map for this!

Facial Expressions are very subtle on High Elf Villager or I didn't successed in making them work.


Click to expand...

They are very subtle, yes, definitely. It's really hard to see differences unless you do a side by side, and not all the models react the same. From my experience, Thrall is quite expressive.

 You must log in or register to reply here.


### Similar threads

Locked

 Reforged - Lip sync on characters during cinematic


Solcius123

Feb 4, 2020

World Editor Help Zone

Replies22Views8K

Jun 17, 2020
InsaneMonster

 How to: Warcraft 3 Reforged Localized Dialogues


InsaneMonster

Aug 18, 2021

Misc/General Mapping Tutorials

Replies4Views5K

May 7, 2024
OutsiderXE

 How to: Warcraft 3 Reforged Animated Portraits


InsaneMonster

Jun 17, 2020

Misc/General Mapping Tutorials

Replies18Views13K

May 29, 2023
InsaneMonster

Locked

 Using regular expressions to find and delete keyframes


GhostWolf

Nov 10, 2016

Modeling & Animation

Replies5Views3K

Nov 14, 2016
Hermit

 How to add custom background to your custom campaign


CustomCreations

Feb 6, 2023

Misc/General Mapping Tutorials

Replies4Views4K

Feb 16, 2025
MapMaker14

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