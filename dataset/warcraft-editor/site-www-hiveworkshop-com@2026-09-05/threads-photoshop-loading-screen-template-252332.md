---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/photoshop-loading-screen-template.252332/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Photoshop Loading Screen Template
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/photoshop-loading-screen-template.252332/>
#### Frotty

#### Frotty

 Level 23

JoinedJan 1, 2009Messages1,622

Resources: 13

#### Resources
Models 3Tools 1Maps 7Tutorials 1Wurst 1

Hi, I made a photoshop template for creating Loadingscreen quick & easy.

1. Download the .psd file from the attachments and open it in Photoshop

2. Insert your loadingscreen like so:

3. Use photoshop's "Save for Web" function:


Drag-Select all slices so there are no grey areas left:


Press Save... and save with these settings to a folder of your choice:


Your files will be inside an "images" folder inside of your chosen folder:

4. Convert the .png files to .blp by using your favourite converter (I used blplab-batch converter with these settings):

5. Import the files into your map using the import-editor. Replace war3mapImported\ with UI\Glues\Loading\Load-Generic\

6. Set Loading Screen to Generic


And you're done!


Pros over the model-version:

No model file needed

Smaller filesize

Players don't need to press a key

#### Attachments

loadingscreen-template.psd

 87.4 KB

 · Views: 483



1.png

 287.4 KB

 · Views: 1,696



2.png

 8.4 KB

 · Views: 1,480



3.png

 510.3 KB

 · Views: 1,538



4.png

 31.4 KB

 · Views: 1,479



5.png

 66.9 KB

 · Views: 1,414



6.png

 13.9 KB

 · Views: 1,502



7.png

 6.8 KB

 · Views: 1,362



8.png

 7.3 KB

 · Views: 1,407




 Last edited by a moderator: Aug 6, 2014

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

very good and useful, I will make sure to try this.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great.

Images have to be attached to the hive. Since there aren't that many, I might do it for you soonTM.

#### Frotty

#### Frotty

 Level 23

JoinedJan 1, 2009Messages1,622

Resources: 13

#### Resources
Models 3Tools 1Maps 7Tutorials 1Wurst 1

PurgeandFire said:

 Great.


Images have to be attached to the hive. Since there aren't that many, I might do it for you soonTM.


Click to expand...


Ah, the pain of forum attachments.


Would be great if you could do it for me, but I guess I can as well.

#### GywGod133

#### GywGod133

 Level 13

JoinedJul 16, 2012Messages679

Resources: 14

#### Resources
Icons 6Maps 4Spells and Systems 2Tutorials 2

 my map

#### EarthSouvenir

#### EarthSouvenir

 Level 2

JoinedNov 11, 2013Messages19

Why don't you put the converter too? so other people won't wastin time to search for it..

no offense...

~ Fairy ~

#### yogaprad

#### yogaprad

 Level 5

JoinedSep 8, 2013Messages143

i cant see the screenshot

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I attached the images to the hive.


Approved.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

Cool tutorial, all along I thought those bottom/upper right textures were the metal stuff in the campaign loading screen corners.

#### Frotty

#### Frotty

 Level 23

JoinedJan 1, 2009Messages1,622

Resources: 13

#### Resources
Models 3Tools 1Maps 7Tutorials 1Wurst 1

PurgeandFire said:

 I attached the images to the hive.


Approved.


Click to expand...


Thanks


You saved my lazy ass

#### Heinvers

#### Heinvers

 Level 53

JoinedMay 7, 2010Messages9,267

Resources: 101

#### Resources
Models 3Icons 65Packs 3Skins 27Templates 3

Thank you for this deer Frotty

May the wurst be with you!

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

http://en.wikipedia.org/wiki/Conchita_Wurst

#### Frotty

#### Frotty

 Level 23

JoinedJan 1, 2009Messages1,622

Resources: 13

#### Resources
Models 3Tools 1Maps 7Tutorials 1Wurst 1

#### AlwaysAndForever

#### AlwaysAndForever

 Level 11

JoinedJun 20, 2017Messages380

Resources: 3

#### Resources
Maps 3

Awesome

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

For now the image must be composed/created with a 4:3 aspect ratio in mind. Additionally one can probably use non power of 2 textures for a higher resolution loading screen.

#### Frotty

#### Frotty

 Level 23

JoinedJan 1, 2009Messages1,622

