---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/add-multiple-skins-for-a-single-model.262796/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Add Multiple Skins for a Single Model
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/add-multiple-skins-for-a-single-model.262796/>
#### Sylvie

#### Sylvie

 Level 11

JoinedDec 14, 2014Messages550

Resources: 5

#### Resources
Icons 1Skins 1Maps 1Templates 1Tutorials 1

How to add multiple skins for a single model.

For this tutorial, you'll need of a single tool: War3 Model Editor, by Magos.

For this tutorial, I'll be using the Scarlet Crusader, by CloudWolf.

First of all, open the WME, and open its MPQ Browser [Windows > MPQ Browser]. Select War3.mpq (for Reign of Chaos units) or the War3x.mpq (for The Frozen Throne units) [MPQ Browser > File > War3.mpq or War3x.mpq].

The next step is to find the model that you wish to have the skin replaced. For me, the Footman is that unit.

When the model is opened, you should open the Texture Manager [Windows > Texture Manager]

The next step is to import your custom skin into the TM. In my case, it's the Scarlet Crusader. [To import the skin, right-click inside the TM window, and click "Import"]

The path for the Scarlet Crusader is "footman.blp".

After importing your custom skin, you should open the Material Manager [Windows > Material Manager].

Now, open all the Materials, and then open the Material Layer.

Now, you should replace the Texture ID. In the screenshot above, you should see the "Replaceable ID 1"; you should not do anything there (neither in the 0 or 2). So just press cancel and move to another Material Layer.

And you also shouldn't change the "Textures\gutz.blp" aswell.

Once you find the models's standard skin, you should change it to use your custom skin.

Once you make it, you'll note the changes in your model:

This isn't done yet, that's why you should check all of the Material Layers in all of the Materials.

Once you're done, the model will be using your custom skin.

You're done! Now just save the model with the name you want! (be sure to add .mdx at the end of the name, e.g. ScarletFootman.mdx)

Remember:

You should also do this in the "_Portrait", otherwise, the ingame portrait won't show up, or it will be bugged, and save the portrait with the same name of your model, with the "_Portrait" in addition.

You can also remove the standard texture, to reduce the filesize. That's optional.

You should import the custom models and the custom skin that you used for the model, otherwise, the green box will show up.


 Last edited: Apr 8, 2015

#### Sylvie

#### Sylvie

 Level 11

JoinedDec 14, 2014Messages550

Resources: 5

#### Resources
Icons 1Skins 1Maps 1Templates 1Tutorials 1

All feedback is welcome!

#### Forsaken

#### Forsaken

 Level 23

JoinedApr 5, 2012Messages1,837

Resources: 15

#### Resources
Maps 4Templates 11

Good job!

#### WhiteFang

#### WhiteFang

 Level 21

JoinedJul 6, 2014Messages6,786

This is a very usefull tutorial.I didn't know about this.It solved my sea giant skin problem.Good going and thank you.

#### Sclammerz

#### Sclammerz

Patron

 Level 25

JoinedApr 12, 2014Messages1,893

Resources: 4

#### Resources
Skins 1Maps 3

##
 Legacy of the Third War


I think there is already a tutorial in this kind of stuff but hey, seems very useful.

#### Sylvie

#### Sylvie

 Level 11

JoinedDec 14, 2014Messages550

Resources: 5

#### Resources
Icons 1Skins 1Maps 1Templates 1Tutorials 1

Sclammerz said:

 I think there is already a tutorial in this kind of stuff but hey, seems very useful.


Click to expand...

Yeah, but all of them was hard to understand, at least for me, and needed of multiple tools. This requires only one.


 Last edited: Feb 18, 2015

#### Sclammerz

#### Sclammerz

Patron

 Level 25

JoinedApr 12, 2014Messages1,893

Resources: 4

#### Resources
Skins 1Maps 3

##
 Legacy of the Third War


Sylvie said:

 Yeah, but all of them was hard to understand, at least for me, abd beeded of multiple tools. This requires only one.


Click to expand...


I think so too.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

You forgot to add that you shouldn't change 'gutz.blp' as well, but nice tutorial.

#### Sylvie

#### Sylvie

 Level 11

JoinedDec 14, 2014Messages550

Resources: 5

#### Resources
Icons 1Skins 1Maps 1Templates 1Tutorials 1

Mythic said:

 You forgot to add that you shouldn't change 'gutz.blp' as well, but nice tutorial.


Click to expand...


Done, thanks!

#### Heinvers

#### Heinvers

 Level 53

JoinedMay 7, 2010Messages9,267

Resources: 101

#### Resources
Models 3Icons 65Packs 3Skins 27Templates 3

Don't forget to remove the old texture once you modify the model materials to use the new one.

btw it's Portrait.

#### Sylvie

#### Sylvie

 Level 11

JoinedDec 14, 2014Messages550

Resources: 5

#### Resources
Icons 1Skins 1Maps 1Templates 1Tutorials 1

Heinvers said:

 Don't forget to remove the old texture once you modify the model materials to use the new one.

btw it's Portrait.


