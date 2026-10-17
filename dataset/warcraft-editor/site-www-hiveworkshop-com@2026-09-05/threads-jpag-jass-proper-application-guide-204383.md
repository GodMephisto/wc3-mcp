---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/jpag-jass-proper-application-guide.204383/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Jpag JASS Proper Application Guide
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/jpag-jass-proper-application-guide.204383/>
#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

JPAG - JASS Proper Application Guide

Bettering the cause of readable source code

If you are building a map you want to submit for review, building a


spell or system to submit for public use, this guide can help to establish a


general formula for making good, readable code. The more people that


follow this, the easier it will be to read code because you know to expect


certain things and not get weird surprises like a STRUCT_NAME_LIKE_THIS.


The reason I have chosen these rules is that JASS standards defined


by Blizzard in the original form, while they still had the opportunity


to style the language in any format they chose, they chose to do it this


way. If we are to have a standard, then Blizzard's standard makes the


most sense.


These rules help to do something similar for writing your JASS code.


There has been a standard in the past for JESP, but this revives the


topic and makes points more clear. Henceforth we can refer to this


tutorial format as the JPAG format.​

Naming Conventions

CONSTANT_NAMES are all capital letters and each new word


is indicated by being seperated from the rest by an underscore.

TEXTMACROS are good to write in this format, though it is not


a requirement. "constant <type> CONSTANT_NAME", however,


should follow this format, as well as the "key" type. Constant functions are an


exception, as they should still adhere to the FunctionName syntax.

FunctionNames start with an uppercase and each new word is


indicated by a new capitalitzed letter. FunctionName. StructNames,

ModuleNames, LibraryNames and ScopeNames should


also follow this format. FunctionInterface names and InterfaceNames


for lack of better placement should just be treated the same as


StructName. Though the use of either is highly discouraged because


interfaces double the code length and add unnecessary handles to your


map which in some cases also drastically slow CPU performance.


The reason why these non-JASS names should be capitalized is


just for English-language syntax and syntax of many languages.


Names are always capitalized in English. Save for variable names


and method names (reasons outlined below), keeping to this


convention is ideal if we want a uniform standard. There is also


the idea of "this is a property of 'that'" in which case it makes more


sense to write "StructName.memberName" than to write something


like "structname.MemberName" or "StructName.MEMBER_NAME"


although sometimes the latter is unavoidable but please try your


best to avoid it because it's really ugly Nestharus.

variableNames and methodNames should begin with a


lowercase letter and each new word seperated by a new capitalized


letter. This is called camelCasing. variableNames, this was designed


by Blizzard. methodNames are typically formatted like this in most


mainstream languages like C and are also formatted in such a way


with most of vJass resources, it makes sense that this should also


be the standard.


Also, short variable names are generally discouraged. A variable


should have a name that has some semblence to its purpose. If


it is for general purpose, simply writing "data" or "value" is only


fine if there is no other "general purpose" variable that might also


share such a name.


AMENDMENT: Added on 25 Feb 2016 by Bribe:


Creation and destruction of artificial types should match the naming conventions set by Blizzard and Vexorian. Struct methods should be named "create" or "destroy". Other functions which try to serve the same purpose should begin with Create or Destroy.


There is another naming convention which should be encouraged: adding and removing. These should be reserved for container-type syntax. In JASS, you have GroupAddUnit, GroupRemoveUnit, ForceAddPlayer, ForceRemovePlayer and others. The naming convention should contain the words add and/or remove when you are working with structures like linked list, linear stacks, Tables and any other "containing" element which may be utilized or invented.​

Indentation

Indentation is four spaces in JASS and every block is required


to be indented, with the rare exception of indenting the contents of


an all-encompassing library or scope because it's usually obvious


then.


Sometimes people make "exitwhen" on the same indentation as


"loop", or make "local" on the same indentation as "function". Neither


way used endorsed by Blizzard, though there are compelling arguments


to support it, if you so desire. Though I encourage to do it the Blizzard


way, doing it this other way is also permitted.​

Documentation

A resource should document all of its public interface at the top


of the script, and if it has library requirements it should provide


a link to those requirements.


Obviously, in the JassHelper manual there are established


"scope" and "library" keywords. because it is controversial and


this is a global standard, I will only make the following clear:


If it is a resource that will not be required by others, for example


a spell or a fully-automatic system are not required by any other


library, it is OK if it is a scope as long as its requirements are


still linked to and declared.


Obviously if it is to be required by others, containing any form


of public API, it needs to be a library.


Use your best judgment with outlining the public API at the top


of your library.​

Configuration

If your resource has things that are able to be configured, like


spell duration or FX art path, damage amount or whether to even


deal damage, they need to be easily configurable either through


dedicated functions or through dedicated constants. Some other


