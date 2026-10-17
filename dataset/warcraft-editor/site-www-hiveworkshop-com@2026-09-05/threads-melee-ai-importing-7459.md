---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/melee-ai-importing.7459/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Melee AI Importing
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/melee-ai-importing.7459/>
#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

This is about how to import melee AI's (Artificial Intelligence) to melee maps. Its pretty easy. There is the .wai files and .ai files. Theres a example map that can be downloaded.

The Difference


The difference is that .wai files are edited using the WE AI editor which could be time cosuming clicking all over the place, also if you wanted an custom AI you will have to import your maps object data. After fnishing the AI the editor will generate an .ai file for you, which contains the final AI that will be used by the map.


.ai files can also be manually edited with Notepad/JassCraft. They're written in JASS and allow for much more powerful AI scripts than possible in the GUI editor (see the AMAI project). However it isn't always necessary to write the JASS from scratch. You can also generate an .ai file with the GUI edtior and later add a few teweaks manually.

AI importing

Contents


1. Importing the AI


2. Starting the AI


3. Other things


4. Seeing if it works

1. Importing the AI


How do I import the .wai or .ai file?


1. Open the Import Manager.


2. Click on the import button.


3. Find the AI you want to import.


4. Import it.


In my case the AI's I've imported are the Blizzard ones that came along. (WyrmMonger.)

2. Starting the AI


How do I start the .wai or .ai file?


1. Remove the Action from the default trigger:

Actions
Melee Game - Run melee AI scripts (for computer players)

2. Now create a new trigger called: WyrmMonger Initialization for example.


3. Add these

WyrmMonger Initialization
Events
Time - Elapsed game time is 0.00 seconds

Conditions
(Race of Player 2 (Blue)) Equal to Undead

Actions
AI - Start melee AI script for Player 2 (Blue): war3mapImported\WyrmMonger.ai

3. Other


The other triggers if you are only using 1 race.


1. These are the other triggers.

Normal Initialization
Events
Time - Elapsed game time is 0.00 seconds

Conditions
(Race of Player 1 (Red)) Not equal to Undead

(Race of Player 2 (Blue)) Not equal to Undead

Actions
Melee Game - Run melee AI scripts (for computer players)

4. Seeing if it works


Now to see if the .wai or .ai file it works on the map.


1. Open the Frozen Throne, then the map you imported the AI on.


2. Make the computer to the race on what melee AI you made for it. (WyrmMonger is Undead.)


Also note that the .wai file only contains the GUI definitions of your AI. It isn't required by the game, only by the editor. So after having finished your map, and imported the .ai file, you can remove the .wai file from your map. But you should consider keeping a backup of that .wai somewhere on your harddrive in case you'd like to make edits later.


It should work,but if it doesn't you might of have setted the wrong race or for the condition. The method is very similar. Remember its only works for melee related maps.


-Av3n

#### Attachments

BootyBayAI.w3x

 140.9 KB

 · Views: 1,912




 Last edited: Dec 29, 2006

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Good work on the tutorial


When you get your comp fixed and attached the demo map you speak of, then I will approve this.


Cheers,


~Archian.

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

And after that I'll post my AI Editor tut that i wrote from Wc3c. (Im actually Av3n)


-Av3n

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

Attached*


-Av3n

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Thank you


The trigger tags have some *technical difficulties* atm, they will be fixed soon enough.


~Tutorial Approved

 You must log in or register to reply here.


### Similar threads

Locked

 Newgen Editor doesn't import AI properly.


Disasterpiece512

May 3, 2010

Warcraft Editing Tools

Replies0Views2K

May 3, 2010
Disasterpiece512

Locked

 AI files don't import properly in Newgen


Disasterpiece512

May 4, 2010

World Editor Help Zone

Replies5Views2K

Dec 25, 2022
Pyrogasm

Locked

[General]
 AI refuse to work


Chaosy

May 10, 2015

World Editor Help Zone

Replies13Views4K

May 12, 2015
Shar Dundred

Locked

 AI to WAI


myronbeg

Jun 7, 2011

World Editor Help Zone

Replies5Views8K

Jun 8, 2011
myronbeg

 Must Have Campaign AI Programming Techniques


Tommi Gustafsson

Jan 8, 2018

AI Tutorials

Replies5Views9K

Mar 31, 2023
Barade

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