Resources: 13

#### Resources
Models 3Tools 1Maps 7Tutorials 1Wurst 1

Pretty sure that would break backwards compatibility with older versions? If it even works.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Frotty said:

 Pretty sure that would break backwards compatibility with older versions? If it even works.


Click to expand...

As far as I am aware all versions of Warcraft III supported non power of 2 textures. This is because the support is entirely up to the underlying graphic driver, and all modern graphic cards (eg the trash one gets integrated into a processor or the lowest end budget card from 10 years ago) and drivers support non power of 2 textures.

Textures larger than 512 in any dimension are only supported by recent versions of Warcraft III. (EDIT: this was reverted in an even more recent patch as of 1.30)


 Last edited: Mar 28, 2019

#### Frotty

#### Frotty

 Level 23

JoinedJan 1, 2009Messages1,622

Resources: 13

#### Resources
Models 3Tools 1Maps 7Tutorials 1Wurst 1

Dr Super Good said:

 As far as I am aware all versions of Warcraft III supported non power of 2 textures. This is because the support is entirely up to the underlying graphic driver, and all modern graphic cards (eg the trash one gets integrated into a processor or the lowest end budget card from 10 years ago) and drivers support non power of 2 textures.

Textures larger than 512 in any dimension are only supported by recent versions of Warcraft III.


Click to expand...

I'm not talking about pot, but previous versions being rendered in 4:3 and stretched. So even if it would work, it wouldn't look good. (I would think)

perhaps with the custom model it could work?

#### Frotty

#### Frotty

 Level 23

JoinedJan 1, 2009Messages1,622

Resources: 13

#### Resources
Models 3Tools 1Maps 7Tutorials 1Wurst 1

The template is 4:3 ..

#### Edy

#### Edy

 Level 9

JoinedNov 21, 2015Messages226

Resources: 1

#### Resources
Maps 1

Can I have a download link for the photoshop ur using, can't find any free photoshops and adobe is trash imo.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Edy said:

 Can I have a download link for the photoshop ur using, can't find any free photoshops and adobe is trash imo.


Click to expand...

Not possible. Photoshop licence is not free. You might be able to get it at a discount as a student however.

Free alternatives include paint.net as well as the highly recommended GIMP.

#### Edy

#### Edy

 Level 9

JoinedNov 21, 2015Messages226

Resources: 1

#### Resources
Maps 1

Dr Super Good said:

 Not possible. Photoshop licence is not free. You might be able to get it at a discount as a student however.

Free alternatives include paint.net as well as the highly recommended GIMP.


Click to expand...

Thx for the info

#### Frotty

#### Frotty

 Level 23

JoinedJan 1, 2009Messages1,622

Resources: 13

#### Resources
Models 3Tools 1Maps 7Tutorials 1Wurst 1

photoshop is an adobe product, you will not find it anywhere else.

I'm using an older version in these screenshots.

I don't know if the existing photoshop alternatives (such as gimp) can read and export the slices template properly.

#### Anglo Warior

#### Anglo Warior

 Level 7

JoinedDec 28, 2011Messages145

Resources: 4

#### Resources
Icons 4

What are the pixel sizes of those slices? I'd love to use this method but I do not have photoshop, so I'd need to chop the image manually.

Edit: Nevermind, Figured it out.

For those wondering:

The full image should be 1024 x 768

The top panels should be 512 x 512

The bottom panels should be 512 x 256


 Last edited: Jan 1, 2020

 You must log in or register to reply here.


### Similar threads

 High Resolution Loading Screens


Mayday

Feb 7, 2020

2D Art Tutorials
2

Replies54Views27K

Jan 7, 2026
Elprede

 Custom Loading Screen Tutorial V1.5


Sopho

May 8, 2004

2D Art Tutorials

Replies24Views80K

Sep 2, 2022
dantles1992

Locked

 loading screen quality


Reager

Jul 3, 2007

World Editor Help Zone

Replies10Views2K

Jul 4, 2007
Reager

 [WIP] Using Photoshop Brushes


Elfsilver Lord

Dec 19, 2008

2D Art Tutorials

Replies4Views7K

Dec 26, 2008
Pyritie

Locked

 Request: Loading Screen


Develyngyr

Jul 20, 2014

Requests

Replies4Views1K

Jul 21, 2014
andreariona

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

2D Art Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top