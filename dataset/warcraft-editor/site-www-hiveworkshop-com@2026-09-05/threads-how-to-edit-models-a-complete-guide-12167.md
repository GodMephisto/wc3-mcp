---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-edit-models-a-complete-guide.12167/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Edit Models a Complete Guide
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-edit-models-a-complete-guide.12167/>
#### Horn

#### Horn

 Level 15

JoinedMar 31, 2004Messages860

Resources: 20

#### Resources
Models 14Icons 1Skins 3Tutorials 2

In this tutorial I will be using 3DS Max 5, Wordpad, Yobgul's File converter, and although it isn't required, Image Viewer 2 to admire my results outside Max.


Firstly, decide what you want to merge and then plan to isolate the various models and their parts.


I will be taking a Troll trapper model, and giving it an Axe, whilst removing its existing weapons. Then I will go on to change its animations in a further tutorial.

Stage 1. Getting me a big rusty Axe!


Load up 3DS Max.


Click on the utilities button:


The scroll down and select MAXscript:


Find the script the MDX Importer/Exporter script by KMK. Attached if you do not have this:

DOWNLOAD HERE


Then you will have to select it from the drop down box:


And you will be presented with the script's functions:


Select them as so:


Then you can click on import and find the mdx (model) you want to import and edit/use. Here is my Spirit Walker who's Axe I'll be stealing:


Isn't he cute? We'll see about that.


Notice the model is made up of random coloured meshes. Delete all the meshes that aren't the same colour as the object you want to steal/edit. In this case, the Axe. Just click on them and press delete.


Don't worry about deleting the other bits, you are only going to use the data for the Axe when you're done with this stage.


Now, select this mesh, and click on modify:


Expand the editable mesh by clicking the small +button:


And then select Vertex:


Now, your model should be covered in small blue dots, which are its vertices.


Now you want to start deleting all the parts of mesh that you do not want. It may be easier/quicker to use polygon/element to select and delete these areas, instead of vertex. Also, make sure you click on remove isolated vertices:


And you should end up with just your required object, which in this case is a Big Axe!


Now, export the model using 'File > Export'. Export as .3ds for now, as you will need to position the Axe on the next model before making it a .mdx.


 Last edited by a moderator: Mar 30, 2007

#### Horn

#### Horn

 Level 15

JoinedMar 31, 2004Messages860

Resources: 20

#### Resources
Models 14Icons 1Skins 3Tutorials 2

How to edit models - Part 2, who gets the axe

Using the step-by-step instructions in part 1, import your main model. In this case it is a Troll Trapper.


Again, delete any meshes you wont be editing, such as the corpse.


Now, I am going to shrink the small axes into oblivion. Click on a mesh. Select modify and go to vertex edit. You MUST use vertex edit.


Select the vertices of one axe:


Then select the scale tool:


Return to your vertices, and shrink them to oblivion. Sometimes there will be another mesh underneath. Just repeat this process.


Now, just to be safe, move the vertices inside the parent area, ie the hand:


Repeat this for all objects you want to hide on that mesh. Then export the mesh using the script, and repeat this process on the other mesh. I save them as 'edit1', 'edit2' etc.


Generally there are two main body meshes. One is for Team colour, and the other for transparency (ie two meshes with two different material properties). Some models have more, many more.


You should have a edited model and an axe ready to position. if you dont, import the Axe using 'file > import'.


Now, select the Axe, go to modify and vertex edit. Select all the vertices (Ctrl + A), and choose the Move tool. Now you can move the axe into position, in the troll's hand. This must be done in vertex mode:


Now you can export the axe using the script, as .mdx.


You now have 2 edited mesh files, and an axe file, all in mdx. You no longer need to use 3DS max for this basic tutorial. Proceed to stage 3.


 Last edited by a moderator: Mar 31, 2007

#### Horn

#### Horn

 Level 15

JoinedMar 31, 2004Messages860

Resources: 20

#### Resources
Models 14Icons 1Skins 3Tutorials 2

How to edit models - Part 3, editing mdl data

You need to get a copy of Yobgul's converter for this stage.


Convert all files being used from .mdx to .mdl.


