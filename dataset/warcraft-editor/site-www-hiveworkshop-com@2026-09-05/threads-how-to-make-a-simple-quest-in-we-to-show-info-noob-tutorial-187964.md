---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-make-a-simple-quest-in-we-to-show-info-noob-tutorial.187964/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Make a Simple Quest in WE to Show Info Noob Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-make-a-simple-quest-in-we-to-show-info-noob-tutorial.187964/>
#### Slayer14

#### Slayer14

 Level 10

JoinedDec 12, 2010Messages569

Resources: 7

#### Resources
Maps 5Tutorials 2

How to Make a Simple Quest in WE to Show Info (Beginner Tutorial)

I know, what a seemingly useless tutorial for the average WE user, but not for those new guys. And yes, I made this for a new guy.


OK, lets begin.


First off, open the World Editor, which will be often refered to as WE, not only in this tutorial, but almost anywhere else you go. Then open up your map that you want to add a quest to.


Open up the Trigger Editor.


Create a new trigger. It does not matter what category you create it in or the name you give it. (In this tutorial, I have named it Quest Creator.)


Click the button titled "New Event".


A window titled "Configure Event" should a appear. There is a dropbox (denoted in the following screenshot) that contains a list of events for your trigger. You don't need to open this for creating quests at the beginning of the game. Click OK.


Your trigger now has the "Map Initialization" event.


Click the Actions button. A window titled "Configure Action" should appear. Open up the dropbox (denoted in the following screenshot) by clicking the arrow to the right of it.


Scroll down in the list of Actions until you find the Action "Quest - Create Quest". Select it. The "Configure Action" window now looks like the following screenshot.


You can basically do whatever you want at this point. You can make the quest a Required Quest or an Optional Quest, depending on which column you want it to be in on the Quest Log. The first column is Required Quests and the second column is Optional Quests. Give the quest a suitable title like "Game Info", make the description actual information about the game, and make the icon really anything you want, though I'd use a book or a scroll.


And now, well, your done. That's it. Anyway, have fun!


Okay, if you wanted a fully functioning, yet simple quest, follow the next 8 steps.


In this tutorial, I will be making a quest that only has one quest requirement, kill 3 Murloc Tiderunners. This tutorial is making the assumption that the WE is already open along a map that you want to put a quest on.

Step 1: Open the Trigger Editor.

Step 2: Create a new trigger and give it the event, "Map initialization". In this tutorial, this trigger will be referred to has "Quest Creator." (You may choose to skip this step if you already have a trigger with the event, "Map initialization.")

Step 3: Create the "Quest - Create Quest" action for the "Quest Creator" trigger. Give the action a suitable title, description, icon, and choose to either display it as a required quest or an optional quest.

Step 4: Create a variable with the type Quest. The name of the variable doesn't matter, as long as you know which variable is the variable for your quest. In this tutorial, I will call it "KtM_Quest." Refer to the Screenshot below for help.

Step 5: Create the action, "Set Variable" for the "Quest Creator" trigger. Set the "Variable" to KtM Quest, or whatever the name is for your variable, and set the second value to "(Last Created Quest)." This sets the variable that you made to your quest.

Step 6: Then create the action "Quest - Create Quest Requirement" for KtM_Quest with a suitable description of your choosing (What you have to do to complete the quest. In this tutorial, I will make the description, "Kill 3 Murloc Tiderunners.").

Step 7: Create a new trigger. In this tutorial, I will call it "Kill the Murlocs Completion." Also, for this tutorial, I'm going to give it the following events, conditions, and actions:

Kill the Murlocs Completion
Events
Unit - Murloc Tiderunner 0005 <gen> Dies

Unit - Murloc Tiderunner 0006 <gen> Dies

Unit - Murloc Tiderunner 0007 <gen> Dies

Comment: The events are for when any of your units die

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Murloc Tiderunner 0005 <gen> is dead) Equal to True

(Murloc Tiderunner 0006 <gen> is dead) Equal to True

(Murloc Tiderunner 0007 <gen> is dead) Equal to True

Comment: This makes sure all of the other Murlocs are dead

Then - Actions
Quest - Mark (Last created quest requirement) as Completed

Quest - Mark KtM_Quest as Completed

Comment: The above action is when the quest you made gets completed

