---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/must-have-campaign-ai-programming-techniques.302149/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Must Have Campaign AI Programming Techniques
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/must-have-campaign-ai-programming-techniques.302149/>
#### Tommi Gustafsson

#### Tommi Gustafsson

 Level 19

JoinedMay 26, 2017Messages448

Resources: 6

#### Resources
Tools 1Maps 2Tutorials 3

Hello!

I was writing a couple of AIs for a Warcraft III Campaign, and came up some very important techniques that have not been discussed in any of the tutorials I've seen.

1. Place your common AI code and globals to a custom common.ai

If you are programming a lot of custom code for the AIs and not using Blizzard's functions in common.ai, you may end up writing all the way the same code into many AI files. I came up with a good technique to avoid that. Instead of writing duplicate code, you should place all your common code into a custom common.ai file.

First, extract the original Scripts\common.ai from war3x.mpq

Place it into your working directory and rename it to common2.ai (or whatever you like)

Now, you may edit the common2.ai file as much as you like

Place your custom globals into the globals sectionsat the start of the file

Place your custom functions at the end of the file

Then, import it into your campaign and set its path to Scripts\common.ai

Now, Warcraft III loads your custom common.ai instead of the original file, and you get to share your code across all AI files and across all maps in the campaign.
Very handy!

2. Check AI errors with PJASS

PJASS is an excellent tool to spot errors in your AI. When you are writing an AI, it is a tremendous help, if you can use it to spot all errors before importing your AI file to your map.

Download pjass.exe from the above link. Be sure to get the latest version.

Place pjass.exe into your working directory.

Export common.j from war3x.mpq to your working directory.

Place all your custom AI files into your working directory.

Open Windows Powershell in your working directory (File Explorer -> File -> Open Windows PowerShell)

Run .\pjass.exe .\common.j .\common2.ai .\custom.ai

This checks both your common code in common2.ai and all your code in your custom.ai for errors (where custom.ai is the name of your custom ai file that you want to check for errors)
Great! No more starting Warcraft III to check if you made any syntax errors in your AI file!

Regards,

Tommi

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

There's another tool for syntax checking. Can't remember the name. No, it's not pJass.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Really useful trick! Thanks for sharing. Approved!

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

JassDemo is a decent alternative for pJass, just sayin'

Thanks for the useful informations though!

#### Zion Torres

#### Zion Torres

 Level 2

JoinedDec 14, 2020Messages2

Tommi Gustafsson said:

 Hello!

I was writing a couple of AIs for a Warcraft III Campaign, and came up some very important techniques that have not been discussed in any of the tutorials I've seen.

1. Place your common AI code and globals to a custom common.ai

If you are programming a lot of custom code for the AIs and not using Blizzard's functions in common.ai, you may end up writing all the way the same code into many AI files. I came up with a good technique to avoid that. Instead of writing duplicate code, you should place all your common code into a custom common.ai file.

First, extract the original Scripts\common.ai from war3x.mpq

Place it into your working directory and rename it to common2.ai (or whatever you like)

Now, you may edit the common2.ai file as much as you like

Place your custom globals into the globals sectionsat the start of the file

Place your custom functions at the end of the file

Then, import it into your campaign and set its path to Scripts\common.ai

Now, Warcraft III loads your custom common.ai instead of the original file, and you get to share your code across all AI files and across all maps in the campaign.
Very handy!

2. Check AI errors with PJASS

PJASS is an excellent tool to spot errors in your AI. When you are writing an AI, it is a tremendous help, if you can use it to spot all errors before importing your AI file to your map.

Download pjass.exe from the above link. Be sure to get the latest version.

Place pjass.exe into your working directory.

Export common.j from war3x.mpq to your working directory.

Place all your custom AI files into your working directory.

Open Windows Powershell in your working directory (File Explorer -> File -> Open Windows PowerShell)

Run .\pjass.exe .\common.j .\common2.ai .\custom.ai

This checks both your common code in common2.ai and all your code in your custom.ai for errors (where custom.ai is the name of your custom ai file that you want to check for errors)
Great! No more starting Warcraft III to check if you made any syntax errors in your AI file!

Regards,

Tommi


Click to expand...

Thanks for sharing

#### Barade

#### Barade

 Level 29

JoinedFeb 2, 2006Messages1,692

Resources: 5

#### Resources
Tools 1Maps 2Spells and Systems 2

Useful tips. I persionally use this Syntax Checker which has a GUI and allows choosing AI script and it requires Java: JASS Syntax Checker 0.1.7

If you check your common.ai you will get some duplicated code errors.

For my map I use multiple campaign AI and lots of shared code in a custom common.ai: wowr/ai at master · tdauth/wowr

 You must log in or register to reply here.


### Similar threads

 Melee AI Importing


Av3n

Dec 9, 2006

AI Tutorials

Replies4Views43K

Dec 29, 2006
Archian

Locked

 [JASS] AI Scripting


mckill2009

Jul 6, 2012

World Editor Help Zone

Replies2Views2K

Jul 8, 2012
mckill2009

Locked

 Structs in JASS AI?


Axis.Highpriest

Sep 6, 2023

Triggers & Scripts

Replies2Views1K

Sep 6, 2023
Axis.Highpriest

Locked

[AI]
 Let Campaign AI build expansions


Barade

Jan 3, 2023

Triggers & Scripts

Replies0Views1K

Jan 3, 2023
Barade

Locked

[AI]
 Pause campaign AI


Barade

Apr 14, 2024

Triggers & Scripts

Replies1Views1K

Apr 14, 2024
Footman16

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