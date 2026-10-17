---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/the-benefits-of-saving-information-to-dummies.307663/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# The Benefits of Saving Information to Dummies
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/the-benefits-of-saving-information-to-dummies.307663/>
#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

#
 The Benefits of Saving Information to Dummies

##
 Theory and Framing

Certain effects that modify the traits of a unit cause problems with MUI, or cause problems when they fire multiple times upon the same unit before the duration of the original effect has ended. This happens due to values being overwritten, causing Warcraft to "forget" about information it was given. This tutorial will focus on how we can utilize dummy units to save information and ensure MUI​

##
 Quick Disclaimer

The trigger examples in this tutorial use a unit indexer, which simplifies the dynamic indexing process for ease/readability. As such, anytime you see

Set handle = (Custom value of unit1)
Please be aware that this is in lieu of dynamic indexing. This tutorial will work the same way if you utilize hashtables as your MUI beast of choice.

##
 Our Method

Unit - Create 1 Dummy for player at (some point) facing Default building facing degrees

Set unit1 = (Last created unit)

Set handle = (Custom value of unit1)

Set unithandle[handle] = (some unit)
So what exactly are we doing here? After creating a dummy, we assign a variable to it. We then retrieve the custom value of the dummy to use it for dynamic indexing. We save the unit we are going to act upon using the custom value of the dummy. By saving the unit with the custom value of the dummy, we are binding the unit to the dummy.

What are the benefits of saving the unit we want to modify to our dummy? I'm glad you asked! Whenever some event fires (usually a spell being cast, but this could work with any event), we are creating a unit which will hold information for us. By creating a unit to hold information each time the event fires, we are able to make sure that each firing of the event works correctly on the unit firing the event, even if one unit fires the event multiple times before any of them have finished. We also ensure that the information pertaining to a unit isn't overwritten or changed prematurely!

So what are some practical applications of this technique?

Abilities that stack on themselves independently (each stack has a separate duration, instead of refreshing the duration for all stacks)

Abilities that modify hero attributes (multiple modifications without this method may cause permanent attribute gain/loss if not handled correctly!)

Multiple knock backs on a unit with different directions/forces (in cases where the knock back is saved to the unit being knocked back, another knockback from the same source will override the first)

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Or you could just use a new struct instance.

But I assume this is mainly for GUI.

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

Chaosy said:

 Or you could just use a new struct instance.

But I assume this is mainly for GUI.


Click to expand...

As denoted by the clearly visible GUI tag on the title.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Mhm. But even in GUI there are alternatives.

You should perhaps argue negatives and positives other approaches. For example, if I want to use hashtables normally, why should I use this method over a hashtable workaround

When I used GUI I worked exclusively with hashtables, unit indexers was a no-go


 Last edited: Aug 5, 2018

#### Pyrogasm

#### Pyrogasm

 Level 45

JoinedFeb 27, 2007Messages5,594

Resources: 1

#### Resources
Spells and Systems 1

I... really don't understand the point of this tutorial. As someone extremely familiar with the WE's ins and outs, it is very ubclear what this tutorial is going to teach me and why I would do this way over any other way is left unmentuoned. Both of those are important! Imagine an actual noob trying to grok this.

Your method here boils down to this:

Give units indices, somehow. Your method specifies "in lieu of a unit indexer" but does not explain what that is, why it's important, and how you should go about this if you DID have an indexer installed.

Use the unit's index as an array index for any data you might store.
That's... not really a method. It's literally using a unit indexer for what its supposed to be used for. Nothing more nothjng less. And at the end of the day you've just replaced GetHandleId() with GetUnitIndex() and used array set/read rather than a hashtable save/load. I get that maybe hashtables seem intimidating to GUI users, but they are so useful and versatile I would never recommend a GUI user do anything else. Especially since even though you can't do Handle ID of (YourUnitVariable), you can set some of the blizzard globals (like bj_lastCreatedUnit) and use the event response functions to get Handle ID of (last created unit).

