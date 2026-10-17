---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/crash-course-on-ripping-sc2-models.330190/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Crash Course on Ripping Sc2 Models
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/crash-course-on-ripping-sc2-models.330190/>
#### Boar

#### Boar

 Level 13

JoinedOct 10, 2009Messages439

Resources: 2

#### Resources
Maps 1Tutorials 1

The Programs You Will Need

3ds Studio Max ( I'm using the latest )

Taylor Mouse's M3 Importer ( This is how we import sc2/hots stuff into 3ds max )

Install by placing the script in C\Program Files\Autodesk\3ds Max 2020\scripts\Startup

Neodex 2.7 ( Download link at the bottom )

Starcraft 2 or Heroes of the Storm ( Or both! )

Casc Viewer ( Extract Hots and Sc2 files )

Retera's Model Studio ( Reforged Hack )

Homework before we begin

First of all go grab all the above programs and install them.

Once you've done that give yourself a pat on the back; we've done the easiest step and now we get to do the hardest part. Extracting Sc2 / Hots files with casc.

Go launch yourself the Casc viewer, navigate to where you installed Sc2 and open up the Casc MPQ. We'll need all these files for later ( when we view them via m3 importer ), so go ahead and extract them to a place you'll remember. There are a lot of files and this will take some time....

Great! Now on to the easy stuff.​

The Hard Stuff

Okay I might have lied about the hard stuff being done. Time to rip those models out of the casc storage we just saved.

Open up 3ds Studio Max and navigate to the right side panel > MaxScript > M3 Import

If you've installed everything correctly you should see something like this:

Click on Import... and navigate around until you find your desired Sc2 / Hots model. Once you've found it slam that like, subscribe and +rep button to import it!

So I went ahead and imported fusion core from base.sc2assets/assets/buildings/terran and ended up getting this:

Depending on the mood of the blizzard model creator at the time, you'll be greeted with either a blank looking model, a weird looking model or if you're lucky, a textured one.

Don't panic if you have a non-textured or weirdly textured model. That's what the Material Editor is for and oddly enough, the next step of this tutorial.​

Help my model needs textures!

Skip this step if your model is already textured, or has a weird repeating texture.

Strap on in, here's the fun part! Open up the Material Editor

Using the Material Editor, create a Warcraft 3 Material by searching and dragging it onto the palette

And now you'll need to find the texture file Sc2 uses for the model by yourself. My best advice is to search through the Casc files you extracted looking for ${YourModelName}_diff.dds.

But once you do find it, drag and drop it onto the Material Editor and then connect it with the Warcraft 3 Material we made earlier

This tells the renderer ( and the exporter ) to create a material using these textures. Remember where these textures are from! You'll have to include them with the exported model otherwise you'll get a textureless, gray mesh.

There's various settings you can tinker with on the right hand side, but that's up to you to mess around with.

Using 3ds max, select the part of the model you want to apply the texture to and then in the Material Editor select Assign Material To Selection

This will tell the meshes you have selected to use the chosen Material... and you're done. Next step!​

Crash Course On Texturing

Does your texture repeat strangely, have seams or doesn't match the mesh? Read this simple step to fix it. Doctors hate him! Forewarning, this is a fairly complex stage and I'll be going through it quickly to keep the guide simple. Feel free to experiment and read further tutorials online

In 3ds max select the mesh that has a strange texture, in the modifiers panel at the top right you'll need to add "Unwrap UVW". This lets us manually control where the texture is applied

Once you've done that, click on Open UV Editor. You'll be greeted with something that looks like this

If your texture file is blank ( The square in the background ), you just have to select the material you made at the top right ( where the screenshot has Bitmaptextu... ). If you can't find your material there, that means you never applied it to the mesh. Go back and do that.

Our goal here is to move the mesh around so it aligns with the texture file. Just experiment with moving it around and the various settings until you get it looking right! Read some more indepth, specialized online tutorial if you need help on this step.

Bam! Once you've got the mesh lined up and looking nicely let's go on to the next stage.

Exporting The Model​

Wow, look at that. The model is looking great! There's a few more things you can do in 3ds max to improve how it looks ( tinkering with materials, cleaning up animations ) but let's just export it and get it working in Warcraft 3.

At the top of the screen select Neodex > Export Wc3 Model

Name the model something witty or relevant and for now, keep the export settings the default options

And then export it!

Once you're done you'll have the model you just ripped sitting at a mdx file. Just be aware you'll need to include the textures you are using in the same directory you saved the file.

And you're done! You can just import your model and textures into Warcraft 3 as you would with anything else.​

Troubleshooting

Oh boy, you ran into a problem? Let me short list some fixes here.

I can't open the model in warcraft 3!​

​

Yeah for some reason this happens. Just open the model you created with Retera's Model Studio and save it. RMS does some weird magic that fixes any model corruptions. Don't ask me how, just bless @Retera​

My model's animations are super jank!

In the Neodex menu you can tinker around with how it imports animations. My best advice would be to slap all the buttons in Neodex > Animation Tools to see if it fixes it. Jank / low fps animations can be fixed by setting the controllers Neodex uses to Linear​

​

#### Attachments

upload_2021-1-26_10-1-34.png

 46.7 KB

 · Views: 141



upload_2021-1-26_10-7-0.png

 20.7 KB

 · Views: 135



upload_2021-1-26_10-11-55.png

 33.1 KB

 · Views: 139




 Last edited by a moderator: Jun 6, 2022

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Titles should use the [h3] or [h2] tags, OR at least apply bold to them to stand out more. I like the orange color tho.

Some of the larger images should be put into hidden tags.

Bonus points for intending text like this after a title

​
edit, probably good show a picture of the end product.

#### Boar

#### Boar

 Level 13

JoinedOct 10, 2009Messages439

Resources: 2

#### Resources
Maps 1Tutorials 1

Fixed, how's that looking?

#### Symphoneum

#### Symphoneum

 Level 29

JoinedAug 14, 2020Messages263

Resources: 37

#### Resources
Maps 1Reforged HD Models 20Ported Models 15Reforged HD Skins 1

Hi, I've been trying to use your tutorial and it worked pretty well but after exporting all of the animations look like the picture I've attached. The bones are in the right positions but I think there might be a skinweight issue. Can you help me? Thanks!

#### Boar

#### Boar

 Level 13

JoinedOct 10, 2009Messages439

Resources: 2

#### Resources
Maps 1Tutorials 1

Symphoneum said:

 Hi, I've been trying to use your tutorial and it worked pretty well but after exporting all of the animations look like the picture I've attached. The bones are in the right positions but I think there might be a skinweight issue. Can you help me? Thanks!View attachment 390171

Click to expand...

Is that the HOTS Deathwing? Yeah, some of the newer models don't export well with Tyler's exporter. And I'm not a modelling wizard enough to really know how to fix it.

What I've done to fix some weird animation weights before is:

Boar said:

My model's animations are super jank!

In the Neodex menu you can tinker around with how it imports animations. My best advice would be to slap all the buttons in Neodex > Animation Tools to see if it fixes it. Jank / low fps animations can be fixed by setting the controllers Neodex uses to Linear


Click to expand...

And in the case of the one Defiler model I use, I had to completely rebone and reanimate him myself to fix the problem.

#### Symphoneum

#### Symphoneum

 Level 29

JoinedAug 14, 2020Messages263

Resources: 37

#### Resources
Maps 1Reforged HD Models 20Ported Models 15Reforged HD Skins 1

Boar said:

 Is that the HOTS Deathwing? Yeah, some of the newer models don't export well with Tyler's exporter. And I'm not a modelling wizard enough to really know how to fix it.

What I've done to fix some weird animation weights before is:

And in the case of the one Defiler model I use, I had to completely rebone and reanimate him myself to fix the problem.


Click to expand...

Yeah, it is the HoTS Deathwing. I've been having trouble with every model so far. This is the closest I've gotten to a clean model, though! I may just end up reassigning the matrix manually for each bone. Thanks for the tips! I will try them out

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Replaced header underline with bold, added indents for all the sections.

Approved, always like tutorials with pictures.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

Is there no alternative method to 3DS Max?

#### Symphoneum

#### Symphoneum

 Level 29

JoinedAug 14, 2020Messages263

Resources: 37

#### Resources
Maps 1Reforged HD Models 20Ported Models 15Reforged HD Skins 1

Tal'Rarity said:

 Is there no alternative method to 3DS Max?


Click to expand...

While this is probably the most reliable method there is also a very good blender m3 addon and there is also an excellent mdl exporter by @twilac. You should be able to port sc2 models to wc3 using that method, too.

#### twilac

#### twilac

 Level 12

JoinedNov 9, 2020Messages23

Resources: 9

#### Resources
Models 8Ported Models 1

Tal'Rarity said:

 Is there no alternative method to 3DS Max?


Click to expand...

###
 GitHub - SC2Mapster/m3addon: Blender Addon to import and export m3 files


Blender Addon to import and export m3 files. Contribute to SC2Mapster/m3addon development by creating an account on GitHub.

 github.com


###
 GitHub - tw1lac/mdl-exporter at 3.4


Warcraft MDL exporter for Blender. Contribute to tw1lac/mdl-exporter development by creating an account on GitHub.

 github.com


 You must log in or register to reply here.


### Similar threads

 Maya and 3ds Max 5 Integration [Reviewed: Ghan]


anetherond

Jun 7, 2008

3D Modeling Tutorials

Replies3Views8K

Feb 18, 2009
Skylark

S

 How to edit models- a complete guide


Horn

Oct 4, 2004

3D Modeling Tutorials

Replies5Views49K

Dec 1, 2013
animation31

 Creating Ghosts


Daelin

Jul 14, 2005

3D Modeling Tutorials

Replies10Views27K

Aug 14, 2023
DREADMAN

 Geomerging


eubz

Apr 21, 2012

3D Modeling Tutorials

Replies19Views32K

Jan 22, 2019
Amigoltu

 Basic 3D S.Max 5 - Chapter 1: Modelling and UW-Map


xXMephistoXx

Jan 9, 2008

3D Modeling Tutorials

Replies17Views16K

Sep 13, 2017
xXMephistoXx

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