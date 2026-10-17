---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/enhancing-the-skies.201315/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Enhancing the Skies
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/enhancing-the-skies.201315/>
#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

# [Terraining]: Enhancing the skies

A simple interactive guide for average terrainers hoping to reach the next level


 - By fladdermasken

 List of Content


 Chapter 1: Introduction & Required Tools
1.1 Preamble

1.2 The Ultimate Terraining Map 3.0
Chapter 2: Main

2.1 Terrain Fog

2.2 Clouds

2.3 Stormsphere
Chapter 3: Aftermath
3.1 Epilogue

# Chapter 1: Introduction & Required Tools

### 1.1 Preamble


Firstly, this doesn't by a long shot cover the whole aspect, it just briefly introduces you to the fundamentals. I don't really consider learning this by an interactive tutorial to be ideal, seeing as it's the kind of experimential procedure where you learn by doing, to cut an otherwise (more) lenghtly paragraph short. But since this was requested, I decided to give it a shot.


Secondly, to make matters easier for yours truly, I'm going to assume that you know your terraining fundamentals already. If you don't, you have some catching up to do before you have a go at this. This would for instance include:
Terrain palette familiarity

Basic camera knowledge

Object editor familiarity

### 1.2 The Ultimate Terraining Map 3.0


We will be using The Ultimate Terraining Map 3.0 (commonly refered to as UTM) for this procedure, simply because it conveniently enough features all the custom models we will need. If you want to import the models manually to your own map, I couldn't care less.


Anyhow, the ultimate terraining map is, as described by the author:

 The Ultimate Terraining Map (UTM) was developed long ago to make terraining easier for terrainers everywhere. Some of the features of the map include...

Over 949 imported files and yet still under 4MB!

Pathing set to none for most doodads.

Doodads reorganzied so that none are missed or forgotten.

Click to expand...

# Chapter 2: Main

### 2.1 Terrain Fog


Terrain fog is a dandy little tool. It will [highlight]dramatically[/code] affect the final outcome of your sky, so pay attention. To enable terrain fog, tick the box under Scenario » Map Options. Style and density are somewhat irrelevant, so this chapter will focus on the other three options, while still briefly mentioning the others below.

Style: This determines the propagation rate. Linear is the smoothest, and exponential, as the term suggests, is more aggressive. Linear is recommended.

Z Start: This will determine where your fog starts. I recommend values ranging from 0-1000, higher values tend to output less pleasing results.

Z End: In contrast to Z Start, this value determines where your fog ends. I tend to use numbers like 8000-25000 all depending on how big the map is.

Density: Adjusts the fog density, as the name suggests. Literally fills the exact same function as the Z difference does, so I quite frankly find it irrelevant.

Color: The color of your fog. Pretty much self explanatory.

Click to expand...


Follow these steps:

Use the raise tool to create some mountains, hills and mounds. Since this is an experiment, there's no real emphasis on the scenery as long as your map isn't completely flat.


I will be working with the example displayed below.

Go to Scenario » Map Preferences and set sky display to fogged sky and time of day to fixed - noon. Experiment with the fog settings we discussed. I'd recommend a pale and gentle gradient, it shouldn't push too much for attention. The transition should be smooth and natural. Avoid abrupt shifts in general, colors should transition naturally. A clear blue sky should never clash with a blood red fog gradient and vice versa.

Bad: The red fog in this example clashes with the blue sky. It just doesn't look natural. Z Start is set to 6000, Z End is set to 8000. As you can see, the scenery looks plain awful.


Make it a general rule of thumb that the background should be fairly anonymous.

Click to expand...

Good: This, in contrast to prior examples, looks seemingly well. The transition is alot smoother, the color gradient isn't as aggressive, and the sky merges just fine with the background.


Z Start is set to 0, Z End is set to 12000. The color gradient is pale light blue (205/215/225).

Click to expand...

### 2.2 Clouds


