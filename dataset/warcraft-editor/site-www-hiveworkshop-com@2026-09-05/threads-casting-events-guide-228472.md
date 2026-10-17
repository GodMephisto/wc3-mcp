---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/casting-events-guide.228472/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Casting Events Guide
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/casting-events-guide.228472/>
#### Starquizer

#### Starquizer

 Level 16

JoinedMar 3, 2006Messages1,564

Resources: 7

#### Resources
Maps 1Spells and Systems 3Tutorials 3

SPELL CASTING EVENTS GUIDE

Well, many mappers and spell makers keep asking the same question with different variations:

[RAINBOW]

"What is the difference between Begins Channeling/Casting and Starts the effect of an ability and which one should I use ?"

[/RAINBOW]

Here is the answer:

As you can see, this is a simple map for the sequence of MOST of the spells.

MOST ?

Yes, some spells casting time field takes place after the 'Start the effect of an ability' event.

For instance,

- Blizzard & Rain of Fire:

Casting Time = 1 and it is the time between each wave,

- Flame Strike:

Casting Time = 1.33 and it the time that you see a yellow black-hole-like effect before the flame pillar appears.

- Earthquake, Monsoon:

No, although this requires channeling but its not like the previous.

Now, for that word "Any Interruption Means"

There are many things that can interrupt your spell, I will talk about them now.

1) Move/smart/attack-move/attack/cast another spell:

Can be issued for spells that its effect not instant, like Blizzard, Earthquake, ..., etc and only after the "Start Effect" event; issuing the order before this point will queue the order and make the caster move to the destination after the spell finish.

2) Stop Order:

This immediately stops/cancels the spell casting and can be issued at any time after "Begins Channeling" and before "Finishes Casting"

3) Stun:

If the caster is stun at any time after "Begins Channeling" and before "Starts the Effect" he will re-cast the spell again provided that he isn't issued any other order during the stun period and he has enough mana for the spell but if the stun is after "Start Effect" then the spell is disrupted and the mana is wasted in case the spell needs mana.

4) Entangle/Silence:

This disrupts the casting completely, but if it is before "Start Effect" then no mana wasted.

ok, that's it. But you may be wondering about what I said early about casting time and 'Start the effect of an ability' event, there are tons of spells how to know about them.

To answer that, you must try by using the same method I used; which is a bunch of triggers to test the time and then changing the 'Casting time' and 'Cast point' fields from the object editor to different values.

The triggers are in the map attached with this guide.

I hope that answers your questions about spell casting events.

Thanks for reading.

 - Showed the Follow Through Time on the photo and map is updated with new one including a unit with channel spell. (credits to Maker)

#### Attachments

Casting Events.w3x

 20.1 KB

 · Views: 351




 Last edited: Aug 9, 2013

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

I'm glad that you made this since this seems to be a mystery to many users.

You could add that the follow through time of Channel, which is widely used as base ability for custom abilities, is the time between "Starts the effect..." and "Finishes casting".

#### Starquizer

#### Starquizer

 Level 16

JoinedMar 3, 2006Messages1,564

Resources: 7

#### Resources
Maps 1Spells and Systems 3Tutorials 3

@ Maker

Done, thanks for pointing that.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Oh you <3

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great job getting this information. Approved.

#### Wareditor

#### Wareditor

 Level 15

JoinedJan 16, 2009Messages764

Resources: 4

#### Resources
Maps 4

An instant interruption (in the form of a "stop" or "move" order) responding to the begins casting event or begins channeling event will stop the spell from triggering its effect event, its mana consomption and its cooldown but for an instant spell based on channel, the effect and the mana consumption will still trigger (but not the cooldown). In fact, the stop casting event will occur but the spell will keep going until its finish casting event.

A solution I have found is to delay the order by 0.1.

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

This is incredibly useful (and I didn't even have to necro-post!).

#### Map Designer

#### Map Designer

 Level 17

JoinedMay 2, 2011Messages1,351

Resources: 6

#### Resources
Maps 2Spells and Systems 4

Quite a valuable Tutorial. I learned from this tutorial that (Begins Channeling an ability) takes effect before cast time, while (begins casting an ability) only takes effect after cast time has already finished. now since most abilities have 0 cast time, these 2 should coincide (or to be exact begins channeling will always occur first, then directly followed by begins casting if cast time was 0)

Thanks!

 You must log in or register to reply here.


### Similar threads

Locked

 Now I am tired of spells that use the wrong events, READ!


Vexorian

Aug 18, 2004

Triggers & Scripts

Replies0Views1K

Aug 18, 2004
Vexorian

 How to: Create Spells in GUI - The Beginning


baassee

Aug 18, 2011

Trigger (GUI) Editor Tutorials

Replies6Views32K

Oct 22, 2011
PurgeandFire

 A Complete Beginners Guide to Hashtables


Jazztastic

Jun 15, 2011

Trigger (GUI) Editor Tutorials
2

Replies57Views45K

Nov 12, 2024
PurgeandFire

 Making any Spell Autocasting


Ralle

Mar 15, 2006

Trigger (GUI) Editor Tutorials

Replies12Views59K

Dec 22, 2017
Rogue Kessler

Locked

 Cast time and me D:


legorlan

Jan 30, 2011

World Editor Help Zone

Replies4Views1K

Jan 31, 2011
Maker

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Trigger (GUI) Editor Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top