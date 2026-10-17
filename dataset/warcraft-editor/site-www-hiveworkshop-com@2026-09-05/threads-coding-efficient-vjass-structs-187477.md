---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/coding-efficient-vjass-structs.187477/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Coding Efficient vJASS Structs
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/coding-efficient-vjass-structs.187477/>
#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

gy this please


 Last edited: Feb 4, 2014

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice tutorial, simple enough. I like that you included the generated code, as it definitely makes the difference clear.

I'll need to reorganize it a bit before I can approve it, but this is definitely a useful tutorial, and overall the custom allocators/deallocators don't look that bad either.

EDIT: Whatever, it is organized well enough. ~Approved.


 Last edited: Jan 20, 2011

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

The doublefree protection should not be there when having debugmode disabled.

 allocate/deallocate as they might as well be put into create and destroy


Click to expand...

You could also directly have used allocate/deallocate instead of create/destroy.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

->You could also directly have used allocate/deallocate instead of create/destroy.


vJASS convention is to name constructors and destructors create/destroy, not allocate/deallocate.


->The doublefree protection should not be there when having debugmode disabled.


You sure? : o


Anywho, it still spams triggers with extending >: P

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

This is probably my favorite tutorial from the ones you released because almost no one knows what vJass is really doing.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Here's another interesting tidbit of information http://www.thehelper.net/forums/showthread.php/161603-onDestroy-and-destroy?p=1332666#post1332666

#### Tom_Kazansky

#### Tom_Kazansky

 Level 2

JoinedDec 17, 2008Messages19

Resources: 1

#### Resources
Models 1

wow, I have never known about this, thanks for posting this.

also, I have a question: is the case "struct extends interface" like "struct extends struct"?

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

gy this please

#### Dark-Zalor

#### Dark-Zalor

 Level 19

JoinedOct 10, 2011Messages580

Resources: 28

#### Resources
Maps 25Spells and Systems 2Sounds 1

gy graveyard?

For what reason? is there other complete tutorial about struct?

#### Blightsower

#### Blightsower

 Level 18

JoinedJun 20, 2014Messages556

Resources: 19

#### Resources
Packs 1Spells and Systems 17Tutorials 1

Where is the tutorial moved to?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Blightsower said:

 Where is the tutorial moved to?


Click to expand...

I believe he moved his resources to his github:

###
 JASS/jass/Tutorials/coding efficient vjass structs.txt at master · nestharus/JASS


Contribute to nestharus/JASS development by creating an account on GitHub.

 github.com


 You must log in or register to reply here.


### Similar threads

 Saving and Loading


Nestharus

Apr 4, 2011

Tutorial Archive

Replies40Views26K

Feb 4, 2014
Nestharus

 The Art of Module Interfaces


Nestharus

Feb 1, 2011

Tutorial Archive

Replies6Views5K

Mar 15, 2022
Ralle

 vJass Indexer Tutorial - Structs


Dynasti

Jan 18, 2009

Tutorial Archive

Replies4Views8K

Jan 20, 2009
Dynasti

 Structs (A rough crash course)


MyPad

May 10, 2017

Tutorial Archive

Replies17Views5K

Oct 4, 2020
MyPad

Locked

 Efficiency in struct


Malhorne

Jan 1, 2014

Advanced Scripting

Replies6Views2K

Jan 1, 2014
TriggerHappy

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