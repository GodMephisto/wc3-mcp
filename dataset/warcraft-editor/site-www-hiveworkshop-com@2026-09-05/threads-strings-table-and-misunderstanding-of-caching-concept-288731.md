---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/strings-table-and-misunderstanding-of-caching-concept.288731/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Strings Table and Misunderstanding of Caching Concept
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/strings-table-and-misunderstanding-of-caching-concept.288731/>
#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

WC3 written on C and uses typical caching method when it comes to strings. Every existing string being cached into virtual table which may improve mem use while working with tons of strings with the same content. You can learn more about it over the internet.

I've been using async strings in my LoD for a long time. I never knew about any kind of problems there, since I came up with jassing when v1.26 already been there. That means I totally miss glorious return bug times. So I asked the guy who had some experience back then about what he knows about desync caused by strings.

He answered that there was typecasting of strings to IDs, which actually means to their table's index.

Obviously, strings tables aren't synced - what the purpose? It's inner object with no purpose but to speed up perfomance. So chinese and english and russian players could met in the same map, having totally no issues about syncing. Remember - every string, every description, every tooltip is a string, which is cached and has some address and table index.

So, what been about desync. It happened when somebody decided to use string's index as a key for storing info. I believe it was due to lack of StringHash back then. Obviously whenever 2 players met, and they have different wc3 language, different CustomKeys settings or even modified map, bad things could happen.

But hey, it's 2016 already, and .24 with StringHash and removed RB been here for 8 years already. We don't care about indexing anymore. We just dont have to. We can declare any string we want locally, any amount of them, without any doubt.

TLDR: Stop false promising decyncs if somebody uses local declared strings - it wont' happen. You can try to proof desync by a real example tho. Until then it's a myth.

#### Aniki

#### Aniki

 Level 13

JoinedNov 7, 2014Messages571

Resources: 21

#### Resources
Tools 1Maps 1Spells and Systems 14JASS 3Tutorials 2

Another string table myth.

 So, what been about desync. It happened when somebody decided to use string's index as a key for storing info.


Click to expand...

Even then, just storing info doesn't cause a desync:

 JASS:

function store_info takes nothing returns nothing
 local integer i
 local string s

 // assuming "string-A" and "string-B" have not been added to the string table yet
 if GetLocalPlayer() == Player(0) then
 // Player(0)'s "string-A" would have an id of say X
 set s = "string-A"
 set s = "string-B"
 else
 // for other players "string-A" would have an id of X + 1
 set s = "string-B"
 set s = "string-A"
 endif

 set i = GetStringId("string-A")
 call BJDebugMsg(I2S(i))

 set i_info[i] = 1
 set r_info[i] = 6.28
 set b_info[i] = true
 set s_info[i] = "Z"

 call BJDebugMsg(I2S(i_info[i]))
 call BJDebugMsg(R2S(r_info[i]))
 if b_info[i] then
 call BJDebugMsg("true")
 endif
 call BJDebugMsg(s_info[i])
endfunction

The GetStringId function is from here.

 Until then it's a myth.


Click to expand...

Agree.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Warcraft 3 was written on C++ actually.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

A Void said:

 Warcraft 3 was written on C++ actually.


Click to expand...

I dont know how to distinguish one from another ,since C++ extends C, and we only have decompiled sources to guess.

Aniki said:

 Even then, just storing info doesn't cause a desync


Click to expand...

game cache updates (normally used for storing data before hashtables) causes sync call (SyncSaved*) to have it common between all clients. I can imagine some clients being dropped due to incorrect data sent, but wont say for sure.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

DracoL1ch said:

 I dont know how to distinguish one from another ,since C++ extends C, and we only have decompiled sources to guess.


Click to expand...

C++ and C are completely different languages. Recent features render them incompatible which each other. Only a subset of C is possible in C++, with the rest throwing syntax errors.

DracoL1ch said:

 game cache updates (normally used for storing data before hashtables) causes sync call (SyncSaved*) to have it common between all clients. I can imagine some clients being dropped due to incorrect data sent, but wont say for sure.


Click to expand...

There is no needed for "SyncSaved" call because data written is implicitly already identical between all clients. SyncSaved was obviously intended to allow multiplayer use of GameCache where it would sync values stored locally between players at map initialization, however since data persistence is not possible it never had any practical purpose except for some more modern synchronization techniques.

DracoL1ch said:

 StringHash


Click to expand...

StringHash cannot be used reliably because of the low bit length of the resulting hash. Professional cryptographic hashes have hash lengths of 192 bits or more, StringHash only has 32bits. If you do the mathematics after just a few hundred hashes the chance for a collision starts to become significant. At least 1 person has already complained that an error in his map was caused by a hash collision of StringHash.

DracoL1ch said:

 TLDR: Stop false promising decyncs if somebody uses local declared strings - it wont' happen. You can try to proof desync by a real example tho. Until then it's a myth.