Additionally, this method cannot be directly applied to attaching data to timers, one of if not the most common data types to necessitate attaching data in the first place. You also only have one example bit of code, which is imo insufficient for a tutorial (give complete example or something more complex as well).

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Pyrogasm said:

 That's... not really a method. It's literally using a unit indexer for what its supposed to be used for.


Click to expand...

The difference is that a unit indexer is traditionally attaching data to a unit that is directly involved in an event. (example, caster of a spell) Which means that casting two spells with the same unit usually overwrites values rater than stacking. So there is a difference in method.

This method makes it like a ghetto struct in a sense.

#### Pyrogasm

#### Pyrogasm

 Level 45

JoinedFeb 27, 2007Messages5,594

Resources: 1

#### Resources
Spells and Systems 1

The whole reason Unit Indexers were introduced was because the H2I return bug wouldn't always give you numbers you could use in arrays. Now that we have GetHandleId() that starts at 0 with no units on the map and the ability to go beyond 0-8191 in array indexes, they're not super necessary any more. Coming at the tutorial from that direction ("You could use an indexer but instead do this ") might make a certain amount of sense. What really needs to happen is Blizzard allows using GetHandleId() on variables in GUI, because right now you can only use it on event responses and a few other trigger editor functions (unless you manually set, for example, bj_lastCreatedUnit before its equivalent GUI event response that in your handle id call).

Chaosy said:

 The difference is that a unit indexer is traditionally attaching data to a unit that is directly involved in an event.


Click to expand...

Unless something changed drastically in the last two years, a unit indexer is explicitly a system that gives each unit a unique ID number that can be used for parallel arrays (or hashtables, or static struct instances, etc.). Sometimes the indexer saves the unit's index to its custom value, sometimes they save them in hashtables, sometimes their own structs using GetHandleId(). At the end of the day, all an indexer does is convert unique units into unique numbers for you to use. Attaching data to the unit is not a necessary function of the system, though many include the ability to do so. I find myself attaching data to timers far more often than I do to units.

Chaosy said:

 Which means that casting two spells with the same unit usually overwrites values rater than stacking.


Click to expand...

If my indexer gives me unique numbers for unique units, I never have to worry about data collision if I just make separate arrays for each 'thing' that uses the indexer. Get ID, save data in variables prefixed with "spellname_", and use that variable wherever else it needs to be used. I know you're not the OP here, but if what I just said sounds really simple and exactly like the tutorial they wrote, that's because again all the tutorial tells you to do is use an indexer to get a unique number and use that number for your arrays. I sound like I'm talking in circles because this tutorial really isn't about anything other than how you should use an indexer properly without hashtables.

Chaosy said:

 This method makes it like a ghetto struct in a sense.


Click to expand...

Yes, but GUI parallel arrays have always been ghetto structs.


 Last edited: Aug 6, 2018

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Pyrogasm said:

 Unless something changed drastically in the last two years, a unit indexer is explicitly a system that gives each unit a unique ID number that can be used for parallel arrays (or hashtables, or static struct instances, etc.). Sometimes the indexer saves the unit's index to its custom value, sometimes they save them in hashtables, sometimes their own structs using GetHandleId(). At the end of the day, all an indexer does is convert unique units into unique numbers for you to use. Attaching data to the unit is not a necessary function of the system, though many include the ability to do so. I find myself attaching data to timers far more often than I do to units.


Click to expand...

As you say, the system does not enforce a certain way of using it. It just hands out an id to use.

However, the common practice I seem to spot for GUI users who use this is to bind it to a unit in a manner which does not stack. This makes it easier in a lot of cases even if it is not required.

Pyrogasm said:

 Yes, but GUI parallel arrays have always been ghetto structs.


Click to expand...

Not in most cases, unless a struct instance is bound to a unit which is not always the case.

#### Pyrogasm

#### Pyrogasm

 Level 45

JoinedFeb 27, 2007Messages5,594

Resources: 1

#### Resources
Spells and Systems 1

Chaosy said:

 However, the common practice I seem to spot for GUI users who use this is to bind it to a unit in a manner which does not stack. This makes it easier in a lot of cases even if it is not required.


