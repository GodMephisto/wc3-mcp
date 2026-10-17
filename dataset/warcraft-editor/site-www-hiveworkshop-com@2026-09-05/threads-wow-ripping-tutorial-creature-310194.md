---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/wow-ripping-tutorial-creature.310194/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Wow Ripping Tutorial Creature
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/wow-ripping-tutorial-creature.310194/>
#### Power

#### Power

 Level 17

JoinedJan 18, 2009Messages291

Resources: 9

#### Resources
Models 8Tutorials 1

DELETED


 Last edited: Jul 6, 2025

#### deepstrasz

#### deepstrasz

Map Reviewer

 Level 84

JoinedJun 4, 2009Messages22,555

Resources: 7

#### Resources
Maps 4Spells and Systems 2Sounds 1

Not sure if this helps or not but I'm gonna link this post from another threadt here: click.

Not sure about external sites for links. Wouldn't uploading those programs in your pastebin (of Hiveworkshop) and linking them be better?

Power said:

 Don't panic it's easy , just delete these lines so the file need to start start with MD20 ALWAYS!


Click to expand...

Why?

Power said:

 When you open them , you have to delete the first 7 Bytes of the Anim , just select them and hit the DEL button. Save it and do it for every Anim File of the model you want to rip.


Click to expand...

What do these things actually do? Why are they required?

#### YourArthas

#### YourArthas

 Level 18

JoinedMar 18, 2014Messages760

Resources: 4

#### Resources
Models 1Reforged HD Models 2Ported Models 1

##
 Warcraft III: Rebirth


Another note: After converting model be sure to export textures AGAIN otherwise alpha channels will be messed up. And sometimes models have wrong normals cause of mdlvis , you can select right ones with mdlvis and if it has none , try importing it with neodex and export again with option fix normals checked (it mostly works).

First 7 bytes thing is because converter was not updated to Legion 7.3 and the new 'headers' are added to those bytes in patch 7.3

Also note that this is for fully ripping a model with all it's animations you can preview all of them and then delete unneeded ones and rename animations as you wish when you make the model ready for your map

#### Power

#### Power

 Level 17

JoinedJan 18, 2009Messages291

Resources: 9

#### Resources
Models 8Tutorials 1

deepstrasz said:

 Not sure if this helps or not but I'm gonna link this post from another threadt here: click.

Not sure about external sites for links. Wouldn't uploading those programs in your pastebin (of Hiveworkshop) and linking them be better?

Why?

What do these things actually do? Why are they required?


Click to expand...

The link you provided with tutorials is outdated, they won't work anymore.

Second , i will move the downloads into pastebin as soon is possible.

And Last , if you don't delete them, when you convert the model and open it in MDLVIS the animations are going to be bugged and the model is going to be out of use. The converter was programmed to convert models with MD20 at start, but blizzard updated their m2 models and now you have to delete the first 7 bytes.

#### deepstrasz

#### deepstrasz

Map Reviewer

 Level 84

JoinedJun 4, 2009Messages22,555

Resources: 7

#### Resources
Maps 4Spells and Systems 2Sounds 1

Power said:

 The link you provided with tutorials is outdated, they won't work anymore.


Click to expand...

Power said:

 And Last , if you don't delete them, when you convert the model and open it in MDLVIS the animations are going to be bugged and the model is going to be out of use. The converter was programmed to convert models with MD20 at start, but blizzard updated their m2 models and now you have to delete the first 7 bytes.


Click to expand...

So, that doesn't affect the models at all? That's awesome, that they still work.

#### YourArthas

#### YourArthas

 Level 18

JoinedMar 18, 2014Messages760

Resources: 4

#### Resources
Models 1Reforged HD Models 2Ported Models 1

##
 Warcraft III: Rebirth


deepstrasz said:

So, that doesn't affect the models at all? That's awesome, that they still work.


Click to expand...

Yes it is just a recently added header to .anim files just think like model's are not called as 'Warcraft III' model but as 'Warcraft III Reforged' model, if you keep the added 'reforged' converter could not understand it well and messes up animations but if you delete it they are all fine

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Great work! I'll invite more people here so they can leave feedback.

