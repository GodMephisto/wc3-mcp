---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-easily-post-triggers.163285/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Easily Post Triggers
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-easily-post-triggers.163285/>
#### MortAr

#### MortAr

 Level 19

JoinedFeb 25, 2009Messages2,004

Resources: 11

#### Resources
Icons 6Packs 1Spells 2Tutorials 2

How to easily post triggers

###
 Table of Contents

Top

Step 1 - choosing the right trigger

Step 2 - copying your trigger

Step 3 - pasting your trigger into the forum

Step 4 - additional features

Step 5 - posting JASS scripts

End

Step 1

- choosing the right trigger -

I'm guessing most of you have already learned how to trigger in GUI, but, anyway. Finding the right trigger will mean - simply open your map, and then open the "Trigger Editor" or simply press (F4), and navigate to the folder where the trigger is and select it. After finding the right trigger that needs to be copied, you can proceed to the next step.​

Back to top

Step 2

- copying the needed trigger -

After finding and selecting the right trigger, you must copy it in order to post it. To do this, follow these little steps - When you've selected the trigger, in the panel where all the Events/Condition/Actions are displayed, there's a little white icon with the name of the trigger. Click there to select it, then RIGHT click the mouse and a menu will pop-up. From this menu, simply select "Copy as Text" and don't touch anything else, for now.​

Back to top

Step 3

- Pasting the trigger onto the forums -

After you are done with step 2, you can proceed with this one. Here is the main deal of how to actually post your trigger on the forum. This can be done very easily by following these simple steps - Open up the forum, and after that, go to the RIGHT section of the forums (usually that will be "World Editor Help Forum" or the "Triggers & Scripts" forum). When you finally have found the place where you want to post your trigger, make a new thread or post and click the button "Go Advanced". After that, you will see an empty field for your text - here you'll paste your trigger (Note: keep in mind that your trigger is currently copied to the clipboard, which means, if you copy any other text, you'll lose the trigger text. If that happens, go back to step 2 before proceeding). Now the main part of this tutorial - finally posting the trigger. There's two ways of posting the trigger into the forum field. The first method is by pressing this

 little icon which will automatically wrap the [trigger][/trigger] tags into the field for the text. After the tags are parsed, click between them and press "CTRL+V" or RIGHT click with the mouse and click "PASTE". After that, a huge text will appear into the text-box .. this is your trigger's actions , events, and conditions. When this is done, you can freely post the thread or the post and show your trigger to the other peoples.​

Back to top

Step 4

- Additional features when using the trigger tags -

Maybe you've noticed that the browser window will not shrink with the trigger-tags, that means you definitely have to make some changes. Most of people might yell at you and tell you to use the [hidden][/hidden] tags, which are mostly the greatest thing ever made for situations like this. The "hidden" will simply hide the huge area of text of your trigger-tags so the page won't get filled with text and make it look ugly. Here is an example how this is going to look like:

Melee Initialization
Events
Map initialization

Conditions

Actions
Melee Game - Use melee time of day (for all players)

Melee Game - Limit Heroes to 1 per Hero-type (for all players)

Melee Game - Give trained Heroes a Scroll of Town Portal (for all players)

Melee Game - Set starting resources (for all players)

Melee Game - Remove creeps and critters from used start locations (for all players)

Melee Game - Create starting units (for all players)

Melee Game - Run melee AI scripts (for computer players)

Melee Game - Enforce victory/defeat conditions (for all players)

You now can see that this text is hidden, thus is doesn't shrink the browser's window nor filling the post with text.

Note: In order to use the [hidden][/hidden] tags you need to enter an valid title for the hidden-box.

To do that, simply write this: [hidden="Write your title here"][/hidden]. This would look exactly like this:

OHAI

Additional bbcodes and such can be found here.​

Back to top

Step 5

- posting JASS scripts -

Hey! Lets not forget the scripts here... With them, its even easier to post in forums. Simply, find the trigger/script you want to post, select all the text inside, and then press "CTRL+C" or "RIGHT" click with the mouse and click "COPY". Now you can proceed to the forums (follow step 3 if needed) and use the [code=jass][/code] tags, placing the text between them with "CTRL+V" or "RIGHT" click with the mouse and click "PASTE". An example of a JASS function can be viewed here:

 JASS:

function Test takes nothing returns nothing
 call TriggerSleepAction ( 1.00 )
endfunction

Now please notice that, here we don't need to use the "hidden" tags in order to hide the text for better readability, because the tags has a hiding function by itself.

If you want to just show someone an simple function or a single row of the code, you can use the [icode=jass][/icode] tags.

Example of the tags can be viewed here:

function Test takes nothing returns nothing

call TriggerSleepAction ( 1.00 )

The [icode=jass] tags allow you to post things of a simple line or word without having to return to the next line. For example: return won't skip to the next line.

​

Back to top


 Last edited by a moderator: May 2, 2010

#### Darkness-4ever

#### Darkness-4ever

 Level 9

JoinedNov 4, 2007Messages932

I was unaware there were people in need of a tutorial of this magnitude to correctly post triggers in the forums, all it took me was a couple of trial & errors to get the hang of it.

#### MortAr

#### MortAr

 Level 19

JoinedFeb 25, 2009Messages2,004

Resources: 11

#### Resources
Icons 6Packs 1Spells 2Tutorials 2

I did this tutorial not only for the people that do not know how to post triggers at all.. It's for everyone that do not know to properly post triggers.. because I've seen a lot of threads with peoples asking for help without stating where is the problem or is it a trigger or a JASS scripts.

So, the tutorial is for everyone that need a little bit help when they need to post trigger(s) or JASS script(s).

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

There is nothing wrong with the tutorial except for the grammar which I've fixed. I know it isn't very useful, but it is a nice thing to link back. For example:


[situation]RandomUser: how do i post my triggers help

OtherUser: Read this tutorial:

https://www.hiveworkshop.com/forums/tutorial-submission-283/how-easily-post-triggers-163285/[/situation]


Thus, ~Approved.

#### MortAr

#### MortAr

 Level 19

JoinedFeb 25, 2009Messages2,004

Resources: 11

#### Resources
Icons 6Packs 1Spells 2Tutorials 2

PurgeandFire111 said:

 There is nothing wrong with the tutorial except for the grammar which I've fixed. I know it isn't very useful, but it is a nice thing to link back. For example:


[situation]RandomUser: how do i post my triggers help

OtherUser: Read this tutorial:

https://www.hiveworkshop.com/forums/tutorial-submission-283/how-easily-post-triggers-163285/[/situation]


Thus, ~Approved.


Click to expand...


Thanks for fixing grammar, I was actually planning to do it someday since I knew I had some mistakes done.


And, hey thanks for the approval.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Moving to GY in favor of Rheiko's tutorial, since it is updated for hive 2 and has some screenshots to boot.

#### Masky

#### Masky

 Level 5

JoinedMar 16, 2013Messages108

usseful for me <3 thanks

#### 3ICE

#### 3ICE

 Level 5

JoinedJul 4, 2007Messages84

PurgeandFire said:

 Moving to GY in favor of Rheiko's tutorial, since it is updated for hive 2 and has some screenshots to boot.


Click to expand...

Link How To Post Your Trigger (Took me a while to find, even with advanced Google-Fu.)

 You must log in or register to reply here.


### Similar threads

 How To Post Your Trigger


Rheiko

Jul 13, 2016

Misc/General Mapping Tutorials

Replies6Views11K

Aug 1, 2016
IcemanBo

 How to Copy & paste your triggers onto the Hiveworkshop


waveseeker

Mar 28, 2011

Tutorial Archive

Replies13Views5K

Apr 6, 2011
waveseeker

 How to: Create Professions easily


duderock101

Sep 18, 2009

Tutorial Archive

Replies6Views2K

Sep 22, 2009
Septimus

 How to replace a unit with a new one using a region.


Cobbot

Dec 30, 2008

Tutorial Archive

Replies12Views2K

Jan 5, 2009
Slaydon

N

 How to add flight to any unit


No Good Namez

Apr 27, 2008

Tutorial Archive

Replies2Views1K

Apr 28, 2008
PurplePoot

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