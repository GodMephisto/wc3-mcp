---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/creating-doodads-in-gmax-simple-grass.201569/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Creating Doodads in Gmax Simple Grass
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/creating-doodads-in-gmax-simple-grass.201569/>
#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

 grass


##

 MODELING DOODADS IN GMAX

### (Simple Grass)


I suppose that you already know how to save in-game textures using the image extractors and also that you have the knowledge in exporting models using dexporter. I will not be including those things here because I know that there are already tutorials about them here in the hive.

Requirements:


1. Gmax, of course.


2. Basic gmax knowledge;


3. Knowledge in saving in-game textures;


4. Knowledge in exporting models from gmax using dexporter.


If you don't know those three things, then you better look for them and read about them. I am sure there are tutorials for them that are already present here.

## TOPICS:
1. Creating the Plane


2. Texturing the Plane


3. Creating the new Plane


Let us start.


1. Save some in-game textures to use in our grass model. In this tutorial I will use Felwood Natural.tga.


2. Open your gmax software.

## CREATING THE PLANE
1. Click the front view port. In the "create geometry" roll out, in the keyboard entry, type the length: 50 cm, width: 50 cm. In the parameters type in the length segs 1 and in the width segs 1. Click create.


2. In the modify panel, right-click Plane then click convert to editable mesh.


3. Click the Gmax Material navigator. A window will appear similar to the one below.


Click file system and find the texture that you have saved earlier. In this case I use Felwood Natural.tga. Drop the texture to the plane that you have created. Close the Gmax Material Navigator.


4. So far, you have created a plane to where we will start creating our grass model. We also have identified the texture to use for our model. So, the next step will be the texturing process for our model.

## TEXTURING THE PLANE
1. In the modify panel, modifier list, click Unwrap UVW under the UV Coordinate Modifiers. We now have the new modifier, the Unwrap UVW.


2. Now, click the + sign in the Unwrap UVW, then click Select Face. In the parameters, click edit. The Edit UVWs window appears.


3. Box select all the points. Click scale. Now, point the cursor to any of the points, left-click and drag to scale it.


4. Now, click the "move" command button. Point the cursor to any of the points (the points should be red in color) then, hold and drag it to the grass drawing. If the rectangle for UV mapping still doesn't fit the drawing or still needs some work, you have to edit it so your work will look like the picture below.


5.close the Unwrap UVWs Window. The plane should now looks like the picture below.


6. Click "select face" to end the command.


So we have come up with the texturing process and our work is at 50% completion. Now, we move on.

## CREATING THE NEW PLANE
So far, we only have a single plane for our model. That is not a good model. To continue, we will create another plane from the existing one.


1. Click the Select and Rotate Command Button. In the front view, select the Y axis. Hold Shift and start rotating the plane by holding the Y axis (left clicking it) downward. A clone option window will pop up as soon as you release the mouse. Just click "OK". We now have two planes crossing each other.


2. Select both planes in the front view. Click the select and move command buttons. Position the cursor in the Y axis and pull the model upward until it looks like the picture below.


Below is our model ready for exporting...


At this point, you can already export the model, and after exporting and converting it to .mdx file to use it in game. In our tutorial, I only use two planes for the grass but you can create more if you'd like.


Below is our final model:

#### Attachments

edit uv window.jpg

 86.5 KB

 · Views: 1,797



End of UVW unwrapping.jpg

 87.2 KB

 · Views: 1,885



final model.JPG

 19.7 KB

 · Views: 1,808



material navigator.jpg

 33.3 KB

 · Views: 1,804



ready for export.jpg

 100.6 KB

 · Views: 2,047



unwrap UV.jpg

 151.3 KB

 · Views: 311



unwrapping done.jpg

 141.9 KB

 · Views: 1,791



adjusting height.jpg

 102.3 KB

 · Views: 1,797




 Last edited: Nov 1, 2011

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1


 eubz said:



 [color="light blue"]So, these the things that you need here:[/color]


Click to expand...

haha...

its too simple I think... but maybe useful...

btw, I was thinking why the cattail and the green background did not show on the model...

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

Adiktuz said:

 haha...


its too simple I think... but maybe useful...


btw, I was thinking why the cattail and the green background did not show on the model...


Click to expand...


Well, it has already been edited.

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

Adiktuz said:

 btw, I was thinking why the cattail and the green background did not show on the model...


Click to expand...


it is affected by the filtermode in the exporting process...

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

A nice simple tutorial. It is certainly useful for those who want to make simple, yet effective grasses. Afterall, we can always use more shrubberies.


I will fix up the grammar and detach the signature and it will be ready for approval.

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

I do hope that this simple tutorial will help those who seek simple modelling tutorials.

#### stonneash

#### stonneash

 Level 26

JoinedJul 20, 2011Messages1,273

Resources: 140

#### Resources
Models 47Icons 74Packs 5Skins 8Spells and Systems 3Tutorials 3

pretty good,but is there a tut that show how to convert{gmax-->mdx\mdl}


i used so many scriots but none worked

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

stonneash said:

 pretty good,but is there a tut that show how to convert{gmax-->mdx\mdl}


i used so many scriots but none worked


Click to expand...


you need to read: https://www.hiveworkshop.com/forums/3d-modeling-tutorials-282/gmax-exporting-tutorial-95857/


and you need to download:

https://www.hiveworkshop.com/forums/tools-560/mdlx-converter-62991/

#### stonneash

#### stonneash

 Level 26

JoinedJul 20, 2011Messages1,273

Resources: 140

#### Resources
Models 47Icons 74Packs 5Skins 8Spells and Systems 3Tutorials 3

thnx alot dude ur epic but i yet have a prob......

my uvw doesn't work,it goes random please help

 You must log in or register to reply here.


### Similar threads

 Creating Attachment Weapon in Gmax using SPLines


eubz

Nov 7, 2011

3D Modeling Tutorials

Replies17Views20K

Apr 14, 2012
Tompoolk

 GMax Exporting Tutorial


AirHead

Aug 22, 2008

3D Modeling Tutorials

Replies10Views14K

Jan 18, 2009
Pomegranate

P

 Teamcolor Particles


Anvil

Nov 28, 2012

3D Modeling Tutorials

Replies4Views7K

Dec 19, 2012
Anvil

 Geomerging


eubz

Apr 21, 2012

3D Modeling Tutorials

Replies19Views32K

Jan 22, 2019
Amigoltu

 Basics of Special Effects (SFX)


Daelin

Nov 21, 2006

3D Modeling Tutorials

Replies1Views30K

Nov 21, 2006
Archian

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