We should now be ready to bring out the big guns, the models that will make your sky look authentic and not like the inside of a horse's mouth: Clouds. As previously mentioned, the models we need are conveniently organized and available in the ultimate terraining map, you will find them under the cinematic category in the doodad palette. Before starting, you should scale the models in accordance to the size of the scenery. Below you will find an approximate example for how I adjust my scaling value in correspondence to the z end value. Follow the steps below:

Z End Value

Scaling Value

 5000 - 8000

 5.0 - 8.0

 8000-12000

 8.0 - 12.0

 12000 +

 12.0

Open the object editor, find the cloud models [Doodads » Custom Doodads » Cinematic » Sky: Clouds (1-3)], and adjust the following:

Art - Default Scale Value: The default scaling value the model will have when placed (if the random scale function is disabled)

Editor - Maximum Value: The maximum scaling value of the model. Pretty self explanatory.

Editor - Minimum Value: The polar opposite of the maximum scaling value.

Click to expand...

Ascertain that you set Editor - Ignore Model Clicks to true. This implies that the clouds won't be selectable in the editor.


This is convenient for several reasons:

Clouds are excessively large and it would thus be impossible to select anything else.

Overly stretched models have a tendency of crashing the editor when selected.

Adjust the color values of the clouds (Art - Tinting Color). The lesser the values, the more saturated the clouds.


In this brief example I've used the following values:

 Sky: Clouds (1)
Art - Default Scale Value: 4.0

Art - Tinting Color (RGB): 75/25/5

Editor - Maximum Value: 4.0

Editor - Minimum Value: 4.0
Sky: Clouds (2)
Art - Default Scale Value: 8.0

Art - Tinting Color (RGB): 75/25/5

Editor - Maximum Value: 8.0

Editor - Minimum Value: 8.0
Sky: Clouds (3)
Art - Default Scale Value: 8.0

Art - Tinting Color (RGB): 75/25/5

Editor - Maximum Value: 8.0

Editor - Minimum Value: 8.0

Click to expand...

That's a basic foundation. This procedure takes practice to master, so just keep experimenting and you'll get the hang of it before you can say retinal detachment. There are several models under the cinematic category which can be used to emphasise and enhance the sky.

### 2.3 Stormsphere


Before I leave you, I have to mention one other aspect. The stormsphere-curtain. The only difference between this and the previously mentioned aspect is the base sky. Where as in the previous example we started with a preset sky and enhanced it, we will in this scenario work from scratch with a completely monochromatic curtain.

Go to Object Editor » Doodads » Custom Doodads » Cinematic and locate Sky: Stormsphere. The stormsphere is an animated dome (or sphere) which portrays a lightning storm in full effect. It's a rather nice effect in moderation, but by default it will not interact well with other light sources and is thus seldom used. We can, however, obviously circumvent that particular drawback.

Set the listed settings as such:

 Sky: Stormsphere
Art - Default Scale Value: 10.0

Art - Tinting Color (RGB): (0-15)/(0-15)/(0-15)

Editor - Maximum Value: 10.0

Editor - Minimum Value: 10.0

Click to expand...

Completely blank color values (0/0/0) will output a completely monochromatic wall, while low values will add a glimmer of sorts to the sky base.


Individual preferences, no particular weight on either of the options. Just go with the flow.

Place the stormsphere. You shouldn't be able to see it at first because of its immense scale, but just press and hold ctrl and scroll towards yourself and it'll show eventually. You'll notice that instead of a preset sky, the sky will be painted in the same color gradient as your terrain fog. This is quite convenient since this dramatically decreases any risk of fog and sky clashing.

Add clouds as you did earlier. In this example I'm using these settings:

 Sky: Clouds (1)
Art - Default Scale Value: 4.0

Art - Tinting Color (RGB): 35/15/5

Editor - Maximum Value: 4.0

Editor - Minimum Value: 4.0
Sky: Clouds (2)
Art - Default Scale Value: 8.0

Art - Tinting Color (RGB): 35/15/5

Editor - Maximum Value: 8.0

