---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/basic-weapon-removal.31486/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Basic Weapon Removal
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/basic-weapon-removal.31486/>
#### Kimbo

#### Kimbo

 Level 45

JoinedJun 3, 2005Messages6,946

Resources: 146

#### Resources
Models 18Icons 110Packs 3Skins 11Tutorials 4

This basic tutorial will teach you how to do simple weapon removal tasks (remove weapons from units) rather than requesting a bunch of them.

 Weapon Removal


 Difficulty:


Medium: 4/10

 Tools Required:


Wc3viewer

MdlxConv

Vertex Modifier

 1. Open your Wc3viewer :)


Look for the model you wish to disarm of its weapon (if you wish to find a TFT model, you will need to open the War3x.mpq file and look there "file>Open mpq") You can use the search tool to help you find the model: "View>Find in mpq."

 2. Extract the Model


 Now "Right click" on the model's name (its name) and select "Extract Mdx..."

 3. Converting the model to mdl


 Now Open MdlxConv and convert the model to .mdl.

 4. Removing the Weapon


 Now this is the part were many people make errors.

Open the file (the mdl version) using "Vertex Modfier.

Often the weapon is found on the first Geoset so type 1 or click okay when the 1 is visible.

Select the "add button" and select the vertex points of the weapon you wish to use.

This part may take a while and be frustrating for those who find searching difficult. Just examine the model. You can look at it in wireframe by viewing the model you extracted using the "Wireframe mode" button to toggle the wireframe view within wc3viewer.

Delete the selected vertex (hopefully its the model, if not, press undo "Ctrl+Z". Or either reopen the model (if not saved when you made the error).

 5. Finishing up


 Now save the model and use MdlxConv to convert it to mdx by opening the mdl version (that you just edited).

Now test the model in wc3viewer and in Warcraft III to see if you have it right. Enjoy!


 Last edited by a moderator: Jul 24, 2013

#### Kimbo

#### Kimbo

 Level 45

JoinedJun 3, 2005Messages6,946

Resources: 146

#### Resources
Models 18Icons 110Packs 3Skins 11Tutorials 4


 PurgeandFire111 said:



 Nice tutorial. But I have a question:


1.) How does this have an advantage over alphaing out the weapon by skinning?


Gj on the tutorial though, +rep


Click to expand...

Not all models set their mdl material data to transparent, thus alphaing will only work on 1/2 or so of the models.. just a guess.


And skins are sometimes bigger than models themselves.


 DredLord2 said:



 Is there a way to convert the mdx, or mdl file into a tga or bmp, or anything othre than those 2?


Click to expand...


No..mdx is the compressed version of mdl used within wc3.


A 3d model is not a picture file, its a 3d model file.


 Kitabake said:




 Dark Hunter1357 said:



 Why when i removed the weapon on the attached file and i removed the other weapon (he had two, one for death and the other one for all other animations), and i import it, the world editor crashes??


Its. because i romeved a geoset, now i know.


Click to expand...


Removing geosets do not cause the model to crash if you also remove all geoset animations related to that geoset.


It's very easy, just select geoset animation manager in magos and remove all geoset animations that refer to geoset "None"


Click to expand...


 Ardnived said:



 the programs that are linked to this topic is for windows only, are there mac versions i can use?


Click to expand...

No, there is no mac versions to my knowledge.


 Wolverabid said:



IMPORTANT NOTICE - Read Before Posting!

Werewulf said:

 The tools are linked already. =/


and lombre, this thread about a tutorial not vertex modify.


Click to expand...

Since the tutorial forums have been open for posting this problem has occasionally cropped up: instead of standing alone as an instructional document the tutorials threads become infested with with spam.

 Stay tuned: I may soon make some changes to prevent future spamming of our valuable tutorial resources.

Click to expand...


 Werewulf said:



 Seriously, this is the last time people post crap here, anymore and I'm neg repping.


Click to expand...


Okay, im serious. Next unrelated post gets a neg rep and the thread closed.


 Last edited by a moderator: Sep 2, 2007

#### SuPa-

#### SuPa-

 Level 21

JoinedMar 18, 2007Messages2,237

Resources: 38

#### Resources
Models 23Icons 5Skins 6Maps 1Tutorials 3

GJ, this's helpful for all those people who go "OMGZERZ CAN U G1v m3 WEPUNLES MEDOLS?!?!"


Although most people don't bother themselves with the 2 minutes it takes to read the tutorial and then remove the weapon :-/

#### Deep Sea Kraken

#### Deep Sea Kraken

 Level 15

JoinedDec 29, 2006Messages985

Resources: 10

#### Resources
Models 9Icons 1

Ive also found another way to do this, using Mago's model editor - you open the model you want (.mpq browser) then go to: windor -> model editor, then select all of the polies (at least i think they are polies) or the red wireframe dots on the weapon then go to Transformation -> Scaling then set the X Y and Z values to 0 and the weapon will disappear, or whatever part of the model you used.


But using this can lead to wierd things happening with your model like wierd pieces of flat color moving around (happened to me when i tried to remove 2 heads from a hydra), but i do not think you will get this problem with weapons.


Hope that was not off topic.

#### lordolly

#### lordolly

 Level 2

JoinedSep 5, 2007Messages14

VertexModify

I tryed this but my VertexModify programme won't op-en there is a missing data link or something i DL the VertexModify from the link you provided where else can i find this programme or is there something i can do to make mine work?

#### Kimbo

#### Kimbo

 Level 45

JoinedJun 3, 2005Messages6,946

Resources: 146

#### Resources
Models 18Icons 110Packs 3Skins 11Tutorials 4

Screw it. closed.

Status
 Not open for further replies.


### Similar threads

 How to add/remove hero glow with modelling


Alok

Feb 25, 2017

3D Modeling Tutorials

Replies7Views11K

Feb 2, 2021
Gumberbunbder

 Basic Animation Transfers


Xarwin

Mar 20, 2007

3D Modeling Tutorials
2

Replies70Views87K

Jan 31, 2024
Vertigo2604

 remove weapons with 3 tools


ikillforeyou

Sep 21, 2008

Tutorial Archive

Replies2Views5K

Oct 23, 2008
Hawkwing

Locked

 How To Make One Model Use Many Skins


oz02

Jun 4, 2004

3D Modeling Tutorials

Replies3Views64K

Sep 19, 2007
Wolverabid

 Create Models only with free tools


D a w n

Nov 16, 2008

3D Modeling Tutorials
234

Replies174Views87K

Jun 10, 2014
clockwork2

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