In this example, the files are axe.mdl, edit1.mdl, edit2.mdl and ForestTrollTrapper.mdl.


First, load up edit1.mdl.


Scroll down to the geoset data:


Select from before vertices down to the normals:


Copy this data, and open up ForestTrollTrapper.mdl.


Do search (Ctrl + F) and paste the copied data , and it should find the geoset as it looks for number of vertices, which is the first line. Then select the same area of data you copied, from vertices to normals, and replace with the copied data from edit1.


Save the data, and if you want to check the result, convert the ForestTrapper back to mdx and look at the model in image viewer.


Repeat this stage with edit2.mdl and any further edits of meshes on the original model you made.


Now we will actually merge a new geoset, the Axe, to the edited model.


Load up axe.mdl and select from Geoset, all the way down to the bracket after selectiongroup:


Copy this data and load up the troll file. Do a search for 'bone':


Paste the copied data before the word bone, but after the bracket on the line above.


Next you need to find which bone is the right hand, so do a search for 'hand'.


You want to note the Object ID of the bone. You can also use the Parent number, which is probably more reliable.


Return to your added geoset by scrolling up, and change the number for matrices to the bone ID you just found.


This should now make your axe geoset move with the right hand. Best to save, convert and check that it does before continuing.


Note, it is generally quicker/easier to enter the matrice, material ID and even a comment name for your geoset before you paste it into the main file.


To add a comment, type // before it.


 Last edited by a moderator: Mar 30, 2007

#### Horn

#### Horn

 Level 15

JoinedMar 31, 2004Messages860

Resources: 20

#### Resources
Models 14Icons 1Skins 3Tutorials 2

How to edit models - Part 4, final touches and notes

Continuing from part 3, we need to set-up a material and texture for the new geoset.


Near the top of the file, seach for Textures.


Add 1 to the quantity (ie textures 5).


Add a new texture to the end of the previous textures. Before the closed bracket that leads onto Materials:


You can copy an existing texture line, or write it from scratch. Just ensure you use { and } and not ( ) or [ ].


Write the texture filepath as you wish. For this example, I will just point it towards the spirit walker, as that's where the Axe is from.


Experts may refer to the UVW Mapping tutorial if they wish to add the axe to the troll texture, so that you dont need to use 2 textures.


Next, look down at the Materials:


Similar to textures, add 1 to the total, and add a new material after the current materials. For a standard, alpha-able texture, use the following data:


The texture ID must correspond with the texture you just added. Counting from 0, find what texture you added, and enter the number. this is generally 1 less than the total.


Finally, find your geoset again, and search for Material ID. Enter the ID for the material you added. Again, counting from 0. And agagin, generally 1 less than the total number of materials.


This should give your axe its own material and texture. Save and convert to mdx to check.

Usually this should complete your geoset merge and give you a finished, edited model. However, this particular model has a few twists that need to be changed.


You will want to check out my other tutorials for further editing and fixing of your edited models.


 Last edited by a moderator: Mar 30, 2007

#### shadowslayer133

#### shadowslayer133

 Level 4

JoinedJan 1, 2007Messages29

Resources: 2

#### Resources
Maps 2

 I Made A Donut Using This!

*Sigh* but my donut doesn't work


 Last edited: Jan 17, 2007

#### animation31

#### animation31

 Level 1

JoinedDec 1, 2013Messages3

how to import and play the animations of the imported model?

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

Locked

 not enough data on 3ds max tuts


waaaks!

Jul 7, 2007

Modeling & Animation

Replies2Views1K

Jul 10, 2007
Deleted member 126647

D

 Milkshape Attachment Modeling


HappyTauren

Feb 15, 2008

3D Modeling Tutorials
2

Replies64Views37K

Nov 23, 2018
Amigoltu

 Geoset Importing and Exporting Guide


-Peper-

Oct 30, 2010

3D Modeling Tutorials

Replies31Views38K

Apr 14, 2012
-Peper-

 How to Swap Reforged Model Parts


Retera

Oct 30, 2021

3D Modeling Tutorials

Replies20Views9K

Mar 12, 2023
FatCatTrap

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