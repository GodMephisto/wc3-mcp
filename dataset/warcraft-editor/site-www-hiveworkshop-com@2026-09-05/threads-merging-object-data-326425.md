---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/merging-object-data.326425/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Merging Object Data
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/merging-object-data.326425/>
#### Whitewolf8

#### Whitewolf8

 Level 11

JoinedAug 31, 2011Messages293

Resources: 7

#### Resources
Models 1Maps 4Tutorials 2

Introduction!

Greetings!

This is just something to note a potential way to merge object data, since I can't use Grimoire stuff, being on v1.31 I started to experiment, so this is the result of that.

To preface, this should also work properly in all tabs of the Object Editor, and probably even the Trigger Editor, Hex Editors are pretty versatile being able to do as they do.

Apologies if things don't make sense, I just naturally seem to trail off on explanations a lot.

This is not the only way to merge data, other options might suit you better. If this seems to complicated, or run into some sort of issue you can always try something else.​

It is advised that you backup your map before attempting this as it might not be reversible if you make a mistake.

What we need~

Warcraft 3, of course, as well as World Editor

A Hex Editor, I recommend HxD.

Let's begin!

So here we have our two sets of units we're going to combine. Sure could just copy them over but that's no fun.... And becomes much harder if both have over 100 units.

Of course we'll need to export the unit datas for both of these unit sets (I'd recommend against exporting all of the data at once, just bits at once, Units, Abilities, Upgrades, all that.

Beautiful and simple! Of course the data will fill out the more you change the units around, but this'll do for now.

This is what you need to copy-- everything from the 0C offset onwards... If you only have custom units-- we'll get to this later.