My suggestions so far:

Use the word "export" instead of "ripping".

Don't link to other sites for downloads. The images and tools used should be on the Hive. Simply upload it to our pastebin (and make them last 'forever') and link to it. We don't want broken links in the future etc.

These 3 link to other sites:
Casc Explorer : WoW-Tools/CASCExplorer

jm2converter : jm2converter.jar

010 Edtor : 010 Editor 7.0.2 x64.rar

Use SPOILER tags for the images.

EXAMPLE

The formatting could be improved a bit e.g. use headlines.

You should also explain how to add attachment points
@PurgeandFire@FerSZ@General Frank

Power said:

 If you want i can make a video too!


Click to expand...

Please do

YourArthas said:

 Another note: After converting model be sure to export textures AGAIN otherwise alpha channels will be messed up.


Click to expand...

This should probably be in the tutorial


 Last edited: Nov 15, 2018

#### Power

#### Power

 Level 17

JoinedJan 18, 2009Messages291

Resources: 9

#### Resources
Models 8Tutorials 1

Archian , i didnt added the jm2 converter and casc explorer because these programs are always updated by their creators , so i believe is going to be better if i let them like this , also today i'm traveling all day so i won't be able to update the tutorial , But after i reach my destination i will update it the best i can , but please i need feedback , i want peoples to try it .

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

images are broken/not loading for me, i'll take a closer look when they're available. Looks promising, just needs some formatting and spelling/grammar fixes

#### deepstrasz

#### deepstrasz

Map Reviewer

 Level 84

JoinedJun 4, 2009Messages22,555

Resources: 7

#### Resources
Maps 4Spells and Systems 2Sounds 1

PurgeandFire said:

 images are broken/not loading for me, i'll take a closer look when they're available. Looks promising, just needs some formatting and spelling/grammar fixes


Click to expand...

@Power just needs to take a look at the BB codes to get the idea how writing works on Hiveworkshop: BB Codes | HIVE

#### Ether

#### Ether

 Level 14

JoinedNov 13, 2017Messages733

Resources: 7

#### Resources
Skins 7

@Power

Hello, I just read the tutorial and it looks very good. Anyways, I followed this and an error when 40-50% of CDN index building loads...

Any troubleshooting advice here? Maybe someone else might experience this problem.

#### YourArthas

#### YourArthas

 Level 18

JoinedMar 18, 2014Messages760

Resources: 4

#### Resources
Models 1Reforged HD Models 2Ported Models 1

##
 Warcraft III: Rebirth


Battleborn said:

 Any troubleshooting advice here? Maybe someone else might experience this problem.


Click to expand...

Online feature is like this sadly try again and again till it works, or use your installed WoW

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Image attachments are broken.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Images are indeed broken.

But, more importantly I think there are too many of them.

For example, I do not think we need a picture how to right click somewhere.

But if you insist, put it in a hidden tag.

Because I feel like the format of:

1 line of text > image > 1 line of text

is a bit annoying to read to be honest

#### Power

#### Power

 Level 17

JoinedJan 18, 2009Messages291

Resources: 9

#### Resources
Models 8Tutorials 1

Guys i'm for vacation in Germany , i will fix it when i get back , for the moment i don't have time. Sorry you have to wait a little . Also i need some feedback from you guys , i need you to try the tutorial.

#### Zordrak

#### Zordrak

 Level 8

JoinedDec 19, 2014Messages300

Resources: 1

#### Resources
Maps 1

Is it also necessary to have WoW for ripping? Or can the WoW model files also be downloaded somewhere?

#### YourArthas

#### YourArthas

 Level 18

JoinedMar 18, 2014Messages760

Resources: 4

#### Resources
Models 1Reforged HD Models 2Ported Models 1

##
 Warcraft III: Rebirth


Zordrak said:

 Is it also necessary to have WoW for ripping? Or can the WoW model files also be downloaded somewhere?


Click to expand...

Well, you don't need it but it is better if you have WoW cause Cascexplorer's online feature is abit fucked up

