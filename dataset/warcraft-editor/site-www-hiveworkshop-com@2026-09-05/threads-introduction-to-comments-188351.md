---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/introduction-to-comments.188351/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Introduction to Comments
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/introduction-to-comments.188351/>
#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

This tutorial will introduce the nested, delimited, and limited comments as well as good commenting.


This is chapter 3 of the complete guide to vJASS.

Chapter 3- Comments


 Comments are little code notes that are used to describe what a line(s) of code does. Any code that is commented out is ignored by the parser. Comments are typically used by programmers to explain why they are doing what they are doing, not what they are doing. They may also go over how to use code and how code works (documentation, normally located above all of the code).


 Comments may be single line or delimited.


 Single Line Comment- //


 Delimited Comment (multiple lines)- /*code*/


 A singe line comment will comment out everything past it -> set x = 1 //setting x to 1. If you notice, //setting x to 1 is colored green. It happens to be a comment.


 A delimited comment will comment out everything between /* and */ set x /*x is a var*/ = 1 //1 is a value. Delimited comments may also be nested /*/**/*/. Delimited comments are used for documentation and temporarily commenting out buggy code or test code.

 JASS:

 /*This stuff is within a delimited comment/*this stuff
 is within a nested delimited comment*/

 The formatting of a delimited comment doesn't matter as long as it starts with /* and ends with */
 */

Commenting Exercises

 Paste the code into the map and then comment it out to get rid of the syntax errors.

Problem 1

 JASS:

 Comment me to get rid of the syntax error

 JASS:

 //Comment me to get rid of the syntax error

​
Problem 2

 JASS:

 I am
 your
 syntax error
 yes, you will

 not stop
 me at all

 JASS:

 /*I am
 your
 syntax error
 yes, you will

 not stop
 me at all*/

​

Click to expand...

Good Commenting

Bad


 The code below describes what each line does, but it doesn't describe why it does it. While the code can be read, it's purpose remains very unclear. Furthermore, the comments seem pointless. For example, in the first line, it is easy to read that i is being set to 1000. What the person reading comments really wants to know is what i is and why it's being set to 1000.

 JASS:

 local integer i = 1000 //set i to 1000
 local integer x //declare x

 //do some loops
 set x = i //set x to i
 loop
 //display some text
 call DisplayTextToPlayer(GetLocalPlayer(), 0, 0, I2S(x))
 set x = x - 1 //decrease x
 exitwhen x == 0 //exit when x is 0
 endloop

 //do some more loops
 set x = 0 //reset x
 loop
 //dispay some more text
 call DisplayTextToPlayer(GetLocalPlayer(), 0, 0, I2S(x))
 set x = x + 1 //increase x
 exitwhen x == i //exit when x is i
 endloop

Good


 The code below describes why each action is taken if the action may be unclear. For example, it states that i determines how many times to repeat an action (meaning it's being set to 1000 to repeat it 1000 times). It states x is the current number of repetitions. Even someone who can't read code could understand what the code did by its comments.

 JASS:

 local integer i = 1000 //how many times to repeat an action
 local integer x //current number of repetitions

 //repeat backwards to display the value of x in warcraft 3 from the value of i to 0
 //i -> 0
 set x = i
 loop
 call DisplayTextToPlayer(GetLocalPlayer(), 0, 0, I2S(x))
 set x = x - 1
 exitwhen x == 0
 endloop

 //repeat forwards to display the value of x in warcraft 3 from 0 to the value of i
 //0 -> i
 set x = 0
 loop
 call DisplayTextToPlayer(GetLocalPlayer(), 0, 0, I2S(x))
 set x = x + 1
 exitwhen x == i
 endloop


 There are various styles to making good documentation.

 JASS:

 /*=====================================================
 =====================================================*/

 JASS:

 ////////////////////////////////////////////////////////
 //
 ////////////////////////////////////////////////
 //
 ////////////////////////////////////////////////
 //
 //
 ////////////////////////////////////////////////////////

 JASS:

 /*------------------------------------------------------
 ------------------------------------------------------*/


 There is no set way to do it, so the best way to write good documentation for code is to write what looks good.


 A good thing to do with code is to list out all of the functions, variables, structs, modules, and etc at the top of the code (a simple list) and to place a descriptive list below it (a second list with maybe a paragraph of information on what each thing does and examples of how they are used).


Click to expand...


 Last edited: Jan 28, 2011

#### Jagex

#### Jagex

 Level 4

JoinedJan 28, 2011Messages47

Sucks like hell. It is already in another tutorial. Please don't waste memory space.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice explanations. I can't see anything wrong with it, especially since the info is pretty basic.  But it fits well in the series, so gj. ~Approved.

#### SanKakU

#### SanKakU

 Level 13

JoinedMay 11, 2008Messages1,198

Resources: 6

#### Resources
Icons 2Maps 3Spells 1

i have to agree with jagex, unfortunately. you've made tons of tutorials and they're not even very good.


please just delete the lot of them, start over. make fewer tutorials and make them make sense. i can't even agree that you think comments are only used for explaining why and not what or other questions.


i know it sounds harsh, but i'm serious. i've given plus rep to other tutorial makers on this site but you've made many more tutorials than any of them and afair i haven't plus rep you for any of yours. now if there was some random post in the forums of yours that i plus rep, that's another story entirely.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

 i know it sounds harsh, but i'm serious. i've given plus rep to other tutorial makers on this site but you've made many more tutorials than any of them and afair i haven't plus rep you for any of yours. now if there was some random post in the forums of yours that i plus rep, that's another story entirely.


Click to expand...


Just doing it to share my experience. If you can make it better, go for it =). I don't have an ego, I don't care ^_^. The only thing that gets to me is when people block off knowledge or resources because they can't find a use for it themselves ;\. If you rewrite this and it's even near identical, then submit it and get this one gy'd. As long as the information is out there chapter by chapter, it's all good.


I just kind of write them straight through once now because I have a lot to write to share everything I've learned over the years with vJASS ^_-. Can't bother myself with too much quality, I just get the information out there and as long as one person can understand it, then at the least it can be translated ;P. The vJASS tut series though (and this is a chapter from it) did have a lot of effort put into it. This actually went through 5 revisions I think ;o. The variable chapter, which I'll get to eventually, will be an interactive tutorial using String Parser to introduce variables, this way you don't have to see any unfamiliar code when learning how they work ;D.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

gy by request

#### The Terminator

#### The Terminator

 Level 1

JoinedApr 28, 2020Messages2

Awakening of Stormrage was very brutal for me, this and under the burning sky were the two hardest for me in all of roc.

#### The Terminator

#### The Terminator

 Level 1

JoinedApr 28, 2020Messages2

In hard mode

 You must log in or register to reply here.


### Similar threads

 Advanced Third Person Camera


Megafyr

Oct 4, 2009

Tutorial Archive

Replies10Views22K

Jun 23, 2010
Bommelding

Locked

 Array Of Structs, How to Acess


rover2341

Jan 26, 2010

Triggers & Scripts

Replies0Views1K

Jan 26, 2010
rover2341

Locked

[JASS]
 why my we crash?


machris13

May 9, 2010

Triggers & Scripts

Replies5Views1K

May 9, 2010
M0RT

 Multi-unit Instanceability With Timers (vJASS)


busterkomo

Jul 28, 2009

Tutorial Archive

Replies8Views2K

Jan 18, 2010
Kwah

Locked

[JASS]
 Trigger to create a square pattern, but something is wrong :S


Komaqtion

Feb 5, 2010

Triggers & Scripts

Replies24Views4K

Feb 28, 2010
Berb

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