Editor - Minimum Value: 8.0
Sky: Clouds (3)
Art - Default Scale Value: 8.0

Art - Tinting Color (RGB): 55/20/5

Editor - Maximum Value: 8.0

Editor - Minimum Value: 8.0
Sky: Stormsphere
Art - Default Scale Value: 10.0

Art - Tinting Color (RGB): 10/5/2

Editor - Maximum Value: 10.0

Editor - Minimum Value: 10.0

Click to expand...

# Chapter 3: Aftermath

### 3.1 Epilogue


This concludes my guide, the rest is just creativity, perspective and individual preferences. Combine techniques, compliment with other cinematic effects, try out odd color gradients and in general think outside the box (or pyramid, or sphere, or bag, or whatever the hell you want). Sure hope you don't consider this a complete waste of time. As mentioned in the preamble, this aspect is impossible to cover in an interactive guide, but at least this should give you a brief idea on how to think and which components to work with.


To wrap this whole bag of sweets up, here are a select few terraining pieces which demonstrate the technique we've experienced and experimented with today ~[/hippie teacher].

 Showcase


 Credits


~Void~ - Tower of Dawn

raid1000 - Waterfall Castle

~Void~ - Arthas

thedeathhunterxx - Mount Aragal

CoBrA b - Post Cataclysm


 Last edited by a moderator: Aug 24, 2011

#### Statharas

#### Statharas

 Level 23

JoinedJul 9, 2008Messages2,552

Resources: 20

#### Resources
Icons 1Maps 1StarCraft II Resources 5Tutorials 13

... That is DEFINITELY NOT IN WC3. Please tell me that i am right...

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

Statharas said:

 ... That is DEFINITELY NOT IN WC3. Please tell me that i am right...


Click to expand...

No. You're wrong.

#### Deleted member 177737

D

#### Deleted member 177737

Seems kind of useless to me, but somebody might have a use for it =/

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

Seems like a somewhat useful tutorial.

By saying "I could care less" you propably mean "I couln't care less".

#### Amargaard

#### Amargaard

 Level 19

JoinedJun 29, 2009Messages6,759

Resources: 16

#### Resources
Icons 16

Very nice tutorial Fladdermasken, I could use this myself. +rep

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

Thanks for the responses.

Yayoi said:

 Seems kind of useless to me, but somebody might have a use for it =/


Click to expand...

It's a terraining tutorial. Should be utterly pointless for everyone not interested in terraining

Maker said:

 By saying "I could care less" you propably mean "I couln't care less".


Click to expand...

Yes. Yes I do.


Must've been a typo...

#### Celaquil

#### Celaquil

Patron

 Level 37

JoinedNov 24, 2007Messages4,427

Resources: 4

#### Resources
Maps 4

Yayoi said:

 Seems kind of useless to me, but somebody might have a use for it =/


Click to expand...

That, to me, seems like a useless comment.

If you are so bold as to claim a Tutorial is useless, then at least have the respect and courtesy of actually explaining HOW it is useless.

Nevertheless, I find this tutorial very proffessional and useful, especially considering how many terrainers that doesn't get this thing with skies.

#### Skycraft

#### Skycraft

 Level 17

JoinedNov 11, 2010Messages1,974

Nice tutorial, I finally might be able to make a 'decent' custom sky.

Good job!

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

Thanks for the support.

#### Bugz

#### Bugz

 Level 16

JoinedApr 18, 2011Messages1,844

Resources: 7

#### Resources
Maps 4StarCraft II Resources 1Tutorials 2

Usefull, i always needed to find out that :v, thanks for making it

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

Glad it turned out well.

And if I may issue a request to purgeandfire whenever he finds the time to review this, would you please change the title to: How To Design A Custom Sky?

Premature thanks and conveniently-sized ostrich egg.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Look good to me, vote for approval

#### Pharaoh_

#### Pharaoh_

 Level 40

JoinedNov 6, 2008Messages8,316

Resources: 13