Click to expand...

You do point out why it's a simple path many tread, but should we not try to steer them in the right direction? Properly learning to use your indexer of choice is a far more valuable skill than hacking a shitty method to work simply by creating new units to use it on. A tutorial with "why not to do this", "what you should do instead", and "how to determine if your data will be overwritten by multiple attaches in a system like this" seems more appropriate for the subject matter.

Chaosy said:

 Not in most cases, unless a struct instance is bound to a unit which is not always the case.


Click to expand...

I'm not arguing to be right, I'm just curious how this distinction is meaningful to you? Structs members are arrays, so structs are literally giant sets of parallel arrays... therefore parallel arrays are ghetto structs. The data exists regardless of whether you 'bind' the struct (or your arrays) to any other object, just like the specific instance of data stays in your giant parallel arrays stays there if you forget which index you actually need to flush.

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

Pyrogasm said:

 I... really don't understand the point of this tutorial.


Click to expand...

Pyrogasm said:

 You do point out why it's a simple path many tread, but should we not try to steer them in the right direction? Properly learning to use your indexer of choice is a far more valuable skill than hacking a shitty method to work simply by creating new units to use it on.


Click to expand...

Ahhh, the classic "don't understand, talk shit". Is it a shitty method or do you fail to understand how small shifts in framing problems can make life insanely easy?

I'm going to boil this tutorial down for you as much as possible.

BY CREATING A UNIT EACH TIME WE WISH TO SAVE DATA, WE CAN HAVE MULTIPLE PIECES OF SIMILAR DATA SAVED BY THE SAME UNIT AT THE SAME TIME.

We either get a unique integer from the unit indexer, or by retrieving the unit handle and saving to a hashtable (effectively a unique integer)

Most GUI users crafting custom spells will use unit groups and timer loops. If we use the caster in a loop every cast, we will overwrite data. If we use a dummy for each cast, we are essentially giving every single cast a unique integer and tying it back to the caster, instead of the same integer for each cast by the same caster.

Lets say we have a spell that saps 10 strength for 30 seconds, with a 10 second cooldown. If we loop through using the casters index for each cast, we are going to overwrite data

CAST 1: +10 str, 30 second duration is saved as duration[castershandle]

CAST 2: +20 str, 30 second duration is saving over itself at duration[castershandle]

CAST 3:+30 str, 30 second duration (etc.)

30 SECONDS LATER

-10 str, the caster still has permanently taken +20 str from the target (probably not what the effect is intended to do)

If we generate a unit for each cast, the unit has a unique integer, allowing stacks to work

CAST 1: +10 str, 30 second duration saved as duration[dummy1]

CAST 2: +20 str, 30 second duration saved as duration[dummy2], duration[dummy1] is at 20 seconds

CAST 3: +30 str, 30 seconds on duration[dummy3], 20 on duration[dummy2], 10 on duration[dummy1]

10 SECONDS LATER

-10 str from duration[dummy1], caster has +20

10 SECONDS LATER

-10 str from duration[dummy2], caster has +10

10 SECONDS LASTER

-10 str from duration[dummy3], caster has 0, target has 0

Honestly, this method is incredibly easy to implement in GUI, fits into common GUI coding practices, works with hashtables and dynamic indexing, and extends the range of what can be MUI in GUI. Saying this isn't using a unit indexer properly or is a shitty hack of a method just shows a lack of understanding on your end, not a problem with the method. This allows GUI users to think about and frame problems in new ways.

Pyrogasm said:

 A tutorial with "why not to do this", "what you should do instead", and "how to determine if your data will be overwritten by multiple attaches in a system like this" seems more appropriate for the subject matter.


Click to expand...

Pyrogasm said:

 Your method here boils down to this:

Give units indices, somehow. Your method specifies "in lieu of a unit indexer" but does not explain what that is, why it's important, and how you should go about this if you DID have an indexer installed.

Use the unit's index as an array index for any data you might store.

Click to expand...