Click to expand...

That's actually optional, that's not really necessary, but I'll put it in. Thanks!

Oh, well... Thanks

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

Nice tutorial, easy to grasp. Thanks for sharing.

The footman model really looks shitty even with better texture lol.

#### Sclammerz

#### Sclammerz

Patron

 Level 25

JoinedApr 12, 2014Messages1,893

Resources: 4

#### Resources
Skins 1Maps 3

##
 Legacy of the Third War


How do I remove those images on her feet? :3

#### Sylvie

#### Sylvie

 Level 11

JoinedDec 14, 2014Messages550

Resources: 5

#### Resources
Icons 1Skins 1Maps 1Templates 1Tutorials 1

@Sclammerz: You probably replaced the hero glow texture with her skin. Remember, don't do anything the the "Replace ID". Neither the 0, 1 or 2.

#### Sclammerz

#### Sclammerz

Patron

 Level 25

JoinedApr 12, 2014Messages1,893

Resources: 4

#### Resources
Skins 1Maps 3

##
 Legacy of the Third War


​

Sylvie said:

 @Sclammerz: You probably replaced the hero glow texture with her skin. Remember, don't do anything the the "Replace ID". Neither the 0, 1 or 2.


Click to expand...


Ok, Thanks. noted.

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

Sclammerz said:

 [IMG]https://www.hiveworkshop.com/forums/pastebin_data/xezt7g/Something.jpg[/IMG]


Click to expand...

Image already deleted?

#### Sclammerz

#### Sclammerz

Patron

 Level 25

JoinedApr 12, 2014Messages1,893

Resources: 4

#### Resources
Skins 1Maps 3

##
 Legacy of the Third War


chobibo said:

 Image already deleted?


Click to expand...


yep, it is solve now and I don't want to store unneeded items at my paste bin anymore.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I generally find MDX Pather to be quicker than changing the actual materials (as demonstrated in this tutorial), but I feel like having another one of these tutorials isn't detrimental. This method is reasonably quick, and allows for some interesting modularity in cases where you want to keep some geosets textured as one thing and others as another. I'm sure people will appreciate knowing how to edit materials.

I'm going to make a few quick edits to fix some spelling. Approved!

EDIT: I made some minor clarifications and fixed a bit of grammar, and I moved it to our 3D Modeling Tutorial repository. Good job with the tutorial!

#### Sylvie

#### Sylvie

 Level 11

JoinedDec 14, 2014Messages550

Resources: 5

#### Resources
Icons 1Skins 1Maps 1Templates 1Tutorials 1

Oh thanks!

Sorry for the typos tho..

#### daverave1212

#### daverave1212

 Level 11

JoinedApr 9, 2010Messages425

Resources: 8

#### Resources
Skins 2Tools 2Maps 3Tutorials 1

Oh wow, nice, i never got around that model editor anyway

#### Adamus

#### Adamus

 Level 7

JoinedOct 2, 2014Messages27

Resources: 4

#### Resources
Reforged HD Models 4

Great Tutorial it worked for the units, I tried for heroes particular the dreadlord it doesn't show the character at all he is the invisible dreadlord lol.. Do these steps apply to heroes as well? Thanks

Flux said:

 It should work on any types. Maybe you did something wrong.

This tutorial does the same but a LOT easier in my opinion (requires extra tool). if you are still struggling, have a look at it.


Click to expand...

Great thanks that did work and very shorter and easier. On a side note i achieved a victory mwhahaha i figured out my mistake with this tutorial its a fine point so they both work lol and now both are really easy thanks Flux, and Sylvie!!


 Last edited: Sep 29, 2015

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Adamus said:

 Great Tutorial it worked for the units, I tried for heroes particular the dreadlord it doesn't show the character at all he is the invisible dreadlord lol.. Do these steps apply to heroes as well? Thanks


Click to expand...


It should work on any types. Maybe you did something wrong.

This tutorial does the same but a LOT easier in my opinion (requires extra tool). if you are still struggling, have a look at it.

#### DarkBlueLacky

#### DarkBlueLacky

 Level 3

JoinedNov 18, 2015Messages41

Thanks.

#### Ardenaso

#### Ardenaso

HD Model Reviewer

 Level 35

JoinedJun 22, 2013Messages2,047

Resources: 23

#### Resources
Tools 1Reforged HD Models 19Reforged HD Skins 3

There are times when it fails even when the skin used is present at the Import Editor; do I also have to change the import path?

#### YetAnotherYoutuber

#### YetAnotherYoutuber

 Level 8

JoinedJun 9, 2015Messages240

i dont get it

changed both material layers that had textures/footman to the custom texture, left gutz be, then saved as .mdx

imported into editor, along with the original model, editor says -could not load file-

the model of unit in wme looks like it should, but editor wont load and use it

am i understanding it right that you, after saving the model, import it along with original model, and then select the model you created in the -art model file- as a custom one for the footman?

#### Ardenaso

#### Ardenaso

HD Model Reviewer

 Level 35

JoinedJun 22, 2013Messages2,047