Quest - Display to Player Group - Player 1 (Red) the Quest Completed message: |cffffcc00QUEST COM...

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Execution count of (This trigger)) Equal to 1

Comment: The above condition is when the trigger fires a first time.

Then - Actions
Quest - Display to Player Group - Player 1 (Red) the Quest Update message: |cffffcc00QUEST UPD...

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Execution count of (This trigger)) Equal to 2

Comment: The above condition is when the trigger fires a second time.

Then - Actions
Quest - Display to Player Group - Player 1 (Red) the Quest Update message: |cffffcc00QUEST UPD...

Else - Actions

(Note: The actions I've created that display quest messages like this one...

Quest - Display to Player Group - Player 1 (Red) the Quest Update message: |cffffcc00QUEST UPD...
are optional and tells the player when he has killed one of the Murlocs that are required for him to kill.)


With this setup, the trigger works absolutely fine, but wait, no reward!? You can't have a quest with no reward! And that leads into our final step.

Step 8: Right before my second set of If/Then/Else actions with the "(Evaluation Count of (this trigger))" conditions and under my first If/Then/Else action, create a simple reward like "Player - Add Property" and use this to add a small amount of gold. You could have better rewards such as giving the player an item of some sort. Just use "Item - Create" and make it whatever item you want and create the item at a certain region.


It's true that you could use a variable with your Hero and have the item be given straight to your Hero, but that would require more work and as I already stated, another variable.

Warning: This tutorial may contain a few leaks since I did not check it. Leaks really don't bother me since it seems like they never really seem to do anything to the game (e.g. make the game take really long to unload, slow the game down, etc.). Leaks supposedly do make the game take longer to unload, but it seems that if your computer runs WC3 pretty good and is a little over the recommended system requirements, than your fine. I'm sure this doesn't have that many leaks since I didn't use any "Wait" commands, "Elapsed Time", or "Periodic Event" events.


That's it! You did it. Your quest should now be complete!

#### Attachments

TutorialScrn01.jpg

 1 MB

 · Views: 7,128



TutorialScrn02.jpg

 420.9 KB

 · Views: 7,841



TutorialScrn03.jpg

 358.1 KB

 · Views: 6,879



TutorialScrn04.jpg

 396.4 KB

 · Views: 6,797



TutorialScrn05.jpg

 379.1 KB

 · Views: 6,750



TutorialScrn07.jpg

 491.4 KB

 · Views: 7,074



TutorialScrn08.jpg

 466.9 KB

 · Views: 6,504



TutorialScrn06.jpg

 165.3 KB

 · Views: 6,768




 Last edited: Jan 31, 2011

#### RazorHedgeFan

#### RazorHedgeFan

 Level 3

JoinedJan 3, 2011Messages14

Resources: 2

#### Resources
Maps 1Tutorials 1

Nice tutorial +Rep


But there's a problem ...


Don't use word "noob". More respectful is "Beginner"


Also you could tell them how to make trigger work with variables. Since it's more useful.

#### Slayer14

#### Slayer14

 Level 10

JoinedDec 12, 2010Messages569

Resources: 7

#### Resources
Maps 5Tutorials 2

OK, it's now a beginner tutorial instead a "noob" tutorial. Sorry for using the word "noob". I agree, it is a little disrespectful.

I may make another tutorial that explains the usage of variables in quests.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I think you should merge this tutorial with the other one. They both are for simple quests so they should be in the same post.

#### Slayer14

#### Slayer14

 Level 10

JoinedDec 12, 2010Messages569

Resources: 7

#### Resources
Maps 5Tutorials 2

I'm sorry to ask, but how would I do that?

#### RazorHedgeFan

#### RazorHedgeFan

 Level 3

JoinedJan 3, 2011Messages14

Resources: 2

#### Resources
Maps 1Tutorials 1

Well instead of doing another tutorial with variables you can merge it into one. Purgeand meant you shouldn't make another tutorial. You can simply add it to this tutorial.

#### Slayer14

#### Slayer14

 Level 10

JoinedDec 12, 2010Messages569

Resources: 7

#### Resources
Maps 5Tutorials 2

Could I get rid of the other one if I copied it's text and put it on this tutorial?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

You can copy the text and put it in this tutorial, and then I'll move the other one to the graveyard for you.

#### Slayer14