And yet, here you are stating exactly what this method is. Do you fail to see the practical application of generating unique integers for every cast, even with the practical applications I put inside the tutorial that would obviously and easily benefit from having unique integers for each cast? Or did you decide to go 95% of the way there on the concept and stop to shitpost all over my tutorial?

As far as the scope of the tutorial, please take another peak at the title. The Benefits of Saving Information to Dummies. Not "How to use a unit indexer, also how to stack abilities". Also not "Baby's first indexing event for new friends who need hand holding". The indexing isn't the important part of this tutorial. You should consider writing a tutorial yourself, since you seem to have a lot of unrelated bullshit to say on mine.

#### Pyrogasm

#### Pyrogasm

 Level 45

JoinedFeb 27, 2007Messages5,594

Resources: 1

#### Resources
Spells and Systems 1

Oho boy, sooooomeone got butthurt because I critiqued their tutorial in the tutorial submission section. What the fuck were you looking for, instant praise and adoration? No, I gave feedback.

I'm not ignorant, I'm approaching this tutorial as a noob would, rather than as someone with experience. I've been modding wc3 since it came out, and if I spent the time to read through your tutorial multiple times and I still don't understand the basic essence of why it exists (either by my admission of confusion in my post, or your accusation of misunderstanding in your post)... then do you think someone with very scant WE knowledge is going to get it? Do you think they're going to instantly recognize why they might find this solution preferable to others? You aimed this tutorial at inexperienced GUI users and then didn't adequately explain the purpose and method in simple terms that laid out the explicit benefits of doing things that way. I say you "aimed it at them" despite you saying above that it's not "baby's first unit indexer" or whatever because of the subject matter of the tutorial itself. Would a reasonably competent GUI user think to google to find this thread? Probably not. The content of your tutorial and the way you explained it are mismatched, in my opinion. If you want to help new mappers you need to use lots of examples (ffs at least one fully-written trigger example), links (usually to other tutorials), and explanations of the new/confusing/unusual terms ("unit indexer" is a good example here, that phrase is meaningless to a noob). You have precisely zero of these. I don't think arguing that this is for advanced mappers is reasonable since it's really not the solution you should be using to do things like this anyway and you also didn't really approach/explain anything at a high level either.

If you think the example you gave to me is such a perfect explanation of why you might want to use this method... why didn't you put such an example in the tutorial? I suggested you should in my previous post and you didn't quote that part so IDK if you actually read it. But okay let me tear down that example in just one sentence: when any instance of the debuff expires on a unit... how do you get the unit index (the one you'll need to flush) back again? The index you got from the dummy unit. That has locust and doesn't interact with anything on the map. That you didn't save into a variable you can't be sure wasn't overwritten (the problem you're trying to solve here), that you didn't save in a hashtable (because for some reason you're not using them (I get it the point of the tutorial is an alternative to hashtables)), and that you didn't 'attach' to another unit in some other way (because that runs into the same problems).

Go ahead, write an example of that strength sapping ability using your method that DOESN'T use a specific timed life on the dummy and the death event of the dummy unit to get back the proper index, restore the strength, and flush the data. No GUI novice is going to think of THAT method to get it back, and you didn't even mention it in your post.

You have also failed to address the fact that attaching to units is usually second-most-necessary to attaching to timers, to which this method cannot in any way be applied.

Jazztastic said:

 And yet, here you are stating exactly what this method is. Do you fail to see the practical application of generating unique integers for every cast, even with the practical applications I put inside the tutorial that would obviously and easily benefit from having unique integers for each cast? Or did you decide to go 95% of the way there on the concept and stop to shitpost all over my tutorial?


Click to expand...

I stated it so simply because it's not a method. Dude you wrote a tutorial that just tells people to use an indexer to do what an indexer is supposed to do. That's the entirety of the method you posted.Get (handle) ID, use ID in array index. It's not that I don't understand your method, I simply don't understand why this is a tutorial at all. If you had properly aimed it at noobs (as I have outlined above) then I would accept it as a tutorial with legitimate purpose, but as of now it's short, unexplained, and without a complete example. If you're trying to show how it is beneficial (as your title states), then write 1 example using this method and write 1 example using a different method, showing how this method is superior/more convenient/intuitive/simpler/whatever.

