---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/manual-dependency-resolution-of-parameterized-function-calls-in-jass.289461/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Manual Dependency Resolution of Parameterized Function Calls in JASS
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/manual-dependency-resolution-of-parameterized-function-calls-in-jass.289461/>
#### Aniki

#### Aniki

 Level 13

JoinedNov 7, 2014Messages571

Resources: 21

#### Resources
Tools 1Maps 1Spells and Systems 14JASS 3Tutorials 2

Manual dependency resolution of parameterized function calls in Jass

The Jass language has the limitation that it cannot call functions that take parameters before they are declared, which means that if a function A wants to call function B, function B must be declared before function A. There is no forward declaration in Jass.

 JASS:

// this compiles:
//
function B takes integer b returns integer
 return b * b
endfunction

function A takes integer a returns integer
 return B(a) + B(a)
endfunction

// this does not compile:
//
function A takes integer a returns integer
 return B(a) + B(a) // error: undeclared function B
endfunction

function B takes integer b returns integer
 return b * b
endfunction

vJass has the concept of libraries which can declare requirements of other libraries and with that information the vJass compiler/preprocessor (jasshelper) can automatically rearrange the generated map script by doing dependency sorting in a way in which functions that call other functions always come after them so that there won't be any undeclared function errors.

 Yet another issue with World editor and Jass was that it is impossible to control the order of triggers in the map's script when saved.


Click to expand...

This is a quote from jasshelpermanual.html, and its not correct.

A mapper can manually do what jasshelper does automatically when libraries are used in vJass simply by rearranging the triggers they create in World Editor's Trigger Editor.

The order in which the triggers need to be rearrange is the same when functions are called, i.e triggers with functions that call functions from other triggers must be declared after them.

Now if you go and open World Editor (WE), open the Trigger Editor (F4), create a trigger called A with the following content:

 JASS:

function A takes integer a returns integer
 return B(a) + B(a)
endfunction

and a trigger called B with the following content:

 JASS:

function B takes integer b returns integer
 return b * b
endfunction

and rearrange the triggers so that A comes after B

then save the map, what you should see is... undeclared function B???

Why did we get the error when we put trigger A after trigger B, that should've worked (according to this tutorial) right?

It will work, but because we created trigger B after trigger A, WE's internal representation of the triggers is not B followed by A but A followed by B and so we get the error.

How can we force WE's "internal representation of the triggers" to be in the order in which we've rearranged the triggers? Simple. After saving the map and getting the undeclared function error, close the map, reopen it, resave it, and it should now compile without the undeclared function error.

And with that simple procedure the mapper turns themselves into "the human Jass parameterized function calls dependency resolver".

#### Attachments

mdrij-tut-img-1.png

 3.3 KB

 · Views: 374



mdrij-tut-img-2.png

 1 KB

 · Views: 364



#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Yes that's good possible apporoach. I think it's known already by some guys, but I'm not sure it's already covered inside a tutorial. If it's not the case it's definilty useful.

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

Aniki said:

 After saving the map and getting the undeclared function error, close the map, reopen it, resave it, and it should now compile without the undeclared function error.


Click to expand...

I've always known to do this, but never really understood why it fixes itself.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

KILLCIDE said:

 I've always known to do this, but never really understood why it fixes itself.


Click to expand...

I think it has to do with how the editor compiles when you create a trigger vs. when you open a fresh map.

When you open a map, it compiles triggers based on the order they come in (top to bottom). When you create a new trigger, that is placed at the bottom of the compilation regardless of where you put it in the list of triggers (I think). At least, that is what this thread seems to suggest (SAUS's posts mention the method in this tutorial later on in the thread):

Use functions from other triggers

But it only keeps that order for as long as the editor is open. When you close the editor and reopen, it doesn't know what order you created the triggers in, so it just chooses top-to-bottom again.

According to Almia's thread, there may be other weird quirks that switch up the order as well, e.g. adding stuff to the map header.

This tutorial gives some nice insight. The thread title is pretty verbose, though.  I came in here expecting a thesis paper.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Can we call it something like "Trigger Order Manipulation"?

It would benefit from little effort of formatting, like different parts or topics, so it's better structured and easier to read. (even it's not that big)

Maybe it was useful if you note that we not encourage such tequniques for public code/submissions.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

It could be approved from content, but some changes like formatting is asked for, to be a tutorial. See comments.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Good content, but Aniki seems busy too busy. I would want to change the title, and maybe some formatting. Gy for now.

 You must log in or register to reply here.


### Similar threads

 Custom Event Data


Nestharus

Jan 17, 2011

Tutorial Archive

Replies5Views5K

Dec 7, 2013
PurgeandFire

 Slightly different approach to H2I/H2S


Lord_BoNes

Sep 21, 2008

Tutorial Archive

Replies1Views2K

Sep 21, 2008
PurplePoot

 Attack-damage detection engine (JASS)


paskovich

Apr 29, 2007

Tutorial Archive

Replies15Views17K

Nov 28, 2021
Dr Super Good

 Magic of JASS [Reviewed: Ghan_04]


forzamoto

Jun 27, 2008

Tutorial Archive

Replies6Views2K

Jul 18, 2008
Ralle

 JASS in detail


thelifelessone

Aug 29, 2009

Tutorial Archive

Replies3Views2K

Sep 2, 2009
Dreadnought[dA]

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