Resources: 23

#### Resources
Tools 1Reforged HD Models 19Reforged HD Skins 3

You have done the Scarlet Footman, you should also do the Scarlet Captain

I did the Scarlet Captain and it still won't work.

#### Epic Producer

#### Epic Producer

 Level 5

JoinedApr 13, 2018Messages124

Sylvie , i have a question. how to find its path after its done?

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Check the Texture Manager. Usually, the path is x.blp where x is the filename.

#### Epic Producer

#### Epic Producer

 Level 5

JoinedApr 13, 2018Messages124

Thank you!

Just a request, why cant I make this skin ZL_ForestSeeker.blp work? I followed the path but warden just remained warden.

(NOTE= Im not making it multiple skins one model. Just original import skin...)

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

I started to have doubt the path might be wrong there. You might want to check if it's Maiev's or Warden's texture.

#### za Republiku

#### za Republiku

 Level 1

JoinedJan 27, 2020Messages2

I changed the look of the jaina to BloodElfEnchantress but at startup I see only some shadow how to fix it ?

#### Ardenaso

#### Ardenaso

HD Model Reviewer

 Level 35

JoinedJun 22, 2013Messages2,047

Resources: 23

#### Resources
Tools 1Reforged HD Models 19Reforged HD Skins 3

Hoping to see a Reforged version with screenshots

#### Em-Rock

#### Em-Rock

 Level 1

JoinedFeb 11, 2019Messages2

Sylvie said:

How to add multiple skins for a single model.

For this tutorial, you'll need of a single tool: War3 Model Editor, by Magos.

For this tutorial, I'll be using the Scarlet Crusader, by CloudWolf.

First of all, open the WME, and open its MPQ Browser [Windows > MPQ Browser]. Select War3.mpq (for Reign of Chaos units) or the War3x.mpq (for The Frozen Throne units) [MPQ Browser > File > War3.mpq or War3x.mpq].

The next step is to find the model that you wish to have the skin replaced. For me, the Footman is that unit.

When the model is opened, you should open the Texture Manager [Windows > Texture Manager]

The next step is to import your custom skin into the TM. In my case, it's the Scarlet Crusader. [To import the skin, right-click inside the TM window, and click "Import"]

The path for the Scarlet Crusader is "footman.blp".

After importing your custom skin, you should open the Material Manager [Windows > Material Manager].

Now, open all the Materials, and then open the Material Layer.

Now, you should replace the Texture ID. In the screenshot above, you should see the "Replaceable ID 1"; you should not do anything there (neither in the 0 or 2). So just press cancel and move to another Material Layer.

And you also shouldn't change the "Textures\gutz.blp" aswell.

Once you find the models's standard skin, you should change it to use your custom skin.

Once you make it, you'll note the changes in your model:

This isn't done yet, that's why you should check all of the Material Layers in all of the Materials.

Once you're done, the model will be using your custom skin.

You're done! Now just save the model with the name you want! (be sure to add .mdx at the end of the name, e.g. ScarletFootman.mdx)

Remember:

You should also do this in the "_Portrait", otherwise, the ingame portrait won't show up, or it will be bugged, and save the portrait with the same name of your model, with the "_Portrait" in addition.

You can also remove the standard texture, to reduce the filesize. That's optional.

You should import the custom models and the custom skin that you used for the model, otherwise, the green box will show up.

Click to expand...

I have a problem : I did exactly what you told us to do in this tuto, however, when I import my custom model in the Map Editor, the custom skin is not showing up. It replaced by a white stuff.What can I do to solve this problem ? Do you know if it's because of the Reforged or not ?

#### Ardenaso

#### Ardenaso

HD Model Reviewer

 Level 35

JoinedJun 22, 2013Messages2,047

Resources: 23

#### Resources
Tools 1Reforged HD Models 19Reforged HD Skins 3

if it's Reforged I think you need to use Retera Model Studio

#### Em-Rock

#### Em-Rock

 Level 1

JoinedFeb 11, 2019Messages2

Ardenaso said:

 if it's Reforged I think you need to use Retera Model Studio


Click to expand...

Alright thanks, I'll give a try.

 You must log in or register to reply here.


### Similar threads

 Geomerging


eubz

Apr 21, 2012

3D Modeling Tutorials

Replies19Views32K

Jan 22, 2019
Amigoltu

 Creating Ghosts


Daelin

Jul 14, 2005

3D Modeling Tutorials

Replies10Views27K

Aug 14, 2023
DREADMAN

 Geoset Importing and Exporting Guide


-Peper-

Oct 30, 2010

3D Modeling Tutorials

Replies31Views38K

Apr 14, 2012
-Peper-

 How to make cool portraits without needing a separate file


Spacebuns

Sep 5, 2014

3D Modeling Tutorials

Replies11Views11K

Nov 27, 2017
Diegoit

[Misc]
 Importing a custom skin without overwriting the default one


gabriono

Mar 3, 2017

Tutorial Archive

Replies1Views2K

May 7, 2017
IcemanBo

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

3D Modeling Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top