creative approaches to configuring your resource are sometimes


also allowed, depending on a few factors like presentation, ease


of use and with a lesser emphasis on efficiency.​

Initialization Priorities

NOTE: Users of Cohadar's JassHelper can ignore this next


step for the most part as initialization is much more intuitive


in his version.


This is not clear enough to most, but here is how the order of


initialization fires in the realm of JASS:

Module Initializers

 JASS:

module M
 private static method onInit takes nothing returns nothing
 endmethod
endmodule
struct S extends array
 implement M
endstruct

Struct Initializers

 JASS:

struct S extends array
 private static method onInit takes nothing returns nothing
 endmethod
endstruct

Library Initializers

 JASS:

library L initializer Init
 private function Init takes nothing returns nothing
 endfunction
endlibrary

Scope Initializers

 JASS:

scope S initializer Init
 private function Init takes nothing returns nothing
 endfunction
endscope

InitTrig_ Initializers

 JASS:

function InitTrig_MyTrigger takes nothing returns nothing
endfunction

"Event - Map Initialization" GUI Initializers

MyTrigger
Events
Map Initialization

Conditions

Actions

.

0-seconds game time (happens after map has finished loading)

 JASS:

call TimerStart(CreateTimer(), 0, false, function OnLoad)


Rather than letting all initializers in one library or scope fire first,


initializers fire in that above sequence.


Because a modules initialize first, you should try to do everything


from within a module initializer if it initializes public API. Spells,


however, can get away library, scope, struct, InitTrig_ or Map


Initialization. A unit casting a spell from within an initializer may


bug your resource but it is too much to ask for everyone to use


module initializers for everything.


However, systems that create things which might be used by the


the public during initialization (such as a hashtable) need to use


modules to initialize, just in case. Its API is horrendous but it is


reliable.​

Math Operators

Formatting math operators has been pretty freestyle for a long


time, however I owe this new addition to the guide to Nestharus


as he proposed a pretty logical way to organize operators:


One space between addition/subtraction operators, and no spaces


between multiplication/division operators. This clearly shows the


precedence of the operations.

 JASS:

1 + 1 * 3 + 6 - 2 / 1 * 6
1+1 * 3+6-2 / 1 * 6
1+1*3+6-2/1*6
1 + 1*3 + 6 - 2/1*6 //Correct


 Last edited: Feb 25, 2016

#### Ignitedstar

#### Ignitedstar

 Level 5

JoinedJun 30, 2004Messages170

This is so much better than Mag's. No offense to Mag, of course.

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

 Indentation is four spaces in JASS


Click to expand...

How did you come with this value ?

 Sometimes people make "exitwhen" on the same indentation as

"loop", or make "local" on the same indentation as "function". Both

ways are absolutely horrible and will be laughed at upon viewing.

Your resource won't be approved by me if it employs such silliness

or similar.


Click to expand...

Jass is not an indented based language such as python.

It's really verbose and blocks are clearly identified with their begin & end keywords.

I don't see anything bad with :

 JASS:

loop
exitwhen <condition>

 // stuff

endloop

It highlights when the end of the loop occurs.

Quite same for locals, with a new empty line between the function declaration and the local variable it seems fine.

Even if personnaly i prefer to add an indentation for locals declaration.

I also prefer to add a new empty line just after the function declaration, for the readability of the arguments and which type of value is returned.

At least it's a matter of opinion, it's 100 % subjective and i don't see anything funny, nor silly in that.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

The point is to coincide with the existing standards that were set by Blizzard.

Their entire common.ai library, cheats.j library and blizzard.j libraries were formatted this way. To change it now "just because" goes entirely against what these guidelines stand for.

Your argument could be manipulatd with JASS "not being indentation based" into arguing for no indentation at all. It's a very slippery slope especially with people like yourself who have preference-based arguments for "doing it your own way". It is this kind of thing that I recommend for building your own map, but for public resources, it's best to stick to uniform design.

I have personally found many of your JASS snippets very strangely formatted and with too much whitespace, so it's no surprise to me that you might have some objection to a certain format.

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

While i'm agree it's fine to follow Blizzard's standard, i still don't see anything silly or funny with it (exitwhen and locals).

But ok it doesn't follow the standard, i have to agree with that.

Also i've never talked about no indentation at all, plz don't use false logic.

And subjectivity is subjective.

Btw, you haven't answered to my first question (but since i've edited my post maybe you haven't seen it yet).

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

It is obvious where the value came from and I made that clear by quoting the sources.

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

But blizzard.j is the root of the devil  (joke)

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

 This is so much better than Mag's. No offense to Mag, of course.


Click to expand...

None taken

I'm already in the process of changing it to a Speedfreak tutorial ;P

 The English language has certain syntax like sentences, commas