#### Zordrak

#### Zordrak

 Level 8

JoinedDec 19, 2014Messages300

Resources: 1

#### Resources
Maps 1

YourArthas said:

 Well, you don't need it but it is better if you have WoW cause Cascexplorer's online feature is abit fucked up


Click to expand...

Ok thanks. But i think there are already all good WoW models ripped and available. So i have decided to import all already useable WoW models. I can highly recommend the map 'Human Error' (here on Hive) for that matter. You can export the models from that map.

#### Hermit

#### Hermit

 Level 22

JoinedMay 29, 2013Messages1,573

Resources: 8

#### Resources
Models 4Icons 3Sounds 1

Power said:

 open the folder of your model and there you're going to see 4 different file types.

M2 , Skin, BLP and Anim File


Click to expand...

What if there are no Anim files in the folder?

#### YourArthas

#### YourArthas

 Level 18

JoinedMar 18, 2014Messages760

Resources: 4

#### Resources
Models 1Reforged HD Models 2Ported Models 1

##
 Warcraft III: Rebirth


Hermit said:

 What if there are no Anim files in the folder?


Click to expand...

No problem at all, m2's have animations but if you add .anim files this is kinda saying 'hey we added more animations in a later patch and m2's limitations were not enough to handle 100 animations so we added a .anim file'

#### Hermit

#### Hermit

 Level 22

JoinedMay 29, 2013Messages1,573

Resources: 8

#### Resources
Models 4Icons 3Sounds 1

YourArthas said:

 No problem at all, m2's have animations but if you add .anim files this is kinda saying 'hey we added more animations in a later patch and m2's limitations were not enough to handle 100 animations so we added a .anim file'


Click to expand...

Thanks for the info, but now I have another problem.

Power said:

 And Just hit ENTER.

After the convert is finished youre going to see the results.


Click to expand...

What results am I supposed to see?

Power said:

 Open the m2 model with mdlvis


Click to expand...

I tried two different versions of MdlVis, but neither can open m2 files.

#### Attachments

MDLVISERROR.jpg

 21.8 KB

 · Views: 1,689



#### YourArthas

#### YourArthas

 Level 18

JoinedMar 18, 2014Messages760

Resources: 4

#### Resources
Models 1Reforged HD Models 2Ported Models 1

##
 Warcraft III: Rebirth


Hermit said:

 Thanks for the info, but now I have another problem.

What results am I supposed to see?

I tried two different versions of MdlVis, but neither can open m2 files.

Click to expand...

Dıd you 'rip' the model first?

#### Hermit

#### Hermit

 Level 22

JoinedMay 29, 2013Messages1,573

Resources: 8

#### Resources
Models 4Icons 3Sounds 1

YourArthas said:

 Dıd you 'rip' the model first?


Click to expand...

You mean the Command Prompt thing? I did that, but I don't know if it even did anything.

#### YourArthas

#### YourArthas

 Level 18

JoinedMar 18, 2014Messages760

Resources: 4

#### Resources
Models 1Reforged HD Models 2Ported Models 1

##
 Warcraft III: Rebirth


Hermit said:

 You mean the Command Prompt thing? I did that, but I don't know if it even did anything.


Click to expand...

Probably you did something wrong, you must convert it to '-bc' then mdlvis would open it, also which model is it?

#### Hermit

#### Hermit

 Level 22

JoinedMay 29, 2013Messages1,573

Resources: 8

#### Resources
Models 4Icons 3Sounds 1

YourArthas said:

 Probably you did something wrong


Click to expand...

I don't think so; maybe the jm2converter.jar doesn't work.

YourArthas said:

 you must convert it to '-bc'


Click to expand...

?

YourArthas said:

 also which model is it?


Click to expand...

The crab mob. I know it's already ripped and uploaded on Warcraft Underground; I just wanted to try this out myself.

#### Attachments

CommandPrompt.jpg

 81.5 KB

 · Views: 1,726




 Last edited: Dec 11, 2018

#### YourArthas

#### YourArthas

 Level 18