#### Resources
Icons 4Spells and Systems 7User Interface 1Tutorials 1

Nice tutorial, but I would expect more screenshot-presentation of possible recoloring and maybe some placement tips, to avoid really intense effects (when placing the models on the top of each other).

I would also keep this title, because "designing a custom sky" might refer to photoshop work.

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

Thanks -Kobas- and Pharao_ for criticism and support.

Pharaoh_ said:

 Nice tutorial, but I would expect more screenshot-presentation of possible recoloring and maybe some placement tips, to avoid really intense effects (when placing the models on the top of each other).


Click to expand...

Presentation of the recoloring process as in more thoroughly demonstrating different effects to the tinting tool?

Pharaoh_ said:

 I would also keep this title, because "designing a custom sky" might refer to photoshop work.


Click to expand...

I see your point. I suppose we can negotiate a compromise.


"Terraining A Custom Sky" - how does that sound? :]

#### Skycraft

#### Skycraft

 Level 17

JoinedNov 11, 2010Messages1,974

Sounds better than the one right now.

I approve.

#### Pharaoh_

#### Pharaoh_

 Level 40

JoinedNov 6, 2008Messages8,316

Resources: 13

#### Resources
Icons 4Spells and Systems 7User Interface 1Tutorials 1

fladdermasken said:

 Thanks -Kobas- and Pharao_ for criticism and support.

Presentation of the recoloring process as in more thoroughly demonstrating different effects to the tinting tool?


Click to expand...

Yes. :]

fladdermasken said:

 I see your point. I suppose we can negotiate a compromise.

"Terraining A Custom Sky" - how does that sound? :]


Click to expand...

See, no matter how generic we turned the whole process of "terraining" into (a process that involves sky modification too), terrain has to do with the ground, so "terraining skies" is like "burning water": can't be done.

How about: "[Terraining]: Enhancing the skies"? This way, the topic falls into the category of terraining, without making things complex :]

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

Pharaoh_ said:

 How about: "[Terraining]: Enhancing the skies"? This way, the topic falls into the category of terraining, without making things complex :]


Click to expand...

I like it. Changed.

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

This tutorial should be approved...

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Vote for approval.


This is quite useful :3

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great tutorial. ~Approved. I'll change the title and move it to the appropriate forum.

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

Thanks a ton purge

#### Orcnet

#### Orcnet

 Level 30

JoinedJul 31, 2010Messages5,248

Resources: 5

#### Resources
Maps 4Tutorials 1

was been going hard with those fog shits I've been getting pissed with, looks like I found a tut for it, approve

#### PrinceYaser

#### PrinceYaser

 Level 57

JoinedMay 7, 2016Messages2,237

Resources: 328

#### Resources
Models 2Icons 295Skins 15Maps 3Templates 7Reforged HD Icons 6

Wow! Very great tutorial! I can now create beautiful skies.

Thanks Fladdermasken!

#### Johnsen

#### Johnsen

 Level 3

JoinedMay 21, 2021Messages14

Resources: 1

#### Resources
Maps 1

Super cool, thank you for sharing these techniques!

 You must log in or register to reply here.


### Similar threads

 All about fog


Volvox

Aug 12, 2008

Misc/General Mapping Tutorials

Replies5Views24K

Mar 5, 2020
urachi

Locked

 Sky and fog things


shadowvzs

Jan 6, 2012

World Editor Help Zone

Replies1Views2K

Jan 7, 2012
Hashjie

 Enchance Your Terrain With Eye Candy


HappyTauren

Jan 26, 2007

Tutorial Archive

Replies6Views3K

Jun 7, 2007
Wolverabid

 Natural Environments


~Void~

Aug 30, 2008

Misc/General Mapping Tutorials
2

Replies78Views54K

May 22, 2021
Johnsen

Locked

 Request the guidance of the sky


son

Jul 9, 2012

Terrain & Level Design

Replies18Views7K

Jul 23, 2012
WindowsVistaSucks

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