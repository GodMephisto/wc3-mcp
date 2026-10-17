---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/custom-team-colors-tutorial.276499/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Custom Team Colors Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/custom-team-colors-tutorial.276499/>
#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

This is a tutorial about creating and using custom team colors based on the replaceable textures method using War Club ability. You can find more information about this method here. This method basically involves using modified replaceable material id's to change texture of the model via in-game ability called War Club.

First of all to be able to change team color we need to edit replaceable id of your model that will be using custom team color.

Start Magos Model Editor, open up your model and do as showed:

Now import the model into the map and save it. The second part involves triggering, but you don't need to worry about that. I've taken care of that by creating a system which you can download and edit. It's GUI and very simple to understand.

Currently it's not possible to change more than one material with War Club ability, thus you can't change Team Glows. Alternative option would be to create a custom team glow texture and apply it to your custom team glow model and then attach it to a unit.

This method is very useful for campaigns that don't have file size limit.

Screenshot 1

Team colors are 1x1 pixel .tga images to keep file size lowest. Converting them to .blp increases the size, so I kept it like that.

Furthermore, any questions will be answered in the comments.

#### Attachments

Custom Team Color System.w3x

 488.8 KB

 · Views: 437




 Last edited: Sep 30, 2016

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

The White, Aqua and Midnight blue team color remaind the same as they look, no difference except the black and green. So if I understand correct this isn't a total change of existing team color but making them more shading or alpha...etc.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Warseeker said:

 The White, Aqua and Midnight blue team color remaind the same as they look, no difference except the black and green.


Click to expand...

What do you mean? They don't look the same, White is similar to gray, Aqua is similar to teal and Midnight Blue is similar to purple but they have completely different tones.

You can add your own custom team colors, these were just my examples.

#### RED BARON

#### RED BARON

 Level 35

JoinedOct 9, 2006Messages6,392

Resources: 56

#### Resources
Models 43Icons 7Packs 1Skins 3Maps 2

Warseeker said:

 The White, Aqua and Midnight blue team color remaind the same as they look, no difference except the black and green. So if I understand correct this isn't a total change of existing team color but making them more shading or alpha...etc.


Click to expand...

That's just the examples. When you change the blp it makes it much more obvious.

I would however given the above comment suggest to Void to make some radically different colors as examples so it would be very obvious that they changed.

Also you should mention that the editor requires something? At least mine crashes due to an unknown custom trigger.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

RED BARON said:

 That's just the examples. When you change the blp it makes it much more obvious.

I would however given the above comment suggest to Void to make some radically different colors as examples so it would be very obvious that they changed.


Click to expand...

It's not .blp, it's .tga.

I will eventually update this tutorial, I kinda rushed it a bit.

#### MasterHaosis

#### MasterHaosis

Patron

 Level 46

JoinedSep 18, 2007Messages2,331

Resources: 37

#### Resources
Models 19Icons 15Maps 3

A Void, you got rep from me for trying. Although not finished, this can mimic new colors to some extend with some drawbacks of course, but still, we are one more step closer to it!

Cool thing is that it can really give some new effect in game, even chat messages are in that new color. Also you can change your color in second. I can imagine few human players doing that all time haha.

But serious disadvantage is that you need to extract every single unit you want to change and import back which can make map very big especially if there are custom races. This works for maps with lesser units.

A Void, man I think you are closer to success. Keep researching maybe you will find way how to directly avoid this stuff with import/export of every unit. Maybe somehow it could directly affect main texture without editing it.

#### RED BARON

#### RED BARON

 Level 35

JoinedOct 9, 2006Messages6,392

Resources: 56

#### Resources
Models 43Icons 7Packs 1Skins 3Maps 2

A Void said:

 It's not .blp, it's .tga.

I will eventually update this tutorial, I kinda rushed it a bit.


Click to expand...

Yea, I noticed. I was actually going to make some updates pictures with this.. but it crashes. I guess you used a jass version of the editor?

EDIT: Okay, so one cannot edit the triggers in any way, nor save the map. I have now tried both a fully updated regular editor and the newest newgen, nothing works. So what should the "unknown" script be? Just clicking the unknown crashes the editor.

