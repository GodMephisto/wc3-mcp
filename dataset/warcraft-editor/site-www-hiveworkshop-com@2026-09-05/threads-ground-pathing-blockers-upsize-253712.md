---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ground-pathing-blockers-upsize.253712/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Ground Pathing Blockers Upsize
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ground-pathing-blockers-upsize.253712/>
#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

Hi, people! This is my first ever tutorial, so please be gentle. This is all about making Pathing Blockers larger than the two defaults.

1. What is Pathing?

This tutorial is not going discuss what is pathing in great detail, as there are many other tutorials that do so. Pathing tutorials can be found here and here.

Basically, pathing determines what kind of units can go through a certain area in a map, be it air-blockers, ground-blockers, or Pathing Blockers ('Both').

This tutorial is going to focus on ground pathing blockers as they are the most widely used for terraining purposes, such as creating custom cliffs. Tutorial about that here.

2. Why Upsize?

Why create larger pathing blockers, when small blockers can block everything eventually? Well, first, it's a waste of effort if you're blocking huge areas. Yes, there are larger placement brushes, but it somehow feels less messy when you use large blockers.

Also, there are Destructible limits for non-JNGP editors, so if your map is huge, large blockers might be handy.

3. Renaming

First, we're going to name the regular one-block pathing blocker 'Tiny'.

It doesn't really matter if you name it tiny or not, it just adds clarity later on when you choose pathing blockers from the terrain editor.

Second, rename the default 'Large' pathing blocker into something around 'Small'. Yes, it is indeed small.

4. Creating New Blockers

Click the create Destructible button:

Base it off the Pathing Blocker (Small). Now, modify its pathing texture field to the corresponding size you want:

(Size; String Path)

Medium: PathTextures\6x6SimpleSolid.tga

Large: PathTextures\8x8Default.tga

Huge: PathTextures\10x10SimpleSolid.tga

Gargantuan: PathTextures\12x12Default.tga

5. Using Blockers

They look like these in the terrain editor if you press 'P' after placing them.

6. Unused Ground Blockers

In the Object Editor you can find many ground blockers, some are rectangular just like the Windmill texture:

PathTextures\6x8Default.tga

There are a few cases where though the texture is not used by default, they're still usable. Most of them are under the size of 12, such as:

PathTextures\8x10Default.tga

PathTextures\6x10Default.tga

PathTextures\2x8Default.tga

7. Custom Blockers

I won't explain this myself as it's already explained here and I would just probably copy paste what he wrote and be flamed upon. Anyways, I think Photoshop is used for this 90% of the time.

==========

Yeah the tutorial is /so/ simple, but I wanted to share it with multiple people and thought that sending away links to the tutorial is easier than actually copy-pasting the message.

It's mostly for beginners anyways. I remember the time when I didn't know how to create custom objects... </3

#### Attachments

Tiny1.png

 5.2 KB

 · Views: 703



PathingBlockTut.png

 728.2 KB

 · Views: 766



NewDes.png

 5.2 KB

 · Views: 679



PathingTut2.png

 762.2 KB

 · Views: 906




 Last edited: Jun 26, 2014

#### GreeN!X

#### GreeN!X

 Level 23

JoinedOct 20, 2012Messages3,068

Resources: 4

#### Resources
Icons 1Maps 1Templates 1Tutorials 1

Well, a similar tut is already here. Though yours is a lot more textier and looks pro-er. I dunno, I can't do a more sensible comment. I need sleep...

https://www.hiveworkshop.com/forums...raining-alternative-pathing-technique-216570/

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

personally I prefer img tags rather than attatchments. Anyyway, useful. Also..

"Hi, people! This is my first ever tutorial, so please be gentle. This is all about making Pathing Blockers larger than the two defaults."

should be under the title Introduction/title of thread

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

@GreeN!X, I actually didn't read that tut's contents as I thought it was an in-depth explanation of what pathing is, and how to place pathing blockers. xD

@Chaosy, done. For so long, I find it weird that this is my first time attaching images in this forum.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Short, simple, and sweet. Approved.

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

It's a good tut for those people who just want to know how to use multiple sizes for pathing blockers.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

Help the noobs become masters to live in a world without sorrow, fear and dread. Love is strong and only cares for joyful giving. <3

jk

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

It's dangerous to do that alone, use this as your BGM


 Last edited: Jun 23, 2014

#### Solu9

#### Solu9

 Level 26

JoinedJan 25, 2011Messages2,293

Resources: 63

#### Resources
Models 28Icons 27Tools 1Maps 5Spells and Systems 1User Interface 1

Cute little tutorial Wrathion.

Would you consider adding a "create a custom sized PathTexture" section?

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

Was thinking. Right after I wake up. About 12h from now.

#### All_Time_Pro

#### All_Time_Pro

 Level 13

JoinedJul 21, 2015Messages303

Resources: 3

#### Resources
Maps 3

do you know how to make a pathing blocker that is

either:

A) freely moveable

B) or is half the size of (small) pathing blocker?

#### Chaosium

#### Chaosium

 Level 39

JoinedAug 29, 2012Messages1,890

Resources: 10

#### Resources
Models 10

##
 Insurgent Cry


Here's a 1x1 blocker, not really freely movable because things adhere to the grid whenever they have a path texture

#### Attachments

Blocker1x1.tga

 48 bytes

 · Views: 62



 You must log in or register to reply here.


### Similar threads

 [Terraining] Alternative Pathing Technique


sonofjay

May 22, 2012

Misc/General Mapping Tutorials

Replies34Views19K

May 15, 2013
EmoBrother

Locked

 pathing texture pathes


Tigrick

Aug 28, 2012

World Editor Help Zone

Replies5Views3K

Aug 29, 2012
rulerofiron99

 Pathing - Everything about it


Dr. Boom

Jan 1, 2010

Misc/General Mapping Tutorials

Replies28Views58K

Apr 12, 2024
Freddyk

 Walkable Walls/Doodads


Archian

Aug 10, 2006

Misc/General Mapping Tutorials

Replies0Views38K

Aug 10, 2006
Archian

Locked

 Pathing Blockers and LOS Blockers


GreeN!X

Feb 9, 2013

World Editor Help Zone

Replies2Views2K

Feb 9, 2013
retard

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