Jazztastic said:

 The indexing isn't the important part of this tutorial.


Click to expand...

...it's the the ONLY thing there is in the tutorial. Do you agree with my 2 steps you quoted? It sounds like you do, and that's basically the only thing you suggest users do/interact with in this method.

Jazztastic said:

 You should consider writing a tutorial yourself, since you seem to have a lot of unrelated bullshit to say on mine.


Click to expand...

lol, like I've never properly explained something in my life

I didn't attack you, your character, or your ideas; I attacked the tutorial you wrote and submitted for feedback... but you seem to be having a great time doing so to me. I wonder who's in the wrong here.


 Last edited: Aug 8, 2018

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Fun fact by the way, I think this method is convenient but ugly and reminds me of classic WoW.

In classic WoW they used invisible rabbits which they killed with code in order to complete certain quests.

The reason is that the quest API was built as either "kill X, collect Y, or talk to Z" if a quest had different criteria it could not be completed.

So if there was an escort quest they killed an invisible rabbit once the unit reached a certain point in order to complete the quest.

This method feels kinda similar where it is convenient but subpar.

A solution that comes to mind is dynamic indexing. As far as I know it solves the problem without this workaround.

Still, my problem is not so much the solution itself but more along the lines of if it would be "ok" for the spell section, and I am not sure as I do not believe I have seen this approach in an approved GUI spell before.

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

Pyrogasm said:

 Oho boy, sooooomeone got butthurt because I critiqued their tutorial in the tutorial submission section. What the fuck were you looking for, instant praise and adoration? No, I gave feedback.

I'm not ignorant, I'm approaching this tutorial as a noob would, rather than as someone with experience. I've been modding wc3 since it came out, and if I spent the time to read through your tutorial multiple times and I still don't understand the basic essence of why it exists (either by my admission of confusion in my post, or your accusation of misunderstanding in your post)... then do you think someone with very scant WE knowledge is going to get it? Do you think they're going to instantly recognize why they might find this solution preferable to others? You aimed this tutorial at inexperienced GUI users and then didn't adequately explain the purpose and method in simple terms that laid out the explicit benefits of doing things that way. I say you "aimed it at them" despite you saying above that it's not "baby's first unit indexer" or whatever because of the subject matter of the tutorial itself. Would a reasonably competent GUI user think to google to find this thread? Probably not. The content of your tutorial and the way you explained it are mismatched, in my opinion. If you want to help new mappers you need to use lots of examples (ffs at least one fully-written trigger example), links (usually to other tutorials), and explanations of the new/confusing/unusual terms ("unit indexer" is a good example here, that phrase is meaningless to a noob). You have precisely zero of these. I don't think arguing that this is for advanced mappers is reasonable since it's really not the solution you should be using to do things like this anyway and you also didn't really approach/explain anything at a high level either.

If you think the example you gave to me is such a perfect explanation of why you might want to use this method... why didn't you put such an example in the tutorial? I suggested you should in my previous post and you didn't quote that part so IDK if you actually read it. But okay let me tear down that example in just one sentence: when any instance of the debuff expires on a unit... how do you get the unit index (the one you'll need to flush) back again? The index you got from the dummy unit. That has locust and doesn't interact with anything on the map. That you didn't save into a variable you can't be sure wasn't overwritten (the problem you're trying to solve here), that you didn't save in a hashtable (because for some reason you're not using them (I get it the point of the tutorial is an alternative to hashtables)), and that you didn't 'attach' to another unit in some other way (because that runs into the same problems).

Go ahead, write an example of that strength sapping ability using your method that DOESN'T use a specific timed life on the dummy and the death event of the dummy unit to get back the proper index, restore the strength, and flush the data. No GUI novice is going to think of THAT method to get it back, and you didn't even mention it in your post.