#### MasterHaosis

#### MasterHaosis

Patron

 Level 46

JoinedSep 18, 2007Messages2,331

Resources: 37

#### Resources
Models 19Icons 15Maps 3

Oh, and if you are Undead or got Black Ranger and posses/charm enemy unit they will still be original color, in this case red. I totally forgot about that

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

MasterHaosis said:

 Maybe somehow it could directly affect main texture without editing it.


Click to expand...


I tried everything already, changing team color path in War3Skins.txt, replacing one of the team colors with another, using team color id for destructibles and units. Nothing worked. This is the method I came up which actually is a solution.


 MasterHaosis said:



 Cool thing is that it can really give some new effect in game, even chat messages are in that new color. Also you can change your color in second. I can imagine few human players doing that all time haha.


Click to expand...


You can change computer players to use custom color, you can disable changing team color for players. This system is free to be edited, you can make your own changes.


 RED BARON said:



 EDIT: Okay, so one cannot edit the triggers in any way, nor save the map. I have now tried both a fully updated regular editor and the newest newgen, nothing works. So what should the "unknown" script be? Just clicking the unknown crashes the editor.


Click to expand...


I used Jass New Gen World Editor. I'll convert the map for normal world editor, thanks for notifying.


 MasterHaosis said:



 Oh, and if you are Undead or got Black Ranger and posses/charm enemy unit they will still be original color, in this case red. I totally forgot about that


Click to expand...


I will update the map with a fix to that problem.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

This seems like a nice alternative to the other tutorial, granted the author has enough file space within their map. You should write down within your tutorial that you have to edit the material for every model that you want to support custom team colors on.

Also, I haven't looked at the code within the map, but it appears to crash when I switch to a custom color and then back to a standard color (e.g. -Fuchsia to -Gray).

Other than that, it looks ready to be approved!

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

PurgeandFire said:

 This seems like a nice alternative to the other tutorial, granted the author has enough file space within their map. You should write down within your tutorial that you have to edit the material for every model that you want to support custom team colors on.

Also, I haven't looked at the code within the map, but it appears to crash when I switch to a custom color and then back to a standard color (e.g. -Fuchsia to -Gray).

Other than that, it looks ready to be approved!


Click to expand...

It probably crashes because I used a method to hide War Club aka change texture ability, it doesn't work with mac. But hiding it is not necessary as it is a temporary ability. I'll eventually update that.

#### RED BARON

#### RED BARON

 Level 35

JoinedOct 9, 2006Messages6,392

Resources: 56

#### Resources
Models 43Icons 7Packs 1Skins 3Maps 2

A Void said:

 I used Jass New Gen World Editor. I'll convert the map for normal world editor, thanks for notifying.


Click to expand...


Sounds good.  I'll check it again when time.

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

So, is it possible to create till 256 color from 12 team color with that?

I thought that depend on the contrast color as this model, the team color of it is quite different tho...

#### MasterHaosis

#### MasterHaosis

Patron

 Level 46

JoinedSep 18, 2007Messages2,331

Resources: 37

#### Resources
Models 19Icons 15Maps 3

A Void said:

 I tried everything already, changing team color path in War3Skins.txt, replacing one of the team colors with another, using team color id for destructibles and units. Nothing worked. This is the method I came up which actually is a solution.


Click to expand...

Yeah, but there must be something. Shar Dundred and I are talking much regarding this. He believes that there is some order somewhere to tell game to look at mpq first instead of map file regarding colors.

A Void said:

 I will update the map with a fix to that problem.

Click to expand...

How? I think that it also applies to those possessed units as well, they must be exported/edited/imported too

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Nifty. Seems quite size heavy which is quite the downside though.

Still, likely something I will use in the future.

#### JesusHipster

#### JesusHipster

 Level 35

JoinedSep 19, 2011Messages2,119

Resources: 70

#### Resources
Models 62Icons 7Skins 1

Or you could just replace the needed TC texture (ex. ReplaceableTextures\TeamColor\TeamColor00.blp, 01, 02... etc) with a colour of your choice. Or is there a sort of limitation regarding this method?