JoinedMar 18, 2014Messages760

Resources: 4

#### Resources
Models 1Reforged HD Models 2Ported Models 1

##
 Warcraft III: Rebirth


Hermit said:

 I don't think so; maybe the jm2converter.jar doesn't work.

?

The crab mob. I know it's already ripped and uploaded on Warcraft Underground; I just wanted to try this out myself.


Click to expand...

Erm this one you tried is a really old model it might be cause of it, I'll try myself now to find out what is wrong

Edit: It worked to me did the model have everything exported, .skin and .m2 and .blps?

#### Hermit

#### Hermit

 Level 22

JoinedMay 29, 2013Messages1,573

Resources: 8

#### Resources
Models 4Icons 3Sounds 1

YourArthas said:

 did the model have everything exported, .skin and .m2 and .blps?


Click to expand...

Yes, I exported all files from the Crab folder.

#### YourArthas

#### YourArthas

 Level 18

JoinedMar 18, 2014Messages760

Resources: 4

#### Resources
Models 1Reforged HD Models 2Ported Models 1

##
 Warcraft III: Rebirth


Hermit said:

 Yes, I exported all files from the Crab folder.


Click to expand...

can you try this one, it worked for me if it is not working it is probably because of jmconverter(corrupted or something)

edit: also do not delete m2 header of this, this one is an old model it has the right header

#### Attachments

crab.rar

 515 KB

 · Views: 186




 Last edited: Dec 11, 2018

#### Hermit

#### Hermit

 Level 22

JoinedMay 29, 2013Messages1,573

Resources: 8

#### Resources
Models 4Icons 3Sounds 1

YourArthas said:

 can you try this one


Click to expand...

I get this error when I try to open it in Mdlvis:

YourArthas said:

 also do not delete m2 header of this, this one is an old model it has the right header


Click to expand...

How do I know if it's an old model? This should probably be mentioned in the tutorial.

#### Attachments

ERROR.jpg

 11.8 KB

 · Views: 1,657



ERROR2.jpg

 13 KB

 · Views: 1,657



#### YourArthas

#### YourArthas

 Level 18

JoinedMar 18, 2014Messages760

Resources: 4

#### Resources
Models 1Reforged HD Models 2Ported Models 1

##
 Warcraft III: Rebirth


Hermit said:

 I get this error when I try to open it in Mdlvis:

How do I know if it's an old model? This should probably be mentioned in the tutorial.


Click to expand...

So weird... also header if model has md20 header it does not need a deletion

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Any updates for this?

#### YourArthas

#### YourArthas

 Level 18

JoinedMar 18, 2014Messages760

Resources: 4

#### Resources
Models 1Reforged HD Models 2Ported Models 1

##
 Warcraft III: Rebirth


Archian said:

 Any updates for this?

Click to expand...

Henry is abit busy with his real life, but I'm kinda always here to help out/explain stuff

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Any plans to update this?

Images are broken (attach them to the post) and it would seem there are issues that needs fixing with this tutorial.

#### Ether

#### Ether

 Level 14

JoinedNov 13, 2017Messages733

Resources: 7

#### Resources
Skins 7

CascExplorer look for Net Frameworks and Net Frameworks says my op system is not compatible. 64-bit I am why?

#### Duke

#### Duke

 Level 10

JoinedNov 23, 2006Messages592

Resources: 1

#### Resources
Maps 1

Hey, I got trouble with extracting a model I would really like to have, in case there was anyone knowledgeable about the subject and willing to help, please let me know

#### YourArthas

#### YourArthas

 Level 18

JoinedMar 18, 2014Messages760

Resources: 4

#### Resources
Models 1Reforged HD Models 2Ported Models 1

##
 Warcraft III: Rebirth


Duke said:

 Hey, I got trouble with extracting a model I would really like to have, in case there was anyone knowledgeable about the subject and willing to help, please let me know


Click to expand...

Which model was it?

#### Duke

#### Duke

 Level 10

JoinedNov 23, 2006Messages592

Resources: 1

#### Resources
Maps 1

YourArthas said:

 Which model was it?