#### Slayer14

 Level 10

JoinedDec 12, 2010Messages569

Resources: 7

#### Resources
Maps 5Tutorials 2

OK, I've merged the two posts, you're free to move the other tutorial to the graveyard. Btw, is there a way to change the thread name? Just wondering because I changed the title, but the actual thread still has the old name.

#### Jagex

#### Jagex

 Level 4

JoinedJan 28, 2011Messages47

Useless tutorial, unless it's how to create a quest in JASS. Otherwise, you could might as well create an encyclopedia about events, conditions and actions.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Looks good. ~Approved.

#### Elf

E

#### Elf

Good only for beginners.. :/

#### Slayer14

#### Slayer14

 Level 10

JoinedDec 12, 2010Messages569

Resources: 7

#### Resources
Maps 5Tutorials 2

Yes, that's the purpose of this tutorial. It was designed to help only beginners. That's why the first posts title has the phrase "(Noob Tutorial)."

EDIT:

Oh, Purgeandfire, I noticed you edited my tutorial for me so the 1st and 2nd tutorial go together much better and it looks as if I made them that way. Thanks. It looks better that way.


 Last edited: Jan 31, 2011

#### Prozter

P

#### Prozter

thx alot it helped alot xD

#### DUTY OF RS

#### DUTY OF RS

 Level 12

JoinedMar 30, 2013Messages664

Resources: 8

#### Resources
Maps 6Templates 2

u missed something -.-

-Where can the player get the quest from????

You didnt write about that... from the unit that gives you the quest ;o

#### WretchedWyrm

#### WretchedWyrm

 Level 1

JoinedJan 27, 2014Messages2

This is pretty much my go to guide for anything quest related, thank you for posting this. One question though, do you know how to make repeating radiant quests? Like you go within range of someone, they tell you to go kill blah blah blah, return, complete it, then they give you another quest, but a random one, not in a predefined order?

#### James Raphael

#### James Raphael

 Level 4

JoinedMay 27, 2014Messages65

Can you make it repeatable please.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

James Raphael said:

 Can you make it repeatable please.


Click to expand...


this method wont work if you want it to be repeatable.

#### James Raphael

#### James Raphael

 Level 4

JoinedMay 27, 2014Messages65

okay bro...because recently someone just tell me how well good luck with your whatever your doing...

#### pizzalord22

#### pizzalord22

 Level 1

JoinedOct 28, 2015Messages3

good tut but

so i am a total "noob" your tutorial made me understand a lot and i thank you for that,

but i do not know how to do all the if(all conditions are true) do else stuff, which you did not explain in your tutorial.

Also how do i make my "farmer joe" give this quest to some one when they talk to him?


 Last edited: Oct 28, 2015

#### DaneTheBeast

#### DaneTheBeast

 Level 12

JoinedDec 11, 2014Messages665

Resources: 1

#### Resources
Maps 1

WretchedWyrm said:

 This is pretty much my go to guide for anything quest related, thank you for posting this. One question though, do you know how to make repeating radiant quests? Like you go within range of someone, they tell you to go kill blah blah blah, return, complete it, then they give you another quest, but a random one, not in a predefined order?


Click to expand...


Make a region around that unit, when you enter it trigger the quest. Then trigger the killing stuff, and then trigger when they return back to the NPC (Checking if they killed enough)


I hope I made sense

 You must log in or register to reply here.


### Similar threads

[GUI]
 How to Create a Quest That Uses Variables


Slayer14

Jan 26, 2011

Tutorial Archive

Replies3Views3K

Jan 26, 2011
Maker

 Quests


Bob27

Feb 23, 2006

Trigger (GUI) Editor Tutorials

Replies0Views36K

Feb 23, 2006
Bob27

 How to make a Find that Person Quest.


Dark Hunter1357

Apr 11, 2008

Trigger (GUI) Editor Tutorials

Replies4Views11K

Jun 25, 2008
Ghan_04

 Creating Easy Multi-User Quests


The_Big_S

Aug 15, 2007

Trigger (GUI) Editor Tutorials

Replies33Views21K

Nov 17, 2007
PurplePoot

[GUI]
 Make a quest tutorial


Chaosy

Aug 16, 2011

Tutorial Archive

Replies1Views4K

Nov 9, 2011
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