Click to expand...

My proof is from observations with examples. many people suffered from desyncs until they got rid of the locally created strings after which the desyncs went away. The strings were used for model paths or text so did not alter game state (eg the effect was still created for all players, just using a different model path).

DracoL1ch said:

 Remember - every string, every description, every tooltip is a string, which is cached and has some address and table index.


Click to expand...

This makes the assumption that the entire game engine uses the same table for everything. Which is almost certainly wrong. A brief examination of the game shows that strings can (well could years ago when I did the test) be located in as many as 3 different spots in memory at the same time. It is completely possible that the JASS system has its entirely private string table and management system which is decoupled from the rest of the game engine which must deal with tooltips and other such strings. Additionally tooltips and other such strings always exist between all clients, even if the string itself is different due to localization. Also the sync problem was reported with locally creating strings such that the string table of one client features 1 more string than the other clients, something which cannot be caused by varying locales.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

Dr Super Good said:

 My proof is from observations with examples. many people suffered from desyncs until they got rid of the locally created strings after which the desyncs went away. The strings were used for model paths or text so did not alter game state (eg the effect was still created for all players, just using a different model path).


Click to expand...

yet you didnt provide them, so it as well can be outdated versions OR totally different things

Dr Super Good said:

 It is completely possible that the JASS system has its entirely private string table and management system which is decoupled from the rest of the game engine which must deal with tooltips and other such strings.


Click to expand...

Storm.dll takes care about all strings, while game only runned by Game.dll. so it's totally legit to assume so

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

I do not remember ever experiencing a game desync that I could trace back to a pure string asynchronity and the only idea that came to my mind why this would make sense was that some network traffic like chat messages could be represented by string table lookups instead of transmitting the raw characters everytime. Please provide a concrete case. Also this is one of the questions that Blizzard should easily be able to answer.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

That it's written in "C" or not does not really matter, just make it straight forward/ on topic.

You could link to Purge's String tutorial, they should have or get an idea about the string table at least.

DracoL1ch said:

 TLDR: Stop false promising decyncs if somebody uses local declared strings - it wont' happen.


Click to expand...

You really mean "declared"? ...

DracoL1ch said:

 I've been using async strings in my LoD


Click to expand...

... and you are using it in what ever LoD is? I hardly can imagine this does work as explained. What is your LoD example?

DracoL1ch said:

 But hey, it's 2016 already, and .24 with StringHash and removed RB been here for 8 years already. We don't care about indexing anymore. We just dont have to. We can declare any string we want locally, any amount of them, without any doubt.


Click to expand...

I honestly don't understand this explaination. Could you elaborate why declaring local strings should be safe now?

As far as I remember, I always had to declare strings globaly, and then made the initialization localy, otherwise I had issues.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

IcemanBo said:

 .. and you are using it in what ever LoD is? I hardly can imagine this does work as explained. What is your LoD example?


Click to expand...

legends of dota, top2 custom map after dota itself

IcemanBo said:

 Could you elaborate why declaring local strings should be safe now?


Click to expand...

because strings were used as keys for gamecache. nobody should use gamecache today, so it's safe to say it's irrelevant

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

How does it look like to declare local strings?

Or you do only mean local registration in the string table, which would make more sense, like localy printing random debug messages.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

 JASS:

global string s="totally sync string"

local string s="sync string"
local string s2="sync string"
if localplayer==myplayer then
 set s="async string 1"
 set s2="async string 2"
endif

the first string usage is it's own declaration, I didn't mean it declares like variables

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Yeh, okay, but all clients have the strings declared and initialisized, it's just differently assigned for local clients, which is okay.

It could be easily clarified with an example I guess, I also got a sligthly wrong intention first when reading.

People often made did something like localy creating handles in combination with local strings, or did global creation, but with local initialiaztion and such, and then got errors,

but plainly what you show above I also think is perfectly okay.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

IcemanBo said:

 That it's written in "C" or not does not really matter, just make it straight forward/ on topic.


Click to expand...

thats whole reason why string hash exists at all, and it's should help to understand why it used to be a thing back to 2006

 Code:

int sub_6F4C4E60()
{
 int v0; // ecx@1
 int v1; // ebp@1
 int v2; // eax@2
 int v3; // ebx@2
 int v4; // ebx@2
 _BYTE *v5; // edi@4
 unsigned int v6; // esi@6
 int v7; // eax@12
 int v8; // eax@14
 int v10; // [sp+14h] [bp-Ch]@1
 int v11; // [sp+18h] [bp-8h]@1
 int v12; // [sp+1Ch] [bp-4h]@4

 v0 = dword_6FAB7D14;
 v1 = 0;
 v11 = 0;
 v10 = 0;
 if ( dword_6FAB7D14 )
 {
 v2 = *(_DWORD *)(dword_6FAB7D14 + 12);
 v3 = v2 < 0;
 LOBYTE(v3) = v2 <= 0;
 v4 = v2 & (v3 - 1);
 if ( v4 > 0 )
 {
 while ( 1 )
 {
 if ( !v4 )
 goto LABEL_19;
 v5 = (_BYTE *)*sub_6F4C4300(v4 + 8, &v12);
 if ( v12 )
 Storm_403(v12, "e:\\Drive1\\temp\\buildwar3x\\Storm\\H\\stpl.h", 2601, 0);
 v6 = *(_DWORD *)(v4 + 4);
 ++v11;
 if ( v6 > 1 )
 {
 if ( v5 )
 {
 if ( *v5 )
 {
 v1 = v6 + v1 - 1;
 v10 += (v6 - 1) * Storm_506(v5);
 if ( v6 > 0x10 )
 nullsub_1("%s referred to %d times\n", v5, v6);
 }
 }
 }
 v0 = dword_6FAB7D14;
 if ( v4 )
 v7 = v4 + *(_DWORD *)(dword_6FAB7D14 + 4);
 else
LABEL_19:
 v7 = v0 + 8;
 v8 = *(_DWORD *)(v7 + 4);
 if ( v8 <= 0 )
 break;
 v4 = v8;
 }
 }
 }
 return nullsub_1("strings: %d reuses: %d savedBytes: %d\n", v11, v1, v10);
}

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Some things is impossible to understand from showed code, like the (seemingly) important operations, or the random goto, but I anyways don't know what showed code should exactly help.

But that your statement about desync/async is clear in the tutorial seems more important to me, as if it's written in C or C++. I honestly could not argue about it anyways.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

IcemanBo said:

 but I anyways don't know what showed code should exactly help.


Click to expand...

just kind of proof of C-related behaviour in terms of working with strings

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

But isn't the most parts of C also simply applyable in C++, even there exist also other appoaches?

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

It needs some little re-working.

Outline important information, with possibly a code example and such.

Some text formatting and further linking to pnf's string tutorial was good.

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

DracoL1ch said:

 So, what been about desync. It happened when somebody decided to use string's index as a key for storing info. I believe it was due to lack of StringHash back then. Obviously whenever 2 players met, and they have different wc3 language, different CustomKeys settings or even modified map, bad things could happen.


Click to expand...

Hold on, why is that string different for each player in the first place? Did the JASS script retrieved a string different to each other depending on the wc3 language used. Also, could the same thing happen when using ExecuteFunc?

(honest non-sarcastic questions)

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

Flux said:

 Hold on, why is that string different for each player in the first place? Did the JASS script retrieved a string different to each other depending on the wc3 language used. Also, could the same thing happen when using ExecuteFunc?

(honest non-sarcastic questions)

Click to expand...

ANY string inside the quotes passed through jass is a string which goes to stringtable

 JASS:

local string s="asd"
if LocalPlayer==MyPlayer then
set s="dsa"
endif

now only MyPlayer has string entry "dsa", rest have no value assigned.

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Got it, the string index returned from StringTable is different for each player due to stuffs like that which later causes desync. However getting the index of a string on the StringTable is still safe if StringTable remained sync at all times. (Make all players register the string then just null them

 JASS:

local string s = "asd"
set s = "dsa"
if GetLocalPlayer() != MyPlayer then
 set s = ""
endif

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

yes, yet there's barely any reason you will base anything on a string table anyway

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Maybe an alternative to StringHash if purpose is to get a unique id based on a string paremeter and not necessarily have to be consistent every game.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Submission:String Table Misunderstanding

Date:24 Jan 2017

Status:Graveyard

Note:

It's some discussable information to share, though it's currently not in an approveable "tutorial" state.

[JASS/AI] - Strings table and misunderstanding of caching concept

Ping us anytime when you find some time to work on it.

 You must log in or register to reply here.


### Similar threads

 Name Changing Tutorial (Strings Explained) [Reviewed: Ralle]


DarkArchon

Dec 16, 2007

Tutorial Archive

Replies12Views2K

Jul 19, 2008
Ralle

[vJASS]
 StringHashEx guaranteed no-collision string hash


Waffle

Feb 1, 2017

Graveyard

Replies3Views3K

Feb 14, 2017
IcemanBo

Locked

[General]
 GetLocalPlayer() and Strings


Tristronic

Mar 16, 2023

World Editor Help Zone

Replies10Views2K

Mar 19, 2023
Tristronic

Locked

 [Documentation] String Type


PurgeandFire

Sep 2, 2013

Advanced Scripting

Replies48Views20K

Oct 7, 2018
IcemanBo

[vJASS]
 Sync (Game Cache)


TriggerHappy

May 14, 2016

JASS Resources
2

Replies65Views25K

Oct 16, 2022
Frotty

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