---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-a-dissipate-animation-using-magos-war3modeleditor.32576/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making a Dissipate Animation Using Magos War3modeleditor
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-a-dissipate-animation-using-magos-war3modeleditor.32576/>
#### Callahan

#### Callahan

 Level 37

JoinedMar 28, 2004Messages1,613

Resources: 132

#### Resources
Models 108Icons 12Packs 2Skins 2Maps 5Spells and Systems 2Tutorials 1

How to make a dissipate animation from a decay flesh animation.


It is very simple, in fact you don't need an animating program to make a dissipate animation!

 Difficulty


Easy: 3/10

 You need :


Magos War3ModelEditor

 Little Glossary


Parent: A parent has a child which depends upon the parent.


Parents have a +/- icon that you can click to show/hide their child.

Child: A child is dependent on its parent.


Step 1.
Open War3ModelEditor.

Open your model.

Click to sequence manager>Right click to Decay flesh click to Edit rename it to Dissipate and then


set the interval to 2000, meaning:


From : x


To : x + 2000

Example


From :33333


To : 35333

Step 2.
Click to windows>node manager.

Create a new bone and call it "Mother" for example, and move it to the top.

Right click to your new bone called Mother and click Move left until it's at the maximum left.

Make your new bone Mother the parent of every others bone and helper. This means right click to highest parent bone (or helper) and Move right usually you just have to make the helper called Bone_Root the child of the bone you've created.


Or if there is no Bone_Root you have to spot parents bones or helper (highest parent is usually a helper) by hiding every child by clicking the +/- icon to hide every child so click on every " - " to hide every child.


Then you got the highest parent bone.So then move it right so that it become the child the the bone Mother.

Right click on Mother > click to Edit Node click to Translation


I assume that you have previously checked your new dissipate sequence interval so here we have 33333 to 35333.Then we have to add something like this:


x: { 0, 0, 0 }


x+2000: { 0, 0, 500 }


So here we have:


33333: { 0, 0, 0 }


35333: { 0, 0, 500 }


Very important: Set the interpolation type to linear

Step 3.
Go to Node manager right click Create Object event

Create a new event track to your object event. Set the number to the interval of your dissipate sequence x (33333 in this case) set:

Type: Sound (SND) and then

Data: DDSH - HumanDissipate

NB: You have to delete the corpse geoset and geoset animation that references it or it will be shown as your dissipate anim. Oh, and dissipate animations usually last around 2000 milliseconds.


I hope this tutorial is helpful and fast to perform if you understand the information.


 Last edited by a moderator: Apr 15, 2007

#### Deep Sea Kraken

#### Deep Sea Kraken

 Level 15

JoinedDec 29, 2006Messages985

Resources: 10

#### Resources
Models 9Icons 1

The dissipate looks a little wierd, but its better than not having one, nice tutorial - very easy to follow, ty ive been wondering how to add a dissipate animation for a while

#### Lord_T

#### Lord_T

 Level 23

JoinedOct 27, 2004Messages911

Resources: 46

#### Resources
Models 36Icons 8Packs 2

Agreed, this helps alot, and actually looks pretty good on some models.

#### Asomekiller

A

#### Asomekiller

Do you need tft or something else for this to work? because when i try to open it it says like c:/blizzard entertainment is missing or something like that. Help?

#### CoffinPuppet

#### CoffinPuppet

 Level 5

JoinedOct 7, 2007Messages61

Resources: 3

#### Resources
Models 3

Hey there


Callahan, got a Question.


How can i Speed up the Disscipate Process? My Model won't discipate Properly (It's doing it waaaaaaay too slow). And, I deleted the Geoset.- Geoset Animations which cause the Corpse. Is it normal, that Decay Bone doesn't show up?


Sorry the Troublesome, I'm new at Modeling

#### donut3.5

#### donut3.5

 Level 28

JoinedFeb 22, 2006Messages3,051

Resources: 32

#### Resources
Models 20Icons 10Maps 1Tutorials 1

Where ever it says x+2000, change that to, say, x+1000, or x+500 (but that might be too quick.)

--donut3.5--

#### alcadeas

A

#### alcadeas

man it doesn't work,and i need a model so i have downloaded one,anyway where can i find a tutorial to create or edit models(creatures)?I want to make a custom map and i want to be a good map.So plss give me some help anyone.

#### syltman

#### syltman

 Level 14

JoinedJun 13, 2007Messages1,430

I understand everything except part 2

Oh well... +rep

#### Deep Sea Kraken

