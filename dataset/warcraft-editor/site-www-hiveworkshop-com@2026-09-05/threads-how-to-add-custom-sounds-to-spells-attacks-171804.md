---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-add-custom-sounds-to-spells-attacks.171804/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Add Custom Sounds to Spells Attacks
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-add-custom-sounds-to-spells-attacks.171804/>
#### bonethewise

B

#### bonethewise

First off make your attack with object editor.


Example: Choose Firebolt (warlock) i will call mine Tutorial Blast


Do what you wish with it and go to trigger editor. Create new catagory and call it Tutorial.


Create a new trigger called Tutorial blast or whatever you called the spell.


Make the event Generic Event Unit starts channeling ability,


Condition Ablility is equal to Tutorial Blast


Action Play Sound


Should look like this

Events
Unit - A unit Begins channeling an ability

Conditions
(Ability being cast) Equal to Tutorial Blast (Warlock)

Actions
Sound - Play explosion_01 <gen>

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

It's a bit too short and simple for a tutorial. And it needs some organization. I'll probably graveyard this soon, sorry. D;

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Omg dude, this one will be deleted or should I say removed really soon if you don't improve it!

Explain how to use Sound Editor!

How to import files

Explain mp3 file: what are frames, bit rate, what means stereo for example!

Add links to some legal, free and simple programs that can help to lower file size!

To crop some part of sound, increase it volume etc etc!

Explain sound generally at first place then explain how to use it with WE!

I saw maps that have over 17 MB (only because 1 mp3 file 15 MB )

Show how sound can create leak and how to clean it, but care sound isn't simple point leak!

Show how to play music and how to play sound in WE there is difference, show us what is it!

Explain volume controls (trigger actions), how list work etc etc!

Explain also why we use this one in cinematic maps:

 Sound - Clear the music list

 Sound - Stop music Immediately

 Sound - Set Ambient Sounds to 0.00%

 Sound - Set Animation and Spell Sounds to 0.00%

 Sound - Set Combat Sounds to 0.00%

 Sound - Set Fire Sounds to 0.00%

You can't show use how to attach sound to spell or attack -.- We already know that!

Show users something about sound that they don't know! That is point of tutorial after all to help people really learn something not to find some trigger action and ask themselves how this works?

I show you only some basic stuff, you should now try to impress us with some awesome tutorial!

#### Septimus

#### Septimus

 Level 31

JoinedMay 3, 2008Messages3,146

Resources: 50

#### Resources
Packs 1Maps 35Spells and Systems 8Tutorials 6

for ordinary blizzard spells, you don't need trigger to create those custom sounds.


all you got to do was imports a sounds and replaces the paths of the sounds it currently use.


this tutorial have no use at all.

#### bi_a_op

#### bi_a_op

Patron

 Level 7

JoinedNov 3, 2015Messages85

Resources: 1

#### Resources
Maps 1

Thanks

#### Ravager16829

#### Ravager16829

 Level 40

JoinedMay 14, 2021Messages2,455

It's usually better make a new thread in the World Editor Help Zone rather than doing a necro-post for this old thread.

And yes, this method has been changed on the latest patch (default in-game audio no longer uses MP3, but FLAC).

 You must log in or register to reply here.


### Similar threads

H

 Jump Tutorial


Histenchist

Jun 4, 2007

Tutorial Archive

Replies5Views3K

Jun 27, 2007
Wolverabid

 How to add a bouncing attack


XPQJ

Mar 24, 2009

Tutorial Archive

Replies7Views4K

Apr 12, 2009
Pyritie

[GUI]
 how to make a chain ability (The easy way)


ShadowBladeX

Apr 20, 2016

Tutorial Archive

Replies20Views4K

Feb 12, 2017
ShadowBladeX

Locked

 Global Variable


Nintendo_Masta

Nov 13, 2005

Triggers & Scripts

Replies1Views2K

Nov 14, 2005
Daelin

[GUI]
 How to properly use "Crow Form"


Vehster

Aug 28, 2011

Tutorial Archive

Replies8Views6K

Nov 9, 2011
PurgeandFire

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