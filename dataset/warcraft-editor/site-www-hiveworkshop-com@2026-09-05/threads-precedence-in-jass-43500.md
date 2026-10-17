---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/precedence-in-jass.43500/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Precedence in JASS
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/precedence-in-jass.43500/>
#### Earth-Fury

#### Earth-Fury

 Level 11

JoinedFeb 18, 2004Messages394

Resources: 1

#### Resources
Tutorials 1

Precedence in JASS

 Definition of Precedence


 In a programming language, the precedence of an operator determines in what order it will be evaluated. An operator with a higher precedence will be evaluated before another with a lower precedence. For instance multiplication has a higher precedence than addition.

http://www.google.ca/search?q=define:+precedence

The rules of precedence are important if you don’t want your code littered with needless parenthesis. JASS has only a few operators, so remembering their precedence is not so hard:

( )

not

* /

+ -

<= >= > < == !=

and or


The higher on the list, the higher an operators precedence. If two operators are in the same row, they share the same precedence and are thus executed from left-to-right. By this chart, the following examples are all true:

 JASS:

a == b != c // Is the same as…
(a == b) != c

 JASS:

a != b == c // Is the same as…
(a != b) == c

 JASS:

1 + 2 * 3 – 4 / 2 // Is the same as…
1 + (2 * 3) – (4 / 2)

 JASS:

false == 1 > 2 // This will fail to even compile, as it’s the same as…
(false == 1) > 2

 JASS:

not 4 > 3 // This will also fail to compile, as it’s the same as…
(not 4) > 3

 JASS:

a == b or c == d // Is the same as…
(a == b) or (c == d)


The or and the and operators are executed in a short circuit manner. This means that if from the current execution, an answer is determinate, execution stops. By this, the following are both true:

 JASS:

f() // Returns false
t() // Returns true

t() or f() // f() will never be executed.

f() and t() // t() will never be executed.

Much credit to PurplePoot for helping me test precedence

Edit: Tiny formatting edit and removal of signature


 Last edited: Feb 29, 2008

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I never even knew this. =D

Thanks.

Seems good to me.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Really no reason not to approve this, IMO.

~Guess what?

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

Woah, thats really useful to reference to


+rep

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

you have forgotten to write the t() and f() functions

Else, nice to know but i think for my mental sanity (to keep the code easily readable) i will still use () ^^


 Last edited: May 4, 2008

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

 you have forgotten to write the t() and f() functions


Click to expand...

Both will be executed, of course.

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

Uhhh, wait a second....


Does that mean if you have a function for converting indexes, only the first function will fire, bugging out your JASS???


Ex.

 JASS:

udg_UnitArray[IndexFunction(u1)] == udg_UnitArray[IndexFunction(u2)]


Or maybe...

 JASS:

udg_UnitArray[IndexFunction(u1)] == udg_UnitArray[IndexFunction(u2)] and udg_UnitArray[IndexFunction(u3)] == udg_UnitArray[IndexFunction(u4)]

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Ands and Ors are short circuit, meaning in any circumstance where the answer is known after one of the two booleans is evaluated, they will immediately finish execution.

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

Ohhh, I gotcha, so if lets say it is an Or, and the first one is true, it will.... oh thats what the example means ><

So, if your second boolean function has some kind of unrelated call statement in there, you should be careful, because it will not always go through that secondary conditional function

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Yes, exactly.

 You must log in or register to reply here.


### Similar threads

Locked

 Booleans and Logic [Rough Draft]


PurplePoot

Oct 6, 2009

Triggers & Scripts

Replies0Views1K

Oct 6, 2009
PurplePoot

 JPAG - JASS Proper Application Guide


Bribe

Sep 27, 2011

JASS Tutorials
23

Replies136Views49K

Oct 18, 2016
Deleted member 219079

D

 [vJass] SleepAction - A Cinematic Lifesaver


Bribe

Aug 25, 2010

JASS Tutorials

Replies13Views24K

Nov 27, 2021
Bribe

Locked

 Vexorian Map Optimizer does not optimize operations


_Guhun_

Mar 17, 2018

World Editor Help Zone

Replies4Views2K

Mar 17, 2018
Dr Super Good

 Basic JASS Tips


Daelin

Jan 8, 2006

Scripting Tutorials

Replies1Views15K

Apr 10, 2006
PurplePoot

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

JASS Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top