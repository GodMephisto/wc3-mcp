---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-create-new-unit-using-existing-textures.252820/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Create New Unit Using Existing Textures
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-create-new-unit-using-existing-textures.252820/>
#### GaLaxY_256

#### GaLaxY_256

 Level 13

JoinedMar 21, 2014Messages893

Resources: 8

#### Resources
Models 2Icons 3Tutorials 3

How to create new unit using existing textures

By Love Seeker

About:

This tutorial will explain you how to make new units that have same model as original but with different texture! They do not require any custom texture and look nice (well, this depends on you )

Requirements:

Softwares:

Magos' Model Editor

MDX Pather
Knowledge:

Well, at the moment, you should only know how to import models into the world editor and that's all.

Let's get started!

Berserk Troll Axe Thrower
This unit will cover all units of this tutorial. Any units below this unit will follow the same step with paths only being difference.

Exporting the model

Step 1 - Open Magos' Model Editor and go to Windows->MPQ Browser

Step 2 - Once you get to MPQ Browser, go to File and select the MPQ you are going to export model from (the MPQ you will extract varies with unit), but in this unit I'll be opeing war3.mpq

Step 3 - When MPQ is opened, go to Units\Creeps\ForestTroll. Well, you will see the two .mdx files; one for model file and the other portrait.

 Right-click on ForestTroll.mdx file and click Export. You will be prompted to choose directory to export, I usually export them in the new folder. Do the same for portrait file..

Now you have just exported the unit's model!

Changing the texture

Step 4 - Open MDX Pather, and open the exported ForestTroll.mdx file (or whatever name you gave it).

Step 5 - After opening the file you will see "Units\Creeps\ForestTroll\ForestTroll.blp". Click it so it will appear in the empty box above it and we can edit the texture path.

Step 6 - We will be changing the forest troll texture to berserker one. So change "Units\Creeps\ForestTroll\ForestTroll.blp" to "Units\Orc\Headhunter\BeserkHeadhunter.blp". Do the same for portrait file, too.

Final Step - Open Magos' Model Editor and from there, open your "texture-changed" model and see if it works. If you get the unit model like me, congrats you have just finished Berserker Troll Axe Thrower(or name of your choice)! Import them into your map and enjoy!

AS I MENTIONED ABOVE, THE UNITS BELOW THIS TEXT WILL HAVE THE SAME STEPS TO DO, THE ONLY DIFFERENCE IS MODEL PATHS TO FIND, TEXTURES TO CHANGE IN MDX PATHER. SO I'LL BE PROVIDING MODEL PATHS, CHANGE WHICH TEXTURE TO WHICH TEXTURE AND THE UNIT'S SCREENSHOT IN MAGOS'.

TROLL SPEAR THROWER (4 Versions)

A short note on these models: These spear throwers require alternate tag(don't know what I should say so let's say tag) to function correctly..

Forest Troll Version

Which MPQ : war3x.mpq

Model Path : units\orc\headhunter\headhunter_v1.mdx and its portrait file

Change texture from : units\orc\headhunter\beserkheadhunter.blp

To : units\orc\headhunter\foresttroll.blp

Forest Troll Trapper Version

Which MPQ : war3x.mpq

Model Path : units\orc\headhunter\headhunter_v1.mdx and its portrait file

Change texture from : units\orc\headhunter\beserkheadhunter.blp

To : units\creeps\foresttrolltrapper\foresttrolltrapper.blp

Ice Troll Version

Which MPQ : war3x.mpq

Model Path : units\orc\headhunter\headhunter_v1.mdx and its portrait file

Change texture from : units\orc\headhunter\beserkheadhunter.blp

To : units\creeps\icetroll\icetroll.blp

Dark Troll Version

Which MPQ : war3x.mpq

Model Path : units\orc\headhunter\headhunter_v1.mdx and its portrait file

Change texture from : units\orc\headhunter\beserkheadhunter.blp

To : units\creeps\darktroll\darktroll.blp

ORC AXE THROWER

Which MPQ : war3.mpq

Model Path : units\demon\ChaosOrcRange\ChaosOrcRange.mdx and its portrait file

Change texture from : units\demon\ChaosOrcRange\ChaosAxeThrower.blp

To : units\orc\grunt\grunt.blp

LIVING HUNTRESS STATUE

Which MPQ : war3.mpq

Model Path : units\NightElf\Huntress\Huntress.mdx and its portrait file

Change texture from : Textures\Sentinel.blp

To : Doodads\Ashenvale\Props\SentinelStatue\AshenStatueSmall.blp

UNDERGROUND ACOLYTE

Which MPQ : war3.mpq

Model Path : units\Undead\Acolyte\Acolyte.mdx and its portrait file

Change texture from : Textures\Acolyte.blp

To : Units\Other\DalaranReject\DalaranRejectAcolyte.blp

Well, that's list of models as possible as I can present to you. I hope you get the same model as mine and enjoy making them. Also feel free to feedback, ask questions without hesitate. Good Luck!


 Last edited: May 4, 2018

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Lacks structure.

try looks at other tutorials,

https://www.hiveworkshop.com/forums...ls-279/creating-custom-error-messages-232494/

https://www.hiveworkshop.com/forums...als-279/complete-concise-gui-tutorial-111863/

