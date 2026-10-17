---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-make-xy-quad-particles-emitter-and-rain-drop-effect-tutorial.221890/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Make Xy Quad Particles Emitter and Rain Drop Effect Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-make-xy-quad-particles-emitter-and-rain-drop-effect-tutorial.221890/>
#### Anvil

#### Anvil

 Level 13

JoinedJun 8, 2011Messages472

Resources: 10

#### Resources
Models 7StarCraft II Resources 1Tutorials 2

Hi all, after trial and error I found how XY Quad option works and since there wasn't a tutorial I decided to make one.

First of all, what is XY Quad?

When you create a particle emitter, you'll see something like this:

If you check the XY Quad option, this will make the particles appear only in the x/y axis ( like a flat texture ), you get this:

Also the particles don't stay facing the camera ( Billboarded ), like shorelines in blizzard's waterfalls and other models.

Important Values in XY Quad Particles are 2 ( to appear in the editor and in game ):

Speed and Latitude

Speed: The value can not be 0, the minimum possible value is 0.002 but in the game you can see the particles are slow and choppy, so I recommend a value from 0.5 to 1 or above. (You can use negative speed)

Latitud: The value can not be 0, the minimum possible value is 0.06, from 0.08 to 0.06 like with the speed, in the game the particles are slow and choppy, recommended values from 0.5 to 1 or above. (You can use negative latitud)

Things that you need to follow the tutorial:

War 3 Model Editor by Magos / Download here

1) Open the program, go to File/New.

2) Now, you need the texture to make the rain drops so, go to Windows/MPQ Browser.

In the popup window click File and then press the war3.mpq or war3x.mpq file.

Note: If you don't have the program in the same directory that you have instaled warcraft III you will get an error. The solution, put the program in the same directory or

open the .mpq file using File/Open MPQ Archive, find the .mpq file in the game directory.

3) When you open the .mpq file you will see some folders inside, open war3.mpq go to the textures folder and find this file:

Right-click on the texture, another popup window appear on the screen, click "Use as texture". Close this window.

4) How to make a particle emitter, go to Windows/Node Manager in a blank area in the new window Right-click and then "Create particle emitter 2"

5) Now, right click in the particle emitter that you created, select edit node. Here you need to put a "Z" value, because if you left 0, the particles collide with the floor and this has an ugly look. Put the value like 5. Close this window.

6) After edit the particle emitter, make double-click in the particle emitter or right-click / edit to edit the particles properties.

Fill the values like me, remember the obligatories values for make a XY Quad emitter are speed and latitude, this values can't be minor than 0.002 and 0.06.

7) When you finish with this, close the node manager window, go to Windows/Sequence Manager, Right-click in a blank area in the new window and select "Create New"

Fill the values like me:

8) When you done, close the sequence manager, go to File/Save As, and save your model.

The only thing you need now is import it in to the map.

Credits to Magos for War 3 Model Editor.

I attached the file I created and used for this tutorial.

Sorry for my grammar, English is not my first language.

I hope this tutorial helps someone.

#### Attachments

01.jpg

 56.8 KB

 · Views: 1,732



03.jpg

 44.2 KB

 · Views: 1,681



04.jpg

 127 KB

 · Views: 1,692



05.jpg

 167.1 KB

 · Views: 1,837



06.jpg

 158.2 KB

 · Views: 1,690



07.jpg

 207.4 KB

 · Views: 1,651



08.jpg

 127.9 KB

 · Views: 1,665



09.jpg

 698.8 KB

 · Views: 1,979



RainDropsTest.mdx

 1.1 KB

 · Views: 335



particles00.jpg

 59.4 KB

 · Views: 1,579



particles01.jpg

 57.5 KB

 · Views: 1,622




 Last edited: Sep 5, 2012

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Man, your timing couldn't have been better, I've had this exact problems for a few weeks

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

So what does editing the XY Quad do for the particle? Perhaps add a little section in the beginning to describe what it does. I read the tutorial, and it is definitely approvable, but I don't understand quite what it does, so I figure others might feel the same.

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Well, really, if anybody reads this tutorial, they will know well enough what it does. XY Quad in wc3 basically means that the particle doesn't bilboard the camera, but the sky. It is that simple. This is not always a desired effect, but when it is, it (used to be) hard to pull off because nobody could figure out what's wrong.

#### Anvil

#### Anvil

 Level 13

JoinedJun 8, 2011Messages472

Resources: 10

#### Resources
Models 7StarCraft II Resources 1Tutorials 2

PurgeandFire111 said:

 So what does editing the XY Quad do for the particle? Perhaps add a little section in the beginning to describe what it does. I read the tutorial, and it is definitely approvable, but I don't understand quite what it does, so I figure others might feel the same.


Click to expand...

I understand what you're saying, I'll explain what is XY Quad.

Basically, it's what HappyTauren says.

Edit:

- Added an introduction to XY Quad particle emitter.

- The images are now in hidden tag

Let me know if it needs something else.


 Last edited: Aug 29, 2012

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Well, Purge, you can approve it, I can vouch for it

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great! Thanks guys for the extra info!  Approved.

#### JesusHipster

#### JesusHipster

 Level 35

JoinedSep 19, 2011Messages2,119

Resources: 70

#### Resources
Models 62Icons 7Skins 1

It's nice for bewbies and it's well written, good job)

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

RetroSexual said:

 It's nice for bewbies and it's well written, good job)


Click to expand...


I think you might want to edit this post.

#### UgoUgo

#### UgoUgo

 Level 25

JoinedNov 27, 2012Messages686

Resources: 49

#### Resources
Models 24Icons 22Skins 1Maps 1User Interface 1

Finally, my years of problem is solved.... Only if I have seen this earlier... Everything could be much more easier! Thanks for this tutorial anyway

#### Veronnis

#### Veronnis

 Level 14

JoinedJul 19, 2010Messages309

Resources: 18

#### Resources
Models 3Icons 1Skins 5Maps 4Reforged HD Skins 4Sounds 1

After a half hour of :"Why is this invisible?" I typed this in the Hivesearch and found this tutorial. I had everything right but the speed. Thanks so much!

 You must log in or register to reply here.


### Similar threads

 Particle Emitters 2


Vinz

Dec 18, 2020

3D Modeling Tutorials

Replies19Views14K

Mar 13, 2026
OVOgenez

 Teamcolor Particles


Anvil

Nov 28, 2012

3D Modeling Tutorials

Replies4Views7K

Dec 19, 2012
Anvil

 Scratch-Made Weapon Attachments with Particle Emitters


Fabioz

Apr 8, 2008

3D Modeling Tutorials
2

Replies52Views42K

Oct 13, 2010
Ikithe

 Classical Ribbons vs Particles Ribbons


Daelin

Nov 21, 2006

3D Modeling Tutorials

Replies0Views25K

Nov 21, 2006
Daelin

 Model Editing


Fabioz

Jan 18, 2009

3D Modeling Tutorials

Replies45Views66K

Oct 10, 2021
KaiZen7th

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