#### RED BARON

#### RED BARON

 Level 35

JoinedOct 9, 2006Messages6,392

Resources: 56

#### Resources
Models 43Icons 7Packs 1Skins 3Maps 2

JesusHipster said:

 Or you could just replace the needed TC texture (ex. ReplaceableTextures\TeamColor\TeamColor00.blp, 01, 02... etc) with a colour of your choice. Or is there a sort of limitation regarding this method?


Click to expand...


According to what I understand, it takes the color directly from the mpq, thus importing a changed texture to replace team colors has no effect.

#### RED BARON

#### RED BARON

 Level 35

JoinedOct 9, 2006Messages6,392

Resources: 56

#### Resources
Models 43Icons 7Packs 1Skins 3Maps 2

Doesn't look like the map has been updated yet, still that "unknown" custom function that crashes the editor. Looking forward to testing it when it works.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

JesusHipster said:

 Or you could just replace the needed TC texture (ex. ReplaceableTextures\TeamColor\TeamColor00.blp, 01, 02... etc) with a colour of your choice. Or is there a sort of limitation regarding this method?


Click to expand...


That method is discussed in this tutorial. However, team colors are loaded when Warcraft III is started up, not upon map-loading, so you can't replace it individually for a map. The players have to use a modified MPQ.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Ok, I finished this system. The map has been completely redone. You can download and test it out now.

If you have any questions or suggestions, please comment.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Approved.

#### RED BARON

#### RED BARON

 Level 35

JoinedOct 9, 2006Messages6,392

Resources: 56

#### Resources
Models 43Icons 7Packs 1Skins 3Maps 2

A Void said:

 Ok, I finished this system. The map has been completely redone. You can download and test it out now.


Click to expand...


Nice it works perfect now.


Hence I can present my screenshot that should showcase how much you can actually do with this.. and even more if willing to offer some space for it.

#### Attachments

double color.jpg

 935.5 KB

 · Views: 2,352



#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

RED BARON said:

 Nice it works perfect now.

Hence I can present my screenshot that should showcase how much you can actually do with this.. and even more if willing to offer some space for it.


Click to expand...

Cool, you combined different colors in one team color texture. This system is especially useful for campaigns as they have no battle.net file size limit.

#### RED BARON

#### RED BARON

 Level 35

JoinedOct 9, 2006Messages6,392

Resources: 56

#### Resources
Models 43Icons 7Packs 1Skins 3Maps 2

A Void said:

 Cool, you combined different colors in one team color texture. This system is especially useful for campaigns as they have no limit.

Click to expand...


Yep, my thought exactly. This one is just a 2x2 pixel.. but with more one can do a lot.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

System has been updated, made some trigger optimizations.

#### fran bel

#### fran bel

 Level 17

JoinedMay 13, 2020Messages227

Resources: 22

#### Resources
Reforged HD Models 17Reforged HD Skins 1Ported Icons 3Reforged HD Icons 1

Ask this tutorial so that more than 12 colors can be selected or to replace the ones that already are? for example the mod nirvana adds more races, can't you do something similar with the team color?

 You must log in or register to reply here.


### Similar threads

 Custom Team Colors


Shar Dundred

Mar 5, 2016

Misc/General Mapping Tutorials

Replies19Views14K

Jun 25, 2025
Marok

 How To Change Textures on a Tree or other Doodad (Without importing several models)


InfernalTater

Sep 3, 2012

Misc/General Mapping Tutorials

Replies6Views19K

Mar 11, 2024
fostaa

Locked

 Adding in Team Color to Custom Model


Fabioz

Apr 12, 2008

Texturing & Surfacing

Replies1Views1K

Apr 12, 2008
madcat2

Locked

 Team-colored Glow


xyzier_24

Oct 25, 2014

Requests

Replies6Views2K

Oct 26, 2014
xyzier_24

 Crediting Tutorial


General Frank

May 18, 2008

Misc/General Mapping Tutorials

Replies13Views16K

Apr 29, 2026
berkoax

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