Click to expand...

It is a wicker golem model. The strange thing is that it works in Retera's model studio with animations and everything, but nowhere else. I have attached the file, would be really glad if you could tell me how to fix it so I can deal with if it happens next time.

Retera's studio:

Magos (error message with mdl, with mdx it jsut shuts off):

#### Attachments

wicker.rar

 4.4 MB

 · Views: 176



#### deepstrasz

#### deepstrasz

Map Reviewer

 Level 84

JoinedJun 4, 2009Messages22,555

Resources: 7

#### Resources
Maps 4Spells and Systems 2Sounds 1

@Retera, could you make your software work for WoW too as seen above?

#### YourArthas

#### YourArthas

 Level 18

JoinedMar 18, 2014Messages760

Resources: 4

#### Resources
Models 1Reforged HD Models 2Ported Models 1

##
 Warcraft III: Rebirth


Duke said:

 It is a wicker golem model. The strange thing is that it works in Retera's model studio with animations and everything, but nowhere else. I have attached the file, would be really glad if you could tell me how to fix it so I can deal with if it happens next time.

Retera's studio:

View attachment 334341

Magos (error message with mdl, with mdx it jsut shuts off):

View attachment 334342

Click to expand...

I got this thing already but I'll fix it for you now, it is probably about animated textures and paths that's why it gives that error,

here you go but you need to export and convert fresh textures because they were broken also the fixing this issue is pretty simple, open the model with mdlvis optimize it save it as mdl, open and optimize it again after that magos will be able to open it you'll need to change the particles, they use replaceable ID 0 because of something Blizzard changed recently, I used a dust texture instead of it, edit them as you wish change colors sizes etc.

@deepstrasz Your suggestion would be a pretty cool addition and lifesaver to be honest since nearly each patch m2 format gets changes by Blizzard, Retera can actively update the tool too.

#### Attachments

wicker.rar

 2.2 MB

 · Views: 140




 Last edited: Oct 5, 2019

#### Duke

#### Duke

 Level 10

JoinedNov 23, 2006Messages592

Resources: 1

#### Resources
Maps 1

YourArthas said:

 I got this thing already but I'll fix it for you now, it is probably about animated textures and paths that's why it gives that error,

here you go but you need to export and convert fresh textures because they were broken also the fixing this issue is pretty simple, open the model with mdlvis optimize it save it as mdl, open and optimize it again after that magos will be able to open it you'll need to change the particles, they use replaceable ID 0 because of something Blizzard changed recently, I used a dust texture instead of it, edit them as you wish change colors sizes etc.

@deepstrasz Your suggestion would be a pretty cool addition and lifesaver to be honest since nearly each patch m2 format gets changes by Blizzard, Retera can actively update the tool too.


Click to expand...

Thanks a lot for fixing the model!

"open the model with mdlvis optimize it save it as mdl, open and optimize it again after that magos will be able to open it"

I have just tried to do that (open mdx in Mdlvis -> optimize -> save as mdl -> optimize -> save) but magos still gives the same error. Is there any other step?

#### YourArthas

#### YourArthas

 Level 18

JoinedMar 18, 2014Messages760

Resources: 4

#### Resources
Models 1Reforged HD Models 2Ported Models 1

##
 Warcraft III: Rebirth


Duke said:

 Thanks a lot for fixing the model!

"open the model with mdlvis optimize it save it as mdl, open and optimize it again after that magos will be able to open it"

I have just tried to do that (open mdx in Mdlvis -> optimize -> save as mdl -> optimize -> save) but magos still gives the same error. Is there any other step?


Click to expand...

After saving as mdl close it, open it again and optimzie again that did it for me also check the linearize animations option too when optimizing.

#### Duke

#### Duke

 Level 10

JoinedNov 23, 2006Messages592

Resources: 1

#### Resources
Maps 1

YourArthas said:

 After saving as mdl close it, open it again and optimzie again that did it for me also check the linearize animations option too when optimizing.


Click to expand...

That helped, thanks again!

#### KyraDragoness

#### KyraDragoness

 Level 1