https://www.hiveworkshop.com/forums/tutorial-submission-283/faq-wehz-228993/

#### noob

#### noob

 Level 14

JoinedSep 28, 2011Messages968

Resources: 5

#### Resources
Maps 4Tutorials 1

Why not add whips with ReplaceableTextures\Weather\RainTail.blp as id2 texture they look pretty good.

#### WerBackIII

#### WerBackIII

 Level 14

JoinedJul 27, 2007Messages789

Resources: 11

#### Resources
Skins 5Maps 6

noob said:

 Why not add whips with ReplaceableTextures\Weather\RainTail.blp as id2 texture they look pretty good.


Click to expand...


Can you post a screenshot of an example?

#### noob

#### noob

 Level 14

JoinedSep 28, 2011Messages968

Resources: 5

#### Resources
Maps 4Tutorials 1

As seen in the world editor.

e:did not knew the orthographic error I made sorry.

The problem is maybe it is too much fast animated (it looks like if it were pulsating 5 times a second)

#### Attachments

WC3ScrnShot_061114_232055_01.jpg

 115 KB

 · Views: 610



#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

This tutorial will really benefit from screenshots of the final results. If I were new to modding, I wouldn't know what this tutorial does or is used for.

As such, you should probably organize your tutorial. Start off with an introduction stating precisely what the tutorial's purpose is, and then go through the steps in detail. If you explain the process once in detail, you won't have to repeat yourself! And screenshots always help users follow along. Always assume that the user is unfamiliar with the program. If you simply say "extract the model", there is a chance that the user will be lost and confused. However, if you say "go to File -> Save As, and save the model", then the user has a better idea of what to do. Similarly, they might not know how to get a preexisting model opened in magos' model editor. Sometimes you can mention that the tutorial requires "some knowledge of X program", but it isn't the best habit since it casts off new modders (who are more likely to be the ones reading your tutorial).

This has potential to be a neat little trick, and it covers some solid concepts to know such as changing textures used and using ingame textures to your advantage, but it needs work before it can be approved.

P.S. Try to avoid polls in tutorials if you can.

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Yeah, definitely screenshots. I actually want to be interested in what you have to say... But I'm an inexperienced-enough modeler that I have no idea what's going on. :<

#### hiphop4eva

#### hiphop4eva

 Level 8

JoinedJul 8, 2014Messages433

Resources: 1

#### Resources
Maps 1

Umm, I wanna see pictures.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

this needs better formatting, and especially images.

#### GaLaxY_256

#### GaLaxY_256

 Level 13

JoinedMar 21, 2014Messages893

Resources: 8

#### Resources
Models 2Icons 3Tutorials 3

Tutorial updated! Images are added too! With better formating and some removed units

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Looks good! Might want to double-check some of the spelling for the paths; I didn't see them all but I noticed "Grunt" was "Grant".

#### GaLaxY_256

#### GaLaxY_256

 Level 13

JoinedMar 21, 2014Messages893

Resources: 8

#### Resources
Models 2Icons 3Tutorials 3

Kyrbi0 said:

 Looks good! Might want to double-check some of the spelling for the paths; I didn't see them all but I noticed "Grunt" was "Grant".


Click to expand...

Wow thanks sir!

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Love the new format and the screenshots, they look fucking awesome. Thanks so much for updating it.

Approved!

#### GaLaxY_256

#### GaLaxY_256

 Level 13

JoinedMar 21, 2014Messages893

Resources: 8

#### Resources
Models 2Icons 3Tutorials 3

PurgeandFire said:

 Love the new format and the screenshots, they look fucking awesome. Thanks so much for updating it.

Approved!


Click to expand...

Thank you sir! It took me 3 years to take those screenies (meh, just kidding )

E: Sir PnF, if possible, can you please change the thread name according to haeder? Thanks

E2: Found it but where can i see edit history? I am not in touch with new Hive


 Last edited: May 5, 2018

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Updated the title! @Love Seeker

#### prinze janus

#### prinze janus

 Level 2

JoinedJun 24, 2018Messages7

It wont work for me

#### GaLaxY_256

#### GaLaxY_256

 Level 13

JoinedMar 21, 2014Messages893

Resources: 8

#### Resources
Models 2Icons 3Tutorials 3

Can you describe what's the error? If possible, proper screenshot would help

 You must log in or register to reply here.


### Similar threads

 Change Model Texture Using Alternate Animations


Flux

Apr 3, 2015

3D Modeling Tutorials

Replies36Views14K

Apr 2, 2017
gusanomental

Locked

 How To Make One Model Use Many Skins


oz02

Jun 4, 2004

3D Modeling Tutorials

Replies3Views64K

Sep 19, 2007
Wolverabid

 Creating Ghosts


Daelin

Jul 14, 2005

3D Modeling Tutorials

Replies10Views27K

Aug 14, 2023
DREADMAN

 Add Multiple Skins for a Single Model


Sylvie

Feb 17, 2015

3D Modeling Tutorials

Replies34Views18K

Mar 20, 2021
Em-Rock

 Exporting files from a MPQ.


Deolrin

May 14, 2009

Misc/General Mapping Tutorials

Replies10Views19K

Dec 8, 2009
Newbestyle

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

3D Modeling Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top