#### Deep Sea Kraken

 Level 15

JoinedDec 29, 2006Messages985

Resources: 10

#### Resources
Models 9Icons 1

I. Love. This. Tutorial.

#### thisruoy

#### thisruoy

 Level 11

JoinedAug 4, 2007Messages640

Resources: 9

#### Resources
Icons 5Skins 2Maps 1Tutorials 1

So the guys just ment to fly up right

#### Callahan

#### Callahan

 Level 37

JoinedMar 28, 2004Messages1,613

Resources: 132

#### Resources
Models 108Icons 12Packs 2Skins 2Maps 5Spells and Systems 2Tutorials 1

yes but it's better than nothing.

The DH and MK are also immobile during dissipate.Does it cause trouble?No.

#### mr_danutzzz

#### mr_danutzzz

 Level 2

JoinedNov 18, 2007Messages15

yeah its cool this tutorial and i think you will help a lot of people especially me.


the dissipate animation is quite good but how can i make the waist go up than the rest of the body?


i've seen how looks a original dissipate animation and i saw the waist is up more than the body. also is a dissipate effect but this i think i can do it myself


Nice tutorial again!!

#### NatDis

#### NatDis

 Level 8

JoinedJan 11, 2008Messages164

Resources: 5

#### Resources
Models 4Icons 1

By changing certain numbers and adding a particular geoset and removing another, I have successfully made a unit out of a hero. Of course, all credits go to this tutorial.

#### Deolrin

#### Deolrin

 Level 50

JoinedApr 18, 2008Messages8,455

Resources: 85

#### Resources
Models 59Icons 21Packs 2Skins 1Tutorials 2

Ehh... it doesn't work with me:

my base model is forest troll trapper w/o axes. and the only thing that happens is that my model appears half-transparent at some places where should be Team Color(at his hair and arm-guards) and he carries the ensnare web in his hand in-game, and when he dies he plays the dead anim and then just gone.i tries several times but all time same. but i tried with other model(the little goblin-in-barrel from the sappers) and it worked! and btw, garithos(the guy that looks like the knight but with an axe,shield,helm and hes ginger) got that dissipate, too...

EDIT:NVM, that worked! woohoo! thats SO COOL! now i got hero glow AND dissipate anim! woo hoo ty vm!!! as i get some rep, the first man to get it will be YOU!!! ALL CREDITS COME TO YOU CALLAHAN!

#### SmartGoodBrother

#### SmartGoodBrother

 Level 7

JoinedJul 4, 2008Messages105

Resources: 11

#### Resources
Maps 11

I understand nothing, i'm still where i started...

#### Shadeus

#### Shadeus

 Level 2

JoinedSep 6, 2008Messages18

is it possible to make a Undead dissipate tutorial?

#### Shadeus

#### Shadeus

 Level 2

JoinedSep 6, 2008Messages18

that interval thing doesnt work!!! i change it like this:


From: x


To: x+2000


What am i doing wrong?


 Last edited: Nov 15, 2008

#### aeman

#### aeman

 Level 19

JoinedMay 14, 2014Messages917

Resources: 6

#### Resources
Skins 6

i didn't understand the second part. the mother part should be where?

#### ShadowMidas

#### ShadowMidas

 Level 14

JoinedSep 11, 2015Messages559

Resources: 1

#### Resources
Models 1

can you upload tutorial dissipate video?

#### strongwill

#### strongwill

 Level 9

JoinedFeb 18, 2015Messages78

Resources: 9

#### Resources
Models 9

Thank You brother. Very Helpful.

 You must log in or register to reply here.


### Similar threads

 Making Effects in Magos - Advanced Techniques


WILL THE ALMIGHTY

Feb 23, 2008

3D Modeling Tutorials

Replies14Views10K

Nov 2, 2008
3rgytr54

Locked

 Dissipate and Decay question


FeelsGoodMan

Oct 6, 2020

Modeling & Animation

Replies2Views988

Oct 6, 2020
FeelsGoodMan

 Making Effects in Magos - Advanced techniques 2


WILL THE ALMIGHTY

May 3, 2008

3D Modeling Tutorials

Replies5Views8K

Oct 9, 2014
ChaoticHunter

Locked

 Making a unit dissipate


loktar

Jan 21, 2016

Triggers & Scripts

Replies17Views2K

Jan 23, 2016
loktar

[3D Art]
 Creating a simple Dissipate animation


Spacebuns

Oct 10, 2020

Tutorial Submission

Replies3Views2K

Oct 10, 2020
loktar

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