JoinedNov 25, 2019Messages1

Greetings,

First, thanks a lot for the tutorial, it works wonders.

Yet I have a question : as the pictures are missing, I can't properly go through this step :

 Select them all and open them with 010 Editor!

When you open them , you have to delete the first 7 Bytes of the Anim


Click to expand...

I can't figure how a byte is actually depicted i.e. is a byte here a whole line, or just one doublet, or whatever ?

For now I can rip all models that don't have .anim files nevertheless it would be awesome to be able to rip the others.

Thank you in advance

#### Ether

#### Ether

 Level 14

JoinedNov 13, 2017Messages733

Resources: 7

#### Resources
Skins 7

KyraDragoness said:

 Greetings,

First, thanks a lot for the tutorial, it works wonders.

Yet I have a question : as the pictures are missing, I can't properly go through this step :

I can't figure how a byte is actually depicted i.e. is a byte here a whole line, or just one doublet, or whatever ?

For now I can rip all models that don't have .anim files nevertheless it would be awesome to be able to rip the others.

Thank you in advance


Click to expand...

Use powerful text editor and open the .m2 model file and delete the first 7 characters/space. The file should start with something like MD20 instead of MD21.

#### Azsure

#### Azsure

 Level 14

JoinedDec 24, 2008Messages2,129

Resources: 8

#### Resources
Models 7Maps 1

I don't know if the OP is still active here, but I'd appreciate if the attached pictures were re-uploaded/fixed.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Graveyarded until updated.

Primarily the broken images, the other issues are minor in comparison.

#### RanchBurner

#### RanchBurner

 Level 11

JoinedJul 3, 2010Messages242

Resources: 12

#### Resources
Skins 4Maps 1StarCraft II Resources 4Ported Skins 2Tutorials 1

Hey does anyone know if this can be done with Matrix Eater 3D? And does it matter if I extract the .m2 file from CASCExplorer or CASCView?

#### Razorclaw_X

#### Razorclaw_X

 Level 22

JoinedApr 12, 2018Messages494

CASC Explorer and CASC View are functionally no different from each other when it comes to extracting files.

#### RanchBurner

#### RanchBurner

 Level 11

JoinedJul 3, 2010Messages242

Resources: 12

#### Resources
Skins 4Maps 1StarCraft II Resources 4Ported Skins 2Tutorials 1

Razorclaw_X said:

 CASC Explorer and CASC View are functionally no different from each other when it comes to extracting files.


Click to expand...

 I didn't think so either.

Next, what if we did not want all those animations per model? We'd probably only need on average ten, eleven, or twelve out of 300+ animations.

#### RanchBurner

#### RanchBurner

 Level 11

JoinedJul 3, 2010Messages242

Resources: 12

#### Resources
Skins 4Maps 1StarCraft II Resources 4Ported Skins 2Tutorials 1

Power said:

 Guys i'm for vacation in Germany , i will fix it when i get back , for the moment i don't have time. Sorry you have to wait a little . Also i need some feedback from you guys , i need you to try the tutorial.


Click to expand...

Yeah, looks like you then forgot about it for over two years. You really should fix those tutorial images so people can follow along better. Or is there a video tutorial on YouTube?

 You must log in or register to reply here.


### Similar threads

Locked

 Tutorial for ripping models required


Wayshan

Mar 24, 2013

World Editor Help Zone

Replies1Views1K

Mar 24, 2013
Arhowk

Locked

 Wow rips?


wowcraft

Jan 25, 2024

Site Discussion

Replies15Views2K

Jan 31, 2024
Lt_Hawkeye

Locked

 Wow rips?


Thedraes

Jul 1, 2015

Site Discussion

Replies38Views7K

Aug 4, 2015
Zwiebelchen

Locked

 Map Uses WoW Ripped Models


neo_sluf

Mar 2, 2013

Site Discussion

Replies1Views2K

Mar 2, 2013
-Kobas-

Locked

 WoW Rips?


ndh

May 25, 2012

Requests

Replies2Views2K

May 25, 2012
ndh

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