You have also failed to address the fact that attaching to units is usually second-most-necessary to attaching to timers, to which this method cannot in any way be applied.

I stated it so simply because it's not a method. Dude you wrote a tutorial that just tells people to use an indexer to do what an indexer is supposed to do. That's the entirety of the method you posted.Get (handle) ID, use ID in array index. It's not that I don't understand your method, I simply don't understand why this is a tutorial at all. If you had properly aimed it at noobs (as I have outlined above) then I would accept it as a tutorial with legitimate purpose, but as of now it's short, unexplained, and without a complete example. If you're trying to show how it is beneficial (as your title states), then write 1 example using this method and write 1 example using a different method, showing how this method is superior/more convenient/intuitive/simpler/whatever.

...it's the the ONLY thing there is in the tutorial. Do you agree with my 2 steps you quoted? It sounds like you do, and that's basically the only thing you suggest users do/interact with in this method.

lol, like I've never properly explained something in my life

I didn't attack you, your character, or your ideas; I attacked the tutorial you wrote and submitted for feedback... but you seem to be having a great time doing so to me. I wonder who's in the wrong here.


Click to expand...

Lol you still don't get it.

Mods, please graveyard this tutorial if you don't find it approval worthy. I absolutely will not update it.

 (because for some reason you're not using them (I get it the point of the tutorial is an alternative to hashtables)),


Click to expand...

How many times do I have to tell you this method works with both hashtables and dynamic indexing?

Really dude reread what I have written, then read your responses to it. You keep arguing against what you think this tutorial is, instead of what the tutorial actually is.

Your signature is a laff and a haff bud, but maybe you should add a bit about the reading comprehension you so desperately need

Also if you don't want people to flame you, try the following technique

*DON'T say what you are reading is shitty and also say you don't understand it

 Oho boy, sooooomeone got butthurt because I critiqued their tutorial in the tutorial submission section. What the fuck were you looking for, instant praise and adoration? No, I gave feedback.


Click to expand...

Your feedback was "This is shitty and I don't understand"

Then your feedback was "You should do this method that doesn't do the same thing"

Finally your feedback is "OP must be mad because he cursed at me"

I get it man. You don't get it. It's that simple.

 If you think the example you gave to me is such a perfect explanation of why you might want to use this method... why didn't you put such an example in the tutorial? I suggested you should in my previous post and you didn't quote that part so IDK if you actually read it. But okay let me tear down that example in just one sentence: when any instance of the debuff expires on a unit... how do you get the unit index (the one you'll need to flush) back again? The index you got from the dummy unit. That has locust and doesn't interact with anything on the map. That you didn't save into a variable you can't be sure wasn't overwritten (the problem you're trying to solve here), that you didn't save in a hashtable (because for some reason you're not using them (I get it the point of the tutorial is an alternative to hashtables)), and that you didn't 'attach' to another unit in some other way (because that runs into the same problems).


Click to expand...

I'm choosing to quote this section so you can know that I saw it and still ignored you.


 Last edited: Aug 9, 2018

#### Pyrogasm

#### Pyrogasm

 Level 45

JoinedFeb 27, 2007Messages5,594

Resources: 1

#### Resources
Spells and Systems 1

I don't get your power trip, dude. You're fucked up, and I reported this thread.

Signature was set in 2007 and I never changed it. If you hate it you can rage internally at 16-year-old me.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

What a discussion!^^..

We should compare:

Dummy Unit MethodDynamic Indexing (link)

Key/Access to data ..Create dummy unitAllocate integer from list

Access to key..Loop through group with all dummies O(n)Loop through all used integers, O(n)

Data accessible from caster?NoNo

How I understand MUI is ensured, this method is delegating indexing (in form of hash or unit indexer) to a dummy unit, instead of an integer.

One could argue for the dummy method..

usage of a unit group to loop over dummies -> no algorithm for allocating/deallocating indices is required that keep keys in bounds. Simple add() and remove() can be used.