and paragraphs, and tends to follow a general syntax pattern in

printed works. The content of the writing is obviously very diverse

but the way it is presented is the same, even though there is tech-

nically "nothing to stop you" from screwing up your grammar and

writing your message anyway. It's just no guarantee that everyone

can make sense of it and/or be able to take you seriously, specially

if u rite lyk thiz.


Click to expand...

Booooooooring XD

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Magtheridon96 said:

 Booooooooring XD


Click to expand...


That describes one external perspective of my lifestyle in a nutshell.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Bribe said:

 The point is to coincide with the existing standards that were set by Blizzard.

Their entire common.ai library, cheats.j library and blizzard.j libraries were formatted this way. To change it now "just because" goes entirely against what these guidelines stand for.


Click to expand...

Blizzard does not write in vJass and they create a lot of swapping function wrappers. Should we mimic that, too?

But that is why I hardly upload resources/only tell of the concepts as I cannot match up the standards and have too big of a custom environment.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

 That describes one external perspective of my lifestyle in a nutshell.


Click to expand...

I thought that paragraph was really good actually

I usually just go directly to the point. (Which isn't always good)

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

This behavior doesn't really work with womans (unless you have some Benjamin Franklin with you).

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Ofcourse it works with teh womens ;D

Directly to the point: "Hey, wanna fuck?"

#### Ignitedstar

#### Ignitedstar

 Level 5

JoinedJun 30, 2004Messages170

Can we have a thread where people keep their pants on WHILE talking about programming? xD

ANYWAY, as much as I see WaterKnight's point, I'd have to say that what you said is a bit unfair. Apparently they use them for GUI when all they do is redirect to the native. We don't know why they made those swapper functions, and even if we could ask them, they would probably not tell us since if you think about it, JASS is... ten years old, now? Somewhere around there.


 Last edited: Sep 28, 2011

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

But that means that we can completely neglect blizzard.j. As for me, I even neglect natives because of having my own wrappers for everything, only writing them at the lowest levels of the header. Now, I see that this does not work as a lot of people use natives in the public resources and the requirements should be manageable. Maybe, at some point, the native functions could be renamed.

In the end of the day, the conventions shall be beneficial, so I do not think that an old standard laid out by Blizzard would last forever.

Also have to repeat my question from the other thread: Why CONSTANTS_VARIABLES in big and non-constant globalVariables in camel? Other than Blizzard does it this way.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

There is no other reason. If everybody does their code differently then it's

spaghetti code all around, and then for each resource there is a big learning

curve just to understand how it works.

Rather than being biased and making up my own layout, which as has been

touched on would not be objective any more, Blizzard were the ones who

actually designed the syntax of the language and this is how they presented

it to us, from a time when they could have done anything they wanted with

it.

Obviously not everyone has the same preference in art but in the end there

are some things that should be standards in programming because otherwise

we are all doing things differently and the learning curve is huge.

Obviously you do not have to re-code your resources to conform to such a

standard but a big factor for me in approving resources is ease of use and

ease of implementation (hence why there are still so many Nestharus codes

still pending).

Your resource may be so easy to use and implement already that there may

be an exception made to the rules, but from what I have seen strang coding

style and strange API tend to go hand in hand.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

 How did you come with this value ?


Click to expand...


He meant like this...

 JASS:

function four takes nothing returns nothing
12341234
12341234
12341234
endfunction

 ...the following guidelines will ensure that we have a uniform standard to adhere to general readability.


Click to expand...


in the end it's up to the user/coder on how he will create his code whether it'll be for public or for self...a moderator can't just reject a spell/system if he writes like this...

 JASS:

private function DR_DIES...


instead of this...

 JASS:

private function DrDies...


the rules simply do not imply readability...But I do agree that the second method is more presentable than the first one...

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

I did not speak out against a standard, only said that the standard should be well chosen and does not have to cling to Blizzard. vJass and JNGP would not have been developed if people did not feel the necessity in advancing.


@mckill2009: And Troll-Brain probably meant to ask why precisely 4 spaces and not 1 or 2 or 3 or 5...

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

I swear there is more objection to what has been written in this thing when I'm just outlining the most basic things.

Obviously I've struck a nerve too deeply. Controversy was not the intention here.

Perhaps I should rename this to something like "General Guidelines for Standard Readability for people who are looking for such a thing".

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

So what did you expect for answers in this thread?


I repeat that I am not against the general idea but you have proposed a tutorial and the contents and detail can be discussed. It is the same with other tutorial threads and presented resources.


@mckill2009: Of course, a moderator can reject this if it does not obey the guidelines.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

I think then that this should just be a set of standards that can be

taken at face value, that do not have to be implemented but are

rather "one option of doing it".

I think comparing this to the English langauge syntax was a big

straw man argument, now that I dig a little deeper.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

You may force these rules all the way you like. You are the moderator and have to apply a structure with your staff.

You created an open thread for it and reasoned back, implying that you actually want to discuss the matter. Now do not be so whiny when people tell their point of view. Also just a few have replied yet.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

 @mckill2009: Of course, a moderator can reject this if it does not obey the guidelines.


Click to expand...


If I dont follow the rules, I'ts OK if my spell is rejected but if I dont


follow the guidelines and still my resource is rejected, I must say that it's a total BS...

 I think comparing this to the English langauge syntax was a big


straw man argument, now that I dig a little deeper.


Click to expand...


got it right, coz seems like the guidelines you imply is like "you must do this", instead of "i suggest/recommend this"...

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Well the idea here has been to make it a rule, yeah my choice of

the word "Guideline" was ill-chosen because it implied that it was

optional.

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

A forum is not a a democracy anyway, make it as a (v)Jass submission rule.

Same for short (obfusced) names, they shouldn't be tolerated, if the author really care about it, he could post this "optimized" version together with the one which is readable.

#### Ignitedstar

#### Ignitedstar

 Level 5

JoinedJun 30, 2004Messages170

mckill2009 said:

 If I dont follow the rules, It's OK if my spell is rejected but if I dont follow the guidelines and still my resource is rejected, I must say that it's a total BS...


Click to expand...

Um, I think you need to rephrase that because this sentence says something that I don't think it should be saying. Mind telling us what this resource was? Not everything needs to be a public resource. Especially if it's easy to make.

@WaterKnight: From what I've been reading, I understand Bribe's point very clearly, though yours is a little foggy. Keeping a standard is what a person wants to do if you're submitting a resource to a public space, like the Hive. There's a reason why JESP got called a standard.

People can code stuff in Warcraft in any way they want. Bribe's point was never "You HAVE to do this a certain way." BUT, if somebody is planning on submitting their code as a resource, it needs to follow rules. This is why he (and to a point, Mag) created this thread. This way, people who submit something don't have to explain how things work a thousand times just because the coding language and/or format is different.

But then, you're saying that there's nothing wrong with the standard; it's just that Blizzard made inefficient ways of coding through blizzard.j (and common.j too?) and it should be avoided. It's true that Blizzard made the language, but from my knowledge, anyone who codes in JASS is always told to avoid blizzard.j as much as possible because of its general inefficiency and nonsensical functions. At least, this is what I was told when I started learning JASS five years ago.

Scopes, modules, libraries, and structs were introduced by Vexorian and vJass. There's also ZINC and whatever-else that has been made. I believe one of the reasons why ZINC was made is because, well... Zinc Is Not C. And since Jass is based off C... In any of these cases, none of these additions weren't made by Blizzard. Therefore, I don't think these are even relevant to the argument because I believe we're talking about Blizzard's functions.

My deduction raises a contradiction. You two (WaterKnight and Bribe) generally agree on the same thing. Blizzard.j is flawed, any JASS user would agree (any decent programmer would probably agree-- but don't forget that JASS is nine years old). I honestly don't believe there was an argument to begin with. So how did this happen? If I'm wrong anywhere, feel free to point it out.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

I understood that Bribe wants to enforce a standard for public submissions, which I generally agree on. At least to determine what can be featured by the site (be approved).

The starting post presents a model for these guidelines from what Bribe has in mind. And this model in parts was criticized and/or scrutinized/questioned (which seems legitimate since its a normal tutorial thread open for repliers and to be reviewed). As a counter argument to some of these points, Bribe mentioned his proposed ways would be better because of adhering to Blizzard's style whereas I tried to invalidate the sense of this point by displaying some of its flaws we all agree on and that we already use vJass (which this tutorial includes), so people do not actually stick to old patterns of Blizzard in whole and there can be better ways, especially in consideration of a progressing map making process. Was not so long that LUA scripts were not permitted, was it?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Guidelines are nice, they make code become uniform, and so long as the guidelines are proper, they make code become more readable.


As for the guidelines, I agree with what Bribe has stated.


For indentation, I believe the reasoning for 4 spaces is the fact that it is the amount of spaces you receive from one press of the tab key in TESH. Of course, that was not the initial reasoning, but since most JASSers use NewGen coupled with TESH, it makes sense that we make the guidelines in accordance to what is done most.


I suppose it isn't necessary to restrict it, but you have to keep in mind that we are looking for uniformity.


As with the other stuff. The easiest way to achieve uniformity is to use what is used most. Allowing too many variations obviously won't achieve the same results. All of the arguments are completely valid, and it is fine to argue to rework the guidelines if necessary, but for now I believe these will do, no?


I'll leave this in submissions for a while for discussion, unless we get a flow of hearty approvals in which case I'll gladly move it.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

 Um, I think you need to rephrase that because this sentence says something that I don't think it should be saying. Mind telling us what this resource was? Not everything needs to be a public resource. Especially if it's easy to make.


Click to expand...


I didnt say anything wrong so why should I rephrase that?...anyway it's up


to them if they wanna make it a rule, it's OK for me anyway but until then


it's still a guideline, not mandatory...

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

It's not going to be a rule unless there is overwhelming support to make

it one.

So far there isn't. So for now this is "a very recommended way" to write

code, and if someone has any alternative suggestions on a standard way

to write their code then their input is appreciated.

So far the arguments against this are basically "no standards at all" and

not so much arguments against any particular point while supporting any

others.

I can see how vJass-type things such as library, module, struct, scope,

etc. might be contested, for example, but no one has given me very

constructive criticism here, for example it could be argued I should just

leave the vJass-exclusive things out and only clarify the normal JASS

things.

#### Ignitedstar

#### Ignitedstar

 Level 5

JoinedJun 30, 2004Messages170

mckill2009 said:

 I didnt say anything wrong so why should I rephrase that?


Click to expand...

Unless it means what you want it to mean, let me explain to you in detail how what you wrote may not make sense.

mckill2009 said:

 If I don't follow the rules, It's OK if my spell is rejected, but if I don't follow the guidelines and still my resource is rejected, I must say that it's a total BS...


Click to expand...

"If I don't follow the rules, it's okay if my spell is rejected." Nothing wrong here. It's the next sentence.

If I don't follow the guildlines and still my resource is rejected, it's total BS. Read that carefully and you'll realize there is something that's... off.

What you wrote says that what you make doesn't follow the guidelines for a resource, they get rejected and it's BS.

Of course the resource would get rejected. It doesn't follow the guidelines. This is a double negative-- two negative statements put in the same sentence are making a positive statement. In other words, you expect resources that don't follow guidelines to be approved. In addition, it raises the unfortunate implication that you may also expect resources that do follow the guidelines to be rejected.

I believe that don't is supposed to be do. It makes tons of more sense if you follow the guidelines, but your resource is rejected.

---

@Bribe: I think that decision is too rash. You've done a great job; the first three paragraphs make amends to the issues that have been presented. I don't see anything wrong with a JPAG, but this is coming from someone who has been taught JASS using JSEP. I'm used to seeing submitted spells being read this way.

The purpose of this tutorial is to show how people should submit their resources if they want them approved; It says nothing about how people should code their work otherwise, especially if they're not going to submit anything.

The reason why the things written in the tutorial are there is for consistency. It makes code easier to read. If another can't read your code, but you're submitting it, there's no point. Why submit a resource that no one can read? Or use, for that matter? Of course, when it comes to using stuff, that's what comments and instructions are for. That's the discretion of the maker, not the user.

I don't even remember Bribe writing that JPAG will replace JESP. A person can use either. I really don't see a problem.

---

@WaterKnight: You keep (or kept) stressing the point of "why are you using Blizzard's standard when it's not normal". Well then, what do you mean by normal? All programming languages use different formats when it comes to syntax. Unless you're trying to say that Java and Python look identical. If you're always using your own wrappers anyway, why this is a problem? And I have to ask: Do you plan on submitting anything? If no, don't worry about it.

This piece may sound very childish, but I have to ask. You keep asking people why use something like Blizzard's syntax for certain things, but I can equally ask you, "Why not?" I just don't see the point in fighting over whether a constant should be named this_constant_does_something over THIS_CONSTANT_DOES_SOMETHING. The constant will still do the same thing, it's just one is all capitals and the other isn't. Either one isn't saving any bytes. "Why are we doing this way?" is a good question, but "why is it a problem when JASS is already like this?" is probably a better question.

If it really bothers you so much that it's completely unacceptable, then I turn over to the answer I gave Magtheridon in his 'Jass for Speedfreaks' topic: "Write your own programming language for Warcraft." Or, simply enough as what I've been trying to say for the last couple of days: "Don't submit anything to the Hive." There are plenty of other sites other than the Hive that will accept your resource, like the TheHelper.

If it doesn't bother you that much, then I really would like an opinion with decisive evidence of why the method you suggest is better for people whom submit their resources than what Bribe has already written.

I probably sound like an asshole at this point, but the point stands: I don't just understand how this is such a big deal.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

mckill2009 meant it that way that guidelines should not be obligatory because he/she understood the word "guideline" as a loose rule you only may stick to. Bribe also agreed on this and admitted the term could have been misleading as he actually wanted to enforce it to be a serious, fix law.

Where have you read that I think of Blizzard's style as abnormal? I have not compared it to any other language, nor claimed that an indentation of 4 is good or bad for example, only that being an "old and original" style might not be the most crucial argument to keep it. If Blizzard's .j files were totally obsfucated, would you still uphold it? Now, we can talk about what were the thoughts and benefits Blizzard saw while developing this language and compare it to our targets and current situation. As I said, the scene evolves, so the whole perspective might change in time. Though, and this was mentioned just some posts ago, I agree to the demand to (at least temporarily) maintain backwards compatibility, as many of the current uploads are in traditional format.

The references to me using wrappers should have illustrated that some other style may be better matching with newer resources and that the native functions being OfThisFormatAtTheMoment may be up to us to change or to decide in future. So this would be no mandatory barrier.

I have asked questions to Bribe for the simple reason that he made suggestions and his reasoning from the starting post did not convince me. Would you not have asked me if I announced that all vars should have the suffix "Var" in it? If you want to insist on your point, that's okay, if you elaborate and try to reason it, be ready for backfire. And the big deal was more like in the art of debating.

#### Ignitedstar

#### Ignitedstar

 Level 5

JoinedJun 30, 2004Messages170

If he meant it that way, I would have certainly hoped he wouldn't need someone else to elaborate that for him. I can speak from experience to say that when bypassing occurs in writing, 90% of the time it's not the reader's fault. I want to see him type that himself in his own words.

What you've written is pretty much the reason why I posted a whole mess; I was confused. There are a few finer details that I'd like to discuss, but I really don't have time to write something that will take me another three hours. I was under the impression that there was a difference in opinion because of a problem. But there is no problem. This lonely fact makes my entire argument null. The reason why I wrote all of that was because I was falsifying my own assumption; I needed to verify whether or not a problem existed.

The only reason why any of this seemed to have happened is so people can acknowledge another person's opinion. This certainly isn't a bad thing, but I'm the kind of person who would like some progress out of a conversation. So far we haven't accomplished anything. I'm sorry if this offends anyone, but so far my brain is translating this as a waste of time. So are we actually going to do anything?

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

@Ignitedstar


I think I already answered that in post #23...

 got it right, coz seems like the guidelines you imply is like "you must do this", instead of "i suggest/recommend this"...


Click to expand...


furthermore, it was clarified in post #24 that it's optional...


Bribe & WaterKnight understands what I mean, why can't you?...


lastly, I'm not really good in english so pls pardon me if I have errors in writing, peace buddy, case closed!...

#### Ignitedstar

#### Ignitedstar

 Level 5

JoinedJun 30, 2004Messages170

It doesn't look like you were paying attention to my explanation of why what you wrote could be taken the wrong way. But since this is the Internet, I guess that's expected.

I think it's funny how the conversation ends with "So are we actually going to do anything?"

I'll take that as a, "No."

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

I think it's not good that a moderator has his own unofficial rules for a submission.

Indeed, Bribe has clearly said that he won't approve a code which doesn't follow these rules.

And honestly they are good enough, so why not make them official ?

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

I have re-written some of the more contested parts and re-iterated

my arguments to better present why this should be the standard. I

appreciate anyone's time to give this another looking over to see if

there are still things too controversial to be included.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

FunctionNames start with an uppercase and each new word is

indicated by a new capitalitzed letter. FunctionName. StructNames,

ModuleNames, LibraryNames and ScopeNames should

also follow this format.

The reason why these non-JASS names should be capitalized is

just for English-language syntax and syntax of many languages.

Names are always capitalized in English. Save for variable names

and method names (reasons outlined below), keeping to this

convention is ideal if we want a uniform standard.

variableNames and methodNames should begin with a

lowercase letter and each new word seperated by a new capitalized

letter. This is called camelCasing. variableNames, this was designed

by Blizzard. methodNames are typically formatted like this in most

mainstream languages like C and are also formatted in such a way

with most of vJass resources, it makes sense that this should also

be the standard.​

Click to expand...

"Names always start with a capital letter according to English. Of course, this rule only applies to the things I had in mind and sticks to the classic pattern I have no idea of why it was turned out this way."

May I help out with my thoughts what could be possible reasons for these standards? This does not mean that it would explain it all but could think part of.

The capitalization illustrates that the identifier is globally accessible. When writing a struct, you can only enter it from outside via the struct name or an instance. All members are subordinates, so they begin lower-case. Often, inside, users only write "call doSomething" without "this." or "." as an abbreviation because its visible by the lower-case already that we are still in the same scope. Appended words start big again to recognize them. CONSTANT_NAMES are yelled to demonstrate their solidness and to outline a contrast but I must say I do not use them in Wc3 very much, as they have very limited use in jass and most times I know from the name if it has to be a constant or not, or have to declare them as pseudo-constants, defining them once in init function/method. Functions/Methods have imperative names because they should do something. Variables are more passive attributes that are limited to themselves/do not have the might. An exception might be operators like myVar = value but I regard this as a listener, reactions follow on the variable's setting.

For the reasons mentioned, I would not write scopeless globals thisCase for example.


 Last edited: Oct 6, 2011

#### ShadowFlare

#### ShadowFlare

 Level 11

JoinedJun 30, 2008Messages580

Resources: 2

#### Resources
Spells 2

Loops:

I find this easier to read:

 JASS:

loop
exitwhen n > variable
 //Do Stuff
set n = n+1
endloop
set n = 0

Than:

 JASS:

loop
 exitwhen n > variable
 //do stuff
 set n = n+1
endloop
set n = 0

The first example seems easier for me to find exactly where the variables I want to change are.

Perhaps make this the standard?

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

For loops, after letting go of (some of) my ego recently the exitwhen statement being on the same line of the loop is pretty good, though the "set" statement does not look good there. That's why I don't define it as such a key rule. I say in this one it's a matter of preference, because JASS loops are so ugly no matter what we do.

#### ShadowFlare

#### ShadowFlare

 Level 11

JoinedJun 30, 2008Messages580

Resources: 2

#### Resources
Spells 2

Agreed, ugly they will always be. But here is another example:

 JASS:

 loop
 exitwhen a > slength
 loop
 exitwhen b > NA
 if (SubString(GetEventPlayerChatString(), a-1, a) == NOTALLOWED[b]) then
 set accepted = false
 endif
 set b = b+1
 endloop
 set b = 0
 loop
 exitwhen c > MC
 if (StringCase(SubStringBJ(GetEventPlayerChatString(), a, a), false) == SubStringBJ(Curse[c], 1, 1)) then
 if (StringCase(SubStringBJ(GetEventPlayerChatString(), a, a+StringLength(Curse[c])-1), false) == Curse[c]) then
 set curse = false
 endif
 endif
 set c = c+1
 endloop
 set c = 0
 set a = a+1
 endloop
 set a = 0

Nested Loops look a lot cleaner in this format, with the set and exitwhen variable on the same line, or else it would look like this:

 JASS:

 loop
 exitwhen a > slength
 loop
 exitwhen b > NA
 if (SubString(GetEventPlayerChatString(), a-1, a) == NOTALLOWED[b]) then
 set accepted = false
 endif
 set b = b+1
 endloop
 set b = 0
 loop
 exitwhen c > MC
 if (StringCase(SubStringBJ(GetEventPlayerChatString(), a, a), false) == SubStringBJ(Curse[c], 1, 1)) then
 if (StringCase(SubStringBJ(GetEventPlayerChatString(), a, a+StringLength(Curse[c])-1), false) == Curse[c]) then
 set curse = false
 endif
 endif
 set c = c+1
 endloop
 set c = 0
 set a = a+1
 endloop
 set a = 0

IGNORE TEH BJ's

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

May be useful for the standard FOR loop to point out the actual loop body independent to the loop's framework. I usually put in a blank line though. Generally, I divide the parts more, for example by a separation line between the loops and what has to be done after another action.

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

I'm totally with you ShadowFlare, in simple incrementations loops, it just makes the code more readable.

EDIT : And with you too WaterKnight ^^

We are lacking something : a way to clearly see if a variable is global or local (and incidentally avoid conflict names, even if it's not really a problem in vJass with the private keyword).

Blizzard uses the prefix "udg_" for the user defined globals and "bj_" for blizzard jass : blizzard.j known as "GUI", should we use "g_" ?

Well, i think it's too much for many of them, but personnaly it hurts my eyes when i see a one lower case letter for a global variable such as "q" or whatever.

Personnaly i use that :

CamelCase for globals and camelCase for locals.

Usually i use verbs for functions and names/adjectives, whatever except verbs for variables, i mean the name by itself should be clear enough to make the difference between a function and a global variable.

But there is an exception the functions which returns a boolean and a boolean global variable, both could be written IsSomething. (ofc functions used in a boolexpr but where the returned boolean doesn't matter are not concerned)

Sometimes i also use This_is_a_global_variable, this_is_a_local_variable if the name is long enough.

I'm not pretending my ways are good, just suggesting something because actually there is nothing good about it.

And it's way more obvious to see a difference between a function and a global variable rather than a global and a local variable if they use all the same syntax.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

I write global variables like it was suggested here to write constant variables. Seems really much more useful for me to know if a variable is global or local than if it is constant/not or even more constant in jass meaning. There are only a few scopeless global variables that are primitive code support like NULL or INVALID for a null struct instance, ARRAY_SIZE etc. And even these could be wrapped in scopes.

I really do not like it when struct instances are compared to numbers in resources.

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

Hmm i dislike THIS_IS_A_GLOBAL.

- Because of my azerty keyboard and Windows i've to switch to uppercase / lower case for letters and "_", even if it's not the case in linux : uppercase all the way. (trivial issue i suppose i could fix it on windows )

- For public resources it's really bad since you can't make easily the difference between the configuration part and the "hardcoded" one, even if i'm agree the config part should just be on the top.

- static ifs (even if it's quite the same reason as said above)

- I JUST HATE IT, WHY WOULD YOU CRY ?

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Well as struct instances are numbers, it makes sense to compare them to numbers. They aren't handles, after all.

I agree that globals are not easy to differentiate between locals, I tend to code all my non-constant globals inside of a "struct X extends array", and when I reference them I always include the dot as a prefix.

If a var shares the same name as its "getter" I usually affix it with a V. Like a method operator that is readonly, I use methodOperatorName and methodOperatorNameV for the thing.

Obviously this one doesn't have to be the way I do it, because it's a bad excuse to code everything inside of a struct just to tell the difference between global and local. I think the place of all caps should be reserved for constants, personally. But maybe Troll-Brain is onto something with using underscores_for_globals instead of camelCasing.

I'm not going to edit the first post unless we can get a real good understanding on globals, or I could just leave it up to the user on this one since it's not as big a priority.


 Last edited: Oct 6, 2011

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

Aha, i've found the point where nobody uses the same rule

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Troll-Brain said:

 Hmm i dislike THIS_IS_A_GLOBAL.

- Because of my azerty keyboard and Windows i've to switch to uppercase / lower case for letters and "_", even if it's not the case in linux : uppercase all the way.


Click to expand...

That would not be a great difference to camelCase though. Well and yes, querty you can write the word in one go.

Troll-Brain said:

 - For public resources it's really bad since you can't make easily the difference between the configuration part and the "hardcoded" one, even if i'm agree the config part should just be on the top.


Click to expand...

Option thingies should really be in an extra paragraph but if you want it at top, you should also have an initializer there as you cannot do everything in globals like filling arrays.

Troll-Brain said:

 - static ifs (even if it's quite the same reason as said above)


Click to expand...

Elaborate please.

@Bribe: But we do not want to consider them everywhere as numbers, only if it's for dynamical and low-level reasons and then there could be a basic type referring to all structs. Additionally, the 0 being an invalid struct instance was introduced by JassHelper. It might be different with another allocation etc.

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

WaterKnight said:

 That would not be a great difference to camelCase though. Well and yes, querty you can write the word in one go.


Click to expand...

For me, it does (not talking about the keyboard thingie, just about camelCase).

 Option thingies should really be in an extra paragraph but if you want it at top, you should also have an initializer there as you cannot do everything in globals like filling arrays.


Click to expand...

Yeah i know, and yes sometimes i use an initializer in the top such as :

 JASS:

library MyLibrary initializer init ...

private function DoInit ...

private function init ...
 call DoInit()
 ...

 Elaborate please.


Click to expand...

Just to make clear that the needed paramater must be a constant. (also i suppose if you use a non constant one, it will considers it as FALSE instead of displaying an error on save, but i could be wrong about this last point)

 @Bribe: But we do not want to consider them everywhere as numbers.


Click to expand...

Correct me if i'm wrong but the typecast integer <-> struct is implicit only in the way struct -> integer and must be explicit in the way integer -> struct.

Elaborate plz.

Also usefulness or not, pretty much many languages use a different convention for a constant or not variable.

#### ShadowFlare

#### ShadowFlare

 Level 11

JoinedJun 30, 2008Messages580

Resources: 2

#### Resources
Spells 2

Global; i.e public integer Attribute = 0

CONSTANT private constant integer ATTRIBUTE = 0

localName local integer dX = 0

Whats wrong with this?

 You must log in or register to reply here.


### Similar threads

 vJass: Advanced Initialization


TriggerHappy

Jan 29, 2014

JASS Tutorials

Replies23Views22K

Jun 1, 2025
DarknessFalls003

 Structs for Dummies


Magtheridon96

Jun 15, 2011

JASS Tutorials

Replies19Views26K

May 3, 2016
Zwiebelchen

 Vanilla Jass Header Initialization


Almia

Aug 27, 2013

JASS Tutorials

Replies8Views6K

Aug 31, 2013
Rheiko

 [vJass] SleepAction - A Cinematic Lifesaver


Bribe

Aug 25, 2010

JASS Tutorials

Replies13Views24K

Nov 27, 2021
Bribe

Locked

 xJASS Compiler Survey


Nestharus

Mar 3, 2015

Advanced Scripting

Replies10Views3K

Mar 18, 2015
Ruke

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