Copy it on to your main unit data, change the 08 offset to the addition of these two, 5+5 in Hexidecimal is A, so A for me (Use a calculator, it'll get harder to keep track of the more you have.)

And there we have it! We've combined the two! Buuuut that only covers a very specific case like this where you haven't got overlapping IDs or something, so let's do something about that, shall we?

Here, we'll be changing IDs around, this method isn't that great and can't always be done, but it's a method that exists for where you may use it. You'll see that I've also removed what isn't needed to copy from the new 'donor' data, just to make it easier.

Here, we hit Replace (Ctrl+R), set 'Search for:' to the first two bits of ID for your units or abilities or... what have you, you may need to do this multiple times for various things, then set 'Replace with:' with a new bit, so here I have 'Search for: u0' and 'Replace with: u1', set the 'Case sensitive' checkbox (Units use lowercase for their first letter, while heroes use uppercase, don't let them mix up or things will break) and Replace all! Do that all the times you need to aaaaand... Done!

Copy it all over to your main data and yay! Now all of the units have their new IDs. Of course you can also easily change what abilities that everything has and such this way by using ability IDs instead-- assuming you're also copying their abilities over.

So... Now what happens if you have changed the standard units as well? It becomes a bit more complicated but not too much.

My my! How crowded.

One easy way to do this is to export data, then File>Reset Standard Units to Defaults and export that data separately (Keep backups! Always keep backups!), this separate one is more for reference than anything.

For the reference data we'll need to find some kind of point to anchor off of, as it were, for this, we're using the Custom Footman ID of h000, trace back a little aaand...

There! that 05 telling us how many units we have, change that to the new number of units (still A since I'm only ending up with 10 in all), copy the donor data and there we go!

Just pointing out the model path as another reference point.

So... The IDs get kind of funky after getting to 256 units.

The 08 offset still has a number as it should (here 80) but there's another after it (03). Flip those around so you get 380 instead, and converting that from Hex to Decimal and you get 896. That's quite a lot. After getting to 256, new hundreds and thousands will instead be written AFTER the initial tens do, so that's mostly just a thing to keep in mind.

Hopefully that covers it all. Good luck!​


 Last edited by a moderator: Oct 18, 2020

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I don't think this approach should be encouraged.

Fiddling with hex values have a high chance of corrupting your map if you make a mistake.

You can of course use a backup so it's not a dealbreaker, but at the very least using a backup should be encouraged in the tutorial itself.

Additionally, there are better ways to do this.

Wurst, vjass and Moonscript(I think) have object editing support, meaning just about all custom units (and possibly data) should be generated with code if you use any of those languages.

If you don't, I would suggest using JNGP's GRIMEX. This only works for older versions of wc3 though.

Release: GMSI - Wc3C.net also exists if you happen to know java/C

I also found a program uploaded in a WEHZ thread.

I have not tried it so I don't exactly vouch for it.

But my point is, I think there are way better solutions in terms of ease of use and safety.

And no doubt, I have not found everything on the subject.

#### Attachments

W3M and W3X Files Format.zip

 33.4 KB

 · Views: 131



#### Whitewolf8

#### Whitewolf8

 Level 11

JoinedAug 31, 2011Messages293

Resources: 7

#### Resources
Models 1Maps 4Tutorials 2

Oh, absoutely there are definitely ways better than this-- mostly in this case I'm just more versed in hex than actual programming languages so it was easiest for me so I thought it would be apt to at least put the knowledge out there in case there are other rare example like me. All other things I tried on this I'd never really got working but that could just be my ineptitude. (I'm on v1.31, so anything needing earlier versions are unfortunately out of the question).

Didn't I mention backups in the thing itself? Huh. I'll put something up in it now, then!

Thanks for taking a look!

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Yeah, you did mention it.

I just think it should be more obvious as it is easy to miss in my opinion.

Easy solution is to underline it or highlight it with color.

Although I am skeptical about the approach, I think that might be personal bias so I am willing to compromise and let the userbase decide what works for them and their situation.

In order to make an informed decision, I think alternatives should be mentioned though.

A few other structure/layout changes:

Large images should be put in hidden tags.

Colored titles are a +

Indent after each title makes it easier to read

More linebreaks.

Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus porttitor nibh vitae velit porta accumsan. Donec rutrum, velit eu rutrum lacinia, lorem diam pellentesque enim, vel consectetur felis velit vel odio. Ut pharetra, tortor tempus tempor molestie, erat sapien dictum neque, sit amet imperdiet elit mi in tellus. Sed vitae velit consectetur, eleifend odio non, porttitor elit. Proin venenatis, turpis non tincidunt rhoncus, lorem augue tincidunt elit, et faucibus tortor nulla vitae metus.

>>>

Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus porttitor nibh vitae velit porta accumsan. Donec rutrum, velit eu rutrum lacinia, lorem diam pellentesque enim, vel consectetur felis velit vel odio.

Ut pharetra, tortor tempus tempor molestie, erat sapien dictum neque, sit amet imperdiet elit mi in tellus.

Sed vitae velit consectetur, eleifend odio non, porttitor elit. Proin venenatis, turpis non tincidunt rhoncus, lorem augue tincidunt elit, et faucibus tortor nulla vitae metus.

edit: to clarify, I don't want you to add 4 tutorials. Just mention that the other options exist with a link or two perhaps so users can look into it.


 Last edited: Oct 5, 2020

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Did some changes.

Colors added

Edited introduction slightly

Edited the warning at the top

Added hidden tags to images

There are other changes that would be nice (some mentioned in the above post), and I would also say that some of the explanations are a bit rambly.

However, I feel like I do not have the right to edit anything that changes the spirit of the tutorial.

So approved for now but I think I can be improved still.

#### Whitewolf8

#### Whitewolf8

 Level 11

JoinedAug 31, 2011Messages293

Resources: 7

#### Resources
Models 1Maps 4Tutorials 2

Oh dear, I forgot about this immediately upon seeing it.

Still, thank you! I'm not especially good at explaining things, not too sure how it'd be improved upon.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Well it is not something that I can teach.

I can give you a pointer however.

 Copy it all over to your main data and yay! Now all of the units have their new IDs. Of course you can also easily change what abilities that everything has and such this way by using ability IDs instead-- assuming you're also copying their abilities over.


Click to expand...

So... Now what happens if you have changed the standard units as well? It becomes a bit more complicated but not too much.

Click to expand...

Basically, you use a lot of unnecessary words that do not bring any new knowledge or clarification.

If you can trim those down it will seem a lot less rambly.

These are just examples by the way, its not all of them.

 You must log in or register to reply here.


### Similar threads

 The Basics of Object Data


Ralle

Jan 11, 2006

Misc/General Mapping Tutorials

Replies0Views25K

Jan 11, 2006
Ralle

 Importing/Exporting Object Data from WE


RedBlade

May 24, 2007

Misc/General Mapping Tutorials

Replies17Views21K

Aug 7, 2021
ThePerfectIdiot

 Raw Data Tutorial


Oninuva

Apr 9, 2007

Misc/General Mapping Tutorials

Replies2Views15K

Apr 25, 2007
Wolverabid

Locked

 Merge object data


loktar

Jan 4, 2014

World Editor Help Zone

Replies2Views2K

Jan 4, 2014
loktar

 Transitions in Campaigns Guide


Starquizer

Jul 20, 2012

Misc/General Mapping Tutorials

Replies2Views14K

Jul 30, 2012
Magtheridon96

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