there is no extra onDestroy overhead, like one writes LastIndex back to CurrentIndex in dynamic indexing, when one instance ended.
One could argue against the dummy unit method..

indexing method, extra for the dummy, is required. A hashtable, or a unit indexer has to be used.

more heavy and less performant, as objects like a unit is for sure more memory costly and slower to operate with than using native integers. A unit may be also detected through other codes while it exists only for existing.

dummy unit in object editor is needed, which might mean extra effort
So others than usages, or installation I lack of seeing functional differences that might benefit or disadvantage the user, when using dummy-method instead of dynamic indexing, or vice versa.

===

It would be useful if some more things are explained at begining. Like one would wonder why there is actually a problem with what exactly? -> There might be a problem when using the 'caster' as only key to store data, so in consequence the caster can not store multiple packages of data at same time. A new unique key is needed each time, that can be used to access data.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Good when it comes to dealing with Unit Indexer. However, as argued, Dynamic Indexing can be an alternative for this matter.

Oh, @IcemanBo add the fact that unit has permanent leak while integer does not. I see the tutorial has it's purpose but the way it's currently explained is not really helpful to newer people who approach GUI MUI coding.

#### Pyrogasm

#### Pyrogasm

 Level 45

JoinedFeb 27, 2007Messages5,594

Resources: 1

#### Resources
Spells and Systems 1

IcemanBo said:

 It would be useful if some more things are explained at begining. Like one would wonder why there is actually a problem with what exactly? -> There might be a problem when using the 'caster' as only key to store data, so in consequence the caster can not store multiple packages of data at same time. A new unique key is needed each time, that can be used to access data.


Click to expand...

Daffa the Mage said:

 I see the tutorial has it's purpose but the way it's currently explained is not really helpful to newer people who approach GUI MUI coding.


Click to expand...

Trying to bring this up with the OP is pointless; they didn't listen to me and they won't listen to you.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

So I am quite split on this after coming back to it.

The main thing for this tutorial is that the method showcased is relatively easy to use compared to alternatives to achieve the same effect. (in GUI)

However there are quite a few cons, I would say.

Firstly, I think the example is lackluster and not sufficient. I understood it by just reading the title and some of the tutorial, I did not need the trigger example. I suspect that if someone with less experience/knowledge came here to learn I don't think the example is clear enough to show an uncertain user exactly how to put it into practice. It only showed the very starting point and I think a full example of a finished spell/system using this method would be ideal.

The target audience for this is very small, I think. The primary uses for this method require what I would consider advanced knowledge, and very few capable of such stick to GUI rather than text-based code which has cleaner solutions at hand.

Similarly, the method usage itself is also very niche I would say. I cannot think of a time when I have required the use of something similar to this back when I was a GUI purist. In addition, I see very few asking about such in the WEHZ.
I would be willing to look past the last two points as there is a case to be made about simply accepting niche but useful tutorials.

However, I firmly believe this needs an extended trigger example in order to be considered finished.

Author stated that he does not intend to update this tutorial, but I'll leave it in the submissions for a week or two just in case before graveyarding it unless updated.

edit: graveyarded


 Last edited: Oct 9, 2020

 You must log in or register to reply here.


### Similar threads

 Dummy Units Tutorial


orcfan32

Jun 3, 2005

Tutorial Archive

Replies7Views13K

Mar 13, 2007
Wolverabid

Locked

 How to preload dummies 'Exactly'!


MN Lahmar

Feb 22, 2019

World Editor Help Zone

Replies2Views2K

Mar 10, 2019
MN Lahmar

[GUI]
 Save and Load in GUI


Chaosy

Oct 24, 2014

Tutorial Archive

Replies19Views6K

Nov 28, 2016
IcemanBo

 How to create Nova Spells [Reviewed: PurplePoot]


DenZel94

May 23, 2008

Tutorial Archive

Replies1Views2K

May 23, 2008
PurplePoot

 Basics of The World Editor


TheBlooddancer

Sep 25, 2008

Tutorial Archive

Replies2Views2K

Sep 27, 2008
TheBlooddancer

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